<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Categorias.aspx.cs" Inherits="GUI.Categorias" Title="Categorías" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .categoria-tipo
        {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-size: .78rem;
            font-weight: 600;
            color: var(--texto-suave);
        }

        tr[data-activa="0"]
        {
            opacity: .55;
        }

        .campos-tipo[hidden]
        {
            display: none;
        }

        .campos-tipo
        {
            border: 1px dashed var(--borde-fuerte);
            border-radius: var(--radio);
            padding: 14px 14px 0;
            background: var(--superficie-2);
            margin-top: 4px;
        }

        .campos-tipo > .etiqueta-grupo
        {
            font-size: .85rem;
            font-weight: 600;
            color: var(--texto-suave);
            margin-bottom: 10px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="categorias.titulo">Categorías</h1>
            <p class="texto-suave sin-margen" data-i18n="categorias.bajada">
                Cada categoría identifica un activo digital de tu empresa y agrupa todas las
                sesiones que se graben sobre el.
            </p>
        </div>
        <div class="acciones" id="phNuevaCategoria" runat="server">
            <button type="button" class="btn btn-primario" data-abre-modal="modalCategoria" data-nueva-categoria>
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span data-i18n="categorias.nueva">Nueva categoría</span>
            </button>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso aviso-exito" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <div class="filtros">
        <div class="campo">
            <label for="catBuscar" data-i18n="categorias.buscar">Buscar</label>
            <input type="search" class="entrada" id="catBuscar"
                   placeholder="Nombre de la categoría..." data-i18n-attr="placeholder:categorias.buscar.placeholder" />
        </div>
        <div class="campo">
            <label for="catTipo" data-i18n="categorias.tipo">Tipo de activo</label>
            <select class="entrada" id="catTipo">
                <option value="" data-i18n="comun.todos">Todos</option>
                <option value="Software">Software</option>
                <option value="AppWeb">App web</option>
                <option value="AppMovil">App móvil</option>
                <option value="Videojuego">Videojuego</option>
                <option value="Publicidad">Publicidad</option>
            </select>
        </div>
        <div class="campo">
            <label class="casilla-simple">
                <input type="checkbox" id="catMostrarInactivas" />
                <span data-i18n="categorias.mostrarInactivas">Mostrar desactivadas</span>
            </label>
        </div>
    </div>

    <div class="tabla-scroll">
        <table class="tabla" id="tablaCategorias">
            <caption class="solo-lectores">Categorías registradas por tu empresa</caption>
            <thead>
                <tr>
                    <th scope="col" data-i18n="categorias.col.nombre">Categoría</th>
                    <th scope="col" data-i18n="categorias.col.tipo">Tipo de activo</th>
                    <th scope="col" data-i18n="categorias.col.activo">Activo evaluado</th>
                    <th scope="col" data-i18n="categorias.col.sesiones">Sesiones</th>
                    <th scope="col" data-i18n="categorias.col.creada">Creada</th>
                    <th scope="col"><span class="solo-lectores" data-i18n="comun.acciones">Acciones</span></th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptCategorias" runat="server" OnItemCommand="rptCategorias_ItemCommand">
                    <ItemTemplate>
                        <tr data-tipo="<%#: Eval("Tipo") %>" data-activa="<%#: (bool)Eval("Activa") ? "1" : "0" %>"
                            data-id="<%#: Eval("IdCategoria") %>" data-nombre="<%#: Eval("NombreCategoria") %>"
                            data-e-nombre="<%#: Eval("NombreCategoria") %>" data-e-tipo="<%#: Eval("Tipo") %>"
                            data-e-activo="<%#: Eval("NombreActivo") %>" data-e-flujo="<%#: Eval("FlujoEsperado") %>"
                            <%# AtributosEspecificos((BE.Categoria_BE)Container.DataItem) %>>
                            <td><strong><%#: Eval("NombreCategoria") %></strong></td>
                            <td><span class="categoria-tipo"><svg width="15" height="15" aria-hidden="true"><use href="#i-carpeta" /></svg><%#: EtiquetaTipo((BE.TipoActivoCategoria)Eval("Tipo")) %></span></td>
                            <td class="texto-suave"><%#: Dato((string)Eval("NombreActivo")) %></td>
                            <td><%#: Eval("CantidadSesiones") %></td>
                            <td class="texto-suave"><%#: Fecha((DateTime)Eval("FechaCreacion")) %></td>
                            <td>
                                <div class="acciones-fila">
                                    <a class="btn btn-fantasma btn-chico" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>" data-i18n="comun.verSesiones">Ver sesiones</a>
                                    <asp:PlaceHolder runat="server" Visible="<%# PuedeGestionar %>">
                                        <button type="button" class="icono-boton" aria-label="Editar la categoría" data-editar-categoria>
                                            <svg width="17" height="17" aria-hidden="true"><use href="#i-lapiz" /></svg>
                                        </button>
                                        <asp:LinkButton ID="btnEliminarCategoria" runat="server" CssClass="icono-boton" ToolTip="Eliminar la categoría"
                                            CommandName="Eliminar" CommandArgument='<%# Eval("IdCategoria") %>'
                                            OnClientClick="return confirm('¿Eliminar esta categoría? Si tiene sesiones grabadas, se va a desactivar en lugar de borrarse.');">
                                            <svg width="17" height="17" aria-hidden="true"><use href="#i-tacho" /></svg>
                                        </asp:LinkButton>
                                    </asp:PlaceHolder>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>

    <div class="vacio" id="categoriasVacio" hidden>
        <svg width="34" height="34" aria-hidden="true"><use href="#i-carpeta" /></svg>
        <p class="sin-margen" data-i18n="categorias.vacio">No hay categorías que coincidan con el filtro.</p>
    </div>

    <asp:HiddenField ID="hfCatId" runat="server" ClientIDMode="Static" Value="0" />

    <div class="modal-fondo" id="modalCategoria" role="dialog" aria-modal="true" aria-labelledby="catTitulo" hidden>
        <div class="modal ancho">

            <div class="modal-cabecera">
                <div>
                    <h2 id="catTitulo" data-i18n="categorias.modal.titulo">Registrar nueva categoría</h2>
                    <p class="subtitulo sin-margen" data-i18n="categorias.modal.subtitulo">
                        Los campos disponibles cambian según el tipo de activo digital seleccionado.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <div class="campo">
                    <label for="mcNombre" data-i18n="categorias.modal.nombre">Nombre de la categoría</label>
                    <asp:TextBox ID="mcNombre" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static"
                        placeholder="Checkout flow v2" />
                </div>

                <div class="campo">
                    <label for="mcTipo" data-i18n="categorias.modal.tipo">Tipo de activo digital</label>
                    <asp:DropDownList ID="mcTipo" runat="server" CssClass="entrada" ClientIDMode="Static">
                        <asp:ListItem Value="Software" Text="Software" />
                        <asp:ListItem Value="AppWeb" Text="Aplicación web" />
                        <asp:ListItem Value="AppMovil" Text="Aplicación móvil" />
                        <asp:ListItem Value="Videojuego" Text="Videojuego" />
                        <asp:ListItem Value="Publicidad" Text="Publicidad digital" />
                    </asp:DropDownList>
                </div>

                <div class="campo">
                    <label for="mcActivo" data-i18n="categorias.modal.activo">Nombre del activo evaluado</label>
                    <asp:TextBox ID="mcActivo" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static"
                        placeholder="Panel de administración interno" />
                </div>

                <div class="campo">
                    <label for="mcFlujo" data-i18n="categorias.modal.flujo">Flujo o proceso a analizar</label>
                    <asp:TextBox ID="mcFlujo" runat="server" CssClass="entrada" TextMode="MultiLine" Rows="2" MaxLength="500" ClientIDMode="Static"
                        placeholder="Que parte del activo se va a evaluar" />
                </div>

                <div class="campos-tipo" data-tipo="Software">
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">Campos específicos del tipo seleccionado</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcSo" data-i18n="categorias.campo.so">Sistema operativo</label>
                            <asp:TextBox ID="mcSo" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="Windows 11" />
                        </div>
                        <div class="campo">
                            <label for="mcVersionSw" data-i18n="categorias.campo.version">Versión del software</label>
                            <asp:TextBox ID="mcVersionSw" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="1.4.2" />
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="AppWeb" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">Campos específicos del tipo seleccionado</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcUrl" data-i18n="categorias.campo.url">URL (opcional)</label>
                            <asp:TextBox ID="mcUrl" runat="server" CssClass="entrada" MaxLength="300" ClientIDMode="Static" placeholder="app.empresa.com/panel" />
                        </div>
                        <div class="campo">
                            <label for="mcDispositivo" data-i18n="categorias.campo.dispositivo">Dispositivo objetivo</label>
                            <asp:DropDownList ID="mcDispositivo" runat="server" CssClass="entrada" ClientIDMode="Static">
                                <asp:ListItem Value="Escritorio" Text="Escritorio" />
                                <asp:ListItem Value="Tablet" Text="Tablet" />
                                <asp:ListItem Value="Movil" Text="Móvil" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="AppMovil" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">Campos específicos del tipo seleccionado</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcSoMovil" data-i18n="categorias.campo.soMovil">Sistema operativo objetivo</label>
                            <asp:DropDownList ID="mcSoMovil" runat="server" CssClass="entrada" ClientIDMode="Static">
                                <asp:ListItem Value="Android" Text="Android" />
                                <asp:ListItem Value="Ios" Text="iOS" />
                            </asp:DropDownList>
                        </div>
                        <div class="campo">
                            <label for="mcVersionApp" data-i18n="categorias.campo.versionApp">Versión de la aplicación</label>
                            <asp:TextBox ID="mcVersionApp" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="3.4.0" />
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="Videojuego" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">Campos específicos del tipo seleccionado</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcPlataforma" data-i18n="categorias.campo.plataforma">Plataforma</label>
                            <asp:DropDownList ID="mcPlataforma" runat="server" CssClass="entrada" ClientIDMode="Static">
                                <asp:ListItem Value="Pc" Text="PC" />
                                <asp:ListItem Value="Consola" Text="Consola" />
                                <asp:ListItem Value="Movil" Text="Móvil" />
                            </asp:DropDownList>
                        </div>
                        <div class="campo">
                            <label for="mcVersionJuego" data-i18n="categorias.campo.version">Versión del juego</label>
                            <asp:TextBox ID="mcVersionJuego" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="0.9.3 beta" />
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="Publicidad" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">Campos específicos del tipo seleccionado</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcFormato" data-i18n="categorias.campo.formato">Formato</label>
                            <asp:TextBox ID="mcFormato" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="Banner 970x250" />
                        </div>
                        <div class="campo">
                            <label for="mcCanal" data-i18n="categorias.campo.canal">Canal de distribución</label>
                            <asp:TextBox ID="mcCanal" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="Display / redes" />
                        </div>
                    </div>
                </div>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <asp:Button ID="btnGuardarCategoria" runat="server" CssClass="btn btn-primario" Text="Registrar categoría" OnClick="btnGuardarCategoria_Click" ClientIDMode="Static" />
            </div>

        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var buscar = document.getElementById("catBuscar");
            var tipo = document.getElementById("catTipo");
            var mostrarInactivas = document.getElementById("catMostrarInactivas");
            var filas = document.querySelectorAll("#tablaCategorias tbody tr");
            var vacio = document.getElementById("categoriasVacio");

            function normalizar(texto) {
                return window.Falke && window.Falke.normalizar ? window.Falke.normalizar(texto) : String(texto || "").toLowerCase().trim();
            }

            function filtrar() {
                var texto = normalizar(buscar.value);
                var tipoElegido = tipo.value;
                var visibles = 0;

                for (var i = 0; i < filas.length; i++) {
                    var f = filas[i];
                    var okTexto = texto === "" || normalizar(f.textContent).indexOf(texto) !== -1;
                    var okTipo = tipoElegido === "" || f.getAttribute("data-tipo") === tipoElegido;
                    var okActiva = mostrarInactivas.checked || f.getAttribute("data-activa") === "1";
                    var mostrar = okTexto && okTipo && okActiva;

                    f.hidden = !mostrar;

                    if (mostrar) visibles++;
                }

                vacio.hidden = visibles > 0;
            }

            buscar.addEventListener("input", filtrar);
            tipo.addEventListener("change", filtrar);
            mostrarInactivas.addEventListener("change", filtrar);
            filtrar();

            var selectorTipo = document.getElementById("mcTipo");
            var bloques = document.querySelectorAll(".campos-tipo");

            function mostrarCamposDelTipo() {
                for (var i = 0; i < bloques.length; i++) {
                    bloques[i].hidden = bloques[i].getAttribute("data-tipo") !== selectorTipo.value;
                }
            }

            selectorTipo.addEventListener("change", mostrarCamposDelTipo);

            function limpiarModal() {
                document.getElementById("hfCatId").value = "0";
                document.getElementById("catTitulo").textContent = "Registrar nueva categoría";
                document.getElementById("btnGuardarCategoria").value = "Registrar categoría";
                document.getElementById("mcNombre").value = "";
                selectorTipo.value = "Software";
                document.getElementById("mcActivo").value = "";
                document.getElementById("mcFlujo").value = "";
                document.getElementById("mcSo").value = "";
                document.getElementById("mcVersionSw").value = "";
                document.getElementById("mcUrl").value = "";
                document.getElementById("mcDispositivo").value = "Escritorio";
                document.getElementById("mcSoMovil").value = "Android";
                document.getElementById("mcVersionApp").value = "";
                document.getElementById("mcPlataforma").value = "Pc";
                document.getElementById("mcVersionJuego").value = "";
                document.getElementById("mcFormato").value = "";
                document.getElementById("mcCanal").value = "";
                mostrarCamposDelTipo();
            }

            document.addEventListener("click", function (e) {
                var nueva = e.target.closest ? e.target.closest("[data-nueva-categoria]") : null;
                var editar = e.target.closest ? e.target.closest("[data-editar-categoria]") : null;

                if (nueva) {
                    limpiarModal();
                }

                if (editar) {
                    var fila = editar.closest("tr");
                    if (!fila) return;
                    var d = fila.dataset;

                    document.getElementById("hfCatId").value = d.id;
                    document.getElementById("catTitulo").textContent = "Editar categoría";
                    document.getElementById("btnGuardarCategoria").value = "Guardar cambios";
                    document.getElementById("mcNombre").value = d.eNombre || "";
                    selectorTipo.value = d.eTipo || "Software";
                    document.getElementById("mcActivo").value = d.eActivo || "";
                    document.getElementById("mcFlujo").value = d.eFlujo || "";
                    document.getElementById("mcSo").value = d.eSo || "";
                    document.getElementById("mcVersionSw").value = d.eVersionsw || "";
                    document.getElementById("mcUrl").value = d.eUrl || "";
                    document.getElementById("mcDispositivo").value = d.eDispositivo || "Escritorio";
                    document.getElementById("mcSoMovil").value = d.eSomovil || "Android";
                    document.getElementById("mcVersionApp").value = d.eVersionapp || "";
                    document.getElementById("mcPlataforma").value = d.ePlataforma || "Pc";
                    document.getElementById("mcVersionJuego").value = d.eVersionjuego || "";
                    document.getElementById("mcFormato").value = d.eFormato || "";
                    document.getElementById("mcCanal").value = d.eCanal || "";
                    mostrarCamposDelTipo();
                }
            });
        })();
    </script>
</asp:Content>
