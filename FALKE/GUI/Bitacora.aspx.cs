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

        protected override void AlCambiarIdioma()
        {
            if (actor == null) return;

            MostrarAlcance();
            EtiquetarListas();
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
                    ? IdiomaActual_GUI.Texto("bitacora_sin_registros_filtros")
                    : IdiomaActual_GUI.Texto("bitacora_registros_rango", desde, hasta, pagina.Total);
                litPaginado.Text = pagina.Total == 0 ? IdiomaActual_GUI.Texto("bitacora_sin_registros") : IdiomaActual_GUI.Texto("bitacora_pagina", pagina.Pagina, pagina.TotalPaginas);
                phVacio.Visible = pagina.Total == 0;
                lnkAnterior.Visible = pagina.Pagina > 1;
                lnkSiguiente.Visible = pagina.Pagina < pagina.TotalPaginas;

            }
            catch (InvalidOperationException ex)
            {
                Avisar("aviso-peligro", MensajeDe(ex));
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
                litContador.Text = IdiomaActual_GUI.Texto("bitacora_error_cargar");
            }
        }

        private void CargarListas()
        {
            ddlModulo.Items.Clear();
            ddlModulo.Items.Add(new ListItem(IdiomaActual_GUI.Texto("comun_todos"), string.Empty));
            foreach (string modulo in new BitacoraGestor_TLL().ObtenerModulos(actor))
                ddlModulo.Items.Add(new ListItem(TextoBitacora_GUI.Modulo(modulo), modulo));

            ddlAccion.Items.Clear();
            ddlAccion.Items.Add(new ListItem(IdiomaActual_GUI.Texto("comun_todas"), string.Empty));
            ddlAccion.Items.Add(new ListItem(IdiomaActual_GUI.Texto("bitacora_accion_inicio_sesion"), BitacoraGestor_TLL.ACCION_INICIO_SESION));
            ddlAccion.Items.Add(new ListItem(IdiomaActual_GUI.Texto("bitacora_accion_cierre_sesion"), BitacoraGestor_TLL.ACCION_CIERRE_SESION));
            ddlAccion.Items.Add(new ListItem(IdiomaActual_GUI.Texto("bitacora_accion_alta"), BitacoraGestor_TLL.ACCION_ALTA));
            ddlAccion.Items.Add(new ListItem(IdiomaActual_GUI.Texto("bitacora_accion_modificacion"), BitacoraGestor_TLL.ACCION_MODIFICACION));
            ddlAccion.Items.Add(new ListItem(IdiomaActual_GUI.Texto("bitacora_accion_baja"), BitacoraGestor_TLL.ACCION_BAJA));

            ddlCriticidad.Items.Clear();
            ddlCriticidad.Items.Add(new ListItem(IdiomaActual_GUI.Texto("comun_todas"), string.Empty));
            ddlCriticidad.Items.Add(new ListItem(IdiomaActual_GUI.Texto("bitacora_criticidad_alta"), ((int)CriticidadBitacora.Alta).ToString()));
            ddlCriticidad.Items.Add(new ListItem(IdiomaActual_GUI.Texto("bitacora_criticidad_media"), ((int)CriticidadBitacora.Media).ToString()));
            ddlCriticidad.Items.Add(new ListItem(IdiomaActual_GUI.Texto("bitacora_criticidad_baja"), ((int)CriticidadBitacora.Baja).ToString()));

            phFiltroEmpresa.Visible = actor.VeBitacoraCompleta();

            if (actor.VeBitacoraCompleta())
            {
                ddlEmpresa.Items.Clear();
                ddlEmpresa.Items.Add(new ListItem(IdiomaActual_GUI.Texto("comun_todas"), string.Empty));

                foreach (Empresa_BE empresa in new Empresa_BLL().ObtenerParaFiltroDeBitacora(actor))
                    ddlEmpresa.Items.Add(new ListItem(empresa.NombreEmpresa, empresa.IdEmpresa.ToString()));
            }
        }

        private void EtiquetarListas()
        {
            foreach (ListItem item in ddlModulo.Items)
                item.Text = item.Value.Length == 0 ? IdiomaActual_GUI.Texto("comun_todos") : TextoBitacora_GUI.Modulo(item.Value);

            var acciones = new Dictionary<string, string>
            {
                { BitacoraGestor_TLL.ACCION_INICIO_SESION, "bitacora_accion_inicio_sesion" },
                { BitacoraGestor_TLL.ACCION_CIERRE_SESION, "bitacora_accion_cierre_sesion" },
                { BitacoraGestor_TLL.ACCION_ALTA, "bitacora_accion_alta" },
                { BitacoraGestor_TLL.ACCION_MODIFICACION, "bitacora_accion_modificacion" },
                { BitacoraGestor_TLL.ACCION_BAJA, "bitacora_accion_baja" }
            };

            foreach (ListItem item in ddlAccion.Items)
                item.Text = acciones.ContainsKey(item.Value) ? IdiomaActual_GUI.Texto(acciones[item.Value]) : IdiomaActual_GUI.Texto("comun_todas");

            var criticidades = new Dictionary<string, string>
            {
                { ((int)CriticidadBitacora.Alta).ToString(), "bitacora_criticidad_alta" },
                { ((int)CriticidadBitacora.Media).ToString(), "bitacora_criticidad_media" },
                { ((int)CriticidadBitacora.Baja).ToString(), "bitacora_criticidad_baja" }
            };

            foreach (ListItem item in ddlCriticidad.Items)
                item.Text = criticidades.ContainsKey(item.Value) ? IdiomaActual_GUI.Texto(criticidades[item.Value]) : IdiomaActual_GUI.Texto("comun_todas");

            if (actor.VeBitacoraCompleta() && ddlEmpresa.Items.Count > 0)
                ddlEmpresa.Items[0].Text = IdiomaActual_GUI.Texto("comun_todas");
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
                litBajada.Text = IdiomaActual_GUI.Texto("bitacora_bajada_completa");
                litAlcance.Text = IdiomaActual_GUI.Texto("bitacora_alcance_completa");
                return;
            }

            litBajada.Text = IdiomaActual_GUI.Texto("bitacora_bajada_empresa");
            litAlcance.Text = IdiomaActual_GUI.Texto("bitacora_alcance_empresa");
        }

        protected static string EtiquetaCriticidad(CriticidadBitacora criticidad)
        {
            switch (criticidad)
            {
                case CriticidadBitacora.Alta: return IdiomaActual_GUI.Texto("bitacora_criticidad_alta");
                case CriticidadBitacora.Media: return IdiomaActual_GUI.Texto("bitacora_criticidad_media");
                default: return IdiomaActual_GUI.Texto("bitacora_criticidad_baja");
            }
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
