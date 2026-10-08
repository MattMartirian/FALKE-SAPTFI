using System;
using System.Collections.Specialized;
using System.Globalization;
using TE;

namespace GUI
{
    public static class FiltroBitacora_GUI
    {
        public static FiltroBitacora_TE Parsear(string desde, string hasta, string horaDesde, string horaHasta,
            string modulo, string accion, string usuario, string empresa, string criticidad)
        {
            var filtro = new FiltroBitacora_TE
            {
                Desde = Fecha(desde),
                Hasta = Fecha(hasta),
                HoraDesde = Hora(horaDesde),
                HoraHasta = Hora(horaHasta),
                Modulo = Texto(modulo),
                Accion = Texto(accion),
                Usuario = Texto(usuario)
            };

            int idEmpresa;
            if (int.TryParse(empresa, out idEmpresa) && idEmpresa > 0) filtro.IdEmpresa = idEmpresa;

            int nivel;
            if (int.TryParse(criticidad, out nivel) && Enum.IsDefined(typeof(CriticidadBitacora), nivel))
                filtro.Criticidad = (CriticidadBitacora)nivel;

            return filtro;
        }

        public static FiltroBitacora_TE Leer(NameValueCollection q)
        {
            return Parsear(q["d"], q["h"], q["hd"], q["hh"], q["m"], q["a"], q["u"], q["e"], q["c"]);
        }

        private static string Texto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }

        private static DateTime? Fecha(string valor)
        {
            DateTime fecha;

            return DateTime.TryParseExact((valor ?? string.Empty).Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha)
                ? fecha
                : (DateTime?)null;
        }

        private static TimeSpan? Hora(string valor)
        {
            TimeSpan hora;
            string v = (valor ?? string.Empty).Trim();

            if (!TimeSpan.TryParse(v, CultureInfo.InvariantCulture, out hora)) return null;

            return hora >= TimeSpan.Zero && hora < TimeSpan.FromHours(24) ? hora : (TimeSpan?)null;
        }
    }
}
