using System.Globalization;
using System.Text;

namespace TLL
{
    public static class TextoBitacora_TLL
    {
        private const string PREFIJO_MODULO = "bitacora_mod_";

        public static string Descripcion(string descripcion, string codigoIdioma)
        {
            if (string.IsNullOrEmpty(descripcion)) return string.Empty;

            return Traductor_TLL.Existe(descripcion) ? Traductor_TLL.Traducir(descripcion, codigoIdioma) : descripcion;
        }

        public static string Detalle(string detalle, string codigoIdioma)
        {
            return DetalleBitacora_TLL.Traducir(detalle, codigoIdioma);
        }

        public static string Modulo(string modulo, string codigoIdioma)
        {
            if (string.IsNullOrEmpty(modulo)) return string.Empty;

            string clave = PREFIJO_MODULO + Normalizar(modulo);

            return Traductor_TLL.Existe(clave) ? Traductor_TLL.Traducir(clave, codigoIdioma) : modulo;
        }

        private static string Normalizar(string texto)
        {
            var resultado = new StringBuilder();

            foreach (char c in texto.Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue;

                resultado.Append(char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : '_');
            }

            return resultado.ToString();
        }
    }
}
