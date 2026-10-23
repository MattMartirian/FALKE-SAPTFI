namespace TLL
{
    public interface IMensajeTraducible_TLL
    {
        string Clave { get; }
        object[] Parametros { get; }
        string Traducir(string codigoIdioma);
    }
}
