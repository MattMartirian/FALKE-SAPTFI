using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Bitacora : System.Web.UI.Page
    {
        private const int TAMANO_PAGINA = 50;

        private ActorUsuario_TLL actor;

        private int PaginaActual
        {
            get { return ViewState["pagina"] == null ? 1 : (int)ViewState["pagina"]; }
            set { ViewState["pagina"] = value; }
        }

        // Las listas se arman en Init: así reciben lo que se eligió antes de que corra el evento del botón.
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

                lnkImprimir.NavigateUrl = "BitacoraImprimir.aspx?" + FiltroBitacora_GUI.AQuery(filtro);
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
            ddlAccion.Items.Add(new ListItem("Exportación", BitacoraGestor_TLL.ACCION_EXPORTACION));

            ddlCriticidad.Items.Clear();
            ddlCriticidad.Items.Add(new ListItem("Todas", string.Empty));
            ddlCriticidad.Items.Add(new ListItem("Alta", ((int)CriticidadBitacora.Alta).ToString()));
            ddlCriticidad.Items.Add(new ListItem("Media", ((int)CriticidadBitacora.Media).ToString()));
            ddlCriticidad.Items.Add(new ListItem("Baja", ((int)CriticidadBitacora.Baja).ToString()));

            phFiltroEmpresa.Visible = actor.VeBitacoraCompleta;

            if (actor.VeBitacoraCompleta)
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
                actor.VeBitacoraCompleta ? ddlEmpresa.SelectedValue : null, ddlCriticidad.SelectedValue);
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
            if (actor.VeBitacoraCompleta) ddlEmpresa.SelectedIndex = 0;

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

        protected void btnExportarCsv_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            List<BitacoraVista_TE> registros;

            try
            {
                registros = new BitacoraGestor_TLL().Exportar(actor, ArmarFiltro(), "CSV");
            }
            catch (InvalidOperationException ex)
            {
                Avisar("aviso-peligro", ex.Message);
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                Avisar("aviso-peligro", ex.Message);
                return;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Bitacora.Exportar", ex);
                Avisar("aviso-peligro", "No se pudo exportar la bitácora. Volvé a intentarlo.");
                return;
            }

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.ContentEncoding = new UTF8Encoding(true);
            Response.Charset = "utf-8";
            Response.AddHeader("Content-Disposition", "attachment; filename=bitacora_" + DateTime.Now.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture) + ".csv");
            Response.BinaryWrite(new UTF8Encoding(true).GetPreamble());
            Response.Write(ArmarCsv(registros));
            Response.End();
        }

        // Separador ";" (lo que espera Excel en configuración regional argentina). Las celdas que arrancan con
        // =, +, - o @ se prefijan para que una planilla no las interprete como fórmula.
        private static string ArmarCsv(List<BitacoraVista_TE> registros)
        {
            var sb = new StringBuilder();
            sb.Append("Fecha y hora;Usuario;Empresa;Módulo;Descripción;Criticidad\r\n");

            foreach (var r in registros)
            {
                sb.Append(Celda(r.FechaHoraBitacora.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture))).Append(';')
                  .Append(Celda(r.Actor)).Append(';')
                  .Append(Celda(r.NombreEmpresa)).Append(';')
                  .Append(Celda(r.ModuloBitacora)).Append(';')
                  .Append(Celda(r.DescripcionBitacora)).Append(';')
                  .Append(Celda(r.CriticidadBitacora.ToString())).Append("\r\n");
            }

            return sb.ToString();
        }

        private static string Celda(string valor)
        {
            valor = (valor ?? string.Empty).Replace("\r", " ").Replace("\n", " ");

            if (valor.Length > 0 && "=+-@\t".IndexOf(valor[0]) >= 0) valor = "'" + valor;

            return "\"" + valor.Replace("\"", "\"\"") + "\"";
        }

        private void Avisar(string variante, string texto)
        {
            pnlAviso.CssClass = "aviso " + variante;
            litAviso.Text = Server.HtmlEncode(texto);
            pnlAviso.Visible = true;
        }

        private void MostrarAlcance()
        {
            if (actor.VeBitacoraCompleta)
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
