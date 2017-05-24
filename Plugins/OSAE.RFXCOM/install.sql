CALL osae_sp_object_type_add ('RFXCOM','RFXCOM Plugin','','PLUGIN',1,1,0,1,'Plugin for RFXCOM Antenna');
CALL osae_sp_object_type_state_add ('RFXCOM','ON','Running','RFXCOM Plugin is Running');
CALL osae_sp_object_type_state_add ('RFXCOM','OFF','Stopped','RFXCOM Plugin is Stopped');
CALL osae_sp_object_type_event_add ('RFXCOM','ON','Started','RFXCOM Plugin has Started');
CALL osae_sp_object_type_event_add ('RFXCOM','OFF','Stopped','RFXCOM Plugin has Stopped');
CALL osae_sp_object_type_method_add ('RFXCOM','ON','Start','','','','','Start the RFXCOM Plugin');
CALL osae_sp_object_type_method_add ('RFXCOM','OFF','Stop','','','','','Stop the RFXCOM Plugin');
CALL osae_sp_object_type_property_add('RFXCOM','Port', 'Integer', '','', 0,1,'The COM Port number');
CALL osae_sp_object_type_property_add('RFXCOM','Learning Mode', 'Boolean', '','', 0,1,'Automatically create an object for unknown devices');
CALL osae_sp_object_type_property_add ('RFXCOM','Temp Units','String','','',0,1,'Farenheit or Celsius');
CALL osae_sp_object_type_property_option_add('RFXCOM','Temp Units','Celsius');
CALL osae_sp_object_type_property_option_add('RFXCOM','Temp Units','Farenheit ');
CALL osae_sp_object_type_property_add('RFXCOM','Trust Level','Integer','','90',0,1,'Trust Level of the Plugin');
CALL osae_sp_object_type_property_add('RFXCOM','Version','String','','',0,1,'Version of the RFXCOM Plugin');
CALL osae_sp_object_type_property_add('RFXCOM','Author','String','','',0,1,'Author of the RFXCOM Plugin');

