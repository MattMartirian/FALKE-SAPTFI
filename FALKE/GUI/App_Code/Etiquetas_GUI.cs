using System.Collections.Generic;
using System.Web;
using TLL;

namespace GUI
{
    // Lo que se muestra de cada rol, grupo o permiso: su descripción (PermisoTable.descripcion_permiso) o, si no tiene, el nombre interno.
    public static class Etiquetas_GUI
    {
        private const string CLAVE = "FALKE_ETIQUETAS_PERMISOS";

        public static string De(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return string.Empty;

            return Permiso_TLL.Etiqueta(Cargar(), nombre);
        }

        // Se lee una sola vez por pedido.
        private static Dictionary<string, string> Cargar()
        {
            HttpContext contexto = HttpContext.Current;
            var guardadas = contexto == null ? null : contexto.Items[CLAVE] as Dictionary<string, string>;

            if (guardadas != null) return guardadas;

            guardadas = new Permiso_TLL().ObtenerEtiquetas();
            if (contexto != null) contexto.Items[CLAVE] = guardadas;

            return guardadas;
        }

        // Para que un cambio de descripción hecho en este mismo pedido se vea enseguida.
        public static void Olvidar()
        {
            if (HttpContext.Current != null) HttpContext.Current.Items.Remove(CLAVE);
        }
    }
}
