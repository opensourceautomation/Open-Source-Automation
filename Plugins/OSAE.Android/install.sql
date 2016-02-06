CALL osae_sp_object_type_add ('ANDROID','Android Plugin','Android','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('ANDROID','ON','Running');
CALL osae_sp_object_type_state_add('ANDROID','OFF','Stopped');
CALL osae_sp_object_type_event_add('ANDROID','ON','Started');
CALL osae_sp_object_type_event_add('ANDROID','OFF','Stopped');
CALL osae_sp_object_type_method_add('ANDROID','NOTIFYALL','Notify All','message','category','','default');
CALL osae_sp_object_type_method_add('ANDROID','EXECUTEALL','Execute All','task','','','');
CALL osae_sp_object_type_method_add('ANDROID','ON','Start','','','','');
CALL osae_sp_object_type_method_add('ANDROID','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add('ANDROID','Version','String','','',0);
CALL osae_sp_object_type_property_add('ANDROID','Author','String','','',0);

CALL osae_sp_object_type_add('ANDROID DEVICE', 'Android Device', 'ANDROID', 'Android Device', 0, 0, 0, 1);
CALL osae_sp_object_type_state_add ('ANDROID DEVICE','ON','On');
CALL osae_sp_object_type_state_add ('ANDROID DEVICE','OFF','Off');
CALL osae_sp_object_type_event_add ('ANDROID DEVICE','ON','On');
CALL osae_sp_object_type_event_add ('ANDROID DEVICE','OFF','Off');
CALL osae_sp_object_type_method_add ('ANDROID DEVICE','NOTIFY','Notify','message','category','','default');
CALL osae_sp_object_type_method_add ('ANDROID DEVICE','EXECUTE','Execute','task','','','');
CALL osae_sp_object_type_property_add ('ANDROID DEVICE','Owner','String','','',0);
CALL osae_sp_object_type_property_add ('ANDROID DEVICE','GCMID','String','','',0);


