
-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.3.7', '', '');

DELIMITER $$

DROP PROCEDURE osae_sp_run_scheduled_methods$$
CREATE DEFINER=`osae`@`%` PROCEDURE `osae_sp_run_scheduled_methods`()
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
                CALL osae_sp_method_queue_add('SCRIPT PROCESSOR','NAMED SCRIPT',vPATTERN,'SYSTEM','SYSTEM','osae_sp_run_scheduled_methods');
            END IF;         
        END LOOP;
    CLOSE cur1;
    CALL osae_sp_process_recurring();   
END
$$

DROP PROCEDURE osae_sp_process_recurring$$
CREATE DEFINER=`osae`@`%` PROCEDURE `osae_sp_process_recurring`()
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
                IF dCURDATETIME > CONCAT(dCURDATE,' ',ADDTIME(NOW(),SEC_TO_TIME(iRECURRINGMINUTES * 60))) THEN
                    SET dCURDAYOFWEEK = dCURDAYOFWEEK + 1;
                    SET dCURDATE=DATE_ADD(CURDATE(),INTERVAL 1 DAY);
                END IF; 
                CALL osae_sp_debug_log_add(CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),'SYSTEM'); 
                SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                IF iMATCHES = 0 THEN
                    CALL osae_sp_schedule_queue_add (CONCAT(dCURDATE,' ',ADDTIME(NOW(),SEC_TO_TIME(iRECURRINGMINUTES * 60))),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vPATTERN,iRECURRINGID);
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



DROP PROCEDURE osae_sp_schedule_recurring_add$$
CREATE DEFINER=`osae`@`%` PROCEDURE `osae_sp_schedule_recurring_add`(
  IN  pschedule_name     varchar(400),
  IN  pobject            varchar(400),
  IN  pmethod            varchar(400),
  IN  pparameter1        varchar(2000),
  IN  pparameter2        varchar(2000),
  IN  ppattern           varchar(400),
  IN  precurringtime     time,
  IN  psunday            tinyint(1),
  IN  pmonday            tinyint(1),
  IN  ptuesday           tinyint(1),
  IN  pwednesday         tinyint(1),
  IN  pthursday          tinyint(1),
  IN  pfriday            tinyint(1),
  IN  psaturday          tinyint(1),
  IN  pinterval          varchar(10),
  IN  precurringminutes  int(8),
  IN  precurringday      int(4),
  IN  precurringdate     date
)
BEGIN
DECLARE vObjectID INT DEFAULT NULL;
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vPatternID INT DEFAULT NULL;
    SELECT pattern_id INTO vPatternID FROM osae_pattern WHERE pattern=ppattern;
    SELECT object_id,method_id INTO vObjectID,vMethodID FROM osae_v_object_method WHERE UPPER(object_name)=UPPER(pobject) AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    INSERT INTO osae_schedule_recurring (schedule_name,object_id,method_id,parameter_1,parameter_2,pattern_id,interval_unit,recurring_time,recurring_minutes,recurring_day,recurring_date,sunday,monday,tuesday,wednesday,thursday,friday,saturday) VALUES(pschedule_name,vObjectID,vMethodID,pparameter1,pparameter2,vPatternID,pinterval,precurringtime,precurringminutes,precurringday,precurringdate,psunday,pmonday,ptuesday,pwednesday,pthursday,pfriday,psaturday);
END
$$


DROP PROCEDURE osae_sp_schedule_recurring_update$$
CREATE DEFINER=`osae`@`%` PROCEDURE `osae_sp_schedule_recurring_update`(
  IN  poldschedulename   varchar(400),
  IN  pnewschedulename   varchar(400),
  IN  pobject            varchar(400),
  IN  pmethod            varchar(400),
  IN  pparameter1        varchar(2000),
  IN  pparameter2        varchar(2000),
  IN  ppattern           varchar(400),
  IN  precurringtime     time,
  IN  psunday            tinyint(1),
  IN  pmonday            tinyint(1),
  IN  ptuesday           tinyint(1),
  IN  pwednesday         tinyint(1),
  IN  pthursday          tinyint(1),
  IN  pfriday            tinyint(1),
  IN  psaturday          tinyint(1),
  IN  pinterval          varchar(10),
  IN  precurringminutes  int(8),
  IN  precurringday      int(4),
  IN  pprecurringdate    date
)
BEGIN
DECLARE vObjectID INT DEFAULT NULL;
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vPatternID INT DEFAULT NULL;
    SELECT pattern_id INTO vPatternID FROM osae_pattern WHERE pattern=ppattern;
    SELECT object_id,method_id INTO vObjectID,vMethodID FROM osae_v_object_method WHERE UPPER(object_name)=UPPER(pobject) AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    UPDATE osae_schedule_recurring SET schedule_name=pnewschedulename,object_id=vObjectID,method_id=vMethodID,parameter_1=pparameter1,parameter_2=pparameter2,pattern_id=vPatternID,interval_unit=pinterval,recurring_time=precurringtime,recurring_minutes=precurringminutes,recurring_day=precurringday,recurring_date=pprecurringdate,sunday=psunday,monday=pmonday,tuesday=ptuesday,wednesday=pwednesday,thursday=pthursday,friday=pfriday,saturday=psaturday WHERE schedule_name=poldschedulename;
END
$$

DROP PROCEDURE osae_sp_event_log_add$$
CREATE DEFINER = `osae`@`%`
PROCEDURE osae_sp_event_log_add(
  IN  pobject      varchar(200),
  IN  pevent       varchar(200),
  IN  pfromobject  varchar(200),
  IN  pdebuginfo   varchar(1000)
)
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
            CALL osae_sp_debug_log_add(CONCAT('Event_Log_add (',pobject,' ',pevent,') ',pfromobject,' vFromObjectID ',COALESCE(vFromObjectID,'NULL')),vDebugTrace);
            SELECT event_id INTO vEventID FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pevent);
            INSERT INTO osae_event_log (object_id,event_id,from_object_id,debug_trace) VALUES(vObjectID,vEventID,vFromObjectID,vDebugTrace);
        END IF;
    END IF; 
END
$$