"use strict";

// FUNCIONES COMPARTIDAS

var CALOR_ANCHO = 96;
var CALOR_ALTO = 54;
var CALOR_RADIO = 4;

var OPACIDAD_MIN = 0;
var OPACIDAD_MAX = 0.9;
var CURVA_OPACIDAD = 5;

function armarMolde() {
    var molde = [];
    var sigma = CALOR_RADIO / 2;

    for (var dy = -CALOR_RADIO; dy <= CALOR_RADIO; dy++) {
        for (var dx = -CALOR_RADIO; dx <= CALOR_RADIO; dx++) {
            molde.push({ dx: dx, dy: dy, peso: Math.exp(-(dx * dx + dy * dy) / (2 * sigma * sigma)) });
        }
    }

    return molde;
}

var MOLDE_CALOR = armarMolde();

function calcularCalor(pts, desde, hasta) {
    var grilla = new Float32Array(CALOR_ANCHO * CALOR_ALTO);
    var max = 0;

    for (var i = desde; i <= hasta; i++) {
        var p = pts[i];

        if (!p.ok) continue;

        var cx = Math.min(CALOR_ANCHO - 1, Math.max(0, Math.floor(p.x * CALOR_ANCHO)));
        var cy = Math.min(CALOR_ALTO - 1, Math.max(0, Math.floor(p.y * CALOR_ALTO)));

        for (var k = 0; k < MOLDE_CALOR.length; k++) {
            var gx = cx + MOLDE_CALOR[k].dx;
            var gy = cy + MOLDE_CALOR[k].dy;

            if (gx < 0 || gx >= CALOR_ANCHO || gy < 0 || gy >= CALOR_ALTO) continue;

            var pos = gy * CALOR_ANCHO + gx;

            grilla[pos] += MOLDE_CALOR[k].peso;

            if (grilla[pos] > max) max = grilla[pos];
        }
    }

    return { grilla: grilla, max: max };
}

function colorCalor(v) {
    //rgb
    var FRIO = [86, 99, 124], MEDIO = [232, 175, 82], CALIENTE = [242, 97, 87];
    var desde = v < 0.5 ? FRIO : MEDIO;
    var hasta = v < 0.5 ? MEDIO : CALIENTE;
    var f = v < 0.5 ? v * 2 : (v - 0.5) * 2;

    return {
        r: Math.round(desde[0] + (hasta[0] - desde[0]) * f),
        g: Math.round(desde[1] + (hasta[1] - desde[1]) * f),
        b: Math.round(desde[2] + (hasta[2] - desde[2]) * f),
        a: v <= 0 ? 0 : OPACIDAD_MIN + (OPACIDAD_MAX - OPACIDAD_MIN) * (1 - Math.exp(-CURVA_OPACIDAD * v)) / (1 - Math.exp(-CURVA_OPACIDAD))
    };
}

var capaCalor = document.createElement("canvas");
capaCalor.width = CALOR_ANCHO;
capaCalor.height = CALOR_ALTO;

function pintarCalor(ctx, w, h, grilla, max) {
    var capa = capaCalor.getContext("2d");
    var imagen = capa.createImageData(CALOR_ANCHO, CALOR_ALTO);

    for (var i = 0; i < grilla.length; i++) {
        var c = colorCalor(grilla[i] / max);

        imagen.data[i * 4] = c.r;
        imagen.data[i * 4 + 1] = c.g;
        imagen.data[i * 4 + 2] = c.b;
        imagen.data[i * 4 + 3] = Math.round(c.a * 255);
    }

    capa.putImageData(imagen, 0, 0);
    ctx.imageSmoothingEnabled = true;
    ctx.drawImage(capaCalor, 0, 0, w, h);
}


var ZONA_UMBRAL = 0.35;
var ZONA_MIN_CELDAS = 8;
var ZONA_MAX = 5;
var CIEGA_BLOQUES_X = 8;
var CIEGA_BLOQUES_Y = 6;
var CIEGA_UMBRAL = 0.04;

function buscarZonas(grilla, umbral) {
    var visitada = new Uint8Array(grilla.length);
    var zonas = [];

    for (var inicio = 0; inicio < grilla.length; inicio++) {
        if (visitada[inicio] || grilla[inicio] < umbral) continue;

        var pendientes = [inicio];
        var zona = { x0: CALOR_ANCHO, x1: 0, y0: CALOR_ALTO, y1: 0, celdas: 0, calor: 0 };

        visitada[inicio] = 1;

        while (pendientes.length > 0) {
            var pos = pendientes.pop();
            var x = pos % CALOR_ANCHO;
            var y = Math.floor(pos / CALOR_ANCHO);

            zona.celdas++;
            zona.calor += grilla[pos];
            zona.x0 = Math.min(zona.x0, x);
            zona.x1 = Math.max(zona.x1, x);
            zona.y0 = Math.min(zona.y0, y);
            zona.y1 = Math.max(zona.y1, y);

            var vecinas = [[x - 1, y], [x + 1, y], [x, y - 1], [x, y + 1]];

            for (var k = 0; k < vecinas.length; k++) {
                var vx = vecinas[k][0], vy = vecinas[k][1];

                if (vx < 0 || vx >= CALOR_ANCHO || vy < 0 || vy >= CALOR_ALTO) continue;

                var vpos = vy * CALOR_ANCHO + vx;

                if (visitada[vpos] || grilla[vpos] < umbral) continue;

                visitada[vpos] = 1;
                pendientes.push(vpos);
            }
        }

        if (zona.celdas >= ZONA_MIN_CELDAS) zonas.push(zona);
    }

    zonas.sort(function (a, b) { return b.calor - a.calor; });

    return zonas.slice(0, ZONA_MAX);
}

