-- recommit

--
-- Disable foreign keys
--
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

USE osae;

DELIMITER $$

--
-- Drop function "osae_fn_lookup_object_id"
--
DROP FUNCTION IF EXISTS osae_fn_lookup_object_id$$

DELIMITER ;

--
-- Alter table "osae_object_type"
--
ALTER TABLE osae_object_type
  ADD COLUMN object_type_tooltip VARCHAR(255) DEFAULT NULL AFTER hide_redundant_events;

--
-- Alter table "osae_object_type_event"
--
ALTER TABLE osae_object_type_event
  ADD COLUMN event_tooltip VARCHAR(255) DEFAULT NULL AFTER object_type_id;

--
-- Alter table "osae_object_type_method"
--
ALTER TABLE osae_object_type_method
  ADD COLUMN method_tooltip VARCHAR(255) DEFAULT NULL AFTER param_2_default;

--
-- Alter table "osae_object_type_property"
--
ALTER TABLE osae_object_type_property
  ADD COLUMN property_tooltip VARCHAR(255) DEFAULT NULL AFTER property_object_type_id,
  ADD COLUMN property_required TINYINT(1) NOT NULL DEFAULT -1 AFTER property_tooltip;

--
-- Alter table "osae_object_type_state"
--
ALTER TABLE osae_object_type_state
  ADD COLUMN state_tooltip VARCHAR(255) DEFAULT NULL AFTER object_type_id;

--
-- Alter table "osae_object_property"
--
ALTER TABLE osae_object_property
  ADD COLUMN property_integer INT(11) DEFAULT NULL AFTER interest_level,
  ADD COLUMN propery_decimal DECIMAL(10, 4) DEFAULT NULL AFTER property_integer,
  ADD COLUMN property_boolean INT(1) DEFAULT NULL AFTER propery_decimal;

--
-- Alter table "osae_object_property_history"
--
ALTER TABLE osae_object_property_history
  ADD COLUMN property_integer INT(11) DEFAULT NULL AFTER property_value,
  ADD COLUMN property_decimal DECIMAL(12, 4) DEFAULT NULL AFTER property_integer,
  ADD COLUMN property_boolean INT(1) DEFAULT NULL AFTER property_decimal,
  ADD COLUMN property_float FLOAT DEFAULT NULL AFTER property_boolean;

DELIMITER $$

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
-- Alter procedure "osae_sp_object_property_set"
--
DROP PROCEDURE osae_sp_object_property_set$$
CREATE PROCEDURE osae_sp_object_property_set(IN pname varchar(200), IN pproperty varchar(200), IN pvalue varchar(4000), IN pfromobject varchar(200), IN pdebuginfo varchar(2000))
BEGIN
DECLARE vDebugTrace VARCHAR(2000) DEFAULT '';
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
DECLARE vTrustLevelExists INT DEFAULT 0;
DECLARE vEventCount INT;
  #This proc runs thousands of times and must use efficient SQL, edit out the use of generic views
  SET vDebugTrace = CONCAT(pdebuginfo,' -> object_property_set');
  # 049 The following function was improved to not use the v_object view and should save likes of work
  SET vObjectCount = osae_fn_object_exists(pfromobject);
  IF vObjectCount = 1 THEN
    # 049 View replaced with efficient sql below 
    SET vPropertyCount = osae_fn_object_property_exists(pname, pproperty);
    IF vPropertyCount > 0 THEN
      # 049 Below call optomized to replace all views
      SET vTrustLevelExists = osae_fn_trust_level_property_exists(pfromobject);
      SET vObjectID = osae_fn_object_getid(pname);
      SELECT object_type_id,min_trust_level INTO vObjectTypeID,vMinTrustLevel FROM osae_object WHERE object_id=vObjectID;
      SELECT trust_level,object_type_property_id INTO vOldTrustLevel,vObjectTypePropertyID FROM osae_v_object_property WHERE object_id=vObjectID AND property_name=pproperty AND (property_value IS NULL OR property_value != pvalue);        
      SELECT property_value INTO vNewTrustLevel FROM osae_v_object_property WHERE object_name=pfromobject AND property_name='TRUST LEVEL';
      SELECT property_id,COALESCE(property_value,'') INTO vPropertyID, vPropertyValue FROM osae_v_object_property WHERE object_id=vObjectID AND property_name=pproperty AND (property_value IS NULL OR property_value != pvalue);
      #Insert Trust Level Rejection Code Here, maybe shppech command until conversation tracking is in
      IF vNewTrustLevel >= vMinTrustLevel THEN
        UPDATE osae_object_property SET property_value=pvalue,trust_level=vNewTrustLevel,source_name=pfromobject,interest_level=0 WHERE object_id=vObjectID AND object_type_property_id=vPropertyID;
        SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND event_name=pproperty;
        IF vEventCount > 0 THEN  
          CALL osae_sp_event_log_add(pname,pproperty,pfromobject,vDebugTrace,pvalue,NULL);
        END IF;
        #Since this property has changed, it has generated interest
        #Review the where clause, don't remember why it has to be null to get this update
        UPDATE osae_object_property SET interest_level = interest_level + 1 WHERE object_type_property_id = vObjectTypePropertyID AND (property_value IS NULL OR property_value = '');
      ELSE
        CALL osae_sp_debug_log_add(CONCAT(pfromobject, ' tried to set properties on ',pname,', but lacks sufficient trust'),'object_property_set'); 
      END IF;
        END IF;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_object_type_add"
--
DROP PROCEDURE osae_sp_object_type_add$$
CREATE PROCEDURE osae_sp_object_type_add(IN pname VARCHAR(200), IN pdesc VARCHAR(200), IN pownedby VARCHAR(200), IN pbasetype VARCHAR(200), IN ptypeowner TINYINT, IN psystem TINYINT, IN pcontainer TINYINT, IN phideredundantevents TINYINT, IN ptooltip VARCHAR(255))
BEGIN
DECLARE vOwnerTypeCount INT;
DECLARE vOwnerTypeID INT Default NULL;
DECLARE vBaseTypeCount INT;
DECLARE vBaseTypeID INT Default NULL;
    SELECT COUNT(object_name) INTO vOwnerTypeCount FROM osae_v_object WHERE object_name=pownedby;
    IF vOwnerTypeCount > 0 THEN
        SELECT object_id INTO vOwnerTypeID FROM osae_v_object WHERE object_name=pownedby;
    END IF; 
    SELECT COUNT(object_type) INTO vBaseTypeCount FROM osae_v_object_type WHERE object_type=pbasetype;
    IF vBaseTypeCount > 0 THEN
        SELECT object_type_id INTO vBaseTypeID FROM osae_v_object_type WHERE object_type=pbasetype;
    END IF; 
    INSERT INTO osae_object_type (object_type,object_type_description,plugin_object_id,base_type_id,system_hidden,object_type_owner,container,hide_redundant_events,object_type_tooltip) VALUES(UPPER(pname),pdesc,vOwnerTypeID,vBaseTypeID,psystem,ptypeowner,pcontainer,phideredundantevents,ptooltip) ON DUPLICATE KEY UPDATE object_type_description=pdesc,plugin_object_id=vOwnerTypeID,base_type_id=vBaseTypeID,system_hidden=psystem,object_type_owner=ptypeowner,container=pcontainer,hide_redundant_events=phideredundantevents,object_type_tooltip=ptooltip;
    IF vBaseTypeCount = 0 THEN
        SELECT object_type_id INTO vBaseTypeID FROM osae_object_type WHERE object_type=UPPER(pname);
        UPDATE osae_object_type SET base_type_id=vBaseTypeID WHERE object_type_id=vBaseTypeID;
    END IF;
END
$$

--
-- Alter procedure "osae_sp_object_type_clone"
--
DROP PROCEDURE osae_sp_object_type_clone$$
CREATE PROCEDURE osae_sp_object_type_clone(IN pnewname varchar(200), IN pbasename varchar(200))
BEGIN
DECLARE vBaseTypeID INT DEFAULT 0; 
DECLARE vNewTypeID INT;
    SELECT object_type_id INTO vBaseTypeID FROM osae_v_object_type WHERE object_type=pbasename;
    IF vBaseTypeID != 0 THEN
      # CALL OBJECT_TYPE_DOES_NOT_EXIST();
   # ELSE
      INSERT INTO osae_object_type (object_type,object_type_description,plugin_object_id,system_hidden,object_type_owner,base_type_id,container,object_type_tooltip) SELECT pnewname,t.object_type_description,t.plugin_object_id,t.system_hidden,t.object_type_owner,t.base_type_id,t.container,t.object_type_tooltip FROM osae_object_type t WHERE object_type=pbasename;
      SELECT object_type_id INTO vNewTypeID FROM osae_object_type WHERE object_type=pnewname;
      INSERT INTO osae_object_type_state (state_name,state_label,state_tooltip,object_type_id) SELECT state_name,state_label,state_tooltip,vNewTypeID FROM osae_object_type_state t WHERE object_type_id=vBaseTypeID;
      INSERT INTO osae_object_type_event (event_name,event_label,event_tooltip,object_type_id) SELECT event_name,event_label,event_tooltip,vNewTypeID FROM osae_object_type_event t WHERE object_type_id=vBaseTypeID;
      INSERT INTO osae_object_type_method (method_name,method_label,method_tooltip,object_type_id) SELECT method_name,method_label,method_tooltip,vNewTypeID FROM osae_object_type_method t WHERE object_type_id=vBaseTypeID;
      INSERT INTO osae_object_type_property (property_name,property_datatype,property_tooltip,object_type_id) SELECT property_name,property_datatype,property_tooltip,vNewTypeID FROM osae_object_type_property t WHERE object_type_id=vBaseTypeID;
    END IF;
END
$$

--
-- Alter procedure "osae_sp_object_type_event_add"
--
DROP PROCEDURE osae_sp_object_type_event_add$$
CREATE PROCEDURE osae_sp_object_type_event_add(IN pobjecttype VARCHAR(200), IN pname VARCHAR(200), IN plabel VARCHAR(200), IN ptooltip VARCHAR(255))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        INSERT INTO osae_object_type_event (event_name,event_label,event_tooltip,object_type_id) VALUES(UPPER(pname),plabel,ptooltip,vObjectTypeID) ON DUPLICATE KEY UPDATE event_label=plabel,event_tooltip=ptooltip,object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_object_type_event_update"
--
DROP PROCEDURE osae_sp_object_type_event_update$$
CREATE PROCEDURE osae_sp_object_type_event_update(IN poldname VARCHAR(200), IN pnewname VARCHAR(200), IN plabel VARCHAR(200), IN pobjecttype VARCHAR(200), IN ptooltip VARCHAR(255))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        UPDATE osae_object_type_event SET event_name=UPPER(pnewname),event_label=plabel,event_tooltip=ptooltip WHERE event_name=UPPER(poldname) AND object_type_id=vObjectTypeID;
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
  DECLARE vRequired INT;
  DECLARE vTooltip VARCHAR(255);
  DECLARE vParam1Name VARCHAR(200);
  DECLARE vParam1Default VARCHAR(200);
  DECLARE vParam2Name VARCHAR(200);
  DECLARE vParam2Default VARCHAR(200);
  DECLARE vDataType VARCHAR(200);
  DECLARE vDefault VARCHAR(200);
  DECLARE vTrackHistory VARCHAR(200);
  DECLARE vPropertyObjectType VARCHAR(200);
  DECLARE vPropertyOption VARCHAR(200);
  DECLARE vObjectTypeTooltip VARCHAR(255);

  DECLARE state_cursor CURSOR FOR SELECT state_name,state_label,state_tooltip FROM osae_v_object_type_state WHERE object_type=vObjectType;
  DECLARE event_cursor CURSOR FOR SELECT event_name,event_label,event_tooltip FROM osae_v_object_type_event WHERE object_type=vObjectType;
  DECLARE method_cursor CURSOR FOR SELECT method_name,method_label,param_1_label,param_1_default,param_2_label,param_2_default,method_tooltip FROM osae_v_object_type_method WHERE object_type=vObjectType;
  DECLARE property_cursor CURSOR FOR SELECT property_name,property_datatype,property_default,property_object_type,track_history,property_required,property_tooltip FROM osae_v_object_type_property WHERE object_type=vObjectType;
  DECLARE property_option_cursor CURSOR FOR SELECT option_name FROM osae_v_object_type_property_option WHERE object_type=vObjectType and property_name=vname;

  DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_finished = TRUE;

  #SET vObjectType = CONCAT(objectName,'2');

  SELECT object_type,object_type_description,COALESCE(object_name,''),base_type,object_type_owner,system_hidden,container,hide_redundant_events,COALESCE(object_type_tooltip,'') INTO vObjectType,vDescription,vOwner,vBaseType,vTypeOwner,vSystemType,vContainer,vHideRedundant,vObjectTypeTooltip FROM osae_v_object_type WHERE object_type=pObjectType;
  SET vResults = CONCAT('CALL osae_sp_object_type_add (\'', REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vDescription,'\'','\\\''),'\',\'',vOwner,'\',\'',vBaseType,'\',',vTypeOwner,',', vSystemType,',',vContainer,',',vHideRedundant,',\'',vObjectTypeTooltip,'\');','\r\n');

  OPEN state_cursor;
    get_states: LOOP
    SET v_finished = FALSE;
      FETCH state_cursor INTO vName,vLabel,vTooltip;
      IF v_finished OR vName IS NULL THEN 
        LEAVE get_states;
      END IF;
      SET vResults = CONCAT(vResults,'CALL osae_sp_object_type_state_add(\'',REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vName,'\'','\\\''),'\',\'',REPLACE(vLabel,'\'','\\\''),'\',\'',REPLACE(vTooltip,'\'','\\\''),'\');','\r\n');
    END LOOP get_states;
  CLOSE state_cursor;


  OPEN event_cursor;
  get_events: LOOP
    SET v_finished = FALSE;
    FETCH event_cursor INTO vName,vLabel,vTooltip;
    IF v_finished THEN 
      LEAVE get_events;
    END IF;
    SET vResults = CONCAT(vResults,'CALL osae_sp_object_type_event_add(\'',REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vName,'\'','\\\''),'\',\'',REPLACE(vLabel,'\'','\\\''),'\',\'',REPLACE(vTooltip,'\'','\\\''),'\');','\r\n');
  END LOOP get_events;
  CLOSE event_cursor;

  OPEN method_cursor;
  get_methods: LOOP
    SET v_finished = FALSE;
    FETCH method_cursor INTO vName,vLabel,vParam1Name,vParam1Default,vParam2Name,vParam2Default,vTooltip;
    IF v_finished OR vName IS NULL THEN 
      LEAVE get_methods;
    END IF;
    SET vResults = CONCAT(vResults,'CALL osae_sp_object_type_method_add(\'',REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vName,'\'','\\\''),'\',\'',REPLACE(vLabel,'\'','\\\''),'\',\'',vParam1Name,'\',\'',vParam2Name,'\',\'',vParam1Default,'\',\'',vParam2Default,'\',\'',REPLACE(vTooltip,'\'','\\\''),'\');','\r\n');
  END LOOP get_methods;
  CLOSE method_cursor;
  SET v_finished = 0;

  OPEN property_cursor;
  get_properties: LOOP
    SET v_finished = FALSE;
    FETCH property_cursor INTO vName,vDataType,vDefault,vPropertyObjectType,vTrackHistory,vRequired,vTooltip;
    IF v_finished THEN 
      LEAVE get_properties;
    END IF;
    SET vResults = CONCAT(vResults,'CALL osae_sp_object_type_property_add(\'',REPLACE(vObjectType,'\'','\\\''),'\',\'',REPLACE(vName,'\'','\\\''),'\',\'',vDataType,'\',\'',vPropertyObjectType,'\',\'',vDefault,'\',',vTrackHistory,',',vRequired,',\'',REPLACE(vTooltip,'\'','\\\''),'\');','\r\n');
  
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
-- Alter procedure "osae_sp_object_type_method_add"
--
DROP PROCEDURE osae_sp_object_type_method_add$$
CREATE PROCEDURE osae_sp_object_type_method_add(IN pobjecttype VARCHAR(200), IN pname VARCHAR(200), IN plabel VARCHAR(200), IN pparam1 VARCHAR(100), IN pparam2 VARCHAR(100), IN pparam1default VARCHAR(1024), IN pparam2default VARCHAR(1024), IN ptooltip VARCHAR(255))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        INSERT INTO osae_object_type_method (method_name,method_label,object_type_id,param_1_label,param_2_label,param_1_default,param_2_default,method_tooltip) VALUES(UPPER(pname),plabel,vObjectTypeID,pparam1,pparam2,pparam1default,pparam2default,ptooltip) ON DUPLICATE KEY UPDATE method_label=plabel,object_type_id=vObjectTypeID,param_1_label=pparam1,param_2_label=pparam2,param_1_default=pparam1default,param_2_default=pparam2default,method_tooltip=ptooltip;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_object_type_method_update"
--
DROP PROCEDURE osae_sp_object_type_method_update$$
CREATE PROCEDURE osae_sp_object_type_method_update(IN poldname VARCHAR(200), IN pnewname VARCHAR(200), IN plabel VARCHAR(200), IN pobjecttype VARCHAR(200), IN pparam1 VARCHAR(100), IN pparam2 VARCHAR(100), IN pparam1default VARCHAR(1024), IN pparam2default VARCHAR(1024), IN ptooltip VARCHAR(255))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount = 1 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        UPDATE osae_object_type_method SET method_name=UPPER(pnewname),method_label=plabel,param_1_label=pparam1,param_2_label=pparam2,param_1_default=pparam1default,param_2_default=pparam2default,method_tooltip=ptooltip WHERE method_name=UPPER(poldname) AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_object_type_property_add"
--
DROP PROCEDURE osae_sp_object_type_property_add$$
CREATE PROCEDURE osae_sp_object_type_property_add(IN pobjecttype VARCHAR(200), IN ppropertyname VARCHAR(200), IN ppropertytype VARCHAR(50), IN ppropertyobjecttype VARCHAR(200), IN pdefault VARCHAR(255), IN ptrackhistory TINYINT(1), IN prequired TINYINT(1), IN ptooltip VARCHAR(255))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
DECLARE vPropertyObjectTypeCount INT;
DECLARE vPropertyObjectTypeID INT;

    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        IF UPPER(ppropertytype) = 'OBJECTTYPE' THEN
            SELECT COUNT(object_type_id) INTO vPropertyObjectTypeCount FROM osae_object_type WHERE UPPER(object_type)=UPPER(ppropertyobjecttype);
            IF vPropertyObjectTypeCount > 0 THEN
                SELECT object_type_id INTO vPropertyObjectTypeID FROM osae_object_type WHERE UPPER(object_type)=UPPER(ppropertyobjecttype);
            END IF;
        END IF;
        INSERT INTO osae_object_type_property (property_name,property_datatype,property_object_type_id,property_default,object_type_id,track_history,property_required,property_tooltip) VALUES(ppropertyname,ppropertytype,vPropertyObjectTypeID,pdefault,vObjectTypeID,ptrackhistory,prequired,ptooltip) ON DUPLICATE KEY UPDATE property_datatype=ppropertytype,object_type_id=vObjectTypeID,property_required=prequired,property_tooltip=ptooltip;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_object_type_property_option_add"
--
DROP PROCEDURE osae_sp_object_type_property_option_add$$
CREATE PROCEDURE osae_sp_object_type_property_option_add(IN pobjecttype VARCHAR(200),
                                                  IN pproperty   VARCHAR(200),
                                                  IN pvalue      VARCHAR(200)
                                                  )
BEGIN
  DECLARE vObjectTypePropertyID INT;

  SELECT property_id INTO vObjectTypePropertyID FROM  osae_v_object_type_property
  WHERE upper(object_type) = upper(pobjecttype) AND upper(property_name) = upper(pproperty);
  IF vObjectTypePropertyID IS NOT NULL THEN
      INSERT INTO osae_object_type_property_option (option_name, property_id) VALUES (pvalue, vObjectTypePropertyID) ON DUPLICATE KEY UPDATE option_name = pvalue;
  END IF;
END
$$

--
-- Alter procedure "osae_sp_object_type_property_update"
--
DROP PROCEDURE osae_sp_object_type_property_update$$
CREATE PROCEDURE osae_sp_object_type_property_update(IN poldname VARCHAR(200), IN pnewname VARCHAR(200), IN pparamtype VARCHAR(50), IN ppropertyobjecttype VARCHAR(200), IN pdefault VARCHAR(255), IN pobjecttype VARCHAR(200), IN ptrackhistory TINYINT(1), IN prequired TINYINT(1), IN ptooltip VARCHAR(255))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
DECLARE vPropertyObjectTypeCount INT;
DECLARE vPropertyObjectTypeID INT;

    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        IF UPPER(pparamtype) = 'OBJECT TYPE' THEN
            SELECT COUNT(object_type_id) INTO vPropertyObjectTypeCount FROM osae_object_type WHERE object_type=ppropertyobjecttype;
            IF vPropertyObjectTypeCount > 0 THEN
                SELECT object_type_id INTO vPropertyObjectTypeID FROM osae_object_type WHERE object_type=ppropertyobjecttype;
            END IF;
        END IF;

        IF vObjectTypeCount > 0 THEN
            SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=ppropertyobjecttype;
        END IF;
        UPDATE osae_object_type_property SET property_name=pnewname,property_datatype=pparamtype,property_object_type_id=vPropertyObjectTypeID,property_default=pdefault,property_tooltip=ptooltip,track_history=ptrackhistory,property_required=prequired WHERE property_name=poldname AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_object_type_state_add"
--
DROP PROCEDURE osae_sp_object_type_state_add$$
CREATE PROCEDURE osae_sp_object_type_state_add(IN pobjecttype VARCHAR(200), IN pname VARCHAR(200), IN plabel VARCHAR(200), IN ptooltip VARCHAR(255))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        INSERT INTO osae_object_type_state (state_name,state_label,state_tooltip,object_type_id) VALUES(UPPER(pname),plabel,ptooltip,vObjectTypeID) ON DUPLICATE KEY UPDATE state_label=plabel,state_tooltip=ptooltip,object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_object_type_state_update"
--
DROP PROCEDURE osae_sp_object_type_state_update$$
CREATE PROCEDURE osae_sp_object_type_state_update(IN poldname VARCHAR(200), IN pnewname VARCHAR(200), IN plabel VARCHAR(200), IN pobjecttype VARCHAR(200), IN ptooltip VARCHAR(255))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        UPDATE osae_object_type_state SET state_name=UPPER(pnewname),state_label=plabel,state_tooltip=ptooltip WHERE state_name=UPPER(poldname) AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Alter procedure "osae_sp_object_type_update"
--
DROP PROCEDURE osae_sp_object_type_update$$
CREATE PROCEDURE osae_sp_object_type_update(IN poldname VARCHAR(200), IN pnewname VARCHAR(200), IN pdesc VARCHAR(200), IN pownedby VARCHAR(200), IN pbasetype VARCHAR(200), IN ptypeowner TINYINT, IN psystem TINYINT, IN pcontainer TINYINT, IN phideredundantevents TINYINT, IN ptooltip VARCHAR(255))
BEGIN
DECLARE vOwnerTypeCount INT;
DECLARE vOwnerTypeID INT Default NULL;
DECLARE vBaseTypeCount INT;
DECLARE vBaseTypeID INT Default NULL;
    SELECT COUNT(object_name) INTO vOwnerTypeCount FROM osae_v_object WHERE UPPER(object_name)=UPPER(pownedby);
    IF vOwnerTypeCount = 1 THEN
        SELECT object_id INTO vOwnerTypeID FROM osae_v_object WHERE  UPPER(object_name)=UPPER(pownedby);
    END IF; 
    SELECT COUNT(object_type) INTO vBaseTypeCount FROM osae_v_object_type WHERE object_type=pbasetype;
    IF vBaseTypeCount = 1 THEN
        SELECT object_type_id INTO vBaseTypeID FROM osae_v_object_type WHERE object_type=pbasetype;
    END IF;     
    UPDATE osae_object_type SET object_type=UPPER(pnewname),object_type_description=pdesc,plugin_object_id=vOwnerTypeID,base_type_id=vBaseTypeID,system_hidden=psystem,object_type_owner=ptypeowner,container=pcontainer,hide_redundant_events=phideredundantevents,object_type_tooltip=ptooltip WHERE object_type=poldname;
END
$$

--
-- Alter procedure "osae_sp_screen_object_add"
--
DROP PROCEDURE osae_sp_screen_object_add$$
CREATE PROCEDURE osae_sp_screen_object_add(IN pscreenname varchar(200),
IN pobjectname varchar(200),
IN pcontrolname varchar(200))
BEGIN
  DECLARE vScreenID int;
  DECLARE vObjectID int;
  DECLARE vControlID int;
  SELECT
    osae_fn_object_getid(pscreenname) INTO vScreenID;
  SELECT
    osae_fn_object_getid(pobjectname) INTO vObjectID;
  SELECT
    osae_fn_object_getid(pcontrolname) INTO vControlID;
  # TODO - Add Duplicate Check, working on Constraint now
  INSERT INTO osae_screen_object (screen_id, object_id, control_id)
    VALUES (vScreenID, vObjectID, vControlID);
END
$$

--
-- Alter procedure "osae_sp_screen_object_delete"
--
DROP PROCEDURE osae_sp_screen_object_delete$$
CREATE PROCEDURE osae_sp_screen_object_delete(IN pscreenname varchar(200),
IN pobjectname varchar(200),
IN pcontrolname varchar(200))
BEGIN
  DECLARE vScreenID int;
  DECLARE vObjectID int;
  DECLARE vControlID int;
  SELECT
    osae_fn_object_getid(pscreenname) INTO vScreenID;
  SELECT
    osae_fn_object_getid(pobjectname) INTO vObjectID;
  SELECT
    osae_fn_object_getid(pcontrolname) INTO vControlID;
  DELETE
    FROM osae_screen_object
  WHERE screen_id = vScreenID
    AND object_id = vObjectID
    AND control_id = vControlID;
END
$$

--
-- Alter procedure "osae_sp_screen_object_update"
--
DROP PROCEDURE osae_sp_screen_object_update$$
CREATE PROCEDURE osae_sp_screen_object_update(IN pscreenname varchar(200),
IN pobjectname varchar(200),
IN pcontrolname varchar(200))
BEGIN
  DECLARE vScreenID int;
  DECLARE vObjectID int;
  DECLARE vControlID int;
  SELECT
    osae_fn_object_getid(pscreenname) INTO vScreenID;
  SELECT
    osae_fn_object_getid(pobjectname) INTO vObjectID;
  SELECT
    osae_fn_object_getid(pcontrolname) INTO vControlID;
  UPDATE osae_screen_object
  SET object_id = vObjectID
  WHERE screen_id = vScreenID
  AND control_id = vControlID;
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
# 049 Optomised below, making the new view, which is as light weight as possible, but maybe should be a proc, not a view.
DECLARE curs CURSOR FOR SELECT object_name FROM osae_v_system_plugins_errored;
DECLARE CONTINUE HANDLER FOR NOT FOUND SET bDone = 1;
    SET vOldCount = (SELECT property_value FROM osae_v_object_property WHERE object_name='SYSTEM' AND property_name='Plugins Errored');  
    # 049 the count should be able to be gotten from the above cursor source view, not v_object, modifyinh the view above.
    #SET iPluginErrorCount = (SELECT COUNT(object_name) FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1 AND state_name='OFF' AND container_state_name='ON');
    SET iPluginErrorCount = (SELECT COUNT(object_name) FROM osae_v_system_plugins_errored);
 
    IF vOldCount != iPluginErrorCount THEN  
        #SET iPluginCount = (SELECT COUNT(object_id) FROM osae_v_object WHERE base_type='PLUGIN');
        SET iPluginCount = osae_fn_plugin_count();
        #SET iPluginEnabledCount = (SELECT COUNT(object_id) FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1);
        SET iPluginEnabledCount = osae_fn_plugin_enabled_count();
        #SET iPluginRunningCount = (SELECT COUNT(object_id) FROM osae_v_object WHERE base_type='PLUGIN' AND enabled=1 AND state_name='ON');
        SET iPluginRunningCount = osae_fn_plugin_running_count();

        CALL osae_sp_object_property_set('SYSTEM','Plugins Found',iPluginCount,'SYSTEM','system_count_plugins');
        CALL osae_sp_object_property_set('SYSTEM','Plugins Enabled',iPluginEnabledCount,'SYSTEM','system_count_plugins');
        CALL osae_sp_object_property_set('SYSTEM','Plugins Running',iPluginRunningCount,'SYSTEM','system_count_plugins');
        CALL osae_sp_object_property_set('SYSTEM','Plugins Errored',iPluginErrorCount,'SYSTEM','system_count_plugins');

        CASE iPluginErrorCount
          WHEN 0 THEN 
            SET vOutput = 'All Plugins are Running';
            CALL osae_sp_object_property_set('SYSTEM','Plugins',vOutput,'SYSTEM','system_count_plugins');            
          WHEN 1 THEN 
          # TODO This only runs rarely, so skipping for now, but should not be using v_object
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
      END IF;
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
      # maybe add Detailed Occupants Enabled check here
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
DECLARE vSystemID INT;
DECLARE done INT DEFAULT 0;  
DECLARE cur1 CURSOR FOR SELECT method_queue_id,object_name,object_type,method_name,parameter_1,parameter_2 FROM osae_v_method_queue WHERE object_owner_id=vSystemID;
DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
  SET vSystemID = osae_fn_object_getid('SYSTEM');
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
                SELECT count(state_name) INTO vStateCount FROM osae_v_object_type_state WHERE state_name=vMethod AND object_type=vObjectType; 
                IF vStateCount = 1 THEN   
                    CALL osae_sp_object_state_set (vObjectName,vMethod,'SYSTEM','process_system_methods');
                ELSE
                    SELECT count(property_name) INTO vPropertyCount FROM osae_v_object_type_property WHERE property_name=vMethod AND object_type=vObjectType; 
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
-- Create function "osae_fn_object_exists"
--
CREATE FUNCTION osae_fn_object_exists(pobjectname varchar(200))
  RETURNS int(11)
