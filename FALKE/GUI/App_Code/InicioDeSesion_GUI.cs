using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TE;

namespace GUI
{

    public static class InicioDeSesion_GUI
    {
        private const int SEGUNDOS_DE_VIDA = 30;

        public sealed class Pendiente
        {
            public Usuario_TE Usuario { get; set; }
            public bool Recordarme { get; set; }
            public bool IrAIntegridad { get; set; }
            public string Direccion { get; set; }
            public DateTime Vence { get; set; }
        }

        private static readonly object candado = new object();
        private static readonly Dictionary<string, Pendiente> pendientes = new Dictionary<string, Pendiente>();

        public static string Preparar(Usuario_TE usuario, bool recordarme, bool irAIntegridad, string direccion)
        {
            var bytes = new byte[32];

            using (var azar = RandomNumberGenerator.Create()) azar.GetBytes(bytes);

            string ticket = Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

            lock (candado)
            {
                Purgar();
                pendientes[ticket] = new Pendiente
                {
                    Usuario = usuario,
                    Recordarme = recordarme,
                    IrAIntegridad = irAIntegridad,
                    Direccion = direccion ?? string.Empty,
                    Vence = DateTime.UtcNow.AddSeconds(SEGUNDOS_DE_VIDA)
                };
            }

            return ticket;
        }

        public static Pendiente Consumir(string ticket, string direccion)
        {
            if (string.IsNullOrEmpty(ticket)) return null;

            lock (candado)
            {
                Purgar();

                Pendiente pendiente;
                if (!pendientes.TryGetValue(ticket, out pendiente)) return null;

                pendientes.Remove(ticket);

                if (pendiente.Vence < DateTime.UtcNow) return null;
                if (!string.Equals(pendiente.Direccion, direccion ?? string.Empty, StringComparison.Ordinal)) return null;

                return pendiente;
            }
        }

        private static void Purgar()
        {
            DateTime ahora = DateTime.UtcNow;
            var vencidos = new List<string>();

            foreach (var par in pendientes)
            {
                if (par.Value.Vence < ahora) vencidos.Add(par.Key);
            }

            foreach (string clave in vencidos) pendientes.Remove(clave);
        }
    }
}
