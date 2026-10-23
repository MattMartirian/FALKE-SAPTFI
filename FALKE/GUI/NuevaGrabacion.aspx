<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="NuevaGrabacion.aspx.cs" Inherits="GUI.NuevaGrabacion" Title="[[app_nueva_grabacion]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .asistente
        {
            max-width: 780px;
            margin: 0 auto;
        }

        .pasos-barra
        {
            display: flex;
            align-items: flex-start;
            margin-bottom: 28px;
            list-style: none;
            padding: 0;
        }

        .pasos-barra li
        {
            flex: 1;
            text-align: center;
            position: relative;
            font-size: .82rem;
            color: var(--texto-suave);
        }

        .pasos-barra li::before
        {
            content: "";
            position: absolute;
            top: 17px;
            left: -50%;
            width: 100%;
            height: 2px;
            background: var(--borde);
        }

        .pasos-barra li:first-child::before
        {
            display: none;
        }

        .pasos-barra .bolita
        {
            position: relative;
            z-index: 1;
            width: 34px;
            height: 34px;
            margin: 0 auto 8px;
            border-radius: 50%;
            background: var(--superficie);
            border: 2px solid var(--borde);
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            font-size: .88rem;
        }

        .pasos-barra li[aria-current="step"]
        {
            color: var(--texto);
            font-weight: 600;
        }

        .pasos-barra li[aria-current="step"] .bolita
        {
            background: var(--acento);
            border-color: var(--acento);
            color: var(--marca-azul);
        }

        .pasos-barra li.completado .bolita
        {
            background: var(--exito);
            border-color: var(--exito);
            color: #FFFFFF;
        }

        .paso-panel[hidden]
        {
            display: none;
        }

        .paso-acciones
        {
            display: flex;
            gap: 10px;
            justify-content: flex-end;
            margin-top: 24px;
        }

        .paso-acciones .izquierda
        {
            margin-right: auto;
        }

        .chequeo
        {
            display: flex;
            align-items: center;
            gap: 13px;
            padding: 14px 16px;
            border: 1px solid var(--borde);
            border-radius: var(--radio);
            margin-bottom: 10px;
        }

        .chequeo .icono
        {
            width: 34px;
            height: 34px;
            flex: none;
            border-radius: var(--radio-sm);
            background: var(--superficie-2);
            color: var(--texto-suave);
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .chequeo .texto
        {
            flex: 1;
            min-width: 0;
        }

        .chequeo .nombre
        {
            font-weight: 600;
            font-size: .92rem;
        }

        .chequeo .detalle
        {
            font-size: .8rem;
            color: var(--texto-suave);
        }

        .chequeo.correcto
        {
            border-color: var(--exito);
            background: var(--exito-suave);
        }

        .chequeo.correcto .icono
        {
            background: var(--exito);
            color: #FFFFFF;
        }

        .chequeo.problema
        {
            border-color: var(--peligro);
            background: var(--peligro-suave);
        }

        .chequeo.problema .icono
        {
            background: var(--peligro);
            color: #FFFFFF;
        }

        .grabando
        {
            text-align: center;
        }

        .reloj-grabacion
        {
            font-family: var(--fuente-mono);
            font-size: 2.4rem;
            font-weight: 700;
            letter-spacing: .04em;
        }

        .indicador-rec
        {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            font-family: var(--fuente-mono);
            font-size: .8rem;
            font-weight: 700;
            letter-spacing: .08em;
            text-transform: uppercase;
            color: var(--marca-coral);
            margin-bottom: 6px;
        }

        .indicador-rec .punto
        {
            width: 11px;
            height: 11px;
            border-radius: 50%;
            background: var(--marca-coral);
            animation: latir 1.4s ease-in-out infinite;
        }

        @keyframes latir
        {
            0%, 100% { opacity: 1; }
            50%      { opacity: .25; }
        }

        .previa-captura
        {
            margin: 18px 0;
        }

        .controles-grabacion
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
            gap: 10px;
            margin-top: 18px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

<div class="asistente">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="grabacion.titulo">[[app_nueva_grabacion]]</h1>
            <p class="texto-suave sin-margen" data-i18n="grabacion.bajada">
                [[nuevagrabacion_configura_la_sesion_verifica_el_disposit]]
            </p>
        </div>
    </div>

    <ol class="pasos-barra" id="pasosBarra">
        <li aria-current="step" data-paso="1">
            <span class="bolita" aria-hidden="true">1</span>
            <span data-i18n="grabacion.paso1">[[nuevagrabacion_datos_de_la_sesion]]</span>
        </li>
        <li data-paso="2">
            <span class="bolita" aria-hidden="true">2</span>
            <span data-i18n="grabacion.paso2">[[nuevagrabacion_dispositivo]]</span>
        </li>
        <li data-paso="3">
            <span class="bolita" aria-hidden="true">3</span>
            <span data-i18n="grabacion.paso3">[[nuevagrabacion_grabacion]]</span>
        </li>
    </ol>

    <section class="tarjeta paso-panel" id="panelPaso1" aria-labelledby="tituloPaso1">
        <h2 id="tituloPaso1" style="font-size:1.1rem" data-i18n="grabacion.datos.titulo">[[nuevagrabacion_datos_de_la_sesion]]</h2>
        <p class="texto-suave texto-chico" data-i18n="grabacion.datos.bajada">
            [[nuevagrabacion_la_categoria_define_el_tipo_de]]
        </p>

        <div class="campo">
            <label for="grCategoria" data-i18n="grabacion.categoria">[[categorias_categoria]]</label>

            <select class="entrada" id="grCategoria">
                <option value="1">Checkout flow v2 - Software</option>
                <option value="2">Onboarding RPG - Videojuego</option>
                <option value="3">Campaña Q3 - Publicidad</option>
                <option value="4">Panel interno - App web</option>
                <option value="5">Alta de cuenta - App móvil</option>
            </select>
            <p class="ayuda">
                <span data-i18n="grabacion.categoria.ayuda">[[nuevagrabacion_no_encuentras_la_que_necesitas]]</span>
                <a href="<%: ResolveUrl("~/Categorias.aspx") %>" data-i18n="grabacion.categoria.crear">[[nuevagrabacion_crear_una_categoria_nueva]]</a>
            </p>
        </div>

        <div class="campo">
            <label for="grNombre" data-i18n="grabacion.nombre">[[nuevagrabacion_nombre_de_la_sesion]]</label>
            <input type="text" class="entrada" id="grNombre"
                   placeholder="Checkout - App e-commerce" data-i18n-attr="placeholder:grabacion.nombre.placeholder" />
        </div>

        <div class="fila-campos">
            <div class="campo">
                <label for="grTester" data-i18n="grabacion.tester">[[nuevagrabacion_identificador_del_tester]]</label>
                <input type="text" class="entrada" id="grTester"
                       placeholder="Tester 07" data-i18n-attr="placeholder:grabacion.tester.placeholder" />
                <p class="ayuda" data-i18n="grabacion.tester.ayuda">
                    [[nuevagrabacion_usa_un_codigo_no_el_nombre]]
                </p>
            </div>
            <div class="campo">
                <label for="grAnalista" data-i18n="grabacion.analista">[[nuevagrabacion_analista_responsable]]</label>
                <input type="text" class="entrada" id="grAnalista" value="Maria Gomez" readonly />
                <p class="ayuda" data-i18n="grabacion.analista.ayuda">[[nuevagrabacion_se_toma_del_usuario_con_la]]</p>
            </div>
        </div>

        <div class="campo">
            <label for="grObjetivo" data-i18n="grabacion.objetivo">[[nuevagrabacion_objetivo_de_la_prueba_opcional]]</label>
            <textarea class="entrada" id="grObjetivo" rows="3"
                      placeholder="[[nuevagrabacion_que_quieres_validar_con_esta_sesion]]"
                      data-i18n-attr="placeholder:grabacion.objetivo.placeholder"></textarea>
        </div>

        <div class="paso-acciones">
            <a class="btn btn-fantasma izquierda" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>" data-i18n="comun.cancelar">[[categorias_cancelar]]</a>
            <button type="button" class="btn btn-primario" data-ir-paso="2">
                <span data-i18n="grabacion.continuar">[[nuevagrabacion_continuar]]</span>
                <svg width="17" height="17" aria-hidden="true"><use href="#i-flecha-der" /></svg>
            </button>
        </div>
    </section>

    <section class="tarjeta paso-panel" id="panelPaso2" aria-labelledby="tituloPaso2" hidden>
        <h2 id="tituloPaso2" style="font-size:1.1rem" data-i18n="grabacion.dispositivo.titulo">[[nuevagrabacion_verificacion_del_dispositivo]]</h2>
        <p class="texto-suave texto-chico" data-i18n="grabacion.dispositivo.bajada">
            [[nuevagrabacion_antes_de_grabar_la_plataforma_revisa]]
        </p>

        <div class="aviso aviso-exito" id="resumenDispositivo">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-tilde" /></svg>
            <div>
                <strong data-i18n="grabacion.dispositivo.listo">[[nuevagrabacion_dispositivo_listo]]</strong>
                <p class="sin-margen" data-i18n="grabacion.dispositivo.listoDetalle">
                    El Tobii 4C está conectado, calibrado y con el driver actualizado.
                </p>
            </div>
        </div>

        <div class="chequeo correcto" data-chequeo="usb">
            <span class="icono"><svg width="18" height="18" aria-hidden="true"><use href="#i-dispositivo" /></svg></span>
            <span class="texto">
                <span class="nombre" data-i18n="grabacion.chequeo.usb">[[nuevagrabacion_conexion_usb]]</span>
                <span class="detalle" data-i18n="grabacion.chequeo.usb.detalle">Tobii 4C detectado en el puerto USB 3.0</span>
            </span>
            <span class="badge badge-exito" data-i18n="estado.conectado">[[nuevagrabacion_conectado]]</span>
        </div>

        <div class="chequeo correcto" data-chequeo="calibracion">
            <span class="icono"><svg width="18" height="18" aria-hidden="true"><use href="#i-ojo" /></svg></span>
            <span class="texto">
                <span class="nombre" data-i18n="grabacion.chequeo.calibracion">[[nuevagrabacion_calibracion]]</span>
                <span class="detalle" data-i18n="grabacion.chequeo.calibracion.detalle">Última calibración hace 4 minutos &middot; calidad 92%</span>
            </span>
            <span class="badge badge-exito" data-i18n="estado.correcta">[[nuevagrabacion_correcta]]</span>
        </div>

        <div class="chequeo correcto" data-chequeo="driver">
            <span class="icono"><svg width="18" height="18" aria-hidden="true"><use href="#i-engranaje" /></svg></span>
            <span class="texto">
                <span class="nombre" data-i18n="grabacion.chequeo.driver">[[nuevagrabacion_driver_de_tobii]]</span>
                <span class="detalle">
                    <span data-i18n="grabacion.chequeo.driver.instalado">[[nuevagrabacion_instalado]]</span>: 4.2.1 &middot;
                    <span data-i18n="grabacion.chequeo.driver.requerido">[[nuevagrabacion_requerido]]</span>: 4.0 o superior
                </span>
            </span>
            <span class="badge badge-exito" data-i18n="estado.actualizado">[[nuevagrabacion_actualizado]]</span>
        </div>

        <div class="fila mt-16">
            <button type="button" class="btn btn-secundario btn-chico" id="btnVerificar">
                <svg width="16" height="16" aria-hidden="true"><use href="#i-refrescar" /></svg>
                <span data-i18n="grabacion.verificar">[[nuevagrabacion_verificar_nuevamente]]</span>
            </button>
            <button type="button" class="btn btn-fantasma btn-chico" id="btnSimularFallo" data-i18n="grabacion.simularFallo">
                [[nuevagrabacion_simular_un_fallo_demo]]
            </button>
        </div>

        <div class="paso-acciones">
            <button type="button" class="btn btn-secundario izquierda" data-ir-paso="1" data-i18n="comun.volver">[[nuevagrabacion_volver]]</button>
            <button type="button" class="btn btn-primario" data-ir-paso="3" id="btnIrAGrabar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-video" /></svg>
                <span data-i18n="grabacion.iniciar">[[nuevagrabacion_iniciar_la_grabacion]]</span>
            </button>
        </div>
    </section>

    <section class="tarjeta paso-panel grabando" id="panelPaso3" aria-labelledby="tituloPaso3" hidden>
        <h2 id="tituloPaso3" class="solo-lectores">[[nuevagrabacion_grabacion_en_curso]]</h2>

        <div class="campo derecha" style="text-align:left">
            <label for="grFuente" data-i18n="grabacion.fuente">[[nuevagrabacion_fuente_de_captura]]</label>

            <select class="entrada" id="grFuente">
                <option data-i18n="grabacion.fuente.pantalla">Pantalla completa (1920 x 1080 - monitor principal)</option>
                <option data-i18n="grabacion.fuente.ventana">Ventana - Chrome: Checkout Demo</option>
                <option data-i18n="grabacion.fuente.figma">Ventana - Figma: Prototipo checkout</option>
            </select>
        </div>

        <div class="indicador-rec" id="indicadorRec">
            <span class="punto" aria-hidden="true"></span>
            <span data-i18n="grabacion.grabando">[[nuevagrabacion_grabando]]</span>
        </div>

        <div class="reloj-grabacion" id="relojGrabacion" role="timer" aria-live="off">00:00:00</div>

        <p class="texto-suave texto-chico" id="estadoGrabacion" role="status" data-i18n="grabacion.sincronizado">
            [[nuevagrabacion_pantalla_y_seguimiento_ocular_sincroniza]]
        </p>

        <div class="previa-captura">
            <div class="grafico grafico-video" aria-hidden="true">
                <svg viewBox="0 0 320 180" preserveAspectRatio="none" style="position:absolute;inset:0">
                    <circle cx="118" cy="70" r="4" fill="#E8AF52" />
                    <circle cx="168" cy="96" r="4" fill="#E8AF52" opacity=".7" />
                    <circle cx="206" cy="64" r="4" fill="#E8AF52" opacity=".45" />
                </svg>
            </div>
        </div>

        <div class="controles-grabacion">
            <button type="button" class="btn btn-secundario" id="btnPausar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-pausa" /></svg>
                <span id="textoPausar" data-i18n="grabacion.pausar">[[nuevagrabacion_pausar]]</span>
            </button>
            <button type="button" class="btn btn-secundario" id="btnReiniciar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-refrescar" /></svg>
                <span data-i18n="grabacion.reiniciar">[[nuevagrabacion_reiniciar]]</span>
            </button>
            <button type="button" class="btn btn-primario" data-abre-modal="modalFinalizar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg>
                <span data-i18n="grabacion.finalizar">[[nuevagrabacion_finalizar]]</span>
            </button>
            <button type="button" class="btn btn-peligro" data-abre-modal="modalCancelar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-cerrar" /></svg>
                <span data-i18n="grabacion.cancelar">[[categorias_cancelar]]</span>
            </button>
        </div>

        <div class="aviso aviso-alerta mt-24 mb-0" style="text-align:left">
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen" data-i18n="grabacion.avisoDesconexion">
                [[nuevagrabacion_si_el_dispositivo_se_desconecta_durante]]
            </p>
        </div>
    </section>

</div>

<div class="modal-fondo" id="modalFinalizar" role="dialog" aria-modal="true" aria-labelledby="finTitulo" hidden>
    <div class="modal">
        <div class="modal-cabecera">
            <div>
                <h2 id="finTitulo" data-i18n="grabacion.finModal.titulo">[[nuevagrabacion_finalizar_la_grabacion]]</h2>
                <p class="subtitulo sin-margen" data-i18n="grabacion.finModal.subtitulo">
                    [[nuevagrabacion_la_sesion_se_cierra_y_se]]
                </p>
            </div>
            <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
            </button>
        </div>
        <div class="modal-cuerpo">
            <p class="texto-suave sin-margen" data-i18n="grabacion.finModal.texto">
                [[nuevagrabacion_el_procesamiento_arranca_solo_y_suele]]
            </p>
        </div>
        <div class="modal-pie">
            <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="grabacion.finModal.seguir">[[nuevagrabacion_seguir_grabando]]</button>
            <button type="button" class="btn btn-primario" data-i18n="grabacion.finModal.confirmar">[[nuevagrabacion_si_finalizar]]</button>
        </div>
    </div>
</div>

<div class="modal-fondo" id="modalCancelar" role="dialog" aria-modal="true" aria-labelledby="canTitulo" hidden>
    <div class="modal">
        <div class="modal-cabecera">
            <div>
                <h2 id="canTitulo" data-i18n="grabacion.canModal.titulo">[[nuevagrabacion_cancelar_la_grabacion]]</h2>
                <p class="subtitulo sin-margen" data-i18n="grabacion.canModal.subtitulo">
                    [[nuevagrabacion_esta_accion_no_se_puede_deshacer]]
                </p>
            </div>
            <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
            </button>
        </div>
        <div class="modal-cuerpo">
            <div class="aviso aviso-peligro mb-0">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
                <p class="sin-margen" data-i18n="grabacion.canModal.texto">
                    [[nuevagrabacion_se_descartan_todos_los_datos_capturados]]
                </p>
            </div>
        </div>
        <div class="modal-pie">
            <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="grabacion.canModal.seguir">[[nuevagrabacion_seguir_grabando]]</button>
            <button type="button" class="btn btn-peligro" data-i18n="grabacion.canModal.confirmar">[[nuevagrabacion_si_descartar_todo]]</button>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var pasos = document.querySelectorAll("#pasosBarra li");
            var paneles = {
                "1": document.getElementById("panelPaso1"),
                "2": document.getElementById("panelPaso2"),
                "3": document.getElementById("panelPaso3")
            };
            var pasoActual = 1;

            function irAPaso(numero) {
                if (numero === 3 && !dispositivoOk) return;

                pasoActual = numero;

                for (var clave in paneles) {
                    paneles[clave].hidden = clave !== String(numero);
                }

                for (var i = 0; i < pasos.length; i++) {
                    var n = parseInt(pasos[i].getAttribute("data-paso"), 10);

                    pasos[i].classList.toggle("completado", n < numero);

                    if (n === numero) {
                        pasos[i].setAttribute("aria-current", "step");
                    } else {
                        pasos[i].removeAttribute("aria-current");
                    }
                }

                if (numero === 3) arrancarCronometro();

                window.scrollTo(0, 0);
            }

            var botonesPaso = document.querySelectorAll("[data-ir-paso]");

            for (var i = 0; i < botonesPaso.length; i++) {
                botonesPaso[i].addEventListener("click", function () {
                    irAPaso(parseInt(this.getAttribute("data-ir-paso"), 10));
                });
            }

            var dispositivoOk = true;
            var resumen = document.getElementById("resumenDispositivo");
            var chequeoDriver = document.querySelector("[data-chequeo='driver']");
            var btnIrAGrabar = document.getElementById("btnIrAGrabar");

            function pintarResumen(ok) {
                dispositivoOk = ok;

                resumen.className = ok ? "aviso aviso-exito" : "aviso aviso-peligro";
                resumen.querySelector("use").setAttribute("href", ok ? "#i-tilde" : "#i-alerta");
                resumen.querySelector("strong").textContent = ok ? "[[js:nuevagrabacion_dispositivo_listo]]" : "[[js:nuevagrabacion_el_dispositivo_no_esta_listo]]";
                resumen.querySelector("p").textContent = ok
                    ? "[[js:nuevagrabacion_el_tobii_4c_esta_conectado_calibrado]]"
                    : "[[js:nuevagrabacion_resuelve_el_problema_marcado_en_rojo]]";

                btnIrAGrabar.disabled = !ok;
            }

            document.getElementById("btnSimularFallo").addEventListener("click", function () {
                chequeoDriver.className = "chequeo problema";
                chequeoDriver.querySelector(".detalle").textContent = "[[js:nuevagrabacion_instalado_3_4_0_requerido_4]]";
                chequeoDriver.querySelector(".badge").className = "badge badge-peligro";
                chequeoDriver.querySelector(".badge").textContent = "[[js:nuevagrabacion_desactualizado]]";

                pintarResumen(false);
            });

            document.getElementById("btnVerificar").addEventListener("click", function () {
                chequeoDriver.className = "chequeo correcto";
                chequeoDriver.querySelector(".detalle").textContent = "[[js:nuevagrabacion_instalado_4_2_1_requerido_4]]";
                chequeoDriver.querySelector(".badge").className = "badge badge-exito";
                chequeoDriver.querySelector(".badge").textContent = "[[js:nuevagrabacion_actualizado]]";

                pintarResumen(true);
            });

            var reloj = document.getElementById("relojGrabacion");
            var indicador = document.getElementById("indicadorRec");
            var estado = document.getElementById("estadoGrabacion");
            var btnPausar = document.getElementById("btnPausar");
            var textoPausar = document.getElementById("textoPausar");
            var segundos = 0;
            var intervalo = null;
            var enPausa = false;

            function dosDigitos(n) {
                return (n < 10 ? "0" : "") + n;
            }

            function pintarReloj() {
                var h = Math.floor(segundos / 3600);
                var m = Math.floor((segundos % 3600) / 60);
                var s = segundos % 60;

                reloj.textContent = dosDigitos(h) + ":" + dosDigitos(m) + ":" + dosDigitos(s);
            }

            function arrancarCronometro() {
                if (intervalo) return;

                intervalo = setInterval(function () {
                    if (enPausa) return;

                    segundos++;
                    pintarReloj();
                }, 1000);
            }

            btnPausar.addEventListener("click", function () {
                enPausa = !enPausa;

                textoPausar.textContent = enPausa ? "Reanudar" : "Pausar";
                indicador.style.opacity = enPausa ? ".45" : "1";
                estado.textContent = enPausa
                    ? "[[js:nuevagrabacion_grabacion_en_pausa_los_datos_capturados]]"
                    : "[[js:nuevagrabacion_pantalla_y_seguimiento_ocular_sincroniza]]";
            });

            document.getElementById("btnReiniciar").addEventListener("click", function () {
                segundos = 0;
                pintarReloj();
                estado.textContent = "[[js:nuevagrabacion_la_grabacion_se_reinicio_se_descartaron]]";
            });
        })();
    </script>
</asp:Content>
