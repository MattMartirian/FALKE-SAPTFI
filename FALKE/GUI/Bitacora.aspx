<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Bitacora.aspx.cs" Inherits="GUI.Bitacora" Title="Bitácora" %>
<%@ MasterType VirtualPath="~/App.master" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .fila-critica td:first-child::before,
        .fila-media td:first-child::before,
        .fila-baja td:first-child::before
        {
            content: "";
            display: inline-block;
            width: 8px;
            height: 8px;
            margin-right: 9px;
            border-radius: 50%;
            vertical-align: middle;
            background: var(--borde-fuerte);
        }

        .fila-critica td:first-child::before { background: var(--peligro); }
        .fila-media td:first-child::before { background: var(--alerta); }

        .filtros-bitacora
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(170px, 1fr));
            gap: 14px;
            align-items: end;
        }

        .filtros-bitacora .campo
        {
            margin-bottom: 0;
        }

        .celda-fecha
        {
            white-space: nowrap;
            font-variant-numeric: tabular-nums;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="bitacora.titulo">Bitácora</h1>
            <p class="texto-suave sin-margen">
                <asp:Literal ID="litBajada" runat="server" />
            </p>
        </div>
        <div class="acciones">

            <button type="button" class="btn btn-secundario" data-abre-modal="modalExportarBitacora">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-descarga" /></svg>
                <span data-i18n="bitacora.exportar">Exportar</span>
            </button>
        </div>
    </div>

    <div class="aviso aviso-info">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen">
            <asp:Literal ID="litAlcance" runat="server" />
        </p>
    </div>

    <div class="tarjeta mb-24">

        <div class="filtros-bitacora mb-16">

            <div class="campo">
                <label for="bitDesde" data-i18n="bitacora.desde">Desde</label>
                <input type="date" class="entrada" id="bitDesde" />
            </div>

            <div class="campo">
                <label for="bitHasta" data-i18n="bitacora.hasta">Hasta</label>
                <input type="date" class="entrada" id="bitHasta" />
            </div>

            <div class="campo">
                <label for="bitHoraDesde" data-i18n="bitacora.horaDesde">Hora desde</label>
                <input type="time" class="entrada" id="bitHoraDesde" />
            </div>

            <div class="campo">
                <label for="bitHoraHasta" data-i18n="bitacora.horaHasta">Hora hasta</label>
                <input type="time" class="entrada" id="bitHoraHasta" />
            </div>

            <div class="campo">
                <label for="bitModulo" data-i18n="bitacora.modulo">Módulo</label>
                <select class="entrada" id="bitModulo">
                    <option value="" data-i18n="comun.todos">Todos</option>
                    <option value="Seguridad" data-i18n="bitacora.modulo.seguridad">Seguridad</option>
                    <option value="Usuarios" data-i18n="bitacora.modulo.usuarios">Usuarios</option>
                    <option value="Empresas" data-i18n="bitacora.modulo.empresas">Empresas</option>
                    <option value="Sesiones" data-i18n="bitacora.modulo.sesiones">Sesiones</option>
                    <option value="Analisis" data-i18n="bitacora.modulo.analisis">Análisis</option>
                    <option value="Sistema" data-i18n="bitacora.modulo.sistema">Sistema</option>
                </select>
            </div>

            <div class="campo">
                <label for="bitAccion" data-i18n="bitacora.accion">Acción</label>
                <select class="entrada" id="bitAccion">
                    <option value="" data-i18n="comun.todas">Todas</option>
                    <option value="Inicio de sesion">Inicio de sesión</option>
                    <option value="Cierre de sesion">Cierre de sesión</option>
                    <option value="Alta">Alta</option>
                    <option value="Modificacion">Modificación</option>
                    <option value="Baja">Baja</option>
                    <option value="Exportacion">Exportación</option>
                </select>
            </div>

            <div class="campo">
                <label for="bitUsuario" data-i18n="bitacora.usuario">Usuario</label>
                <input type="search" class="entrada" id="bitUsuario"
                       placeholder="Nombre o email..."
                       data-i18n-attr="placeholder:bitacora.usuario.placeholder" />
            </div>

            <asp:PlaceHolder ID="phFiltroEmpresa" runat="server">
                <div class="campo">
                    <label for="bitEmpresa" data-i18n="bitacora.empresa">Empresa</label>
                    <select class="entrada" id="bitEmpresa">
                        <option value="" data-i18n="comun.todas">Todas</option>
                        <option value="Pattern Blue">Pattern Blue</option>
                        <option value="Ironhide Game Studio">Ironhide Game Studio</option>
                        <option value="Mercado Cruz S.A.">Mercado Cruz S.A.</option>
                        <option value="Nova Publicidad">Nova Publicidad</option>
                        <option value="Austral Seguros">Austral Seguros</option>
                    </select>
                </div>
            </asp:PlaceHolder>

            <div class="campo">
                <label for="bitCriticidad" data-i18n="bitacora.criticidad">Criticidad</label>
                <select class="entrada" id="bitCriticidad">
                    <option value="" data-i18n="comun.todas">Todas</option>
                    <option value="Alta" data-i18n="criticidad.alta">Alta</option>
                    <option value="Media" data-i18n="criticidad.media">Media</option>
                    <option value="Baja" data-i18n="criticidad.baja">Baja</option>
                </select>
            </div>

        </div>

        <div class="fila-entre">
            <p class="texto-chico texto-suave sin-margen" id="bitContador" role="status">

                Mostrando 10 de 10 registros.
            </p>
            <button type="button" class="btn btn-secundario btn-chico" id="btnLimpiarBitacora"
                    data-i18n="comun.limpiarFiltros">Limpiar filtros</button>
        </div>

    </div>

    <div class="tabla-scroll">
        <table class="tabla" id="tablaBitacora">
            <caption class="solo-lectores">Registros de la bitácora</caption>
            <thead>
                <tr>
                    <th scope="col" data-i18n="bitacora.col.fecha">Fecha y hora</th>
                    <th scope="col" data-i18n="bitacora.col.usuario">Usuario</th>
                    <th scope="col" data-i18n="bitacora.col.empresa">Empresa</th>
                    <th scope="col" data-i18n="bitacora.col.modulo">Módulo</th>
                    <th scope="col" data-i18n="bitacora.col.descripcion">Descripción</th>
                    <th scope="col" data-i18n="bitacora.col.criticidad">Criticidad</th>
                </tr>
            </thead>
            <tbody>

                <tr class="fila-critica" data-modulo="Sistema" data-accion="Modificacion" data-empresa="Pattern Blue" data-criticidad="Alta">
                    <td class="celda-fecha">08/09/2026 14:41</td>
                    <td>Ramiro Diaz</td>
                    <td class="texto-suave">Pattern Blue</td>
                    <td><span class="badge badge-neutro">Sistema</span></td>
                    <td>Recalculo de digitos verificadores sobre la tabla Usuario</td>
                    <td><span class="badge badge-peligro" data-i18n="criticidad.alta">Alta</span></td>
                </tr>

                <tr class="fila-baja" data-modulo="Seguridad" data-accion="Inicio de sesion" data-empresa="Ironhide Game Studio" data-criticidad="Baja">
                    <td class="celda-fecha">08/09/2026 14:32</td>
                    <td>Julieta Fernandez</td>
                    <td class="texto-suave">Ironhide Game Studio</td>
                    <td><span class="badge badge-neutro">Seguridad</span></td>
                    <td>Inicio de sesión correcto</td>
                    <td><span class="badge badge-neutro" data-i18n="criticidad.baja">Baja</span></td>
                </tr>

                <tr class="fila-media" data-modulo="Analisis" data-accion="Exportacion" data-empresa="Ironhide Game Studio" data-criticidad="Media">
                    <td class="celda-fecha">08/09/2026 14:05</td>
                    <td>Maria Gomez</td>
                    <td class="texto-suave">Ironhide Game Studio</td>
                    <td><span class="badge badge-neutro">Análisis</span></td>
                    <td>Exportación del reporte de la sesión Onboarding v3</td>
                    <td><span class="badge badge-alerta" data-i18n="criticidad.media">Media</span></td>
                </tr>

                <tr class="fila-critica" data-modulo="Usuarios" data-accion="Modificacion" data-empresa="Ironhide Game Studio" data-criticidad="Alta">
                    <td class="celda-fecha">08/09/2026 11:20</td>
                    <td>Julieta Fernandez</td>
                    <td class="texto-suave">Ironhide Game Studio</td>
                    <td><span class="badge badge-neutro">Usuarios</span></td>
                    <td>Bloqueo de la cuenta sofia.lopez@ironhide.com</td>
                    <td><span class="badge badge-peligro" data-i18n="criticidad.alta">Alta</span></td>
                </tr>

                <tr class="fila-media" data-modulo="Seguridad" data-accion="Inicio de sesion" data-empresa="Ironhide Game Studio" data-criticidad="Media">
                    <td class="celda-fecha">08/09/2026 11:02</td>
                    <td>Sofia Lopez</td>
                    <td class="texto-suave">Ironhide Game Studio</td>
                    <td><span class="badge badge-neutro">Seguridad</span></td>
                    <td>Cuenta bloqueada por 5 intentos fallidos</td>
                    <td><span class="badge badge-alerta" data-i18n="criticidad.media">Media</span></td>
                </tr>

                <tr class="fila-baja" data-modulo="Sesiones" data-accion="Alta" data-empresa="Ironhide Game Studio" data-criticidad="Baja">
                    <td class="celda-fecha">08/09/2026 10:47</td>
                    <td>Maria Gomez</td>
                    <td class="texto-suave">Ironhide Game Studio</td>
                    <td><span class="badge badge-neutro">Sesiones</span></td>
                    <td>Alta de la sesión Nivel 4 - tutorial (tester T-042)</td>
                    <td><span class="badge badge-neutro" data-i18n="criticidad.baja">Baja</span></td>
                </tr>

                <tr class="fila-critica" data-modulo="Empresas" data-accion="Alta" data-empresa="Pattern Blue" data-criticidad="Alta">
                    <td class="celda-fecha">07/09/2026 17:12</td>
                    <td>Ramiro Diaz</td>
                    <td class="texto-suave">Pattern Blue</td>
                    <td><span class="badge badge-neutro">Empresas</span></td>
                    <td>Alta de la empresa Austral Seguros y de su administrador</td>
                    <td><span class="badge badge-peligro" data-i18n="criticidad.alta">Alta</span></td>
                </tr>

                <tr class="fila-critica" data-modulo="Sistema" data-accion="Modificacion" data-empresa="Pattern Blue" data-criticidad="Alta">
                    <td class="celda-fecha">06/09/2026 19:00</td>
                    <td>Ramiro Diaz</td>
                    <td class="texto-suave">Pattern Blue</td>
                    <td><span class="badge badge-neutro">Sistema</span></td>
                    <td>Sistema puesto en mantenimiento (actualización 1.4)</td>
                    <td><span class="badge badge-peligro" data-i18n="criticidad.alta">Alta</span></td>
                </tr>

                <tr class="fila-media" data-modulo="Usuarios" data-accion="Alta" data-empresa="Mercado Cruz S.A." data-criticidad="Media">
                    <td class="celda-fecha">06/09/2026 15:33</td>
                    <td>Pablo Herrera</td>
                    <td class="texto-suave">Mercado Cruz S.A.</td>
                    <td><span class="badge badge-neutro">Usuarios</span></td>
                    <td>Invitación enviada a laura.vega@mercadocruz.com</td>
                    <td><span class="badge badge-alerta" data-i18n="criticidad.media">Media</span></td>
                </tr>

                <tr class="fila-baja" data-modulo="Seguridad" data-accion="Cierre de sesion" data-empresa="Nova Publicidad" data-criticidad="Baja">
                    <td class="celda-fecha">06/09/2026 12:18</td>
                    <td>Carla Suarez</td>
                    <td class="texto-suave">Nova Publicidad</td>
                    <td><span class="badge badge-neutro">Seguridad</span></td>
                    <td>Cierre de sesión</td>
                    <td><span class="badge badge-neutro" data-i18n="criticidad.baja">Baja</span></td>
                </tr>

            </tbody>
        </table>
    </div>

    <div class="vacio" id="bitacoraVacio" hidden>
        <svg width="34" height="34" aria-hidden="true"><use href="#i-libro" /></svg>
        <p class="sin-margen" data-i18n="bitacora.vacio">No hay registros que coincidan con los filtros.</p>
    </div>

    <p class="texto-chico texto-suave mt-16" data-i18n="bitacora.retencion">
        Los registros de la bitácora no se pueden editar ni borrar. Se conservan por el
        plazo que fije la política de retención del sistema.
    </p>

    <div class="modal-fondo" id="modalExportarBitacora" role="dialog" aria-modal="true" aria-labelledby="expBitTitulo" hidden>
        <div class="modal">

            <div class="modal-cabecera">
                <div>
                    <h2 id="expBitTitulo" data-i18n="bitacora.exportar.titulo">Exportar la bitácora</h2>
                    <p class="subtitulo sin-margen" data-i18n="bitacora.exportar.subtitulo">
                        Se exporta lo que quedó después de aplicar los filtros.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <p class="etiqueta" data-i18n="bitacora.exportar.formato">Formato</p>

                <label class="opcion">
                    <input type="radio" name="formatoBitacora" value="csv" checked data-foco-inicial />
                    <span>
                        <span class="opcion-titulo">CSV</span>
                        <span class="opcion-detalle" data-i18n="bitacora.exportar.csv">Para abrir en una planilla de cálculo.</span>
                    </span>
                </label>

                <label class="opcion">
                    <input type="radio" name="formatoBitacora" value="pdf" />
                    <span>
                        <span class="opcion-titulo">PDF</span>
                        <span class="opcion-detalle" data-i18n="bitacora.exportar.pdf">Para archivar o presentar como evidencia.</span>
                    </span>
                </label>

                <div class="aviso aviso-info mt-16 mb-0">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-info" /></svg>
                    <p class="sin-margen" data-i18n="bitacora.exportar.aviso">
                        La exportación también queda registrada en la bitácora.
                    </p>
                </div>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <button type="button" class="btn btn-primario" data-i18n="bitacora.exportar.confirmar">Exportar</button>
            </div>

        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var modulo = document.getElementById("bitModulo");
            var accion = document.getElementById("bitAccion");
            var usuario = document.getElementById("bitUsuario");
            var criticidad = document.getElementById("bitCriticidad");
            var empresa = document.getElementById("bitEmpresa");
            var contador = document.getElementById("bitContador");
            var vacio = document.getElementById("bitacoraVacio");
            var filas = document.querySelectorAll("#tablaBitacora tbody tr");
            var botonLimpiar = document.getElementById("btnLimpiarBitacora");

            function filtrar() {
                var texto = usuario.value.trim().toLowerCase();
                var visibles = 0;

                for (var i = 0; i < filas.length; i++)
                {
                    var fila = filas[i];
                    var okTexto = texto === "" || fila.cells[1].textContent.toLowerCase().indexOf(texto) !== -1;
                    var okModulo = modulo.value === "" || fila.getAttribute("data-modulo") === modulo.value;
                    var okAccion = accion.value === "" || fila.getAttribute("data-accion") === accion.value;
                    var okCritic = criticidad.value === "" || fila.getAttribute("data-criticidad") === criticidad.value;
                    var okEmpresa = !empresa || empresa.value === "" || fila.getAttribute("data-empresa") === empresa.value;
                    var mostrar = okTexto && okModulo && okAccion && okCritic && okEmpresa;

                    fila.hidden = !mostrar;

                    if (mostrar) visibles++;
                }

                contador.textContent = "Mostrando " + visibles + " de " + filas.length + " registros.";
                vacio.hidden = visibles > 0;
            }

            modulo.addEventListener("change", filtrar);
            accion.addEventListener("change", filtrar);
            criticidad.addEventListener("change", filtrar);
            usuario.addEventListener("input", filtrar);

            if (empresa) empresa.addEventListener("change", filtrar);

            botonLimpiar.addEventListener("click", function () {
                var campos = document.querySelectorAll(".filtros-bitacora .entrada");

                for (var i = 0; i < campos.length; i++)
                    campos[i].value = "";

                filtrar();
            });
        })();
    </script>
</asp:Content>
