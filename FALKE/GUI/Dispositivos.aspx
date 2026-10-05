<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Dispositivos.aspx.cs" Inherits="GUI.Dispositivos" Title="Dispositivos" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .punto-estado
        {
            width: 9px;
            height: 9px;
            border-radius: var(--radio-full);
            display: inline-block;
            margin-right: 7px;
            background: var(--texto-tenue);
        }

        .punto-estado.enUso { background: var(--exito); }
        .punto-estado.disponible { background: var(--info); }
        .punto-estado.mantenimiento { background: var(--alerta); }
        .punto-estado.baja { background: var(--peligro); }

        .fila-baja-dispositivo > td
        {
            opacity: .55;
        }

        /* Las columnas tienen ancho fijo: abrir un detalle no cambia la disposición de la tabla. */
        #tablaDispositivos
        {
            table-layout: fixed;
            min-width: 820px;
        }

        #tablaDispositivos th:nth-child(1) { width: 17%; }
        #tablaDispositivos th:nth-child(2) { width: 21%; }
        #tablaDispositivos th:nth-child(3) { width: 17%; }
        #tablaDispositivos th:nth-child(4) { width: 14%; }
        #tablaDispositivos th:nth-child(5) { width: 14%; }
        #tablaDispositivos th:nth-child(6) { width: 170px; }

        #tablaDispositivos td
        {
            overflow-wrap: anywhere;
        }

        /* El detalle se despliega debajo de su dispositivo con la misma animación de las preguntas frecuentes. */
        .tabla tbody tr.fila-detalle:hover
        {
            background: transparent;
        }

        .tabla tbody tr.fila-detalle > td
        {
            padding: 0;
        }

        .acordeon-panel.ancho > *
        {
            max-width: none;
            padding: 0 20px;
        }

        .acordeon-panel.abierto .detalle-interno
        {
            border-bottom: 1px solid var(--borde);
        }

        .detalle-interno
        {
            background: var(--superficie-2);
            color: var(--texto);
            font-size: .88rem;
        }

        .detalle-cuerpo
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 28px;
        }

        .datos-dispositivo
        {
            width: 100%;
            border-collapse: collapse;
        }

        .datos-dispositivo th,
        .datos-dispositivo td
        {
            padding: 8px 0;
            border-bottom: 1px solid var(--borde);
            text-align: left;
            vertical-align: top;
        }

        .datos-dispositivo th
        {
            width: 42%;
            font-weight: 600;
            color: var(--texto-suave);
            padding-right: 16px;
        }

        .datos-dispositivo tr:last-child th,
        .datos-dispositivo tr:last-child td
        {
            border-bottom: 0;
        }

        /* Si hay muchos préstamos, la lista se desplaza dentro del detalle en lugar de estirarlo. */
        .historial-scroll
        {
            max-height: 190px;
            overflow-y: auto;
            border: 1px solid var(--borde);
            border-radius: var(--radio-md);
            background: var(--superficie);
        }

        .historial-scroll .tabla tbody tr:last-child td
        {
            border-bottom: 0;
        }

        .acciones-detalle
        {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 18px;
            padding-top: 16px;
            border-top: 1px solid var(--borde);
        }

        /* Botón "Ver detalle": cambia de texto y gira la flecha según esté abierto. */
        .btn-detalle .signo
        {
            transition: transform .28s cubic-bezier(.22, 1, .36, 1);
        }

        .btn-detalle[aria-expanded="true"] .signo
        {
            transform: rotate(180deg);
        }

        .btn-detalle .t-ocultar,
        .btn-detalle[aria-expanded="true"] .t-ver
        {
            display: none;
        }

        .btn-detalle[aria-expanded="true"] .t-ocultar
        {
            display: inline;
        }

        .agregar-modelo
        {
            display: flex;
            gap: 10px;
            align-items: flex-start;
        }

        .agregar-modelo .entrada
        {
            flex: 1;
        }

        .tabla-modelos .entrada
        {
            min-width: 200px;
        }

        .tabla-modelos td
        {
            white-space: nowrap;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="dispositivos.titulo">Dispositivos</h1>
            <p class="texto-suave sin-margen" data-i18n="dispositivos.bajada">
                Inventario de eye trackers y a qué empresa está prestado cada uno.
            </p>
        </div>
        <div class="acciones">
            <asp:PlaceHolder ID="phBotonModelos" runat="server">
                <button type="button" class="btn btn-secundario" data-abre-modal="modalModelos">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-engranaje" /></svg>
                    <span>Modelos</span>
                </button>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phBotonAsignar" runat="server">
                <button type="button" class="btn btn-secundario" data-abre-modal="modalAsignar">
                    <svg width="17" height="17" aria-hidden="true"><use href="#i-empresa" /></svg>
                    <span data-i18n="dispositivos.asignar">Asignar a empresa</span>
                </button>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phBotonAlta" runat="server">
                <button type="button" class="btn btn-primario" data-abre-modal="modalAltaDispositivo">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                    <span data-i18n="dispositivos.nuevo">Alta de dispositivo</span>
                </button>
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
            <div class="metrica-nombre" data-i18n="dispositivos.metrica.total">En inventario</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litPrestados" runat="server" /></div>
            <div class="metrica-nombre" data-i18n="dispositivos.metrica.prestados">Prestados</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litDisponibles" runat="server" /></div>
            <div class="metrica-nombre" data-i18n="dispositivos.metrica.disponibles">Disponibles</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litTaller" runat="server" /></div>
            <div class="metrica-nombre" data-i18n="dispositivos.metrica.taller">En mantenimiento</div>
        </div>
    </div>

    <asp:Panel ID="pnlFiltros" runat="server" DefaultButton="btnBuscar">
        <div class="filtros">
            <div class="campo">
                <label for="txtBuscar" data-i18n="dispositivos.buscar">Buscar</label>
                <asp:TextBox ID="txtBuscar" runat="server" CssClass="entrada" MaxLength="100" placeholder="Serie, modelo o empresa..." ClientIDMode="Static" />
            </div>
            <div class="campo">
                <label for="ddlFiltroEstado" data-i18n="dispositivos.estado">Estado</label>
                <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="entrada" ClientIDMode="Static" />
            </div>
            <div class="campo">
                <label for="ddlFiltroModelo" data-i18n="dispositivos.modelo">Modelo</label>
                <asp:DropDownList ID="ddlFiltroModelo" runat="server" CssClass="entrada" ClientIDMode="Static" />
            </div>
            <div class="acciones-filtro">
                <asp:Button ID="btnBuscar" runat="server" CssClass="btn btn-secundario" Text="Buscar" OnClick="btnBuscar_Click" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>

    <asp:HiddenField ID="hfDispositivo" runat="server" ClientIDMode="Static" />

    <div class="tabla-scroll">
        <table class="tabla" id="tablaDispositivos">
            <caption class="solo-lectores">Inventario de dispositivos</caption>
            <thead>
                <tr>
                    <th scope="col" data-i18n="dispositivos.col.serie">Número de serie</th>
                    <th scope="col" data-i18n="dispositivos.col.modelo">Modelo</th>
                    <th scope="col" data-i18n="dispositivos.col.empresa">Empresa</th>
                    <th scope="col" data-i18n="dispositivos.col.entrega">Entregado</th>
                    <th scope="col" data-i18n="dispositivos.col.estado">Estado</th>
                    <th scope="col"><span class="solo-lectores" data-i18n="comun.acciones">Acciones</span></th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptDispositivos" runat="server" EnableViewState="false">
                    <ItemTemplate>
                        <tr class="<%#: ClaseFila(Container.DataItem) %>">
                            <td class="mono"><%#: Eval("NumeroSerie") %></td>
                            <td><%#: Eval("Modelo") %></td>
                            <td><%#: Dato((string)Eval("EmpresaActual")) %></td>
                            <td class="texto-suave"><%#: Fecha((DateTime?)Eval("FechaEntregaActual")) %></td>
                            <td><span class="punto-estado <%#: ClasePunto((BE.EstadoDispositivo)Eval("Estado")) %>" aria-hidden="true"></span><%#: Etiqueta((BE.EstadoDispositivo)Eval("Estado")) %></td>
                            <td>
                                <div class="acciones-fila">
                                    <button type="button" class="btn btn-secundario btn-chico btn-detalle" data-acordeon
                                            aria-controls="det-<%#: Eval("IdDispositivo") %>" aria-expanded="<%#: EstaAbierto(Container.DataItem) ? "true" : "false" %>">
                                        <span class="t-ver">Ver detalle</span><span class="t-ocultar">Ocultar detalle</span>
                                        <svg class="signo" width="15" height="15" aria-hidden="true"><use href="#i-flecha-abajo" /></svg>
                                    </button>
                                </div>
                            </td>
                        </tr>
                        <tr class="fila-detalle">
                            <td colspan="6">
                                <div class="acordeon-panel ancho<%#: EstaAbierto(Container.DataItem) ? " abierto" : "" %>" id="det-<%#: Eval("IdDispositivo") %>">
                                    <div class="detalle-interno">
                                        <div class="detalle-cuerpo">
                                            <div>
                                                <table class="datos-dispositivo">
                                                    <tbody>
                                                        <tr><th scope="row">Prestado a</th><td><%#: Dato((string)Eval("EmpresaActual")) %></td></tr>
                                                        <tr><th scope="row">Fecha de entrega</th><td><%#: Fecha((DateTime?)Eval("FechaEntregaActual")) %></td></tr>
                                                        <tr><th scope="row">Driver requerido</th><td class="mono"><%#: Dato((string)Eval("DriverRequerido")) %></td></tr>
                                                        <tr><th scope="row">Firmware</th><td class="mono"><%#: Dato((string)Eval("Firmware")) %></td></tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                            <div>
                                                <p class="etiqueta" data-i18n="dispositivos.det.historial">Historial de préstamos</p>
                                                <asp:Repeater runat="server" DataSource='<%# HistorialDe(Container.DataItem) %>' Visible='<%# HistorialDe(Container.DataItem).Count > 0 %>'>
                                                    <HeaderTemplate>
                                                        <div class="historial-scroll">
                                                            <table class="tabla">
                                                                <caption class="solo-lectores">Historial de préstamos del dispositivo</caption>
                                                                <thead>
                                                                    <tr>
                                                                        <th scope="col">Empresa</th>
                                                                        <th scope="col">Desde</th>
                                                                        <th scope="col">Hasta</th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td><%#: Eval("NombreEmpresa") %></td>
                                                            <td class="texto-suave"><%#: Fecha((DateTime)Eval("FechaEntrega")) %></td>
                                                            <td class="texto-suave"><%#: Hasta(Container.DataItem) %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                                </tbody>
                                                            </table>
                                                        </div>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                                <asp:PlaceHolder runat="server" Visible='<%# HistorialDe(Container.DataItem).Count == 0 %>'>
                                                    <p class="texto-chico texto-suave sin-margen">Todavía no se prestó a ninguna empresa.</p>
                                                </asp:PlaceHolder>
                                            </div>
                                        </div>

                                        <asp:PlaceHolder runat="server" Visible='<%# HayAcciones(Container.DataItem) %>'>
                                            <div class="acciones-detalle">
                                                <asp:PlaceHolder runat="server" Visible='<%# MostrarAsignar(Container.DataItem) %>'>
                                                    <button type="button" class="btn btn-primario btn-chico" data-abre-modal="modalAsignar" data-dispositivo="<%#: Eval("IdDispositivo") %>" data-asignar-actual>Asignar a empresa</button>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# MostrarDevolver(Container.DataItem) %>'>
                                                    <button type="button" class="btn btn-secundario btn-chico" data-abre-modal="modalDevolver" data-dispositivo="<%#: Eval("IdDispositivo") %>">Registrar devolución</button>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# MostrarMantenimiento(Container.DataItem) %>'>
                                                    <button type="button" class="btn btn-secundario btn-chico" data-dispositivo="<%#: Eval("IdDispositivo") %>"
                                                            data-envia="btnMantenimiento" data-confirma="¿Enviar el dispositivo a mantenimiento? Deja de estar disponible para préstamos.">Enviar a mantenimiento</button>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# MostrarTerminar(Container.DataItem) %>'>
                                                    <button type="button" class="btn btn-secundario btn-chico" data-dispositivo="<%#: Eval("IdDispositivo") %>"
                                                            data-envia="btnTerminar" data-confirma="¿Terminar el mantenimiento? El dispositivo vuelve a estar disponible para préstamos.">Terminar mantenimiento</button>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# MostrarEditar(Container.DataItem) %>'>
                                                    <button type="button" class="btn btn-fantasma btn-chico" data-abre-modal="modalEditar" data-dispositivo="<%#: Eval("IdDispositivo") %>"
                                                            data-modelo="<%#: Eval("IdModelo") %>" data-firmware="<%#: Eval("Firmware") %>" data-driver="<%#: Eval("DriverRequerido") %>" data-rellena-edicion>Editar datos</button>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# MostrarBaja(Container.DataItem) %>'>
                                                    <button type="button" class="btn btn-peligro btn-chico" data-abre-modal="modalBaja" data-dispositivo="<%#: Eval("IdDispositivo") %>">Dar de baja</button>
                                                </asp:PlaceHolder>
                                            </div>
                                        </asp:PlaceHolder>
                                    </div>
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
            <svg width="34" height="34" aria-hidden="true"><use href="#i-dispositivo" /></svg>
            <p class="sin-margen" data-i18n="dispositivos.vacio">No hay dispositivos que coincidan con el filtro.</p>
        </div>
    </asp:PlaceHolder>

    <%-- Botones de las acciones que piden confirmación: los dispara el botón del detalle, después de confirmar. --%>
    <asp:PlaceHolder ID="phAccionesOcultas" runat="server">
        <asp:Button ID="btnMantenimiento" runat="server" style="display:none" Text="Enviar a mantenimiento" OnClick="btnMantenimiento_Click" CausesValidation="false" ClientIDMode="Static" />
        <asp:Button ID="btnTerminar" runat="server" style="display:none" Text="Terminar mantenimiento" OnClick="btnTerminar_Click" CausesValidation="false" ClientIDMode="Static" />
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phModalModelos" runat="server">
    <div class="modal-fondo" id="modalModelos" role="dialog" aria-modal="true" aria-labelledby="modelosTitulo" hidden>
        <div class="modal ancho">
            <div class="modal-cabecera">
                <div>
                    <h2 id="modelosTitulo">Modelos de dispositivo</h2>
                    <p class="subtitulo sin-margen">
                        Un modelo no se borra: al darlo de baja deja de ofrecerse para dispositivos nuevos, y los que ya lo tienen lo conservan.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <asp:Panel ID="pnlAvisoModelos" runat="server" CssClass="aviso" Visible="false" role="status">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
                    <p class="sin-margen"><asp:Literal ID="litAvisoModelos" runat="server" /></p>
                </asp:Panel>

                <div class="campo">
                    <label for="txtNuevoModelo">Agregar un modelo</label>
                    <div class="agregar-modelo">
                        <asp:TextBox ID="txtNuevoModelo" runat="server" CssClass="entrada" MaxLength="50" placeholder="Tobii Eye Tracker 5" ClientIDMode="Static" />
                        <asp:Button ID="btnAgregarModelo" runat="server" CssClass="btn btn-primario" Text="Agregar" OnClick="btnAgregarModelo_Click" CausesValidation="false" ClientIDMode="Static" />
                    </div>
                </div>

                <div class="tabla-scroll">
                    <table class="tabla tabla-modelos">
                        <caption class="solo-lectores">Modelos de dispositivo</caption>
                        <thead>
                            <tr>
                                <th scope="col">Nombre</th>
                                <th scope="col">Dispositivos</th>
                                <th scope="col">Estado</th>
                                <th scope="col"><span class="solo-lectores">Acciones</span></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptModelos" runat="server" OnItemCommand="rptModelos_ItemCommand">
                                <ItemTemplate>
                                    <tr class="<%#: (bool)Eval("Activo") ? "" : "fila-baja-dispositivo" %>">
                                        <td><asp:TextBox ID="txtNombreModelo" runat="server" CssClass="entrada" MaxLength="50" Text='<%# Eval("Nombre") %>' /></td>
                                        <td class="texto-suave"><%#: Eval("CantidadDispositivos") %></td>
                                        <td><span class="badge <%#: (bool)Eval("Activo") ? "badge-exito" : "badge-neutro" %>"><%#: (bool)Eval("Activo") ? "Activo" : "De baja" %></span></td>
                                        <td>
                                            <div class="acciones-fila">
                                                <asp:Button runat="server" CssClass="btn btn-secundario btn-chico" Text="Guardar nombre" CommandName="renombrar" CommandArgument='<%# Eval("IdModelo") %>' CausesValidation="false" data-guardar="si" />
                                                <asp:Button runat="server" CssClass='<%# (bool)Eval("Activo") ? "btn btn-peligro btn-chico" : "btn btn-secundario btn-chico" %>'
                                                            Text='<%# (bool)Eval("Activo") ? "Dar de baja" : "Reactivar" %>' CommandName='<%# (bool)Eval("Activo") ? "baja" : "reactivar" %>'
                                                            CommandArgument='<%# Eval("IdModelo") %>' CausesValidation="false" />
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <asp:PlaceHolder ID="phSinModelos" runat="server" Visible="false">
                    <p class="texto-chico texto-suave mt-8 sin-margen">Todavía no hay modelos: agregá el primero para poder dar de alta dispositivos.</p>
                </asp:PlaceHolder>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cerrar</button>
            </div>
        </div>
    </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phModalAsignar" runat="server">
    <div class="modal-fondo" id="modalAsignar" role="dialog" aria-modal="true" aria-labelledby="asigTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="asigTitulo" data-i18n="asignar.titulo">Asignar dispositivo</h2>
                    <p class="subtitulo sin-margen" data-i18n="asignar.subtitulo">
                        El préstamo queda asociado a la empresa hasta que se registre la devolución.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="campo">
                    <label for="ddlAsigDispositivo" data-i18n="asignar.dispositivo">Dispositivo disponible</label>
                    <asp:DropDownList ID="ddlAsigDispositivo" runat="server" CssClass="entrada" ClientIDMode="Static" data-foco-inicial="si" />
                </div>
                <div class="campo">
                    <label for="ddlAsigEmpresa" data-i18n="asignar.empresa">Empresa</label>
                    <asp:DropDownList ID="ddlAsigEmpresa" runat="server" CssClass="entrada" ClientIDMode="Static" />
                    <p class="ayuda" data-i18n="asignar.empresa.ayuda">
                        Solo aparecen las empresas activas, con los dispositivos que ya tienen y el tope de su plan.
                    </p>
                </div>
                <div class="aviso aviso-info mb-0">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
                    <p class="sin-margen">La fecha de entrega es la de hoy. El préstamo queda en la bitácora de la empresa.</p>
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <asp:Button ID="btnAsignar" runat="server" CssClass="btn btn-primario" Text="Registrar préstamo" OnClick="btnAsignar_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phModalAlta" runat="server">
    <div class="modal-fondo" id="modalAltaDispositivo" role="dialog" aria-modal="true" aria-labelledby="altaDisTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="altaDisTitulo" data-i18n="altaDispositivo.titulo">Alta de dispositivo</h2>
                    <p class="subtitulo sin-margen" data-i18n="altaDispositivo.subtitulo">
                        Se suma al inventario en estado disponible.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="campo">
                    <label for="txtAltaSerie" data-i18n="altaDispositivo.serie">Número de serie</label>
                    <asp:TextBox ID="txtAltaSerie" runat="server" CssClass="entrada mono" MaxLength="30" ClientIDMode="Static" data-foco-inicial="si" />
                    <p class="ayuda">El de fábrica: figura en la etiqueta del equipo y lo informa el SDK de Tobii. No se puede repetir ni cambiar después.</p>
                </div>
                <div class="fila-campos">
                    <div class="campo">
                        <label for="ddlAltaModelo" data-i18n="altaDispositivo.modelo">Modelo</label>
                        <asp:DropDownList ID="ddlAltaModelo" runat="server" CssClass="entrada" ClientIDMode="Static" />
                    </div>
                    <div class="campo">
                        <label for="txtAltaFirmware" data-i18n="altaDispositivo.firmware">Firmware <span class="texto-tenue">(opcional)</span></label>
                        <asp:TextBox ID="txtAltaFirmware" runat="server" CssClass="entrada" MaxLength="20" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="campo mb-0">
                    <label for="txtAltaDriver">Driver requerido <span class="texto-tenue">(opcional)</span></label>
                    <asp:TextBox ID="txtAltaDriver" runat="server" CssClass="entrada" MaxLength="20" ClientIDMode="Static" />
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <asp:Button ID="btnCrear" runat="server" CssClass="btn btn-primario" Text="Dar de alta" OnClick="btnCrear_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phModalEditar" runat="server">
    <div class="modal-fondo" id="modalEditar" role="dialog" aria-modal="true" aria-labelledby="editarDisTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="editarDisTitulo">Editar datos del dispositivo</h2>
                    <p class="subtitulo sin-margen">El número de serie y el estado no se editan desde acá.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="campo">
                    <label for="ddlEdModelo">Modelo</label>
                    <asp:DropDownList ID="ddlEdModelo" runat="server" CssClass="entrada" ClientIDMode="Static" />
                </div>
                <div class="fila-campos">
                    <div class="campo mb-0">
                        <label for="txtEdFirmware">Firmware</label>
                        <asp:TextBox ID="txtEdFirmware" runat="server" CssClass="entrada" MaxLength="20" ClientIDMode="Static" />
                    </div>
                    <div class="campo mb-0">
                        <label for="txtEdDriver">Driver requerido</label>
                        <asp:TextBox ID="txtEdDriver" runat="server" CssClass="entrada" MaxLength="20" ClientIDMode="Static" />
                    </div>
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cancelar</button>
                <asp:Button ID="btnGuardarEdicion" runat="server" CssClass="btn btn-primario" Text="Guardar cambios" OnClick="btnGuardarEdicion_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phModalDevolver" runat="server">
    <div class="modal-fondo" id="modalDevolver" role="dialog" aria-modal="true" aria-labelledby="devolverTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="devolverTitulo">Registrar devolución</h2>
                    <p class="subtitulo sin-margen">Cierra el préstamo con la fecha de hoy.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <label class="casilla-simple">
                    <asp:CheckBox ID="chkRequiereMantenimiento" runat="server" ClientIDMode="Static" />
                    <span>Vuelve con problemas: enviarlo a mantenimiento</span>
                </label>
                <p class="ayuda">Si no lo marcás, queda disponible para prestarlo de nuevo.</p>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cancelar</button>
                <asp:Button ID="btnDevolver" runat="server" CssClass="btn btn-primario" Text="Registrar devolución" OnClick="btnDevolver_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phModalBaja" runat="server">
    <div class="modal-fondo" id="modalBaja" role="dialog" aria-modal="true" aria-labelledby="bajaTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="bajaTitulo">Dar de baja el dispositivo</h2>
                    <p class="subtitulo sin-margen">La baja es definitiva y queda en la bitácora.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="aviso aviso-alerta">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen">Un dispositivo dado de baja no se puede prestar ni modificar. Se conserva su historial.</p>
                </div>
                <div class="campo mb-0">
                    <label for="txtBajaMotivo">Motivo <span class="texto-tenue">(obligatorio)</span></label>
                    <asp:TextBox ID="txtBajaMotivo" runat="server" CssClass="entrada" MaxLength="300" ClientIDMode="Static" />
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cancelar</button>
                <asp:Button ID="btnBaja" runat="server" CssClass="btn btn-peligro" Text="Dar de baja" OnClick="btnBaja_Click" CausesValidation="false" />
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

            // Cada botón del detalle avisa sobre qué dispositivo actúa: el servidor lo lee del campo oculto.
            document.addEventListener("click", function (e) {
                var boton = e.target.closest ? e.target.closest("[data-dispositivo]") : null;
                if (!boton) return;

                var id = boton.getAttribute("data-dispositivo");
                var oculto = porId("hfDispositivo");
                if (oculto) oculto.value = id;

                // "Asignar a empresa" desde el detalle: el dispositivo ya viene elegido.
                if (boton.hasAttribute("data-asignar-actual")) {
                    var lista = porId("ddlAsigDispositivo");
                    if (lista) lista.value = id;
                }

                // "Editar datos": el formulario se llena con los datos de ese dispositivo.
                if (boton.hasAttribute("data-rellena-edicion")) {
                    var modelo = porId("ddlEdModelo"), firmware = porId("txtEdFirmware"), driver = porId("txtEdDriver");
                    if (modelo) modelo.value = boton.getAttribute("data-modelo");
                    if (firmware) firmware.value = boton.getAttribute("data-firmware") || "";
                    if (driver) driver.value = boton.getAttribute("data-driver") || "";
                }

                // Acciones que se confirman y se envían con un botón oculto del servidor.
                var destino = boton.getAttribute("data-envia");
                if (destino) {
                    var texto = boton.getAttribute("data-confirma");
                    var envio = porId(destino);
                    if (envio && (!texto || window.confirm(texto))) envio.click();
                }
            });

            // En la ventana de modelos, Enter agrega el modelo nuevo o guarda el nombre que se está editando.
            document.addEventListener("keydown", function (e) {
                if (e.key !== "Enter" || !e.target || e.target.tagName !== "INPUT" || e.target.type !== "text") return;
                if (!e.target.closest || !e.target.closest("#modalModelos")) return;

                e.preventDefault();

                var fila = e.target.closest("tr");
                var accion = fila ? fila.querySelector("[data-guardar]") : porId("btnAgregarModelo");
                if (accion) accion.click();
            });

            // Si el detalle de un dispositivo llega abierto (por ejemplo después de dar un alta), se lo deja a la vista.
            window.addEventListener("load", function () {
                var abierto = document.querySelector(".fila-detalle .acordeon-panel.abierto");
                if (abierto && abierto.scrollIntoView) abierto.scrollIntoView({ block: "nearest" });
            });
        })();
    </script>
</asp:Content>
