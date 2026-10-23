using System.Collections.Generic;

namespace TLL
{
    public interface ITraductorAutomatico_TLL
    {
        List<string> ObtenerCodigosDisponibles();
        bool Disponible(string codigoIdioma);
        Dictionary<string, string> ObtenerTraducciones(string codigoIdioma);
    }
}
