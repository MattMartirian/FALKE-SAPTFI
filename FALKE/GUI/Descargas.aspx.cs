using System;
using System.IO;
using TLL;

namespace GUI
{
    public partial class Descargas : PaginaConAviso_GUI
    {
        private static readonly bool MODO_PRUEBA = true;

        // el zip con todo el software (agente + instalador de Tobii) se arma a mano y se deja en esta carpeta (viaja con la publicacion del sitio)
        private const string CARPETA = "~/Instaladores";
        private const string ARCHIVO = "FALKE_SW.zip";

        private bool permitido;

        protected void Page_Load(object sender, EventArgs e)
        {
            permitido = SesionActual_GUI.ExigirPermiso(Patentes_TLL.OPERAR_ANALISIS);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!permitido) return;

            bool disponible = File.Exists(RutaArchivo());

            btnTodo.Enabled = disponible || MODO_PRUEBA;
            litIncluye.Text = disponible ? Server.HtmlEncode(Traducir("descargas_tobii_titulo") + " · " + Traducir("descargas_agente_titulo")) : "";

            if (!disponible) Avisar("aviso-alerta", Traducir("descargas_sin_instaladores"));
        }

        protected void btnTodo_Click(object sender, EventArgs e)
        {
            if (!File.Exists(RutaArchivo()))
            {
                if (MODO_PRUEBA) descargaPasos.Attributes.Remove("hidden");

                return;
            }

            Intentar(EnviarArchivo, "Descargas.Zip", mensaje => Avisar("aviso-peligro", mensaje));
        }

        private string RutaArchivo()
        {
            return Path.Combine(Server.MapPath(CARPETA), ARCHIVO);
        }

        // manda el zip tal cual esta en el servidor (TransmitFile no lo carga entero en memoria)
        private void EnviarArchivo()
        {
            Response.Clear();
            Response.ContentType = "application/zip";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + ARCHIVO + "\"");
            Response.TransmitFile(RutaArchivo());
            Response.Flush();
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
