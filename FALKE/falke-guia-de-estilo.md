# Falke — Guía de Estilo Visual

**Azul Halcón v2 · Dirección: Vitrina técnica + Escultura de datos**
Documento de referencia para diseño de marca y producto. Basado en las conclusiones del TFI y en las direcciones visuales exploradas en Figma (`FALKE_WEBDESIGN`).

---

## 1. Qué debe transmitir la marca

Falke es una herramienta de eye-tracking y mapas de calor para equipos de producto (estudios de videojuegos, agencias de publicidad digital, empresas de software). Está en etapa de introducción al mercado: todavía tiene que educar antes de vender, así que cada pieza debe poder funcionar como material de demo, no solo como imagen de portada.

Todo lo visual debe reforzar tres ideas del TFI:

- **Precisión práctica, no sofisticación técnica.** La marca explícitamente *no* debe sentirse a "laboratorio de investigación". Cuanto más se acerque una pieza a "panel de control en uso" y más se aleje de "vitrina de ciencia", mejor.
- **Evidencia concreta, no opinión.** El eje narrativo de toda la marca es que Falke convierte sesiones de uso en datos medibles y verificables — no en impresiones subjetivas de un diseñador o un product manager.
- **Confianza y manejo serio de datos sensibles.** El producto captura biometría real (mirada, atención) bajo la Ley 25.326. La identidad visual tiene que transmitir seriedad y control, no solo estética — nunca frivolidad ni "gadget" de consumo.

La dirección visual que se estuvo desarrollando (cruce de **Bitácora de Sesión** y **Escultura de Datos**) responde directamente a esto: trata cada pieza de comunicación como un objeto documentado y medido — con número de registro, marcadores numerados, coordenadas — en vez de una ilustración decorativa.

---

## 2. Dirección visual: vitrina técnica + objeto de datos

La estética de referencia combina dos lenguajes que ya estaban en el catálogo de direcciones:

- **De "Bitácora de Sesión"**: el marco de "documento técnico de campo" — corner brackets, número de pieza/registro, pines numerados conectados por líneas punteadas a una anotación breve. Es el lenguaje que convierte cualquier imagen en "evidencia catalogada".
- **De "Escultura de Datos"**: el objeto geométrico limpio como protagonista — el heatmap convertido en volumen isométrico. Es lo que le da a la marca una imagen "de exhibición", memorable y distinta de cualquier dashboard de SaaS genérico.

**Regla de combinación:** el objeto geométrico siempre es el protagonista; el aparato de señalización (brackets, tags, pines) se agrega con moderación alrededor de él, nunca encima ni compitiendo en peso visual. Si una pieza empieza a sentirse como un diagrama de ingeniería en vez de un objeto exhibido, hay que sacar elementos de señalización, no agregar más objeto.

Elementos reutilizables de esta dirección:

- Corner brackets (4 esquinas, trazo fino, color dorado) enmarcando cualquier imagen/objeto que se quiera presentar como "pieza".
- Etiqueta tipo "PIEZA N.° 0X · FALKE" o "REG · 0X" en una esquina, tipografía monoespaciada, color secundario.
- Pines numerados (círculo relleno + número) conectados por una línea punteada a una anotación corta en mayúsculas monoespaciadas.
- El objeto (heatmap, sesión, sculpture) siempre flota directamente sobre el fondo oscuro del panel — nunca con una caja de fondo clara separada detrás, que rompe la idea de "vitrina".

---

## 3. Paleta de color — Azul Halcón v2

Reemplaza a la paleta institucional original (navy/dorado/perla). Cinco roles, cada uno con un trabajo específico — nunca intercambiables entre sí.

| Rol | Color | Hex | Uso |
|---|---|---|---|
| Estructura / principal | Azul Halcón | `#0E1A34` | Fondos, superficies grandes, texto sobre claro |
| Marca / acción | Dorado | `#E8AF52` | CTAs, links, brackets, badges, logo — "hacé algo" |
| Señal / dato en vivo | Coral | `#F26157` | Indicadores en vivo, picos de atención, alertas — nunca botones ni navegación |
| Texto secundario | Gris-azulado | `#56637C` | Cuerpo de texto secundario, captions, chrome de UI |
| Fondo | Blanco perlado | `#FCFCF7` | Fondo general, texto invertido sobre oscuro |

### Variantes derivadas (necesarias en la práctica)

Ningún color de marca funciona en todos los contextos con un solo valor. Estas variantes ya se usaron en piezas construidas y conviene fijarlas como parte del sistema:

- **Dorado sobre fondo claro (texto/labels chicos):** `#9F6E22` — el E8AF52 puro no tiene contraste suficiente como texto sobre blanco.
- **Dorado sobre fondo oscuro (texto/labels chicos, variante clara):** `#FAC26E`.
- **Secundario sobre fondo oscuro (texto de apoyo legible):** `#848DAF` (uso general) / `#A0A6BB` (variante más sutil, para chrome de baja jerarquía).
- **Paneles oscuros anidados** (vitrinas, pantallas dentro de una pieza, "poster" técnico): `#0B1428`, o variantes más claras como `#132245` / `#273C6B` para dar profundidad dentro del mismo panel oscuro.
- **Líneas y bordes:** no necesitan un color propio. Usar el color principal (`#0E1A34`) u blanco perlado al 8–12% de opacidad según el fondo.

### Reglas de uso del coral

