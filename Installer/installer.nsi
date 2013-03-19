;--------------------------------
;Includes

  !include "MUI2.nsh"
  !include "InstallOptions.nsh"
  !include "fileassoc.nsh"
  !include "LogicLib.nsh"
  !include "x64.nsh"
  !include "WinVer.nsh"
  !include "DotNetVer.nsh"

;--------------------------------
;General

  ;Name and file
  Name "Open Source Automation"
  OutFile "OSA Setup v0.4.0_x86.exe"

  ;Default installation folder
  InstallDir "$PROGRAMFILES\OSA"
  
;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING  

  CRCCheck on
  XPStyle on 
  
  ShowInstDetails show
  
  BrandingText "OSA Installer"
  
  

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_LICENSE "License.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  !define MUI_FINISHPAGE_RUN_TEXT "Thank you for installing Open Source Automation."
  !insertmacro MUI_PAGE_FINISH
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Install Types
  InstType "Server"
  InstType "Client"
  InstType /NOCUSTOM

;Installer Sections
 
; These are the programs that are needed by OSA.
Section -Prerequisites
  SectionIn 1 2
  SetOutPath $INSTDIR
  ${If} ${HasDotNet4.0}
    ${If} ${DOTNETVER_4_0} HasDotNetFullProfile 1
      DetailPrint "Microsoft .NET Framework 4.0 (Full Profile) available."
    ${Else}
      File "dotNetFx40_Full_setup.exe"
      ExecWait "$INSTDIR\dotNetFx40_Full_setup.exe /q /norestart"
    ${EndIf}    
  ${Else}
    File "dotNetFx40_Full_setup.exe"
    ExecWait "$INSTDIR\dotNetFx40_Full_setup.exe"
  ${EndIf}
  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\10.0\VC\VCRedist\x86" 'Installed'
  ${If} $0 == 1
    DetailPrint "VC++ 2011 Redist. already installed"
  ${Else}
  DetailPrint $0
    File "vcredist_x86.exe"
    ExecWait "$INSTDIR\vcredist_x86.exe /q"
    Delete "$INSTDIR\vcredist_x86.exe"
    Goto endVC  
  ${EndIf}
  endVC:
  
  Delete "$INSTDIR\dotNetFx40_Full_setup.exe"
  
SectionEnd

