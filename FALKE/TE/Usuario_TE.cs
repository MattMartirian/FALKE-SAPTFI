using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TE
{
    public class Usuario_TE
    {
        public int IdUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public string ContrasenaHashUsuario { get; set; }
        public PermisoCompuesto_TE Rol { get; set; }
        public EstadoUsuario Estado { get; set; }
        public int IntentosFallidosUsuario { get; set; }
        public int IdIdioma { get; set; } //TODO Cambiar por clase Idioma_TE
        public string DVH { get; set; }
        public bool EsCuentaEmergencia { get; set; } 
    }

    public enum EstadoUsuario
    {
        Pendiente = 0,
        Activo = 1,
        Bloqueado = 2,
        Inactivo = 3
    }
}
