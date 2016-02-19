--
-- Disable foreign keys
--
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

USE osae;

--
-- Alter table "osae_images"
--
ALTER TABLE osae_images
  ADD COLUMN image_width INT(10) UNSIGNED DEFAULT 0 AFTER image_type,
  ADD COLUMN image_height INT(10) UNSIGNED DEFAULT 0 AFTER image_width,
  ADD COLUMN image_dpi INT(4) UNSIGNED DEFAULT 0 AFTER image_height;

--
-- Alter table "osae_log"
--
ALTER TABLE osae_log
  CHANGE COLUMN Thread Thread VARCHAR(255) DEFAULT NULL,
  CHANGE COLUMN Level Level VARCHAR(255) DEFAULT NULL,
  CHANGE COLUMN Logger Logger VARCHAR(255) DEFAULT NULL,
  CHANGE COLUMN Message Message VARCHAR(4000) DEFAULT NULL;

--
-- Alter table "osae_object"
--
ALTER TABLE osae_object
  CHANGE COLUMN last_updated last_updated DATETIME DEFAULT CURRENT_TIMESTAMP,
  CHANGE COLUMN last_state_change last_state_change DATETIME DEFAULT CURRENT_TIMESTAMP,
  ADD COLUMN min_trust_level INT(4) DEFAULT 30 AFTER last_state_change;

--
-- Alter table "osae_object_property"
--
ALTER TABLE osae_object_property
  CHANGE COLUMN last_updated last_updated DATETIME DEFAULT CURRENT_TIMESTAMP;

DELIMITER $$

--
-- Alter procedure "osae_sp_event_log_add"
--
DROP PROCEDURE osae_sp_event_log_add$$
CREATE PROCEDURE osae_sp_event_log_add(IN pobject VARCHAR(200), IN pevent VARCHAR(200), IN pfromobject VARCHAR(200), IN pdebuginfo VARCHAR(1000), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000))
BEGIN
DECLARE vObjectCount INT;
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vEventCount INT;
DECLARE vEventID INT;
DECLARE vFromObjectID INT DEFAULT NULL;
DECLARE vFromObjectCount INT;
DECLARE vDebugTrace VARCHAR(2000);
    SET vDebugTrace = CONCAT(COALESCE(pdebuginfo,''),' -> event_log_add');
    SELECT object_id INTO vFromObjectID FROM osae_object WHERE UPPER(object_name)=UPPER(pfromobject) OR UPPER(object_alias)=UPPER(pfromobject);
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject);
    IF vObjectCount > 0 THEN
        SELECT object_id,object_type_id INTO vObjectID,vObjectTypeID FROM osae_object WHERE UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject);
        SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pevent);
        IF vEventCount = 1 THEN  
            #CALL osae_sp_debug_log_add(CONCAT('Event_Log_add (',pobject,' ',pevent,') ',pfromobject,' vFromObjectID ',COALESCE(vFromObjectID,'NULL')),vDebugTrace);
            SELECT event_id INTO vEventID FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pevent);
            INSERT INTO osae_event_log (object_id,event_id,from_object_id,debug_trace,parameter_1,parameter_2) VALUES(vObjectID,vEventID,vFromObjectID,vDebugTrace,pparameter1,pparameter2);
        END IF;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_image_add"
--
DROP PROCEDURE osae_sp_image_add$$
CREATE PROCEDURE osae_sp_image_add(IN pimage_data LONGBLOB, IN pimage_name VARCHAR(45), IN pimage_type VARCHAR(4), IN pimage_width INT, IN pimage_height INT, IN pimage_dpi INT)
BEGIN
  DECLARE iid INT DEFAULT 0;
  SELECT image_id INTO iid FROM osae_images WHERE image_name = pimage_name;

  IF iid = 0 THEN
    INSERT INTO osae_images (`image_data`,`image_name`,`image_type`,`image_width`,`image_height`,`image_dpi`)
  		VALUES (pimage_data,pimage_name,pimage_type,pimage_width,pimage_height,pimage_dpi);
  
    SELECT LAST_INSERT_ID();
  ELSE
    SELECT iid;
  END IF;
END
$$

--
-- Alter procedure "osae_sp_method_queue_add"
--
DROP PROCEDURE osae_sp_method_queue_add$$
CREATE PROCEDURE osae_sp_method_queue_add(IN pobject varchar(200), IN pmethod varchar(200), IN pparameter1 varchar(1024), IN pparameter2 varchar(1024), IN pfromobject varchar(200), IN pdebuginfo varchar(1000))
BEGIN
DECLARE vObjectCount INT;
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vMethodCount INT;
DECLARE vMethodID INT;
DECLARE vFromObjectID INT DEFAULT NULL;
DECLARE vFromObjectCount INT;
DECLARE vFromObjectType VARCHAR(255);
DECLARE vFromObjectTrust INT;
DECLARE vMethodTrustCount INT;
DECLARE vMethodObjectTrust INT;
DECLARE vDebugTrace VARCHAR(1000) DEFAULT "";

DECLARE vMethod VARCHAR(200);

# Verify the FromObject has a trust_level and add one if not
SELECT COUNT(object_property_id) INTO vMethodTrustCount FROM osae_v_object_property WHERE property_name='Trust Level' AND (UPPER(object_name)=UPPER(pfromobject) OR UPPER(object_alias)=UPPER(pfromobject));
IF vMethodTrustCount = 0 THEN
    SELECT object_type INTO vFromObjectType FROM osae_v_object WHERE (UPPER(object_name)=UPPER(pfromobject) OR UPPER(object_alias)=UPPER(pfromobject));
    CALL osae_sp_object_type_property_add(vFromObjectType,'Trust Level','Integer','','50',0);
END IF;

SELECT property_value INTO vFromObjectTrust FROM osae_v_object_property WHERE property_name='Trust Level' AND (UPPER(object_name)=UPPER(pfromobject) OR UPPER(object_alias)=UPPER(pfromobject));

#SET max_sp_recursion_depth = 10;
SET vDebugTrace = CONCAT(pdebuginfo,' -> method_queue_add');
    SELECT object_id INTO vFromObjectID FROM osae_object WHERE UPPER(object_name)=UPPER(pfromobject) OR UPPER(object_alias)=UPPER(pfromobject);

    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject);
    IF vObjectCount = 1 THEN
        SELECT object_id,object_type_id,min_trust_level INTO vObjectID,vObjectTypeID,vMethodObjectTrust FROM osae_object WHERE UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject);
        SELECT COUNT(method_id) INTO vMethodCount FROM osae_object_type_method WHERE object_type_id=vObjectTypeID AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
        IF vMethodCount > 0 THEN       
            SELECT method_id INTO vMethodID FROM osae_object_type_method WHERE object_type_id=vObjectTypeID AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
            
                IF vFromObjectTrust < vMethodObjectTrust THEN
                    CALL osae_sp_debug_log_add(CONCAT(pfromobject, ' tried to control ',pobject,', but lacks sufficient trust'),'method_queue_add'); 
                ELSE
            
            
            # Check to see if an app owns this, or SYSTEM, if system owns we will have to handle events....
            #SELECT count(object_id) INTO vSystemCount FROM osae_v_object WHERE object_id=vObjectID AND owned_by='SYSTEM';
            #IF vSystemCount = 1 THEN
           #     SELECT object_name,object_type,base_type INTO vObjectName,vObjectType,vBaseType FROM osae_v_object WHERE object_id=vObjectID;
           #     SELECT method_name INTO vMethod FROM osae_object_type_method WHERE method_id=vMethodID;        
                # Here is the magic, if the Method Name matches a State, then the Method's Job is to Set that State, so look up the state
           #     SELECT count(state_name) INTO vStateCount FROM osae_v_object_type_state WHERE UPPER(state_name)=UPPER(vMethod) AND object_type=vObjectType; 
           #     IF vStateCount = 1 THEN   
          #          CALL osae_sp_object_state_set (vObjectName,vMethod,pfromobject,vDebugTrace);
           #     ELSE
            #      SELECT count(property_name) INTO vPropertyCount FROM osae_v_object_type_property WHERE UPPER(property_name)=UPPER(vMethod) AND object_type=vObjectType; 
            #      IF vPropertyCount = 1 THEN   
           #           CALL osae_sp_object_property_set (vObjectName,vMethod,pparameter1,pfromobject,vDebugTrace);
           #       END IF;
           #     END IF;                
         #   ELSE
                INSERT INTO osae_method_queue (object_id,method_id,parameter_1,parameter_2,from_object_id,debug_trace) VALUES(vObjectID,vMethodID,pparameter1,pparameter2,vFromObjectID,vDebugTrace);
          #  END IF;       
        END IF;
    END IF; 
    END IF;
END
$$

