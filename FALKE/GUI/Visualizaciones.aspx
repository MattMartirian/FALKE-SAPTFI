<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Visualizaciones.aspx.cs" Inherits="GUI.Visualizaciones" Title="Visualizaciones" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        .vis
        {
            display: grid;
            grid-template-columns: 268px minmax(0, 1fr);
            min-height: calc(100vh - var(--alto-topbar));
            margin: -26px -28px -60px;
        }

        .vis-rail
        {
            border-right: 1px solid var(--borde);
            background: var(--superficie);
            padding: 18px 14px;
            position: sticky;
            top: var(--alto-topbar);
            align-self: start;
            max-height: calc(100vh - var(--alto-topbar));
            overflow-y: auto;
            display: flex;
            flex-direction: column;
            gap: 14px;
        }

        .vis-rail h2
        {
            font-size: .84rem;
            font-weight: 600;
            color: var(--texto-suave);
            margin: 0;
        }

        .vis-buscador
        {
            position: relative;
        }

        .vis-chips
        {
            display: flex;
            flex-wrap: wrap;
            gap: 6px;
        }

        .vis-lista
        {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .vis-sesion
        {
            display: block;
            width: 100%;
            text-align: left;
            padding: 11px 12px;
            border: 1px solid var(--borde);
            border-radius: var(--radio);
            background: var(--superficie);
            color: var(--texto);
            font-family: inherit;
            cursor: pointer;
        }

        .vis-sesion:hover
        {
            border-color: var(--borde-fuerte);
            background: var(--superficie-2);
        }

        .vis-sesion[aria-current="true"]
        {
            border-color: var(--acento);
            background: var(--acento-suave);
        }

        .vis-sesion .nombre
        {
            display: block;
            font-weight: 600;
            font-size: .9rem;
            margin-bottom: 5px;
        }

        .vis-sesion .meta
        {
            display: block;
            font-family: var(--fuente-mono);
            font-size: .7rem;
            color: var(--texto-suave);
            margin-top: 5px;
        }

        .badge-vivo
        {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-family: var(--fuente-mono);
            font-size: .64rem;
            letter-spacing: .08em;
            text-transform: uppercase;
            color: var(--marca-coral);
        }

        .badge-vivo::before
        {
            content: "";
            width: 6px;
            height: 6px;
            border-radius: 50%;
            background: var(--marca-coral);
            animation: latir 1.4s ease-in-out infinite;
        }

        @keyframes latir
        {
            0%, 100% { opacity: 1; }
            50%      { opacity: .3; }
        }

        .vis-main
        {
            padding: 24px 26px 20px;
            min-width: 0;
        }

        .vis-main
        {
            container-type: inline-size;
        }

        .tablero
        {
            --alto-visual: clamp(200px, calc((100vh - 370px) / 2), 280px);
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            grid-template-rows: auto;
            grid-auto-rows: var(--alto-visual);
            gap: 16px;
        }

        @container (max-width: 700px)
        {
            .tablero { grid-template-columns: repeat(2, minmax(0, 1fr)); }
        }

        @container (max-width: 440px)
        {
            .tablero { grid-template-columns: minmax(0, 1fr); }
        }

        .visual
        {
            display: flex;
            flex-direction: column;
            width: 100%;
            text-align: left;
            padding: 16px;
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
            color: var(--texto);
            font-family: inherit;
            cursor: pointer;
            transition: border-color .15s ease;
        }

        .visual:hover
        {
            border-color: var(--acento);
        }

        .visual.destacada
        {
            grid-column: 1 / -1;
            cursor: default;
        }

        .visual.destacada:hover
        {
            border-color: var(--borde);
        }

        .visual .rotulo
        {
            display: flex;
            align-items: center;
            gap: 8px;
            font-weight: 600;
            font-size: .9rem;
            margin-bottom: 12px;
        }

        .visual .rotulo svg
        {
            color: var(--texto-suave);
        }

        .visual:not(.destacada)
        {
            min-height: 0;
        }

        .visual .grafico
        {
            flex: 1;
            min-height: 0;
            aspect-ratio: auto;
        }

        .visual .grafico svg .zona-interes,
        .visual .grafico svg .zona-ciega,
        .visual .grafico svg .linea-recorrido,
        .visual .grafico svg ellipse
        {
            vector-effect: non-scaling-stroke;
        }

        .punto-fijo
        {
            stroke-linecap: round;
            vector-effect: non-scaling-stroke;
        }

        .visual .ver
        {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            margin-top: 12px;
            font-size: .82rem;
            font-weight: 600;
            color: var(--acento-texto);
        }

        .visor-lienzo
        {
            aspect-ratio: 16 / 9;
            border-radius: var(--radio);
            overflow: hidden;
            background: var(--superficie-2);
            border: 1px solid var(--borde);
            position: relative;
        }

        .visor-controles
        {
            display: flex;
            align-items: center;
            gap: 12px;
            margin-top: 14px;
            flex-wrap: wrap;
        }

        .visor-tiempo
        {
            font-family: var(--fuente-mono);
            font-size: .82rem;
            color: var(--texto-suave);
            white-space: nowrap;
        }

        .linea-tiempo
        {
            flex: 1 1 220px;
            accent-color: var(--acento);
            min-width: 160px;
        }

        .visor-botones
        {
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .visor-botones .icono-boton
        {
            color: var(--texto);
        }

        .visor-botones .icono-boton:hover
        {
            background: var(--superficie-2);
        }

        .visor-botones .principal
        {
            width: 44px;
            height: 44px;
            border-radius: 50%;
            background: var(--acento);
            color: var(--marca-azul);
        }

        .visor-botones .principal:hover
        {
            background: var(--acento-hover);
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

        @media (max-width: 940px)
        {
            .vis { grid-template-columns: minmax(0, 1fr); margin: -20px -16px -48px; }
            .vis-rail
            {
                position: static;
                max-height: none;
                border-right: 0;
                border-bottom: 1px solid var(--borde);
            }
            .vis-main { padding: 18px 16px 48px; }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

<div class="vis">

    <nav class="vis-rail" aria-label="Sesiones y filtros">

        <h2 data-i18n="vis.rail.sesiones">Sesiones</h2>

        <div class="vis-buscador campo-buscar">
            <label class="solo-lectores" for="visBuscar" data-i18n="vis.buscar">Buscar por nombre</label>
            <svg aria-hidden="true"><use href="#i-buscar" /></svg>
            <input type="search" class="entrada" id="visBuscar"
                   placeholder="Checkout, onboarding..." data-i18n-attr="placeholder:vis.buscar.placeholder" />
        </div>

        <div class="vis-chips chips" data-unico role="group" aria-label="Filtrar por categoría">
            <button type="button" class="chip" aria-pressed="true" data-categoria="todas" data-i18n="vis.cat.todas">Todas</button>
            <button type="button" class="chip" aria-pressed="false" data-categoria="appweb" data-i18n="vis.cat.appweb">App web</button>
            <button type="button" class="chip" aria-pressed="false" data-categoria="software" data-i18n="vis.cat.software">Software</button>
            <button type="button" class="chip" aria-pressed="false" data-categoria="videojuego" data-i18n="vis.cat.videojuego">Videojuego</button>
            <button type="button" class="chip" aria-pressed="false" data-categoria="publicidad" data-i18n="vis.cat.publicidad">Publicidad</button>
            <button type="button" class="chip" aria-pressed="false" data-categoria="appmovil" data-i18n="vis.cat.appmovil">App móvil</button>
        </div>

        <a class="btn btn-secundario btn-chico btn-bloque" href="<%: ResolveUrl("~/Categorias.aspx") %>">
            <svg width="16" height="16" aria-hidden="true"><use href="#i-mas" /></svg>
            <span data-i18n="vis.nuevaCategoria">Nueva categoría</span>
        </a>

        <h2 data-i18n="vis.rail.listado">Listado de sesiones</h2>

        <div class="vis-lista" id="listaSesiones">

            <button type="button" class="vis-sesion" aria-current="true"
                    data-categoria="appweb" data-estado="finalizado" data-tipo="App e-commerce"
                    data-nombre="Checkout — App e-commerce" data-fecha="12 jul 2026" data-duracion="18:42" data-analista="María Gómez">
                <span class="nombre">Checkout — App e-commerce</span>
                <span class="badge badge-exito" data-i18n="estado.finalizado">Finalizado</span>
                <span class="meta">12 jul 2026 &middot; 18:42</span>
            </button>

            <button type="button" class="vis-sesion"
                    data-categoria="videojuego" data-estado="finalizado" data-tipo="RPG móvil"
                    data-nombre="Onboarding — RPG móvil" data-fecha="10 jul 2026" data-duracion="09:15" data-analista="Nicolás Castro">
                <span class="nombre">Onboarding — RPG móvil</span>
                <span class="badge badge-exito" data-i18n="estado.finalizado">Finalizado</span>
                <span class="meta">10 jul 2026 &middot; 09:15</span>
            </button>

            <button type="button" class="vis-sesion" disabled
                    data-categoria="publicidad" data-estado="procesando" data-tipo="Campaña Q3"
                    data-nombre="Landing — Campaña Q3" data-fecha="08 jul 2026" data-duracion="04:03" data-analista="María Gómez">
                <span class="nombre">Landing — Campaña Q3</span>
                <span class="badge-vivo" data-i18n="estado.procesando">Procesando</span>
                <span class="meta">08 jul 2026 &middot; 04:03</span>
            </button>

            <button type="button" class="vis-sesion"
                    data-categoria="software" data-estado="finalizado" data-tipo="Panel interno"
                    data-nombre="Login — Panel interno" data-fecha="05 jul 2026" data-duracion="06:51" data-analista="María Gómez">
                <span class="nombre">Login — Panel interno</span>
                <span class="badge badge-exito" data-i18n="estado.finalizado">Finalizado</span>
                <span class="meta">05 jul 2026 &middot; 06:51</span>
            </button>

            <button type="button" class="vis-sesion"
                    data-categoria="appweb" data-estado="finalizado" data-tipo="App e-commerce"
                    data-nombre="Carrito — App e-commerce" data-fecha="01 jul 2026" data-duracion="12:27" data-analista="Valentina Ruiz">
                <span class="nombre">Carrito — App e-commerce</span>
                <span class="badge badge-exito" data-i18n="estado.finalizado">Finalizado</span>
                <span class="meta">01 jul 2026 &middot; 12:27</span>
            </button>

            <button type="button" class="vis-sesion"
                    data-categoria="appmovil" data-estado="finalizado" data-tipo="App móvil"
                    data-nombre="Alta de cuenta — App móvil" data-fecha="27 jun 2026" data-duracion="07:40" data-analista="Nicolás Castro">
                <span class="nombre">Alta de cuenta — App móvil</span>
                <span class="badge badge-exito" data-i18n="estado.finalizado">Finalizado</span>
                <span class="meta">27 jun 2026 &middot; 07:40</span>
            </button>

        </div>

        <div class="vacio" id="sesionesVacio" hidden>
            <svg width="28" height="28" aria-hidden="true"><use href="#i-carpeta" /></svg>
            <p class="sin-margen" data-i18n="vis.sinSesiones">No hay sesiones que coincidan con los filtros.</p>
        </div>

    </nav>

    <div class="vis-main">

        <p class="migas">
            <span data-i18n="vis.rail.sesiones">Sesiones</span>
            <span aria-hidden="true">/</span>
            <strong id="migaSesion">Checkout — App e-commerce</strong>
        </p>

        <div class="pagina-cabecera">
            <div>
                <h1 id="tituloSesion" tabindex="-1">Checkout — App e-commerce</h1>
                <p class="texto-suave sin-margen" id="subtituloSesion">
                    12 jul 2026 &middot; 18:42 min &middot; Analista: María Gómez &middot; App e-commerce
                </p>
            </div>
            <div class="acciones">
                <button type="button" class="btn btn-primario" data-abre-modal="modalExportar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-descarga" /></svg>
                    <span data-i18n="vis.exportar">Descargar reporte</span>
                </button>
            </div>
        </div>

        <div class="tablero">

            <div class="visual destacada">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-metricas" /></svg>
                    <span data-i18n="vis.metricas.titulo">Métricas generales</span>
                </span>
                <div class="metricas">
                    <div>
                        <div class="metrica-valor">18:42</div>
                        <div class="metrica-nombre" data-i18n="vis.metrica.duracion">Duración total</div>
                    </div>
                    <div>
                        <div class="metrica-valor">2,3 s</div>
                        <div class="metrica-nombre" data-i18n="vis.metrica.fijacionMedia">Fijación media</div>
                    </div>
                    <div>
                        <div class="metrica-valor">47</div>
                        <div class="metrica-nombre" data-i18n="vis.metrica.fijaciones">Fijaciones</div>
                    </div>
                    <div>
                        <div class="metrica-valor">62%</div>
                        <div class="metrica-nombre" data-i18n="vis.metrica.atencion">Atención activa</div>
                    </div>
                    <div>
                        <div class="metrica-valor">38%</div>
                        <div class="metrica-nombre" data-i18n="vis.metrica.sacadico">Movimiento sacádico</div>
                    </div>
                </div>
            </div>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="grabacion">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-video" /></svg>
                    <span data-i18n="vis.grafico.grabacion">Grabación con seguimiento</span>
                </span>
                <span class="grafico grafico-video" aria-hidden="true">
                    <span class="play"><svg width="22" height="22"><use href="#i-play" /></svg></span>
                </span>
                <span class="ver">
                    <span data-i18n="comun.verDetalle">Ver detalle</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="calor">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-calor" /></svg>
                    <span data-i18n="vis.grafico.calor">Mapa de calor</span>
                </span>
                <span class="grafico grafico-calor" aria-hidden="true"></span>
                <span class="ver">
                    <span data-i18n="comun.verDetalle">Ver detalle</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="zonas">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-zonas" /></svg>
                    <span data-i18n="vis.grafico.zonas">Zonas de interés y ciegas</span>
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
                    <span data-i18n="comun.verDetalle">Ver detalle</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="dispersion">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-dispersion" /></svg>
                    <span data-i18n="vis.grafico.dispersion">Dispersión de mirada</span>
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
                    <span data-i18n="comun.verDetalle">Ver detalle</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="atencion">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-torta" /></svg>
                    <span data-i18n="vis.grafico.atencion">Atención frente a distracción</span>
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
                    <span data-i18n="comun.verDetalle">Ver detalle</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

            <button type="button" class="visual" data-abre-modal="modalVisor" data-visual="recorrido">
                <span class="rotulo">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-recorrido" /></svg>
                    <span data-i18n="vis.grafico.recorrido">Recorrido de mirada</span>
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
                    <span data-i18n="comun.verDetalle">Ver detalle</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </span>
            </button>

        </div>

    </div>
</div>

<div class="modal-fondo" id="modalVisor" role="dialog" aria-modal="true" aria-labelledby="visorTitulo" hidden>
    <div class="modal extra">

        <div class="modal-cabecera">
            <div>
                <h2 id="visorTitulo" data-i18n="vis.visor.titulo">Grabación con seguimiento</h2>
                <p class="subtitulo sin-margen" id="visorSubtitulo">Checkout — App e-commerce &middot; 12 jul 2026</p>
            </div>
            <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
            </button>
        </div>

        <div class="modal-cuerpo">

            <div class="visor-lienzo" id="visorLienzo"></div>

            <div class="visor-controles">
                <div class="visor-botones">
                    <button type="button" class="icono-boton" aria-label="Retroceder diez segundos">
                        <svg width="20" height="20" aria-hidden="true"><use href="#i-atras" /></svg>
                    </button>
                    <button type="button" class="icono-boton principal" id="btnReproducir" aria-label="Reproducir">
                        <svg width="20" height="20" aria-hidden="true" data-icono-play><use href="#i-play" /></svg>
                        <svg width="20" height="20" aria-hidden="true" data-icono-pausa style="display:none"><use href="#i-pausa" /></svg>
                    </button>
                    <button type="button" class="icono-boton" aria-label="Adelantar diez segundos">
                        <svg width="20" height="20" aria-hidden="true"><use href="#i-adelante" /></svg>
                    </button>
                </div>

                <span class="visor-tiempo" id="visorTiempo">06:32 / 18:42</span>

                <label class="solo-lectores" for="visorLinea" data-i18n="vis.visor.linea">Posición en la grabación</label>
                <input type="range" class="linea-tiempo" id="visorLinea" min="0" max="100" value="35" />

                <button type="button" class="btn btn-secundario btn-chico" data-cierra-modal data-i18n="comun.salir">Salir</button>
            </div>

            <div class="leyenda" id="visorLeyenda"></div>

            <div class="aviso aviso-info mt-16 mb-0">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
                <p class="sin-margen" id="visorAyuda" data-i18n="vis.visor.ayuda">
                    Muévete por la línea de tiempo para ver cómo cambia el análisis a lo largo de la sesión.
                </p>
            </div>

        </div>
    </div>
</div>

<div class="modal-fondo" id="modalExportar" role="dialog" aria-modal="true" aria-labelledby="exportarTitulo" hidden>
    <div class="modal">

        <div class="modal-cabecera">
            <div>
                <h2 id="exportarTitulo" data-i18n="vis.exportar.titulo">Exportar análisis</h2>
                <p class="subtitulo sin-margen" data-i18n="vis.exportar.subtitulo">
                    Elige qué incluir en el reporte PDF.
                </p>
            </div>
            <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
            </button>
        </div>

        <div class="modal-cuerpo">

            <p class="etiqueta" data-i18n="vis.exportar.contenido">Contenido del reporte</p>

            <label class="opcion">
                <input type="checkbox" checked disabled />
                <span>
                    <span class="opcion-titulo" data-i18n="vis.exportar.portada">Nombre, fecha y métricas generales</span>
                    <span class="opcion-detalle" data-i18n="vis.exportar.portada.detalle">Siempre se incluye en el reporte.</span>
                </span>
            </label>

            <p class="etiqueta mt-16" data-i18n="vis.exportar.visualizaciones">Visualizaciones a incluir</p>

            <label class="opcion">
                <input type="checkbox" name="expVisual" value="grabacion" />
                <span>
                    <span class="opcion-titulo" data-i18n="vis.grafico.grabacion">Grabación con seguimiento</span>
                    <span class="opcion-detalle" data-i18n="vis.exportar.grabacion.detalle">Se incluye como cuadro representativo.</span>
                </span>
            </label>

            <label class="opcion">
                <input type="checkbox" name="expVisual" value="calor" checked />
                <span>
                    <span class="opcion-titulo" data-i18n="vis.grafico.calor">Mapa de calor</span>
                    <span class="opcion-detalle" data-i18n="vis.exportar.calor.detalle">Concentración de la atención visual.</span>
                </span>
            </label>

            <label class="opcion">
                <input type="checkbox" name="expVisual" value="zonas" checked />
                <span>
                    <span class="opcion-titulo" data-i18n="vis.grafico.zonas">Zonas de interés y ciegas</span>
                    <span class="opcion-detalle" data-i18n="vis.exportar.zonas.detalle">Áreas miradas y áreas ignoradas.</span>
                </span>
            </label>

            <label class="opcion">
                <input type="checkbox" name="expVisual" value="dispersion" />
                <span>
                    <span class="opcion-titulo" data-i18n="vis.grafico.dispersion">Dispersión de mirada</span>
                    <span class="opcion-detalle" data-i18n="vis.exportar.dispersion.detalle">Qué tan estable fue la atención.</span>
                </span>
            </label>

            <label class="opcion">
                <input type="checkbox" name="expVisual" value="atencion" />
                <span>
                    <span class="opcion-titulo" data-i18n="vis.grafico.atencion">Atención frente a distracción</span>
                    <span class="opcion-detalle" data-i18n="vis.exportar.atencion.detalle">Fijación activa frente a movimiento sacádico.</span>
                </span>
            </label>

            <label class="opcion">
                <input type="checkbox" name="expVisual" value="recorrido" checked />
                <span>
                    <span class="opcion-titulo" data-i18n="vis.grafico.recorrido">Recorrido de mirada</span>
                    <span class="opcion-detalle" data-i18n="vis.exportar.recorrido.detalle">Trayecto secuencial sobre el activo.</span>
                </span>
            </label>

            <p class="ayuda" id="exportarAviso" role="status"></p>

        </div>

        <div class="modal-pie">
            <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
            <button type="button" class="btn btn-primario" id="btnExportar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-descarga" /></svg>
                <span data-i18n="vis.exportar.boton">Exportar reporte</span>
            </button>
        </div>

    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var chips = document.querySelectorAll(".vis-chips .chip");
            var sesiones = document.querySelectorAll(".vis-sesion");
            var buscar = document.getElementById("visBuscar");
            var vacio = document.getElementById("sesionesVacio");
            var categoriaActiva = "todas";

            var tituloSesion = document.getElementById("tituloSesion");
            var migaSesion = document.getElementById("migaSesion");
            var subtituloSesion = document.getElementById("subtituloSesion");
            var visorSubtitulo = document.getElementById("visorSubtitulo");

            function normalizar(texto) {
                return window.Falke && window.Falke.normalizar ? window.Falke.normalizar(texto) : String(texto || "").toLowerCase().trim();
            }

            function filtrar() {
                var texto = normalizar(buscar.value);
                var visibles = 0;

                for (var i = 0; i < sesiones.length; i++) {
                    var s = sesiones[i];
                    var okCategoria = categoriaActiva === "todas" || s.getAttribute("data-categoria") === categoriaActiva;
                    var okTexto = texto === "" || normalizar(s.getAttribute("data-nombre")).indexOf(texto) !== -1;
                    var mostrar = okCategoria && okTexto;

                    s.hidden = !mostrar;

                    if (mostrar) visibles++;
                }

                vacio.hidden = visibles > 0;
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

            function elegirSesion(boton) {
                if (boton.disabled) return;

                for (var k = 0; k < sesiones.length; k++) {
                    sesiones[k].removeAttribute("aria-current");
                }
                boton.setAttribute("aria-current", "true");

                var nombre = boton.getAttribute("data-nombre");

                tituloSesion.textContent = nombre;
                migaSesion.textContent = nombre;
                subtituloSesion.textContent = boton.getAttribute("data-fecha") + " · " +
                    boton.getAttribute("data-duracion") + " min · Analista: " +
                    boton.getAttribute("data-analista") + " · " + boton.getAttribute("data-tipo");
                visorSubtitulo.textContent = nombre + " · " + boton.getAttribute("data-fecha");

                tituloSesion.focus();
                if (window.innerWidth <= 940) window.scrollTo(0, tituloSesion.offsetTop - 70);
            }

            for (i = 0; i < sesiones.length; i++) {
                sesiones[i].addEventListener("click", function () { elegirSesion(this); });
            }

            var VISUALES = {
                grabacion: {
                    titulo: "Grabación con seguimiento",
                    lienzo: '<span class="grafico grafico-video" style="height:100%;aspect-ratio:auto;border:0"></span>',
                    leyenda: '<span><i style="background:#F26157"></i>Punto de mirada actual</span>'
                },
                calor: {
                    titulo: "Mapa de calor",
                    lienzo: '<span class="grafico grafico-calor" style="height:100%;aspect-ratio:auto;border:0"></span>',
                    leyenda: '<span><i style="background:#F26157"></i>Mayor concentración</span>' +
                             '<span><i style="background:#E8AF52"></i>Concentración media</span>' +
                             '<span><i style="background:#56637C"></i>Menor concentración</span>'
                },
                zonas: {
                    titulo: "Zonas de interés y zonas ciegas",
                    lienzo: '<svg viewBox="0 0 320 180" preserveAspectRatio="none" style="width:100%;height:100%">' +
                            '<rect class="zona-interes" x="30" y="26" width="120" height="62" rx="6" />' +
                            '<rect class="zona-interes" x="170" y="60" width="90" height="46" rx="6" />' +
                            '<rect class="zona-ciega" x="42" y="112" width="86" height="44" rx="6" />' +
                            '<rect class="zona-ciega" x="212" y="122" width="74" height="36" rx="6" /></svg>',
                    leyenda: '<span><i style="background:rgba(232,175,82,.5);border:1px solid #E8AF52"></i>Zona de interés</span>' +
                             '<span><i style="background:transparent;border:1px dashed #56637C"></i>Zona ciega</span>'
                },
                dispersion: {
                    titulo: "Dispersión de mirada",
                    lienzo: '<svg viewBox="0 0 320 180" preserveAspectRatio="none" style="width:100%;height:100%">' +
                            '<ellipse cx="158" cy="88" rx="86" ry="48" fill="rgba(232,175,82,.16)" stroke="#E8AF52" stroke-width="2" stroke-dasharray="5 4" />' +
                            '<circle class="punto-mirada" cx="120" cy="66" r="4" /><circle class="punto-mirada" cx="168" cy="58" r="4" />' +
                            '<circle class="punto-mirada" cx="196" cy="92" r="4" /><circle class="punto-mirada" cx="142" cy="104" r="4" />' +
                            '<circle class="punto-mirada" cx="104" cy="98" r="4" /><circle class="punto-mirada" cx="182" cy="120" r="4" />' +
                            '<circle cx="158" cy="88" r="5" fill="#F26157" /></svg>',
                    leyenda: '<span><i style="background:#56637C"></i>Punto de mirada</span>' +
                             '<span><i style="background:#F26157"></i>Centroide</span>' +
                             '<span><i style="background:transparent;border:1px dashed #E8AF52"></i>Elipse de dispersión</span>'
                },
                atencion: {
                    titulo: "Atención frente a distracción",
                    lienzo: '<span style="display:flex;height:100%;align-items:center;justify-content:center">' +
                            '<svg viewBox="0 0 120 120" style="height:76%">' +
                            '<circle cx="60" cy="60" r="44" fill="none" stroke="#0E1A34" stroke-width="18" />' +
                            '<circle cx="60" cy="60" r="44" fill="none" stroke="#E8AF52" stroke-width="18" stroke-dasharray="171 276" transform="rotate(-90 60 60)" />' +
                            '<text x="60" y="66" text-anchor="middle" font-size="20" font-weight="700" fill="currentColor">62%</text>' +
                            '</svg></span>',
                    leyenda: '<span><i style="background:#E8AF52"></i>Fijación activa (62%)</span>' +
                             '<span><i style="background:#0E1A34"></i>Movimiento sacádico (38%)</span>'
                },
                recorrido: {
                    titulo: "Recorrido de mirada",
                    lienzo: '<svg viewBox="0 0 320 180" preserveAspectRatio="none" style="width:100%;height:100%">' +
                            '<path class="linea-recorrido" d="M40 132 C 88 40, 132 150, 176 74 S 250 44, 284 96" />' +
                            '<circle cx="40" cy="132" r="5" fill="#56637C" /><circle cx="176" cy="74" r="5" fill="#0E1A34" />' +
                            '<circle cx="284" cy="96" r="5" fill="#F26157" /></svg>',
                    leyenda: '<span><i style="background:#56637C"></i>Inicio</span>' +
                             '<span><i style="background:#F26157"></i>Final</span>'
                }
            };

            var visorTitulo = document.getElementById("visorTitulo");
            var visorLienzo = document.getElementById("visorLienzo");
            var visorLeyenda = document.getElementById("visorLeyenda");
            var abrenVisor = document.querySelectorAll("[data-visual]");

            for (i = 0; i < abrenVisor.length; i++) {
                abrenVisor[i].addEventListener("click", function () {
                    var datos = VISUALES[this.getAttribute("data-visual")];
                    if (!datos) return;
                    visorTitulo.textContent = datos.titulo;
                    visorLienzo.innerHTML = datos.lienzo;
                    visorLeyenda.innerHTML = datos.leyenda;
                });
            }

            var btnReproducir = document.getElementById("btnReproducir");
            var reproduciendo = false;

            btnReproducir.addEventListener("click", function () {
                reproduciendo = !reproduciendo;
                this.querySelector("[data-icono-play]").style.display = reproduciendo ? "none" : "";
                this.querySelector("[data-icono-pausa]").style.display = reproduciendo ? "" : "none";
                this.setAttribute("aria-label", reproduciendo ? "Pausar" : "Reproducir");
            });

            var visorLinea = document.getElementById("visorLinea");
            var visorTiempo = document.getElementById("visorTiempo");
            var DURACION_DEMO = 1122;

            function aMinutos(segundos) {
                var m = Math.floor(segundos / 60);
                var s = Math.floor(segundos % 60);
                return (m < 10 ? "0" : "") + m + ":" + (s < 10 ? "0" : "") + s;
            }

            visorLinea.addEventListener("input", function () {
                var actual = DURACION_DEMO * (this.value / 100);
                visorTiempo.textContent = aMinutos(actual) + " / " + aMinutos(DURACION_DEMO);
            });

            var btnExportar = document.getElementById("btnExportar");
            var exportarAviso = document.getElementById("exportarAviso");

            btnExportar.addEventListener("click", function () {
                var elegidas = document.querySelectorAll("input[name='expVisual']:checked");

                if (elegidas.length === 0) {
                    exportarAviso.className = "ayuda texto-peligro";
                    exportarAviso.textContent = "Elige al menos una visualización para incluir en el reporte.";
                    return;
                }

                exportarAviso.className = "ayuda texto-suave";
                exportarAviso.textContent = "Se generaría un PDF con " + elegidas.length + " visualización(es).";
            });
        })();
    </script>
</asp:Content>
