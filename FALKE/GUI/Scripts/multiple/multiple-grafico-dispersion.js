"use strict";

class GraficoDispersion extends GraficoMultiple {
    dibujarSesiones(ctx, w, h, sesiones) {
        var resultados = [];
        var i;

        for (var s = 0; s < sesiones.length; s++) {
            var puntos = [];
            var validos = sesiones[s].analisis.validos;

            for (i = 0; i < validos.length; i++) puntos.push({ x: validos[i].x * w, y: validos[i].y * h });

            if (puntos.length >= 3) resultados.push(covarianza(puntos));
        }

        if (resultados.length === 0) {
            this.escribirSinDatos(ctx, w, h);
            return;
        }

        var prom = { cx: 0, cy: 0, sxx: 0, syy: 0, sxy: 0 };

        for (i = 0; i < resultados.length; i++) {
            prom.cx += resultados[i].cx / resultados.length;
            prom.cy += resultados[i].cy / resultados.length;
            prom.sxx += resultados[i].sxx / resultados.length;
            prom.syy += resultados[i].syy / resultados.length;
            prom.sxy += resultados[i].sxy / resultados.length;
        }

        dibujarElipse(ctx, prom.cx, prom.cy, prom.sxx, prom.syy, prom.sxy);

        ctx.fillStyle = "rgba(86,99,124,.75)";

        for (i = 0; i < resultados.length; i++) {
            ctx.beginPath();
            ctx.arc(resultados[i].cx, resultados[i].cy, 5, 0, Math.PI * 2);
            ctx.fill();
        }

        dibujarCentroide(ctx, prom.cx, prom.cy);
    }
}
