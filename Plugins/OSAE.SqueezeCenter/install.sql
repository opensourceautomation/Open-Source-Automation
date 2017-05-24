CALL osae_sp_object_type_add ('SQUEEZEBOX','Squeezebox Device','','THING',0,0,0,1,'Squeezebox Device.
Squeezebox devices are normally used to play or stream Audio.');
CALL osae_sp_object_type_state_add('SQUEEZEBOX','ON','On','This Squeezebox device is ON.');
CALL osae_sp_object_type_state_add('SQUEEZEBOX','OFF','Off','This Squeezebox device is OFF.');
CALL osae_sp_object_type_event_add('SQUEEZEBOX','ON','On','This Squeezebox turned On.');
CALL osae_sp_object_type_event_add('SQUEEZEBOX','OFF','Off','This Squeezebox turned Off.');
CALL osae_sp_object_type_method_add('SQUEEZEBOX','PLAY','Play','Item','','','','Run command with parameter 1.');
CALL osae_sp_object_type_method_add('SQUEEZEBOX','STOP','Stop','','','','','Send a Stop command.');
CALL osae_sp_object_type_method_add('SQUEEZEBOX','SHOW','Display Message','message','','','','Send a Show Message command with parameter 1.');

CALL osae_sp_object_type_add ('SQUEEZEBOX SERVER','Squeezebox Server','','PLUGIN',1,1,0,1,'Squeezebox Server Plugin');
CALL osae_sp_object_type_state_add ('SQUEEZEBOX SERVER','ON','Running','Squeezebox Server Plugin is Running');
CALL osae_sp_object_type_state_add ('SQUEEZEBOX SERVER','OFF','Stopped','Squeezebox Server Plugin is Stopped');
CALL osae_sp_object_type_event_add ('SQUEEZEBOX SERVER','ON','Started','Squeezebox Server Plugin Started');
CALL osae_sp_object_type_event_add ('SQUEEZEBOX SERVER','OFF','Stopped','Squeezebox Server Plugin Stopped');
CALL osae_sp_object_type_method_add ('SQUEEZEBOX SERVER','ON','Start','','','','','Start the Squeezebox Server Plugin');
CALL osae_sp_object_type_method_add ('SQUEEZEBOX SERVER','OFF','Stop','','','','','Stop the Squeezebox Server Plugin');
CALL osae_sp_object_type_property_add ('SQUEEZEBOX SERVER','Server Address','String','','',0,1,'Server Address');
CALL osae_sp_object_type_property_add ('SQUEEZEBOX SERVER','CLI Port','String','','9090',0,1,'CLI Port');
CALL osae_sp_object_type_property_add ('SQUEEZEBOX SERVER','TTS Save Path','String','','',0,1,'TTS Save Path');
CALL osae_sp_object_type_property_add ('SQUEEZEBOX SERVER','TTS Play Path','String','','',0,1,'TTS Play Path');
CALL osae_sp_object_type_property_add('SQUEEZEBOX SERVER','Trust Level','Integer','','90',0,1,'Squeezebox Server plugin Trust Level');
CALL osae_sp_object_type_property_add('SQUEEZEBOX SERVER','Version','String','','',0,1,'Version of the Squeezebox Server plugin');
CALL osae_sp_object_type_property_add('SQUEEZEBOX SERVER','Author','String','','',0,1,'Author of the Squeezebox Server plugin');



