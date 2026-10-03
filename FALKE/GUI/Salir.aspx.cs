using System;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Salir : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            SesionActual_GUI.RestaurarDesdeCookie();

            if (SesionActual_GUI.HayUsuario)
            {
                litConfirmar.Text = "Vas a cerrar la sesión de " + Server.HtmlEncode(SesionActual_GUI.Email) + " en este navegador.";
                pnlConfirmar.Visible = true;
            }
            else
            {
                litDetalle.Text = "No había ninguna sesión abierta en este navegador.";
                pnlHecho.Visible = true;
            }
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            string email = SesionActual_GUI.HayUsuario ? SesionActual_GUI.Email : null;

            if (email != null)
            {
                RegistrarEnBitacora();
                SesionActual_GUI.Cerrar();
                litDetalle.Text = "Se cerró la sesión de " + Server.HtmlEncode(email) + ".";
            }
            else
            {
                litDetalle.Text = "No había ninguna sesión abierta en este navegador.";
            }

            pnlConfirmar.Visible = false;
            pnlHecho.Visible = true;
        }

        private void RegistrarEnBitacora()
        {
            try
            {
                new BitacoraGestor_TLL().Registrar(SesionActual_GUI.IdUsuario, "Seguridad", "Cierre de sesión", CriticidadBitacora.Baja);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Salir", ex);
            }
        }
    }
}
