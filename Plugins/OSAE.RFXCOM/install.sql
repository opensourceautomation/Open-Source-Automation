CALL osae_sp_object_type_add ('RFXCOM','RFXCOM Plugin','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','RFXCOM');
CALL osae_sp_object_type_state_add ('OFF','Stopped','RFXCOM');
CALL osae_sp_object_type_event_add ('ON','Started','RFXCOM');
CALL osae_sp_object_type_event_add ('OFF','Stopped','RFXCOM');
CALL osae_sp_object_type_method_add ('ON','Start','RFXCOM','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','RFXCOM','','','','');
CALL osae_sp_object_type_property_add('Port', 'Integer', '', 'RFXCOM', 0);
CALL osae_sp_object_type_property_add('Learning Mode', 'Boolean', '', 'RFXCOM', 0);
CALL osae_sp_object_type_property_add ('Temp Units','String','','RFXCOM',0);
CALL osae_sp_object_type_property_option_add('RFXCOM','Temp Units','Celsius');
CALL osae_sp_object_type_property_option_add('RFXCOM','Temp Units','Farenheit ');

CALL osae_sp_object_type_add ('OS TEMP SENSOR','Oregon Scientific Temperature Sensor','','',0,0,0,1);
CALL osae_sp_object_type_event_add ('TEMPERATURE','Temperature Changed','OS TEMP SENSOR');
CALL osae_sp_object_type_property_add('Temperature', 'String', '', 'OS TEMP SENSOR', 0);
CALL osae_sp_object_type_property_add('Battery', 'String', '', 'OS TEMP SENSOR', 0);

CALL osae_sp_object_type_add ('OS RAIN METER','Oregon Scientific Rain Meter','','',0,0,0,1);
CALL osae_sp_object_type_event_add ('RAIN RATE','Rain Rate','OS RAIN METER');
CALL osae_sp_object_type_property_add('Rain Rate', 'String', '', 'OS RAIN METER', 0);
CALL osae_sp_object_type_property_add('Total Rain', 'String', '', 'OS RAIN METER', 0);
CALL osae_sp_object_type_property_add('Battery', 'String', '', 'OS RAIN METER', 0);

CALL osae_sp_object_type_add ('HUMIDITY METER','Humidity Meter','','',0,0,0,1);
CALL osae_sp_object_type_event_add ('HUMIDITY','Humidity Changed','HUMIDITY METER');
CALL osae_sp_object_type_property_add('Humidity', 'String', '', 'HUMIDITY METER', 0);
CALL osae_sp_object_type_property_add('Status', 'String', '', 'HUMIDITY METER', 0);
CALL osae_sp_object_type_property_add('Battery', 'String', '', 'HUMIDITY METER', 0);

CALL osae_sp_object_type_add ('TEMP HUM METER','Temperature and Humidity Meter','','',0,0,0,1);
CALL osae_sp_object_type_event_add ('HUMIDITY','Humidity Changed','TEMP HUM METER');
CALL osae_sp_object_type_event_add ('TEMPERATURE','Temperature Changed','TEMP HUM METER');
CALL osae_sp_object_type_property_add('Temperature', 'String', '', 'TEMP HUM METER', 0);
CALL osae_sp_object_type_property_add('Humidity', 'String', '', 'TEMP HUM METER', 0);
CALL osae_sp_object_type_property_add('Status', 'String', '', 'TEMP HUM METER', 0);
CALL osae_sp_object_type_property_add('Battery', 'String', '', 'TEMP HUM METER', 0);

CALL osae_sp_object_type_add ('TEMP HUM BARO METER','Temperature Humidity and Barometric Meter','','',0,0,0,1);
CALL osae_sp_object_type_event_add ('HUMIDITY','Humidity Changed','TEMP HUM BARO METER');
CALL osae_sp_object_type_event_add ('TEMPERATURE','Temperature Changed','TEMP HUM BARO METER');
CALL osae_sp_object_type_event_add ('BAROMETER','Barometer Changed','TEMP HUM BARO METER');
CALL osae_sp_object_type_property_add('Temperature', 'String', '', 'TEMP HUM BARO METER', 0);
CALL osae_sp_object_type_property_add('Humidity', 'String', '', 'TEMP HUM BARO METER', 0);
CALL osae_sp_object_type_property_add('Status', 'String', '', 'TEMP HUM BARO METER', 0);
CALL osae_sp_object_type_property_add('Barometer', 'String', '', 'TEMP HUM BARO METER', 0);
CALL osae_sp_object_type_property_add('Forcast', 'String', '', 'TEMP HUM BARO METER', 0);
CALL osae_sp_object_type_property_add('Battery', 'String', '', 'TEMP HUM BARO METER', 0);

