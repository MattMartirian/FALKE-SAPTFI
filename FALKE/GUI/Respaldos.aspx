<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Respaldos.aspx.cs" Inherits="GUI.Respaldos" Title="[[respaldos_respaldos]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .fila-sin-archivo > td
        {
            opacity: .55;
        }

        .fila-sin-archivo > td:last-child
        {
            opacity: 1;
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
            <h1>[[respaldos_respaldos]]</h1>
            <p class="texto-suave sin-margen">
                [[respaldos_copias_de_seguridad_de_la_base]]
            </p>
        </div>
        <div class="acciones">
            <asp:PlaceHolder ID="phBotonGenerar" runat="server">
                <asp:LinkButton ID="btnGenerar" runat="server" CssClass="btn btn-primario" OnClick="btnGenerar_Click" CausesValidation="false"
                                OnClientClick="this.classList.add('cargando'); this.querySelector('span').textContent = '[[js:respaldos_generando_la_copia]]'; return true;">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-base" /></svg>
                    <span>[[respaldos_generar_respaldo]]</span>
                </asp:LinkButton>
            </asp:PlaceHolder>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <div class="metricas mb-24">
        <div>
            <div class="metrica-valor"><asp:Literal ID="litTotal" runat="server" /></div>
            <div class="metrica-nombre">[[respaldos_respaldos_guardados]]</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litUltimo" runat="server" /></div>
            <div class="metrica-nombre">[[respaldos_ultimo_respaldo]]</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litEspacio" runat="server" /></div>
            <div class="metrica-nombre">[[respaldos_espacio_ocupado]]</div>
        </div>
    </div>

    <asp:PlaceHolder ID="phAvisoRestaurar" runat="server">
        <div class="aviso aviso-alerta">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen">
                [[respaldos_restaurar_un_respaldo_reemplaza_toda_la]]
            </p>
        </div>
    </asp:PlaceHolder>

    <div class="tabla-scroll">
        <table class="tabla" id="tablaRespaldos">
            <caption class="solo-lectores">[[respaldos_respaldos_de_la_base_de_datos]]</caption>
            <thead>
                <tr>
                    <th scope="col">[[panel_fecha]]</th>
                    <th scope="col">[[respaldos_archivo]]</th>
                    <th scope="col">[[respaldos_tamano]]</th>
                    <th scope="col">[[respaldos_generado_por]]</th>
                    <th scope="col">[[dispositivos_estado]]</th>
                    <th scope="col"><span class="solo-lectores">[[categorias_acciones]]</span></th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptRespaldos" runat="server" EnableViewState="false">
                    <ItemTemplate>
                        <tr class="<%#: ((TE.Respaldo_TE)Container.DataItem).ArchivoDisponible ? "" : "fila-sin-archivo" %>">
                            <td><%#: Fecha(((TE.Respaldo_TE)Container.DataItem).FechaGeneracion) %></td>
                            <td class="mono"><%#: Eval("NombreArchivo") %></td>
                            <td class="texto-suave"><%#: Tamano((long?)Eval("Tamano")) %></td>
                            <td class="texto-suave"><%#: Eval("NombreUsuario") %></td>
                            <td>
                                <span class="badge <%#: ((TE.Respaldo_TE)Container.DataItem).ArchivoDisponible ? "badge-exito" : "badge-peligro" %>"><%#: ((TE.Respaldo_TE)Container.DataItem).ArchivoDisponible ? Traducir("respaldos_disponible") : Traducir("respaldos_archivo_no_encontrado") %></span>
                            </td>
                            <td>
                                <div class="acciones-fila">
                                    <asp:PlaceHolder runat="server" Visible='<%# MostrarRestaurar(Container.DataItem) %>'>
                                        <button type="button" class="btn btn-peligro btn-chico" data-abre-modal="modalRestaurar"
                                                data-respaldo="<%#: Eval("IdRespaldo") %>" data-fecha="<%#: Fecha(((TE.Respaldo_TE)Container.DataItem).FechaGeneracion) %>" data-archivo="<%#: Eval("NombreArchivo") %>">[[respaldos_restaurar]]</button>
                                    </asp:PlaceHolder>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>

    <asp:PlaceHolder ID="phVacio" runat="server" Visible="false">
        <div class="vacio">
            <svg width="34" height="34" aria-hidden="true"><use href="#i-base" /></svg>
            <p class="sin-margen">[[respaldos_todavia_no_se_genero_ningun_respaldo]]</p>
        </div>
    </asp:PlaceHolder>

    <asp:HiddenField ID="hfRespaldo" runat="server" ClientIDMode="Static" />

    <asp:PlaceHolder ID="phModalRestaurar" runat="server">
    <div class="modal-fondo" id="modalRestaurar" role="dialog" aria-modal="true" aria-labelledby="restTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="restTitulo">[[respaldos_confirmar_la_restauracion]]</h2>
                    <p class="subtitulo sin-margen">[[nuevagrabacion_esta_accion_no_se_puede_deshacer]]</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="aviso aviso-peligro">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen">
                        [[respaldos_vas_a_restaurar_la_copia_del]] <strong id="restFecha"></strong> (<span class="mono" id="restArchivo"></span>).
                    </p>
                </div>

                <p>[[respaldos_al_confirmar]]</p>
                <ul class="consecuencias">
                    <li>[[respaldos_se_reemplaza_toda_la_base_actual]]</li>
                    <li>[[respaldos_los_usuarios_conectados_quedan_sin_servi]]</li>
                    <li>[[respaldos_lo_que_se_cargo_despues_de]]</li>
                    <li>[[respaldos_los_digitos_verificadores_vuelven_al_est]]</li>
                </ul>

                <div class="campo mb-0">
                    <label for="txtConfirmacion">[[respaldos_para_confirmar_escribi_restaurar]]</label>
                    <asp:TextBox ID="txtConfirmacion" runat="server" CssClass="entrada" autocomplete="off" ClientIDMode="Static" data-foco-inicial="si" />
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>[[categorias_cancelar]]</button>
                <asp:Button ID="btnRestaurar" runat="server" CssClass="btn btn-peligro" Text="[[respaldos_restaurar_y_cerrar_sesiones]]" OnClick="btnRestaurar_Click" CausesValidation="false" ClientIDMode="Static"
                            OnClientClick="this.value = '[[js:respaldos_restaurando]]'; return true;" />
            </div>
        </div>
    </div>
    </asp:PlaceHolder>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            function porId(id) { return document.getElementById(id); }

            // El botón de cada fila dice qué copia se va a restaurar.
            document.addEventListener("click", function (e) {
                var boton = e.target.closest ? e.target.closest("[data-respaldo]") : null;
                if (!boton) return;

                porId("hfRespaldo").value = boton.getAttribute("data-respaldo");
                porId("restFecha").textContent = boton.getAttribute("data-fecha");
                porId("restArchivo").textContent = boton.getAttribute("data-archivo");
                porId("txtConfirmacion").value = "";
                revisar();
            });

            // El botón de restaurar se habilita cuando se escribió la palabra (el servidor lo vuelve a comprobar).
            function revisar() {
                var caja = porId("txtConfirmacion"), boton = porId("btnRestaurar");
                if (caja && boton) boton.disabled = caja.value.trim().toUpperCase() !== "RESTAURAR";
            }

            var caja = porId("txtConfirmacion");
            if (caja) {
                caja.addEventListener("input", revisar);
                caja.addEventListener("keydown", function (e) { if (e.key === "Enter") e.preventDefault(); });
                revisar();
            }
        })();
    </script>
</asp:Content>
