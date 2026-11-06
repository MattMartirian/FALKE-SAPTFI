using SECURITY;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using TE;

namespace TLL
{
    // Cuenta de emergencia (break-glass): vive en Web.config, nunca se guarda en la base y cada ingreso queda en un archivo plano.
    public class AccesoEmergencia_TLL
    {
        private readonly Cifrador_SECURITY cifrador;

        public AccesoEmergencia_TLL()
        {
            cifrador = Cifrador_SECURITY.CifradorSingleton;
        }

        internal bool EsCredencial(string identificador, string contrasenaPlana)
        {
            string usuarioConfigurado = ConfigurationManager.AppSettings["FALKE_EMERGENCY_USER"];
            string hashConfigurado = ConfigurationManager.AppSettings["FALKE_EMERGENCY_HASH"];

            if (string.IsNullOrEmpty(usuarioConfigurado) || string.IsNullOrEmpty(hashConfigurado)) return false;

            if (identificador != usuarioConfigurado) return false;

            return cifrador.Encoder(contrasenaPlana) == hashConfigurado;
        }

        internal Usuario_TE ConstruirUsuarioEnMemoria(string identificador)
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

        internal void LoguearAccesoAArchivo(string identificador)
        {
            try
            {
                string linea = $"{DateTime.Now:o} | ACCESO DE EMERGENCIA | {identificador}";
                string ruta = HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/App_Data/emergencia.log") : "emergencia.log";

                File.AppendAllText(ruta, linea + Environment.NewLine);
            }
            catch
            {
            }
        }
    }
}
