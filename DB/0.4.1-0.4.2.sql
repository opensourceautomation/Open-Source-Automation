
DELIMITER $$

--
-- Alter procedure "osae_sp_run_scheduled_methods"
--
DROP PROCEDURE osae_sp_run_scheduled_methods$$
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
            CALL osae_sp_debug_log_add(CONCAT('Found Schedule to run:',iSCHEDULEID,'  Object=',vObjectName,'   ScriptID=',COALESCE(iSCRIPTID,0)),'osae_sp_run_scheduled_methods');
            IF vObjectName <> '' THEN
                CALL osae_sp_debug_log_add(CONCAT('Adding method to queue:',' Object=',vObjectName,'   Method=',vMethodName),'osae_sp_run_scheduled_methods');
                CALL osae_sp_method_queue_add(vObjectName,vMethodName,vPARAM1,vPARAM2,'SYSTEM','osae_sp_run_scheduled_methods');
            ELSEIF iSCRIPTID <> 0 THEN
                SELECT script_processor_id INTO vSCRIPTPROCID FROM osae_script WHERE script_id=iSCRIPTID;
                SELECT script_processor_plugin_name INTO scriptProc FROM osae_script_processors WHERE script_processor_id=vSCRIPTPROCID;
                CALL osae_sp_debug_log_add(CONCAT('Adding method to queue:',' Object=',scriptProc,'   ScriptID=',iSCRIPTID),'osae_sp_run_scheduled_methods');
                CALL osae_sp_method_queue_add(scriptProc,'RUN SCRIPT',iSCRIPTID,'SYSTEM','SYSTEM','osae_sp_run_scheduled_methods');
            ELSE
                CALL osae_sp_debug_log_add(CONCAT('ERROR Adding method to queue:',' vObjectName=',vObjectName,'   ScriptID=',iSCRIPTID),'osae_sp_run_scheduled_methods');
            END IF;         
            DELETE FROM osae_schedule_queue WHERE schedule_ID=iSCHEDULEID; 
        END LOOP;
    CLOSE cur1;
    CALL osae_sp_process_recurring();   
END
$$

DELIMITER ;


-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.4.2', '', '');

CALL osae_sp_object_type_method_add ('RELOAD PLUGINS','Reload plugins','SERVICE','','','','');