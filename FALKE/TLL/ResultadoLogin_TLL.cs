using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE;

namespace TLL
{
    public class ResultadoLogin_TLL
    {
        public bool Exito { get; private set; }
        public string Motivo { get; private set; }
        public Usuario_TE Usuario { get; private set; }
        public string Token { get; private set; }

        private ResultadoLogin_TLL() { }

        public static ResultadoLogin_TLL Exitoso(Usuario_TE u) =>        new ResultadoLogin_TLL { Exito = true, Usuario = u };

        public static ResultadoLogin_TLL CredencialesInvalidas() =>      new ResultadoLogin_TLL { Exito = false, Motivo = "CREDENCIALES_INVALIDAS" };

        public static ResultadoLogin_TLL BloqueadoPorIntentos() =>       new ResultadoLogin_TLL { Exito = false, Motivo = "USUARIO_BLOQUEADO_INTENTOS" };

        public static ResultadoLogin_TLL Inactivo() =>                   new ResultadoLogin_TLL { Exito = false, Motivo = "USUARIO_INACTIVO" };

        public static ResultadoLogin_TLL EmpresaNoActiva(string motivo) => new ResultadoLogin_TLL { Exito = false, Motivo = motivo };

        public static ResultadoLogin_TLL BloqueoEstricto() =>            new ResultadoLogin_TLL { Exito = false, Motivo = "USUARIO_BLOQUEO_ESTRICTO" };

        public static ResultadoLogin_TLL UsuarioPendienteActivacion() => new ResultadoLogin_TLL { Exito = false, Motivo = "USUARIO_PENDIENTE" };

        public static ResultadoLogin_TLL IntegridadComprometida() =>     new ResultadoLogin_TLL { Exito = false, Motivo = "DVH_INVALIDO" };
    }
}
