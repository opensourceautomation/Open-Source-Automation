--
-- Disable foreign keys
--
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

USE osae;

DELIMITER $$

--
-- Drop function "osae_fn_lookup_object_id"
--
DROP FUNCTION IF EXISTS osae_fn_lookup_object_id$$

--
-- Alter procedure "osae_sp_object_property_set"
--
DROP PROCEDURE osae_sp_object_property_set$$
CREATE PROCEDURE osae_sp_object_property_set(IN pname varchar(200), IN pproperty varchar(200), IN pvalue varchar(4000), IN pfromobject varchar(200), IN pdebuginfo varchar(2000))
BEGIN
DECLARE vDebugTrace VARCHAR(2000) DEFAULT '';
DECLARE vObjectID INT DEFAULT 0;
DECLARE vObjectCount INT DEFAULT 0;
DECLARE vObjectTypeID INT DEFAULT 0;
DECLARE vObjectTypePropertyID INT DEFAULT 0;
DECLARE vPropertyID INT DEFAULT 0;
DECLARE vPropertyValue VARCHAR(4000);
DECLARE vPropertyCount INT DEFAULT 0;
DECLARE vObjectTrustCount INT DEFAULT 0;
DECLARE vFromObjectType VARCHAR(255);
DECLARE vMinTrustLevel INT DEFAULT 0;
DECLARE vOldTrustLevel INT DEFAULT 0;
DECLARE vNewTrustLevel INT DEFAULT 50;
DECLARE vTrustLevelExists INT DEFAULT 0;
DECLARE vEventCount INT;
  #This proc runs thousands of times and must use efficient SQL, edit out the use of generic views
  SET vDebugTrace = CONCAT(pdebuginfo,' -> object_property_set');
  # 049 The following function was improved to not use the v_object view and should save likes of work
  SET vObjectCount = osae_fn_object_exists(pfromobject);
  IF vObjectCount = 1 THEN
    # 049 View replaced with efficient sql below 
    SET vPropertyCount = osae_fn_object_property_exists(pname, pproperty);
    IF vPropertyCount > 0 THEN
      # 049 Below call optomized to replace all views
      SET vTrustLevelExists = osae_fn_trust_level_property_exists(pfromobject);
      SET vObjectID = osae_fn_object_getid(pname);
      SELECT object_type_id,min_trust_level INTO vObjectTypeID,vMinTrustLevel FROM osae_object WHERE object_id=vObjectID;
      SELECT trust_level,object_type_property_id INTO vOldTrustLevel,vObjectTypePropertyID FROM osae_v_object_property WHERE object_id=vObjectID AND property_name=pproperty AND (property_value IS NULL OR property_value != pvalue);        
      SELECT property_value INTO vNewTrustLevel FROM osae_v_object_property WHERE object_name=pfromobject AND property_name='TRUST LEVEL';
      SELECT property_id,COALESCE(property_value,'') INTO vPropertyID, vPropertyValue FROM osae_v_object_property WHERE object_id=vObjectID AND property_name=pproperty AND (property_value IS NULL OR property_value != pvalue);
      #Insert Trust Level Rejection Code Here, maybe shppech command until conversation tracking is in
      IF vNewTrustLevel >= vMinTrustLevel THEN
        UPDATE osae_object_property SET property_value=pvalue,trust_level=vNewTrustLevel,source_name=pfromobject,interest_level=0 WHERE object_id=vObjectID AND object_type_property_id=vPropertyID;
        SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND event_name=pproperty;
        IF vEventCount > 0 THEN  
          CALL osae_sp_event_log_add(pname,pproperty,pfromobject,vDebugTrace,pvalue,NULL);
        END IF;
        #Since this property has changed, it has generated interest
        #Review the where clause, don't remember why it has to be null to get this update
        UPDATE osae_object_property SET interest_level = interest_level + 1 WHERE object_type_property_id = vObjectTypePropertyID AND (property_value IS NULL OR property_value = '');
      ELSE
        CALL osae_sp_debug_log_add(CONCAT(pfromobject, ' tried to set properties on ',pname,', but lacks sufficient trust'),'object_property_set'); 
      END IF;
        END IF;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_screen_object_add"
--
DROP PROCEDURE osae_sp_screen_object_add$$
CREATE PROCEDURE osae_sp_screen_object_add(IN pscreenname varchar(200),
IN pobjectname varchar(200),
IN pcontrolname varchar(200))
BEGIN
  DECLARE vScreenID int;
  DECLARE vObjectID int;
  DECLARE vControlID int;
  SELECT
    osae_fn_object_getid(pscreenname) INTO vScreenID;
  SELECT
    osae_fn_object_getid(pobjectname) INTO vObjectID;
  SELECT
    osae_fn_object_getid(pcontrolname) INTO vControlID;
  # TODO - Add Duplicate Check, working on Constraint now
  INSERT INTO osae_screen_object (screen_id, object_id, control_id)
    VALUES (vScreenID, vObjectID, vControlID);
END
$$

--
-- Alter procedure "osae_sp_screen_object_delete"
--
DROP PROCEDURE osae_sp_screen_object_delete$$
CREATE PROCEDURE osae_sp_screen_object_delete(IN pscreenname varchar(200),
IN pobjectname varchar(200),
IN pcontrolname varchar(200))
BEGIN
  DECLARE vScreenID int;
  DECLARE vObjectID int;
  DECLARE vControlID int;
  SELECT
    osae_fn_object_getid(pscreenname) INTO vScreenID;
  SELECT
    osae_fn_object_getid(pobjectname) INTO vObjectID;
  SELECT
    osae_fn_object_getid(pcontrolname) INTO vControlID;
  DELETE
    FROM osae_screen_object
  WHERE screen_id = vScreenID
    AND object_id = vObjectID
    AND control_id = vControlID;
