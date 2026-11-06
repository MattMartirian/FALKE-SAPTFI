using System;
using System.IO;

namespace AgenteMirada
{
    public static class Registro
    {
        private static readonly object candado = new object();
        private static readonly string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FALKE", "agente.log");
        private static string ultimo;

        public static void Escribir(string texto)
        {
            lock (candado)
            {
                if (texto == ultimo) return;

                ultimo = texto;

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ruta));
                    File.AppendAllText(ruta, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + texto + Environment.NewLine);
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
