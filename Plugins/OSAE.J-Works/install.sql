CALL osae_sp_object_type_add ('JWORKS','J-Works Plugin','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','JWORKS');
CALL osae_sp_object_type_state_add ('OFF','Stopped','ZJWORKS');
CALL osae_sp_object_type_event_add ('ON','Started','JWORKS');
CALL osae_sp_object_type_event_add ('OFF','Stopped','JWORKS');
CALL osae_sp_object_type_method_add ('ON','Start','JWORKS','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','JWORKS','','','','');
CALL osae_sp_object_type_method_add ('POLL','Poll Devices now','JWORKS','','','','');
CALL osae_sp_object_type_property_add ('Polling Interval','Integer','','JWORKS',0);
CALL osae_sp_object_type_property_add ('Computer Name','String','','JWORKS',0);

CALL osae_sp_object_type_add ('JWORKS INPUT','J-Works Input','','JWORKS INPUT',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','JWORKS INPUT');
CALL osae_sp_object_type_state_add ('OFF','Off','JWORKS INPUT');
CALL osae_sp_object_type_event_add ('ON','On','JWORKS INPUT');
CALL osae_sp_object_type_event_add ('OFF','Off','JWORKS INPUT');
CALL osae_sp_object_type_property_add ('Serial','String','','JWORKS INPUT',0);
CALL osae_sp_object_type_property_add ('Id','String','','JWORKS INPUT',0);
CALL osae_sp_object_type_property_add ('Invert','Boolean','','JWORKS INPUT',0);

CALL osae_sp_object_type_add ('JWORKS OUTPUT','J-Works Output','','JWORKS OUTPUT',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','JWORKS OUTPUT');
CALL osae_sp_object_type_state_add ('OFF','Off','JWORKS OUTPUT');
CALL osae_sp_object_type_event_add ('ON','On','JWORKS OUTPUT');
CALL osae_sp_object_type_event_add ('OFF','Off','JWORKS OUTPUT');
CALL osae_sp_object_type_method_add ('ON','On','JWORKS OUTPUT','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','JWORKS OUTPUT','','','','');
CALL osae_sp_object_type_property_add ('Serial','String','','JWORKS OUTPUT',0);
CALL osae_sp_object_type_property_add ('Id','String','','JWORKS OUTPUT',0);
