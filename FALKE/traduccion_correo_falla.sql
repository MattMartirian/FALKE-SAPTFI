USE FalkeDB;
GO
UPDATE t SET t.texto_traduccion = v.texto
FROM dbo.TraduccionTable t
JOIN dbo.IdiomaTable i ON i.id_idioma = t.id_idioma
JOIN (VALUES
 (N'es', N' Hubo un error al enviar el correo de activación. Reenvíe la invitación más tarde desde la sección Usuarios.'),
 (N'en', N' There was an error sending the activation email. Resend the invitation later from the Users section.'),
 (N'pt', N' Ocorreu um erro ao enviar o e-mail de ativação. Reenvie o convite mais tarde na seção Usuários.'),
 (N'de', N' Beim Senden der Aktivierungs-E-Mail ist ein Fehler aufgetreten. Senden Sie die Einladung später im Bereich Benutzer erneut.')
) v(codigo, texto) ON v.codigo = i.codigo_idioma
WHERE t.codigo_clave_traduccion = N'empresa_nueva_ok_correo_falla';
GO
