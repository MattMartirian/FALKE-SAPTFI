using System.Collections.Generic;
using ORM;
using SECURITY;
using SERVICIOS;
using TE;

namespace TLL
{
    public class UsuarioTLL
    {
        private const int MAX_INTENTOS_FALLIDOS = 5;

        private readonly UsuarioRepository usuarioRepo;
        private readonly Cifrador cifrador;
        private readonly GestorIntegridad gestorIntegridad;

        public UsuarioTLL()
        {
            usuarioRepo = new UsuarioRepository();
            cifrador = Cifrador.CypherInstance;
            gestorIntegridad = new GestorIntegridad();
        }

        public ResultadoLogin ValidarCredenciales(string email, string contrasenaPlana)
        {
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

        public void RegistrarUsuario(Usuario_TE usuario, string contrasenaPlana)
        {
            usuario.ContrasenaHashUsuario = cifrador.Encoder(contrasenaPlana);
            usuario.IntentosFallidosUsuario = 0;
            usuario.Estado = EstadoUsuario.Pendiente;

            usuarioRepo.Alta(usuario);
            gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });
        }

        public void ActualizarDatosUsuario(Usuario_TE usuario)
        {
            usuarioRepo.Modificar(usuario);
            gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });
        }

        public List<Usuario_TE> ObtenerPorEmpresa(int idEmpresa) => usuarioRepo.ObtenerPorEmpresa(idEmpresa);

        public Usuario_TE ObtenerPorId(int idUsuario) => usuarioRepo.ObtenerPorPK(idUsuario);

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
    }
}