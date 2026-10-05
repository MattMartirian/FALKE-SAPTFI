using System;
using System.Globalization;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Usuarios : System.Web.UI.Page
    {
        private const int ID_IDIOMA_ESPANOL = 1;
        private const int TAMANO_PAGINA = 25;

        private ActorUsuario_TLL actor;
        private PaginaUsuarios_TE pagina;

        protected bool VeTodas { get; private set; }
        protected bool PuedeGestionar { get; private set; }
        protected bool PuedeGestionarCuenta { get; private set; }
        protected bool PuedeEditarDatos { get; private set; }

        private int PaginaActual
        {
            get { return ViewState["pagina"] == null ? 1 : (int)ViewState["pagina"]; }
            set { ViewState["pagina"] = value; }
        }

        // Las listas se arman en Init: así reciben lo que el usuario eligió antes de que corra el evento del botón.
        protected void Page_Init(object sender, EventArgs e)
        {
            actor = SesionActual_GUI.ObtenerActor();

            if (actor == null) return;

            // El perfil ya no vive acá: los enlaces viejos (?perfil=1) llevan a la pantalla propia.
            if (Request.QueryString["perfil"] == "1")
            {
                Response.Redirect("MiPerfil.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                actor = null;
                return;
            }

            if (!actor.Puede(Patentes_TLL.VER_USUARIOS))
            {
                Response.Redirect("SinPermiso.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                actor = null;
                return;
            }

            VeTodas = actor.VeTodasLasEmpresas;
            bool puedeAlta = actor.Puede(Patentes_TLL.REGISTRAR_USUARIO);
            bool puedeEstado = actor.Puede(Patentes_TLL.CAMBIAR_ESTADO_USUARIO);
            bool puedeRol = actor.Puede(Patentes_TLL.CAMBIAR_ROL_USUARIO);
            PuedeGestionarCuenta = puedeEstado || puedeRol;
            PuedeEditarDatos = actor.Puede(Patentes_TLL.MODIFICAR_USUARIO);
            PuedeGestionar = PuedeGestionarCuenta || PuedeEditarDatos;

            phInvitar.Visible = puedeAlta;
            phInvEmpresa.Visible = actor.Puede(Patentes_TLL.CREAR_USUARIO_OTRA_EMPRESA);
            phFicha.Visible = !VeTodas;
            phSoloLectura.Visible = !PuedeGestionar && !puedeAlta;
            phFiltroEmpresa.Visible = VeTodas;
            phColEmpresa.Visible = VeTodas;
            phColAcciones.Visible = PuedeGestionar;
            phGestionEstado.Visible = puedeEstado;
            phGestionRol.Visible = puedeRol;
            phDatosAvanzados.Visible = actor.Puede(Patentes_TLL.CAMBIAR_EMAIL_EMPRESA_USUARIO);

            litBajada.Text = VeTodas
                ? "Usuarios de todas las empresas. Los cambios de estado y de rol quedan registrados en la bitácora."
                : PuedeGestionar
                    ? "Gestioná las cuentas de tu empresa y el estado de cada una."
                    : "Integrantes de tu empresa y sus roles.";

            CargarListasFijas();
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
                CargarFiltros();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                pagina = new Usuario_TLL().ListarUsuarios(actor, ArmarFiltro());

                if (pagina.Items.Count == 0 && pagina.Total > 0 && PaginaActual > 1)
                {
                    PaginaActual = Math.Max(1, pagina.TotalPaginas);
                    pagina = new Usuario_TLL().ListarUsuarios(actor, ArmarFiltro());
                }

                rptUsuarios.DataSource = pagina.Items;
                rptUsuarios.DataBind();

                phVacio.Visible = pagina.Items.Count == 0;
                MostrarPaginado();
                MostrarFicha();
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Usuarios.Listar", ex);
                Avisar("aviso-peligro", "No se pudo cargar el listado de usuarios.");
            }
        }

        private void CargarListasFijas()
        {
            var bll = new Usuario_TLL();

            ddlNuevoRol.Items.Clear();
            invRol.Items.Clear();

            var deGestion = bll.RolesDeGestion();

            foreach (string rol in bll.RolesAsignables(actor))
            {
                var item = new ListItem(EtiquetaRol(rol), rol);

                // Cambiar a un rol de gestión pide confirmar el aviso (la pantalla lo muestra según esta marca).
                if (deGestion.Contains(rol)) item.Attributes["data-gestion"] = "1";

                ddlNuevoRol.Items.Add(item);
                invRol.Items.Add(new ListItem(EtiquetaRol(rol), rol));
            }

            ddlNuevoEstado.Items.Clear();
            ddlNuevoEstado.Items.Add(new ListItem(EtiquetaEstado(EstadoUsuario.Activo), ((int)EstadoUsuario.Activo).ToString()));
            ddlNuevoEstado.Items.Add(new ListItem(EtiquetaEstado(EstadoUsuario.Inactivo), ((int)EstadoUsuario.Inactivo).ToString()));
            ddlNuevoEstado.Items.Add(new ListItem(EtiquetaEstado(EstadoUsuario.BloqueoEstricto), ((int)EstadoUsuario.BloqueoEstricto).ToString()));

            if (VeTodas)
            {
                CargarEmpresas(invEmpresa, false);
                invEmpresa.Attributes["data-propia"] = actor.IdEmpresa.ToString();

                // Arranca en la empresa del propio gestor: elegir otra es una decisión con aviso.
                if (!IsPostBack && invEmpresa.Items.FindByValue(actor.IdEmpresa.ToString()) != null)
                    invEmpresa.SelectedValue = actor.IdEmpresa.ToString();
            }

            if (phDatosAvanzados.Visible) CargarEmpresas(ddlDatosEmpresa, false);
        }

        private void CargarFiltros()
        {
            ddlRolFiltro.Items.Add(new ListItem("Todos los roles", string.Empty));
            foreach (string rol in new Usuario_TLL().RolesAsignables(actor))
                ddlRolFiltro.Items.Add(new ListItem(EtiquetaRol(rol), rol));

            ddlEstadoFiltro.Items.Add(new ListItem("Todos los estados", string.Empty));
            foreach (EstadoUsuario estado in Enum.GetValues(typeof(EstadoUsuario)))
                ddlEstadoFiltro.Items.Add(new ListItem(EtiquetaEstado(estado), ((int)estado).ToString()));

            if (VeTodas)
            {
                CargarEmpresas(ddlEmpresaFiltro, true);

                int empresa;
                if (int.TryParse(Request.QueryString["empresa"], out empresa) && ddlEmpresaFiltro.Items.FindByValue(empresa.ToString()) != null)
                    ddlEmpresaFiltro.SelectedValue = empresa.ToString();
            }
        }

        private void CargarEmpresas(DropDownList lista, bool conTodas)
        {
            lista.Items.Clear();

            if (conTodas) lista.Items.Add(new ListItem("Todas las empresas", string.Empty));

            foreach (Empresa_BE empresa in new Empresa_BLL().ObtenerTodas())
                lista.Items.Add(new ListItem(empresa.NombreEmpresa, empresa.IdEmpresa.ToString()));
        }

        private FiltroUsuarios_TE ArmarFiltro()
        {
            var filtro = new FiltroUsuarios_TE
            {
                Texto = txtBuscar.Text,
                Rol = ddlRolFiltro.SelectedValue,
                Pagina = PaginaActual,
                Tamano = TAMANO_PAGINA
            };

            int estado;
            if (int.TryParse(ddlEstadoFiltro.SelectedValue, out estado) && Enum.IsDefined(typeof(EstadoUsuario), estado))
                filtro.Estado = (EstadoUsuario)estado;

            int empresa;
            if (VeTodas && int.TryParse(ddlEmpresaFiltro.SelectedValue, out empresa)) filtro.IdEmpresa = empresa;

            return filtro;
        }

        private void MostrarPaginado()
        {
            int desde = pagina.Total == 0 ? 0 : (pagina.Pagina - 1) * pagina.Tamano + 1;
            int hasta = desde == 0 ? 0 : desde + pagina.Items.Count - 1;

            litPaginado.Text = pagina.Total == 0
                ? "Sin resultados"
                : "Mostrando " + desde + "–" + hasta + " de " + pagina.Total;

            lnkAnterior.Visible = pagina.Pagina > 1;
            lnkSiguiente.Visible = pagina.Pagina < pagina.TotalPaginas;
        }

        private void MostrarFicha()
        {
            if (VeTodas || actor.IdEmpresa <= 0) { phFicha.Visible = false; return; }

            Empresa_BE empresa = new Empresa_BLL().ObtenerPorId(actor.IdEmpresa);
            if (empresa == null) { phFicha.Visible = false; return; }

            string nombre = empresa.NombreEmpresa ?? string.Empty;

            litSigla.Text = Server.HtmlEncode(nombre.Length >= 2 ? nombre.Substring(0, 2).ToUpperInvariant() : nombre.ToUpperInvariant());
            litEmpresaNombre.Text = Server.HtmlEncode(nombre);
            litEmpresaAlta.Text = "Cliente desde " + empresa.FechaAlta.ToString("MMM yyyy", new CultureInfo("es-AR"));
            litEmpresaUsuarios.Text = pagina.Total + (pagina.Total == 1 ? " usuario" : " usuarios");
            litEmpresaPlan.Text = "Plan " + empresa.PlanSuscripcion;
            litEmpresaEstado.Text = empresa.Estado.ToString();
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

        protected void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            int idUsuario;
            int estado;

            if (actor == null || !int.TryParse(hfIdUsuario.Value, out idUsuario) || !int.TryParse(ddlNuevoEstado.SelectedValue, out estado))
            {
                Avisar("aviso-peligro", "No se pudo identificar al usuario. Volvé a intentarlo.");
                return;
            }

            Ejecutar("Usuarios.CambiarEstado", "El estado de la cuenta se actualizó.", () =>
                new Usuario_TLL().CambiarEstado(actor, idUsuario, (EstadoUsuario)estado, txtMotivoEstado.Text));
        }

        protected void btnCambiarRol_Click(object sender, EventArgs e)
        {
            int idUsuario;

            if (actor == null || !int.TryParse(hfIdUsuario.Value, out idUsuario))
            {
                Avisar("aviso-peligro", "No se pudo identificar al usuario. Volvé a intentarlo.");
                return;
            }

            Ejecutar("Usuarios.CambiarRol", "El rol se actualizó.", () =>
                new Usuario_TLL().CambiarRol(actor, idUsuario, ddlNuevoRol.SelectedValue, txtMotivoRol.Text, chkConfirmaRol.Checked));
        }

        protected void btnGuardarDatos_Click(object sender, EventArgs e)
        {
            int idUsuario;
            int idioma;

            if (actor == null || !int.TryParse(hfIdDatos.Value, out idUsuario) || !int.TryParse(ddlDatosIdioma.SelectedValue, out idioma))
            {
                Avisar("aviso-peligro", "No se pudo identificar al usuario. Volvé a intentarlo.");
                return;
            }

            var tll = new Usuario_TLL();
            Usuario_TE actual = tll.ObtenerPorId(idUsuario);

            if (actual == null)
            {
                Avisar("aviso-peligro", "El usuario no existe.");
                return;
            }

            // Lo que esta cuenta no puede cambiar (correo y empresa) se conserva tal cual.
            string email = actual.EmailUsuario;
            int idEmpresa = actual.IdEmpresa;

            if (phDatosAvanzados.Visible)
            {
                email = txtDatosEmail.Text;
                if (!int.TryParse(ddlDatosEmpresa.SelectedValue, out idEmpresa)) idEmpresa = actual.IdEmpresa;
            }

            try
            {
                string token = tll.ModificarDatosUsuario(actor, idUsuario, txtDatosNombre.Text, txtDatosApellido.Text, idioma, email, idEmpresa, txtDatosMotivo.Text, chkDatosConfirma.Checked);

                if (token != null)
                {
                    CorreosCuenta_GUI.EnviarActivacion(email.Trim().ToLowerInvariant(), txtDatosNombre.Text.Trim(), token,
                        "Actualizamos el correo de tu cuenta de Falke. Sigue pendiente de activación.");
                }

                Avisar("aviso-exito", "Los datos del usuario se actualizaron." + (token != null ? " Como su cuenta sigue pendiente, le enviamos un enlace de activación nuevo al correo nuevo." : string.Empty));
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
                LogErrores_SERVICE.Registrar("Usuarios.ModificarDatos", ex);
                Avisar("aviso-peligro", "No se pudo completar la operación. Volvé a intentarlo.");
            }
        }

        protected void btnInvitar_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            string nombre = invNombre.Text.Trim();
            string apellido = invApellido.Text.Trim();
            string email = invEmail.Text.Trim();

            if (nombre.Length == 0 || apellido.Length == 0 || email.Length == 0)
            {
                AvisarInvitacion("aviso-peligro", "Nombre, apellido y correo son obligatorios.");
                return;
            }

            int idEmpresa = actor.IdEmpresa;
            if (actor.Puede(Patentes_TLL.CREAR_USUARIO_OTRA_EMPRESA) && !int.TryParse(invEmpresa.SelectedValue, out idEmpresa))
                idEmpresa = actor.IdEmpresa;

            try
            {
                var usuario = new Usuario_TE
                {
                    IdEmpresa = idEmpresa,
                    NombreUsuario = nombre,
                    ApellidoUsuario = apellido,
                    EmailUsuario = email,
                    Rol = new PermisoCompuesto_TE(invRol.SelectedValue, true),
                    IdIdioma = ID_IDIOMA_ESPANOL,
                    EsCuentaEmergencia = false
                };

                string token = new Usuario_TLL().RegistrarUsuario(actor, usuario);

                string enlace = WebHelper.UrlAbsoluta("DefinirClave.aspx?token=" + Uri.EscapeDataString(token));

                string cuerpo =
                    "Hola " + nombre + "," + Environment.NewLine + Environment.NewLine +
                    "Se creó tu cuenta en Falke." + Environment.NewLine +
                    "Para activarla y definir tu contraseña, entrá a este enlace (vence en 48 horas):" + Environment.NewLine +
                    enlace + Environment.NewLine;

                Email_SERVICE.Enviar(email, "Activá tu cuenta de Falke", cuerpo);

                invNombre.Text = string.Empty;
                invApellido.Text = string.Empty;
                invEmail.Text = string.Empty;

                Avisar("aviso-exito", "Se creó la cuenta de " + email + " en estado «Pendiente de activación». El enlace de activación quedó en App_Data/mails/.");
            }
            catch (UnauthorizedAccessException ex)
            {
                AvisarInvitacion("aviso-peligro", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                AvisarInvitacion("aviso-peligro", ex.Message);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Usuarios.Invitar", ex);
                AvisarInvitacion("aviso-peligro", "No se pudo registrar el usuario. Volvé a intentarlo.");
            }
        }

        private void Ejecutar(string origen, string exito, Action accion)
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

        private void AvisarInvitacion(string variante, string texto)
        {
            pnlInvitarAviso.CssClass = "aviso " + variante;
            litInvitarAviso.Text = Server.HtmlEncode(texto);
            pnlInvitarAviso.Visible = true;

            AbrirModal("modalInvitar");
        }

        private void AbrirModal(string id)
        {
            ClientScript.RegisterStartupScript(GetType(), "abrir" + id,
                "window.addEventListener('load',function(){try{window.Falke.abrirModal('" + id + "');}catch(e){}});", true);
        }

        protected string Iniciales(object item)
        {
            var u = (UsuarioListado_TE)item;
            string n = string.IsNullOrEmpty(u.NombreUsuario) ? "?" : u.NombreUsuario.Substring(0, 1);
            string a = string.IsNullOrEmpty(u.ApellidoUsuario) ? string.Empty : u.ApellidoUsuario.Substring(0, 1);

            return (n + a).ToUpperInvariant();
        }

        protected string NombreCompleto(object item)
        {
            var u = (UsuarioListado_TE)item;

            return (u.NombreUsuario + " " + u.ApellidoUsuario).Trim();
        }

        protected bool EsOtro(object item)
        {
            return ((UsuarioListado_TE)item).IdUsuario != actor.IdUsuario;
        }

        protected bool EsOtraEmpresa(object item)
        {
            return ((UsuarioListado_TE)item).IdEmpresa != actor.IdEmpresa;
        }

        protected static string EtiquetaRol(string rol)
        {
            return Etiquetas_GUI.De(rol);
        }

        // Las cuentas dadas de baja o con bloqueo estricto se muestran atenuadas.
        protected static string ClaseFila(object item)
        {
            EstadoUsuario estado = ((UsuarioListado_TE)item).Estado;

            return estado == EstadoUsuario.Inactivo || estado == EstadoUsuario.BloqueoEstricto ? "fila-deshabilitada" : string.Empty;
        }

        protected static string EtiquetaEstado(EstadoUsuario estado)
        {
            switch (estado)
            {
                case EstadoUsuario.Pendiente: return "Pendiente de activación";
                case EstadoUsuario.Activo: return "Activo";
                case EstadoUsuario.BloqueadoPorIntentos: return "Bloqueado";
                case EstadoUsuario.Inactivo: return "Inactivo";
                case EstadoUsuario.BloqueoEstricto: return "Bloqueo estricto";
                default: return estado.ToString();
            }
        }

        protected static string ClaseEstado(EstadoUsuario estado)
        {
            switch (estado)
            {
                case EstadoUsuario.Activo: return "badge-exito";
                case EstadoUsuario.Pendiente: return "badge-alerta";
                case EstadoUsuario.BloqueadoPorIntentos:
                case EstadoUsuario.BloqueoEstricto: return "badge-peligro";
                default: return "badge-neutro";
            }
        }
    }
}
