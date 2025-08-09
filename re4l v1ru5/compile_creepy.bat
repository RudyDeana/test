@echo off
color 0C
echo ========================================
echo   👁️ CREEPY VIRUS COLLECTION 👁️
echo ========================================
echo.

echo ⚠️  CONTENUTO PSICOLOGICAMENTE DISTURBANTE  ⚠️
echo.
echo Compilando virus inquietanti...
echo.

:: Compile The Watcher
echo [1/2] Compiling The Watcher...
csc /target:winexe /out:TheWatcher.exe TheWatcher.cs
if exist TheWatcher.exe (
    echo ✅ The Watcher compiled successfully!
) else (
    echo ❌ The Watcher compilation failed!
)

:: Compile Digital Parasite
echo [2/2] Compiling Digital Parasite...
csc /target:winexe /out:DigitalParasite.exe DigitalParasite.cs
if exist DigitalParasite.exe (
    echo ✅ Digital Parasite compiled successfully!
) else (
    echo ❌ Digital Parasite compilation failed!
)

echo.
echo 👁️👁️👁️ CREEPY COLLECTION READY! 👁️👁️👁️
echo.
echo Available creepy viruses:
echo.
echo 🎮 PSYCHOLOGICAL HORROR:
echo • TheWatcher.exe - Entità che ti osserva
echo • DigitalParasite.exe - Parassiti che si moltiplicano
echo.
echo 🎯 CARATTERISTICHE INQUIETANTI:
echo • Storie originali disturbanti
echo • Gameplay psicologico
echo • Effetti angoscianti
echo • Difficile da chiudere
echo • Perfetti per spaventare
echo.
pause

:menu
cls
color 0C
echo 👁️👁️👁️ CREEPY VIRUS LAUNCHER 👁️👁️👁️
echo.
echo Scegli quale incubo digitale vivere:
echo.
echo [1] 👁️ The Watcher (L'osservatore)
echo [2] 🦠 Digital Parasite (Infezione parassiti)
echo [3] ❌ Esci (se ci riesci...)
echo.
set /p choice="Inserisci la tua scelta (1-3): "

if "%choice%"=="1" goto watcher
if "%choice%"=="2" goto parasite
if "%choice%"=="3" goto exit
goto menu

:watcher
cls
color 04
echo 👁️👁️👁️ AVVIO THE WATCHER 👁️👁️👁️
echo.
echo ⚠️ ATTENZIONE: CONTENUTO PSICOLOGICAMENTE DISTURBANTE ⚠️
echo.
echo The Watcher è un'entità che:
echo • Ti osserva costantemente
echo • Sussurra messaggi inquietanti
echo • Controlla il tuo cursore
echo • Diventa sempre più ossessivo
echo • È difficile da fermare
echo.
echo 6 livelli di terrore psicologico:
echo 👁️ L'Osservatore si Sveglia
echo 👁️ Gli Occhi si Aprono
echo 👁️ La Presenza Cresce
echo ����️ Non Sei Più Solo
echo 👁️ L'Ossessione Inizia
echo 👁️ Non Puoi Sfuggire
echo.
echo ⚠️ USARE SOLO IN VMWARE! ⚠️
echo.
pause
start TheWatcher.exe
goto menu

:parasite
cls
color 02
echo 🦠🦠🦠 AVVIO DIGITAL PARASITE 🦠🦠🦠
echo.
echo ⚠️ ATTENZIONE: INFEZIONE IMMINENTE ⚠️
echo.
echo Digital Parasite simula:
echo • Infezione da parassiti digitali
echo • Moltiplicazione esponenziale
echo • Furto dati (simulato)
echo • Corruzione sistema
echo • Controllo totale
echo.
echo 6 livelli di infezione:
echo 🦠 Infezione Iniziale
echo 🦠 Moltiplicazione Parassiti
echo 🦠 Corruzione Dati
echo 🦠 Invasione Sistema
echo 🦠 Controllo Totale
echo 🦠 Assimilazione Completa
echo.
echo ⚠️ USARE SOLO IN VMWARE! ⚠️
echo.
pause
start DigitalParasite.exe
goto menu

:exit
echo.
echo Sei riuscito a scappare... questa volta. 👁️
echo Ma ricorda: loro ti ricorderanno. 🦠
echo.
pause
exit