END
$$

--
-- Alter procedure "osae_sp_screen_object_update"
--
DROP PROCEDURE osae_sp_screen_object_update$$
CREATE PROCEDURE osae_sp_screen_object_update(IN pscreenname varchar(200),
IN pobjectname varchar(200),
IN pcontrolname varchar(200))
BEGIN
  DECLARE vScreenID int;
  DECLARE vObjectID int;
  DECLARE vControlID int;
  SELECT
    osae_fn_object_getid(pscreenname) INTO vScreenID;
  SELECT
    osae_fn_object_getid(pobjectname) INTO vObjectID;
  SELECT
    osae_fn_object_getid(pcontrolname) INTO vControlID;
  UPDATE osae_screen_object
  SET object_id = vObjectID
  WHERE screen_id = vScreenID
  AND control_id = vControlID;
END
$$

--
-- Alter procedure "osae_sp_system_count_plugins"
--
DROP PROCEDURE osae_sp_system_count_plugins$$
CREATE PROCEDURE osae_sp_system_count_plugins()
BEGIN
DECLARE vPluginCount INT;
DECLARE vOldCount INT;
DECLARE iPluginCount INT;
DECLARE iPluginEnabledCount INT;
DECLARE iPluginRunningCount INT;  
DECLARE iPluginErrorCount INT;
DECLARE bDone INT; 
DECLARE vOutput VARCHAR(200);
DECLARE oCount INT;
DECLARE var1 CHAR(40);
# 049 Optomised below, making the new view, which is as light weight as possible, but maybe should be a proc, not a view.
DECLARE curs CURSOR FOR SELECT object_name FROM osae_v_system_plugins_errored;
DECLARE CONTINUE HANDLER FOR NOT FOUND SET bDone = 1;
    SET vOldCount = (SELECT property_value FROM osae_v_object_property WHERE object_name='SYSTEM' AND property_name='Plugins Errored');  
    # 049 the count should be able to be gotten from the above cursor source view, not v_object, modifyinh the view above.
    #SET iPluginErrorCount = (SELECT COUNT(object_name) FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1 AND state_name='OFF' AND container_state_name='ON');
    SET iPluginErrorCount = (SELECT COUNT(object_name) FROM osae_v_system_plugins_errored);
 
    IF vOldCount != iPluginErrorCount THEN  
        #SET iPluginCount = (SELECT COUNT(object_id) FROM osae_v_object WHERE base_type='PLUGIN');
        SET iPluginCount = osae_fn_plugin_count();
        #SET iPluginEnabledCount = (SELECT COUNT(object_id) FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1);
        SET iPluginEnabledCount = osae_fn_plugin_enabled_count();
        #SET iPluginRunningCount = (SELECT COUNT(object_id) FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1 AND state_name='ON');
        SET iPluginRunningCount = osae_fn_plugin_running_count();

        CALL osae_sp_object_property_set('SYSTEM','Plugins Found',iPluginCount,'SYSTEM','system_count_plugins');
        CALL osae_sp_object_property_set('SYSTEM','Plugins Enabled',iPluginEnabledCount,'SYSTEM','system_count_plugins');
        CALL osae_sp_object_property_set('SYSTEM','Plugins Running',iPluginRunningCount,'SYSTEM','system_count_plugins');
        CALL osae_sp_object_property_set('SYSTEM','Plugins Errored',iPluginErrorCount,'SYSTEM','system_count_plugins');

        CASE iPluginErrorCount
          WHEN 0 THEN 
            SET vOutput = 'All Plugins are Running';
            CALL osae_sp_object_property_set('SYSTEM','Plugins',vOutput,'SYSTEM','system_count_plugins');            
          WHEN 1 THEN 
          # TODO This only runs rarely, so skipping for now, but should not be using v_object
            SET vOutput = (SELECT COALESCE(object_name,'Unknown') FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1 AND state_name='OFF' LIMIT 1);
            SET vOutput = CONCAT(vOutput,' is Stopped!');
            CALL osae_sp_object_property_set('SYSTEM','Plugins',vOutput,'SYSTEM','system_count_plugins');
          ELSE
            OPEN curs;
            SET oCount = 0;
            SET bDone = 0;
            SET vOutput = '';
            REPEAT
              FETCH curs INTO var1;
              IF oCount < iPluginErrorCount THEN
                IF oCount = 0 THEN
                  SET vOutput = CONCAT(vOutput,CONCAT(' and ', var1, ' are Stopped!'));
                ELSEIF oCount = 1 THEN
                  SET vOutput = CONCAT(var1, vOutput);
                ELSE
                  SET vOutput = CONCAT(var1, ', ', vOutput);
                END IF;
                SET oCount = oCount + 1;
              END IF;
            UNTIL bDone END REPEAT;
            CLOSE curs;
            CALL osae_sp_object_property_set('SYSTEM','Plugins',vOutput,'SYSTEM','system_count_plugins');
         END CASE;
      END IF;
END
$$

