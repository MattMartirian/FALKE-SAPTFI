using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using TE;
using TLL;
using BE;
using BLL;
using SERVICES;

namespace GUI
{
    public partial class NuevaGrabacion : PaginaBase_GUI
    {
        protected const string URL_AGENTE = "http://localhost:8887";
        protected const int ESPERA_AGENTE_MS = 2500;
        protected const int SONDEO_MS = 3000;
        protected const int SONDEO_GRABACION_MS = 1000;
        protected const int ANCHO_VIDEO = 1280;
        protected const int ALTO_VIDEO = 720;
        protected const int FPS_VIDEO = 30;
        protected const int BITS_VIDEO_POR_SEGUNDO = 2500000;

        private static readonly int[] RETRASOS_SEGUNDOS = { 0, 3, 5, 10 };

        public class Chequeo
        {
            public string Id { get; set; }
            public string Nombre { get; set; }
            public string Inicial { get; set; }
            public string Clase { get; set; }
            public bool Obligatorio { get; set; }
            public bool ConEnlaceInstalacion { get; set; }
            public bool ConBotonTobii { get; set; }
            public bool ConCasilla { get; set; }
            public Dictionary<string, string> Textos { get; set; }

            public Chequeo()
            {
                Textos = new Dictionary<string, string>();
            }
        }

        public class Opcion
        {
            public string Valor { get; set; }
            public string Texto { get; set; }
        }

        // Solo se muestra: la sesion siempre se guarda con el ID del usuario logueado (el servidor no acepta otro).
        protected string NombreAnalista
        {
            get { return SesionActual_GUI.HayUsuario ? (SesionActual_GUI.Nombre ?? string.Empty) : string.Empty; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(TLL.Patentes_TLL.OPERAR_ANALISIS)) return;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            rptChequeos.DataSource = ArmarChequeos();
            rptChequeos.DataBind();

            rptCategorias.DataSource = ArmarCategorias();
            rptCategorias.DataBind();

            rptEdades.DataSource = ArmarOpciones(PerfilTester_BE.RangosEdad, null);
            rptEdades.DataBind();
            rptGeneros.DataSource = ArmarOpciones(PerfilTester_BE.Generos, "grabacion_tester_genero_");
            rptGeneros.DataBind();
            rptFamiliaridades.DataSource = ArmarOpciones(PerfilTester_BE.Familiaridades, "grabacion_tester_familiaridad_");
            rptFamiliaridades.DataBind();

            rptRetrasos.DataSource = ArmarRetrasos();
            rptRetrasos.DataBind();
        }

        private List<Opcion> ArmarCategorias()
        {
            var opciones = new List<Opcion>();

            try
            {
                var actor = SesionActual_GUI.ObtenerActor();
                if (actor == null) return opciones;

                foreach (Categoria_BE c in new Categoria_BLL().ObtenerPorEmpresa(actor))
                {
                    if (c.Activa) opciones.Add(new Opcion { Valor = c.IdCategoria.ToString(), Texto = c.NombreCategoria });
                }
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("NuevaGrabacion.Categorias", ex);
            }

            return opciones;
        }

        private List<Opcion> ArmarOpciones(string[] valores, string prefijoClave)
        {
            var opciones = new List<Opcion>();

            foreach (string valor in valores)
            {
                string texto = prefijoClave == null ? valor : Traducir(prefijoClave + valor);
                opciones.Add(new Opcion { Valor = valor, Texto = texto });
            }

            return opciones;
        }

        private List<Opcion> ArmarRetrasos()
        {
            var opciones = new List<Opcion>();

            foreach (int segundos in RETRASOS_SEGUNDOS)
            {
                string texto = segundos == 0 ? Traducir("grabacion_rec_sin_retraso") : Traducir("grabacion_rec_retraso_n", segundos);
                opciones.Add(new Opcion { Valor = segundos.ToString(), Texto = texto });
            }

            return opciones;
        }

