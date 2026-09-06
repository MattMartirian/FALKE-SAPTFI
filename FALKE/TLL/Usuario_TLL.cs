using ORM;
using SECURITY;
using SERVICES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Web;
using TE;

namespace TLL
{
    public class Usuario_TLL
    {
        private const int MAX_INTENTOS_FALLIDOS = 5;
        private const int LARGO_MINIMO_CONTRASENA = 8;

        public const string TOKEN_ACTIVACION = "activacion";
        public const string TOKEN_RECUPERACION = "recuperacion";

        public const string ROL_GESTOR = "Gestor";
        public const string ROL_ADMINISTRADOR = "Administrador";
        public const string ROL_USUARIO = "Usuario";

        private static readonly TimeSpan VIGENCIA_ACTIVACION = TimeSpan.FromHours(48);
        private static readonly TimeSpan VIGENCIA_RECUPERACION = TimeSpan.FromHours(2);

        private readonly UsuarioRepository usuarioRepo;
        private readonly TokenRepository tokenRepo;
        private readonly Cifrador_SECURITY cifrador;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;

        public Usuario_TLL()
        {
            usuarioRepo = new UsuarioRepository();
            tokenRepo = new TokenRepository();
            cifrador = Cifrador_SECURITY.CifradorSingleton;
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
        }

        public ResultadoLogin_TLL ValidarCredenciales(string email, string contrasenaPlana)
        {
            // creación del usuario de emergencia antes de pasar a consultas de BD
            if (EsCredencialDeEmergencia(email, contrasenaPlana))
            {
                var usuarioEmergencia = ConstruirUsuarioEmergenciaEnMemoria(email);
                LoguearAccesoEmergenciaAArchivo(email);
                //TODO: Traducir.
                bitacora.Registrar(0, "Seguridad", "Acceso de emergencia (break-glass) con identificador '" + email + "'", CriticidadBitacora.Alta);
                return ResultadoLogin_TLL.Exitoso(usuarioEmergencia);
            }

            email = NormalizarEmail(email);

            var usuario = usuarioRepo.ObtenerPorEmail(email);

            if (usuario == null) return ResultadoLogin_TLL.CredencialesInvalidas();

            if (usuario.Estado == EstadoUsuario.Bloqueado)
            {
                //TODO: Traducir.
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Intento de inicio de sesión sobre una cuenta bloqueada", CriticidadBitacora.Media);
                return ResultadoLogin_TLL.UsuarioBloqueado();
            }

            if (usuario.Estado == EstadoUsuario.Pendiente)
            {
                //TODO: Traducir.
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Intento de inicio de sesión sobre una cuenta pendiente de activación", CriticidadBitacora.Baja);
                return ResultadoLogin_TLL.UsuarioPendienteActivacion();
            }

            if (!VerificarContrasena(contrasenaPlana, usuario.ContrasenaHashUsuario))
            {
                RegistrarIntentoFallido(usuario);
                return ResultadoLogin_TLL.CredencialesInvalidas();
            }

            var inconsistencias = gestorIntegridad.VerificarIntegridadTodasLasTablas();
            if (inconsistencias.Count > 0)
            {
                // acá se va a decidir el nivel de detalle según el rol del usuario que intenta loguearse
                //TODO: Traducir.
                bitacora.Registrar(usuario.IdUsuario, "Integridad", "Inicio de sesión rechazado: la integridad de los datos está comprometida (" + inconsistencias.Count + " inconsistencia/s)", CriticidadBitacora.Alta);
                return ResultadoLogin_TLL.IntegridadComprometida();
            }

            Transaccion_ORM.Ejecutar(() =>
            {
                ResetearIntentosFallidos(usuario);
                //TODO: Traducir.
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Inicio de sesión exitoso", CriticidadBitacora.Baja);
            });

            return ResultadoLogin_TLL.Exitoso(usuario);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns>Token para validar usuario</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string RegistrarUsuario(Usuario_TE usuario)
        {
            usuario.EmailUsuario = NormalizarEmail(usuario.EmailUsuario);

            //TODO: Traducir.
            if (!EsEmailValido(usuario.EmailUsuario)) throw new InvalidOperationException("El email no tiene un formato valido.");

            //TODO: Traducir.
            if (usuarioRepo.ObtenerPorEmail(usuario.EmailUsuario) != null) throw new InvalidOperationException("Ya existe un usuario registrado con ese email.");

            usuario.ContrasenaHashUsuario = cifrador.Encoder(Cifrador_SECURITY.GenerarSecretoUrlSafe());
            usuario.IntentosFallidosUsuario = 0;
            usuario.Estado = EstadoUsuario.Pendiente;

            //TODO: Traducir.
            string rolNombre = usuario.Rol != null ? usuario.Rol.Nombre : "(sin rol)";

            string token = Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.Alta(usuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                string t = EmitirToken(usuario.IdUsuario, TOKEN_ACTIVACION, VIGENCIA_ACTIVACION);

                //TODO: Traducir.
                bitacora.Registrar(usuario.IdUsuario, "Usuarios", "Alta de usuario '" + usuario.EmailUsuario + "' (empresa " + usuario.IdEmpresa + ", rol " + rolNombre + "); queda pendiente de activación", CriticidadBitacora.Media);

                return t;
            });

            return token;
        }

