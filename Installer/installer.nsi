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
  OutFile "OSA Setup v0.3.9_x86.exe"

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
   
  ${If} $0 != 0
  ${AndIf} $1 != 0
  ${AndIf} $2 != 0
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
  File "..\DB\0.3.3-0.3.4.sql"
  File "..\DB\0.3.4-0.3.5.sql"
  File "..\DB\0.3.5-0.3.6.sql"
  File "..\DB\0.3.6-0.3.7.sql"
  File "..\DB\0.3.7-0.3.8.sql"
  File "..\DB\0.3.8-0.3.9.sql"
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
  File "..\output\lib\OSAE.api.dll"
  File "..\output\OSAE.GUI.exe"
  File "..\output\OSAEService.exe"
  File "..\output\OSAEService.exe.config"
  File "..\output\PluginDescriptionEditor.exe"
  File "..\output\HostView.dll"
  File "..\output\OSAE.VR.exe"
  CreateDirectory "Sounds"
  CreateDirectory "Logs"
  CreateDirectory "Images"
  CreateDirectory "AddIns"
  CreateDirectory "wwwroot"
  CreateDirectory "AddInSideAdapters"
  CreateDirectory "AddInViews"
  CreateDirectory "contracts"
  CreateDirectory "HostSideAdapters"
  CreateDirectory "lib"
  
  SetOutPath "$INSTDIR\lib"
  File "..\output\lib\OSAE.api.dll"
  File "MySql.Data.dll"
  
  SetOutPath "$INSTDIR\AddInSideAdapters"
  File "..\output\AddInSideAdapters\AddInSideAdapter.dll"
  File "..\output\AddInSideAdapters\AdInV1AdapterToV2.dll"
  
  SetOutPath "$INSTDIR\AddInViews"
  File "..\output\AddInViews\AddInView.dll"
  File "..\output\AddInViews\AddInView2.dll"
  
  SetOutPath "$INSTDIR\contracts"
  File "..\output\contracts\OpenSourceAutomation.Contract.dll"
  
  SetOutPath "$INSTDIR\HostSideAdapters"
  File "..\output\HostSideAdapters\Template.dll"
  
  SetOutPath "$INSTDIR\wwwroot"
  File "..\output\wwwroot\*.*"
  CreateDirectory "bootstrap"
  CreateDirectory "includes"
  CreateDirectory "mobile"
  CreateDirectory "images"
  
  SetOutPath "$INSTDIR\wwwroot\bootstrap"
  CreateDirectory "css"
  CreateDirectory "js"
  CreateDirectory "img"
  SetOutPath "$INSTDIR\wwwroot\bootstrap\css"
  File "..\output\wwwroot\bootstrap\css\*.*"
  SetOutPath "$INSTDIR\wwwroot\bootstrap\js"
  File "..\output\wwwroot\bootstrap\js\*.*"
  SetOutPath "$INSTDIR\wwwroot\bootstrap\img"
  File "..\output\wwwroot\bootstrap\img\*.*"
  SetOutPath "$INSTDIR\wwwroot\includes"
  File "..\output\wwwroot\includes\*.*"
  SetOutPath "$INSTDIR\wwwroot\images"
  File "..\output\Images\*.jpg"
  File "..\output\Images\*.png"
  SetOutPath "$INSTDIR\wwwroot\mobile"
  File "..\output\wwwroot\mobile\*.*"
  
  CreateDirectory "jquery"
  SetOutPath "$INSTDIR\wwwroot\mobile\jquery"
  File "..\output\wwwroot\mobile\jquery\*.*"
  
  CreateDirectory "images"
  SetOutPath "$INSTDIR\wwwroot\mobile\jquery\images"
  File "..\output\wwwroot\mobile\jquery\images\*.*"
  
  SetOutPath "$INSTDIR\Logs"
  
  SetOutPath "$INSTDIR\Images"
  File "..\output\Images\*.jpg"
  File "..\output\Images\*.png" 
  
  SetOutPath "$INSTDIR\AddIns"
  CreateDirectory "Bluetooth"
  CreateDirectory "Email"
  CreateDirectory "Jabber"
  CreateDirectory "Network Monitor"
  CreateDirectory "Script Processor"
  CreateDirectory "Speech"
  CreateDirectory "Weather"
  CreateDirectory "Web Server"
  
  SetOutPath "$INSTDIR\AddIns\Bluetooth"
  File "..\output\AddIns\Bluetooth\Bluetooth.osapd"
  File "..\output\AddIns\Bluetooth\OSAE.Bluetooth.dll"
  File "..\output\AddIns\Bluetooth\InTheHand.Net.Personal.dll"
  File "..\output\AddIns\Bluetooth\Screenshot.jpg"
    
  SetOutPath "$INSTDIR\AddIns\Email"
  File "..\output\AddIns\Email\Email.osapd"
  File "..\output\AddIns\Email\OSAE.Email.dll"
  File "..\output\AddIns\Email\Screenshot.jpg"
  
  SetOutPath "$INSTDIR\AddIns\Jabber"
  File "..\output\AddIns\Jabber\Jabber.osapd"
  File "..\output\AddIns\Jabber\OSAE.Jabber.dll"
  File "..\output\AddIns\Jabber\agsXMPP.dll"
  File "..\output\AddIns\Jabber\Screenshot.jpg"
    
  SetOutPath "$INSTDIR\AddIns\Network Monitor"
  File "..\output\AddIns\Network Monitor\Network Monitor.osapd"
  File "..\output\AddIns\Network Monitor\OSAE.NetworkMonitor.dll"
  File "..\output\AddIns\Network Monitor\Screenshot.jpg"
  
  SetOutPath "$INSTDIR\AddIns\Script Processor"
  File "..\output\AddIns\Script Processor\Script Processor.osapd"
  File "..\output\AddIns\Script Processor\OSAE.ScriptProcessor.dll"
  File "..\output\AddIns\Script Processor\Screenshot.jpg"
  
  SetOutPath "$INSTDIR\AddIns\Speech"
  File "..\output\AddIns\Speech\Speech.osapd"
  File "..\output\AddIns\Speech\OSAE.Speech.dll"
  File "..\output\AddIns\Speech\Screenshot.jpg"
  
  SetOutPath "$INSTDIR\AddIns\Weather"
  File "..\output\AddIns\Weather\Weather.osapd"
  File "..\output\AddIns\Weather\OSAE.Weather.dll"  
  File "..\output\AddIns\Weather\Screenshot.jpg"

  SetOutPath "$INSTDIR\AddIns\Web Server"
  File "..\output\AddIns\Web Server\Web Server.osapd"
  File "..\output\AddIns\Web Server\OSAE.WebServer.dll"  
  File "..\output\AddIns\Web Server\HttpServer.dll"
  File "..\output\AddIns\Web Server\Screenshot.jpg"

  SetOutPath "C:\"
  CreateDirectory "PHP"
  SetOutPath "C:\PHP"
  CreateDirectory "ext"
  File "php-cgi.exe"
  File "php5.dll"
  File "php.ini"
  SetOutPath "C:\PHP\ext"
  File "php_mysql.dll"
  
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
  
  SetOutPath $INSTDIR
  File "DBInstall\GAC\bin\Debug\GAC.exe"
  ExecWait "$INSTDIR\GAC.exe"
  Delete "$INSTDIR\GAC.exe"
    
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
  File "..\output\lib\OSAE.api.dll"
  File "..\output\OSAE.GUI.exe"
  File "..\output\ClientService.exe"
  File "..\output\ClientService.exe.config"
  File "..\output\PluginDescriptionEditor.exe"
  File "..\output\HostView.dll"
  File "..\output\OSAE.VR.exe"
  CreateDirectory "Sounds"
  CreateDirectory "Logs"
  CreateDirectory "Images"
  CreateDirectory "AddIns"
  CreateDirectory "AddInSideAdapters"
  CreateDirectory "AddInViews"
  CreateDirectory "contracts"
  CreateDirectory "HostSideAdapters"
  CreateDirectory "lib"
  
  SetOutPath "$INSTDIR\lib"
  File "..\output\lib\OSAE.api.dll"
  File "MySql.Data.dll"
  
  SetOutPath "$INSTDIR\AddInSideAdapters"
  File "..\output\AddInSideAdapters\AddInSideAdapter.dll"
  File "..\output\AddInSideAdapters\AdInV1AdapterToV2.dll"
  
  SetOutPath "$INSTDIR\AddInViews"
  File "..\output\AddInViews\AddInView.dll"
  File "..\output\AddInViews\AddInView2.dll"
  
  SetOutPath "$INSTDIR\contracts"
  File "..\output\contracts\OpenSourceAutomation.Contract.dll"
  
  SetOutPath "$INSTDIR\HostSideAdapters"
  File "..\output\HostSideAdapters\Template.dll"
  
  SetOutPath "$INSTDIR\Images"
  File "..\output\Images\*.jpg"
  File "..\output\Images\*.png" 
  
  SetOutPath "$INSTDIR\AddIns"
  CreateDirectory "Speech"
     
  SetOutPath "$INSTDIR\AddIns\Speech"
  File "..\output\AddIns\Speech\Speech.osapd"
  File "..\output\AddIns\Speech\OSAE.Speech.dll"
  File "..\output\AddIns\Speech\Screenshot.jpg"
  
  SetOutPath "$INSTDIR\AddIns\Bluetooth"
  File "..\output\AddIns\Bluetooth\Bluetooth.osapd"
  File "..\output\AddIns\Bluetooth\OSAE.Bluetooth.dll"
  File "..\output\AddIns\Bluetooth\InTheHand.Net.Personal.dll"
  File "..\output\AddIns\Bluetooth\Screenshot.jpg"

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
  
  SetOutPath $INSTDIR
  File "DBInstall\GAC\bin\Debug\GAC.exe"
  ExecWait "$INSTDIR\GAC.exe"
  Delete "$INSTDIR\GAC.exe"
    
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