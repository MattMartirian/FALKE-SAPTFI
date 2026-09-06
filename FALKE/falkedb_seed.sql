/* ============================================================================
   Datos base para poder usar Login / Registrar usuario / recuperar contrasena.

   El script de creacion (falkedb.sql) crea las tablas y carga Empresa e Idioma,
   pero:

   1. Deja PermisoTable vacia. Como UsuarioTable.rol_permiso tiene FK contra
      PermisoTable.nombre_permiso, cualquier alta de usuario falla con
      "FK_Usuario_Permiso". -> se agregan los roles minimos.

   2. Deja IntegridadTable con una sola fila (Usuario) y con datos del entorno
      del autor (5 registros), y sin filas para el resto de las tablas bajo
      control de integridad. Como ValidarCredenciales verifica la integridad de
      TODAS las tablas de TablasBD, el login normal queda bloqueado con
      DVH_INVALIDO. -> se inicializa IntegridadTable en "tabla vacia" (DVV = "0",
      0 registros) para cada tabla controlada.

   Tablas bajo control de integridad (enum ORM.TablasBD): Usuario, EmpresaCliente,
   Categoria, CategoriaAppWeb, CategoriaPublicidad, CategoriaSoftware,
   CategoriaVideojuego, SesionGrabada, AnalisisMultiple, AnalisisMultiple_Sesion,
   Token.

   Ejecutar una sola vez sobre la base FalkeDB.

   IMPORTANTE: si alguna de esas tablas YA tiene datos (p. ej. EmpresaClienteTable
   trae 1 fila), este script la deja marcada como "vacia" y el login normal
   seguira bloqueado hasta que un administrador entre con el acceso de emergencia
   (admin / admin) y ejecute la pantalla "Recalcular integridad" una vez. Ese
   recalculo recorre todas las tablas, calcula DVH y DVV reales y los almacena.

   Flujo de prueba:
     - Login con el acceso de emergencia: usuario "admin" / contrasena "admin".
     - (una vez) Recalcular integridad -> deja DVH/DVV coherentes en todas las tablas.
     - Registrar usuario -> se genera un mail en GUI/App_Data/mails/ con un enlace
       a EstablecerContrasena.aspx?token=...
     - Abrir ese enlace, definir la contrasena -> la cuenta queda Activa.
     - Login normal con ese usuario.
   ============================================================================ */
USE [FalkeDB];
GO

/* ---------- Roles / permisos base ---------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.PermisoTable WHERE nombre_permiso = N'Gestor')
    INSERT dbo.PermisoTable (nombre_permiso, tipo_permiso, es_rol_permiso)
    VALUES (N'Gestor', N'compuesto', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.PermisoTable WHERE nombre_permiso = N'Administrador')
    INSERT dbo.PermisoTable (nombre_permiso, tipo_permiso, es_rol_permiso)
    VALUES (N'Administrador', N'compuesto', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.PermisoTable WHERE nombre_permiso = N'Usuario')
    INSERT dbo.PermisoTable (nombre_permiso, tipo_permiso, es_rol_permiso)
    VALUES (N'Usuario', N'compuesto', 1);
GO

/* ---------- Patentes (permisos simples que las pantallas exigen) --------- */
IF NOT EXISTS (SELECT 1 FROM dbo.PermisoTable WHERE nombre_permiso = N'REGISTRAR_EMPRESA')
    INSERT dbo.PermisoTable (nombre_permiso, tipo_permiso, es_rol_permiso)
    VALUES (N'REGISTRAR_EMPRESA', N'simple', 0);

IF NOT EXISTS (SELECT 1 FROM dbo.PermisoTable WHERE nombre_permiso = N'REGISTRAR_USUARIO')
    INSERT dbo.PermisoTable (nombre_permiso, tipo_permiso, es_rol_permiso)
    VALUES (N'REGISTRAR_USUARIO', N'simple', 0);

IF NOT EXISTS (SELECT 1 FROM dbo.PermisoTable WHERE nombre_permiso = N'CREAR_USUARIO_OTRA_EMPRESA')
    INSERT dbo.PermisoTable (nombre_permiso, tipo_permiso, es_rol_permiso)
    VALUES (N'CREAR_USUARIO_OTRA_EMPRESA', N'simple', 0);

