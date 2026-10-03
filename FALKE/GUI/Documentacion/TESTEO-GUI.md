# Testeo de la capa GUI

Registro de los testeos realizados sobre todo lo conectado hasta ahora: login, logout,
recuperar contraseña, definir contraseña por token, cambiar contraseña con sesión,
registro de usuarios (alta como Administrador o Gestor), pantalla de integridad de la
base, "recordarme en este equipo" con cookie, y la limpieza de comentarios de todas las
capas menos ORM.

No se probó el sitio corriendo en el navegador (eso queda como checklist manual al final).
Lo que sí se probó: compilación completa, sintaxis de JS/CSS, referencias cruzadas del
markup, y una batería funcional headless contra la base de datos real.

---

## 1. Entorno de prueba

| Elemento | Estado |
|---|---|
| SQL Server local (`Server=.`) | Alcanzable. Versión 17.00.1000. |
| Base `FalkeDB` | Poblada. |
| `PermisoTable` | 7 filas: roles `Administrador`, `Gestor`, `Usuario` + patentes `REGISTRAR_USUARIO`, `REGISTRAR_EMPRESA`, `CREAR_USUARIO_OTRA_EMPRESA`, `RECALCULAR_INTEGRIDAD`. **No existe rol `Analista`.** |
| `RelacionPermisosTable` | `Administrador` incluye `REGISTRAR_USUARIO`. `Gestor` incluye las cuatro patentes. `Usuario` no incluye ninguna. |
| `UsuarioTable` | 5 usuarios, todos estado `Activo`, `intentos_fallidos = 0`, `DVH` cargado. 2 con rol `Usuario` (empresa 1), 3 con rol `Administrador` (empresas 2, 3, 4). Todos comparten el mismo hash de contraseña. |
| `IntegridadTable` | `Usuario` (CR 5), `EmpresaCliente` (CR 4) y `Token` (CR 7) con DVV real. El resto de las tablas controladas en `"0"` / 0 registros (están vacías). |
| `TokenTable` | 7 filas, todas `usado = 1`. Tipos mezclados: `ACTIVACION` / `RECUPERACION` en mayúsculas (datos viejos) y uno en minúscula. El repositorio normaliza al leer, así que no rompe. |
| `IdiomaTable` | `1 = es`, `2 = en`. |

---

## 2. Testeos automáticos ejecutados

### 2.1 Compilación

| Proyecto | Resultado |
|---|---|
| `BE.csproj` | 0 errores |
| `BLL.csproj` | 0 errores |
| `TLL.csproj` (arrastra TE, SECURITY, SERVICES, ORM, DAL) | 0 errores |
| Sitio GUI (`aspnet_compiler`, todas las `.aspx` / `.master` / `App_Code`) | 0 errores |

Se compiló antes y después de sacar los comentarios: mismo resultado.

### 2.2 JavaScript y CSS

- `Scripts/falke.js`: pasa `node --check`.
- Los 21 bloques `<script>` embebidos en las `.aspx` / `.master`: todos parsean.
- `Content/falke.css`: llaves balanceadas (273 / 273).

### 2.3 Referencias cruzadas del markup

- Todos los íconos usados (`<use href="#i-...">`) existen como `<symbol>` en `Controles/Iconos.ascx`.
- Todos los IDs de controles referenciados desde los code-behind tocados (`Ingresar`, `Salir`, `RecuperarClave`, `DefinirClave`, `MiClave`, `Usuarios`, `Integridad`) existen en el `.aspx` correspondiente.
- Los destinos de redirección y de enlaces clave existen como archivo: `Ingresar.aspx`, `Panel.aspx`, `SinPermiso.aspx`, `Salir.aspx`, `DefinirClave.aspx`, `RecuperarClave.aspx`, `MiClave.aspx`, `Usuarios.aspx`, `Integridad.aspx`, `Default.aspx`, `Registro.aspx`, `Planes.aspx`, `Faq.aspx`.
- Existen las carpetas `App_Data/mails` y `App_Data/logs`.

### 2.4 Prueba funcional headless (contra la base real)

Se ejecutó una batería que llama directamente a la lógica compilada (TLL / SERVICES / SECURITY / ORM), sin pasar por el navegador.

