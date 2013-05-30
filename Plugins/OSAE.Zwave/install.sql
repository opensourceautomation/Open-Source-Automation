CALL osae_sp_object_type_add ('ZWAVE','ZWave Plugin','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','ZWAVE');
CALL osae_sp_object_type_state_add ('OFF','Stopped','ZWAVE');
CALL osae_sp_object_type_event_add ('ON','Started','ZWAVE');
CALL osae_sp_object_type_event_add ('OFF','Stopped','ZWAVE');
CALL osae_sp_object_type_method_add ('ON','Start','ZWAVE','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','ZWAVE','','','','');
CALL osae_sp_object_type_method_add ('REMOVE FAILED NODE','Remove Failed Node','ZWAVE','Object Address','','','');
CALL osae_sp_object_type_method_add ('ADD CONTROLLER','Add Controller','ZWAVE','','','','');
CALL osae_sp_object_type_method_add ('ADD DEVICE','Add Device','ZWAVE','','','','');
CALL osae_sp_object_type_method_add ('REMOVE CONTROLLER','Remove Controller','ZWAVE','','','','');
CALL osae_sp_object_type_method_add ('REMOVE DEVICE','Remove Device','ZWAVE','','','','');
CALL osae_sp_object_type_method_add ('RESET CONTROLLER','Reset Controller','ZWAVE','','','','');
CALL osae_sp_object_type_method_add ('NODE NEIGHBOR UPDATE','Node Neighbor Update','ZWAVE','Object Address','','','');
CALL osae_sp_object_type_method_add ('NETWORK UPDATE','Network Update','ZWAVE','','','','');
CALL osae_sp_object_type_property_add ('Port','Integer','','ZWAVE',0);
CALL osae_sp_object_type_property_add ('Home ID','String','','ZWAVE',0);
CALL osae_sp_object_type_property_add ('Polling Interval','Integer','','ZWAVE',0);


CALL osae_sp_object_type_add ('ZWAVE DEVICE','ZWave Device','','ZWAVE DEVICE',0,0,0,1);