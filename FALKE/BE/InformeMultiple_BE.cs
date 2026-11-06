using System.Collections.Generic;

namespace BE
{
    public class InformeMultiple_BE
    {
        public Empresa_BE Empresa { get; set; }
        public Categoria_BE Categoria { get; set; }
        public List<SesionGrabada_BE> Sesiones { get; set; }
    }
}
