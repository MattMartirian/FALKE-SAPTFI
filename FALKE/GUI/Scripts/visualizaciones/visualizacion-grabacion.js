"use strict";

// grabacion con seguimiento

class VisualizacionGrabacion extends Visualizacion {
    constructor() {
        super();

        this.titulo = VisTextos.grabacionTitulo;
        this.leyenda = VisTextos.grabacionLeyenda;
    }

    // la base "visualizacion" lo llama cada vez que el video cambia
    dibujar() {
        var analisis = this.sesion.analisis;

        // fallback si no hay json
        if (!analisis) return;

        var ctx = this.canvas.getContext("2d");
        var w = this.canvas.width;
        var h = this.canvas.height;
        var pts = analisis.pts;

        ctx.clearRect(0, 0, w, h);

        this.dibujarMirada(ctx, w, h, pts, this.video.currentTime * 1000);
    }
}

// .beginpath arma un dibujo nuevo
// y .arc hace el circulo