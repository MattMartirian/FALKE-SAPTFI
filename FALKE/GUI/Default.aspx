<%@ Page Language="C#" MasterPageFile="~/Publico.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="GUI.Inicio" Title="Inicio" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        .hero
        {
            background: var(--marca-azul);
            color: var(--marca-perla);
            padding: 76px 0 88px;
        }

        .hero .contenedor
        {
            display: grid;
            grid-template-columns: 1.05fr .95fr;
            gap: 56px;
            align-items: center;
        }

        .hero h1
        {
            font-size: 2.9rem;
            line-height: 1.12;
            margin-bottom: 18px;
            text-wrap: balance;
        }

        .hero h1 em
        {
            color: var(--marca-dorado);
            font-style: normal;
        }

        .hero p.bajada
        {
            font-size: 1.08rem;
            color: rgba(252, 252, 247, .84);
            max-width: 52ch;
            margin-bottom: 28px;
        }

        .hero .btn-secundario
        {
            background: transparent;
            border-color: rgba(252, 252, 247, .35);
            color: var(--marca-perla);
        }

        .hero .btn-secundario:hover
        {
            background: rgba(252, 252, 247, .08);
            border-color: rgba(252, 252, 247, .6);
        }

        .previa
        {
            background: #0B1428;
            border: 1px solid rgba(252, 252, 247, .1);
            padding: 20px 22px 22px;
        }

        .previa-barra
        {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            margin-bottom: 14px;
            font-family: var(--fuente-mono);
            font-size: .68rem;
            letter-spacing: .08em;
            text-transform: uppercase;
            color: rgba(252, 252, 247, .55);
        }

        .previa-vivo
        {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            color: var(--marca-coral);
        }

        .previa-vivo::before
        {
            content: "";
            width: 7px;
            height: 7px;
            border-radius: 50%;
            background: var(--marca-coral);
        }

        .previa-pieza
        {
            display: block;
            width: 100%;
            height: auto;
            border-radius: var(--radio);
        }

        .previa-datos
        {
            display: flex;
            flex-wrap: wrap;
            gap: 6px 28px;
            margin-top: 14px;
            font-size: .82rem;
            color: rgba(252, 252, 247, .6);
        }

        .previa-datos b
        {
            margin-right: 6px;
            font-family: var(--fuente-mono);
            font-size: 1rem;
            font-weight: 700;
            color: var(--marca-dorado);
        }

        .seccion
        {
            padding: 76px 0;
        }

        .seccion-titulo
        {
            max-width: 60ch;
            margin: 0 0 36px;
        }

        .seccion-titulo h2
        {
            text-wrap: balance;
        }

        .seccion-titulo p
        {
            color: var(--texto-suave);
            font-size: 1.03rem;
            margin-bottom: 0;
        }

        .valores
        {
            display: grid;
            grid-template-columns: 1fr 1fr;
            column-gap: 56px;
        }

        .valor
        {
            display: grid;
            grid-template-columns: 11rem 1fr;
            gap: 20px;
            padding: 22px 0;
            border-top: 1px solid var(--borde-fuerte);
        }

        .valor h3
        {
            font-size: 1.05rem;
            margin: 0;
        }

        .valor p
        {
            margin: 0;
            color: var(--texto-suave);
            max-width: 46ch;
        }

        .pasos
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(215px, 1fr));
            gap: 0 32px;
        }

        .paso
        {
            padding-top: 16px;
            border-top: 2px solid var(--acento);
        }

        .paso .n
        {
            display: block;
            margin-bottom: 6px;
            font-family: var(--fuente-mono);
            font-size: .82rem;
            font-weight: 700;
            color: var(--acento-texto);
        }

        .franja
        {
            background: var(--marca-azul);
            color: var(--marca-perla);
            padding: 60px 0;
        }

        .franja h2
        {
            color: var(--marca-perla);
            max-width: 24ch;
            margin-bottom: 32px;
        }

        .franja h2 em
        {
            color: var(--marca-dorado);
            font-style: normal;
        }

        .comparativa
        {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            border-top: 1px solid rgba(252, 252, 247, .2);
        }

        .comparativa > div
        {
            padding: 22px 28px 4px 0;
        }

        .comparativa > div + div
        {
            padding-left: 28px;
            border-left: 1px solid rgba(252, 252, 247, .14);
        }

        .comparativa .destacada
        {
            border-top: 3px solid var(--marca-dorado);
            margin-top: -2px;
        }

        .comparativa h3
        {
            font-size: 1rem;
            color: var(--marca-perla);
        }

        .comparativa .destacada h3
        {
            color: var(--marca-dorado);
        }

        .comparativa p
        {
            color: rgba(252, 252, 247, .78);
            font-size: .92rem;
            margin: 0;
            max-width: 38ch;
        }

        .segmentos
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 0 40px;
        }

        .segmento
        {
            padding-top: 16px;
            border-top: 1px solid var(--borde-fuerte);
        }

        .segmento p
        {
            color: var(--texto-suave);
            margin: 0;
        }

        .cierre
        {
            position: relative;
            padding: 64px 56px;
            background: #0B1428;
            color: var(--marca-perla);
        }

        .cierre .esquina { opacity: .7; }
        .cierre .esquina-si { top: 14px; left: 14px; }
        .cierre .esquina-sd { top: 14px; right: 14px; }
        .cierre .esquina-ii { bottom: 14px; left: 14px; }
        .cierre .esquina-id { bottom: 14px; right: 14px; }

        .cierre h2
        {
            color: var(--marca-perla);
            font-size: clamp(2rem, 4vw, 2.7rem);
            line-height: 1.14;
            max-width: 18ch;
            margin: 0 0 28px;
        }

        .cierre .btn-secundario
        {
            background: transparent;
            border-color: rgba(252, 252, 247, .32);
            color: var(--marca-perla);
        }

        .cierre .btn-secundario:hover
        {
            background: rgba(252, 252, 247, .08);
            border-color: rgba(252, 252, 247, .55);
        }

        @media (max-width: 880px)
        {
            .hero .contenedor { grid-template-columns: 1fr; gap: 34px; }
            .hero h1 { font-size: 2.2rem; }
            .valores { grid-template-columns: 1fr; }
            .valor { grid-template-columns: 1fr; gap: 6px; }
            .comparativa { grid-template-columns: 1fr; }
            .comparativa > div + div { padding-left: 0; border-left: 0; border-top: 1px solid rgba(252, 252, 247, .14); }
            .cierre { padding: 44px 28px; }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <section class="hero">
        <div class="contenedor">

            <div>
                <h1 data-i18n="inicio.titulo">Descubre lo que tus usuarios <em>realmente observan</em></h1>

                <p class="bajada" data-i18n="inicio.bajada">
                    Falke mide la atención visual real sobre tus productos digitales con
                    seguimiento ocular de precisión profesional. Tú realizas las pruebas con tus
                    propios usuarios; nosotros ponemos el dispositivo, el procesamiento y los informes.
                </p>

                <div class="fila">
                    <a class="btn btn-primario btn-grande" href="<%: ResolveUrl("~/Planes.aspx") %>" data-i18n="inicio.cta.planes">Ver los planes</a>
                    <a class="btn btn-secundario btn-grande" href="<%: ResolveUrl("~/Registro.aspx") %>" data-i18n="inicio.cta.demo">Pedir una demostración</a>
                </div>
            </div>

            <div class="previa marco-pieza" aria-hidden="true">
                <span class="esquina esquina-si"></span>
                <span class="esquina esquina-sd"></span>
                <span class="esquina esquina-ii"></span>
                <span class="esquina esquina-id"></span>
                <div class="previa-barra">
                    <span>Sesión N.&deg; 04 &middot; Falke</span>
                    <span class="previa-vivo">En vivo</span>
                </div>
                <svg class="previa-pieza" viewBox="0 0 480 240">
                    <defs>
                        <radialGradient id="calorFuerte">
                            <stop offset="0" stop-color="#F26157" stop-opacity=".95" />
                            <stop offset=".45" stop-color="#E8AF52" stop-opacity=".62" />
                            <stop offset="1" stop-color="#E8AF52" stop-opacity="0" />
                        </radialGradient>
                        <radialGradient id="calorSuave">
                            <stop offset="0" stop-color="#E8AF52" stop-opacity=".45" />
                            <stop offset="1" stop-color="#E8AF52" stop-opacity="0" />
                        </radialGradient>
                    </defs>

                    <rect width="480" height="240" fill="#132245" />

                    <rect width="480" height="26" fill="#FCFCF7" fill-opacity=".05" />
                    <rect x="16" y="9" width="34" height="8" rx="2" fill="#FCFCF7" fill-opacity=".24" />
                    <rect x="72" y="10" width="22" height="6" rx="2" fill="#FCFCF7" fill-opacity=".14" />
                    <rect x="102" y="10" width="26" height="6" rx="2" fill="#FCFCF7" fill-opacity=".14" />
                    <rect x="136" y="10" width="20" height="6" rx="2" fill="#FCFCF7" fill-opacity=".14" />

                    <rect x="32" y="56" width="236" height="14" rx="3" fill="#FCFCF7" fill-opacity=".26" />
                    <rect x="32" y="78" width="176" height="14" rx="3" fill="#FCFCF7" fill-opacity=".26" />
                    <rect x="32" y="108" width="226" height="6" rx="2" fill="#FCFCF7" fill-opacity=".12" />
                    <rect x="32" y="121" width="198" height="6" rx="2" fill="#FCFCF7" fill-opacity=".12" />
                    <rect x="32" y="134" width="146" height="6" rx="2" fill="#FCFCF7" fill-opacity=".12" />
                    <rect x="32" y="160" width="92" height="28" rx="4" fill="#E8AF52" fill-opacity=".18" stroke="#E8AF52" stroke-opacity=".55" />
                    <rect x="134" y="160" width="70" height="28" rx="4" fill="none" stroke="#FCFCF7" stroke-opacity=".18" />
                    <rect x="32" y="206" width="120" height="5" rx="2" fill="#FCFCF7" fill-opacity=".08" />

                    <circle cx="112" cy="68" r="66" fill="url(#calorFuerte)" />
                    <circle cx="214" cy="84" r="46" fill="url(#calorSuave)" />
                    <circle cx="80" cy="174" r="54" fill="url(#calorFuerte)" />
                    <circle cx="150" cy="128" r="34" fill="url(#calorSuave)" />
                    <circle cx="44" cy="14" r="22" fill="url(#calorSuave)" />

                    <polyline points="44,14 96,66 170,70 214,86 152,126 84,172" fill="none" stroke="#FCFCF7" stroke-opacity=".5" stroke-width="1" stroke-dasharray="3 3" />
                    <g fill="#FCFCF7" fill-opacity=".85">
                        <circle cx="44" cy="14" r="2.5" />
                        <circle cx="96" cy="66" r="3.5" />
                        <circle cx="170" cy="70" r="3" />
                        <circle cx="214" cy="86" r="2.5" />
                        <circle cx="152" cy="126" r="2.5" />
                        <circle cx="84" cy="172" r="4" />
                        <circle cx="60" cy="86" r="2" />
                        <circle cx="126" cy="74" r="2" />
                        <circle cx="104" cy="178" r="2" />
                    </g>

                    <g stroke="#FCFCF7" stroke-opacity=".5" stroke-dasharray="4 3">
                        <line x1="286" y1="63" x2="344" y2="63" />
                        <line x1="226" y1="174" x2="344" y2="174" />
                    </g>
                    <g>
                        <circle cx="278" cy="63" r="9" fill="#F26157" />
                        <circle cx="218" cy="174" r="9" fill="#F26157" />
                    </g>
                    <g font-family="Space Mono, Consolas, monospace" font-size="10" font-weight="700" fill="#FCFCF7" text-anchor="middle">
                        <text x="278" y="66.5">1</text>
                        <text x="218" y="177.5">2</text>
                    </g>
                    <g font-family="Space Mono, Consolas, monospace" font-size="9" fill="#FCFCF7" fill-opacity=".65" letter-spacing=".6">
                        <text x="350" y="60">TITULAR</text>
                        <text x="350" y="72" fill-opacity=".45">2,3 s de mirada</text>
                        <text x="350" y="171">BOTÓN</text>
                        <text x="350" y="183" fill-opacity=".45">0,9 s de mirada</text>
                    </g>
                </svg>

                <div class="previa-datos">
                    <span><b>18:42</b>duración</span>
                    <span><b>62%</b>atención activa</span>
                    <span><b>47</b>fijaciones</span>
                </div>
            </div>

        </div>
    </section>

    <section class="seccion">
        <div class="contenedor">

            <div class="seccion-titulo">
                <h2 data-i18n="inicio.problema.titulo">Las métricas que usas hoy no revelan dónde mira el usuario</h2>
                <p data-i18n="inicio.problema.bajada">
                    Los clics, el tiempo de sesión y las encuestas son información indirecta.
                    Ninguna muestra qué elemento capta la atención y cuál pasa desapercibido.
                </p>
            </div>

            <div class="valores">

                <article class="valor">
                    <h3 data-i18n="inicio.valor1.titulo">Evidencia real</h3>
                    <p data-i18n="inicio.valor1.texto">
                        Uso de hardware Tobii con infrarrojos que registra la
                        mirada con precisión de nivel profesional.
                    </p>
                </article>

                <article class="valor">
                    <h3 data-i18n="inicio.valor2.titulo">Tus usuarios, tu entorno</h3>
                    <p data-i18n="inicio.valor2.texto">
                        Las sesiones las realizas tú, con tus propios testers, sobre tu producto real, asegurandote un control total.
                    </p>
                </article>

                <article class="valor">
                    <h3 data-i18n="inicio.valor3.titulo">Informes que se entienden</h3>
                    <p data-i18n="inicio.valor3.texto">
                        Nadie necesita interpretar datos biométricos crudos: la plataforma
                        devuelve mapas y métricas listas para interpretar y utilizar.
                    </p>
                </article>

                <article class="valor">
                    <h3 data-i18n="inicio.valor4.titulo">Sin comprar equipamiento</h3>
                    <p data-i18n="inicio.valor4.texto">
                        El dispositivo Tobii es dado en préstamo con la suscripción y vuelve al
                        terminar el contrato. Sin inversión inicial.
                    </p>
                </article>

                <article class="valor">
                    <h3 data-i18n="inicio.valor5.titulo">Resultados en minutos</h3>
                    <p data-i18n="inicio.valor5.texto">
                        Al terminar la grabación, el procesamiento arranca solo. Los
                        análisis quedan disponibles en menos de cinco minutos.
                    </p>
                </article>

                <article class="valor">
                    <h3 data-i18n="inicio.valor6.titulo">Datos protegidos</h3>
                    <p data-i18n="inicio.valor6.texto">
                        Los datos de mirada se disocian de la identidad del tester al guardarse,
                        conforme a lo estipulado en la Ley N.&deg; 25.326.
                    </p>
                </article>

            </div>
        </div>
    </section>

    <section class="seccion" style="background: var(--superficie-2); border-block: 1px solid var(--borde);">
        <div class="contenedor">

            <div class="seccion-titulo">
                <h2 data-i18n="inicio.comoFunciona.titulo">¿Cómo funciona Falke?</h2>
                <p data-i18n="inicio.comoFunciona.bajada">Cuatro pasos entre la firma del contrato y el primer informe.</p>
            </div>

            <div class="pasos">
                <div class="paso">
                    <span class="n" aria-hidden="true">1</span>
                    <h3 data-i18n="inicio.paso1.titulo">Recibo del dispositivo</h3>
                    <p class="texto-suave" data-i18n="inicio.paso1.texto">
                        Enviamos el Tobii en préstamo con su guía de instalación. Llega dentro de
                        los diez días de firmado del contrato.
                    </p>
                </div>
                <div class="paso">
                    <span class="n" aria-hidden="true">2</span>
                    <h3 data-i18n="inicio.paso2.titulo">Configurado del análisis</h3>
                    <p class="texto-suave" data-i18n="inicio.paso2.texto">
                        Registras la categoría del activo a evaluar (software, aplicación web, móvil,
                        videojuego o publicidad).
                    </p>
                </div>
                <div class="paso">
                    <span class="n" aria-hidden="true">3</span>
                    <h3 data-i18n="inicio.paso3.titulo">Grabas la sesión</h3>
                    <p class="texto-suave" data-i18n="inicio.paso3.texto">
                        La pantalla y el seguimiento ocular se graban sincronizados. Puedes pausar,
                        reanudar o reiniciar en cualquier momento.
                    </p>
                </div>
                <div class="paso">
                    <span class="n" aria-hidden="true">4</span>
                    <h3 data-i18n="inicio.paso4.titulo">Auditas los resultados</h3>
                    <p class="texto-suave" data-i18n="inicio.paso4.texto">
                        Mapa de calor, zonas de interés y ciegas, dispersión, recorrido y atención
                        frente a distracción. Todo exportable a PDF.
                    </p>
                </div>
            </div>

        </div>
    </section>

    <section class="franja">
        <div class="contenedor">

            <h2 data-i18n="inicio.comparativa.titulo">Precisión profesional, <em>al alcance de tu empresa</em></h2>

            <div class="comparativa">
                <div>
                    <h3 data-i18n="inicio.comparativa.webcam.titulo">Soluciones por cámara web</h3>
                    <p data-i18n="inicio.comparativa.webcam.texto">
                        Económicas, pero la precisión depende de la postura y la iluminación, y
                        limitan la duración del estudio.
                    </p>
                </div>
                <div class="destacada">
                    <h3 data-i18n="inicio.comparativa.falke.titulo">Falke</h3>
                    <p data-i18n="inicio.comparativa.falke.texto">
                        Infrarrojo dedicado, sin límite de duración ni de tipo de activo,
                        bajo suscripción y con el hardware incluido.
                    </p>
                </div>
                <div>
                    <h3 data-i18n="inicio.comparativa.propio.titulo">Equipo propio</h3>
                    <p data-i18n="inicio.comparativa.propio.texto">
                        La misma calidad de dato, pero exige comprar equipos, licencias y
                        formar gente antes del primer estudio.
                    </p>
                </div>
            </div>

        </div>
    </section>

    <section class="seccion">
        <div class="contenedor">

            <div class="seccion-titulo">
                <h2 data-i18n="inicio.paraQuien.titulo">Pensado para ti</h2>
            </div>

            <div class="segmentos">
                <article class="segmento">
                    <h3 data-i18n="inicio.segmento1.titulo">Estudios de videojuegos</h3>
                    <p data-i18n="inicio.segmento1.texto">
                        Validar tutoriales, HUD y elementos críticos de escena antes del lanzamiento.
                    </p>
                </article>
                <article class="segmento">
                    <h3 data-i18n="inicio.segmento2.titulo">Agencias de publicidad digital</h3>
                    <p data-i18n="inicio.segmento2.texto">
                        Demostrar con evidencia si la pieza dirige la atención al mensaje que importa.
                    </p>
                </article>
                <article class="segmento">
                    <h3 data-i18n="inicio.segmento3.titulo">Empresas de software</h3>
                    <p data-i18n="inicio.segmento3.texto">
                        Detectar problemas de usabilidad durante el desarrollo y no después del lanzamiento.
                    </p>
                </article>
            </div>

        </div>
    </section>

    <section class="contenedor">
        <div class="cierre marco-pieza">
            <span class="esquina esquina-si" aria-hidden="true"></span>
            <span class="esquina esquina-sd" aria-hidden="true"></span>
            <span class="esquina esquina-ii" aria-hidden="true"></span>
            <span class="esquina esquina-id" aria-hidden="true"></span>

            <h2 data-i18n="inicio.cierre.titulo">Empieza a decidir con evidencia</h2>

            <div class="fila">
                <a class="btn btn-primario btn-grande" href="<%: ResolveUrl("~/Planes.aspx") %>" data-i18n="inicio.cta.planes">Ver los planes</a>
                <a class="btn btn-secundario btn-grande" href="<%: ResolveUrl("~/Faq.aspx") %>" data-i18n="inicio.cta.faq">Leer las preguntas frecuentes</a>
            </div>
        </div>
    </section>

</asp:Content>
