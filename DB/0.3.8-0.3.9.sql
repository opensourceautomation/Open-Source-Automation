
DELIMITER $$

DROP PROCEDURE IF EXISTS osae_sp_pattern_parse$$
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_parse(
  IN ppattern varchar(2000)
)
BEGIN
  DECLARE vInput VARCHAR(2000) DEFAULT '';
  DECLARE vOutput VARCHAR(2000) DEFAULT '';  
  DECLARE vOld VARCHAR(200);  
  DECLARE vWorking VARCHAR(200); 
  DECLARE vDot INT DEFAULT 0;
  DECLARE vSpace1 INT DEFAULT 0;
  DECLARE vSpace2 INT DEFAULT 0;  
  DECLARE vObject VARCHAR(200);
  DECLARE vParam VARCHAR(255);  
  DECLARE vTemp VARCHAR(255);  
    SET vInput = ppattern; 
    SELECT INSTR(vInput,'[') INTO vSpace1;
    SELECT INSTR(vInput,']') INTO vSpace2;
        
    WHILE vSpace2 > vSpace1 DO 
      SELECT MID(vInput,vSpace1,vSpace2 - vSpace1 + 1) INTO vOld; 
      SELECT MID(vInput,vSpace1+1,vSpace2 - vSpace1 - 1) INTO vWorking; 
      #SELECT vOld, vWorking;     
      SELECT INSTR(vWorking,'.') INTO vDot;
      IF vDOT > 0 THEN
        SET vObject = LEFT(vWorking,vDot - 1);
        SET vParam = RIGHT(vWorking,LENGTH(vWorking) - vDot);
        IF vParam = 'State' THEN
          SELECT state_name INTO vTemp FROM osae_v_object WHERE object_name=vObject;        
          SET vInput = REPLACE(vInput,vOld,vTemp);
        ELSE
          SELECT property_value INTO vTemp FROM osae_v_object_property WHERE object_name=vObject AND property_name=vParam;
          SET vInput = REPLACE(vInput,vOld,vTemp);          
        END IF;      
      END IF;
      SELECT INSTR(vInput,'[') INTO vSpace1;
      SELECT INSTR(vInput,']') INTO vSpace2;
    END WHILE;
    SELECT vInput;
END
$$

DELIMITER ;


DROP TABLE IF EXISTS osae_event_log;
CREATE TABLE IF NOT EXISTS osae_event_log(
  event_log_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  object_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  event_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  parameter_1 VARCHAR(2000) DEFAULT NULL,
  parameter_2 VARCHAR(2000) DEFAULT NULL,
  log_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  from_object_id INT(10) UNSIGNED DEFAULT NULL,
  debug_trace VARCHAR(2000) DEFAULT NULL,
  PRIMARY KEY (event_log_id),
  CONSTRAINT osae_fk_events_log_to_events FOREIGN KEY (event_id)
  REFERENCES osae_object_type_event (event_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_events_log_to_objects FOREIGN KEY (object_id)
  REFERENCES osae_object (object_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 1
CHARACTER SET utf8
COLLATE utf8_general_ci;


DELIMITER $$

DROP PROCEDURE IF EXISTS osae_sp_event_log_add$$

CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_event_log_add(IN pobject VARCHAR(200), IN pevent VARCHAR(200), IN pfromobject VARCHAR(200), IN pdebuginfo VARCHAR(1000), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000))
BEGIN
DECLARE vObjectCount INT;
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vEventCount INT;
DECLARE vEventID INT;
DECLARE vFromObjectID INT DEFAULT NULL;
DECLARE vFromObjectCount INT;
DECLARE vDebugTrace VARCHAR(2000);
    SET vDebugTrace = CONCAT(COALESCE(pdebuginfo,''),' -> osae_sp_event_log_add');
    SELECT object_id INTO vFromObjectID FROM osae_object WHERE UPPER(object_name)=UPPER(pfromobject);
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(object_name)=UPPER(pobject);
    IF vObjectCount > 0 THEN
        SELECT object_id,object_type_id INTO vObjectID,vObjectTypeID FROM osae_object WHERE UPPER(object_name)=UPPER(pobject);
        SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pevent);
        IF vEventCount = 1 THEN  
            #CALL osae_sp_debug_log_add(CONCAT('Event_Log_add (',pobject,' ',pevent,') ',pfromobject,' vFromObjectID ',COALESCE(vFromObjectID,'NULL')),vDebugTrace);
            SELECT event_id INTO vEventID FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pevent);
            INSERT INTO osae_event_log (object_id,event_id,from_object_id,debug_trace,parameter_1,parameter_2) VALUES(vObjectID,vEventID,vFromObjectID,vDebugTrace,pparameter1,pparameter2);
        END IF;
    END IF; 
