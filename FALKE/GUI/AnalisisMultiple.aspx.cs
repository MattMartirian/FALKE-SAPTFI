using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using BE;
using BLL;
using SERVICES;
using TE;

namespace GUI
{
    public partial class AnalisisMultiple : PaginaBase_GUI
    {
        public class SesionItem
        {
            public string Token { get; set; }
            public string Nombre { get; set; }
            public int IdCategoria { get; set; }
            public string Categoria { get; set; }
            public string Fecha { get; set; }
            public string Duracion { get; set; }
            public string Analista { get; set; }
            public int DuracionSegundos { get; set; }
            public int DesfaseMs { get; set; }
            public int Muestras { get; set; }
            public string Edad { get; set; }
            public string Genero { get; set; }
            public string Familiaridad { get; set; }
        }

        protected bool HayError;
        protected bool HaySesiones;

        // los grupos del tester (edad, genero, familiaridad) con su texto traducido, en JSON para el navegador
        protected string GruposJson;

        // url de un archivo estatico (css / js) con su fecha de modificacion, asi el navegador no usa una copia vieja del cache
        protected string UrlVersionada(string rutaVirtual)
        {
            return ResolveUrl(rutaVirtual) + "?v=" + System.IO.File.GetLastWriteTimeUtc(Server.MapPath(rutaVirtual)).Ticks;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(TLL.Patentes_TLL.OPERAR_ANALISIS)) return;

            CargarSesiones();
            GruposJson = "{\"edad\":" + ListaJson(PerfilTester_BE.RangosEdad, null) +
                         ",\"genero\":" + ListaJson(PerfilTester_BE.Generos, "grabacion_tester_genero_") +
                         ",\"familiaridad\":" + ListaJson(PerfilTester_BE.Familiaridades, "grabacion_tester_familiaridad_") + "}";
        }

        private string ListaJson(string[] valores, string prefijo)
        {
            var partes = new List<string>();

            foreach (string v in valores)
            {
                string texto = prefijo == null ? v : Traducir(prefijo + v);
                partes.Add("{\"valor\":\"" + HttpUtility.JavaScriptStringEncode(v) + "\",\"texto\":\"" + HttpUtility.JavaScriptStringEncode(texto) + "\"}");
            }

            return "[" + string.Join(",", partes.ToArray()) + "]";
        }

        // lista las sesiones de la empresa del usuario (el BLL toma la empresa del actor, no del navegador)
        // al navegador le llega un token al azar por sesion, igual que en Visualizaciones
        private void CargarSesiones()
        {
            var items = new List<SesionItem>();
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
                        IdCategoria = s.IdCategoria,
                        Categoria = s.NombreCategoria,
                        Fecha = s.Fecha.ToString("dd MMM yyyy", cultura),
                        Duracion = FormatoDuracion(s.DuracionSegundos),
                        Analista = s.NombreAnalista,
                        DuracionSegundos = s.DuracionSegundos,
                        DesfaseMs = s.DesfaseMiradaMs,
                        Muestras = s.Muestras,
                        Edad = s.RangoEdad,
                        Genero = s.Genero,
                        Familiaridad = s.Familiaridad
                    });
                }
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("AnalisisMultiple.Listar", ex);
                HayError = true;
                items.Clear();
            }

            Session[GrabacionArchivo_GUI.CLAVE_SESION] = tokens;
            HaySesiones = items.Count > 0;

            rptSesiones.DataSource = items;
            rptSesiones.DataBind();
        }

        private static string FormatoDuracion(int segundos)
        {
            var t = TimeSpan.FromSeconds(Math.Max(0, segundos));
            return t.TotalHours >= 1 ? string.Format("{0}:{1:00}:{2:00}", (int)t.TotalHours, t.Minutes, t.Seconds) : string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
        }
    }
}
