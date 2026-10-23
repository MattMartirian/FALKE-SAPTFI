using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ORM;
using TE;

namespace TLL
{
    public static class Traductor_TLL
    {
        public const string CODIGO_BASE = "en";
        public const string CODIGO_DIAGNOSTICO = "es";

        private static readonly object bloqueo = new object();
        private static readonly Dictionary<string, Dictionary<string, string>> catalogos = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        public static string Traducir(string clave, string codigoIdioma, params object[] parametros)
        {
            if (string.IsNullOrEmpty(clave)) return string.Empty;

            string texto = Buscar(clave, codigoIdioma) ?? Buscar(clave, CODIGO_BASE) ?? clave;

            if (parametros == null || parametros.Length == 0) return texto;

            object[] resueltos = parametros.Select(p => p is ClaveTexto_TLL ? ((ClaveTexto_TLL)p).Traducir(codigoIdioma) : p).ToArray();

            try
            {
                return string.Format(Cultura(codigoIdioma), texto, resueltos);
            }
            catch (FormatException)
            {
                return texto;
            }
        }

        public static bool Existe(string clave)
        {
            return !string.IsNullOrEmpty(clave) && Buscar(clave, CODIGO_BASE) != null;
        }

        public static CultureInfo Cultura(string codigoIdioma)
        {
            string codigo = (codigoIdioma ?? string.Empty).Trim();

            if (codigo.Equals("es", StringComparison.OrdinalIgnoreCase)) return CultureInfo.GetCultureInfo("es-AR");
            if (codigo.Equals("en", StringComparison.OrdinalIgnoreCase)) return CultureInfo.GetCultureInfo("en-US");

            try
            {
                return CultureInfo.GetCultureInfo(codigo);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.InvariantCulture;
            }
        }

        public static void Invalidar()
        {
            lock (bloqueo)
            {
                catalogos.Clear();
            }
        }

        private static string Buscar(string clave, string codigoIdioma)
        {
            if (string.IsNullOrWhiteSpace(codigoIdioma)) return null;

            string texto;
            return Catalogo(codigoIdioma.Trim()).TryGetValue(clave, out texto) ? texto : null;
        }

        private static Dictionary<string, string> Catalogo(string codigoIdioma)
        {
            lock (bloqueo)
            {
                Dictionary<string, string> catalogo;

                if (!catalogos.TryGetValue(codigoIdioma, out catalogo))
                {
                    catalogo = Cargar(codigoIdioma);
                    catalogos[codigoIdioma] = catalogo;
                }

                return catalogo;
            }
        }

        private static Dictionary<string, string> Cargar(string codigoIdioma)
        {
            try
            {
                Idioma_TE idioma = new IdiomaRepository().ObtenerPorCodigo(codigoIdioma);

                if (idioma != null)
                {
                    Dictionary<string, string> textos = new TraduccionRepository().ObtenerTextos(idioma.IdIdioma);
                    if (textos.Count > 0) return textos;
                }
            }
            catch (Exception)
            {
            }

            return new TraductorResx_TLL().ObtenerTraducciones(codigoIdioma);
        }
    }
}
