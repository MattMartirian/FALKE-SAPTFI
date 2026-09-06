using System.Web;
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

        private static HttpContext Ctx
        {
            get { return HttpContext.Current; }
        }

        public static bool HayUsuario
        {
            get { return Ctx.Session[K_EMAIL] != null; }
        }

        /// <summary>Id del usuario logueado. -1 para emergencia, 0 si no hay sesion.</summary>
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
            Ctx.Session.Clear();
            Ctx.Session.Abandon();
        }

        public static bool Exigir()
        {
            if (HayUsuario) return true;

            Redirigir("Login.aspx");
            return false;
        }

        public static bool ExigirPermiso(string patente)
        {
            if (!HayUsuario)
            {
                Redirigir("Login.aspx");
                return false;
            }

            if (!Puede(patente))
            {
                Redirigir("MenuPruebas.aspx?err=permiso");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Para el Page_Load del Login: si ya hay sesion activa, redirige al menu y devuelve true.
        /// </summary>
        public static bool RedirigirSiAutenticado()
        {
            if (!HayUsuario) return false;

            Redirigir("MenuPruebas.aspx");
            return true;
        }

        private static void Redirigir(string url)
        {
            Ctx.Response.Redirect(url, false);
            Ctx.ApplicationInstance.CompleteRequest();
        }
    }
}
