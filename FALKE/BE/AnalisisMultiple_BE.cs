using System;
using System.Collections.Generic;

namespace BE
{
    public class AnalisisMultiple_BE
    {
        public int IdAnalisis { get; set; }
        public int IdEmpresa { get; set; }
        public int IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public List<int> IdSesiones { get; set; } = new List<int>();
    }
}
