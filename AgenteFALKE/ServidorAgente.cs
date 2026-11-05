using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AgenteMirada
{
    public class ServidorAgente
    {
        private const string ENLACE_TOBII = "tobiieyetracking:";

        private readonly HttpListener escucha = new HttpListener();
        private readonly Grabador grabador;

        public ServidorAgente(int puerto, Grabador grabador)
        {
            escucha.Prefixes.Add("http://localhost:" + puerto + "/");
            this.grabador = grabador;
        }

        public void Iniciar()
        {
            escucha.Start();

            Task.Run(() => AceptarAsync());
        }

        public void Detener()
        {
            escucha.Close();
        }

        private async Task AceptarAsync()
        {
            while (escucha.IsListening)
            {
                HttpListenerContext contexto;

                try
                {
                    contexto = await escucha.GetContextAsync();
                }
                catch (Exception)
                {
                    break;
                }

                Atender(contexto);
            }
        }

        private void Atender(HttpListenerContext contexto)
        {
            HttpListenerResponse respuesta = contexto.Response;

            try
            {
                respuesta.AddHeader("Access-Control-Allow-Origin", "*");
                respuesta.AddHeader("Access-Control-Allow-Methods", "GET, OPTIONS");
                respuesta.AddHeader("Access-Control-Allow-Private-Network", "true");

                if (contexto.Request.HttpMethod == "OPTIONS")
                {
                    respuesta.StatusCode = 204;
                    return;
                }

                switch (contexto.Request.Url.AbsolutePath)
                {
                    case "/estado":
                        Responder(respuesta, ArmarEstado());
                        break;
                    case "/iniciar":
                        Responder(respuesta, "{\"inicio_utc\":" + grabador.Iniciar() + "}");
                        break;
                    case "/detener":
                        Responder(respuesta, grabador.Detener() ?? "{}");
                        break;
                    case "/tobii":
                        Process.Start(new ProcessStartInfo(ENLACE_TOBII) { UseShellExecute = true });
                        Responder(respuesta, "{}");
                        break;
                    case "/archivo":
                        EntregarArchivo(respuesta);
                        break;
                    default:
                        respuesta.StatusCode = 404;
                        break;
                }
            }
            catch (Exception error)
            {
                Registro.Escribir(contexto.Request.Url.AbsolutePath + ": " + error.Message);
                respuesta.StatusCode = 500;
            }
            finally
            {
                respuesta.Close();
            }
        }

        private string ArmarEstado()
        {
            string estado = grabador.EstadoFuente;
            bool simulado = estado == Estados.ConectadoSimulado;
            bool conectado = estado == Estados.Conectado || simulado;

            return "{\"estado\":\"" + estado + "\",\"conectado\":" + Json(conectado) + ",\"simulado\":" + Json(simulado) + ",\"grabando\":" + Json(grabador.Grabando) + ",\"muestras\":" + grabador.Muestras + "}";
        }

        private static string Json(bool valor)
        {
            return valor ? "true" : "false";
        }

        private void EntregarArchivo(HttpListenerResponse respuesta)
        {
            if (grabador.Grabando || !File.Exists(grabador.Ruta))
            {
                respuesta.StatusCode = 409;
                return;
            }

            byte[] datos = File.ReadAllBytes(grabador.Ruta);

            respuesta.ContentType = "application/json";
            respuesta.ContentLength64 = datos.Length;
            respuesta.OutputStream.Write(datos, 0, datos.Length);
        }

        private static void Responder(HttpListenerResponse respuesta, string json)
        {
            byte[] datos = Encoding.UTF8.GetBytes(json);

            respuesta.ContentType = "application/json";
            respuesta.ContentLength64 = datos.Length;
            respuesta.OutputStream.Write(datos, 0, datos.Length);
        }
    }
}
