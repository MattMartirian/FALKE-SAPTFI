using System;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Usuarios : System.Web.UI.Page
    {
        private const string ROL_ANALISTA = "Analista";
        private const string PATENTE_ALTA = "REGISTRAR_USUARIO";
        private const string PATENTE_OTRA_EMPRESA = "CREAR_USUARIO_OTRA_EMPRESA";
        private const int ID_IDIOMA_ESPANOL = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            AplicarPermisos();
        }

        private void AplicarPermisos()
        {
            bool puedeAlta = SesionActual_GUI.HayUsuario
                ? SesionActual_GUI.Puede(PATENTE_ALTA)
                : Master.RolActual != ROL_ANALISTA;

            phInvitar.Visible = puedeAlta;
            phSoloLectura.Visible = !puedeAlta;
            phInvEmpresa.Visible = SesionActual_GUI.Puede(PATENTE_OTRA_EMPRESA);

            if (puedeAlta)
                litBajada.Text = "Gestioná las cuentas de tu empresa y el estado de cada una.";
            else
                litBajada.Text = "Integrantes de tu empresa y sus roles.";
        }

        protected void btnInvitar_Click(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.HayUsuario)
            {
                Response.Redirect("Ingresar.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!SesionActual_GUI.Puede(PATENTE_ALTA))
            {
                Avisar("aviso-peligro", "Tu cuenta no tiene permiso para dar de alta usuarios.");
                return;
            }

            string nombre = invNombre.Text.Trim();
            string apellido = invApellido.Text.Trim();
            string email = invEmail.Text.Trim();
            string rol = invRol.SelectedValue;

            if (nombre.Length == 0 || apellido.Length == 0 || email.Length == 0)
            {
                Avisar("aviso-peligro", "Nombre, apellido y correo son obligatorios.");
                return;
            }

            int idEmpresa;
            if (!ResolverEmpresa(out idEmpresa))
            {
                Avisar("aviso-peligro", "Indicá un id de empresa válido.");
                return;
            }

            try
            {
                var usuario = new Usuario_TE
                {
                    IdEmpresa = idEmpresa,
                    NombreUsuario = nombre,
                    ApellidoUsuario = apellido,
                    EmailUsuario = email,
                    Rol = new PermisoCompuesto_TE(rol, true),
                    IdIdioma = ID_IDIOMA_ESPANOL,
                    EsCuentaEmergencia = false
                };

                string token = new Usuario_TLL().RegistrarUsuario(usuario);

                string enlace = WebHelper.UrlAbsoluta("DefinirClave.aspx?token=" + Uri.EscapeDataString(token));

                string cuerpo =
                    "Hola " + nombre + "," + Environment.NewLine + Environment.NewLine +
                    "Se creó tu cuenta en Falke." + Environment.NewLine +
                    "Para activarla y definir tu contraseña, entrá a este enlace (vence en 48 horas):" + Environment.NewLine +
                    enlace + Environment.NewLine;

                Email_SERVICE.Enviar(email, "Activá tu cuenta de Falke", cuerpo);

                LimpiarFormulario();
                Avisar("aviso-exito", "Se creó la cuenta de " + email + " en estado «Pendiente de activación». El enlace de activación quedó en App_Data/mails/.");
            }
            catch (InvalidOperationException ex)
            {
                Avisar("aviso-peligro", ex.Message);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Usuarios.Invitar", ex);
                Avisar("aviso-peligro", "No se pudo registrar el usuario. Volvé a intentarlo.");
            }
        }

        private bool ResolverEmpresa(out int idEmpresa)
        {
            if (SesionActual_GUI.Puede(PATENTE_OTRA_EMPRESA))
            {
                string texto = invEmpresa.Text.Trim();

                if (texto.Length == 0)
                {
                    idEmpresa = SesionActual_GUI.IdEmpresa;
                    return idEmpresa > 0;
                }

                return int.TryParse(texto, out idEmpresa) && idEmpresa > 0;
            }

            idEmpresa = SesionActual_GUI.IdEmpresa;
            return idEmpresa > 0;
        }

        private void LimpiarFormulario()
        {
            invNombre.Text = string.Empty;
            invApellido.Text = string.Empty;
            invEmail.Text = string.Empty;
            invEmpresa.Text = string.Empty;
            invRol.SelectedIndex = 0;
        }

        private void Avisar(string variante, string texto)
        {
            pnlInvitarAviso.CssClass = "aviso " + variante;
            litInvitarAviso.Text = Server.HtmlEncode(texto);
            pnlInvitarAviso.Visible = true;

            ClientScript.RegisterStartupScript(GetType(), "reabrirInvitar",
                "window.addEventListener('load',function(){try{window.Falke.abrirModal('modalInvitar');}catch(e){}});", true);
        }
    }
}
