using System;

namespace AgenteMirada
{
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
