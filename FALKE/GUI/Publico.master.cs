using System;
using System.Web.UI.HtmlControls;

namespace GUI
{
    public partial class PublicoMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SesionActual_GUI.RestaurarDesdeCookie();

            MarcarPaginaActual();

            bool haySesion = SesionActual_GUI.HayUsuario;
            phAnonimo.Visible = !haySesion;
            phAutenticado.Visible = haySesion;
        }

        private void MarcarPaginaActual()
        {
            string archivo = System.IO.Path.GetFileName(Request.CurrentExecutionFilePath);

            Marcar(navInicio, archivo, "Default.aspx");
            Marcar(navPlanes, archivo, "Planes.aspx");
            Marcar(navFaq, archivo, "Faq.aspx");
        }

        private static void Marcar(HtmlAnchor link, string archivoActual, string archivoDelLink)
        {
            if (!string.Equals(archivoActual, archivoDelLink, StringComparison.OrdinalIgnoreCase)) return;

            link.Attributes["aria-current"] = "page";
        }
    }
}
