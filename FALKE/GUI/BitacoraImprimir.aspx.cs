using System;
using System.Collections.Generic;
using System.Globalization;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class BitacoraImprimir : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Exigir()) return;

            ActorUsuario_TLL actor = SesionActual_GUI.ObtenerActor();

            if (actor == null || !actor.Puede(Patentes_TLL.VER_BITACORA))
            {
                Response.Redirect("SinPermiso.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            try
            {
                FiltroBitacora_TE filtro = FiltroBitacora_GUI.Leer(Request.QueryString);
                List<BitacoraVista_TE> registros = new BitacoraGestor_TLL().Exportar(actor, filtro, "PDF");

                rptRegistros.DataSource = registros;
                rptRegistros.DataBind();

                litMeta.Text = Server.HtmlEncode(
                    "Generado el " + DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) +
                    " por " + SesionActual_GUI.Nombre + ". " + registros.Count + " registros. " + DescribirFiltros(filtro));
            }
            catch (InvalidOperationException ex)
            {
                Fallar(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Fallar(ex.Message);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Bitacora.Imprimir", ex);
                Fallar("No se pudo generar la vista de la bitácora.");
            }
        }

        private void Fallar(string mensaje)
        {
            phInforme.Visible = false;
            litError.Text = Server.HtmlEncode(mensaje);
            phError.Visible = true;
        }

        private static string DescribirFiltros(FiltroBitacora_TE f)
        {
            var partes = new List<string>();

            if (f.Desde.HasValue) partes.Add("desde " + f.Desde.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            if (f.Hasta.HasValue) partes.Add("hasta " + f.Hasta.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            if (f.HoraDesde.HasValue) partes.Add("hora desde " + f.HoraDesde.Value.ToString(@"hh\:mm", CultureInfo.InvariantCulture));
            if (f.HoraHasta.HasValue) partes.Add("hora hasta " + f.HoraHasta.Value.ToString(@"hh\:mm", CultureInfo.InvariantCulture));
            if (!string.IsNullOrEmpty(f.Modulo)) partes.Add("módulo " + f.Modulo);
            if (!string.IsNullOrEmpty(f.Accion)) partes.Add("acción " + f.Accion);
            if (!string.IsNullOrEmpty(f.Usuario)) partes.Add("usuario «" + f.Usuario + "»");
            if (f.IdEmpresa.HasValue) partes.Add("empresa n.º " + f.IdEmpresa.Value);
            if (f.Criticidad.HasValue) partes.Add("criticidad " + f.Criticidad.Value);

            return partes.Count == 0 ? "Sin filtros." : "Filtros: " + string.Join(", ", partes) + ".";
        }
    }
}
