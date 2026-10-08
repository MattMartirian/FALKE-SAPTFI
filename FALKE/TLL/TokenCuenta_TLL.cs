using ORM;
using SECURITY;
using SERVICES;
using System;
using TE;

namespace TLL
{
    // Enlaces de un solo uso que se mandan por correo: activación de cuenta (48 h) y recuperación de contraseña (2 h).
    public class TokenCuenta_TLL
    {
        public const string TOKEN_ACTIVACION = "activacion";
        public const string TOKEN_RECUPERACION = "recuperacion";

        private static readonly TimeSpan VIGENCIA_ACTIVACION = TimeSpan.FromHours(48);
        private static readonly TimeSpan VIGENCIA_RECUPERACION = TimeSpan.FromHours(2);

        private readonly UsuarioRepository usuarioRepo;
        private readonly TokenRepository tokenRepo;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;

        public TokenCuenta_TLL()
        {
            usuarioRepo = new UsuarioRepository();
            tokenRepo = new TokenRepository();
            gestorIntegridad = new GestorIntegridad_SERVICE();
        }

        internal string EmitirActivacion(int idUsuario, bool invalidarAnteriores = true)
        {
            return Emitir(idUsuario, TOKEN_ACTIVACION, VIGENCIA_ACTIVACION, invalidarAnteriores);
        }

        internal string EmitirRecuperacion(int idUsuario)
        {
            return Emitir(idUsuario, TOKEN_RECUPERACION, VIGENCIA_RECUPERACION);
        }

        // Marca el token como usado e invalida los demás pendientes del mismo tipo. Se llama dentro de la transacción de quien lo usa.
        internal void Consumir(TokenInfo info)
        {
            tokenRepo.MarcarUsado(info.IdToken);
            tokenRepo.InvalidarPendientes(info.IdUsuario, info.Tipo);
            gestorIntegridad.RecalcularTabla(TablasBD.Token);
        }

        internal TokenInfo Leer(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            return tokenRepo.ObtenerPorToken(token);
        }

        internal ResultadoToken_TLL Evaluar(TokenInfo info)
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

            // Un enlace que se mandó antes de dar de baja o bloquear la cuenta ya no sirve.
            if (usuario.Estado == EstadoUsuario.Inactivo || usuario.Estado == EstadoUsuario.BloqueoEstricto) return ResultadoToken_TLL.Falla("TOKEN_INVALIDO");

            return ResultadoToken_TLL.Ok(usuario.EmailUsuario);
        }

        private string Emitir(int idUsuario, string tipo, TimeSpan vigencia, bool invalidarAnteriores = true)
        {
            string token = Cifrador_SECURITY.GenerarSecretoUrlSafe();
            var ahora = DateTime.Now;

            Transaccion_ORM.Ejecutar(() =>
            {
                if (invalidarAnteriores) tokenRepo.InvalidarPendientes(idUsuario, tipo);
                tokenRepo.Crear(idUsuario, token, tipo, ahora, ahora.Add(vigencia));
                gestorIntegridad.RecalcularTabla(TablasBD.Token);
            });

            return token;
        }
    }
}
