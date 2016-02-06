CALL osae_sp_object_type_add ('SSH','SSH','','PLUGIN',0,1,0,1);
CALL osae_sp_object_type_state_add ('SSH','ON','Running');
CALL osae_sp_object_type_state_add ('SSH','OFF','Stopped');
CALL osae_sp_object_type_event_add ('SSH','ON','Started');
CALL osae_sp_object_type_event_add ('SSH','OFF','Stopped');
CALL osae_sp_object_type_method_add ('SSH','EXECUTE','Execute Command','Server/Username/Password','Command','','');
CALL osae_sp_object_type_method_add ('SSH','ON','Start','','','','');
CALL osae_sp_object_type_method_add ('SSH','OFF','Stop','','','','');
