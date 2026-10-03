<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Respaldos.aspx.cs" Inherits="GUI.Respaldos" Title="Respaldos y mantenimiento" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .fila-respaldo .mono
        {
            font-size: .82rem;
        }

        .fila-respaldo .datos-respaldo
        {
            display: flex;
            flex-wrap: wrap;
            gap: 4px 16px;
            font-size: .82rem;
            color: var(--texto-suave);
            margin-top: 4px;
        }

        .fila-respaldo.marcada
        {
            border-color: var(--peligro);
            background: var(--superficie-2);
        }

        .caja-mantenimiento
        {
            background: var(--superficie-alt);
            color: var(--marca-perla);
            border-radius: var(--radio-lg);
            padding: 22px 24px;
        }

        .caja-mantenimiento h2
        {
            color: var(--marca-perla);
            margin-top: 0;
        }

        .caja-mantenimiento p
        {
            color: rgba(252, 252, 247, .78);
        }

        .vista-aviso
        {
            background: rgba(252, 252, 247, .08);
            border: 1px dashed rgba(252, 252, 247, .35);
            border-radius: var(--radio);
            padding: 14px 16px;
            margin-top: 16px;
            font-size: .9rem;
            color: var(--marca-perla);
        }

        .caja-mantenimiento .entrada
        {
            background: rgba(252, 252, 247, .1);
            border-color: rgba(252, 252, 247, .3);
            color: var(--marca-perla);
        }

        .caja-mantenimiento label
        {
            color: rgba(252, 252, 247, .85);
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="pagina-cabecera">
        <div>
            <h1 data-i18n="respaldos.titulo">Respaldos y mantenimiento</h1>
            <p class="texto-suave sin-margen" data-i18n="respaldos.bajada">
                Copias de seguridad de la base y puesta en mantenimiento del sistema.
            </p>
        </div>
        <div class="acciones">
            <button type="button" class="btn btn-primario" data-abre-modal="modalNuevoRespaldo">
                <svg width="18" height="18" aria-hidden="true"><use href="#i-base" /></svg>
                <span data-i18n="respaldos.generar">Generar respaldo</span>
            </button>
        </div>
    </div>

    <div class="pestanas" role="tablist" aria-label="Secciones de respaldos">
        <button type="button" class="pestana" role="tab" aria-selected="true"
                data-pestana="panelRespaldos" id="tabRespaldos" aria-controls="panelRespaldos"
                data-i18n="respaldos.tab.copias">Copias de seguridad</button>
        <button type="button" class="pestana" role="tab" aria-selected="false"
                data-pestana="panelMantenimiento" id="tabMantenimiento" aria-controls="panelMantenimiento"
                data-i18n="respaldos.tab.mantenimiento">Mantenimiento del sistema</button>
    </div>

    <div id="panelRespaldos" role="tabpanel" aria-labelledby="tabRespaldos">

        <div class="metricas mb-24">
            <div>
                <div class="metrica-valor">18</div>
                <div class="metrica-nombre" data-i18n="respaldos.metrica.total">Respaldos guardados</div>
            </div>
            <div>
                <div class="metrica-valor">hoy 03:00</div>
                <div class="metrica-nombre" data-i18n="respaldos.metrica.ultimo">Último respaldo</div>
            </div>
            <div>
                <div class="metrica-valor">4,2 GB</div>
                <div class="metrica-nombre" data-i18n="respaldos.metrica.espacio">Espacio ocupado</div>
            </div>
            <div>
                <div class="metrica-valor" data-i18n="respaldos.metrica.diario">Diario</div>
                <div class="metrica-nombre" data-i18n="respaldos.metrica.frecuencia">Frecuencia automática</div>
            </div>
        </div>

        <div class="aviso aviso-alerta">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
            <p class="sin-margen" data-i18n="respaldos.avisoRestaurar">
                Restaurar un respaldo reemplaza toda la base por la del momento de la copia:
                lo que se cargó después se pierde. Al confirmar, el sistema se pone en
                mantenimiento y se cierran todas las sesiones, incluida la tuya.
            </p>
        </div>

        <h2 class="mt-24" data-i18n="respaldos.disponibles">Respaldos disponibles</h2>

        <div id="listaRespaldos">

            <label class="opcion fila-respaldo">
                <input type="radio" name="respaldo" value="1" data-fecha="8 sep 2026, 03:00" />
                <span>
                    <span class="opcion-titulo">
                        8 sep 2026, 03:00
                        <span class="badge badge-exito ml-8" data-i18n="respaldos.automatico">Automático</span>
                    </span>
                    <span class="datos-respaldo">
                        <span class="mono">falke_20260908_0300.bak</span>
                        <span>1,4 GB</span>
                        <span data-i18n="respaldos.completo">Copia completa</span>
                        <span class="texto-exito" data-i18n="respaldos.verificado">Verificado</span>
                    </span>
                </span>
            </label>

            <label class="opcion fila-respaldo">
                <input type="radio" name="respaldo" value="2" data-fecha="7 sep 2026, 03:00" />
                <span>
                    <span class="opcion-titulo">
                        7 sep 2026, 03:00
                        <span class="badge badge-exito ml-8" data-i18n="respaldos.automatico">Automático</span>
                    </span>
                    <span class="datos-respaldo">
                        <span class="mono">falke_20260907_0300.bak</span>
                        <span>1,4 GB</span>
                        <span data-i18n="respaldos.completo">Copia completa</span>
                        <span class="texto-exito" data-i18n="respaldos.verificado">Verificado</span>
                    </span>
                </span>
            </label>

            <label class="opcion fila-respaldo">
                <input type="radio" name="respaldo" value="3" data-fecha="6 sep 2026, 18:42" />
                <span>
                    <span class="opcion-titulo">
                        6 sep 2026, 18:42
                        <span class="badge badge-info ml-8" data-i18n="respaldos.manual">Manual</span>
                    </span>
                    <span class="datos-respaldo">
                        <span class="mono">falke_20260906_1842.bak</span>
                        <span>1,3 GB</span>
                        <span data-i18n="respaldos.previoActualizacion">Previo a la actualización 1.4</span>
                        <span class="texto-exito" data-i18n="respaldos.verificado">Verificado</span>
                    </span>
                </span>
            </label>

            <label class="opcion fila-respaldo">
                <input type="radio" name="respaldo" value="4" data-fecha="5 sep 2026, 03:00" />
                <span>
                    <span class="opcion-titulo">
                        5 sep 2026, 03:00
                        <span class="badge badge-exito ml-8" data-i18n="respaldos.automatico">Automático</span>
                    </span>
                    <span class="datos-respaldo">
                        <span class="mono">falke_20260905_0300.bak</span>
                        <span>1,3 GB</span>
                        <span data-i18n="respaldos.completo">Copia completa</span>
                        <span class="texto-peligro" data-i18n="respaldos.sinVerificar">Sin verificar</span>
                    </span>
                </span>
            </label>

            <label class="opcion fila-respaldo">
                <input type="radio" name="respaldo" value="5" data-fecha="4 sep 2026, 03:00" />
                <span>
                    <span class="opcion-titulo">
                        4 sep 2026, 03:00
                        <span class="badge badge-exito ml-8" data-i18n="respaldos.automatico">Automático</span>
                    </span>
                    <span class="datos-respaldo">
                        <span class="mono">falke_20260904_0300.bak</span>
                        <span>1,3 GB</span>
                        <span data-i18n="respaldos.completo">Copia completa</span>
                        <span class="texto-exito" data-i18n="respaldos.verificado">Verificado</span>
                    </span>
                </span>
            </label>

        </div>

        <div class="fila-entre mt-24">
            <p class="texto-chico texto-suave sin-margen" id="respaldoElegido" role="status"
               data-i18n="respaldos.ningunoElegido">
                No elegiste ningún respaldo todavía.
            </p>
            <button type="button" class="btn btn-peligro" id="btnRestaurar" disabled
                    data-abre-modal="modalRestaurar" data-i18n="respaldos.restaurar">
                Restaurar el respaldo elegido
            </button>
        </div>

    </div>

    <div id="panelMantenimiento" role="tabpanel" aria-labelledby="tabMantenimiento" hidden>

        <div class="tarjeta mb-24">
            <div class="tarjeta-cabecera">
                <h2 class="sin-margen" data-i18n="mantenimiento.estadoActual">Estado actual del sistema</h2>
                <span class="badge badge-exito" id="badgeEstadoSistema" data-i18n="mantenimiento.operativo">Operativo</span>
            </div>
            <p class="texto-suave" data-i18n="mantenimiento.estadoAyuda">
                Cuando el sistema está bloqueado, solo pueden entrar los usuarios de Pattern
                Blue. Al resto le aparece la pantalla de mantenimiento.
            </p>
        </div>

        <div class="caja-mantenimiento">

            <h2 data-i18n="mantenimiento.programar">Programar una ventana de mantenimiento</h2>
            <p data-i18n="mantenimiento.programarAyuda">
                Se le avisa a todos los usuarios conectados con la anticipación que elijas,
                para que puedan guardar lo que están haciendo.
            </p>

            <div class="fila-campos">
                <div class="campo">
                    <label for="mtAnticipacion" data-i18n="mantenimiento.anticipacion">Avisar con</label>
                    <select class="entrada" id="mtAnticipacion">
                        <option value="15">15 minutos</option>
                        <option value="30" selected>30 minutos</option>
                        <option value="60">1 hora</option>
                        <option value="120">2 horas</option>
                        <option value="1440">1 día</option>
                    </select>
                </div>
                <div class="campo">
                    <label for="mtDuracion" data-i18n="mantenimiento.duracion">Duración estimada</label>
                    <select class="entrada" id="mtDuracion">
                        <option value="15">15 minutos</option>
                        <option value="30">30 minutos</option>
                        <option value="60" selected>1 hora</option>
                        <option value="180">3 horas</option>
                    </select>
                </div>
            </div>

            <div class="campo">
                <label for="mtMotivo" data-i18n="mantenimiento.motivo">Motivo (opcional, se muestra en el aviso)</label>
                <input type="text" class="entrada" id="mtMotivo"
                       placeholder="Actualizacion de versión, restauración, tareas de base..."
                       data-i18n-attr="placeholder:mantenimiento.motivo.placeholder" />
            </div>

            <div class="vista-aviso" role="status">
                <strong data-i18n="mantenimiento.vistaPrevia">Así lo va a ver cada usuario:</strong>
                <p class="sin-margen mt-8" id="textoAviso">
                    La página será puesta en mantenimiento en 30 minutos, guarde sus archivos
                    antes de la hora informada.
                </p>
            </div>

            <div class="fila mt-24">
                <button type="button" class="btn btn-primario" data-abre-modal="modalProgramar"
                        data-i18n="mantenimiento.programarBoton">Programar y avisar</button>
                <button type="button" class="btn btn-peligro" data-abre-modal="modalBloquear"
                        data-i18n="mantenimiento.bloquearYa">Bloquear ahora</button>
            </div>

        </div>

        <h2 class="mt-24" data-i18n="mantenimiento.historial">Últimas ventanas de mantenimiento</h2>

        <div class="tabla-scroll">
            <table class="tabla">
                <caption class="solo-lectores">Historial de ventanas de mantenimiento</caption>
                <thead>
                    <tr>
                        <th scope="col" data-i18n="mantenimiento.col.inicio">Inicio</th>
                        <th scope="col" data-i18n="mantenimiento.col.duracion">Duración</th>
                        <th scope="col" data-i18n="mantenimiento.col.motivo">Motivo</th>
                        <th scope="col" data-i18n="mantenimiento.col.responsable">Responsable</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>6 sep 2026, 19:00</td>
                        <td class="texto-suave">42 min</td>
                        <td>Actualización a la versión 1.4</td>
                        <td class="texto-suave">Ramiro Diaz</td>
                    </tr>
                    <tr>
                        <td>21 ago 2026, 22:00</td>
                        <td class="texto-suave">1 h 10 min</td>
                        <td>Restauración de respaldo por error de carga</td>
                        <td class="texto-suave">Ramiro Diaz</td>
                    </tr>
                    <tr>
                        <td>3 ago 2026, 20:30</td>
                        <td class="texto-suave">25 min</td>
                        <td>Mantenimiento de indices de la base</td>
                        <td class="texto-suave">Ramiro Diaz</td>
                    </tr>
                </tbody>
            </table>
        </div>

    </div>

    <div class="modal-fondo" id="modalRestaurar" role="dialog" aria-modal="true" aria-labelledby="restTitulo" hidden>
        <div class="modal">

            <div class="modal-cabecera">
                <div>
                    <h2 id="restTitulo" data-i18n="restaurar.titulo">Confirmar la restauración</h2>
                    <p class="subtitulo sin-margen" data-i18n="restaurar.subtitulo">Esta acción no se puede deshacer.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <div class="aviso aviso-peligro">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen">
                        <span data-i18n="restaurar.aviso">Vas a restaurar el respaldo del</span>
                        <strong id="restFecha">8 sep 2026, 03:00</strong>.
                    </p>
                </div>

                <p data-i18n="restaurar.consecuencias">Al confirmar:</p>
                <ul>
                    <li data-i18n="restaurar.paso1">El sistema se pone en mantenimiento inmediatamente.</li>
                    <li data-i18n="restaurar.paso2">Se cierran todas las sesiones abiertas, incluida la tuya.</li>
                    <li data-i18n="restaurar.paso3">Se reemplaza la base actual por la del respaldo elegido.</li>
                    <li data-i18n="restaurar.paso4">Se recalculan los digitos verificadores al terminar.</li>
                </ul>

                <div class="campo">
                    <label for="restConfirmacion" data-i18n="restaurar.escribir">
                        Para confirmar, escribi RESTAURAR
                    </label>
                    <input type="text" class="entrada" id="restConfirmacion" autocomplete="off" data-foco-inicial />
                </div>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>

                <button type="button" class="btn btn-peligro" id="btnConfirmarRestaurar" disabled
                        data-i18n="restaurar.confirmar">Restaurar y cerrar sesiones</button>
            </div>

        </div>
    </div>

    <div class="modal-fondo" id="modalProgramar" role="dialog" aria-modal="true" aria-labelledby="progTitulo" hidden>
        <div class="modal">

            <div class="modal-cabecera">
                <div>
                    <h2 id="progTitulo" data-i18n="programar.titulo">Programar el mantenimiento</h2>
                    <p class="subtitulo sin-margen" data-i18n="programar.subtitulo">
                        El aviso le llega a todos los usuarios conectados.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">
                <p data-i18n="programar.texto">
                    Se va a enviar este aviso ahora mismo y el sistema se va a bloquear
                    cuando se cumpla el plazo:
                </p>
                <div class="aviso aviso-alerta mb-0">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-reloj" /></svg>
                    <p class="sin-margen" id="textoAvisoModal">
                        La página será puesta en mantenimiento en 30 minutos, guarde sus archivos
                        antes de la hora informada.
                    </p>
                </div>
            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <button type="button" class="btn btn-primario" data-i18n="programar.confirmar">Enviar aviso y programar</button>
            </div>

        </div>
    </div>

    <div class="modal-fondo" id="modalBloquear" role="dialog" aria-modal="true" aria-labelledby="bloqTitulo" hidden>
        <div class="modal">

            <div class="modal-cabecera">
                <div>
                    <h2 id="bloqTitulo" data-i18n="bloquear.titulo">Bloquear el sistema ahora</h2>
                    <p class="subtitulo sin-margen" data-i18n="bloquear.subtitulo">Sin aviso previo a los usuarios.</p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">
                <div class="aviso aviso-peligro mb-0">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen" data-i18n="bloquear.aviso">
                        Todos los usuarios que no sean de Pattern Blue van a perder el acceso
                        de inmediato y podrían perder trabajo sin guardar. Usalo solo ante una
                        urgencia.
                    </p>
                </div>
            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <button type="button" class="btn btn-peligro" data-i18n="bloquear.confirmar">Bloquear ahora</button>
            </div>

        </div>
    </div>

    <div class="modal-fondo" id="modalNuevoRespaldo" role="dialog" aria-modal="true" aria-labelledby="nuevoRespTitulo" hidden>
        <div class="modal">

            <div class="modal-cabecera">
                <div>
                    <h2 id="nuevoRespTitulo" data-i18n="nuevoRespaldo.titulo">Generar un respaldo</h2>
                    <p class="subtitulo sin-margen" data-i18n="nuevoRespaldo.subtitulo">
                        Se genera una copia completa de la base en este momento.
                    </p>
                </div>
                <button type="button" class="modal-cerrar" data-cierra-modal aria-label="Cerrar">
                    <svg width="18" height="18" aria-hidden="true"><use href="#i-cerrar" /></svg>
                </button>
            </div>

            <div class="modal-cuerpo">

                <div class="campo">
                    <label for="nrDescripcion" data-i18n="nuevoRespaldo.descripcion">Descripción</label>
                    <input type="text" class="entrada" id="nrDescripcion"
                           placeholder="Previo a la actualizacion 1.5"
                           data-i18n-attr="placeholder:nuevoRespaldo.descripcion.placeholder" data-foco-inicial />
                    <p class="ayuda" data-i18n="nuevoRespaldo.descripcion.ayuda">
                        Sirve para reconocerlo después en la lista.
                    </p>
                </div>

                <label class="casilla-simple">
                    <input type="checkbox" id="nrVerificar" checked />
                    <span data-i18n="nuevoRespaldo.verificar">Verificar la copia al terminar</span>
                </label>

                <p class="texto-chico texto-suave mt-16 sin-margen" data-i18n="nuevoRespaldo.duracion">
                    El proceso tarda unos minutos y no interrumpe el uso del sistema.
                </p>

            </div>

            <div class="modal-pie">
                <button type="button" class="btn btn-secundario" data-cierra-modal data-i18n="comun.cancelar">Cancelar</button>
                <button type="button" class="btn btn-primario" data-i18n="nuevoRespaldo.generar">Generar respaldo</button>
            </div>

        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            "use strict";

            var lista = document.getElementById("listaRespaldos");
            var botonRestaurar = document.getElementById("btnRestaurar");
            var textoElegido = document.getElementById("respaldoElegido");
            var fechaEnModal = document.getElementById("restFecha");

            lista.addEventListener("change", function (evento) {
                var radio = evento.target;

                if (radio.name !== "respaldo") return;

                var fecha = radio.getAttribute("data-fecha");

                textoElegido.textContent = "Respaldo elegido: " + fecha;
                fechaEnModal.textContent = fecha;
                botonRestaurar.disabled = false;
            });

            var confirmacion = document.getElementById("restConfirmacion");
            var botonConfirmar = document.getElementById("btnConfirmarRestaurar");

            confirmacion.addEventListener("input", function () {
                botonConfirmar.disabled = confirmacion.value.trim().toUpperCase() !== "RESTAURAR";
            });

            var anticipacion = document.getElementById("mtAnticipacion");
            var motivo = document.getElementById("mtMotivo");
            var textoAviso = document.getElementById("textoAviso");
            var textoAvisoModal = document.getElementById("textoAvisoModal");

            function enPalabras(minutos) {
                if (minutos < 60) return minutos + " minutos";

                if (minutos === 60) return "1 hora";

                if (minutos < 1440) return (minutos / 60) + " horas";

                return "1 dia";
            }

            function armarAviso() {
                var mensaje = "La pagina sera puesta en mantenimiento en " +
                              enPalabras(parseInt(anticipacion.value, 10)) +
                              ", guarde sus archivos antes de la hora informada.";

                if (motivo.value.trim() !== "")
                    mensaje = mensaje + " Motivo: " + motivo.value.trim() + ".";

                textoAviso.textContent = mensaje;
                textoAvisoModal.textContent = mensaje;
            }

            anticipacion.addEventListener("change", armarAviso);
            motivo.addEventListener("input", armarAviso);
        })();
    </script>
</asp:Content>
