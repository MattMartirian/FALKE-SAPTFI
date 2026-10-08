using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BE;
using ORM;
using SERVICES;
using TE;
using TLL;
using static TLL.TextoHelper_TLL;

namespace BLL
{
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

        public static int? LimiteDelPlan(PlanSuscripcion plan)
        {
            switch (plan)
            {
                case PlanSuscripcion.Scout: return 1;
                case PlanSuscripcion.Hunter: return 3;
                default: return null;
            }
        }

        public List<Dispositivo_BE> Listar(ActorUsuario_TE actor, string texto = null, EstadoDispositivo? estado = null, int? idModelo = null)
        {
            actor.Exigir(Patentes_TLL.VER_DISPOSITIVOS);

            texto = (texto ?? string.Empty).Trim();

            return dispositivoRepo.ObtenerTodos().Where(d =>
                (texto.Length == 0 ||
                 Contiene(d.NumeroSerie, texto) || Contiene(d.Modelo, texto) || Contiene(d.EmpresaActual, texto)) &&
                (!estado.HasValue || d.Estado == estado.Value) &&
                (!idModelo.HasValue || d.IdModelo == idModelo.Value))
                .ToList();
        }

        public Dispositivo_BE ObtenerPorId(ActorUsuario_TE actor, int idDispositivo)
        {
            actor.Exigir(Patentes_TLL.VER_DISPOSITIVOS);

            return dispositivoRepo.ObtenerPorPK(idDispositivo);
        }

        public List<Prestamo_BE> ObtenerHistorialCompleto(ActorUsuario_TE actor)
        {
            actor.Exigir(Patentes_TLL.VER_DISPOSITIVOS);

            return prestamoRepo.ObtenerTodos();
        }

        public List<Empresa_BE> EmpresasParaPrestar(ActorUsuario_TE actor)
        {
            actor.Exigir(Patentes_TLL.ASIGNAR_DISPOSITIVO);

            return empresaRepo.ObtenerResumen()
                .Where(e => e.Estado == EstadoEmpresa.Activa && e.IdEmpresa != BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA)
                .OrderBy(e => e.NombreEmpresa)
                .ToList();
        }

        public List<Dispositivo_BE> ObtenerDeMiEmpresa(ActorUsuario_TE actor)
        {
            if (actor == null || actor.IdEmpresa <= 0 || !actor.Puede(Patentes_TLL.VER_DATOS_EMPRESA))
                throw new UnauthorizedAccessException("No tenés permiso para ver los dispositivos de la empresa.");

            return dispositivoRepo.ObtenerDeEmpresa(actor.IdEmpresa);
        }

        public void Crear(ActorUsuario_TE actor, Dispositivo_BE dispositivo)
        {
            actor.Exigir(Patentes_TLL.ALTA_DISPOSITIVO);

            if (dispositivo == null) throw new ArgumentNullException(nameof(dispositivo));

            ValidarYNormalizar(dispositivo);


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

                bitacora.Auditar(actor, "Dispositivos", null, "Alta del dispositivo " + dispositivo.NumeroSerie + " (" + dispositivo.Modelo + ").", CriticidadBitacora.Media);
            });
        }

        public void Modificar(ActorUsuario_TE actor, int idDispositivo, Dispositivo_BE nuevos)
        {
            actor.Exigir(Patentes_TLL.ALTA_DISPOSITIVO);

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

                bitacora.Auditar(actor, "Dispositivos", actual.IdEmpresaActual, "Modificación del dispositivo " + actual.NumeroSerie + ": " + string.Join("; ", cambios) + ".", CriticidadBitacora.Media);
            });
        }

        public void AsignarAEmpresa(ActorUsuario_TE actor, int idDispositivo, int idEmpresa)
        {
            actor.Exigir(Patentes_TLL.ASIGNAR_DISPOSITIVO);

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

                bitacora.Auditar(actor, "Dispositivos", idEmpresa, "Préstamo del dispositivo " + dispositivo.NumeroSerie + " (" + dispositivo.Modelo + ") a la empresa \"" + empresa.NombreEmpresa + "\".", CriticidadBitacora.Media);
            });
        }

        public void RegistrarDevolucion(ActorUsuario_TE actor, int idDispositivo, bool requiereMantenimiento)
        {
            actor.Exigir(Patentes_TLL.ASIGNAR_DISPOSITIVO);

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

                bitacora.Auditar(actor, "Dispositivos", prestamo.IdEmpresa, "Devolución del dispositivo " + dispositivo.NumeroSerie + " por la empresa \"" + empresa + "\"" +
                    (requiereMantenimiento ? "; pasa a mantenimiento." : "."), CriticidadBitacora.Media);
            });
        }

        public void EnviarAMantenimiento(ActorUsuario_TE actor, int idDispositivo)
        {
            actor.Exigir(Patentes_TLL.ALTA_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);

            if (dispositivo.Estado == EstadoDispositivo.EnUso) throw new InvalidOperationException("El dispositivo está en préstamo: registrá primero la devolución.");
            if (dispositivo.Estado != EstadoDispositivo.Disponible) throw new InvalidOperationException("Solo se envía a mantenimiento un dispositivo disponible.");

            CambiarEstado(actor, dispositivo, EstadoDispositivo.EnMantenimiento, "Se envió el dispositivo " + dispositivo.NumeroSerie + " a mantenimiento.", CriticidadBitacora.Media);
        }

        public void TerminarMantenimiento(ActorUsuario_TE actor, int idDispositivo)
        {
            actor.Exigir(Patentes_TLL.ALTA_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);

            if (dispositivo.Estado != EstadoDispositivo.EnMantenimiento) throw new InvalidOperationException("El dispositivo no está en mantenimiento.");

            CambiarEstado(actor, dispositivo, EstadoDispositivo.Disponible,
                "El dispositivo " + dispositivo.NumeroSerie + " terminó el mantenimiento y vuelve a estar disponible.", CriticidadBitacora.Media);
        }

        public void DarDeBaja(ActorUsuario_TE actor, int idDispositivo, string motivo)
        {
            actor.Exigir(Patentes_TLL.ALTA_DISPOSITIVO);

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

        private void CambiarEstado(ActorUsuario_TE actor, Dispositivo_BE dispositivo, EstadoDispositivo nuevo, string descripcion, CriticidadBitacora criticidad)
        {
            dispositivo.Estado = nuevo;

            Transaccion_ORM.Ejecutar(() =>
            {
                dispositivoRepo.Modificar(dispositivo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { dispositivo.IdDispositivo.ToString() });

                bitacora.Auditar(actor, "Dispositivos", null, descripcion, criticidad);
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

        private static bool Contiene(string texto, string buscado)
        {
            return texto != null && texto.IndexOf(buscado, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
