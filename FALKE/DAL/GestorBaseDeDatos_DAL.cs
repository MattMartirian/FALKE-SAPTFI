using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public sealed class GestorBaseDeDatos_DAL
    {
        #region Singleton

        private static readonly Lazy<GestorBaseDeDatos_DAL> _instancia = new Lazy<GestorBaseDeDatos_DAL>(() => new GestorBaseDeDatos_DAL());

        public static GestorBaseDeDatos_DAL Instancia => _instancia.Value;

        #endregion

        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["FalkeDB"].ConnectionString;

        [ThreadStatic] private static SqlConnection conexionTransaccion;
        [ThreadStatic] private static SqlTransaction transaccion;
        [ThreadStatic] private static int nivelTransaccion;

        private SqlConnection CrearConexion()
        {
            return new SqlConnection(connectionString);
        }

        #region Transaccion

        public bool HayTransaccionActiva
        {
            get { return transaccion != null; }
        }

        /// <summary>
        /// Abre una transaccion. Si ya hay una abierta incrementa el nivel (transaccion anidada)
        /// el commit real ocurre cuando se cierra la mas externa.
        /// </summary>
        public void IniciarTransaccion()
        {
            if (transaccion != null)
            {
                nivelTransaccion++;
                return;
            }

            conexionTransaccion = CrearConexion();
            conexionTransaccion.Open();
            transaccion = conexionTransaccion.BeginTransaction();
            nivelTransaccion = 1;
        }

        /// <summary>Confirma. Solo hace COMMIT cuando se cierra la transaccion mas externa.</summary>
        public void Confirmar()
        {
            //TODO: Traducir.
            if (transaccion == null) throw new InvalidOperationException("No hay una transaccion activa para confirmar.");

            nivelTransaccion--;
            if (nivelTransaccion > 0) return;

            try
            {
                transaccion.Commit();
            }
            finally
            {
                LimpiarTransaccion();
            }
        }

        /// <summary>Revierte toda la transaccion sin importar el nivel de anidamiento.</summary>
        public void Revertir()
        {
            if (transaccion == null) return;

            try
            {
                transaccion.Rollback();
            }
            catch
            {
                // La conexion pudo haberse perdido: el motor ya habra descartado la transaccion.
            }
            finally
            {
                LimpiarTransaccion();
            }
        }

        private static void LimpiarTransaccion()
        {
            if (transaccion != null) transaccion.Dispose();

            if (conexionTransaccion != null) conexionTransaccion.Dispose();

            transaccion = null;
            conexionTransaccion = null;
            nivelTransaccion = 0;
        }

        public void AbortarTransaccion()
        {
            if (transaccion == null) return;

            try
            {
                transaccion.Rollback();
            }
            catch
            {
                // La conexion pudo haberse perdido: el motor ya habra descartado la transaccion.
            }
            finally
            {
                LimpiarTransaccion();
            }
        }

        #endregion

        #region Helpers SQL

        public DataTable EjecutarQuery(string sql, params SqlParameter[] parametros)
        {
            SqlConnection conexionPropia;
            var cmd = CrearComando(sql, parametros, out conexionPropia);

            try
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            finally
            {
                cmd.Dispose();
                if (conexionPropia != null) conexionPropia.Dispose();
            }
        }

        public int EjecutarNonQuery(string sql, params SqlParameter[] parametros)
        {
            SqlConnection conexionPropia;
            var cmd = CrearComando(sql, parametros, out conexionPropia);

            try
            {
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Dispose();
                if (conexionPropia != null) conexionPropia.Dispose();
            }
        }

        public T EjecutarScalar<T>(string sql, params SqlParameter[] parametros)
        {
            SqlConnection conexionPropia;
            var cmd = CrearComando(sql, parametros, out conexionPropia);

            try
            {
                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value) return default(T);

                return (T)Convert.ChangeType(result, typeof(T));
            }
            finally
            {
                cmd.Dispose();
                if (conexionPropia != null) conexionPropia.Dispose();
            }
        }

        private SqlCommand CrearComando(string sql, SqlParameter[] parametros, out SqlConnection conexionPropia)
        {
            SqlCommand cmd = null;
            conexionPropia = null;

            try
            {
                if (transaccion != null)
                {
                    cmd = new SqlCommand(sql, conexionTransaccion, transaccion);
                }
                else
                {
                    conexionPropia = CrearConexion();
                    conexionPropia.Open();
                    cmd = new SqlCommand(sql, conexionPropia);
                }

                if (parametros != null && parametros.Length > 0) cmd.Parameters.AddRange(parametros);

                return cmd;
            }
            catch
            {
                if (cmd != null) cmd.Dispose();
                if (conexionPropia != null) conexionPropia.Dispose();
                conexionPropia = null;
                throw;
            }
        }

        #endregion
    }
}
