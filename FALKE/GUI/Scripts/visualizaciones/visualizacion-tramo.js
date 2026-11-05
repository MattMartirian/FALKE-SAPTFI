"use strict";

// VISUALIZACION CON TRAMO (clase intermedia)
// La usan el mapa de calor y las zonas. Agrega lo que tienen en comun como las barras desde/hasta para elegir que parte del video se analiza

var CALOR_ANCHO = 96;  
var CALOR_ALTO = 54;
var CALOR_RADIO = 4;   


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

class VisualizacionTramo extends Visualizacion {
    constructor() {
        super();

        this.opciones = document.getElementById("calorOpciones");
        this.barraDesde = document.getElementById("calorDesde");
        this.barraHasta = document.getElementById("calorHasta");
        this.txtDesde = document.getElementById("calorDesdeTxt");
        this.txtHasta = document.getElementById("calorHastaTxt");
        this.cajaTiempoReal = document.getElementById("calorTiempoRealCaja");
        this.checkTiempoReal = document.getElementById("calorTiempoReal");

        this.tieneTiempoReal = false;   
        this.aviso = null;              
        this.clave = "";                

        this.barraDesde.addEventListener("input", () => {
            if (!this.abierta) return;

            if (Number(this.barraDesde.value) > Number(this.barraHasta.value)) this.barraHasta.value = this.barraDesde.value;

            this.actualizar();
        });

        this.barraHasta.addEventListener("input", () => {
            if (!this.abierta) return;

            if (Number(this.barraHasta.value) < Number(this.barraDesde.value)) this.barraDesde.value = this.barraHasta.value;

            this.actualizar();
        });

        this.checkTiempoReal.addEventListener("change", () => {
            if (this.abierta) this.actualizar();
        });
    }

    alAbrir() {
        this.video.style.opacity = "0.55";

        this.barraDesde.value = 0;
        this.barraHasta.value = 1000;
        this.checkTiempoReal.checked = true;
        this.cajaTiempoReal.hidden = !this.tieneTiempoReal;
        this.opciones.hidden = false;
        this.clave = "";

        this.aviso = document.createElement("p");
        this.aviso.className = "sin-margen";
        this.aviso.hidden = true;
        this.aviso.style.cssText = "position:absolute;inset:0;display:flex;align-items:center;justify-content:center;padding:16px;text-align:center;pointer-events:none;color:var(--texto)";
        this.aviso.textContent = VisTextos.sinDatos;
        this.lienzo.appendChild(this.aviso);
    }

    alCerrar() {
        this.opciones.hidden = true;
        this.aviso = null;
    }

    alCambiarTamano() {
        this.clave = "";
    }

    dibujar() {
        var dur = this.duracion();
        var desdeSeg = dur * Number(this.barraDesde.value) / 1000;
        var hastaSeg = dur * Number(this.barraHasta.value) / 1000;

        this.txtDesde.textContent = aMinutos(desdeSeg);
        this.txtHasta.textContent = aMinutos(hastaSeg);

        var w = this.canvas.width;
        var h = this.canvas.height;
        var ctx = this.canvas.getContext("2d");
        var pts = this.sesion.analisis ? this.sesion.analisis.pts : null;

        if (!pts) {
            ctx.clearRect(0, 0, w, h);
            this.aviso.hidden = !this.sesion.sinMirada;
            return;
        }

        var primera = buscarMuestra(pts, desdeSeg * 1000 - 1) + 1;
        var ultimaTramo = buscarMuestra(pts, hastaSeg * 1000);

        this.aviso.hidden = ultimaTramo >= primera;

        var ultima = ultimaTramo;

        if (this.tieneTiempoReal && this.checkTiempoReal.checked) {
            ultima = Math.min(ultimaTramo, buscarMuestra(pts, this.video.currentTime * 1000));
        }

        var clave = primera + "-" + ultima;

        if (clave === this.clave) return;

        this.clave = clave;
        ctx.clearRect(0, 0, w, h);

        if (ultima < primera) return;

        var calor = this.armarCalor(pts, primera, ultima);

        if (calor.max > 0) this.dibujarTramo(ctx, w, h, calor);
    }

    armarCalor(pts, desde, hasta) {
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

    dibujarTramo(ctx, w, h, calor) { }
}
