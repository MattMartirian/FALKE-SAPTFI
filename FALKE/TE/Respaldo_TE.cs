using System;
using System.IO;

namespace TE
{
    public class Respaldo_TE
    {
        public int IdRespaldo { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string Ruta { get; set; }
        public long? Tamano { get; set; }
        public string DVH { get; set; }
        public string NombreUsuario { get; set; }
        public bool ArchivoDisponible { get; set; }

        public string NombreArchivo
        {
            get { return string.IsNullOrEmpty(Ruta) ? string.Empty : Path.GetFileName(Ruta); }
        }
    }
}
