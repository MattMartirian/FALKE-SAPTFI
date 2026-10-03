# FALKE — Fachada de la interfaz web

Documento de la capa GUI: qué se construyó, cómo está armado y por qué.
Complementa a `CONEXION-GUI.md`, que explica qué hay que hacer para que cada pantalla
tenga funcionalidad real.

**Estado actual:** todo lo que hay acá es **fachada**. Las únicas pantallas con
funcionalidad real son `Ingresar.aspx` (login) y `Salir.aspx` (logout). El resto tiene
la navegación armada y los formularios completos, pero los botones no guardan nada.

---

## 1. Qué se construyó

### 1.1 Archivos base (compartidos por todas las pantallas)

| Archivo | Qué es |
|---|---|
| `Content/falke.css` | Hoja de estilos global. Paleta, tipografía, layout y todos los componentes reutilizables. |
| `Scripts/falke.js` | JavaScript global. Tema día/noche, sidebar, modales, acordeones, pestañas, chips, ver contraseña y el ayudante de traducción. |
| `Controles/Iconos.ascx` | Sprite SVG con ~45 íconos. Se incluye una vez por página y se usan con `<use href="#i-nombre">`. |
| `Publico.master` | Master page de las pantallas públicas (navbar + pie). |
| `App.master` | Master page del dashboard (topbar + sidebar plegable). |

No hay ninguna librería ni framework: HTML, CSS y JavaScript a mano. La única
dependencia externa es la tipografía de marca, servida desde Google Fonts (ver 3.3); si
no hay conexión, cae en la pila de fuentes de sistema definida en `falke.css` y la
página se sigue viendo bien.

### 1.2 Pantallas públicas (`Publico.master`)

| Pantalla | Archivo | Función |
|---|---|---|
| Inicio | `Default.aspx` | Landing comercial: qué es FALKE, propuesta de valor, cómo funciona, comparación contra webcam y contra armar el equipo propio, segmentos y llamado a la acción. |
| Planes | `Planes.aspx` | Los tres planes (Scout, Hunter, Apex) con facturación mensual/anual y tabla comparativa. El botón "Contratar" abre el formulario de contacto con el mensaje ya escrito y editable. |
| Preguntas frecuentes | `Faq.aspx` | 19 preguntas en 5 grupos, con buscador y filtro por categoría. Acordeón accesible. |
| Iniciar sesión | `Ingresar.aspx` | **Funcional.** Email + contraseña, ver contraseña, recordarme y enlace a recuperación. |
| Cerrar sesión | `Salir.aspx` | **Funcional.** Cierra la sesión y muestra la confirmación. |
| Cómo obtener una cuenta | `Registro.aspx` | Reemplaza al registro automático: explica el circuito comercial en 6 pasos y por qué no hay alta libre. Deriva a Planes y a FAQ. |
| Recuperar contraseña | `RecuperarClave.aspx` | Pedido del enlace de recuperación por email. |

### 1.3 Pantallas del dashboard (`App.master`)

| Pantalla | Archivo | Rol que la ve | Función |
|---|---|---|---|
| Página principal | `Panel.aspx` | Todos | Saludo, accesos rápidos, métricas del mes, últimas sesiones, estado del dispositivo y actividad reciente. |
| Visualizaciones | `Visualizaciones.aspx` | Analista, Administrador, Gestor | Navbar vertical de categorías pegada al menú; al elegir una aparecen los botones grandes de sesión; al elegir sesión, las métricas y los 7 análisis. Cada análisis abre el reproductor con controles propios. Exportar reporte con casillas de selección. |
| Nueva grabación | `NuevaGrabacion.aspx` | Analista, Administrador, Gestor | Asistente de 3 pasos: datos de la sesión, calibración del dispositivo (USB, calibración, driver y versión) y captura de pantalla. |
| Categorías | `Categorias.aspx` | Analista, Administrador, Gestor | Categorías de análisis de la empresa. Los campos del formulario cambian según el tipo de activo. |
| Análisis múltiple | `AnalisisMultiple.aspx` | Analista, Administrador, Gestor | Comparación promediada de dos o más sesiones del mismo tipo de activo. |
| Usuarios | `Usuarios.aspx` | Todos | Ficha de la empresa, integrantes y sus roles, perfil propio (nombre, apellido, idioma; el email no se toca) y, para el administrador, invitar y cambiar estados. |
| Cambiar contraseña | `MiClave.aspx` | Todos | Cambio de la contraseña propia con requisitos y barra de fuerza. |
| Avisos | `Notificaciones.aspx` | Todos | Listado completo de notificaciones, con filtro y marcar como leídas. |
| Empresas cliente | `Empresas.aspx` | Gestor | Cartera de clientes con plan, estado y dispositivos prestados. Alta de empresa junto con su primer administrador. |
| Dispositivos | `Dispositivos.aspx` | Gestor | Inventario de eye trackers, préstamos, devoluciones y mantenimiento. |
| Respaldos y mantenimiento | `Respaldos.aspx` | Gestor | Copias de seguridad, restauración (cierra todas las sesiones) y programación de la ventana de mantenimiento con el aviso a todos los usuarios. |
| Bitácora | `Bitacora.aspx` | Administrador, Gestor, Webmaster | Registro de movimientos con alcance según el rol y filtros por fecha, hora, módulo, acción, usuario, criticidad y — solo para el gestor — empresa. |
| Dígito verificador | `Integridad.aspx` | Webmaster | Estado de integridad de la base: tablas con problemas, registros inconsistentes, DVH/DVV y recálculo. |
| Sin permiso | `SinPermiso.aspx` | — | A donde cae el usuario logueado que pide una sección que su rol no incluye. |
| Mantenimiento | `Mantenimiento.aspx` | — | Pantalla que ve el usuario común mientras el sistema está bloqueado. No usa master page. |

