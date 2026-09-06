using System;
using System.IO;

namespace SERVICES
{
    public static class LogErrores_SERVICE
    {
        private static readonly object _candado = new object();

        public static void Registrar(string contexto, Exception ex)
        {
            try
            {
                string carpeta = Path.Combine(CarpetaDatos(), "logs");
                Directory.CreateDirectory(carpeta);

                string linea =
                    DateTime.Now.ToString("o") + " | " + contexto + Environment.NewLine +
                    (ex == null ? "(sin excepcion)" : ex.ToString()) + Environment.NewLine +
                    new string('-', 60) + Environment.NewLine;

                lock (_candado)
                {
                    File.AppendAllText(Path.Combine(carpeta, "errores.log"), linea);
                }
            }
            catch
            {
                // intencional: el logging nunca debe tirar
            }
        }

        private static string CarpetaDatos()
        {
            string dir = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            return string.IsNullOrEmpty(dir) ? AppDomain.CurrentDomain.BaseDirectory : dir;
        }
    }
}
