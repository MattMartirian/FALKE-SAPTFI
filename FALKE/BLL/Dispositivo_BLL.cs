using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BE;
using ORM;
using SERVICES;
using TE;
using TLL;

namespace BLL
{
    // Inventario de eye trackers de Pattern Blue y préstamos a las empresas cliente.
    // El aparato se identifica por su número de serie de fábrica (único); el id interno es solo la clave de las tablas.
    public class Dispositivo_BLL
    {
        private const int LARGO_SERIE = 30;
        private const int LARGO_FIRMWARE = 20;
        private const int LARGO_DRIVER = 20;
        private const int LARGO_MOTIVO = 300;

        private static readonly Regex FormatoSerie = new Regex(@"^[A-Za-z0-9][A-Za-z0-9 ._/\-]*$");

        private readonly DispositivoRepository dispositivoRepo;
        private readonly ModeloDispositivoRepository modeloRepo;
        private readonly PrestamoRepository prestamoRepo;
        private readonly EmpresaRepository empresaRepo;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;

        public Dispositivo_BLL()
        {
            dispositivoRepo = new DispositivoRepository();
            modeloRepo = new ModeloDispositivoRepository();
            prestamoRepo = new PrestamoRepository();
            empresaRepo = new EmpresaRepository();
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
        }

        // Cuántos dispositivos incluye cada plan (ver Planes.aspx). Apex es "a convenir": sin tope fijo.
        public static int? LimiteDelPlan(PlanSuscripcion plan)
        {
            switch (plan)
            {
                case PlanSuscripcion.Scout: return 1;
                case PlanSuscripcion.Hunter: return 3;
                default: return null;
            }
        }

        // ---- Consultas

        public List<Dispositivo_BE> Listar(ActorUsuario_TLL actor, string texto = null, EstadoDispositivo? estado = null, int? idModelo = null)
        {
            Exigir(actor, Patentes_TLL.VER_DISPOSITIVOS);

            texto = (texto ?? string.Empty).Trim();

            return dispositivoRepo.ObtenerTodos().Where(d =>
                (texto.Length == 0 ||
                 Contiene(d.NumeroSerie, texto) || Contiene(d.Modelo, texto) || Contiene(d.EmpresaActual, texto)) &&
                (!estado.HasValue || d.Estado == estado.Value) &&
                (!idModelo.HasValue || d.IdModelo == idModelo.Value))
                .ToList();
        }

        public Dispositivo_BE ObtenerPorId(ActorUsuario_TLL actor, int idDispositivo)
        {
            Exigir(actor, Patentes_TLL.VER_DISPOSITIVOS);

            return dispositivoRepo.ObtenerPorPK(idDispositivo);
        }

        public List<Prestamo_BE> ObtenerHistorial(ActorUsuario_TLL actor, int idDispositivo)
        {
            Exigir(actor, Patentes_TLL.VER_DISPOSITIVOS);

            return prestamoRepo.ObtenerHistorial(idDispositivo);
        }

        // Los préstamos de todos los dispositivos, del más reciente al más antiguo (la pantalla los reparte en el detalle de cada uno).
        public List<Prestamo_BE> ObtenerHistorialCompleto(ActorUsuario_TLL actor)
        {
            Exigir(actor, Patentes_TLL.VER_DISPOSITIVOS);

            return prestamoRepo.ObtenerTodos();
        }

        // Las empresas activas a las que se puede prestar, con cuántos dispositivos tienen hoy (para elegir al asignar).
        public List<Empresa_BE> EmpresasParaPrestar(ActorUsuario_TLL actor)
        {
            Exigir(actor, Patentes_TLL.ASIGNAR_DISPOSITIVO);

            return empresaRepo.ObtenerResumen()
                .Where(e => e.Estado == EstadoEmpresa.Activa && e.IdEmpresa != BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA)
                .OrderBy(e => e.NombreEmpresa)
                .ToList();
        }

        // La propia empresa ve los dispositivos que tiene en préstamo, y solo esos.
        public List<Dispositivo_BE> ObtenerDeMiEmpresa(ActorUsuario_TLL actor)
        {
            if (actor == null || actor.IdEmpresa <= 0 || !actor.Puede(Patentes_TLL.VER_DATOS_EMPRESA))
                throw new UnauthorizedAccessException("No tenés permiso para ver los dispositivos de la empresa.");

            return dispositivoRepo.ObtenerDeEmpresa(actor.IdEmpresa);
        }

        // ---- Inventario

