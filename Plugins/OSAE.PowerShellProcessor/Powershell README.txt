PowerShell Tools for Open Source Automation
===========================================

These tools allow you to interact with OSA from a powershell command line. 

Pre-Requisits:

	1. PowerShell version 3.0 (default on Win8) otherwise get it from here: http://www.microsoft.com/en-us/download/details.aspx?id=34595 It's now called the windows management framework


Install:

	In windows Powershell as ADMINISTRATOR!:

	# For non 64bit environments you may need to use $env:windir\Microsoft.NET\Framework\v4.0.30319\installutil
	set-alias installutil $env:windir\Microsoft.NET\Framework64\v4.0.30319\installutil
	
	#make sure you are in the directory where the OSAPS dll is located
	installutil OSAPS.dll
	
	#this should show that OSAPS is registered
	get-PSsnapin -registered

	#enable the addin
	add-pssnapin OSAPS

	#need to allow the execution of scripts in powershell so we change the execution policy
	Set-ExecutionPolicy RemoteSigned

	#optional... save the console so that it can be reloaded
	export-console OSAPSCustomShell

	#this will load the powershell with the assemblies loaded
	PS -PSConsoleFile OSAPSCustomShell.mcf

	#or you will need to execute
	add-pssnapin OSAPS
	#from  the shell when you open it


Useage:

There are three commands:

	1. Get an OSA object, called get-OSAPS
	2. Set an OSA object, called set-OSAPS
	3. Invoke a method on an object, called invoke-OSAPS

Each command takes the required set of parameters for the object name, property value or method name respectively

e.g.

	invoke-osaps -name "Plugwise" -Method "On" -Value "D364C8"  

This runs the function "On" on the component called "Plugiwse" with the paramtere "D364C8" being passed to the function

e.g.

	$x = get-osaps -name pvbeancounter
	$solarpower = $x.properties[0].Value

This gets the component called "pvbeancounter" and in the next line gets the first property (value) stored.