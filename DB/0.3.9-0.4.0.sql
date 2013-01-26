
USE osae;

DELIMITER $$

DROP TRIGGER IF EXISTS tr_osae_event_log_after_insert$$
CREATE
DEFINER = 'osae'@'%'
TRIGGER tr_osae_event_log_after_insert
	AFTER INSERT
	ON osae_event_log
	FOR EACH ROW
BEGIN
DECLARE vEventCount INT;
DECLARE vEventID INT;
DECLARE vEventScriptID INT;
DECLARE vEventName VARCHAR(200);
DECLARE vObjectName VARCHAR(200);
DECLARE scriptProc VARCHAR(200);
DECLARE vDebugTrace VARCHAR(2000);
    SET vDebugTrace = CONCAT(COALESCE(NEW.debug_trace,''),' -> tr_osae_event_log_after_insert');
    CALL osae_sp_debug_log_add(CONCAT('Event_Trigger is running for ',NEW.object_id,' ',NEW.event_id),vDebugTrace);
    SELECT COUNT(event_script_id) INTO vEventCount FROM osae_v_object_event_script WHERE object_id=NEW.object_id AND event_id=NEW.event_id AND event_script IS NOT NULL and event_script<>'';
    IF vEventCount = 1 THEN
        SELECT event_script_id,event_name,object_name INTO vEventScriptID,vEventName,vObjectName FROM osae_v_object_event_script WHERE object_id=NEW.object_id AND event_id=NEW.event_id AND event_script IS NOT NULL and event_script<>'' LIMIT 1;
        SELECT property_value INTO scriptProc FROM osae_v_object_property WHERE object_name='SYSTEM' AND property_name='Script Processor';
        CALL osae_sp_method_queue_add (scriptProc,'EVENT SCRIPT',vEventScriptID,'','SYSTEM',vDebugTrace);
    END IF; 
END
$$

DELIMITER $$

DROP PROCEDURE IF EXISTS osae_sp_run_scheduled_methods$$
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_run_scheduled_methods()
BEGIN
DECLARE iSCHEDULEID INT;
DECLARE iOBJECTID INT DEFAULT 0;
DECLARE vObjectName VARCHAR(400) DEFAULT '';
DECLARE iMETHODID INT DEFAULT 0;
DECLARE vMethodName VARCHAR(400);
DECLARE vPARAM1 VARCHAR(200);
DECLARE vPARAM2 VARCHAR(200);
DECLARE iPATTERNID INT DEFAULT 0;
DECLARE vPATTERN VARCHAR(200);
DECLARE done INT DEFAULT 0;  
DECLARE scriptProc VARCHAR(200);
DECLARE cur1 CURSOR FOR SELECT schedule_ID,COALESCE(object_name,''),method_name,parameter_1,parameter_2,pattern_id FROM osae_v_schedule_queue WHERE queue_datetime < NOW();
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
    CALL osae_sp_debug_log_add('Starting to run osae_sp_run_scheduled_methods','osae_sp_run_scheduled_methods');
    OPEN cur1; 
    Loop_Tag: LOOP
        FETCH cur1 INTO iSCHEDULEID,vObjectName,vMethodName,vPARAM1,vPARAM2,iPATTERNID;
        IF done THEN
            Leave Loop_Tag;
        END IF;
            CALL osae_sp_debug_log_add(CONCAT('Found Scheduled Method to run:',iSCHEDULEID,'  Object=',vObjectName,'   PatternID=',COALESCE(iPATTERNID,0)),'osae_sp_run_scheduled_methods');
            DELETE FROM osae_schedule_queue WHERE schedule_ID=iSCHEDULEID; 
            IF vObjectName != '' THEN
                CALL osae_sp_method_queue_add(vObjectName,vMethodName,vPARAM1,vPARAM2,'SYSTEM','osae_sp_run_scheduled_methods');
            ELSEIF iPATTERNID != 0 THEN
                SELECT pattern INTO vPATTERN FROM osae_pattern WHERE pattern_id=iPATTERNID;
                SELECT property_value INTO scriptProc FROM osae_v_object_property WHERE object_name='SYSTEM' AND property_name='Script Processor';
                CALL osae_sp_method_queue_add(scriptProc,'NAMED SCRIPT',vPATTERN,'SYSTEM','SYSTEM','osae_sp_run_scheduled_methods');
            END IF;         
        END LOOP;
    CLOSE cur1;
    CALL osae_sp_process_recurring();   
END
$$


