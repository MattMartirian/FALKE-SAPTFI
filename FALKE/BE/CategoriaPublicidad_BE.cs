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
                { "categoria_campo_formato", FormatoPublicidad },
                { "canal", CanalPublicidad }
            };
        }

        public override void AplicarCampos(IDictionary<string, string> campos)
        {
            FormatoPublicidad = Valor(campos, "categoria_campo_formato");
            CanalPublicidad = Valor(campos, "canal");
        }

        public override ErrorValidacion_BE NormalizarYValidar()
        {
            FormatoPublicidad = Limpiar(FormatoPublicidad);
            CanalPublicidad = Limpiar(CanalPublicidad);

            return ExcedeLargo(FormatoPublicidad, "categoria_campo_formato")
                ?? ExcedeLargo(CanalPublicidad, "categoria_campo_canal");
        }
    }
}
