

--
-- Disable foreign keys
--
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

SET NAMES 'utf8';
USE osae;


--
-- Drop view "osae_v_event_script_logjunk"
--
DROP VIEW osae_v_event_script_logjunk CASCADE;

DELIMITER $$

--
-- Drop procedure "osae_sp_object_event_script_update"
--
DROP PROCEDURE IF EXISTS osae_sp_object_event_script_update$$

--
-- Drop procedure "osae_object_name_get_by_address"
--
DROP PROCEDURE IF EXISTS osae_object_name_get_by_address$$

DELIMITER ;

--
-- Create table "osae_images"
--
CREATE TABLE osae_images (
  image_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  image_data LONGBLOB NOT NULL,
  image_name VARCHAR(45) NOT NULL,
  image_type VARCHAR(4) NOT NULL,
  PRIMARY KEY (image_id)
)
ENGINE = INNODB
CHARACTER SET latin1
COLLATE latin1_swedish_ci;

--
-- Alter table "osae_pattern"
--
ALTER TABLE osae_pattern
  DROP COLUMN script,
  ADD COLUMN script_id INT(11) DEFAULT NULL AFTER pattern;

--
-- Create table "osae_script"
--
CREATE TABLE osae_script (
  script_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  script TEXT DEFAULT NULL,
  script_processor_id INT(10) UNSIGNED DEFAULT NULL,
  script_name VARCHAR(255) NOT NULL,
  PRIMARY KEY (script_id),
  UNIQUE INDEX script_name (script_name)
)
ENGINE = INNODB
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Create table "osae_script_processors"
--
CREATE TABLE osae_script_processors (
  script_processor_id INT(11) NOT NULL AUTO_INCREMENT,
  script_processor_name VARCHAR(45) NOT NULL COMMENT 'visual name in UI',
  script_processor_plugin_name VARCHAR(45) NOT NULL COMMENT 'the name of the plugin to process the script',
  PRIMARY KEY (script_processor_id),
  UNIQUE INDEX script_processor_id_UNIQUE (script_processor_id)
)
ENGINE = INNODB
CHARACTER SET latin1
COLLATE latin1_swedish_ci;

