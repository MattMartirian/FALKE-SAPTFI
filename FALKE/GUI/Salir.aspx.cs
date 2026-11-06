using System;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Salir : PaginaBase_GUI
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            SesionActual_GUI.RestaurarDesdeCookie();

            if (SesionActual_GUI.HayUsuario)
            {
                litConfirmar.Text = IdiomaActual_GUI.Texto("salir_confirmar", Server.HtmlEncode(SesionActual_GUI.Email));
                pnlConfirmar.Visible = true;
            }
            else
            {
                MostrarDetalle("salir_sin_sesion", null);
                pnlHecho.Visible = true;
            }
        }

        protected override void AlCambiarIdioma()
        {
            if (pnlConfirmar.Visible) litConfirmar.Text = IdiomaActual_GUI.Texto("salir_confirmar", Server.HtmlEncode(SesionActual_GUI.Email));

            string clave = ViewState["detalle"] as string;

            if (clave != null) MostrarDetalle(clave, ViewState["email"] as string);
        }

        private void MostrarDetalle(string clave, string email)
        {
            ViewState["detalle"] = clave;
            ViewState["email"] = email;
            litDetalle.Text = email == null ? IdiomaActual_GUI.Texto(clave) : IdiomaActual_GUI.Texto(clave, Server.HtmlEncode(email));
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            string email = SesionActual_GUI.HayUsuario ? SesionActual_GUI.Email : null;

            if (email != null)
            {
                RegistrarEnBitacora();
                SesionActual_GUI.Cerrar();
                MostrarDetalle("salir_cerrada", email);
            }
            else
            {
                MostrarDetalle("salir_sin_sesion", null);
            }

            pnlConfirmar.Visible = false;
            pnlHecho.Visible = true;
        }

        private void RegistrarEnBitacora()
        {
            try
            {
                new BitacoraGestor_TLL().Registrar(SesionActual_GUI.IdUsuario, "Seguridad", EventosBitacora_TLL.CIERRE_SESION, CriticidadBitacora.Baja);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Salir", ex);
            }
        }
    }
}
