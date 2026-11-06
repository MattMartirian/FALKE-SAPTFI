<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Bitacora.aspx.cs" Inherits="GUI.Bitacora" Title="[[app_bitacora]]" %>
<%@ MasterType VirtualPath="~/App.master" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .fila-critica td:first-child::before,
        .fila-media td:first-child::before,
        .fila-baja td:first-child::before
        {
            content: "";
            display: inline-block;
            width: 8px;
            height: 8px;
            margin-right: 9px;
            border-radius: 50%;
            vertical-align: middle;
            background: var(--borde-fuerte);
        }
/**/
        .fila-critica td:first-child::before { background: var(--peligro); }
        .fila-media td:first-child::before { background: var(--alerta); }

        .filtros-bitacora
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(170px, 1fr));
            gap: 14px;
            align-items: end;
        }

        .filtros-bitacora .campo
        {
            margin-bottom: 0;
        }

        .celda-fecha
        {
            white-space: nowrap;
            font-variant-numeric: tabular-nums;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="bitacora.titulo">[[app_bitacora]]</h1>
            <p class="texto-suave sin-margen">
                <asp:Literal ID="litBajada" runat="server" />
            </p>
        </div>
    </div>

    <div class="aviso aviso-info">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen">
            <asp:Literal ID="litAlcance" runat="server" />
        </p>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <asp:Panel ID="pnlFiltros" runat="server" CssClass="tarjeta mb-24" DefaultButton="btnBuscar">

        <div class="filtros-bitacora mb-16">

            <div class="campo">
                <label for="txtDesde" data-i18n="bitacora.desde">[[bitacora_desde]]</label>
                <asp:TextBox ID="txtDesde" runat="server" TextMode="Date" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="txtHasta" data-i18n="bitacora.hasta">[[bitacora_hasta]]</label>
                <asp:TextBox ID="txtHasta" runat="server" TextMode="Date" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="txtHoraDesde" data-i18n="bitacora.horaDesde">[[bitacora_hora_desde]]</label>
                <asp:TextBox ID="txtHoraDesde" runat="server" TextMode="Time" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="txtHoraHasta" data-i18n="bitacora.horaHasta">[[bitacora_hora_hasta]]</label>
                <asp:TextBox ID="txtHoraHasta" runat="server" TextMode="Time" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="ddlModulo" data-i18n="bitacora.modulo">[[bitacora_modulo]]</label>
                <asp:DropDownList ID="ddlModulo" runat="server" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="ddlAccion" data-i18n="bitacora.accion">[[bitacora_accion]]</label>
                <asp:DropDownList ID="ddlAccion" runat="server" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="txtUsuario" data-i18n="bitacora.usuario">[[bitacora_usuario]]</label>
                <asp:TextBox ID="txtUsuario" runat="server" CssClass="entrada" MaxLength="100" placeholder="[[bitacora_nombre_o_correo]]" autocomplete="off" ClientIDMode="Static" />
            </div>

            <asp:PlaceHolder ID="phFiltroEmpresa" runat="server">
                <div class="campo">
                    <label for="ddlEmpresa" data-i18n="bitacora.empresa">[[bitacora_empresa]]</label>
                    <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="entrada" ClientIDMode="Static" />
                </div>
            </asp:PlaceHolder>

            <div class="campo">
                <label for="ddlCriticidad" data-i18n="bitacora.criticidad">[[bitacora_criticidad]]</label>
                <asp:DropDownList ID="ddlCriticidad" runat="server" CssClass="entrada" ClientIDMode="Static" />
            </div>

        </div>

        <div class="fila-entre">
            <p class="texto-chico texto-suave sin-margen" role="status" id="contadorBitacora"><asp:Literal ID="litContador" runat="server" /></p>
            <div class="fila">
                <asp:Button ID="btnLimpiar" runat="server" CssClass="btn btn-fantasma btn-chico" Text="[[comun_limpiar_filtros]]" OnClick="btnLimpiar_Click" CausesValidation="false" />
                <asp:Button ID="btnBuscar" runat="server" CssClass="btn btn-secundario btn-chico" Text="[[comun_aplicar_filtros]]" OnClick="btnBuscar_Click" />
            </div>
        </div>

    </asp:Panel>

    <div id="resultadosBitacora">
    <div class="tabla-scroll">
        <table class="tabla" id="tablaBitacora">
            <caption class="solo-lectores">[[bitacora_registros_de_la_bitacora]]</caption>
            <thead>
                <tr>
                    <th scope="col" data-i18n="bitacora.col.fecha">[[bitacora_fecha_y_hora]]</th>
                    <th scope="col" data-i18n="bitacora.col.usuario">[[bitacora_usuario]]</th>
                    <th scope="col" data-i18n="bitacora.col.empresa">[[bitacora_empresa]]</th>
                    <th scope="col" data-i18n="bitacora.col.modulo">[[bitacora_modulo]]</th>
                    <th scope="col" data-i18n="bitacora.col.descripcion">[[bitacora_descripcion]]</th>
                    <th scope="col" data-i18n="bitacora.col.detalle">[[bitacora_detalle]]</th>
                    <th scope="col" data-i18n="bitacora.col.criticidad">[[bitacora_criticidad]]</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptBitacora" runat="server">
                    <ItemTemplate>
                        <tr class="<%#: ClaseFila((TE.CriticidadBitacora)Eval("CriticidadBitacora")) %>">
                            <td class="celda-fecha"><%#: ((DateTime)Eval("FechaHoraBitacora")).ToString("g", GUI.IdiomaActual_GUI.Cultura) %></td>
                            <td><%#: Eval("Actor") %><%# string.IsNullOrEmpty((string)Eval("EmailActor")) ? "" : "<br /><span class=\"texto-chico texto-suave\">" + HttpUtility.HtmlEncode((string)Eval("EmailActor")) + "</span>" %></td>
                            <td class="texto-suave"><%#: Eval("NombreEmpresa") %></td>
                            <td><span class="badge badge-neutro"><%#: GUI.TextoBitacora_GUI.Modulo(Eval("ModuloBitacora")) %></span></td>
                            <td><%#: GUI.TextoBitacora_GUI.Descripcion(Eval("DescripcionBitacora")) %></td>
                            <td class="texto-suave"><%#: GUI.TextoBitacora_GUI.Detalle(Eval("DetalleBitacora")) %></td>
                            <td><span class="badge <%#: ClaseBadge((TE.CriticidadBitacora)Eval("CriticidadBitacora")) %>"><%#: EtiquetaCriticidad((TE.CriticidadBitacora)Eval("CriticidadBitacora")) %></span></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>

    <div class="paginado-bitacora fila-entre mt-16">
        <span class="texto-chico texto-suave"><asp:Literal ID="litPaginado" runat="server" /></span>
        <div class="fila">
            <asp:LinkButton ID="lnkAnterior" runat="server" CssClass="btn btn-fantasma btn-chico" OnClick="lnkAnterior_Click" CausesValidation="false">[[bitacora_anterior]]</asp:LinkButton>
            <asp:LinkButton ID="lnkSiguiente" runat="server" CssClass="btn btn-fantasma btn-chico" OnClick="lnkSiguiente_Click" CausesValidation="false">[[bitacora_siguiente]]</asp:LinkButton>
        </div>
    </div>

    <asp:PlaceHolder ID="phVacio" runat="server" Visible="false">
        <div class="vacio">
            <svg width="34" height="34" aria-hidden="true"><use href="#i-libro" /></svg>
            <p class="sin-margen" data-i18n="bitacora.vacio">[[bitacora_no_hay_registros_que_coincidan_con]]</p>
        </div>
    </asp:PlaceHolder>
    </div>

    <p class="texto-chico texto-suave mt-16" data-i18n="bitacora.retencion">
        [[bitacora_los_registros_de_la_bitacora_no]]
    </p>


</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var caja = document.getElementById("txtUsuario");
            if (!caja || !window.fetch || !window.FormData || !window.DOMParser) return;

            var formulario = caja.form;
            var espera = null;
            var pedido = null;

            function filtrar() {
                var boton = formulario.querySelector('[name$="btnBuscar"]');
                if (!boton) return;

                if (pedido) pedido.abort();
                pedido = window.AbortController ? new AbortController() : null;

                var datos = new FormData(formulario);
                datos.append(boton.name, boton.value);

                var opciones = { method: "POST", body: datos, credentials: "same-origin" };
                if (pedido) opciones.signal = pedido.signal;

                fetch(formulario.getAttribute("action") || window.location.href, opciones)
                    .then(function (respuesta) { return respuesta.text(); })
                    .then(function (html) {
                        var doc = new DOMParser().parseFromString(html, "text/html");
                        var resultados = doc.getElementById("resultadosBitacora");
                        var contador = doc.getElementById("contadorBitacora");

                        if (!resultados || !contador) { boton.click(); return; }

                        document.getElementById("resultadosBitacora").innerHTML = resultados.innerHTML;
                        document.getElementById("contadorBitacora").innerHTML = contador.innerHTML;

                        ["__VIEWSTATE", "__VIEWSTATEGENERATOR", "__EVENTVALIDATION"].forEach(function (nombre) {
                            var nuevo = doc.querySelector('input[name="' + nombre + '"]');
                            var actual = formulario.querySelector('input[name="' + nombre + '"]');
                            if (nuevo && actual) actual.value = nuevo.value;
                        });
                    })
                    .catch(function (error) {
                        if (error && error.name === "AbortError") return;
                        boton.click();
                    });
            }

            caja.addEventListener("input", function () {
                window.clearTimeout(espera);
                espera = window.setTimeout(filtrar, 350);
            });
        })();
    </script>
</asp:Content>
