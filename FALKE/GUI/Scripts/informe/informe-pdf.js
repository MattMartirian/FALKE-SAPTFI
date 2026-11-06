"use strict";

var PDF_ANCHO = 210;
var PDF_ALTO = 297;
var PDF_MARGEN = 16;
var PDF_CONTENIDO = PDF_ANCHO - PDF_MARGEN * 2;
var PDF_LIMITE = PDF_ALTO - 18;
var PDF_RELLENO = 5;
var PT = 0.3528;
var INTERLINEADO = 1.3;

var PDF_AZUL = [14, 26, 52];
var PDF_DORADO = [232, 175, 82];
var PDF_CORAL = [242, 97, 87];
var PDF_GRIS = [86, 99, 124];
var PDF_TENUE = [124, 135, 156];
var PDF_BORDE = [224, 222, 210];
var PDF_SUPERFICIE = [250, 249, 244];
var PDF_ACENTO_SUAVE = [251, 241, 220];
var PDF_BLANCO = [255, 255, 255];
var PDF_PERLA = [252, 252, 247];
var PDF_PERLA_SUAVE = [196, 202, 216];

var PDF_VISUALIZACIONES = ["calor", "zonas", "dispersion", "atencion", "recorrido"];

class InformePdf {
    constructor(datos, logo) {
        this.datos = datos;
        this.textos = datos.textos || {};
        this.logo = logo;
        this.y = 0;
        this.figuras = 0;
        this.formatoNumero = new Intl.NumberFormat(datos.idioma || undefined, { maximumFractionDigits: 1 });

        this.doc = new jspdf.jsPDF({ unit: "mm", format: "a4", compress: true });
        this.doc.setLineHeightFactor(INTERLINEADO);
        cargarFuentesInforme(this.doc);
    }

    T(clave) {
        return this.textos[clave] || clave;
    }

    formato(clave) {
        var texto = this.T(clave);

        for (var i = 1; i < arguments.length; i++) texto = texto.split("{" + (i - 1) + "}").join(arguments[i]);

        return texto;
    }

    numero(n) {
        return this.formatoNumero.format(n);
    }

    porcentaje(n) {
        return this.numero(n) + "%";
    }

    fuente(tipo, tamano, color) {
        var doc = this.doc;

        if (tipo === "semi") doc.setFont("ManropeSemi", "normal");
        else if (tipo === "negrita") doc.setFont("Manrope", "bold");
        else if (tipo === "titulo") doc.setFont("SpaceGrotesk", "bold");
        else if (tipo === "mono") doc.setFont("SpaceMono", "bold");
        else doc.setFont("Manrope", "normal");

        doc.setFontSize(tamano);
        doc.setTextColor(color[0], color[1], color[2]);
    }

    altoLineas(lineas, tamano) {
        return lineas * tamano * PT * INTERLINEADO;
    }

    cortar(texto, ancho) {
        return this.doc.splitTextToSize(String(texto), ancho);
    }

    escribir(lineas, x, y, opciones) {
        var o = opciones || {};

        o.baseline = "top";
        this.doc.text(lineas, x, y, o);
    }

    espacio(alto) {
        if (this.y + alto > PDF_LIMITE) this.nuevaPagina();
    }

    nuevaPagina() {
        this.doc.addPage();
        this.encabezado();
    }

    encabezado() {
        var doc = this.doc;

        doc.setFillColor(PDF_AZUL[0], PDF_AZUL[1], PDF_AZUL[2]);
        doc.rect(0, 0, PDF_ANCHO, 13, "F");

        if (this.logo) doc.addImage(this.logo, "PNG", PDF_MARGEN, 3.2, 23, 6.6);

        this.fuente("semi", 8, PDF_PERLA_SUAVE);
        var texto = this.cortar(this.datos.sesion || "", PDF_CONTENIDO - 40)[0] || "";
        this.escribir(texto, PDF_ANCHO - PDF_MARGEN, 4.6, { align: "right" });

        this.y = 23;
    }

