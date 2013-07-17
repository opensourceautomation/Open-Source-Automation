CALL osae_sp_object_type_add ('SysLog','SysLog','','SysLog',0,0,0,1);
CALL osae_sp_object_type_state_add ('OFF','Stopped','SysLog');
CALL osae_sp_object_type_state_add ('ON','Running','SysLog');
CALL osae_sp_object_type_event_add ('OFF','Stopped','SysLog');
CALL osae_sp_object_type_event_add ('ON','Started','SysLog');
CALL osae_sp_object_type_method_add ('OFF','Stop','SysLog','','','','');
CALL osae_sp_object_type_method_add ('ON','Start','SysLog','','','','');
CALL osae_sp_object_type_property_add ('Log to file','Boolean','TRUE','SysLog',0);
CALL osae_sp_object_type_property_add ('Port','Integer','514','SysLog',0);

CALL osae_sp_object_type_add ('SysLog-Trigger','SysLog Trigger','','SysLog-Trigger',0,1,0,1);
CALL osae_sp_object_type_event_add ('Triggered','Event Occurred','SysLog-Trigger');
CALL osae_sp_object_type_property_add ('Trigger String','String','','SysLog-Trigger',0);
CALL osae_sp_object_type_property_add ('Source IP','Integer','','SysLog-Trigger',0);
CALL osae_sp_object_type_property_add ('Exact Match','Boolean','FALSE','SysLog-Trigger',0);