--
-- Alter procedure "osae_sp_system_count_room_occupants"
--
DROP PROCEDURE osae_sp_system_count_room_occupants$$
CREATE PROCEDURE osae_sp_system_count_room_occupants()
BEGIN
  DECLARE oHouse char(255);
  DECLARE vHouseOccupantCount int;
  DECLARE vHouseOldCount int;
  DECLARE vHouseRoomCount int;
  DECLARE vHouseOldRoomCount int;
  DECLARE vTemp varchar(200);
  DECLARE vOutput,vOutput2 varchar(200);
  DECLARE bDone, bDone2 int;
  DECLARE var1 char(255);
  DECLARE var2 char(255);
  DECLARE oCount, oCount2 int;
  DECLARE sContainerName char(255);
  DECLARE iContainerOccupants int;
  DECLARE iContainerOldOccupants int;

  DECLARE cursPlaces CURSOR FOR SELECT room, occupant_count FROM osae_v_system_occupied_rooms;
  DECLARE cursOccpiedPlaces CURSOR FOR SELECT room, occupant_count FROM osae_v_system_occupied_rooms WHERE occupant_count > 0;
  DECLARE cursPeople CURSOR FOR SELECT object_name FROM osae_v_object WHERE object_type = 'PERSON' AND state_name='ON';
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET bDone = 1;

  # Handle The HOUSE level occupant count
  SET oHouse = (SELECT object_name FROM osae_v_object WHERE object_type = 'HOUSE' LIMIT 1);
  SET vHouseOldCount = (SELECT property_value FROM osae_v_object_property WHERE object_name = oHouse AND property_name = 'Occupant Count');
  SELECT SUM(occupant_count) INTO vHouseOccupantCount FROM osae_v_system_occupied_rooms;
  SET vHouseOldRoomCount = (SELECT IF(CHAR_LENGTH(property_value) > 0, property_value, 0) FROM osae_v_object_property WHERE object_name = oHouse AND property_name = 'Occupied Room Count');
  SELECT COUNT(room) INTO vHouseRoomCount FROM osae_v_system_occupied_rooms WHERE occupant_count > 0;
  IF vHouseOldCount != vHouseOccupantCount THEN
    CALL osae_sp_object_property_set(oHouse, 'Occupant Count', vHouseOccupantCount, 'SYSTEM', 'system_count_occupants');
    CASE vHouseOccupantCount
      WHEN 0 THEN
        SET vOutput = 'Nobody is here';
        CALL osae_sp_object_property_set(oHouse, 'Occupants', vOutput, 'SYSTEM', 'system_count_occupants');
        CALL osae_sp_method_queue_add(oHouse, 'OFF', '', '', 'SYSTEM', 'Auto-Occupancy Logic');
      WHEN 1 THEN
        SET vOutput = (SELECT COALESCE(object_name, 'Nobody') FROM osae_v_object WHERE object_type = 'PERSON' AND state_name = 'ON' LIMIT 1);
        SET vOutput = CONCAT(vOutput, ' is here');
        CALL osae_sp_object_property_set(oHouse, 'Occupants', vOutput, 'SYSTEM', 'system_count_occupants');
        CALL osae_sp_method_queue_add(oHouse, 'ON', '', '', 'SYSTEM', 'Auto-Occupancy Logic');
      ELSE
        OPEN cursPeople;
        SET oCount = 0;
        SET bDone = 0;
        SET vOutput = '';
        REPEAT
          FETCH cursPeople INTO var1;
          IF oCount < vHouseOccupantCount THEN
            IF oCount = 0 THEN
              SET vOutput = CONCAT(vOutput, CONCAT(' and ', var1, ' are here'));
            ELSEIF oCount = 1 THEN
              SET vOutput = CONCAT(var1, vOutput);
            ELSE
              SET vOutput = CONCAT(var1, ', ', vOutput);
            END IF;
            SET oCount = oCount + 1;
          END IF;
        UNTIL bDone END REPEAT;

        CLOSE cursPeople;
        CALL osae_sp_object_property_set(oHouse, 'Occupants', vOutput, 'SYSTEM', 'system_count_occupants');
        CALL osae_sp_method_queue_add(oHouse, 'ON', '', '', 'SYSTEM', 'Auto-Occupancy Logic');
    END CASE;
  END IF;

  IF vHouseOldRoomCount != vHouseRoomCount THEN
    CALL osae_sp_object_property_set(oHouse, 'Occupied Room Count', vHouseRoomCount, 'SYSTEM', 'system_count_occupants');
  END IF;

  CASE vHouseRoomCount
    WHEN 0 THEN
      SET vOutput = 'All rooms are vacant';
      CALL osae_sp_object_property_set(oHouse, 'Occupied Rooms', vOutput, 'SYSTEM', 'system_count_occupants');
    WHEN 1 THEN
      SET vOutput = (SELECT COALESCE(object_name, 'Unknown') FROM osae_v_object WHERE object_type = 'ROOM' AND state_name = 'ON' LIMIT 1);
      SET vOutput = CONCAT(vOutput, ' is occupied');
      CALL osae_sp_object_property_set(oHouse, 'Occupied Rooms', vOutput, 'SYSTEM', 'system_count_occupants');
    ELSE OPEN cursOccpiedPlaces;
      SET oCount = 0;
      SET bDone = 0;
      SET vOutput = '';
      REPEAT
        FETCH cursOccpiedPlaces INTO var1,var2;
        IF oCount < vHouseRoomCount THEN
          IF oCount = 0 THEN
            SET vOutput = CONCAT(vOutput, CONCAT(' and ', var1, ' are occupied'));
          ELSEIF oCount = 1 THEN
            SET vOutput = CONCAT(var1, vOutput);
          ELSE
            SET vOutput = CONCAT(var1, ', ', vOutput);
          END IF;
          SET oCount = oCount + 1;
        END IF;
      UNTIL bDone END REPEAT;
      CLOSE cursOccpiedPlaces;
      CALL osae_sp_object_property_set(oHouse, 'Occupied Rooms', vOutput, 'SYSTEM', 'system_count_occupants');
  END CASE;

  #Count the occupants in each Room and turn the objects on and off...
  OPEN cursPlaces;
  SET bDone = 0;
  REPEAT
    SET sContainerName = '';
    SET iContainerOccupants = 0;
    FETCH cursPlaces INTO sContainerName, iContainerOccupants;
    SET iContainerOldOccupants = (SELECT COALESCE(property_value, 0) FROM osae_v_object_property WHERE object_name = sContainerName AND property_name = 'Occupant Count' AND property_value != '');
    IF iContainerOccupants != iContainerOldOccupants THEN
      CALL osae_sp_debug_log_add(CONCAT('Counting Container Occupants Found a Change in ', sContainerName), 'SYSTEM');
      CALL osae_sp_object_property_set(sContainerName, 'Occupant Count', iContainerOccupants, '', '');
      # maybe add Detailed Occupants Enabled check here
      IF iContainerOccupants = 0 THEN
        CALL osae_sp_method_queue_add(sContainerName, 'OFF', '', '', 'SYSTEM', 'Auto-Occupancy Logic');
        CALL osae_sp_object_property_set(sContainerName, 'Occupants', 'Nobody', 'SYSTEM', 'system_count_container_occupants');
      ELSE
        CALL osae_sp_method_queue_add(sContainerName, 'ON', '', '', 'SYSTEM', 'Auto-Occupancy Logic');
        IF iContainerOccupants = 1 THEN
          SET vOutput = (SELECT object_name FROM osae_v_object WHERE object_type = 'PERSON' AND container_name = sContainerName LIMIT 1);
          CALL osae_sp_object_property_set(sContainerName, 'Occupants', vOutput, 'SYSTEM', 'system_count_occupants');
        ELSE
          BLOCK2: BEGIN
            DECLARE cursRoomOccupants CURSOR FOR SELECT object_name FROM osae_v_object WHERE object_type = 'PERSON' AND container_name=sContainerName;
            DECLARE CONTINUE HANDLER FOR NOT FOUND SET bDone2 = 1;
            OPEN cursRoomOccupants;
            SET oCount2 = 0;
            SET bDone2 = 0;
            SET vOutput2 = '';
            REPEAT
              FETCH cursRoomOccupants INTO var2;
              IF oCount2 < iContainerOccupants THEN
                IF oCount2 = 0 THEN
                  SET vOutput2 = CONCAT(vOutput2, CONCAT(' and ', var2, ' are here'));
                ELSEIF oCount2 = 1 THEN
                  SET vOutput2 = CONCAT(var2, vOutput2);
                ELSE
                  SET vOutput2 = CONCAT(var2, ', ', vOutput2);
                END IF;
                SET oCount2 = oCount2 + 1;
              END IF;
            UNTIL bDone2 END REPEAT;
        CLOSE cursRoomOccupants;
