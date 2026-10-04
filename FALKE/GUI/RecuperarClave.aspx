<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RecuperarClave.aspx.cs" Inherits="GUI.RecuperarClave" %>
<%@ Register Src="~/Controles/Iconos.ascx" TagPrefix="falke" TagName="Iconos" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Recuperar contraseña - Falke</title>

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
        .recuperar-pagina
        {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 32px 20px;
            background: var(--fondo);
        }

        .recuperar-caja
        {
            width: 100%;
            max-width: 520px;
            background: var(--superficie);
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            box-shadow: var(--sombra);
            padding: 40px 40px 34px;
        }

        .recuperar-volver
        {
            display: inline-flex;
            align-items: center;
            gap: 7px;
            margin-bottom: 22px;
            font-size: .86rem;
            font-weight: 600;
            color: var(--texto-suave);
        }

        .recuperar-volver svg
        {
            width: 16px;
            height: 16px;
        }

        .recuperar-caja h1
        {
            font-size: 1.55rem;
            margin-bottom: 6px;
        }

        .recuperar-caja .bajada
        {
            color: var(--texto-suave);
            margin-bottom: 26px;
        }

        .recuperar-icono
        {
            color: var(--acento-texto);
            margin-bottom: 12px;
        }

        .recuperar-pie
        {
            margin-top: 26px;
            padding-top: 20px;
            border-top: 1px solid var(--borde);
            text-align: center;
            font-size: .9rem;
        }

        @media (max-width: 460px)
        {
            .recuperar-pagina { padding: 0; align-items: stretch; }
            .recuperar-caja { border: 0; box-shadow: none; border-radius: 0; padding: 30px 22px 26px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <falke:Iconos runat="server" ID="ucIconos" />

        <main class="recuperar-pagina">
            <div class="recuperar-caja">

                <a class="recuperar-volver" href="<%: ResolveUrl("~/Default.aspx") %>">
                    <svg aria-hidden="true"><use href="#i-atras" /></svg>
                    <span data-i18n="recuperar.volverInicio">Volver al inicio</span>
                </a>

                <div class="recuperar-icono" aria-hidden="true">
                    <svg width="26" height="26"><use href="#i-mail" /></svg>
                </div>

                <h1 data-i18n="recuperar.titulo">Recuperar mi contraseña</h1>
                <p class="bajada" data-i18n="recuperar.bajada">
                    Escribí el correo con el que entrás a Falke y te enviamos un enlace para
                    definir una contraseña nueva.
                </p>

                <asp:Panel ID="avisoEnviado" runat="server" CssClass="aviso aviso-exito" role="status" Visible="false">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-tilde" /></svg>
                    <p class="sin-margen" data-i18n="recuperar.enviado">
                        Si ese correo pertenece a una cuenta registrada, vas a recibir el enlace
                        para definir una contraseña nueva. El aviso se genera como archivo en
                        App_Data/mails/.
                    </p>
                </asp:Panel>

                <asp:Panel ID="bloqueFormulario" runat="server">

                    <asp:Panel ID="avisoError" runat="server" CssClass="aviso aviso-peligro" Visible="false">
                        <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                        <p class="sin-margen"><asp:Literal ID="litError" runat="server" /></p>
                    </asp:Panel>

                    <div class="campo">
                        <label for="recEmail" data-i18n="recuperar.email">Correo de la cuenta</label>
                        <asp:TextBox ID="recEmail" runat="server" CssClass="entrada" TextMode="Email"
                                     ClientIDMode="Static" autocomplete="email"
                                     placeholder="nombre@empresa.com"
                                     data-i18n-attr="placeholder:recuperar.email.placeholder" />
                        <p class="ayuda" data-i18n="recuperar.email.ayuda">
                            Tiene que ser el mismo correo con el que te dieron de alta.
                        </p>
                    </div>

                    <asp:Button ID="btnEnviarEnlace" runat="server" CssClass="btn btn-primario btn-grande btn-bloque"
                                OnClick="btnEnviarEnlace_Click" Text="Enviarme el enlace" data-i18n="recuperar.enviar" />

                    <p class="texto-chico texto-suave mt-16 sin-margen" data-i18n="recuperar.vigencia">
                        El enlace vence a las dos horas de haberse generado y se puede usar una sola vez.
                    </p>

                </asp:Panel>

                <div class="recuperar-pie">
                    <p class="sin-margen">
                        <a href="<%: ResolveUrl("~/Ingresar.aspx") %>" data-i18n="recuperar.volver">Volver al inicio de sesión</a>
                    </p>
                    <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="recuperar.bloqueada">
                        Si tu cuenta fue dada de baja o tiene un bloqueo estricto, no se envía ningún enlace:
                        lo resuelve el administrador de tu empresa.
                    </p>
                </div>

            </div>
        </main>

        <script src="<%: ResolveUrl("~/Scripts/falke.js") %>?v=<%: System.IO.File.GetLastWriteTimeUtc(Server.MapPath("~/Scripts/falke.js")).Ticks %>"></script>
    </form>
</body>
</html>
