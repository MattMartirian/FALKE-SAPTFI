namespace TLL
{
    public class ResultadoToken
    {
        public bool Exito { get; private set; }
        public string Motivo { get; private set; }
        public string Email { get; private set; }

        private ResultadoToken() { }

        public static ResultadoToken Ok(string email)
        {
            return new ResultadoToken { Exito = true, Email = email };
        }

        public static ResultadoToken Falla(string motivo, string email = null)
        {
            return new ResultadoToken { Exito = false, Motivo = motivo, Email = email };
        }
    }
}
