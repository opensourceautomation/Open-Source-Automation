CALL osae_sp_object_type_add ('MediaCenter','Windows Media Center Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','MediaCenter');
CALL osae_sp_object_type_state_add ('OFF','Stopped','MediaCenter');
CALL osae_sp_object_type_event_add ('ON','Started','MediaCenter');
CALL osae_sp_object_type_event_add ('OFF','Stopped','MediaCenter');
CALL osae_sp_object_type_method_add ('SCAN','Scan for MediaCenter Devices','MediaCenter','','','','');
CALL osae_sp_object_type_method_add ('NOTIFYALL','Notify All','MediaCenter','message','timeout','','30');
CALL osae_sp_object_type_property_add ('Poll Interval','Integer','60','MediaCenter',0);           

CALL osae_sp_object_type_add('MediaCenter Device', 'MediaCenter Device', '', 'MediaCenter Device', 0, 0, 0, 1);
CALL osae_sp_object_type_state_add ('PLAYING','Playing','MediaCenter Device');
CALL osae_sp_object_type_state_add ('STOPPED','Stopped','MediaCenter Device');
CALL osae_sp_object_type_state_add ('PAUSED','Paused','MediaCenter Device');
CALL osae_sp_object_type_state_add ('ON','On','MediaCenter Device');
CALL osae_sp_object_type_state_add ('OFF','Off','MediaCenter Device');
CALL osae_sp_object_type_event_add ('PLAYING','Playing','MediaCenter Device');
CALL osae_sp_object_type_event_add ('STOPPED','Stopped','MediaCenter Device');
CALL osae_sp_object_type_event_add ('PAUSED','Paused','MediaCenter Device');
CALL osae_sp_object_type_event_add ('ON','On','MediaCenter Device');
CALL osae_sp_object_type_event_add ('OFF','Off','MediaCenter Device');

CALL osae_sp_object_type_property_add ('Type','String','PC','MediaCenter Device',0);   
CALL osae_sp_object_type_property_add ('IP','String','','MediaCenter Device',0);           
CALL osae_sp_object_type_property_add ('Network Port','Integer','40500','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Mute','Boolean','','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Volume','String','','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Last Screen','String','','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Last Key Press','String','','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Dialog Visible','Boolean','','MediaCenter Device',0);

CALL osae_sp_object_type_property_add ('Media Type','String','','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Media Name','String','','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Media Time','String','','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Current Position','String','','MediaCenter Device',0);

CALL osae_sp_object_type_property_add ('Recording','Boolean','','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Recording Name','String','','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Recording Channel','String','','MediaCenter Device',0);
CALL osae_sp_object_type_property_add ('Number of Recordings','Integer','','MediaCenter Device',0);

CALL osae_sp_object_type_method_add ('PLAY','Play Button','MediaCenter Device','','','','');
CALL osae_sp_object_type_method_add ('STOP','Stop Button','MediaCenter Device','','','','');
CALL osae_sp_object_type_method_add ('PAUSE','Pause Button','MediaCenter Device','','','','');
CALL osae_sp_object_type_method_add ('MUTE','Mute','MediaCenter Device','','','','');
CALL osae_sp_object_type_method_add ('UNMUTE','UnMute','MediaCenter Device','','','','');
CALL osae_sp_object_type_method_add ('SETVOLUME','Set Volume','MediaCenter Device','level','','25','');
CALL osae_sp_object_type_method_add ('NOTIFY','Notify','MediaCenter Device','message','timeout','','30');
CALL osae_sp_object_type_method_add ('GOTO','Go To','MediaCenter Device','page','','','');
CALL osae_sp_object_type_method_add ('SEND_COMMAND','Send Command','MediaCenter Device','command','','','');

