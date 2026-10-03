# FALKE — Cómo darle funcionalidad a la fachada

Guía pantalla por pantalla: qué hay que hacer para que cada una funcione de verdad, con
qué capa se conecta, qué datos necesita y qué controles hay que reemplazar.

Complementa a `FACHADA-GUI.md`, que explica qué es cada pantalla y cómo está armada.

> **Lo que ya funciona:** `Ingresar.aspx` (login) y `Salir.aspx` (logout). Todo lo demás
> es fachada.

---

## 0. Antes de empezar: lo transversal

Estas cinco cosas afectan a casi todas las pantallas. Conviene resolverlas primero, una
sola vez, y después ir pantalla por pantalla.

### 0.1 Destapar el control de sesión

Cada code-behind del dashboard tiene esta línea comentada:

```csharp
// if (!SesionActual_GUI.Exigir()) return;
```

o, en las pantallas con permiso específico:

```csharp
// if (!SesionActual_GUI.ExigirPermiso("EMPRESAS_VER")) return;
```

Hay que descomentarlas todas y poner el nombre real de la patente. `SesionActual_GUI` ya
tiene `Exigir()`, `ExigirPermiso(patente)`, `Puede(patente)` y `RedirigirSiAutenticado()`.

Las patentes que usa la fachada como nombre tentativo son `EMPRESAS_VER`,
`DISPOSITIVOS_VER`, `RESPALDOS_GESTIONAR`, `BITACORA_VER` e `INTEGRIDAD_VER`. Si en la
base se llaman distinto, hay que cambiar el string.

### 0.2 Borrar el selector de rol de prueba

En `App.master` hay un bloque marcado con `//TODO: BORRAR ESTE BLOQUE` que permite
cambiar de rol con `?rol=Analista` y guarda el resultado en `Session["RolVistaPrevia"]`.
Existe solo para poder revisar el menú de los cuatro roles sin cargar usuarios.

Hay que borrar:

- El bloque de chips de rol del `App.master`.
- El método `ResolverRol()` de `App.master.cs` queda reducido a leer
  `SesionActual_GUI.Rol`.
- La línea `Session.Remove("RolVistaPrevia")` de `Ingresar.aspx.cs`.

### 0.3 Llenar los datos del usuario en la topbar

`App.master.cs` tiene los literales `litNombre`, `litEmail`, `litRol`, `litIniciales` y
`litIniciales2`, que hoy caen en un usuario de ejemplo cuando no hay sesión. Al conectar
salen de `SesionActual_GUI`.

El contador de avisos sin leer está fijo en la constante `AVISOS_SIN_LEER = 3`; tiene que
salir de la tabla de notificaciones (ver 0.5). Cuando es 0 hay que ocultar el
`phPuntoAviso` (el círculo coral de la campana).

### 0.4 El idioma

El usuario ya tiene `IdIdioma` en `Usuario_TE`. Falta:

- Una tabla `Idioma` (id, código ISO, nombre) y una tabla `Traduccion` (id idioma, clave,
  texto).
- Un método que devuelva el diccionario completo de un idioma como
  `Dictionary<string, string>`.
- En las master pages, serializar ese diccionario a JSON y llamar a
  `Falke.traducir(diccionario)` al final de la página.

Las claves ya están puestas en el HTML como `data-i18n="pantalla.bloque.elemento"`, así
que se puede sacar la lista de claves con un `grep` sobre la carpeta GUI y precargar la
tabla de traducciones con eso.

Los textos que se arman en JavaScript están marcados con `//TODO traducir` y hay que
pasarlos también al diccionario.

### 0.5 Notificaciones

No existe todavía. Hace falta:

**Tabla `Notificacion`:** `IdNotificacion` (int), `IdUsuario` (int), `Tipo` (string o
enum: `Sistema` / `Empresa`), `Asunto` (string), `Texto` (string), `FechaHora`
(DateTime), `Leida` (bool).

**Clase `Notificacion_TLL`** con:

```csharp
List<Notificacion_TE> ObtenerDelUsuario(int idUsuario);
int ContarSinLeer(int idUsuario);
void MarcarLeidas(int idUsuario, List<int> ids);
void CrearParaTodos(string tipo, string asunto, string texto);  // avisos de mantenimiento
void CrearParaUsuario(int idUsuario, string tipo, string asunto, string texto);
```

Es transversal porque la alimentan tres pantallas distintas: Respaldos (aviso de
mantenimiento), el servicio de análisis (sesión procesada) y Usuarios (invitaciones).

---

## 1. Pantallas públicas

### 1.1 `Default.aspx` — Inicio

**Qué falta:** nada funcional. Es contenido estático.

**Opcional:** si algún día los planes cambian de precio, la franja de comparación y los
textos que mencionan importes deberían salir de la misma fuente que `Planes.aspx` para no
quedar desfasados.

