
CALL osae_sp_object_type_add ('NEST','Nest Plugin','','PLUGIN',1,1,0,1);


CALL osae_sp_object_type_state_add ('NEST','ON','Running');

CALL osae_sp_object_type_state_add ('NEST','OFF','Stopped');


CALL osae_sp_object_type_event_add ('NEST','ON','Started');

CALL osae_sp_object_type_event_add ('NEST','OFF','Stopped');
CALL osae_sp_object_type_method_add ('NEST','GET ACCESS TOKEN','Get an Access Token','','','','');

CALL osae_sp_object_type_method_add ('NEST','SEARCH FOR NEW DEVICES','Search For New Devices','','','','');

CALL osae_sp_object_type_method_add ('NEST','RESTART STREAMING UPDATES','Restart the streaming updates from Nest','','','','');


CALL osae_sp_object_type_property_add ('NEST','Pin','String','','',0);

CALL osae_sp_object_type_property_add ('NEST','Access Token','String','','',0);

CALL osae_sp_object_type_property_add ('NEST','Client Version','Integer','','',0);



CALL osae_sp_object_type_property_add('NEST','Version','String','','',0);
CALL osae_sp_object_type_property_add('NEST','Author','String','','',0);
CALL osae_sp_object_type_property_add('NEST','Trust Level','Integer','','90',0);

CALL osae_sp_object_type_add ('NEST STRUCTURE','Nest Structure','NEST','NEST STRUCTURE',0,0,0,1);


CALL osae_sp_object_type_state_add ('NEST STRUCTURE','AWAY','Away');

CALL osae_sp_object_type_state_add ('NEST STRUCTURE','HOME','Home');

CALL osae_sp_object_type_state_add ('NEST STRUCTURE','AUTO-AWAY','Auto-Away');


CALL osae_sp_object_type_event_add ('NEST STRUCTURE','AWAY','State Set to Away');

CALL osae_sp_object_type_event_add ('NEST STRUCTURE','HOME','State Set to Home');

CALL osae_sp_object_type_event_add ('NEST STRUCTURE','AUTO-AWAY','State Set to Auto-Away');


CALL osae_sp_object_type_method_add ('NEST STRUCTURE','AWAY','Set State to Away','','','','');

CALL osae_sp_object_type_method_add ('NEST STRUCTURE','HOME','Set State to Home','','','','');

CALL osae_sp_object_type_method_add ('NEST STRUCTURE','SET ETA','Set ETA','Trip Name','Expected Time','','');

CALL osae_sp_object_type_method_add ('NEST STRUCTURE','CANCEL ETA','Cancel ETA','Trip Name','','','');


CALL osae_sp_object_type_property_add ('NEST STRUCTURE','Id','String','','',0);

CALL osae_sp_object_type_property_add ('NEST STRUCTURE','Name','String','','',0);

CALL osae_sp_object_type_property_add ('NEST STRUCTURE','Country Code','String','','',0);

CALL osae_sp_object_type_property_add ('NEST STRUCTURE','Postal Code','String','','',0);

CALL osae_sp_object_type_property_add ('NEST STRUCTURE','Peak Start Time','String','','',0);

CALL osae_sp_object_type_property_add ('NEST STRUCTURE','Peak End Time','String','','',0);

CALL osae_sp_object_type_property_add ('NEST STRUCTURE','Time Zone','String','','',0);





CALL osae_sp_object_type_add ('NEST THERMOSTAT','Nest Thermostat','NEST','NEST THERMOSTAT',0,0,0,1);


CALL osae_sp_object_type_state_add ('NEST THERMOSTAT','HEAT','Heat');

CALL osae_sp_object_type_state_add ('NEST THERMOSTAT','COOL','Cool');

CALL osae_sp_object_type_state_add ('NEST THERMOSTAT','HEAT-COOL','Heat-Cool');

CALL osae_sp_object_type_state_add ('NEST THERMOSTAT','OFF','Off');

CALL osae_sp_object_type_state_add ('NEST THERMOSTAT','OFFLINE','Offline');


CALL osae_sp_object_type_event_add ('NEST THERMOSTAT','HEAT','State Set to Heat');

CALL osae_sp_object_type_event_add ('NEST THERMOSTAT','COOL','State Set to Cool');

CALL osae_sp_object_type_event_add ('NEST THERMOSTAT','HEAT-COOL','State Set to Heat-Cool');

CALL osae_sp_object_type_event_add ('NEST THERMOSTAT','OFF','State Set to Off');

