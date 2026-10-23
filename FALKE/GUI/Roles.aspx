<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Roles.aspx.cs" Inherits="GUI.Roles" Title="[[app_roles_y_permisos]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        .roles
        {
            display: grid;
            grid-template-columns: 300px minmax(0, 1fr);
            gap: 20px;
            align-items: start;
        }

        .roles-lista
        {
            position: sticky;
            top: 76px;
            max-height: calc(100vh - 100px);
            overflow-y: auto;
            background: var(--superficie);
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            padding: 14px 10px;
        }

        .roles-lista h2
        {
            font-size: .84rem;
            font-weight: 600;
            color: var(--texto-suave);
            margin: 6px 8px 8px;
        }

        .roles-lista h2:not(:first-child)
        {
            margin-top: 18px;
        }

        .rol-item
        {
            display: block;
            padding: 9px 10px;
            border-radius: var(--radio);
            color: var(--texto);
            border: 1px solid transparent;
        }

        .rol-item:hover
        {
            background: var(--superficie-2);
            text-decoration: none;
        }

        .rol-item[aria-current="true"]
        {
            background: var(--acento-suave);
            border-color: var(--acento);
        }

        .rol-item .nombre
        {
            display: block;
            font-weight: 600;
            font-size: .92rem;
        }

        .rol-item .meta
        {
            display: block;
            font-size: .78rem;
            color: var(--texto-suave);
        }

        .rol-detalle
        {
            background: var(--superficie);
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            padding: 20px 22px;
        }

        .rol-cabecera
        {
            display: flex;
            align-items: flex-start;
            gap: 14px;
            flex-wrap: wrap;
            margin-bottom: 14px;
        }

        .rol-cabecera h2
        {
            margin: 0 0 4px;
            font-size: 1.3rem;
        }

        .rol-cabecera .acciones
        {
            margin-left: auto;
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }

        .rol-descripcion
        {
            margin-bottom: 18px;
        }

        .rol-descripcion label
        {
            display: block;
            font-weight: 600;
            font-size: .88rem;
            margin-bottom: 6px;
        }

        .rol-descripcion-fila
        {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .rol-descripcion-fila .entrada
        {
            flex: 1 1 280px;
        }

        .rol-bloques
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
            gap: 22px;
            margin: 18px 0;
        }

        .rol-bloques h3
        {
            font-size: .98rem;
            margin-bottom: 8px;
        }

        .lista-marcas
        {
            max-height: 460px;
            overflow-y: auto;
            border: 1px solid var(--borde);
            border-radius: var(--radio);
            padding: 6px 4px;
        }

        .lista-marcas table
        {
            width: 100%;
            border-collapse: collapse;
        }

        .lista-marcas td
        {
            padding: 6px 10px;
        }

        .lista-marcas label
        {
            margin-left: 8px;
            font-size: .88rem;
            cursor: pointer;
        }

        .efectivos
        {
            margin: 0;
            padding-left: 18px;
            font-size: .88rem;
            color: var(--texto-suave);
        }

        .efectivos li
        {
            margin-bottom: 3px;
        }

        .rol-pie
        {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            padding-top: 14px;
            border-top: 1px solid var(--borde);
        }

        @media (max-width: 900px)
        {
            .roles { grid-template-columns: minmax(0, 1fr); }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1>[[app_roles_y_permisos]]</h1>
            <p class="texto-suave sin-margen">
                [[roles_un_rol_reune_grupos_y_permisos]]
            </p>
        </div>
        <div class="acciones">
            <button type="button" class="btn btn-secundario" data-abre-modal="modalNuevo" data-tipo-nuevo="grupo">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span>[[roles_nuevo_grupo]]</span>
            </button>
            <button type="button" class="btn btn-primario" data-abre-modal="modalNuevo" data-tipo-nuevo="rol">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span>[[roles_nuevo_rol]]</span>
            </button>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <div class="roles">

        <nav class="roles-lista" aria-label="[[roles_roles_y_grupos]]">
            <h2>[[roles_roles]]</h2>
            <asp:Repeater ID="rptRoles" runat="server">
                <ItemTemplate>
                    <a class="rol-item" href="<%#: Enlace((string)Eval("Nombre")) %>" aria-current="<%# EsActual((string)Eval("Nombre")) ? "true" : "false" %>">
                        <span class="nombre"><%#: Eval("Etiqueta") %></span>
                        <span class="meta"><%#: Meta(Container.DataItem) %></span>
                    </a>
                </ItemTemplate>
            </asp:Repeater>

            <h2>[[roles_grupos]]</h2>
            <asp:Repeater ID="rptGrupos" runat="server">
                <ItemTemplate>
                    <a class="rol-item" href="<%#: Enlace((string)Eval("Nombre")) %>" aria-current="<%# EsActual((string)Eval("Nombre")) ? "true" : "false" %>">
                        <span class="nombre"><%#: Eval("Etiqueta") %></span>
                        <span class="meta"><%#: Meta(Container.DataItem) %></span>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
            <asp:PlaceHolder ID="phSinGrupos" runat="server" Visible="false">
                <p class="texto-chico texto-suave" style="margin: 0 8px">[[roles_todavia_no_hay_grupos]]</p>
            </asp:PlaceHolder>

            <h2>[[roles_permisos]]</h2>
            <asp:Repeater ID="rptPatentes" runat="server">
                <ItemTemplate>
                    <a class="rol-item" href="<%#: Enlace((string)Eval("Nombre")) %>" aria-current="<%# EsActual((string)Eval("Nombre")) ? "true" : "false" %>">
                        <span class="nombre"><%#: Eval("Etiqueta") %></span>
                        <span class="meta"><%#: Meta(Container.DataItem) %></span>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </nav>

        <section class="rol-detalle">
            <asp:PlaceHolder ID="phDetalle" runat="server">

                <div class="rol-cabecera">
                    <div>
                        <h2><asp:Literal ID="litNombre" runat="server" /></h2>
                        <asp:PlaceHolder ID="phEtiquetas" runat="server">
                            <span class="badge" id="badgeClase" runat="server"><asp:Literal ID="litClase" runat="server" /></span>
                            <span class="texto-chico texto-suave"><asp:Literal ID="litMeta" runat="server" /></span>
                        </asp:PlaceHolder>
                        <div class="texto-chico texto-suave mt-8">[[roles_nombre_interno]] <code><asp:Literal ID="litNombreInterno" runat="server" /></code></div>
                    </div>
                    <div class="acciones">
                        <button type="button" class="btn btn-peligro btn-chico" id="btnAbrirEliminar" runat="server" data-abre-modal="modalEliminar">[[roles_eliminar]]</button>
                    </div>
                </div>

                <div class="rol-descripcion">
                    <label for="txtDescripcion">[[dispositivos_nombre]]</label>
                    <div class="rol-descripcion-fila">
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="entrada" MaxLength="200" ClientIDMode="Static" />
                        <asp:Button ID="btnGuardarDescripcion" runat="server" CssClass="btn btn-secundario" Text="[[comun_guardar_nombre]]" OnClick="btnGuardarDescripcion_Click" />
                    </div>
                </div>

                <asp:Panel ID="pnlFijo" runat="server" CssClass="aviso aviso-info" Visible="false">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-candado" /></svg>
                    <p class="sin-margen">[[roles_los_roles_gestor_y_webmaster_son]]</p>
                </asp:Panel>

                <asp:PlaceHolder ID="phComposicion" runat="server">
                    <div class="rol-bloques">
                        <div>
                            <h3>[[roles_que_incluye]]</h3>
                            <p class="texto-chico texto-suave">[[roles_marca_los_grupos_y_permisos_que]] <strong><asp:Literal ID="litNombre2" runat="server" /></strong>.</p>

                            <asp:PlaceHolder ID="phGrupos" runat="server">
                                <p class="etiqueta">[[roles_grupos]]</p>
                                <div class="lista-marcas">
                                    <asp:CheckBoxList ID="cblGrupos" runat="server" RepeatLayout="Table" CellPadding="0" CellSpacing="0" />
                                </div>
                            </asp:PlaceHolder>

                            <p class="etiqueta mt-16">[[roles_permisos]]</p>
                            <div class="lista-marcas">
                                <asp:CheckBoxList ID="cblPatentes" runat="server" RepeatLayout="Table" CellPadding="0" CellSpacing="0" />
                            </div>
                        </div>

                        <div>
                            <h3>[[roles_permisos_efectivos]]</h3>
                            <p class="texto-chico texto-suave">[[roles_todo_lo_que_da_este_elemento]]</p>
                            <ul class="efectivos"><asp:Literal ID="litEfectivos" runat="server" /></ul>
                        </div>
                    </div>

                    <div class="rol-pie">
                        <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primario" Text="[[comun_guardar_cambios]]" OnClick="btnGuardar_Click" />
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phIncluidoEn" runat="server">
                    <h3 class="mt-16">[[roles_incluido_en]]</h3>
                    <ul class="efectivos"><asp:Literal ID="litIncluidoEn" runat="server" /></ul>
                </asp:PlaceHolder>

            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phVacio" runat="server" Visible="false">
                <div class="vacio">
                    <svg width="28" height="28" aria-hidden="true"><use href="#i-candado" /></svg>
                    <p class="sin-margen">[[roles_elegi_un_rol_un_grupo_o]]</p>
                </div>
            </asp:PlaceHolder>
        </section>

    </div>

    <asp:HiddenField ID="hfTipoNuevo" runat="server" ClientIDMode="Static" Value="rol" />

    <div class="modal-fondo" id="modalNuevo" role="dialog" aria-modal="true" aria-labelledby="nuevoTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="nuevoTitulo">[[roles_nuevo_rol]]</h2>
                    <p class="subtitulo sin-margen" id="nuevoAyuda">[[roles_un_rol_es_lo_que_se]]</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="campo mb-0">
                    <label for="txtNombreNuevo">[[dispositivos_nombre]]</label>
                    <asp:TextBox ID="txtNombreNuevo" runat="server" CssClass="entrada" MaxLength="60" ClientIDMode="Static" data-foco-inicial="si" />
                    <p class="ayuda">[[roles_entre_3_y_60_caracteres_letras]]</p>
                </div>
                <div class="campo mt-16 mb-0" id="campoAmbito">
                    <label for="selAmbitoNuevo">[[roles_tipo_de_rol]]</label>
                    <select id="selAmbitoNuevo" name="ambitoNuevo" class="entrada">
                        <option value="general">[[roles_general_para_usuarios_de_cualquier_empre]]</option>
                        <option value="gestion">[[roles_de_gestion_solo_para_usuarios_de]]</option>
                    </select>
                    <p class="ayuda">[[roles_solo_un_rol_de_gestion_puede]]</p>
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>[[categorias_cancelar]]</button>
                <asp:Button ID="btnCrear" runat="server" CssClass="btn btn-primario" Text="[[roles_crear]]" OnClick="btnCrear_Click" />
            </div>
        </div>
    </div>

    <div class="modal-fondo" id="modalEliminar" role="dialog" aria-modal="true" aria-labelledby="eliminarTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="eliminarTitulo">[[roles_eliminar]]</h2>
                    <p class="subtitulo sin-margen">[[roles_se_elimina_solo_si_no_esta]]</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="aviso aviso-alerta mb-0">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen">[[roles_esta_accion_no_se_puede_deshacer]]</p>
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>[[categorias_cancelar]]</button>
                <asp:Button ID="btnEliminar" runat="server" CssClass="btn btn-peligro" Text="[[roles_eliminar]]" OnClick="btnEliminar_Click" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            document.addEventListener("click", function (e) {
                var boton = e.target.closest ? e.target.closest("[data-tipo-nuevo]") : null;
                if (!boton) return;

                var tipo = boton.getAttribute("data-tipo-nuevo");
                document.getElementById("hfTipoNuevo").value = tipo;
                document.getElementById("nuevoTitulo").textContent = tipo === "rol" ? "[[js:roles_nuevo_rol]]" : "[[js:roles_nuevo_grupo]]";
                document.getElementById("nuevoAyuda").textContent = tipo === "rol"
                    ? "[[js:roles_un_rol_es_lo_que_se]]"
                    : "[[js:roles_un_grupo_ordena_permisos_y_puede]]";
                document.getElementById("txtNombreNuevo").value = "";
                document.getElementById("selAmbitoNuevo").value = "general";
                document.getElementById("campoAmbito").hidden = tipo !== "rol";
            });
        })();
    </script>
</asp:Content>
