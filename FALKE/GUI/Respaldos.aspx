<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Respaldos.aspx.cs" Inherits="GUI.Respaldos" Title="Respaldos" %>

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
            <h1>Respaldos</h1>
            <p class="texto-suave sin-margen">
                Copias de seguridad de la base de datos. Quedan guardadas en el servidor: no se descargan desde acá.
            </p>
        </div>
        <div class="acciones">
            <asp:PlaceHolder ID="phBotonGenerar" runat="server">
                <asp:LinkButton ID="btnGenerar" runat="server" CssClass="btn btn-primario" OnClick="btnGenerar_Click" CausesValidation="false"
                                OnClientClick="this.classList.add('cargando'); this.querySelector('span').textContent = 'Generando la copia...'; return true;">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-base" /></svg>
                    <span>Generar respaldo</span>
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
            <div class="metrica-nombre">Respaldos guardados</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litUltimo" runat="server" /></div>
            <div class="metrica-nombre">Último respaldo</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litEspacio" runat="server" /></div>
            <div class="metrica-nombre">Espacio ocupado</div>
        </div>
    </div>

    <asp:PlaceHolder ID="phAvisoRestaurar" runat="server">
        <div class="aviso aviso-alerta">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen">
                Restaurar un respaldo reemplaza toda la base por la del momento de la copia: lo que se cargó después se pierde.
                Al confirmar se cierran todas las sesiones, incluida la tuya.
            </p>
        </div>
    </asp:PlaceHolder>

    <div class="tabla-scroll">
        <table class="tabla" id="tablaRespaldos">
            <caption class="solo-lectores">Respaldos de la base de datos</caption>
            <thead>
                <tr>
                    <th scope="col">Fecha</th>
                    <th scope="col">Archivo</th>
                    <th scope="col">Tamaño</th>
                    <th scope="col">Generado por</th>
                    <th scope="col">Estado</th>
                    <th scope="col"><span class="solo-lectores">Acciones</span></th>
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
                                <span class="badge <%#: ((TE.Respaldo_TE)Container.DataItem).ArchivoDisponible ? "badge-exito" : "badge-peligro" %>"><%#: ((TE.Respaldo_TE)Container.DataItem).ArchivoDisponible ? "Disponible" : "Archivo no encontrado" %></span>
                            </td>
                            <td>
                                <div class="acciones-fila">
                                    <asp:PlaceHolder runat="server" Visible='<%# MostrarRestaurar(Container.DataItem) %>'>
                                        <button type="button" class="btn btn-peligro btn-chico" data-abre-modal="modalRestaurar"
                                                data-respaldo="<%#: Eval("IdRespaldo") %>" data-fecha="<%#: Fecha(((TE.Respaldo_TE)Container.DataItem).FechaGeneracion) %>" data-archivo="<%#: Eval("NombreArchivo") %>">Restaurar</button>
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
            <p class="sin-margen">Todavía no se generó ningún respaldo.</p>
        </div>
    </asp:PlaceHolder>

    <asp:HiddenField ID="hfRespaldo" runat="server" ClientIDMode="Static" />

    <asp:PlaceHolder ID="phModalRestaurar" runat="server">
    <div class="modal-fondo" id="modalRestaurar" role="dialog" aria-modal="true" aria-labelledby="restTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="restTitulo">Confirmar la restauración</h2>
                    <p class="subtitulo sin-margen">Esta acción no se puede deshacer.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="aviso aviso-peligro">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen">
                        Vas a restaurar la copia del <strong id="restFecha"></strong> (<span class="mono" id="restArchivo"></span>).
                    </p>
                </div>

                <p>Al confirmar:</p>
                <ul class="consecuencias">
                    <li>Se reemplaza toda la base actual por la de la copia elegida.</li>
                    <li>Los usuarios conectados quedan sin servicio unos segundos y se cierran todas las sesiones, incluida la tuya.</li>
                    <li>Lo que se cargó después de la copia se pierde.</li>
                    <li>Los dígitos verificadores vuelven al estado de la copia: revisalos en «Dígito verificador».</li>
                </ul>

                <div class="campo mb-0">
                    <label for="txtConfirmacion">Para confirmar, escribí RESTAURAR</label>
                    <asp:TextBox ID="txtConfirmacion" runat="server" CssClass="entrada" autocomplete="off" ClientIDMode="Static" data-foco-inicial="si" />
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cancelar</button>
                <asp:Button ID="btnRestaurar" runat="server" CssClass="btn btn-peligro" Text="Restaurar y cerrar sesiones" OnClick="btnRestaurar_Click" CausesValidation="false" ClientIDMode="Static"
                            OnClientClick="this.value = 'Restaurando...'; return true;" />
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
