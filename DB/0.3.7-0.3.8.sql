


CALL osae_sp_object_type_add ('BINARY SWITCH','Binary Switch','','BINARY SWITCH',0,1,0,1);
CALL osae_sp_object_type_state_add ('ON','On','BINARY SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','BINARY SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','BINARY SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','BINARY SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','BINARY SWITCH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','BINARY SWITCH','','','','');

CALL osae_sp_object_type_add ('MULTILEVEL SWITCH','Multilevel Switch','','MULTILEVEL SWITCH',0,1,0,1);
CALL osae_sp_object_type_state_add ('ON','On','MULTILEVEL SWITCH');
CALL osae_sp_object_type_state_add ('OFF','Off','MULTILEVEL SWITCH');
CALL osae_sp_object_type_event_add ('ON','On','MULTILEVEL SWITCH');
CALL osae_sp_object_type_event_add ('OFF','Off','MULTILEVEL SWITCH');
CALL osae_sp_object_type_method_add ('ON','On','MULTILEVEL SWITCH','Level','100','','');
CALL osae_sp_object_type_method_add ('OFF','Off','MULTILEVEL SWITCH','','','','');
CALL osae_sp_object_type_property_add ('Level','String','','MULTILEVEL SWITCH',1);

CALL osae_sp_object_type_add ('THERMOSTAT','Thermostat','','THERMOSTAT',0,1,0,1);
CALL osae_sp_object_type_state_add ('HEAT ON','Heat On','THERMOSTAT');
CALL osae_sp_object_type_state_add ('OFF','Off','THERMOSTAT');
CALL osae_sp_object_type_state_add ('COOL ON','Cool On','THERMOSTAT');
CALL osae_sp_object_type_event_add ('COOL ON','Cool On','THERMOSTAT');
CALL osae_sp_object_type_event_add ('HEAT ON','Heat On','THERMOSTAT');
CALL osae_sp_object_type_event_add ('FAN ON','Fan On','THERMOSTAT');
CALL osae_sp_object_type_event_add ('FAN OFF','Fan Off','THERMOSTAT');
CALL osae_sp_object_type_event_add ('OFF','Off','THERMOSTAT');
CALL osae_sp_object_type_event_add ('TEMPERATURE','Tempurature','THERMOSTAT');
CALL osae_sp_object_type_event_add ('HEATSP','Heat Setpoint','THERMOSTAT');
CALL osae_sp_object_type_event_add ('COOLSP','Cool Setpoint','THERMOSTAT');
CALL osae_sp_object_type_method_add ('HEAT','Heat','THERMOSTAT','','','','');
CALL osae_sp_object_type_method_add ('AUTO','Auto','THERMOSTAT','','','','');
CALL osae_sp_object_type_method_add ('COOL','Cool','THERMOSTAT','','','','');
CALL osae_sp_object_type_method_add ('FAN AUTO','Fan Auto','THERMOSTAT','','','','');
CALL osae_sp_object_type_method_add ('FAN ON','Fan On','THERMOSTAT','','','','');
CALL osae_sp_object_type_method_add ('HEATSP','Heat Setpoint','THERMOSTAT','Setpoint','','','');
CALL osae_sp_object_type_method_add ('COOLSP','Cool Setpoint','THERMOSTAT','Setpoint','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','THERMOSTAT','','','','');
CALL osae_sp_object_type_property_add ('Heat Setpoint','Integer','','THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Coolt Setpoint','Integer','','THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Current Temperature','Integer','','THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Operating Mode','String','','THERMOSTAT',0);
CALL osae_sp_object_type_property_add ('Fan Mode','String','','THERMOSTAT',0);

CALL osae_sp_object_type_update ('X10 RELAY','X10 RELAY','X10 Relay','','BINARY SWITCH',1,1,0,0);
CALL osae_sp_object_type_update ('X10 DIMMER','X10 DIMMER','X10 Dimmer','','MULTILEVEL SWITCH',1,1,0,0);

DELIMITER $$

DROP TRIGGER IF EXISTS osae_tr_object_before_insert$$
CREATE 
        DEFINER = 'osae'@'%'
TRIGGER osae.osae_tr_object_before_insert
        BEFORE INSERT
        ON osae.osae_object
        FOR EACH ROW
BEGIN
DECLARE iState INT;
    IF ISNULL(NEW.state_id) THEN
        SELECT state_id INTO iState FROM osae_object_type_state WHERE object_type_id=NEW.object_type_id AND state_name="OFF";
        IF ISNULL(iState) THEN
            SELECT state_id INTO iState FROM osae_object_type_state WHERE object_type_id=NEW.object_type_id LIMIT 1;
         END IF;
        SET NEW.state_id=iState;
    END IF;
END

$$