    portada() {
        var doc = this.doc;
        var d = this.datos;

        this.fuente("titulo", 19, PDF_BLANCO);
        var titulo = this.cortar(this.T("informe_titulo"), PDF_CONTENIDO);
        this.fuente("semi", 11.5, PDF_DORADO);
        var sesion = this.cortar(d.sesion || "", PDF_CONTENIDO);

        var yTitulo = 33;
        var yBajada = yTitulo + this.altoLineas(titulo.length, 19) + 1.5;
        var altoBanda = yBajada + this.altoLineas(sesion.length, 11.5) + 9;

        doc.setFillColor(PDF_AZUL[0], PDF_AZUL[1], PDF_AZUL[2]);
        doc.rect(0, 0, PDF_ANCHO, altoBanda, "F");

        doc.setFillColor(PDF_DORADO[0], PDF_DORADO[1], PDF_DORADO[2]);
        doc.rect(0, altoBanda, PDF_ANCHO, 1.2, "F");

        if (this.logo) {
            doc.addImage(this.logo, "PNG", PDF_MARGEN, 11, 38, 10.9);
        } else {
            this.fuente("titulo", 16, PDF_BLANCO);
            this.escribir("FALKE", PDF_MARGEN, 12);
        }

        this.fuente("texto", 7.5, PDF_PERLA_SUAVE);
        this.escribir(this.T("informe_emision") + ": " + (d.fechaEmision || ""), PDF_ANCHO - PDF_MARGEN, 12.5, { align: "right" });
        if (d.emitidoPor) this.escribir(this.T("informe_emitido_por") + ": " + d.emitidoPor, PDF_ANCHO - PDF_MARGEN, 16.8, { align: "right" });

        this.fuente("titulo", 19, PDF_BLANCO);
        this.escribir(titulo, PDF_MARGEN, yTitulo);
        this.fuente("semi", 11.5, PDF_DORADO);
        this.escribir(sesion, PDF_MARGEN, yBajada);

        this.y = altoBanda + 11;
    }

    seccion(titulo, siguiente) {
        this.espacio(9 + (siguiente || 0));

        this.doc.setFillColor(PDF_DORADO[0], PDF_DORADO[1], PDF_DORADO[2]);
        this.doc.rect(PDF_MARGEN, this.y + 0.6, 1.3, 4.6, "F");

        this.fuente("titulo", 12, PDF_AZUL);
        this.escribir(titulo, PDF_MARGEN + 4, this.y);

        this.y += 8.5;
    }

    tarjeta(x, y, ancho, alto, relleno) {
        var c = relleno || PDF_BLANCO;

        this.doc.setDrawColor(PDF_BORDE[0], PDF_BORDE[1], PDF_BORDE[2]);
        this.doc.setLineWidth(0.3);
        this.doc.setFillColor(c[0], c[1], c[2]);
        this.doc.roundedRect(x, y, ancho, alto, 2, 2, "FD");
    }

    tablaDatos(titulo, filas) {
        var datos = filas.filter(function (f) { return f[1] !== null && f[1] !== undefined && String(f[1]).trim() !== ""; });

        if (datos.length === 0) return;

        var anchoColumna = (PDF_CONTENIDO - PDF_RELLENO * 2 - 8) / 2;
        var renglones = [];
        var alto = PDF_RELLENO * 2 - 3;

        for (var i = 0; i < datos.length; i += 2) {
            var celdas = [];
            var altoRenglon = 0;

            for (var j = i; j < i + 2 && j < datos.length; j++) {
                this.fuente("semi", 7.5, PDF_TENUE);
                var etiqueta = this.cortar(datos[j][0], anchoColumna);
                this.fuente("texto", 9.5, PDF_AZUL);
                var valor = this.cortar(datos[j][1], anchoColumna);
                var altoCelda = this.altoLineas(etiqueta.length, 7.5) + 0.6 + this.altoLineas(valor.length, 9.5);

                celdas.push({ etiqueta: etiqueta, valor: valor });
                altoRenglon = Math.max(altoRenglon, altoCelda);
            }

            renglones.push({ celdas: celdas, alto: altoRenglon });
            alto += altoRenglon + 3;
        }

        this.seccion(titulo, alto);
        this.tarjeta(PDF_MARGEN, this.y, PDF_CONTENIDO, alto, PDF_SUPERFICIE);

        var y = this.y + PDF_RELLENO;

        for (var r = 0; r < renglones.length; r++) {
            for (var c = 0; c < renglones[r].celdas.length; c++) {
                var x = PDF_MARGEN + PDF_RELLENO + c * (anchoColumna + 8);
                var celda = renglones[r].celdas[c];

                this.fuente("semi", 7.5, PDF_TENUE);
                this.escribir(celda.etiqueta, x, y);
                this.fuente("texto", 9.5, PDF_AZUL);
                this.escribir(celda.valor, x, y + this.altoLineas(celda.etiqueta.length, 7.5) + 0.6);
            }

            y += renglones[r].alto + 3;
        }

        this.y += alto + 8;
    }

