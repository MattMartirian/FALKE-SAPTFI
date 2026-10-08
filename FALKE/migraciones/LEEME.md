# Migraciones de FalkeDB

Scripts SQL que llevan la base al esquema que espera la aplicación. Se aplican **en orden** sobre `FalkeDB`
(las anteriores a la 5 ya estaban aplicadas cuando se empezó a numerarlas).

Todos son idempotentes (se pueden volver a ejecutar sin romper nada), corren dentro de una transacción y están
guardados sin BOM y en ASCII, para pegarlos en SSMS o ejecutarlos con `sqlcmd -I -f 65001`.

| N.º | Archivo | Qué hace |
|---|---|---|
| 5 | `2026-10-04_descripciones_y_permisos.sql` | Descripciones de permisos (`descripcion_permiso`) y patentes de edición de usuarios y empresas. |
| 6 | `2026-10-04_rol_analista.sql` | El rol base `Usuario` pasa a llamarse `Analista`. |
| 7 | `2026-10-05_categorias.sql` | Categorías: columna `activa_categoria` y permiso `GESTIONAR_CATEGORIAS`. |
| 8 | `2026-10-05_dispositivos.sql` | Dispositivos y préstamos bajo el control de integridad. |
| 9 | `2026-10-05_dispositivos_permisos.sql` | Permisos de dispositivos separados: ver, alta y asignar. |
| 10 | `2026-10-05_modelos_dispositivo.sql` | Catálogo de modelos; se quita la calibración. |
| 11 | `2026-10-05_respaldos.sql` | Respaldos: permisos (ver, hacer, restaurar) y `BackupTable` bajo integridad. |
| 12 | `2026-10-05_webmaster.sql` | Rol Webmaster, roles de gestión y `VER_BITACORA_COMPLETA`. |
| 13 | `2026-10-05_quitar_notificaciones.sql` | Se elimina `NotificacionTable`. |
| 14 | `2026-10-05_relaciones_permisos_validas.sql` | Disparador que rechaza estructuras imposibles de permisos. |

## Después de aplicarlas

1. Entrar con la cuenta de emergencia y ejecutar **Recalcular integridad** (pantalla *Dígito verificador*).
2. Asignarle el rol **Webmaster** a quien corresponda, desde *Usuarios*.

Las que agregan una tabla bajo control de integridad (8, 10 y 11) deben aplicarse **antes** de ejecutar la versión
nueva de la aplicación: sin su fila en `IntegridadTable` el sistema rechaza todos los ingresos.
