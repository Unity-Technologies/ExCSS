@ECHO OFF

dotnet build "ExCSS.sln" -c Release
if ERRORLEVEL 1 exit 1
dotnet test "ExCSS.sln" -c Release
if ERRORLEVEL 1 exit 1

ECHO.
ECHO ==[Prepare DLLs for zipping]==
ECHO.
ECHO [Clear .\builds dir]
rmdir /S /Q .\builds
if ERRORLEVEL 1 exit 1

ECHO [Remove old builds.zip]
del /Q .\builds.zip

ECHO [Make .\builds\lib\netstandard2.0 dir]
mkdir .\builds\lib\netstandard2.0 
if ERRORLEVEL 1 exit 1

ECHO [Copy files to .\builds\lib\netstandard2.0 dir]
del .\src\ExCSS\bin\Release\netstandard2.0\ExCSS.Unity.deps.json
copy .\src\ExCSS\bin\Release\netstandard2.0\*.* .\builds\lib\netstandard2.0
if ERRORLEVEL 1 exit 1
