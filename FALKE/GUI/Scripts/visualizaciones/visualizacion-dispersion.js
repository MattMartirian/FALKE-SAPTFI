"use strict";

// dispersion de la mirada

class VisualizacionDispersion extends VisualizacionTramo {
    constructor() {
        super();

        this.titulo = VisTextos.dispersionTitulo;
        this.leyenda = VisTextos.dispersionLeyenda;
    }

    dibujarTramo(ctx, w, h, pts, primera, ultima) {
        var puntos = [];

        for (var i = primera; i <= ultima; i++) {
            if (pts[i].ok) puntos.push({ x: pts[i].x * w, y: pts[i].y * h });
        }

        if (puntos.length === 0) return;

        // centro y dispersion de los puntos
        var c = covarianza(puntos);

        // la elipse necesita al menos 3 puntos
        if (puntos.length >= 3) dibujarElipse(ctx, c.cx, c.cy, c.sxx, c.syy, c.sxy);

        // los puntos de mirada son grises
        ctx.fillStyle = "rgba(86,99,124,.75)";

        for (i = 0; i < puntos.length; i++) {
            ctx.beginPath();
            ctx.arc(puntos[i].x, puntos[i].y, 4, 0, Math.PI * 2);
            ctx.fill();
        }

        dibujarCentroide(ctx, c.cx, c.cy);
    }
}