---

## 2. Pantallas que no estaban en la lista original

Estas salieron de leer el TFI y de tapar agujeros de navegación. Cada una existe porque
sin ella queda un camino cortado.

**`RecuperarClave.aspx` — Recuperar contraseña.**
El login tiene el enlace "Olvidé mi contraseña" y tenía que llevar a algún lado. Pide el
email y avisa que se mandó el enlace. El mensaje de confirmación es siempre el mismo
exista o no la cuenta, para no revelar qué emails están registrados.

**`MiClave.aspx` — Cambiar mi contraseña.**
Los usuarios no se registran solos, pero sí tienen que poder cambiar su clave desde
adentro. Está enlazada desde el modal de perfil. Valida los requisitos en pantalla y
avisa que el cambio cierra las sesiones abiertas en otros equipos.

**`Categorias.aspx` — Categorías de análisis.**
Visualizaciones filtra por categoría y Nueva grabación pide elegir una: alguien las tiene
que dar de alta. Como el TFI define tipos de activo distintos (software, app web, app
móvil, videojuego, publicidad) y cada uno necesita datos propios, el formulario cambia
según el tipo elegido.

**`AnalisisMultiple.aspx` — Análisis múltiple.**
El TFI pide comparar varias sesiones y exige que sean del mismo tipo de activo. Es una
pantalla propia porque la selección múltiple y las métricas promediadas no entran cómodas
dentro de Visualizaciones. La regla de compatibilidad está aplicada: la primera sesión
elegida fija el tipo y deshabilita las incompatibles; hacen falta al menos dos.

**`Dispositivos.aspx` — Inventario de dispositivos.**
El hardware se presta con la suscripción, así que Pattern Blue necesita saber dónde está
cada equipo, a quién se lo prestó, desde cuándo y cuándo se calibró por última vez. La
pantalla de empresas muestra cuántos dispositivos tiene cada cliente; el detalle vive acá.

**`Notificaciones.aspx` — Avisos.**
La campana del encabezado muestra los últimos tres y necesitaba un "ver todos". Además es
donde aterriza el aviso de mantenimiento que dispara el gestor.

**`SinPermiso.aspx` — Acceso denegado.**
Con cinco roles y permisos por patente, tarde o temprano alguien pide una URL que no le
corresponde. Mostrarle un error del servidor es feo y no le dice qué hacer; esta pantalla
le explica que le falta el permiso y a quién pedírselo.

**`Mantenimiento.aspx` — Sistema en mantenimiento.**
El TFI pide poder bloquear la página. Si se bloquea, el usuario común tiene que ver algo
que le explique qué pasa y cuándo vuelve, no un error. No usa master page a propósito:
durante el bloqueo no hay ninguna otra pantalla a la que se pueda ir.

---

## 3. Paleta y temas

La paleta de marca sigue la guía de estilo "Azul Halcón v2" (`falke-guia-de-estilo.md`).
Cinco roles, cada uno con un trabajo fijo, nunca intercambiables entre sí:

| Nombre | Valor | Rol |
|---|---|---|
| Azul Halcón | `#0E1A34` | Estructura: fondos, superficies grandes, texto sobre claro. |
| Dorado | `#E8AF52` | Marca / acción: botones, links, resaltados, seleccionado. |
| Coral | `#F26157` | Señal en vivo: grabando, avisos sin leer, picos de un mapa de calor. **Nunca en botones ni en navegación** — ese lugar es del dorado. |
| Gris azulado | `#56637C` | Texto secundario. |
| Perla | `#FCFCF7` | Fondo general, texto invertido sobre oscuro. |

