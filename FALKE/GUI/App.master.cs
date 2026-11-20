using System;
using System.Collections.Generic;
using System.Text;
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

        private static readonly HashSet<string> PaginasConAyuda = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "panel", "visualizaciones", "nuevagrabacion", "descargas", "categorias", "analisismultiple", "mistobiis", "usuarios", "miempresa", "empresas", "empresanueva", "dispositivos", "bitacora", "integridad", "roles", "respaldos", "idiomas", "miperfil", "miclave"
        };

        private string paginaActual;

        private string PaginaActual
        {
            get
            {
                if (paginaActual == null) paginaActual = System.IO.Path.GetFileNameWithoutExtension(Request.CurrentExecutionFilePath) ?? string.Empty;

                return paginaActual;
            }
        }

        public bool TieneAyuda
        {
            get { return PaginasConAyuda.Contains(PaginaActual); }
        }

        public string TextoAyuda
        {
            get { return IdiomaActual_GUI.Texto("ayuda_" + PaginaActual.ToLowerInvariant()); }
        }

        public string HtmlAyuda
        {
            get
            {
                StringBuilder html = new StringBuilder();
                string listaAbierta = null;

                foreach (string cruda in (TextoAyuda ?? string.Empty).Replace("\r", string.Empty).Split('\n'))
                {
                    string linea = cruda.Trim();

                    if (linea.Length == 0) continue;

                    string tipo = null;
                    string contenido = linea;
                    int punto = linea.IndexOf(". ", StringComparison.Ordinal);

                    if (linea.StartsWith("- ", StringComparison.Ordinal))
                    {
                        tipo = "ul";
                        contenido = linea.Substring(2);
                    }
                    else if (punto > 0 && punto <= 2 && char.IsDigit(linea[0]))
                    {
                        tipo = "ol";
                        contenido = linea.Substring(punto + 2);
                    }

                    if (tipo != listaAbierta)
                    {
                        if (listaAbierta != null) html.Append("</").Append(listaAbierta).Append(">");
                        if (tipo != null) html.Append("<").Append(tipo).Append(">");

                        listaAbierta = tipo;
                    }

                    if (tipo != null) html.Append("<li>").Append(HttpUtility.HtmlEncode(contenido)).Append("</li>");
                    else html.Append(linea.EndsWith(":", StringComparison.Ordinal) ? "<p class=\"ayuda-intro\">" : "<p>").Append(HttpUtility.HtmlEncode(linea)).Append("</p>");
                }

                if (listaAbierta != null) html.Append("</").Append(listaAbierta).Append(">");

                return html.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));

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
            navDescargas.Visible = operaAnalisis;
            navAnalisisMultiple.Visible = operaAnalisis;
            navMisTobiis.Visible = operaAnalisis;

            navUsuarios.Visible = Puede(Patentes_TLL.VER_USUARIOS);
            navMiEmpresa.Visible = Puede(Patentes_TLL.VER_DATOS_EMPRESA) && SesionActual_GUI.IdEmpresa > 0 && !Puede(Patentes_TLL.VER_USUARIOS_TODAS_EMPRESAS);
            navEmpresas.Visible = Puede(Patentes_TLL.VER_USUARIOS_TODAS_EMPRESAS);
            navDispositivos.Visible = Puede(Patentes_TLL.VER_DISPOSITIVOS);

            navBitacora.Visible = Puede(Patentes_TLL.VER_BITACORA);
            navIntegridad.Visible = Puede(Patentes_TLL.RECALCULAR_INTEGRIDAD);
            navRoles.Visible = Puede(Patentes_TLL.GESTIONAR_ROLES);
            navRespaldos.Visible = Puede(Patentes_TLL.VER_RESPALDOS);
            navIdiomas.Visible = Puede(Patentes_TLL.GESTIONAR_IDIOMAS);

            grupoOrganizacion.Visible = navUsuarios.Visible || navMiEmpresa.Visible || navEmpresas.Visible || navDispositivos.Visible;
            grupoSistema.Visible = navBitacora.Visible || navIntegridad.Visible || navRoles.Visible || navRespaldos.Visible || navIdiomas.Visible;
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
            Marcar(navDescargas, archivo, "Descargas.aspx");
            Marcar(navAnalisisMultiple, archivo, "AnalisisMultiple.aspx");
            Marcar(navMisTobiis, archivo, "MisTobiis.aspx");
            Marcar(navUsuarios, archivo, "Usuarios.aspx");
            Marcar(navMiEmpresa, archivo, "MiEmpresa.aspx");
            Marcar(navEmpresas, archivo, "Empresas.aspx");
            Marcar(navDispositivos, archivo, "Dispositivos.aspx");
            Marcar(navBitacora, archivo, "Bitacora.aspx");
            Marcar(navIntegridad, archivo, "Integridad.aspx");
            Marcar(navRoles, archivo, "Roles.aspx");
            Marcar(navRespaldos, archivo, "Respaldos.aspx");
            Marcar(navIdiomas, archivo, "Idiomas.aspx");
        }

        private static void Marcar(HtmlAnchor link, string archivoActual, string archivoDelLink)
        {
            if (!string.Equals(archivoActual, archivoDelLink, StringComparison.OrdinalIgnoreCase)) return;

            link.Attributes["aria-current"] = "page";
        }
    }
}
