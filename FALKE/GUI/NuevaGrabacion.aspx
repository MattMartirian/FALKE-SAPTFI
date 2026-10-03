<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="NuevaGrabacion.aspx.cs" Inherits="GUI.NuevaGrabacion" Title="Nueva grabación" %>

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
            <h1 data-i18n="grabacion.titulo">Nueva grabación</h1>
            <p class="texto-suave sin-margen" data-i18n="grabacion.bajada">
                Configura la sesión, verifica el dispositivo y empieza a grabar.
            </p>
        </div>
    </div>

    <ol class="pasos-barra" id="pasosBarra">
        <li aria-current="step" data-paso="1">
            <span class="bolita" aria-hidden="true">1</span>
            <span data-i18n="grabacion.paso1">Datos de la sesión</span>
        </li>
        <li data-paso="2">
            <span class="bolita" aria-hidden="true">2</span>
            <span data-i18n="grabacion.paso2">Dispositivo</span>
        </li>
        <li data-paso="3">
            <span class="bolita" aria-hidden="true">3</span>
            <span data-i18n="grabacion.paso3">Grabación</span>
        </li>
    </ol>

    <section class="tarjeta paso-panel" id="panelPaso1" aria-labelledby="tituloPaso1">
        <h2 id="tituloPaso1" style="font-size:1.1rem" data-i18n="grabacion.datos.titulo">Datos de la sesión</h2>
        <p class="texto-suave texto-chico" data-i18n="grabacion.datos.bajada">
            La categoría define el tipo de activo digital y agrupa todas las sesiones que hagas sobre él.
        </p>

        <div class="campo">
            <label for="grCategoria" data-i18n="grabacion.categoria">Categoría</label>

            <select class="entrada" id="grCategoria">
                <option value="1">Checkout flow v2 - Software</option>
                <option value="2">Onboarding RPG - Videojuego</option>
                <option value="3">Campaña Q3 - Publicidad</option>
                <option value="4">Panel interno - App web</option>
                <option value="5">Alta de cuenta - App móvil</option>
            </select>
            <p class="ayuda">
                <span data-i18n="grabacion.categoria.ayuda">¿No encuentras la que necesitas?</span>
                <a href="<%: ResolveUrl("~/Categorias.aspx") %>" data-i18n="grabacion.categoria.crear">Crear una categoría nueva</a>
            </p>
        </div>

        <div class="campo">
            <label for="grNombre" data-i18n="grabacion.nombre">Nombre de la sesión</label>
            <input type="text" class="entrada" id="grNombre"
                   placeholder="Checkout - App e-commerce" data-i18n-attr="placeholder:grabacion.nombre.placeholder" />
        </div>

        <div class="fila-campos">
            <div class="campo">
                <label for="grTester" data-i18n="grabacion.tester">Identificador del tester</label>
                <input type="text" class="entrada" id="grTester"
                       placeholder="Tester 07" data-i18n-attr="placeholder:grabacion.tester.placeholder" />
                <p class="ayuda" data-i18n="grabacion.tester.ayuda">
                    Usa un código, no el nombre real: los datos se guardan disociados de la identidad.
                </p>
            </div>
            <div class="campo">
                <label for="grAnalista" data-i18n="grabacion.analista">Analista responsable</label>
                <input type="text" class="entrada" id="grAnalista" value="Maria Gomez" readonly />
                <p class="ayuda" data-i18n="grabacion.analista.ayuda">Se toma del usuario con la sesión iniciada.</p>
            </div>
        </div>

        <div class="campo">
            <label for="grObjetivo" data-i18n="grabacion.objetivo">Objetivo de la prueba (opcional)</label>
            <textarea class="entrada" id="grObjetivo" rows="3"
                      placeholder="¿Qué quieres validar con esta sesión?"
                      data-i18n-attr="placeholder:grabacion.objetivo.placeholder"></textarea>
        </div>

        <div class="paso-acciones">
            <a class="btn btn-fantasma izquierda" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>" data-i18n="comun.cancelar">Cancelar</a>
            <button type="button" class="btn btn-primario" data-ir-paso="2">
                <span data-i18n="grabacion.continuar">Continuar</span>
                <svg width="17" height="17" aria-hidden="true"><use href="#i-flecha-der" /></svg>
            </button>
        </div>
    </section>

    <section class="tarjeta paso-panel" id="panelPaso2" aria-labelledby="tituloPaso2" hidden>
        <h2 id="tituloPaso2" style="font-size:1.1rem" data-i18n="grabacion.dispositivo.titulo">Verificación del dispositivo</h2>
        <p class="texto-suave texto-chico" data-i18n="grabacion.dispositivo.bajada">
            Antes de grabar, la plataforma revisa que el Tobii este conectado, calibrado y con
            el driver correcto. Si algo falla no se puede iniciar la sesión.
        </p>

        <div class="aviso aviso-exito" id="resumenDispositivo">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-tilde" /></svg>
            <div>
                <strong data-i18n="grabacion.dispositivo.listo">Dispositivo listo</strong>
                <p class="sin-margen" data-i18n="grabacion.dispositivo.listoDetalle">
                    El Tobii 4C está conectado, calibrado y con el driver actualizado.
                </p>
            </div>
        </div>

        <div class="chequeo correcto" data-chequeo="usb">
            <span class="icono"><svg width="18" height="18" aria-hidden="true"><use href="#i-dispositivo" /></svg></span>
            <span class="texto">
                <span class="nombre" data-i18n="grabacion.chequeo.usb">Conexión USB</span>
                <span class="detalle" data-i18n="grabacion.chequeo.usb.detalle">Tobii 4C detectado en el puerto USB 3.0</span>
            </span>
            <span class="badge badge-exito" data-i18n="estado.conectado">Conectado</span>
        </div>

        <div class="chequeo correcto" data-chequeo="calibracion">
            <span class="icono"><svg width="18" height="18" aria-hidden="true"><use href="#i-ojo" /></svg></span>
            <span class="texto">
                <span class="nombre" data-i18n="grabacion.chequeo.calibracion">Calibración</span>
                <span class="detalle" data-i18n="grabacion.chequeo.calibracion.detalle">Última calibración hace 4 minutos &middot; calidad 92%</span>
            </span>
            <span class="badge badge-exito" data-i18n="estado.correcta">Correcta</span>
        </div>

        <div class="chequeo correcto" data-chequeo="driver">
            <span class="icono"><svg width="18" height="18" aria-hidden="true"><use href="#i-engranaje" /></svg></span>
            <span class="texto">
                <span class="nombre" data-i18n="grabacion.chequeo.driver">Driver de Tobii</span>
                <span class="detalle">
                    <span data-i18n="grabacion.chequeo.driver.instalado">Instalado</span>: 4.2.1 &middot;
                    <span data-i18n="grabacion.chequeo.driver.requerido">requerido</span>: 4.0 o superior
                </span>
            </span>
            <span class="badge badge-exito" data-i18n="estado.actualizado">Actualizado</span>
        </div>

        <div class="fila mt-16">
            <button type="button" class="btn btn-secundario btn-chico" id="btnVerificar">
                <svg width="16" height="16" aria-hidden="true"><use href="#i-refrescar" /></svg>
                <span data-i18n="grabacion.verificar">Verificar nuevamente</span>
            </button>
            <button type="button" class="btn btn-fantasma btn-chico" id="btnSimularFallo" data-i18n="grabacion.simularFallo">
                Simular un fallo (demo)
            </button>
        </div>

        <div class="paso-acciones">
            <button type="button" class="btn btn-secundario izquierda" data-ir-paso="1" data-i18n="comun.volver">Volver</button>
            <button type="button" class="btn btn-primario" data-ir-paso="3" id="btnIrAGrabar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-video" /></svg>
                <span data-i18n="grabacion.iniciar">Iniciar la grabación</span>
            </button>
        </div>
    </section>

    <section class="tarjeta paso-panel grabando" id="panelPaso3" aria-labelledby="tituloPaso3" hidden>
        <h2 id="tituloPaso3" class="solo-lectores">Grabación en curso</h2>

        <div class="campo derecha" style="text-align:left">
            <label for="grFuente" data-i18n="grabacion.fuente">Fuente de captura</label>

            <select class="entrada" id="grFuente">
                <option data-i18n="grabacion.fuente.pantalla">Pantalla completa (1920 x 1080 - monitor principal)</option>
                <option data-i18n="grabacion.fuente.ventana">Ventana - Chrome: Checkout Demo</option>
                <option data-i18n="grabacion.fuente.figma">Ventana - Figma: Prototipo checkout</option>
            </select>
        </div>

        <div class="indicador-rec" id="indicadorRec">
            <span class="punto" aria-hidden="true"></span>
            <span data-i18n="grabacion.grabando">Grabando</span>
        </div>

        <div class="reloj-grabacion" id="relojGrabacion" role="timer" aria-live="off">00:00:00</div>

        <p class="texto-suave texto-chico" id="estadoGrabacion" role="status" data-i18n="grabacion.sincronizado">
            Pantalla y seguimiento ocular sincronizados con la misma marca de tiempo.
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
                <span id="textoPausar" data-i18n="grabacion.pausar">Pausar</span>
            </button>
            <button type="button" class="btn btn-secundario" id="btnReiniciar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-refrescar" /></svg>
                <span data-i18n="grabacion.reiniciar">Reiniciar</span>
            </button>
            <button type="button" class="btn btn-primario" data-abre-modal="modalFinalizar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg>
                <span data-i18n="grabacion.finalizar">Finalizar</span>
            </button>
            <button type="button" class="btn btn-peligro" data-abre-modal="modalCancelar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-cerrar" /></svg>
                <span data-i18n="grabacion.cancelar">Cancelar</span>
            </button>
        </div>

        <div class="aviso aviso-alerta mt-24 mb-0" style="text-align:left">
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen" data-i18n="grabacion.avisoDesconexion">
                Si el dispositivo se desconecta durante la sesión, la grabación se interrumpe y los
                datos capturados se descartan. No muevas el cable durante la grabación.
            </p>
        </div>
    </section>

