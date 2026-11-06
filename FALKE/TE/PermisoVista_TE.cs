using System.Collections.Generic;

namespace TE
{
    public enum ClasePermiso
    {
        Rol,
        Grupo,
        Patente
    }

    public class PermisoVista_TE
    {
        public string Nombre { get; set; }
        public ClasePermiso Clase { get; set; }
        public string Descripcion { get; set; }
        public string Etiqueta { get; set; }
        public bool EsBase { get; set; }
        public bool EsFijo { get; set; }
        public bool EsDeGestion { get; set; }
        public int UsuariosAsignados { get; set; }
        public List<string> Incluye { get; set; } = new List<string>();
        public List<string> IncluidoEn { get; set; } = new List<string>();
        public List<string> PatentesEfectivas { get; set; } = new List<string>();
    }

    public class CatalogoPermisos_TE
    {
        public List<PermisoVista_TE> Roles { get; set; } = new List<PermisoVista_TE>();
        public List<PermisoVista_TE> Grupos { get; set; } = new List<PermisoVista_TE>();
        public List<PermisoVista_TE> Patentes { get; set; } = new List<PermisoVista_TE>();
    }
}
