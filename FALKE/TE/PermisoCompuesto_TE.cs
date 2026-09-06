using System;
using System.Collections.Generic;

namespace TE
{
    // Composite
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

            //TODO: Traducir.
            if (hijo.Nombre == Nombre) throw new PermisoInvalidoException("Un permiso no puede incluirse a sí mismo.");

            //TODO: Traducir.
            if (EsRolPermiso && hijo is PermisoCompuesto_TE hijoCompuesto && hijoCompuesto.EsRolPermiso) throw new PermisoInvalidoException($"El rol \"{Nombre}\" no puede incluir a otro rol (\"{hijo.Nombre}\").");

            //TODO: Traducir.
            if (GeneraCiclo(hijo)) throw new PermisoInvalidoException($"Agregar \"{hijo.Nombre}\" a \"{Nombre}\" generaría un ciclo de composición.");

            hijos.Add(hijo);
        }

        public override void Quitar(PermisoAbstracto_TE hijo)
        {
            hijos.Remove(hijo);
        }

        public override HashSet<string> ObtenerPermisosEfectivos()
        {
            var efectivos = new HashSet<string> { Nombre };

            foreach (var hijo in hijos)
            {
                efectivos.UnionWith(hijo.ObtenerPermisosEfectivos());
            }

            return efectivos;
        }

        private bool GeneraCiclo(PermisoAbstracto_TE candidato)
        {
            return candidato.ObtenerPermisosEfectivos().Contains(Nombre);
        }

        public void AgregarHijoPersistido(PermisoAbstracto_TE hijo)
        {
            hijos.Add(hijo);
        }
    }
}