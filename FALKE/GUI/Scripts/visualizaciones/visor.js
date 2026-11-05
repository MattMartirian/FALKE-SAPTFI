"use strict";

// VISOR
// maneja el modal donde se ven las visualizaciones
// los botones de abajo (reproducir, +-10 s, linea de tiempo) y el teclado con ella.

var Visor = {
    tipos: {},      
    activa: null,   

    abrir: function (tipo, sesion) {
        var nueva = Visor.tipos[tipo];

        if (!nueva || !sesion) return;

        // si habia otra abierta, la cierro antes
        if (Visor.activa) Visor.activa.cerrar();

        Visor.activa = nueva;
        nueva.abrir(sesion);
    },

    refrescar: function () {
        if (Visor.activa && Visor.activa.abierta) Visor.activa.actualizar();
    },

    iniciar: function () {
        var modal = document.getElementById("modalVisor");

        document.getElementById("btnReproducir").addEventListener("click", function () {
            if (Visor.activa) Visor.activa.alternar();
        });

        document.getElementById("btnAtras").addEventListener("click", function () {
            if (Visor.activa) Visor.activa.saltar(-10);
        });

        document.getElementById("btnAdelante").addEventListener("click", function () {
            if (Visor.activa) Visor.activa.saltar(10);
        });

        document.getElementById("visorLinea").addEventListener("input", function () {
            if (Visor.activa) Visor.activa.irA(this.value);
        });

        document.addEventListener("keydown", function (e) {
            if (!Visor.activa || modal.hidden) return;

            var tag = (e.target && e.target.tagName) || "";

            if (tag === "BUTTON" || tag === "INPUT" || tag === "SELECT" || tag === "TEXTAREA") return;

            if (e.key === " ") { e.preventDefault(); Visor.activa.alternar(); }
            else if (e.key === "ArrowLeft") Visor.activa.saltar(-5);
            else if (e.key === "ArrowRight") Visor.activa.saltar(5);
        });

        new MutationObserver(function () {
            if (modal.hidden && Visor.activa) Visor.activa.cerrar();
        }).observe(modal, { attributes: true, attributeFilter: ["hidden"] });
    }
};
