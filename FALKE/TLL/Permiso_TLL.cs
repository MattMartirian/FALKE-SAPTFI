using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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


        public CatalogoPermisos_TE ObtenerCatalogo(ActorUsuario_TE actor)
        {
            ExigirGestionar(actor);

            var arbol = permisoRepo.ConstruirArbol();
            var padres = IndexarPadres(arbol.Values);
            var usuarios = usuarioRepo.ContarUsuariosPorRol();
            var descripciones = arbol.Values.Where(p => !string.IsNullOrWhiteSpace(p.Descripcion)).ToDictionary(p => p.Nombre, p => p.Descripcion);

            var catalogo = new CatalogoPermisos_TE();

            foreach (var permiso in arbol.Values)
            {
                var vista = new PermisoVista_TE
                {
                    Nombre = permiso.Nombre,
                    Descripcion = permiso.Descripcion,
                    Etiqueta = Etiqueta(descripciones, permiso.Nombre),
                    Clase = permiso.TipoPermiso == TipoPermiso.Simple ? ClasePermiso.Patente : permiso.EsRolPermiso ? ClasePermiso.Rol : ClasePermiso.Grupo,
                    EsBase = EsRolBase(permiso.Nombre),
                    EsFijo = EsRolFijo(permiso.Nombre) && !actor.EsEmergencia,
                    EsDeGestion = permiso.EsRolPermiso && permiso.EsDeGestion,
                    Incluye = permiso.ObtenerHijos().Select(h => h.Nombre).OrderBy(n => n).ToList(),
                    IncluidoEn = padres.ContainsKey(permiso.Nombre) ? padres[permiso.Nombre].OrderBy(n => n).ToList() : new List<string>(),
                    PatentesEfectivas = permiso.ObtenerPatentes().OrderBy(p => Etiqueta(descripciones, p)).ToList()
                };

                int cantidad;
                vista.UsuariosAsignados = usuarios.TryGetValue(permiso.Nombre, out cantidad) ? cantidad : 0;

                switch (vista.Clase)
                {
                    case ClasePermiso.Rol: catalogo.Roles.Add(vista); break;
                    case ClasePermiso.Grupo: catalogo.Grupos.Add(vista); break;
                    default: catalogo.Patentes.Add(vista); break;
                }
            }

            catalogo.Roles = catalogo.Roles.OrderByDescending(r => r.EsBase).ThenBy(r => r.Etiqueta).ToList();
            catalogo.Grupos = catalogo.Grupos.OrderBy(g => g.Etiqueta).ToList();
            catalogo.Patentes = catalogo.Patentes.OrderBy(p => p.Etiqueta).ToList();

            return catalogo;
        }
        public void CrearRol(ActorUsuario_TE actor, string nombre, string descripcion = null, bool deGestion = false)
        {
            Crear(actor, nombre, true, descripcion, deGestion);
        }

        public void CrearGrupo(ActorUsuario_TE actor, string nombre, string descripcion = null)
        {
            Crear(actor, nombre, false, descripcion, false);
        }
        public Dictionary<string, string> ObtenerEtiquetas()
        {
            var descripciones = permisoRepo.ObtenerDescripciones();

            return permisoRepo.ObtenerTodos().ToDictionary(p => p.Nombre, p => Etiqueta(descripciones, p.Nombre));
        }

        public static string Etiqueta(Dictionary<string, string> descripciones, string nombre)
        {
            string texto;

            return nombre != null && descripciones.TryGetValue(nombre, out texto) && !string.IsNullOrWhiteSpace(texto) ? texto : nombre;
        }
        public void CambiarDescripcion(ActorUsuario_TE actor, string nombre, string descripcion)
        {
            if (actor == null || !actor.Puede(Patentes_TLL.CAMBIAR_DESCRIPCION_PERMISO))
            {
                bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", EventosBitacora_TLL.ACCION_RECHAZADA_PERMISO, CriticidadBitacora.Media, null, DetalleBitacora_TLL.Armar(Patentes_TLL.CAMBIAR_DESCRIPCION_PERMISO));
                throw new ExcepcionNoAutorizada_TLL("permiso_sin_permiso_descripciones");
            }

            PermisoAbstracto_TE permiso = permisoRepo.ObtenerPorPK(nombre ?? string.Empty);
            if (permiso == null) throw new ExcepcionTraducible_TLL("permiso_no_existe", nombre);

            string nueva = ValidarDescripcion(descripcion);
            string anterior = string.IsNullOrWhiteSpace(permiso.Descripcion) ? null : permiso.Descripcion;

            if (string.Equals(anterior ?? string.Empty, nueva ?? string.Empty, StringComparison.Ordinal))
                throw new ExcepcionTraducible_TLL("comun_sin_cambios");

            Transaccion_ORM.Ejecutar(() =>
            {
                permiso.Descripcion = nueva;
                permisoRepo.Modificar(permiso);

                bitacora.Auditar(actor, "Roles", null, EventosBitacora_TLL.ROL_DESCRIPCION, CriticidadBitacora.Media,
                    DetalleBitacora_TLL.Armar(permiso.Nombre));
            });
        }

        private static string ValidarDescripcion(string descripcion)
        {
            descripcion = (descripcion ?? string.Empty).Trim();

            if (descripcion.Length == 0) return null;

            if (descripcion.Length > 200) throw new ExcepcionTraducible_TLL("permiso_nombre_largo");

            if (descripcion.Any(char.IsControl)) throw new ExcepcionTraducible_TLL("permiso_nombre_control");

            return descripcion;
        }

        public void GuardarComposicion(ActorUsuario_TE actor, string nombre, IEnumerable<string> incluidos)
        {
            ExigirGestionar(actor);

            var nuevos = new HashSet<string>((incluidos ?? new string[0]).Select(x => (x ?? string.Empty).Trim()).Where(x => x.Length > 0));

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.BloquearComposicion();

                var arbol = permisoRepo.ConstruirArbol();

                PermisoAbstracto_TE nodo;
                PermisoCompuesto_TE objetivo = arbol.TryGetValue(nombre ?? string.Empty, out nodo) ? nodo as PermisoCompuesto_TE : null;

                if (objetivo == null) throw new ExcepcionTraducible_TLL("permiso_solo_roles_grupos");

                if (EsRolFijo(nombre) && !actor.EsEmergencia) throw new ExcepcionTraducible_TLL("permiso_rol_fijo", nombre);

                foreach (string inc in nuevos)
                {
                    if (!arbol.ContainsKey(inc)) throw new ExcepcionTraducible_TLL("permiso_incluido_no_existe", inc);

                    if (inc != nombre && arbol[inc].EsRolPermiso) throw new ExcepcionTraducible_TLL("permiso_es_un_rol", inc);
                }

                var actuales = new HashSet<string>(objetivo.ObtenerHijos().Select(h => h.Nombre));
                var aQuitar = actuales.Except(nuevos).OrderBy(x => x).ToList();
                var aAgregar = nuevos.Except(actuales).OrderBy(x => x).ToList();

                if (aQuitar.Count == 0 && aAgregar.Count == 0) throw new ExcepcionTraducible_TLL("comun_sin_cambios");

                var roles = arbol.Values.Where(p => p.EsRolPermiso).ToList();
                var patentesDeRolesAntes = roles.ToDictionary(r => r.Nombre, r => r.ObtenerPatentes());

                foreach (string q in aQuitar) objetivo.Quitar(arbol[q]);
                foreach (string a in aAgregar) objetivo.Agregar(arbol[a]);

                foreach (var rol in roles.Where(r => !r.EsDeGestion))
                {
                    var nuevosReservados = rol.ObtenerPatentes().Except(patentesDeRolesAntes[rol.Nombre]).Where(Patentes_TLL.EsDeProveedor).OrderBy(x => x).ToList();

                    if (nuevosReservados.Count > 0)
                    {
                        var etiquetas = permisoRepo.ObtenerDescripciones();

                        throw new ExcepcionTraducible_TLL("permiso_rol_general_reservados", Etiqueta(etiquetas, rol.Nombre), string.Join(", ", nuevosReservados.Select(p => Etiqueta(etiquetas, p))));
                    }
                }

                permisoRepo.Modificar(objetivo);

                bitacora.Auditar(actor, "Roles", null, EventosBitacora_TLL.ROL_COMPOSICION, CriticidadBitacora.Alta,
                    DetalleBitacora_TLL.Armar(nombre));
            });
        }

        public void EliminarRolOGrupo(ActorUsuario_TE actor, string nombre)
        {
            ExigirGestionar(actor);

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.BloquearComposicion();

                var arbol = permisoRepo.ConstruirArbol();

                PermisoAbstracto_TE permiso;
                if (!arbol.TryGetValue(nombre ?? string.Empty, out permiso)) throw new ExcepcionTraducible_TLL("permiso_no_existe", nombre);

                ExigirEditable(permiso);

                if (permiso.EsRolPermiso && usuarioRepo.ExisteUsuarioConRol(nombre))
                    throw new ExcepcionTraducible_TLL("permiso_rol_asignado", nombre);

                var padres = IndexarPadres(arbol.Values);
                var enUso = padres.ContainsKey(nombre) ? padres[nombre].OrderBy(x => x).ToList() : new List<string>();

                if (enUso.Count > 0) throw new ExcepcionTraducible_TLL("permiso_incluido_en", nombre, string.Join(", ", enUso));

                var compuesto = (PermisoCompuesto_TE)permiso;
                foreach (PermisoAbstracto_TE hijo in compuesto.ObtenerHijos().ToList()) compuesto.Quitar(hijo);

                permisoRepo.Modificar(compuesto);
                permisoRepo.Eliminar(nombre);

                bitacora.Auditar(actor, "Roles", null, EventosBitacora_TLL.ROL_BAJA, CriticidadBitacora.Alta,
                    DetalleBitacora_TLL.Armar(nombre));
            });
        }

        // Las patentes que da un rol o un grupo (sin los nombres de los grupos intermedios). Sin permiso, ninguna.
        public static HashSet<string> ObtenerPatentes(PermisoAbstracto_TE permiso)
        {
            return permiso == null ? new HashSet<string>() : permiso.ObtenerPatentes();
        }

        private void Crear(ActorUsuario_TE actor, string nombre, bool esRol, string descripcion, bool deGestion)
        {
            ExigirGestionar(actor);

            nombre = ValidarNombre(nombre);
            descripcion = ValidarDescripcion(descripcion);

            if (permisoRepo.Existe(nombre)) throw new ExcepcionTraducible_TLL("permiso_nombre_duplicado", nombre);

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.Alta(new PermisoCompuesto_TE(nombre, esRol) { Descripcion = descripcion, EsDeGestion = esRol && deGestion });

                bitacora.Auditar(actor, "Roles", null, EventosBitacora_TLL.ROL_ALTA, CriticidadBitacora.Media,
                    DetalleBitacora_TLL.Armar(nombre));
            });
        }

        private static string ValidarNombre(string nombre)
        {
            nombre = (nombre ?? string.Empty).Trim();

            if (!Regex.IsMatch(nombre, @"^[\p{L}\p{N}][\p{L}\p{N} _\.\-]{1,58}[\p{L}\p{N}]$"))
                throw new ExcepcionTraducible_TLL("permiso_nombre_formato");

            return nombre;
        }

        private static bool EsRolBase(string nombre)
        {
            return nombre == Usuario_TLL.ROL_GESTOR || nombre == Usuario_TLL.ROL_WEBMASTER || nombre == Usuario_TLL.ROL_ADMINISTRADOR || nombre == Usuario_TLL.ROL_ANALISTA;
        }

        // Gestor y Webmaster son fijos: su composición no se edita, para que Pattern Blue nunca quede sin acceso a lo que cada uno cuida.
        // Solo la cuenta de emergencia puede cambiarla (es la que queda para arreglar un error de configuración).
        private static bool EsRolFijo(string nombre)
        {
            return nombre == Usuario_TLL.ROL_GESTOR || nombre == Usuario_TLL.ROL_WEBMASTER;
        }

        private static void ExigirEditable(PermisoAbstracto_TE permiso)
        {
            if (permiso.TipoPermiso == TipoPermiso.Simple)
                throw new ExcepcionTraducible_TLL("permiso_del_sistema_eliminar", permiso.Nombre);

            if (EsRolBase(permiso.Nombre))
                throw new ExcepcionTraducible_TLL("permiso_rol_base_eliminar", permiso.Nombre);
        }

        private void ExigirGestionar(ActorUsuario_TE actor)
        {
            if (actor != null && actor.Puede(Patentes_TLL.GESTIONAR_ROLES)) return;

            bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", EventosBitacora_TLL.ACCION_RECHAZADA_PERMISO, CriticidadBitacora.Media, null, DetalleBitacora_TLL.Armar(Patentes_TLL.GESTIONAR_ROLES));
            throw new ExcepcionNoAutorizada_TLL("permiso_sin_permiso_gestionar");
        }

        // Para cada permiso, los roles y grupos que lo incluyen directamente.
        private static Dictionary<string, List<string>> IndexarPadres(IEnumerable<PermisoAbstracto_TE> permisos)
        {
            var indice = new Dictionary<string, List<string>>();

            foreach (PermisoAbstracto_TE padre in permisos)
            {
                foreach (PermisoAbstracto_TE hijo in padre.ObtenerHijos())
                {
                    List<string> lista;
                    if (!indice.TryGetValue(hijo.Nombre, out lista)) indice[hijo.Nombre] = lista = new List<string>();
                    lista.Add(padre.Nombre);
                }
            }

            return indice;
        }

        public static bool ComprobarPermiso(string permisoBuscado, PermisoAbstracto_TE permisoActual)
        {
            if (string.IsNullOrEmpty(permisoBuscado)) return false;

            if (permisoActual == null) return false;

            return permisoActual.Contiene(permisoBuscado);
        }

    }
}
