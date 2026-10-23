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
    public partial class Usuarios : PaginaConAviso_GUI
    {
        private const int TAMANO_PAGINA = 25;

        private ActorUsuario_TE actor;
        private PaginaUsuarios_TE pagina;

        protected bool VeTodas { get; private set; }
        protected bool PuedeGestionar { get; private set; }
        protected bool PuedeGestionarCuenta { get; private set; }
        protected bool PuedeEditarDatos { get; private set; }
        protected bool PuedeInvitar { get; private set; }

        private int PaginaActual
        {
            get { return ViewState["pagina"] == null ? 1 : (int)ViewState["pagina"]; }
            set { ViewState["pagina"] = value; }
        }

        private void CargarIdiomas()
        {
            try
            {
                foreach (Idioma_TE idioma in new Idioma_TLL().ListarActivos())
                    ddlDatosIdioma.Items.Add(new ListItem(idioma.NombreIdioma, idioma.IdIdioma.ToString(CultureInfo.InvariantCulture)));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Usuarios.Idiomas", ex);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            actor = SesionActual_GUI.ObtenerActor();

            if (actor == null) return;

            CargarIdiomas();

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

            VeTodas = actor.VeTodasLasEmpresas();
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
            PuedeInvitar = puedeAlta;
            phColAcciones.Visible = PuedeGestionar || PuedeInvitar;
            phGestionEstado.Visible = puedeEstado;
            phGestionRol.Visible = puedeRol;
            phDatosAvanzados.Visible = actor.Puede(Patentes_TLL.CAMBIAR_EMAIL_EMPRESA_USUARIO);

            MostrarBajada();
            CargarListasFijas();
        }

        protected override void AlCambiarIdioma()
        {
            if (actor == null) return;

            MostrarBajada();

            ConservandoSeleccion(() =>
            {
                CargarListasFijas();
                CargarFiltros();
            }, ddlNuevoRol, invRol, ddlNuevoEstado, ddlRolFiltro, ddlEstadoFiltro, ddlEmpresaFiltro, invEmpresa, ddlDatosEmpresa);
        }

        private void MostrarBajada()
        {
            litBajada.Text = VeTodas
                ? IdiomaActual_GUI.Texto("usuarios_titulo_todas")
                : PuedeGestionar
                    ? IdiomaActual_GUI.Texto("usuarios_titulo_admin")
                    : IdiomaActual_GUI.Texto("usuarios_titulo_integrantes");
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
                Avisar("aviso-peligro", IdiomaActual_GUI.Texto("usuarios_error_listado"));
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

                if (!IsPostBack && invEmpresa.Items.FindByValue(actor.IdEmpresa.ToString()) != null)
                    invEmpresa.SelectedValue = actor.IdEmpresa.ToString();
            }

            if (phDatosAvanzados.Visible) CargarEmpresas(ddlDatosEmpresa, false);
        }

        private void CargarFiltros()
        {
            ddlRolFiltro.Items.Clear();
            ddlEstadoFiltro.Items.Clear();

            ddlRolFiltro.Items.Add(new ListItem(IdiomaActual_GUI.Texto("usuarios_todos_roles"), string.Empty));
            foreach (string rol in new Usuario_TLL().RolesAsignables(actor))
                ddlRolFiltro.Items.Add(new ListItem(EtiquetaRol(rol), rol));

            ddlEstadoFiltro.Items.Add(new ListItem(IdiomaActual_GUI.Texto("usuarios_todos_estados"), string.Empty));
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

            if (conTodas) lista.Items.Add(new ListItem(IdiomaActual_GUI.Texto("usuarios_todas_empresas"), string.Empty));

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
                ? IdiomaActual_GUI.Texto("usuarios_sin_resultados")
                : IdiomaActual_GUI.Texto("usuarios_mostrando", desde, hasta, pagina.Total);

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
            litEmpresaAlta.Text = IdiomaActual_GUI.Texto("usuarios_cliente_desde", empresa.FechaAlta.ToString("MMM yyyy", IdiomaActual_GUI.Cultura));
            litEmpresaUsuarios.Text = pagina.Total == 1 ? Traducir("comun_usuario_singular") : Traducir("comun_usuario_plural", pagina.Total);
            litEmpresaPlan.Text = IdiomaActual_GUI.Texto("comun_plan_etiqueta", empresa.PlanSuscripcion);
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
                Avisar("aviso-peligro", IdiomaActual_GUI.Texto("usuarios_error_identificar"));
                return;
            }

            Ejecutar("Usuarios.CambiarEstado", IdiomaActual_GUI.Texto("usuarios_estado_actualizado"), () =>
                new Usuario_TLL().CambiarEstado(actor, idUsuario, (EstadoUsuario)estado, txtMotivoEstado.Text));
        }

        protected void btnCambiarRol_Click(object sender, EventArgs e)
        {
            int idUsuario;

            if (actor == null || !int.TryParse(hfIdUsuario.Value, out idUsuario))
            {
                Avisar("aviso-peligro", IdiomaActual_GUI.Texto("usuarios_error_identificar"));
                return;
            }

            Ejecutar("Usuarios.CambiarRol", IdiomaActual_GUI.Texto("usuarios_rol_actualizado"), () =>
                new Usuario_TLL().CambiarRol(actor, idUsuario, ddlNuevoRol.SelectedValue, txtMotivoRol.Text, chkConfirmaRol.Checked));
        }

        protected void btnGuardarDatos_Click(object sender, EventArgs e)
        {
            int idUsuario;
            int idioma;

            if (actor == null || !int.TryParse(hfIdDatos.Value, out idUsuario) || !int.TryParse(ddlDatosIdioma.SelectedValue, out idioma))
            {
                Avisar("aviso-peligro", IdiomaActual_GUI.Texto("usuarios_error_identificar"));
                return;
            }

            var tll = new Usuario_TLL();
            Usuario_TE actual = tll.ObtenerPorId(idUsuario);

            if (actual == null)
            {
                Avisar("aviso-peligro", IdiomaActual_GUI.Texto("usuario_no_existe"));
                return;
            }

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
                        IdiomaActual_GUI.Texto("usuarios_mail_correo_actualizado"));
                }

                Avisar("aviso-exito", IdiomaActual_GUI.Texto("usuarios_datos_actualizados") + (token != null ? IdiomaActual_GUI.Texto("usuarios_datos_actualizados_pendiente") : string.Empty));
            }
            catch (UnauthorizedAccessException ex)
            {
                Avisar("aviso-peligro", MensajeDe(ex));
            }
            catch (InvalidOperationException ex)
            {
                Avisar("aviso-peligro", MensajeDe(ex));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Usuarios.ModificarDatos", ex);
                Avisar("aviso-peligro", IdiomaActual_GUI.Texto("comun_error_reintentar"));
            }
        }

        protected void rptUsuarios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (actor == null || e.CommandName != "reenviar") return;

            pnlAviso.Visible = false;

            int idUsuario;
            if (!int.TryParse(Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture), out idUsuario))
            {
                Avisar("aviso-peligro", IdiomaActual_GUI.Texto("usuarios_error_identificar"));
                return;
            }

            try
            {
                SolicitudEnlace_TLL solicitud = new Usuario_TLL().ReenviarInvitacion(actor, idUsuario);

                CorreosCuenta_GUI.EnviarActivacion(solicitud.Email, solicitud.Nombre, solicitud.Token,
                    IdiomaActual_GUI.Texto("usuarios_invitacion_reenviada"));

                InvitacionesEnviadas()[idUsuario] = DateTime.Now;
            }
            catch (UnauthorizedAccessException ex)
            {
                Avisar("aviso-peligro", MensajeDe(ex));
            }
            catch (InvalidOperationException ex)
            {
                Avisar("aviso-peligro", MensajeDe(ex));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Usuarios.ReenviarInvitacion", ex);
                Avisar("aviso-peligro", IdiomaActual_GUI.Texto("usuarios_error_reenviar"));
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
                AvisarInvitacion("aviso-peligro", IdiomaActual_GUI.Texto("usuarios_invitar_obligatorios"));
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
                    IdIdioma = IdiomaActual_GUI.IdIdiomaBase(),
                    EsCuentaEmergencia = false
                };

                string token = new Usuario_TLL().RegistrarUsuario(actor, usuario);

                CorreosCuenta_GUI.EnviarActivacion(email, nombre, token, IdiomaActual_GUI.Texto("correo_cuenta_creada"));

                invNombre.Text = string.Empty;
                invApellido.Text = string.Empty;
                invEmail.Text = string.Empty;

                Avisar("aviso-exito", IdiomaActual_GUI.Texto("usuarios_invitacion_ok", email));
            }
            catch (UnauthorizedAccessException ex)
            {
                AvisarInvitacion("aviso-peligro", MensajeDe(ex));
            }
            catch (InvalidOperationException ex)
            {
                AvisarInvitacion("aviso-peligro", MensajeDe(ex));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Usuarios.Invitar", ex);
                AvisarInvitacion("aviso-peligro", IdiomaActual_GUI.Texto("usuarios_error_registrar"));
            }
        }

        private void AvisarInvitacion(string variante, string texto)
        {
            Avisar(pnlInvitarAviso, litInvitarAviso, variante, texto);

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

        // Las invitaciones reenviadas se recuerdan un rato en la sesión: el botón queda gris, con un check, en lugar de dar un aviso.
        private const string CLAVE_INVITACIONES = "usuarios.invitacionesEnviadas";
        private static readonly TimeSpan MEMORIA_DE_INVITACIONES = TimeSpan.FromMinutes(15);

        private System.Collections.Generic.Dictionary<int, DateTime> InvitacionesEnviadas()
        {
            var enviadas = Session[CLAVE_INVITACIONES] as System.Collections.Generic.Dictionary<int, DateTime>;

            if (enviadas == null) Session[CLAVE_INVITACIONES] = enviadas = new System.Collections.Generic.Dictionary<int, DateTime>();

            return enviadas;
        }

        protected bool InvitacionEnviada(object item)
        {
            DateTime cuando;

            return InvitacionesEnviadas().TryGetValue(((UsuarioListado_TE)item).IdUsuario, out cuando) && DateTime.Now - cuando < MEMORIA_DE_INVITACIONES;
        }

        protected bool EsPendiente(object item)
        {
            return ((UsuarioListado_TE)item).Estado == EstadoUsuario.Pendiente;
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
                case EstadoUsuario.Pendiente: return IdiomaActual_GUI.Texto("comun_pendiente_activacion");
                case EstadoUsuario.Activo: return IdiomaActual_GUI.Texto("comun_estado_activo");
                case EstadoUsuario.BloqueadoPorIntentos: return IdiomaActual_GUI.Texto("comun_estado_bloqueado");
                case EstadoUsuario.Inactivo: return IdiomaActual_GUI.Texto("comun_estado_inactivo");
                case EstadoUsuario.BloqueoEstricto: return IdiomaActual_GUI.Texto("comun_estado_bloqueo_estricto");
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
