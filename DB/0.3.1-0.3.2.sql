-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.3.2', '', '');

-- Add System Plugin property to all base plugin object types
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'FALSE', 'PLUGIN', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'TRUE', 'WEATHER PLUGIN', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'FALSE', 'BLUETOOTH', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'TRUE', 'NETWORK MONITOR', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'FALSE', 'CM15A', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'TRUE', 'EMAIL', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'FALSE', 'SPEECH', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'TRUE', 'SCRIPT PROCESSOR', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'FALSE', 'W800RF', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'FALSE', 'USBUIRT', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'FALSE', 'INSTEON', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'TRUE', 'JABBER', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'FALSE', 'ZWAVE', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'FALSE', 'CM11A', 0);
CALL osae_sp_object_type_property_add('System Plugin', 'Boolean', 'TRUE', 'WEB SERVER', 0);

CALL osae_sp_object_property_set('Script Processor', 'System Plugin', 'TRUE', '', '');
CALL osae_sp_object_property_set('Email', 'System Plugin', 'TRUE', '', '');
CALL osae_sp_object_property_set('Web Server', 'System Plugin', 'TRUE', '', '');

-- Remove the 'Computer Name' property from the Weather plugin object type
CALL osae_sp_object_type_property_delete('Computer Name', 'WEATHER PLUGIN');
CALL osae_sp_object_type_property_delete('Computer Name', 'JABBER');
CALL osae_sp_object_type_property_delete('Computer Name', 'NETWORK MONITOR');




USE osae;

DELIMITER $$

DROP PROCEDURE IF EXISTS osae_sp_object_update$$
CREATE DEFINER = 'root'@'%'
PROCEDURE osae_sp_object_update(IN poldname    VARCHAR(200),
                                IN pnewname    VARCHAR(200),
                                IN pdesc       VARCHAR(200),
                                IN pobjecttype VARCHAR(200),
                                IN paddress    VARCHAR(200),
                                IN pcontainer  VARCHAR(200),
                                IN penabled    TINYINT
                                )
BEGIN

  DECLARE vAddressCount    INT;

  DECLARE vObjectTypeCount INT;

  DECLARE vObjectTypeID    INT;

  DECLARE vContainerCount  INT;

  DECLARE vContainerID     INT DEFAULT NULL;

  SELECT count(object_type_id)
  INTO
    vObjectTypeCount
  FROM
    osae_object_type
  WHERE
    object_type = pobjecttype;

  IF vObjectTypeCount > 0 THEN

    SELECT object_type_id
    INTO
      vObjectTypeID
    FROM
      osae_object_type
    WHERE
      object_type = pobjecttype;

    SELECT count(object_id)
    INTO
      vContainerCount
    FROM
      osae_v_object
    WHERE
      object_name = pcontainer
      AND container = 1;

    IF vContainerCount = 1 THEN

      SELECT object_id
      INTO
        vContainerID
      FROM
        osae_v_object
      WHERE
        object_name = pcontainer
        AND container = 1;

    END IF;

    SELECT count(object_id)
    INTO
      vAddressCount
    FROM
      osae_object
    WHERE
      upper(object_name) <> upper(poldname)
      AND (upper(address) = upper(paddress)
      AND address IS NOT NULL
      AND address <> '');

    IF vAddressCount = 0 THEN

      UPDATE osae_object
      SET
        object_name = pnewname, object_description = pdesc, object_type_id = vObjectTypeID, address = paddress, container_id = vContainerID, enabled = penabled, last_updated = now()
      WHERE
        object_name = poldname;

    ELSE

      CALL osae_sp_debug_log_add(concat('Object_Updated Failed on ', pnewname, ' due to duplicate data.'), 'SYSTEM');

    END IF;

  END IF;

END
$$

DELIMITER ;