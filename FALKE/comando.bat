@echo off
setlocal enabledelayedexpansion

set "OUTPUT=archivos_cs.txt"

:: Crear/vaciar archivo de salida
> "%OUTPUT%" echo.

:: Recorrer todos los .cs de la carpeta y subcarpetas
for /r %%F in (*.cs) do (
    echo =====inicio de %%~nxF=====>>"%OUTPUT%"
    type "%%F">>"%OUTPUT%"
    echo.>>"%OUTPUT%"
    echo =====fin de %%~nxF=====>>"%OUTPUT%"
    echo.>>"%OUTPUT%"
)

echo.
echo Listo. Se genero: %OUTPUT%
pause