        public void ActualizarDatosUsuario(Usuario_TE usuario)
        {
            usuario.EmailUsuario = NormalizarEmail(usuario.EmailUsuario);

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.Modificar(usuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                //TODO: Traducir.
                bitacora.Registrar(usuario.IdUsuario, "Usuarios", "Modificación de datos del usuario '" + usuario.EmailUsuario + "'", CriticidadBitacora.Baja);
            });
        }

        public List<Usuario_TE> ObtenerPorEmpresa(int idEmpresa) => usuarioRepo.ObtenerPorEmpresa(idEmpresa);

        public Usuario_TE ObtenerPorId(int idUsuario) => usuarioRepo.ObtenerPorPK(idUsuario);

        public Usuario_TE ObtenerPorEmail(string email) => usuarioRepo.ObtenerPorEmail(NormalizarEmail(email));

        public bool CambiarContrasena(string email, string contrasenaActual, string contrasenaNueva, out string error)
        {
            error = null;
            email = NormalizarEmail(email);

            var usuario = usuarioRepo.ObtenerPorEmail(email);

            if (usuario == null|| usuario.Estado == EstadoUsuario.Bloqueado|| string.IsNullOrEmpty(contrasenaActual)|| !VerificarContrasena(contrasenaActual, usuario.ContrasenaHashUsuario))
            {
                if (usuario != null && usuario.Estado != EstadoUsuario.Bloqueado)
                {
                    RegistrarIntentoFallido(usuario);
                }

                //TODO: Traducir.
                error = "No se pudo cambiar la contrasena. Verifique los datos ingresados.";
                return false;
            }

            if (!EsContrasenaAceptable(contrasenaNueva))
            {
                //TODO: Traducir.
                error = "La nueva contrasena debe tener al menos " + LARGO_MINIMO_CONTRASENA + " caracteres.";
                return false;
            }

            usuario.ContrasenaHashUsuario = cifrador.Encoder(contrasenaNueva);
            usuario.IntentosFallidosUsuario = 0;

            if (usuario.Estado == EstadoUsuario.Pendiente)
                usuario.Estado = EstadoUsuario.Activo;

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.Modificar(usuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                //TODO: Traducir.
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Cambio de contraseña", CriticidadBitacora.Media);
            });

