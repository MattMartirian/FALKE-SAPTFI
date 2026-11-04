(function (global) {
    var INTERVALO_REINTENTO_MS = 2000;

    function ClienteMirada(opciones) {
        opciones = opciones || {};

        this.url = opciones.url || "ws://localhost:8887/mirada";
        this.estado = "desconectado";
        this.conectado = false;
        this.socket = null;
        this.cerrado = false;
        this.escuchas = { mirada: [], estado: [], conexion: [] };
    }

    ClienteMirada.prototype.on = function (evento, funcion) {
        this.escuchas[evento].push(funcion);

        return this;
    };

    ClienteMirada.prototype.emitir = function (evento, dato) {
        this.escuchas[evento].forEach(function (funcion) {
            funcion(dato);
        });
    };

    ClienteMirada.prototype.conectar = function () {
        var cliente = this;

        cliente.cerrado = false;

        try {
            cliente.socket = new WebSocket(cliente.url);
        } catch (error) {
            cliente.reintentar();
            return;
        }

        cliente.socket.onopen = function () {
            cliente.conectado = true;
            cliente.emitir("conexion", true);
        };

        cliente.socket.onmessage = function (evento) {
            var mensaje;

            try {
                mensaje = JSON.parse(evento.data);
            } catch (error) {
                return;
            }

            if (mensaje.tipo === "mirada") {
                cliente.emitir("mirada", mensaje);
            } else if (mensaje.tipo === "estado") {
                cliente.estado = mensaje.estado;
                cliente.emitir("estado", mensaje.estado);
            }
        };

        cliente.socket.onerror = function () {
            cliente.socket.close();
        };

        cliente.socket.onclose = function () {
            var estabaConectado = cliente.conectado;

            cliente.conectado = false;
            cliente.estado = "desconectado";

            if (estabaConectado) cliente.emitir("conexion", false);

            if (!cliente.cerrado) cliente.reintentar();
        };
    };

    ClienteMirada.prototype.reintentar = function () {
        var cliente = this;

        setTimeout(function () {
            if (!cliente.cerrado) cliente.conectar();
        }, INTERVALO_REINTENTO_MS);
    };

    ClienteMirada.prototype.desconectar = function () {
        this.cerrado = true;

        if (this.socket) this.socket.close();
    };

    ClienteMirada.aViewport = function (x, y) {
        var borde = (window.outerWidth - window.innerWidth) / 2;
        var barraSuperior = window.outerHeight - window.innerHeight - borde;

        return {
            x: x * screen.width - window.screenX - borde,
            y: y * screen.height - window.screenY - barraSuperior
        };
    };

    global.ClienteMirada = ClienteMirada;
})(window);
