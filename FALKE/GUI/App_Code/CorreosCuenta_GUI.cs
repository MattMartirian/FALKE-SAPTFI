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
                "Hola " + nombre + "," + Environment.NewLine + Environment.NewLine +
                introduccion + Environment.NewLine +
                "Para activarla y definir tu contraseña, entrá a este enlace (vence en 48 horas):" + Environment.NewLine +
                enlace + Environment.NewLine;

            Email_SERVICE.Enviar(email, "Activá tu cuenta de Falke", cuerpo);
        }
    }
}
