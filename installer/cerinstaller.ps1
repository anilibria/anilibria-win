Write-Host "Anilibria client installer for Windows 10"

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

Write-Host "Retrieve version current distributive...."

$response = Invoke-WebRequest -Method Get -Uri "https://anilibria.github.io/anilibria-win/dist/Anilibria.appinstaller" -ContentType "application/xml" -TimeoutSec 60
$xmlContent = [System.Text.Encoding]::UTF8.GetString($response.Content)
$xmlClear = $xmlContent.Substring(39)
$xml = [xml]$xmlClear
$version = $xml.AppInstaller.Attributes["Version"].Value

Write-Host "Version $($version)" 

$certificateUri = "https://anilibria.github.io/anilibria-win/dist/Anilibria_" + $version + "_Test/Anilibria_" + $version + "_x86_x64_arm.cer"

$certificatePath = [System.IO.Path]::GetTempPath() + "Anilibria_" + $version + "_x86_x64_arm.cer"

Write-Host "Retrieve certificate....."

Invoke-WebRequest -Uri $certificateUri -Method Get -OutFile $certificatePath

Write-Host "Import certificate....."

Import-Certificate -Filepath $certificatePath -CertStoreLocation Cert:\LocalMachine\Root

Write-Host "Remove certificate from temporary folder...."

Remove-Item $certificatePath

Write-Host "Installation complete"

[System.Diagnostics.Process]::Start("ms-appinstaller:?source=https://anilibria.github.io/anilibria-win/dist/Anilibria.appinstaller")