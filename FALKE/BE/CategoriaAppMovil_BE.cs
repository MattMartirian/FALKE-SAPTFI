using System;
using System.Collections.Generic;

namespace BE
{
    public class CategoriaAppMovil_BE : Categoria_BE
    {
        public SistemaOperativoMovil SoAppMovil { get; set; }
        public string VersionAppMovil { get; set; }

        public override TipoActivoCategoria Tipo
        {
            get { return TipoActivoCategoria.AppMovil; }
        }

        public override IDictionary<string, string> ObtenerCampos()
        {
            return new Dictionary<string, string>
            {
                { "somovil", SoAppMovil.ToString() },
                { "versionapp", VersionAppMovil }
            };
        }

        public override void AplicarCampos(IDictionary<string, string> campos)
        {
            SoAppMovil = Enumerado<SistemaOperativoMovil>(campos, "somovil");
            VersionAppMovil = Valor(campos, "versionapp");
        }

        public override ErrorValidacion_BE NormalizarYValidar()
        {
            VersionAppMovil = Limpiar(VersionAppMovil);

            if (!Enum.IsDefined(typeof(SistemaOperativoMovil), SoAppMovil)) return new ErrorValidacion_BE("categoria_so_invalido");

            return ExcedeLargo(VersionAppMovil, "categoria_campo_version_app");
        }
    }
}
