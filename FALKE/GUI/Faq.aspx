<%@ Page Language="C#" MasterPageFile="~/Publico.master" AutoEventWireup="true" CodeFile="Faq.aspx.cs" Inherits="GUI.Faq" Title="Preguntas frecuentes" %>

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
            <h1 data-i18n="faq.titulo">Preguntas frecuentes</h1>
            <p data-i18n="faq.bajada">
                Todo lo que suelen preguntarnos antes de contratar Falke. Si te queda alguna duda,
                escríbenos y la respondemos.
            </p>

            <div class="faq-herramientas">
                <div class="faq-buscador campo-buscar">
                    <label class="solo-lectores" for="faqBuscar" data-i18n="faq.buscarEtiqueta">Buscar en las preguntas frecuentes</label>
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-buscar" /></svg>
                    <input type="search" class="entrada" id="faqBuscar"
                           placeholder="Buscar una pregunta..." data-i18n-attr="placeholder:faq.buscarPlaceholder" />
                </div>
            </div>

            <div class="chips" data-unico style="justify-content: center;">
                <button type="button" class="chip" aria-pressed="true" data-filtro="todas" data-i18n="faq.filtro.todas">Todas</button>
                <button type="button" class="chip" aria-pressed="false" data-filtro="servicio" data-i18n="faq.filtro.servicio">El servicio</button>
                <button type="button" class="chip" aria-pressed="false" data-filtro="contratacion" data-i18n="faq.filtro.contratacion">Contratación</button>
                <button type="button" class="chip" aria-pressed="false" data-filtro="plataforma" data-i18n="faq.filtro.plataforma">Plataforma</button>
                <button type="button" class="chip" aria-pressed="false" data-filtro="datos" data-i18n="faq.filtro.datos">Datos y seguridad</button>
                <button type="button" class="chip" aria-pressed="false" data-filtro="hardware" data-i18n="faq.filtro.hardware">Hardware y soporte</button>
            </div>
        </header>

        <section class="faq-grupo" data-grupo="servicio">
            <h2 data-i18n="faq.grupo.servicio">Sobre el servicio</h2>

            <div class="acordeon">

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r1">
                            <span data-i18n="faq.p1.pregunta">¿Qué es exactamente Falke?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r1">
                        <p data-i18n="faq.p1.respuesta">
                            Es una suite de analítica conductual basada en seguimiento ocular. Un
                            dispositivo Tobii registra donde mira tu usuario mientras usa tu producto
                            digital, y la plataforma convierte esos datos en mapas de calor, zonas de
                            interés, recorridos de mirada y métricas de atención listas para interpretar.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r2">
                            <span data-i18n="faq.p2.pregunta">¿En qué se diferencia del eye tracking por cámara web?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r2">
                        <p data-i18n="faq.p2.respuesta">
                            Falke usa hardware de infrarrojo dedicado, no la cámara web del equipo.
                            Eso da mayor precisión y no se ve afectado por la postura ni la iluminación
                            del participante. Además no imponemos límite de duración ni de tipo de
                            activo: puedes analizar un videojuego o un flujo completo de software.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r3">
                            <span data-i18n="faq.p3.pregunta">¿Qué tipos de activos digitales puedo analizar?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r3">
                        <p data-i18n="faq.p3.respuesta">
                            Software de escritorio, aplicaciones web, aplicaciones móviles, videojuegos
                            y piezas de publicidad digital. Al crear la categoría eliges el tipo y la
                            plataforma pide los datos propios de ese caso (sistema operativo, plataforma,
                            canal de distribución, etc.).
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r4">
                            <span data-i18n="faq.p4.pregunta">¿Ustedes ponen los testers?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r4">
                        <p data-i18n="faq.p4.respuesta">
                            No. El modelo de Falke está pensado para que trabajes con tus propios
                            usuarios, en tu propio entorno. Eso da mucha más fidelidad que un panel
                            externo, sobre todo en productos con audiencias específicas. Tampoco
                            hacemos consultoría de diseño: entregamos la evidencia, la decisión es
                            de tu equipo.
                        </p>
                    </div>
                </div>

            </div>
        </section>

        <section class="faq-grupo" data-grupo="contratacion">
            <h2 data-i18n="faq.grupo.contratacion">Contratación y facturación</h2>

            <div class="acordeon">

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r5">
                            <span data-i18n="faq.p5.pregunta">¿Por qué no puedo crear una cuenta solo?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r5">
                        <p data-i18n="faq.p5.respuesta">
                            Porque el servicio incluye un dispositivo físico en préstamo. El alta la
                            hace un gestor de Pattern Blue una vez firmado el contrato: se registra la
                            empresa, se crea el usuario administrador y esa persona recibe un correo con
                            un enlace para definir su contraseña. Desde ahí el administrador invita al
                            resto de su equipo.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r6">
                            <span data-i18n="faq.p6.pregunta">¿Cuánto tarda desde que firmo hasta que puedo grabar?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r6">
                        <p data-i18n="faq.p6.respuesta">
                            El objetivo es que puedas correr tu primera sesión dentro de los diez días
                            de firmado el contrato, incluyendo el envío del dispositivo. En AMBA,
                            Córdoba, Rosario y Mendoza ese plazo se cumple sin problema; para otras
                            localidades se acuerda caso por caso.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r7">
                            <span data-i18n="faq.p7.pregunta">¿Qué pasa cuando termina el contrato?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r7">
                        <p data-i18n="faq.p7.respuesta">
                            El dispositivo vuelve a Pattern Blue sin costo para ti, reusando el mismo
                            embalaje con el que llegó. Te avisamos por correo con anticipación y
                            coordinamos el retiro.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r8">
                            <span data-i18n="faq.p8.pregunta">¿Conviene pagar por mes o por año?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r8">
                        <p data-i18n="faq.p8.respuesta">
                            El pago anual anticipado tiene un 20% de descuento sobre el precio mensual.
                            Si recién estás evaluando la herramienta, el ciclo mensual te deja más margen.
                        </p>
                    </div>
                </div>

            </div>
        </section>

        <section class="faq-grupo" data-grupo="plataforma">
            <h2 data-i18n="faq.grupo.plataforma">Uso de la plataforma</h2>

            <div class="acordeon">

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r9">
                            <span data-i18n="faq.p9.pregunta">¿Qué visualizaciones obtengo de cada sesión?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r9">
                        <p data-i18n="faq.p9.respuesta">
                            Seis: la grabación con el seguimiento superpuesto, el mapa de calor, las
                            zonas de interés y zonas ciegas, la dispersión de mirada, el recorrido de
                            mirada y la relación atención vs. distracción. Además de las métricas
                            estadísticas: duración, cantidad de fijaciones y tiempo de fijación medio.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r10">
                            <span data-i18n="faq.p10.pregunta">¿Cuánto tarda el procesamiento?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r10">
                        <p data-i18n="faq.p10.respuesta">
                            Arranca solo al terminar la grabación y, en condiciones normales de carga,
                            deja todas las visualizaciones disponibles en menos de cinco minutos. No hay
                            ningún paso manual de tu lado.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r11">
                            <span data-i18n="faq.p11.pregunta">¿Puedo combinar varias sesiones en un solo análisis?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r11">
                        <p data-i18n="faq.p11.respuesta">
                            Sí, con el análisis múltiple (planes Hunter y Apex). Se promedian los datos
                            de varias sesiones para ver patrones comunes entre distintos testers. La
                            plataforma válida que todas las sesiones sean del mismo tipo de activo,
                            porque si no el promedio no sería válido.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r12">
                            <span data-i18n="faq.p12.pregunta">¿En qué idiomas está la plataforma?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r12">
                        <p data-i18n="faq.p12.respuesta">
                            Español, inglés y portugués. Cada usuario elige el suyo desde su perfil y la
                            preferencia se mantiene entre sesiones, sin afectar al resto del equipo.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r13">
                            <span data-i18n="faq.p13.pregunta">¿Puedo compartir los resultados con gente sin cuenta?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r13">
                        <p data-i18n="faq.p13.respuesta">
                            Sí. Cada análisis se exporta a PDF eligiendo con casillas qué visualizaciones
                            y métricas incluir. Ese reporte se puede compartir con cualquiera, sin
                            necesidad de que entre a la plataforma.
                        </p>
                    </div>
                </div>

            </div>
        </section>

        <section class="faq-grupo" data-grupo="datos">
            <h2 data-i18n="faq.grupo.datos">Datos y seguridad</h2>

            <div class="acordeon">

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r14">
                            <span data-i18n="faq.p14.pregunta">¿Qué pasa con los datos de las personas que participan?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r14">
                        <p data-i18n="faq.p14.respuesta">
                            Los datos de seguimiento ocular se disocian de la identidad del tester en el
                            momento de guardarse: lo que queda almacenado no permite identificar a la
                            persona. Es un requisito de la Ley N.° 25.326 de Protección de Datos Personales
                            y está contemplado desde el diseño del sistema.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r15">
                            <span data-i18n="faq.p15.pregunta">¿Dónde se guardan las grabaciones?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r15">
                        <p data-i18n="faq.p15.respuesta">
                            En la infraestructura propia de Pattern Blue, nunca en plataformas de
                            terceros. Los datos de cada empresa se almacenan separados, sin posibilidad
                            de acceso cruzado, y las comunicaciones entre tu navegador y el servidor
                            viajan cifradas.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r16">
                            <span data-i18n="faq.p16.pregunta">¿Quién puede ver lo que hace mi equipo?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r16">
                        <p data-i18n="faq.p16.respuesta">
                            El administrador de tu empresa ve la bitácora de acciones de los usuarios de
                            tu propia organización, y nada más. Los análisis y las grabaciones solo son
                            accesibles para los usuarios de tu cuenta.
                        </p>
                    </div>
                </div>

            </div>
        </section>

        <section class="faq-grupo" data-grupo="hardware">
            <h2 data-i18n="faq.grupo.hardware">Hardware y soporte</h2>

            <div class="acordeon">

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r17">
                            <span data-i18n="faq.p17.pregunta">¿Qué necesito en mi equipo?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r17">
                        <p data-i18n="faq.p17.respuesta">
                            Un equipo de oficina estándar con un puerto USB libre, un navegador
                            actualizado (Chrome, Firefox o Edge) y el driver del dispositivo Tobii
                            instalado. La plataforma verifica la conexión, la calibración y la versión
                            del driver antes de dejarte iniciar una grabación.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r18">
                            <span data-i18n="faq.p18.pregunta">¿Y si el dispositivo falla?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r18">
                        <p data-i18n="faq.p18.respuesta">
                            Si la falla no es atribuible al uso, la reparación o el reemplazo corren por
                            cuenta de Pattern Blue, sin costo adicional. Quedan afuera los daños por
                            golpes, líquidos, modificaciones no autorizadas o extravío.
                        </p>
                    </div>
                </div>

                <div class="acordeon-item">
                    <h3 class="sin-margen">
                        <button type="button" class="acordeon-boton" aria-expanded="false" aria-controls="r19">
                            <span data-i18n="faq.p19.pregunta">¿Hay límite de duración en las sesiones?</span>
                            <svg class="signo" width="18" height="18" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                        </button>
                    </h3>
                    <div class="acordeon-panel" id="r19">
                        <p data-i18n="faq.p19.respuesta">
                            No. Puedes grabar una sesión tan larga como necesites, que es justamente lo
                            que permite analizar videojuegos o flujos completos de software empresarial.
                        </p>
                    </div>
                </div>

            </div>
        </section>

        <div class="faq-sin-resultados vacio" id="faqVacio" hidden>
            <svg width="34" height="34" aria-hidden="true"><use href="#i-buscar" /></svg>
            <p class="sin-margen" data-i18n="faq.sinResultados">
                No encontramos ninguna pregunta con ese texto. Prueba con otras palabras.
            </p>
        </div>

        <div class="faq-contacto">
            <h2 data-i18n="faq.contacto.titulo">¿Te quedó alguna duda?</h2>
            <p class="texto-suave" data-i18n="faq.contacto.texto">
                Escríbenos desde la pantalla de planes y te respondemos a la brevedad.
            </p>
            <a class="btn btn-primario" href="<%: ResolveUrl("~/Planes.aspx") %>" data-i18n="faq.contacto.boton">Ir a los planes</a>
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
