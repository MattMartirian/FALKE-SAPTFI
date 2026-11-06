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
    public class AnalisisMultiple_WebMethod : WebService
    {
        private const int MAX_SESIONES = 500;

        private static readonly string[] CLAVES_INFORME =
        {
            "informe_titulo", "informe_emision", "informe_emitido_por", "informe_pagina", "informe_seccion_empresa", "informe_empresa",
            "informe_cuit", "informe_rubro", "informe_categoria", "informe_tipo_activo", "informe_activo", "informe_figura",
            "informe_seccion_visualizaciones", "informe_seccion_definiciones", "informe_multiple_seccion_analisis", "informe_multiple_sesiones_elegidas",
            "informe_multiple_todos", "informe_multiple_seccion_sesiones", "informe_multiple_col_numero", "informe_multiple_col_sesion",
            "informe_multiple_col_fecha", "informe_multiple_col_duracion", "informe_multiple_por_edad", "informe_multiple_por_genero",
            "informe_multiple_por_familiaridad", "informe_multiple_dato_con_mirada", "informe_multiple_dato_consenso", "informe_multiple_dato_barras",
            "informe_multiple_y_mas", "informe_multiple_def_mediana", "informe_multiple_def_muestra", "informe_multiple_def_titulo_muestra",
            "informe_multiple_def_titulo_mediana", "analisismultiple_analisis_multiple", "analisismultiple_sesiones_combinadas",
            "analisismultiple_duracion_promedio", "analisismultiple_fijacion_media", "analisismultiple_fijaciones_promedio",
            "analisismultiple_atencion_activa", "analisismultiple_sin_datos", "analisismultiple_muestra_pequena", "analisismultiple_mediana_n",
            "analisismultiple_calidad_titulo", "analisismultiple_calidad_usadas", "analisismultiple_calidad_excluidas", "analisismultiple_calidad_variacion",
            "analisismultiple_calidad_aviso", "analisismultiple_calidad_pocas", "analisismultiple_calidad_duraciones", "analisismultiple_metricas_promediadas",
            "analisismultiple_filtro_titulo", "analisismultiple_densidad_de_fijaciones_promediada_entre", "analisismultiple_areas_que_la_mayoria_de_los",
            "analisismultiple_cuanto_varia_la_atencion_entre_los", "analisismultiple_proporcion_media_de_fijacion_activa_fren",
            "analisismultiple_recorrido_superpuesto", "analisismultiple_comparacion_directa_del_porcentaje_de_at", "analisismultiple_centro_de_cada_sesion",
            "analisismultiple_mapa_de_calor", "analisismultiple_zonas_de_interes_y_ciegas", "analisismultiple_dispersion_de_mirada",
            "analisismultiple_atencion_vs_distraccion", "analisismultiple_recorrido_de_mirada", "analisismultiple_atencion_por_sesion",
            "vis_fijacion_activa_n", "vis_sacadico_n", "vis_ayuda_duracion", "vis_ayuda_fijacion_media", "vis_ayuda_fijaciones",
            "vis_ayuda_atencion", "grabacion_tester_edad", "grabacion_tester_genero", "grabacion_tester_familiaridad", "visualizaciones_mayor_concentracion",
            "visualizaciones_concentracion_media", "visualizaciones_menor_concentracion", "visualizaciones_zona_de_interes",
            "visualizaciones_zona_ciega", "visualizaciones_centroide", "visualizaciones_elipse_de_dispersion", "default_inicio",
            "visualizaciones_final"
        };

        public class Grupo
        {
            public string valor { get; set; }
            public string texto { get; set; }
        }

        public class RespuestaInformeMultiple
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

            public string categoria { get; set; }
            public string tipoActivo { get; set; }
            public string activo { get; set; }

            public Dictionary<string, List<Grupo>> grupos { get; set; }
        }

        [WebMethod(EnableSession = true)]
        public RespuestaInformeMultiple DatosInformeMultiple(string[] tokens, string idioma)
        {
            try
            {
                ActorUsuario_TE actor = SesionActual_GUI.ObtenerActor();
                if (actor == null) return new RespuestaInformeMultiple { mensaje = IdiomaActual_GUI.Texto("comun_sesion_expirada") };

                var guardados = Session[GrabacionArchivo_GUI.CLAVE_SESION] as Dictionary<string, int>;
                var ids = new List<int>();

                if (guardados == null || tokens == null || tokens.Length == 0 || tokens.Length > MAX_SESIONES)
                    return new RespuestaInformeMultiple { mensaje = IdiomaActual_GUI.Texto("sesion_no_existe") };

                foreach (string token in tokens)
                {
                    int idSesion;

                    if (token == null || !guardados.TryGetValue(token, out idSesion))
                        return new RespuestaInformeMultiple { mensaje = IdiomaActual_GUI.Texto("sesion_no_existe") };

                    ids.Add(idSesion);
                }

                string codigo = Informe_GUI.IdiomaValido(idioma);
                InformeMultiple_BE informe = new SesionGrabada_BLL().ObtenerParaInformeMultiple(actor, ids, codigo);

                return Armar(informe, codigo);
            }
            catch (Exception ex)
            {
                var traducible = ex as IMensajeTraducible_TLL;

                if (traducible != null) return new RespuestaInformeMultiple { mensaje = traducible.Traducir(IdiomaActual_GUI.Codigo) };

                LogErrores_SERVICE.Registrar("AnalisisMultiple.Informe", ex);
                return new RespuestaInformeMultiple { mensaje = IdiomaActual_GUI.Texto("vis_exportar_error") };
            }
        }

        private static RespuestaInformeMultiple Armar(InformeMultiple_BE informe, string codigo)
        {
            CultureInfo cultura = Traductor_TLL.Cultura(codigo);

            var r = new RespuestaInformeMultiple
            {
                ok = true,
                idioma = cultura.Name.Length > 0 ? cultura.Name : codigo,
                textos = Informe_GUI.Textos(CLAVES_INFORME, codigo),
                emitidoPor = SesionActual_GUI.Nombre,
                fechaEmision = DateTime.Now.ToString("g", cultura),
                categoria = informe.Sesiones[0].NombreCategoria,
                tipoActivo = Informe_GUI.EtiquetaTipo(informe.Sesiones[0].TipoActivo, codigo),
                grupos = new Dictionary<string, List<Grupo>>()
            };

            if (informe.Empresa != null)
            {
                r.empresa = informe.Empresa.NombreEmpresa;
                r.cuit = informe.Empresa.Cuit;
                r.rubro = informe.Empresa.Rubro;
            }

            if (informe.Categoria != null) r.activo = informe.Categoria.NombreActivo;

            r.grupos["edad"] = Grupos(PerfilTester_BE.RangosEdad, null, codigo);
            r.grupos["genero"] = Grupos(PerfilTester_BE.Generos, "grabacion_tester_genero_", codigo);
            r.grupos["familiaridad"] = Grupos(PerfilTester_BE.Familiaridades, "grabacion_tester_familiaridad_", codigo);

            return r;
        }

        private static List<Grupo> Grupos(string[] valores, string prefijo, string codigo)
        {
            var lista = new List<Grupo>();

            foreach (string v in valores)
            {
                lista.Add(new Grupo { valor = v, texto = prefijo == null ? v : Traductor_TLL.Traducir(prefijo + v, codigo) });
            }

            return lista;
        }
    }
}
