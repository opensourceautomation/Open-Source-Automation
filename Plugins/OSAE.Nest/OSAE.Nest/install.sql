
CALL osae_sp_object_type_add ('NEST','Nest Plugin','','PLUGIN',1,1,0,1);

CALL osae_sp_object_type_state_add ('ON','Running','NEST');
CALL osae_sp_object_type_state_add ('OFF','Stopped','NEST');

CALL osae_sp_object_type_event_add ('ON','Started','NEST');
CALL osae_sp_object_type_event_add ('OFF','Stopped','NEST');

CALL osae_sp_object_type_method_add ('GET ACCESS TOKEN','Get an Access Token','NEST','','','','');
CALL osae_sp_object_type_method_add ('SEARCH FOR NEW DEVICES','Search For New Devices','NEST','','','','');
CALL osae_sp_object_type_method_add ('RESTART STREAMING UPDATES','Restart the streaming updates from Nest','NEST','','','','');

CALL osae_sp_object_type_property_add ('Pin','String','','NEST',0);
CALL osae_sp_object_type_property_add ('Access Token','String','','NEST',0);
CALL osae_sp_object_type_property_add ('Client Version','Integer','','NEST',0);


CALL osae_sp_object_type_add ('NEST STRUCTURE','Nest Structure','NEST','NEST STRUCTURE',0,0,0,1);

CALL osae_sp_object_type_state_add ('AWAY','Away','NEST STRUCTURE');
CALL osae_sp_object_type_state_add ('HOME','Home','NEST STRUCTURE');
CALL osae_sp_object_type_state_add ('AUTO-AWAY','Auto-Away','NEST STRUCTURE');

CALL osae_sp_object_type_event_add ('AWAY','State Set to Away','NEST STRUCTURE');
CALL osae_sp_object_type_event_add ('HOME','State Set to Home','NEST STRUCTURE');
CALL osae_sp_object_type_event_add ('AUTO-AWAY','State Set to Auto-Away','NEST STRUCTURE');

CALL osae_sp_object_type_method_add ('AWAY','Set State to Away','NEST STRUCTURE','','','','');
CALL osae_sp_object_type_method_add ('HOME','Set State to Home','NEST STRUCTURE','','','','');
CALL osae_sp_object_type_method_add ('SET ETA','Set ETA','NEST STRUCTURE','Trip Name','Expected Time','','');
CALL osae_sp_object_type_method_add ('CANCEL ETA','Cancel ETA','NEST STRUCTURE','Trip Name','','','');

CALL osae_sp_object_type_property_add ('Id','String','','NEST STRUCTURE',0);
CALL osae_sp_object_type_property_add ('Name','String','','NEST STRUCTURE',0);
CALL osae_sp_object_type_property_add ('Country Code','String','','NEST STRUCTURE',0);
CALL osae_sp_object_type_property_add ('Postal Code','String','','NEST STRUCTURE',0);
CALL osae_sp_object_type_property_add ('Peak Start Time','String','','NEST STRUCTURE',0);
CALL osae_sp_object_type_property_add ('Peak End Time','String','','NEST STRUCTURE',0);
CALL osae_sp_object_type_property_add ('Time Zone','String','','NEST STRUCTURE',0);



CALL osae_sp_object_type_add ('NEST THERMOSTAT','Nest Thermostat','NEST','NEST THERMOSTAT',0,0,0,1);

CALL osae_sp_object_type_state_add ('HEAT','Heat','NEST THERMOSTAT');
CALL osae_sp_object_type_state_add ('COOL','Cool','NEST THERMOSTAT');
CALL osae_sp_object_type_state_add ('HEAT-COOL','Heat-Cool','NEST THERMOSTAT');
CALL osae_sp_object_type_state_add ('OFF','Off','NEST THERMOSTAT');
CALL osae_sp_object_type_state_add ('OFFLINE','Offline','NEST THERMOSTAT');

CALL osae_sp_object_type_event_add ('HEAT','State Set to Heat','NEST THERMOSTAT');
CALL osae_sp_object_type_event_add ('COOL','State Set to Cool','NEST THERMOSTAT');
CALL osae_sp_object_type_event_add ('HEAT-COOL','State Set to Heat-Cool','NEST THERMOSTAT');
CALL osae_sp_object_type_event_add ('OFF','State Set to Off','NEST THERMOSTAT');
CALL osae_sp_object_type_event_add ('OFFLINE','State Set to Offline','NEST THERMOSTAT');

