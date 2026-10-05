using ORM;

namespace SERVICES
{
    public class InconsistenciaIntegridad_SERVICE
    {
        public TablasBD Tabla { get; set; }
        public TipoInconsistencia Tipo { get; set; }
        public string ClaveRegistro { get; set; }
        public string Detalle { get; set; }
        public int? NumeroRegistro { get; set; }
        public string[] Columnas { get; set; }
        public string[] Datos { get; set; }
        public string DvhAlmacenado { get; set; }
        public string DvhRecalculado { get; set; }

    }

    public enum TipoInconsistencia
    {
        RegistrosAgregados,
        RegistrosEliminados,
        RegistroAlterado,
        FirmaTablaInvalida,
        ErrorLectura
    }
}
