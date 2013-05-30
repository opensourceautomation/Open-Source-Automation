CALL osae_sp_object_type_add ('Android','Android Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','Android');
CALL osae_sp_object_type_state_add ('OFF','Stopped','Android');
CALL osae_sp_object_type_event_add ('ON','Started','Android');
CALL osae_sp_object_type_event_add ('OFF','Stopped','Android');
CALL osae_sp_object_type_method_add ('NOTIFYALL','Notify All','Android','message','category','','default');
CALL osae_sp_object_type_method_add ('EXECUTEALL','Execute All','Android','task','','','');

CALL osae_sp_object_type_add('Android Device', 'Android Device', 'Android', 'Android Device', 0, 0, 0, 1);
CALL osae_sp_object_type_state_add ('ON','On','Android Device');
CALL osae_sp_object_type_state_add ('OFF','Off','Android Device');
CALL osae_sp_object_type_event_add ('ON','On','Android Device');
CALL osae_sp_object_type_event_add ('OFF','Off','Android Device');

CALL osae_sp_object_type_property_add ('Owner','String','','Android Device',0);            

CALL osae_sp_object_type_method_add ('NOTIFY','Notify','Android Device','message','category','','default');
CALL osae_sp_object_type_method_add ('EXECUTE','Execute','Android Device','task','','','');