END BLOCK2;
          CALL osae_sp_object_property_set(sContainerName, 'Occupants', vOutput2, 'SYSTEM', 'system_count_occupants');
        END IF;
      END IF;
    END IF;
  UNTIL bDone END REPEAT;
  CLOSE cursPlaces;
END
$$

--
-- Alter procedure "osae_sp_system_process_methods"
--
DROP PROCEDURE osae_sp_system_process_methods$$
CREATE PROCEDURE osae_sp_system_process_methods()
BEGIN
DECLARE vMethodQueueID INT;
DECLARE vSystemCount INT;
DECLARE vObjectName VARCHAR(200);
DECLARE vObjectType VARCHAR(200);
DECLARE vBaseType VARCHAR(200);
DECLARE vMethod VARCHAR(400);
DECLARE vParam1 VARCHAR(400);
DECLARE vParam2 VARCHAR(400);
DECLARE vStateCount INT;
DECLARE vPropertyCount INT;
DECLARE vSendMethod VARCHAR(40);
DECLARE vContainerID INT;
DECLARE vSystemID INT;
DECLARE done INT DEFAULT 0;  
DECLARE cur1 CURSOR FOR SELECT method_queue_id,object_name,object_type,method_name,parameter_1,parameter_2 FROM osae_v_method_queue WHERE object_owner_id=vSystemID;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
  SET vSystemID = osae_fn_object_getid('SYSTEM');
    OPEN cur1; 
    Loop_Tag: LOOP
        FETCH cur1 INTO vMethodQueueID,vObjectName,vObjectType,vMethod,vParam1,vParam2;
        IF done THEN
            Leave Loop_Tag;
        END IF;
            CALL osae_sp_debug_log_add(CONCAT('SYSTEM handling ', vObjectName,' ',vMethod),'process_system_methods'); 
            DELETE FROM osae_method_queue WHERE method_queue_id = vMethodQueueID;
            #SELECT object_name,object_type,base_type INTO vObjectName,vObjectType,vBaseType FROM osae_v_object WHERE object_id=vObjectID;
            #SELECT method_name INTO vMethod FROM osae_object_type_method WHERE method_id=NEW.method_id;        
            # Here is the magic, if the Method Name matches a State, then the Method's Job is to Set that State, so look up the state
            IF UPPER(vMethod) = 'SEND MESSAGE' THEN
                SELECT UPPER(property_value) INTO vSendMethod FROM osae_v_object_property WHERE UPPER(object_name)=UPPER(vObjectName) AND UPPER(property_name)='COMMUNICATION METHOD'; 
                IF vSendMethod = 'SPEECH' THEN
                     CALL osae_sp_method_queue_add('SPEECH','SAY',vParam1,'','SYSTEM','process_system_methods');
                ELSEIF vSendMethod = 'JABBER' THEN
                     CALL osae_sp_method_queue_add('JABBER','SEND MESSAGE',vObjectName,vParam1,'SYSTEM','process_system_methods');
                END IF;
            ELSEIF UPPER(vMethod) = 'SET CONTAINER' THEN
                SELECT object_id INTO vContainerID FROM osae_object WHERE object_name=vParam1;
                UPDATE osae_object SET container_id = vContainerID,last_updated=NOW() WHERE object_name=vObjectName;
            ELSE
                SELECT count(state_name) INTO vStateCount FROM osae_v_object_type_state WHERE state_name=vMethod AND object_type=vObjectType; 
                IF vStateCount = 1 THEN   
                    CALL osae_sp_object_state_set (vObjectName,vMethod,'SYSTEM','process_system_methods');
                ELSE
                    SELECT count(property_name) INTO vPropertyCount FROM osae_v_object_type_property WHERE property_name=vMethod AND object_type=vObjectType; 
                    IF vPropertyCount = 1 THEN   
                        CALL osae_sp_object_property_set (vObjectName,vMethod,NEW.parameter_1,'SYSTEM','process_system_methods');
                    END IF;
                END IF;
            END IF;                   
        END LOOP;
    CLOSE cur1;   
