using System;
using TE;

namespace TLL
{
    public class ResultadoLogin_TLL
    {
        public bool Exito { get; private set; }
        public string Motivo { get; private set; }
        public Usuario_TE Usuario { get; private set; }
        public string Token { get; private set; }

        // Entró, pero la integridad de los datos está comprometida y esta cuenta puede repararla: hay que llevarla a la pantalla de Dígito verificador.
        public bool RequiereRevisarIntegridad { get; private set; }

        // Solo en CREDENCIALES_INVALIDAS de una cuenta que existe: cuántos intentos quedan antes del bloqueo. Con un correo inexistente es null,
        // así el mensaje no deja averiguar qué cuentas existen.
        public int? IntentosRestantes { get; private set; }

        private ResultadoLogin_TLL() { }

        public static ResultadoLogin_TLL Exitoso(Usuario_TE u, bool requiereRevisarIntegridad = false) => new ResultadoLogin_TLL { Exito = true, Usuario = u, RequiereRevisarIntegridad = requiereRevisarIntegridad };

        public static ResultadoLogin_TLL CredencialesInvalidas(int? intentosRestantes = null) => new ResultadoLogin_TLL { Exito = false, Motivo = "CREDENCIALES_INVALIDAS", IntentosRestantes = intentosRestantes };

        public static ResultadoLogin_TLL BloqueadoPorIntentos() =>       new ResultadoLogin_TLL { Exito = false, Motivo = "USUARIO_BLOQUEADO_INTENTOS" };

        public static ResultadoLogin_TLL Inactivo() =>                   new ResultadoLogin_TLL { Exito = false, Motivo = "USUARIO_INACTIVO" };

        public static ResultadoLogin_TLL EmpresaNoActiva(string motivo) => new ResultadoLogin_TLL { Exito = false, Motivo = motivo };

        public static ResultadoLogin_TLL BloqueoEstricto() =>            new ResultadoLogin_TLL { Exito = false, Motivo = "USUARIO_BLOQUEO_ESTRICTO" };

        public static ResultadoLogin_TLL UsuarioPendienteActivacion() => new ResultadoLogin_TLL { Exito = false, Motivo = "USUARIO_PENDIENTE" };

        public static ResultadoLogin_TLL IntegridadComprometida() =>     new ResultadoLogin_TLL { Exito = false, Motivo = "DVH_INVALIDO" };
    }
}
