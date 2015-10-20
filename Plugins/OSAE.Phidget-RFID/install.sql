CALL osae_sp_object_type_add ('PHIDGET-RFID','Phidget RFID Plugin','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('PHIDGET-RFID', 'ON','Running');
CALL osae_sp_object_type_state_add ('PHIDGET-RFID', 'OFF','Stopped');
CALL osae_sp_object_type_event_add ('PHIDGET-RFID', 'ON','Started');
CALL osae_sp_object_type_event_add ('PHIDGET-RFID', 'OFF','Stopped');
CALL osae_sp_object_type_method_add ('PHIDGET-RFID', 'ON','Start','','','','');
CALL osae_sp_object_type_method_add ('PHIDGET-RFID', 'OFF','Stop','','','','');
CALL osae_sp_object_type_property_add ('PHIDGET-RFID', 'Attached','Boolean','FALSE','',0);
CALL osae_sp_object_type_property_add ('PHIDGET-RFID', 'Name','String','','',0);
CALL osae_sp_object_type_property_add ('PHIDGET-RFID', 'Version','String','','',0);
CALL osae_sp_object_type_property_add ('PHIDGET-RFID', 'Serial','String','','',0);
CALL osae_sp_object_type_property_add ('PHIDGET-RFID', 'Antenna Enabled','Boolean','FALSE','',0);
CALL osae_sp_object_type_property_add ('PHIDGET-RFID', 'LED Enabled','Boolean','TRUE','',0);
CALL osae_sp_object_type_property_add ('PHIDGET-RFID', 'Output 1 ON','Boolean','FALSE','',0);
CALL osae_sp_object_type_property_add ('PHIDGET-RFID', 'Output 2 ON','Boolean','FALSE','',0);
CALL osae_sp_object_type_property_add ('PHIDGET-RFID', 'Last Tag Read','String','','',0);


CALL osae_sp_object_type_add ('PHIDGET RFID TAG','Phidget RFID Tag','','THING',0,0,0,0);
CALL osae_sp_object_type_state_add ('PHIDGET RFID TAG', 'ON','Detected');
CALL osae_sp_object_type_event_add ('PHIDGET RFID TAG', 'ON','Detected');