---

### 1.2 `Planes.aspx` — Planes y contratación

**Qué falta:** que el formulario de contacto mande el mensaje.

**Con qué se conecta:** `Email_SERVICE.Enviar(destino, asunto, cuerpo)`, que ya existe.

**Datos del formulario:**

| Campo | Control actual | Tipo | Obligatorio |
|---|---|---|---|
| Plan elegido | `ctPlan` (hidden) | `PlanSuscripcion` | sí |
| Nombre | `ctNombre` | `string` | sí |
| Apellido | `ctApellido` | `string` | sí |
| Empresa | `ctEmpresa` | `string` | sí |
| Email | `ctEmail` | `string` | sí |
| Teléfono | `ctTelefono` | `string` | sí |
| Mensaje | `ctMensaje` (textarea) | `string` | sí, viene prellenado y es editable |
| Consentimiento | `ctConsentimiento` | `bool` | sí |

**Qué hacer:**

1. Pasar los `<input>` a `<asp:TextBox>` y el `<textarea>` a
   `<asp:TextBox TextMode="MultiLine">`.
2. Validar en el servidor: los campos obligatorios, el formato del email y que el
   consentimiento esté tildado. El JavaScript actual valida en el navegador y eso se
   puede saltear.
3. Mandar el mail a la casilla comercial de Pattern Blue con
   `Email_SERVICE.Enviar(...)`.
4. Conviene además guardar el pedido en una tabla `ConsultaComercial` (nombre, apellido,
   empresa, email, teléfono, plan, mensaje, fecha, estado) para no depender del correo.
5. Poner un captcha o un límite de envíos por IP: es un formulario público.

**Ojo:** los precios están escritos en el HTML como `data-precio-mensual` y
`data-precio-anual`. Si van a cambiar seguido, conviene una tabla `Plan` (nombre, precio
mensual, precio anual, límite de usuarios, dispositivos incluidos, características).

---

### 1.3 `Faq.aspx` — Preguntas frecuentes

**Qué falta:** nada, salvo que se quieran editar las preguntas sin tocar el HTML.

**Si se quiere administrar:** tabla `PreguntaFrecuente` (id, grupo, pregunta, respuesta,
orden, activa) y un Repeater. En ese caso, el buscador del JavaScript sigue sirviendo
porque trabaja sobre lo ya renderizado.

---

### 1.4 `Ingresar.aspx` — Iniciar sesión ✅ FUNCIONA

Ya llama a `Usuario_TLL.ValidarCredenciales(email, contrasena)` y traduce los cuatro
motivos de `ResultadoLogin_TLL` (`CREDENCIALES_INVALIDAS`, `USUARIO_BLOQUEADO`,
`USUARIO_PENDIENTE`, `DVH_INVALIDO`) a un texto entendible.

**Lo que queda pendiente:**

- La casilla "Recordarme" no hace nada. Para que funcione hace falta una cookie
  persistente con un token propio (no la contraseña), con vencimiento y revocable.
- Borrar la línea `Session.Remove("RolVistaPrevia")` cuando se saque el selector de rol
  (ver 0.2).
- Registrar el inicio de sesión en la bitácora, como ya hace `Salir.aspx` con el cierre.

---

### 1.5 `Salir.aspx` — Cerrar sesión ✅ FUNCIONA

Registra el cierre en la bitácora y llama a `SesionActual_GUI.Cerrar()`.

---

### 1.6 `Registro.aspx` — Cómo obtener una cuenta

**Qué falta:** nada. Es informativa a propósito: en FALKE los usuarios no se registran
solos, los da de alta Pattern Blue (la empresa y su administrador) y después el
administrador invita a su equipo.

---

### 1.7 `RecuperarClave.aspx` — Recuperar contraseña

**Con qué se conecta:** `Usuario_TLL.SolicitarRecuperacion(email)`, que ya existe y
devuelve el token.

**Qué hacer:**

1. Pasar el campo a `<asp:TextBox>` y el botón a `<asp:Button>`.
2. Llamar a `SolicitarRecuperacion(email)` y mandar el mail con `Email_SERVICE.Enviar()`,
   incluyendo el enlace a la pantalla donde se define la contraseña nueva con el token en
   la query string.
3. **Mostrar siempre el mismo mensaje**, exista o no la cuenta. Si el mensaje cambia, se
   convierte en una forma de averiguar qué emails están registrados.
4. Limitar los pedidos por email y por hora.
5. Registrar el pedido en la bitácora con criticidad `Media`.

**Nota:** `Usuario_TLL` ya tiene `ValidarTokenContrasena(token)` y
`EstablecerContrasenaConToken(token, contrasenaNueva)`, con la constante
`TOKEN_RECUPERACION`. La pantalla donde la persona escribe la contraseña nueva a partir
del enlace todavía no está hecha en la fachada nueva (existe la vieja
`EstablecerContrasena.aspx` de pruebas); conviene rehacerla con `Publico.master` y el
mismo diseño que `MiClave.aspx`.