CALL osae_sp_object_type_add ('WIND SENSOR','Wind Sensor','','',0,0,0,1);
CALL osae_sp_object_type_event_add ('DIRECTION','Direction Changed','WIND SENSOR');
CALL osae_sp_object_type_event_add ('AVERAGE SPEED','Average Speed Changed','WIND SENSOR');
CALL osae_sp_object_type_event_add ('TEMPERATURE','Temperature Changed','WIND SENSOR');
CALL osae_sp_object_type_event_add ('WINDCHILL','Windchill Changed','WIND SENSOR');
CALL osae_sp_object_type_property_add('Direction', 'String', '', 'WIND SENSOR', 0);
CALL osae_sp_object_type_property_add('Average Speed', 'String', '', 'WIND SENSOR', 0);
CALL osae_sp_object_type_property_add('Wind Gust', 'String', '', 'WIND SENSOR', 0);
CALL osae_sp_object_type_property_add('Temperature', 'String', '', 'WIND SENSOR', 0);
CALL osae_sp_object_type_property_add('Windchill', 'String', '', 'WIND SENSOR', 0);
CALL osae_sp_object_type_property_add('Battery', 'String', '', 'WIND SENSOR', 0);

CALL osae_sp_object_type_add ('UV SENSOR','Wind Sensor','','',0,0,0,1);
CALL osae_sp_object_type_event_add ('TEMPERATURE','Temperature Changed','UV SENSOR');
CALL osae_sp_object_type_event_add ('Level','Level Changed','UV SENSOR');
CALL osae_sp_object_type_property_add('Temperature', 'String', '', 'UV SENSOR', 0);
CALL osae_sp_object_type_property_add('Level', 'String', '', 'UV SENSOR', 0);
CALL osae_sp_object_type_property_add('Description', 'String', '', 'UV SENSOR', 0);
CALL osae_sp_object_type_property_add('Battery', 'String', '', 'UV SENSOR', 0);

CALL osae_sp_object_type_add ('CURRENT METER','Current Meter','','',0,0,0,1);
CALL osae_sp_object_type_property_add('Count', 'String', '', 'CURRENT METER', 0);
CALL osae_sp_object_type_property_add('Channel 1', 'String', '', 'CURRENT METER', 0);
CALL osae_sp_object_type_property_add('Channel 2', 'String', '', 'CURRENT METER', 0);
CALL osae_sp_object_type_property_add('Channel 3', 'String', '', 'CURRENT METER', 0);

CALL osae_sp_object_type_add ('SCALE','Current Meter','','',0,0,0,1);
CALL osae_sp_object_type_property_add('Weight', 'String', '', 'SCALE', 0);

CALL osae_sp_object_type_add ('ARC BINARY SWITCH','ARC Binary Switch','','BINARY SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','ARC BINARY SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','ARC BINARY SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','ARC BINARY SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','ARC BINARY SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','ARC BINARY SWITCH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','ARC BINARY SWITCH','','','','');

CALL osae_sp_object_type_add ('ELRO BINARY SWITCH','ELRO Binary Switch','','BINARY SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','ELRO BINARY SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','ELRO BINARY SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','ELRO BINARY SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','ELRO BINARY SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','ELRO BINARY SWITCH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','ELRO BINARY SWITCH','','','','');

CALL osae_sp_object_type_add ('WAVEMAN BINARY SWITCH','WAVEMAN Binary Switch','','BINARY SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','WAVEMAN BINARY SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','WAVEMAN BINARY SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','WAVEMAN BINARY SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','WAVEMAN BINARY SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','WAVEMAN BINARY SWITCH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','WAVEMAN BINARY SWITCH','','','','');