    parrafo(titulo, texto) {
        if (!texto) return;

        this.fuente("texto", 9.5, PDF_AZUL);
        var lineas = this.cortar(texto, PDF_CONTENIDO - PDF_RELLENO * 2);
        var alto = this.altoLineas(lineas.length, 9.5) + PDF_RELLENO * 2;

        this.seccion(titulo, alto);
        this.tarjeta(PDF_MARGEN, this.y, PDF_CONTENIDO, alto, PDF_SUPERFICIE);
        this.fuente("texto", 9.5, PDF_AZUL);
        this.escribir(lineas, PDF_MARGEN + PDF_RELLENO, this.y + PDF_RELLENO);

        this.y += alto + 8;
    }

    metricas(lista, titulo) {
        var separacion = 3;
        var ancho = (PDF_CONTENIDO - separacion * (lista.length - 1)) / lista.length;
        var altoNombre = 0;
        var altoDetalle = 0;
        var nombres = [];
        var detalles = [];

        for (var i = 0; i < lista.length; i++) {
            this.fuente("texto", 7.5, PDF_GRIS);
            nombres.push(this.cortar(lista[i].nombre, ancho - 6));
            altoNombre = Math.max(altoNombre, this.altoLineas(nombres[i].length, 7.5));

            this.fuente("texto", 7, PDF_TENUE);
            detalles.push(lista[i].detalle ? this.cortar(lista[i].detalle, ancho - 6) : []);
            altoDetalle = Math.max(altoDetalle, this.altoLineas(detalles[i].length, 7));
        }

        var alto = 4.5 + 7 + 1.5 + altoNombre + (altoDetalle > 0 ? 1 + altoDetalle : 0) + 4.5;

        this.seccion(titulo || this.T("informe_seccion_metricas"), alto);

        for (i = 0; i < lista.length; i++) {
            var x = PDF_MARGEN + i * (ancho + separacion);

            this.tarjeta(x, this.y, ancho, alto, PDF_BLANCO);
            this.doc.setFillColor(PDF_DORADO[0], PDF_DORADO[1], PDF_DORADO[2]);
            this.doc.rect(x + 3, this.y, 8, 0.9, "F");

            this.fuente("mono", 13, PDF_AZUL);
            this.escribir(lista[i].valor, x + 3, this.y + 4.5);
            this.fuente("texto", 7.5, PDF_GRIS);
            this.escribir(nombres[i], x + 3, this.y + 4.5 + 7 + 1.5);

            if (detalles[i].length > 0) {
                this.fuente("texto", 7, PDF_TENUE);
                this.escribir(detalles[i], x + 3, this.y + 4.5 + 7 + 1.5 + altoNombre + 1);
            }
        }

        this.y += alto + 8;
    }

    muestraLeyenda(item, x, y) {
        var doc = this.doc;
        var lado = 3;

        doc.setLineWidth(0.35);
        doc.setLineDashPattern(item.punteado ? [0.7, 0.5] : [], 0);
        doc.setFillColor(item.relleno[0], item.relleno[1], item.relleno[2]);

        var borde = item.borde || item.relleno;
        doc.setDrawColor(borde[0], borde[1], borde[2]);

        if (item.circulo) doc.circle(x + lado / 2, y + lado / 2, lado / 2, "FD");
        else doc.roundedRect(x, y, lado, lado, 0.5, 0.5, "FD");

        doc.setLineDashPattern([], 0);
    }