---

## 2. Dashboard — Análisis

### 2.1 `Panel.aspx` — Página principal

**Qué falta:** todos los números.

| Bloque | De dónde sale | Tipo |
|---|---|---|
| Saludo | `SesionActual_GUI.Nombre` (ya está resuelto) | `string` |
| Sesiones del mes | contar `SesionGrabada` de la empresa en el mes | `int` |
| Sesiones en procesamiento | contar por estado | `int` |
| Categorías activas | contar `Categoria` activas de la empresa | `int` |
| Dispositivos asignados | contar préstamos abiertos | `int` |
| Últimas sesiones | las 5 últimas de la empresa | `List<SesionGrabada_BE>` |
| Estado del dispositivo | último chequeo informado por el SDK | fecha + porcentaje |
| Actividad reciente | últimas entradas de bitácora del usuario | `List<Bitacora_TE>` |

**Qué hacer:** conviene una sola consulta que devuelva las cuatro métricas juntas en un
DTO, en vez de cuatro viajes a la base. La tabla de últimas sesiones pasa a Repeater.

---

### 2.2 `Visualizaciones.aspx` — Visualizaciones

Es la pantalla más grande y la que más necesita del módulo de análisis.

**Riel de categorías:** Repeater sobre `Categoria_BLL.ObtenerPorEmpresa(idEmpresa)`.
Necesita id, nombre y tipo de activo. El botón "Nueva categoría" lleva a
`Categorias.aspx`.

**Listado de sesiones:** `SesionGrabada_BLL.Buscar(idEmpresa, idCategoria, desde, hasta,
estado)`. Por sesión hacen falta:

| Dato | Tipo |
|---|---|
| Id | `int` |
| Nombre | `string` |
| Fecha y hora | `DateTime` |
| Duración | `int` (segundos) o `TimeSpan` |
| Estado | enum: `Procesando`, `Finalizada`, `ConError` |
| Tester | `string` (código, nunca el nombre real) |
| Categoría | `int` + nombre |

Los botones grandes de sesión ya tienen `data-categoria`, `data-estado`, `data-sesion`,
`data-fecha` y `data-duracion`; hay que llenarlos desde el Repeater. El botón de una
sesión en proceso va `disabled`.

**Métricas del análisis:** las calcula el módulo de análisis a partir de las muestras de
mirada. Conviene traerlas ya resumidas en un DTO — no tiene sentido mandar miles de puntos
al navegador para que los promedie el JavaScript:

| Métrica | Tipo |
|---|---|
| Tiempo total de atención | `TimeSpan` |
| Tiempo hasta la primera fijación | `decimal` (segundos) |
| Cantidad de fijaciones | `int` |
| Porcentaje de atención vs distracción | `decimal` |
| Zonas de interés detectadas | `int` |
| Zonas ciegas detectadas | `int` |

**Los siete análisis.** Cada tarjeta abre el reproductor. Lo que necesita cada uno:

| Análisis | Qué le hace falta |
|---|---|
| Ver grabación | La URL del video, servida por un handler que valide permisos (nunca la ruta física). |
| Mapa de calor | Una imagen generada por el servidor, o la lista de puntos con su peso para dibujarla. |
| Zonas de interés y ciegas | Lista de rectángulos con coordenadas, tamaño y tiempo de permanencia. |
| Dispersión de mirada | Lista de puntos (x, y, timestamp). |
| Atención vs distracción | Dos porcentajes y la serie en el tiempo. |
| Recorrido de mirada | Lista ordenada de fijaciones con su duración, para dibujar la línea. |
| Comparar | Lleva a `AnalisisMultiple.aspx`. |

Los SVG que hay ahora son dibujos de ejemplo: se reemplazan por lo que devuelva el
servicio.

**Reproductor:** los controles ya están (play, pausa, adelante, atrás, línea de tiempo,
tiempo transcurrido, leyenda). Falta engancharlos a un `<video>` real y sincronizar la
capa de análisis con el tiempo del video. `DURACION_DEMO = 1122` es un valor fijo de la
fachada y hay que sacarlo.

**Exportar reporte:** el modal tiene las casillas `name="expVisual"`. Al confirmar hay que
generar el PDF con lo tildado y **registrar la exportación en la bitácora**: son datos
biométricos, la Ley N° 25.326 pide poder rastrear quién los sacó del sistema.

---

### 2.3 `NuevaGrabacion.aspx` — Nueva grabación

**Paso 1 — datos de la sesión.**

