@echo off
echo ========================================
echo    nFire Educational Virus Compiler
echo ========================================
echo.

echo Compiling the educational virus...
dotnet build --configuration Release

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ Compilation successful!
    echo.
    pause
    
    cd bin\Release\net6.0-windows
    start FakeVirus.exe
) else (
    echo.
    echo ❌ Compilation failed!
    echo Make sure you have .NET 6.0 SDK installed.
    echo.
    pause
)