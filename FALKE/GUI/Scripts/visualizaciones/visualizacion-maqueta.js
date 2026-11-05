"use strict";

// VISUALIZACION DE EJEMPLO

class VisualizacionMaqueta extends Visualizacion {
    constructor(datos) {
        super();

        this.titulo = datos.titulo;
        this.leyenda = datos.leyenda;
        this.dibujoHtml = datos.lienzo;
    }

    abrir(sesion) {
        this.sesion = sesion;
        this.abierta = true;

        this.elTitulo.textContent = this.titulo;
        this.elLeyenda.innerHTML = this.leyenda;
        this.lienzo.innerHTML = this.dibujoHtml;
        this.controles.hidden = true;
    }

    cerrar() {
        if (!this.abierta) return;

        this.abierta = false;
        this.lienzo.innerHTML = "";
    }
}