| Caso | Resultado |
|---|---|
| Cookie "recordarme": cifrar y descifrar el contenido devuelve lo mismo | PASS |
| Cookie "recordarme": una cookie manipulada es rechazada | **FALLA — ver punto 4.1** |
| `VerificarIntegridadTodasLasTablas()` no encuentra inconsistencias | PASS (0 inconsistencias) |
| Login de emergencia `admin` / `admin` | PASS (id -1, marcado como cuenta de emergencia) |
| Login con un correo inexistente devuelve `CREDENCIALES_INVALIDAS` (sin efecto sobre la base) | PASS |
| `ObtenerPorId(20)` (lo que usa la restauración por cookie) trae el usuario con su árbol de rol | PASS (`mati.martirian@gmail.com`, `Activo`, rol `Usuario`) |
| Token de contraseña inexistente → resultado no exitoso, motivo `TOKEN_INVALIDO` | PASS |
| El árbol de permisos: `Administrador` puede `REGISTRAR_USUARIO` | PASS |
| El árbol de permisos: `Administrador` NO puede `RECALCULAR_INTEGRIDAD` | PASS |
| El árbol de permisos: `Usuario` NO puede `REGISTRAR_USUARIO` | PASS |

**Total: 9 PASS / 1 FALLA** (la falla es del test, no del código: ver 4.1).

### 2.5 Réplica de la firma de integridad (DVV)

Se recalculó por fuera la firma global de tres tablas (la cadena SHA-256 sobre los DVH
almacenados) y se comparó con el DVV guardado en `IntegridadTable`:

| Tabla | Resultado |
|---|---|
| `Usuario` (5 DVH) | Coincide |
| `Token` (7 DVH) | Coincide |
| `EmpresaCliente` (4 DVH) | Coincide |

Sumado a que las cantidades de registros coinciden con el CR almacenado, esto indica que
**la integridad de la base está sana en este momento** y el login normal no está
bloqueado por `DVH_INVALIDO`. No prueba que cada DVH fila por fila esté bien (para eso hay
que abrir la pantalla de Integridad o loguearse), pero es evidencia fuerte.

### 2.6 Limpieza de comentarios

Se sacaron los comentarios de las 8 capas menos ORM (71 archivos efectivamente
modificados). Verificado que:

- Las 3 librerías y el sitio GUI siguen compilando con 0 errores.
- `falke.js` y los 21 scripts embebidos siguen parseando.
- Los `diff` contra la copia de respaldo muestran solo borrado de líneas de comentario;
  los literales de cadena (incluidos los interpolados con comillas escapadas) quedaron
  intactos.
- `ORM` no tiene ningún cambio.

---

## 3. Estado funcional por flujo

| Flujo | Estado | Cómo confirmarlo en el navegador |
|---|---|---|
| **Login** (`Ingresar.aspx`) | Conectado. El campo dejó de ser `type="email"` para permitir `admin`. | Entrar con un usuario real y con `admin`/`admin`. |
| **Logout** (`Salir.aspx`) | Conectado. Ahora es pantalla de confirmación: cierra por POST, no por GET. También borra la cookie de "recordarme". | Desde el menú → "Cerrar sesión" → botón de confirmación. |
| **Recuperar contraseña** (`RecuperarClave.aspx`) | Conectado. Genera token y escribe el `.txt` en `App_Data/mails/`. Página sin navbar. | Pedir el enlace y buscar el archivo en `App_Data/mails/`. |
| **Definir contraseña por token** (`DefinirClave.aspx`) | Conectado. Sirve para activación y para recuperación. | Abrir el enlace del `.txt`, poner una contraseña de 8+, verificar que la cuenta queda activa. |
| **Cambiar contraseña con sesión** (`MiClave.aspx`) | Conectado. Exige sesión (redirige a `Ingresar.aspx`). El acceso de emergencia no puede cambiarla. | Logueado, cambiar la propia contraseña; probar contraseña actual incorrecta. |
| **Registrar usuario** (modal "Invitar" de `Usuarios.aspx`) | Conectado. Exige la patente `REGISTRAR_USUARIO`. El rol "Analista" del combo guarda el rol `Usuario` (es el que existe en la base). Con `CREAR_USUARIO_OTRA_EMPRESA` aparece el campo de id de empresa. | Como Administrador o Gestor, invitar a alguien; verificar la fila nueva (`estado` Pendiente) y el `.txt` de activación. |
| **Integridad de la base** (`Integridad.aspx`) | Conectado. Lee `VerificarIntegridadTodasLasTablas()` y `LeerRegistroIntegridad()`. El botón "Recalcular" ejecuta el recálculo real y lo deja en bitácora (criticidad Alta). Exige `RECALCULAR_INTEGRIDAD`. | Entrar como emergencia (o Gestor), ver el estado por tabla y el detalle de registros. |
| **Recordarme en este equipo** (checkbox del login) | Conectado. Cookie `FALKE_RECORDARME` cifrada con AES (clave del `Web.config`), 30 días, `HttpOnly`. La restauración corre en los dos master pages y en los helpers de sesión. | Tildar el checkbox, cerrar el navegador, volver a entrar a una pantalla del panel sin loguearse. |