| Campo | Tipo | Nota |
|---|---|---|
| Categoría | `int` | combo desde `Categoria_BLL.ObtenerPorEmpresa()` |
| Nombre de la sesión | `string` | |
| Tester | `string` | **código, nunca el nombre real** |
| Analista | `int` | el usuario logueado, de solo lectura |
| Objetivo | `string` | texto largo |

El tester va codificado por la Ley N° 25.326: la mirada es un dato biométrico y tiene que
quedar disociado de la identidad de la persona.

Se guarda con `SesionGrabada_BLL.Crear(...)`, que devuelve el id del borrador.

**Paso 2 — calibración del dispositivo.** Los tres chequeos hoy los simula el JavaScript.
En la versión real los informa el componente local que habla con el SDK de Tobii:

| Chequeo | Dato que necesita |
|---|---|
| Conexión USB | `bool` conectado + modelo y número de serie |
| Calibración | `DateTime?` de la última calibración + si sigue vigente |
| Driver | `bool` instalado + `string` versión instalada |

La **versión mínima requerida** hoy está escrita en el HTML (`4.0+`); tiene que salir de
configuración, porque cambia con las actualizaciones.

**Hay que borrar el botón `#btnSimularFallo`:** existe solo para poder ver el estado de
error de la fachada.

**Paso 3 — captura.** La captura de pantalla y el flujo de mirada los maneja el cliente
local, no el navegador. La web solo arranca, pausa y cierra la sesión, y avisa cuando el
archivo terminó de subir. Al finalizar: `SesionGrabada_BLL.Finalizar(idSesion)` y registro
en bitácora.

---

### 2.4 `Categorias.aspx` — Categorías

**Listado:** Repeater sobre `Categoria_BLL.ObtenerPorEmpresa(idEmpresa)`.

| Dato | Tipo |
|---|---|
| Id | `int` |
| Nombre | `string` |
| Tipo de activo | enum: `Software`, `AppWeb`, `AppMovil`, `Videojuego`, `Publicidad` |
| Cantidad de sesiones | `int` |
| Fecha de creación | `DateTime` |
| Activa | `bool` |
| Flujo esperado | `string` (texto largo) |

**Los campos que dependen del tipo.** El formulario muestra un bloque distinto según el
tipo elegido (url, plataforma, motor, medio, etc.). Guardar una columna por tipo es
incómodo: conviene una tabla `AtributoCategoria` (id categoría, clave, valor) o una
columna JSON.

**Reglas:**

- Solo el administrador de la empresa da de alta o edita; el analista ve el listado.
  Validar con `SesionActual_GUI.Puede(patente)`, no solo ocultando el botón.
- Una categoría con sesiones cargadas **no se borra, se desactiva**.
- Cada alta, edición y baja lógica va a la bitácora.

---

### 2.5 `AnalisisMultiple.aspx` — Análisis múltiple

**Listado:** `SesionGrabada_BLL.ObtenerFinalizadas(idEmpresa)`. Por sesión: id, nombre,
categoría, tipo de activo, fecha y duración.

**Generar:** pasarle la lista de ids al servicio de análisis, que devuelve las métricas
promediadas y los gráficos combinados.

**Validar en el servidor** (el JavaScript ya lo hace en pantalla, pero se puede saltear):

- Mínimo dos sesiones.
- Todas del mismo tipo de activo. Es la regla que fija el TFI: comparar el recorrido de
  mirada de un videojuego con el de una pieza publicitaria no dice nada.
- Todas de la empresa del usuario.

El reporte comparativo se exporta igual que en Visualizaciones y también queda en la
bitácora.

---

## 3. Dashboard — Organización

### 3.1 `Usuarios.aspx` — Usuarios

**Ficha de la empresa:** `Empresa_BLL.ObtenerPorId(SesionActual_GUI.IdEmpresa)` devuelve
`Empresa_BE` con `NombreEmpresa`, `PlanSuscripcion`, `Estado` y `FechaAlta`. Faltan en la
entidad el rubro y el domicilio, que hoy están escritos en el HTML.

**Listado:** `Usuario_TLL.ObtenerPorEmpresa(idEmpresa)` devuelve `List<Usuario_TE>`:

| Campo de `Usuario_TE` | Se muestra como |
|---|---|
| `IdUsuario` | clave de la fila |
| `NombreUsuario` + `ApellidoUsuario` | nombre completo e iniciales del avatar |
| `EmailUsuario` | columna Email |
| `Rol` (`PermisoCompuesto_TE`) | columna Rol |
| `Estado` (`EstadoUsuario`: `Pendiente`, `Activo`, `Bloqueado`, `Inactivo`) | badge de estado |
| `IdIdioma` | idioma en el detalle |

**Falta en la entidad:** la fecha del último acceso. Hoy la columna está en el HTML; hay
que agregar el campo o sacarlo de la última entrada de bitácora del usuario.

