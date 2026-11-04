using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Idiomas : PaginaConAviso_GUI
    {
        private const int TAMANO_PAGINA = 25;

        private ActorUsuario_TE actor;
        private List<Idioma_TE> idiomas;
        private int totalTraducciones;

        public class FilaTraduccion
        {
            public string Clave { get; set; }
            public string TextoBase { get; set; }
            public string Texto { get; set; }
            public bool Pendiente { get; set; }
            public string Origen { get; set; }
        }

        private int IdiomaSeleccionado
        {
            get { return ViewState["idioma"] == null ? 0 : (int)ViewState["idioma"]; }
            set { ViewState["idioma"] = value; }
        }

        private int PaginaActual
        {
            get { return ViewState["pagina"] == null ? 1 : (int)ViewState["pagina"]; }
            set { ViewState["pagina"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            actor = SesionActual_GUI.ObtenerActor();

            if (actor == null) return;

            if (!actor.Puede(Patentes_TLL.GESTIONAR_IDIOMAS))
            {
                Response.Redirect("SinPermiso.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                actor = null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (actor == null)
            {
                SesionActual_GUI.Exigir();
                return;
            }

            if (IsPostBack) return;

            int idioma;
            if (int.TryParse(Request.QueryString["idioma"], out idioma)) IdiomaSeleccionado = idioma;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                idiomas = new Idioma_TLL().Listar(actor);

                rptIdiomas.DataSource = idiomas;
                rptIdiomas.DataBind();

                MostrarResumen();
                MostrarDisponibles();
                MostrarTraducciones();
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Idiomas.Listar", ex);
                Avisar("aviso-peligro", Traducir("idiomas_no_se_pudo_cargar"));
            }
        }

        protected void rptIdiomas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (actor == null) return;

            pnlAviso.Visible = false;

            int idIdioma;
            if (!int.TryParse(Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture), out idIdioma))
            {
                Avisar("aviso-peligro", Traducir("idiomas_no_se_pudo_identificar"));
                return;
            }

            switch (e.CommandName)
            {
                case "revisar":
                    IdiomaSeleccionado = idIdioma;
                    PaginaActual = 1;
                    break;

                case "habilitar":
                    Ejecutar("Idiomas.Habilitar", Traducir("idiomas_habilitado_ok"), () => new Idioma_TLL().CambiarEstado(actor, idIdioma, true));
                    break;

                case "deshabilitar":
                    Ejecutar("Idiomas.Deshabilitar", Traducir("idiomas_deshabilitado_ok"), () => new Idioma_TLL().CambiarEstado(actor, idIdioma, false));
                    break;
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            pnlAviso.Visible = false;

            string codigo = string.IsNullOrWhiteSpace(txtCodigo.Text) ? ddlDisponibles.SelectedValue : txtCodigo.Text;
            string nombre = txtNombre.Text;
            Idioma_TE agregado = null;

            if (Ejecutar(() => agregado = new Idioma_TLL().Agregar(actor, codigo, nombre), "Idiomas.Agregar", "modalAgregar"))
            {
                txtCodigo.Text = string.Empty;
                txtNombre.Text = string.Empty;

                IdiomaSeleccionado = agregado.IdIdioma;
                PaginaActual = 1;
                Avisar("aviso-exito", Traducir("idiomas_agregado_ok", agregado.NombreIdioma, agregado.CantidadTraducciones, agregado.CantidadPendientes));
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            pnlAviso.Visible = false;

            int idIdioma;
            if (!int.TryParse(hfEliminar.Value, out idIdioma))
            {
                Avisar("aviso-peligro", Traducir("idiomas_no_se_pudo_identificar"));
                return;
            }

            if (Ejecutar(() => new Idioma_TLL().Eliminar(actor, idIdioma), "Idiomas.Eliminar", null))
            {
                if (IdiomaSeleccionado == idIdioma) IdiomaSeleccionado = 0;
                Avisar("aviso-exito", Traducir("idiomas_eliminado_ok"));
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
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

        protected void btnGuardarTraduccion_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            pnlAviso.Visible = false;

            if (Ejecutar(() => new Idioma_TLL().GuardarTraduccion(actor, IdiomaSeleccionado, hfClave.Value, txtTexto.Text), "Idiomas.GuardarTraduccion", "modalTraduccion"))
                Avisar("aviso-exito", Traducir("idiomas_traduccion_guardada"));
        }

        private bool Ejecutar(Action accion, string origen, string modal)
        {
            return Intentar(accion, origen, mensaje => Fallar(mensaje, modal));
        }

        private void Fallar(string mensaje, string modal)
        {
            Avisar("aviso-peligro", mensaje);

            if (modal != null) AbrirModal(modal);
        }

        private void AbrirModal(string modal)
        {
            ClientScript.RegisterStartupScript(GetType(), "abrir" + modal,
                "window.addEventListener('load',function(){try{window.Falke.abrirModal('" + modal + "');}catch(e){}});", true);
        }

        private void MostrarResumen()
        {
            Idioma_TE idiomaBase = idiomas.FirstOrDefault(i => i.EsBase);

            litActivos.Text = Traducir("idiomas_habilitados_de", idiomas.Count(i => i.Activo), idiomas.Count);
            litTextos.Text = (idiomaBase == null ? 0 : idiomaBase.CantidadTraducciones).ToString(CultureInfo.InvariantCulture);
        }

        private void MostrarDisponibles()
        {
            List<Idioma_TE> disponibles = new Idioma_TLL().ListarDisponibles(actor);

            ConservandoSeleccion(() =>
            {
                ddlDisponibles.Items.Clear();
                ddlDisponibles.Items.Add(new ListItem(Traducir("idiomas_desde_cero"), string.Empty));

                foreach (Idioma_TE d in disponibles)
                {
                    var item = new ListItem(d.NombreIdioma + " (" + d.CodigoIdioma + ")", d.CodigoIdioma);
                    item.Attributes["data-nombre"] = d.NombreIdioma;
                    ddlDisponibles.Items.Add(item);
                }
            }, ddlDisponibles);
        }

        private void MostrarTraducciones()
        {
            Idioma_TE seleccionado = idiomas.FirstOrDefault(i => i.IdIdioma == IdiomaSeleccionado);

            phTraducciones.Visible = seleccionado != null;

            if (seleccionado == null) return;

            litIdiomaNombre.Text = Server.HtmlEncode(seleccionado.NombreIdioma);

            List<FilaTraduccion> filas = ArmarFilas(seleccionado);

            totalTraducciones = filas.Count;

            int totalPaginas = Math.Max(1, (totalTraducciones + TAMANO_PAGINA - 1) / TAMANO_PAGINA);
            if (PaginaActual > totalPaginas) PaginaActual = totalPaginas;

            List<FilaTraduccion> pagina = filas.Skip((PaginaActual - 1) * TAMANO_PAGINA).Take(TAMANO_PAGINA).ToList();

            rptTraducciones.DataSource = pagina;
            rptTraducciones.DataBind();
            phSinTraducciones.Visible = pagina.Count == 0;

            int desde = totalTraducciones == 0 ? 0 : (PaginaActual - 1) * TAMANO_PAGINA + 1;
            int hasta = desde == 0 ? 0 : desde + pagina.Count - 1;

            litPaginado.Text = totalTraducciones == 0 ? Traducir("idiomas_sin_resultados") : Traducir("idiomas_mostrando", desde, hasta, totalTraducciones);

            lnkAnterior.Visible = PaginaActual > 1;
            lnkSiguiente.Visible = PaginaActual < totalPaginas;
        }

        private List<FilaTraduccion> ArmarFilas(Idioma_TE seleccionado)
        {
            var tlll = new Idioma_TLL();
            Idioma_TE idiomaBase = idiomas.First(i => i.EsBase);

            Dictionary<string, string> textosBase = tlll.ListarTraducciones(actor, idiomaBase.IdIdioma, false)
                .ToDictionary(t => t.CodigoClave, t => t.Texto ?? string.Empty, StringComparer.Ordinal);

            string buscado = (txtBuscar.Text ?? string.Empty).Trim();
            var filas = new List<FilaTraduccion>();

            foreach (Traduccion_TE t in tlll.ListarTraducciones(actor, seleccionado.IdIdioma, chkPendientes.Checked))
            {
                string textoBase;
                textosBase.TryGetValue(t.CodigoClave, out textoBase);

                var fila = new FilaTraduccion
                {
                    Clave = t.CodigoClave,
                    TextoBase = textoBase ?? string.Empty,
                    Texto = t.Texto ?? string.Empty,
                    Pendiente = t.Pendiente,
                    Origen = EtiquetaOrigen(t.Origen)
                };

                if (buscado.Length > 0 && !Contiene(fila, buscado)) continue;

                filas.Add(fila);
            }

            return filas;
        }

        private static bool Contiene(FilaTraduccion fila, string buscado)
        {
            return fila.Clave.IndexOf(buscado, StringComparison.OrdinalIgnoreCase) >= 0
                || fila.TextoBase.IndexOf(buscado, StringComparison.OrdinalIgnoreCase) >= 0
                || fila.Texto.IndexOf(buscado, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string EtiquetaOrigen(OrigenTraduccion origen)
        {
            switch (origen)
            {
                case OrigenTraduccion.Resx: return Traducir("idiomas_origen_paquete");
                case OrigenTraduccion.Manual: return Traducir("idiomas_origen_manual");
                case OrigenTraduccion.Auto: return Traducir("idiomas_origen_automatica");
                default: return origen.ToString();
            }
        }

        protected bool PuedeModificar(object item)
        {
            var idioma = (Idioma_TE)item;

            return !idioma.EsBase;
        }
    }
}
