"use strict";

// RECORRIDO DE LA MIRADA
// une los puntos del tramo como un trazo (cuerpo de serpiente): lo mas viejo es chico y azul, lo mas nuevo es grande y coral

// radio de los extremos (en px, con el lienzo en 960 de ancho) y sus colores [r, g, b]
var RECORRIDO_RADIO_VIEJO = 2;
var RECORRIDO_RADIO_NUEVO = 9;
var RECORRIDO_COLOR_VIEJO = [86, 99, 124];
var RECORRIDO_COLOR_NUEVO = [242, 97, 87];

// reborde del primer y del ultimo punto
var RECORRIDO_COLOR_BORDE = "#fff";
var RECORRIDO_ANCHO_BORDE = 3;

class VisualizacionRecorrido extends VisualizacionTramo {
    constructor() {
        super();

        this.titulo = VisTextos.recorridoTitulo;
        this.leyenda = VisTextos.recorridoLeyenda;
        this.tieneTiempoReal = true;
        this.tieneMirada = false;
        this.textoTiempoReal = VisTextos.recorridoTiempoReal;
    }

    // valor entre a y b segun f (f = 0 da a, f = 1 da b)
    mezclar(a, b, f) {
        return a + (b - a) * f;
    }

    // radio y color de un punto; f va de 0 (el mas viejo del tramo) a 1 (el mas nuevo)
    estilo(f, escala) {
        var v = RECORRIDO_COLOR_VIEJO;
        var n = RECORRIDO_COLOR_NUEVO;

        return {
            radio: this.mezclar(RECORRIDO_RADIO_VIEJO, RECORRIDO_RADIO_NUEVO, f) * escala,
            color: "rgb(" + Math.round(this.mezclar(v[0], n[0], f)) + "," +
                            Math.round(this.mezclar(v[1], n[1], f)) + "," +
                            Math.round(this.mezclar(v[2], n[2], f)) + ")"
        };
    }

    dibujarTramo(ctx, w, h, pts, primera, ultima) {
        // solo las muestras con mirada
        var puntos = [];

        for (var i = primera; i <= ultima; i++) {
            if (pts[i].ok) puntos.push({ x: pts[i].x * w, y: pts[i].y * h, t: pts[i].t });
        }

        if (puntos.length === 0) return;

        var escala = w / 960;
        var cantidad = puntos.length;

        ctx.lineCap = "round";

        // del mas viejo al mas nuevo, asi lo nuevo queda arriba
        for (var k = 0; k < cantidad; k++) {
            var p = puntos[k];
            var f = cantidad > 1 ? k / (cantidad - 1) : 1;   // siempre de 0 a 1, sin importar cuantos puntos haya
            var e = this.estilo(f, escala);

            // trazo desde el punto anterior (si no hay un hueco grande sin datos entre los dos)
            if (k > 0 && p.t - puntos[k - 1].t <= HUECO_MAX_MS) {
                ctx.beginPath();
                ctx.moveTo(puntos[k - 1].x, puntos[k - 1].y);
                ctx.lineTo(p.x, p.y);
                ctx.lineWidth = e.radio * 2;
                ctx.strokeStyle = e.color;
                ctx.stroke();
            }

            ctx.beginPath();
            ctx.arc(p.x, p.y, e.radio, 0, Math.PI * 2);
            ctx.fillStyle = e.color;
            ctx.fill();
        }

        this.marcarExtremo(ctx, puntos[0], 0, escala);
        this.marcarExtremo(ctx, puntos[cantidad - 1], 1, escala);
    }

    // circulo con reborde blanco sobre el primer o el ultimo punto
    marcarExtremo(ctx, punto, f, escala) {
        var e = this.estilo(f, escala);

        ctx.beginPath();
        ctx.arc(punto.x, punto.y, e.radio, 0, Math.PI * 2);
        ctx.fillStyle = e.color;
        ctx.fill();
        ctx.lineWidth = RECORRIDO_ANCHO_BORDE;
        ctx.strokeStyle = RECORRIDO_COLOR_BORDE;
        ctx.stroke();
    }
}
