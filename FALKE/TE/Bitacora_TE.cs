using System;
using System.Collections.Generic;

namespace TE
{
    public enum CriticidadBitacora
    {
        Baja = 1,
        Media = 2,
        Alta = 3
    }

    public class Bitacora_TE
    {
        public int IdEventoBitacora { get; set; }
        public int IdUsuario { get; set; }
        public int? IdEmpresa { get; set; }
        public string ModuloBitacora { get; set; }
        public string DescripcionBitacora { get; set; }
        public CriticidadBitacora CriticidadBitacora { get; set; }
        public DateTime FechaHoraBitacora { get; set; }

        public Bitacora_TE() { }

        public Bitacora_TE(int idUsuario, string moduloBitacora, string descripcionBitacora, CriticidadBitacora criticidadBitacora, DateTime fechaHoraBitacora, int idEventoBitacora = 0)
        {
            IdEventoBitacora = idEventoBitacora;
            IdUsuario = idUsuario;
            ModuloBitacora = moduloBitacora;
            DescripcionBitacora = descripcionBitacora;
            CriticidadBitacora = criticidadBitacora;
            FechaHoraBitacora = fechaHoraBitacora;
        }
    }

    public class BitacoraVista_TE
    {
        public int IdEventoBitacora { get; set; }
        public DateTime FechaHoraBitacora { get; set; }
        public string Actor { get; set; }

        public string EmailActor { get; set; }
        public int? IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string ModuloBitacora { get; set; }
        public string DescripcionBitacora { get; set; }
        public CriticidadBitacora CriticidadBitacora { get; set; }
    }

    public class FiltroBitacora_TE
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public TimeSpan? HoraDesde { get; set; }
        public TimeSpan? HoraHasta { get; set; }

        public string Modulo { get; set; }
        public string Accion { get; set; }
        public string Usuario { get; set; }
        public int? IdEmpresa { get; set; }
        public CriticidadBitacora? Criticidad { get; set; }

        public int Pagina { get; set; } = 1;
        public int Tamano { get; set; } = 50;
    }

    public class PaginaBitacora_TE
    {
        public List<BitacoraVista_TE> Items { get; set; } = new List<BitacoraVista_TE>();
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int Tamano { get; set; }

        public int TotalPaginas
        {
            get { return Tamano <= 0 ? 0 : (Total + Tamano - 1) / Tamano; }
        }
    }
}
