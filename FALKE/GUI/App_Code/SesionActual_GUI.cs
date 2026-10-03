using System;
using System.Web;
using SECURITY;
using TE;
using TLL;

namespace GUI
{
    public static class SesionActual_GUI
    {
        private const string K_ID = "UsuarioId";
        private const string K_EMAIL = "UsuarioEmail";
        private const string K_NOMBRE = "UsuarioNombre";
        private const string K_ROL = "UsuarioRol";
        private const string K_EMPRESA = "UsuarioIdEmpresa";
        private const string K_EMERGENCIA = "UsuarioEmergencia";
        private const string K_PERMISOS = "UsuarioPermisos";

        private const string COOKIE_RECORDARME = "FALKE_RECORDARME";
        private const string ITEM_RESTAURA = "FALKE_RECORDARME_RESUELTO";
        private const int DIAS_RECORDARME = 30;

        private static HttpContext Ctx
        {
            get { return HttpContext.Current; }
        }

        public static bool HayUsuario
        {
            get { return Ctx.Session[K_EMAIL] != null; }
        }

        public static int IdUsuario
        {
            get
            {
                object valor = Ctx.Session[K_ID];
                return valor == null ? 0 : (int)valor;
            }
        }

        public static string Email
        {
            get { return (string)Ctx.Session[K_EMAIL]; }
        }

        public static string Nombre
        {
            get { return (string)Ctx.Session[K_NOMBRE]; }
        }

        public static string Rol
        {
            get { return (string)Ctx.Session[K_ROL] ?? string.Empty; }
        }

        public static int IdEmpresa
        {
            get
            {
                object valor = Ctx.Session[K_EMPRESA];
                return valor == null ? 0 : (int)valor;
            }
        }

        public static bool EsEmergencia
        {
            get
            {
                object valor = Ctx.Session[K_EMERGENCIA];
                return valor != null && (bool)valor;
            }
        }

        public static PermisoAbstracto_TE PermisoActual
        {
            get { return Ctx.Session[K_PERMISOS] as PermisoAbstracto_TE; }
        }

        public static bool Puede(string patente)
        {
            RestaurarDesdeCookie();

            if (!HayUsuario) return false;

            if (EsEmergencia) return true;

            return Permiso_TLL.ComprobarPermiso(patente, PermisoActual);
        }

        public static void Iniciar(int idUsuario, string email, string nombre, string rol, int idEmpresa, bool esEmergencia, PermisoAbstracto_TE permiso)
        {
            Ctx.Session[K_ID] = idUsuario;
            Ctx.Session[K_EMAIL] = email;
            Ctx.Session[K_NOMBRE] = nombre;
            Ctx.Session[K_ROL] = rol;
            Ctx.Session[K_EMPRESA] = idEmpresa;
            Ctx.Session[K_EMERGENCIA] = esEmergencia;
            Ctx.Session[K_PERMISOS] = permiso;
        }

        public static void Cerrar()
        {
            OlvidarEsteEquipo();
            Ctx.Session.Clear();
            Ctx.Session.Abandon();
        }

        public static bool Exigir()
        {
            RestaurarDesdeCookie();

            if (HayUsuario) return true;

            Redirigir("Ingresar.aspx");
            return false;
        }

        public static bool ExigirPermiso(string patente)
        {
            RestaurarDesdeCookie();

            if (!HayUsuario)
            {
                Redirigir("Ingresar.aspx");
                return false;
            }

            if (!Puede(patente))
            {
                Redirigir("SinPermiso.aspx");
                return false;
            }

            return true;
        }

        public static bool RedirigirSiAutenticado()
        {
            RestaurarDesdeCookie();

            if (!HayUsuario) return false;

            Redirigir("Panel.aspx");
            return true;
        }

        public static void RecordarEnEsteEquipo(int idUsuario)
        {
            if (idUsuario <= 0 || Ctx == null) return;

            DateTime vence = DateTime.Now.AddDays(DIAS_RECORDARME);
            string carga = idUsuario.ToString() + "|" + vence.Ticks.ToString();

            string valor;
            try
            {
                valor = Cifrador_SECURITY.CifradorSingleton.EncriptadoReversible(carga);
            }
            catch
            {
                return;
            }

            HttpCookie cookie = new HttpCookie(COOKIE_RECORDARME, valor)
            {
                HttpOnly = true,
                Secure = Ctx.Request.IsSecureConnection,
                Expires = vence,
                Path = "/"
            };

            Ctx.Response.Cookies.Add(cookie);
        }

        public static void OlvidarEsteEquipo()
        {
            if (Ctx == null) return;

            HttpCookie cookie = new HttpCookie(COOKIE_RECORDARME, string.Empty)
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(-1),
                Path = "/"
            };

            Ctx.Response.Cookies.Add(cookie);
        }

        public static void RestaurarDesdeCookie()
        {
            if (Ctx == null) return;
            if (Ctx.Items[ITEM_RESTAURA] != null) return;
            Ctx.Items[ITEM_RESTAURA] = true;

            if (HayUsuario) return;

            HttpCookie cookie = Ctx.Request.Cookies[COOKIE_RECORDARME];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value)) return;

            try
            {
                string carga = Cifrador_SECURITY.CifradorSingleton.DesencriptadoReversible(cookie.Value);
                string[] partes = carga.Split('|');

                int idUsuario;
                long ticks;

                if (partes.Length != 2 ||
                    !int.TryParse(partes[0], out idUsuario) ||
                    !long.TryParse(partes[1], out ticks) ||
                    new DateTime(ticks) < DateTime.Now)
                {
                    OlvidarEsteEquipo();
                    return;
                }

                Usuario_TE usuario = new Usuario_TLL().ObtenerPorId(idUsuario);

                if (usuario == null || usuario.EsCuentaEmergencia || usuario.Estado != EstadoUsuario.Activo)
                {
                    OlvidarEsteEquipo();
                    return;
                }

                string nombre = (usuario.NombreUsuario + " " + usuario.ApellidoUsuario).Trim();
                string rol = usuario.Rol != null ? usuario.Rol.Nombre : string.Empty;

                Iniciar(usuario.IdUsuario, usuario.EmailUsuario, nombre, rol, usuario.IdEmpresa, usuario.EsCuentaEmergencia, usuario.Rol);
            }
            catch
            {
                OlvidarEsteEquipo();
            }
        }

        private static void Redirigir(string url)
        {
            Ctx.Response.Redirect(url, false);
            Ctx.ApplicationInstance.CompleteRequest();
        }
    }
}