    medirTextoFigura(f, ancho) {
        this.fuente("negrita", 10.5, PDF_AZUL);
        f.lineasTitulo = this.cortar(f.titulo, ancho);
        this.fuente("texto", 8, PDF_GRIS);
        f.lineasDescripcion = this.cortar(f.descripcion, ancho);

        var alto = this.altoLineas(f.lineasTitulo.length, 10.5) + 1.2 + this.altoLineas(f.lineasDescripcion.length, 8) + 3.5;

        this.fuente("texto", 7.5, PDF_AZUL);
        f.lineasLeyenda = [];

        for (var i = 0; i < f.leyenda.length; i++) {
            f.lineasLeyenda.push(this.cortar(f.leyenda[i].texto, ancho - 5));
            alto += this.altoLineas(f.lineasLeyenda[i].length, 7.5) + 1.4;
        }

        if (f.datos.length > 0) alto += 2.2;

        this.fuente("semi", 8, PDF_AZUL);
        f.lineasDatos = [];

        for (i = 0; i < f.datos.length; i++) {
            f.lineasDatos.push(this.cortar(f.datos[i], ancho));
            alto += this.altoLineas(f.lineasDatos[i].length, 8) + 1;
        }

        return alto;
    }

    escribirTextoFigura(f, x, y, ancho) {
        this.fuente("negrita", 10.5, PDF_AZUL);
        this.escribir(f.lineasTitulo, x, y);
        y += this.altoLineas(f.lineasTitulo.length, 10.5) + 1.2;

        this.fuente("texto", 8, PDF_GRIS);
        this.escribir(f.lineasDescripcion, x, y);
        y += this.altoLineas(f.lineasDescripcion.length, 8) + 3.5;

        for (var i = 0; i < f.leyenda.length; i++) {
            this.muestraLeyenda(f.leyenda[i], x, y + 0.1);
            this.fuente("texto", 7.5, PDF_AZUL);
            this.escribir(f.lineasLeyenda[i], x + 5, y);
            y += this.altoLineas(f.lineasLeyenda[i].length, 7.5) + 1.4;
        }

        if (f.datos.length > 0) {
            this.doc.setDrawColor(PDF_BORDE[0], PDF_BORDE[1], PDF_BORDE[2]);
            this.doc.setLineWidth(0.3);
            this.doc.line(x, y + 0.6, x + ancho, y + 0.6);
            y += 2.2;
        }

        this.fuente("semi", 8, PDF_AZUL);

        for (i = 0; i < f.datos.length; i++) {
            this.escribir(f.lineasDatos[i], x, y);
            y += this.altoLineas(f.lineasDatos[i].length, 8) + 1;
        }
    }

    figura(f) {
        this.figuras++;

        var interior = PDF_CONTENIDO - PDF_RELLENO * 2;
        var anchoImagen = 104;
        var altoImagen = anchoImagen * f.canvas.height / f.canvas.width;
        var anchoTexto = interior - anchoImagen - 6;

        this.fuente("texto", 7, PDF_TENUE);
        var pie = this.cortar(this.formato("informe_figura", this.figuras) + ". " + f.pie, anchoImagen);
        var altoPie = this.altoLineas(pie.length, 7);
        var altoTexto = this.medirTextoFigura(f, anchoTexto);
        var alto = PDF_RELLENO * 2 + Math.max(altoImagen + 1.8 + altoPie, altoTexto);

        this.espacio(alto);
        this.tarjeta(PDF_MARGEN, this.y, PDF_CONTENIDO, alto, PDF_BLANCO);

        var x = PDF_MARGEN + PDF_RELLENO;
        var y = this.y + PDF_RELLENO;

        this.doc.addImage(f.canvas.toDataURL("image/jpeg", 0.92), "JPEG", x, y, anchoImagen, altoImagen);
        this.doc.setDrawColor(PDF_BORDE[0], PDF_BORDE[1], PDF_BORDE[2]);
        this.doc.setLineWidth(0.3);
        this.doc.rect(x, y, anchoImagen, altoImagen, "S");

        this.fuente("texto", 7, PDF_TENUE);
        this.escribir(pie, x, y + altoImagen + 1.8);

        this.escribirTextoFigura(f, x + anchoImagen + 6, y, anchoTexto);

        this.y += alto + 5;
    }

