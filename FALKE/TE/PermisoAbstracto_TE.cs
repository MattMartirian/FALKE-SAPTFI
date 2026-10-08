using System;
using System.Collections.Generic;

namespace TE
{
    public enum TipoPermiso
    {
        Simple,
        Compuesto
    }

    public abstract class PermisoAbstracto_TE
    {
        public string Nombre { get; }

        public string Descripcion { get; set; }

        public bool EsDeGestion { get; set; }

        public abstract TipoPermiso TipoPermiso { get; }

        public virtual bool EsRolPermiso => false;

        protected PermisoAbstracto_TE(string nombre)
        {
            Nombre = nombre;
        }


        public virtual void Agregar(PermisoAbstracto_TE hijo)
        {
            throw new PermisoInvalidoException($"\"{Nombre}\" es un permiso Simple, no puede contener otros permisos.");
        }

        public virtual void Quitar(PermisoAbstracto_TE hijo)
        {
            throw new PermisoInvalidoException($"\"{Nombre}\" es un permiso Simple, no tiene hijos.");
        }

        public virtual IReadOnlyList<PermisoAbstracto_TE> ObtenerHijos()
        {
            return Array.Empty<PermisoAbstracto_TE>();
        }


        public bool Contiene(string nombrePermiso)
        {
            return Contiene(nombrePermiso, new HashSet<PermisoAbstracto_TE>());
        }


        public HashSet<string> ObtenerPermisosEfectivos()
        {
            var resultado = new HashSet<string>();
            AcumularEfectivos(resultado, new HashSet<PermisoAbstracto_TE>());
            return resultado;
        }

        public HashSet<string> ObtenerPatentes()
        {
            var resultado = new HashSet<string>();
            AcumularPatentes(resultado, new HashSet<PermisoAbstracto_TE>());
            return resultado;
        }

       
        internal abstract bool Contiene(string nombrePermiso, HashSet<PermisoAbstracto_TE> visitados);
        internal abstract void AcumularEfectivos(HashSet<string> resultado, HashSet<PermisoAbstracto_TE> visitados);
        internal abstract void AcumularPatentes(HashSet<string> resultado, HashSet<PermisoAbstracto_TE> visitados);
    }

    public class PermisoInvalidoException : System.Exception
    {
        public PermisoInvalidoException(string mensaje) : base(mensaje) { }
    }
}
