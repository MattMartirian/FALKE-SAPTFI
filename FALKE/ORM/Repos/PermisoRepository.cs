using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DAL;
using TE;

namespace ORM
{
    public class PermisoRepository : RepositoryBase<PermisoAbstracto_TE, string>
    {
        public PermisoRepository() : base() { }

        public override void Alta(PermisoAbstracto_TE p)
        {
            string sql = @"
                INSERT INTO PermisoTable (nombre_permiso, tipo_permiso, es_rol_permiso)
                VALUES (@nombre, @tipo, @esRol)";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@nombre", p.Nombre),
                new SqlParameter("@tipo", p.TipoPermiso.ToString().ToLowerInvariant()),
                new SqlParameter("@esRol", p.EsRolPermiso)
            );
        }

        public override void Modificar(PermisoAbstracto_TE p)
        {
            string sql = @"
                UPDATE PermisoTable SET
                    tipo_permiso = @tipo,
                    es_rol_permiso = @esRol
                WHERE nombre_permiso = @nombre";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@nombre", p.Nombre),
                new SqlParameter("@tipo", p.TipoPermiso.ToString().ToLowerInvariant()),
                new SqlParameter("@esRol", p.EsRolPermiso)
            );
        }

        public void ModificarNombre(string nombreViejo, string nombreNuevo)
        {
            // Son dos queries, se envuelven en una transaccion para que
            // el rename del permiso y el de sus relaciones sean atómicos
            string sqlPermiso = @"
                UPDATE PermisoTable SET nombre_permiso = @nuevo
                WHERE nombre_permiso = @viejo";

            Gestor.EjecutarNonQuery(sqlPermiso,
                new SqlParameter("@viejo", nombreViejo),
                new SqlParameter("@nuevo", nombreNuevo)
            );

            string sqlRelaciones = @"
                UPDATE RelacionPermisosTable SET
                    nombre_permiso_compuesto = CASE WHEN nombre_permiso_compuesto = @viejo THEN @nuevo ELSE nombre_permiso_compuesto END,
                    nombre_permiso_incluido  = CASE WHEN nombre_permiso_incluido  = @viejo THEN @nuevo ELSE nombre_permiso_incluido  END
                WHERE nombre_permiso_compuesto = @viejo OR nombre_permiso_incluido = @viejo";

            Gestor.EjecutarNonQuery(sqlRelaciones,
                new SqlParameter("@viejo", nombreViejo),
                new SqlParameter("@nuevo", nombreNuevo)
            );
        }

        public bool PermisoEnRelacion(string nombre)
        {
            string sql = @"
                SELECT COUNT(1) FROM RelacionPermisosTable
                WHERE nombre_permiso_compuesto = @nombre OR nombre_permiso_incluido = @nombre";

            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@nombre", nombre));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        public void Eliminar(string nombre)
        {
            string sql = "DELETE FROM PermisoTable WHERE nombre_permiso = @nombre";
            Gestor.EjecutarNonQuery(sql, new SqlParameter("@nombre", nombre));
        }

        public override PermisoAbstracto_TE ObtenerPorPK(string nombre)
        {
            string sql = "SELECT * FROM PermisoTable WHERE nombre_permiso = @nombre";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@nombre", nombre));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<PermisoAbstracto_TE> ObtenerTodos()
        {
            string sql = "SELECT * FROM PermisoTable ORDER BY nombre_permiso";
            return MapTodos(Gestor.EjecutarQuery(sql));
        }

        public List<PermisoAbstracto_TE> ObtenerPorTipo(TipoPermiso tipo)
        {
            string sql = "SELECT * FROM PermisoTable WHERE tipo_permiso = @tipo ORDER BY nombre_permiso";
            return MapTodos(Gestor.EjecutarQuery(sql, new SqlParameter("@tipo", tipo.ToString().ToLowerInvariant())));
        }

        public bool Existe(string nombre)
        {
            string sql = "SELECT COUNT(1) FROM PermisoTable WHERE nombre_permiso = @nombre";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@nombre", nombre));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        public void AgregarRelacion(string nombreCompuesto, string nombreIncluido)
        {
            string sql = @"
                INSERT INTO RelacionPermisosTable (nombre_permiso_compuesto, nombre_permiso_incluido)
                VALUES (@compuesto, @incluido)";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@compuesto", nombreCompuesto),
                new SqlParameter("@incluido", nombreIncluido)
            );
        }

        public void EliminarRelacion(string nombreCompuesto, string nombreIncluido)
        {
            string sql = @"
                DELETE FROM RelacionPermisosTable
                WHERE nombre_permiso_compuesto = @compuesto AND nombre_permiso_incluido = @incluido";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@compuesto", nombreCompuesto),
                new SqlParameter("@incluido", nombreIncluido)
            );
        }

        public List<(string Compuesto, string Incluido)> ObtenerTodasLasRelaciones()
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

        public List<string> ObtenerHijosDirectos(string nombreCompuesto)
        {
            const string sql = @"
                SELECT nombre_permiso_incluido
                FROM RelacionPermisosTable
                WHERE nombre_permiso_compuesto = @compuesto";

            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@compuesto", nombreCompuesto));
            var hijos = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                hijos.Add(row["nombre_permiso_incluido"].ToString());
            }

            return hijos;
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
                if (compuesto != null) ExpandirSubarbol(compuesto, nodos, hijosPorPadre, new HashSet<string>(), expandidos);
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

            ExpandirSubarbol(raiz, nodos, IndexarHijosDirectos(), new HashSet<string>(), new HashSet<string>());
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

        private static void ExpandirSubarbol(PermisoCompuesto_TE padre, Dictionary<string, PermisoAbstracto_TE> nodos, Dictionary<string, List<string>> hijosPorPadre, HashSet<string> enCamino, HashSet<string> expandidos)
        {
            if (!expandidos.Add(padre.Nombre)) return;

            enCamino.Add(padre.Nombre);

            List<string> hijos;
            if (hijosPorPadre.TryGetValue(padre.Nombre, out hijos))
            {
                foreach (var nombreHijo in hijos)
                {
                    if (enCamino.Contains(nombreHijo)) continue;

                    PermisoAbstracto_TE hijoNodo;
                    if (!nodos.TryGetValue(nombreHijo, out hijoNodo)) continue;

                    padre.AgregarHijoPersistido(hijoNodo);

                    PermisoCompuesto_TE hijoCompuesto = hijoNodo as PermisoCompuesto_TE;
                    if (hijoCompuesto != null) ExpandirSubarbol(hijoCompuesto, nodos, hijosPorPadre, enCamino, expandidos);
                }
            }

            enCamino.Remove(padre.Nombre);
        }

        private static PermisoAbstracto_TE Map(DataRow dr)
        {
            string nombre = Valor<string>(dr, "nombre_permiso");
            var tipo = Valor<TipoPermiso>(dr, "tipo_permiso");
            bool esRol = Valor<bool>(dr, "es_rol_permiso");

            return tipo == TipoPermiso.Simple ? (PermisoAbstracto_TE)new PermisoSimple_TE(nombre) : new PermisoCompuesto_TE(nombre, esRol);
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