--
-- Alter procedure "osae_sp_object_add"
--
DROP PROCEDURE osae_sp_object_add$$
CREATE PROCEDURE osae_sp_object_add(IN pname VARCHAR(100), IN palias VARCHAR(100), IN pdescription VARCHAR(200), IN pobjecttype VARCHAR(200), IN paddress VARCHAR(200), IN pcontainer VARCHAR(200), IN pmintrustlevel INT, IN penabled TINYINT(1), OUT results INTEGER)
BEGIN
  DECLARE vAlias           VARCHAR(255) DEFAULT '';
  DECLARE vAliasCount      INT;  
  DECLARE vObjectCount     INT;
  DECLARE vObjectTypeCount INT;
  DECLARE iContainer       INT;
  DECLARE iObjectID        INT;
  DECLARE vObjectTypeID    INT;
  DECLARE vContainerCount  INT;
  DECLARE vContainerID     INT DEFAULT NULL;
  SET results = 1;
  #Massage the alias to make sure it is unique, or blank it out.
  IF pname != palias THEN
    IF palias != '' THEN
      SELECT count(object_id) INTO vAliasCount FROM osae_object WHERE object_name = palias OR object_alias = palias;
      IF vAliasCount > 0 THEN
        SET vAlias = '';
      ELSE
        SET vAlias = palias;
      END IF;
    END IF;
     
  END IF; 
  SELECT count(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type = pobjecttype;
  IF vObjectTypeCount > 0 THEN
    SELECT object_type_id, container INTO vObjectTypeID, iContainer FROM osae_object_type WHERE  object_type = pobjecttype;
    SELECT count(object_id) INTO vContainerCount FROM osae_v_object WHERE object_name = pcontainer AND container = 1;
    IF vContainerCount = 1 THEN
      SELECT object_id INTO vContainerID FROM osae_v_object WHERE object_name = pcontainer AND container = 1;
    END IF;
    SELECT count(object_id) INTO vObjectCount FROM osae_object WHERE upper(object_name) = upper(pname) OR upper(object_alias) = upper(pname) OR (upper(address) = upper(paddress) AND address IS NOT NULL AND address <> '');
    IF vObjectCount = 0 THEN
      INSERT INTO osae_object (object_name, object_alias, object_description, object_type_id, address, container_id, min_trust_level, enabled) VALUES (pname, vAlias, pdescription, vObjectTypeID, paddress, vContainerID, pmintrustlevel, penabled);
    ELSE
      CALL osae_sp_debug_log_add(concat('Object_Add Failed to Add ', pname, ' due to duplicate data.'), 'SYSTEM');
      SET results = 3;
    END IF;                                                                                 
    IF iContainer = 1 AND vContainerCount = 0 THEN
      SELECT object_id INTO iObjectID FROM osae_object WHERE object_name = pname;
      UPDATE osae_object SET container_id = iObjectID WHERE object_id = iObjectID;
    END IF;
  ELSE
    SET results = 2;
  END IF;
END
$$

--
-- Alter procedure "osae_sp_object_export"
--
DROP PROCEDURE osae_sp_object_export$$
CREATE PROCEDURE osae_sp_object_export(IN objectName VARCHAR(255))
BEGIN
  DECLARE vObjectName VARCHAR(255);
  DECLARE vDescription VARCHAR(200);
  DECLARE vObjectType VARCHAR(200);
  DECLARE vAddress VARCHAR(200);
  DECLARE vContainer VARCHAR(200);
  DECLARE vItemName VARCHAR(2000);
  DECLARE vItemLabel VARCHAR(2000);
  DECLARE vEnabled INT;
  DECLARE vProcResults INT;
  DECLARE vMinTrustLevel INT;

  DECLARE vPropertyName VARCHAR(200);
  DECLARE vPropertyValue VARCHAR(2000);

  DECLARE vResults TEXT;
  DECLARE v_finished BOOL; 

  DECLARE property_cursor CURSOR FOR SELECT property_name,COALESCE(property_value,'') AS property_value FROM osae_v_object_property WHERE (UPPER(object_name)=UPPER(objectName) OR UPPER(object_name)=UPPER(objectName)) AND property_datatype != "List";
  DECLARE property_list_cursor CURSOR FOR SELECT property_name,COALESCE(item_name,'') AS item_name,COALESCE(item_label,'') AS item_label FROM osae_v_object_property_array WHERE (UPPER(object_name)=UPPER(objectName) OR UPPER(object_name)=UPPER(objectName)) AND property_datatype = "List";

  DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_finished = TRUE;

  #SET vObjectType = CONCAT(objectName,'2');
  SELECT object_name,object_description,object_type,COALESCE(address,''),COALESCE(container_name,''),min_trust_level,enabled INTO vObjectName,vDescription,vObjectType,vAddress,vContainer,vMinTrustLevel,vEnabled FROM osae_v_object WHERE (UPPER(object_name)=UPPER(objectName) OR UPPER(object_name)=UPPER(objectName));
  SET vResults = CONCAT('CALL osae_sp_object_add (\'', REPLACE(vObjectName,'\'','\\\''),'\',\'',REPLACE(vDescription,'\'','\\\''),'\',\'',REPLACE(vObjectType,'\'','\\\''),'\',\'',vAddress,'\',\'',REPLACE(vContainer,'\'','\\\''),'\',',vMinTrustLevel,',',vEnabled,',@results);','\r\n');

  OPEN property_cursor;
  get_properties: LOOP
    SET v_finished = FALSE;
    FETCH property_cursor INTO vPropertyName,vPropertyValue;
    IF v_finished THEN 
      LEAVE get_properties;
    END IF;
    SET vResults = CONCAT(vResults,'CALL osae_sp_object_property_set(\'',REPLACE(vObjectName,'\'','\\\''),'\',\'',REPLACE(vPropertyName,'\'','\\\''),'\',\'',REPLACE(vPropertyValue,'\'','\\\''),'\',\'SYSTEM\',\'Import\');','\r\n');
  END LOOP get_properties;
  CLOSE property_cursor;

  OPEN property_list_cursor;
  get_properties_list: LOOP
    SET v_finished = FALSE;
    FETCH property_list_cursor INTO vPropertyName,vItemName,vItemLabel;
    IF v_finished THEN 
      LEAVE get_properties_list;
    END IF;
    SET vResults = CONCAT(vResults,'CALL osae_sp_object_property_array_add(\'',REPLACE(vObjectName,'\'','\\\''),'\',\'',REPLACE(vPropertyName,'\'','\\\''),'\',\'',REPLACE(vItemName,'\'','\\\''),'\',\'',vItemLabel,'\');','\r\n');
  END LOOP get_properties_list;
  CLOSE property_list_cursor;

 SELECT vResults; 
END
$$

--
-- Create procedure "osae_sp_object_export_all"
--
CREATE PROCEDURE osae_sp_object_export_all()
BEGIN
  DECLARE vResults TEXT;
  DECLARE vObjectName VARCHAR(255);
  DECLARE v_finished BOOL; 
  DECLARE object_list_cursor CURSOR FOR SELECT object_name FROM osae_v_object WHERE container_name !='SYSTEM';
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_finished = TRUE;

  OPEN object_list_cursor;
  get_properties_list: LOOP
    SET v_finished = FALSE;
    FETCH object_list_cursor INTO vObjectName;
    IF v_finished THEN 
      LEAVE get_properties_list;
    END IF;

    CALL osae_sp_object_export(vObjectName);
  END LOOP get_properties_list;
  CLOSE object_list_cursor;
  #SELECT vResults;
END
$$

--
-- Alter procedure "osae_sp_object_property_set"
--
DROP PROCEDURE osae_sp_object_property_set$$
CREATE PROCEDURE osae_sp_object_property_set(IN pname varchar(200), IN pproperty varchar(200), IN pvalue varchar(4000), IN pfromobject varchar(200), IN pdebuginfo varchar(2000))
BEGIN
DECLARE vObjectID INT DEFAULT 0;
DECLARE vObjectCount INT DEFAULT 0;
DECLARE vObjectTypeID INT DEFAULT 0;
DECLARE vObjectTypePropertyID INT DEFAULT 0;
DECLARE vPropertyID INT DEFAULT 0;
DECLARE vPropertyValue VARCHAR(4000);
DECLARE vPropertyCount INT DEFAULT 0;
DECLARE vObjectTrustCount INT DEFAULT 0;
DECLARE vFromObjectType VARCHAR(255);
DECLARE vMinTrustLevel INT DEFAULT 0;
DECLARE vOldTrustLevel INT DEFAULT 0;
DECLARE vNewTrustLevel INT DEFAULT 50;

DECLARE vEventCount INT;
DECLARE vDebugTrace VARCHAR(2000) DEFAULT '';

# Verify the FromObject has a trust_level and add one if not
SELECT COUNT(object_property_id) INTO vObjectTrustCount FROM osae_v_object_property WHERE property_name='Trust Level' AND (UPPER(object_name)=UPPER(pfromobject) OR UPPER(object_alias)=UPPER(pfromobject));
IF vObjectTrustCount = 0 THEN
    SELECT object_type INTO vFromObjectType FROM osae_v_object WHERE (UPPER(object_name)=UPPER(pfromobject) OR UPPER(object_alias)=UPPER(pfromobject));
    CALL osae_sp_object_type_property_add(vFromObjectType,'Trust Level','Integer','','50',0);
END IF;

    SET vDebugTrace = CONCAT(pdebuginfo,' -> object_property_set');
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(object_name)=UPPER(pname) OR UPPER(object_alias)=UPPER(pname) LIMIT 1; 
    IF vObjectCount  = 1 THEN  
        SELECT object_id,object_type_id,min_trust_level INTO vObjectID,vObjectTypeID,vMinTrustLevel FROM osae_object WHERE UPPER(object_name)=UPPER(pname) OR UPPER(object_alias)=UPPER(pname) LIMIT 1;
        SELECT COUNT(property_id),trust_level,object_type_property_id INTO vPropertyCount,vOldTrustLevel,vObjectTypePropertyID FROM osae_v_object_property WHERE (UPPER(object_name)=UPPER(pname) OR UPPER(object_alias)=UPPER(pname)) AND UPPER(property_name)=UPPER(pproperty) AND (property_value IS NULL OR property_value != pvalue); # AND object_type="PERSON";        
        IF vPropertyCount > 0 THEN
            SELECT property_value INTO vNewTrustLevel FROM osae_v_object_property WHERE UPPER(object_name)=UPPER(pfromobject) AND UPPER(property_name)='TRUST LEVEL';
            SELECT property_id,COALESCE(property_value,'') INTO vPropertyID, vPropertyValue FROM osae_v_object_property WHERE (UPPER(object_name)=UPPER(pname) OR UPPER(object_alias)=UPPER(pname)) AND UPPER(property_name)=UPPER(pproperty) AND (property_value IS NULL OR property_value != pvalue);
            #Insert Trust Level Rejection Code Here, maybe shppech command until converstaion tracking is in
            IF vNewTrustLevel >= vMinTrustLevel THEN
                UPDATE osae_object_property SET property_value=pvalue,trust_level=vNewTrustLevel,source_name=pfromobject,interest_level=0 WHERE object_id=vObjectID AND object_type_property_id=vPropertyID;
                #Updating a property is causing nesting because of the following line.   Maybe last updated needs to be a property.
                #UPDATE osae_object SET last_updated=NOW() WHERE object_id=vObjectID;            
                SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pproperty);
                IF vEventCount > 0 THEN  
                    CALL osae_sp_event_log_add(pname,pproperty,pfromobject,vDebugTrace,pvalue,NULL);
                END IF;
                #Since this property has changed, it has generated interest
                UPDATE osae_object_property SET interest_level = interest_level + 1 WHERE object_type_property_id = vObjectTypePropertyID AND (property_value IS NULL OR property_value = '');
            ELSE
                CALL osae_sp_debug_log_add(CONCAT(pfromobject, ' tried to set properties on ',pname,', but lacks sufficient trust'),'object_property_set'); 
            END IF;
        END IF;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_object_type_export"
