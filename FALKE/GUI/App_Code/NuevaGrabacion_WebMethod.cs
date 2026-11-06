using System;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    // Web methods de Nueva grabacion: suben el video por partes y guardan la sesion.
    // La pagina los llama con fetch a NuevaGrabacion_WebMethod.asmx/<metodo> (ver urlMetodos en NuevaGrabacion.aspx).
    [WebService(Namespace = "http://falke.local/")]
    [ScriptService]
    public class NuevaGrabacion_WebMethod : WebService
    {
        private const string CARPETA_GRABACIONES = "~/App_Data/Grabaciones";
        private const int HORAS_VIDA_TEMPORAL = 24;

        public class DatosSesion
        {
            public int idCategoria { get; set; }
            public int idDispositivo { get; set; }
            public string nombre { get; set; }
            public string objetivo { get; set; }
            public string edad { get; set; }
            public string genero { get; set; }
            public string familiaridad { get; set; }
            public int duracion { get; set; }
            public int desfaseMiradaMs { get; set; }
        }

        public class RespuestaGuardado
        {
            public bool ok { get; set; }
            public string mensaje { get; set; }
            public string token { get; set; }
            public int idSesion { get; set; }
            public string siguienteNombre { get; set; }
        }

        [WebMethod(EnableSession = true)]
        public RespuestaGuardado IniciarSubida()
        {
            return Ejecutar(actor =>
            {
                actor.Exigir(Patentes_TLL.OPERAR_ANALISIS);

                string carpeta = CarpetaTemporal();
                Directory.CreateDirectory(carpeta);
                LimpiarTemporales(carpeta);

                string token = Guid.NewGuid().ToString("N");
                using (File.Create(RutaTemporal(actor, token))) { }

                return new RespuestaGuardado { ok = true, token = token };
            });
        }

        [WebMethod(EnableSession = true)]
        public RespuestaGuardado SubirParte(string token, long desde, string base64)
        {
            return Ejecutar(actor =>
            {
                actor.Exigir(Patentes_TLL.OPERAR_ANALISIS);

                string ruta = RutaTemporal(actor, token);
                byte[] bytes = Convert.FromBase64String(base64 ?? string.Empty);

                using (var archivo = new FileStream(ruta, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    if (archivo.Length == desde + bytes.Length) return new RespuestaGuardado { ok = true };

                    if (archivo.Length != desde) throw new ExcepcionTraducible_TLL("sesion_error_guardado");
                    if (archivo.Length + bytes.Length > SesionGrabada_BLL.MAX_BYTES_VIDEO) throw new ExcepcionTraducible_TLL("sesion_video_muy_grande");

                    archivo.Seek(0, SeekOrigin.End);
                    archivo.Write(bytes, 0, bytes.Length);
                }

                return new RespuestaGuardado { ok = true };
            });
        }

        [WebMethod(EnableSession = true)]
        public RespuestaGuardado FinalizarSubida(string token, DatosSesion datos, string mirada)
        {
            return Ejecutar(actor =>
            {
                if (datos == null) throw new ExcepcionTraducible_TLL("comun_solicitud_invalida");

                string ruta = RutaTemporal(actor, token);

                var sesion = new SesionGrabada_BE
                {
                    IdCategoria = datos.idCategoria,
                    IdDispositivo = datos.idDispositivo,
                    Nombre = datos.nombre,
                    Objetivo = datos.objetivo,
                    RangoEdad = datos.edad,
                    Genero = datos.genero,
                    Familiaridad = datos.familiaridad,
                    DuracionSegundos = datos.duracion,
                    DesfaseMiradaMs = datos.desfaseMiradaMs
                };

                MemoryStream streamMirada = null;
                if (!string.IsNullOrWhiteSpace(mirada))
                {
                    try { new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 8 }.DeserializeObject(mirada); }
                    catch { throw new ExcepcionTraducible_TLL("sesion_error_guardado"); }

                    streamMirada = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(mirada));
                }

                var bll = new SesionGrabada_BLL();

                SesionGrabada_BE guardada;

                using (var video = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    guardada = bll.Guardar(actor, sesion, video, streamMirada, HttpContext.Current.Server.MapPath(CARPETA_GRABACIONES));
                }

                BorrarTemporal(ruta);

                return new RespuestaGuardado
                {
                    ok = true,
                    idSesion = guardada.IdSesion,
                    siguienteNombre = bll.ProponerNombreSiguiente(actor, guardada.IdCategoria, guardada.Nombre)
                };
            });
        }

        private static RespuestaGuardado Ejecutar(Func<ActorUsuario_TE, RespuestaGuardado> paso)
        {
            try
            {
                ActorUsuario_TE actor = SesionActual_GUI.ObtenerActor();
                if (actor == null) return new RespuestaGuardado { mensaje = IdiomaActual_GUI.Texto("comun_sesion_expirada") };

                return paso(actor);
            }
            catch (Exception ex)
            {
                // En C# solo se puede capturar un tipo que herede de Exception, no una interfaz:
                // por eso se pregunta si la excepcion implementa IMensajeTraducible_TLL.
                var traducible = ex as IMensajeTraducible_TLL;

                if (traducible != null) return new RespuestaGuardado { mensaje = traducible.Traducir(IdiomaActual_GUI.Codigo) };

                SERVICES.LogErrores_SERVICE.Registrar("NuevaGrabacion.Guardar", ex);
                return new RespuestaGuardado { mensaje = IdiomaActual_GUI.Texto("sesion_error_guardado") };
            }
        }

        private static string CarpetaTemporal()
        {
            return Path.Combine(HttpContext.Current.Server.MapPath(CARPETA_GRABACIONES), "_tmp");
        }

        private static string RutaTemporal(ActorUsuario_TE actor, string token)
        {
            Guid guid;
            if (!Guid.TryParseExact(token ?? string.Empty, "N", out guid)) throw new ExcepcionTraducible_TLL("comun_solicitud_invalida");

            return Path.Combine(CarpetaTemporal(), actor.IdUsuario + "_" + guid.ToString("N") + ".part");
        }

        private static void LimpiarTemporales(string carpeta)
        {
            foreach (string archivo in Directory.GetFiles(carpeta, "*.part"))
            {
                if (File.GetLastWriteTimeUtc(archivo) < DateTime.UtcNow.AddHours(-HORAS_VIDA_TEMPORAL)) BorrarTemporal(archivo);
            }
        }

        private static void BorrarTemporal(string ruta)
        {
            try { if (File.Exists(ruta)) File.Delete(ruta); } catch { }
        }
    }
}
