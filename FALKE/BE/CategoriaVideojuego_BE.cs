using System;
using System.Collections.Generic;

namespace BE
{
    public class CategoriaVideojuego_BE : Categoria_BE
    {
        public PlataformaVideojuego Plataforma { get; set; }
        public string VersionVideojuego { get; set; }

        public override TipoActivoCategoria Tipo
        {
            get { return TipoActivoCategoria.Videojuego; }
        }

        public override IDictionary<string, string> ObtenerCampos()
        {
            return new Dictionary<string, string>
            {
                { "plataforma", Plataforma.ToString() },
                { "versionjuego", VersionVideojuego }
            };
        }

        public override void AplicarCampos(IDictionary<string, string> campos)
        {
            Plataforma = Enumerado<PlataformaVideojuego>(campos, "plataforma");
            VersionVideojuego = Valor(campos, "versionjuego");
        }

        public override string NormalizarYValidar()
        {
            VersionVideojuego = Limpiar(VersionVideojuego);

            if (!Enum.IsDefined(typeof(PlataformaVideojuego), Plataforma)) return "La plataforma no es válida.";

            return ExcedeLargo(VersionVideojuego, "versión del juego");
        }
    }
}
