using System;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class DefinirClave : PaginaBase_GUI
    {
        private string Token
        {
            get { return Request.QueryString["token"] ?? string.Empty; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            ResultadoToken_TLL validacion = new Contrasena_TLL().ValidarTokenContrasena(Token);

            if (validacion.Exito)
            {
                litEmail.Text = Server.HtmlEncode(validacion.Email);
            }
            else
            {
                pnlForm.Visible = false;
                MostrarError(TextoDelMotivo(validacion.Motivo));
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNueva.Text != txtRepetir.Text)
            {
                MostrarError(IdiomaActual_GUI.Texto("definirclave_no_coinciden"));
                return;
            }

            try
            {
                ResultadoToken_TLL resultado = new Contrasena_TLL().EstablecerContrasenaConToken(Token, txtNueva.Text);

                if (resultado.Exito)
                {
                    pnlForm.Visible = false;
                    pnlError.Visible = false;
                    pnlOk.Visible = true;
                    return;
                }

                if (resultado.Motivo != "CONTRASENA_DEBIL")
                {
                    pnlForm.Visible = false;
                }

                MostrarError(TextoDelMotivo(resultado.Motivo));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("DefinirClave", ex);
                MostrarError(IdiomaActual_GUI.Texto("comun_error_procesar"));
            }
        }

        private void MostrarError(string texto)
        {
            litError.Text = Server.HtmlEncode(texto);
            pnlError.Visible = true;
        }

        private static string TextoDelMotivo(string motivo)
        {
            switch (motivo)
            {
                case "CONTRASENA_DEBIL":
                    return IdiomaActual_GUI.Texto("clave_politica");

                case "TOKEN_EXPIRADO":
                    return IdiomaActual_GUI.Texto("definirclave_vencido");

                case "TOKEN_USADO":
                    return IdiomaActual_GUI.Texto("definirclave_usado");

                default:
                    return IdiomaActual_GUI.Texto("definirclave_invalido");
            }
        }
    }
}