END
$$

--
-- Create function "osae_fn_object_exists"
--
CREATE FUNCTION osae_fn_object_exists(pobjectname varchar(200))
  RETURNS int(11)
BEGIN
DECLARE vObjectCount INT DEFAULT 0;
  #Bad Bad, the following line was hitting the object view and only hitting the table was needed!!!!
  SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE object_name=pobjectname OR object_alias=pobjectname;
  IF vObjectCount = 0 THEN
    RETURN 0;
  ELSE
    RETURN 1;
  END IF;
END
$$

--
-- Create function "osae_fn_object_getid"
--
CREATE FUNCTION osae_fn_object_getid(pobjectname varchar(200))
  RETURNS int(11)
BEGIN
  DECLARE vObjectID int;
  SELECT object_id INTO vObjectID FROM osae_object WHERE object_name = pobjectname OR object_alias = pobjectname;
  RETURN vObjectID;
END
$$

--
-- Create function "osae_fn_object_property_exists"
--
CREATE FUNCTION osae_fn_object_property_exists(pobjectname varchar(200), ppropertyname varchar(200))
  RETURNS int(11)
BEGIN
DECLARE vObjectCount INT DEFAULT 0;

  #SELECT COUNT(object_id) INTO vObjectCount FROM osae_v_object_property WHERE (object_name=pobjectname OR object_alias=pobjectname) AND property_name=ppropertyname;
  SELECT COUNT(object_id) INTO vObjectCount FROM osae_object_property 
    INNER JOIN osae_object ON osae_object_property.object_id = osae_object.object_id
    INNER JOIN osae_object_type_property ON osae_object_property.object_type_property_id = osae_object_type_property.property_id
    WHERE (object_name=pobjectname OR object_alias=pobjectname) AND property_name=ppropertyname;
  IF vObjectCount = 0 THEN
    RETURN 0;
  ELSE
    RETURN 1;
  END IF;
END
$$

--
-- Create function "osae_fn_plugin_count"
--
CREATE FUNCTION osae_fn_plugin_count()
  RETURNS int(11)
BEGIN
DECLARE vObjectCount INT DEFAULT 0;
SELECT 
  COUNT(osae_object.object_name) INTO vObjectCount
FROM
  osae_object
Inner Join osae_object_type ON 
  osae_object.object_type_id = osae_object_type.object_type_id
Inner Join osae_object_type osae_object_base_type ON 
  osae_object_type.base_type_id = osae_object_base_type.object_type_id
WHERE
  osae_object_base_type.object_type = 'PLUGIN';

RETURN vObjectCount;
END
$$

--
-- Create function "osae_fn_plugin_enabled_count"
--
CREATE FUNCTION osae_fn_plugin_enabled_count()
  RETURNS int(11)
BEGIN
DECLARE vObjectCount INT DEFAULT 0;
SELECT 
  COUNT(osae_object.object_name) INTO vObjectCount
FROM
  osae_object
Inner Join osae_object_type ON 
  osae_object.object_type_id = osae_object_type.object_type_id
Inner Join osae_object_type osae_object_base_type ON 
  osae_object_type.base_type_id = osae_object_base_type.object_type_id
WHERE
  osae_object_base_type.object_type = 'PLUGIN'
  AND osae_object.enabled = 1;

RETURN vObjectCount;
END
$$

--
-- Create function "osae_fn_plugin_running_count"
--
CREATE FUNCTION osae_fn_plugin_running_count()
  RETURNS int(11)
BEGIN

DECLARE vObjectCount INT DEFAULT 0;
SELECT
  COUNT(osae_object.object_name) INTO vObjectCount
FROM osae_object
  INNER JOIN osae_object_type
    ON osae_object.object_type_id = osae_object_type.object_type_id
  INNER JOIN osae_object_type osae_object_base_type
    ON osae_object_type.base_type_id = osae_object_base_type.object_type_id
  INNER JOIN osae_object_type_state
    ON osae_object.state_id = osae_object_type_state.state_id
    AND osae_object.object_type_id = osae_object_type_state.object_type_id
WHERE osae_object_base_type.object_type = 'PLUGIN'
AND osae_object.enabled = 1
AND osae_object_type_state.state_name = 'ON';

RETURN vObjectCount;
END
$$

--
-- Create function "osae_fn_trust_level_property_exists"
--
CREATE FUNCTION osae_fn_trust_level_property_exists(pname      varchar(200))
  RETURNS int(11)
