using System;
using SERVICES;

namespace GUI
{
    public static class CorreosCuenta_GUI
    {
        public static void EnviarActivacion(string email, string nombre, string token, string introduccion)
        {
            string enlace = WebHelper.UrlAbsoluta("DefinirClave.aspx?token=" + Uri.EscapeDataString(token));

            string cuerpo =
                IdiomaActual_GUI.Texto("correo_hola", nombre) + Environment.NewLine + Environment.NewLine +
                introduccion + Environment.NewLine +
                IdiomaActual_GUI.Texto("correo_activar_enlace") + Environment.NewLine +
                enlace + Environment.NewLine;

            Email_SERVICE.Enviar(email, IdiomaActual_GUI.Texto("usuarios_mail_invitacion_asunto"), cuerpo);
        }
    }
}
