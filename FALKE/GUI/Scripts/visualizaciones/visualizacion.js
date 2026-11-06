"use strict";


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
        this.elTitulo = document.getElementById("visorTitulo");
        this.lienzo = document.getElementById("visorLienzo");
        this.elLeyenda = document.getElementById("visorLeyenda");
        this.controles = document.getElementById("visorControles");
        this.linea = document.getElementById("visorLinea");
        this.elTiempo = document.getElementById("visorTiempo");
        this.btnReproducir = document.getElementById("btnReproducir");

        this.titulo = "";       
        this.leyenda = "";      
        this.sesion = null;     
        this.abierta = false;
        this.video = null;
        this.canvas = null;
        this.cuadro = 0;        
    }

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
        this.actualizar();

        this.duracionFija = false;

        // bajamos el video entero y lo usamos como archivo local,
        // asi firefox puede saltar a cualquier segundo
        var video = this.video;

        fetch(sesion.urlVideo)
            .then((resp) => resp.blob())
            .then((blob) => {
                if (this.video === video) video.src = URL.createObjectURL(blob);
            })
            .catch(() => { video.src = sesion.urlVideo; });
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

    crearLienzo() {
        var video = document.createElement("video");
        var canvas = document.createElement("canvas");
        var error = document.createElement("p");

        
        video.playsInline = true;
        video.preload = "auto";
        video.disablePictureInPicture = true;
        video.setAttribute("controlslist", "nodownload noplaybackrate");
        video.addEventListener("contextmenu", (e) => e.preventDefault());

       
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

        canvas.addEventListener("click", () => this.alternar());
    }

    alCargarVideo() {
        var v = this.video;

        if (v.videoWidth && v.videoHeight) {
            this.lienzo.style.aspectRatio = v.videoWidth + " / " + v.videoHeight;
            this.canvas.height = Math.round(this.canvas.width * v.videoHeight / v.videoWidth);
            this.alCambiarTamano();
        }

        this.arreglarDuracion();
        this.actualizar();
    }

    arreglarDuracion() {
        var v = this.video;

        if (v.duration !== Infinity) {
            this.duracionFija = true;
            return;
        }

        var volver = () => {
            v.removeEventListener("timeupdate", volver);
            v.currentTime = 0;
            this.duracionFija = true;
        };

        v.addEventListener("timeupdate", volver);
        v.currentTime = 1e101;
    }

    // dibuja el punto de mirada del instante t (ms), con su estela de 10 muestras
    dibujarMirada(ctx, w, h, pts, t) {
        var idx = buscarMuestra(pts, t);

        // fallback si no hay muestra
        if (idx < 0) return;

        // las 10 muestras anteriores
        for (var i = Math.max(0, idx - 10); i < idx; i++) {
            if (!pts[i].ok) continue;

            ctx.beginPath();
            // arc(centro x * width, centro y * height, radio, angulo inicial, angulo final)
            ctx.arc(pts[i].x * w, pts[i].y * h, 5, 0, Math.PI * 2);

            // control de opacidad, cuanto mas viejo menos se ve
            ctx.fillStyle = "rgba(242,97,87," + (0.3 * (i - (idx - 10)) / 10) + ")";
            ctx.fill();
        }

        var p = pts[idx];
        if (!p.ok || t - p.t > 400) return;

        // vista actual es mas grande, con borde blanco
        ctx.beginPath();
        ctx.arc(p.x * w, p.y * h, 13, 0, Math.PI * 2);
        ctx.fillStyle = "rgba(242,97,87,.55)";
        ctx.fill();
        ctx.lineWidth = 2;
        ctx.strokeStyle = "#fff";
        ctx.stroke();
    }

    alAbrir() { }
    alCerrar() { }
    alCambiarTamano() { }
    dibujar() { }

    duracion() {
        var real = this.video.duration;

        // ya con la duracion real del video, usamos esa (la de la sesion puede ser mas larga)
        if (this.duracionFija && isFinite(real) && real > 0) return real;

        return Math.max(this.sesion.duracionSeg, isFinite(real) ? real : 0);
    }

    actualizar() {
        if (!this.video) return;

        var dur = this.duracion();

        this.elTiempo.textContent = aMinutos(this.video.currentTime) + " / " + aMinutos(dur);

        if (dur > 0) this.linea.value = Math.min(1000, Math.round(this.video.currentTime * 1000 / dur));

        this.dibujar();
    }

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
