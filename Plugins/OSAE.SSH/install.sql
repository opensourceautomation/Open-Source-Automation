CALL osae_sp_object_type_add ('SSH','SSH','','PLUGIN',0,1,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','SSH');
CALL osae_sp_object_type_state_add ('OFF','Stopped','SSH');
CALL osae_sp_object_type_event_add ('ON','Started','SSH');
CALL osae_sp_object_type_event_add ('OFF','Stopped','SSH');
CALL osae_sp_object_type_method_add ('EXECUTE','Execute Command','SSH','Server/Username/Password','Command','','');
CALL osae_sp_object_type_method_add ('ON','Start','SSH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','SSH','','','','');
