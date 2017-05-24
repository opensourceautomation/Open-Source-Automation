CALL osae_sp_object_type_add ('PHIDGET-IK','Phidget Interface Kit Plugin','','PLUGIN',1,1,0,1,'Phidget Interface Kit Plugin');
CALL osae_sp_object_type_state_add('PHIDGET-IK','ON','Running','Phidget IK plugin is Running');
CALL osae_sp_object_type_state_add('PHIDGET-IK','OFF','Stopped','Phidget IK plugin is Stopped');
CALL osae_sp_object_type_event_add('PHIDGET-IK','ON','Started','Phidget IK plugin Started');
CALL osae_sp_object_type_event_add('PHIDGET-IK','OFF','Stopped','Phidget IK plugin Stopped');
CALL osae_sp_object_type_method_add('PHIDGET-IK','ON','Start','','','','','Start the Phidget IK plugin');
CALL osae_sp_object_type_method_add('PHIDGET-IK','OFF','Stop','','','','','Stop the Phidget IK plugin');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Serial','String','','',0,0,'Serial Number');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Version','String','','',0,0,'Version');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Attached','Boolean','FALSE','',0,1,'Attached');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Name','String','','',0,0,'Name');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Digital Inputs','Integer','','',0,0,'Digital Inputs');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Digital Outputs','Integer','','',0,0,'Digital Outputs');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Analog Inputs','Integer','','',0,0,'Analog Inputs');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Sensitivity','Integer','','',0,0,'Sensitivity');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Ratiometric','Boolean','','',0,0,'Ratiometric');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Debug','Boolean','FALSE','',0,1,'Use Debug to show extra logs');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Trust Level','Integer','','90',0,1,'Trust Level 0-100');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Version','String','','',0,1,'Version of the Phidget IK plugin');
CALL osae_sp_object_type_property_add('PHIDGET-IK','Author','String','','',0,1,'Author of the Phidget IK plugin');

CALL osae_sp_object_type_add ('PHIDGET ANALOG INPUT','Phidget Analog Input','','SENSOR',0,0,0,0,'Phidget Analog Input');
CALL osae_sp_object_type_state_add('PHIDGET ANALOG INPUT','ON','On','On');
CALL osae_sp_object_type_state_add('PHIDGET ANALOG INPUT','OFF','Off','Off');
CALL osae_sp_object_type_event_add('PHIDGET ANALOG INPUT','ON','On','On');
CALL osae_sp_object_type_event_add('PHIDGET ANALOG INPUT','OFF','Off','Off');
CALL osae_sp_object_type_property_add('PHIDGET ANALOG INPUT','Formula','String','','',0,0,'Formula to apply');
CALL osae_sp_object_type_property_add('PHIDGET ANALOG INPUT','Analog Value','String','','',0,0,'Analog Value');

CALL osae_sp_object_type_add ('PHIDGET DIGITAL INPUT','Phidget Digital Input','','SENSOR',0,0,0,0,'Phidget Digital Input');
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL INPUT','ON','On','On');
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL INPUT','OFF','Off','Off');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL INPUT','ON','On','On');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL INPUT','OFF','Off','Off');

CALL osae_sp_object_type_add ('PHIDGET DIGITAL INPUT OC','Phidget Digital Input Opened Closed','','SENSOR',0,0,0,1,'Phidget Digital Input Opened Closed');
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL INPUT OC','ON','Closed','Closed');
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL INPUT OC','OFF','Opened','Opened');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL INPUT OC','ON','Closed','Closed');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL INPUT OC','OFF','Opened','Opened');

CALL osae_sp_object_type_add ('PHIDGET DIGITAL OUTPUT','Phidget Digital Output','','SENSOR',0,0,0,0,'Phidget Digital Output');
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL OUTPUT','ON','On','On');
CALL osae_sp_object_type_state_add('PHIDGET DIGITAL OUTPUT','OFF','Off','Off');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL OUTPUT','ON','On','On');
CALL osae_sp_object_type_event_add('PHIDGET DIGITAL OUTPUT','OFF','Off','Off');
CALL osae_sp_object_type_method_add('PHIDGET DIGITAL OUTPUT','ON','On','','','','','Turn On');
CALL osae_sp_object_type_method_add('PHIDGET DIGITAL OUTPUT','OFF','Off','','','','','Turn On');