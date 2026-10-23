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
    public class Respaldo_TLL
    {
        public const string MODULO = "Respaldos";
        public const string PALABRA_CONFIRMACION = "RESTAURAR";
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

        public List<Respaldo_TE> Listar(ActorUsuario_TE actor)
        {
            actor.Exigir(Patentes_TLL.VER_RESPALDOS);

            List<Respaldo_TE> lista = respaldoRepo.ObtenerTodos();

            foreach (Respaldo_TE r in lista)
            {
                r.ArchivoDisponible = servidor.ArchivoExiste(r.Ruta);

                if (!r.IdUsuario.HasValue || string.IsNullOrWhiteSpace(r.NombreUsuario)) r.NombreUsuario = BitacoraGestor_TLL.ActorSinUsuario();
            }

            return lista;
        }

        // Copia completa de la base, verificada al terminar. Devuelve el registro de la copia.
        public Respaldo_TE Generar(ActorUsuario_TE actor)
        {
            actor.Exigir(Patentes_TLL.HACER_RESPALDO);

            if (RestauracionEnCurso) throw new ExcepcionTraducible_TLL("respaldo_en_curso");

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
                throw new ExcepcionTraducible_TLL("respaldo_error_generar", ex.Message);
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

                bitacora.Guardar(new Bitacora_TE(actor.IdUsuario, MODULO, EventosBitacora_TLL.RESPALDO_GENERADO, CriticidadBitacora.Media, DateTime.Now, 0, DetalleBitacora_TLL.Armar(respaldo.NombreArchivo)));
            });

            return respaldo;
        }

        // Reemplaza toda la base por la de la copia elegida. Quien llama debe cerrar las sesiones al terminar.
        public void Restaurar(ActorUsuario_TE actor, int idRespaldo, string confirmacion)
        {
            actor.Exigir(Patentes_TLL.RESTAURAR_RESPALDO);

            if (!string.Equals((confirmacion ?? string.Empty).Trim(), PALABRA_CONFIRMACION, StringComparison.OrdinalIgnoreCase))
                throw new ExcepcionTraducible_TLL("respaldo_confirmar", PALABRA_CONFIRMACION);

            Respaldo_TE respaldo = respaldoRepo.ObtenerPorPK(idRespaldo);

            if (respaldo == null) throw new ExcepcionTraducible_TLL("respaldo_no_existe");
            if (!servidor.ArchivoExiste(respaldo.Ruta)) throw new ExcepcionTraducible_TLL("respaldo_archivo_falta");

            // Antes de tocar nada se comprueba que el archivo se pueda leer: una copia dañada no debe destruir la base actual.
            try
            {
                servidor.Verificar(respaldo.Ruta);
            }
            catch (SqlException ex)
            {
                throw new ExcepcionTraducible_TLL("respaldo_archivo_danado", ex.Message);
            }

            if (Interlocked.CompareExchange(ref restauracionEnCurso, 1, 0) != 0)
                throw new ExcepcionTraducible_TLL("respaldo_restauracion_en_curso");

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
                    throw new ExcepcionTraducible_TLL("respaldo_error_restaurar", ex.Message);
                }

                Reincorporar(registrados);
                AuditarRestauracion(actor, respaldo);
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
        private void AuditarRestauracion(ActorUsuario_TE actor, Respaldo_TE respaldo)
        {
            try
            {
                int idUsuario = actor.IdUsuario > 0 && new Usuario_TLL().ObtenerPorId(actor.IdUsuario) != null ? actor.IdUsuario : 0;

                bitacora.Guardar(new Bitacora_TE(idUsuario, MODULO, EventosBitacora_TLL.RESPALDO_RESTAURADO, CriticidadBitacora.Alta, DateTime.Now, 0, DetalleBitacora_TLL.Armar(respaldo.NombreArchivo)));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Respaldo.Auditar", ex);
            }
        }
    }
}
