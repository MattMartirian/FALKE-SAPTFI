<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Visualizaciones.aspx.cs" Inherits="GUI.Visualizaciones" Title="[[app_visualizaciones]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <link rel="stylesheet" href="<%: UrlVersionada("~/Content/visualizaciones.css") %>" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

<div class="vis">

    <nav class="vis-rail" aria-label="[[visualizaciones_sesiones_y_filtros]]">

        <h2 data-i18n="vis.rail.sesiones">[[categorias_sesiones]]</h2>

        <div class="vis-buscador campo-buscar">
            <label class="solo-lectores" for="visBuscar" data-i18n="vis.buscar">[[visualizaciones_buscar_por_nombre]]</label>
            <svg aria-hidden="true"><use href="#i-buscar" /></svg>
            <input type="search" class="entrada" id="visBuscar"
                   placeholder="Checkout, onboarding..." data-i18n-attr="placeholder:vis.buscar.placeholder" />
        </div>

        <div class="vis-chips chips" data-unico role="group" aria-label="[[visualizaciones_filtrar_por_categoria]]">
            <button type="button" class="chip" aria-pressed="true" data-categoria="todas" data-i18n="vis.cat.todas">[[empresas_todas]]</button>
            <button type="button" class="chip" aria-pressed="false" data-categoria="appweb" data-i18n="vis.cat.appweb">[[analisismultiple_app_web]]</button>
            <button type="button" class="chip" aria-pressed="false" data-categoria="software" data-i18n="vis.cat.software">[[analisismultiple_software]]</button>
            <button type="button" class="chip" aria-pressed="false" data-categoria="videojuego" data-i18n="vis.cat.videojuego">[[analisismultiple_videojuego]]</button>
            <button type="button" class="chip" aria-pressed="false" data-categoria="publicidad" data-i18n="vis.cat.publicidad">[[categorias_publicidad]]</button>
            <button type="button" class="chip" aria-pressed="false" data-categoria="appmovil" data-i18n="vis.cat.appmovil">[[analisismultiple_app_movil]]</button>
        </div>

        <div class="vis-filtros" role="group" aria-label="[[grabacion_tester_titulo]]">
            <select class="entrada" id="filtroEdad" aria-label="[[grabacion_tester_edad]]">
                <option value="">[[vis_filtro_edad]]</option>
                <asp:Repeater ID="rptFiltroEdad" runat="server"><ItemTemplate><option value='<%#: Eval("Valor") %>'><%#: Eval("Texto") %></option></ItemTemplate></asp:Repeater>
            </select>
            <select class="entrada" id="filtroGenero" aria-label="[[grabacion_tester_genero]]">
                <option value="">[[vis_filtro_genero]]</option>
                <asp:Repeater ID="rptFiltroGenero" runat="server"><ItemTemplate><option value='<%#: Eval("Valor") %>'><%#: Eval("Texto") %></option></ItemTemplate></asp:Repeater>
            </select>
            <select class="entrada" id="filtroFamiliaridad" aria-label="[[grabacion_tester_familiaridad]]">
                <option value="">[[vis_filtro_familiaridad]]</option>
                <asp:Repeater ID="rptFiltroFamiliaridad" runat="server"><ItemTemplate><option value='<%#: Eval("Valor") %>'><%#: Eval("Texto") %></option></ItemTemplate></asp:Repeater>
            </select>
        </div>

        <a class="btn btn-secundario btn-chico btn-bloque" href="<%: ResolveUrl("~/Categorias.aspx") %>">
            <svg width="16" height="16" aria-hidden="true"><use href="#i-mas" /></svg>
            <span data-i18n="vis.nuevaCategoria">[[categorias_nueva_categoria]]</span>
        </a>

        <h2 data-i18n="vis.rail.listado">[[visualizaciones_listado_de_sesiones]]</h2>

        <div class="vis-lista" id="listaSesiones">
            <%-- Una fila por sesion de la empresa del usuario (la arma el servidor). El token es al azar: no es el ID de la base. --%>
            <asp:Repeater ID="rptSesiones" runat="server">
                <ItemTemplate>
                    <button type="button" class="vis-sesion"
                            data-token='<%#: Eval("Token") %>' data-categoria='<%#: Eval("TipoClave") %>'
                            data-nombre='<%#: Eval("Nombre") %>' data-categoria-nombre='<%#: Eval("Categoria") %>' data-tipo='<%#: Eval("Tipo") %>'
                            data-fecha='<%#: Eval("Fecha") %>' data-duracion='<%#: Eval("Duracion") %>' data-duracion-seg='<%#: Eval("DuracionSegundos") %>'
                            data-analista='<%#: Eval("Analista") %>' data-objetivo='<%#: Eval("Objetivo") %>' data-muestras='<%#: Eval("Muestras") %>'
                            data-edad='<%#: Eval("Edad") %>' data-genero='<%#: Eval("Genero") %>' data-familiaridad='<%#: Eval("Familiaridad") %>'
                            data-perfil='<%#: Eval("Perfil") %>' data-desfase='<%#: Eval("DesfaseMs") %>'>
                        <span class="nombre"><%#: Eval("Nombre") %></span>
                        <span class="categoria"><%#: Eval("Categoria") %></span>
                        <span class="badge badge-exito">[[panel_finalizado]]</span>
                        <span class="meta"><%#: Eval("Fecha") %> &middot; <%#: Eval("Duracion") %></span>
                    </button>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="vacio" id="sesionesVacio" hidden>
            <svg width="28" height="28" aria-hidden="true"><use href="#i-carpeta" /></svg>
            <p class="sin-margen" data-i18n="vis.sinSesiones">[[visualizaciones_no_hay_sesiones_que_coincidan_con]]</p>
        </div>

    </nav>

    <div class="vis-main">

        <% if (HayError) { %>
        <div class="aviso aviso-peligro">
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen">[[vis_error_listado]]</p>
        </div>
        <% } %>

        <div class="vacio" id="vacioMain">
            <svg width="28" height="28" aria-hidden="true"><use href="#i-video" /></svg>
            <p class="sin-margen"><%: Traducir(HaySesiones ? "vis_elige_sesion" : "vis_sin_sesiones_empresa") %></p>
            <% if (!HaySesiones) { %>
            <a class="btn btn-primario" href="<%: ResolveUrl("~/NuevaGrabacion.aspx") %>">[[app_nueva_grabacion]]</a>
            <% } %>
        </div>

        <div id="detalle" hidden>

        <p class="migas">
            <span data-i18n="vis.rail.sesiones">[[categorias_sesiones]]</span>
            <span aria-hidden="true">/</span>
            <strong id="migaSesion"></strong>
        </p>

        <div class="pagina-cabecera">
            <div>
                <h1 id="tituloSesion" tabindex="-1"></h1>
                <p class="texto-suave sin-margen" id="subtituloSesion"></p>
                <p class="texto-suave sin-margen" id="perfilSesion"></p>
                <p class="texto-suave sin-margen" id="objetivoSesion" hidden></p>
            </div>
            <div class="acciones">
                <button type="button" class="btn btn-primario" id="btnAbrirExportar" data-abre-modal="modalExportar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-descarga" /></svg>
                    <span data-i18n="vis.exportar">[[analisismultiple_descargar_reporte]]</span>
                </button>
            </div>
        </div>

        <div class="aviso aviso-alerta" id="avisoSinMirada" hidden>
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen">[[vis_sin_mirada]]</p>
        </div>

        <div class="aviso aviso-peligro" id="avisoErrorSesion" hidden>
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen">[[vis_error_cargar]]</p>
        </div>

        <div class="tablero" id="tablero">

            <div class="visual destacada">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-metricas" /></svg>
                    <span data-i18n="vis.metricas.titulo">[[visualizaciones_metricas_generales]]</span>
                </span>
                <div class="metricas">
                    <div class="metrica" tabindex="0" data-ayuda="[[vis_ayuda_duracion]]">
                        <div class="metrica-valor" id="mDuracion">-</div>
                        <div class="metrica-nombre" data-i18n="vis.metrica.duracion">[[visualizaciones_duracion_total]]</div>
                    </div>
                    <div class="metrica" tabindex="0" data-ayuda="[[vis_ayuda_fijacion_media]]">
                        <div class="metrica-valor" id="mFijacionMedia">-</div>
                        <div class="metrica-nombre" data-i18n="vis.metrica.fijacionMedia">[[analisismultiple_fijacion_media]]</div>
                    </div>
                    <div class="metrica" tabindex="0" data-ayuda="[[vis_ayuda_fijaciones]]">
                        <div class="metrica-valor" id="mFijaciones">-</div>
                        <div class="metrica-nombre" data-i18n="vis.metrica.fijaciones">[[visualizaciones_fijaciones]]</div>
                    </div>
                    <div class="metrica" tabindex="0" data-ayuda="[[vis_ayuda_atencion]]">
                        <div class="metrica-valor" id="mAtencion">-</div>
                        <div class="metrica-nombre" data-i18n="vis.metrica.atencion">[[analisismultiple_atencion_activa]]</div>
                    </div>
                    <div class="metrica" tabindex="0" data-ayuda="[[vis_ayuda_sacadico]]">
                        <div class="metrica-valor" id="mSacadico">-</div>
                        <div class="metrica-nombre" data-i18n="vis.metrica.sacadico">[[visualizaciones_movimiento_sacadico]]</div>
                    </div>
                </div>
            </div>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="grabacion">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-video" /></svg>
                    <span data-i18n="vis.grafico.grabacion">[[visualizaciones_grabacion_con_seguimiento]]</span>
                </span>
                <span class="grafico grafico-video" aria-hidden="true">
                    <span class="play"><svg width="22" height="22"><use href="#i-play" /></svg></span>
                </span>
                <span class="ver">
                    <span data-i18n="comun.verDetalle">[[dispositivos_ver_detalle]]</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="calor">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-calor" /></svg>
                    <span data-i18n="vis.grafico.calor">[[analisismultiple_mapa_de_calor]]</span>
                </span>
                <span class="grafico grafico-calor" aria-hidden="true"></span>
                <span class="ver">
                    <span data-i18n="comun.verDetalle">[[dispositivos_ver_detalle]]</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="zonas">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-zonas" /></svg>
                    <span data-i18n="vis.grafico.zonas">[[analisismultiple_zonas_de_interes_y_ciegas]]</span>
                </span>
                <span class="grafico" aria-hidden="true">
                    <svg viewBox="0 0 320 180" preserveAspectRatio="none">
                        <rect class="zona-interes" x="30" y="26" width="120" height="62" rx="6" />
                        <rect class="zona-interes" x="170" y="60" width="90" height="46" rx="6" />
                        <rect class="zona-ciega" x="42" y="112" width="86" height="44" rx="6" />
                        <rect class="zona-ciega" x="212" y="122" width="74" height="36" rx="6" />
                    </svg>
                </span>
                <span class="ver">
                    <span data-i18n="comun.verDetalle">[[dispositivos_ver_detalle]]</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="dispersion">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-dispersion" /></svg>
                    <span data-i18n="vis.grafico.dispersion">[[analisismultiple_dispersion_de_mirada]]</span>
                </span>
                <span class="grafico" aria-hidden="true">
                    <svg viewBox="0 0 320 180" preserveAspectRatio="none">
                        <ellipse cx="158" cy="88" rx="86" ry="48" fill="rgba(232,175,82,.16)" stroke="#E8AF52" stroke-width="2" stroke-dasharray="5 4" />
                        <path class="punto-fijo" d="M120 66h0" stroke-width="8" stroke="#56637C" />
                        <path class="punto-fijo" d="M168 58h0" stroke-width="8" stroke="#56637C" />
                        <path class="punto-fijo" d="M196 92h0" stroke-width="8" stroke="#56637C" />
                        <path class="punto-fijo" d="M142 104h0" stroke-width="8" stroke="#56637C" />
                        <path class="punto-fijo" d="M104 98h0" stroke-width="8" stroke="#56637C" />
                        <path class="punto-fijo" d="M182 120h0" stroke-width="8" stroke="#56637C" />
                        <path class="punto-fijo" d="M150 82h0" stroke-width="8" stroke="#56637C" />
                        <path class="punto-fijo" d="M158 88h0" stroke-width="10" stroke="#F26157" />
                    </svg>
                </span>
                <span class="ver">
                    <span data-i18n="comun.verDetalle">[[dispositivos_ver_detalle]]</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="atencion">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-torta" /></svg>
                    <span data-i18n="vis.grafico.atencion">[[visualizaciones_atencion_frente_a_distraccion]]</span>
                </span>
                <span class="grafico" aria-hidden="true" style="display:flex;align-items:center;justify-content:center">
                    <svg viewBox="0 0 120 120" style="width:auto;height:82%">
                        <circle cx="60" cy="60" r="44" fill="none" stroke="#0E1A34" stroke-width="18" />
                        <circle cx="60" cy="60" r="44" fill="none" stroke="#E8AF52" stroke-width="18"
                                stroke-dasharray="171 276" transform="rotate(-90 60 60)" />
                        <text x="60" y="66" text-anchor="middle" font-size="20" font-weight="700" fill="currentColor">62%</text>
                    </svg>
                </span>
                <span class="ver">
                    <span data-i18n="comun.verDetalle">[[dispositivos_ver_detalle]]</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="recorrido">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-recorrido" /></svg>
                    <span data-i18n="vis.grafico.recorrido">[[analisismultiple_recorrido_de_mirada]]</span>
                </span>
                <span class="grafico" aria-hidden="true">
                    <svg viewBox="0 0 320 180" preserveAspectRatio="none">
                        <path class="linea-recorrido" d="M40 132 C 88 40, 132 150, 176 74 S 250 44, 284 96" />
                        <path class="punto-fijo" d="M40 132h0" stroke-width="10" stroke="#56637C" />
                        <path class="punto-fijo" d="M176 74h0" stroke-width="10" stroke="#0E1A34" />
                        <path class="punto-fijo" d="M284 96h0" stroke-width="10" stroke="#F26157" />
                    </svg>
                </span>
                <span class="ver">
                    <span data-i18n="comun.verDetalle">[[dispositivos_ver_detalle]]</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

        </div>

        </div>

    </div>
</div>

<div class="modal-fondo" id="modalVisor" role="dialog" aria-modal="true" aria-labelledby="visorTitulo" hidden>
    <div class="modal extra">

        <div class="modal-cabecera">
            <div>
                <h2 id="visorTitulo" data-i18n="vis.visor.titulo">[[visualizaciones_grabacion_con_seguimiento]]</h2>
                <p class="subtitulo sin-margen" id="visorSubtitulo"></p>
            </div>
            <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
            </button>
        </div>

        <div class="modal-cuerpo">

            <div class="visor-lienzo" id="visorLienzo"></div>

            <div class="franja-caja" id="franjaCaja" hidden>
                <canvas id="franja" width="960" height="36" aria-hidden="true"></canvas>
            </div>

            <div id="calorOpciones" hidden>
                <div class="calor-tramo" role="group" aria-labelledby="calorTramoTitulo">
                    <p class="calor-tramo-titulo" id="calorTramoTitulo">[[vis_calor_tramo]]</p>

                    <label for="calorDesde">[[vis_calor_desde]]</label>
                    <input type="range" id="calorDesde" min="0" max="1000" value="0" />
                    <span class="visor-tiempo" id="calorDesdeTxt">00:00</span>

                    <label for="calorHasta">[[vis_calor_hasta]]</label>
                    <input type="range" id="calorHasta" min="0" max="1000" value="1000" />
                    <span class="visor-tiempo" id="calorHastaTxt">00:00</span>
                </div>

                <label class="calor-tiempo-real" id="calorTiempoRealCaja">
                    <input type="checkbox" id="calorTiempoReal" />
                    <span>[[vis_calor_tiempo_real]]</span>
                </label>

                <label class="calor-tiempo-real" id="calorMiradaCaja">
                    <input type="checkbox" id="calorMirada" />
                    <span>[[vis_mirada_tiempo_real]]</span>
                </label>
            </div>

            <div class="visor-controles" id="visorControles">
                <div class="visor-botones">
                    <button type="button" class="icono-boton" id="btnAtras" aria-label="[[visualizaciones_retroceder_diez_segundos]]">
                        <svg width="20" height="20" aria-hidden="true"><use href="#i-atras" /></svg>
                    </button>
                    <button type="button" class="icono-boton principal" id="btnReproducir" aria-label="[[visualizaciones_reproducir]]">
                        <svg width="20" height="20" aria-hidden="true" data-icono-play><use href="#i-play" /></svg>
                        <svg width="20" height="20" aria-hidden="true" data-icono-pausa style="display:none"><use href="#i-pausa" /></svg>
                    </button>
                    <button type="button" class="icono-boton" id="btnAdelante" aria-label="[[visualizaciones_adelantar_diez_segundos]]">
                        <svg width="20" height="20" aria-hidden="true"><use href="#i-adelante" /></svg>
                    </button>
                </div>

                <span class="visor-tiempo" id="visorTiempo">00:00 / 00:00</span>

                <label class="solo-lectores" for="visorLinea" data-i18n="vis.visor.linea">[[visualizaciones_posicion_en_la_grabacion]]</label>
                <input type="range" class="linea-tiempo" id="visorLinea" min="0" max="1000" value="0" />

                <button type="button" class="btn btn-secundario btn-chico" data-cierra-modal data-i18n="comun.salir">[[visualizaciones_salir]]</button>
            </div>

            <div class="leyenda" id="visorLeyenda"></div>

            <%-- esto es para la info: aviso "Muevete por la linea de tiempo..." sacado porque ya no da info importante
            <div class="aviso aviso-info mt-16 mb-0">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
                <p class="sin-margen" id="visorAyuda" data-i18n="vis.visor.ayuda">
                    [[visualizaciones_muevete_por_la_linea_de_tiempo]]
                </p>
            </div>
            --%>

        </div>
    </div>
</div>

<div class="modal-fondo" id="modalExportar" role="dialog" aria-modal="true" aria-labelledby="exportarTitulo" hidden>
    <div class="modal ancho">

        <div class="modal-cabecera">
            <div>
                <h2 id="exportarTitulo" data-i18n="vis.exportar.titulo">[[visualizaciones_exportar_analisis]]</h2>
                <p class="subtitulo sin-margen" data-i18n="vis.exportar.subtitulo">
                    [[visualizaciones_elige_que_incluir_en_el_reporte]]
                </p>
            </div>
            <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
            </button>
        </div>

        <div class="modal-cuerpo">

            <div class="campo exportar-idioma">
                <label for="expIdioma">[[vis_exportar_idioma]]</label>
                <%-- por defecto el idioma del usuario; el informe puede salir en cualquier idioma activo --%>
                <select class="entrada" id="expIdioma">
                    <asp:Repeater ID="rptIdiomasInforme" runat="server">
                        <ItemTemplate><option value='<%#: Eval("CodigoIdioma") %>'<%# IdiomaElegido((string)Eval("CodigoIdioma")) %>><%#: Eval("NombreIdioma") %></option></ItemTemplate>
                    </asp:Repeater>
                </select>
            </div>

            <p class="etiqueta mt-16" data-i18n="vis.exportar.contenido">[[visualizaciones_contenido_del_reporte]]</p>

            <div class="exportar-opciones">
                <label class="opcion">
                    <input type="checkbox" checked disabled />
                    <span>
                        <span class="opcion-titulo">[[vis_exportar_obligatorio]]</span>
                        <span class="opcion-detalle" data-i18n="vis.exportar.portada.detalle">[[visualizaciones_siempre_se_incluye_en_el_reporte]]</span>
                    </span>
                </label>

                <label class="opcion">
                    <input type="checkbox" name="expDato" value="activo" checked />
                    <span>
                        <span class="opcion-titulo">[[vis_exportar_activo]]</span>
                        <span class="opcion-detalle">[[vis_exportar_activo_detalle]]</span>
                    </span>
                </label>

                <label class="opcion">
                    <input type="checkbox" name="expDato" value="perfil" checked />
                    <span>
                        <span class="opcion-titulo">[[vis_exportar_perfil]]</span>
                        <span class="opcion-detalle">[[vis_exportar_perfil_detalle]]</span>
                    </span>
                </label>

                <label class="opcion">
                    <input type="checkbox" name="expDato" value="objetivo" checked />
                    <span>
                        <span class="opcion-titulo">[[vis_exportar_objetivo]]</span>
                        <span class="opcion-detalle">[[vis_exportar_objetivo_detalle]]</span>
                    </span>
                </label>

                <label class="opcion">
                    <input type="checkbox" name="expDato" value="dispositivo" checked />
                    <span>
                        <span class="opcion-titulo">[[vis_exportar_dispositivo]]</span>
                        <span class="opcion-detalle">[[vis_exportar_dispositivo_detalle]]</span>
                    </span>
                </label>

                <label class="opcion">
                    <input type="checkbox" name="expDato" value="definiciones" checked />
                    <span>
                        <span class="opcion-titulo">[[vis_exportar_definiciones]]</span>
                        <span class="opcion-detalle">[[vis_exportar_definiciones_detalle]]</span>
                    </span>
                </label>
            </div>

            <p class="etiqueta mt-16" data-i18n="vis.exportar.visualizaciones">[[visualizaciones_visualizaciones_a_incluir]]</p>

            <div class="exportar-opciones">
                <label class="opcion">
                    <input type="checkbox" name="expVisual" value="calor" checked />
                    <span>
                        <span class="opcion-titulo" data-i18n="vis.grafico.calor">[[analisismultiple_mapa_de_calor]]</span>
                        <span class="opcion-detalle" data-i18n="vis.exportar.calor.detalle">[[visualizaciones_concentracion_de_la_atencion_visual]]</span>
                    </span>
                </label>

                <label class="opcion">
                    <input type="checkbox" name="expVisual" value="zonas" checked />
                    <span>
                        <span class="opcion-titulo" data-i18n="vis.grafico.zonas">[[analisismultiple_zonas_de_interes_y_ciegas]]</span>
                        <span class="opcion-detalle" data-i18n="vis.exportar.zonas.detalle">[[visualizaciones_areas_miradas_y_areas_ignoradas]]</span>
                    </span>
                </label>

                <label class="opcion">
                    <input type="checkbox" name="expVisual" value="dispersion" />
                    <span>
                        <span class="opcion-titulo" data-i18n="vis.grafico.dispersion">[[analisismultiple_dispersion_de_mirada]]</span>
                        <span class="opcion-detalle" data-i18n="vis.exportar.dispersion.detalle">[[visualizaciones_que_tan_estable_fue_la_atencion]]</span>
                    </span>
                </label>

                <label class="opcion">
                    <input type="checkbox" name="expVisual" value="atencion" />
                    <span>
                        <span class="opcion-titulo" data-i18n="vis.grafico.atencion">[[visualizaciones_atencion_frente_a_distraccion]]</span>
                        <span class="opcion-detalle" data-i18n="vis.exportar.atencion.detalle">[[visualizaciones_fijacion_activa_frente_a_movimiento_saca]]</span>
                    </span>
                </label>

                <label class="opcion">
                    <input type="checkbox" name="expVisual" value="recorrido" checked />
                    <span>
                        <span class="opcion-titulo" data-i18n="vis.grafico.recorrido">[[analisismultiple_recorrido_de_mirada]]</span>
                        <span class="opcion-detalle" data-i18n="vis.exportar.recorrido.detalle">[[visualizaciones_trayecto_secuencial_sobre_el_activo]]</span>
                    </span>
                </label>
            </div>

            <p class="ayuda" id="exportarAviso" role="status"></p>

        </div>

        <div class="modal-pie">
            <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">[[categorias_cancelar]]</button>
            <button type="button" class="btn btn-primario" id="btnExportar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-descarga" /></svg>
                <span data-i18n="vis.exportar.boton">[[visualizaciones_exportar_reporte]]</span>
            </button>
        </div>

    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        // textos traducidos para los .js de las visualizaciones (un .js externo no puede leer los [[...]] del servidor)
        var VisTextos = {
            errorCargar: "[[js:vis_error_cargar]]",
            sinDatos: "[[js:vis_calor_sin_datos]]",
            pausar: "[[js:vis_pausar]]",
            reproducir: "[[js:visualizaciones_reproducir]]",
            grabacionTitulo: "[[js:visualizaciones_grabacion_con_seguimiento]]",
            grabacionLeyenda: '<span><i style="background:#F26157"></i>[[js:visualizaciones_punto_de_mirada_actual]]</span>',
            calorTitulo: "[[js:analisismultiple_mapa_de_calor]]",
            calorLeyenda: '<span><i style="background:#F26157"></i>[[js:visualizaciones_mayor_concentracion]]</span>' +
                          '<span><i style="background:#E8AF52"></i>[[js:visualizaciones_concentracion_media]]</span>' +
                          '<span><i style="background:#56637C"></i>[[js:visualizaciones_menor_concentracion]]</span>',
            zonasTitulo: "[[js:visualizaciones_zonas_de_interes_y_zonas_ciegas]]",
            dispersionTitulo: "[[js:analisismultiple_dispersion_de_mirada]]",
            dispersionLeyenda: '<span><i style="background:#56637C"></i>[[js:visualizaciones_punto_de_mirada]]</span>' +
                               '<span><i style="background:#F26157"></i>[[js:visualizaciones_centroide]]</span>' +
                               '<span><i style="background:transparent;border:1px dashed #E8AF52"></i>[[js:visualizaciones_elipse_de_dispersion]]</span>',
            recorridoTiempoReal: "[[js:vis_recorrido_tiempo_real]]",
            recorridoTitulo: "[[js:analisismultiple_recorrido_de_mirada]]",
            recorridoLeyenda: '<span><i style="background:#56637C"></i>[[js:default_inicio]]</span>' +
                              '<span><i style="background:#F26157"></i>[[js:visualizaciones_final]]</span>',
            atencionTitulo: "[[js:visualizaciones_atencion_frente_a_distraccion]]",
            atencionActiva: "[[js:vis_fijacion_activa_n]]",
            atencionSacadico: "[[js:vis_sacadico_n]]",
            atencionSinDatos: "[[js:vis_atencion_sin_datos]]",
            zonasLeyenda: '<span><i style="background:rgba(232,175,82,.5);border:1px solid #E8AF52"></i>[[js:visualizaciones_zona_de_interes]]</span>' +
                          '<span><i style="background:transparent;border:1px dashed #56637C"></i>[[js:visualizaciones_zona_ciega]]</span>'
        };
    </script>

    <%-- el orden importa: cada archivo usa lo que define el anterior --%>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/analisis.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/comun.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/visualizacion.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/visualizacion-grabacion.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/visualizacion-tramo.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/visualizacion-calor.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/visualizacion-zonas.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/visualizacion-dispersion.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/visualizacion-atencion.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/visualizacion-recorrido.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/visualizaciones/visor.js") %>"></script>

    <script src="<%: UrlVersionada("~/Scripts/lib/jspdf.umd.min.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/informe/informe-fuentes.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/informe/informe-imagenes.js") %>"></script>
    <script src="<%: UrlVersionada("~/Scripts/informe/informe-pdf.js") %>"></script>

    <script>
        (function () {
            "use strict";
            // COMO FUNCIONA ESTA PAGINA
            // - La lista de sesiones la arma el backend con las de la empresa del usuario logueado.
            // - Al elegir una sesion se pide el JSON a GrabacionArchivo.ashx
            // - El video tambien sale de GrabacionArchivo.ashx (con Range, para poder saltar).
            // - Las visualizaciones (video, mapa de calor, zonas...) son clases en Scripts/visualizaciones/; aca solo se conectan.

            var URL_ARCHIVO = "<%: ResolveUrl("~/GrabacionArchivo.ashx") %>";
            var COLOR_CORAL = "#F26157";
            var COLOR_AMBAR = "#E8AF52";
            var COLOR_GRIS = "#56637C";

            var chips = document.querySelectorAll(".vis-chips .chip");
            var sesiones = document.querySelectorAll(".vis-sesion");
            var buscar = document.getElementById("visBuscar");
            var filtroEdad = document.getElementById("filtroEdad");
            var filtroGenero = document.getElementById("filtroGenero");
            var filtroFamiliaridad = document.getElementById("filtroFamiliaridad");
            var vacio = document.getElementById("sesionesVacio");
            var categoriaActiva = "todas";

            var vacioMain = document.getElementById("vacioMain");
            var detalle = document.getElementById("detalle");
            var tituloSesion = document.getElementById("tituloSesion");
            var migaSesion = document.getElementById("migaSesion");
            var subtituloSesion = document.getElementById("subtituloSesion");
            var perfilSesion = document.getElementById("perfilSesion");
            var objetivoSesion = document.getElementById("objetivoSesion");
            var avisoSinMirada = document.getElementById("avisoSinMirada");
            var avisoErrorSesion = document.getElementById("avisoErrorSesion");
            var visorSubtitulo = document.getElementById("visorSubtitulo");

            var actual = null;   
            var cargaN = 0;      // numero de la ultima carga, para ignorar respuestas viejas si el usuario cambia rapido

            function normalizar(texto) {
                return window.Falke && window.Falke.normalizar ? window.Falke.normalizar(texto) : String(texto || "").toLowerCase().trim();
            }

            function urlArchivo(token, tipo) {
                return URL_ARCHIVO + "?t=" + encodeURIComponent(token) + "&k=" + tipo;
            }

            function formatoNumero(n) {
                try {
                    return new Intl.NumberFormat(document.documentElement.lang || undefined, { maximumFractionDigits: 1 }).format(n);
                } catch (e) {
                    return String(Math.round(n * 10) / 10);
                }
            }

            function filtrar() {
                var texto = normalizar(buscar.value);
                var visibles = 0;

                for (var i = 0; i < sesiones.length; i++) {
                    var s = sesiones[i];
                    var okCategoria = categoriaActiva === "todas" || s.getAttribute("data-categoria") === categoriaActiva;
                    var okTexto = texto === "" || normalizar(s.getAttribute("data-nombre") + " " + s.getAttribute("data-categoria-nombre")).indexOf(texto) !== -1;
                    var okEdad = !filtroEdad.value || s.getAttribute("data-edad") === filtroEdad.value;
                    var okGenero = !filtroGenero.value || s.getAttribute("data-genero") === filtroGenero.value;
                    var okFam = !filtroFamiliaridad.value || s.getAttribute("data-familiaridad") === filtroFamiliaridad.value;
                    var mostrar = okCategoria && okTexto && okEdad && okGenero && okFam;

                    s.hidden = !mostrar;

                    if (mostrar) visibles++;
                }

                vacio.hidden = visibles > 0 || sesiones.length === 0;
            }

            for (var i = 0; i < chips.length; i++) {
                chips[i].addEventListener("click", function () {
                    for (var j = 0; j < chips.length; j++) {
                        chips[j].setAttribute("aria-pressed", chips[j] === this ? "true" : "false");
                    }
                    categoriaActiva = this.getAttribute("data-categoria");
                    filtrar();
                });
            }

            buscar.addEventListener("input", filtrar);
            filtroEdad.addEventListener("change", filtrar);
            filtroGenero.addEventListener("change", filtrar);
            filtroFamiliaridad.addEventListener("change", filtrar);


            function limpiarTablero() {
                var ids = ["mFijacionMedia", "mFijaciones", "mAtencion", "mSacadico"];

                for (var i = 0; i < ids.length; i++) document.getElementById(ids[i]).textContent = "-";
            }

            function pintarTablero() {
                var a = actual.analisis;

                document.getElementById("mFijacionMedia").textContent = formatoNumero(a.fijMedia) + " s";
                document.getElementById("mFijaciones").textContent = a.fijs.length;
                document.getElementById("mAtencion").textContent = a.atencion + "%";
                document.getElementById("mSacadico").textContent = a.sacadico + "%";

                // si el visor ya estaba abierto cuando llegaron los datos, se vuelve a dibujar
                Visor.refrescar();
            }

            function elegirSesion(boton) {
                for (var k = 0; k < sesiones.length; k++) sesiones[k].removeAttribute("aria-current");

                boton.setAttribute("aria-current", "true");

                var nombre = boton.getAttribute("data-nombre");
                var objetivo = boton.getAttribute("data-objetivo");
                var n = ++cargaN;

                actual = {
                    token: boton.getAttribute("data-token"),
                    urlVideo: urlArchivo(boton.getAttribute("data-token"), "video"),
                    nombre: nombre,
                    fecha: boton.getAttribute("data-fecha"),
                    duracionSeg: Number(boton.getAttribute("data-duracion-seg")) || 0,
                    desfase: Number(boton.getAttribute("data-desfase")) || 0,
                    sinMirada: Number(boton.getAttribute("data-muestras")) === 0,
                    analisis: null
                };

                vacioMain.hidden = true;
                detalle.hidden = false;
                avisoSinMirada.hidden = true;
                avisoErrorSesion.hidden = true;

                tituloSesion.textContent = nombre;
                migaSesion.textContent = nombre;
                subtituloSesion.textContent = boton.getAttribute("data-fecha") + " · " +
                    boton.getAttribute("data-duracion") + " [[js:visualizaciones_min_analista]] " +
                    boton.getAttribute("data-analista") + " · " + boton.getAttribute("data-categoria-nombre") + " · " + boton.getAttribute("data-tipo");
                perfilSesion.textContent = boton.getAttribute("data-perfil");
                objetivoSesion.textContent = objetivo ? "[[js:vis_objetivo]]".replace("{0}", objetivo) : "";
                objetivoSesion.hidden = !objetivo;
                visorSubtitulo.textContent = nombre + " · " + boton.getAttribute("data-fecha");
                document.getElementById("mDuracion").textContent = boton.getAttribute("data-duracion");

                limpiarTablero();

                tituloSesion.focus();
                if (window.innerWidth <= 940) window.scrollTo(0, tituloSesion.offsetTop - 70);

                if (Number(boton.getAttribute("data-muestras")) === 0) {
                    avisoSinMirada.hidden = false;
                    return;
                }

                fetch(urlArchivo(actual.token, "mirada"), { credentials: "same-origin", cache: "no-store" })
                    .then(function (respuesta) {
                        if (!respuesta.ok) throw new Error();

                        return respuesta.json();
                    })
                    .then(function (json) {
                        if (n !== cargaN) return;

                        actual.analisis = analizar(json, actual.desfase);
                        pintarTablero();
                    })
                    .catch(function () {
                        if (n === cargaN) avisoErrorSesion.hidden = false;
                    });
            }

            for (i = 0; i < sesiones.length; i++) {
                sesiones[i].addEventListener("click", function () { elegirSesion(this); });
            }

            Visor.tipos.grabacion = new VisualizacionGrabacion();
            Visor.tipos.calor = new VisualizacionCalor();
            Visor.tipos.zonas = new VisualizacionZonas();
            Visor.tipos.dispersion = new VisualizacionDispersion();
            Visor.tipos.atencion = new VisualizacionAtencion();
            Visor.tipos.recorrido = new VisualizacionRecorrido();
            Visor.iniciar();

            var botonesVisual = document.querySelectorAll("[data-visual]");

            for (i = 0; i < botonesVisual.length; i++) {
                botonesVisual[i].addEventListener("click", function () { Visor.abrir(this.getAttribute("data-visual"), actual); });
            }

            var URL_INFORME = "<%: ResolveUrl("~/Visualizaciones_WebMethod.asmx") %>/DatosInforme";
            var URL_LOGO = "<%: ResolveUrl("~/Content/img/falke-logo.svg") %>";
            var btnExportar = document.getElementById("btnExportar");
            var exportarAviso = document.getElementById("exportarAviso");
            var expIdioma = document.getElementById("expIdioma");

            function avisoExportar(clase, texto) {
                exportarAviso.className = "ayuda " + clase;
                exportarAviso.textContent = texto;
            }

            function marcadas(nombre) {
                var lista = [];
                var casillas = document.querySelectorAll("input[name='" + nombre + "']:checked");

                for (var i = 0; i < casillas.length; i++) lista.push(casillas[i].value);

                return lista;
            }

            function pedirDatosInforme(token, idioma) {
                return fetch(URL_INFORME, {
                    method: "POST",
                    credentials: "same-origin",
                    headers: { "Content-Type": "application/json; charset=utf-8" },
                    body: JSON.stringify({ token: token, idioma: idioma })
                })
                    .then(function (respuesta) {
                        if (!respuesta.ok) throw new Error();

                        return respuesta.json();
                    })
                    .then(function (json) {
                        if (!json.d || !json.d.ok) {
                            var error = new Error(json.d && json.d.mensaje);
                            error.delServidor = true;
                            throw error;
                        }

                        return json.d;
                    });
            }

            document.getElementById("btnAbrirExportar").addEventListener("click", function () {
                if (!btnExportar.disabled) avisoExportar("", "");
            });

            btnExportar.addEventListener("click", function () {
                var visuales = marcadas("expVisual");
                var datos = marcadas("expDato");

                if (!actual) return;

                if (visuales.length === 0) {
                    avisoExportar("texto-peligro", "[[js:visualizaciones_elige_al_menos_una_visualizacion_para]]");
                    return;
                }

                if (!actual.analisis && !actual.sinMirada) {
                    avisoExportar("texto-peligro", "[[js:vis_exportar_esperar]]");
                    return;
                }

                var sesion = actual;
                var segundo = sesion.duracionSeg / 2;
                var datosInforme = null;

                btnExportar.disabled = true;
                avisoExportar("texto-suave", "[[js:vis_exportar_generando]]");

                pedirDatosInforme(sesion.token, expIdioma.value)
                    .then(function (respuesta) {
                        datosInforme = respuesta;

                        return InformeImagenes.logo(URL_LOGO);
                    })
                    .then(function (logo) {
                        var informe = new InformePdf(datosInforme, logo);

                        informe.armar({
                            sesion: sesion,
                            analisis: sesion.analisis,
                            visuales: visuales,
                            incluir: {
                                activo: datos.indexOf("activo") !== -1,
                                perfil: datos.indexOf("perfil") !== -1,
                                objetivo: datos.indexOf("objetivo") !== -1,
                                dispositivo: datos.indexOf("dispositivo") !== -1,
                                definiciones: datos.indexOf("definiciones") !== -1
                            },
                            tipos: Visor.tipos,
                            segundo: segundo
                        });

                        informe.guardar();

                        avisoExportar("texto-exito", "[[js:vis_exportar_listo]]");
                    })
                    .catch(function (error) {
                        if (window.console) console.error(error);

                        avisoExportar("texto-peligro", error && error.delServidor && error.message ? error.message : "[[js:vis_exportar_error]]");
                    })
                    .then(function () {
                        btnExportar.disabled = false;
                    });
            });
        })();
    </script>
</asp:Content>
