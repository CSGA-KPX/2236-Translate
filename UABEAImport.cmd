@echo off

set asset=G:\SteamApps\steamapps\common\2236ad\2236 A.D. -Universal Edition-\2236 A.D. -Universal Edition-_Data\StreamingAssets\2236\Windows\2236.scenarios.asset
set UABEA=K:\Source\Repos\2236-Translate\uabea-windows\UABEAvalonia.exe
set rebuildDir=K:\Source\Repos\2236-Translate\Repacked2

IF NOT EXIST "%rebuildDir%\2236.scenarios.asset" (copy backup\2236.scenarios.asset "%rebuildDir%\")

"%UABEA%" batchimportbundle -md %rebuildDir%

copy /B /Y "%rebuildDir%\2236.scenarios.asset" "%asset%"

echo 清除缓存

rd /S "%USERPROFILE%\AppData\LocalLow\Unity\Chloro_2236 A_D_ -Universal Edition-"

pause