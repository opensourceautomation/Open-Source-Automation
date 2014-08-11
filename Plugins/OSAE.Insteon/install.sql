CALL osae_sp_object_type_add ('INSTEON','INSTEON PLM PLUGIN','Insteon','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add('ON','On','INSTEON');
CALL osae_sp_object_type_state_add('OFF','Off','INSTEON');
CALL osae_sp_object_type_event_add('ON','On','INSTEON');
CALL osae_sp_object_type_event_add('OFF','Off','INSTEON');
CALL osae_sp_object_type_property_add('Port','Integer','','INSTEON',0);
CALL osae_sp_object_type_property_add('System Plugin','Boolean','FALSE','INSTEON',0);
CALL osae_sp_object_type_property_add('Debug','Boolean','FALSE','INSTEON',0);

CALL osae_sp_object_type_add ('INSTEON APPLIANCELINC','Insteon Appliancelinc','','INSTEON APPLIANCELINC',0,1,0,0);
CALL osae_sp_object_type_state_add ('ON','On','INSTEON APPLIANCELINC');
CALL osae_sp_object_type_state_add ('OFF','Off','INSTEON APPLIANCELINC');
CALL osae_sp_object_type_event_add ('ON','On','INSTEON APPLIANCELINC');
CALL osae_sp_object_type_event_add ('OFF','Off','INSTEON APPLIANCELINC');
CALL osae_sp_object_type_method_add ('ON','On','INSTEON APPLIANCELINC','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','INSTEON APPLIANCELINC','','','','');

CALL osae_sp_object_type_add ('INSTEON DIMMER','Insteon Dimmer','Insteon','MULTILEVEL SWITCH',0,0,0,0);
CALL osae_sp_object_type_state_add('ON','On','INSTEON DIMMER');
CALL osae_sp_object_type_state_add('OFF','Off','INSTEON DIMMER');
CALL osae_sp_object_type_event_add('ON','On','INSTEON DIMMER');
CALL osae_sp_object_type_event_add('OFF','Off','INSTEON DIMMER');
CALL osae_sp_object_type_method_add('ON','On','INSTEON DIMMER','Dim Level in %','','100','');
CALL osae_sp_object_type_method_add('OFF','Off','INSTEON DIMMER','','','','');
CALL osae_sp_object_type_method_add('BRIGHT','Bright','INSTEON DIMMER','','','','');
CALL osae_sp_object_type_method_add('DIM','Dim','INSTEON DIMMER','','','','');
CALL osae_sp_object_type_property_add('Off Timer','Integer','','INSTEON DIMMER',0);
CALL osae_sp_object_type_property_add('Level','Integer','','INSTEON DIMMER',0);