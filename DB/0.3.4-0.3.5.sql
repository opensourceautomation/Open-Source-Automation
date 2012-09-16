
USE osae;

-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.3.5', '', '');
CALL osae_sp_object_property_set('SYSTEM', 'Debug', 'FALSE', '', '');

CALL osae_sp_object_type_add ('SCRIPT PROCESSOR','Generic Plugin Class','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','SCRIPT PROCESSOR');
CALL osae_sp_object_type_state_add ('OFF','Stopped','SCRIPT PROCESSOR');
CALL osae_sp_object_type_event_add ('ON','Started','SCRIPT PROCESSOR');
CALL osae_sp_object_type_event_add ('OFF','Stopped','SCRIPT PROCESSOR');
CALL osae_sp_object_type_method_add ('ON','Start','SCRIPT PROCESSOR','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','SCRIPT PROCESSOR','','','','');
CALL osae_sp_object_type_method_add ('EVENT SCRIPT','Run Event Script','SCRIPT PROCESSOR','Event ID','','','');
CALL osae_sp_object_type_method_add ('NAMED SCRIPT','Run Named Script','SCRIPT PROCESSOR','Script Name','','','');
CALL osae_sp_object_type_property_add ('System Plugin','Boolean','TRUE','SCRIPT PROCESSOR',0);
CALL osae_sp_object_type_method_delete ('PATTERN SCRIPT','SCRIPT PROCESSOR');

CALL osae_sp_object_type_update('THING', 'THING', 'Core Type: Things', 'SYSTEM', 'THING', 0, 1, 0, 1);
CALL osae_sp_object_type_property_add ('OFF TIMER','Integer','','THING',0);
CALL osae_sp_object_type_property_add ('OFF TIMER','Integer','','PERSON',0);

CALL osae_sp_object_type_property_add ('Prune Logs','Boolean','TRUE','SYSTEM',0);
CALL osae_sp_object_property_set ('SYSTEM','Prune Logs','TRUE','','');

CALL osae_sp_object_type_property_add ('Poll Interval','Integer','30','NETWORK MONITOR',0);
CALL osae_sp_object_property_set ('Network Monitor','Poll Interval','30','','');
CALL osae_sp_object_property_set ('Network Monitor','Poll Interval','30','','');

CALL osae_sp_object_type_add ('SPEECH','Generic Plugin Class','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','SPEECH');
CALL osae_sp_object_type_state_add ('OFF','Stopped','SPEECH');
CALL osae_sp_object_type_event_add ('ON','Started','SPEECH');
CALL osae_sp_object_type_event_add ('OFF','Stopped','SPEECH');
CALL osae_sp_object_type_method_add ('ON','On','SPEECH','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','SPEECH','','','','');
CALL osae_sp_object_type_method_add ('SPEAK','Say','SPEECH','Message','','Hello','');
CALL osae_sp_object_type_method_add ('SPEAKFROM','Say From List','SPEECH','Object Name','Property Name','Speech List','Greetings');
CALL osae_sp_object_type_method_add ('PLAY','Play','SPEECH','File','','','');
CALL osae_sp_object_type_method_add ('PLAYFROM','Play From List','SPEECH','List','','','');
CALL osae_sp_object_type_method_add ('STOP','Stop','SPEECH','','','','');
CALL osae_sp_object_type_method_add ('PAUSE','Pause','SPEECH','','','','');
CALL osae_sp_object_type_method_add ('SETVOICE','Set Voice','SPEECH','Voice','','Anna','');
CALL osae_sp_object_type_method_add ('SETTTSRATE','Set TTS Rate','SPEECH','Rate','','100','');
CALL osae_sp_object_type_method_add ('SETTTSVOLUME','Set TTS Volume','SPEECH','Volume','','100','');
CALL osae_sp_object_type_property_add ('Computer Name','String','','SPEECH',0);
CALL osae_sp_object_type_property_add ('Voice','String','','SPEECH',0);
CALL osae_sp_object_type_property_add ('Voices','List','','SPEECH',0);
CALL osae_sp_object_type_property_add ('System Plugin','Boolean','FALSE','SPEECH',0);
CALL osae_sp_object_type_property_add ('TTS Rate','Integer','','SPEECH',0);
CALL osae_sp_object_type_property_add ('TTS Volume','Integer','','SPEECH',0);