--
-- Create table "osae_pattern_script"
--
CREATE TABLE osae_pattern_script (
  pattern_script_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  pattern_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  script_id INT(11) DEFAULT NULL,
  script_sequence INT(11) DEFAULT NULL,
  PRIMARY KEY (pattern_script_id),
  UNIQUE INDEX script_sequence (script_sequence, pattern_id),
  CONSTRAINT FK_osae_pattern_script_osae_pattern_pattern_id FOREIGN KEY (pattern_id)
    REFERENCES osae_pattern(pattern_id) ON DELETE RESTRICT ON UPDATE RESTRICT
)
ENGINE = INNODB
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Create table "osae_object_type_event_script"
--
CREATE TABLE osae_object_type_event_script (
  object_type_event_script_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  object_type_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  event_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  script_id INT(11) DEFAULT NULL,
  script_sequence INT(11) DEFAULT NULL,
  PRIMARY KEY (object_type_event_script_id),
  UNIQUE INDEX script_sequence (script_sequence, object_type_id, event_id),
  CONSTRAINT FK_osae_object_type_event_script_osae_object_type_event_event_id FOREIGN KEY (event_id)
    REFERENCES osae_object_type_event(event_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_osae_object_type_event_script_osae_object_type_object_type_id FOREIGN KEY (object_type_id)
    REFERENCES osae_object_type(object_type_id) ON DELETE RESTRICT ON UPDATE RESTRICT
)
ENGINE = INNODB
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Alter table "osae_object_event_script"
--
ALTER TABLE osae_object_event_script
  DROP COLUMN event_script,
  DROP FOREIGN KEY osae_fk_object_script_to_events,
  DROP INDEX osae_fk_object_script_to_events,
  DROP FOREIGN KEY osae_fk_object_script_to_object,
  DROP INDEX osae_object_event_script_unq,
  ADD COLUMN script_id INT(11) DEFAULT NULL AFTER event_id,
  ADD COLUMN script_sequence INT(11) DEFAULT NULL AFTER script_id;

ALTER TABLE osae_object_event_script
  ADD UNIQUE INDEX script_sequence (script_sequence, object_id, event_id);

ALTER TABLE osae_object_event_script
  ADD CONSTRAINT FK_osae_object_event_script_osae_object_object_id FOREIGN KEY (object_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE RESTRICT;

ALTER TABLE osae_object_event_script
  ADD CONSTRAINT FK_osae_object_event_script_osae_object_type_event_event_id FOREIGN KEY (event_id)
    REFERENCES osae_object_type_event(event_id) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Alter table "osae_object_property"
--
ALTER TABLE osae_object_property
  CHANGE COLUMN property_value property_value VARCHAR(4000) DEFAULT NULL;

--
-- Alter table "osae_schedule_queue"
--
ALTER TABLE osae_schedule_queue
  DROP COLUMN pattern_id,
  DROP FOREIGN KEY osae_fk_schedule_pattern_to_pattern_id,
  DROP INDEX osae_fk_schedule_pattern_to_pattern_id,
  ADD COLUMN script_id INT(10) UNSIGNED DEFAULT NULL AFTER recurring_id;

ALTER TABLE osae_schedule_queue
  ADD CONSTRAINT osae_fk_schedule_script_to_script_id FOREIGN KEY (script_id)
    REFERENCES osae_script(script_id) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Alter table "osae_schedule_recurring"
--
ALTER TABLE osae_schedule_recurring
  DROP COLUMN pattern_id,
  DROP FOREIGN KEY osae_fk_recurring_pattern_to_pattern_id,
  DROP INDEX osae_fk_recurring_pattern_to_pattern_id,
  ADD COLUMN script_id INT(10) UNSIGNED DEFAULT NULL AFTER recurring_date;

ALTER TABLE osae_schedule_recurring
  ADD CONSTRAINT osae_fk_recurring_script_to_script_id FOREIGN KEY (script_id)
    REFERENCES osae_script(script_id) ON DELETE CASCADE ON UPDATE CASCADE;

DELIMITER $$

--
-- Create procedure "osae_sp_image_add"
--
CREATE PROCEDURE osae_sp_image_add(IN pimage_data LONGBLOB, IN pimage_name VARCHAR(45), IN pimage_type VARCHAR(4))
BEGIN
  DECLARE iid INT DEFAULT 0;
  SELECT image_id INTO iid FROM osae_images WHERE image_name = pimage_name;

  IF iid = 0 THEN
    INSERT INTO osae_images
  		(`image_data`,		
  		`image_name`,
  		`image_type`)
  		VALUES
  		(
  		pimage_data,		
  		pimage_name,
  		pimage_type
  		);
  
    SELECT LAST_INSERT_ID();
  ELSE
    SELECT iid;
  END IF;
END
$$

--
-- Create procedure "osae_sp_image_delete"
--
CREATE PROCEDURE osae_sp_image_delete(
IN pimage_id INT
)
BEGIN
	DELETE FROM `osae`.`osae_images`
	WHERE image_id = pimage_id;
END
$$

--
-- Alter procedure "osae_sp_object_event_script_add"
--
DROP PROCEDURE osae_sp_object_event_script_add$$
CREATE PROCEDURE osae_sp_object_event_script_add(IN pobject VARCHAR(200), IN pevent VARCHAR(200), IN pscriptid INT)
BEGIN
DECLARE vObjectCount INT;
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vEventCount INT;
DECLARE vEventID INT;
DECLARE vScriptSeq INT;
SET vScriptSeq = 0;
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE object_name=pobject;
    IF vObjectCount > 0 THEN
        SELECT object_id,object_type_id INTO vObjectID,vObjectTypeID FROM osae_object WHERE object_name=pobject;
        SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND (event_name=pevent OR event_label=pevent);
        IF vEventCount = 1 THEN       
            SELECT event_id INTO vEventID FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND (event_name=pevent OR event_label=pevent);
            SELECT COALESCE(script_sequence, 0) INTO vScriptSeq FROM osae_object_event_script where object_id = vObjectID AND event_id = vEventID ORDER BY script_sequence DESC LIMIT 1;
            INSERT INTO osae_object_event_script (object_id,event_id,script_id, script_sequence) VALUES(vObjectID,vEventID,pscriptid,vScriptSeq+1);
        END IF;
    END IF; 
END
$$

--
-- Create procedure "osae_sp_object_event_script_delete"
--
CREATE PROCEDURE osae_sp_object_event_script_delete(IN peventscriptid INT)
BEGIN
  DELETE FROM osae_object_event_script
    WHERE event_script_id = peventscriptid;
END
$$

--
-- Alter procedure "osae_sp_object_property_array_delete"
--
DROP PROCEDURE osae_sp_object_property_array_delete$$
CREATE PROCEDURE osae_sp_object_property_array_delete(
  IN  pobject    varchar(400),
  IN  pproperty  varchar(400),
  IN  pvalue     varchar(2000)
)
BEGIN
  DECLARE vPropertyArrayID INT;
  SELECT property_array_id INTO vPropertyArrayID FROM osae_v_object_property_array WHERE object_name=pobject AND property_name=pproperty AND item_name=pvalue LIMIT 1;
  
  DELETE FROM osae_object_property_array WHERE property_array_id=vPropertyArrayID;
END
$$

--
-- Create procedure "osae_sp_object_type_event_script_add"
--
CREATE PROCEDURE osae_sp_object_type_event_script_add(IN pobjtypename VARCHAR(255), IN pevent VARCHAR(255), IN pscriptid INT)
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
DECLARE vEventCount INT;
DECLARE vEventID INT;
DECLARE vScriptSeq INT;
SET vScriptSeq = 0;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjtypename;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjtypename;
        SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND (event_name=pevent OR event_label=pevent);
        IF vEventCount = 1 THEN       
            SELECT event_id INTO vEventID FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND (event_name=pevent OR event_label=pevent);
            SELECT COALESCE(script_sequence, 0) INTO vScriptSeq FROM osae_object_type_event_script where object_type_id = vObjectTypeID AND event_id = vEventID ORDER BY script_sequence DESC LIMIT 1;
            INSERT INTO osae_object_type_event_script (object_type_id,event_id,script_id, script_sequence) VALUES(vObjectTypeID,vEventID,pscriptid,vScriptSeq+1);
        END IF;
    END IF; 
END
$$

--
-- Create procedure "osae_sp_object_type_event_script_delete"
--
CREATE PROCEDURE osae_sp_object_type_event_script_delete(IN pobjtypeeventscriptid INT)
BEGIN
  DELETE FROM osae_object_type_event_script
    WHERE object_type_event_script_id = pobjtypeeventscriptid;
END
$$

--
-- Alter procedure "osae_sp_pattern_add"
--
DROP PROCEDURE osae_sp_pattern_add$$
CREATE PROCEDURE osae_sp_pattern_add(
  IN pname varchar(400)
)
BEGIN
  INSERT INTO osae_pattern (pattern) VALUES (pname);
END
$$

--
-- Create procedure "osae_sp_pattern_scripts_get"
--
CREATE PROCEDURE osae_sp_pattern_scripts_get(IN ppattern VARCHAR(255))
BEGIN
  SELECT script_id
    FROM osae_pattern_script s
    INNER JOIN osae_pattern p ON p.pattern_id = s.pattern_id
    WHERE p.pattern = ppattern;
END
$$

--
-- Create procedure "osae_sp_pattern_script_add"
--
CREATE PROCEDURE osae_sp_pattern_script_add(IN ppattern VARCHAR(255), IN pscriptid INT)
BEGIN
DECLARE vPatternCount INT;
DECLARE vPatternID INT;
DECLARE vScriptSeq INT;
SET vScriptSeq = 0;
    SELECT COUNT(pattern_id) INTO vPatternCount FROM osae_pattern WHERE pattern=ppattern;
    IF vPatternCount > 0 THEN
        SELECT pattern_id INTO vPatternID FROM osae_pattern WHERE pattern=ppattern;
        SELECT COALESCE(script_sequence, 0) INTO vScriptSeq FROM osae_pattern_script where pattern_id = vPatternID ORDER BY script_sequence DESC LIMIT 1;
        INSERT INTO osae_pattern_script (pattern_id,script_id, script_sequence) VALUES(vPatternID,pscriptid,vScriptSeq+1);
        
    END IF; 
END
$$

--
-- Create procedure "osae_sp_pattern_script_delete"
--
CREATE PROCEDURE osae_sp_pattern_script_delete(IN ppatternscriptid INT)
BEGIN
DELETE FROM osae_pattern_script
    WHERE pattern_script_id = ppatternscriptid;
END
$$

--
-- Alter procedure "osae_sp_pattern_update"
--
DROP PROCEDURE osae_sp_pattern_update$$
CREATE PROCEDURE osae_sp_pattern_update(
  IN  poldpattern  varchar(400),
  IN  pnewpattern  varchar(400)
)
BEGIN
  UPDATE osae_pattern SET pattern=pnewpattern WHERE pattern=poldpattern;
END
$$

--
-- Alter procedure "osae_sp_process_recurring"
--
DROP PROCEDURE osae_sp_process_recurring$$
CREATE PROCEDURE osae_sp_process_recurring()
BEGIN
DECLARE iRECURRINGID INT;
DECLARE vOBJECTNAME VARCHAR(400) DEFAULT '';
DECLARE vMETHODNAME VARCHAR(400) DEFAULT '';
DECLARE vPARAM1 VARCHAR(200);
DECLARE vPARAM2 VARCHAR(200);
DECLARE vSCRIPTNAME VARCHAR(200);
DECLARE iSCRIPTID INT;
DECLARE cINTERVAL CHAR(1);
DECLARE cSUNDAY CHAR(1);
DECLARE cMONDAY CHAR(1);
DECLARE cTUESDAY CHAR(1);
DECLARE cWEDNESDAY CHAR(1);
DECLARE cTHURSDAY CHAR(1);
DECLARE cFRIDAY CHAR(1);
DECLARE cSATURDAY CHAR(1);
DECLARE dRECURRINGDATE DATE;
DECLARE iRECURRINGMINUTES INT;
DECLARE dRECURRINGDAY INT;
DECLARE dRECURRINGTIME TIME;
DECLARE dCURDATE DATE;
DECLARE dCURDATETIME DATETIME;
DECLARE dCURDAYOFWEEK INT DEFAULT 0;
DECLARE dCURDAYOFMONTH INT DEFAULT 1;
DECLARE dCURDAY INT DEFAULT 1;
DECLARE iMATCHES INT DEFAULT 0;
DECLARE iDATEDIFF INT DEFAULT 0;
DECLARE done INT DEFAULT 0;  
DECLARE cur1 CURSOR FOR SELECT recurring_id,interval_unit,recurring_time,recurring_minutes,recurring_date,recurring_day,object_name,method_name,parameter_1,parameter_2,script_id, script_name, sunday,monday,tuesday,wednesday,thursday,friday,saturday FROM osae_v_schedule_recurring;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
    OPEN cur1; 
    Loop_Tag: LOOP
        IF done THEN
            Leave Loop_Tag;
        END IF;
        FETCH cur1 INTO iRECURRINGID,cINTERVAL,dRECURRINGTIME,iRECURRINGMINUTES,dRECURRINGDATE,dRECURRINGDAY,vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,iSCRIPTID,vSCRIPTNAME,cSUNDAY,cMONDAY,cTUESDAY,cWEDNESDAY,cTHURSDAY,cFRIDAY,cSATURDAY;
        CALL osae_sp_debug_log_add(CONCAT('ID=',iRECURRINGID,', Interval=',cINTERVAL,' Time=',dRECURRINGTIME,' Date=',dRECURRINGDATE),'sp_process_recurring'); 
        IF NOT done THEN
            IF cINTERVAL = 'Y' THEN
                SET dCURDATE = CURDATE();
                CALL osae_sp_debug_log_add(CONCAT('--IF ',dRECURRINGDATE,' < ',dCURDATE,' THEN'),'SYSTEM'); 
                IF dRECURRINGDATE < dCURDATE THEN
                    SET iDATEDIFF = DATEDIFF(dCURDATE,dRECURRINGDATE) / 365; 
                    CALL osae_sp_debug_log_add(CONCAT('sp_process_recurring: DateDiff=',iDATEDIFF),'SYSTEM'); 
                    SET dRECURRINGDATE = DATE_ADD(dRECURRINGDATE,INTERVAL iDATEDIFF YEAR);
                    IF dRECURRINGDATE < dCURDATE THEN 
                        SET dRECURRINGDATE = DATE_ADD(dRECURRINGDATE,INTERVAL 1 YEAR);                   
                    END IF;                                     
                END IF;
                CALL osae_sp_debug_log_add(CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),'SYSTEM'); 
                SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                IF iMATCHES = 0 THEN
                    CALL osae_sp_schedule_queue_add (CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);
                END IF;
              ELSEIF cINTERVAL = 'T' THEN   
                SET dCURDATETIME = NOW();             
                SET dCURDATE = CURDATE();
                IF dCURDATETIME > ADDTIME(NOW(),SEC_TO_TIME(iRECURRINGMINUTES * 60)) THEN
                    SET dCURDAYOFWEEK = dCURDAYOFWEEK + 1;
                    SET dCURDATE=DATE_ADD(CURDATE(),INTERVAL 1 DAY);
                END IF; 
                CALL osae_sp_debug_log_add(CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),'SYSTEM'); 
                SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                IF iMATCHES = 0 THEN
                    CALL osae_sp_schedule_queue_add (ADDTIME(NOW(),SEC_TO_TIME(iRECURRINGMINUTES * 60)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);
               END IF;               
            ELSEIF cINTERVAL = 'M' THEN                
                SET dCURDATE = CURDATE();
                SET dRECURRINGDATE = CONCAT(YEAR(NOW()),'-',MONTH(NOW()),'-' ,dRECURRINGDAY);                
                IF dRECURRINGDATE < dCURDATE THEN
                    CALL osae_sp_debug_log_add(CONCAT('sp_process_recurring: DateDiff=',iDATEDIFF),'SYSTEM');                
                    SET dRECURRINGDATE = DATE_ADD(dRECURRINGDATE,INTERVAL 1 MONTH);
                    IF dRECURRINGDATE < dCURDATE THEN 
                        SET dRECURRINGDATE = DATE_ADD(dRECURRINGDATE,INTERVAL 1 MONTH);                   
                    END IF;                                     
                END IF;
                CALL osae_sp_debug_log_add(CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),'SYSTEM'); 
                SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                IF iMATCHES = 0 THEN
                    CALL osae_sp_schedule_queue_add (CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);
                END IF;               
            ELSEIF cINTERVAL = 'D' THEN                
                SET dCURDATETIME = NOW();
                SET dCURDATE = CURDATE();
                SET dCURDAYOFWEEK = DAYOFWEEK(NOW()); 
                SET dCURDAYOFMONTH = DAYOFMONTH(NOW());
  
                IF dCURDATETIME > CONCAT(dCURDATE,' ',dRECURRINGTIME) THEN
                    SET dCURDAYOFWEEK = dCURDAYOFWEEK + 1;
                    SET dCURDATE=DATE_ADD(CURDATE(),INTERVAL 1 DAY);
                END IF; 
                CALL osae_sp_debug_log_add(CONCAT('IF ',dCURDATETIME,' > ',dCURDATE,' ',dRECURRINGTIME,' Then Write new queue'),'SYSTEM');              
                IF dCURDAYOFWEEK = 1 AND cSUNDAY = 1 THEN
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);
                    END IF; 
                END IF; 
                IF dCURDAYOFWEEK = 2 AND cMONDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);          
                    END IF; 
                END IF; 
                IF dCURDAYOFWEEK = 3 AND cTUESDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);
                    END IF; 
                END IF;                 
                IF dCURDAYOFWEEK = 4 AND cWEDNESDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);   
                    END IF; 
                END IF;  
                IF dCURDAYOFWEEK = 5 AND cTHURSDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);                    
                    END IF; 
                END IF;
                IF dCURDAYOFWEEK = 6 AND cFRIDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);                    
                    END IF; 
                END IF;
                IF dCURDAYOFWEEK = 7 AND cSATURDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);                    
                   END IF; 
                END IF;                                                                           
            END IF;         
        END IF;
     END LOOP;
    CLOSE cur1;   