**Mi perfil.** Se guarda con `Usuario_TLL.ActualizarDatosUsuario(usuario)`. Solo viajan
**nombre, apellido e idioma**. El email **no se modifica nunca**: es la identidad de
acceso y está ligado a la invitación. El campo está `readonly` en la pantalla, pero el
servidor tiene que ignorarlo aunque llegue modificado.

**Invitar usuario.** `Usuario_TLL.RegistrarUsuario(usuario)` ya existe y devuelve el
token (constante `TOKEN_ACTIVACION`). El flujo es:

1. Crear el usuario con `Estado = EstadoUsuario.Pendiente` y sin contraseña.
2. Mandar el mail con `Email_SERVICE.Enviar()` incluyendo el enlace con el token.
3. La persona entra al enlace y define su contraseña con
   `EstablecerContrasenaConToken(token, contrasenaNueva)`.

**El administrador nunca define la contraseña de otro.**

**Cambios de estado.** Falta un método `CambiarEstado(idUsuario, EstadoUsuario)` en
`Usuario_TLL`. Cada cambio va a la bitácora con criticidad `Alta`.

**Permisos.** Hoy la pantalla compara `Master.RolActual == "Analista"` para decidir qué
mostrar. Al conectar conviene reemplazarlo por `SesionActual_GUI.Puede(patente)`, que es
lo que corresponde con el sistema de permisos compuestos que ya existe en `Permiso_TLL`.

---

### 3.2 `MiClave.aspx` — Cambiar mi contraseña

**Con qué se conecta:** `Usuario_TLL.CambiarContrasena(email, contrasenaActual,
contrasenaNueva, out string error)`, que **ya existe** y devuelve `bool`.

**Qué hacer:**

1. Pasar los tres campos a `<asp:TextBox TextMode="Password">` y el botón a
   `<asp:Button>`.
2. Llamar al método con `SesionActual_GUI.Email` y mostrar el `error` que devuelve si da
   `false`.
3. **Volver a validar los requisitos en el servidor.** La barra de fuerza y la lista de
   requisitos del JavaScript son una ayuda visual, no una validación.
4. Invalidar las otras sesiones del usuario (la pantalla ya avisa que eso va a pasar).
5. Registrar el cambio en la bitácora con criticidad `Alta`.

Los requisitos que valida la fachada (8 caracteres, mayúscula, minúscula, número) son los
que están escritos en el JavaScript; si la política real del sistema es otra, hay que
alinear las dos.

---

### 3.3 `Notificaciones.aspx` — Avisos

Ver el punto **0.5**, que define la tabla y el TLL.

**Listado:** Repeater sobre `Notificacion_TLL.ObtenerDelUsuario(idUsuario)`.

**Marcar como leídas:** hoy el botón solo saca el resaltado en pantalla; tiene que llamar
a `MarcarLeidas()` y refrescar el contador de la campana.

---

### 3.4 `Empresas.aspx` — Empresas cliente (Gestor)

**Listado:** `Empresa_BLL.ObtenerTodas()` devuelve `List<Empresa_BE>`:

| Campo de `Empresa_BE` | Se muestra como |
|---|---|
| `IdEmpresa` | clave de la tarjeta |
| `NombreEmpresa` | nombre y sigla |
| `PlanSuscripcion` (`Scout`, `Hunter`, `Apex`) | badge del plan |
| `Estado` (`Activa`, `Bloqueada`, `Deshabilitada`) | badge de estado |
| `FechaAlta` | "Cliente desde..." |
| `NumContactoEmpresa` | teléfono del detalle |

**Falta en la entidad**, y hoy está escrito en el HTML: CUIT, rubro, domicilio, contacto
comercial (nombre y email), tipo de facturación (mensual/anual), fecha de renovación y
límite de usuarios. Son campos nuevos de `Empresa_BE` o de una tabla `ContratoEmpresa`
aparte, que probablemente sea más prolijo porque el contrato cambia con el tiempo y la
empresa no.

Los contadores de las tarjetas (usuarios, dispositivos, sesiones) son consultas agregadas;
conviene un DTO de listado que las traiga junto con la empresa y no una consulta por
tarjeta.

**Alta de empresa.** `Empresa_BLL.RegistrarEmpresa(Empresa_BE empresa, Usuario_TE
adminInicial)` **ya existe y hace exactamente lo que pide la pantalla**: crea la empresa y
su primer administrador. Solo falta armar las dos entidades con lo que carga el formulario
y llamarlo.

Es importante que las dos altas queden **en la misma transacción**: una empresa sin
administrador no puede empezar a trabajar y nadie podría entrar a arreglarla. El proyecto
ya tiene transacciones centralizadas, así que alcanza con envolver la llamada.

