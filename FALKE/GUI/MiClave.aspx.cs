using System;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class MiClave : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Exigir()) return;

            if (SesionActual_GUI.EsEmergencia)
            {
                claveActual.Enabled = false;
                claveNueva.Enabled = false;
                claveRepetir.Enabled = false;
                btnGuardarClave.Enabled = false;
                Avisar("aviso-alerta", "El acceso de emergencia no tiene una contraseña en la base: no se puede cambiar desde acá.");
            }
        }

        protected void btnGuardarClave_Click(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Exigir()) return;

            if (SesionActual_GUI.EsEmergencia) return;

            if (claveNueva.Text != claveRepetir.Text)
            {
                Avisar("aviso-peligro", "Las dos contraseñas nuevas no coinciden.");
                return;
            }

            try
            {
                string error;
                bool ok = new Usuario_TLL().CambiarContrasena(SesionActual_GUI.Email, claveActual.Text, claveNueva.Text, out error);

                if (ok)
                {
                    claveActual.Text = string.Empty;
                    claveNueva.Text = string.Empty;
                    claveRepetir.Text = string.Empty;
                    Avisar("aviso-exito", "La contraseña quedó actualizada.");
                    return;
                }

                Avisar("aviso-peligro", error);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("MiClave", ex);
                Avisar("aviso-peligro", "Ocurrió un error al procesar la solicitud. Volvé a intentarlo.");
            }
        }

        private void Avisar(string variante, string texto)
        {
            pnlAviso.CssClass = "aviso " + variante;
            litAviso.Text = Server.HtmlEncode(texto);
            pnlAviso.Visible = true;
        }
    }
}
