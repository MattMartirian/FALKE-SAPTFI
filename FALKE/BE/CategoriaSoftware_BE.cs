using System.Collections.Generic;

namespace BE
{
    public class CategoriaSoftware_BE : Categoria_BE
    {
        public string SistemaOperativoSoftware { get; set; }
        public string VersionSoftware { get; set; }

        public override TipoActivoCategoria Tipo
        {
            get { return TipoActivoCategoria.Software; }
        }

        public override IDictionary<string, string> ObtenerCampos()
        {
            return new Dictionary<string, string>
            {
                { "so", SistemaOperativoSoftware },
                { "versionsw", VersionSoftware }
            };
        }

        public override void AplicarCampos(IDictionary<string, string> campos)
        {
            SistemaOperativoSoftware = Valor(campos, "so");
            VersionSoftware = Valor(campos, "versionsw");
        }

        public override ErrorValidacion_BE NormalizarYValidar()
        {
            SistemaOperativoSoftware = Limpiar(SistemaOperativoSoftware);
            VersionSoftware = Limpiar(VersionSoftware);

            return ExcedeLargo(SistemaOperativoSoftware, "categoria_campo_so")
                ?? ExcedeLargo(VersionSoftware, "categoria_campo_version_software");
        }
    }
}