delimiter $$
CREATE TABLE `osae_images` (
  `image_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `image_data` longblob NOT NULL,
  `image_name` varchar(45) NOT NULL,
  `image_type` varchar(4) NOT NULL,
  PRIMARY KEY (`image_id`)
) ENGINE=InnoDB AUTO_INCREMENT=257 DEFAULT CHARSET=latin1
  $$

delimiter $$

CREATE DEFINER = 'osae'@'%'
PROCEDURE `osae_sp_image_add`(
IN  pimage_data         LONGBLOB,
IN  pimage_name			VARCHAR(45),
IN  pimage_type			VARCHAR(4)
)
BEGIN

	INSERT INTO `osae`.`osae_images`
		(`image_data`,		
		`image_name`,
		`image_type`)
		VALUES
		(
		pimage_data,		
		pimage_name,
		pimage_type
		);
END$$

delimiter $$
DROP VIEW IF EXISTS osae_v_screen_object CASCADE$$
CREATE OR REPLACE DEFINER = 'osae'@'%' VIEW osae_v_screen_object
AS
SELECT `so`.`screen_object_id` AS `screen_object_id`
     , `so`.`screen_id` AS `screen_id`
     , `so`.`object_id` AS `object_id`
     , `so`.`control_id` AS `control_id`
     , `screen`.`object_name` AS `screen_name`
     , `control`.`object_name` AS `control_name`
     , `o`.`object_name` AS `object_name`
     , `controltype`.`object_type` AS `control_type`
     , `controlbasetype`.`object_type` AS `control_base_type`
     , `o`.`last_updated` AS `last_updated`
     , `o`.`last_state_change` AS `last_state_change`
     , timestampdiff(MINUTE, `o`.`last_state_change`, now()) AS `time_in_state`
     , `state`.`state_name`
FROM
  (((((`osae_screen_object` `so`
JOIN `osae_object` `screen`
ON ((`screen`.`object_id` = `so`.`screen_id`)))
JOIN `osae_object` `control`
ON ((`control`.`object_id` = `so`.`control_id`)))
JOIN `osae_object_type` `controltype`
ON ((`controltype`.`object_type_id` = `control`.`object_type_id`)))
JOIN `osae_object_type` `controlbasetype`
ON ((`controlbasetype`.`object_type_id` = `controltype`.`base_type_id`)))
JOIN `osae_object` `o`
ON ((`o`.`object_id` = `so`.`object_id`))
JOIN `osae_object_type_state` `state`
ON ((`o`.`state_id` = `state`.`state_id`)))
$$

delimiter $$
CREATE DEFINER = 'osae'@'%'
PROCEDURE `osae_sp_image_delete`(
IN pimage_id INT
)
BEGIN
	DELETE FROM `osae`.`osae_images`
	WHERE image_id = pimage_id;
END$$


-- Fix for scheduler running named scripts
DELIMITER $$
DROP PROCEDURE IF EXISTS osae_sp_process_recurring$$

CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_process_recurring()
BEGIN
DECLARE iRECURRINGID INT;
DECLARE vOBJECTNAME VARCHAR(400) DEFAULT '';
DECLARE vMETHODNAME VARCHAR(400) DEFAULT '';
DECLARE vPARAM1 VARCHAR(200);
DECLARE vPARAM2 VARCHAR(200);
DECLARE vPATTERN VARCHAR(200);
DECLARE iPATTERNID INT;
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
DECLARE cur1 CURSOR FOR SELECT recurring_id,interval_unit,recurring_time,recurring_minutes,recurring_date,recurring_day,object_name,method_name,parameter_1,parameter_2,pattern_id, pattern, sunday,monday,tuesday,wednesday,thursday,friday,saturday FROM osae_v_schedule_recurring;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
    OPEN cur1; 
    Loop_Tag: LOOP
        IF done THEN
            Leave Loop_Tag;
        END IF;
        FETCH cur1 INTO iRECURRINGID,cINTERVAL,dRECURRINGTIME,iRECURRINGMINUTES,dRECURRINGDATE,dRECURRINGDAY,vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,iPATTERNID,vPATTERN,cSUNDAY,cMONDAY,cTUESDAY,cWEDNESDAY,cTHURSDAY,cFRIDAY,cSATURDAY;
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
                    CALL osae_sp_schedule_queue_add (CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);
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
                    CALL osae_sp_schedule_queue_add (ADDTIME(NOW(),SEC_TO_TIME(iRECURRINGMINUTES * 60)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);
                   -- CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',ADDTIME(NOW(),SEC_TO_TIME(iRECURRINGMINUTES * 60))),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);
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
                    CALL osae_sp_schedule_queue_add (CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);
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
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);
                    END IF; 
                END IF; 
                IF dCURDAYOFWEEK = 2 AND cMONDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);          
                    END IF; 
                END IF; 
                IF dCURDAYOFWEEK = 3 AND cTUESDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);
                    END IF; 
                END IF;                 
                IF dCURDAYOFWEEK = 4 AND cWEDNESDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);   
                    END IF; 
                END IF;  
                IF dCURDAYOFWEEK = 5 AND cTHURSDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);                    
                    END IF; 
                END IF;
                IF dCURDAYOFWEEK = 6 AND cFRIDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);                    
                    END IF; 
                END IF;
                IF dCURDAYOFWEEK = 7 AND cSATURDAY = 1 THEN                
                    SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                    IF iMATCHES = 0 THEN
                        CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);                    
                   END IF; 
                END IF;                                                                           
            END IF;         
        END IF;
     END LOOP;
    CLOSE cur1;   
END

$$



delimiter ;

alter table osae_object_property modify property_value VARCHAR(4000) DEFAULT NULL ; 

-- Update Images to have consistant Path, Removeing "." from ".\"
update osae_object_property set property_value = replace(property_value, '.\\', '\\') where property_value like '.\\\\%';

CALL osae_sp_object_type_state_update('OPENED','ON','Opened','X10 DS10A');
CALL osae_sp_object_type_state_update('CLOSED','OFF','Closed','X10 DS10A');
CALL osae_sp_object_type_event_update('OPENED','ON','Opened','X10 DS10A');
CALL osae_sp_object_type_event_update('CLOSED','OFF','Closed','X10 DS10A');

CALL osae_sp_object_type_property_add ('Script Processor','String','','SYSTEM',0);
CALL osae_sp_object_property_set('SYSTEM','Script Processor','Script Processor', '', '');

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


-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.4.0', '', '');