using System;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class RegistrarEmpresa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso("REGISTRAR_EMPRESA")) return;

            if (!IsPostBack) CargarEmpresas();
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso("REGISTRAR_EMPRESA")) return;

            int idIdioma;
            if (!int.TryParse(txtAdminIdioma.Text.Trim(), out idIdioma))
            {
                lblMsg.Text = "El id de idioma debe ser numerico.";
                return;
            }

            if (txtNombreEmpresa.Text.Trim().Length == 0 ||
                txtAdminNombre.Text.Trim().Length == 0 ||
                txtAdminApellido.Text.Trim().Length == 0 ||
                txtAdminEmail.Text.Trim().Length == 0)
            {
                lblMsg.Text = "El nombre de la empresa y el nombre, apellido y email del administrador son obligatorios.";
                return;
            }

            PlanSuscripcion plan;
            if (!Enum.TryParse(txtPlan.Text.Trim(), true, out plan) || !Enum.IsDefined(typeof(PlanSuscripcion), plan))
            {
                lblMsg.Text = "El plan debe ser uno de: Scout, Hunter, Apex.";
                return;
            }

            try
            {
                var empresa = new Empresa_BE
                {
                    NombreEmpresa = txtNombreEmpresa.Text.Trim(),
                    NumContactoEmpresa = txtContacto.Text.Trim(),
                    PlanSuscripcion = plan,
                    FechaAlta = DateTime.Now
                };

                var admin = new Usuario_TE
                {
                    NombreUsuario = txtAdminNombre.Text.Trim(),
                    ApellidoUsuario = txtAdminApellido.Text.Trim(),
                    EmailUsuario = txtAdminEmail.Text.Trim(),
                    Rol = new PermisoCompuesto_TE(Usuario_TLL.ROL_ADMINISTRADOR, true),
                    IdIdioma = idIdioma,
                    EsCuentaEmergencia = false
                };

                string token = new Empresa_BLL().RegistrarEmpresa(empresa, admin);

                string link = WebHelper.UrlAbsoluta("EstablecerContrasena.aspx?token=" + Uri.EscapeDataString(token));
                string cuerpo =
                    "Hola " + admin.NombreUsuario + "," + Environment.NewLine + Environment.NewLine +
                    "Se creo la empresa \"" + empresa.NombreEmpresa + "\" en FALKE y sos su administrador." + Environment.NewLine +
                    "Para activar tu cuenta y definir tu contrasena, entra a este enlace (vence en 48 horas):" + Environment.NewLine +
                    link + Environment.NewLine;

                Email_SERVICE.Enviar(admin.EmailUsuario, "Activa tu cuenta de administrador en FALKE", cuerpo);

                lblMsg.Text = "Empresa creada (id " + empresa.IdEmpresa + "). Se registro el administrador " +
                              admin.EmailUsuario + " (estado Pendiente) y se envio el mail de activacion.";
                CargarEmpresas();
            }
            catch (InvalidOperationException ex)
            {
                // Regla de negocio (nombre de empresa o email duplicado, email invalido): mensaje util.
                lblMsg.Text = ex.Message;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("RegistrarEmpresa", ex);
                lblMsg.Text = "No se pudo crear la empresa. Intente nuevamente.";
            }
        }

        private void CargarEmpresas()
        {
            gvEmpresas.DataSource = new Empresa_BLL().ObtenerTodas();
            gvEmpresas.DataBind();
        }
    }
}
