using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE;

namespace TLL
{
    public class ResultadoLogin
    {
        public bool Exito { get; private set; }
        public string Motivo { get; private set; }
        public Usuario_TE Usuario { get; private set; }

        private ResultadoLogin() { }

        public static ResultadoLogin Exitoso(Usuario_TE u) =>        new ResultadoLogin { Exito = true, Usuario = u };

        public static ResultadoLogin CredencialesInvalidas() =>      new ResultadoLogin { Exito = false, Motivo = "CREDENCIALES_INVALIDAS" };

        public static ResultadoLogin UsuarioBloqueado() =>           new ResultadoLogin { Exito = false, Motivo = "USUARIO_BLOQUEADO" };

        public static ResultadoLogin UsuarioPendienteActivacion() => new ResultadoLogin { Exito = false, Motivo = "USUARIO_PENDIENTE" };

        public static ResultadoLogin IntegridadComprometida() =>     new ResultadoLogin { Exito = false, Motivo = "DVH_INVALIDO" };
    }
}
