using System;
using System.Globalization;
using BE;
using BLL;
using SERVICES;
using TLL;
using TE;

namespace GUI
{
    public partial class MiEmpresa : System.Web.UI.Page
    {
        private static readonly CultureInfo Cultura = new CultureInfo("es-AR");

        private ActorUsuario_TE actor;
        private bool edicionEnCurso;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(Patentes_TLL.VER_DATOS_EMPRESA)) return;

            actor = SesionActual_GUI.ObtenerActor();

            // Quien ve todas las empresas (el Gestor, la cuenta de emergencia) tiene su pantalla de Empresas: "Mi empresa" es para el administrador de un cliente.
            if (actor.VeTodasLasEmpresas())
            {
                Response.Redirect("Empresas.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                actor = null;
                return;
            }

            if (actor.EsEmergencia || actor.IdEmpresa <= 0)
            {
                actor = null;
                MostrarError("Esta cuenta no pertenece a ninguna empresa cliente, así que no hay datos de empresa para mostrar.");
                return;
            }

            bool puedeEditar = actor.Puede(Patentes_TLL.MODIFICAR_CONTACTO_EMPRESA);
            btnEditarContacto.Visible = puedeEditar;
            phContactoEditable.Visible = puedeEditar;
            phSoloPatternBlue.Visible = !puedeEditar;
        }

        // Los datos se muestran al final del pedido para que reflejen lo que se acaba de guardar.
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                Empresa_BE empresa = new Empresa_BLL().ObtenerMiEmpresa(actor);

                if (empresa == null)
                {
                    MostrarError("No se encontró la empresa de tu cuenta.");
                    return;
                }

                string nombre = empresa.NombreEmpresa ?? string.Empty;

                litSigla.Text = Server.HtmlEncode(Sigla(nombre));
                litNombre.Text = Server.HtmlEncode(nombre);
                litAlta.Text = Server.HtmlEncode(Fecha(empresa.FechaAlta));
                litPlanFicha.Text = "Plan " + empresa.PlanSuscripcion;
                litEstadoFicha.Text = empresa.Estado.ToString();
                badgeEstado.Attributes["class"] = "badge " + (empresa.Estado == EstadoEmpresa.Activa ? "badge-exito" : "badge-peligro");

                litUsuarios.Text = empresa.CantidadUsuarios.ToString();
                litDispositivos.Text = empresa.DispositivosPrestados.ToString();
                litSesiones.Text = empresa.SesionesGrabadas.ToString();

                litRazon.Text = Server.HtmlEncode(nombre);
                litCuit.Text = Server.HtmlEncode(Dato(empresa.Cuit));
                litRubro.Text = Server.HtmlEncode(Dato(empresa.Rubro));
                litDomicilio.Text = Server.HtmlEncode(Dato(empresa.Domicilio));
                litTelefono.Text = Server.HtmlEncode(Dato(empresa.NumContactoEmpresa));

                litPlan.Text = empresa.PlanSuscripcion.ToString();
                litFacturacion.Text = empresa.Facturacion.HasValue ? empresa.Facturacion.Value.ToString() : "—";
                litRenovacion.Text = empresa.FechaRenovacion.HasValue ? Fecha(empresa.FechaRenovacion.Value) : "—";
                litEstado.Text = empresa.Estado.ToString();

                var dispositivos = new Dispositivo_BLL().ObtenerDeMiEmpresa(actor);
                rptDispositivos.DataSource = dispositivos;
                rptDispositivos.DataBind();
                phSinDispositivos.Visible = dispositivos.Count == 0;

                // Si el guardado falló se conserva lo que la persona escribió; si no, el formulario arranca con los datos actuales.
                if (!edicionEnCurso)
                {
                    edRubro.Text = empresa.Rubro;
                    edDomicilio.Text = empresa.Domicilio;
                    edTelefono.Text = empresa.NumContactoEmpresa;
                }
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("MiEmpresa", ex);
                MostrarError("No se pudieron cargar los datos de la empresa.");
            }
        }

        protected void btnGuardarContacto_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                new Empresa_BLL().ModificarContacto(actor, edRubro.Text, edDomicilio.Text, edTelefono.Text);

                Avisar("aviso-exito", "Los datos de contacto se actualizaron.");
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                Avisar("aviso-peligro", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Avisar("aviso-peligro", ex.Message);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("MiEmpresa.Contacto", ex);
                Avisar("aviso-peligro", "No se pudo guardar. Volvé a intentarlo.");
            }

            edicionEnCurso = true;
            ClientScript.RegisterStartupScript(GetType(), "abrirContacto",
                "window.addEventListener('load',function(){try{window.Falke.abrirModal('modalContacto');}catch(e){}});", true);
        }

        private void Avisar(string variante, string texto)
        {
            pnlAviso.CssClass = "aviso " + variante;
            litAviso.Text = Server.HtmlEncode(texto);
            pnlAviso.Visible = true;
        }

        private void MostrarError(string texto)
        {
            phDatos.Visible = false;
            litError.Text = Server.HtmlEncode(texto);
            pnlError.Visible = true;
        }

        protected static string FechaDispositivo(DateTime? fecha)
        {
            return fecha.HasValue ? Fecha(fecha.Value) : "—";
        }

        private static string Dato(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "—" : valor;
        }

        private static string Fecha(DateTime fecha)
        {
            return fecha.ToString("d MMM yyyy", Cultura);
        }

        private static string Sigla(string nombre)
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
