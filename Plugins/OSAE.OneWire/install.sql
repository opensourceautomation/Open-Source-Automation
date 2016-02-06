CALL osae_sp_object_type_add ('1WIRE','1-Wire','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('1WIRE','ON','Running');
CALL osae_sp_object_type_state_add ('1WIRE','OFF','Stopped');
CALL osae_sp_object_type_event_add ('1WIRE','ON','Started');
CALL osae_sp_object_type_event_add ('1WIRE','OFF','Stopped');
CALL osae_sp_object_type_method_add ('1WIRE','ON','Start','','','','');
CALL osae_sp_object_type_method_add ('1WIRE','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add ('1WIRE','Adapter','String','','',0);
CALL osae_sp_object_type_property_add ('1WIRE','Port','Integer','','4',0);
CALL osae_sp_object_type_property_add ('1WIRE','Unit of Measure','String','','',0);
CALL osae_sp_object_type_property_add('1WIRE','Computer Name','String','','',0);
CALL osae_sp_object_type_property_add('1WIRE','Poll Interval','Integer','','60',0);
CALL osae_sp_object_type_property_option_add('1WIRE','Adapter','{DS9097U_DS948X}');
CALL osae_sp_object_type_property_add('1WIRE','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add('1WIRE','Version','String','','',0);
CALL osae_sp_object_type_property_add('1WIRE','Author','String','','',0);

CALL osae_sp_object_type_add('1WIRE TEMP SENSOR','1-Wire Temperature Sensor','','1WIRE TEMP SENSOR', 0, 0, 0, 1);
CALL osae_sp_object_type_event_add ('1WIRE TEMP SENSOR','TEMPERATURE','Temperature');
CALL osae_sp_object_type_property_add ('1WIRE TEMP SENSOR','Temperature','String','','',0);