BEGIN
DECLARE vObjectCount INT DEFAULT 0;
  # 049 the following line WAS hitting the object view and only hitting the table was needed!!!!
  SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE object_name=pobjectname OR object_alias=pobjectname;
  IF vObjectCount = 0 THEN
    RETURN 0;
  ELSE
    RETURN 1;
  END IF;
END
$$

--
-- Create function "osae_fn_object_getid"
--
CREATE FUNCTION osae_fn_object_getid(pobjectname varchar(200))
  RETURNS int(11)
BEGIN
  DECLARE vObjectID int;
  SELECT object_id INTO vObjectID FROM osae_object WHERE object_name = pobjectname OR object_alias = pobjectname;
  RETURN vObjectID;
END
$$

--
-- Create function "osae_fn_object_property_exists"
--
CREATE FUNCTION osae_fn_object_property_exists(pobjectname varchar(200), ppropertyname varchar(200))
  RETURNS int(11)
BEGIN
DECLARE vObjectCount INT DEFAULT 0;

  #SELECT COUNT(object_id) INTO vObjectCount FROM osae_v_object_property WHERE (object_name=pobjectname OR object_alias=pobjectname) AND property_name=ppropertyname;
  SELECT COUNT(osae_object.object_id) INTO vObjectCount FROM osae_object_property 
    INNER JOIN osae_object ON osae_object_property.object_id = osae_object.object_id
    INNER JOIN osae_object_type_property ON osae_object_property.object_type_property_id = osae_object_type_property.property_id
    WHERE (object_name=pobjectname OR object_alias=pobjectname) AND property_name=ppropertyname;
  IF vObjectCount = 0 THEN
    RETURN 0;
  ELSE
    RETURN 1;
  END IF;
END
$$

--
-- Create function "osae_fn_plugin_count"
--
CREATE FUNCTION osae_fn_plugin_count()
  RETURNS int(11)
BEGIN
DECLARE vObjectCount INT DEFAULT 0;
SELECT 
  COUNT(osae_object.object_name) INTO vObjectCount
FROM
  osae_object
Inner Join osae_object_type ON 
  osae_object.object_type_id = osae_object_type.object_type_id
Inner Join osae_object_type osae_object_base_type ON 
  osae_object_type.base_type_id = osae_object_base_type.object_type_id
WHERE
  osae_object_base_type.object_type = 'PLUGIN';

RETURN vObjectCount;
END
$$

--
-- Create function "osae_fn_plugin_enabled_count"
--
CREATE FUNCTION osae_fn_plugin_enabled_count()
  RETURNS int(11)
BEGIN
DECLARE vObjectCount INT DEFAULT 0;
SELECT 
  COUNT(osae_object.object_name) INTO vObjectCount
FROM
  osae_object
Inner Join osae_object_type ON 
  osae_object.object_type_id = osae_object_type.object_type_id
Inner Join osae_object_type osae_object_base_type ON 
  osae_object_type.base_type_id = osae_object_base_type.object_type_id
WHERE
  osae_object_base_type.object_type = 'PLUGIN'
  AND osae_object.enabled = 1;

RETURN vObjectCount;
END
$$

--
-- Create function "osae_fn_plugin_running_count"
--
CREATE FUNCTION osae_fn_plugin_running_count()
  RETURNS int(11)
BEGIN

DECLARE vObjectCount INT DEFAULT 0;
SELECT
  COUNT(osae_object.object_name) INTO vObjectCount
FROM osae_object
  INNER JOIN osae_object_type
    ON osae_object.object_type_id = osae_object_type.object_type_id
  INNER JOIN osae_object_type osae_object_base_type
    ON osae_object_type.base_type_id = osae_object_base_type.object_type_id
  INNER JOIN osae_object_type_state
    ON osae_object.state_id = osae_object_type_state.state_id
    AND osae_object.object_type_id = osae_object_type_state.object_type_id
WHERE osae_object_base_type.object_type = 'PLUGIN'
AND osae_object.enabled = 1
AND osae_object_type_state.state_name = 'ON';

RETURN vObjectCount;
END
$$

--
-- Create function "osae_fn_trust_level_property_exists"
--
CREATE FUNCTION osae_fn_trust_level_property_exists(pname      varchar(200))
  RETURNS int(11)
BEGIN
DECLARE vObjectTrustCount INT DEFAULT 0;
DECLARE vFromObjectType VARCHAR(255);

  # Verify the FromObject has a trust_level and add one if not
  # 049 ^^^^ Stupid expensive, runs too much, replaced with direct SQL
  #SELECT COUNT(object_property_id) INTO vObjectTrustCount FROM osae_v_object_property WHERE property_name='Trust Level' AND object_name=pname OR object_alias=pname;
  SELECT COUNT(osae_object.object_id) INTO vObjectTrustCount FROM osae_object_property 
    INNER JOIN osae_object ON osae_object_property.object_id = osae_object.object_id
    INNER JOIN osae_object_type_property ON osae_object_property.object_type_property_id = osae_object_type_property.property_id
    WHERE (object_name=pname OR object_alias=pname) AND property_name='Trust Level';
  IF vObjectTrustCount = 0 THEN
    # 049 Replaced View
    #SELECT object_type INTO vFromObjectType FROM osae_v_object WHERE object_name=pfromobject OR object_alias=pfromobject;
    SELECT osae_object_type.object_type INTO vFromObjectType FROM osae_object
      INNER JOIN osae_object_type ON osae_object.object_type_id = osae_object_type.object_type_id
      WHERE object_name=pfromobject OR object_alias=pfromobject;
    CALL osae_sp_object_type_property_add(vFromObjectType,'Trust Level','Integer','','50',0);
  END IF;
RETURN 1;
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
  DECLARE vDetailedOccupancy VARCHAR(20);
  SELECT base_type, object_name INTO vContainerType, vContainerName FROM osae_v_object WHERE object_id = NEW.container_id;
  SELECT base_type, state_name INTO vBaseType, vStateName FROM osae_v_object WHERE object_id = OLD.object_id;
  SET vPersonID = (SELECT object_type_id FROM osae_object_type WHERE object_type = 'PERSON');
  IF NEW.object_type_id = vPersonID AND OLD.container_id <> NEW.container_id AND vContainerType = 'PLACE' THEN
    CALL osae_sp_debug_log_add('Object After Update - Update Screen Position: ', 'SYSTEM');
    #CALL osae_sp_system_count_room_occupants;
    #Update the Screen Objects to reflect the container
    CALL osae_sp_screen_object_position(OLD.object_name, vContainerName);
  END IF;

  SET vDetailedOccupancy = (SELECT property_value FROM osae_v_object_property WHERE object_name = "SYSTEM" and property_name="Detailed Occupancy Enabled");
  IF vDetailedOccupancy = "TRUE" THEN
    IF OLD.state_id <> NEW.state_id AND vBaseType = "SENSOR" AND vStateName != "OFF" THEN
      CALL osae_sp_system_who_tripped_sensor(NEW.object_name);
    END IF;
  END IF;
END
$$

--
-- Alter trigger "osae_tr_object_property_after_update"
--
DROP TRIGGER IF EXISTS osae_tr_object_property_after_update$$
CREATE TRIGGER osae_tr_object_property_after_update
	AFTER UPDATE
	ON osae_object_property
	FOR EACH ROW
BEGIN
  DECLARE vTrack Boolean;
  DECLARE vDataType VARCHAR(100);
    SELECT track_history, property_datatype INTO vTrack, vDataType FROM osae_v_object_type_property WHERE property_id=NEW.object_type_property_id;
    IF vTrack THEN
  #     INSERT INTO osae_object_property_history (object_property_id,property_value) VALUES(NEW.object_property_id,NEW.property_value);
      CASE vDataType
        WHEN "String" THEN INSERT INTO osae_object_property_history (object_property_id,property_value) VALUES(NEW.object_property_id,NEW.property_value);
        WHEN "Integer" THEN INSERT INTO osae_object_property_history (object_property_id,property_value,property_integer) VALUES(NEW.object_property_id,NEW.property_value,NEW.property_value);
        WHEN "Decimal" THEN INSERT INTO osae_object_property_history (object_property_id,property_value,property_decimal) VALUES(NEW.object_property_id,NEW.property_value,NEW.property_value);
        WHEN "Boolean" THEN INSERT INTO osae_object_property_history (object_property_id,property_value,property_boolean) VALUES(NEW.object_property_id,NEW.property_value,NEW.property_value);
        WHEN "Float" THEN INSERT INTO osae_object_property_history (object_property_id,property_value,property_float) VALUES(NEW.object_property_id,NEW.property_value,NEW.property_value);
     END CASE;
   END IF;
END
$$

DELIMITER ;

--
-- Alter view "osae_v_method_queue"
--
CREATE OR REPLACE 
VIEW osae_v_method_queue
AS
	select `osae_method_queue`.`method_queue_id` AS `method_queue_id`,`osae_method_queue`.`entry_time` AS `entry_time`,`osae_object`.`object_name` AS `object_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_method_queue`.`parameter_1` AS `parameter_1`,`osae_method_queue`.`parameter_2` AS `parameter_2`,`osae_from_object`.`object_name` AS `from_object`,`osae_method_queue`.`debug_trace` AS `debug_trace`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_description` AS `object_description`,`osae_method_queue`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_method_queue`.`from_object_id` AS `from_object_id`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_types1`.`object_type` AS `base_type`,`osae_owner_object`.`object_name` AS `object_owner`,`osae_owner_object`.`object_id` AS `object_owner_id` from ((((((`osae_object_type` left join `osae_object` `osae_owner_object` on((`osae_object_type`.`plugin_object_id` = `osae_owner_object`.`object_id`))) left join `osae_object_type` `osae_object_types1` on((`osae_object_type`.`base_type_id` = `osae_object_types1`.`object_type_id`))) join `osae_object` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) join `osae_object_type_method` on((`osae_object_type`.`object_type_id` = `osae_object_type_method`.`object_type_id`))) join `osae_method_queue` on(((`osae_object`.`object_id` = `osae_method_queue`.`object_id`) and (`osae_object_type_method`.`method_id` = `osae_method_queue`.`method_id`)))) left join `osae_object` `osae_from_object` on((`osae_from_object`.`object_id` = `osae_method_queue`.`from_object_id`)));

--
-- Alter view "osae_v_object_event"
--
CREATE OR REPLACE 
VIEW osae_v_object_event
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`container_id` AS `container_id`,`osae_object`.`enabled` AS `enabled`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`container` AS `container`,`osae_object_type`.`object_type_tooltip` AS `object_type_tooltip`,`osae_object_type_event`.`event_id` AS `event_id`,`osae_object_type_event`.`event_name` AS `event_name`,`osae_object_type_event`.`event_label` AS `event_label`,coalesce(`osae_object_type_event`.`event_tooltip`,'') AS `event_tooltip` from ((`osae_object` join `osae_object_type` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) join `osae_object_type_event` on((`osae_object_type`.`object_type_id` = `osae_object_type_event`.`object_type_id`)));

--
-- Alter view "osae_v_object_method"
--
CREATE OR REPLACE 
VIEW osae_v_object_method
AS
	select `osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object`.`object_id` AS `object_id`,`osae_object_type`.`object_type_id` AS `object_type_id`,coalesce(`osae_object_type`.`object_type_tooltip`,'') AS `object_type_tooltip`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_object_type_method`.`param_1_label` AS `param_1_label`,`osae_object_type_method`.`param_2_label` AS `param_2_label`,`osae_object_type_method`.`param_1_default` AS `param_1_default`,`osae_object_type_method`.`param_2_default` AS `param_2_default`,`osae_object_type_method`.`method_id` AS `method_id`,`osae_object_type_method`.`method_tooltip` AS `method_tooltip` from ((`osae_object` left join `osae_object_type` on((`osae_object_type`.`object_type_id` = `osae_object`.`object_type_id`))) join `osae_object_type_method` on((`osae_object_type`.`object_type_id` = `osae_object_type_method`.`object_type_id`)));

--
-- Create view "osae_v_object_off_timer_ready"
--
CREATE
VIEW osae_v_object_off_timer_ready
AS
SELECT
  `osae_object`.`object_name` AS `object_name`
FROM (((`osae_object`
  JOIN `osae_object_type_state`
    ON ((`osae_object`.`state_id` = `osae_object_type_state`.`state_id`)))
  JOIN `osae_object_property`
    ON ((`osae_object_property`.`object_id` = `osae_object`.`object_id`)))
  JOIN `osae_object_type_property`
    ON ((`osae_object_property`.`object_type_property_id` = `osae_object_type_property`.`property_id`)))
WHERE ((`osae_object_type_state`.`state_name` <> 'OFF')
AND (`osae_object_type_property`.`property_name` = 'OFF TIMER')
AND (`osae_object_property`.`property_value` IS NOT NULL)
AND (`osae_object_property`.`property_value` <> '')
AND (`osae_object_property`.`property_value` <> '-1')
AND (subtime(now(), sec_to_time(`osae_object_property`.`property_value`)) >= `osae_object`.`last_updated`));

--
-- Alter view "osae_v_object_property"
--
CREATE OR REPLACE 
VIEW osae_v_object_property
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`container_id` AS `container_id`,`osae_object`.`min_trust_level` AS `min_trust_level`,`osae_object`.`enabled` AS `enabled`,`osae_object`.`last_updated` AS `object_last_updated`,coalesce(`osae_object`.`last_state_change`,now()) AS `last_state_change`,`osae_object_property`.`last_updated` AS `last_updated`,`osae_object_property`.`object_property_id` AS `object_property_id`,`osae_object_property`.`object_type_property_id` AS `object_type_property_id`,`osae_object_property`.`trust_level` AS `trust_level`,`osae_object_property`.`interest_level` AS `interest_level`,coalesce(`osae_object_property`.`source_name`,'') AS `source_name`,coalesce(`osae_object_property`.`property_value`,'') AS `property_value`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type_property`.`property_datatype` AS `property_datatype`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_type_property`.`property_default` AS `property_default`,`osae_object_type_property`.`property_id` AS `property_id`,`osae_object_type_property`.`track_history` AS `track_history`,`osae_object_type_property`.`property_tooltip` AS `property_tooltip`,`osae_object_type_property`.`property_required` AS `property_required`,`ot1`.`object_type` AS `base_type`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_property`.`property_object_type_id` AS `property_object_type_id`,`osae_object_type_1`.`object_type` AS `property_object_type`,`osae_object_1`.`object_name` AS `container_name`,timestampdiff(SECOND,`osae_object`.`last_state_change`,now()) AS `time_in_state` from (((((((`osae_object` join `osae_object_property` on((`osae_object`.`object_id` = `osae_object_property`.`object_id`))) join `osae_object_type` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) join `osae_object_type_property` on(((`osae_object_type`.`object_type_id` = `osae_object_type_property`.`object_type_id`) and (`osae_object_property`.`object_type_property_id` = `osae_object_type_property`.`property_id`)))) left join `osae_object_type_state` on((`osae_object`.`state_id` = `osae_object_type_state`.`state_id`))) join `osae_object_type` `ot1` on((`osae_object_type`.`base_type_id` = `ot1`.`object_type_id`))) left join `osae_object_type` `osae_object_type_1` on((`osae_object_type_property`.`property_object_type_id` = `osae_object_type_1`.`object_type_id`))) left join `osae_object` `osae_object_1` on((`osae_object`.`container_id` = `osae_object_1`.`object_id`)));

--
-- Alter view "osae_v_object_property_history"
--
CREATE OR REPLACE 
VIEW osae_v_object_property_history
AS
	select `osae_object_property_history`.`history_id` AS `history_id`,`osae_object_property_history`.`history_timestamp` AS `history_timestamp`,(case `osae_object_type_property`.`property_datatype` when 'Boolean' then if((`osae_object_property_history`.`property_value` = 'TRUE'),1,0) when 'Integer' then `osae_object_property_history`.`property_integer` when 'Float' then `osae_object_property_history`.`property_float` else `osae_object_property_history`.`property_value` end) AS `property_value`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object_property_history`.`object_property_id` AS `object_property_id`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_type_property`.`property_datatype` AS `property_datatype`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object`.`object_description` AS `object_description` from ((((`osae_object_property_history` join `osae_object_property` on((`osae_object_property_history`.`object_property_id` = `osae_object_property`.`object_property_id`))) join `osae_object` on((`osae_object_property`.`object_id` = `osae_object`.`object_id`))) join `osae_object_type_property` on((`osae_object_property`.`object_type_property_id` = `osae_object_type_property`.`property_id`))) join `osae_object_type` on(((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`) and (`osae_object_type_property`.`object_type_id` = `osae_object_type`.`object_type_id`))));

--
-- Alter view "osae_v_object_state"
--
CREATE OR REPLACE 
VIEW osae_v_object_state
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`object_type_id` AS `object_type_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type_state`.`state_id` AS `state_id`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_state`.`state_label` AS `state_label`,coalesce(`osae_object_type_state`.`state_tooltip`,'') AS `state_tooltip` from ((`osae_object` left join `osae_object_type` on((`osae_object_type`.`object_type_id` = `osae_object`.`object_type_id`))) join `osae_object_type_state` on((`osae_object_type`.`object_type_id` = `osae_object_type_state`.`object_type_id`)));

--
-- Alter view "osae_v_object_type"
--
CREATE OR REPLACE 
VIEW osae_v_object_type
AS
	select `osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`container` AS `container`,`osae_object_type`.`object_type_tooltip` AS `object_type_tooltip`,`osae_object_type`.`hide_redundant_events` AS `hide_redundant_events`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`ot1`.`object_type` AS `base_type` from ((`osae_object_type` left join `osae_object` on((`osae_object`.`object_id` = `osae_object_type`.`plugin_object_id`))) left join `osae_object_type` `ot1` on((`osae_object_type`.`base_type_id` = `ot1`.`object_type_id`)));

--
-- Alter view "osae_v_object_type_event"
--
CREATE OR REPLACE 
VIEW osae_v_object_type_event
AS
	select `osae_object_type_event`.`event_id` AS `event_id`,`osae_object_type_event`.`event_name` AS `event_name`,`osae_object_type_event`.`event_label` AS `event_label`,coalesce(`osae_object_type_event`.`event_tooltip`,'') AS `event_tooltip`,`osae_object_type_event`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`hide_redundant_events` AS `hide_redundant_events`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id` from (`osae_object_type` join `osae_object_type_event` on((`osae_object_type`.`object_type_id` = `osae_object_type_event`.`object_type_id`)));

--
-- Alter view "osae_v_object_type_method"
--
CREATE OR REPLACE 
VIEW osae_v_object_type_method
AS
	select `osae_object_type_1`.`object_type` AS `base_type`,`osae_object_type_method`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_type_method`.`method_label` AS `method_label`,coalesce(`osae_object_type_method`.`method_tooltip`,'') AS `method_tooltip`,`osae_object_type_method`.`object_type_id` AS `object_type_id`,coalesce(`osae_object_type_method`.`param_1_label`,'') AS `param_1_label`,coalesce(`osae_object_type_method`.`param_2_label`,'') AS `param_2_label`,coalesce(`osae_object_type_method`.`param_1_default`,'') AS `param_1_default`,coalesce(`osae_object_type_method`.`param_2_default`,'') AS `param_2_default`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden` from ((`osae_object_type` left join `osae_object_type_method` on((`osae_object_type`.`object_type_id` = `osae_object_type_method`.`object_type_id`))) left join `osae_object_type` `osae_object_type_1` on((`osae_object_type`.`base_type_id` = `osae_object_type_1`.`object_type_id`)));

--
-- Alter view "osae_v_object_type_property"
--
CREATE OR REPLACE 
VIEW osae_v_object_type_property
AS
	select `osae_object_type_property`.`property_id` AS `property_id`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_type_property`.`property_datatype` AS `property_datatype`,`osae_object_type_property`.`property_default` AS `property_default`,`osae_object_type_property`.`object_type_id` AS `object_type_id`,`osae_object_type_property`.`property_object_type_id` AS `property_object_type_id`,coalesce(`osae_object_type_property`.`property_tooltip`,'') AS `property_tooltip`,`osae_object_type_property`.`track_history` AS `track_history`,`osae_object_type_property`.`property_required` AS `property_required`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`system_hidden` AS `system_hidden`,coalesce(`osae_object_type_1`.`object_type`,'') AS `property_object_type` from ((`osae_object_type` join `osae_object_type_property` on((`osae_object_type`.`object_type_id` = `osae_object_type_property`.`object_type_id`))) left join `osae_object_type` `osae_object_type_1` on((`osae_object_type_property`.`property_object_type_id` = `osae_object_type_1`.`object_type_id`)));

--
-- Alter view "osae_v_object_type_state"
--
CREATE OR REPLACE 
VIEW osae_v_object_type_state
AS
	select `osae_object_type_1`.`object_type` AS `base_type`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type_state`.`state_label` AS `state_label`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type_state`.`state_name` AS `state_name`,coalesce(`osae_object_type_state`.`state_tooltip`,'') AS `state_tooltip`,`osae_object_type_state`.`state_id` AS `state_id` from ((`osae_object_type` left join `osae_object_type_state` on((`osae_object_type`.`object_type_id` = `osae_object_type_state`.`object_type_id`))) left join `osae_object_type` `osae_object_type_1` on((`osae_object_type`.`base_type_id` = `osae_object_type_1`.`object_type_id`)));

--
-- Alter view "osae_v_system_occupied_rooms"
--
CREATE OR REPLACE 
VIEW osae_v_system_occupied_rooms
AS
	select `osae_rooms`.`object_name` AS `room`,count(`osae_occupants`.`object_name`) AS `occupant_count` from (((`osae_object` `osae_rooms` join `osae_object_type` `osae_room_type` on((`osae_rooms`.`object_type_id` = `osae_room_type`.`object_type_id`))) left join `osae_object` `osae_occupants` on((`osae_rooms`.`object_id` = `osae_occupants`.`container_id`))) left join `osae_object_type` `osae_occupant_type` on((`osae_occupants`.`object_type_id` = `osae_occupant_type`.`object_type_id`))) where ((`osae_room_type`.`object_type` = 'ROOM') and (`osae_occupant_type`.`object_type` = 'PERSON')) group by `osae_rooms`.`object_name`;

--
-- Create view "osae_v_system_plugins_errored"
--
CREATE
VIEW osae_v_system_plugins_errored
AS
SELECT
  `osae_object`.`object_name` AS `object_name`
FROM (((((`osae_object`
  JOIN `osae_object_type_state` `osae_object_state`
    ON (((`osae_object`.`state_id` = `osae_object_state`.`state_id`)
    AND (`osae_object`.`object_type_id` = `osae_object_state`.`object_type_id`))))
  JOIN `osae_object_type`
    ON ((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`)))
  JOIN `osae_object_type` `osae_object_base_type`
    ON ((`osae_object_type`.`base_type_id` = `osae_object_base_type`.`object_type_id`)))
  JOIN `osae_object` `osae_container`
    ON ((`osae_object`.`container_id` = `osae_container`.`object_id`)))
  JOIN `osae_object_type_state` `osae_contianer_state`
    ON (((`osae_container`.`state_id` = `osae_contianer_state`.`state_id`)
    AND (`osae_container`.`object_type_id` = `osae_contianer_state`.`object_type_id`))))
WHERE ((`osae_object_state`.`state_name` = 'OFF')
AND (`osae_object_base_type`.`object_type` = 'PLUGIN')
AND (`osae_object`.`enabled` = 1)
AND (`osae_contianer_state`.`state_name` = 'ON'));

--
-- Alter view "osae_v_object"
--
CREATE OR REPLACE 
VIEW osae_v_object
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`last_updated` AS `last_updated`,`osae_object`.`last_state_change` AS `last_state_change`,`osae_object`.`min_trust_level` AS `min_trust_level`,`osae_object`.`enabled` AS `enabled`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`container` AS `container`,`osae_object_type`.`object_type_tooltip` AS `object_type_tooltip`,`osae_object_type_state`.`state_id` AS `state_id`,coalesce(`osae_object_type_state`.`state_name`,'') AS `state_name`,coalesce(`osae_object_type_state`.`state_label`,'') AS `state_label`,`objects_2`.`object_name` AS `owned_by`,`object_types_2`.`object_type` AS `base_type`,`objects_1`.`object_name` AS `container_name`,`osae_object`.`container_id` AS `container_id`,(select max(`osae_v_object_property`.`last_updated`) AS `expr1` from `osae_v_object_property` where ((`osae_v_object_property`.`object_id` = `osae_object`.`object_id`) and (`osae_v_object_property`.`property_name` <> 'Time'))) AS `property_last_updated`,timestampdiff(SECOND,`osae_object`.`last_state_change`,now()) AS `time_in_state`,`osae_object_type_state_1`.`state_name` AS `container_state_name`,`osae_object_type_state_1`.`state_label` AS `container_state_label` from ((((((`osae_object` left join `osae_object_type` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) left join `osae_object_type` `object_types_2` on((`osae_object_type`.`base_type_id` = `object_types_2`.`object_type_id`))) left join `osae_object` `objects_2` on((`osae_object_type`.`plugin_object_id` = `objects_2`.`object_id`))) left join `osae_object_type_state` on(((`osae_object_type`.`object_type_id` = `osae_object_type_state`.`object_type_id`) and (`osae_object_type_state`.`state_id` = `osae_object`.`state_id`)))) left join `osae_object` `objects_1` on((`objects_1`.`object_id` = `osae_object`.`container_id`))) left join `osae_object_type_state` `osae_object_type_state_1` on((`objects_1`.`state_id` = `osae_object_type_state_1`.`state_id`)));

--
-- Create view "osae_v_object_type_method_list_full"
--
CREATE
VIEW osae_v_object_type_method_list_full
AS
SELECT
  `osae_v_object_type_method`.`base_type` AS `base_type`,
  `osae_v_object_type_method`.`object_type` AS `object_type`,
  `osae_v_object_type_method`.`method_label` AS `method_label`
FROM `osae_v_object_type_method`
WHERE ((`osae_v_object_type_method`.`base_type` NOT IN ('CONTROL', 'SCREEN', 'LIST'))
AND `osae_v_object_type_method`.`object_type` IN (SELECT DISTINCT
    `osae_v_object`.`object_type`
  FROM `osae_v_object`));

DELIMITER $$

--
-- Alter event "osae_ev_day_timer"
--
ALTER EVENT osae_ev_day_timer
	DO 
