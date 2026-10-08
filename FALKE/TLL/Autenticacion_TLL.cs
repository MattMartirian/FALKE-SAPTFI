using ORM;
using SECURITY;
using SERVICES;
using System;
using TE;

namespace TLL
{
    // Inicio de sesión: valida credenciales, cuenta los intentos fallidos, bloquea, controla la integridad y el estado de la empresa.
    public class Autenticacion_TLL
    {
        private const int MAX_INTENTOS_FALLIDOS = 5;

        public const string MOTIVO_EMPRESA_BLOQUEADA = "EMPRESA_BLOQUEADA";
        public const string MOTIVO_EMPRESA_DESHABILITADA = "EMPRESA_DESHABILITADA";

        private readonly UsuarioRepository usuarioRepo;
        private readonly Cifrador_SECURITY cifrador;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;
        private readonly AccesoEmergencia_TLL emergencia;

        public Autenticacion_TLL()
        {
            usuarioRepo = new UsuarioRepository();
            cifrador = Cifrador_SECURITY.CifradorSingleton;
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
            emergencia = new AccesoEmergencia_TLL();
        }

        public ResultadoLogin_TLL ValidarCredenciales(string email, string contrasenaPlana)
        {
            if (emergencia.EsCredencial(email, contrasenaPlana))
            {
                var usuarioEmergencia = emergencia.ConstruirUsuarioEnMemoria(email);
                emergencia.LoguearAccesoAArchivo(email);
                bitacora.Registrar(0, "Seguridad", "Acceso de emergencia (break-glass) con identificador '" + email + "'", CriticidadBitacora.Alta);
                return ResultadoLogin_TLL.Exitoso(usuarioEmergencia, HayInconsistenciasDeIntegridad());
            }

            email = TextoHelper_TLL.NormalizarEmail(email);

            var usuario = usuarioRepo.ObtenerPorEmail(email);

            if (usuario == null) return ResultadoLogin_TLL.CredencialesInvalidas();

            if (usuario.Estado == EstadoUsuario.BloqueadoPorIntentos)
            {
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Intento de inicio de sesión sobre una cuenta bloqueada por intentos fallidos", CriticidadBitacora.Media);
                return ResultadoLogin_TLL.BloqueadoPorIntentos();
            }

            if (usuario.Estado == EstadoUsuario.Pendiente)
            {
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Intento de inicio de sesión sobre una cuenta pendiente de activación", CriticidadBitacora.Baja);
                return ResultadoLogin_TLL.UsuarioPendienteActivacion();
            }

            if (usuario.Estado == EstadoUsuario.Inactivo)
            {
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Intento de inicio de sesión sobre una cuenta inactiva (dada de baja)", CriticidadBitacora.Baja, usuario.IdEmpresa);
                return ResultadoLogin_TLL.Inactivo();
            }

            if (usuario.Estado == EstadoUsuario.BloqueoEstricto)
            {
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Intento de inicio de sesión sobre una cuenta con bloqueo estricto", CriticidadBitacora.Media, usuario.IdEmpresa);
                return ResultadoLogin_TLL.BloqueoEstricto();
            }

            if (!VerificarContrasena(contrasenaPlana, usuario.ContrasenaHashUsuario))
            {
                RegistrarIntentoFallido(usuario);

                if (usuario.Estado == EstadoUsuario.BloqueadoPorIntentos) return ResultadoLogin_TLL.BloqueadoPorIntentos();

                return ResultadoLogin_TLL.CredencialesInvalidas(Math.Max(0, MAX_INTENTOS_FALLIDOS - usuario.IntentosFallidosUsuario));
            }

            string motivoEmpresa = MotivoEmpresaSinIngreso(usuario.IdEmpresa);
            if (motivoEmpresa != null)
            {
                bitacora.Registrar(usuario.IdUsuario, "Seguridad",
                    "Inicio de sesión rechazado: la empresa del usuario está " + (motivoEmpresa == MOTIVO_EMPRESA_DESHABILITADA ? "dada de baja" : "bloqueada"),
                    CriticidadBitacora.Media, usuario.IdEmpresa);
                return ResultadoLogin_TLL.EmpresaNoActiva(motivoEmpresa);
            }

            var inconsistencias = gestorIntegridad.VerificarIntegridadTodasLasTablas();
            bool revisarIntegridad = inconsistencias.Count > 0;

            if (revisarIntegridad)
            {
                if (!Permiso_TLL.ComprobarPermiso(Patentes_TLL.RECALCULAR_INTEGRIDAD, usuario.Rol))
                {
                    bitacora.Registrar(usuario.IdUsuario, "Integridad", "Inicio de sesión rechazado: la integridad de los datos está comprometida (" + inconsistencias.Count + " inconsistencia/s)", CriticidadBitacora.Alta);
                    return ResultadoLogin_TLL.IntegridadComprometida();
                }

                bitacora.Registrar(usuario.IdUsuario, "Integridad", "Inicio de sesión con la integridad de los datos comprometida (" + inconsistencias.Count + " inconsistencia/s): se lo lleva a Dígito verificador para revisarla", CriticidadBitacora.Alta);
            }

            Transaccion_ORM.Ejecutar(() =>
            {
                ResetearIntentosFallidos(usuario);
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Inicio de sesión exitoso", CriticidadBitacora.Baja);
            });

            return ResultadoLogin_TLL.Exitoso(usuario, revisarIntegridad);
        }

