<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="MiClave.aspx.cs" Inherits="GUI.MiClave" Title="[[miclave_cambiar_mi_contrasena]]" %>

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
            <h1 data-i18n="miClave.titulo">[[miclave_cambiar_mi_contrasena]]</h1>
            <p class="texto-suave sin-margen" data-i18n="miClave.bajada">
                [[miclave_elegi_una_contrasena_nueva_para_tu]]
            </p>
        </div>
        <div class="acciones">
            <a class="btn btn-secundario" href="<%: ResolveUrl("~/MiPerfil.aspx") %>">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-persona" /></svg>
                <span data-i18n="miClave.volverPerfil">[[miclave_volver_a_mi_perfil]]</span>
            </a>
        </div>
    </div>

    <div class="tarjeta clave-caja">

        <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false">
            <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
        </asp:Panel>

        <div class="campo">
            <label for="claveActual" data-i18n="miClave.actual">[[miclave_contrasena_actual]]</label>
            <div class="campo-con-boton">
                <asp:TextBox ID="claveActual" runat="server" CssClass="entrada" TextMode="Password"
                             ClientIDMode="Static" autocomplete="current-password" />
                <button type="button" class="boton-dentro" data-ver-clave="claveActual"
                        aria-pressed="false" title="[[ingresar_mostrar_la_contrasena]]">
                    <span class="solo-lectores" data-i18n="comun.verContrasena">[[ingresar_mostrar_u_ocultar_la_contrasena]]</span>
                    <svg width="19" height="19" aria-hidden="true" data-icono-clave="ver"><use href="#i-ojo" /></svg>
                    <svg width="19" height="19" aria-hidden="true" data-icono-clave="ocultar" style="display:none"><use href="#i-ojo-tachado" /></svg>
                </button>
            </div>
        </div>

        <hr />

        <div class="campo">
            <label for="claveNueva" data-i18n="miClave.nueva">[[definirclave_contrasena_nueva]]</label>
            <div class="campo-con-boton">
                <asp:TextBox ID="claveNueva" runat="server" CssClass="entrada" TextMode="Password"
                             ClientIDMode="Static" autocomplete="new-password" aria-describedby="listaRequisitos" />
                <button type="button" class="boton-dentro" data-ver-clave="claveNueva"
                        aria-pressed="false" title="[[ingresar_mostrar_la_contrasena]]">
                    <span class="solo-lectores" data-i18n="comun.verContrasena">[[ingresar_mostrar_u_ocultar_la_contrasena]]</span>
                    <svg width="19" height="19" aria-hidden="true" data-icono-clave="ver"><use href="#i-ojo" /></svg>
                    <svg width="19" height="19" aria-hidden="true" data-icono-clave="ocultar" style="display:none"><use href="#i-ojo-tachado" /></svg>
                </button>
            </div>

            <div class="fuerza" aria-hidden="true"><span id="barraFuerza"></span></div>
            <p class="fuerza-texto" id="textoFuerza" role="status" data-i18n="miClave.fuerza.vacia">
                [[miclave_todavia_no_escribiste_nada]]
            </p>

            <ul class="requisitos" id="listaRequisitos">
                <li data-requisito="largo">
                    <span class="marca-req" aria-hidden="true"></span>
                    <span data-i18n="miClave.req.largo">[[miclave_al_menos_8_caracteres]]</span>
                </li>
                <li data-requisito="mayuscula">
                    <span class="marca-req" aria-hidden="true"></span>
                    <span data-i18n="miClave.req.mayuscula">[[miclave_una_mayuscula]]</span>
                </li>
                <li data-requisito="minuscula">
                    <span class="marca-req" aria-hidden="true"></span>
                    <span data-i18n="miClave.req.minuscula">[[miclave_una_minuscula]]</span>
                </li>
                <li data-requisito="numero">
                    <span class="marca-req" aria-hidden="true"></span>
                    <span data-i18n="miClave.req.numero">[[miclave_un_numero]]</span>
                </li>
                <li data-requisito="especial">
                    <span class="marca-req" aria-hidden="true"></span>
                    <span data-i18n="miClave.req.especial">[[miclave_un_caracter_especial_por_ejemplo]]</span>
                </li>
            </ul>
        </div>

        <div class="campo">
            <label for="claveRepetir" data-i18n="miClave.repetir">[[definirclave_repetir_la_contrasena_nueva]]</label>
            <asp:TextBox ID="claveRepetir" runat="server" CssClass="entrada" TextMode="Password"
                         ClientIDMode="Static" autocomplete="new-password" />
            <p class="ayuda" id="avisoRepetir" role="status" data-i18n="miClave.repetir.ayuda">
                [[definirclave_las_dos_tienen_que_coincidir]]
            </p>
        </div>

        <div class="aviso aviso-info">
            <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen" data-i18n="miClave.avisoSesiones">
                [[miclave_al_cambiarla_se_cierran_las_sesiones]]
            </p>
        </div>

        <div class="fila-entre">
            <a class="btn btn-secundario" href="<%: ResolveUrl("~/Panel.aspx") %>" data-i18n="comun.cancelar">[[categorias_cancelar]]</a>
            <asp:Button ID="btnGuardarClave" runat="server" CssClass="btn btn-primario" ClientIDMode="Static"
                        OnClick="btnGuardarClave_Click" Text="[[miclave_cambiar_contrasena]]" data-i18n="miClave.guardar" />
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

            var TEXTOS = ["[[js:miclave_muy_debil]]", "[[js:miclave_debil]]", "[[js:miclave_aceptable]]", "[[js:miclave_buena]]", "[[js:miclave_fuerte]]"];
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
                    textoFuerza.textContent = "[[js:miclave_todavia_no_escribiste_nada]]";
                }
                else
                {
                    barra.style.background = COLORES[cuantos - 1] || COLORES[0];
                    textoFuerza.textContent = "[[js:miclave_fuerza_0]]".replace("{0}", (TEXTOS[cuantos - 1] || TEXTOS[0]));
                }

                var coinciden = clave !== "" && clave === repetir.value;

                if (repetir.value === "")
                    avisoRepetir.textContent = "[[js:definirclave_las_dos_tienen_que_coincidir]]";
                else if (coinciden)
                    avisoRepetir.textContent = "[[js:miclave_coinciden]]";
                else
                    avisoRepetir.textContent = "[[js:miclave_todavia_no_coinciden]]";

                boton.disabled = !(actual.value !== "" && cuantos === items.length && coinciden);
            }

            actual.addEventListener("input", revisar);
            nueva.addEventListener("input", revisar);
            repetir.addEventListener("input", revisar);
        })();
    </script>
</asp:Content>
