namespace BE
{
    public class ErrorValidacion_BE
    {
        public string Codigo { get; private set; }
        public object[] Parametros { get; private set; }

        public ErrorValidacion_BE(string codigo, params object[] parametros)
        {
            Codigo = codigo;
            Parametros = parametros ?? new object[0];
        }
    }
}
