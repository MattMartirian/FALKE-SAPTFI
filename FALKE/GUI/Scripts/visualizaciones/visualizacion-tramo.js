"use strict";

// VISUALIZACION CON TRAMO (clase intermedia)
// La usan el mapa de calor y las zonas. Agrega lo que tienen en comun como las barras desde/hasta para elegir que parte del video se analiza

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
        this.etiquetaTiempoReal = this.cajaTiempoReal.querySelector("span");
        this.textoOriginal = this.etiquetaTiempoReal.textContent;
        this.textoTiempoReal = null;   
        this.cajaMirada = document.getElementById("calorMiradaCaja");
        this.checkMirada = document.getElementById("calorMirada");
        this.capaMirada = null;   

        this.tieneTiempoReal = false;   
        this.tieneMirada = true;     
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

        this.checkMirada.addEventListener("change", () => {
            if (this.abierta) this.actualizar();
        });
    }

    alAbrir() {
        this.video.style.opacity = "0.55";

        this.barraDesde.value = 0;
        this.barraHasta.value = 1000;
        this.checkTiempoReal.checked = true;
        this.checkMirada.checked = false;
        this.cajaMirada.hidden = !this.tieneMirada;
        this.cajaTiempoReal.hidden = !this.tieneTiempoReal;
        this.etiquetaTiempoReal.textContent = this.textoTiempoReal || this.textoOriginal;
        this.opciones.hidden = false;
        this.clave = "";

        this.aviso = document.createElement("p");
        this.aviso.className = "sin-margen";
        this.aviso.hidden = true;
        this.aviso.style.cssText = "position:absolute;inset:0;display:flex;align-items:center;justify-content:center;padding:16px;text-align:center;pointer-events:none;color:var(--texto)";
        this.aviso.textContent = VisTextos.sinDatos;
        this.lienzo.appendChild(this.aviso);

        // el canvas principal solo se redibuja cuando cambia el tramo, asi que el punto va en otro
        this.capaMirada = document.createElement("canvas");
        this.capaMirada.style.cssText = "position:absolute;inset:0;width:100%;height:100%;pointer-events:none";
        this.lienzo.appendChild(this.capaMirada);
    }

    alCerrar() {
        this.opciones.hidden = true;
        this.aviso = null;
        this.capaMirada = null;
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

        this.dibujarCapaMirada();

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

        this.dibujarTramo(ctx, w, h, pts, primera, ultima);
    }

    dibujarCapaMirada() {
        var capa = this.capaMirada;

        if (capa.width !== this.canvas.width || capa.height !== this.canvas.height) {
            capa.width = this.canvas.width;
            capa.height = this.canvas.height;
        }

        var ctx = capa.getContext("2d");

        ctx.clearRect(0, 0, capa.width, capa.height);

        if (!this.checkMirada.checked || !this.sesion.analisis) return;

        this.dibujarMirada(ctx, capa.width, capa.height, this.sesion.analisis.pts, this.video.currentTime * 1000);
    }

    dibujarTramo(ctx, w, h, pts, primera, ultima) { }
}
