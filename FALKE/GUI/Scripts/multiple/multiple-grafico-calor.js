"use strict";

class GraficoCalor extends GraficoMultiple {
    dibujarSesiones(ctx, w, h, sesiones) {
        var promedio = new Float32Array(CALOR_ANCHO * CALOR_ALTO);
        var max = 0;
        var i;

        for (var s = 0; s < sesiones.length; s++) {
            var pts = sesiones[s].analisis.pts;
            var calor = calcularCalor(pts, 0, pts.length - 1);
            var suma = 0;

            for (i = 0; i < calor.grilla.length; i++) suma += calor.grilla[i];

            if (suma <= 0) continue;

            for (i = 0; i < calor.grilla.length; i++) promedio[i] += calor.grilla[i] / suma / sesiones.length;
        }

        for (i = 0; i < promedio.length; i++) max = Math.max(max, promedio[i]);

        if (max <= 0) {
            this.escribirSinDatos(ctx, w, h);
            return;
        }

        pintarCalor(ctx, w, h, promedio, max);
    }
}
