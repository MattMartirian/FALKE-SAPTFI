using System;

namespace GUI
{
    public partial class Visualizaciones : PaginaBase_GUI
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(TLL.Patentes_TLL.OPERAR_ANALISIS)) return;
        }
    }
}
