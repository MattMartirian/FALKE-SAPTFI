using System;
using System.Web.UI.HtmlControls;

namespace GUI
{
    public partial class AppMaster : System.Web.UI.MasterPage
    {
        private const string ROL_ANALISTA = "Analista";
        private const string ROL_ADMINISTRADOR = "Administrador";
        private const string ROL_GESTOR = "Gestor";
        private const string ROL_WEBMASTER = "Webmaster";

        private const string K_ROL_VISTA_PREVIA = "RolVistaPrevia";

        private const int AVISOS_SIN_LEER = 3;

        private string rolActual;

        public string RolActual
        {
            get
            {
                if (rolActual == null) rolActual = ResolverRol();

                return rolActual;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SesionActual_GUI.RestaurarDesdeCookie();

            MostrarUsuario();
            AplicarPermisosDeMenu();
            MarcarPaginaActual();

            phPuntoAviso.Visible = AVISOS_SIN_LEER > 0;
        }

        private string ResolverRol()
        {
            string pedido = Request["rol"];

            if (EsRolConocido(pedido)) Session[K_ROL_VISTA_PREVIA] = pedido;

            string vistaPrevia = Session[K_ROL_VISTA_PREVIA] as string;

            if (EsRolConocido(vistaPrevia)) return vistaPrevia;

            if (SesionActual_GUI.HayUsuario && SesionActual_GUI.EsEmergencia) return ROL_GESTOR;

            if (SesionActual_GUI.HayUsuario && !string.IsNullOrEmpty(SesionActual_GUI.Rol)) return SesionActual_GUI.Rol;

            return ROL_GESTOR;
        }

        private static bool EsRolConocido(string rol)
        {
            if (string.IsNullOrEmpty(rol)) return false;

            return rol == ROL_ANALISTA || rol == ROL_ADMINISTRADOR || rol == ROL_GESTOR || rol == ROL_WEBMASTER;
        }

        private void MostrarUsuario()
        {
            string nombre = "Usuario de demostracion";
            string email = "demo@patternblue.com.ar";

            if (SesionActual_GUI.HayUsuario)
            {
                if (!string.IsNullOrWhiteSpace(SesionActual_GUI.Nombre)) nombre = SesionActual_GUI.Nombre;

                email = SesionActual_GUI.Email;
            }

            litNombre.Text = Server.HtmlEncode(nombre);
            litEmail.Text = Server.HtmlEncode(email);
            litRol.Text = Server.HtmlEncode(RolActual);

            string iniciales = CalcularIniciales(nombre);

            litIniciales.Text = iniciales;
            litIniciales2.Text = iniciales;
        }

        private static string CalcularIniciales(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto)) return "?";

            string[] partes = nombreCompleto.Trim().Split(' ');

            if (partes.Length == 1) return partes[0].Substring(0, 1).ToUpper();

            return (partes[0].Substring(0, 1) + partes[partes.Length - 1].Substring(0, 1)).ToUpper();
        }

        private void AplicarPermisosDeMenu()
        {
            bool esEmergencia = SesionActual_GUI.HayUsuario && SesionActual_GUI.EsEmergencia;

            bool esAnalista = RolActual == ROL_ANALISTA;
            bool esAdministrador = RolActual == ROL_ADMINISTRADOR;
            bool esGestor = RolActual == ROL_GESTOR || esEmergencia;
            bool esWebmaster = RolActual == ROL_WEBMASTER || esEmergencia;

            bool operaAnalisis = esAnalista || esAdministrador || esGestor;

            navVisualizaciones.Visible = operaAnalisis;
            navNuevaGrabacion.Visible = operaAnalisis;
            navCategorias.Visible = operaAnalisis;
            navAnalisisMultiple.Visible = operaAnalisis;

            navUsuarios.Visible = operaAnalisis;
            navEmpresas.Visible = esGestor;
            navDispositivos.Visible = esGestor;

            navBitacora.Visible = esAdministrador || esGestor || esWebmaster;
            navIntegridad.Visible = esWebmaster;
            navRespaldos.Visible = esGestor;

            grupoOrganizacion.Visible = navUsuarios.Visible || navEmpresas.Visible || navDispositivos.Visible;
            grupoSistema.Visible = navBitacora.Visible || navIntegridad.Visible || navRespaldos.Visible;
        }

        private void MarcarPaginaActual()
        {
            string archivo = System.IO.Path.GetFileName(Request.CurrentExecutionFilePath);

            if (archivo == "MiClave.aspx") archivo = "Usuarios.aspx";
            if (archivo == "EmpresaNueva.aspx") archivo = "Empresas.aspx";

            Marcar(navPanel, archivo, "Panel.aspx");
            Marcar(navVisualizaciones, archivo, "Visualizaciones.aspx");
            Marcar(navNuevaGrabacion, archivo, "NuevaGrabacion.aspx");
            Marcar(navCategorias, archivo, "Categorias.aspx");
            Marcar(navAnalisisMultiple, archivo, "AnalisisMultiple.aspx");
            Marcar(navUsuarios, archivo, "Usuarios.aspx");
            Marcar(navEmpresas, archivo, "Empresas.aspx");
            Marcar(navDispositivos, archivo, "Dispositivos.aspx");
            Marcar(navBitacora, archivo, "Bitacora.aspx");
            Marcar(navIntegridad, archivo, "Integridad.aspx");
            Marcar(navRespaldos, archivo, "Respaldos.aspx");
        }

        private static void Marcar(HtmlAnchor link, string archivoActual, string archivoDelLink)
        {
            if (!string.Equals(archivoActual, archivoDelLink, StringComparison.OrdinalIgnoreCase)) return;

            link.Attributes["aria-current"] = "page";
        }
    }
}