--
DROP PROCEDURE osae_sp_object_type_export$$
CREATE PROCEDURE osae_sp_object_type_export(IN pObjectType VARCHAR(255))
BEGIN
  DECLARE vObjectType VARCHAR(200);
  DECLARE vResults TEXT;
  DECLARE vDescription VARCHAR(200);
  DECLARE vOwner VARCHAR(200);
  DECLARE vBaseType VARCHAR(200);
  DECLARE vTypeOwner INT;
  DECLARE vSystemType INT;
  DECLARE vContainer INT;
  DECLARE vHideRedundant INT;
  DECLARE v_finished INT; 
  DECLARE vName VARCHAR(200);
  DECLARE vLabel VARCHAR(200);
  DECLARE vParam1Name VARCHAR(200);
  DECLARE vParam1Default VARCHAR(200);
  DECLARE vParam2Name VARCHAR(200);
  DECLARE vParam2Default VARCHAR(200);
  DECLARE vDataType VARCHAR(200);
  DECLARE vDefault VARCHAR(200);
  DECLARE vTrackHistory VARCHAR(200);
  DECLARE vPropertyObjectType VARCHAR(200);
  DECLARE vPropertyOption VARCHAR(200);

  DECLARE state_cursor CURSOR FOR SELECT state_name,state_label FROM osae_v_object_type_state WHERE object_type=vObjectType;
  DECLARE event_cursor CURSOR FOR SELECT event_name,event_label FROM osae_v_object_type_event WHERE object_type=vObjectType;
  DECLARE method_cursor CURSOR FOR SELECT method_name,method_label,param_1_label,param_1_default,param_2_label,param_2_default FROM osae_v_object_type_method WHERE object_type=vObjectType;
  DECLARE property_cursor CURSOR FOR SELECT property_name,property_datatype,property_default,property_object_type,track_history FROM osae_v_object_type_property WHERE object_type=vObjectType;
  DECLARE property_option_cursor CURSOR FOR SELECT option_name FROM osae_v_object_type_property_option WHERE object_type=vObjectType and property_name=vname;

  DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_finished = TRUE;

  #SET vObjectType = CONCAT(objectName,'2');

  SELECT object_type,object_type_description,COALESCE(object_name,''),base_type,object_type_owner,system_hidden,container,hide_redundant_events INTO vObjectType,vDescription,vOwner,vBaseType,vTypeOwner,vSystemType,vContainer,vHideRedundant FROM osae_v_object_type WHERE object_type=pObjectType;
  SET vResults = CONCAT('CALL osae_sp_object_type_add (\'', REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vDescription,'\'','\\\''),'\',\'',vOwner,'\',\'',vBaseType,'\',',vTypeOwner,',', vSystemType,',',vContainer,',',vHideRedundant,');','\r\n');

  OPEN state_cursor;
    get_states: LOOP
    SET v_finished = FALSE;
      FETCH state_cursor INTO vName,vLabel;
      IF v_finished THEN 
        LEAVE get_states;
      END IF;
      SET vResults = CONCAT(vResults,'CALL osae_sp_object_type_state_add(\'',REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vName,'\'','\\\''),'\',\'',REPLACE(vLabel,'\'','\\\''),'\');','\r\n');
    END LOOP get_states;
  CLOSE state_cursor;


  OPEN event_cursor;
  get_events: LOOP
    SET v_finished = FALSE;
    FETCH event_cursor INTO vName,vLabel;
    IF v_finished THEN 
      LEAVE get_events;
    END IF;
    SET vResults = CONCAT(vResults,'CALL osae_sp_object_type_event_add(\'',REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vName,'\'','\\\''),'\',\'',REPLACE(vLabel,'\'','\\\''),'\');','\r\n');
  END LOOP get_events;
  CLOSE event_cursor;

  OPEN method_cursor;
  get_methods: LOOP
    SET v_finished = FALSE;
    FETCH method_cursor INTO vName,vLabel,vParam1Name,vParam1Default,vParam2Name,vParam2Default;
    IF v_finished THEN 
      LEAVE get_methods;
    END IF;
    SET vResults = CONCAT(vResults,'CALL osae_sp_object_type_method_add(\'',REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vName,'\'','\\\''),'\',\'',REPLACE(vLabel,'\'','\\\''),'\',\'',vParam1Name,'\',\'',vParam2Name,'\',\'',vParam1Default,'\',\'',vParam2Default,'\');','\r\n');
  END LOOP get_methods;
  CLOSE method_cursor;
  SET v_finished = 0;

  OPEN property_cursor;
  get_properties: LOOP
    SET v_finished = FALSE;
    FETCH property_cursor INTO vName,vDataType,vDefault,vPropertyObjectType,vTrackHistory;
    IF v_finished THEN 
      LEAVE get_properties;
    END IF;
    SET vResults = CONCAT(vResults,'CALL osae_sp_object_type_property_add(\'',REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vName,'\'','\\\''),'\',\'',vDataType,'\',\'',vPropertyObjectType,'\',\'',vDefault,'\',',vTrackHistory,');','\r\n');
  
    OPEN property_option_cursor;
    get_property_options: LOOP
      SET v_finished = FALSE;
      FETCH property_option_cursor INTO vPropertyOption;
      IF v_finished THEN
        SET v_finished := false;
        LEAVE get_property_options;
      END IF;
      SET vResults = CONCAT(vResults,'CALL osae_sp_object_type_property_option_add(\'',REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vName,'\'','\\\''),'\',\'',vPropertyOption,'\');','\r\n');
 
    END LOOP get_property_options;
    CLOSE property_option_cursor;
  
  
  
  END LOOP get_properties;
  CLOSE property_cursor;

  SELECT vResults;
END
$$

