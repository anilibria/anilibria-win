$certFile = Get-ChildItem -Filter *.cer -r | Select-Object -first 1
$certificatePath = $certFile.FullName

Import-Certificate -Filepath $certificatePath -CertStoreLocation Cert:\LocalMachine\Root

$registryPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock" 

$Name1 = "AllowAllTrustedApps" 
$value1 = "1" 
New-ItemProperty -Path $registryPath -Name $Name1 -Value $value1 -PropertyType DWORD -Force 
 
$Name2 = "AllowDevelopmentWithoutDevLicense" 
$value2 = "0" 
 
New-ItemProperty -Path $registryPath -Name $Name2 -Value $value2 -PropertyType DWORD -Force

$scriptFile = Get-ChildItem -Filter *Package.ps1 -r | Select-Object -first 1

&$scriptFile.FullName -Force
