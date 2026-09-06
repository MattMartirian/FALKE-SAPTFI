using SERVICES;
using System;
using TE;
using TLL;

namespace GUI
{
    public partial class RecalcularIntegridad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso("RECALCULAR_INTEGRIDAD")) return;
        }

        protected void btnVerificar_Click(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso("RECALCULAR_INTEGRIDAD")) return;

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

                // El flujo va directo a SERVICES, no a TLL/BLL: la bitacora se registra desde aca.
                if (inconsistencias.Count == 0)
                {
                    new BitacoraGestor_TLL().Registrar(SesionActual_GUI.IdUsuario, "Integridad", "Verificación de integridad ejecutada: sin inconsistencias", CriticidadBitacora.Baja);
                }
                else
                {
                    new BitacoraGestor_TLL().Registrar(SesionActual_GUI.IdUsuario, "Integridad", "Verificación de integridad ejecutada: " + inconsistencias.Count + " inconsistencia(s) detectada(s)", CriticidadBitacora.Alta);
                }
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("RecalcularIntegridad/Verificar", ex);
                lblMsg.Text = "Ocurrio un error al verificar la integridad.";
            }
        }

        protected void btnRecalcular_Click(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso("RECALCULAR_INTEGRIDAD")) return;

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

                // El flujo va directo a SERVICES, no a TLL/BLL: la bitacora se registra desde aca.
                new BitacoraGestor_TLL().Registrar(SesionActual_GUI.IdUsuario, "Integridad", "Recálculo y almacenamiento de DVH/DVV: " + resumen.TablasProcesadas + " tabla(s), " + resumen.RegistrosProcesados + " registro(s)", CriticidadBitacora.Alta);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("RecalcularIntegridad/Recalcular", ex);
                lblMsg.Text = "Ocurrio un error al recalcular la integridad.";
            }
        }
    }
}
