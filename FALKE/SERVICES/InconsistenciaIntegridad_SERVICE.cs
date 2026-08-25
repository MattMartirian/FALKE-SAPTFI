using ORM;

namespace SERVICIOS
{
    public class InconsistenciaIntegridad_SERVICE
    {
        public TablasBD Tabla { get; set; }
        public TipoInconsistencia Tipo { get; set; }
        public string ClaveRegistro { get; set; }
        public string Detalle { get; set; }
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