CALL osae_sp_object_type_event_add ('NEST THERMOSTAT','OFFLINE','State Set to Offline');


CALL osae_sp_object_type_method_add ('NEST THERMOSTAT','HEAT','Set State to Heat','','','','');

CALL osae_sp_object_type_method_add ('NEST THERMOSTAT','COOL','Set State to Cool','','','','');

CALL osae_sp_object_type_method_add ('NEST THERMOSTAT','OFF','Set State to Off','','','','');

CALL osae_sp_object_type_method_add ('NEST THERMOSTAT','HEAT-COOL','Set State to Heat-Cool','','','','');


CALL osae_sp_object_type_method_add ('NEST THERMOSTAT','SET TARGET','Set Target Temperature','Temperature','','','');

CALL osae_sp_object_type_method_add ('NEST THERMOSTAT','SET TARGET HIGH','Set Target High Temperature','Temperature','','','');

CALL osae_sp_object_type_method_add ('NEST THERMOSTAT','SET TARGET LOW','Set Target Low Temperature','Temperature','','','');

CALL osae_sp_object_type_method_add ('NEST THERMOSTAT','TURN ON FAN','Turn On Fan','','','','');

CALL osae_sp_object_type_method_add ('NEST THERMOSTAT','TURN OFF FAN','Turn Off Fan','','','','');


CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','ID','String','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Name','String','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Name Long','String','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Ambient Temperature','Float','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Humidity','Integer','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Target Temperature','Float','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Target Temperature High','Float','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Target Temperature Low','Float','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Away Temperature High','Float','','',0);
CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Away Temperature Low','Float','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Has Leaf','Boolean','','FALSE',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Temperature Scale','String','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Locale','String','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Software Version','String','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Structure ID','String','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Last Connection','String','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Is Online','Boolean','','FALSE',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Can Cool','Boolean','','TRUE',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Can Heat','Boolean','','TRUE',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Using Emergency Heat','Boolean','','FALSE',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Has Fan','Boolean','','',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Fan Timer Active','Boolean','','FALSE',0);

CALL osae_sp_object_type_property_add ('NEST THERMOSTAT','Fan Timer Timeout','String','','',0);





CALL osae_sp_object_type_add ('NEST PROTECT','Nest Protect','NEST','NEST PROTECT',0,0,0,1);


CALL osae_sp_object_type_state_add ('NEST PROTECT','SMOKE EMERGENCY','Smoke Emergency');

CALL osae_sp_object_type_state_add ('NEST PROTECT','SMOKE WARNING','Smoke Warning');

CALL osae_sp_object_type_state_add ('NEST PROTECT','CO EMERGENCY','CO Emergency');

CALL osae_sp_object_type_state_add ('NEST PROTECT','CO Warning','CO Warning');

CALL osae_sp_object_type_state_add ('NEST PROTECT','BATTERY REPLACE','Battery Replace');

CALL osae_sp_object_type_state_add ('NEST PROTECT','ONLINE','Online');

CALL osae_sp_object_type_state_add ('NEST PROTECT','OFFLINE','Offline');


CALL osae_sp_object_type_event_add ('NEST PROTECT','SMOKE EMERGENCY','Smoke Emergency');

CALL osae_sp_object_type_event_add ('NEST PROTECT','SMOKE WARNING','Smoke Warning');

CALL osae_sp_object_type_event_add ('NEST PROTECT','CO EMERGENCY','CO Emergency');

CALL osae_sp_object_type_event_add ('NEST PROTECT','CO WARNING','CO Warning');

CALL osae_sp_object_type_event_add ('NEST PROTECT','BATTERY REPLACE','Replace Battery');

CALL osae_sp_object_type_event_add ('NEST PROTECT','ONLINE','State Set to Online');

CALL osae_sp_object_type_event_add ('NEST PROTECT','OFFLINE','State Set to Offline');


CALL osae_sp_object_type_property_add ('NEST PROTECT','ID','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Name','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Name Long','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Battery Health','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','CO Alarm State','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Smoke Alarm State','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Manual Test Active','Boolean','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Last Manual Test','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','UI Color State','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Locale','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Software Version','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Structure ID','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Last Connection','String','','',0);

CALL osae_sp_object_type_property_add ('NEST PROTECT','Is Online','Boolean','','FALSE',0);



CALL osae_sp_object_add ('Nest','','Nest','NEST','','SYSTEM',1,@out_results);

