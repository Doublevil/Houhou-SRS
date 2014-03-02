[Setup]
AppName=Houhou SRS
AppVersion=1.0.0
DefaultDirName={pf}\Houhou SRS
DefaultGroupName=Houhou SRS
UninstallDisplayIcon={app}\Houhou.exe
Compression=lzma2
SolidCompression=yes

[Files]
Source: "..\Kanji.Interface\bin\Release\Data\*"; DestDir: "{app}\Data"; Flags: recursesubdirs
Source: "..\Kanji.Interface\bin\Release\*.dll"; DestDir: "{app}"
Source: "..\Kanji.Interface\bin\Release\Houhou.exe"; DestDir: "{app}"; DestName: "Houhou SRS.exe"
Source: "..\Kanji.Interface\bin\Release\Houhou.exe.config"; DestDir: "{app}"; DestName: "Houhou SRS.exe.config"
Source: "..\Kanji.Interface\bin\Release\x64\*.dll"; DestDir: "{app}\x64";
Source: "..\Kanji.Interface\bin\Release\x86\*.dll"; DestDir: "{app}\x86";

[Icons]
Name: "{group}\Houhou SRS"; Filename: "{app}\Houhou SRS.exe"
Name: "{commondesktop}\Houhou SRS"; Filename: "{app}\Houhou SRS.exe"