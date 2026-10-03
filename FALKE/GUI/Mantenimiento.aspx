<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Mantenimiento.aspx.cs" Inherits="GUI.Mantenimiento" %>
<%@ Register Src="~/Controles/Iconos.ascx" TagPrefix="falke" TagName="Iconos" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Sistema en mantenimiento - Falke</title>

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

        .pantalla-mantenimiento
        {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 40px 20px;
            background: var(--marca-azul);
        }

        .caja-mant
        {
            max-width: 560px;
            width: 100%;
            background: var(--superficie);
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            box-shadow: var(--sombra);
            padding: 44px 40px 36px;
        }

        .caja-mant .marca
        {
            display: inline-flex;
            padding: 10px 14px;
            margin-bottom: 30px;
            background: var(--marca-azul);
            border-radius: var(--radio);
        }

        .caja-mant .simbolo
        {
            margin: 0 0 14px;
            color: var(--acento-texto);
        }

        .caja-mant h1
        {
            font-size: 1.55rem;
            margin-bottom: 10px;
        }

        .estimado
        {
            display: inline-flex;
            align-items: center;
            gap: 9px;
            padding: 10px 16px;
            border-radius: var(--radio);
            background: var(--superficie-2);
            border: 1px solid var(--borde);
            font-weight: 600;
            margin: 8px 0 22px;
        }

        .caja-mant-pie
        {
            margin-top: 26px;
            padding-top: 20px;
            border-top: 1px solid var(--borde);
            font-size: .86rem;
            color: var(--texto-suave);
        }

        @media (max-width: 600px)
        {
            .caja-mant
            {
                padding: 32px 22px 26px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <falke:Iconos runat="server" ID="ucIconos" />

        <main class="pantalla-mantenimiento">
            <div class="caja-mant">

                <span class="marca">
                    <img src="<%: ResolveUrl("~/Content/img/falke-logo.svg") %>" alt="Falke" />
                </span>

                <div class="simbolo" aria-hidden="true">
                    <svg width="36" height="36"><use href="#i-engranaje" /></svg>
                </div>

                <h1 data-i18n="mantenimientoPagina.titulo">Estamos haciendo mantenimiento</h1>

                <p class="texto-suave" data-i18n="mantenimientoPagina.texto">
                    El sistema no está disponible por unos minutos mientras realizamos tareas
                    programadas. Tus datos y tus grabaciones están a salvo.
                </p>

                <p class="estimado">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-reloj" /></svg>
                    <span data-i18n="mantenimientoPagina.estimado">Estimamos volver a las 20:00</span>
                </p>

                <div class="aviso aviso-info" style="text-align:left">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
                    <p class="sin-margen" data-i18n="mantenimientoPagina.aviso">
                        Si estabas grabando una sesión, la grabación quedó guardada y va a estar
                        disponible cuando el sistema vuelva.
                    </p>
                </div>

                <button type="button" class="btn btn-primario btn-grande mt-16" id="btnReintentar"
                        data-i18n="mantenimientoPagina.reintentar">
                    Reintentar
                </button>

                <div class="caja-mant-pie">
                    <p class="sin-margen" data-i18n="mantenimientoPagina.contacto">
                        Si es urgente, escríbenos a soporte@patternblue.com
                    </p>
                </div>

            </div>
        </main>

    </form>

    <script src="<%: ResolveUrl("~/Scripts/falke.js") %>?v=<%: System.IO.File.GetLastWriteTimeUtc(Server.MapPath("~/Scripts/falke.js")).Ticks %>"></script>
    <script>
        (function () {
            "use strict";

            document.getElementById("btnReintentar").addEventListener("click", function () {
                window.location.reload();
            });
        })();
    </script>
</body>
</html>