--
-- Alter procedure "osae_sp_object_update"
--
DROP PROCEDURE osae_sp_object_update$$
CREATE PROCEDURE osae_sp_object_update(IN poldname VARCHAR(100), IN pnewname VARCHAR(100), IN palias VARCHAR(100), IN pdesc VARCHAR(200), IN pobjecttype VARCHAR(200), IN paddress VARCHAR(200), IN pcontainer VARCHAR(200), IN pmintrustlevel INT, IN penabled TINYINT)
BEGIN
DECLARE vAlias           VARCHAR(255) DEFAULT '';
DECLARE vAliasCount      INT; 
DECLARE vAddressCount INT;
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
DECLARE vContainerCount INT;
DECLARE vContainerID INT DEFAULT NULL;

  IF pnewname != palias THEN
    IF palias != '' THEN
      SELECT count(object_id) INTO vAliasCount FROM osae_object WHERE object_name = palias OR object_alias = palias;
      IF vAliasCount > 0 THEN
        SET vAlias = '';
      ELSE
        SET vAlias = palias;
      END IF;
    END IF;
  END IF;

    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        SELECT COUNT(object_id) INTO vContainerCount FROM osae_v_object WHERE object_name=pcontainer AND container=1;
        IF vContainerCount = 1 THEN
            SELECT object_id INTO vContainerID FROM osae_v_object WHERE object_name=pcontainer AND container=1; 
        END IF;
        SELECT COUNT(object_id) INTO vAddressCount FROM osae_object WHERE UPPER(object_name) <> UPPER(poldname) AND (UPPER(address)=UPPER(paddress) AND address IS NOT NULL AND address <> '');                 
        IF vAddressCount = 0 THEN
          UPDATE osae_object SET object_name=pnewname,object_alias=vAlias,object_description=pdesc,object_type_id=vObjectTypeID,address=paddress,container_id=vContainerID,min_trust_level=pmintrustlevel,enabled=penabled,last_updated=NOW() WHERE object_name=poldname;
        ELSE
          CALL osae_sp_debug_log_add (CONCAT('Object_Updated Failed on ',pnewname,' due to duplicate data.'),'SYSTEM');
        END IF;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_run_readers"
--
DROP PROCEDURE osae_sp_run_readers$$
CREATE PROCEDURE osae_sp_run_readers()
BEGIN
DECLARE iReadersReady INT;
    SELECT COUNT(object_property_scraper_id) INTO iReadersReady FROM osae_v_object_property_scraper_ready;
    IF iReadersReady > 0 THEN
        CALL osae_sp_method_queue_add('Script Processor','RUN READERS','','','SYSTEM','run_readers');
    END IF;         
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
    #CALL osae_sp_debug_log_add('Starting to run osae_sp_run_scheduled_methods','osae_sp_run_scheduled_methods');
    OPEN cur1; 
    Loop_Tag: LOOP
        FETCH cur1 INTO iSCHEDULEID,vObjectName,vMethodName,vPARAM1,vPARAM2,iSCRIPTID;
        IF done THEN
            Leave Loop_Tag;
        END IF;
            CALL osae_sp_debug_log_add(CONCAT('Found Schedule to run:',iSCHEDULEID,'  Object=',vObjectName,'   ScriptID=',COALESCE(iSCRIPTID,0)),'run_scheduled_methods');
            IF vObjectName <> '' THEN
                CALL osae_sp_debug_log_add(CONCAT('Adding method to queue:',' Object=',vObjectName,'   Method=',vMethodName),'run_scheduled_methods');
                CALL osae_sp_method_queue_add(vObjectName,vMethodName,vPARAM1,vPARAM2,'SYSTEM','run_scheduled_methods');
            ELSEIF iSCRIPTID <> 0 THEN
                SELECT script_processor_id INTO vSCRIPTPROCID FROM osae_script WHERE script_id=iSCRIPTID;
                SELECT script_processor_plugin_name INTO scriptProc FROM osae_script_processors WHERE script_processor_id=vSCRIPTPROCID;
                CALL osae_sp_debug_log_add(CONCAT('Adding method to queue:',' Object=',scriptProc,'   ScriptID=',iSCRIPTID),'run_scheduled_methods');
                CALL osae_sp_method_queue_add(scriptProc,'RUN SCRIPT',iSCRIPTID,'SYSTEM','SYSTEM','run_scheduled_methods');
            ELSE
                CALL osae_sp_debug_log_add(CONCAT('ERROR Adding method to queue:',' vObjectName=',vObjectName,'   ScriptID=',iSCRIPTID),'run_scheduled_methods');
            END IF;         
            DELETE FROM osae_schedule_queue WHERE schedule_ID=iSCHEDULEID; 
        END LOOP;
    CLOSE cur1;
    CALL osae_sp_process_recurring();   
END
$$

--
-- Create procedure "osae_sp_script_export_all"
--
CREATE PROCEDURE osae_sp_script_export_all()
BEGIN
  DECLARE vResults TEXT;
  DECLARE vScriptName VARCHAR(255);
  DECLARE v_finished BOOL; 
  DECLARE script_cursor CURSOR FOR SELECT script_name FROM osae_script;
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_finished = TRUE;

  OPEN script_cursor;
  get_script_list: LOOP
    SET v_finished = FALSE;
    FETCH script_cursor INTO vScriptName;
    IF v_finished THEN 
      LEAVE get_script_list;
    END IF;

    CALL osae_sp_script_export(vScriptName);
  END LOOP get_script_list;
  CLOSE script_cursor;
  #SELECT vResults;
END
$$

--
-- Alter procedure "osae_sp_system_count_plugins"
--
DROP PROCEDURE osae_sp_system_count_plugins$$
CREATE PROCEDURE osae_sp_system_count_plugins()
BEGIN
DECLARE vPluginCount INT;
DECLARE vOldCount INT;
DECLARE iPluginCount INT;
DECLARE iPluginEnabledCount INT;
DECLARE iPluginRunningCount INT;  
DECLARE iPluginErrorCount INT;
DECLARE bDone INT; 
DECLARE vOutput VARCHAR(200);
DECLARE oCount INT;
DECLARE var1 CHAR(40);
  
DECLARE curs CURSOR FOR SELECT object_name FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1 AND state_name='OFF';
DECLARE CONTINUE HANDLER FOR NOT FOUND SET bDone = 1;
    SET vOldCount = (SELECT COUNT(property_value) FROM osae_v_object_property WHERE object_name='SYSTEM' AND property_name='Plugins Running');  
    SET iPluginErrorCount = (SELECT COUNT(object_name) FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1 AND state_name='OFF');
 
   -- IF vOldCount != iPluginErrorCount THEN  
        SET iPluginCount = (SELECT COUNT(object_name) FROM osae_v_object WHERE base_type='PLUGIN');
        SET iPluginEnabledCount = (SELECT COUNT(object_name) FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1);
        SET iPluginRunningCount = (SELECT COUNT(object_name) FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1 AND state_name='ON');

        CALL osae_sp_object_property_set('SYSTEM','Plugins Found',iPluginCount,'SYSTEM','system_count_plugins');
        CALL osae_sp_object_property_set('SYSTEM','Plugins Enabled',iPluginEnabledCount,'SYSTEM','system_count_plugins');
        CALL osae_sp_object_property_set('SYSTEM','Plugins Running',iPluginRunningCount,'SYSTEM','system_count_plugins');
        CALL osae_sp_object_property_set('SYSTEM','Plugins Errored',iPluginErrorCount,'SYSTEM','system_count_plugins');

        CASE iPluginErrorCount
          WHEN 0 THEN 
            SET vOutput = 'All Plugins are Running';
            CALL osae_sp_object_property_set('SYSTEM','Plugins',vOutput,'SYSTEM','system_count_plugins');            
          WHEN 1 THEN 
            SET vOutput = (SELECT COALESCE(object_name,'Unknown') FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1 AND state_name='OFF' LIMIT 1);
            SET vOutput = CONCAT(vOutput,' is Stopped!');
            CALL osae_sp_object_property_set('SYSTEM','Plugins',vOutput,'SYSTEM','system_count_plugins');
          ELSE
            OPEN curs;
            SET oCount = 0;
            SET bDone = 0;
            SET vOutput = '';
            REPEAT
              FETCH curs INTO var1;
              IF oCount < iPluginErrorCount THEN
                IF oCount = 0 THEN
                  SET vOutput = CONCAT(vOutput,CONCAT(' and ', var1, ' are Stopped!'));
                ELSEIF oCount = 1 THEN
                  SET vOutput = CONCAT(var1, vOutput);
                ELSE
                  SET vOutput = CONCAT(var1, ', ', vOutput);
                END IF;
                SET oCount = oCount + 1;
              END IF;
            UNTIL bDone END REPEAT;
            CLOSE curs;
            CALL osae_sp_object_property_set('SYSTEM','Plugins',vOutput,'SYSTEM','system_count_plugins');
         END CASE;
 --     END IF;
END
$$

