using System;
using System.Collections.Generic;
using System.Web;
using TLL;
using BE;
using BLL;
using SERVICES;

namespace GUI
{
    public partial class NuevaGrabacion : PaginaBase_GUI
    {
        protected const string URL_AGENTE = "http://localhost:8887";
        protected const int ESPERA_AGENTE_MS = 2500;
        protected const int SONDEO_MS = 3000;
        protected const int SONDEO_GRABACION_MS = 1000;
        protected const int ANCHO_VIDEO = 1280;
        protected const int ALTO_VIDEO = 720;
        protected const int FPS_VIDEO = 30;
        protected const int BITS_VIDEO_POR_SEGUNDO = 2500000;

        private static readonly int[] RETRASOS_SEGUNDOS = { 0, 3, 5, 10 };

        public class Chequeo
        {
            public string Id { get; set; }
            public string Nombre { get; set; }
            public string Inicial { get; set; }
            public string Clase { get; set; }
            public bool Obligatorio { get; set; }
            public bool ConEnlaceInstalacion { get; set; }
            public bool ConBotonTobii { get; set; }
            public bool ConCasilla { get; set; }
            public Dictionary<string, string> Textos { get; set; }

            public Chequeo()
            {
                Textos = new Dictionary<string, string>();
            }
        }

        public class Opcion
        {
            public string Valor { get; set; }
            public string Texto { get; set; }
        }

        // Solo se muestra: la sesion siempre se guarda con el ID del usuario logueado (el servidor no acepta otro).
        protected string NombreAnalista
        {
            get { return SesionActual_GUI.HayUsuario ? (SesionActual_GUI.Nombre ?? string.Empty) : string.Empty; }
        }

        protected bool SinDispositivos { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(TLL.Patentes_TLL.OPERAR_ANALISIS)) return;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            rptChequeos.DataSource = ArmarChequeos();
            rptChequeos.DataBind();

            rptCategorias.DataSource = ArmarCategorias();
            rptCategorias.DataBind();

            var dispositivos = ArmarDispositivos();
            SinDispositivos = dispositivos.Count == 0;
            rptDispositivos.DataSource = dispositivos;
            rptDispositivos.DataBind();

            rptEdades.DataSource = ArmarOpciones(PerfilTester_BE.RangosEdad, null);
            rptEdades.DataBind();
            rptGeneros.DataSource = ArmarOpciones(PerfilTester_BE.Generos, "grabacion_tester_genero_");
            rptGeneros.DataBind();
            rptFamiliaridades.DataSource = ArmarOpciones(PerfilTester_BE.Familiaridades, "grabacion_tester_familiaridad_");
            rptFamiliaridades.DataBind();

            rptRetrasos.DataSource = ArmarRetrasos();
            rptRetrasos.DataBind();
        }

        private List<Opcion> ArmarCategorias()
        {
            var opciones = new List<Opcion>();

            try
            {
                var actor = SesionActual_GUI.ObtenerActor();
                if (actor == null) return opciones;

                foreach (Categoria_BE c in new Categoria_BLL().ObtenerPorEmpresa(actor))
                {
                    if (c.Activa) opciones.Add(new Opcion { Valor = c.IdCategoria.ToString(), Texto = c.NombreCategoria });
                }
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("NuevaGrabacion.Categorias", ex);
            }

            return opciones;
        }

        // los dispositivos asignados a la empresa del usuario
        private List<Opcion> ArmarDispositivos()
        {
            var opciones = new List<Opcion>();

            try
            {
                var actor = SesionActual_GUI.ObtenerActor();
                if (actor == null) return opciones;

                foreach (Dispositivo_BE d in new SesionGrabada_BLL().DispositivosParaGrabar(actor))
                {
                    opciones.Add(new Opcion { Valor = d.IdDispositivo.ToString(), Texto = d.Modelo + " - " + d.NumeroSerie });
                }
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("NuevaGrabacion.Dispositivos", ex);
            }

            return opciones;
        }

