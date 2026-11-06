using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace ORM
{
    public class DispositivoRepository : RepositoryBase<Dispositivo_BE, int>
    {
        public DispositivoRepository() : base() { }

        public override void Alta(Dispositivo_BE d)
        {
            const string sql = @"
                INSERT INTO DispositivoTable
                    (numero_serie_dispositivo, id_modelo_dispositivo, firmware_dispositivo, driver_requerido_dispositivo, estado_dispositivo)
                VALUES
                    (@serie, @modelo, @firmware, @driver, @estado);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            d.IdDispositivo = Gestor.EjecutarScalar<int>(sql,
                new SqlParameter("@serie", d.NumeroSerie),
                new SqlParameter("@modelo", d.IdModelo),
                new SqlParameter("@firmware", ValorONulo(d.Firmware)),
                new SqlParameter("@driver", ValorONulo(d.DriverRequerido)),
                new SqlParameter("@estado", EstadoABase(d.Estado)));
        }

        // El número de serie no se modifica: es la identidad del aparato.
        public override void Modificar(Dispositivo_BE d)
        {
            const string sql = @"
                UPDATE DispositivoTable SET
                    id_modelo_dispositivo = @modelo,
                    firmware_dispositivo = @firmware,
                    driver_requerido_dispositivo = @driver,
                    estado_dispositivo = @estado
                WHERE id_dispositivo = @id";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@modelo", d.IdModelo),
                new SqlParameter("@firmware", ValorONulo(d.Firmware)),
                new SqlParameter("@driver", ValorONulo(d.DriverRequerido)),
                new SqlParameter("@estado", EstadoABase(d.Estado)),
                new SqlParameter("@id", d.IdDispositivo));
        }

        public override Dispositivo_BE ObtenerPorPK(int pk)
        {
            var dt = Gestor.EjecutarQuery(SqlSeleccion() + " WHERE d.id_dispositivo = @id", new SqlParameter("@id", pk));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<Dispositivo_BE> ObtenerTodos()
        {
            return MapTodos(Gestor.EjecutarQuery(SqlSeleccion() + " ORDER BY d.numero_serie_dispositivo"));
        }

        // Los dispositivos que la empresa tiene hoy (préstamo abierto).
        public List<Dispositivo_BE> ObtenerDeEmpresa(int idEmpresa)
        {
            return MapTodos(Gestor.EjecutarQuery(SqlSeleccion() + " WHERE p.id_empresa = @empresa ORDER BY d.numero_serie_dispositivo",
                new SqlParameter("@empresa", idEmpresa)));
        }

        public bool ExisteSerie(string serie)
        {
            var dt = Gestor.EjecutarQuery("SELECT COUNT(1) FROM DispositivoTable WHERE numero_serie_dispositivo = @serie", new SqlParameter("@serie", serie));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        #region Mapping

        public static string EstadoABase(EstadoDispositivo estado)
        {
            switch (estado)
            {
                case EstadoDispositivo.EnUso: return "en_uso";
                case EstadoDispositivo.EnMantenimiento: return "en_mantenimiento";
                case EstadoDispositivo.DeBaja: return "de_baja";
                default: return "disponible";
            }
        }

        public static EstadoDispositivo EstadoDeBase(string valor)
        {
            switch ((valor ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "en_uso": return EstadoDispositivo.EnUso;
                case "en_mantenimiento": return EstadoDispositivo.EnMantenimiento;
                case "de_baja": return EstadoDispositivo.DeBaja;
                default: return EstadoDispositivo.Disponible;
            }
        }

        private static string SqlSeleccion()
        {
            return @"
                SELECT d.id_dispositivo, d.numero_serie_dispositivo, d.id_modelo_dispositivo, m.nombre_modelo_dispositivo, d.firmware_dispositivo,
                       d.driver_requerido_dispositivo, d.estado_dispositivo, d.DVH,
                       p.id_empresa AS id_empresa_actual, e.nombre_empresa AS empresa_actual, p.fecha_entrega_prestamo AS fecha_entrega_actual
                FROM DispositivoTable d
                JOIN ModeloDispositivoTable m ON m.id_modelo_dispositivo = d.id_modelo_dispositivo
                LEFT JOIN PrestamoTable p ON p.id_dispositivo = d.id_dispositivo AND p.fecha_devolucion_prestamo IS NULL
                LEFT JOIN EmpresaClienteTable e ON e.id_empresa = p.id_empresa";
        }

        private static Dispositivo_BE Map(DataRow dr)
        {
            return new Dispositivo_BE
            {
                IdDispositivo = Valor<int>(dr, "id_dispositivo"),
                NumeroSerie = Valor<string>(dr, "numero_serie_dispositivo"),
                IdModelo = Valor<int>(dr, "id_modelo_dispositivo"),
                Modelo = Valor<string>(dr, "nombre_modelo_dispositivo"),
                Firmware = Valor<string>(dr, "firmware_dispositivo"),
                DriverRequerido = Valor<string>(dr, "driver_requerido_dispositivo"),
                Estado = EstadoDeBase(Valor<string>(dr, "estado_dispositivo")),
                DVH = Valor<string>(dr, "DVH"),
                IdEmpresaActual = Valor<int?>(dr, "id_empresa_actual"),
                EmpresaActual = Valor<string>(dr, "empresa_actual"),
                FechaEntregaActual = Valor<DateTime?>(dr, "fecha_entrega_actual")
            };
        }

        private static List<Dispositivo_BE> MapTodos(DataTable dt)
        {
            var lista = new List<Dispositivo_BE>();
            foreach (DataRow row in dt.Rows) lista.Add(Map(row));
            return lista;
        }

        #endregion
    }
}
