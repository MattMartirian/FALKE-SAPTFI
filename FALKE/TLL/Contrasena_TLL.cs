using ORM;
using SECURITY;
using SERVICES;
using System;
using System.Linq;
using TE;

namespace TLL
{
    // Contraseñas: cambio con la clave actual, recuperación por correo, activación de cuenta con token y política de calidad.
    public class Contrasena_TLL
    {
        private const int LARGO_MINIMO_CONTRASENA = 8;
        public const string POLITICA_CONTRASENA = "La contraseña debe tener al menos 8 caracteres, con una mayúscula, una minúscula, un número y un carácter especial (por ejemplo # ! @ $ %).";

        private readonly UsuarioRepository usuarioRepo;
        private readonly Cifrador_SECURITY cifrador;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;
        private readonly Autenticacion_TLL autenticacion;
        private readonly TokenCuenta_TLL tokens;

        public Contrasena_TLL()
        {
            usuarioRepo = new UsuarioRepository();
            cifrador = Cifrador_SECURITY.CifradorSingleton;
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
            autenticacion = new Autenticacion_TLL();
            tokens = new TokenCuenta_TLL();
        }

        public bool CambiarContrasena(string email, string contrasenaActual, string contrasenaNueva, out string error)
        {
            error = null;
            email = TextoHelper_TLL.NormalizarEmail(email);

            var usuario = usuarioRepo.ObtenerPorEmail(email);

            if (usuario == null|| Autenticacion_TLL.EstaBloqueado(usuario)|| string.IsNullOrEmpty(contrasenaActual)|| !autenticacion.VerificarContrasena(contrasenaActual, usuario.ContrasenaHashUsuario))
            {
                if (usuario != null && !Autenticacion_TLL.EstaBloqueado(usuario))
                {
                    autenticacion.RegistrarIntentoFallido(usuario, "cambio de contraseña");
                }

                error = "No se pudo cambiar la contraseña. Verificá los datos ingresados.";
                return false;
            }

            if (!EsContrasenaAceptable(contrasenaNueva))
            {
                error = POLITICA_CONTRASENA;
                return false;
            }

            if (contrasenaNueva == contrasenaActual)
            {
                error = "La nueva contraseña no puede ser igual a la actual.";
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

                bitacora.Guardar(new Bitacora_TE(usuario.IdUsuario, "Seguridad", "Cambio de contraseña", CriticidadBitacora.Media, DateTime.Now));
            });

            return true;
        }

        public SolicitudEnlace_TLL SolicitarEnlace(string email)
        {
            var usuario = usuarioRepo.ObtenerPorEmail(TextoHelper_TLL.NormalizarEmail(email));
            if (usuario == null) return null;

            if (usuario.Estado == EstadoUsuario.BloqueoEstricto || usuario.Estado == EstadoUsuario.Inactivo) return null;

            if (!autenticacion.EmpresaPermiteIngreso(usuario.IdEmpresa)) return null;

            string token = null;

            Transaccion_ORM.Ejecutar(() =>
            {
                token = tokens.EmitirRecuperacion(usuario.IdUsuario);
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Solicitud de recuperación de contraseña", CriticidadBitacora.Baja);
            });

            return new SolicitudEnlace_TLL { Token = token, Nombre = usuario.NombreUsuario };
        }

        public ResultadoToken_TLL ValidarTokenContrasena(string token)
        {
            return tokens.Evaluar(tokens.Leer(token));
        }

        public ResultadoToken_TLL EstablecerContrasenaConToken(string token, string contrasenaNueva)
        {
            var info = tokens.Leer(token);
            var validacion = tokens.Evaluar(info);
            if (!validacion.Exito) return validacion;

            if (!EsContrasenaAceptable(contrasenaNueva)) return ResultadoToken_TLL.Falla("CONTRASENA_DEBIL", validacion.Email);

            var usuario = usuarioRepo.ObtenerPorPK(info.IdUsuario);

            usuario.ContrasenaHashUsuario = cifrador.Encoder(contrasenaNueva);
            usuario.IntentosFallidosUsuario = 0;

            if (usuario.Estado == EstadoUsuario.Pendiente || usuario.Estado == EstadoUsuario.BloqueadoPorIntentos)
                usuario.Estado = EstadoUsuario.Activo;

            string via = info.Tipo == TokenCuenta_TLL.TOKEN_ACTIVACION ? "activación" : "recuperación";

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.Modificar(usuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                tokens.Consumir(info);

                bitacora.Guardar(new Bitacora_TE(usuario.IdUsuario, "Seguridad", "Contraseña establecida mediante token de " + via + "; la cuenta queda activa", CriticidadBitacora.Media, DateTime.Now));
            });

            return ResultadoToken_TLL.Ok(usuario.EmailUsuario);
        }

        // Mínimo 8 caracteres, con mayúscula, minúscula, número y un carácter especial (ni letra, ni número, ni espacio).
        public static bool EsContrasenaAceptable(string contrasena)
        {
            return !string.IsNullOrWhiteSpace(contrasena)
                && contrasena.Length >= LARGO_MINIMO_CONTRASENA
                && contrasena.Any(char.IsUpper)
                && contrasena.Any(char.IsLower)
                && contrasena.Any(char.IsDigit)
                && contrasena.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        }
    }
}
