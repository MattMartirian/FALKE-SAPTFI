"use strict";

// SESIONES ELEGIDAS
// arma los datos de cada sesion elegida y baja su archivo de mirada

// devuelve una promesa con la lista de sesiones, cada una con su analisis (o null si no tiene datos de mirada)
function cargarSesiones(casillas, urlArchivo) {
    var pedidos = [];

    for (var i = 0; i < casillas.length; i++) pedidos.push(cargarSesion(casillas[i], urlArchivo));

    return Promise.all(pedidos);
}

function cargarSesion(casilla, urlArchivo) {
    var fila = casilla.closest(".fila-sesion");

    var sesion = {
        nombre: casilla.getAttribute("data-nombre"),
        duracionSeg: Number(fila.getAttribute("data-duracion-seg")) || 0,
        edad: fila.getAttribute("data-edad"),
        genero: fila.getAttribute("data-genero"),
        familiaridad: fila.getAttribute("data-familiaridad"),
        analisis: null,      // el resultado de analizar() sobre el archivo de mirada
        conMirada: false,    // true si tiene al menos un punto de mirada valido
        fallo: false         // true si no se pudo bajar el archivo
    };

    // sin muestras no hay nada que bajar
    if (Number(fila.getAttribute("data-muestras")) === 0) return Promise.resolve(sesion);

    var url = urlArchivo + "?t=" + encodeURIComponent(casilla.value) + "&k=mirada";

    return fetch(url, { credentials: "same-origin", cache: "no-store" })
        .then(function (respuesta) {
            if (!respuesta.ok) throw new Error();

            return respuesta.json();
        })
        .then(function (json) {
            sesion.analisis = analizar(json, Number(fila.getAttribute("data-desfase")) || 0);
            sesion.conMirada = sesion.analisis.validos.length > 0;

            return sesion;
        })
        .catch(function () {
            sesion.fallo = true;

            return sesion;
        });
}
