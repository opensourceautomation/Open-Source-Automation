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

  SELECT vResults;END
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
DROP FUNCTION IF EXISTS osae_fn_object_exists$$
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
DROP FUNCTION IF EXISTS osae_fn_object_getid$$
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
DROP FUNCTION IF EXISTS osae_fn_object_property_exists$$
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
DROP FUNCTION IF EXISTS osae_fn_plugin_count$$
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
DROP FUNCTION IF EXISTS osae_fn_plugin_enabled_count$$
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
DROP FUNCTION IF EXISTS osae_fn_plugin_running_count$$
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
DROP FUNCTION IF EXISTS osae_fn_trust_level_property_exists$$
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
CREATE OR REPLACE
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
	select `osae_rooms`.`object_name` AS `room`,count(`osae_occupants`.`object_name`) AS `occupant_count` from ((`osae_object` `osae_rooms` join `osae_object_type` `osae_room_type` on((`osae_rooms`.`object_type_id` = `osae_room_type`.`object_type_id`))) left join `osae_object` `osae_occupants` on((`osae_rooms`.`object_id` = `osae_occupants`.`container_id`))) where (`osae_room_type`.`object_type` = 'ROOM') group by `osae_rooms`.`object_name`;

--
-- Create view "osae_v_system_plugins_errored"
--
CREATE OR REPLACE
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
CREATE OR REPLACE
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

CALL osae_sp_object_type_add ('CONTROL STATE IMAGE','Control - Object State','','CONTROL',0,1,0,1,'Displays Images matching different States in the Screens app.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Object Name','String','','',0,1,'The Object whos State is represented by this Control.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 Name','String','','',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 Name','String','','',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 Name','String','','',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 Image','File','','',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 Image','File','','',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 Image','File','','',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 X','Integer','','100',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 1 Y','Integer','','100',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 X','Integer','','100',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 2 Y','Integer','','100',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 X','Integer','','100',0,0,'');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','State 3 Y','Integer','','100',0,0,'');
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
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Slider Method','String','','',0,0,'The Method that the Slider Value is sent to.');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Contained X','Integer','','100',0,0,'The X for the Default position of another Control displayed within this Control');
CALL osae_sp_object_type_property_add('CONTROL STATE IMAGE','Contained Y','Integer','','100',0,0,'The Y for the Default position of another Control displayed within this Control');

CALL osae_sp_object_type_add ('USER CONTROL WEATHERCONTROL','Custom User Control','SYSTEM','USER CONTROL',0,1,0,1,'This combines many basic controls into a single customizable Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Control Type','String','','',0,0,'');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Object Name','String','','',0,1,'The Weather Object\'s Name where all the weather data is stored.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','X','Integer','','0',0,1,'The Left most position of the Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Y','Integer','','0',0,1,'The Left most position of the Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','ZOrder','Integer','','0',0,0,'');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Width','Integer','','440',0,1,'The Width of the Maximized Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Font Name','String','','Arial',0,1,'The Font of the main text.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Font Size','Integer','','14',0,1,'The Size of the main text.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Back Color','String','','Gray',0,1,'The Background Color of the Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Fore Color','String','','Black',0,1,'The Color of the main text.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Height','Integer','','270',0,1,'The overall Height of the Weather Control.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Minimized','Boolean','','TRUE',0,1,'Display Only Today\'s Weather.');
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Max Days','Integer','','5',0,1,'How many Days of Weather to Display.');

