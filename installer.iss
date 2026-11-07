#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif
#ifndef PublishDir
  #define PublishDir "artifacts\publish"
#endif
#ifndef InstallerOutput
  #define InstallerOutput "artifacts\installer"
#endif
[Setup]
AppId={{9FEC2950-470C-4A70-ADCD-EE58E640A4DD}
AppName=STA Calcolo
AppVersion={#AppVersion}
AppPublisher=Studio Tassinari
AppPublisherURL=https://github.com/studiotassinari/Calcolo
DefaultDirName={localappdata}\Programs\STA Calcolo
DefaultGroupName=STA Calcolo
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0
OutputDir={#InstallerOutput}
OutputBaseFilename=STA-Calcolo-Setup-{#AppVersion}-win-x64
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\STA.Calcolo.Desktop.exe
CloseApplications=no
RestartApplications=no
AppMutex=Local\STA.Calcolo.Desktop
UsePreviousAppDir=yes
DisableDirPage=auto
SetupMutex=STA-Calcolo-Setup
SetupLogging=yes
[Languages]
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
[Tasks]
Name: "desktopicon"; Description: "Crea un collegamento sul desktop"; Flags: unchecked
[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "artifacts\updater\STA.Calcolo.Updater.exe"; DestDir: "{app}\Recovery"; DestName: "STA.Calcolo.Recovery.exe"; Flags: ignoreversion
[Icons]
Name: "{group}\STA Calcolo"; Filename: "{app}\STA.Calcolo.Desktop.exe"
Name: "{group}\Ripristina STA Calcolo"; Filename: "{app}\Recovery\STA.Calcolo.Recovery.exe"; Parameters: "--root ""{app}"" --parent 0"
Name: "{autodesktop}\STA Calcolo"; Filename: "{app}\STA.Calcolo.Desktop.exe"; Tasks: desktopicon
