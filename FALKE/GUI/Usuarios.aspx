<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Usuarios.aspx.cs" Inherits="GUI.Usuarios" Title="Usuarios" %>
<%@ MasterType VirtualPath="~/App.master" %>

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

        .ficha-empresa .datos
        {
            display: flex;
            flex-wrap: wrap;
            gap: 4px 18px;
            font-size: .84rem;
            color: rgba(252, 252, 247, .78);
            margin-top: 4px;
        }

        .ficha-empresa .derecha
        {
            margin-left: auto;
            display: flex;
            gap: 8px;
            align-items: center;
        }

        .filtros-usuarios
        {
            display: grid;
            grid-template-columns: minmax(200px, 2fr) repeat(auto-fit, minmax(150px, 1fr));
            gap: 14px;
            align-items: end;
        }

        .filtros-usuarios .campo
        {
            margin: 0;
        }

        .paginado
        {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
            margin-top: 14px;
            font-size: .88rem;
            color: var(--texto-suave);
        }

        .fila-deshabilitada > td:not(.celda-acciones)
        {
            opacity: .5;
        }

        .gestion-columnas
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 0 28px;
        }

        .gestion-columnas > .gestion-seccion + .gestion-seccion
        {
            padding-left: 28px;
            border-left: 1px solid var(--borde);
        }

        .gestion-seccion h3
        {
            font-size: 1rem;
            margin-bottom: 4px;
        }

        .gestion-actual
        {
            font-size: .86rem;
            color: var(--texto-suave);
            margin: 0 0 12px;
        }

        .gestion-seccion .campo
        {
            margin-bottom: 12px;
        }

        .gestion-seccion .aviso
        {
            padding: 10px 12px;
            margin-bottom: 12px;
            font-size: .82rem;
        }

        .gestion-seccion .aviso label
        {
            margin: 8px 0 0;
            font-size: .84rem;
        }

        @media (max-width: 720px)
        {
            .gestion-columnas > .gestion-seccion + .gestion-seccion
            {
                padding-left: 0;
                border-left: 0;
                border-top: 1px solid var(--borde);
                padding-top: 16px;
                margin-top: 16px;
            }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="usuarios.titulo">Usuarios</h1>
            <p class="texto-suave sin-margen">
                <asp:Literal ID="litBajada" runat="server" />
            </p>
        </div>
        <div class="acciones">
            <asp:PlaceHolder ID="phInvitar" runat="server">
                <button type="button" class="btn btn-primario" data-abre-modal="modalInvitar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                    <span data-i18n="usuarios.invitar">Invitar usuario</span>
                </button>
            </asp:PlaceHolder>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <asp:PlaceHolder ID="phFicha" runat="server">
        <section class="ficha-empresa" aria-label="Datos de tu empresa">
            <span class="sigla" aria-hidden="true"><asp:Literal ID="litSigla" runat="server" /></span>
            <div>
                <strong style="font-size:1.05rem"><asp:Literal ID="litEmpresaNombre" runat="server" /></strong>
                <div class="datos">
                    <span><asp:Literal ID="litEmpresaAlta" runat="server" /></span>
                    <span><asp:Literal ID="litEmpresaUsuarios" runat="server" /></span>
                </div>
            </div>
            <div class="derecha">
                <span class="badge badge-alerta"><asp:Literal ID="litEmpresaPlan" runat="server" /></span>
                <span class="badge badge-exito"><asp:Literal ID="litEmpresaEstado" runat="server" /></span>
            </div>
        </section>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phSoloLectura" runat="server">
        <div class="aviso aviso-info mb-24">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen">
                Puedes ver a los integrantes de tu empresa y sus roles, y editar tus propios datos.
                La gestión de las cuentas la hace el administrador.
            </p>
        </div>
    </asp:PlaceHolder>

    <div class="filtros filtros-usuarios">
        <div class="campo">
            <label for="txtBuscar">Buscar</label>
            <asp:TextBox ID="txtBuscar" runat="server" CssClass="entrada" placeholder="Nombre o correo" MaxLength="100" ClientIDMode="Static" />
        </div>
        <asp:PlaceHolder ID="phFiltroEmpresa" runat="server">
            <div class="campo">
                <label for="ddlEmpresaFiltro">Empresa</label>
                <asp:DropDownList ID="ddlEmpresaFiltro" runat="server" CssClass="entrada" ClientIDMode="Static" />
            </div>
        </asp:PlaceHolder>
        <div class="campo">
            <label for="ddlRolFiltro">Rol</label>
            <asp:DropDownList ID="ddlRolFiltro" runat="server" CssClass="entrada" ClientIDMode="Static" />
        </div>
        <div class="campo">
            <label for="ddlEstadoFiltro">Estado</label>
            <asp:DropDownList ID="ddlEstadoFiltro" runat="server" CssClass="entrada" ClientIDMode="Static" />
        </div>
        <div class="acciones-filtro">
            <asp:Button ID="btnBuscar" runat="server" CssClass="btn btn-secundario" Text="Buscar" OnClick="btnBuscar_Click" />
        </div>
    </div>

    <div class="tabla-scroll">
        <table class="tabla">
            <caption class="solo-lectores">Usuarios</caption>
            <thead>
                <tr>
                    <th scope="col">Usuario</th>
                    <asp:PlaceHolder ID="phColEmpresa" runat="server"><th scope="col">Empresa</th></asp:PlaceHolder>
                    <th scope="col">Rol</th>
                    <th scope="col">Estado</th>
                    <asp:PlaceHolder ID="phColAcciones" runat="server"><th scope="col"><span class="solo-lectores">Acciones</span></th></asp:PlaceHolder>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptUsuarios" runat="server" OnItemCommand="rptUsuarios_ItemCommand">
                    <ItemTemplate>
                        <tr class="<%#: ClaseFila(Container.DataItem) %>">
                            <td>
                                <div class="celda-doble">
                                    <span class="avatar" aria-hidden="true"><%#: Iniciales(Container.DataItem) %></span>
                                    <div>
                                        <div class="principal"><%#: Eval("NombreUsuario") %> <%#: Eval("ApellidoUsuario") %></div>
                                        <div class="secundario"><%#: Eval("EmailUsuario") %></div>
                                    </div>
                                </div>
                            </td>
                            <asp:PlaceHolder runat="server" Visible="<%# VeTodas %>"><td><%#: Eval("NombreEmpresa") %></td></asp:PlaceHolder>
                            <td><%#: EtiquetaRol((string)Eval("Rol")) %></td>
                            <td><span class="badge <%#: ClaseEstado((TE.EstadoUsuario)Eval("Estado")) %>"><%#: EtiquetaEstado((TE.EstadoUsuario)Eval("Estado")) %></span></td>
                            <asp:PlaceHolder runat="server" Visible="<%# PuedeGestionar || PuedeInvitar %>">
                                <td class="celda-acciones">
                                    <div class="acciones-fila">
                                        <asp:PlaceHolder runat="server" Visible="<%# EsOtro(Container.DataItem) && PuedeEditarDatos %>">
                                            <button type="button" class="btn btn-secundario btn-chico" data-abre-modal="modalDatos" data-editar-datos="1"
                                                    data-id="<%#: Eval("IdUsuario") %>"
                                                    data-cuenta="<%#: NombreCompleto(Container.DataItem) %> · <%#: Eval("EmailUsuario") %>"
                                                    data-n="<%#: Eval("NombreUsuario") %>"
                                                    data-a="<%#: Eval("ApellidoUsuario") %>"
                                                    data-email="<%#: Eval("EmailUsuario") %>"
                                                    data-idioma="<%#: Eval("IdIdioma") %>"
                                                    data-id-empresa="<%#: Eval("IdEmpresa") %>"
                                                    data-empresa="<%#: Eval("NombreEmpresa") %>"
                                                    data-otra-empresa="<%#: EsOtraEmpresa(Container.DataItem) ? "1" : "0" %>">
                                                Editar datos
                                            </button>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" Visible="<%# EsOtro(Container.DataItem) && PuedeGestionarCuenta %>">
                                            <button type="button" class="btn btn-secundario btn-chico" data-abre-modal="modalGestion" data-gestionar="1"
                                                    data-id="<%#: Eval("IdUsuario") %>"
                                                    data-nombre="<%#: NombreCompleto(Container.DataItem) %>"
                                                    data-email="<%#: Eval("EmailUsuario") %>"
                                                    data-empresa="<%#: Eval("NombreEmpresa") %>"
                                                    data-rol="<%#: Eval("Rol") %>"
                                                    data-rol-etiqueta="<%#: EtiquetaRol((string)Eval("Rol")) %>"
                                                    data-estado="<%#: (int)(TE.EstadoUsuario)Eval("Estado") %>"
                                                    data-estado-etiqueta="<%#: EtiquetaEstado((TE.EstadoUsuario)Eval("Estado")) %>"
                                                    data-otra-empresa="<%#: EsOtraEmpresa(Container.DataItem) ? "1" : "0" %>">
                                                Gestionar
                                            </button>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" Visible="<%# EsOtro(Container.DataItem) && PuedeInvitar && EsPendiente(Container.DataItem) && !InvitacionEnviada(Container.DataItem) %>">
                                            <asp:LinkButton runat="server" CssClass="btn btn-secundario btn-chico" CommandName="reenviar" CommandArgument='<%# Eval("IdUsuario") %>'
                                                            CausesValidation="false" ToolTip="Genera un enlace de activación nuevo y lo envía por correo"
                                                            OnClientClick="if (this.getAttribute('data-enviando')) return false; this.setAttribute('data-enviando', '1'); return true;">Reenviar invitación</asp:LinkButton>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" Visible="<%# EsOtro(Container.DataItem) && PuedeInvitar && EsPendiente(Container.DataItem) && InvitacionEnviada(Container.DataItem) %>">
                                            <span class="btn btn-secundario btn-chico" aria-disabled="true" title="La invitación ya se envió">
                                                <svg width="15" height="15" aria-hidden="true"><use href="#i-tilde" /></svg>
                                                Invitación enviada
                                            </span>
                                        </asp:PlaceHolder>
                                    </div>
                                </td>
                            </asp:PlaceHolder>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

        <asp:PlaceHolder ID="phVacio" runat="server" Visible="false">
            <div class="vacio">
                <svg width="28" height="28" aria-hidden="true"><use href="#i-usuarios" /></svg>
                <p class="sin-margen">No hay usuarios que coincidan con los filtros.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <div class="paginado">
        <span><asp:Literal ID="litPaginado" runat="server" /></span>
        <div class="fila">
            <asp:LinkButton ID="lnkAnterior" runat="server" CssClass="btn btn-fantasma btn-chico" OnClick="lnkAnterior_Click" CausesValidation="false">Anterior</asp:LinkButton>
            <asp:LinkButton ID="lnkSiguiente" runat="server" CssClass="btn btn-fantasma btn-chico" OnClick="lnkSiguiente_Click" CausesValidation="false">Siguiente</asp:LinkButton>
        </div>
    </div>

    <div class="modal-fondo" id="modalGestion" role="dialog" aria-modal="true" aria-labelledby="gestionTitulo" hidden>
        <div class="modal ancho">
            <div class="modal-cabecera">
                <div>
                    <h2 id="gestionTitulo">Gestionar usuario</h2>
                    <p class="subtitulo sin-margen"><span id="gestionNombre"></span> &middot; <span id="gestionEmail"></span></p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <asp:HiddenField ID="hfIdUsuario" runat="server" ClientIDMode="Static" />

                <div class="gestion-columnas">
                <asp:PlaceHolder ID="phGestionEstado" runat="server">
                    <section class="gestion-seccion">
                        <h3>Estado de la cuenta</h3>
                        <p class="gestion-actual">Estado actual: <strong id="gestionEstadoActual"></strong></p>
                        <div class="campo">
                            <label for="ddlNuevoEstado">Nuevo estado</label>
                            <asp:DropDownList ID="ddlNuevoEstado" runat="server" CssClass="entrada" ClientIDMode="Static" />
                            <p class="ayuda">Inactivo: dada de baja, no puede entrar ni recuperar la contraseña. Bloqueo estricto: no puede entrar ni pedir el cambio de contraseña. «Bloqueado» (por contraseña incorrecta) lo pone el sistema: la persona se desbloquea recuperando su contraseña.</p>
                        </div>
                        <div class="campo">
                            <label for="txtMotivoEstado">Motivo <span class="texto-tenue" data-solo-otra-empresa>(obligatorio)</span></label>
                            <asp:TextBox ID="txtMotivoEstado" runat="server" CssClass="entrada" MaxLength="300" ClientIDMode="Static" />
                        </div>
                        <asp:Button ID="btnCambiarEstado" runat="server" CssClass="btn btn-primario" Text="Cambiar estado" OnClick="btnCambiarEstado_Click" />
                    </section>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phGestionRol" runat="server">
                    <section class="gestion-seccion">
                        <h3>Rol</h3>
                        <p class="gestion-actual">Rol actual: <strong id="gestionRolActual"></strong></p>
                        <div class="campo">
                            <label for="ddlNuevoRol">Nuevo rol</label>
                            <asp:DropDownList ID="ddlNuevoRol" runat="server" CssClass="entrada" ClientIDMode="Static" />
                        </div>
                        <div class="aviso aviso-alerta" id="avisoRol" hidden>
                            <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                            <div>
                                <strong>Estás modificando una cuenta de un cliente.</strong>
                                <p class="sin-margen">Cambia los permisos de esa persona. Queda en la bitácora y los administradores de la empresa ven que lo hizo el equipo de Pattern Blue.</p>
                                <label class="casilla-simple mt-8">
                                    <asp:CheckBox ID="chkConfirmaRol" runat="server" ClientIDMode="Static" />
                                    <span>Entiendo el aviso y quiero continuar</span>
                                </label>
                            </div>
                        </div>
                        <div class="campo">
                            <label for="txtMotivoRol">Motivo <span class="texto-tenue" data-solo-otra-empresa>(obligatorio)</span></label>
                            <asp:TextBox ID="txtMotivoRol" runat="server" CssClass="entrada" MaxLength="300" ClientIDMode="Static" />
                        </div>
                        <asp:Button ID="btnCambiarRol" runat="server" CssClass="btn btn-primario" Text="Cambiar rol" OnClick="btnCambiarRol_Click" />
                    </section>
                </asp:PlaceHolder>
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cerrar</button>
            </div>
        </div>
    </div>

    <div class="modal-fondo" id="modalDatos" role="dialog" aria-modal="true" aria-labelledby="datosTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="datosTitulo">Editar datos del usuario</h2>
                    <p class="subtitulo sin-margen" id="datosCuenta"></p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <asp:HiddenField ID="hfIdDatos" runat="server" ClientIDMode="Static" />

                <div class="aviso aviso-alerta" id="avisoDatosCliente" hidden>
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <div>
                        <strong>Estás modificando una cuenta de un cliente.</strong>
                        <p class="sin-margen">Queda en la bitácora y los administradores de la empresa ven que lo hizo el equipo de Pattern Blue.</p>
                    </div>
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="txtDatosNombre">Nombre</label>
                        <asp:TextBox ID="txtDatosNombre" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" />
                    </div>
                    <div class="campo">
                        <label for="txtDatosApellido">Apellido</label>
                        <asp:TextBox ID="txtDatosApellido" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="campo">
                    <label for="ddlDatosIdioma">Idioma de la interfaz</label>
                    <asp:DropDownList ID="ddlDatosIdioma" runat="server" CssClass="entrada" ClientIDMode="Static">
                        <asp:ListItem Value="1">Español</asp:ListItem>
                        <asp:ListItem Value="2">Inglés</asp:ListItem>
                        <asp:ListItem Value="3">Portugués</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <asp:PlaceHolder ID="phDatosAvanzados" runat="server" Visible="false">
                    <div class="campo">
                        <label for="txtDatosEmail">Correo electrónico</label>
                        <asp:TextBox ID="txtDatosEmail" runat="server" CssClass="entrada" TextMode="Email" MaxLength="255" ClientIDMode="Static" />
                    </div>
                    <div class="campo">
                        <label for="ddlDatosEmpresa">Empresa</label>
                        <asp:DropDownList ID="ddlDatosEmpresa" runat="server" CssClass="entrada" ClientIDMode="Static" />
                    </div>
                    <div class="aviso aviso-alerta" id="avisoDatosSensibles" hidden>
                        <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                        <div>
                            <strong>Cambiar el correo o la empresa es delicado.</strong>
                            <p class="sin-margen">El correo es con lo que la persona entra, y la empresa define a qué datos accede. Si la cuenta sigue pendiente, se envía un enlace de activación nuevo al correo nuevo.</p>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <div class="campo">
                    <label for="txtDatosMotivo">Motivo <span class="texto-tenue" id="datosMotivoObligatorio" hidden>(obligatorio)</span></label>
                    <asp:TextBox ID="txtDatosMotivo" runat="server" CssClass="entrada" MaxLength="300" ClientIDMode="Static" />
                </div>
                <label class="casilla-simple" id="datosConfirma" hidden>
                    <asp:CheckBox ID="chkDatosConfirma" runat="server" ClientIDMode="Static" />
                    <span>Entiendo el aviso y quiero continuar</span>
                </label>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal>Cancelar</button>
                <asp:Button ID="btnGuardarDatos" runat="server" CssClass="btn btn-primario" Text="Guardar datos" OnClick="btnGuardarDatos_Click" />
            </div>
        </div>
    </div>

    <div class="modal-fondo" id="modalInvitar" role="dialog" aria-modal="true" aria-labelledby="invitarTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="invitarTitulo" data-i18n="invitar.titulo">Invitar a un usuario</h2>
                    <p class="subtitulo sin-margen" data-i18n="invitar.subtitulo">
                        Se le envía un correo con un enlace para que defina su contraseña.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">

                <asp:Panel ID="pnlInvitarAviso" runat="server" CssClass="aviso" Visible="false">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
                    <p class="sin-margen"><asp:Literal ID="litInvitarAviso" runat="server" /></p>
                </asp:Panel>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="invNombre" data-i18n="invitar.nombre">Nombre</label>
                        <asp:TextBox ID="invNombre" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" data-foco-inicial="si" />
                    </div>
                    <div class="campo">
                        <label for="invApellido" data-i18n="invitar.apellido">Apellido</label>
                        <asp:TextBox ID="invApellido" runat="server" CssClass="entrada" MaxLength="100" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="campo">
                    <label for="invEmail" data-i18n="invitar.email">Correo electrónico</label>
                    <asp:TextBox ID="invEmail" runat="server" CssClass="entrada" TextMode="Email" MaxLength="255" ClientIDMode="Static"
                                 placeholder="nombre@empresa.com" />
                    <p class="ayuda" data-i18n="invitar.email.ayuda">Ahí llega el enlace de activación.</p>
                </div>
                <div class="campo">
                    <label for="invRol" data-i18n="invitar.rol">Rol</label>
                    <asp:DropDownList ID="invRol" runat="server" CssClass="entrada" ClientIDMode="Static" />
                </div>
                <asp:PlaceHolder ID="phInvEmpresa" runat="server" Visible="false">
                    <div class="campo">
                        <label for="invEmpresa" data-i18n="invitar.empresa">Empresa</label>
                        <asp:DropDownList ID="invEmpresa" runat="server" CssClass="entrada" ClientIDMode="Static" />
                        <p class="ayuda" data-i18n="invitar.empresa.ayuda">Como gestor podés dar de alta usuarios en otra empresa.</p>
                    </div>
                    <div class="aviso aviso-alerta" id="avisoInvEmpresa" hidden>
                        <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
                        <p class="sin-margen"><strong>Cuidado:</strong> estás creando un usuario en la empresa <strong id="invEmpresaNombre"></strong>, que no es la tuya.</p>
                    </div>
                </asp:PlaceHolder>
                <div class="aviso aviso-info mb-0">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
                    <p class="sin-margen" data-i18n="invitar.aviso">
                        La cuenta queda en estado «Pendiente de activación» hasta que la persona
                        entre al enlace y defina su contraseña.
                    </p>
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <asp:Button ID="btnInvitar" runat="server" CssClass="btn btn-primario" OnClick="btnInvitar_Click"
                            Text="Enviar invitación" data-i18n="invitar.enviar" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            function poner(id, texto) {
                var el = document.getElementById(id);
                if (el) el.textContent = texto;
            }

            function actualizarAviso() {
                var modal = document.getElementById("modalGestion");
                var aviso = document.getElementById("avisoRol");
                var rol = document.getElementById("ddlNuevoRol");
                if (!modal || !aviso) return;

                var otra = modal.getAttribute("data-otra-empresa") === "1";
                var esGestor = rol && rol.selectedOptions.length > 0 && rol.selectedOptions[0].getAttribute("data-gestion") === "1";
                aviso.hidden = !(otra || esGestor);

                var marcas = modal.querySelectorAll("[data-solo-otra-empresa]");
                for (var i = 0; i < marcas.length; i++) marcas[i].hidden = !otra;
            }

            document.addEventListener("click", function (e) {
                var boton = e.target.closest ? e.target.closest("[data-gestionar]") : null;
                if (!boton) return;

                var modal = document.getElementById("modalGestion");
                modal.setAttribute("data-otra-empresa", boton.getAttribute("data-otra-empresa"));

                document.getElementById("hfIdUsuario").value = boton.getAttribute("data-id");
                poner("gestionNombre", boton.getAttribute("data-nombre"));
                poner("gestionEmail", boton.getAttribute("data-email"));
                poner("gestionEstadoActual", boton.getAttribute("data-estado-etiqueta"));
                poner("gestionRolActual", boton.getAttribute("data-rol-etiqueta"));

                var motivoE = document.getElementById("txtMotivoEstado");
                var motivoR = document.getElementById("txtMotivoRol");
                var confirma = document.getElementById("chkConfirmaRol");
                if (motivoE) motivoE.value = "";
                if (motivoR) motivoR.value = "";
                if (confirma) confirma.checked = false;

                actualizarAviso();
            });

            document.addEventListener("change", function (e) {
                if (e.target && e.target.id === "ddlNuevoRol") actualizarAviso();
                if (e.target && (e.target.id === "ddlDatosEmpresa" || e.target.id === "txtDatosEmail")) actualizarAvisoDatos();
                if (e.target && e.target.id === "invEmpresa") actualizarAvisoInvitacion();
            });

            document.addEventListener("input", function (e) {
                if (e.target && e.target.id === "txtDatosEmail") actualizarAvisoDatos();
            });

            // Invitar: el gestor ve un aviso mientras la empresa elegida no sea la suya.
            function actualizarAvisoInvitacion() {
                var lista = document.getElementById("invEmpresa");
                var aviso = document.getElementById("avisoInvEmpresa");
                if (!lista || !aviso) return;

                var otra = lista.value !== lista.getAttribute("data-propia");
                aviso.hidden = !otra;
                poner("invEmpresaNombre", lista.options[lista.selectedIndex] ? lista.options[lista.selectedIndex].text : "");
            }

            // Editar datos: avisos y confirmación según a quién se edita y qué se cambia.
            function actualizarAvisoDatos() {
                var modal = document.getElementById("modalDatos");
                if (!modal) return;

                var otra = modal.getAttribute("data-otra-empresa") === "1";
                var email = document.getElementById("txtDatosEmail");
                var empresa = document.getElementById("ddlDatosEmpresa");
                var sensible = (email && email.value.trim().toLowerCase() !== modal.getAttribute("data-email-original")) ||
                               (empresa && empresa.value !== modal.getAttribute("data-empresa-original"));

                var avisoCliente = document.getElementById("avisoDatosCliente");
                var avisoSensible = document.getElementById("avisoDatosSensibles");
                if (avisoCliente) avisoCliente.hidden = !otra;
                if (avisoSensible) avisoSensible.hidden = !sensible;

                document.getElementById("datosConfirma").hidden = !(otra || sensible);
                document.getElementById("datosMotivoObligatorio").hidden = !(otra || sensible);
            }

            document.addEventListener("click", function (e) {
                var boton = e.target.closest ? e.target.closest("[data-editar-datos]") : null;
                if (!boton) return;

                var modal = document.getElementById("modalDatos");
                modal.setAttribute("data-otra-empresa", boton.getAttribute("data-otra-empresa"));
                modal.setAttribute("data-email-original", (boton.getAttribute("data-email") || "").toLowerCase());
                modal.setAttribute("data-empresa-original", boton.getAttribute("data-id-empresa"));

                document.getElementById("hfIdDatos").value = boton.getAttribute("data-id");
                poner("datosCuenta", boton.getAttribute("data-cuenta"));
                document.getElementById("txtDatosNombre").value = boton.getAttribute("data-n");
                document.getElementById("txtDatosApellido").value = boton.getAttribute("data-a");
                document.getElementById("ddlDatosIdioma").value = boton.getAttribute("data-idioma");

                var email = document.getElementById("txtDatosEmail");
                var empresa = document.getElementById("ddlDatosEmpresa");
                if (email) email.value = boton.getAttribute("data-email");
                if (empresa) empresa.value = boton.getAttribute("data-id-empresa");

                document.getElementById("txtDatosMotivo").value = "";
                document.getElementById("chkDatosConfirma").checked = false;

                actualizarAvisoDatos();
            });

            actualizarAvisoInvitacion();
        })();
    </script>
</asp:Content>