CALL osae_sp_object_type_method_add ('HEAT','Set State to Heat','NEST THERMOSTAT','','','','');
CALL osae_sp_object_type_method_add ('COOL','Set State to Cool','NEST THERMOSTAT','','','','');
CALL osae_sp_object_type_method_add ('OFF','Set State to Off','NEST THERMOSTAT','','','','');
CALL osae_sp_object_type_method_add ('HEAT-COOL','Set State to Heat-Cool','NEST THERMOSTAT','','','','');

CALL osae_sp_object_type_method_add ('SET TARGET','Set Target Temperature','NEST THERMOSTAT','Temperature','','','');
CALL osae_sp_object_type_method_add ('SET TARGET HIGH','Set Target High Temperature','NEST THERMOSTAT','Temperature','','','');
CALL osae_sp_object_type_method_add ('SET TARGET LOW','Set Target Low Temperature','NEST THERMOSTAT','Temperature','','','');
CALL osae_sp_object_type_method_add ('TURN ON FAN','Turn On Fan','NEST THERMOSTAT','','','','');
CALL osae_sp_object_type_method_add ('TURN OFF FAN','Turn Off Fan','NEST THERMOSTAT','','','','');

CALL osae_sp_object_type_property_add ('ID','String','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Name','String','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Name Long','String','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Ambient Temperature','Float','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Humidity','Integer','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Target Temperature','Float','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Target Temperature High','Float','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Target Temperature Low','Float','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Away Temperature High','Float','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Away Temperature Low','Float','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Has Leaf','Boolean','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Temperature Scale','String','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Locale','String','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Software Version','String','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Structure ID','String','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Last Connection','String','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Is Online','Boolean','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Can Cool','Boolean','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Can Heat','Boolean','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Using Emergency Heat','Boolean','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Has Fan','Boolean','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Fan Timer Active','Boolean','','NEST THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Fan Timer Timeout','String','','NEST THERMOSTAT',0);



CALL osae_sp_object_type_add ('NEST PROTECT','Nest Protect','NEST','NEST PROTECT',0,0,0,1);

CALL osae_sp_object_type_state_add ('SMOKE EMERGENCY','Smoke Emergency','NEST PROTECT');
CALL osae_sp_object_type_state_add ('SMOKE WARNING','Smoke Warning','NEST PROTECT');
CALL osae_sp_object_type_state_add ('CO EMERGENCY','CO Emergency','NEST PROTECT');
CALL osae_sp_object_type_state_add ('CO Warning','CO Warning','NEST PROTECT');
CALL osae_sp_object_type_state_add ('BATTERY REPLACE','Battery Replace','NEST PROTECT');
CALL osae_sp_object_type_state_add ('ONLINE','Online','NEST PROTECT');
CALL osae_sp_object_type_state_add ('OFFLINE','Offline','NEST PROTECT');

CALL osae_sp_object_type_event_add ('SMOKE EMERGENCY','Smoke Emergency','NEST PROTECT');
CALL osae_sp_object_type_event_add ('SMOKE WARNING','Smoke Warning','NEST PROTECT');
CALL osae_sp_object_type_event_add ('CO EMERGENCY','CO Emergency','NEST PROTECT');
CALL osae_sp_object_type_event_add ('CO WARNING','CO Warning','NEST PROTECT');
CALL osae_sp_object_type_event_add ('BATTERY REPLACE','Replace Battery','NEST PROTECT');
CALL osae_sp_object_type_event_add ('ONLINE','State Set to Online','NEST PROTECT');
CALL osae_sp_object_type_event_add ('OFFLINE','State Set to Offline','NEST PROTECT');

CALL osae_sp_object_type_property_add ('ID','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Name','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Name Long','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Battery Health','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('CO Alarm State','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Smoke Alarm State','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Manual Test Active','Boolean','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Last Manual Test','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('UI Color State','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Locale','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Software Version','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Structure ID','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Last Connection','String','','NEST PROTECT',0);
CALL osae_sp_object_type_property_add ('Is Online','Boolean','','NEST PROTECT',0);

CALL osae_sp_object_add ('Nest','Nest','NEST','','SYSTEM',1,@out_results);

