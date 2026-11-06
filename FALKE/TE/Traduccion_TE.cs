namespace TE
{
    public enum OrigenTraduccion
    {
        Resx = 1,
        Manual = 2,
        Auto = 3
    }

    public class Traduccion_TE
    {
        public string CodigoClave { get; set; }
        public int IdIdioma { get; set; }
        public string Texto { get; set; }
        public bool Pendiente { get; set; }
        public OrigenTraduccion Origen { get; set; }

        public Traduccion_TE() { }

        public Traduccion_TE(string codigoClave, int idIdioma, string texto, bool pendiente, OrigenTraduccion origen)
        {
            CodigoClave = codigoClave;
            IdIdioma = idIdioma;
            Texto = texto;
            Pendiente = pendiente;
            Origen = origen;
        }
    }

    public class TraduccionClave_TE
    {
        public string CodigoClave { get; set; }
        public int IdIdioma { get; set; }

        public TraduccionClave_TE() { }

        public TraduccionClave_TE(string codigoClave, int idIdioma)
        {
            CodigoClave = codigoClave;
            IdIdioma = idIdioma;
        }
    }
}
