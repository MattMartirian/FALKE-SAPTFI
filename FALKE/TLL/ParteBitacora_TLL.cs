namespace TLL
{
    public sealed class ParteBitacora_TLL
    {
        public string Valor { get; private set; }

        public ParteBitacora_TLL(string valor)
        {
            Valor = valor ?? string.Empty;
        }

        public override string ToString()
        {
            return Valor;
        }

        public static implicit operator string(ParteBitacora_TLL parte)
        {
            return parte == null ? null : parte.Valor;
        }
    }
}
