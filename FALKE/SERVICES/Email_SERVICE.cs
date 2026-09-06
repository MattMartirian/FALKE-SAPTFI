using System;
using System.IO;

namespace SERVICES
{
    /// <summary>
    /// Envio de mails. Por ahora simula el envio escribiendo un archivo en App_Data/mails;
    /// mas adelante va a delegar en una API externa de correo (misma firma publica).
    /// </summary>
    public static class Email_SERVICE
    {
        public static string Enviar(string destino, string asunto, string cuerpo)
        {
            string carpeta = Path.Combine(CarpetaDatos(), "mails");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(
                carpeta,
                DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "_" + Limpiar(destino) + ".txt");

            string contenido =
                "Para:   " + destino + Environment.NewLine +
                "Asunto: " + asunto + Environment.NewLine +
                "Fecha:  " + DateTime.Now.ToString("o") + Environment.NewLine +
                new string('-', 50) + Environment.NewLine +
                cuerpo + Environment.NewLine;

            File.WriteAllText(archivo, contenido);
            return archivo;
        }

        private static string Limpiar(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return "sin-destino";

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                valor = valor.Replace(c, '_');
            }

            return valor.Replace('@', '_');
        }

        // En una app web ASP.NET DataDirectory apunta a App_Data; fuera de ese contexto
        // (tests, tareas) se cae al directorio base del proceso.
        private static string CarpetaDatos()
        {
            string dir = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            return string.IsNullOrEmpty(dir) ? AppDomain.CurrentDomain.BaseDirectory : dir;
        }
    }
}
