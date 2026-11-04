# AgenteFALKE

Programa de consola que corre en la PC del usuario: lee el Tobii Eye Tracker 5 y publica la mirada en `ws://localhost:8887/mirada`.
La pagina de Falke se conecta a ese WebSocket. Compila aparte del sitio.

## Compilar
1. Abrir `AgenteFALKE.csproj` en Visual Studio y compilar en **Release | x64**. Genera `AgenteMirada.exe`.
2. Sin el SDK de Tobii compila igual, pero solo funciona el modo simulado.

## Activar el Tobii real (una vez)
1. Descargar el SDK Stream Engine de Tobii y copiar su carpeta dentro de este proyecto como `tobii\` (con el `Interop.cs`).
2. Copiar `tobii_stream_engine.dll` (x64) junto al `.csproj`. Se copia a la salida en cada compilacion.
3. Si el compilador marca un tipo no encontrado en `FuenteTobii.cs` (`tobii_error_t`, `tobii_gaze_point_t`, `tobii_validity_t`, `timestamp_us`), ajustar el nombre a como figura en el `Interop.cs` del SDK.
4. Si la dll no se encuentra al ejecutar, apuntar la constante `stream_engine_dll` del `Interop.cs` a `tobii_stream_engine`.

## Ejecutar
- `IniciarAgente.bat`: Tobii real. Editar `ORIGEN` con la direccion del sitio de Falke.
- `IniciarSimulado.bat`: mirada simulada, para probar sin aparato.

## Zip para los usuarios
`AgenteMirada.exe`, `tobii_stream_engine.dll`, `IniciarAgente.bat`. Va en el servidor como `AgenteMirada.zip`.

## Mensajes (WebSocket, JSON, version 1)
- `{"v":1,"tipo":"mirada","x":0.5123,"y":0.4301,"valida":true,"t":123456,"utc":1793830000000}`
  `x` e `y` relativos a la pantalla (0 a 1); `t` en microsegundos del dispositivo; `utc` en milisegundos Unix de esta PC.
- `{"v":1,"tipo":"estado","estado":"conectado"}` (`conectado`, `conectado_simulado`, `sin_tracker`, `error_sdk`, `iniciando`).
