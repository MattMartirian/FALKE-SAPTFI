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

    // Catálogo de modelos de eye tracker. No se borran: se dan de baja y dejan de ofrecerse para dispositivos nuevos.
    public class ModeloDispositivo_BE
    {
        public int IdModelo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public string DVH { get; set; }

        // Solo lo llena el listado: cuántos dispositivos del inventario son de este modelo. No se guarda en la tabla.
        public int CantidadDispositivos { get; set; }
    }

    public class Dispositivo_BE
    {
        public int IdDispositivo { get; set; }

        // Número de serie de fábrica (el que figura en la etiqueta del equipo y que informa el SDK): identifica al aparato y no se repite.
        public string NumeroSerie { get; set; }

        // El modelo se elige del catálogo: IdModelo es lo que se guarda; Modelo es el nombre, que llena el listado.
        public int IdModelo { get; set; }
        public string Modelo { get; set; }
        public string Firmware { get; set; }
        public string DriverRequerido { get; set; }
        public EstadoDispositivo Estado { get; set; }
        public string DVH { get; set; }

        // Solo los llena el listado: el préstamo abierto, si lo tiene. No se guardan en la tabla.
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
