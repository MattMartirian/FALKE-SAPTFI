<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="MiClave.aspx.cs" Inherits="GUI.MiClave" Title="Cambiar mi contraseña" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        .clave-caja
        {
            max-width: 560px;
        }

        .requisitos
        {
            list-style: none;
            margin: 14px 0 0;
            padding: 0;
            display: grid;
            gap: 7px;
        }

        .requisitos li
        {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: .87rem;
            color: var(--texto-suave);
        }

        .requisitos li.cumplido
        {
            color: var(--exito);
            font-weight: 600;
        }

        .requisitos .marca-req
        {
            width: 18px;
            height: 18px;
            flex: none;
            border-radius: var(--radio-full);
            border: 2px solid var(--borde-fuerte);
        }

        .requisitos li.cumplido .marca-req
        {
            border-color: var(--exito);
            background: var(--exito);
        }

        .fuerza
        {
            height: 6px;
            border-radius: var(--radio-full);
            background: var(--borde);
            overflow: hidden;
            margin-top: 16px;
        }

        .fuerza span
        {
            display: block;
            height: 100%;
            width: 0;
            background: var(--peligro);
            transition: width .2s ease, background .2s ease;
        }

        .fuerza-texto
        {
            font-size: .82rem;
            color: var(--texto-suave);
            margin-top: 7px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="miClave.titulo">Cambiar mi contraseña</h1>
            <p class="texto-suave sin-margen" data-i18n="miClave.bajada">
                Elegí una contraseña nueva para tu cuenta. Se te va a pedir la actual.
            </p>
        </div>
        <div class="acciones">
            <a class="btn btn-secundario" href="<%: ResolveUrl("~/MiPerfil.aspx") %>">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-persona" /></svg>
                <span data-i18n="miClave.volverPerfil">Volver a mi perfil</span>
            </a>
        </div>
    </div>

    <div class="tarjeta clave-caja">

        <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false">
            <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
        </asp:Panel>

        <div class="campo">
            <label for="claveActual" data-i18n="miClave.actual">Contraseña actual</label>
            <div class="campo-con-boton">
                <asp:TextBox ID="claveActual" runat="server" CssClass="entrada" TextMode="Password"
                             ClientIDMode="Static" autocomplete="current-password" />
                <button type="button" class="boton-dentro" data-ver-clave="claveActual"
                        aria-pressed="false" title="Mostrar la contraseña">
                    <span class="solo-lectores" data-i18n="comun.verContrasena">Mostrar u ocultar la contraseña</span>
                    <svg width="19" height="19" aria-hidden="true" data-icono-clave="ver"><use href="#i-ojo" /></svg>
                    <svg width="19" height="19" aria-hidden="true" data-icono-clave="ocultar" style="display:none"><use href="#i-ojo-tachado" /></svg>
                </button>
            </div>
        </div>

        <hr />

        <div class="campo">
            <label for="claveNueva" data-i18n="miClave.nueva">Contraseña nueva</label>
            <div class="campo-con-boton">
                <asp:TextBox ID="claveNueva" runat="server" CssClass="entrada" TextMode="Password"
                             ClientIDMode="Static" autocomplete="new-password" aria-describedby="listaRequisitos" />
                <button type="button" class="boton-dentro" data-ver-clave="claveNueva"
                        aria-pressed="false" title="Mostrar la contraseña">
                    <span class="solo-lectores" data-i18n="comun.verContrasena">Mostrar u ocultar la contraseña</span>
                    <svg width="19" height="19" aria-hidden="true" data-icono-clave="ver"><use href="#i-ojo" /></svg>
                    <svg width="19" height="19" aria-hidden="true" data-icono-clave="ocultar" style="display:none"><use href="#i-ojo-tachado" /></svg>
                </button>
            </div>

            <div class="fuerza" aria-hidden="true"><span id="barraFuerza"></span></div>
            <p class="fuerza-texto" id="textoFuerza" role="status" data-i18n="miClave.fuerza.vacia">
                Todavía no escribiste nada.
            </p>

            <ul class="requisitos" id="listaRequisitos">
                <li data-requisito="largo">
                    <span class="marca-req" aria-hidden="true"></span>
                    <span data-i18n="miClave.req.largo">Al menos 8 caracteres</span>
                </li>
                <li data-requisito="mayuscula">
                    <span class="marca-req" aria-hidden="true"></span>
                    <span data-i18n="miClave.req.mayuscula">Una mayúscula</span>
                </li>
                <li data-requisito="minuscula">
                    <span class="marca-req" aria-hidden="true"></span>
                    <span data-i18n="miClave.req.minuscula">Una minúscula</span>
                </li>
                <li data-requisito="numero">
                    <span class="marca-req" aria-hidden="true"></span>
                    <span data-i18n="miClave.req.numero">Un número</span>
                </li>
                <li data-requisito="especial">
                    <span class="marca-req" aria-hidden="true"></span>
                    <span data-i18n="miClave.req.especial">Un carácter especial (por ejemplo # ! @ $ %)</span>
                </li>
            </ul>
        </div>

        <div class="campo">
            <label for="claveRepetir" data-i18n="miClave.repetir">Repetir la contraseña nueva</label>
            <asp:TextBox ID="claveRepetir" runat="server" CssClass="entrada" TextMode="Password"
                         ClientIDMode="Static" autocomplete="new-password" />
            <p class="ayuda" id="avisoRepetir" role="status" data-i18n="miClave.repetir.ayuda">
                Las dos tienen que coincidir.
            </p>
        </div>

        <div class="aviso aviso-info">
            <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen" data-i18n="miClave.avisoSesiones">
                Al cambiarla se cierran las sesiones abiertas en otros equipos y vas a tener
                que volver a entrar en ellos.
            </p>
        </div>

        <div class="fila-entre">
            <a class="btn btn-secundario" href="<%: ResolveUrl("~/Panel.aspx") %>" data-i18n="comun.cancelar">Cancelar</a>
            <asp:Button ID="btnGuardarClave" runat="server" CssClass="btn btn-primario" ClientIDMode="Static"
                        OnClick="btnGuardarClave_Click" Text="Cambiar contraseña" data-i18n="miClave.guardar" />
        </div>

    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var actual = document.getElementById("claveActual");
            var nueva = document.getElementById("claveNueva");
            var repetir = document.getElementById("claveRepetir");
            var boton = document.getElementById("btnGuardarClave");
            var barra = document.getElementById("barraFuerza");
            var textoFuerza = document.getElementById("textoFuerza");
            var avisoRepetir = document.getElementById("avisoRepetir");
            var items = document.querySelectorAll("#listaRequisitos li");

            var TEXTOS = ["Muy débil", "Débil", "Aceptable", "Buena", "Fuerte"];
            var COLORES = ["var(--peligro)", "var(--alerta)", "var(--info)", "var(--exito)", "var(--exito)"];

            function requisitosCumplidos(clave) {
                return {
                    largo: clave.length >= 8,
                    mayuscula: /\p{Lu}/u.test(clave),
                    minuscula: /\p{Ll}/u.test(clave),
                    numero: /\p{Nd}/u.test(clave),
                    especial: /[^\p{L}\p{Nd}\s]/u.test(clave)
                };
            }

            function revisar() {
                var clave = nueva.value;
                var cumple = requisitosCumplidos(clave);
                var cuantos = 0;

                for (var i = 0; i < items.length; i++)
                {
                    var nombre = items[i].getAttribute("data-requisito");
                    var ok = cumple[nombre];

                    items[i].classList.toggle("cumplido", ok);

                    if (ok) cuantos++;
                }

                barra.style.width = (cuantos * 20) + "%";

                if (clave === "")
                {
                    textoFuerza.textContent = "Todavía no escribiste nada.";
                }
                else
                {
                    barra.style.background = COLORES[cuantos - 1] || COLORES[0];
                    textoFuerza.textContent = "Fuerza: " + (TEXTOS[cuantos - 1] || TEXTOS[0]);
                }

                var coinciden = clave !== "" && clave === repetir.value;

                if (repetir.value === "")
                    avisoRepetir.textContent = "Las dos tienen que coincidir.";
                else if (coinciden)
                    avisoRepetir.textContent = "Coinciden.";
                else
                    avisoRepetir.textContent = "Todavía no coinciden.";

                boton.disabled = !(actual.value !== "" && cuantos === items.length && coinciden);
            }

            actual.addEventListener("input", revisar);
            nueva.addEventListener("input", revisar);
            repetir.addEventListener("input", revisar);
        })();
    </script>
</asp:Content>
