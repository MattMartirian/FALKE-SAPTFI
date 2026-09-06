using System;
using TE;
using TLL;

namespace GUI
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblEstado.Text = SesionActual_GUI.HayUsuario
                    ? "Sesion iniciada como: " + SesionActual_GUI.Email
                    : "No hay ninguna sesion iniciada.";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // No hay llamada a TLL/BLL en este flujo: la bitacora se registra desde aca.
            if (SesionActual_GUI.HayUsuario)
            {
                new BitacoraGestor_TLL().Registrar(SesionActual_GUI.IdUsuario, "Seguridad", "Cierre de sesión", CriticidadBitacora.Baja);
            }

            SesionActual_GUI.Cerrar();
            Response.Redirect("Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
