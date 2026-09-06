using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace ORM
{
    public class EmpresaRepository : RepositoryBase<Empresa_BE, int>
    {
        public EmpresaRepository() : base() { }

        public override void Alta(Empresa_BE e)
        {
            //TODO: Cambiarlo a IDENTITY en la base de datos y usar SCOPE_IDENTITY() para obtener el ID generado.
            // id_empresa no es IDENTITY en la base: se calcula el proximo como en TokenTable.
            //TODO: si se define id_empresa como IDENTITY, cambiar el INSERT por VALUES + SELECT SCOPE_IDENTITY().
            string sql = @"
                INSERT INTO EmpresaClienteTable
                    (id_empresa, nombre_empresa, num_contacto_empresa,
                     plan_suscripcion_empresa, fecha_alta_empresa, estado_empresa)
                SELECT
                    ISNULL(MAX(id_empresa), 0) + 1, @nombre, @contacto,
                    @plan, @fechaAlta, @estado
                FROM EmpresaClienteTable;
                SELECT ISNULL(MAX(id_empresa), 0) FROM EmpresaClienteTable;";

            var idGenerado = Gestor.EjecutarScalar<int>(sql,
                new SqlParameter("@nombre", ValorONulo(e.NombreEmpresa)),
                new SqlParameter("@contacto", ValorONulo(e.NumContactoEmpresa)),
                new SqlParameter("@plan", Normalizar(e.PlanSuscripcion)),
                new SqlParameter("@fechaAlta", e.FechaAlta),
                new SqlParameter("@estado", Normalizar(e.Estado))
            );

            e.IdEmpresa = idGenerado;
        }

        public override void Modificar(Empresa_BE e)
        {
            string sql = @"
                UPDATE EmpresaClienteTable SET
                    nombre_empresa = @nombre,
                    num_contacto_empresa = @contacto,
                    plan_suscripcion_empresa = @plan,
                    fecha_alta_empresa = @fechaAlta,
                    estado_empresa = @estado
                WHERE id_empresa = @id";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@id", e.IdEmpresa),
                new SqlParameter("@nombre", ValorONulo(e.NombreEmpresa)),
                new SqlParameter("@contacto", ValorONulo(e.NumContactoEmpresa)),
                new SqlParameter("@plan", Normalizar(e.PlanSuscripcion)),
                new SqlParameter("@fechaAlta", e.FechaAlta),
                new SqlParameter("@estado", Normalizar(e.Estado))
            );
        }

        public override Empresa_BE ObtenerPorPK(int pk)
        {
            string sql = "SELECT * FROM EmpresaClienteTable WHERE id_empresa = @id";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@id", pk));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<Empresa_BE> ObtenerTodos()
        {
            string sql = "SELECT * FROM EmpresaClienteTable ORDER BY nombre_empresa";
            return MapTodos(Gestor.EjecutarQuery(sql));
        }

        public bool ExisteNombre(string nombre)
        {
            string sql = "SELECT COUNT(1) FROM EmpresaClienteTable WHERE nombre_empresa = @nombre";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@nombre", nombre));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        #region Mapping

        private static string Normalizar(Enum valor)
        {
            return valor.ToString().ToLowerInvariant();
        }

        private static Empresa_BE Map(DataRow dr)
        {
            return new Empresa_BE
            {
                IdEmpresa = Valor<int>(dr, "id_empresa"),
                NombreEmpresa = Valor<string>(dr, "nombre_empresa"),
                NumContactoEmpresa = Valor<string>(dr, "num_contacto_empresa"),
                PlanSuscripcion = Valor<PlanSuscripcion>(dr, "plan_suscripcion_empresa"),
                FechaAlta = Valor<DateTime>(dr, "fecha_alta_empresa"),
                Estado = Valor<EstadoEmpresa>(dr, "estado_empresa"),
                DVH = Valor<string>(dr, "DVH")
            };
        }

        private static List<Empresa_BE> MapTodos(DataTable dt)
        {
            var lista = new List<Empresa_BE>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(Map(row));
            }
            return lista;
        }

        #endregion
    }
}
