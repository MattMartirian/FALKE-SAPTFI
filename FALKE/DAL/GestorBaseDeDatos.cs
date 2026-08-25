using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public sealed class GestorBaseDeDatos
    {
        #region Singleton

        private static readonly Lazy<GestorBaseDeDatos> _instancia = new Lazy<GestorBaseDeDatos>(() => new GestorBaseDeDatos());

        public static GestorBaseDeDatos Instancia => _instancia.Value;

        #endregion

        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["FalkeDB"].ConnectionString;

        private SqlConnection CrearConexion()
        {
            return new SqlConnection(_connectionString);
        }

        #region Helpers SQL

        public DataTable EjecutarQuery(string sql, params SqlParameter[] parametros)
        {
            using (var con = CrearConexion())
            {
                con.Open();
   
                using (var cmd = new SqlCommand(sql, con))
                {
                    if (parametros != null && parametros.Length > 0) cmd.Parameters.AddRange(parametros);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public int EjecutarNonQuery(string sql, params SqlParameter[] parametros)
        {
            using (var con = CrearConexion())
            {
                con.Open();

                using (var cmd = new SqlCommand(sql, con))
                {
                    if (parametros != null && parametros.Length > 0) cmd.Parameters.AddRange(parametros);

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public T EjecutarScalar<T>(string sql, params SqlParameter[] parametros)
        {
            using (var con = CrearConexion())
            {
                con.Open();

                using (var cmd = new SqlCommand(sql, con))
                {
                    if (parametros != null && parametros.Length > 0)  cmd.Parameters.AddRange(parametros);

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value) return default(T);

                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
        }

        #endregion
    }
}