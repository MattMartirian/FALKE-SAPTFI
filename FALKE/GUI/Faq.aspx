<%@ Page Language="C#" MasterPageFile="~/Publico.master" AutoEventWireup="true" CodeFile="Faq.aspx.cs" Inherits="GUI.Faq" Title="[[faq_preguntas_frecuentes]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .faq-cabecera
        {
            max-width: 860px;
            margin: 0 auto;
            padding: 56px 0 28px;
        }

        .faq-cabecera p
        {
            color: var(--texto-suave);
            max-width: 58ch;
            margin: 0;
        }

        .faq-herramientas
        {
            display: flex;
            flex-wrap: wrap;
            gap: 14px;
            align-items: center;
            max-width: 860px;
            margin: 26px auto 30px;
        }

        .faq-buscador
        {
            position: relative;
            width: 100%;
            max-width: 380px;
        }

        .faq-grupo
        {
            max-width: 860px;
            margin: 0 auto 34px;
        }

        .faq-grupo > h2
        {
            font-size: 1.2rem;
            color: var(--texto);
            margin-bottom: 12px;
        }

        .faq-sin-resultados
        {
            max-width: 860px;
            margin: 0 auto;
        }

        .faq-contacto
        {
            max-width: 860px;
            margin: 48px auto 0;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="contenedor">

        <header class="faq-cabecera">
            <h1 data-i18n="faq.titulo">[[faq_preguntas_frecuentes]]</h1>
            <p data-i18n="faq.bajada">
                [[faq_todo_lo_que_suelen_preguntarnos_antes]]
            </p>

            <div class="faq-herramientas">
                <div class="faq-buscador campo-buscar">
                    <label class="solo-lectores" for="faqBuscar" data-i18n="faq.buscarEtiqueta">[[faq_buscar_en_las_preguntas_frecuentes]]</label>
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-buscar" /></svg>
                    <input type="search" class="entrada" id="faqBuscar"
                           placeholder="[[faq_buscar_una_pregunta]]" data-i18n-attr="placeholder:faq.buscarPlaceholder" />
                </div>
            </div>

            <div class="chips" data-unico style="justify-content: center;">
                <button type="button" class="chip" aria-pressed="true" data-filtro="todas" data-i18n="faq.filtro.todas">[[empresas_todas]]</button>
                <button type="button" class="chip" aria-pressed="false" data-filtro="servicio" data-i18n="faq.filtro.servicio">[[faq_el_servicio]]</button>
                <button type="button" class="chip" aria-pressed="false" data-filtro="contratacion" data-i18n="faq.filtro.contratacion">[[faq_contratacion]]</button>
                <button type="button" class="chip" aria-pressed="false" data-filtro="plataforma" data-i18n="faq.filtro.plataforma">[[categorias_plataforma]]</button>
                <button type="button" class="chip" aria-pressed="false" data-filtro="datos" data-i18n="faq.filtro.datos">[[faq_datos_y_seguridad]]</button>
                <button type="button" class="chip" aria-pressed="false" data-filtro="hardware" data-i18n="faq.filtro.hardware">[[faq_hardware_y_soporte]]</button>
            </div>
        </header>

        <section class="faq-grupo" data-grupo="servicio">
            <h2 data-i18n="faq.grupo.servicio">[[faq_sobre_el_servicio]]</h2>

            <div class="acordeon">

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r1">
                            <span data-i18n="faq.p1.pregunta">[[faq_que_es_exactamente_falke]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r1">
                        <p data-i18n="faq.p1.respuesta">
                            [[faq_es_una_suite_de_analitica_conductual]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r2">
                            <span data-i18n="faq.p2.pregunta">[[faq_en_que_se_diferencia_del_eye]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r2">
                        <p data-i18n="faq.p2.respuesta">
                            [[faq_falke_usa_hardware_de_infrarrojo_dedicad]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r3">
                            <span data-i18n="faq.p3.pregunta">[[faq_que_tipos_de_activos_digitales_puedo]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r3">
                        <p data-i18n="faq.p3.respuesta">
                            [[faq_software_de_escritorio_aplicaciones_web]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r4">
                            <span data-i18n="faq.p4.pregunta">[[faq_ustedes_ponen_los_testers]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r4">
                        <p data-i18n="faq.p4.respuesta">
                            [[faq_no_el_modelo_de_falke_esta]]
                        </p>
                    </div>
                </div>

            </div>
        </section>

        <section class="faq-grupo" data-grupo="contratacion">
            <h2 data-i18n="faq.grupo.contratacion">[[faq_contratacion_y_facturacion]]</h2>

            <div class="acordeon">

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r5">
                            <span data-i18n="faq.p5.pregunta">[[faq_por_que_no_puedo_crear_una]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r5">
                        <p data-i18n="faq.p5.respuesta">
                            [[faq_porque_el_servicio_incluye_un_dispositiv]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r6">
                            <span data-i18n="faq.p6.pregunta">[[faq_cuanto_tarda_desde_que_firmo_hasta]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r6">
                        <p data-i18n="faq.p6.respuesta">
                            [[faq_el_objetivo_es_que_puedas_correr]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r7">
                            <span data-i18n="faq.p7.pregunta">[[faq_que_pasa_cuando_termina_el_contrato]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r7">
                        <p data-i18n="faq.p7.respuesta">
                            [[faq_el_dispositivo_vuelve_a_pattern_blue]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r8">
                            <span data-i18n="faq.p8.pregunta">[[faq_conviene_pagar_por_mes_o_por]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r8">
                        <p data-i18n="faq.p8.respuesta">
                            [[faq_el_pago_anual_anticipado_tiene_un]]
                        </p>
                    </div>
                </div>

            </div>
        </section>

        <section class="faq-grupo" data-grupo="plataforma">
            <h2 data-i18n="faq.grupo.plataforma">[[faq_uso_de_la_plataforma]]</h2>

            <div class="acordeon">

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r9">
                            <span data-i18n="faq.p9.pregunta">[[faq_que_visualizaciones_obtengo_de_cada_sesi]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r9">
                        <p data-i18n="faq.p9.respuesta">
                            [[faq_seis_la_grabacion_con_el_seguimiento]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r10">
                            <span data-i18n="faq.p10.pregunta">[[faq_cuanto_tarda_el_procesamiento]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r10">
                        <p data-i18n="faq.p10.respuesta">
                            [[faq_arranca_solo_al_terminar_la_grabacion]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r11">
                            <span data-i18n="faq.p11.pregunta">[[faq_puedo_combinar_varias_sesiones_en_un]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r11">
                        <p data-i18n="faq.p11.respuesta">
                            [[faq_si_con_el_analisis_multiple_planes]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r12">
                            <span data-i18n="faq.p12.pregunta">[[faq_en_que_idiomas_esta_la_plataforma]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r12">
                        <p data-i18n="faq.p12.respuesta">
                            [[faq_espanol_ingles_y_portugues_cada_usuario]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r13">
                            <span data-i18n="faq.p13.pregunta">[[faq_puedo_compartir_los_resultados_con_gente]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r13">
                        <p data-i18n="faq.p13.respuesta">
                            [[faq_si_cada_analisis_se_exporta_a]]
                        </p>
                    </div>
                </div>

            </div>
        </section>

        <section class="faq-grupo" data-grupo="datos">
            <h2 data-i18n="faq.grupo.datos">[[faq_datos_y_seguridad]]</h2>

            <div class="acordeon">

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r14">
                            <span data-i18n="faq.p14.pregunta">[[faq_que_pasa_con_los_datos_de]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r14">
                        <p data-i18n="faq.p14.respuesta">
                            [[faq_los_datos_de_seguimiento_ocular_se]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r15">
                            <span data-i18n="faq.p15.pregunta">[[faq_donde_se_guardan_las_grabaciones]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r15">
                        <p data-i18n="faq.p15.respuesta">
                            [[faq_en_la_infraestructura_propia_de_pattern]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r16">
                            <span data-i18n="faq.p16.pregunta">[[faq_quien_puede_ver_lo_que_hace]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r16">
                        <p data-i18n="faq.p16.respuesta">
                            [[faq_el_administrador_de_tu_empresa_ve]]
                        </p>
                    </div>
                </div>

            </div>
        </section>

        <section class="faq-grupo" data-grupo="hardware">
            <h2 data-i18n="faq.grupo.hardware">[[faq_hardware_y_soporte]]</h2>

            <div class="acordeon">

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r17">
                            <span data-i18n="faq.p17.pregunta">[[faq_que_necesito_en_mi_equipo]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r17">
                        <p data-i18n="faq.p17.respuesta">
                            [[faq_un_equipo_de_oficina_estandar_con]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r18">
                            <span data-i18n="faq.p18.pregunta">[[faq_y_si_el_dispositivo_falla]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r18">
                        <p data-i18n="faq.p18.respuesta">
                            [[faq_si_la_falla_no_es_atribuible]]
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r19">
                            <span data-i18n="faq.p19.pregunta">[[faq_hay_limite_de_duracion_en_las]]</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r19">
                        <p data-i18n="faq.p19.respuesta">
                            [[faq_no_puedes_grabar_una_sesion_tan]]
                        </p>
                    </div>
                </div>

            </div>
        </section>

        <div class="faq-sin-resultados vacio" id="faqVacio" hidden>
            <svg width="34" height="34" aria-hidden="true"><use href="#i-buscar" /></svg>
            <p class="sin-margen" data-i18n="faq.sinResultados">
                [[faq_no_encontramos_ninguna_pregunta_con_ese]]
            </p>
        </div>

        <div class="faq-contacto">
            <h2 data-i18n="faq.contacto.titulo">[[faq_te_quedo_alguna_duda]]</h2>
            <p class="texto-suave" data-i18n="faq.contacto.texto">
                [[faq_escribenos_desde_la_pantalla_de_planes]]
            </p>
            <a class="btn btn-primario" href="<%: ResolveUrl("~/Planes.aspx") %>" data-i18n="faq.contacto.boton">[[faq_ir_a_los_planes]]</a>
        </div>

    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var buscador = document.getElementById("faqBuscar");
            var vacio = document.getElementById("faqVacio");
            var chips = document.querySelectorAll("[data-filtro]");
            var grupos = document.querySelectorAll(".faq-grupo");
            var categoriaActiva = "todas";

            function normalizar(texto) {
                return texto.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "");
            }

            function filtrar() {
                var texto = normalizar(buscador.value.trim());
                var visibles = 0;

                for (var g = 0; g < grupos.length; g++) {
                    var grupo = grupos[g];
                    var coincideCategoria = categoriaActiva === "todas" || grupo.getAttribute("data-grupo") === categoriaActiva;
                    var items = grupo.querySelectorAll(".acordeon-item");
                    var visiblesEnGrupo = 0;

                    for (var i = 0; i < items.length; i++) {
                        var coincideTexto = texto === "" || normalizar(items[i].textContent).indexOf(texto) !== -1;
                        var mostrar = coincideCategoria && coincideTexto;

                        items[i].hidden = !mostrar;

                        if (mostrar) visiblesEnGrupo++;
                    }

                    grupo.hidden = visiblesEnGrupo === 0;
                    visibles += visiblesEnGrupo;
                }

                vacio.hidden = visibles > 0;
            }

            for (var i = 0; i < chips.length; i++) {
                chips[i].addEventListener("click", function () {
                    categoriaActiva = this.getAttribute("data-filtro");
                    filtrar();
                });
            }

            buscador.addEventListener("input", filtrar);
        })();
    </script>
</asp:Content>
