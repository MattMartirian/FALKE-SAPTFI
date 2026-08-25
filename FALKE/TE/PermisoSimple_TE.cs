using System.Collections.Generic;

namespace TE
{
    public class PermisoSimple_TE : PermisoAbstracto_TE
    {
        public override TipoPermiso TipoPermiso => TipoPermiso.Simple;

        public PermisoSimple_TE(string nombre) : base(nombre) { }

        public override HashSet<string> ObtenerPermisosEfectivos()
        {
            return new HashSet<string> { Nombre };
        }
    }
}