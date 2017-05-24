CALL osae_sp_object_type_add ('POWERSHELL','PowerShell Script Processor','','PLUGIN',0,0,0,1,'The PowerShell plugin.
This plugin can be used within OSA and outside of OSA to command OSA.');
CALL osae_sp_object_type_state_add('POWERSHELL','ON','Running','The PowerShell plugin is Running.');
CALL osae_sp_object_type_state_add('POWERSHELL','OFF','Stopped','The PowerShell plugin is Stopped.');
CALL osae_sp_object_type_event_add('POWERSHELL','ON','Started','The PowerShell plugin Started.');
CALL osae_sp_object_type_event_add('POWERSHELL','OFF','Stopped','The PowerShell plugin Stopped.');
CALL osae_sp_object_type_method_add('POWERSHELL','ON','Start','','','','','Start the PowerShell plugin.');
CALL osae_sp_object_type_method_add('POWERSHELL','OFF','Stop','','','','','Stop the PowerShell plugin.');
CALL osae_sp_object_type_method_add('POWERSHELL','RUN SCRIPT','RUN SCRIPT','','','','','Run the selected script.');
CALL osae_sp_object_type_property_add('POWERSHELL','System Plugin','Boolean','','TRUE',0,0,'Is the PowerShell Plugin is a system plugin?');
CALL osae_sp_object_type_property_add('POWERSHELL','Trust Level','Integer','','90',0,0,'The Trust Level of the PowerShell plugin.');
CALL osae_sp_object_type_property_add('POWERSHELL','Version','String','','',0,0,'Version of the PowerShell Plugin.');
CALL osae_sp_object_type_property_add('POWERSHELL','Author','String','','',0,0,'Author of the PowerShell Plugin.');