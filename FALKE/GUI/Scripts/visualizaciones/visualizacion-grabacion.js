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
        var t = this.video.currentTime * 1000;

        ctx.clearRect(0, 0, w, h);

        var idx = buscarMuestra(pts, t);

        // fallback si no hay muestra
        if (idx < 0) return;

        // las 10 muestras anteriores
        for (var i = Math.max(0, idx - 10); i < idx; i++) {
            if (!pts[i].ok) continue;

            ctx.beginPath();
            // arc(centro x * width, centro y * height, radio, angulo inicial, angulo final)
            ctx.arc(pts[i].x * w, pts[i].y * h, 5, 0, Math.PI * 2);

            // control de opacidad, cuanto mas viejo menos se ve
            ctx.fillStyle = "rgba(242,97,87," + (0.3 * (i - (idx - 10)) / 10) + ")";
            ctx.fill();
        }

        var p = pts[idx];
        if (!p.ok || t - p.t > 400) return;

        // vista actual es mas grande, con borde blanco
        ctx.beginPath();
        ctx.arc(p.x * w, p.y * h, 13, 0, Math.PI * 2);
        ctx.fillStyle = "rgba(242,97,87,.55)";
        ctx.fill();
        ctx.lineWidth = 2;
        ctx.strokeStyle = "#fff";
        ctx.stroke();
    }
}

// .beginpath arma un dibujo nuevo
// y .arc hace el circulo