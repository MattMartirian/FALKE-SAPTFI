using System.Collections.Generic;
using ORM;
using TE;

namespace TLL
{
    //TODO Revisar si darle integridad a la tabla de permisos y relaciones.
    public class Permiso_TLL
    {
        private readonly PermisoRepository permisoRepo;
        private readonly UsuarioRepository usuarioRepo;
        private readonly BitacoraGestor_TLL bitacora;
        //private readonly GestorIntegridad gestorIntegridad;

        public Permiso_TLL()
        {
            permisoRepo = new PermisoRepository();
            usuarioRepo = new UsuarioRepository();
            bitacora = new BitacoraGestor_TLL();
            //gestorIntegridad = new GestorIntegridad();
        }

        public void CrearPermiso(string nombre, TipoPermiso tipo, bool esRol)
        {
            //TODO: Traducir.
            if (tipo == TipoPermiso.Simple && esRol) throw new PermisoInvalidoException("Un permiso simple no puede ser rol.");

            //TODO: Traducir.
            if (permisoRepo.Existe(nombre)) throw new PermisoInvalidoException("Ya existe un permiso con el nombre \"" + nombre + "\".");

            PermisoAbstracto_TE permiso = tipo == TipoPermiso.Simple ? (PermisoAbstracto_TE)new PermisoSimple_TE(nombre) : new PermisoCompuesto_TE(nombre, esRol);

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.Alta(permiso);
                //gestorIntegridad.ActualizarDVHRegistro(TablasBD.Permiso, new[] { nombre });

                //TODO: Traducir.
                bitacora.Registrar(0, "Permisos", "Alta de permiso \"" + nombre + "\" (" + tipo + (esRol ? ", rol" : "") + ")", CriticidadBitacora.Media);
            });
        }

        public void AgregarPermisoAComposicion(string nombreCompuesto, string nombreIncluido)
        {
            var arbol = ConstruirArbolCompleto();

            //TODO: Traducir.
            if (!arbol.TryGetValue(nombreCompuesto, out var compuestoNodo)) throw new PermisoInvalidoException("El permiso \"" + nombreCompuesto + "\" no existe.");

            //TODO: Traducir.
            if (!arbol.TryGetValue(nombreIncluido, out var incluidoNodo)) throw new PermisoInvalidoException("El permiso \"" + nombreIncluido + "\" no existe.");

            compuestoNodo.Agregar(incluidoNodo);

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.AgregarRelacion(nombreCompuesto, nombreIncluido);
                //gestorIntegridad.ActualizarDVHRegistro(TablasBD.RelacionPermisos, new[] { nombreCompuesto, nombreIncluido });

                //TODO: Traducir.
                bitacora.Registrar(0, "Permisos", "Se agregó \"" + nombreIncluido + "\" a la composición de \"" + nombreCompuesto + "\"", CriticidadBitacora.Alta);
            });
        }

        public void ModificarNombrePermiso(string nombreViejo, string nombreNuevo)
        {
            //TODO: Traducir.
            if (permisoRepo.ObtenerPorPK(nombreViejo) == null) throw new PermisoInvalidoException("El permiso \"" + nombreViejo + "\" no existe.");

            //TODO: Traducir.
            if (permisoRepo.Existe(nombreNuevo)) throw new PermisoInvalidoException("Ya existe un permiso con el nombre \"" + nombreNuevo + "\".");

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.ModificarNombre(nombreViejo, nombreNuevo);
                //gestorIntegridad.ActualizarDVHRegistro(TablasBD.Permiso, new[] { nombreNuevo });

                //TODO: Traducir.
                bitacora.Registrar(0, "Permisos", "Renombrado de permiso \"" + nombreViejo + "\" a \"" + nombreNuevo + "\"", CriticidadBitacora.Media);
            });
        }

        public void EliminarPermiso(string nombre)
        {
            //TODO: Traducir.
            if (permisoRepo.ObtenerPorPK(nombre) == null) throw new PermisoInvalidoException("El permiso \"" + nombre + "\" no existe.");

            //TODO: Traducir.
            if (permisoRepo.PermisoEnRelacion(nombre)) throw new PermisoInvalidoException("\"" + nombre + "\" está en uso dentro de una composición: quitalo de ahí antes de eliminarlo.");

            //TODO: Traducir.
            if (usuarioRepo.ExisteUsuarioConRol(nombre)) throw new PermisoInvalidoException("\"" + nombre + "\" está asignado a uno o más usuarios: no se puede eliminar.");

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.Eliminar(nombre);
                //gestorIntegridad.GuardarIntegridadTabla(TablasBD.Permiso);

                //TODO: Traducir.
                bitacora.Registrar(0, "Permisos", "Baja de permiso \"" + nombre + "\"", CriticidadBitacora.Alta);
            });
        }

        public void QuitarPermisoDeComposicion(string nombreCompuesto, string nombreIncluido)
        {
            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.EliminarRelacion(nombreCompuesto, nombreIncluido);
                //gestorIntegridad.GuardarIntegridadTabla(TablasBD.RelacionPermisos);

                //TODO: Traducir.
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