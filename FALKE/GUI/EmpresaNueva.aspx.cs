using System;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class EmpresaNueva : PaginaBase_GUI
    {

        private ActorUsuario_TE actor;

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
                Avisar(IdiomaActual_GUI.Texto("empresa_nueva_plan_invalido"));
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
                IdIdioma = IdiomaActual_GUI.IdIdiomaBase()
            };

            if (admin.NombreUsuario.Length == 0 || admin.ApellidoUsuario.Length == 0 || admin.EmailUsuario.Length == 0)
            {
                Avisar(IdiomaActual_GUI.Texto("empresa_nueva_admin_obligatorio"));
                return;
            }

            string token;

            try
            {
                token = new Empresa_BLL().RegistrarEmpresa(actor, empresa, admin);
            }
            catch (UnauthorizedAccessException ex)
            {
                Avisar(MensajeDe(ex));
                return;
            }
            catch (InvalidOperationException ex)
            {
                Avisar(MensajeDe(ex));
                return;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("EmpresaNueva.Registrar", ex);
                Avisar(IdiomaActual_GUI.Texto("empresa_nueva_error"));
                return;
            }

            string resultado = IdiomaActual_GUI.Texto("empresa_nueva_ok", empresa.NombreEmpresa, admin.EmailUsuario);

            try
            {
                CorreosCuenta_GUI.EnviarActivacion(admin.EmailUsuario, admin.NombreUsuario, token,
                    IdiomaActual_GUI.Texto("empresa_nueva_mail_intro", empresa.NombreEmpresa));

                resultado += IdiomaActual_GUI.Texto("empresa_nueva_ok_enlace");
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("EmpresaNueva.Correo", ex);
                resultado += IdiomaActual_GUI.Texto("empresa_nueva_ok_correo_falla");
            }

            Session["AvisoEmpresas"] = resultado;

            Response.Redirect("Empresas.aspx?id=" + empresa.IdEmpresa, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void Avisar(string texto)
        {
            PublicarAviso(pnlAviso, litAviso, null, texto);
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