**Bloquear o deshabilitar.** Falta `Empresa_BLL.CambiarEstado(idEmpresa, EstadoEmpresa)`.
Bloquear una empresa deja afuera a todos sus usuarios de golpe, así que conviene pedir
confirmación escrita como hace la restauración de respaldos.

**Todo lo de esta pantalla va a la bitácora con criticidad `Alta`.**

---

### 3.5 `Dispositivos.aspx` — Dispositivos (Gestor)

No existe la capa todavía. Hace falta:

**Tabla y entidad `Dispositivo_BE`:**

| Campo | Tipo |
|---|---|
| `IdDispositivo` | `int` |
| `NumeroSerie` | `string` (único) |
| `Modelo` | `string` |
| `Firmware` | `string` |
| `Estado` | enum: `Disponible`, `EnUso`, `EnMantenimiento`, `DeBaja` |
| `FechaCompra` | `DateTime` |
| `UltimaCalibracion` | `DateTime?` |
| `DVH` | `string` |

**Tabla y entidad `Prestamo_BE`:**

| Campo | Tipo |
|---|---|
| `IdPrestamo` | `int` |
| `IdDispositivo` | `int` |
| `IdEmpresa` | `int` |
| `FechaEntrega` | `DateTime` |
| `FechaDevolucion` | `DateTime?` (null = préstamo abierto) |
| `Observaciones` | `string` |

**Clase `Dispositivo_BLL`:**

```csharp
List<Dispositivo_BE> ObtenerTodos();
Dispositivo_BE ObtenerPorId(int id);
List<Prestamo_BE> ObtenerHistorial(int idDispositivo);
void RegistrarPrestamo(int idDispositivo, int idEmpresa, DateTime fecha, string observaciones);
void RegistrarDevolucion(int idPrestamo, DateTime fecha);
void CambiarEstado(int idDispositivo, EstadoDispositivo estado);
void Crear(Dispositivo_BE dispositivo);
```

**Reglas:**

- Solo se puede asignar un dispositivo que esté `Disponible`, y solo a una empresa
  `Activa`.
- El número de serie no se puede repetir.
- Registrar la devolución cierra el préstamo abierto y devuelve el dispositivo a
  `Disponible`.
- Todos los movimientos van a la bitácora: son bienes prestados.

La cantidad de dispositivos que muestra `Empresas.aspx` sale de contar los préstamos
abiertos de cada empresa.

---

## 4. Dashboard — Sistema

### 4.1 `Respaldos.aspx` — Respaldos y mantenimiento (Gestor)

Es la pantalla más delicada de la suite. Tiene dos mitades independientes.

#### Copias de seguridad

No existe la capa. Hace falta un `Respaldo_SERVICE` con:

```csharp
List<Respaldo> ObtenerTodos();
void Generar(string descripcion, bool verificar);
void Restaurar(int idRespaldo);
```

**Datos de un respaldo:**

| Campo | Tipo |
|---|---|
| `IdRespaldo` | `int` |
| `FechaHora` | `DateTime` |
| `NombreArchivo` | `string` |
| `TamanoBytes` | `long` |
| `EsAutomatico` | `bool` |
| `Descripcion` | `string` |
| `Verificado` | `bool` |

**Generar** tarda minutos: conviene hacerlo en segundo plano y mostrar el avance, no
bloquear la petición.

**Restaurar** tiene un orden que hay que respetar:

1. Poner el sistema en mantenimiento.
2. Cerrar todas las sesiones abiertas.
3. Restaurar la base.
4. **Recalcular DVH y DVV** con `GestorIntegridad_SERVICE.RecalcularTodasLasTablas()`.
5. Cerrar la sesión del propio gestor y mandarlo a `Mantenimiento.aspx`.

El paso 4 no es opcional: después de una restauración los dígitos verificadores
corresponden a los datos del respaldo, y si algo quedó a medio camino el sistema tiene que
detectarlo.

La palabra "RESTAURAR" que se pide escribir es una barrera de la GUI para evitar el clic
distraído; el servidor igual tiene que validar el permiso y confirmar la operación por su
cuenta.

#### Mantenimiento del sistema

**Estado del sistema:** guardarlo en configuración (`bool EnMantenimiento`, `DateTime?
InicioProgramado`, `DateTime? FinEstimado`, `string Motivo`).

**La redirección no se hace desde esta pantalla.** Conviene un módulo o `Global.asax` que,
en cada petición, mande a `Mantenimiento.aspx` a todo el que no sea usuario de Pattern
Blue mientras el sistema esté bloqueado. Esa lógica tiene que dejar pasar `Content/` y
`Scripts/`, si no la pantalla de mantenimiento se ve sin estilos.

**El aviso a los usuarios.** Al programar la ventana hay que llamar a
`Notificacion_TLL.CrearParaTodos(...)` con el texto que muestra la vista previa:

> La página será puesta en mantenimiento en {tiempo}, guarde sus archivos antes de la hora
> informada.

