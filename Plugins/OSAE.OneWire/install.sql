CALL osae_sp_object_type_add ('1WIRE','1-Wire','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','1WIRE');
CALL osae_sp_object_type_state_add ('OFF','Stopped','1WIRE');
CALL osae_sp_object_type_event_add ('ON','Started','1WIRE');
CALL osae_sp_object_type_event_add ('OFF','Stopped','1WIRE');
CALL osae_sp_object_type_method_add ('ON','Start','1WIRE','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','1WIRE','','','','');
CALL osae_sp_object_type_property_add ('Adapter','String','','1WIRE',0);
CALL osae_sp_object_type_property_add ('Port','Integer','','1WIRE',0);
CALL osae_sp_object_type_property_add ('Unit of Measure','String','','1WIRE',0);
CALL osae_sp_object_type_property_add('Computer Name', 'String', '', '1WIRE', 0);
CALL osae_sp_object_type_property_add('Poll Interval', 'Integer', '', '1WIRE', 0);
CALL osae_sp_object_type_property_option_add('1WIRE','Adapter','{DS9097U_DS948X}');

CALL osae_sp_object_type_add('1WIRE TEMP SENSOR', '1-Wire Temperature Sensor', '', '1WIRE TEMP SENSOR', 0, 0, 0, 1);
CALL osae_sp_object_type_property_add ('Temperature','String','','1WIRE TEMP SENSOR',0);
CALL osae_sp_object_type_event_add ('TEMPERATURE','Temperature','1WIRE TEMP SENSOR');