--
-- Alter procedure "osae_sp_system_count_room_occupants"
--
DROP PROCEDURE osae_sp_system_count_room_occupants$$
CREATE PROCEDURE osae_sp_system_count_room_occupants()
BEGIN
  DECLARE oHouse char(255);
  DECLARE vHouseOccupantCount int;
  DECLARE vHouseOldCount int;
  DECLARE vHouseRoomCount int;
  DECLARE vHouseOldRoomCount int;
  DECLARE vTemp varchar(200);
  DECLARE vOutput,vOutput2 varchar(200);
  DECLARE bDone, bDone2 int;
  DECLARE var1 char(255);
  DECLARE var2 char(255);
  DECLARE oCount, oCount2 int;
  DECLARE sContainerName char(255);
  DECLARE iContainerOccupants int;
  DECLARE iContainerOldOccupants int;

  DECLARE cursPlaces CURSOR FOR SELECT room, occupant_count FROM osae_v_system_occupied_rooms;
  DECLARE cursOccpiedPlaces CURSOR FOR SELECT room, occupant_count FROM osae_v_system_occupied_rooms WHERE occupant_count > 0;
  DECLARE cursPeople CURSOR FOR SELECT object_name FROM osae_v_object WHERE object_type = 'PERSON' AND state_name='ON';
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET bDone = 1;

  # Handle The HOUSE level occupant count
  SET oHouse = (SELECT object_name FROM osae_v_object WHERE object_type = 'HOUSE' LIMIT 1);
  SET vHouseOldCount = (SELECT property_value FROM osae_v_object_property WHERE object_name = oHouse AND property_name = 'Occupant Count');
  SELECT SUM(occupant_count) INTO vHouseOccupantCount FROM osae_v_system_occupied_rooms;
  SET vHouseOldRoomCount = (SELECT IF(CHAR_LENGTH(property_value) > 0, property_value, 0) FROM osae_v_object_property WHERE object_name = oHouse AND property_name = 'Occupied Room Count');
  SELECT COUNT(room) INTO vHouseRoomCount FROM osae_v_system_occupied_rooms WHERE occupant_count > 0;
  IF vHouseOldCount != vHouseOccupantCount THEN
    CALL osae_sp_object_property_set(oHouse, 'Occupant Count', vHouseOccupantCount, 'SYSTEM', 'system_count_occupants');
    CASE vHouseOccupantCount
      WHEN 0 THEN
        SET vOutput = 'Nobody is here';
        CALL osae_sp_object_property_set(oHouse, 'Occupants', vOutput, 'SYSTEM', 'system_count_occupants');
        CALL osae_sp_method_queue_add(oHouse, 'OFF', '', '', 'SYSTEM', 'Auto-Occupancy Logic');
      WHEN 1 THEN
        SET vOutput = (SELECT COALESCE(object_name, 'Nobody') FROM osae_v_object WHERE object_type = 'PERSON' AND state_name = 'ON' LIMIT 1);
        SET vOutput = CONCAT(vOutput, ' is here');
        CALL osae_sp_object_property_set(oHouse, 'Occupants', vOutput, 'SYSTEM', 'system_count_occupants');
        CALL osae_sp_method_queue_add(oHouse, 'ON', '', '', 'SYSTEM', 'Auto-Occupancy Logic');
      ELSE
        OPEN cursPeople;
        SET oCount = 0;
        SET bDone = 0;
        SET vOutput = '';
        REPEAT
          FETCH cursPeople INTO var1;
          IF oCount < vHouseOccupantCount THEN
            IF oCount = 0 THEN
              SET vOutput = CONCAT(vOutput, CONCAT(' and ', var1, ' are here'));
            ELSEIF oCount = 1 THEN
              SET vOutput = CONCAT(var1, vOutput);
            ELSE
              SET vOutput = CONCAT(var1, ', ', vOutput);
            END IF;
            SET oCount = oCount + 1;
          END IF;
        UNTIL bDone END REPEAT;

        CLOSE cursPeople;
        CALL osae_sp_object_property_set(oHouse, 'Occupants', vOutput, 'SYSTEM', 'system_count_occupants');
        CALL osae_sp_method_queue_add(oHouse, 'ON', '', '', 'SYSTEM', 'Auto-Occupancy Logic');
    END CASE;
  END IF;

  IF vHouseOldRoomCount != vHouseRoomCount THEN
    CALL osae_sp_object_property_set(oHouse, 'Occupied Room Count', vHouseRoomCount, 'SYSTEM', 'system_count_occupants');
  END IF;

  CASE vHouseRoomCount
    WHEN 0 THEN
      SET vOutput = 'All rooms are vacant';
      CALL osae_sp_object_property_set(oHouse, 'Occupied Rooms', vOutput, 'SYSTEM', 'system_count_occupants');
    WHEN 1 THEN
      SET vOutput = (SELECT COALESCE(object_name, 'Unknown') FROM osae_v_object WHERE object_type = 'ROOM' AND state_name = 'ON' LIMIT 1);
      SET vOutput = CONCAT(vOutput, ' is occupied');
      CALL osae_sp_object_property_set(oHouse, 'Occupied Rooms', vOutput, 'SYSTEM', 'system_count_occupants');
    ELSE OPEN cursOccpiedPlaces;
      SET oCount = 0;
      SET bDone = 0;
      SET vOutput = '';
      REPEAT
        FETCH cursOccpiedPlaces INTO var1,var2;
        IF oCount < vHouseRoomCount THEN
          IF oCount = 0 THEN
            SET vOutput = CONCAT(vOutput, CONCAT(' and ', var1, ' are occupied'));
          ELSEIF oCount = 1 THEN
            SET vOutput = CONCAT(var1, vOutput);
          ELSE
            SET vOutput = CONCAT(var1, ', ', vOutput);
          END IF;
          SET oCount = oCount + 1;
        END IF;
      UNTIL bDone END REPEAT;
      CLOSE cursOccpiedPlaces;
      CALL osae_sp_object_property_set(oHouse, 'Occupied Rooms', vOutput, 'SYSTEM', 'system_count_occupants');
  END CASE;

  #Count the occupants in each Room and turn the objects on and off...
  OPEN cursPlaces;
  SET bDone = 0;
  REPEAT
    SET sContainerName = '';
    SET iContainerOccupants = 0;
    FETCH cursPlaces INTO sContainerName, iContainerOccupants;
    SET iContainerOldOccupants = (SELECT COALESCE(property_value, 0) FROM osae_v_object_property WHERE object_name = sContainerName AND property_name = 'Occupant Count' AND property_value != '');
    IF iContainerOccupants != iContainerOldOccupants THEN
      CALL osae_sp_debug_log_add(CONCAT('Counting Container Occupants Found a Change in ', sContainerName), 'SYSTEM');
      CALL osae_sp_object_property_set(sContainerName, 'Occupant Count', iContainerOccupants, '', '');
      IF iContainerOccupants = 0 THEN
        CALL osae_sp_method_queue_add(sContainerName, 'OFF', '', '', 'SYSTEM', 'Auto-Occupancy Logic');
        CALL osae_sp_object_property_set(sContainerName, 'Occupants', 'Nobody', 'SYSTEM', 'system_count_container_occupants');
      ELSE
        CALL osae_sp_method_queue_add(sContainerName, 'ON', '', '', 'SYSTEM', 'Auto-Occupancy Logic');
        IF iContainerOccupants = 1 THEN
          SET vOutput = (SELECT object_name FROM osae_v_object WHERE object_type = 'PERSON' AND container_name = sContainerName LIMIT 1);
          CALL osae_sp_object_property_set(sContainerName, 'Occupants', vOutput, 'SYSTEM', 'system_count_occupants');
        ELSE
          BLOCK2: BEGIN
            DECLARE cursRoomOccupants CURSOR FOR SELECT object_name FROM osae_v_object WHERE object_type = 'PERSON' AND container_name=sContainerName;
            DECLARE CONTINUE HANDLER FOR NOT FOUND SET bDone2 = 1;
            OPEN cursRoomOccupants;
            SET oCount2 = 0;
            SET bDone2 = 0;
            SET vOutput2 = '';
            REPEAT
              FETCH cursRoomOccupants INTO var2;
              IF oCount2 < iContainerOccupants THEN
                IF oCount2 = 0 THEN
                  SET vOutput2 = CONCAT(vOutput2, CONCAT(' and ', var2, ' are here'));
                ELSEIF oCount2 = 1 THEN
                  SET vOutput2 = CONCAT(var2, vOutput2);
                ELSE
                  SET vOutput2 = CONCAT(var2, ', ', vOutput2);
                END IF;
                SET oCount2 = oCount2 + 1;
              END IF;
            UNTIL bDone2 END REPEAT;
        CLOSE cursRoomOccupants;
END BLOCK2;
          CALL osae_sp_object_property_set(sContainerName, 'Occupants', vOutput2, 'SYSTEM', 'system_count_occupants');
        END IF;
      END IF;
    END IF;
  UNTIL bDone END REPEAT;
  CLOSE cursPlaces;
END
$$

--
-- Alter procedure "osae_sp_system_process_methods"
--
DROP PROCEDURE osae_sp_system_process_methods$$
CREATE PROCEDURE osae_sp_system_process_methods()
BEGIN
DECLARE vMethodQueueID INT;
DECLARE vSystemCount INT;
DECLARE vObjectName VARCHAR(200);
DECLARE vObjectType VARCHAR(200);
DECLARE vBaseType VARCHAR(200);
DECLARE vMethod VARCHAR(400);
DECLARE vParam1 VARCHAR(400);
DECLARE vParam2 VARCHAR(400);
DECLARE vStateCount INT;
DECLARE vPropertyCount INT;
DECLARE vSendMethod VARCHAR(40);
DECLARE vContainerID INT;

