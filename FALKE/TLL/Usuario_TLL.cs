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
    public class UsuarioTLL
    {
        private const int MAX_INTENTOS_FALLIDOS = 5;
        private const int LARGO_MINIMO_CONTRASENA = 8;

        public const string TOKEN_ACTIVACION = "ACTIVACION";
        public const string TOKEN_RECUPERACION = "RECUPERACION";

        private static readonly TimeSpan VIGENCIA_ACTIVACION = TimeSpan.FromHours(48);
        private static readonly TimeSpan VIGENCIA_RECUPERACION = TimeSpan.FromHours(2);

        private readonly UsuarioRepository usuarioRepo;
        private readonly TokenRepository tokenRepo;
        private readonly Cifrador cifrador;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;

        public UsuarioTLL()
        {
            usuarioRepo = new UsuarioRepository();
            tokenRepo = new TokenRepository();
            cifrador = Cifrador.CypherInstance;
            gestorIntegridad = new GestorIntegridad_SERVICE();
        }

        public ResultadoLogin ValidarCredenciales(string email, string contrasenaPlana)
        {
            // creación del usuario de emergencia antes de pasar a consultas de BD
            if (EsCredencialDeEmergencia(email, contrasenaPlana))
            {
                var usuarioEmergencia = ConstruirUsuarioEmergenciaEnMemoria(email);
                LoguearAccesoEmergenciaAArchivo(email);
                return ResultadoLogin.Exitoso(usuarioEmergencia);
            }

            var inconsistencias = gestorIntegridad.VerificarIntegridadTodasLasTablas();
            if (inconsistencias.Count > 0)
            {
                // acá se va a decidir el nivel de detalle según el rol del usuario que intenta loguearse
                return ResultadoLogin.IntegridadComprometida();
            }

            var usuario = usuarioRepo.ObtenerPorEmail(email);

            if (usuario == null) return ResultadoLogin.CredencialesInvalidas();

            if (usuario.Estado == EstadoUsuario.Bloqueado) return ResultadoLogin.UsuarioBloqueado();

            if (usuario.Estado == EstadoUsuario.Pendiente) return ResultadoLogin.UsuarioPendienteActivacion();

            if (!VerificarContrasena(contrasenaPlana, usuario.ContrasenaHashUsuario))
            {
                RegistrarIntentoFallido(usuario);
                return ResultadoLogin.CredencialesInvalidas();
            }

            ResetearIntentosFallidos(usuario);

            return ResultadoLogin.Exitoso(usuario);
        }

        public string RegistrarUsuario(Usuario_TE usuario)
        {
            if (usuarioRepo.ObtenerPorEmail(usuario.EmailUsuario) != null) throw new InvalidOperationException("Ya existe un usuario registrado con ese email.");

            usuario.ContrasenaHashUsuario = cifrador.Encoder(Cifrador.GenerarSecretoUrlSafe());
            usuario.IntentosFallidosUsuario = 0;
            usuario.Estado = EstadoUsuario.Pendiente;

            usuarioRepo.Alta(usuario);
            gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

            return EmitirToken(usuario.IdUsuario, TOKEN_ACTIVACION, VIGENCIA_ACTIVACION);
        }

        public void ActualizarDatosUsuario(Usuario_TE usuario)
        {
            usuarioRepo.Modificar(usuario);
            gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });
        }

        public List<Usuario_TE> ObtenerPorEmpresa(int idEmpresa) => usuarioRepo.ObtenerPorEmpresa(idEmpresa);

        public Usuario_TE ObtenerPorId(int idUsuario) => usuarioRepo.ObtenerPorPK(idUsuario);

        public Usuario_TE ObtenerPorEmail(string email) => usuarioRepo.ObtenerPorEmail(email);

        public bool CambiarContrasena(string email, string contrasenaActual, string contrasenaNueva, out string error)
        {
            error = null;

            var usuario = usuarioRepo.ObtenerPorEmail(email);

            if (usuario == null|| usuario.Estado == EstadoUsuario.Bloqueado|| string.IsNullOrEmpty(contrasenaActual)|| !VerificarContrasena(contrasenaActual, usuario.ContrasenaHashUsuario))
            {
                if (usuario != null && usuario.Estado != EstadoUsuario.Bloqueado) RegistrarIntentoFallido(usuario);

                error = "No se pudo cambiar la contrasena. Verifique los datos ingresados.";
                return false;
            }

            if (!EsContrasenaAceptable(contrasenaNueva))
            {
                error = "La nueva contrasena debe tener al menos " + LARGO_MINIMO_CONTRASENA + " caracteres.";
                return false;
            }

            usuario.ContrasenaHashUsuario = cifrador.Encoder(contrasenaNueva);
            usuario.IntentosFallidosUsuario = 0;

            if (usuario.Estado == EstadoUsuario.Pendiente)
                usuario.Estado = EstadoUsuario.Activo;

            usuarioRepo.Modificar(usuario);
            gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });
            return true;
        }

        public string SolicitarRecuperacion(string email)
        {
            var usuario = usuarioRepo.ObtenerPorEmail(email);
            if (usuario == null) return null;

            return EmitirToken(usuario.IdUsuario, TOKEN_RECUPERACION, VIGENCIA_RECUPERACION);
        }

        public ResultadoToken ValidarTokenContrasena(string token)
        {
            return EvaluarToken(LeerToken(token));
        }

        public ResultadoToken EstablecerContrasenaConToken(string token, string contrasenaNueva)
        {
            var info = LeerToken(token);
            var validacion = EvaluarToken(info);
            if (!validacion.Exito) return validacion;

            if (!EsContrasenaAceptable(contrasenaNueva))
                return ResultadoToken.Falla("CONTRASENA_DEBIL", validacion.Email);

            var usuario = usuarioRepo.ObtenerPorPK(info.IdUsuario);

            usuario.ContrasenaHashUsuario = cifrador.Encoder(contrasenaNueva);
            usuario.IntentosFallidosUsuario = 0;

            if (usuario.Estado == EstadoUsuario.Pendiente || usuario.Estado == EstadoUsuario.Bloqueado)
                usuario.Estado = EstadoUsuario.Activo;

            usuarioRepo.Modificar(usuario);
            gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

            tokenRepo.MarcarUsado(info.IdToken);
            tokenRepo.InvalidarPendientes(usuario.IdUsuario, info.Tipo);
            gestorIntegridad.RecalcularTabla(TablasBD.Token);

            return ResultadoToken.Ok(usuario.EmailUsuario);
        }

        private TokenInfo LeerToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            return tokenRepo.ObtenerPorToken(token);
        }

        private ResultadoToken EvaluarToken(TokenInfo info)
        {
            if (info == null || (info.Tipo != TOKEN_ACTIVACION && info.Tipo != TOKEN_RECUPERACION))
                return ResultadoToken.Falla("TOKEN_INVALIDO");

            if (info.Usado)
                return ResultadoToken.Falla("TOKEN_USADO");

            if (info.FechaExpiracion.HasValue && info.FechaExpiracion.Value < DateTime.Now)
                return ResultadoToken.Falla("TOKEN_EXPIRADO");

            var usuario = usuarioRepo.ObtenerPorPK(info.IdUsuario);
            if (usuario == null)
                return ResultadoToken.Falla("TOKEN_INVALIDO");

            return ResultadoToken.Ok(usuario.EmailUsuario);
        }

        private string EmitirToken(int idUsuario, string tipo, TimeSpan vigencia)
        {
            tokenRepo.InvalidarPendientes(idUsuario, tipo);

            string token = Cifrador.GenerarSecretoUrlSafe();
            var ahora = DateTime.Now;
            tokenRepo.Crear(idUsuario, token, tipo, ahora, ahora.Add(vigencia));

            gestorIntegridad.RecalcularTabla(TablasBD.Token);
            return token;
        }

        private static bool EsContrasenaAceptable(string contrasena)
        {
            return !string.IsNullOrWhiteSpace(contrasena) && contrasena.Length >= LARGO_MINIMO_CONTRASENA;
        }

        private void RegistrarIntentoFallido(Usuario_TE usuario)
        {
            usuario.IntentosFallidosUsuario++;

            if (usuario.IntentosFallidosUsuario >= MAX_INTENTOS_FALLIDOS)
            {
                usuario.Estado = EstadoUsuario.Bloqueado;
                usuarioRepo.ActualizarEstado(usuario.IdUsuario, (int)usuario.Estado);
            }

            usuarioRepo.ActualizarIntentosFallidos(usuario.IdUsuario, usuario.IntentosFallidosUsuario);
            gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });
        }

        private void ResetearIntentosFallidos(Usuario_TE usuario)
        {
            if (usuario.IntentosFallidosUsuario == 0) return;

            usuario.IntentosFallidosUsuario = 0;
            usuarioRepo.ActualizarIntentosFallidos(usuario.IdUsuario, 0);
            gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });
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
                string linea = $"{DateTime.Now:o} | ACCESO DE EMERGENCIA | {identificador}";
                string ruta = HttpContext.Current != null
                    ? HttpContext.Current.Server.MapPath("~/App_Data/emergencia.log")
                    : "emergencia.log";

                File.AppendAllText(ruta, linea + Environment.NewLine);
            }
            catch
            {
                // Un fallo al escribir el log de emergencia no debe impedir el acceso de emergencia en sí.
            }
        }
    }
}