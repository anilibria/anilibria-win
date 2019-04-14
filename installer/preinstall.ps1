$registryPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock" 
 
$Name1 = "AllowAllTrustedApps" 
$value1 = "1" 
New-ItemProperty -Path $registryPath -Name $name1 -Value $value1 -PropertyType DWORD -Force 
 
$Name2 = "AllowDevelopmentWithoutDevLicense" 
$value2 = "0" 
 
New-ItemProperty -Path $registryPath -Name $name2 -Value $value2 -PropertyType DWORD -Force


