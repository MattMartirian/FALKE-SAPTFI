using System;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class RecuperarClave : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnEnviarEnlace_Click(object sender, EventArgs e)
        {
            string email = recEmail.Text.Trim();

            if (email.Length == 0)
            {
                litError.Text = "Escribí el correo de tu cuenta para continuar.";
                avisoError.Visible = true;
                return;
            }

            try
            {
                SolicitudEnlace_TLL solicitud = new Usuario_TLL().SolicitarEnlace(email);

                if (solicitud != null)
                {
                    string enlace = WebHelper.UrlAbsoluta("DefinirClave.aspx?token=" + Uri.EscapeDataString(solicitud.Token));

                    string cuerpo =
                        "Recibimos un pedido para restablecer la contraseña de tu cuenta de Falke." + Environment.NewLine +
                        "Si fuiste vos, entrá a este enlace (vence en dos horas y se usa una sola vez):" + Environment.NewLine +
                        enlace + Environment.NewLine + Environment.NewLine +
                        "Si no fuiste vos, ignorá este mensaje: tu contraseña actual sigue siendo válida." + Environment.NewLine;

                    Email_SERVICE.Enviar(email, "Restablecer la contraseña de Falke", cuerpo);
                }

                bloqueFormulario.Visible = false;
                avisoEnviado.Visible = true;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("RecuperarClave", ex);
                litError.Text = "Ocurrió un error al procesar la solicitud. Volvé a intentarlo.";
                avisoError.Visible = true;
            }
        }
    }
}