        public void Crear(ActorUsuario_TLL actor, Dispositivo_BE dispositivo)
        {
            Exigir(actor, Patentes_TLL.ALTA_DISPOSITIVO);

            if (dispositivo == null) throw new ArgumentNullException(nameof(dispositivo));

            ValidarYNormalizar(dispositivo);

            // Para un dispositivo nuevo solo se ofrecen los modelos activos.
            ModeloDispositivo_BE modelo = ObtenerModelo(dispositivo.IdModelo);
            if (!modelo.Activo) throw new InvalidOperationException("El modelo \"" + modelo.Nombre + "\" está dado de baja: elegí otro.");
            dispositivo.Modelo = modelo.Nombre;

            dispositivo.NumeroSerie = (dispositivo.NumeroSerie ?? string.Empty).Trim();
            if (dispositivoRepo.ExisteSerie(dispositivo.NumeroSerie)) throw new InvalidOperationException("Ya existe un dispositivo con ese número de serie.");

            dispositivo.Estado = EstadoDispositivo.Disponible;

            Transaccion_ORM.Ejecutar(() =>
            {
                dispositivoRepo.Alta(dispositivo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { dispositivo.IdDispositivo.ToString() });

                Auditar(actor, null, "Alta del dispositivo " + dispositivo.NumeroSerie + " (" + dispositivo.Modelo + ").", CriticidadBitacora.Media);
            });
        }

        // Modelo, firmware y driver. El número de serie y el estado no se editan desde acá.
        // Se puede conservar el modelo actual aunque esté dado de baja, pero no cambiar a uno dado de baja.
        public void Modificar(ActorUsuario_TLL actor, int idDispositivo, Dispositivo_BE nuevos)
        {
            Exigir(actor, Patentes_TLL.ALTA_DISPOSITIVO);

            if (nuevos == null) throw new ArgumentNullException(nameof(nuevos));

            Dispositivo_BE actual = ObtenerExistente(idDispositivo);
            ExigirNoDadoDeBaja(actual);

            nuevos.NumeroSerie = actual.NumeroSerie;
            ValidarYNormalizar(nuevos);

            ModeloDispositivo_BE modelo = ObtenerModelo(nuevos.IdModelo);
            if (!modelo.Activo && modelo.IdModelo != actual.IdModelo) throw new InvalidOperationException("El modelo \"" + modelo.Nombre + "\" está dado de baja: elegí otro.");

            var cambios = new List<string>();
            Registrar(cambios, "modelo", actual.Modelo, modelo.Nombre);
            Registrar(cambios, "firmware", actual.Firmware, nuevos.Firmware);
            Registrar(cambios, "driver requerido", actual.DriverRequerido, nuevos.DriverRequerido);

            if (cambios.Count == 0) throw new InvalidOperationException("No hay cambios para guardar.");

            actual.IdModelo = modelo.IdModelo;
            actual.Modelo = modelo.Nombre;
            actual.Firmware = nuevos.Firmware;
            actual.DriverRequerido = nuevos.DriverRequerido;

            Transaccion_ORM.Ejecutar(() =>
            {
                dispositivoRepo.Modificar(actual);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { actual.IdDispositivo.ToString() });

                Auditar(actor, actual.IdEmpresaActual, "Modificación del dispositivo " + actual.NumeroSerie + ": " + string.Join("; ", cambios) + ".", CriticidadBitacora.Media);
            });
        }

        // ---- Préstamos

