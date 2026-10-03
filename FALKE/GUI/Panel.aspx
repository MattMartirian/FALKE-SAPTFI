<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Panel.aspx.cs" Inherits="GUI.PaginaPrincipal" Title="Página principal" %>

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
            grid-template-columns: minmax(0, 2fr) minmax(260px, 1fr);
            gap: 18px;
            align-items: start;
        }

        @media (max-width: 1000px)
        {
            .panel-columnas { grid-template-columns: minmax(0, 1fr); }
        }

        .panel-columnas .tabla td:nth-child(3),
        .panel-columnas .tabla td:nth-child(4)
        {
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
            padding: 11px 0;
            border-bottom: 1px solid var(--borde);
            font-size: .86rem;
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

            <h1><span data-i18n="panel.saludo">Hola</span>, <asp:Literal ID="litNombre" runat="server" /></h1>
            <p class="texto-suave sin-margen" data-i18n="panel.bajada">
                Este es el resumen de la actividad de tu empresa en Falke.
            </p>
        </div>
        <div class="acciones">
            <a class="btn btn-primario" href="<%: ResolveUrl("~/NuevaGrabacion.aspx") %>">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-video" /></svg>
                <span data-i18n="panel.nuevaGrabacion">Nueva grabación</span>
            </a>
        </div>
    </div>

    <nav class="accesos" aria-label="Accesos rápidos">

        <a class="acceso" href="<%: ResolveUrl("~/NuevaGrabacion.aspx") %>">
            <span>
                <span class="titulo" data-i18n="panel.acceso1.titulo">Grabar una sesión</span>
                <span class="detalle" data-i18n="panel.acceso1.detalle">Calibrar el dispositivo y empezar</span>
            </span>
        </a>

        <a class="acceso" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>">
            <span>
                <span class="titulo" data-i18n="panel.acceso2.titulo">Auditar resultados</span>
                <span class="detalle" data-i18n="panel.acceso2.detalle">Mapas, zonas y recorridos</span>
            </span>
        </a>

        <a class="acceso" href="<%: ResolveUrl("~/AnalisisMultiple.aspx") %>">
            <span>
                <span class="titulo" data-i18n="panel.acceso3.titulo">Comparar sesiones</span>
                <span class="detalle" data-i18n="panel.acceso3.detalle">Promediar varios testers</span>
            </span>
        </a>

        <a class="acceso" href="<%: ResolveUrl("~/Categorias.aspx") %>">
            <span>
                <span class="titulo" data-i18n="panel.acceso4.titulo">Categorías</span>
                <span class="detalle" data-i18n="panel.acceso4.detalle">Organizar los activos digitales</span>
            </span>
        </a>

    </nav>

    <div class="tira-metricas">

        <div>
            <div class="metrica-nombre" data-i18n="panel.metrica1">Sesiones este mes</div>
            <div class="metrica-valor">14</div>
            <p class="texto-chico texto-exito sin-margen mt-8" data-i18n="panel.metrica1.detalle">+4 respecto del mes pasado</p>
        </div>

        <div>
            <div class="metrica-nombre" data-i18n="panel.metrica2">En procesamiento</div>
            <div class="metrica-valor">1</div>
            <p class="texto-chico texto-suave sin-margen mt-8" data-i18n="panel.metrica2.detalle">Queda disponible en unos minutos</p>
        </div>

        <div>
            <div class="metrica-nombre" data-i18n="panel.metrica3">Categorías activas</div>
            <div class="metrica-valor">6</div>
            <p class="texto-chico texto-suave sin-margen mt-8" data-i18n="panel.metrica3.detalle">Sobre 5 tipos de activo</p>
        </div>

        <div>
            <div class="metrica-nombre" data-i18n="panel.metrica4">Dispositivos asignados</div>
            <div class="metrica-valor">1</div>
            <p class="texto-chico texto-suave sin-margen mt-8" data-i18n="panel.metrica4.detalle">Tobii 4C &middot; calibrado</p>
        </div>

    </div>

    <div class="panel-columnas">

        <section class="tarjeta">
            <div class="tarjeta-cabecera">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-metricas" /></svg>
                <span data-i18n="panel.ultimas.titulo">Últimas sesiones</span>
                <a class="derecha enlace-detalle" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>">
                    <span data-i18n="panel.ultimas.verTodas">Ver todas</span>
                    <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                </a>
            </div>

            <div class="tabla-scroll" style="border:0">
                <table class="tabla">
                    <caption class="solo-lectores">Últimas sesiones grabadas</caption>
                    <thead>
                        <tr>
                            <th scope="col" data-i18n="panel.tabla.sesion">Sesión</th>
                            <th scope="col" data-i18n="panel.tabla.tipo">Tipo de activo</th>
                            <th scope="col" data-i18n="panel.tabla.fecha">Fecha</th>
                            <th scope="col" data-i18n="panel.tabla.duracion">Duración</th>
                            <th scope="col" data-i18n="panel.tabla.estado">Estado</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><a href="<%: ResolveUrl("~/Visualizaciones.aspx") %>">Checkout - App e-commerce</a></td>
                            <td>App web</td>
                            <td>12 jul 2026</td>
                            <td>18:42</td>
                            <td><span class="badge badge-exito" data-i18n="estado.finalizado">Finalizado</span></td>
                        </tr>
                        <tr>
                            <td><a href="<%: ResolveUrl("~/Visualizaciones.aspx") %>">Onboarding - RPG móvil</a></td>
                            <td>Videojuego</td>
                            <td>10 jul 2026</td>
                            <td>09:15</td>
                            <td><span class="badge badge-exito" data-i18n="estado.finalizado">Finalizado</span></td>
                        </tr>
                        <tr>
                            <td><a href="<%: ResolveUrl("~/Visualizaciones.aspx") %>">Landing - Campaña Q3</a></td>
                            <td>Publicidad</td>
                            <td>08 jul 2026</td>
                            <td>04:03</td>
                            <td><span class="badge badge-alerta" data-i18n="estado.procesando">Procesando</span></td>
                        </tr>
                        <tr>
                            <td><a href="<%: ResolveUrl("~/Visualizaciones.aspx") %>">Login - Panel interno</a></td>
                            <td>Software</td>
                            <td>05 jul 2026</td>
                            <td>06:51</td>
                            <td><span class="badge badge-exito" data-i18n="estado.finalizado">Finalizado</span></td>
                        </tr>
                        <tr>
                            <td><a href="<%: ResolveUrl("~/Visualizaciones.aspx") %>">Carrito - App e-commerce</a></td>
                            <td>App web</td>
                            <td>01 jul 2026</td>
                            <td>12:27</td>
                            <td><span class="badge badge-exito" data-i18n="estado.finalizado">Finalizado</span></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </section>

        <div class="pila">

            <section class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-dispositivo" /></svg>
                    <span data-i18n="panel.dispositivo.titulo">Tu dispositivo</span>
                </div>

                <div class="fila-entre mb-8">
                    <strong>Tobii Eye Tracker 4C</strong>
                    <span class="badge badge-exito" data-i18n="estado.listo">Listo</span>
                </div>
                <p class="texto-chico texto-suave" data-i18n="panel.dispositivo.detalle">
                    Conectado por USB 3.0 &middot; última calibración hace 4 minutos &middot; driver 4.2.1
                </p>

                <div class="barra exito" role="img" aria-label="Calidad de la calibración: 92 por ciento">
                    <span style="width: 92%"></span>
                </div>
                <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="panel.dispositivo.calidad">Calidad de calibración: 92%</p>

                <div class="tarjeta-pie">
                    <a class="enlace-detalle" href="<%: ResolveUrl("~/NuevaGrabacion.aspx") %>">
                        <span data-i18n="panel.dispositivo.verificar">Verificar el dispositivo</span>
                        <svg width="14" height="14" aria-hidden="true"><use href="#i-flecha-der" /></svg>
                    </a>
                </div>
            </section>

            <section class="tarjeta">
                <div class="tarjeta-cabecera">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-reloj" /></svg>
                    <span data-i18n="panel.actividad.titulo">Actividad reciente</span>
                </div>

                <ul class="actividad">
                    <li>
                        <svg width="17" height="17" aria-hidden="true"><use href="#i-tilde" /></svg>
                        <span>
                            <span data-i18n="panel.actividad1">Terminó el procesamiento de «Checkout — App e-commerce»</span>
                            <span class="cuando">Hace 2 horas</span>
                        </span>
                    </li>
                    <li>
                        <svg width="17" height="17" aria-hidden="true"><use href="#i-carpeta" /></svg>
                        <span>
                            <span data-i18n="panel.actividad2">Se registró la categoría «Checkout flow v2»</span>
                            <span class="cuando">Hace 5 horas</span>
                        </span>
                    </li>
                    <li>
                        <svg width="17" height="17" aria-hidden="true"><use href="#i-descarga" /></svg>
                        <span>
                            <span data-i18n="panel.actividad3">Se exportó el reporte PDF del análisis n.&deg; 2398</span>
                            <span class="cuando">Ayer</span>
                        </span>
                    </li>
                    <li>
                        <svg width="17" height="17" aria-hidden="true"><use href="#i-usuarios" /></svg>
                        <span>
                            <span data-i18n="panel.actividad4">Se invitó a diego.paz@ironhide.com</span>
                            <span class="cuando">Hace 2 días</span>
                        </span>
                    </li>
                </ul>
            </section>

        </div>

    </div>

</asp:Content>
