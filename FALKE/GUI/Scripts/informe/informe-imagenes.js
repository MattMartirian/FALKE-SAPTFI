"use strict";

var INFORME_ANCHO_IMAGEN = 960;

var InformeImagenes = {
    lienzo: function () {
        var ancho = INFORME_ANCHO_IMAGEN;
        var alto = Math.round(ancho * 9 / 16);
        var canvas = document.createElement("canvas");
        var ctx = canvas.getContext("2d");

        canvas.width = ancho;
        canvas.height = alto;

        ctx.fillStyle = "#FAF9F4";
        ctx.fillRect(0, 0, ancho, alto);

        ctx.lineWidth = 1;
        ctx.strokeStyle = "rgba(124,135,156,.28)";

        for (var i = 1; i < 4; i++) {
            ctx.beginPath();
            ctx.moveTo(ancho * i / 4, 0);
            ctx.lineTo(ancho * i / 4, alto);
            ctx.moveTo(0, alto * i / 4);
            ctx.lineTo(ancho, alto * i / 4);
            ctx.stroke();
        }

        return canvas;
    },

    visualizacion: function (tipo, tipos, analisis, segundo) {
        var canvas = InformeImagenes.lienzo();
        var ctx = canvas.getContext("2d");
        var pts = analisis ? analisis.pts : [];

        if (pts.length > 0) {
            if (tipo === "grabacion") tipos.grabacion.dibujarMirada(ctx, canvas.width, canvas.height, pts, segundo * 1000);
            else tipos[tipo].dibujarTramo(ctx, canvas.width, canvas.height, pts, 0, pts.length - 1);
        }

        return canvas;
    },

    atencion: function (analisis, duracionSeg) {
        var canvas = document.createElement("canvas");
        var ctx = canvas.getContext("2d");
        var ancho = INFORME_ANCHO_IMAGEN, alto = Math.round(INFORME_ANCHO_IMAGEN * 9 / 16);

        canvas.width = ancho;
        canvas.height = alto;

        ctx.fillStyle = "#FFFFFF";
        ctx.fillRect(0, 0, ancho, alto);

        var porcentaje = analisis ? analisis.atencion : 0;
        var cx = ancho / 2, cy = 200, radio = 135;

        ctx.lineWidth = 50;
        ctx.strokeStyle = analisis ? "#56637C" : "rgba(124,135,156,.25)";
        ctx.beginPath();
        ctx.arc(cx, cy, radio, 0, Math.PI * 2);
        ctx.stroke();

        if (porcentaje > 0) {
            ctx.strokeStyle = "#E8AF52";
            ctx.beginPath();
            ctx.arc(cx, cy, radio, -Math.PI / 2, -Math.PI / 2 + Math.PI * 2 * porcentaje / 100);
            ctx.stroke();
        }

        ctx.fillStyle = "#0E1A34";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.font = "700 62px 'Space Grotesk', sans-serif";
        ctx.fillText(analisis ? porcentaje + "%" : "-", cx, cy);

        var x0 = 60, x1 = ancho - 60, y0 = 395, altoFranja = 64;
        var anchoFranja = x1 - x0;

        ctx.fillStyle = "rgba(124,135,156,.25)";
        ctx.fillRect(x0, y0, anchoFranja, altoFranja);

        if (analisis && duracionSeg > 0) {
            var pts = analisis.pts;
            var estados = estadosAtencion(pts, analisis.fijs);
            var durMs = duracionSeg * 1000;
            var anchoMuestra = Math.max(1, INTERVALO_MS / durMs * anchoFranja);

            for (var i = 0; i < pts.length; i++) {
                if (estados[i] === 0) continue;

                ctx.fillStyle = estados[i] === 2 ? "#E8AF52" : "#56637C";
                ctx.fillRect(x0 + Math.min(1, pts[i].t / durMs) * anchoFranja, y0, anchoMuestra, altoFranja);
            }
        }

        ctx.fillStyle = "#56637C";
        ctx.font = "400 22px 'Space Mono', monospace";
        ctx.textBaseline = "top";
        ctx.textAlign = "left";
        ctx.fillText("00:00", x0, y0 + altoFranja + 12);
        ctx.textAlign = "center";
        ctx.fillText(aMinutos(duracionSeg / 2), x0 + anchoFranja / 2, y0 + altoFranja + 12);
        ctx.textAlign = "right";
        ctx.fillText(aMinutos(duracionSeg), x1, y0 + altoFranja + 12);

        return canvas;
    },

    logo: function (url) {
        return new Promise(function (resolver) {
            var imagen = new Image();

            imagen.onload = function () {
                try {
                    var canvas = document.createElement("canvas");

                    canvas.width = 700;
                    canvas.height = 200;
                    canvas.getContext("2d").drawImage(imagen, 0, 0, canvas.width, canvas.height);

                    resolver(canvas.toDataURL("image/png"));
                } catch (e) {
                    resolver(null);
                }
            };

            imagen.onerror = function () { resolver(null); };
            imagen.src = url;
        });
    }
};
