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
        private readonly PermisoRepository permisoRepo;

        public UsuarioRepository() : base()
        {
            permisoRepo = new PermisoRepository();
        }

        public override void Alta(Usuario_TE u)
        {
            string sql = @"
                INSERT INTO UsuarioTable
                    (id_empresa, nombre_usuario, apellido_usuario, email_usuario,
                     contrasena_hash_usuario, rol_permiso, estado_usuario,
                     intentos_fallidos_usuario, id_idioma)
                VALUES
                    (@idEmpresa, @nombre, @apellido, @email,
                     @hash, @rolPermiso, @estado,
                     @intentos, @idIdioma);
                SELECT SCOPE_IDENTITY();";

            var idGenerado = Gestor.EjecutarScalar<decimal>(sql,
                new SqlParameter("@idEmpresa", u.IdEmpresa),
                new SqlParameter("@nombre", ValorONulo(u.NombreUsuario)),
                new SqlParameter("@apellido", ValorONulo(u.ApellidoUsuario)),
                new SqlParameter("@email", ValorONulo(u.EmailUsuario)),
                new SqlParameter("@hash", ValorONulo(u.ContrasenaHashUsuario)),
                new SqlParameter("@rolPermiso", u.Rol.Nombre),
                new SqlParameter("@estado", (int)u.Estado),
                new SqlParameter("@intentos", u.IntentosFallidosUsuario),
                new SqlParameter("@idIdioma", u.IdIdioma)
            );

            u.IdUsuario = Convert.ToInt32(idGenerado);
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
                    rol_permiso = @rolPermiso,
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
                new SqlParameter("@rolPermiso", u.Rol.Nombre),
                new SqlParameter("@estado", (int)u.Estado),
                new SqlParameter("@intentos", u.IntentosFallidosUsuario),
                new SqlParameter("@idIdioma", u.IdIdioma)
            );
        }

        public override Usuario_TE ObtenerPorPK(int pk)
        {
            string sql = "SELECT * FROM UsuarioTable WHERE id_usuario = @id";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@id", pk));
            return dt.Rows.Count == 0 ? null : MapConRolPropio(dt.Rows[0]);
        }

        public override List<Usuario_TE> ObtenerTodos()
        {
            string sql = "SELECT * FROM UsuarioTable ORDER BY apellido_usuario, nombre_usuario";
            return MapTodos(Gestor.EjecutarQuery(sql));
        }

        public Usuario_TE ObtenerPorEmail(string email)
        {
            string sql = "SELECT * FROM UsuarioTable WHERE email_usuario = @email";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@email", ValorONulo(email)));
            return dt.Rows.Count == 0 ? null : MapConRolPropio(dt.Rows[0]);
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
            Gestor.EjecutarNonQuery(sql, new SqlParameter("@id", idUsuario), new SqlParameter("@intentos", intentos));
        }

        public void ActualizarEstado(int idUsuario, int estado)
        {
            string sql = "UPDATE UsuarioTable SET estado_usuario = @estado WHERE id_usuario = @id";
            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@id", idUsuario),
                new SqlParameter("@estado", estado));
        }

        public bool ExisteUsuarioConRol(string nombrePermiso)
        {
            string sql = "SELECT COUNT(1) FROM UsuarioTable WHERE rol_permiso = @nombre";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@nombre", nombrePermiso));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        #region Mapping

        private static Usuario_TE Map(DataRow dr, PermisoCompuesto_TE rol)
        {
            return new Usuario_TE
            {
                IdUsuario = Valor<int>(dr, "id_usuario"),
                IdEmpresa = Valor<int>(dr, "id_empresa"),
                NombreUsuario = Valor<string>(dr, "nombre_usuario"),
                ApellidoUsuario = Valor<string>(dr, "apellido_usuario"),
                EmailUsuario = NormalizarEmail(Valor<string>(dr, "email_usuario")),
                ContrasenaHashUsuario = Valor<string>(dr, "contrasena_hash_usuario"),
                Rol = rol,
                Estado = Valor<EstadoUsuario>(dr, "estado_usuario"),
                IntentosFallidosUsuario = Valor<int>(dr, "intentos_fallidos_usuario"),
                IdIdioma = Valor<int>(dr, "id_idioma"),
                DVH = Valor<string>(dr, "DVH")
            };
        }

        private Usuario_TE MapConRolPropio(DataRow dr)
        {
            string nombreRol = Valor<string>(dr, "rol_permiso");
            return Map(dr, ResolverRol(nombreRol, permisoRepo.ConstruirArbolRol(nombreRol)));
        }

        private static string NormalizarEmail(string email)
        {
            return email == null ? null : email.Trim().ToLowerInvariant();
        }

        private static PermisoCompuesto_TE ResolverRol(string nombreRol, PermisoCompuesto_TE nodoResuelto)
        {
            if (string.IsNullOrEmpty(nombreRol)) return null;

            return nodoResuelto ?? new PermisoCompuesto_TE(nombreRol, true);
        }

        private static PermisoCompuesto_TE ResolverRol(string nombreRol, Dictionary<string, PermisoAbstracto_TE> arbolPermisos)
        {
            if (string.IsNullOrEmpty(nombreRol)) return null;

            PermisoAbstracto_TE nodo = null;
            if (arbolPermisos != null) arbolPermisos.TryGetValue(nombreRol, out nodo);

            return ResolverRol(nombreRol, nodo as PermisoCompuesto_TE);
        }

        private List<Usuario_TE> MapTodos(DataTable dt)
        {
            var arbolPermisos = permisoRepo.ConstruirArbol();
            var lista = new List<Usuario_TE>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(Map(row, ResolverRol(Valor<string>(row, "rol_permiso"), arbolPermisos)));
            }

            return lista;
        }

        #endregion
    }
}