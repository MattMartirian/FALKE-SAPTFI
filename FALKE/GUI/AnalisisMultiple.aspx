<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="AnalisisMultiple.aspx.cs" Inherits="GUI.AnalisisMultiple" Title="[[analisismultiple_analisis_multiple]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .seleccion
        {
            display: grid;
            grid-template-columns: minmax(0, 1fr) 300px;
            gap: 18px;
            align-items: start;
        }

        @media (max-width: 980px)
        {
            .seleccion { grid-template-columns: minmax(0, 1fr); }
        }

        .resumen-seleccion
        {
            position: sticky;
            top: calc(var(--alto-topbar) + 16px);
        }

        .elegidas
        {
            list-style: none;
            margin: 0 0 14px;
            padding: 0;
            display: grid;
            gap: 6px;
        }

        .elegidas li
        {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: .84rem;
            padding: 8px 10px;
            background: var(--superficie-2);
            border-radius: var(--radio-sm);
        }

        .elegidas li span
        {
            flex: 1;
            min-width: 0;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .contador
        {
            font-size: 2rem;
            font-weight: 700;
            line-height: 1;
        }

        .fila-sesion .opcion-detalle
        {
            display: flex;
            flex-wrap: wrap;
            gap: 4px 12px;
        }

        .fila-sesion.incompatible
        {
            opacity: .5;
        }

        .barra-lista
        {
            padding: 10px 0;
        }

        .grafico canvas
        {
            width: 100%;
            height: 100%;
            display: block;
        }

        .leyenda
        {
            display: flex;
            flex-wrap: wrap;
            gap: 14px;
            margin-top: 14px;
            font-size: .8rem;
            color: var(--texto-suave);
        }

        .leyenda span
        {
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .leyenda i
        {
            width: 13px;
            height: 13px;
            border-radius: 3px;
            display: inline-block;
        }

        .tablero-multiple
        {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 16px;
        }

        .filtros
        {
            display: grid;
            gap: 14px;
        }

        .filtro-grupo
        {
            border: 0;
            margin: 0;
            padding: 0;
        }

        .filtro-grupo legend
        {
            padding: 0;
            margin-bottom: 8px;
            font-size: .85rem;
            font-weight: 600;
            color: var(--texto-suave);
        }

        .filtro-opciones
        {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }

        .filtro-opcion
        {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 10px;
            border: 1px solid var(--borde);
            border-radius: var(--radio-sm);
            background: var(--superficie);
            font-size: .85rem;
            cursor: pointer;
        }

        .filtro-opcion:has(input:checked)
        {
            border-color: var(--acento);
            background: var(--superficie-2);
        }

        .filtro-pie
        {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            margin-top: 16px;
        }

        @media (max-width: 700px)
        {
            .tablero-multiple { grid-template-columns: minmax(0, 1fr); }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <section id="panelSeleccion">

        <div class="pagina-cabecera">
            <div>
                <h1 data-i18n="multiple.titulo">[[analisismultiple_analisis_multiple]]</h1>
                <p class="texto-suave sin-margen medida" data-i18n="multiple.bajada">
                    [[analisismultiple_combina_varias_sesiones_para_obtener_res]]
                </p>
            </div>
        </div>

        <div class="aviso aviso-info">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
            <div>
                <strong data-i18n="multiple.regla.titulo">[[analisismultiple_todas_las_sesiones_tienen_que_ser]]</strong>
                <p class="sin-margen" data-i18n="multiple.regla.texto">
                    [[analisismultiple_promediar_sesiones_de_tipos_distintos_da]]
                </p>
            </div>
        </div>

        <div class="seleccion">

            <div class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-ojo" /></svg>
                    <span data-i18n="multiple.disponibles">[[analisismultiple_sesiones_finalizadas]]</span>
                    <span class="derecha texto-chico texto-suave" data-i18n="multiple.soloFinalizadas">[[analisismultiple_solo_se_listan_las_ya_procesadas]]</span>
                </div>

                <div class="barra-lista">
                    <button type="button" class="btn btn-secundario btn-chico" id="btnCategoria" disabled>
                        [[analisismultiple_seleccionar_todos_los_de_esta_categoria]]
                    </button>
                </div>

                <% if (HayError) { %>
                <div class="aviso aviso-peligro">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen">[[vis_error_listado]]</p>
                </div>
                <% } %>

                <% if (!HaySesiones && !HayError) { %>
                <div class="vacio">
                    <svg width="28" height="28" aria-hidden="true"><use href="#i-carpeta" /></svg>
                    <p class="sin-margen"><%: Traducir("vis_sin_sesiones_empresa") %></p>
                </div>
                <% } %>

                <div id="listaElegibles">
                    <asp:Repeater ID="rptSesiones" runat="server">
                        <ItemTemplate>
                            <label class="opcion fila-sesion" data-categoria='<%#: Eval("IdCategoria") %>' data-categoria-nombre='<%#: Eval("Categoria") %>'
                                   data-duracion-seg='<%#: Eval("DuracionSegundos") %>' data-desfase='<%#: Eval("DesfaseMs") %>' data-muestras='<%#: Eval("Muestras") %>'
                                   data-edad='<%#: Eval("Edad") %>' data-genero='<%#: Eval("Genero") %>' data-familiaridad='<%#: Eval("Familiaridad") %>'>
                                <input type="checkbox" name="sesion" value='<%#: Eval("Token") %>' data-nombre='<%#: Eval("Nombre") %>' />
                                <span>
                                    <span class="opcion-titulo"><%#: Eval("Nombre") %></span>
                                    <span class="opcion-detalle">
                                        <span><%#: Eval("Categoria") %></span><span><%#: Eval("Fecha") %></span><span><%#: Eval("Duracion") %></span><span><%#: Eval("Analista") %></span>
                                    </span>
                                </span>
                            </label>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <aside class="tarjeta resumen-seleccion">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-comparar" /></svg>
                    <span data-i18n="multiple.resumen">[[analisismultiple_seleccion_actual]]</span>
                </div>

                <p class="contador" id="contadorElegidas">0</p>
                <p class="texto-chico texto-suave" id="tipoElegido" data-i18n="multiple.sinTipo">
                    [[analisismultiple_todavia_no_elegiste_ninguna_sesion]]
                </p>

                <ul class="elegidas" id="listaElegidas"></ul>

                <button type="button" class="btn btn-primario btn-bloque" id="btnGenerar" disabled>
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-metricas" /></svg>
                    <span data-i18n="multiple.generar">[[analisismultiple_generar_analisis]]</span>
                </button>

                <p class="ayuda" id="avisoSeleccion" role="status" data-i18n="multiple.minimo">
                    [[analisismultiple_necesitas_al_menos_dos_sesiones_del]]
                </p>
            </aside>

        </div>
    </section>

    <section id="panelResultado" hidden>

        <p class="migas">
            <button type="button" class="btn btn-fantasma btn-chico" id="btnVolverSeleccion" style="padding-left:0">
                <span data-i18n="multiple.volver">[[analisismultiple_seleccion_de_sesiones]]</span>
            </button>
            / <strong data-i18n="multiple.resultado">[[analisismultiple_resultado_combinado]]</strong>
        </p>

        <div class="pagina-cabecera">
            <div>
                <h1 id="tituloResultado" data-i18n="multiple.resultado.titulo">[[analisismultiple_resultado_combinado]]</h1>
                <p class="texto-suave sin-margen" id="subtituloResultado"></p>
            </div>
            <div class="acciones">
                <button type="button" class="btn btn-primario">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-descarga" /></svg>
                    <span data-i18n="multiple.exportar">[[analisismultiple_descargar_reporte]]</span>
                </button>
            </div>
        </div>

        <div class="aviso aviso-info" id="resultadoEstado" role="status" hidden>
            <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen" id="resultadoEstadoTexto"></p>
        </div>

        <div class="aviso aviso-alerta" id="resultadoFallos" hidden>
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen" id="resultadoFallosTexto"></p>
        </div>

        <div id="resultadoCuerpo" hidden>

            <div class="tarjeta mb-24">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-comparar" /></svg>
                    <span>[[analisismultiple_filtro_titulo]]</span>
                </div>
                <p class="texto-chico texto-suave">[[analisismultiple_filtro_ayuda]]</p>

                <div class="filtros" id="filtrosGrupos"></div>

                <div class="filtro-pie">
                    <p class="texto-suave sin-margen" id="filtroResumen" role="status"></p>
                    <button type="button" class="btn btn-primario" id="btnRegenerar">[[analisismultiple_filtro_regenerar]]</button>
                </div>
            </div>

            <div class="aviso aviso-alerta" id="filtroVacio" hidden>
                <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
                <p class="sin-margen">[[analisismultiple_filtro_vacio]]</p>
            </div>

            <div id="resultadoDatos">
            <div class="tarjeta mb-24">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
                    <span>[[analisismultiple_calidad_titulo]]</span>
                </div>
                <div class="metricas" id="calidadValores"></div>
                <div id="calidadAvisos"></div>
            </div>

            <div class="tarjeta mb-24">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-metricas" /></svg>
                    <span data-i18n="multiple.metricas">[[analisismultiple_metricas_promediadas]]</span>
                </div>
                <div class="metricas" id="metricasGenerales"></div>
            </div>

            <div class="tarjeta mb-24">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-comparar" /></svg>
                    <span>[[analisismultiple_por_grupo_titulo]]</span>
                </div>
                <div id="metricasGrupos"></div>
            </div>

            <h2 class="mb-16" style="font-size:1.1rem" data-i18n="multiple.graficos">[[analisismultiple_graficos_promediados]]</h2>
            <%-- esto es para la info: nota de la cuadricula sacada
            <p class="texto-chico texto-suave mb-16">[[analisismultiple_nota_cuadricula]]</p>
            --%>

            <div class="tablero-multiple">

                <article class="tarjeta">
                    <div class="tarjeta-cabecera">
                        <svg width="17" height="17" aria-hidden="true"><use href="#i-calor" /></svg>
                        <span data-i18n="vis.grafico.calor">[[analisismultiple_mapa_de_calor]]</span>
                    </div>
                    <div class="grafico" id="graficoCalor" aria-hidden="true"></div>
                    <div class="leyenda">
                        <span><i style="background:#F26157"></i>[[visualizaciones_mayor_concentracion]]</span>
                        <span><i style="background:#E8AF52"></i>[[visualizaciones_concentracion_media]]</span>
                        <span><i style="background:#56637C"></i>[[visualizaciones_menor_concentracion]]</span>
                    </div>
                    <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.calor.detalle">
                        [[analisismultiple_densidad_de_fijaciones_promediada_entre]]
                    </p>
                </article>

                <article class="tarjeta">
                    <div class="tarjeta-cabecera">
                        <svg width="17" height="17" aria-hidden="true"><use href="#i-zonas" /></svg>
                        <span data-i18n="vis.grafico.zonas">[[analisismultiple_zonas_de_interes_y_ciegas]]</span>
                    </div>
                    <div class="grafico" id="graficoZonas" aria-hidden="true"></div>
                    <div class="leyenda">
                        <span><i style="background:rgba(232,175,82,.5);border:1px solid #E8AF52"></i>[[visualizaciones_zona_de_interes]]</span>
                        <span><i style="background:transparent;border:1px dashed #56637C"></i>[[visualizaciones_zona_ciega]]</span>
                    </div>
                    <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.zonas.detalle">
                        [[analisismultiple_areas_que_la_mayoria_de_los]]
                    </p>
                </article>

                <article class="tarjeta">
                    <div class="tarjeta-cabecera">
                        <svg width="17" height="17" aria-hidden="true"><use href="#i-dispersion" /></svg>
                        <span data-i18n="vis.grafico.dispersion">[[analisismultiple_dispersion_de_mirada]]</span>
                    </div>
                    <div class="grafico" id="graficoDispersion" aria-hidden="true"></div>
                    <div class="leyenda">
                        <span><i style="background:#56637C"></i>[[analisismultiple_centro_de_cada_sesion]]</span>
                        <span><i style="background:#F26157"></i>[[visualizaciones_centroide]]</span>
                        <span><i style="background:transparent;border:1px dashed #E8AF52"></i>[[visualizaciones_elipse_de_dispersion]]</span>
                    </div>
                    <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.dispersion.detalle">
                        [[analisismultiple_cuanto_varia_la_atencion_entre_los]]
                    </p>
                </article>

                <article class="tarjeta">
                    <div class="tarjeta-cabecera">
                        <svg width="17" height="17" aria-hidden="true"><use href="#i-torta" /></svg>
                        <span data-i18n="vis.grafico.atencion">[[analisismultiple_atencion_vs_distraccion]]</span>
                    </div>
                    <div class="grafico" id="graficoAtencion" aria-hidden="true"></div>
                    <div class="leyenda" id="leyendaAtencion"></div>
                    <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.atencion.detalle">
                        [[analisismultiple_proporcion_media_de_fijacion_activa_fren]]
                    </p>
                </article>

                <article class="tarjeta">
                    <div class="tarjeta-cabecera">
                        <svg width="17" height="17" aria-hidden="true"><use href="#i-recorrido" /></svg>
                        <span data-i18n="vis.grafico.recorrido">[[analisismultiple_recorrido_de_mirada]]</span>
                    </div>
                    <div class="grafico" id="graficoRecorrido" aria-hidden="true"></div>
                    <div class="leyenda">
                        <span><i style="background:#56637C"></i>[[default_inicio]]</span>
                        <span><i style="background:#F26157"></i>[[visualizaciones_final]]</span>
                    </div>
                    <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.recorrido.detalle">
                        [[analisismultiple_recorrido_superpuesto]]
                    </p>
                </article>

                <article class="tarjeta">
                    <div class="tarjeta-cabecera">
                        <svg width="17" height="17" aria-hidden="true"><use href="#i-comparar" /></svg>
                        <span data-i18n="multiple.porSesion">[[analisismultiple_atencion_por_sesion]]</span>
                    </div>
                    <div class="grafico" id="graficoPorSesion" aria-hidden="true"></div>
                    <div class="leyenda" id="leyendaPorSesion"></div>
                    <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.porSesion.detalle">
                        [[analisismultiple_comparacion_directa_del_porcentaje_de_at]]
                    </p>
                </article>

            </div>
            </div>
        </div>
    </section>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        var URL_ARCHIVO = "<%: ResolveUrl("~/GrabacionArchivo.ashx") %>";
        var GRUPOS = <%= GruposJson %>;

        // textos traducidos para los .js (un .js externo no puede leer los [[...]] del servidor)
        var MultiTextos = {
            duracion: "[[js:analisismultiple_duracion_promedio]]",
            fijacionMedia: "[[js:analisismultiple_fijacion_media]]",
            fijaciones: "[[js:analisismultiple_fijaciones_promedio]]",
            atencion: "[[js:analisismultiple_atencion_activa]]",
            sesiones: "[[js:analisismultiple_sesiones_combinadas]]",
            grupoEdad: "[[js:grabacion_tester_edad]]",
            grupoGenero: "[[js:grabacion_tester_genero]]",
            grupoFamiliaridad: "[[js:grabacion_tester_familiaridad]]",
            sinDatos: "[[js:analisismultiple_sin_datos]]",
            muestraPequena: "[[js:analisismultiple_muestra_pequena]]",
            mediana: "[[js:analisismultiple_mediana_n]]",
            calidadUsadas: "[[js:analisismultiple_calidad_usadas]]",
            calidadExcluidas: "[[js:analisismultiple_calidad_excluidas]]",
            calidadVariacion: "[[js:analisismultiple_calidad_variacion]]",
            calidadAviso: "[[js:analisismultiple_calidad_aviso]]",
            calidadPocas: "[[js:analisismultiple_calidad_pocas]]",
            calidadDuraciones: "[[js:analisismultiple_calidad_duraciones]]",
            errorCargar: "[[js:analisismultiple_error_cargar_n]]",
            calculando: "[[js:analisismultiple_calculando]]",
            filtroResumen: "[[js:analisismultiple_filtro_resumen_n]]",
            atencionActiva: "[[js:vis_fijacion_activa_n]]",
            atencionSacadico: "[[js:vis_sacadico_n]]"
        };
    </script>

    <%-- el orden importa: cada archivo usa lo que define el anterior --%>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/analisis.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/comun.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-estadistica.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-sesiones.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-metricas.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-calidad.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-filtro.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-grafico.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-grafico-calor.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-grafico-zonas.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-grafico-dispersion.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-grafico-atencion.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-grafico-porsesion.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/multiple/multiple-grafico-recorrido.js") %>"></script>

    <script>
        (function () {
            "use strict";

            var casillas = document.querySelectorAll("input[name='sesion']");
            var filas = document.querySelectorAll(".fila-sesion");
            var contador = document.getElementById("contadorElegidas");
            var tipoElegido = document.getElementById("tipoElegido");
            var listaElegidas = document.getElementById("listaElegidas");
            var btnGenerar = document.getElementById("btnGenerar");
            var aviso = document.getElementById("avisoSeleccion");

            var btnCategoria = document.getElementById("btnCategoria");

            function elegidas() {
                return document.querySelectorAll("input[name='sesion']:checked");
            }

            // la fila de la primera sesion elegida; de ahi sale la categoria activa (null si no hay ninguna)
            function filaActiva() {
                var marcadas = elegidas();

                return marcadas.length === 0 ? null : marcadas[0].closest(".fila-sesion");
            }

            function actualizar() {
                var marcadas = elegidas();
                var activa = filaActiva();
                var categoria = activa === null ? null : activa.getAttribute("data-categoria");
                var i;

                for (i = 0; i < filas.length; i++) {
                    var casilla = filas[i].querySelector("input");
                    var incompatible = categoria !== null && filas[i].getAttribute("data-categoria") !== categoria;

                    casilla.disabled = incompatible;
                    filas[i].classList.toggle("incompatible", incompatible);
                }

                contador.textContent = marcadas.length;

                listaElegidas.innerHTML = "";

                for (i = 0; i < marcadas.length; i++) {
                    var li = document.createElement("li");
                    var icono = document.createElement("span");

                    icono.textContent = marcadas[i].getAttribute("data-nombre");
                    li.appendChild(icono);
                    listaElegidas.appendChild(li);
                }

                btnCategoria.disabled = categoria === null;

                if (categoria === null) {
                    tipoElegido.textContent = "[[js:analisismultiple_todavia_no_elegiste_ninguna_sesion]]";
                } else {
                    tipoElegido.textContent = "[[js:analisismultiple_categoria_activa_0]]".replace("{0}", activa.getAttribute("data-categoria-nombre"));
                }

                var suficientes = marcadas.length >= 2;

                btnGenerar.disabled = !suficientes;
                aviso.textContent = suficientes
                    ? "[[js:analisismultiple_listo_para_combinar_0_sesiones]]".replace("{0}", marcadas.length)
                    : "[[js:analisismultiple_necesitas_al_menos_dos_sesiones_del]]";
            }

            for (var i = 0; i < casillas.length; i++) {
                casillas[i].addEventListener("change", actualizar);
            }

            // marca todas las sesiones de la categoria de la que ya elegiste una
            btnCategoria.addEventListener("click", function () {
                var activa = filaActiva();

                if (activa === null) return;

                for (var i = 0; i < filas.length; i++) {
                    if (filas[i].getAttribute("data-categoria") === activa.getAttribute("data-categoria")) {
                        filas[i].querySelector("input").checked = true;
                    }
                }

                actualizar();
            });

            var panelSeleccion = document.getElementById("panelSeleccion");
            var panelResultado = document.getElementById("panelResultado");
            var subtitulo = document.getElementById("subtituloResultado");
            var estado = document.getElementById("resultadoEstado");
            var estadoTexto = document.getElementById("resultadoEstadoTexto");
            var fallos = document.getElementById("resultadoFallos");
            var fallosTexto = document.getElementById("resultadoFallosTexto");
            var cuerpo = document.getElementById("resultadoCuerpo");
            var corrida = 0;

            // los graficos (cada uno arma su canvas dentro de su tarjeta)
            // los 6 graficos (cada uno arma su canvas dentro de su tarjeta)
            var graficoCalor = new GraficoCalor("graficoCalor");
            var graficoZonas = new GraficoZonas("graficoZonas");
            var graficoDispersion = new GraficoDispersion("graficoDispersion");
            var graficoAtencion = new GraficoAtencion("graficoAtencion");
            var graficoRecorrido = new GraficoRecorrido("graficoRecorrido");
            var graficoPorSesion = new GraficoPorSesion("graficoPorSesion");

            // leyenda de atencion: dorado = atencion, gris = distraccion (con los porcentajes si se conocen)
            function leyendaAtencion(pct) {
                var activa = MultiTextos.atencionActiva;
                var sacadico = MultiTextos.atencionSacadico;

                activa = pct === null ? activa.replace(/\s*\{0\}\s*%?/, "") : activa.replace("{0}", pct);
                sacadico = pct === null ? sacadico.replace(/\s*\{0\}\s*%?/, "") : sacadico.replace("{0}", 100 - pct);

                return '<span><i style="background:' + COLOR_ATENCION + '"></i>' + activa + '</span>' +
                       '<span><i style="background:' + COLOR_DISTRACCION + '"></i>' + sacadico + '</span>';
            }

            var filtros = document.getElementById("filtrosGrupos");
            var datos = document.getElementById("resultadoDatos");
            var sesionesCargadas = [];

            armarFiltros(filtros, GRUPOS);

            // arma calidad, metricas, tablas y graficos con las sesiones que cumplen el filtro del tester
            function regenerar() {
                var elegidas = filtrarSesiones(filtros, sesionesCargadas);
                var conMirada = [];

                document.getElementById("filtroResumen").textContent = MultiTextos.filtroResumen.replace("{0}", elegidas.length).replace("{1}", sesionesCargadas.length);
                document.getElementById("filtroVacio").hidden = elegidas.length > 0;
                datos.hidden = elegidas.length === 0;

                if (elegidas.length === 0) return;

                for (var i = 0; i < elegidas.length; i++) {
                    if (elegidas[i].conMirada) conMirada.push(elegidas[i]);
                }

                pintarCalidad(document.getElementById("calidadValores"), document.getElementById("calidadAvisos"), evaluarCalidad(elegidas));
                pintarGenerales(document.getElementById("metricasGenerales"), elegidas);
                pintarGrupos(document.getElementById("metricasGrupos"), elegidas, gruposMarcados(document.getElementById("filtrosGrupos"), GRUPOS));

                graficoCalor.dibujar(conMirada);
                graficoZonas.dibujar(conMirada);
                graficoDispersion.dibujar(conMirada);
                graficoAtencion.dibujar(conMirada);
                graficoRecorrido.dibujar(conMirada);
                graficoPorSesion.dibujar(conMirada);

                document.getElementById("leyendaAtencion").innerHTML = leyendaAtencion(conMirada.length ? graficoAtencion.porcentaje : null);
                document.getElementById("leyendaPorSesion").innerHTML = leyendaAtencion(null);
            }

            document.getElementById("btnRegenerar").addEventListener("click", regenerar);

            btnGenerar.addEventListener("click", function () {
                var marcadas = elegidas();
                var nombres = [];
                var n = ++corrida;

                for (var i = 0; i < marcadas.length; i++) {
                    nombres.push(marcadas[i].getAttribute("data-nombre"));
                }

                subtitulo.textContent = filaActiva().getAttribute("data-categoria-nombre") + " · " + nombres.join(" · ");

                estadoTexto.textContent = MultiTextos.calculando;
                estado.hidden = false;
                fallos.hidden = true;
                cuerpo.hidden = true;

                panelSeleccion.hidden = true;
                panelResultado.hidden = false;
                window.scrollTo(0, 0);

                cargarSesiones(marcadas, URL_ARCHIVO).then(function (sesiones) {
                    if (n !== corrida) return;

                    var sinCargar = 0;

                    for (var k = 0; k < sesiones.length; k++) {
                        if (sesiones[k].fallo) sinCargar++;
                    }

                    fallos.hidden = sinCargar === 0;
                    fallosTexto.textContent = MultiTextos.errorCargar.replace("{0}", sinCargar);

                    // arranca con todo seleccionado y se genera de una
                    sesionesCargadas = sesiones;
                    marcarTodos(filtros);
                    regenerar();

                    estado.hidden = true;
                    cuerpo.hidden = false;
                });
            });

            document.getElementById("btnVolverSeleccion").addEventListener("click", function () {
                panelSeleccion.hidden = false;
                panelResultado.hidden = true;
                window.scrollTo(0, 0);
            });

            actualizar();
        })();
    </script>
</asp:Content>
