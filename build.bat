@echo off

set PROJECT_PATH=%~dp0
set SOURCE_PATH=%PROJECT_PATH%WMR100.NET.ConsoleDemo\

cd /d "%PROJECT_PATH%"

IF NOT EXIST build (
	mkdir build
)

del /S /Q .\build\*.*

copy /Y .\WMR100.NET.ConsoleDemo\bin\Debug\*.exe .\build\
copy /Y .\WMR100.NET.ConsoleDemo\bin\Debug\*.dll .\build\
copy /Y .\WMR100.NET.ConsoleDemo\bin\Debug\*.pdb .\build\

echo Cleaning up...
del .\build\*vshost.exe

echo Done.

exit;
rem pause