CALL osae_sp_object_type_add ('OS TEMP SENSOR','Oregon Scientific Temperature Sensor','','',0,0,0,1,'');
CALL osae_sp_object_type_event_add ('OS TEMP SENSOR','TEMPERATURE','Temperature Changed','');
CALL osae_sp_object_type_property_add('OS TEMP SENSOR','Temperature', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('OS TEMP SENSOR','Battery', 'String', '','', 0,0,'');

CALL osae_sp_object_type_add ('OS RAIN METER','Oregon Scientific Rain Meter','','',0,0,0,1,'');
CALL osae_sp_object_type_event_add ('OS RAIN METER','RAIN RATE','Rain Rate','');
CALL osae_sp_object_type_property_add('OS RAIN METER','Rain Rate', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('OS RAIN METER','Total Rain', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('OS RAIN METER','Battery', 'String', '','', 0,0,'');

CALL osae_sp_object_type_add ('HUMIDITY METER','Humidity Meter','','',0,0,0,1,'');
CALL osae_sp_object_type_event_add ('HUMIDITY METER','HUMIDITY','Humidity Changed','');
CALL osae_sp_object_type_property_add('HUMIDITY METER','Humidity', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('HUMIDITY METER','Status', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('HUMIDITY METER','Battery', 'String', '','', 0,0,'');

CALL osae_sp_object_type_add ('TEMP HUM METER','Temperature and Humidity Meter','','',0,0,0,1,'');
CALL osae_sp_object_type_event_add ('TEMP HUM METER','HUMIDITY','Humidity Changed','');
CALL osae_sp_object_type_event_add ('TEMP HUM METER','TEMPERATURE','Temperature Changed','');
CALL osae_sp_object_type_property_add('TEMP HUM METER','Temperature', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('TEMP HUM METER','Humidity', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('TEMP HUM METER','Status', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('TEMP HUM METER','Battery', 'String', '','', 0,0,'');

CALL osae_sp_object_type_add ('TEMP HUM BARO METER','Temperature Humidity and Barometric Meter','','',0,0,0,1,'');
CALL osae_sp_object_type_event_add ('TEMP HUM BARO METER','HUMIDITY','Humidity Changed','');
CALL osae_sp_object_type_event_add ('TEMP HUM BARO METER','TEMPERATURE','Temperature Changed','');
CALL osae_sp_object_type_event_add ('TEMP HUM BARO METER','BAROMETER','Barometer Changed','');
CALL osae_sp_object_type_property_add('TEMP HUM BARO METER','Temperature', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('TEMP HUM BARO METER','Humidity', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('TEMP HUM BARO METER','Status', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('TEMP HUM BARO METER','Barometer', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('TEMP HUM BARO METER','Forcast', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('TEMP HUM BARO METER','Battery', 'String', '','', 0,0,'');

CALL osae_sp_object_type_add ('WIND SENSOR','Wind Sensor','','',0,0,0,1,'');
CALL osae_sp_object_type_event_add ('WIND SENSOR','DIRECTION','Direction Changed','');
CALL osae_sp_object_type_event_add ('WIND SENSOR','AVERAGE SPEED','Average Speed Changed','');
CALL osae_sp_object_type_event_add ('WIND SENSOR','TEMPERATURE','Temperature Changed','');
CALL osae_sp_object_type_event_add ('WIND SENSOR','WINDCHILL','Windchill Changed','');
CALL osae_sp_object_type_property_add('WIND SENSOR','Direction', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('WIND SENSOR','Average Speed', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('WIND SENSOR','Wind Gust', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('WIND SENSOR','Temperature', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('WIND SENSOR','Windchill', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('WIND SENSOR','Battery', 'String', '','', 0,0,'');

CALL osae_sp_object_type_add ('UV SENSOR','Wind Sensor','','',0,0,0,1,'');
CALL osae_sp_object_type_event_add ('UV SENSOR','TEMPERATURE','Temperature Changed','');
CALL osae_sp_object_type_event_add ('UV SENSOR','Level','Level Changed','');
CALL osae_sp_object_type_property_add('UV SENSOR','Temperature', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('UV SENSOR','Level', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('UV SENSOR','Description', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('UV SENSOR','Battery', 'String', '','', 0,0,'');

CALL osae_sp_object_type_add ('CURRENT METER','Current Meter','','',0,0,0,1,'');
CALL osae_sp_object_type_property_add('CURRENT METER','Count', 'String', '', '', 0,0,'');
CALL osae_sp_object_type_property_add('CURRENT METER','Channel 1', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('CURRENT METER','Channel 2', 'String', '','', 0,0,'');
CALL osae_sp_object_type_property_add('CURRENT METER','Channel 3', 'String', '','', 0,0,'');

CALL osae_sp_object_type_add ('SCALE','Current Meter','','',0,0,0,1,'');
CALL osae_sp_object_type_property_add('SCALE','Weight', 'String', '','', 0,0,'');

CALL osae_sp_object_type_add ('ARC BINARY SWITCH','ARC Binary Switch','','BINARY SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('ARC BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('ARC BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('ARC BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('ARC BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('ARC BINARY SWITCH','ON','On','','','','','');
CALL osae_sp_object_type_method_add ('ARC BINARY SWITCH','OFF','Off','','','','','');

CALL osae_sp_object_type_add ('ELRO BINARY SWITCH','ELRO Binary Switch','','BINARY SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('ELRO BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('ELRO BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('ELRO BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('ELRO BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('ELRO BINARY SWITCH','ON','On','','','','','');
CALL osae_sp_object_type_method_add ('ELRO BINARY SWITCH','OFF','Off','','','','','');

CALL osae_sp_object_type_add ('WAVEMAN BINARY SWITCH','WAVEMAN Binary Switch','','BINARY SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('WAVEMAN BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('WAVEMAN BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('WAVEMAN BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('WAVEMAN BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('WAVEMAN BINARY SWITCH','ON','On','','','','','');
CALL osae_sp_object_type_method_add ('WAVEMAN BINARY SWITCH','OFF','Off','','','','','');

CALL osae_sp_object_type_add ('EMW200 BINARY SWITCH','ARC Binary Switch','','BINARY SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('EMW200 BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('EMW200 BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('EMW200 BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('EMW200 BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('EMW200 BINARY SWITCH','ON','On','','','','','');
CALL osae_sp_object_type_method_add ('EMW200 BINARY SWITCH','OFF','Off','','','','','');

CALL osae_sp_object_type_add ('EMW100 BINARY SWITCH','ARC Binary Switch','','BINARY SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('EMW100 BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('EMW100 BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('EMW100 BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('EMW100 BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('EMW100 BINARY SWITCH','ON','On','','','','','');
CALL osae_sp_object_type_method_add ('EMW100 BINARY SWITCH','OFF','Off','','','','','');

CALL osae_sp_object_type_add ('RISING SUN BINARY SWITCH','RISING SUN Binary Switch','','BINARY SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('RISING SUN BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('RISING SUN BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('RISING SUN BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('RISING SUN BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('RISING SUN BINARY SWITCH','ON','On','','','','','');
CALL osae_sp_object_type_method_add ('RISING SUN BINARY SWITCH','OFF','Off','','','','','');

CALL osae_sp_object_type_add ('IMPULS BINARY SWITCH','IMPULS Binary Switch','','BINARY SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('IMPULS BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('IMPULS BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('IMPULS BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('IMPULS BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('IMPULS BINARY SWITCH','ON','On','','','','','');
CALL osae_sp_object_type_method_add ('IMPULS BINARY SWITCH','OFF','Off','','','','','');

CALL osae_sp_object_type_add ('AC DIMMER SWITCH','AC Multilevel Switch','','MULTILEVEL SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('AC DIMMER SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('AC DIMMER SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('AC DIMMER SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('AC DIMMER SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('AC DIMMER SWITCH','ON','On','Level','','100','','');
CALL osae_sp_object_type_method_add ('AC DIMMER SWITCH','OFF','Off','','','','','');
CALL osae_sp_object_type_property_add ('AC DIMMER SWITCH','Level','String','','',1,0,'');

CALL osae_sp_object_type_add ('HEU DIMMER SWITCH','HEU Multilevel Switch','','MULTILEVEL SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('HEU DIMMER SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('HEU DIMMER SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('HEU DIMMER SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('HEU DIMMER SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('HEU DIMMER SWITCH','ON','On','Level','','100','','');
CALL osae_sp_object_type_method_add ('HEU DIMMER SWITCH','OFF','Off','','','','','');
CALL osae_sp_object_type_property_add ('HEU DIMMER SWITCH','Level','String','','',1,0,'');

CALL osae_sp_object_type_add ('ANSLUT DIMMER SWITCH','ANSLUT Multilevel Switch','','MULTILEVEL SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('ANSLUT DIMMER SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('ANSLUT DIMMER SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('ANSLUT DIMMER SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('ANSLUT DIMMER SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('ANSLUT DIMMER SWITCH','ON','On','Level','','100','','');
CALL osae_sp_object_type_method_add ('ANSLUT DIMMER SWITCH','OFF','Off','','','','','');
CALL osae_sp_object_type_property_add ('ANSLUT DIMMER SWITCH','Level','String','','',1,0,'');

CALL osae_sp_object_type_add ('LIGHTWAVERF DIMMER SWITCH','LIGHTWAVERF Multilevel Switch','','MULTILEVEL SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('LIGHTWAVERF DIMMER SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('LIGHTWAVERF DIMMER SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('LIGHTWAVERF DIMMER SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('LIGHTWAVERF DIMMER SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('LIGHTWAVERF DIMMER SWITCH','ON','On','Level','','100','','');
CALL osae_sp_object_type_method_add ('LIGHTWAVERF DIMMER SWITCH','OFF','Off','','','','','');
CALL osae_sp_object_type_property_add ('LIGHTWAVERF DIMMER SWITCH','Level','String','','',1,0,'');

CALL osae_sp_object_type_add ('LIGHTWAVERF BINARY SWITCH','LIGHTWAVERF Binary Switch','','BINARY SWITCH',0,0,0,1,'');
CALL osae_sp_object_type_state_add ('LIGHTWAVERF BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_state_add ('LIGHTWAVERF BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_event_add ('LIGHTWAVERF BINARY SWITCH','ON','On','');
CALL osae_sp_object_type_event_add ('LIGHTWAVERF BINARY SWITCH','OFF','Off','');
CALL osae_sp_object_type_method_add ('LIGHTWAVERF BINARY SWITCH','ON','On','','','','','');
CALL osae_sp_object_type_method_add ('LIGHTWAVERF BINARY SWITCH','OFF','Off','','','','','');







