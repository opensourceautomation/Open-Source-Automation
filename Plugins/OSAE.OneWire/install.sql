CALL osae_sp_object_type_add ('1WIRE','1-Wire','','PLUGIN',1,0,0,1,'1-Wire plugin');
CALL osae_sp_object_type_state_add ('1WIRE','ON','Running','1-Wire plugin is Running');
CALL osae_sp_object_type_state_add ('1WIRE','OFF','Stopped','1-Wire plugin is Stopped');
CALL osae_sp_object_type_event_add ('1WIRE','ON','Started','1-Wire plugin Started');
CALL osae_sp_object_type_event_add ('1WIRE','OFF','Stopped','1-Wire plugin Stopped');
CALL osae_sp_object_type_method_add ('1WIRE','ON','Start','','','','','Start the 1-Wire plugin');
CALL osae_sp_object_type_method_add ('1WIRE','OFF','Stop','','','','','Stop the 1-Wire plugin');
CALL osae_sp_object_type_property_add ('1WIRE','Adapter','String','','',0,0,'Adapter');
CALL osae_sp_object_type_property_add ('1WIRE','Port','Integer','','4',0,0,'Port');
CALL osae_sp_object_type_property_add ('1WIRE','Unit of Measure','String','','',0,0,'Unit of Measure');
CALL osae_sp_object_type_property_add('1WIRE','Poll Interval','Integer','','60',0,0,'Poll Interval');
CALL osae_sp_object_type_property_option_add('1WIRE','Adapter','{DS9097U_DS948X}');
CALL osae_sp_object_type_property_add('1WIRE','Version','String','','',0,1,'Version of the 1WIRE plugin');
CALL osae_sp_object_type_property_add('1WIRE','Author','String','','',0,1,'Author of the 1WIRE plugin');
CALL osae_sp_object_type_property_add('1WIRE','Trust Level','Integer','','90',0,1,'1WIRE plugin Trust Level');

CALL osae_sp_object_type_add('1WIRE TEMP SENSOR','1-Wire Temperature Sensor','','1WIRE TEMP SENSOR', 0, 0, 0, 1,'1-Wire Temperature Sensor');
CALL osae_sp_object_type_event_add ('1WIRE TEMP SENSOR','TEMPERATURE','Temperature','Temperature Changed');
CALL osae_sp_object_type_property_add ('1WIRE TEMP SENSOR','Temperature','String','','',0,0,'Temperature');

