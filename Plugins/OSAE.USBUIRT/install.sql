CALL osae_sp_object_type_add ('USBUIRT','USBUIRT Plugin','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('USBUIRT','ON','Running');
CALL osae_sp_object_type_state_add ('USBUIRT','OFF','Stopped');
CALL osae_sp_object_type_event_add ('USBUIRT','ON','Started');
CALL osae_sp_object_type_event_add ('USBUIRT','OFF','Stopped');
CALL osae_sp_object_type_method_add ('USBUIRT','ON','Start','','','','');
CALL osae_sp_object_type_method_add ('USBUIRT','OFF','Stop','','','','');
CALL osae_sp_object_type_method_add ('USBUIRT','TRANSMIT','Transmit','IR Code','','','');
CALL osae_sp_object_type_property_add ('USBUIRT','System Plugin','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('USBUIRT','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add('USBUIRT','Version','String','','',0);
CALL osae_sp_object_type_property_add('USBUIRT','Author','String','','',0);