El texto se arma hoy en el JavaScript de la pantalla y está marcado con `//TODO traducir`;
al conectar tiene que armarse en el servidor y guardarse ya traducido al idioma de cada
usuario, o guardar la clave y los parámetros y traducirlo al mostrarlo.

**Historial de ventanas:** tabla con inicio, duración real, motivo y responsable.

**Todo lo de esta pantalla va a la bitácora con criticidad `Alta`.**

---

### 4.2 `Bitacora.aspx` — Bitácora

**Con qué se conecta:** `BitacoraGestor_TLL`, que ya tiene `ObtenerTodas()` y
`Registrar(idUsuario, modulo, descripcion, criticidad)`.

**Lo que falta en el TLL:** un método de búsqueda con filtros y paginado. `ObtenerTodas()`
no sirve para esta pantalla: la bitácora crece muy rápido y no se puede traer entera.

```csharp
List<Bitacora_TE> Buscar(FiltroBitacora filtro, int pagina, int tamanoPagina);
int Contar(FiltroBitacora filtro);
```

**`FiltroBitacora` necesita:**

| Campo | Tipo | Viene del control |
|---|---|---|
| Desde | `DateTime?` | `bitDesde` |
| Hasta | `DateTime?` | `bitHasta` |
| Hora desde | `TimeSpan?` | `bitHoraDesde` |
| Hora hasta | `TimeSpan?` | `bitHoraHasta` |
| Módulo | `string` | `bitModulo` |
| Acción | `string` | `bitAccion` |
| Usuario | `string` (busca en nombre y email) | `bitUsuario` |
| Empresa | `int?` | `bitEmpresa` (solo gestor) |
| Criticidad | `CriticidadBitacora?` | `bitCriticidad` |

**El alcance según el rol se arma en el TLL, no en la página:**

| Rol | Qué ve |
|---|---|
| Administrador | `where IdEmpresa = el suyo` |
| Webmaster | `where ModuloBitacora in ('Sistema', 'Seguridad')` |
| Gestor | sin restricción, y con el filtro por empresa habilitado |

Esto es importante: hoy la fachada esconde filas con JavaScript, y eso **no es seguridad**.
Si las filas llegan al navegador, están al alcance de cualquiera que abra las herramientas
de desarrollo. El filtrado tiene que estar en el `WHERE`.

**Falta en `Bitacora_TE`:** la empresa. Hoy la entidad tiene `IdUsuario`, así que la
empresa sale por join con `Usuario`. Si se quiere filtrar cómodo conviene desnormalizar y
guardar también `IdEmpresa` en el registro, porque el usuario puede cambiar de empresa y
el registro histórico no debería cambiar con él.

**Otra cosa:** `IdUsuario` debería aceptar `null`. Hay eventos de sistema (un respaldo
automático, una tarea programada) que no los dispara ninguna persona.

**Exportar:** genera el CSV o el PDF de lo filtrado y registra la exportación en la propia
bitácora.

**La bitácora es de solo lectura.** No existe editar ni borrar, ni siquiera para el
gestor. Si existiera, dejaría de servir como evidencia.

---

### 4.3 `Integridad.aspx` — Dígito verificador (Webmaster)

Esta pantalla es la que más cerca está de poder conectarse: `GestorIntegridad_SERVICE` ya
tiene casi todo.

**Verificación completa:** `VerificarIntegridadTodasLasTablas()` devuelve
`List<InconsistenciaIntegridad_SERVICE>`, y cada elemento ya trae exactamente lo que la
pantalla muestra:

| Campo del servicio | Dónde se muestra |
|---|---|
| `Tabla` (`TablasBD`) | columna Tabla |
| `Tipo` (`TipoInconsistencia`) | columna Estado |
| `ClaveRegistro` | columna Id del modal de detalle |
| `Detalle` | descripción |
| `DvhAlmacenado` | columna "DVH guardado" |
| `DvhRecalculado` | columna "DVH calculado" |
| `Columnas` / `Datos` | para mostrar qué cambió |

**Verificación de una tabla:** `VerificarIntegridadTabla(TablasBD tabla)` alimenta el
modal de detalle.

**Recálculo:** `RecalcularTodasLasTablas()` devuelve
`ResultadoRecalculoIntegridad_SERVICE` con `TablasProcesadas`, `RegistrosProcesados` y la
lista de `DetalleTablaRecalculo_SERVICE` (tabla, registros, DVV). Para el alcance "solo
las tablas con problemas" hay que iterar `RecalcularTabla(tabla)` sobre las que fallaron.

**Lo que falta:**

- **Los conteos por tabla.** La pantalla muestra cuántos registros tiene cada tabla; hoy
  están escritos en el HTML. Hace falta un método que devuelva el total por tabla, o
  aprovechar el `Registros` de `DetalleTablaRecalculo_SERVICE`.
