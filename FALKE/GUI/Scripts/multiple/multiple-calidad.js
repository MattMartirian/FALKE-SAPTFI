"use strict";

var N_MIN_SESIONES = 5;

var CV_DURACION_MAX = 0.5;

function evaluarCalidad(sesiones) {
    var usadas = 0;
    var duraciones = [];

    for (var i = 0; i < sesiones.length; i++) {
        if (sesiones[i].conMirada) usadas++;
        duraciones.push(sesiones[i].duracionSeg);
    }

    var cv = coefVariacion(duraciones);
    var avisos = [];

    if (usadas < N_MIN_SESIONES) avisos.push(MultiTextos.calidadPocas.replace("{0}", N_MIN_SESIONES));
    if (cv >= CV_DURACION_MAX) avisos.push(MultiTextos.calidadDuraciones);

    return {
        total: sesiones.length,
        usadas: usadas,
        excluidas: sesiones.length - usadas,
        cv: cv,
        pocas: usadas < N_MIN_SESIONES,
        variada: cv >= CV_DURACION_MAX,
        avisos: avisos
    };
}

function pintarCalidad(valores, avisos, calidad) {
    valores.innerHTML = "";
    avisos.innerHTML = "";

    var items = [
        [calidad.usadas + " / " + calidad.total, MultiTextos.calidadUsadas],
        [String(calidad.excluidas), MultiTextos.calidadExcluidas],
        [Math.round(calidad.cv * 100) + "%", MultiTextos.calidadVariacion]
    ];

    for (var i = 0; i < items.length; i++) {
        var caja = crear("div", "metrica");

        caja.appendChild(crear("div", "metrica-valor", items[i][0]));
        caja.appendChild(crear("div", "metrica-nombre", items[i][1]));
        valores.appendChild(caja);
    }

    if (calidad.avisos.length === 0) return;

    var aviso = crear("div", "aviso aviso-alerta mt-16 mb-0");
    var texto = crear("p", "sin-margen");

    texto.appendChild(crear("strong", "", MultiTextos.calidadAviso));

    for (var j = 0; j < calidad.avisos.length; j++) {
        texto.appendChild(document.createElement("br"));
        texto.appendChild(document.createTextNode(calidad.avisos[j]));
    }

    aviso.appendChild(texto);
    avisos.appendChild(aviso);
}
