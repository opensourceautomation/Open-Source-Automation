CALL osae_sp_object_type_add ('COMPUTER POWER','Computer Power Monitor Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','COMPUTER POWER');
CALL osae_sp_object_type_state_add ('OFF','Stopped','COMPUTER POWER');
CALL osae_sp_object_type_event_add ('ON','Started','COMPUTER POWER');
CALL osae_sp_object_type_event_add ('OFF','Stopped','COMPUTER POWER');
CALL osae_sp_object_type_method_add ('ON','Start','COMPUTER POWER','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','COMPUTER POWER','','','','');
CALL osae_sp_object_type_method_add ('UPDATE','Update','COMPUTER POWER','','','','');
CALL osae_sp_object_type_property_add ('Update Interval','Integer','30','COMPUTER POWER',0);
CALL osae_sp_object_type_property_add ('Computer Name','String','','COMPUTER POWER',0);

CALL osae_sp_object_type_event_add ('PowerLost','PowerLost','COMPUTER');
CALL osae_sp_object_type_event_add ('PowerRestored','PowerRestored','COMPUTER');
CALL osae_sp_object_type_property_add ('PowerLineStatus','String','','COMPUTER',0);
CALL osae_sp_object_type_property_add ('BatteryChargeStatus','String','','COMPUTER',0);
CALL osae_sp_object_type_property_add ('BatteryFullLifeTime','Integer','','COMPUTER',0);
CALL osae_sp_object_type_property_add ('BatteryLifePercent','Integer','','COMPUTER',0);
CALL osae_sp_object_type_property_add ('BatteryLifeRemaining','Integer','','COMPUTER',0);