using ORM;
using TE;
using System;
using System.Collections.Generic;
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

        // Cada usuario ve su propia actividad (lo que hizo él), sin necesitar permiso sobre la bitácora general.
        public List<Bitacora_TE> ObtenerMiActividad(ActorUsuario_TE actor, int cantidad = 10)
        {
            if (actor == null || actor.IdUsuario <= 0) return new List<Bitacora_TE>();

            return bitacoraRepo.ObtenerPorUsuario(actor.IdUsuario, Math.Max(1, Math.Min(cantidad, 50)));
        }

        public void Registrar(int idUsuario, string modulo, string descripcion, CriticidadBitacora criticidad, int? idEmpresa = null)
        {
            try
            {
                Guardar(new Bitacora_TE(idUsuario, modulo, descripcion, criticidad, DateTime.Now) { IdEmpresa = idEmpresa });
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

        // El Gestor ve todo con nombres; el administrador solo los eventos de su empresa y el equipo de Pattern Blue sin nombre.
        private List<BitacoraVista_TE> ObtenerAlcance(ActorUsuario_TE actor)
        {
            if (actor == null || !actor.Puede(Patentes_TLL.VER_BITACORA)) throw new UnauthorizedAccessException("No tenés permiso para ver la bitácora.");

            if (actor.VeBitacoraCompleta())
                return bitacoraRepo.ObtenerVista(null, false, ID_EMPRESA_PROVEEDORA, Usuario_TLL.ROL_GESTOR, ETIQUETA_PROVEEDOR);

            if (actor.IdEmpresa <= 0) throw new UnauthorizedAccessException("La sesión no tiene una empresa asociada.");

            return bitacoraRepo.ObtenerVista(actor.IdEmpresa, true, ID_EMPRESA_PROVEEDORA, Usuario_TLL.ROL_GESTOR, ETIQUETA_PROVEEDOR);
        }

        // Los filtros se aplican acá, sobre lo que el alcance del actor permite ver (el filtro por empresa solo rige para quien ve todas).
        private static List<BitacoraVista_TE> Filtrar(List<BitacoraVista_TE> registros, FiltroBitacora_TE filtro, ActorUsuario_TE actor)
        {
            if (filtro == null) return registros;

            if (filtro.Desde.HasValue && filtro.Hasta.HasValue && filtro.Desde.Value.Date > filtro.Hasta.Value.Date)
                throw new InvalidOperationException("La fecha \"desde\" no puede ser posterior a la fecha \"hasta\".");

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

            if (d.StartsWith("inicio de sesi")) return ACCION_INICIO_SESION;
            if (d.StartsWith("cierre de sesi")) return ACCION_CIERRE_SESION;
            if (d.StartsWith("alta")) return ACCION_ALTA;
            if (d.StartsWith("baja")) return ACCION_BAJA;
            if (d.StartsWith("cambio") || d.StartsWith("modificaci") || d.StartsWith("renombrado")) return ACCION_MODIFICACION;

            return string.Empty;
        }

        public List<Bitacora_TE> ObtenerTodas()
        {
            return bitacoraRepo.ObtenerTodos();
        }
    }
}