END
$$

--
-- Alter procedure "osae_sp_run_scheduled_methods"
--
DROP PROCEDURE osae_sp_run_scheduled_methods$$
CREATE PROCEDURE osae_sp_run_scheduled_methods()
BEGIN
DECLARE iSCHEDULEID INT;
DECLARE iOBJECTID INT DEFAULT 0;
DECLARE vObjectName VARCHAR(400) DEFAULT '';
DECLARE iMETHODID INT DEFAULT 0;
DECLARE vMethodName VARCHAR(400);
DECLARE vPARAM1 VARCHAR(200);
DECLARE vPARAM2 VARCHAR(200);
DECLARE iSCRIPTID INT DEFAULT 0;
DECLARE vSCRIPTPROCID INT DEFAULT 0;
DECLARE done INT DEFAULT 0;  
DECLARE scriptProc VARCHAR(200);
DECLARE cur1 CURSOR FOR SELECT schedule_ID,COALESCE(object_name,''),method_name,parameter_1,parameter_2,script_id FROM osae_v_schedule_queue WHERE queue_datetime < NOW();
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
    CALL osae_sp_debug_log_add('Starting to run osae_sp_run_scheduled_methods','osae_sp_run_scheduled_methods');
    OPEN cur1; 
    Loop_Tag: LOOP
        FETCH cur1 INTO iSCHEDULEID,vObjectName,vMethodName,vPARAM1,vPARAM2,iSCRIPTID;
        IF done THEN
            Leave Loop_Tag;
        END IF;
            CALL osae_sp_debug_log_add(CONCAT('Found Scheduled Method to run:',iSCHEDULEID,'  Object=',vObjectName,'   ScriptID=',COALESCE(iSCRIPTID,0)),'osae_sp_run_scheduled_methods');
            DELETE FROM osae_schedule_queue WHERE schedule_ID=iSCHEDULEID; 
            IF vObjectName != '' THEN
                CALL osae_sp_method_queue_add(vObjectName,vMethodName,vPARAM1,vPARAM2,'SYSTEM','osae_sp_run_scheduled_methods');
            ELSEIF iSCRIPTID != 0 THEN
                SELECT script_processor_id INTO vSCRIPTPROCID FROM osae_script WHERE script_id=iSCRIPTID;
                SELECT script_processor_name INTO scriptProc FROM osae_script_processor WHERE script_processor_id=vSCRIPTPROCID;
                CALL osae_sp_method_queue_add(scriptProc,'RUN SCRIPT',vSCRIPTID,'SYSTEM','SYSTEM','osae_sp_run_scheduled_methods');
            END IF;         
        END LOOP;
    CLOSE cur1;
    CALL osae_sp_process_recurring();   
