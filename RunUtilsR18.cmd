@echo off
if "%1"=="" (
    start /B /WAIT "" cmd /k "%~f0" aaa
)
set "PATH=%cd%\2236Utils\bin\Debug\net6.0\;%PATH%"

set "projFile=%~dp0\projectR18.json"

2236utils.exe

