using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Respaldos : PaginaConAviso_GUI
    {
        private const string CLAVE_AVISO = "respaldos.aviso";
        private static readonly CultureInfo Cultura = new CultureInfo("es-AR");

        private ActorUsuario_TE actor;

        // Entrar y ver es VER_RESPALDOS; generar y restaurar piden cada uno el suyo.
        protected bool PuedeGenerar { get; private set; }
        protected bool PuedeRestaurar { get; private set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            actor = SesionActual_GUI.ObtenerActor();

            if (actor == null) return;

            if (!actor.Puede(Patentes_TLL.VER_RESPALDOS))
            {
                Response.Redirect("SinPermiso.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                actor = null;
                return;
            }

            PuedeGenerar = actor.Puede(Patentes_TLL.HACER_RESPALDO);
            PuedeRestaurar = actor.Puede(Patentes_TLL.RESTAURAR_RESPALDO);

            phBotonGenerar.Visible = PuedeGenerar;
            phAvisoRestaurar.Visible = PuedeRestaurar;
            phModalRestaurar.Visible = PuedeRestaurar;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (actor == null)
            {
                SesionActual_GUI.Exigir();
                return;
            }

            if (IsPostBack) return;

            string aviso = Session[CLAVE_AVISO] as string;
            Session.Remove(CLAVE_AVISO);

            if (!string.IsNullOrEmpty(aviso)) Avisar("aviso-exito", aviso);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                List<Respaldo_TE> respaldos = new Respaldo_TLL().Listar(actor);

                rptRespaldos.DataSource = respaldos;
                rptRespaldos.DataBind();
                phVacio.Visible = respaldos.Count == 0;

                litTotal.Text = respaldos.Count.ToString(CultureInfo.InvariantCulture);
                litUltimo.Text = respaldos.Count == 0 ? "—" : Server.HtmlEncode(Fecha(respaldos[0].FechaGeneracion));
                litEspacio.Text = Server.HtmlEncode(Tamano(respaldos.Where(r => r.ArchivoDisponible).Sum(r => r.Tamano ?? 0)));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Respaldos.Listar", ex);
                Avisar("aviso-peligro", "No se pudo cargar la lista de respaldos.");
            }
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                Respaldo_TE creado = new Respaldo_TLL().Generar(actor);

                Session[CLAVE_AVISO] = "Se generó el respaldo " + creado.NombreArchivo + ".";
                Response.Redirect("Respaldos.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (UnauthorizedAccessException ex)
            {
                Avisar("aviso-peligro", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                LogErrores_SERVICE.Registrar("Respaldos.Generar", ex);
                Avisar("aviso-peligro", ex.Message);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Respaldos.Generar", ex);
                Avisar("aviso-peligro", "No se pudo generar el respaldo. Volvé a intentarlo.");
            }
        }

        protected void btnRestaurar_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            int idRespaldo;
            int.TryParse(hfRespaldo.Value, out idRespaldo);

            try
            {
                new Respaldo_TLL().Restaurar(actor, idRespaldo, txtConfirmacion.Text);
            }
            catch (UnauthorizedAccessException ex)
            {
                Fallar(ex.Message);
                return;
            }
            catch (InvalidOperationException ex)
            {
                LogErrores_SERVICE.Registrar("Respaldos.Restaurar", ex);
                Fallar(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Respaldos.Restaurar", ex);
                Fallar("No se pudo restaurar la base. Volvé a intentarlo.");
                return;
            }

            // La base cambió: ninguna sesión abierta (tampoco esta) sigue siendo confiable.
            SesionActual_GUI.CerrarTodasLasSesiones();
            SesionActual_GUI.Cerrar();
            Response.Redirect("Ingresar.aspx?cuenta=respaldo", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void Fallar(string mensaje)
        {
            Avisar("aviso-peligro", mensaje);

            ClientScript.RegisterStartupScript(GetType(), "abrirRestaurar",
                "window.addEventListener('load',function(){try{window.Falke.abrirModal('modalRestaurar');}catch(e){}});", true);
        }

        protected bool MostrarRestaurar(object item)
        {
            return PuedeRestaurar && ((Respaldo_TE)item).ArchivoDisponible;
        }

        protected static string Fecha(DateTime fecha)
        {
            return fecha.ToString("d MMM yyyy, HH:mm", Cultura);
        }

        protected static string Tamano(long? bytes)
        {
            if (!bytes.HasValue || bytes.Value <= 0) return "—";

            double valor = bytes.Value;
            string[] unidades = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;

            while (valor >= 1024 && i < unidades.Length - 1)
            {
                valor /= 1024;
                i++;
            }

            return valor.ToString(i == 0 ? "0" : "0.0", Cultura) + " " + unidades[i];
        }
    }
}
