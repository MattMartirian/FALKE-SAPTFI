using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class SelectorIdioma : UserControl, IObservadorIdioma_TLL
    {
        private SujetoIdioma_TLL sujeto;

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                foreach (Idioma_TE idioma in new Idioma_TLL().ListarActivos())
                    ddlIdioma.Items.Add(new ListItem(idioma.NombreIdioma, idioma.CodigoIdioma));

                sujeto = IdiomaActual_GUI.Sujeto;
                sujeto.Registrar(this);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("SelectorIdioma.Cargar", ex);
                Visible = false;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (sujeto != null) Seleccionar(sujeto.CodigoIdioma);
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (sujeto != null) sujeto.Quitar(this);
        }

        public void ActualizarIdioma(string codigoIdioma)
        {
            Seleccionar(codigoIdioma);
        }

        protected void ddlIdioma_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                IdiomaActual_GUI.Cambiar(ddlIdioma.SelectedValue);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("SelectorIdioma.Cambiar", ex);
            }
        }

        private void Seleccionar(string codigo)
        {
            ListItem item = ddlIdioma.Items.FindByValue(codigo);

            if (item == null) return;

            ddlIdioma.ClearSelection();
            item.Selected = true;
        }
    }
}
