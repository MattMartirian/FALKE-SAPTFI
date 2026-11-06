using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.SessionState;
using BE;
using BLL;
using SERVICES;
using TE;

namespace GUI
{
    public class GrabacionArchivo_GUI : IHttpHandler, IReadOnlySessionState
    {
        public const string CLAVE_SESION = "VisSesionesTokens";
        public const string CARPETA = "~/App_Data/Grabaciones";

        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext ctx)
        {
            ActorUsuario_TE actor = SesionActual_GUI.ObtenerActor();
            if (actor == null) { Terminar(ctx, 401); return; }

            var tokens = ctx.Session[CLAVE_SESION] as Dictionary<string, int>;
            string token = ctx.Request.QueryString["t"];
            string tipo = ctx.Request.QueryString["k"];
            int idSesion;

            if (tokens == null || token == null || !tokens.TryGetValue(token, out idSesion) || (tipo != "video" && tipo != "mirada"))
            {
                Terminar(ctx, 404);
                return;
            }

            try
            {
                new SesionGrabada_BLL().ObtenerPropia(actor, idSesion);
            }
            catch (Exception)
            {
                Terminar(ctx, 404);
                return;
            }

            string archivo = tipo == "video" ? SesionGrabada_BLL.NombreVideo(idSesion) : SesionGrabada_BLL.NombreMirada(idSesion);
            string ruta = Path.Combine(ctx.Server.MapPath(CARPETA), archivo);

            if (!File.Exists(ruta)) { Terminar(ctx, 404); return; }

            try
            {
                Entregar(ctx, ruta, tipo == "video" ? "video/webm" : "application/json; charset=utf-8");
            }
            catch (HttpException)
            {
                // el navegador corto la descarga (saltar en el video, cerrar la pagina): es normal
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Grabacion.Archivo", ex);
                if (!ctx.Response.IsClientConnected) return;
                try { if (!ctx.Response.BufferOutput || ctx.Response.StatusCode == 200) Terminar(ctx, 500); } catch (Exception) { }
            }
        }

        private static void Entregar(HttpContext ctx, string ruta, string tipoContenido)
        {
            long total = new FileInfo(ruta).Length;
            long ini = 0;
            long fin = total - 1;
            bool parcial = false;

            // El <video> pide pedazos ("Range: bytes=a-b") para poder saltar a cualquier punto sin bajar todo.
            string rango = ctx.Request.Headers["Range"];

            if (!string.IsNullOrEmpty(rango) && rango.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string[] p = rango.Substring(6).Split(',')[0].Split('-');

                    if (p[0].Length == 0)
                    {
                        ini = Math.Max(0, total - long.Parse(p[1]));
                    }
                    else
                    {
                        ini = long.Parse(p[0]);
                        if (p.Length > 1 && p[1].Length > 0) fin = Math.Min(long.Parse(p[1]), total - 1);
                    }
                }
                catch (Exception)
                {
                    Terminar(ctx, 416);
                    return;
                }

                if (ini > fin || ini >= total)
                {
                    ctx.Response.AddHeader("Content-Range", "bytes */" + total);
                    Terminar(ctx, 416);
                    return;
                }

                parcial = true;
            }

            ctx.Response.StatusCode = parcial ? 206 : 200;
            ctx.Response.ContentType = tipoContenido;
            ctx.Response.AddHeader("Accept-Ranges", "bytes");
            ctx.Response.AddHeader("Content-Length", (fin - ini + 1).ToString());
            if (parcial) ctx.Response.AddHeader("Content-Range", "bytes " + ini + "-" + fin + "/" + total);
            ctx.Response.AddHeader("Content-Disposition", "inline");
            ctx.Response.AddHeader("X-Content-Type-Options", "nosniff");
            ctx.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            ctx.Response.Cache.SetNoStore();

            ctx.Response.TransmitFile(ruta, ini, fin - ini + 1);
        }

        private static void Terminar(HttpContext ctx, int estado)
        {
            ctx.Response.StatusCode = estado;
            ctx.Response.TrySkipIisCustomErrors = true;
        }
    }
}