END
$$

--
-- Alter procedure "osae_sp_schedule_queue_add"
--
DROP PROCEDURE osae_sp_schedule_queue_add$$
CREATE PROCEDURE osae_sp_schedule_queue_add(IN pscheduleddate DATETIME, IN pobject VARCHAR(400), IN pmethod VARCHAR(400), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000), IN pscript VARCHAR(200), IN precurringid INT(10))
BEGIN
DECLARE vObjectID INT DEFAULT NULL;
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vScriptID INT DEFAULT NULL;
DECLARE vRecurringID INT DEFAULT NULL;
    SELECT script_id INTO vScriptID FROM osae_script WHERE UPPER(script_name)=UPPER(pscript);
    SELECT object_id,method_id INTO vObjectID,vMethodID FROM osae_v_object_method WHERE UPPER(object_name)=UPPER(pobject) AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    IF precurringid > 0 THEN
        SET vRecurringID = precurringid;
    END IF;
    INSERT INTO osae_schedule_queue (queue_datetime,object_id,method_id,parameter_1,parameter_2,script_id,recurring_id) VALUES(pscheduleddate,vObjectID,vMethodID,pparameter1,pparameter2,vScriptID,vRecurringID);
END
$$

--
-- Alter procedure "osae_sp_schedule_recurring_add"
--
DROP PROCEDURE osae_sp_schedule_recurring_add$$
CREATE PROCEDURE osae_sp_schedule_recurring_add(IN pschedule_name VARCHAR(400), IN pobject VARCHAR(400), IN pmethod VARCHAR(400), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000), IN pscript VARCHAR(400), IN precurringtime TIME, IN psunday TINYINT(1), IN pmonday TINYINT(1), IN ptuesday TINYINT(1), IN pwednesday TINYINT(1), IN pthursday TINYINT(1), IN pfriday TINYINT(1), IN psaturday TINYINT(1), IN pinterval VARCHAR(10), IN precurringminutes INT(8), IN precurringday INT(4), IN precurringdate DATE)
BEGIN
DECLARE vObjectID INT DEFAULT NULL;
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vScriptID INT DEFAULT NULL;
    SELECT script_id INTO vScriptID FROM osae_script WHERE script_name=pscript;
    SELECT object_id,method_id INTO vObjectID,vMethodID FROM osae_v_object_method WHERE UPPER(object_name)=UPPER(pobject) AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    INSERT INTO osae_schedule_recurring (schedule_name,object_id,method_id,parameter_1,parameter_2,script_id,interval_unit,recurring_time,recurring_minutes,recurring_day,recurring_date,sunday,monday,tuesday,wednesday,thursday,friday,saturday) VALUES(pschedule_name,vObjectID,vMethodID,pparameter1,pparameter2,vScriptID,pinterval,precurringtime,precurringminutes,precurringday,precurringdate,psunday,pmonday,ptuesday,pwednesday,pthursday,pfriday,psaturday);
