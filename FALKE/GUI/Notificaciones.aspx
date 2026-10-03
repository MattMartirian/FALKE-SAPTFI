<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Notificaciones.aspx.cs" Inherits="GUI.Notificaciones" Title="Avisos" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .aviso-item
        {
            display: flex;
            gap: 14px;
            align-items: flex-start;
            padding: 16px 18px;
            border: 1px solid var(--borde);
            border-radius: var(--radio);
            background: var(--superficie);
            margin-bottom: 10px;
        }

        .aviso-item.sin-leer
        {
            border-color: var(--acento);
            background: var(--superficie-2);
        }

        .aviso-item.sin-leer .aviso-asunto
        {
            font-weight: 700;
        }

        .aviso-item .icono
        {
            width: 38px;
            height: 38px;
            flex: none;
            border-radius: var(--radio-full);
            display: flex;
            align-items: center;
            justify-content: center;
            background: var(--acento-suave);
            color: var(--acento-texto);
        }

        .aviso-item .icono.peligro
        {
            background: var(--superficie-2);
            color: var(--peligro);
        }

        .aviso-item .icono.info
        {
            background: var(--superficie-2);
            color: var(--info);
        }

        .aviso-asunto
        {
            display: block;
            font-weight: 600;
            margin-bottom: 3px;
        }

        .aviso-texto
        {
            margin: 0;
            font-size: .9rem;
            color: var(--texto-suave);
        }

        .aviso-fecha
        {
            font-size: .78rem;
            color: var(--texto-tenue);
            white-space: nowrap;
            margin-left: auto;
            padding-left: 12px;
        }

        @media (max-width: 640px)
        {
            .aviso-item
            {
                flex-wrap: wrap;
            }

            .aviso-fecha
            {
                margin-left: 52px;
                padding-left: 0;
            }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="avisos.titulo">Avisos</h1>
            <p class="texto-suave sin-margen" data-i18n="avisos.bajada">
                Novedades del sistema y de tu empresa.
            </p>
        </div>
        <div class="acciones">

            <button type="button" class="btn btn-secundario" id="btnMarcarTodos">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg>
                <span data-i18n="avisos.marcarTodos">Marcar todos como leidos</span>
            </button>
        </div>
    </div>

    <div class="chips mb-24" data-unico role="group" aria-label="Filtrar avisos">
        <button type="button" class="chip" data-filtro="todos" aria-pressed="true"
                data-i18n="comun.todos">Todos</button>
        <button type="button" class="chip" data-filtro="sinLeer" aria-pressed="false"
                data-i18n="avisos.sinLeer">Sin leer</button>
        <button type="button" class="chip" data-filtro="sistema" aria-pressed="false"
                data-i18n="avisos.sistema">Sistema</button>
        <button type="button" class="chip" data-filtro="empresa" aria-pressed="false"
                data-i18n="avisos.empresa">Mi empresa</button>
    </div>

    <div id="listaAvisos">

        <article class="aviso-item sin-leer" data-tipo="sistema">
            <span class="icono peligro" aria-hidden="true">
                <svg width="20" height="20"><use href="#i-alerta" /></svg>
            </span>
            <div>
                <span class="aviso-asunto" data-i18n="avisos.mantenimiento.asunto">Mantenimiento programado</span>
                <p class="aviso-texto" data-i18n="avisos.mantenimiento.texto">
                    La página será puesta en mantenimiento en 30 minutos, guarde sus archivos
                    antes de la hora informada.
                </p>
            </div>
            <span class="aviso-fecha">hace 5 min</span>
        </article>

        <article class="aviso-item sin-leer" data-tipo="empresa">
            <span class="icono" aria-hidden="true">
                <svg width="20" height="20"><use href="#i-video" /></svg>
            </span>
            <div>
                <span class="aviso-asunto" data-i18n="avisos.procesada.asunto">Sesión procesada</span>
                <p class="aviso-texto" data-i18n="avisos.procesada.texto">
                    El análisis de la sesión "Nivel 4 - tutorial" ya está disponible en
                    Visualizaciones.
                </p>
            </div>
            <span class="aviso-fecha">hace 2 h</span>
        </article>

        <article class="aviso-item sin-leer" data-tipo="empresa">
            <span class="icono info" aria-hidden="true">
                <svg width="20" height="20"><use href="#i-usuarios" /></svg>
            </span>
            <div>
                <span class="aviso-asunto" data-i18n="avisos.nuevoUsuario.asunto">Nuevo integrante</span>
                <p class="aviso-texto" data-i18n="avisos.nuevoUsuario.texto">
                    Diego Paz fue invitado a tu empresa y todavía no activo su cuenta.
                </p>
            </div>
            <span class="aviso-fecha">ayer</span>
        </article>

        <article class="aviso-item" data-tipo="empresa">
            <span class="icono" aria-hidden="true">
                <svg width="20" height="20"><use href="#i-dispositivo" /></svg>
            </span>
            <div>
                <span class="aviso-asunto" data-i18n="avisos.calibracion.asunto">Calibración recomendada</span>
                <p class="aviso-texto" data-i18n="avisos.calibracion.texto">
                    El dispositivo TB5-2026-0148 no se calibra hace más de una semana.
                </p>
            </div>
            <span class="aviso-fecha">3 sep</span>
        </article>

        <article class="aviso-item" data-tipo="sistema">
            <span class="icono info" aria-hidden="true">
                <svg width="20" height="20"><use href="#i-info" /></svg>
            </span>
            <div>
                <span class="aviso-asunto" data-i18n="avisos.version.asunto">Nueva versión</span>
                <p class="aviso-texto" data-i18n="avisos.version.texto">
                    La versión 1.4 suma el análisis múltiple de sesiones y mejoras en el
                    mapa de calor.
                </p>
            </div>
            <span class="aviso-fecha">1 sep</span>
        </article>

        <article class="aviso-item" data-tipo="empresa">
            <span class="icono" aria-hidden="true">
                <svg width="20" height="20"><use href="#i-descarga" /></svg>
            </span>
            <div>
                <span class="aviso-asunto" data-i18n="avisos.reporte.asunto">Reporte listo</span>
                <p class="aviso-texto" data-i18n="avisos.reporte.texto">
                    El reporte comparativo de tres sesiones de Onboarding término de generarse.
                </p>
            </div>
            <span class="aviso-fecha">28 ago</span>
        </article>

    </div>

    <div class="vacio" id="avisosVacio" hidden>
        <svg width="34" height="34" aria-hidden="true"><use href="#i-campana" /></svg>
        <p class="sin-margen" data-i18n="avisos.vacio">No hay avisos en esta categoría.</p>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var chips = document.querySelectorAll(".chip[data-filtro]");
            var avisos = document.querySelectorAll("#listaAvisos .aviso-item");
            var vacio = document.getElementById("avisosVacio");
            var botonMarcar = document.getElementById("btnMarcarTodos");

            function filtrar(filtro) {
                var visibles = 0;

                for (var i = 0; i < avisos.length; i++)
                {
                    var mostrar = filtro === "todos" ||
                                  (filtro === "sinLeer" && avisos[i].classList.contains("sin-leer")) ||
                                  avisos[i].getAttribute("data-tipo") === filtro;

                    avisos[i].hidden = !mostrar;

                    if (mostrar) visibles++;
                }

                vacio.hidden = visibles > 0;
            }

            for (var i = 0; i < chips.length; i++)
            {
                chips[i].addEventListener("click", function () {
                    filtrar(this.getAttribute("data-filtro"));
                });
            }

            botonMarcar.addEventListener("click", function () {
                for (var j = 0; j < avisos.length; j++)
                    avisos[j].classList.remove("sin-leer");

                var punto = document.querySelector(".punto-aviso");

                if (punto) punto.hidden = true;
            });
        })();
    </script>
</asp:Content>
