<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="AnalisisMultiple.aspx.cs" Inherits="GUI.AnalisisMultiple" Title="Análisis múltiple" %>

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

        .tablero-multiple
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(258px, 1fr));
            gap: 16px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <section id="panelSeleccion">

        <div class="pagina-cabecera">
            <div>
                <h1 data-i18n="multiple.titulo">Análisis múltiple</h1>
                <p class="texto-suave sin-margen medida" data-i18n="multiple.bajada">
                    Combina varias sesiones para obtener resultados promediados y ver que patrones
                    se repiten entre distintos testers.
                </p>
            </div>
        </div>

        <div class="aviso aviso-info">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
            <div>
                <strong data-i18n="multiple.regla.titulo">Todas las sesiones tienen que ser del mismo tipo de activo</strong>
                <p class="sin-margen" data-i18n="multiple.regla.texto">
                    Promediar sesiones de tipos distintos daria un resultado estadisticamente
                    inválido, así que al elegir la primera se deshabilitan las que no son compatibles.
                    Hacen falta al menos dos.
                </p>
            </div>
        </div>

        <div class="seleccion">

            <div class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-ojo" /></svg>
                    <span data-i18n="multiple.disponibles">Sesiones finalizadas</span>
                    <span class="derecha texto-chico texto-suave" data-i18n="multiple.soloFinalizadas">Solo se listan las ya procesadas</span>
                </div>

                <div id="listaElegibles">

                    <label class="opcion fila-sesion" data-tipo="appweb">
                        <input type="checkbox" name="sesion" value="2451" data-nombre="Checkout - App e-commerce" />
                        <span>
                            <span class="opcion-titulo">Checkout - App e-commerce</span>
                            <span class="opcion-detalle">
                                <span>App web</span><span>12 jul 2026</span><span>18:42 min</span><span>Maria Gomez</span>
                            </span>
                        </span>
                    </label>

                    <label class="opcion fila-sesion" data-tipo="appweb">
                        <input type="checkbox" name="sesion" value="2390" data-nombre="Carrito - App e-commerce" />
                        <span>
                            <span class="opcion-titulo">Carrito - App e-commerce</span>
                            <span class="opcion-detalle">
                                <span>App web</span><span>01 jul 2026</span><span>12:27 min</span><span>Valentina Ruiz</span>
                            </span>
                        </span>
                    </label>

                    <label class="opcion fila-sesion" data-tipo="appweb">
                        <input type="checkbox" name="sesion" value="2364" data-nombre="Checkout - Tester 03" />
                        <span>
                            <span class="opcion-titulo">Checkout - Tester 03</span>
                            <span class="opcion-detalle">
                                <span>App web</span><span>28 jun 2026</span><span>15:08 min</span><span>Maria Gomez</span>
                            </span>
                        </span>
                    </label>

                    <label class="opcion fila-sesion" data-tipo="videojuego">
                        <input type="checkbox" name="sesion" value="2402" data-nombre="Onboarding - RPG movil" />
                        <span>
                            <span class="opcion-titulo">Onboarding - RPG móvil</span>
                            <span class="opcion-detalle">
                                <span>Videojuego</span><span>10 jul 2026</span><span>09:15 min</span><span>Nicolas Castro</span>
                            </span>
                        </span>
                    </label>

                    <label class="opcion fila-sesion" data-tipo="software">
                        <input type="checkbox" name="sesion" value="2377" data-nombre="Login - Panel interno" />
                        <span>
                            <span class="opcion-titulo">Login - Panel interno</span>
                            <span class="opcion-detalle">
                                <span>Software</span><span>05 jul 2026</span><span>06:51 min</span><span>Maria Gomez</span>
                            </span>
                        </span>
                    </label>

                    <label class="opcion fila-sesion" data-tipo="appmovil">
                        <input type="checkbox" name="sesion" value="2318" data-nombre="Alta de cuenta - App movil" />
                        <span>
                            <span class="opcion-titulo">Alta de cuenta - App móvil</span>
                            <span class="opcion-detalle">
                                <span>App móvil</span><span>27 jun 2026</span><span>07:40 min</span><span>Nicolas Castro</span>
                            </span>
                        </span>
                    </label>

                </div>
            </div>

            <aside class="tarjeta resumen-seleccion">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-comparar" /></svg>
                    <span data-i18n="multiple.resumen">Selección actual</span>
                </div>

                <p class="contador" id="contadorElegidas">0</p>
                <p class="texto-chico texto-suave" id="tipoElegido" data-i18n="multiple.sinTipo">
                    Todavía no elegiste ninguna sesión.
                </p>

                <ul class="elegidas" id="listaElegidas"></ul>

                <button type="button" class="btn btn-primario btn-bloque" id="btnGenerar" disabled>
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-metricas" /></svg>
                    <span data-i18n="multiple.generar">Generar análisis</span>
                </button>

                <p class="ayuda" id="avisoSeleccion" role="status" data-i18n="multiple.minimo">
                    Necesitas al menos dos sesiones del mismo tipo.
                </p>
            </aside>

        </div>
    </section>

    <section id="panelResultado" hidden>

        <p class="migas">
            <button type="button" class="btn btn-fantasma btn-chico" id="btnVolverSeleccion" style="padding-left:0">
                <span data-i18n="multiple.volver">Selección de sesiones</span>
            </button>
            / <strong data-i18n="multiple.resultado">Resultado combinado</strong>
        </p>

        <div class="pagina-cabecera">
            <div>
                <h1 id="tituloResultado" data-i18n="multiple.resultado.titulo">Resultado combinado</h1>
                <p class="texto-suave sin-margen" id="subtituloResultado"></p>
            </div>
            <div class="acciones">
                <button type="button" class="btn btn-primario">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-descarga" /></svg>
                    <span data-i18n="multiple.exportar">Descargar reporte</span>
                </button>
            </div>
        </div>

        <div class="tarjeta mb-24">
            <div class="tarjeta-cabecera">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-metricas" /></svg>
                <span data-i18n="multiple.metricas">Métricas promediadas</span>
            </div>

            <div class="metricas">
                <div>
                    <div class="metrica-valor">15:26</div>
                    <div class="metrica-nombre" data-i18n="multiple.metrica.duracion">Duración promedio</div>
                </div>
                <div>
                    <div class="metrica-valor">2.1 s</div>
                    <div class="metrica-nombre" data-i18n="vis.metrica.fijacionMedia">Fijación media</div>
                </div>
                <div>
                    <div class="metrica-valor">41</div>
                    <div class="metrica-nombre" data-i18n="multiple.metrica.fijaciones">Fijaciones promedio</div>
                </div>
                <div>
                    <div class="metrica-valor">58%</div>
                    <div class="metrica-nombre" data-i18n="vis.metrica.atencion">Atención activa</div>
                </div>
                <div>
                    <div class="metrica-valor" id="metricaSesiones">3</div>
                    <div class="metrica-nombre" data-i18n="multiple.metrica.sesiones">Sesiones combinadas</div>
                </div>
            </div>
        </div>

        <h2 class="mb-16" style="font-size:1.1rem" data-i18n="multiple.graficos">Gráficos promediados</h2>

        <div class="tablero-multiple">

            <article class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-calor" /></svg>
                    <span data-i18n="vis.grafico.calor">Mapa de calor</span>
                </div>
                <div class="grafico grafico-calor" aria-hidden="true"></div>
                <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.calor.detalle">
                    Densidad de fijaciones promediada entre las sesiones elegidas.
                </p>
            </article>

            <article class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-zonas" /></svg>
                    <span data-i18n="vis.grafico.zonas">Zonas de interés y ciegas</span>
                </div>
                <div class="grafico" aria-hidden="true">
                    <svg viewBox="0 0 320 180" preserveAspectRatio="none">
                        <rect class="zona-interes" x="36" y="30" width="128" height="58" rx="6" />
                        <rect class="zona-interes" x="182" y="52" width="82" height="48" rx="6" />
                        <rect class="zona-ciega" x="48" y="114" width="96" height="42" rx="6" />
                    </svg>
                </div>
                <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.zonas.detalle">
                    Áreas que la mayoria de los testers miro y áreas que ignoraron.
                </p>
            </article>

            <article class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-dispersion" /></svg>
                    <span data-i18n="vis.grafico.dispersion">Dispersión de mirada</span>
                </div>
                <div class="grafico" aria-hidden="true">
                    <svg viewBox="0 0 320 180" preserveAspectRatio="none">
                        <ellipse cx="156" cy="88" rx="94" ry="52" fill="rgba(232,175,82,.16)" stroke="#E8AF52" stroke-width="2" stroke-dasharray="5 4" />
                        <circle class="punto-mirada" cx="112" cy="70" r="4" />
                        <circle class="punto-mirada" cx="160" cy="60" r="4" />
                        <circle class="punto-mirada" cx="200" cy="96" r="4" />
                        <circle class="punto-mirada" cx="138" cy="108" r="4" />
                        <circle class="punto-mirada" cx="186" cy="124" r="4" />
                        <circle cx="156" cy="88" r="5" fill="#F26157" />
                    </svg>
                </div>
                <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.dispersion.detalle">
                    Cuanto varia la atención entre los distintos participantes.
                </p>
            </article>

            <article class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-torta" /></svg>
                    <span data-i18n="vis.grafico.atencion">Atención vs. distracción</span>
                </div>
                <div class="grafico" aria-hidden="true" style="display:flex;align-items:center;justify-content:center">
                    <svg viewBox="0 0 120 120" style="width:auto;height:82%">
                        <circle cx="60" cy="60" r="44" fill="none" stroke="#0E1A34" stroke-width="18" />
                        <circle cx="60" cy="60" r="44" fill="none" stroke="#E8AF52" stroke-width="18"
                                stroke-dasharray="160 276" transform="rotate(-90 60 60)" />
                        <text x="60" y="66" text-anchor="middle" font-size="20" font-weight="700" fill="currentColor">58%</text>
                    </svg>
                </div>
                <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.atencion.detalle">
                    Proporción media de fijación activa frente a movimiento sacádico.
                </p>
            </article>

            <article class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-recorrido" /></svg>
                    <span data-i18n="vis.grafico.recorrido">Recorrido de mirada</span>
                </div>
                <div class="grafico" aria-hidden="true">
                    <svg viewBox="0 0 320 180" preserveAspectRatio="none">
                        <path class="linea-recorrido" d="M44 128 C 92 48, 138 144, 180 78 S 248 50, 280 92" opacity=".45" />
                        <path class="linea-recorrido" d="M40 136 C 84 60, 130 138, 176 84 S 244 58, 284 100" />
                        <circle cx="40" cy="136" r="5" fill="#56637C" />
                        <circle cx="284" cy="100" r="5" fill="#F26157" />
                    </svg>
                </div>
                <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.recorrido.detalle">
                    Trayecto tipico: la línea llena es el promedio del conjunto.
                </p>
            </article>

            <article class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-comparar" /></svg>
                    <span data-i18n="multiple.porSesion">Atención por sesión</span>
                </div>
                <div class="grafico" aria-hidden="true" style="display:flex;align-items:flex-end;gap:10px;padding:20px">
                    <span style="flex:1;height:62%;background:var(--marca-azul);border-radius:4px"></span>
                    <span style="flex:1;height:48%;background:var(--marca-gris);border-radius:4px"></span>
                    <span style="flex:1;height:76%;background:var(--marca-dorado);border-radius:4px"></span>
                </div>
                <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="multiple.porSesion.detalle">
                    Comparación directa del porcentaje de atención activa de cada sesión.
                </p>
            </article>

        </div>
    </section>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
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

            var TIPOS = {
                appweb: "App web",
                appmovil: "App móvil",
                software: "Software",
                videojuego: "Videojuego",
                publicidad: "Publicidad"
            };

            function elegidas() {
                return document.querySelectorAll("input[name='sesion']:checked");
            }

            function tipoActivo() {
                var marcadas = elegidas();

                if (marcadas.length === 0) return null;

                return marcadas[0].closest(".fila-sesion").getAttribute("data-tipo");
            }

            function actualizar() {
                var marcadas = elegidas();
                var tipo = tipoActivo();
                var i;

                for (i = 0; i < filas.length; i++) {
                    var casilla = filas[i].querySelector("input");
                    var incompatible = tipo !== null && filas[i].getAttribute("data-tipo") !== tipo;

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

                if (tipo === null) {
                    tipoElegido.textContent = "Todavia no elegiste ninguna sesion.";
                } else {
                    tipoElegido.textContent = "Tipo de activo: " + TIPOS[tipo] + ".";
                }

                var suficientes = marcadas.length >= 2;

                btnGenerar.disabled = !suficientes;
                aviso.textContent = suficientes
                    ? "Listo para combinar " + marcadas.length + " sesiones."
                    : "Necesitas al menos dos sesiones del mismo tipo.";
            }

            for (var i = 0; i < casillas.length; i++) {
                casillas[i].addEventListener("change", actualizar);
            }

            var panelSeleccion = document.getElementById("panelSeleccion");
            var panelResultado = document.getElementById("panelResultado");
            var subtitulo = document.getElementById("subtituloResultado");
            var metricaSesiones = document.getElementById("metricaSesiones");

            btnGenerar.addEventListener("click", function () {
                var marcadas = elegidas();
                var nombres = [];

                for (var i = 0; i < marcadas.length; i++) {
                    nombres.push(marcadas[i].getAttribute("data-nombre"));
                }

                subtitulo.textContent = TIPOS[tipoActivo()] + " · " + nombres.join(" · ");
                metricaSesiones.textContent = marcadas.length;

                panelSeleccion.hidden = true;
                panelResultado.hidden = false;
                window.scrollTo(0, 0);
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
