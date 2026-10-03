using System.Collections.Generic;
using ORM;
using TE;

namespace TLL
{
    public class Permiso_TLL
    {
        private readonly PermisoRepository permisoRepo;
        private readonly UsuarioRepository usuarioRepo;
        private readonly BitacoraGestor_TLL bitacora;

        public Permiso_TLL()
        {
            permisoRepo = new PermisoRepository();
            usuarioRepo = new UsuarioRepository();
            bitacora = new BitacoraGestor_TLL();
        }

        public void CrearPermiso(string nombre, TipoPermiso tipo, bool esRol)
        {
            if (tipo == TipoPermiso.Simple && esRol) throw new PermisoInvalidoException("Un permiso simple no puede ser rol.");

            if (permisoRepo.Existe(nombre)) throw new PermisoInvalidoException("Ya existe un permiso con el nombre \"" + nombre + "\".");

            PermisoAbstracto_TE permiso = tipo == TipoPermiso.Simple ? (PermisoAbstracto_TE)new PermisoSimple_TE(nombre) : new PermisoCompuesto_TE(nombre, esRol);

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.Alta(permiso);

                bitacora.Registrar(0, "Permisos", "Alta de permiso \"" + nombre + "\" (" + tipo + (esRol ? ", rol" : "") + ")", CriticidadBitacora.Media);
            });
        }

        public void AgregarPermisoAComposicion(string nombreCompuesto, string nombreIncluido)
        {
            var arbol = ConstruirArbolCompleto();

            if (!arbol.TryGetValue(nombreCompuesto, out var compuestoNodo)) throw new PermisoInvalidoException("El permiso \"" + nombreCompuesto + "\" no existe.");

            if (!arbol.TryGetValue(nombreIncluido, out var incluidoNodo)) throw new PermisoInvalidoException("El permiso \"" + nombreIncluido + "\" no existe.");

            compuestoNodo.Agregar(incluidoNodo);

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.AgregarRelacion(nombreCompuesto, nombreIncluido);

                bitacora.Registrar(0, "Permisos", "Se agregó \"" + nombreIncluido + "\" a la composición de \"" + nombreCompuesto + "\"", CriticidadBitacora.Alta);
            });
        }

        public void ModificarNombrePermiso(string nombreViejo, string nombreNuevo)
        {
            if (permisoRepo.ObtenerPorPK(nombreViejo) == null) throw new PermisoInvalidoException("El permiso \"" + nombreViejo + "\" no existe.");

            if (permisoRepo.Existe(nombreNuevo)) throw new PermisoInvalidoException("Ya existe un permiso con el nombre \"" + nombreNuevo + "\".");

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.ModificarNombre(nombreViejo, nombreNuevo);

                bitacora.Registrar(0, "Permisos", "Renombrado de permiso \"" + nombreViejo + "\" a \"" + nombreNuevo + "\"", CriticidadBitacora.Media);
            });
        }

        public void EliminarPermiso(string nombre)
        {
            if (permisoRepo.ObtenerPorPK(nombre) == null) throw new PermisoInvalidoException("El permiso \"" + nombre + "\" no existe.");

            if (permisoRepo.PermisoEnRelacion(nombre)) throw new PermisoInvalidoException("\"" + nombre + "\" está en uso dentro de una composición: quitalo de ahí antes de eliminarlo.");

            if (usuarioRepo.ExisteUsuarioConRol(nombre)) throw new PermisoInvalidoException("\"" + nombre + "\" está asignado a uno o más usuarios: no se puede eliminar.");

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.Eliminar(nombre);

                bitacora.Registrar(0, "Permisos", "Baja de permiso \"" + nombre + "\"", CriticidadBitacora.Alta);
            });
        }

        public void QuitarPermisoDeComposicion(string nombreCompuesto, string nombreIncluido)
        {
            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.EliminarRelacion(nombreCompuesto, nombreIncluido);

                bitacora.Registrar(0, "Permisos", "Se quitó \"" + nombreIncluido + "\" de la composición de \"" + nombreCompuesto + "\"", CriticidadBitacora.Alta);
            });
        }

        public PermisoAbstracto_TE ObtenerPermiso(string nombre)
        {
            var arbol = ConstruirArbolCompleto();

            return arbol.TryGetValue(nombre, out var nodo) ? nodo : null;
        }

        public List<PermisoAbstracto_TE> ObtenerTodos()
        {
            return permisoRepo.ObtenerTodos();
        }

        public List<PermisoAbstracto_TE> ObtenerRoles()
        {
            return permisoRepo.ConstruirArbolDeRoles();
        }

        public static bool ComprobarPermiso(string permisoBuscado, PermisoAbstracto_TE permisoActual)
        {
            if (string.IsNullOrEmpty(permisoBuscado)) return false;

            if (permisoActual == null) return false;

            return permisoActual.Contiene(permisoBuscado);
        }

        private Dictionary<string, PermisoAbstracto_TE> ConstruirArbolCompleto()
        {
            return permisoRepo.ConstruirArbol();
        }
    }
}