END
$$

DELIMITER ;



CREATE OR REPLACE DEFINER = 'osae'@'%' VIEW osae_v_event_log
AS
SELECT `osae_event_log`.`event_log_id` AS `event_log_id`
     , `osae_event_log`.`parameter_1` AS `parameter_1`
     , `osae_event_log`.`parameter_2` AS `parameter_2`
     , `osae_event_log`.`from_object_id` AS `from_object_id`
     , `osae_event_log`.`debug_trace` AS `debug_trace`
     , `osae_event_log`.`log_time` AS `log_time`
     , `osae_object`.`object_id` AS `object_id`
     , `osae_object`.`object_name` AS `object_name`
     , `osae_object`.`object_description` AS `object_description`
     , `osae_object`.`state_id` AS `state_id`
     , `osae_object`.`object_value` AS `object_value`
     , `osae_object`.`address` AS `address`
     , `osae_object`.`container_id` AS `container_id`
     , `osae_object`.`enabled` AS `enabled`
     , `osae_object_type_event`.`event_id` AS `event_id`
     , `osae_object_type_event`.`event_name` AS `event_name`
     , `osae_object_type_event`.`event_label` AS `event_label`
     , `osae_object_type`.`object_type_id` AS `object_type_id`
     , `osae_object_type`.`object_type` AS `object_type`
     , `osae_object_type`.`object_type_description` AS `object_type_description`
     , `osae_object_type`.`plugin_object_id` AS `plugin_object_id`
     , `osae_object_type`.`system_hidden` AS `system_hidden`
     , `osae_object_type`.`object_type_owner` AS `object_type_owner`
     , `osae_object_type`.`base_type_id` AS `base_type_id`
     , `osae_object_type`.`container` AS `container`
     , `osae_object1`.`object_name` AS `from_object_name`
FROM
  ((((`osae_object`
JOIN `osae_event_log`
ON ((`osae_object`.`object_id` = `osae_event_log`.`object_id`)))
JOIN `osae_object_type_event`
ON ((`osae_event_log`.`event_id` = `osae_object_type_event`.`event_id`)))
JOIN `osae_object_type`
ON ((`osae_object_type_event`.`object_type_id` = `osae_object_type`.`object_type_id`)))
LEFT JOIN `osae_object` `osae_object1`
ON ((`osae_object1`.`object_id` = `osae_event_log`.`from_object_id`)));




DELIMITER $$
DROP PROCEDURE IF EXISTS osae_sp_object_state_set$$

CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_state_set(IN pname varchar(200), IN pstate varchar(50), IN pfromobject varchar(200), IN pdebuginfo varchar(2000))
BEGIN
DECLARE vObjectCount INT;
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vStateCount INT;
DECLARE vOldStateID INT;
DECLARE vStateID INT;
DECLARE vEventCount INT;
DECLARE vHideRedundantEvents INT;
DECLARE vDebugTrace VARCHAR(2000) DEFAULT '';
    SET vDebugTrace = CONCAT(pdebuginfo,' -> osae_sp_object_state_set');
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(object_name)=UPPER(pname);
    IF vObjectCount = 1 THEN
        SELECT object_id,object_type_id,state_id INTO vObjectID,vObjectTypeID,vOldStateID FROM osae_object WHERE UPPER(object_name)=UPPER(pname) LIMIT 1;
        SELECT COUNT(state_id) INTO vStateCount FROM osae_object_type_state WHERE object_type_id=vObjectTypeID AND (UPPER(state_name)=UPPER(pstate) OR UPPER(state_label)=UPPER(pstate));
        IF vStateCount = 1 THEN       
            SELECT state_id INTO vStateID FROM osae_object_type_state WHERE object_type_id=vObjectTypeID AND (UPPER(state_name)=UPPER(pstate) OR UPPER(state_label)=UPPER(pstate)) LIMIT 1;
            UPDATE osae_object SET state_id=vStateID,last_updated=NOW() WHERE object_id=vObjectID;
            SELECT COUNT(event_id) INTO vEventCount FROM osae_v_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pstate) LIMIT 1;
            IF vEventCount = 1 THEN
                SELECT hide_redundant_events INTO vHideRedundantEvents FROM osae_v_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pstate) LIMIT 1;
                IF vOldStateID <> vStateID OR vHideRedundantEvents = 0 Then
                    CALL osae_sp_event_log_add(pname,pstate,pfromobject,vDebugTrace,NULL,NULL);
                END IF;
            END IF;  
        END IF;
    END IF; 
