<%@ Page Language="C#" MasterPageFile="~/Publico.master" AutoEventWireup="true" CodeFile="Planes.aspx.cs" Inherits="GUI.Planes" Title="Planes" %>

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
            <h1 data-i18n="planes.titulo">Planes de suscripción</h1>
            <p data-i18n="planes.bajada">
                Todos los planes incluyen el dispositivo Tobii en préstamo, la plataforma completa
                y el procesamiento en la infraestructura de Pattern Blue. Cambia el alcance del
                análisis y el nivel de acompañamiento.
            </p>

            <div class="selector-ciclo" role="group" aria-label="Ciclo de facturacion">
                <button type="button" id="btnMensual" aria-pressed="true" data-i18n="planes.mensual">Mensual</button>
                <button type="button" id="btnAnual" aria-pressed="false" data-i18n="planes.anual">Anual (20% menos)</button>
            </div>
        </header>

        <div class="planes">

            <article class="tarjeta plan">
                <h2 data-i18n="planes.scout.nombre">Falke Scout</h2>
                <p class="para-quien" data-i18n="planes.scout.paraQuien">
                    Para equipos que incorporan el seguimiento ocular como práctica de validación.
                </p>

                <div class="plan-precio">
                    <span class="monto" data-precio-mensual="USD 590" data-precio-anual="USD 470">USD 590</span>
                    <span class="periodo" data-i18n="planes.porMes">/ mes</span>
                </div>
                <p class="plan-facturacion" data-factura-mensual="Facturacion mensual. Un dispositivo en prestamo."
                                            data-factura-anual="Facturacion anual: USD 5.640 por ano. Un dispositivo en prestamo.">
                    Facturación mensual. Un dispositivo en préstamo.
                </p>

                <ul>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.scout.item1">Un dispositivo Tobii Eye Tracker en préstamo</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.scout.item2">Sesiones de análisis individuales ilimitadas</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.scout.item3">Las 6 visualizaciones analíticas por sesión</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.scout.item4">Métricas estadísticas</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.scout.item5">Soporte técnico remoto en horario comercial</span></li>
                    <li class="no-incluye"><svg width="17" height="17" aria-hidden="true"><use href="#i-cerrar" /></svg><span data-i18n="planes.scout.item6">Sin análisis múltiple</span></li>
                </ul>

                <button type="button" class="btn btn-secundario btn-bloque"
                        data-abre-modal="modalContacto" data-plan="Falke Scout" data-i18n="planes.contratar">
                    Quiero este plan
                </button>
            </article>

            <article class="tarjeta plan destacado">
                <span class="plan-cinta" data-i18n="planes.masElegido">El más elegido</span>

                <h2 data-i18n="planes.hunter.nombre">Falke Hunter</h2>
                <p class="para-quien" data-i18n="planes.hunter.paraQuien">
                    Para equipos que usan el servicio de forma regular y necesitan comparar entre testers.
                </p>

                <div class="plan-precio">
                    <span class="monto" data-precio-mensual="USD 950" data-precio-anual="USD 760">USD 950</span>
                    <span class="periodo" data-i18n="planes.porMes">/ mes</span>
                </div>
                <p class="plan-facturacion" data-factura-mensual="Facturacion mensual. Hasta 3 dispositivos en prestamo."
                                            data-factura-anual="Facturacion anual: USD 9.120 por ano. Hasta 3 dispositivos en prestamo.">
                    Facturación mensual. Hasta 3 dispositivos en préstamo.
                </p>

                <ul>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item1">Todo lo del plan Scout</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item2">Análisis múltiple: promedio de hasta 10 sesiones</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item3">Informes comparativos entre sesiones</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item5">Soporte prioritario con tiempo de respuesta garantizado</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.hunter.item6">Hasta 3 dispositivos simultáneos</span></li>
                </ul>

                <button type="button" class="btn btn-primario btn-bloque"
                        data-abre-modal="modalContacto" data-plan="Falke Hunter" data-i18n="planes.contratar">
                    Quiero este plan
                </button>
            </article>

            <article class="tarjeta plan">
                <h2 data-i18n="planes.apex.nombre">Falke Apex</h2>
                <p class="para-quien" data-i18n="planes.apex.paraQuien">
                    Para organizaciones con varios equipos, proyectos en paralelo y necesidades de escala.
                </p>

                <div class="plan-precio">
                    <span class="monto" data-precio-mensual="A convenir" data-precio-anual="A convenir">A convenir</span>
                </div>
                <p class="plan-facturacion" data-factura-mensual="Condiciones segun volumen de dispositivos y perfil de uso."
                                            data-factura-anual="Condiciones segun volumen de dispositivos y perfil de uso.">
                    Condiciones según volumen de dispositivos y perfil de uso.
                </p>

                <ul>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.apex.item1">Todo lo del plan Hunter</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.apex.item2">Cantidad de dispositivos a convenir</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.apex.item3">Usuarios adicionales dentro de la organización</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.apex.item4">Condiciones de contrato y soporte a medida</span></li>
                    <li><svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg><span data-i18n="planes.apex.item5">Acompañamiento en la puesta en marcha</span></li>
                </ul>

                <button type="button" class="btn btn-secundario btn-bloque"
                        data-abre-modal="modalContacto" data-plan="Falke Apex" data-i18n="planes.contactar">
                    Hablar con ventas
                </button>
            </article>

        </div>

        <div class="aviso aviso-info mt-24">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen" data-i18n="planes.aviso">
                Todos los planes incluyen seguridad y cumplimiento normativo: disociación de los
                datos biométricos, cifrado de las comunicaciones y almacenamiento separado por
                empresa, conforme a la Ley N.° 25.326.
            </p>
        </div>

        <section class="comparador">
            <h2 class="mb-24" data-i18n="planes.comparador.titulo">Comparación detallada</h2>

            <div class="tabla-scroll">
                <table class="tabla">
                    <caption class="solo-lectores">Comparación de funciones incluidas en cada plan</caption>
                    <thead>
                        <tr>
                            <th scope="col" data-i18n="planes.comparador.funcion">Función</th>
                            <th scope="col" class="plan-col">Scout</th>
                            <th scope="col" class="plan-col">Hunter</th>
                            <th scope="col" class="plan-col">Apex</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.dispositivo">Dispositivos en préstamo</th>
                            <td class="centro">1</td>
                            <td class="centro">3</td>
                            <td class="centro" data-i18n="planes.comparador.aConvenir">A convenir</td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.sesiones">Sesiones individuales</th>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.visualizaciones">6 visualizaciones analíticas</th>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.multiple">Análisis múltiple</th>
                            <td class="centro no" data-i18n="planes.comparador.noIncluido">No incluido</td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.soporte">Soporte prioritario</th>
                            <td class="centro no" data-i18n="planes.comparador.noIncluido">No incluido</td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                        </tr>
                        <tr>
                            <th scope="row" data-i18n="planes.comparador.historial">Historial y auditoría de sesiones</th>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
                            <td class="centro si"><svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg><span class="solo-lectores">Incluido</span></td>
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
                    <h2 id="tituloContacto" data-i18n="contacto.titulo">Hablemos de tu implementación</h2>
                    <p class="subtitulo sin-margen" data-i18n="contacto.subtitulo">
                        Déjanos tus datos y un ejecutivo comercial te contacta para coordinar la puesta en marcha.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <div class="campo">
                    <label for="ctPlan" data-i18n="contacto.plan">Plan de interés</label>
                    <input type="text" class="entrada" id="ctPlan" value="Falke Hunter" readonly />
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="ctNombre" data-i18n="contacto.nombre">Nombre</label>
                        <input type="text" class="entrada" id="ctNombre" data-foco-inicial
                               placeholder="Maria" data-i18n-attr="placeholder:contacto.nombre.placeholder" />
                    </div>
                    <div class="campo">
                        <label for="ctApellido" data-i18n="contacto.apellido">Apellido</label>
                        <input type="text" class="entrada" id="ctApellido"
                               placeholder="Gomez" data-i18n-attr="placeholder:contacto.apellido.placeholder" />
                    </div>
                </div>

                <div class="campo">
                    <label for="ctEmpresa" data-i18n="contacto.empresa">Empresa</label>
                    <input type="text" class="entrada" id="ctEmpresa"
                           placeholder="Nombre de tu organización" data-i18n-attr="placeholder:contacto.empresa.placeholder" />
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="ctEmail" data-i18n="contacto.email">Correo de contacto</label>
                        <input type="email" class="entrada" id="ctEmail"
                               placeholder="maria@empresa.com" data-i18n-attr="placeholder:contacto.email.placeholder" />
                    </div>
                    <div class="campo">
                        <label for="ctTelefono" data-i18n="contacto.telefono">Teléfono</label>
                        <input type="tel" class="entrada" id="ctTelefono"
                               placeholder="+54 11 0000-0000" data-i18n-attr="placeholder:contacto.telefono.placeholder" />
                    </div>
                </div>

                <div class="campo">
                    <label for="ctMensaje" data-i18n="contacto.mensaje">Mensaje</label>

                    <textarea class="entrada" id="ctMensaje" rows="4"></textarea>
                    <p class="ayuda" data-i18n="contacto.ayuda">El mensaje viene escrito por defecto, pero puedes cambiarlo.</p>
                </div>

                <label class="casilla-simple">
                    <input type="checkbox" id="ctConsentimiento" />
                    <span data-i18n="contacto.consentimiento">
                        Acepto que Pattern Blue use estos datos para contactarme por esta consulta.
                    </span>
                </label>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <button type="button" class="btn btn-primario" data-i18n="contacto.enviar">Enviar consulta</button>
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
                return "Hola! Me interesa comunicarme con ustedes para discutir la puesta en " +
                       "funcionamiento del plan " + plan + ". Quedo a la espera de su respuesta.";
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
