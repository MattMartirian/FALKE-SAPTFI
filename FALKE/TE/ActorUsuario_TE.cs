namespace TE
{
    public class ActorUsuario_TE
    {
        public int IdUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public bool EsEmergencia { get; set; }

        public PermisoAbstracto_TE Permiso { get; set; }

        public bool Puede(string patente)
        {
            if (EsEmergencia) return true;

            return !string.IsNullOrEmpty(patente) && Permiso != null && Permiso.Contiene(patente);
        }
    }
}
