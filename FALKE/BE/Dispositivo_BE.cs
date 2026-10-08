using System;

namespace BE
{
    public enum EstadoDispositivo
    {
        Disponible,
        EnUso,
        EnMantenimiento,
        DeBaja
    }

    public class ModeloDispositivo_BE
    {
        public int IdModelo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public string DVH { get; set; }

        public int CantidadDispositivos { get; set; }
    }

    public class Dispositivo_BE
    {
        public int IdDispositivo { get; set; }
        public string NumeroSerie { get; set; }
        public int IdModelo { get; set; }
        public string Modelo { get; set; }
        public string Firmware { get; set; }
        public string DriverRequerido { get; set; }
        public EstadoDispositivo Estado { get; set; }
        public string DVH { get; set; }
        public int? IdEmpresaActual { get; set; }
        public string EmpresaActual { get; set; }
        public DateTime? FechaEntregaActual { get; set; }
    }

    public class Prestamo_BE
    {
        public int IdPrestamo { get; set; }
        public int IdDispositivo { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public DateTime FechaEntrega { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string DVH { get; set; }

        public bool Abierto
        {
            get { return !FechaDevolucion.HasValue; }
        }
    }
}
