using System;
using System.Collections.Generic;
using System.Threading;
using Tobii.StreamEngine;

namespace AgenteMirada
{
    public class FuenteTobii : IFuenteMirada
    {
        private const int ESPERA_REINTENTO_MS = 2000;
        private const int ESPERA_CIERRE_MS = 3000;

        private static FuenteTobii instancia;

        private Thread hilo;
        private volatile bool detener;
        private string ultimoEstado;

        public event Action<MuestraMirada> MuestraRecibida;
        public event Action<string> EstadoCambiado;

        public void Iniciar()
        {
            instancia = this;
            detener = false;

            hilo = new Thread(Ejecutar);
            hilo.IsBackground = true;
            hilo.Name = "FuenteTobii";
            hilo.Start();
        }

        public void Detener()
        {
            detener = true;

            if (hilo != null) hilo.Join(ESPERA_CIERRE_MS);
        }

        private void Ejecutar()
        {
            IntPtr api;

            if (!Correcto(Interop.tobii_api_create(out api, null)))
            {
                Publicar("error_sdk");
                return;
            }

            try
            {
                while (!detener)
                {
                    LeerDispositivo(api);

                    if (detener) break;

                    Publicar("sin_tracker");
                    Thread.Sleep(ESPERA_REINTENTO_MS);
                }
            }
            finally
            {
                Interop.tobii_api_destroy(api);
            }
        }

        private void LeerDispositivo(IntPtr api)
        {
            List<string> urls;

            if (!Correcto(Interop.tobii_enumerate_local_device_urls(api, out urls)) || urls.Count == 0) return;

            IntPtr dispositivo;

            if (!Correcto(Interop.tobii_device_create(api, urls[0], Interop.tobii_field_of_use_t.TOBII_FIELD_OF_USE_STORE_OR_TRANSFER_FALSE, out dispositivo))) return;

            try
            {
                if (!Correcto(Interop.tobii_gaze_point_subscribe(dispositivo, AlRecibirMirada))) return;

                Publicar("conectado");

                while (!detener)
                {
                    tobii_error_t espera = Interop.tobii_wait_for_callbacks(new[] { dispositivo });

                    if (!Correcto(espera) && espera != tobii_error_t.TOBII_ERROR_TIMED_OUT) break;

                    if (!Correcto(Interop.tobii_device_process_callbacks(dispositivo))) break;
                }

                Interop.tobii_gaze_point_unsubscribe(dispositivo);
            }
            finally
            {
                Interop.tobii_device_destroy(dispositivo);
            }
        }

        private static void AlRecibirMirada(ref tobii_gaze_point_t punto, IntPtr usuario)
        {
            FuenteTobii fuente = instancia;

            if (fuente == null) return;

            var muestra = new MuestraMirada
            {
                X = punto.position.x,
                Y = punto.position.y,
                Valida = punto.validity == tobii_validity_t.TOBII_VALIDITY_VALID,
                TiempoUs = punto.timestamp_us,
                UtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            fuente.MuestraRecibida?.Invoke(muestra);
        }

        private void Publicar(string estado)
        {
            if (estado == ultimoEstado) return;

            ultimoEstado = estado;

            EstadoCambiado?.Invoke(estado);
        }

        private static bool Correcto(tobii_error_t resultado)
        {
            return resultado == tobii_error_t.TOBII_ERROR_NO_ERROR;
        }
    }
}
