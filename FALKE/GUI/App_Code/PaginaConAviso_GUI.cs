using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SERVICES;

namespace GUI
{
    public abstract class PaginaConAviso_GUI : Page
    {
        private const string MENSAJE_GENERICO = "No se pudo completar la operación. Volvé a intentarlo.";

        protected void Avisar(string variante, string texto)
        {
            Avisar(Buscar<Panel>(this, "pnlAviso"), Buscar<Literal>(this, "litAviso"), variante, texto);
        }

        protected void Avisar(Panel panel, Literal literal, string variante, string texto)
        {
            panel.CssClass = "aviso " + variante;
            literal.Text = Server.HtmlEncode(texto);
            panel.Visible = true;
        }

        protected virtual bool EsErrorDeUsuario(Exception ex)
        {
            return ex is UnauthorizedAccessException || ex is InvalidOperationException;
        }

        protected bool Intentar(Action accion, string origen, Action<string> alFallar)
        {
            try
            {
                accion();
                return true;
            }
            catch (Exception ex)
            {
                if (EsErrorDeUsuario(ex))
                {
                    alFallar(ex.Message);
                }
                else
                {
                    LogErrores_SERVICE.Registrar(origen, ex);
                    alFallar(MENSAJE_GENERICO);
                }
            }

            return false;
        }

        protected void Ejecutar(string origen, string exito, Action accion)
        {
            if (Intentar(accion, origen, mensaje => Avisar("aviso-peligro", mensaje))) Avisar("aviso-exito", exito);
        }

        private static T Buscar<T>(Control raiz, string id) where T : Control
        {
            foreach (Control hijo in raiz.Controls)
            {
                if (hijo.ID == id) return hijo as T;

                T encontrado = Buscar<T>(hijo, id);
                if (encontrado != null) return encontrado;
            }

            return null;
        }
    }
}
