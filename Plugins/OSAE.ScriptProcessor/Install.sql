CALL osae_sp_object_type_add ('SCRIPT PROCESSOR','Generic Plugin Class','Script Processor','PLUGIN',1,0,0,1,'The Script Processor plugin.
This plugin is used to execute scripts within the OSA system
to automate certain objects or devices.');
CALL osae_sp_object_type_state_add('SCRIPT PROCESSOR','ON','Running','The Script Processor plugin is Running.');
CALL osae_sp_object_type_state_add('SCRIPT PROCESSOR','OFF','Stopped','The Script Processor plugin is Stopped.');
CALL osae_sp_object_type_event_add('SCRIPT PROCESSOR','ON','Started','The Script Processor plugin Started.');
CALL osae_sp_object_type_event_add('SCRIPT PROCESSOR','OFF','Stopped','The Script Processor plugin Stopped.');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','ON','Start','','','','','Start the Script Processor plugin.');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','OFF','Stop','','','','','Stop the Script Processor plugin.');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','RUN SCRIPT','Run Script','','','','','Run the selected script.');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','RUN READERS','Run Readers','','','','','Run the selected Reader.');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','System Plugin','Boolean','','TRUE',0,0,'Is the Script Processor Plugin is a system plugin?');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Debug','Boolean','','FALSE',0,1,'Add extra logging to help with Debugging.');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Trust Level','Integer','','50',0,0,'The Trust Level of the Script Processor plugin.');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Version','String','','',0,0,'Version of the Script Processor Plugin.');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Author','String','','',0,0,'Author of the Script Processor Plugin.');