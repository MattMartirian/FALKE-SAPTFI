<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="MiEmpresa.aspx.cs" Inherits="GUI.MiEmpresa" Title="Mi empresa" %>

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
            <h1>Mi empresa</h1>
            <p class="texto-suave sin-margen">Datos de tu empresa en Falke.</p>
        </div>
        <div class="acciones">
            <a class="btn btn-primario" href="<%: ResolveUrl("~/Usuarios.aspx") %>">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-usuarios" /></svg>
                <span>Ver empleados</span>
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
                El rubro, el domicilio y el teléfono los actualizás vos. La razón social, el CUIT, el plan y la
                facturación los administra Pattern Blue: si algo cambió, escríbenos y lo actualizamos.
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phSoloPatternBlue" runat="server">
                Estos datos los administra Pattern Blue. Si algo cambió (domicilio, teléfono, razón social),
                escríbenos y lo actualizamos.
            </asp:PlaceHolder>
        </p>
    </div>

    <asp:PlaceHolder ID="phDatos" runat="server">

        <section class="ficha-empresa" aria-label="Tu empresa">
            <span class="sigla" aria-hidden="true"><asp:Literal ID="litSigla" runat="server" /></span>
            <div>
                <strong style="font-size:1.1rem"><asp:Literal ID="litNombre" runat="server" /></strong>
                <div class="texto-chico" style="color: rgba(252, 252, 247, .78)">Cliente desde <asp:Literal ID="litAlta" runat="server" /></div>
            </div>
            <div class="derecha">
                <span class="badge badge-alerta"><asp:Literal ID="litPlanFicha" runat="server" /></span>
                <span class="badge" id="badgeEstado" runat="server"><asp:Literal ID="litEstadoFicha" runat="server" /></span>
            </div>
        </section>

        <div class="metricas mb-24">
            <div>
                <div class="metrica-valor"><asp:Literal ID="litUsuarios" runat="server" /></div>
                <div class="metrica-nombre">Empleados con cuenta</div>
            </div>
            <div>
                <div class="metrica-valor"><asp:Literal ID="litDispositivos" runat="server" /></div>
                <div class="metrica-nombre">Dispositivos en préstamo</div>
            </div>
            <div>
                <div class="metrica-valor"><asp:Literal ID="litSesiones" runat="server" /></div>
                <div class="metrica-nombre">Sesiones grabadas</div>
            </div>
        </div>

        <div class="grid grid-2">

            <section class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-empresa" /></svg>
                    <span>Datos de la empresa</span>
                    <button type="button" id="btnEditarContacto" runat="server" class="btn btn-secundario btn-chico" style="margin-left:auto" data-abre-modal="modalContacto">Editar contacto</button>
                </div>
                <table class="datos-empresa">
                    <tbody>
                        <tr><th scope="row">Razón social</th><td><asp:Literal ID="litRazon" runat="server" /></td></tr>
                        <tr><th scope="row">CUIT</th><td><asp:Literal ID="litCuit" runat="server" /></td></tr>
                        <tr><th scope="row">Rubro</th><td><asp:Literal ID="litRubro" runat="server" /></td></tr>
                        <tr><th scope="row">Domicilio</th><td><asp:Literal ID="litDomicilio" runat="server" /></td></tr>
                        <tr><th scope="row">Teléfono de contacto</th><td><asp:Literal ID="litTelefono" runat="server" /></td></tr>
                    </tbody>
                </table>
            </section>

            <section class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-calendario" /></svg>
                    <span>Plan y facturación</span>
                </div>
                <table class="datos-empresa">
                    <tbody>
                        <tr><th scope="row">Plan contratado</th><td><asp:Literal ID="litPlan" runat="server" /></td></tr>
                        <tr><th scope="row">Facturación</th><td><asp:Literal ID="litFacturacion" runat="server" /></td></tr>
                        <tr><th scope="row">Próxima renovación</th><td><asp:Literal ID="litRenovacion" runat="server" /></td></tr>
                        <tr><th scope="row">Estado de la cuenta</th><td><asp:Literal ID="litEstado" runat="server" /></td></tr>
                    </tbody>
                </table>
            </section>

        </div>

    </asp:PlaceHolder>

    <div class="modal-fondo" id="modalContacto" role="dialog" aria-modal="true" aria-labelledby="contactoTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="contactoTitulo">Editar datos de contacto</h2>
                    <p class="subtitulo sin-margen">Rubro, domicilio y teléfono de tu empresa. Queda registrado en la bitácora.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="campo">
                    <label for="edRubro">Rubro</label>
                    <asp:TextBox ID="edRubro" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" />
                </div>
                <div class="campo">
                    <label for="edDomicilio">Domicilio</label>
                    <asp:TextBox ID="edDomicilio" runat="server" CssClass="entrada" MaxLength="200" ClientIDMode="Static" />
                </div>
                <div class="campo mb-0">
                    <label for="edTelefono">Teléfono de contacto</label>
                    <asp:TextBox ID="edTelefono" runat="server" CssClass="entrada" MaxLength="50" ClientIDMode="Static" />
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cancelar</button>
                <asp:Button ID="btnGuardarContacto" runat="server" CssClass="btn btn-primario" Text="Guardar" OnClick="btnGuardarContacto_Click" />
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlError" runat="server" CssClass="aviso aviso-peligro" Visible="false" role="alert">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
        <p class="sin-margen"><asp:Literal ID="litError" runat="server" /></p>
    </asp:Panel>

</asp:Content>
