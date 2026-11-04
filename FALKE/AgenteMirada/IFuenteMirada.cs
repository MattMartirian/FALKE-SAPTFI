using System;

namespace AgenteMirada
{
    public interface IFuenteMirada
    {
        event Action<MuestraMirada> MuestraRecibida;
        event Action<string> EstadoCambiado;

        void Iniciar();
        void Detener();
    }
}