CALL osae_sp_object_type_add ('SPEECH','Generic Plugin Class','Speech','PLUGIN',1,0,0,1,'Speech Plugin handles Text-to-Speech output to the Default sound device.');
CALL osae_sp_object_type_state_add('SPEECH','ON','Running','Speech Plugin is Running.');
CALL osae_sp_object_type_state_add('SPEECH','OFF','Stopped','Speech Plugin is Stopped.');
CALL osae_sp_object_type_event_add('SPEECH','ON','Started','This Speech Plugin has Started.');
CALL osae_sp_object_type_event_add('SPEECH','OFF','Stopped','This Speech Plugin has Stopped.');
CALL osae_sp_object_type_method_add('SPEECH','ON','Start','','','','','Start this Speech plugin.');
CALL osae_sp_object_type_method_add('SPEECH','OFF','Stop','','','','','Stop this Speech plugin.');
CALL osae_sp_object_type_method_add('SPEECH','SPEAK','Say','Message','','Hello','','Speak this Text.');
CALL osae_sp_object_type_method_add('SPEECH','SPEAKFROM','Say From List','Object Name','Property Name','Speech List','Greetings','Speak from a List of Text.');
CALL osae_sp_object_type_method_add('SPEECH','PLAY','Play','File','','','','Play Media File.');
CALL osae_sp_object_type_method_add('SPEECH','PLAYFROM','Play From List','List','','','','Play a Random entry from a list of Media Files.');
CALL osae_sp_object_type_method_add('SPEECH','STOP','Stop Playing','','','','','');
CALL osae_sp_object_type_method_add('SPEECH','PAUSE','Pause','','','','','Pause media playback.');
CALL osae_sp_object_type_method_add('SPEECH','MUTEVR','Mute the Microphone','','','','','Mute the Microphone.');
CALL osae_sp_object_type_method_add('SPEECH','SETVOICE','Set Voice','Voice','','Anna','','Pick a Voice from the installed Windows Voices.');
CALL osae_sp_object_type_method_add('SPEECH','SETTTSRATE','Set TTS Rate','Rate','','0','','Set Speech Speed from -10 to 10.');
CALL osae_sp_object_type_method_add('SPEECH','SETTTSVOLUME','Set TTS Volume','Volume','','100','','Set Volume Level from 0 to 100.');
CALL osae_sp_object_type_property_add('SPEECH','Voice','String','','',0,0,'The currently selected Voice.');
CALL osae_sp_object_type_property_add('SPEECH','Voices','List','','',0,0,'List of Voices loaded in Windows.');
CALL osae_sp_object_type_property_add('SPEECH','System Plugin','Boolean','','FALSE',0,0,'');
CALL osae_sp_object_type_property_add('SPEECH','TTS Rate','Integer','','0',0,1,'The current Rate of Speech from -10 to 10.');
CALL osae_sp_object_type_property_add('SPEECH','TTS Volume','Integer','','100',0,1,'The current Volume Level from 0 to 100.');
CALL osae_sp_object_type_property_add('SPEECH','Speaking','Boolean','','FALSE',0,1,'This Speech Client is currently Talking.');
CALL osae_sp_object_type_property_add('SPEECH','Debug','Boolean','','FALSE',0,1,'Add Debug Info to the Logs.');
CALL osae_sp_object_type_property_add('SPEECH','Trust Level','Integer','','90',0,1,'The Trust Level this plugin has to control other Objects.');
CALL osae_sp_object_type_property_add('SPEECH','Version','String','','',0,0,'The Version of the Speech plugin.');
CALL osae_sp_object_type_property_add('SPEECH','Author','String','','',0,0,'Who wrote this Speech Plugin.');

CALL osae_sp_object_type_add ('GUI CLIENT','Touch Screen App','','THING',1,1,0,1,'');
CALL osae_sp_object_type_state_add('GUI CLIENT','ON','Running','This Screens App is Running.');
CALL osae_sp_object_type_state_add('GUI CLIENT','OFF','Stopped','This Screens App is Not Running.');
CALL osae_sp_object_type_event_add('GUI CLIENT','ON','Started','This Screen app Started.');
CALL osae_sp_object_type_event_add('GUI CLIENT','OFF','Stopped','This Screen app shutdown.');
CALL osae_sp_object_type_method_add('GUI CLIENT','SCREEN SET','Screen Set','Screen Name','','','','Force this Screen app to Switch Screens.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Current Screen','String','','',0,0,'The Screen currently being Displayed on this Screen app.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Default Screen','String','','',0,0,'The First Screen to display when this Screens app starts.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Current User','String','','',0,0,'The Person logged into this Screen app.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Debug','Boolean','','FALSE',0,1,'Add Debug info to the Log.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Title','String','','OSA Screens',0,0,'');
CALL osae_sp_object_type_property_add('GUI CLIENT','Logout on Close','Boolean','','FALSE',0,1,'Reset the User when this app closes.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Trust Level','Integer','','50',0,1,'');
CALL osae_sp_object_type_property_add('GUI CLIENT','Width','Integer','','640',0,0,'');
CALL osae_sp_object_type_property_add('GUI CLIENT','Height','Integer','','480',0,0,'');
CALL osae_sp_object_type_property_add('GUI CLIENT','Show Frame','Boolean','','TRUE',0,1,'Show Windows Frame around the Screen with Min/Max/Resizing.');
CALL osae_sp_object_type_property_add('GUI CLIENT','Use Global Screen Settings','Boolean','','FALSE',0,0,'');