El coral es el color más delicado del sistema: se usa solo para lo que es literalmente un
dato en vivo (el punto de "grabando" en `NuevaGrabacion.aspx`, el círculo de avisos sin
leer de la campana, los picos de los mapas de calor y los marcadores de los gráficos de
mirada) y como máximo en badges o alertas — nunca como relleno de un botón. Por eso
`.btn-peligro` es un botón de contorno (borde y texto en el tono de peligro, sin relleno
sólido): la acción "peligrosa" se distingue por el ícono y el texto, no por competirle al
dorado el rol de color de acción.

### 3.1 Cómo funcionan los dos temas

Todos los colores están definidos como variables CSS en `falke.css`:

```css
:root { --fondo: #F2F1EA; --texto: #0E1A34; ... }                 /* día */
html[data-tema="noche"] { --fondo: #0B1528; --texto: #FCFCF7; ... } /* noche */
```

El tema se guarda en `localStorage` con la clave `falke.tema`. Cada master page tiene un
script mínimo en el `<head>` que aplica el atributo `data-tema` **antes** de pintar la
página, para que no se vea el destello blanco al entrar en modo noche. El botón de sol/luna
de la topbar llama a `Falke.alternarTema()`.

Si el usuario nunca eligió, se usa la preferencia del sistema operativo
(`prefers-color-scheme`).

### 3.2 Contraste

Los colores de la marca no siempre alcanzan el contraste que pide WCAG AA sobre fondo
claro, así que se usan las variantes derivadas que fija la guía de estilo:

- `--acento-texto` día: `#976819` (dorado oscurecido, ~4.7:1 sobre blanco).
- `--acento-texto` noche: `#FAC26E` (dorado aclarado, ~11:1 sobre el fondo oscuro).
- `--texto-suave` / `--texto-tenue` noche: `#848DAF` / `#A0A6BB` (gris-azulado legible
  sobre superficies oscuras).
- `--peligro` día: `#C4372C` (~5.2:1 sobre blanco). Es un rojo de sistema para texto y
  badges de estado, distinto del coral de marca — el coral queda reservado para señales
  en vivo (ver 3.1 de `falke-guia-de-estilo.md`).

El color nunca es el único indicador: los estados llevan además texto, ícono o forma
(por ejemplo, la barrita de criticidad de la bitácora y el punto de estado de los
dispositivos van siempre acompañados de la palabra).

### 3.3 Tipografía

Tres familias, tres trabajos distintos, cargadas desde Google Fonts con la pila de
sistema como respaldo (`--fuente-titulo`, `--fuente` y `--fuente-mono` en `falke.css`):

| Familia | Uso | Dónde se ve |
|---|---|---|
| **Space Grotesk** (Bold) | Titulares | Todos los `h1`–`h5` de la fachada, automático en toda pantalla. |
| **Manrope** (Regular/Medium) | Cuerpo, botones, formularios | El resto del texto. |
| **Space Mono** (Regular/Bold) | Datos, etiquetas técnicas | `.mono` (series, hashes), `.metrica-valor`/`.metrica-nombre` (todos los números de las pantallas de métricas), el nombre de sección de la topbar, los títulos de grupo del sidebar, la etiqueta del hero y la vista previa del producto en `Default.aspx`. |

El monoespaciado en los números es a propósito: un dato en Space Mono se lee como "esto
se midió", no como una cifra decorativa. No se aplicó a los encabezados de las tablas
densas (Usuarios, Bitácora, Dispositivos, etc.) para no arriesgar que las columnas se
desborden — ahí la letra sigue siendo Manrope en mayúsculas.

---

## 4. Estructura visual

### 4.1 Pantallas públicas

```
┌─────────────────────────────────────────┐
│ navbar: FALKE  Inicio Planes FAQ ...    │  ← .nav-publica, pegada arriba
├─────────────────────────────────────────┤
│                                         │
│   contenido (.contenedor, máx 1200px)   │
│                                         │
├─────────────────────────────────────────┤
│ pie azul con enlaces y datos legales    │  ← .pie
└─────────────────────────────────────────┘
```

### 4.2 Dashboard

```
┌─────────────────────────────────────────┐
│ topbar: logo · SECCIÓN · 🔔 ☀ 👤        │  ← ancho completo, pegada arriba
├───────────┬─────────────────────────────┤
│ sidebar   │                             │
│ Análisis  │   contenido de la pantalla  │
│ Organiz.  │                             │
│ Sistema   │                             │
│ ‹ plegar  │                             │
└───────────┴─────────────────────────────┘
```

