CALL osae_sp_object_type_add ('ANDROID','Android Plugin','Android','PLUGIN',1,0,0,1,'Android Plugin');
CALL osae_sp_object_type_state_add('ANDROID','ON','Running','Android Plugin is Running.');
CALL osae_sp_object_type_state_add('ANDROID','OFF','Stopped','Android Plugin is Stopped.');
CALL osae_sp_object_type_event_add('ANDROID','ON','Started','Android Plugin Started.');
CALL osae_sp_object_type_event_add('ANDROID','OFF','Stopped','Android Plugin Stopped.');
CALL osae_sp_object_type_method_add('ANDROID','NOTIFYALL','Notify All','message','category','','default','Notify All');
CALL osae_sp_object_type_method_add('ANDROID','EXECUTEALL','Execute All','task','','','','Execute All');
CALL osae_sp_object_type_method_add('ANDROID','ON','Start','','','','','Start the Android Plugin.');
CALL osae_sp_object_type_method_add('ANDROID','OFF','Stop','','','','','Stop the Android Plugin.');
CALL osae_sp_object_type_property_add('INSTEON','Version','String','','',0,1,'Version of the Android plugin');
CALL osae_sp_object_type_property_add('INSTEON','Author','String','','',0,1,'Author of the Android plugin');
CALL osae_sp_object_type_property_add('INSTEON','Trust Level','Integer','','90',0,1,'Android plugin Trust Level');

CALL osae_sp_object_type_add('ANDROID DEVICE','Android Device','ANDROID','Android Device',0,0,0,1,'Android Device');
CALL osae_sp_object_type_state_add ('ANDROID DEVICE','ON','On','On');
CALL osae_sp_object_type_state_add ('ANDROID DEVICE','OFF','Off','Off');
CALL osae_sp_object_type_event_add ('ANDROID DEVICE','ON','On','On');
CALL osae_sp_object_type_event_add ('ANDROID DEVICE','OFF','Off','Off');
CALL osae_sp_object_type_method_add ('ANDROID DEVICE','NOTIFY','Notify','message','category','','default','Notify');
CALL osae_sp_object_type_method_add ('ANDROID DEVICE','EXECUTE','Execute','task','','','','Execute task');
CALL osae_sp_object_type_property_add ('ANDROID DEVICE','Owner','String','','',0,0,'Owner');
CALL osae_sp_object_type_property_add ('ANDROID DEVICE','GCMID','String','','',0,0,'GCMID');


