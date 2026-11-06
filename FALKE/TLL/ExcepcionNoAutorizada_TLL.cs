using System;

namespace TLL
{
    public class ExcepcionNoAutorizada_TLL : UnauthorizedAccessException, IMensajeTraducible_TLL
    {
        public string Clave { get; private set; }
        public object[] Parametros { get; private set; }

        public ExcepcionNoAutorizada_TLL(string clave, params object[] parametros) : base(clave)
        {
            Clave = clave;
            Parametros = parametros ?? new object[0];
        }

        public override string Message
        {
            get { return Traducir(Traductor_TLL.CODIGO_DIAGNOSTICO); }
        }

        public string Traducir(string codigoIdioma)
        {
            return Traductor_TLL.Traducir(Clave, codigoIdioma, Parametros);
        }
    }
}
