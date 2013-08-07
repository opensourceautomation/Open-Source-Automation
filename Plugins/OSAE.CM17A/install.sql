CALL osae_sp_object_type_add ('CM17A','X10 CM17A FireCracker','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','CM17A');
CALL osae_sp_object_type_state_add ('OFF','Stopped','CM17A');
CALL osae_sp_object_type_event_add ('ON','Started','CM17A');
CALL osae_sp_object_type_event_add ('OFF','Stopped','CM17A');
CALL osae_sp_object_type_method_add ('ON','Start','CM17A','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','CM17A','','','','');
CALL osae_sp_object_type_property_add ('Port','Integer','1','CM17A',0);
