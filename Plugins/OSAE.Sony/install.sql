CALL osae_sp_object_type_add ('SONY','Sony Plugin','','PLUGIN',1,0,0,1);

CALL osae_sp_object_type_state_add ('SONY','ON','Running');

CALL osae_sp_object_type_state_add ('SONY','OFF','Stopped');

CALL osae_sp_object_type_method_add ('SONY','DISCOVERY','Discovery','','','','');

CALL osae_sp_object_type_property_add('SONY','Refresh','String','1','',0);