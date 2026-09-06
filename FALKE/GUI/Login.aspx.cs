using System;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && SesionActual_GUI.RedirigirSiAutenticado()) return;
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                var resultado = new Usuario_TLL().ValidarCredenciales(txtEmail.Text.Trim(), txtPass.Text);

                if (resultado.Exito)
                {
                    IniciarSesion(resultado.Usuario);
                    Response.Redirect("MenuPruebas.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Motivo es un codigo de negocio (CREDENCIALES_INVALIDAS, USUARIO_BLOQUEADO, ...),
                // no informacion sensible: se puede mostrar.
                lblMsg.Text = "Login fallido. Motivo: " + resultado.Motivo;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Login", ex);
                lblMsg.Text = "Ocurrio un error al procesar la solicitud. Intente nuevamente.";
            }
        }

        private static void IniciarSesion(Usuario_TE usuario)
        {
            string nombre = (usuario.NombreUsuario + " " + usuario.ApellidoUsuario).Trim();
            string rol = usuario.Rol != null ? usuario.Rol.Nombre : string.Empty;

            // El rol ya viene como arbol Composite (con hijos) desde la lectura del usuario.
            SesionActual_GUI.Iniciar(usuario.IdUsuario, usuario.EmailUsuario, nombre, rol, usuario.IdEmpresa, usuario.EsCuentaEmergencia, usuario.Rol);
        }

        protected void btn1_Click(object sender, EventArgs e)
        {
        }
    }
}
