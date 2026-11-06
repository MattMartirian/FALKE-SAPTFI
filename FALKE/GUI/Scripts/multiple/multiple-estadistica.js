"use strict";

// ESTADISTICA BASICA
// las usa el analisis multiple para combinar los valores de varias sesiones

function media(lista) {
    var suma = 0;

    for (var i = 0; i < lista.length; i++) suma += lista[i];

    return suma / lista.length;
}

// el valor del medio de la lista ordenada (si son pares, el promedio de los dos del medio)
function mediana(lista) {
    var orden = lista.slice().sort(function (a, b) { return a - b; });
    var medio = Math.floor(orden.length / 2);

    return orden.length % 2 === 1 ? orden[medio] : (orden[medio - 1] + orden[medio]) / 2;
}

// coeficiente de variacion = desvio / media. 0 = todos iguales; cuanto mas alto, mas distintos entre si
function coefVariacion(lista) {
    var promedio = media(lista);

    if (lista.length < 2 || promedio === 0) return 0;

    var suma = 0;

    for (var i = 0; i < lista.length; i++) suma += (lista[i] - promedio) * (lista[i] - promedio);

    return Math.sqrt(suma / lista.length) / promedio;
}