    armarFigura(tipo, o) {
        var a = o.analisis;
        var duracionSeg = o.sesion.duracionSeg;
        var tramo = this.formato("informe_tramo", "00:00", aMinutos(duracionSeg));
        var sinMirada = [this.T("informe_sin_mirada")];
        var f = { leyenda: [], datos: [], pie: tramo };

        if (tipo === "calor") {
            f.titulo = this.T("analisismultiple_mapa_de_calor");
            f.descripcion = this.T("informe_desc_calor");
            f.leyenda = [
                { relleno: PDF_CORAL, texto: this.T("visualizaciones_mayor_concentracion") },
                { relleno: PDF_DORADO, texto: this.T("visualizaciones_concentracion_media") },
                { relleno: PDF_GRIS, texto: this.T("visualizaciones_menor_concentracion") }
            ];
            f.datos = a ? [this.formato("informe_dato_muestras_validas", this.numero(a.validos.length), this.numero(a.pts.length),
                this.porcentaje(a.pts.length ? a.validos.length * 100 / a.pts.length : 0))] : sinMirada;
        }
        else if (tipo === "zonas") {
            f.titulo = this.T("analisismultiple_zonas_de_interes_y_ciegas");
            f.descripcion = this.T("informe_desc_zonas");
            f.leyenda = [
                { relleno: [250, 236, 212], borde: PDF_DORADO, texto: this.T("visualizaciones_zona_de_interes") },
                { relleno: [239, 240, 243], borde: PDF_GRIS, punteado: true, texto: this.T("visualizaciones_zona_ciega") }
            ];
            f.datos = sinMirada;

            if (a && a.pts.length > 0) {
                var calor = calcularCalor(a.pts, 0, a.pts.length - 1);

                if (calor.max > 0) {
                    var zonas = buscarZonas(calor.grilla, calor.max * ZONA_UMBRAL).length;
                    var ciegos = bloquesCiegos(calor.grilla, calor.max).filter(function (c) { return c; }).length;

                    f.datos = [this.formato("informe_dato_zonas", zonas),
                               this.formato("informe_dato_ciegas", ciegos, CIEGA_BLOQUES_X * CIEGA_BLOQUES_Y)];
                }
            }
        }
        else if (tipo === "dispersion") {
            f.titulo = this.T("analisismultiple_dispersion_de_mirada");
            f.descripcion = this.T("informe_desc_dispersion");
            f.leyenda = [
                { relleno: PDF_GRIS, circulo: true, texto: this.T("visualizaciones_punto_de_mirada") },
                { relleno: PDF_CORAL, borde: PDF_BLANCO, circulo: true, texto: this.T("visualizaciones_centroide") },
                { relleno: [252, 244, 230], borde: PDF_DORADO, punteado: true, circulo: true, texto: this.T("visualizaciones_elipse_de_dispersion") }
            ];
            f.datos = a && a.validos.length >= 3 ? this.datosDispersion(a.validos) : sinMirada;
        }
        else if (tipo === "atencion") {
            f.titulo = this.T("visualizaciones_atencion_frente_a_distraccion");
            f.descripcion = this.T("informe_desc_atencion");
            f.leyenda = [
                { relleno: PDF_DORADO, texto: this.T("analisismultiple_atencion_activa") },
                { relleno: PDF_GRIS, texto: this.T("visualizaciones_movimiento_sacadico") },
                { relleno: [229, 231, 236], borde: PDF_GRIS, texto: this.T("vis_atencion_sin_datos") }
            ];
            f.datos = sinMirada;

            if (a) {
                var segundosValidos = a.validos.length * INTERVALO_MS / 1000;
                var segundosSinDatos = (a.pts.length - a.validos.length) * INTERVALO_MS / 1000;

                f.datos = [
                    this.formato("informe_dato_atencion", this.porcentaje(a.atencion), this.numero(segundosValidos * a.atencion / 100)),
                    this.formato("informe_dato_sacadico", this.porcentaje(a.sacadico), this.numero(segundosValidos * a.sacadico / 100)),
                    this.formato("informe_dato_sin_datos", this.numero(segundosSinDatos))
                ];
            }
        }
        else if (tipo === "recorrido") {
            f.titulo = this.T("analisismultiple_recorrido_de_mirada");
            f.descripcion = this.T("informe_desc_recorrido");
            f.leyenda = [
                { relleno: PDF_GRIS, circulo: true, texto: this.T("default_inicio") },
                { relleno: PDF_CORAL, borde: PDF_BLANCO, circulo: true, texto: this.T("visualizaciones_final") }
            ];
            f.datos = a ? [this.formato("informe_dato_recorrido", this.numero(a.validos.length)),
                           this.formato("informe_dato_fijaciones", this.numero(a.fijs.length))] : sinMirada;
        }

        f.canvas = tipo === "atencion"
            ? InformeImagenes.atencion(a, duracionSeg)
            : InformeImagenes.visualizacion(tipo, o.tipos, a, o.segundo);

        return f;
    }

