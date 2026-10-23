using System.Collections.Generic;
using System.Web;

namespace GUI
{
    public static class OrigenTexto_GUI
    {
        private const string K_ORIGENES = "OrigenTexto_GUI";

        private class Origen
        {
            public string Clave;
            public object[] Parametros;
        }

        public static void Registrar(string texto, string clave, object[] parametros)
        {
            HttpContext contexto = HttpContext.Current;

            if (contexto == null || string.IsNullOrEmpty(texto) || string.IsNullOrEmpty(clave)) return;

            object[] copia = parametros ?? new object[0];

            if (!SonSerializables(copia)) return;

            var origenes = contexto.Items[K_ORIGENES] as Dictionary<string, Origen>;

            if (origenes == null)
            {
                origenes = new Dictionary<string, Origen>();
                contexto.Items[K_ORIGENES] = origenes;
            }

            origenes[texto] = new Origen { Clave = clave, Parametros = copia };
        }

        public static bool Buscar(string texto, out string clave, out object[] parametros)
        {
            clave = null;
            parametros = null;

            HttpContext contexto = HttpContext.Current;
            var origenes = contexto == null ? null : contexto.Items[K_ORIGENES] as Dictionary<string, Origen>;
            Origen origen;

            if (origenes == null || texto == null || !origenes.TryGetValue(texto, out origen)) return false;

            clave = origen.Clave;
            parametros = origen.Parametros;

            return true;
        }

        private static bool SonSerializables(object[] parametros)
        {
            foreach (object parametro in parametros)
            {
                if (parametro == null) continue;

                if (!(parametro is string || parametro is bool || parametro is char || parametro is byte || parametro is short || parametro is int || parametro is long
                    || parametro is decimal || parametro is double || parametro is float || parametro is System.DateTime)) return false;
            }

            return true;
        }
    }
}
