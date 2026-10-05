using System;
using System.Web;
using System.Web.UI.HtmlControls;
using TLL;

namespace GUI
{
    public partial class AppMaster : System.Web.UI.MasterPage
    {
        private string rolActual;

        public string RolActual
        {
            get
            {
                if (rolActual == null) rolActual = Etiquetas_GUI.De(SesionActual_GUI.EsEmergencia ? Usuario_TLL.ROL_GESTOR : SesionActual_GUI.Rol);

                return rolActual;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Las pantallas de la aplicación muestran datos de la cuenta: el navegador no las guarda, así que
            // "atrás" después de cerrar sesión no las vuelve a mostrar.
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));

            // Mientras se restaura la base nadie puede usar la aplicación: se desvía a la pantalla de mantenimiento.
            if (Respaldo_TLL.RestauracionEnCurso)
            {
                Response.Redirect("Mantenimiento.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            SesionActual_GUI.RestaurarDesdeCookie();
            SesionActual_GUI.VerificarVigencia();

            MostrarUsuario();
            AplicarPermisosDeMenu();
            MarcarPaginaActual();
        }

        // El botón de perfil dice, al pasar el mouse, de quién es la sesión.
        private void MostrarUsuario()
        {
            string nombre = SesionActual_GUI.HayUsuario ? (SesionActual_GUI.Nombre ?? string.Empty) : string.Empty;
            string email = SesionActual_GUI.HayUsuario ? (SesionActual_GUI.Email ?? string.Empty) : string.Empty;

            lnkPerfil.Attributes["title"] = "Mi perfil" + (nombre.Length > 0 ? " — " + nombre + " (" + email + ") · " + RolActual : string.Empty);
        }

        private void AplicarPermisosDeMenu()
        {
            bool operaAnalisis = Puede(Patentes_TLL.OPERAR_ANALISIS);

            navVisualizaciones.Visible = operaAnalisis;
            navNuevaGrabacion.Visible = operaAnalisis;
            navCategorias.Visible = operaAnalisis;
            navAnalisisMultiple.Visible = operaAnalisis;

            navUsuarios.Visible = Puede(Patentes_TLL.VER_USUARIOS);
            // Quien ve todas las empresas ya tiene su pantalla de Empresas: "Mi empresa" es para el administrador de un cliente.
            navMiEmpresa.Visible = Puede(Patentes_TLL.VER_DATOS_EMPRESA) && SesionActual_GUI.IdEmpresa > 0 && !Puede(Patentes_TLL.VER_USUARIOS_TODAS_EMPRESAS);
            navEmpresas.Visible = Puede(Patentes_TLL.VER_USUARIOS_TODAS_EMPRESAS);
            navDispositivos.Visible = Puede(Patentes_TLL.VER_DISPOSITIVOS);

            navBitacora.Visible = Puede(Patentes_TLL.VER_BITACORA);
            navIntegridad.Visible = Puede(Patentes_TLL.RECALCULAR_INTEGRIDAD);
            navRoles.Visible = Puede(Patentes_TLL.GESTIONAR_ROLES);
            navRespaldos.Visible = Puede(Patentes_TLL.VER_RESPALDOS);

            grupoOrganizacion.Visible = navUsuarios.Visible || navMiEmpresa.Visible || navEmpresas.Visible || navDispositivos.Visible;
            grupoSistema.Visible = navBitacora.Visible || navIntegridad.Visible || navRoles.Visible || navRespaldos.Visible;
        }

        private static bool Puede(string patente)
        {
            return SesionActual_GUI.HayUsuario && SesionActual_GUI.Puede(patente);
        }

        private void MarcarPaginaActual()
        {
            string archivo = System.IO.Path.GetFileName(Request.CurrentExecutionFilePath);

            if (archivo == "EmpresaNueva.aspx") archivo = "Empresas.aspx";

            Marcar(navPanel, archivo, "Panel.aspx");
            Marcar(navVisualizaciones, archivo, "Visualizaciones.aspx");
            Marcar(navNuevaGrabacion, archivo, "NuevaGrabacion.aspx");
            Marcar(navCategorias, archivo, "Categorias.aspx");
            Marcar(navAnalisisMultiple, archivo, "AnalisisMultiple.aspx");
            Marcar(navUsuarios, archivo, "Usuarios.aspx");
            Marcar(navMiEmpresa, archivo, "MiEmpresa.aspx");
            Marcar(navEmpresas, archivo, "Empresas.aspx");
            Marcar(navDispositivos, archivo, "Dispositivos.aspx");
            Marcar(navBitacora, archivo, "Bitacora.aspx");
            Marcar(navIntegridad, archivo, "Integridad.aspx");
            Marcar(navRoles, archivo, "Roles.aspx");
            Marcar(navRespaldos, archivo, "Respaldos.aspx");
        }

        private static void Marcar(HtmlAnchor link, string archivoActual, string archivoDelLink)
        {
            if (!string.Equals(archivoActual, archivoDelLink, StringComparison.OrdinalIgnoreCase)) return;

            link.Attributes["aria-current"] = "page";
        }
    }
}