BEGIN
DECLARE vObjectTrustCount INT DEFAULT 0;
DECLARE vFromObjectType VARCHAR(255);

  # Verify the FromObject has a trust_level and add one if not
  # 049 ^^^^ Stupid expensive, runs too much, replaced with direct SQL
  #SELECT COUNT(object_property_id) INTO vObjectTrustCount FROM osae_v_object_property WHERE property_name='Trust Level' AND object_name=pname OR object_alias=pname;
  SELECT COUNT(object_id) INTO vObjectTrustCount FROM osae_object_property 
    INNER JOIN osae_object ON osae_object_property.object_id = osae_object.object_id
    INNER JOIN osae_object_type_property ON osae_object_property.object_type_property_id = osae_object_type_property.property_id
    WHERE (object_name=pobjectname OR object_alias=pobjectname) AND property_name='Trust Level';
  IF vObjectTrustCount = 0 THEN
    # 049 Replaced View
    #SELECT object_type INTO vFromObjectType FROM osae_v_object WHERE object_name=pfromobject OR object_alias=pfromobject;
    SELECT osae_object_type.object_type INTO vFromObjectType FROM osae_object
      INNER JOIN osae_object_type ON osae_object.object_type_id = osae_object_type.object_type_id
      WHERE object_name=pfromobject OR object_alias=pfromobject;
    CALL osae_sp_object_type_property_add(vFromObjectType,'Trust Level','Integer','','50',0);
  END IF;
RETURN 1;
END
$$

--
-- Alter trigger "osae_tr_object_after_update"
--
DROP TRIGGER IF EXISTS osae_tr_object_after_update$$
CREATE TRIGGER osae_tr_object_after_update
	AFTER UPDATE
	ON osae_object
	FOR EACH ROW
BEGIN
  DECLARE vPersonID int;
  DECLARE vBaseType varchar(200);
  DECLARE vContainerType varchar(200);
  DECLARE vContainerName varchar(100);
  DECLARE vStateName varchar(100);
  DECLARE vDetailedOccupancy VARCHAR(20);
  SELECT base_type, object_name INTO vContainerType, vContainerName FROM osae_v_object WHERE object_id = NEW.container_id;
  SELECT base_type, state_name INTO vBaseType, vStateName FROM osae_v_object WHERE object_id = OLD.object_id;
  SET vPersonID = (SELECT object_type_id FROM osae_object_type WHERE object_type = 'PERSON');
  IF NEW.object_type_id = vPersonID AND OLD.container_id <> NEW.container_id AND vContainerType = 'PLACE' THEN
    CALL osae_sp_debug_log_add('Object After Update - Update Screen Position: ', 'SYSTEM');
    #CALL osae_sp_system_count_room_occupants;
    #Update the Screen Objects to reflect the container
    CALL osae_sp_screen_object_position(OLD.object_name, vContainerName);
  END IF;

  SET vDetailedOccupancy = (SELECT property_value FROM osae_v_object_property WHERE object_name = "SYSTEM" and property_name="Detailed Occupancy Enabled");
  IF vDetailedOccupancy = "TRUE" THEN
    IF OLD.state_id <> NEW.state_id AND vBaseType = "SENSOR" AND vStateName != "OFF" THEN
      CALL osae_sp_system_who_tripped_sensor(NEW.object_name);
    END IF;
  END IF;
END
$$

DELIMITER ;

--
-- Alter view "osae_v_method_queue"
--
CREATE OR REPLACE 
VIEW osae_v_method_queue
AS
	select `osae_method_queue`.`method_queue_id` AS `method_queue_id`,`osae_method_queue`.`entry_time` AS `entry_time`,`osae_object`.`object_name` AS `object_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_method_queue`.`parameter_1` AS `parameter_1`,`osae_method_queue`.`parameter_2` AS `parameter_2`,`osae_from_object`.`object_name` AS `from_object`,`osae_method_queue`.`debug_trace` AS `debug_trace`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_description` AS `object_description`,`osae_method_queue`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_method_queue`.`from_object_id` AS `from_object_id`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_types1`.`object_type` AS `base_type`,`osae_owner_object`.`object_name` AS `object_owner`,`osae_owner_object`.`object_id` AS `object_owner_id` from ((((((`osae_object_type` left join `osae_object` `osae_owner_object` on((`osae_object_type`.`plugin_object_id` = `osae_owner_object`.`object_id`))) left join `osae_object_type` `osae_object_types1` on((`osae_object_type`.`base_type_id` = `osae_object_types1`.`object_type_id`))) join `osae_object` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) join `osae_object_type_method` on((`osae_object_type`.`object_type_id` = `osae_object_type_method`.`object_type_id`))) join `osae_method_queue` on(((`osae_object`.`object_id` = `osae_method_queue`.`object_id`) and (`osae_object_type_method`.`method_id` = `osae_method_queue`.`method_id`)))) left join `osae_object` `osae_from_object` on((`osae_from_object`.`object_id` = `osae_method_queue`.`from_object_id`)));

--
-- Create view "osae_v_object_off_timer_ready"
--
CREATE
VIEW osae_v_object_off_timer_ready
AS
SELECT
  `osae_object`.`object_name` AS `object_name`
FROM (((`osae_object`
  JOIN `osae_object_type_state`
    ON ((`osae_object`.`state_id` = `osae_object_type_state`.`state_id`)))
  JOIN `osae_object_property`
    ON ((`osae_object_property`.`object_id` = `osae_object`.`object_id`)))
  JOIN `osae_object_type_property`
    ON ((`osae_object_property`.`object_type_property_id` = `osae_object_type_property`.`property_id`)))
WHERE ((`osae_object_type_state`.`state_name` <> 'OFF')
AND (`osae_object_type_property`.`property_name` = 'OFF TIMER')
AND (`osae_object_property`.`property_value` IS NOT NULL)
AND (`osae_object_property`.`property_value` <> '')
AND (`osae_object_property`.`property_value` <> '-1')
AND (subtime(now(), sec_to_time(`osae_object_property`.`property_value`)) >= `osae_object`.`last_updated`));

--
-- Alter view "osae_v_object_type_method"
--
CREATE OR REPLACE 
VIEW osae_v_object_type_method
AS
	select `osae_object_type_1`.`object_type` AS `base_type`,`osae_object_type_method`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_object_type_method`.`object_type_id` AS `object_type_id`,coalesce(`osae_object_type_method`.`param_1_label`,'') AS `param_1_label`,coalesce(`osae_object_type_method`.`param_2_label`,'') AS `param_2_label`,coalesce(`osae_object_type_method`.`param_1_default`,'') AS `param_1_default`,coalesce(`osae_object_type_method`.`param_2_default`,'') AS `param_2_default`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden` from ((`osae_object_type` join `osae_object_type_method` on((`osae_object_type`.`object_type_id` = `osae_object_type_method`.`object_type_id`))) join `osae_object_type` `osae_object_type_1` on((`osae_object_type`.`base_type_id` = `osae_object_type_1`.`object_type_id`)));

--
-- Alter view "osae_v_system_occupied_rooms"
--
CREATE OR REPLACE 
VIEW osae_v_system_occupied_rooms
AS
	select `osae_rooms`.`object_name` AS `room`,count(`osae_occupants`.`object_name`) AS `occupant` from ((`osae_object` `osae_rooms` join `osae_object_type` `osae_room_type` on((`osae_rooms`.`object_type_id` = `osae_room_type`.`object_type_id`))) left join `osae_object` `osae_occupants` on((`osae_rooms`.`object_id` = `osae_occupants`.`container_id`))) where (`osae_room_type`.`object_type` = 'ROOM') group by `osae_rooms`.`object_name`;

--
-- Create view "osae_v_system_plugins_errored"
--
CREATE
VIEW osae_v_system_plugins_errored
AS
SELECT
  `osae_object`.`object_name` AS `object_name`
FROM (((((`osae_object`
  JOIN `osae_object_type_state` `osae_object_state`
    ON (((`osae_object`.`state_id` = `osae_object_state`.`state_id`)
    AND (`osae_object`.`object_type_id` = `osae_object_state`.`object_type_id`))))
  JOIN `osae_object_type`
    ON ((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`)))
  JOIN `osae_object_type` `osae_object_base_type`
    ON ((`osae_object_type`.`base_type_id` = `osae_object_base_type`.`object_type_id`)))
  JOIN `osae_object` `osae_container`
    ON ((`osae_object`.`container_id` = `osae_container`.`object_id`)))
  JOIN `osae_object_type_state` `osae_contianer_state`
    ON (((`osae_container`.`state_id` = `osae_contianer_state`.`state_id`)
    AND (`osae_container`.`object_type_id` = `osae_contianer_state`.`object_type_id`))))
