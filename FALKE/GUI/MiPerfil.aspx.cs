using System;
using System.Collections.Generic;
using System.Globalization;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    // El espacio personal de cada usuario. No pide ningún permiso: basta con haber iniciado sesión, y siempre trabaja sobre la propia cuenta.
    public partial class MiPerfil : System.Web.UI.Page
    {
        private const string CLAVE_AVISO = "perfil.aviso";

        private ActorUsuario_TE actor;
        private Usuario_TE yo;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Exigir()) return;

            actor = SesionActual_GUI.ObtenerActor();

            if (actor == null) return;

            if (!actor.EsEmergencia)
            {
                yo = new Usuario_TLL().ObtenerPorId(actor.IdUsuario);

                if (yo == null)
                {
                    Avisar("aviso-peligro", "No se encontró tu cuenta.");
                    phDatos.Visible = false;
                    return;
                }
            }

            if (IsPostBack) return;

            string aviso = Session[CLAVE_AVISO] as string;
            Session.Remove(CLAVE_AVISO);

            if (!string.IsNullOrEmpty(aviso)) Avisar("aviso-exito", aviso);

            if (yo != null) CargarFormulario();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                MostrarFicha();
                MostrarActividad();
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("MiPerfil.Mostrar", ex);
                Avisar("aviso-peligro", "No se pudo cargar tu perfil.");
            }
        }

        private void CargarFormulario()
        {
            txtNombre.Text = yo.NombreUsuario;
            txtApellido.Text = yo.ApellidoUsuario;
            txtCorreo.Text = yo.EmailUsuario;

            var idioma = ddlIdioma.Items.FindByValue(yo.IdIdioma.ToString(CultureInfo.InvariantCulture));
            if (idioma != null) ddlIdioma.SelectedValue = idioma.Value;
        }

        private void MostrarFicha()
        {
            bool emergencia = actor.EsEmergencia;

            string nombre = emergencia ? "Acceso de emergencia" : (yo.NombreUsuario + " " + yo.ApellidoUsuario).Trim();
            string correo = emergencia ? SesionActual_GUI.Email : yo.EmailUsuario;
            string rol = Etiquetas_GUI.De(emergencia ? Usuario_TLL.ROL_GESTOR : SesionActual_GUI.Rol);
            string empresa = emergencia ? "Pattern Blue" : NombreEmpresa();

            litIniciales.Text = Server.HtmlEncode(Iniciales(nombre));
            litNombreCompleto.Text = Server.HtmlEncode(nombre);
            litCorreoFicha.Text = Server.HtmlEncode(correo ?? string.Empty);
            litRol.Text = Server.HtmlEncode(rol);
            litEmpresaFicha.Text = Server.HtmlEncode(empresa);
            litRolDetalle.Text = Server.HtmlEncode(rol);
            litEmpresaDetalle.Text = Server.HtmlEncode(empresa);

            phDatos.Visible = !emergencia && yo != null;
            phEmergencia.Visible = emergencia;
            phCambiarClave.Visible = !emergencia;
            phSinClave.Visible = emergencia;
        }

        private void MostrarActividad()
        {
            List<Bitacora_TE> actividad = new BitacoraGestor_TLL().ObtenerMiActividad(actor, 10);

            rptActividad.DataSource = actividad;
            rptActividad.DataBind();
            phSinActividad.Visible = actividad.Count == 0;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (actor == null || actor.EsEmergencia || yo == null) return;

            int idioma;
            if (!int.TryParse(ddlIdioma.SelectedValue, out idioma))
            {
                Avisar("aviso-peligro", "Elegí un idioma.");
                return;
            }

            try
            {
                var tll = new Usuario_TLL();
                tll.ActualizarPerfil(actor.IdUsuario, txtNombre.Text, txtApellido.Text, idioma);

                // El nombre de la sesión (arriba a la derecha y en los saludos) se actualiza enseguida.
                Usuario_TE actualizado = tll.ObtenerPorId(actor.IdUsuario);
                if (actualizado != null)
                {
                    SesionActual_GUI.Iniciar(actualizado.IdUsuario, actualizado.EmailUsuario, (actualizado.NombreUsuario + " " + actualizado.ApellidoUsuario).Trim(),
                        actualizado.Rol != null ? actualizado.Rol.Nombre : string.Empty, actualizado.IdEmpresa, false, actualizado.Rol);
                }

                Session[CLAVE_AVISO] = "Tus datos se guardaron.";
                Response.Redirect("MiPerfil.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (InvalidOperationException ex)
            {
                Avisar("aviso-peligro", ex.Message);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("MiPerfil.Guardar", ex);
                Avisar("aviso-peligro", "No se pudieron guardar los cambios. Volvé a intentarlo.");
            }
        }

        private string NombreEmpresa()
        {
            Empresa_BE empresa = new Empresa_BLL().ObtenerPorId(actor.IdEmpresa);

            return empresa != null ? empresa.NombreEmpresa : "—";
        }

        private static string Iniciales(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto)) return "?";

            string[] partes = nombreCompleto.Trim().Split(' ');

            if (partes.Length == 1) return partes[0].Substring(0, 1).ToUpperInvariant();

            return (partes[0].Substring(0, 1) + partes[partes.Length - 1].Substring(0, 1)).ToUpperInvariant();
        }

        private void Avisar(string variante, string texto)
        {
            pnlAviso.CssClass = "aviso " + variante;
            litAviso.Text = Server.HtmlEncode(texto);
            pnlAviso.Visible = true;
        }
    }
}
