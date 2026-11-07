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
AppName=TA-Calcolo
AppVersion={#AppVersion}
AppPublisher=Tassinari Associati
AppPublisherURL=https://github.com/studiotassinari/Calcolo
DefaultDirName={localappdata}\Programs\TA-Calcolo
DefaultGroupName=TA-Calcolo
UsePreviousGroup=no
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0
OutputDir={#InstallerOutput}
OutputBaseFilename=TA-Calcolo-Setup-{#AppVersion}-win-x64
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
SetupIconFile=apps\STA.Calcolo.Desktop\Assets\Brand\App.ico
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
Name: "{group}\TA-Calcolo"; Filename: "{app}\STA.Calcolo.Desktop.exe"
Name: "{group}\Ripristina TA-Calcolo"; Filename: "{app}\Recovery\STA.Calcolo.Recovery.exe"; Parameters: "--root ""{app}"" --parent 0"
Name: "{autodesktop}\TA-Calcolo"; Filename: "{app}\STA.Calcolo.Desktop.exe"; Tasks: desktopicon
[InstallDelete]
Type: files; Name: "{userprograms}\STA Calcolo\STA Calcolo.lnk"
Type: files; Name: "{userprograms}\STA Calcolo\Ripristina STA Calcolo.lnk"
Type: files; Name: "{autodesktop}\STA Calcolo.lnk"
