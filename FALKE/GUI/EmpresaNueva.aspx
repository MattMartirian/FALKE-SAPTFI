<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="EmpresaNueva.aspx.cs" Inherits="GUI.EmpresaNueva" Title="Registrar empresa cliente" %>

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
        <a href="<%: ResolveUrl("~/Empresas.aspx") %>" data-i18n="empresas.titulo">Empresas cliente</a>
        <span aria-hidden="true">/</span>
        <strong data-i18n="altaEmpresa.titulo">Registrar empresa cliente</strong>
    </p>

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="altaEmpresa.titulo">Registrar empresa cliente</h1>
            <p class="texto-suave sin-margen" data-i18n="altaEmpresa.bajada">
                Alta manual del gestor de Pattern Blue, una vez formalizado el acuerdo comercial.
                Se crea también el usuario administrador inicial, que recibe un enlace de activación.
            </p>
        </div>
    </div>

    <div class="alta">

        <nav class="alta-rail" aria-label="Empresas registradas">
            <span class="titulo">
                <span data-i18n="empresas.registradas">Empresas registradas</span>
                <span>6</span>
            </span>
            <ul>
                <li class="nueva">
                    <span class="sigla" aria-hidden="true">+</span>
                    <span data-i18n="altaEmpresa.enCurso">Nueva empresa (en curso)</span>
                </li>
                <li><span class="sigla" aria-hidden="true">IR</span>Ironhide Game Studio</li>
                <li><span class="sigla" aria-hidden="true">MC</span>Mercado Cruz S.A.</li>
                <li><span class="sigla" aria-hidden="true">NV</span>Nova Publicidad</li>
                <li><span class="sigla" aria-hidden="true">DL</span>Delta Labs</li>
                <li><span class="sigla" aria-hidden="true">PX</span>Pixel Norte</li>
                <li><span class="sigla" aria-hidden="true">AU</span>Austral Seguros</li>
            </ul>
        </nav>

        <form class="alta-form" onsubmit="return false">

            <div class="alta-bloque">
                <h3><span class="n" aria-hidden="true">1</span><span data-i18n="altaEmpresa.bloque.empresa">Datos de la empresa</span></h3>
                <p class="aclaracion" data-i18n="altaEmpresa.bloque.empresa.ayuda">
                    Los datos fiscales van tal como figuran en la factura.
                </p>

                <div class="campo">
                    <label for="aeRazon" data-i18n="altaEmpresa.razon">Razón social</label>
                    <input type="text" class="entrada" id="aeRazon" data-preview="nombre" data-foco-inicial />
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="aeCuit" data-i18n="altaEmpresa.cuit">CUIT</label>
                        <input type="text" class="entrada" id="aeCuit" placeholder="30-00000000-0" />
                    </div>
                    <div class="campo">
                        <label for="aeRubro" data-i18n="altaEmpresa.rubro">Rubro</label>
                        <input type="text" class="entrada" id="aeRubro" data-preview="rubro" />
                    </div>
                </div>

                <div class="campo">
                    <label for="aeDomicilio" data-i18n="altaEmpresa.domicilio">Domicilio</label>
                    <input type="text" class="entrada" id="aeDomicilio" />
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="aePlan" data-i18n="altaEmpresa.plan">Plan de suscripción</label>
                        <select class="entrada" id="aePlan" data-preview="plan">
                            <option value="Scout">Scout</option>
                            <option value="Hunter" selected>Hunter</option>
                            <option value="Apex">Apex</option>
                        </select>
                    </div>
                    <div class="campo">
                        <label for="aeFacturacion" data-i18n="altaEmpresa.facturacion">Facturación</label>
                        <select class="entrada" id="aeFacturacion">
                            <option value="mensual" data-i18n="planes.mensual">Mensual</option>
                            <option value="anual" selected data-i18n="planes.anual">Anual</option>
                        </select>
                    </div>
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="aeContacto" data-i18n="altaEmpresa.contacto">Número de contacto</label>
                        <input type="tel" class="entrada" id="aeContacto" data-preview="telefono" placeholder="+54 11 0000-0000" />
                    </div>
                    <div class="campo">
                        <label for="aeDispositivos" data-i18n="altaEmpresa.dispositivos">Dispositivos a prestar</label>
                        <input type="number" class="entrada" id="aeDispositivos" value="1" min="0" max="20" />
                    </div>
                </div>
            </div>

            <div class="alta-bloque">
                <h3><span class="n" aria-hidden="true">2</span><span data-i18n="altaEmpresa.bloque.admin">Administrador inicial de la cuenta</span></h3>
                <p class="aclaracion" data-i18n="altaEmpresa.bloque.admin.ayuda">
                    Es quien después invita y administra al resto de los usuarios de su empresa.
                </p>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="aeAdminNombre" data-i18n="altaEmpresa.admin.nombre">Nombre</label>
                        <input type="text" class="entrada" id="aeAdminNombre" data-preview="adminNombre" />
                    </div>
                    <div class="campo">
                        <label for="aeAdminApellido" data-i18n="altaEmpresa.admin.apellido">Apellido</label>
                        <input type="text" class="entrada" id="aeAdminApellido" data-preview="adminApellido" />
                    </div>
                </div>

                <div class="campo">
                    <label for="aeAdminEmail" data-i18n="altaEmpresa.admin.email">Correo electrónico</label>
                    <input type="email" class="entrada" id="aeAdminEmail" data-preview="adminEmail" />
                    <p class="ayuda" data-i18n="altaEmpresa.admin.email.ayuda">
                        Ahí llega el enlace para que defina su contraseña. Después no se puede cambiar.
                    </p>
                </div>

                <div class="campo">
                    <span class="etiqueta" data-i18n="altaEmpresa.admin.rol">Rol asignado</span>
                    <span class="rol-fijo">
                        <svg width="14" height="14" aria-hidden="true"><use href="#i-candado" /></svg>
                        <span data-i18n="altaEmpresa.admin.rolFijo">Administrador (cuenta inicial)</span>
                    </span>
                </div>
            </div>

            <div class="aviso aviso-alerta mb-0">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-alerta" /></svg>
                <p class="sin-margen" data-i18n="altaEmpresa.aviso">
                    El alta queda registrada en la bitácora con criticidad alta. Si la empresa
                    no continúa, se la deshabilita en lugar de borrarla.
                </p>
            </div>

            <div class="alta-pie">
                <a class="btn btn-secundario" href="<%: ResolveUrl("~/Empresas.aspx") %>" data-i18n="comun.cancelar">Cancelar</a>
                <button type="submit" class="btn btn-primario" data-i18n="altaEmpresa.crear">Registrar empresa</button>
            </div>

        </form>

        <aside class="alta-lado">

            <div class="previa-cuenta">
                <span class="rotulo" data-i18n="altaEmpresa.previa.titulo">Vista previa de la cuenta</span>
                <div class="cab">
                    <span class="sigla" aria-hidden="true" id="pvSigla">¿?</span>
                    <div>
                        <strong id="pvNombre" data-i18n="altaEmpresa.previa.sinNombre">Nombre de la empresa</strong><br />
                        <span class="badge badge-alerta mt-8" id="pvPlan">Hunter</span>
                    </div>
                </div>
                <div class="lineas">
                    <span id="pvRubro" data-i18n="altaEmpresa.previa.sinRubro">Rubro sin completar</span>
                    <span id="pvTelefono">&mdash;</span>
                    <span class="estado-inicial" data-i18n="altaEmpresa.previa.estado">Estado inicial: pendiente de activación</span>
                    <span id="pvAdmin" data-i18n="altaEmpresa.previa.sinAdmin">Administrador sin completar</span>
                </div>
            </div>

            <div class="pasos-alta">
                <h3 data-i18n="altaEmpresa.pasos.titulo">Qué pasa después de registrar</h3>
                <ol>
                    <li><span><strong data-i18n="altaEmpresa.paso1.t">Se crea la cuenta.</strong>
                        <span data-i18n="altaEmpresa.paso1.d"> La empresa queda registrada en estado pendiente de activación.</span></span></li>
                    <li><span><strong data-i18n="altaEmpresa.paso2.t">Se envía el correo.</strong>
                        <span data-i18n="altaEmpresa.paso2.d"> El administrador recibe un enlace para definir sus credenciales.</span></span></li>
                    <li><span><strong data-i18n="altaEmpresa.paso3.t">Define su contraseña.</strong>
                        <span data-i18n="altaEmpresa.paso3.d"> El enlace lo lleva a la pantalla de activación de cuenta.</span></span></li>
                    <li><span><strong data-i18n="altaEmpresa.paso4.t">La cuenta queda activa.</strong>
                        <span data-i18n="altaEmpresa.paso4.d"> El administrador ya puede invitar a los demás usuarios de su empresa.</span></span></li>
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
                pvNombre.textContent = n || "Nombre de la empresa";
                pvSigla.textContent = n ? sigla(n) : "?";
                pvPlan.textContent = plan.value;
                pvRubro.textContent = rubro.value.trim() || "Rubro sin completar";
                pvTelefono.textContent = telefono.value.trim() || "—";

                var an = (adminNombre.value.trim() + " " + adminApellido.value.trim()).trim();
                var ae = adminEmail.value.trim();
                if (an && ae) pvAdmin.textContent = an + " · " + ae;
                else if (an) pvAdmin.textContent = an;
                else pvAdmin.textContent = "Administrador sin completar";
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
