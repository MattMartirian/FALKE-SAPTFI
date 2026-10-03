using System;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class DefinirClave : System.Web.UI.Page
    {
        private string Token
        {
            get { return Request.QueryString["token"] ?? string.Empty; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            ResultadoToken_TLL validacion = new Usuario_TLL().ValidarTokenContrasena(Token);

            if (validacion.Exito)
            {
                litEmail.Text = Server.HtmlEncode(validacion.Email);
            }
            else
            {
                pnlForm.Visible = false;
                MostrarError(TextoDelMotivo(validacion.Motivo));
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNueva.Text != txtRepetir.Text)
            {
                MostrarError("Las dos contraseñas no coinciden.");
                return;
            }

            try
            {
                ResultadoToken_TLL resultado = new Usuario_TLL().EstablecerContrasenaConToken(Token, txtNueva.Text);

                if (resultado.Exito)
                {
                    pnlForm.Visible = false;
                    pnlError.Visible = false;
                    pnlOk.Visible = true;
                    return;
                }

                if (resultado.Motivo != "CONTRASENA_DEBIL")
                {
                    pnlForm.Visible = false;
                }

                MostrarError(TextoDelMotivo(resultado.Motivo));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("DefinirClave", ex);
                MostrarError("Ocurrió un error al procesar la solicitud. Volvé a intentarlo.");
            }
        }

        private void MostrarError(string texto)
        {
            litError.Text = Server.HtmlEncode(texto);
            pnlError.Visible = true;
        }

        private static string TextoDelMotivo(string motivo)
        {
            switch (motivo)
            {
                case "CONTRASENA_DEBIL":
                    return "La contraseña tiene que tener al menos 8 caracteres.";

                case "TOKEN_EXPIRADO":
                    return "El enlace venció. Pedí uno nuevo desde «Recuperar mi contraseña».";

                case "TOKEN_USADO":
                    return "Este enlace ya fue utilizado. Si necesitás cambiar la contraseña, pedí uno nuevo.";

                default:
                    return "El enlace no es válido. Revisá que hayas copiado la dirección completa.";
            }
        }
    }
}
