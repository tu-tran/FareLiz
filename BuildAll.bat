@set NET_FRAMEWORK="%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe"

@del /F /S /Q *.log > nul 2>&1
@set PLATFORM=x86

:StartBuild
@set OUTPUT_DIR=%CD%\Bin\%PLATFORM%
@set LOG_FILE=Release.%PLATFORM%.log
@echo =========================================================
@echo PROCESSING %PLATFORM% BUILD...
@echo =========================================================
@echo Cleaning up %PLATFORM% Release...
@del /F /S /Q "%OUTPUT_DIR%\*" > nul 2>&1

@echo Building %PLATFORM% Release...
@%NET_FRAMEWORK% FareLiz.sln /t:Rebuild /p:Configuration=Release /p:Platform=%PLATFORM% > "%LOG_FILE%" 2>&1
@echo Exit code %ERRORLEVEL% >> "%LOG_FILE%"
@IF %ERRORLEVEL% NEQ 0 GOTO  Error

@IF %PLATFORM%==x86 (
	@set PLATFORM=x64
	GOTO StartBuild
)

@IF %ERRORLEVEL% EQU 0 GOTO  End

:Error
@echo An error occured. Process halted!
@pause

:End