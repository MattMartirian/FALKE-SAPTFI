using System;
using TLL;

namespace GUI
{
    public partial class EstablecerContrasena : System.Web.UI.Page
    {
        private string Token
        {
            get { return Request.QueryString["token"] ?? string.Empty; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            ResultadoToken val = new UsuarioTLL().ValidarTokenContrasena(Token);

            if (val.Exito)
            {
                lblEmail.Text = val.Email;
            }
            else
            {
                pnlForm.Visible = false;
                lblMsg.Text = MensajeMotivo(val.Motivo);
            }
        }

        protected void btnEstablecer_Click(object sender, EventArgs e)
        {
            if (txtNueva.Text != txtRepetir.Text)
            {
                lblMsg.Text = "Las contrasenas no coinciden.";
                return;
            }

            try
            {
                ResultadoToken res = new UsuarioTLL().EstablecerContrasenaConToken(Token, txtNueva.Text);

                if (res.Exito)
                {
                    pnlForm.Visible = false;
                    lblMsg.Text = "Listo: la contrasena quedo definida y la cuenta esta activa. Ya podes iniciar sesion.";
                    return;
                }

                // Si el token dejo de ser valido, no tiene sentido reintentar: se oculta el form.
                if (res.Motivo != "CONTRASENA_DEBIL")
                    pnlForm.Visible = false;

                lblMsg.Text = MensajeMotivo(res.Motivo);
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar("EstablecerContrasena", ex);
                lblMsg.Text = "Ocurrio un error al procesar la solicitud. Intente nuevamente.";
            }
        }

        private static string MensajeMotivo(string motivo)
        {
            switch (motivo)
            {
                case "TOKEN_EXPIRADO":
                    return "El enlace vencio. Solicita uno nuevo desde 'Recuperar contrasena'.";
                case "TOKEN_USADO":
                    return "Este enlace ya fue utilizado.";
                case "CONTRASENA_DEBIL":
                    return "La contrasena debe tener al menos 8 caracteres.";
                default:
                    return "El enlace no es valido.";
            }
        }
    }
}
