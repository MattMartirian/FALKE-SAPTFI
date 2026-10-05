using System;
using System.IO;

namespace TE
{
    // Una copia de seguridad de la base de datos. El archivo queda en el servidor y no se descarga.
    public class Respaldo_TE
    {
        public int IdRespaldo { get; set; }

        // null = la hizo la cuenta de emergencia (no existe en UsuarioTable) o el usuario ya no existe.
        public int? IdUsuario { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string Ruta { get; set; }
        public long? Tamano { get; set; }
        public string DVH { get; set; }

        // Solo los llena el listado. No se guardan en la tabla.
        public string NombreUsuario { get; set; }
        public bool ArchivoDisponible { get; set; }

        public string NombreArchivo
        {
            get { return string.IsNullOrEmpty(Ruta) ? string.Empty : Path.GetFileName(Ruta); }
        }
    }
}
