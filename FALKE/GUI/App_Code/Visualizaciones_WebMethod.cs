using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Script.Services;
using System.Web.Services;
using BE;
using BLL;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    [WebService(Namespace = "http://falke.local/")]
    [ScriptService]
    public class Visualizaciones_WebMethod : WebService
    {
        private static readonly string[] CLAVES_INFORME =
        {
            "informe_titulo", "informe_emision", "informe_emitido_por", "informe_pagina", "informe_nota",
            "informe_seccion_empresa", "informe_empresa", "informe_cuit", "informe_rubro",
            "informe_seccion_sesion", "informe_sesion", "informe_categoria", "informe_tipo_activo", "informe_fecha_grabacion",
            "informe_duracion", "informe_analista", "informe_muestras", "informe_dispositivo",
            "informe_seccion_activo", "informe_activo", "informe_flujo",
            "informe_seccion_perfil", "informe_edad", "informe_genero", "informe_familiaridad",
            "informe_seccion_objetivo", "informe_seccion_metricas", "informe_seccion_visualizaciones", "informe_seccion_definiciones",
            "informe_figura", "informe_tramo", "informe_cuadro", "informe_sin_mirada",
            "informe_desc_grabacion", "informe_desc_calor", "informe_desc_zonas", "informe_desc_dispersion", "informe_desc_atencion", "informe_desc_recorrido",
            "informe_dato_muestras_validas", "informe_dato_zonas", "informe_dato_ciegas", "informe_dato_centroide", "informe_dato_elipse",
            "informe_dato_atencion", "informe_dato_sacadico", "informe_dato_sin_datos", "informe_dato_recorrido", "informe_dato_fijaciones",

            "visualizaciones_grabacion_con_seguimiento", "analisismultiple_mapa_de_calor", "analisismultiple_zonas_de_interes_y_ciegas",
            "analisismultiple_dispersion_de_mirada", "visualizaciones_atencion_frente_a_distraccion", "analisismultiple_recorrido_de_mirada",
            "visualizaciones_duracion_total", "analisismultiple_fijacion_media", "visualizaciones_fijaciones",
            "analisismultiple_atencion_activa", "visualizaciones_movimiento_sacadico",
            "vis_ayuda_duracion", "vis_ayuda_fijacion_media", "vis_ayuda_fijaciones", "vis_ayuda_atencion", "vis_ayuda_sacadico",

            "visualizaciones_punto_de_mirada_actual", "visualizaciones_mayor_concentracion", "visualizaciones_concentracion_media",
            "visualizaciones_menor_concentracion", "visualizaciones_zona_de_interes", "visualizaciones_zona_ciega",
            "visualizaciones_punto_de_mirada", "visualizaciones_centroide", "visualizaciones_elipse_de_dispersion",
            "default_inicio", "visualizaciones_final", "vis_atencion_sin_datos"
        };

        public class Dato
        {
            public string etiqueta { get; set; }
            public string valor { get; set; }
        }

        public class RespuestaInforme
        {
            public bool ok { get; set; }
            public string mensaje { get; set; }

            public string idioma { get; set; }
            public Dictionary<string, string> textos { get; set; }

            public string empresa { get; set; }
            public string cuit { get; set; }
            public string rubro { get; set; }
            public string emitidoPor { get; set; }
            public string fechaEmision { get; set; }

            public string sesion { get; set; }
            public string categoria { get; set; }
            public string tipoActivo { get; set; }
            public string fechaGrabacion { get; set; }
            public string duracion { get; set; }
            public string analista { get; set; }
            public string muestras { get; set; }
            public string dispositivo { get; set; }
            public string objetivo { get; set; }

            public string edad { get; set; }
            public string genero { get; set; }
            public string familiaridad { get; set; }

            public string activo { get; set; }
            public string flujo { get; set; }
            public List<Dato> camposActivo { get; set; }
        }

        [WebMethod(EnableSession = true)]
        public RespuestaInforme DatosInforme(string token, string idioma)
        {
            try
            {
                ActorUsuario_TE actor = SesionActual_GUI.ObtenerActor();
                if (actor == null) return new RespuestaInforme { mensaje = IdiomaActual_GUI.Texto("comun_sesion_expirada") };

                var tokens = Session[GrabacionArchivo_GUI.CLAVE_SESION] as Dictionary<string, int>;
                int idSesion;

                if (tokens == null || token == null || !tokens.TryGetValue(token, out idSesion))
                    return new RespuestaInforme { mensaje = IdiomaActual_GUI.Texto("sesion_no_existe") };

                string codigo = Informe_GUI.IdiomaValido(idioma);
                InformeSesion_BE informe = new SesionGrabada_BLL().ObtenerParaInforme(actor, idSesion, codigo);

                return Armar(informe, codigo);
            }
            catch (Exception ex)
            {
                var traducible = ex as IMensajeTraducible_TLL;

                if (traducible != null) return new RespuestaInforme { mensaje = traducible.Traducir(IdiomaActual_GUI.Codigo) };

                LogErrores_SERVICE.Registrar("Visualizaciones.Informe", ex);
                return new RespuestaInforme { mensaje = IdiomaActual_GUI.Texto("vis_exportar_error") };
            }
        }

        private static RespuestaInforme Armar(InformeSesion_BE informe, string codigo)
        {
            CultureInfo cultura = Traductor_TLL.Cultura(codigo);
            SesionGrabada_BE s = informe.Sesion;

            var r = new RespuestaInforme
            {
                ok = true,
                idioma = cultura.Name.Length > 0 ? cultura.Name : codigo,
                textos = new Dictionary<string, string>(),
                emitidoPor = SesionActual_GUI.Nombre,
                fechaEmision = DateTime.Now.ToString("g", cultura),
                sesion = s.Nombre,
                categoria = s.NombreCategoria,
                tipoActivo = Informe_GUI.EtiquetaTipo(s.TipoActivo, codigo),
                fechaGrabacion = s.Fecha.ToString("g", cultura),
                duracion = FormatoDuracion(s.DuracionSegundos),
                analista = s.NombreAnalista,
                muestras = s.Muestras.ToString("N0", cultura),
                objetivo = s.Objetivo,
                edad = s.RangoEdad,
                genero = TextoOpcion("grabacion_tester_genero_", s.Genero, codigo),
                familiaridad = TextoOpcion("grabacion_tester_familiaridad_", s.Familiaridad, codigo),
                camposActivo = new List<Dato>()
            };

            foreach (string clave in CLAVES_INFORME) r.textos[clave] = Traductor_TLL.Traducir(clave, codigo);

            if (informe.Empresa != null)
            {
                r.empresa = informe.Empresa.NombreEmpresa;
                r.cuit = informe.Empresa.Cuit;
                r.rubro = informe.Empresa.Rubro;
            }

            if (informe.Dispositivo != null) r.dispositivo = informe.Dispositivo.Modelo + " - " + informe.Dispositivo.NumeroSerie;

            if (informe.Categoria != null)
            {
                r.activo = informe.Categoria.NombreActivo;
                r.flujo = informe.Categoria.FlujoEsperado;

                foreach (KeyValuePair<string, string> campo in informe.Categoria.ObtenerCampos())
                {
                    if (string.IsNullOrWhiteSpace(campo.Value)) continue;

                    r.camposActivo.Add(new Dato
                    {
                        etiqueta = Traductor_TLL.Traducir("informe_campo_" + campo.Key, codigo),
                        valor = ValorCampo(campo.Value, codigo)
                    });
                }
            }

            return r;
        }

        private static string ValorCampo(string valor, string codigo)
        {
            switch (valor)
            {
                case "Escritorio": return Traductor_TLL.Traducir("categorias_escritorio", codigo);
                case "Movil": return Traductor_TLL.Traducir("categorias_movil", codigo);
                case "Consola": return Traductor_TLL.Traducir("categorias_consola", codigo);
                case "Pc": return "PC";
                case "Ios": return "iOS";
                default: return valor;
            }
        }

        private static string TextoOpcion(string prefijo, string valor, string codigo)
        {
            return string.IsNullOrEmpty(valor) ? null : Traductor_TLL.Traducir(prefijo + valor, codigo);
        }

        private static string FormatoDuracion(int segundos)
        {
            var t = TimeSpan.FromSeconds(Math.Max(0, segundos));
            return t.TotalHours >= 1 ? string.Format("{0}:{1:00}:{2:00}", (int)t.TotalHours, t.Minutes, t.Seconds) : string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
        }
    }
}
