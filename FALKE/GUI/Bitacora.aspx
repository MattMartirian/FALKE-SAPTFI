<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Bitacora.aspx.cs" Inherits="GUI.Bitacora" Title="Bitácora" %>
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
            <h1 data-i18n="bitacora.titulo">Bitácora</h1>
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
                <label for="txtDesde" data-i18n="bitacora.desde">Desde</label>
                <asp:TextBox ID="txtDesde" runat="server" TextMode="Date" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="txtHasta" data-i18n="bitacora.hasta">Hasta</label>
                <asp:TextBox ID="txtHasta" runat="server" TextMode="Date" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="txtHoraDesde" data-i18n="bitacora.horaDesde">Hora desde</label>
                <asp:TextBox ID="txtHoraDesde" runat="server" TextMode="Time" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="txtHoraHasta" data-i18n="bitacora.horaHasta">Hora hasta</label>
                <asp:TextBox ID="txtHoraHasta" runat="server" TextMode="Time" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="ddlModulo" data-i18n="bitacora.modulo">Módulo</label>
                <asp:DropDownList ID="ddlModulo" runat="server" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="ddlAccion" data-i18n="bitacora.accion">Acción</label>
                <asp:DropDownList ID="ddlAccion" runat="server" CssClass="entrada" ClientIDMode="Static" />
            </div>

            <div class="campo">
                <label for="txtUsuario" data-i18n="bitacora.usuario">Usuario</label>
                <asp:TextBox ID="txtUsuario" runat="server" CssClass="entrada" MaxLength="100" placeholder="Nombre o correo" autocomplete="off" ClientIDMode="Static" />
                <p class="ayuda">La lista se filtra mientras escribís.</p>
            </div>

            <asp:PlaceHolder ID="phFiltroEmpresa" runat="server">
                <div class="campo">
                    <label for="ddlEmpresa" data-i18n="bitacora.empresa">Empresa</label>
                    <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="entrada" ClientIDMode="Static" />
                </div>
            </asp:PlaceHolder>

            <div class="campo">
                <label for="ddlCriticidad" data-i18n="bitacora.criticidad">Criticidad</label>
                <asp:DropDownList ID="ddlCriticidad" runat="server" CssClass="entrada" ClientIDMode="Static" />
            </div>

        </div>

        <div class="fila-entre">
            <p class="texto-chico texto-suave sin-margen" role="status" id="contadorBitacora"><asp:Literal ID="litContador" runat="server" /></p>
            <div class="fila">
                <asp:Button ID="btnLimpiar" runat="server" CssClass="btn btn-fantasma btn-chico" Text="Limpiar filtros" OnClick="btnLimpiar_Click" CausesValidation="false" />
                <asp:Button ID="btnBuscar" runat="server" CssClass="btn btn-secundario btn-chico" Text="Aplicar filtros" OnClick="btnBuscar_Click" />
            </div>
        </div>

    </asp:Panel>

    <div id="resultadosBitacora">
    <div class="tabla-scroll">
        <table class="tabla" id="tablaBitacora">
            <caption class="solo-lectores">Registros de la bitácora</caption>
            <thead>
                <tr>
                    <th scope="col" data-i18n="bitacora.col.fecha">Fecha y hora</th>
                    <th scope="col" data-i18n="bitacora.col.usuario">Usuario</th>
                    <th scope="col" data-i18n="bitacora.col.empresa">Empresa</th>
                    <th scope="col" data-i18n="bitacora.col.modulo">Módulo</th>
                    <th scope="col" data-i18n="bitacora.col.descripcion">Descripción</th>
                    <th scope="col" data-i18n="bitacora.col.criticidad">Criticidad</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptBitacora" runat="server">
                    <ItemTemplate>
                        <tr class="<%#: ClaseFila((TE.CriticidadBitacora)Eval("CriticidadBitacora")) %>">
                            <td class="celda-fecha"><%#: ((DateTime)Eval("FechaHoraBitacora")).ToString("dd/MM/yyyy HH:mm") %></td>
                            <td><%#: Eval("Actor") %><%# string.IsNullOrEmpty((string)Eval("EmailActor")) ? "" : "<br /><span class=\"texto-chico texto-suave\">" + HttpUtility.HtmlEncode((string)Eval("EmailActor")) + "</span>" %></td>
                            <td class="texto-suave"><%#: Eval("NombreEmpresa") %></td>
                            <td><span class="badge badge-neutro"><%#: Eval("ModuloBitacora") %></span></td>
                            <td><%#: Eval("DescripcionBitacora") %></td>
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
            <asp:LinkButton ID="lnkAnterior" runat="server" CssClass="btn btn-fantasma btn-chico" OnClick="lnkAnterior_Click" CausesValidation="false">Anterior</asp:LinkButton>
            <asp:LinkButton ID="lnkSiguiente" runat="server" CssClass="btn btn-fantasma btn-chico" OnClick="lnkSiguiente_Click" CausesValidation="false">Siguiente</asp:LinkButton>
        </div>
    </div>

    <asp:PlaceHolder ID="phVacio" runat="server" Visible="false">
        <div class="vacio">
            <svg width="34" height="34" aria-hidden="true"><use href="#i-libro" /></svg>
            <p class="sin-margen" data-i18n="bitacora.vacio">No hay registros que coincidan con los filtros.</p>
        </div>
    </asp:PlaceHolder>
    </div>

    <p class="texto-chico texto-suave mt-16" data-i18n="bitacora.retencion">
        Los registros de la bitácora no se pueden editar ni borrar. Se conservan por el
        plazo que fije la política de retención del sistema.
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

            // Pide la página con los filtros actuales (como si se apretara "Aplicar filtros") y cambia solo la tabla y el contador:
            // así se puede seguir escribiendo. El servidor siempre aplica el alcance de la cuenta: cada uno ve solo lo suyo.
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

                        // Si la sesión venció la respuesta es otra página: se envía el formulario de la forma común.
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
