using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SERVICES;
using TLL;

namespace GUI
{
    public abstract class PaginaConAviso_GUI : PaginaBase_GUI
    {
        protected void Avisar(string variante, string texto)
        {
            Avisar(Buscar<Panel>(this, "pnlAviso"), Buscar<Literal>(this, "litAviso"), variante, texto);
        }

        protected void Avisar(Panel panel, Literal literal, string variante, string texto)
        {
            PublicarAviso(panel, literal, "aviso " + variante, texto);
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
                    alFallar(MensajeDe(ex));
                }
                else
                {
                    LogErrores_SERVICE.Registrar(origen, ex);
                    alFallar(Traducir("comun_error_reintentar"));
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
