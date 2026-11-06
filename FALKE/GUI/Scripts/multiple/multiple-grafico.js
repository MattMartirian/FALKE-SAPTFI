"use strict";

class GraficoMultiple {
    constructor(idCaja) {
        this.canvas = document.createElement("canvas");
        this.canvas.width = 960;
        this.canvas.height = 540;

        document.getElementById(idCaja).appendChild(this.canvas);

        this.cuadricula = true;
    }

    dibujar(sesiones) {
        var ctx = this.canvas.getContext("2d");
        var w = this.canvas.width;
        var h = this.canvas.height;

        ctx.clearRect(0, 0, w, h);

        if (this.cuadricula) this.dibujarCuadricula(ctx, w, h);

        if (sesiones.length === 0) {
            this.escribirSinDatos(ctx, w, h);
            return;
        }

        this.dibujarSesiones(ctx, w, h, sesiones);
    }

    dibujarCuadricula(ctx, w, h) {
        ctx.lineWidth = 1;
        ctx.strokeStyle = "rgba(124,135,156,.28)";

        for (var i = 1; i < 4; i++) {
            ctx.beginPath();
            ctx.moveTo(w * i / 4, 0);
            ctx.lineTo(w * i / 4, h);
            ctx.moveTo(0, h * i / 4);
            ctx.lineTo(w, h * i / 4);
            ctx.stroke();
        }
    }

    escribirSinDatos(ctx, w, h) {
        ctx.fillStyle = "#7C879C";
        ctx.font = "600 28px sans-serif";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillText(MultiTextos.sinDatos, w / 2, h / 2);
        ctx.textAlign = "start";
    }

    dibujarSesiones(ctx, w, h, sesiones) { }
}
