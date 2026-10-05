using System;
using System.Linq;
using System.Web.UI.WebControls;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Roles : System.Web.UI.Page
    {
        private const string CLAVE_AVISO = "roles.aviso";

        private ActorUsuario_TLL actor;
        private CatalogoPermisos_TE catalogo;

        private string Seleccionado
        {
            get { return (Request.QueryString["e"] ?? string.Empty).Trim(); }
        }

        // Las listas con casillas se arman en Init para que reciban lo que se tildó antes de que corra el botón.
        protected void Page_Init(object sender, EventArgs e)
        {
            actor = SesionActual_GUI.ObtenerActor();

            if (actor == null) return;

            if (!actor.Puede(Patentes_TLL.GESTIONAR_ROLES))
            {
                Response.Redirect("SinPermiso.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                actor = null;
                return;
            }

            try
            {
                catalogo = new Permiso_TLL().ObtenerCatalogo(actor);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Roles.Catalogo", ex);
                actor = null;
                Response.Redirect("SinPermiso.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            foreach (var grupo in catalogo.Grupos)
            {
                if (grupo.Nombre != Seleccionado) cblGrupos.Items.Add(new ListItem(grupo.Etiqueta, grupo.Nombre));
            }

            foreach (var patente in catalogo.Patentes)
                cblPatentes.Items.Add(new ListItem(patente.Etiqueta, patente.Nombre));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (actor == null)
            {
                SesionActual_GUI.Exigir();
                return;
            }

            if (!IsPostBack)
            {
                string aviso = Session[CLAVE_AVISO] as string;
                Session.Remove(CLAVE_AVISO);

                if (!string.IsNullOrEmpty(aviso)) Avisar("aviso-exito", aviso);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                catalogo = new Permiso_TLL().ObtenerCatalogo(actor);

                rptRoles.DataSource = catalogo.Roles;
                rptRoles.DataBind();

                rptGrupos.DataSource = catalogo.Grupos;
                rptGrupos.DataBind();
                phSinGrupos.Visible = catalogo.Grupos.Count == 0;

                rptPatentes.DataSource = catalogo.Patentes;
                rptPatentes.DataBind();

                MostrarDetalle();
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Roles.Mostrar", ex);
                Avisar("aviso-peligro", "No se pudo cargar la lista de roles y permisos.");
            }
        }

        private PermisoVista_TE Actual()
        {
            return catalogo.Roles.Concat(catalogo.Grupos).Concat(catalogo.Patentes).FirstOrDefault(p => p.Nombre == Seleccionado);
        }

        private void MostrarDetalle()
        {
            PermisoVista_TE actual = Actual();

            phDetalle.Visible = actual != null;
            phVacio.Visible = actual == null;

            if (actual == null) return;

            bool esPatente = actual.Clase == ClasePermiso.Patente;
            bool esRol = actual.Clase == ClasePermiso.Rol;

            litNombre.Text = Server.HtmlEncode(actual.Etiqueta);
            litNombre2.Text = Server.HtmlEncode(actual.Etiqueta);
            litNombreInterno.Text = Server.HtmlEncode(actual.Nombre);
            litClase.Text = esRol ? (actual.EsDeGestion ? "Rol de gestión" : "Rol general") : "Grupo";
            badgeClase.Attributes["class"] = "badge " + (esRol ? "badge-info" : "badge-neutro");
            phEtiquetas.Visible = !esPatente;
            litMeta.Text = Server.HtmlEncode(Meta(actual));

            // Los permisos los define el sistema: no se combinan ni se eliminan, solo se les edita la descripción.
            phComposicion.Visible = !esPatente;
            btnAbrirEliminar.Visible = !esPatente && !actual.EsBase;
            pnlFijo.Visible = actual.EsFijo;
            btnGuardar.Visible = !actual.EsFijo;

            bool puedeDescribir = actor.Puede(Patentes_TLL.CAMBIAR_DESCRIPCION_PERMISO);
            txtDescripcion.Enabled = puedeDescribir;
            btnGuardarDescripcion.Visible = puedeDescribir;

            if (!esPatente)
            {
                cblGrupos.Enabled = !actual.EsFijo;
                cblPatentes.Enabled = !actual.EsFijo;
                phGrupos.Visible = cblGrupos.Items.Count > 0;

                if (!IsPostBack)
                {
                    foreach (ListItem item in cblGrupos.Items) item.Selected = actual.Incluye.Contains(item.Value);
                    foreach (ListItem item in cblPatentes.Items) item.Selected = actual.Incluye.Contains(item.Value);
                }

                var etiquetas = catalogo.Patentes.ToDictionary(p => p.Nombre, p => p.Etiqueta);

                litEfectivos.Text = actual.PatentesEfectivas.Count == 0
                    ? "<li>Todavía no da ningún permiso.</li>"
                    : string.Concat(actual.PatentesEfectivas.Select(p => "<li>" + Server.HtmlEncode(Permiso_TLL.Etiqueta(etiquetas, p)) + "</li>"));
            }

            if (!IsPostBack || !editandoDescripcion) txtDescripcion.Text = actual.Descripcion ?? string.Empty;

            var etiquetasTodas = catalogo.Roles.Concat(catalogo.Grupos).ToDictionary(p => p.Nombre, p => p.Etiqueta);

            phIncluidoEn.Visible = actual.IncluidoEn.Count > 0;
            litIncluidoEn.Text = string.Concat(actual.IncluidoEn.Select(n => "<li>" + Server.HtmlEncode(Permiso_TLL.Etiqueta(etiquetasTodas, n)) + "</li>"));
        }

        private bool editandoDescripcion;

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            var marcados = cblGrupos.Items.Cast<ListItem>().Concat(cblPatentes.Items.Cast<ListItem>())
                .Where(i => i.Selected).Select(i => i.Value).ToList();

            Ejecutar("Roles.Guardar", "Los cambios se guardaron y quedaron en la bitácora.",
                () => new Permiso_TLL().GuardarComposicion(actor, Seleccionado, marcados), null);
        }

        protected void btnGuardarDescripcion_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            editandoDescripcion = true;

            string texto = txtDescripcion.Text;

            Ejecutar("Roles.Descripcion", "El nombre se guardó y quedó en la bitácora.",
                () => new Permiso_TLL().CambiarDescripcion(actor, Seleccionado, texto), null);

            if (pnlAviso.CssClass.Contains("aviso-exito")) editandoDescripcion = false;
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            bool esRol = hfTipoNuevo.Value != "grupo";
            bool deGestion = esRol && Request.Form["ambitoNuevo"] == "gestion";
            string nombre = txtNombreNuevo.Text.Trim();

            Ejecutar("Roles.Crear", (esRol ? (deGestion ? "Rol de gestión" : "Rol") : "Grupo") + " «" + nombre + "» creado. Ahora marcá qué incluye.",
                () =>
                {
                    var bll = new Permiso_TLL();
                    if (esRol) bll.CrearRol(actor, nombre, null, deGestion); else bll.CrearGrupo(actor, nombre);
                },
                nombre, "modalNuevo");
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            string nombre = Seleccionado;

            Ejecutar("Roles.Eliminar", "«" + nombre + "» se eliminó.",
                () => new Permiso_TLL().EliminarRolOGrupo(actor, nombre),
                string.Empty, "modalEliminar");
        }

        // destino: a qué elemento ir después de un éxito (null = quedarse donde está; vacío = volver a la lista).
        private void Ejecutar(string origen, string exito, Action accion, string destino, string modalSiFalla = null)
        {
            try
            {
                accion();
            }
            catch (PermisoInvalidoException ex)
            {
                Fallar(ex.Message, modalSiFalla);
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                Fallar(ex.Message, modalSiFalla);
                return;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar(origen, ex);
                Fallar("No se pudo completar la operación. Volvé a intentarlo.", modalSiFalla);
                return;
            }

            if (destino == null)
            {
                Avisar("aviso-exito", exito);
                return;
            }

            Session[CLAVE_AVISO] = exito;
            Response.Redirect(destino.Length == 0 ? "Roles.aspx" : Enlace(destino), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void Fallar(string mensaje, string modal)
        {
            Avisar("aviso-peligro", mensaje);

            if (modal != null)
                ClientScript.RegisterStartupScript(GetType(), "abrir" + modal,
                    "window.addEventListener('load',function(){try{window.Falke.abrirModal('" + modal + "');}catch(e){}});", true);
        }

        private void Avisar(string variante, string texto)
        {
            pnlAviso.CssClass = "aviso " + variante;
            litAviso.Text = Server.HtmlEncode(texto);
            pnlAviso.Visible = true;
        }

        protected string Enlace(string nombre)
        {
            return "Roles.aspx?e=" + Uri.EscapeDataString(nombre);
        }

        protected bool EsActual(string nombre)
        {
            return nombre == Seleccionado;
        }

        protected static string Meta(object item)
        {
            var p = (PermisoVista_TE)item;
            string resto;

            switch (p.Clase)
            {
                case ClasePermiso.Rol:
                    string usuarios = p.UsuariosAsignados == 1 ? "1 usuario" : p.UsuariosAsignados + " usuarios";
                    resto = usuarios + (p.EsDeGestion ? " · de gestión" : string.Empty) + (p.EsFijo ? " · fijo" : string.Empty);
                    break;

                case ClasePermiso.Grupo:
                    resto = p.PatentesEfectivas.Count == 1 ? "1 permiso" : p.PatentesEfectivas.Count + " permisos";
                    break;

                default:
                    resto = string.Empty;
                    break;
            }

            return resto;
        }
    }
}
