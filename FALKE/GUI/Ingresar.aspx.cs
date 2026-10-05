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
            // Segundo paso del inicio de sesión: ya con una sesión de identificador nuevo, se completa la sesión con el ticket de un solo uso.
            if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString["inicio"]) && CompletarInicioDeSesion()) return;

            if (!IsPostBack && SesionActual_GUI.RedirigirSiAutenticado()) return;

            if (!IsPostBack && Request.QueryString["cuenta"] == "empresa")
                Avisar("Tu sesión se cerró porque la empresa de tu cuenta fue bloqueada. Comunícate con Pattern Blue para regularizar la situación.");

            if (!IsPostBack && Request.QueryString["cuenta"] == "baja")
                Avisar("Tu sesión se cerró porque la empresa de tu cuenta fue dada de baja. Si crees que es un error, comunícate con Pattern Blue.");

            if (!IsPostBack && Request.QueryString["cuenta"] == "clave")
                Avisar("Tu sesión se cerró porque se cambió la contraseña de tu cuenta. Volvé a entrar con la nueva.");

            if (!IsPostBack && Request.QueryString["cuenta"] == "respaldo")
                Avisar("Se restauró la base de datos desde una copia de seguridad y se cerraron todas las sesiones. Volvé a entrar.");

            if (!IsPostBack && Request.QueryString["cuenta"] == "desactivada")
                Avisar("Tu sesión se cerró porque la cuenta fue dada de baja, bloqueada o cambió de estado. Comunícate con el administrador de tu empresa.");

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
                    // Se renueva el identificador de la sesión: se descarta la anterior y la sesión nueva se arma en el pedido siguiente.
                    string ticket = InicioDeSesion_GUI.Preparar(resultado.Usuario, chkRecordarme.Checked, resultado.RequiereRevisarIntegridad, Request.UserHostAddress);

                    SesionActual_GUI.DescartarSesionActual();

                    Response.Redirect("Ingresar.aspx?inicio=" + Uri.EscapeDataString(ticket), false);
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

        // Devuelve true si inició la sesión (y ya redirigió). Un ticket vencido, repetido o de otra dirección no hace nada: se muestra el formulario.
        private bool CompletarInicioDeSesion()
        {
            InicioDeSesion_GUI.Pendiente pendiente = InicioDeSesion_GUI.Consumir(Request.QueryString["inicio"], Request.UserHostAddress);

            if (pendiente == null) return false;

            IniciarSesion(pendiente.Usuario);
            SesionActual_GUI.FijarHuella(pendiente.Usuario);

            if (pendiente.Recordarme)
                SesionActual_GUI.RecordarEnEsteEquipo(pendiente.Usuario.IdUsuario);

            // Si la integridad de los datos está comprometida y esta cuenta puede repararla, entra directo a Dígito verificador.
            Response.Redirect(pendiente.IrAIntegridad ? "Integridad.aspx" : "Panel.aspx", false);
            Context.ApplicationInstance.CompleteRequest();

            return true;
        }

        private static string TextoDelMotivo(string motivo)
        {
            switch (motivo)
            {
                case "CREDENCIALES_INVALIDAS":
                    return "El correo o la contraseña no son correctos.";

                case "USUARIO_BLOQUEADO_INTENTOS":
                    return "La cuenta está bloqueada por contraseñas incorrectas. Usa «Olvidé mi contraseña» para recuperar el acceso.";

                case Usuario_TLL.MOTIVO_EMPRESA_BLOQUEADA:
                    return "La empresa de tu cuenta está bloqueada. Comunícate con Pattern Blue para regularizar la situación.";

                case Usuario_TLL.MOTIVO_EMPRESA_DESHABILITADA:
                    return "La empresa de tu cuenta fue dada de baja y ya no tiene acceso a Falke. Si crees que es un error, comunícate con Pattern Blue.";

                case "USUARIO_BLOQUEO_ESTRICTO":
                    return "La cuenta tiene un bloqueo estricto. Comunícate con el administrador de tu empresa: el acceso no se puede recuperar desde acá.";

                case "USUARIO_INACTIVO":
                    return "La cuenta fue dada de baja. Si crees que es un error, comunícate con el administrador de tu empresa.";

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
