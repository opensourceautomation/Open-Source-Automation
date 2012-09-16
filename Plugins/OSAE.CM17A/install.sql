CALL osae_sp_object_type_add ('CM17A','X10 CM17A FireCracker','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','CM17A');
CALL osae_sp_object_type_state_add ('OFF','Stopped','CM17A');
CALL osae_sp_object_type_event_add ('ON','Started','CM17A');
CALL osae_sp_object_type_event_add ('OFF','Stopped','CM17A');
CALL osae_sp_object_type_method_add ('ON','Start','CM17A','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','CM17A','','','','');
CALL osae_sp_object_type_property_add ('Port','Integer','1','CM17A',0);

CALL osae_sp_object_type_add ('X10 RELAY','X10 Dimmer','CM17A','X10 RELAY',1,1,0,0);
CALL osae_sp_object_type_state_add ('ON','On','X10 RELAY');
CALL osae_sp_object_type_state_add ('OFF','Off','X10 RELAY');
CALL osae_sp_object_type_event_add ('ON','On','X10 RELAY');
CALL osae_sp_object_type_event_add ('OFF','Off','X10 RELAY');
CALL osae_sp_object_type_method_add ('ON','On','X10 RELAY','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','X10 RELAY','','','','');
CALL osae_sp_object_type_method_add ('TOGGLE','Toggle','X10 RELAY','','','','');
CALL osae_sp_object_type_property_add ('Off Timer','Integer','','X10 RELAY',0);

CALL osae_sp_object_type_add ('X10 DIMMER','X10 Dimmer','','X10 DIMMER',1,1,0,0);
CALL osae_sp_object_type_state_add ('ON','On','X10 DIMMER');
CALL osae_sp_object_type_state_add ('OFF','Off','X10 DIMMER');
CALL osae_sp_object_type_event_add ('ON','On','X10 DIMMER');
CALL osae_sp_object_type_event_add ('OFF','Off','X10 DIMMER');
CALL osae_sp_object_type_method_add ('ON','On','X10 DIMMER','Dim Level in %','','100','');
CALL osae_sp_object_type_method_add ('OFF','Off','X10 DIMMER','','','','');
CALL osae_sp_object_type_method_add ('BRIGHT','Bright','X10 DIMMER','Increment %','','10','');
CALL osae_sp_object_type_method_add ('DIM','Dim','X10 DIMMER','Decrement %','','10','');
CALL osae_sp_object_type_method_add ('TOGGLE','Toggle','X10 DIMMER','','','','');
CALL osae_sp_object_type_property_add ('Off Timer','Integer','','X10 DIMMER',0);
CALL osae_sp_object_type_property_add ('Level','Integer','','X10 DIMMER',1);

CALL osae_sp_object_type_add ('X10 SENSOR','X10 Sensor (MS16A, etc)','','X10 SENSOR',1,1,0,0);
CALL osae_sp_object_type_state_add ('ON','On','X10 SENSOR');
CALL osae_sp_object_type_state_add ('OFF','Off','X10 SENSOR');
CALL osae_sp_object_type_event_add ('ON','On','X10 SENSOR');
CALL osae_sp_object_type_event_add ('OFF','Off','X10 SENSOR');
CALL osae_sp_object_type_property_add ('OFF TIMER','Integer','','X10 SENSOR',0);

CALL osae_sp_object_type_add ('X10 DS10A','X10 DS10A','','X10 DS10A',1,1,0,1);
CALL osae_sp_object_type_state_add ('OPENED','Opened','X10 DS10A');
CALL osae_sp_object_type_state_add ('CLOSED','Closed','X10 DS10A');
CALL osae_sp_object_type_event_add ('OPENED','Opened','X10 DS10A');
CALL osae_sp_object_type_event_add ('CLOSED','Closed','X10 DS10A');




