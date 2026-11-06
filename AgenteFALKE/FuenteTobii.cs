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

        private readonly tobii_gaze_point_callback_t receptorMirada;

        private Thread hilo;
        private volatile bool detener;
        private string ultimoEstado;

        public event Action<MuestraMirada> MuestraRecibida;
        public event Action<string> EstadoCambiado;

        public FuenteTobii()
        {
            receptorMirada = AlRecibirMirada;
        }

        public void Iniciar()
        {
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
            while (!detener)
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
                    Publicar(Estados.ErrorSdk);
                    return;
                }
                catch (BadImageFormatException)
                {
                    Registro.Escribir("tobii_stream_engine.dll no es de 64 bits.");
                    Publicar(Estados.ErrorSdk);
                    return;
                }

                if (Correcto(creado))
                {
                    try
                    {
                        LeerDispositivo(api);
                    }
                    finally
                    {
                        Interop.tobii_api_destroy(api);
                    }

                    Publicar(Estados.SinTracker);
                }
                else
                {
                    Registro.Escribir("tobii_api_create: " + creado);
                    Publicar(Estados.ErrorSdk);
                }

                if (!detener) Thread.Sleep(ESPERA_REINTENTO_MS);
            }
        }

        private void LeerDispositivo(IntPtr api)
        {
            List<string> urls = Interop.ListarDispositivos(api);

            if (urls.Count == 0)
            {
                Registro.Escribir("El SDK de Tobii no lista ningun eye tracker.");
                return;
            }

            IntPtr dispositivo;

            tobii_error_t resultado = Interop.tobii_device_create(api, urls[0], tobii_field_of_use_t.TOBII_FIELD_OF_USE_STORE_OR_TRANSFER_FALSE, out dispositivo);

            if (!Correcto(resultado))
            {
                Registro.Escribir("tobii_device_create: " + resultado);
                return;
            }

            try
            {
                resultado = Interop.tobii_gaze_point_subscribe(dispositivo, receptorMirada, IntPtr.Zero);

                if (!Correcto(resultado))
                {
                    Registro.Escribir("tobii_gaze_point_subscribe: " + resultado);
                    return;
                }

                Publicar(Estados.Conectado);

                while (!detener)
                {
                    Thread.Sleep(INTERVALO_LECTURA_MS);

                    resultado = Interop.tobii_device_process_callbacks(dispositivo);

                    if (!Correcto(resultado))
                    {
                        Registro.Escribir("tobii_device_process_callbacks: " + resultado);
                        break;
                    }
                }

                Interop.tobii_gaze_point_unsubscribe(dispositivo);
            }
            finally
            {
                Interop.tobii_device_destroy(dispositivo);
            }
        }

        private void AlRecibirMirada(ref tobii_gaze_point_t punto, IntPtr usuario)
        {
            var muestra = new MuestraMirada
            {
                X = punto.x,
                Y = punto.y,
                Valida = punto.validity == tobii_validity_t.TOBII_VALIDITY_VALID,
                TiempoUs = punto.timestamp_us,
                UtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            MuestraRecibida?.Invoke(muestra);
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
