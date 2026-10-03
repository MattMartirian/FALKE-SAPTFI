<%@ Control Language="C#" %>

<svg xmlns="http://www.w3.org/2000/svg" style="display:none" aria-hidden="true" focusable="false">

    <symbol id="i-logo" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M12 3 20.5 12 12 21 3.5 12Z" />
        <circle cx="12" cy="12" r="2.6" />
    </symbol>

    <symbol id="i-casa" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M3.5 10.5 12 3.5l8.5 7M5.5 9.5V20h13V9.5" />
    </symbol>
    <symbol id="i-ojo" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M2.5 12S6 5.5 12 5.5 21.5 12 21.5 12 18 18.5 12 18.5 2.5 12 2.5 12Z" />
        <circle cx="12" cy="12" r="3" />
    </symbol>
    <symbol id="i-ojo-tachado" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M4 12s3-6 8-6c1.3 0 2.5.3 3.5.9M20 12s-1 2.1-2.9 3.8M9.5 9.5a3 3 0 0 0 4.2 4.2M4 4l16 16" />
    </symbol>
    <symbol id="i-video" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <rect x="3" y="6" width="12.5" height="12" rx="2.5" />
        <path d="m15.5 10.5 5.5-3v9l-5.5-3Z" />
    </symbol>
    <symbol id="i-carpeta" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M3.5 6.5h5l2 2.5h10v9a1.5 1.5 0 0 1-1.5 1.5H5a1.5 1.5 0 0 1-1.5-1.5Z" />
    </symbol>
    <symbol id="i-comparar" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M4 20V11m5.3 9V5m5.4 15v-7m5.3 7V8" />
    </symbol>
    <symbol id="i-usuarios" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <circle cx="9.5" cy="8.5" r="3.2" />
        <path d="M3.5 19.5c0-3.1 2.7-5 6-5s6 1.9 6 5M16.5 6.2a3 3 0 0 1 0 5.6m1.3 3.2c1.8.6 3 1.9 3 4" />
    </symbol>
    <symbol id="i-empresa" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M4 20V6.5L12 4v16M12 20h8V10l-8-2.5M7 9.5h2M7 13h2M7 16.5h2M15 12.5h2M15 16.5h2" />
    </symbol>
    <symbol id="i-dispositivo" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <rect x="2.5" y="8.5" width="19" height="7" rx="3.5" />
        <circle cx="8" cy="12" r="1.6" />
        <circle cx="16" cy="12" r="1.6" />
    </symbol>
    <symbol id="i-libro" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M5 4.5h11a2 2 0 0 1 2 2v13H7a2 2 0 0 1-2-2Z" />
        <path d="M5 17.5h13M8.5 8.5h6M8.5 12h6" />
    </symbol>
    <symbol id="i-escudo" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M12 3.5 19 6v6c0 4.2-3 7-7 8.5-4-1.5-7-4.3-7-8.5V6Z" />
        <path d="m9 12 2 2 4-4" />
    </symbol>
    <symbol id="i-base" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <ellipse cx="12" cy="6.5" rx="7.5" ry="3" />
        <path d="M4.5 6.5v11c0 1.7 3.4 3 7.5 3s7.5-1.3 7.5-3v-11M4.5 12c0 1.7 3.4 3 7.5 3s7.5-1.3 7.5-3" />
    </symbol>
    <symbol id="i-campana" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M6 9.5a6 6 0 0 1 12 0c0 4 1.5 5.5 1.5 5.5h-15S6 13.5 6 9.5Z" />
        <path d="M10 18.5a2 2 0 0 0 4 0" />
    </symbol>
    <symbol id="i-persona" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <circle cx="12" cy="8.5" r="3.5" />
        <path d="M4.5 20c0-3.6 3.4-5.5 7.5-5.5s7.5 1.9 7.5 5.5" />
    </symbol>
    <symbol id="i-engranaje" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <circle cx="12" cy="12" r="3" />
        <path d="M12 3v2.5M12 18.5V21M3 12h2.5M18.5 12H21M5.6 5.6l1.8 1.8M16.6 16.6l1.8 1.8M18.4 5.6l-1.8 1.8M7.4 16.6l-1.8 1.8" />
    </symbol>

    <symbol id="i-sol" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <circle cx="12" cy="12" r="4" />
        <path d="M12 2v2.5M12 19.5V22M2 12h2.5M19.5 12H22M4.9 4.9l1.8 1.8M17.3 17.3l1.8 1.8M19.1 4.9l-1.8 1.8M6.7 17.3l-1.8 1.8" />
    </symbol>
    <symbol id="i-luna" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M20 14.2A8.2 8.2 0 0 1 9.8 4a8.5 8.5 0 1 0 10.2 10.2Z" />
    </symbol>

    <symbol id="i-menu" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M4 7h16M4 12h16M4 17h16" />
    </symbol>
    <symbol id="i-cerrar" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="m6 6 12 12M18 6 6 18" />
    </symbol>
    <symbol id="i-mas" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M12 5v14M5 12h14" />
    </symbol>
    <symbol id="i-buscar" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <circle cx="10.5" cy="10.5" r="6" />
        <path d="m15 15 4.5 4.5" />
    </symbol>
    <symbol id="i-filtro" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M4 6h16l-6.2 7.3V19l-3.6-2v-3.7Z" />
    </symbol>
    <symbol id="i-descarga" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M12 4v10m0 0 3.8-3.8M12 14l-3.8-3.8M4.5 16.5v2A1.5 1.5 0 0 0 6 20h12a1.5 1.5 0 0 0 1.5-1.5v-2" />
    </symbol>
    <symbol id="i-flecha-der" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="m9 5 7 7-7 7" />
    </symbol>
    <symbol id="i-flecha-abajo" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="m5 9 7 7 7-7" />
    </symbol>
    <symbol id="i-lapiz" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M4 20h4L19 9a2.1 2.1 0 0 0-3-3L5 17Z" />
    </symbol>
    <symbol id="i-tacho" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M4.5 7h15M9.5 7V5h5v2M6.5 7l1 13h9l1-13" />
    </symbol>
    <symbol id="i-refrescar" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M20 12a8 8 0 1 1-2.6-5.9M20 4v4.5h-4.5" />
    </symbol>
    <symbol id="i-candado" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <rect x="5" y="10.5" width="14" height="9.5" rx="2" />
        <path d="M8.5 10.5V8a3.5 3.5 0 0 1 7 0v2.5" />
    </symbol>
    <symbol id="i-mail" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <rect x="3" y="5.5" width="18" height="13" rx="2" />
        <path d="m3.5 7 8.5 6 8.5-6" />
    </symbol>
    <symbol id="i-salir" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M14 4.5H6.5A1.5 1.5 0 0 0 5 6v12a1.5 1.5 0 0 0 1.5 1.5H14M17 8.5 20.5 12 17 15.5M20 12H10" />
    </symbol>

    <symbol id="i-tilde" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="m5 12.5 4.5 4.5L19 7" />
    </symbol>
    <symbol id="i-alerta" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M12 4.5 21 19.5H3Z" />
        <path d="M12 10v4M12 16.8v.2" />
    </symbol>
    <symbol id="i-info" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <circle cx="12" cy="12" r="8.5" />
        <path d="M12 11v5.5M12 8v.2" />
    </symbol>
    <symbol id="i-reloj" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <circle cx="12" cy="12" r="8.5" />
        <path d="M12 7v5.2l3.3 2" />
    </symbol>
    <symbol id="i-calendario" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <rect x="3.5" y="5.5" width="17" height="15" rx="2" />
        <path d="M3.5 10h17M8 3.5v3.5M16 3.5v3.5" />
    </symbol>

    <symbol id="i-play" viewBox="0 0 24 24" fill="currentColor" stroke="none">
        <path d="M8 5.5 19 12 8 18.5Z" />
    </symbol>
    <symbol id="i-pausa" viewBox="0 0 24 24" fill="currentColor" stroke="none">
        <path d="M8 5h3v14H8zM13 5h3v14h-3z" />
    </symbol>
    <symbol id="i-atras" viewBox="0 0 24 24" fill="currentColor" stroke="none">
        <path d="M11 12 19 6.5v11ZM4 12l7-5.5v11Z" />
    </symbol>
    <symbol id="i-adelante" viewBox="0 0 24 24" fill="currentColor" stroke="none">
        <path d="M13 12 5 6.5v11ZM20 12l-7-5.5v11Z" />
    </symbol>

    <symbol id="i-calor" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M12 3.5c3 4 5.5 6 5.5 9.5a5.5 5.5 0 0 1-11 0C6.5 9.5 9 7.5 12 3.5Z" />
    </symbol>
    <symbol id="i-zonas" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <rect x="3.5" y="4.5" width="17" height="15" rx="2" />
        <path d="M9.5 4.5v15M9.5 12h11" />
    </symbol>
    <symbol id="i-dispersion" viewBox="0 0 24 24" fill="currentColor" stroke="none">
        <circle cx="7" cy="8" r="1.6" />
        <circle cx="13" cy="6.5" r="1.6" />
        <circle cx="10" cy="14" r="1.6" />
        <circle cx="17" cy="11" r="1.6" />
        <circle cx="15.5" cy="17" r="1.6" />
        <circle cx="5.5" cy="17.5" r="1.6" />
    </symbol>
    <symbol id="i-torta" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <circle cx="12" cy="12" r="8.5" />
        <path d="M12 3.5v8.5h8.5" />
    </symbol>
    <symbol id="i-recorrido" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M3.5 17c3-8 6-1 9-6.5s5 2 8-2.5" />
    </symbol>
    <symbol id="i-metricas" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">
        <path d="M4 19.5V4M4 19.5h16M8 16v-4.5M12.5 16V7.5M17 16v-7" />
    </symbol>
</svg>
