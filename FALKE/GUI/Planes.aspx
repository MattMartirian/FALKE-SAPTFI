<%@ Page Language="C#" MasterPageFile="~/Publico.master" AutoEventWireup="true" CodeFile="Planes.aspx.cs" Inherits="GUI.Planes" Title="[[planes_planes]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .planes-cabecera
        {
            padding: 56px 0 32px;
        }

        .planes-cabecera p
        {
            color: var(--texto-suave);
            max-width: 60ch;
            margin: 0;
        }

        .selector-ciclo
        {
            display: inline-flex;
            gap: 4px;
            padding: 4px;
            border: 1px solid var(--borde);
            border-radius: var(--radio);
            background: var(--superficie);
            margin-top: 26px;
        }

        .selector-ciclo button
        {
            border: 0;
            background: transparent;
            padding: 8px 20px;
            border-radius: var(--radio-sm);
            font-family: inherit;
            font-size: .88rem;
            font-weight: 600;
            color: var(--texto-suave);
            cursor: pointer;
        }

        .selector-ciclo button[aria-pressed="true"]
        {
            background: var(--marca-azul);
            color: var(--marca-perla);
        }

        html[data-tema="noche"] .selector-ciclo button[aria-pressed="true"]
        {
            background: var(--acento);
            color: var(--marca-azul);
        }

        .planes
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(285px, 1fr));
            gap: 20px;
            align-items: start;
        }

        .plan
        {
            position: relative;
            display: flex;
            flex-direction: column;
            height: 100%;
        }

        .plan.destacado
        {
            border-color: var(--acento);
            box-shadow: var(--sombra);
        }

        .plan-cinta
        {
            position: absolute;
            top: -12px;
            left: 50%;
            transform: translateX(-50%);
            background: var(--acento);
            color: var(--marca-azul);
            font-size: .8rem;
            font-weight: 700;
            padding: 3px 12px;
            border-radius: var(--radio-sm);
            white-space: nowrap;
        }

        .plan h2
        {
            font-size: 1.3rem;
            margin-bottom: 2px;
        }

        .plan .para-quien
        {
            color: var(--texto-suave);
            font-size: .87rem;
            min-height: 3.4em;
        }

        .plan-precio
        {
            display: flex;
            align-items: baseline;
            gap: 6px;
            margin: 18px 0 2px;
        }

        .plan-precio .monto
        {
            font-size: 2.1rem;
            font-weight: 700;
            line-height: 1;
        }

        .plan-precio .periodo
        {
            color: var(--texto-suave);
            font-size: .9rem;
        }

        .plan-facturacion
        {
            font-size: .8rem;
            color: var(--texto-suave);
            min-height: 2.6em;
        }

        .plan ul
        {
            list-style: none;
            margin: 20px 0;
            padding: 0;
            display: grid;
            gap: 10px;
            flex: 1;
        }

        .plan li
        {
            display: flex;
            gap: 10px;
            font-size: .88rem;
            align-items: flex-start;
        }

        .plan li svg
        {
            flex: none;
            margin-top: 2px;
            color: var(--exito);
        }

        .plan li.no-incluye
        {
            color: var(--texto-tenue);
        }

        .plan li.no-incluye svg
        {
            color: var(--borde-fuerte);
        }

        .comparador
        {
            margin-top: 64px;
        }

        .comparador .tabla { table-layout: fixed; }
        .comparador .tabla th:first-child,
        .comparador .tabla td:first-child { width: 34%; }
        .comparador .tabla .plan-col,
        .comparador .tabla td.centro { width: 22%; }

        .comparador .tabla thead th,
        .comparador .tabla tbody td { text-align: center; }

        .comparador .tabla thead th:first-child,
        .comparador .tabla tbody th { text-align: left; }

        .comparador td.si svg { color: var(--exito); }
        .comparador td.no    { color: var(--texto-tenue); }

        @media (max-width: 640px)
        {
            .comparador .tabla { table-layout: auto; }
            .comparador .tabla th:first-child,
            .comparador .tabla td:first-child { width: auto; }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="contenedor">

        <header class="planes-cabecera">
            <h1 data-i18n="planes.titulo">[[planes_planes_de_suscripcion]]</h1>
            <p data-i18n="planes.bajada">
                [[planes_todos_los_planes_incluyen_el_dispositivo]]
            </p>

            <div class="selector-ciclo" role="group" aria-label="[[planes_ciclo_de_facturacion]]">
                <button type="button" id="btnMensual" aria-pressed="true" data-i18n="planes.mensual">[[empresanueva_mensual]]</button>
                <button type="button" id="btnAnual" aria-pressed="false" data-i18n="planes.anual">[[planes_anual_20_menos]]</button>
            </div>
        </header>

        <div class="planes">

            <article class="tarjeta plan">
                <h2 data-i18n="planes.scout.nombre">Falke Scout</h2>
                <p class="para-quien" data-i18n="planes.scout.paraQuien">
                    [[planes_para_equipos_que_incorporan_el_seguimien]]
                </p>

                <div class="plan-precio">
                    <span class="monto" data-precio-mensual="USD 590" data-precio-anual="USD 470">USD 590</span>
                    <span class="periodo" data-i18n="planes.porMes">[[planes_mes]]</span>
                </div>
                <p class="plan-facturacion" data-factura-mensual="Facturacion mensual. Un dispositivo en prestamo."
                                            data-factura-anual="Facturacion anual: USD 5.640 por ano. Un dispositivo en prestamo.">
                    [[planes_facturacion_mensual_un_dispositivo_en_pr]]
                </p>

                <ul>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.scout.item1">[[planes_un_dispositivo_tobii_eye_tracker_en]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.scout.item2">[[planes_sesiones_de_analisis_individuales_ilimit]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.scout.item3">[[planes_las_6_visualizaciones_analiticas_por_ses]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.scout.item4">[[planes_metricas_estadisticas_y_exportacion_a_pd]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.scout.item5">[[planes_soporte_tecnico_remoto_en_horario_comerc]]</span></li>
                    <li class="no-incluye"><svg width="17" height="17" aria-hidden="true"><use href="#i-cerrar" /></svg><span data-i18n="planes.scout.item6">[[planes_sin_analisis_multiple]]</span></li>
                </ul>

                <button type="button" class="btn btn-secundario btn-bloque"
                        data-abre-modal="modalContacto" data-plan="Falke Scout" data-i18n="planes.contratar">
                    [[planes_quiero_este_plan]]
                </button>
            </article>

            <article class="tarjeta plan destacado">
                <span class="plan-cinta" data-i18n="planes.masElegido">[[planes_el_mas_elegido]]</span>

                <h2 data-i18n="planes.hunter.nombre">Falke Hunter</h2>
                <p class="para-quien" data-i18n="planes.hunter.paraQuien">
                    [[planes_para_equipos_que_usan_el_servicio]]
                </p>

                <div class="plan-precio">
                    <span class="monto" data-precio-mensual="USD 950" data-precio-anual="USD 760">USD 950</span>
                    <span class="periodo" data-i18n="planes.porMes">[[planes_mes]]</span>
                </div>
                <p class="plan-facturacion" data-factura-mensual="Facturacion mensual. Hasta 3 dispositivos en prestamo."
                                            data-factura-anual="Facturacion anual: USD 9.120 por ano. Hasta 3 dispositivos en prestamo.">
                    [[planes_facturacion_mensual_hasta_3_dispositivos]]
                </p>

                <ul>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item1">[[planes_todo_lo_del_plan_scout]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item2">[[planes_analisis_multiple_promedio_de_hasta_10]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item3">[[planes_informes_comparativos_entre_sesiones]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item4">[[planes_exportacion_de_los_datos_crudos_de]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item5">[[planes_soporte_prioritario_con_tiempo_de_respue]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item6">[[planes_hasta_3_dispositivos_simultaneos]]</span></li>
                </ul>

                <button type="button" class="btn btn-primario btn-bloque"
                        data-abre-modal="modalContacto" data-plan="Falke Hunter" data-i18n="planes.contratar">
                    [[planes_quiero_este_plan]]
                </button>
            </article>

            <article class="tarjeta plan">
                <h2 data-i18n="planes.apex.nombre">Falke Apex</h2>
                <p class="para-quien" data-i18n="planes.apex.paraQuien">
                    [[planes_para_organizaciones_con_varios_equipos_p]]
                </p>

                <div class="plan-precio">
                    <span class="monto" data-precio-mensual="A convenir" data-precio-anual="A convenir">[[planes_a_convenir]]</span>
                </div>
                <p class="plan-facturacion" data-factura-mensual="Condiciones segun volumen de dispositivos y perfil de uso."
                                            data-factura-anual="Condiciones segun volumen de dispositivos y perfil de uso.">
                    [[planes_condiciones_segun_volumen_de_dispositivo]]
                </p>

                <ul>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.apex.item1">[[planes_todo_lo_del_plan_hunter]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.apex.item2">[[planes_cantidad_de_dispositivos_a_convenir]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.apex.item3">[[planes_usuarios_adicionales_dentro_de_la_organi]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.apex.item4">[[planes_condiciones_de_contrato_y_soporte_a]]</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.apex.item5">[[planes_acompanamiento_en_la_puesta_en_marcha]]</span></li>
                </ul>

                <button type="button" class="btn btn-secundario btn-bloque"
                        data-abre-modal="modalContacto" data-plan="Falke Apex" data-i18n="planes.contactar">
                    [[planes_hablar_con_ventas]]
                </button>
            </article>

        </div>

        <div class="aviso aviso-info mt-24">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen" data-i18n="planes.aviso">
                [[planes_todos_los_planes_incluyen_seguridad_y]]
            </p>
        </div>

        <section class="comparador">
            <h2 class="mb-24" data-i18n="planes.comparador.titulo">[[planes_comparacion_detallada]]</h2>

            <div class="tabla-scroll">
                <table class="tabla">
                    <caption class="solo-lectores">[[planes_comparacion_de_funciones_incluidas_en_ca]]</caption>
                    <thead>
                        <tr>
                            <th scope="col" data-i18n="planes.comparador.funcion">[[planes_funcion]]</th>
                            <th scope="col" class="plan-col">Scout</th>
                            <th scope="col" class="plan-col">Hunter</th>
                            <th scope="col" class="plan-col">Apex</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.dispositivo">[[miempresa_dispositivos_en_prestamo]]</th>
                            <td class="centro">1</td>
                            <td class="centro">3</td>
                            <td class="centro" data-i18n="planes.comparador.aConvenir">[[planes_a_convenir]]</td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.sesiones">[[planes_sesiones_individuales]]</th>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.visualizaciones">[[planes_6_visualizaciones_analiticas]]</th>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.multiple">[[analisismultiple_analisis_multiple]]</th>
                            <td class="centro no" data-i18n="planes.comparador.noIncluido">[[planes_no_incluido]]</td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.crudos">[[planes_exportar_datos_crudos]]</th>
                            <td class="centro no" data-i18n="planes.comparador.noIncluido">[[planes_no_incluido]]</td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.soporte">[[planes_soporte_prioritario]]</th>
                            <td class="centro no" data-i18n="planes.comparador.noIncluido">[[planes_no_incluido]]</td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.historial">[[planes_historial_y_auditoria_de_sesiones]]</th>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">[[planes_incluido]]</span></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </section>

    </div>

    <div class="modal-fondo" id="modalContacto" role="dialog" aria-modal="true" aria-labelledby="tituloContacto" hidden>
        <div class="modal ancho">

            <div class="modal-cabecera">
                <div>
                    <h2 id="tituloContacto" data-i18n="contacto.titulo">[[planes_hablemos_de_tu_implementacion]]</h2>
                    <p class="subtitulo sin-margen" data-i18n="contacto.subtitulo">
                        [[planes_dejanos_tus_datos_y_un_ejecutivo]]
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <div class="campo">
                    <label for="ctPlan" data-i18n="contacto.plan">[[planes_plan_de_interes]]</label>
                    <input type="text" class="entrada" id="ctPlan" value="Falke Hunter" readonly />
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="ctNombre" data-i18n="contacto.nombre">[[dispositivos_nombre]]</label>
                        <input type="text" class="entrada" id="ctNombre" data-foco-inicial
                               placeholder="Maria" data-i18n-attr="placeholder:contacto.nombre.placeholder" />
                    </div>
                    <div class="campo">
                        <label for="ctApellido" data-i18n="contacto.apellido">[[empresanueva_apellido]]</label>
                        <input type="text" class="entrada" id="ctApellido"
                               placeholder="Gomez" data-i18n-attr="placeholder:contacto.apellido.placeholder" />
                    </div>
                </div>

                <div class="campo">
                    <label for="ctEmpresa" data-i18n="contacto.empresa">[[bitacora_empresa]]</label>
                    <input type="text" class="entrada" id="ctEmpresa"
                           placeholder="[[planes_nombre_de_tu_organizacion]]" data-i18n-attr="placeholder:contacto.empresa.placeholder" />
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="ctEmail" data-i18n="contacto.email">[[planes_correo_de_contacto]]</label>
                        <input type="email" class="entrada" id="ctEmail"
                               placeholder="maria@empresa.com" data-i18n-attr="placeholder:contacto.email.placeholder" />
                    </div>
                    <div class="campo">
                        <label for="ctTelefono" data-i18n="contacto.telefono">[[planes_telefono]]</label>
                        <input type="tel" class="entrada" id="ctTelefono"
                               placeholder="+54 11 0000-0000" data-i18n-attr="placeholder:contacto.telefono.placeholder" />
                    </div>
                </div>

                <div class="campo">
                    <label for="ctMensaje" data-i18n="contacto.mensaje">[[planes_mensaje]]</label>

                    <textarea class="entrada" id="ctMensaje" rows="4"></textarea>
                    <p class="ayuda" data-i18n="contacto.ayuda">[[planes_el_mensaje_viene_escrito_por_defecto]]</p>
                </div>

                <label class="casilla-simple">
                    <input type="checkbox" id="ctConsentimiento" />
                    <span data-i18n="contacto.consentimiento">
                        [[planes_acepto_que_pattern_blue_use_estos]]
                    </span>
                </label>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">[[categorias_cancelar]]</button>
                <button type="button" class="btn btn-primario" data-i18n="contacto.enviar">[[planes_enviar_consulta]]</button>
            </div>

        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var btnMensual = document.getElementById("btnMensual");
            var btnAnual = document.getElementById("btnAnual");

            function mostrarCiclo(ciclo) {
                var montos = document.querySelectorAll("[data-precio-mensual]");
                var facturas = document.querySelectorAll("[data-factura-mensual]");
                var i;

                for (i = 0; i < montos.length; i++) {
                    montos[i].textContent = montos[i].getAttribute("data-precio-" + ciclo);
                }

                for (i = 0; i < facturas.length; i++) {
                    facturas[i].textContent = facturas[i].getAttribute("data-factura-" + ciclo);
                }

                btnMensual.setAttribute("aria-pressed", ciclo === "mensual" ? "true" : "false");
                btnAnual.setAttribute("aria-pressed", ciclo === "anual" ? "true" : "false");
            }

            btnMensual.addEventListener("click", function () { mostrarCiclo("mensual"); });
            btnAnual.addEventListener("click", function () { mostrarCiclo("anual"); });

            var botonesPlan = document.querySelectorAll("[data-plan]");
            var campoPlan = document.getElementById("ctPlan");
            var campoMensaje = document.getElementById("ctMensaje");

            function armarMensaje(plan) {
                return "[[js:planes_hola_me_interesa_comunicarme_con_ustedes]]".replace("{0}", plan);
            }

            for (var i = 0; i < botonesPlan.length; i++) {
                botonesPlan[i].addEventListener("click", function () {
                    var plan = this.getAttribute("data-plan");

                    campoPlan.value = plan;
                    campoMensaje.value = armarMensaje(plan);
                });
            }

            campoMensaje.value = armarMensaje(campoPlan.value);
        })();
    </script>
</asp:Content>
