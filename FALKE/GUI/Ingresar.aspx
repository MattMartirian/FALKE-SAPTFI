<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Ingresar.aspx.cs" Inherits="GUI.Ingresar" %>
<%@ Register Src="~/Controles/Iconos.ascx" TagPrefix="falke" TagName="Iconos" %>
<!DOCTYPE html>
<html lang="<%: GUI.IdiomaActual_GUI.Codigo %>">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>[[ingresar_iniciar_sesion_falke]]</title>

    <script>
        (function () {
            try {
                var t = localStorage.getItem("falke.tema");
                if (!t) { t = window.matchMedia("(prefers-color-scheme: dark)").matches ? "noche" : "dia"; }
                document.documentElement.setAttribute("data-tema", t);
            } catch (e) { document.documentElement.setAttribute("data-tema", "dia"); }
        })();
    </script>

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@500;700&amp;family=Manrope:wght@400;500;600;700&amp;family=Space+Mono:wght@400;700&amp;display=swap" />

    <link rel="stylesheet" href="<%: ResolveUrl("~/Content/falke.css") %>?v=<%: System.IO.File.GetLastWriteTimeUtc(Server.MapPath("~/Content/falke.css")).Ticks %>" />

    <style>
        .login-pagina
        {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 32px 20px;
            background: var(--fondo);
        }

        .ingreso-caja
        {
            display: grid;
            grid-template-columns: 1fr 1fr;
            width: 100%;
            max-width: 940px;
            background: var(--superficie);
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            box-shadow: var(--sombra);
            overflow: hidden;
        }

        .ingreso-formulario
        {
            padding: 40px 44px 38px;
        }

        .ingreso-volver
        {
            display: inline-flex;
            align-items: center;
            gap: 7px;
            margin-bottom: 22px;
            font-size: .86rem;
            font-weight: 600;
            color: var(--texto-suave);
        }

        .ingreso-volver svg
        {
            width: 16px;
            height: 16px;
        }

        .ingreso-formulario h1
        {
            font-size: 1.7rem;
            margin-bottom: 6px;
        }

        .ingreso-formulario .bajada
        {
            color: var(--texto-suave);
            font-size: .92rem;
            margin-bottom: 26px;
        }

        .linea-opciones
        {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            flex-wrap: wrap;
            margin-bottom: 22px;
        }

        .login-nota
        {
            display: flex;
            gap: 8px;
            align-items: flex-start;
            margin-top: 16px;
            font-size: .8rem;
            color: var(--texto-suave);
        }

        .login-nota .punto
        {
            flex: none;
            width: 7px;
            height: 7px;
            margin-top: 6px;
            border-radius: 50%;
            background: var(--marca-coral);
        }

        .ingreso-pie
        {
            margin-top: 26px;
            padding-top: 20px;
            border-top: 1px solid var(--borde);
            font-size: .87rem;
            color: var(--texto-suave);
        }

        .ingreso-marca
        {
            position: relative;
            background: #0B1428;
            color: var(--marca-perla);
            padding: 40px 40px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            overflow: hidden;
        }

        .ingreso-marca > *
        {
            position: relative;
            z-index: 1;
        }

        .ingreso-marca > .marca-iso
        {
            position: absolute;
            top: 0;
            left: 40px;
            right: 40px;
            height: 33.33%;
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 1;
        }

        .marca-iso img
        {
            height: 62px;
            width: auto;
        }

        .ingreso-marca h2
        {
            color: var(--marca-perla);
            font-size: 1.4rem;
        }

        .ingreso-marca p
        {
            color: rgba(252, 252, 247, .8);
            font-size: .92rem;
        }

        .ingreso-marca ul
        {
            list-style: none;
            margin: 22px 0 0;
            padding: 0;
            display: grid;
            gap: 12px;
        }

        .ingreso-marca li
        {
            display: flex;
            gap: 10px;
            align-items: flex-start;
            font-size: .9rem;
            color: rgba(252, 252, 247, .88);
        }

        .ingreso-marca li svg
        {
            flex: none;
            margin-top: 2px;
            color: var(--marca-dorado);
        }

        .mensaje-login:empty { display: none; }

        @media (max-width: 820px)
        {
            .ingreso-caja { grid-template-columns: 1fr; }
            .ingreso-marca { order: -1; padding: 30px 32px; }
            .ingreso-formulario { padding: 30px 26px 28px; }
        }

        @media (max-width: 460px)
        {
            .login-pagina { padding: 0; align-items: stretch; }
            .ingreso-caja { border: 0; box-shadow: none; border-radius: 0; background: transparent; }
            .ingreso-marca { border-radius: 0; }
            .ingreso-formulario { padding: 26px 20px 28px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <falke:Iconos runat="server" ID="ucIconos" />

        <main class="login-pagina">
            <div class="ingreso-caja">

                <div class="ingreso-formulario">

                    <a class="ingreso-volver" href="<%: ResolveUrl("~/Default.aspx") %>">
                        <svg aria-hidden="true"><use href="#i-atras" /></svg>
                        <span data-i18n="ingresar.volver">[[ingresar_volver_al_inicio]]</span>
                    </a>

                    <h1 data-i18n="ingresar.titulo">[[ingresar_iniciar_sesion]]</h1>
                    <p class="bajada" data-i18n="ingresar.bajada">
                        [[ingresar_entra_con_el_correo_con_el]]
                    </p>

                    <asp:Panel ID="pnlMensaje" runat="server" CssClass="aviso aviso-peligro mensaje-login" role="alert" Visible="false">
                        <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                        <p class="sin-margen"><asp:Literal ID="litMensaje" runat="server" /></p>
                    </asp:Panel>

                    <div class="campo">
                        <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" data-i18n="ingresar.email">[[empresanueva_correo_electronico]]</asp:Label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="entrada"
                                     autocomplete="username" placeholder="tu.nombre@empresa.com"
                                     data-i18n-attr="placeholder:ingresar.email.placeholder" />
                    </div>

                    <div class="campo">
                        <asp:Label ID="lblPass" runat="server" AssociatedControlID="txtPass" data-i18n="ingresar.contrasena">[[ingresar_contrasena]]</asp:Label>
                        <div class="campo-con-boton">
                            <asp:TextBox ID="txtPass" runat="server" CssClass="entrada" TextMode="Password"
                                         autocomplete="current-password" />
                            <button type="button" class="boton-dentro" data-ver-clave="<%= txtPass.ClientID %>"
                                    aria-pressed="false" title="[[ingresar_mostrar_la_contrasena]]">
                                <span class="solo-lectores" data-i18n="ingresar.verContrasena">[[ingresar_mostrar_u_ocultar_la_contrasena]]</span>
                                <svg width="19" height="19" aria-hidden="true" data-icono-clave="ver"><use href="#i-ojo" /></svg>
                                <svg width="19" height="19" aria-hidden="true" data-icono-clave="ocultar" style="display:none"><use href="#i-ojo-tachado" /></svg>
                            </button>
                        </div>
                    </div>

                    <div class="linea-opciones">
                        <label class="casilla-simple">
                            <asp:CheckBox ID="chkRecordarme" runat="server" />
                            <span data-i18n="ingresar.recordarme">[[ingresar_recordarme_en_este_equipo]]</span>
                        </label>

                        <a href="<%: ResolveUrl("~/RecuperarClave.aspx") %>" data-i18n="ingresar.olvide">[[ingresar_olvide_mi_contrasena]]</a>
                    </div>

                    <asp:Button ID="btnIngresar" runat="server" CssClass="btn btn-primario btn-grande btn-bloque"
                                Text="[[ingresar_ingresar]]" OnClick="btnIngresar_Click" />

                    <p class="login-nota" data-i18n="ingresar.bloqueo">
                        <span class="punto" aria-hidden="true"></span>
                        [[ingresar_despues_de_cinco_intentos_fallidos_la]]
                    </p>

                    <div class="ingreso-pie">
                        <span data-i18n="ingresar.sinCuenta">[[ingresar_todavia_no_eres_cliente]]</span>
                        <a href="<%: ResolveUrl("~/Registro.aspx") %>" data-i18n="ingresar.comoObtener">[[ingresar_asi_se_obtiene_una_cuenta]]</a>
                    </div>

                </div>

                <aside class="ingreso-marca">
                    <div class="marca-iso">
                        <img src="<%: ResolveUrl("~/Content/img/falke-logo.svg") %>" alt="Falke" />
                    </div>

                    <div class="marca-texto">
                        <h2 data-i18n="ingresar.marca.titulo">[[ingresar_descubre_lo_que_tus_usuarios_miran]]</h2>
                        <p data-i18n="ingresar.marca.texto">
                            [[ingresar_analitica_conductual_biometrica_por_segu]]
                        </p>

                        <ul>
                            <li>
                                <svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg>
                                <span data-i18n="ingresar.marca.item1">[[ingresar_seis_visualizaciones_por_sesion_listas_e]]</span>
                            </li>
                            <li>
                                <svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg>
                                <span data-i18n="ingresar.marca.item2">[[ingresar_tus_propios_testers_sobre_tu_propio]]</span>
                            </li>
                            <li>
                                <svg width="18" height="18" aria-hidden="true"><use href="#i-tilde" /></svg>
                                <span data-i18n="ingresar.marca.item3">[[ingresar_datos_biometricos_disociados_y_cifrados]]</span>
                            </li>
                        </ul>
                    </div>
                </aside>

            </div>
        </main>

        <script>window.FalkeTextos = { modoDia: "[[js:falke_js_modo_dia]]", modoNoche: "[[js:falke_js_modo_noche]]", expandirMenu: "[[js:falke_js_expandir_menu]]", plegarMenu: "[[js:app_plegar_el_menu]]", ocultarClave: "[[js:falke_js_ocultar_contrasena]]", mostrarClave: "[[js:ingresar_mostrar_la_contrasena]]" };</script>
        <script src="<%: ResolveUrl("~/Scripts/falke.js") %>?v=<%: System.IO.File.GetLastWriteTimeUtc(Server.MapPath("~/Scripts/falke.js")).Ticks %>"></script>
    </form>
</body>
</html>
