CALL osae_sp_object_type_add ('W800RF','Generic Plugin Class','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add('ON','Running','W800RF');
CALL osae_sp_object_type_state_add('OFF','Stopped','W800RF');
CALL osae_sp_object_type_state_add('ERROR','Error','W800RF');
CALL osae_sp_object_type_event_add('ON','Started','W800RF');
CALL osae_sp_object_type_event_add('OFF','Stopped','W800RF');
CALL osae_sp_object_type_method_add('ON','Start','W800RF','','','','');
CALL osae_sp_object_type_method_add('OFF','Stop','W800RF','','','','');
CALL osae_sp_object_type_method_add('SET PORT','Set ComPort','W800RF','ComPort','','4','');
CALL osae_sp_object_type_method_add('SET DEBOUNCE','Set Debounce','W800RF','Inteval in ms','','90','');
CALL osae_sp_object_type_method_add('SET LEARNING MODE','Set Learning Mode','W800RF','True/False','','TRUE','');
CALL osae_sp_object_type_property_add('Port','Integer','','','W800RF',0);
CALL osae_sp_object_type_property_add('Computer Name','Object','','','W800RF',0);
CALL osae_sp_object_type_property_add('Learning Mode','Boolean','TRUE','','W800RF',0);
CALL osae_sp_object_type_property_add('Debounce','Integer','120','','W800RF',0);
CALL osae_sp_object_type_property_add('System Plugin','Boolean','FALSE','','W800RF',0);
CALL osae_sp_object_type_property_add('Debug','Boolean','FALSE','','W800RF',0);


CALL osae_sp_object_type_add ('X10 DS10A','X10 DS10A','','SENSOR',0,0,0,1);
CALL osae_sp_object_type_state_add('X10 DS10A','ON','Opened');
CALL osae_sp_object_type_state_add('X10 DS10A','OFF','Closed');
CALL osae_sp_object_type_event_add('X10 DS10A','ON','Opened');
CALL osae_sp_object_type_event_add('X10 DS10A','OFF','Closed');

CALL osae_sp_object_type_add ('X10 DIMMER','X10 Dimmer','','SWITCH',0,0,0,0);
CALL osae_sp_object_type_state_add('X10 DIMMER','ON','On');
CALL osae_sp_object_type_state_add('X10 DIMMER','OFF','Off');
CALL osae_sp_object_type_event_add('X10 DIMMER','ON','On');
CALL osae_sp_object_type_event_add('X10 DIMMER','OFF','Off');
CALL osae_sp_object_type_method_add('X10 DIMMER','ON','On','Dim Level in %','','100','');
CALL osae_sp_object_type_method_add('X10 DIMMER','OFF','Off','','','','');
CALL osae_sp_object_type_method_add('X10 DIMMER','BRIGHT','Bright','Increment %','','10','');
CALL osae_sp_object_type_method_add('X10 DIMMER','DIM','Dim','Decrement %','','10','');
CALL osae_sp_object_type_property_add('X10 DIMMER','Off Timer','Integer','','',0);
CALL osae_sp_object_type_property_add('X10 DIMMER','Level','Integer','','',0);

CALL osae_sp_object_type_add ('X10 RELAY','X10 Relay','','SWITCH',0,0,0,0);
CALL osae_sp_object_type_state_add('X10 RELAY','ON','On');
CALL osae_sp_object_type_state_add('X10 RELAY','OFF','Off');
CALL osae_sp_object_type_event_add('X10 RELAY','ON','On');
CALL osae_sp_object_type_event_add('X10 RELAY','OFF','Off');
CALL osae_sp_object_type_method_add('X10 RELAY','ON','On','','','','');
CALL osae_sp_object_type_method_add('X10 RELAY','OFF','Off','','','','');
CALL osae_sp_object_type_property_add('X10 RELAY','Off Timer','Integer','','',0);


