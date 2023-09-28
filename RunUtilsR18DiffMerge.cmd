@echo off
if "%1"=="" (
    start /B /WAIT "" cmd /k "%~f0" aaa
)
set "PATH=%cd%\2236Utils\bin\Debug\net6.0\;%PATH%"

set "projFile=%~dp0\projectR18.json"

2236utils.exe diffmerge backup\message.uni.diff.json project.json backup\project.uni.json backup\project.r18.json projectR18.json