END
$$

--
-- Alter procedure "osae_sp_schedule_recurring_update"
--
DROP PROCEDURE osae_sp_schedule_recurring_update$$
CREATE PROCEDURE osae_sp_schedule_recurring_update(IN poldschedulename VARCHAR(400), IN pnewschedulename VARCHAR(400), IN pobject VARCHAR(400), IN pmethod VARCHAR(400), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000), IN pscript VARCHAR(400), IN precurringtime TIME, IN psunday TINYINT(1), IN pmonday TINYINT(1), IN ptuesday TINYINT(1), IN pwednesday TINYINT(1), IN pthursday TINYINT(1), IN pfriday TINYINT(1), IN psaturday TINYINT(1), IN pinterval VARCHAR(10), IN precurringminutes INT(8), IN precurringday INT(4), IN pprecurringdate DATE)
BEGIN
DECLARE vObjectID INT DEFAULT NULL;
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vScriptID INT DEFAULT NULL;
    SELECT script_id INTO vScriptID FROM osae_script WHERE script_name=pscript;
    SELECT object_id,method_id INTO vObjectID,vMethodID FROM osae_v_object_method WHERE UPPER(object_name)=UPPER(pobject) AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    UPDATE osae_schedule_recurring SET schedule_name=pnewschedulename,object_id=vObjectID,method_id=vMethodID,parameter_1=pparameter1,parameter_2=pparameter2,script_id=vScriptID,interval_unit=pinterval,recurring_time=precurringtime,recurring_minutes=precurringminutes,recurring_day=precurringday,recurring_date=pprecurringdate,sunday=psunday,monday=pmonday,tuesday=ptuesday,wednesday=pwednesday,thursday=pthursday,friday=pfriday,saturday=psaturday WHERE schedule_name=poldschedulename;
END
$$

--
-- Create procedure "osae_sp_script_add"
--
CREATE PROCEDURE osae_sp_script_add(IN pname VARCHAR(255), IN pscriptprocessor VARCHAR(255), IN pscript TEXT)
BEGIN
  DECLARE vScriptProcessorID INT;
  SELECT script_processor_id INTO vScriptProcessorID FROM osae_script_processors WHERE script_processor_name = pscriptprocessor;
  INSERT INTO osae_script(script_name, script_processor_id, script)
    VALUES(pname,vScriptProcessorID,pscript);
END
$$

--
-- Create procedure "osae_sp_script_delete"
--
CREATE PROCEDURE osae_sp_script_delete(IN pname VARCHAR(255))
BEGIN
  DELETE FROM osae_script
    WHERE script_name = pname;
