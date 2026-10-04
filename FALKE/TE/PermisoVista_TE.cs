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

        // Descripción guardada (puede ser null) y lo que se muestra: la descripción o, si no hay, el nombre.
        public string Descripcion { get; set; }
        public string Etiqueta { get; set; }

        // Roles base: no se renombran ni se eliminan. El Gestor además es fijo (no se edita su composición).
        public bool EsBase { get; set; }
        public bool EsFijo { get; set; }

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
