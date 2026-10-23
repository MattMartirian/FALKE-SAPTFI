using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TLL;

namespace GUI
{
    public abstract class PaginaBase_GUI : Page, IObservadorIdioma_TLL
    {
        private static readonly Regex Marcador = new Regex(@"\[\[(js:)?([A-Za-z0-9_.]+)\]\]", RegexOptions.Compiled);

        private SujetoIdioma_TLL sujeto;
        private string codigo = Traductor_TLL.CODIGO_BASE;
        private bool cargada;

        private const string K_AVISOS = "AvisosTraducibles";

        protected string CodigoIdioma
        {
            get { return codigo; }
        }

        public void ActualizarIdioma(string codigoIdioma)
        {
            codigo = codigoIdioma;

            if (!cargada) return;

            RetraducirAvisos();
            AlCambiarIdioma();
        }

        protected virtual void AlCambiarIdioma()
        {
            // template method aca cada pagina decide que hacer
        }

        protected static void ConservandoSeleccion(Action reconstruir, params DropDownList[] listas)
        {
            var seleccion = listas.Select(l => l.SelectedValue).ToList();

            reconstruir();

            for (int i = 0; i < listas.Length; i++)
            {
                ListItem item = listas[i].Items.FindByValue(seleccion[i]);

                if (item == null) continue;

                listas[i].ClearSelection();
                item.Selected = true;
            }
        }

        protected string Traducir(string clave, params object[] parametros)
        {
            string texto = Traductor_TLL.Traducir(clave, codigo, parametros);

            OrigenTexto_GUI.Registrar(texto, clave, parametros);

            return texto;
        }

        protected virtual string MensajeDe(Exception ex)
        {
            string texto = Mensajes_TLL.Interpretar(ex, codigo);
            string clave;
            object[] parametros;

            if (Mensajes_TLL.Descomponer(ex, out clave, out parametros)) OrigenTexto_GUI.Registrar(texto, clave, parametros);

            return texto;
        }

        protected void PublicarAviso(Panel panel, Literal literal, string clases, string texto)
        {
            if (clases != null) panel.CssClass = clases;

            literal.Text = Server.HtmlEncode(texto);
            panel.Visible = true;

            var avisos = ViewState[K_AVISOS] as Hashtable ?? new Hashtable();
            string clave;
            object[] parametros;

            if (OrigenTexto_GUI.Buscar(texto, out clave, out parametros))
                avisos[panel.UniqueID] = new object[] { literal.UniqueID, texto, clave, parametros };
            else
                avisos.Remove(panel.UniqueID);

            ViewState[K_AVISOS] = avisos;
        }

        protected virtual void RetraducirAvisos()
        {
            var avisos = ViewState[K_AVISOS] as Hashtable;

            if (avisos == null) return;

            foreach (string idPanel in avisos.Keys.Cast<string>().ToList())
            {
                var entrada = (object[])avisos[idPanel];
                var panel = FindControl(idPanel) as Panel;
                var literal = FindControl((string)entrada[0]) as Literal;

                if (panel == null || literal == null || !panel.Visible || literal.Text != Server.HtmlEncode((string)entrada[1])) continue;

                string texto = Traductor_TLL.Traducir((string)entrada[2], codigo, (object[])entrada[3]);

                literal.Text = Server.HtmlEncode(texto);
                entrada[1] = texto;
            }

            ViewState[K_AVISOS] = avisos;
        }

        protected override void OnInit(EventArgs e)
        {
            sujeto = IdiomaActual_GUI.Sujeto;
            codigo = sujeto.CodigoIdioma;
            sujeto.Registrar(this);

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cargada = true;
        }

        protected override void OnUnload(EventArgs e)
        {
            if (sujeto != null) sujeto.Quitar(this);

            base.OnUnload(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var html = new StringWriter();

            using (var intermedio = new HtmlTextWriter(html))
            {
                base.Render(intermedio);
            }

            writer.Write(Marcador.Replace(html.ToString(), Reemplazar));
        }

        private string Reemplazar(Match marcador)
        {
            string texto = Traductor_TLL.Traducir(marcador.Groups[2].Value, codigo);

            return marcador.Groups[1].Success ? HttpUtility.JavaScriptStringEncode(texto).Replace("\\'", "\\u0027") : HttpUtility.HtmlEncode(texto);
        }
    }
}
