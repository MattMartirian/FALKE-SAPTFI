using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TE;

namespace ORM
{
    // El registro de las copias hechas (BackupTable). El archivo en sí lo maneja Respaldos_ORM.
    public class RespaldoRepository : RepositoryBase<Respaldo_TE, int>
    {
        public RespaldoRepository() : base() { }

        // Si el usuario ya no existe en la base (por ejemplo, después de restaurar una copia anterior a su alta) queda sin usuario.
        public override void Alta(Respaldo_TE r)
        {
            const string sql = @"
                INSERT INTO BackupTable (id_usuario, fecha_generacion_backup, ruta_backup, tamano_backup)
                VALUES ((SELECT id_usuario FROM UsuarioTable WHERE id_usuario = @usuario), @fecha, @ruta, @tamano);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            r.IdRespaldo = Gestor.EjecutarScalar<int>(sql,
                new SqlParameter("@usuario", r.IdUsuario.HasValue ? (object)r.IdUsuario.Value : DBNull.Value),
                new SqlParameter("@fecha", r.FechaGeneracion),
                new SqlParameter("@ruta", r.Ruta),
                new SqlParameter("@tamano", r.Tamano.HasValue ? (object)r.Tamano.Value : DBNull.Value));
        }

        public override void Modificar(Respaldo_TE entidad)
        {
            throw new NotSupportedException("Un respaldo no se modifica: es el registro de una copia ya hecha.");
        }

        public override Respaldo_TE ObtenerPorPK(int pk)
        {
            var dt = Gestor.EjecutarQuery(SqlSeleccion() + " WHERE b.id_backup = @id", new SqlParameter("@id", pk));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        // Del más reciente al más antiguo.
        public override List<Respaldo_TE> ObtenerTodos()
        {
            var lista = new List<Respaldo_TE>();
            foreach (DataRow row in Gestor.EjecutarQuery(SqlSeleccion() + " ORDER BY b.fecha_generacion_backup DESC, b.id_backup DESC").Rows) lista.Add(Map(row));
            return lista;
        }

        public bool ExisteRuta(string ruta)
        {
            var dt = Gestor.EjecutarQuery("SELECT COUNT(1) FROM BackupTable WHERE ruta_backup = @ruta", new SqlParameter("@ruta", ruta));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        private static string SqlSeleccion()
        {
            return @"
                SELECT b.id_backup, b.id_usuario, b.fecha_generacion_backup, b.ruta_backup, b.tamano_backup, b.DVH,
                       LTRIM(RTRIM(COALESCE(u.nombre_usuario, N'') + N' ' + COALESCE(u.apellido_usuario, N''))) AS nombre_usuario
                FROM BackupTable b
                LEFT JOIN UsuarioTable u ON u.id_usuario = b.id_usuario";
        }

        private static Respaldo_TE Map(DataRow dr)
        {
            return new Respaldo_TE
            {
                IdRespaldo = Valor<int>(dr, "id_backup"),
                IdUsuario = Valor<int?>(dr, "id_usuario"),
                FechaGeneracion = Valor<DateTime>(dr, "fecha_generacion_backup"),
                Ruta = Valor<string>(dr, "ruta_backup"),
                Tamano = Valor<long?>(dr, "tamano_backup"),
                DVH = Valor<string>(dr, "DVH"),
                NombreUsuario = Valor<string>(dr, "nombre_usuario")
            };
        }
    }
}
