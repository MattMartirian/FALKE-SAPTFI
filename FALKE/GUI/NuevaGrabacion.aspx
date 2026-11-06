<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="NuevaGrabacion.aspx.cs" Inherits="GUI.NuevaGrabacion" Title="[[app_nueva_grabacion]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        #grAnalista:disabled { background: var(--superficie-2); color: var(--texto-suave); -webkit-text-fill-color: var(--texto-suave); cursor: default; user-select: none; pointer-events: none; opacity: 1; }

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
            display: block;
            font-weight: 600;
            font-size: .92rem;
        }

        .chequeo .detalle
        {
            display: block;
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

        .chequeo.pendiente
        {
            border-color: var(--alerta);
            background: var(--alerta-suave);
        }

        .chequeo.pendiente .icono
        {
            background: var(--alerta);
            color: #FFFFFF;
        }

        .chequeo .icono > *
        {
            display: none;
        }

        .chequeo:not(.correcto):not(.problema):not(.pendiente) .icono .espera,
        .chequeo.correcto .icono .si,
        .chequeo.problema .icono .no,
        .chequeo.pendiente .icono .pregunta
        {
            display: block;
        }

        .chequeo .icono .pregunta
        {
            font-size: 1.15rem;
            font-weight: 800;
            line-height: 1;
        }

        .chequeo:not(.problema) .solo-problema,
        .sin-agente [data-abre-tobii]
        {
            display: none;
        }

        .chequeo .accion
        {
            display: flex;
            flex-direction: column;
            align-items: flex-end;
            gap: 8px;
            flex: none;
        }

        .resumen .espera,
        .resumen .si,
        .resumen .no
        {
            display: none;
        }

        .resumen.aviso-alerta .espera,
        .resumen.aviso-exito .si,
        .resumen.aviso-peligro .no
        {
            display: block;
        }

        .chequeo .confirmar
        {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-size: .82rem;
            cursor: pointer;
        }

        .rec-guia
        {
            display: flex;
            align-items: center;
            gap: 10px;
            margin: 0 0 10px;
            text-align: left;
            font-size: .95rem;
        }

        .rec-num
        {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            flex: none;
            width: 24px;
            height: 24px;
            border-radius: 50%;
            background: var(--acento);
            color: var(--marca-azul);
            font-size: .8rem;
            font-weight: 700;
        }

        .rec-fuente
        {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            margin-bottom: 10px;
            text-align: left;
        }

        .rec-fuente .etiqueta
        {
            margin-bottom: 2px;
            font-size: .78rem;
            font-weight: 400;
            color: var(--texto-suave);
        }

        .rec-fuente .detalle
        {
            margin-left: 8px;
            font-size: .82rem;
            color: var(--texto-suave);
        }

        #panelPaso3
        {
            --reserva-visor: 490px;
        }

        .visor
        {
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            aspect-ratio: 16 / 9;
            border: 1px solid var(--borde);
            border-radius: var(--radio);
            overflow: hidden;
            background: linear-gradient(160deg, #16294D 0%, #0E1A34 100%);
        }

        .visor video
        {
            position: absolute;
            inset: 0;
            width: 100%;
            height: 100%;
            object-fit: contain;
            background: #000000;
        }

        .visor-vacio
        {
            position: relative;
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 12px;
            padding: 0 24px;
            color: var(--marca-perla);
            text-align: center;
        }

        .visor-vacio p
        {
            margin: 0;
            opacity: .85;
            font-size: .92rem;
        }

        .visor-cuenta
        {
            position: absolute;
            inset: 0;
            z-index: 2;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 6px;
            background: rgba(14, 26, 52, .72);
            color: var(--marca-perla);
        }

        .visor-cuenta strong
        {
            font-family: var(--fuente-mono);
            font-size: 5rem;
            line-height: 1;
        }

        /* el grabando es todo CSS, sin js. el circulo rojo late con keyframes */
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
        }

        .punto
        {
            width: 11px;
            height: 11px;
            border-radius: 50%;
            background: var(--marca-coral);
             /* aca le digo la animacion y cuanto tarda */
            animation: latir 1.4s ease-in-out infinite;
        }

        /* esta es la animacion */
        @keyframes latir
        {
            0%, 100% { opacity: 1; }
            50%      { opacity: .25; }
        }

        .visor-rec
        {
            position: absolute;
            top: 12px;
            left: 12px;
            z-index: 2;
            padding: 6px 12px;
            border-radius: 999px;
            background: rgba(14, 26, 52, .85);
            color: var(--marca-perla);
        }

        .rec-retraso
        {
            display: flex;
            align-items: center;
            gap: 12px;
            margin-top: 12px;
            text-align: left;
        }

        .rec-retraso .etiqueta
        {
            margin: 0;
        }

        .segmentos
        {
            display: inline-flex;
            border: 1px solid var(--borde-fuerte);
            border-radius: var(--radio);
            overflow: hidden;
        }

        .segmento input
        {
            position: absolute;
            opacity: 0;
            pointer-events: none;
        }

        .segmento span
        {
            display: block;
            padding: 8px 16px;
            font-size: .88rem;
            cursor: pointer;
            border-left: 1px solid var(--borde-fuerte);
        }

        .segmento:first-child span
        {
            border-left: 0;
        }

        .segmento input:checked + span
        {
            background: var(--acento);
            color: var(--marca-azul);
            font-weight: 600;
        }

        .segmento input:focus-visible + span
        {
            outline: 2px solid var(--primario);
            outline-offset: -2px;
        }

        .rec-estado
        {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            gap: 10px 24px;
            margin-top: 12px;
            padding: 10px 14px;
            border: 1px solid var(--borde);
            border-radius: var(--radio);
            background: var(--superficie-2);
            text-align: left;
        }

        .rec-tiempo
        {
            display: inline-flex;
            align-items: center;
            gap: 10px;
        }

        .rec-tiempo strong
        {
            font-family: var(--fuente-mono);
            font-size: 1.5rem;
            letter-spacing: .04em;
        }

        .dato-rec span
        {
            display: block;
            font-size: .75rem;
            color: var(--texto-suave);
        }

        .dato-rec strong
        {
            font-size: .92rem;
        }

        .dato-rec.ok strong
        {
            color: var(--exito);
        }

        .dato-rec.mal strong
        {
            color: var(--peligro);
        }

        .rec-datos
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
            gap: 10px;
            margin-top: 12px;
            text-align: left;
        }

        .rec-datos span
        {
            display: block;
            font-size: .78rem;
            color: var(--texto-suave);
        }

        .rec-datos strong
        {
            font-family: var(--fuente-mono);
            font-size: 1.15rem;
        }

        .controles-grabacion
        {
            display: flex;
            flex-wrap: wrap;
            justify-content: flex-end;
            gap: 10px;
            margin-top: 12px;
        }

        .controles-grabacion .izquierda
        {
            margin-right: auto;
        }

        #panelPaso3 .aviso
        {
            margin: 12px 0 0;
            text-align: left;
        }

        /* barra de la subidam, el relleno se pinta solo segun "value" (0 a 100) */
        .subida { margin: 0 0 12px; }
        .subida progress { width: 100%; height: 10px; }
        .exito-grabacion { display: flex; flex-direction: column; align-items: center; text-align: center; gap: 8px; padding: 28px 16px; border: 1px solid var(--borde, #d9dee5); border-radius: 12px; margin-bottom: 12px; color: var(--exito, #1a7f45); }
        .exito-grabacion p { color: inherit; }
        .exito-grabacion .ayuda { color: var(--texto-suave, #5b6675); }
        .exito-opciones { display: flex; flex-wrap: wrap; gap: 16px; width: 100%; max-width: 720px; margin-top: 12px; }
        .exito-opcion { flex: 1 1 280px; display: flex; flex-direction: column; align-items: center; gap: 8px; }
        .exito-opcion .btn { width: 100%; }

        #panelPaso3[data-estado="preparar"] [data-solo]:not([data-solo~="preparar"]),
        #panelPaso3[data-estado="cuenta"] [data-solo]:not([data-solo~="cuenta"]),
        #panelPaso3[data-estado="grabando"] [data-solo]:not([data-solo~="grabando"]),
        #panelPaso3[data-estado="revisar"] [data-solo]:not([data-solo~="revisar"]),
        #panelPaso3[data-estado="guardado"] [data-solo]:not([data-solo~="guardado"]),
        #panelPaso3 [hidden]
        {
            display: none !important;
        }

        .contenido:has(#panelPaso3[data-estado="guardado"]):not(:has(#panelPaso2:not([hidden])))
        {
            padding: 24px;
        }

        .contenido:has(#panelPaso3[data-estado="guardado"]):not(:has(#panelPaso2:not([hidden]))) .asistente .pagina-cabecera,
        .contenido:has(#panelPaso3[data-estado="guardado"]):not(:has(#panelPaso2:not([hidden]))) .asistente .pasos-barra,
        .contenido:has(#panelPaso3[data-estado="guardado"]):not(:has(#panelPaso2:not([hidden]))) .asistente .controles-grabacion
        {
            display: none;
        }

        .contenido:has(#panelPaso3[data-estado="guardado"]):not(:has(#panelPaso2:not([hidden]))) .asistente #panelPaso3
        {
            flex: 1 1 auto;
            justify-content: center;
            align-items: center;
            margin: 0;
            background: transparent;
            border: 0;
            box-shadow: none;
        }

        .contenido:has(#panelPaso3[data-estado="guardado"]):not(:has(#panelPaso2:not([hidden]))) .exito-grabacion
        {
            width: 100%;
            max-width: 680px;
            margin: 0;
            padding: 36px 32px 32px;
            background: var(--superficie);
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            box-shadow: var(--sombra-lg);
        }

        .contenido:has(.asistente):not(:has(#panelPaso2:not([hidden])))
        {
            box-sizing: border-box;
            height: calc(100vh - var(--alto-topbar));
            height: calc(100dvh - var(--alto-topbar));
            padding-top: 16px;
            padding-bottom: 16px;
            overflow-y: auto;
        }

        .contenido:has(.asistente):not(:has(#panelPaso2:not([hidden]))) .asistente
        {
            display: flex;
            flex-direction: column;
            min-height: 100%;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .pagina-cabecera
        {
            margin-bottom: 10px;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .pasos-barra
        {
            margin-bottom: 10px;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .pasos-barra .bolita
        {
            margin-bottom: 4px;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .paso-panel:not([hidden])
        {
            display: flex;
            flex-direction: column;
            flex: 0 1 auto;
            min-height: 0;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .paso-acciones
        {
            flex-wrap: wrap;
            margin-top: auto;
            padding-top: 12px;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .chequeo
        {
            padding: 9px 14px;
            margin-bottom: 7px;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .previa-captura
        {
            display: flex;
            justify-content: center;
            margin: 12px 0;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .previa-captura .visor
        {
            width: min(100%, calc((100vh - var(--reserva-visor)) * 16 / 9));
            width: min(100%, calc((100dvh - var(--reserva-visor)) * 16 / 9));
            height: auto;
            min-height: 0;
            aspect-ratio: 16 / 9;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .controles-grabacion
        {
            margin-top: 12px;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .tarjeta
        {
            padding: 14px 18px;
        }

        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente .tarjeta .aviso
        {
            padding: 9px 14px;
            margin-bottom: 12px;
        }
        .contenido:not(:has(#panelPaso2:not([hidden]))) .asistente #panelPaso1 .campo
        {
            margin-bottom: 8px;
        }

        #grObjetivo
        {
            box-sizing: border-box;
            height: 134px;
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

        <div class="fila-campos">
            <div class="campo">
                <label for="grCategoria" data-i18n="grabacion.categoria">[[categorias_categoria]]</label>

                <select class="entrada" id="grCategoria" data-perfil="">
                    <asp:Repeater ID="rptCategorias" runat="server">
                        <ItemTemplate><option value='<%#: Eval("Valor") %>'><%#: Eval("Texto") %></option></ItemTemplate>
                    </asp:Repeater>
                </select>
                <p class="ayuda">
                    <span data-i18n="grabacion.categoria.ayuda">[[nuevagrabacion_no_encuentras_la_que_necesitas]]</span>
                    <a href='<%: ResolveUrl("~/Categorias.aspx") %>' data-i18n="grabacion.categoria.crear">[[nuevagrabacion_crear_una_categoria_nueva]]</a>
                </p>
            </div>

            <div class="campo">
                <label for="grDispositivo">[[grabacion_dispositivo]]</label>

                <select class="entrada" id="grDispositivo" data-perfil="" <%= SinDispositivos ? "disabled" : "" %>>
                    <% if (SinDispositivos) { %><option value="">[[grabacion_dispositivo_sin]]</option><% } else { %><option value="">[[grabacion_tester_seleccionar]]</option><% } %>
                    <asp:Repeater ID="rptDispositivos" runat="server">
                        <ItemTemplate><option value='<%#: Eval("Valor") %>'><%#: Eval("Texto") %></option></ItemTemplate>
                    </asp:Repeater>
                </select>
            </div>
        </div>

        <div class="fila-campos">
            <div class="campo">
                <label for="grNombre" data-i18n="grabacion.nombre">[[nuevagrabacion_nombre_de_la_sesion]]</label>
                <input type="text" class="entrada" id="grNombre"
                       placeholder="Checkout - App e-commerce" data-i18n-attr="placeholder:grabacion.nombre.placeholder" />
            </div>
            <div class="campo">
                <label for="grAnalista" data-i18n="grabacion.analista">[[grabacion_responsable_tu]]</label>
                <input type="text" class="entrada" id="grAnalista" value='<%: NombreAnalista %>' disabled />
            </div>
        </div>

        <p class="etiqueta sin-margen">[[grabacion_tester_titulo]]</p>
        <p class="texto-suave texto-chico">[[grabacion_tester_ayuda]]</p>
        <div class="fila-campos">
            <div class="campo">
                <label for="grEdad">[[grabacion_tester_edad]]</label>
                <select class="entrada" id="grEdad" data-perfil="">
                    <option value="">[[grabacion_tester_seleccionar]]</option>
                    <asp:Repeater ID="rptEdades" runat="server"><ItemTemplate><option value='<%#: Eval("Valor") %>'><%#: Eval("Texto") %></option></ItemTemplate></asp:Repeater>
                </select>
            </div>
            <div class="campo">
                <label for="grGenero">[[grabacion_tester_genero]]</label>
                <select class="entrada" id="grGenero" data-perfil="">
                    <option value="">[[grabacion_tester_seleccionar]]</option>
                    <asp:Repeater ID="rptGeneros" runat="server"><ItemTemplate><option value='<%#: Eval("Valor") %>'><%#: Eval("Texto") %></option></ItemTemplate></asp:Repeater>
                </select>
            </div>
            <div class="campo">
                <label for="grFamiliaridad">[[grabacion_tester_familiaridad]]</label>
                <select class="entrada" id="grFamiliaridad" data-perfil="">
                    <option value="">[[grabacion_tester_seleccionar]]</option>
                    <asp:Repeater ID="rptFamiliaridades" runat="server"><ItemTemplate><option value='<%#: Eval("Valor") %>'><%#: Eval("Texto") %></option></ItemTemplate></asp:Repeater>
                </select>
            </div>
        </div>

        <div class="campo">
            <label for="grObjetivo" data-i18n="grabacion.objetivo">[[nuevagrabacion_objetivo_de_la_prueba_opcional]]</label>
            <textarea class="entrada" id="grObjetivo" rows="2"
                      placeholder="[[nuevagrabacion_que_quieres_validar_con_esta_sesion]]"
                      data-i18n-attr="placeholder:grabacion.objetivo.placeholder"></textarea>
        </div>

        <div class="paso-acciones">
            <a class="btn btn-fantasma izquierda" href='<%: ResolveUrl("~/Visualizaciones.aspx") %>' data-i18n="comun.cancelar">[[categorias_cancelar]]</a>
            <button type="button" class="btn btn-primario" data-ir-paso="2" id="btnContinuar" disabled>
                <span data-i18n="grabacion.continuar">[[nuevagrabacion_continuar]]</span>
                <svg width="17" height="17" aria-hidden="true"><use href="#i-flecha-der" /></svg>
            </button>
        </div>
    </section>

    <section class="tarjeta paso-panel" id="panelPaso2" aria-labelledby="tituloPaso2" hidden
             data-agente="<%: URL_AGENTE %>" data-espera="<%: ESPERA_AGENTE_MS %>" data-sondeo="<%: SONDEO_MS %>">
        <h2 id="tituloPaso2" style="font-size:1.1rem">[[grabacion_prep_titulo]]</h2>
        <p class="texto-suave texto-chico">[[grabacion_prep_bajada]]</p>

        <div class="aviso aviso-alerta resumen" id="resumenDispositivo" role="status">
            <svg class="espera" width="20" height="20" aria-hidden="true"><use href="#i-refrescar" /></svg>
            <svg class="si" width="20" height="20" aria-hidden="true"><use href="#i-tilde" /></svg>
            <svg class="no" width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
            <div class="espera"><strong>[[grabacion_prep_verificando]]</strong></div>
            <div class="si">
                <strong>[[grabacion_prep_resumen_ok]]</strong>
                <p class="sin-margen" id="notaRecomendada">[[grabacion_prep_resumen_ok_recom]]</p>
            </div>
            <div class="no">
                <strong>[[grabacion_prep_resumen_falta]]</strong>
                <p class="sin-margen">[[grabacion_prep_resumen_falta_detalle]]</p>
            </div>
        </div>

        <asp:Repeater ID="rptChequeos" runat="server">
            <ItemTemplate>
                <div class='chequeo <%# Eval("Clase") %>' data-chequeo='<%# Eval("Id") %>'<%# Atributos(Container.DataItem) %>>
                    <span class="icono">
                        <svg class="espera" width="18" height="18" aria-hidden="true"><use href="#i-refrescar" /></svg>
                        <svg class="si" width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg>
                        <svg class="no" width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                        <b class="pregunta">?</b>
                    </span>
                    <span class="texto">
                        <span class="nombre"><%#: Eval("Nombre") %><asp:PlaceHolder runat="server" Visible='<%# !(bool)Eval("Obligatorio") %>'> <span class="badge badge-alerta">[[grabacion_prep_recomendado]]</span></asp:PlaceHolder></span>
                        <span class="detalle"><%#: Eval("Inicial") %></span>
                    </span>
                    <span class="accion">
                        <asp:PlaceHolder runat="server" Visible='<%# Eval("ConEnlaceInstalacion") %>'>
                            <a class="btn btn-secundario btn-chico solo-problema" href='<%: ResolveUrl("~/Descargas.aspx") %>'>
                                <svg width="16" height="16" aria-hidden="true"><use href="#i-descarga" /></svg>
                                <span>[[grabacion_prep_ir_instalacion]]</span>
                            </a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" Visible='<%# Eval("ConBotonTobii") %>'>
                            <button type="button" class="btn btn-secundario btn-chico" data-abre-tobii>
                                <svg width="16" height="16" aria-hidden="true"><use href="#i-ojo" /></svg>
                                <span>[[grabacion_prep_abrir_tobii]]</span>
                            </button>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" Visible='<%# Eval("ConCasilla") %>'>
                            <label class="confirmar"><input type="checkbox" /> <span>[[grabacion_prep_ya_lo_hice]]</span></label>
                        </asp:PlaceHolder>
                    </span>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <div class="fila mt-16">
            <button type="button" class="btn btn-secundario btn-chico" id="btnVerificar">
                <svg width="16" height="16" aria-hidden="true"><use href="#i-refrescar" /></svg>
                <span>[[nuevagrabacion_verificar_nuevamente]]</span>
            </button>
        </div>

        <div class="paso-acciones">
            <button type="button" class="btn btn-secundario izquierda" data-ir-paso="1">[[nuevagrabacion_volver]]</button>
            <button type="button" class="btn btn-primario" data-ir-paso="3" id="btnIrAGrabar" disabled>
                <svg width="17" height="17" aria-hidden="true"><use href="#i-video" /></svg>
                <span>[[nuevagrabacion_iniciar_la_grabacion]]</span>
            </button>
        </div>
    </section>

    <section class="tarjeta paso-panel grabando" id="panelPaso3" aria-labelledby="tituloPaso3" data-estado="preparar"
             data-bitrate="<%: BITS_VIDEO_POR_SEGUNDO %>" data-ancho="<%: ANCHO_VIDEO %>" data-alto="<%: ALTO_VIDEO %>"
             data-fps="<%: FPS_VIDEO %>" data-sondeo="<%: SONDEO_GRABACION_MS %>" hidden>
        <h2 id="tituloPaso3" class="solo-lectores">[[grabacion_rec_titulo]]</h2>

        <p class="rec-guia" id="guiaElegir" data-solo="preparar"><span class="rec-num">1</span>[[grabacion_rec_guia_elegir]]</p>
        <p class="rec-guia" id="guiaListo" data-solo="preparar" hidden><span class="rec-num">2</span>[[grabacion_rec_guia_listo]]</p>
        <p class="rec-guia" data-solo="cuenta">[[grabacion_rec_guia_cuenta]]</p>
        <p class="rec-guia" data-solo="grabando">[[grabacion_rec_guia_grabando]]</p>
        <p class="rec-guia" data-solo="revisar"><strong>[[grabacion_rec_listo_titulo]]</strong>&nbsp;[[grabacion_rec_listo_ayuda]]</p>

        <div class="rec-fuente" id="filaFuente" data-solo="preparar cuenta grabando" hidden>
            <div>
                <span class="etiqueta">[[grabacion_rec_fuente]]</span>
                <strong id="fuenteNombre">-</strong><span class="detalle" id="fuenteDetalle"></span>
            </div>
            <button type="button" class="btn btn-secundario btn-chico" id="btnCambiar" data-solo="preparar">
                <svg width="16" height="16" aria-hidden="true"><use href="#i-refrescar" /></svg>
                <span>[[grabacion_rec_cambiar]]</span>
            </button>
        </div>

        <div class="aviso aviso-alerta" id="avisoFuente" data-solo="preparar cuenta grabando" hidden>
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen">[[grabacion_rec_aviso_no_completa]]</p>
        </div>

        <div class="previa-captura" data-solo="preparar cuenta grabando revisar">
            <div class="visor">
                <video id="previaVideo" muted playsinline hidden></video>
                <div class="visor-vacio" id="visorVacio">
                    <svg width="40" height="40" aria-hidden="true"><use href="#i-video" /></svg>
                    <p>[[grabacion_rec_sin_pantalla]]</p>
                    <button type="button" class="btn btn-primario btn-grande" id="btnElegir">[[grabacion_rec_elegir]]</button>
                </div>
                <div class="visor-cuenta" id="visorCuenta" hidden>
                    <strong id="cuentaNumero">0</strong>
                    <span>[[grabacion_rec_cuenta_ayuda]]</span>
                </div>
                <div class="indicador-rec visor-rec" data-solo="grabando">
                    <span class="punto" aria-hidden="true"></span>
                    <span>[[nuevagrabacion_grabando]]</span>
                </div>
            </div>
        </div>

        <div class="rec-retraso" role="radiogroup" aria-labelledby="etiquetaRetraso" data-solo="preparar">
            <span class="etiqueta" id="etiquetaRetraso">[[grabacion_rec_retraso]]</span>
            <div class="segmentos">
                <asp:Repeater ID="rptRetrasos" runat="server"><ItemTemplate><label class="segmento"><input type="radio" name="grRetraso" value='<%#: Eval("Valor") %>'<%# Container.ItemIndex == 0 ? " checked" : "" %> /><span><%#: Eval("Texto") %></span></label></ItemTemplate></asp:Repeater>
            </div>
        </div>

        <div class="rec-estado" data-solo="grabando">
            <div class="rec-tiempo">
                <span class="punto" aria-hidden="true"></span>
                <strong id="relojGrabacion" role="timer" aria-live="off">00:00:00</strong>
            </div>
            <div class="dato-rec">
                <span>[[grabacion_rec_chip_pantalla]]</span>
                <strong id="chipPantalla">-</strong>
            </div>
            <div class="dato-rec ok" id="chipMirada">
                <span>[[grabacion_rec_chip_mirada]]</span>
                <strong id="chipMiradaTexto">-</strong>
            </div>
        </div>

        <div class="rec-datos" data-solo="revisar">
            <div><span>[[grabacion_rec_duracion]]</span><strong id="resDuracion">-</strong></div>
            <div><span>[[grabacion_rec_muestras]]</span><strong id="resMuestras">-</strong></div>
            <div><span>[[grabacion_rec_tamano]]</span><strong id="resTamano">-</strong></div>
        </div>

        <div class="aviso aviso-alerta" id="avisoSinMuestras" data-solo="revisar" hidden>
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen">[[grabacion_rec_sin_muestras]]</p>
        </div>

        <div class="aviso aviso-peligro" id="avisoGuardar" data-solo="revisar" hidden>
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen">[[grabacion_rec_error_guardar]]</p>
        </div>

        <!-- <progress> arranca en 0 y el script le va subiendo "value" (0-100) a medida
             que el servidor confirma cada parte del video -->
        <div class="subida" id="subida" data-solo="revisar" hidden>
            <progress id="barraSubida" max="100" value="0"></progress>
            <p class="sin-margen" id="textoSubida">[[grabacion_rec_subiendo]]</p>
        </div>

        <div class="exito-grabacion" data-solo="guardado">
            <svg width="44" height="44" aria-hidden="true"><use href="#i-tilde" /></svg>
            <h3 class="sin-margen">[[grabacion_rec_guardada]]</h3>
            <p class="sin-margen" id="exitoTexto"></p>

            <div class="exito-opciones">
                <div class="exito-opcion">
                    <a class="btn btn-primario btn-grande" href='<%: ResolveUrl("~/Visualizaciones.aspx") %>'>[[grabacion_ok_ver]]</a>
                    <p class="ayuda sin-margen">[[grabacion_ok_ver_ayuda]]</p>
                </div>
                <div class="exito-opcion">
                    <button type="button" class="btn btn-secundario btn-grande" id="btnOtraParte">[[grabacion_ok_otra]]</button>
                    <p class="ayuda sin-margen" id="exitoOtra"></p>
                </div>
            </div>
        </div>

        <div class="aviso aviso-peligro" id="avisoAgente" data-solo="preparar" hidden>
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen">[[grabacion_rec_error_agente]]</p>
        </div>

        <div class="aviso aviso-peligro" id="avisoMirada" data-solo="grabando" hidden>
            <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen">[[grabacion_rec_mirada_perdida]]</p>
        </div>

        <div class="controles-grabacion">
            <button type="button" class="btn btn-fantasma izquierda" data-ir-paso="2" data-solo="preparar">[[nuevagrabacion_volver]]</button>
            <button type="button" class="btn btn-primario btn-grande" id="btnIniciarRec" data-solo="preparar" disabled>
                <svg width="17" height="17" aria-hidden="true"><use href="#i-play" /></svg>
                <span>[[nuevagrabacion_iniciar_la_grabacion]]</span>
            </button>
            <button type="button" class="btn btn-secundario" id="btnCancelarCuenta" data-solo="cuenta">[[categorias_cancelar]]</button>
            <button type="button" class="btn btn-secundario izquierda" data-abre-modal="modalCancelar" data-solo="grabando">[[categorias_cancelar]]</button>
            <button type="button" class="btn btn-primario btn-grande" id="btnFinalizar" data-solo="grabando">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg>
                <span>[[nuevagrabacion_finalizar]]</span>
            </button>
            <button type="button" class="btn btn-peligro izquierda" id="btnDescartarRec" data-abre-modal="modalCancelar" data-solo="revisar">[[grabacion_rec_descartar]]</button>
            <button type="button" class="btn btn-primario btn-grande" id="btnGuardar" data-solo="revisar">[[grabacion_rec_guardar]]</button>
        </div>
    </section>

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
            <button type="button" class="btn btn-secundario" data-cierra-modal>[[grabacion_rec_no_descartar]]</button>
            <button type="button" class="btn btn-peligro" id="btnDescartar" data-cierra-modal>[[nuevagrabacion_si_descartar_todo]]</button>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        // COMO FUNCIONA LA GRABACION 
        // pantalla con api del navegador navigator.mediaDevices.getDisplayMedia().
        // el navegador muestra su selector y devuelve un mediastream. el navegador no puede leer el tobii eso
        // lo hace el agente local por HTTP con
        //    fetch(): /estado (esta conectado?)
        //    /iniciar, /detener y / archivo(el JSON con las muestras de mirada).
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
                if (numero === 3 && !listo) return;

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

                sondear(numero === 2);

                if (numero === 3) limpiar(); else soltarPantalla();

                window.scrollTo(0, 0);
            }

            var botonesPaso = document.querySelectorAll("[data-ir-paso]");

            for (var i = 0; i < botonesPaso.length; i++) {
                botonesPaso[i].addEventListener("click", function () {
                    irAPaso(parseInt(this.getAttribute("data-ir-paso"), 10));
                });
            }

            var perfil = document.querySelectorAll("[data-perfil]");
            var btnContinuar = document.getElementById("btnContinuar");

            var campoNombre = document.getElementById("grNombre");

            // habilita "Continuar" solo si estan la categoria, el perfil del tester y el nombre
            function revisarPerfil() {
                var completo = campoNombre.value.trim() !== "";

                for (var i = 0; i < perfil.length; i++) {
                    if (!perfil[i].value) completo = false;
                }

                btnContinuar.disabled = !completo;
            }

            for (var i = 0; i < perfil.length; i++) {
                perfil[i].addEventListener("change", revisarPerfil);
            }

            campoNombre.addEventListener("input", revisarPerfil);

            // el navegador puede restaurar los valores (recargar, volver atras) SIN avisar con "change":
            // por eso se revisa una vez al cargar y cada vez que la pagina se muestra de nuevo
            revisarPerfil();
            window.addEventListener("pageshow", revisarPerfil);

            var panel = document.getElementById("panelPaso2");
            var resumen = document.getElementById("resumenDispositivo");
            var nota = document.getElementById("notaRecomendada");
            var btnIrAGrabar = document.getElementById("btnIrAGrabar");
            var urlAgente = panel.getAttribute("data-agente");
            var esperaMs = Number(panel.getAttribute("data-espera"));
            var sondeoMs = Number(panel.getAttribute("data-sondeo"));
            var ultimo = null;
            var listo = false;
            var sondeo = null;

            function consultar(ruta) {
                return fetch(urlAgente + ruta, { cache: "no-store", signal: AbortSignal.timeout(esperaMs) })
                    .then(function (respuesta) { return respuesta.json(); })
                    .catch(function () { return null; });
            }

            function estadoDeFila(fila, agente) {
                var casilla = fila.querySelector("input");

                if (casilla) return casilla.checked ? "correcto" : "pendiente";

                var cumplido = {
                    agente: agente,
                    tobii: agente && ultimo.conectado,
                    pantalla: !!(navigator.mediaDevices && navigator.mediaDevices.getDisplayMedia && window.MediaRecorder)
                };

                return cumplido[fila.getAttribute("data-chequeo")] ? "correcto" : "problema";
            }

            function pintar() {
                var agente = ultimo !== null;
                var filas = panel.querySelectorAll("[data-chequeo]");

                for (var i = 0; i < filas.length; i++) {
                    var estado = estadoDeFila(filas[i], agente);
                    var texto = estado;

                    if (!agente && filas[i].hasAttribute("data-sinagente")) texto = "sinagente";
                    if (agente && ultimo.simulado && estado === "correcto" && filas[i].hasAttribute("data-simulado")) texto = "simulado";

                    filas[i].className = "chequeo " + estado;
                    filas[i].querySelector(".detalle").textContent = filas[i].getAttribute("data-" + texto);
                }

                listo = !panel.querySelector("[data-obligatorio]:not(.correcto)");
                panel.classList.toggle("sin-agente", !agente);
                resumen.className = "aviso resumen " + (listo ? "aviso-exito" : "aviso-peligro");
                nota.hidden = !panel.querySelector(".pendiente");
                btnIrAGrabar.disabled = !listo;
            }

            function verificar() {
                return consultar("/estado").then(function (datos) {
                    ultimo = datos;
                    pintar();
                });
            }

            function sondear(activo) {
                clearInterval(sondeo);

                if (!activo) return;

                verificar();
                sondeo = setInterval(verificar, sondeoMs);
            }

            document.getElementById("btnVerificar").addEventListener("click", verificar);
            panel.addEventListener("change", pintar);

            panel.addEventListener("click", function (evento) {
                if (evento.target.closest("[data-abre-tobii]")) consultar("/tobii");
            });

            var panel3 = document.getElementById("panelPaso3");
            var previa = document.getElementById("previaVideo");
            var visorVacio = document.getElementById("visorVacio");
            var visorCuenta = document.getElementById("visorCuenta");
            var cuentaNumero = document.getElementById("cuentaNumero");
            var reloj = document.getElementById("relojGrabacion");
            var fuenteNombre = document.getElementById("fuenteNombre");
            var fuenteDetalle = document.getElementById("fuenteDetalle");
            var avisoFuente = document.getElementById("avisoFuente");
            var avisoAgente = document.getElementById("avisoAgente");
            var avisoMirada = document.getElementById("avisoMirada");
            var filaFuente = document.getElementById("filaFuente");
            var guiaElegir = document.getElementById("guiaElegir");
            var guiaListo = document.getElementById("guiaListo");
            var chipPantalla = document.getElementById("chipPantalla");
            var chipMirada = document.getElementById("chipMirada");
            var chipMiradaTexto = document.getElementById("chipMiradaTexto");
            var btnCambiar = document.getElementById("btnCambiar");
            var avisoSinMuestras = document.getElementById("avisoSinMuestras");
            var avisoGuardar = document.getElementById("avisoGuardar");
            var btnElegir = document.getElementById("btnElegir");
            var btnIniciarRec = document.getElementById("btnIniciarRec");
            var btnCancelarCuenta = document.getElementById("btnCancelarCuenta");
            var btnFinalizar = document.getElementById("btnFinalizar");
            var btnGuardar = document.getElementById("btnGuardar");
            var btnDescartar = document.getElementById("btnDescartar");
            var bitrate = Number(panel3.getAttribute("data-bitrate"));
            var anchoVideo = Number(panel3.getAttribute("data-ancho"));
            var altoVideo = Number(panel3.getAttribute("data-alto"));
            var fpsVideo = Number(panel3.getAttribute("data-fps"));
            var sondeoGrabacionMs = Number(panel3.getAttribute("data-sondeo"));
            var nombresFuente = {
                monitor: "[[js:grabacion_rec_fuente_monitor]]",
                window: "[[js:grabacion_rec_fuente_window]]",
                browser: "[[js:grabacion_rec_fuente_browser]]"
            };
            var textoMiradaOk = "[[js:grabacion_rec_mirada_ok]]";
            var textoMiradaPerdida = "[[js:grabacion_rec_mirada_sin_senal]]";

            var flujo = null;
            var grabador = null;
            var trozos = [];
            var grabando = false;
            var detenido = null;
            var inicioVideo = 0;
            var finVideo = 0;
            var inicioMirada = 0;
            var resumenMirada = null;
            var videoFinal = null;
            var urlVideo = null;
            var temporizadorCuenta = null;
            var temporizadorReloj = null;
            var temporizadorMirada = null;

            function ponerEstado(estado) {
                panel3.setAttribute("data-estado", estado);
            }

            function protegerSalida(proteger) {
                window.onbeforeunload = proteger ? function (evento) { evento.preventDefault(); evento.returnValue = ""; } : null;
            }

            function dosDigitos(n) {
                return (n < 10 ? "0" : "") + n;
            }

            function formatoTiempo(ms) {
                var total = Math.floor(ms / 1000);

                return dosDigitos(Math.floor(total / 3600)) + ":" + dosDigitos(Math.floor((total % 3600) / 60)) + ":" + dosDigitos(total % 60);
            }

            function soltarPantalla() {
                if (flujo) {
                    flujo.getTracks().forEach(function (pista) { pista.stop(); });
                    flujo = null;
                }
            }

            function vaciarVisor() {
                previa.pause();
                previa.controls = false;
                previa.removeAttribute("src");
                previa.srcObject = null;
                previa.hidden = true;
                visorVacio.hidden = false;

                if (urlVideo) {
                    URL.revokeObjectURL(urlVideo);
                    urlVideo = null;
                }
            }

            function limpiar() {
                clearInterval(temporizadorCuenta);
                clearInterval(temporizadorReloj);
                clearInterval(temporizadorMirada);
                protegerSalida(false);
                soltarPantalla();
                vaciarVisor();

                trozos = [];
                grabando = false;
                videoFinal = null;
                resumenMirada = null;
                fuenteNombre.textContent = "-";
                fuenteDetalle.textContent = "";
                filaFuente.hidden = true;
                guiaElegir.hidden = false;
                guiaListo.hidden = true;
                visorCuenta.hidden = true;
                avisoFuente.hidden = true;
                avisoAgente.hidden = true;
                avisoMirada.hidden = true;
                avisoSinMuestras.hidden = true;
                avisoGuardar.hidden = true;
                btnGuardar.disabled = false;
                btnIniciarRec.disabled = true;
                ponerEstado("preparar");
            }

            function mostrarPantalla(nuevo) {
                var pista = nuevo.getVideoTracks()[0];
                var ajustes = pista.getSettings();
                var tipo = ajustes.displaySurface;
                var detalle = [];

                if (tipo !== "monitor" && pista.label) detalle.push(pista.label);
                if (ajustes.width && ajustes.height) detalle.push(ajustes.width + " \u00d7 " + ajustes.height);
                if (ajustes.frameRate) detalle.push(Math.round(ajustes.frameRate) + " fps");

                fuenteNombre.textContent = nombresFuente[tipo] || pista.label || "[[js:grabacion_rec_chip_pantalla]]";
                fuenteDetalle.textContent = detalle.join(" \u00b7 ");
                filaFuente.hidden = false;
                guiaElegir.hidden = true;
                guiaListo.hidden = false;
                avisoFuente.hidden = tipo !== "window" && tipo !== "browser";

                previa.srcObject = nuevo;
                previa.hidden = false;
                visorVacio.hidden = true;
                previa.play();

                btnIniciarRec.disabled = false;

                pista.addEventListener("ended", function () {
                    if (flujo !== nuevo) return;

                    if (grabando) {
                        terminar(false);
                    } else {
                        limpiar();
                    }
                });
            }

            // pide la pantalla con getDisplayMedia
            function elegirPantalla() {
                navigator.mediaDevices.getDisplayMedia({
                    video: {
                        displaySurface: "monitor",
                        width: { ideal: anchoVideo, max: anchoVideo },
                        height: { ideal: altoVideo, max: altoVideo },
                        frameRate: { ideal: fpsVideo, max: fpsVideo }
                    },
                    audio: false
                })
                    .then(function (nuevo) {
                        soltarPantalla();
                        flujo = nuevo;
                        mostrarPantalla(nuevo);
                    })
                    .catch(function () { });
            }

            function mostrarCuenta(restante) {
                cuentaNumero.textContent = restante;
                visorCuenta.hidden = false;
            }

            function cancelarCuenta() {
                clearInterval(temporizadorCuenta);
                visorCuenta.hidden = true;
                ponerEstado("preparar");
            }

            function comenzar() {
                var retraso = Number(document.querySelector("input[name='grRetraso']:checked").value);

                avisoAgente.hidden = true;

                if (retraso === 0) {
                    empezar();
                    return;
                }

                ponerEstado("cuenta");
                mostrarCuenta(retraso);

                temporizadorCuenta = setInterval(function () {
                    retraso--;

                    if (retraso > 0) {
                        mostrarCuenta(retraso);
                        return;
                    }

                    clearInterval(temporizadorCuenta);
                    empezar();
                }, 1000);
            }

            //   1. Se inicia al agente (/iniciar) y empieza a guardar mirada y devuelve su hora de inicio
            //   2. Se crea el MediaRecorder sobre el MediaStream
            //   3. recorder.start(1000) devuelve una parte cada 1000ms
            //   4. onstart marca inicioVideo (Date.now()) cuando el MediaRecorder realmente arranca.
            function empezar() {
                visorCuenta.hidden = true;

                consultar("/iniciar").then(function (datos) {
                    if (!datos || !datos.inicio_utc) {
                        avisoAgente.hidden = false;
                        ponerEstado("preparar");
                        return;
                    }

                    var formato = MediaRecorder.isTypeSupported("video/webm;codecs=vp9") ? "video/webm;codecs=vp9" : "video/webm";

                    inicioMirada = datos.inicio_utc;
                    trozos = [];
                    // videoKeyFrameIntervalDuration: un cuadro completo cada 400 ms. Sin esto Chrome casi no los pone y,
                    // al adelantar un video pesado, el visor tiene que decodificar desde el principio y queda congelado
                    grabador = new MediaRecorder(flujo, { mimeType: formato, videoBitsPerSecond: bitrate, videoKeyFrameIntervalDuration: 400 });
                    grabador.ondataavailable = function (evento) {
                        if (evento.data.size > 0) trozos.push(evento.data);
                    };
                    grabador.onstart = function () {
                        inicioVideo = Date.now();
                    };
                    grabador.start(1000);

                    grabando = true;
                    reloj.textContent = "00:00:00";
                    chipPantalla.textContent = fuenteNombre.textContent + " \u00b7 " + fuenteDetalle.textContent;
                    pintarMirada(true, 0);
                    ponerEstado("grabando");
                    protegerSalida(true);

                    temporizadorReloj = setInterval(function () {
                        reloj.textContent = formatoTiempo(Date.now() - inicioVideo);
                    }, 500);

                    temporizadorMirada = setInterval(function () {
                        consultar("/estado").then(function (estado) {
                            var bien = !!(estado && estado.conectado && estado.grabando);

                            pintarMirada(bien, bien ? Number(estado.muestras) || 0 : 0);
                        });
                    }, sondeoGrabacionMs);
                });
            }

            function pintarMirada(bien, muestras) {
                avisoMirada.hidden = bien;
                chipMirada.className = "dato-rec " + (bien ? "ok" : "mal");
                chipMiradaTexto.textContent = bien ? textoMiradaOk.replace("{0}", muestras) : textoMiradaPerdida;
            }

            function terminar(descartando) {
                if (!grabando) return;

                grabando = false;
                clearInterval(temporizadorReloj);
                clearInterval(temporizadorMirada);
                finVideo = Date.now();
                detenido = consultar("/detener");

                grabador.onstop = function () {
                    soltarPantalla();

                    detenido.then(function (datos) {
                        if (descartando) {
                            limpiar();
                            return;
                        }

                        resumenMirada = datos;
                        videoFinal = new Blob(trozos, { type: "video/webm" });
                        mostrarResumen();
                    });
                };

                grabador.stop();
            }

            function mostrarResumen() {
                var muestras = resumenMirada && resumenMirada.muestras ? resumenMirada.muestras : 0;

                vaciarVisor();
                urlVideo = URL.createObjectURL(videoFinal);
                previa.src = urlVideo;
                previa.controls = true;
                previa.hidden = false;
                visorVacio.hidden = true;

                document.getElementById("resDuracion").textContent = formatoTiempo(finVideo - inicioVideo);
                document.getElementById("resMuestras").textContent = muestras;
                document.getElementById("resTamano").textContent = (videoFinal.size / 1048576).toFixed(1) + " MB";
                avisoSinMuestras.hidden = muestras > 0;
                avisoGuardar.hidden = true;
                ponerEstado("revisar");
            }

            var btnOtraParte = document.getElementById("btnOtraParte");
            var subida = document.getElementById("subida");
            var barraSubida = document.getElementById("barraSubida");
            var textoSubida = document.getElementById("textoSubida");
            var textoExito = "[[js:grabacion_ok_texto]]";
            var textoOtra = "[[js:grabacion_ok_otra_ayuda]]";
            var textoErrorSubida = "[[js:grabacion_rec_error_guardar]]";
            var siguienteNombre = "";

            function bloquear(bloqueado) {
                btnGuardar.disabled = bloqueado;
                document.getElementById("btnDescartarRec").disabled = bloqueado;
                subida.hidden = !bloqueado;
            }

            function mostrarError(mensaje) {
                avisoGuardar.querySelector("p").textContent = mensaje || textoErrorSubida;
                avisoGuardar.hidden = false;
                bloquear(false);
            }

            var urlMetodos = "<%: ResolveUrl("~/NuevaGrabacion_WebMethod.asmx") %>/";
            var TAMANO_PARTE = 2 * 1024 * 1024;
            var INTENTOS = 3;

            function llamar(metodo, cuerpo) {
                // aca se hace el fetch con el ajax para actualizar la barra
                return fetch(urlMetodos + metodo, {
                    method: "POST",
                    headers: { "Content-Type": "application/json; charset=utf-8" },
                    body: JSON.stringify(cuerpo || {})
                })
                    .then(function (respuesta) {
                        if (!respuesta.ok) throw new Error();

                        return respuesta.json();
                    })
                    .then(function (json) {
                        if (!json.d || !json.d.ok) {
                            var error = new Error(json.d && json.d.mensaje);
                            error.definitivo = true;
                            throw error;
                        }

                        return json.d;
                    });
            }

            function conReintentos(metodo, cuerpo, intento) {
                return llamar(metodo, cuerpo).catch(function (error) {
                    if (error.definitivo || (intento || 1) >= INTENTOS) throw error;

                    return new Promise(function (resolver) { setTimeout(resolver, 800 * (intento || 1)); })
                        .then(function () { return conReintentos(metodo, cuerpo, (intento || 1) + 1); });
                });
            }

            function aBase64(trozo) {
                return new Promise(function (resolver, rechazar) {
                    var lector = new FileReader();

                    lector.onload = function () { resolver(String(lector.result).split(",")[1] || ""); };
                    lector.onerror = rechazar;
                    lector.readAsDataURL(trozo);
                });
            }

            // como avanza la barra :
            //   - El video se corta en partes de 2 MB (TAMANO_PARTE) con video.slice
            //   - Cada parte se sube con SubirParte y cuando el servidor responde OK se actualiza la barra
            function subirPartes(token, video, desde) {
                if (desde >= video.size) return Promise.resolve();

                return aBase64(video.slice(desde, desde + TAMANO_PARTE))
                    .then(function (base64) {
                        return conReintentos("SubirParte", { token: token, desde: desde, base64: base64 });
                    })
                    .then(function () {
                        var enviado = Math.min(desde + TAMANO_PARTE, video.size);

                        barraSubida.value = Math.round(enviado * 100 / video.size);

                        return subirPartes(token, video, enviado);
                    });
            }

            function subir(video, mirada) {
                var datos = {
                    idCategoria: parseInt(document.getElementById("grCategoria").value, 10),
                    idDispositivo: parseInt(document.getElementById("grDispositivo").value, 10),
                    nombre: document.getElementById("grNombre").value,
                    objetivo: document.getElementById("grObjetivo").value,
                    edad: document.getElementById("grEdad").value,
                    genero: document.getElementById("grGenero").value,
                    familiaridad: document.getElementById("grFamiliaridad").value,
                    duracion: Math.round((finVideo - inicioVideo) / 1000),
                    desfaseMiradaMs: resumenMirada && inicioMirada ? Math.round(inicioMirada - inicioVideo) : 0
                };
                var token = null;

                barraSubida.value = 0; // empieza vacia; bloquear(true) hace visible el contenedor .subida
                textoSubida.textContent = "[[js:grabacion_rec_subiendo]]";
                bloquear(true);

                (mirada && mirada.size > 0 ? mirada.text() : Promise.resolve(""))
                    .then(function (textoMirada) {
                        return conReintentos("IniciarSubida")
                            .then(function (r) {
                                token = r.token;

                                return subirPartes(token, video, 0);
                            })
                            .then(function () {
                                return llamar("FinalizarSubida", { token: token, datos: datos, mirada: textoMirada });
                            });
                    })
                    .then(function (r) {
                        siguienteNombre = r.siguienteNombre;
                        document.getElementById("exitoTexto").textContent = textoExito.replace("{0}", datos.nombre);
                        document.getElementById("exitoOtra").textContent = textoOtra.replace("{0}", siguienteNombre);
                        bloquear(false);
                        vaciarVisor();
                        protegerSalida(false);
                        ponerEstado("guardado");
                    })
                    .catch(function (error) {
                        mostrarError(error && error.definitivo ? error.message : null);
                    });
            }

            function guardar() {
                avisoGuardar.hidden = true;
                bloquear(true);
                textoSubida.textContent = "[[js:grabacion_rec_subiendo]]";

                fetch(urlAgente + "/archivo", { cache: "no-store" })
                    .then(function (respuesta) {
                        if (!respuesta.ok) throw new Error();

                        return respuesta.blob();
                    })
                    .then(function (mirada) {
                        subir(videoFinal, mirada);
                    })
                    .catch(function () {
                        mostrarError();
                    });
            }

            function otraParte() {
                document.getElementById("grNombre").value = siguienteNombre;
                limpiar();
            }

            function descartar() {
                if (grabando) {
                    terminar(true);
                } else {
                    limpiar();
                }
            }

            btnElegir.addEventListener("click", elegirPantalla);
            btnCambiar.addEventListener("click", elegirPantalla);
            btnIniciarRec.addEventListener("click", comenzar);
            btnCancelarCuenta.addEventListener("click", cancelarCuenta);
            btnFinalizar.addEventListener("click", function () { terminar(false); });
            btnGuardar.addEventListener("click", guardar);
            btnOtraParte.addEventListener("click", otraParte);
            btnDescartar.addEventListener("click", descartar);
        })();
    </script>
</asp:Content>
