using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SERVICES
{
    public static class Email_SERVICE
    {
        public static string Enviar(string destino, string asunto, string cuerpo)
        {
            if (Config("FALKE_SMTP_HOST").Length == 0) return GuardarEnArchivo(destino, asunto, cuerpo);

            try
            {
                EnviarPorSmtp(destino, asunto, cuerpo);
                return string.Empty;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Email.Enviar", ex);
                return GuardarEnArchivo(destino, asunto, cuerpo);
            }
        }

        private static void EnviarPorSmtp(string destino, string asunto, string cuerpo)
        {
            string usuario = Config("FALKE_SMTP_USER");
            string clave = Config("FALKE_SMTP_PASS");
            string remitente = Config("FALKE_SMTP_FROM");
            if (remitente.Length == 0) remitente = usuario;

            int puerto;
            if (!int.TryParse(Config("FALKE_SMTP_PORT"), out puerto)) puerto = 587;

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            using (var cliente = new SmtpClient(Config("FALKE_SMTP_HOST"), puerto))
            using (var mensaje = new MailMessage(new MailAddress(remitente, "FALKE"), new MailAddress(destino)))
            {
                cliente.EnableSsl = !string.Equals(Config("FALKE_SMTP_SSL"), "false", StringComparison.OrdinalIgnoreCase);
                cliente.Timeout = 15000;

                if (usuario.Length > 0) cliente.Credentials = new NetworkCredential(usuario, clave);

                mensaje.Subject = asunto;
                mensaje.Body = cuerpo;
                mensaje.SubjectEncoding = Encoding.UTF8;
                mensaje.BodyEncoding = Encoding.UTF8;

                cliente.Send(mensaje);
            }
        }

        private static string GuardarEnArchivo(string destino, string asunto, string cuerpo)
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

        private static string Config(string clave)
        {
            return (ConfigurationManager.AppSettings[clave] ?? string.Empty).Trim();
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

        private static string CarpetaDatos()
        {
            string dir = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            return string.IsNullOrEmpty(dir) ? AppDomain.CurrentDomain.BaseDirectory : dir;
        }
    }
}
