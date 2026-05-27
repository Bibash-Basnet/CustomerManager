@echo off
REM Customer Manager Installer Batch Script
REM This script copies files to Program Files and creates shortcuts

setlocal enabledelayedexpansion

REM Define paths
set APP_NAME=Customer Manager
set APP_VERSION=1.0.0
set INSTALL_PATH=%ProgramFiles%\Customer Manager
set SOURCE_PATH=D:\Learning\CustomerManager\CustomerManager\bin\Release\net10.0-windows\win-x64\publish

echo.
echo ===================================
echo %APP_NAME% v%APP_VERSION% Installer
echo ===================================
echo.

REM Check if running as admin
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: This installer requires administrator privileges!
    echo Please right-click and select "Run as administrator"
    pause
    exit /b 1
)

REM Create installation directory
if not exist "%INSTALL_PATH%" (
    echo Creating installation directory...
    mkdir "%INSTALL_PATH%"
) else (
    echo Installation directory already exists. Backing up old version...
    if exist "%INSTALL_PATH%\backup" rmdir /s /q "%INSTALL_PATH%\backup"
    mkdir "%INSTALL_PATH%\backup"
    move "%INSTALL_PATH%\*.*" "%INSTALL_PATH%\backup\" >nul 2>&1
)

REM Copy application files
echo Installing files...
xcopy "%SOURCE_PATH%\*.*" "%INSTALL_PATH%\" /E /I /Y >nul 2>&1

if %errorLevel% neq 0 (
    echo ERROR: Failed to copy files!
    pause
    exit /b 1
)

REM Create Start Menu shortcut
echo Creating Start Menu shortcut...
set SHORTCUT_PATH=%APPDATA%\Microsoft\Windows\Start Menu\Programs\%APP_NAME%.lnk

powershell -Command ^
    "$WshShell = New-Object -ComObject WScript.Shell;" ^
    "$Shortcut = $WshShell.CreateShortcut('%APPDATA%\Microsoft\Windows\Start Menu\Programs\%APP_NAME%.lnk');" ^
    "$Shortcut.TargetPath = '%INSTALL_PATH%\CustomerManager.exe';" ^
    "$Shortcut.WorkingDirectory = '%INSTALL_PATH%';" ^
    "$Shortcut.Description = '%APP_NAME% v%APP_VERSION%';" ^
    "$Shortcut.Save();"

REM Create Desktop shortcut
echo Creating Desktop shortcut...
powershell -Command ^
    "$WshShell = New-Object -ComObject WScript.Shell;" ^
    "$Shortcut = $WshShell.CreateShortcut('%USERPROFILE%\Desktop\%APP_NAME%.lnk');" ^
    "$Shortcut.TargetPath = '%INSTALL_PATH%\CustomerManager.exe';" ^
    "$Shortcut.WorkingDirectory = '%INSTALL_PATH%';" ^
    "$Shortcut.Description = '%APP_NAME% v%APP_VERSION%';" ^
    "$Shortcut.Save();"

REM Add to registry for Add/Remove Programs (optional)
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\CustomerManager" ^
    /v DisplayName /d "%APP_NAME% v%APP_VERSION%" /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\CustomerManager" ^
    /v DisplayVersion /d "%APP_VERSION%" /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\CustomerManager" ^
    /v InstallLocation /d "%INSTALL_PATH%" /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\CustomerManager" ^
    /v Publisher /d "Your Company" /f >nul 2>&1

echo.
echo ===================================
echo Installation Complete!
echo ===================================
echo.
echo %APP_NAME% has been installed to:
echo %INSTALL_PATH%
echo.
echo You can find shortcuts at:
echo - Start Menu: %APP_NAME%
echo - Desktop: %APP_NAME%
echo.
echo Ready to use!
pause
