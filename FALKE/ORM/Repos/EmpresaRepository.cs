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
            string sql = @"
                INSERT INTO EmpresaClienteTable
                    (nombre_empresa, num_contacto_empresa,
                     plan_suscripcion_empresa, fecha_alta_empresa, estado_empresa,
                     cuit_empresa, rubro_empresa, domicilio_empresa, facturacion_empresa, fecha_renovacion_empresa)
                VALUES
                    (@nombre, @contacto, @plan, @fechaAlta, @estado,
                     @cuit, @rubro, @domicilio, @facturacion, @renovacion);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var idGenerado = Gestor.EjecutarScalar<int>(sql, ParametrosDatos(e));

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
                    estado_empresa = @estado,
                    cuit_empresa = @cuit,
                    rubro_empresa = @rubro,
                    domicilio_empresa = @domicilio,
                    facturacion_empresa = @facturacion,
                    fecha_renovacion_empresa = @renovacion
                WHERE id_empresa = @id";

            var parametros = new List<SqlParameter>(ParametrosDatos(e));
            parametros.Add(new SqlParameter("@id", e.IdEmpresa));

            Gestor.EjecutarNonQuery(sql, parametros.ToArray());
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

        public List<Empresa_BE> ObtenerResumen()
        {
            return ConsultarResumen(null);
        }

        public Empresa_BE ObtenerResumenPorId(int idEmpresa)
        {
            var lista = ConsultarResumen(idEmpresa);

            return lista.Count == 0 ? null : lista[0];
        }

        private List<Empresa_BE> ConsultarResumen(int? idEmpresa)
        {
            string sql = @"
                SELECT e.*,
                       (SELECT COUNT(1) FROM UsuarioTable u WHERE u.id_empresa = e.id_empresa) AS cant_usuarios,
                       (SELECT COUNT(1) FROM PrestamoTable p WHERE p.id_empresa = e.id_empresa AND p.fecha_devolucion_prestamo IS NULL) AS cant_dispositivos,
                       (SELECT COUNT(1) FROM SesionGrabadaTable s
                            INNER JOIN CategoriaTable c ON c.id_categoria = s.id_categoria
                            WHERE c.id_empresa = e.id_empresa) AS cant_sesiones
                FROM EmpresaClienteTable e
                WHERE (@id IS NULL OR e.id_empresa = @id)
                ORDER BY e.nombre_empresa";

            var lista = new List<Empresa_BE>();

            var tabla = Gestor.EjecutarQuery(sql, new SqlParameter("@id", idEmpresa.HasValue ? (object)idEmpresa.Value : DBNull.Value));

            foreach (DataRow row in tabla.Rows)
            {
                Empresa_BE empresa = Map(row);
                empresa.CantidadUsuarios = Valor<int>(row, "cant_usuarios");
                empresa.DispositivosPrestados = Valor<int>(row, "cant_dispositivos");
                empresa.SesionesGrabadas = Valor<int>(row, "cant_sesiones");
                lista.Add(empresa);
            }

            return lista;
        }

        public bool ExisteNombre(string nombre, int excluirIdEmpresa = 0)
        {
            string sql = "SELECT COUNT(1) FROM EmpresaClienteTable WHERE nombre_empresa = @nombre AND id_empresa <> @excluir";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@nombre", nombre), new SqlParameter("@excluir", excluirIdEmpresa));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        public bool ExisteCuit(string cuit, int excluirIdEmpresa = 0)
        {
            string sql = "SELECT COUNT(1) FROM EmpresaClienteTable WHERE cuit_empresa = @cuit AND id_empresa <> @excluir";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@cuit", cuit), new SqlParameter("@excluir", excluirIdEmpresa));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        #region Mapping

        private static string Normalizar(Enum valor)
        {
            return valor.ToString().ToLowerInvariant();
        }

        private static SqlParameter[] ParametrosDatos(Empresa_BE e)
        {
            return new[]
            {
                new SqlParameter("@nombre", ValorONulo(e.NombreEmpresa)),
                new SqlParameter("@contacto", ValorONulo(e.NumContactoEmpresa)),
                new SqlParameter("@plan", Normalizar(e.PlanSuscripcion)),
                new SqlParameter("@fechaAlta", e.FechaAlta),
                new SqlParameter("@estado", Normalizar(e.Estado)),
                new SqlParameter("@cuit", ValorONulo(e.Cuit)),
                new SqlParameter("@rubro", ValorONulo(e.Rubro)),
                new SqlParameter("@domicilio", ValorONulo(e.Domicilio)),
                new SqlParameter("@facturacion", e.Facturacion.HasValue ? (object)Normalizar(e.Facturacion.Value) : DBNull.Value),
                new SqlParameter("@renovacion", e.FechaRenovacion.HasValue ? (object)e.FechaRenovacion.Value : DBNull.Value)
            };
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
                Cuit = Valor<string>(dr, "cuit_empresa"),
                Rubro = Valor<string>(dr, "rubro_empresa"),
                Domicilio = Valor<string>(dr, "domicilio_empresa"),
                Facturacion = Valor<CicloFacturacion?>(dr, "facturacion_empresa"),
                FechaRenovacion = Valor<DateTime?>(dr, "fecha_renovacion_empresa"),
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