Section Server s1
  SectionIn 1
  
  SetOutPath $INSTDIR
  
  SimpleSC::ExistsService "MySql"
  Pop $0
  SimpleSC::ExistsService "MySql55"
  Pop $1
  SimpleSC::ExistsService "MySQL"
  Pop $2
  SimpleSC::ExistsService "MySql56"
  Pop $3
  
  ${If} $0 != 0
  ${AndIf} $1 != 0
  ${AndIf} $2 != 0
  ${AndIf} $3 != 0
    SetOutPath "$INSTDIR"
    File "..\DB\osae.sql"
    File "MySql.Data.dll"
    File "mysql-5.5.21-win32.msi"
    ExecWait 'msiexec /q /log "$INSTDIR\MySQLINstall.log" /i mysql-5.5.21-win32.msi installdir="$PROGRAMFILES\MySql"'
    SetOutPath "$PROGRAMFILES\MySql"
    ExecWait '"$PROGRAMFILES\MySql\bin\mysqld.exe" --install MySQL --defaults-file="$PROGRAMFILES\MySql\my.ini"'
    ExecWait '"$PROGRAMFILES\MySql\bin\MySQLInstanceConfig.exe" -i -q "-lc:mysql_install_log.txt" ServerType=DEVELOPMENT DatabaseType=MIXED ConnectionUsage=DSS Port=3306 RootPassword=password'
    Delete "mysql-5.5.21-win32.msi"
    File "my.ini"
    
    SimpleSC::RestartService "MySql" "" 30
    ExecWait '"$PROGRAMFILES\MySql\bin\mysql" -uroot -ppassword --execute "CREATE USER `osae`@`%` IDENTIFIED BY $\'osaePass$\'";'
    ExecWait '"$PROGRAMFILES\MySql\bin\mysql" -uroot -ppassword --execute "GRANT ALL ON osae.* TO `osae`@`%`";'
    ExecWait '"$PROGRAMFILES\MySql\bin\mysql" -uroot -ppassword --execute "GRANT SUPER PRIVILEGES ON *.* TO `osae`@`%`";' 

    Goto endMysql
  ${Else}
    DetailPrint "MySql is already installed!"
    SimpleSC::GetServiceStatus "MyService"
    Pop $0 ; returns an errorcode (<>0) otherwise success (0)
    Pop $1 ; return the status of the service (See "service_status" in the parameters)
    
    ${If} $1 == 4
      ExecWait "net start MySql"
    ${EndIf}
  ${EndIf}
 
  endMysql: 
    Delete "$INSTDIR\mysql-5.5.21-win32.msi"
  
  SetOutPath "$INSTDIR"  
  File "..\DB\osae.sql"
  File "..\DB\0.3.9-0.4.0.sql"
  File "MySql.Data.dll"
  File "DBInstall\DBInstall\bin\Debug\DBInstall.exe"
  ExecWait 'DBInstall.exe "$INSTDIR" "Server"'
  Goto endDBInstall
  endDBInstall:   
  Delete "DBInstall.exe"
  
  SimpleSC::StopService "OSAE" 1 30
  
  SetOutPath "$INSTDIR"
  Delete "..\output\OSAE Manager.exe"
  Delete "..\output\OSAE Manager.exe.config"
  
  File "..\output\ICSharpCode.SharpZipLib.dll"
  File "..\output\OSAE.Manager.exe"
  File "..\output\OSAE.Manager.exe.config"
  File "..\output\LogViewer.exe"
  File "..\output\OSAE.api.dll"
  File "..\output\OSAE.GUI.exe"
  File "..\output\OSAEService.exe"
  File "..\output\OSAEService.exe.config"
  File "..\output\PluginDescriptionEditor.exe"
  File "..\output\OSAE.VR.exe"
  CreateDirectory "Sounds"
  CreateDirectory "Logs"

  CreateDirectory "Plugins"
  
  
  SetOutPath "$INSTDIR\Logs"
  
  SetOutPath "$INSTDIR\Plugins"
  CreateDirectory "Bluetooth"
  CreateDirectory "Email"
  CreateDirectory "Jabber"
  CreateDirectory "Network Monitor"
  CreateDirectory "Script Processor"
  CreateDirectory "Speech"
  CreateDirectory "Weather"
  CreateDirectory "Web Server"
  
  SetOutPath "$INSTDIR\Plugins\Bluetooth"
  File "..\output\Plugins\Bluetooth\Bluetooth.osapd"
  File "..\output\Plugins\Bluetooth\OSAE.Bluetooth.dll"
  File "..\output\Plugins\Bluetooth\InTheHand.Net.Personal.dll"
  File "..\output\Plugins\Bluetooth\Screenshot.jpg"
    
  SetOutPath "$INSTDIR\Plugins\Email"
  File "..\output\Plugins\Email\Email.osapd"
  File "..\output\Plugins\Email\OSAE.Email.dll"
  File "..\output\Plugins\Email\Screenshot.jpg"
  
  SetOutPath "$INSTDIR\Plugins\Jabber"
  File "..\output\Plugins\Jabber\Jabber.osapd"
  File "..\output\Plugins\Jabber\OSAE.Jabber.dll"
  File "..\output\Plugins\Jabber\agsXMPP.dll"
  File "..\output\Plugins\Jabber\Screenshot.jpg"
    
  SetOutPath "$INSTDIR\Plugins\Network Monitor"
  File "..\output\Plugins\Network Monitor\Network Monitor.osapd"
  File "..\output\Plugins\Network Monitor\OSAE.NetworkMonitor.dll"
  File "..\output\Plugins\Network Monitor\Screenshot.jpg"
  
  SetOutPath "$INSTDIR\Plugins\Script Processor"
  File "..\output\Plugins\Script Processor\Script Processor.osapd"
  File "..\output\Plugins\Script Processor\OSAE.ScriptProcessor.dll"
  File "..\output\Plugins\Script Processor\Screenshot.jpg"
  
  SetOutPath "$INSTDIR\Plugins\Speech"
  File "..\output\Plugins\Speech\Speech.osapd"
  File "..\output\Plugins\Speech\OSAE.Speech.dll"
  File "..\output\Plugins\Speech\Screenshot.jpg"
  
  SetOutPath "$INSTDIR\Plugins\Weather"
  File "..\output\Plugins\Weather\Weather.osapd"
  File "..\output\Plugins\Weather\OSAE.Weather.dll"  
  File "..\output\Plugins\Weather\Screenshot.jpg"

  SetOutPath "$INSTDIR\Plugins\Web Server"
  File "..\output\Plugins\Web Server\Web Server.osapd"
  File "..\output\Plugins\Web Server\Screenshot.jpg"
  CreateDirectory "wwwroot"
    
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot"
  File "..\WebUI\*.*"
  CreateDirectory "bootstrap"
  CreateDirectory "Bin"
  CreateDirectory "mobile"
  CreateDirectory "Images"
  CreateDirectory "css"
  
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\bootstrap"
  CreateDirectory "css"
  CreateDirectory "js"
  CreateDirectory "img"
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\bootstrap\css"
  File "..\WebUI\bootstrap\css\*.*"
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\bootstrap\js"
  File "..\WebUI\bootstrap\js\*.*"
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\bootstrap\img"
  File "..\WebUI\bootstrap\img\*.*"
  
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\Bin"
  File "..\WebUI\Bin\*.*"
  
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\Images"
  File "..\WebUI\Images\*.*"
  
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\mobile"
  File "..\WebUI\mobile\*.*"
  CreateDirectory "images"
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\mobile\images"
  File "..\WebUI\mobile\images\*.*"
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\mobile"
  CreateDirectory "jquery"
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\mobile\jquery"
  File "..\WebUI\mobile\jquery\*.*"
  CreateDirectory "images"
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\mobile\jquery\images"
  File "..\WebUI\mobile\jquery\images\*.*"
  
  SetOutPath "$INSTDIR\Plugins\Web Server\wwwroot\css"
  File "..\WebUI\css\*.*"


 
  SetOutPath $INSTDIR
  File "UltiDev.WebServer.msi"

  DetailPrint "Installing UltiDev Web Server Pro"
  ExecWait 'msiexec.exe /passive /i "$INSTDIR\UltiDev.WebServer.msi"'

  ; Unregister website to make sure no files are in use by webserver while upgrading 
  ; and to pick up any changes in how we register it now

  DetailPrint "Unregistering Website"
  ExecWait '"$PROGRAMFILES64\UltiDev\Web Server\UWS.RegApp.exe" /unreg /AppID:{58fe03ca-9975-4df2-863e-a228614258c4}'
  ; Register the website 

  ExecWait '"$PROGRAMFILES64\UltiDev\Web Server\UWS.RegApp.exe" /r /AppId={58fe03ca-9975-4df2-863e-a228614258c4} /path:"$INSTDIR\Plugins\Web Server\wwwroot" "/EndPoints:http://*:8081/" /ddoc:default.aspx /appname:"Open Source Automation" /apphost=SharedLocalSystem /clr:4 /vpath:"/"'
  
  # Start Menu Shortcuts
  SetShellVarContext all
  CreateDirectory "$SMPROGRAMS\OSA"
  createShortCut "$SMPROGRAMS\OSA\Manager.lnk" "$INSTDIR\OSAE.Manager.exe"
  createShortCut "$SMPROGRAMS\OSA\OSAE.GUI.lnk" "$INSTDIR\OSAE.GUI.exe"

  ${If} ${AtLeastWinVista}
    SetShellVarContext all
    ShellLink::SetRunAsAdministrator "$INSTDIR\OSAE.Manager.exe"
    ShellLink::SetRunAsAdministrator "$SMPROGRAMS\OSA\Manager.lnk"
    #Pop $0
  ${EndIf}          
  
  SimpleSC::InstallService "OSAE" "OSAE Service" "16" "2" "$INSTDIR\OSAEService.exe" "" "" ""
  Pop $0 ; returns an errorcode (<>0) otherwise success (0)

  WriteRegStr HKLM "SOFTWARE\OSAE\DBSETTINGS" "INSTALLDIR" "$INSTDIR"
  WriteRegStr HKLM "SOFTWARE\OSAE\DBSETTINGS" "DBCONNECTION" "localhost"
  WriteRegStr HKLM "SOFTWARE\OSAE\DBSETTINGS" "DBPORT" "3306"
  WriteRegStr HKLM "SOFTWARE\OSAE\DBSETTINGS" "DBNAME" "osae"
  WriteRegStr HKLM "SOFTWARE\OSAE\DBSETTINGS" "DBUSERNAME" "osae"
  WriteRegStr HKLM "SOFTWARE\OSAE\DBSETTINGS" "DBPASSWORD" "osaePass"
  
  !insertmacro APP_ASSOCIATE "osapd" "OSA.osapd" "Plugin Description" "$INSTDIR\PluginDescriptionEditor.exe,0" "Open" "$INSTDIR\PluginDescriptionEditor.exe $\"%1$\""  
  !insertmacro APP_ASSOCIATE "osapp" "OSA.osapp" "Plugin Package" "$INSTDIR\PluginInstaller.exe,0" "Open" "$INSTDIR\OSAE Manager.exe $\"%1$\"" 
  !insertmacro UPDATEFILEASSOC    
  
  AccessControl::GrantOnFile \
    "$INSTDIR" "(BU)" "GenericRead + GenericWrite"
  
  writeUninstaller $INSTDIR\uninstall.exe
  
  
  
  MessageBox MB_OKCANCEL "Open Source Automation has been successfully installed. Would you like to start the OSAE Service now?" IDOK lbl_ok  IDCANCEL lblDone
    lbl_ok:
      SimpleSC::StartService "OSAE" "" 30
      Pop $0
    GoTo lblDone
    lblDone:
    