    datosDispersion(validos) {
        var c = covarianza(validos);
        var mitad = (c.sxx - c.syy) / 2;
        var radio = Math.sqrt(mitad * mitad + c.sxy * c.sxy);
        var largo = (c.sxx + c.syy) / 2 + radio;
        var ancho = Math.max((c.sxx + c.syy) / 2 - radio, 0);
        var area = Math.min(1, Math.PI * DISPERSION_K * DISPERSION_K * Math.sqrt(largo * ancho));

        return [
            this.formato("informe_dato_centroide", this.porcentaje(c.cx * 100), this.porcentaje(c.cy * 100)),
            this.formato("informe_dato_elipse", this.porcentaje(area * 100))
        ];
    }

    definiciones() {
        var claves = [
            ["visualizaciones_duracion_total", "vis_ayuda_duracion"],
            ["analisismultiple_fijacion_media", "vis_ayuda_fijacion_media"],
            ["visualizaciones_fijaciones", "vis_ayuda_fijaciones"],
            ["analisismultiple_atencion_activa", "vis_ayuda_atencion"],
            ["visualizaciones_movimiento_sacadico", "vis_ayuda_sacadico"]
        ];
        var lista = [];

        for (var i = 0; i < claves.length; i++) lista.push({ nombre: this.T(claves[i][0]), texto: this.T(claves[i][1]) });

        this.tarjetaDefiniciones(lista);
    }

    tarjetaDefiniciones(lista) {
        var ancho = PDF_CONTENIDO - PDF_RELLENO * 2;
        var partes = [];
        var alto = PDF_RELLENO * 2 - 2.5;

        for (var i = 0; i < lista.length; i++) {
            this.fuente("negrita", 8.5, PDF_AZUL);
            var nombre = this.cortar(lista[i].nombre, ancho);
            this.fuente("texto", 8.5, PDF_GRIS);
            var texto = this.cortar(lista[i].texto, ancho);

            partes.push({ nombre: nombre, texto: texto });
            alto += this.altoLineas(nombre.length + texto.length, 8.5) + 2.5;
        }

        this.seccion(this.T("informe_seccion_definiciones"), alto);
        this.tarjeta(PDF_MARGEN, this.y, PDF_CONTENIDO, alto, PDF_SUPERFICIE);

        var y = this.y + PDF_RELLENO;

        for (i = 0; i < partes.length; i++) {
            this.fuente("negrita", 8.5, PDF_AZUL);
            this.escribir(partes[i].nombre, PDF_MARGEN + PDF_RELLENO, y);
            y += this.altoLineas(partes[i].nombre.length, 8.5);
            this.fuente("texto", 8.5, PDF_GRIS);
            this.escribir(partes[i].texto, PDF_MARGEN + PDF_RELLENO, y);
            y += this.altoLineas(partes[i].texto.length, 8.5) + 2.5;
        }

        this.y += alto + 8;
    }

