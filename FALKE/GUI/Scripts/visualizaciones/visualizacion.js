"use strict";

// VISUALIZACION (clase base)
// Es todo lo que tienen en comun las ventanas del visor: el video, el canvas encima, los controles y el bucle de reproduccion.
// Cada visualizacion (grabacion, calor, zonas...) hereda de esta con "extends" y solo define lo suyo:
//   - this.titulo y this.leyenda
//   - dibujar(): que se dibuja encima del video
//   - alAbrir() / alCerrar() / alCambiarTamano(): ganchos opcionales para hacer algo extra
// Nota: en los eventos uso funciones flecha (() => ...) porque asi "this" sigue siendo la visualizacion.

// pasa segundos a "mm:ss" (o "h:mm:ss")
function aMinutos(segundos) {
    segundos = Math.max(0, segundos || 0);

    var h = Math.floor(segundos / 3600);
    var m = Math.floor((segundos % 3600) / 60);
    var s = Math.floor(segundos % 60);
    var mm = (m < 10 ? "0" : "") + m;
    var ss = (s < 10 ? "0" : "") + s;

    return h > 0 ? h + ":" + mm + ":" + ss : mm + ":" + ss;
}

class Visualizacion {
    constructor() {
        // las partes del modal son siempre las mismas para todas las visualizaciones
        this.elTitulo = document.getElementById("visorTitulo");
        this.lienzo = document.getElementById("visorLienzo");
        this.elLeyenda = document.getElementById("visorLeyenda");
        this.controles = document.getElementById("visorControles");
        this.linea = document.getElementById("visorLinea");
        this.elTiempo = document.getElementById("visorTiempo");
        this.btnReproducir = document.getElementById("btnReproducir");

        this.titulo = "";       // lo define cada visualizacion
        this.leyenda = "";      // html de la leyenda, lo define cada visualizacion
        this.sesion = null;     // la sesion que se esta viendo (ahi estan el video y el analisis)
        this.abierta = false;
        this.video = null;
        this.canvas = null;
        this.cuadro = 0;        // id del requestAnimationFrame, para poder cancelarlo
    }

    // ------------------------------------------------------------------ abrir y cerrar

    abrir(sesion) {
        this.sesion = sesion;
        this.abierta = true;

        this.elTitulo.textContent = this.titulo;
        this.elLeyenda.innerHTML = this.leyenda;
        this.crearLienzo();
        this.controles.hidden = false;
        this.ponerIcono(false);
        this.linea.value = 0;
        this.elTiempo.textContent = "00:00 / " + aMinutos(sesion.duracionSeg);

        this.alAbrir();

        this.video.src = sesion.urlVideo;
    }

    cerrar() {
        if (!this.abierta) return;

        this.abierta = false;
        cancelAnimationFrame(this.cuadro);

        if (this.video) {
            this.video.pause();
            this.video.removeAttribute("src");
            this.video.load();
        }

        this.lienzo.innerHTML = "";
        this.lienzo.style.aspectRatio = "";
        this.video = null;
        this.canvas = null;

        this.alCerrar();
    }

