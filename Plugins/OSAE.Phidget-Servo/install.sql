CALL osae_sp_object_type_add ('PHIDGET-SERVO','Phidget Servo Controller','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','PHIDGET-SERVO');
CALL osae_sp_object_type_state_add ('OFF','Stopped','PHIDGET-SERVO');
CALL osae_sp_object_type_event_add ('ON','Started','PHIDGET-SERVO');
CALL osae_sp_object_type_event_add ('OFF','Stopped','PHIDGET-SERVO');
CALL osae_sp_object_type_method_add ('ON','Start','PHIDGET-SERVO','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','PHIDGET-SERVO','','','','');
CALL osae_sp_object_type_property_add ('Attached','Boolean','','PHIDGET-SERVO',0);
CALL osae_sp_object_type_property_add ('Computer Name','String','','PHIDGET-SERVO',0);
CALL osae_sp_object_type_property_add ('Serial','String','','PHIDGET-SERVO',0);
CALL osae_sp_object_type_property_add ('Position','Integer','','PHIDGET-SERVO',0);
CALL osae_sp_object_type_property_add ('Default Position','Integer','','PHIDGET-SERVO',0);


CALL osae_sp_object_type_add ('PHIDGET SERVO','Phidget Servo','Phidget-Servo-VanMain','PHIDGET SERVO',0,0,0,0);
CALL osae_sp_object_type_method_add ('POSITION','Set Position','PHIDGET SERVO','Position','','','');
CALL osae_sp_object_type_property_add ('Default Position','Integer','170','PHIDGET SERVO',0);
CALL osae_sp_object_type_property_add ('Position','Integer','170','PHIDGET SERVO',0);
CALL osae_sp_object_type_property_add ('Min Position','Integer','0','PHIDGET SERVO',0);
CALL osae_sp_object_type_property_add ('Max Position','Integer','240','PHIDGET SERVO',0);