La topbar ocupa todo el ancho y el sidebar arranca debajo. El sidebar se pliega a solo
íconos con el botón del pie; el estado queda guardado en `localStorage`
(`falke.sidebar`). En pantallas chicas el sidebar se pliega solo.

El menú tiene tres grupos y cada uno se oculta entero si el rol no tiene ninguna de sus
opciones:

- **Análisis:** Página principal, Visualizaciones, Nueva grabación, Categorías, Análisis múltiple.
- **Organización:** Usuarios, Empresas, Dispositivos.
- **Sistema:** Bitácora, Dígito verificador, Respaldos.

### 4.3 La topbar

- **Izquierda:** logo FALKE, que además vuelve al panel.
- **Centro:** nombre de la sección actual (sale del `Title` de la página).
- **Derecha:** tres botones.
  - **Campana:** abre el panel de avisos con los últimos tres y el enlace a
    `Notificaciones.aspx`. Lleva un círculo coral cuando hay avisos sin leer.
  - **Sol/luna:** cambia el tema.
  - **Persona:** muestra nombre, apellido, email y rol, más "Ver mi perfil" (que abre
    `Usuarios.aspx?perfil=1`, o sea la pantalla de usuarios con el perfil propio ya
    abierto) y "Cerrar sesión".

---

## 5. Accesibilidad

Lo que se tuvo en cuenta, para que no haya que redescubrirlo después:

- **Enlace de salto.** Primer elemento que recibe el foco con Tab en las dos master
  pages: lleva directo al contenido y saltea el menú.
- **HTML con sentido.** `<header>`, `<nav>`, `<main>`, `<section>`, `<article>`,
  `<table>` con `<caption>` y `<th scope>`. Los títulos van en orden, sin saltar niveles.
- **Todo se maneja con teclado.** No hay nada que dependa de pasar el mouse. Los modales
  atrapan el foco mientras están abiertos, se cierran con Esc y devuelven el foco al
  botón que los abrió.
- **Estados anunciados.** `aria-expanded` en acordeones y desplegables, `aria-pressed` en
  chips y botones que alternan, `aria-selected` en pestañas, `aria-current="page"` en la
  opción del menú donde estás parado.
- **Mensajes que aparecen solos.** Van con `role="status"` (informativos) o `role="alert"`
  (errores), para que el lector de pantalla los lea sin que el usuario tenga que buscarlos.
- **Íconos.** Siempre `aria-hidden="true"`, porque son decorativos: al lado hay texto. Si
  un botón es solo ícono, lleva `aria-label` o un `<span class="solo-lectores">`.
- **Foco visible.** Ningún `outline: none` sin reemplazo; el foco se ve en los dos temas.
- **Formularios.** Cada campo tiene su `<label for>`, y los textos de ayuda se asocian con
  `aria-describedby` donde hace falta.
- **Contraste.** Ver el punto 3.2.
- **Movimiento.** Las transiciones son cortas y suaves; no hay nada que parpadee ni se
  mueva solo.

---

## 6. Preparación para traducir

Todavía no hay traducción real, pero está todo marcado para que agregarla sea mecánico.

### 6.1 La convención

Cada texto visible lleva un atributo `data-i18n` con su clave:

```html
<h1 data-i18n="usuarios.titulo">Usuarios</h1>
<p data-i18n="usuarios.soloLectura">Como analista podes ver...</p>
```

La clave se arma como `pantalla.bloque.elemento`, siempre en minúscula y en camelCase
cuando hace falta (`altaEmpresa.admin.email.ayuda`). El texto que está escrito en el HTML
es el de español, que hace de valor por defecto: si falta una traducción, se ve ese.

Para los textos que viven en atributos:

```html
<input data-i18n-attr="placeholder:empresas.buscar.placeholder" />
<button data-i18n-attr="title:comun.cerrar;aria-label:comun.cerrar">
```

### 6.2 Cómo se aplica

`falke.js` expone:

```js
Falke.traducir(diccionario);
```

Recorre todo lo que tenga `data-i18n` y `data-i18n-attr` y reemplaza el contenido con lo
que encuentre en el diccionario. El diccionario es un objeto plano
(`{ "usuarios.titulo": "Users", ... }`) que en su momento va a venir del servidor según el
idioma del usuario.

### 6.3 Textos que se arman en JavaScript

Algunos mensajes se construyen en el momento (contadores, el aviso de mantenimiento, la
fuerza de la contraseña). Esos no pueden llevar `data-i18n`, así que están marcados en el
código con:

```js
//TODO traducir: texto armado en el JS.
```

