"use strict";

// FILTRO POR PERFIL DEL TESTER
// tres grupos de casillas (edad, genero, familiaridad), que arrancan todas marcadas. Dentro de un grupo alcanza con cumplir
// una opcion marcada; entre grupos hay que cumplir todos. Un grupo sin nada marcado cuenta como "todos"

function armarFiltros(contenedor, grupos) {
    contenedor.innerHTML = "";

    var dimensiones = dimensionesDelTester(grupos);

    for (var d = 0; d < dimensiones.length; d++) {
        var dim = dimensiones[d];
        var conjunto = crear("fieldset", "filtro-grupo");
        var opciones = crear("div", "filtro-opciones");

        conjunto.appendChild(crear("legend", "", dim.titulo));

        for (var o = 0; o < dim.opciones.length; o++) {
            var etiqueta = crear("label", "filtro-opcion");
            var casilla = document.createElement("input");

            casilla.type = "checkbox";
            casilla.value = dim.opciones[o].valor;
            casilla.checked = true;
            casilla.setAttribute("data-campo", dim.campo);

            etiqueta.appendChild(casilla);
            etiqueta.appendChild(crear("span", "", dim.opciones[o].texto));
            opciones.appendChild(etiqueta);
        }

        conjunto.appendChild(opciones);
        contenedor.appendChild(conjunto);
    }
}

// los valores marcados de un grupo (vacio = no se filtra por ese grupo)
function valoresMarcados(contenedor, campo) {
    var casillas = contenedor.querySelectorAll("input[data-campo='" + campo + "']:checked");
    var valores = [];

    for (var i = 0; i < casillas.length; i++) valores.push(casillas[i].value);

    return valores;
}

function filtrarSesiones(contenedor, sesiones) {
    var campos = ["edad", "genero", "familiaridad"];
    var marcados = {};
    var i, c;

    for (c = 0; c < campos.length; c++) marcados[campos[c]] = valoresMarcados(contenedor, campos[c]);

    var resultado = [];

    for (i = 0; i < sesiones.length; i++) {
        var cumple = true;

        for (c = 0; c < campos.length; c++) {
            var lista = marcados[campos[c]];

            if (lista.length > 0 && lista.indexOf(sesiones[i][campos[c]]) < 0) cumple = false;
        }

        if (cumple) resultado.push(sesiones[i]);
    }

    return resultado;
}

function marcarTodos(contenedor) {
    var casillas = contenedor.querySelectorAll("input[type=checkbox]");

    for (var i = 0; i < casillas.length; i++) casillas[i].checked = true;
}

// los grupos que quedaron marcados (para mostrar solo esas filas en las metricas por grupo)
function gruposMarcados(contenedor, grupos) {
    var campos = ["edad", "genero", "familiaridad"];
    var resultado = {};

    for (var c = 0; c < campos.length; c++) {
        var campo = campos[c];
        var marcados = valoresMarcados(contenedor, campo);

        resultado[campo] = [];

        for (var o = 0; o < grupos[campo].length; o++) {
            // sin nada marcado cuenta como todos
            if (marcados.length === 0 || marcados.indexOf(grupos[campo][o].valor) >= 0) resultado[campo].push(grupos[campo][o]);
        }
    }

    return resultado;
}
