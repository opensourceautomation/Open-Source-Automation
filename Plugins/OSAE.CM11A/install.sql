CALL osae_sp_object_type_add ('CM11A','X10 Plugin Class','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('CM11A','ON','Running');
CALL osae_sp_object_type_state_add('CM11A','OFF','Stopped');
CALL osae_sp_object_type_event_add('CM11A','ON','Started');
CALL osae_sp_object_type_event_add('CM11A','OFF','Stopped');
CALL osae_sp_object_type_method_add('CM11A','ON','Start','','','','');
CALL osae_sp_object_type_method_add('CM11A','OFF','Stop','','','','');
CALL osae_sp_object_type_method_add('CM11A','CLEAR','Clear CM11A Memory','','','','');
CALL osae_sp_object_type_method_add('CM11A','RESET','Reset the CM11A','','','','');
CALL osae_sp_object_type_method_add('CM11A','SET POLL RATE','Set CM11a Poll Rate','Rate in ms','','30000','');
CALL osae_sp_object_type_method_add('CM11A','SET LEARNING MODE','Set Learning Mode','TRUE/FALSE','','TRUE','');
CALL osae_sp_object_type_property_add('CM11A','Port','Integer','','',0);
CALL osae_sp_object_type_property_add('CM11A','Poll Rate','Integer','','120',0);
CALL osae_sp_object_type_property_add('CM11A','Learning Mode','Boolean',',','TRUE',0);
CALL osae_sp_object_type_property_add('CM11A','System Plugin','Boolean','','FALSE',0);

CALL osae_sp_object_type_add ('X10 DIMMER','X10 Dimmer','','SWITCH',0,0,0,0);
CALL osae_sp_object_type_state_add('X10 DIMMER','ON','On');
CALL osae_sp_object_type_state_add('X10 DIMMER','OFF','Off');
CALL osae_sp_object_type_event_add('X10 DIMMER','ON','On');
CALL osae_sp_object_type_event_add('X10 DIMMER','OFF','Off');
CALL osae_sp_object_type_method_add('X10 DIMMER','ON','On','Dim Level in %','','100','');
CALL osae_sp_object_type_method_add('X10 DIMMER','OFF','Off','','','','');
CALL osae_sp_object_type_method_add('X10 DIMMER','BRIGHT','Bright','','','','');
CALL osae_sp_object_type_method_add('X10 DIMMER','DIM','Dim','','','','');
CALL osae_sp_object_type_property_add('X10 DIMMER','Off Timer','Integer','','',0);
CALL osae_sp_object_type_property_add('X10 DIMMER','Level','Integer','','',0);

CALL osae_sp_object_type_add ('X10 RELAY','X10 Relay','','SWITCH',0,0,0,0);
CALL osae_sp_object_type_state_add('X10 RELAY','ON','On');
CALL osae_sp_object_type_state_add('X10 RELAY','OFF','Off');
CALL osae_sp_object_type_event_add('X10 RELAY','ON','On');
CALL osae_sp_object_type_event_add('X10 RELAY','OFF','Off');
CALL osae_sp_object_type_method_add('X10 RELAY','ON','On','','','100','');
CALL osae_sp_object_type_method_add('X10 RELAY','OFF','Off','','','','');
CALL osae_sp_object_type_property_add('X10 RELAY','Off Timer','Integer','','',0);



