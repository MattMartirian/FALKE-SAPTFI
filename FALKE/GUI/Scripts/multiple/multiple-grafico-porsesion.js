"use strict";

// ATENCION POR SESION (barras)
// una barra por sesion: lo dorado es su porcentaje de atencion; la linea punteada es el promedio

class GraficoPorSesion extends GraficoMultiple {
    constructor(idCaja) {
        super(idCaja);

        this.cuadricula = false;
    }

    dibujarSesiones(ctx, w, h, sesiones) {
        var izq = 60, der = 20, arriba = 20, abajo = 20;
        var alto = h - arriba - abajo;
        var ancho = w - izq - der;
        var i;

        // lineas de referencia al 0, 25, 50, 75 y 100 %
        ctx.font = "500 18px sans-serif";
        ctx.fillStyle = "#7C879C";
        ctx.textBaseline = "middle";
        ctx.lineWidth = 1;
        ctx.strokeStyle = "rgba(124,135,156,.28)";

        for (i = 0; i <= 4; i++) {
            var y = arriba + alto - alto * i / 4;

            ctx.beginPath();
            ctx.moveTo(izq, y);
            ctx.lineTo(w - der, y);
            ctx.stroke();
            ctx.fillText(i * 25 + "%", 8, y);
        }

        var paso = ancho / sesiones.length;
        var grosor = Math.min(70, paso * 0.7);
        var valores = [];

        for (i = 0; i < sesiones.length; i++) {
            var pct = sesiones[i].analisis.atencion;
            var x = izq + paso * i + (paso - grosor) / 2;

            valores.push(pct);

            ctx.fillStyle = COLOR_DISTRACCION;
            ctx.fillRect(x, arriba, grosor, alto);

            ctx.fillStyle = COLOR_ATENCION;
            ctx.fillRect(x, arriba + alto * (1 - pct / 100), grosor, alto * pct / 100);
        }

        // promedio
        var yMedia = arriba + alto * (1 - media(valores) / 100);

        ctx.setLineDash([8, 6]);
        ctx.lineWidth = 2;
        ctx.strokeStyle = "#F26157";
        ctx.beginPath();
        ctx.moveTo(izq, yMedia);
        ctx.lineTo(w - der, yMedia);
        ctx.stroke();
        ctx.setLineDash([]);
    }
}
