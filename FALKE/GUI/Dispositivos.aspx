<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Dispositivos.aspx.cs" Inherits="GUI.Dispositivos" Title="Dispositivos" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .punto-estado
        {
            width: 9px;
            height: 9px;
            border-radius: var(--radio-full);
            display: inline-block;
            margin-right: 7px;
            background: var(--texto-tenue);
        }

        .punto-estado.enUso { background: var(--exito); }
        .punto-estado.disponible { background: var(--info); }
        .punto-estado.mantenimiento { background: var(--alerta); }
        .punto-estado.baja { background: var(--peligro); }

        .modelo-figura
        {
            aspect-ratio: 16 / 6;
            border-radius: var(--radio);
            background: var(--superficie-2);
            border: 1px solid var(--borde);
            display: flex;
            align-items: center;
            justify-content: center;
            color: var(--texto-tenue);
            margin-bottom: 16px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="dispositivos.titulo">Dispositivos</h1>
            <p class="texto-suave sin-margen" data-i18n="dispositivos.bajada">
                Inventario de eye trackers y a qué empresa está prestado cada uno.
            </p>
        </div>
        <div class="acciones">
            <button type="button" class="btn btn-secundario" data-abre-modal="modalAsignar">
                <svg width="17" height="17" aria-hidden="true"><use href="#i-empresa" /></svg>
                <span data-i18n="dispositivos.asignar">Asignar a empresa</span>
            </button>
            <button type="button" class="btn btn-primario" data-abre-modal="modalAltaDispositivo">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-mas" /></svg>
                <span data-i18n="dispositivos.nuevo">Alta de dispositivo</span>
            </button>
        </div>
    </div>

    <div class="metricas mb-24">
        <div>
            <div class="metrica-valor">26</div>
            <div class="metrica-nombre" data-i18n="dispositivos.metrica.total">En inventario</div>
        </div>
        <div>
            <div class="metrica-valor">19</div>
            <div class="metrica-nombre" data-i18n="dispositivos.metrica.prestados">Prestados</div>
        </div>
        <div>
            <div class="metrica-valor">5</div>
            <div class="metrica-nombre" data-i18n="dispositivos.metrica.disponibles">Disponibles</div>
        </div>
        <div>
            <div class="metrica-valor">2</div>
            <div class="metrica-nombre" data-i18n="dispositivos.metrica.taller">En mantenimiento</div>
        </div>
    </div>

    <div class="filtros">
        <div class="campo">
            <label for="disBuscar" data-i18n="dispositivos.buscar">Buscar</label>
            <input type="search" class="entrada" id="disBuscar"
                   placeholder="Serie o empresa..."
                   data-i18n-attr="placeholder:dispositivos.buscar.placeholder" />
        </div>
        <div class="campo">
            <label for="disEstado" data-i18n="dispositivos.estado">Estado</label>
            <select class="entrada" id="disEstado">
                <option value="" data-i18n="comun.todos">Todos</option>
                <option value="enUso" data-i18n="estado.enUso">En uso</option>
                <option value="disponible" data-i18n="estado.disponible">Disponible</option>
                <option value="mantenimiento" data-i18n="estado.mantenimiento">En mantenimiento</option>
                <option value="baja" data-i18n="estado.baja">De baja</option>
            </select>
        </div>
        <div class="campo">
            <label for="disModelo" data-i18n="dispositivos.modelo">Modelo</label>
            <select class="entrada" id="disModelo">
                <option value="" data-i18n="comun.todos">Todos</option>
                <option value="Tobii Eye Tracker 5">Tobii Eye Tracker 5</option>
                <option value="Tobii Pro Nano">Tobii Pro Nano</option>
            </select>
        </div>
    </div>

    <div class="tabla-scroll">
        <table class="tabla" id="tablaDispositivos">
            <caption class="solo-lectores">Inventario de dispositivos</caption>
            <thead>
                <tr>
                    <th scope="col" data-i18n="dispositivos.col.serie">Número de serie</th>
                    <th scope="col" data-i18n="dispositivos.col.modelo">Modelo</th>
                    <th scope="col" data-i18n="dispositivos.col.empresa">Empresa</th>
                    <th scope="col" data-i18n="dispositivos.col.entrega">Entregado</th>
                    <th scope="col" data-i18n="dispositivos.col.calibracion">Última calibración</th>
                    <th scope="col" data-i18n="dispositivos.col.estado">Estado</th>
                    <th scope="col"><span class="solo-lectores" data-i18n="comun.acciones">Acciones</span></th>
                </tr>
            </thead>
            <tbody>

                <tr data-estado="enUso" data-modelo="Tobii Eye Tracker 5">
                    <td class="mono">TB5-2026-0148</td>
                    <td>Tobii Eye Tracker 5</td>
                    <td>Ironhide Game Studio</td>
                    <td class="texto-suave">18 jun 2026</td>
                    <td class="texto-suave">2 sep 2026</td>
                    <td><span class="punto-estado enUso" aria-hidden="true"></span><span data-i18n="estado.enUso">En uso</span></td>
                    <td>
                        <div class="acciones-fila">
                            <button type="button" class="btn btn-fantasma btn-chico" data-abre-modal="modalDispositivo" data-i18n="comun.ver">Ver</button>
                        </div>
                    </td>
                </tr>

                <tr data-estado="enUso" data-modelo="Tobii Pro Nano">
                    <td class="mono">TPN-2026-0031</td>
                    <td>Tobii Pro Nano</td>
                    <td>Mercado Cruz S.A.</td>
                    <td class="texto-suave">3 mar 2026</td>
                    <td class="texto-suave">28 ago 2026</td>
                    <td><span class="punto-estado enUso" aria-hidden="true"></span><span data-i18n="estado.enUso">En uso</span></td>
                    <td>
                        <div class="acciones-fila">
                            <button type="button" class="btn btn-fantasma btn-chico" data-abre-modal="modalDispositivo" data-i18n="comun.ver">Ver</button>
                        </div>
                    </td>
                </tr>

                <tr data-estado="disponible" data-modelo="Tobii Eye Tracker 5">
                    <td class="mono">TB5-2026-0152</td>
                    <td>Tobii Eye Tracker 5</td>
                    <td class="texto-tenue">&mdash;</td>
                    <td class="texto-suave">&mdash;</td>
                    <td class="texto-suave">1 sep 2026</td>
                    <td><span class="punto-estado disponible" aria-hidden="true"></span><span data-i18n="estado.disponible">Disponible</span></td>
                    <td>
                        <div class="acciones-fila">
                            <button type="button" class="btn btn-fantasma btn-chico" data-abre-modal="modalAsignar" data-i18n="dispositivos.asignarCorto">Asignar</button>
                        </div>
                    </td>
                </tr>

                <tr data-estado="mantenimiento" data-modelo="Tobii Eye Tracker 5">
                    <td class="mono">TB5-2025-0090</td>
                    <td>Tobii Eye Tracker 5</td>
                    <td class="texto-tenue">&mdash;</td>
                    <td class="texto-suave">&mdash;</td>
                    <td class="texto-suave">14 jul 2026</td>
                    <td><span class="punto-estado mantenimiento" aria-hidden="true"></span><span data-i18n="estado.mantenimiento">En mantenimiento</span></td>
                    <td>
                        <div class="acciones-fila">
                            <button type="button" class="btn btn-fantasma btn-chico" data-abre-modal="modalDispositivo" data-i18n="comun.ver">Ver</button>
                        </div>
                    </td>
                </tr>

                <tr data-estado="enUso" data-modelo="Tobii Pro Nano">
                    <td class="mono">TPN-2025-0018</td>
                    <td>Tobii Pro Nano</td>
                    <td>Austral Seguros</td>
                    <td class="texto-suave">9 nov 2025</td>
                    <td class="texto-suave">30 ago 2026</td>
                    <td><span class="punto-estado enUso" aria-hidden="true"></span><span data-i18n="estado.enUso">En uso</span></td>
                    <td>
                        <div class="acciones-fila">
                            <button type="button" class="btn btn-fantasma btn-chico" data-abre-modal="modalDispositivo" data-i18n="comun.ver">Ver</button>
                        </div>
                    </td>
                </tr>

                <tr data-estado="baja" data-modelo="Tobii Eye Tracker 5">
                    <td class="mono">TB5-2024-0007</td>
                    <td>Tobii Eye Tracker 5</td>
                    <td class="texto-tenue">&mdash;</td>
                    <td class="texto-suave">&mdash;</td>
                    <td class="texto-suave">2 feb 2026</td>
                    <td><span class="punto-estado baja" aria-hidden="true"></span><span data-i18n="estado.baja">De baja</span></td>
                    <td>
                        <div class="acciones-fila">
                            <button type="button" class="btn btn-fantasma btn-chico" data-abre-modal="modalDispositivo" data-i18n="comun.ver">Ver</button>
                        </div>
                    </td>
                </tr>

            </tbody>
        </table>
    </div>

    <div class="vacio" id="dispositivosVacio" hidden>
        <svg width="34" height="34" aria-hidden="true"><use href="#i-dispositivo" /></svg>
        <p class="sin-margen" data-i18n="dispositivos.vacio">No hay dispositivos que coincidan con el filtro.</p>
    </div>

    <div class="modal-fondo" id="modalDispositivo" role="dialog" aria-modal="true" aria-labelledby="disTitulo" hidden>
        <div class="modal">

            <div class="modal-cabecera">
                <div>
                    <h2 id="disTitulo">TB5-2026-0148</h2>
                    <p class="subtitulo sin-margen">Tobii Eye Tracker 5</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <div class="modelo-figura" aria-hidden="true">
                    <svg width="44" height="44"><use href="#i-dispositivo" /></svg>
                </div>

                <div class="tabla-scroll mb-24">
                    <table class="tabla">
                        <caption class="solo-lectores">Datos del dispositivo</caption>
                        <tbody>
                            <tr><th scope="row" data-i18n="dispositivos.det.empresa">Prestado a</th><td>Ironhide Game Studio</td></tr>
                            <tr><th scope="row" data-i18n="dispositivos.det.entrega">Fecha de entrega</th><td>18 jun 2026</td></tr>
                            <tr><th scope="row" data-i18n="dispositivos.det.driver">Driver requerido</th><td class="mono">4.0 o superior</td></tr>
                            <tr><th scope="row" data-i18n="dispositivos.det.firmware">Firmware</th><td class="mono">2.13.4</td></tr>
                            <tr><th scope="row" data-i18n="dispositivos.det.calibracion">Última calibración</th><td>2 sep 2026</td></tr>
                            <tr><th scope="row" data-i18n="dispositivos.det.sesiones">Sesiones grabadas</th><td>48</td></tr>
                        </tbody>
                    </table>
                </div>

                <p class="etiqueta" data-i18n="dispositivos.det.historial">Historial de préstamos</p>
                <div class="tabla-scroll">
                    <table class="tabla">
                        <caption class="solo-lectores">Historial de préstamos del dispositivo</caption>
                        <thead>
                            <tr>
                                <th scope="col" data-i18n="dispositivos.col.empresa">Empresa</th>
                                <th scope="col" data-i18n="dispositivos.col.desde">Desde</th>
                                <th scope="col" data-i18n="dispositivos.col.hasta">Hasta</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr><td>Ironhide Game Studio</td><td class="texto-suave">18 jun 2026</td><td class="texto-suave" data-i18n="dispositivos.actual">En curso</td></tr>
                            <tr><td>Pixel Norte</td><td class="texto-suave">4 ene 2026</td><td class="texto-suave">30 may 2026</td></tr>
                        </tbody>
                    </table>
                </div>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cerrar">Cerrar</button>

                <button type="button" class="btn btn-secundario" data-i18n="dispositivos.devolver">Registrar devolución</button>
                <button type="button" class="btn btn-primario" data-i18n="dispositivos.mantenimiento">Enviar a mantenimiento</button>
            </div>

        </div>
    </div>

    <div class="modal-fondo" id="modalAsignar" role="dialog" aria-modal="true" aria-labelledby="asigTitulo" hidden>
        <div class="modal">

            <div class="modal-cabecera">
                <div>
                    <h2 id="asigTitulo" data-i18n="asignar.titulo">Asignar dispositivo</h2>
                    <p class="subtitulo sin-margen" data-i18n="asignar.subtitulo">
                        El préstamo queda asociado a la empresa mientras dure la suscripción.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <div class="campo">
                    <label for="asigDispositivo" data-i18n="asignar.dispositivo">Dispositivo disponible</label>
                    <select class="entrada" id="asigDispositivo" data-foco-inicial>
                        <option value="TB5-2026-0152">TB5-2026-0152 &mdash; Tobii Eye Tracker 5</option>
                        <option value="TB5-2026-0161">TB5-2026-0161 &mdash; Tobii Eye Tracker 5</option>
                        <option value="TPN-2026-0044">TPN-2026-0044 &mdash; Tobii Pro Nano</option>
                    </select>
                </div>

                <div class="campo">
                    <label for="asigEmpresa" data-i18n="asignar.empresa">Empresa</label>
                    <select class="entrada" id="asigEmpresa">
                        <option value="1">Ironhide Game Studio</option>
                        <option value="2">Mercado Cruz S.A.</option>
                        <option value="3">Nova Publicidad</option>
                        <option value="4">Austral Seguros</option>
                    </select>
                    <p class="ayuda" data-i18n="asignar.empresa.ayuda">
                        Solo aparecen las empresas activas.
                    </p>
                </div>

                <div class="campo">
                    <label for="asigFecha" data-i18n="asignar.fecha">Fecha de entrega</label>
                    <input type="date" class="entrada" id="asigFecha" />
                </div>

                <div class="campo">
                    <label for="asigNota" data-i18n="asignar.nota">Observaciones</label>
                    <textarea class="entrada" id="asigNota" rows="3"
                              placeholder="Remito, transporte, contacto que lo recibe..."
                              data-i18n-attr="placeholder:asignar.nota.placeholder"></textarea>
                </div>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <button type="button" class="btn btn-primario" data-i18n="asignar.confirmar">Registrar préstamo</button>
            </div>

        </div>
    </div>

    <div class="modal-fondo" id="modalAltaDispositivo" role="dialog" aria-modal="true" aria-labelledby="altaDisTitulo" hidden>
        <div class="modal">

            <div class="modal-cabecera">
                <div>
                    <h2 id="altaDisTitulo" data-i18n="altaDispositivo.titulo">Alta de dispositivo</h2>
                    <p class="subtitulo sin-margen" data-i18n="altaDispositivo.subtitulo">
                        Se suma al inventario en estado disponible.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <div class="campo">
                    <label for="adSerie" data-i18n="altaDispositivo.serie">Número de serie</label>
                    <input type="text" class="entrada" id="adSerie" placeholder="TB5-2026-0000" data-foco-inicial />
                    <p class="ayuda" data-i18n="altaDispositivo.serie.ayuda">No se puede repetir.</p>
                </div>

                <div class="fila-campos">
                    <div class="campo">
                        <label for="adModelo" data-i18n="altaDispositivo.modelo">Modelo</label>
                        <select class="entrada" id="adModelo">
                            <option>Tobii Eye Tracker 5</option>
                            <option>Tobii Pro Nano</option>
                        </select>
                    </div>
                    <div class="campo">
                        <label for="adFirmware" data-i18n="altaDispositivo.firmware">Firmware</label>
                        <input type="text" class="entrada" id="adFirmware" placeholder="2.13.4" />
                    </div>
                </div>

                <div class="campo">
                    <label for="adCompra" data-i18n="altaDispositivo.compra">Fecha de compra</label>
                    <input type="date" class="entrada" id="adCompra" />
                </div>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <button type="button" class="btn btn-primario" data-i18n="altaDispositivo.crear">Dar de alta</button>
            </div>

        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var buscar = document.getElementById("disBuscar");
            var estado = document.getElementById("disEstado");
            var modelo = document.getElementById("disModelo");
            var filas = document.querySelectorAll("#tablaDispositivos tbody tr");
            var vacio = document.getElementById("dispositivosVacio");

            function filtrar() {
                var texto = buscar.value.trim().toLowerCase();
                var visibles = 0;

                for (var i = 0; i < filas.length; i++)
                {
                    var okTexto = texto === "" || filas[i].textContent.toLowerCase().indexOf(texto) !== -1;
                    var okEstado = estado.value === "" || filas[i].getAttribute("data-estado") === estado.value;
                    var okModelo = modelo.value === "" || filas[i].getAttribute("data-modelo") === modelo.value;
                    var mostrar = okTexto && okEstado && okModelo;

                    filas[i].hidden = !mostrar;

                    if (mostrar) visibles++;
                }

                vacio.hidden = visibles > 0;
            }

            buscar.addEventListener("input", filtrar);
            estado.addEventListener("change", filtrar);
            modelo.addEventListener("change", filtrar);
        })();
    </script>
</asp:Content>
