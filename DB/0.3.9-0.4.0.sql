
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

CREATE DEFINER = 'osae'@'%'
PROCEDURE `osae_sp_image_delete`(
IN pimage_id INT
)
BEGIN
	DELETE FROM `osae`.`osae_images`
	WHERE image_id = pimage_id;
END$$


delimiter ;

alter table osae_object_property modify property_value VARCHAR(4000) DEFAULT NULL ; 

-- Update Images to have consistant Path, Removeing "." from ".\"
update osae_object_property set property_value = replace(property_value, '.\\', '\\') where property_value like '.\\\\%';

CALL osae_sp_object_type_property_add ('Script Processor','String','','SYSTEM',0);
CALL osae_sp_object_property_set('SYSTEM','Script Processor','Script Processor', '', '');

CALL osae_sp_object_type_add ('CONTROL CAMERA VIEWER','Control - IP Camera Viewer','','CONTROL',0,1,0,1);
CALL osae_sp_object_type_property_add ('X','Integer','','CONTROL CAMERA VIEWER',1);
CALL osae_sp_object_type_property_add ('Y','Integer','','CONTROL CAMERA VIEWER',1);
CALL osae_sp_object_type_property_add ('ZOrder','Integer','','CONTROL CAMERA VIEWER',1);
CALL osae_sp_object_type_property_add ('Object Name','String','','CONTROL CAMERA VIEWER',0);


-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.4.0', '', '');