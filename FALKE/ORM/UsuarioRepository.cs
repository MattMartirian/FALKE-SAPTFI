using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DAL;
using TE;

namespace ORM
{
    public class UsuarioRepository : RepositoryBase<Usuario_TE, int>
    {
        public UsuarioRepository() : base() { }

        public override void Alta(Usuario_TE u)
        {
            string sql = @"
                INSERT INTO UsuarioTable
                    (id_empresa, nombre_usuario, apellido_usuario, email_usuario,
                     contrasena_hash_usuario, id_permiso, estado_usuario,
                     intentos_fallidos_usuario, id_idioma)
                VALUES
                    (@idEmpresa, @nombre, @apellido, @email,
                     @hash, @idPermiso, @estado,
                     @intentos, @idIdioma);
                SELECT SCOPE_IDENTITY();";

            var idGenerado = Gestor.EjecutarScalar<decimal>(sql,
                new SqlParameter("@idEmpresa", u.IdEmpresa),
                new SqlParameter("@nombre", ValorONulo(u.NombreUsuario)),
                new SqlParameter("@apellido", ValorONulo(u.ApellidoUsuario)),
                new SqlParameter("@email", ValorONulo(u.EmailUsuario)),
                new SqlParameter("@hash", ValorONulo(u.ContrasenaHashUsuario)),
                new SqlParameter("@idPermiso", u.IdPermiso),
                new SqlParameter("@estado", (int)u.Estado),
                new SqlParameter("@intentos", u.IntentosFallidosUsuario),
                new SqlParameter("@idIdioma", u.IdIdioma)
            );

            u.IdUsuario = Convert.ToInt32(idGenerado);

            //TODO : Se podría calcular el DVH aquí mismo (llamar al gestor para que lo haga el desde aca), hay que revisarlo
            // Nota: el DVH del registro recién creado NO se calcula acá.
            // Es responsabilidad de la capa superior (TLL) llamar a
            // GestorIntegridad.ActualizarDVHRegistro luego del alta.
        }

        public override void Modificar(Usuario_TE u)
        {
            string sql = @"
                UPDATE UsuarioTable SET
                    id_empresa = @idEmpresa,
                    nombre_usuario = @nombre,
                    apellido_usuario = @apellido,
                    email_usuario = @email,
                    contrasena_hash_usuario = @hash,
                    id_permiso = @idPermiso,
                    estado_usuario = @estado,
                    intentos_fallidos_usuario = @intentos,
                    id_idioma = @idIdioma
                WHERE id_usuario = @id";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@id", u.IdUsuario),
                new SqlParameter("@idEmpresa", u.IdEmpresa),
                new SqlParameter("@nombre", ValorONulo(u.NombreUsuario)),
                new SqlParameter("@apellido", ValorONulo(u.ApellidoUsuario)),
                new SqlParameter("@email", ValorONulo(u.EmailUsuario)),
                new SqlParameter("@hash", ValorONulo(u.ContrasenaHashUsuario)),
                new SqlParameter("@idPermiso", u.IdPermiso),
                new SqlParameter("@estado", (int)u.Estado),
                new SqlParameter("@intentos", u.IntentosFallidosUsuario),
                new SqlParameter("@idIdioma", u.IdIdioma)
            );
        }

        public override Usuario_TE ObtenerPorPK(int pk)
        {
            string sql = "SELECT * FROM UsuarioTable WHERE id_usuario = @id";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@id", pk));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<Usuario_TE> ObtenerTodos()
        {
            string sql = "SELECT * FROM UsuarioTable ORDER BY apellido_usuario, nombre_usuario";
            return MapTodos(Gestor.EjecutarQuery(sql));
        }

        public Usuario_TE ObtenerPorEmail(string email)
        {
            string sql = "SELECT * FROM UsuarioTable WHERE email_usuario = @email";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@email", email));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public List<Usuario_TE> ObtenerPorEmpresa(int idEmpresa)
        {
            string sql = @"
                SELECT * FROM UsuarioTable
                WHERE id_empresa = @idEmpresa
                ORDER BY apellido_usuario, nombre_usuario";

            return MapTodos(Gestor.EjecutarQuery(sql, new SqlParameter("@idEmpresa", idEmpresa)));
        }

        public void ActualizarIntentosFallidos(int idUsuario, int intentos)
        {
            string sql = "UPDATE UsuarioTable SET intentos_fallidos_usuario = @intentos WHERE id_usuario = @id";
            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@id", idUsuario),
                new SqlParameter("@intentos", intentos));
        }

        public void ActualizarEstado(int idUsuario, int estado)
        {
            string sql = "UPDATE UsuarioTable SET estado_usuario = @estado WHERE id_usuario = @id";
            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@id", idUsuario),
                new SqlParameter("@estado", estado));
        }

        #region Mapping

        private static Usuario_TE Map(DataRow dr)
        {
            return new Usuario_TE
            {
                IdUsuario = Valor<int>(dr, "id_usuario"),
                IdEmpresa = Valor<int>(dr, "id_empresa"),
                NombreUsuario = Valor<string>(dr, "nombre_usuario"),
                ApellidoUsuario = Valor<string>(dr, "apellido_usuario"),
                EmailUsuario = Valor<string>(dr, "email_usuario"),
                ContrasenaHashUsuario = Valor<string>(dr, "contrasena_hash_usuario"),
                IdPermiso = Valor<int>(dr, "id_permiso"),
                Estado = Valor<EstadoUsuario>(dr, "estado_usuario"),
                IntentosFallidosUsuario = Valor<int>(dr, "intentos_fallidos_usuario"),
                IdIdioma = Valor<int>(dr, "id_idioma"),
                DVH = Valor<string>(dr, "DVH")
            };
        }

        private static List<Usuario_TE> MapTodos(DataTable dt)
        {
            var lista = new List<Usuario_TE>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(Map(row));
            }
            return lista;
        }

        #endregion
    }
}