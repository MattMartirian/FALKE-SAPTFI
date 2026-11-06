using System;
using System.Collections.Generic;
using System.Globalization;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Visualizaciones : PaginaBase_GUI
    {
        public class SesionItem
        {
            public string Token { get; set; }
            public string Nombre { get; set; }
            public string Categoria { get; set; }
            public string Tipo { get; set; }
            public string TipoClave { get; set; }
            public string Fecha { get; set; }
            public string Duracion { get; set; }
            public int DuracionSegundos { get; set; }
            public string Analista { get; set; }
            public string Objetivo { get; set; }
            public int Muestras { get; set; }
            public string Edad { get; set; }
            public string Genero { get; set; }
            public string GeneroTexto { get; set; }
            public string Familiaridad { get; set; }
            public string FamiliaridadTexto { get; set; }
            public string Perfil { get; set; }
            public int DesfaseMs { get; set; }
        }

        public class Opcion
        {
            public string Valor { get; set; }
            public string Texto { get; set; }
        }

        protected bool HayError;
        protected bool HaySesiones;

        // url de un archivo estatico (css / js) con su fecha de modificacion, asi el navegador no usa una copia vieja del cache
        protected string UrlVersionada(string rutaVirtual)
        {
            return ResolveUrl(rutaVirtual) + "?v=" + System.IO.File.GetLastWriteTimeUtc(Server.MapPath(rutaVirtual)).Ticks;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(TLL.Patentes_TLL.OPERAR_ANALISIS)) return;

            CargarSesiones();
            CargarFiltros();
            CargarIdiomasInforme();
        }

        // La empresa NO viene del navegador: el BLL la toma del actor (sesion del servidor).
        // Cada sesion se le entrega al navegador con un token al azar, y la tabla token -> ID queda solo en la Session
        // del servidor. El navegador nunca ve un ID real ni puede pedir uno que no se le haya dado.
        private void CargarSesiones()
        {
            var items = new List<SesionItem>();
            // Se juntan con los de cargas anteriores (por si hay dos pestanas abiertas). Igual el BLL revalida la empresa en cada pedido.
            var tokens = Session[GrabacionArchivo_GUI.CLAVE_SESION] as Dictionary<string, int> ?? new Dictionary<string, int>();
            if (tokens.Count > 5000) tokens.Clear();

            try
            {
                ActorUsuario_TE actor = SesionActual_GUI.ObtenerActor();
                if (actor == null) return;

                CultureInfo cultura = IdiomaActual_GUI.Cultura;

                foreach (SesionGrabada_BE s in new SesionGrabada_BLL().Listar(actor))
                {
                    string token = Guid.NewGuid().ToString("N");
                    tokens[token] = s.IdSesion;

                    items.Add(new SesionItem
                    {
                        Token = token,
                        Nombre = s.Nombre,
                        Categoria = s.NombreCategoria,
                        Tipo = EtiquetaTipo(s.TipoActivo),
                        TipoClave = (s.TipoActivo ?? string.Empty).ToLowerInvariant(),
                        Fecha = s.Fecha.ToString("dd MMM yyyy", cultura),
                        Duracion = FormatoDuracion(s.DuracionSegundos),
                        DuracionSegundos = s.DuracionSegundos,
                        Analista = s.NombreAnalista,
                        Objetivo = s.Objetivo,
                        Muestras = s.Muestras,
                        Edad = s.RangoEdad,
                        Genero = s.Genero,
                        GeneroTexto = TextoOpcion("grabacion_tester_genero_", s.Genero),
                        Familiaridad = s.Familiaridad,
                        FamiliaridadTexto = TextoOpcion("grabacion_tester_familiaridad_", s.Familiaridad),
                        Perfil = Traducir("vis_tester_perfil", s.RangoEdad, TextoOpcion("grabacion_tester_genero_", s.Genero),
                                          TextoOpcion("grabacion_tester_familiaridad_", s.Familiaridad).ToLower(cultura)),
                        DesfaseMs = s.DesfaseMiradaMs
                    });
                }
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Visualizaciones.Listar", ex);
                HayError = true;
                items.Clear();
            }

            Session[GrabacionArchivo_GUI.CLAVE_SESION] = tokens;
            HaySesiones = items.Count > 0;

            rptSesiones.DataSource = items;
            rptSesiones.DataBind();
        }

        private void CargarFiltros()
        {
            rptFiltroEdad.DataSource = Opciones(PerfilTester_BE.RangosEdad, null);
            rptFiltroEdad.DataBind();
            rptFiltroGenero.DataSource = Opciones(PerfilTester_BE.Generos, "grabacion_tester_genero_");
            rptFiltroGenero.DataBind();
            rptFiltroFamiliaridad.DataSource = Opciones(PerfilTester_BE.Familiaridades, "grabacion_tester_familiaridad_");
            rptFiltroFamiliaridad.DataBind();
        }

        private void CargarIdiomasInforme()
        {
            try
            {
                rptIdiomasInforme.DataSource = new Idioma_TLL().ListarActivos();
                rptIdiomasInforme.DataBind();
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Visualizaciones.Idiomas", ex);
            }
        }

        protected string IdiomaElegido(string codigo)
        {
            return string.Equals(codigo, IdiomaActual_GUI.Codigo, StringComparison.OrdinalIgnoreCase) ? " selected" : "";
        }

        private List<Opcion> Opciones(string[] valores, string prefijo)
        {
            var lista = new List<Opcion>();
            foreach (string v in valores) lista.Add(new Opcion { Valor = v, Texto = prefijo == null ? v : Traducir(prefijo + v) });
            return lista;
        }

        private string TextoOpcion(string prefijo, string valor)
        {
            return string.IsNullOrEmpty(valor) ? "-" : Traducir(prefijo + valor);
        }

        private static string FormatoDuracion(int segundos)
        {
            var t = TimeSpan.FromSeconds(Math.Max(0, segundos));
            return t.TotalHours >= 1 ? string.Format("{0}:{1:00}:{2:00}", (int)t.TotalHours, t.Minutes, t.Seconds) : string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
        }

        private static string EtiquetaTipo(string tipo)
        {
            switch (tipo)
            {
                case "AppWeb": return IdiomaActual_GUI.Texto("analisismultiple_app_web");
                case "AppMovil": return IdiomaActual_GUI.Texto("analisismultiple_app_movil");
                case "Videojuego": return IdiomaActual_GUI.Texto("analisismultiple_videojuego");
                case "Publicidad": return IdiomaActual_GUI.Texto("categorias_publicidad");
                case "Software": return IdiomaActual_GUI.Texto("analisismultiple_software");
                default: return tipo ?? string.Empty;
            }
        }
    }
}
