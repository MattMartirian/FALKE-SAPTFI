using System;

namespace AgenteMirada
{
    public static class Estados
    {
        public const string Iniciando = "iniciando";
        public const string Conectado = "conectado";
        public const string ConectadoSimulado = "conectado_simulado";
        public const string SinTracker = "sin_tracker";
        public const string ErrorSdk = "error_sdk";
    }

    public struct MuestraMirada
    {
        public double X;
        public double Y;
        public bool Valida;
        public long TiempoUs;
        public long UtcMs;
    }

    public interface IFuenteMirada
    {
        event Action<MuestraMirada> MuestraRecibida;
        event Action<string> EstadoCambiado;

        void Iniciar();
        void Detener();
    }
}
