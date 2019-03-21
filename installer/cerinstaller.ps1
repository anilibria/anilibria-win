#Import-Certificate -Filepath "$PSScriptRoot\test.cer" -CertStoreLocation Cert:\LocalMachine\Root

$response = Invoke-WebRequest -Method Get -Uri "https://anilibria.github.io/anilibria-win/dist/Anilibria.appinstaller" -ContentType "application/xml" -TimeoutSec 60
$xmlContent = [System.Text.Encoding]::UTF8.GetString($response.Content)
$xmlClear = $xmlContent.Replace("﻿<?xml version=""1.0"" encoding=""utf-8""?>", "")
$xml = [xml]$xmlClear
$version = $xml.AppInstaller.Attributes["Version"].Value

$pathToCetrificate = "https://anilibria.github.io/anilibria-win/dist/Anilibria_" + $version + "_Test/Anilibria_" + $version + "_x86_x64_arm.cer"

$parentPath = [System.IO.Path]::GetTempPath() + "Anilibria_" + $version + "_x86_x64_arm.cer"

Invoke-WebRequest -Uri $pathToCetrificate -Method Get -OutFile $parentPath

Import-Certificate -Filepath $parentPath -CertStoreLocation Cert:\LocalMachine\Root

