using TE;

namespace TLL
{
    // Atajos de lectura sobre el actor: dos permisos que deciden el alcance de lo que ve. Viven en TLL porque usan las constantes de Patentes_TLL.
    public static class ActorUsuarioAlcance_TLL
    {
        // Ve a todas las empresas cliente (el Gestor); los demás, solo la suya.
        public static bool VeTodasLasEmpresas(this ActorUsuario_TE actor)
        {
            return actor != null && actor.Puede(Patentes_TLL.VER_USUARIOS_TODAS_EMPRESAS);
        }

        // La bitácora de todas las empresas (la del Gestor y la del Webmaster). Los demás ven solo la de su empresa.
        public static bool VeBitacoraCompleta(this ActorUsuario_TE actor)
        {
            return actor != null && actor.Puede(Patentes_TLL.VER_BITACORA_COMPLETA);
        }
    }
}
