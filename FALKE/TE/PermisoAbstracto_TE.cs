using System;
using System.Collections.Generic;

namespace TE
{
    public enum TipoPermiso
    {
        Simple,
        Compuesto
    }

    // Patrón Composite (GoF) aplicado a los permisos.
    //   Component  -> PermisoAbstracto_TE: declara las operaciones comunes y la administración de hijos (variante "transparente").
    //   Leaf       -> PermisoSimple_TE: una patente. No tiene hijos.
    //   Composite  -> PermisoCompuesto_TE: un rol o un grupo. Guarda hijos y delega en ellos cada operación.
    // Cada operación (Contiene, ObtenerPatentes, ObtenerPermisosEfectivos) se pide igual a una hoja o a un compuesto: la hoja responde por
    // sí misma y el compuesto combina las respuestas de sus hijos. Un grupo puede estar en varios roles, así que la estructura es un grafo
    // sin ciclos y no un árbol estricto; por eso las operaciones recuerdan los nodos ya visitados (no repiten un grupo compartido).
    public abstract class PermisoAbstracto_TE
    {
        public string Nombre { get; }

        // Lo que se muestra en pantalla. Si está vacío se muestra el nombre, que es interno y no se cambia.
        public string Descripcion { get; set; }

        // Solo para roles: un rol "de gestión" es del personal de Pattern Blue y se asigna únicamente a sus usuarios.
        // Los demás son roles generales y pueden asignarse a usuarios de cualquier empresa.
        public bool EsDeGestion { get; set; }

        public abstract TipoPermiso TipoPermiso { get; }

        public virtual bool EsRolPermiso => false;

        protected PermisoAbstracto_TE(string nombre)
        {
            Nombre = nombre;
        }

        // ---- Administración de hijos. La hoja los rechaza; el compuesto los implementa.

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

        // ---- Operaciones de la composición.

        // ¿Este permiso es, o contiene en cualquier nivel, el permiso con ese nombre?
        public bool Contiene(string nombrePermiso)
        {
            return Contiene(nombrePermiso, new HashSet<PermisoAbstracto_TE>());
        }

        // Los nombres de este permiso y de todo lo que contiene (también los de los grupos intermedios).
        public HashSet<string> ObtenerPermisosEfectivos()
        {
            var resultado = new HashSet<string>();
            AcumularEfectivos(resultado, new HashSet<PermisoAbstracto_TE>());
            return resultado;
        }

        // Solo las patentes (hojas) que da este permiso, sin los nombres de los grupos intermedios.
        public HashSet<string> ObtenerPatentes()
        {
            var resultado = new HashSet<string>();
            AcumularPatentes(resultado, new HashSet<PermisoAbstracto_TE>());
            return resultado;
        }

        // Cada una la implementa la hoja (responde por sí misma) y el compuesto (la delega en sus hijos).
        internal abstract bool Contiene(string nombrePermiso, HashSet<PermisoAbstracto_TE> visitados);
        internal abstract void AcumularEfectivos(HashSet<string> resultado, HashSet<PermisoAbstracto_TE> visitados);
        internal abstract void AcumularPatentes(HashSet<string> resultado, HashSet<PermisoAbstracto_TE> visitados);
    }

    public class PermisoInvalidoException : System.Exception
    {
        public PermisoInvalidoException(string mensaje) : base(mensaje) { }
    }
}
