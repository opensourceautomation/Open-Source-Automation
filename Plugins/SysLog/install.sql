CALL osae_sp_object_type_add ('SysLog','SysLog','','SysLog',0,0,0,1,'Represents the SYSLOG Plugin');
CALL osae_sp_object_type_state_add ('SysLog','OFF','Stopped','The SYSLOG plugin IS Running');
CALL osae_sp_object_type_state_add ('SysLog','ON','Running','The SYSLOG plugin is NOT Running');
CALL osae_sp_object_type_event_add ('SysLog','OFF','Stopped','The SYSLOG plugin has Stopped');
CALL osae_sp_object_type_event_add ('SysLog','ON','Started','The SYSLOG plugin was Started');
CALL osae_sp_object_type_method_add ('SysLog','OFF','Stop','','','','','Will execute a Stop command for the SYSLOG plugin');
CALL osae_sp_object_type_method_add ('SysLog','ON','Start','','','','','Will execute a Start command for the SYSLOG plugin');
CALL osae_sp_object_type_property_add ('SysLog','Log to file','Boolean','','TRUE',0,0,"Path to the file to save SYSLOG information');
CALL osae_sp_object_type_property_add ('SysLog','Port','Integer','','514',0,0,'The port the SYSLOG plugin will be using');

CALL osae_sp_object_type_add ('SysLog-Trigger','SysLog Trigger','','SysLog-Trigger',0,1,0,1,'Represents a SYSLOG Trigger');
CALL osae_sp_object_type_event_add ('SysLog-Trigger','Triggered','Event Occurred','Executes the SYSLOG Trigger');
CALL osae_sp_object_type_property_add ('SysLog-Trigger','Trigger String','String','','',0,0,'The Trigger String this SYSLOG trigger will use');
CALL osae_sp_object_type_property_add ('SysLog-Trigger','Source IP','Integer','','',0,0,'The Source IP this SYSLOG trigger will use');
CALL osae_sp_object_type_property_add ('SysLog-Trigger','Exact Match','Boolean','','FALSE',0,0,'The EXACT-MATCH this SYSLOG trigger will use');