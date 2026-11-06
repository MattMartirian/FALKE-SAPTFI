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

        public static string NormalizarEmail(string email)
        {
            return email == null ? null : email.Trim().ToLowerInvariant();
        }

        public static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            int arroba = email.IndexOf('@');
            if (arroba <= 0 || arroba != email.LastIndexOf('@')) return false;

            int punto = email.IndexOf('.', arroba);
            return punto > arroba + 1 && punto < email.Length - 1;
        }
    }
}
