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

        public void ActualizarRol(int idUsuario, string nombreRol)
        {
            string sql = "UPDATE UsuarioTable SET rol_permiso = @rol WHERE id_usuario = @id";
            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@id", idUsuario),
                new SqlParameter("@rol", nombreRol));
        }

        public void ActualizarDatos(int idUsuario, string nombre, string apellido, int idIdioma, string email, int idEmpresa)
        {
            string sql = @"
                UPDATE UsuarioTable SET
                    nombre_usuario = @nombre,
                    apellido_usuario = @apellido,
                    id_idioma = @idIdioma,
                    email_usuario = @email,
                    id_empresa = @empresa
                WHERE id_usuario = @id";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@id", idUsuario),
                new SqlParameter("@nombre", ValorONulo(nombre)),
                new SqlParameter("@apellido", ValorONulo(apellido)),
                new SqlParameter("@idIdioma", idIdioma),
                new SqlParameter("@email", ValorONulo(email)),
                new SqlParameter("@empresa", idEmpresa));
        }

        public void ActualizarPerfil(int idUsuario, string nombre, string apellido, int idIdioma)
        {
            string sql = @"
                UPDATE UsuarioTable SET
                    nombre_usuario = @nombre,
                    apellido_usuario = @apellido,
                    id_idioma = @idIdioma
                WHERE id_usuario = @id";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@id", idUsuario),
                new SqlParameter("@nombre", ValorONulo(nombre)),
                new SqlParameter("@apellido", ValorONulo(apellido)),
                new SqlParameter("@idIdioma", idIdioma));
        }

        public int ContarAdministradoresActivos(int idEmpresa, string nombreRolAdministrador, int excluirIdUsuario)
        {
            string sql = @"
                SELECT COUNT(1) FROM UsuarioTable
                WHERE id_empresa = @idEmpresa
                  AND rol_permiso = @rol
                  AND estado_usuario = @activo
                  AND id_usuario <> @excluir";

            var dt = Gestor.EjecutarQuery(sql,
                new SqlParameter("@idEmpresa", idEmpresa),
                new SqlParameter("@rol", nombreRolAdministrador),
                new SqlParameter("@activo", ((int)EstadoUsuario.Activo).ToString()),
                new SqlParameter("@excluir", excluirIdUsuario));

            return Convert.ToInt32(dt.Rows[0][0]);
        }

        public string ObtenerNombreEmpresa(int idEmpresa)
        {
            var dt = Gestor.EjecutarQuery(
                "SELECT nombre_empresa FROM EmpresaClienteTable WHERE id_empresa = @id",
                new SqlParameter("@id", idEmpresa));

            return dt.Rows.Count == 0 ? null : Convert.ToString(dt.Rows[0][0]);
        }

        public string ObtenerEstadoEmpresa(int idEmpresa)
        {
            var dt = Gestor.EjecutarQuery(
                "SELECT estado_empresa FROM EmpresaClienteTable WHERE id_empresa = @id",
                new SqlParameter("@id", idEmpresa));

            return dt.Rows.Count == 0 ? null : Convert.ToString(dt.Rows[0][0]);
        }

        public bool ExisteEmpresa(int idEmpresa)
        {
            return ObtenerNombreEmpresa(idEmpresa) != null;
        }

        // El filtro por empresa lo fuerza la capa de negocio segun el permiso de la sesion: aca se aplica tal cual.
        public PaginaUsuarios_TE Listar(FiltroUsuarios_TE filtro)
        {
            int tamano = Math.Max(1, Math.Min(filtro.Tamano, FiltroUsuarios_TE.TAMANO_MAXIMO));
            int pagina = Math.Max(1, filtro.Pagina);

            const string desde = @"
                FROM UsuarioTable u
                LEFT JOIN EmpresaClienteTable e ON e.id_empresa = u.id_empresa
                WHERE (@idEmpresa IS NULL OR u.id_empresa = @idEmpresa)
                  AND (@rol IS NULL OR u.rol_permiso = @rol)
                  AND (@estado IS NULL OR u.estado_usuario = @estado)
                  AND (@texto IS NULL
                       OR u.nombre_usuario LIKE @texto ESCAPE '\'
                       OR u.apellido_usuario LIKE @texto ESCAPE '\'
                       OR u.email_usuario LIKE @texto ESCAPE '\')";

            Func<SqlParameter[]> parametros = () => new[]
            {
                new SqlParameter("@idEmpresa", filtro.IdEmpresa.HasValue ? (object)filtro.IdEmpresa.Value : DBNull.Value),
                new SqlParameter("@rol", string.IsNullOrWhiteSpace(filtro.Rol) ? DBNull.Value : (object)filtro.Rol.Trim()),
                new SqlParameter("@estado", filtro.Estado.HasValue ? (object)((int)filtro.Estado.Value).ToString() : DBNull.Value),
                new SqlParameter("@texto", string.IsNullOrWhiteSpace(filtro.Texto) ? DBNull.Value : (object)("%" + EscaparLike(filtro.Texto.Trim()) + "%"))
            };

            var cuenta = Gestor.EjecutarQuery("SELECT COUNT(1) " + desde, parametros());

            var pedido = new List<SqlParameter>(parametros());
            pedido.Add(new SqlParameter("@desplazamiento", (pagina - 1) * tamano));
            pedido.Add(new SqlParameter("@tamano", tamano));

            var dt = Gestor.EjecutarQuery(@"
                SELECT u.id_usuario, u.id_empresa, e.nombre_empresa, u.nombre_usuario, u.apellido_usuario,
                       u.email_usuario, u.id_idioma, u.rol_permiso, u.estado_usuario " + desde + @"
                ORDER BY e.nombre_empresa, u.apellido_usuario, u.nombre_usuario, u.id_usuario
                OFFSET @desplazamiento ROWS FETCH NEXT @tamano ROWS ONLY", pedido.ToArray());

            var resultado = new PaginaUsuarios_TE
            {
                Total = Convert.ToInt32(cuenta.Rows[0][0]),
                Pagina = pagina,
                Tamano = tamano
            };

            foreach (DataRow dr in dt.Rows)
            {
                resultado.Items.Add(new UsuarioListado_TE
                {
                    IdUsuario = Valor<int>(dr, "id_usuario"),
                    IdEmpresa = Valor<int>(dr, "id_empresa"),
                    NombreEmpresa = Valor<string>(dr, "nombre_empresa"),
                    NombreUsuario = Valor<string>(dr, "nombre_usuario"),
                    ApellidoUsuario = Valor<string>(dr, "apellido_usuario"),
                    EmailUsuario = NormalizarEmail(Valor<string>(dr, "email_usuario")),
                    IdIdioma = Valor<int>(dr, "id_idioma"),
                    Rol = Valor<string>(dr, "rol_permiso"),
                    Estado = Valor<EstadoUsuario>(dr, "estado_usuario")
                });
            }

            return resultado;
        }

        private static string EscaparLike(string texto)
        {
            return texto.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_").Replace("[", "\\[");
        }

        public Dictionary<string, int> ContarUsuariosPorRol()
        {
            var cuentas = new Dictionary<string, int>();

            foreach (DataRow row in Gestor.EjecutarQuery("SELECT rol_permiso, COUNT(1) AS n FROM UsuarioTable GROUP BY rol_permiso").Rows)
                cuentas[Convert.ToString(row["rol_permiso"])] = Convert.ToInt32(row["n"]);

            return cuentas;
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