SectionEnd

Section Client s2
  SectionIn 2
  
  SimpleSC::StopService "OSAE Client" 1 30
  
  SetOutPath "$INSTDIR"
  Delete "..\output\OSAE Manager.exe"
  Delete "..\output\OSAE Manager.exe.config"
  
  File "..\output\ICSharpCode.SharpZipLib.dll"
  File "MySql.Data.dll"
  File "..\output\OSAE.Manager.exe"
  File "..\output\OSAE.Manager.exe.config"
  File "..\output\OSAE.api.dll"
  File "..\output\OSAE.GUI.exe"
  File "..\output\ClientService.exe"
  File "..\output\ClientService.exe.config"
  File "..\output\PluginDescriptionEditor.exe"
  File "..\output\OSAE.VR.exe"
  CreateDirectory "Sounds"
  CreateDirectory "Logs"
  CreateDirectory "Plugins"
    
  SetOutPath "$INSTDIR\Plugins"
  CreateDirectory "Speech"
     
  SetOutPath "$INSTDIR\Plugins\Speech"
  File "..\output\Plugins\Speech\Speech.osapd"
  File "..\output\Plugins\Speech\OSAE.Speech.dll"
  File "..\output\Plugins\Speech\Screenshot.jpg"
  
  SetOutPath "$INSTDIR\Plugins\Bluetooth"
  File "..\output\Plugins\Bluetooth\Bluetooth.osapd"
  File "..\output\Plugins\Bluetooth\OSAE.Bluetooth.dll"
  File "..\output\Plugins\Bluetooth\InTheHand.Net.Personal.dll"
  File "..\output\Plugins\Bluetooth\Screenshot.jpg"

  # Start Menu Shortcuts
  SetShellVarContext all
  CreateDirectory "$SMPROGRAMS\OSA"
  createShortCut "$SMPROGRAMS\OSA\Manager.lnk" "$INSTDIR\OSAE.Manager.exe"
  createShortCut "$SMPROGRAMS\OSA\OSAE.GUI.lnk" "$INSTDIR\OSAE.GUI.exe"

  ${If} ${AtLeastWinVista}
    SetShellVarContext all
    ShellLink::SetRunAsAdministrator "$INSTDIR\OSAE.Manager.exe"
    ShellLink::SetRunAsAdministrator "$SMPROGRAMS\OSA\Manager.lnk"
    #Pop $0
  ${EndIf} 
  
  SimpleSC::InstallService "OSAE Client" "OSAE Client Service" "16" "2" "$INSTDIR\ClientService.exe" "" "" ""
  Pop $0 ; returns an errorcode (<>0) otherwise success (0)

  WriteRegStr HKLM "SOFTWARE\OSAE\DBSETTINGS" "INSTALLDIR" "$INSTDIR"
  WriteRegStr HKLM "SOFTWARE\OSAE\DBSETTINGS" "DBNAME" "osae"
  WriteRegStr HKLM "SOFTWARE\OSAE\DBSETTINGS" "DBUSERNAME" "osae"
  WriteRegStr HKLM "SOFTWARE\OSAE\DBSETTINGS" "DBPASSWORD" "osaePass"
  
  !insertmacro APP_ASSOCIATE "osapd" "OSA.osapd" "Plugin Description" "$INSTDIR\PluginDescriptionEditor.exe,0" "Open" "$INSTDIR\PluginDescriptionEditor.exe $\"%1$\""  
  !insertmacro APP_ASSOCIATE "osapp" "OSA.osapp" "Plugin Package" "$INSTDIR\PluginInstaller.exe,0" "Open" "$INSTDIR\OSAE Manager.exe $\"%1$\"" 
  !insertmacro UPDATEFILEASSOC    
  
  AccessControl::GrantOnFile \
    "$INSTDIR" "(BU)" "GenericRead + GenericWrite"
   
  writeUninstaller $INSTDIR\uninstall.exe
  
  SetOutPath $INSTDIR
  File "DBInstall\DBInstall\bin\Debug\DBInstall.exe"
  ExecWait 'DBInstall.exe "$INSTDIR" "Client"'
  Goto endDBInstall2
  endDBInstall2:   
  Delete "DBInstall.exe"
  
  MessageBox MB_OKCANCEL "Open Source Automation has been successfully installed. Would you like to start the OSAE Service now?" IDOK lbl_ok  IDCANCEL lblDone
    lbl_ok:
      SimpleSC::StartService "OSAE Client" "" 30
      Pop $0
    GoTo lblDone
    lblDone:
SectionEnd

;--------------------------------
;Uninstaller Section

Section "Uninstall"
  SectionIn 1 2

  Delete "$INSTDIR\Uninstall.exe"
  Delete "$SMPROGRAMS\OSA\Manager.lnk"
  Delete "$SMPROGRAMS\OSA\Uninstall.lnk"
  Delete "$SMPROGRAMS\OSA\OSAE.GUI.lnk"
  Delete "$SMPROGRAMS\OSA\DevTools.lnk"
  RMDir /r "$SMPROGRAMS\OSA"
  !insertmacro APP_UNASSOCIATE "osapd" "OSA.osapd"
  !insertmacro APP_UNASSOCIATE "osapp" "OSA.osapp"
  Delete $INSTDIR\*.*
  RMDir /r $INSTDIR
  SimpleSC::RemoveService "OSAE"
  Pop $0
  SimpleSC::RemoveService "OSAE Client"
  Pop $1

SectionEnd


!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${s1} "Install the Open Source Automation server.  Install this on your main machine"
  !insertmacro MUI_DESCRIPTION_TEXT ${s2} "Client serivce for Open Source Automation.  Install on client machines."
!insertmacro MUI_FUNCTION_DESCRIPTION_END