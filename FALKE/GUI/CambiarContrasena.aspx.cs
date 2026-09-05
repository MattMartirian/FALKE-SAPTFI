using System;
using TLL;

namespace GUI
{
    public partial class CambiarContrasena : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual.Exigir()) return;

            if (!IsPostBack)
                lblUsuario.Text = SesionActual.Email;
        }

        protected void btnCambiar_Click(object sender, EventArgs e)
        {
            if (!SesionActual.Exigir()) return;

            if (txtNueva.Text != txtRepetir.Text)
            {
                lblMsg.Text = "Las contrasenas nuevas no coinciden.";
                return;
            }

            try
            {
                string error;
                bool ok = new UsuarioTLL().CambiarContrasena(
                    SesionActual.Email, txtActual.Text, txtNueva.Text, out error);

                lblMsg.Text = ok ? "Contrasena actualizada." : error;
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar("CambiarContrasena", ex);
                lblMsg.Text = "Ocurrio un error al procesar la solicitud. Intente nuevamente.";
            }
        }
    }
}