BEGIN
    CALL osae_sp_object_property_set('SYSTEM','Day Of Week',DAYOFWEEK(CURDATE()),'SYSTEM','osae_ev_minute_maint'); 
    CALL osae_sp_object_property_set('SYSTEM','Day Of Month',DAYOFMONTH(CURDATE()),'SYSTEM','osae_ev_minute_maint'); 
    UPDATE osae_object_state_history SET times_this_day=0;
END
$$

--
-- Alter event "osae_ev_minute_maint"
--
ALTER EVENT osae_ev_minute_maint
	DO 
BEGIN
    CALL osae_sp_object_property_set('SYSTEM','Date',CURDATE(),'SYSTEM','osae_ev_minute_maint'); 
    CALL osae_sp_run_scheduled_methods; 
    #CALL osae_sp_debug_log_add('Minute timer','SYSTEM');  
END
$$

--
-- Alter event "osae_ev_off_timer"
--
ALTER EVENT osae_ev_off_timer
	DO 
# This is the most important Event in OSA.   It runs every second and has huge impact on performace and DB stability.  
# Performing full review and performace statistics for v049

BEGIN
  DECLARE iServiceRunning INT DEFAULT 0;
  DECLARE vObjectName  VARCHAR(200);
  DECLARE iLoopCount   INT DEFAULT 0;
  DECLARE iMethodCount INT DEFAULT 0;
  DECLARE iStateCount  INT DEFAULT 0;
  DECLARE done         INT DEFAULT 0;
  #This cursor is the first problem.   String indexing and date compairisons are pretty intense, must optimize this.   Maybe move to a function and run metrics on it.
  #Optomized this cursor to not use a generic view and use a custom view instead
  #DECLARE cur1 CURSOR FOR SELECT object_name FROM osae_v_object_property WHERE state_name <> 'OFF' AND property_name = 'OFF TIMER' AND property_value IS NOT NULL AND property_value <> '' AND property_value <> '-1' AND subtime(now(), sec_to_time(property_value)) > object_last_updated;
  DECLARE cur1 CURSOR FOR SELECT object_name FROM osae_v_object_off_timer_ready;
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
  DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
  #The following was switched from v_object to v_object_state as it is a lighter view
  SET iServiceRunning = (SELECT COUNT(object_id) FROM osae_v_object_state WHERE object_name = 'SERVICE' and state_name = 'ON');
  IF iServiceRunning > 0 THEN
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
  END IF;
END
$$

DELIMITER ;

