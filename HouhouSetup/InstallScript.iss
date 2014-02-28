[Setup]
AppName=Houhou SRS
AppVersion=1.0.0
DefaultDirName={pf}\Houhou SRS
DefaultGroupName=Houhou SRS
UninstallDisplayIcon={app}\Houhou.exe
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output

[Files]
Source: "MyProg.exe"; DestDir: "{app}"
Source: "MyProg.chm"; DestDir: "{app}"
Source: "Readme.txt"; DestDir: "{app}"; Flags: isreadme

[Icons]
Name: "{group}\My Program"; Filename: "{app}\MyProg.exe"
