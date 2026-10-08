using System;
using System.Collections.Generic;

namespace BE
{
    public abstract class Categoria_BE
    {
        protected const int LARGO_CAMPO = 50;

        public int IdCategoria { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreCategoria { get; set; }
        public string NombreActivo { get; set; }
        public string FlujoEsperado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activa { get; set; }
        public string DVH { get; set; }
        public int CantidadSesiones { get; set; }

        public abstract TipoActivoCategoria Tipo { get; }

        public abstract IDictionary<string, string> ObtenerCampos();

        public abstract void AplicarCampos(IDictionary<string, string> campos);

        public abstract string NormalizarYValidar();

        public static Categoria_BE Nueva(TipoActivoCategoria tipo)
        {
            switch (tipo)
            {
                case TipoActivoCategoria.Software: return new CategoriaSoftware_BE();
                case TipoActivoCategoria.AppWeb: return new CategoriaAppWeb_BE();
                case TipoActivoCategoria.AppMovil: return new CategoriaAppMovil_BE();
                case TipoActivoCategoria.Videojuego: return new CategoriaVideojuego_BE();
                case TipoActivoCategoria.Publicidad: return new CategoriaPublicidad_BE();
                default: throw new ArgumentOutOfRangeException(nameof(tipo));
            }
        }

        protected static string Limpiar(string texto)
        {
            string limpio = (texto ?? string.Empty).Trim();

            return limpio.Length == 0 ? null : limpio;
        }

        protected static string Valor(IDictionary<string, string> campos, string clave)
        {
            string valor;

            return campos.TryGetValue(clave, out valor) ? valor : null;
        }

        protected static T Enumerado<T>(IDictionary<string, string> campos, string clave) where T : struct
        {
            T resultado;

            Enum.TryParse(Valor(campos, clave), out resultado);

            return resultado;
        }

        protected static string ExcedeLargo(string valor, string campo, int maximo = LARGO_CAMPO)
        {
            return valor != null && valor.Length > maximo
                ? "El campo \"" + campo + "\" no puede superar los " + maximo + " caracteres."
                : null;
        }
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
