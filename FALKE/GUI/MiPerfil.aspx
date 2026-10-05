<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="MiPerfil.aspx.cs" Inherits="GUI.MiPerfil" Title="Mi perfil" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .ficha-perfil
        {
            display: flex;
            align-items: center;
            gap: 18px;
            flex-wrap: wrap;
            padding: 20px 24px;
            border-radius: var(--radio-lg);
            background: var(--marca-azul);
            color: var(--marca-perla);
            margin-bottom: 24px;
        }

        .ficha-perfil .iniciales
        {
            width: 64px;
            height: 64px;
            flex: none;
            border-radius: var(--radio-full);
            background: var(--marca-dorado);
            color: var(--marca-azul);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.35rem;
            font-weight: 700;
        }

        .ficha-perfil .nombre
        {
            font-size: 1.25rem;
            font-weight: 700;
            margin: 0;
        }

        .ficha-perfil .detalle
        {
            color: rgba(252, 252, 247, .78);
            font-size: .92rem;
        }

        .ficha-perfil .etiquetas
        {
            margin-left: auto;
            display: flex;
            gap: 8px;
            align-items: center;
            flex-wrap: wrap;
        }

        .datos-perfil
        {
            width: 100%;
            border-collapse: collapse;
            font-size: .92rem;
        }

        .datos-perfil th,
        .datos-perfil td
        {
            padding: 10px 0;
            border-bottom: 1px solid var(--borde);
            text-align: left;
            vertical-align: top;
        }

        .datos-perfil th
        {
            width: 38%;
            font-weight: 600;
            color: var(--texto-suave);
            padding-right: 16px;
        }

        .datos-perfil tr:last-child th,
        .datos-perfil tr:last-child td
        {
            border-bottom: 0;
        }

    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1>Mi perfil</h1>
            <p class="texto-suave sin-margen">Tus datos, tu acceso y tu actividad reciente.</p>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <section class="ficha-perfil" aria-label="Tu cuenta">
        <span class="iniciales" aria-hidden="true"><asp:Literal ID="litIniciales" runat="server" /></span>
        <div>
            <p class="nombre"><asp:Literal ID="litNombreCompleto" runat="server" /></p>
            <div class="detalle"><asp:Literal ID="litCorreoFicha" runat="server" /></div>
        </div>
        <div class="etiquetas">
            <span class="badge badge-alerta"><asp:Literal ID="litRol" runat="server" /></span>
            <span class="badge badge-neutro"><asp:Literal ID="litEmpresaFicha" runat="server" /></span>
        </div>
    </section>

    <div class="grid grid-2">

        <section class="tarjeta">
            <div class="tarjeta-cabecera">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-persona" /></svg>
                <span>Mis datos</span>
            </div>

            <asp:PlaceHolder ID="phDatos" runat="server">
                <div class="fila-campos">
                    <div class="campo">
                        <label for="txtNombre">Nombre</label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" />
                    </div>
                    <div class="campo">
                        <label for="txtApellido">Apellido</label>
                        <asp:TextBox ID="txtApellido" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="campo">
                    <label for="txtCorreo">Correo electrónico</label>
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="entrada" ReadOnly="true" ClientIDMode="Static" />
                    <p class="ayuda">Es tu identidad de acceso: no se cambia desde acá. Si necesitás cambiarlo, pedíselo a quien administra tu cuenta.</p>
                </div>
                <div class="campo">
                    <label for="ddlIdioma">Idioma de la interfaz</label>
                    <asp:DropDownList ID="ddlIdioma" runat="server" CssClass="entrada" ClientIDMode="Static">
                        <asp:ListItem Value="1">Español</asp:ListItem>
                        <asp:ListItem Value="2">Inglés</asp:ListItem>
                        <asp:ListItem Value="3">Portugués</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primario" Text="Guardar cambios" OnClick="btnGuardar_Click" CausesValidation="false" />
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEmergencia" runat="server" Visible="false">
                <p class="texto-suave sin-margen">
                    Es el acceso de emergencia: no es una cuenta de la base de datos, así que no tiene datos personales para editar.
                </p>
            </asp:PlaceHolder>
        </section>

        <section class="tarjeta">
            <div class="tarjeta-cabecera">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-candado" /></svg>
                <span>Acceso y seguridad</span>
            </div>
            <table class="datos-perfil">
                <tbody>
                    <tr><th scope="row">Rol</th><td><asp:Literal ID="litRolDetalle" runat="server" /></td></tr>
                    <tr><th scope="row">Empresa</th><td><asp:Literal ID="litEmpresaDetalle" runat="server" /></td></tr>
                    <tr><th scope="row">Contraseña</th>
                        <td>
                            <asp:PlaceHolder ID="phCambiarClave" runat="server">
                                <a class="btn btn-secundario btn-chico" href="<%: ResolveUrl("~/MiClave.aspx") %>">Cambiar mi contraseña</a>
                                <p class="texto-chico texto-suave mt-8 sin-margen">Al cambiarla, las sesiones abiertas en otros equipos se cierran.</p>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phSinClave" runat="server" Visible="false">
                                <span class="texto-suave">No tiene: el acceso de emergencia se define en la configuración.</span>
                            </asp:PlaceHolder>
                        </td>
                    </tr>
                </tbody>
            </table>
        </section>

    </div>

    <section class="tarjeta mt-24">
        <div class="tarjeta-cabecera">
            <svg width="18" height="18" aria-hidden="true"><use href="#i-reloj" /></svg>
            <span>Mi actividad reciente</span>
        </div>
        <div class="tabla-scroll">
            <table class="tabla">
                <caption class="solo-lectores">Los últimos movimientos hechos desde tu cuenta</caption>
                <thead>
                    <tr>
                        <th scope="col">Fecha y hora</th>
                        <th scope="col">Módulo</th>
                        <th scope="col">Descripción</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptActividad" runat="server" EnableViewState="false">
                        <ItemTemplate>
                            <tr>
                                <td class="celda-fecha"><%#: ((DateTime)Eval("FechaHoraBitacora")).ToString("dd/MM/yyyy HH:mm") %></td>
                                <td><span class="badge badge-neutro"><%#: Eval("ModuloBitacora") %></span></td>
                                <td><%#: Eval("DescripcionBitacora") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
        <asp:PlaceHolder ID="phSinActividad" runat="server" Visible="false">
            <p class="texto-chico texto-suave mt-8 sin-margen">Todavía no hay movimientos registrados desde tu cuenta.</p>
        </asp:PlaceHolder>
    </section>

</asp:Content>
