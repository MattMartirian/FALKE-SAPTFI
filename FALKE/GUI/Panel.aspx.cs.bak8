using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class PaginaPrincipal : PaginaBase_GUI
    {
        private const int FILAS = 5;

        public class FilaSesion
        {
            public string Nombre { get; set; }
            public string Categoria { get; set; }
            public string Fecha { get; set; }
            public string Duracion { get; set; }
        }

        public class FilaCategoria
        {
            public string Nombre { get; set; }
            public int Cantidad { get; set; }
            public int Porcentaje { get; set; }
        }

        protected string SesionesMes = "-";
        protected string SesionesMesDetalle = "";
        protected string SesionesMesClase = "texto-suave";
        protected string TiempoMes = "-";
        protected string TiempoMesDetalle = "";
        protected string CategoriasActivas = "-";
        protected string CategoriasDetalle = "";
        protected string Dispositivos = "-";
        protected string DispositivosDetalle = "";

        protected bool HaySesiones;
        protected bool HayCategoriasConSesiones;
        protected bool HayActividad;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Exigir()) return;

            litNombre.Text = Server.HtmlEncode(NombreParaSaludo());

            ActorUsuario_TE actor = SesionActual_GUI.ObtenerActor();

            if (actor == null) return;

            CargarSesiones(actor);
            CargarCategorias(actor);
            CargarDispositivos(actor);
            CargarActividad(actor);
        }

        private static string NombreParaSaludo()
        {
            string nombre = SesionActual_GUI.Nombre;

            if (string.IsNullOrWhiteSpace(nombre)) return "usuario";

            return nombre.Trim().Split(' ')[0];
        }

        private void CargarSesiones(ActorUsuario_TE actor)
        {
            try
            {
                List<SesionGrabada_BE> sesiones = new SesionGrabada_BLL().Listar(actor);
                CultureInfo cultura = IdiomaActual_GUI.Cultura;

                DateTime inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime inicioMesPasado = inicioMes.AddMonths(-1);

                List<SesionGrabada_BE> delMes = sesiones.Where(s => s.Fecha >= inicioMes).ToList();
                int delMesPasado = sesiones.Count(s => s.Fecha >= inicioMesPasado && s.Fecha < inicioMes);
                int diferencia = delMes.Count - delMesPasado;

                SesionesMes = delMes.Count.ToString(cultura);

                if (diferencia == 0)
                {
                    SesionesMesDetalle = Traducir("panel_igual_mes_pasado");
                }
                else
                {
                    SesionesMesDetalle = Traducir("panel_respecto_mes_pasado", (diferencia > 0 ? "+" : "") + diferencia.ToString(cultura));
                    SesionesMesClase = diferencia > 0 ? "texto-exito" : "texto-suave";
                }

                int segundos = delMes.Sum(s => s.DuracionSegundos);

                TiempoMes = FormatoDuracion(segundos);
                TiempoMesDetalle = Traducir("panel_promedio_por_sesion", delMes.Count == 0 ? "-" : FormatoDuracion(segundos / delMes.Count));

                HaySesiones = sesiones.Count > 0;

                rptSesiones.DataSource = sesiones.Take(FILAS).Select(s => new FilaSesion
                {
                    Nombre = s.Nombre,
                    Categoria = s.NombreCategoria,
                    Fecha = s.Fecha.ToString("dd MMM yyyy", cultura),
                    Duracion = FormatoDuracion(s.DuracionSegundos)
                }).ToList();
                rptSesiones.DataBind();
            }
            catch (ExcepcionNoAutorizada_TLL)
            {
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Panel.Sesiones", ex);
            }
        }

        private void CargarCategorias(ActorUsuario_TE actor)
        {
            try
            {
                List<Categoria_BE> categorias = new Categoria_BLL().ObtenerPorEmpresa(actor);
                List<Categoria_BE> activas = categorias.Where(c => c.Activa).ToList();

                CategoriasActivas = activas.Count.ToString(IdiomaActual_GUI.Cultura);
                CategoriasDetalle = Traducir("panel_tipos_de_activo_n", activas.Select(c => c.Tipo).Distinct().Count());

                List<Categoria_BE> conSesiones = categorias.Where(c => c.CantidadSesiones > 0).OrderByDescending(c => c.CantidadSesiones).Take(FILAS).ToList();
                int maximo = conSesiones.Count == 0 ? 1 : conSesiones[0].CantidadSesiones;

                HayCategoriasConSesiones = conSesiones.Count > 0;

                rptCategorias.DataSource = conSesiones.Select(c => new FilaCategoria
                {
                    Nombre = c.NombreCategoria,
                    Cantidad = c.CantidadSesiones,
                    Porcentaje = Math.Max(4, c.CantidadSesiones * 100 / maximo)
                }).ToList();
                rptCategorias.DataBind();
            }
            catch (ExcepcionNoAutorizada_TLL)
            {
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Panel.Categorias", ex);
            }
        }

        private void CargarDispositivos(ActorUsuario_TE actor)
        {
            try
            {
                List<Dispositivo_BE> propios = new Dispositivo_BLL().ObtenerDeMiEmpresa(actor).Where(d => d.Estado != EstadoDispositivo.DeBaja).ToList();

                Dispositivos = propios.Count.ToString(IdiomaActual_GUI.Cultura);

                if (propios.Count == 0)
                {
                    DispositivosDetalle = Traducir("panel_sin_dispositivos");
                }
                else if (propios.Count == 1)
                {
                    DispositivosDetalle = propios[0].Modelo + " · " + Traducir(Dispositivo_BLL.ClaveEstado(propios[0].Estado));
                }
                else
                {
                    DispositivosDetalle = Traducir("panel_dispositivos_disponibles", propios.Count(d => d.Estado == EstadoDispositivo.Disponible));
                }
            }
            catch (ExcepcionNoAutorizada_TLL)
            {
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Panel.Dispositivos", ex);
            }
        }

        private void CargarActividad(ActorUsuario_TE actor)
        {
            try
            {
                List<Bitacora_TE> actividad = new BitacoraGestor_TLL().ObtenerMiActividad(actor, FILAS);

                HayActividad = actividad.Count > 0;

                rptActividad.DataSource = actividad;
                rptActividad.DataBind();
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Panel.Actividad", ex);
            }
        }

        protected string DetalleActividad(object detalle)
        {
            string texto = TextoBitacora_GUI.Detalle(detalle);

            return texto == "-" ? "" : texto;
        }

        private static string FormatoDuracion(int segundos)
        {
            var t = TimeSpan.FromSeconds(Math.Max(0, segundos));
            return t.TotalHours >= 1 ? string.Format("{0}:{1:00}:{2:00}", (int)t.TotalHours, t.Minutes, t.Seconds) : string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
        }
    }
}
