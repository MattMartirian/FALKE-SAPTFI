using System;
using System.Collections.Generic;
using System.Threading;

namespace AgenteMirada
{
    public class FuenteTobii : IFuenteMirada
    {
        private const int ESPERA_REINTENTO_MS = 2000;
        private const int ESPERA_CIERRE_MS = 3000;
        private const int INTERVALO_LECTURA_MS = 5;

        private static FuenteTobii instancia;
        private static readonly tobii_gaze_point_callback_t receptorMirada = AlRecibirMirada;

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
            tobii_error_t creado;

            try
            {
                creado = Interop.tobii_api_create(out api, IntPtr.Zero, IntPtr.Zero);
            }
            catch (DllNotFoundException)
            {
                Registro.Escribir("Falta tobii_stream_engine.dll junto a AgenteMirada.exe.");
                Publicar("error_sdk");
                return;
            }
            catch (BadImageFormatException)
            {
                Registro.Escribir("tobii_stream_engine.dll no es de 64 bits.");
                Publicar("error_sdk");
                return;
            }

            if (!Correcto(creado))
            {
                Registro.Escribir("tobii_api_create: " + creado);
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
            List<string> urls = Interop.ListarDispositivos(api);

            if (urls.Count == 0) return;

            IntPtr dispositivo;

            tobii_error_t creado = Interop.tobii_device_create(api, urls[0], tobii_field_of_use_t.TOBII_FIELD_OF_USE_STORE_OR_TRANSFER_FALSE, out dispositivo);

            if (!Correcto(creado))
            {
                Registro.Escribir("tobii_device_create: " + creado);
                return;
            }

            try
            {
                if (!Correcto(Interop.tobii_gaze_point_subscribe(dispositivo, receptorMirada, IntPtr.Zero))) return;

                Publicar("conectado");

                while (!detener)
                {
                    Thread.Sleep(INTERVALO_LECTURA_MS);

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
                X = punto.x,
                Y = punto.y,
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
