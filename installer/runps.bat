PowerShell -NoProfile -ExecutionPolicy Bypass -NoExit -File "%cd%\preinstall.ps1"
PowerShell -NoProfile -ExecutionPolicy Bypass -NoExit -File "%cd%\Add-AppDevPackage.ps1" "-Force"
exit