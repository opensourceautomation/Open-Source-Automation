
DELIMITER $$

--
-- Alter procedure "osae_sp_pattern_scripts_get"
--
DROP PROCEDURE osae_sp_pattern_scripts_get$$
CREATE PROCEDURE osae_sp_pattern_scripts_get(IN ppattern VARCHAR(255))
BEGIN
  SELECT s.script_id
    FROM osae_pattern_script s
    INNER JOIN osae_pattern p ON p.pattern_id = s.pattern_id
    WHERE p.pattern = ppattern;
END
$$

ALTER TABLE osae_schedule_recurring
  ADD COLUMN active TINYINT(4) DEFAULT 1 AFTER schedule_name$$

DROP PROCEDURE osae_sp_pattern_scripts_get$$
CREATE PROCEDURE osae_sp_pattern_scripts_get(IN ppattern VARCHAR(255))
BEGIN
  SELECT s.script_id
    FROM osae_pattern_script s
    INNER JOIN osae_pattern p ON p.pattern_id = s.pattern_id
    WHERE p.pattern = ppattern;
END
$$

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
DECLARE cur1 CURSOR FOR SELECT recurring_id,interval_unit,recurring_time,recurring_minutes,recurring_date,recurring_day,object_name,method_name,parameter_1,parameter_2,script_id, script_name, sunday,monday,tuesday,wednesday,thursday,friday,saturday FROM osae_v_schedule_recurring WHERE COALESCE(active,1)=1;
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

DROP PROCEDURE IF EXISTS osae_sp_schedule_recurring_activate$$
CREATE PROCEDURE osae_sp_schedule_recurring_activate(
  IN pschedulename VARCHAR(255), IN pactive bit
)
BEGIN
  UPDATE osae_schedule_recurring
    set active = pactive
    WHERE schedule_name = pschedulename;
END
$$

DROP PROCEDURE osae_sp_schedule_recurring_add$$
CREATE PROCEDURE osae_sp_schedule_recurring_add(IN pschedule_name VARCHAR(400), IN pobject VARCHAR(400), IN pmethod VARCHAR(400), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000), IN pscript VARCHAR(400), IN precurringtime TIME, IN psunday TINYINT(1), IN pmonday TINYINT(1), IN ptuesday TINYINT(1), IN pwednesday TINYINT(1), IN pthursday TINYINT(1), IN pfriday TINYINT(1), IN psaturday TINYINT(1), IN pinterval VARCHAR(10), IN precurringminutes INT(8), IN precurringday INT(4), IN precurringdate DATE, IN pactive TINYINT)
BEGIN
DECLARE vObjectID INT DEFAULT NULL;
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vScriptID INT DEFAULT NULL;
    SELECT script_id INTO vScriptID FROM osae_script WHERE script_name=pscript;
    SELECT object_id,method_id INTO vObjectID,vMethodID FROM osae_v_object_method WHERE UPPER(object_name)=UPPER(pobject) AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    INSERT INTO osae_schedule_recurring (schedule_name,object_id,method_id,parameter_1,parameter_2,script_id,interval_unit,recurring_time,recurring_minutes,recurring_day,recurring_date,sunday,monday,tuesday,wednesday,thursday,friday,saturday,active) VALUES(pschedule_name,vObjectID,vMethodID,pparameter1,pparameter2,vScriptID,pinterval,precurringtime,precurringminutes,precurringday,precurringdate,psunday,pmonday,ptuesday,pwednesday,pthursday,pfriday,psaturday,pactive);
END
$$

DROP PROCEDURE osae_sp_schedule_recurring_update$$
CREATE PROCEDURE osae_sp_schedule_recurring_update(IN poldschedulename VARCHAR(400), IN pnewschedulename VARCHAR(400), IN pobject VARCHAR(400), IN pmethod VARCHAR(400), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000), IN pscript VARCHAR(400), IN precurringtime TIME, IN psunday TINYINT(1), IN pmonday TINYINT(1), IN ptuesday TINYINT(1), IN pwednesday TINYINT(1), IN pthursday TINYINT(1), IN pfriday TINYINT(1), IN psaturday TINYINT(1), IN pinterval VARCHAR(10), IN precurringminutes INT(8), IN precurringday INT(4), IN pprecurringdate DATE, IN pactive TINYINT)
BEGIN
DECLARE vObjectID INT DEFAULT NULL;
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vScriptID INT DEFAULT NULL;
    SELECT script_id INTO vScriptID FROM osae_script WHERE script_name=pscript;
    SELECT object_id,method_id INTO vObjectID,vMethodID FROM osae_v_object_method WHERE UPPER(object_name)=UPPER(pobject) AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    UPDATE osae_schedule_recurring SET schedule_name=pnewschedulename,object_id=vObjectID,method_id=vMethodID,parameter_1=pparameter1,parameter_2=pparameter2,script_id=vScriptID,interval_unit=pinterval,recurring_time=precurringtime,recurring_minutes=precurringminutes,recurring_day=precurringday,recurring_date=pprecurringdate,sunday=psunday,monday=pmonday,tuesday=ptuesday,wednesday=pwednesday,thursday=pthursday,friday=pfriday,saturday=psaturday,active=pactive WHERE schedule_name=poldschedulename;
END
$$

DROP TRIGGER IF EXISTS osae_tr_object_before_update$$
CREATE TRIGGER osae_tr_object_before_update
	BEFORE UPDATE
	ON osae_object
	FOR EACH ROW
