using System.Collections.Generic;

namespace TLL
{
    public class SujetoIdioma_TLL
    {
        private readonly List<IObservadorIdioma_TLL> observadores = new List<IObservadorIdioma_TLL>();
        private readonly object bloqueo = new object();

        public string CodigoIdioma { get; private set; }

        public SujetoIdioma_TLL() : this(Traductor_TLL.CODIGO_BASE) { }

        public SujetoIdioma_TLL(string codigoIdioma)
        {
            CodigoIdioma = codigoIdioma;
        }

        public void Registrar(IObservadorIdioma_TLL observador)
        {
            lock (bloqueo)
            {
                if (!observadores.Contains(observador)) observadores.Add(observador);
            }
        }

        public void Quitar(IObservadorIdioma_TLL observador)
        {
            lock (bloqueo)
            {
                observadores.Remove(observador);
            }
        }

        public void CambiarIdioma(string codigoIdioma)
        {
            CodigoIdioma = codigoIdioma;
            Notificar();
        }

        public void Notificar()
        {
            List<IObservadorIdioma_TLL> copia;

            lock (bloqueo)
            {
                copia = new List<IObservadorIdioma_TLL>(observadores);
            }

            foreach (IObservadorIdioma_TLL observador in copia) observador.ActualizarIdioma(CodigoIdioma);
        }
    }
}
