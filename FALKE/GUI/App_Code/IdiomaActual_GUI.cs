using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public static class IdiomaActual_GUI
    {
        private const string K_SUJETO = "IdiomaSujeto";
        private const string K_USUARIO = "IdiomaUsuario";
        private const string COOKIE = "FALKE_IDIOMA";
        private const string COOKIE_EMERGENCIA = "FALKE_IDIOMA_EMERGENCIA";
        private const int DIAS_COOKIE = 365;

        private static HttpContext Ctx
        {
            get { return HttpContext.Current; }
        }

        public static string Codigo
        {
            get { return Sujeto.CodigoIdioma; }
        }

        public static string Texto(string clave, params object[] parametros)
        {
            string texto = Traductor_TLL.Traducir(clave, Codigo, parametros);

            OrigenTexto_GUI.Registrar(texto, clave, parametros);

            return texto;
        }

        public static CultureInfo Cultura
        {
            get { return Traductor_TLL.Cultura(Codigo); }
        }

        public static int IdIdiomaBase()
        {
            return new Idioma_TLL().ObtenerPorCodigo(Traductor_TLL.CODIGO_BASE).IdIdioma;
        }

        public static SujetoIdioma_TLL Sujeto
        {
            get
            {
                var sujeto = Ctx.Session[K_SUJETO] as SujetoIdioma_TLL;

                if (sujeto == null)
                {
                    sujeto = new SujetoIdioma_TLL(Traductor_TLL.CODIGO_BASE);
                    Ctx.Session[K_SUJETO] = sujeto;
                }

                Sincronizar(sujeto);

                return sujeto;
            }
        }

        public static void Cambiar(string codigo)
        {
            Idioma_TE idioma = Activos().FirstOrDefault(i => string.Equals(i.CodigoIdioma, codigo, StringComparison.OrdinalIgnoreCase));

            if (idioma == null) return;

            if (SesionActual_GUI.HayUsuario && !SesionActual_GUI.EsEmergencia)
                new Usuario_TLL().CambiarIdioma(SesionActual_GUI.IdUsuario, idioma.IdIdioma);

            if (SesionActual_GUI.HayUsuario && SesionActual_GUI.EsEmergencia) GuardarCookie(COOKIE_EMERGENCIA, idioma.CodigoIdioma);

            GuardarCookie(COOKIE, idioma.CodigoIdioma);
            Sujeto.CambiarIdioma(idioma.CodigoIdioma);
        }

        private static void Sincronizar(SujetoIdioma_TLL sujeto)
        {
            SesionActual_GUI.RestaurarDesdeCookie();

            int usuario = SesionActual_GUI.HayUsuario ? SesionActual_GUI.IdUsuario : 0;
            object resuelto = Ctx.Session[K_USUARIO];

            if (resuelto != null && (int)resuelto == usuario) return;

            Ctx.Session[K_USUARIO] = usuario;

            string codigo = Resolver(usuario);

            if (usuario != 0) GuardarCookie(COOKIE, codigo);

            if (!string.Equals(codigo, sujeto.CodigoIdioma, StringComparison.OrdinalIgnoreCase)) sujeto.CambiarIdioma(codigo);
        }

        private static string Resolver(int idUsuario)
        {
            try
            {
                List<Idioma_TE> activos = Activos();

                if (idUsuario > 0 && !SesionActual_GUI.EsEmergencia)
                {
                    Usuario_TE usuario = new Usuario_TLL().ObtenerPorId(idUsuario);
                    Idioma_TE propio = usuario == null ? null : activos.FirstOrDefault(i => i.IdIdioma == usuario.IdIdioma);

                    if (propio != null) return propio.CodigoIdioma;
                }

                string nombreCookie = idUsuario != 0 && SesionActual_GUI.EsEmergencia && Ctx.Request.Cookies[COOKIE_EMERGENCIA] != null ? COOKIE_EMERGENCIA : COOKIE;
                HttpCookie cookie = Ctx.Request.Cookies[nombreCookie];
                Idioma_TE elegido = cookie == null ? null : activos.FirstOrDefault(i => string.Equals(i.CodigoIdioma, cookie.Value, StringComparison.OrdinalIgnoreCase));

                if (elegido != null) return elegido.CodigoIdioma;

                Idioma_TE navegador = DelNavegador(activos);

                if (navegador != null) return navegador.CodigoIdioma;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("IdiomaActual.Resolver", ex);
            }

            return Traductor_TLL.CODIGO_BASE;
        }

        private static Idioma_TE DelNavegador(List<Idioma_TE> activos)
        {
            string[] preferidos = Ctx.Request.UserLanguages;

            if (preferidos == null) return null;

            foreach (string preferido in preferidos)
            {
                if (string.IsNullOrWhiteSpace(preferido)) continue;

                string etiqueta = preferido.Split(';')[0].Trim();
                string primario = etiqueta.Split('-')[0];

                Idioma_TE exacto = activos.FirstOrDefault(i => string.Equals(i.CodigoIdioma, etiqueta, StringComparison.OrdinalIgnoreCase));

                if (exacto != null) return exacto;

                Idioma_TE general = activos.FirstOrDefault(i => string.Equals(i.CodigoIdioma, primario, StringComparison.OrdinalIgnoreCase));

                if (general != null) return general;
            }

            return null;
        }

        private static List<Idioma_TE> Activos()
        {
            return new Idioma_TLL().ListarActivos();
        }

        private static void GuardarCookie(string nombre, string codigo)
        {
            var cookie = new HttpCookie(nombre, codigo)
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(DIAS_COOKIE)
            };

            Ctx.Response.Cookies.Set(cookie);
        }
    }
}
