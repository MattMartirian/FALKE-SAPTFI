using System;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class EmpresaNueva : System.Web.UI.Page
    {
        private const int ID_IDIOMA_ESPANOL = 1;

        private ActorUsuario_TLL actor;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(Patentes_TLL.REGISTRAR_EMPRESA)) return;

            actor = SesionActual_GUI.ObtenerActor();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                var empresas = new Empresa_BLL().ObtenerCartera(actor);

                litCuentaEmpresas.Text = empresas.Count.ToString();
                rptEmpresas.DataSource = empresas;
                rptEmpresas.DataBind();
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("EmpresaNueva.Listar", ex);
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            CicloFacturacion facturacion;
            PlanSuscripcion plan;

            if (!Enum.TryParse(aePlan.SelectedValue, out plan) || !Enum.TryParse(aeFacturacion.SelectedValue, out facturacion))
            {
                Avisar("El plan o la facturación no son válidos.");
                return;
            }

            var empresa = new Empresa_BE
            {
                NombreEmpresa = aeRazon.Text,
                Cuit = aeCuit.Text,
                Rubro = aeRubro.Text,
                Domicilio = aeDomicilio.Text,
                NumContactoEmpresa = aeContacto.Text,
                PlanSuscripcion = plan,
                Facturacion = facturacion
            };

            var admin = new Usuario_TE
            {
                NombreUsuario = aeAdminNombre.Text.Trim(),
                ApellidoUsuario = aeAdminApellido.Text.Trim(),
                EmailUsuario = aeAdminEmail.Text.Trim(),
                IdIdioma = ID_IDIOMA_ESPANOL
            };

            if (admin.NombreUsuario.Length == 0 || admin.ApellidoUsuario.Length == 0 || admin.EmailUsuario.Length == 0)
            {
                Avisar("El nombre, el apellido y el correo del administrador son obligatorios.");
                return;
            }

            string token;

            try
            {
                token = new Empresa_BLL().RegistrarEmpresa(actor, empresa, admin);
            }
            catch (UnauthorizedAccessException ex)
            {
                Avisar(ex.Message);
                return;
            }
            catch (InvalidOperationException ex)
            {
                Avisar(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("EmpresaNueva.Registrar", ex);
                Avisar("No se pudo registrar la empresa. Volvé a intentarlo.");
                return;
            }

            string resultado = "Se registró la empresa «" + empresa.NombreEmpresa + "» y su administrador " + admin.EmailUsuario +
                               " (pendiente de activación).";

            try
            {
                CorreosCuenta_GUI.EnviarActivacion(admin.EmailUsuario, admin.NombreUsuario, token,
                    "Se creó la empresa \"" + empresa.NombreEmpresa + "\" en Falke y sos su administrador.");

                resultado += " El enlace de activación quedó en App_Data/mails/.";
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("EmpresaNueva.Correo", ex);
                resultado += " No se pudo enviar el correo de activación: desde Usuarios se puede reenviar la invitación.";
            }

            Session["AvisoEmpresas"] = resultado;

            Response.Redirect("Empresas.aspx?id=" + empresa.IdEmpresa, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void Avisar(string texto)
        {
            litAviso.Text = Server.HtmlEncode(texto);
            pnlAviso.Visible = true;
        }

        protected static string Sigla(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return "?";

            string[] partes = nombre.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string sigla = partes.Length == 1
                ? partes[0].Substring(0, Math.Min(2, partes[0].Length))
                : partes[0].Substring(0, 1) + partes[1].Substring(0, 1);

            return sigla.ToUpperInvariant();
        }
    }
}
