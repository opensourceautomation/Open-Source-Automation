CALL osae_sp_object_type_add ('COMPUTER POWER','Computer Power Monitor Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('COMPUTER POWER','ON','Running');
CALL osae_sp_object_type_state_add ('COMPUTER POWER','OFF','Stopped');
CALL osae_sp_object_type_event_add ('COMPUTER POWER','ON','Started');
CALL osae_sp_object_type_event_add ('COMPUTER POWER','OFF','Stopped');
CALL osae_sp_object_type_method_add ('COMPUTER POWER','ON','Start','','','','');
CALL osae_sp_object_type_method_add ('COMPUTER POWER','OFF','Stop','','','','');
CALL osae_sp_object_type_method_add ('COMPUTER POWER','UPDATE','Update','','','','');
CALL osae_sp_object_type_property_add ('COMPUTER POWER','Update Interval','Integer','','30',0);
CALL osae_sp_object_type_property_add ('COMPUTER POWER','Computer Name','String','','',0);

CALL osae_sp_object_type_event_add ('COMPUTER','PowerLost','PowerLost');
CALL osae_sp_object_type_event_add ('COMPUTER','PowerRestored','PowerRestored');
CALL osae_sp_object_type_property_add ('COMPUTER','PowerLineStatus','String','','',0);
CALL osae_sp_object_type_property_add ('COMPUTER','BatteryChargeStatus','String','','',0);
CALL osae_sp_object_type_property_add ('COMPUTER','BatteryFullLifeTime','Integer','','',0);
CALL osae_sp_object_type_property_add ('COMPUTER','BatteryLifePercent','Integer','','',0);
CALL osae_sp_object_type_property_add ('COMPUTER','BatteryLifeRemaining','Integer','','',0);