CALL osae_sp_object_type_add ('EMW200 BINARY SWITCH','ARC Binary Switch','','BINARY SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','EMW200 BINARY SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','EMW200 BINARY SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','EMW200 BINARY SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','EMW200 BINARY SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','EMW200 BINARY SWITCH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','EMW200 BINARY SWITCH','','','','');

CALL osae_sp_object_type_add ('EMW100 BINARY SWITCH','ARC Binary Switch','','BINARY SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','EMW100 BINARY SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','EMW100 BINARY SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','EMW100 BINARY SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','EMW100 BINARY SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','EMW100 BINARY SWITCH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','EMW100 BINARY SWITCH','','','','');

CALL osae_sp_object_type_add ('RISING SUN BINARY SWITCH','RISING SUN Binary Switch','','BINARY SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','RISING SUN BINARY SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','RISING SUN BINARY SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','RISING SUN BINARY SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','RISING SUN BINARY SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','RISING SUN BINARY SWITCH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','RISING SUN BINARY SWITCH','','','','');

CALL osae_sp_object_type_add ('IMPULS BINARY SWITCH','IMPULS Binary Switch','','BINARY SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','IMPULS BINARY SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','IMPULS BINARY SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','IMPULS BINARY SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','IMPULS BINARY SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','IMPULS BINARY SWITCH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','IMPULS BINARY SWITCH','','','','');

CALL osae_sp_object_type_add ('AC DIMMER SWITCH','AC Multilevel Switch','','MULTILEVEL SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','AC DIMMER SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','AC DIMMER SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','AC DIMMER SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','AC DIMMER SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','AC DIMMER SWITCH','Level','','100','');
CALL osae_sp_object_type_method_add ('OFF','Off','AC DIMMER SWITCH','','','','');
CALL osae_sp_object_type_property_add ('Level','String','','AC DIMMER SWITCH',1);

CALL osae_sp_object_type_add ('HEU DIMMER SWITCH','HEU Multilevel Switch','','MULTILEVEL SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','HEU DIMMER SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','HEU DIMMER SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','HEU DIMMER SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','HEU DIMMER SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','HEU DIMMER SWITCH','Level','','100','');
CALL osae_sp_object_type_method_add ('OFF','Off','HEU DIMMER SWITCH','','','','');
CALL osae_sp_object_type_property_add ('Level','String','','HEU DIMMER SWITCH',1);

CALL osae_sp_object_type_add ('ANSLUT DIMMER SWITCH','ANSLUT Multilevel Switch','','MULTILEVEL SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','ANSLUT DIMMER SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','ANSLUT DIMMER SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','ANSLUT DIMMER SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','ANSLUT DIMMER SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','ANSLUT DIMMER SWITCH','Level','','100','');
CALL osae_sp_object_type_method_add ('OFF','Off','ANSLUT DIMMER SWITCH','','','','');
CALL osae_sp_object_type_property_add ('Level','String','','ANSLUT DIMMER SWITCH',1);

CALL osae_sp_object_type_add ('LIGHTWAVERF DIMMER SWITCH','LIGHTWAVERF Multilevel Switch','','MULTILEVEL SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','LIGHTWAVERF DIMMER SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','LIGHTWAVERF DIMMER SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','LIGHTWAVERF DIMMER SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','LIGHTWAVERF DIMMER SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','LIGHTWAVERF DIMMER SWITCH','Level','','100','');
CALL osae_sp_object_type_method_add ('OFF','Off','LIGHTWAVERF DIMMER SWITCH','','','','');
CALL osae_sp_object_type_property_add ('Level','String','','LIGHTWAVERF DIMMER SWITCH',1);

CALL osae_sp_object_type_add ('LIGHTWAVERF BINARY SWITCH','LIGHTWAVERF Binary Switch','','BINARY SWITCH',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','On','LIGHTWAVERF BINARY SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','LIGHTWAVERF BINARY SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','LIGHTWAVERF BINARY SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','LIGHTWAVERF BINARY SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','LIGHTWAVERF BINARY SWITCH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','LIGHTWAVERF BINARY SWITCH','','','','');
