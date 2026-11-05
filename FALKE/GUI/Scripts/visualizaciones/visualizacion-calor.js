"use strict";

// MAPA DE CALOR

class VisualizacionCalor extends VisualizacionTramo {
    constructor() {
        super();

        this.titulo = VisTextos.calorTitulo;
        this.leyenda = VisTextos.calorLeyenda;
        this.tieneTiempoReal = true;

        this.capa = document.createElement("canvas");
        this.capa.width = CALOR_ANCHO;
        this.capa.height = CALOR_ALTO;
    }

    // color de una celda segun cuanto calor tiene
    colorDeCelda(v) {
        //rgb
        var FRIO = [86, 99, 124], MEDIO = [232, 175, 82], CALIENTE = [242, 97, 87];
        var desde = v < 0.5 ? FRIO : MEDIO;
        var hasta = v < 0.5 ? MEDIO : CALIENTE;
        var f = v < 0.5 ? v * 2 : (v - 0.5) * 2;

        return {
            r: Math.round(desde[0] + (hasta[0] - desde[0]) * f),
            g: Math.round(desde[1] + (hasta[1] - desde[1]) * f),
            b: Math.round(desde[2] + (hasta[2] - desde[2]) * f),
            a: Math.min(0.8, v * 1.6)
        };
    }

    dibujarTramo(ctx, w, h, calor) {
        var capa = this.capa.getContext("2d");
        var imagen = capa.createImageData(CALOR_ANCHO, CALOR_ALTO);

        for (var i = 0; i < calor.grilla.length; i++) {
            var c = this.colorDeCelda(calor.grilla[i] / calor.max);

            imagen.data[i * 4] = c.r;
            imagen.data[i * 4 + 1] = c.g;
            imagen.data[i * 4 + 2] = c.b;
            imagen.data[i * 4 + 3] = Math.round(c.a * 255);
        }

        capa.putImageData(imagen, 0, 0);
        ctx.imageSmoothingEnabled = true;
        ctx.drawImage(this.capa, 0, 0, w, h);
    }
}
