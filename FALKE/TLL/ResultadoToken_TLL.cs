namespace TLL
{
    public class ResultadoToken_TLL
    {
        public bool Exito { get; private set; }
        public string Motivo { get; private set; }
        public string Email { get; private set; }

        private ResultadoToken_TLL() { }

        public static ResultadoToken_TLL Ok(string email)
        {
            return new ResultadoToken_TLL { Exito = true, Email = email };
        }

        public static ResultadoToken_TLL Falla(string motivo, string email = null)
        {
            return new ResultadoToken_TLL { Exito = false, Motivo = motivo, Email = email };
        }
    }
}
