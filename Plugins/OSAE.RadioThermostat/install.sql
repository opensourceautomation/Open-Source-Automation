CALL osae_sp_object_type_add ('RADIO THERMOSTAT','Radio Thermostat Plugin','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('RADIO THERMOSTAT','ON','Running');
CALL osae_sp_object_type_state_add ('RADIO THERMOSTAT','OFF','Stopped');
CALL osae_sp_object_type_event_add ('RADIO THERMOSTAT','ON','Started');
CALL osae_sp_object_type_event_add ('RADIO THERMOSTAT','OFF','Stopped');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT','ON','Start','','','','');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT','Poll Interval','Integer','','60',0);
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT','Version','String','','',0);
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT','Author','String','','',0);

CALL osae_sp_object_type_add ('RADIO THERMOSTAT DEVICE','Radio Thermostat Device','','',0,0,0,1);
CALL osae_sp_object_type_state_add ('RADIO THERMOSTAT DEVICE','ON','On');
CALL osae_sp_object_type_state_add ('RADIO THERMOSTAT DEVICE','OFF','Off');
CALL osae_sp_object_type_event_add ('RADIO THERMOSTAT DEVICE','ON','On');
CALL osae_sp_object_type_event_add ('RADIO THERMOSTAT DEVICE','OFF','Off');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','SET TEMPORARY COOL','Set Temporary Cool Temp','Temperature','','','');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','SET TEMPORARY HEAT','Set Temporary Heat Temp','Temperature','','','');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','SET HOLD','Set Hold','','','','');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','REMOVE HOLD','Remove Hold','','','','');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','REBOOT','Reboot Device','','','','');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','SET LED','Set LED Color','Color','','','');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Current Temperature','Float','','0',0);
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Thermostat State','String','','',0);
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Fan State','String','','',0);
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Set Temperature','Float','','0',0);
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Thermostat Mode','String','','',0);
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Fan Mode','String','','',0);
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Hold','String','','',0);
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Override', 'String','','',0);


