using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace ORM
{
    public class PrestamoRepository : RepositoryBase<Prestamo_BE, int>
    {
        public PrestamoRepository() : base() { }

        public override void Alta(Prestamo_BE p)
        {
            const string sql = @"
                INSERT INTO PrestamoTable (id_dispositivo, id_empresa, fecha_entrega_prestamo, fecha_devolucion_prestamo)
                VALUES (@dispositivo, @empresa, @entrega, @devolucion);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            p.IdPrestamo = Gestor.EjecutarScalar<int>(sql,
                new SqlParameter("@dispositivo", p.IdDispositivo),
                new SqlParameter("@empresa", p.IdEmpresa),
                new SqlParameter("@entrega", p.FechaEntrega),
                new SqlParameter("@devolucion", ValorONulo(p.FechaDevolucion)));
        }

        // Un préstamo solo cambia al cerrarse: se le pone la fecha de devolución.
        public override void Modificar(Prestamo_BE p)
        {
            Gestor.EjecutarNonQuery("UPDATE PrestamoTable SET fecha_devolucion_prestamo = @devolucion WHERE id_prestamo = @id",
                new SqlParameter("@devolucion", ValorONulo(p.FechaDevolucion)),
                new SqlParameter("@id", p.IdPrestamo));
        }

        public override Prestamo_BE ObtenerPorPK(int pk)
        {
            var dt = Gestor.EjecutarQuery(SqlSeleccion() + " WHERE p.id_prestamo = @id", new SqlParameter("@id", pk));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<Prestamo_BE> ObtenerTodos()
        {
            return MapTodos(Gestor.EjecutarQuery(SqlSeleccion() + " ORDER BY p.fecha_entrega_prestamo DESC, p.id_prestamo DESC"));
        }

        public Prestamo_BE ObtenerAbierto(int idDispositivo)
        {
            var dt = Gestor.EjecutarQuery(SqlSeleccion() + " WHERE p.id_dispositivo = @id AND p.fecha_devolucion_prestamo IS NULL", new SqlParameter("@id", idDispositivo));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public List<Prestamo_BE> ObtenerHistorial(int idDispositivo)
        {
            return MapTodos(Gestor.EjecutarQuery(SqlSeleccion() + " WHERE p.id_dispositivo = @id ORDER BY p.fecha_entrega_prestamo DESC, p.id_prestamo DESC",
                new SqlParameter("@id", idDispositivo)));
        }

        public int ContarAbiertosDeEmpresa(int idEmpresa)
        {
            var dt = Gestor.EjecutarQuery("SELECT COUNT(1) FROM PrestamoTable WHERE id_empresa = @empresa AND fecha_devolucion_prestamo IS NULL",
                new SqlParameter("@empresa", idEmpresa));
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        #region Mapping

        private static string SqlSeleccion()
        {
            return @"
                SELECT p.id_prestamo, p.id_dispositivo, p.id_empresa, e.nombre_empresa,
                       p.fecha_entrega_prestamo, p.fecha_devolucion_prestamo, p.DVH
                FROM PrestamoTable p
                LEFT JOIN EmpresaClienteTable e ON e.id_empresa = p.id_empresa";
        }

        private static Prestamo_BE Map(DataRow dr)
        {
            return new Prestamo_BE
            {
                IdPrestamo = Valor<int>(dr, "id_prestamo"),
                IdDispositivo = Valor<int>(dr, "id_dispositivo"),
                IdEmpresa = Valor<int>(dr, "id_empresa"),
                NombreEmpresa = Valor<string>(dr, "nombre_empresa"),
                FechaEntrega = Valor<DateTime>(dr, "fecha_entrega_prestamo"),
                FechaDevolucion = Valor<DateTime?>(dr, "fecha_devolucion_prestamo"),
                DVH = Valor<string>(dr, "DVH")
            };
        }

        private static List<Prestamo_BE> MapTodos(DataTable dt)
        {
            var lista = new List<Prestamo_BE>();
            foreach (DataRow row in dt.Rows) lista.Add(Map(row));
            return lista;
        }

        #endregion
    }
}