        private List<Opcion> ArmarOpciones(string[] valores, string prefijoClave)
        {
            var opciones = new List<Opcion>();

            foreach (string valor in valores)
            {
                string texto = prefijoClave == null ? valor : Traducir(prefijoClave + valor);
                opciones.Add(new Opcion { Valor = valor, Texto = texto });
            }

            return opciones;
        }

        private List<Opcion> ArmarRetrasos()
        {
            var opciones = new List<Opcion>();

            foreach (int segundos in RETRASOS_SEGUNDOS)
            {
                string texto = segundos == 0 ? Traducir("grabacion_rec_sin_retraso") : Traducir("grabacion_rec_retraso_n", segundos);
                opciones.Add(new Opcion { Valor = segundos.ToString(), Texto = texto });
            }

            return opciones;
        }

        protected string Atributos(object elemento)
        {
            var chequeo = (Chequeo)elemento;
            string atributos = chequeo.Obligatorio ? " data-obligatorio" : "";

            foreach (KeyValuePair<string, string> texto in chequeo.Textos)
            {
                atributos += " data-" + texto.Key + "=\"" + HttpUtility.HtmlAttributeEncode(texto.Value) + "\"";
            }

            return atributos;
        }

        private List<Chequeo> ArmarChequeos()
        {
            string verificando = Traducir("grabacion_prep_verificando");
            string calibracionPantalla = Traducir("grabacion_prep_cal_pantalla_detalle");
            string calibracionVista = Traducir("grabacion_prep_cal_vista_detalle");
            string confirmado = Traducir("grabacion_prep_confirmado");

            return new List<Chequeo>
            {
                new Chequeo
                {
                    Id = "agente",
                    Nombre = Traducir("grabacion_prep_agente"),
                    Inicial = verificando,
                    Obligatorio = true,
                    ConEnlaceInstalacion = true,
                    Textos =
                    {
                        { "correcto", Traducir("grabacion_prep_agente_ok") },
                        { "problema", Traducir("grabacion_prep_agente_falta") }
                    }
                },
                new Chequeo
                {
                    Id = "tobii",
                    Nombre = Traducir("grabacion_prep_tobii"),
                    Inicial = verificando,
                    Obligatorio = true,
                    ConBotonTobii = true,
                    Textos =
                    {
                        { "correcto", Traducir("grabacion_prep_tobii_ok") },
                        { "simulado", Traducir("grabacion_prep_tobii_simulado") },
                        { "problema", Traducir("grabacion_prep_tobii_falta") },
                        { "sinagente", Traducir("grabacion_prep_tobii_sin_agente") }
                    }
                },
                new Chequeo
                {
                    Id = "pantalla",
                    Nombre = Traducir("grabacion_prep_pantalla"),
                    Inicial = verificando,
                    Obligatorio = true,
                    Textos =
                    {
                        { "correcto", Traducir("grabacion_prep_pantalla_ok") },
                        { "problema", Traducir("grabacion_prep_pantalla_falta") }
                    }
                },
                new Chequeo
                {
                    Id = "calPantalla",
                    Nombre = Traducir("grabacion_prep_cal_pantalla"),
                    Inicial = calibracionPantalla,
                    Clase = "pendiente",
                    ConBotonTobii = true,
                    ConCasilla = true,
                    Textos =
                    {
                        { "pendiente", calibracionPantalla },
                        { "correcto", confirmado }
                    }
                },
                new Chequeo
                {
                    Id = "calVista",
                    Nombre = Traducir("grabacion_prep_cal_vista"),
                    Inicial = calibracionVista,
                    Clase = "pendiente",
                    ConBotonTobii = true,
                    ConCasilla = true,
                    Textos =
                    {
                        { "pendiente", calibracionVista },
                        { "correcto", confirmado }
                    }
                }
            };
        }
    }
}
