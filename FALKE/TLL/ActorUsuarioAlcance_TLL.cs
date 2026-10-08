using System;
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

        public static void Exigir(this ActorUsuario_TE actor, string patente)
        {
            if (actor != null && actor.Puede(patente)) return;

            new BitacoraGestor_TLL().Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", "Acción rechazada por falta de permiso (" + patente + ")", CriticidadBitacora.Media);
            throw new UnauthorizedAccessException("No tenés permiso para realizar esta acción.");
        }
    }
}
