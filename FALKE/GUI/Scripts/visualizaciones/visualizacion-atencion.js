"use strict";



var COLOR_ATENCION = "#E8AF52";
var COLOR_DISTRACCION = "#56637C";
var COLOR_SIN_DATOS = "rgba(124,135,156,.25)";

class VisualizacionAtencion extends VisualizacionTramo {
    constructor() {
        super();

        this.titulo = VisTextos.atencionTitulo;
        this.leyenda = this.armarLeyenda(null, null);

        this.franjaCaja = document.getElementById("franjaCaja");
        this.franja = document.getElementById("franja");
        this.estados = null;  

        this.franja.addEventListener("click", (e) => {
            if (!this.abierta || !this.video) return;

            var caja = this.franja.getBoundingClientRect();

            this.video.currentTime = this.duracion() * (e.clientX - caja.left) / caja.width;
        });
    }

    alAbrir() {
        super.alAbrir();

        this.estados = null;
        this.franjaCaja.hidden = false;
    }

    alCerrar() {
        super.alCerrar();

        this.franjaCaja.hidden = true;
    }

    dibujar() {
        super.dibujar();

        this.dibujarFranja();
    }

    armarLeyenda(atencion, distraccion) {
        var activa = VisTextos.atencionActiva;
        var sacadico = VisTextos.atencionSacadico;

        activa = atencion === null ? activa.replace(/\s*\{0\}\s*%?/, "") : activa.replace("{0}", atencion);
        sacadico = distraccion === null ? sacadico.replace(/\s*\{0\}\s*%?/, "") : sacadico.replace("{0}", distraccion);

        return '<span><i style="background:' + COLOR_ATENCION + '"></i>' + activa + '</span>' +
               '<span><i style="background:' + COLOR_DISTRACCION + '"></i>' + sacadico + '</span>' +
               '<span><i style="background:' + COLOR_SIN_DATOS + ';border:1px solid ' + COLOR_DISTRACCION + '"></i>' + VisTextos.atencionSinDatos + '</span>';
    }

    obtenerEstados() {
        if (!this.estados) this.estados = estadosAtencion(this.sesion.analisis.pts, this.sesion.analisis.fijs);

        return this.estados;
    }

    // dona con el porcentaje de atencion del tramo, arriba a la derecha del video
    dibujarTramo(ctx, w, h, pts, primera, ultima) {
        var estados = this.obtenerEstados();
        var atentas = 0, distraidas = 0;

        for (var i = primera; i <= ultima; i++) {
            if (estados[i] === 2) atentas++;
            else if (estados[i] === 1) distraidas++;
        }

        var validas = atentas + distraidas;

        if (validas === 0) return;

        // mismo criterio que el tablero: distraccion = 100 - atencion
        var pctAtencion = Math.min(100, Math.round(atentas * 100 / validas));

        this.elLeyenda.innerHTML = this.armarLeyenda(pctAtencion, 100 - pctAtencion);

        // cuadro blanco de fondo
        var lado = Math.round(h * 0.36);
        var x0 = w - lado - 20;
        var y0 = 20;
        var cx = x0 + lado / 2;
        var cy = y0 + lado * 0.43;
        var radio = lado * 0.3;

        ctx.fillStyle = "rgba(255,255,255,.93)";
        ctx.fillRect(x0, y0, lado, lado);

        // aro entero gris (distraccion) y encima el arco dorado (atencion), que arranca arriba y avanza en sentido horario
        ctx.lineWidth = lado * 0.11;
        ctx.strokeStyle = COLOR_DISTRACCION;
        ctx.beginPath();
        ctx.arc(cx, cy, radio, 0, Math.PI * 2);
        ctx.stroke();

        ctx.strokeStyle = COLOR_ATENCION;
        ctx.beginPath();
        ctx.arc(cx, cy, radio, -Math.PI / 2, -Math.PI / 2 + Math.PI * 2 * pctAtencion / 100);
        ctx.stroke();

        // porcentaje en el centro y segundos de atencion sobre segundos con datos abajo
        ctx.fillStyle = "#0E1A34";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.font = "700 " + Math.round(lado * 0.2) + "px sans-serif";
        ctx.fillText(pctAtencion + "%", cx, cy);
        ctx.font = "600 " + Math.round(lado * 0.09) + "px sans-serif";
        ctx.fillText(Math.round(atentas * INTERVALO_MS / 1000) + " s / " + Math.round(validas * INTERVALO_MS / 1000) + " s", cx, y0 + lado * 0.9);
        ctx.textAlign = "start";
    }

    // franja de toda la grabacion: un rectangulito por muestra, del color de su estado
    dibujarFranja() {
        if (!this.sesion.analisis) return;

        var pts = this.sesion.analisis.pts;
        var estados = this.obtenerEstados();
        var ctx = this.franja.getContext("2d");
        var w = this.franja.width;
        var h = this.franja.height;
        var durMs = this.duracion() * 1000;
        var desdeMs = this.duracion() * Number(this.barraDesde.value);
        var hastaMs = this.duracion() * Number(this.barraHasta.value);
        var ancho = Math.max(1, INTERVALO_MS / durMs * w);

        ctx.clearRect(0, 0, w, h);

        // fondo: lo que quede sin pintar es "sin datos"
        ctx.fillStyle = COLOR_SIN_DATOS;
        ctx.fillRect(0, 0, w, h);

        for (var i = 0; i < pts.length; i++) {
            if (estados[i] === 0) continue;

            // lo que queda fuera del tramo elegido se ve apagado
            ctx.globalAlpha = pts[i].t >= desdeMs && pts[i].t <= hastaMs ? 1 : 0.3;
            ctx.fillStyle = estados[i] === 2 ? COLOR_ATENCION : COLOR_DISTRACCION;
            ctx.fillRect(pts[i].t / durMs * w, 0, ancho, h);
        }

        ctx.globalAlpha = 1;

        // marca del momento actual del video
        ctx.fillStyle = "#F26157";
        ctx.fillRect(this.video.currentTime * 1000 / durMs * w - 1, 0, 3, h);
    }
}