</div>

<div class="modal-fondo" id="modalFinalizar" role="dialog" aria-modal="true" aria-labelledby="finTitulo" hidden>
    <div class="modal">
        <div class="modal-cabecera">
            <div>
                <h2 id="finTitulo" data-i18n="grabacion.finModal.titulo">¿Finalizar la grabación?</h2>
                <p class="subtitulo sin-margen" data-i18n="grabacion.finModal.subtitulo">
                    La sesión se cierra y se envia a procesar. No se puede seguir grabando después.
                </p>
            </div>
            <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
            </button>
        </div>
        <div class="modal-cuerpo">
            <p class="texto-suave sin-margen" data-i18n="grabacion.finModal.texto">
                El procesamiento arranca solo y suele tardar menos de cinco minutos. Cuando termine
                vas a recibir un aviso y la sesión queda disponible en Visualizaciones.
            </p>
        </div>
        <div class="modal-pie">
            <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="grabacion.finModal.seguir">Seguir grabando</button>
            <button type="button" class="btn btn-primario" data-i18n="grabacion.finModal.confirmar">Sí, finalizar</button>
        </div>
    </div>
</div>

<div class="modal-fondo" id="modalCancelar" role="dialog" aria-modal="true" aria-labelledby="canTitulo" hidden>
    <div class="modal">
        <div class="modal-cabecera">
            <div>
                <h2 id="canTitulo" data-i18n="grabacion.canModal.titulo">¿Cancelar la grabación?</h2>
                <p class="subtitulo sin-margen" data-i18n="grabacion.canModal.subtitulo">
                    Esta acción no se puede deshacer.
                </p>
            </div>
            <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
            </button>
        </div>
        <div class="modal-cuerpo">
            <div class="aviso aviso-peligro mb-0">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
                <p class="sin-margen" data-i18n="grabacion.canModal.texto">
                    Se descartan todos los datos capturados hasta ahora, incluida la grabación de
                    pantalla y los datos de seguimiento ocular. No se guarda nada.
                </p>
            </div>
        </div>
        <div class="modal-pie">
            <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="grabacion.canModal.seguir">Seguir grabando</button>
            <button type="button" class="btn btn-peligro" data-i18n="grabacion.canModal.confirmar">Sí, descartar todo</button>
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
                resumen.querySelector("strong").textContent = ok ? "Dispositivo listo" : "El dispositivo no esta listo";
                resumen.querySelector("p").textContent = ok
                    ? "El Tobii 4C esta conectado, calibrado y con el driver actualizado."
                    : "Resuelve el problema marcado en rojo y verifica de nuevo para poder grabar.";

                btnIrAGrabar.disabled = !ok;
            }

            document.getElementById("btnSimularFallo").addEventListener("click", function () {
                chequeoDriver.className = "chequeo problema";
                chequeoDriver.querySelector(".detalle").textContent = "Instalado: 3.4.0 - requerido: 4.0 o superior";
                chequeoDriver.querySelector(".badge").className = "badge badge-peligro";
                chequeoDriver.querySelector(".badge").textContent = "Desactualizado";

                pintarResumen(false);
            });

            document.getElementById("btnVerificar").addEventListener("click", function () {
                chequeoDriver.className = "chequeo correcto";
                chequeoDriver.querySelector(".detalle").textContent = "Instalado: 4.2.1 - requerido: 4.0 o superior";
                chequeoDriver.querySelector(".badge").className = "badge badge-exito";
                chequeoDriver.querySelector(".badge").textContent = "Actualizado";

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
                    ? "Grabacion en pausa. Los datos capturados hasta ahora se conservan."
                    : "Pantalla y seguimiento ocular sincronizados con la misma marca de tiempo.";
            });

            document.getElementById("btnReiniciar").addEventListener("click", function () {
                segundos = 0;
                pintarReloj();
                estado.textContent = "La grabacion se reinicio. Se descartaron los datos anteriores.";
            });
        })();
    </script>
</asp:Content>
