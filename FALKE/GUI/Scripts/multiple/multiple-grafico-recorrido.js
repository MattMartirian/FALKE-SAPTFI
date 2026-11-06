"use strict";

var RECORRIDO_COLOR = "rgba(86,99,124,.28)";

class GraficoRecorrido extends GraficoMultiple {
    dibujarSesiones(ctx, w, h, sesiones) {
        var i, s;

        ctx.lineWidth = 2;
        ctx.lineJoin = "round";
        ctx.strokeStyle = RECORRIDO_COLOR;

        for (s = 0; s < sesiones.length; s++) {
            var validos = sesiones[s].analisis.validos;

            ctx.beginPath();
            ctx.moveTo(validos[0].x * w, validos[0].y * h);

            for (i = 1; i < validos.length; i++) {
                if (validos[i].t - validos[i - 1].t > HUECO_MAX_MS) ctx.moveTo(validos[i].x * w, validos[i].y * h);
                else ctx.lineTo(validos[i].x * w, validos[i].y * h);
            }

            ctx.stroke();
        }

        for (s = 0; s < sesiones.length; s++) {
            var v = sesiones[s].analisis.validos;

            this.marcar(ctx, v[0].x * w, v[0].y * h, "#56637C");
            this.marcar(ctx, v[v.length - 1].x * w, v[v.length - 1].y * h, "#F26157");
        }
    }

    marcar(ctx, x, y, color) {
        ctx.beginPath();
        ctx.arc(x, y, 6, 0, Math.PI * 2);
        ctx.fillStyle = color;
        ctx.fill();
        ctx.lineWidth = 2;
        ctx.strokeStyle = "#fff";
        ctx.stroke();
    }
}
