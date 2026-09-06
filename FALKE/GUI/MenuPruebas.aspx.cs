using System;

namespace GUI
{
    public partial class MenuPruebas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SesionActual_GUI.HayUsuario)
            {
                string ubicacion = SesionActual_GUI.EsEmergencia ? "acceso de emergencia" : "empresa " + SesionActual_GUI.IdEmpresa;
                string rol = string.IsNullOrEmpty(SesionActual_GUI.Rol) ? "(sin rol)" : SesionActual_GUI.Rol;

                lblSesion.Text = "Sesion iniciada como: " + SesionActual_GUI.Nombre + " (" + SesionActual_GUI.Email +
                                 ") - rol " + rol + " - " + ubicacion + ".";
            }
            else
            {
                lblSesion.Text = "No hay ninguna sesion iniciada.";
            }

            if (Request["err"] == "permiso") lblAviso.Text = "No tenes permiso para acceder a esa seccion.";

            // Se ocultan los accesos que el permiso del usuario no habilita.
            liRegistrarEmpresa.Visible = SesionActual_GUI.Puede("REGISTRAR_EMPRESA");
            liRegistrarUsuario.Visible = SesionActual_GUI.Puede("REGISTRAR_USUARIO");
            liRecalcular.Visible = SesionActual_GUI.Puede("RECALCULAR_INTEGRIDAD");
        }
    }
}
