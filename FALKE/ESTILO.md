# Manual de estilo — FALKE

Convenciones observadas en el código actual. Sirve de referencia para escribir código nuevo que "se lea igual" que el resto.

---

## IDIOMA
- **Todo en español**: clases, métodos, variables, comentarios, mensajes, códigos de error.
- Identificadores **sin acentos**. En comentarios y strings el código viejo usa acentos (`composición`), el nuevo (GUI) los evita (`solicitud`, `Contrasena`).

## FORMATO
- Indentación: **4 espacios**.
- Llaves **Allman** (llave en su propia línea) para tipos, métodos y bloques de varias sentencias.
- Líneas en blanco entre pasos lógicos dentro de un método (espaciado vertical generoso).
- Un `using` de recurso por bloque, anidados: `using (var con = CrearConexion()) { ... }`.
- BOM al inicio en archivos viejos; los nuevos van sin BOM.

## ARCHIVOS
- Un archivo por clase principal, con el mismo nombre.
- Se permiten tipos auxiliares en el mismo archivo (enum + clase + excepción juntos, p. ej. `PermisoAbstracto_TE.cs`).

## NOMBRE DE CLASE
- **Siempre `{Clase}_{Capa}`**: nombre, guión bajo, capa. Ej.: `Usuario_TE`, `Integridad_ORM`, `GestorIntegridad_SERVICE`, `Usuario_TLL`, `ResultadoLogin_TLL`.
- **Únicas excepciones**:
  - **Repositorios**: `{Entidad}Repository` (`UsuarioRepository`, `PermisoRepository`).
  - **Enums**: nombre a secas, sin sufijo (`EstadoUsuario`, `TipoPermiso`, `TablasBD`).

## NAMESPACES / CAPAS
- Un namespace por proyecto: `BE  TE  DAL  ORM  SERVICES  BLL  TLL  SECURITY  GUI`.
- Dependencias **solo hacia abajo**: `GUI → TLL/SERVICES → ORM → DAL`.
- Sin inyección de dependencias: el constructor (sin parámetros) instancia lo que necesita (`usuarioRepo = new UsuarioRepository();`).

## CLASES
- **PascalCase**. `sealed` para singletons, `abstract` para clases base. No hay interfaces.
- Orden interno: constantes → `static readonly` → campos privados → constructor → métodos públicos → helpers privados → mapping al final.
- **Regions** solo para bloques recurrentes: `Singleton`, `Mapping`, `Helpers`, `Helpers SQL`. Línea en blanco después de `#region` y antes de `#endregion`. La mayoría de las clases no usan regions.

## SINGLETON
```csharp
private static readonly Lazy<T> _instancia = new Lazy<T>(() => new T());
public static T Instancia => _instancia.Value;
private T() { ... }
```

## MÉTODOS
- **PascalCase**, empiezan con verbo: `Obtener…`, `Registrar…`, `Calcular…`, `Validar…`, `Guardar…`, `Actualizar…`.
- Métodos/propiedades booleanas: prefijo `Es… / Hay… / Existe… / Genera…`.
- **Expression-bodied (`=>`)** para one-liners: propiedades, pass-through (`ObtenerPorId(int id) => usuarioRepo.ObtenerPorPK(id);`) y factories.
- **Guard clauses** arriba, con `return` / `throw` temprano.

## VARIABLES Y CAMPOS
- Campos privados: `camelCase` **sin** `_` (`usuarioRepo`, `cifrador`, `gestorIntegridad`). Excepción: campos `static` de singleton llevan `_` (`_instancia`, `_connectionString`).
- Locales y parámetros: `camelCase`, en español, `var` casi siempre. Nombres cortos en repos (`u`, `p`, `dr`, `dt`).
- Propiedades públicas: `PascalCase` (`IdUsuario`, `NombreUsuario`). Siglas en mayúscula (`DVH`); se escribe `Id`, no `ID`.

## CONSTANTES
- `const` y `static readonly` de negocio/config: **`UPPER_SNAKE_CASE`** (`MAX_INTENTOS_FALLIDOS`, `TOKEN_ACTIVACION`, `VIGENCIA_ACTIVACION`).
- (`Cifrador` usa `PascalCase` en sus consts internas: excepción histórica, no imitar.)

## ENUMS
- `PascalCase` el tipo y los miembros. Valor explícito cuando importa el número (`Pendiente = 0`).
- Se definen en el mismo archivo que la clase que los usa.

## IF
- **Siempre en una sola línea** cuando el cuerpo es una sentencia (sin llaves):
  ```csharp
  if (usuario == null) return ResultadoLogin.CredencialesInvalidas();
  ```
