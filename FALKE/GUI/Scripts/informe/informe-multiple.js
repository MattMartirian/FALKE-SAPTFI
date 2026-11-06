"use strict";

var MULTIPLE_VISUALIZACIONES = ["calor", "zonas", "dispersion", "atencion", "recorrido", "porsesion"];
var MULTIPLE_MAX_LINEAS_SESIONES = 30;
var MULTIPLE_TABLA_ENTERA = 90;

class InformeMultiplePdf extends InformePdf {
    nombreMetrica(i) {
        var claves = ["analisismultiple_duracion_promedio", "analisismultiple_fijacion_media", "analisismultiple_fijaciones_promedio", "analisismultiple_atencion_activa"];

        return this.T(claves[i]);
    }

    formatoMetrica(i, valor) {
        if (i === 0) return formatoMinutos(valor);
        if (i === 1) return this.numero(valor) + " s";
        if (i === 2) return this.numero(valor);

        return Math.round(valor) + "%";
    }

    textoGrupo(campo, valor) {
        var lista = this.datos.grupos[campo] || [];

        for (var i = 0; i < lista.length; i++) {
            if (lista[i].valor === valor) return lista[i].texto;
        }

        return valor || "-";
    }

    opcionesMarcadas(campo, marcados) {
        var lista = this.datos.grupos[campo] || [];

        if (marcados[campo].length === 0) return lista;

        return lista.filter(function (o) { return marcados[campo].indexOf(o.valor) >= 0; });
    }

    fecha(iso) {
        var f = new Date(iso + "T00:00:00");

        if (!iso || isNaN(f.getTime())) return "-";

        try {
            return new Intl.DateTimeFormat(this.datos.idioma || undefined, { dateStyle: "medium" }).format(f);
        } catch (e) {
            return iso;
        }
    }

    cabeceraTabla(cabeceras, anchos, alto, relleno) {
        var doc = this.doc;
        var x = PDF_MARGEN;

        doc.setFillColor(PDF_SUPERFICIE[0], PDF_SUPERFICIE[1], PDF_SUPERFICIE[2]);
        doc.rect(PDF_MARGEN, this.y, PDF_CONTENIDO, alto, "F");

        this.fuente("semi", 7.5, PDF_TENUE);

        for (var i = 0; i < cabeceras.length; i++) {
            this.escribir(cabeceras[i], x + relleno, this.y + relleno);
            x += anchos[i];
        }

        this.y += alto;
    }

    cerrarTabla(inicio) {
        this.doc.setDrawColor(PDF_BORDE[0], PDF_BORDE[1], PDF_BORDE[2]);
        this.doc.setLineWidth(0.3);
        this.doc.roundedRect(PDF_MARGEN, inicio, PDF_CONTENIDO, this.y - inicio, 2, 2, "S");
    }

