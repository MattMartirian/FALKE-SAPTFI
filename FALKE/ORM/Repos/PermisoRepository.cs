using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DAL;
using TE;

namespace ORM
{
    public class PermisoRepository : RepositoryBase<PermisoAbstracto_TE, string>
    {
        public PermisoRepository() : base() { }

        // Alta del permiso. Si es un rol o un grupo, también se guardan los hijos que ya tiene el Composite.
        public override void Alta(PermisoAbstracto_TE p)
        {
            string sql = @"
                INSERT INTO PermisoTable (nombre_permiso, tipo_permiso, es_rol_permiso, descripcion_permiso, es_de_gestion_permiso)
                VALUES (@nombre, @tipo, @esRol, @descripcion, @deGestion)";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@nombre", p.Nombre),
                new SqlParameter("@tipo", p.TipoPermiso.ToString().ToLowerInvariant()),
                new SqlParameter("@esRol", p.EsRolPermiso),
                new SqlParameter("@descripcion", ValorONulo(p.Descripcion)),
                new SqlParameter("@deGestion", p.EsRolPermiso && p.EsDeGestion)
            );

            PermisoCompuesto_TE compuesto = p as PermisoCompuesto_TE;
            if (compuesto == null) return;

            foreach (PermisoAbstracto_TE hijo in compuesto.ObtenerHijos()) AgregarRelacion(compuesto.Nombre, hijo.Nombre);
        }

        // Deja la base igual que el permiso: su descripción y su tipo de rol y, si es un rol o un grupo, sus hijos. La diferencia se calcula
        // comparando los hijos del Composite con las relaciones que hay guardadas: se borran las que sobran y se agregan las que faltan,
        // sin reescribir toda la composición. El nombre es interno y no cambia.
        public override void Modificar(PermisoAbstracto_TE p)
        {
            Gestor.EjecutarNonQuery(@"
                UPDATE PermisoTable SET
                    descripcion_permiso = @descripcion,
                    es_de_gestion_permiso = @deGestion
                WHERE nombre_permiso = @nombre",
                new SqlParameter("@nombre", p.Nombre),
                new SqlParameter("@descripcion", ValorONulo(p.Descripcion)),
                new SqlParameter("@deGestion", p.EsRolPermiso && p.EsDeGestion));

            if (p.TipoPermiso != TipoPermiso.Compuesto) return;

            var guardados = new HashSet<string>(ObtenerNombresDeHijosGuardados(p.Nombre));
            var actuales = new HashSet<string>(p.ObtenerHijos().Select(h => h.Nombre));

            foreach (string sobrante in guardados.Except(actuales).OrderBy(x => x)) EliminarRelacion(p.Nombre, sobrante);
            foreach (string faltante in actuales.Except(guardados).OrderBy(x => x)) AgregarRelacion(p.Nombre, faltante);
        }

        // Serializa las ediciones de la composición: dos personas que editen a la vez no pueden, entre las dos, crear un ciclo que
        // ninguna de las dos vio. Se pide dentro de la transacción y se libera al terminar.
        public void BloquearComposicion()
        {
            Gestor.EjecutarNonQuery("EXEC sp_getapplock @Resource = N'FALKE_COMPOSICION_PERMISOS', @LockMode = N'Exclusive', @LockOwner = N'Transaction', @LockTimeout = 15000");
        }

        public Dictionary<string, string> ObtenerDescripciones()
        {
            var descripciones = new Dictionary<string, string>();

            foreach (DataRow row in Gestor.EjecutarQuery("SELECT nombre_permiso, descripcion_permiso FROM PermisoTable WHERE descripcion_permiso IS NOT NULL").Rows)
            {
                string texto = Convert.ToString(row["descripcion_permiso"]);
                if (!string.IsNullOrWhiteSpace(texto)) descripciones[Convert.ToString(row["nombre_permiso"])] = texto;
            }

            return descripciones;
        }

        public void Eliminar(string nombre)
        {
            string sql = "DELETE FROM PermisoTable WHERE nombre_permiso = @nombre";
            Gestor.EjecutarNonQuery(sql, new SqlParameter("@nombre", nombre));
        }

        // Un rol o un grupo vuelve con todo su subárbol: así se puede modificar y guardar con Modificar sin perder sus hijos.
        public override PermisoAbstracto_TE ObtenerPorPK(string nombre)
        {
            string sql = "SELECT * FROM PermisoTable WHERE nombre_permiso = @nombre";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@nombre", nombre));

            if (dt.Rows.Count == 0) return null;

            PermisoAbstracto_TE permiso = Map(dt.Rows[0]);

            return permiso.TipoPermiso == TipoPermiso.Compuesto ? ConstruirArbolRol(nombre) : permiso;
        }

        public override List<PermisoAbstracto_TE> ObtenerTodos()
        {
            string sql = "SELECT * FROM PermisoTable ORDER BY nombre_permiso";
            return MapTodos(Gestor.EjecutarQuery(sql));
        }

        public bool Existe(string nombre)
        {
            string sql = "SELECT COUNT(1) FROM PermisoTable WHERE nombre_permiso = @nombre";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@nombre", nombre));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        private List<string> ObtenerNombresDeHijosGuardados(string nombreCompuesto)
        {
            var dt = Gestor.EjecutarQuery("SELECT nombre_permiso_incluido FROM RelacionPermisosTable WHERE nombre_permiso_compuesto = @compuesto",
                new SqlParameter("@compuesto", nombreCompuesto));

            return dt.Rows.Cast<DataRow>().Select(r => r["nombre_permiso_incluido"].ToString()).ToList();
        }

        private void AgregarRelacion(string nombreCompuesto, string nombreIncluido)
        {
            string sql = @"
                INSERT INTO RelacionPermisosTable (nombre_permiso_compuesto, nombre_permiso_incluido)
                VALUES (@compuesto, @incluido)";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@compuesto", nombreCompuesto),
                new SqlParameter("@incluido", nombreIncluido)
            );
        }

        private void EliminarRelacion(string nombreCompuesto, string nombreIncluido)
        {
            string sql = @"
                DELETE FROM RelacionPermisosTable
                WHERE nombre_permiso_compuesto = @compuesto AND nombre_permiso_incluido = @incluido";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@compuesto", nombreCompuesto),
                new SqlParameter("@incluido", nombreIncluido)
            );
        }

        private List<(string Compuesto, string Incluido)> ObtenerTodasLasRelaciones()
        {
            const string sql = "SELECT nombre_permiso_compuesto, nombre_permiso_incluido FROM RelacionPermisosTable";
            var dt = Gestor.EjecutarQuery(sql);
            var relaciones = new List<(string, string)>();

            foreach (DataRow row in dt.Rows)
            {
                relaciones.Add((row["nombre_permiso_compuesto"].ToString(), row["nombre_permiso_incluido"].ToString()));
            }

            return relaciones;
        }

        public Dictionary<string, PermisoAbstracto_TE> ConstruirArbol()
        {
            var nodos = new Dictionary<string, PermisoAbstracto_TE>();

            foreach (var permiso in ObtenerTodos())
            {
                nodos[permiso.Nombre] = permiso;
            }

            var hijosPorPadre = IndexarHijosDirectos();
            var expandidos = new HashSet<string>();

            foreach (var nodo in nodos.Values)
            {
                PermisoCompuesto_TE compuesto = nodo as PermisoCompuesto_TE;
                if (compuesto != null) ExpandirSubarbol(compuesto, nodos, hijosPorPadre, expandidos);
            }

            return nodos;
        }

        public PermisoCompuesto_TE ConstruirArbolRol(string nombreRol)
        {
            if (string.IsNullOrEmpty(nombreRol)) return null;

            var nodos = new Dictionary<string, PermisoAbstracto_TE>();

            foreach (var permiso in ObtenerTodos())
            {
                nodos[permiso.Nombre] = permiso;
            }

            PermisoAbstracto_TE raizNodo;
            if (!nodos.TryGetValue(nombreRol, out raizNodo)) return null;

            PermisoCompuesto_TE raiz = raizNodo as PermisoCompuesto_TE;
            if (raiz == null) return null;

            ExpandirSubarbol(raiz, nodos, IndexarHijosDirectos(), new HashSet<string>());
            return raiz;
        }

        public List<PermisoAbstracto_TE> ConstruirArbolDeRoles()
        {
            var roles = new List<PermisoAbstracto_TE>();

            foreach (var nodo in ConstruirArbol().Values)
            {
                if (nodo.EsRolPermiso) roles.Add(nodo);
            }

            return roles;
        }

        private Dictionary<string, List<string>> IndexarHijosDirectos()
        {
            var indice = new Dictionary<string, List<string>>();

            foreach (var relacion in ObtenerTodasLasRelaciones())
            {
                List<string> lista;
                if (!indice.TryGetValue(relacion.Compuesto, out lista))
                {
                    lista = new List<string>();
                    indice[relacion.Compuesto] = lista;
                }

                lista.Add(relacion.Incluido);
            }

            return indice;
        }

        // Arma el subárbol de un rol o grupo con Agregar, el mismo camino que usa el negocio: la estructura se valida al cargarla.
        // Se expande primero el subárbol de cada hijo, así Agregar puede detectar un ciclo; si la base tuviera uno (dato corrupto), esa
        // relación se ignora y el árbol queda sin ciclos.
        private static void ExpandirSubarbol(PermisoCompuesto_TE padre, Dictionary<string, PermisoAbstracto_TE> nodos, Dictionary<string, List<string>> hijosPorPadre, HashSet<string> expandidos)
        {
            if (!expandidos.Add(padre.Nombre)) return;

            List<string> hijos;
            if (!hijosPorPadre.TryGetValue(padre.Nombre, out hijos)) return;

            foreach (var nombreHijo in hijos)
            {
                PermisoAbstracto_TE hijoNodo;
                if (!nodos.TryGetValue(nombreHijo, out hijoNodo)) continue;

                PermisoCompuesto_TE hijoCompuesto = hijoNodo as PermisoCompuesto_TE;
                if (hijoCompuesto != null) ExpandirSubarbol(hijoCompuesto, nodos, hijosPorPadre, expandidos);

                try
                {
                    padre.Agregar(hijoNodo);
                }
                catch (PermisoInvalidoException)
                {
                }
            }
        }

        private static PermisoAbstracto_TE Map(DataRow dr)
        {
            string nombre = Valor<string>(dr, "nombre_permiso");
            var tipo = Valor<TipoPermiso>(dr, "tipo_permiso");
            bool esRol = Valor<bool>(dr, "es_rol_permiso");

            PermisoAbstracto_TE permiso = tipo == TipoPermiso.Simple ? (PermisoAbstracto_TE)new PermisoSimple_TE(nombre) : new PermisoCompuesto_TE(nombre, esRol);

            if (dr.Table.Columns.Contains("descripcion_permiso")) permiso.Descripcion = Valor<string>(dr, "descripcion_permiso");
            if (dr.Table.Columns.Contains("es_de_gestion_permiso")) permiso.EsDeGestion = Valor<bool>(dr, "es_de_gestion_permiso");

            return permiso;
        }

        private static List<PermisoAbstracto_TE> MapTodos(DataTable dt)
        {
            var lista = new List<PermisoAbstracto_TE>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(Map(row));
            }
            return lista;
        }
    }
}