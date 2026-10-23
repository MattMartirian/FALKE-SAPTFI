<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="EmpresaNueva.aspx.cs" Inherits="GUI.EmpresaNueva" Title="[[empresanueva_registrar_empresa_cliente]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        .alta
        {
            display: grid;
            grid-template-columns: 250px minmax(0, 1fr) 300px;
            gap: 20px;
            align-items: start;
        }

        .alta-rail
        {
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
            overflow: hidden;
        }

        .alta-rail .titulo
        {
            padding: 14px;
            font-size: .84rem;
            font-weight: 600;
            color: var(--texto-suave);
            border-bottom: 1px solid var(--borde);
            display: flex;
            justify-content: space-between;
        }

        .alta-rail ul
        {
            list-style: none;
            margin: 0;
            padding: 0;
            max-height: 520px;
            overflow-y: auto;
        }

        .alta-rail li
        {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 11px 14px;
            border-bottom: 1px solid var(--borde);
            font-size: .86rem;
        }

        .alta-rail li .sigla
        {
            width: 32px;
            height: 32px;
            flex: none;
            border-radius: var(--radio-sm);
            background: var(--marca-azul);
            color: var(--marca-dorado);
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            font-size: .74rem;
        }

        .alta-rail li.nueva
        {
            background: var(--acento-suave);
            outline: 1px solid var(--acento);
            outline-offset: -1px;
            font-weight: 600;
        }

        .alta-rail li.nueva .sigla
        {
            background: var(--marca-dorado);
            color: var(--marca-azul);
        }

        .alta-form
        {
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
            padding: 22px 24px 24px;
        }

        .alta-bloque
        {
            border: 1px solid var(--borde);
            border-radius: var(--radio);
            padding: 16px 18px 6px;
            margin-bottom: 18px;
        }

        .alta-bloque > h3
        {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: .98rem;
            margin: 0 0 4px;
        }

        .alta-bloque > h3 .n
        {
            width: 22px;
            height: 22px;
            flex: none;
            border-radius: var(--radio-sm);
            background: var(--marca-azul);
            color: var(--marca-perla);
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-family: var(--fuente-mono);
            font-size: .78rem;
        }

        .alta-bloque > .aclaracion
        {
            font-size: .84rem;
            color: var(--texto-suave);
            margin: 0 0 14px;
        }

        .fila-campos
        {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 14px;
        }

        .rol-fijo
        {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 7px 12px;
            border-radius: var(--radio-full);
            background: var(--acento-suave);
            color: var(--acento-texto);
            font-size: .82rem;
            font-weight: 600;
        }

        .alta-lado
        {
            display: flex;
            flex-direction: column;
            gap: 18px;
            position: sticky;
            top: calc(var(--alto-topbar) + 20px);
        }

        .previa-cuenta
        {
            background: var(--superficie-alt);
            color: var(--marca-perla);
            border-radius: var(--radio-lg);
            padding: 18px 20px;
        }

        .previa-cuenta .rotulo
        {
            font-size: .78rem;
            color: rgba(252, 252, 247, .6);
        }

        .previa-cuenta .cab
        {
            display: flex;
            align-items: center;
            gap: 12px;
            margin: 12px 0 14px;
        }

        .previa-cuenta .sigla
        {
            width: 42px;
            height: 42px;
            flex: none;
            border-radius: var(--radio);
            background: var(--marca-dorado);
            color: var(--marca-azul);
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
        }

        .previa-cuenta .lineas
        {
            display: grid;
            gap: 7px;
            font-size: .82rem;
            color: rgba(252, 252, 247, .82);
        }

        .previa-cuenta .lineas span
        {
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .previa-cuenta .estado-inicial::before
        {
            content: "";
            width: 7px;
            height: 7px;
            border-radius: 50%;
            background: var(--marca-coral);
            flex: none;
        }

        .pasos-alta
        {
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
            padding: 18px 20px;
        }

        .pasos-alta h3
        {
            font-size: .95rem;
            margin: 0 0 12px;
        }

        .pasos-alta ol
        {
            margin: 0;
            padding: 0;
            list-style: none;
            counter-reset: p;
            display: grid;
            gap: 12px;
        }

        .pasos-alta li
        {
            display: flex;
            gap: 10px;
            font-size: .84rem;
            color: var(--texto-suave);
        }

        .pasos-alta li::before
        {
            counter-increment: p;
            content: counter(p);
            flex: none;
            width: 20px;
            height: 20px;
            border-radius: 50%;
            background: var(--acento-suave);
            color: var(--acento-texto);
            font-family: var(--fuente-mono);
            font-size: .72rem;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .pasos-alta li strong
        {
            color: var(--texto);
        }

        .alta-pie
        {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            margin-top: 20px;
        }

        @media (max-width: 1080px)
        {
            .alta { grid-template-columns: minmax(0, 1fr); }
            .alta-lado { position: static; }
            .alta-rail ul { max-height: 240px; }
        }

        @media (max-width: 560px)
        {
            .fila-campos { grid-template-columns: minmax(0, 1fr); }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <p class="migas">
        <a href="<%: ResolveUrl("~/Empresas.aspx") %>" data-i18n="empresas.titulo">[[empresanueva_empresas_cliente]]</a>
        <span aria-hidden="true">/</span>
        <strong data-i18n="altaEmpresa.titulo">[[empresanueva_registrar_empresa_cliente]]</strong>
    </p>

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="altaEmpresa.titulo">[[empresanueva_registrar_empresa_cliente]]</h1>
            <p class="texto-suave sin-margen" data-i18n="altaEmpresa.bajada">
                [[empresanueva_alta_manual_del_gestor_de_pattern]]
            </p>
        </div>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso aviso-peligro" Visible="false" role="alert">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <div class="alta">

        <nav class="alta-rail" aria-label="[[empresanueva_empresas_registradas]]">
            <span class="titulo">
                <span data-i18n="empresas.registradas">[[empresanueva_empresas_registradas]]</span>
                <span><asp:Literal ID="litCuentaEmpresas" runat="server" /></span>
            </span>
            <ul>
                <li class="nueva">
                    <span class="sigla" aria-hidden="true">+</span>
                    <span data-i18n="altaEmpresa.enCurso">[[empresanueva_nueva_empresa_en_curso]]</span>
                </li>
                <asp:Repeater ID="rptEmpresas" runat="server">
                    <ItemTemplate>
                        <li><span class="sigla" aria-hidden="true"><%#: Sigla((string)Eval("NombreEmpresa")) %></span><%#: Eval("NombreEmpresa") %></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </nav>

        <div class="alta-form">

            <div class="alta-bloque">
                <h3><span class="n" aria-hidden="true">1</span><span data-i18n="altaEmpresa.bloque.empresa">[[empresanueva_datos_de_la_empresa]]</span></h3>
                <p class="aclaracion" data-i18n="altaEmpresa.bloque.empresa.ayuda">
                    [[empresanueva_los_datos_fiscales_van_tal_como]]
                </p>

                <div class="campo">
                    <label for="aeRazon" data-i18n="altaEmpresa.razon">[[empresanueva_razon_social]]</label>
                    <asp:TextBox ID="aeRazon" runat="server" CssClass="entrada" ClientIDMode="Static" MaxLength="150" data-preview="nombre" data-foco-inicial="si" />
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="aeCuit" data-i18n="altaEmpresa.cuit">[[empresanueva_cuit]]</label>
                        <asp:TextBox ID="aeCuit" runat="server" CssClass="entrada" ClientIDMode="Static" MaxLength="20" placeholder="30-12345678-1" />
                        <p class="ayuda">[[empresanueva_11_digitos_con_o_sin_guiones]]</p>
                    </div>
                    <div class="campo">
                        <label for="aeRubro" data-i18n="altaEmpresa.rubro">[[empresanueva_rubro]]</label>
                        <asp:TextBox ID="aeRubro" runat="server" CssClass="entrada" ClientIDMode="Static" MaxLength="100" data-preview="rubro" />
                    </div>
                </div>

                <div class="campo">
                    <label for="aeDomicilio" data-i18n="altaEmpresa.domicilio">[[empresanueva_domicilio]]</label>
                    <asp:TextBox ID="aeDomicilio" runat="server" CssClass="entrada" ClientIDMode="Static" MaxLength="200" />
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="aePlan" data-i18n="altaEmpresa.plan">[[empresanueva_plan_de_suscripcion]]</label>
                        <asp:DropDownList ID="aePlan" runat="server" CssClass="entrada" ClientIDMode="Static" data-preview="plan">
                            <asp:ListItem Value="Scout">Scout</asp:ListItem>
                            <asp:ListItem Value="Hunter" Selected="True">Hunter</asp:ListItem>
                            <asp:ListItem Value="Apex">Apex</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="campo">
                        <label for="aeFacturacion" data-i18n="altaEmpresa.facturacion">[[empresanueva_facturacion]]</label>
                        <asp:DropDownList ID="aeFacturacion" runat="server" CssClass="entrada" ClientIDMode="Static">
                            <asp:ListItem Value="Mensual">[[empresanueva_mensual]]</asp:ListItem>
                            <asp:ListItem Value="Anual" Selected="True">[[empresanueva_anual]]</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="aeContacto" data-i18n="altaEmpresa.contacto">[[empresanueva_numero_de_contacto]]</label>
                        <asp:TextBox ID="aeContacto" runat="server" CssClass="entrada" ClientIDMode="Static" TextMode="Phone" MaxLength="50" data-preview="telefono" placeholder="+54 11 0000-0000" />
                    </div>
                </div>
            </div>

            <div class="alta-bloque">
                <h3><span class="n" aria-hidden="true">2</span><span data-i18n="altaEmpresa.bloque.admin">[[empresanueva_administrador_inicial_de_la_cuenta]]</span></h3>
                <p class="aclaracion" data-i18n="altaEmpresa.bloque.admin.ayuda">
                    [[empresanueva_es_quien_despues_invita_y_administra]]
                </p>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="aeAdminNombre" data-i18n="altaEmpresa.admin.nombre">[[dispositivos_nombre]]</label>
                        <asp:TextBox ID="aeAdminNombre" runat="server" CssClass="entrada" ClientIDMode="Static" MaxLength="100" data-preview="adminNombre" />
                    </div>
                    <div class="campo">
                        <label for="aeAdminApellido" data-i18n="altaEmpresa.admin.apellido">[[empresanueva_apellido]]</label>
                        <asp:TextBox ID="aeAdminApellido" runat="server" CssClass="entrada" ClientIDMode="Static" MaxLength="100" data-preview="adminApellido" />
                    </div>
                </div>

                <div class="campo">
                    <label for="aeAdminEmail" data-i18n="altaEmpresa.admin.email">[[empresanueva_correo_electronico]]</label>
                    <asp:TextBox ID="aeAdminEmail" runat="server" CssClass="entrada" ClientIDMode="Static" TextMode="Email" MaxLength="255" data-preview="adminEmail" />
                    <p class="ayuda" data-i18n="altaEmpresa.admin.email.ayuda">
                        [[empresanueva_ahi_llega_el_enlace_para_que]]
                    </p>
                </div>

                <div class="campo">
                    <span class="etiqueta" data-i18n="altaEmpresa.admin.rol">[[empresanueva_rol_asignado]]</span>
                    <span class="rol-fijo">
                        <svg width="14" height="14" aria-hidden="true"><use href="#i-candado" /></svg>
                        <span data-i18n="altaEmpresa.admin.rolFijo">[[empresanueva_administrador_cuenta_inicial]]</span>
                    </span>
                </div>
            </div>

            <div class="aviso aviso-alerta mb-0">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
                <p class="sin-margen" data-i18n="altaEmpresa.aviso">
                    [[empresanueva_el_alta_queda_registrada_en_la]]
                </p>
            </div>

            <div class="alta-pie">
                <a class="btn btn-secundario" href="<%: ResolveUrl("~/Empresas.aspx") %>" data-i18n="comun.cancelar">[[categorias_cancelar]]</a>
                <asp:Button ID="btnRegistrar" runat="server" CssClass="btn btn-primario" Text="[[empresanueva_registrar_empresa]]" OnClick="btnRegistrar_Click" />
            </div>

        </div>

        <aside class="alta-lado">

            <div class="previa-cuenta">
                <span class="rotulo" data-i18n="altaEmpresa.previa.titulo">[[empresanueva_vista_previa_de_la_cuenta]]</span>
                <div class="cab">
                    <span class="sigla" aria-hidden="true" id="pvSigla">¿?</span>
                    <div>
                        <strong id="pvNombre" data-i18n="altaEmpresa.previa.sinNombre">[[empresanueva_nombre_de_la_empresa]]</strong><br />
                        <span class="badge badge-alerta mt-8" id="pvPlan">Hunter</span>
                    </div>
                </div>
                <div class="lineas">
                    <span id="pvRubro" data-i18n="altaEmpresa.previa.sinRubro">[[empresanueva_rubro_sin_completar]]</span>
                    <span id="pvTelefono">&mdash;</span>
                    <span class="estado-inicial" data-i18n="altaEmpresa.previa.estado">[[empresanueva_estado_inicial_pendiente_de_activacion]]</span>
                    <span id="pvAdmin" data-i18n="altaEmpresa.previa.sinAdmin">[[empresanueva_administrador_sin_completar]]</span>
                </div>
            </div>

            <div class="pasos-alta">
                <h3 data-i18n="altaEmpresa.pasos.titulo">[[empresanueva_que_pasa_despues_de_registrar]]</h3>
                <ol>
                    <li><span><strong data-i18n="altaEmpresa.paso1.t">[[empresanueva_se_crea_la_cuenta]]</strong>
                        <span data-i18n="altaEmpresa.paso1.d"> [[empresanueva_la_empresa_queda_registrada_en_estado]]</span></span></li>
                    <li><span><strong data-i18n="altaEmpresa.paso2.t">[[empresanueva_se_envia_el_correo]]</strong>
                        <span data-i18n="altaEmpresa.paso2.d"> [[empresanueva_el_administrador_recibe_un_enlace_para]]</span></span></li>
                    <li><span><strong data-i18n="altaEmpresa.paso3.t">[[empresanueva_define_su_contrasena]]</strong>
                        <span data-i18n="altaEmpresa.paso3.d"> [[empresanueva_el_enlace_lo_lleva_a_la]]</span></span></li>
                    <li><span><strong data-i18n="altaEmpresa.paso4.t">[[empresanueva_la_cuenta_queda_activa]]</strong>
                        <span data-i18n="altaEmpresa.paso4.d"> [[empresanueva_el_administrador_ya_puede_invitar_a]]</span></span></li>
                </ol>
            </div>

        </aside>

    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var pvSigla = document.getElementById("pvSigla");
            var pvNombre = document.getElementById("pvNombre");
            var pvPlan = document.getElementById("pvPlan");
            var pvRubro = document.getElementById("pvRubro");
            var pvTelefono = document.getElementById("pvTelefono");
            var pvAdmin = document.getElementById("pvAdmin");

            var razon = document.getElementById("aeRazon");
            var rubro = document.getElementById("aeRubro");
            var plan = document.getElementById("aePlan");
            var telefono = document.getElementById("aeContacto");
            var adminNombre = document.getElementById("aeAdminNombre");
            var adminApellido = document.getElementById("aeAdminApellido");
            var adminEmail = document.getElementById("aeAdminEmail");

            function sigla(texto) {
                var partes = texto.trim().split(/\s+/).filter(Boolean);
                if (partes.length === 0) return "?";
                if (partes.length === 1) return partes[0].substring(0, 2).toUpperCase();
                return (partes[0].charAt(0) + partes[1].charAt(0)).toUpperCase();
            }

            function refrescar() {
                var n = razon.value.trim();
                pvNombre.textContent = n || "[[js:empresanueva_nombre_de_la_empresa]]";
                pvSigla.textContent = n ? sigla(n) : "?";
                pvPlan.textContent = plan.value;
                pvRubro.textContent = rubro.value.trim() || "[[js:empresanueva_rubro_sin_completar]]";
                pvTelefono.textContent = telefono.value.trim() || "—";

                var an = (adminNombre.value.trim() + " " + adminApellido.value.trim()).trim();
                var ae = adminEmail.value.trim();
                if (an && ae) pvAdmin.textContent = an + " · " + ae;
                else if (an) pvAdmin.textContent = an;
                else pvAdmin.textContent = "[[js:empresanueva_administrador_sin_completar]]";
            }

            var campos = [razon, rubro, plan, telefono, adminNombre, adminApellido, adminEmail];
            for (var i = 0; i < campos.length; i++) {
                campos[i].addEventListener("input", refrescar);
                campos[i].addEventListener("change", refrescar);
            }

            refrescar();
        })();
    </script>
</asp:Content>
