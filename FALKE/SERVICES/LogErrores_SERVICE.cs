using System;
using System.IO;

namespace SERVICES
{
    /// <summary>
    /// Log de errores criticos a App_Data/logs/errores.log, disponible para cualquier capa.
    /// Nunca propaga fallos propios: registrar un error no debe romper el flujo que lo reporto.
    /// </summary>
    public static class LogErrores_SERVICE
    {
        // Serializa las escrituras concurrentes: sin esto, dos hilos escribiendo a la vez
        // chocan con IOException (archivo en uso) y el error se pierde.
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
