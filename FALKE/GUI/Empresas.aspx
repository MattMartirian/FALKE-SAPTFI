<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Empresas.aspx.cs" Inherits="GUI.Empresas" Title="Empresas cliente" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        .emp
        {
            display: grid;
            grid-template-columns: 320px minmax(0, 1fr);
            gap: 20px;
            align-items: start;
        }

        .emp-rail
        {
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
            overflow: hidden;
        }

        .emp-rail-cab
        {
            padding: 14px;
            border-bottom: 1px solid var(--borde);
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .emp-rail-cab .titulo
        {
            font-size: .84rem;
            font-weight: 600;
            color: var(--texto-suave);
            display: flex;
            justify-content: space-between;
        }

        .emp-buscador
        {
            position: relative;
        }

        .emp-lista
        {
            max-height: 620px;
            overflow-y: auto;
        }

        .emp-fila
        {
            display: flex;
            align-items: flex-start;
            gap: 12px;
            width: 100%;
            text-align: left;
            padding: 13px 14px;
            border: 0;
            border-bottom: 1px solid var(--borde);
            background: transparent;
            color: var(--texto);
            font-family: inherit;
            cursor: pointer;
        }

        .emp-fila:hover
        {
            background: var(--superficie-2);
        }

        .emp-fila[aria-current="true"]
        {
            background: var(--acento-suave);
            outline: 1px solid var(--acento);
            outline-offset: -1px;
        }

        .emp-fila .sigla
        {
            width: 38px;
            height: 38px;
            flex: none;
            border-radius: var(--radio-sm);
            background: var(--marca-azul);
            color: var(--marca-dorado);
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            font-size: .82rem;
        }

        .emp-fila .info
        {
            min-width: 0;
            flex: 1;
        }

        .emp-fila .nombre
        {
            display: block;
            font-weight: 600;
            font-size: .9rem;
        }

        .emp-fila .rubro
        {
            display: block;
            font-size: .76rem;
            color: var(--texto-suave);
            margin-top: 2px;
        }

        .emp-fila .marcas
        {
            display: flex;
            flex-wrap: wrap;
            gap: 4px;
            margin-top: 6px;
        }

        .senal-vencido
        {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-family: var(--fuente-mono);
            font-size: .62rem;
            letter-spacing: .06em;
            text-transform: uppercase;
            color: var(--marca-coral);
        }

        .senal-vencido::before
        {
            content: "";
            width: 6px;
            height: 6px;
            border-radius: 50%;
            background: var(--marca-coral);
        }

        .emp-panel
        {
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
            padding: 22px 24px 26px;
        }

        .emp-detalle-cab
        {
            display: flex;
            align-items: center;
            gap: 14px;
            flex-wrap: wrap;
            margin-bottom: 8px;
        }

        .emp-detalle-cab .sigla
        {
            width: 48px;
            height: 48px;
            flex: none;
            border-radius: var(--radio);
            background: var(--marca-azul);
            color: var(--marca-dorado);
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
        }

        .emp-detalle-cab h2
        {
            margin: 0 0 3px;
            font-size: 1.25rem;
        }

        .emp-detalle-cab .cuit
        {
            font-family: var(--fuente-mono);
            font-size: .78rem;
            color: var(--texto-suave);
        }

        .emp-detalle-cab .marcas
        {
            margin-left: auto;
            display: flex;
            gap: 6px;
        }

        .emp-datos
        {
            width: 100%;
            border-collapse: collapse;
        }

        .emp-datos th,
        .emp-datos td
        {
            text-align: left;
            padding: 10px 0;
            border-bottom: 1px solid var(--borde);
            font-size: .9rem;
            vertical-align: top;
        }

        .emp-datos th
        {
            width: 210px;
            color: var(--texto-suave);
            font-weight: 600;
        }

        .emp-acciones
        {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 22px;
        }

        @media (max-width: 940px)
        {
            .emp { grid-template-columns: minmax(0, 1fr); }
            .emp-lista { max-height: 340px; }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="empresas.titulo">Empresas cliente</h1>
            <p class="texto-suave sin-margen" data-i18n="empresas.bajada">
                Cartera de empresas que contrataron Falke, con su plan y su estado.
            </p>
        </div>
        <div class="acciones">
            <a class="btn btn-primario" href="<%: ResolveUrl("~/EmpresaNueva.aspx") %>">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span data-i18n="empresas.nueva">Nueva empresa</span>
            </a>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso aviso-exito" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <div class="metricas mb-24">
        <div>
            <div class="metrica-valor"><asp:Literal ID="litTotal" runat="server" /></div>
            <div class="metrica-nombre" data-i18n="empresas.metrica.total">Empresas cliente</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litActivas" runat="server" /></div>
            <div class="metrica-nombre" data-i18n="empresas.metrica.activas">Activas</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litDispositivos" runat="server" /></div>
            <div class="metrica-nombre" data-i18n="empresas.metrica.dispositivos">Dispositivos prestados</div>
        </div>
        <div>
            <div class="metrica-valor"><asp:Literal ID="litUsuarios" runat="server" /></div>
            <div class="metrica-nombre" data-i18n="empresas.metrica.usuarios">Usuarios totales</div>
        </div>
    </div>

    <div class="emp">

        <div class="emp-rail">
            <div class="emp-rail-cab">
                <span class="titulo">
                    <span data-i18n="empresas.registradas">Empresas registradas</span>
                    <span id="empCuenta"><asp:Literal ID="litCuenta" runat="server" /></span>
                </span>
                <div class="emp-buscador campo-buscar">
                    <label class="solo-lectores" for="empBuscar" data-i18n="empresas.buscar">Buscar</label>
                    <svg aria-hidden="true"><use href="#i-buscar" /></svg>
                    <input type="search" class="entrada" id="empBuscar"
                           placeholder="Razón social, CUIT o rubro..." data-i18n-attr="placeholder:empresas.buscar.placeholder" />
                </div>
                <div class="chips" data-unico role="group" aria-label="Filtrar por estado">
                    <button type="button" class="chip" aria-pressed="true" data-estado-filtro="" data-i18n="comun.todos">Clientes</button>
                    <button type="button" class="chip" aria-pressed="false" data-estado-filtro="Activa" data-i18n="estado.activa">Activas</button>
                    <button type="button" class="chip" aria-pressed="false" data-estado-filtro="Bloqueada" data-i18n="estado.bloqueada">Bloqueadas</button>
                    <button type="button" class="chip" aria-pressed="false" data-estado-filtro="Deshabilitada">Dadas de baja<asp:Literal ID="litBajas" runat="server" /></button>
                </div>
            </div>

            <div class="emp-lista" id="empLista" role="listbox" aria-label="Empresas cliente">

                <asp:Repeater ID="rptEmpresas" runat="server">
                    <ItemTemplate>
                        <button type="button" class="emp-fila" role="option"
                                data-id="<%#: Eval("IdEmpresa") %>"
                                data-nombre="<%#: Eval("NombreEmpresa") %>" data-razon="<%#: Eval("NombreEmpresa") %>" data-sigla="<%#: Sigla((string)Eval("NombreEmpresa")) %>"
                                data-cuit="<%#: Dato((string)Eval("Cuit")) %>" data-rubro="<%#: Dato((string)Eval("Rubro")) %>"
                                data-plan="<%#: Eval("PlanSuscripcion") %>" data-factura="<%#: Factura(Container.DataItem) %>"
                                data-estado="<%#: Eval("Estado") %>" data-alta="<%#: Fecha((DateTime?)Eval("FechaAlta")) %>"
                                data-renovacion="<%#: Fecha((DateTime?)Eval("FechaRenovacion")) %>"
                                data-usuarios="<%#: Eval("CantidadUsuarios") %>" data-dispositivos="<%#: Eval("DispositivosPrestados") %>" data-sesiones="<%#: Eval("SesionesGrabadas") %>"
                                data-telefono="<%#: Dato((string)Eval("NumContactoEmpresa")) %>" data-domicilio="<%#: Dato((string)Eval("Domicilio")) %>"
                                data-e-nombre="<%#: Eval("NombreEmpresa") %>" data-e-cuit="<%#: Eval("Cuit") %>" data-e-rubro="<%#: Eval("Rubro") %>"
                                data-e-domicilio="<%#: Eval("Domicilio") %>" data-e-telefono="<%#: Eval("NumContactoEmpresa") %>"
                                data-e-plan="<%#: Eval("PlanSuscripcion") %>" data-e-factura="<%#: Eval("Facturacion") %>"
                                data-proveedora="<%#: (int)Eval("IdEmpresa") == TLL.BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA ? "1" : "0" %>"
                                data-vencido="0">
                            <span class="sigla" aria-hidden="true"><%#: Sigla((string)Eval("NombreEmpresa")) %></span>
                            <span class="info">
                                <span class="nombre"><%#: Eval("NombreEmpresa") %></span>
                                <span class="rubro"><%#: Dato((string)Eval("Rubro")) %></span>
                                <span class="marcas">
                                    <span class="badge <%#: ClasePlan((BE.PlanSuscripcion)Eval("PlanSuscripcion")) %>"><%#: Eval("PlanSuscripcion") %></span>
                                    <span class="badge <%#: ClaseEstado((BE.EstadoEmpresa)Eval("Estado")) %>"><%#: Eval("Estado") %></span>
                                </span>
                            </span>
                        </button>
                    </ItemTemplate>
                </asp:Repeater>

            </div>

            <div class="vacio" id="empVacio" hidden>
                <svg width="28" height="28" aria-hidden="true"><use href="#i-empresa" /></svg>
                <p class="sin-margen" data-i18n="empresas.vacio">No hay empresas que coincidan.</p>
            </div>
        </div>

        <div class="emp-panel">
            <div id="empDetalle" data-puede-editar="<%= PuedeEditar ? "1" : "0" %>" data-puede-estado="<%= PuedeCambiarEstado ? "1" : "0" %>"></div>
        </div>

    </div>

    <asp:HiddenField ID="hfSeleccion" runat="server" ClientIDMode="Static" />

    <div class="modal-fondo" id="modalEditarEmpresa" role="dialog" aria-modal="true" aria-labelledby="editarEmpresaTitulo" hidden>
        <div class="modal ancho">
            <div class="modal-cabecera">
                <div>
                    <h2 id="editarEmpresaTitulo">Editar datos de la empresa</h2>
                    <p class="subtitulo sin-margen">Cambiar la razón social, el CUIT o el plan exige indicar el motivo. Todo queda en la bitácora.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <asp:HiddenField ID="hfEdId" runat="server" ClientIDMode="Static" />
                <div class="aviso aviso-alerta">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <div>
                        <strong>Estás modificando los datos de un cliente: <span id="avisoEditarEmpresa"></span>.</strong>
                        <p class="sin-margen">Lo que cambies lo ve la empresa en «Mi empresa». Cambiar el CUIT, el plan o la facturación afecta la facturación. Queda en la bitácora y la empresa lo ve como hecho por el equipo de Pattern Blue.</p>
                    </div>
                </div>
                <div class="fila-campos">
                    <div class="campo">
                        <label for="edRazon">Razón social</label>
                        <asp:TextBox ID="edRazon" runat="server" CssClass="entrada" MaxLength="150" ClientIDMode="Static" />
                    </div>
                    <div class="campo">
                        <label for="edCuit">CUIT</label>
                        <asp:TextBox ID="edCuit" runat="server" CssClass="entrada" MaxLength="20" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="fila-campos">
                    <div class="campo">
                        <label for="edRubro">Rubro</label>
                        <asp:TextBox ID="edRubro" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" />
                    </div>
                    <div class="campo">
                        <label for="edContacto">Teléfono de contacto</label>
                        <asp:TextBox ID="edContacto" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="campo">
                    <label for="edDomicilio">Domicilio</label>
                    <asp:TextBox ID="edDomicilio" runat="server" CssClass="entrada" MaxLength="200" ClientIDMode="Static" />
                </div>
                <div class="fila-campos">
                    <div class="campo">
                        <label for="edPlan">Plan de suscripción</label>
                        <asp:DropDownList ID="edPlan" runat="server" CssClass="entrada" ClientIDMode="Static">
                            <asp:ListItem Value="Scout">Scout</asp:ListItem>
                            <asp:ListItem Value="Hunter">Hunter</asp:ListItem>
                            <asp:ListItem Value="Apex">Apex</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="campo">
                        <label for="edFactura">Facturación</label>
                        <asp:DropDownList ID="edFactura" runat="server" CssClass="entrada" ClientIDMode="Static">
                            <asp:ListItem Value="Mensual">Mensual</asp:ListItem>
                            <asp:ListItem Value="Anual">Anual</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="campo mb-0">
                    <label for="edMotivo">Motivo <span class="texto-tenue">(obligatorio si cambia la razón social, el CUIT o el plan)</span></label>
                    <asp:TextBox ID="edMotivo" runat="server" CssClass="entrada" MaxLength="300" ClientIDMode="Static" />
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cancelar</button>
                <asp:Button ID="btnGuardarEmpresa" runat="server" CssClass="btn btn-primario" Text="Guardar cambios" OnClick="btnGuardarEmpresa_Click" />
            </div>
        </div>
    </div>

    <div class="modal-fondo" id="modalEstadoEmpresa" role="dialog" aria-modal="true" aria-labelledby="estadoEmpresaTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="estadoEmpresaTitulo">Cambiar el estado de la empresa</h2>
                    <p class="subtitulo sin-margen" id="estadoEmpresaNombre"></p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <asp:HiddenField ID="hfEstId" runat="server" ClientIDMode="Static" />
                <div class="aviso aviso-alerta">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <div>
                        <strong>Esto afecta a todos los usuarios de la empresa.</strong>
                        <p class="sin-margen">Al cambiar el estado a Bloqueada o Deshabilitada nadie de la empresa puede ingresar. Las cuentas y los datos no se alteran.</p>
                    </div>
                </div>
                <div class="campo">
                    <label for="edEstado">Estado de la empresa</label>
                    <asp:DropDownList ID="edEstado" runat="server" CssClass="entrada" ClientIDMode="Static">
                        <asp:ListItem Value="Activa">Activa</asp:ListItem>
                        <asp:ListItem Value="Bloqueada">Bloqueada</asp:ListItem>
                        <asp:ListItem Value="Deshabilitada">Deshabilitada</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="campo mb-0">
                    <label for="edMotivoEstado">Motivo <span class="texto-tenue">(obligatorio)</span></label>
                    <asp:TextBox ID="edMotivoEstado" runat="server" CssClass="entrada" MaxLength="300" ClientIDMode="Static" />
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cancelar</button>
                <asp:Button ID="btnCambiarEstadoEmpresa" runat="server" CssClass="btn btn-primario" Text="Cambiar estado" OnClick="btnCambiarEstadoEmpresa_Click" />
            </div>
        </div>
    </div>

    <script type="application/json" id="empTextos">
    {
        "tabDatos": "Datos",
        "tabPlan": "Plan y facturación",
        "tabEquipo": "Equipo y dispositivos",
        "tabSesiones": "Actividad",
        "razon": "Razón social",
        "cuit": "CUIT",
        "rubro": "Rubro",
        "domicilio": "Domicilio",
        "contacto": "Teléfono de contacto",
        "estado": "Estado",
        "alta": "Cliente desde",
        "plan": "Plan contratado",
        "factura": "Facturación",
        "renovacion": "Próxima renovación",
        "pago": "Último pago registrado",
        "limite": "Límite de usuarios",
        "usuarios": "Usuarios de la cuenta",
        "dispositivos": "Dispositivos en préstamo",
        "sesiones": "Sesiones grabadas",
        "editar": "Editar datos",
        "bloquear": "Bloquear empresa",
        "desbloquear": "Desbloquear empresa",
        "verUsuarios": "Ver usuarios",
        "estadoEmpresa": "Cambiar estado",
        "vencidoAviso": "Esta empresa tiene un pago vencido. El acceso de sus usuarios está suspendido hasta la regularización."
    }
    </script>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var T = JSON.parse(document.getElementById("empTextos").textContent);
            var filas = document.querySelectorAll(".emp-fila");
            var buscar = document.getElementById("empBuscar");
            var chips = document.querySelectorAll(".chips [data-estado-filtro]");
            var vacio = document.getElementById("empVacio");
            var detalle = document.getElementById("empDetalle");
            var cuenta = document.getElementById("empCuenta");
            var estadoFiltro = "";
            var actual = null;

            function esc(texto) {
                var nodo = document.createElement("div");
                nodo.textContent = texto == null ? "" : texto;
                return nodo.innerHTML;
            }

            function badgeEstado(estado) {
                var mapa = {
                    "Activa": "badge-exito", "Bloqueada": "badge-peligro", "Deshabilitada": "badge-neutro"
                };
                return '<span class="badge ' + (mapa[estado] || "badge-neutro") + '">' + esc(estado) + '</span>';
            }

            function fila(th, td) {
                return '<tr><th>' + th + '</th><td>' + td + '</td></tr>';
            }

            function panelDatos(d) {
                return '<table class="emp-datos"><tbody>' +
                    fila(T.razon, esc(d.razon)) +
                    fila(T.cuit, esc(d.cuit)) +
                    fila(T.rubro, esc(d.rubro)) +
                    fila(T.domicilio, esc(d.domicilio)) +
                    fila(T.contacto, esc(d.telefono)) +
                    fila(T.estado, badgeEstado(d.estado)) +
                    fila(T.alta, esc(d.alta)) +
                    '</tbody></table>';
            }

            function panelPlan(d) {
                return '<table class="emp-datos"><tbody>' +
                    fila(T.plan, esc(d.plan)) +
                    fila(T.factura, esc(d.factura)) +
                    fila(T.renovacion, esc(d.renovacion)) +
                    fila(T.usuarios, esc(d.usuarios)) +
                    '</tbody></table>';
            }

            function panelEquipo(d) {
                return '<table class="emp-datos"><tbody>' +
                    fila(T.usuarios, esc(d.usuarios)) +
                    fila(T.dispositivos, esc(d.dispositivos)) +
                    '</tbody></table>' +
                    '<div class="emp-acciones"><a class="btn btn-secundario" href="Dispositivos.aspx">' +
                    T.dispositivos + '</a></div>';
            }

            function panelSesiones(d) {
                return '<div class="metricas">' +
                    '<div><div class="metrica-valor">' + esc(d.sesiones) + '</div><div class="metrica-nombre">' + T.sesiones + '</div></div>' +
                    '<div><div class="metrica-valor">' + esc(d.usuarios) + '</div><div class="metrica-nombre">' + T.usuarios + '</div></div>' +
                    '<div><div class="metrica-valor">' + esc(d.dispositivos) + '</div><div class="metrica-nombre">' + T.dispositivos + '</div></div>' +
                    '</div>';
            }

            function mostrar(f) {
                for (var i = 0; i < filas.length; i++) filas[i].removeAttribute("aria-current");
                f.setAttribute("aria-current", "true");

                var d = f.dataset;
                actual = d;
                document.getElementById("hfSeleccion").value = d.id;
                var aviso = d.vencido === "1"
                    ? '<div class="aviso aviso-peligro mb-24"><svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>' +
                      '<p class="sin-margen">' + T.vencidoAviso + '</p></div>'
                    : '';

                detalle.innerHTML =
                    '<div class="emp-detalle-cab">' +
                        '<span class="sigla" aria-hidden="true">' + esc(d.sigla) + '</span>' +
                        '<div><h2>' + esc(d.nombre) + '</h2><span class="cuit">' + esc(d.cuit) + '</span></div>' +
                        '<span class="marcas"><span class="badge badge-alerta">' + esc(d.plan) + '</span>' + badgeEstado(d.estado) + '</span>' +
                    '</div>' +
                    aviso +
                    '<div class="pestanas" role="tablist">' +
                        '<button type="button" class="pestana" role="tab" aria-selected="true" data-pestana="tabDatos">' + T.tabDatos + '</button>' +
                        '<button type="button" class="pestana" role="tab" aria-selected="false" data-pestana="tabPlan">' + T.tabPlan + '</button>' +
                        '<button type="button" class="pestana" role="tab" aria-selected="false" data-pestana="tabEquipo">' + T.tabEquipo + '</button>' +
                        '<button type="button" class="pestana" role="tab" aria-selected="false" data-pestana="tabSesiones">' + T.tabSesiones + '</button>' +
                    '</div>' +
                    '<div id="tabDatos" role="tabpanel">' + panelDatos(d) + '</div>' +
                    '<div id="tabPlan" role="tabpanel" hidden>' + panelPlan(d) + '</div>' +
                    '<div id="tabEquipo" role="tabpanel" hidden>' + panelEquipo(d) + '</div>' +
                    '<div id="tabSesiones" role="tabpanel" hidden>' + panelSesiones(d) + '</div>' +
                    '<div class="emp-acciones">' +
                        '<a class="btn btn-secundario" href="Usuarios.aspx?empresa=' + encodeURIComponent(d.id) + '">' + T.verUsuarios + '</a>' +
                        (detalle.dataset.puedeEditar === "1" ? '<button type="button" class="btn btn-secundario" data-abre-modal="modalEditarEmpresa" data-editar-empresa>' + T.editar + '</button>' : '') +
                        (detalle.dataset.puedeEstado === "1" && d.proveedora !== "1" ? '<button type="button" class="btn btn-secundario" data-abre-modal="modalEstadoEmpresa" data-estado-empresa>' + T.estadoEmpresa + '</button>' : '') +
                    '</div>';

                var pestanas = detalle.querySelectorAll(".pestana");
                for (var p = 0; p < pestanas.length; p++) {
                    pestanas[p].addEventListener("click", function () {
                        var destino = this.getAttribute("data-pestana");
                        for (var q = 0; q < pestanas.length; q++) {
                            var esta = pestanas[q] === this;
                            pestanas[q].setAttribute("aria-selected", esta ? "true" : "false");
                            document.getElementById(pestanas[q].getAttribute("data-pestana")).hidden = !esta;
                        }
                    });
                }
            }

            for (var i = 0; i < filas.length; i++) {
                filas[i].addEventListener("click", function () { mostrar(this); });
            }

            function filtrar() {
                var texto = buscar.value.trim().toLowerCase();
                var visibles = 0;
                for (var j = 0; j < filas.length; j++) {
                    var f = filas[j];
                    var busca = (f.dataset.nombre + " " + f.dataset.cuit + " " + f.dataset.rubro).toLowerCase();
                    var okTexto = texto === "" || busca.indexOf(texto) !== -1;
                    var okEstado = estadoFiltro === "" ? f.dataset.estado !== "Deshabilitada" : f.dataset.estado === estadoFiltro;
                    var ver = okTexto && okEstado;
                    f.hidden = !ver;
                    if (ver) visibles++;
                }
                vacio.hidden = visibles > 0;
                cuenta.textContent = visibles;
            }

            buscar.addEventListener("input", filtrar);

            for (i = 0; i < chips.length; i++) {
                chips[i].addEventListener("click", function () {
                    for (var k = 0; k < chips.length; k++) {
                        chips[k].setAttribute("aria-pressed", chips[k] === this ? "true" : "false");
                    }
                    estadoFiltro = this.getAttribute("data-estado-filtro");
                    filtrar();
                });
            }

            var pedida = document.getElementById("hfSeleccion").value || new URLSearchParams(window.location.search).get("id") || "";
            var elegida = document.querySelector('.emp-fila[data-id="' + pedida + '"]');

            document.addEventListener("click", function (e) {
                var editar = e.target.closest ? e.target.closest("[data-editar-empresa]") : null;
                var estado = e.target.closest ? e.target.closest("[data-estado-empresa]") : null;

                if (editar && actual) {
                    document.getElementById("hfEdId").value = actual.id;
                    document.getElementById("avisoEditarEmpresa").textContent = actual.eNombre || actual.nombre || "";
                    document.getElementById("edRazon").value = actual.eNombre || "";
                    document.getElementById("edCuit").value = actual.eCuit || "";
                    document.getElementById("edRubro").value = actual.eRubro || "";
                    document.getElementById("edDomicilio").value = actual.eDomicilio || "";
                    document.getElementById("edContacto").value = actual.eTelefono || "";
                    document.getElementById("edPlan").value = actual.ePlan || "Scout";
                    document.getElementById("edFactura").value = actual.eFactura || "Mensual";
                    document.getElementById("edMotivo").value = "";
                }

                if (estado && actual) {
                    document.getElementById("hfEstId").value = actual.id;
                    document.getElementById("estadoEmpresaNombre").textContent = actual.nombre + " · estado actual: " + actual.estado;
                    var lista = document.getElementById("edEstado");
                    lista.value = actual.estado === "Activa" ? "Bloqueada" : "Activa";
                    for (var o = 0; o < lista.options.length; o++) {
                        lista.options[o].disabled = actual.estado === "Deshabilitada" && lista.options[o].value !== "Activa";
                    }
                    document.getElementById("edMotivoEstado").value = "";
                }
            });

            if (elegida && elegida.dataset.estado === "Deshabilitada") {
                var chipBaja = document.querySelector('[data-estado-filtro="Deshabilitada"]');
                if (chipBaja) chipBaja.click();
            } else {
                filtrar();
            }

            var primera = elegida || [].slice.call(filas).filter(function (f) { return !f.hidden; })[0];
            if (primera) mostrar(primera);
            else vacio.hidden = false;
        })();
    </script>
</asp:Content>
