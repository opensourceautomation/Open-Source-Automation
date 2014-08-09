CALL osae_sp_object_type_add ('Sony','Sony Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','Sony');
CALL osae_sp_object_type_state_add ('OFF','Stopped','Sony');
CALL osae_sp_object_type_method_add ('DISCOVERY','Discovery','Sony','','','','');
CALL osae_sp_object_type_property_add('Refresh','String','1','Sony',0);