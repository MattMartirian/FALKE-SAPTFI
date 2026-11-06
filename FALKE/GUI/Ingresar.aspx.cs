using System;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Ingresar : PaginaBase_GUI
    {
        private const string K_MOTIVO = "MotivoAviso";
        private const string K_INTENTOS = "IntentosAviso";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Segundo paso del inicio de sesión: ya con una sesión de identificador nuevo, se completa la sesión con el ticket de un solo uso.
            if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString["inicio"]) && CompletarInicioDeSesion()) return;

            if (!IsPostBack && SesionActual_GUI.RedirigirSiAutenticado()) return;

            if (!IsPostBack && Request.QueryString["cuenta"] == "empresa")
                Avisar(IdiomaActual_GUI.Texto("ingresar_cierre_empresa_bloqueada"));

            if (!IsPostBack && Request.QueryString["cuenta"] == "baja")
                Avisar(IdiomaActual_GUI.Texto("ingresar_cierre_empresa_baja"));

            if (!IsPostBack && Request.QueryString["cuenta"] == "clave")
                Avisar(IdiomaActual_GUI.Texto("ingresar_cierre_clave_cambiada"));

            if (!IsPostBack && Request.QueryString["cuenta"] == "respaldo")
                Avisar(IdiomaActual_GUI.Texto("ingresar_cierre_restauracion"));

            if (!IsPostBack && Request.QueryString["cuenta"] == "desactivada")
                Avisar(IdiomaActual_GUI.Texto("ingresar_cierre_cuenta"));

            if (!IsPostBack) txtEmail.Focus();
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (email.Length == 0 || txtPass.Text.Length == 0)
            {
                Avisar(IdiomaActual_GUI.Texto("ingresar_completar_datos"));
                return;
            }

            try
            {
                var resultado = new Autenticacion_TLL().ValidarCredenciales(email, txtPass.Text);

                if (resultado.Exito)
                {
                    // Se renueva el identificador de la sesión: se descarta la anterior y la sesión nueva se arma en el pedido siguiente.
                    string ticket = InicioDeSesion_GUI.Preparar(resultado.Usuario, chkRecordarme.Checked, resultado.RequiereRevisarIntegridad, Request.UserHostAddress);

                    SesionActual_GUI.DescartarSesionActual();

                    Response.Redirect("Ingresar.aspx?inicio=" + Uri.EscapeDataString(ticket), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                AvisarMotivo(resultado.Motivo, resultado.IntentosRestantes);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Ingresar", ex);
                Avisar(IdiomaActual_GUI.Texto("comun_error_procesar_vuelve"));
            }
        }

        // Devuelve true si inició la sesión (y ya redirigió). Un ticket vencido, repetido o de otra dirección no hace nada: se muestra el formulario.
        private bool CompletarInicioDeSesion()
        {
            InicioDeSesion_GUI.Pendiente pendiente = InicioDeSesion_GUI.Consumir(Request.QueryString["inicio"], Request.UserHostAddress);

            if (pendiente == null) return false;

            IniciarSesion(pendiente.Usuario);
            SesionActual_GUI.FijarHuella(pendiente.Usuario);

            if (pendiente.Recordarme)
                SesionActual_GUI.RecordarEnEsteEquipo(pendiente.Usuario.IdUsuario);

            // Si la integridad de los datos está comprometida y esta cuenta puede repararla, entra directo a Dígito verificador.
            Response.Redirect(pendiente.IrAIntegridad ? "Integridad.aspx" : "Panel.aspx", false);
            Context.ApplicationInstance.CompleteRequest();

            return true;
        }

        private static string TextoDelMotivo(string motivo, int? intentosRestantes = null)
        {
            switch (motivo)
            {
                case "CREDENCIALES_INVALIDAS":
                    // Los intentos restantes solo llegan si el correo existe: con uno inexistente el mensaje es el genérico.
                    return IdiomaActual_GUI.Texto("ingresar_datos_incorrectos") + (intentosRestantes.HasValue
                        ? " " + (intentosRestantes.Value == 1 ? IdiomaActual_GUI.Texto("login_quedan_uno") : IdiomaActual_GUI.Texto("login_quedan_varios", intentosRestantes.Value))
                        : string.Empty);

                case "USUARIO_BLOQUEADO_INTENTOS":
                    return IdiomaActual_GUI.Texto("ingresar_cuenta_bloqueada");

                case Autenticacion_TLL.MOTIVO_EMPRESA_BLOQUEADA:
                    return IdiomaActual_GUI.Texto("ingresar_empresa_bloqueada");

                case Autenticacion_TLL.MOTIVO_EMPRESA_DESHABILITADA:
                    return IdiomaActual_GUI.Texto("ingresar_empresa_baja");

                case "USUARIO_BLOQUEO_ESTRICTO":
                    return IdiomaActual_GUI.Texto("ingresar_bloqueo_estricto");

                case "USUARIO_INACTIVO":
                    return IdiomaActual_GUI.Texto("ingresar_cuenta_baja");

                case "USUARIO_PENDIENTE":
                    return IdiomaActual_GUI.Texto("ingresar_cuenta_pendiente");

                case "DVH_INVALIDO":
                    return IdiomaActual_GUI.Texto("ingresar_sistema_no_disponible");

                default:
                    return IdiomaActual_GUI.Texto("ingresar_no_se_pudo");
            }
        }

        protected override void AlCambiarIdioma()
        {
            string motivo = ViewState[K_MOTIVO] as string;

            if (motivo != null && pnlMensaje.Visible) AvisarMotivo(motivo, ViewState[K_INTENTOS] as int?);
        }

        private void AvisarMotivo(string motivo, int? intentosRestantes)
        {
            Avisar(TextoDelMotivo(motivo, intentosRestantes));

            ViewState[K_MOTIVO] = motivo;
            ViewState[K_INTENTOS] = intentosRestantes;
        }

        private void Avisar(string texto)
        {
            ViewState.Remove(K_MOTIVO);
            ViewState.Remove(K_INTENTOS);

            PublicarAviso(pnlMensaje, litMensaje, null, texto);
        }

        private static void IniciarSesion(Usuario_TE usuario)
        {
            string nombre = (usuario.NombreUsuario + " " + usuario.ApellidoUsuario).Trim();
            string rol = usuario.Rol != null ? usuario.Rol.Nombre : string.Empty;

            SesionActual_GUI.Iniciar(usuario.IdUsuario, usuario.EmailUsuario, nombre, rol, usuario.IdEmpresa, usuario.EsCuentaEmergencia, usuario.Rol);
        }
    }
}
