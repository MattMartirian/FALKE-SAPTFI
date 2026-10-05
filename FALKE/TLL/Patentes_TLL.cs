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
        public const string VER_BITACORA_COMPLETA = "VER_BITACORA_COMPLETA";
        public const string VER_DISPOSITIVOS = "VER_DISPOSITIVOS";
        public const string ALTA_DISPOSITIVO = "ALTA_DISPOSITIVO";
        public const string ASIGNAR_DISPOSITIVO = "ASIGNAR_DISPOSITIVO";
        public const string GESTIONAR_MODELOS_DISPOSITIVO = "GESTIONAR_MODELOS_DISPOSITIVO";
        public const string VER_RESPALDOS = "VER_RESPALDOS";
        public const string HACER_RESPALDO = "HACER_RESPALDO";
        public const string RESTAURAR_RESPALDO = "RESTAURAR_RESPALDO";
        public const string CAMBIAR_DESCRIPCION_PERMISO = "CAMBIAR_DESCRIPCION_PERMISO";
        public const string MODIFICAR_USUARIO = "MODIFICAR_USUARIO";
        public const string CAMBIAR_EMAIL_EMPRESA_USUARIO = "CAMBIAR_EMAIL_EMPRESA_USUARIO";
        public const string MODIFICAR_CONTACTO_EMPRESA = "MODIFICAR_CONTACTO_EMPRESA";
        public const string GESTIONAR_CATEGORIAS = "GESTIONAR_CATEGORIAS";

        // Permisos reservados a Pattern Blue: un rol que los incluya solo se puede asignar a usuarios de la empresa proveedora,
        // porque dan alcance sobre todas las empresas cliente o sobre el sistema en sí.
        private static readonly HashSet<string> deProveedor = new HashSet<string>
        {
            VER_USUARIOS_TODAS_EMPRESAS, CREAR_USUARIO_OTRA_EMPRESA, REGISTRAR_EMPRESA, MODIFICAR_EMPRESA, CAMBIAR_ESTADO_EMPRESA,
            GESTIONAR_ROLES, RECALCULAR_INTEGRIDAD, VER_DISPOSITIVOS, ALTA_DISPOSITIVO, ASIGNAR_DISPOSITIVO, GESTIONAR_MODELOS_DISPOSITIVO,
            VER_RESPALDOS, HACER_RESPALDO, RESTAURAR_RESPALDO, VER_BITACORA_COMPLETA, CAMBIAR_DESCRIPCION_PERMISO, CAMBIAR_EMAIL_EMPRESA_USUARIO
        };

        // Permisos de infraestructura: son del Webmaster (integridad de los datos y copias de seguridad). El Gestor, que se ocupa de los
        // clientes, no los tiene; por eso no cuentan al comparar quién tiene "más permisos" que quién.
        private static readonly HashSet<string> deInfraestructura = new HashSet<string>
        {
            RECALCULAR_INTEGRIDAD, VER_RESPALDOS, HACER_RESPALDO, RESTAURAR_RESPALDO
        };

        public static bool EsDeInfraestructura(string patente)
        {
            return deInfraestructura.Contains(patente);
        }

        public static bool EsDeProveedor(string patente)
        {
            return deProveedor.Contains(patente);
        }
    }
}
