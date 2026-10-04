<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Roles.aspx.cs" Inherits="GUI.Roles" Title="Roles y permisos" %>

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
            <h1>Roles y permisos</h1>
            <p class="texto-suave sin-margen">
                Un rol reúne grupos y permisos. Los grupos ordenan permisos y pueden contener otros grupos.
                Los permisos los define el sistema: acá solo se combinan.
            </p>
        </div>
        <div class="acciones">
            <button type="button" class="btn btn-secundario" data-abre-modal="modalNuevo" data-tipo-nuevo="grupo">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span>Nuevo grupo</span>
            </button>
            <button type="button" class="btn btn-primario" data-abre-modal="modalNuevo" data-tipo-nuevo="rol">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span>Nuevo rol</span>
            </button>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <div class="roles">

        <nav class="roles-lista" aria-label="Roles y grupos">
            <h2>Roles</h2>
            <asp:Repeater ID="rptRoles" runat="server">
                <ItemTemplate>
                    <a class="rol-item" href="<%#: Enlace((string)Eval("Nombre")) %>" aria-current="<%# EsActual((string)Eval("Nombre")) ? "true" : "false" %>">
                        <span class="nombre"><%#: Eval("Etiqueta") %></span>
                        <span class="meta"><%#: Meta(Container.DataItem) %></span>
                    </a>
                </ItemTemplate>
            </asp:Repeater>

            <h2>Grupos</h2>
            <asp:Repeater ID="rptGrupos" runat="server">
                <ItemTemplate>
                    <a class="rol-item" href="<%#: Enlace((string)Eval("Nombre")) %>" aria-current="<%# EsActual((string)Eval("Nombre")) ? "true" : "false" %>">
                        <span class="nombre"><%#: Eval("Etiqueta") %></span>
                        <span class="meta"><%#: Meta(Container.DataItem) %></span>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
            <asp:PlaceHolder ID="phSinGrupos" runat="server" Visible="false">
                <p class="texto-chico texto-suave" style="margin: 0 8px">Todavía no hay grupos.</p>
            </asp:PlaceHolder>

            <h2>Permisos</h2>
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
                        <div class="texto-chico texto-suave mt-8">Nombre interno: <code><asp:Literal ID="litNombreInterno" runat="server" /></code></div>
                    </div>
                    <div class="acciones">
                        <button type="button" class="btn btn-peligro btn-chico" id="btnAbrirEliminar" runat="server" data-abre-modal="modalEliminar">Eliminar</button>
                    </div>
                </div>

                <div class="rol-descripcion">
                    <label for="txtDescripcion">Nombre</label>
                    <div class="rol-descripcion-fila">
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="entrada" MaxLength="200" ClientIDMode="Static" />
                        <asp:Button ID="btnGuardarDescripcion" runat="server" CssClass="btn btn-secundario" Text="Guardar nombre" OnClick="btnGuardarDescripcion_Click" />
                    </div>
                </div>

                <asp:Panel ID="pnlFijo" runat="server" CssClass="aviso aviso-info" Visible="false">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-candado" /></svg>
                    <p class="sin-margen">El rol Gestor es fijo: no se edita su composición ni se elimina, para que Pattern Blue nunca quede sin acceso. Sí se le puede cambiar el nombre.</p>
                </asp:Panel>

                <asp:PlaceHolder ID="phComposicion" runat="server">
                    <div class="rol-bloques">
                        <div>
                            <h3>Qué incluye</h3>
                            <p class="texto-chico texto-suave">Marcá los grupos y permisos que forman parte de <strong><asp:Literal ID="litNombre2" runat="server" /></strong>.</p>

                            <asp:PlaceHolder ID="phGrupos" runat="server">
                                <p class="etiqueta">Grupos</p>
                                <div class="lista-marcas">
                                    <asp:CheckBoxList ID="cblGrupos" runat="server" RepeatLayout="Table" CellPadding="0" CellSpacing="0" />
                                </div>
                            </asp:PlaceHolder>

                            <p class="etiqueta mt-16">Permisos</p>
                            <div class="lista-marcas">
                                <asp:CheckBoxList ID="cblPatentes" runat="server" RepeatLayout="Table" CellPadding="0" CellSpacing="0" />
                            </div>
                        </div>

                        <div>
                            <h3>Permisos efectivos</h3>
                            <p class="texto-chico texto-suave">Todo lo que da este elemento, contando lo que hereda de sus grupos.</p>
                            <ul class="efectivos"><asp:Literal ID="litEfectivos" runat="server" /></ul>
                        </div>
                    </div>

                    <div class="rol-pie">
                        <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primario" Text="Guardar cambios" OnClick="btnGuardar_Click" />
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phIncluidoEn" runat="server">
                    <h3 class="mt-16">Incluido en</h3>
                    <ul class="efectivos"><asp:Literal ID="litIncluidoEn" runat="server" /></ul>
                </asp:PlaceHolder>

            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phVacio" runat="server" Visible="false">
                <div class="vacio">
                    <svg width="28" height="28" aria-hidden="true"><use href="#i-candado" /></svg>
                    <p class="sin-margen">Elegí un rol, un grupo o un permiso de la lista.</p>
                </div>
            </asp:PlaceHolder>
        </section>

    </div>

    <asp:HiddenField ID="hfTipoNuevo" runat="server" ClientIDMode="Static" Value="rol" />

    <div class="modal-fondo" id="modalNuevo" role="dialog" aria-modal="true" aria-labelledby="nuevoTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="nuevoTitulo">Nuevo rol</h2>
                    <p class="subtitulo sin-margen" id="nuevoAyuda">Un rol es lo que se le asigna a un usuario. Después le marcás qué incluye.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="campo mb-0">
                    <label for="txtNombreNuevo">Nombre</label>
                    <asp:TextBox ID="txtNombreNuevo" runat="server" CssClass="entrada" MaxLength="60" ClientIDMode="Static" data-foco-inicial="si" />
                    <p class="ayuda">Entre 3 y 60 caracteres: letras, números, espacios, guiones y puntos.</p>
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cancelar</button>
                <asp:Button ID="btnCrear" runat="server" CssClass="btn btn-primario" Text="Crear" OnClick="btnCrear_Click" />
            </div>
        </div>
    </div>

    <div class="modal-fondo" id="modalEliminar" role="dialog" aria-modal="true" aria-labelledby="eliminarTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="eliminarTitulo">Eliminar</h2>
                    <p class="subtitulo sin-margen">Se elimina solo si no está asignado a usuarios ni incluido en otro elemento.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="aviso aviso-alerta mb-0">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen">Esta acción no se puede deshacer y queda registrada en la bitácora.</p>
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cancelar</button>
                <asp:Button ID="btnEliminar" runat="server" CssClass="btn btn-peligro" Text="Eliminar" OnClick="btnEliminar_Click" />
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
                document.getElementById("nuevoTitulo").textContent = tipo === "rol" ? "Nuevo rol" : "Nuevo grupo";
                document.getElementById("nuevoAyuda").textContent = tipo === "rol"
                    ? "Un rol es lo que se le asigna a un usuario. Después le marcás qué incluye."
                    : "Un grupo ordena permisos y puede contener otros grupos. Después lo incluís en un rol.";
                document.getElementById("txtNombreNuevo").value = "";
            });
        })();
    </script>
</asp:Content>
