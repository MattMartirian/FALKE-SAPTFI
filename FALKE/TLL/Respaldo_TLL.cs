using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using ORM;
using SERVICES;
using TE;

namespace TLL
{
    // Copias de seguridad de la base de datos. Los archivos quedan en el servidor de base de datos: acá no se descargan,
    // solo se generan, se listan y se restauran, y cada paso queda en la bitácora.
    public class Respaldo_TLL
    {
        public const string MODULO = "Respaldos";
        public const string PALABRA_CONFIRMACION = "RESTAURAR";

        // Mientras se restaura, el resto de las pantallas se desvía a "Mantenimiento".
        private static int restauracionEnCurso;

        public static bool RestauracionEnCurso
        {
            get { return Volatile.Read(ref restauracionEnCurso) == 1; }
        }

        private readonly RespaldoRepository respaldoRepo;
        private readonly Respaldos_ORM servidor;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;

        public Respaldo_TLL()
        {
            respaldoRepo = new RespaldoRepository();
            servidor = new Respaldos_ORM();
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
        }

        // Del más reciente al más antiguo. ArchivoDisponible dice si el archivo sigue en el servidor.
        public List<Respaldo_TE> Listar(ActorUsuario_TE actor)
        {
            Exigir(actor, Patentes_TLL.VER_RESPALDOS);

            List<Respaldo_TE> lista = respaldoRepo.ObtenerTodos();

            foreach (Respaldo_TE r in lista)
            {
                r.ArchivoDisponible = servidor.ArchivoExiste(r.Ruta);

                if (!r.IdUsuario.HasValue || string.IsNullOrWhiteSpace(r.NombreUsuario)) r.NombreUsuario = "Sin usuario (cuenta de emergencia)";
            }

            return lista;
        }

        // Copia completa de la base, verificada al terminar. Devuelve el registro de la copia.
        public Respaldo_TE Generar(ActorUsuario_TE actor)
        {
            Exigir(actor, Patentes_TLL.HACER_RESPALDO);

            if (RestauracionEnCurso) throw new InvalidOperationException("Hay una restauración en curso: esperá a que termine.");

            DateTime ahora = DateTime.Now;
            string ruta = servidor.RutaNueva(ahora);

            for (int numero = 2; respaldoRepo.ExisteRuta(ruta); numero++) ruta = servidor.RutaNueva(ahora, numero);

            long? tamano;

            try
            {
                tamano = servidor.Respaldar(ruta);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("No se pudo generar la copia de seguridad. El servidor respondió: " + ex.Message, ex);
            }

            var respaldo = new Respaldo_TE
            {
                IdUsuario = actor.IdUsuario > 0 ? actor.IdUsuario : (int?)null,
                FechaGeneracion = ahora,
                Ruta = ruta,
                Tamano = tamano
            };

            // El archivo ya existe: si el registro o la auditoría fallan, se revierten juntos.
            Transaccion_ORM.Ejecutar(() =>
            {
                respaldoRepo.Alta(respaldo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Backup, new[] { respaldo.IdRespaldo.ToString() });

                bitacora.Guardar(new Bitacora_TE(actor.IdUsuario, MODULO, "Copia de seguridad generada: " + respaldo.NombreArchivo + ".", CriticidadBitacora.Media, DateTime.Now));
            });

            return respaldo;
        }

