using System;
using System.Collections.Generic;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class MisTobiis : PaginaBase_GUI
    {
        protected const string URL_AGENTE = "http://localhost:8887";

        private bool permitido;

        protected bool SinDispositivos { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            permitido = SesionActual_GUI.ExigirPermiso(Patentes_TLL.OPERAR_ANALISIS);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!permitido) return;

            var lista = new List<Dispositivo_BE>();

            try
            {
                var actor = SesionActual_GUI.ObtenerActor();
                if (actor != null) lista = new SesionGrabada_BLL().DispositivosParaGrabar(actor);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("MisTobiis.Lista", ex);
            }

            SinDispositivos = lista.Count == 0;
            rptTobiis.DataSource = lista;
            rptTobiis.DataBind();
        }

        protected string TextoEstado(object estado)
        {
            return Traducir(Dispositivo_BLL.ClaveEstado((EstadoDispositivo)estado));
        }
    }
}
