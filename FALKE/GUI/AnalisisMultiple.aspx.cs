using System;

namespace GUI
{
    public partial class AnalisisMultiple : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(TLL.Patentes_TLL.OPERAR_ANALISIS)) return;
        }
    }
}