        // Reemplaza toda la base por la de la copia elegida. Quien llama debe cerrar las sesiones al terminar.
        public void Restaurar(ActorUsuario_TE actor, int idRespaldo, string confirmacion)
        {
            Exigir(actor, Patentes_TLL.RESTAURAR_RESPALDO);

            if (!string.Equals((confirmacion ?? string.Empty).Trim(), PALABRA_CONFIRMACION, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Para confirmar, escribí " + PALABRA_CONFIRMACION + ".");

            Respaldo_TE respaldo = respaldoRepo.ObtenerPorPK(idRespaldo);

            if (respaldo == null) throw new InvalidOperationException("El respaldo no existe.");
            if (!servidor.ArchivoExiste(respaldo.Ruta)) throw new InvalidOperationException("El archivo de ese respaldo ya no está en el servidor.");

            // Antes de tocar nada se comprueba que el archivo se pueda leer: una copia dañada no debe destruir la base actual.
            try
            {
                servidor.Verificar(respaldo.Ruta);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("El archivo del respaldo está dañado o no se puede leer: no se restauró nada. " + ex.Message, ex);
            }

            string quien = DescribirActor(actor);

            if (Interlocked.CompareExchange(ref restauracionEnCurso, 1, 0) != 0)
                throw new InvalidOperationException("Ya hay una restauración en curso.");

            try
            {
                // La restauración devuelve la tabla de respaldos al estado de la copia: se guarda lo que había para no perder el registro de las posteriores.
                List<Respaldo_TE> registrados = respaldoRepo.ObtenerTodos();

                try
                {
                    servidor.Restaurar(respaldo.Ruta);
                }
                catch (SqlException ex)
                {
                    throw new InvalidOperationException("No se pudo restaurar la base. El servidor respondió: " + ex.Message, ex);
                }

                Reincorporar(registrados);
                AuditarRestauracion(actor, quien, respaldo);
            }
            finally
            {
                Volatile.Write(ref restauracionEnCurso, 0);
            }
        }

        // Las copias hechas después de la restaurada siguen existiendo como archivos: se vuelven a registrar para poder usarlas.
        private void Reincorporar(List<Respaldo_TE> registrados)
        {
            try
            {
                var existentes = new HashSet<string>(respaldoRepo.ObtenerTodos().Select(r => r.Ruta), StringComparer.OrdinalIgnoreCase);

                foreach (Respaldo_TE r in registrados.OrderBy(x => x.FechaGeneracion))
                {
                    if (existentes.Contains(r.Ruta)) continue;

                    respaldoRepo.Alta(new Respaldo_TE { IdUsuario = r.IdUsuario, FechaGeneracion = r.FechaGeneracion, Ruta = r.Ruta, Tamano = r.Tamano });
                }

                gestorIntegridad.RecalcularTabla(TablasBD.Backup);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Respaldo.Reincorporar", ex);
            }
        }

        // El evento se escribe en la base ya restaurada. Si el usuario no existía cuando se hizo la copia, el evento queda sin usuario y se nombra en el texto.
        private void AuditarRestauracion(ActorUsuario_TE actor, string quien, Respaldo_TE respaldo)
        {
            try
            {
                int idUsuario = actor.IdUsuario > 0 && new Usuario_TLL().ObtenerPorId(actor.IdUsuario) != null ? actor.IdUsuario : 0;

                string descripcion = "Restauración de la base de datos desde la copia " + respaldo.NombreArchivo + " del " + respaldo.FechaGeneracion.ToString("dd/MM/yyyy HH:mm") +
                    ". Se cerraron las sesiones abiertas." + (idUsuario == 0 ? " La hizo " + quien + "." : string.Empty);

                bitacora.Guardar(new Bitacora_TE(idUsuario, MODULO, descripcion, CriticidadBitacora.Alta, DateTime.Now));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Respaldo.Auditar", ex);
            }
        }

        private static string DescribirActor(ActorUsuario_TE actor)
        {
            if (actor.EsEmergencia || actor.IdUsuario <= 0) return "la cuenta de emergencia";

            Usuario_TE usuario = new Usuario_TLL().ObtenerPorId(actor.IdUsuario);

            return usuario == null ? "el usuario " + actor.IdUsuario : (usuario.NombreUsuario + " " + usuario.ApellidoUsuario).Trim() + " (" + usuario.EmailUsuario + ")";
        }

        private void Exigir(ActorUsuario_TE actor, string patente)
        {
            if (actor != null && actor.Puede(patente)) return;

            bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", "Acción rechazada por falta de permiso (" + patente + ")", CriticidadBitacora.Media);
            throw new UnauthorizedAccessException("No tenés permiso para realizar esta acción.");
        }
    }
}