Buscando `TODO traducir` en la carpeta GUI aparecen todos.

### 6.4 Idiomas previstos

El perfil del usuario deja elegir español, inglés y portugués. La preferencia se guarda en
el usuario, no en el navegador, así que lo acompaña a cualquier equipo donde entre.

---

## 7. Decisiones que conviene conocer

**Los archivos son todos nuevos.** No se tocó ninguna pantalla que ya existía. Las
pantallas de prueba anteriores (`Login.aspx`, `Logout.aspx`, `MenuPruebas.aspx`,
`RegistrarUsuario.aspx`, etc.) siguen intactas; el login y el logout nuevos se llaman
`Ingresar.aspx` y `Salir.aspx` para no pisarlas.

**Se puede recorrer todo sin base de datos.** Los code-behind del dashboard tienen la
línea `SesionActual_GUI.Exigir()` comentada con su TODO, y `App.master` cae en un usuario
de ejemplo cuando no hay sesión. Así se puede revisar la fachada entera sin cargar datos.

**Hay un selector de rol para probar.** Agregando `?rol=Analista`, `?rol=Administrador`,
`?rol=Gestor` o `?rol=Webmaster` a cualquier URL del dashboard se cambia el rol de la
vista previa y se ve cómo queda el menú de cada uno. Está marcado en `App.master` con
`//TODO: BORRAR ESTE BLOQUE` y hay que sacarlo antes de que esto salga a producción.

**Los datos son inventados pero coherentes.** Las empresas, los usuarios, los números de
serie y las métricas están escritos en el HTML. Son consistentes entre pantallas (la misma
empresa aparece con los mismos datos en Usuarios, Empresas y Dispositivos) para que se
entienda cómo se relacionan las cosas.

**Los permisos se muestran, no se aplican.** Ocultar una opción del menú es comodidad, no
seguridad: cualquiera puede escribir la URL. Por eso cada pantalla tiene su
`ExigirPermiso()` comentado, y en `CONEXION-GUI.md` está anotado que el filtrado por rol
tiene que hacerse en la consulta, nunca escondiendo filas en el navegador.

**El código es a propósito simple.** Bucles `for` clásicos, `getElementById`, un listener
delegado por tipo de evento, sin clases ni patrones raros. La idea es que se pueda leer de
arriba a abajo sin tener que descifrar nada.

---

## 8. Componentes disponibles en `falke.css`

Referencia rápida para cuando haya que armar una pantalla nueva sin inventar estilos.

| Componente | Clases |
|---|---|
| Botones | `.btn` + `.btn-primario`, `-secundario`, `-oscuro`, `-peligro`, `-fantasma`, `-chico`, `-grande`, `-bloque` |
| Tarjetas | `.tarjeta`, `.tarjeta-cabecera`, `.tarjeta-pie` |
| Grillas | `.grid` + `.grid-2`, `.grid-3`, `.grid-4` |
| Métricas | `.metricas`, `.metrica-valor`, `.metrica-nombre` |
| Formularios | `.campo`, `.entrada`, `.ayuda`, `.etiqueta`, `.fila-campos`, `.opcion`, `.casilla-simple`, `.campo-con-boton`, `.boton-dentro` |
| Filtros | `.filtros`, `.chips`, `.chip` |
| Tablas | `.tabla-scroll`, `.tabla`, `.celda-doble`, `.acciones-fila` |
| Estados | `.badge-*`, `.aviso-*`, `.vacio` |
| Navegación | `.pestanas`, `.pestana`, `.acordeon*`, `.migas`, `.lista-lateral`, `.item-lista` |
| Superpuestos | `.modal-fondo`, `.modal`, `.modal-cabecera`, `-cuerpo`, `-pie`, `.desplegable*` |
| Gráficos | `.grafico`, `.grafico-calor`, `.grafico-video`, `.punto-mirada`, `.linea-recorrido`, `.zona-interes`, `.zona-ciega`, `.barra` |
| Pieza exhibida | `.marco-pieza` + `.esquina-si/-sd/-ii/-id` (corner brackets), `.etiqueta-registro` — usar solo donde algo se presenta como objeto de marca, no en pantallas de trabajo. |
| Utilidades | `.mt-*`, `.mb-*`, `.ml-8`, `.fila`, `.fila-entre`, `.centrado`, `.derecha`, `.mono`, `.medida`, `.sin-margen`, `.solo-lectores` |

Todo lo que sea específico de una pantalla va en el `<style>` de esa página, dentro del
`ContentPlaceHolderID="cabeza"`. Lo que se repite en tres o más pantallas conviene subirlo
a `falke.css`.