WHERE ((`osae_object_state`.`state_name` = 'OFF')
AND (`osae_object_base_type`.`object_type` = 'PLUGIN')
AND (`osae_object`.`enabled` = 1)
AND (`osae_contianer_state`.`state_name` = 'ON'));

--
-- Alter view "osae_v_object"
--
CREATE OR REPLACE 
VIEW osae_v_object
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`last_updated` AS `last_updated`,`osae_object`.`last_state_change` AS `last_state_change`,`osae_object`.`min_trust_level` AS `min_trust_level`,`osae_object`.`enabled` AS `enabled`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`container` AS `container`,`osae_object_type_state`.`state_id` AS `state_id`,coalesce(`osae_object_type_state`.`state_name`,'') AS `state_name`,coalesce(`osae_object_type_state`.`state_label`,'') AS `state_label`,`objects_2`.`object_name` AS `owned_by`,`object_types_2`.`object_type` AS `base_type`,`objects_1`.`object_name` AS `container_name`,`osae_object`.`container_id` AS `container_id`,(select max(`osae_v_object_property`.`last_updated`) AS `expr1` from `osae_v_object_property` where ((`osae_v_object_property`.`object_id` = `osae_object`.`object_id`) and (`osae_v_object_property`.`property_name` <> 'Time'))) AS `property_last_updated`,timestampdiff(SECOND,`osae_object`.`last_state_change`,now()) AS `time_in_state`,`osae_object_type_state_1`.`state_name` AS `container_state_name`,`osae_object_type_state_1`.`state_label` AS `container_state_label` from ((((((`osae_object` left join `osae_object_type` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) left join `osae_object_type` `object_types_2` on((`osae_object_type`.`base_type_id` = `object_types_2`.`object_type_id`))) left join `osae_object` `objects_2` on((`osae_object_type`.`plugin_object_id` = `objects_2`.`object_id`))) left join `osae_object_type_state` on(((`osae_object_type`.`object_type_id` = `osae_object_type_state`.`object_type_id`) and (`osae_object_type_state`.`state_id` = `osae_object`.`state_id`)))) left join `osae_object` `objects_1` on((`objects_1`.`object_id` = `osae_object`.`container_id`))) left join `osae_object_type_state` `osae_object_type_state_1` on((`objects_1`.`state_id` = `osae_object_type_state_1`.`state_id`)));

--
-- Create view "osae_v_object_type_method_list_full"
--
CREATE
VIEW osae_v_object_type_method_list_full
AS
SELECT
  `osae_v_object_type_method`.`base_type` AS `base_type`,
  `osae_v_object_type_method`.`object_type` AS `object_type`,
  `osae_v_object_type_method`.`method_label` AS `method_label`
FROM `osae_v_object_type_method`
WHERE ((`osae_v_object_type_method`.`base_type` NOT IN ('CONTROL', 'SCREEN', 'LIST'))
AND `osae_v_object_type_method`.`object_type` IN (SELECT DISTINCT
    `osae_v_object`.`object_type`
  FROM `osae_v_object`));

DELIMITER $$

--
-- Alter event "osae_ev_off_timer"
--
ALTER EVENT osae_ev_off_timer
	DO 
# This is the most important Event in OSA.   It runs every second and has huge impact on performace and DB stability.  
# Performing full review and performace statistics for v049

