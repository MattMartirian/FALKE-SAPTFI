using System;
using System.Collections.Generic;

namespace AgenteMirada
{
    internal static class Program
    {
        private const int PUERTO = 8887;
        private const string PREFIJO_ORIGEN = "--origen=";

        private static void Main(string[] args)
        {
            bool simulado = Array.IndexOf(args, "--sim") >= 0;

            var origenes = new List<string> { "http://localhost:53192" };

            foreach (string argumento in args)
            {
                if (argumento.StartsWith(PREFIJO_ORIGEN)) origenes.Add(argumento.Substring(PREFIJO_ORIGEN.Length));
            }

            var servidor = new ServidorMirada(PUERTO, origenes);
            IFuenteMirada fuente = simulado ? (IFuenteMirada)new FuenteSimulada() : new FuenteTobii();

            fuente.MuestraRecibida += servidor.Difundir;
            fuente.EstadoCambiado += servidor.DifundirEstado;

            servidor.Iniciar();
            fuente.Iniciar();

            Console.WriteLine("Agente de mirada en ws://localhost:" + PUERTO + "/mirada (" + (simulado ? "simulado" : "Tobii") + ")");
            Console.WriteLine("Origenes permitidos: " + string.Join(", ", origenes));
            Console.WriteLine("Presione Enter para cerrar.");
            Console.ReadLine();

            fuente.Detener();
            servidor.Detener();
        }
    }
}
