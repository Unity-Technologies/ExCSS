@ECHO OFF

ECHO.
ECHO ==[Restoring packages]==
ECHO.
set nuGetDir=%~dp0.nuget
call "%nuGetDir%\nuget.exe" restore "%~dp0ExCSS.Unity.sln"
if ERRORLEVEL 1 exit 1

ECHO.
ECHO ==[Building library]==
ECHO.
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319
call "%msBuildDir%\msbuild.exe" ExCSS.Unity.sln  /p:Configuration=Release /l:FileLogger,Microsoft.Build.Engine;logfile=Manual_MSBuild_ReleaseVersion_LOG.log
if ERRORLEVEL 1 exit 1

ECHO.
ECHO ==[Run Debug unit tests]==
ECHO.
call .\nunit-console\nunit3-console.exe .\ExCSS.Tests\bin\DebugUnity35\ExCSS.Tests.dll
if ERRORLEVEL 1 exit 1

ECHO.
ECHO ==[Run Release unit tests]==
ECHO.
call .\nunit-console\nunit3-console.exe .\ExCSS.Tests\bin\ReleaseUnity35\ExCSS.Tests.dll
if ERRORLEVEL 1 exit 1

ECHO.
ECHO ==[Prepare DLLs for zipping]==
ECHO.
ECHO [Clear .\builds dir]
rmdir /S /Q .\builds
if ERRORLEVEL 1 exit 1

ECHO [Remove old builds.zip]
del /Q .\builds.zip

ECHO [Make .\builds\lib\net35 dir]
mkdir .\builds\lib\net35
if ERRORLEVEL 1 exit 1

ECHO [Copy files to .\builds\lib\net35 dir]
copy .\ExCSS\bin\ReleaseUnity35\*.* .\builds\lib\net35
if ERRORLEVEL 1 exit 1