        protected string Atributos(object elemento)
        {
            var chequeo = (Chequeo)elemento;
            string atributos = chequeo.Obligatorio ? " data-obligatorio" : "";

            foreach (KeyValuePair<string, string> texto in chequeo.Textos)
            {
                atributos += " data-" + texto.Key + "=\"" + HttpUtility.HtmlAttributeEncode(texto.Value) + "\"";
            }

            return atributos;
        }

        private List<Chequeo> ArmarChequeos()
        {
            string verificando = Traducir("grabacion_prep_verificando");
            string calibracionPantalla = Traducir("grabacion_prep_cal_pantalla_detalle");
            string calibracionVista = Traducir("grabacion_prep_cal_vista_detalle");
            string confirmado = Traducir("grabacion_prep_confirmado");

            return new List<Chequeo>
            {
                new Chequeo
                {
                    Id = "agente",
                    Nombre = Traducir("grabacion_prep_agente"),
                    Inicial = verificando,
                    Obligatorio = true,
                    ConEnlaceInstalacion = true,
                    Textos =
                    {
                        { "correcto", Traducir("grabacion_prep_agente_ok") },
                        { "problema", Traducir("grabacion_prep_agente_falta") }
                    }
                },
                new Chequeo
                {
                    Id = "tobii",
                    Nombre = Traducir("grabacion_prep_tobii"),
                    Inicial = verificando,
                    Obligatorio = true,
                    ConBotonTobii = true,
                    Textos =
                    {
                        { "correcto", Traducir("grabacion_prep_tobii_ok") },
                        { "simulado", Traducir("grabacion_prep_tobii_simulado") },
                        { "problema", Traducir("grabacion_prep_tobii_falta") },
                        { "sinagente", Traducir("grabacion_prep_tobii_sin_agente") }
                    }
                },
                new Chequeo
                {
                    Id = "pantalla",
                    Nombre = Traducir("grabacion_prep_pantalla"),
                    Inicial = verificando,
                    Obligatorio = true,
                    Textos =
                    {
                        { "correcto", Traducir("grabacion_prep_pantalla_ok") },
                        { "problema", Traducir("grabacion_prep_pantalla_falta") }
                    }
                },
                new Chequeo
                {
                    Id = "calPantalla",
                    Nombre = Traducir("grabacion_prep_cal_pantalla"),
                    Inicial = calibracionPantalla,
                    Clase = "pendiente",
                    ConBotonTobii = true,
                    ConCasilla = true,
                    Textos =
                    {
                        { "pendiente", calibracionPantalla },
                        { "correcto", confirmado }
                    }
                },
                new Chequeo
                {
                    Id = "calVista",
                    Nombre = Traducir("grabacion_prep_cal_vista"),
                    Inicial = calibracionVista,
                    Clase = "pendiente",
                    ConBotonTobii = true,
                    ConCasilla = true,
                    Textos =
                    {
                        { "pendiente", calibracionVista },
                        { "correcto", confirmado }
                    }
                }
            };
        }

        #region Guardado en el servidor (WebMethods)

        private const string CARPETA_GRABACIONES = "~/App_Data/Grabaciones";
        private const int HORAS_VIDA_TEMPORAL = 24;

        public class DatosSesion
        {
            public int idCategoria { get; set; }
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
        public static RespuestaGuardado IniciarSubida()
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
        public static RespuestaGuardado SubirParte(string token, long desde, string base64)
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
        public static RespuestaGuardado FinalizarSubida(string token, DatosSesion datos, string mirada)
        {
            return Ejecutar(actor =>
            {
                if (datos == null) throw new ExcepcionTraducible_TLL("comun_solicitud_invalida");

                string ruta = RutaTemporal(actor, token);

                var sesion = new SesionGrabada_BE
                {
                    IdCategoria = datos.idCategoria,
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

        #endregion
    }
}
