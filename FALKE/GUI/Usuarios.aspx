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

        .ficha-empresa .datos span
        {
            display: inline-flex;
            align-items: center;
            gap: 5px;
        }

        .ficha-empresa .derecha
        {
            margin-left: auto;
            display: flex;
            gap: 8px;
            align-items: center;
        }

        .um
        {
            display: grid;
            grid-template-columns: 320px minmax(0, 1fr);
            gap: 20px;
            align-items: start;
        }

        .um-panel
        {
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
            overflow: hidden;
        }

        .um-lista-cab
        {
            padding: 14px 14px 10px;
            border-bottom: 1px solid var(--borde);
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .um-buscador
        {
            position: relative;
        }

        .um-lista
        {
            max-height: 560px;
            overflow-y: auto;
        }

        .um-fila
        {
            display: flex;
            align-items: center;
            gap: 11px;
            width: 100%;
            text-align: left;
            padding: 12px 14px;
            border: 0;
            border-bottom: 1px solid var(--borde);
            background: transparent;
            color: var(--texto);
            font-family: inherit;
            cursor: pointer;
        }

        .um-fila:hover
        {
            background: var(--superficie-2);
        }

        .um-fila[aria-current="true"]
        {
            background: var(--acento-suave);
            outline: 1px solid var(--acento);
            outline-offset: -1px;
        }

        .um-fila .avatar
        {
            flex: none;
        }

        .um-fila .info
        {
            min-width: 0;
            flex: 1;
        }

        .um-fila .nombre
        {
            display: block;
            font-weight: 600;
            font-size: .9rem;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .um-fila .sub
        {
            display: block;
            font-size: .78rem;
            color: var(--texto-suave);
        }

        .punto-linea
        {
            flex: none;
            width: 8px;
            height: 8px;
            border-radius: 50%;
            background: var(--marca-coral);
        }

        .en-linea
        {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-family: var(--fuente-mono);
            font-size: .66rem;
            letter-spacing: .08em;
            text-transform: uppercase;
            color: var(--marca-coral);
        }

        .um-detalle
        {
            padding: 22px 24px 24px;
        }

        .um-detalle-cab
        {
            display: flex;
            align-items: center;
            gap: 14px;
            margin-bottom: 20px;
        }

        .um-detalle-cab h2
        {
            margin: 0 0 4px;
            font-size: 1.2rem;
        }

        .um-datos
        {
            width: 100%;
            border-collapse: collapse;
        }

        .um-datos th,
        .um-datos td
        {
            text-align: left;
            padding: 10px 0;
            border-bottom: 1px solid var(--borde);
            font-size: .9rem;
            vertical-align: top;
        }

        .um-datos th
        {
            width: 190px;
            color: var(--texto-suave);
            font-weight: 600;
        }

        .um-acciones
        {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 20px;
        }

        .um-form .fila-campos
        {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 14px;
        }

        @media (max-width: 900px)
        {
            .um { grid-template-columns: minmax(0, 1fr); }
            .um-lista { max-height: 320px; }
            .um-form .fila-campos { grid-template-columns: minmax(0, 1fr); }
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

    <section class="ficha-empresa" aria-label="Datos de tu empresa">
        <span class="sigla" aria-hidden="true">IR</span>
        <div>
            <strong style="font-size:1.05rem">Ironhide Game Studio</strong>
            <div class="datos">
                <span><svg width="13" height="13" aria-hidden="true"><use href="#i-empresa" /></svg>Videojuegos</span>
                <span><svg width="13" height="13" aria-hidden="true"><use href="#i-dispositivo" /></svg>1 dispositivo en préstamo</span>
                <span><svg width="13" height="13" aria-hidden="true"><use href="#i-usuarios" /></svg>6 usuarios</span>
                <span><svg width="13" height="13" aria-hidden="true"><use href="#i-calendario" /></svg>Cliente desde jun 2026</span>
            </div>
        </div>
        <div class="derecha">
            <span class="badge badge-alerta">Plan Hunter</span>
            <span class="badge badge-exito" data-i18n="estado.activa">Activa</span>
        </div>
    </section>

    <asp:PlaceHolder ID="phSoloLectura" runat="server">
        <div class="aviso aviso-info mb-24">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen" data-i18n="usuarios.soloLectura">
                Como analista puedes ver a los integrantes de tu empresa y sus roles, y editar
                tus propios datos. La gestión de las cuentas la hace el administrador.
            </p>
        </div>
    </asp:PlaceHolder>

    <div class="um" id="um" data-rol="<%: Master.RolActual %>">

        <div class="um-panel">
            <div class="um-lista-cab">
                <div class="um-buscador campo-buscar">
                    <label class="solo-lectores" for="usBuscar" data-i18n="usuarios.buscar">Buscar</label>
                    <svg aria-hidden="true"><use href="#i-buscar" /></svg>
                    <input type="search" class="entrada" id="usBuscar"
                           placeholder="Nombre o correo..." data-i18n-attr="placeholder:usuarios.buscar.placeholder" />
                </div>
                <div class="chips" data-unico role="group" aria-label="Filtrar por rol">
                    <button type="button" class="chip" aria-pressed="true" data-rol-filtro="" data-i18n="comun.todos">Todos</button>
                    <button type="button" class="chip" aria-pressed="false" data-rol-filtro="Administrador">Administradores</button>
                    <button type="button" class="chip" aria-pressed="false" data-rol-filtro="Analista">Analistas</button>
                </div>
            </div>

            <div class="um-lista" id="usLista" role="listbox" aria-label="Usuarios de la empresa">

                <button type="button" class="um-fila" role="option" aria-current="true" data-yo="1"
                        data-nombre="Julieta" data-apellido="Fernández" data-email="julieta.fernandez@ironhide.com"
                        data-rol="Administrador" data-estado="Activo" data-idioma="Español" data-alta="18 jun 2026"
                        data-acceso="Hoy 14:32" data-enlinea="1" data-sesiones="21" data-reportes="8" data-fallidos="0">
                    <span class="avatar" aria-hidden="true">JF</span>
                    <span class="info">
                        <span class="nombre">Julieta Fernández</span>
                        <span class="sub" data-i18n="usuarios.vos">Tú &middot; Administrador</span>
                    </span>
                    <span class="punto-linea" title="En línea" aria-hidden="true"></span>
                </button>

                <button type="button" class="um-fila" role="option"
                        data-nombre="María" data-apellido="Gómez" data-email="maria.gomez@ironhide.com"
                        data-rol="Analista" data-estado="Activo" data-idioma="Español" data-alta="18 jun 2026"
                        data-acceso="Hoy 13:54" data-enlinea="1" data-sesiones="12" data-reportes="4" data-fallidos="0">
                    <span class="avatar" aria-hidden="true">MG</span>
                    <span class="info">
                        <span class="nombre">María Gómez</span>
                        <span class="sub">Analista</span>
                    </span>
                    <span class="punto-linea" title="En línea" aria-hidden="true"></span>
                </button>

                <button type="button" class="um-fila" role="option"
                        data-nombre="Diego" data-apellido="Paz" data-email="diego.paz@ironhide.com"
                        data-rol="Analista" data-estado="Pendiente" data-idioma="Español" data-alta="9 sep 2026"
                        data-acceso="&mdash;" data-enlinea="0" data-sesiones="0" data-reportes="0" data-fallidos="0">
                    <span class="avatar" aria-hidden="true">DP</span>
                    <span class="info">
                        <span class="nombre">Diego Paz</span>
                        <span class="sub" data-i18n="estado.pendiente">Pendiente de activación</span>
                    </span>
                </button>

                <button type="button" class="um-fila" role="option"
                        data-nombre="Sofía" data-apellido="López" data-email="sofia.lopez@ironhide.com"
                        data-rol="Analista" data-estado="Bloqueado" data-idioma="Español" data-alta="4 mar 2026"
                        data-acceso="12 jul 2026" data-enlinea="0" data-sesiones="31" data-reportes="9" data-fallidos="5">
                    <span class="avatar" aria-hidden="true">SL</span>
                    <span class="info">
                        <span class="nombre">Sofía López</span>
                        <span class="sub" data-i18n="estado.bloqueado">Bloqueado</span>
                    </span>
                </button>

                <button type="button" class="um-fila" role="option"
                        data-nombre="Nicolás" data-apellido="Castro" data-email="nicolas.castro@ironhide.com"
                        data-rol="Analista" data-estado="Activo" data-idioma="Inglés" data-alta="2 feb 2026"
                        data-acceso="Ayer 18:10" data-enlinea="0" data-sesiones="27" data-reportes="6" data-fallidos="0">
                    <span class="avatar" aria-hidden="true">NC</span>
                    <span class="info">
                        <span class="nombre">Nicolás Castro</span>
                        <span class="sub">Analista</span>
                    </span>
                </button>

                <button type="button" class="um-fila" role="option"
                        data-nombre="Valentina" data-apellido="Ruiz" data-email="valentina.ruiz@ironhide.com"
                        data-rol="Analista" data-estado="Inactivo" data-idioma="Portugués" data-alta="20 nov 2025"
                        data-acceso="28 may 2026" data-enlinea="0" data-sesiones="14" data-reportes="3" data-fallidos="0">
                    <span class="avatar" aria-hidden="true">VR</span>
                    <span class="info">
                        <span class="nombre">Valentina Ruiz</span>
                        <span class="sub" data-i18n="estado.inactivo">Inactivo</span>
                    </span>
                </button>

            </div>

            <div class="vacio" id="usVacio" hidden>
                <svg width="28" height="28" aria-hidden="true"><use href="#i-usuarios" /></svg>
                <p class="sin-margen" data-i18n="usuarios.vacio">No hay usuarios que coincidan.</p>
            </div>
        </div>

        <div class="um-panel">
            <div class="um-detalle" id="usDetalle"></div>
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
                        <asp:TextBox ID="invNombre" runat="server" CssClass="entrada" ClientIDMode="Static" data-foco-inicial="si" />
                    </div>
                    <div class="campo">
                        <label for="invApellido" data-i18n="invitar.apellido">Apellido</label>
                        <asp:TextBox ID="invApellido" runat="server" CssClass="entrada" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="campo">
                    <label for="invEmail" data-i18n="invitar.email">Correo electrónico</label>
                    <asp:TextBox ID="invEmail" runat="server" CssClass="entrada" TextMode="Email" ClientIDMode="Static"
                                 placeholder="nombre@ironhide.com" />
                    <p class="ayuda" data-i18n="invitar.email.ayuda">Ahí llega el enlace de activación.</p>
                </div>
                <div class="campo">
                    <label for="invRol" data-i18n="invitar.rol">Rol</label>
                    <asp:DropDownList ID="invRol" runat="server" CssClass="entrada" ClientIDMode="Static">
                        <asp:ListItem Value="Usuario">Analista</asp:ListItem>
                        <asp:ListItem Value="Administrador">Administrador</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:PlaceHolder ID="phInvEmpresa" runat="server" Visible="false">
                    <div class="campo">
                        <label for="invEmpresa" data-i18n="invitar.empresa">Id de empresa</label>
                        <asp:TextBox ID="invEmpresa" runat="server" CssClass="entrada" ClientIDMode="Static" />
                        <p class="ayuda" data-i18n="invitar.empresa.ayuda">Como gestor podés dar de alta usuarios en otra empresa.</p>
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

    <script type="application/json" id="usTextos">
    {
        "rol": "Rol",
        "estado": "Estado",
        "idioma": "Idioma de la interfaz",
        "alta": "Fecha de alta",
        "acceso": "Último acceso",
        "email": "Correo electrónico",
        "sesiones": "Sesiones grabadas",
        "reportes": "Reportes exportados",
        "fallidos": "Intentos fallidos",
        "guardar": "Guardar cambios",
        "cambiarClave": "Cambiar mi contraseña",
        "emailFijo": "El correo no se puede modificar: es tu identidad de acceso.",
        "bloquear": "Bloquear cuenta",
        "desbloquear": "Desbloquear cuenta",
        "reactivar": "Reactivar cuenta",
        "reenviar": "Reenviar invitación",
        "enLinea": "En línea ahora",
        "tuCuenta": "Esta es tu cuenta"
    }
    </script>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var um = document.getElementById("um");
            var rolPagina = um.getAttribute("data-rol") || "Gestor";
            var esAnalista = rolPagina === "Analista";
            var T = JSON.parse(document.getElementById("usTextos").textContent);

            var filas = document.querySelectorAll(".um-fila");
            var buscar = document.getElementById("usBuscar");
            var chips = document.querySelectorAll(".chips [data-rol-filtro]");
            var vacio = document.getElementById("usVacio");
            var detalle = document.getElementById("usDetalle");
            var rolFiltro = "";

            function badgeEstado(estado) {
                var mapa = {
                    "Activo": ["badge-exito", "Activo"],
                    "Pendiente": ["badge-alerta", "Pendiente de activación"],
                    "Bloqueado": ["badge-peligro", "Bloqueado"],
                    "Inactivo": ["badge-neutro", "Inactivo"]
                };
                var d = mapa[estado] || ["badge-neutro", estado];
                return '<span class="badge ' + d[0] + '">' + d[1] + '</span>';
            }

            function iniciales(n, a) {
                return (n.charAt(0) + a.charAt(0)).toUpperCase();
            }

            function fichaLectura(f, completo) {
                var d = f.dataset;
                var filasTabla =
                    '<tr><th>' + T.rol + '</th><td>' + d.rol + '</td></tr>' +
                    '<tr><th>' + T.estado + '</th><td>' + badgeEstado(d.estado) + '</td></tr>' +
                    '<tr><th>' + T.idioma + '</th><td>' + d.idioma + '</td></tr>' +
                    '<tr><th>' + T.alta + '</th><td>' + d.alta + '</td></tr>';

                if (completo) {
                    filasTabla =
                        '<tr><th>' + T.email + '</th><td>' + d.email + '</td></tr>' +
                        filasTabla +
                        '<tr><th>' + T.acceso + '</th><td>' + d.acceso + '</td></tr>' +
                        '<tr><th>' + T.sesiones + '</th><td>' + d.sesiones + '</td></tr>' +
                        '<tr><th>' + T.reportes + '</th><td>' + d.reportes + '</td></tr>' +
                        '<tr><th>' + T.fallidos + '</th><td>' + d.fallidos + '</td></tr>';
                }

                var enlinea = d.enlinea === "1"
                    ? '<span class="en-linea"><span class="punto-linea" aria-hidden="true"></span>' + T.enLinea + '</span>'
                    : '';

                var acciones = "";
                if (completo && !esAnalista) {
                    var boton;
                    if (d.estado === "Bloqueado") boton = '<button type="button" class="btn btn-primario">' + T.desbloquear + '</button>';
                    else if (d.estado === "Inactivo") boton = '<button type="button" class="btn btn-primario">' + T.reactivar + '</button>';
                    else if (d.estado === "Pendiente") boton = '<button type="button" class="btn btn-secundario">' + T.reenviar + '</button>';
                    else boton = '<button type="button" class="btn btn-peligro">' + T.bloquear + '</button>';
                    acciones = '<div class="um-acciones">' + boton + '</div>';
                }

                return '' +
                    '<div class="um-detalle-cab">' +
                        '<span class="avatar grande" aria-hidden="true">' + iniciales(d.nombre, d.apellido) + '</span>' +
                        '<div><h2>' + d.nombre + ' ' + d.apellido + '</h2>' +
                        '<span class="badge badge-neutro">' + d.rol + '</span> ' + enlinea + '</div>' +
                    '</div>' +
                    '<table class="um-datos"><tbody>' + filasTabla + '</tbody></table>' +
                    acciones;
            }

            function opcionIdioma(valor, actual) {
                return '<option' + (valor === actual ? ' selected' : '') + '>' + valor + '</option>';
            }

            function fichaPropia(f) {
                var d = f.dataset;
                return '' +
                    '<div class="um-detalle-cab">' +
                        '<span class="avatar grande" aria-hidden="true">' + iniciales(d.nombre, d.apellido) + '</span>' +
                        '<div><h2>' + d.nombre + ' ' + d.apellido + '</h2>' +
                        '<span class="badge badge-info">' + T.tuCuenta + '</span></div>' +
                    '</div>' +
                    '<form class="um-form" onsubmit="return false">' +
                        '<div class="fila-campos">' +
                            '<div class="campo"><label for="pfNombre">Nombre</label>' +
                                '<input type="text" class="entrada" id="pfNombre" value="' + d.nombre + '" /></div>' +
                            '<div class="campo"><label for="pfApellido">Apellido</label>' +
                                '<input type="text" class="entrada" id="pfApellido" value="' + d.apellido + '" /></div>' +
                        '</div>' +
                        '<div class="campo"><label for="pfEmail">' + T.email + '</label>' +
                            '<input type="email" class="entrada" id="pfEmail" value="' + d.email + '" readonly />' +
                            '<p class="ayuda">' + T.emailFijo + '</p></div>' +
                        '<div class="campo"><label for="pfIdioma">' + T.idioma + '</label>' +
                            '<select class="entrada" id="pfIdioma">' +
                                opcionIdioma("Español", d.idioma) + opcionIdioma("Inglés", d.idioma) + opcionIdioma("Portugués", d.idioma) +
                            '</select></div>' +
                        '<div class="um-acciones">' +
                            '<button type="submit" class="btn btn-primario">' + T.guardar + '</button>' +
                            '<a class="btn btn-secundario" href="MiClave.aspx">' + T.cambiarClave + '</a>' +
                        '</div>' +
                    '</form>';
            }

            function mostrar(f) {
                for (var i = 0; i < filas.length; i++) filas[i].removeAttribute("aria-current");
                f.setAttribute("aria-current", "true");

                if (f.dataset.yo === "1") detalle.innerHTML = fichaPropia(f);
                else detalle.innerHTML = fichaLectura(f, !esAnalista);
            }

            for (var i = 0; i < filas.length; i++) {
                filas[i].addEventListener("click", function () { mostrar(this); });
            }

            function filtrar() {
                var texto = buscar.value.trim().toLowerCase();
                var visibles = 0;
                for (var j = 0; j < filas.length; j++) {
                    var f = filas[j];
                    var busca = (f.dataset.nombre + " " + f.dataset.apellido + " " + f.dataset.email).toLowerCase();
                    var okTexto = texto === "" || busca.indexOf(texto) !== -1;
                    var okRol = rolFiltro === "" || f.dataset.rol === rolFiltro;
                    var ver = okTexto && okRol;
                    f.hidden = !ver;
                    if (ver) visibles++;
                }
                vacio.hidden = visibles > 0;
            }

            buscar.addEventListener("input", filtrar);

            for (i = 0; i < chips.length; i++) {
                chips[i].addEventListener("click", function () {
                    for (var k = 0; k < chips.length; k++) {
                        chips[k].setAttribute("aria-pressed", chips[k] === this ? "true" : "false");
                    }
                    rolFiltro = this.getAttribute("data-rol-filtro");
                    filtrar();
                });
            }

            var abrirPerfil = window.location.search.indexOf("perfil=1") !== -1;
            var inicial = abrirPerfil ? document.querySelector('.um-fila[data-yo="1"]') : filas[0];
            mostrar(inicial || filas[0]);
        })();
    </script>
</asp:Content>
