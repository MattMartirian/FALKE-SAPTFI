/*
  Migracion 19 (version corta) - Solo los textos nuevos: pantalla Instalacion e Idiomas (crear desde cero).

  Inserta unicamente lo que falta en los idiomas ya cargados; no pisa textos existentes.
  Idempotente, una sola transaccion, ASCII sin BOM. Despues, reiniciar la aplicacion.
*/
USE [FalkeDB];
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @textos TABLE (codigo_idioma varchar(10), clave varchar(100), texto nvarchar(500));

    INSERT INTO @textos (codigo_idioma, clave, texto) VALUES
        ('es', 'app_instalacion', N'Instalaci' + NCHAR(243) + N'n'),
        ('es', 'descargas_titulo', N'Primera vez en esta PC'),
        ('es', 'descargas_primera_vez', N'Si es tu primera vez, descarga esto'),
        ('es', 'descargas_explicacion', N'Para registrar la mirada, esta PC necesita el software del eye tracker y el agente de Falke. Se instalan una sola vez.'),
        ('es', 'descargas_descargar_todo', N'Descargar todo (.zip)'),
        ('es', 'descargas_descargar', N'Descargar'),
        ('es', 'descargas_disponible', N'Disponible'),
        ('es', 'descargas_no_disponible', N'No disponible en el servidor'),
        ('es', 'descargas_sitio_oficial', N'Sitio oficial'),
        ('es', 'descargas_sin_instaladores', N'El servidor no tiene ning' + NCHAR(250) + N'n instalador cargado.'),
        ('es', 'descargas_tobii_titulo', N'Tobii Experience'),
        ('es', 'descargas_tobii_descripcion', N'Controlador y servicios del Tobii Eye Tracker 5. Sin esto la PC no detecta el dispositivo.'),
        ('es', 'descargas_agente_titulo', N'Agente de mirada de Falke'),
        ('es', 'descargas_agente_descripcion', N'Programa que lee la mirada del Tobii y la env' + NCHAR(237) + N'a a esta p' + NCHAR(225) + N'gina. Se ejecuta en la PC donde se graba.'),
        ('es', 'descargas_despues_de_descargar', N'Despu' + NCHAR(233) + N's de descargar'),
        ('es', 'descargas_paso_1', N'Instala Tobii Experience y reinicia la PC si te lo pide.'),
        ('es', 'descargas_paso_2', N'Conecta el Tobii Eye Tracker 5 y completa su calibraci' + NCHAR(243) + N'n.'),
        ('es', 'descargas_paso_3', N'Descomprime AgenteMirada.zip en una carpeta y ejecuta AgenteMirada.exe.'),
        ('es', 'descargas_paso_4', N'Deja el agente abierto y entra a Nueva grabaci' + NCHAR(243) + N'n.'),
        ('es', 'descargas_requisitos', N'Requiere Windows 10 u 11 de 64 bits y el Tobii Eye Tracker 5 conectado por USB.'),
        ('es', 'idioma_nombre_invalido', N'El nombre del idioma no puede superar los {0} caracteres.'),
        ('es', 'idioma_traduccion_base_obligatoria', N'El texto del idioma base no puede quedar vac' + NCHAR(237) + N'o: es el que se muestra cuando falta una traducci' + NCHAR(243) + N'n.'),
        ('es', 'idiomas_desde_cero', N'Otro idioma (crear desde cero)'),
        ('es', 'idiomas_codigo', N'C' + NCHAR(243) + N'digo'),
        ('es', 'idiomas_nombre', N'Nombre del idioma'),
        ('es', 'idiomas_agregar_ayuda', N'Si eleg' + NCHAR(237) + N's un paquete incluido se cargan sus traducciones. Si lo cre' + NCHAR(225) + N's desde cero queda sin textos: cada uno que falte se muestra en el idioma base.'),
        ('es', 'idiomas_fallback_ayuda', N'Si un texto no est' + NCHAR(225) + N' traducido en este idioma se muestra el del idioma base; si tampoco existe ah' + NCHAR(237) + N', se muestra la clave.'),
        ('es', 'descargas_bienvenida_titulo', N'Prepara esta PC antes de tu primera grabaci' + NCHAR(243) + N'n'),
        ('es', 'descargas_bienvenida_mensaje', N'Para registrar la mirada, esta PC necesita dos programas: el controlador del Tobii Eye Tracker 5 y el agente de Falke. Desc' + NCHAR(225) + N'rgalos juntos en un solo archivo; se instalan una ' + NCHAR(250) + N'nica vez.'),
        ('en', 'app_instalacion', N'Installation'),
        ('en', 'descargas_titulo', N'First time on this PC'),
        ('en', 'descargas_primera_vez', N'If this is your first time, download this'),
        ('en', 'descargas_explicacion', N'To record gaze, this PC needs the eye tracker software and the Falke agent. You only install them once.'),
        ('en', 'descargas_descargar_todo', N'Download all (.zip)'),
        ('en', 'descargas_descargar', N'Download'),
        ('en', 'descargas_disponible', N'Available'),
        ('en', 'descargas_no_disponible', N'Not available on the server'),
        ('en', 'descargas_sitio_oficial', N'Official site'),
        ('en', 'descargas_sin_instaladores', N'The server has no installers loaded.'),
        ('en', 'descargas_tobii_titulo', N'Tobii Experience'),
        ('en', 'descargas_tobii_descripcion', N'Driver and services for the Tobii Eye Tracker 5. Without it the PC does not detect the device.'),
        ('en', 'descargas_agente_titulo', N'Falke gaze agent'),
        ('en', 'descargas_agente_descripcion', N'Program that reads the Tobii gaze and sends it to this page. It runs on the PC where you record.'),
        ('en', 'descargas_despues_de_descargar', N'After downloading'),
        ('en', 'descargas_paso_1', N'Install Tobii Experience and restart the PC if asked.'),
        ('en', 'descargas_paso_2', N'Connect the Tobii Eye Tracker 5 and complete its calibration.'),
        ('en', 'descargas_paso_3', N'Unzip AgenteMirada.zip into a folder and run AgenteMirada.exe.'),
        ('en', 'descargas_paso_4', N'Leave the agent open and go to New recording.'),
        ('en', 'descargas_requisitos', N'Requires 64-bit Windows 10 or 11 and the Tobii Eye Tracker 5 connected by USB.'),
        ('en', 'idioma_nombre_invalido', N'The language name cannot exceed {0} characters.'),
        ('en', 'idioma_traduccion_base_obligatoria', N'The base language text cannot be empty: it is the one shown when a translation is missing.'),
        ('en', 'idiomas_desde_cero', N'Other language (create from scratch)'),
        ('en', 'idiomas_codigo', N'Code'),
        ('en', 'idiomas_nombre', N'Language name'),
        ('en', 'idiomas_agregar_ayuda', N'If you choose an included package, its translations are loaded. If you create it from scratch it has no texts: each missing one is shown in the base language.'),
        ('en', 'idiomas_fallback_ayuda', N'If a text is not translated in this language, the base language text is shown; if it does not exist there either, the key is shown.'),
        ('en', 'descargas_bienvenida_titulo', N'Set up this PC before your first recording'),
        ('en', 'descargas_bienvenida_mensaje', N'To record gaze, this PC needs two programs: the Tobii Eye Tracker 5 driver and the Falke agent. Download them together in a single file; you only install them once.'),
        ('pt', 'app_instalacion', N'Instala' + NCHAR(231) + NCHAR(227) + N'o'),
        ('pt', 'descargas_titulo', N'Primeira vez neste PC'),
        ('pt', 'descargas_primera_vez', N'Se ' + NCHAR(233) + N' a sua primeira vez, baixe isto'),
        ('pt', 'descargas_explicacion', N'Para registrar o olhar, este PC precisa do software do eye tracker e do agente do Falke. Eles s' + NCHAR(227) + N'o instalados uma ' + NCHAR(250) + N'nica vez.'),
        ('pt', 'descargas_descargar_todo', N'Baixar tudo (.zip)'),
        ('pt', 'descargas_descargar', N'Baixar'),
        ('pt', 'descargas_disponible', N'Dispon' + NCHAR(237) + N'vel'),
        ('pt', 'descargas_no_disponible', N'N' + NCHAR(227) + N'o dispon' + NCHAR(237) + N'vel no servidor'),
        ('pt', 'descargas_sitio_oficial', N'Site oficial'),
        ('pt', 'descargas_sin_instaladores', N'O servidor n' + NCHAR(227) + N'o tem nenhum instalador carregado.'),
        ('pt', 'descargas_tobii_titulo', N'Tobii Experience'),
        ('pt', 'descargas_tobii_descripcion', N'Driver e servi' + NCHAR(231) + N'os do Tobii Eye Tracker 5. Sem isso o PC n' + NCHAR(227) + N'o detecta o dispositivo.'),
        ('pt', 'descargas_agente_titulo', N'Agente de olhar do Falke'),
        ('pt', 'descargas_agente_descripcion', N'Programa que l' + NCHAR(234) + N' o olhar do Tobii e o envia para esta p' + NCHAR(225) + N'gina. Roda no PC onde voc' + NCHAR(234) + N' grava.'),
        ('pt', 'descargas_despues_de_descargar', N'Depois de baixar'),
        ('pt', 'descargas_paso_1', N'Instale o Tobii Experience e reinicie o PC se for solicitado.'),
        ('pt', 'descargas_paso_2', N'Conecte o Tobii Eye Tracker 5 e conclua a calibra' + NCHAR(231) + NCHAR(227) + N'o.'),
        ('pt', 'descargas_paso_3', N'Descompacte o AgenteMirada.zip em uma pasta e execute o AgenteMirada.exe.'),
        ('pt', 'descargas_paso_4', N'Deixe o agente aberto e entre em Nova grava' + NCHAR(231) + NCHAR(227) + N'o.'),
        ('pt', 'descargas_requisitos', N'Requer Windows 10 ou 11 de 64 bits e o Tobii Eye Tracker 5 conectado por USB.'),
        ('pt', 'idioma_nombre_invalido', N'O nome do idioma n' + NCHAR(227) + N'o pode ter mais de {0} caracteres.'),
        ('pt', 'idioma_traduccion_base_obligatoria', N'O texto do idioma base n' + NCHAR(227) + N'o pode ficar vazio: ' + NCHAR(233) + N' o que aparece quando falta uma tradu' + NCHAR(231) + NCHAR(227) + N'o.'),
        ('pt', 'idiomas_desde_cero', N'Outro idioma (criar do zero)'),
        ('pt', 'idiomas_codigo', N'C' + NCHAR(243) + N'digo'),
        ('pt', 'idiomas_nombre', N'Nome do idioma'),
        ('pt', 'idiomas_agregar_ayuda', N'Se voc' + NCHAR(234) + N' escolher um pacote inclu' + NCHAR(237) + N'do, as tradu' + NCHAR(231) + NCHAR(245) + N'es dele s' + NCHAR(227) + N'o carregadas. Se criar do zero, o idioma fica sem textos: cada um que faltar aparece no idioma base.'),
        ('pt', 'idiomas_fallback_ayuda', N'Se um texto n' + NCHAR(227) + N'o estiver traduzido neste idioma, aparece o do idioma base; se tamb' + NCHAR(233) + N'm n' + NCHAR(227) + N'o existir l' + NCHAR(225) + N', aparece a chave.'),
        ('pt', 'descargas_bienvenida_titulo', N'Prepare este PC antes da sua primeira grava' + NCHAR(231) + NCHAR(227) + N'o'),
        ('pt', 'descargas_bienvenida_mensaje', N'Para registrar o olhar, este PC precisa de dois programas: o driver do Tobii Eye Tracker 5 e o agente do Falke. Baixe os dois juntos em um ' + NCHAR(250) + N'nico arquivo; eles s' + NCHAR(227) + N'o instalados uma s' + NCHAR(243) + N' vez.'),
        ('de', 'app_instalacion', N'Installation'),
        ('de', 'descargas_titulo', N'Erstmalig auf diesem PC'),
        ('de', 'descargas_primera_vez', N'Wenn Sie zum ersten Mal hier sind, laden Sie dies herunter'),
        ('de', 'descargas_explicacion', N'Um den Blick aufzuzeichnen, ben' + NCHAR(246) + N'tigt dieser PC die Eyetracker-Software und den Falke-Agenten. Sie werden nur einmal installiert.'),
        ('de', 'descargas_descargar_todo', N'Alles herunterladen (.zip)'),
        ('de', 'descargas_descargar', N'Herunterladen'),
        ('de', 'descargas_disponible', N'Verf' + NCHAR(252) + N'gbar'),
        ('de', 'descargas_no_disponible', N'Auf dem Server nicht verf' + NCHAR(252) + N'gbar'),
        ('de', 'descargas_sitio_oficial', N'Offizielle Website'),
        ('de', 'descargas_sin_instaladores', N'Auf dem Server sind keine Installationsprogramme hinterlegt.'),
        ('de', 'descargas_tobii_titulo', N'Tobii Experience'),
        ('de', 'descargas_tobii_descripcion', N'Treiber und Dienste f' + NCHAR(252) + N'r den Tobii Eye Tracker 5. Ohne sie erkennt der PC das Ger' + NCHAR(228) + N't nicht.'),
        ('de', 'descargas_agente_titulo', N'Falke-Blickagent'),
        ('de', 'descargas_agente_descripcion', N'Programm, das den Blick des Tobii liest und an diese Seite sendet. Es l' + NCHAR(228) + N'uft auf dem PC, auf dem aufgezeichnet wird.'),
        ('de', 'descargas_despues_de_descargar', N'Nach dem Herunterladen'),
        ('de', 'descargas_paso_1', N'Installieren Sie Tobii Experience und starten Sie den PC neu, falls dazu aufgefordert.'),
        ('de', 'descargas_paso_2', N'Schlie' + NCHAR(223) + N'en Sie den Tobii Eye Tracker 5 an und f' + NCHAR(252) + N'hren Sie die Kalibrierung durch.'),
        ('de', 'descargas_paso_3', N'Entpacken Sie AgenteMirada.zip in einen Ordner und starten Sie AgenteMirada.exe.'),
        ('de', 'descargas_paso_4', N'Lassen Sie den Agenten ge' + NCHAR(246) + N'ffnet und ' + NCHAR(246) + N'ffnen Sie Neue Aufnahme.'),
        ('de', 'descargas_requisitos', N'Erfordert 64-Bit-Windows 10 oder 11 und den per USB verbundenen Tobii Eye Tracker 5.'),
        ('de', 'idioma_nombre_invalido', N'Der Sprachname darf h' + NCHAR(246) + N'chstens {0} Zeichen lang sein.'),
        ('de', 'idioma_traduccion_base_obligatoria', N'Der Text der Basissprache darf nicht leer sein: Er wird angezeigt, wenn eine ' + NCHAR(220) + N'bersetzung fehlt.'),
        ('de', 'idiomas_desde_cero', N'Andere Sprache (neu erstellen)'),
        ('de', 'idiomas_codigo', N'Code'),
        ('de', 'idiomas_nombre', N'Name der Sprache'),
        ('de', 'idiomas_agregar_ayuda', N'Wenn Sie ein enthaltenes Paket w' + NCHAR(228) + N'hlen, werden dessen ' + NCHAR(220) + N'bersetzungen geladen. Wenn Sie die Sprache neu erstellen, hat sie keine Texte: Jeder fehlende Text wird in der Basissprache angezeigt.'),
        ('de', 'idiomas_fallback_ayuda', N'Ist ein Text in dieser Sprache nicht ' + NCHAR(252) + N'bersetzt, wird der Text der Basissprache angezeigt; fehlt er auch dort, wird der Schl' + NCHAR(252) + N'ssel angezeigt.'),
        ('de', 'descargas_bienvenida_titulo', N'Richten Sie diesen PC vor Ihrer ersten Aufnahme ein'),
        ('de', 'descargas_bienvenida_mensaje', N'Um den Blick aufzuzeichnen, ben' + NCHAR(246) + N'tigt dieser PC zwei Programme: den Treiber des Tobii Eye Tracker 5 und den Falke-Agenten. Laden Sie beide gemeinsam in einer Datei herunter; die Installation ist nur einmal n' + NCHAR(246) + N'tig.');

    INSERT INTO dbo.TraduccionTable (codigo_clave_traduccion, id_idioma, texto_traduccion, pendiente_traduccion, origen_traduccion)
    SELECT x.clave, i.id_idioma, x.texto, 0, 'resx'
    FROM @textos x
    INNER JOIN dbo.IdiomaTable i ON i.codigo_idioma = x.codigo_idioma
    WHERE NOT EXISTS (SELECT 1 FROM dbo.TraduccionTable t
                      WHERE t.codigo_clave_traduccion = x.clave AND t.id_idioma = i.id_idioma);

    PRINT CONCAT('Textos agregados: ', @@ROWCOUNT);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
