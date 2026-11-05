"use strict";

// ZONAS DE INTERES Y ZONAS CIEGAS


var ZONA_UMBRAL = 0.35;     
var ZONA_MIN_CELDAS = 8;    
var ZONA_MAX = 5;           
var CIEGA_BLOQUES_X = 8;   
var CIEGA_BLOQUES_Y = 6;
var CIEGA_UMBRAL = 0.04;   

class VisualizacionZonas extends VisualizacionTramo {
    constructor() {
        super();

        this.titulo = VisTextos.zonasTitulo;
        this.leyenda = VisTextos.zonasLeyenda;
    }

    buscarZonas(calor) {
        var umbral = calor.max * ZONA_UMBRAL;
        var visitada = new Uint8Array(calor.grilla.length);
        var zonas = [];

        for (var inicio = 0; inicio < calor.grilla.length; inicio++) {
            if (visitada[inicio] || calor.grilla[inicio] < umbral) continue;

            var pendientes = [inicio];
            var zona = { x0: CALOR_ANCHO, x1: 0, y0: CALOR_ALTO, y1: 0, celdas: 0, calor: 0 };

            visitada[inicio] = 1;

            while (pendientes.length > 0) {
                var pos = pendientes.pop();
                var x = pos % CALOR_ANCHO;
                var y = Math.floor(pos / CALOR_ANCHO);

                zona.celdas++;
                zona.calor += calor.grilla[pos];
                zona.x0 = Math.min(zona.x0, x);
                zona.x1 = Math.max(zona.x1, x);
                zona.y0 = Math.min(zona.y0, y);
                zona.y1 = Math.max(zona.y1, y);

                var vecinas = [[x - 1, y], [x + 1, y], [x, y - 1], [x, y + 1]];

                for (var k = 0; k < vecinas.length; k++) {
                    var vx = vecinas[k][0], vy = vecinas[k][1];

                    if (vx < 0 || vx >= CALOR_ANCHO || vy < 0 || vy >= CALOR_ALTO) continue;

                    var vpos = vy * CALOR_ANCHO + vx;

                    if (visitada[vpos] || calor.grilla[vpos] < umbral) continue;

                    visitada[vpos] = 1;
                    pendientes.push(vpos);
                }
            }

            if (zona.celdas >= ZONA_MIN_CELDAS) zonas.push(zona);
        }

        zonas.sort(function (a, b) { return b.calor - a.calor; });

        return zonas.slice(0, ZONA_MAX);
    }

    buscarZonasCiegas(calor) {
        var anchoBloque = CALOR_ANCHO / CIEGA_BLOQUES_X;
        var altoBloque = CALOR_ALTO / CIEGA_BLOQUES_Y;
        var rectangulos = [];

        for (var by = 0; by < CIEGA_BLOQUES_Y; by++) {
            var ciega = [];

            for (var bx = 0; bx < CIEGA_BLOQUES_X; bx++) {
                var maximo = 0;

                for (var y = by * altoBloque; y < (by + 1) * altoBloque; y++) {
                    for (var x = bx * anchoBloque; x < (bx + 1) * anchoBloque; x++) {
                        maximo = Math.max(maximo, calor.grilla[y * CALOR_ANCHO + x]);
                    }
                }

                ciega.push(maximo < calor.max * CIEGA_UMBRAL);
            }

            bx = 0;

            while (bx < CIEGA_BLOQUES_X) {
                if (!ciega[bx]) { bx++; continue; }

                var desde = bx;

                while (bx < CIEGA_BLOQUES_X && ciega[bx]) bx++;

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

    dibujarTramo(ctx, w, h, calor, pts, primera, ultima) {
        var ciegas = this.buscarZonasCiegas(calor);
        var zonas = this.buscarZonas(calor);
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

        var validas = 0;

        for (i = primera; i <= ultima; i++) if (pts[i].ok) validas++;

        for (i = 0; i < zonas.length; i++) {
            var z = zonas[i];
            var zx = z.x0 / CALOR_ANCHO * w, zy = z.y0 / CALOR_ALTO * h;
            var zw = (z.x1 - z.x0 + 1) / CALOR_ANCHO * w, zh = (z.y1 - z.y0 + 1) / CALOR_ALTO * h;

            ctx.fillStyle = "rgba(232,175,82,.28)";
            ctx.strokeStyle = "#E8AF52";
            ctx.fillRect(zx, zy, zw, zh);
            ctx.strokeRect(zx, zy, zw, zh);

            
            var dentro = 0;

            for (var m = primera; m <= ultima; m++) {
                var pm = pts[m];

                if (pm.ok && pm.x * CALOR_ANCHO >= z.x0 && pm.x * CALOR_ANCHO < z.x1 + 1 && pm.y * CALOR_ALTO >= z.y0 && pm.y * CALOR_ALTO < z.y1 + 1) dentro++;
            }

            var texto = (i + 1) + " · " + (validas > 0 ? Math.round(dentro * 100 / validas) : 0) + "%";

            ctx.font = "700 15px sans-serif";

            var ancho = ctx.measureText(texto).width + 14;

            ctx.fillStyle = "#0E1A34";
            ctx.fillRect(zx, zy, ancho, 24);
            ctx.fillStyle = "#fff";
            ctx.textBaseline = "middle";
            ctx.fillText(texto, zx + 7, zy + 12.5);
        }
    }
}
