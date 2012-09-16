CALL osae_sp_object_type_add ('USBUIRT','USBUIRT Plugin','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','USBUIRT');
CALL osae_sp_object_type_state_add ('OFF','Stopped','USBUIRT');
CALL osae_sp_object_type_event_add ('ON','Started','USBUIRT');
CALL osae_sp_object_type_event_add ('OFF','Stopped','USBUIRT');
CALL osae_sp_object_type_method_add ('ON','Start','USBUIRT','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','USBUIRT','','','','');
CALL osae_sp_object_type_method_add ('TRANSMIT','Transmit','USBUIRT','IR Code','','','');
CALL osae_sp_object_type_property_add ('Computer Name','String','','USBUIRT',0);
CALL osae_sp_object_type_property_add ('System Plugin','Boolean','FALSE','USBUIRT',0);



