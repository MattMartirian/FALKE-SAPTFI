using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using TE;

namespace GUI
{
    // Convierte lo que se escribe en los filtros de la bitácora al filtro de la capa de negocio y a la cadena de consulta
    // que usa la vista imprimible. Solo traduce formatos: las reglas del filtro viven en BitacoraGestor_TLL.
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

        public static string AQuery(FiltroBitacora_TE f)
        {
            var sb = new StringBuilder();

            Agregar(sb, "d", f.Desde.HasValue ? f.Desde.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null);
            Agregar(sb, "h", f.Hasta.HasValue ? f.Hasta.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null);
            Agregar(sb, "hd", f.HoraDesde.HasValue ? f.HoraDesde.Value.ToString(@"hh\:mm", CultureInfo.InvariantCulture) : null);
            Agregar(sb, "hh", f.HoraHasta.HasValue ? f.HoraHasta.Value.ToString(@"hh\:mm", CultureInfo.InvariantCulture) : null);
            Agregar(sb, "m", f.Modulo);
            Agregar(sb, "a", f.Accion);
            Agregar(sb, "u", f.Usuario);
            Agregar(sb, "e", f.IdEmpresa.HasValue ? f.IdEmpresa.Value.ToString(CultureInfo.InvariantCulture) : null);
            Agregar(sb, "c", f.Criticidad.HasValue ? ((int)f.Criticidad.Value).ToString(CultureInfo.InvariantCulture) : null);

            return sb.ToString();
        }

        private static void Agregar(StringBuilder sb, string clave, string valor)
        {
            if (string.IsNullOrEmpty(valor)) return;

            if (sb.Length > 0) sb.Append('&');
            sb.Append(clave).Append('=').Append(Uri.EscapeDataString(valor));
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
