using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Bitacora : PaginaConAviso_GUI
    {
        private const int TAMANO_PAGINA = 50;

        private ActorUsuario_TE actor;

        private int PaginaActual
        {
            get { return ViewState["pagina"] == null ? 1 : (int)ViewState["pagina"]; }
            set { ViewState["pagina"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            actor = SesionActual_GUI.ObtenerActor();

            if (actor == null) return;

            if (!actor.Puede(Patentes_TLL.VER_BITACORA))
            {
                Response.Redirect("SinPermiso.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                actor = null;
                return;
            }

            try
            {
                CargarListas();
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Bitacora.Listas", ex);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (actor == null)
            {
                SesionActual_GUI.Exigir();
                return;
            }

            MostrarAlcance();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                FiltroBitacora_TE filtro = ArmarFiltro();
                filtro.Pagina = PaginaActual;
                filtro.Tamano = TAMANO_PAGINA;

                PaginaBitacora_TE pagina = new BitacoraGestor_TLL().ObtenerVista(actor, filtro);

                PaginaActual = pagina.Pagina;

                rptBitacora.DataSource = pagina.Items;
                rptBitacora.DataBind();

                int desde = pagina.Total == 0 ? 0 : (pagina.Pagina - 1) * pagina.Tamano + 1;
                int hasta = desde == 0 ? 0 : desde + pagina.Items.Count - 1;

                litContador.Text = pagina.Total == 0
                    ? "No hay registros que coincidan con los filtros."
                    : "Registros " + desde + "–" + hasta + " de " + pagina.Total + ".";
                litPaginado.Text = pagina.Total == 0 ? "Sin registros" : "Página " + pagina.Pagina + " de " + pagina.TotalPaginas;
                phVacio.Visible = pagina.Total == 0;
                lnkAnterior.Visible = pagina.Pagina > 1;
                lnkSiguiente.Visible = pagina.Pagina < pagina.TotalPaginas;

            }
            catch (InvalidOperationException ex)
            {
                Avisar("aviso-peligro", ex.Message);
                rptBitacora.DataSource = null;
                rptBitacora.DataBind();
                litContador.Text = string.Empty;
                litPaginado.Text = string.Empty;
                lnkAnterior.Visible = false;
                lnkSiguiente.Visible = false;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Bitacora.Listar", ex);
                litContador.Text = "No se pudo cargar la bitácora.";
            }
        }

        private void CargarListas()
        {
            ddlModulo.Items.Clear();
            ddlModulo.Items.Add(new ListItem("Todos", string.Empty));
            foreach (string modulo in new BitacoraGestor_TLL().ObtenerModulos(actor))
                ddlModulo.Items.Add(new ListItem(modulo, modulo));

            ddlAccion.Items.Clear();
            ddlAccion.Items.Add(new ListItem("Todas", string.Empty));
            ddlAccion.Items.Add(new ListItem("Inicio de sesión", BitacoraGestor_TLL.ACCION_INICIO_SESION));
            ddlAccion.Items.Add(new ListItem("Cierre de sesión", BitacoraGestor_TLL.ACCION_CIERRE_SESION));
            ddlAccion.Items.Add(new ListItem("Alta", BitacoraGestor_TLL.ACCION_ALTA));
            ddlAccion.Items.Add(new ListItem("Modificación", BitacoraGestor_TLL.ACCION_MODIFICACION));
            ddlAccion.Items.Add(new ListItem("Baja", BitacoraGestor_TLL.ACCION_BAJA));

            ddlCriticidad.Items.Clear();
            ddlCriticidad.Items.Add(new ListItem("Todas", string.Empty));
            ddlCriticidad.Items.Add(new ListItem("Alta", ((int)CriticidadBitacora.Alta).ToString()));
            ddlCriticidad.Items.Add(new ListItem("Media", ((int)CriticidadBitacora.Media).ToString()));
            ddlCriticidad.Items.Add(new ListItem("Baja", ((int)CriticidadBitacora.Baja).ToString()));

            phFiltroEmpresa.Visible = actor.VeBitacoraCompleta();

            if (actor.VeBitacoraCompleta())
            {
                ddlEmpresa.Items.Clear();
                ddlEmpresa.Items.Add(new ListItem("Todas", string.Empty));

                foreach (Empresa_BE empresa in new Empresa_BLL().ObtenerParaFiltroDeBitacora(actor))
                    ddlEmpresa.Items.Add(new ListItem(empresa.NombreEmpresa, empresa.IdEmpresa.ToString()));
            }
        }

        private FiltroBitacora_TE ArmarFiltro()
        {
            return FiltroBitacora_GUI.Parsear(txtDesde.Text, txtHasta.Text, txtHoraDesde.Text, txtHoraHasta.Text,
                ddlModulo.SelectedValue, ddlAccion.SelectedValue, txtUsuario.Text,
                actor.VeBitacoraCompleta() ? ddlEmpresa.SelectedValue : null, ddlCriticidad.SelectedValue);
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            PaginaActual = 1;
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtDesde.Text = string.Empty;
            txtHasta.Text = string.Empty;
            txtHoraDesde.Text = string.Empty;
            txtHoraHasta.Text = string.Empty;
            txtUsuario.Text = string.Empty;
            ddlModulo.SelectedIndex = 0;
            ddlAccion.SelectedIndex = 0;
            ddlCriticidad.SelectedIndex = 0;
            if (actor.VeBitacoraCompleta()) ddlEmpresa.SelectedIndex = 0;

            PaginaActual = 1;
        }

        protected void lnkAnterior_Click(object sender, EventArgs e)
        {
            PaginaActual = Math.Max(1, PaginaActual - 1);
        }

        protected void lnkSiguiente_Click(object sender, EventArgs e)
        {
            PaginaActual = PaginaActual + 1;
        }

        private void MostrarAlcance()
        {
            if (actor.VeBitacoraCompleta())
            {
                litBajada.Text = "Todos los movimientos registrados en el sistema.";
                litAlcance.Text = "Ves la bitácora completa de todas las empresas " +
                                  "cliente y de Pattern Blue.";
                return;
            }

            litBajada.Text = "Movimientos registrados en tu empresa.";
            litAlcance.Text = "Estás viendo los movimientos que afectan a tu empresa. Lo que hace el " +
                              "equipo de Pattern Blue aparece sin nombre. Los registros no se pueden editar ni borrar.";
        }

        protected static string EtiquetaCriticidad(CriticidadBitacora criticidad)
        {
            return criticidad.ToString();
        }

        protected static string ClaseFila(CriticidadBitacora criticidad)
        {
            switch (criticidad)
            {
                case CriticidadBitacora.Alta: return "fila-critica";
                case CriticidadBitacora.Media: return "fila-media";
                default: return "fila-baja";
            }
        }

        protected static string ClaseBadge(CriticidadBitacora criticidad)
        {
            switch (criticidad)
            {
                case CriticidadBitacora.Alta: return "badge-peligro";
                case CriticidadBitacora.Media: return "badge-alerta";
                default: return "badge-neutro";
            }
        }
    }
}
