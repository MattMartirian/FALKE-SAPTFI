<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="MiPerfil.aspx.cs" Inherits="GUI.MiPerfil" Title="[[app_mi_perfil]]" %>

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
            <h1>[[app_mi_perfil]]</h1>
            <p class="texto-suave sin-margen">[[miperfil_tus_datos_tu_acceso_y_tu]]</p>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <section class="ficha-perfil" aria-label="[[miperfil_tu_cuenta]]">
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
                <span>[[miperfil_mis_datos]]</span>
            </div>

            <asp:PlaceHolder ID="phDatos" runat="server">
                <div class="fila-campos">
                    <div class="campo">
                        <label for="txtNombre">[[dispositivos_nombre]]</label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" />
                    </div>
                    <div class="campo">
                        <label for="txtApellido">[[empresanueva_apellido]]</label>
                        <asp:TextBox ID="txtApellido" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="campo">
                    <label for="txtCorreo">[[empresanueva_correo_electronico]]</label>
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="entrada" ReadOnly="true" ClientIDMode="Static" />
                    <p class="ayuda">[[miperfil_es_tu_identidad_de_acceso_no]]</p>
                </div>
                <div class="campo">
                    <label for="ddlIdioma">[[miperfil_idioma_de_la_interfaz]]</label>
                    <asp:DropDownList ID="ddlIdioma" runat="server" CssClass="entrada" ClientIDMode="Static" />
                </div>
                <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primario" Text="[[comun_guardar_cambios]]" OnClick="btnGuardar_Click" CausesValidation="false" />
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phEmergencia" runat="server" Visible="false">
                <p class="texto-suave sin-margen">
                    [[miperfil_es_el_acceso_de_emergencia_no]]
                </p>
            </asp:PlaceHolder>
        </section>

        <section class="tarjeta">
            <div class="tarjeta-cabecera">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-candado" /></svg>
                <span>[[miperfil_acceso_y_seguridad]]</span>
            </div>
            <table class="datos-perfil">
                <tbody>
                    <tr><th scope="row">[[miperfil_rol]]</th><td><asp:Literal ID="litRolDetalle" runat="server" /></td></tr>
                    <tr><th scope="row">[[bitacora_empresa]]</th><td><asp:Literal ID="litEmpresaDetalle" runat="server" /></td></tr>
                    <tr><th scope="row">[[ingresar_contrasena]]</th>
                        <td>
                            <asp:PlaceHolder ID="phCambiarClave" runat="server">
                                <a class="btn btn-secundario btn-chico" href="<%: ResolveUrl("~/MiClave.aspx") %>">[[miclave_cambiar_mi_contrasena]]</a>
                                <p class="texto-chico texto-suave mt-8 sin-margen">[[miperfil_al_cambiarla_las_sesiones_abiertas_en]]</p>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phSinClave" runat="server" Visible="false">
                                <span class="texto-suave">[[miperfil_no_tiene_el_acceso_de_emergencia]]</span>
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
            <span>[[miperfil_mi_actividad_reciente]]</span>
        </div>
        <div class="tabla-scroll">
            <table class="tabla">
                <caption class="solo-lectores">[[miperfil_los_ultimos_movimientos_hechos_desde_tu]]</caption>
                <thead>
                    <tr>
                        <th scope="col">[[bitacora_fecha_y_hora]]</th>
                        <th scope="col">[[bitacora_modulo]]</th>
                        <th scope="col">[[bitacora_descripcion]]</th>
                        <th scope="col">[[bitacora_detalle]]</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptActividad" runat="server" EnableViewState="false">
                        <ItemTemplate>
                            <tr>
                                <td class="celda-fecha"><%#: ((DateTime)Eval("FechaHoraBitacora")).ToString("g", GUI.IdiomaActual_GUI.Cultura) %></td>
                                <td><span class="badge badge-neutro"><%#: GUI.TextoBitacora_GUI.Modulo(Eval("ModuloBitacora")) %></span></td>
                                <td><%#: GUI.TextoBitacora_GUI.Descripcion(Eval("DescripcionBitacora")) %></td>
                                <td class="texto-suave"><%#: GUI.TextoBitacora_GUI.Detalle(Eval("DetalleBitacora")) %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
        <asp:PlaceHolder ID="phSinActividad" runat="server" Visible="false">
            <p class="texto-chico texto-suave mt-8 sin-margen">[[miperfil_todavia_no_hay_movimientos_registrados_d]]</p>
        </asp:PlaceHolder>
    </section>

</asp:Content>