function bloquesCiegos(grilla, max) {
    var anchoBloque = CALOR_ANCHO / CIEGA_BLOQUES_X;
    var altoBloque = CALOR_ALTO / CIEGA_BLOQUES_Y;
    var ciegos = [];

    for (var by = 0; by < CIEGA_BLOQUES_Y; by++) {
        for (var bx = 0; bx < CIEGA_BLOQUES_X; bx++) {
            var maximo = 0;

            for (var y = by * altoBloque; y < (by + 1) * altoBloque; y++) {
                for (var x = bx * anchoBloque; x < (bx + 1) * anchoBloque; x++) {
                    maximo = Math.max(maximo, grilla[y * CALOR_ANCHO + x]);
                }
            }

            ciegos.push(maximo < max * CIEGA_UMBRAL);
        }
    }

    return ciegos;
}

function unirBloques(ciegos) {
    var rectangulos = [];

    for (var by = 0; by < CIEGA_BLOQUES_Y; by++) {
        var bx = 0;

        while (bx < CIEGA_BLOQUES_X) {
            if (!ciegos[by * CIEGA_BLOQUES_X + bx]) { bx++; continue; }

            var desde = bx;

            while (bx < CIEGA_BLOQUES_X && ciegos[by * CIEGA_BLOQUES_X + bx]) bx++;

            var hasta = bx - 1;
            var estirada = false;

            for (var r = 0; r < rectangulos.length; r++) {
                if (rectangulos[r].x0 === desde && rectangulos[r].x1 === hasta && rectangulos[r].y1 === by - 1) {
                    rectangulos[r].y1 = by;
                    estirada = true;
                    break;
                }
            }

            if (!estirada) rectangulos.push({ x0: desde, x1: hasta, y0: by, y1: by });
        }
    }

    return rectangulos;
}

function dibujarZonas(ctx, w, h, zonas, ciegas) {
    var i;

    ctx.lineWidth = 2;
    ctx.setLineDash([7, 5]);
    ctx.strokeStyle = "#56637C";
    ctx.fillStyle = "rgba(86,99,124,.14)";

    for (i = 0; i < ciegas.length; i++) {
        var q = ciegas[i];
        var qx = q.x0 / CIEGA_BLOQUES_X * w, qy = q.y0 / CIEGA_BLOQUES_Y * h;
        var qw = (q.x1 - q.x0 + 1) / CIEGA_BLOQUES_X * w, qh = (q.y1 - q.y0 + 1) / CIEGA_BLOQUES_Y * h;

        ctx.fillRect(qx + 1, qy + 1, qw - 2, qh - 2);
        ctx.strokeRect(qx + 1, qy + 1, qw - 2, qh - 2);
    }

    ctx.setLineDash([]);
    ctx.fillStyle = "rgba(232,175,82,.28)";
    ctx.strokeStyle = "#E8AF52";

    for (i = 0; i < zonas.length; i++) {
        var z = zonas[i];
        var zx = z.x0 / CALOR_ANCHO * w, zy = z.y0 / CALOR_ALTO * h;
        var zw = (z.x1 - z.x0 + 1) / CALOR_ANCHO * w, zh = (z.y1 - z.y0 + 1) / CALOR_ALTO * h;

        ctx.fillRect(zx, zy, zw, zh);
        ctx.strokeRect(zx, zy, zw, zh);
    }
}

var DISPERSION_K = 2;

function covarianza(puntos) {
    var cx = 0, cy = 0, i;

    for (i = 0; i < puntos.length; i++) {
        cx += puntos[i].x;
        cy += puntos[i].y;
    }

    cx /= puntos.length;
    cy /= puntos.length;

    var sxx = 0, syy = 0, sxy = 0;

    for (i = 0; i < puntos.length; i++) {
        var dx = puntos[i].x - cx;
        var dy = puntos[i].y - cy;

        sxx += dx * dx;
        syy += dy * dy;
        sxy += dx * dy;
    }

    return { cx: cx, cy: cy, sxx: sxx / puntos.length, syy: syy / puntos.length, sxy: sxy / puntos.length };
}

function dibujarElipse(ctx, cx, cy, sxx, syy, sxy) {
    var mitad = (sxx - syy) / 2;
    var radio = Math.sqrt(mitad * mitad + sxy * sxy);
    var largo = (sxx + syy) / 2 + radio;
    var ancho = Math.max((sxx + syy) / 2 - radio, 0);
    var angulo = 0.5 * Math.atan2(2 * sxy, sxx - syy);

    if (largo <= 0) return;

    ctx.beginPath();
    ctx.ellipse(cx, cy, DISPERSION_K * Math.sqrt(largo), DISPERSION_K * Math.sqrt(ancho), angulo, 0, Math.PI * 2);
    ctx.fillStyle = "rgba(232,175,82,.16)";
    ctx.fill();
    ctx.lineWidth = 2;
    ctx.setLineDash([7, 5]);
    ctx.strokeStyle = "#E8AF52";
    ctx.stroke();
    ctx.setLineDash([]);
}

function dibujarCentroide(ctx, cx, cy) {
    ctx.beginPath();
    ctx.arc(cx, cy, 7, 0, Math.PI * 2);
    ctx.fillStyle = "#F26157";
    ctx.fill();
    ctx.lineWidth = 2;
    ctx.strokeStyle = "#fff";
    ctx.stroke();
}
