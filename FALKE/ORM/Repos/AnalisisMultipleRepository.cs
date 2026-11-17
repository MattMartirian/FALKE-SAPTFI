using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace ORM
{
    public class AnalisisMultipleRepository : RepositoryBase<AnalisisMultiple_BE, int>
    {
        public AnalisisMultipleRepository() : base() { }

        public override void Alta(AnalisisMultiple_BE a)
        {
            const string sql = @"
                INSERT INTO AnalisisMultipleTable (id_empresa, id_usuario, fecha_creacion_analisis_multiple)
                VALUES (@idEmpresa, @idUsuario, @fecha);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            a.IdAnalisis = Gestor.EjecutarScalar<int>(sql,
                new SqlParameter("@idEmpresa", a.IdEmpresa),
                new SqlParameter("@idUsuario", a.IdUsuario),
                new SqlParameter("@fecha", a.Fecha));

            foreach (int idSesion in a.IdSesiones)
            {
                Gestor.EjecutarNonQuery(
                    "INSERT INTO AnalisisMultiple_SesionTable (id_analisis_multiple, id_sesion) VALUES (@idAnalisis, @idSesion)",
                    new SqlParameter("@idAnalisis", a.IdAnalisis),
                    new SqlParameter("@idSesion", idSesion));
            }
        }

        public override void Modificar(AnalisisMultiple_BE a)
        {
            throw new NotSupportedException();
        }

        public override AnalisisMultiple_BE ObtenerPorPK(int pk)
        {
            var dt = Gestor.EjecutarQuery("SELECT * FROM AnalisisMultipleTable WHERE id_analisis_multiple = @id", new SqlParameter("@id", pk));
            if (dt.Rows.Count == 0) return null;

            AnalisisMultiple_BE analisis = Map(dt.Rows[0]);
            analisis.IdSesiones = IdsDeSesiones(pk);
            return analisis;
        }

        public override List<AnalisisMultiple_BE> ObtenerTodos()
        {
            var lista = new List<AnalisisMultiple_BE>();
            var dt = Gestor.EjecutarQuery("SELECT * FROM AnalisisMultipleTable ORDER BY fecha_creacion_analisis_multiple DESC");

            foreach (DataRow row in dt.Rows)
            {
                AnalisisMultiple_BE analisis = Map(row);
                analisis.IdSesiones = IdsDeSesiones(analisis.IdAnalisis);
                lista.Add(analisis);
            }

            return lista;
        }

        private List<int> IdsDeSesiones(int idAnalisis)
        {
            var ids = new List<int>();
            var dt = Gestor.EjecutarQuery(
                "SELECT id_sesion FROM AnalisisMultiple_SesionTable WHERE id_analisis_multiple = @id ORDER BY id_sesion",
                new SqlParameter("@id", idAnalisis));

            foreach (DataRow row in dt.Rows) ids.Add(Valor<int>(row, "id_sesion"));

            return ids;
        }

        private static AnalisisMultiple_BE Map(DataRow dr)
        {
            return new AnalisisMultiple_BE
            {
                IdAnalisis = Valor<int>(dr, "id_analisis_multiple"),
                IdEmpresa = Valor<int>(dr, "id_empresa"),
                IdUsuario = Valor<int>(dr, "id_usuario"),
                Fecha = Valor<DateTime>(dr, "fecha_creacion_analisis_multiple")
            };
        }
    }
}
