using System;

namespace GUI
{
    public partial class PaginaPrincipal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            litNombre.Text = Server.HtmlEncode(NombreParaSaludo());
        }

        private static string NombreParaSaludo()
        {
            if (!SesionActual_GUI.HayUsuario) return "María";

            string nombre = SesionActual_GUI.Nombre;

            if (string.IsNullOrWhiteSpace(nombre)) return "usuario";

            return nombre.Trim().Split(' ')[0];
        }
    }
}
