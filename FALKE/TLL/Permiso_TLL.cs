using System.Collections.Generic;
using ORM;
using SERVICIOS;
using TE;

namespace TLL
{
    //TODO Revisar si darle integridad a la tabla de permisos y relaciones.
    public class PermisoTLL
    {
        private readonly PermisoRepository permisoRepo;
        private readonly UsuarioRepository usuarioRepo;
        //private readonly GestorIntegridad gestorIntegridad;

        public PermisoTLL()
        {
            permisoRepo = new PermisoRepository();
            usuarioRepo = new UsuarioRepository();
            //gestorIntegridad = new GestorIntegridad();
        }

        public void CrearPermiso(string nombre, TipoPermiso tipo, bool esRol)
        {
            if (tipo == TipoPermiso.Simple && esRol) throw new PermisoInvalidoException("Un permiso simple no puede ser rol.");

            if (permisoRepo.Existe(nombre)) throw new PermisoInvalidoException("Ya existe un permiso con el nombre \"" + nombre + "\".");

            PermisoAbstracto_TE permiso = tipo == TipoPermiso.Simple
                ? (PermisoAbstracto_TE)new PermisoSimple_TE(nombre)
                : new PermisoCompuesto_TE(nombre, esRol);

            permisoRepo.Alta(permiso);
            //gestorIntegridad.ActualizarDVHRegistro(TablasBD.Permiso, new[] { nombre });
        }

        public void AgregarPermisoAComposicion(string nombreCompuesto, string nombreIncluido)
        {
            var arbol = ConstruirArbolCompleto();

            if (!arbol.TryGetValue(nombreCompuesto, out var compuestoNodo))
                throw new PermisoInvalidoException("El permiso \"" + nombreCompuesto + "\" no existe.");

            if (!arbol.TryGetValue(nombreIncluido, out var incluidoNodo))
                throw new PermisoInvalidoException("El permiso \"" + nombreIncluido + "\" no existe.");

            compuestoNodo.Agregar(incluidoNodo);

            permisoRepo.AgregarRelacion(nombreCompuesto, nombreIncluido);
            //gestorIntegridad.ActualizarDVHRegistro(TablasBD.RelacionPermisos, new[] { nombreCompuesto, nombreIncluido });
        }

        public void ModificarNombrePermiso(string nombreViejo, string nombreNuevo)
        {
            if (permisoRepo.ObtenerPorPK(nombreViejo) == null)
                throw new PermisoInvalidoException("El permiso \"" + nombreViejo + "\" no existe.");

            if (permisoRepo.Existe(nombreNuevo))
                throw new PermisoInvalidoException("Ya existe un permiso con el nombre \"" + nombreNuevo + "\".");

            permisoRepo.ModificarNombre(nombreViejo, nombreNuevo);
            //gestorIntegridad.ActualizarDVHRegistro(TablasBD.Permiso, new[] { nombreNuevo });
        }

        public void EliminarPermiso(string nombre)
        {
            if (permisoRepo.ObtenerPorPK(nombre) == null) throw new PermisoInvalidoException("El permiso \"" + nombre + "\" no existe.");

            if (permisoRepo.PermisoEnRelacion(nombre)) throw new PermisoInvalidoException("\"" + nombre + "\" está en uso dentro de una composición: quitalo de ahí antes de eliminarlo.");

            if (usuarioRepo.ExisteUsuarioConRol(nombre)) throw new PermisoInvalidoException("\"" + nombre + "\" está asignado a uno o más usuarios: no se puede eliminar.");

            permisoRepo.Eliminar(nombre);
            //gestorIntegridad.GuardarIntegridadTabla(TablasBD.Permiso);
        }

        public void QuitarPermisoDeComposicion(string nombreCompuesto, string nombreIncluido)
        {
            permisoRepo.EliminarRelacion(nombreCompuesto, nombreIncluido);
            //gestorIntegridad.GuardarIntegridadTabla(TablasBD.RelacionPermisos);
        }

        public PermisoAbstracto_TE ObtenerPermiso(string nombre)
        {
            return permisoRepo.ObtenerPorPK(nombre);
        }

        public List<PermisoAbstracto_TE> ObtenerTodos()
        {
            return permisoRepo.ObtenerTodos();
        }
            
        public HashSet<string> ObtenerPermisosEfectivos(string nombrePermiso)
        {
            var arbol = ConstruirArbolCompleto();

            return arbol.TryGetValue(nombrePermiso, out var nodo)
                ? nodo.ObtenerPermisosEfectivos()
                : new HashSet<string>();
        }

        private Dictionary<string, PermisoAbstracto_TE> ConstruirArbolCompleto()
        {
            var permisos = permisoRepo.ObtenerTodos();
            var relaciones = permisoRepo.ObtenerTodasLasRelaciones();
            var nodos = new Dictionary<string, PermisoAbstracto_TE>();

            foreach (var p in permisos)
                nodos[p.Nombre] = p;

            foreach (var (nombreCompuesto, nombreIncluido) in relaciones)
            {
                if (!nodos.TryGetValue(nombreCompuesto, out var padreNodo) || !(padreNodo is PermisoCompuesto_TE padre))
                    continue; // TODO: dato inconsistente en la base (ver integridad), se ignora por ahora

                if (!nodos.TryGetValue(nombreIncluido, out var hijoNodo))
                    continue;

                padre.AgregarHijoPersistido(hijoNodo);
            }

            return nodos;
        }
    }
}