--
-- Enable foreign keys
--
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
-- Container Object
CALL osae_sp_object_type_add ('CONTAINER','Core Type: Container','','CONTAINER',0,1,1,1,'Container that can hold other objects.
This is a Core-Type object and is usually NOT USED by users.');
CALL osae_sp_object_type_state_add('CONTAINER','ON','Closed','This state represents that this Container is Closed,');
CALL osae_sp_object_type_state_add('CONTAINER','OFF','Open','This state represents that this Container is OPEN,');
CALL osae_sp_object_type_event_add('CONTAINER','OFF','Opened','This event will fire when the Container state is changed to Open.');
CALL osae_sp_object_type_event_add('CONTAINER','ON','Closed','This event will fire when the Container state is changed to Closed.');
CALL osae_sp_object_type_property_add('CONTAINER','Content List','List','','',0,0,'This property will contain the objects that are located in this Container.
This property is automatically populated by the system.');

-- Control Object
CALL osae_sp_object_type_add ('CONTROL','Core Type: Display control','','CONTROL',0,1,0,1,'Screen Control.
This is a Core-Type object and is usually NOT USED by users.');

-- Browser Control
CALL osae_sp_object_type_add ('CONTROL BROWSER','Control - Browser Frame','','CONTROL',0,1,0,1,'Web Browser control used in the Screens application 
or in the WebUI Screens.');
CALL osae_sp_object_type_property_add('CONTROL BROWSER','URI','String','','',0,1,'Enter the URL of the Browser to be displayed.');
CALL osae_sp_object_type_property_add('CONTROL BROWSER','X','Integer','','0',0,1,'Enter the X-Position of the Browser to be displayed.');
CALL osae_sp_object_type_property_add('CONTROL BROWSER','Y','Integer','','0',0,1,'Enter the X-Position of the Browser to be displayed.');
CALL osae_sp_object_type_property_add('CONTROL BROWSER','ZOrder','Integer','','0',0,1,'Enter the ZOrder-Position of the Browser to be displayed.');
CALL osae_sp_object_type_property_add('CONTROL BROWSER','Width','Integer','','0',0,1,'Enter the Width of the Browser to be displayed.');
CALL osae_sp_object_type_property_add('CONTROL BROWSER','Height','Integer','','0',0,1,'Enter the Height of the Browser to be displayed.');

-- Camera Viewer Control
CALL osae_sp_object_type_add ('CONTROL CAMERA VIEWER','Control - IP Camera Viewer','','CONTROL',0,1,0,1,'Camera Viewer control used in the Screens
application or in the WebUI Screens.');
CALL osae_sp_object_type_property_add('CONTROL CAMERA VIEWER','X','Integer','','0',0,1,'Enter the X position to place the viewer on the Screen.');
CALL osae_sp_object_type_property_add('CONTROL CAMERA VIEWER','Y','Integer','','0',0,1,'Enter the Y position to place the viewer on the Screen.');
CALL osae_sp_object_type_property_add('CONTROL CAMERA VIEWER','ZOrder','Integer','','0',0,1,'Enter the ZOrder position to place the viewer on the Screen.');
CALL osae_sp_object_type_property_add('CONTROL CAMERA VIEWER','Object Name','String','','',0,1,'Enter the IP Camera Object Name to view in the viewer.');
CALL osae_sp_object_type_property_add('CONTROL CAMERA VIEWER','Width','Integer','','400',0,1,'Enter the Width of the viewer to display in the screen app.');
CALL osae_sp_object_type_property_add('CONTROL CAMERA VIEWER','Height','Integer','','0',0,1,'Enter the Height of the viewer to display in the screen app.');

-- Click Image Control
CALL osae_sp_object_type_add ('CONTROL CLICK IMAGE','Control - Click Control','','CONTROL',0,1,0,1,'Click Image control used in the Screens application or in the WebUI Screens.
A Click image is used to send a Method or Script command to the OSA engine to perform.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','X','Integer','','0',0,1,'Enter the X position to display this Click image on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Y','Integer','','0',0,1,'Enter the Y position to display this Click image on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Normal Image','File','','',0,1,'Select the image to display when for this Click Image.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Press Object Name','String','','',0,1,'Enter the Object Name of the object to execute
the Method or Script on when image is pressed.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Press Method Name','String','','',0,0,'Enter the Method name to execute when the this image is pressed.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Press Method Param 1','String','','',0,0,'Enter the Parameter 1 value to send with the method when pressed.
This is optional.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Press Method Param 2','String','','',0,0,'Enter the Parameter 2 value to send with the method when pressed.
This is optional.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','ZOrder','Integer','','0',0,1,'Enter the ZOrder position to display this Click image on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Pressed Image','File','','',0,0,'Select the image to display when Pressed.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Release Method Name','String','','',0,0,'Enter the Method name to execute when the this image is released.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Release Object Name','String','','',0,0,'Enter the Object Name of the object to execute
the Method or Script on when image is released.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Release Method Param 1','String','','',0,0,'Enter the Parameter 1 value to send with the method when released.
This is optional.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Press Script Param 2','String','','',0,0,'Enter the Parameter 2 value to send with the Script when pressed.
This is optional.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Press Script Param 1','String','','',0,0,'Enter the Parameter 1 value to send with the Script when pressed.
This is optional.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Press Script Name','String','','',0,0,'Enter the Script name to execute when the this image is pressed.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Release Method Param 2','String','','',0,0,'Enter the Parameter 2 value to send with the method when released.
This is optional.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Release Script Name','String','','',0,0,'Enter the Script name to execute when the this image is released.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Release Script Param 1','String','','',0,0,'Enter the Parameter 1 value to send with the Script when released.
This is optional.');
CALL osae_sp_object_type_property_add('CONTROL CLICK IMAGE','Release Script Param 2','String','','',0,0,'Enter the Parameter 2 value to send with the Script when released.
This is optional.');

-- Navigation Image Control
CALL osae_sp_object_type_add ('CONTROL NAVIGATION IMAGE','Control - Navigation Image','','CONTROL',0,1,0,1,'Navigation Image control used in the Screens application or in the WebUI Screens.
A Navigation image is used to change to another screen.');
CALL osae_sp_object_type_property_add('CONTROL NAVIGATION IMAGE','X','Integer','','0',0,1,'Enter the X position to display this Navigation image on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL NAVIGATION IMAGE','Y','Integer','','0',0,1,'Enter the Y position to display this Navigation image on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL NAVIGATION IMAGE','Image','File','','',0,1,'Select the Image to be displayed for this Navigation image.');
CALL osae_sp_object_type_property_add('CONTROL NAVIGATION IMAGE','Screen','String','','',0,1,'Enter the Screen to navigate to when this image is clicked.');
CALL osae_sp_object_type_property_add('CONTROL NAVIGATION IMAGE','ZOrder','Integer','','0',0,1,'Enter the ZOrder position to display this Navigation image on the
Screen app or WebUI screens.');

-- Property Label Control
CALL osae_sp_object_type_add ('CONTROL PROPERTY LABEL','Control - Property Value','','CONTROL',0,1,0,1,'Property Label control used in the Screens application or in the WebUI Screens.
A Property Label is used to display the current value of a particular property.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','Object Name','String','','',0,1,'Enter the Name of the Object for the property value to be displayed.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','Property Name','String','','',0,1,'Enter the name of the Property who\'s value will be displayed.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','X','Integer','','200',0,1,'Enter the X position to display this Property Label on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','Y','Integer','','200',0,1,'Enter the Y position to display this Property Label on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','Font Name','String','','Arial',0,1,'Select the Font name used to display the Text.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','Font Size','Integer','','12',0,1,'Enter the Font size used to display the text.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','ZOrder','Integer','','2',0,1,'Enter the Zorder position to display this Property Label on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','Suffix','String','','',0,0,'Enter any Suffix information to display after the text.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','Prefix','String','','',0,0,'Enter any PREFIX information to display before the text.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','Back Color','String','','',0,1,'Select the color to display behind the text.');
CALL osae_sp_object_type_property_add('CONTROL PROPERTY LABEL','Fore Color','String','','',0,1,'Select the color used to display the text.');

-- Screen Object Control
CALL osae_sp_object_type_add ('CONTROL SCREEN OBJECTS','Control - Screen Objects','','CONTROL',0,1,0,1,'Screen Object Control.
This is usually used by the system and NOT by users.');
CALL osae_sp_object_type_property_add('CONTROL SCREEN OBJECTS','X','Integer','','0',0,1,'Enter the X position to display this Control on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL SCREEN OBJECTS','Y','Integer','','0',0,1,'Enter the Y position to display this Control on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL SCREEN OBJECTS','ZOrder','Integer','','0',0,1,'Enter the ZOrder position to display this Control on the
Screen app or WebUI screens.');

-- State Image Control
CALL osae_sp_object_type_add ('CONTROL STATE IMAGE','Control - Object State','','CONTROL',0,1,0,1,'TState Image control used in the Screens application or in the WebUI Screens.
A State Image is used to display Images matching different States in the Screens app.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Object Name','String','','',0,1,'The Object whos State is represented by this Control.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 Name','String','','',0,1,'The name that represents the first State.
This is usually The State Label.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 Name','String','','',0,1,'The name that represents the second State.
This is usually the State Label.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 Name','String','','',0,0,'The name that represents the second State.  This is usually the state label.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 Image','File','','',0,1,'The image to display when object is in State 1');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 Image','File','','',0,1,'The image to display when object is in State 2.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 Image','File','','',0,0,'The image to display when object is in State 3.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 X','Integer','','100',0,1,'The X position to display the State 1 Image.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 Y','Integer','','100',0,1,'The Y position to display the State 1 Image.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 X','Integer','','100',0,1,'The X position to display the State 2 Image.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 Y','Integer','','100',0,1,'The Y position to display the State 2 Image.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 X','Integer','','100',0,0,'The X position to display the State 3 Image.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 Y','Integer','','100',0,0,'The Y position to display the State 3 Image.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','ZOrder','Integer','','1',0,1,'The Stacking Order when overlapping other Images.   Highest Values are on Top.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 Image 2','File','','',0,0,'Depricated');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 Image 3','File','','',0,0,'Depricated');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 Image 4','File','','',0,0,'Depricated');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 Image 2','File','','',0,0,'Depricated');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 Image 3','File','','',0,0,'Depricated');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 Image 4','File','','',0,0,'Depricated');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Repeat Animation','Boolean','','TRUE',0,0,'Depricated.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Frame Delay','Integer','','100',0,0,'Depricated');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Show Slider','Boolean','','FALSE',0,1,'Show a Slider Bar to set a 0 to 100 value for the associated Method.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 Image 2','File','','',0,0,'Depricated');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 Image 3','File','','',0,0,'Depricated');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 Image 4','File','','',0,0,'Depricated');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Slider Method','String','','',0,1,'The Method that the Slider Value is sent to.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Contained X','Integer','','100',0,1,'The X for the Default position of another Control displayed within this Control');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Contained Y','Integer','','100',0,1,'The Y for the Default position of another Control displayed within this Control');

-- Static Label Control
CALL osae_sp_object_type_add ('CONTROL STATIC LABEL','Control - Static Text Label','','CONTROL',0,1,0,1,'This object represents a Static Label control used in the Screens application or in the WebUI Screens.
This is used to display TEXT in the Screens app or on the WebUI Screens page.');
CALL osae_sp_object_type_property_add('CONTROL STATIC LABEL','X','Integer','','0',0,1,'Enter the X position to display this Static Label on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL STATIC LABEL','Y','Integer','','0',0,1,'Enter the Y position to display this Static Label on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL STATIC LABEL','Font Name','String','','Arial',0,1,'Enter the Font name to be used to display the text.');
CALL osae_sp_object_type_property_add('CONTROL STATIC LABEL','Font Size','Integer','','12',0,1,'Enter the Font Size used to display the text.');
CALL osae_sp_object_type_property_add('CONTROL STATIC LABEL','Value','String','','',0,1,'Enter the Text to be displayed.');
CALL osae_sp_object_type_property_add('CONTROL STATIC LABEL','ZOrder','Integer','','0',0,1,'Enter the ZOrder position to display this Static Label on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL STATIC LABEL','Background Color','String','','',0,1,'Select the Background color to be displayed behind the text.');

-- Timer Label Control
CALL osae_sp_object_type_add ('CONTROL TIMER LABEL','Control - Timer Label','','CONTROL',0,1,0,0,'Timer Label control used in the Screens application or in the WebUI Screens.
A Timer Label is used to display the current value of a particular Timer or OFF Timer.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Object Name','String','','',0,1,'Enter the Name of the Object for the Timer value to be displayed.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Property Name','String','','',0,1,'Enter the name of the Property who\'s value will be displayed.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','X','Integer','','0',0,1,'Enter the X position to display this Timer Label on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Y','Integer','','0',0,1,'Enter the Y position to display this Timer Label on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Font Name','String','','Arial',0,1,'Select the Font name used to display the Text.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Font Size','Integer','','12',0,1,'Enter the Font size used to display the text.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','ZOrder','Integer','','0',0,1,'Enter the ZOrder position to display this Timer Label on the
Screen app or WebUI screens.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Suffix','String','','',0,0,'Enter any Suffix information to display after the text.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Prefix','String','','',0,0,'Enter any PREFIX information to display before the text.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Back Color','String','','',0,1,'Select the color to display behind the text.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Fore Color','String','','',0,1,'Select the color used to display the text.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Font Color','String','','',0,1,'Select the color used to display the text.');
CALL osae_sp_object_type_property_add('CONTROL TIMER LABEL','Type','String','','',0,0,'');

-- User Selector Control
CALL osae_sp_object_type_add ('CONTROL USER SELECTOR','Control - User Selector','','CONTROL',0,1,0,1,'User Selector control used in the Screens application or in the WebUI Screens.
This is used to Allow users to log in to the Screens app.
This control is Automatically Added to each screen!');
CALL osae_sp_object_type_property_add('CONTROL USER SELECTOR','X','Integer','','0',0,1,'X position to display the User Selector on the Screen app.');
CALL osae_sp_object_type_property_add('CONTROL USER SELECTOR','Y','Integer','','0',0,1,'Y position to display the User Selector on the Screen app.');
CALL osae_sp_object_type_property_add('CONTROL USER SELECTOR','ZOrder','Integer','','0',0,1,'Enter the ZOrder position to display the User Selector on the
Screen app.');
CALL osae_sp_object_type_add ('USER CONTROL','User Control','SYSTEM','CONTROL',0,1,0,1,'User Control Object.
This is mainly used by the system and NOT by users.');
CALL osae_sp_object_type_property_add('USER CONTROL','X','Integer','','0',0,1,'X position to display this User Control on the');
CALL osae_sp_object_type_property_add('USER CONTROL','Y','Integer','','0',0,1,'Y position to display this User Control on the');
CALL osae_sp_object_type_property_add('USER CONTROL','Image','File','','',0,0,'Select the Image to be displayed for this User Control.');
CALL osae_sp_object_type_property_add('USER CONTROL','Screen','String','','',0,0,'Screen that this User Control is displayed on.');
CALL osae_sp_object_type_property_add('USER CONTROL','ZOrder','Integer','','0',0,1,'ZOrder position to display this User Controll on the');
CALL osae_sp_object_type_property_add('USER CONTROL','Control Type','String','','',0,1,'The Proper Control Type this User Control is associated with.');

-- User Control Object
CALL osae_sp_object_type_add ('USER CONTROL','User Control','SYSTEM','CONTROL',0,1,0,1,'User Control Object.
This is mainly used by the system and NOT by users.');
CALL osae_sp_object_type_property_add('USER CONTROL','X','Integer','','0',0,1,'X position to display this User Control on the');
CALL osae_sp_object_type_property_add('USER CONTROL','Y','Integer','','0',0,1,'Y position to display this User Control on the');
CALL osae_sp_object_type_property_add('USER CONTROL','Image','File','','',0,0,'Select the Image to be displayed for this User Control.');
CALL osae_sp_object_type_property_add('USER CONTROL','Screen','String','','',0,0,'Enter the screen that this User Control is displayed on.');
CALL osae_sp_object_type_property_add('USER CONTROL','ZOrder','Integer','','0',0,1,'Enter the ZOrder position to display this User Controll on the');
CALL osae_sp_object_type_property_add('USER CONTROL','Control Type','String','','',0,1,'The Proper Control Type this User Control is associated with.');

-- File List object
CALL osae_sp_object_type_add ('FILE LIST','File List','','LIST',0,0,0,1,'File List Object that can hold many File entries.');
CALL osae_sp_object_type_property_add('FILE LIST','Values','File','','',0,0,'Enter the Files to be contained in this List.');
CALL osae_sp_object_type_add ('LIST','Core Datatype: List','SYSTEM','LIST',0,1,0,1,'This object represents a List Object that can hold many entries.');
CALL osae_sp_object_type_property_add('LIST','Values','List','','',0,0,'Enter the values to be contained in this List.');

-- List Object
CALL osae_sp_object_type_add ('LIST','Core Datatype: List','SYSTEM','LIST',0,1,0,1,'List Object that can hold many entries.');
CALL osae_sp_object_type_property_add('LIST','Values','List','','',0,0,'Enter the values to be contained in this List.');

-- Speech List Object
CALL osae_sp_object_type_add ('SPEECH LIST','Speech List','','LIST',0,1,0,1,'Speech List Object that can hold many Speech entries.');
CALL osae_sp_object_type_property_add('SPEECH LIST','House Entered - HOME','List','','',0,0,'Enter the Speech Patterns to be contained in the House Entered - Home list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','House Entered - AWAY','List','','',0,0,'Enter the Speech Patterns to be contained in the House Entered-Away list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','House Entered - Halloween','List','','',0,0,'Enter the Speech Patterns to be contained in the House Entered-Halloween list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Greetings','List','','',0,0,'Enter the Speech Patterns to be contained in the Greetings list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Thanks','List','','',0,0,'Enter the Speech Patterns to be contained in the Thanks list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Weather - Current','List','','',0,0,'Enter the Speech Patterns to be contained in the Weather - Current list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Weather - Tomorrow','List','','',0,0,'Enter the Speech Patterns to be contained in the Weather - Tomorrow list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Mailbox - Mailman','List','','',0,0,'Enter the Speech Patterns to be contained in the Mailbox - Mailman list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Mailbox - Owners','List','','',0,0,'Enter the Speech Patterns to be contained in the Mailbox - Owners list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Mailbox - Secure','List','','',0,0,'Enter the Speech Patterns to be contained in the Mailbox - Secure list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Refridgerator - HOME','List','','',0,0,'Enter the Speech Patterns to be contained in the Refrigerator - Home list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Refridgerator - AWAY','List','','',0,0,'Enter the Speech Patterns to be contained in the Refrigerator - Away list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Living room - AWAY','List','','',0,0,'Enter the Speech Patterns to be contained in the Living Room - Away list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Time','List','','',0,0,'Enter the Speech Patterns to be contained in the Time list.');
CALL osae_sp_object_type_property_add('SPEECH LIST','Wake Response','List','','',0,0,'Enter the Speech Patterns to be contained in the Wake Response list.');

-- Person Object
CALL osae_sp_object_type_add ('PERSON','Core Type: People','SYSTEM','PERSON',0,1,0,1,'This object represents a Human entity.
This is usually a Person or a User OSA can track.');
CALL osae_sp_object_type_state_add('PERSON','ON','Here','This PERSON is Here.');
CALL osae_sp_object_type_state_add('PERSON','OFF','Gone','This PERSON is Gone.');
CALL osae_sp_object_type_event_add('PERSON','ON','Arrived','Person Arrived.');
CALL osae_sp_object_type_event_add('PERSON','OFF','Left','Person Left.');
CALL osae_sp_object_type_method_add('PERSON','ON','Arriving','','','','','Set this person to Here.');
CALL osae_sp_object_type_method_add('PERSON','OFF','Leaving','','','','','Set this person to Gone.');
CALL osae_sp_object_type_method_add('PERSON','SEND MESSAGE','Send Message','Message','Method','','Automatic','Send a message to this person.');
CALL osae_sp_object_type_method_add('PERSON','SET CONTAINER','Set Container','Object','','','','ESet this person in a Container');
CALL osae_sp_object_type_property_add('PERSON','Email Address','String','','',0,0,'Email address for this PERSON');
CALL osae_sp_object_type_property_add('PERSON','Home Phone','String','','',0,0,'Person\'s Home Phone Number.');
CALL osae_sp_object_type_property_add('PERSON','Mobile Phone','String','','',0,0,'Person\'s Mobile Phone Number.');
CALL osae_sp_object_type_property_add('PERSON','Birthdate','DateTime','','',0,0,'Birth Date of this PERSON.');
CALL osae_sp_object_type_property_add('PERSON','JabberID','String','','',0,0,'Person\'s Jabber ID.');
CALL osae_sp_object_type_property_add('PERSON','JabberStatus','String','','',0,0,'This person\'s Jabber Status.
This is automatically populated by the system.');
CALL osae_sp_object_type_property_add('PERSON','Security Level','String','','User',0,1,'This person\'s Security Level.
(Admin, User or Guest)');
CALL osae_sp_object_type_property_option_add('PERSON','Security Level','Admin');
CALL osae_sp_object_type_property_option_add('PERSON','Security Level','User');
CALL osae_sp_object_type_property_option_add('PERSON','Security Level','Guest');
CALL osae_sp_object_type_property_add('PERSON','Password','Password','','',0,1,'This Person\'s Log-In password to the WebUI.');
CALL osae_sp_object_type_property_add('PERSON','OFF TIMER','Integer','','-1',0,0,'The number of seconds before this Room is set to Leaving.  (-1 = Disabled)');
CALL osae_sp_object_type_property_add('PERSON','Full Name','String','','',0,0,'Enter this person\'s Full Name.');
CALL osae_sp_object_type_property_add('PERSON','Trust Level','Integer','','90',0,1,'This person\'s Trust level. (0-100)');
CALL osae_sp_object_type_property_add('PERSON','Communication Method','String','','Speech',0,0,'Enter the Communication method OSA should
use to communicate with this PERSON.');
CALL osae_sp_object_type_property_add('PERSON','Mother','Object Type','PERSON','',0,0,'Person Object that represents this person\'s mother.');
CALL osae_sp_object_type_property_add('PERSON','Father','Object Type','PERSON','',0,0,'Person Object that represents this person\'s father.');
CALL osae_sp_object_type_property_add('PERSON','PIN','Password','','',0,1,'This Person\'s Log-In pin to the Screens Application.');

-- City Object
CALL osae_sp_object_type_add ('CITY','Core Type: Locations','SYSTEM','PLACE',0,1,1,1,'City in a State.');
CALL osae_sp_object_type_state_add('CITY','ON','Exists','This City exist.');
CALL osae_sp_object_type_property_add('CITY','Population','Integer','','0',0,0,'Current population of this city.');
CALL osae_sp_object_type_property_add('CITY','Tax Rate','Float','','',0,0,'Current Tax Rate for this city.');
CALL osae_sp_object_type_property_add('CITY','URL','String','','',0,0,'URL for this City\'s website.');
CALL osae_sp_object_type_property_add('CITY','Mayor','String','','',0,0,'Current Mayor of this city.');
CALL osae_sp_object_type_property_add('CITY','Zip Code','Integer','','0',0,0,'Zip Code for this city.');
CALL osae_sp_object_type_property_add('CITY','Elevation','Integer','','0',0,0,'Elevation above sea level for this city.');

-- Country Object
CALL osae_sp_object_type_add ('COUNTRY','Core Type: Locations','SYSTEM','PLACE',0,1,1,1,'Country Location.');
CALL osae_sp_object_type_state_add('COUNTRY','ON','Exists','This  Country exist.');
CALL osae_sp_object_type_property_add('COUNTRY','Population','Integer','','0',0,0,'Current population of this city.');
CALL osae_sp_object_type_property_add('COUNTRY','URL','String','','',0,0,'URL for this Country\'s Website.');
CALL osae_sp_object_type_property_add('COUNTRY','President','String','','',0,0,'Name of the current President.');
CALL osae_sp_object_type_property_add('COUNTRY','Vice President ','String','','',0,0,'Name of the current Vice-President.');
CALL osae_sp_object_type_property_add('COUNTRY','Total Area','String','','',0,0,'Total square miles of this country.');
CALL osae_sp_object_type_property_add('COUNTRY','GDP','String','','',0,0,'Gross Domestic Product Value for this country.');

-- House Object
CALL osae_sp_object_type_add ('HOUSE','Core Type: Locations','SYSTEM','PLACE',0,1,1,1,'House location in a City.');
CALL osae_sp_object_type_state_add('HOUSE','ON','Occupied','This State represents that the House is Occupied.');
CALL osae_sp_object_type_state_add('HOUSE','OFF','Vacant','This State represents that the House is Vacant.');
CALL osae_sp_object_type_event_add('HOUSE','ON','Occupied','This event will fire when the state is changed to Occupied.');
CALL osae_sp_object_type_event_add('HOUSE','OFF','Vacated','This event will fire when the state is changed to Vacant.');
CALL osae_sp_object_type_method_add('HOUSE','ON','Occupy','','','','','Executing this method will set the state to Occupied.');
CALL osae_sp_object_type_method_add('HOUSE','OFF','Vacate','','','','','Executing this method will set the state to Vacant.');
CALL osae_sp_object_type_property_add('HOUSE','OFF TIMER','Integer','','-1',1,0,'The number of seconds before the house changes to Vacate.  (-1 = Disabled)');
CALL osae_sp_object_type_property_add('HOUSE','Temperature','Integer','','0',1,0,'The current temperature of the house.');
CALL osae_sp_object_type_property_add('HOUSE','Occupants','String','','',0,0,'A list of who is currently in the house.');
CALL osae_sp_object_type_property_add('HOUSE','Light Level','Integer','','100',1,0,'The current Light Level of this house.');
CALL osae_sp_object_type_property_add('HOUSE','External Access Points','Integer','','0',0,0,'Enter the number of External Access Points in the House.');
CALL osae_sp_object_type_property_add('HOUSE','Occupant Count','Integer','','0',1,0,'The number of occupants in this house.');

-- Place Object
CALL osae_sp_object_type_add ('PLACE','Core Type: Locations','SYSTEM','PLACE',0,1,1,1,'Place Location.
This is the Core-Type for all Locations or Places.');
CALL osae_sp_object_type_state_add('PLACE','ON','Occupied','This Place is Occupied.');
CALL osae_sp_object_type_state_add('PLACE','OFF','Vacant','This Place is Vacant.');
CALL osae_sp_object_type_event_add('PLACE','ON','Occupied','This event will fire when the state is changed to Occupied.');
CALL osae_sp_object_type_event_add('PLACE','OFF','Vacated','This event will fire when the state is changed to Vacant.');
CALL osae_sp_object_type_method_add('PLACE','ON','Occupy','','','','','Executing this method will set the state to Occupied.');
CALL osae_sp_object_type_method_add('PLACE','OFF','Vacate','','','','','Executing this method will set the state to Vacant.');
CALL osae_sp_object_type_property_add('PLACE','OFF TIMER','Integer','','-1',0,0,'The number of seconds before this Place is set to Vacate.  (-1 = Disabled)');
CALL osae_sp_object_type_property_add('PLACE','Temperature','Integer','','0',0,0,'The current temperature of this place.');
CALL osae_sp_object_type_property_add('PLACE','Occupants','String','','',0,0,'A List of current occupants.');
CALL osae_sp_object_type_property_add('PLACE','Light Level','Integer','','100',0,0,'The current Light Level of this place.');
CALL osae_sp_object_type_property_add('PLACE','Occupant Count','Integer','','0',0,0,'The current occupancy count for this place.');

-- Room Object
CALL osae_sp_object_type_add ('ROOM','Core Type: Locations','SYSTEM','PLACE',0,1,1,1,'Room location in a House.');
CALL osae_sp_object_type_state_add('ROOM','ON','Occupied','This Room is Occupied.');
CALL osae_sp_object_type_state_add('ROOM','OFF','Vacant','This Room is Vacant.');
CALL osae_sp_object_type_event_add('ROOM','ON','Occupied','This event will fire when the state is changed to Occupied.');
CALL osae_sp_object_type_event_add('ROOM','OFF','Vacated','This event will fire when the state is changed to Vacant.');
CALL osae_sp_object_type_method_add('ROOM','ON','Occupy','','','','','Executing this method will set the state to Occupied.');
CALL osae_sp_object_type_method_add('ROOM','OFF','Vacate','','','','','Executing this method will set the state to Vacant.');
CALL osae_sp_object_type_property_add('ROOM','OFF TIMER','Integer','','-1',1,0,'The number of seconds before this Room is set to Vacate.  (-1 = Disabled)');
CALL osae_sp_object_type_property_add('ROOM','Temperature','Integer','','0',1,0,'This property will contain the Temperature of this room.');
CALL osae_sp_object_type_property_add('ROOM','External Access Points','Integer','','0',0,0,'Enter the total number of External Access points to this room.');
CALL osae_sp_object_type_property_add('ROOM','Internal Access Points','Integer','','0',0,0,'Enter the total number of Internal Access points to this room.');
CALL osae_sp_object_type_property_add('ROOM','Light Level','Integer','','100',1,0,'Enter the Light Level of this Room.');
CALL osae_sp_object_type_property_add('ROOM','Occupants','String','','',0,0,'This property will contain the names of the Occupants.');
CALL osae_sp_object_type_property_add('ROOM','Occupant Count','Integer','','0',1,0,'This property will contain the total number of Occupants.');

-- State Object
CALL osae_sp_object_type_add ('STATE','Core Type: Locations','SYSTEM','PLACE',0,1,1,1,'State location in a Country.');
CALL osae_sp_object_type_state_add('STATE','ON','Exists','This state represents that this State exist.');
CALL osae_sp_object_type_property_add('STATE','Population','Integer','','0',0,0,'Enter the current population of this State.');
CALL osae_sp_object_type_property_add('STATE','URL','String','','',0,0,'Enter the URL for this State\'s website.');
CALL osae_sp_object_type_property_add('STATE','Governor','String','','',0,0,'Enter the name of the Current Governor.');
CALL osae_sp_object_type_property_add('STATE','Capital','String','','',0,0,'Enter the Capital City of this State.');
CALL osae_sp_object_type_property_add('STATE','Latitude','String','','',0,0,'Enter the Latitude for this State.');
CALL osae_sp_object_type_property_add('STATE','Longitude','String','','',0,0,'Enter the Longitude for this State.');
CALL osae_sp_object_type_property_add('STATE','Median Household Income','Integer','','0',0,0,'Enter the current average household income for this State.');

-- Bluetooth Plugin
CALL osae_sp_object_type_add ('BLUETOOTH','Bluetooth Plugin','Bluetooth','PLUGIN',1,0,0,1,'Bluetooth Plugin');
CALL osae_sp_object_type_state_add('BLUETOOTH','ON','Running','Bluetooth plugin is running');
CALL osae_sp_object_type_state_add('BLUETOOTH','OFF','Stopped','Bluetooth plugin is stopped');
CALL osae_sp_object_type_event_add('BLUETOOTH','ON','Started','Bluetooth plugin started');
CALL osae_sp_object_type_event_add('BLUETOOTH','OFF','Stopped','Bluetooth plugin stopped');
CALL osae_sp_object_type_method_add('BLUETOOTH','ON','Start','','','','','Start the Bluetooth plugin');
CALL osae_sp_object_type_method_add('BLUETOOTH','OFF','Stop','','','','','Stop the Bluetooth plugin');
CALL osae_sp_object_type_property_add('BLUETOOTH','Scan Interval','Integer','','60',1,1,'How many Seconds in between scans');
CALL osae_sp_object_type_property_add('BLUETOOTH','Discover Length','Integer','','8',0,1,'How many Seconds spent scanning each cycle');
CALL osae_sp_object_type_property_add('BLUETOOTH','Learning Mode','Boolean','','TRUE',1,1,'Automatically create Objects for newly scanned devices');
CALL osae_sp_object_type_property_add('BLUETOOTH','System Plugin','Boolean','','FALSE',0,1,'');
CALL osae_sp_object_type_property_add('BLUETOOTH','Debug','Boolean','','FALSE',0,1,'Use Debug to show extra logs');
CALL osae_sp_object_type_property_add('BLUETOOTH','Trust Level','Integer','','90',0,1,'Trust Level 0-100');
CALL osae_sp_object_type_property_add('BLUETOOTH','Version','String','','',0,1,'Version of the Event Ghost plugin');
CALL osae_sp_object_type_property_add('BLUETOOTH','Author','String','','',0,1,'Author of the Event Ghost plugin');

CALL osae_sp_object_type_add ('BLUETOOTH DEVICE','Bluetooth Device','','THING',0,0,0,1,'Bluetooth Device');
CALL osae_sp_object_type_state_add('BLUETOOTH DEVICE','ON','Here','Device is here');
CALL osae_sp_object_type_state_add('BLUETOOTH DEVICE','OFF','Gone','Device is gone');
CALL osae_sp_object_type_event_add('BLUETOOTH DEVICE','ON','Arrived','Device arrived');
CALL osae_sp_object_type_event_add('BLUETOOTH DEVICE','OFF','Left','Device Left');
CALL osae_sp_object_type_method_add('BLUETOOTH DEVICE','ON','Arriving','','','','','Set Device to here');
CALL osae_sp_object_type_method_add('BLUETOOTH DEVICE','OFF','Leaving','','','','','Set Device to gone');
CALL osae_sp_object_type_property_add('BLUETOOTH DEVICE','Discover Type','String','','',0,1,'0 - Use all methods.
1 - Discover Devices.
2 - Get Service Records.
3 - Connection Attempt.');

-- Email Plugin
CALL osae_sp_object_type_add ('EMAIL','Email Plugin','Email','PLUGIN',1,0,0,1,'Email plugin.');
CALL osae_sp_object_type_state_add('EMAIL','ON','Running','The Email plugin is Running.');
CALL osae_sp_object_type_state_add('EMAIL','OFF','Stopped','The Email plugin is Stopped.');
CALL osae_sp_object_type_event_add('EMAIL','ON','Started','The Email plugin Started.');
CALL osae_sp_object_type_event_add('EMAIL','OFF','Stopped','The Email plugin Stopped.');
CALL osae_sp_object_type_event_add('EMAIL','EMAIL SENT','Email Sent','The Email plugin successfully sent a message.');
CALL osae_sp_object_type_method_add('EMAIL','ON','Start','','','','','Start the Email plugin.');
CALL osae_sp_object_type_method_add('EMAIL','OFF','Stop','','','','','Stop the Email plugin.');
CALL osae_sp_object_type_method_add('EMAIL','SEND EMAIL','Send Email','TO','Message','','Test Message','');
CALL osae_sp_object_type_property_add('EMAIL','SMTP Server','String','','',0,1,'Set the full URL to the SMTP server.');
CALL osae_sp_object_type_property_add('EMAIL','SMTP Port','String','','',0,1,'Set the SMTP Port number.');
CALL osae_sp_object_type_property_add('EMAIL','ssl','Boolean','','TRUE',0,1,'Select True/False if the SMTP server requires SSL encryption.');
CALL osae_sp_object_type_property_add('EMAIL','Username','String','','',0,1,'Enter the Username used to Login to the SMTP server.');
CALL osae_sp_object_type_property_add('EMAIL','Password','String','','',0,1,'Enter the Email Password for the SMTP server.');
CALL osae_sp_object_type_property_add('EMAIL','From Address','String','','',0,1,'Enter the email address Emails will be sent from.');
CALL osae_sp_object_type_property_add('EMAIL','System Plugin','Boolean','','TRUE',0,0,'Is the Email Plugin is a system plugin.');
CALL osae_sp_object_type_property_add('EMAIL','Trust Level','Integer','','90',0,1,'The Trust Level of the Email plugin.');
CALL osae_sp_object_type_property_add('EMAIL','Version','String','','',0,0,'Version of the Email Plugin.');
CALL osae_sp_object_type_property_add('EMAIL','Author','String','','',0,0,'Author of the Email Plugin.');

-- Jabber Plugin
CALL osae_sp_object_type_add ('JABBER','Jabber Plugin','Jabber','PLUGIN',1,1,0,1,'Jabber plugin.
The Jabber plugin is used to enable communication
with OSA via a jabber instant messager like Google Talk.');
CALL osae_sp_object_type_state_add('JABBER','ON','Running','The Jabber plugin is Running.');
CALL osae_sp_object_type_state_add('JABBER','OFF','Stopped','The Jabber plugin is NOT running.');
CALL osae_sp_object_type_event_add('JABBER','ON','Started','The Jabber plugin Started.');
CALL osae_sp_object_type_event_add('JABBER','OFF','Stopped','The Jabber plugin Stopped.');
CALL osae_sp_object_type_method_add('JABBER','SEND MESSAGE','Send Message','To','Message','','','Send Message');
CALL osae_sp_object_type_method_add('JABBER','SEND FROM LIST','Send From List','To','List','','','Send From List');
CALL osae_sp_object_type_method_add('JABBER','SEND QUESTION','Send Question','To','','','','Send Question');
CALL osae_sp_object_type_method_add('JABBER','OFF','Stop','','','','','Stop the Jabber plugin.');
CALL osae_sp_object_type_method_add('JABBER','ON','Start','','','','','Start the Jabber plugin.');
CALL osae_sp_object_type_property_add('JABBER','Username','String','','',0,1,'Enter the Username to login to the Jabber service.');
CALL osae_sp_object_type_property_add('JABBER','Password','String','','',0,1,'Enter the Password used to Login to the Jabber service.');
CALL osae_sp_object_type_property_add('JABBER','System Plugin','Boolean','','TRUE',0,0,'Is the Jabber Plugin is a system plugin?');
CALL osae_sp_object_type_property_add('JABBER','Debug','Boolean','','FALSE',0,1,'Add extra logging to help with Debugging.');
CALL osae_sp_object_type_property_add('JABBER','Trust Level','Integer','','90',0,1,'The Trust Level of the Jabber plugin.');
CALL osae_sp_object_type_property_add('JABBER','Version','String','','',0,0,'Version of the Jabber Plugin.');
CALL osae_sp_object_type_property_add('JABBER','Author','String','','',0,0,'Author of the Jabber Plugin.');

-- Network Monitor Plugin
CALL osae_sp_object_type_add ('NETWORK MONITOR','Network Monitor Plugin','Network Monitor','PLUGIN',1,0,0,1,'Network Monitor plugin.
The Network Monitor simply pings network devices to determine their availability.');
CALL osae_sp_object_type_state_add('NETWORK MONITOR','ON','Running','The Network Monitor plugin is Running.');
CALL osae_sp_object_type_state_add('NETWORK MONITOR','OFF','Stopped','The Network Monitor plugin is Stopped.');
CALL osae_sp_object_type_event_add('NETWORK MONITOR','ON','Started','The Network Monitor plugin Started.');
CALL osae_sp_object_type_event_add('NETWORK MONITOR','OFF','Stopped','The Network Monitor plugin Stopped.');
CALL osae_sp_object_type_method_add('NETWORK MONITOR','ON','Start','','','','','Start the Network Monitor plugin.');
CALL osae_sp_object_type_method_add('NETWORK MONITOR','OFF','Stop','','','','','Stop the Network Monitor plugin.');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','System Plugin','Boolean','','TRUE',0,0,'Is the Network Monitor Plugin is a system plugin?');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Poll Interval','Integer','','30',0,1,'Enter the number of seconds to poll for connected devices,');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Debug','Boolean','','FALSE',0,1,'Add extra logging to help with Debugging.');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Version','String','','',0,0,'Version of the Network Monitor Plugin.');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Author','String','','',0,0,'Author of the Network Monitor Plugin.');
CALL osae_sp_object_type_property_add('NETWORK MONITOR','Trust Level','Integer','','90',0,1,'The Trust Level of the Network Monitor plugin.');

-- Plugin object
CALL osae_sp_object_type_add ('PLUGIN','Generic Plugin Class','','PLUGIN',1,1,0,1,'Generic plugin Object.
Plugin are used to create controlability to other objects or devices.');
CALL osae_sp_object_type_state_add('PLUGIN','ON','Running','The plugin is Running.');
CALL osae_sp_object_type_state_add('PLUGIN','OFF','Stopped','The plugin is Stopped.');
CALL osae_sp_object_type_state_add('PLUGIN','ERROR','Error','The plugin is in Error.');
CALL osae_sp_object_type_event_add('PLUGIN','ON','Started','The plugin Started.');
CALL osae_sp_object_type_event_add('PLUGIN','OFF','Stopped','The plugin Stopped.');
CALL osae_sp_object_type_event_add('PLUGIN','ERROR','Error','This event will fire when the plugin has an error..');
CALL osae_sp_object_type_method_add('PLUGIN','ON','Start','','','','','Start the plugin.');
CALL osae_sp_object_type_method_add('PLUGIN','OFF','Stop','','','','','Stop the plugin.');
CALL osae_sp_object_type_property_add('PLUGIN','Port','Integer','','0',0,1,'Enter the COM port');
CALL osae_sp_object_type_property_add('PLUGIN','System Plugin','Boolean','','FALSE',0,0,'Is this Plugin is a system plugin?');

-- Powershell Plugin
CALL osae_sp_object_type_add ('POWERSHELL','PowerShell Script Processor','','PLUGIN',0,0,0,1,'The PowerShell plugin.
This plugin can be used within OSA and outside of OSA to command OSA.');
CALL osae_sp_object_type_state_add('POWERSHELL','ON','Running','The PowerShell plugin is Running.');
CALL osae_sp_object_type_state_add('POWERSHELL','OFF','Stopped','The PowerShell plugin is Stopped.');
CALL osae_sp_object_type_event_add('POWERSHELL','ON','Started','The PowerShell plugin Started.');
CALL osae_sp_object_type_event_add('POWERSHELL','OFF','Stopped','The PowerShell plugin Stopped.');
CALL osae_sp_object_type_method_add('POWERSHELL','ON','Start','','','','','Start the PowerShell plugin.');
CALL osae_sp_object_type_method_add('POWERSHELL','OFF','Stop','','','','','Stop the PowerShell plugin.');
CALL osae_sp_object_type_method_add('POWERSHELL','RUN SCRIPT','RUN SCRIPT','','','','','Run the selected script.');
CALL osae_sp_object_type_property_add('POWERSHELL','System Plugin','Boolean','','TRUE',0,0,'Is the PowerShell Plugin is a system plugin?');
CALL osae_sp_object_type_property_add('POWERSHELL','Trust Level','Integer','','90',0,0,'The Trust Level of the PowerShell plugin.');
CALL osae_sp_object_type_property_add('POWERSHELL','Version','String','','',0,0,'Version of the PowerShell Plugin.');
CALL osae_sp_object_type_property_add('POWERSHELL','Author','String','','',0,0,'Author of the PowerShell Plugin.');

-- Rest Plugin
CALL osae_sp_object_type_add ('REST','Rest API','Rest','PLUGIN',0,0,0,1,'The Rest plugin.
The Rest plugin an be used to execute methods, get object
information, find all objects of a certain type, etc.');
CALL osae_sp_object_type_state_add('REST','OFF','Stopped','The Rest plugin is Stopped.');
CALL osae_sp_object_type_state_add('REST','ON','Running','The Rest plugin is Running.');
CALL osae_sp_object_type_event_add('REST','OFF','Stopped','The Rest plugin Stopped.');
CALL osae_sp_object_type_event_add('REST','ON','Started','The Rest plugin Started.');
CALL osae_sp_object_type_method_add('REST','OFF','Stop','','','','','Stop the Rest plugin.');
CALL osae_sp_object_type_method_add('REST','ON','Start','','','','','Start the Rest plugin.');
CALL osae_sp_object_type_property_add('REST','System Plugin','Boolean','','TRUE',0,0,'Is the Rest Plugin is a system plugin.');
CALL osae_sp_object_type_property_add('REST','Show Help','Boolean','','TRUE',0,1,'Select True/False for the Rest plugin to display the Help page.');
CALL osae_sp_object_type_property_add('REST','Rest Port','Integer','','8732',0,1,'Enter the system port number the Rest plugin will listen on.');
CALL osae_sp_object_type_property_add('REST','Version','String','','',0,0,'Version of the Rest Plugin.');
CALL osae_sp_object_type_property_add('REST','Author','String','','',0,0,'Author of the Rest Plugin.');
CALL osae_sp_object_type_property_add('REST','Trust Level','Integer','','90',0,0,'The Trust Level of the Rest plugin.');

-- Script Processor Plugin
CALL osae_sp_object_type_add ('SCRIPT PROCESSOR','Generic Plugin Class','Script Processor','PLUGIN',1,0,0,1,'The Script Processor plugin.
This plugin is used to execute scripts within the OSA system
to automate certain objects or devices.');
CALL osae_sp_object_type_state_add('SCRIPT PROCESSOR','ON','Running','The Script Processor plugin is Running.');
CALL osae_sp_object_type_state_add('SCRIPT PROCESSOR','OFF','Stopped','The Script Processor plugin is Stopped.');
CALL osae_sp_object_type_event_add('SCRIPT PROCESSOR','ON','Started','The Script Processor plugin Started.');
CALL osae_sp_object_type_event_add('SCRIPT PROCESSOR','OFF','Stopped','The Script Processor plugin Stopped.');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','ON','Start','','','','','Start the Script Processor plugin.');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','OFF','Stop','','','','','Stop the Script Processor plugin.');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','RUN SCRIPT','Run Script','','','','','Run the selected script.');
CALL osae_sp_object_type_method_add('SCRIPT PROCESSOR','RUN READERS','Run Readers','','','','','Run the selected Reader.');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','System Plugin','Boolean','','TRUE',0,0,'Is the Script Processor Plugin is a system plugin?');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Debug','Boolean','','FALSE',0,1,'Add extra logging to help with Debugging.');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Trust Level','Integer','','50',0,1,'The Trust Level of the Script Processor plugin.');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Version','String','','',0,1,'Version of the Script Processor Plugin.');
CALL osae_sp_object_type_property_add('SCRIPT PROCESSOR','Author','String','','',0,1,'Author of the Script Processor Plugin.');

-- Speech Plugin
CALL osae_sp_object_type_add ('SPEECH','Speech plugin','Speech','PLUGIN',1,0,0,1,'Speech plugin');
CALL osae_sp_object_type_state_add('SPEECH','ON','Running','The Speech plugin is Running');
CALL osae_sp_object_type_state_add('SPEECH','OFF','Stopped','The Speech plugin is Stopped!');
CALL osae_sp_object_type_event_add('SPEECH','ON','Started','The Speech plugin was Started');
CALL osae_sp_object_type_event_add('SPEECH','OFF','Stopped','The Speech plugin was Stopped!');
CALL osae_sp_object_type_method_add('SPEECH','ON','On','','','','','Start the Speech plugin');
CALL osae_sp_object_type_method_add('SPEECH','OFF','Off','','','','','Stop the Speech plugin');
CALL osae_sp_object_type_method_add('SPEECH','SPEAK','Say','Message','','Hello','','Say');
CALL osae_sp_object_type_method_add('SPEECH','SPEAKFROM','Say From List','Object Name','Property Name','Speech List','Greetings','Say From List');
CALL osae_sp_object_type_method_add('SPEECH','PLAY','Play','File','','','','Play');
CALL osae_sp_object_type_method_add('SPEECH','PLAYFROM','Play From List','List','','','','Play From List');
CALL osae_sp_object_type_method_add('SPEECH','STOP','Stop','','','','','Stop');
CALL osae_sp_object_type_method_add('SPEECH','PAUSE','Pause','','','','','Pause');
CALL osae_sp_object_type_method_add('SPEECH','MUTEVR','Mute the Microphone','','','','','Mute the Microphone');
CALL osae_sp_object_type_method_add('SPEECH','SETVOICE','Set Voice','Voice','','Anna','','Set Voice');
CALL osae_sp_object_type_method_add('SPEECH','SETTTSRATE','Set TTS Rate','Rate','','0','','Set TTS Rate');
CALL osae_sp_object_type_method_add('SPEECH','SETTTSVOLUME','Set TTS Volume','Volume','','100','','Set TTS Volume');
CALL osae_sp_object_type_property_add('SPEECH','Voice','String','','',0,1,'Voice');
CALL osae_sp_object_type_property_add('SPEECH','Voices','List','','',0,0,'Available Voices');
CALL osae_sp_object_type_property_add('SPEECH','System Plugin','Boolean','','FALSE',0,1,'System Plugin');
CALL osae_sp_object_type_property_add('SPEECH','TTS Rate','Integer','','',0,1,'TTS Rate');
CALL osae_sp_object_type_property_add('SPEECH','TTS Volume','Integer','','',0,1,'TTS Volume');
CALL osae_sp_object_type_property_add('SPEECH','Speaking','Boolean','','FALSE',0,1,'Speaking');
CALL osae_sp_object_type_property_add('SPEECH','Debug','Boolean','','FALSE',0,1,'Debug');
CALL osae_sp_object_type_property_add('SPEECH','Trust Level','Integer','','90',0,1,'Speech Trust Level');
CALL osae_sp_object_type_property_add('SPEECH','Version','String','','',0,1,'Speech plugin Version');
CALL osae_sp_object_type_property_add('SPEECH','Author','String','','',0,1,'Speech plugin Author');

-- VR Client Plugin
CALL osae_sp_object_type_add ('VR CLIENT','Generic Plugin Class','','PLUGIN',1,1,0,1,'The Voice Recognition plugin.
This Plugin enables OSA to recognize Speech inputs to perform operations within OSA.');
CALL osae_sp_object_type_state_add('VR CLIENT','ON','Running','The VR Client plugin is Running.');
CALL osae_sp_object_type_state_add('VR CLIENT','OFF','Stopped','The VR Client plugin is Stopped.');
CALL osae_sp_object_type_event_add('VR CLIENT','ON','Started','The VR Client plugin Started.');
CALL osae_sp_object_type_event_add('VR CLIENT','OFF','Stopped','The VR Client plugin Stopped.');
CALL osae_sp_object_type_method_add('VR CLIENT','ON','On','','','','','Start the VR Client plugin.');
CALL osae_sp_object_type_method_add('VR CLIENT','OFF','Off','','','','','Stop the VR Client plugin.');
CALL osae_sp_object_type_method_add('VR CLIENT','SPEAK','Say','','','','','Say the entered value.');
CALL osae_sp_object_type_method_add('VR CLIENT','SPEAKFROM','Say From List','','','','','Say the selected value from a list.');
CALL osae_sp_object_type_method_add('VR CLIENT','PLAY','Play','','','','','Play the entered value.');
CALL osae_sp_object_type_method_add('VR CLIENT','PLAYFROM','Play From List','','','','','Play the selected value from a list.');
CALL osae_sp_object_type_method_add('VR CLIENT','STOP','Stop','','','','','Stop the VR Client speaking.');
CALL osae_sp_object_type_method_add('VR CLIENT','PAUSE','Pause','','','','','Pause the VR Client plugin.');
CALL osae_sp_object_type_method_add('VR CLIENT','MUTEVR','Mute the Microphone','','','','','Mute the VR Client Microphone.');
CALL osae_sp_object_type_method_add('VR CLIENT','SETVOICE','Set Voice','','','','','Set the VR Client Voice property.');
CALL osae_sp_object_type_property_add('VR CLIENT','VR Input Muted','Boolean','','TRUE',0,0,'Is this VR Client is Muted.');
CALL osae_sp_object_type_property_add('VR CLIENT','Voice','String','','',0,1,'The Voice this VR client will use when speaking.');
CALL osae_sp_object_type_property_add('VR CLIENT','Voices','List','','',0,0,'List of available voices that this VR Client can use.');
CALL osae_sp_object_type_property_add('VR CLIENT','VR Enabled','Boolean','','FALSE',0,0,'Is this VR Client is enabled.');
CALL osae_sp_object_type_property_add('VR CLIENT','VR Sleep Pattern','String','','Thanks',0,1,'Speech Pattern used to put this VR Client to sleep.');
CALL osae_sp_object_type_property_add('VR CLIENT','VR Wake Pattern','String','','VR Wake',0,1,'Speech Pattern used to wake this VR Client.');
CALL osae_sp_object_type_property_add('VR CLIENT','Can Hear this Plugin','Object','','Speech',0,1,'Object that can hear this VR Client.');
CALL osae_sp_object_type_property_add('VR CLIENT','Trust Level','Integer','','50',0,1,'Trust Level the VR Client plugin.');

-- Weather Plugin
CALL osae_sp_object_type_add ('WEATHER PLUGIN','Weather Plugin','','PLUGIN',1,0,0,1,'The Weather plugin.
This Plugin retrieves weather forecast and conditions from
the NOAA website based on your zip code.');
CALL osae_sp_object_type_state_add('WEATHER PLUGIN','ON','Running','The Weather plugin is Running.');
CALL osae_sp_object_type_state_add('WEATHER PLUGIN','OFF','Stopped','The Weather plugin is Stopped.');
CALL osae_sp_object_type_event_add('WEATHER PLUGIN','ON','Started','The Weather plugin Started.');
CALL osae_sp_object_type_event_add('WEATHER PLUGIN','OFF','Stopped','The Weather plugin Stopped.');
CALL osae_sp_object_type_method_add('WEATHER PLUGIN','ON','Start','','','','','Start the Weather plugin.');
CALL osae_sp_object_type_method_add('WEATHER PLUGIN','OFF','Stop','','','','','Stop the Weather plugin.');
CALL osae_sp_object_type_method_add('WEATHER PLUGIN','UPDATE','Update','','','','','Update Weather plugin information');
CALL osae_sp_object_type_property_add('WEATHER PLUGIN','Feed URL','String','','',0,1,'Feed URL required to retrieve the NOAA weather information.');
CALL osae_sp_object_type_property_add('WEATHER PLUGIN','Zipcode','String','','',0,1,'Zip Code for the location to retrieve the Weather information.');
CALL osae_sp_object_type_property_add('WEATHER PLUGIN','Update Interval','String','','30',0,1,'Number of minutes for the Weather plugin to Refresh
the weather conditions and forecast information retrieved.');
CALL osae_sp_object_type_property_add('WEATHER PLUGIN','System Plugin','Boolean','','TRUE',0,0,'Is the Weather Plugin is a system plugin.');

-- Web Server Plugin
CALL osae_sp_object_type_add ('WEB SERVER','OSA Web Server Plugin','Web Server','PLUGIN',1,0,0,1,'The Web Server plugin.
This is used to run the WebUI interface and all representing pages.');
CALL osae_sp_object_type_state_add('WEB SERVER','ON','Running','Web Service is Running.');
CALL osae_sp_object_type_state_add('WEB SERVER','OFF','Stopped','Web Service is Stopped.');
CALL osae_sp_object_type_event_add('WEB SERVER','ON','Started','Web Service Started.');
CALL osae_sp_object_type_event_add('WEB SERVER','OFF','Stopped','Web Serviced Stopped!');
CALL osae_sp_object_type_method_add('WEB SERVER','ON','Start','','','','','Start the Web Service.');
CALL osae_sp_object_type_method_add('WEB SERVER','OFF','Stop','','','','','Stop the Web Service.');
CALL osae_sp_object_type_property_add('WEB SERVER','System Plugin','Boolean','','TRUE',0,0,'Is the Web Server Plugin is a system plugin?');
CALL osae_sp_object_type_property_add('WEB SERVER','Timeout','Integer','','60',0,0,'How long you can sit idle on the Web UI before you are required to sign in again.');
CALL osae_sp_object_type_property_add('WEB SERVER','Hide Controls','Boolean','','FALSE',0,0,'Hide the Controls that make up Screens on the Object page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Version','String','','',0,0,'This is the installed version of the Web Server Plugin.
This property is automatically populated by the system.');
CALL osae_sp_object_type_property_add('WEB SERVER','Author','String','','',0,0,'This is the Author of the Web Server Plugin.
This property is automatically populated by the system.');
CALL osae_sp_object_type_property_add('WEB SERVER','Config Trust','Integer','','69',0,1,'Enter the minimum trust required to access the Config page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Analytics Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Analytics page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Debug Log Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Debug Log page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Default Screen','String','','',0,0,'The First Screen displayed after logging into the Web UI.');
CALL osae_sp_object_type_property_add('WEB SERVER','Event Log Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Event Log page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Images Add Trust','Integer','','55',0,1,'Enter the minimum trust required to add Images to the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Images Delete Trust','Integer','','60',0,1,'Enter the minimum trust required to delete Images from the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Images Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Images page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Logs Clear Trust','Integer','','60',0,1,'Enter the minimum trust required to clear server logs');
CALL osae_sp_object_type_property_add('WEB SERVER','Logs Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Server Logs page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Management Trust','Integer','','45',0,0,'Enter the minimum trust required to access the Logs page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Method Log Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Method Logs page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Add Trust','Integer','','50',0,1,'Enter the minimum trust required to add Objects to the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Delete Trust','Integer','','60',0,1,'Enter the minimum trust required to delete Objects from the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Objects page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Update Trust','Integer','','55',0,1,'Enter the minimum trust required to update Objects in the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Add Trust','Integer','','45',0,1,'Enter the minimum trust required to add Object Types to the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Delete Trust','Integer','','60',0,1,'Enter the minimum trust required to delete Object Types from the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Object Type page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Object Type Update Trust','Integer','','55',0,1,'Enter the minimum trust required to update Object Types in the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Add Trust','Integer','','50',0,1,'Enter the minimum trust required to add Patterns to the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Delete Trust','Integer','','60',0,1,'Enter the minimum trust required to delete Patterns from the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Trust','Integer','','50',0,1,'Enter the minimum trust required to access the Pattern page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Update Trust','Integer','','55',0,1,'Enter the minimum trust required to update Patterns in the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Add Trust','Integer','','45',0,1,'Enter the minimum trust required to add Readers to the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Delete Trust','Integer','','60',0,1,'Enter the minimum trust required to delete Readers from the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Reader page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Update Trust','Integer','','55',0,1,'Enter the minimum trust required to update Readers in the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Add Trust','Integer','','50',0,1,'Enter the minimum trust required to add Schedules to the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Delete Trust','Integer','','60',0,1,'Enter the minimum trust required to delete Schedules from the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Trust','Integer','','50',0,1,'Enter the minimum trust required to access the Schedule page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Update Trust','Integer','','55',0,1,'Enter the minimum trust required to update Schedules in the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Screen Trust','Integer','','20',0,1,'Enter the minimum trust required to access the Screens page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Script Add Trust','Integer','','45',0,1,'Enter the minimum trust required to add Scripts to the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Script Delete Trust','Integer','','60',0,1,'Enter the minimum trust required to delete Scripts from the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Script Object Add Trust','Integer','','45',0,1,'Enter the minimum trust required to add Object Scripts to the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Script ObjectType Add Trust','Integer','','60',0,1,'Enter the minimum trust required to add Object Type Scripts to the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Script Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Scripts page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Script Update Trust','Integer','','55',0,1,'Enter the minimum trust required to update Scripts in the database.');
CALL osae_sp_object_type_property_add('WEB SERVER','Server Log Trust','Integer','','50',0,1,'Enter the minimum trust required to access the Server Logs page.');
CALL osae_sp_object_type_property_add('WEB SERVER','Values Trust','Integer','','45',0,1,'Enter the minimum trust required to access the Values page.');
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Update Trust','Integer','','50',0,1,'Enter the minimum trust required to update Object Types in the database.');

-- WUnderground Plugin
CALL osae_sp_object_type_add ('WUNDERGROUND','Weather Underground','WUnderground','PLUGIN',1,0,0,1,'Wunderground plugin.
This Plugin retrieves weather forecast and conditions from
the WUnderground website based on your account.');
CALL osae_sp_object_type_state_add('WUNDERGROUND','ON','Running','The WUnderground plugin is Running.');
CALL osae_sp_object_type_state_add('WUNDERGROUND','OFF','Stopped','The WUnderground plugin is Stopped.');
CALL osae_sp_object_type_event_add('WUNDERGROUND','ON','Started','The WUnderground plugin Started.');
CALL osae_sp_object_type_event_add('WUNDERGROUND','OFF','Stopped','The WUnderground plugin Stopped.');
CALL osae_sp_object_type_method_add('WUNDERGROUND','ON','Start','','','','','Start the WUnderground plugin.');
CALL osae_sp_object_type_method_add('WUNDERGROUND','OFF','Stop','','','','','Stop the WUnderground plugin.');
CALL osae_sp_object_type_method_add('WUNDERGROUND','UPDATE','Update','','','','','Upate the WUnderground plugin information.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','PWS','String','','KMOKANSA61',0,0,'WUnderground Weather Station.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','Conditions Interval','Integer','','5',0,1,'Number of minutes for the WUnderground to Refresh
the weather conditions information retrieved.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','Forecast Interval','Integer','','60',0,1,'Number of minutes for the WUnderground to Refresh
the weather forecast information.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','Metric','Boolean','','FALSE',0,0,'Use the Metric system?');
CALL osae_sp_object_type_property_add('WUNDERGROUND','DuskPre','Integer','','0',0,0,'Number of minutes for Pre-Dusk.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','DuskPost','Integer','','0',0,0,'Number of minutes for Post-Dusk.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','DawnPre','Integer','','0',0,0,'Number of minutes for Pre-Dawn.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','DawnPost','Integer','','0',0,0,'Number of minutes for Post-Dawn.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','State','String','','',0,0,'State in which the WUnderground will search
for conditions and forecast.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','Key','String','','',0,1,'Key provided with your account setup at WUnderground.com.
This is used to login to the service.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','City','String','','',0,0,'City in which the WUnderground will search
for conditions and forecast.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','Trust Level','Integer','','50',0,1,'Trust Level for the WUnderground plugin.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','Debug','Boolean','','FALSE',0,1,'Add extra logging to help with Debugging.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','Version','String','','',0,0,'Version of the WUnderground Plugin.');
CALL osae_sp_object_type_property_add('WUNDERGROUND','Author','String','','',0,0,'Author of the WUnderground Plugin.');

-- Screen Object
CALL osae_sp_object_type_add ('SCREEN','Core Type: Display Screen','','SCREEN',0,1,1,1,'Screen in the Screens application or WebUI Screens page.');
CALL osae_sp_object_type_property_add('SCREEN','Background Image','File','','',0,1,'Image to display as the background for this screen.');

-- Binary Sensor On/Off object
CALL osae_sp_object_type_add ('BINARY SENSOR','Generic On/Off Sensor','','SENSOR',0,1,0,1,'Binary Sensor. (On/Off)
This object is used to receive the status of a Binary Sensor and display the state of the sensor in OSA.');
CALL osae_sp_object_type_state_add('BINARY SENSOR','ON','On','This states that the Binary sensor is On.');
CALL osae_sp_object_type_state_add('BINARY SENSOR','OFF','Off','This states that the Binary sensor is OFF.');
CALL osae_sp_object_type_event_add('BINARY SENSOR','ON','On','This event will fire when the Binary sensor state changes to ON.');
CALL osae_sp_object_type_event_add('BINARY SENSOR','OFF','Off','This event will fire when the Binary sensor state changes to OFF.');
CALL osae_sp_object_type_property_add('BINARY SENSOR','OFF TIMER','Integer','','-1',0,0,'The number of seconds before this Sensor is set to Off. (-1 = Disabled)');

-- Binary Sensor OC object
CALL osae_sp_object_type_add ('BINARY SENSOR OC','Generic Open/Closed Sensor','','SENSOR',0,1,0,1,'Binary Sensor OC. (Open/Closed)
This object is used to receive the status of a Binary Sensor and display the state of the sensor in OSA.');
CALL osae_sp_object_type_state_add('BINARY SENSOR OC','ON','Closed','Binary sensor is Closed.');
CALL osae_sp_object_type_state_add('BINARY SENSOR OC','OFF','Open','Binary sensor is Open.');
CALL osae_sp_object_type_event_add('BINARY SENSOR OC','ON','Closed','Binary Sensor state changed to Closed.');
CALL osae_sp_object_type_event_add('BINARY SENSOR OC','OFF','Open','Binary sensor state changed to Open.');
CALL osae_sp_object_type_property_add('BINARY SENSOR OC','OFF TIMER','Integer','','-1',0,0,'The number of seconds before this Sensor is set to Open.  (-1 = Disabled)');


-- Sensor Object
CALL osae_sp_object_type_add ('SENSOR','TOP Level Sensor Type','','SENSOR',0,1,0,1,'Base Type Binary Sensor.
Sensors are used to receive the status of a connected Sensor and display the state of the sensor in OSA.');
CALL osae_sp_object_type_state_add('SENSOR','ON','On','Sensor is ON.');
CALL osae_sp_object_type_state_add('SENSOR','OFF','Off','Sensor is OFF.');
CALL osae_sp_object_type_event_add('SENSOR','ON','On','Sensor state changed to ON.');
CALL osae_sp_object_type_event_add('SENSOR','OFF','Off','Sensor state changed to OFF.');
CALL osae_sp_object_type_property_add('SENSOR','OFF TIMER','Integer','','-1',0,0,'The number of seconds before this Sensor is set to Off.  (-1 = Disabled)');

-- Service Object
CALL osae_sp_object_type_add ('SERVICE','OSA Service','','SERVICE',0,1,1,1,'OSA service that runs in the background on each machine.
This object is required and must be running in order for OSA to operate.');
CALL osae_sp_object_type_state_add('SERVICE','ON','Running','The OSA Service is Running.');
CALL osae_sp_object_type_state_add('SERVICE','OFF','Stopped','The OSA Service is Stopped.');
CALL osae_sp_object_type_event_add('SERVICE','ON','Started','OSA Service Started.');
CALL osae_sp_object_type_event_add('SERVICE','OFF','Stopped','OSA Service Stopped.');
CALL osae_sp_object_type_method_add('SERVICE','EXECUTE','Execute Command','Program','Arguments','','','This method will initiate the program in parameter 1
with the included arguments specified in parameter 2.');
CALL osae_sp_object_type_method_add('SERVICE','START PLUGIN','Start Plugin','Plugin name','','','','Start the specified plugin in parameter 1.');
CALL osae_sp_object_type_method_add('SERVICE','STOP PLUGIN','Stop Plugin','Plugin name','','','','Stop the specified plugin in parameter 1.');
CALL osae_sp_object_type_method_add('SERVICE','RELOAD PLUGINS','Reload plugins','','','','','Reload all available plugins.');
CALL osae_sp_object_type_method_add('SERVICE','BROADCAST','Broadcast','group','message','Command','Test Message','Group Broadcast Message.');
CALL osae_sp_object_type_property_add('SERVICE','Trust Level','Integer','','90',0,1,'Trust Level for the WUnderground plugin.');

-- Binary Switch Object
CALL osae_sp_object_type_add ('BINARY SWITCH','Binary Switch','','SWITCH',0,1,0,1,'Binary Switch.
A Binary Switch is used to control the On and Off states.');
CALL osae_sp_object_type_state_add('BINARY SWITCH','ON','On','Binary Switch is ON.');
CALL osae_sp_object_type_state_add('BINARY SWITCH','OFF','Off','Binary Switch is OFF.');
CALL osae_sp_object_type_event_add('BINARY SWITCH','ON','On','Binary Switch State changed to ON.');
CALL osae_sp_object_type_event_add('BINARY SWITCH','OFF','Off','Binary Switch State changed to OFF.');
CALL osae_sp_object_type_method_add('BINARY SWITCH','ON','On','','','','','Run an On command on the Binary Switch.');
CALL osae_sp_object_type_method_add('BINARY SWITCH','OFF','Off','','','','','Run an Off command on the Binary Switch.');

-- Multi-Level Switch
CALL osae_sp_object_type_add ('MULTILEVEL SWITCH','Multilevel Switch','','SWITCH',0,1,0,1,'Multi-Level Switch.
A Multi-Level switch is used to control the
On, Off and Level status.');
CALL osae_sp_object_type_state_add('MULTILEVEL SWITCH','ON','On','The Multi-Level Switch is ON.');
CALL osae_sp_object_type_state_add('MULTILEVEL SWITCH','OFF','Off','The Multi-Level Switch is OFF.');
CALL osae_sp_object_type_event_add('MULTILEVEL SWITCH','ON','On','The Multi-Level Switch State changes to ON.');
CALL osae_sp_object_type_event_add('MULTILEVEL SWITCH','OFF','Off','The Multi-Level Switch State changes to OFF.');
CALL osae_sp_object_type_method_add('MULTILEVEL SWITCH','ON','On','Level','100','','','Run an On command on the Multi-Level Switch.');
CALL osae_sp_object_type_method_add('MULTILEVEL SWITCH','OFF','Off','','','','','Run an Off command on the Multi-Level Switch,
and set the state to OFF.');
CALL osae_sp_object_type_property_add('MULTILEVEL SWITCH','Level','String','','',0,0,'The Light Level of this Multi-Level Switch.');

-- Switch Object
CALL osae_sp_object_type_add ('SWITCH','Binary Switch','','SWITCH',0,1,0,1,'Switch.
A Switch is used to control the On and Off states.');
CALL osae_sp_object_type_state_add('SWITCH','ON','On','This states that the Binary Switch is ON.');
CALL osae_sp_object_type_state_add('SWITCH','OFF','Off','This states that the Binary Switch is OFF.');
CALL osae_sp_object_type_event_add('SWITCH','ON','On','This Event will fire when the Switch State changes to ON.');
CALL osae_sp_object_type_event_add('SWITCH','OFF','Off','This Event will fire when the Switch State changes to OFF.');
CALL osae_sp_object_type_method_add('SWITCH','ON','On','','','','','Executing this method will run an On command on the Switch,
and set the state to ON.');
CALL osae_sp_object_type_method_add('SWITCH','OFF','Off','','','','','Executing this method will run an Off command on the Switch,
and set the state to OFF.');

-- System Object
CALL osae_sp_object_type_add ('SYSTEM','Core System Data','SYSTEM','SYSTEM',1,1,1,1,'The main system object.
This is a required object and is used to run the OSA Engine.');
CALL osae_sp_object_type_state_add('SYSTEM','HOME','Home','System is in Home mode.');
CALL osae_sp_object_type_state_add('SYSTEM','AWAY','Away','System is in Away mode.');
CALL osae_sp_object_type_state_add('SYSTEM','SLEEP','Sleep','System is in Sleep mode.');
CALL osae_sp_object_type_event_add('SYSTEM','OCCUPANTS','Set Occupants','Occupant count was set by system.');
CALL osae_sp_object_type_event_add('SYSTEM','AWAY','Away','System was set to Away mode.');
CALL osae_sp_object_type_event_add('SYSTEM','HOME','State Set to Home','System was set to Home mode.');
CALL osae_sp_object_type_event_add('SYSTEM','SLEEP','Sleep','System was set to Sleep mode.');
CALL osae_sp_object_type_event_add('SYSTEM','OCCUPIED LOCATIONS','Occupied Locations Set','Occupied Locations was set.');
CALL osae_sp_object_type_event_add('SYSTEM','PLUGINS ERRORED','Plugins Errored','A plugin has been placed in Error status.');
CALL osae_sp_object_type_method_add('SYSTEM','OCCUPANTS','Set Occupants','Number of Occupants','','0','','Executing this method will set the number of occupants
provided in parameter 1.');
CALL osae_sp_object_type_method_add('SYSTEM','AWAY','Away','','','','','Set System to Away mode.');
CALL osae_sp_object_type_method_add('SYSTEM','HOME','Home','','','','','Set System to Home mode.');
CALL osae_sp_object_type_method_add('SYSTEM','SLEEP','Sleep','','','','','Set System to Sleep mode.');
CALL osae_sp_object_type_property_add('SYSTEM','Occupants','String','','',0,0,'The names of who is occupying.');
CALL osae_sp_object_type_property_add('SYSTEM','ZIP Code','String','','',0,0,'Enter the Zip Code for the system location.');
CALL osae_sp_object_type_property_add('SYSTEM','Latitude','Integer','','0',0,0,'Enter the Latitude of the systems location.');
CALL osae_sp_object_type_property_add('SYSTEM','Longitude','Integer','','0',0,0,'Enter the Longitude of the Systems Location.');
CALL osae_sp_object_type_property_add('SYSTEM','Date','DateTime','','',0,0,'This is the current Date.');
CALL osae_sp_object_type_property_add('SYSTEM','Time','DateTime','','',0,0,'Current Time in Military format.');
CALL osae_sp_object_type_property_add('SYSTEM','Day Of Week','Integer','','0',0,0,'Current Day of the Week.');
CALL osae_sp_object_type_property_add('SYSTEM','Violations','Integer','','0',0,0,'Total alarm violations in series.');
CALL osae_sp_object_type_property_add('SYSTEM','Day Of Month','Integer','','0',0,0,'Current Day of the Month.');
CALL osae_sp_object_type_property_add('SYSTEM','Time AMPM','DateTime','','',0,0,'Time in AM/PM format.');
CALL osae_sp_object_type_property_add('SYSTEM','Occupied Location String','String','','',0,0,'The location names that are currently occupied.');
CALL osae_sp_object_type_property_add('SYSTEM','Occupied Locations','Integer','','0',0,0,'The number of locations that is currently occupied.');
CALL osae_sp_object_type_property_add('SYSTEM','DB Version','String','','',0,1,'Currently installed Database version.');
CALL osae_sp_object_type_property_add('SYSTEM','Debug','Boolean','','TRUE',0,1,'Debug control the amount of details in the logs.');
CALL osae_sp_object_type_property_add('SYSTEM','Prune Logs','Boolean','','TRUE',0,0,'Select True/False if the system should Prune the logs and startup.');
CALL osae_sp_object_type_property_add('SYSTEM','Plugins Found','Integer','','0',1,0,'The total number of plugins found during the system startup process.');
CALL osae_sp_object_type_property_add('SYSTEM','Plugins Running','Integer','','0',1,0,'The number of Plugins that started successfully. ');
CALL osae_sp_object_type_property_add('SYSTEM','Plugins Enabled','Integer','','0',1,0,'Total number of Plugins that are Enabled and will startup automatically.');
CALL osae_sp_object_type_property_add('SYSTEM','Plugins Errored','Integer','','0',1,0,'The number of Plugins that failed to start.');
CALL osae_sp_object_type_property_add('SYSTEM','Plugins','String','','',0,0,'Total number of Plugins that the system is aware of.');
CALL osae_sp_object_type_property_add('SYSTEM','AI Focused on Object Type','String','','',0,0,'System is curious about this Object Type.');
CALL osae_sp_object_type_property_add('SYSTEM','AI Focused on Property','String','','',0,0,'This is what property of the object the system is curious about.');
CALL osae_sp_object_type_property_add('SYSTEM','Trust Level','Integer','','50',0,0,'This is how much the system is trusted.  ');
CALL osae_sp_object_type_property_add('SYSTEM','Occupant Count','Integer','','0',0,0,'Total number of Occupants.');
CALL osae_sp_object_type_property_add('SYSTEM','Detailed Occupancy Enabled','Boolean','','FALSE',0,0,'If TRUE, it will try to guess where everyone is.   Works best with single occupancy.');
CALL osae_sp_object_type_property_add('SYSTEM','Culture','String','','en-US',0,1,'This is the Globalization setting for language, Date and currency settings.');


-- Computer Object
CALL osae_sp_object_type_add ('COMPUTER','Core Type: Computer','','THING',0,1,1,1,'Computer device.
A Computer device can be a desktop,
laptop or any OS LAN enabled device.');
CALL osae_sp_object_type_state_add('COMPUTER','ON','On','This state represents that this Computer is ON.');
CALL osae_sp_object_type_state_add('COMPUTER','OFF','Off','This state represents that this Computer is OFF.');
CALL osae_sp_object_type_event_add('COMPUTER','ON','On','This event will fire when this Computer state is changed to On.');
CALL osae_sp_object_type_event_add('COMPUTER','OFF','Off','This event will fire when this Computer state is changed to Off.');
CALL osae_sp_object_type_method_add('COMPUTER','ON','On','','','','','Executing this method will set this Computer state to On.');
CALL osae_sp_object_type_method_add('COMPUTER','OFF','Off','','','','','Executing this method will set this Computer state to Off.');
CALL osae_sp_object_type_property_add('COMPUTER','Host Name','String','','',0,0,'Enter this Computer\'s Name.');
CALL osae_sp_object_type_property_add('COMPUTER','OS','String','','',0,0,'Enter this Computer\'s Operating system.');
CALL osae_sp_object_type_property_add('COMPUTER','IP Address','String','','',0,0,'Enter this Computer\'s IP Address.');
CALL osae_sp_object_type_property_add('COMPUTER','Processor','String','','',0,0,'Enter this Computer\'s processor.');
CALL osae_sp_object_type_property_add('COMPUTER','RAM','String','','',0,0,'Enter the amount of this Computer\'s RAM.');

-- GUI Client
CALL osae_sp_object_type_add ('GUI CLIENT','Touch Screen App','','THING',1,1,0,1,'Screen App or Touch Screen app on a Client computer.');
CALL osae_sp_object_type_state_add('GUI CLIENT','ON','Running','This Screens App is Running.');
CALL osae_sp_object_type_state_add('GUI CLIENT','OFF','Stopped','This Screens App is Stopped.');
CALL osae_sp_object_type_event_add('GUI CLIENT','ON','Started','This Screen app Started.');
CALL osae_sp_object_type_event_add('GUI CLIENT','OFF','Stopped','This Screen app Stopped.');
CALL osae_sp_object_type_method_add('GUI CLIENT','SCREEN SET','Screen Set','Screen Name','','','','Force this Screen app to Switch Screens.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Current Screen','String','','',0,0,'The Screen currently being Displayed on this Screen app.
This property is automatically populated by the system.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Default Screen','String','','',0,0,'The First Screen to display when this Screens app starts.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Current User','String','','',0,0,'The Person currently logged into this Screen app.
This property is automatically populated by the system.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Debug','Boolean','','FALSE',0,1,'Add Screen App Debug info to the Log.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Title','String','','OSA Screens',0,0,'The title displayed in the Top bar of the Screen app.');
CALL osae_sp_object_type_property_add('GUI CLIENT','LogOut on Close','Boolean','','FALSE',0,1,'Reset the User when this app closes.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Trust Level','Integer','','50',0,1,'Enter the Trust Level the CM15A plugin will have
to be able to control other objects.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Use Global Screen Settings','Boolean','','',0,0,'');
CALL osae_sp_object_type_property_add('GUI CLIENT','Height','Integer','','0',0,1,'The height of the Screen app window.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Width','Integer','','0',0,1,'The width of the Screen app window.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Show Frame','Boolean','','',0,1,'Show Windows Frame around the Screen with Min/Max/Resizing.');

-- IP Camera
CALL osae_sp_object_type_add ('IP CAMERA','Core Type: Container','','THING',0,1,1,1,'IP Camera device.
This object is also compatible with the IPCam Plugin. ');
CALL osae_sp_object_type_state_add('IP CAMERA','ON','On','This state represents that this IP Camera is On.');
CALL osae_sp_object_type_state_add('IP CAMERA','OFF','Off','This state represents that this IP Camera is Off.');
CALL osae_sp_object_type_event_add('IP CAMERA','ON','On','This event will fire when the IP Camera state is changed to On.');
CALL osae_sp_object_type_event_add('IP CAMERA','OFF','Off','This event will fire when the IP Camera state is changed to Off.');
CALL osae_sp_object_type_method_add('IP CAMERA','SNAPSHOT','SnapShot','','','','','Run the SnapShot URL save in the SnapShot Property.
Make sure you have also filled in the Save Location property.');
CALL osae_sp_object_type_property_add('IP CAMERA','Stream Address','String','','',0,1,'This is the URL that is used to retrieve the MJPEG video from the IP Camera.');
CALL osae_sp_object_type_property_add('IP CAMERA','Height','Integer','','300',0,1,'Enter the Height of the IP Camera Image.');
CALL osae_sp_object_type_property_add('IP CAMERA','Width','Integer','','400',0,1,'Enter the Width of the IP Camera Image.');
CALL osae_sp_object_type_property_add('IP CAMERA','Save Location','String','','',0,0,'Enter the full path to be used to save Snap Shots from this IP Camera.');
CALL osae_sp_object_type_property_add('IP CAMERA','SNAPSHOT','String','','',0,0,'Enter the URL Command used to take a Snap Shot from this IP Camera.');

-- Network Device
CALL osae_sp_object_type_add ('NETWORK DEVICE','Network Device','','THING',0,0,0,1,'Network device.
A Network device can be any desktop, laptop or
LAN enabled device with an IP Address..');
CALL osae_sp_object_type_state_add('NETWORK DEVICE','ON','On-Line','This Network Device is On Line.');
CALL osae_sp_object_type_state_add('NETWORK DEVICE','OFF','Off-Line','This Network Device is Off Line.');
CALL osae_sp_object_type_event_add('NETWORK DEVICE','ON','On-Line','The Network device came On-Line.');
CALL osae_sp_object_type_event_add('NETWORK DEVICE','OFF','Off-Line','The Network device went Off-Line.');
CALL osae_sp_object_type_property_add('NETWORK DEVICE','IP Address','String','','',0,1,'Enter the IP Address for this Network Device.');

-- Property Reader







CALL osae_sp_object_type_add ('PROPERTY READER','Property Reader','','THING',0,0,0,1,'');
CALL osae_sp_object_type_state_add('PROPERTY READER','ON','Here','This state represents that this Property Reader is either
On or In Range.');
CALL osae_sp_object_type_state_add('PROPERTY READER','OFF','Gone','This state represents that this Property Reader is either
Off or NOT in Range.');
CALL osae_sp_object_type_event_add('PROPERTY READER','ON','Arrived','This event will fire when this Property Reader state
is changed to Here or On.');
CALL osae_sp_object_type_event_add('PROPERTY READER','OFF','Left','This event will fire when this Property Reader state
is changed to Gone or Off.');
CALL osae_sp_object_type_method_add('PROPERTY READER','ON','Arriving','','','','','Executing this method will set this Property Reader to Here/On.');
CALL osae_sp_object_type_method_add('PROPERTY READER','OFF','Leaving','','','','','Executing this method will set this Property Reader to Gone/Off.');
CALL osae_sp_object_type_property_add('PROPERTY READER','URL','String','','',0,1,'Enter the URL of this Property Reader.');
CALL osae_sp_object_type_property_add('PROPERTY READER','Search Prefix','String','','',0,0,'Enter any Search Prefix this reader should use.');
CALL osae_sp_object_type_property_add('PROPERTY READER','Search Suffix','String','','',0,0,'Enter any Search Suffix this reader should use.');
CALL osae_sp_object_type_property_add('PROPERTY READER','Search Prefix Offset','Integer','','1',0,0,'Enter any Search Prefix Offset this reader should use.');
CALL osae_sp_object_type_property_add('PROPERTY READER','Object','String','','',0,1,'Select the Object with the property to be read.');
CALL osae_sp_object_type_property_add('PROPERTY READER','Property','String','','',0,1,'Enter the Property name this Reader is to read.');

-- Stock Object
CALL osae_sp_object_type_add ('STOCK','Profile of a Stock','SYSTEM','THING',0,0,0,1,'Stock symbol.
This can be used to track Stock trading prices.');
CALL osae_sp_object_type_state_add('STOCK','ON','Open','This Stock has OPENED for trading.');
CALL osae_sp_object_type_state_add('STOCK','OFF','Closed','This Stock has CLOSED for trading.');
CALL osae_sp_object_type_event_add('STOCK','ON','Opened for Trading','This Stock Opened.');
CALL osae_sp_object_type_event_add('STOCK','OFF','Trading Closed','This Stock Closed.');
CALL osae_sp_object_type_method_add('STOCK','ON','Open for Trading','','','','','Set this Stock\'s state to Open.');
CALL osae_sp_object_type_method_add('STOCK','OFF','Close Trading','','','','','Set this Stock\'s state to Closed.');
CALL osae_sp_object_type_property_add('STOCK','Price','Integer','','0',0,0,'Stock\'s current Price.');
CALL osae_sp_object_type_property_add('STOCK','Previous Close','Integer','','0',0,0,'Stock\'s Previous close value.');
CALL osae_sp_object_type_property_add('STOCK','Open','Integer','','0',0,0,'Stock\'s Open value.');
CALL osae_sp_object_type_property_add('STOCK','Year To Date Return','Integer','','0',0,0,'Stock\'s Year to Date return value.');
CALL osae_sp_object_type_property_add('STOCK','Low Today','Integer','','0',0,0,'Stock\'s Today Low value.');
CALL osae_sp_object_type_property_add('STOCK','52 Week Low','Integer','','0',0,0,'Stock\'s 52 Week Low value.');
CALL osae_sp_object_type_property_add('STOCK','52 Week High','Integer','','0',0,0,'Stock\'s 52 Week High value.');
CALL osae_sp_object_type_property_add('STOCK','High Today','Integer','','0',0,0,'Stock\'s Today High value.');

-- Thermostat Object
CALL osae_sp_object_type_add ('THERMOSTAT','Thermostat','','THING',0,1,0,1,'Thermostat.
This can be a wired or Wifi enabled Thermostat.');
CALL osae_sp_object_type_state_add('THERMOSTAT','HEAT ON','Heat On','This state represents that this thermostat Cool is Off.');
CALL osae_sp_object_type_state_add('THERMOSTAT','OFF','Off','This state represents that this thermostat is currently Off.');
CALL osae_sp_object_type_state_add('THERMOSTAT','COOL ON','Cool On','This state represents that this thermostat Cool is On.');
CALL osae_sp_object_type_event_add('THERMOSTAT','COOL ON','Cool On','This event will fire when this Thermostat state changes to Cool-On.');
CALL osae_sp_object_type_event_add('THERMOSTAT','HEAT ON','Heat On','This event will fire when this Thermostat state changes to Heat-On.');
CALL osae_sp_object_type_event_add('THERMOSTAT','FAN ON','Fan On','This event will fire when this Thermostat Fan changes to on.');
CALL osae_sp_object_type_event_add('THERMOSTAT','FAN OFF','Fan Off','This event will fire when this Thermostat Fan changes to off.');
CALL osae_sp_object_type_event_add('THERMOSTAT','OFF','Off','This event will fire when this Thermostat state changes to Off.');
CALL osae_sp_object_type_event_add('THERMOSTAT','TEMPERATURE','Tempurature','This event will fire when this Thermostat Temperature changes.');
CALL osae_sp_object_type_event_add('THERMOSTAT','HEAT SP CHANGED','Heat Setpoint','This event will fire when this Thermostat Cool Setpoint changes.');
CALL osae_sp_object_type_event_add('THERMOSTAT','COOL SP CHANGED','Cool Setpoint','This event will fire when this Thermostat Cool Setpoint changes.');
CALL osae_sp_object_type_event_add('THERMOSTAT','FAN MODE CHANGED','Fan Mode','This event will fire when this Fan Mode changes.');
CALL osae_sp_object_type_method_add('THERMOSTAT','HEAT','Heat','','','','','This state represents that this thermostat is currently set to Heat.');
CALL osae_sp_object_type_method_add('THERMOSTAT','AUTO','Auto','','','','','Executing this method will set this Thermostat to Auto.');
CALL osae_sp_object_type_method_add('THERMOSTAT','COOL','Cool','','','','','Executing this method will set this Thermostat to Cool.');
CALL osae_sp_object_type_method_add('THERMOSTAT','FAN AUTO','Fan Auto','','','','','Set this Thermostat\'s Fan to Auto.');
CALL osae_sp_object_type_method_add('THERMOSTAT','FAN ON','Fan On','','','','','Set this Thermostat\'s Fan to On.');
CALL osae_sp_object_type_method_add('THERMOSTAT','FAN OFF','Fan Off','','','','','Set this Thermostat\'s Fan to Off.');
CALL osae_sp_object_type_method_add('THERMOSTAT','HEATSP','Heat Setpoint','Setpoint','','','','Set this Thermostat\'s Heat Setpoint to Parameter 1.');
CALL osae_sp_object_type_method_add('THERMOSTAT','COOLSP','Cool Setpoint','Setpoint','','','','Set this Thermostat\'s Cool Setpoint to Parameter 1.');
CALL osae_sp_object_type_method_add('THERMOSTAT','OFF','Off','','','','','Set this Thermostat to Off.');
CALL osae_sp_object_type_property_add('THERMOSTAT','Heat Setpoint','Integer','','0',0,0,'Currently set Heat Setpoint.');
CALL osae_sp_object_type_property_add('THERMOSTAT','Cool Setpoint','Integer','','0',0,0,'Currently set Cool Setpoint.');
CALL osae_sp_object_type_property_add('THERMOSTAT','Current Temperature','Integer','','0',0,0,'Currently Temperature.');
CALL osae_sp_object_type_property_add('THERMOSTAT','Operating Mode','String','','',0,0,'Current Operating Mode.');
CALL osae_sp_object_type_property_option_add('THERMOSTAT','Operating Mode','Heat');
CALL osae_sp_object_type_property_option_add('THERMOSTAT','Operating Mode','Cool');
CALL osae_sp_object_type_property_option_add('THERMOSTAT','Operating Mode','Auto');
CALL osae_sp_object_type_property_option_add('THERMOSTAT','Operating Mode','Off');
CALL osae_sp_object_type_property_add('THERMOSTAT','Fan Mode','String','','',0,0,'Currently set Fan Mode.');
CALL osae_sp_object_type_property_option_add('THERMOSTAT','Fan Mode','Auto');
CALL osae_sp_object_type_property_option_add('THERMOSTAT','Fan Mode','On');

-- Thing Object
CALL osae_sp_object_type_add ('THING','TOP Level Type for all Things','SYSTEM','THING',0,1,1,1,'OSA THING.
This is the Top Level object for ALL Things.');
CALL osae_sp_object_type_state_add('THING','ON','On','This Thing is ON.');
CALL osae_sp_object_type_state_add('THING','OFF','Off','This Thing is OFF.');
CALL osae_sp_object_type_event_add('THING','ON','On','This Thing state is changed to On.');
CALL osae_sp_object_type_event_add('THING','OFF','Off','This Thing state is changed to Off.');
CALL osae_sp_object_type_method_add('THING','ON','On','','','','','Set this Thing\'s state of On.');
CALL osae_sp_object_type_method_add('THING','OFF','Off','','','','','Set this Thing\'s state of Off.');
CALL osae_sp_object_type_property_add('THING','OFF TIMER','Integer','','-1',0,0,'The number of seconds before this Thing is set to Off.  (-1 = Disabled)');
CALL osae_sp_object_type_property_add('THING','URL','String','','',0,0,'the full URL to this THING.');

-- Weather Object
CALL osae_sp_object_type_add ('WEATHER','Weather Data','SYSTEM','THING',0,1,0,1,'Weather object.
This object will contain all the weather information
retrieved from one of the weather plugins.');
CALL osae_sp_object_type_state_add('WEATHER','ON','Current','The current Weather data is Up-To-Date.');
CALL osae_sp_object_type_state_add('WEATHER','OFF','Obsolete','The current Weather data is Obsolete.');
CALL osae_sp_object_type_event_add('WEATHER','ON','Updated','The Weather plugin Updated.');
CALL osae_sp_object_type_event_add('WEATHER','OFF','Hung','The Weather state changes to Hung.');
CALL osae_sp_object_type_event_add('WEATHER','DAY','Day','The Weather status of Day occured.');
CALL osae_sp_object_type_event_add('WEATHER','NIGHT','Night','The Weather status of Night occured.');
CALL osae_sp_object_type_event_add('WEATHER','DAWN','Dawn','The Weather status of Dawn occured.');
CALL osae_sp_object_type_event_add('WEATHER','DUSK','Dusk','The Weather status of Dusk occured.');
CALL osae_sp_object_type_method_add('WEATHER','ON','Updated','','','','','Force the plugin to update.');
CALL osae_sp_object_type_method_add('WEATHER','OFF','Hung','','','','','Set the Weather state to Obsolete.');
CALL osae_sp_object_type_property_add('WEATHER','Night1 Low','Integer','','0',0,0,'Low-Temp for Night 1.');
CALL osae_sp_object_type_property_add('WEATHER','Night2 Low','Integer','','0',0,0,'Low-Temp for Night 2.');
CALL osae_sp_object_type_property_add('WEATHER','Night3 Low','Integer','','0',0,0,'Low-Temp for Night 3.');
CALL osae_sp_object_type_property_add('WEATHER','Night4 Low','Integer','','0',0,0,'Low-Temp for Night 4.');
CALL osae_sp_object_type_property_add('WEATHER','Day1 High','Integer','','0',0,0,'High-Temp for Day 1.');
CALL osae_sp_object_type_property_add('WEATHER','Day2 High','Integer','','0',0,0,'High-Temp for Day 2.');
CALL osae_sp_object_type_property_add('WEATHER','Day3 High','Integer','','0',0,0,'High-Temp for Day 3.');
CALL osae_sp_object_type_property_add('WEATHER','Day4 High','Integer','','0',0,0,'High-Temp for Day 4.');
CALL osae_sp_object_type_property_add('WEATHER','Day1 Precip','Integer','','0',0,0,'Percipitation % for Day 1.');
CALL osae_sp_object_type_property_add('WEATHER','Day2 Precip','Integer','','0',0,0,'Percipitation % for Day 2.');
CALL osae_sp_object_type_property_add('WEATHER','Day3 Precip','Integer','','0',0,0,' Percipitation % for Day 3.');
CALL osae_sp_object_type_property_add('WEATHER','Day4 Precip','Integer','','0',0,0,'Percipitation % for Day 4.');
CALL osae_sp_object_type_property_add('WEATHER','Conditions','String','','',0,0,'Current Conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Wind Speed','Float','','',0,0,'Current Wind Speed.');
CALL osae_sp_object_type_property_add('WEATHER','Wind Directions','String','','',0,0,'Current Wind Direction.');
CALL osae_sp_object_type_property_add('WEATHER','Humidity','Integer','','0',0,0,'Current Humidity.');
CALL osae_sp_object_type_property_add('WEATHER','Pressure','Float','','',0,0,'Current Pressure.');
CALL osae_sp_object_type_property_add('WEATHER','Dewpoint','Float','','',0,0,'Current Dewpoint.');
CALL osae_sp_object_type_property_add('WEATHER','Image','String','','',0,0,'Image for the current conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Visibility','Float','','',0,0,'Current Visability.');
CALL osae_sp_object_type_property_add('WEATHER','Windchill','Integer','','0',0,0,'Current Wind Chill.');
CALL osae_sp_object_type_property_add('WEATHER','Temp','Float','','',0,0,'Current Tempature.');
CALL osae_sp_object_type_property_add('WEATHER','Last Updated','DateTime','','',0,0,'Date and Time of the last Update.');
CALL osae_sp_object_type_property_add('WEATHER','Day5 High','Integer','','0',0,0,'High-Temp for Day 5.');
CALL osae_sp_object_type_property_add('WEATHER','Day6 High','Integer','','0',0,0,'High-Temp for Day 6.');
CALL osae_sp_object_type_property_add('WEATHER','Night5 Low','Integer','','0',0,0,'Low-Temp for Night 5.');
CALL osae_sp_object_type_property_add('WEATHER','Night6 Low','Integer','','0',0,0,'Low-Temp for Night 6.');
CALL osae_sp_object_type_property_add('WEATHER','Day7 High','Integer','','0',0,0,'High-Temp for Day 7.');
CALL osae_sp_object_type_property_add('WEATHER','Day5 Precip','Integer','','0',0,0,'Percipitation % for Day 5.');
CALL osae_sp_object_type_property_add('WEATHER','Day6 Precip','Integer','','0',0,0,'Percipitation % for Day 6.');
CALL osae_sp_object_type_property_add('WEATHER','Day7 Precip','Integer','','0',0,0,'Percipitation % for Day 7.');
CALL osae_sp_object_type_property_add('WEATHER','Night7 Low','Integer','','0',0,0,'Low-Temp for Night 7.');
CALL osae_sp_object_type_property_add('WEATHER','Night1 Precip','Integer','','0',0,0,'Percipitation % for Night 1.');
CALL osae_sp_object_type_property_add('WEATHER','Night2 Precip','Integer','','0',0,0,'Percipitation % for Night 2.');
CALL osae_sp_object_type_property_add('WEATHER','Night3 Precip','Integer','','0',0,0,'Percipitation % for Night 3.');
CALL osae_sp_object_type_property_add('WEATHER','Night4 Precip','Integer','','0',0,0,'Percipitation % for Night 4.');
CALL osae_sp_object_type_property_add('WEATHER','Night5 Precip','Integer','','0',0,0,'Percipitation % for Night 5.');
CALL osae_sp_object_type_property_add('WEATHER','Night6 Precip','Integer','','0',0,0,'Percipitation % for Night 6.');
CALL osae_sp_object_type_property_add('WEATHER','Night7 Precip','Integer','','0',0,0,'Percipitation % for Night 7.');
CALL osae_sp_object_type_property_add('WEATHER','Tonight Precip','Integer','','0',0,0,'Percipitation % for Tonight.');
CALL osae_sp_object_type_property_add('WEATHER','Today Precip','Integer','','0',0,0,'Percipitation % for Today.');
CALL osae_sp_object_type_property_add('WEATHER','Night1 Forecast','String','','',0,0,'Forecast for Night 1.');
CALL osae_sp_object_type_property_add('WEATHER','Night2 Forecast','String','','',0,0,'Forecast for Night 2.');
CALL osae_sp_object_type_property_add('WEATHER','Night3 Forecast','String','','',0,0,'Forecast for Night 3.');
CALL osae_sp_object_type_property_add('WEATHER','Night4 Forecast','String','','',0,0,'Forecast for Night 4.');
CALL osae_sp_object_type_property_add('WEATHER','Night5 Forecast','String','','',0,0,'Forecast for Night 5.');
CALL osae_sp_object_type_property_add('WEATHER','Night6 Forecast','String','','',0,0,'Forecast for Night 6.');
CALL osae_sp_object_type_property_add('WEATHER','Night7 Forecast','String','','',0,0,'Forecast for Night 7.');
CALL osae_sp_object_type_property_add('WEATHER','Tonight Forecast','String','','',0,0,'Forecast for Tonight.');
CALL osae_sp_object_type_property_add('WEATHER','Today Forecast','String','','',0,0,'Forecast for Today.');
CALL osae_sp_object_type_property_add('WEATHER','Day1 Forecast','String','','',0,0,'Forecast for Day 1.');
CALL osae_sp_object_type_property_add('WEATHER','Day2 Forecast','String','','',0,0,'Forecast for Day 2.');
CALL osae_sp_object_type_property_add('WEATHER','Day3 Forecast','String','','',0,0,'Forecast for Day 3.');
CALL osae_sp_object_type_property_add('WEATHER','Day4 Forecast','String','','',0,0,'Forecast for Day 4.');
CALL osae_sp_object_type_property_add('WEATHER','Day5 Forecast','String','','',0,0,'Forecast for Day 5.');
CALL osae_sp_object_type_property_add('WEATHER','Day6 Forecast','String','','',0,0,'Forecast for Day 6.');
CALL osae_sp_object_type_property_add('WEATHER','Day7 Forecast','String','','',0,0,'Forecast for Day 7.');
CALL osae_sp_object_type_property_add('WEATHER','Night1 Image','String','','',0,0,'Image for the Night 1 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Night2 Image','String','','',0,0,'Image for the Night 2 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Night3 Image','String','','',0,0,'Image for the Night 3 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Night4 Image','String','','',0,0,'Image for the Night 4 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Night5 Image','String','','',0,0,'Image for the Night 5 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Night6 Image','String','','',0,0,'Image for the Night 6 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Night7 Image','String','','',0,0,'Image for the Night 7 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Tonight Image','String','','',0,0,'Image for Tonight\'s conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Today Image','String','','',0,0,'Image for Today\'s conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Day1 Image','String','','',0,0,'Image for the Day 1 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Day2 Image','String','','',0,0,'Image for the Day 2 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Day3 Image','String','','',0,0,'Image for the Day 3 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Day4 Image','String','','',0,0,'Image for the Day 4 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Day5 Image','String','','',0,0,'Image for the Day 5 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Day6 Image','String','','',0,0,'Image for the Day 6 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Day7 Image','String','','',0,0,'Image for the Day 7 conditions.');
CALL osae_sp_object_type_property_add('WEATHER','Night1 Summary','String','','',0,0,'Summary for Night 1.');
CALL osae_sp_object_type_property_add('WEATHER','Night2 Summary','String','','',0,0,'Summary for Night 2.');
CALL osae_sp_object_type_property_add('WEATHER','Night3 Summary','String','','',0,0,'Summary for Night 3.');
CALL osae_sp_object_type_property_add('WEATHER','Night4 Summary','String','','',0,0,'Summary for Night 4.');
CALL osae_sp_object_type_property_add('WEATHER','Night5 Summary','String','','',0,0,'Summary for Night 5.');
CALL osae_sp_object_type_property_add('WEATHER','Night6 Summary','String','','',0,0,'Summary for Night 6.');
CALL osae_sp_object_type_property_add('WEATHER','Night7 Summary','String','','',0,0,'Summary for Night 7.');
CALL osae_sp_object_type_property_add('WEATHER','Tonight Summary','String','','',0,0,'Summary for Tonight.');
CALL osae_sp_object_type_property_add('WEATHER','Today Summary','String','','',0,0,'Summary for Today.');
CALL osae_sp_object_type_property_add('WEATHER','Day1 Summary','String','','',0,0,'Summary for Day 1.');
CALL osae_sp_object_type_property_add('WEATHER','Day2 Summary','String','','',0,0,'Summary for Day 2.');
CALL osae_sp_object_type_property_add('WEATHER','Day3 Summary','String','','',0,0,'Summary for Day 3.');
CALL osae_sp_object_type_property_add('WEATHER','Day4 Summary','String','','',0,0,'Summary for Day 4.');
CALL osae_sp_object_type_property_add('WEATHER','Day5 Summary','String','','',0,0,'Summary for Day 5.');
CALL osae_sp_object_type_property_add('WEATHER','Day6 Summary','String','','',0,0,'Summary for Day 6.');
CALL osae_sp_object_type_property_add('WEATHER','Day7 Summary','String','','',0,0,'Summary for Day 7.');
CALL osae_sp_object_type_property_add('WEATHER','Sunrise','DateTime','','',0,0,'Time of today\'s Sunrise.');
CALL osae_sp_object_type_property_add('WEATHER','Sunset','DateTime','','',0,0,'Time of today\'s Sunset.');
CALL osae_sp_object_type_property_add('WEATHER','Tonight Low','Integer','','0',0,0,'Low Tempature of Tonight..');
CALL osae_sp_object_type_property_add('WEATHER','Today High','Integer','','0',0,0,'High Tempature of Today..');
CALL osae_sp_object_type_property_add('WEATHER','Latitude','Float','','',0,0,'Latitude of this system.');
CALL osae_sp_object_type_property_add('WEATHER','Longitude','Float','','',0,0,'Longitude of this system.');
CALL osae_sp_object_type_property_add('WEATHER','DayNight','String','','',0,0,'Current status of Day/Night.');

-- State Button User Control
CALL osae_sp_object_type_add ('USER CONTROL STATEBUTTON','Custom User Control','SYSTEM','USER CONTROL',0,1,0,1,'Custom User Control');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','Control Type','String','','',0,1,'The Proper Control Type this State Button is associated with.');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','Object Name','String','','',0,1,'Enter the Name of the object this State Button is associated to.');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','X','Integer','','0',0,1,'Enter the X position to display this State Button on the');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','Y','Integer','','0',0,1,'Enter the Y position to display this State Button on the');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','ZOrder','Integer','','0',0,1,'Enter the ZOrder position to display this State Button on the');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','Width','Integer','','75',0,1,'Enter the Width of this State Button.');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','Font Name','String','','Arial',0,1,'Enter the Font name for this State Button.');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','Font Size','Integer','','12',0,1,'Enter the Font size of this State Button.');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','Back Color','String','','Transparent',0,1,'The Background Color of the State Button.');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','Fore Color','String','','',0,1,'Select the Fore Color of this State Button.');
CALL osae_sp_object_type_property_add('USER CONTROL STATEBUTTON','Height','Integer','','40',0,1,'Enter the Height of this State Button.');

-- Weather User Control
CALL osae_sp_object_type_add ('USER CONTROL WEATHERCONTROL','Custom User Control','SYSTEM','USER CONTROL',0,1,0,1,'This combines many basic controls into a single customizable Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Font Name','String','','Arial',0,1,'The Font of the main text.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Object Name','String','','',0,1,'The Weather Object\'s Name where all the weather data is stored.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','X','Integer','','0',0,1,'The Left most position of the Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Y','Integer','','0',0,1,'The Top most position of the Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','ZOrder','Integer','','0',0,1,'Enter the ZOrder used to display this Weather Control in the screen application.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Fore Color','String','','',0,1,'The Color of the main text.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Font Size','Integer','','12',0,1,'The Size of the main text.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Control Type','String','','',0,1,'This property will contain the Proper Control Type to associate with.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Back Color','String','','',0,1,'The Background Color of the Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Height','Integer','','0',0,1,'The overall Height of the Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Width','Integer','','0',0,1,'The Width of the Maximized Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Minimized','Boolean','','TRUE',0,1,'Display Only Today\'s Weather.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Max Days','Integer','','5',0,1,'How many Days of Weather to Display.');


INSERT INTO osae_images(image_data, image_name, image_type, image_width, image_height, image_dpi) VALUES
(x'89504E470D0A1A0A0000000D494844520000003A000000390806000000672F3886000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000001974455874536F667477617265007061696E742E6E657420342E302E3137336E9F63000024A7494441546843A57B775455D7F6F5B9FD72EFA583A8604740D1D862378929EFE525312F31F1C534535F34B1F70682A888052B5205C4DE454429A2028AA26247C582A28282D27B677E731F302F637CFFE537EE58EC7BCED967ED3DD79AABECEB504233142D4D5000CDEA9A96F2C5DBE2239A7EF4F901D337CDC4E2C8C5F0D9B914DE915E581AE6039FD0E5F00BF5C1CA306F78877B61F1564F78842FC1E2700F2C09F38057F862CE5B00EFD0F9F00A5D2CDFF3085D028FC8A5F0DCE68D45914BB068EB62786EF58077D82278728ED0B1702BE7709E77A8277CC23CB17CEB22F86C5D28CB523EF70EF58277C8728A6FDBB88CBABD643D2B0FACC0CA43BEF27EBF5FFC1316AFF5C0851B176F353434BC0D404D9184484D0DCD4A7ED135B6D48EF979FA8427DEC14B5AAE965EC3CDAA4C649466E04EED2D3C430E5EA208792DCF915D9985FB6599785CF7104FF979C24F0EC71CF9DB23CECD462EAF725B7290539F8D27CD4FF1B8E909B29B1FCBF298B372F97921CB73BE29CFC6939667C86D7E82BCE61C3C6F79CC27D9C8E3EC9CA647B28E272D2F38371F4F9B3936E7F18D677C2ED67F8487B88FBB7559C82CBB8D98B4188C9D30B6FEE88923571A9AEAA7D43737E965A04083292FFFF1AF1F7EF15EC9A1E4FDC8AE7F84BBB5F7E4171F71B18CC234EC3E1F8535FBFCB1393A001149C1088FDF8290980084C606212836185B4E8460737C2036C66DC2E6844D08E4F3A0A301D8B86F3D020E6E42707420424E0423342E0481C782B0E9F016041C0842E0A110EA0843F0F1563DE25970CC96563916C07B1CE3A83F9E9218264B6002259EF34FF0F9F18D387C750FF79A89EB25D770A7E2366E155EC34B1A62D2FC9F11B63BB0BE098D531B9B1B54520D4AFA78FB2F4CDFCA45F269E3B34FD290969B86A40727F1D9F431F8CFBC31F8CE631C7EF7FB1DB336CFC1CCC0C998BCE1574C59F33B66AC9F81E91B6662DAE65998BC653A266D99823F02A6603265CABAA998B26A3266AC9E8A391B676151C87CD27621E606CDC3CCF5B331D37F3EE66E5C8C99EB6650CF744C5F370DD3FCA761EADAA9984C91DF5F3F0D53026652E70C4CDA3405BF6FE6BD80E998BA790A26F9FF8A5F7C27E0C765DFE2DD9FDEC6D26D4BE96172A0F921E2EFC4E061D56D4C98311E976F67DC6B42737B29FAF2FEE03F564DAC7D882C1CCE3C241362CDDED5183DE16D1CB9B20F67731370A726439008779BB290D1988E6B20A5F9C96CCEC2ADA6BBB8DE448BB6DCC275DCE47883D737F9EC0E353EC03DD24ACCBDC567B7700399FC9BD99229EBBAC74DDDE78C2CDCC5FD963BB8475D771A6F23B3E9366E34DFC58D962CBE41FD7CEB06DFBFC9BF42D35DA1A5F93AAED665E056DD4D9CCD3B87897E93F1F68FA391597F0B29F949A4731622E2C33065D1D4864660A2B430725E75507200CE579EE7066E625DCC3A7C3A750C1E3471830DDC7A7D3A2E56A4E05CD159A496A5E14CDD699CAE3F8DE4EA7348A94A9325B9F22CAF53915223C6733853918AD4EAF3486FCCC0A5E62BB840E39C6BE0BDBA64A4D6A420ADEA2C2E54F179753ABF934194F3D491C6F7CE959F450AC753949394C44A7EAFA6DEAA14A4969F464AF9499CAD3C8594EA644A2AAED45E4152EE19C66A1E16462D81CB3817D91027726270BFF11EBE9FFE034A1A2B32249F23CBA9FC1C2ED55E42625E3C86FF3804A9F96770B5F1AAAC28B12C0E27AB12915A7B1E69CD17910602442A929BCE21B9210DA94DDC58234136B7CA99C6149C6E4896C7D4967338557F86DF0990CFCE36A522AD3E151728E9B5E770A1EE1CCE37516F4B1A2E349F43BA18A99FD0B9068DCAEFA7C57B7C769EEBA435D0E0F5C9B29E946682E798DE7401296529385F912E7BFDEB35E3F1FD86F1FC760537EBAE605938F1DD4B6F90BE5CFE0D524ACF21A33E03F3A2666352C02F48AB4C26C0C456650495C4BFA75A68D9A61424349C447C433C121BE9593E3BD5CCB1E50C4EF193D8928493DC9E90A4E664243572AB32880B48AF4F23B8F3B858770197EA2FE13C3D9E5C9E8253F4D2497A28A93609A71B8596537CFFA4AC2BA1F9A4AC3BB999C66A384526252299EBF20E9F9DC289DA78D928698D6771E2551C297E0B971A2F60F06F03109F7B0C974B2F60FB991D387C3E1AD2F76B7E45EC9344DC68B885CF168FC1DEFB9138F6721F9572D34D54D6128F58501A131153178713042936223EF1CDF1DC0837C60D1D6F3CC1792740FF23B6295EF6E2A972EA789E8003D7F760CF85ED086166DE1217C8CCC9EC7B2A8294A3A1F268D4179C577906F1049B40B3C6F16F02D74C6889A321136539D9CC7B2D09B20178C5E789F27552D3491C2B8D25DC74C415714E7E22717C86EDE72299853399C1439078EB14A4CF967C8DA48254C6E1158C9EFE0EE28A0FD0BA87115376809E8BC5F1A66388693E8668CA91C658C436102C41C5341EC5919A4388AD89A67D4F8B6571B429865013B999D3384E0B6F4D0BC3FA83FE581EEE0DAFC04558B0692E16042DC092A8655814B11C0B429762E5AE55F08F5ECF0D9E465CD9691C2A8BC591EA181CAA3844431E4742E371C4351DE73EB876D3091C23F8632DBC6EA1599B63B96A1CA26B8E22BE2909C72AE29056978EDF36FE06FFA3FEC8245051FA8EDE3C0E69ACD7789C293C8BF4AA0CBC3FEF3D24D61EA2050F22BA742712EA8E20AEE13017388CC32DD1388263F262310D313852B71F876AF6B61AA23E1AC70832BA2E1AF1A4D7F6AC9DD89810882504327FF57C786EF0C0FC8D73302F782E666F9D8319A1B3B060FB12786D5F8EFF7A4E943BAEB9411E0849DB8EB3CC15C7CBE9AFC624EA3D2AEB150614867E2DE23AB6F908625A0E714F8770407C9A0FE340CD1130CD6162F86478EC5B82ABB5D7B1EE6400F6DE3E0C69BCEF78C43D8F67264BC3E8C5A371A472278ED6ED4074D52EC4D61EA0070FE068E33E1C6CDE8F834271D3511CAE3F82E8A60388C1411CACDB47C0FB71ACE1A84CA35D0F77C1738F177EF59984DF7DA6C237723542E2C2B0EFE67E1C7F41231570D32FA27120E72002CE04C09F1E9FB57636262CFA85EDE0526C39BD9559FC3C12AA13192EC77098463E40C31F6C3982834DD138DC48693A88232DFB7118FBF8D9CDCF4EECC57EECACDA4BE29FC6F7C113E0757409CE55A4C12F6E0DF6DE3B08E92BDF7FE3787E34628B63D16F763FECAA88C4BEC6EDD853BD0307EAF7605FC36EEC69D8813D4D1C1BF7606FFD5EECABDDC305F732260F22AA722BF6D7ED464CE52124952560E99E25F8EFF2FFE2BB4513B0FAD07A049D0BC7917C52AD26811E8F912977B4EE100E96EE4174D17E24142762E3C9CD72AF3BDD6F167E5EFC5FECBEBC0FB14527648688357736EFC6DEA6BDD8DB4060B2701FCDBBB0BB857B24CC5D75513840C3EF2ADB2D47EF77C1E3E111B300C9A5A7E177C20F31D93190BE58FB2183F900A2ABA3D17BBE3BB671E342C18E865DD8DE486988A2446257C336CA76ECA8A3D444C9CAF735EF40444D08F6D4EDC4B192A3084C09C0CF5E3FE1FBF9DFC173C7121CA3074566DCCF70D841E344D178BBEB77C91B1586A2BFE4584FAA3B85C03341F863F9EF98B4782266F84F97B3E8FEF27DD85ECF359AC31125F6C1EFBBB8D64E8E518D919470EC6BD9891DD511740AE79644316E63F075F8584C3B3411E798F5FDE27D91901B07E9FDF52311FE2A08DBAB76C06D691F6E3C1C3B9AA8A8650F42EBA31056BF1591F561DC28EF53226BB722AC2E0CA175E1086DD88AE0FA2079F1632F8FE1BF7E13316EEA5798BE7606B6DD0CC79E925D08AF0D412438AF36089B8B3623B29C06AB26E8CAEDD85A198EE0C650041607E1E8ABA3F0DEEE855F16FE8CAF677D09FF243F26A6BDF27AA18D41086B08457803D7A781236A23B837F6C9F5C1D8D5BC8D060CC3F6EA28841405633FBDFF71C4FBF8F1F0781CAB398839076621FAE951486EABBB63E18339F07AE68D769EED11DC1C8A304420A8711B02082690CA42EA82B0B58652BB45561ED4C0269BB2B98ECD7D33AF0BD87C5F0CC598C99F63EC1F63B17CAF0F8E141DC6B6EA486C69DC0CB6F708453076603BB6D7D278C534164364435900021184E505CBB0A7621F7630897D337F3CBE9CFE19BE5F351E075FEEC1B62ABE59BF85FBE021806B07D7D16875A108E05E36D70450E736D998819581585FB24EBE1EB16D303E8FFB082B9FFAE0DF41631094150CC976AD25C65EFA1463D23F81E429C1BB792956611D36D1D2EBF9F2262EB0A969333634FA53D622A07903021A366043FD3A04366DC2BA125F399667462EC087BF7D8A4FA67C806DD742B0B7E400B6D5EDC1AAB2755851BE127E15ABB0B1662382AB699C4A0ABD12D1B21DAB8AFD10D8B0055B2A83B1B37C0F6645CEC1DB3FBD834FE67D8A432F48F9F228990D018D215C93666989C006825C5FBF09FED5FEE44A0436D5F27BDD5AACA85A81CDFC0C3C3C00434E0FC2579963E1BECE0DFECF5613689825469D1F8ED197DE86E425E1BD87EFE3DBB20958D4E28D95580B9FBA15F0A8E0E1B7D6137ECDCBB1A6CE0F6B6B57614D8D1F7C4B97635DF15A6C2FDE89F1BE3F63E484F730D6E323ECCF8B4278F176AC2DDCF22750DFB295585DE907FF0A5FAC29F5C5AA727FACADDA00DF57CBB0BACC0FABB8E980B26078C62CC25BBF0CC77B53DEC5FA8BEBB1B538141BCB3712E416F8D5ACC3BABA8D5855B91ABED5BEF02CF1A45356630D3F3E588A49C513F143DECF308699A37DAC03DEBF3B1AEEE16E5857BA069255A4096F5D1985776FBC07C94F42E7BB9DE196EF8651AF46E2DBBA6F30A3791A552C812FFF7A577BC0BBCC0BBEDCECB22A5F7815AFC49AA240028DC6A79EDFA3FFF8617877D1286CCD63D928D884D5DCA07FCD1A8A1FD6551124C7B5F5F46EC3722C6B5A8E158DBEF0AD5C8655D4B5BA710D3696AD87C7B11978F39B9E1839B13F569DF3C63626BB95652B6460C2637E952BE05DEC89954DBCC7CF4C7EBEABFF161FBDFC10A3B247E09D07EF428A50C2ECA81E6F658D846B444FAC2D235075B08451D787E3ADEBA3216D90E0F8A223AC0BAD60CA36C1F9450F0C7F3E145F55FD87EAA611EE625ACE07F32B166256E96C78562F83CFABB5087DC504B0F04BF419D70F2317BF8980FC0DF02B5E03BF5A32A2C20B2B2A3D681C6FACA0F8D47AC3BB6109963479C0B3DE031E65345E393BA74A2F6CA097E7474FC680AFBB61D0AFBDB0EAF2527A730D3C4AE66171F542CCAF9E0BCFC685F004BB2CCCC7F70DDFE11F35EFC3FDB91BBA3D7082CB03BE973318D24E0952B484110F46A067B8331944EA6AC2240CBAD01F43D386415A2FC121DF1E165546982ACD612AB28465AE3D6CB2DAA147B60BDE2BF90013EA7E20641EA8F1072657FE86F9E573B1FEE56A7C30E72DB87EDA196FCEEF8335CF57C2AB6419169479C2A366116501BCAA3C65309E354BE059C7FB758BB1A87A1116D1105E55CBE059E98D75E5EBF1FBAEFFC2F1A3F6E8F465172CBEE08975B5AB097026E6B44CC36F4D3FE39BFAF1F8A8F6630CAF7D0B8ECF3BC1EEB9152C9EEA619F67818E39B670CF7587B49F4063250CBD3F0C3DB73146193E92C5763D86A60FC4B0B3C365A0B6B9B6305499C1BCD112EA0A33A80A8D30165A13B01DACEED9A2DBFD6E78EBD5307CD9F4097E681987DFEA7E2485E761C4ACFEE8F4A13D7ACF7683CF331F2C285A8C3934C2ACAA99B2CCAD988779E5948AB9985FC97690A3783EEDD56CCC2DA787E83111C7BFECFE09361FDAC2E1AB2E5874D51B4B1BBDF153E5F7185FF7393EA864CC3DEF0DAB07DC4B211D52660563891E9A7C09C602354C8FB470297085745482FA941643EF098FBA614DC97A48E6117A8CB8341C23CF8F82B456825D8E1DF4857A285FAAA02AD2404FEF9AD758C0A2DC42A6B4C34B3B38E4DAA0539E03FA97B96344D9488CCFFB16ED2638403FCA84AE337BC13B6F1566BD988359653330B96232FEA89C8C29E5D33183D7B3CBA6604E29A56C366655CCC1EF255330B56A3AE7FC8E794573317ED757D07FA087E29F3A8C8D198F318F3F43BF276FC0F58533DA3F6D07DBA776B07C6E0DDB6A7B98551860D3640963A50EC6621DCC9F19D0E3850BA4031214096A8CB8FF365CB6F6827FF146488650330C393F14C32F8E94635400B528B584F6A5165675565097AB20BDE48BB49A79A9196CCACD61470FDBD3C3F6A4B4F36D577C7095A5E96DD2A5332DF9A50D265D9A8D9FAFFF88E92F276362D124FC56FC07FE289A8A6985D330EBD524CC7CF93B6616CEC0ACE23998983F9146988C1F0A26E0E77B3F6198EF5048BDA9CB9D3116FA0E86678C42E79B9DD1F1717BD8E5D9A05D517B5815DBC0A2D28A8E5093755AE8AAF430961B6159608D1EC2A387252813751896350A3DC398758B08D4B4D5425636E40AA91B24A1634107589699A07CCEC905DC78B104039599971860596C8279911E16B9E6707EE22A53E35F373EC684D49F611A6D05C94982E6134BFC7AF20F7C953016DF5CFC02FFBEFC053EBD3C0E63D3FF837117C6E13FE9FFE6F829BE3CCF7BE7BFC298D431F89273C6667C816FCE7E8701F30741722550370963B78DC377777EC0909B433120AF3F1C9F7684D58B76503DD1417AA222581BE82A8D9004603AC6906B81AEAC182211294E6A30346B383DEA868D25046A0C336168C6700CBE360452203D9A6B0D1379AFA21775A51A18ABF550172909D404DB722B98BD50A17B615738C57786348D1BFA460169104782941C287D689C7735301FA785F429AFBF6893CF289F53C652BE6C1B5F8B78FE6FCA3F29BD286486D45E827E8459EBF35F296C665CB27AA26BB93363D316EA3C3DF76941FAFE152843A780D49581AA31E4DE500275C18652C6A82E4C81372FF5C3900C5A324082FD53961652545DAC81AA5C07439335549506989531462B6C6078AE41AFC7DDA09D4B4A8B4D75A10880161443DB6843B1A774A4B4A370D3F228E6098374A2746893EE6DF7C45C071ACD8C9B3452B786D7468A2D853496DE97D02BD1194E795DA0CC275D5F9AC150AC8759B948982AE85E6960CA3510680F391949494A0CBAF7267A443863BDC8BABA4809032FBA61F0C50132D076CFAC997848DD42352452575169064DB509864A0B58955BC29EF4ED73A73BA47F5199D82037A3D07163669C6FA680DA4C09B58E5E55F19944D1A8A0D02AA1D5F019EF4BA6D677242DDF137384F099464B16A934D0A8F4D0AA75D02A494D034128F9DC9AF2A604B7DD3DE09CEF2A1BDD5469090DF76828D341FD4A01C34B35CC73CDD0BDA05B1B50E2BA3F083D22BBC3BF7C2D8146F1C62517B9C4481B09F4A9BD4C09295F032997712ABC5BA6679C1A64657685060CCEEED30AD48E802C4851253D6150CADE903DC16BA3C90A5A330B680CE6D0188D3033D3416FD04061A186C29C1ED02B61D092354A02D3729EDA0485A4E1777A4BAB265009667A35DFA77E01F40D02DDD51B1DB2BB32AC0C50D08382758672BEF38A79A44001CB673AF4C8275052573A495C5903658FCA7554B74DC29B97DD313C9D1D05B3AEFD131BD6262334AFCCA02F358782CAD4B49A450DBD4AA0362FF418F2E00D481F5199A50045B01A260735BF0B8F2924A8D4DC9C4A0B8546CFFB3A28945A28257A564183A884700EDFD3D2205A95899E3793412A953402DF9775F0B94E4763AB395F84427FEEF3E050747DE40AE50B7A9A0ED0B0ACE82BB8BF421A854085477BE493BA4738FF2443F2EE20D65117AC9763349C417B691046A48F682D2FB9AC4BA56A18995D6D6AED20D15A8A325AABDE04FB5A6B742CB4C5C04C761F8C19B10195056B1E37A9D3B2B1309AD3734628D5ACC1F486A0A6F090999A6C501961509A43A736128019B47ABE63C6677A7A5C4DD690091AD259C9D1CCA885858505F4D425D8207B9471DA3FF20DB83DEAC38ECD06A63A4B48E50C936A1AB59861F14A0D3302EDC67A2B808A3A3AE4EE30B8B1BCC859571FCE0EE2D2100CBFC0F2223CCA564A5FAA82A6803152CD7A2A14D1624ADEB3AFB181036BD5C0CCBE903EE6E222C9E8F94CA58346D24145C02A8EC2332A6E5AC3672AC6A94A52CAF7241A40C4A3EC7D21F49A52299E91F6FC2EDE51D23802B44E43E390FA7A93792B73585B0745F48373960B2C0A6D6496499504D8065445A0FA5C2381F6FC13E8D03BAD40378B864117A9C2A08BFDF1CE65360C6C01ED599475A54C06A5A41DD3B659AD914D0393448996AD16B3F05303DEC8247545B91019919956C964A451924A4ACEA5E764500284D8B492F41374145E1199B82B456458C6B3485082A24A8A00AEA468F85D4FE398D1605A7A5A368C58A72F632EAA1F7A66BB41F39CD465D72695B3AC549239257C8F894978B4BBF0A868181254189E350CBDC25DE9515257CDAC2B80BE7B992D605BAF2B402985C54A18E88C0355199303818BB835B1897EE34E3F489F7071514A841718474A05DF51B4C6A300A6E0860515351AC69E153DE928A1DF6C777C75F02B7C1CC44E4A94166660850028322F4799EA7CD78C400D04AA13BA98B145D21340FB45F541CFC76ED0161808946B916502A8B2880CA0478DCFFE7FA0AE613DB0AE90C7345584A215E895B764EADA3CB3915B2B15BD2A957024C8D74045EDB2CEB745BFBBFD217DC8C5CD29DC98461249846044F66DA3A4F094C89A2A03EFD39356DF1A318E5DD1F0CCA118C52E6CD87A26BF36CFCA1E1560390AA0C2A33A02D530A129A94366021B1101D425A717F741E6B0864A254AA82A5432502D7B73D353027D4EEA1EFC1F50975066DD5704AA0CA747D30710E83BB247AD9F5AB313A295E85545314711AF4C4E5A36CD6A66DC3F81FE838B337684E7B4042A09A022A3CAE545DCE38982DE34DA18E4A640FBAD1A9FDFFE144EB71CE172CB196FEFE5F9D79973451D6E03294443A0E25D2DD9A1664D559991A26D40FB46B9B3F5ECC57262DE0A54C426812A9843B4054A189FE8D1A30DA8225EF927D07585ACA3EAADEC20781E1D9DF136A475F4E89356A0EA625A931D8700294B9116AA5C2DAC5FD8A0FF1D3617EF71711177021837262904588A00CB6BE1157382B7B16532115D110BFE3BB11FC035EB0DF4BF3B04DA1F795FD05774422AD253D09DA2E67B02AC4A8402939CECD136EAF6DDD617DDE951CD4B934C5DA988D99D6C139541F34200E5E9450015A79778E1D111A46E4FAC2FF26F037A7E20DE11BF19F9B7021581AD2E64717FC5F45DC23860AFAB25500D815AF18834F03681BECBC55F03E5C6649022F1685B296CC60C6CA1606117B5B33D81B0E0775EDE03FD32DFC4A8BBF4E628BE23EA236BAF002AE6292566510255529F1CF3AF81B62523F7A8BEE8C61855179833CBB2AC717FC209A283D311A8798E013DF35A812A4F088F8EF88B47C354189836E04FA0B68F6DC8777AF0151721EF35C58C015A4C47EA6AF374B0C9FB0B502B4A1B55FFA42D4B8A8837AD648449C53AAC2275ED4933D2D76941478CBE33121FDFF900D23B9CD7D649892C2D1A0AF56BA01CE506420015EDA53008EB68EF6D7D64A09A17F468019F91AE1A3634D20BAE97A72250139C9FB1A9DFD70A74E45D7A34C4191B5E11A82A5449A0830894562650BB472C2F056CC3085445A05AA6717116D517F25E2E8132590DCAE401E035506656997AAF1311AF05508564805E6BCB7B042A9A74F6C52E93BBE197BBDFE0E3F8F75ACF9C02A8C8BA042A129A56D45BC10E827D0D542D1822D6E17CF70802CD668C3EB7805674473C4989FD8923A54E007D6444CF36A0AAE36A19A85BA86B2B5075881AFD52FBE3AD747A742DA9FBD01A66F93C1550342F4861A66D25DB2BF1AB83006A916D8E8137E8D1D15C5C147299BA348644AA71732239C9195480A7A80DECB09CECA0B7637CF7B740DFA92E70FCDABE958E7212A2F718CB6A821440356AD140F03E810BFAEAF52C31A28CD1A37D2348DD7B2E3C41B17B237D75CF8543D86115A8A17BCA3A9AA5875B1EBBB6BD345EAC0A23EFF03C1ADC139B8A36D001211A0C48E98F21293CD90B8F3EB085A9C004CD53D6D03CC659B9857C1C3214B211C8E6666EAAF0C6157646A205141B105E14E5A5AD3352D01B72CCCA6069591A42645539B3BE3E76092AD29B3A9DA8BB34267B631D8DF5578F8A8C6B3259B0F9A747854109D43DB4173ADFE80ECD43B68C8F581572C8B4971AE8F37948C8E121E18E0EBD727B43DACDF78F6930FCD65F80EA4318A3043A20915E120D439635CCF34D503F6601CE679FF99416CF63F34DAA984819FB2776187493279DB6ACAB644CEAD5EC9CD819A955147A44A315C72EBEC3AE462F8088D8138D3E7B5B99AE22D35244272468DF0A9434FC0B500D0F0AE6E63C8A518F4CDDB65EB7DFA3FE7078D91E962FD89E3EE3619BB12AE2534327E83375707DF23FA0C36EB2050CEA89CD8504AA09641D4D1D808127B9799617DB3B3630CF33429FC39AF9CA16CA670C7682553FD5CACA05D00157794C137554A47D7A4CA3101BA485D9C98846E1B588326164263669D9C9F0AC29719EA46759D0315312A446C782AF111E25F55442C818F95E1B7836FFA2C197D7618CF60AEA05F72C77D83F77905B51C52366771EC2B5B97C8F40CD6EE9E0C264257ED7154087DE180AD7407AF4155B402D810E618C0E3ECD4E459497DBB630E4F088966DE4B1A7351654CFF550E6305964EBA1BFA5469F8BAEAD2D2069A8302A5B4F23BAD65389811B35B06408CA896396D8BCF0B85ECF18D71BE8219E482862BE42A365B2E1318BA2E51C2DCFA11A03BB299E5565F0A4B61C1A82EEF4A800EA7AD305D6393630CB314245D68938D53CE55AD98CD15B7A38673319F18CAD89D160043DEAB2C5191B5F8AF3689084C129FD30E4CC10486B5A81EA0848779F25E10E4B4B0E2D2B28FC908B3F601D7D60827B3A957DC0C585A5C519544E3CE25846DA0A4F525AE392D71AC69888DBB69815F12C0EDB6A0D1B7691C44469E1A9E6CFEE4800E3284E34C2384A33EA168DC560B680DB06A0EF9DDEB0CBB195814A0FE8B9674C648F15303C22736EF13C7A9FBD2EFB7719E80DC6E86BA006021D94ECFE3F8FDEB486F61E3D91C58D5CA72252445F6001295B03B3C706D8645BA0D7792A133F64890D880423C08A5812A7116E5488C4F64F120772515FCD288656111B970F0104A2D5D298348682F4954F2922830B2168954A45DAB23489F7C46F4D0324B806F7428FCBDD61FDC08A6CB3967F0D54E5B03B7A4416B501ED96C5837704F77D548D61D786B601658C1AB630199D7E036F9E616D144D7D06CF809904759F16BB4DC04F2DA0C8661C30666DD8E76AAFABE17A95CA267171F1B3A4F8714BFC402644FC7A277EF87A2DE2BA5BDB28441CD18488EFA2A117C2538D3CF7F58F6642CFEB1FCBC477A1BF1FE52309DD77F784E325275294C6679913FB523EA2471F3256B3553032467B64926D6102A856FE65D3751393D10B711EF537B1B40C87FB191EBD3649E870C10636374C30DD27C0BB7A68396A1F305ED95EE9EE9186D71470CD7185FD3E7A7E39B3EA729E517D189B3E46598CFC6E5C2ABE9BB749EBFDD639ADF27A8EF95266F2A51A98FB6861B1540B931733BD173DE3CDC4B7CC08AB9516507A93256BC98E95A4EEDD01E8FCA0077437B94EB63594F758EF9F9841718F99FF8902E6B775E8799940C952E98052FE3DB8F70657843EDB4403077647CF3DAEAC39A3A0DC459A90BECED7BB4077998923D3049B1CA6722AB57C6284D9032E784309EB3BB670BADA116E377AC0ED9ACBFF419C29DD65E975BD27DCAF097145EFAB2EE87DC54D9ED3FD4A3774CFEC810E371DD1F19E13ACB8B6C5433B5833FB6B195EEADB6A766B56B07E6482C36D7BB48F7384B48AEDE6A56E18727D28DE58DD0B518F8221FD2B6A1886EFEF0B97345774BF444A322129B728D0FBE140D8DDED04B3EB8C930C5A88F12A62D7E2B1A0366BDC750B982E19614DAA5B5F31FF5B6249315EA5D7AF32BB5FB186C5151B5866D8C1EA12C1A45BC3986601ABCB644E9A9EEBB1A66630C3DE600263D322DD601C33C41C1EB587FEBC199CAE7766FCB2A15F2EC1B4DF063D6FF4C298C79FC261B20D92CB1220799C9A7AAFFD3C63E31BE7FBA0EFED7E708EE5E4692C0F07ACD1F5BA1BBADDEE06A7CC0EB0BD6AD30AEC86154C1926B4BBE100BB2BED6073D11A36972CFF96585FB286D5B57630BF6A0F8B0C7B58519FED950EB0CBE07A171DA8DB0E76E9F45E3A0D906E098B8BADC6B1BE61410319607BC306FA2403DAA712640A4B1EFB00F1AF0DAE17DCD1E54C0F0C8E1D8C4FD6FDB3BE0039A9D27DDCFFD065864BF6BBE9EFC1E97417B44FE88A2E49BD20ADE04B4C4EFA703D1CA33BA07F8A3BFAA6F442D7A46EB03BE68076C73BA05342177411F3FFA6744EEC8A4E495DE124E46417593A27764157DEEF4EE946FDCE89DDE096EC8CAEF11DD125DE014E71EDD0F55447744A74E0D809832E0D8179144F499EDCEF46097DAEF585EBF9DEF8F0EAC7D04F32C3F1C747336B50E62A15A1DC2C302D68957AA2A2F1C3ABFF449F0B6FA2476A1FB824F686C5564B283692265E54328F3297E241F1A1785396514895FF93085D42CF5F45DC5BDA26629D256DF7C4335F8A7082B8E62144BEA61EEB281BF9B8D93399B199FA263ED8F78FE629FBA6BE2A44C1AC86E62ABDD4826645194A2CBFF5FB7CAFF54FBA86912746F1203E04DD937A30B03BA07D6227D8C6B787638A23BA9DA3C553BAA0C31927744CEE08C7B31DD121A5FDDF968E14A7331DD139D9F14FE994D2014EC9BC9FDC8EDF1DE098EAC079FC7E8E944EB285FD293BB4E37DBB33F6B20EEBE3E6E8778D4C4B70C0D08B7DF18FCB23E11EE8820F7DFEF9A40005EF37A14EFCE708B62C2DE2BF8240AA4191C5C68495BF9B7DA74BB55B6957DBE34057B89C7486F31937F2DD198EC95DD18ECA3A9E7094E9DBF39C0B3A2674806392E3DF16A7938EE84243764BE8248F9D139CD059DC4B72222D39D2A042BA267742C79334C819D29D22E24FD0BD7D4247B91DED99C83DEDB68373A8035C5639625CC8274FF3F1FCED4634C8D864A0B5808CB8156CA526AB31EBBDC9FBFEB8333870009C36B447BBCD1D6117E40487C8CEE8B4A33B9CB7F740A71047D86F7240A7D02E700CED8C0E7F531C43189B944EC1ADA35348678A13750AA1316469BD96D70AEEC491C931B8BB2C2E3B7AA37B4437B4F7B541FF152EF84FC0BFB0EB66585529F2BFA943F59F208146F5FF036BC66A00BC378B810000000049454E44AE426082', 'Lock - Green', 'png', 58, 57, 96),
(x'89504E470D0A1A0A0000000D49484452000000390000003908060000008C188385000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000001974455874536F667477617265007061696E742E6E657420342E302E3137336E9F63000023E5494441546843857907545467B7F6508729CC30140534D84D6C51634B2CB1F78EBD2B0A08762CD82B16C096448D8A0545B17741458D51636C518CDD58415004A4080CDDE77FF601F225DFFDEFBAACB5D77BE6BC6D3FBBEF830A800A9F0BAD50646EFA22F6FAF4CD73039EAD1836009B7DC7E2F0AC19D837753A768EF5C601D291317C3762348E78F962DFE8C9D83766220E8D1C874363BCB0D7DB1BBB7D7CB07FEC7885F68EF5C52E6F5F6CF7F5C50E1F5FECF5F2C33EEEDBC575BB7CBCB86624CF1C86489FE1A411D83FCE1BFBC77863DFD871249E376E34C90B11A37D1139D65F993FC8DF27F9FEC858991B8F7DFED3B0DF770A22C74FC0861183B1D667787674F8C6E3C5D91F1B0185960A36920A0505D6282AE87123EAD8B9D16D5B3E3C1F1A9C977AFA14524F9CC2C7A868649E3F8FAC736751101D85A2D32554187D0E79672E2954147D96148DDC18BE3B1F83A298F3F87CEE0210731145172E22FF22D75DF80585517C3E7D0E9FB836EB4C34CF3B85A23327911B7D1C66197957FEE933C88B3E83DC3351C83B1B0DF3B968E49C39CBDF6751483E8407F3B143BCEF348A7EB9849C73E791793C1AF1BBF62233FA24FEDABFEB73D09861E62943073D84D9DC15C5B052C91F8A0B071DD9BA296946E776BCF034B2230E2077CF51248445223BFA3C8ECF0BC491D95371789A1F8E054CC2F119D37170EA141C983289E3641C9A368573937068AA3F0E4FF1C7D1497E3839D11F51132622CA7F324E904E4D0CC0D949337066EA2C9C9C11C83366E06440004E4C9BC6713A8E934E4C0BC0F1A953718467CB79FBA74DC4BE005ACA8C49D83BD91707A74C40D4EC409C9B3307C778B7DC173577161E6CDC8C8CA327917DF808B28F1EC5FB4347706CE1524CE9E199698E7B178882625B55F2E3473F7BB56D999377E61432C3B6226BC336E0E8051C18E40DBFAAB511DCA333D6F5EB8AED43FA60EB803ED8D8AF3FB60C1E88B0C19ED832B0177E1EDC0F9B070D50E6B6F6EB896DA49D7D7B6277AF5ED8DDB30F767B0E4078DFFE08EFD30F3B7AF7C7668E3FF7E51E8E617D78565FBA06694B3F9ED1BF3FC2060CC2D6817C37886B867862E3C09EF879502F6CECCBBBFAF6C5CE21C3B86600D6F5E88035DD3A20A84D5B04B56C8D576BD7E1C396AD7814B24EB1B0A8C5ABB0D2CB2F01E6BCA9AAEDD3677C383A6766F19B9FD6C0BCFD67E46C0BC79CAA75B0B5D300BC59B719F907F6232D6C230AB787E1F38E9DC8DFBE1B45E11128DABE05C5E161C80BE7BBF09277C5BB76E373C46E206217B09BC4B12862270A76EFE09A6DC8DDBE0DE66D1CC3B6A160EB361486ED8479EB76E54EF3F65DC8DDB11BF93BF6A260470489E7EEDC81BC9D612852CEDA87FCADE1C8DCB895EBC291B7771792C33629E0AE07CEC6927A75706AD4088026FF68F54F78BE792796F4F02C8ABB7AED836A49FB0E48D91B81770459B02B0C935DCBE3D8E0914859BB051F823700643C734D2872434351B09A8258BD9EB41639AB96237F75307242D7203B94EF427F40EE1AD2BAF5C85BBB16056B57C3BC2E0469EB5721ED8710A4AF5F8DCC75426B91BD86FB655FC86A64F34CF9FD69ED7A7CE2FE6C21392F783572828391BF668D32E6AC588DBCE075DCB78E6BD623E7073E6FFA11591B372273D346A4FCB816B3AB79E0F0F06148D8BC15A9E17B10337B2EC267CC846AB7673F24FDBC097911E1D8D8A605F6F4EE81CFBB2390B17C0D8AD76E44FCCCD9CA4558BF0E85BC2C3778150AD684F0F76A1485AE048239C7CB0B43D6223F94E082298C55AB90BF2A88825886EC1F424B008684226D6508925705238567A4072FE73BCEAF5C4A40413087AC8499E79AD7AE5184541C22E786022121285AB102E9F3E6237DC10214AD0E45F11A0A67E952642E5A84F4258B29A86064FC1882BF56CC4760EDEA88DBF403E2A89853E3BD716979105497BD7D10B772256E4E9B8AAD1DDA2399605ECD9F87240688C2C5CB78F842025A8F2C1E98BB64113E0BF38BE6213768090A972DC5E7C541F8BC6805F2962C8779E932E42C594A5A821CCEA5AF588684E015880B0D413C35FD76F58F784D4DBFA146E3D7AC4052E832A42C5F4C812E4276D06298572C2568025E118482A541285EB20C9F79278296123005BA828221D04F0B17A298E763D93260D54AE4F39E94A079C8D8108CD0168DB1AD4747BAC506BCA000F77A8F852A6AD020A4F0E23B33666365BDFA94F44AA42E9A8FBCF973514430C270CA9CD9489B3513E63981C89F3D1D79810130CF9E439A8782C0F9C89ECEF70B172B42C85A300F59DCF391024A085A85A76B7EC4CDD53FE06AE8065CA4A99FA3995E5CBD0A375607E1CF554BF1342808CF7957FA5232BF6016B2E706227BF66C9E3D9F77CD47E1DC9928E47DE07B2C5A00F3626A8E54B498C2279F58B410857C9FB16036B24282F060E6746C6FD70A492B16E0C9829908EDDC01AA189A6BCE0F3F2166D8186CF9AE153ECC5F444D2DE0E133903B6B2AB2E6CF42064375CEEC59282250CC98064C0F402185923F630E0AC49C03099802907D1F16CFC7B3B9B3719742B83E7F09CE2E5A8E430B9761EFFCC5D8BB700922C8DC01023AB9643EA216CEC3C5454B7163C112BC5A4EC1042D442E99CE9F3397E70ACD46C19CE91C2703015379EF0C45B8D973792FF9299C49F054403E81E5F2CEF479F390B8701182BFAC8E94657311B76C0E423AB483EA4AA75EC80F5A8DE8EEFDB0BB592BA4537A02A860EE34A44D1E8BF4E913903B3B00C53309702A0F9DC2CBA612E8B440144D9B857C02CE0F9C892CE6BC349AF9539ADE194A7FFFEC1988983707078396613F811DA355445100D18BE6729C8B934B17E2F0B225D8B768250E2F598568AEB9BB6C21DE533B598181C865DECC9D320539B3A6207F16414E9A024C988A828059C823D8FC802928624E159E3E07F0FEC9D3901C3003EF69412BAB5543CAC240BADD7404B725C85F5B7544CE1C4ABC7B1F6C6FD00499D4D0A74993911B300169E34721C36F2C0AA64EC2671E58E4CF71E2245E38199F274C43F1E4A9C89B3C01394CDEC97C7E392B109717CCC5BEC0E9D835932003673031CFC1B985B37063D12CC40A71EEC6CC69B844419E9D3B0F47E72DC581F9CBB095CC1E66A1719BFB12A999AC091390E933166953C6C33C9360A650C0FE537867000A2693978913018E98407E3897E6E38F14CEBD099C87906AD5F16EFA443C9DE48BF56D69AEBFB46A8BBCC54B71AC6B676C6E501F1F274E458EDF4498C78F43B6F728E48E1D8362DFF1C827E5FAF821CFDF0FB97EE391C77AB4D04F9EBDB9C71771D367E1F729331139610AC279C6490AE12AA5FB27AB96BF027CF17EA63FD26815E9137C91C2BD09E3FDF19CD5D09D89337171EA6C6CF59E84ADBE13718202BC3F9D56E13F1E995E23914C21A74FF58399A0CCBEA5778F1F8F02D6C5203F60DD9AC7BADA4C05BCE7794F087431D360E2C4B178ED3F16DBDA7786EA6A8B568C904B70B0431BFC5CBF2EB518809CB13E04E50BF318265716DF9F47519B24330BE8EC7104EFE505F3E851C8F31A836C16DA490CD58F66CEC149BF006C1CE1833DDE93F1BBDF34C453C2097E5E48F219828FE3862B6B0B468E41FE8831C81A350AC9A34722DE671CEE4D9C8CC33E93B1698C1FB67879E31CB59838C11FE6093E48E3FE8FFE5E4867D19E397A2C72D92414088DF642FEA83128222F39A346A3808AF930CE170FD91884567043FA442FBCF36521DFA61354BFD5AE8B4C5F76186D5A6163AD2F91C5CA3E7328010818265630207D1E340A858378D0B0D1300F1F899C6143903F6CB032A68F1E8E573EDEB848CDED1C3D019B86FAB05B998067E327227DB21F52FC8623C96B2052478EC0A761646C8837F238668C1D4AA6FA2279BC275EFA0DC56FFEFE081FE58DE0A1A31146E663A9A98C89E391E13B0219638628BCE493AFE2C1A3F0790879193E42B93F7BF820E48C1E8A3CEE49A7009F0C19816D95BE40FA984178D1B71B22196754E74D8E88EBDC19C79B36C2D6EA5591C9D62967E030640E20884183513C70048A3D87E173BFE1281A3002E68183913B64202F1C88ACC1FD91CACB1F5320BB878DC29A81C31136C80B67867BE3D5E8310A731FC7782265445F640E1E869C01A360F61C81EC7E43B88F004775E37C17BCF5EA8927FEDE3836CA0BC183872364E0509C1D3A0CAFBC462385CC66B086CD1D3484BC8C2CE1A3DF50140C1C04F390FEF834B80FB2877A229FF5B3997BDEF29E9FED7548ECD406779B364464BDC6505DB0B6C5835AB5718A5ADC55C11D9FB839A7476FE4F51E88DC9E0391D77730CC3DFB037D87A190BF0B594017B040CF1ED01719833CF161D850DCF31A8BE5DDBB62C5C0FED8CB02FA26A5FA76C820A4F6ED818C81DD9035A01B72599417F41E80A2DEFD90CF423BAB7F1FCEF542E6905E481EDA17AF470DC4556AE4C7810311C4C27E5F2F4FFC317808DE8EE88F1CF68AD9DC5BD857044DB32F3DA3B01FC179F620F85EC060FE66B3F061403F6CB1B2C4FDAA5571C9AD02226B3584EA86B51D625D2B629BCA029BD879BD6DD218796DDAA1B8634FA037B5D9A50772BBF582B97D571475ED8DEC6EDD91D7AF2F0A78588627B5316438AEF41FA0809CD3A93D4E0D198CE76346E39DA727327A75C7A73E8CDEBD3AA2A047771477EB89C2EEDD95E79C9E3DF0A9574FA4F6EC8614CF9E4819D60FB1C307E007BE5FD0AE137674EE89EB8306E279FF5EC8A6C6F27A1224859CCFEE259BFB95F3BA7706BA77017A744156CBC6486ADE0837EB7C85ADC471C7A91CAE3ABA23A24A1DA86EA9ACF0C8D90D276D34D8CFC99744FFDAE4820F956B22AF490B1ED29D8774C7E74E5D504C4A6FDD16D99D3A23BF6B77A477EA8A94FE4370B98727423A76C3EC662D70BE575FC40F1E8CC4AE3D904986D37BB44716A990D1BBB86B371475EB823C3297D3A32B327BD05C29B414DE91CAF62CD6B31796B7FA1E01CDBE53A2E24DB6542F7B11409FEEBCB30B05DD19053D29EC5E04C733D1BE15F0EDB72868D00071E55DF09401E782A39382E34F83336E3A7F8183D5EB43F5C0D20E7F39B922C6468B539C8C3739E385A515E2750E4876AB884F5F7D85B43AB578607B14B7F91ED9DFB7406E7B36D8D476CAF7EDF0AE7B6FDCA069CF6FDA16731B7F8F18028FEFD91BEFDA77415AC72E78DFAE2552DA3747569B96C869CDFD6D5A23B35D6B64B4FD1EE96DDB20AD4367A4B4EB8CB42EDDF0A8771FAC68D11AD39A36C7E6D6ED71BD775FBCECD806599DDBC3DC8EF7B36342B74E40A7D6286ADE10392CC633DC2BE2BDDE01CF6D6CF0D460C03547179C50D15C758EB8E558119195A9C92702D2E882C3047892F4C6C18064A33D92EC3448D6D8238E80DFD8DA20BF465564D7AA868C86B508F05BE4B42393ED3A20A96B2F5CEF3E005E551B605CCD8638FCED7778D5B13392DB7445563B9A64878EC8EAC05C4C60F96D39B66DAF0828BF751B1285D5B22332BFA3B65B77C35FDDFA21E8BB3698D4B80536F1FD2D36D96F3B76400E8591D7B205F29A368599692EB37A15A4BBB921CDE888145B1D526CEC90ACD521DED111B71DD92A1247ACD684EBCEEE08F7F88A9A5459E3A98313B5688DE39C7C65D023D1C60A49963648B5B0452AC7142B2B7CD0DA20C9518B37E5B4F858A70A321AD5476A9366486EDB09975B7785778D461859B5360E7FD7146F3A7540DAF75D50D4A10F996F47FA1E39AD5A20BB65737C22B3592D5AC0FC6D73E4356B8EFC66AD91DDB40D3E35EF8047ED7B626EBD261853A31ED6366C893B5DFBE35DAB3648AFFF35921948125D5DF0D6A8C75B3B5B7CB05623CDCA0E99D65AE4E80C4823C8449303AE1B4D0AC8FB46275C2F5711E1D5EB42F5D0CA168F4C4EF44935F60A480707C43308A5596991A9D2204345B0547FAAB50A1F341648B45321D5494FC04C3DE5DCF1BA661D447DF5357C2A7D89E11E9570E8BBAF6962CD91CCFC944B0D65346D86F46F1B238DF4F1DB8648F9B611529B7E83F4460D29A886C86CD800698D9BE03D01DFFABE2DA654AA0A4F63392C70AA844B5FB7C0E34AD5F18677C5EBB548D4DA22496D45A15BE023638950A6A51A996A3B2490F7173A357E7730E2A080742E872B2E6ED852959ABC67698DC7CE2E384AC91C21B8046767A4D9E99069A145B64A8B4F1634056ECA73D023D956A59068F529F7DD3738E29A8B2B36D2125A714D53D2F20AF6B8D4A8261E366888F846CD114F00F14D1B23EEDB26886BDE18F1DF7DC3E7864868D6086F09F65D33F9DD08F71B37C2C9268D30DCC111ADC9878F4A87FD956B238667DFD168F1426F877704916C6B85346B2B0ADF0A9FA8808F5CFB912923D14285D7062D62CB97C71EF2F1C8B9BC02F2678F1A50DDA5961E32221DE4A6239C4CD01B9161A55100E6593BC04C077E6F618544CB124DBEA746DF9B8CB8CB087CC9DD0347AA54C6B20AAE68CCBDB54801EE0684D5F24004B57ABA462D1CF5F0C0894A9571BA6A0D9CA95113E7BFAC81989A5571B65A0D44B1903EC1FDC76B56633EAB81355F5643076B6BD4E13903C8C3B6FA4D70A04A155C702F8FD8724EF8CB5ECB20438D1250BA8D2DD228E84C069C2C9D061FECAC914045DC226F62AEF7188C149FAC4273BD6F414D3A3A13A0358E72F29DDE5E31812C15C9921AB56320B2552345CD0379C84747073C7430613DD74E2679D1743A72ACC7CBEA52CA6DF8DCCF4A8511F4E3A1324F1A47F2A130C7F30E7F3EFB93C9F1BC77BC850DBC29C0B1D69618442176E1BE7A9CFF8A2442EB441A459A4BDA4D7A52CE0D89F60E48B5B5A30BA990C9FB522DA9493B1BBCB3618E37EA149012401FEA4CB8ED5401119508F2112FFF8B208F7194C093A8D3524296F8C8CBD3ADD4C850EB904ABF4DE2DC3B6B1BC431A245D3A486F077139A4A5D465F773E9B488E2479AE49AA63658D1A3296522D5A8ABC9777D548554B477927A0BE245522952758270AC185CF15485F93BA939690BF7BEE5F22CECE09D9D6F6345515F26DACF181A398700241BE316A71DB6850403E26C83B8EAC783C6A33F090D1670C3CFF01A941BAB5458984E8A702F223352B4E9EC468F64CEB807D7AF11B02A226B41C8DAC9A1CD45A18281483A52DECF84EC868650323CF35D277EC99AAF4DCAFB5B154484F21D8D35A4C163A8E9C271FF6DCA3274035470D4703AD4484D78034436587DF5CBFC26B8D8B0232D7C21266AE91785106F2B541831B4C7F02F211F9BCCD8A4701798F2F9ED22705E4093E0BC8341B82A40995814C2513390CD5D994CE4BAD132235CE4AA0712070173B13F4B65A0594AD1081AB09484D4B5093713D7FEB28240DF7AB99CF6CEDD450ABD5B0E7D906FA9D810C6BAD74DC6F010DF78B35B8DA5AC381694C0428C045DBFEE421DAB9121EAB193328D03C8D357218045369E21F35B648B0B560FAD3328518151C0FF425E6BAB71241DEA1341E3939538B36A520B5345386674A3A959AC9D6B0F2E15C06B59546677E421F8DD43951939664C8068E364645433A5EA6E14536D61AD83041EB085CCBE25FC37D763CC786266FC940A1E2B9161C6DB9DF8E64CD336CB85FCDF78AF6C88323DDC59E11534BD335D1D7C594C7924EBB55C23DB53D5259A864692C91AFB755226B9A9D9D92DB25BA0A48093C0FED59F1B854C46E4668D54D32F7C0C585136AC55CDF690CF8C44BD32C986C694A663B1782A4A3F3E044069E58B6313B0CF6F8966B1D488ECCA506A6193519B220090815838A35E7B41C059C8D00A0E0846CA941D1A88E26A6673053536B56345F4BE63F2B9A9CBC1770222021B110F1DF31A4A84A55F0A7BD09C9767A64F0EC5CCEA7D39A244F7EA07FC6D9EB71C350127824BDDD70A9C0E84A903778E07DE6C663A59A7CAF352A20336972922B3F599B1874989F0852A2D75DDAFD0E8ECDB8D6892666529119D10AB5604581A9E8472A06230DE7B4242B9E6F53AA21279223D73A949AA60DC992732AAEB1620A90D1961AB190390AC78AA38E24A949A2F071D6A977B50624D032D2C9AF9915593A23F62746DB64A69EB754C02D82141C228C92144290B729DD8734D713948C4820D9E04073B551484C34C3866983FE94C62E2549A3473C2B8948BD012DB956FC47FC46C3DCA9E72852B764A216121032674BED88003CF82CE9E11B92302D5154D68B00049815F9B026A3D61490805431F0C9B39C2BD179027D3BA65235DC566B1864041C030FCD5E290A08F203F74B55F407D39B98EB039677D79D5D69AEAC78EE70527CF20425252093688A9242C42F25E1A6D3B75219203E5AB26027E00416F3FB59B897041E15AC85496A504B896A285DF1370BFE162D095031E3F21C65FD3C8F9AD850B709C69BCAA30A7F1B498E5A465D5A892599B520E3368C9A65202D78B69C519BE4CFB9F3047957CB9292EB33A99432908AB93223C4B158F883659DB85D19C83DACA7557F1090803CC90D6520C59985CA40A6D0745368B209F4BDD74C1F918CB8C2B4890C89B98916B49C5793543433157D4C313DCE4974945CB7D2A9320E9BAAE017B75A88FCE24BF4B1D12A79B182BD3D8C3AFDDF20AD69CA02D282429751B42DD1D587EFCFB344BBCF623C9540C527B3B9FE7F8234FD0BE4AEAA6CB504E403A61001A944577621922385D26CD985D8D24C19843ED0F7E239BE64688E60086FC1B5FF016943663464F01F20459B9C136D49F5B2BF7A43DC74AF836B3A379CAD5013DE86728C9AD6284F97B0676D2AA947809680A5D973B429F55D01E9CDE7982FAAE3BE964A606DAD041CBE2F0B3C4974110179EBBFCCF55F20C55CCB4026530BD25EA5310A96817C2F2019905E306A456874A5201958385A72BDA4020B928AFB04A0FC135B4C5634519714C8D4F26B8D3A88A1B96F633DDC8DCC552619C5CC09D486E75B513B42251AB5E25E160DDC5B02D212E73CAAE30123ABF094CA7D697C97C6318365E77B5ADE1B3D030F414A0DFE405A2D069EBF41DE3739E218B52220130832894C26F17D0ADB2F694805E43B46B23702925DC1EE7F8014201220AC38AFE285E24B0252C04B31A0E3B975C98CD4AFC7AB5443949B3B7EA094257055A0E6750286D1D242F69224978A166DF92CC58544D7EAA4313CEB8C47353C286D9253280C293D056C3A4127B278794590374D25202585FCEEE45602F2364D4D401EE52152A027D071452A42529827F3400198C072ED259F9FB2DF0C6783DA9C6B9DC8BC00B1A5106CC8B01535A93049018926D424492595B9D697F71C621772B27A35AC607F289156FC55F3BF80B451F697041EC9932349A7BFA88AFB0C7EA9363AA5A94F217DE01E29D8DF3275BD6074BDC1BAFA10D7FE4990BF11E48E2AB54A40DE53BA104B4502F1049940A98864DE33F07CA09909C078FACC736AF631E7B769B5FF00C9845F0AD04A982473123C4413760280CF35580C74E3FA90AA55B0AE9207FCD9C9D4A0894B9524D1548A032B967E96620DD48E8AE70AC8B2524F22F108D2290F8264C5239F3C12C9F77B9EFB9E1A4DA126E3E993D28A5D2F05798F79F26F90B7D859FCE9E4A28094EF3CAFA9EEB7AC1EDED2B7DE31F02431BABE2588D7BCF4A51D9B5246C3EDAC2C04A4A3C284ADE28F960292172A4C2BEF6D142D49E5238580741AFD3CDC31CCE30B3423436E8C861A92AD62EAE287F4670A460096F8B385A249C9B5027218E9B84715DCA6D013E9C3AF399FC892EF3D05FF8E1617C7CAE919F9BA46ABDCC7B5B106275C7172C70EF990759D4CC532851CE27880932FA8A95754BD9024DD77566C529923DFF2F037AC26EE3B9AB093ED8CF8A4543C4AD741B2E6FA92EAC652A94B256F6A15E07C66A926A629F9B222A91CC944218A3F4BCD2AD58DB56892A66A413791942405BE967BCB3429BDE9918A95580CE8904061BF643C88A712DEF2EE446A523EB63D64CEBD6C302920EF199DF1ABC9B504E46F644801C983E57BE53316B87F897D3380245242025234F9920C3F63EAB8C12E65330FFB8E6B9D09D2C87DA21149FA5214085809446A5E2E75AA302D9D87867E67CF34632239B01696364B345DA24902A33695F421115BCE225FE2CF653E29FDEB21F72F709B65671C2BAFE7DC13474DBEE49A386AF395DA06B16C9E2FB01A5334E9501E178DAED82E1F97AF72D15DE7F238401B97C92704F9949708D0783AF47BB642EFAC58CED1E15FB12BBF6372409883BD121DCB91111341886F49512DE59D14DCB694B02DF70A3815DB3605282D416F6D84D1C2816941A76859DA2D1D19D45298924634B41C3B9AB29086FCE828007B8296467B38697F7977DCD239E00D8B8132904FF9FE25ADE105EFBDA3B655408AB2EE3894C30543F912905728BD3F588FEEA7A94470F29191ED14997AC643DEB0CA7F4BC9C751F2CFE97B8F6926D718C1366935A505BA9476528897E44325D2D24C957422158B981E6B576BA95FE9FBA21D09543A9E25A9454741CA3EA50FE55C49FB555AD32AE7FDC79FE533C83E17575A9211AF09528089D93E27FFCFF9FC9C1DCD6D3B5B9CB7372A5F1D6F1B9C154D6E1373BD4233B9CDCA601FCD4BBEA33C20C8C794E453A6815734B17794FE5B0B4A8E8CDDA7ED5FA5A96E2250D1A41B2F70652F273D9F34C7A21D29EF3424314335414AE0115316AD947426D40E99920EA52C4508280D01CA3B119852F4973E971503F2AD689F63395C61B5236E23BCBDE7F36B5A8044FD67D4E22D9AEC396A5294759DFDE479877F80FCC3C50DFBB94940DE671E7CC4AEE009352B07C5D3B4845EB27E7DCC92EA1ADB99CD2C18A476751790B69630D24C8541F1313D811A0408E785441392D0CB22A53C0B303D41D9739D3C0B95CC957C0251D6B0249477D2044817E2470B38E4EC865FE8060F981F254648C0111E9FD1E29E528B37D56A44EBED1590BFB3FC8C319647987C19F8959789262359CDECE0E41DFAE43D9AD6434AFE39DFBDB1B4C76B6AF22F46BF87BCE01A0F0BA7AFF5E3DA66644AEA5229C0851129DF1AF09DB45365D4B094CA3E68C91AE92A648F1404F2BBECBDFC96F7F22CED98AC93F3BB901649AB55EE0B5C61CB172B399B9615473F7F42FE1FD3359E4A50A46F9FB13728CABAAA774094C1059B2B334F9EE78BD80A1ED849A96EE3F31F4EAECC456AE5A0A7AC2C9EDB1809D6804764FE29C3F42336CDBF299F24ED106CED82508D092B68368B29CD85DCB39866BF84D25DC2B420B48896B2905621B4A0749475C2B48C0B1960E653A8322EFA2F5A4C412F275F3FF18C3DD4E41F4CEE776CEDF188813096FCBC521B201FE21E52B34F6D8DB869ABC7098D065B88E3A293138E907E6485A5FA9511F3122B9E18370FE5FF93512C806F3AB8E0B1B11CFEB4D4E29ECA9EF6EEA098C4139AE67DB58A666183DF0CEE88D1B92BCE7D81C5B07CE98E2178F9CFB5FCFB2C86A5DBFF8FCE9B9CFFA6182106BD186706091717FCE2EC824B2EE5FE459799DEA48AB9C62AE79EC640707678C426FE01FDFE09C7BF5801FD498DDEB133E141C52ACAFF26C304A44705EC752F8F95352B4345D445B7EBD4C331BD114718743670C1294AE086B61CEE5A3822C195953FB5F980D28CE5DC1D82BCA551E1B2AD052ED3742F33EC5FA1995C65BBF31B9BD92BA4CB7CFE95EF84E4F9DFA4FD07711D03978C65FB85AEB2F5BAC2D2F1B2CE0E57346AFC4EBAC97B6E339DC8D74571A587D6D6900FE3B12C54EEE89C70CEE8A86850E872AD9A4A1311AAD3E2C2045FA836383BDD3D58B97271B4AB1B4E53A21BE9F03F70612C1DF6968307621874EE38BA11981E7FB0CABFA657E392D602BFE82D71D5A0A646EDF11B23DA559D3D2E33B42BC4C37FE525BF32D594FDFE7BEE6F322863D93A899A8A804A7F5FD4E9705EAFC10546F35FED35B8C57A37D6C9C48ACB88DB047F43A7C6759E7B8D79F3BCD6014738BF967CEF376A71DCB53C4ED7AE8D7926D3E7673BB6E6ABEE2D9ED3D957A58A3F5EB9124E5193D7AA55510290FC1B601B7DE6185BA3B395AAE0280F3BA6B1C75186E6A3D2989A0C1CF99BCF87590C1F64FE3A40C685F633C21DA01B1C6010F8F7F84F3229A3EC3BC8F54758C91C61F43EC4517E4732DFEDA10085F619F53824FFE37076A4B5E9B157CB74A7A19FEA6CB19399404C541473D0A4C76996A57B28F07082F4ABE09680B4145FB61D2F4C211DDB846CAC52B520823E124969FEFA850722F57A05E84FCC752B386E2EA54D8C663F7214B3161F96771B4BE92792BCFFBFC6FF26D95B7656C91D256BE59E752401205A127E84E4FD9AD277328AF5457DE1CE665E8DD3EEEE38FAD5979FC7E9B57997D686CC406E2EB3744EBE555EEC8346C31CDD9E6FA8551F472AD74454795744BB32105474C3199313CEB0A23FC70A228AB9E794689192BE5CAE22CED819197C1C98801D4A47834292ABCED014A3747A9CA554E5F77F46E33FC89EBEC47DA4F3060796610E0C64268524889D634714C5AE278ADD4514AB9B2846CE28FAEE592AE0345DE70473763483DC519D01C74D2EAC88CA6157F5AA98E4A0332FEAD9290C79D9461414510445B0443E58CAA7F79E51AFC9A2E91AFB3B91356B1444B83A61AFC91EC799430F696926F62E38C046F46039671C7476C23EB591E6ED4AC0261CA7D99DA0204EB0033846068F1A0CCA788C39579EFF4D0E0AC91E190F8BB9D3148FD2CC8ED3ACCB48D61E61B7738CB5F2711628A7B92E8A679E260F27640DDF1D7771C20146DF036E6E58CF9E7453E5CA585AAF766ACAE92373919FC91AA3F40F80150A8B2D90926D85942CF5C36D61BDB6F4EA1A1FD6AA0936D5FE123BEA34C081862DB0A74E6344D46D80F0FA7510DEE06BECAAFB0D22F86E6FEDFAD857B73E0E7CDD88D410FBEA0B35F8C7F8DF54B246D6463668803DDF34C4AE86F5B1AB5E3D4490F6D4FD9F14C9E87FA0CED738545BA83EF6D7E6BBDA75B1BB6E5D6CAD5F0B1B1BD74378970E78FEF30620FDC3461464B1522CF9C36758FE3FBF588FEFCA5DFE020000000049454E44AE426082', 'Lock - Red', 'png', 57, 57, 96);