END
$$

DELIMITER ;



DELIMITER $$
DROP PROCEDURE IF EXISTS osae_sp_object_type_clone$$

CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_clone(IN pnewname varchar(200), IN pbasename varchar(200))
BEGIN
DECLARE vBaseTypeID INT DEFAULT 0; 
DECLARE vNewTypeID INT;
    SELECT object_type_id INTO vBaseTypeID FROM osae_v_object_type WHERE object_type=pbasename;
    IF vBaseTypeID = 0 THEN
       CALL OBJECT_TYPE_DOES_NOT_EXIST();
    ELSE
      INSERT INTO osae_object_type (object_type,object_type_description,plugin_object_id,system_hidden,object_type_owner,base_type_id) SELECT pnewname,t.object_type_description,t.plugin_object_id,t.system_hidden,t.object_type_owner,t.base_type_id FROM osae_object_type t WHERE object_type=pbasename;
      SELECT object_type_id INTO vNewTypeID FROM osae_object_type WHERE object_type=pnewname;
      INSERT INTO osae_object_type_state (state_name,state_label,object_type_id) SELECT state_name,state_label,vNewTypeID FROM osae_object_type_state t WHERE object_type_id=vBaseTypeID;
      INSERT INTO osae_object_type_event (event_name,event_label,object_type_id) SELECT event_name,event_label,vNewTypeID FROM osae_object_type_event t WHERE object_type_id=vBaseTypeID;
      INSERT INTO osae_object_type_method (method_name,method_label,object_type_id) SELECT method_name,method_label,vNewTypeID FROM osae_object_type_method t WHERE object_type_id=vBaseTypeID;
      INSERT INTO osae_object_type_property (property_name,property_datatype,object_type_id) SELECT property_name,property_datatype,vNewTypeID FROM osae_object_type_property t WHERE object_type_id=vBaseTypeID;
    END IF;
END
$$


DELIMITER ;

DELIMITER $$
DROP PROCEDURE IF EXISTS osae_sp_object_state_set_by_address$$

CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_state_set_by_address(IN paddress varchar(400), IN pstate varchar(50), IN pfromobject varchar(200), IN pdebuginfo varchar(2000))
BEGIN
DECLARE vObjectCount INT;
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vStateCount INT;
DECLARE vOldStateID INT;
DECLARE vStateID INT;
DECLARE vEventCount INT;
DECLARE vHideRedundantEvents INT;
DECLARE vDebugTrace VARCHAR(2000) DEFAULT '';
    SET vDebugTrace = CONCAT(pdebuginfo,' -> osae_sp_object_state_set');
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(address)=UPPER(paddress);
    IF vObjectCount = 1 THEN
        SELECT object_id,object_type_id,state_id INTO vObjectID,vObjectTypeID,vOldStateID FROM osae_object WHERE UPPER(address)=UPPER(paddress) AND paddress != '' LIMIT 1;
        SELECT COUNT(state_id) INTO vStateCount FROM osae_object_type_state WHERE object_type_id=vObjectTypeID AND (UPPER(state_name)=UPPER(pstate) OR UPPER(state_label)=UPPER(pstate));
        IF vStateCount = 1 THEN       
            SELECT state_id INTO vStateID FROM osae_object_type_state WHERE object_type_id=vObjectTypeID AND (UPPER(state_name)=UPPER(pstate) OR UPPER(state_label)=UPPER(pstate)) LIMIT 1;
            UPDATE osae_object SET state_id=vStateID,last_updated=NOW() WHERE object_id=vObjectID;
            SELECT COUNT(event_id),hide_redundant_events INTO vEventCount,vHideRedundantEvents FROM osae_v_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pstate) LIMIT 1;
            IF vOldStateID <> vStateID OR vHideRedundantEvents = 0 Then
                IF vEventCount = 1 THEN  
                    CALL osae_sp_event_log_add(pname,pstate,pfromobject,vDebugTrace,NULL,NULL);
                END IF;
            END IF;  
        END IF;
    END IF; 
