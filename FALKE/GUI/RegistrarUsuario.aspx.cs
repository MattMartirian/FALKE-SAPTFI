using System;
using TE;
using TLL;

namespace GUI
{
    public partial class RegistrarUsuario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirige si no hay sesion (UX). El control real esta en el handler.
            if (!SesionActual.Exigir()) return;
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            // Alta de usuarios: accion de administrador. Guard efectivo del handler.
            if (!SesionActual.Exigir()) return;

            int idEmpresa;
            int idIdioma;
            if (!int.TryParse(txtEmpresa.Text.Trim(), out idEmpresa) ||
                !int.TryParse(txtIdioma.Text.Trim(), out idIdioma))
            {
                lblMsg.Text = "El id de empresa y el id de idioma deben ser numericos.";
                return;
            }

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

                string token = new UsuarioTLL().RegistrarUsuario(usuario);

                string link = WebHelper.UrlAbsoluta("EstablecerContrasena.aspx?token=" + Uri.EscapeDataString(token));
                string cuerpo =
                    "Hola " + usuario.NombreUsuario + "," + Environment.NewLine + Environment.NewLine +
                    "Un administrador creo tu cuenta en FALKE." + Environment.NewLine +
                    "Para activarla y definir tu contrasena, entra a este enlace (vence en 48 horas):" + Environment.NewLine +
                    link + Environment.NewLine;

                MockMailer.Enviar(usuario.EmailUsuario, "Activa tu cuenta de FALKE", cuerpo);

                lblMsg.Text = "Usuario registrado (id " + usuario.IdUsuario + ", estado Pendiente). " +
                              "Se envio el mail de activacion.";
            }
            catch (InvalidOperationException ex)
            {
                // Regla de negocio (p. ej. email duplicado): mensaje util, no sensible.
                lblMsg.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar("RegistrarUsuario", ex);
                lblMsg.Text = "No se pudo registrar el usuario. Intente nuevamente.";
            }
        }
    }
}