        // Presta un dispositivo disponible a una empresa activa, dentro de lo que incluye su plan.
        public void AsignarAEmpresa(ActorUsuario_TLL actor, int idDispositivo, int idEmpresa)
        {
            Exigir(actor, Patentes_TLL.ASIGNAR_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);

            if (dispositivo.Estado != EstadoDispositivo.Disponible)
                throw new InvalidOperationException("Solo se puede prestar un dispositivo disponible (este está " + NombreEstado(dispositivo.Estado).ToLowerInvariant() + ").");

            if (idEmpresa == BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA) throw new InvalidOperationException("Los dispositivos se prestan a empresas cliente, no a Pattern Blue.");

            Empresa_BE empresa = empresaRepo.ObtenerPorPK(idEmpresa);
            if (empresa == null) throw new InvalidOperationException("La empresa no existe.");
            if (empresa.Estado != EstadoEmpresa.Activa) throw new InvalidOperationException("Solo se prestan dispositivos a empresas activas.");

            int? limite = LimiteDelPlan(empresa.PlanSuscripcion);
            int enPrestamo = prestamoRepo.ContarAbiertosDeEmpresa(idEmpresa);

            if (limite.HasValue && enPrestamo >= limite.Value)
                throw new InvalidOperationException("El plan " + empresa.PlanSuscripcion + " de \"" + empresa.NombreEmpresa + "\" incluye " + limite.Value +
                    (limite.Value == 1 ? " dispositivo" : " dispositivos") + " y ya tiene " + enPrestamo + " en préstamo.");

            var prestamo = new Prestamo_BE { IdDispositivo = idDispositivo, IdEmpresa = idEmpresa, FechaEntrega = DateTime.Now };
            dispositivo.Estado = EstadoDispositivo.EnUso;

            Transaccion_ORM.Ejecutar(() =>
            {
                prestamoRepo.Alta(prestamo);
                dispositivoRepo.Modificar(dispositivo);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Prestamo, new[] { prestamo.IdPrestamo.ToString() });
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { idDispositivo.ToString() });

                Auditar(actor, idEmpresa, "Préstamo del dispositivo " + dispositivo.NumeroSerie + " (" + dispositivo.Modelo + ") a la empresa \"" + empresa.NombreEmpresa + "\".", CriticidadBitacora.Media);
            });
        }

        // Cierra el préstamo abierto. El dispositivo vuelve a estar disponible, o pasa a mantenimiento si vuelve con problemas.
        public void RegistrarDevolucion(ActorUsuario_TLL actor, int idDispositivo, bool requiereMantenimiento)
        {
            Exigir(actor, Patentes_TLL.ASIGNAR_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);
            Prestamo_BE prestamo = prestamoRepo.ObtenerAbierto(idDispositivo);

            if (dispositivo.Estado != EstadoDispositivo.EnUso || prestamo == null)
                throw new InvalidOperationException("El dispositivo no está en préstamo.");

            string empresa = prestamo.NombreEmpresa;

            prestamo.FechaDevolucion = DateTime.Now;
            dispositivo.Estado = requiereMantenimiento ? EstadoDispositivo.EnMantenimiento : EstadoDispositivo.Disponible;

            Transaccion_ORM.Ejecutar(() =>
            {
                prestamoRepo.Modificar(prestamo);
                dispositivoRepo.Modificar(dispositivo);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Prestamo, new[] { prestamo.IdPrestamo.ToString() });
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { idDispositivo.ToString() });

                Auditar(actor, prestamo.IdEmpresa, "Devolución del dispositivo " + dispositivo.NumeroSerie + " por la empresa \"" + empresa + "\"" +
                    (requiereMantenimiento ? "; pasa a mantenimiento." : "."), CriticidadBitacora.Media);
            });
        }

        // ---- Estado

        public void EnviarAMantenimiento(ActorUsuario_TLL actor, int idDispositivo)
        {
            Exigir(actor, Patentes_TLL.ALTA_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);

            if (dispositivo.Estado == EstadoDispositivo.EnUso) throw new InvalidOperationException("El dispositivo está en préstamo: registrá primero la devolución.");
            if (dispositivo.Estado != EstadoDispositivo.Disponible) throw new InvalidOperationException("Solo se envía a mantenimiento un dispositivo disponible.");

            CambiarEstado(actor, dispositivo, EstadoDispositivo.EnMantenimiento, "Se envió el dispositivo " + dispositivo.NumeroSerie + " a mantenimiento.", CriticidadBitacora.Media);
        }

        // Vuelve a estar disponible. La calibración no se registra acá: la hace la empresa en su puesto.
        public void TerminarMantenimiento(ActorUsuario_TLL actor, int idDispositivo)
        {
            Exigir(actor, Patentes_TLL.ALTA_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);

            if (dispositivo.Estado != EstadoDispositivo.EnMantenimiento) throw new InvalidOperationException("El dispositivo no está en mantenimiento.");

            CambiarEstado(actor, dispositivo, EstadoDispositivo.Disponible,
                "El dispositivo " + dispositivo.NumeroSerie + " terminó el mantenimiento y vuelve a estar disponible.", CriticidadBitacora.Media);
        }

        // La baja es definitiva y exige motivo.
        public void DarDeBaja(ActorUsuario_TLL actor, int idDispositivo, string motivo)
        {
            Exigir(actor, Patentes_TLL.ALTA_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);
            ExigirNoDadoDeBaja(dispositivo);

            if (dispositivo.Estado == EstadoDispositivo.EnUso) throw new InvalidOperationException("El dispositivo está en préstamo: registrá primero la devolución.");

            motivo = (motivo ?? string.Empty).Trim();
            if (motivo.Length == 0) throw new InvalidOperationException("Indicá el motivo de la baja.");
            if (motivo.Length > LARGO_MOTIVO) throw new InvalidOperationException("El motivo no puede superar los " + LARGO_MOTIVO + " caracteres.");

            CambiarEstado(actor, dispositivo, EstadoDispositivo.DeBaja, "Baja del dispositivo " + dispositivo.NumeroSerie + ". Motivo: " + motivo, CriticidadBitacora.Alta);
        }

        public static string NombreEstado(EstadoDispositivo estado)
        {
            switch (estado)
            {
                case EstadoDispositivo.EnUso: return "En uso";
                case EstadoDispositivo.EnMantenimiento: return "En mantenimiento";
                case EstadoDispositivo.DeBaja: return "De baja";
                default: return "Disponible";
            }
        }

        // ---- Internos

        private void CambiarEstado(ActorUsuario_TLL actor, Dispositivo_BE dispositivo, EstadoDispositivo nuevo, string descripcion, CriticidadBitacora criticidad)
        {
            dispositivo.Estado = nuevo;

            Transaccion_ORM.Ejecutar(() =>
            {
                dispositivoRepo.Modificar(dispositivo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { dispositivo.IdDispositivo.ToString() });

                Auditar(actor, null, descripcion, criticidad);
            });
        }

        private Dispositivo_BE ObtenerExistente(int idDispositivo)
        {
            Dispositivo_BE dispositivo = dispositivoRepo.ObtenerPorPK(idDispositivo);

            if (dispositivo == null) throw new InvalidOperationException("El dispositivo no existe.");

            return dispositivo;
        }

        private ModeloDispositivo_BE ObtenerModelo(int idModelo)
        {
            ModeloDispositivo_BE modelo = idModelo > 0 ? modeloRepo.ObtenerPorPK(idModelo) : null;

            if (modelo == null) throw new InvalidOperationException("Elegí el modelo del dispositivo.");

            return modelo;
        }

        private static void ExigirNoDadoDeBaja(Dispositivo_BE dispositivo)
        {
            if (dispositivo.Estado == EstadoDispositivo.DeBaja) throw new InvalidOperationException("El dispositivo está dado de baja.");
        }

        private void Exigir(ActorUsuario_TLL actor, string patente)
        {
            if (actor != null && actor.Puede(patente)) return;

            bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", "Acción rechazada por falta de permiso (" + patente + ")", CriticidadBitacora.Media);
            throw new UnauthorizedAccessException("No tenés permiso para realizar esta acción.");
        }

        // Estos eventos son de auditoría: si no se pueden guardar, el cambio completo se revierte.
        // Los préstamos y devoluciones quedan atribuidos a la empresa afectada: ella los ve en su propia bitácora.
        private void Auditar(ActorUsuario_TLL actor, int? idEmpresa, string descripcion, CriticidadBitacora criticidad)
        {
            bitacora.Guardar(new Bitacora_TE(actor.IdUsuario, "Dispositivos", descripcion, criticidad, DateTime.Now) { IdEmpresa = idEmpresa });
        }

        private static void ValidarYNormalizar(Dispositivo_BE d)
        {
            d.NumeroSerie = (d.NumeroSerie ?? string.Empty).Trim();
            d.Firmware = Recortar(d.Firmware);
            d.DriverRequerido = Recortar(d.DriverRequerido);

            if (d.NumeroSerie.Length == 0) throw new InvalidOperationException("El número de serie es obligatorio.");
            if (d.NumeroSerie.Length > LARGO_SERIE) throw new InvalidOperationException("El número de serie no puede superar los " + LARGO_SERIE + " caracteres.");
            if (!FormatoSerie.IsMatch(d.NumeroSerie)) throw new InvalidOperationException("El número de serie solo admite letras, números, espacios y . _ / -");

            if ((d.Firmware ?? string.Empty).Length > LARGO_FIRMWARE) throw new InvalidOperationException("El firmware no puede superar los " + LARGO_FIRMWARE + " caracteres.");
            if ((d.DriverRequerido ?? string.Empty).Length > LARGO_DRIVER) throw new InvalidOperationException("El driver requerido no puede superar los " + LARGO_DRIVER + " caracteres.");
        }

        private static void Registrar(List<string> cambios, string campo, string antes, string despues)
        {
            antes = string.IsNullOrEmpty(antes) ? null : antes;
            despues = string.IsNullOrEmpty(despues) ? null : despues;

            if (string.Equals(antes, despues, StringComparison.Ordinal)) return;

            cambios.Add(campo + ": " + (antes ?? "(vacío)") + " → " + (despues ?? "(vacío)"));
        }

        private static string Recortar(string texto)
        {
            string limpio = (texto ?? string.Empty).Trim();
            return limpio.Length == 0 ? null : limpio;
        }

        private static bool Contiene(string texto, string buscado)
        {
            return texto != null && texto.IndexOf(buscado, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
