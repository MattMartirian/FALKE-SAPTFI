"use strict";

// ZONAS DE INTERES Y ZONAS CIEGAS (consenso)
// en cada sesion se buscan sus zonas calientes y sus bloques ciegos; despues se cuentan en cuantas sesiones coincide cada lugar.
// se dibuja lo que coincide en CONSENSO_ZONAS o mas de las sesiones

var CONSENSO_ZONAS = 0.6;

class GraficoZonas extends GraficoMultiple {
    dibujarSesiones(ctx, w, h, sesiones) {
        var calores = [];
        var i, s;

        for (s = 0; s < sesiones.length; s++) {
            var pts = sesiones[s].analisis.pts;
            var calor = calcularCalor(pts, 0, pts.length - 1);

            if (calor.max > 0) calores.push(calor);
        }

        if (calores.length === 0) {
            this.escribirSinDatos(ctx, w, h);
            return;
        }

        // cuantas sesiones tienen cada celda caliente y cada bloque ciego
        var interes = new Uint8Array(CALOR_ANCHO * CALOR_ALTO);
        var ciegos = new Uint8Array(CIEGA_BLOQUES_X * CIEGA_BLOQUES_Y);

        for (s = 0; s < calores.length; s++) {
            var umbral = calores[s].max * ZONA_UMBRAL;
            var ciegoEnSesion = bloquesCiegos(calores[s].grilla, calores[s].max);

            for (i = 0; i < interes.length; i++) {
                if (calores[s].grilla[i] >= umbral) interes[i]++;
            }

            for (i = 0; i < ciegos.length; i++) {
                if (ciegoEnSesion[i]) ciegos[i]++;
            }
        }

        // minimo de sesiones que tienen que coincidir
        var minimo = Math.ceil(calores.length * CONSENSO_ZONAS - 0.0001);
        var ciegosEnConsenso = [];

        for (i = 0; i < ciegos.length; i++) ciegosEnConsenso.push(ciegos[i] >= minimo);

        dibujarZonas(ctx, w, h, buscarZonas(interes, minimo), unirBloques(ciegosEnConsenso));
    }
}
