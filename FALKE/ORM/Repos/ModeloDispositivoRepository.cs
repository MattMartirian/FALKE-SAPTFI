using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace ORM
{
    public class ModeloDispositivoRepository : RepositoryBase<ModeloDispositivo_BE, int>
    {
        public ModeloDispositivoRepository() : base() { }

        public override void Alta(ModeloDispositivo_BE m)
        {
            const string sql = @"
                INSERT INTO ModeloDispositivoTable (nombre_modelo_dispositivo, activo_modelo_dispositivo)
                VALUES (@nombre, @activo);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            m.IdModelo = Gestor.EjecutarScalar<int>(sql,
                new SqlParameter("@nombre", m.Nombre),
                new SqlParameter("@activo", m.Activo));
        }

        public override void Modificar(ModeloDispositivo_BE m)
        {
            Gestor.EjecutarNonQuery(@"
                UPDATE ModeloDispositivoTable SET
                    nombre_modelo_dispositivo = @nombre,
                    activo_modelo_dispositivo = @activo
                WHERE id_modelo_dispositivo = @id",
                new SqlParameter("@nombre", m.Nombre),
                new SqlParameter("@activo", m.Activo),
                new SqlParameter("@id", m.IdModelo));
        }

        public override ModeloDispositivo_BE ObtenerPorPK(int pk)
        {
            var dt = Gestor.EjecutarQuery(SqlSeleccion() + " WHERE m.id_modelo_dispositivo = @id", new SqlParameter("@id", pk));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<ModeloDispositivo_BE> ObtenerTodos()
        {
            var lista = new List<ModeloDispositivo_BE>();
            foreach (DataRow row in Gestor.EjecutarQuery(SqlSeleccion() + " ORDER BY m.nombre_modelo_dispositivo").Rows) lista.Add(Map(row));
            return lista;
        }

        // El nombre es único sin distinguir mayúsculas. excluirId permite renombrar un modelo sin chocar consigo mismo.
        public bool ExisteNombre(string nombre, int excluirId)
        {
            var dt = Gestor.EjecutarQuery(
                "SELECT COUNT(1) FROM ModeloDispositivoTable WHERE nombre_modelo_dispositivo = @nombre AND id_modelo_dispositivo <> @id",
                new SqlParameter("@nombre", nombre),
                new SqlParameter("@id", excluirId));

            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        private static string SqlSeleccion()
        {
            return @"
                SELECT m.id_modelo_dispositivo, m.nombre_modelo_dispositivo, m.activo_modelo_dispositivo, m.DVH,
                       (SELECT COUNT(1) FROM DispositivoTable d WHERE d.id_modelo_dispositivo = m.id_modelo_dispositivo) AS cantidad_dispositivos
                FROM ModeloDispositivoTable m";
        }

        private static ModeloDispositivo_BE Map(DataRow dr)
        {
            return new ModeloDispositivo_BE
            {
                IdModelo = Valor<int>(dr, "id_modelo_dispositivo"),
                Nombre = Valor<string>(dr, "nombre_modelo_dispositivo"),
                Activo = Valor<bool>(dr, "activo_modelo_dispositivo"),
                DVH = Valor<string>(dr, "DVH"),
                CantidadDispositivos = Valor<int>(dr, "cantidad_dispositivos")
            };
        }
    }
}
