CALL osae_sp_object_type_add ('ZWAVE','ZWave Plugin','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ZWAVE','ON','Running');
CALL osae_sp_object_type_state_add ('ZWAVE','OFF','Stopped');
CALL osae_sp_object_type_event_add ('ZWAVE','ON','Started');
CALL osae_sp_object_type_event_add ('ZWAVE','OFF','Stopped');
CALL osae_sp_object_type_method_add ('ZWAVE','ON','Start','','','','');
CALL osae_sp_object_type_method_add ('ZWAVE','OFF','Stop','','','','');
CALL osae_sp_object_type_method_add ('ZWAVE','REMOVE FAILED NODE','Remove Failed Node','Object Address','','','');
CALL osae_sp_object_type_method_add ('ZWAVE','ADD CONTROLLER','Add Controller','','','','');
CALL osae_sp_object_type_method_add ('ZWAVE','ADD DEVICE','Add Device','','','','');
CALL osae_sp_object_type_method_add ('ZWAVE','REMOVE CONTROLLER','Remove Controller','','','','');
CALL osae_sp_object_type_method_add ('ZWAVE','REMOVE DEVICE','Remove Device','','','','');
CALL osae_sp_object_type_method_add ('ZWAVE','RESET CONTROLLER','Reset Controller','','','','');
CALL osae_sp_object_type_method_add ('ZWAVE','NODE NEIGHBOR UPDATE','Node Neighbor Update','Object Address','','','');
CALL osae_sp_object_type_method_add ('ZWAVE','NETWORK UPDATE','Network Update','','','','');
CALL osae_sp_object_type_property_add ('ZWAVE','Port','Integer','','',0);
CALL osae_sp_object_type_property_add ('ZWAVE','Home ID','String','','',0);
CALL osae_sp_object_type_property_add ('ZWAVE','Polling Interval','Integer','','',0);


CALL osae_sp_object_type_add ('ZWAVE DEVICE','ZWave Device','','ZWAVE DEVICE',0,0,0,1);