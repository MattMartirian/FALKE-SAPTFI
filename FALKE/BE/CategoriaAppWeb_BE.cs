using System;
using System.Collections.Generic;

namespace BE
{
    public class CategoriaAppWeb_BE : Categoria_BE
    {
        private const int LARGO_URL = 300;

        public string UrlAppWeb { get; set; }
        public DispositivoObjetivo DispositivoAppWeb { get; set; }

        public override TipoActivoCategoria Tipo
        {
            get { return TipoActivoCategoria.AppWeb; }
        }

        public override IDictionary<string, string> ObtenerCampos()
        {
            return new Dictionary<string, string>
            {
                { "url", UrlAppWeb },
                { "dispositivo", DispositivoAppWeb.ToString() }
            };
        }

        public override void AplicarCampos(IDictionary<string, string> campos)
        {
            UrlAppWeb = Valor(campos, "url");
            DispositivoAppWeb = Enumerado<DispositivoObjetivo>(campos, "dispositivo");
        }

        public override string NormalizarYValidar()
        {
            UrlAppWeb = Limpiar(UrlAppWeb);

            if (!Enum.IsDefined(typeof(DispositivoObjetivo), DispositivoAppWeb)) return "El dispositivo objetivo no es válido.";

            return ExcedeLargo(UrlAppWeb, "URL", LARGO_URL);
        }
    }
}
