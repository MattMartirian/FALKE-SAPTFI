using System;

namespace GUI
{
    public partial class Dispositivos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(TLL.Patentes_TLL.GESTIONAR_DISPOSITIVOS)) return;
        }
    }
}