    tabla(titulo, columnas, filas, nota) {
        var relleno = 2.2;
        var total = 0;
        var anchos = [];
        var cabeceras = [];
        var medidas = [];
        var i, j;

        for (i = 0; i < columnas.length; i++) total += columnas[i].ancho;
        for (i = 0; i < columnas.length; i++) anchos.push(PDF_CONTENIDO * columnas[i].ancho / total);

        var lineasCabecera = 1;

        this.fuente("semi", 7.5, PDF_TENUE);

        for (i = 0; i < columnas.length; i++) {
            cabeceras.push(this.cortar(columnas[i].texto, anchos[i] - relleno * 2));
            lineasCabecera = Math.max(lineasCabecera, cabeceras[i].length);
        }

        var altoCabecera = this.altoLineas(lineasCabecera, 7.5) + relleno * 2;

        for (i = 0; i < filas.length; i++) {
            var celdas = [];
            var lineas = 1;

            for (j = 0; j < filas[i].length; j++) {
                this.fuente(j === 0 ? "semi" : "texto", 8.5, PDF_AZUL);
                celdas.push(this.cortar(filas[i][j], anchos[j] - relleno * 2));
                lineas = Math.max(lineas, celdas[j].length);
            }

            medidas.push({ celdas: celdas, alto: this.altoLineas(lineas, 8.5) + relleno * 2 });
        }

        var altoTotal = altoCabecera;

        for (i = 0; i < medidas.length; i++) altoTotal += medidas[i].alto;

        this.seccion(titulo, altoTotal <= MULTIPLE_TABLA_ENTERA ? altoTotal : altoCabecera + (medidas.length > 0 ? medidas[0].alto : 0));

        var inicio = this.y;

        this.cabeceraTabla(cabeceras, anchos, altoCabecera, relleno);

        for (i = 0; i < medidas.length; i++) {
            if (this.y + medidas[i].alto > PDF_LIMITE) {
                this.cerrarTabla(inicio);
                this.nuevaPagina();
                inicio = this.y;
                this.cabeceraTabla(cabeceras, anchos, altoCabecera, relleno);
            }

            this.doc.setDrawColor(PDF_BORDE[0], PDF_BORDE[1], PDF_BORDE[2]);
            this.doc.setLineWidth(0.2);
            this.doc.line(PDF_MARGEN, this.y, PDF_MARGEN + PDF_CONTENIDO, this.y);

            var x = PDF_MARGEN;

            for (j = 0; j < medidas[i].celdas.length; j++) {
                this.fuente(j === 0 ? "semi" : "texto", 8.5, PDF_AZUL);
                this.escribir(medidas[i].celdas[j], x + relleno, this.y + relleno);
                x += anchos[j];
            }

            this.y += medidas[i].alto;
        }

        this.cerrarTabla(inicio);

        if (nota) {
            this.fuente("texto", 7, PDF_TENUE);

            var lineasNota = this.cortar(nota, PDF_CONTENIDO);

            this.espacio(this.altoLineas(lineasNota.length, 7) + 2);
            this.escribir(lineasNota, PDF_MARGEN, this.y + 2);
            this.y += this.altoLineas(lineasNota.length, 7) + 2;
        }

        this.y += 8;
    }

    avisoCalidad(titulo, lineas) {
        var ancho = PDF_CONTENIDO - PDF_RELLENO * 2;

        this.fuente("negrita", 8.5, PDF_AZUL);
        var cabecera = this.cortar(titulo, ancho);
        this.fuente("texto", 8.5, PDF_AZUL);

        var cuerpo = [];

        for (var i = 0; i < lineas.length; i++) cuerpo = cuerpo.concat(this.cortar(lineas[i], ancho));

        var alto = PDF_RELLENO * 2 + this.altoLineas(cabecera.length + cuerpo.length, 8.5);

        this.espacio(alto);
        this.tarjeta(PDF_MARGEN, this.y, PDF_CONTENIDO, alto, PDF_ACENTO_SUAVE);
        this.fuente("negrita", 8.5, PDF_AZUL);
        this.escribir(cabecera, PDF_MARGEN + PDF_RELLENO, this.y + PDF_RELLENO);
        this.fuente("texto", 8.5, PDF_AZUL);
        this.escribir(cuerpo, PDF_MARGEN + PDF_RELLENO, this.y + PDF_RELLENO + this.altoLineas(cabecera.length, 8.5));

        this.y += alto + 8;
    }

    filtro(o) {
        var self = this;
        var campos = [
            ["edad", "grabacion_tester_edad"],
            ["genero", "grabacion_tester_genero"],
            ["familiaridad", "grabacion_tester_familiaridad"]
        ];
        var filas = [];

        campos.forEach(function (c) {
            var lista = self.opcionesMarcadas(c[0], o.marcados);
            var todas = (self.datos.grupos[c[0]] || []).length;
            var texto = lista.length === todas ? self.T("informe_multiple_todos") : lista.map(function (g) { return g.texto; }).join(", ");

            filas.push([self.T(c[1]), texto]);
        });

        this.tablaDatos(this.T("analisismultiple_filtro_titulo"), filas);
    }

