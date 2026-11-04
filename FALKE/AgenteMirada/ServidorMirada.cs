using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgenteMirada
{
    public class ServidorMirada
    {
        private const int TAMANO_COLA = 200;

        private readonly HttpListener escucha = new HttpListener();
        private readonly HashSet<string> origenesPermitidos;
        private readonly ConcurrentDictionary<Guid, BlockingCollection<string>> clientes = new ConcurrentDictionary<Guid, BlockingCollection<string>>();

        private volatile string ultimoEstado = ArmarEstado("iniciando");
        private volatile bool activo;

        public ServidorMirada(int puerto, IEnumerable<string> origenes)
        {
            escucha.Prefixes.Add("http://localhost:" + puerto + "/");
            origenesPermitidos = new HashSet<string>(origenes, StringComparer.OrdinalIgnoreCase);
        }

        public void Iniciar()
        {
            activo = true;
            escucha.Start();

            Task.Run(() => AceptarAsync());
        }

        public void Detener()
        {
            activo = false;
            escucha.Close();
        }

        public void Difundir(MuestraMirada muestra)
        {
            string mensaje = string.Format(
                CultureInfo.InvariantCulture,
                "{{\"tipo\":\"mirada\",\"x\":{0:0.0000},\"y\":{1:0.0000},\"valida\":{2},\"t\":{3}}}",
                muestra.X,
                muestra.Y,
                muestra.Valida ? "true" : "false",
                muestra.TiempoUs);

            Enviar(mensaje);
        }

        public void DifundirEstado(string estado)
        {
            ultimoEstado = ArmarEstado(estado);

            Enviar(ultimoEstado);
        }

        private static string ArmarEstado(string estado)
        {
            return "{\"tipo\":\"estado\",\"estado\":\"" + estado + "\"}";
        }

        private void Enviar(string mensaje)
        {
            foreach (BlockingCollection<string> cola in clientes.Values)
            {
                try
                {
                    cola.TryAdd(mensaje);
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        private async Task AceptarAsync()
        {
            while (activo)
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

                Task.Run(() => AtenderAsync(contexto));
            }
        }

        private async Task AtenderAsync(HttpListenerContext contexto)
        {
            if (!contexto.Request.IsWebSocketRequest || !OrigenPermitido(contexto.Request.Headers["Origin"]))
            {
                contexto.Response.StatusCode = 403;
                contexto.Response.Close();
                return;
            }

            HttpListenerWebSocketContext conexion = await contexto.AcceptWebSocketAsync(null);
            WebSocket socket = conexion.WebSocket;
            var cola = new BlockingCollection<string>(TAMANO_COLA);
            Guid id = Guid.NewGuid();

            clientes[id] = cola;
            cola.TryAdd(ultimoEstado);

            try
            {
                await Task.WhenAny(EnviarAsync(socket, cola), RecibirAsync(socket));
            }
            finally
            {
                BlockingCollection<string> quitada;

                clientes.TryRemove(id, out quitada);
                cola.CompleteAdding();
                socket.Dispose();
            }
        }

        private bool OrigenPermitido(string origen)
        {
            return origen != null && origenesPermitidos.Contains(origen);
        }

        private static Task EnviarAsync(WebSocket socket, BlockingCollection<string> cola)
        {
            return Task.Run(async () =>
            {
                foreach (string mensaje in cola.GetConsumingEnumerable())
                {
                    byte[] datos = Encoding.UTF8.GetBytes(mensaje);

                    await socket.SendAsync(new ArraySegment<byte>(datos), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            });
        }

        private static async Task RecibirAsync(WebSocket socket)
        {
            var buffer = new byte[1024];

            while (socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult resultado = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (resultado.MessageType == WebSocketMessageType.Close) break;
            }
        }
    }
}