IF NOT EXISTS (SELECT 1 FROM dbo.PermisoTable WHERE nombre_permiso = N'RECALCULAR_INTEGRIDAD')
    INSERT dbo.PermisoTable (nombre_permiso, tipo_permiso, es_rol_permiso)
    VALUES (N'RECALCULAR_INTEGRIDAD', N'simple', 0);
GO

/* ---------- Composicion de roles ---------------------------------------------
   Gestor (Pattern Blue): puede todo, incluido crear usuarios de otras empresas.
   Administrador (de una empresa cliente): solo da de alta usuarios de SU empresa. */
IF NOT EXISTS (SELECT 1 FROM dbo.RelacionPermisosTable WHERE nombre_permiso_compuesto = N'Gestor' AND nombre_permiso_incluido = N'REGISTRAR_EMPRESA')
    INSERT dbo.RelacionPermisosTable (nombre_permiso_compuesto, nombre_permiso_incluido) VALUES (N'Gestor', N'REGISTRAR_EMPRESA');

IF NOT EXISTS (SELECT 1 FROM dbo.RelacionPermisosTable WHERE nombre_permiso_compuesto = N'Gestor' AND nombre_permiso_incluido = N'REGISTRAR_USUARIO')
    INSERT dbo.RelacionPermisosTable (nombre_permiso_compuesto, nombre_permiso_incluido) VALUES (N'Gestor', N'REGISTRAR_USUARIO');

IF NOT EXISTS (SELECT 1 FROM dbo.RelacionPermisosTable WHERE nombre_permiso_compuesto = N'Gestor' AND nombre_permiso_incluido = N'CREAR_USUARIO_OTRA_EMPRESA')
    INSERT dbo.RelacionPermisosTable (nombre_permiso_compuesto, nombre_permiso_incluido) VALUES (N'Gestor', N'CREAR_USUARIO_OTRA_EMPRESA');

IF NOT EXISTS (SELECT 1 FROM dbo.RelacionPermisosTable WHERE nombre_permiso_compuesto = N'Gestor' AND nombre_permiso_incluido = N'RECALCULAR_INTEGRIDAD')
    INSERT dbo.RelacionPermisosTable (nombre_permiso_compuesto, nombre_permiso_incluido) VALUES (N'Gestor', N'RECALCULAR_INTEGRIDAD');

IF NOT EXISTS (SELECT 1 FROM dbo.RelacionPermisosTable WHERE nombre_permiso_compuesto = N'Administrador' AND nombre_permiso_incluido = N'REGISTRAR_USUARIO')
    INSERT dbo.RelacionPermisosTable (nombre_permiso_compuesto, nombre_permiso_incluido) VALUES (N'Administrador', N'REGISTRAR_USUARIO');
GO

/* ---------- IntegridadTable inicial: "tabla vacia" por cada tabla controlada -
   DVV de una tabla vacia = "0" (ver GestorIntegridad_SERVICE.CalcularDigitoVerificador).
   No pisa filas ya existentes.                                                  */
MERGE dbo.IntegridadTable AS destino
USING (VALUES
        (N'Usuario'),
        (N'EmpresaCliente'),
        (N'Categoria'),
        (N'CategoriaAppWeb'),
        (N'CategoriaPublicidad'),
        (N'CategoriaSoftware'),
        (N'CategoriaVideojuego'),
        (N'SesionGrabada'),
        (N'AnalisisMultiple'),
        (N'AnalisisMultiple_Sesion'),
        (N'Token')
      ) AS origen (tabla_integridad)
   ON destino.tabla_integridad = origen.tabla_integridad
WHEN MATCHED THEN
    UPDATE SET dvv_integridad = N'0', cantidad_registros_integridad = 0
WHEN NOT MATCHED THEN
    INSERT (tabla_integridad, dvv_integridad, cantidad_registros_integridad)
    VALUES (origen.tabla_integridad, N'0', 0);
GO
