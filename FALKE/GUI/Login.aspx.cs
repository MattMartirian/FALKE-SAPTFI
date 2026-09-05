using System;
using TLL;

namespace GUI
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                var resultado = new UsuarioTLL().ValidarCredenciales(txtEmail.Text.Trim(), txtPass.Text);

                if (resultado.Exito)
                {
                    string nombre = (resultado.Usuario.NombreUsuario + " " + resultado.Usuario.ApellidoUsuario).Trim();
                    SesionActual.Iniciar(resultado.Usuario.EmailUsuario, nombre);
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
                ErrorLog.Registrar("Login", ex);
                lblMsg.Text = "Ocurrio un error al procesar la solicitud. Intente nuevamente.";
            }
        }
    }
}
