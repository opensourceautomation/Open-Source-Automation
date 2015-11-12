CALL osae_sp_object_type_add ('PHIDGET-IK','Phidget Interface Kit Plugin','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add('PHIDGET-IK','ON','Running');
CALL osae_sp_object_type_state_add('PHIDGET-IK','OFF','Stopped');
CALL osae_sp_object_type_event_add('PHIDGET-IK','ON','Started');
CALL osae_sp_object_type_event_add('PHIDGET-IK','OFF','Stopped');
CALL osae_sp_object_type_method_add('PHIDGET-IK','ON','Start','','','','');
CALL osae_sp_object_type_method_add('PHIDGET-IK','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Computer Name','Object','','',0);
CALL osae_sp_object_type_property_add('PHIDGET-IK','Serial','String','','',0);
CALL osae_sp_object_type_property_add('PHIDGET-IK','Version','String','','',0);
CALL osae_sp_object_type_property_add('PHIDGET-IK','Attached','Boolean','FALSE','',0);
CALL osae_sp_object_type_property_add('PHIDGET-IK','Name','String','','',0);
CALL osae_sp_object_type_property_add('PHIDGET-IK','Digital Inputs','Integer','','',0);
CALL osae_sp_object_type_property_add('PHIDGET-IK','Digital Outputs','Integer','','',0);
CALL osae_sp_object_type_property_add('PHIDGET-IK','Analog Inputs','Integer','','',0);
CALL osae_sp_object_type_property_add('PHIDGET-IK','Sensitivity','Integer','','',0);
CALL osae_sp_object_type_property_add('PHIDGET-IK','Ratiometric','Boolean','','',0);
CALL osae_sp_object_type_property_add('PHIDGET-IK','Debug','Boolean','FALSE','',0);

CALL osae_sp_object_type_add ('PHIDGET ANALOG INPUT','Phidget Analog Input','','SENSOR',0,0,0,0);
CALL osae_sp_object_type_state_add('PHIDGET ANALOG INPUT','ON','On');
CALL osae_sp_object_type_state_add('PHIDGET ANALOG INPUT','OFF','Off');
CALL osae_sp_object_type_event_add('PHIDGET ANALOG INPUT','ON','On');
CALL osae_sp_object_type_event_add('PHIDGET ANALOG INPUT','OFF','Off');
CALL osae_sp_object_type_property_add('PHIDGET ANALOG INPUT','Formula','String','','',0);
CALL osae_sp_object_type_property_add('PHIDGET ANALOG INPUT','Analog Value','String','','',0);

CALL osae_sp_object_type_add ('PHIDGET DIGITAL INPUT','Phidget Digital Input','','SENSOR',0,0,0,0);
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL INPUT','ON','On');
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL INPUT','OFF','Off');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL INPUT','ON','On');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL INPUT','OFF','Off');

CALL osae_sp_object_type_add ('PHIDGET DIGITAL INPUT OC','Phidget Digital Input Opened Closed','','SENSOR',0,0,0,1);
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL INPUT OC','ON','Closed');
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL INPUT OC','OFF','Opened');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL INPUT OC','ON','Closed');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL INPUT OC','OFF','Opened');

CALL osae_sp_object_type_add ('PHIDGET DIGITAL OUTPUT','Phidget Digital Output','','SENSOR',0,0,0,0);
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL OUTPUT','ON','On');
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL OUTPUT','OFF','Off');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL OUTPUT','ON','On');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL OUTPUT','OFF','Off');
CALL osae_sp_object_type_method_add('PHIDGET DIGITAL OUTPUT','ON','On','','','','');
CALL osae_sp_object_type_method_add('PHIDGET DIGITAL OUTPUT','OFF','Off','','','','');