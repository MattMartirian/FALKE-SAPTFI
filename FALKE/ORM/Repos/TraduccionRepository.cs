using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using TE;

namespace ORM
{
    public class TraduccionRepository : RepositoryBase<Traduccion_TE, TraduccionClave_TE>
    {
        private const int FILAS_POR_LOTE = 200;

        public TraduccionRepository() : base() { }

        public override void Alta(Traduccion_TE t)
        {
            const string sql = @"
                INSERT INTO TraduccionTable (codigo_clave_traduccion, id_idioma, texto_traduccion, pendiente_traduccion, origen_traduccion)
                VALUES (@clave, @idioma, @texto, @pendiente, @origen)";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@clave", t.CodigoClave),
                new SqlParameter("@idioma", t.IdIdioma),
                new SqlParameter("@texto", ValorONulo(t.Texto)),
                new SqlParameter("@pendiente", t.Pendiente),
                new SqlParameter("@origen", t.Origen.ToString().ToLowerInvariant()));
        }

        public override void Modificar(Traduccion_TE t)
        {
            Gestor.EjecutarNonQuery(@"
                UPDATE TraduccionTable SET
                    texto_traduccion = @texto,
                    pendiente_traduccion = @pendiente,
                    origen_traduccion = @origen
                WHERE codigo_clave_traduccion = @clave AND id_idioma = @idioma",
                new SqlParameter("@texto", ValorONulo(t.Texto)),
                new SqlParameter("@pendiente", t.Pendiente),
                new SqlParameter("@origen", t.Origen.ToString().ToLowerInvariant()),
                new SqlParameter("@clave", t.CodigoClave),
                new SqlParameter("@idioma", t.IdIdioma));
        }

        public void AltaMasiva(IEnumerable<Traduccion_TE> traducciones)
        {
            var lote = new List<Traduccion_TE>();

            foreach (Traduccion_TE t in traducciones)
            {
                lote.Add(t);

                if (lote.Count < FILAS_POR_LOTE) continue;

                InsertarLote(lote);
                lote.Clear();
            }

            if (lote.Count > 0) InsertarLote(lote);
        }

        public void EliminarPorIdioma(int idIdioma)
        {
            Gestor.EjecutarNonQuery("DELETE FROM TraduccionTable WHERE id_idioma = @idioma", new SqlParameter("@idioma", idIdioma));
        }

        public override Traduccion_TE ObtenerPorPK(TraduccionClave_TE pk)
        {
            var dt = Gestor.EjecutarQuery(
                SqlSeleccion() + " WHERE codigo_clave_traduccion = @clave AND id_idioma = @idioma",
                new SqlParameter("@clave", pk.CodigoClave),
                new SqlParameter("@idioma", pk.IdIdioma));

            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<Traduccion_TE> ObtenerTodos()
        {
            return MapTodos(Gestor.EjecutarQuery(SqlSeleccion() + " ORDER BY id_idioma, codigo_clave_traduccion"));
        }

        public List<Traduccion_TE> ObtenerPorIdioma(int idIdioma)
        {
            return MapTodos(Gestor.EjecutarQuery(
                SqlSeleccion() + " WHERE id_idioma = @idioma ORDER BY codigo_clave_traduccion",
                new SqlParameter("@idioma", idIdioma)));
        }

        public Dictionary<string, string> ObtenerTextos(int idIdioma)
        {
            var textos = new Dictionary<string, string>(StringComparer.Ordinal);

            var dt = Gestor.EjecutarQuery(
                "SELECT codigo_clave_traduccion, texto_traduccion FROM TraduccionTable WHERE id_idioma = @idioma AND pendiente_traduccion = 0 AND texto_traduccion IS NOT NULL",
                new SqlParameter("@idioma", idIdioma));

            foreach (DataRow row in dt.Rows) textos[Valor<string>(row, "codigo_clave_traduccion")] = Valor<string>(row, "texto_traduccion");

            return textos;
        }

        public bool Existe(string codigoClave, int idIdioma)
        {
            var dt = Gestor.EjecutarQuery(
                "SELECT COUNT(1) FROM TraduccionTable WHERE codigo_clave_traduccion = @clave AND id_idioma = @idioma",
                new SqlParameter("@clave", codigoClave),
                new SqlParameter("@idioma", idIdioma));

            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        private void InsertarLote(List<Traduccion_TE> lote)
        {
            var sql = new StringBuilder("INSERT INTO TraduccionTable (codigo_clave_traduccion, id_idioma, texto_traduccion, pendiente_traduccion, origen_traduccion) VALUES ");
            var parametros = new List<SqlParameter>();

            for (int i = 0; i < lote.Count; i++)
            {
                if (i > 0) sql.Append(", ");

                sql.Append("(@c" + i + ", @i" + i + ", @t" + i + ", @p" + i + ", @o" + i + ")");

                parametros.Add(new SqlParameter("@c" + i, lote[i].CodigoClave));
                parametros.Add(new SqlParameter("@i" + i, lote[i].IdIdioma));
                parametros.Add(new SqlParameter("@t" + i, ValorONulo(lote[i].Texto)));
                parametros.Add(new SqlParameter("@p" + i, lote[i].Pendiente));
                parametros.Add(new SqlParameter("@o" + i, lote[i].Origen.ToString().ToLowerInvariant()));
            }

            Gestor.EjecutarNonQuery(sql.ToString(), parametros.ToArray());
        }

        private static string SqlSeleccion()
        {
            return @"
                SELECT codigo_clave_traduccion, id_idioma, texto_traduccion, pendiente_traduccion, origen_traduccion
                FROM TraduccionTable";
        }

        private static List<Traduccion_TE> MapTodos(DataTable dt)
        {
            var lista = new List<Traduccion_TE>();
            foreach (DataRow row in dt.Rows) lista.Add(Map(row));
            return lista;
        }

        private static Traduccion_TE Map(DataRow dr)
        {
            return new Traduccion_TE
            {
                CodigoClave = Valor<string>(dr, "codigo_clave_traduccion"),
                IdIdioma = Valor<int>(dr, "id_idioma"),
                Texto = Valor<string>(dr, "texto_traduccion"),
                Pendiente = Valor<bool>(dr, "pendiente_traduccion"),
                Origen = Valor<OrigenTraduccion>(dr, "origen_traduccion")
            };
        }
    }
}
