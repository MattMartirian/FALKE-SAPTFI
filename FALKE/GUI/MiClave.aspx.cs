using System;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class MiClave : PaginaConAviso_GUI
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Exigir()) return;

            if (SesionActual_GUI.EsEmergencia)
            {
                claveActual.Enabled = false;
                claveNueva.Enabled = false;
                claveRepetir.Enabled = false;
                btnGuardarClave.Enabled = false;
                Avisar("aviso-alerta", IdiomaActual_GUI.Texto("miclave_emergencia"));
            }
        }

        protected void btnGuardarClave_Click(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Exigir()) return;

            if (SesionActual_GUI.EsEmergencia) return;

            if (claveNueva.Text != claveRepetir.Text)
            {
                Avisar("aviso-peligro", IdiomaActual_GUI.Texto("miclave_no_coinciden"));
                return;
            }

            try
            {
                string error;
                bool ok = new Contrasena_TLL().CambiarContrasena(SesionActual_GUI.Email, claveActual.Text, claveNueva.Text, out error);

                if (ok)
                {
                    SesionActual_GUI.ActualizarHuellaTrasCambioDeClave();

                    claveActual.Text = string.Empty;
                    claveNueva.Text = string.Empty;
                    claveRepetir.Text = string.Empty;
                    Avisar("aviso-exito", IdiomaActual_GUI.Texto("miclave_actualizada"));
                    return;
                }

                Avisar("aviso-peligro", Traducir(error));
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("MiClave", ex);
                Avisar("aviso-peligro", IdiomaActual_GUI.Texto("comun_error_procesar"));
            }
        }
    }
}
