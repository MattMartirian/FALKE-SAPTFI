using System.Collections.Generic;

namespace BE
{
    public class CategoriaPublicidad_BE : Categoria_BE
    {
        public string FormatoPublicidad { get; set; }
        public string CanalPublicidad { get; set; }

        public override TipoActivoCategoria Tipo
        {
            get { return TipoActivoCategoria.Publicidad; }
        }

        public override IDictionary<string, string> ObtenerCampos()
        {
            return new Dictionary<string, string>
            {
                { "formato", FormatoPublicidad },
                { "canal", CanalPublicidad }
            };
        }

        public override void AplicarCampos(IDictionary<string, string> campos)
        {
            FormatoPublicidad = Valor(campos, "formato");
            CanalPublicidad = Valor(campos, "canal");
        }

        public override string NormalizarYValidar()
        {
            FormatoPublicidad = Limpiar(FormatoPublicidad);
            CanalPublicidad = Limpiar(CanalPublicidad);

            return ExcedeLargo(FormatoPublicidad, "formato")
                ?? ExcedeLargo(CanalPublicidad, "canal de distribución");
        }
    }
}
