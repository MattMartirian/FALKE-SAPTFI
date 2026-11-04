namespace AgenteMirada
{
    public static class FabricaFuente
    {
        public static bool TobiiDisponible
        {
            get { return true; }
        }

        public static IFuenteMirada Crear(bool simulado)
        {
            return simulado ? (IFuenteMirada)new FuenteSimulada() : new FuenteTobii();
        }
    }
}
