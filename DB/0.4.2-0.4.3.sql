
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

DELIMITER ;

-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.4.3', '', '');
