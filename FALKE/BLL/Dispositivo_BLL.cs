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
                throw new ExcepcionNoAutorizada_TLL("disp_sin_permiso_ver");

            return dispositivoRepo.ObtenerDeEmpresa(actor.IdEmpresa);
        }

        public void Crear(ActorUsuario_TE actor, Dispositivo_BE dispositivo)
        {
            actor.Exigir(Patentes_TLL.ALTA_DISPOSITIVO);

            if (dispositivo == null) throw new ArgumentNullException(nameof(dispositivo));

            ValidarYNormalizar(dispositivo);


            ModeloDispositivo_BE modelo = ObtenerModelo(dispositivo.IdModelo);
            if (!modelo.Activo) throw new ExcepcionTraducible_TLL("disp_modelo_de_baja", modelo.Nombre);
            dispositivo.Modelo = modelo.Nombre;

            dispositivo.NumeroSerie = (dispositivo.NumeroSerie ?? string.Empty).Trim();
            if (dispositivoRepo.ExisteSerie(dispositivo.NumeroSerie)) throw new ExcepcionTraducible_TLL("disp_serie_duplicada");

            dispositivo.Estado = EstadoDispositivo.Disponible;

            Transaccion_ORM.Ejecutar(() =>
            {
                dispositivoRepo.Alta(dispositivo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { dispositivo.IdDispositivo.ToString() });

                bitacora.Auditar(actor, "Dispositivos", null, EventosBitacora_TLL.DISPOSITIVO_ALTA, CriticidadBitacora.Media, DetalleBitacora_TLL.Armar(dispositivo.NumeroSerie));
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
            if (!modelo.Activo && modelo.IdModelo != actual.IdModelo) throw new ExcepcionTraducible_TLL("disp_modelo_de_baja", modelo.Nombre);

            var cambios = new List<ParteBitacora_TLL>();
            Registrar(cambios, "bitacora_campo_modelo", actual.Modelo, modelo.Nombre);
            Registrar(cambios, "bitacora_campo_firmware", actual.Firmware, nuevos.Firmware);
            Registrar(cambios, "bitacora_campo_driver", actual.DriverRequerido, nuevos.DriverRequerido);

            if (cambios.Count == 0) throw new ExcepcionTraducible_TLL("comun_sin_cambios");

            actual.IdModelo = modelo.IdModelo;
            actual.Modelo = modelo.Nombre;
            actual.Firmware = nuevos.Firmware;
            actual.DriverRequerido = nuevos.DriverRequerido;

            Transaccion_ORM.Ejecutar(() =>
            {
                dispositivoRepo.Modificar(actual);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { actual.IdDispositivo.ToString() });

                bitacora.Auditar(actor, "Dispositivos", actual.IdEmpresaActual, EventosBitacora_TLL.DISPOSITIVO_MODIFICACION, CriticidadBitacora.Media,
                    DetalleBitacora_TLL.Armar(actual.NumeroSerie, ": ", DetalleBitacora_TLL.Lista(cambios, ", ")));
            });
        }

        public void AsignarAEmpresa(ActorUsuario_TE actor, int idDispositivo, int idEmpresa)
        {
            actor.Exigir(Patentes_TLL.ASIGNAR_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);

            if (dispositivo.Estado != EstadoDispositivo.Disponible)
                throw new ExcepcionTraducible_TLL("disp_solo_prestar_disponible", new ClaveTexto_TLL(ClaveEstado(dispositivo.Estado), true));

            if (idEmpresa == BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA) throw new ExcepcionTraducible_TLL("disp_prestar_a_pattern_blue");

            Empresa_BE empresa = empresaRepo.ObtenerPorPK(idEmpresa);
            if (empresa == null) throw new ExcepcionTraducible_TLL("disp_empresa_no_existe");
            if (empresa.Estado != EstadoEmpresa.Activa) throw new ExcepcionTraducible_TLL("disp_solo_empresas_activas");

            int? limite = LimiteDelPlan(empresa.PlanSuscripcion);
            int enPrestamo = prestamoRepo.ContarAbiertosDeEmpresa(idEmpresa);

            if (limite.HasValue && enPrestamo >= limite.Value)
                throw new ExcepcionTraducible_TLL(limite.Value == 1 ? "disp_tope_plan_uno" : "disp_tope_plan_varios", empresa.PlanSuscripcion, empresa.NombreEmpresa, limite.Value, enPrestamo);

            var prestamo = new Prestamo_BE { IdDispositivo = idDispositivo, IdEmpresa = idEmpresa, FechaEntrega = DateTime.Now };
            dispositivo.Estado = EstadoDispositivo.EnUso;

            Transaccion_ORM.Ejecutar(() =>
            {
                prestamoRepo.Alta(prestamo);
                dispositivoRepo.Modificar(dispositivo);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Prestamo, new[] { prestamo.IdPrestamo.ToString() });
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { idDispositivo.ToString() });

                bitacora.Auditar(actor, "Dispositivos", idEmpresa, EventosBitacora_TLL.DISPOSITIVO_PRESTAMO, CriticidadBitacora.Media,
                    DetalleBitacora_TLL.Armar(dispositivo.NumeroSerie));
            });
        }

        public void RegistrarDevolucion(ActorUsuario_TE actor, int idDispositivo, bool requiereMantenimiento)
        {
            actor.Exigir(Patentes_TLL.ASIGNAR_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);
            Prestamo_BE prestamo = prestamoRepo.ObtenerAbierto(idDispositivo);

            if (dispositivo.Estado != EstadoDispositivo.EnUso || prestamo == null)
                throw new ExcepcionTraducible_TLL("disp_no_esta_prestado");

            string empresa = prestamo.NombreEmpresa;

            prestamo.FechaDevolucion = DateTime.Now;
            dispositivo.Estado = requiereMantenimiento ? EstadoDispositivo.EnMantenimiento : EstadoDispositivo.Disponible;

            Transaccion_ORM.Ejecutar(() =>
            {
                prestamoRepo.Modificar(prestamo);
                dispositivoRepo.Modificar(dispositivo);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Prestamo, new[] { prestamo.IdPrestamo.ToString() });
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { idDispositivo.ToString() });

                bitacora.Auditar(actor, "Dispositivos", prestamo.IdEmpresa, EventosBitacora_TLL.DISPOSITIVO_DEVOLUCION, CriticidadBitacora.Media,
                    DetalleBitacora_TLL.Armar(dispositivo.NumeroSerie));
            });
        }

        public void EnviarAMantenimiento(ActorUsuario_TE actor, int idDispositivo)
        {
            actor.Exigir(Patentes_TLL.ALTA_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);

            if (dispositivo.Estado == EstadoDispositivo.EnUso) throw new ExcepcionTraducible_TLL("disp_prestado_registrar_devolucion");
            if (dispositivo.Estado != EstadoDispositivo.Disponible) throw new ExcepcionTraducible_TLL("disp_solo_mantenimiento_disponible");

            CambiarEstado(actor, dispositivo, EstadoDispositivo.EnMantenimiento, EventosBitacora_TLL.DISPOSITIVO_MANTENIMIENTO, DetalleBitacora_TLL.Armar(dispositivo.NumeroSerie), CriticidadBitacora.Media);
        }

        public void TerminarMantenimiento(ActorUsuario_TE actor, int idDispositivo)
        {
            actor.Exigir(Patentes_TLL.ALTA_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);

            if (dispositivo.Estado != EstadoDispositivo.EnMantenimiento) throw new ExcepcionTraducible_TLL("disp_no_esta_en_mantenimiento");

            CambiarEstado(actor, dispositivo, EstadoDispositivo.Disponible, EventosBitacora_TLL.DISPOSITIVO_FIN_MANTENIMIENTO, DetalleBitacora_TLL.Armar(dispositivo.NumeroSerie), CriticidadBitacora.Media);
        }

        public void DarDeBaja(ActorUsuario_TE actor, int idDispositivo, string motivo)
        {
            actor.Exigir(Patentes_TLL.ALTA_DISPOSITIVO);

            Dispositivo_BE dispositivo = ObtenerExistente(idDispositivo);
            ExigirNoDadoDeBaja(dispositivo);

            if (dispositivo.Estado == EstadoDispositivo.EnUso) throw new ExcepcionTraducible_TLL("disp_prestado_registrar_devolucion");

            motivo = (motivo ?? string.Empty).Trim();
            if (motivo.Length == 0) throw new ExcepcionTraducible_TLL("disp_motivo_baja");
            if (motivo.Length > LARGO_MOTIVO) throw new ExcepcionTraducible_TLL("comun_motivo_largo", LARGO_MOTIVO);

            CambiarEstado(actor, dispositivo, EstadoDispositivo.DeBaja, EventosBitacora_TLL.DISPOSITIVO_BAJA, DetalleBitacora_TLL.Armar(dispositivo.NumeroSerie, DetalleBitacora_TLL.Motivo(motivo)), CriticidadBitacora.Alta);
        }

        public static string ClaveEstado(EstadoDispositivo estado)
        {
            switch (estado)
            {
                case EstadoDispositivo.EnUso: return "disp_estado_en_uso";
                case EstadoDispositivo.EnMantenimiento: return "disp_estado_mantenimiento";
                case EstadoDispositivo.DeBaja: return "disp_estado_de_baja";
                default: return "disp_estado_disponible";
            }
        }

        private void CambiarEstado(ActorUsuario_TE actor, Dispositivo_BE dispositivo, EstadoDispositivo nuevo, string evento, string detalle, CriticidadBitacora criticidad)
        {
            dispositivo.Estado = nuevo;

            Transaccion_ORM.Ejecutar(() =>
            {
                dispositivoRepo.Modificar(dispositivo);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Dispositivo, new[] { dispositivo.IdDispositivo.ToString() });

                bitacora.Auditar(actor, "Dispositivos", null, evento, criticidad, detalle);
            });
        }

        private Dispositivo_BE ObtenerExistente(int idDispositivo)
        {
            Dispositivo_BE dispositivo = dispositivoRepo.ObtenerPorPK(idDispositivo);

            if (dispositivo == null) throw new ExcepcionTraducible_TLL("disp_no_existe");

            return dispositivo;
        }

        private ModeloDispositivo_BE ObtenerModelo(int idModelo)
        {
            ModeloDispositivo_BE modelo = idModelo > 0 ? modeloRepo.ObtenerPorPK(idModelo) : null;

            if (modelo == null) throw new ExcepcionTraducible_TLL("disp_elegir_modelo");

            return modelo;
        }

        private static void ExigirNoDadoDeBaja(Dispositivo_BE dispositivo)
        {
            if (dispositivo.Estado == EstadoDispositivo.DeBaja) throw new ExcepcionTraducible_TLL("disp_ya_de_baja");
        }

        private static void ValidarYNormalizar(Dispositivo_BE d)
        {
            d.NumeroSerie = (d.NumeroSerie ?? string.Empty).Trim();
            d.Firmware = Recortar(d.Firmware);
            d.DriverRequerido = Recortar(d.DriverRequerido);

            if (d.NumeroSerie.Length == 0) throw new ExcepcionTraducible_TLL("disp_serie_obligatoria");
            if (d.NumeroSerie.Length > LARGO_SERIE) throw new ExcepcionTraducible_TLL("disp_serie_larga", LARGO_SERIE);
            if (!FormatoSerie.IsMatch(d.NumeroSerie)) throw new ExcepcionTraducible_TLL("disp_serie_caracteres");

            if ((d.Firmware ?? string.Empty).Length > LARGO_FIRMWARE) throw new ExcepcionTraducible_TLL("disp_firmware_largo", LARGO_FIRMWARE);
            if ((d.DriverRequerido ?? string.Empty).Length > LARGO_DRIVER) throw new ExcepcionTraducible_TLL("disp_driver_largo", LARGO_DRIVER);
        }

        private static void Registrar(List<ParteBitacora_TLL> cambios, string campo, string antes, string despues)
        {
            antes = string.IsNullOrEmpty(antes) ? null : antes;
            despues = string.IsNullOrEmpty(despues) ? null : despues;

            if (string.Equals(antes, despues, StringComparison.Ordinal)) return;

            cambios.Add(DetalleBitacora_TLL.Clave(campo));
        }

        private static bool Contiene(string texto, string buscado)
        {
            return texto != null && texto.IndexOf(buscado, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
