CALL osae_sp_object_type_add ('Sony','Sony Plugin','','PLUGIN',1,0,0,1,'Plugin used to control Sony Smart devices through LAN');
CALL osae_sp_object_type_state_add ('Sony','ON','Running','Sony plugin is Running');
CALL osae_sp_object_type_state_add ('Sony','OFF','Stopped','Sony plugin is Stopped');
CALL osae_sp_object_type_method_add ('Sony','DISCOVERY','Discovery','','','','','Search LAN for Sony Devices');
CALL osae_sp_object_type_method_add ('Sony','SETDEBUG','Set Debug','TRUE/FALSE','','FALSE','','Set Debug setting');
CALL osae_sp_object_type_property_add('Sony','Refresh','String','','5','',0,0,'Minutes to Auto-Refresh device states');
CALL osae_sp_object_type_property_add('Sony','Debug','Boolean','','FALSE',0,0,'Set debug logging');
