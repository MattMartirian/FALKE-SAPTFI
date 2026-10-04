using System.Collections.Generic;

namespace TLL
{
    // Las patentes las define el desarrollador: cada una está atada a una función de la página.
    // Desde la pantalla de roles solo se combinan y se les edita la descripción (PermisoTable.descripcion_permiso); el nombre es interno.
    public static class Patentes_TLL
    {
        public const string REGISTRAR_EMPRESA = "REGISTRAR_EMPRESA";
        public const string MODIFICAR_EMPRESA = "MODIFICAR_EMPRESA";
        public const string CAMBIAR_ESTADO_EMPRESA = "CAMBIAR_ESTADO_EMPRESA";
        public const string VER_DATOS_EMPRESA = "VER_DATOS_EMPRESA";
        public const string REGISTRAR_USUARIO = "REGISTRAR_USUARIO";
        public const string CREAR_USUARIO_OTRA_EMPRESA = "CREAR_USUARIO_OTRA_EMPRESA";
        public const string VER_USUARIOS_TODAS_EMPRESAS = "VER_USUARIOS_TODAS_EMPRESAS";
        public const string CAMBIAR_ESTADO_USUARIO = "CAMBIAR_ESTADO_USUARIO";
        public const string CAMBIAR_ROL_USUARIO = "CAMBIAR_ROL_USUARIO";
        public const string RECALCULAR_INTEGRIDAD = "RECALCULAR_INTEGRIDAD";
        public const string GESTIONAR_ROLES = "GESTIONAR_ROLES";
        public const string OPERAR_ANALISIS = "OPERAR_ANALISIS";
        public const string VER_USUARIOS = "VER_USUARIOS";
        public const string VER_BITACORA = "VER_BITACORA";
        public const string GESTIONAR_DISPOSITIVOS = "GESTIONAR_DISPOSITIVOS";
        public const string GESTIONAR_RESPALDOS = "GESTIONAR_RESPALDOS";
        public const string CAMBIAR_DESCRIPCION_PERMISO = "CAMBIAR_DESCRIPCION_PERMISO";
        public const string MODIFICAR_USUARIO = "MODIFICAR_USUARIO";
        public const string CAMBIAR_EMAIL_EMPRESA_USUARIO = "CAMBIAR_EMAIL_EMPRESA_USUARIO";
        public const string MODIFICAR_CONTACTO_EMPRESA = "MODIFICAR_CONTACTO_EMPRESA";

        // Permisos reservados a Pattern Blue: un rol que los incluya solo se puede asignar a usuarios de la empresa proveedora,
        // porque dan alcance sobre todas las empresas cliente o sobre el sistema en sí.
        private static readonly HashSet<string> deProveedor = new HashSet<string>
        {
            VER_USUARIOS_TODAS_EMPRESAS, CREAR_USUARIO_OTRA_EMPRESA, REGISTRAR_EMPRESA, MODIFICAR_EMPRESA, CAMBIAR_ESTADO_EMPRESA,
            GESTIONAR_ROLES, RECALCULAR_INTEGRIDAD, GESTIONAR_DISPOSITIVOS, GESTIONAR_RESPALDOS,
            CAMBIAR_DESCRIPCION_PERMISO, CAMBIAR_EMAIL_EMPRESA_USUARIO
        };

        public static bool EsDeProveedor(string patente)
        {
            return deProveedor.Contains(patente);
        }
    }
}
