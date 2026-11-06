<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Categorias.aspx.cs" Inherits="GUI.Categorias" Title="[[app_categorias]]" %>

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
            <h1 data-i18n="categorias.titulo">[[app_categorias]]</h1>
            <p class="texto-suave sin-margen" data-i18n="categorias.bajada">
                [[categorias_cada_categoria_identifica_un_activo_digi]]
            </p>
        </div>
        <div class="acciones" id="phNuevaCategoria" runat="server">
            <button type="button" class="btn btn-primario" data-abre-modal="modalCategoria" data-nueva-categoria>
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span data-i18n="categorias.nueva">[[categorias_nueva_categoria]]</span>
            </button>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso aviso-exito" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <div class="filtros">
        <div class="campo">
            <label for="catBuscar" data-i18n="categorias.buscar">[[categorias_buscar]]</label>
            <input type="search" class="entrada" id="catBuscar"
                   placeholder="[[categorias_nombre_de_la_categoria]]" data-i18n-attr="placeholder:categorias.buscar.placeholder" />
        </div>
        <div class="campo">
            <label for="catTipo" data-i18n="categorias.tipo">[[categorias_tipo_de_activo]]</label>
            <select class="entrada" id="catTipo">
                <option value="" data-i18n="comun.todos">[[categorias_todos]]</option>
                <option value="Software">[[analisismultiple_software]]</option>
                <option value="AppWeb">[[analisismultiple_app_web]]</option>
                <option value="AppMovil">[[analisismultiple_app_movil]]</option>
                <option value="Videojuego">[[analisismultiple_videojuego]]</option>
                <option value="Publicidad">[[categorias_publicidad]]</option>
            </select>
        </div>
        <div class="campo">
            <label class="casilla-simple">
                <input type="checkbox" id="catMostrarInactivas" />
                <span data-i18n="categorias.mostrarInactivas">[[categorias_mostrar_desactivadas]]</span>
            </label>
        </div>
    </div>

    <div class="tabla-scroll">
        <table class="tabla" id="tablaCategorias">
            <caption class="solo-lectores">[[categorias_categorias_registradas_por_tu_empresa]]</caption>
            <thead>
                <tr>
                    <th scope="col" data-i18n="categorias.col.nombre">[[categorias_categoria]]</th>
                    <th scope="col" data-i18n="categorias.col.tipo">[[categorias_tipo_de_activo]]</th>
                    <th scope="col" data-i18n="categorias.col.activo">[[categorias_activo_evaluado]]</th>
                    <th scope="col" data-i18n="categorias.col.sesiones">[[categorias_sesiones]]</th>
                    <th scope="col" data-i18n="categorias.col.creada">[[categorias_creada]]</th>
                    <th scope="col"><span class="solo-lectores" data-i18n="comun.acciones">[[categorias_acciones]]</span></th>
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
                                    <a class="btn btn-fantasma btn-chico" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>" data-i18n="comun.verSesiones">[[categorias_ver_sesiones]]</a>
                                    <asp:PlaceHolder runat="server" Visible="<%# PuedeGestionar %>">
                                        <button type="button" class="icono-boton" aria-label="[[categorias_editar_la_categoria]]" data-abre-modal="modalCategoria" data-editar-categoria>
                                            <svg width="17" height="17" aria-hidden="true"><use href="#i-lapiz" /></svg>
                                        </button>
                                        <asp:LinkButton ID="btnEliminarCategoria" runat="server" CssClass="icono-boton" ToolTip="[[categorias_eliminar_la_categoria]]"
                                            CommandName="Eliminar" CommandArgument='<%# Eval("IdCategoria") %>'
                                            OnClientClick="return confirm('[[js:categorias_eliminar_esta_categoria_si_tiene_sesione]]');">
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
        <p class="sin-margen" data-i18n="categorias.vacio">[[categorias_no_hay_categorias_que_coincidan_con]]</p>
    </div>

    <asp:HiddenField ID="hfCatId" runat="server" ClientIDMode="Static" Value="0" />

    <div class="modal-fondo" id="modalCategoria" role="dialog" aria-modal="true" aria-labelledby="catTitulo" hidden>
        <div class="modal ancho">

            <div class="modal-cabecera">
                <div>
                    <h2 id="catTitulo" data-i18n="categorias.modal.titulo">[[categorias_registrar_nueva_categoria]]</h2>
                    <p class="subtitulo sin-margen" data-i18n="categorias.modal.subtitulo">
                        [[categorias_los_campos_disponibles_cambian_segun_el]]
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <div class="campo">
                    <label for="mcNombre" data-i18n="categorias.modal.nombre">[[categorias_nombre_de_la_categoria_2]]</label>
                    <asp:TextBox ID="mcNombre" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static"
                        placeholder="Checkout flow v2" />
                </div>

                <div class="campo">
                    <label for="mcTipo" data-i18n="categorias.modal.tipo">[[categorias_tipo_de_activo_digital]]</label>
                    <asp:DropDownList ID="mcTipo" runat="server" CssClass="entrada" ClientIDMode="Static">
                        <asp:ListItem Value="Software" Text="[[analisismultiple_software]]" />
                        <asp:ListItem Value="AppWeb" Text="[[categorias_aplicacion_web]]" />
                        <asp:ListItem Value="AppMovil" Text="[[categorias_aplicacion_movil]]" />
                        <asp:ListItem Value="Videojuego" Text="[[analisismultiple_videojuego]]" />
                        <asp:ListItem Value="Publicidad" Text="[[categorias_publicidad_digital]]" />
                    </asp:DropDownList>
                </div>

                <div class="campo">
                    <label for="mcActivo" data-i18n="categorias.modal.activo">[[categorias_nombre_del_activo_evaluado]]</label>
                    <asp:TextBox ID="mcActivo" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static"
                        placeholder="Panel de administración interno" />
                </div>

                <div class="campo">
                    <label for="mcFlujo" data-i18n="categorias.modal.flujo">[[categorias_flujo_o_proceso_a_analizar]]</label>
                    <asp:TextBox ID="mcFlujo" runat="server" CssClass="entrada" TextMode="MultiLine" Rows="2" MaxLength="500" ClientIDMode="Static"
                        placeholder="[[categorias_que_parte_del_activo_se_va]]" />
                </div>

                <div class="campos-tipo" data-tipo="Software">
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">[[categorias_campos_especificos_del_tipo_seleccionado]]</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcSo" data-i18n="categorias.campo.so">[[categorias_sistema_operativo]]</label>
                            <asp:TextBox ID="mcSo" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="Windows 11" />
                        </div>
                        <div class="campo">
                            <label for="mcVersionSw" data-i18n="categorias.campo.version">[[categorias_version_del_software]]</label>
                            <asp:TextBox ID="mcVersionSw" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="1.4.2" />
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="AppWeb" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">[[categorias_campos_especificos_del_tipo_seleccionado]]</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcUrl" data-i18n="categorias.campo.url">[[categorias_url_opcional]]</label>
                            <asp:TextBox ID="mcUrl" runat="server" CssClass="entrada" MaxLength="300" ClientIDMode="Static" placeholder="app.empresa.com/panel" />
                        </div>
                        <div class="campo">
                            <label for="mcDispositivo" data-i18n="categorias.campo.dispositivo">[[categorias_dispositivo_objetivo]]</label>
                            <asp:DropDownList ID="mcDispositivo" runat="server" CssClass="entrada" ClientIDMode="Static">
                                <asp:ListItem Value="Escritorio" Text="[[categorias_escritorio]]" />
                                <asp:ListItem Value="Tablet" Text="Tablet" />
                                <asp:ListItem Value="Movil" Text="[[categorias_movil]]" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="AppMovil" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">[[categorias_campos_especificos_del_tipo_seleccionado]]</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcSoMovil" data-i18n="categorias.campo.soMovil">[[categorias_sistema_operativo_objetivo]]</label>
                            <asp:DropDownList ID="mcSoMovil" runat="server" CssClass="entrada" ClientIDMode="Static">
                                <asp:ListItem Value="Android" Text="Android" />
                                <asp:ListItem Value="Ios" Text="iOS" />
                            </asp:DropDownList>
                        </div>
                        <div class="campo">
                            <label for="mcVersionApp" data-i18n="categorias.campo.versionApp">[[categorias_version_de_la_aplicacion]]</label>
                            <asp:TextBox ID="mcVersionApp" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="3.4.0" />
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="Videojuego" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">[[categorias_campos_especificos_del_tipo_seleccionado]]</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcPlataforma" data-i18n="categorias.campo.plataforma">[[categorias_plataforma]]</label>
                            <asp:DropDownList ID="mcPlataforma" runat="server" CssClass="entrada" ClientIDMode="Static">
                                <asp:ListItem Value="Pc" Text="PC" />
                                <asp:ListItem Value="Consola" Text="[[categorias_consola]]" />
                                <asp:ListItem Value="Movil" Text="[[categorias_movil]]" />
                            </asp:DropDownList>
                        </div>
                        <div class="campo">
                            <label for="mcVersionJuego" data-i18n="categorias.campo.version">[[categorias_version_del_juego]]</label>
                            <asp:TextBox ID="mcVersionJuego" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="0.9.3 beta" />
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="Publicidad" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">[[categorias_campos_especificos_del_tipo_seleccionado]]</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcFormato" data-i18n="categorias.campo.formato">[[categorias_formato]]</label>
                            <asp:TextBox ID="mcFormato" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="Banner 970x250" />
                        </div>
                        <div class="campo">
                            <label for="mcCanal" data-i18n="categorias.campo.canal">[[categorias_canal_de_distribucion]]</label>
                            <asp:TextBox ID="mcCanal" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" placeholder="Display / redes" />
                        </div>
                    </div>
                </div>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">[[categorias_cancelar]]</button>
                <asp:Button ID="btnGuardarCategoria" runat="server" CssClass="btn btn-primario" Text="[[categorias_registrar_categoria]]" OnClick="btnGuardarCategoria_Click" ClientIDMode="Static" />
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
                document.getElementById("catTitulo").textContent = "[[js:categorias_registrar_nueva_categoria]]";
                document.getElementById("btnGuardarCategoria").value = "[[js:categorias_registrar_categoria]]";
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
                    document.getElementById("catTitulo").textContent = "[[js:categorias_editar_categoria]]";
                    document.getElementById("btnGuardarCategoria").value = "[[js:comun_guardar_cambios]]";
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