    pies() {
        var doc = this.doc;
        var total = doc.getNumberOfPages();
        var izquierda = [this.datos.empresa, this.datos.sesion].filter(function (t) { return t; }).join(" · ");

        for (var i = 1; i <= total; i++) {
            doc.setPage(i);
            doc.setDrawColor(PDF_BORDE[0], PDF_BORDE[1], PDF_BORDE[2]);
            doc.setLineWidth(0.3);
            doc.line(PDF_MARGEN, PDF_ALTO - 13, PDF_ANCHO - PDF_MARGEN, PDF_ALTO - 13);

            this.fuente("texto", 7, PDF_TENUE);
            this.escribir(this.cortar(izquierda, PDF_CONTENIDO - 35)[0] || "", PDF_MARGEN, PDF_ALTO - 10.5);
            this.escribir(this.formato("informe_pagina", i, total), PDF_ANCHO - PDF_MARGEN, PDF_ALTO - 10.5, { align: "right" });
        }
    }

    armar(o) {
        var d = this.datos;
        var a = o.analisis;

        this.doc.setProperties({
            title: this.T("informe_titulo") + " - " + d.sesion,
            subject: d.sesion,
            author: d.empresa || "",
            creator: "Falke"
        });

        this.portada();

        this.tablaDatos(this.T("informe_seccion_empresa"), [
            [this.T("informe_empresa"), d.empresa],
            [this.T("informe_cuit"), d.cuit],
            [this.T("informe_rubro"), d.rubro]
        ]);

        this.tablaDatos(this.T("informe_seccion_sesion"), [
            [this.T("informe_sesion"), d.sesion],
            [this.T("informe_categoria"), d.categoria],
            [this.T("informe_tipo_activo"), d.tipoActivo],
            [this.T("informe_fecha_grabacion"), d.fechaGrabacion],
            [this.T("informe_duracion"), d.duracion],
            [this.T("informe_analista"), d.analista],
            [this.T("informe_muestras"), d.muestras],
            [this.T("informe_dispositivo"), o.incluir.dispositivo ? d.dispositivo : null]
        ]);

        if (o.incluir.activo) {
            var filas = [[this.T("informe_activo"), d.activo], [this.T("informe_flujo"), d.flujo]];

            (d.camposActivo || []).forEach(function (c) { filas.push([c.etiqueta, c.valor]); });

            this.tablaDatos(this.T("informe_seccion_activo"), filas);
        }

        if (o.incluir.perfil) {
            this.tablaDatos(this.T("informe_seccion_perfil"), [
                [this.T("informe_edad"), d.edad],
                [this.T("informe_genero"), d.genero],
                [this.T("informe_familiaridad"), d.familiaridad]
            ]);
        }

        if (o.incluir.objetivo) this.parrafo(this.T("informe_seccion_objetivo"), d.objetivo);

        this.metricas([
            { nombre: this.T("visualizaciones_duracion_total"), valor: d.duracion },
            { nombre: this.T("analisismultiple_fijacion_media"), valor: a ? this.numero(a.fijMedia) + " s" : "-" },
            { nombre: this.T("visualizaciones_fijaciones"), valor: a ? this.numero(a.fijs.length) : "-" },
            { nombre: this.T("analisismultiple_atencion_activa"), valor: a ? a.atencion + "%" : "-" },
            { nombre: this.T("visualizaciones_movimiento_sacadico"), valor: a ? a.sacadico + "%" : "-" }
        ]);

        var elegidas = PDF_VISUALIZACIONES.filter(function (t) { return o.visuales.indexOf(t) !== -1; });

        if (elegidas.length > 0) {
            var primera = this.armarFigura(elegidas[0], o);

            this.seccion(this.T("informe_seccion_visualizaciones"), 70);
            this.figura(primera);

            for (var i = 1; i < elegidas.length; i++) this.figura(this.armarFigura(elegidas[i], o));

            this.y += 3;
        }

        if (o.incluir.definiciones) this.definiciones();

        this.pies();
    }

    nombreArchivo() {
        var nombre = this.T("informe_titulo") + " - " + (this.datos.sesion || "");

        return nombre.replace(/[\\\/:*?"<>|]+/g, "-").trim() + ".pdf";
    }

    guardar() {
        this.doc.save(this.nombreArchivo());
    }
}