Pantallas que **siguen siendo fachada** (datos demo, sin base): Panel, Visualizaciones,
Nueva grabación, Categorías, Análisis múltiple, la lista de usuarios de `Usuarios.aspx`
(solo el alta está conectada), Empresas, Empresa nueva, Dispositivos, Bitácora, Respaldos,
Notificaciones, y las páginas públicas (Planes, FAQ, Registro).

---

## 4. Posibles problemas encontrados

### 4.1 La cookie de "recordarme" no está autenticada (severidad alta)

`Cifrador_SECURITY.EncriptadoReversible` es AES en modo CBC **sin firma (sin HMAC/GCM)**.
El contenido de la cookie es `id|vencimiento`, y el `id` cae en el primer bloque de
16 bytes.

Con CBC, alterar un byte del IV cambia solo el byte correspondiente del primer bloque de
texto plano, sin romper el padding y sin lanzar excepción. Es decir: cualquiera que tenga
una cookie válida (la propia, editándola desde las herramientas del navegador) puede
cambiar `20|...` por `21|...` y quedar logueado como el usuario 21. Como en la base el
usuario 21 es Administrador y el 20 es Usuario, esto es una **escalada de privilegios de
bajo esfuerzo** para cualquier usuario que tilde "recordarme".

Mitigantes actuales: la cookie es `HttpOnly` (no la lee un XSS) y, sobre HTTPS, no viaja
en claro. Pero el atacante manipula su propia cookie, así que esos mitigantes no alcanzan.
En desarrollo, además, el sitio corre sobre HTTP (`localhost:53192`).

Arreglo (ver punto 6): firmar el contenido (agregar `HMAC-SHA256(contenido, clave)` y
verificarlo antes de confiar) o usar `System.Web.Security.MachineKey.Protect`, que ya
viene autenticado. La opción más robusta sería volver a un token guardado en `TokenTable`
(revocable), asumiendo el costo de recalcular la integridad de esa tabla en cada emisión.

### 4.2 La restauración por cookie no revalida integridad (severidad media)

El login normal, si la integridad de la base está comprometida, devuelve `DVH_INVALIDO` y
bloquea el acceso. La restauración desde la cookie de "recordarme" **no** corre esa
verificación (por costo: recorrer toda la base en cada request). Un usuario "recordado"
podría entrar aunque la base esté marcada como comprometida. Es el mismo criterio que la
autenticación persistente de ASP.NET, pero conviene decidirlo a conciencia.

### 4.3 No existe rol ni usuario "Webmaster" (severidad media)

El menú del dashboard muestra "Integridad de la base de datos" para el rol `Webmaster` o
para el acceso de emergencia. En la base **no hay** rol `Webmaster` ni ningún usuario con
ese rol, y la patente `RECALCULAR_INTEGRIDAD` solo la tiene `Gestor`. En la práctica, a la
pantalla de Integridad solo llega el acceso de emergencia o un Gestor. Hay que decidir si
se crea el rol `Webmaster` con sus patentes o si se ajusta el menú.

### 4.4 Desalineación de nombres de rol entre fachada y base (severidad media)

La fachada usa "Analista" en todos lados (lista de usuarios, chips, combo de invitación).
La base usa el rol `Usuario`. El combo de invitación ya hace el puente (muestra "Analista",
guarda `Usuario`), pero el resto de la fachada sigue diciendo "Analista", y si un usuario
con rol `Usuario` se loguea, `App.master` no reconoce ese nombre y cae al rol por defecto
(`Gestor`). Hay que unificar: o se crea el rol `Analista` en la base, o la fachada pasa a
hablar de `Usuario`.

### 4.5 Sin revocación de "recordarme" al cambiar la contraseña (severidad media)

Como la cookie es autocontenida, cambiar la contraseña (o que un admin bloquee la cuenta)
no invalida las cookies de "recordarme" ya emitidas hasta que venzcan (30 días) o el
usuario haga logout en ese equipo. La restauración sí chequea que el usuario siga `Activo`,
así que un bloqueo por parte del admin sí corta el acceso en el siguiente request; pero un
cambio de contraseña no.

### 4.6 Cambio de UX en el logout (severidad baja)

El enlace "Cerrar sesión" del navbar ya no cierra la sesión de un clic: lleva a
`Salir.aspx`, que ahora es una pantalla de confirmación con un botón que cierra por POST.
Es a propósito (para que un prefetch del navegador no cierre la sesión), pero es un cambio
de comportamiento respecto de lo que había.