CALL osae_sp_object_type_add ('VR CLIENT','Generic Plugin Class','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','VR CLIENT');
CALL osae_sp_object_type_state_add ('OFF','Stopped','VR CLIENT');
CALL osae_sp_object_type_event_add ('ON','Started','VR CLIENT');
CALL osae_sp_object_type_event_add ('OFF','Stopped','VR CLIENT');
CALL osae_sp_object_type_method_add ('ON','On','VR CLIENT','','','','');
CALL osae_sp_object_type_method_add ('OFF','Off','VR CLIENT','','','','');
CALL osae_sp_object_type_method_add ('MUTEVR','Mute the Microphone','VR CLIENT','','','','');
CALL osae_sp_object_type_property_add ('Computer Name','String','','VR CLIENT',0);
CALL osae_sp_object_type_property_add ('VR Input Muted','Boolean','TRUE','VR CLIENT',0);
CALL osae_sp_object_type_property_add ('Voice','String','','VR CLIENT',1);
CALL osae_sp_object_type_property_add ('Voices','List','','VR CLIENT',0);
CALL osae_sp_object_type_property_add ('VR Enabled','Boolean','FALSE','VR CLIENT',0);
CALL osae_sp_object_type_property_add ('VR Sleep Phrase','String','Thank You','VR CLIENT',0);
CALL osae_sp_object_type_property_add ('VR Wake Phrase','String','Computer','VR CLIENT',0);


DELIMITER $$

DROP PROCEDURE IF EXISTS osae_sp_object_event_script_update$$
CREATE DEFINER = 'root'@'localhost'
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
            UPDATE osae_object_event_script SET event_script=ptext WHERE object_id=vObjectID AND event_id=vEventID;
         -- CALL osae_sp_debug_log_add(CONCAT('Updated ',vObjectID,' - ',vEventID,ptext),'');  
        END IF;
    END IF; 
END$$


DROP TRIGGER IF EXISTS osae_tr_object_before_update$$
CREATE 
	DEFINER = 'root'@'localhost'
TRIGGER osae_tr_object_before_update
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
    END IF;    
END$$


DROP PROCEDURE IF EXISTS osae_sp_object_add$$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE osae_sp_object_add(IN  pname        VARCHAR(200),
                             IN  pdescription VARCHAR(200),
                             IN  pobjecttype  VARCHAR(200),
                             IN  paddress     VARCHAR(200),
                             IN  pcontainer   VARCHAR(200),
                             IN  penabled     TINYINT(1),
                             OUT results      INTEGER
                             )
BEGIN
  DECLARE vObjectCount     INT;
  DECLARE vObjectTypeCount INT;
  DECLARE iContainer       INT;
  DECLARE iObjectID        INT;
  DECLARE vObjectTypeID    INT;
  DECLARE vContainerCount  INT;
  DECLARE vContainerID     INT DEFAULT NULL;
  SET results = 1;

  SELECT count(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type = pobjecttype;
  IF vObjectTypeCount > 0 THEN
    SELECT object_type_id, container INTO vObjectTypeID, iContainer FROM osae_object_type WHERE  object_type = pobjecttype;
    SELECT count(object_id) INTO vContainerCount FROM osae_v_object WHERE object_name = pcontainer AND container = 1;
    IF vContainerCount = 1 THEN
      SELECT object_id INTO vContainerID FROM osae_v_object WHERE object_name = pcontainer AND container = 1;
    END IF;
    SELECT count(object_id) INTO vObjectCount FROM osae_object WHERE upper(object_name) = upper(pname) OR (upper(address) = upper(paddress) AND address IS NOT NULL AND address <> '');
    IF vObjectCount = 0 THEN
      INSERT INTO osae_object (object_name, object_description, object_type_id, address, container_id, enabled) VALUES (pname, pdescription, vObjectTypeID, paddress, vContainerID, penabled);
    ELSE
      CALL osae_sp_debug_log_add(concat('Object_Add Failed to Add ', pname, ' due to duplicate data.'), 'SYSTEM');
      SET results = 3;
    END IF;                                                                                 
    IF iContainer = 1 THEN
      SELECT object_id INTO iObjectID FROM osae_object WHERE object_name = pname;
      UPDATE osae_object SET  container_id = iObjectID WHERE object_id = iObjectID;
    END IF;
  ELSE
    SET results = 2;
  END IF;
END$$

