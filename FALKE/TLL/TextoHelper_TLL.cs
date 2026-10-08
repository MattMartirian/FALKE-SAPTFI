namespace TLL
{
    public static class TextoHelper_TLL
    {
        public static string Recortar(string texto)
        {
            string limpio = (texto ?? string.Empty).Trim();

            return limpio.Length == 0 ? null : limpio;
        }

        public static int Largo(string texto)
        {
            return texto == null ? 0 : texto.Length;
        }
    }
}
