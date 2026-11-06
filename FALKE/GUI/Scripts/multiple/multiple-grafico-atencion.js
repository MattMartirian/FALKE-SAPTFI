"use strict";

// ATENCION FRENTE A DISTRACCION PROMEDIADA (dona)
// el porcentaje es el promedio de la atencion de cada sesion

var COLOR_ATENCION = "#E8AF52";
var COLOR_DISTRACCION = "#56637C";

class GraficoAtencion extends GraficoMultiple {
    constructor(idCaja) {
        super(idCaja);

        this.cuadricula = false;
        this.porcentaje = null;   // el ultimo porcentaje dibujado (lo usa la leyenda)
    }

    dibujarSesiones(ctx, w, h, sesiones) {
        var valores = [];

        for (var i = 0; i < sesiones.length; i++) valores.push(sesiones[i].analisis.atencion);

        this.porcentaje = Math.round(media(valores));

        var cx = w / 2, cy = h / 2;
        var radio = h * 0.28;

        ctx.lineWidth = h * 0.12;

        // aro entero gris (distraccion) y encima el arco dorado (atencion)
        ctx.strokeStyle = COLOR_DISTRACCION;
        ctx.beginPath();
        ctx.arc(cx, cy, radio, 0, Math.PI * 2);
        ctx.stroke();

        ctx.strokeStyle = COLOR_ATENCION;
        ctx.beginPath();
        ctx.arc(cx, cy, radio, -Math.PI / 2, -Math.PI / 2 + Math.PI * 2 * this.porcentaje / 100);
        ctx.stroke();

        ctx.fillStyle = "#7C879C";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.font = "700 " + Math.round(h * 0.13) + "px sans-serif";
        ctx.fillText(this.porcentaje + "%", cx, cy);
        ctx.textAlign = "start";
    }
}
