<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="MisTobiis.aspx.cs" Inherits="GUI.MisTobiis" Title="[[mistobiis_mis_tobiis]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .tobii-herramientas
        {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 14px;
        }

        .tobii-estado
        {
            margin: 0 0 6px;
        }

        .tobii-guia[hidden]
        {
            display: none;
        }

        /* la previsualizacion se dibuja encima de toda la pagina y no tapa los clics */
        .tobii-previa
        {
            position: fixed;
            inset: 0;
            z-index: 900;
            pointer-events: none;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="mistobiis.titulo">[[mistobiis_mis_tobiis]]</h1>
            <p class="texto-suave sin-margen" data-i18n="mistobiis.bajada">[[mistobiis_bajada]]</p>
        </div>
    </div>

    <section class="tarjeta mb-16">
        <h2 style="font-size:1.1rem">[[mistobiis_herramientas]]</h2>
        <p class="texto-suave texto-chico tobii-estado" id="estadoAgente" aria-live="polite"></p>

        <div class="tobii-herramientas">
            <button type="button" class="btn btn-primario" id="btnAbrir">[[mistobiis_abrir_tobii_experience]]</button>
            <button type="button" class="btn btn-secundario" id="btnRecalibrar">[[mistobiis_recalibrar_tobii]]</button>
            <button type="button" class="btn btn-secundario" id="btnPantalla">[[mistobiis_calibrar_pantalla]]</button>
            <button type="button" class="btn btn-secundario" id="btnPrevia">[[mistobiis_previsualizar_mirada]]</button>
        </div>

        <div class="aviso aviso-info tobii-guia mt-16 mb-0" id="guiaTobii" hidden>
            <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen" id="guiaTexto"></p>
        </div>
    </section>

    <section class="tarjeta">
        <h2 style="font-size:1.1rem">[[mistobiis_tobiis_de_tu_empresa]]</h2>

        <div class="tabla-scroll">
            <table class="tabla">
                <thead>
                    <tr>
                        <th scope="col">[[dispositivos_numero_de_serie]]</th>
                        <th scope="col">[[dispositivos_modelo]]</th>
                        <th scope="col">[[dispositivos_estado]]</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptTobiis" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td class="mono"><%#: Eval("NumeroSerie") %></td>
                                <td><%#: Eval("Modelo") %></td>
                                <td><%#: TextoEstado(Eval("Estado")) %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>

        <% if (SinDispositivos) { %><p class="texto-suave mt-8 sin-margen">[[grabacion_dispositivo_sin]]</p><% } %>
    </section>

    <script>
        (function () {
            var urlAgente = "<%= URL_AGENTE %>";
            var textos = {
                agenteSi: "[[js:mistobiis_agente_conectado]]",
                agenteNo: "[[js:mistobiis_agente_no_responde]]",
                tobiiSi: "[[js:mistobiis_tobii_conectado]]",
                tobiiNo: "[[js:mistobiis_tobii_sin_conectar]]",
                guiaRecalibrar: "[[js:mistobiis_guia_recalibrar]]",
                guiaPantalla: "[[js:mistobiis_guia_pantalla]]",
                guiaPrevia: "[[js:mistobiis_guia_previa]]",
                guiaDejarPrevia: "[[js:mistobiis_guia_dejar_previa]]",
                previsualizar: "[[js:mistobiis_previsualizar_mirada]]",
                dejarPrevia: "[[js:mistobiis_dejar_de_previsualizar]]"
            };

            var estado = document.getElementById("estadoAgente");
            var guia = document.getElementById("guiaTobii");
            var guiaTexto = document.getElementById("guiaTexto");
            var btnPrevia = document.getElementById("btnPrevia");

            function pedir(ruta) {
                return fetch(urlAgente + ruta, { cache: "no-store", signal: AbortSignal.timeout(2500) })
                    .then(function (respuesta) { return respuesta.json(); })
                    .catch(function () { return null; });
            }

            function mostrarGuia(texto) {
                guiaTexto.textContent = texto;
                guia.hidden = false;
            }

            function revisarAgente() {
                pedir("/estado").then(function (datos) {
                    if (datos === null) {
                        estado.textContent = textos.agenteNo;
                    } else {
                        estado.textContent = textos.agenteSi + " · " + (datos.conectado ? textos.tobiiSi : textos.tobiiNo);
                    }
                });
            }

            // abre Tobii Experience (lo hace el agente, porque el navegador no puede abrir programas) y muestra los pasos que siguen
            function abrirTobii(guiaSiAbre) {
                pedir("/tobii").then(function (datos) {
                    if (datos === null) {
                        mostrarGuia(textos.agenteNo);
                        return;
                    }

                    if (guiaSiAbre) mostrarGuia(guiaSiAbre); else guia.hidden = true;
                });
            }

            document.getElementById("btnAbrir").addEventListener("click", function () { abrirTobii(null); });
            document.getElementById("btnRecalibrar").addEventListener("click", function () { abrirTobii(textos.guiaRecalibrar); });
            document.getElementById("btnPantalla").addEventListener("click", function () { abrirTobii(textos.guiaPantalla); });

            // previsualizar mirada: un punto y una estela que siguen la mirada sobre esta pagina
            var lienzo = null;
            var intervalo = null;
            var estela = [];
            var PUNTOS_ESTELA = 12;

            function pintarPrevia(datos) {
                var ctx = lienzo.getContext("2d");

                ctx.clearRect(0, 0, lienzo.width, lienzo.height);

                if (!datos.ok) return;

                // la mirada viene respecto de la pantalla: se la pasa a la ventana del navegador (se ajusta mejor en pantalla completa)
                var px = datos.x * screen.width - window.screenX;
                var py = datos.y * screen.height - window.screenY - (window.outerHeight - window.innerHeight);

                estela.push({ x: px, y: py });
                if (estela.length > PUNTOS_ESTELA) estela.shift();

                for (var i = 0; i < estela.length; i++) {
                    var f = (i + 1) / estela.length;

                    ctx.beginPath();
                    ctx.arc(estela[i].x, estela[i].y, 4 + 10 * f, 0, Math.PI * 2);
                    ctx.fillStyle = "rgba(242, 97, 87, " + (0.15 + 0.5 * f) + ")";
                    ctx.fill();
                }
            }

            function empezarPrevia() {
                // tambien se abre Tobii Experience, donde esta la opcion "Preview my gaze"
                pedir("/tobii");

                lienzo = document.createElement("canvas");
                lienzo.className = "tobii-previa";
                lienzo.width = window.innerWidth;
                lienzo.height = window.innerHeight;
                document.body.appendChild(lienzo);

                estela = [];
                btnPrevia.textContent = textos.dejarPrevia;
                mostrarGuia(textos.guiaPrevia);

                intervalo = setInterval(function () {
                    pedir("/mirada").then(function (datos) {
                        if (lienzo === null) return;

                        if (datos === null) {
                            terminarPrevia();
                            mostrarGuia(textos.agenteNo);
                            return;
                        }

                        pintarPrevia(datos);
                    });
                }, 50);
            }

            function terminarPrevia() {
                clearInterval(intervalo);
                intervalo = null;

                if (lienzo !== null) {
                    lienzo.remove();
                    lienzo = null;
                }

                btnPrevia.textContent = textos.previsualizar;
                mostrarGuia(textos.guiaDejarPrevia);
            }

            btnPrevia.addEventListener("click", function () {
                if (lienzo === null) empezarPrevia(); else terminarPrevia();
            });

            document.addEventListener("keydown", function (evento) {
                if (evento.key === "Escape" && lienzo !== null) terminarPrevia();
            });

            revisarAgente();
            setInterval(revisarAgente, 5000);
        })();
    </script>

</asp:Content>
