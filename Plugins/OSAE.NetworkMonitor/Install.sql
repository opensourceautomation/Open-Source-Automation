CALL osae_sp_object_type_add ('NETWORK MONITOR','Network Monitor Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('NETWORK MONITOR','ON','Running');
CALL osae_sp_object_type_state_add('NETWORK MONITOR','OFF','Stopped');
CALL osae_sp_object_type_event_add('NETWORK MONITOR','ON','Started');
CALL osae_sp_object_type_event_add('NETWORK MONITOR','OFF','Stopped');
CALL osae_sp_object_type_method_add('NETWORK MONITOR','ON','Start','','','','');
CALL osae_sp_object_type_method_add('NETWORK MONITOR','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','System Plugin','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Poll Interval','Integer','','30',0);

CALL osae_sp_object_type_add ('NETWORK DEVICE','Network Device','','THING',0,0,0,1);
CALL osae_sp_object_type_state_add('NETWORK DEVICE','ON','On-Line');
CALL osae_sp_object_type_state_add('NETWORK DEVICE','OFF','Off-Line');
CALL osae_sp_object_type_event_add('NETWORK DEVICE','ON','On-Line');
CALL osae_sp_object_type_event_add('NETWORK DEVICE','OFF','Off-Line');
CALL osae_sp_object_type_property_add('NETWORK DEVICE','IP Address','String','','',0);