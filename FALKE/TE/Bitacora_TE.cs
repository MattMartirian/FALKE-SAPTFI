using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}