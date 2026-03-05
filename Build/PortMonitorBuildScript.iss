; =========================================================
;  Port Monitor Application Installer – Inno Setup Script
; =========================================================

#define MyAppName        "PortMonitor"
#define MyAppVersion     "2.0.0"
#define MyAppPublisher   "Hubbell"
#define MyAppURL         "https://www.Hubbell.com/"
#define MyAppExeName     "PortMoniter.exe"

; ---- Paths ----
; SourcePath = folder where this .iss lives. If your .iss is inside D:\Repo\PortMonitor\Build\,
; then BuildRoot points to D:\Repo\PortMonitor\Build\
#define BuildRoot        AddBackslash(SourcePath)
; Your compiled WPF exe location:
#define SourceDir        AddBackslash(SourcePath) + "..\Src\PortMoniter\PortMoniter\bin\Release"

; com0com dependency that you keep under Build\Virtual\
#define Com0ComSubDir    "Virtual"
#define Com0ComSetupName "PortManager.exe"

; Optional file associations
#define MyAppAssocName   MyAppName + " File"
#define MyAppAssocExt    ".myp"
#define MyAppAssocKey    StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
AppId={{56251706-CB18-4ECC-AF88-C4BE1E51E2AE}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
ChangesAssociations=yes
DisableProgramGroupPage=yes
InfoBeforeFile={#BuildRoot}\Instruction.txt
SetupIconFile={#BuildRoot}\serialMonitor.ico
OutputBaseFilename=PortMonitor
Compression=lzma
SolidCompression=yes
WizardStyle=modern
; Installer must run elevated
PrivilegesRequired=admin
; Info & icon from Build folder

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "install_com0com"; \
    Description: "Install Virtual Port Manager (com0com) if not already installed"; \
    GroupDescription: "Optional components:"; \
    Flags: checkedonce
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

; =========================================================
;  FILES
; =========================================================
[Files]
; Your WPF app payload from Release
Source: "{#SourceDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; 🔹 Include the dependency folder Build\Virtual\ (contains PortManager.exe) in the installer
; It will be placed under {app}\_redist\Virtual\...
Source: "{#BuildRoot}\{#Com0ComSubDir}\*"; \
    DestDir: "{app}\_redist\{#Com0ComSubDir}"; \
    Flags: ignoreversion recursesubdirs createallsubdirs

; =========================================================
;  REGISTRY (file association)
; =========================================================
[Registry]
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\Applications\{#MyAppExeName}\SupportedTypes"; ValueType: string; ValueName: ".myp"; ValueData: ""
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

; =========================================================
;  RUN
; =========================================================
[Run]
; Launch your app (if app manifest requires admin, UAC will elevate here)
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: shellexec waituntilterminated postinstall skipifsilent; Verb: "runas"

; Conditionally run com0com installer only when missing and task selected