DECLARE done INT DEFAULT 0;  
DECLARE cur1 CURSOR FOR SELECT method_queue_id,object_name,object_type,method_name,parameter_1,parameter_2 FROM osae_v_method_queue WHERE object_owner='SYSTEM';
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
    OPEN cur1; 
    Loop_Tag: LOOP
        FETCH cur1 INTO vMethodQueueID,vObjectName,vObjectType,vMethod,vParam1,vParam2;
        IF done THEN
            Leave Loop_Tag;
        END IF;
            CALL osae_sp_debug_log_add(CONCAT('SYSTEM handling ', vObjectName,' ',vMethod),'process_system_methods'); 
            DELETE FROM osae_method_queue WHERE method_queue_id = vMethodQueueID;
            #SELECT object_name,object_type,base_type INTO vObjectName,vObjectType,vBaseType FROM osae_v_object WHERE object_id=vObjectID;
            #SELECT method_name INTO vMethod FROM osae_object_type_method WHERE method_id=NEW.method_id;        
            # Here is the magic, if the Method Name matches a State, then the Method's Job is to Set that State, so look up the state
            IF UPPER(vMethod) = 'SEND MESSAGE' THEN
                SELECT UPPER(property_value) INTO vSendMethod FROM osae_v_object_property WHERE UPPER(object_name)=UPPER(vObjectName) AND UPPER(property_name)='COMMUNICATION METHOD'; 
                IF vSendMethod = 'SPEECH' THEN
                     CALL osae_sp_method_queue_add('SPEECH','SAY',vParam1,'','SYSTEM','process_system_methods');
                ELSEIF vSendMethod = 'JABBER' THEN
                     CALL osae_sp_method_queue_add('JABBER','SEND MESSAGE',vObjectName,vParam1,'SYSTEM','process_system_methods');
                END IF;
            ELSEIF UPPER(vMethod) = 'SET CONTAINER' THEN
                SELECT object_id INTO vContainerID FROM osae_object WHERE object_name=vParam1;
                UPDATE osae_object SET container_id = vContainerID,last_updated=NOW() WHERE object_name=vObjectName;
            ELSE
                SELECT count(state_name) INTO vStateCount FROM osae_v_object_type_state WHERE UPPER(state_name)=UPPER(vMethod) AND object_type=vObjectType; 
                IF vStateCount = 1 THEN   
                    CALL osae_sp_object_state_set (vObjectName,vMethod,'SYSTEM','process_system_methods');
                ELSE
                    SELECT count(property_name) INTO vPropertyCount FROM osae_v_object_type_property WHERE UPPER(property_name)=UPPER(vMethod) AND object_type=vObjectType; 
                    IF vPropertyCount = 1 THEN   
                        CALL osae_sp_object_property_set (vObjectName,vMethod,NEW.parameter_1,'SYSTEM','process_system_methods');
                    END IF;
                END IF;
            END IF;                   
        END LOOP;
    CLOSE cur1;   
END
$$

--
-- Alter trigger "osae_tr_method_queue_before_delete"
--
DROP TRIGGER IF EXISTS osae_tr_method_queue_before_delete$$
CREATE TRIGGER osae_tr_method_queue_before_delete
	BEFORE DELETE
	ON osae_method_queue
	FOR EACH ROW
BEGIN
  INSERT INTO osae_method_log (entry_time,object_id,method_id,parameter_1,parameter_2,from_object_id,debug_trace) VALUES(OLD.entry_time,OLD.object_id,OLD.method_id,OLD.parameter_1,OLD.parameter_2,OLD.from_object_id,CONCAT(OLD.debug_trace,' -> method_queue_before_delete'));
END
$$

--
-- Alter trigger "osae_tr_object_after_update"
--
DROP TRIGGER IF EXISTS osae_tr_object_after_update$$
CREATE TRIGGER osae_tr_object_after_update
	AFTER UPDATE
	ON osae_object
	FOR EACH ROW
BEGIN
  DECLARE vPersonID int;
  DECLARE vBaseType varchar(200);
  DECLARE vContainerType varchar(200);
  DECLARE vContainerName varchar(100);
  DECLARE vStateName varchar(100);
  SELECT base_type, object_name INTO vContainerType, vContainerName FROM osae_v_object WHERE object_id = NEW.container_id;
  SELECT base_type, state_name INTO vBaseType, vStateName FROM osae_v_object WHERE object_id = OLD.object_id;
  SET vPersonID = (SELECT object_type_id FROM osae_object_type WHERE object_type = 'PERSON');
  IF NEW.object_type_id = vPersonID AND OLD.container_id <> NEW.container_id AND vContainerType = 'PLACE' THEN
    CALL osae_sp_debug_log_add('Object After Update - Update Screen Position: ', 'SYSTEM');
    #CALL osae_sp_system_count_room_occupants;
    #Update the Screen Objects to reflect the container
    CALL osae_sp_screen_object_position(OLD.object_name, vContainerName);
  END IF;

  IF OLD.state_id <> NEW.state_id AND vBaseType = "SENSOR" AND vStateName != "OFF" THEN
    CALL osae_sp_system_who_tripped_sensor(NEW.object_name);
  END IF;

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
DECLARE vObjectName VARCHAR(400); 
DECLARE vScriptSeq INT;
DECLARE vPrevScriptSeq INT;
DECLARE scriptProc VARCHAR(200);
DECLARE vDebugTrace VARCHAR(2000);
    SET vPrevScriptSeq = -1;
    SET vScriptSeq = 0;
    SET vDebugTrace = CONCAT(COALESCE(NEW.debug_trace,''),' -> event_log_after_insert');
    CALL osae_sp_debug_log_add(CONCAT('Event_Trigger is running for ',NEW.object_id,' ',NEW.event_id),vDebugTrace);
    SELECT object_name INTO vObjectName FROM osae_object WHERE object_id=NEW.object_id LIMIT 1;
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
            CALL osae_sp_method_queue_add (scriptProc,'RUN SCRIPT',vScriptID,vObjectName,'SYSTEM',vDebugTrace);
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
            CALL osae_sp_method_queue_add (scriptProc,'RUN SCRIPT',vScriptID,vObjectName,'SYSTEM',vDebugTrace);
          END IF;
        END WHILE;
    END IF; 
END
$$

DELIMITER ;

