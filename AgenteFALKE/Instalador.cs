using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AgenteMirada
{
    public static class Instalador
    {
        private const string NOMBRE_EXE = "AgenteMirada.exe";
        private const string NOMBRE_DLL = "tobii_stream_engine.dll";
        private const string CLAVE_INICIO = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string VALOR_INICIO = "FALKEAgente";
        private const string TITULO = "FALKE";
        private const int ESPERA_CIERRE_MS = 3000;

        private static string Carpeta
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FALKE", "Agente"); }
        }

        public static bool EstaInstalado
        {
            get { return string.Equals(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), Carpeta, StringComparison.OrdinalIgnoreCase); }
        }

        public static void ExtraerDll(string carpeta)
        {
            string destino = Path.Combine(carpeta, NOMBRE_DLL);

            if (File.Exists(destino)) return;

            using (Stream origen = Assembly.GetExecutingAssembly().GetManifestResourceStream(NOMBRE_DLL))
            using (FileStream salida = File.Create(destino))
            {
                origen.CopyTo(salida);
            }
        }

        public static void Instalar()
        {
            try
            {
                CerrarOtrasCopias();

                Directory.CreateDirectory(Carpeta);

                string exe = Path.Combine(Carpeta, NOMBRE_EXE);

                File.Copy(Assembly.GetExecutingAssembly().Location, exe, true);
                ExtraerDll(Carpeta);

                using (RegistryKey clave = Registry.CurrentUser.OpenSubKey(CLAVE_INICIO, true))
                {
                    clave.SetValue(VALOR_INICIO, "\"" + exe + "\"");
                }

                Process.Start(exe);

                MessageBox.Show("El agente de FALKE qued\u00f3 instalado y se inicia con Windows. Ya puedes volver a la p\u00e1gina.", TITULO);
            }
            catch (Exception error)
            {
                Registro.Escribir("Instalacion: " + error.Message);
                MessageBox.Show("No se pudo instalar el agente: " + error.Message, TITULO);
            }
        }

        public static void Desinstalar()
        {
            CerrarOtrasCopias();

            using (RegistryKey clave = Registry.CurrentUser.OpenSubKey(CLAVE_INICIO, true))
            {
                if (clave != null) clave.DeleteValue(VALOR_INICIO, false);
            }

            MessageBox.Show("El agente de FALKE fue desinstalado.", TITULO);

            var borrado = new ProcessStartInfo("cmd.exe", "/c ping 127.0.0.1 -n 3 >nul & rmdir /s /q \"" + Carpeta + "\"");
            borrado.CreateNoWindow = true;
            borrado.UseShellExecute = false;
            Process.Start(borrado);
        }

        private static void CerrarOtrasCopias()
        {
            int propio = Process.GetCurrentProcess().Id;

            foreach (Process proceso in Process.GetProcessesByName("AgenteMirada"))
            {
                if (proceso.Id == propio) continue;

                proceso.Kill();
                proceso.WaitForExit(ESPERA_CIERRE_MS);
            }
        }
    }
}
