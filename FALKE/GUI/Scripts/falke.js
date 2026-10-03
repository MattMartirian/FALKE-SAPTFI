(function ()
{
    "use strict";

    var CLAVE_TEMA = "falke.tema";

    function temaGuardado()
    {
        try
        {
            return localStorage.getItem(CLAVE_TEMA);
        }
        catch (e)
        {
            return null;
        }
    }

    function aplicarTema(tema)
    {
        document.documentElement.setAttribute("data-tema", tema);

        var botones = document.querySelectorAll("[data-accion='cambiar-tema']");

        for (var i = 0; i < botones.length; i++)
        {
            botones[i].setAttribute("aria-pressed", tema === "noche" ? "true" : "false");
            botones[i].title = tema === "noche" ? "Cambiar a modo dia" : "Cambiar a modo noche";
        }

        mostrarIconosTema(tema);
    }

    function mostrarIconosTema(tema)
    {
        var soles = document.querySelectorAll("[data-icono-tema='dia']");
        var lunas = document.querySelectorAll("[data-icono-tema='noche']");
        var i;

        for (i = 0; i < soles.length; i++)
        {
            soles[i].style.display = tema === "noche" ? "" : "none";
        }

        for (i = 0; i < lunas.length; i++)
        {
            lunas[i].style.display = tema === "noche" ? "none" : "";
        }
    }

    function iniciarTema()
    {
        var guardado = temaGuardado();
        var prefiereOscuro = window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches;
        var tema = guardado ? guardado : (prefiereOscuro ? "noche" : "dia");

        aplicarTema(tema);
    }

    function alternarTema()
    {
        var actual = document.documentElement.getAttribute("data-tema");
        var nuevo = actual === "noche" ? "dia" : "noche";

        aplicarTema(nuevo);

        try
        {
            localStorage.setItem(CLAVE_TEMA, nuevo);
        }
        catch (e)
        {
        }
    }

    var CLAVE_SIDEBAR = "falke.sidebar";

    function aplicarSidebar(plegada)
    {
        var app = document.querySelector(".app");

        if (!app) return;

        if (plegada)
        {
            app.classList.add("sidebar-plegada");
        }
        else
        {
            app.classList.remove("sidebar-plegada");
        }

        var boton = document.querySelector("[data-accion='plegar-sidebar']");

        if (boton)
        {
            boton.setAttribute("aria-expanded", plegada ? "false" : "true");
            boton.title = plegada ? "Expandir el menu" : "Plegar el menu";
        }
    }

    function iniciarSidebar()
    {
        var guardado = null;

        try
        {
            guardado = localStorage.getItem(CLAVE_SIDEBAR);
        }
        catch (e)
        {
            guardado = null;
        }

        aplicarSidebar(guardado === "plegada");
    }

    function alternarSidebar()
    {
        var app = document.querySelector(".app");

        if (!app) return;

        var plegada = !app.classList.contains("sidebar-plegada");

        aplicarSidebar(plegada);

        try
        {
            localStorage.setItem(CLAVE_SIDEBAR, plegada ? "plegada" : "abierta");
        }
        catch (e)
        {
        }
    }

    function cerrarDesplegables(excepto)
    {
        var abiertos = document.querySelectorAll("[data-desplegable]");

        for (var i = 0; i < abiertos.length; i++)
        {
            if (abiertos[i] === excepto) continue;

            var panel = document.getElementById(abiertos[i].getAttribute("data-desplegable"));

            if (panel) panel.hidden = true;

            abiertos[i].setAttribute("aria-expanded", "false");
        }
    }

    function alternarDesplegable(boton)
    {
        var panel = document.getElementById(boton.getAttribute("data-desplegable"));

        if (!panel) return;

        var abrir = panel.hidden;

        cerrarDesplegables(boton);

        panel.hidden = !abrir;
        boton.setAttribute("aria-expanded", abrir ? "true" : "false");
    }

    var modalAbierto = null;
    var elementoQueAbrio = null;

    function abrirModal(id, boton)
    {
        var modal = document.getElementById(id);

        if (!modal) return;

        modal.hidden = false;
        modalAbierto = modal;
        elementoQueAbrio = boton || null;

        document.body.style.overflow = "hidden";

        var foco = modal.querySelector("[data-foco-inicial]");

        if (!foco) foco = modal.querySelector("input, select, textarea, button");

        if (foco) foco.focus();
    }

    function cerrarModal()
    {
        if (!modalAbierto) return;

        modalAbierto.hidden = true;
        modalAbierto = null;
        document.body.style.overflow = "";

        if (elementoQueAbrio) elementoQueAbrio.focus();

        elementoQueAbrio = null;
    }

    function atraparTab(evento)
    {
        if (!modalAbierto || evento.key !== "Tab") return;

        var foco = modalAbierto.querySelectorAll("a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled])");

        if (foco.length === 0) return;

        var primero = foco[0];
        var ultimo = foco[foco.length - 1];

        if (evento.shiftKey && document.activeElement === primero)
        {
            evento.preventDefault();
            ultimo.focus();
        }
        else if (!evento.shiftKey && document.activeElement === ultimo)
        {
            evento.preventDefault();
            primero.focus();
        }
    }

    function alternarAcordeon(boton)
    {
        var panel = document.getElementById(boton.getAttribute("aria-controls"));

        if (!panel) return;

        var abierto = boton.getAttribute("aria-expanded") === "true";

        boton.setAttribute("aria-expanded", abierto ? "false" : "true");

        if (panel.classList.contains("acordeon-panel")) panel.classList.toggle("abierto", !abierto);
        else panel.hidden = abierto;
    }

    function activarPestana(boton)
    {
        var grupo = boton.closest(".pestanas");

        if (!grupo) return;

        var botones = grupo.querySelectorAll(".pestana");
        var i;

        for (i = 0; i < botones.length; i++)
        {
            var esta = botones[i] === boton;
            var panel = document.getElementById(botones[i].getAttribute("data-pestana"));

            botones[i].setAttribute("aria-selected", esta ? "true" : "false");

            if (panel) panel.hidden = !esta;
        }
    }

    function alternarChip(chip)
    {
        var grupo = chip.closest(".chips");

        if (grupo && grupo.hasAttribute("data-unico"))
        {
            var otros = grupo.querySelectorAll(".chip");

            for (var i = 0; i < otros.length; i++)
            {
                otros[i].setAttribute("aria-pressed", otros[i] === chip ? "true" : "false");
            }

            return;
        }

        var activo = chip.getAttribute("aria-pressed") === "true";
        chip.setAttribute("aria-pressed", activo ? "false" : "true");
    }

    function alternarClave(boton)
    {
        var campo = document.getElementById(boton.getAttribute("data-ver-clave"));

        if (!campo) return;

        var oculta = campo.type === "password";

        campo.type = oculta ? "text" : "password";
        boton.setAttribute("aria-pressed", oculta ? "true" : "false");
        boton.title = oculta ? "Ocultar la contrasena" : "Mostrar la contrasena";

        mostrarIconosClave(boton, oculta);
    }

    function mostrarIconosClave(boton, visible)
    {
        var ojoAbierto = boton.querySelector("[data-icono-clave='ver']");
        var ojoCerrado = boton.querySelector("[data-icono-clave='ocultar']");

        if (ojoAbierto) ojoAbierto.style.display = visible ? "none" : "";
        if (ojoCerrado) ojoCerrado.style.display = visible ? "" : "none";
    }

    function traducir(diccionario)
    {
        if (!diccionario) return;

        var textos = document.querySelectorAll("[data-i18n]");
        var i;

        for (i = 0; i < textos.length; i++)
        {
            var clave = textos[i].getAttribute("data-i18n");

            if (diccionario[clave]) textos[i].textContent = diccionario[clave];
        }

        var atributos = document.querySelectorAll("[data-i18n-attr]");

        for (i = 0; i < atributos.length; i++)
        {
            aplicarAtributos(atributos[i], diccionario);
        }
    }

    function aplicarAtributos(elemento, diccionario)
    {
        var pares = elemento.getAttribute("data-i18n-attr").split(";");

        for (var i = 0; i < pares.length; i++)
        {
            var partes = pares[i].split(":");

            if (partes.length !== 2) continue;

            var atributo = partes[0].trim();
            var clave = partes[1].trim();

            if (diccionario[clave]) elemento.setAttribute(atributo, diccionario[clave]);
        }
    }

    function alHacerClick(evento)
    {
        var destino = evento.target;

        if (!destino || !destino.closest) return;

        var boton;

        boton = destino.closest("[data-accion='cambiar-tema']");
        if (boton)
        {
            alternarTema();
            return;
        }

        boton = destino.closest("[data-accion='plegar-sidebar']");
        if (boton)
        {
            alternarSidebar();
            return;
        }

        boton = destino.closest("[data-desplegable]");
        if (boton)
        {
            alternarDesplegable(boton);
            return;
        }

        boton = destino.closest("[data-abre-modal]");
        if (boton)
        {
            evento.preventDefault();
            abrirModal(boton.getAttribute("data-abre-modal"), boton);
            return;
        }

        boton = destino.closest("[data-cierra-modal]");
        if (boton)
        {
            evento.preventDefault();
            cerrarModal();
            return;
        }

        if (destino.classList && destino.classList.contains("modal-fondo"))
        {
            cerrarModal();
            return;
        }

        boton = destino.closest(".acordeon-boton");
        if (boton)
        {
            alternarAcordeon(boton);
            return;
        }

        boton = destino.closest(".pestana");
        if (boton)
        {
            activarPestana(boton);
            return;
        }

        boton = destino.closest(".chip");
        if (boton)
        {
            alternarChip(boton);
            return;
        }

        boton = destino.closest("[data-ver-clave]");
        if (boton)
        {
            alternarClave(boton);
            return;
        }

        if (!destino.closest(".desplegable-panel")) cerrarDesplegables(null);
    }

    function alPresionarTecla(evento)
    {
        if (evento.key === "Escape")
        {
            if (modalAbierto)
            {
                cerrarModal();
                return;
            }

            cerrarDesplegables(null);
            return;
        }

        atraparTab(evento);
    }

    function alCambiar(evento)
    {
        var entrada = evento.target;

        if (!entrada || !entrada.closest) return;

        var opcion = entrada.closest(".opcion");

        if (!opcion) return;

        if (entrada.type === "radio" && entrada.name)
        {
            var hermanos = document.querySelectorAll("input[name='" + entrada.name + "']");

            for (var i = 0; i < hermanos.length; i++)
            {
                var caja = hermanos[i].closest(".opcion");

                if (caja) caja.classList.toggle("marcada", hermanos[i].checked);
            }

            return;
        }

        opcion.classList.toggle("marcada", entrada.checked);
    }

    function iniciar()
    {
        iniciarTema();
        iniciarSidebar();

        document.addEventListener("click", alHacerClick);
        document.addEventListener("keydown", alPresionarTecla);
        document.addEventListener("change", alCambiar);

        var marcadas = document.querySelectorAll(".opcion input:checked");

        for (var i = 0; i < marcadas.length; i++)
        {
            var caja = marcadas[i].closest(".opcion");

            if (caja) caja.classList.add("marcada");
        }
    }

    window.Falke = {
        abrirModal: abrirModal,
        cerrarModal: cerrarModal,
        alternarTema: alternarTema,
        traducir: traducir
    };

    if (document.readyState === "loading")
    {
        document.addEventListener("DOMContentLoaded", iniciar);
    }
    else
    {
        iniciar();
    }
})();
