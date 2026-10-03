<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Integridad.aspx.cs" Inherits="GUI.Integridad" Title="Integridad de la base de datos" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        .int-cabecera
        {
            display: flex;
            align-items: flex-start;
            gap: 18px;
            margin-bottom: 24px;
        }

        .int-cabecera-icono
        {
            flex: none;
            margin-top: 4px;
            color: var(--acento-texto);
        }

        .int-cabecera h1
        {
            font-size: 1.5rem;
            margin: 0 0 6px;
        }

        .int-cabecera p
        {
            color: var(--texto-suave);
            max-width: 70ch;
            margin: 0;
            font-size: .92rem;
        }

        .int-sello
        {
            margin-left: auto;
            flex: none;
            display: inline-flex;
            align-items: center;
            gap: 7px;
            padding: 4px 10px;
            border-radius: var(--radio-sm);
            border: 1px solid var(--borde-fuerte);
            color: var(--texto-suave);
            font-size: .82rem;
            font-weight: 600;
        }

        .int-metricas
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 14px;
            margin: 22px 0 32px;
        }

        .int-metrica
        {
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
            padding: 16px 18px;
        }

        .int-metrica.alerta
        {
            border-color: var(--peligro);
            background: var(--peligro-suave);
        }

        .int-metrica .rotulo
        {
            font-size: .82rem;
            color: var(--texto-suave);
        }

        .int-metrica .dato
        {
            font-family: var(--fuente-mono);
            font-size: 1.5rem;
            font-weight: 700;
            line-height: 1.1;
            margin: 8px 0 4px;
        }

        .int-metrica.alerta .dato
        {
            color: var(--peligro);
        }

        .int-metrica .sub
        {
            font-size: .82rem;
            color: var(--texto-suave);
        }

        .int-seccion-cab
        {
            display: flex;
            align-items: flex-end;
            justify-content: space-between;
            gap: 16px;
            flex-wrap: wrap;
            margin-bottom: 16px;
        }

        .int-seccion-cab h2
        {
            margin: 0 0 4px;
        }

        .int-seccion-cab p
        {
            margin: 0;
            color: var(--texto-suave);
            font-size: .88rem;
        }

        .int-celda-tabla
        {
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .int-tile
        {
            flex: none;
            display: flex;
            color: var(--exito);
        }

        .int-tile.mal
        {
            color: var(--peligro);
        }

        .int-celda-tabla .nombre
        {
            font-weight: 600;
        }

        .int-detalle.mal
        {
            color: var(--peligro);
        }

        .int-tabla td
        {
            vertical-align: middle;
        }

        .int-tabla .col-detalle
        {
            max-width: 380px;
            font-size: .86rem;
        }

        #detIntCuerpo td
        {
            font-size: .82rem;
        }

        @media (max-width: 720px)
        {
            .int-cabecera
            {
                flex-wrap: wrap;
            }

            .int-sello
            {
                margin-left: 0;
            }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="int-cabecera">
        <span class="int-cabecera-icono" aria-hidden="true">
            <svg width="24" height="24"><use href="#i-escudo" /></svg>
        </span>
        <div>
            <h1 data-i18n="integridad.titulo">Integridad de la base de datos</h1>
            <p data-i18n="integridad.bajada">
                Verificación mediante dígitos verificadores (DVH por registro y DVV por tabla)
                sobre las tablas bajo control de integridad. Se recalcula automáticamente al
                iniciar sesión y puede recalcularse manualmente ante una inconsistencia detectada.
            </p>
        </div>
        <span class="int-sello">
            <svg width="13" height="13" aria-hidden="true"><use href="#i-candado" /></svg>
            <span data-i18n="integridad.sello">Uso exclusivo Pattern Blue</span>
        </span>
    </div>

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso mb-24" Visible="false">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <asp:Panel ID="pnlAlertaProblema" runat="server" CssClass="aviso aviso-peligro" role="status" Visible="false">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
        <div>
            <strong><asp:Literal ID="litResumenAlerta" runat="server" /></strong>
            <p class="sin-margen" data-i18n="integridad.alerta.texto">
                El acceso al sistema permanece bloqueado para el resto de los usuarios hasta
                ejecutar el recálculo completo. Solo el webmaster puede ver el detalle técnico
                de las tablas y registros afectados.
            </p>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlAlertaOk" runat="server" CssClass="aviso aviso-exito" role="status" Visible="false">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-tilde" /></svg>
        <p class="sin-margen" data-i18n="integridad.alerta.ok">
            La verificación no encontró inconsistencias: los datos coinciden con los dígitos
            verificadores almacenados.
        </p>
    </asp:Panel>

    <div class="int-metricas">
        <div class="int-metrica">
            <div class="rotulo" data-i18n="integridad.metrica.controladas">Tablas bajo control</div>
            <div class="dato"><asp:Literal ID="litTotalTablas" runat="server" Text="0" /></div>
            <div class="sub" data-i18n="integridad.metrica.controladas.sub">Verificadas en cada inicio de sesión</div>
        </div>
        <div class="int-metrica">
            <div class="rotulo" data-i18n="integridad.metrica.integras">Tablas íntegras</div>
            <div class="dato"><asp:Literal ID="litIntegras" runat="server" Text="0" /></div>
            <div class="sub" data-i18n="integridad.metrica.integras.sub">DVV y DVH coinciden</div>
        </div>
        <div class="int-metrica" runat="server" id="cardConProblemas">
            <div class="rotulo" data-i18n="integridad.metrica.inconsistentes">Tablas con inconsistencias</div>
            <div class="dato"><asp:Literal ID="litConProblemas" runat="server" Text="0" /></div>
            <div class="sub" data-i18n="integridad.metrica.inconsistentes.sub">Requieren recálculo</div>
        </div>
        <div class="int-metrica" runat="server" id="cardRegistros">
            <div class="rotulo" data-i18n="integridad.metrica.afectados">Registros alterados</div>
            <div class="dato"><asp:Literal ID="litRegistrosAfectados" runat="server" Text="0" /></div>
            <div class="sub" data-i18n="integridad.metrica.afectados.sub">Con DVH que no coincide</div>
        </div>
    </div>

    <div class="int-seccion-cab">
        <div>
            <h2 data-i18n="integridad.porTabla">Estado de integridad por tabla</h2>
            <p data-i18n="integridad.porTabla.ayuda">
                Dígito verificador horizontal (DVH) por registro y vertical (DVV) por tabla,
                según SHA-256 encadenado.
            </p>
        </div>
        <button type="button" class="btn btn-peligro" data-abre-modal="modalRecalcular">
            <svg width="16" height="16" aria-hidden="true"><use href="#i-refrescar" /></svg>
            <span data-i18n="integridad.recalcular">Recalcular dígitos verificadores</span>
        </button>
    </div>

    <div class="tabla-scroll">
        <table class="tabla int-tabla">
            <caption class="solo-lectores" data-i18n="integridad.tabla.caption">Estado de integridad de cada tabla controlada</caption>
            <thead>
                <tr>
                    <th scope="col" data-i18n="integridad.col.tabla">Tabla</th>
                    <th scope="col" data-i18n="integridad.col.registros">Registros</th>
                    <th scope="col" data-i18n="integridad.col.estado">Estado</th>
                    <th scope="col" data-i18n="integridad.col.detalle">Detalle</th>
                    <th scope="col"><span class="solo-lectores" data-i18n="comun.acciones">Acciones</span></th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptTablas" runat="server">
                    <ItemTemplate>
                        <tr>
                            <th scope="row">
                                <span class="int-celda-tabla">
                                    <span class='<%# Eval("ClaseTile") %>' aria-hidden="true"><svg width="17" height="17"><use href="#i-base" /></svg></span>
                                    <span class="nombre"><%# Eval("Nombre") %></span>
                                </span>
                            </th>
                            <td class="mono"><%# Eval("Registros") %></td>
                            <td><%# Eval("BadgeHtml") %></td>
                            <td class='<%# Eval("ClaseDetalle") %>'><%# Eval("DetalleHtml") %></td>
                            <td>
                                <button type="button" class="btn btn-fantasma btn-chico js-ver-detalle"
                                        data-abre-modal="modalDetalleTabla" data-tabla='<%# Eval("Nombre") %>' <%# Eval("AtributoDetalle") %>>
                                    <svg width="15" height="15" aria-hidden="true"><use href="#i-ojo" /></svg>
                                    <span data-i18n="integridad.verDetalle">Ver detalle</span>
                                </button>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>

    <div class="aviso aviso-alerta mt-24">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-escudo" /></svg>
        <p class="sin-margen" data-i18n="integridad.avisoRecalculo">
            Recalcular los dígitos hace que el sistema vuelva a aceptar los datos tal como
            están hoy. Antes de hacerlo conviene revisar los registros marcados y, si
            corresponde, restaurar un respaldo: recalcular sobre datos adulterados deja el
            problema adentro.
        </p>
    </div>

    <div class="modal-fondo" id="modalDetalleTabla" role="dialog" aria-modal="true" aria-labelledby="detIntTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="detIntTitulo" data-i18n="integridad.detalle.titulo">Registros observados por el control de integridad</h2>
                    <p class="subtitulo sin-margen mono" id="detIntTabla"></p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <p class="texto-suave" id="detIntResumen"></p>
                <div class="tabla-scroll">
                    <table class="tabla">
                        <caption class="solo-lectores" data-i18n="integridad.detalle.caption">Registros con dígito verificador incorrecto</caption>
                        <thead>
                            <tr>
                                <th scope="col" data-i18n="integridad.col.numero">Registro n.&deg;</th>
                                <th scope="col" data-i18n="integridad.col.clave">Clave (PK)</th>
                                <th scope="col" data-i18n="integridad.col.dvhGuardado">DVH guardado</th>
                                <th scope="col" data-i18n="integridad.col.dvhCalculado">DVH calculado</th>
                                <th scope="col" data-i18n="integridad.col.datos">Datos del registro</th>
                            </tr>
                        </thead>
                        <tbody id="detIntCuerpo"></tbody>
                    </table>
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cerrar">Cerrar</button>
            </div>
        </div>
    </div>

    <div class="modal-fondo" id="modalRecalcular" role="dialog" aria-modal="true" aria-labelledby="recalcTitulo" hidden>
        <div class="modal">
            <div class="modal-cabecera">
                <div>
                    <h2 id="recalcTitulo" data-i18n="recalcular.titulo">Recalcular los dígitos verificadores</h2>
                    <p class="subtitulo sin-margen" data-i18n="recalcular.subtitulo">Revisá bien antes de confirmar.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>
            <div class="modal-cuerpo">
                <div class="aviso aviso-peligro">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen" data-i18n="recalcular.aviso">
                        Al recalcular, los datos actuales pasan a considerarse correctos y se
                        pierde la evidencia de que algo fue modificado por fuera del sistema.
                    </p>
                </div>
                <div class="campo">
                    <label for="ddlAlcance" data-i18n="recalcular.alcance">Alcance</label>
                    <asp:DropDownList ID="ddlAlcance" runat="server" CssClass="entrada" ClientIDMode="Static" data-foco-inicial="si">
                        <asp:ListItem Value="problemas">Solo las tablas con inconsistencias</asp:ListItem>
                        <asp:ListItem Value="todas">Todas las tablas bajo control</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="campo">
                    <label for="txtMotivo" data-i18n="recalcular.motivo">Motivo</label>
                    <asp:TextBox ID="txtMotivo" runat="server" CssClass="entrada" TextMode="MultiLine" Rows="3" ClientIDMode="Static"
                                 placeholder="Queda registrado en la bitácora"
                                 data-i18n-attr="placeholder:recalcular.motivo.placeholder" />
                </div>
            </div>
            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <asp:Button ID="btnRecalcular" runat="server" CssClass="btn btn-peligro" OnClick="btnRecalcular_Click"
                            Text="Recalcular ahora" data-i18n="recalcular.confirmar" />
            </div>
        </div>
    </div>

    <script type="application/json" id="intDetalleJson"><asp:Literal ID="litDetalleJson" runat="server" Text="{}" /></script>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var datos = {};
            try {
                datos = JSON.parse(document.getElementById("intDetalleJson").textContent || "{}");
            } catch (e) {
                datos = {};
            }

            var rotulo = document.getElementById("detIntTabla");
            var resumen = document.getElementById("detIntResumen");
            var cuerpo = document.getElementById("detIntCuerpo");
            var botones = document.querySelectorAll(".js-ver-detalle");

            function esc(valor) {
                valor = valor == null ? "" : String(valor);
                return valor.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
                            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
            }

            function recorta(valor) {
                valor = valor == null ? "" : String(valor);
                return valor.length > 18 ? valor.slice(0, 18) + "…" : valor;
            }

            function pintar(tabla) {
                var info = datos[tabla];
                rotulo.textContent = tabla;

                if (!info) {
                    resumen.textContent = "";
                    cuerpo.innerHTML = "";
                    return;
                }

                resumen.textContent = info.resumen || "";

                var filas = info.registros || [];
                var html = "";

                for (var i = 0; i < filas.length; i++) {
                    var f = filas[i];
                    html += "<tr>" +
                        "<td class='mono'>" + (f.n == null ? "" : esc(f.n)) + "</td>" +
                        "<td class='mono'>" + esc(f.clave) + "</td>" +
                        "<td><span class='mono' title='" + esc(f.guardado) + "'>" + esc(recorta(f.guardado)) + "</span></td>" +
                        "<td><span class='mono texto-peligro' title='" + esc(f.calculado) + "'>" + esc(recorta(f.calculado)) + "</span></td>" +
                        "<td>" + esc(f.datos) + "</td>" +
                        "</tr>";
                }

                cuerpo.innerHTML = html;
            }

            for (var i = 0; i < botones.length; i++) {
                botones[i].addEventListener("click", function () {
                    pintar(this.getAttribute("data-tabla"));
                });
            }
        })();
    </script>
</asp:Content>
