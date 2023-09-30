@echo off
if "%1"=="" (
    start /B /WAIT "" cmd /k "%~f0" aaa
)
set "PATH=%cd%\2236Utils\bin\Debug\net6.0\;%PATH%"

set "projFile=%~dp0\project.json"
2236utils.exe repack backup\asset\steam\2236.scenarios.asset build\steam\2236.scenarios.asset

set "projFile=%~dp0\projectR18.json"
2236utils.exe repack backup\asset\dlsite\2236.scenarios.asset build\dlsite\2236.scenarios.asset