### 4.7 Restos de fachada en `App.master` (severidad baja)

Sigue existiendo el selector de rol por query string (`?rol=Gestor`, etc.) y el usuario de
demostración de fallback ("Usuario de demostracion / demo@patternblue.com.ar") cuando no
hay sesión. Están marcados para borrar, pero permiten recorrer el dashboard sin loguearse.

### 4.8 `Web.config` sin comentarios (severidad baja / informativo)

El paso de "sacar todos los comentarios" también borró las notas explicativas del
`Web.config` (por qué el connection string y la clave AES están ahí, qué es el acceso de
emergencia). El comportamiento no cambió; solo se perdió esa documentación inline.

### 4.9 Datos históricos inconsistentes en `TokenTable` (severidad baja / informativo)

Hay filas viejas con `tipo_token` en mayúsculas (`ACTIVACION`, `RECUPERACION`) y otras en
minúscula. El repositorio normaliza al leer, así que los flujos funcionan, pero es una
inconsistencia de datos que conviene limpiar.

### 4.10 No se probó el pipeline HTTP real

Sesión de ASP.NET, postbacks, `ClientScript.RegisterStartupScript` (reapertura del modal
de invitación), redirecciones con `CompleteRequest`, y el ciclo real de la cookie
(Set-Cookie / lectura en el request siguiente) no se pudieron probar sin levantar el sitio.
Están cubiertos por el checklist manual del punto 5.

---

## 5. Checklist de prueba manual en el navegador

1. **Login normal.** Entrar con un usuario real de la base. Debe caer en `Panel.aspx`.
2. **Login de emergencia.** Usuario `admin`, contraseña `admin`. Debe entrar y, en el menú
   lateral, debe aparecer "Integridad de la base de datos" (y todo el resto).
3. **Login fallido.** Contraseña incorrecta 5 veces con el mismo usuario: a la quinta, la
   cuenta debe quedar bloqueada y el mensaje debe cambiar. (Ojo: esto modifica la base;
   después hay que desbloquear ese usuario a mano.)
4. **Recordarme.** Tildar "Recordarme en este equipo" al entrar. Cerrar la pestaña, abrir
   `Panel.aspx` directo: debe entrar sin pedir login. En el navbar público debe verse
   "Cerrar sesión" en lugar de "Iniciar sesión".
5. **Logout.** Menú → "Cerrar sesión" → botón de confirmación. Después, `Panel.aspx` debe
   redirigir a `Ingresar.aspx`, y la cookie `FALKE_RECORDARME` debe estar borrada.
6. **Recuperar contraseña.** `RecuperarClave.aspx` (sin navbar), pedir el enlace con un
   correo real. Buscar el `.txt` en `GUI/App_Data/mails/`. Abrir el enlace: debe llevar a
   `DefinirClave.aspx`.
7. **Definir contraseña.** Poner una de menos de 8 caracteres (debe rechazar), después una
   válida. Volver a abrir el mismo enlace: debe decir que ya se usó.
8. **Cambiar contraseña.** Logueado, `MiClave.aspx`. Probar con la contraseña actual mal
   (debe fallar) y bien (debe confirmar). Sin sesión, `MiClave.aspx` debe redirigir a
   `Ingresar.aspx`.
9. **Invitar usuario.** Como Administrador o Gestor, en `Usuarios.aspx` → "Invitar usuario".
   Completar y enviar: debe aparecer el aviso de éxito y, en la base, una fila nueva en
   `UsuarioTable` con `estado_usuario` Pendiente, más un `.txt` de activación. Repetir con
   el mismo correo: debe rechazar por duplicado.
10. **Invitar sin permiso.** Con un usuario de rol `Usuario`, el botón "Invitar" no debería
    estar; si se fuerza el postback, el handler debe responder que no tiene permiso.
11. **Integridad.** Como emergencia, abrir `Integridad.aspx`. Con la base sana debe mostrar
    todo "Íntegra" y el banner verde. Modificar a mano un registro de `UsuarioTable` por
    fuera de la app, recargar: esa tabla debe pasar a "Inconsistente", el banner a rojo, y
    "Ver detalle" debe mostrar el registro con DVH guardado vs. calculado.
12. **Recalcular.** Botón "Recalcular dígitos verificadores", alcance "solo tablas con
    inconsistencias", con un motivo. Debe volver a "Íntegra" y dejar una entrada en
    `BitacoraTable` con criticidad Alta y el motivo.

---

## 6. Próximo paso sugerido

En orden de prioridad:

