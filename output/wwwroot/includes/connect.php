<?php
require 'class.WindowsRegistry.php';

$winReg = new WindowsRegistry(); 
$winReg->ReadValue("HKEY_LOCAL_MACHINE\\Software\\OSAE\\DBSETTINGS", "DBCONNECTION"); 
$hostname = $winReg->ReadValue("HKEY_LOCAL_MACHINE\\Software\\OSAE\\DBSETTINGS", "DBCONNECTION");
$database = $winReg->ReadValue("HKEY_LOCAL_MACHINE\\Software\\OSAE\\DBSETTINGS", "DBNAME");
$username = $winReg->ReadValue("HKEY_LOCAL_MACHINE\\Software\\OSAE\\DBSETTINGS", "DBUSERNAME");
$password = $winReg->ReadValue("HKEY_LOCAL_MACHINE\\Software\\OSAE\\DBSETTINGS", "DBPASSWORD");

if($hostname = 'localhost')
{
  $hostname = '127.0.0.1';
}
mysql_connect($hostname, $username, $password)
or die ('Unable to connect!'); 
mysql_select_db($database);
?> 