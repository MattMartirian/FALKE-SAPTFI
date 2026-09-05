using System;

namespace GUI
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblEstado.Text = SesionActual.HayUsuario
                    ? "Sesion iniciada como: " + SesionActual.Email
                    : "No hay ninguna sesion iniciada.";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            SesionActual.Cerrar();
            Response.Redirect("Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
