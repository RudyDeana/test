@echo off
echo =================
echo   FAKE VIRUS ?
echo =================

echo Creating advanced virus executable...
echo.

:: Compile basic version
echo [1/3] Compiling basic version...
csc /target:winexe /out:nFire_Basic_Virus.exe FakeVirus.cs

:: Compile advanced version  
echo [2/3] Compiling advanced version...
csc /target:winexe /out:nFire_Advanced_Virus.exe AdvancedFakeVirus.cs

:: Create boot screen simulator
echo [3/3] Compiling boot screen simulator...
csc /target:winexe /out:nFire_Boot_Simulator.exe BootScreenSimulator.cs

if exist nFire_Basic_Virus.exe (
    echo.
    echo ✅ Basic version compiled successfully!
)

if exist nFire_Advanced_Virus.exe (
    echo ✅ Advanced version compiled successfully!
)

if exist nFire_Boot_Simulator.exe (
    echo ✅ Boot simulator compiled successfully!
)

echo.
echo 🔥 nFire Virus Collection Ready! 🔥
echo.
echo Available executables:
echo • nFire_Basic_Virus.exe - Versione base
echo • nFire_Advanced_Virus.exe - Versione avanzata  
echo • nFire_Boot_Simulator.exe - Simulatore boot
echo.
echo Premi ESC durante l'esecuzione per uscire.
echo.
pause

echo Quale versione vuoi eseguire?
echo [1] Basic Virus
echo [2] Advanced Virus  
echo [3] Boot Simulator
echo [4] Esci
echo.
set /p choice="Scegli (1-4): "

if "%choice%"=="1" start nFire_Basic_Virus.exe
if "%choice%"=="2" start nFire_Advanced_Virus.exe  
if "%choice%"=="3" start nFire_Boot_Simulator.exe
if "%choice%"=="4" exit