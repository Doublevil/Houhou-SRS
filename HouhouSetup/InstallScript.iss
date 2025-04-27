[Setup]
AppName=Houhou SRS
AppVersion=1.4
DefaultDirName={pf}\Houhou SRS
DefaultGroupName=Houhou SRS
UninstallDisplayIcon={app}\Houhou.exe
Compression=lzma2
SolidCompression=yes

[Files]
Source: "NDP481-Web.exe"; DestDir: {tmp}; Flags: deleteafterinstall; Check: not IsRequiredDotNetDetected
Source: "..\Kanji.Interface\bin\Release\Data\*"; DestDir: "{app}\Data"; Flags: ignoreversion recursesubdirs
Source: "..\Kanji.Interface\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Kanji.Interface\bin\Release\Houhou.exe"; DestDir: "{app}"; DestName: "Houhou SRS.exe"; Flags: ignoreversion
Source: "..\Kanji.Interface\bin\Release\Houhou.exe.config"; DestDir: "{app}"; DestName: "Houhou SRS.exe.config"; AfterInstall: ChangeEndpointAddress; Flags: ignoreversion
Source: "..\Kanji.Interface\bin\Release\x64\*.dll"; DestDir: "{app}\x64"; Flags: ignoreversion
Source: "..\Kanji.Interface\bin\Release\x86\*.dll"; DestDir: "{app}\x86"; Flags: ignoreversion                     

[Icons]
Name: "{group}\Houhou SRS"; Filename: "{app}\Houhou SRS.exe"
Name: "{commondesktop}\Houhou SRS"; Filename: "{app}\Houhou SRS.exe"

[Run]
Filename: "{tmp}\NDP481-Web.exe"; Check: not IsRequiredDotNetDetected; StatusMsg: Please follow the directions of the Microsoft .NET Framework 4.8.1 installer to continue.

[Code]
function IsDotNet481Installed(): Boolean;
var
  success: Boolean;
  releaseVersion: Cardinal;
begin
  success := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', releaseVersion)
             or RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client', 'Release', releaseVersion);
  Result := success and (releaseVersion >= 533320);
end;

var
  LibPage: TInputDirWizardPage;

procedure InitializeWizard();
begin
  LibPage := CreateInputDirPage(wpSelectDir, 'Select User Directory Location',
    'Where should the user files be stored?',
    'To continue, click Next. If you would like to select a different folder, ' +
    'click Browse.', False, 'User Directory');
  LibPage.Add('');
  LibPage.Values[0] := ExpandConstant('{userdocs}\Houhou');
end;

procedure UpdateUserPath;
var
C: AnsiString;
CU: String;
begin
        LoadStringFromFile(WizardDirValue + '\Houhou SRS.exe.config', C);
        CU := C;
        StringChangeEx(CU, '[userdir]', LibPage.Values[0], True);
        C := CU;
        SaveStringToFile(WizardDirValue + '\Houhou SRS.exe.config', C, False);
end;

const
  ConfigEndpointPath = '//configuration/userSettings/Kanji.Interface.Properties.Settings/setting[@name="UserDirectoryPath"]/value';

procedure ChangeEndpointAddress;
var
  XMLNode: Variant;
  TextNode: Variant;
  XMLDocument: Variant;  
begin
  XMLDocument := CreateOleObject('Msxml2.DOMDocument.6.0');
  try
    XMLDocument.async := False;
    XMLDocument.preserveWhiteSpace := True;
    XMLDocument.load(WizardDirValue + '\Houhou SRS.exe.config');    
    if (XMLDocument.parseError.errorCode <> 0) then
      RaiseException(XMLDocument.parseError.reason)
    else
    begin
      XMLDocument.setProperty('SelectionLanguage', 'XPath');
      XMLNode := XMLDocument.selectSingleNode(ConfigEndpointPath);
      TextNode := XMLDocument.createTextNode(LibPage.Values[0]);
      XMLNode.removeChild(XMLNode.childNodes.item(0));
      XMLNode.appendChild(TextNode);
      XMLDocument.save(WizardDirValue + '\Houhou SRS.exe.config');
    end;
  except
    MsgBox('An error occured during processing application ' +
      'config file!' + #13#10 + GetExceptionMessage, mbError, MB_OK);
  end;
end;

function InitializeSetup(): Boolean;
begin
    if not IsDotNet481Installed() then begin
        MsgBox('Houhou SRS requires the Microsoft .NET Framework 4.8.1.'#13#13
            'At the end of the installation process, the Microsoft .NET Framework 4.8.1 web installer will be started.'#13
            'Please check your internet connection before proceeding.', mbInformation, MB_OK);
    end;

    result := true;
end;