        // Marca que cambia cuando cambia la contraseña: las sesiones y las cookies de "recordarme" que la llevan dejan de valer.
        public string HuellaDeAcceso(Usuario_TE usuario)
        {
            return cifrador.Encoder(usuario.ContrasenaHashUsuario ?? string.Empty).Substring(0, 16);
        }

        // Pattern Blue (la empresa proveedora) nunca se bloquea: desde ahí se administra todo.
        public bool EmpresaPermiteIngreso(int idEmpresa)
        {
            return MotivoEmpresaSinIngreso(idEmpresa) == null;
        }

        // null si la empresa deja entrar; si no, por qué no: bloqueada (sigue siendo cliente) o deshabilitada (dada de baja).
        public string MotivoEmpresaSinIngreso(int idEmpresa)
        {
            if (idEmpresa <= 0 || idEmpresa == BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA) return null;

            string estado = usuarioRepo.ObtenerEstadoEmpresa(idEmpresa);

            if (estado == null || string.Equals(estado, "activa", StringComparison.OrdinalIgnoreCase)) return null;

            return string.Equals(estado, "deshabilitada", StringComparison.OrdinalIgnoreCase) ? MOTIVO_EMPRESA_DESHABILITADA : MOTIVO_EMPRESA_BLOQUEADA;
        }

        internal static bool EstaBloqueado(Usuario_TE usuario)
        {
            return usuario.Estado == EstadoUsuario.BloqueadoPorIntentos || usuario.Estado == EstadoUsuario.BloqueoEstricto;
        }

        internal bool VerificarContrasena(string contrasenaPlana, string hashAlmacenado)
        {
            return cifrador.Encoder(contrasenaPlana) == hashAlmacenado;
        }

        internal void RegistrarIntentoFallido(Usuario_TE usuario, string contexto = "inicio de sesión")
        {
            usuario.IntentosFallidosUsuario++;

            bool seBloqueo = usuario.IntentosFallidosUsuario >= MAX_INTENTOS_FALLIDOS;
            if (seBloqueo) usuario.Estado = EstadoUsuario.BloqueadoPorIntentos;

            Transaccion_ORM.Ejecutar(() =>
            {
                if (seBloqueo) usuarioRepo.ActualizarEstado(usuario.IdUsuario, (int)usuario.Estado);

                usuarioRepo.ActualizarIntentosFallidos(usuario.IdUsuario, usuario.IntentosFallidosUsuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                if (seBloqueo)
                {
                    bitacora.Guardar(new Bitacora_TE(usuario.IdUsuario, "Seguridad", "Cuenta bloqueada por superar el máximo de intentos fallidos", CriticidadBitacora.Alta, DateTime.Now));
                }
                else
                {
                    bitacora.Guardar(new Bitacora_TE(usuario.IdUsuario, "Seguridad", "Intento fallido de " + contexto + " (" + usuario.IntentosFallidosUsuario + " de " + MAX_INTENTOS_FALLIDOS + ")", CriticidadBitacora.Baja, DateTime.Now));
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

        // La cuenta de emergencia siempre entra; si la integridad está comprometida, también se la lleva a Dígito verificador.
        private bool HayInconsistenciasDeIntegridad()
        {
            try
            {
                return gestorIntegridad.VerificarIntegridadTodasLasTablas().Count > 0;
            }
            catch
            {
                return true;
            }
        }
    }
}
