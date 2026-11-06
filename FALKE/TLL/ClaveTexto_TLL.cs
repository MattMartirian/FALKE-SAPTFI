using System.Globalization;

namespace TLL
{
    public class ClaveTexto_TLL
    {
        public string Clave { get; private set; }
        public bool Minusculas { get; private set; }

        public ClaveTexto_TLL(string clave, bool minusculas = false)
        {
            Clave = clave;
            Minusculas = minusculas;
        }

        public string Traducir(string codigoIdioma)
        {
            string texto = Traductor_TLL.Traducir(Clave, codigoIdioma);

            return Minusculas ? texto.ToLower(Traductor_TLL.Cultura(codigoIdioma)) : texto;
        }
    }
}
