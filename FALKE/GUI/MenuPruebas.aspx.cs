using System;

namespace GUI
{
    public partial class MenuPruebas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblSesion.Text = SesionActual.HayUsuario
                ? "Sesion iniciada como: " + SesionActual.Nombre + " (" + SesionActual.Email + ")"
                : "No hay ninguna sesion iniciada.";
        }
    }
}
