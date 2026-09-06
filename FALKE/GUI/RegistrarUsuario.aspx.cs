using System;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class RegistrarUsuario : System.Web.UI.Page
    {
        // Patente para crear usuarios en empresas distintas a la propia (rol Gestor de Pattern Blue).
        private const string PATENTE_OTRA_EMPRESA = "CREAR_USUARIO_OTRA_EMPRESA";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Alta de usuarios: requiere la patente REGISTRAR_USUARIO. Guard efectivo en el handler.
            if (!SesionActual_GUI.ExigirPermiso("REGISTRAR_USUARIO")) return;

            if (IsPostBack) return;

            // El administrador de una empresa solo puede dar de alta usuarios de SU empresa.
            // El Gestor puede elegir la empresa.
            if (!SesionActual_GUI.Puede(PATENTE_OTRA_EMPRESA))
            {
                txtEmpresa.Text = SesionActual_GUI.IdEmpresa.ToString();
                txtEmpresa.Enabled = false;
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso("REGISTRAR_USUARIO")) return;

            int idEmpresa;
            int idIdioma;
            if (!int.TryParse(txtEmpresa.Text.Trim(), out idEmpresa) ||
                !int.TryParse(txtIdioma.Text.Trim(), out idIdioma))
            {
                lblMsg.Text = "El id de empresa y el id de idioma deben ser numericos.";
                return;
            }

            // Salvo que tenga la patente de otra empresa, la empresa se fuerza a la del actor.
            if (!SesionActual_GUI.Puede(PATENTE_OTRA_EMPRESA)) idEmpresa = SesionActual_GUI.IdEmpresa;

            if (txtNombre.Text.Trim().Length == 0 ||
                txtApellido.Text.Trim().Length == 0 ||
                txtEmail.Text.Trim().Length == 0 ||
                txtRol.Text.Trim().Length == 0)
            {
                lblMsg.Text = "Nombre, apellido, email y rol son obligatorios.";
                return;
            }

            try
            {
                var usuario = new Usuario_TE
                {
                    IdEmpresa = idEmpresa,
                    NombreUsuario = txtNombre.Text.Trim(),
                    ApellidoUsuario = txtApellido.Text.Trim(),
                    EmailUsuario = txtEmail.Text.Trim(),
                    Rol = new PermisoCompuesto_TE(txtRol.Text.Trim(), true),
                    IdIdioma = idIdioma,
                    EsCuentaEmergencia = false
                };

                string token = new Usuario_TLL().RegistrarUsuario(usuario);

                string link = WebHelper.UrlAbsoluta("EstablecerContrasena.aspx?token=" + Uri.EscapeDataString(token));
                string cuerpo =
                    "Hola " + usuario.NombreUsuario + "," + Environment.NewLine + Environment.NewLine +
                    "Un administrador creo tu cuenta en FALKE." + Environment.NewLine +
                    "Para activarla y definir tu contrasena, entra a este enlace (vence en 48 horas):" + Environment.NewLine +
                    link + Environment.NewLine;

                Email_SERVICE.Enviar(usuario.EmailUsuario, "Activa tu cuenta de FALKE", cuerpo);

                lblMsg.Text = "Usuario registrado (id " + usuario.IdUsuario + ", empresa " + usuario.IdEmpresa +
                              ", estado Pendiente). Se envio el mail de activacion.";
            }
            catch (InvalidOperationException ex)
            {
                // Regla de negocio (p. ej. email duplicado o formato invalido): mensaje util, no sensible.
                lblMsg.Text = ex.Message;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("RegistrarUsuario", ex);
                lblMsg.Text = "No se pudo registrar el usuario. Intente nuevamente.";
            }
        }
    }
}
