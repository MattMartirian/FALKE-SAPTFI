using TLL;

namespace GUI
{
    public static class TextoBitacora_GUI
    {
        public static string Modulo(object modulo)
        {
            return TextoBitacora_TLL.Modulo(modulo as string, IdiomaActual_GUI.Codigo);
        }

        public static string Descripcion(object descripcion)
        {
            return TextoBitacora_TLL.Descripcion(descripcion as string, IdiomaActual_GUI.Codigo);
        }

        public static string Detalle(object detalle)
        {
            string texto = TextoBitacora_TLL.Detalle(detalle as string, IdiomaActual_GUI.Codigo);

            return texto.Length == 0 ? "-" : texto;
        }
    }
}