END
$$

DELIMITER ;



DELIMITER $$

DROP PROCEDURE IF EXISTS osae_sp_object_property_set$$

CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_set(IN pname varchar(200), IN pproperty varchar(200), IN pvalue varchar(255), IN pfromobject varchar(200), IN pdebuginfo varchar(2000))
BEGIN
DECLARE vObjectID INT DEFAULT 0;
DECLARE vObjectCount INT DEFAULT 0;
DECLARE vObjectTypeID INT DEFAULT 0;
DECLARE vPropertyID INT DEFAULT 0;
DECLARE vPropertyValue VARCHAR(2000);
DECLARE vPropertyCount INT DEFAULT 0;
DECLARE vEventCount INT;
DECLARE vDebugTrace VARCHAR(2000) DEFAULT '';
    SET vDebugTrace = CONCAT(pdebuginfo,' -> osae_sp_object_property_set');
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(object_name)=UPPER(pname); 
    IF vObjectCount > 0 THEN  
        SELECT object_id,object_type_id INTO vObjectID,vObjectTypeID FROM osae_object WHERE UPPER(object_name)=UPPER(pname);
        SELECT COUNT(property_id) INTO vPropertyCount FROM osae_v_object_property WHERE UPPER(object_name)=UPPER(pname) AND UPPER(property_name)=UPPER(pproperty) AND (property_value IS NULL OR property_value != pvalue);        
        IF vPropertyCount > 0 THEN
            SELECT property_id,COALESCE(property_value,'') INTO vPropertyID, vPropertyValue FROM osae_v_object_property WHERE UPPER(object_name)=UPPER(pname) AND UPPER(property_name)=UPPER(pproperty) AND (property_value IS NULL OR property_value != pvalue);
            UPDATE osae_object_property SET property_value=pvalue WHERE object_id=vObjectID AND object_type_property_id=vPropertyID;
            UPDATE osae_object SET last_updated=NOW() WHERE object_id=vObjectID;            
            SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pproperty);
            IF vEventCount > 0 THEN  
                CALL osae_sp_event_log_add(pname,pproperty,pfromobject,vDebugTrace,pvalue,NULL);
            END IF;
        END IF;
    END IF; 
END
$$

DELIMITER ;

DELIMITER $$

DROP PROCEDURE IF EXISTS osae_sp_object_event_script_update$$
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_event_script_update(IN pobject varchar(200), IN pevent varchar(200), IN ptext text)
BEGIN
  DECLARE vObjectCount INT;
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vEventCount INT;
DECLARE vEventID INT;
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(object_name)=UPPER(pobject);
    IF vObjectCount > 0 THEN
        SELECT object_id,object_type_id INTO vObjectID,vObjectTypeID FROM osae_object WHERE UPPER(object_name)=UPPER(pobject);
        SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND (UPPER(event_name)=UPPER(pevent) OR UPPER(event_label)=UPPER(pevent));
        IF vEventCount = 1 THEN     
            SELECT event_id INTO vEventID FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND (UPPER(event_name)=UPPER(pevent) OR UPPER(event_label)=UPPER(pevent));
            
            INSERT INTO osae_object_event_script (object_id,event_id,event_script) VALUES(vObjectID,vEventID,ptext)
            ON DUPLICATE KEY UPDATE event_script=ptext;
         -- CALL osae_sp_debug_log_add(CONCAT('Updated ',vObjectID,' - ',vEventID,ptext),'');  
        END IF;
    END IF; 
END
$$

DELIMITER ;


CALL osae_sp_object_type_method_add ('START PLUGIN','Start Plugin','SERVICE','Plugin name','','','');
CALL osae_sp_object_type_method_add ('STOP PLUGIN','Stop Plugin','SERVICE','Plugin name','','','');   
CALL osae_sp_object_type_state_add ('ERROR','Error','PLUGIN');
CALL osae_sp_object_type_event_add ('ERROR','Error','PLUGIN');

call osae_sp_pattern_add('Editor');
SET @vResults = 0;
CALL osae_sp_object_add('Custom Property List','Custom Property List','LIST','','SYSTEM',1,@vResults);


--  Added by Vaugh so VR won't listen to a speech client
CALL osae_sp_object_type_property_add ('Can Hear this Plugin','String','','VR CLIENT',0);
CALL osae_sp_object_type_property_add ('Speaking','Boolean','FALSE','SPEECH',0);


-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.3.9', '', '');