using System;
using System.Data.SqlClient;
using DAL;

namespace ORM
{
    public class TokenInfo
    {
        public int IdToken { get; set; }
        public int IdUsuario { get; set; }
        public string Tipo { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public bool Usado { get; set; }
    }

    public class TokenRepository
    {
        private readonly GestorBaseDeDatos Gestor;

        public TokenRepository()
        {
            Gestor = GestorBaseDeDatos.Instancia;
        }

        public void Crear(int idUsuario, string token, string tipo, DateTime creacion, DateTime expiracion)
        {
            const string sql = @"
                INSERT INTO TokenTable
                    (id_token, id_usuario, token, tipo_token, fecha_creacion, fecha_expiracion, usado)
                SELECT
                    ISNULL(MAX(id_token), 0) + 1, @idUsuario, @token, @tipo, @creacion, @expiracion, 0
                FROM TokenTable";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@idUsuario", idUsuario),
                new SqlParameter("@token", token),
                new SqlParameter("@tipo", (object)tipo ?? DBNull.Value),
                new SqlParameter("@creacion", creacion),
                new SqlParameter("@expiracion", expiracion));
        }

        public TokenInfo ObtenerPorToken(string token)
        {
            const string sql = @"
                SELECT id_token, id_usuario, tipo_token, fecha_expiracion, usado
                FROM TokenTable
                WHERE token = @token";

            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@token", token));
            if (dt.Rows.Count == 0) return null;

            var r = dt.Rows[0];
            return new TokenInfo
            {
                IdToken = Convert.ToInt32(r["id_token"]),
                IdUsuario = Convert.ToInt32(r["id_usuario"]),
                Tipo = r["tipo_token"] == DBNull.Value ? null : r["tipo_token"].ToString(),
                FechaExpiracion = r["fecha_expiracion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["fecha_expiracion"]),
                Usado = Convert.ToBoolean(r["usado"])
            };
        }

        public void MarcarUsado(int idToken)
        {
            Gestor.EjecutarNonQuery("UPDATE TokenTable SET usado = 1 WHERE id_token = @id",new SqlParameter("@id", idToken));
        }

        public void InvalidarPendientes(int idUsuario, string tipo)
        {
            Gestor.EjecutarNonQuery("UPDATE TokenTable SET usado = 1 WHERE id_usuario = @id AND tipo_token = @tipo AND usado = 0",new SqlParameter("@id", idUsuario),new SqlParameter("@tipo", tipo));
        }
    }
}