    sesiones(o) {
        var self = this;
        var columnas = [
            { texto: this.T("informe_multiple_col_numero"), ancho: 7 },
            { texto: this.T("informe_multiple_col_sesion"), ancho: 36 },
            { texto: this.T("informe_multiple_col_fecha"), ancho: 19 },
            { texto: this.T("informe_multiple_col_duracion"), ancho: 15 },
            { texto: this.T("grabacion_tester_edad"), ancho: 17 },
            { texto: this.T("grabacion_tester_genero"), ancho: 19 },
            { texto: this.T("grabacion_tester_familiaridad"), ancho: 20 },
            { texto: this.T("analisismultiple_atencion_activa"), ancho: 16 }
        ];
        var filas = o.elegidas.map(function (s, i) {
            return [
                String(i + 1),
                s.nombre,
                self.fecha(s.fecha),
                formatoMinutos(s.duracionSeg),
                s.edad || "-",
                self.textoGrupo("genero", s.genero),
                self.textoGrupo("familiaridad", s.familiaridad),
                s.conMirada ? Math.round(s.analisis.atencion) + "%" : self.T("analisismultiple_sin_datos")
            ];
        });

        this.tabla(this.T("informe_multiple_seccion_sesiones"), columnas, filas);
    }

    calidad(o) {
        var c = o.calidad;

        this.metricas([
            { nombre: this.T("analisismultiple_calidad_usadas"), valor: c.usadas + " / " + c.total },
            { nombre: this.T("analisismultiple_calidad_excluidas"), valor: String(c.excluidas) },
            { nombre: this.T("analisismultiple_calidad_variacion"), valor: Math.round(c.cv * 100) + "%" }
        ], this.T("analisismultiple_calidad_titulo"));

        var avisos = [];

        if (c.pocas) avisos.push(this.formato("analisismultiple_calidad_pocas", N_MIN_SESIONES));
        if (c.variada) avisos.push(this.T("analisismultiple_calidad_duraciones"));

        if (avisos.length > 0) {
            this.y -= 4;
            this.avisoCalidad(this.T("analisismultiple_calidad_aviso"), avisos);
        }
    }

    promedios(o) {
        var lista = [];

        for (var i = 0; i < METRICAS.length; i++) {
            var valores = valoresDe(o.elegidas, METRICAS[i]);

            lista.push({
                nombre: this.nombreMetrica(i),
                valor: valores.length ? this.formatoMetrica(i, media(valores)) : this.T("analisismultiple_sin_datos"),
                detalle: valores.length ? this.formato("analisismultiple_mediana_n", this.formatoMetrica(i, mediana(valores))) : null
            });
        }

        lista.push({ nombre: this.T("analisismultiple_sesiones_combinadas"), valor: String(o.elegidas.length) });

        this.metricas(lista, this.T("analisismultiple_metricas_promediadas"));
    }

    porGrupo(o, campo, claveTitulo, claveColumna) {
        var self = this;
        var columnas = [{ texto: this.T(claveColumna), ancho: 30 }, { texto: this.T("analisismultiple_sesiones_combinadas"), ancho: 20 }];
        var hayPequena = false;

        for (var i = 0; i < METRICAS.length; i++) columnas.push({ texto: this.nombreMetrica(i), ancho: 24 });

        var filas = this.opcionesMarcadas(campo, o.marcados).map(function (opcion) {
            var delGrupo = o.elegidas.filter(function (s) { return s[campo] === opcion.valor; });
            var fila = [opcion.texto, String(delGrupo.length)];

            for (var k = 0; k < METRICAS.length; k++) {
                var valores = valoresDe(delGrupo, METRICAS[k]);

                if (valores.length === 0) {
                    fila.push(self.T("analisismultiple_sin_datos"));
                    continue;
                }

                var pequena = valores.length / valoresDe(o.elegidas, METRICAS[k]).length < MUESTRA_PEQUENA;

                if (pequena) hayPequena = true;

                fila.push(self.formatoMetrica(k, media(valores)) + (pequena ? " *" : ""));
            }

            return fila;
        });

        this.tabla(this.T(claveTitulo), columnas, filas, hayPequena ? "* " + this.T("analisismultiple_muestra_pequena") : null);
    }

