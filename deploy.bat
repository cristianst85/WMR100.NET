@echo off

rem Script to automatically deploy build files to test server.

set PROJECT_PATH=%~dp0
set DEPLOY_PATH=K:\devel\wmr100.net

cd /d "%PROJECT_PATH%"

IF EXIST %DEPLOY_PATH% (
	del /S /Q %DEPLOY_PATH%\*.*
	copy /Y .\build\*.* %DEPLOY_PATH%\*.*
)

echo Done.

exit;
rem pause