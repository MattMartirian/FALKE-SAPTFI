using TE;

namespace TLL
{
    public static class ActorUsuarioAlcance_TLL
    {
        public static bool VeTodasLasEmpresas(this ActorUsuario_TE actor)
        {
            return actor != null && actor.Puede(Patentes_TLL.VER_USUARIOS_TODAS_EMPRESAS);
        }

        public static bool VeBitacoraCompleta(this ActorUsuario_TE actor)
        {
            return actor != null && actor.Puede(Patentes_TLL.VER_BITACORA_COMPLETA);
        }
    }
}
