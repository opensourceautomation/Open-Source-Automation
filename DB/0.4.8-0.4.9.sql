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