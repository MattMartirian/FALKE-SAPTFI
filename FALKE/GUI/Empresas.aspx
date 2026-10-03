<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Empresas.aspx.cs" Inherits="GUI.Empresas" Title="Empresas cliente" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        .emp
        {
            display: grid;
            grid-template-columns: 320px minmax(0, 1fr);
            gap: 20px;
            align-items: start;
        }

        .emp-rail
        {
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
            overflow: hidden;
        }

        .emp-rail-cab
        {
            padding: 14px;
            border-bottom: 1px solid var(--borde);
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .emp-rail-cab .titulo
        {
            font-size: .84rem;
            font-weight: 600;
            color: var(--texto-suave);
            display: flex;
            justify-content: space-between;
        }

        .emp-buscador
        {
            position: relative;
        }

        .emp-lista
        {
            max-height: 620px;
            overflow-y: auto;
        }

        .emp-fila
        {
            display: flex;
            align-items: flex-start;
            gap: 12px;
            width: 100%;
            text-align: left;
            padding: 13px 14px;
            border: 0;
            border-bottom: 1px solid var(--borde);
            background: transparent;
            color: var(--texto);
            font-family: inherit;
            cursor: pointer;
        }

        .emp-fila:hover
        {
            background: var(--superficie-2);
        }

        .emp-fila[aria-current="true"]
        {
            background: var(--acento-suave);
            outline: 1px solid var(--acento);
            outline-offset: -1px;
        }

        .emp-fila .sigla
        {
            width: 38px;
            height: 38px;
            flex: none;
            border-radius: var(--radio-sm);
            background: var(--marca-azul);
            color: var(--marca-dorado);
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            font-size: .82rem;
        }

        .emp-fila .info
        {
            min-width: 0;
            flex: 1;
        }

        .emp-fila .nombre
        {
            display: block;
            font-weight: 600;
            font-size: .9rem;
        }

        .emp-fila .rubro
        {
            display: block;
            font-size: .76rem;
            color: var(--texto-suave);
            margin-top: 2px;
        }

        .emp-fila .marcas
        {
            display: flex;
            flex-wrap: wrap;
            gap: 4px;
            margin-top: 6px;
        }

        .senal-vencido
        {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-family: var(--fuente-mono);
            font-size: .62rem;
            letter-spacing: .06em;
            text-transform: uppercase;
            color: var(--marca-coral);
        }

        .senal-vencido::before
        {
            content: "";
            width: 6px;
            height: 6px;
            border-radius: 50%;
            background: var(--marca-coral);
        }

        .emp-panel
        {
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
            padding: 22px 24px 26px;
        }

        .emp-detalle-cab
        {
            display: flex;
            align-items: center;
            gap: 14px;
            flex-wrap: wrap;
            margin-bottom: 8px;
        }

        .emp-detalle-cab .sigla
        {
            width: 48px;
            height: 48px;
            flex: none;
            border-radius: var(--radio);
            background: var(--marca-azul);
            color: var(--marca-dorado);
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
        }

        .emp-detalle-cab h2
        {
            margin: 0 0 3px;
            font-size: 1.25rem;
        }

        .emp-detalle-cab .cuit
        {
            font-family: var(--fuente-mono);
            font-size: .78rem;
            color: var(--texto-suave);
        }

        .emp-detalle-cab .marcas
        {
            margin-left: auto;
            display: flex;
            gap: 6px;
        }

        .emp-datos
        {
            width: 100%;
            border-collapse: collapse;
        }

        .emp-datos th,
        .emp-datos td
        {
            text-align: left;
            padding: 10px 0;
            border-bottom: 1px solid var(--borde);
            font-size: .9rem;
            vertical-align: top;
        }

        .emp-datos th
        {
            width: 210px;
            color: var(--texto-suave);
            font-weight: 600;
        }

        .emp-acciones
        {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 22px;
        }

        @media (max-width: 940px)
        {
            .emp { grid-template-columns: minmax(0, 1fr); }
            .emp-lista { max-height: 340px; }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="empresas.titulo">Empresas cliente</h1>
            <p class="texto-suave sin-margen" data-i18n="empresas.bajada">
                Cartera de empresas que contrataron Falke, con su plan y su estado.
            </p>
        </div>
        <div class="acciones">
            <a class="btn btn-primario" href="<%: ResolveUrl("~/EmpresaNueva.aspx") %>">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span data-i18n="empresas.nueva">Nueva empresa</span>
            </a>
        </div>
    </div>

    <div class="metricas mb-24">
        <div>
            <div class="metrica-valor">14</div>
            <div class="metrica-nombre" data-i18n="empresas.metrica.total">Empresas cliente</div>
        </div>
        <div>
            <div class="metrica-valor">12</div>
            <div class="metrica-nombre" data-i18n="empresas.metrica.activas">Activas</div>
        </div>
        <div>
            <div class="metrica-valor">19</div>
            <div class="metrica-nombre" data-i18n="empresas.metrica.dispositivos">Dispositivos prestados</div>
        </div>
        <div>
            <div class="metrica-valor">78</div>
            <div class="metrica-nombre" data-i18n="empresas.metrica.usuarios">Usuarios totales</div>
        </div>
    </div>

    <div class="emp">

        <div class="emp-rail">
            <div class="emp-rail-cab">
                <span class="titulo">
                    <span data-i18n="empresas.registradas">Empresas registradas</span>
                    <span id="empCuenta">6</span>
                </span>
                <div class="emp-buscador campo-buscar">
                    <label class="solo-lectores" for="empBuscar" data-i18n="empresas.buscar">Buscar</label>
                    <svg aria-hidden="true"><use href="#i-buscar" /></svg>
                    <input type="search" class="entrada" id="empBuscar"
                           placeholder="Razón social o CUIT..." data-i18n-attr="placeholder:empresas.buscar.placeholder" />
                </div>
                <div class="chips" data-unico role="group" aria-label="Filtrar por estado">
                    <button type="button" class="chip" aria-pressed="true" data-estado-filtro="" data-i18n="comun.todos">Todas</button>
                    <button type="button" class="chip" aria-pressed="false" data-estado-filtro="Activa" data-i18n="estado.activa">Activas</button>
                    <button type="button" class="chip" aria-pressed="false" data-estado-filtro="Bloqueada" data-i18n="estado.bloqueada">Bloqueadas</button>
                </div>
            </div>

            <div class="emp-lista" id="empLista" role="listbox" aria-label="Empresas cliente">

                <button type="button" class="emp-fila" role="option" aria-current="true"
                        data-nombre="Ironhide Game Studio" data-razon="Ironhide Game Studio S.R.L." data-sigla="IR"
                        data-cuit="30-71234567-8" data-rubro="Desarrollo de videojuegos" data-plan="Hunter" data-factura="Anual — USD 760 por mes"
                        data-estado="Activa" data-alta="15 jun 2026" data-renovacion="15 jun 2027" data-pago="15 ago 2026"
                        data-usuarios="6" data-dispositivos="1" data-sesiones="48" data-limite="10"
                        data-contacto="Lucía Blanco" data-contactoemail="lucia.blanco@ironhide.com" data-domicilio="Av. Italia 1234, Montevideo"
                        data-vencido="0">
                    <span class="sigla" aria-hidden="true">IR</span>
                    <span class="info">
                        <span class="nombre">Ironhide Game Studio</span>
                        <span class="rubro">Videojuegos</span>
                        <span class="marcas">
                            <span class="badge badge-alerta">Hunter</span>
                            <span class="badge badge-exito" data-i18n="estado.activa">Activa</span>
                        </span>
                    </span>
                </button>

                <button type="button" class="emp-fila" role="option"
                        data-nombre="Mercado Cruz S.A." data-razon="Mercado Cruz S.A." data-sigla="MC"
                        data-cuit="30-70998877-1" data-rubro="Comercio electrónico" data-plan="Apex" data-factura="Anual — a convenir"
                        data-estado="Activa" data-alta="3 mar 2026" data-renovacion="3 mar 2027" data-pago="3 sep 2026"
                        data-usuarios="14" data-dispositivos="4" data-sesiones="206" data-limite="25"
                        data-contacto="Pablo Herrera" data-contactoemail="pablo.herrera@mercadocruz.com" data-domicilio="Av. Corrientes 980, CABA"
                        data-vencido="0">
                    <span class="sigla" aria-hidden="true">MC</span>
                    <span class="info">
                        <span class="nombre">Mercado Cruz S.A.</span>
                        <span class="rubro">Comercio electrónico</span>
                        <span class="marcas">
                            <span class="badge badge-info">Apex</span>
                            <span class="badge badge-exito" data-i18n="estado.activa">Activa</span>
                        </span>
                    </span>
                </button>

                <button type="button" class="emp-fila" role="option"
                        data-nombre="Nova Publicidad" data-razon="Nova Publicidad S.A.S." data-sigla="NV"
                        data-cuit="30-71555222-4" data-rubro="Agencia de publicidad" data-plan="Scout" data-factura="Mensual — USD 590"
                        data-estado="Activa" data-alta="20 jul 2026" data-renovacion="20 ago 2026" data-pago="20 ago 2026"
                        data-usuarios="4" data-dispositivos="1" data-sesiones="22" data-limite="5"
                        data-contacto="Carla Suárez" data-contactoemail="carla.suarez@novapublicidad.com" data-domicilio="Bv. Oroño 1500, Rosario"
                        data-vencido="0">
                    <span class="sigla" aria-hidden="true">NV</span>
                    <span class="info">
                        <span class="nombre">Nova Publicidad</span>
                        <span class="rubro">Agencia de publicidad</span>
                        <span class="marcas">
                            <span class="badge badge-neutro">Scout</span>
                            <span class="badge badge-exito" data-i18n="estado.activa">Activa</span>
                        </span>
                    </span>
                </button>

                <button type="button" class="emp-fila" role="option"
                        data-nombre="Delta Labs" data-razon="Delta Labs S.R.L." data-sigla="DL"
                        data-cuit="30-71778899-0" data-rubro="Software a medida" data-plan="Hunter" data-factura="Anual — USD 760 por mes"
                        data-estado="Bloqueada" data-alta="9 nov 2025" data-renovacion="9 nov 2026" data-pago="9 jul 2026"
                        data-usuarios="8" data-dispositivos="2" data-sesiones="164" data-limite="10"
                        data-contacto="Ramiro Ferro" data-contactoemail="ramiro.ferro@deltalabs.com" data-domicilio="Ruta 8 Km 60, Pilar"
                        data-vencido="1">
                    <span class="sigla" aria-hidden="true">DL</span>
                    <span class="info">
                        <span class="nombre">Delta Labs</span>
                        <span class="rubro">Software a medida</span>
                        <span class="marcas">
                            <span class="badge badge-alerta">Hunter</span>
                            <span class="badge badge-peligro" data-i18n="estado.bloqueada">Bloqueada</span>
                            <span class="senal-vencido" data-i18n="empresas.pagoVencido">Pago vencido</span>
                        </span>
                    </span>
                </button>

                <button type="button" class="emp-fila" role="option"
                        data-nombre="Pixel Norte" data-razon="Pixel Norte S.A.S." data-sigla="PX"
                        data-cuit="30-71222333-6" data-rubro="Diseño de interfaces" data-plan="Scout" data-factura="Mensual — USD 590"
                        data-estado="Deshabilitada" data-alta="4 ene 2026" data-renovacion="&mdash;" data-pago="30 may 2026"
                        data-usuarios="3" data-dispositivos="0" data-sesiones="41" data-limite="5"
                        data-contacto="Inés Ledesma" data-contactoemail="ines.ledesma@pixelnorte.com" data-domicilio="San Martín 233, Salta"
                        data-vencido="0">
                    <span class="sigla" aria-hidden="true">PX</span>
                    <span class="info">
                        <span class="nombre">Pixel Norte</span>
                        <span class="rubro">Diseño de interfaces</span>
                        <span class="marcas">
                            <span class="badge badge-neutro">Scout</span>
                            <span class="badge badge-neutro" data-i18n="estado.deshabilitada">Deshabilitada</span>
                        </span>
                    </span>
                </button>

                <button type="button" class="emp-fila" role="option"
                        data-nombre="Austral Seguros" data-razon="Austral Seguros S.A." data-sigla="AU"
                        data-cuit="30-70445566-2" data-rubro="Seguros" data-plan="Apex" data-factura="Anual — a convenir"
                        data-estado="Activa" data-alta="7 sep 2026" data-renovacion="7 sep 2027" data-pago="7 sep 2026"
                        data-usuarios="21" data-dispositivos="6" data-sesiones="311" data-limite="40"
                        data-contacto="Gonzalo Vidal" data-contactoemail="gonzalo.vidal@australseguros.com" data-domicilio="Av. Libertador 5000, CABA"
                        data-vencido="0">
                    <span class="sigla" aria-hidden="true">AU</span>
                    <span class="info">
                        <span class="nombre">Austral Seguros</span>
                        <span class="rubro">Seguros</span>
                        <span class="marcas">
                            <span class="badge badge-info">Apex</span>
                            <span class="badge badge-exito" data-i18n="estado.activa">Activa</span>
                        </span>
                    </span>
                </button>

            </div>

            <div class="vacio" id="empVacio" hidden>
                <svg width="28" height="28" aria-hidden="true"><use href="#i-empresa" /></svg>
                <p class="sin-margen" data-i18n="empresas.vacio">No hay empresas que coincidan.</p>
            </div>
        </div>

        <div class="emp-panel">
            <div id="empDetalle"></div>
        </div>

    </div>

    <script type="application/json" id="empTextos">
    {
        "tabDatos": "Datos",
        "tabPlan": "Plan y facturación",
        "tabEquipo": "Equipo y dispositivos",
        "tabSesiones": "Actividad",
        "razon": "Razón social",
        "cuit": "CUIT",
        "rubro": "Rubro",
        "domicilio": "Domicilio",
        "contacto": "Contacto comercial",
        "estado": "Estado",
        "alta": "Cliente desde",
        "plan": "Plan contratado",
        "factura": "Facturación",
        "renovacion": "Próxima renovación",
        "pago": "Último pago registrado",
        "limite": "Límite de usuarios",
        "usuarios": "Usuarios de la cuenta",
        "dispositivos": "Dispositivos en préstamo",
        "sesiones": "Sesiones grabadas",
        "editar": "Editar datos",
        "bloquear": "Bloquear empresa",
        "desbloquear": "Desbloquear empresa",
        "verUsuarios": "Ver usuarios",
        "vencidoAviso": "Esta empresa tiene un pago vencido. El acceso de sus usuarios está suspendido hasta la regularización."
    }
    </script>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var T = JSON.parse(document.getElementById("empTextos").textContent);
            var filas = document.querySelectorAll(".emp-fila");
            var buscar = document.getElementById("empBuscar");
            var chips = document.querySelectorAll(".chips [data-estado-filtro]");
            var vacio = document.getElementById("empVacio");
            var detalle = document.getElementById("empDetalle");
            var cuenta = document.getElementById("empCuenta");
            var estadoFiltro = "";

            function badgeEstado(estado) {
                var mapa = {
                    "Activa": "badge-exito", "Bloqueada": "badge-peligro", "Deshabilitada": "badge-neutro"
                };
                return '<span class="badge ' + (mapa[estado] || "badge-neutro") + '">' + estado + '</span>';
            }

            function fila(th, td) {
                return '<tr><th>' + th + '</th><td>' + td + '</td></tr>';
            }

            function panelDatos(d) {
                return '<table class="emp-datos"><tbody>' +
                    fila(T.razon, d.razon) +
                    fila(T.cuit, d.cuit) +
                    fila(T.rubro, d.rubro) +
                    fila(T.domicilio, d.domicilio) +
                    fila(T.contacto, d.contacto + ' &mdash; ' + d.contactoemail) +
                    fila(T.estado, badgeEstado(d.estado)) +
                    fila(T.alta, d.alta) +
                    '</tbody></table>';
            }

            function panelPlan(d) {
                return '<table class="emp-datos"><tbody>' +
                    fila(T.plan, d.plan) +
                    fila(T.factura, d.factura) +
                    fila(T.renovacion, d.renovacion) +
                    fila(T.pago, d.pago) +
                    fila(T.limite, d.limite + ' (' + d.usuarios + ' en uso)') +
                    '</tbody></table>';
            }

            function panelEquipo(d) {
                return '<table class="emp-datos"><tbody>' +
                    fila(T.usuarios, d.usuarios) +
                    fila(T.dispositivos, d.dispositivos) +
                    '</tbody></table>' +
                    '<div class="emp-acciones"><a class="btn btn-secundario" href="Dispositivos.aspx">' +
                    T.dispositivos + '</a></div>';
            }

            function panelSesiones(d) {
                return '<div class="metricas">' +
                    '<div><div class="metrica-valor">' + d.sesiones + '</div><div class="metrica-nombre">' + T.sesiones + '</div></div>' +
                    '<div><div class="metrica-valor">' + d.usuarios + '</div><div class="metrica-nombre">' + T.usuarios + '</div></div>' +
                    '<div><div class="metrica-valor">' + d.dispositivos + '</div><div class="metrica-nombre">' + T.dispositivos + '</div></div>' +
                    '</div>';
            }

            function mostrar(f) {
                for (var i = 0; i < filas.length; i++) filas[i].removeAttribute("aria-current");
                f.setAttribute("aria-current", "true");

                var d = f.dataset;
                var aviso = d.vencido === "1"
                    ? '<div class="aviso aviso-peligro mb-24"><svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>' +
                      '<p class="sin-margen">' + T.vencidoAviso + '</p></div>'
                    : '';

                var accionEstado = d.estado === "Bloqueada"
                    ? '<button type="button" class="btn btn-primario">' + T.desbloquear + '</button>'
                    : '<button type="button" class="btn btn-peligro">' + T.bloquear + '</button>';

                detalle.innerHTML =
                    '<div class="emp-detalle-cab">' +
                        '<span class="sigla" aria-hidden="true">' + d.sigla + '</span>' +
                        '<div><h2>' + d.nombre + '</h2><span class="cuit">' + d.cuit + '</span></div>' +
                        '<span class="marcas"><span class="badge badge-alerta">' + d.plan + '</span>' + badgeEstado(d.estado) + '</span>' +
                    '</div>' +
                    aviso +
                    '<div class="pestanas" role="tablist">' +
                        '<button type="button" class="pestana" role="tab" aria-selected="true" data-pestana="tabDatos">' + T.tabDatos + '</button>' +
                        '<button type="button" class="pestana" role="tab" aria-selected="false" data-pestana="tabPlan">' + T.tabPlan + '</button>' +
                        '<button type="button" class="pestana" role="tab" aria-selected="false" data-pestana="tabEquipo">' + T.tabEquipo + '</button>' +
                        '<button type="button" class="pestana" role="tab" aria-selected="false" data-pestana="tabSesiones">' + T.tabSesiones + '</button>' +
                    '</div>' +
                    '<div id="tabDatos" role="tabpanel">' + panelDatos(d) + '</div>' +
                    '<div id="tabPlan" role="tabpanel" hidden>' + panelPlan(d) + '</div>' +
                    '<div id="tabEquipo" role="tabpanel" hidden>' + panelEquipo(d) + '</div>' +
                    '<div id="tabSesiones" role="tabpanel" hidden>' + panelSesiones(d) + '</div>' +
                    '<div class="emp-acciones">' +
                        '<a class="btn btn-secundario" href="Usuarios.aspx">' + T.verUsuarios + '</a>' +
                        '<button type="button" class="btn btn-secundario">' + T.editar + '</button>' +
                        accionEstado +
                    '</div>';

                var pestanas = detalle.querySelectorAll(".pestana");
                for (var p = 0; p < pestanas.length; p++) {
                    pestanas[p].addEventListener("click", function () {
                        var destino = this.getAttribute("data-pestana");
                        for (var q = 0; q < pestanas.length; q++) {
                            var esta = pestanas[q] === this;
                            pestanas[q].setAttribute("aria-selected", esta ? "true" : "false");
                            document.getElementById(pestanas[q].getAttribute("data-pestana")).hidden = !esta;
                        }
                    });
                }
            }

            for (var i = 0; i < filas.length; i++) {
                filas[i].addEventListener("click", function () { mostrar(this); });
            }

            function filtrar() {
                var texto = buscar.value.trim().toLowerCase();
                var visibles = 0;
                for (var j = 0; j < filas.length; j++) {
                    var f = filas[j];
                    var busca = (f.dataset.nombre + " " + f.dataset.cuit + " " + f.dataset.rubro).toLowerCase();
                    var okTexto = texto === "" || busca.indexOf(texto) !== -1;
                    var okEstado = estadoFiltro === "" || f.dataset.estado === estadoFiltro;
                    var ver = okTexto && okEstado;
                    f.hidden = !ver;
                    if (ver) visibles++;
                }
                vacio.hidden = visibles > 0;
                cuenta.textContent = visibles;
            }

            buscar.addEventListener("input", filtrar);

            for (i = 0; i < chips.length; i++) {
                chips[i].addEventListener("click", function () {
                    for (var k = 0; k < chips.length; k++) {
                        chips[k].setAttribute("aria-pressed", chips[k] === this ? "true" : "false");
                    }
                    estadoFiltro = this.getAttribute("data-estado-filtro");
                    filtrar();
                });
            }

            mostrar(filas[0]);
        })();
    </script>
</asp:Content>
