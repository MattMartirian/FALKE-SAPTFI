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
                    (id_usuario, id_empresa, modulo_bitacora, descripcion_bitacora, criticidad_bitacora, fecha_hora_bitacora)
                VALUES
                    (@idUsuario,
                     COALESCE(@idEmpresa, (SELECT id_empresa FROM UsuarioTable WHERE id_usuario = @idUsuario)),
                     @modulo, @descripcion, @criticidad, @fecha)";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@idUsuario", b.IdUsuario > 0 ? (object)b.IdUsuario : DBNull.Value),
                new SqlParameter("@idEmpresa", b.IdEmpresa.HasValue ? (object)b.IdEmpresa.Value : DBNull.Value),
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

        // Los últimos eventos que hizo un usuario, del más reciente al más antiguo.
        public List<Bitacora_TE> ObtenerPorUsuario(int idUsuario, int maximo)
        {
            const string sql = @"
                SELECT TOP (@maximo) *
                FROM BitacoraTable
                WHERE id_usuario = @idUsuario
                ORDER BY fecha_hora_bitacora DESC, id_evento_bitacora DESC";

            return MapTodos(Gestor.EjecutarQuery(sql, new SqlParameter("@idUsuario", idUsuario), new SqlParameter("@maximo", maximo)));
        }

        // Con ocultarProveedor el actor de Pattern Blue sale como la etiqueta, sin nombre: se resuelve en la consulta, no en la pantalla.
        // Devuelve todo lo que el alcance permite ver; el filtrado y la paginación los hace la capa de negocio.
        public List<BitacoraVista_TE> ObtenerVista(int? idEmpresa, bool ocultarProveedor, int idEmpresaProveedor, string rolProveedor, string etiquetaProveedor)
        {
            var dt = Gestor.EjecutarQuery(@"
                SELECT b.id_evento_bitacora, b.fecha_hora_bitacora, b.modulo_bitacora, b.descripcion_bitacora,
                       b.criticidad_bitacora, b.id_empresa, e.nombre_empresa,
                       CASE
                           WHEN b.id_usuario IS NULL THEN N'Sistema'
                           WHEN @ocultar = 1 AND (u.rol_permiso = @rolProveedor OR u.id_empresa = @empresaProveedor) THEN @etiqueta
                           ELSE LTRIM(RTRIM(ISNULL(u.nombre_usuario, N'') + N' ' + ISNULL(u.apellido_usuario, N'')))
                       END AS actor,
                       CASE
                           WHEN b.id_usuario IS NULL THEN NULL
                           WHEN @ocultar = 1 AND (u.rol_permiso = @rolProveedor OR u.id_empresa = @empresaProveedor) THEN NULL
                           ELSE u.email_usuario
                       END AS email_actor
                FROM BitacoraTable b
                LEFT JOIN UsuarioTable u ON u.id_usuario = b.id_usuario
                LEFT JOIN EmpresaClienteTable e ON e.id_empresa = b.id_empresa
                WHERE (@idEmpresa IS NULL OR b.id_empresa = @idEmpresa)
                ORDER BY b.fecha_hora_bitacora DESC, b.id_evento_bitacora DESC",
                new SqlParameter("@idEmpresa", idEmpresa.HasValue ? (object)idEmpresa.Value : DBNull.Value),
                new SqlParameter("@ocultar", ocultarProveedor ? 1 : 0),
                new SqlParameter("@empresaProveedor", idEmpresaProveedor),
                new SqlParameter("@rolProveedor", rolProveedor),
                new SqlParameter("@etiqueta", etiquetaProveedor));

            var resultado = new List<BitacoraVista_TE>();

            foreach (DataRow dr in dt.Rows)
            {
                resultado.Add(new BitacoraVista_TE
                {
                    IdEventoBitacora = Valor<int>(dr, "id_evento_bitacora"),
                    FechaHoraBitacora = Valor<DateTime>(dr, "fecha_hora_bitacora"),
                    Actor = Valor<string>(dr, "actor"),
                    EmailActor = Valor<string>(dr, "email_actor"),
                    IdEmpresa = Valor<int?>(dr, "id_empresa"),
                    NombreEmpresa = Valor<string>(dr, "nombre_empresa"),
                    ModuloBitacora = Valor<string>(dr, "modulo_bitacora"),
                    DescripcionBitacora = Valor<string>(dr, "descripcion_bitacora"),
                    CriticidadBitacora = Valor<CriticidadBitacora>(dr, "criticidad_bitacora")
                });
            }

            return resultado;
        }

        #region Mapping

        private static Bitacora_TE Map(DataRow dr)
        {
            return new Bitacora_TE
            {
                IdEventoBitacora = Valor<int>(dr, "id_evento_bitacora"),
                IdUsuario = Valor<int>(dr, "id_usuario"),
                IdEmpresa = Valor<int?>(dr, "id_empresa"),
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