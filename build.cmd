@echo off
pushd %~dp0
setlocal

if exist bin goto Build
mkdir bin

:Build

REM Find one of the Visual Studio 2017/2019/2022 installations.
REM Also handle x86 operating systems, where %ProgramFiles(x86)% is not defined. Always quote the
REM %MSBuild% value when setting the variable and never quote %MSBuild% references.
set MSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
if not exist %MSBuild% @set MSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
if not exist %MSBuild% @set MSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
if not exist %MSBuild% @set MSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
if not exist %MSBuild% @set MSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist %MSBuild% @set MSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" 
if not exist %MSBuild% @set MSBuild="%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
if not exist %MSBuild% @set MSBuild="%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
if not exist %MSBuild% @set MSBuild="%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

if "%1" == "" goto BuildDefaults

%MSBuild% ODataConnectedService.msbuild /m /nr:false /t:%* /p:Platform="Any CPU" /p:Desktop=true /v:M /fl /flp:LogFile=bin\msbuild.log;Verbosity=Normal
if %ERRORLEVEL% neq 0 goto BuildFail
goto BuildSuccess

:BuildDefaults
%MSBuild% ODataConnectedService.msbuild /m /nr:false /p:Platform="Any CPU" /p:Desktop=true /v:M /fl /flp:LogFile=bin\msbuild.log;Verbosity=Normal
if %ERRORLEVEL% neq 0 goto BuildFail
goto BuildSuccess

:BuildFail
echo.
echo *** BUILD FAILED ***
goto End

:BuildSuccess
echo.
echo **** BUILD SUCCESSFUL ***
goto End

:End
popd
endlocal
