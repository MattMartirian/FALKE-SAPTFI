"use strict";

// ANALISIS

var INTERVALO_MS = 100;     
var UMBRAL_FIJACION = 0.04;  
var ATENCION_MIN_MS = 1500;  
var HUECO_MAX_MS = 350;      


function analizar(datos, desfase) {
    var inicio = datos.inicio_utc;
    var lista = datos.muestras || [];
    var pts = [];

    for (var i = 0; i < lista.length; i++) {
        var m = lista[i];
        pts.push({ t: (m.u - inicio) + desfase, x: m.x, y: m.y, ok: m.ok === 1 });
    }

    var validos = pts.filter(function (p) { return p.ok; });
    var fijs = fijaciones(validos);
    var tFij = 0;

    for (var k = 0; k < fijs.length; k++) tFij += fijs[k].dur;


    var tAtencion = 0;

    for (var n = 0; n < fijs.length; n++) {
        if (fijs[n].dur >= ATENCION_MIN_MS) tAtencion += fijs[n].dur;
    }

    var tValido = validos.length * INTERVALO_MS;
    var atencion = tValido > 0 ? Math.min(100, Math.round(tAtencion * 100 / tValido)) : 0;

    return {
        pts: pts,
        validos: validos,
        fijs: fijs,
        atencion: atencion,
        sacadico: tValido > 0 ? 100 - atencion : 0,
        fijMedia: fijs.length ? tFij / fijs.length / 1000 : 0
    };
}


function fijaciones(v) {
    var res = [];
    var i = 0;

    while (i < v.length) {
        var j = i;
        var minX = v[i].x, maxX = minX, minY = v[i].y, maxY = minY;

        while (j + 1 < v.length && v[j + 1].t - v[j].t <= HUECO_MAX_MS) {
            var n = v[j + 1];
            var nMinX = Math.min(minX, n.x), nMaxX = Math.max(maxX, n.x);
            var nMinY = Math.min(minY, n.y), nMaxY = Math.max(maxY, n.y);

            if ((nMaxX - nMinX) + (nMaxY - nMinY) > UMBRAL_FIJACION) break;

            minX = nMinX; maxX = nMaxX; minY = nMinY; maxY = nMaxY;
            j++;
        }

        if (j > i) {
            var sx = 0, sy = 0;

            for (var k = i; k <= j; k++) { sx += v[k].x; sy += v[k].y; }

            var c = j - i + 1;
            res.push({ t0: v[i].t, dur: (v[j].t - v[i].t) + INTERVALO_MS, x: sx / c, y: sy / c });
            i = j + 1;
        } else {
            i++;
        }
    }

    return res;
}


function buscarMuestra(muestras, tiempoMs) {
    var desde = 0;
    var hasta = muestras.length - 1;
    var encontrada = -1;

    // busqueda binaria
    while (desde <= hasta) {
        var medio = Math.floor((desde + hasta) / 2);

        if (muestras[medio].t <= tiempoMs) {
            // esta sirve
            encontrada = medio;
            desde = medio + 1;
        } else {
            // se pasó
            hasta = medio - 1;
        }
    }

    return encontrada;
}
