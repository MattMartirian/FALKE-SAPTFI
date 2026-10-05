using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ORM;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class Integridad : System.Web.UI.Page
    {
        private const string PATENTE_RECALCULO = "RECALCULAR_INTEGRIDAD";

        private static readonly NumberFormatInfo FormatoMiles = new NumberFormatInfo
        {
            NumberGroupSeparator = ".",
            NumberGroupSizes = new[] { 3 }
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(PATENTE_RECALCULO)) return;

            if (!IsPostBack) CargarEstado();
        }

        protected void btnRecalcular_Click(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.Puede(PATENTE_RECALCULO))
            {
                MostrarAviso("aviso-peligro", "Tu cuenta no tiene permiso para recalcular los dígitos verificadores.");
                return;
            }

            try
            {
                var gestor = new GestorIntegridad_SERVICE();
                string alcance = ddlAlcance.SelectedValue;
                int tablasProcesadas = 0;
                int registrosProcesados = 0;

                if (alcance == "todas")
                {
                    var resultado = gestor.RecalcularTodasLasTablas();
                    tablasProcesadas = resultado.TablasProcesadas;
                    registrosProcesados = resultado.RegistrosProcesados;
                }
                else
                {
                    var conProblema = new List<TablasBD>();
                    foreach (var inc in gestor.VerificarIntegridadTodasLasTablas())
                        if (!conProblema.Contains(inc.Tabla)) conProblema.Add(inc.Tabla);

                    foreach (var tabla in conProblema)
                    {
                        var detalle = gestor.RecalcularTabla(tabla);
                        tablasProcesadas++;
                        registrosProcesados += detalle.Registros;
                    }
                }

                var restantes = gestor.VerificarIntegridadTodasLasTablas();
                string motivo = txtMotivo.Text.Trim();

                new BitacoraGestor_TLL().Registrar(SesionActual_GUI.IdUsuario, "Integridad",
                    "Recálculo de dígitos verificadores (" + alcance + "): " + tablasProcesadas + " tabla(s), " +
                    registrosProcesados + " registro(s)." + (motivo.Length > 0 ? " Motivo: " + motivo : string.Empty),
                    CriticidadBitacora.Alta);

                if (restantes.Count == 0)
                    MostrarAviso("aviso-exito", "Recálculo completo: " + tablasProcesadas + " tabla(s), " +
                        registrosProcesados + " registro(s). La verificación posterior no encontró inconsistencias.");
                else
                    MostrarAviso("aviso-alerta", "Recálculo completo: " + tablasProcesadas + " tabla(s), " +
                        registrosProcesados + " registro(s). La verificación posterior todavía encuentra " +
                        restantes.Count + " inconsistencia(s).");

                txtMotivo.Text = string.Empty;
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Integridad/Recalcular", ex);
                MostrarAviso("aviso-peligro", "Ocurrió un error al recalcular la integridad.");
            }

            CargarEstado();
        }

        private void CargarEstado()
        {
            try
            {
                var inconsistencias = new GestorIntegridad_SERVICE().VerificarIntegridadTodasLasTablas();
                var integridadRepo = new Integridad_ORM();

                var porTabla = new Dictionary<TablasBD, List<InconsistenciaIntegridad_SERVICE>>();
                foreach (var inc in inconsistencias)
                {
                    List<InconsistenciaIntegridad_SERVICE> lista;
                    if (!porTabla.TryGetValue(inc.Tabla, out lista))
                    {
                        lista = new List<InconsistenciaIntegridad_SERVICE>();
                        porTabla[inc.Tabla] = lista;
                    }
                    lista.Add(inc);
                }

                var filas = new List<FilaIntegridad>();
                var json = new StringBuilder("{");
                bool primeraTabla = true;

                int total = 0;
                int conProblemas = 0;
                int registrosAfectados = 0;

                foreach (TablasBD tabla in (TablasBD[])Enum.GetValues(typeof(TablasBD)))
                {
                    total++;

                    List<InconsistenciaIntegridad_SERVICE> lista;
                    porTabla.TryGetValue(tabla, out lista);
                    bool inconsistente = lista != null && lista.Count > 0;
                    if (inconsistente) conProblemas++;

                    string nombre = Prettify(tabla.ToString());

                    var registro = integridadRepo.LeerRegistroIntegridad(tabla);
                    string registros = registro.HasValue ? registro.Value.Item2.ToString("#,0", FormatoMiles) : "—";

                    // La columna Detalle solo dice si la tabla está bien o no; qué registros son los que no cierran se ve en «Ver detalle».
                    string detalle = inconsistente
                        ? "La tabla se encuentra corrupta"
                        : "El DVV coincide y los DVH de todos los registros son válidos.";

                    filas.Add(new FilaIntegridad
                    {
                        Nombre = nombre,
                        Registros = registros,
                        Inconsistente = inconsistente,
                        Detalle = detalle
                    });

                    if (!inconsistente) continue;

                    var alterados = lista.FindAll(x => x.Tipo == TipoInconsistencia.RegistroAlterado);
                    registrosAfectados += alterados.Count;

                    if (!primeraTabla) json.Append(',');
                    primeraTabla = false;

                    AppendJsonString(json, nombre);
                    json.Append(":{\"resumen\":");
                    AppendJsonString(json, detalle);
                    json.Append(",\"motivos\":[");

                    // Diferencias que no son de un registro puntual (registros agregados o eliminados, firma global).
                    var generales = lista.FindAll(x => x.Tipo != TipoInconsistencia.RegistroAlterado);
                    for (int k = 0; k < generales.Count; k++)
                    {
                        if (k > 0) json.Append(',');
                        AppendJsonString(json, generales[k].Detalle);
                    }

                    json.Append("],\"registros\":[");

                    for (int k = 0; k < alterados.Count; k++)
                    {
                        var a = alterados[k];
                        if (k > 0) json.Append(',');
                        json.Append("{\"n\":").Append(a.NumeroRegistro.HasValue
                            ? a.NumeroRegistro.Value.ToString(CultureInfo.InvariantCulture)
                            : "null");
                        json.Append(",\"clave\":");
                        AppendJsonString(json, a.ClaveRegistro);
                        json.Append(",\"guardado\":");
                        AppendJsonString(json, a.DvhAlmacenado);
                        json.Append(",\"calculado\":");
                        AppendJsonString(json, a.DvhRecalculado);
                        json.Append('}');
                    }

                    json.Append("]}");
                }

                json.Append('}');

                rptTablas.DataSource = filas;
                rptTablas.DataBind();
                litDetalleJson.Text = json.ToString();

                litTotalTablas.Text = total.ToString(CultureInfo.InvariantCulture);
                litIntegras.Text = (total - conProblemas).ToString(CultureInfo.InvariantCulture);
                litConProblemas.Text = conProblemas.ToString(CultureInfo.InvariantCulture);
                litRegistrosAfectados.Text = registrosAfectados.ToString(CultureInfo.InvariantCulture);

                bool comprometida = conProblemas > 0;
                pnlAlertaProblema.Visible = comprometida;
                pnlAlertaOk.Visible = !comprometida;
                litResumenAlerta.Text = "Se detectaron inconsistencias en " + conProblemas + " de " + total + " tablas.";

                cardConProblemas.Attributes["class"] = comprometida ? "int-metrica alerta" : "int-metrica";
                cardRegistros.Attributes["class"] = registrosAfectados > 0 ? "int-metrica alerta" : "int-metrica";
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Integridad/CargarEstado", ex);
                pnlAlertaProblema.Visible = false;
                pnlAlertaOk.Visible = false;
                MostrarAviso("aviso-peligro", "No se pudo leer el estado de integridad de la base.");
            }
        }

        private void MostrarAviso(string variante, string texto)
        {
            pnlAviso.CssClass = "aviso mb-24 " + variante;
            litAviso.Text = Server.HtmlEncode(texto);
            pnlAviso.Visible = true;
        }

        private static string Prettify(string nombre)
        {
            var sb = new StringBuilder(nombre.Length + 4);

            for (int i = 0; i < nombre.Length; i++)
            {
                char c = nombre[i];

                if (c == '_')
                {
                    sb.Append(' ');
                    continue;
                }

                if (i > 0 && char.IsUpper(c) && sb.Length > 0 && sb[sb.Length - 1] != ' ') sb.Append(' ');

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static void AppendJsonString(StringBuilder sb, string valor)
        {
            sb.Append('"');

            if (valor != null)
            {
                foreach (char c in valor)
                {
                    switch (c)
                    {
                        case '"': sb.Append("\\\""); break;
                        case '\\': sb.Append("\\\\"); break;
                        case '\n': sb.Append("\\n"); break;
                        case '\r': sb.Append("\\r"); break;
                        case '\t': sb.Append("\\t"); break;
                        case '<': sb.Append("\\u003c"); break;
                        case '>': sb.Append("\\u003e"); break;
                        case '&': sb.Append("\\u0026"); break;
                        default:
                            if (c < ' ') sb.Append("\\u").Append(((int)c).ToString("x4"));
                            else sb.Append(c);
                            break;
                    }
                }
            }

            sb.Append('"');
        }

        public class FilaIntegridad
        {
            public string Nombre { get; set; }
            public string Registros { get; set; }
            public bool Inconsistente { get; set; }
            public string Detalle { get; set; }

            public string ClaseTile
            {
                get { return Inconsistente ? "int-tile mal" : "int-tile"; }
            }

            public string ClaseDetalle
            {
                get { return Inconsistente ? "col-detalle int-detalle mal" : "col-detalle"; }
            }

            public string BadgeHtml
            {
                get
                {
                    return Inconsistente
                        ? "<span class=\"badge badge-peligro\">Inconsistente</span>"
                        : "<span class=\"badge badge-exito\">Íntegra</span>";
                }
            }

            public string AtributoDetalle
            {
                get { return Inconsistente ? string.Empty : "disabled"; }
            }

            public string DetalleHtml
            {
                get { return System.Web.HttpUtility.HtmlEncode(Detalle ?? string.Empty); }
            }
        }
    }
}