CALL osae_sp_object_type_add ('WEB SERVER','OSA Web Server Plugin','Web Server','PLUGIN',1,0,0,1,'');
CALL osae_sp_object_type_state_add('WEB SERVER','ON','Running','Web Service is Running.');
CALL osae_sp_object_type_state_add('WEB SERVER','OFF','Stopped','Web Service is Stopped.');
CALL osae_sp_object_type_event_add('WEB SERVER','ON','Started','Web Service Started.');
CALL osae_sp_object_type_event_add('WEB SERVER','OFF','Stopped','Web Serviced Stopped!');
CALL osae_sp_object_type_method_add('WEB SERVER','ON','Start','','','','','Start the Web Service.');
CALL osae_sp_object_type_method_add('WEB SERVER','OFF','Stop','','','','','Stop the Web Service.');
CALL osae_sp_object_type_property_add('WEB SERVER','System Plugin','Boolean','','TRUE',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Timeout','Integer','','60',0,0,'How long you can sit idle on the Web UI before you are required to sign in again.');
CALL osae_sp_object_type_property_add('WEB SERVER','Version','String','','',0,0,'The Version of the Web Server Plugin.');
CALL osae_sp_object_type_property_add('WEB SERVER','Author','String','','',0,0,'Who Created the Web Service');
CALL osae_sp_object_type_property_add('WEB SERVER','Config Trust','Integer','','69',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Analytics Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Debug Log Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Default Screen','String','','',0,0,'The First Screen displayed after logging into the Web UI.');
CALL osae_sp_object_type_property_add('WEB SERVER','Event Log Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Images Add Trust','Integer','','55',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Images Delete Trust','Integer','','60',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Images Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Logs Clear Trust','Integer','','60',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Logs Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Management Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Method Log Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Add Trust','Integer','','50',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Delete Trust','Integer','','60',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Objects Update Trust','Integer','','55',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Add Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Delete Trust','Integer','','60',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Object Type Update Trust','Integer','','55',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Add Trust','Integer','','50',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Delete Trust','Integer','','60',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Trust','Integer','','50',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Pattern Update Trust','Integer','','55',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Add Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Delete Trust','Integer','','60',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Reader Update Trust','Integer','','55',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Add Trust','Integer','','50',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Delete Trust','Integer','','60',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Trust','Integer','','50',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Schedule Update Trust','Integer','','55',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Screen Trust','Integer','','20',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Script Add Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Script Delete Trust','Integer','','60',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Script Object Add Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Script ObjectType Add Trust','Integer','','60',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Script Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Script Update Trust','Integer','','55',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Server Log Trust','Integer','','50',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Values Trust','Integer','','45',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','ObjectType Update Trust','Integer','','50',0,0,'');
CALL osae_sp_object_type_property_add('WEB SERVER','Hide Controls','Boolean','','FALSE',0,0,'Hide the Controls that make up Screens on the Object page.');

CALL osae_sp_object_type_add ('SYSTEM','Core System Data','SYSTEM','SYSTEM',1,1,1,1,'The main system object.');
CALL osae_sp_object_type_state_add('SYSTEM','HOME','Home','System is in Home mode.');
CALL osae_sp_object_type_state_add('SYSTEM','AWAY','Away','System is in Away mode.');
CALL osae_sp_object_type_state_add('SYSTEM','SLEEP','Sleep','System is in Sleep mode.');
CALL osae_sp_object_type_event_add('SYSTEM','OCCUPANTS','Set Occupants','');
CALL osae_sp_object_type_event_add('SYSTEM','AWAY','Away','System was set to Away mode.');
CALL osae_sp_object_type_event_add('SYSTEM','HOME','State Set to Home','System was set to Home mode.');
CALL osae_sp_object_type_event_add('SYSTEM','SLEEP','Sleep','System was set to Sleep mode.');
CALL osae_sp_object_type_event_add('SYSTEM','OCCUPIED LOCATIONS','Occupied Locations Set','');
CALL osae_sp_object_type_event_add('SYSTEM','PLUGINS ERRORED','Plugins Errored','');
CALL osae_sp_object_type_method_add('SYSTEM','OCCUPANTS','Set Occupants','Number of Occupants','','0','','');
CALL osae_sp_object_type_method_add('SYSTEM','AWAY','Away','','','','','Set System to Away mode.');
CALL osae_sp_object_type_method_add('SYSTEM','HOME','Home','','','','','Set System to Home mode.');
CALL osae_sp_object_type_method_add('SYSTEM','SLEEP','Sleep','','','','','Set System to Sleep mode.');
CALL osae_sp_object_type_property_add('SYSTEM','ZIP Code','String','','',0,0,'');
CALL osae_sp_object_type_property_add('SYSTEM','Latitude','Integer','','0',0,0,'');
CALL osae_sp_object_type_property_add('SYSTEM','Longitude','Integer','','0',0,0,'');
CALL osae_sp_object_type_property_add('SYSTEM','Date','DateTime','','',0,0,'This is the current Date.');
CALL osae_sp_object_type_property_add('SYSTEM','Time','DateTime','','',0,0,'Current Time in Military format.');
CALL osae_sp_object_type_property_add('SYSTEM','Day Of Week','Integer','','0',0,0,'Current Day of the Week.');
CALL osae_sp_object_type_property_add('SYSTEM','Violations','Integer','','0',0,0,'Total alarm violations in series.');
CALL osae_sp_object_type_property_add('SYSTEM','Day Of Month','Integer','','0',0,0,'Current Day of the Month.');
CALL osae_sp_object_type_property_add('SYSTEM','Time AMPM','DateTime','','',0,0,'Time in AM/PM format.');
CALL osae_sp_object_type_property_add('SYSTEM','DB Version','String','','',0,0,'');
CALL osae_sp_object_type_property_add('SYSTEM','Debug','Boolean','','TRUE',0,1,'Debug control the amount of details in the logs.');
CALL osae_sp_object_type_property_add('SYSTEM','Prune Logs','Boolean','','TRUE',0,0,'');
CALL osae_sp_object_type_property_add('SYSTEM','Plugins Found','Integer','','0',1,0,'');
CALL osae_sp_object_type_property_add('SYSTEM','Plugins Running','Integer','','0',1,0,'The number of Plugins that started successfully. ');
CALL osae_sp_object_type_property_add('SYSTEM','Plugins Enabled','Integer','','0',1,0,'Total number of Plugins that are Enabled and will startup automatically.');
CALL osae_sp_object_type_property_add('SYSTEM','Plugins Errored','Integer','','0',1,0,'The number of Plugins that failed to start.');
CALL osae_sp_object_type_property_add('SYSTEM','Plugins','String','','',0,0,'Total number of Plugins that the system is aware of.');
CALL osae_sp_object_type_property_add('SYSTEM','AI Focused on Object Type','String','','',0,0,'System is curious about this Object Type.');
CALL osae_sp_object_type_property_add('SYSTEM','AI Focused on Property','String','','',0,0,'This is what property of the object the system is curious about.');
CALL osae_sp_object_type_property_add('SYSTEM','Trust Level','Integer','','50',0,0,'This is how much the system is trusted.  ');
CALL osae_sp_object_type_property_add('SYSTEM','Detailed Occupancy Enabled','Boolean','','FALSE',0,0,'If TRUE, it will try to guess where everyone is.   Works best with single occupancy.');





