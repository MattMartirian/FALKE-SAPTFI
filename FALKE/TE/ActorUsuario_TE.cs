namespace TE
{
    // Quién está haciendo la acción: el usuario con sesión iniciada (o la cuenta de emergencia) y sus permisos.
    // Se arma en cada pedido a partir de la sesión y se pasa a las capas de negocio, que deciden con él sin conocer la sesión ni la pantalla.
    public class ActorUsuario_TE
    {
        public int IdUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public bool EsEmergencia { get; set; }

        // El rol del usuario: la raíz de su árbol de permisos (Composite).
        public PermisoAbstracto_TE Permiso { get; set; }

        // La cuenta de emergencia puede todo; el resto, solo lo que su árbol de permisos contiene.
        public bool Puede(string patente)
        {
            if (EsEmergencia) return true;

            return !string.IsNullOrEmpty(patente) && Permiso != null && Permiso.Contiene(patente);
        }
    }
}
