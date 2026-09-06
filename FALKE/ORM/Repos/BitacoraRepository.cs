using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DAL;
using TE;

namespace ORM
{
    public class BitacoraRepository : RepositoryBase<Bitacora_TE, int>
    {
        public BitacoraRepository(): base() { }

        public override void Alta(Bitacora_TE b)
        {
            string sql = @"
                INSERT INTO BitacoraTable
                    (id_usuario, modulo_bitacora, descripcion_bitacora, criticidad_bitacora, fecha_hora_bitacora)
                VALUES
                    (@idUsuario, @modulo, @descripcion, @criticidad, @fecha)";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@idUsuario", b.IdUsuario > 0 ? (object)b.IdUsuario : DBNull.Value),
                new SqlParameter("@modulo", ValorONulo(b.ModuloBitacora)),
                new SqlParameter("@descripcion", ValorONulo(b.DescripcionBitacora)),
                new SqlParameter("@criticidad", (int)b.CriticidadBitacora),
                new SqlParameter("@fecha", b.FechaHoraBitacora)
            );
        }

        public override void Modificar(Bitacora_TE b)
        {
            //TODO: Traducir.
            throw new NotSupportedException("La bitácora es de solo lectura: no se permite modificar eventos ya registrados.");
        }

        public override Bitacora_TE ObtenerPorPK(int pk)
        {
            string sql = @"
                SELECT *
                FROM BitacoraTable
                WHERE id_evento_bitacora = @id";

            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@id", pk));

            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<Bitacora_TE> ObtenerTodos()
        {
            string sql = @"
                SELECT *
                FROM BitacoraTable
                ORDER BY fecha_hora_bitacora DESC";

            return MapTodos(Gestor.EjecutarQuery(sql));
        }

        public List<Bitacora_TE> ObtenerPorUsuario(int idUsuario)
        {
            const string sql = @"
                SELECT *
                FROM BitacoraTable
                WHERE id_usuario = @idUsuario
                ORDER BY fecha_hora_bitacora DESC";

            return MapTodos(Gestor.EjecutarQuery(sql, new SqlParameter("@idUsuario", idUsuario)));
        }

        #region Mapping

        private static Bitacora_TE Map(DataRow dr)
        {
            return new Bitacora_TE
            {
                IdEventoBitacora = Valor<int>(dr, "id_evento_bitacora"),
                IdUsuario = Valor<int>(dr, "id_usuario"),
                ModuloBitacora = Valor<string>(dr, "modulo_bitacora"),
                DescripcionBitacora = Valor<string>(dr, "descripcion_bitacora"),
                CriticidadBitacora = Valor<CriticidadBitacora>(dr, "criticidad_bitacora"),
                FechaHoraBitacora = Valor<DateTime>(dr, "fecha_hora_bitacora")
            };
        }

        private static List<Bitacora_TE> MapTodos(DataTable dt)
        {
            var lista = new List<Bitacora_TE>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(Map(row));
            }

            return lista;
        }

        #endregion
    }
}