END
$$

--
-- Create procedure "osae_sp_script_processor_add"
--
CREATE PROCEDURE osae_sp_script_processor_add(IN pname VARCHAR(255), IN ppluginname VARCHAR(255))
BEGIN
  DECLARE vCount INT;

   SELECT count(script_processor_id) INTO vCount FROM osae_script_processors WHERE script_processor_name = pname;
   IF vCount = 0 THEN
      INSERT INTO osae_script_processors(script_processor_name, script_processor_plugin_name)
      VALUES(pname,ppluginname);
   END IF;
END
$$

--
-- Create procedure "osae_sp_script_processor_by_event_script_id"
--
CREATE PROCEDURE osae_sp_script_processor_by_event_script_id(
IN pEventScriptId int
)
BEGIN
SELECT script_processor_plugin_name FROM osae_script_processors
INNER JOIN osae_object_event_script
ON osae_script_processors.script_processor_id = osae_object_event_script.script_processor_id
WHERE osae_object_event_script.event_script_id = pEventScriptId;
END
$$

--
-- Create procedure "osae_sp_script_processor_by_pattern"
--
CREATE PROCEDURE osae_sp_script_processor_by_pattern(
IN pPattern VARCHAR(400)
)
BEGIN
SELECT script_processor_plugin_name FROM osae_script_processors
INNER JOIN osae_pattern
ON osae_script_processors.script_processor_id = osae_pattern.script_processor_id
WHERE osae_pattern.pattern = pPattern;
END
$$

--
-- Create procedure "osae_sp_script_processor_by_script_id"
--
CREATE PROCEDURE osae_sp_script_processor_by_script_id(
IN pScriptId int
)
BEGIN
  SELECT script_processor_plugin_name
  FROM
    osae_script_processors
  INNER JOIN osae_script
  ON osae_script_processors.script_processor_id = osae_script.script_processor_id
  WHERE
    osae_script.script_id = pScriptId;
END
$$

--
-- Create procedure "osae_sp_script_update"
--
CREATE PROCEDURE osae_sp_script_update(IN poldname VARCHAR(255), IN pname VARCHAR(255), IN pscriptprocessor VARCHAR(255), IN pscript TEXT)
BEGIN
  DECLARE vScriptProcessorID INT;
  SELECT script_processor_id INTO vScriptProcessorID FROM osae_script_processors WHERE script_processor_name = pscriptprocessor;

  UPDATE osae_script
    SET script_name = pname, script_processor_id = vScriptProcessorID, script = pscript
    WHERE script_name = poldname;

END
$$

--
-- Alter trigger "tr_osae_event_log_after_insert"
--
DROP TRIGGER IF EXISTS tr_osae_event_log_after_insert$$
CREATE TRIGGER tr_osae_event_log_after_insert
	AFTER INSERT
	ON osae_event_log
	FOR EACH ROW
BEGIN
DECLARE vEventCount INT;
DECLARE vScriptID INT;
DECLARE vScriptSeq INT;
DECLARE vPrevScriptSeq INT;
DECLARE scriptProc VARCHAR(200);
DECLARE vDebugTrace VARCHAR(2000);
    SET vPrevScriptSeq = -1;
    SET vScriptSeq = 0;
    SET vDebugTrace = CONCAT(COALESCE(NEW.debug_trace,''),' -> tr_osae_event_log_after_insert');
    CALL osae_sp_debug_log_add(CONCAT('Event_Trigger is running for ',NEW.object_id,' ',NEW.event_id),vDebugTrace);
    SELECT COUNT(event_script_id) INTO vEventCount FROM osae_v_object_event_script WHERE object_id=NEW.object_id AND event_id=NEW.event_id AND script IS NOT NULL and script<>'';
    IF vEventCount > 0 THEN
        WHILE vPrevScriptSeq != vScriptSeq DO
          SET vPrevScriptSeq = vScriptSeq; 
          SELECT script_sequence, s.script_id, sp.script_processor_plugin_name INTO vScriptSeq,vScriptID,scriptProc 
            FROM osae_v_object_event_script e
            INNER JOIN osae_script s ON e.script_id = s.script_id
            INNER JOIN osae_script_processors sp ON sp.script_processor_id = s.script_processor_id
            WHERE object_id=NEW.object_id AND event_id=NEW.event_id AND s.script IS NOT NULL and s.script<>'' AND script_sequence > vScriptSeq
            ORDER BY script_sequence ASC LIMIT 1;
          IF vPrevScriptSeq != vScriptSeq THEN
            # CALL osae_sp_debug_log_add(CONCAT(vScriptSeq, '-',vScriptID,'-',scriptProc),vDebugTrace);
            CALL osae_sp_method_queue_add (scriptProc,'RUN SCRIPT',vScriptID,'','SYSTEM',vDebugTrace);
          END IF;
        END WHILE;
    END IF; 
END
$$

DELIMITER ;

--
-- Alter view "osae_v_object_event_script"
--
CREATE OR REPLACE 
VIEW osae_v_object_event_script
AS
	select `osae_object_event_script`.`event_script_id` AS `event_script_id`,`osae_object_event_script`.`script_id` AS `script_id`,`osae_script`.`script_name` AS `script_name`,`osae_object_event_script`.`script_sequence` AS `script_sequence`,`osae_script`.`script` AS `script`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_type_id` AS `object_type_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`container_id` AS `container_id`,`osae_object`.`enabled` AS `enabled`,`osae_object_type_event`.`event_id` AS `event_id`,`osae_object_type_event`.`event_name` AS `event_name`,`osae_object_type_event`.`event_label` AS `event_label` from (((`osae_object` join `osae_object_event_script` on((`osae_object`.`object_id` = `osae_object_event_script`.`object_id`))) join `osae_object_type_event` on((`osae_object_event_script`.`event_id` = `osae_object_type_event`.`event_id`))) join `osae_script` on((`osae_script`.`script_id` = `osae_object_event_script`.`script_id`)));

