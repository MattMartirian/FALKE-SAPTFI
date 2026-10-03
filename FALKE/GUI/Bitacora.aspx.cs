using System;

namespace GUI
{
    public partial class Bitacora : System.Web.UI.Page
    {
        private const string ROL_ADMINISTRADOR = "Administrador";
        private const string ROL_GESTOR = "Gestor";
        private const string ROL_WEBMASTER = "Webmaster";

        protected void Page_Load(object sender, EventArgs e)
        {

            MostrarAlcance();
        }

        private void MostrarAlcance()
        {
            string rol = Master.RolActual;

            phFiltroEmpresa.Visible = rol == ROL_GESTOR;

            if (rol == ROL_ADMINISTRADOR)
            {
                litBajada.Text = "Movimientos registrados en tu empresa.";
                litAlcance.Text = "Estás viendo solo los movimientos de los usuarios de tu " +
                                  "empresa. Los registros no se pueden editar ni borrar.";
                return;
            }

            if (rol == ROL_WEBMASTER)
            {
                litBajada.Text = "Movimientos de la gestión de la página.";
                litAlcance.Text = "Estás viendo solo los movimientos de sistema y seguridad " +
                                  "referidos a la gestión de la página.";
                return;
            }

            litBajada.Text = "Todos los movimientos registrados en el sistema.";
            litAlcance.Text = "Como gestor ves la bitácora completa de todas las empresas " +
                              "cliente y de Pattern Blue.";
        }
    }
}
