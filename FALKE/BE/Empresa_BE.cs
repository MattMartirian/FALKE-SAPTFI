using System;

namespace BE
{
    public class Empresa_BE
    {
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string NumContactoEmpresa { get; set; }
        public PlanSuscripcion PlanSuscripcion { get; set; }
        public DateTime FechaAlta { get; set; }
        public EstadoEmpresa Estado { get; set; }
        public string Cuit { get; set; }
        public string Rubro { get; set; }
        public string Domicilio { get; set; }
        public CicloFacturacion? Facturacion { get; set; }
        public DateTime? FechaRenovacion { get; set; }
        public string DVH { get; set; }

        // Solo los llena el listado; no se guardan en la tabla.
        public int CantidadUsuarios { get; set; }
        public int DispositivosPrestados { get; set; }
        public int SesionesGrabadas { get; set; }
    }

    public enum PlanSuscripcion
    {
        Scout,
        Hunter,
        Apex
    }

    public enum EstadoEmpresa
    {
        Activa,
        // Sigue siendo cliente pero no puede acceder (por ejemplo, falta de pago): se levanta pasando a Activa.
        Bloqueada,
        // Dada de baja: ya no es cliente. Solo puede volver a Activa si retoma el servicio.
        Deshabilitada
    }

    public enum CicloFacturacion
    {
        Mensual,
        Anual
    }
}
