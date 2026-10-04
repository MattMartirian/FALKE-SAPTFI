using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BE;
using BLL;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class Empresas : System.Web.UI.Page
    {
        private static readonly CultureInfo Cultura = new CultureInfo("es-AR");

        private ActorUsuario_TLL actor;

        protected bool PuedeEditar { get; private set; }
        protected bool PuedeCambiarEstado { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(Patentes_TLL.VER_USUARIOS_TODAS_EMPRESAS)) return;

            actor = SesionActual_GUI.ObtenerActor();

            PuedeEditar = actor.Puede(Patentes_TLL.MODIFICAR_EMPRESA);
            PuedeCambiarEstado = actor.Puede(Patentes_TLL.CAMBIAR_ESTADO_EMPRESA);

            if (IsPostBack) return;

            string aviso = Session["AvisoEmpresas"] as string;

            if (!string.IsNullOrEmpty(aviso))
            {
                Session.Remove("AvisoEmpresas");
                Avisar("aviso-exito", aviso);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                List<Empresa_BE> empresas = new Empresa_BLL().ObtenerCartera(actor);

                // Las dadas de baja ya no son clientes: no cuentan en los totales y el listado las oculta salvo con su filtro.
                var clientes = empresas.Where(x => x.Estado != EstadoEmpresa.Deshabilitada).ToList();

                litTotal.Text = clientes.Count.ToString();
                litActivas.Text = clientes.Count(x => x.Estado == EstadoEmpresa.Activa).ToString();
                litDispositivos.Text = clientes.Sum(x => x.DispositivosPrestados).ToString();
                litUsuarios.Text = clientes.Sum(x => x.CantidadUsuarios).ToString();
                litCuenta.Text = clientes.Count.ToString();
                litBajas.Text = empresas.Count - clientes.Count > 0 ? " (" + (empresas.Count - clientes.Count) + ")" : string.Empty;

                rptEmpresas.DataSource = empresas;
                rptEmpresas.DataBind();
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Empresas.Listar", ex);
                Avisar("aviso-peligro", "No se pudo cargar el listado de empresas.");
            }
        }

        protected void btnGuardarEmpresa_Click(object sender, EventArgs e)
        {
            int idEmpresa;
            PlanSuscripcion plan;
            CicloFacturacion facturacion;

            if (actor == null || !int.TryParse(hfEdId.Value, out idEmpresa) ||
                !Enum.TryParse(edPlan.SelectedValue, out plan) || !Enum.TryParse(edFactura.SelectedValue, out facturacion))
            {
                Avisar("aviso-peligro", "No se pudo identificar la empresa. Volvé a intentarlo.");
                return;
            }

            var nuevos = new Empresa_BE
            {
                NombreEmpresa = edRazon.Text,
                Cuit = edCuit.Text,
                Rubro = edRubro.Text,
                Domicilio = edDomicilio.Text,
                NumContactoEmpresa = edContacto.Text,
                PlanSuscripcion = plan,
                Facturacion = facturacion
            };

            Ejecutar("Empresas.Modificar", idEmpresa, "Los datos de la empresa se actualizaron.",
                () => new Empresa_BLL().ModificarDatos(actor, idEmpresa, nuevos, edMotivo.Text));
        }

        protected void btnCambiarEstadoEmpresa_Click(object sender, EventArgs e)
        {
            int idEmpresa;
            EstadoEmpresa estado;

            if (actor == null || !int.TryParse(hfEstId.Value, out idEmpresa) || !Enum.TryParse(edEstado.SelectedValue, out estado))
            {
                Avisar("aviso-peligro", "No se pudo identificar la empresa. Volvé a intentarlo.");
                return;
            }

            Ejecutar("Empresas.Estado", idEmpresa, "El estado de la empresa se actualizó.",
                () => new Empresa_BLL().CambiarEstado(actor, idEmpresa, estado, edMotivoEstado.Text));
        }

        private void Ejecutar(string origen, int idEmpresa, string exito, Action accion)
        {
            hfSeleccion.Value = idEmpresa.ToString();

            try
            {
                accion();
                Avisar("aviso-exito", exito);
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
                LogErrores_SERVICE.Registrar(origen, ex);
                Avisar("aviso-peligro", "No se pudo completar la operación. Volvé a intentarlo.");
            }
        }

        private void Avisar(string variante, string texto)
        {
            pnlAviso.CssClass = "aviso " + variante;
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

        protected static string Dato(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "—" : valor;
        }

        protected static string Fecha(DateTime? fecha)
        {
            return fecha.HasValue ? fecha.Value.ToString("d MMM yyyy", Cultura) : "—";
        }

        protected static string Factura(object item)
        {
            var empresa = (Empresa_BE)item;

            return empresa.Facturacion.HasValue ? empresa.Facturacion.Value.ToString() : "—";
        }

        protected static string ClasePlan(PlanSuscripcion plan)
        {
            switch (plan)
            {
                case PlanSuscripcion.Hunter: return "badge-alerta";
                case PlanSuscripcion.Apex: return "badge-info";
                default: return "badge-neutro";
            }
        }

        protected static string ClaseEstado(EstadoEmpresa estado)
        {
            switch (estado)
            {
                case EstadoEmpresa.Activa: return "badge-exito";
                case EstadoEmpresa.Bloqueada: return "badge-peligro";
                default: return "badge-neutro";
            }
        }
    }
}
