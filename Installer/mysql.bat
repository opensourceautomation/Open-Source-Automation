@echo off

"%PROGRAMFILES%\MySql\bin\mysqld.exe" --install MySQL --defaults-file="%PROGRAMFILES%\MySql\my.ini"
"%PROGRAMFILES%\MySql\bin\MySQLInstanceConfig.exe" -i -q "-lc:mysql_install_log.txt" ServerType=DEVELOPMENT DatabaseType=MIXED ConnectionUsage=DSS Port=3306 RootPassword=password
net start MySql
"%PROGRAMFILES%\MySql\bin\mysql" -uroot -ppassword --execute "CREATE USER `root`@`%` IDENTIFIED BY 'password'";
"%PROGRAMFILES%\MySql\bin\mysql" -uroot -ppassword --execute "GRANT ALL ON *.* TO `root`@`%`";