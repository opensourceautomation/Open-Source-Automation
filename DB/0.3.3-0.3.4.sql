
USE osae;

-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.3.4', '', '');

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