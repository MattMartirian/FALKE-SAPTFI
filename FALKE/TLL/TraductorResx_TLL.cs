using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace TLL
{
    public class TraductorResx_TLL : ITraductorAutomatico_TLL
    {
        private const string PREFIJO = "Recursos.Textos.";
        private const string SUFIJO = ".resources";

        private static readonly Assembly ensamblado = typeof(TraductorResx_TLL).Assembly;

        public List<string> ObtenerCodigosDisponibles()
        {
            var codigos = new List<string>();

            foreach (string nombre in ensamblado.GetManifestResourceNames())
            {
                string codigo = ExtraerCodigo(nombre);
                if (codigo != null) codigos.Add(codigo);
            }

            codigos.Sort(StringComparer.OrdinalIgnoreCase);
            return codigos;
        }

        public bool Disponible(string codigoIdioma)
        {
            return BuscarRecurso(codigoIdioma) != null;
        }

        public Dictionary<string, string> ObtenerTraducciones(string codigoIdioma)
        {
            var textos = new Dictionary<string, string>(StringComparer.Ordinal);

            string recurso = BuscarRecurso(codigoIdioma);
            if (recurso == null) return textos;

            var manager = new ResourceManager(recurso.Substring(0, recurso.Length - SUFIJO.Length), ensamblado);
            ResourceSet conjunto = manager.GetResourceSet(CultureInfo.InvariantCulture, true, false);

            if (conjunto == null) return textos;

            foreach (DictionaryEntry entrada in conjunto)
            {
                string texto = entrada.Value as string;
                if (texto != null) textos[Convert.ToString(entrada.Key)] = texto;
            }

            return textos;
        }

        private static string BuscarRecurso(string codigoIdioma)
        {
            if (string.IsNullOrWhiteSpace(codigoIdioma)) return null;

            foreach (string nombre in ensamblado.GetManifestResourceNames())
            {
                if (string.Equals(ExtraerCodigo(nombre), codigoIdioma.Trim(), StringComparison.OrdinalIgnoreCase)) return nombre;
            }

            return null;
        }

        private static string ExtraerCodigo(string nombreRecurso)
        {
            int inicio = nombreRecurso.IndexOf(PREFIJO, StringComparison.OrdinalIgnoreCase);

            if (inicio < 0 || !nombreRecurso.EndsWith(SUFIJO, StringComparison.OrdinalIgnoreCase)) return null;

            inicio += PREFIJO.Length;
            int largo = nombreRecurso.Length - SUFIJO.Length - inicio;

            return largo > 0 ? nombreRecurso.Substring(inicio, largo) : null;
        }
    }
}
