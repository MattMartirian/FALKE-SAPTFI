using System;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Ingresar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && SesionActual_GUI.RedirigirSiAutenticado()) return;

            if (!IsPostBack) txtEmail.Focus();
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (email.Length == 0 || txtPass.Text.Length == 0)
            {
                Avisar("Completa el correo y la contraseña para continuar.");
                return;
            }

            try
            {
                var resultado = new Usuario_TLL().ValidarCredenciales(email, txtPass.Text);

                if (resultado.Exito)
                {
                    IniciarSesion(resultado.Usuario);
                    Session.Remove("RolVistaPrevia");

                    if (chkRecordarme.Checked)
                        SesionActual_GUI.RecordarEnEsteEquipo(resultado.Usuario.IdUsuario);

                    Response.Redirect("Panel.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Avisar(TextoDelMotivo(resultado.Motivo));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Ingresar", ex);
                Avisar("Ocurrió un error al procesar la solicitud. Vuelve a intentarlo.");
            }
        }

        private static string TextoDelMotivo(string motivo)
        {
            switch (motivo)
            {
                case "CREDENCIALES_INVALIDAS":
                    return "El correo o la contraseña no son correctos.";

                case "USUARIO_BLOQUEADO":
                    return "La cuenta está bloqueada por intentos fallidos. Usa «Olvidé mi contraseña» para recuperar el acceso.";

                case "USUARIO_PENDIENTE":
                    return "La cuenta todavía no fue activada. Revisa el correo con el enlace de activación.";

                case "DVH_INVALIDO":
                    return "El sistema no está disponible en este momento. Comunícate con el administrador o con soporte técnico.";

                default:
                    return "No se pudo iniciar sesión. Vuelve a intentarlo.";
            }
        }

        private void Avisar(string texto)
        {
            litMensaje.Text = Server.HtmlEncode(texto);
            pnlMensaje.Visible = true;
        }

        private static void IniciarSesion(Usuario_TE usuario)
        {
            string nombre = (usuario.NombreUsuario + " " + usuario.ApellidoUsuario).Trim();
            string rol = usuario.Rol != null ? usuario.Rol.Nombre : string.Empty;

            SesionActual_GUI.Iniciar(usuario.IdUsuario, usuario.EmailUsuario, nombre, rol, usuario.IdEmpresa, usuario.EsCuentaEmergencia, usuario.Rol);
        }
    }
}
