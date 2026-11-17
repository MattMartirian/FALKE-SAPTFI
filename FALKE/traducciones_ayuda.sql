-- Textos de la ayuda en linea (boton "?" de la barra superior).
-- 21 claves por idioma. Actualiza las existentes e inserta las que faltan, para los idiomas cargados en IdiomaTable.
USE FalkeDB;
GO
SET NOCOUNT ON;
GO

DECLARE @t TABLE (clave varchar(100), codigo varchar(10), texto nvarchar(max));
INSERT INTO @t (clave, codigo, texto) VALUES
(N'ayuda_boton', N'es', N'Averiguá cómo funciona esta página'),
(N'ayuda_boton', N'en', N'Find out how this page works'),
(N'ayuda_boton', N'pt', N'Descubra como esta página funciona'),
(N'ayuda_boton', N'de', N'Erfahren Sie, wie diese Seite funktioniert'),
(N'ayuda_subtitulo', N'es', N'Para qué sirve esta página'),
(N'ayuda_subtitulo', N'en', N'What this page is for'),
(N'ayuda_subtitulo', N'pt', N'Para que serve esta página'),
(N'ayuda_subtitulo', N'de', N'Wozu diese Seite dient'),
(N'ayuda_panel', N'es', N'Esta es la pantalla de inicio de Falke.
Muestra un resumen de la actividad de tu empresa y los últimos movimientos hechos desde tu cuenta.
- Con "Nueva grabación" iniciás una sesión de seguimiento ocular directamente desde acá.
Para volver a esta pantalla desde cualquier otra, usá el logo de Falke o "Página principal" en el menú.'),
(N'ayuda_panel', N'en', N'This is the Falke home screen.
It shows a summary of your company''s activity and the latest movements made from your account.
- With "New recording" you start an eye-tracking session directly from here.
To return to this screen from any other, use the Falke logo or "Home page" in the menu.'),
(N'ayuda_panel', N'pt', N'Esta é a tela inicial do Falke.
Mostra um resumo da atividade da sua empresa e as últimas movimentações feitas a partir da sua conta.
- Com "Nova gravação" você inicia uma sessão de rastreamento ocular diretamente daqui.
Para voltar a esta tela a partir de qualquer outra, use o logotipo do Falke ou "Página inicial" no menu.'),
(N'ayuda_panel', N'de', N'Dies ist die Startseite von Falke.
Sie zeigt eine Zusammenfassung der Aktivität Ihres Unternehmens und die letzten Vorgänge, die von Ihrem Konto aus ausgeführt wurden.
- Mit "Neue Aufnahme" starten Sie direkt von hier aus eine Eye-Tracking-Sitzung.
Um von einer anderen Seite hierher zurückzukehren, nutzen Sie das Falke-Logo oder im Menü "Startseite".'),
(N'ayuda_visualizaciones', N'es', N'Esta es la pantalla de visualización de sesiones.
Desde acá abrís una sesión grabada y analizás la mirada del tester: métricas generales, video con seguimiento, mapa de calor, zonas de interés y ciegas, dispersión, atención frente a distracción y recorrido.
- Con "Filtros" buscás por nombre, categoría y rango de fechas. "Limpiar filtros" restablece la búsqueda.
- Con "Ver detalle" en cada gráfico abrís ese análisis en detalle.
- Con "Descargar reporte" generás un PDF de la sesión. Elegís las secciones y el idioma, y confirmás con "Exportar reporte".
Para analizar una sesión:
1. Aplicá los filtros que necesites para ubicarla en el listado.
2. Seleccioná la sesión.
3. Revisá las métricas y abrí cada gráfico con "Ver detalle".
4. Para obtener el análisis en PDF, tocá "Descargar reporte" y elegí los datos a incluir.'),
(N'ayuda_visualizaciones', N'en', N'This is the session viewing screen.
From here you open a recorded session and analyze the tester''s gaze: general metrics, video with tracking, heat map, areas of interest and blind spots, dispersion, attention versus distraction and gaze path.
- With "Filters" you search by name, category and date range. "Clear filters" resets the search.
- With "View details" on each chart you open that analysis in detail.
- With "Download report" you generate a PDF of the session. You choose the sections and the language, and confirm with "Export report".
To analyze a session:
1. Apply the filters you need to find it in the list.
2. Select the session.
3. Review the metrics and open each chart with "View details".
4. To get the analysis as a PDF, click "Download report" and choose the data to include.'),
(N'ayuda_visualizaciones', N'pt', N'Esta é a tela de visualização de sessões.
Daqui você abre uma sessão gravada e analisa o olhar do testador: métricas gerais, vídeo com rastreamento, mapa de calor, zonas de interesse e cegas, dispersão, atenção versus distração e trajeto.
- Com "Filtros" você busca por nome, categoria e intervalo de datas. "Limpar filtros" restabelece a busca.
- Com "Ver detalhes" em cada gráfico você abre essa análise em detalhe.
- Com "Baixar relatório" você gera um PDF da sessão. Escolhe as seções e o idioma, e confirma com "Exportar relatório".
Para analisar uma sessão:
1. Aplique os filtros de que precisar para encontrá-la na lista.
2. Selecione a sessão.
3. Revise as métricas e abra cada gráfico com "Ver detalhes".
4. Para obter a análise em PDF, clique em "Baixar relatório" e escolha os dados a incluir.'),
(N'ayuda_visualizaciones', N'de', N'Dies ist die Seite zur Ansicht der Sitzungen.
Von hier öffnen Sie eine aufgezeichnete Sitzung und analysieren den Blick der Testperson: allgemeine Metriken, Video mit Tracking, Heatmap, Interessen- und blinde Zonen, Streuung, Aufmerksamkeit gegenüber Ablenkung und Blickverlauf.
- Mit "Filter" suchen Sie nach Name, Kategorie und Datumsbereich. "Filter zurücksetzen" stellt die Suche zurück.
- Mit "Details anzeigen" bei jedem Diagramm öffnen Sie diese Analyse im Detail.
- Mit "Bericht herunterladen" erzeugen Sie ein PDF der Sitzung. Sie wählen Abschnitte und Sprache und bestätigen mit "Bericht exportieren".
So analysieren Sie eine Sitzung:
1. Wenden Sie die Filter an, die Sie brauchen, um sie in der Liste zu finden.
2. Wählen Sie die Sitzung aus.
3. Prüfen Sie die Metriken und öffnen Sie jedes Diagramm mit "Details anzeigen".
4. Um die Analyse als PDF zu erhalten, klicken Sie auf "Bericht herunterladen" und wählen Sie die aufzunehmenden Daten.'),
(N'ayuda_nuevagrabacion', N'es', N'Esta es la pantalla de nueva grabación.
Desde acá registrás una sesión de seguimiento ocular: configurás los datos de la sesión, verificás el equipo y grabás mientras el tester usa el activo que evaluás.
- Con "Continuar" pasás de los datos de la sesión a la verificación del equipo.
- Con "Abrir Tobii Experience" calibrás el eye tracker. Con "Verificar nuevamente" repetís el control después de corregir un punto.
- Con "Iniciar la grabación" comienza la cuenta regresiva y luego la captura. "Pausar" y "Reanudar" la detienen y la retoman, "Reiniciar" la vuelve a empezar y "Finalizar" la termina.
- Con "Guardar" la sesión queda registrada en tu cuenta. "Descartar" no guarda nada. "Grabar otra parte" continúa la misma prueba y "Ver análisis" abre la sesión guardada.
Para grabar una sesión:
1. Elegí la categoría y el dispositivo, nombrá la sesión y cargá los datos anónimos del tester.
2. Tocá "Continuar" y verificá que los puntos obligatorios estén en verde.
3. Elegí la pantalla a grabar y el momento de inicio, y tocá "Iniciar la grabación".
4. Pasá a la aplicación que vas a evaluar y trabajá con normalidad.
5. Tocá "Finalizar", revisá el video y tocá "Guardar".'),
(N'ayuda_nuevagrabacion', N'en', N'This is the new recording screen.
From here you record an eye-tracking session: you set up the session data, check the equipment and record while the tester uses the asset you are evaluating.
- With "Continue" you move from the session data to the equipment check.
- With "Open Tobii Experience" you calibrate the eye tracker. With "Check again" you repeat the check after fixing an item.
- With "Start recording" the countdown begins and then the capture. "Pause" and "Resume" stop and continue it, "Restart" starts it over and "Finish" ends it.
- With "Save" the session is stored in your account. "Discard" saves nothing. "Record another part" continues the same test and "View analysis" opens the saved session.
To record a session:
1. Choose the category and the device, name the session and enter the tester''s anonymous data.
2. Click "Continue" and verify that the required items are green.
3. Choose the screen to record and the start moment, and click "Start recording".
4. Switch to the application you are going to evaluate and work as usual.
5. Click "Finish", review the video and click "Save".'),
(N'ayuda_nuevagrabacion', N'pt', N'Esta é a tela de nova gravação.
Daqui você registra uma sessão de rastreamento ocular: configura os dados da sessão, verifica o equipamento e grava enquanto o testador usa o ativo que você avalia.
- Com "Continuar" você passa dos dados da sessão para a verificação do equipamento.
- Com "Abrir Tobii Experience" você calibra o eye tracker. Com "Verificar novamente" você repete o controle depois de corrigir um item.
- Com "Iniciar gravação" começa a contagem regressiva e depois a captura. "Pausar" e "Retomar" a interrompem e a continuam, "Reiniciar" a recomeça e "Finalizar" a encerra.
- Com "Salvar" a sessão fica registrada na sua conta. "Descartar" não salva nada. "Gravar outra parte" continua o mesmo teste e "Ver análise" abre a sessão salva.
Para gravar uma sessão:
1. Escolha a categoria e o dispositivo, dê um nome à sessão e preencha os dados anônimos do testador.
2. Clique em "Continuar" e verifique se os itens obrigatórios estão verdes.
3. Escolha a tela a gravar e o momento de início, e clique em "Iniciar gravação".
4. Passe para o aplicativo que vai avaliar e trabalhe normalmente.
5. Clique em "Finalizar", revise o vídeo e clique em "Salvar".'),
(N'ayuda_nuevagrabacion', N'de', N'Dies ist die Seite für neue Aufnahmen.
Von hier zeichnen Sie eine Eye-Tracking-Sitzung auf: Sie konfigurieren die Sitzungsdaten, prüfen das Gerät und nehmen auf, während die Testperson das bewertete Produkt nutzt.
- Mit "Weiter" gehen Sie von den Sitzungsdaten zur Prüfung des Geräts.
- Mit "Tobii Experience öffnen" kalibrieren Sie den Eye-Tracker. Mit "Erneut prüfen" wiederholen Sie die Kontrolle, nachdem Sie einen Punkt behoben haben.
- Mit "Aufnahme starten" beginnen der Countdown und danach die Erfassung. "Pausieren" und "Fortsetzen" halten sie an und setzen sie fort, "Neu starten" beginnt von vorn und "Beenden" schließt sie ab.
- Mit "Speichern" wird die Sitzung in Ihrem Konto abgelegt. "Verwerfen" speichert nichts. "Weiteren Teil aufnehmen" setzt denselben Test fort und "Analyse ansehen" öffnet die gespeicherte Sitzung.
So zeichnen Sie eine Sitzung auf:
1. Wählen Sie Kategorie und Gerät, benennen Sie die Sitzung und geben Sie die anonymen Daten der Testperson ein.
2. Klicken Sie auf "Weiter" und prüfen Sie, ob die Pflichtpunkte grün sind.
3. Wählen Sie den aufzunehmenden Bildschirm und den Startzeitpunkt und klicken Sie auf "Aufnahme starten".
4. Wechseln Sie zu der Anwendung, die Sie bewerten, und arbeiten Sie wie gewohnt.
5. Klicken Sie auf "Beenden", prüfen Sie das Video und klicken Sie auf "Speichern".'),
(N'ayuda_descargas', N'es', N'Esta es la pantalla de instalación.
Reúne los programas que necesita la PC para registrar la mirada: el controlador del Tobii Eye Tracker 5 y el agente de Falke. Se instalan una sola vez por PC.
- Con "Descargar todo (.zip)" descargás ambos programas en un único archivo.
Requisitos: Windows 10 u 11 de 64 bits y el Tobii Eye Tracker 5 conectado por USB.
Para instalarlos:
1. Descargá el archivo con "Descargar todo (.zip)".
2. Instalá Tobii Experience y reiniciá la PC si lo solicita.
3. Conectá el Tobii Eye Tracker 5 y completá su calibración.
4. Ejecutá AgenteMirada.exe. Se instala solo y queda activo en cada inicio de Windows.
5. Abrí "Nueva grabación": la página comprueba que el agente esté activo.'),
(N'ayuda_descargas', N'en', N'This is the installation screen.
It gathers the programs the PC needs to record gaze: the Tobii Eye Tracker 5 driver and the Falke agent. They are installed once per PC.
- With "Download all (.zip)" you download both programs in a single file.
Requirements: 64-bit Windows 10 or 11 and the Tobii Eye Tracker 5 connected by USB.
To install them:
1. Download the file with "Download all (.zip)".
2. Install Tobii Experience and restart the PC if it asks you to.
3. Connect the Tobii Eye Tracker 5 and complete its calibration.
4. Run AgenteMirada.exe. It installs itself and stays active at every Windows startup.
5. Open "New recording": the page checks that the agent is active.'),
(N'ayuda_descargas', N'pt', N'Esta é a tela de instalação.
Reúne os programas de que o PC precisa para registrar o olhar: o driver do Tobii Eye Tracker 5 e o agente do Falke. Eles são instalados uma única vez por PC.
- Com "Baixar tudo (.zip)" você baixa os dois programas em um único arquivo.
Requisitos: Windows 10 ou 11 de 64 bits e o Tobii Eye Tracker 5 conectado por USB.
Para instalá-los:
1. Baixe o arquivo com "Baixar tudo (.zip)".
2. Instale o Tobii Experience e reinicie o PC se ele solicitar.
3. Conecte o Tobii Eye Tracker 5 e conclua a calibração.
4. Execute AgenteMirada.exe. Ele se instala sozinho e fica ativo a cada inicialização do Windows.
5. Abra "Nova gravação": a página verifica se o agente está ativo.'),
(N'ayuda_descargas', N'de', N'Dies ist die Installationsseite.
Sie bündelt die Programme, die der PC für die Blickaufzeichnung braucht: den Treiber des Tobii Eye Tracker 5 und den Falke-Agenten. Sie werden einmal pro PC installiert.
- Mit "Alles herunterladen (.zip)" laden Sie beide Programme in einer einzigen Datei herunter.
Voraussetzungen: Windows 10 oder 11 mit 64 Bit und der per USB angeschlossene Tobii Eye Tracker 5.
So installieren Sie sie:
1. Laden Sie die Datei mit "Alles herunterladen (.zip)" herunter.
2. Installieren Sie Tobii Experience und starten Sie den PC neu, wenn dies verlangt wird.
3. Schließen Sie den Tobii Eye Tracker 5 an und schließen Sie die Kalibrierung ab.
4. Führen Sie AgenteMirada.exe aus. Das Programm installiert sich selbst und ist bei jedem Windows-Start aktiv.
5. Öffnen Sie "Neue Aufnahme": Die Seite prüft, ob der Agent aktiv ist.'),
(N'ayuda_categorias', N'es', N'Esta es la pantalla de categorías.
Una categoría representa un activo digital de tu empresa, como una app o un videojuego, y agrupa las sesiones grabadas sobre él. Desde acá creás, buscás, editás y eliminás categorías.
- Con "Nueva categoría" abrís el formulario de alta.
- Con el buscador, "Tipo de activo" y "Mostrar desactivadas" filtrás el listado.
- Con "Ver sesiones" accedés a las sesiones grabadas sobre esa categoría.
- Si eliminás una categoría que tiene sesiones grabadas, en lugar de borrarse queda desactivada.
Para crear una categoría:
1. Tocá "Nueva categoría".
2. Escribí el nombre y elegí el tipo de activo digital.
3. Completá los campos específicos de ese tipo, como el nombre del activo evaluado y el flujo a analizar.
4. Guardá la categoría.'),
(N'ayuda_categorias', N'en', N'This is the categories screen.
A category represents a digital asset of your company, such as an app or a video game, and groups the sessions recorded on it. From here you create, search, edit and delete categories.
- With "New category" you open the creation form.
- With the search box, "Asset type" and "Show deactivated" you filter the list.
- With "View sessions" you access the sessions recorded on that category.
- If you delete a category that has recorded sessions, it is deactivated instead of removed.
To create a category:
1. Click "New category".
2. Type the name and choose the type of digital asset.
3. Fill in the fields specific to that type, such as the name of the evaluated asset and the flow to analyze.
4. Save the category.'),
(N'ayuda_categorias', N'pt', N'Esta é a tela de categorias.
Uma categoria representa um ativo digital da sua empresa, como um aplicativo ou um videogame, e agrupa as sessões gravadas sobre ele. Daqui você cria, busca, edita e exclui categorias.
- Com "Nova categoria" você abre o formulário de cadastro.
- Com a busca, "Tipo de ativo" e "Mostrar desativadas" você filtra a lista.
- Com "Ver sessões" você acessa as sessões gravadas sobre essa categoria.
- Se você excluir uma categoria que tem sessões gravadas, em vez de ser apagada ela fica desativada.
Para criar uma categoria:
1. Clique em "Nova categoria".
2. Digite o nome e escolha o tipo de ativo digital.
3. Preencha os campos específicos desse tipo, como o nome do ativo avaliado e o fluxo a analisar.
4. Salve a categoria.'),
(N'ayuda_categorias', N'de', N'Dies ist die Seite der Kategorien.
Eine Kategorie steht für ein digitales Produkt Ihres Unternehmens, etwa eine App oder ein Videospiel, und fasst die dazu aufgezeichneten Sitzungen zusammen. Hier legen Sie Kategorien an, suchen, bearbeiten und löschen sie.
- Mit "Neue Kategorie" öffnen Sie das Formular zum Anlegen.
- Mit der Suche, "Asset-Typ" und "Deaktivierte anzeigen" filtern Sie die Liste.
- Mit "Sitzungen anzeigen" greifen Sie auf die zu dieser Kategorie aufgezeichneten Sitzungen zu.
- Wenn Sie eine Kategorie mit aufgezeichneten Sitzungen löschen, wird sie deaktiviert statt entfernt.
So legen Sie eine Kategorie an:
1. Klicken Sie auf "Neue Kategorie".
2. Geben Sie den Namen ein und wählen Sie den Typ des digitalen Produkts.
3. Füllen Sie die Felder aus, die für diesen Typ gelten, etwa den Namen des bewerteten Produkts und den zu analysierenden Ablauf.
4. Speichern Sie die Kategorie.'),
(N'ayuda_analisismultiple', N'es', N'Esta es la pantalla de análisis múltiple.
Combina varias sesiones para promediar los resultados e identificar qué patrones se repiten entre distintos testers. Las sesiones deben ser de la misma categoría y se necesitan al menos dos.
- Con "Seleccionar todos los de esta categoría" marcás todas las sesiones de esa categoría.
- Con "Generar análisis" obtenés el resultado combinado con sus gráficos promediados.
- Con "Regenerar datos" recalculás el resultado después de modificar la selección.
- Con "Descargar reporte" generás un PDF. Elegís el contenido y el idioma, y confirmás con "Exportar reporte".
Para generar un análisis:
1. Seleccioná una sesión del listado. Las de otras categorías se deshabilitan.
2. Agregá al menos otra sesión, o usá "Seleccionar todos los de esta categoría".
3. Tocá "Generar análisis".
4. Para excluir una sesión, desmarcala y tocá "Regenerar datos".
5. Para obtener el resultado en PDF, tocá "Descargar reporte".'),
(N'ayuda_analisismultiple', N'en', N'This is the multiple analysis screen.
It combines several sessions to average the results and identify which patterns repeat across different testers. The sessions must belong to the same category and at least two are needed.
- With "Select all in this category" you mark every session of that category.
- With "Generate analysis" you get the combined result with its averaged charts.
- With "Regenerate data" you recalculate the result after changing the selection.
- With "Download report" you generate a PDF. You choose the content and the language, and confirm with "Export report".
To generate an analysis:
1. Select a session from the list. Sessions from other categories are disabled.
2. Add at least one more session, or use "Select all in this category".
3. Click "Generate analysis".
4. To exclude a session, uncheck it and click "Regenerate data".
5. To get the result as a PDF, click "Download report".'),
(N'ayuda_analisismultiple', N'pt', N'Esta é a tela de análise múltipla.
Combina várias sessões para calcular a média dos resultados e identificar quais padrões se repetem entre diferentes testadores. As sessões devem ser da mesma categoria e são necessárias pelo menos duas.
- Com "Selecionar todas desta categoria" você marca todas as sessões dessa categoria.
- Com "Gerar análise" você obtém o resultado combinado com seus gráficos médios.
- Com "Regenerar dados" você recalcula o resultado depois de alterar a seleção.
- Com "Baixar relatório" você gera um PDF. Escolhe o conteúdo e o idioma, e confirma com "Exportar relatório".
Para gerar uma análise:
1. Selecione uma sessão da lista. As de outras categorias ficam desabilitadas.
2. Adicione pelo menos mais uma sessão, ou use "Selecionar todas desta categoria".
3. Clique em "Gerar análise".
4. Para excluir uma sessão, desmarque-a e clique em "Regenerar dados".
5. Para obter o resultado em PDF, clique em "Baixar relatório".'),
(N'ayuda_analisismultiple', N'de', N'Dies ist die Seite der Mehrfachanalyse.
Sie kombiniert mehrere Sitzungen, um die Ergebnisse zu mitteln und zu erkennen, welche Muster sich bei verschiedenen Testpersonen wiederholen. Die Sitzungen müssen zur selben Kategorie gehören und es werden mindestens zwei benötigt.
- Mit "Alle dieser Kategorie auswählen" markieren Sie alle Sitzungen dieser Kategorie.
- Mit "Analyse erstellen" erhalten Sie das kombinierte Ergebnis mit seinen gemittelten Diagrammen.
- Mit "Daten neu erzeugen" berechnen Sie das Ergebnis nach einer Änderung der Auswahl neu.
- Mit "Bericht herunterladen" erzeugen Sie ein PDF. Sie wählen Inhalt und Sprache und bestätigen mit "Bericht exportieren".
So erstellen Sie eine Analyse:
1. Wählen Sie eine Sitzung aus der Liste. Sitzungen anderer Kategorien sind deaktiviert.
2. Fügen Sie mindestens eine weitere Sitzung hinzu oder nutzen Sie "Alle dieser Kategorie auswählen".
3. Klicken Sie auf "Analyse erstellen".
4. Um eine Sitzung auszuschließen, deaktivieren Sie sie und klicken Sie auf "Daten neu erzeugen".
5. Um das Ergebnis als PDF zu erhalten, klicken Sie auf "Bericht herunterladen".'),
(N'ayuda_mistobiis', N'es', N'Esta es la pantalla de Mis Tobiis.
Muestra los eye trackers de tu empresa y reúne las herramientas para prepararlos antes de grabar.
- Con "Abrir Tobii Experience" abrís el programa del fabricante.
- Con "Recalibrar Tobii" mejorás la calibración del dispositivo.
- Con "Calibrar pantalla" configurás el eye tracker para tu monitor.
- Con "Previsualizar mirada" verificás dónde registra la mirada el dispositivo.
Si aparece el aviso de que no se encontró el agente de Falke, verificá que esté instalado y en ejecución. Se instala desde "Instalación".
Para preparar el Tobii antes de grabar:
1. Tocá "Recalibrar Tobii" y seguí las indicaciones de Tobii Experience.
2. Si cambiaste de monitor, tocá "Calibrar pantalla".
3. Tocá "Previsualizar mirada" y verificá que el punto acompañe tu mirada.'),
(N'ayuda_mistobiis', N'en', N'This is the My Tobiis screen.
It shows your company''s eye trackers and gathers the tools to prepare them before recording.
- With "Open Tobii Experience" you open the manufacturer''s program.
- With "Recalibrate Tobii" you improve the device''s calibration.
- With "Set up screen" you configure the eye tracker for your monitor.
- With "Preview gaze" you verify where the device is registering your gaze.
If a notice says the Falke agent was not found, verify that it is installed and running. It is installed from "Installation".
To prepare the Tobii before recording:
1. Click "Recalibrate Tobii" and follow the instructions in Tobii Experience.
2. If you changed monitors, click "Set up screen".
3. Click "Preview gaze" and verify that the dot follows your gaze.'),
(N'ayuda_mistobiis', N'pt', N'Esta é a tela de Meus Tobiis.
Mostra os eye trackers da sua empresa e reúne as ferramentas para prepará-los antes de gravar.
- Com "Abrir Tobii Experience" você abre o programa do fabricante.
- Com "Recalibrar Tobii" você melhora a calibração do dispositivo.
- Com "Configurar tela" você configura o eye tracker para o seu monitor.
- Com "Pré-visualizar olhar" você verifica onde o dispositivo registra o olhar.
Se aparecer o aviso de que o agente do Falke não foi encontrado, verifique se ele está instalado e em execução. Ele é instalado em "Instalação".
Para preparar o Tobii antes de gravar:
1. Clique em "Recalibrar Tobii" e siga as instruções do Tobii Experience.
2. Se você trocou de monitor, clique em "Configurar tela".
3. Clique em "Pré-visualizar olhar" e verifique se o ponto acompanha o seu olhar.'),
(N'ayuda_mistobiis', N'de', N'Dies ist die Seite Meine Tobiis.
Sie zeigt die Eye-Tracker Ihres Unternehmens und bündelt die Werkzeuge, um sie vor der Aufnahme vorzubereiten.
- Mit "Tobii Experience öffnen" starten Sie das Programm des Herstellers.
- Mit "Tobii neu kalibrieren" verbessern Sie die Kalibrierung des Geräts.
- Mit "Bildschirm einrichten" konfigurieren Sie den Eye-Tracker für Ihren Monitor.
- Mit "Blick Vorschau" prüfen Sie, wo das Gerät Ihren Blick erfasst.
Wenn ein Hinweis meldet, dass der Falke-Agent nicht gefunden wurde, prüfen Sie, ob er installiert ist und läuft. Er wird unter "Installation" installiert.
So bereiten Sie den Tobii vor der Aufnahme vor:
1. Klicken Sie auf "Tobii neu kalibrieren" und folgen Sie den Anweisungen in Tobii Experience.
2. Wenn Sie den Monitor gewechselt haben, klicken Sie auf "Bildschirm einrichten".
3. Klicken Sie auf "Blick Vorschau" und prüfen Sie, ob der Punkt Ihrem Blick folgt.'),
(N'ayuda_usuarios', N'es', N'Esta es la pantalla de usuarios de tu empresa.
Lista a los integrantes con su rol y su estado. Si administrás la cuenta, desde acá invitás usuarios nuevos y gestionás los existentes. Si no, consultás el listado y editás tus propios datos.
- Con "Invitar usuario" das de alta a una persona. Recibe por correo un enlace para activar su cuenta.
- Con "Gestionar", en la fila de un usuario, cambiás el estado de su cuenta o su rol.
- Con "Editar datos" modificás nombre, apellido e idioma de la interfaz.
- Con "Reenviar invitación" enviás un nuevo enlace de activación a quien todavía no activó su cuenta.
- Con el buscador y los filtros de empresa, rol y estado acotás el listado.
Para invitar a un usuario:
1. Tocá "Invitar usuario".
2. Completá los datos de la persona, incluido su correo electrónico.
3. Enviá la invitación.
4. La persona recibe un enlace por correo para definir su contraseña.'),
(N'ayuda_usuarios', N'en', N'This is the users screen for your company.
It lists the members with their role and status. If you manage the account, from here you invite new users and manage the existing ones. If not, you view the list and edit your own details.
- With "Invite user" you register a person. They receive a link by email to activate their account.
- With "Manage", on a user''s row, you change the account status or the role.
- With "Edit details" you change the first name, last name and interface language.
- With "Resend invitation" you send a new activation link to someone who has not yet activated their account.
- With the search box and the company, role and status filters you narrow the list.
To invite a user:
1. Click "Invite user".
2. Fill in the person''s details, including their email address.
3. Send the invitation.
4. The person receives a link by email to set their password.'),
(N'ayuda_usuarios', N'pt', N'Esta é a tela de usuários da sua empresa.
Lista os integrantes com sua função e seu status. Se você administra a conta, daqui convida novos usuários e gerencia os existentes. Se não, consulta a lista e edita seus próprios dados.
- Com "Convidar usuário" você cadastra uma pessoa. Ela recebe por e-mail um link para ativar a conta.
- Com "Gerenciar", na linha de um usuário, você altera o status da conta ou a função.
- Com "Editar dados" você altera nome, sobrenome e idioma da interface.
- Com "Reenviar convite" você envia um novo link de ativação a quem ainda não ativou a conta.
- Com a busca e os filtros de empresa, função e status você reduz a lista.
Para convidar um usuário:
1. Clique em "Convidar usuário".
2. Preencha os dados da pessoa, incluindo o e-mail.
3. Envie o convite.
4. A pessoa recebe um link por e-mail para definir a senha.'),
(N'ayuda_usuarios', N'de', N'Dies ist die Benutzerseite Ihres Unternehmens.
Sie listet die Mitglieder mit Rolle und Status auf. Wenn Sie das Konto verwalten, laden Sie von hier neue Benutzer ein und verwalten die vorhandenen. Andernfalls sehen Sie die Liste und bearbeiten Ihre eigenen Daten.
- Mit "Benutzer einladen" legen Sie eine Person an. Sie erhält per E-Mail einen Link zur Aktivierung ihres Kontos.
- Mit "Verwalten" in der Zeile eines Benutzers ändern Sie den Kontostatus oder die Rolle.
- Mit "Daten bearbeiten" ändern Sie Vorname, Nachname und Oberflächensprache.
- Mit "Einladung erneut senden" schicken Sie einen neuen Aktivierungslink an jemanden, der sein Konto noch nicht aktiviert hat.
- Mit der Suche und den Filtern für Unternehmen, Rolle und Status grenzen Sie die Liste ein.
So laden Sie einen Benutzer ein:
1. Klicken Sie auf "Benutzer einladen".
2. Füllen Sie die Daten der Person aus, einschließlich ihrer E-Mail-Adresse.
3. Senden Sie die Einladung.
4. Die Person erhält per E-Mail einen Link, um ihr Passwort festzulegen.'),
(N'ayuda_miempresa', N'es', N'Esta es la pantalla de tu empresa.
Muestra los datos de la empresa y los dispositivos que tiene en préstamo. Podés actualizar el rubro, el domicilio y el teléfono de contacto. La razón social, el CUIT, el plan y la facturación los administra Pattern Blue: si alguno cambió, comunicalo y se actualiza.
- Con "Editar contacto" habilitás los campos modificables.
- Con "Ver empleados" accedés al listado de usuarios de tu empresa.
Para actualizar los datos de contacto:
1. Tocá "Editar contacto".
2. Modificá el rubro, el domicilio o el teléfono.
3. Guardá los cambios.'),
(N'ayuda_miempresa', N'en', N'This is your company screen.
It shows the company''s details and the devices it has on loan. You can update the industry, address and contact phone. Pattern Blue manages the company name, tax ID, plan and billing: if any of them changed, let us know and it will be updated.
- With "Edit contact" you enable the editable fields.
- With "View employees" you access your company''s user list.
To update the contact details:
1. Click "Edit contact".
2. Change the industry, address or phone.
3. Save the changes.'),
(N'ayuda_miempresa', N'pt', N'Esta é a tela da sua empresa.
Mostra os dados da empresa e os dispositivos que ela tem em empréstimo. Você pode atualizar o ramo, o endereço e o telefone de contato. A razão social, o CUIT, o plano e o faturamento são administrados pela Pattern Blue: se algum mudou, comunique e será atualizado.
- Com "Editar contato" você habilita os campos editáveis.
- Com "Ver funcionários" você acessa a lista de usuários da sua empresa.
Para atualizar os dados de contato:
1. Clique em "Editar contato".
2. Altere o ramo, o endereço ou o telefone.
3. Salve as alterações.'),
(N'ayuda_miempresa', N'de', N'Dies ist die Seite Ihres Unternehmens.
Sie zeigt die Daten des Unternehmens und die Geräte, die es ausgeliehen hat. Sie können Branche, Adresse und Kontakttelefon aktualisieren. Firmenname, Steuernummer, Tarif und Abrechnung verwaltet Pattern Blue: Hat sich eines davon geändert, teilen Sie es mit und es wird aktualisiert.
- Mit "Kontakt bearbeiten" aktivieren Sie die bearbeitbaren Felder.
- Mit "Mitarbeiter anzeigen" greifen Sie auf die Benutzerliste Ihres Unternehmens zu.
So aktualisieren Sie die Kontaktdaten:
1. Klicken Sie auf "Kontakt bearbeiten".
2. Ändern Sie Branche, Adresse oder Telefon.
3. Speichern Sie die Änderungen.'),
(N'ayuda_empresas', N'es', N'Esta es la pantalla de empresas cliente.
Es la cartera de empresas que contrataron Falke, con su plan y su estado. Desde acá buscás una empresa, consultás su detalle, editás sus datos y cambiás su estado.
- Con "Todas", "Activas", "Bloqueadas" y "Dadas de baja" filtrás el listado por estado.
- Con "Nueva empresa" registrás una empresa cliente.
- El detalle de cada empresa tiene solapas de datos, plan, equipo y sesiones, y accesos a sus dispositivos y usuarios.
- Con "Editar" modificás los datos. Cambiar la razón social, el CUIT o el plan requiere indicar un motivo y queda registrado en la bitácora.
Para cambiar el estado de una empresa:
1. Buscá la empresa y abrí su detalle.
2. Tocá el botón de estado de la empresa.
3. Elegí el nuevo estado e indicá el motivo, que es obligatorio.
4. Confirmá el cambio.'),
(N'ayuda_empresas', N'en', N'This is the client companies screen.
It is the portfolio of companies that contracted Falke, with their plan and status. From here you search for a company, view its detail, edit its data and change its status.
- With "All", "Active", "Blocked" and "Deactivated" you filter the list by status.
- With "New company" you register a client company.
- Each company''s detail has tabs for data, plan, team and sessions, and links to its devices and users.
- With "Edit" you change the data. Changing the company name, tax ID or plan requires a reason and is recorded in the audit log.
To change a company''s status:
1. Find the company and open its detail.
2. Click the company status button.
3. Choose the new status and enter the reason, which is required.
4. Confirm the change.'),
(N'ayuda_empresas', N'pt', N'Esta é a tela de empresas cliente.
É a carteira de empresas que contrataram o Falke, com seu plano e seu status. Daqui você busca uma empresa, consulta o detalhe, edita os dados e altera o status.
- Com "Todas", "Ativa", "Bloqueada" e "Desativada" você filtra a lista por status.
- Com "Nova empresa" você cadastra uma empresa cliente.
- O detalhe de cada empresa tem abas de dados, plano, equipe e sessões, e acessos aos seus dispositivos e usuários.
- Com "Editar" você altera os dados. Mudar a razão social, o CUIT ou o plano exige informar um motivo e fica registrado no registro de auditoria.
Para alterar o status de uma empresa:
1. Busque a empresa e abra o detalhe.
2. Clique no botão de status da empresa.
3. Escolha o novo status e informe o motivo, que é obrigatório.
4. Confirme a alteração.'),
(N'ayuda_empresas', N'de', N'Dies ist die Seite der Kundenunternehmen.
Sie ist das Portfolio der Unternehmen, die Falke beauftragt haben, mit Tarif und Status. Von hier suchen Sie ein Unternehmen, sehen seine Details, bearbeiten seine Daten und ändern seinen Status.
- Mit "Alle", "Aktiv", "Gesperrt" und "Deaktiviert" filtern Sie die Liste nach Status.
- Mit "Neues Unternehmen" registrieren Sie ein Kundenunternehmen.
- Die Details jedes Unternehmens haben Reiter für Daten, Tarif, Team und Sitzungen sowie Zugänge zu seinen Geräten und Benutzern.
- Mit "Bearbeiten" ändern Sie die Daten. Eine Änderung von Firmenname, Steuernummer oder Tarif erfordert einen Grund und wird im Audit-Protokoll festgehalten.
So ändern Sie den Status eines Unternehmens:
1. Suchen Sie das Unternehmen und öffnen Sie seine Details.
2. Klicken Sie auf die Schaltfläche für den Status des Unternehmens.
3. Wählen Sie den neuen Status und geben Sie den Grund an, der Pflicht ist.
4. Bestätigen Sie die Änderung.'),
(N'ayuda_empresanueva', N'es', N'Esta es la pantalla de registro de empresas cliente.
La usa el gestor de Pattern Blue una vez formalizado el acuerdo comercial. Se cargan los datos de la empresa y los de su administrador inicial, que recibe un enlace por correo para activar su cuenta.
- Al registrar, se crean juntos la empresa y su administrador inicial.
- El CUIT tiene 11 dígitos, con o sin guiones. Los datos fiscales se cargan tal como figuran en la factura.
Para registrar una empresa:
1. Completá los datos de la empresa: razón social, CUIT, rubro, domicilio, plan de suscripción, facturación y número de contacto.
2. Completá nombre, apellido y correo electrónico del administrador inicial.
3. Registrá la empresa.
4. Si el correo de activación no se pudo enviar, reenviá la invitación más tarde desde "Usuarios".'),
(N'ayuda_empresanueva', N'en', N'This is the client company registration screen.
Pattern Blue''s manager uses it once the commercial agreement is formalized. You enter the company''s data and its initial administrator''s, who receives a link by email to activate the account.
- When you register, the company and its initial administrator are created together.
- The tax ID has 11 digits, with or without hyphens. Tax data is entered exactly as it appears on the invoice.
To register a company:
1. Fill in the company data: company name, tax ID, industry, address, subscription plan, billing and contact number.
2. Fill in the first name, last name and email of the initial administrator.
3. Register the company.
4. If the activation email could not be sent, resend the invitation later from "Users".'),
(N'ayuda_empresanueva', N'pt', N'Esta é a tela de cadastro de empresas cliente.
O gestor da Pattern Blue a usa depois de formalizado o acordo comercial. Preenchem-se os dados da empresa e os do administrador inicial, que recebe um link por e-mail para ativar a conta.
- Ao cadastrar, a empresa e o administrador inicial são criados juntos.
- O CUIT tem 11 dígitos, com ou sem hífens. Os dados fiscais são preenchidos exatamente como constam na nota fiscal.
Para cadastrar uma empresa:
1. Preencha os dados da empresa: razão social, CUIT, ramo, endereço, plano de assinatura, faturamento e número de contato.
2. Preencha nome, sobrenome e e-mail do administrador inicial.
3. Cadastre a empresa.
4. Se o e-mail de ativação não pôde ser enviado, reenvie o convite mais tarde em "Usuários".'),
(N'ayuda_empresanueva', N'de', N'Dies ist die Seite zur Registrierung von Kundenunternehmen.
Der Manager von Pattern Blue nutzt sie, sobald die Geschäftsvereinbarung formalisiert ist. Erfasst werden die Daten des Unternehmens und die seines ersten Administrators, der per E-Mail einen Link zur Aktivierung seines Kontos erhält.
- Bei der Registrierung werden das Unternehmen und sein erster Administrator gemeinsam angelegt.
- Die Steuernummer hat 11 Ziffern, mit oder ohne Bindestriche. Die steuerlichen Daten werden so eingetragen, wie sie auf der Rechnung stehen.
So registrieren Sie ein Unternehmen:
1. Füllen Sie die Unternehmensdaten aus: Firmenname, Steuernummer, Branche, Adresse, Abo-Tarif, Abrechnung und Kontaktnummer.
2. Füllen Sie Vorname, Nachname und E-Mail des ersten Administrators aus.
3. Registrieren Sie das Unternehmen.
4. Konnte die Aktivierungs-E-Mail nicht gesendet werden, senden Sie die Einladung später unter "Benutzer" erneut.'),
(N'ayuda_dispositivos', N'es', N'Esta es la pantalla de dispositivos.
Es el inventario de eye trackers e indica a qué empresa está prestado cada uno. Desde acá das de alta equipos, los prestás, registrás devoluciones y das de baja los que ya no se usan.
- Con "Modelos" administrás los modelos de dispositivo. Un modelo no se borra: al darlo de baja deja de ofrecerse para equipos nuevos.
- Con "Alta de dispositivo" incorporás un equipo al inventario como disponible. El número de serie es el de fábrica y no se modifica después.
- Con "Asignar a empresa" prestás un dispositivo disponible a una empresa activa.
- Con "Registrar devolución" cerrás el préstamo. Si el equipo vuelve con fallas, marcalo para enviarlo a mantenimiento.
- Con "Dar de baja" retirás el dispositivo. El motivo es obligatorio.
Para prestar un dispositivo:
1. Tocá "Asignar a empresa".
2. Elegí el dispositivo disponible y la empresa.
3. Confirmá. La fecha de entrega es la del día y el préstamo continúa hasta que registres la devolución.'),
(N'ayuda_dispositivos', N'en', N'This is the devices screen.
It is the eye tracker inventory and shows which company each one is loaned to. From here you add devices, lend them, record returns and retire the ones no longer in use.
- With "Models" you manage the device models. A model is never deleted: once retired, it stops being offered for new devices.
- With "Add device" you add a unit to the inventory as available. The serial number is the factory one and is not changed afterwards.
- With "Assign to company" you lend an available device to an active company.
- With "Record return" you close the loan. If the device comes back faulty, mark it to send it to maintenance.
- With "Retire" you take the device out of service. The reason is required.
To lend a device:
1. Click "Assign to company".
2. Choose the available device and the company.
3. Confirm. The delivery date is today and the loan continues until you record the return.'),
(N'ayuda_dispositivos', N'pt', N'Esta é a tela de dispositivos.
É o inventário de eye trackers e indica a qual empresa cada um está emprestado. Daqui você cadastra equipamentos, empresta, registra devoluções e dá baixa nos que não são mais usados.
- Com "Modelos" você administra os modelos de dispositivo. Um modelo não é apagado: ao dar baixa, deixa de ser oferecido para equipamentos novos.
- Com "Adicionar dispositivo" você incorpora um equipamento ao inventário como disponível. O número de série é o de fábrica e não é alterado depois.
- Com "Atribuir a uma empresa" você empresta um dispositivo disponível a uma empresa ativa.
- Com "Registrar devolução" você encerra o empréstimo. Se o equipamento volta com falhas, marque-o para enviar à manutenção.
- Com "Baixar" você retira o dispositivo. O motivo é obrigatório.
Para emprestar um dispositivo:
1. Clique em "Atribuir a uma empresa".
2. Escolha o dispositivo disponível e a empresa.
3. Confirme. A data de entrega é a do dia e o empréstimo continua até você registrar a devolução.'),
(N'ayuda_dispositivos', N'de', N'Dies ist die Geräteseite.
Sie ist das Inventar der Eye-Tracker und zeigt, an welches Unternehmen jedes Gerät verliehen ist. Von hier erfassen Sie Geräte, verleihen sie, buchen Rückgaben und scheiden nicht mehr genutzte Geräte aus.
- Mit "Modelle" verwalten Sie die Gerätemodelle. Ein Modell wird nie gelöscht: Nach der Ausmusterung wird es für neue Geräte nicht mehr angeboten.
- Mit "Gerät hinzufügen" nehmen Sie ein Gerät als verfügbar ins Inventar auf. Die Seriennummer ist die des Herstellers und wird später nicht geändert.
- Mit "Einem Unternehmen zuweisen" verleihen Sie ein verfügbares Gerät an ein aktives Unternehmen.
- Mit "Rückgabe erfassen" beenden Sie den Verleih. Kommt das Gerät mit Defekten zurück, markieren Sie es für die Wartung.
- Mit "Ausmustern" nehmen Sie das Gerät außer Betrieb. Der Grund ist Pflicht.
So verleihen Sie ein Gerät:
1. Klicken Sie auf "Einem Unternehmen zuweisen".
2. Wählen Sie das verfügbare Gerät und das Unternehmen.
3. Bestätigen Sie. Das Übergabedatum ist das heutige und der Verleih dauert bis zur erfassten Rückgabe.'),
(N'ayuda_bitacora', N'es', N'Esta es la pantalla de la bitácora.
Registra las acciones relevantes del sistema: quién las realizó, cuándo, en qué módulo y con qué criticidad. Los registros no se pueden editar ni eliminar.
- Con "Desde", "Hasta", "Hora desde" y "Hora hasta" definís el período.
- Con "Módulo", "Acción", "Usuario", "Empresa" y "Criticidad" filtrás por tipo de evento o por persona.
- Con "Anterior" y "Siguiente" navegás entre las páginas de resultados.
Para buscar un evento:
1. Definí el período con fechas y horas.
2. Filtrá por módulo, acción, usuario, empresa o criticidad.
3. Revisá los resultados.
4. Si hay más de una página, avanzá con "Siguiente".'),
(N'ayuda_bitacora', N'en', N'This is the audit log screen.
It records the relevant actions in the system: who performed them, when, in which module and with what severity. Records cannot be edited or deleted.
- With "From", "To", "Time from" and "Time to" you define the period.
- With "Module", "Action", "User", "Company" and "Severity" you filter by event type or by person.
- With "Previous" and "Next" you move between result pages.
To find an event:
1. Define the period with dates and times.
2. Filter by module, action, user, company or severity.
3. Review the results.
4. If there is more than one page, advance with "Next".'),
(N'ayuda_bitacora', N'pt', N'Esta é a tela do registro de auditoria.
Registra as ações relevantes do sistema: quem as realizou, quando, em qual módulo e com qual criticidade. Os registros não podem ser editados nem excluídos.
- Com "De", "Até", "Hora de" e "Hora até" você define o período.
- Com "Módulo", "Ação", "Usuário", "Empresa" e "Criticidade" você filtra por tipo de evento ou por pessoa.
- Com "Anterior" e "Próxima" você navega entre as páginas de resultados.
Para buscar um evento:
1. Defina o período com datas e horas.
2. Filtre por módulo, ação, usuário, empresa ou criticidade.
3. Revise os resultados.
4. Se houver mais de uma página, avance com "Próxima".'),
(N'ayuda_bitacora', N'de', N'Dies ist die Seite des Audit-Protokolls.
Es hält die relevanten Aktionen im System fest: wer sie ausgeführt hat, wann, in welchem Modul und mit welchem Schweregrad. Einträge lassen sich weder bearbeiten noch löschen.
- Mit "Von", "Bis", "Uhrzeit von" und "Uhrzeit bis" legen Sie den Zeitraum fest.
- Mit "Modul", "Aktion", "Benutzer", "Unternehmen" und "Schweregrad" filtern Sie nach Ereignisart oder Person.
- Mit "Zurück" und "Weiter" wechseln Sie zwischen den Ergebnisseiten.
So finden Sie ein Ereignis:
1. Legen Sie den Zeitraum mit Datum und Uhrzeit fest.
2. Filtern Sie nach Modul, Aktion, Benutzer, Unternehmen oder Schweregrad.
3. Prüfen Sie die Ergebnisse.
4. Bei mehr als einer Seite blättern Sie mit "Weiter" vor.'),
(N'ayuda_integridad', N'es', N'Esta es la pantalla de integridad de la base de datos.
Verifica que los datos no hayan sido alterados, comparando los dígitos verificadores de cada registro (DVH) y de cada tabla (DVV). El sistema realiza este control al iniciar sesión. Desde acá consultás el resultado por tabla y, si corresponde, recalculás.
- Con "Ver detalle" en cada tabla consultás sus registros y los que no coinciden.
- Con "Recalcular dígitos verificadores" los generás nuevamente con los datos actuales. Hay que indicar el alcance y el motivo.
Recalcular toma como válidos los datos actuales, por lo que conviene hacerlo solo después de revisar qué cambió. Mientras existan inconsistencias, el resto de los usuarios no puede ingresar al sistema.
Para revisar y corregir la integridad:
1. Consultá el estado de cada tabla.
2. Si hay inconsistencias, abrí "Ver detalle" para identificar los registros que no coinciden.
3. Si el cambio es legítimo, tocá "Recalcular dígitos verificadores".
4. Indicá el alcance y el motivo, y confirmá.'),
(N'ayuda_integridad', N'en', N'This is the database integrity screen.
It verifies that the data has not been altered, by comparing the check digits of each record (DVH) and of each table (DVV). The system runs this check at sign-in. From here you view the result per table and, if needed, recalculate.
- With "View details" on each table you view its records and the ones that do not match.
- With "Recalculate check digits" you generate them again from the current data. You must indicate the scope and the reason.
Recalculating takes the current data as valid, so do it only after reviewing what changed. While inconsistencies exist, the other users cannot enter the system.
To review and correct integrity:
1. Check the status of each table.
2. If there are inconsistencies, open "View details" to identify the records that do not match.
3. If the change is legitimate, click "Recalculate check digits".
4. Indicate the scope and the reason, and confirm.'),
(N'ayuda_integridad', N'pt', N'Esta é a tela de integridade do banco de dados.
Verifica se os dados não foram alterados, comparando os dígitos verificadores de cada registro (DVH) e de cada tabela (DVV). O sistema faz esse controle ao iniciar sessão. Daqui você consulta o resultado por tabela e, se for o caso, recalcula.
- Com "Ver detalhes" em cada tabela você consulta seus registros e os que não coincidem.
- Com "Recalcular dígitos verificadores" você os gera de novo com os dados atuais. É preciso informar o alcance e o motivo.
Recalcular toma como válidos os dados atuais, por isso convém fazê-lo só depois de revisar o que mudou. Enquanto houver inconsistências, os demais usuários não conseguem entrar no sistema.
Para revisar e corrigir a integridade:
1. Consulte o status de cada tabela.
2. Se houver inconsistências, abra "Ver detalhes" para identificar os registros que não coincidem.
3. Se a alteração for legítima, clique em "Recalcular dígitos verificadores".
4. Informe o alcance e o motivo, e confirme.'),
(N'ayuda_integridad', N'de', N'Dies ist die Seite zur Integrität der Datenbank.
Sie prüft, ob die Daten verändert wurden, indem die Prüfziffern jedes Datensatzes (DVH) und jeder Tabelle (DVV) verglichen werden. Das System führt diese Kontrolle bei der Anmeldung durch. Von hier sehen Sie das Ergebnis pro Tabelle und berechnen bei Bedarf neu.
- Mit "Details anzeigen" bei einer Tabelle sehen Sie ihre Datensätze und die, die nicht übereinstimmen.
- Mit "Prüfziffern neu berechnen" erzeugen Sie sie aus den aktuellen Daten neu. Umfang und Grund sind anzugeben.
Beim Neuberechnen gelten die aktuellen Daten als gültig. Tun Sie es daher erst, nachdem Sie geprüft haben, was sich geändert hat. Solange Unstimmigkeiten bestehen, können die übrigen Benutzer nicht ins System.
So prüfen und korrigieren Sie die Integrität:
1. Prüfen Sie den Status jeder Tabelle.
2. Bei Unstimmigkeiten öffnen Sie "Details anzeigen", um die nicht übereinstimmenden Datensätze zu erkennen.
3. Ist die Änderung berechtigt, klicken Sie auf "Prüfziffern neu berechnen".
4. Geben Sie Umfang und Grund an und bestätigen Sie.'),
(N'ayuda_roles', N'es', N'Esta es la pantalla de roles y permisos.
Un rol reúne grupos y permisos. Los grupos ordenan permisos y pueden contener otros grupos. Los permisos los define el sistema y desde acá solo se combinan.
- Con "Nuevo rol" creás un rol y con "Nuevo grupo" creás un grupo.
- Con "Eliminar" borrás el rol o grupo seleccionado.
- Al seleccionar un elemento ves qué incluye, sus permisos efectivos y en qué otros está incluido.
- Gestor y Webmaster son roles fijos: no se puede editar su composición ni eliminarlos.
Para crear un rol:
1. Tocá "Nuevo rol".
2. Escribí el nombre y elegí el tipo de rol.
3. Marcá los grupos y permisos que incluye.
4. Guardá el rol.'),
(N'ayuda_roles', N'en', N'This is the roles and permissions screen.
A role gathers groups and permissions. Groups organize permissions and can contain other groups. The system defines the permissions and from here they are only combined.
- With "New role" you create a role and with "New group" you create a group.
- With "Delete" you remove the selected role or group.
- When you select an item you see what it includes, its effective permissions and where else it is included.
- Gestor and Webmaster are fixed roles: their composition cannot be edited and they cannot be deleted.
To create a role:
1. Click "New role".
2. Type the name and choose the role type.
3. Check the groups and permissions it includes.
4. Save the role.'),
(N'ayuda_roles', N'pt', N'Esta é a tela de funções e permissões.
Uma função reúne grupos e permissões. Os grupos organizam permissões e podem conter outros grupos. As permissões são definidas pelo sistema e daqui apenas são combinadas.
- Com "Nova função" você cria uma função e com "Novo grupo" você cria um grupo.
- Com "Excluir" você remove a função ou o grupo selecionado.
- Ao selecionar um elemento, você vê o que ele inclui, suas permissões efetivas e em que outros está incluído.
- Gestor e Webmaster são funções fixas: sua composição não pode ser editada e elas não podem ser excluídas.
Para criar uma função:
1. Clique em "Nova função".
2. Digite o nome e escolha o tipo de função.
3. Marque os grupos e as permissões que ela inclui.
4. Salve a função.'),
(N'ayuda_roles', N'de', N'Dies ist die Seite für Rollen und Berechtigungen.
Eine Rolle fasst Gruppen und Berechtigungen zusammen. Gruppen ordnen Berechtigungen und können andere Gruppen enthalten. Die Berechtigungen legt das System fest und sie werden von hier nur kombiniert.
- Mit "Neue Rolle" legen Sie eine Rolle an und mit "Neue Gruppe" eine Gruppe.
- Mit "Löschen" entfernen Sie die ausgewählte Rolle oder Gruppe.
- Wenn Sie ein Element auswählen, sehen Sie, was es enthält, seine wirksamen Berechtigungen und wo es sonst enthalten ist.
- Gestor und Webmaster sind feste Rollen: Ihre Zusammensetzung lässt sich nicht bearbeiten und sie lassen sich nicht löschen.
So legen Sie eine Rolle an:
1. Klicken Sie auf "Neue Rolle".
2. Geben Sie den Namen ein und wählen Sie die Rollenart.
3. Markieren Sie die Gruppen und Berechtigungen, die sie enthält.
4. Speichern Sie die Rolle.'),
(N'ayuda_respaldos', N'es', N'Esta es la pantalla de respaldos.
Administra las copias de seguridad de la base de datos. Quedan almacenadas en el servidor y no se descargan desde acá.
- Con "Generar respaldo" creás una copia de la base en ese momento.
- Cada respaldo del listado tiene un botón para restaurarlo.
Restaurar reemplaza toda la base por el estado de la copia: lo cargado después se pierde. Al confirmar, se cierran todas las sesiones, incluida la tuya.
Para restaurar un respaldo:
1. Elegí el respaldo en el listado y tocá su botón de restauración.
2. Leé el aviso con lo que se va a perder.
3. Escribí RESTAURAR para confirmar.
4. Tocá "Restaurar y cerrar sesiones".
5. Al terminar, volvé a ingresar y revisá la pantalla de integridad.'),
(N'ayuda_respaldos', N'en', N'This is the backups screen.
It manages the database backups. They are stored on the server and are not downloaded from here.
- With "Generate backup" you create a copy of the database at that moment.
- Each backup in the list has a button to restore it.
Restoring replaces the whole database with the state of the copy: anything loaded afterward is lost. When you confirm, all sessions are closed, including yours.
To restore a backup:
1. Choose the backup in the list and click its restore button.
2. Read the notice about what will be lost.
3. Type RESTAURAR to confirm.
4. Click "Restore and close sessions".
5. When it finishes, sign in again and check the integrity screen.'),
(N'ayuda_respaldos', N'pt', N'Esta é a tela de backups.
Administra as cópias de segurança do banco de dados. Elas ficam armazenadas no servidor e não são baixadas daqui.
- Com "Gerar backup" você cria uma cópia do banco nesse momento.
- Cada backup da lista tem um botão para restaurá-lo.
Restaurar substitui todo o banco pelo estado da cópia: o que foi carregado depois se perde. Ao confirmar, todas as sessões são encerradas, inclusive a sua.
Para restaurar um backup:
1. Escolha o backup na lista e clique no botão de restauração.
2. Leia o aviso sobre o que será perdido.
3. Digite RESTAURAR para confirmar.
4. Clique em "Restaurar e encerrar as sessões".
5. Ao terminar, entre de novo e revise a tela de integridade.'),
(N'ayuda_respaldos', N'de', N'Dies ist die Seite der Sicherungen.
Hier werden die Datenbanksicherungen verwaltet. Sie sind auf dem Server gespeichert und werden von hier nicht heruntergeladen.
- Mit "Backup erstellen" legen Sie in diesem Moment eine Kopie der Datenbank an.
- Jede Sicherung in der Liste hat eine Schaltfläche zum Wiederherstellen.
Beim Wiederherstellen wird die gesamte Datenbank durch den Stand der Kopie ersetzt: Was danach erfasst wurde, geht verloren. Mit der Bestätigung werden alle Sitzungen beendet, auch Ihre.
So stellen Sie eine Sicherung wieder her:
1. Wählen Sie die Sicherung in der Liste und klicken Sie auf ihre Schaltfläche zum Wiederherstellen.
2. Lesen Sie den Hinweis darauf, was verloren geht.
3. Geben Sie zur Bestätigung RESTAURAR ein.
4. Klicken Sie auf "Wiederherstellen und Sitzungen beenden".
5. Melden Sie sich danach neu an und prüfen Sie die Integritätsseite.'),
(N'ayuda_idiomas', N'es', N'Esta es la pantalla de idiomas.
Muestra los idiomas de la interfaz y sus traducciones. Desde acá agregás idiomas, editás traducciones, activás o desactivás un idioma y lo eliminás.
- Con "Agregar idioma" creás uno nuevo. Si elegís un paquete incluido, se cargan sus traducciones. Si lo creás desde cero, tenés que cargar las traducciones a mano.
- Con "Traducciones" abrís los textos de un idioma junto al texto base. Al editar una traducción, queda marcada como manual.
- Con "Solo pendientes" filtrás los textos sin traducir.
- Con "Habilitar" y "Deshabilitar" definís si el idioma está disponible en la aplicación.
Para editar una traducción:
1. Tocá "Traducciones" en el idioma que querés modificar.
2. Buscá el texto o activá "Solo pendientes".
3. Abrí el texto y escribí la traducción. Conservá los marcadores como {0} o {1}.
4. Guardá. Si la dejás vacía, queda pendiente y se muestra el texto base.'),
(N'ayuda_idiomas', N'en', N'This is the languages screen.
It shows the interface languages and their translations. From here you add languages, edit translations, enable or disable a language and delete it.
- With "Add language" you create a new one. If you choose a bundled package, its translations are loaded. If you create it from scratch, you must enter the translations by hand.
- With "Translations" you open a language''s texts next to the base text. When you edit a translation, it is marked as manual.
- With "Pending only" you filter the untranslated texts.
- With "Enable" and "Disable" you define whether the language is available in the application.
To edit a translation:
1. Click "Translations" on the language you want to change.
2. Find the text or turn on "Pending only".
3. Open the text and type the translation. Keep placeholders such as {0} or {1}.
4. Save. If you leave it empty, it stays pending and the base text is shown.'),
(N'ayuda_idiomas', N'pt', N'Esta é a tela de idiomas.
Mostra os idiomas da interface e suas traduções. Daqui você adiciona idiomas, edita traduções, ativa ou desativa um idioma e o exclui.
- Com "Adicionar idioma" você cria um novo. Se escolher um pacote incluído, suas traduções são carregadas. Se criá-lo do zero, você precisa inserir as traduções manualmente.
- Com "Traduções" você abre os textos de um idioma ao lado do texto base. Ao editar uma tradução, ela fica marcada como manual.
- Com "Só pendentes" você filtra os textos sem tradução.
- Com "Ativar" e "Desativar" você define se o idioma está disponível no aplicativo.
Para editar uma tradução:
1. Clique em "Traduções" no idioma que quer alterar.
2. Procure o texto ou ative "Só pendentes".
3. Abra o texto e digite a tradução. Mantenha marcadores como {0} ou {1}.
4. Salve. Se deixá-la vazia, ela fica pendente e o texto base é exibido.'),
(N'ayuda_idiomas', N'de', N'Dies ist die Seite der Sprachen.
Sie zeigt die Sprachen der Oberfläche und ihre Übersetzungen. Von hier fügen Sie Sprachen hinzu, bearbeiten Übersetzungen, aktivieren oder deaktivieren eine Sprache und löschen sie.
- Mit "Sprache hinzufügen" legen Sie eine neue an. Wählen Sie ein mitgeliefertes Paket, werden seine Übersetzungen geladen. Legen Sie sie von Grund auf an, müssen Sie die Übersetzungen von Hand eintragen.
- Mit "Übersetzungen" öffnen Sie die Texte einer Sprache neben dem Basistext. Eine bearbeitete Übersetzung wird als manuell markiert.
- Mit "Nur ausstehende" filtern Sie die noch nicht übersetzten Texte.
- Mit "Aktivieren" und "Deaktivieren" legen Sie fest, ob die Sprache in der Anwendung verfügbar ist.
So bearbeiten Sie eine Übersetzung:
1. Klicken Sie bei der Sprache, die Sie ändern möchten, auf "Übersetzungen".
2. Suchen Sie den Text oder aktivieren Sie "Nur ausstehende".
3. Öffnen Sie den Text und geben Sie die Übersetzung ein. Behalten Sie Platzhalter wie {0} oder {1} bei.
4. Speichern Sie. Lassen Sie sie leer, bleibt sie ausstehend und der Basistext wird angezeigt.'),
(N'ayuda_miperfil', N'es', N'Esta es la pantalla de tu perfil.
Muestra los datos de tu cuenta. Podés modificar nombre, apellido e idioma de la interfaz. El correo es tu identificador de acceso y no se cambia desde acá: para modificarlo, pedíselo a quien administra tu cuenta.
- Con "Cambiar mi contraseña" accedés a la pantalla para definir una nueva.
Con el acceso de emergencia no hay datos personales para editar.
Para actualizar tus datos:
1. Modificá nombre, apellido o idioma de la interfaz.
2. Guardá los cambios.'),
(N'ayuda_miperfil', N'en', N'This is your profile screen.
It shows your account details. You can change your first name, last name and interface language. The email is your sign-in identifier and is not changed from here: to change it, ask whoever manages your account.
- With "Change my password" you access the screen to set a new one.
With the emergency access there is no personal data to edit.
To update your details:
1. Change your first name, last name or interface language.
2. Save the changes.'),
(N'ayuda_miperfil', N'pt', N'Esta é a tela do seu perfil.
Mostra os dados da sua conta. Você pode alterar nome, sobrenome e idioma da interface. O e-mail é o seu identificador de acesso e não é alterado por aqui: para modificá-lo, peça a quem administra a sua conta.
- Com "Alterar minha senha" você acessa a tela para definir uma nova.
Com o acesso de emergência não há dados pessoais para editar.
Para atualizar seus dados:
1. Altere nome, sobrenome ou idioma da interface.
2. Salve as alterações.'),
(N'ayuda_miperfil', N'de', N'Dies ist die Seite Ihres Profils.
Sie zeigt die Daten Ihres Kontos. Sie können Vorname, Nachname und Oberflächensprache ändern. Die E-Mail-Adresse ist Ihre Zugangskennung und wird hier nicht geändert: Um sie zu ändern, wenden Sie sich an die Person, die Ihr Konto verwaltet.
- Mit "Mein Passwort ändern" gelangen Sie zur Seite, auf der Sie ein neues festlegen.
Beim Notfallzugang gibt es keine persönlichen Daten zu bearbeiten.
So aktualisieren Sie Ihre Daten:
1. Ändern Sie Vorname, Nachname oder Oberflächensprache.
2. Speichern Sie die Änderungen.'),
(N'ayuda_miclave', N'es', N'Esta es la pantalla para cambiar tu contraseña.
Solicita la contraseña actual y la nueva. Al cambiarla, se cierran las sesiones abiertas en otros equipos y hay que volver a ingresar en ellos.
- Con el ícono del ojo mostrás u ocultás lo que escribís.
- Con "Volver a mi perfil" salís sin modificar nada.
Para cambiar la contraseña:
1. Escribí la contraseña actual.
2. Escribí la nueva. Debe tener al menos 8 caracteres, con mayúscula, minúscula, número y un carácter especial.
3. Repetí la contraseña nueva.
4. Guardá el cambio.'),
(N'ayuda_miclave', N'en', N'This is the screen to change your password.
It asks for the current password and the new one. When you change it, sessions open on other computers are closed and you have to sign in again on them.
- With the eye icon you show or hide what you type.
- With "Back to my profile" you leave without changing anything.
To change the password:
1. Type the current password.
2. Type the new one. It must have at least 8 characters, with an uppercase letter, a lowercase letter, a number and a special character.
3. Repeat the new password.
4. Save the change.'),
(N'ayuda_miclave', N'pt', N'Esta é a tela para alterar a sua senha.
Solicita a senha atual e a nova. Ao alterá-la, as sessões abertas em outros computadores são encerradas e é preciso entrar de novo neles.
- Com o ícone do olho você mostra ou oculta o que digita.
- Com "Voltar ao meu perfil" você sai sem alterar nada.
Para alterar a senha:
1. Digite a senha atual.
2. Digite a nova. Ela deve ter pelo menos 8 caracteres, com maiúscula, minúscula, número e um caractere especial.
3. Repita a senha nova.
4. Salve a alteração.'),
(N'ayuda_miclave', N'de', N'Auf dieser Seite ändern Sie Ihr Passwort.
Sie verlangt das aktuelle und das neue Passwort. Nach der Änderung werden Sitzungen auf anderen Rechnern beendet und Sie müssen sich dort neu anmelden.
- Mit dem Augensymbol blenden Sie Ihre Eingabe ein oder aus.
- Mit "Zurück zu meinem Profil" verlassen Sie die Seite, ohne etwas zu ändern.
So ändern Sie das Passwort:
1. Geben Sie das aktuelle Passwort ein.
2. Geben Sie das neue ein. Es braucht mindestens 8 Zeichen mit Großbuchstabe, Kleinbuchstabe, Ziffer und Sonderzeichen.
3. Wiederholen Sie das neue Passwort.
4. Speichern Sie die Änderung.');

UPDATE tr SET tr.texto_traduccion = t.texto, tr.pendiente_traduccion = 0, tr.origen_traduccion = 'resx'
FROM dbo.TraduccionTable tr
JOIN dbo.IdiomaTable i ON i.id_idioma = tr.id_idioma
JOIN @t t ON t.codigo = i.codigo_idioma AND t.clave = tr.codigo_clave_traduccion;

INSERT INTO dbo.TraduccionTable (codigo_clave_traduccion, id_idioma, texto_traduccion, pendiente_traduccion, origen_traduccion)
SELECT t.clave, i.id_idioma, t.texto, 0, 'resx'
FROM @t t
JOIN dbo.IdiomaTable i ON i.codigo_idioma = t.codigo
WHERE NOT EXISTS (SELECT 1 FROM dbo.TraduccionTable tr WHERE tr.codigo_clave_traduccion = t.clave AND tr.id_idioma = i.id_idioma);
GO

PRINT 'Listo: textos de la ayuda en linea cargados.';
GO
