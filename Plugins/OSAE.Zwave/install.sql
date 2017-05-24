CALL osae_sp_object_type_add ('ZWAVE','ZWave Plugin','','PLUGIN',1,1,0,1,'The ZWave plugin.
This is used to control the Zwave controller and devices.');
CALL osae_sp_object_type_state_add('ZWAVE','ON','Running','The ZWave plugin is Running.');
CALL osae_sp_object_type_state_add('ZWAVE','OFF','Stopped','The ZWave plugin is Stopped.');
CALL osae_sp_object_type_event_add('ZWAVE','ON','Started','The ZWave plugin Started.');
CALL osae_sp_object_type_event_add('ZWAVE','OFF','Stopped','The ZWave plugin Stopped.');
CALL osae_sp_object_type_method_add('ZWAVE','ON','Start','','','','','Start the ZWave plugin.');
CALL osae_sp_object_type_method_add('ZWAVE','OFF','Stop','','','','','Stop the ZWave plugin.');
CALL osae_sp_object_type_method_add ('ZWAVE','REMOVE FAILED NODE','Remove Failed Node','Object Address','','','','Remove Failed Node');
CALL osae_sp_object_type_method_add ('ZWAVE','ADD CONTROLLER','Add Controller','','','','','Add Controller');
CALL osae_sp_object_type_method_add ('ZWAVE','ADD DEVICE','Add Device','','','','','Add Device');
CALL osae_sp_object_type_method_add ('ZWAVE','REMOVE CONTROLLER','Remove Controller','','','','','Remove Controller');
CALL osae_sp_object_type_method_add ('ZWAVE','REMOVE DEVICE','Remove Device','','','','','Remove Device');
CALL osae_sp_object_type_method_add ('ZWAVE','RESET CONTROLLER','Reset Controller','','','','','Reset Controller');
CALL osae_sp_object_type_method_add ('ZWAVE','NODE NEIGHBOR UPDATE','Node Neighbor Update','Object Address','','','','Node Neighbor Update');
CALL osae_sp_object_type_method_add ('ZWAVE','NETWORK UPDATE','Network Update','','','','','Network Update');
CALL osae_sp_object_type_property_add('ZWAVE','Port','Integer','','0',0,1,'Serial Port number your ZWave controller is attached to.');
CALL osae_sp_object_type_property_add('ZWAVE','Home ID','String','','',0,1,'Home ID this ZWave plugin will use.');
CALL osae_sp_object_type_property_add('ZWAVE','System Plugin','Boolean','','FALSE',0,0,'Is the ZWave Plugin is a system plugin?');
CALL osae_sp_object_type_property_add ('ZWAVE','Polling Interval','Integer','','',0,1,'Polling Interval');
CALL osae_sp_object_type_property_add('ZWAVE','Trust Level','Integer','','50',0,0,'Trust Level for the ZWave plugin.');
CALL osae_sp_object_type_property_add('ZWAVE','Debug','Boolean','','FALSE',0,1,'Add extra logging to help with Debugging.');
CALL osae_sp_object_type_property_add('ZWAVE','Version','String','','',0,0,'Version of the ZWave Plugin.');
CALL osae_sp_object_type_property_add('ZWAVE','Author','String','','',0,0,'Author of the ZWave Plugin.');

CALL osae_sp_object_type_add ('ZWAVE DEVICE','ZWave Device','','ZWAVE DEVICE',0,0,0,1,'ZWave Device');