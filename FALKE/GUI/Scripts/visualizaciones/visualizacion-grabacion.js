"use strict";

class VisualizacionGrabacion extends Visualizacion {
    constructor() {
        super();

        this.titulo = VisTextos.grabacionTitulo;
        this.leyenda = VisTextos.grabacionLeyenda;
    }

    dibujar() {
        var analisis = this.sesion.analisis;

        if (!analisis) return;

        var ctx = this.canvas.getContext("2d");
        var w = this.canvas.width;
        var h = this.canvas.height;
        var pts = analisis.pts;
        var t = this.video.currentTime * 1000;

        ctx.clearRect(0, 0, w, h);

        var idx = buscarMuestra(pts, t);

        if (idx < 0) return;

        for (var i = Math.max(0, idx - 10); i < idx; i++) {
            if (!pts[i].ok) continue;

            ctx.beginPath();
            ctx.arc(pts[i].x * w, pts[i].y * h, 5, 0, Math.PI * 2);
            ctx.fillStyle = "rgba(242,97,87," + (0.08 + 0.3 * (i - (idx - 10)) / 10) + ")";
            ctx.fill();
        }

        var p = pts[idx];

        if (!p.ok || t - p.t > 400) return;

        ctx.beginPath();
        ctx.arc(p.x * w, p.y * h, 11, 0, Math.PI * 2);
        ctx.fillStyle = "rgba(242,97,87,.55)";
        ctx.fill();
        ctx.lineWidth = 2;
        ctx.strokeStyle = "#fff";
        ctx.stroke();
    }
}
