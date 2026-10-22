using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TE;

namespace ORM
{
    public class IdiomaRepository : RepositoryBase<Idioma_TE, int>
    {
        public IdiomaRepository() : base() { }

        public override void Alta(Idioma_TE i)
        {
            const string sql = @"
                INSERT INTO IdiomaTable (codigo_idioma, nombre_idioma, activo_idioma, es_base_idioma)
                VALUES (@codigo, @nombre, @activo, @esBase);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            i.IdIdioma = Gestor.EjecutarScalar<int>(sql,
                new SqlParameter("@codigo", i.CodigoIdioma),
                new SqlParameter("@nombre", ValorONulo(i.NombreIdioma)),
                new SqlParameter("@activo", i.Activo),
                new SqlParameter("@esBase", i.EsBase));
        }

        public override void Modificar(Idioma_TE i)
        {
            Gestor.EjecutarNonQuery(@"
                UPDATE IdiomaTable SET
                    nombre_idioma = @nombre,
                    activo_idioma = @activo
                WHERE id_idioma = @id",
                new SqlParameter("@nombre", ValorONulo(i.NombreIdioma)),
                new SqlParameter("@activo", i.Activo),
                new SqlParameter("@id", i.IdIdioma));
        }

        public void Eliminar(int idIdioma)
        {
            Gestor.EjecutarNonQuery("DELETE FROM IdiomaTable WHERE id_idioma = @id", new SqlParameter("@id", idIdioma));
        }

        public override Idioma_TE ObtenerPorPK(int pk)
        {
            var dt = Gestor.EjecutarQuery(SqlSeleccion() + " WHERE i.id_idioma = @id", new SqlParameter("@id", pk));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public Idioma_TE ObtenerPorCodigo(string codigo)
        {
            var dt = Gestor.EjecutarQuery(SqlSeleccion() + " WHERE i.codigo_idioma = @codigo", new SqlParameter("@codigo", codigo));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public Idioma_TE ObtenerBase()
        {
            var dt = Gestor.EjecutarQuery(SqlSeleccion() + " WHERE i.es_base_idioma = 1");
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<Idioma_TE> ObtenerTodos()
        {
            return MapTodos(Gestor.EjecutarQuery(SqlSeleccion() + " ORDER BY i.es_base_idioma DESC, i.nombre_idioma"));
        }

        public List<Idioma_TE> ObtenerActivos()
        {
            return MapTodos(Gestor.EjecutarQuery(SqlSeleccion() + " WHERE i.activo_idioma = 1 ORDER BY i.es_base_idioma DESC, i.nombre_idioma"));
        }

        public bool ExisteCodigo(string codigo)
        {
            var dt = Gestor.EjecutarQuery("SELECT COUNT(1) FROM IdiomaTable WHERE codigo_idioma = @codigo", new SqlParameter("@codigo", codigo));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        private static string SqlSeleccion()
        {
            return @"
                SELECT i.id_idioma, i.codigo_idioma, i.nombre_idioma, i.activo_idioma, i.es_base_idioma,
                       (SELECT COUNT(1) FROM TraduccionTable t WHERE t.id_idioma = i.id_idioma) AS cantidad_traducciones,
                       (SELECT COUNT(1) FROM TraduccionTable t WHERE t.id_idioma = i.id_idioma AND t.pendiente_traduccion = 1) AS cantidad_pendientes,
                       (SELECT COUNT(1) FROM UsuarioTable u WHERE u.id_idioma = i.id_idioma) AS cantidad_usuarios
                FROM IdiomaTable i";
        }

        private static List<Idioma_TE> MapTodos(DataTable dt)
        {
            var lista = new List<Idioma_TE>();
            foreach (DataRow row in dt.Rows) lista.Add(Map(row));
            return lista;
        }

        private static Idioma_TE Map(DataRow dr)
        {
            return new Idioma_TE
            {
                IdIdioma = Valor<int>(dr, "id_idioma"),
                CodigoIdioma = Valor<string>(dr, "codigo_idioma"),
                NombreIdioma = Valor<string>(dr, "nombre_idioma"),
                Activo = Valor<bool>(dr, "activo_idioma"),
                EsBase = Valor<bool>(dr, "es_base_idioma"),
                CantidadTraducciones = Valor<int>(dr, "cantidad_traducciones"),
                CantidadPendientes = Valor<int>(dr, "cantidad_pendientes"),
                CantidadUsuarios = Valor<int>(dr, "cantidad_usuarios")
            };
        }
    }
}
