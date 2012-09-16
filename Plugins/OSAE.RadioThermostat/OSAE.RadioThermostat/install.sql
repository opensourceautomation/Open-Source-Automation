CALL osae_sp_object_type_add ('RADIO THERMOSTAT','Radio Thermostat Plugin','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','RADIO THERMOSTAT');
CALL osae_sp_object_type_state_add ('OFF','Stopped','RADIO THERMOSTAT');
CALL osae_sp_object_type_event_add ('ON','Started','RADIO THERMOSTAT');
CALL osae_sp_object_type_event_add ('OFF','Stopped','RADIO THERMOSTAT');
CALL osae_sp_object_type_method_add ('ON','Start','RADIO THERMOSTAT','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','RADIO THERMOSTAT','','','','');
CALL osae_sp_object_type_property_add('Poll Interval', 'Integer', '', 'RADIO THERMOSTAT', 0);

CALL osae_sp_object_type_add ('RADIO THERMOSTAT DEVICE','Radio Thermostat Device','','',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','RADIO THERMOSTAT DEVICE');
CALL osae_sp_object_type_state_add ('OFF','Off','RADIO THERMOSTAT DEVICE');
CALL osae_sp_object_type_event_add ('ON','On','RADIO THERMOSTAT DEVICE');
CALL osae_sp_object_type_event_add ('OFF','Off','RADIO THERMOSTAT DEVICE');
CALL osae_sp_object_type_method_add ('SET TEMPORARY COOL','Set Temporary Cool Temp','RADIO THERMOSTAT DEVICE','Temperature','','','');
CALL osae_sp_object_type_method_add ('SET TEMPORARY HEAT','Set Temporary Heat Temp','RADIO THERMOSTAT DEVICE','Temperature','','','');
CALL osae_sp_object_type_method_add ('SET HOLD','Set Hold','RADIO THERMOSTAT DEVICE','','','','');
CALL osae_sp_object_type_method_add ('REMOVE HOLD','Remove Hold','RADIO THERMOSTAT DEVICE','','','','');
CALL osae_sp_object_type_method_add ('REBOOT','Reboot Device','RADIO THERMOSTAT DEVICE','','','','');
CALL osae_sp_object_type_method_add ('SET LED','Set LED Color','RADIO THERMOSTAT DEVICE','Color','','','');
CALL osae_sp_object_type_property_add('Current Temperature', 'Float', '', 'RADIO THERMOSTAT DEVICE', 0);
CALL osae_sp_object_type_property_add('Thermostat State', 'String', '', 'RADIO THERMOSTAT DEVICE', 0);
CALL osae_sp_object_type_property_add('Fan State', 'String', '', 'RADIO THERMOSTAT DEVICE', 0);
CALL osae_sp_object_type_property_add('Set Temperature', 'Float', '', 'RADIO THERMOSTAT DEVICE', 0);
CALL osae_sp_object_type_property_add('Thermostat Mode', 'String', '', 'RADIO THERMOSTAT DEVICE', 0);
CALL osae_sp_object_type_property_add('Fan Mode', 'String', '', 'RADIO THERMOSTAT DEVICE', 0);
CALL osae_sp_object_type_property_add('Hold', 'String', '', 'RADIO THERMOSTAT DEVICE', 0);
CALL osae_sp_object_type_property_add('Override', 'String', '', 'RADIO THERMOSTAT DEVICE', 0);


