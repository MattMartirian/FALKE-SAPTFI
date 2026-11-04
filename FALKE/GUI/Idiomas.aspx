<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Idiomas.aspx.cs" Inherits="GUI.Idiomas" Title="[[app_idiomas]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .paginado
        {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
            margin-top: 14px;
            font-size: .88rem;
            color: var(--texto-suave);
        }

        .fila-deshabilitada > td:not(.celda-acciones)
        {
            opacity: .55;
        }

        .texto-largo
        {
            max-width: 420px;
            white-space: pre-wrap;
            word-break: break-word;
        }

        .tabla-fija
        {
            table-layout: fixed;
            width: 100%;
        }

        .tabla-fija th,
        .tabla-fija td
        {
            vertical-align: top;
        }

        .celda-fija
        {
            white-space: normal;
            overflow-wrap: anywhere;
            word-break: break-word;
        }

        .consecuencias
        {
            margin: 0 0 16px;
            padding-left: 20px;
        }

        .consecuencias li
        {
            margin-bottom: 4px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1>[[app_idiomas]]</h1>
            <p class="texto-suave sin-margen">
                [[idiomas_bajada]]
            </p>
        </div>
        <div class="acciones">
            <button type="button" class="btn btn-primario" data-abre-modal="modalAgregar">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span>[[idiomas_agregar_idioma]]</span>
            </button>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <div class="metricas mb-24">
        <div>
            <div class="metrica-valor"><asp:Literal ID="litActivos" runat="server" /></div>
            <div class="metrica-nombre">[[idiomas_habilitados]]</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litTextos" runat="server" /></div>
            <div class="metrica-nombre">[[idiomas_textos_catalogo]]</div>
        </div>
    </div>

    <div class="tabla-scroll">
        <table class="tabla" id="tablaIdiomas">
            <caption class="solo-lectores">[[idiomas_tabla_idiomas]]</caption>
            <thead>
                <tr>
                    <th scope="col">[[idiomas_col_idioma]]</th>
                    <th scope="col">[[dispositivos_estado]]</th>
                    <th scope="col">[[idiomas_col_traducciones]]</th>
                    <th scope="col"><span class="solo-lectores">[[categorias_acciones]]</span></th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptIdiomas" runat="server" OnItemCommand="rptIdiomas_ItemCommand">
                    <ItemTemplate>
                        <tr class="<%#: ((TE.Idioma_TE)Container.DataItem).Activo ? "" : "fila-deshabilitada" %>">
                            <td>
                                <div class="celda-doble">
                                    <div>
                                        <div class="principal"><%#: Eval("NombreIdioma") %></div>
                                        <div class="secundario mono"><%#: Eval("CodigoIdioma") %></div>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <asp:PlaceHolder runat="server" Visible='<%# ((TE.Idioma_TE)Container.DataItem).Activo %>'>
                                    <span class="badge badge-exito">[[idiomas_estado_habilitado]]</span>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" Visible='<%# !((TE.Idioma_TE)Container.DataItem).Activo %>'>
                                    <span class="badge badge-neutro">[[idiomas_estado_deshabilitado]]</span>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" Visible='<%# ((TE.Idioma_TE)Container.DataItem).EsBase %>'>
                                    <span class="badge badge-info">[[idiomas_base]]</span>
                                </asp:PlaceHolder>
                            </td>
                            <td><%#: Eval("CantidadTraducciones") %></td>
                            <td class="celda-acciones">
                                <div class="acciones-fila">
                                    <asp:LinkButton runat="server" CssClass="btn btn-secundario btn-chico" CommandName="revisar" CommandArgument='<%# Eval("IdIdioma") %>' CausesValidation="false">[[idiomas_col_traducciones]]</asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="btn btn-secundario btn-chico" CommandName="deshabilitar" CommandArgument='<%# Eval("IdIdioma") %>' CausesValidation="false"
                                                    Visible='<%# ((TE.Idioma_TE)Container.DataItem).Activo %>' Enabled='<%# PuedeModificar(Container.DataItem) %>'>[[idiomas_deshabilitar]]</asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="btn btn-secundario btn-chico" CommandName="habilitar" CommandArgument='<%# Eval("IdIdioma") %>' CausesValidation="false"
                                                    Visible='<%# !((TE.Idioma_TE)Container.DataItem).Activo %>'>[[idiomas_habilitar]]</asp:LinkButton>
                                    <button type="button" class="btn btn-peligro btn-chico" data-abre-modal="modalEliminar"
                                            data-idioma="<%#: Eval("IdIdioma") %>" data-nombre="<%#: Eval("NombreIdioma") %>" <%# PuedeModificar(Container.DataItem) ? "" : "disabled" %>>[[roles_eliminar]]</button>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>

    <asp:PlaceHolder ID="phTraducciones" runat="server" Visible="false">
        <div class="pagina-cabecera mt-24">
            <div>
                <h2>[[idiomas_traducciones_de]] <asp:Literal ID="litIdiomaNombre" runat="server" /></h2>
                <p class="texto-suave sin-margen">[[idiomas_cada_texto]]</p>
                <p class="texto-suave sin-margen">[[idiomas_fallback_ayuda]]</p>
            </div>
        </div>

        <asp:Panel ID="pnlFiltros" runat="server" DefaultButton="btnBuscar">
            <div class="filtros">
                <div class="campo">
                    <label for="txtBuscar">[[categorias_buscar]]</label>
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="entrada" placeholder="[[idiomas_clave_o_texto]]" MaxLength="100" ClientIDMode="Static" />
                </div>
                <div class="campo">
                    <span class="etiqueta">[[idiomas_mostrar]]</span>
                    <label class="casilla-simple">
                        <asp:CheckBox ID="chkPendientes" runat="server" ClientIDMode="Static" />
                        [[idiomas_solo_pendientes]]
                    </label>
                </div>
                <div class="acciones-filtro">
                    <asp:Button ID="btnBuscar" runat="server" CssClass="btn btn-secundario" Text="[[categorias_buscar]]" OnClick="btnBuscar_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>

        <div class="tabla-scroll mt-24">
            <table class="tabla tabla-fija" id="tablaTraducciones">
                <caption class="solo-lectores">[[idiomas_tabla_traducciones]]</caption>
                <colgroup>
                    <col style="width: 26%" />
                    <col style="width: 31%" />
                    <col style="width: 31%" />
                    <col style="width: 12%" />
                </colgroup>
                <thead>
                    <tr>
                        <th scope="col">[[idiomas_col_clave]]</th>
                        <th scope="col">[[idiomas_col_base]]</th>
                        <th scope="col">[[idiomas_col_traduccion]]</th>
                        <th scope="col"><span class="solo-lectores">[[categorias_acciones]]</span></th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptTraducciones" runat="server" EnableViewState="false">
                        <ItemTemplate>
                            <tr>
                                <td class="mono celda-fija"><%#: Eval("Clave") %></td>
                                <td class="texto-suave celda-fija"><%#: Eval("TextoBase") %></td>
                                <td class="celda-fija"><%#: Eval("Texto") %><asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("Pendiente") %>'> <span class="badge badge-alerta">[[idiomas_pendiente]]</span></asp:PlaceHolder></td>
                                <td class="celda-acciones">
                                    <div class="acciones-fila">
                                        <button type="button" class="btn btn-secundario btn-chico" data-abre-modal="modalTraduccion"
                                                data-clave="<%#: Eval("Clave") %>" data-base="<%#: Eval("TextoBase") %>" data-texto="<%#: Eval("Texto") %>">[[idiomas_editar]]</button>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>

        <asp:PlaceHolder ID="phSinTraducciones" runat="server" Visible="false">
            <div class="vacio">
                <svg width="34" height="34" aria-hidden="true"><use href="#i-idioma" /></svg>
                <p class="sin-margen">[[idiomas_sin_coincidencias]]</p>
            </div>
        </asp:PlaceHolder>

        <div class="paginado">
            <span><asp:Literal ID="litPaginado" runat="server" /></span>
            <div class="fila">
                <asp:LinkButton ID="lnkAnterior" runat="server" CssClass="btn btn-fantasma btn-chico" OnClick="lnkAnterior_Click" CausesValidation="false">[[bitacora_anterior]]</asp:LinkButton>
                <asp:LinkButton ID="lnkSiguiente" runat="server" CssClass="btn btn-fantasma btn-chico" OnClick="lnkSiguiente_Click" CausesValidation="false">[[bitacora_siguiente]]</asp:LinkButton>
            </div>
        </div>
    </asp:PlaceHolder>

    <div class="modal-fondo" id="modalAgregar" role="dialog" aria-modal="true" aria-labelledby="agregarTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="agregarTitulo">[[idiomas_agregar_un_idioma]]</h2>
                    <p class="subtitulo sin-margen">[[idiomas_agregar_ayuda]]</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="campo">
                    <label for="ddlDisponibles">[[idiomas_col_idioma]]</label>
                    <asp:DropDownList ID="ddlDisponibles" runat="server" CssClass="entrada" ClientIDMode="Static" data-foco-inicial="si" />
                </div>
                <div class="campo">
                    <label for="txtCodigo">[[idiomas_codigo]]</label>
                    <asp:TextBox ID="txtCodigo" runat="server" CssClass="entrada" MaxLength="12" ClientIDMode="Static" placeholder="it" />
                </div>
                <div class="campo mb-0">
                    <label for="txtNombre">[[idiomas_nombre]]</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="entrada" MaxLength="60" ClientIDMode="Static" placeholder="Italiano" />
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>[[categorias_cancelar]]</button>
                <asp:Button ID="btnAgregar" runat="server" CssClass="btn btn-primario" Text="[[idiomas_agregar_idioma]]" OnClick="btnAgregar_Click" CausesValidation="false" ClientIDMode="Static"
                            OnClientClick="this.value = '[[js:idiomas_agregando]]'; return true;" />
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfEliminar" runat="server" ClientIDMode="Static" />

    <div class="modal-fondo" id="modalEliminar" role="dialog" aria-modal="true" aria-labelledby="eliminarTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="eliminarTitulo">[[idiomas_eliminar_el_idioma]]</h2>
                    <p class="subtitulo sin-margen">[[idiomas_eliminar_subtitulo]]</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="aviso aviso-peligro">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen">[[idiomas_vas_a_eliminar]] <strong id="elimNombre"></strong>.</p>
                </div>
                <ul class="consecuencias">
                    <li>[[idiomas_eliminar_consecuencia_1]]</li>
                    <li>[[idiomas_eliminar_consecuencia_2]]</li>
                </ul>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>[[categorias_cancelar]]</button>
                <asp:Button ID="btnEliminar" runat="server" CssClass="btn btn-peligro" Text="[[idiomas_eliminar_idioma]]" OnClick="btnEliminar_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfClave" runat="server" ClientIDMode="Static" />

    <div class="modal-fondo" id="modalTraduccion" role="dialog" aria-modal="true" aria-labelledby="traduccionTitulo" hidden>
        <div class="modal ancho">
            <div class="modal-cabecera">
                <div>
                    <h2 id="traduccionTitulo">[[idiomas_editar_traduccion]]</h2>
                    <p class="subtitulo sin-margen mono" id="trClave"></p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="campo">
                    <label>[[idiomas_texto_base]]</label>
                    <p class="texto-largo sin-margen" id="trBase"></p>
                </div>
                <div class="campo mb-0">
                    <label for="txtTexto">[[idiomas_col_traduccion]]</label>
                    <asp:TextBox ID="txtTexto" runat="server" TextMode="MultiLine" Rows="4" CssClass="entrada" ClientIDMode="Static" data-foco-inicial="si" />
                    <p class="ayuda texto-suave">[[idiomas_marcadores_ayuda]]</p>
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>[[categorias_cancelar]]</button>
                <asp:Button ID="btnGuardarTraduccion" runat="server" CssClass="btn btn-primario" Text="[[idiomas_guardar_traduccion]]" OnClick="btnGuardarTraduccion_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            function porId(id) { return document.getElementById(id); }

            var lista = porId("ddlDisponibles");

            if (lista) {
                lista.addEventListener("change", function () {
                    var opcion = lista.options[lista.selectedIndex];

                    porId("txtCodigo").value = opcion.value;
                    porId("txtNombre").value = opcion.getAttribute("data-nombre") || "";
                });
            }

            document.addEventListener("click", function (e) {
                var boton = e.target.closest ? e.target.closest("[data-abre-modal]") : null;
                if (!boton) return;

                if (boton.hasAttribute("data-clave")) {
                    porId("hfClave").value = boton.getAttribute("data-clave");
                    porId("trClave").textContent = boton.getAttribute("data-clave");
                    porId("trBase").textContent = boton.getAttribute("data-base");
                    porId("txtTexto").value = boton.getAttribute("data-texto");
                }

                if (boton.hasAttribute("data-idioma")) {
                    porId("hfEliminar").value = boton.getAttribute("data-idioma");
                    porId("elimNombre").textContent = boton.getAttribute("data-nombre");
                }
            });
        })();
    </script>
</asp:Content>
