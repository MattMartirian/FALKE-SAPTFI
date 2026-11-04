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

        private static readonly TimeSpan VIGENCIA = TimeSpan.FromSeconds(30);

        private static readonly object bloqueo = new object();
        private static string codigoBase;
        private static DateTime baseLeidaEn;
        private static readonly Dictionary<string, DateTime> cargadosEn = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Dictionary<string, string>> catalogos = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        public static string CodigoBase
        {
            get
            {
                lock (bloqueo)
                {
                    if (codigoBase == null || DateTime.UtcNow - baseLeidaEn > VIGENCIA)
                    {
                        string leido = LeerCodigoBase();

                        if (leido != null) codigoBase = leido;

                        baseLeidaEn = DateTime.UtcNow;
                    }

                    return codigoBase ?? CODIGO_BASE;
                }
            }
        }

        public static string Traducir(string clave, string codigoIdioma, params object[] parametros)
        {
            if (string.IsNullOrEmpty(clave)) return string.Empty;

            string codigoUsado = codigoIdioma;
            string texto = Buscar(clave, codigoIdioma);

            if (texto == null)
            {
                codigoUsado = CodigoBase;
                texto = Buscar(clave, codigoUsado);
            }

            if (texto == null) return clave;

            if (parametros == null || parametros.Length == 0) return texto;

            object[] resueltos = parametros.Select(p => p is ClaveTexto_TLL ? ((ClaveTexto_TLL)p).Traducir(codigoIdioma) : p).ToArray();

            try
            {
                return string.Format(Cultura(codigoUsado), texto, resueltos);
            }
            catch (FormatException)
            {
                return texto;
            }
        }

        public static bool Existe(string clave)
        {
            return !string.IsNullOrEmpty(clave) && Buscar(clave, CodigoBase) != null;
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
                cargadosEn.Clear();
                codigoBase = null;
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

                DateTime cargadoEn;
                bool existe = catalogos.TryGetValue(codigoIdioma, out catalogo);

                if (existe && cargadosEn.TryGetValue(codigoIdioma, out cargadoEn) && DateTime.UtcNow - cargadoEn <= VIGENCIA) return catalogo;

                bool leido;
                Dictionary<string, string> nuevo = Cargar(codigoIdioma, out leido);

                if (!leido) return existe ? catalogo : nuevo;

                catalogos[codigoIdioma] = nuevo;
                cargadosEn[codigoIdioma] = DateTime.UtcNow;

                return nuevo;
            }
        }

        private static Dictionary<string, string> Cargar(string codigoIdioma, out bool leido)
        {
            var vacio = new Dictionary<string, string>(StringComparer.Ordinal);

            try
            {
                Idioma_TE idioma = new IdiomaRepository().ObtenerPorCodigo(codigoIdioma);

                leido = true;

                return idioma == null ? vacio : new TraduccionRepository().ObtenerTextos(idioma.IdIdioma);
            }
            catch (Exception)
            {
                leido = false;

                return vacio;
            }
        }

        private static string LeerCodigoBase()
        {
            try
            {
                Idioma_TE idiomaBase = new IdiomaRepository().ObtenerBase();

                return idiomaBase == null ? null : idiomaBase.CodigoIdioma;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
