using System.Collections.Generic;

namespace SERVICES
{
    public class ResultadoRecalculoIntegridad_SERVICE
    {
        public int TablasProcesadas { get; set; }
        public int RegistrosProcesados { get; set; }
        public List<DetalleTablaRecalculo_SERVICE> Tablas { get; set; }

        public ResultadoRecalculoIntegridad_SERVICE()
        {
            Tablas = new List<DetalleTablaRecalculo_SERVICE>();
        }
    }

    public class DetalleTablaRecalculo_SERVICE
    {
        public string Tabla { get; set; }
        public int Registros { get; set; }
        public string Dvv { get; set; }
    }
}