BEGIN
  DECLARE iServiceRunning INT DEFAULT 0;
  DECLARE vObjectName  VARCHAR(200);
  DECLARE iLoopCount   INT DEFAULT 0;
  DECLARE iMethodCount INT DEFAULT 0;
  DECLARE iStateCount  INT DEFAULT 0;
  DECLARE done         INT DEFAULT 0;
  #This cursor is the first problem.   String indexing and date compairisons are pretty intense, must optimize this.   Maybe move to a function and run metrics on it.
  #Optomized this cursor to not use a generic view and use a custom view instead
  #DECLARE cur1 CURSOR FOR SELECT object_name FROM osae_v_object_property WHERE state_name <> 'OFF' AND property_name = 'OFF TIMER' AND property_value IS NOT NULL AND property_value <> '' AND property_value <> '-1' AND subtime(now(), sec_to_time(property_value)) > object_last_updated;
  DECLARE cur1 CURSOR FOR SELECT object_name FROM osae_v_object_off_tmer_ready;
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
  DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
  #The following was switched from v_object to v_object_state as it is a lighter view
  SET iServiceRunning = (SELECT COUNT(object_id) FROM osae_v_object_state WHERE object_name = 'SERVICE' and state_name = 'ON');
  IF iServiceRunning > 0 THEN
  CALL osae_sp_object_property_set('SYSTEM', 'Time', curtime(), 'SYSTEM', 'osae_ev_off_timer');
  CALL osae_sp_object_property_set('SYSTEM', 'Time AMPM', DATE_FORMAT(now(), '%h:%i %p'), 'SYSTEM', 'osae_ev_off_timer');
  CALL osae_sp_system_process_methods();
  CALL osae_sp_system_count_room_occupants();
  CALL osae_sp_system_count_plugins();
  #SELECT count(object_name) INTO iLoopCount FROM osae_v_object_property WHERE state_name <> 'OFF' AND property_name = 'OFF TIMER' AND property_value IS NOT NULL AND property_value <> '' AND subtime(now(), sec_to_time(property_value)) > object_last_updated;
  OPEN cur1;

Loop_Tag:
  LOOP
    FETCH cur1 INTO vObjectName;
    IF done THEN
      LEAVE Loop_Tag;
    END IF;
    SELECT count(method_id) INTO iMethodCount FROM osae_v_object_method WHERE upper(object_name) = upper(vObjectName) AND upper(method_name) = 'OFF';
    IF iMethodCount > 0 THEN
      CALL osae_sp_debug_log_add(concat('Turning ', vObjectName, ' Off'), 'osae_ev_off_timer');
      CALL osae_sp_method_queue_add(vObjectName, 'OFF', '', '', 'SYSTEM', 'osae_ev_off_timer');
    ELSE
      SELECT count(state_id) INTO iStateCount FROM osae_v_object_state WHERE upper(object_name) = upper(vObjectName) AND upper(state_name) = 'OFF';
      IF iStateCount > 0 THEN
        CALL osae_sp_debug_log_add(concat('Turning ', vObjectName, ' Off'), 'osae_ev_off_timer');
        CALL osae_sp_object_state_set(vObjectName, 'OFF', 'SYSTEM', 'osae_ev_off_timer');
      END IF;
    END IF;
  END LOOP;
  CLOSE cur1;

  SELECT count(method_id) INTO iMethodCount FROM osae_v_object_method;
  END IF;
END
$$

DELIMITER ;

--
-- Enable foreign keys
--
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;

CALL osae_sp_object_property_set('SYSTEM','DB Version','049','SYSTEM','');
CALL osae_sp_object_type_property_update('Show Slider','Show Slider','Boolean','','FALSE','CONTROL STATE IMAGE',0);
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Height','Integer','','270',0);
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Minimized','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Max Days','Integer','','5',0);
CALL osae_sp_object_type_property_add('SYSTEM','Detailed Occupancy Enabled','Boolean','','FALSE',0);
CALL osae_sp_object_type_method_update('SETTTSVOLUME','SETTTSVOLUME','Set TTS Volume','SPEECH','Volume','','100','');
CALL osae_sp_object_type_property_update('TTS Volume','TTS Volume','Integer','','100','SPEECH',0);
CALL osae_sp_object_type_property_add('GUI CLIENT','Width','Integer','','640',0);
CALL osae_sp_object_type_property_add('GUI CLIENT','Height','Integer','','480',0);
CALL osae_sp_object_type_property_add('GUI CLIENT','Show Frame','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('GUI CLIENT','Use Global Screen Settings','Boolean','','FALSE',0);

CALL osae_sp_object_type_property_add('WEB SERVER','Config Trust','Integer','','69',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Analytics Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Debug Log Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Default Screen','String','','',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Event Log Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Images Add Trust','Integer','','55',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Images Delete Trust','Integer','','60',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Images Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Logs Clear Trust','Integer','','60',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Logs Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Management Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Method Log Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Add Trust','Integer','','50',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Delete Trust','Integer','','60',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Update Trust','Integer','','55',0);
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Add Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Delete Trust','Integer','','60',0);
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Object Type Update Trust','Integer','','55',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Add Trust','Integer','','50',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Delete Trust','Integer','','60',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Trust','Integer','','50',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Update Trust','Integer','','55',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Add Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Delete Trust','Integer','','60',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Update Trust','Integer','','55',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Add Trust','Integer','','50',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Delete Trust','Integer','','60',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Trust','Integer','','50',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Update Trust','Integer','','55',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Screen Trust','Integer','','20',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Script Add Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Script Delete Trust','Integer','','60',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Script Object Add Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Script ObjectType Add Trust','Integer','','60',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Script Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Script Update Trust','Integer','','55',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Server Log Trust','Integer','','50',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Values Trust','Integer','','45',0);
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Update Trust','Integer','','50',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Hide Controls','Boolean','','FALSE',0);