BEGIN
DECLARE vState VARCHAR(50);
DECLARE vEventCount INT;
DECLARE vHideRedundantRvents INT;
DECLARE vEvent VARCHAR(200);
    IF OLD.object_type_id <> NEW.object_type_id THEN
        DELETE FROM osae_object_property WHERE object_id=OLD.object_id;
        INSERT INTO osae_object_property (object_id,object_type_property_id,property_value) SELECT OLD.object_id, property_id, property_default FROM osae_object_type_property WHERE object_type_id=NEW.object_type_id;
        DELETE FROM osae_object_state_history WHERE object_id=OLD.object_id;
        INSERT INTO osae_object_state_history (object_id,state_id) SELECT OLD.object_id, state_id FROM osae_v_object_state WHERE object_type_id=NEW.object_type_id;        
        DELETE FROM osae_object_property WHERE object_id=OLD.object_id;
        INSERT INTO osae_object_property (object_id,object_type_property_id,property_value) SELECT OLD.object_id, property_id, property_default FROM osae_object_type_property WHERE object_type_id=NEW.object_type_id;
        DELETE FROM osae_object_event_script WHERE object_id=OLD.object_id;
    END IF;
    IF OLD.state_id <> NEW.state_id THEN
        SET NEW.last_state_change=NOW();
        UPDATE osae_object_state_history SET times_this_hour=times_this_hour + 1, times_this_day=times_this_day + 1,times_this_month=times_this_month+1,times_this_year=times_this_year+1,times_ever=times_ever + 1 WHERE object_id=OLD.object_id AND state_id=NEW.state_id;
        INSERT INTO osae_object_state_change_history (object_id, state_id) VALUES (OLD.object_id, NEW.state_id);
    END IF;    
END
$$


DELIMITER ;

ALTER TABLE osae_event_log
  ADD INDEX IDX_osae_event_log_log_time (log_time);

CREATE TABLE osae_object_state_change_history (
  history_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  history_timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  object_id INT(10) UNSIGNED NOT NULL,
  state_id INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (history_id)
)
ENGINE = INNODB
CHARACTER SET utf8
COLLATE utf8_general_ci;


CREATE
VIEW osae_v_object_state_change_history
AS
SELECT
  `osae_object`.`object_id` AS `object_id`,
  `osae_object`.`object_name` AS `object_name`,
  `osae_object`.`object_description` AS `object_description`,
  `osae_object`.`address` AS `address`,
  `osae_object_type_state`.`state_name` AS `state_name`,
  `osae_object_type_state`.`state_label` AS `state_label`,
  `osae_object_type_state`.`state_id` AS `state_id`,
  `osae_object_state_change_history`.`history_timestamp` AS `history_timestamp`
FROM ((`osae_object`
  JOIN `osae_object_state_change_history`
    ON ((`osae_object`.`object_id` = `osae_object_state_change_history`.`object_id`)))
  JOIN `osae_object_type_state`
    ON (((`osae_object_state_change_history`.`state_id` = `osae_object_type_state`.`state_id`) AND (`osae_object`.`object_type_id` = `osae_object_type_state`.`object_type_id`))));



CREATE OR REPLACE 
VIEW osae_v_schedule_recurring
AS
	select `osae_schedule_recurring`.`recurring_id` AS `recurring_id`,`osae_schedule_recurring`.`schedule_name` AS `schedule_name`,`osae_schedule_recurring`.`parameter_1` AS `parameter_1`,`osae_schedule_recurring`.`parameter_2` AS `parameter_2`,`osae_schedule_recurring`.`recurring_time` AS `recurring_time`,`osae_schedule_recurring`.`monday` AS `monday`,`osae_schedule_recurring`.`tuesday` AS `tuesday`,`osae_schedule_recurring`.`wednesday` AS `wednesday`,`osae_schedule_recurring`.`thursday` AS `thursday`,`osae_schedule_recurring`.`friday` AS `friday`,`osae_schedule_recurring`.`saturday` AS `saturday`,`osae_schedule_recurring`.`sunday` AS `sunday`,`osae_schedule_recurring`.`interval_unit` AS `interval_unit`,`osae_schedule_recurring`.`recurring_minutes` AS `recurring_minutes`,`osae_schedule_recurring`.`recurring_day` AS `recurring_day`,`osae_schedule_recurring`.`recurring_date` AS `recurring_date`,`osae_schedule_recurring`.`active` AS `active`,`osae_script`.`script_id` AS `script_id`,`osae_script`.`script_name` AS `script_name`,`osae_script`.`script` AS `script`,`osae_object_type_method`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_object_type_method`.`object_type_id` AS `object_type_id`,`osae_object_type_method`.`param_1_label` AS `param_1_label`,`osae_object_type_method`.`param_2_label` AS `param_2_label`,`osae_object_type_method`.`param_1_default` AS `param_1_default`,`osae_object_type_method`.`param_2_default` AS `param_2_default`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id` from (((`osae_schedule_recurring` left join `osae_object` on((`osae_schedule_recurring`.`object_id` = `osae_object`.`object_id`))) left join `osae_script` on((`osae_schedule_recurring`.`script_id` = `osae_script`.`script_id`))) left join `osae_object_type_method` on((`osae_schedule_recurring`.`method_id` = `osae_object_type_method`.`method_id`)));


-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.4.3', '', '');
