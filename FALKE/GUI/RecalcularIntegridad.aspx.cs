using SERVICES;
using System;
using TLL;

namespace GUI
{
    public partial class RecalcularIntegridad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual.Exigir()) return;
        }

        protected void btnVerificar_Click(object sender, EventArgs e)
        {
            if (!SesionActual.Exigir()) return;

            try
            {
                var inconsistencias = new GestorIntegridad_SERVICE().VerificarIntegridadTodasLasTablas();

                gvResumen.Visible = false;
                gvInconsistencias.DataSource = inconsistencias;
                gvInconsistencias.DataBind();
                gvInconsistencias.Visible = inconsistencias.Count > 0;

                lblMsg.Text = inconsistencias.Count == 0
                    ? "Sin inconsistencias: la integridad almacenada coincide con los datos."
                    : inconsistencias.Count + " inconsistencia(s) detectada(s) (ver detalle).";
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar("RecalcularIntegridad/Verificar", ex);
                lblMsg.Text = "Ocurrio un error al verificar la integridad.";
            }
        }

        protected void btnRecalcular_Click(object sender, EventArgs e)
        {
            if (!SesionActual.Exigir()) return;

            try
            {
                var tll = new GestorIntegridad_SERVICE();

                var resumen = tll.RecalcularTodasLasTablas();
                gvResumen.DataSource = resumen.Tablas;
                gvResumen.DataBind();
                gvResumen.Visible = resumen.Tablas.Count > 0;

                var inconsistencias = tll.VerificarIntegridadTodasLasTablas();
                gvInconsistencias.DataSource = inconsistencias;
                gvInconsistencias.DataBind();
                gvInconsistencias.Visible = inconsistencias.Count > 0;

                lblMsg.Text = "Recalculo completo y almacenado: " + resumen.TablasProcesadas +
                              " tabla(s), " + resumen.RegistrosProcesados + " registro(s). " +
                              (inconsistencias.Count == 0
                                  ? "Verificacion posterior: sin inconsistencias."
                                  : "Verificacion posterior: " + inconsistencias.Count + " inconsistencia(s).");
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar("RecalcularIntegridad/Recalcular", ex);
                lblMsg.Text = "Ocurrio un error al recalcular la integridad.";
            }
        }
    }
}