    // arma el <video>, el <canvas> que va encima (donde se dibuja) y el cartel de error
    crearLienzo() {
        var video = document.createElement("video");
        var canvas = document.createElement("canvas");
        var error = document.createElement("p");

        // sin controles del navegador (los nuestros estan abajo), sin audio, sin descarga ni picture-in-picture
        video.muted = true;
        video.playsInline = true;
        video.preload = "auto";
        video.disablePictureInPicture = true;
        video.setAttribute("controlslist", "nodownload noplaybackrate");
        video.addEventListener("contextmenu", (e) => e.preventDefault());

        // el canvas tiene 960 de ancho; el alto sale de la proporcion del video cuando carga
        canvas.width = 960;
        canvas.height = 540;

        error.className = "sin-margen";
        error.hidden = true;
        error.style.cssText = "position:absolute;inset:0;display:flex;align-items:center;justify-content:center;padding:16px;text-align:center;color:#fff;background:rgba(14,26,52,.8)";
        error.textContent = VisTextos.errorCargar;

        this.lienzo.appendChild(video);
        this.lienzo.appendChild(canvas);
        this.lienzo.appendChild(error);

        this.video = video;
        this.canvas = canvas;

        video.addEventListener("loadedmetadata", () => this.alCargarVideo());
        video.addEventListener("timeupdate", () => this.actualizar());
        video.addEventListener("seeked", () => this.actualizar());
        video.addEventListener("play", () => { this.ponerIcono(true); this.cuadro = requestAnimationFrame(() => this.bucle()); });
        video.addEventListener("pause", () => this.ponerIcono(false));
        video.addEventListener("ended", () => this.ponerIcono(false));
        video.addEventListener("error", () => { error.hidden = false; });

        // tocar el video lo pausa / lo reproduce
        canvas.addEventListener("click", () => this.alternar());
    }

    // cuando el video informa su tamano, el lienzo toma su proporcion real (asi lo que va de 0 a 1 cae donde corresponde)
    alCargarVideo() {
        var v = this.video;

        if (v.videoWidth && v.videoHeight) {
            this.lienzo.style.aspectRatio = v.videoWidth + " / " + v.videoHeight;
            this.canvas.height = Math.round(this.canvas.width * v.videoHeight / v.videoWidth);
            this.alCambiarTamano();
        }

        this.arreglarDuracion();
    }

    // un webm grabado en el navegador dice que dura "Infinity": saltando al final y volviendo se calcula la duracion real
    arreglarDuracion() {
        var v = this.video;

        if (v.duration !== Infinity) return;

        var volver = () => {
            v.removeEventListener("timeupdate", volver);
            v.currentTime = 0;
        };

        v.addEventListener("timeupdate", volver);
        v.currentTime = 1e101;
    }

    // ------------------------------------------------------------------ ganchos (cada visualizacion redefine los que necesite)

    alAbrir() { }
    alCerrar() { }
    alCambiarTamano() { }
    dibujar() { }

    // ------------------------------------------------------------------ reproduccion

    duracion() {
        var v = this.video;

        return isFinite(v.duration) && v.duration > 0 ? v.duration : this.sesion.duracionSeg;
    }

    // se llama en cada cambio del video: actualiza el tiempo, la linea y vuelve a dibujar
    actualizar() {
        if (!this.video) return;

        var dur = this.duracion();

        this.elTiempo.textContent = aMinutos(this.video.currentTime) + " / " + aMinutos(dur);

        if (dur > 0) this.linea.value = Math.min(1000, Math.round(this.video.currentTime * 1000 / dur));

        this.dibujar();
    }

    // mientras el video corre, se redibuja en cada cuadro de pantalla
    bucle() {
        this.actualizar();

        if (this.video && !this.video.paused && !this.video.ended) {
            this.cuadro = requestAnimationFrame(() => this.bucle());
        }
    }

    alternar() {
        if (!this.video) return;

        if (this.video.paused) this.video.play(); else this.video.pause();
    }

    saltar(segundos) {
        if (!this.video) return;

        this.video.currentTime = Math.max(0, Math.min(this.duracion() || 0, this.video.currentTime + segundos));
    }

    // la linea de tiempo va de 0 a 1000: la paso a segundos del video
    irA(valorLinea) {
        if (!this.video) return;

        this.video.currentTime = this.duracion() * valorLinea / 1000;
    }

    ponerIcono(reproduciendo) {
        this.btnReproducir.querySelector("[data-icono-play]").style.display = reproduciendo ? "none" : "";
        this.btnReproducir.querySelector("[data-icono-pausa]").style.display = reproduciendo ? "" : "none";
        this.btnReproducir.setAttribute("aria-label", reproduciendo ? VisTextos.pausar : VisTextos.reproducir);
    }
}
