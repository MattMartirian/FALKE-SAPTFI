using System;

namespace GUI
{
    public partial class PaginaPrincipal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Exigir()) return;

            litNombre.Text = Server.HtmlEncode(NombreParaSaludo());
        }

        private static string NombreParaSaludo()
        {
            string nombre = SesionActual_GUI.Nombre;

            if (string.IsNullOrWhiteSpace(nombre)) return "usuario";

            return nombre.Trim().Split(' ')[0];
        }
    }
}
