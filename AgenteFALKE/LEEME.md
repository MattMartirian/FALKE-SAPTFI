# AgenteFALKE

Un solo archivo, `AgenteMirada.exe`, que se instala solo en la PC del usuario. Corre sin ventana, inicia con la sesion de Windows, lee la mirada del Tobii Eye Tracker 5 y la guarda en un archivo con marcas de tiempo mientras la pagina de Falke lo pide. Antes de grabar, la pagina consulta `/estado` para comprobar que el agente esta activo y el tracker conectado. Al terminar, le pide el archivo y lo sube al servidor junto con el video.

## Compilar
Abrir `AgenteFALKE.csproj` en Visual Studio y compilar en **Release | x64**. `bin\x64\Release\AgenteMirada.exe` ya lleva adentro `tobii_stream_engine.dll` (se toma de esta carpeta). Ese es el unico archivo a entregar; va en el servidor como `AgenteMirada.exe`.

## Instalar (usuario)
Ejecutar `AgenteMirada.exe`. Se copia a `%LOCALAPPDATA%\FALKE\Agente`, se registra para iniciar con Windows (sin administrador), se arranca y avisa con un mensaje. Volver a ejecutarlo actualiza la instalacion.

## Desinstalar
`AgenteMirada.exe --desinstalar` (tambien desde la copia instalada). Detiene el agente, quita el inicio automatico y borra la carpeta.

## Probar sin instalar
- `AgenteMirada.exe --local`: corre en el lugar, con el Tobii.
- `AgenteMirada.exe --sim`: corre en el lugar con mirada simulada, sin aparato.
Para detenerlo, terminar `AgenteMirada.exe` desde el Administrador de tareas. Los problemas se anotan en `%LOCALAPPDATA%\FALKE\agente.log`.

## Requisitos en la PC del usuario
Windows 10/11 de 64 bits, Tobii Experience instalado con el Eye Tracker 5 calibrado y conectado.

## Archivos
- `Program.cs`: decide si instalar o correr, y arranca todo; evita dos copias.
- `Instalador.cs`: instala, desinstala y saca la dll de adentro del exe.
- `FuenteTobii.cs` / `Interop.cs`: lectura del Tobii. `FuenteSimulada.cs`: mirada de prueba. `IFuenteMirada.cs`: lo que ambas fuentes entregan.
- `Grabador.cs`: escribe el archivo.
- `ServidorAgente.cs`: HTTP en `localhost:8887`. Responde a cualquier pagina que lo consulte.
- `Registro.cs`: anota errores en el log.

## Interfaz (HTTP)
- `GET /estado` -> `{"estado":"conectado","conectado":true,"simulado":false,"grabando":false,"muestras":0}` (`muestras` cuenta las guardadas en la grabacion en curso)
- `GET /iniciar` -> empieza a guardar. `{"inicio_utc":1793830000000}`
- El archivo guarda 10 muestras por segundo (una cada 100 ms), aunque el Tobii entregue mas.
- `GET /detener` -> `{"inicio_utc":...,"fin_utc":...,"muestras":N}`
- `GET /tobii` -> abre la aplicacion Tobii Experience (no se puede abrir una pantalla puntual como la calibracion).
- `GET /archivo` -> el archivo (409 si sigue grabando)

## Formato del archivo (un unico JSON en `%TEMP%\falke-mirada.json`)
Las muestras se juntan en memoria y el archivo se escribe al detener; si la grabacion se interrumpe antes, no queda archivo.
```
{"v":1,"inicio_utc":1793830000000,"fin_utc":1793830060000,"muestras":[
  {"u":1793830000033,"t":123456,"x":0.5123,"y":0.4301,"ok":1}
]}
```
`u` = milisegundos Unix de esta PC (mismo reloj que usa la pagina para el video); `t` = microsegundos del dispositivo; `x`,`y` relativos a la pantalla (0 a 1); `ok` = 0 si el ojo no se detecto.
