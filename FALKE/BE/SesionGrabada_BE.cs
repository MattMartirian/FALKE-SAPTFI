using System;

namespace BE
{
    public class SesionGrabada_BE
    {
        public const string ESTADO_FINALIZADA = "Finalizada";

        public int IdSesion { get; set; }
        public int IdCategoria { get; set; }
        public int IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }

        public string Nombre { get; set; }
        public string Objetivo { get; set; }

        public string RangoEdad { get; set; }
        public string Genero { get; set; }
        public string Familiaridad { get; set; }

        public int DuracionSegundos { get; set; }
        public int Muestras { get; set; }
        public long BytesVideo { get; set; }
        public int DesfaseMiradaMs { get; set; }

        public string DVH { get; set; }

        // Solo de lectura (vienen de los JOIN del listado, no son columnas de SesionGrabadaTable)
        public string NombreCategoria { get; set; }
        public string TipoActivo { get; set; }
        public string NombreAnalista { get; set; }
    }
}
