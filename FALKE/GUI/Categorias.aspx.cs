using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class Categorias : System.Web.UI.Page
    {
        private static readonly CultureInfo Cultura = new CultureInfo("es-AR");

        private ActorUsuario_TLL actor;

        protected bool PuedeGestionar { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(Patentes_TLL.OPERAR_ANALISIS)) return;

            actor = SesionActual_GUI.ObtenerActor();
            PuedeGestionar = actor.Puede(Patentes_TLL.GESTIONAR_CATEGORIAS);
            phNuevaCategoria.Visible = PuedeGestionar;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                List<Categoria_BE> categorias = new Categoria_BLL().ObtenerPorEmpresa(actor);

                rptCategorias.DataSource = categorias;
                rptCategorias.DataBind();
            }
            catch (UnauthorizedAccessException ex)
            {
                Avisar("aviso-peligro", ex.Message);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Categorias.Listar", ex);
                Avisar("aviso-peligro", "No se pudo cargar el listado de categorías.");
            }
        }

        protected void rptCategorias_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar") return;

            int idCategoria = Convert.ToInt32(e.CommandArgument);

            Ejecutar("Categorias.Eliminar", () => new Categoria_BLL().Eliminar(actor, idCategoria),
                "La categoría se eliminó.");
        }

        protected void btnGuardarCategoria_Click(object sender, EventArgs e)
        {
            int idCategoria;
            TipoActivoCategoria tipo;

            if (actor == null || !int.TryParse(hfCatId.Value, out idCategoria) || !Enum.TryParse(mcTipo.SelectedValue, out tipo))
            {
                Avisar("aviso-peligro", "No se pudo identificar la categoría. Volvé a intentarlo.");
                return;
            }

            var categoria = new Categoria_BE
            {
                NombreCategoria = mcNombre.Text,
                Tipo = tipo,
                NombreActivo = mcActivo.Text,
                FlujoEsperado = mcFlujo.Text,
                SistemaOperativoSoftware = mcSo.Text,
                VersionSoftware = mcVersionSw.Text,
                UrlAppWeb = mcUrl.Text,
                VersionAppMovil = mcVersionApp.Text,
                VersionVideojuego = mcVersionJuego.Text,
                FormatoPublicidad = mcFormato.Text,
                CanalPublicidad = mcCanal.Text
            };

            DispositivoObjetivo dispositivo;
            SistemaOperativoMovil soMovil;
            PlataformaVideojuego plataforma;

            if (Enum.TryParse(mcDispositivo.SelectedValue, out dispositivo)) categoria.DispositivoAppWeb = dispositivo;
            if (Enum.TryParse(mcSoMovil.SelectedValue, out soMovil)) categoria.SoAppMovil = soMovil;
            if (Enum.TryParse(mcPlataforma.SelectedValue, out plataforma)) categoria.Plataforma = plataforma;

            if (idCategoria == 0)
            {
                Ejecutar("Categorias.Alta", () => new Categoria_BLL().Crear(actor, categoria), "Se registró la categoría.");
            }
            else
            {
                Ejecutar("Categorias.Modificar", () => new Categoria_BLL().Modificar(actor, idCategoria, categoria), "Los datos de la categoría se actualizaron.");
            }
        }

        private void Ejecutar(string origen, Action accion, string exito)
        {
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

        protected static string Dato(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "—" : valor;
        }

        protected static string Fecha(DateTime fecha)
        {
            return fecha.ToString("d MMM yyyy", Cultura);
        }

        protected static string EtiquetaTipo(TipoActivoCategoria tipo)
        {
            switch (tipo)
            {
                case TipoActivoCategoria.AppWeb: return "App web";
                case TipoActivoCategoria.AppMovil: return "App móvil";
                default: return tipo.ToString();
            }
        }
    }
}