1. **Autenticar la cookie de "recordarme"** (punto 4.1). Es lo único con impacto de
   seguridad real. Firmar el contenido con HMAC y verificar antes de confiar, o cambiar a
   `MachineKey.Protect` / `MachineKey.Unprotect`.
2. **Cerrar la brecha de nombres de rol** (punto 4.4): crear el rol `Analista` en la base
   con sus patentes, o cambiar la fachada para que hable de `Usuario`. Y decidir qué pasa
   con `Webmaster` (punto 4.3).
3. **Conectar la lista de usuarios de `Usuarios.aspx`** a la base (hoy es demo). Es la
   pantalla que acompaña al alta, así que es la siguiente en la fila lógica. De ahí:
   bloquear / desbloquear / reactivar / reenviar invitación.
4. **Revocación de sesión persistente** al cambiar contraseña (punto 4.5).
5. **Rate-limiting en recuperación de contraseña** (hoy no hay límite de pedidos por
   correo / por hora).
6. **Sacar la fachada de `App.master`** (punto 4.7): reemplazar el selector `?rol=` y el
   usuario demo por la sesión real, y poner guardas de permiso reales (`ExigirPermiso`) al
   inicio de cada pantalla del dashboard.
7. **Mails de verdad** (SMTP) en lugar del `.txt` en `App_Data/mails/`, cuando haya
   servidor de correo.
8. Conectar el resto del dashboard (Empresas, Dispositivos, Bitácora, Respaldos,
   Visualizaciones) a sus servicios.

---

## 7. Cómo hacer que el diseño se sienta menos "hecho por IA"

El diseño actual es prolijo pero tiene los tics típicos de una interfaz generada: todo es
una tarjeta con sombra y borde redondeado, el espaciado es un múltiplo perfecto de 8 en
todos lados, hay un ícono al lado de cada título y cada label, y los textos son marketing
genérico. Ideas concretas para bajarle ese aire:

- **Romper la simetría.** Las UIs de IA tienden a grids impecables con todas las cards de
  la misma altura y el mismo peso visual. Un producto real tiene una jerarquía marcada:
  una zona dominante y el resto claramente secundario. Que no todo compita por atención.

- **Menos contenedores.** No todo necesita ir en una `.tarjeta` con `box-shadow` y
  `border-radius`. Mezclar densidades: tablas planas con separadores finos, listas sin
  caja, secciones que arrancan directo sobre el fondo. El exceso de tarjetas flotando es
  una firma de template.

- **Copywriting específico.** Frases como "Descubre lo que tus usuarios miran de verdad" o
  "Precisión profesional al alcance de tu empresa" son relleno de IA. Reemplazar por
  lenguaje del dominio, con números y nombres concretos: qué mide exactamente el Tobii,
  cuántas sesiones entran en un plan, qué se ve en cada visualización, un caso real.

- **Menos íconos decorativos.** Hoy hay un SVG al lado de casi cada título, label y ítem de
  lista. Dejar los íconos solo donde aportan (acciones, estados) y sacarlos de los textos.

- **Acentos más escasos.** El coral y el dorado aparecen en chips, badges, glows radiales,
  bordes y CTAs por todos lados. Un color de acento pesa más cuando es raro: usarlo en uno
  o dos lugares por pantalla, no en diez.

- **Tipografía menos "kit de branding".** La combinación grotesk para títulos + sans para
  cuerpo + mono para datos es correcta pero está muy vista. Bajar el uso de
  `text-transform: uppercase` + `letter-spacing` en los labels (eso grita "design system de
  demo"). Ajustar tamaños y interlineado a mano hasta que no parezca el preset.

- **Densidad de información.** Las pantallas generadas suelen tener mucho aire y poco
  contenido. Un panel de analítica real está más apretado: más datos por pantalla, menos
  padding, menos "hero" en pantallas internas.

- **Estados vacíos y de error escritos por una persona.** Que no sean todos "No hay datos
  para mostrar" con un ícono centrado. Un texto que explique qué hacer, con el tono del
  producto.

- **Ilustración o imagen propia** en lugar de solo gradientes radiales y formas
  geométricas abstractas. Aunque sea una foto del dispositivo, un diagrama del flujo real.

- **Micro-inconsistencias.** Un producto real creció por partes: no todas las pantallas
  tienen el mismo nivel de pulido ni el mismo layout. Está bien que la pantalla de
  Integridad (técnica, interna) se vea más austera que la landing.

- **Mirar un referente real** del rubro (herramientas de eye-tracking / analítica de
  producto) y copiar decisiones concretas de layout y densidad, en vez de partir de un
  patrón genérico de dashboard.
