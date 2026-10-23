using System;
using System.Text.RegularExpressions;
using TE;

namespace TLL
{
    public static class Mensajes_TLL
    {
        private static readonly Regex formatoClave = new Regex(@"^[a-z][a-z0-9_]*$", RegexOptions.Compiled);

        public static string Interpretar(Exception ex, string codigoIdioma)
        {
            string clave;
            object[] parametros;

            if (Descomponer(ex, out clave, out parametros)) return Traductor_TLL.Traducir(clave, codigoIdioma, parametros);

            return ex.Message;
        }

        public static bool Descomponer(Exception ex, out string clave, out object[] parametros)
        {
            clave = null;
            parametros = null;

            var traducible = ex as IMensajeTraducible_TLL;
            if (traducible != null)
            {
                clave = traducible.Clave;
                parametros = traducible.Parametros;
                return true;
            }

            var permiso = ex as PermisoInvalidoException;
            if (permiso != null && permiso.Codigo != null)
            {
                clave = permiso.Codigo;
                parametros = permiso.Parametros;
                return true;
            }

            if (formatoClave.IsMatch(ex.Message) && Traductor_TLL.Existe(ex.Message))
            {
                clave = ex.Message;
                parametros = new object[0];
                return true;
            }

            return false;
        }
    }
}
