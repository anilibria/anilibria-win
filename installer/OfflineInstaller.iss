#define MyAppName "Anilibria Offline Installer"
#define MyAppVersion "0.1.12.0"
#define MyAppPublisher "EmptyFlow"
#define MyAppURL "https://www.anilibria.tv"
#define MyAppExeName "AnilibriaOfflineInstaller"
#define MyAppIcoName "anilibriaicon.ico"

[Setup]
AppId={{9B533D44-2551-4904-A457-8163BCABFB41}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=MIT.txt
OutputBaseFilename={#MyAppExeName}{#MyAppVersion}
SetupIconFile=anilibriaicon.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Dirs]
Name: "{app}\"

[Files]
Source: "Add-AppDevPackage.resources\*"; DestDir: "{app}/Add-AppDevPackage.resources"; Flags: ignoreversion recursesubdirs
Source: "Dependencies\*"; DestDir: "{app}/Dependencies"; Flags: ignoreversion recursesubdirs
Source: "preinstall.ps1"; DestDir: "{app}"; Flags: ignoreversion
Source: "Anilibria_{#MyAppVersion}_x86_x64_arm.appxbundle"; DestDir: "{app}"; Flags: ignoreversion
Source: "Anilibria_{#MyAppVersion}_x86_x64_arm.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "Add-AppDevPackage.ps1"; DestDir: "{app}"; Flags: ignoreversion
Source: "runinstaller.bat"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppIcoName}"; DestDir: "{app}"

[Run]
Filename: "{app}\AnilibriaInstaller.exe"; WorkingDir: "{app}"; Description: "Run true installer"

