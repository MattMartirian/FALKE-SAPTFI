using System;

namespace BE
{
    public class Categoria_BE
    {
        public int IdCategoria { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreCategoria { get; set; }
        public TipoActivoCategoria Tipo { get; set; }
        public string NombreActivo { get; set; }
        public string FlujoEsperado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activa { get; set; }
        public string DVH { get; set; }

        public int CantidadSesiones { get; set; }

        // TODO: Cuando este realizado correctamente el flujo de Categorias, dividir en subclases con herencia.
        // Específicos de software
        public string SistemaOperativoSoftware { get; set; }
        public string VersionSoftware { get; set; }

        // Específicos de app web
        public string UrlAppWeb { get; set; }
        public DispositivoObjetivo DispositivoAppWeb { get; set; }

        // Específicos de app móvil
        public SistemaOperativoMovil SoAppMovil { get; set; }
        public string VersionAppMovil { get; set; }

        // Específicos de videojuego
        public PlataformaVideojuego Plataforma { get; set; }
        public string VersionVideojuego { get; set; }

        // Específicos de publicidad
        public string FormatoPublicidad { get; set; }
        public string CanalPublicidad { get; set; }
    }

    public enum TipoActivoCategoria
    {
        Software,
        AppWeb,
        AppMovil,
        Videojuego,
        Publicidad
    }

    public enum DispositivoObjetivo
    {
        Escritorio,
        Tablet,
        Movil
    }

    public enum SistemaOperativoMovil
    {
        Android,
        Ios
    }

    public enum PlataformaVideojuego
    {
        Pc,
        Consola,
        Movil
    }
}