            return true;
        }

        public string SolicitarRecuperacion(string email)
        {
            var usuario = usuarioRepo.ObtenerPorEmail(NormalizarEmail(email));
            if (usuario == null) return null;

            string token = null;

            Transaccion_ORM.Ejecutar(() =>
            {
                token = EmitirToken(usuario.IdUsuario, TOKEN_RECUPERACION, VIGENCIA_RECUPERACION);
                //TODO: Traducir.
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Solicitud de recuperación de contraseña", CriticidadBitacora.Baja);
            });

            return token;
        }

        public ResultadoToken_TLL ValidarTokenContrasena(string token)
        {
            return EvaluarToken(LeerToken(token));
        }

        public ResultadoToken_TLL EstablecerContrasenaConToken(string token, string contrasenaNueva)
        {
            var info = LeerToken(token);
            var validacion = EvaluarToken(info);
            if (!validacion.Exito) return validacion;

            if (!EsContrasenaAceptable(contrasenaNueva))
                return ResultadoToken_TLL.Falla("CONTRASENA_DEBIL", validacion.Email);

            var usuario = usuarioRepo.ObtenerPorPK(info.IdUsuario);

            usuario.ContrasenaHashUsuario = cifrador.Encoder(contrasenaNueva);
            usuario.IntentosFallidosUsuario = 0;

            if (usuario.Estado == EstadoUsuario.Pendiente || usuario.Estado == EstadoUsuario.Bloqueado)
                usuario.Estado = EstadoUsuario.Activo;

            //TODO: Traducir.
            string via = info.Tipo == TOKEN_ACTIVACION ? "activación" : "recuperación";

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.Modificar(usuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                tokenRepo.MarcarUsado(info.IdToken);
                tokenRepo.InvalidarPendientes(usuario.IdUsuario, info.Tipo);
                gestorIntegridad.RecalcularTabla(TablasBD.Token);

                //TODO: Traducir.
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Contraseña establecida mediante token de " + via + "; la cuenta queda activa", CriticidadBitacora.Media);
            });

            return ResultadoToken_TLL.Ok(usuario.EmailUsuario);
        }

        private TokenInfo LeerToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            return tokenRepo.ObtenerPorToken(token);
        }

        private ResultadoToken_TLL EvaluarToken(TokenInfo info)
        {
            if (info == null || (info.Tipo != TOKEN_ACTIVACION && info.Tipo != TOKEN_RECUPERACION))
            {
                return ResultadoToken_TLL.Falla("TOKEN_INVALIDO");
            }

            if (info.Usado)
            {
                return ResultadoToken_TLL.Falla("TOKEN_USADO");
            }

            if (info.FechaExpiracion.HasValue && info.FechaExpiracion.Value < DateTime.Now)
            {
                return ResultadoToken_TLL.Falla("TOKEN_EXPIRADO");
            }

            var usuario = usuarioRepo.ObtenerPorPK(info.IdUsuario);
            if (usuario == null) return ResultadoToken_TLL.Falla("TOKEN_INVALIDO");

            return ResultadoToken_TLL.Ok(usuario.EmailUsuario);
        }

        private string EmitirToken(int idUsuario, string tipo, TimeSpan vigencia)
        {
            string token = Cifrador_SECURITY.GenerarSecretoUrlSafe();
            var ahora = DateTime.Now;

            Transaccion_ORM.Ejecutar(() =>
            {
                tokenRepo.InvalidarPendientes(idUsuario, tipo);
                tokenRepo.Crear(idUsuario, token, tipo, ahora, ahora.Add(vigencia));
                gestorIntegridad.RecalcularTabla(TablasBD.Token);
            });

            return token;
        }

        private static bool EsContrasenaAceptable(string contrasena)
        {
            return !string.IsNullOrWhiteSpace(contrasena) && contrasena.Length >= LARGO_MINIMO_CONTRASENA;
        }

        private static string NormalizarEmail(string email)
        {
            return email == null ? null : email.Trim().ToLowerInvariant();
        }

        private static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            int arroba = email.IndexOf('@');
            if (arroba <= 0 || arroba != email.LastIndexOf('@')) return false;

            int punto = email.IndexOf('.', arroba);
            return punto > arroba + 1 && punto < email.Length - 1;
        }

        private void RegistrarIntentoFallido(Usuario_TE usuario)
        {
            usuario.IntentosFallidosUsuario++;

            bool seBloqueo = usuario.IntentosFallidosUsuario >= MAX_INTENTOS_FALLIDOS;
            if (seBloqueo) usuario.Estado = EstadoUsuario.Bloqueado;

            Transaccion_ORM.Ejecutar(() =>
            {
                if (seBloqueo) usuarioRepo.ActualizarEstado(usuario.IdUsuario, (int)usuario.Estado);

                usuarioRepo.ActualizarIntentosFallidos(usuario.IdUsuario, usuario.IntentosFallidosUsuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                if (seBloqueo)
                {
                    //TODO: Traducir.
                    bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Cuenta bloqueada por superar el máximo de intentos fallidos", CriticidadBitacora.Alta);
                }
            });
        }

        private void ResetearIntentosFallidos(Usuario_TE usuario)
        {
            if (usuario.IntentosFallidosUsuario == 0) return;

            usuario.IntentosFallidosUsuario = 0;

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarIntentosFallidos(usuario.IdUsuario, 0);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });
            });
        }

        private bool VerificarContrasena(string contrasenaPlana, string hashAlmacenado)
        {
            return cifrador.Encoder(contrasenaPlana) == hashAlmacenado;
        }

        private bool EsCredencialDeEmergencia(string identificador, string contrasenaPlana)
        {
            string usuarioConfigurado = ConfigurationManager.AppSettings["FALKE_EMERGENCY_USER"];
            string hashConfigurado = ConfigurationManager.AppSettings["FALKE_EMERGENCY_HASH"];

            if (string.IsNullOrEmpty(usuarioConfigurado) || string.IsNullOrEmpty(hashConfigurado)) return false;

            if (identificador != usuarioConfigurado) return false;

            return cifrador.Encoder(contrasenaPlana) == hashConfigurado;
        }

        private Usuario_TE ConstruirUsuarioEmergenciaEnMemoria(string identificador)
        {
            return new Usuario_TE
            {
                IdUsuario = -1,
                NombreUsuario = "EMERGENCIA",
                ApellidoUsuario = string.Empty,
                EmailUsuario = identificador,
                Estado = EstadoUsuario.Activo,
                EsCuentaEmergencia = true
            };
        }

        private void LoguearAccesoEmergenciaAArchivo(string identificador)
        {
            try
            {
                //TODO: Traducir.
                string linea = $"{DateTime.Now:o} | ACCESO DE EMERGENCIA | {identificador}";
                string ruta = HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/App_Data/emergencia.log") : "emergencia.log";

                File.AppendAllText(ruta, linea + Environment.NewLine);
            }
            catch
            {
                // Un fallo al escribir el log de emergencia no debe impedir el acceso de emergencia en sí.
            }
        }
    }
}