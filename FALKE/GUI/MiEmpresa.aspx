<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="MiEmpresa.aspx.cs" Inherits="GUI.MiEmpresa" Title="[[app_mi_empresa]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        .ficha-empresa
        {
            display: flex;
            align-items: center;
            gap: 16px;
            flex-wrap: wrap;
            padding: 16px 20px;
            border-radius: var(--radio-lg);
            background: var(--marca-azul);
            color: var(--marca-perla);
            margin-bottom: 22px;
        }

        .ficha-empresa .sigla
        {
            width: 46px;
            height: 46px;
            flex: none;
            border-radius: var(--radio);
            background: var(--marca-dorado);
            color: var(--marca-azul);
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
        }

        .ficha-empresa .derecha
        {
            margin-left: auto;
            display: flex;
            gap: 8px;
            align-items: center;
        }

        .datos-empresa
        {
            width: 100%;
            border-collapse: collapse;
            font-size: .92rem;
        }

        .datos-empresa th,
        .datos-empresa td
        {
            padding: 10px 0;
            border-bottom: 1px solid var(--borde);
            text-align: left;
            vertical-align: top;
        }

        .datos-empresa th
        {
            width: 40%;
            font-weight: 600;
            color: var(--texto-suave);
            padding-right: 16px;
        }

        .datos-empresa tr:last-child th,
        .datos-empresa tr:last-child td
        {
            border-bottom: 0;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1>[[app_mi_empresa]]</h1>
            <p class="texto-suave sin-margen">[[miempresa_datos_de_tu_empresa_en_falke]]</p>
        </div>
        <div class="acciones">
            <a class="btn btn-primario" href="<%: ResolveUrl("~/Usuarios.aspx") %>">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-usuarios" /></svg>
                <span>[[miempresa_ver_empleados]]</span>
            </a>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <div class="aviso aviso-info">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen">
            <asp:PlaceHolder ID="phContactoEditable" runat="server">
                [[miempresa_el_rubro_el_domicilio_y_el]]
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phSoloPatternBlue" runat="server">
                [[miempresa_estos_datos_los_administra_pattern_blue]]
            </asp:PlaceHolder>
        </p>
    </div>

    <asp:PlaceHolder ID="phDatos" runat="server">

        <section class="ficha-empresa" aria-label="[[miempresa_tu_empresa]]">
            <span class="sigla" aria-hidden="true"><asp:Literal ID="litSigla" runat="server" /></span>
            <div>
                <strong style="font-size:1.1rem"><asp:Literal ID="litNombre" runat="server" /></strong>
                <div class="texto-chico" style="color: rgba(252, 252, 247, .78)">[[miempresa_cliente_desde]] <asp:Literal ID="litAlta" runat="server" /></div>
            </div>
            <div class="derecha">
                <span class="badge badge-alerta"><asp:Literal ID="litPlanFicha" runat="server" /></span>
                <span class="badge" id="badgeEstado" runat="server"><asp:Literal ID="litEstadoFicha" runat="server" /></span>
            </div>
        </section>

        <div class="metricas mb-24">
            <div>
                <div class="metrica-valor"><asp:Literal ID="litUsuarios" runat="server" /></div>
                <div class="metrica-nombre">[[miempresa_empleados_con_cuenta]]</div>
            </div>
            <div>
                <div class="metrica-valor"><asp:Literal ID="litDispositivos" runat="server" /></div>
                <div class="metrica-nombre">[[miempresa_dispositivos_en_prestamo]]</div>
            </div>
            <div>
                <div class="metrica-valor"><asp:Literal ID="litSesiones" runat="server" /></div>
                <div class="metrica-nombre">[[miempresa_sesiones_grabadas]]</div>
            </div>
        </div>

        <div class="grid grid-2">

            <section class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-empresa" /></svg>
                    <span>[[empresanueva_datos_de_la_empresa]]</span>
                    <button type="button" id="btnEditarContacto" runat="server" class="btn btn-secundario btn-chico" style="margin-left:auto" data-abre-modal="modalContacto">[[miempresa_editar_contacto]]</button>
                </div>
                <table class="datos-empresa">
                    <tbody>
                        <tr><th scope="row">[[empresanueva_razon_social]]</th><td><asp:Literal ID="litRazon" runat="server" /></td></tr>
                        <tr><th scope="row">[[empresanueva_cuit]]</th><td><asp:Literal ID="litCuit" runat="server" /></td></tr>
                        <tr><th scope="row">[[empresanueva_rubro]]</th><td><asp:Literal ID="litRubro" runat="server" /></td></tr>
                        <tr><th scope="row">[[empresanueva_domicilio]]</th><td><asp:Literal ID="litDomicilio" runat="server" /></td></tr>
                        <tr><th scope="row">[[empresas_telefono_de_contacto]]</th><td><asp:Literal ID="litTelefono" runat="server" /></td></tr>
                    </tbody>
                </table>
            </section>

            <section class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-calendario" /></svg>
                    <span>[[miempresa_plan_y_facturacion]]</span>
                </div>
                <table class="datos-empresa">
                    <tbody>
                        <tr><th scope="row">[[miempresa_plan_contratado]]</th><td><asp:Literal ID="litPlan" runat="server" /></td></tr>
                        <tr><th scope="row">[[empresanueva_facturacion]]</th><td><asp:Literal ID="litFacturacion" runat="server" /></td></tr>
                        <tr><th scope="row">[[miempresa_proxima_renovacion]]</th><td><asp:Literal ID="litRenovacion" runat="server" /></td></tr>
                        <tr><th scope="row">[[miempresa_estado_de_la_cuenta]]</th><td><asp:Literal ID="litEstado" runat="server" /></td></tr>
                    </tbody>
                </table>
            </section>

        </div>

        <section class="tarjeta mt-24">
            <div class="tarjeta-cabecera">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-dispositivo" /></svg>
                <span>[[miempresa_dispositivos_en_prestamo]]</span>
            </div>
            <div class="tabla-scroll">
                <table class="tabla">
                    <caption class="solo-lectores">[[miempresa_dispositivos_que_la_empresa_tiene_en]]</caption>
                    <thead>
                        <tr>
                            <th scope="col">[[dispositivos_numero_de_serie]]</th>
                            <th scope="col">[[dispositivos_modelo]]</th>
                            <th scope="col">[[dispositivos_entregado]]</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptDispositivos" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="mono"><%#: Eval("NumeroSerie") %></td>
                                    <td><%#: Eval("Modelo") %></td>
                                    <td class="texto-suave"><%#: FechaDispositivo((DateTime?)Eval("FechaEntregaActual")) %></td>                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
            <asp:PlaceHolder ID="phSinDispositivos" runat="server" Visible="false">
                <p class="texto-chico texto-suave mt-8 sin-margen">[[miempresa_todavia_no_tenes_dispositivos_en_prestam]]</p>
            </asp:PlaceHolder>
        </section>

    </asp:PlaceHolder>

    <div class="modal-fondo" id="modalContacto" role="dialog" aria-modal="true" aria-labelledby="contactoTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="contactoTitulo">[[miempresa_editar_datos_de_contacto]]</h2>
                    <p class="subtitulo sin-margen">[[miempresa_rubro_domicilio_y_telefono_de_tu]]</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="[[categorias_cerrar]]">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="campo">
                    <label for="edRubro">[[empresanueva_rubro]]</label>
                    <asp:TextBox ID="edRubro" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" />
                </div>
                <div class="campo">
                    <label for="edDomicilio">[[empresanueva_domicilio]]</label>
                    <asp:TextBox ID="edDomicilio" runat="server" CssClass="entrada" MaxLength="200" ClientIDMode="Static" />
                </div>
                <div class="campo mb-0">
                    <label for="edTelefono">[[empresas_telefono_de_contacto]]</label>
                    <asp:TextBox ID="edTelefono" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" />
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>[[categorias_cancelar]]</button>
                <asp:Button ID="btnGuardarContacto" runat="server" CssClass="btn btn-primario" Text="[[comun_guardar]]" OnClick="btnGuardarContacto_Click" />
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlError" runat="server" CssClass="aviso aviso-peligro" Visible="false" role="alert">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
        <p class="sin-margen"><asp:Literal ID="litError" runat="server" /></p>
    </asp:Panel>

</asp:Content>
