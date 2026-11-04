namespace AgenteMirada
{
    public static class FabricaFuente
    {
        public static bool TobiiDisponible
        {
            get { return false; }
        }

        public static IFuenteMirada Crear(bool simulado)
        {
            return new FuenteSimulada();
        }
    }
}
