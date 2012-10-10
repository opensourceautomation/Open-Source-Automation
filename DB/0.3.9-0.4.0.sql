
USE osae;


delimiter $$
CREATE TABLE `osae_images` (
  `image_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `image_data` longblob NOT NULL,
  `image_name` varchar(45) NOT NULL,
  `image_type` varchar(4) NOT NULL,
  PRIMARY KEY (`image_id`)
) ENGINE=InnoDB AUTO_INCREMENT=257 DEFAULT CHARSET=latin1
  $$

delimiter $$

CREATE DEFINER = 'osae'@'%'
PROCEDURE `osae_sp_image_add`(
IN  pimage_data         LONGBLOB,
IN  pimage_name			VARCHAR(45),
IN  pimage_type			VARCHAR(4)
)
BEGIN

	INSERT INTO `osae`.`osae_images`
		(`image_data`,		
		`image_name`,
		`image_type`)
		VALUES
		(
		pimage_data,		
		pimage_name,
		pimage_type
		);
END$$

delimiter $$

CREATE DEFINER = 'osae'@'%'
PROCEDURE `osae_sp_image_delete`(
IN pimage_id INT
)
BEGIN
	DELETE FROM `osae`.`osae_images`
	WHERE image_id = pimage_id;
END$$


delimiter ;

alter table osae_object_property modify property_value VARCHAR(4000) DEFAULT NULL ; 

-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.4.0', '', '');