    definiciones(o) {
        var lista = [];
        var claves = [
            ["analisismultiple_duracion_promedio", "vis_ayuda_duracion"],
            ["analisismultiple_fijacion_media", "vis_ayuda_fijacion_media"],
            ["analisismultiple_fijaciones_promedio", "vis_ayuda_fijaciones"],
            ["analisismultiple_atencion_activa", "vis_ayuda_atencion"]
        ];

        for (var i = 0; i < claves.length; i++) lista.push({ nombre: this.T(claves[i][0]), texto: this.T(claves[i][1]) });

        lista.push({ nombre: this.T("informe_multiple_def_titulo_mediana"), texto: this.T("informe_multiple_def_mediana") });
        lista.push({ nombre: this.T("informe_multiple_def_titulo_muestra"), texto: this.formato("informe_multiple_def_muestra", this.porcentaje(MUESTRA_PEQUENA * 100)) });

        this.tarjetaDefiniciones(lista);
    }

    sobreFondo(origen) {
        var canvas = document.createElement("canvas");
        var ctx = canvas.getContext("2d");

        canvas.width = origen.width;
        canvas.height = origen.height;

        ctx.fillStyle = "#FAF9F4";
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(origen, 0, 0);

        return canvas;
    }

    leyendaAtencion(clave, pct) {
        var texto = this.T(clave);

        return pct === null ? texto.replace(/\s*\{0\}\s*%?/, "") : texto.replace("{0}", pct);
    }

    armarFigura(tipo, o) {
        var n = o.elegidas.length;
        var f = {
            leyenda: [],
            datos: [this.formato("informe_multiple_dato_con_mirada", o.conMirada.length, n)],
            pie: this.T("analisismultiple_sesiones_combinadas") + ": " + n
        };
        var pct = o.conMirada.length > 0 ? o.porcentajeAtencion : null;
        var atencion = [
            { relleno: PDF_DORADO, texto: this.leyendaAtencion("vis_fijacion_activa_n", pct) },
            { relleno: PDF_GRIS, texto: this.leyendaAtencion("vis_sacadico_n", pct === null ? null : 100 - pct) }
        ];

        if (tipo === "calor") {
            f.titulo = this.T("analisismultiple_mapa_de_calor");
            f.descripcion = this.T("analisismultiple_densidad_de_fijaciones_promediada_entre");
            f.leyenda = [
                { relleno: PDF_CORAL, texto: this.T("visualizaciones_mayor_concentracion") },
                { relleno: PDF_DORADO, texto: this.T("visualizaciones_concentracion_media") },
                { relleno: PDF_GRIS, texto: this.T("visualizaciones_menor_concentracion") }
            ];
        }
        else if (tipo === "zonas") {
            f.titulo = this.T("analisismultiple_zonas_de_interes_y_ciegas");
            f.descripcion = this.T("analisismultiple_areas_que_la_mayoria_de_los");
            f.leyenda = [
                { relleno: [250, 236, 212], borde: PDF_DORADO, texto: this.T("visualizaciones_zona_de_interes") },
                { relleno: [239, 240, 243], borde: PDF_GRIS, punteado: true, texto: this.T("visualizaciones_zona_ciega") }
            ];
            f.datos.push(this.formato("informe_multiple_dato_consenso", this.porcentaje(CONSENSO_ZONAS * 100)));
        }
        else if (tipo === "dispersion") {
            f.titulo = this.T("analisismultiple_dispersion_de_mirada");
            f.descripcion = this.T("analisismultiple_cuanto_varia_la_atencion_entre_los");
            f.leyenda = [
                { relleno: PDF_GRIS, circulo: true, texto: this.T("analisismultiple_centro_de_cada_sesion") },
                { relleno: PDF_CORAL, borde: PDF_BLANCO, circulo: true, texto: this.T("visualizaciones_centroide") },
                { relleno: [252, 244, 230], borde: PDF_DORADO, punteado: true, circulo: true, texto: this.T("visualizaciones_elipse_de_dispersion") }
            ];
        }
        else if (tipo === "atencion") {
            f.titulo = this.T("analisismultiple_atencion_vs_distraccion");
            f.descripcion = this.T("analisismultiple_proporcion_media_de_fijacion_activa_fren");
            f.leyenda = atencion;
        }
        else if (tipo === "recorrido") {
            f.titulo = this.T("analisismultiple_recorrido_de_mirada");
            f.descripcion = this.T("analisismultiple_recorrido_superpuesto");
            f.leyenda = [
                { relleno: PDF_GRIS, circulo: true, texto: this.T("default_inicio") },
                { relleno: PDF_CORAL, borde: PDF_BLANCO, circulo: true, texto: this.T("visualizaciones_final") }
            ];
        }
        else {
            f.titulo = this.T("analisismultiple_atencion_por_sesion");
            f.descripcion = this.T("analisismultiple_comparacion_directa_del_porcentaje_de_at");
            f.leyenda = [
                { relleno: PDF_DORADO, texto: this.leyendaAtencion("vis_fijacion_activa_n", null) },
                { relleno: PDF_GRIS, texto: this.leyendaAtencion("vis_sacadico_n", null) }
            ];

            if (o.conMirada.length > 0) {
                f.datos.push(this.T("informe_multiple_dato_barras"));

                for (var i = 0; i < o.conMirada.length && i < MULTIPLE_MAX_LINEAS_SESIONES; i++) {
                    f.datos.push((i + 1) + ". " + o.conMirada[i].nombre + ": " + Math.round(o.conMirada[i].analisis.atencion) + "%");
                }

                if (o.conMirada.length > MULTIPLE_MAX_LINEAS_SESIONES) {
                    f.datos.push(this.formato("informe_multiple_y_mas", o.conMirada.length - MULTIPLE_MAX_LINEAS_SESIONES));
                }
            }
        }

        f.canvas = this.sobreFondo(o.graficos[tipo]);

        return f;
    }

