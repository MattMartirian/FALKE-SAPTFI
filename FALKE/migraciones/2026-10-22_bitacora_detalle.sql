/*
  Migracion 17 - Bitacora: columna detalle_bitacora.

  - BitacoraTable.detalle_bitacora (NVARCHAR(1000), NULL): especifica sobre quien o sobre que se hizo la accion.
    descripcion_bitacora pasa a guardar un codigo de evento (por ejemplo bitacora_ev_inicio_sesion) que la
    aplicacion traduce al idioma de quien mira; el detalle puede llevar nombres propios (usuarios, empresas,
    numeros de serie) y fragmentos traducibles entre llaves.
  - Los registros anteriores conservan su descripcion en texto libre y quedan con el detalle vacio.

  Idempotente, en una sola transaccion, sin BOM y en ASCII.
  No toca IntegridadTable: BitacoraTable no esta bajo control de integridad.
*/
USE [FalkeDB];
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF COL_LENGTH('dbo.BitacoraTable', 'detalle_bitacora') IS NULL
        EXEC (N'ALTER TABLE dbo.BitacoraTable ADD detalle_bitacora NVARCHAR(1000) NULL');

    COMMIT TRANSACTION;

    EXEC (N'SELECT TOP (5) id_evento_bitacora, descripcion_bitacora, detalle_bitacora FROM dbo.BitacoraTable ORDER BY id_evento_bitacora DESC');
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
