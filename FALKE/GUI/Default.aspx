<%@ Page Language="C#" MasterPageFile="~/Publico.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="GUI.Inicio" Title="[[default_inicio]]" %>

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
                <h1 data-i18n="inicio.titulo">[[default_descubre_lo_que_tus_usuarios]] <em>[[default_realmente_observan]]</em></h1>

                <p class="bajada" data-i18n="inicio.bajada">
                    [[default_falke_mide_la_atencion_visual_real]]
                </p>

                <div class="fila">
                    <a class="btn btn-primario btn-grande" href="<%: ResolveUrl("~/Planes.aspx") %>" data-i18n="inicio.cta.planes">[[default_ver_los_planes]]</a>
                    <a class="btn btn-secundario btn-grande" href="<%: ResolveUrl("~/Registro.aspx") %>" data-i18n="inicio.cta.demo">[[default_pedir_una_demostracion]]</a>
                </div>
            </div>

            <div class="previa marco-pieza" aria-hidden="true">
                <span class="esquina esquina-si"></span>
                <span class="esquina esquina-sd"></span>
                <span class="esquina esquina-ii"></span>
                <span class="esquina esquina-id"></span>
                <div class="previa-barra">
                    <span>Sesión N.&deg; 04 &middot; Falke</span>
                    <span class="previa-vivo">[[default_en_vivo]]</span>
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
                    <span><b>18:42</b>[[default_duracion]]</span>
                    <span><b>62%</b>[[default_atencion_activa]]</span>
                    <span><b>47</b>[[default_fijaciones]]</span>
                </div>
            </div>

        </div>
    </section>

    <section class="seccion">
        <div class="contenedor">

            <div class="seccion-titulo">
                <h2 data-i18n="inicio.problema.titulo">[[default_las_metricas_que_usas_hoy_no]]</h2>
                <p data-i18n="inicio.problema.bajada">
                    [[default_los_clics_el_tiempo_de_sesion]]
                </p>
            </div>

            <div class="valores">

                <article class="valor">
                    <h3 data-i18n="inicio.valor1.titulo">[[default_evidencia_real]]</h3>
                    <p data-i18n="inicio.valor1.texto">
                        [[default_uso_de_hardware_tobii_con_infrarrojos]]
                    </p>
                </article>

                <article class="valor">
                    <h3 data-i18n="inicio.valor2.titulo">[[default_tus_usuarios_tu_entorno]]</h3>
                    <p data-i18n="inicio.valor2.texto">
                        [[default_las_sesiones_las_realizas_tu_con]]
                    </p>
                </article>

                <article class="valor">
                    <h3 data-i18n="inicio.valor3.titulo">[[default_informes_que_se_entienden]]</h3>
                    <p data-i18n="inicio.valor3.texto">
                        [[default_nadie_necesita_interpretar_datos_biometr]]
                    </p>
                </article>

                <article class="valor">
                    <h3 data-i18n="inicio.valor4.titulo">[[default_sin_comprar_equipamiento]]</h3>
                    <p data-i18n="inicio.valor4.texto">
                        [[default_el_dispositivo_tobii_es_dado_en]]
                    </p>
                </article>

                <article class="valor">
                    <h3 data-i18n="inicio.valor5.titulo">[[default_resultados_en_minutos]]</h3>
                    <p data-i18n="inicio.valor5.texto">
                        [[default_al_terminar_la_grabacion_el_procesamient]]
                    </p>
                </article>

                <article class="valor">
                    <h3 data-i18n="inicio.valor6.titulo">[[default_datos_protegidos]]</h3>
                    <p data-i18n="inicio.valor6.texto">
                        [[default_los_datos_de_mirada_se_disocian]]
                    </p>
                </article>

            </div>
        </div>
    </section>

    <section class="seccion" style="background: var(--superficie-2); border-block: 1px solid var(--borde);">
        <div class="contenedor">

            <div class="seccion-titulo">
                <h2 data-i18n="inicio.comoFunciona.titulo">[[default_como_funciona_falke]]</h2>
                <p data-i18n="inicio.comoFunciona.bajada">[[default_cuatro_pasos_entre_la_firma_del]]</p>
            </div>

            <div class="pasos">
                <div class="paso">
                    <span class="n" aria-hidden="true">1</span>
                    <h3 data-i18n="inicio.paso1.titulo">[[default_recibo_del_dispositivo]]</h3>
                    <p class="texto-suave" data-i18n="inicio.paso1.texto">
                        [[default_enviamos_el_tobii_en_prestamo_con]]
                    </p>
                </div>
                <div class="paso">
                    <span class="n" aria-hidden="true">2</span>
                    <h3 data-i18n="inicio.paso2.titulo">[[default_configurado_del_analisis]]</h3>
                    <p class="texto-suave" data-i18n="inicio.paso2.texto">
                        [[default_registras_la_categoria_del_activo_a]]
                    </p>
                </div>
                <div class="paso">
                    <span class="n" aria-hidden="true">3</span>
                    <h3 data-i18n="inicio.paso3.titulo">[[default_grabas_la_sesion]]</h3>
                    <p class="texto-suave" data-i18n="inicio.paso3.texto">
                        [[default_la_pantalla_y_el_seguimiento_ocular]]
                    </p>
                </div>
                <div class="paso">
                    <span class="n" aria-hidden="true">4</span>
                    <h3 data-i18n="inicio.paso4.titulo">[[default_auditas_los_resultados]]</h3>
                    <p class="texto-suave" data-i18n="inicio.paso4.texto">
                        [[default_mapa_de_calor_zonas_de_interes]]
                    </p>
                </div>
            </div>

        </div>
    </section>

    <section class="franja">
        <div class="contenedor">

            <h2 data-i18n="inicio.comparativa.titulo">[[default_precision_profesional]] <em>[[default_al_alcance_de_tu_empresa]]</em></h2>

            <div class="comparativa">
                <div>
                    <h3 data-i18n="inicio.comparativa.webcam.titulo">[[default_soluciones_por_camara_web]]</h3>
                    <p data-i18n="inicio.comparativa.webcam.texto">
                        [[default_economicas_pero_la_precision_depende_de]]
                    </p>
                </div>
                <div class="destacada">
                    <h3 data-i18n="inicio.comparativa.falke.titulo">Falke</h3>
                    <p data-i18n="inicio.comparativa.falke.texto">
                        [[default_infrarrojo_dedicado_sin_limite_de_duraci]]
                    </p>
                </div>
                <div>
                    <h3 data-i18n="inicio.comparativa.propio.titulo">[[default_equipo_propio]]</h3>
                    <p data-i18n="inicio.comparativa.propio.texto">
                        [[default_la_misma_calidad_de_dato_pero]]
                    </p>
                </div>
            </div>

        </div>
    </section>

    <section class="seccion">
        <div class="contenedor">

            <div class="seccion-titulo">
                <h2 data-i18n="inicio.paraQuien.titulo">[[default_pensado_para_ti]]</h2>
            </div>

            <div class="segmentos">
                <article class="segmento">
                    <h3 data-i18n="inicio.segmento1.titulo">[[default_estudios_de_videojuegos]]</h3>
                    <p data-i18n="inicio.segmento1.texto">
                        [[default_validar_tutoriales_hud_y_elementos_criti]]
                    </p>
                </article>
                <article class="segmento">
                    <h3 data-i18n="inicio.segmento2.titulo">[[default_agencias_de_publicidad_digital]]</h3>
                    <p data-i18n="inicio.segmento2.texto">
                        [[default_demostrar_con_evidencia_si_la_pieza]]
                    </p>
                </article>
                <article class="segmento">
                    <h3 data-i18n="inicio.segmento3.titulo">[[default_empresas_de_software]]</h3>
                    <p data-i18n="inicio.segmento3.texto">
                        [[default_detectar_problemas_de_usabilidad_durante]]
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

            <h2 data-i18n="inicio.cierre.titulo">[[default_empieza_a_decidir_con_evidencia]]</h2>

            <div class="fila">
                <a class="btn btn-primario btn-grande" href="<%: ResolveUrl("~/Planes.aspx") %>" data-i18n="inicio.cta.planes">[[default_ver_los_planes]]</a>
                <a class="btn btn-secundario btn-grande" href="<%: ResolveUrl("~/Faq.aspx") %>" data-i18n="inicio.cta.faq">[[default_leer_las_preguntas_frecuentes]]</a>
            </div>
        </div>
    </section>

</asp:Content>
