
DELIMITER $$

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
                SELECT script_processor_name INTO scriptProc FROM osae_script_processors WHERE script_processor_id=vSCRIPTPROCID;
                CALL osae_sp_method_queue_add(scriptProc,'RUN SCRIPT',iSCRIPTID,'SYSTEM','SYSTEM','osae_sp_run_scheduled_methods');
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
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vScriptID INT DEFAULT NULL;
DECLARE vRecurringID INT DEFAULT NULL;
    SELECT script_id INTO vScriptID FROM osae_script WHERE UPPER(script_name)=UPPER(pscript);
    SELECT method_id INTO vMethodID FROM osae_v_object_method WHERE object_id = pobject AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    IF precurringid > 0 THEN
        SET vRecurringID = precurringid;
    END IF;
    INSERT INTO osae_schedule_queue (queue_datetime,object_id,method_id,parameter_1,parameter_2,script_id,recurring_id) VALUES(pscheduleddate,pobject,vMethodID,pparameter1,pparameter2,vScriptID,vRecurringID);
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
DECLARE vObjectTypeID INT;
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
    SELECT object_type_id INTO vObjectTypeID FROM osae_v_object WHERE object_id=NEW.object_id;
    SELECT COUNT(object_type_event_script_id) INTO vEventCount FROM osae_v_object_type_event_script WHERE object_type_id=vObjectTypeID AND event_id=NEW.event_id AND script IS NOT NULL and script<>'';
    IF vEventCount > 0 THEN
        WHILE vPrevScriptSeq != vScriptSeq DO
          SET vPrevScriptSeq = vScriptSeq; 
          SELECT script_sequence, s.script_id, sp.script_processor_plugin_name INTO vScriptSeq,vScriptID,scriptProc 
            FROM osae_v_object_type_event_script e
            INNER JOIN osae_script s ON e.script_id = s.script_id
            INNER JOIN osae_script_processors sp ON sp.script_processor_id = s.script_processor_id
            WHERE object_type_id=vObjectTypeID AND event_id=NEW.event_id AND s.script IS NOT NULL and s.script<>'' AND script_sequence > vScriptSeq
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
-- Enable foreign keys
--
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;


-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.4.1', '', '');