--
-- Alter view "osae_v_object_property"
--
CREATE OR REPLACE 
VIEW osae_v_object_property
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`container_id` AS `container_id`,`osae_object`.`min_trust_level` AS `min_trust_level`,`osae_object`.`enabled` AS `enabled`,`osae_object`.`last_updated` AS `object_last_updated`,coalesce(`osae_object`.`last_state_change`,now()) AS `last_state_change`,`osae_object_property`.`last_updated` AS `last_updated`,`osae_object_property`.`object_property_id` AS `object_property_id`,`osae_object_property`.`object_type_property_id` AS `object_type_property_id`,`osae_object_property`.`trust_level` AS `trust_level`,`osae_object_property`.`interest_level` AS `interest_level`,coalesce(`osae_object_property`.`source_name`,'') AS `source_name`,coalesce(`osae_object_property`.`property_value`,'') AS `property_value`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type_property`.`property_datatype` AS `property_datatype`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_type_property`.`property_default` AS `property_default`,`osae_object_type_property`.`property_id` AS `property_id`,`osae_object_type_property`.`track_history` AS `track_history`,`ot1`.`object_type` AS `base_type`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_property`.`property_object_type_id` AS `property_object_type_id`,`osae_object_type_1`.`object_type` AS `property_object_type`,`osae_object_1`.`object_name` AS `container_name`,timestampdiff(SECOND,`osae_object`.`last_state_change`,now()) AS `time_in_state` from (((((((`osae_object` join `osae_object_property` on((`osae_object`.`object_id` = `osae_object_property`.`object_id`))) join `osae_object_type` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) join `osae_object_type_property` on(((`osae_object_type`.`object_type_id` = `osae_object_type_property`.`object_type_id`) and (`osae_object_property`.`object_type_property_id` = `osae_object_type_property`.`property_id`)))) left join `osae_object_type_state` on((`osae_object`.`state_id` = `osae_object_type_state`.`state_id`))) join `osae_object_type` `ot1` on((`osae_object_type`.`base_type_id` = `ot1`.`object_type_id`))) left join `osae_object_type` `osae_object_type_1` on((`osae_object_type_property`.`property_object_type_id` = `osae_object_type_1`.`object_type_id`))) left join `osae_object` `osae_object_1` on((`osae_object`.`container_id` = `osae_object_1`.`object_id`)));

--
-- Alter view "osae_v_object"
--
CREATE OR REPLACE 
VIEW osae_v_object
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`last_updated` AS `last_updated`,`osae_object`.`last_state_change` AS `last_state_change`,`osae_object`.`min_trust_level` AS `min_trust_level`,`osae_object`.`enabled` AS `enabled`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`container` AS `container`,`osae_object_type_state`.`state_id` AS `state_id`,coalesce(`osae_object_type_state`.`state_name`,'') AS `state_name`,coalesce(`osae_object_type_state`.`state_label`,'') AS `state_label`,`objects_2`.`object_name` AS `owned_by`,`object_types_2`.`object_type` AS `base_type`,`objects_1`.`object_name` AS `container_name`,`osae_object`.`container_id` AS `container_id`,(select max(`osae_v_object_property`.`last_updated`) from `osae_v_object_property` where ((`osae_v_object_property`.`object_id` = `osae_object`.`object_id`) and (`osae_v_object_property`.`property_name` <> 'Time'))) AS `property_last_updated`,timestampdiff(SECOND,`osae_object`.`last_state_change`,now()) AS `time_in_state` from (((((`osae_object` left join `osae_object_type` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) left join `osae_object_type` `object_types_2` on((`osae_object_type`.`base_type_id` = `object_types_2`.`object_type_id`))) left join `osae_object` `objects_2` on((`osae_object_type`.`plugin_object_id` = `objects_2`.`object_id`))) left join `osae_object_type_state` on(((`osae_object_type`.`object_type_id` = `osae_object_type_state`.`object_type_id`) and (`osae_object_type_state`.`state_id` = `osae_object`.`state_id`)))) left join `osae_object` `objects_1` on((`objects_1`.`object_id` = `osae_object`.`container_id`)));

DELIMITER $$

--
-- Alter event "osae_ev_off_timer"
--
ALTER EVENT osae_ev_off_timer
	DO 
BEGIN
  DECLARE vObjectName  VARCHAR(200);
  DECLARE iLoopCount   INT DEFAULT 0;
  DECLARE iMethodCount INT DEFAULT 0;
  DECLARE iStateCount  INT DEFAULT 0;
  DECLARE done         INT DEFAULT 0;
  DECLARE cur1 CURSOR FOR SELECT object_name FROM osae_v_object_property WHERE state_name <> 'OFF' AND property_name = 'OFF TIMER' AND property_value IS NOT NULL AND property_value <> '' AND property_value <> '-1' AND subtime(now(), sec_to_time(property_value)) > object_last_updated;
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
  DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
  CALL osae_sp_object_property_set('SYSTEM', 'Time', curtime(), 'SYSTEM', 'osae_ev_off_timer');
  CALL osae_sp_object_property_set('SYSTEM', 'Time AMPM', DATE_FORMAT(now(), '%h:%i %p'), 'SYSTEM', 'osae_ev_off_timer');
  CALL osae_sp_system_process_methods();
  CALL osae_sp_system_count_room_occupants();
  CALL osae_sp_system_count_plugins();
  #SELECT count(object_name) INTO iLoopCount FROM osae_v_object_property WHERE state_name <> 'OFF' AND property_name = 'OFF TIMER' AND property_value IS NOT NULL AND property_value <> '' AND subtime(now(), sec_to_time(property_value)) > object_last_updated;
  OPEN cur1;

Loop_Tag:
  LOOP
    FETCH cur1 INTO vObjectName;
    IF done THEN
      LEAVE Loop_Tag;
    END IF;
    SELECT count(method_id) INTO iMethodCount FROM osae_v_object_method WHERE upper(object_name) = upper(vObjectName) AND upper(method_name) = 'OFF';
    IF iMethodCount > 0 THEN
      CALL osae_sp_debug_log_add(concat('Turning ', vObjectName, ' Off'), 'osae_ev_off_timer');
      CALL osae_sp_method_queue_add(vObjectName, 'OFF', '', '', 'SYSTEM', 'osae_ev_off_timer');
    ELSE
      SELECT count(state_id) INTO iStateCount FROM osae_v_object_state WHERE upper(object_name) = upper(vObjectName) AND upper(state_name) = 'OFF';
      IF iStateCount > 0 THEN
        CALL osae_sp_debug_log_add(concat('Turning ', vObjectName, ' Off'), 'osae_ev_off_timer');
        CALL osae_sp_object_state_set(vObjectName, 'OFF', 'SYSTEM', 'osae_ev_off_timer');
      END IF;
    END IF;
  END LOOP;
  CLOSE cur1;

  SELECT count(method_id) INTO iMethodCount FROM osae_v_object_method;
END
$$

DELIMITER ;

--
-- Enable foreign keys
--
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;


CALL osae_sp_object_property_set('SYSTEM','DB Version','048','SYSTEM','');

CALL osae_sp_object_type_method_add('SERVICE','BROADCAST','Broadcast','group','message','Command','Test Message');
CALL osae_sp_object_type_property_add('SERVICE','Trust Level','Integer','','90',0);

CALL osae_sp_object_type_property_add('PERSON','PIN','Password','','',0);

CALL osae_sp_object_type_update ('WEATHER','WEATHER','Weather Data','SYSTEM','THING',0,1,0,1);
CALL osae_sp_object_type_update ('COMPUTER','COMPUTER','Core Type: Computer','','THING',0,1,1,1);
CALL osae_sp_object_type_update ('SERVICE','SERVICE','OSA Service','','SERVICE',0,1,1,1);

CALL osae_sp_object_type_property_add('GUI CLIENT','Current Screen','String','','',0);
CALL osae_sp_object_type_property_add('GUI CLIENT','Default Screen','String','','',0);
CALL osae_sp_object_type_property_add('GUI CLIENT','Current User','String','','',0);
CALL osae_sp_object_type_property_add('GUI CLIENT','Debug','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('GUI CLIENT','Title','String','','OSA Screens',0);
CALL osae_sp_object_type_property_add('GUI CLIENT','Logout on Close','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_delete('Computer Name','GUI CLIENT');

CALL osae_sp_object_type_add ('CONTROL USER SELECTOR','Control - User Selector','','CONTROL',0,1,0,1);
CALL osae_sp_object_type_property_add('CONTROL USER SELECTOR','X','Integer','','',0);
CALL osae_sp_object_type_property_add('CONTROL USER SELECTOR','Y','Integer','','',0);
CALL osae_sp_object_type_property_add('CONTROL USER SELECTOR','ZOrder','Integer','','',0);

CALL osae_sp_object_type_add ('CONTROL SCREEN OBJECTS','Control - Screen Objects','','CONTROL',0,1,0,1);
CALL osae_sp_object_type_property_add('CONTROL SCREEN OBJECTS','X','Integer','','0',1);
CALL osae_sp_object_type_property_add('CONTROL SCREEN OBJECTS','Y','Integer','','0',1);
CALL osae_sp_object_type_property_add('CONTROL SCREEN OBJECTS','ZOrder','Integer','','0',1);

CALL osae_sp_object_type_add ('BLUETOOTH','Bluetooth Plugin','Bluetooth','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('BLUETOOTH','ON','Running');
CALL osae_sp_object_type_state_add('BLUETOOTH','OFF','Stopped');
CALL osae_sp_object_type_event_add('BLUETOOTH','ON','Started');
CALL osae_sp_object_type_event_add('BLUETOOTH','OFF','Stopped');
CALL osae_sp_object_type_method_add('BLUETOOTH','ON','Start','','','','');
CALL osae_sp_object_type_method_add('BLUETOOTH','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add('BLUETOOTH','Scan Interval','Integer','','60',0);
CALL osae_sp_object_type_property_add('BLUETOOTH','Discover Length','Integer','','8',0);
CALL osae_sp_object_type_property_add('BLUETOOTH','Learning Mode','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('BLUETOOTH','System Plugin','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('BLUETOOTH','Debug','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('BLUETOOTH','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add('BLUETOOTH','Version','String','','',0);
CALL osae_sp_object_type_property_add('BLUETOOTH','Author','String','','',0);
CALL osae_sp_object_type_property_delete('Computer Name','BLUETOOTH');

CALL osae_sp_object_type_add ('EMAIL','Email Plugin','Email','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('EMAIL','ON','Running');
CALL osae_sp_object_type_state_add('EMAIL','OFF','Stopped');
CALL osae_sp_object_type_event_add('EMAIL','ON','Started');
CALL osae_sp_object_type_event_add('EMAIL','OFF','Stopped');
CALL osae_sp_object_type_event_add('EMAIL','EMAIL SENT','Email Sent');
CALL osae_sp_object_type_method_add('EMAIL','ON','Start','','','','');
CALL osae_sp_object_type_method_add('EMAIL','OFF','Stop','','','','');
CALL osae_sp_object_type_method_add('EMAIL','SEND EMAIL','Send Email','TO','Message','','Test Message');
CALL osae_sp_object_type_property_add('EMAIL','SMTP Server','String','','',0);
CALL osae_sp_object_type_property_add('EMAIL','SMTP Port','String','','25',0);
CALL osae_sp_object_type_property_add('EMAIL','ssl','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('EMAIL','Username','String','','',0);
CALL osae_sp_object_type_property_add('EMAIL','Password','String','','',0);
CALL osae_sp_object_type_property_add('EMAIL','From Address','String','','',0);
CALL osae_sp_object_type_property_add('EMAIL','System Plugin','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('EMAIL','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add('EMAIL','Version','String','','',0);
CALL osae_sp_object_type_property_add('EMAIL','Author','String','','',0);

CALL osae_sp_object_type_add ('JABBER','Jabber Plugin','Jabber','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add('JABBER','ON','Running');
CALL osae_sp_object_type_state_add('JABBER','OFF','Stopped');
CALL osae_sp_object_type_event_add('JABBER','ON','Started');
CALL osae_sp_object_type_event_add('JABBER','OFF','Stopped');
CALL osae_sp_object_type_method_add('JABBER','SEND MESSAGE','Send Message','To','Message','','');
CALL osae_sp_object_type_method_add('JABBER','SEND FROM LIST','Send From List','To','List','','');
CALL osae_sp_object_type_method_add('JABBER','SEND QUESTION','Send Question','To','','','');
CALL osae_sp_object_type_method_add('JABBER','OFF','Stop','','','','');
CALL osae_sp_object_type_method_add('JABBER','ON','Start','','','','');
CALL osae_sp_object_type_property_add('JABBER','Username','String','','',0);
CALL osae_sp_object_type_property_add('JABBER','Password','String','','',0);
CALL osae_sp_object_type_property_add('JABBER','System Plugin','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('JABBER','Debug','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('JABBER','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add('JABBER','Version','String','','',0);
CALL osae_sp_object_type_property_add('JABBER','Author','String','','',0);

CALL osae_sp_object_type_add ('NETWORK MONITOR','Network Monitor Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('NETWORK MONITOR','ON','Running');
CALL osae_sp_object_type_state_add('NETWORK MONITOR','OFF','Stopped');
CALL osae_sp_object_type_event_add('NETWORK MONITOR','ON','Started');
CALL osae_sp_object_type_event_add('NETWORK MONITOR','OFF','Stopped');
CALL osae_sp_object_type_method_add('NETWORK MONITOR','ON','Start','','','','');
CALL osae_sp_object_type_method_add('NETWORK MONITOR','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','System Plugin','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Poll Interval','Integer','','30',0);
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Debug','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Version','String','','',0);
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Author','String','','',0);
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Trust Level','Integer','','90',0);

CALL osae_sp_object_type_add ('POWERSHELL','PowerShell Script Processor','','PLUGIN',0,0,0,1);
CALL osae_sp_object_type_state_add ('POWERSHELL','ON','Running');
CALL osae_sp_object_type_state_add ('POWERSHELL','OFF','Stopped');
CALL osae_sp_object_type_event_add ('POWERSHELL','ON','Started');
CALL osae_sp_object_type_event_add ('POWERSHELL','OFF','Stopped');
CALL osae_sp_object_type_method_add ('POWERSHELL','ON','Start','','','','');
CALL osae_sp_object_type_method_add ('POWERSHELL','OFF','Stop','','','','');
CALL osae_sp_object_type_method_add ('POWERSHELL','RUN SCRIPT','RUN SCRIPT','','','','');
CALL osae_sp_script_processor_add ('PowerShell','PowerShell');
CALL osae_sp_object_type_property_add ('POWERSHELL','System Plugin','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add ('POWERSHELL','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add ('POWERSHELL','Version','String','','',0);
CALL osae_sp_object_type_property_add ('POWERSHELL','Author','String','','',0);

CALL osae_sp_object_type_add ('REST','Rest API','Rest','PLUGIN',0,0,0,1);
CALL osae_sp_object_type_state_add('REST','OFF','Stopped');
CALL osae_sp_object_type_state_add('REST','ON','Running');
CALL osae_sp_object_type_event_add('REST','OFF','Stopped');
CALL osae_sp_object_type_event_add('REST','ON','Started');
CALL osae_sp_object_type_method_add('REST','OFF','Stop','','','','');
CALL osae_sp_object_type_method_add('REST','ON','Start','','','','');
CALL osae_sp_object_type_property_add('REST','System Plugin','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('REST','Show Help','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('REST','Rest Port','Integer','','8732',0);
CALL osae_sp_object_type_property_add('REST','Version','String','','',0);
CALL osae_sp_object_type_property_add('REST','Author','String','','',0);
CALL osae_sp_object_type_property_add('REST','Trust Level','Integer','','90',0);

CALL osae_sp_object_type_add ('SCRIPT PROCESSOR','Generic Plugin Class','Script Processor','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('SCRIPT PROCESSOR','ON','Running');
CALL osae_sp_object_type_state_add('SCRIPT PROCESSOR','OFF','Stopped');
CALL osae_sp_object_type_event_add('SCRIPT PROCESSOR','ON','Started');
CALL osae_sp_object_type_event_add('SCRIPT PROCESSOR','OFF','Stopped');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','ON','Start','','','','');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','OFF','Stop','','','','');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','RUN SCRIPT','Run Script','','','','');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','RUN READERS','Run Readers','','','','');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','System Plugin','Boolean','','TRUE',0);
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Debug','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Version','String','','',0);
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Author','String','','',0);

CALL osae_sp_object_type_add ('SPEECH','Generic Plugin Class','Speech','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('SPEECH','ON','Running');
CALL osae_sp_object_type_state_add('SPEECH','OFF','Stopped');
CALL osae_sp_object_type_event_add('SPEECH','ON','Started');
CALL osae_sp_object_type_event_add('SPEECH','OFF','Stopped');
CALL osae_sp_object_type_method_add('SPEECH','ON','On','','','','');
CALL osae_sp_object_type_method_add('SPEECH','OFF','Off','','','','');
CALL osae_sp_object_type_method_add('SPEECH','SPEAK','Say','Message','','Hello','');
CALL osae_sp_object_type_method_add('SPEECH','SPEAKFROM','Say From List','Object Name','Property Name','Speech List','Greetings');
CALL osae_sp_object_type_method_add('SPEECH','PLAY','Play','File','','','');
CALL osae_sp_object_type_method_add('SPEECH','PLAYFROM','Play From List','List','','','');
CALL osae_sp_object_type_method_add('SPEECH','STOP','Stop','','','','');
CALL osae_sp_object_type_method_add('SPEECH','PAUSE','Pause','','','','');
CALL osae_sp_object_type_method_add('SPEECH','MUTEVR','Mute the Microphone','','','','');
CALL osae_sp_object_type_method_add('SPEECH','SETVOICE','Set Voice','Voice','','Anna','');
CALL osae_sp_object_type_method_add('SPEECH','SETTTSRATE','Set TTS Rate','Rate','','0','');
CALL osae_sp_object_type_method_add('SPEECH','SETTTSVOLUME','Set TTS Volume','Volume','','100','');
CALL osae_sp_object_type_property_add('SPEECH','Voice','String','','',0);
CALL osae_sp_object_type_property_add('SPEECH','Voices','List','','',0);
CALL osae_sp_object_type_property_add('SPEECH','System Plugin','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('SPEECH','TTS Rate','Integer','','',0);
CALL osae_sp_object_type_property_add('SPEECH','TTS Volume','Integer','','',0);
CALL osae_sp_object_type_property_add('SPEECH','Speaking','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('SPEECH','Debug','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('SPEECH','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add('SPEECH','Version','String','','',0);
CALL osae_sp_object_type_property_add('SPEECH','Author','String','','',0);
CALL osae_sp_object_type_property_delete('Computer Name','SPEECH');

CALL osae_sp_object_type_property_add('WEB SERVER','Hide Controls','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Timeout','Integer','','60',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Version','String','','',0);
CALL osae_sp_object_type_property_add('WEB SERVER','Author','String','','',0);

CALL osae_sp_object_type_property_add('WUNDERGROUND','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add('WUNDERGROUND','Debug','Boolean','','FALSE',0);
CALL osae_sp_object_type_property_add('WUNDERGROUND','Version','String','','',0);
CALL osae_sp_object_type_property_add('WUNDERGROUND','Author','String','','',0);


UPDATE osae_object_type_property SET property_default = '0' WHERE property_datatype = 'Integer' and (property_default = '' or property_default IS NULL);
UPDATE osae_object_type_property SET property_default = '-1' WHERE property_datatype = 'Integer' and property_name = "Off Timer" and (property_default = '' or property_default = "0" or property_default IS NULL);
UPDATE osae_object_property SET property_value = '0' where (property_value IS NULL or property_value = '') and object_type_property_id IN (SELECT property_id FROM osae_object_type_property WHERE property_datatype = 'Integer');
UPDATE osae_object_property SET property_value = '-1' where (property_value IS NULL or property_value = '' or property_value = '0') and object_type_property_id IN (SELECT property_id FROM osae_object_type_property WHERE UPPER(property_name) = 'OFF TIMER');