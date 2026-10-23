/*
  Migracion 18 - Idiomas: textos del estado de cada idioma.

  - Agrega idiomas_estado_habilitado e idiomas_estado_deshabilitado a los idiomas ya cargados
    (es, en, pt y de) si ese idioma existe y todavia no tiene la clave. No pisa textos existentes.

  Idempotente, en una sola transaccion, sin BOM y en ASCII.
  No toca IntegridadTable: TraduccionTable no esta bajo control de integridad.
*/
USE [FalkeDB];
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @textos TABLE (codigo_idioma varchar(10), clave varchar(100), texto nvarchar(500));

    INSERT INTO @textos (codigo_idioma, clave, texto) VALUES
        ('es', 'idiomas_estado_habilitado',    N'Habilitado'),
        ('es', 'idiomas_estado_deshabilitado', N'Deshabilitado'),
        ('en', 'idiomas_estado_habilitado',    N'Enabled'),
        ('en', 'idiomas_estado_deshabilitado', N'Disabled'),
        ('pt', 'idiomas_estado_habilitado',    N'Habilitado'),
        ('pt', 'idiomas_estado_deshabilitado', N'Desabilitado'),
        ('de', 'idiomas_estado_habilitado',    N'Aktiviert'),
        ('de', 'idiomas_estado_deshabilitado', N'Deaktiviert');

    INSERT INTO dbo.TraduccionTable (codigo_clave_traduccion, id_idioma, texto_traduccion, pendiente_traduccion, origen_traduccion)
    SELECT x.clave, i.id_idioma, x.texto, 0, 'resx'
    FROM @textos x
    INNER JOIN dbo.IdiomaTable i ON i.codigo_idioma = x.codigo_idioma
    WHERE NOT EXISTS (SELECT 1 FROM dbo.TraduccionTable t
                      WHERE t.codigo_clave_traduccion = x.clave AND t.id_idioma = i.id_idioma);

    COMMIT TRANSACTION;

    SELECT i.codigo_idioma, t.codigo_clave_traduccion, t.texto_traduccion
    FROM dbo.TraduccionTable t
    INNER JOIN dbo.IdiomaTable i ON i.id_idioma = t.id_idioma
    WHERE t.codigo_clave_traduccion LIKE 'idiomas_estado_%'
    ORDER BY i.codigo_idioma, t.codigo_clave_traduccion;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
