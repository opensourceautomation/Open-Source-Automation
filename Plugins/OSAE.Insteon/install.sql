CALL osae_sp_object_type_add ('INSTEON','INSTEON','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ON','On','INSTEON');
CALL osae_sp_object_type_state_add ('OFF','Off','INSTEON');
CALL osae_sp_object_type_event_add ('ON','On','INSTEON');
CALL osae_sp_object_type_event_add ('OFF','Off','INSTEON');
CALL osae_sp_object_type_property_add ('Port','Integer','','INSTEON',0);
CALL osae_sp_object_type_property_add ('System Plugin','Boolean','FALSE','INSTEON',0);

CALL osae_sp_object_type_add ('INSTEON APPLIANCELINC','Insteon Appliancelinc','','INSTEON APPLIANCELINC',0,1,0,0);
CALL osae_sp_object_type_state_add ('ON','On','INSTEON APPLIANCELINC');
CALL osae_sp_object_type_state_add ('OFF','Off','INSTEON APPLIANCELINC');
CALL osae_sp_object_type_event_add ('ON','On','INSTEON APPLIANCELINC');
CALL osae_sp_object_type_event_add ('OFF','Off','INSTEON APPLIANCELINC');
CALL osae_sp_object_type_method_add ('ON','On','INSTEON APPLIANCELINC','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','INSTEON APPLIANCELINC','','','','');

