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

        // aca se traducen las cosas en una pagina X
        protected override void Render(HtmlTextWriter writer)
        {
            // la pagina se dibuja primero en un texto en memoria para poder reemplazar los textos
            StringWriter textoPagina = new StringWriter();
            HtmlTextWriter escritorTemporal = new HtmlTextWriter(textoPagina);

            base.Render(escritorTemporal);
            escritorTemporal.Flush();

            string html = textoPagina.ToString();

            // por cada [[clave]] o [[js:clave]] se llama a reemplazar
            html = Marcador.Replace(html, Reemplazar);

            writer.Write(html);
        }

        private string Reemplazar(Match marcador)
        {
            // grupo 2: la clave a traducir          grupo 1: el prefijo "js:" por [[js:clave]]
            string clave = marcador.Groups[2].Value;
            bool esParaJavaScript = marcador.Groups[1].Success;

            string texto = Traductor_TLL.Traducir(clave, codigo);

            if (esParaJavaScript)
            {
                // para textos dentro de un <script> ignora comillas, barras y saltos de linea para que el texto no rompa el js
                texto = HttpUtility.JavaScriptStringEncode(texto);

                // elimina las comillas simples '
                texto = texto.Replace("\\'", "\\u0027");
                return texto;
            }

            // convierte <, >, & y comillas en entidades para que el texto no se interprete como codigo
            return HttpUtility.HtmlEncode(texto);
        }
    }
}
