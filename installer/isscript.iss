#define MyAppName "Anilibria Installer"
#define MyAppVersion "1.0"
#define MyAppPublisher "EmptyFlow"
#define MyAppURL "https://www.anilibria.tv"
#define MyAppExeName "AnilibriaInstaller.exe"
#define MyAppIcoName "anilibriaicon.ico"

[Setup]
AppId={{9B533D44-2551-4904-A457-8163BCABFB41}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=MIT.txt
OutputBaseFilename=AnilibriaInstallerSetup
SetupIconFile=anilibriaicon.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "AnilibriaInstaller.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "runinstaller.bat"; DestDir: "{app}"; Flags: ignoreversion
Source: "cerinstaller.ps1"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppIcoName}"; DestDir: "{app}"

[Icons]
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppIcoName}"; Tasks: desktopicon
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: shellexec postinstall skipifsilent

