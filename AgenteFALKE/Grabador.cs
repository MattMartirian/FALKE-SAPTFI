using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace AgenteMirada
{
    public class Grabador
    {
        private readonly object candado = new object();
        private StreamWriter escritor;
        private long inicioUtc;
        private int muestras;

        public Grabador(string ruta)
        {
            Ruta = ruta;
            EstadoFuente = "iniciando";
        }

        public string Ruta { get; private set; }
        public volatile string EstadoFuente;

        public bool Grabando
        {
            get { lock (candado) { return escritor != null; } }
        }

        public long Iniciar()
        {
            lock (candado)
            {
                if (escritor != null) escritor.Dispose();

                inicioUtc = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                muestras = 0;
                escritor = new StreamWriter(Ruta, false, new UTF8Encoding(false));
                escritor.WriteLine("{\"v\":1,\"inicio_utc\":" + inicioUtc + "}");

                return inicioUtc;
            }
        }

        public void Agregar(MuestraMirada m)
        {
            lock (candado)
            {
                if (escritor == null) return;

                escritor.WriteLine(string.Format(
                    CultureInfo.InvariantCulture,
                    "{{\"u\":{0},\"t\":{1},\"x\":{2:0.0000},\"y\":{3:0.0000},\"ok\":{4}}}",
                    m.UtcMs,
                    m.TiempoUs,
                    m.Valida ? m.X : 0,
                    m.Valida ? m.Y : 0,
                    m.Valida ? 1 : 0));

                muestras++;
            }
        }

        public string Detener()
        {
            lock (candado)
            {
                if (escritor == null) return null;

                long fin = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                escritor.WriteLine("{\"fin_utc\":" + fin + "}");
                escritor.Dispose();
                escritor = null;

                return "{\"inicio_utc\":" + inicioUtc + ",\"fin_utc\":" + fin + ",\"muestras\":" + muestras + "}";
            }
        }
    }
}