--
-- Create view "osae_v_object_type_event_script"
--
CREATE
VIEW osae_v_object_type_event_script
AS
SELECT
  `osae_object_type_event_script`.`object_type_event_script_id` AS `object_type_event_script_id`,
  `osae_object_type_event_script`.`script_id` AS `script_id`,
  `osae_script`.`script_name` AS `script_name`,
  `osae_object_type_event_script`.`script_sequence` AS `script_sequence`,
  `osae_script`.`script` AS `script`,
  `osae_object_type`.`object_type_id` AS `object_type_id`,
  `osae_object_type`.`object_type` AS `object_type`,
  `osae_object_type`.`object_type_description` AS `object_type_description`,
  `osae_object_type_event`.`event_id` AS `event_id`,
  `osae_object_type_event`.`event_name` AS `event_name`,
  `osae_object_type_event`.`event_label` AS `event_label`
FROM (((`osae_object_type`
  JOIN `osae_object_type_event_script`
    ON ((`osae_object_type`.`object_type_id` = `osae_object_type_event_script`.`object_type_id`)))
  JOIN `osae_object_type_event`
    ON ((`osae_object_type_event_script`.`event_id` = `osae_object_type_event`.`event_id`)))
  JOIN `osae_script`
    ON ((`osae_script`.`script_id` = `osae_object_type_event_script`.`script_id`)));

--
-- Alter view "osae_v_pattern"
--
CREATE OR REPLACE 
VIEW osae_v_pattern
AS
	select `osae_pattern`.`pattern_id` AS `pattern_id`,`osae_pattern`.`pattern` AS `pattern`,`osae_pattern`.`script_id` AS `script_id`,`osae_pattern_match`.`match_id` AS `match_id`,`osae_pattern_match`.`match` AS `match` from (`osae_pattern` left join `osae_pattern_match` on((`osae_pattern`.`pattern_id` = `osae_pattern_match`.`pattern_id`)));

