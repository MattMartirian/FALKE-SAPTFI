<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Categorias.aspx.cs" Inherits="GUI.Categorias" Title="Categorías" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .categoria-tipo
        {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-size: .78rem;
            font-weight: 600;
            color: var(--texto-suave);
        }

        .campos-tipo[hidden]
        {
            display: none;
        }

        .campos-tipo
        {
            border: 1px dashed var(--borde-fuerte);
            border-radius: var(--radio);
            padding: 14px 14px 0;
            background: var(--superficie-2);
            margin-top: 4px;
        }

        .campos-tipo > .etiqueta-grupo
        {
            font-size: .85rem;
            font-weight: 600;
            color: var(--texto-suave);
            margin-bottom: 10px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="categorias.titulo">Categorías</h1>
            <p class="texto-suave sin-margen" data-i18n="categorias.bajada">
                Cada categoría identifica un activo digital de tu empresa y agrupa todas las
                sesiones que se graben sobre el.
            </p>
        </div>
        <div class="acciones">
            <button type="button" class="btn btn-primario" data-abre-modal="modalCategoria">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span data-i18n="categorias.nueva">Nueva categoría</span>
            </button>
        </div>
    </div>

    <div class="filtros">
        <div class="campo">
            <label for="catBuscar" data-i18n="categorias.buscar">Buscar</label>
            <input type="search" class="entrada" id="catBuscar"
                   placeholder="Nombre de la categoría..." data-i18n-attr="placeholder:categorias.buscar.placeholder" />
        </div>
        <div class="campo">
            <label for="catTipo" data-i18n="categorias.tipo">Tipo de activo</label>
            <select class="entrada" id="catTipo">
                <option value="" data-i18n="comun.todos">Todos</option>
                <option value="Software">Software</option>
                <option value="App web">App web</option>
                <option value="App movil">App móvil</option>
                <option value="Videojuego">Videojuego</option>
                <option value="Publicidad">Publicidad</option>
            </select>
        </div>
    </div>

    <div class="tabla-scroll">
        <table class="tabla" id="tablaCategorias">
            <caption class="solo-lectores">Categorías registradas por tu empresa</caption>
            <thead>
                <tr>
                    <th scope="col" data-i18n="categorias.col.nombre">Categoría</th>
                    <th scope="col" data-i18n="categorias.col.tipo">Tipo de activo</th>
                    <th scope="col" data-i18n="categorias.col.activo">Activo evaluado</th>
                    <th scope="col" data-i18n="categorias.col.sesiones">Sesiones</th>
                    <th scope="col" data-i18n="categorias.col.creada">Creada</th>
                    <th scope="col"><span class="solo-lectores" data-i18n="comun.acciones">Acciones</span></th>
                </tr>
            </thead>
            <tbody>
                <tr data-tipo="Software">
                    <td><strong>Checkout flow v2</strong></td>
                    <td><span class="categoria-tipo"><svg width="15" height="15" aria-hidden="true"><use href="#i-carpeta" /></svg>Software</span></td>
                    <td class="texto-suave">Panel de administración interno</td>
                    <td>3</td>
                    <td class="texto-suave">17 jul 2026</td>
                    <td>
                        <div class="acciones-fila">
                            <a class="btn btn-fantasma btn-chico" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>" data-i18n="comun.verSesiones">Ver sesiones</a>
                            <button type="button" class="icono-boton" aria-label="Editar la categoría Checkout flow v2">
                                <svg width="17" height="17" aria-hidden="true"><use href="#i-lapiz" /></svg>
                            </button>
                        </div>
                    </td>
                </tr>
                <tr data-tipo="Videojuego">
                    <td><strong>Onboarding RPG</strong></td>
                    <td><span class="categoria-tipo"><svg width="15" height="15" aria-hidden="true"><use href="#i-carpeta" /></svg>Videojuego</span></td>
                    <td class="texto-suave">Tutorial de la primera partida</td>
                    <td>1</td>
                    <td class="texto-suave">10 jul 2026</td>
                    <td>
                        <div class="acciones-fila">
                            <a class="btn btn-fantasma btn-chico" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>" data-i18n="comun.verSesiones">Ver sesiones</a>
                            <button type="button" class="icono-boton" aria-label="Editar la categoría Onboarding RPG">
                                <svg width="17" height="17" aria-hidden="true"><use href="#i-lapiz" /></svg>
                            </button>
                        </div>
                    </td>
                </tr>
                <tr data-tipo="Publicidad">
                    <td><strong>Campaña Q3</strong></td>
                    <td><span class="categoria-tipo"><svg width="15" height="15" aria-hidden="true"><use href="#i-carpeta" /></svg>Publicidad</span></td>
                    <td class="texto-suave">Banner display 970x250</td>
                    <td>1</td>
                    <td class="texto-suave">08 jul 2026</td>
                    <td>
                        <div class="acciones-fila">
                            <a class="btn btn-fantasma btn-chico" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>" data-i18n="comun.verSesiones">Ver sesiones</a>
                            <button type="button" class="icono-boton" aria-label="Editar la categoría Campana Q3">
                                <svg width="17" height="17" aria-hidden="true"><use href="#i-lapiz" /></svg>
                            </button>
                        </div>
                    </td>
                </tr>
                <tr data-tipo="App web">
                    <td><strong>Panel interno</strong></td>
                    <td><span class="categoria-tipo"><svg width="15" height="15" aria-hidden="true"><use href="#i-carpeta" /></svg>App web</span></td>
                    <td class="texto-suave">app.ironhide.com/panel</td>
                    <td>2</td>
                    <td class="texto-suave">02 jul 2026</td>
                    <td>
                        <div class="acciones-fila">
                            <a class="btn btn-fantasma btn-chico" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>" data-i18n="comun.verSesiones">Ver sesiones</a>
                            <button type="button" class="icono-boton" aria-label="Editar la categoría Panel interno">
                                <svg width="17" height="17" aria-hidden="true"><use href="#i-lapiz" /></svg>
                            </button>
                        </div>
                    </td>
                </tr>
                <tr data-tipo="App movil">
                    <td><strong>Alta de cuenta</strong></td>
                    <td><span class="categoria-tipo"><svg width="15" height="15" aria-hidden="true"><use href="#i-carpeta" /></svg>App móvil</span></td>
                    <td class="texto-suave">App Android 3.4.0</td>
                    <td>1</td>
                    <td class="texto-suave">27 jun 2026</td>
                    <td>
                        <div class="acciones-fila">
                            <a class="btn btn-fantasma btn-chico" href="<%: ResolveUrl("~/Visualizaciones.aspx") %>" data-i18n="comun.verSesiones">Ver sesiones</a>
                            <button type="button" class="icono-boton" aria-label="Editar la categoría Alta de cuenta">
                                <svg width="17" height="17" aria-hidden="true"><use href="#i-lapiz" /></svg>
                            </button>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <div class="vacio" id="categoriasVacio" hidden>
        <svg width="34" height="34" aria-hidden="true"><use href="#i-carpeta" /></svg>
        <p class="sin-margen" data-i18n="categorias.vacio">No hay categorías que coincidan con el filtro.</p>
    </div>

    <div class="modal-fondo" id="modalCategoria" role="dialog" aria-modal="true" aria-labelledby="catTitulo" hidden>
        <div class="modal ancho">

            <div class="modal-cabecera">
                <div>
                    <h2 id="catTitulo" data-i18n="categorias.modal.titulo">Registrar nueva categoría</h2>
                    <p class="subtitulo sin-margen" data-i18n="categorias.modal.subtitulo">
                        Los campos disponibles cambian según el tipo de activo digital seleccionado.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <div class="campo">
                    <label for="mcEmpresa" data-i18n="categorias.modal.empresa">Empresa</label>

                    <input type="text" class="entrada" id="mcEmpresa" value="Ironhide Game Studio" readonly />
                </div>

                <div class="campo">
                    <label for="mcNombre" data-i18n="categorias.modal.nombre">Nombre de la categoría</label>
                    <input type="text" class="entrada" id="mcNombre" data-foco-inicial
                           placeholder="Checkout flow v2" data-i18n-attr="placeholder:categorias.modal.nombre.placeholder" />
                </div>

                <div class="campo">
                    <label for="mcTipo" data-i18n="categorias.modal.tipo">Tipo de activo digital</label>
                    <select class="entrada" id="mcTipo">
                        <option value="software">Software</option>
                        <option value="appweb">Aplicación web</option>
                        <option value="appmovil">Aplicación móvil</option>
                        <option value="videojuego">Videojuego</option>
                        <option value="publicidad">Publicidad digital</option>
                    </select>
                </div>

                <div class="campo">
                    <label for="mcActivo" data-i18n="categorias.modal.activo">Nombre del activo evaluado</label>
                    <input type="text" class="entrada" id="mcActivo"
                           placeholder="Panel de administración interno" data-i18n-attr="placeholder:categorias.modal.activo.placeholder" />
                </div>

                <div class="campo">
                    <label for="mcFlujo" data-i18n="categorias.modal.flujo">Flujo o proceso a analizar</label>
                    <textarea class="entrada" id="mcFlujo" rows="2"
                              placeholder="Que parte del activo se va a evaluar"
                              data-i18n-attr="placeholder:categorias.modal.flujo.placeholder"></textarea>
                </div>

                <div class="campos-tipo" data-tipo="software">
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">Campos específicos del tipo seleccionado</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcSo" data-i18n="categorias.campo.so">Sistema operativo</label>
                            <input type="text" class="entrada" id="mcSo" placeholder="Windows 11" />
                        </div>
                        <div class="campo">
                            <label for="mcVersionSw" data-i18n="categorias.campo.version">Versión del software</label>
                            <input type="text" class="entrada" id="mcVersionSw" placeholder="1.4.2" />
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="appweb" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">Campos específicos del tipo seleccionado</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcUrl" data-i18n="categorias.campo.url">URL (opcional)</label>
                            <input type="text" class="entrada" id="mcUrl" placeholder="app.empresa.com/panel" />
                        </div>
                        <div class="campo">
                            <label for="mcDispositivo" data-i18n="categorias.campo.dispositivo">Dispositivo objetivo</label>
                            <select class="entrada" id="mcDispositivo">
                                <option>Escritorio</option>
                                <option>Tablet</option>
                                <option>Móvil</option>
                            </select>
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="appmovil" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">Campos específicos del tipo seleccionado</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcSoMovil" data-i18n="categorias.campo.soMovil">Sistema operativo objetivo</label>
                            <select class="entrada" id="mcSoMovil">
                                <option>Android</option>
                                <option>iOS</option>
                            </select>
                        </div>
                        <div class="campo">
                            <label for="mcVersionApp" data-i18n="categorias.campo.versionApp">Versión de la aplicación</label>
                            <input type="text" class="entrada" id="mcVersionApp" placeholder="3.4.0" />
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="videojuego" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">Campos específicos del tipo seleccionado</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcPlataforma" data-i18n="categorias.campo.plataforma">Plataforma</label>
                            <select class="entrada" id="mcPlataforma">
                                <option>PC</option>
                                <option>Consola</option>
                                <option>Móvil</option>
                            </select>
                        </div>
                        <div class="campo">
                            <label for="mcVersionJuego" data-i18n="categorias.campo.version">Versión del juego</label>
                            <input type="text" class="entrada" id="mcVersionJuego" placeholder="0.9.3 beta" />
                        </div>
                    </div>
                </div>

                <div class="campos-tipo" data-tipo="publicidad" hidden>
                    <p class="etiqueta-grupo" data-i18n="categorias.modal.especificos">Campos específicos del tipo seleccionado</p>
                    <div class="fila-campos">
                        <div class="campo">
                            <label for="mcFormato" data-i18n="categorias.campo.formato">Formato</label>
                            <input type="text" class="entrada" id="mcFormato" placeholder="Banner 970x250" />
                        </div>
                        <div class="campo">
                            <label for="mcCanal" data-i18n="categorias.campo.canal">Canal de distribución</label>
                            <input type="text" class="entrada" id="mcCanal" placeholder="Display / redes" />
                        </div>
                    </div>
                </div>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <button type="button" class="btn btn-primario" data-i18n="categorias.modal.registrar">Registrar categoría</button>
            </div>

        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var buscar = document.getElementById("catBuscar");
            var tipo = document.getElementById("catTipo");
            var filas = document.querySelectorAll("#tablaCategorias tbody tr");
            var vacio = document.getElementById("categoriasVacio");

            function filtrar() {
                var texto = buscar.value.trim().toLowerCase();
                var tipoElegido = tipo.value;
                var visibles = 0;

                for (var i = 0; i < filas.length; i++) {
                    var okTexto = texto === "" || filas[i].textContent.toLowerCase().indexOf(texto) !== -1;
                    var okTipo = tipoElegido === "" || filas[i].getAttribute("data-tipo") === tipoElegido;
                    var mostrar = okTexto && okTipo;

                    filas[i].hidden = !mostrar;

                    if (mostrar) visibles++;
                }

                vacio.hidden = visibles > 0;
            }

            buscar.addEventListener("input", filtrar);
            tipo.addEventListener("change", filtrar);

            var selectorTipo = document.getElementById("mcTipo");
            var bloques = document.querySelectorAll(".campos-tipo");

            function mostrarCamposDelTipo() {
                for (var i = 0; i < bloques.length; i++) {
                    bloques[i].hidden = bloques[i].getAttribute("data-tipo") !== selectorTipo.value;
                }
            }

            selectorTipo.addEventListener("change", mostrarCamposDelTipo);
            mostrarCamposDelTipo();
        })();
    </script>
</asp:Content>
