using System;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class RecuperarClave : PaginaBase_GUI
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnEnviarEnlace_Click(object sender, EventArgs e)
        {
            string email = recEmail.Text.Trim();

            if (email.Length == 0)
            {
                litError.Text = IdiomaActual_GUI.Texto("recuperar_escribir_correo");
                avisoError.Visible = true;
                return;
            }

            try
            {
                SolicitudEnlace_TLL solicitud = new Contrasena_TLL().SolicitarEnlace(email);

                if (solicitud != null)
                {
                    string enlace = WebHelper.UrlAbsoluta("DefinirClave.aspx?token=" + Uri.EscapeDataString(solicitud.Token));

                    string cuerpo =
                        IdiomaActual_GUI.Texto("correo_recuperar_pedido") + Environment.NewLine +
                        IdiomaActual_GUI.Texto("correo_recuperar_enlace") + Environment.NewLine +
                        enlace + Environment.NewLine + Environment.NewLine +
                        IdiomaActual_GUI.Texto("correo_recuperar_ignorar") + Environment.NewLine;

                    Email_SERVICE.Enviar(email, IdiomaActual_GUI.Texto("recuperar_mail_asunto"), cuerpo);
                }

                bloqueFormulario.Visible = false;
                avisoEnviado.Visible = true;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("RecuperarClave", ex);
                litError.Text = IdiomaActual_GUI.Texto("comun_error_procesar");
                avisoError.Visible = true;
            }
        }
    }
}
