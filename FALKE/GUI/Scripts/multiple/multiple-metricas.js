"use strict";

var MUESTRA_PEQUENA = 0.10;

function formatoMinutos(segundos) {
    var s = Math.round(segundos);
    var m = Math.floor(s / 60);
    var ss = s % 60;

    return (m < 10 ? "0" : "") + m + ":" + (ss < 10 ? "0" : "") + ss;
}

function formatoNumero(n) {
    try {
        return new Intl.NumberFormat(document.documentElement.lang || undefined, { maximumFractionDigits: 1 }).format(n);
    } catch (e) {
        return String(Math.round(n * 10) / 10);
    }
}

var METRICAS = [
    {
        nombre: function () { return MultiTextos.duracion; },
        valor: function (s) { return s.duracionSeg > 0 ? s.duracionSeg : null; },
        formato: formatoMinutos
    },
    {
        nombre: function () { return MultiTextos.fijacionMedia; },
        valor: function (s) { return s.conMirada && s.analisis.fijs.length > 0 ? s.analisis.fijMedia : null; },
        formato: function (v) { return formatoNumero(v) + " s"; }
    },
    {
        nombre: function () { return MultiTextos.fijaciones; },
        valor: function (s) { return s.conMirada ? s.analisis.fijs.length : null; },
        formato: function (v) { return formatoNumero(v); }
    },
    {
        nombre: function () { return MultiTextos.atencion; },
        valor: function (s) { return s.conMirada ? s.analisis.atencion : null; },
        formato: function (v) { return Math.round(v) + "%"; }
    }
];

function valoresDe(sesiones, metrica) {
    var valores = [];

    for (var i = 0; i < sesiones.length; i++) {
        var v = metrica.valor(sesiones[i]);

        if (v !== null) valores.push(v);
    }

    return valores;
}

function crear(etiqueta, clase, texto) {
    var el = document.createElement(etiqueta);

    if (clase) el.className = clase;
    if (texto !== undefined) el.textContent = texto;

    return el;
}

function pintarGenerales(contenedor, sesiones) {
    contenedor.innerHTML = "";

    for (var i = 0; i < METRICAS.length; i++) {
        var m = METRICAS[i];
        var valores = valoresDe(sesiones, m);
        var caja = crear("div", "metrica");

        caja.appendChild(crear("div", "metrica-valor", valores.length ? m.formato(media(valores)) : MultiTextos.sinDatos));
        caja.appendChild(crear("div", "metrica-nombre", m.nombre()));

        if (valores.length) {
            caja.appendChild(crear("div", "metrica-nombre", MultiTextos.mediana.replace("{0}", m.formato(mediana(valores)))));
        }

        contenedor.appendChild(caja);
    }

    var combinadas = crear("div", "metrica");

    combinadas.appendChild(crear("div", "metrica-valor", String(sesiones.length)));
    combinadas.appendChild(crear("div", "metrica-nombre", MultiTextos.sesiones));
    contenedor.appendChild(combinadas);
}

function celdaGrupo(sesionesGrupo, sesionesTodas, metrica) {
    var td = document.createElement("td");
    var valores = valoresDe(sesionesGrupo, metrica);

    if (valores.length === 0) {
        td.appendChild(crear("span", "texto-suave", MultiTextos.sinDatos));
        return td;
    }

    td.appendChild(crear("div", "mono", metrica.formato(media(valores))));

    var total = valoresDe(sesionesTodas, metrica).length;

    if (valores.length / total < MUESTRA_PEQUENA) {
        td.appendChild(crear("div", "texto-chico texto-suave", MultiTextos.muestraPequena));
    }

    return td;
}

function dimensionesDelTester(grupos) {
    return [
        { campo: "edad", titulo: MultiTextos.grupoEdad, opciones: grupos.edad },
        { campo: "genero", titulo: MultiTextos.grupoGenero, opciones: grupos.genero },
        { campo: "familiaridad", titulo: MultiTextos.grupoFamiliaridad, opciones: grupos.familiaridad }
    ];
}

function pintarGrupos(contenedor, sesiones, grupos) {
    contenedor.innerHTML = "";

    var dimensiones = dimensionesDelTester(grupos);

    for (var d = 0; d < dimensiones.length; d++) {
        var dim = dimensiones[d];
        var envoltorio = crear("div", "tabla-scroll mb-16");
        var tabla = crear("table", "tabla");
        var cabecera = document.createElement("tr");

        contenedor.appendChild(crear("h3", "mb-8", dim.titulo));

        cabecera.appendChild(crear("th", "", dim.titulo));
        cabecera.appendChild(crear("th", "", MultiTextos.sesiones));

        for (var i = 0; i < METRICAS.length; i++) cabecera.appendChild(crear("th", "", METRICAS[i].nombre()));

        tabla.appendChild(crear("thead")).appendChild(cabecera);

        var cuerpo = crear("tbody");

        for (var o = 0; o < dim.opciones.length; o++) {
            var opcion = dim.opciones[o];
            var delGrupo = [];

            for (var s = 0; s < sesiones.length; s++) {
                if (sesiones[s][dim.campo] === opcion.valor) delGrupo.push(sesiones[s]);
            }

            var fila = document.createElement("tr");

            fila.appendChild(crear("th", "", opcion.texto));
            fila.appendChild(crear("td", "mono", String(delGrupo.length)));

            for (var k = 0; k < METRICAS.length; k++) fila.appendChild(celdaGrupo(delGrupo, sesiones, METRICAS[k]));

            cuerpo.appendChild(fila);
        }

        tabla.appendChild(cuerpo);
        envoltorio.appendChild(tabla);
        contenedor.appendChild(envoltorio);
    }
}
