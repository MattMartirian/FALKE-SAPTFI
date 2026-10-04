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

        // ---- Gestión de roles y grupos (solo con GESTIONAR_ROLES). Los permisos (patentes) los define el desarrollador.

        public CatalogoPermisos_TE ObtenerCatalogo(ActorUsuario_TLL actor)
        {
            ExigirGestionar(actor);

            var todos = permisoRepo.ObtenerTodos();
            var relaciones = permisoRepo.ObtenerTodasLasRelaciones();
            var hijos = IndexarHijos(relaciones);
            var padres = IndexarPadres(relaciones);
            var simples = new HashSet<string>(todos.Where(p => p.TipoPermiso == TipoPermiso.Simple).Select(p => p.Nombre));
            var usuarios = usuarioRepo.ContarUsuariosPorRol();
            var descripciones = permisoRepo.ObtenerDescripciones();

            var catalogo = new CatalogoPermisos_TE();

            foreach (var permiso in todos)
            {
                var vista = new PermisoVista_TE
                {
                    Nombre = permiso.Nombre,
                    Descripcion = permiso.Descripcion,
                    Etiqueta = Etiqueta(descripciones, permiso.Nombre),
                    Clase = permiso.TipoPermiso == TipoPermiso.Simple ? ClasePermiso.Patente : permiso.EsRolPermiso ? ClasePermiso.Rol : ClasePermiso.Grupo,
                    EsBase = EsRolBase(permiso.Nombre),
                    EsFijo = permiso.Nombre == Usuario_TLL.ROL_GESTOR,
                    Incluye = Ordenado(hijos, permiso.Nombre),
                    IncluidoEn = Ordenado(padres, permiso.Nombre),
                    PatentesEfectivas = Efectivas(permiso.Nombre, hijos, simples).OrderBy(p => Etiqueta(descripciones, p)).ToList()
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

        public void CrearRol(ActorUsuario_TLL actor, string nombre, string descripcion = null)
        {
            Crear(actor, nombre, true, descripcion);
        }

        public void CrearGrupo(ActorUsuario_TLL actor, string nombre, string descripcion = null)
        {
            Crear(actor, nombre, false, descripcion);
        }

        // Lo que se muestra de cada permiso: su descripción o, si no tiene, el nombre interno.
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

        // El nombre es interno y no se cambia; lo que se edita es la descripción que se muestra. Vacía = vuelve a mostrarse el nombre.
        public void CambiarDescripcion(ActorUsuario_TLL actor, string nombre, string descripcion)
        {
            if (actor == null || !actor.Puede(Patentes_TLL.CAMBIAR_DESCRIPCION_PERMISO))
            {
                bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", "Acción rechazada por falta de permiso (" + Patentes_TLL.CAMBIAR_DESCRIPCION_PERMISO + ")", CriticidadBitacora.Media);
                throw new UnauthorizedAccessException("No tenés permiso para cambiar descripciones.");
            }

            PermisoAbstracto_TE permiso = permisoRepo.ObtenerPorPK(nombre ?? string.Empty);
            if (permiso == null) throw new PermisoInvalidoException("\"" + nombre + "\" no existe.");

            string nueva = ValidarDescripcion(descripcion);
            string anterior = string.IsNullOrWhiteSpace(permiso.Descripcion) ? null : permiso.Descripcion;

            if (string.Equals(anterior ?? string.Empty, nueva ?? string.Empty, StringComparison.Ordinal))
                throw new PermisoInvalidoException("No hay cambios para guardar.");

            string clase = permiso.TipoPermiso == TipoPermiso.Simple ? "permiso" : permiso.EsRolPermiso ? "rol" : "grupo";

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.ModificarDescripcion(permiso.Nombre, nueva);

                Auditar(actor, "Cambio de descripción del " + clase + " \"" + permiso.Nombre + "\": " +
                    (anterior == null ? "(sin descripción)" : "\"" + anterior + "\"") + " → " + (nueva == null ? "(sin descripción)" : "\"" + nueva + "\""),
                    CriticidadBitacora.Media);
            });
        }

        private static string ValidarDescripcion(string descripcion)
        {
            descripcion = (descripcion ?? string.Empty).Trim();

            if (descripcion.Length == 0) return null;

            if (descripcion.Length > 200) throw new PermisoInvalidoException("El nombre no puede superar los 200 caracteres.");

            if (descripcion.Any(char.IsControl)) throw new PermisoInvalidoException("El nombre no puede tener saltos de línea ni caracteres de control.");

            return descripcion;
        }

        public void GuardarComposicion(ActorUsuario_TLL actor, string nombre, IEnumerable<string> incluidos)
        {
            ExigirGestionar(actor);

            var todos = permisoRepo.ObtenerTodos().ToDictionary(p => p.Nombre);

            PermisoAbstracto_TE objetivo;
            if (!todos.TryGetValue(nombre ?? string.Empty, out objetivo) || objetivo.TipoPermiso != TipoPermiso.Compuesto)
                throw new PermisoInvalidoException("Solo los roles y los grupos pueden contener permisos.");

            if (nombre == Usuario_TLL.ROL_GESTOR) throw new PermisoInvalidoException("El rol Gestor es fijo: su composición no se modifica.");

            var nuevos = new HashSet<string>((incluidos ?? new string[0]).Select(x => (x ?? string.Empty).Trim()).Where(x => x.Length > 0));

            foreach (string inc in nuevos)
            {
                PermisoAbstracto_TE hijo;
                if (!todos.TryGetValue(inc, out hijo)) throw new PermisoInvalidoException("El permiso \"" + inc + "\" no existe.");

                if (inc == nombre) throw new PermisoInvalidoException("Un permiso no puede incluirse a sí mismo.");

                if (hijo.EsRolPermiso) throw new PermisoInvalidoException("\"" + inc + "\" es un rol: ni un rol ni un grupo pueden incluir roles.");
            }

            var relaciones = permisoRepo.ObtenerTodasLasRelaciones();
            var hijosAntes = IndexarHijos(relaciones);
            var hijosDespues = IndexarHijos(relaciones.Where(r => r.Compuesto != nombre));
            hijosDespues[nombre] = nuevos.OrderBy(x => x).ToList();

            foreach (string inc in nuevos)
            {
                if (Alcanza(inc, nombre, hijosDespues)) throw new PermisoInvalidoException("Agregar \"" + inc + "\" a \"" + nombre + "\" generaría un ciclo de composición.");
            }

            var simplesTodos = new HashSet<string>(todos.Values.Where(p => p.TipoPermiso == TipoPermiso.Simple).Select(p => p.Nombre));

            foreach (var rol in todos.Values.Where(p => p.EsRolPermiso))
            {
                var nuevosReservados = Efectivas(rol.Nombre, hijosDespues, simplesTodos)
                    .Except(Efectivas(rol.Nombre, hijosAntes, simplesTodos))
                    .Where(Patentes_TLL.EsDeProveedor)
                    .OrderBy(x => x).ToList();

                if (nuevosReservados.Count > 0 && usuarioRepo.ExisteUsuarioConRolFueraDeEmpresa(rol.Nombre, BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA))
                {
                    var etiquetas = permisoRepo.ObtenerDescripciones();

                    throw new PermisoInvalidoException("El rol \"" + Etiqueta(etiquetas, rol.Nombre) + "\" lo usan usuarios de empresas cliente y quedaría con permisos reservados a Pattern Blue (" +
                        string.Join(", ", nuevosReservados.Select(p => Etiqueta(etiquetas, p))) + ").");
                }
            }

            var actuales = new HashSet<string>(hijosAntes.ContainsKey(nombre) ? hijosAntes[nombre] : new List<string>());
            var aQuitar = actuales.Except(nuevos).OrderBy(x => x).ToList();
            var aAgregar = nuevos.Except(actuales).OrderBy(x => x).ToList();

            if (aQuitar.Count == 0 && aAgregar.Count == 0) throw new PermisoInvalidoException("No hay cambios para guardar.");

            var simples = new HashSet<string>(todos.Values.Where(p => p.TipoPermiso == TipoPermiso.Simple).Select(p => p.Nombre));
            var efectivosAntes = Efectivas(nombre, hijosAntes, simples);
            var efectivosDespues = Efectivas(nombre, hijosDespues, simples);
            var ganados = efectivosDespues.Except(efectivosAntes).OrderBy(x => x).ToList();
            var perdidos = efectivosAntes.Except(efectivosDespues).OrderBy(x => x).ToList();

            string tipo = objetivo.EsRolPermiso ? "rol" : "grupo";

            Transaccion_ORM.Ejecutar(() =>
            {
                foreach (string q in aQuitar) permisoRepo.EliminarRelacion(nombre, q);
                foreach (string a in aAgregar) permisoRepo.AgregarRelacion(nombre, a);

                Auditar(actor, "Cambio de composición del " + tipo + " \"" + nombre + "\"" +
                    (aAgregar.Count > 0 ? ". Agrega: " + string.Join(", ", aAgregar) : string.Empty) +
                    (aQuitar.Count > 0 ? ". Quita: " + string.Join(", ", aQuitar) : string.Empty) +
                    ". Permisos efectivos" + (ganados.Count > 0 ? " +[" + string.Join(", ", ganados) + "]" : string.Empty) + (perdidos.Count > 0 ? " -[" + string.Join(", ", perdidos) + "]" : string.Empty),
                    CriticidadBitacora.Alta);
            });
        }

        public void EliminarRolOGrupo(ActorUsuario_TLL actor, string nombre)
        {
            ExigirGestionar(actor);

            PermisoAbstracto_TE permiso = permisoRepo.ObtenerPorPK(nombre ?? string.Empty);
            if (permiso == null) throw new PermisoInvalidoException("\"" + nombre + "\" no existe.");

            ExigirEditable(permiso, "eliminar");

            if (permiso.EsRolPermiso && usuarioRepo.ExisteUsuarioConRol(nombre))
                throw new PermisoInvalidoException("El rol \"" + nombre + "\" está asignado a usuarios: reasignalos antes de eliminarlo.");

            var relaciones = permisoRepo.ObtenerTodasLasRelaciones();
            var enUso = relaciones.Where(r => r.Incluido == nombre).Select(r => r.Compuesto).OrderBy(x => x).ToList();

            if (enUso.Count > 0) throw new PermisoInvalidoException("\"" + nombre + "\" está incluido en: " + string.Join(", ", enUso) + ". Quitalo de ahí antes de eliminarlo.");

            var propios = relaciones.Where(r => r.Compuesto == nombre).Select(r => r.Incluido).ToList();
            string tipo = permiso.EsRolPermiso ? "rol" : "grupo";

            Transaccion_ORM.Ejecutar(() =>
            {
                foreach (string hijo in propios) permisoRepo.EliminarRelacion(nombre, hijo);

                permisoRepo.Eliminar(nombre);

                Auditar(actor, "Baja del " + tipo + " \"" + nombre + "\"" + (propios.Count > 0 ? " (incluía: " + string.Join(", ", propios.OrderBy(x => x)) + ")" : string.Empty), CriticidadBitacora.Alta);
            });
        }

        // Las patentes simples que alcanza un rol o grupo, sin contar los nombres de los grupos intermedios.
        public static HashSet<string> ObtenerPatentes(PermisoAbstracto_TE permiso)
        {
            var patentes = new HashSet<string>();

            if (permiso != null) RecolectarPatentes(permiso, patentes);

            return patentes;
        }

        private static void RecolectarPatentes(PermisoAbstracto_TE nodo, HashSet<string> patentes)
        {
            if (nodo.TipoPermiso == TipoPermiso.Simple)
            {
                patentes.Add(nodo.Nombre);
                return;
            }

            foreach (var hijo in nodo.ObtenerHijos()) RecolectarPatentes(hijo, patentes);
        }

        private void Crear(ActorUsuario_TLL actor, string nombre, bool esRol, string descripcion)
        {
            ExigirGestionar(actor);

            nombre = ValidarNombre(nombre);
            descripcion = ValidarDescripcion(descripcion);

            if (permisoRepo.Existe(nombre)) throw new PermisoInvalidoException("Ya existe un permiso, rol o grupo con el nombre \"" + nombre + "\".");

            Transaccion_ORM.Ejecutar(() =>
            {
                permisoRepo.Alta(new PermisoCompuesto_TE(nombre, esRol) { Descripcion = descripcion });

                Auditar(actor, "Alta del " + (esRol ? "rol" : "grupo") + " \"" + nombre + "\"" + (descripcion == null ? string.Empty : " (descripción: \"" + descripcion + "\")"), CriticidadBitacora.Media);
            });
        }

        private static string ValidarNombre(string nombre)
        {
            nombre = (nombre ?? string.Empty).Trim();

            if (!Regex.IsMatch(nombre, @"^[\p{L}\p{N}][\p{L}\p{N} _\.\-]{1,58}[\p{L}\p{N}]$"))
                throw new PermisoInvalidoException("El nombre debe tener entre 3 y 60 caracteres (letras, números, espacios, guiones o puntos).");

            return nombre;
        }

        private static bool EsRolBase(string nombre)
        {
            return nombre == Usuario_TLL.ROL_GESTOR || nombre == Usuario_TLL.ROL_ADMINISTRADOR || nombre == Usuario_TLL.ROL_ANALISTA;
        }

        private static void ExigirEditable(PermisoAbstracto_TE permiso, string accion)
        {
            if (permiso.TipoPermiso == TipoPermiso.Simple)
                throw new PermisoInvalidoException("\"" + permiso.Nombre + "\" es un permiso del sistema: lo define el desarrollador y no se puede " + accion + ".");

            if (EsRolBase(permiso.Nombre))
                throw new PermisoInvalidoException("\"" + permiso.Nombre + "\" es un rol base del sistema: no se puede " + accion + ".");
        }

        private void ExigirGestionar(ActorUsuario_TLL actor)
        {
            if (actor != null && actor.Puede(Patentes_TLL.GESTIONAR_ROLES)) return;

            bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", "Acción rechazada por falta de permiso (" + Patentes_TLL.GESTIONAR_ROLES + ")", CriticidadBitacora.Media);
            throw new UnauthorizedAccessException("No tenés permiso para gestionar roles y permisos.");
        }

        // Estos eventos son de auditoría: si no se pueden guardar, el cambio completo se revierte.
        private void Auditar(ActorUsuario_TLL actor, string descripcion, CriticidadBitacora criticidad)
        {
            bitacora.Guardar(new Bitacora_TE(actor.IdUsuario, "Roles", descripcion, criticidad, DateTime.Now));
        }

        private static Dictionary<string, List<string>> IndexarHijos(IEnumerable<(string Compuesto, string Incluido)> relaciones)
        {
            var indice = new Dictionary<string, List<string>>();

            foreach (var r in relaciones)
            {
                List<string> lista;
                if (!indice.TryGetValue(r.Compuesto, out lista)) indice[r.Compuesto] = lista = new List<string>();
                lista.Add(r.Incluido);
            }

            return indice;
        }

        private static Dictionary<string, List<string>> IndexarPadres(IEnumerable<(string Compuesto, string Incluido)> relaciones)
        {
            var indice = new Dictionary<string, List<string>>();

            foreach (var r in relaciones)
            {
                List<string> lista;
                if (!indice.TryGetValue(r.Incluido, out lista)) indice[r.Incluido] = lista = new List<string>();
                lista.Add(r.Compuesto);
            }

            return indice;
        }

        private static List<string> Ordenado(Dictionary<string, List<string>> indice, string clave)
        {
            List<string> lista;

            return indice.TryGetValue(clave, out lista) ? lista.OrderBy(x => x).ToList() : new List<string>();
        }

        private static bool Alcanza(string desde, string objetivo, Dictionary<string, List<string>> hijos)
        {
            var visitados = new HashSet<string>();
            var pendientes = new Stack<string>();
            pendientes.Push(desde);

            while (pendientes.Count > 0)
            {
                string actual = pendientes.Pop();

                if (actual == objetivo) return true;
                if (!visitados.Add(actual)) continue;

                List<string> siguientes;
                if (hijos.TryGetValue(actual, out siguientes))
                    foreach (string s in siguientes) pendientes.Push(s);
            }

            return false;
        }

        private static HashSet<string> Efectivas(string nombre, Dictionary<string, List<string>> hijos, HashSet<string> simples)
        {
            var resultado = new HashSet<string>();
            var visitados = new HashSet<string>();
            var pendientes = new Stack<string>();
            pendientes.Push(nombre);

            while (pendientes.Count > 0)
            {
                string actual = pendientes.Pop();

                if (!visitados.Add(actual)) continue;
                if (simples.Contains(actual)) resultado.Add(actual);

                List<string> siguientes;
                if (hijos.TryGetValue(actual, out siguientes))
                    foreach (string s in siguientes) pendientes.Push(s);
            }

            return resultado;
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