- **La fecha de la última verificación.** Guardarla en configuración cada vez que se
  corre.
- **Correr en segundo plano.** Recorrer toda la base tarda; la pantalla debería mostrar el
  último resultado guardado y ofrecer "verificar ahora" como una tarea que arranca y
  después avisa. El botón `#btnVerificarTodo` hoy solo simula la espera con un
  `setTimeout`.
- **El motivo del recálculo.** El campo está en el modal; hay que guardarlo en la bitácora
  junto con el evento, criticidad `Alta`.

**Un detalle importante que la pantalla ya advierte:** recalcular hace que el sistema
acepte como buenos los datos que hay ahora. Si alguien tocó la base por afuera,
recalcular borra la evidencia. Por eso el orden correcto es: revisar los registros
marcados, decidir si corresponde restaurar un respaldo, y recién después recalcular.

**Relación con el login:** `ValidarCredenciales` ya devuelve `DVH_INVALIDO` cuando el
usuario tiene el dígito mal. Esta pantalla es donde el webmaster investiga ese aviso.

---

### 4.4 `SinPermiso.aspx` — Acceso denegado

**Qué falta:**

- Descomentar `SesionActual_GUI.Exigir()`: si ni siquiera hay sesión, corresponde mandarlo
  al login y no acá.
- Registrar el intento en la bitácora con criticidad `Media`, guardando qué sección quiso
  abrir (llega en la query string o en el `Referer`).

---

### 4.5 `Mantenimiento.aspx` — Sistema en mantenimiento

**Qué falta:**

- Leer el estado y la hora estimada de la configuración; hoy el texto está en el HTML.
- Si el sistema **no** está en mantenimiento, esta pantalla no debería mostrarse:
  corresponde redirigir al panel o al login.
- La redirección hacia acá se resuelve en un módulo o en `Global.asax` (ver 4.1).

---

## 5. Resumen de lo que falta en las capas de abajo

Lista corta de lo que la GUI necesita y todavía no existe, para dimensionar el trabajo.

**Entidades nuevas:**

- `SesionGrabada_BE` + su estado (`Procesando`, `Finalizada`, `ConError`).
- `Categoria_BE` + tipo de activo + atributos según el tipo.
- `Dispositivo_BE` y `Prestamo_BE`.
- `Notificacion_TE`.
- `Idioma_TE` y `Traduccion` (el `//TODO` ya está puesto en `Usuario_TE.IdIdioma`).
- `Respaldo` y la ventana de mantenimiento.

**Campos que faltan en entidades que ya existen:**

- `Usuario_TE`: fecha del último acceso.
- `Empresa_BE`: CUIT, rubro, domicilio, contacto comercial, facturación, renovación,
  límite de usuarios (o una `ContratoEmpresa` aparte).
- `Bitacora_TE`: `IdEmpresa`, y que `IdUsuario` acepte `null`.

**Métodos que faltan en clases que ya existen:**

- `Usuario_TLL.CambiarEstado(idUsuario, EstadoUsuario)`.
- `Empresa_BLL.CambiarEstado(idEmpresa, EstadoEmpresa)`.
- `BitacoraGestor_TLL.Buscar(filtro, pagina, tamano)` y `Contar(filtro)`.
- `GestorIntegridad_SERVICE`: conteo de registros por tabla y fecha de última verificación.

**Servicios nuevos:**

- El módulo de análisis de mirada (es el corazón del producto y da para su propio
  documento).
- `Respaldo_SERVICE`.
- El componente local que habla con el SDK de Tobii y le informa a la web el estado del
  dispositivo.

---

## 6. Checklist antes de considerar esto terminado

- [ ] Descomentar todos los `Exigir()` y `ExigirPermiso()` (0.1).
- [ ] Borrar el selector de rol de prueba de `App.master` (0.2).
- [ ] Borrar el botón "Simular fallo" de `NuevaGrabacion.aspx` (2.3).
- [ ] Sacar `DURACION_DEMO` de `Visualizaciones.aspx` (2.2).
- [ ] Reemplazar todos los datos de ejemplo escritos en el HTML por Repeaters.
- [ ] Pasar los `<input>` a `<asp:...>` en las pantallas que envían datos.
- [ ] Revalidar en el servidor **todo** lo que hoy valida el JavaScript.
- [ ] Mover el filtrado por rol de la GUI al `WHERE` de las consultas (4.2).
- [ ] Cargar la tabla de traducciones con las claves `data-i18n` (0.4).
- [ ] Resolver los textos marcados con `//TODO traducir` (0.4).
- [ ] Registrar en bitácora todo lo que la lista de cada pantalla marca como `Alta`.
- [ ] Buscar `//TODO` en toda la carpeta GUI y que no quede ninguno sin resolver.
