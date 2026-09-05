using System;
using System.Collections.Generic;
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

        public string DatosFormateados
        {
            get
            {
                if (Columnas == null || Datos == null) return string.Empty;

                var partes = new List<string>();
                int n = Math.Min(Columnas.Length, Datos.Length);
                for (int i = 0; i < n; i++)
                    partes.Add(Columnas[i] + "=" + Datos[i]);

                return string.Join(" | ", partes);
            }
        }
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
