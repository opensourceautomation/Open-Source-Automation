CALL osae_sp_object_type_add ('PHIDGET-IK','Phidget Interface Kit Plugin','PHIDGET-IK','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','PHIDGET-IK');
CALL osae_sp_object_type_state_add ('OFF','Stopped','PHIDGET-IK');
CALL osae_sp_object_type_event_add ('ON','Started','PHIDGET-IK');
CALL osae_sp_object_type_event_add ('OFF','Stopped','PHIDGET-IK');
CALL osae_sp_object_type_method_add ('ON','Start','PHIDGET-IK','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','PHIDGET-IK','','','','');
CALL osae_sp_object_type_property_add ('Computer Name','String','','PHIDGET-IK',0);
CALL osae_sp_object_type_property_add ('Serial','String','','PHIDGET-IK',0);
CALL osae_sp_object_type_property_add ('Version','String','','PHIDGET-IK',0);
CALL osae_sp_object_type_property_add ('Attached','Boolean','FALSE','PHIDGET-IK',0);
CALL osae_sp_object_type_property_add ('Name','String','','PHIDGET-IK',0);
CALL osae_sp_object_type_property_add ('Digital Inputs','Integer','','PHIDGET-IK',0);
CALL osae_sp_object_type_property_add ('Digital Outputs','Integer','','PHIDGET-IK',0);
CALL osae_sp_object_type_property_add ('Analog Inputs','Integer','','PHIDGET-IK',0);
CALL osae_sp_object_type_property_add ('Sensitivity','Integer','','PHIDGET-IK',0);
CALL osae_sp_object_type_property_add ('Ratiometric','Boolean','','PHIDGET-IK',0);


CALL osae_sp_object_type_add ('PHIDGET ANALOG INPUT','Phidget Analog Input','PHIDGET-IK','PHIDGET ANALOG INPUT',0,0,0,0);
CALL osae_sp_object_type_state_add ('ON','On','PHIDGET ANALOG INPUT');
CALL osae_sp_object_type_state_add ('OFF','Off','PHIDGET ANALOG INPUT');
CALL osae_sp_object_type_event_add ('ON','On','PHIDGET ANALOG INPUT');
CALL osae_sp_object_type_event_add ('OFF','Off','PHIDGET ANALOG INPUT');
CALL osae_sp_object_type_property_add ('Formula','String','','PHIDGET ANALOG INPUT',0);
CALL osae_sp_object_type_property_add ('Analog Value','String','','PHIDGET ANALOG INPUT',0);


CALL osae_sp_object_type_add ('PHIDGET DIGITAL INPUT','Phidget Digital Input','PHIDGET-IK','PHIDGET DIGITAL INPUT',0,0,0,0);
CALL osae_sp_object_type_state_add ('ON','On','PHIDGET DIGITAL INPUT');
CALL osae_sp_object_type_state_add ('OFF','Off','PHIDGET DIGITAL INPUT');
CALL osae_sp_object_type_event_add ('ON','On','PHIDGET DIGITAL INPUT');
CALL osae_sp_object_type_event_add ('OFF','Off','PHIDGET DIGITAL INPUT');


CALL osae_sp_object_type_add ('PHIDGET DIGITAL INPUT OC','Phidget Digital Input Opened Closed','PHIDGET-IK','PHIDGET DIGITAL INPUT',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Closed','PHIDGET DIGITAL INPUT OC');
CALL osae_sp_object_type_state_add ('OFF','Opened','PHIDGET DIGITAL INPUT OC');
CALL osae_sp_object_type_event_add ('ON','Closed','PHIDGET DIGITAL INPUT OC');
CALL osae_sp_object_type_event_add ('OFF','Opened','PHIDGET DIGITAL INPUT OC');


CALL osae_sp_object_type_add ('PHIDGET DIGITAL OUTPUT','Phidget Digital Output','PHIDGET-IK','PHIDGET DIGITAL OUTPUT',0,0,0,0);
CALL osae_sp_object_type_state_add ('ON','On','PHIDGET DIGITAL OUTPUT');
CALL osae_sp_object_type_state_add ('OFF','Off','PHIDGET DIGITAL OUTPUT');
CALL osae_sp_object_type_event_add ('ON','On','PHIDGET DIGITAL OUTPUT');
CALL osae_sp_object_type_event_add ('OFF','Off','PHIDGET DIGITAL OUTPUT');
CALL osae_sp_object_type_method_add ('ON','On','PHIDGET DIGITAL OUTPUT','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','PHIDGET DIGITAL OUTPUT','','','','');