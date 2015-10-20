CALL osae_sp_object_type_add ('XBMC','XBMC Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('XBMC','ON','Running');
CALL osae_sp_object_type_state_add('XBMC','OFF','Stopped');
CALL osae_sp_object_type_event_add('XBMC','ON','Started');
CALL osae_sp_object_type_event_add('XBMC','OFF','Stopped');
CALL osae_sp_object_type_method_add('XBMC','ON','Start','','','','');
CALL osae_sp_object_type_method_add('XBMC','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add('XBMC','Debug','Boolean','FALSE','',0);

CALL osae_sp_object_type_add ('XBMC SYSTEM','XBMC System','','THING',1,0,0,1);
CALL osae_sp_object_type_state_add('XBMC SYSTEM','PLAYING','Playing');
CALL osae_sp_object_type_state_add('XBMC SYSTEM','STOPPED','Stopped');
CALL osae_sp_object_type_state_add('XBMC SYSTEM','PAUSED','Paused');
CALL osae_sp_object_type_event_add('XBMC SYSTEM','PLAYING','Playing');
CALL osae_sp_object_type_event_add('XBMC SYSTEM','STOPPED','Stopped');
CALL osae_sp_object_type_event_add('XBMC SYSTEM','PAUSED','Paused');
CALL osae_sp_object_type_method_add('XBMC SYSTEM','VPLAYPAUSE','Video-Play/Pause','','','','');
CALL osae_sp_object_type_method_add('XBMC SYSTEM','VSTOP','Video-Stop','','','','');
CALL osae_sp_object_type_method_add('XBMC SYSTEM','VBIGSKIPFORWARD','Video-Big Skip Forward','','','','');
CALL osae_sp_object_type_method_add('XBMC SYSTEM','VBIGSKIPBACK','Video-Big Skip Backward','','','','');
CALL osae_sp_object_type_property_add('XBMC SYSTEM','IP','String','','',0);
CALL osae_sp_object_type_property_add('XBMC SYSTEM','Port','Integer','','',0);
CALL osae_sp_object_type_property_add('XBMC SYSTEM','Username','String','','',0);
CALL osae_sp_object_type_property_add('XBMC SYSTEM','Password','String','','',0);
CALL osae_sp_object_type_property_add('XBMC SYSTEM','Current Player','String','','',0);
CALL osae_sp_object_type_property_add('XBMC SYSTEM','Debug','Boolean','FALSE','',0);


