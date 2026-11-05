using System;
using System.IO;
using System.Threading;

namespace AgenteMirada
{
    internal static class Program
    {
        private const int PUERTO = 8887;

        private static void Main(string[] args)
        {
            if (Array.IndexOf(args, "--desinstalar") >= 0)
            {
                Instalador.Desinstalar();
                return;
            }

            bool simulado = Array.IndexOf(args, "--sim") >= 0;
            bool local = simulado || Array.IndexOf(args, "--local") >= 0;

            if (!local && !Instalador.EstaInstalado)
            {
                Instalador.Instalar();
                return;
            }

            Instalador.ExtraerDll(AppDomain.CurrentDomain.BaseDirectory);

            bool nueva;

            using (new Mutex(true, "Local\\FALKE.AgenteMirada", out nueva))
            {
                if (!nueva) return;

                var grabador = new Grabador(Path.Combine(Path.GetTempPath(), "falke-mirada.jsonl"));
                var servidor = new ServidorAgente(PUERTO, grabador);
                IFuenteMirada fuente = simulado ? (IFuenteMirada)new FuenteSimulada() : new FuenteTobii();

                fuente.MuestraRecibida += grabador.Agregar;
                fuente.EstadoCambiado += estado => grabador.EstadoFuente = estado;

                try
                {
                    servidor.Iniciar();
                }
                catch (Exception error)
                {
                    Registro.Escribir("No se pudo abrir el puerto " + PUERTO + ": " + error.Message);
                    return;
                }

                fuente.Iniciar();

                Registro.Escribir("Agente iniciado (" + (simulado ? "simulado" : "Tobii") + ").");

                new ManualResetEvent(false).WaitOne();
            }
        }
    }
}
