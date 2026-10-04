using System.Collections.Generic;

namespace TE
{
    public class UsuarioListado_TE
    {
        public int IdUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public int IdIdioma { get; set; }
        public string Rol { get; set; }
        public EstadoUsuario Estado { get; set; }
    }

    public class FiltroUsuarios_TE
    {
        public const int TAMANO_MAXIMO = 100;

        public int? IdEmpresa { get; set; }
        public string Texto { get; set; }
        public string Rol { get; set; }
        public EstadoUsuario? Estado { get; set; }
        public int Pagina { get; set; } = 1;
        public int Tamano { get; set; } = 25;

        public FiltroUsuarios_TE Copiar()
        {
            return (FiltroUsuarios_TE)MemberwiseClone();
        }
    }

    public class PaginaUsuarios_TE
    {
        public List<UsuarioListado_TE> Items { get; set; } = new List<UsuarioListado_TE>();
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int Tamano { get; set; }

        public int TotalPaginas
        {
            get { return Tamano <= 0 ? 0 : (Total + Tamano - 1) / Tamano; }
        }
    }
}
