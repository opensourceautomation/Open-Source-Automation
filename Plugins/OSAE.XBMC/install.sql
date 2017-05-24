CALL osae_sp_object_type_add ('XBMC','XBMC Plugin','','PLUGIN',1,0,0,1,'XBMC plugin.
This is used to connect and control XBMC devices.');
CALL osae_sp_object_type_state_add('XBMC','ON','Running','The XBMC plugin is Running.');
CALL osae_sp_object_type_state_add('XBMC','OFF','Stopped','The XBMC plugin is Stopped.');
CALL osae_sp_object_type_event_add('XBMC','ON','Started','The XBMC plugin Started.');
CALL osae_sp_object_type_event_add('XBMC','OFF','Stopped','The XBMC plugin Stopped.');
CALL osae_sp_object_type_method_add('XBMC','ON','Start','','','','','Start the XBMC plugin.');
CALL osae_sp_object_type_method_add('XBMC','OFF','Stop','','','','','Stop the XBMC plugin.');
CALL osae_sp_object_type_property_add('XBMC','Debug','Boolean','','FALSE',0,1,'Add extra logging to help with Debugging.');
CALL osae_sp_object_type_property_add('XBMC','Trust Level','Integer','','50',0,0,'Trust Level for the XBMC plugin.');
CALL osae_sp_object_type_property_add('XBMC','Version','String','','',0,0,'Version of the XBMC Plugin.');
CALL osae_sp_object_type_property_add('XBMC','Author','String','','',0,0,'Author of the XBMC Plugin.');

CALL osae_sp_object_type_add ('XBMC SYSTEM','XBMC System','','THING',1,0,0,1,'XBMC Device.
XBMC devices are normally used to play or stream Video.');
CALL osae_sp_object_type_state_add('XBMC SYSTEM','PLAYING','Playing','This XBMC device is Playing.');
CALL osae_sp_object_type_state_add('XBMC SYSTEM','STOPPED','Stopped','This XBMC device is Stopped.');
CALL osae_sp_object_type_state_add('XBMC SYSTEM','PAUSED','Paused','This XBMC device is Paused.');
CALL osae_sp_object_type_event_add('XBMC SYSTEM','PLAYING','Playing','This XBMC device is Playing.');
CALL osae_sp_object_type_event_add('XBMC SYSTEM','STOPPED','Stopped','This XBMC device Stopped.');
CALL osae_sp_object_type_event_add('XBMC SYSTEM','PAUSED','Paused','This XBMC device Paused.');
CALL osae_sp_object_type_method_add('XBMC SYSTEM','VPLAYPAUSE','Video-Play/Pause','','','','','Toggle the Play/Pause on the XBMC device.');
CALL osae_sp_object_type_method_add('XBMC SYSTEM','VSTOP','Video-Stop','','','','','Stop the XBMC device.');
CALL osae_sp_object_type_method_add('XBMC SYSTEM','VBIGSKIPFORWARD','Video-Big Skip Forward','','','','','Big-Skip-Forward on the XBMC device.');
CALL osae_sp_object_type_method_add('XBMC SYSTEM','VBIGSKIPBACK','Video-Big Skip Backward','','','','','Big-Skip-Backward on the XBMC device.');
CALL osae_sp_object_type_property_add('XBMC SYSTEM','IP','String','','',0,1,'IP Address of the XBMC device.');
CALL osae_sp_object_type_property_add('XBMC SYSTEM','Port','Integer','','0',0,1,'Port number of the XBMC device.');
CALL osae_sp_object_type_property_add('XBMC SYSTEM','Username','String','','',0,1,'Username required to log in to the XBMC device.');
CALL osae_sp_object_type_property_add('XBMC SYSTEM','Password','String','','',0,1,'Password required to log in to the XBMC device.');
CALL osae_sp_object_type_property_add('XBMC SYSTEM','Current Player','String','','',0,0,'Current Player in the XBMC device.');
CALL osae_sp_object_type_property_add('XBMC SYSTEM','Debug','Boolean','','FALSE',0,1,'Log extra information to assist with Debugging.');