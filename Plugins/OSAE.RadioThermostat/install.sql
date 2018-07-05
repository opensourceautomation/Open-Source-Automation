CALL osae_sp_object_type_add ('RADIO THERMOSTAT','Radio Thermostat Plugin','RADIO THERMOSTAT','PLUGIN',1,1,0,1,'This repersents the Radio Thermostat Plugin');
CALL osae_sp_object_type_state_add ('RADIO THERMOSTAT','ON','Running','The Radio Thermostat plugin IS Running');
CALL osae_sp_object_type_state_add ('RADIO THERMOSTAT','OFF','Stopped','The Radio Thermostat plugin is NOT Running');
CALL osae_sp_object_type_event_add ('RADIO THERMOSTAT','ON','Started','Occures when The Radio Thermostat has Started');
CALL osae_sp_object_type_event_add ('RADIO THERMOSTAT','OFF','Stopped','Occures when The Radio Thermostat has Stopped');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT','ON','Start','','','','','Executes a Start command on the Radio Thermostat.');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT','OFF','Stop','','','','','Executes a Stop command on the Radio Thermostat.');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT','Poll Interval','Integer','','60',0,0,'Set the number of seconds to poll devices');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT','Trust Level','Integer','','90',0,0,'Set the Trust Level for this plugin');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT','Version','String','','',0,0,'The current version of the Radio Thermostat plugin');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT','Author','String','','',0,0,'The Author of the Radio Thermostat plugin');

CALL osae_sp_object_type_add ('RADIO THERMOSTAT DEVICE','Radio Thermostat Device','RADIO THERMOSTAT','PLUGIN',0,0,0,1,'Represents a Radio Thermostat Device');
CALL osae_sp_object_type_state_add ('RADIO THERMOSTAT DEVICE','ON','On','The Radio Thermostat device is ON');
CALL osae_sp_object_type_state_add ('RADIO THERMOSTAT DEVICE','OFF','Off','The Radio Thermostat device is OFF');
CALL osae_sp_object_type_event_add ('RADIO THERMOSTAT DEVICE','ON','On','Occures when the Radio Thermostat device State changes to ON');
CALL osae_sp_object_type_event_add ('RADIO THERMOSTAT DEVICE','OFF','Off','Occures when the Radio Thermostat device State changes to OFF');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','SET TEMPORARY COOL','Set Temporary Cool Temp','Temperature','','','','Execute a SET COOL TEMP with the provided parameter value as Temp to set');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','SET TEMPORARY HEAT','Set Temporary Heat Temp','Temperature','','','','Execute a SET HEAT TEMP with the provided parameter value as Temp to set');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','SET HOLD','Set Hold','','','','','Executes a Set Hold command on the Radio Thermostat Device');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','REMOVE HOLD','Remove Hold','','','','','Executes a Remove Hold command on the Radio Thermostat Device');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','REBOOT','Reboot Device','','','','','Executes a Reboot command on the Radio Thermostat Device');
CALL osae_sp_object_type_method_add ('RADIO THERMOSTAT DEVICE','SET LED','Set LED Color','Color','','','','Executes a Set LED Color command on the Radio Thermostat Device');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Current Temperature','Float','','0',0,0,'The current Temperature of the Radio Thermostat Device');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Thermostat State','String','','',0,0,'The current Thermostat State of the Radio Thermostat Device');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Fan State','String','','',0,0,'The current Fan State of the Radio Thermostat Device');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Set Temperature','Float','','0',0,0,'The current Set Temperature of the Radio Thermostat Device');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Thermostat Mode','String','','',0,0,'The current Mode of the Radio Thermostat Device');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Fan Mode','String','','',0,0,'The current Fan Mode of the Radio Thermostat Device');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Hold','String','','',0,0,'The current Hold of the Radio Thermostat Device');
CALL osae_sp_object_type_property_add('RADIO THERMOSTAT DEVICE','Override','String','','',0,0,'The current Override of the Radio Thermostat Device');


