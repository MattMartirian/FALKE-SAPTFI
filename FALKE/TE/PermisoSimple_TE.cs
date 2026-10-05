using System.Collections.Generic;

namespace TE
{
    // Leaf del Composite: una patente. Responde por sí misma y no contiene nada (Agregar y Quitar, heredados, lo rechazan).
    public class PermisoSimple_TE : PermisoAbstracto_TE
    {
        public override TipoPermiso TipoPermiso => TipoPermiso.Simple;

        public PermisoSimple_TE(string nombre) : base(nombre) { }

        internal override bool Contiene(string nombrePermiso, HashSet<PermisoAbstracto_TE> visitados)
        {
            return Nombre == nombrePermiso;
        }

        internal override void AcumularEfectivos(HashSet<string> resultado, HashSet<PermisoAbstracto_TE> visitados)
        {
            resultado.Add(Nombre);
        }

        internal override void AcumularPatentes(HashSet<string> resultado, HashSet<PermisoAbstracto_TE> visitados)
        {
            resultado.Add(Nombre);
        }
    }
}
