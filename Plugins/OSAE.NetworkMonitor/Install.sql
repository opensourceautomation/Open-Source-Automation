CALL osae_sp_object_type_add ('NETWORK MONITOR','Network Monitor Plugin','Network Monitor','PLUGIN',1,0,0,1,'his object represents the Network Monitor plugin.
The Network Monitor simply pings network devices to determine their availability.');
CALL osae_sp_object_type_state_add('NETWORK MONITOR','ON','Running','The Network Monitor plugin is Running.');
CALL osae_sp_object_type_state_add('NETWORK MONITOR','OFF','Stopped','The Network Monitor plugin is Stopped.');
CALL osae_sp_object_type_event_add('NETWORK MONITOR','ON','Started','The Network Monitor plugin Started.');
CALL osae_sp_object_type_event_add('NETWORK MONITOR','OFF','Stopped','The Network Monitor plugin Stopped.');
CALL osae_sp_object_type_method_add('NETWORK MONITOR','ON','Start','','','','','Start the Network Monitor plugin.');
CALL osae_sp_object_type_method_add('NETWORK MONITOR','OFF','Stop','','','','','Stop the Network Monitor plugin.');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','System Plugin','Boolean','','TRUE',0,0,'Is the Network Monitor Plugin is a system plugin?');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Poll Interval','Integer','','30',0,1,'Enter the number of seconds to poll for connected devices,');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Debug','Boolean','','FALSE',0,1,'Add extra logging to help with Debugging..');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Version','String','','',0,0,'Version of the Network Monitor Plugin.');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Author','String','','',0,0,'Author of the Network Monitor Plugin.');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Trust Level','Integer','','90',0,1,'The Trust Level of the Network Monitor plugin.');

CALL osae_sp_object_type_add ('NETWORK DEVICE','Network Device','','THING',0,0,0,1,'This object represents a Network device.
A Network device can be any desktop, laptop or
LAN enabled device with an IP Address..');
CALL osae_sp_object_type_state_add('NETWORK DEVICE','ON','On-Line','This Network Device is On Line.');
CALL osae_sp_object_type_state_add('NETWORK DEVICE','OFF','Off-Line','This Network Device is Off Line.');
CALL osae_sp_object_type_event_add('NETWORK DEVICE','ON','On-Line','The Network device came On-Line.');
CALL osae_sp_object_type_event_add('NETWORK DEVICE','OFF','Off-Line','The Network device went Off-Line.');
CALL osae_sp_object_type_property_add('NETWORK DEVICE','IP Address','String','','',0,1,'Enter the IP Address for this Network Device.');