    armar(o) {
        var d = this.datos;
        var inc = o.incluir;

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

        this.tablaDatos(this.T("informe_multiple_seccion_analisis"), [
            [this.T("informe_categoria"), d.categoria],
            [this.T("informe_tipo_activo"), d.tipoActivo],
            [this.T("informe_activo"), d.activo],
            [this.T("informe_multiple_sesiones_elegidas"), String(o.totalCargadas)],
            [this.T("analisismultiple_sesiones_combinadas"), String(o.elegidas.length)],
            [this.T("analisismultiple_calidad_usadas"), String(o.conMirada.length)]
        ]);

        if (inc.filtro) this.filtro(o);
        if (inc.sesiones) this.sesiones(o);
        if (inc.calidad) this.calidad(o);
        if (inc.metricas) this.promedios(o);
        if (inc.grupoEdad) this.porGrupo(o, "edad", "informe_multiple_por_edad", "grabacion_tester_edad");
        if (inc.grupoGenero) this.porGrupo(o, "genero", "informe_multiple_por_genero", "grabacion_tester_genero");
        if (inc.grupoFamiliaridad) this.porGrupo(o, "familiaridad", "informe_multiple_por_familiaridad", "grabacion_tester_familiaridad");

        var elegidas = MULTIPLE_VISUALIZACIONES.filter(function (t) { return o.visuales.indexOf(t) !== -1; });

        if (elegidas.length > 0) {
            var primera = this.armarFigura(elegidas[0], o);

            this.seccion(this.T("informe_seccion_visualizaciones"), 70);
            this.figura(primera);

            for (var i = 1; i < elegidas.length; i++) this.figura(this.armarFigura(elegidas[i], o));

            this.y += 3;
        }

        if (inc.definiciones) this.definiciones(o);

        this.pies();
    }
}
