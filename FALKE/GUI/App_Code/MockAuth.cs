using System;
using System.IO;
using System.Web;

namespace GUI
{
    /// <summary>
    /// Simula el envio de mails escribiendo un archivo de texto en ~/App_Data/mails/.
    /// Solo para las pantallas de prueba.
    /// </summary>
    public static class MockMailer
    {
        public static string Enviar(string destino, string asunto, string cuerpo)
        {
            string carpeta = HttpContext.Current.Server.MapPath("~/App_Data/mails");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(
                carpeta,
                DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "_" + Limpiar(destino) + ".txt");

            string contenido =
                "Para:   " + destino + Environment.NewLine +
                "Asunto: " + asunto + Environment.NewLine +
                "Fecha:  " + DateTime.Now.ToString("o") + Environment.NewLine +
                new string('-', 50) + Environment.NewLine +
                cuerpo + Environment.NewLine;

            File.WriteAllText(archivo, contenido);
            return archivo;
        }

        private static string Limpiar(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return "sin-destino";

            foreach (char c in Path.GetInvalidFileNameChars())
                valor = valor.Replace(c, '_');

            return valor.Replace('@', '_');
        }
    }

    /// <summary>
    /// Log de errores a ~/App_Data/logs/errores.log. Nunca propaga fallos propios:
    /// el logging no debe romper el flujo de la pagina.
    /// </summary>
    public static class ErrorLog
    {
        public static void Registrar(string contexto, Exception ex)
        {
            try
            {
                string carpeta = HttpContext.Current.Server.MapPath("~/App_Data/logs");
                Directory.CreateDirectory(carpeta);

                string linea =
                    DateTime.Now.ToString("o") + " | " + contexto + Environment.NewLine +
                    (ex == null ? "(sin excepcion)" : ex.ToString()) + Environment.NewLine +
                    new string('-', 60) + Environment.NewLine;

                File.AppendAllText(Path.Combine(carpeta, "errores.log"), linea);
            }
            catch
            {
                // intencional: el logging nunca debe tirar
            }
        }
    }

    /// <summary>Manejo minimo de la sesion del usuario logueado.</summary>
    public static class SesionActual
    {
        public static bool HayUsuario
        {
            get { return HttpContext.Current.Session["UsuarioEmail"] != null; }
        }

        public static string Email
        {
            get { return (string)HttpContext.Current.Session["UsuarioEmail"]; }
        }

        public static string Nombre
        {
            get { return (string)HttpContext.Current.Session["UsuarioNombre"]; }
        }

        public static void Iniciar(string email, string nombre)
        {
            HttpContext.Current.Session["UsuarioEmail"] = email;
            HttpContext.Current.Session["UsuarioNombre"] = nombre;
        }

        public static void Cerrar()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }

        /// <summary>
        /// Si no hay sesion, encola el redirect a Login y devuelve false.
        /// El llamador debe hacer 'return' inmediatamente (tanto en Page_Load
        /// como en el handler del boton).
        /// </summary>
        public static bool Exigir()
        {
            if (HayUsuario) return true;

            HttpContext ctx = HttpContext.Current;
            ctx.Response.Redirect("Login.aspx", false);
            ctx.ApplicationInstance.CompleteRequest();
            return false;
        }
    }

    /// <summary>Utilidades web para las pantallas de prueba.</summary>
    public static class WebHelper
    {
        /// <summary>Convierte una ruta relativa a la carpeta actual en URL absoluta.</summary>
        public static string UrlAbsoluta(string relativa)
        {
            return new Uri(HttpContext.Current.Request.Url, relativa).AbsoluteUri;
        }
    }
}
