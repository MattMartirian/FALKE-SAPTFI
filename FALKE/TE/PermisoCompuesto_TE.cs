using System;
using System.Collections.Generic;
using System.Linq;

namespace TE
{
    public class PermisoCompuesto_TE : PermisoAbstracto_TE
    {
        public override TipoPermiso TipoPermiso => TipoPermiso.Compuesto;
        public override bool EsRolPermiso { get; }

        private readonly List<PermisoAbstracto_TE> hijos = new List<PermisoAbstracto_TE>();

        public PermisoCompuesto_TE(string nombre, bool esRolPermiso) : base(nombre)
        {
            EsRolPermiso = esRolPermiso;
        }

        public override IReadOnlyList<PermisoAbstracto_TE> ObtenerHijos() => hijos.AsReadOnly();

        public override void Agregar(PermisoAbstracto_TE hijo)
        {
            if (hijo == null) throw new ArgumentNullException(nameof(hijo));

            if (hijo.Nombre == Nombre) throw new PermisoInvalidoException("Un permiso no puede incluirse a sí mismo.");

            if (hijos.Any(h => h.Nombre == hijo.Nombre)) throw new PermisoInvalidoException($"\"{Nombre}\" ya incluye a \"{hijo.Nombre}\".");

            // Hay ciclo si el candidato ya contiene, directa o indirectamente, a este permiso.
            if (hijo.Contiene(Nombre)) throw new PermisoInvalidoException($"Agregar \"{hijo.Nombre}\" a \"{Nombre}\" generaría un ciclo de composición.");

            hijos.Add(hijo);
        }

        public override void Quitar(PermisoAbstracto_TE hijo)
        {
            if (hijo == null) throw new ArgumentNullException(nameof(hijo));

            PermisoAbstracto_TE actual = hijos.FirstOrDefault(h => h.Nombre == hijo.Nombre);

            if (actual == null) throw new PermisoInvalidoException($"\"{Nombre}\" no incluye a \"{hijo.Nombre}\".");

            hijos.Remove(actual);
        }

        internal override bool Contiene(string nombrePermiso, HashSet<PermisoAbstracto_TE> visitados)
        {
            if (Nombre == nombrePermiso) return true;

            if (!visitados.Add(this)) return false;

            foreach (PermisoAbstracto_TE hijo in hijos)
            {
                if (hijo.Contiene(nombrePermiso, visitados)) return true;
            }

            return false;
        }

        internal override void AcumularEfectivos(HashSet<string> resultado, HashSet<PermisoAbstracto_TE> visitados)
        {
            if (!visitados.Add(this)) return;

            resultado.Add(Nombre);

            foreach (PermisoAbstracto_TE hijo in hijos) hijo.AcumularEfectivos(resultado, visitados);
        }

        internal override void AcumularPatentes(HashSet<string> resultado, HashSet<PermisoAbstracto_TE> visitados)
        {
            if (!visitados.Add(this)) return;

            foreach (PermisoAbstracto_TE hijo in hijos) hijo.AcumularPatentes(resultado, visitados);
        }
    }
}
