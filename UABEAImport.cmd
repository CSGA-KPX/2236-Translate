@echo off

set asset=G:\SteamApps\steamapps\common\2236ad\2236 A.D. -Universal Edition-\2236 A.D. -Universal Edition-_Data\StreamingAssets\2236\Windows\2236.scenarios.asset
set UABEA=%CD%\uabea-windows\UABEAvalonia.exe
set rebuildDir=%CD%\Repacked

IF NOT EXIST "%rebuildDir%\2236.scenarios.asset" (copy backup\2236.scenarios.asset "%rebuildDir%\")

"%UABEA%" batchimportbundle -md %rebuildDir%

copy /B /Y "%rebuildDir%\2236.scenarios.asset" "%asset%"

echo Çå³ý»º´æ

rd /S "%USERPROFILE%\AppData\LocalLow\Unity\Chloro_2236 A_D_ -Universal Edition-"

pause