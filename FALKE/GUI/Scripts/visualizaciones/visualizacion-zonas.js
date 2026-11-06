"use strict";

// ZONAS DE INTERES Y ZONAS CIEGAS

class VisualizacionZonas extends VisualizacionTramo {
    constructor() {
        super();

        this.titulo = VisTextos.zonasTitulo;
        this.leyenda = VisTextos.zonasLeyenda;
    }

    dibujarTramo(ctx, w, h, pts, primera, ultima) {
        var calor = calcularCalor(pts, primera, ultima);

        if (calor.max <= 0) return;

        var ciegas = unirBloques(bloquesCiegos(calor.grilla, calor.max));
        var zonas = buscarZonas(calor.grilla, calor.max * ZONA_UMBRAL);

        dibujarZonas(ctx, w, h, zonas, ciegas);
    }
}
