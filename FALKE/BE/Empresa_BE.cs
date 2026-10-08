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
        Bloqueada,
        Deshabilitada
    }

    public enum CicloFacturacion
    {
        Mensual,
        Anual
    }
}
