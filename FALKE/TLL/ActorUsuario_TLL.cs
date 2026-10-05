using TE;

namespace TLL
{
    public class ActorUsuario_TLL
    {
        public int IdUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public bool EsEmergencia { get; set; }
        public PermisoAbstracto_TE Permiso { get; set; }

        public bool Puede(string patente)
        {
            if (EsEmergencia) return true;

            return Permiso_TLL.ComprobarPermiso(patente, Permiso);
        }

        // La bitácora de todas las empresas (la del Gestor y la del Webmaster). Los demás ven solo la de su empresa.
        public bool VeBitacoraCompleta
        {
            get { return Puede(Patentes_TLL.VER_BITACORA_COMPLETA); }
        }

        public bool VeTodasLasEmpresas
        {
            get { return Puede(Patentes_TLL.VER_USUARIOS_TODAS_EMPRESAS); }
        }
    }
}
