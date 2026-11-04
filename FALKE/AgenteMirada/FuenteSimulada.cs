using System;
using System.Diagnostics;
using System.Threading;

namespace AgenteMirada
{
    public class FuenteSimulada : IFuenteMirada
    {
        private const int INTERVALO_MS = 30;

        private readonly Stopwatch reloj = new Stopwatch();
        private Timer temporizador;

        public event Action<MuestraMirada> MuestraRecibida;
        public event Action<string> EstadoCambiado;

        public void Iniciar()
        {
            reloj.Start();

            EstadoCambiado?.Invoke("conectado_simulado");

            temporizador = new Timer(Emitir, null, 0, INTERVALO_MS);
        }

        public void Detener()
        {
            if (temporizador != null) temporizador.Dispose();

            reloj.Stop();
        }

        private void Emitir(object estado)
        {
            double segundos = reloj.Elapsed.TotalSeconds;

            var muestra = new MuestraMirada
            {
                X = 0.5 + 0.35 * Math.Sin(segundos * 0.9),
                Y = 0.5 + 0.30 * Math.Sin(segundos * 1.3),
                Valida = true,
                TiempoUs = reloj.ElapsedTicks * 1000000L / Stopwatch.Frequency
            };

            MuestraRecibida?.Invoke(muestra);
        }
    }
}
