"use strict";

// MAPA DE CALOR

class VisualizacionCalor extends VisualizacionTramo {
    constructor() {
        super();

        this.titulo = VisTextos.calorTitulo;
        this.leyenda = VisTextos.calorLeyenda;
        this.tieneTiempoReal = true;
    }

    dibujarTramo(ctx, w, h, pts, primera, ultima) {
        var calor = calcularCalor(pts, primera, ultima);

        if (calor.max <= 0) return;

        pintarCalor(ctx, w, h, calor.grilla, calor.max);
    }
}
