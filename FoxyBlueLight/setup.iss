; Script professionnel pour l'installation de Foxy Blue Light
; Date de création : 2025-05-16
; Auteur : theTigerFox

#define MyAppName "Foxy Blue Light"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Fox"
#define MyAppURL "https://the-fox.tech"
#define MyAppExeName "FoxyBlueLight.exe"
#define MyAppAssocName "Fichier de configuration Foxy Blue Light"
#define MyAppAssocExt ".fox"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
; Informations d'application
AppId={{ABB87924-0F8A-4796-9571-20BCC0D5C948}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=C:\Users\donfa\Documents\FoxyBlueLight\FoxyBlueLight\License.txt
InfoBeforeFile=C:\Users\donfa\Documents\FoxyBlueLight\FoxyBlueLight\Before.txt
InfoAfterFile=C:\Users\donfa\Documents\FoxyBlueLight\FoxyBlueLight\After.txt
OutputDir=C:\Users\donfa\OneDrive\Desktop\Setup Light
OutputBaseFilename=Foxy_Light_Setup
SetupIconFile=C:\Users\donfa\Documents\FoxyBlueLight\FoxyBlueLight\Resources\Icons\AppIcon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
WizardResizable=no
WizardImageFile=wizardimage.bmp
WizardSmallImageFile=wizardsmall.bmp
DisableWelcomePage=no
PrivilegesRequiredOverridesAllowed=dialog
ChangesAssociations=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
CloseApplications=yes
RestartApplications=no

; Aspect visuel
SetupLogging=yes
DisableDirPage=auto
UsePreviousAppDir=yes
AllowNoIcons=yes
ShowComponentSizes=no
WizardSizePercent=100
BackColor=$202020
BackColor2=$404040
BackColorDirection=lefttoright
BackSolid=no

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"; LicenseFile: "C:\Users\donfa\Documents\FoxyBlueLight\FoxyBlueLight\License.txt"
Name: "english"; MessagesFile: "compiler:Default.isl"; LicenseFile: "C:\Users\donfa\Documents\FoxyBlueLight\FoxyBlueLight\License.txt"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startmenufolder"; Description: "Créer un raccourci dans le menu Démarrer"; GroupDescription: "{cm:AdditionalIcons}";
Name: "startupicon"; Description: "Démarrer automatiquement au lancement de Windows"; GroupDescription: "Options de démarrage:"; Flags: unchecked

[Files]
; Fichiers principaux de l'application
Source: "C:\Users\donfa\OneDrive\Desktop\win-x64\publish\FoxyBlueLight.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\donfa\OneDrive\Desktop\win-x64\publish\D3DCompiler_47_cor3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\donfa\OneDrive\Desktop\win-x64\publish\PenImc_cor3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\donfa\OneDrive\Desktop\win-x64\publish\PresentationNative_cor3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\donfa\OneDrive\Desktop\win-x64\publish\vcruntime140_cor3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\donfa\OneDrive\Desktop\win-x64\publish\wpfgfx_cor3.dll"; DestDir: "{app}"; Flags: ignoreversion

; Ressources pour l'installateur
Source: "splash.bmp"; Flags: dontcopy
Source: "logo.png"; DestDir: "{app}\Resources"; Flags: ignoreversion

; Fichiers de documentation
Source: "C:\Users\donfa\Documents\FoxyBlueLight\FoxyBlueLight\License.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\donfa\Documents\FoxyBlueLight\FoxyBlueLight\After.txt"; DestDir: "{app}\docs"; DestName: "README.txt"; Flags: ignoreversion

[Registry]
; Associations de fichiers
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""

; Enregistrement du logiciel
Root: HKCU; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "Path"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: dword; ValueName: "InstallDate"; ValueData: "20250516"; Flags: uninsdeletekey

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "Protection avancée des yeux contre la lumière bleue"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; Comment: "Protection avancée des yeux contre la lumière bleue"
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon; Comment: "Protection avancée des yeux contre la lumière bleue"
Name: "{commonstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: startupicon; Comment: "Protection avancée des yeux contre la lumière bleue"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent unchecked
Filename: "{app}\docs\README.txt"; Description: "Lire les informations importantes"; Flags: shellexec postinstall skipifsilent unchecked

[UninstallRun]
; Exécuter une commande pour restaurer les paramètres d'écran d'origine
Filename: "{app}\{#MyAppExeName}"; Parameters: "--restore"; Flags: runhidden

[Messages]
WelcomeLabel1=Bienvenue dans le programme d'installation de {#MyAppName}
WelcomeLabel2=Ce programme va installer {#MyAppName} version {#MyAppVersion} sur votre ordinateur.%n%nProtection avancée des yeux contre la lumière bleue nocive des écrans.%n%nIl est recommandé de fermer toutes les autres applications avant de continuer.
FinishedHeadingLabel=Installation de {#MyAppName} terminée
FinishedLabel={#MyAppName} a été installé avec succès sur votre ordinateur.%n%nCliquez sur Terminer pour quitter le programme d'installation.

[Code]
// Fonctions personnalisées pour l'installateur

// Afficher un écran de démarrage au lancement de l'installateur
procedure InitializeWizard;
var
  SplashImage: TBitmapImage;
begin
  // Créer l'écran de démarrage
  ExtractTemporaryFile('splash.bmp');
  SplashImage := TBitmapImage.Create(WizardForm);
  SplashImage.Bitmap.LoadFromFile(ExpandConstant('{tmp}\splash.bmp'));
  SplashImage.Stretch := True;
  SplashImage.Align := alClient;
  SplashImage.Parent := WizardForm;
  
  // Montrer l'écran pendant 1,5 secondes
  WizardForm.Show;
  Sleep(1500);
  SplashImage.Free;
end;

// Personnaliser la page d'accueil
procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpWelcome then
  begin
    WizardForm.WelcomeLabel1.Font.Size := 14;
    WizardForm.WelcomeLabel1.Font.Style := [fsBold];
    WizardForm.WelcomeLabel1.Font.Color := $FF8000; // Orange
  end;
end;

// Détecter si l'application est déjà en cours d'exécution
function IsAppRunning(): Boolean;
var
  ResultCode: Integer;
begin
  Result := False;
  if Exec('tasklist.exe', '/FI "IMAGENAME eq FoxyBlueLight.exe" /NH', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    // Vérifier la sortie pour voir si le processus existe
    if ResultCode = 0 then
      Result := True;
  end;
end;

// Fermer l'application si elle est en cours d'exécution
function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ResultCode: Integer;
begin
  Result := '';
  if IsAppRunning() then
  begin
    if MsgBox('Foxy Blue Light est actuellement en cours d''exécution. Souhaitez-vous le fermer automatiquement pour continuer l''installation ?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      Exec('taskkill.exe', '/F /IM FoxyBlueLight.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      Sleep(1000); // Attendre 1 seconde pour que l'application se ferme
    end
    else
    begin
      Result := 'Veuillez fermer l''application Foxy Blue Light manuellement avant de continuer l''installation.';
    end;
  end;
end;

// Animation de transition entre les pages (version simplifiée pour éviter les problèmes)
procedure BeforeWizardNext(var Cancel: Boolean);
begin
  WizardForm.Refresh;
  Sleep(200);
end;