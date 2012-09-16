@ECHO OFF

REM The following directory is for .NET 4.0
set DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%DOTNETFX4%

echo Installing OSAEService...
echo ---------------------------------------------------
InstallUtil /i OSAEService.exe
echo ---------------------------------------------------
echo Done.