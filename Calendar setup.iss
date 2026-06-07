[Setup]
AppName=Calendar
AppVersion=1.1.2
DefaultDirName={autopf}\Radish\Calendar
DefaultGroupName=Radish
SetupIconFile=calendar.ico
UninstallDisplayIcon={app}\Calendar.exe
LicenseFile=LICENSE.txt
OutputBaseFilename=CalendarSetup
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
AppPublisher=Radish
AppPublisherURL=https://radish-vert.vercel.app
AppId={{7bfe3a3c-799a-454c-83a2-c5b2ef1007fc}

[Files]
Source: "bin\Release\net10.0-windows\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\Calendar"; Filename: "{app}\Calendar.exe"
Name: "{commondesktop}\Calendar"; Filename: "{app}\Calendar.exe"; Tasks: desktopicon

[Tasks]
Name: desktopicon; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"

[Run]
Filename: "{app}\Calendar.exe"; Description: "Launch Calendar"; Flags: nowait postinstall skipifsilent