--
-- Alter view "osae_v_schedule_queue"
--
CREATE OR REPLACE 
VIEW osae_v_schedule_queue
AS
	select `osae_schedule_queue`.`schedule_id` AS `schedule_id`,`osae_schedule_queue`.`queue_datetime` AS `queue_datetime`,`osae_schedule_queue`.`parameter_1` AS `parameter_1`,`osae_schedule_queue`.`parameter_2` AS `parameter_2`,`osae_script`.`script_id` AS `script_id`,`osae_script`.`script_name` AS `script_name`,`osae_script`.`script` AS `script`,`osae_object_type_method`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_object_type_method`.`param_1_label` AS `param_1_label`,`osae_object_type_method`.`param_2_label` AS `param_2_label`,`osae_object_type_method`.`param_1_default` AS `param_1_default`,`osae_object_type_method`.`param_2_default` AS `param_2_default`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_type_id` AS `object_type_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`container_id` AS `container_id`,`osae_object`.`enabled` AS `enabled`,`osae_object`.`last_updated` AS `last_updated`,`osae_schedule_recurring`.`recurring_id` AS `recurring_id`,coalesce(`osae_schedule_recurring`.`schedule_name`,'One Time Execution') AS `schedule_name` from ((((`osae_schedule_queue` left join `osae_object` on((`osae_object`.`object_id` = `osae_schedule_queue`.`object_id`))) left join `osae_object_type_method` on((`osae_schedule_queue`.`method_id` = `osae_object_type_method`.`method_id`))) left join `osae_script` on((`osae_schedule_queue`.`script_id` = `osae_script`.`script_id`))) left join `osae_schedule_recurring` on((`osae_schedule_queue`.`recurring_id` = `osae_schedule_recurring`.`recurring_id`)));

--
-- Alter view "osae_v_schedule_recurring"
--
CREATE OR REPLACE 
VIEW osae_v_schedule_recurring
AS
	select `osae_schedule_recurring`.`recurring_id` AS `recurring_id`,`osae_schedule_recurring`.`schedule_name` AS `schedule_name`,`osae_schedule_recurring`.`parameter_1` AS `parameter_1`,`osae_schedule_recurring`.`parameter_2` AS `parameter_2`,`osae_schedule_recurring`.`recurring_time` AS `recurring_time`,`osae_schedule_recurring`.`monday` AS `monday`,`osae_schedule_recurring`.`tuesday` AS `tuesday`,`osae_schedule_recurring`.`wednesday` AS `wednesday`,`osae_schedule_recurring`.`thursday` AS `thursday`,`osae_schedule_recurring`.`friday` AS `friday`,`osae_schedule_recurring`.`saturday` AS `saturday`,`osae_schedule_recurring`.`sunday` AS `sunday`,`osae_schedule_recurring`.`interval_unit` AS `interval_unit`,`osae_schedule_recurring`.`recurring_minutes` AS `recurring_minutes`,`osae_schedule_recurring`.`recurring_day` AS `recurring_day`,`osae_schedule_recurring`.`recurring_date` AS `recurring_date`,`osae_script`.`script_id` AS `script_id`,`osae_script`.`script_name` AS `script_name`,`osae_script`.`script` AS `script`,`osae_object_type_method`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_object_type_method`.`object_type_id` AS `object_type_id`,`osae_object_type_method`.`param_1_label` AS `param_1_label`,`osae_object_type_method`.`param_2_label` AS `param_2_label`,`osae_object_type_method`.`param_1_default` AS `param_1_default`,`osae_object_type_method`.`param_2_default` AS `param_2_default`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id` from (((`osae_schedule_recurring` left join `osae_object` on((`osae_schedule_recurring`.`object_id` = `osae_object`.`object_id`))) left join `osae_script` on((`osae_schedule_recurring`.`script_id` = `osae_script`.`script_id`))) left join `osae_object_type_method` on((`osae_schedule_recurring`.`method_id` = `osae_object_type_method`.`method_id`)));

--
-- Alter view "osae_v_screen_object"
--
CREATE OR REPLACE 
VIEW osae_v_screen_object
AS
	select `so`.`screen_object_id` AS `screen_object_id`,`so`.`screen_id` AS `screen_id`,`so`.`object_id` AS `object_id`,`so`.`control_id` AS `control_id`,`screen`.`object_name` AS `screen_name`,`co`.`object_name` AS `control_name`,`oo`.`object_name` AS `object_name`,`ot`.`object_type` AS `control_type`,`oo`.`last_updated` AS `last_updated`,`oo`.`last_state_change` AS `last_state_change`,timestampdiff(MINUTE,`oo`.`last_state_change`,now()) AS `time_in_state`,`ots`.`state_name` AS `state_name` from (((((`osae_screen_object` `so` join `osae_object` `screen` on((`screen`.`object_id` = `so`.`screen_id`))) join `osae_object` `oo` on((`so`.`object_id` = `oo`.`object_id`))) join `osae_object` `co` on((`so`.`control_id` = `co`.`object_id`))) join `osae_object_type` `ot` on((`ot`.`object_type_id` = `co`.`object_type_id`))) left join `osae_object_type_state` `ots` on((`ots`.`state_id` = `oo`.`state_id`)));

--
-- Alter view "osae_v_screen_updates"
--
CREATE OR REPLACE 
VIEW osae_v_screen_updates
AS
	select `osae_v_screen_object`.`screen_object_id` AS `screen_object_id`,`osae_v_screen_object`.`screen_id` AS `screen_id`,`osae_v_screen_object`.`object_id` AS `object_id`,`osae_v_screen_object`.`control_id` AS `control_id`,`osae_v_screen_object`.`screen_name` AS `screen_name`,`osae_v_screen_object`.`control_name` AS `control_name`,`osae_v_screen_object`.`object_name` AS `object_name`,`osae_v_screen_object`.`last_updated` AS `last_updated`,`osae_v_screen_object`.`last_state_change` AS `last_state_change`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_state`.`state_label` AS `state_label`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_property`.`property_value` AS `property_value`,`osae_v_screen_object`.`control_type` AS `control_type` from ((((`osae_object` left join `osae_object_type_state` on((`osae_object`.`state_id` = `osae_object_type_state`.`state_id`))) join `osae_v_screen_object` on((`osae_object`.`object_id` = `osae_v_screen_object`.`object_id`))) left join `osae_object_type_property` on((`osae_object_type_property`.`object_type_id` = `osae_object`.`object_type_id`))) left join `osae_object_property` on((`osae_object_type_property`.`property_id` = `osae_object_property`.`object_type_property_id`))) where (`osae_v_screen_object`.`last_updated` > subtime(now(),'00:00:30'));

--
-- Enable foreign keys
--
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;



delimiter ;

alter table osae_object_property modify property_value VARCHAR(4000) DEFAULT NULL ; 

CALL osae_sp_object_type_state_update('OPENED','ON','Opened','X10 DS10A');
CALL osae_sp_object_type_state_update('CLOSED','OFF','Closed','X10 DS10A');
CALL osae_sp_object_type_event_update('OPENED','ON','Opened','X10 DS10A');
CALL osae_sp_object_type_event_update('CLOSED','OFF','Closed','X10 DS10A');

CALL osae_sp_script_processor_add ('OSA Default Script Processor', 'Script Processor');

CALL osae_sp_object_type_method_add ('RUN SCRIPT','Run Script','Script Processor','','','','');

CALL osae_sp_object_type_add ('CONTROL CAMERA VIEWER','Control - IP Camera Viewer','','CONTROL',0,1,0,1);
CALL osae_sp_object_type_property_add ('X','Integer','','CONTROL CAMERA VIEWER',1);
CALL osae_sp_object_type_property_add ('Y','Integer','','CONTROL CAMERA VIEWER',1);
CALL osae_sp_object_type_property_add ('ZOrder','Integer','','CONTROL CAMERA VIEWER',1);
CALL osae_sp_object_type_property_add ('Object Name','String','','CONTROL CAMERA VIEWER',0);
CALL osae_sp_object_type_add ('IP CAMERA','IP Camera','','IP CAMERA',0,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Motion','IP CAMERA');
CALL osae_sp_object_type_state_add ('OFF','Still','IP CAMERA');
CALL osae_sp_object_type_event_add ('ON','Motion','IP CAMERA');
CALL osae_sp_object_type_event_add ('OFF','Still','IP CAMERA');
CALL osae_sp_object_type_property_add ('Stream Address','String','','IP CAMERA',0);

CALL osae_sp_object_type_add ('REST','Rest API','','REST',0,0,0,1);
CALL osae_sp_object_type_state_add ('OFF','Stopped','REST');
CALL osae_sp_object_type_state_add ('ON','Running','REST');
CALL osae_sp_object_type_event_add ('OFF','Stopped','REST');
CALL osae_sp_object_type_event_add ('ON','Started','REST');
CALL osae_sp_object_type_method_add ('OFF','Stop','REST','','','','');
CALL osae_sp_object_type_method_add ('ON','Start','REST','','','','');
CALL osae_sp_object_type_property_add ('System Plugin','Boolean','TRUE','REST',0);
CALL osae_sp_object_type_property_add ('Show Help','Boolean','TRUE','REST',0);

CALL osae_sp_object_type_property_update ('Password', 'Password', 'Password', '', 'PERSON', 0);

-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.4.0', '', '');