El coral es el color más frágil del sistema porque es el único sin un rol claro si se usa mal. Reglas fijas:

1. Solo para lo que es literalmente un dato en vivo o una alerta: punto de "grabando/conectado", pico de un heatmap, marcador de un dato puntual.
2. Nunca en botones, links o navegación — ese trabajo es del dorado. Si el coral empieza a aparecer donde antes iba dorado, se pierde la distinción entre "esto es marca" y "esto es evidencia".
3. Uso mínimo: idealmente no más del 2–3% del área visual de una pieza. Un punto, un trazo, un número — no un bloque de color.
4. Como texto sobre fondo claro tiene contraste insuficiente (ver tabla de accesibilidad) — usarlo ahí solo en tamaños grandes, badges o dentro de un dot/marcador, nunca en texto corrido.

### Contraste y accesibilidad (contra `#FCFCF7`)

| Color | Ratio aprox. | Uso como texto sobre claro |
|---|---|---|
| `#0E1A34` | ~17:1 | Excelente — texto de cualquier tamaño |
| `#56637C` | ~6:1 | Bien — pasa AA para texto normal |
| `#F26157` | ~3.2:1 | Solo texto grande / UI, no texto de cuerpo |
| `#E8AF52` | ~2:1 | Insuficiente como texto — usar solo en fondos, iconos o bloques |

Consecuencia práctica: cualquier pieza con fondo claro (como Escultura de Datos) necesita las variantes oscurecidas de dorado y coral para texto; cualquier pieza con fondo oscuro puede usar los tonos base o sus variantes claras sin problema.

---

## 4. Tipografía

Combinación usada en las piezas ya construidas, pensada para separar tres registros distintos:

- **Space Grotesk (Bold)** — titulares. Geométrica, contemporánea, sin serifa: cumple el pedido explícito del TFI de tipografía "moderna, alta legibilidad".
- **Manrope (Regular / Medium)** — cuerpo de texto, bajadas, botones. Neutra y muy legible en párrafos largos.
- **Space Mono (Regular / Bold)** — todo lo que sea dato, etiqueta técnica o sistema de señalización: eyebrows, badges, números de registro, pines, leyendas de color. El monoespaciado es lo que hace que un número se lea como "dato medido" y no como decoración — es clave para el concepto de "evidencia, no opinión".

Otras combinaciones (Big Shoulders Display, IBM Plex, Darker Grotesque, Syne, Chivo, etc.) siguen viviendo en las direcciones individuales del catálogo, pero para la dirección combinada (vitrina + escultura) esta terna es la que se fijó.

---

## 5. Motivos y elementos gráficos

- **Corner brackets**: trazo de 2px, ~24–28px de largo por lado, color dorado. Señalan "esto es una pieza exhibida/medida".
- **Tags de registro**: texto mono, tamaño chico (9–10px), color secundario, esquina superior de cualquier panel.
- **Pines numerados + leader punteado**: círculo de 18–20px relleno de coral, número en blanco perlado, línea punteada (dash 4/3) al 50–60% de opacidad hacia una anotación en mayúsculas.
- **Objetos isométricos**: proyección de 3 caras visibles (superior/izquierda/derecha) con degradado de luminosidad entre caras para dar volumen — reservado para representar datos agregados (heatmaps, densidad de atención), no para ilustración genérica.
- **Degradados y resplandores**: usar solo como composición radial ("glows" difuminados) sobre fondo oscuro — nunca como degradado lineal directo entre navy y coral (el punto medio da un marrón sucio). Navy→dorado o dorado→coral sí funcionan como degradado lineal simple si hace falta.

---

## 6. Voz y tono (breve)

Palabras que refuerzan la marca: *evidencia, medido, en vivo, sesión, precisión, dato real, catalogado*.
Palabras a evitar: *laboratorio, algoritmo, inteligencia artificial* (como gancho principal), *insight mágico* — cualquier lenguaje que suene a caja negra en vez de a instrumento de medición transparente.

---

## 7. Qué evitar

- No mezclar el coral con el dorado en el mismo rol (los dos "compitiendo" por CTA o por acento de marca).
- No sobrecargar un objeto de Escultura de Datos con todo el aparato de Bitácora a la vez (grilla de datos + header pesado + brackets + tag): elegir 2–3 elementos de señalización, no el set completo.
- No usar dorado puro como texto de cuerpo sobre fondo claro.
- No dejar un panel/objeto claro flotando dentro de una vitrina oscura sin justificación — si el objeto tiene fondo propio, quitarlo o dejar que herede el fondo del panel.
- No usar el coral fuera de su rol de señal, aunque quede "bien" visualmente — la disciplina de uso es lo que le da significado.

---

## 8. Referencias

Piezas ya construidas en Figma (`FALKE_WEBDESIGN`) que documentan estas decisiones:

- **Direcciones visuales — Falke** / **Direcciones visuales II — Falke**: las diez direcciones originales, incluyendo Bitácora de Sesión y Escultura de Datos.
- **Paletas alternativas — Falke**: 14 paletas candidatas evaluadas antes de definir Azul Halcón v2.
- **Combinaciones recomendadas — Falke**: cruces dirección × paleta evaluados sobre el set original.
- **Azul Halcón v2 — Falke**: recolor de cuatro direcciones (02, 08, 09, 10) con la paleta definitiva.
- **Landing — Bitácora × Escultura — Falke**: concepto de pantalla de landing que aplica todo lo anterior en conjunto.
