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

        public abstract TipoPermiso TipoPermiso { get; }

        public virtual bool EsRolPermiso => false;

        protected PermisoAbstracto_TE(string nombre)
        {
            Nombre = nombre;
        }

        public abstract HashSet<string> ObtenerPermisosEfectivos();

        /// <summary>
        /// Recorre el arbol (Composite): true si este permiso es el buscado o si alguno
        /// de sus hijos, en cualquier nivel, lo contiene. En una hoja se reduce a comparar
        /// el nombre. Sirve para cualquier chequeo de "tengo tal permiso".
        /// </summary>
        public bool Contiene(string nombrePermiso)
        {
            if (Nombre == nombrePermiso) return true;

            foreach (var hijo in ObtenerHijos())
            {
                if (hijo.Contiene(nombrePermiso)) return true;
            }

            return false;
        }

        public virtual void Agregar(PermisoAbstracto_TE hijo)
        {
            //TODO: Traducir.
            throw new PermisoInvalidoException($"\"{Nombre}\" es un permiso Simple, no puede contener otros permisos.");
        }

        public virtual void Quitar(PermisoAbstracto_TE hijo)
        {
            //TODO: Traducir.
            throw new PermisoInvalidoException($"\"{Nombre}\" es un permiso Simple, no tiene hijos.");
        }

        public virtual IReadOnlyList<PermisoAbstracto_TE> ObtenerHijos()
        {
            return Array.Empty<PermisoAbstracto_TE>();
        }
    }

    public class PermisoInvalidoException : System.Exception
    {
        public PermisoInvalidoException(string mensaje) : base(mensaje) { }
    }
}