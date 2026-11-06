<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Panel.aspx.cs" Inherits="GUI.PaginaPrincipal" Title="[[app_pagina_principal]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .saludo h1
        {
            font-size: 1.7rem;
            margin-bottom: 4px;
        }

        .accesos
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(210px, 1fr));
            margin-bottom: 20px;
            border-block: 1px solid var(--borde-fuerte);
        }

        .acceso
        {
            display: block;
            padding: 16px 20px 16px 0;
            color: var(--texto);
        }

        .acceso + .acceso
        {
            padding-left: 20px;
            border-left: 1px solid var(--borde);
        }

        .acceso:hover
        {
            text-decoration: none;
        }

        .acceso:hover .titulo
        {
            color: var(--acento-texto);
            text-decoration: underline;
        }

        .acceso .titulo
        {
            font-weight: 700;
            display: block;
        }

        .acceso .detalle
        {
            font-size: .82rem;
            color: var(--texto-suave);
        }

        .tira-metricas
        {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(190px, 1fr));
            margin-bottom: 24px;
            background: var(--superficie);
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
        }

        .tira-metricas > div
        {
            padding: 16px 20px;
        }

        .tira-metricas > div + div
        {
            border-left: 1px solid var(--borde);
        }

        @media (max-width: 700px)
        {
            .acceso + .acceso
            {
                padding-left: 0;
                border-left: 0;
                border-top: 1px solid var(--borde);
            }

            .tira-metricas > div + div
            {
                border-left: 0;
                border-top: 1px solid var(--borde);
            }
        }

        .panel-columnas
        {
            display: grid;
            grid-template-columns: minmax(0, 1.7fr) minmax(0, 1fr) minmax(0, 1.1fr);
            gap: 18px;
            align-items: stretch;
        }

        @media (max-width: 1280px)
        {
            .panel-columnas { grid-template-columns: minmax(0, 1fr); }
        }

        .panel-columnas .tabla td:nth-child(3),
        .panel-columnas .tabla td:nth-child(4)
        {
            white-space: nowrap;
        }

        .por-categoria
        {
            list-style: none;
            margin: 0;
            padding: 0;
            display: grid;
            gap: 14px;
        }

        .por-categoria .fila-entre
        {
            margin-bottom: 6px;
            font-size: .86rem;
        }

        .por-categoria .nombre
        {
            min-width: 0;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .actividad
        {
            list-style: none;
            margin: 0;
            padding: 0;
        }

        .actividad li
        {
            display: flex;
            gap: 11px;
            padding: 10px 0;
            border-bottom: 1px solid var(--borde);
            font-size: .86rem;
        }

        .actividad li:first-child
        {
            padding-top: 0;
        }

        .actividad li:last-child
        {
            border-bottom: 0;
            padding-bottom: 0;
        }

        .actividad svg
        {
            flex: none;
            margin-top: 2px;
            color: var(--texto-suave);
        }

        .actividad li > span
        {
            min-width: 0;
        }

        .actividad .detalle-actividad
        {
            display: block;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            color: var(--texto-suave);
            font-size: .8rem;
        }

        .actividad .cuando
        {
            display: block;
            font-size: .76rem;
            color: var(--texto-tenue);
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera saludo">
        <div>

            <h1><span data-i18n="panel.saludo">[[panel_hola]]</span>, <asp:Literal ID="litNombre" runat="server" /></h1>
            <p class="texto-suave sin-margen" data-i18n="panel.bajada">
                [[panel_este_es_el_resumen_de_la]]
            </p>
        </div>
        <div class="acciones">
            <a class="btn btn-primario" href="<%: ResolveUrl("~/NuevaGrabacion.aspx") %>">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-video" /></svg>
                <span data-i18n="panel.nuevaGrabacion">[[app_nueva_grabacion]]</span>
            </a>
        </div>
    </div>

    <nav class="accesos" aria-label="[[panel_accesos_rapidos]]">

        <a class="acceso" href="<%: ResolveUrl("~/NuevaGrabacion.aspx") %>">
            <span>
                <span class="titulo" data-i18n="panel.acceso1.titulo">[[panel_grabar_una_sesion]]</span>
                <span class="detalle" data-i18n="panel.acceso1.detalle">[[panel_calibrar_el_dispositivo_y_empezar]]</span>
            </span>
        </a>

        <a class="acceso" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>">
            <span>
                <span class="titulo" data-i18n="panel.acceso2.titulo">[[panel_auditar_resultados]]</span>
                <span class="detalle" data-i18n="panel.acceso2.detalle">[[panel_mapas_zonas_y_recorridos]]</span>
            </span>
        </a>

        <a class="acceso" href="<%: ResolveUrl("~/AnalisisMultiple.aspx") %>">
            <span>
                <span class="titulo" data-i18n="panel.acceso3.titulo">[[panel_comparar_sesiones]]</span>
                <span class="detalle" data-i18n="panel.acceso3.detalle">[[panel_promediar_varios_testers]]</span>
            </span>
        </a>

        <a class="acceso" href="<%: ResolveUrl("~/Categorias.aspx") %>">
            <span>
                <span class="titulo" data-i18n="panel.acceso4.titulo">[[app_categorias]]</span>
                <span class="detalle" data-i18n="panel.acceso4.detalle">[[panel_organizar_los_activos_digitales]]</span>
            </span>
        </a>

    </nav>

    <div class="tira-metricas">

        <div>
            <div class="metrica-nombre" data-i18n="panel.metrica1">[[panel_sesiones_este_mes]]</div>
            <div class="metrica-valor"><%: SesionesMes %></div>
            <p class="texto-chico <%: SesionesMesClase %> sin-margen mt-8"><%: SesionesMesDetalle %></p>
        </div>

        <div>
            <div class="metrica-nombre">[[panel_tiempo_grabado_este_mes]]</div>
            <div class="metrica-valor"><%: TiempoMes %></div>
            <p class="texto-chico texto-suave sin-margen mt-8"><%: TiempoMesDetalle %></p>
        </div>

        <div>
            <div class="metrica-nombre" data-i18n="panel.metrica3">[[panel_categorias_activas]]</div>
            <div class="metrica-valor"><%: CategoriasActivas %></div>
            <p class="texto-chico texto-suave sin-margen mt-8"><%: CategoriasDetalle %></p>
        </div>

        <div>
            <div class="metrica-nombre" data-i18n="panel.metrica4">[[panel_dispositivos_asignados]]</div>
            <div class="metrica-valor"><%: Dispositivos %></div>
            <p class="texto-chico texto-suave sin-margen mt-8"><%: DispositivosDetalle %></p>
        </div>

    </div>

    <div class="panel-columnas">

        <section class="tarjeta">
            <div class="tarjeta-cabecera">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-metricas" /></svg>
                <span data-i18n="panel.ultimas.titulo">[[panel_ultimas_sesiones]]</span>
                <a class="derecha enlace-detalle" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>">
                    <span data-i18n="panel.ultimas.verTodas">[[panel_ver_todas]]</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </a>
            </div>

            <% if (HaySesiones) { %>
            <div class="tabla-scroll" style="border:0">
                <table class="tabla">
                    <caption class="solo-lectores">[[panel_ultimas_sesiones_grabadas]]</caption>
                    <thead>
                        <tr>
                            <th scope="col" data-i18n="panel.tabla.sesion">[[panel_sesion]]</th>
                            <th scope="col">[[panel_categoria]]</th>
                            <th scope="col" data-i18n="panel.tabla.fecha">[[panel_fecha]]</th>
                            <th scope="col" data-i18n="panel.tabla.duracion">[[panel_duracion]]</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptSesiones" runat="server" EnableViewState="false">
                            <ItemTemplate>
                                <tr>
                                    <td><a href="<%: ResolveUrl("~/Visualizaciones.aspx") %>"><%#: Eval("Nombre") %></a></td>
                                    <td><%#: Eval("Categoria") %></td>
                                    <td><%#: Eval("Fecha") %></td>
                                    <td><%#: Eval("Duracion") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
            <% } else { %>
            <p class="texto-chico texto-suave sin-margen"><%: Traducir("vis_sin_sesiones_empresa") %></p>
            <% } %>
        </section>

        <section class="tarjeta">
            <div class="tarjeta-cabecera">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-comparar" /></svg>
                <span>[[panel_sesiones_por_categoria]]</span>
                <a class="derecha enlace-detalle" href="<%: ResolveUrl("~/Categorias.aspx") %>">
                    <span>[[app_categorias]]</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </a>
            </div>

            <% if (HayCategoriasConSesiones) { %>
            <ul class="por-categoria">
                <asp:Repeater ID="rptCategorias" runat="server" EnableViewState="false">
                    <ItemTemplate>
                        <li>
                            <div class="fila-entre">
                                <span class="nombre"><%#: Eval("Nombre") %></span>
                                <strong><%#: Eval("Cantidad") %></strong>
                            </div>
                            <div class="barra" role="img" aria-label='<%#: Eval("Nombre") %>: <%#: Eval("Cantidad") %>'>
                                <span style='width: <%#: Eval("Porcentaje") %>%'></span>
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
            <% } else { %>
            <p class="texto-chico texto-suave sin-margen"><%: Traducir("vis_sin_sesiones_empresa") %></p>
            <% } %>
        </section>

        <section class="tarjeta">
            <div class="tarjeta-cabecera">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-reloj" /></svg>
                <span data-i18n="panel.actividad.titulo">[[panel_actividad_reciente]]</span>
            </div>

            <% if (HayActividad) { %>
            <ul class="actividad">
                <asp:Repeater ID="rptActividad" runat="server" EnableViewState="false">
                    <ItemTemplate>
                        <li>
                            <svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg>
                            <span>
                                <span><%#: GUI.TextoBitacora_GUI.Descripcion(Eval("DescripcionBitacora")) %></span>
                                <span class="detalle-actividad"><%#: DetalleActividad(Eval("DetalleBitacora")) %></span>
                                <span class="cuando"><%#: GUI.TextoBitacora_GUI.Modulo(Eval("ModuloBitacora")) %> &middot; <%#: ((DateTime)Eval("FechaHoraBitacora")).ToString("g", GUI.IdiomaActual_GUI.Cultura) %></span>
                            </span>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
            <% } else { %>
            <p class="texto-chico texto-suave sin-margen">[[miperfil_todavia_no_hay_movimientos_registrados_d]]</p>
            <% } %>
        </section>

    </div>

</asp:Content>
