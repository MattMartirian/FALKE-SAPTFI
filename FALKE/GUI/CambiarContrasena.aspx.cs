using System;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class CambiarContrasena : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Exigir()) return;

            if (!IsPostBack)
                lblUsuario.Text = SesionActual_GUI.Email;
        }

        protected void btnCambiar_Click(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Exigir()) return;

            if (txtNueva.Text != txtRepetir.Text)
            {
                lblMsg.Text = "Las contrasenas nuevas no coinciden.";
                return;
            }

            try
            {
                string error;
                bool ok = new Usuario_TLL().CambiarContrasena(SesionActual_GUI.Email, txtActual.Text, txtNueva.Text, out error);

                lblMsg.Text = ok ? "Contrasena actualizada." : error;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("CambiarContrasena", ex);
                lblMsg.Text = "Ocurrio un error al procesar la solicitud. Intente nuevamente.";
            }
        }
    }
}
