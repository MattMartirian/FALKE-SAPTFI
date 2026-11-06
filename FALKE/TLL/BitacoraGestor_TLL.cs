using ORM;
using TE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace TLL
{
    public class BitacoraGestor_TLL
    {
        public const int ID_EMPRESA_PROVEEDORA = 1;
        public const string ETIQUETA_PROVEEDOR = "Equipo de Pattern Blue";

        private readonly BitacoraRepository bitacoraRepo;

        public BitacoraGestor_TLL()
        {
            bitacoraRepo = new BitacoraRepository();
        }

        public void Guardar(Bitacora_TE bitacora)
        {
            bitacoraRepo.Alta(bitacora);
        }

        public void Auditar(int idUsuario, string modulo, int? idEmpresa, string descripcion, CriticidadBitacora criticidad, string detalle = null)
        {
            Guardar(new Bitacora_TE(idUsuario, modulo, descripcion, criticidad, DateTime.Now, 0, detalle) { IdEmpresa = idEmpresa });
        }

        public void Auditar(ActorUsuario_TE actor, string modulo, int? idEmpresa, string descripcion, CriticidadBitacora criticidad, string detalle = null)
        {
            Auditar(actor.IdUsuario, modulo, idEmpresa, descripcion, criticidad, detalle);
        }

        public List<Bitacora_TE> ObtenerMiActividad(ActorUsuario_TE actor, int cantidad = 10)
        {
            if (actor == null || actor.IdUsuario <= 0) return new List<Bitacora_TE>();

            return bitacoraRepo.ObtenerPorUsuario(actor.IdUsuario, Math.Max(1, Math.Min(cantidad, 50)));
        }

        public void Registrar(int idUsuario, string modulo, string descripcion, CriticidadBitacora criticidad, int? idEmpresa = null, string detalle = null)
        {
            try
            {
                Guardar(new Bitacora_TE(idUsuario, modulo, descripcion, criticidad, DateTime.Now, 0, detalle) { IdEmpresa = idEmpresa });
            }
            catch
            {
            }
        }

        public const string ACCION_INICIO_SESION = "Inicio de sesion";
        public const string ACCION_CIERRE_SESION = "Cierre de sesion";
        public const string ACCION_ALTA = "Alta";
        public const string ACCION_MODIFICACION = "Modificacion";
        public const string ACCION_BAJA = "Baja";

        private static readonly HashSet<string> EVENTOS_ALTA = new HashSet<string>
        {
            EventosBitacora_TLL.USUARIO_ALTA, EventosBitacora_TLL.EMPRESA_ALTA, EventosBitacora_TLL.CATEGORIA_ALTA, EventosBitacora_TLL.MODELO_ALTA,
            EventosBitacora_TLL.DISPOSITIVO_ALTA, EventosBitacora_TLL.ROL_ALTA, EventosBitacora_TLL.IDIOMA_ALTA
        };

        private static readonly HashSet<string> EVENTOS_BAJA = new HashSet<string>
        {
            EventosBitacora_TLL.USUARIO_BAJA, EventosBitacora_TLL.EMPRESA_BAJA, EventosBitacora_TLL.CATEGORIA_BAJA, EventosBitacora_TLL.CATEGORIA_DESACTIVADA, EventosBitacora_TLL.MODELO_BAJA,
            EventosBitacora_TLL.DISPOSITIVO_BAJA, EventosBitacora_TLL.ROL_BAJA, EventosBitacora_TLL.IDIOMA_BAJA
        };

        private static readonly HashSet<string> EVENTOS_MODIFICACION = new HashSet<string>
        {
            EventosBitacora_TLL.USUARIO_ACTIVACION, EventosBitacora_TLL.USUARIO_BLOQUEO, EventosBitacora_TLL.USUARIO_CAMBIO_ROL, EventosBitacora_TLL.USUARIO_MODIFICACION,
            EventosBitacora_TLL.EMPRESA_REACTIVACION, EventosBitacora_TLL.EMPRESA_BLOQUEO, EventosBitacora_TLL.EMPRESA_MODIFICACION, EventosBitacora_TLL.EMPRESA_MODIFICACION_CONTACTO,
            EventosBitacora_TLL.CATEGORIA_MODIFICACION, EventosBitacora_TLL.MODELO_MODIFICACION, EventosBitacora_TLL.MODELO_REACTIVADO,
            EventosBitacora_TLL.DISPOSITIVO_MODIFICACION, EventosBitacora_TLL.ROL_DESCRIPCION, EventosBitacora_TLL.ROL_COMPOSICION,
            EventosBitacora_TLL.IDIOMA_TRADUCCION, EventosBitacora_TLL.IDIOMA_SINCRONIZACION, EventosBitacora_TLL.IDIOMA_HABILITADO,
            EventosBitacora_TLL.IDIOMA_DESHABILITADO
        };

        public static string ActorSinUsuario()
        {
            string emergencia = ConfigurationManager.AppSettings["FALKE_EMERGENCY_USER"];

            return string.IsNullOrWhiteSpace(emergencia) ? "Sistema" : emergencia;
        }

        private List<BitacoraVista_TE> ObtenerAlcance(ActorUsuario_TE actor)
        {
            if (actor == null || !actor.Puede(Patentes_TLL.VER_BITACORA)) throw new ExcepcionNoAutorizada_TLL("bitacora_sin_permiso");

            if (actor.VeBitacoraCompleta())
                return bitacoraRepo.ObtenerVista(null, false, ID_EMPRESA_PROVEEDORA, Usuario_TLL.ROL_GESTOR, ETIQUETA_PROVEEDOR, ActorSinUsuario());

            if (actor.IdEmpresa <= 0) throw new ExcepcionNoAutorizada_TLL("comun_sesion_sin_empresa");

            return bitacoraRepo.ObtenerVista(actor.IdEmpresa, true, ID_EMPRESA_PROVEEDORA, Usuario_TLL.ROL_GESTOR, ETIQUETA_PROVEEDOR, ActorSinUsuario());
        }

        // Los filtros se aplican acá, sobre lo que el alcance del actor permite ver (el filtro por empresa solo rige para quien ve todas).
        private static List<BitacoraVista_TE> Filtrar(List<BitacoraVista_TE> registros, FiltroBitacora_TE filtro, ActorUsuario_TE actor)
        {
            if (filtro == null) return registros;

            if (filtro.Desde.HasValue && filtro.Hasta.HasValue && filtro.Desde.Value.Date > filtro.Hasta.Value.Date)
                throw new ExcepcionTraducible_TLL("bitacora_fecha_invalida");

            string texto = (filtro.Usuario ?? string.Empty).Trim();
            string modulo = (filtro.Modulo ?? string.Empty).Trim();
            string accion = (filtro.Accion ?? string.Empty).Trim();
            TimeSpan? horaDesde = filtro.HoraDesde;
            TimeSpan? horaHasta = filtro.HoraHasta.HasValue ? filtro.HoraHasta.Value.Add(new TimeSpan(0, 0, 0, 59, 999)) : (TimeSpan?)null;

            var resultado = new List<BitacoraVista_TE>();

            foreach (var r in registros)
            {
                if (filtro.Desde.HasValue && r.FechaHoraBitacora.Date < filtro.Desde.Value.Date) continue;
                if (filtro.Hasta.HasValue && r.FechaHoraBitacora.Date > filtro.Hasta.Value.Date) continue;

                TimeSpan hora = r.FechaHoraBitacora.TimeOfDay;

                if (horaDesde.HasValue && horaHasta.HasValue && horaDesde.Value > horaHasta.Value)
                {
                    if (hora < horaDesde.Value && hora > horaHasta.Value) continue;
                }
                else
                {
                    if (horaDesde.HasValue && hora < horaDesde.Value) continue;
                    if (horaHasta.HasValue && hora > horaHasta.Value) continue;
                }

                if (modulo.Length > 0 && !string.Equals(r.ModuloBitacora, modulo, StringComparison.OrdinalIgnoreCase)) continue;
                if (accion.Length > 0 && ClasificarAccion(r.DescripcionBitacora) != accion) continue;
                if (filtro.Criticidad.HasValue && r.CriticidadBitacora != filtro.Criticidad.Value) continue;
                if (actor.VeBitacoraCompleta() && filtro.IdEmpresa.HasValue && r.IdEmpresa != filtro.IdEmpresa.Value) continue;
                // Por nombre o por correo. El correo de un actor enmascarado no llega acá, así que no se puede descubrir buscándolo.
                if (texto.Length > 0 &&
                    (r.Actor ?? string.Empty).IndexOf(texto, StringComparison.CurrentCultureIgnoreCase) < 0 &&
                    (r.EmailActor ?? string.Empty).IndexOf(texto, StringComparison.CurrentCultureIgnoreCase) < 0) continue;

                resultado.Add(r);
            }

            return resultado;
        }

        public PaginaBitacora_TE ObtenerVista(ActorUsuario_TE actor, FiltroBitacora_TE filtro)
        {
            filtro = filtro ?? new FiltroBitacora_TE();

            var filtrados = Filtrar(ObtenerAlcance(actor), filtro, actor);

            int tamano = Math.Max(1, Math.Min(filtro.Tamano, 100));
            int totalPaginas = Math.Max(1, (filtrados.Count + tamano - 1) / tamano);
            int pagina = Math.Max(1, Math.Min(filtro.Pagina, totalPaginas));

            return new PaginaBitacora_TE
            {
                Total = filtrados.Count,
                Pagina = pagina,
                Tamano = tamano,
                Items = filtrados.Skip((pagina - 1) * tamano).Take(tamano).ToList()
            };
        }

        public List<string> ObtenerModulos(ActorUsuario_TE actor)
        {
            return ObtenerAlcance(actor)
                .Select(r => r.ModuloBitacora)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(m => m)
                .ToList();
        }

        public static string ClasificarAccion(string descripcion)
        {
            if (string.IsNullOrEmpty(descripcion)) return string.Empty;

            string d = descripcion.ToLowerInvariant();

            if (d.StartsWith(EventosBitacora_TLL.INICIO_SESION)) return ACCION_INICIO_SESION;
            if (d.StartsWith(EventosBitacora_TLL.CIERRE_SESION)) return ACCION_CIERRE_SESION;
            if (EVENTOS_ALTA.Contains(descripcion)) return ACCION_ALTA;
            if (EVENTOS_BAJA.Contains(descripcion)) return ACCION_BAJA;
            if (EVENTOS_MODIFICACION.Contains(descripcion)) return ACCION_MODIFICACION;

            if (d.StartsWith("inicio de sesi")) return ACCION_INICIO_SESION;
            if (d.StartsWith("cierre de sesi")) return ACCION_CIERRE_SESION;
            if (d.StartsWith("alta")) return ACCION_ALTA;
            if (d.StartsWith("baja")) return ACCION_BAJA;
            if (d.StartsWith("cambio") || d.StartsWith("modificaci") || d.StartsWith("renombrado")) return ACCION_MODIFICACION;

            return string.Empty;
        }
    }
}
