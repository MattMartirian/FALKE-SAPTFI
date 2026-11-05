using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace AgenteMirada
{
    /// <summary>
    /// Junta las muestras en memoria (10 por segundo) y, al detener, escribe un unico JSON valido.
    /// Si la grabacion se interrumpe antes de detener, no queda archivo: se pierde.
    /// </summary>
    public class Grabador
    {
        private const int INTERVALO_MUESTREO_MS = 100;

        private readonly object candado = new object();
        private StringBuilder muestrasJson;
        private bool grabando;
        private long inicioUtc;
        private long proximoUtc;
        private int muestras;

        public Grabador(string ruta)
        {
            Ruta = ruta;
            EstadoFuente = Estados.Iniciando;
        }

        public string Ruta { get; private set; }
        public volatile string EstadoFuente;

        public int Muestras
        {
            get { lock (candado) { return muestras; } }
        }

        public bool Grabando
        {
            get { lock (candado) { return grabando; } }
        }

        public long Iniciar()
        {
            lock (candado)
            {
                inicioUtc = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                proximoUtc = inicioUtc;
                muestras = 0;
                muestrasJson = new StringBuilder();
                grabando = true;

                if (File.Exists(Ruta)) File.Delete(Ruta);

                return inicioUtc;
            }
        }

        public void Agregar(MuestraMirada m)
        {
            lock (candado)
            {
                if (!grabando || m.UtcMs < proximoUtc) return;

                proximoUtc = Math.Max(proximoUtc + INTERVALO_MUESTREO_MS, m.UtcMs + 1);

                if (muestras > 0) muestrasJson.Append(',');

                muestrasJson.Append(string.Format(
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
                if (!grabando) return null;

                long fin = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                string contenido = "{\"v\":1,\"inicio_utc\":" + inicioUtc + ",\"fin_utc\":" + fin + ",\"muestras\":[" + muestrasJson + "]}";
                File.WriteAllText(Ruta, contenido, new UTF8Encoding(false));

                grabando = false;
                muestrasJson = null;

                return "{\"inicio_utc\":" + inicioUtc + ",\"fin_utc\":" + fin + ",\"muestras\":" + muestras + "}";
            }
        }
    }
}