- Si **no** entra en una línea (condición larga, o cuerpo de 2+ sentencias) → **siempre con llaves Allman**. Nunca cuerpo indentado sin llaves.
  ```csharp
  if (!arbol.TryGetValue(nombre, out var nodo))
  {
      throw new PermisoInvalidoException("El permiso no existe.");
  }
  ```
- `else if` a veces se separa con una línea en blanco.

## TERNARIOS
- **Siempre en una sola línea**, sin excepción:
  ```csharp
  return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
  ```
- Si un ternario no entra en una línea legible, se reescribe como `if/else` con llaves.

## BUCLES
- **`foreach` es el default.** El cuerpo va **siempre entre llaves y en líneas propias debajo**, sin excepción (nunca en la misma línea):
  ```csharp
  foreach (DataRow row in dt.Rows)
  {
      lista.Add(Map(row));
  }
  ```
- `for` con índice solo para recorrer arrays/columnas en el mapping (`for (int i = 0; i < columnas.Count; i++)`), también con llaves.
- `while` es raro.

## SWITCH
- `case "X":` en su línea, cuerpo indentado con su propio `return`, `default:` al final.

## SQL (capa ORM)
- **Siempre parametrizado**: `new SqlParameter("@x", valor)`. Nunca concatenar valores.
- Params SQL: `@camelCase`. Columnas: `snake_case`. Tablas: `PascalCaseTable`.
- Multilínea con `@"..."` indentado; una sola línea con `"..."`.
- `ValorONulo(x)` para pasar nullables; `Valor<T>(dr, "col")` para leer.
- Un `SqlParameter` por línea cuando hay varios.

## REPOSITORIOS
- Heredan `RepositoryBase<TEntity, TKey>`; implementan `Alta / Modificar / ObtenerPorPK / ObtenerTodos`.
- Búsquedas extra como métodos públicos (`ObtenerPorEmail`, `ObtenerPorEmpresa`).
- `Map(DataRow)` y `MapTodos(DataTable)` privados `static`, en `#region Mapping` al final.

## RESULTADOS (en vez de excepciones para casos esperados)
- Clase `ResultadoX` con: ctor privado + factories `static` + `bool Exito` + `string Motivo`.
- `Motivo` es un **código `UPPER_SNAKE`** (`"CREDENCIALES_INVALIDAS"`, `"TOKEN_EXPIRADO"`), no un texto para el usuario.
  ```csharp
  public static ResultadoLogin Exitoso(Usuario_TE u) => new ResultadoLogin { Exito = true, Usuario = u };
  ```

## EXCEPCIONES
- Errores de dominio: excepción propia (`PermisoInvalidoException : System.Exception`) o `InvalidOperationException` / `ArgumentException`, con mensaje en español.
- `//TODO` (o `//TODO:`) en español para dejar registrada deuda conocida.

## COMENTARIOS
- `//` arriba del código, explican el **porqué**, no el qué.
- `/// <summary>` solo en helpers compartidos y API pública de ORM / SERVICES / TLL.
- `#region` como comentario estructural.

## GUI — code-behind
- `namespace GUI`; `public partial class X : System.Web.UI.Page`.
- `Page_Load` suele quedar vacío.
- Sesión vía helper estático `SesionActual` (`Exigir()`, `Iniciar()`, `Cerrar()`).
- Redirect: `Response.Redirect(url, false); Context.ApplicationInstance.CompleteRequest(); return;`.
- Manejo de error estándar:
  ```csharp
  try { ... }
  catch (Exception ex)
  {
      ErrorLog.Registrar("Contexto", ex);
      lblMsg.Text = "Ocurrio un error al procesar la solicitud. Intente nuevamente.";
  }
  ```
- Reglas de negocio: `catch (InvalidOperationException ex)` aparte, se muestra `ex.Message`.
- Inputs: `.Trim()` + `int.TryParse(...)`; si falla, texto a `lblMsg` y `return`.

## GUI — markup (.aspx)
- `<%@ Page Language="C#" AutoEventWireup="true" CodeFile="X.aspx.cs" Inherits="GUI.X" %>`.
- HTML5, `<head runat="server">` con meta charset + `<title>`.
- Controles `<asp:…>` autocerrados. `ID` con prefijo húngaro: `txt  lbl  btn  gv  pnl`.
- Sin CSS (pantallas de prueba); `style="..."` inline solo cuando hace falta (p. ej. `overflow-x:auto`).
- Eventos por `OnClick="btnX_Click"`.

## LO QUE NO SE USA
- `async / await`, interfaces, inyección de dependencias.
- LINQ salvo casos muy puntuales (se prefiere `foreach`).
- Los `using` del template (`System.Linq`, `System.Text`, `System.Threading.Tasks`) se recortan a lo que el archivo realmente usa.