CREATE OR REPLACE DEFINER = 'osae'@'%' VIEW osae.osae_v_object_type_method
AS
SELECT `osae_object_type_method`.`method_id` AS `method_id`
     , `osae_object_type_method`.`method_name` AS `method_name`
     , `osae_object_type_method`.`method_label` AS `method_label`
     , `osae_object_type_method`.`object_type_id` AS `object_type_id`
     , coalesce(`osae_object_type_method`.`param_1_label`, '') AS `param_1_label`
     , coalesce(`osae_object_type_method`.`param_2_label`, '') AS `param_2_label`
     , coalesce(`osae_object_type_method`.`param_1_default`, '') AS `param_1_default`
     , coalesce(`osae_object_type_method`.`param_2_default`, '') AS `param_2_default`
     , `osae_object_type`.`object_type` AS `object_type`
     , `osae_object_type`.`object_type_description` AS `object_type_description`
     , `osae_object_type`.`plugin_object_id` AS `plugin_object_id`
     , `osae_object_type`.`system_hidden` AS `system_hidden`
FROM
  (`osae_object_type`
JOIN `osae_object_type_method`
ON ((`osae_object_type`.`object_type_id` = `osae_object_type_method`.`object_type_id`)));

$$

DROP VIEW IF EXISTS osae_v_object_property_history CASCADE$$
CREATE OR REPLACE DEFINER = 'osae'@'%' VIEW osae_v_object_property_history
AS
SELECT `osae_object_property_history`.`history_id` AS `history_id`
     , `osae_object_property_history`.`history_timestamp` AS `history_timestamp`
     , `osae_object_property_history`.`property_value` AS `property_value`
     , `osae_object`.`object_name` AS `object_name`
     , `osae_object_property_history`.`object_property_id` AS `object_property_id`
     , `osae_object_type_property`.`property_name` AS `property_name`
     , `osae_object_type_property`.`property_datatype` AS `property_datatype`
     , `osae_object_type`.`object_type` AS `object_type`
     , `osae_object_type`.`object_type_description` AS `object_type_description`
     , `osae_object`.`object_description` AS `object_description`
FROM
  ((((`osae_object_property_history`
JOIN `osae_object_property`
ON ((`osae_object_property_history`.`object_property_id` = `osae_object_property`.`object_property_id`)))
JOIN `osae_object`
ON ((`osae_object_property`.`object_id` = `osae_object`.`object_id`)))
JOIN `osae_object_type_property`
ON ((`osae_object_property`.`object_type_property_id` = `osae_object_type_property`.`property_id`)))
JOIN `osae_object_type`
ON (((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`) AND (`osae_object_type_property`.`object_type_id` = `osae_object_type`.`object_type_id`))));

$$

DROP EVENT IF EXISTS osae_ev_off_timer$$
CREATE
DEFINER = 'osae'@'%'
EVENT osae_ev_off_timer
ON SCHEDULE EVERY '1' SECOND
STARTS '2010-05-23 10:09:24'
DO
BEGIN
  DECLARE vObjectName  VARCHAR(200);
  DECLARE iLoopCount   INT DEFAULT 0;
  DECLARE iMethodCount INT DEFAULT 0;
  DECLARE iStateCount  INT DEFAULT 0;
  DECLARE done         INT DEFAULT 0;
  DECLARE cur1 CURSOR FOR SELECT object_name FROM osae_v_object_property WHERE state_name <> 'OFF' AND property_name = 'OFF TIMER' AND property_value IS NOT NULL AND property_value <> '' AND subtime(now(), sec_to_time(property_value)) > object_last_updated;
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
  DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
  
  CALL osae_sp_object_property_set('SYSTEM', 'Time', curtime(), 'SYSTEM', 'osae_ev_off_timer');
  CALL osae_sp_object_property_set('SYSTEM', 'Time AMPM', DATE_FORMAT(now(), '%h:%i %p'), 'SYSTEM', 'osae_ev_off_timer');
  CALL osae_sp_system_count_occupants();
  SELECT count(object_name)
  INTO
    iLoopCount
  FROM
    osae_v_object_property
  WHERE
    state_name <> 'OFF'
    AND property_name = 'OFF TIMER'
    AND property_value IS NOT NULL
    AND property_value <> ''
    AND subtime(now(), sec_to_time(property_value)) > object_last_updated;
  OPEN cur1;

Loop_Tag:
  LOOP
    FETCH cur1 INTO vObjectName;
    IF done THEN
      LEAVE Loop_Tag;
    END IF;
    SELECT count(method_id)
    INTO
      iMethodCount
    FROM
      osae_v_object_method
    WHERE
      upper(object_name) = upper(vObjectName)
      AND upper(method_name) = 'OFF';
    IF iMethodCount > 0 THEN
      CALL osae_sp_debug_log_add(concat('Turning ', vObjectName, ' Off'), 'osae_ev_off_timer');
      CALL osae_sp_method_queue_add(vObjectName, 'OFF', '', '', 'SYSTEM', 'osae_ev_off_timer');
    ELSE
      SELECT count(state_id)
      INTO
        iStateCount
      FROM
        osae_v_object_state
      WHERE
        upper(object_name) = upper(vObjectName)
        AND upper(state_name) = 'OFF';
      IF iStateCount > 0 THEN
        CALL osae_sp_debug_log_add(concat('Turning ', vObjectName, ' Off'), 'osae_ev_off_timer');
        CALL osae_sp_object_state_set(vObjectName, 'OFF', 'SYSTEM', 'osae_ev_off_timer');
      END IF;
    END IF;
  END LOOP;
  CLOSE cur1;

  SELECT count(method_id) INTO iMethodCount FROM osae_v_object_method;
END
$$
DELIMITER ;   

-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.3.8', '', '');