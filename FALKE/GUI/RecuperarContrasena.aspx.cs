using System;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class RecuperarContrasena : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRecuperar_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string token = new Usuario_TLL().SolicitarRecuperacion(email);

                // token == null  =>  el email no existe. No se revela esa diferencia.
                if (token != null)
                {
                    string link = WebHelper.UrlAbsoluta(
                        "EstablecerContrasena.aspx?token=" + Uri.EscapeDataString(token));

                    string cuerpo =
                        "Se solicito restablecer la contrasena de tu cuenta de FALKE." + Environment.NewLine +
                        "Si fuiste vos, entra a este enlace (vence en 2 horas):" + Environment.NewLine +
                        link + Environment.NewLine + Environment.NewLine +
                        "Si no fuiste vos, ignora este mensaje: tu contrasena actual sigue siendo valida." + Environment.NewLine;

                    Email_SERVICE.Enviar(email, "Restablecer contrasena de FALKE", cuerpo);
                }

                lblMsg.Text = "Si el email esta registrado, te enviamos un enlace para definir una nueva " +
                              "contrasena (revisa la carpeta App_Data/mails).";
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("RecuperarContrasena", ex);
                lblMsg.Text = "Ocurrio un error al procesar la solicitud. Intente nuevamente.";
            }
        }
    }
}
