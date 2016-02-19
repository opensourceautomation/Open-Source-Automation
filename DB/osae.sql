--
-- Definition for database osae
--
DROP DATABASE IF EXISTS osae;
CREATE DATABASE osae
	CHARACTER SET utf8
	COLLATE utf8_general_ci;

-- 
-- Disable foreign keys
-- 
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

-- 
-- Set SQL mode
-- 
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 
-- Set default database
--
USE osae;

--
-- Definition for table osae_debug_log
--
CREATE TABLE osae_debug_log (
  log_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  log_time TIMESTAMP(2) NOT NULL DEFAULT CURRENT_TIMESTAMP(2) ON UPDATE CURRENT_TIMESTAMP(2),
  entry VARCHAR(255) NOT NULL,
  debug_trace VARCHAR(2000) DEFAULT NULL,
  PRIMARY KEY (log_id)
)
ENGINE = INNODB
AUTO_INCREMENT = 3997
AVG_ROW_LENGTH = 85
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_images
--
CREATE TABLE osae_images (
  image_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  image_data LONGBLOB NOT NULL,
  image_name VARCHAR(45) NOT NULL,
  image_type VARCHAR(4) NOT NULL,
  image_width INT(10) UNSIGNED DEFAULT 0,
  image_height INT(10) UNSIGNED DEFAULT 0,
  image_dpi INT(4) UNSIGNED DEFAULT 0,
  PRIMARY KEY (image_id)
)
ENGINE = INNODB
AUTO_INCREMENT = 136
AVG_ROW_LENGTH = 3491
CHARACTER SET latin1
COLLATE latin1_swedish_ci;

--
-- Definition for table osae_log
--
CREATE TABLE osae_log (
  ID INT(11) NOT NULL AUTO_INCREMENT,
  log_time TIMESTAMP(2) NOT NULL DEFAULT CURRENT_TIMESTAMP(2),
  Thread VARCHAR(255) DEFAULT NULL,
  Level VARCHAR(255) DEFAULT NULL,
  Logger VARCHAR(255) DEFAULT NULL,
  Message VARCHAR(4000) DEFAULT NULL,
  Exception VARCHAR(4000) DEFAULT NULL,
  Date DATETIME DEFAULT NULL,
  PRIMARY KEY (ID)
)
ENGINE = INNODB
AUTO_INCREMENT = 1620
AVG_ROW_LENGTH = 120
CHARACTER SET latin1
COLLATE latin1_swedish_ci;

--
-- Definition for table osae_object_pattern
--
CREATE TABLE osae_object_pattern (
  object_pattern_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  object_id INT(10) UNSIGNED DEFAULT NULL,
  pattern_id INT(10) UNSIGNED DEFAULT NULL,
  PRIMARY KEY (object_pattern_id)
)
ENGINE = INNODB
AUTO_INCREMENT = 1
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_property_scraper
--
CREATE TABLE osae_object_property_scraper (
  object_property_scraper_id INT(11) NOT NULL AUTO_INCREMENT,
  object_property_id INT(10) UNSIGNED DEFAULT NULL,
  URL VARCHAR(4000) DEFAULT NULL,
  search_prefix VARCHAR(255) DEFAULT NULL,
  search_prefix_offset INT(11) DEFAULT NULL,
  search_suffix VARCHAR(255) DEFAULT NULL,
  last_updated DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  update_interval TIME NOT NULL DEFAULT '00:30:00',
  PRIMARY KEY (object_property_scraper_id)
)
ENGINE = INNODB
AUTO_INCREMENT = 9
AVG_ROW_LENGTH = 2340
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_state_change_history
--
CREATE TABLE osae_object_state_change_history (
  history_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  history_timestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  object_id INT(10) UNSIGNED NOT NULL,
  state_id INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (history_id)
)
ENGINE = INNODB
AUTO_INCREMENT = 73221
AVG_ROW_LENGTH = 38
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_type
--
CREATE TABLE osae_object_type (
  object_type_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  object_type VARCHAR(100) NOT NULL,
  object_type_description VARCHAR(200) DEFAULT NULL,
  plugin_object_id INT(10) UNSIGNED ZEROFILL DEFAULT NULL,
  system_hidden TINYINT(1) UNSIGNED NOT NULL DEFAULT 0,
  object_type_owner TINYINT(1) UNSIGNED NOT NULL DEFAULT 0,
  base_type_id INT(10) UNSIGNED ZEROFILL DEFAULT NULL,
  container TINYINT(1) UNSIGNED NOT NULL DEFAULT 0,
  hide_redundant_events TINYINT(1) UNSIGNED NOT NULL DEFAULT 1,
  PRIMARY KEY (object_type_id),
  UNIQUE INDEX osae_object_types_unq_type (object_type)
)
ENGINE = INNODB
AUTO_INCREMENT = 244
AVG_ROW_LENGTH = 256
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_verb
--
CREATE TABLE osae_object_verb (
  object_verb_id INT(11) NOT NULL AUTO_INCREMENT,
  object_id INT(11) DEFAULT NULL,
  verb_id INT(11) DEFAULT NULL,
  transitive_object_type_id INT(11) DEFAULT NULL,
  transitive_object_id INT(11) DEFAULT NULL,
  transitive_verb_id INT(11) DEFAULT NULL,
  adverb VARCHAR(255) DEFAULT NULL,
  transitive_object_type_adjective VARCHAR(255) DEFAULT NULL,
  negative TINYINT(1) DEFAULT 0,
  PRIMARY KEY (object_verb_id)
)
ENGINE = INNODB
AUTO_INCREMENT = 2
AVG_ROW_LENGTH = 16384
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_pattern
--
CREATE TABLE osae_pattern (
  pattern_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  pattern VARCHAR(255) NOT NULL,
  PRIMARY KEY (pattern_id),
  UNIQUE INDEX pattern (pattern)
)
ENGINE = INNODB
AUTO_INCREMENT = 40
AVG_ROW_LENGTH = 862
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_script
--
CREATE TABLE osae_script (
  script_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  script TEXT DEFAULT NULL,
  script_processor_id INT(10) UNSIGNED DEFAULT NULL,
  script_name VARCHAR(255) NOT NULL,
  PRIMARY KEY (script_id),
  UNIQUE INDEX script_name (script_name)
)
ENGINE = INNODB
AUTO_INCREMENT = 293
AVG_ROW_LENGTH = 780
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_script_processors
--
CREATE TABLE osae_script_processors (
  script_processor_id INT(11) NOT NULL AUTO_INCREMENT,
  script_processor_name VARCHAR(45) NOT NULL COMMENT 'visual name in UI',
  script_processor_plugin_name VARCHAR(45) NOT NULL COMMENT 'the name of the plugin to process the script',
  PRIMARY KEY (script_processor_id),
  UNIQUE INDEX script_processor_id_UNIQUE (script_processor_id)
)
ENGINE = INNODB
AUTO_INCREMENT = 3
AVG_ROW_LENGTH = 8192
CHARACTER SET latin1
COLLATE latin1_swedish_ci;

--
-- Definition for table osae_verb
--
CREATE TABLE osae_verb (
  verb_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  verb VARCHAR(255) DEFAULT NULL,
  tense VARCHAR(255) DEFAULT NULL,
  transitive TINYINT(4) DEFAULT NULL,
  PRIMARY KEY (verb_id)
)
ENGINE = INNODB
AUTO_INCREMENT = 2
AVG_ROW_LENGTH = 16384
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_type_event
--
CREATE TABLE osae_object_type_event (
  event_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  event_name VARCHAR(200) NOT NULL,
  event_label VARCHAR(200) DEFAULT NULL,
  object_type_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  PRIMARY KEY (event_id),
  UNIQUE INDEX osae_object_type_events_unq (event_name, object_type_id),
  CONSTRAINT osae_fk_events_to_object_type FOREIGN KEY (object_type_id)
    REFERENCES osae_object_type(object_type_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 311
AVG_ROW_LENGTH = 156
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_type_method
--
CREATE TABLE osae_object_type_method (
  method_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  method_name VARCHAR(200) DEFAULT NULL,
  method_label VARCHAR(200) NOT NULL,
  object_type_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  param_1_label VARCHAR(100) DEFAULT NULL,
  param_2_label VARCHAR(200) DEFAULT NULL,
  param_1_default VARCHAR(512) DEFAULT NULL,
  param_2_default VARCHAR(512) DEFAULT NULL,
  PRIMARY KEY (method_id),
  UNIQUE INDEX osae_object_type_methods_unq (method_name, object_type_id),
  CONSTRAINT osae_fk_methods_to_object_type FOREIGN KEY (object_type_id)
    REFERENCES osae_object_type(object_type_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 452
AVG_ROW_LENGTH = 121
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_type_property
--
CREATE TABLE osae_object_type_property (
  property_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  property_name VARCHAR(200) NOT NULL,
  property_datatype VARCHAR(50) NOT NULL,
  property_default VARCHAR(255) DEFAULT NULL,
  object_type_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  track_history TINYINT(1) NOT NULL DEFAULT 1,
  property_object_type_id INT(10) UNSIGNED DEFAULT NULL,
  PRIMARY KEY (property_id),
  UNIQUE INDEX osae_object_type_properties_unq (property_name, object_type_id),
  CONSTRAINT oase_fk_properties_to_object_type FOREIGN KEY (object_type_id)
    REFERENCES osae_object_type(object_type_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 1415
AVG_ROW_LENGTH = 496
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_type_state
--
CREATE TABLE osae_object_type_state (
  state_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  state_name VARCHAR(20) NOT NULL,
  state_label VARCHAR(50) DEFAULT NULL,
  object_type_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  PRIMARY KEY (state_id),
  UNIQUE INDEX osae_object_type_states_unq (state_name, object_type_id),
  CONSTRAINT osae_fk_states_to_object_type FOREIGN KEY (object_type_id)
    REFERENCES osae_object_type(object_type_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 266
AVG_ROW_LENGTH = 172
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_pattern_match
--
CREATE TABLE osae_pattern_match (
  match_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  pattern_id INT(10) UNSIGNED NOT NULL,
  `match` VARCHAR(400) NOT NULL,
  PRIMARY KEY (match_id),
  CONSTRAINT FK_osae_pattern_match_osae_pattern_pattern_id FOREIGN KEY (pattern_id)
    REFERENCES osae_pattern(pattern_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 117
AVG_ROW_LENGTH = 252
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_pattern_script
--
CREATE TABLE osae_pattern_script (
  pattern_script_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  pattern_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  script_id INT(10) UNSIGNED NOT NULL,
  script_sequence INT(11) DEFAULT NULL,
  PRIMARY KEY (pattern_script_id),
  UNIQUE INDEX script_sequence (script_sequence, pattern_id),
  CONSTRAINT FK_osae_pattern_script_osae_pattern_pattern_id FOREIGN KEY (pattern_id)
    REFERENCES osae_pattern(pattern_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT FK_osae_pattern_script_osae_script_script_id FOREIGN KEY (script_id)
    REFERENCES osae_script(script_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 26
AVG_ROW_LENGTH = 2048
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object
--
CREATE TABLE osae_object (
  object_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  object_name VARCHAR(100) NOT NULL,
  object_alias VARCHAR(100) DEFAULT NULL,
  object_description VARCHAR(200) DEFAULT NULL,
  state_id INT(10) UNSIGNED ZEROFILL DEFAULT NULL,
  object_type_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  object_value VARCHAR(50) DEFAULT NULL,
  address VARCHAR(300) DEFAULT NULL,
  container_id INT(10) UNSIGNED ZEROFILL DEFAULT NULL,
  enabled TINYINT(1) UNSIGNED DEFAULT 1,
  last_updated DATETIME DEFAULT CURRENT_TIMESTAMP,
  last_state_change DATETIME DEFAULT CURRENT_TIMESTAMP,
  min_trust_level INT(4) DEFAULT 30,
  PRIMARY KEY (object_id),
  UNIQUE INDEX osae_object_unq (object_name),
  CONSTRAINT osae_fk_objects_to_container FOREIGN KEY (container_id)
    REFERENCES osae_object(object_id) ON DELETE SET NULL ON UPDATE SET NULL,
  CONSTRAINT osae_fk_objects_to_object_types FOREIGN KEY (object_type_id)
    REFERENCES osae_object_type(object_type_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_objects_to_type_state FOREIGN KEY (state_id)
    REFERENCES osae_object_type_state(state_id) ON DELETE SET NULL ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 3223
AVG_ROW_LENGTH = 112
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_type_event_script
--
CREATE TABLE osae_object_type_event_script (
  object_type_event_script_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  object_type_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  event_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  script_id INT(11) DEFAULT NULL,
  script_sequence INT(11) DEFAULT NULL,
  PRIMARY KEY (object_type_event_script_id),
  UNIQUE INDEX script_sequence (script_sequence, object_type_id, event_id),
  CONSTRAINT FK_osae_object_type_event_script_osae_object_type_event_event_id FOREIGN KEY (event_id)
    REFERENCES osae_object_type_event(event_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT FK_osae_object_type_event_script_osae_object_type_object_type_id FOREIGN KEY (object_type_id)
    REFERENCES osae_object_type(object_type_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 8
AVG_ROW_LENGTH = 4096
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_type_property_option
--
CREATE TABLE osae_object_type_property_option (
  option_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  option_name VARCHAR(200) NOT NULL,
  property_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  PRIMARY KEY (option_id),
  UNIQUE INDEX osae_object_type_property_options_unq (option_name, property_id),
  CONSTRAINT oase_fk_options_to_object_type_property FOREIGN KEY (property_id)
    REFERENCES osae_object_type_property(property_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 1
AVG_ROW_LENGTH = 2730
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_event_log
--
CREATE TABLE osae_event_log (
  event_log_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  object_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  event_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  parameter_1 VARCHAR(2000) DEFAULT NULL,
  parameter_2 VARCHAR(2000) DEFAULT NULL,
  log_time TIMESTAMP(2) NOT NULL DEFAULT CURRENT_TIMESTAMP(2) ON UPDATE CURRENT_TIMESTAMP(2),
  from_object_id INT(10) UNSIGNED DEFAULT NULL,
  debug_trace VARCHAR(2000) DEFAULT NULL,
  PRIMARY KEY (event_log_id),
  CONSTRAINT osae_fk_events_log_to_events FOREIGN KEY (event_id)
    REFERENCES osae_object_type_event(event_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_events_log_to_objects FOREIGN KEY (object_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 280
AVG_ROW_LENGTH = 102
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_method_log
--
CREATE TABLE osae_method_log (
  method_log_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  entry_time TIMESTAMP(2) NOT NULL DEFAULT CURRENT_TIMESTAMP(2),
  object_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  method_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  parameter_1 VARCHAR(1024) DEFAULT NULL,
  parameter_2 VARCHAR(1024) DEFAULT NULL,
  from_object_id INT(10) UNSIGNED DEFAULT NULL,
  debug_trace VARCHAR(2000) DEFAULT NULL,
  PRIMARY KEY (method_log_id),
  INDEX osae_method_log_index02 (method_id),
  INDEX osae_method_log_index03 (object_id),
  CONSTRAINT osae_fk_method_log_to_method FOREIGN KEY (method_id)
    REFERENCES osae_object_type_method(method_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_method_log_to_object FOREIGN KEY (object_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 33
AVG_ROW_LENGTH = 144
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_method_queue
--
CREATE TABLE osae_method_queue (
  method_queue_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  entry_time TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  object_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  method_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  parameter_1 VARCHAR(1024) DEFAULT NULL,
  parameter_2 VARCHAR(1024) DEFAULT NULL,
  from_object_id INT(10) UNSIGNED DEFAULT NULL,
  debug_trace VARCHAR(2000) DEFAULT NULL,
  PRIMARY KEY (method_queue_id),
  CONSTRAINT osae_fk_methods_queue_to_methods FOREIGN KEY (method_id)
    REFERENCES osae_object_type_method(method_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_methods_queue_to_objects FOREIGN KEY (object_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 427
AVG_ROW_LENGTH = 16384
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_event_script
--
CREATE TABLE osae_object_event_script (
  event_script_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  object_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  event_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  script_id INT(11) DEFAULT NULL,
  script_sequence INT(11) DEFAULT NULL,
  PRIMARY KEY (event_script_id),
  UNIQUE INDEX script_sequence (script_sequence, object_id, event_id),
  CONSTRAINT FK_osae_object_event_script_osae_object_object_id FOREIGN KEY (object_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT FK_osae_object_event_script_osae_object_type_event_event_id FOREIGN KEY (event_id)
    REFERENCES osae_object_type_event(event_id) ON DELETE RESTRICT ON UPDATE RESTRICT
)
ENGINE = INNODB
AUTO_INCREMENT = 39
AVG_ROW_LENGTH = 819
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_property
--
CREATE TABLE osae_object_property (
  object_property_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  object_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  object_type_property_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  property_value VARCHAR(4000) DEFAULT NULL,
  last_updated DATETIME DEFAULT CURRENT_TIMESTAMP,
  property_image MEDIUMBLOB DEFAULT NULL,
  trust_level INT(10) UNSIGNED DEFAULT 50,
  source_name VARCHAR(50) DEFAULT '',
  interest_level INT(10) UNSIGNED DEFAULT 0,
  PRIMARY KEY (object_property_id),
  UNIQUE INDEX osae_object_property_index_unq (object_id, object_type_property_id),
  CONSTRAINT osae_fk_object_property_to_object FOREIGN KEY (object_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_object_property_to_object_type FOREIGN KEY (object_type_property_id)
    REFERENCES osae_object_type_property(property_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 10216
AVG_ROW_LENGTH = 61
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_state_history
--
CREATE TABLE osae_object_state_history (
  object_id INT(10) UNSIGNED NOT NULL,
  state_id INT(10) UNSIGNED NOT NULL,
  times_this_hour INT(10) UNSIGNED NOT NULL DEFAULT 0,
  times_this_day INT(10) UNSIGNED NOT NULL DEFAULT 0,
  times_this_month INT(10) UNSIGNED NOT NULL DEFAULT 0,
  times_this_year INT(10) UNSIGNED NOT NULL DEFAULT 0,
  times_ever INT(10) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (object_id, state_id),
  CONSTRAINT osae_fk_object_state_to_object FOREIGN KEY (object_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_object_state_to_state FOREIGN KEY (state_id)
    REFERENCES osae_object_type_state(state_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AVG_ROW_LENGTH = 71
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_schedule_queue
--
CREATE TABLE osae_schedule_queue (
  schedule_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  queue_datetime DATETIME DEFAULT NULL,
  object_id INT(10) UNSIGNED DEFAULT NULL,
  method_id INT(10) UNSIGNED DEFAULT NULL,
  parameter_1 VARCHAR(200) DEFAULT NULL,
  parameter_2 VARCHAR(200) DEFAULT NULL,
  recurring_id INT(10) UNSIGNED DEFAULT NULL,
  script_id INT(10) UNSIGNED DEFAULT NULL,
  PRIMARY KEY (schedule_id),
  INDEX schedule_queue_Index02 (method_id),
  INDEX schedule_queue_Index03 (object_id),
  INDEX schedule_queue_Index04 (recurring_id),
  CONSTRAINT osae_fk_schedule_method_to_method_id FOREIGN KEY (method_id)
    REFERENCES osae_object_type_method(method_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_schedule_object_to_object FOREIGN KEY (object_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_schedule_script_to_script_id FOREIGN KEY (script_id)
    REFERENCES osae_script(script_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 55
AVG_ROW_LENGTH = 3276
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_schedule_recurring
--
CREATE TABLE osae_schedule_recurring (
  recurring_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  object_id INT(10) UNSIGNED DEFAULT NULL,
  method_id INT(10) UNSIGNED DEFAULT NULL,
  parameter_1 VARCHAR(200) DEFAULT NULL,
  parameter_2 VARCHAR(200) DEFAULT NULL,
  recurring_time TIME NOT NULL DEFAULT '00:00:01',
  monday TINYINT(1) UNSIGNED DEFAULT NULL,
  tuesday TINYINT(1) UNSIGNED DEFAULT NULL,
  wednesday TINYINT(1) UNSIGNED DEFAULT NULL,
  thursday TINYINT(1) UNSIGNED DEFAULT NULL,
  friday TINYINT(1) UNSIGNED DEFAULT NULL,
  saturday TINYINT(1) UNSIGNED DEFAULT NULL,
  sunday TINYINT(1) UNSIGNED DEFAULT NULL,
  interval_unit CHAR(1) NOT NULL DEFAULT 'W',
  recurring_minutes INT(8) UNSIGNED DEFAULT NULL,
  recurring_day INT(4) UNSIGNED DEFAULT NULL,
  recurring_date DATE DEFAULT NULL,
  script_id INT(10) UNSIGNED DEFAULT NULL,
  schedule_name VARCHAR(100) DEFAULT NULL,
  active TINYINT(4) DEFAULT 1,
  PRIMARY KEY (recurring_id),
  UNIQUE INDEX osae_schedule_recurring_unq (schedule_name),
  CONSTRAINT osa_fk_recurring_object_to_object_id FOREIGN KEY (object_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_recurring_method_to_method_id FOREIGN KEY (method_id)
    REFERENCES osae_object_type_method(method_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_recurring_script_to_script_id FOREIGN KEY (script_id)
    REFERENCES osae_script(script_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 6
AVG_ROW_LENGTH = 4096
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_screen_object
--
CREATE TABLE osae_screen_object (
  screen_object_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  screen_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  object_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  control_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  PRIMARY KEY (screen_object_id),
  INDEX osae_fk_screen_object_to_object_type (control_id),
  UNIQUE INDEX UK_osae_screen_object (screen_id, control_id),
  CONSTRAINT osae_fk_screen_control_to_object FOREIGN KEY (control_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_screen_object_to_object FOREIGN KEY (object_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT osae_fk_screen_screen_to_object FOREIGN KEY (screen_id)
    REFERENCES osae_object(object_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 51
AVG_ROW_LENGTH = 910
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_property_array
--
CREATE TABLE osae_object_property_array (
  property_array_id INT(10) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  object_property_id INT(10) UNSIGNED ZEROFILL NOT NULL,
  item_name VARCHAR(2000) NOT NULL,
  item_label VARCHAR(400) DEFAULT NULL,
  PRIMARY KEY (property_array_id),
  CONSTRAINT osae_fk_property_array_to_object_property FOREIGN KEY (object_property_id)
    REFERENCES osae_object_property(object_property_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 1565
AVG_ROW_LENGTH = 114
CHARACTER SET utf8
COLLATE utf8_general_ci;

--
-- Definition for table osae_object_property_history
--
CREATE TABLE osae_object_property_history (
  history_id INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  history_timestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  object_property_id INT(10) UNSIGNED NOT NULL,
  property_value VARCHAR(4000) NOT NULL,
  PRIMARY KEY (history_id),
  CONSTRAINT osae_fk_object_property_history_to_object_property FOREIGN KEY (object_property_id)
    REFERENCES osae_object_property(object_property_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AUTO_INCREMENT = 4825
AVG_ROW_LENGTH = 287
CHARACTER SET utf8
COLLATE utf8_general_ci;

DELIMITER $$

--
-- Definition for procedure osae_sp_ai_get_question
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_ai_get_question()
BEGIN
  SELECT
    osae_v_interests.Question,
    osae_v_interests.object_name,
    osae_v_interests.property_name,
    osae_v_interests.property_datatype,
    osae_v_interests.property_object_type,
    osae_v_interests.interest_level
  FROM osae_v_interests
  ORDER BY osae_v_interests.interest_level DESC
  LIMIT 1;
END
$$

--
-- Definition for procedure osae_sp_debug_log_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_debug_log_add(IN pentry varchar(200), IN pdebugtrace varchar(200))
BEGIN
  IF ISNULL(pentry) = FALSE THEN
    INSERT INTO osae_debug_log(entry,debug_trace) VALUES(pentry,pdebugtrace);
  END IF;
END
$$

--
-- Definition for procedure osae_sp_event_log_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_event_log_add(IN pobject VARCHAR(200), IN pevent VARCHAR(200), IN pfromobject VARCHAR(200), IN pdebuginfo VARCHAR(1000), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000))
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
-- Definition for procedure osae_sp_event_log_clear
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_event_log_clear()
BEGIN
    DELETE FROM osae_event_log;
    DELETE FROM osae_debug_log; 
    DELETE FROM osae_method_log;        
END
$$

--
-- Definition for procedure osae_sp_image_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_image_add(IN pimage_data LONGBLOB, IN pimage_name VARCHAR(45), IN pimage_type VARCHAR(4), IN pimage_width INT, IN pimage_height INT, IN pimage_dpi INT)
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
-- Definition for procedure osae_sp_image_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_image_delete(
IN pimage_id INT
)
BEGIN
	DELETE FROM `osae`.`osae_images`
	WHERE image_id = pimage_id;
END
$$

--
-- Definition for procedure osae_sp_method_queue_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_method_queue_add(IN pobject varchar(200), IN pmethod varchar(200), IN pparameter1 varchar(1024), IN pparameter2 varchar(1024), IN pfromobject varchar(200), IN pdebuginfo varchar(1000))
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
-- Definition for procedure osae_sp_method_queue_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_method_queue_delete(
  IN pmethodid int
)
BEGIN
    DELETE FROM osae_method_queue WHERE method_queue_id=pmethodid;
END
$$

--
-- Definition for procedure osae_sp_object_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_add(IN pname VARCHAR(100), IN palias VARCHAR(100), IN pdescription VARCHAR(200), IN pobjecttype VARCHAR(200), IN paddress VARCHAR(200), IN pcontainer VARCHAR(200), IN pmintrustlevel INT, IN penabled TINYINT(1), OUT results INTEGER)
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
-- Definition for procedure osae_sp_object_container_check
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_container_check(IN pObject VARCHAR(255), IN pContainer VARCHAR(255))
BEGIN
DECLARE vObjectName VARCHAR(200);
DECLARE iMatch int;
  SET vObjectName=pObject;
  #Check to see if the Object is Directly assigned to the container.
  SET iMatch = (SELECT COUNT(object_name) FROM osae_v_object WHERE (UPPER(object_name)=UPPER(pObject) OR UPPER(object_alias)=UPPER(pObject)) AND UPPER(container_name) = UPPER(pContainer));
  IF iMatch = 0 THEN
    label1: LOOP
      SET vObjectName = (SELECT container_name FROM osae_v_object WHERE (UPPER(object_name)=UPPER(vObjectName) OR UPPER(object_alias)=UPPER(vObjectName)) AND UPPER(container_name)!=UPPER(vObjectName));
      IF vObjectName IS NULL OR vObjectName = '' THEN
        LEAVE Label1;
      END IF;
      #SELECT vObjectName;
      SET iMatch = (SELECT COUNT(object_name) FROM osae_v_object WHERE (UPPER(object_name)=UPPER(vObjectName) OR UPPER(object_alias)=UPPER(vObjectName)) AND UPPER(container_name) = UPPER(pContainer));
      IF iMatch = 0 THEN
        ITERATE label1;
      END IF;
      LEAVE label1;
    END LOOP label1;
  END IF;
  SELECT iMatch;

END
$$

--
-- Definition for procedure osae_sp_object_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_delete(
  IN pname varchar(200)
)
BEGIN
  DELETE FROM osae_object WHERE UPPER(object_name)=UPPER(pname) OR UPPER(object_alias)=UPPER(pname) ;
END
$$

--
-- Definition for procedure osae_sp_object_event_script_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_event_script_add(IN pobject VARCHAR(200), IN pevent VARCHAR(200), IN pscriptid INT)
BEGIN
DECLARE vObjectCount INT;
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vEventCount INT;
DECLARE vEventID INT;
DECLARE vScriptSeq INT;
SET vScriptSeq = 0;
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject);
    IF vObjectCount > 0 THEN
        SELECT object_id,object_type_id INTO vObjectID,vObjectTypeID FROM osae_object WHERE UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject);
        SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND (event_name=pevent OR event_label=pevent);
        IF vEventCount = 1 THEN       
            SELECT event_id INTO vEventID FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND (event_name=pevent OR event_label=pevent);
            SELECT COALESCE(script_sequence, 0) INTO vScriptSeq FROM osae_object_event_script where object_id = vObjectID AND event_id = vEventID ORDER BY script_sequence DESC LIMIT 1;
            INSERT INTO osae_object_event_script (object_id,event_id,script_id, script_sequence) VALUES(vObjectID,vEventID,pscriptid,vScriptSeq+1);
        END IF;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_event_script_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_event_script_delete(IN peventscriptid INT)
BEGIN
  DELETE FROM osae_object_event_script
    WHERE event_script_id = peventscriptid;
END
$$

--
-- Definition for procedure osae_sp_object_export
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_export(IN objectName VARCHAR(255))
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
-- Definition for procedure osae_sp_object_export_all
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_export_all()
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
-- Definition for procedure osae_sp_object_history_clear
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_history_clear()
BEGIN
DELETE FROM osae_object_state_history;
DELETE FROM osae_object_state_change_history;
DELETE FROM osae_object_property_history;
END
$$

--
-- Definition for procedure osae_sp_object_property_array_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_array_add(
  IN  pobject    varchar(200),
  IN  pproperty  varchar(200),
  IN  pvalue     varchar(1000),
  IN  plabel     varchar(200)
)
BEGIN
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vObjectPropertyID INT;
    SELECT object_property_id INTO vObjectPropertyID FROM osae_v_object_property WHERE (UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject)) AND UPPER(property_name)=UPPER(pproperty);
    IF vObjectPropertyID IS NOT NULL Then
        INSERT INTO osae_object_property_array (object_property_id,item_name,item_label) VALUES(vObjectPropertyID,pvalue,plabel);
    END IF;
END
$$

--
-- Definition for procedure osae_sp_object_property_array_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_array_delete(
  IN  pobject    varchar(400),
  IN  pproperty  varchar(400),
  IN  pvalue     varchar(2000)
)
BEGIN
  DECLARE vPropertyArrayID INT;
  SELECT property_array_id INTO vPropertyArrayID FROM osae_v_object_property_array WHERE object_name=pobject AND property_name=pproperty AND item_name=pvalue LIMIT 1;
  
  DELETE FROM osae_object_property_array WHERE property_array_id=vPropertyArrayID;
END
$$

--
-- Definition for procedure osae_sp_object_property_array_delete_all
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_array_delete_all(
  IN  pobject    varchar(400),
  IN  pproperty  varchar(400)
)
BEGIN
  DECLARE vPropertyID INT;
  SELECT object_property_id INTO vPropertyID FROM osae_v_object_property WHERE object_name=pobject AND property_name=pproperty;
  DELETE FROM osae_object_property_array WHERE object_property_id=vPropertyID;
END
$$

--
-- Definition for procedure osae_sp_object_property_array_get_all
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_array_get_all(IN pobject VARCHAR(255), IN pproperty VARCHAR(255))
BEGIN
  SELECT item_name FROM osae_v_object_property_array WHERE (UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject)) AND UPPER(property_name)=UPPER(pproperty);
END
$$

--
-- Definition for procedure osae_sp_object_property_array_get_random
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_array_get_random(
  IN  pobject    varchar(200),
  IN  pproperty  varchar(200)
)
BEGIN
SELECT item_name FROM osae_v_object_property_array WHERE (UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject)) AND UPPER(property_name)=UPPER(pproperty) ORDER BY RAND() LIMIT 1;
END
$$

--
-- Definition for procedure osae_sp_object_property_array_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_array_update(
  IN  pobject    varchar(200),
  IN  pproperty  varchar(200),
  IN  poldvalue  varchar(2000),
  IN  pnewvalue  varchar(2000),
  IN  pnewlabel  varchar(200)
)
BEGIN
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vObjectPropertyID INT;
    SELECT object_property_id INTO vObjectPropertyID FROM osae_v_object_property WHERE (UPPER(object_name)=UPPER(pobject) OR UPPER(object_name)=UPPER(pobject)) AND UPPER(property_name)=UPPER(pproperty);
    IF vObjectPropertyID IS NOT NULL Then
        UPDATE osae_object_property_array SET item_name=pnewvalue,item_label=pnewlabel WHERE object_property_id=vObjectPropertyID AND item_name=poldvalue;
    END IF;
END
$$

--
-- Definition for procedure osae_sp_object_property_get
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_get(
  IN  pname      varchar(200),
  IN  pproperty  varchar(200)
)
BEGIN
  SELECT property_value FROM osae_v_object_property WHERE (UPPER(object_name)=UPPER(pname) OR UPPER(object_alias)=UPPER(pname)) AND UPPER(property_name)=UPPER(pproperty);
END
$$

--
-- Definition for procedure osae_sp_object_property_scraper_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_scraper_add(IN pobject varchar(200), IN pproperty varchar(200), IN pURL varchar(4000), IN pprefix varchar(255), IN pprefixoffset int(11), IN psuffix varchar(255), IN pupdateinterval time)
BEGIN
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vObjectPropertyID INT;
    SELECT object_property_id INTO vObjectPropertyID FROM osae_v_object_property WHERE (UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject)) AND UPPER(property_name)=UPPER(pproperty);
    IF vObjectPropertyID IS NOT NULL Then
        INSERT INTO osae_object_property_scraper (object_property_id,URL,search_prefix,search_prefix_offset,search_suffix,update_interval) VALUES(vObjectPropertyID,pURL,pprefix,pprefixoffset,psuffix,pupdateinterval);
    END IF;
END
$$

--
-- Definition for procedure osae_sp_object_property_scraper_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_scraper_delete(IN pscraperid int(11))
BEGIN
    DELETE FROM osae_object_property_scraper WHERE object_property_scraper_id=pscraperid;
END
$$

--
-- Definition for procedure osae_sp_object_property_scraper_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_scraper_update(IN pscraperid int(11), IN pobject varchar(200), IN pproperty varchar(200), IN pURL varchar(4000), IN pprefix varchar(255), IN pprefixoffset int(11), IN psuffix varchar(255), IN pupdateinterval time)
BEGIN
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vObjectPropertyID INT;
    SELECT object_property_id INTO vObjectPropertyID FROM osae_v_object_property WHERE (UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject)) AND UPPER(property_name)=UPPER(pproperty);
    IF vObjectPropertyID IS NOT NULL Then
        UPDATE osae_object_property_scraper SET object_property_id=vObjectPropertyID,URL=pURL,search_prefix=pprefix,search_prefix_offset=pprefixoffset,search_suffix=psuffix,update_interval=pupdateinterval WHERE object_property_scraper_id=pscraperid;
    END IF;
END
$$

--
-- Definition for procedure osae_sp_object_property_set
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_property_set(IN pname varchar(200), IN pproperty varchar(200), IN pvalue varchar(4000), IN pfromobject varchar(200), IN pdebuginfo varchar(2000))
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
-- Definition for procedure osae_sp_object_state_set
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_state_set(IN pname varchar(200), IN pstate varchar(50), IN pfromobject varchar(200), IN pdebuginfo varchar(2000))
BEGIN
DECLARE vObjectCount INT;
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vStateCount INT;
DECLARE vOldStateID INT;
DECLARE vStateID INT;
DECLARE vEventCount INT;
DECLARE vHideRedundantEvents INT;
DECLARE vDebugTrace VARCHAR(2000) DEFAULT '';
    SET vDebugTrace = CONCAT(pdebuginfo,' -> osae_sp_object_state_set');
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(object_name)=UPPER(pname) OR UPPER(object_alias)=UPPER(pname);
    IF vObjectCount = 1 THEN
        SELECT object_id,object_type_id,state_id INTO vObjectID,vObjectTypeID,vOldStateID FROM osae_object WHERE UPPER(object_name)=UPPER(pname) OR UPPER(object_alias)=UPPER(pname) LIMIT 1;
        SELECT COUNT(state_id) INTO vStateCount FROM osae_object_type_state WHERE object_type_id=vObjectTypeID AND (UPPER(state_name)=UPPER(pstate) OR UPPER(state_label)=UPPER(pstate));
        IF vStateCount = 1 THEN       
            SELECT state_id INTO vStateID FROM osae_object_type_state WHERE object_type_id=vObjectTypeID AND (UPPER(state_name)=UPPER(pstate) OR UPPER(state_label)=UPPER(pstate)) LIMIT 1;
            UPDATE osae_object SET state_id=vStateID,last_updated=NOW() WHERE object_id=vObjectID;
            SELECT COUNT(event_id) INTO vEventCount FROM osae_v_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pstate) LIMIT 1;
            IF vEventCount = 1 THEN
                SELECT hide_redundant_events INTO vHideRedundantEvents FROM osae_v_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pstate) LIMIT 1;
                IF vOldStateID <> vStateID OR vHideRedundantEvents = 0 Then
                    CALL osae_sp_event_log_add(pname,pstate,pfromobject,vDebugTrace,NULL,NULL);
                END IF;
            END IF;  
        END IF;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_state_set_by_address
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_state_set_by_address(IN paddress varchar(400), IN pstate varchar(50), IN pfromobject varchar(200), IN pdebuginfo varchar(2000))
BEGIN
DECLARE vObjectCount INT;
DECLARE vObjectID INT;
DECLARE vObjectTypeID INT;
DECLARE vStateCount INT;
DECLARE vOldStateID INT;
DECLARE vStateID INT;
DECLARE vEventCount INT;
DECLARE vHideRedundantEvents INT;
DECLARE vDebugTrace VARCHAR(2000) DEFAULT '';
    SET vDebugTrace = CONCAT(pdebuginfo,' -> osae_sp_object_state_set');
    SELECT COUNT(object_id) INTO vObjectCount FROM osae_object WHERE UPPER(address)=UPPER(paddress);
    IF vObjectCount = 1 THEN
        SELECT object_id,object_type_id,state_id INTO vObjectID,vObjectTypeID,vOldStateID FROM osae_object WHERE UPPER(address)=UPPER(paddress) AND paddress != '' LIMIT 1;
        SELECT COUNT(state_id) INTO vStateCount FROM osae_object_type_state WHERE object_type_id=vObjectTypeID AND (UPPER(state_name)=UPPER(pstate) OR UPPER(state_label)=UPPER(pstate));
        IF vStateCount = 1 THEN       
            SELECT state_id INTO vStateID FROM osae_object_type_state WHERE object_type_id=vObjectTypeID AND (UPPER(state_name)=UPPER(pstate) OR UPPER(state_label)=UPPER(pstate)) LIMIT 1;
            UPDATE osae_object SET state_id=vStateID,last_updated=NOW() WHERE object_id=vObjectID;
            SELECT COUNT(event_id),hide_redundant_events INTO vEventCount,vHideRedundantEvents FROM osae_v_object_type_event WHERE object_type_id=vObjectTypeID AND UPPER(event_name)=UPPER(pstate) LIMIT 1;
            IF vOldStateID <> vStateID OR vHideRedundantEvents = 0 Then
                IF vEventCount = 1 THEN  
                    CALL osae_sp_event_log_add(pname,pstate,pfromobject,vDebugTrace,NULL,NULL);
                END IF;
            END IF;  
        END IF;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_test
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_test()
BEGIN
  DECLARE vResults INT;
  DECLARE vCount INT;
  DECLARE SResults Text;

  DELETE FROM osae_object WHERE object_name='TEST OBJECT';
  # OBJECT ADD
  CALL osae_sp_object_add('TEST OBJECT','Test Alias','Test Description','THING','Test Address','SYSTEM',1,vResults);
  SELECT Count(object_name) INTO vCount FROM osae_object WHERE object_name='TEST OBJECT';
  SET SResults = CONCAT('Test Object Add results: ', vResults, ' and I count ',vCount,' test objects','\n');

  # OBJECT UPDATE
  CALL osae_sp_object_update('TEST OBJECT','TEST OBJECT1','Test Alias1','Test Description1','THING','Test Address1','SYSTEM',1);
  SELECT Count(object_name) INTO vCount FROM osae_object WHERE object_name='TEST OBJECT1' AND object_description = 'Test Description1' AND object_alias = 'Test Alias1';
  SET SResults = CONCAT(SResults,'Test Object Update results:  I count ',vCount,' good test objects','\n');

  # OBJECT DELETE
  CALL osae_sp_object_delete('TEST OBJECT1');
  SELECT Count(object_name) INTO vCount FROM osae_object WHERE object_name='TEST OBJECT1';
  SET SResults = CONCAT(SResults,'Test Object Delete results:  I count ',vCount,' test objects','\n');

  SELECT SResults;
END
$$

--
-- Definition for procedure osae_sp_object_type_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_add(
  IN  pname                 varchar(200),
  IN  pdesc                 varchar(200),
  IN  pownedby              varchar(200),
  IN  pbasetype             varchar(200),
  IN  ptypeowner            tinyint,
  IN  psystem               tinyint,
  IN  pcontainer            tinyint,
  IN  phideredundantevents  tinyint
)
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
    INSERT INTO osae_object_type (object_type,object_type_description,plugin_object_id,base_type_id,system_hidden,object_type_owner,container,hide_redundant_events) VALUES(UPPER(pname),pdesc,vOwnerTypeID,vBaseTypeID,psystem,ptypeowner,pcontainer,phideredundantevents) ON DUPLICATE KEY UPDATE object_type_description=pdesc,plugin_object_id=vOwnerTypeID,base_type_id=vBaseTypeID,system_hidden=psystem,object_type_owner=ptypeowner,container=pcontainer,hide_redundant_events=phideredundantevents;
    IF vBaseTypeCount = 0 THEN
        SELECT object_type_id INTO vBaseTypeID FROM osae_object_type WHERE object_type=UPPER(pname);
        UPDATE osae_object_type SET base_type_id=vBaseTypeID WHERE object_type_id=vBaseTypeID;
    END IF;
END
$$

--
-- Definition for procedure osae_sp_object_type_clone
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_clone(IN pnewname varchar(200), IN pbasename varchar(200))
BEGIN
DECLARE vBaseTypeID INT DEFAULT 0; 
DECLARE vNewTypeID INT;
    SELECT object_type_id INTO vBaseTypeID FROM osae_v_object_type WHERE object_type=pbasename;
    IF vBaseTypeID != 0 THEN
      # CALL OBJECT_TYPE_DOES_NOT_EXIST();
   # ELSE
      INSERT INTO osae_object_type (object_type,object_type_description,plugin_object_id,system_hidden,object_type_owner,base_type_id,container) SELECT pnewname,t.object_type_description,t.plugin_object_id,t.system_hidden,t.object_type_owner,t.base_type_id,t.container FROM osae_object_type t WHERE object_type=pbasename;
      SELECT object_type_id INTO vNewTypeID FROM osae_object_type WHERE object_type=pnewname;
      INSERT INTO osae_object_type_state (state_name,state_label,object_type_id) SELECT state_name,state_label,vNewTypeID FROM osae_object_type_state t WHERE object_type_id=vBaseTypeID;
      INSERT INTO osae_object_type_event (event_name,event_label,object_type_id) SELECT event_name,event_label,vNewTypeID FROM osae_object_type_event t WHERE object_type_id=vBaseTypeID;
      INSERT INTO osae_object_type_method (method_name,method_label,object_type_id) SELECT method_name,method_label,vNewTypeID FROM osae_object_type_method t WHERE object_type_id=vBaseTypeID;
      INSERT INTO osae_object_type_property (property_name,property_datatype,object_type_id) SELECT property_name,property_datatype,vNewTypeID FROM osae_object_type_property t WHERE object_type_id=vBaseTypeID;
    END IF;
END
$$

--
-- Definition for procedure osae_sp_object_type_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_delete(
  IN pname varchar(200)
)
BEGIN
  DELETE FROM osae_object_type WHERE object_type=pname;
END
$$

--
-- Definition for procedure osae_sp_object_type_event_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_event_add(IN pobjecttype VARCHAR(200), IN pname VARCHAR(200), IN plabel VARCHAR(200))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        INSERT INTO osae_object_type_event (event_name,event_label,object_type_id) VALUES(UPPER(pname),plabel,vObjectTypeID) ON DUPLICATE KEY UPDATE event_label=plabel,object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_event_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_event_delete(
  IN  pname        varchar(200),
  IN  pobjecttype  varchar(200)
)
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        DELETE FROM osae_object_type_event WHERE event_name=pname AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_event_script_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_event_script_add(IN pobjtypename VARCHAR(255), IN pevent VARCHAR(255), IN pscriptid INT)
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
DECLARE vEventCount INT;
DECLARE vEventID INT;
DECLARE vScriptSeq INT;
SET vScriptSeq = 0;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjtypename;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjtypename;
        SELECT COUNT(event_id) INTO vEventCount FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND (event_name=pevent OR event_label=pevent);
        IF vEventCount = 1 THEN       
            SELECT event_id INTO vEventID FROM osae_object_type_event WHERE object_type_id=vObjectTypeID AND (event_name=pevent OR event_label=pevent);
            SELECT COALESCE(script_sequence, 0) INTO vScriptSeq FROM osae_object_type_event_script where object_type_id = vObjectTypeID AND event_id = vEventID ORDER BY script_sequence DESC LIMIT 1;
            INSERT INTO osae_object_type_event_script (object_type_id,event_id,script_id, script_sequence) VALUES(vObjectTypeID,vEventID,pscriptid,vScriptSeq+1);
        END IF;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_event_script_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_event_script_delete(IN pobjtypeeventscriptid INT)
BEGIN
  DELETE FROM osae_object_type_event_script
    WHERE object_type_event_script_id = pobjtypeeventscriptid;
END
$$

--
-- Definition for procedure osae_sp_object_type_event_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_event_update(
  IN  poldname     varchar(200),
  IN  pnewname     varchar(200),
  IN  plabel       varchar(200),
  IN  pobjecttype  varchar(200)
)
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        UPDATE osae_object_type_event SET event_name=UPPER(pnewname),event_label=plabel WHERE event_name=UPPER(poldname) AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_export
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_export(IN pObjectType VARCHAR(255))
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
-- Definition for procedure osae_sp_object_type_method_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_method_add(IN pobjecttype VARCHAR(200), IN pname VARCHAR(200), IN plabel VARCHAR(200), IN pparam1 VARCHAR(100), IN pparam2 VARCHAR(100), IN pparam1default VARCHAR(1024), IN pparam2default VARCHAR(1024))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        INSERT INTO osae_object_type_method (method_name,method_label,object_type_id,param_1_label,param_2_label,param_1_default,param_2_default) VALUES(UPPER(pname),plabel,vObjectTypeID,pparam1,pparam2,pparam1default,pparam2default) ON DUPLICATE KEY UPDATE method_label=plabel,object_type_id=vObjectTypeID,param_1_label=pparam1,param_2_label=pparam2,param_1_default=pparam1default,param_2_default=pparam2default;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_method_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_method_delete(
  IN  pname        varchar(200),
  IN  pobjecttype  varchar(200)
)
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        DELETE FROM osae_object_type_method WHERE method_name=pname AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_method_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_method_update(
  IN  poldname        varchar(200),
  IN  pnewname        varchar(200),
  IN  plabel          varchar(200),
  IN  pobjecttype     varchar(200),
  IN  pparam1         varchar(100),
  IN  pparam2         varchar(100),
  IN  pparam1default  varchar(1024),
  IN  pparam2default  varchar(1024)
)
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount = 1 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        UPDATE osae_object_type_method SET method_name=UPPER(pnewname),method_label=plabel,param_1_label=pparam1,param_2_label=pparam2,param_1_default=pparam1default,param_2_default=pparam2default WHERE method_name=UPPER(poldname) AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_property_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_property_add(
  IN  pobjecttype    varchar(200),
  IN  ppropertyname          varchar(200),
  IN  ppropertytype     varchar(50),
  IN  ppropertyobjecttype     varchar(200),
  IN  pdefault       varchar(255),
  IN  ptrackhistory  tinyint(1)
)
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
        INSERT INTO osae_object_type_property (property_name,property_datatype,property_object_type_id,property_default,object_type_id,track_history) VALUES(ppropertyname,ppropertytype,vPropertyObjectTypeID,pdefault,vObjectTypeID,ptrackhistory) ON DUPLICATE KEY UPDATE property_datatype=ppropertytype,object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_property_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_property_delete(
  IN  pname        varchar(200),
  IN  pobjecttype  varchar(200)
)
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        DELETE FROM osae_object_type_property WHERE property_name=pname AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_property_option_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_property_option_add(IN pobjecttype VARCHAR(200),
                                                  IN pproperty   VARCHAR(200),
                                                  IN pvalue      VARCHAR(200)
                                                  )
BEGIN
  DECLARE vObjectTypePropertyID INT;

  SELECT property_id
  INTO
    vObjectTypePropertyID
  FROM
    osae_v_object_type_property
  WHERE
    upper(object_type) = upper(pobjecttype)
    AND upper(property_name) = upper(pproperty);
  IF vObjectTypePropertyID IS NOT NULL THEN
  INSERT INTO osae_object_type_property_option (option_name, property_id) VALUES (pvalue, vObjectTypePropertyID);
END IF;
END
$$

--
-- Definition for procedure osae_sp_object_type_property_option_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_property_option_delete(IN pobjecttype VARCHAR(200),
                                                     IN pproperty   VARCHAR(200),
                                                     IN pvalue      VARCHAR(200)
                                                     )
BEGIN
  DECLARE vOptionID INT;

  SELECT option_id
  INTO
    vOptionID
  FROM
    osae_v_object_type_property_option
  WHERE
    upper(object_type) = upper(pobjecttype)
    AND upper(property_name) = upper(pproperty)
    AND upper(option_name) = upper(pvalue);
  IF vOptionID IS NOT NULL THEN
  DELETE
FROM
  osae_object_type_property_option
WHERE
  option_id = vOptionID;
END IF;
END
$$

--
-- Definition for procedure osae_sp_object_type_property_option_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_property_option_update(IN pobjecttype VARCHAR(200),
                                                     IN pproperty   VARCHAR(200),
                                                     IN pnewvalue   VARCHAR(200),
                                                     IN poldvalue   VARCHAR(200)
                                                     )
BEGIN
  DECLARE vOptionID INT;

  SELECT option_id
  INTO
    vOptionID
  FROM
    osae_v_object_type_property_option
  WHERE
    upper(object_type) = upper(pobjecttype)
    AND upper(property_name) = upper(pproperty)
    AND upper(option_name) = upper(poldvalue);
  IF vOptionID IS NOT NULL THEN
  UPDATE osae_object_type_property_option
SET
  option_name = pnewvalue
WHERE
  option_id = vOptionID;
END IF;
END
$$

--
-- Definition for procedure osae_sp_object_type_property_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_property_update(IN poldname VARCHAR(200), IN pnewname VARCHAR(200), IN pparamtype VARCHAR(50), IN ppropertyobjecttype VARCHAR(200), IN pdefault VARCHAR(255), IN pobjecttype VARCHAR(200), IN ptrackhistory TINYINT(1))
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
        UPDATE osae_object_type_property SET property_name=pnewname,property_datatype=pparamtype,property_object_type_id=vPropertyObjectTypeID,property_default=pdefault,track_history=ptrackhistory WHERE property_name=poldname AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_state_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_state_add(IN pobjecttype VARCHAR(200), IN pname VARCHAR(200), IN plabel VARCHAR(200))
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        INSERT INTO osae_object_type_state (state_name,state_label,object_type_id) VALUES(UPPER(pname),plabel,vObjectTypeID) ON DUPLICATE KEY UPDATE state_label=plabel,object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_state_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_state_delete(
  IN  pname        varchar(200),
  IN  pobjecttype  varchar(200)
)
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        DELETE FROM osae_object_type_state WHERE state_name=pname AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_state_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_state_update(
  IN  poldname     varchar(200),
  IN  pnewname     varchar(200),
  IN  plabel       varchar(200),
  IN  pobjecttype  varchar(200)
)
BEGIN
DECLARE vObjectTypeCount INT;
DECLARE vObjectTypeID INT;
    SELECT COUNT(object_type_id) INTO vObjectTypeCount FROM osae_object_type WHERE object_type=pobjecttype;
    IF vObjectTypeCount > 0 THEN
        SELECT object_type_id INTO vObjectTypeID FROM osae_object_type WHERE object_type=pobjecttype;
        UPDATE osae_object_type_state SET state_name=UPPER(pnewname),state_label=plabel WHERE state_name=UPPER(poldname) AND object_type_id=vObjectTypeID;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_object_type_test
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_test()
BEGIN
  DECLARE vResults INT;
  DECLARE vCount INT;
  DECLARE SResults Text;

  DELETE FROM osae_object_type WHERE object_type='TEST TYPE';
  # OBJECT ADD
  CALL osae_sp_object_type_add('TEST OBJECT','Test Alias','Test Description','THING','Test Address','SYSTEM',1,vResults);
  SELECT Count(object_type) INTO vCount FROM osae_object_type WHERE object_type='TEST OBJECT';
  SET SResults = CONCAT('Test Object Add results: ', vResults, ' and I count ',vCount,' test objects','\n');

  # OBJECT UPDATE
  CALL osae_sp_object_type_update('TEST OBJECT','TEST OBJECT1','Test Alias1','Test Description1','THING','Test Address1','SYSTEM',1);
  SELECT Count(object_type) INTO vCount FROM osae_object_type WHERE object_type='TEST OBJECT1' AND object_description = 'Test Description1' AND object_alias = 'Test Alias1';
  SET SResults = CONCAT(SResults,'Test Object Update results:  I count ',vCount,' good test objects','\n');

  # OBJECT DELETE
  CALL osae_sp_object_delete('TEST OBJECT1');
  SELECT Count(object_name) INTO vCount FROM osae_object WHERE object_name='TEST OBJECT1';
  SET SResults = CONCAT(SResults,'Test Object Delete results:  I count ',vCount,' test objects','\n');

  SELECT SResults;
END
$$

--
-- Definition for procedure osae_sp_object_type_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_type_update(
  IN  poldname              varchar(200),
  IN  pnewname              varchar(200),
  IN  pdesc                 varchar(200),
  IN  pownedby              varchar(200),
  IN  pbasetype             varchar(200),
  IN  ptypeowner            tinyint,
  IN  psystem               tinyint,
  IN  pcontainer            tinyint,
  IN  phideredundantevents  tinyint
)
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
    UPDATE osae_object_type SET object_type=UPPER(pnewname),object_type_description=pdesc,plugin_object_id=vOwnerTypeID,base_type_id=vBaseTypeID,system_hidden=psystem,object_type_owner=ptypeowner,container=pcontainer,hide_redundant_events=phideredundantevents WHERE object_type=poldname;
END
$$

--
-- Definition for procedure osae_sp_object_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_update(IN poldname VARCHAR(100), IN pnewname VARCHAR(100), IN palias VARCHAR(100), IN pdesc VARCHAR(200), IN pobjecttype VARCHAR(200), IN paddress VARCHAR(200), IN pcontainer VARCHAR(200), IN pmintrustlevel INT, IN penabled TINYINT)
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
-- Definition for procedure osae_sp_object_verb
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_object_verb(IN pobject VARCHAR(255), IN pverb VARCHAR(255), IN ptransitiveobject VARCHAR(255), IN pnegative BIT)
BEGIN

END
$$

--
-- Definition for procedure osae_sp_pattern_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_add(
  IN pname varchar(400)
)
BEGIN
  INSERT INTO osae_pattern (pattern) VALUES (pname);
END
$$

--
-- Definition for procedure osae_sp_pattern_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_delete(
  IN pname varchar(100)
)
BEGIN
  DELETE FROM osae_pattern WHERE pattern=pname;
END
$$

--
-- Definition for procedure osae_sp_pattern_export
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_export(IN patternName VARCHAR(255))
BEGIN
  DECLARE v_finished BOOL; 
  DECLARE vResults TEXT;
  DECLARE vMatch VARCHAR(2000);
  DECLARE vScriptName VARCHAR(255);
  DECLARE match_cursor CURSOR FOR SELECT `match` FROM osae_v_pattern_match WHERE pattern=patternName;
  DECLARE script_cursor CURSOR FOR SELECT script_name FROM osae_v_pattern_script WHERE pattern=patternName;
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_finished = TRUE;

  SET vResults = CONCAT('CALL osae_sp_pattern_add (\'', patternName,'\');','\r\n');

  OPEN match_cursor;
  get_matches: LOOP
    SET v_finished = FALSE;
    FETCH match_cursor INTO vMatch;
    IF v_finished THEN 
      LEAVE get_matches;
    END IF;
    SET vResults = CONCAT(vResults,'CALL osae_sp_pattern_match_add(\'',patternName,'\',\'',vMatch,'\');','\r\n');
  END LOOP get_matches;
  CLOSE match_cursor;

  OPEN script_cursor;
  get_scripts: LOOP
    SET v_finished = FALSE;
    FETCH script_cursor INTO vScriptName;
    IF v_finished THEN 
      LEAVE get_scripts;
    END IF;
    SET vResults = CONCAT(vResults,'CALL osae_sp_pattern_script_add(\'',patternName,'\',\'',vScriptName,'\');','\r\n');
  END LOOP get_scripts;
  CLOSE script_cursor;

 SELECT vResults; 
END
$$

--
-- Definition for procedure osae_sp_pattern_lookup
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_lookup(IN Input VARCHAR(255))
BEGIN

END
$$

--
-- Definition for procedure osae_sp_pattern_match_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_match_add(
  IN  ppattern  varchar(400),
  IN  pmatch    varchar(400)
)
BEGIN
DECLARE vPatternCount INT;
DECLARE vPatternID INT Default NULL;
    SELECT COUNT(pattern_id) INTO vPatternCount FROM osae_pattern WHERE pattern=ppattern;
    IF vPatternCount > 0 THEN
        SELECT pattern_id INTO vPatternID FROM osae_pattern WHERE pattern=ppattern;
        INSERT INTO osae_pattern_match (pattern_id,`match`) VALUES(vPatternID,pmatch) ON DUPLICATE KEY UPDATE `match`=pmatch;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_pattern_match_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_match_delete(
  IN pname varchar(400)
)
BEGIN
  DELETE FROM osae_pattern_match WHERE `match`=pname;
END
$$

--
-- Definition for procedure osae_sp_pattern_match_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_match_update(IN pPattern VARCHAR(255), IN pOldName VARCHAR(255), IN pNewName VARCHAR(255))
BEGIN
DECLARE vPatternCount INT;
DECLARE vPatternID INT Default NULL;
    SELECT COUNT(pattern_id) INTO vPatternCount FROM osae_pattern WHERE pattern=pPattern;
    IF vPatternCount > 0 THEN
        SELECT pattern_id INTO vPatternID FROM osae_pattern WHERE pattern=pPattern;
        UPDATE osae_pattern_match SET `match`=pNewName WHERE pattern_id=vPatternID and `match`=pOldName;
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_pattern_parse
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_parse(IN ppattern varchar(2000))
BEGIN
  # This script parses output and replaces placeholders with Objects, properties and other values.
  DECLARE vInput VARCHAR(2000) DEFAULT '';
  DECLARE vOutput VARCHAR(2000) DEFAULT '';  
  DECLARE vOld VARCHAR(200);  
  DECLARE vWorking VARCHAR(200); 
  DECLARE vDot INT DEFAULT 0;
  DECLARE vSpace1 INT DEFAULT 0;
  DECLARE vSpace2 INT DEFAULT 0;  
  DECLARE vObject VARCHAR(200);
  DECLARE vParam VARCHAR(255);  
  DECLARE vTemp VARCHAR(255);  
    SET vInput = ppattern; 
    SELECT INSTR(vInput,'[') INTO vSpace1;
    SELECT INSTR(vInput,']') INTO vSpace2;
        
    WHILE vSpace2 > vSpace1 DO 
      SELECT MID(vInput,vSpace1,vSpace2 - vSpace1 + 1) INTO vOld; 
      SELECT MID(vInput,vSpace1+1,vSpace2 - vSpace1 - 1) INTO vWorking; 
      #SELECT vOld, vWorking;     
      SELECT INSTR(vWorking,'.') INTO vDot;
      IF vDOT > 0 THEN
        SET vObject = LEFT(vWorking,vDot - 1);
        SET vParam = RIGHT(vWorking,LENGTH(vWorking) - vDot);
        IF vParam = 'State' THEN
          SELECT state_name INTO vTemp FROM osae_v_object WHERE UPPER(object_name)=UPPER(vObject) OR UPPER(object_alias)=UPPER(vObject);        
          SET vInput = REPLACE(vInput,vOld,vTemp);
        ELSEIF vParam = 'Container' THEN
          SELECT container_name INTO vTemp FROM osae_v_object WHERE UPPER(object_name)=UPPER(vObject) OR UPPER(object_alias)=UPPER(vObject);        
          SET vInput = REPLACE(vInput,vOld,vTemp);
        ELSEIF vParam = 'Object Type' THEN
          SELECT object_type INTO vTemp FROM osae_v_object WHERE UPPER(object_name)=UPPER(vObject) OR UPPER(object_alias)=UPPER(vObject);        
          SET vInput = REPLACE(vInput,vOld,vTemp);
        ELSE
          SELECT property_value INTO vTemp FROM osae_v_object_property WHERE (UPPER(object_name)=UPPER(vObject) OR UPPER(object_alias)=UPPER(vObject)) AND property_name=vParam;
          SET vInput = REPLACE(vInput,vOld,vTemp);          
        END IF;      
      END IF;
      SELECT INSTR(vInput,'[') INTO vSpace1;
      SELECT INSTR(vInput,']') INTO vSpace2;
    END WHILE;
    SELECT vInput;
END
$$

--
-- Definition for procedure osae_sp_pattern_scripts_get
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_scripts_get(IN ppattern VARCHAR(255))
BEGIN
  SELECT s.script_id
    FROM osae_pattern_script s
    INNER JOIN osae_pattern p ON p.pattern_id = s.pattern_id
    WHERE p.pattern = ppattern;
END
$$

--
-- Definition for procedure osae_sp_pattern_script_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_script_add(IN ppattern VARCHAR(255), IN pscript VARCHAR(255))
BEGIN
DECLARE vPatternCount INT;
DECLARE vPatternID INT;
DECLARE vScriptID INT;
DECLARE vScriptSeq INT;
SET vScriptSeq = 0;
    SELECT COUNT(pattern_id) INTO vPatternCount FROM osae_pattern WHERE pattern=ppattern;
    SELECT script_id INTO vScriptID FROM osae_script WHERE script_name=pscript;
    IF vPatternCount > 0 THEN
        SELECT pattern_id INTO vPatternID FROM osae_pattern WHERE pattern=ppattern;
        SELECT COALESCE(script_sequence, 0) INTO vScriptSeq FROM osae_pattern_script where pattern_id = vPatternID ORDER BY script_sequence DESC LIMIT 1;
        INSERT INTO osae_pattern_script (pattern_id,script_id, script_sequence) VALUES(vPatternID,vScriptID,vScriptSeq+1);
        
    END IF; 
END
$$

--
-- Definition for procedure osae_sp_pattern_script_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_script_delete(IN ppatternscriptid INT)
BEGIN
DELETE FROM osae_pattern_script
    WHERE pattern_script_id = ppatternscriptid;
END
$$

--
-- Definition for procedure osae_sp_pattern_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_pattern_update(
  IN  poldpattern  varchar(400),
  IN  pnewpattern  varchar(400)
)
BEGIN
  UPDATE osae_pattern SET pattern=pnewpattern WHERE pattern=poldpattern;
END
$$

--
-- Definition for procedure osae_sp_process_recurring
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_process_recurring()
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
        #CALL osae_sp_debug_log_add(CONCAT('ID=',iRECURRINGID,', Interval=',cINTERVAL,' Time=',dRECURRINGTIME,' Date=',dRECURRINGDATE),'sp_process_recurring'); 
        IF NOT done THEN
            IF cINTERVAL = 'Y' THEN
                SET dCURDATE = CURDATE();
                #CALL osae_sp_debug_log_add(CONCAT('--IF ',dRECURRINGDATE,' < ',dCURDATE,' THEN'),'SYSTEM'); 
                IF dRECURRINGDATE < dCURDATE THEN
                    SET iDATEDIFF = DATEDIFF(dCURDATE,dRECURRINGDATE) / 365; 
                    #CALL osae_sp_debug_log_add(CONCAT('sp_process_recurring: DateDiff=',iDATEDIFF),'SYSTEM'); 
                    SET dRECURRINGDATE = DATE_ADD(dRECURRINGDATE,INTERVAL iDATEDIFF YEAR);
                    IF dRECURRINGDATE < dCURDATE THEN 
                        SET dRECURRINGDATE = DATE_ADD(dRECURRINGDATE,INTERVAL 1 YEAR);                   
                    END IF;                                     
                END IF;
                #CALL osae_sp_debug_log_add(CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),'SYSTEM'); 
                SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                IF iMATCHES = 0 THEN
                    CALL osae_sp_schedule_queue_add (CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);
                END IF;
              ELSEIF cINTERVAL = 'T' THEN   
                SET dCURDATETIME = NOW();             
                SET dCURDATE = CURDATE();
                IF dCURDATETIME > ADDTIME(NOW(),SEC_TO_TIME(iRECURRINGMINUTES * 60)) THEN
                    SET dCURDAYOFWEEK = dCURDAYOFWEEK + 1;
                    If dCURDAYOFWEEK > 7 THEN
			SET dCURDAYOFWEEK = 1;
		    END IF;
                    SET dCURDATE=DATE_ADD(CURDATE(),INTERVAL 1 DAY);
                END IF; 
                #CALL osae_sp_debug_log_add(CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),'SYSTEM'); 
                SELECT COUNT(schedule_ID) INTO iMATCHES FROM osae_schedule_queue WHERE recurring_id=iRECURRINGID;      
                IF iMATCHES = 0 THEN
                    CALL osae_sp_schedule_queue_add (ADDTIME(NOW(),SEC_TO_TIME(iRECURRINGMINUTES * 60)),vOBJECTNAME,vMETHODNAME,vPARAM1,vPARAM2,vSCRIPTNAME,iRECURRINGID);
               END IF;               
            ELSEIF cINTERVAL = 'M' THEN                
                SET dCURDATE = CURDATE();
                SET dRECURRINGDATE = CONCAT(YEAR(NOW()),'-',MONTH(NOW()),'-' ,dRECURRINGDAY);                
                IF dRECURRINGDATE < dCURDATE THEN
                    #CALL osae_sp_debug_log_add(CONCAT('sp_process_recurring: DateDiff=',iDATEDIFF),'SYSTEM');                
                    SET dRECURRINGDATE = DATE_ADD(dRECURRINGDATE,INTERVAL 1 MONTH);
                    IF dRECURRINGDATE < dCURDATE THEN 
                        SET dRECURRINGDATE = DATE_ADD(dRECURRINGDATE,INTERVAL 1 MONTH);                   
                    END IF;                                     
                END IF;
                #CALL osae_sp_debug_log_add(CONCAT(dRECURRINGDATE,' ',TIME(dRECURRINGTIME)),'SYSTEM'); 
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
                    If dCURDAYOFWEEK > 7 THEN
			SET dCURDAYOFWEEK = 1;
		    END IF;
                    SET dCURDATE=DATE_ADD(CURDATE(),INTERVAL 1 DAY);
                END IF; 
                #CALL osae_sp_debug_log_add(CONCAT('IF ',dCURDATETIME,' > ',dCURDATE,' ',dRECURRINGTIME,' Then Write new queue'),'SYSTEM');              
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

--
-- Definition for procedure osae_sp_run_readers
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_run_readers()
BEGIN
DECLARE iReadersReady INT;
    SELECT COUNT(object_property_scraper_id) INTO iReadersReady FROM osae_v_object_property_scraper_ready;
    IF iReadersReady > 0 THEN
        CALL osae_sp_method_queue_add('Script Processor','RUN READERS','','','SYSTEM','run_readers');
    END IF;         
END
$$

--
-- Definition for procedure osae_sp_run_scheduled_methods
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_run_scheduled_methods()
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
-- Definition for procedure osae_sp_schedule_queue_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_schedule_queue_add(IN pscheduleddate DATETIME, IN pobject VARCHAR(400), IN pmethod VARCHAR(400), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000), IN pscript VARCHAR(200), IN precurringid INT(10))
BEGIN
DECLARE vObjectID INT DEFAULT NULL;
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vScriptID INT DEFAULT NULL;
DECLARE vRecurringID INT DEFAULT NULL;
    SELECT script_id INTO vScriptID FROM osae_script WHERE UPPER(script_name)=UPPER(pscript);
    SELECT object_id, method_id INTO vObjectID, vMethodID FROM osae_v_object_method WHERE (UPPER(object_name) = UPPER(pobject) OR UPPER(object_alias) = UPPER(pobject)) AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    IF precurringid > 0 THEN
        SET vRecurringID = precurringid;
    END IF;
    INSERT INTO osae_schedule_queue (queue_datetime,object_id,method_id,parameter_1,parameter_2,script_id,recurring_id) VALUES(pscheduleddate,vObjectID,vMethodID,pparameter1,pparameter2,vScriptID,vRecurringID);
END
$$

--
-- Definition for procedure osae_sp_schedule_queue_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_schedule_queue_delete(
  IN pqueueid int
)
BEGIN
  DELETE FROM osae_schedule_queue WHERE schedule_id=pqueueid;
END
$$

--
-- Definition for procedure osae_sp_schedule_recurring_activate
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_schedule_recurring_activate(
  IN pschedulename VARCHAR(255), IN pactive bit
)
BEGIN
  UPDATE osae_schedule_recurring
    set active = pactive
    WHERE schedule_name = pschedulename;
END
$$

--
-- Definition for procedure osae_sp_schedule_recurring_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_schedule_recurring_add(IN pschedule_name VARCHAR(400), IN pobject VARCHAR(400), IN pmethod VARCHAR(400), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000), IN pscript VARCHAR(400), IN precurringtime TIME, IN psunday TINYINT(1), IN pmonday TINYINT(1), IN ptuesday TINYINT(1), IN pwednesday TINYINT(1), IN pthursday TINYINT(1), IN pfriday TINYINT(1), IN psaturday TINYINT(1), IN pinterval VARCHAR(10), IN precurringminutes INT(8), IN precurringday INT(4), IN precurringdate DATE, IN pactive TINYINT)
BEGIN
DECLARE vObjectID INT DEFAULT NULL;
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vScriptID INT DEFAULT NULL;
    SELECT script_id INTO vScriptID FROM osae_script WHERE script_name=pscript;
    SELECT object_id,method_id INTO vObjectID,vMethodID FROM osae_v_object_method WHERE (UPPER(object_name)=UPPER(pobject) OR UPPER(object_alias)=UPPER(pobject)) AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    INSERT INTO osae_schedule_recurring (schedule_name,object_id,method_id,parameter_1,parameter_2,script_id,interval_unit,recurring_time,recurring_minutes,recurring_day,recurring_date,sunday,monday,tuesday,wednesday,thursday,friday,saturday,active) VALUES(pschedule_name,vObjectID,vMethodID,pparameter1,pparameter2,vScriptID,pinterval,precurringtime,precurringminutes,precurringday,precurringdate,psunday,pmonday,ptuesday,pwednesday,pthursday,pfriday,psaturday,pactive);
END
$$

--
-- Definition for procedure osae_sp_schedule_recurring_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_schedule_recurring_delete(
  IN pschedulename varchar(400)
)
BEGIN
    DELETE FROM osae_schedule_recurring WHERE schedule_name=pschedulename;
END
$$

--
-- Definition for procedure osae_sp_schedule_recurring_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_schedule_recurring_update(IN poldschedulename VARCHAR(400), IN pnewschedulename VARCHAR(400), IN pobject VARCHAR(400), IN pmethod VARCHAR(400), IN pparameter1 VARCHAR(2000), IN pparameter2 VARCHAR(2000), IN pscript VARCHAR(400), IN precurringtime TIME, IN psunday TINYINT(1), IN pmonday TINYINT(1), IN ptuesday TINYINT(1), IN pwednesday TINYINT(1), IN pthursday TINYINT(1), IN pfriday TINYINT(1), IN psaturday TINYINT(1), IN pinterval VARCHAR(10), IN precurringminutes INT(8), IN precurringday INT(4), IN pprecurringdate DATE, IN pactive TINYINT)
BEGIN
DECLARE vObjectID INT DEFAULT NULL;
DECLARE vMethodID INT DEFAULT NULL;
DECLARE vScriptID INT DEFAULT NULL;
    SELECT script_id INTO vScriptID FROM osae_script WHERE script_name=pscript;
    SELECT object_id,method_id INTO vObjectID,vMethodID FROM osae_v_object_method WHERE UPPER(object_name)=UPPER(pobject) AND (UPPER(method_name)=UPPER(pmethod) OR UPPER(method_label)=UPPER(pmethod));
    UPDATE osae_schedule_recurring SET schedule_name=pnewschedulename,object_id=vObjectID,method_id=vMethodID,parameter_1=pparameter1,parameter_2=pparameter2,script_id=vScriptID,interval_unit=pinterval,recurring_time=precurringtime,recurring_minutes=precurringminutes,recurring_day=precurringday,recurring_date=pprecurringdate,sunday=psunday,monday=pmonday,tuesday=ptuesday,wednesday=pwednesday,thursday=pthursday,friday=pfriday,saturday=psaturday,active=pactive WHERE schedule_name=poldschedulename;
END
$$

--
-- Definition for procedure osae_sp_screen_object_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_screen_object_add(
  IN  pscreenname   varchar(200),
  IN  pobjectname   varchar(200),
  IN  pcontrolname  varchar(200)
)
BEGIN
DECLARE vScreenID INT;
DECLARE vObjectID INT;
DECLARE vControlID INT;
    SELECT osae_fn_lookup_object_id(pscreenname) INTO vScreenID;
    SELECT osae_fn_lookup_object_id(pobjectname) INTO vObjectID;    
    SELECT osae_fn_lookup_object_id(pcontrolname) INTO vControlID;
    # TODO - Add Duplicate Check, working on Constraint now
    INSERT INTO osae_screen_object (screen_id,object_id,control_id) VALUES(vScreenID,vObjectID,vControlID);    
END
$$

--
-- Definition for procedure osae_sp_screen_object_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_screen_object_delete(
  IN  pscreenname   varchar(200),
  IN  pobjectname   varchar(200),
  IN  pcontrolname  varchar(200)
)
BEGIN
DECLARE vScreenID INT;
DECLARE vObjectID INT;
DECLARE vControlID INT;
    SELECT osae_fn_lookup_object_id(pscreenname) INTO vScreenID;
    SELECT osae_fn_lookup_object_id(pobjectname) INTO vObjectID;    
    SELECT osae_fn_lookup_object_id(pcontrolname) INTO vControlID;
    DELETE FROM osae_screen_object WHERE screen_id=vScreenID AND object_id=vObjectID AND control_id=vControlID;    
END
$$

--
-- Definition for procedure osae_sp_screen_object_position
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_screen_object_position(IN pobject VARCHAR(255), IN pbyobject VARCHAR(255))
BEGIN
  DECLARE vObject VARCHAR(100);
  DECLARE vBy VARCHAR(100);
  DECLARE vByControlCount INT;
  DECLARE vObjectControlCount INT;
  DECLARE vByControlID INT;
  DECLARE vObjectControlID INT;
  DECLARE vNewX INT;
  DECLARE vNewY INT;
  DECLARE vObjectPropertyXID INT;
  DECLARE vObjectPropertyYID INT;
  SELECT object_name INTO vObject FROM osae_v_object WHERE UPPER(object_name) = UPPER(pobject) OR UPPER(object_alias) = UPPER(pobject);
  SELECT object_name INTO vBy FROM osae_v_object WHERE UPPER(object_name) = UPPER(pbyobject) OR UPPER(object_alias) = UPPER(pbyobject); 
  SELECT COUNT(object_id) INTO vByControlCount FROM osae_v_object_property WHERE object_type='CONTROL STATE IMAGE' AND property_name='Object Name' AND property_value=vBy;
   IF vByControlCount > 0 THEN
     #Get the settings from the screen control to use on the contained control
     SELECT object_id INTO vByControlID FROM osae_v_object_property WHERE object_type='CONTROL STATE IMAGE' AND property_name='Object Name' AND property_value=vBy LIMIT 1;
     SELECT property_value INTO vNewX FROM osae_v_object_property WHERE object_id=vByControlID AND object_type='CONTROL STATE IMAGE' AND property_name='Contained X';
     SELECT property_value INTO vNewY FROM osae_v_object_property WHERE object_id=vByControlID AND object_type='CONTROL STATE IMAGE' AND property_name='Contained Y';
     #The contained object must also have a screen control to bother
     SELECT COUNT(object_id) INTO vObjectControlCount FROM osae_v_object_property WHERE object_type='CONTROL STATE IMAGE' AND property_name='Object Name' AND property_value=vObject; 
     IF vObjectControlCount > 0 THEN
       SELECT object_id INTO vObjectControlID FROM osae_v_object_property WHERE object_type='CONTROL STATE IMAGE' AND property_name='Object Name' AND property_value=vObject LIMIT 1;
       SELECT object_property_id INTO vObjectPropertyXID FROM osae_v_object_property WHERE object_id=vObjectControlID AND object_type='CONTROL STATE IMAGE' AND property_name='State 1 X';
       SELECT object_property_id INTO vObjectPropertyYID FROM osae_v_object_property WHERE object_id=vObjectControlID AND object_type='CONTROL STATE IMAGE' AND property_name='State 1 Y';
       #So update the contained control to the settings of the container
       UPDATE osae_object_property SET property_value = vNewX WHERE object_property_id=vObjectPropertyXID; 
       UPDATE osae_object_property SET property_value = vNewY WHERE object_property_id=vObjectPropertyYID;
     END IF;
  END IF;
END
$$

--
-- Definition for procedure osae_sp_screen_object_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_screen_object_update(
  IN  pscreenname   varchar(200),
  IN  pobjectname   varchar(200),
  IN  pcontrolname  varchar(200)
)
BEGIN
DECLARE vScreenID INT;
DECLARE vObjectID INT;
DECLARE vControlID INT;
    SELECT osae_fn_lookup_object_id(pscreenname) INTO vScreenID;
    SELECT osae_fn_lookup_object_id(pobjectname) INTO vObjectID;    
    SELECT osae_fn_lookup_object_id(pcontrolname) INTO vControlID;
    UPDATE osae_screen_object SET object_id=vObjectID WHERE screen_id=vScreenID AND control_id=vControlID;    
END
$$

--
-- Definition for procedure osae_sp_screen_reflect_container
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_screen_reflect_container(IN pobject VARCHAR(255))
BEGIN
  DECLARE vContainer VARCHAR(100);
  DECLARE vContainerControlCount INT;
  DECLARE vObjectControlCount INT;
  DECLARE vContainerControlID INT;
  DECLARE vObjectControlID INT;
  DECLARE vNewX INT;
  DECLARE vNewY INT;
  DECLARE vObjectPropertyXID INT;
  DECLARE vObjectPropertyYID INT;
  SELECT container_name INTO vContainer FROM osae_v_object WHERE UPPER(object_name) = UPPER(pobject) OR UPPER(object_alias) = UPPER(pobject); 
  #The container must have a screen control to bother
  SELECT COUNT(object_id) INTO vContainerControlCount FROM osae_v_object_property WHERE object_type='CONTROL STATE IMAGE' AND property_name='Object Name' AND property_value=vContainer;
   IF vContainerControlCount > 0 THEN
     #Get the settings from the screen control to use on the contained control
     SELECT object_id INTO vContainerControlID FROM osae_v_object_property WHERE object_type='CONTROL STATE IMAGE' AND property_name='Object Name' AND property_value=vContainer LIMIT 1;
     SELECT property_value INTO vNewX FROM osae_v_object_property WHERE object_id=vContainerControlID AND object_type='CONTROL STATE IMAGE' AND property_name='Contained X';
     SELECT property_value INTO vNewY FROM osae_v_object_property WHERE object_id=vContainerControlID AND object_type='CONTROL STATE IMAGE' AND property_name='Contained Y';
     #The contained object must also have a screen control to bother
     SELECT COUNT(object_id) INTO vObjectControlCount FROM osae_v_object_property WHERE object_type='CONTROL STATE IMAGE' AND property_name='Object Name' AND property_value=pobject; 
     IF vObjectControlCount > 0 THEN
       SELECT object_id INTO vObjectControlID FROM osae_v_object_property WHERE object_type='CONTROL STATE IMAGE' AND property_name='Object Name' AND property_value=pobject LIMIT 1;
       SELECT object_property_id INTO vObjectPropertyXID FROM osae_v_object_property WHERE object_id=vObjectControlID AND object_type='CONTROL STATE IMAGE' AND property_name='State 1 X';
       SELECT object_property_id INTO vObjectPropertyYID FROM osae_v_object_property WHERE object_id=vObjectControlID AND object_type='CONTROL STATE IMAGE' AND property_name='State 1 Y';
       #So update the contained control to the settings of the container
       UPDATE osae_object_property SET property_value = vNewX WHERE object_property_id=vObjectPropertyXID; 
       UPDATE osae_object_property SET property_value = vNewY WHERE object_property_id=vObjectPropertyYID;
     END IF;
  END IF;
END
$$

--
-- Definition for procedure osae_sp_script_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_script_add(IN pname VARCHAR(255), IN pscriptprocessor VARCHAR(255), IN pscript TEXT)
BEGIN
  DECLARE vScriptProcessorID INT;
  SELECT script_processor_id INTO vScriptProcessorID FROM osae_script_processors WHERE script_processor_name = pscriptprocessor;
  INSERT INTO osae_script(script_name, script_processor_id, script)
    VALUES(pname,vScriptProcessorID,pscript);
END
$$

--
-- Definition for procedure osae_sp_script_delete
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_script_delete(IN pname VARCHAR(255))
BEGIN
  DELETE FROM osae_script
    WHERE script_name = pname;
END
$$

--
-- Definition for procedure osae_sp_script_export
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_script_export(IN pscript VARCHAR(255))
BEGIN
  DECLARE vResults TEXT;
  DECLARE vScriptProcessor VARCHAR(255);
  DECLARE vScriptText VARCHAR(4000);
  SELECT script_processor_name, script INTO vScriptProcessor,vScriptText FROM osae_v_script WHERE UPPER(script_name)=UPPER(pscript);
  SET vResults = CONCAT('CALL osae_sp_script_add (\'', pscript,'\',\'',vScriptProcessor,'\'),\'',vScriptText,'\');','\r\n');
  SELECT vResults; 
END
$$

--
-- Definition for procedure osae_sp_script_export_all
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_script_export_all()
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
-- Definition for procedure osae_sp_script_processor_add
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_script_processor_add(IN pname VARCHAR(255), IN ppluginname VARCHAR(255))
BEGIN
  DECLARE vCount INT;

   SELECT count(script_processor_id) INTO vCount FROM osae_script_processors WHERE script_processor_name = pname;
   IF vCount = 0 THEN
      INSERT INTO osae_script_processors(script_processor_name, script_processor_plugin_name)
      VALUES(pname,ppluginname);
   END IF;
END
$$

--
-- Definition for procedure osae_sp_script_processor_by_event_script_id
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_script_processor_by_event_script_id(
IN pEventScriptId int
)
BEGIN
SELECT script_processor_plugin_name FROM osae_script_processors
INNER JOIN osae_object_event_script
ON osae_script_processors.script_processor_id = osae_object_event_script.script_processor_id
WHERE osae_object_event_script.event_script_id = pEventScriptId;
END
$$

--
-- Definition for procedure osae_sp_script_processor_by_pattern
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_script_processor_by_pattern(
IN pPattern VARCHAR(400)
)
BEGIN
SELECT script_processor_plugin_name FROM osae_script_processors
INNER JOIN osae_pattern
ON osae_script_processors.script_processor_id = osae_pattern.script_processor_id
WHERE osae_pattern.pattern = pPattern;
END
$$

--
-- Definition for procedure osae_sp_script_processor_by_script_id
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_script_processor_by_script_id(
IN pScriptId int
)
BEGIN
  SELECT script_processor_plugin_name
  FROM
    osae_script_processors
  INNER JOIN osae_script
  ON osae_script_processors.script_processor_id = osae_script.script_processor_id
  WHERE
    osae_script.script_id = pScriptId;
END
$$

--
-- Definition for procedure osae_sp_script_update
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_script_update(IN poldname VARCHAR(255), IN pname VARCHAR(255), IN pscriptprocessor VARCHAR(255), IN pscript TEXT)
BEGIN
  DECLARE vScriptProcessorID INT;
  SELECT script_processor_id INTO vScriptProcessorID FROM osae_script_processors WHERE script_processor_name = pscriptprocessor;

  UPDATE osae_script
    SET script_name = pname, script_processor_id = vScriptProcessorID, script = pscript
    WHERE script_name = poldname;

END
$$

--
-- Definition for procedure osae_sp_server_log_clear
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_server_log_clear()
BEGIN
 DELETE
   FROM osae_log;
END
$$

--
-- Definition for procedure osae_sp_server_log_get
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_server_log_get(IN pinfo bit,
IN pdebug bit,
IN perror bit,
IN psource varchar(50))
BEGIN
 SELECT
   ID,`log_time`,`Thread`,`Level`,`Logger`,`Message`,`Exception`
 FROM osae_log
 WHERE ((Level = 'INFO' AND pinfo = 1)
 OR (Level = 'DEBUG' AND pdebug = 1)
 OR (Level = 'ERROR' AND perror = 1))
 AND (Logger = psource OR psource = 'ALL')

 ORDER BY osae_log.log_time DESC, ID DESC
 LIMIT 500;
END
$$

--
-- Definition for procedure osae_sp_system_count_occupants
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_system_count_occupants()
BEGIN
DECLARE vOccupantCount INT;
DECLARE vOldCount INT;
DECLARE vTemp VARCHAR(200);
DECLARE vOutput VARCHAR(200);
DECLARE bDone INT;
DECLARE var1 CHAR(40);
DECLARE var2 CHAR(40);
DECLARE oCount INT;
DECLARE oHouse CHAR(255);
DECLARE sContainerName CHAR(255);
DECLARE iContainerOccupants INT;
DECLARE iContainerOldOccupants INT;

DECLARE curs CURSOR FOR SELECT object_name FROM osae_v_object WHERE object_type='PERSON' AND state_name='ON';
DECLARE curs2 Cursor FOR SELECT object_name FROM osae_v_object WHERE object_type='ROOM' AND state_name='ON';
DECLARE CONTINUE HANDLER FOR NOT FOUND SET bDone = 1;
#  SET oHouse = (SELECT object_name FROM osae_v_object WHERE object_type='HOUSE');
 # SET vOldCount = (SELECT property_value FROM osae_v_object_property WHERE object_name=oHouse AND property_name='Occupant Count');
 # SELECT COUNT(object_id) INTO vOccupantCount FROM osae_v_object WHERE object_type='PERSON' AND state_name='ON';
 # IF vOldCount != vOccupantCount THEN
  #  CALL osae_sp_object_property_set(oHouse,'Occupant Count',vOccupantCount,'SYSTEM','osae_sp_system_count_occupants');
  #  CASE vOccupantCount
  #    WHEN 0 THEN 
   #     SET vOutput = 'Nobody is here';
   #     CALL osae_sp_object_property_set(oHouse,'Occupants',vOutput,'SYSTEM','osae_sp_system_count_occupants');
   #     CALL osae_sp_method_queue_add (oHouse,'OFF','','','SYSTEM','Auto-Occupancy Logic');          
  #    WHEN 1 THEN 
   #     SET vOutput = (SELECT COALESCE(object_name,'Nobody') FROM osae_v_object WHERE object_type='PERSON' AND state_name='ON' LIMIT 1);
   #     SET vOutput = CONCAT(vOutput,' is here');
   #     CALL osae_sp_object_property_set(oHouse,'Occupants',vOutput,'SYSTEM','osae_sp_system_count_occupants');
   #     CALL osae_sp_method_queue_add (oHouse,'ON','','','SYSTEM','Auto-Occupancy Logic');  
    #  ELSE
   #     OPEN curs;
   #       SET oCount = 0;
   #       SET bDone = 0;
   #       SET vOutput = '';
   #         REPEAT
   #           FETCH curs INTO var1;
   #           IF oCount < vOccupantCount THEN
   #             IF oCount = 0 THEN
   #               SET vOutput = CONCAT(vOutput,CONCAT(' and ', var1, ' are here'));
   #             ELSEIF oCount = 1 THEN
   #               SET vOutput = CONCAT(var1, vOutput);
   #             ELSE
    #              SET vOutput = CONCAT(var1, ', ', vOutput);
    #            END IF;
    #            SET oCount = oCount + 1;
     #         END IF;
  #          UNTIL bDone END REPEAT;
          
   #       CLOSE curs;
  #        CALL osae_sp_object_property_set(oHouse,'Occupants',vOutput,'SYSTEM','osae_sp_system_count_occupants');
  #        CALL osae_sp_method_queue_add (oHouse,'ON','','','SYSTEM','Auto-Occupancy Logic');  
   #     END CASE;
  #  END IF;

 #   SET vOldCount = 0;
 #   SET vOldCount = (SELECT COALESCE(property_value,0) FROM osae_v_object_property WHERE object_name='SYSTEM' AND property_name='Occupied Locations');
 #   SELECT COUNT(object_id) INTO vOccupantCount FROM osae_v_object WHERE object_type='ROOM' AND state_name='ON';
 #   #CALL osae_sp_debug_log_add(CONCAT('Counted Places: ',vOccupantCount, ' Old count = ',vOldCount),'SYSTEM');
 #   IF vOldCount != vOccupantCount THEN
  #      CALL osae_sp_object_property_set('SYSTEM','Occupied Room Count',vOccupantCount,'SYSTEM','osae_sp_system_count_occupants');
   #     CASE vOccupantCount
   #       WHEN 0 THEN 
    #        SET vOutput = 'All Locations are Vacant';
    #        CALL osae_sp_object_property_set('SYSTEM','Occupied Rooms',vOutput,'SYSTEM','osae_sp_system_count_occupants');            
    #      WHEN 1 THEN 
    #        SET vOutput = (SELECT object_name FROM osae_v_object WHERE object_type='PLACE' AND state_name='ON' LIMIT 1);
    #       SET vOutput = CONCAT('The ',vOutput,' is occupied');
   #         CALL osae_sp_object_property_set('SYSTEM','Occupied Rooms',vOutput,'SYSTEM','osae_sp_system_count_occupants');
    #      ELSE
    #        OPEN curs2;
   #         SET oCount = 0;
    #        SET bDone = 0;
    #        SET vOutput = '';
    #        REPEAT
    #          FETCH curs2 INTO var2;
     #         IF oCount < vOccupantCount THEN
     #           IF oCount = 0 THEN
     #             SET vOutput = CONCAT(vOutput,CONCAT(' and the ', var2, ' are occupied'));
      #          ELSEIF oCount = 1 THEN
     #             SET vOutput = CONCAT('the ', var2, vOutput);
       #         ELSE
       #           SET vOutput = CONCAT('the ', var2, ', ', vOutput);
       #         END IF;
      #          SET oCount = oCount + 1;
       #       END IF;
      #      UNTIL bDone END REPEAT;
          
      #      CLOSE curs2;
       #     CALL osae_sp_object_property_set(oHouse,'Occupied Rooms',vOutput,'SYSTEM','osae_sp_system_count_occupants');
     #    END CASE;
  #  END IF;
END
$$

--
-- Definition for procedure osae_sp_system_count_plugins
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_system_count_plugins()
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
-- Definition for procedure osae_sp_system_count_room_occupants
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_system_count_room_occupants()
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
-- Definition for procedure osae_sp_system_process_methods
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_system_process_methods()
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
-- Definition for procedure osae_sp_system_who_tripped_sensor
--
CREATE DEFINER = 'osae'@'%'
PROCEDURE osae_sp_system_who_tripped_sensor(IN psensor VARCHAR(255))
BEGIN
DECLARE vHouseOccupantCount INT;
DECLARE vRoomOccupantCount INT;
DECLARE vPerson VARCHAR(100);
DECLARE vContainer VARCHAR(100);
DECLARE vContainerID INT;
  SELECT container_name,container_id INTO vContainer,vContainerID FROM osae_v_object WHERE object_name=psensor;
  SELECT property_value INTO vHouseOccupantCount FROM osae_v_object_property WHERE object_name=psensor and property_name="OCCUPANT COUNT";
  IF vHouseOccupantCount = 0 THEN
  #NO ONE IS HERE!    THIS IS AN ALARM CONDITION, but just move Unidentified Person to the Sensor Location
     CALL osae_sp_screen_object_position("Unidentified Person",psensor);
    ELSE
       # FIRST Check to see if anyone is in the same container, if so, use them!
       SELECT property_value INTO vRoomOccupantCount FROM osae_v_object_property WHERE object_name=vContainer and property_name="OCCUPANT COUNT";
      IF vRoomOccupantCount > 0 THEN
        SELECT object_name INTO vPerson FROM osae_v_object WHERE base_type="PERSON" AND state_name = "ON" AND container_name=vContainer LIMIT 1;
        CALL osae_sp_screen_object_position(vPerson,psensor);
      ELSE
        SELECT object_name INTO vPerson FROM osae_v_object WHERE base_type="PERSON" AND state_name = "ON" LIMIT 1;
        #UPDATE osae_object SET container_id=vContainerID WHERE object_name=vPerson;
        #CALL osae_sp_screen_object_position(vPerson,psensor);
        CALL osae_sp_method_queue_add(vPerson,'Set Container',vContainer,'','SYSTEM','');
      END IF;
  END IF;
END
$$

--
-- Definition for function osae_fn_lookup_object_id
--
CREATE DEFINER = 'osae'@'%'
FUNCTION osae_fn_lookup_object_id(
  pobjectname varchar(200)
)
  RETURNS int(11)
BEGIN
DECLARE vObjectID INT;
  SELECT object_id INTO vObjectID FROM osae_object WHERE UPPER(object_name)=UPPER(pobjectname);
  RETURN vObjectID;
END
$$

--
-- Definition for function osae_fn_object_property_get
--
CREATE DEFINER = 'osae'@'%'
FUNCTION osae_fn_object_property_get(
  pname      varchar(200),
  pproperty  varchar(200)
)
  RETURNS varchar(2000) CHARSET utf8
BEGIN
DECLARE vProperty VARCHAR(2000) DEFAULT '';
  SELECT property_value INTO vProperty FROM osae_v_object_property WHERE object_name=pname AND property_name=pproperty;
  RETURN vProperty;
END
$$

--
-- Definition for function osae_fn_replace_objects
--
CREATE DEFINER = 'osae'@'%'
FUNCTION osae_fn_replace_objects()
  RETURNS int(11)
BEGIN
  RETURN NULL;
END
$$

DELIMITER ;

--
-- Definition for view osae_v_event_log
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_event_log
AS
	select `osae_event_log`.`event_log_id` AS `event_log_id`,`osae_event_log`.`parameter_1` AS `parameter_1`,`osae_event_log`.`parameter_2` AS `parameter_2`,`osae_event_log`.`from_object_id` AS `from_object_id`,`osae_event_log`.`debug_trace` AS `debug_trace`,`osae_event_log`.`log_time` AS `log_time`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`container_id` AS `container_id`,`osae_object`.`enabled` AS `enabled`,`osae_object_type_event`.`event_id` AS `event_id`,`osae_object_type_event`.`event_name` AS `event_name`,`osae_object_type_event`.`event_label` AS `event_label`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`container` AS `container`,`osae_object1`.`object_name` AS `from_object_name` from ((((`osae_object` join `osae_event_log` on((`osae_object`.`object_id` = `osae_event_log`.`object_id`))) join `osae_object_type_event` on((`osae_event_log`.`event_id` = `osae_object_type_event`.`event_id`))) join `osae_object_type` on((`osae_object_type_event`.`object_type_id` = `osae_object_type`.`object_type_id`))) left join `osae_object` `osae_object1` on((`osae_object1`.`object_id` = `osae_event_log`.`from_object_id`)));

--
-- Definition for view osae_v_method_log
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_method_log
AS
	select `osae_method_log`.`method_log_id` AS `method_log_id`,`osae_method_log`.`entry_time` AS `entry_time`,`osae_method_log`.`method_id` AS `method_id`,`osae_method_log`.`parameter_1` AS `parameter_1`,`osae_method_log`.`parameter_2` AS `parameter_2`,`osae_method_log`.`from_object_id` AS `from_object_id`,`osae_method_log`.`debug_trace` AS `debug_trace`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_types1`.`object_type` AS `base_type`,`osae_objects1`.`object_name` AS `object_owner` from (((((`osae_object_type` left join `osae_object` `osae_objects1` on((`osae_object_type`.`plugin_object_id` = `osae_objects1`.`object_id`))) left join `osae_object_type` `osae_object_types1` on((`osae_object_type`.`base_type_id` = `osae_object_types1`.`object_type_id`))) join `osae_object` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) join `osae_object_type_method` on((`osae_object_type`.`object_type_id` = `osae_object_type_method`.`object_type_id`))) join `osae_method_log` on(((`osae_object`.`object_id` = `osae_method_log`.`object_id`) and (`osae_object_type_method`.`method_id` = `osae_method_log`.`method_id`))));

--
-- Definition for view osae_v_method_queue
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_method_queue
AS
	select `osae_method_queue`.`method_queue_id` AS `method_queue_id`,`osae_method_queue`.`entry_time` AS `entry_time`,`osae_object`.`object_name` AS `object_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_method_queue`.`parameter_1` AS `parameter_1`,`osae_method_queue`.`parameter_2` AS `parameter_2`,`osae_object_1`.`object_name` AS `from_object`,`osae_method_queue`.`debug_trace` AS `debug_trace`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_description` AS `object_description`,`osae_method_queue`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_method_queue`.`from_object_id` AS `from_object_id`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_types1`.`object_type` AS `base_type`,`osae_objects1`.`object_name` AS `object_owner` from ((((((`osae_object_type` left join `osae_object` `osae_objects1` on((`osae_object_type`.`plugin_object_id` = `osae_objects1`.`object_id`))) left join `osae_object_type` `osae_object_types1` on((`osae_object_type`.`base_type_id` = `osae_object_types1`.`object_type_id`))) join `osae_object` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) join `osae_object_type_method` on((`osae_object_type`.`object_type_id` = `osae_object_type_method`.`object_type_id`))) join `osae_method_queue` on(((`osae_object`.`object_id` = `osae_method_queue`.`object_id`) and (`osae_object_type_method`.`method_id` = `osae_method_queue`.`method_id`)))) left join `osae_object` `osae_object_1` on((`osae_object_1`.`object_id` = `osae_method_queue`.`from_object_id`)));

--
-- Definition for view osae_v_object_event
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_event
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`container_id` AS `container_id`,`osae_object`.`enabled` AS `enabled`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`container` AS `container`,`osae_object_type_event`.`event_id` AS `event_id`,`osae_object_type_event`.`event_name` AS `event_name`,`osae_object_type_event`.`event_label` AS `event_label` from ((`osae_object` join `osae_object_type` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) join `osae_object_type_event` on((`osae_object_type`.`object_type_id` = `osae_object_type_event`.`object_type_id`)));

--
-- Definition for view osae_v_object_event_script
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_event_script
AS
	select `osae_object_event_script`.`event_script_id` AS `event_script_id`,`osae_object_event_script`.`script_id` AS `script_id`,`osae_script`.`script_name` AS `script_name`,`osae_object_event_script`.`script_sequence` AS `script_sequence`,`osae_script`.`script` AS `script`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_type_id` AS `object_type_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`container_id` AS `container_id`,`osae_object`.`enabled` AS `enabled`,`osae_object_type_event`.`event_id` AS `event_id`,`osae_object_type_event`.`event_name` AS `event_name`,`osae_object_type_event`.`event_label` AS `event_label` from (((`osae_object` join `osae_object_event_script` on((`osae_object`.`object_id` = `osae_object_event_script`.`object_id`))) join `osae_object_type_event` on((`osae_object_event_script`.`event_id` = `osae_object_type_event`.`event_id`))) join `osae_script` on((`osae_script`.`script_id` = `osae_object_event_script`.`script_id`)));

--
-- Definition for view osae_v_object_method
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_method
AS
	select `osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object`.`object_id` AS `object_id`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_object_type_method`.`param_1_label` AS `param_1_label`,`osae_object_type_method`.`param_2_label` AS `param_2_label`,`osae_object_type_method`.`param_1_default` AS `param_1_default`,`osae_object_type_method`.`param_2_default` AS `param_2_default`,`osae_object_type_method`.`method_id` AS `method_id` from ((`osae_object` left join `osae_object_type` on((`osae_object_type`.`object_type_id` = `osae_object`.`object_type_id`))) join `osae_object_type_method` on((`osae_object_type`.`object_type_id` = `osae_object_type_method`.`object_type_id`)));

--
-- Definition for view osae_v_object_property
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_property
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`container_id` AS `container_id`,`osae_object`.`min_trust_level` AS `min_trust_level`,`osae_object`.`enabled` AS `enabled`,`osae_object`.`last_updated` AS `object_last_updated`,coalesce(`osae_object`.`last_state_change`,now()) AS `last_state_change`,`osae_object_property`.`last_updated` AS `last_updated`,`osae_object_property`.`object_property_id` AS `object_property_id`,`osae_object_property`.`object_type_property_id` AS `object_type_property_id`,`osae_object_property`.`trust_level` AS `trust_level`,`osae_object_property`.`interest_level` AS `interest_level`,coalesce(`osae_object_property`.`source_name`,'') AS `source_name`,coalesce(`osae_object_property`.`property_value`,'') AS `property_value`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type_property`.`property_datatype` AS `property_datatype`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_type_property`.`property_default` AS `property_default`,`osae_object_type_property`.`property_id` AS `property_id`,`osae_object_type_property`.`track_history` AS `track_history`,`ot1`.`object_type` AS `base_type`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_property`.`property_object_type_id` AS `property_object_type_id`,`osae_object_type_1`.`object_type` AS `property_object_type`,`osae_object_1`.`object_name` AS `container_name`,timestampdiff(SECOND,`osae_object`.`last_state_change`,now()) AS `time_in_state` from (((((((`osae_object` join `osae_object_property` on((`osae_object`.`object_id` = `osae_object_property`.`object_id`))) join `osae_object_type` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) join `osae_object_type_property` on(((`osae_object_type`.`object_type_id` = `osae_object_type_property`.`object_type_id`) and (`osae_object_property`.`object_type_property_id` = `osae_object_type_property`.`property_id`)))) left join `osae_object_type_state` on((`osae_object`.`state_id` = `osae_object_type_state`.`state_id`))) join `osae_object_type` `ot1` on((`osae_object_type`.`base_type_id` = `ot1`.`object_type_id`))) left join `osae_object_type` `osae_object_type_1` on((`osae_object_type_property`.`property_object_type_id` = `osae_object_type_1`.`object_type_id`))) left join `osae_object` `osae_object_1` on((`osae_object`.`container_id` = `osae_object_1`.`object_id`)));

--
-- Definition for view osae_v_object_property_array
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_property_array
AS
	select `osae_object_property_array`.`property_array_id` AS `property_array_id`,`osae_object_property_array`.`item_name` AS `item_name`,`osae_object_property_array`.`item_label` AS `item_label`,`osae_object_property`.`object_property_id` AS `object_property_id`,`osae_object_property`.`property_value` AS `property_value`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object_type_property`.`property_id` AS `property_id`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_type_property`.`property_datatype` AS `property_datatype` from (((`osae_object_property_array` left join `osae_object_property` on((`osae_object_property_array`.`object_property_id` = `osae_object_property`.`object_property_id`))) join `osae_object` on((`osae_object_property`.`object_id` = `osae_object`.`object_id`))) join `osae_object_type_property` on((`osae_object_property`.`object_type_property_id` = `osae_object_type_property`.`property_id`)));

--
-- Definition for view osae_v_object_property_history
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_property_history
AS
	select `osae_object_property_history`.`history_id` AS `history_id`,`osae_object_property_history`.`history_timestamp` AS `history_timestamp`,`osae_object_property_history`.`property_value` AS `property_value`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object_property_history`.`object_property_id` AS `object_property_id`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_type_property`.`property_datatype` AS `property_datatype`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object`.`object_description` AS `object_description` from ((((`osae_object_property_history` join `osae_object_property` on((`osae_object_property_history`.`object_property_id` = `osae_object_property`.`object_property_id`))) join `osae_object` on((`osae_object_property`.`object_id` = `osae_object`.`object_id`))) join `osae_object_type_property` on((`osae_object_property`.`object_type_property_id` = `osae_object_type_property`.`property_id`))) join `osae_object_type` on(((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`) and (`osae_object_type_property`.`object_type_id` = `osae_object_type`.`object_type_id`))));

--
-- Definition for view osae_v_object_state
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_state
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`object_type_id` AS `object_type_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type_state`.`state_id` AS `state_id`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_state`.`state_label` AS `state_label` from ((`osae_object` left join `osae_object_type` on((`osae_object_type`.`object_type_id` = `osae_object`.`object_type_id`))) join `osae_object_type_state` on((`osae_object_type`.`object_type_id` = `osae_object_type_state`.`object_type_id`)));

--
-- Definition for view osae_v_object_state_change_history
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_state_change_history
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`address` AS `address`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_state`.`state_label` AS `state_label`,`osae_object_type_state`.`state_id` AS `state_id`,`osae_object_state_change_history`.`history_timestamp` AS `history_timestamp` from ((`osae_object` join `osae_object_state_change_history` on((`osae_object`.`object_id` = `osae_object_state_change_history`.`object_id`))) join `osae_object_type_state` on(((`osae_object_state_change_history`.`state_id` = `osae_object_type_state`.`state_id`) and (`osae_object`.`object_type_id` = `osae_object_type_state`.`object_type_id`))));

--
-- Definition for view osae_v_object_state_history
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_state_history
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`address` AS `address`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_state`.`state_label` AS `state_label`,`osae_object_type_state`.`state_id` AS `state_id`,`osae_object_state_history`.`times_this_hour` AS `times_this_hour`,`osae_object_state_history`.`times_this_day` AS `times_this_day`,`osae_object_state_history`.`times_this_month` AS `times_this_month`,`osae_object_state_history`.`times_ever` AS `times_ever`,`osae_object_state_history`.`times_this_year` AS `times_this_year` from ((`osae_object` join `osae_object_state_history` on((`osae_object`.`object_id` = `osae_object_state_history`.`object_id`))) join `osae_object_type_state` on(((`osae_object_state_history`.`state_id` = `osae_object_type_state`.`state_id`) and (`osae_object`.`object_type_id` = `osae_object_type_state`.`object_type_id`))));

--
-- Definition for view osae_v_object_type
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_type
AS
	select `osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`container` AS `container`,`osae_object_type`.`hide_redundant_events` AS `hide_redundant_events`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`ot1`.`object_type` AS `base_type` from ((`osae_object_type` left join `osae_object` on((`osae_object`.`object_id` = `osae_object_type`.`plugin_object_id`))) left join `osae_object_type` `ot1` on((`osae_object_type`.`base_type_id` = `ot1`.`object_type_id`)));

--
-- Definition for view osae_v_object_type_event
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_type_event
AS
	select `osae_object_type_event`.`event_id` AS `event_id`,`osae_object_type_event`.`event_name` AS `event_name`,`osae_object_type_event`.`event_label` AS `event_label`,`osae_object_type_event`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`hide_redundant_events` AS `hide_redundant_events`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id` from (`osae_object_type` join `osae_object_type_event` on((`osae_object_type`.`object_type_id` = `osae_object_type_event`.`object_type_id`)));

--
-- Definition for view osae_v_object_type_event_script
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_type_event_script
AS
	select `osae_object_type_event_script`.`object_type_event_script_id` AS `object_type_event_script_id`,`osae_object_type_event_script`.`script_id` AS `script_id`,`osae_script`.`script_name` AS `script_name`,`osae_object_type_event_script`.`script_sequence` AS `script_sequence`,`osae_script`.`script` AS `script`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type_event`.`event_id` AS `event_id`,`osae_object_type_event`.`event_name` AS `event_name`,`osae_object_type_event`.`event_label` AS `event_label` from (((`osae_object_type` join `osae_object_type_event_script` on((`osae_object_type`.`object_type_id` = `osae_object_type_event_script`.`object_type_id`))) join `osae_object_type_event` on((`osae_object_type_event_script`.`event_id` = `osae_object_type_event`.`event_id`))) join `osae_script` on((`osae_script`.`script_id` = `osae_object_type_event_script`.`script_id`)));

--
-- Definition for view osae_v_object_type_method
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_type_method
AS
	select `osae_object_type_method`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_object_type_method`.`object_type_id` AS `object_type_id`,coalesce(`osae_object_type_method`.`param_1_label`,'') AS `param_1_label`,coalesce(`osae_object_type_method`.`param_2_label`,'') AS `param_2_label`,coalesce(`osae_object_type_method`.`param_1_default`,'') AS `param_1_default`,coalesce(`osae_object_type_method`.`param_2_default`,'') AS `param_2_default`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden` from (`osae_object_type` join `osae_object_type_method` on((`osae_object_type`.`object_type_id` = `osae_object_type_method`.`object_type_id`)));

--
-- Definition for view osae_v_object_type_property
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_type_property
AS
	select `osae_object_type_property`.`property_id` AS `property_id`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_type_property`.`property_datatype` AS `property_datatype`,`osae_object_type_property`.`property_default` AS `property_default`,`osae_object_type_property`.`object_type_id` AS `object_type_id`,`osae_object_type_property`.`property_object_type_id` AS `property_object_type_id`,`osae_object_type_property`.`track_history` AS `track_history`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`system_hidden` AS `system_hidden`,coalesce(`osae_object_type_1`.`object_type`,'') AS `property_object_type` from ((`osae_object_type` join `osae_object_type_property` on((`osae_object_type`.`object_type_id` = `osae_object_type_property`.`object_type_id`))) left join `osae_object_type` `osae_object_type_1` on((`osae_object_type_property`.`property_object_type_id` = `osae_object_type_1`.`object_type_id`)));

--
-- Definition for view osae_v_object_type_property_option
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_type_property_option
AS
	select `osae_object_type_property_option`.`option_id` AS `option_id`,`osae_object_type_property_option`.`option_name` AS `option_name`,`osae_object_type_property`.`property_id` AS `property_id`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_type_property`.`property_datatype` AS `property_datatype`,`osae_object_type_property`.`property_default` AS `property_default`,`osae_object_type_property`.`object_type_id` AS `object_type_id`,`osae_object_type_property`.`track_history` AS `track_history`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`system_hidden` AS `system_hidden` from ((`osae_object_type` join `osae_object_type_property` on((`osae_object_type`.`object_type_id` = `osae_object_type_property`.`object_type_id`))) join `osae_object_type_property_option` on((`osae_object_type_property`.`property_id` = `osae_object_type_property_option`.`property_id`)));

--
-- Definition for view osae_v_object_type_state
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_type_state
AS
	select `osae_object_type_1`.`object_type` AS `base_type`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type_state`.`state_label` AS `state_label`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_state`.`state_id` AS `state_id` from ((`osae_object_type` join `osae_object_type_state` on((`osae_object_type`.`object_type_id` = `osae_object_type_state`.`object_type_id`))) join `osae_object_type` `osae_object_type_1` on((`osae_object_type`.`base_type_id` = `osae_object_type_1`.`object_type_id`))) order by `osae_object_type`.`object_type`;

--
-- Definition for view osae_v_object_verb
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_verb
AS
	select `osae_object`.`object_alias` AS `object_alias`,`osae_verb`.`verb` AS `verb`,`osae_object_1`.`object_alias` AS `transitive_object_alias`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_description` AS `object_description`,`osae_object_1`.`object_name` AS `transitive_object_name`,`osae_verb`.`tense` AS `tense`,`osae_verb`.`transitive` AS `transitive`,`osae_object_verb`.`object_verb_id` AS `object_verb_id`,`osae_object_verb`.`object_id` AS `object_id`,`osae_object_verb`.`verb_id` AS `verb_id`,`osae_object_verb`.`transitive_object_type_id` AS `transitive_object_type_id`,`osae_object_verb`.`transitive_verb_id` AS `transitive_verb_id`,`osae_object_verb`.`transitive_object_id` AS `transitive_object_id`,`osae_object_verb`.`adverb` AS `adverb`,`osae_object_verb`.`transitive_object_type_adjective` AS `transitive_object_type_adjective` from (((`osae_object` join `osae_object_verb` on((`osae_object`.`object_id` = `osae_object_verb`.`object_id`))) join `osae_object` `osae_object_1` on((`osae_object_1`.`object_id` = `osae_object_verb`.`transitive_object_id`))) join `osae_verb` on((`osae_verb`.`verb_id` = `osae_object_verb`.`verb_id`)));

--
-- Definition for view osae_v_pattern_match
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_pattern_match
AS
	select `osae_pattern`.`pattern_id` AS `pattern_id`,`osae_pattern`.`pattern` AS `pattern`,`osae_pattern_match`.`match_id` AS `match_id`,`osae_pattern_match`.`match` AS `match` from (`osae_pattern` left join `osae_pattern_match` on((`osae_pattern`.`pattern_id` = `osae_pattern_match`.`pattern_id`)));

--
-- Definition for view osae_v_pattern_script
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_pattern_script
AS
	select `osae_pattern`.`pattern_id` AS `pattern_id`,`osae_pattern`.`pattern` AS `pattern`,`osae_script`.`script_id` AS `script_id`,`osae_script`.`script_name` AS `script_name`,`osae_script`.`script` AS `script` from ((`osae_pattern_script` left join `osae_pattern` on((`osae_pattern_script`.`pattern_id` = `osae_pattern`.`pattern_id`))) left join `osae_script` on((`osae_pattern_script`.`script_id` = `osae_script`.`script_id`)));

--
-- Definition for view osae_v_schedule_queue
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_schedule_queue
AS
	select `osae_schedule_queue`.`schedule_id` AS `schedule_id`,`osae_schedule_queue`.`queue_datetime` AS `queue_datetime`,`osae_schedule_queue`.`parameter_1` AS `parameter_1`,`osae_schedule_queue`.`parameter_2` AS `parameter_2`,`osae_script`.`script_id` AS `script_id`,`osae_script`.`script_name` AS `script_name`,`osae_script`.`script` AS `script`,`osae_object_type_method`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_object_type_method`.`param_1_label` AS `param_1_label`,`osae_object_type_method`.`param_2_label` AS `param_2_label`,`osae_object_type_method`.`param_1_default` AS `param_1_default`,`osae_object_type_method`.`param_2_default` AS `param_2_default`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id`,`osae_object`.`object_type_id` AS `object_type_id`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`container_id` AS `container_id`,`osae_object`.`enabled` AS `enabled`,`osae_object`.`last_updated` AS `last_updated`,`osae_schedule_recurring`.`recurring_id` AS `recurring_id`,coalesce(`osae_schedule_recurring`.`schedule_name`,'One Time Execution') AS `schedule_name` from ((((`osae_schedule_queue` left join `osae_object` on((`osae_object`.`object_id` = `osae_schedule_queue`.`object_id`))) left join `osae_object_type_method` on((`osae_schedule_queue`.`method_id` = `osae_object_type_method`.`method_id`))) left join `osae_script` on((`osae_schedule_queue`.`script_id` = `osae_script`.`script_id`))) left join `osae_schedule_recurring` on((`osae_schedule_queue`.`recurring_id` = `osae_schedule_recurring`.`recurring_id`)));

--
-- Definition for view osae_v_schedule_recurring
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_schedule_recurring
AS
	select `osae_schedule_recurring`.`recurring_id` AS `recurring_id`,`osae_schedule_recurring`.`schedule_name` AS `schedule_name`,`osae_schedule_recurring`.`parameter_1` AS `parameter_1`,`osae_schedule_recurring`.`parameter_2` AS `parameter_2`,`osae_schedule_recurring`.`recurring_time` AS `recurring_time`,`osae_schedule_recurring`.`monday` AS `monday`,`osae_schedule_recurring`.`tuesday` AS `tuesday`,`osae_schedule_recurring`.`wednesday` AS `wednesday`,`osae_schedule_recurring`.`thursday` AS `thursday`,`osae_schedule_recurring`.`friday` AS `friday`,`osae_schedule_recurring`.`saturday` AS `saturday`,`osae_schedule_recurring`.`sunday` AS `sunday`,`osae_schedule_recurring`.`interval_unit` AS `interval_unit`,`osae_schedule_recurring`.`recurring_minutes` AS `recurring_minutes`,`osae_schedule_recurring`.`recurring_day` AS `recurring_day`,`osae_schedule_recurring`.`recurring_date` AS `recurring_date`,`osae_schedule_recurring`.`active` AS `active`,`osae_script`.`script_id` AS `script_id`,`osae_script`.`script_name` AS `script_name`,`osae_script`.`script` AS `script`,`osae_object_type_method`.`method_id` AS `method_id`,`osae_object_type_method`.`method_name` AS `method_name`,`osae_object_type_method`.`method_label` AS `method_label`,`osae_object_type_method`.`object_type_id` AS `object_type_id`,`osae_object_type_method`.`param_1_label` AS `param_1_label`,`osae_object_type_method`.`param_2_label` AS `param_2_label`,`osae_object_type_method`.`param_1_default` AS `param_1_default`,`osae_object_type_method`.`param_2_default` AS `param_2_default`,`osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`state_id` AS `state_id` from (((`osae_schedule_recurring` left join `osae_object` on((`osae_schedule_recurring`.`object_id` = `osae_object`.`object_id`))) left join `osae_script` on((`osae_schedule_recurring`.`script_id` = `osae_script`.`script_id`))) left join `osae_object_type_method` on((`osae_schedule_recurring`.`method_id` = `osae_object_type_method`.`method_id`)));

--
-- Definition for view osae_v_script
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_script
AS
	select `osae_script`.`script_id` AS `script_id`,`osae_script`.`script` AS `script`,`osae_script`.`script_processor_id` AS `script_processor_id`,`osae_script`.`script_name` AS `script_name`,`osae_script_processors`.`script_processor_name` AS `script_processor_name`,`osae_script_processors`.`script_processor_plugin_name` AS `script_processor_plugin_name` from (`osae_script` join `osae_script_processors` on((`osae_script`.`script_processor_id` = `osae_script_processors`.`script_processor_id`)));

--
-- Definition for view osae_v_interests
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_interests
AS
	select concat('What is ',`osae_v_object_property`.`object_name`,'\'s ',`osae_v_object_property`.`property_name`) AS `Question`,`osae_v_object_property`.`object_name` AS `object_name`,`osae_v_object_property`.`property_name` AS `property_name`,`osae_v_object_property`.`property_datatype` AS `property_datatype`,`osae_v_object_property`.`property_object_type` AS `property_object_type`,`osae_v_object_property`.`interest_level` AS `interest_level` from `osae_v_object_property` where (`osae_v_object_property`.`interest_level` > 0) order by `osae_v_object_property`.`interest_level` desc;

--
-- Definition for view osae_v_object
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`last_updated` AS `last_updated`,`osae_object`.`last_state_change` AS `last_state_change`,`osae_object`.`min_trust_level` AS `min_trust_level`,`osae_object`.`enabled` AS `enabled`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`container` AS `container`,`osae_object_type_state`.`state_id` AS `state_id`,coalesce(`osae_object_type_state`.`state_name`,'') AS `state_name`,coalesce(`osae_object_type_state`.`state_label`,'') AS `state_label`,`objects_2`.`object_name` AS `owned_by`,`object_types_2`.`object_type` AS `base_type`,`objects_1`.`object_name` AS `container_name`,`osae_object`.`container_id` AS `container_id`,(select max(`osae_v_object_property`.`last_updated`) from `osae_v_object_property` where ((`osae_v_object_property`.`object_id` = `osae_object`.`object_id`) and (`osae_v_object_property`.`property_name` <> 'Time'))) AS `property_last_updated`,timestampdiff(SECOND,`osae_object`.`last_state_change`,now()) AS `time_in_state` from (((((`osae_object` left join `osae_object_type` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) left join `osae_object_type` `object_types_2` on((`osae_object_type`.`base_type_id` = `object_types_2`.`object_type_id`))) left join `osae_object` `objects_2` on((`osae_object_type`.`plugin_object_id` = `objects_2`.`object_id`))) left join `osae_object_type_state` on(((`osae_object_type`.`object_type_id` = `osae_object_type_state`.`object_type_id`) and (`osae_object_type_state`.`state_id` = `osae_object`.`state_id`)))) left join `osae_object` `objects_1` on((`objects_1`.`object_id` = `osae_object`.`container_id`)));

--
-- Definition for view osae_v_object_property_scraper
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_property_scraper
AS
	select `osae_object_property_scraper`.`object_property_scraper_id` AS `object_property_scraper_id`,`osae_object_property_scraper`.`object_property_id` AS `object_property_id`,`osae_object_property_scraper`.`URL` AS `URL`,`osae_object_property_scraper`.`search_prefix` AS `search_prefix`,`osae_object_property_scraper`.`search_prefix_offset` AS `search_prefix_offset`,`osae_object_property_scraper`.`search_suffix` AS `search_suffix`,`osae_object_property_scraper`.`last_updated` AS `last_updated`,subtime(now(),`osae_object_property_scraper`.`update_interval`) AS `update_due`,`osae_object_property_scraper`.`update_interval` AS `update_interval`,`osae_v_object_property`.`object_id` AS `object_id`,`osae_v_object_property`.`object_name` AS `object_name`,`osae_v_object_property`.`object_alias` AS `object_alias`,`osae_v_object_property`.`object_description` AS `object_description`,`osae_v_object_property`.`state_id` AS `state_id`,`osae_v_object_property`.`address` AS `address`,`osae_v_object_property`.`container_id` AS `container_id`,`osae_v_object_property`.`property_name` AS `property_name`,`osae_v_object_property`.`property_default` AS `property_default`,`osae_v_object_property`.`property_datatype` AS `property_datatype`,`osae_v_object_property`.`object_type_id` AS `object_type_id`,`osae_v_object_property`.`object_type_description` AS `object_type_description`,`osae_v_object_property`.`object_type` AS `object_type`,`osae_v_object_property`.`property_value` AS `property_value`,`osae_v_object_property`.`source_name` AS `source_name`,`osae_v_object_property`.`interest_level` AS `interest_level`,`osae_v_object_property`.`trust_level` AS `trust_level` from (`osae_object_property_scraper` left join `osae_v_object_property` on((`osae_object_property_scraper`.`object_property_id` = `osae_v_object_property`.`object_property_id`)));

--
-- Definition for view osae_v_screen_object
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_screen_object
AS
	select `so`.`screen_object_id` AS `screen_object_id`,`so`.`screen_id` AS `screen_id`,`so`.`object_id` AS `object_id`,`so`.`control_id` AS `control_id`,`screen`.`object_name` AS `screen_name`,`co`.`enabled` AS `control_enabled`,`co`.`object_name` AS `control_name`,`oo`.`object_name` AS `object_name`,`ot`.`object_type` AS `control_type`,`oo`.`last_updated` AS `last_updated`,`oo`.`last_state_change` AS `last_state_change`,timestampdiff(MINUTE,`oo`.`last_state_change`,now()) AS `time_in_state`,`ots`.`state_name` AS `state_name`,(select max(`osae_v_object_property`.`last_updated`) from `osae_v_object_property` where ((`osae_v_object_property`.`object_id` = `oo`.`object_id`) and (`osae_v_object_property`.`property_name` <> 'Time'))) AS `property_last_updated` from (((((`osae_screen_object` `so` join `osae_object` `screen` on((`screen`.`object_id` = `so`.`screen_id`))) join `osae_object` `oo` on((`so`.`object_id` = `oo`.`object_id`))) join `osae_object` `co` on((`so`.`control_id` = `co`.`object_id`))) join `osae_object_type` `ot` on((`ot`.`object_type_id` = `co`.`object_type_id`))) left join `osae_object_type_state` `ots` on((`ots`.`state_id` = `oo`.`state_id`)));

--
-- Definition for view osae_v_object_list_full
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_list_full
AS
	select `osae_v_object`.`base_type` AS `base_type`,`osae_v_object`.`object_type` AS `object_type`,`osae_v_object`.`object_name` AS `object_name`,`osae_v_object`.`container` AS `container` from `osae_v_object` where (`osae_v_object`.`base_type` not in ('CONTROL','SCREEN','LIST')) union select `osae_v_object`.`base_type` AS `base_type`,`osae_v_object`.`object_type` AS `object_type`,`osae_v_object`.`object_alias` AS `object_name`,`osae_v_object`.`container` AS `container` from `osae_v_object` where ((`osae_v_object`.`object_alias` is not null) and (`osae_v_object`.`object_alias` <> '') and (`osae_v_object`.`base_type` not in ('CONTROL','SCREEN','LIST'))) order by `base_type`,`object_type`,`object_name`;

--
-- Definition for view osae_v_object_property_scraper_ready
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_property_scraper_ready
AS
	select `osae_v_object_property_scraper`.`object_property_scraper_id` AS `object_property_scraper_id`,`osae_v_object_property_scraper`.`object_property_id` AS `object_property_id`,`osae_v_object_property_scraper`.`URL` AS `URL`,`osae_v_object_property_scraper`.`search_prefix` AS `search_prefix`,`osae_v_object_property_scraper`.`search_prefix_offset` AS `search_prefix_offset`,`osae_v_object_property_scraper`.`search_suffix` AS `search_suffix`,`osae_v_object_property_scraper`.`last_updated` AS `last_updated`,`osae_v_object_property_scraper`.`update_due` AS `update_due`,`osae_v_object_property_scraper`.`update_interval` AS `update_interval`,`osae_v_object_property_scraper`.`object_id` AS `object_id`,`osae_v_object_property_scraper`.`object_name` AS `object_name`,`osae_v_object_property_scraper`.`object_alias` AS `object_alias`,`osae_v_object_property_scraper`.`object_description` AS `object_description`,`osae_v_object_property_scraper`.`state_id` AS `state_id`,`osae_v_object_property_scraper`.`address` AS `address`,`osae_v_object_property_scraper`.`container_id` AS `container_id`,`osae_v_object_property_scraper`.`property_name` AS `property_name`,`osae_v_object_property_scraper`.`property_default` AS `property_default`,`osae_v_object_property_scraper`.`property_datatype` AS `property_datatype`,`osae_v_object_property_scraper`.`object_type_id` AS `object_type_id`,`osae_v_object_property_scraper`.`object_type_description` AS `object_type_description`,`osae_v_object_property_scraper`.`object_type` AS `object_type`,`osae_v_object_property_scraper`.`property_value` AS `property_value`,`osae_v_object_property_scraper`.`source_name` AS `source_name`,`osae_v_object_property_scraper`.`interest_level` AS `interest_level`,`osae_v_object_property_scraper`.`trust_level` AS `trust_level` from `osae_v_object_property_scraper` where (`osae_v_object_property_scraper`.`last_updated` < `osae_v_object_property_scraper`.`update_due`);

--
-- Definition for view osae_v_object_type_state_list_full
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_type_state_list_full
AS
	select `osae_v_object_type_state`.`base_type` AS `base_type`,`osae_v_object_type_state`.`object_type` AS `object_type`,`osae_v_object_type_state`.`state_label` AS `state_label` from `osae_v_object_type_state` where ((`osae_v_object_type_state`.`base_type` not in ('CONTROL','SCREEN','LIST')) and `osae_v_object_type_state`.`object_type` in (select distinct `osae_v_object`.`object_type` from `osae_v_object`));

--
-- Definition for view osae_v_screen_updates
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_screen_updates
AS
	select `osae_v_screen_object`.`screen_object_id` AS `screen_object_id`,`osae_v_screen_object`.`screen_id` AS `screen_id`,`osae_v_screen_object`.`object_id` AS `object_id`,`osae_v_screen_object`.`control_id` AS `control_id`,`osae_v_screen_object`.`screen_name` AS `screen_name`,`osae_v_screen_object`.`control_name` AS `control_name`,`osae_v_screen_object`.`control_enabled` AS `control_enabled`,`osae_v_screen_object`.`object_name` AS `object_name`,`osae_v_screen_object`.`last_updated` AS `last_updated`,`osae_v_screen_object`.`last_state_change` AS `last_state_change`,`osae_v_screen_object`.`time_in_state` AS `time_in_state`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_state`.`state_label` AS `state_label`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_property`.`property_value` AS `property_value`,`osae_object_property`.`last_updated` AS `property_last_updated`,`osae_v_screen_object`.`control_type` AS `control_type` from ((((`osae_object` left join `osae_object_type_state` on((`osae_object`.`state_id` = `osae_object_type_state`.`state_id`))) join `osae_v_screen_object` on((`osae_object`.`object_id` = `osae_v_screen_object`.`object_id`))) left join `osae_object_type_property` on((`osae_object_type_property`.`object_type_id` = `osae_object`.`object_type_id`))) join `osae_object_property` on((`osae_object_type_property`.`property_id` = `osae_object_property`.`object_type_property_id`))) where ((`osae_v_screen_object`.`last_updated` > subtime(now(),'00:00:30')) or (`osae_object_property`.`last_updated` > subtime(now(),'00:00:30')));

--
-- Definition for view osae_v_system_occupied_rooms
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_system_occupied_rooms
AS
	select `o1`.`object_name` AS `room`,count(`o2`.`object_name`) AS `occupant_count` from (`osae_v_object` `o1` left join `osae_v_object` `o2` on(((`o1`.`object_id` = `o2`.`container_id`) and (`o2`.`base_type` = 'PERSON')))) where (`o1`.`object_type` = 'ROOM') group by `o1`.`object_name`;

DELIMITER $$

--
-- Definition for event osae_ev_day_timer
--
CREATE 
	DEFINER = 'osae'@'%'
EVENT osae_ev_day_timer
	ON SCHEDULE EVERY '1' DAY
	STARTS '2011-01-19 23:59:59'
	DO 
BEGIN
      UPDATE osae_object_state_history SET times_this_day=0;
    END
$$

ALTER EVENT osae_ev_day_timer
	ENABLE
$$

--
-- Definition for event osae_ev_hour_timer
--
CREATE 
	DEFINER = 'osae'@'%'
EVENT osae_ev_hour_timer
	ON SCHEDULE EVERY '1' HOUR
	STARTS '2011-01-19 22:00:00'
	DO 
BEGIN
      UPDATE osae_object_state_history SET times_this_hour=0;
    END
$$

ALTER EVENT osae_ev_hour_timer
	ENABLE
$$

--
-- Definition for event osae_ev_minute_maint
--
CREATE 
	DEFINER = 'osae'@'%'
EVENT osae_ev_minute_maint
	ON SCHEDULE EVERY '1' MINUTE
	STARTS '2010-06-08 21:39:17'
	DO 
BEGIN
    CALL osae_sp_object_property_set('SYSTEM','Date',CURDATE(),'SYSTEM','osae_ev_minute_maint'); 
    CALL osae_sp_object_property_set('SYSTEM','Day Of Week',DAYOFWEEK(CURDATE()),'SYSTEM','osae_ev_minute_maint'); 
    CALL osae_sp_object_property_set('SYSTEM','Day Of Month',DAYOFMONTH(CURDATE()),'SYSTEM','osae_ev_minute_maint'); 
    CALL osae_sp_run_scheduled_methods; 
    #CALL osae_sp_debug_log_add('Minute timer','SYSTEM');  
END
$$

ALTER EVENT osae_ev_minute_maint
	ENABLE
$$

--
-- Definition for event osae_ev_month_timer
--
CREATE 
	DEFINER = 'osae'@'%'
EVENT osae_ev_month_timer
	ON SCHEDULE EVERY '1' MONTH
	STARTS '2011-02-01 12:00:00'
	DO 
BEGIN
      UPDATE osae_object_state_history SET times_this_month=0;
    END
$$

ALTER EVENT osae_ev_month_timer
	ENABLE
$$

--
-- Definition for event osae_ev_off_timer
--
CREATE 
	DEFINER = 'osae'@'%'
EVENT osae_ev_off_timer
	ON SCHEDULE EVERY '1' SECOND
	STARTS '2010-05-23 10:09:24'
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

ALTER EVENT osae_ev_off_timer
	ENABLE
$$

DELIMITER ;

-- 
-- Dumping data for table osae_debug_log
--

-- Table osae.osae_debug_log does not contain any data (it is empty)

-- 
-- Dumping data for table osae_images
--
INSERT INTO osae_images VALUES
(105, x'89504E470D0A1A0A0000000D494844520000001E000000260806000000D3192B620000000467414D410000B18F0BFC6105000000097048597300000EC000000EC0016AD689090000004174455874436F6D6D656E740043524541544F523A2067642D6A7065672076312E3020287573696E6720494A47204A50454720763632292C207175616C697479203D2039300AB04558930000001874455874536F667477617265007061696E742E6E657420342E302E338CE6975000000B0B494441545847B557D96E5BD7159564A74983A670AC79A24451142951E23C5D8E92ECD8D6EC410307D1B265D98A1C0DB635CFA24451942DD94ED274401FFAD2024D5FF212A02F015AA02D82A231621841FAD2FC401F8AE6039AD5B52F492768932268D10B6CDCC3AB7BCF3A6BED75F63E2A900B40613ED407FFA7EBDFE6E78322C6C95C147DE599C4F34549A81FFC87EB9BDEF9B6DF7FED95FFF8BF9E20777DE3F7DF66E2FF6501FC4E5555BDE4C7BFC40986482FF7FCEFE78C25729F3EBFBEEED9375E7CF925C677192F33BEC378211779C07CE49FC97BAF30E45DF956F505431623F7E73E918BE3E791BF383E911B16147CF2C91F0B9F7DF841E1FBEFFFAAF0A38F7E57F4E7274F0A3FFDF44F45F2FC2F4F9F163E7BF661E1679F3D5327FCE98FDF3A79FC20FDBD8747E9577EF9F39FBDF0DB0F7E2DEF14F19DA2A74FFF50F8E4C9EF8BB6B7378A161717EB1697964CB7A7A75B33994CE1C71FFFE64B45DE7BEF17DF3F4AEFBCB8B7BDFAE2FEEEC6C9FDE4E64B4799DD57DE3C3EF83EE3D49BC7FB2F1FEEEF961C1DEC9EBA9FD97BF1F060AFEDCEF41496665FC7CCCD314CDEBC8E83FDDDA1F4FE8EE6209DAC4EEFEDD466D2BB456F4CDDFA7B6F4F17FC7E1FCE7476E2CEDC4C746773A3E1F1F103E7BBEFBE5B54703F9D2AB91AB9F2457CE8226E2446F0FAF8286626C7706F7A028BB39358997F0337AF25309E8861723C8179024E2422988A5FC1E84017C6E243989C18C3D5780C895814894414B726AEE1C698FC1E4164F832224357101F19C2C8D02012A3F17FEC6C6F5F28B87D6B0267437E74F8BC082A2EF83D4EF8188ADB092FA33314405FD779F49E3F8BFEEE7318B9D487E1CB03387FEE0CCEB68770A9AF0B03BDDDE8EDEA465F6F1F7A7AFA100804E07638E071BAE071B9E1F578A178BD080542387FA107376E4E7E513074A917E616038C8D0DD037D4A3515B87867A465D1DB4751A38AC16AEF8229945C83A8AB1D830A283973072F922AEF4F761686000B191081257C73071F316A264ED7438A1D3CA7C8DD037EAD1A4E7FC8666989A4DB0D91C181C1C46C185739D2A405559192A4A4A515E528692E2129C3E7D1AC5AF16C3DCD68AB9E949ACAD2E607579018B0B7731F3C6146E4FDEC4ADF171BC3E710BF7E617B0B0BA89CDDD14EE70EC74BA515959C5A8464D552D6A6B34A8AD15220D686A32A28BAA145C38D389C6BA7A545754A1AABC0295A51528E5024A095E7ABA144EBB0D0B0BB3A04BB1B1BE8EF5B515CCDFBB8BB999594CDF9EC6CCED3B5824E8F26612DBFB87985F59875709A09A80D535B5D068EA51AFD142AB6D4423999B9A5BD1D5D54BE0B31D686A68406D750DAA6595042F2F2B4719994BF8DC2E022EE1E060177BBB49C6363637D6A9C02A565657B0BCB48ACDE41EB65219A4EE3FC4CAE616FCC176D4D4D44153AB453D593668F5D0EB8D30B4B452410BBABB09DCFD5A270C0D3A68B8BAEAEA6A54555612BC12E5A5E58C329CF12B48ED6EE0F1E307C86452383A4A83DB4B1DA75249ECA753D8CB647070FC10878FDEC4EAD68E0A5C5B5B8F3A8D8E4CF5686C6C82D1D88216532B2C6DD63C70078C64ACAD662EC8BA86ACAB287B655905A52FC799A09F2C37F1F02119EDEF10700FFB7B3B3848EF22B9B385BDBD2476F6F7913E3A46E6E1634ABE0D7FA083A0C256075D0381C55C043699DA089C63DCF35A3B8C3ACA51AB8186C0B534447545352ACAC89C92B7077D9C7C0B8F1E1D224DB90FEFEFB3602491E13849D95329A6E0E000478F1FE3F13BEF606D3B095FB0838C7379D535D1D59499AE6EF9AAD43DE70458873A020B63615B41B692DF0A467BC04FA6DB78EBED633C382200E3FE610AF7B98014254F67F691397A80B7DFF9017EF4931F628BCE567C21D5C5F5F564AC0237C360CC01B79AD123E6EA39DF01830033C735550496FC726B151717ABDBCAE753B09D5C43E6018108B84263AD32563636B0B0B68EF9E535ECA4D25CC07D1C1E3FE2DFB6E0F20450554D736972C66A346419731F9B5B2D7475370AFA2E10B841ABE6561C5DC6AD247BF8D4A9538C57A1285E6C6CADE0E0700F7BCCEBF2DA2AC196718F80B30B4B98B9BB888DED5DA4D2191C3E38C6F2EA06AC7637CACBE9156E29D94E5AAD8EE03418C15B69B0AEAE2E140C749D81D5646495D1B262692839F35CC5FC5694332A110E0770F7EE3476921BD8DE21CB8579CCDE99C3DCBD794C4DCF61EAF62CF7F13A3676928C5D4C4ECFA0CD624385F885F5A1A181523735ABE66AA3CC568B1D3D92E3C8955E84FD6E047D2E043C0EF85C36781C16B86C66B8AD6684B89D2EF677211EBD82F16B7124583A63D10846D914AE8E8EB2E827108F271089C4303C3CC27ADD0BBFE2A37BDB60B75861B7D9E16625F3B35607FD0174B677B0718C10F8724F16D4EB80DF65CF02DB2D70985B613799E060C9F4F179908D23E47522CC4612F2BAD4DF418F8C3D7CE6652808F0EEF578D04A13B5185AD0CA2AD5D666667DB6B35978E023707BB813C3232A701FC23E378139999B8C9D5698297DB35EC78AA6858E0D434BC7D750FE8A8A321AAF0465A52CA725C56AC8B8BCAC9469A9500B904653873A36181D8B925EAF5773EA743AE1F12AECCD41B4B777927124CB5880438A1B4E6B1B57AA63476AA3DC36785D0EB646071C94BDD5D4C22D6160156AE436D1D2F1C5AA09351A0D0B05EB310DA4A37B8DCDCCA5D90C8BC502B3D9A6168D2683010E3B15F505E899BCD4577AD0EEA75C3E0F9A0D8D64EE64AEA5273BD893EDF03A6D7039ACB0713166CA5F5F5F8F52322C3A7102274F9E4409F7BA868A188C54A9C58456D5407C9FF23A1C2EB828B19BF237EAF4ECC93E4A4DE091280AA2837DE80878997C275A8D7A95B91C087CFCED235BAFC3CE8F39899D935984B909CD04A9949ACE686A6AE21E35B2389854A6566BD650D2935D2E970AAA30FFA69CE4E17018D10819C706FBD11154284D0B5C76338D4660E65B4E228A00536E37593BB90087CD06BB9561B6C04A10B304DD6B5101F94C40ED7CCFE980D39505F610D8E7F3D17459F0F6F67644C55CB1A17E1E6F1418F40D59B6943CC87BF60844B3115C21630FC13D4EB2E7029CB2009B85724A0A72C1B10ACA238F80BA55501E7B682A9FDF8F6030A4A6231C227084E68A0B301B41138F3E61E63A2F7540F91258CE60623245CCC645C879CACDBCCB21C1C1701250409DB230BE939758D87AB97F85B19CC39A5944C2A1306211E6383ED4476005C6260207B22613B9657B499E55C9F3CCE50028D20B38F7BB9B92BA0826804E198BBC7CC7E316A6DCD3DCE3925F01F6B3D948AD0E07C3188D1278948C05CCD46CA0BB1555EA80000BEB1CB830565973011E75E21C7B19ABB9CC82BA195E55DE3CA0B0F573FFFAC938A8B6C420EF711E08D51C2B5CBDDDC2D5F8E5884B5002FB5560020A6B09915C8DECB1D7CD85B8654C59BDEAA2DC54C6950DC5A396C8004BA79FC0227380475B3B4F98F23BC6936A4164A8976661C120B8E4580515C60C35D73987CB22E49E355D164C510139E602943C53956DD6C901B6D400FF930850E610994ACD96F3759C07FD82C8600FB74633C15877738ECE87C82D40E778209C9C9CC010CFD3F98588F47257F8DBABA6849312500D4EEEA7D47E02FB25BF227590C03CD88BD3E3C2383AD80B6B5B7376FF4A7E55400627136091FAFAF5311E02D86F1F1EA3BFBF97528DE0EA5576A458841143FF403F0FF4098CD0ADC32C0E3E919AF9950806B28C05D8C305B905384AE0B1E8E5CF2D6D46821288E1CBC9FB5C62C6F88DEB3CAC273177F70EAE8D5F57E3D6D4A4DA93A767D99BEFDEC3BDA5252CAD6F20161F55D90B7840C06958010E32CF920A0F8FCBD7C7629F178C27065F70D94C7F533B14F7AE6A283191B449D946DC3A5DFC3FA9AFE702FF4792162ADB8DFF07091B19D39001DEA56F0725A76C8DE20145C0F95C05669E4304F6F16FA1A0F2D7F9F9D9A27F0222DF554D46F839E20000000049454E44AE426082', 'MS16A Off', 'png', 30, 38, 96),
(106, x'89504E470D0A1A0A0000000D494844520000001E000000260806000000D3192B620000000467414D410000B18F0BFC6105000000097048597300000EBF00000EBF01380553240000004174455874436F6D6D656E740043524541544F523A2067642D6A7065672076312E3020287573696E6720494A47204A50454720763632292C207175616C697479203D2039300AB04558930000001874455874536F667477617265007061696E742E6E657420342E302E338CE6975000000B2B4944415458479D57594C9CD719C536CE52A5916D0C030C0C0CC32C3030FBFECF3080E305B33889C1CC30606CBC10DB182F2C06B30E0CC380C17692A68BFAD097566AFA12A98A5429CA43A5B68AAAC6926545E94BF31CA90F55F312A90FCDE9F9EECCB86E93B4557FE9E85FE672CF3DE7FBEE773F4A00EC29F91F2E1947ECFD6FE39F19B7AF00F53705C8F3F3C48B32504F1C2064D0D726956F84FCC1FE02D4B86F818C2B922AC2C234EA92F7E2EFF2A29307F958F8FDE925DF9E819AE85B5024FC1A59F17A66DC0B84453EEC2BFCF6F42A0C52287C2A7E2B2A794AF2EF280CFF97EB1BC629AB4B0BBFFFC7ABF007DFE8CCFF7571A2EF12CFAA282A51F7C21879178B2A88EF10CF15508CB7DC05F24DC6159D29E64531374A09C5A1C83FFFFCF33D9F7CF2FBBD4F3EFA70EFFBEFFFE2F98F3FFECDF37F7CF4E8B94F3FFDC37E7E2FFDD3E3C7FB9E3CF9A8F4B3CF9E947EF0C12FF7FDF8876FBDBC7B2F7BE8FE4E56F7F39FFEC4F8EB0F7FA5E3981738E6A5C78F7FF7E2A347BFDDB3B2B2B86F66664637333B5B737562E2602E9753CE925416BAA7E4BDF77EB66727BBFAE2FACAFC0B1B6B8BFB37D24B2FEFE4D60EBCB9BB799038F4E6EE46D9D6C69A696773CDB29D5BD76F6DAE77DC9CB802C81CC4F8A5F3D8DC583B9ADD582DDBCCA68F66D7578DB9ECDA4BD7AE5C0EF674777DA56991AF8E74767E79F3C6F5FAD5A5C5FD0F77EFB9DE7DF7DD3D25DBD98CEB6CE2F4DF5203AFFEF5C2C8E0976F8C0DE3FAF8286E4F5CC4CCE438E6A6AEE1D2B9118C8D0C617C6C0453936F3C257D4A7E711467534318194A62642489CB17CFE1C2A8BC0F2271E67524064E23353880C1817E8C0CA7FEBEBAB272ADE4EAE58B78A54D4347248458D80F2DE8438408077C08119D6D51F4761D47CFF157D077F218065FEBFD1AF1A99E93E8E93A89DE9E5E7477F7221A8D22E0F522E8F323E80F20140C211C0AA12DDA86E327BA71E1D2F8572503AFF5C0D16C85ADB101E6867A341AEBD0504FD4D5C1586780D7E5E48A5FC5682A41D5498C0E9D41B2FFB5A7A403A74E6168308191B3A3B878E9329254EDF3FA6032CA7C8D30379A6131737E6B13EC4D76B8DD5EF4F79F41C989639D8AA0BAA2029587CBA13B5C81C3658771E8D021941D2C83A3B5053726C671777E1AF377A631337D0BD7AF5DC1D5F14BB83C3686372E5EC6EDA9694CCF2F61692D839B7CF6F902A8AAAA26F4A8A9AE456D8D01B5B522A401168B0D5D74A5E4C4914E34D6D5435F598D6A5D25AACA2B51CE059493BCFC50397C1E37A6A727C12CC5E2C20216EECE61EAF62DDCB83E8989AB13B87EF52666487A67298D958D2D4CCD2D20148E424F427D4D2D0C867AD41B8C301A1BD148E5F6A6167475F590F8950E581A1A50ABAF815E5649725D850E15542E8804FC249CC5E6E61AD6D7D2C40A961617E8C03CE6E6E77067761E4BE9752C6772C86CDFC7DCD232B4583B6A6AEA60A835A29E2A1B8C6698CD36589B5BE8A013274F92F8E4D14E581B4C3070757ABD1ED5555524AF82AE5C4754E0881646666D110F1FDE432E97C1CE4E16DC5EEA39934963239BC17A2E87CDDDFBD87AF026E6975715716D6D3DEA0C262A35A3B1D1029BAD19CDF616385B5D45E20ED8A8D8A8672CA8BA86AAAB697B554525ADD7E1484CA3CA25DCBF4F451BAB245CC7C6FA2A36B36B48AF2E637D3D8DD58D0D64777691BBFF9096AF408B769054D49A606A20B1241789EDF656121714771F6D87CD443B6A0D3090B89609A1AFD4A3B282CA69797B2CC2C997F1E0C116B2B47B6B7B8305238D1C9FD3B43D9361083637B1F3F0211EBEF30EEEAEA4118975507121AE260BB39A3633AB9B9FB5BAFB98109B504762512C6A2BA956E25B49B447352A5DC15B6FEFE2DE0E0988EDAD0CB6B9800C2DCFE63690DBB987B7DFF91E7EF0A3EF6399991D8EB4A92CAEAFA76245DC04ABAD40DCE240B72457F7F10E58859831AEA926B1C4975BABACAC4C6DAB48248C95F45DE4EE918884734CAC79626E7111D377173075E72E5633592E601B5BBB0FF8DB32FCC128AAF54C2E4321B11AAD79C5DCC78E1627B3FA244A7A4F90B8C1A8622B195DC1AD247BF8C08103C44184C3212C2ECF61736B1DEB8CEB9DBBF324BB83DB249C9C9EC5F55B33585C5943269BC3D6BD5DDC995F84CB13804EC75CE19692ED64349A48CE0423790B13ACABAB0B25A7BA8EC065B7B1CA1859B10CB49C71AE667C2B754415E2F1286EDD9AC06A7A112BAB54393D85C99B3770E3F614AE4CDCC095AB93DCC70B585C4D136B189FB88E56A71B95922FAC0F0D0DB4DAD2A492AB9536BB9C1E744B8C13A77B10D7028845FC8806BD88F8DD087A9DF0BB1D08B81C68E3767AB5AF0BA9E4698C9D4B6184A5732899C0300F85B3C3C32CFA2348A54690480CE1CC9941D6EB1E68E108B3B7151EA70B1EB7070156328DB53AA645D1D9DEC1836390C4AF77E749435E687E4F9ED8E384D7D1028FDD0E2F4B6684DF633C38DA423EC47990B485FCEA3D1694E720BF858830A2BC878241B430899AADCD6861956A6D75B03E7B7858041121717BBC13670615712FE29100893959808A7D2E38687D93D9C48A668489078691195F43FB2B2B2B9878875151CE727AB84C419E7515E50C4BA52A4006431DEA78C0985894CC66B38AA9CFE743301486A6C5D0DEDE49C589BC62216E0B07E073B572A5269E48ADB4DB8D90DFCBA3D10B2F6D6FB137734B5859851AB94D8CCCF83295840683818582F598096462F6DA9A184B87034EA7130E875B150D8BD50AAF878E46A2CC99A2D5A7BBD1AED1AE48104DD6462AF731D672267B79267B10F2B9E1F7BAE0E6621CB4BFBEBE1EE554B877DF3E949696E230F7BA818E586D74A9D98E1695401C4F7BBD5E3FFCB43840FB1B4D669EC9115A4DE2C1244A92FDBDE88886187C1F5A6C66A55C1A8208DF23541BF27AF8C79CC4C3C99CA2DC8E269254494D272C160BF7A88DC5C1AE94BA5CF9849233D9EFF72BD230E36F2F581E8FC7914C50F1507F1F3A62615AD30CBFC7C1442331E32D9D485888697780AA7D5C80D7ED86C745389C7091C42160F63A1521BF09A987E37C5EF8FC79E22089239108932E4FDEDEDE8EA424D7D0401FDB9B30ACE686BC5A5A1EE33DDF0231D9481EA6E220C9833EAAE7027CB200B793764A080AE0B32265CB23A40145CAB6874915D134C4626D2A1CF1361227985C2921E6416061EB1367AC8B5647C3FF24961E4C922C2CC9C645483F1560DCA549F0123E120AA94F16C631458B456D88FB57144B1FD6C422126F8B6328C118A7067A491C86CD42E2683EC9C46ED95E1267657951B9348062BD9073BF0768A99F6442E89367B19763820151CA3DCD3D2EF115628D878DD4EA782C8EE1248987A958C8EC4D56667758591D1562515D2017C54A351710541317D4CBB38A659E34408494BD454251AB71FF6A541C5347628CF7141B4215E33057EF7172359AB4B82425B1A6884928AA0562B942BEED0D70210179A6AD21B5A8009DF1E7110EAA121965E9D4482C3647D9DA7AD861CAFB103BD592C4400F93850583E41263452A8A0915EB4286CB22E49E4FBA3C595811F2990B0817952AB5F94C8EF2488D6A125F8D3D754CD56CE9AF536CF44B12FDDDDC1A4D2463DD2D64741162B7101D6343383E7E1103ECA78B0B11EBE51EE67B48858493925081936BB45A23B126F115AB632466632F999E12C5C9FE1EB85A9BF2FB57E2AB08094E26C462F5F9F3A36C0278DEDEDF455F5F0FAD1AC4D9B33C918612C410FA4EF5B1A11FC120B3F50C8B4344AC667C05B1685EB11007B9A0801027493C9A7CFD0B67AB8DA4242222057B9F5A4C8C5D38CF663D8D1BB76EE2DCD87985CB57C6D5993C31C9B3F9D66DDC9E9DC5ECC2228652C34ABD9047859C092BC431C659421164BB7C7E74E88B92B191FE1ABFDBFE17754271EFAA849224926352B611B74ED7B123E8ED3EC1FF91E40895EDC6FF83448D3C3321A3BCCBB91D9398F268941C080B39BF2B62C6B98DC411FED6160BFF796A6AF2A57F008FE1E4F4B59D3D280000000049454E44AE426082', 'MS16A On 1', 'png', 30, 38, 96),
(107, x'89504E470D0A1A0A0000000D494844520000001E000000260806000000D3192B620000000467414D410000B18F0BFC6105000000097048597300000EBF00000EBF01380553240000004174455874436F6D6D656E740043524541544F523A2067642D6A7065672076312E3020287573696E6720494A47204A50454720763632292C207175616C697479203D2039300AB04558930000001874455874536F667477617265007061696E742E6E657420342E302E338CE6975000000B12494441545847A557594C5CE7191DC7769C3449EB986D6060986118666060F6EDCE0A3871CCEA240633CC8089B1636207639BCD600FFB300C186C92345DD487BEB452D3974855A44A51A4566AABAA6AAC585594BCF43D6DA546CA5BAB2A9C9EEFCE92588DDA875EE9E8FEF7BF77FEF39DF37DFF321A00870B788C38A4F9DA25CFD25F78FCBFAFC278478813F26022B4C4E3448958DAC453C4338404F6485072B14F82FDC6A0E52AF6C9BD00F5DBE24B213D463C59EAE4C576313A0948DE0BE4F96801C5B6F4CB37DF185CF1E2BB47839487C2BB472E7959F8A88847A226BE1EC4FF24FE8F4B7E5068962E19A08022810CFC04F19555BCD82E7EF7080AAFFFFBC50F6560F94151990CAE465F80101AA1D1FC9DF894EDB242BFFCEE59E269E219BEFB9C38605B4FC838E284A4F13B848C5114A1DE55F2CF3EFBECD0C71FFFE1E89F7EFFC1D1F7DEFBF9E31F7EF89BA39F3E7870F4934FFEF838FB9FF8F3C387C73928887FBEFFFE2FAA7FF483378FEEDDCD3E756F375BF6B39FFC58FBEB0F7E69E0BB7FC9370F1FFEAEFCC183DF3EB5B29236CCCDCD69E7E6E79FB93A39599BCBE5BEF5D147BF927A10E2439A77DFFDE9A1DDECEAB18D95C56F6FAEA7759B6B4B75BBB9F5636FEC6D3D4D3CF9C6DE667D81143BB98DB6EDAD0DE38DC92BEAB360E2D50BD8DA5C6FCF6EAE9E28F41DE4B2EBF6D7AF5CAEECEDE93A08874338D9D97970E3FAB5E3AB4BE9B2FDBDBB65EFBCF3CED39A9D6CE6F0F9C4D983D4E08BB8383A84D7C647706D620CD39397303735818599D7BF22191FFD7266EAB5BF169F4BFD97C60ECEA792FF283E5FBEF4CAC1C5B1E45F469343489C7B1989C1B3480D0D62687000A323A92F575756749AAB972FE1B958181DA120A28A0FE180172142F17B11243A639112417FF7290CBDD4577A2EE24C6F377ABBBA4BCF9148047E8F0701AF0F019F1FC140104A3088582486174EF7E0E2AB13079AC1977A616FB1C0DAD8007383018DC67A341888FA7A18EBF5F0381DA501C7478731963C87E181974A7D8367CE203994C0E8F9B1529FD7E385C928E335C2DC68469399E35B9A616BB6C1E5F26060E01C34A74F75AA04359595D09657A0AABC12E565E53871E204CA9E2D83BDAD15D727274A83CECDDEC4B5D7AFE0EAC4ABB83C3E8ED72E5DC6F4CC6CE9FD0DB6BD5E3FAAAB6B081D6A6BEA5057AB475D9D0869405393155D3D7D243ED989C67A0374DA1AD45469515DA1450503A82079C5890A78DD2ECCCE4E9506BE737B0133D33771FDDA1426AF4EE2DAD51B985B5C2ABD9F59B883A012818E84BADA3AE8F50618F446188D8D68A4725B732BBABA7A49FC5C079A1A1A50A7AB854EA224795565152AA95C10F2FB90BE338FADAD756CACAF112B584ADFC1EDC5452C2C2EE0D6FC2296D636B09CC921B3730F0B4BCB0847DB515B5B0F7D9D1106AA6C309A61365B616969A5830E747793B8FBF94E581A4CD0333A9D4E879AEA6A9257A3AAA28AA8C4C9B082CC7A1AFBFB7791CB65B0BB9BC5F6E6BADACE64D6B099CD602397C3D6DE3D6CDF7F038BCBAB2A715D9D01F57A13959AD1D8D804ABB5052DB65638DA9C45E20E58A9D8A8632EA8BA96AA6B687B75A596D657E164344C954BB8778F8A365749B881CD8D556C65D7B1B6BA8C8D8D35AC6E6E22BBBB87DCBD7DDC5A5A4138D24152516B82A981C4525C24B6D9DA485C50DCF37C3BAC26DA51A7879EC4752C089D56076D2595D3F2F66888832FE3FEFD6D6469F7F6CE26178C35E4D85EA3ED990C53B0B585DDFD7DECBFFD366EAFAC2114EDA0E2425E4D4DAC6ADACCAA6EF9BAD53DA784D8847A128B6251ABA55AC9AF96688F84A974056FBEB587BBBB242076B633D86100195A9ECD6D22B77B176FBDFD5D7CFF87DFC3F27A064A28A656B1C140C52A71332CD60271AB1D3D525C3D2F74C022C4CC716D0D8925BF9C5A656565EAB40A8514ACACDD46EE2E8948B8C0C25A2416D269CCDEBE83995BB7B19AC932801D6CEFDDE7BB65F80211D4E8585CFA4261355AF28A398FEDAD0E567537347DA749DC6054732B155DC9A92473F8F8F1E3C4B3509420D2CB0BD8DADEC006F37AEBF622C96E619A8453B3F3B876730EE9957564B2396CDFDDC3ADC5349C6E3FAAAA582B9C52329D8C4613C95960246F65817575754173A6EB249C362B571923572C3D2D679E6B985F6D15518D783C829B3727B1BA96C6CA2A55CECE60EAC6755C9F9EC195C9EBB872758AF3F80ED2AB6BC43A2626AFA1CDE18256EA85EB434303AD6E6A568BAB8D363B1D6EF4488E13677B110FFB110DF910097810F2B910F038E073D9E177DA11E3747AB1BF0BA9E1B3187F2585D15402C9E1044652499C1F19E1A23F8A546A14894412E7CE0DA1AFB7176125C4EA6D83DBE184DBE5869F2B59986B75341C41677B07378E2112BFDC93270D7A10F6B9F3C46E073CF656B86D3678B86486D81FE5C6110B7A11E746120BFAD4E76840DA01F605090511DE8381005A59442D9616B472956A6BB3737D7673B3082044E2F67827CE0DA9C47D8887FC24E6607E2AF63A61A7F5CD66135734234CDC308CACF85ADAAFD556B2F0CA5159C1E5B4BC4C85B4AB2A2B9816ADBA00E9F5F5A8E70663E2A264369BD59C7ABD5E04820AC2E128DADB3BA93891572CC431C50FAFB38D919AB823B5D16E17823E0FB7460F3CB4BDD5D6C22961E12AD4C8696264C597A945A8D7EBB950703D66019958BDD666E6D26E87C3E180DDEE52178D268B051E371D0D45583345ABCFF6A03D4CBB4201345B1AA9DCCB5CCB9EECE19EEC46D0EB82CFE3848BC1D869BFC1604005153E76F8308E1C398272CE753D1DB158E9528B0DAD6A01F17BDAEBF1F8E0A3C57EDADF6832734F0ED16A120F0D43333CD0878E4890C9F7A2D56A5695CB8120C4E710D5063D6EFE9883B839984394DBD04C926A59D389A6A626CE512B17079BAAD4E9CC1794ECC93E9F4F2555987F5BC1F2783C8EE104152707FAD11155684D0B7C6E3B0B8DC4CCB79C441421A6DD7EAAF632008FCB05B793B03BE024895DC0EA75A884EC135237BFF37AE0F5E58903240E85422CBA3C797B7B3B86A5B89283FD3CDE28B0981BF26A697994F7FC1188C54672858A03240F78A99E017825009783764A0A0A605B25E5914748FD2A298F3D2CAA50388C6834A6A6231E237182C59512626E044D3CFAC499EBA2D511E52B623983499129526C0C42CE537EE65D0E091EC24B4221F54A60FCA668B1A80D72FE8A62398735731189C7E2482698E3D4601F8915589B481CC91799D82DD34BF2AC5A5E542E0740B15EC839DFFDB4D4473221F44A5BECE53701BF28E59CE61C97FC0A71989B8DACD5F1681C23C3241EA16221B3355B58DD8A6A7544884575815C14ABAA1940401DB8A05EDA6A2EF3A47E22A8DA5B2414B561CEDF301547D52D31CA7B2A4962C9B1C2E8DD0E461396232E49491C56894928AA0562B98AFCB1D7CF40FCD2A6AD4135283F9DF1E5A104D42532C2A5334C62B139C2A3AD9B274C794EF2A4AA490CF6B258B860905C72AC928A6242CD75A1C22508B9E78B2E4FA6A8846C3300A5A854559BAFE408B7D408FF494468738C4A65CD96F3758A077D4D62A08753A399645C770B155D84D82D44A778209C98B884419EA78B8188F57257F81C5453C24149A8828387697598C461C9AF581D25310FF652E929513C3CD00B675B737EFE4A7E5542828309B1587DE1C2180F01DC6FEFEDA1BFBF97560DE1FC79EE48C9049144FF997E1EE84731C46A3DC7C521245633BF826824AF5888030CC82FC4C3241E1B7EF90B479B95A4242242057B4B1613E3172F608947DBEB376FE095F10B2A2E5F9950F7E4C929EECD37A7313D3F8FF93B69245323AA7A218F08390B5688A3CCB3A422C0E3F285B1E4179AF1D101B7CF65FB5CDDA13877D5829222926D52A611A74ED7A993E8EB39CDFF48B285CA74E3FF2051236D16648477D9B7A392536E8D52038A90B35F25669E63240EF15D2CAAFC6D6666EAC8BF01C5C9DB9D67BAD1F00000000049454E44AE426082', 'MS16A On 2', 'png', 30, 38, 96),
(108, x'89504E470D0A1A0A0000000D494844520000001E000000260806000000D3192B620000000467414D410000B18F0BFC6105000000097048597300000EBF00000EBF01380553240000004174455874436F6D6D656E740043524541544F523A2067642D6A7065672076312E3020287573696E6720494A47204A50454720763632292C207175616C697479203D2039300AB04558930000001874455874536F667477617265007061696E742E6E657420342E302E338CE6975000000AEA4944415458476D57596C9CD5159E9004686920781DDBF1783C63CF8C3DF6ECDB3FABED4088D700B1E3F18C1D1327C4243876BC24F1BE8EC7E335064A17F5A12FAD54FA8254212121A4F6A155555588A8423C54E5A14F48455402B50FA885AFDFF96789A918E9D37FEFF9EF7FBF73BE73EE321A00C788E3399C38824789C77378EC08649C7C734CC35FBE7D048F7C078EBE7B9A70C887D22822F264DF2384E0FB84BC13E4C9E52963F2CE9DCC419C91498F3AAF121E714EFA62973127C5F844AE2390C965E27C5F2695BE3C8F12E5A39777F976DE2979FF2DD2DC536CF9B9ABC556F8C9A0DC8B9390AE6A7A3849FE277D22EF583E1D79558EAB63349AAF89CFD857653EF29DE071E91F35C844E2D5E34788A59F974FDE179C9036210EA8CEE5A1BED368FE4AFC571DF85DBFDC874F1246A289B01E21ADCBD91BD8FF9CF82467936FA4164EE5DA8F1DF9C640888395ECFF9D7897EDFF57EC31B5F1E9A79F9EFCE8A33F3DF1973FBE5FF4CE3BBF2ECA4DF2D9C71FFFF929DA9FFCDB8307A773B6AFDE7BEF37FA9FFDE475C7FE6E3A7AB09776FFEA173F37FCEEFD7745A17FCB98070FFEF0830F3EF8FDB19595452DFB1F119FDD1C1B73643299631F7EF8DB87AABCFDF62F4FEEA5572D1B2B73A736D7174DF2B1E0B5FDAD72E2D46BFB9B1579DB4E66E3D1EDAD8DE3B7C76E64A323465FBEF29FADCDF593E9CDD5BA9CEDF34C7ABDF6D51BD74D5D9DED42FE0DF1E5ED895BDAD5A5C5E387FBBBA56FBDF5D609CD4E3A5575397EF19B64DFF3B83AD45F98706AEC1AEE8C8F6276FAD5872423435F4E8FBFF2AF7CBF60BF36FCD5E564E2937CFFFAB59770753881A1C4C3F992FD7DE8EFEBC5D060F2EBD59595273437AF5FC333D1105A830144146F61A0E2F32040B445C3055B4FC739F4BFD05DE8E771A1AB035DED1D857E381C86CFED86DF7364BE4000D17014CF9DEFC4D59747BFD1F4BDD0055BA319963A03EA0DFAC240434D0D6A6B74703BEC05DBC8D00086139730D0FB42C1D677E10212FD710C5D1E2ED83C6E0F8CB5321FEB3067B3981B606DB0C2E974A3B7F71234E7CFB5A904956565D09694A2BCA4AC30B8F8E962D89A9B3031365AB0DD9999C4AD576FE0E6E8CBB83E328257AE5DC7D4F44CE1FD6DB63D1E1F2A2A2A0BB6EA333A54574B2006984C16B4777693F86C1BEA6AF4A8D256A2B25C8B8A526DE183D2A252785C4ECCCC8C176C0BF3B3989E9AC4C4AD718CDD1CC3AD9BB771676EA9F07E7A760101258CAACAEA824DAFAB456D6D1DEAEACD8CBA09EDED5D247EA615268301D5556750452F2B485E5E565EF828E8F36271E12EB6B6D6B1B1BE46AC60697101F37373989D9BC5BDBB73585ADBC0722A83D4CE01669796118AB43C24659486DA7AD4D75B606E6CA28276747490B8E3D936980D46E8CE54A3AAAA0A95151524AF287C7836A420B5BE88C3C35D643229ECEDA5B1BDB9AEB653A9356CA653D8C864B0B57F80EDFBAF616E79F55BC4B524ADAB33C1626944A3B509F666479EB81516465C5B55AD467D86515752F68A322DA52FC7D94888512EE1E080116DAE9270039B1BABD84AAF636D75191B1B6B58DDDC447A6F1F998343DC5B5A4128DC8A1ACAABAF31C2682031251662ABB599C4B9883B9F6D81C54839AA75D091B8BAA28AF9AE82B68C9153F2964890932FE3FEFD6DA429F7F6CE26B636D790617B8DB2A7524CC1D616F60E0F71F8E69B985F594330D2CA62CAE5D5688249646655371E95BAF39C101B5143628958A2D532DA3256B79668098718E90A5E7F631FBB7B242076B653D8A103294A9ECE6C22B3B78B37DEFC217EFCD31F61793D05251855AB58AF67C42A7103CC961C71930D9D525C9DCFB5C22CC4CCF1994A1233BFE55C5AC5C5C528292E4130A860656D1E995D12917096853547CC2E2E62667E01D3F7E6B19A4AD3811D6CEFDFE7BB6578FD615456D540A7CB15569D391B31D7B1ADC9CEAAEE80A6FB3C890DB56A6EA5A2CBB8968B8A8A70FAF469E269284A008BCBB3D8DADEC006F37A6F7E8E64F73045C2F199BBB83579078B2BEB48A533D8DEDDC7BDB945385C3E9497B356B8A4743A3D2537929C0546F22616587B7B3B3417DACFC261B57097A98541AFA3E4CC7325F3AB2D272A108B8531393986D5B545ACAC32CA99698CDF9EC0C4D4346E8C4DE0C6CD71AEE3052CAEAE11EB181DBB8566BB135AA917EE0F0603A53635A8C5D54C991D76173A25C7F18B5D88857C8804BD08FBDD087A9DF0BBEDF03A6DF0396C8872393DDFD38EE4C0458CBC94C450328EC4401C83C9042E0F0E72D31F42323984783C814B97FAD1DDD585901264F536C36577C0E574C1C79D2CC4BD3A120AA3ADA515718ED3C45FECCC9206DC08795D5962971D6E5B135C562BDCDC3283B447FC1E44031EC4789044035EB51FF14BDB4F5B805010E633E0F7A38945D4686E441377A9E6661BF76717FC5E3F82246E89B5E152BF4ADC8D58D047624EE663C41E076C94BEA1DEC81DAD16463DF75856FC19CAAFD596B1F04A50565A82D2926215D22E2F2B655AB4EA06A4D3D5A086078C919B527D7DBD9A538FC7037F40412814414B4B1B238E672316E2A8E283C7D14C4F8D3C919A29B71301AF9B47A31B6ECADE646DE492307317AAE332A965C517AB45A8D3E9B851E8A167011959BD9606E6D26683DD6E87CDE654370D93D90CB78B8A06C3AC99BCD4173BD112A25C413F1ACC758CDCC35C7B1064BE159F0B018F135EB7034E3A63A3FC7ABD1EA58CF091E3C771E2C4099470ADEBA888D942951AAD68520B88E329AFDBED859712FB287F9DB19E6772905293B87F009A81DE6EB486034CBE074D967A3572B91004D90F32DA80DBC58F39898B93D925722B1A4852217B3A613299B8462DDC1CAC6AA40E47B6A0E44CF67ABD2AA9C2FC5B7392C762310CC41971A2B707AD1185D234C2EBB2B1D048CC7C8718B522C494DBC7A83D74C0ED74C2E5206C7638486213B07AED2A216D42EAE2388F1B1E6F96D84FE26030C8A2CB92B7B4B460408A2BD1D7C3EB8D0273BD211B2D258FF029C422B744AD30623FC9FD1E464F073CE280D34E39250539B0AD92F2CA23A43E95D487008B2A180A211289AAE98845491C6771258598078189579F18739D973AAC3C240ED2092932458A8D4EC87DCAC7BCCB25C14D784828A41E718C63F2124BB401AE5F8958EE610DDC4462D1181271E638D9D74D6205161389C3D92213B96579499E55C9F391CB0550A41772AE771F25F5924C083DD2167939C6EF9348B9A6B9C625BF421CE261237B752C12C3E000890719B190591BCCAC6E45953A2CC412758E5C2256A3A6037E75E25CF4D256739925F5110155DE3CA1441BE2FA0D31E2887A2446F84C26482C3956E8BDCB4E6F4272C5252989432A3109256A8148AE227BEDF5D1119FB4296B4075CA4765BC59287E758B0C73EB0C9158640EF36AEBE20D53FA09DE5435F1BE2E160B370C924B8E5552899850739DAB7071429ED9A2CB92292A21DB7440C947AA469BADE4308FD47048F21BE29D3AA2EED972BF4EF2A2AF89F776726934908CFB6EAEA2F310B985E81C2F84A3A3D7D0C7FB74DE11915E9E0AFB0135259C94842A3879885287481C92FC8AD41112FB036AA52725E281DE2E389A1BB2EB57F2AB12129C4C8845EA2B57867909E0797BB08F9E9E2E4AD58FCB97792225E244023D177A78A11F423FABF5123787A048CDFC0A22E16CC442ECA7433E211E20F1F0C08B5FD89B2D24251111CCC95B909818B97A054BBCDA4E4CDEC64B2357545CBF31AA9EC963E33C9B27A73075F72EEE2E2C22911C54A317F2B090B3608538C23C4B2AFCBC2E5F194E7CA11919EAB57A9DD67FAA2714D7AE5A505244724CCA32E2D2693F7716DD9DE7F91F498E50596EFC1F24D1489B0519E653CEED88E49447A3D48022E4B4ABC4CC7394C441BE8B46947F4C4F8F9FFA1FB31269826EFD2C2A0000000049454E44AE426082', 'MS16A On 3', 'png', 30, 38, 96),
(109, x'89504E470D0A1A0A0000000D494844520000001E000000260806000000D3192B620000000467414D410000B18F0BFC6105000000097048597300000EC000000EC0016AD689090000004174455874436F6D6D656E740043524541544F523A2067642D6A7065672076312E3020287573696E6720494A47204A50454720763632292C207175616C697479203D2039300AB04558930000001874455874536F667477617265007061696E742E6E657420342E302E338CE6975000000A9C4944415458476D57496C5BD715556C374E9A3476AC899A2951222991E23C7D7E5294ECC48D4667902C8A94AC5871ACD891256BB0AD819A2989B225D9499A0EE8A29B2E8A2E8B00058200EDA24151B4698C22080AA4E8AA8BB6280214455B046D727AEE27FF971254C0F17FF7BEC777DE3DF7BEC145008E1B28A229004E108F128F1DC1C902A4EF114111FFF43671AC00994BC6E8383AEFE3442DD12A968530118F1D1920045F279E264A88D384901F9D50F0B5C257C884545F84D87AFB91AFCC2B011D134B484F1EE9940E5D0599586C59A910E811894FFA0432B9F4E90B33167144916385F9FFCB760D61928EE3DA8043E23CE1A17D3492FF07E99785689108BE325F7EDCA12DF33F26AD478F38F5088FCAAE4F2A519F22442E6D72FD8FF69716A3F98A8AFE26BFD70614FE687C467CAE8DE13F7619A00170128E2376232135D05CF089540942722F9085482DD4B3EF1FC4FB6C9F21248046DAFF227ECFB6A4E009B67F43B0896774493EFFE8A35F3DF9BB5FBEF7E43BEFFCF84CC1F7E1C71FFFFA1BF43FF587870F4F177CFF7CF7DD9F9CFAFE77DF7C62FFDE76F0606FDBFFA31FFEC0FBF3F77EEA61DF17C4A70F1FBEFFF8071FFCE2D8EA6AA68AF66F893F5E9F983895CBE58E7FF8E1CF0C2575E2CF365717AAB7363211BDE38DFD9D1AE2C937F6B7CCBAEF6E6EF3D4EECE66E9CD896B9FE8BEF1572FFF7B676BE3A9EDAD3513EDCF893FE5B6374CAF5FBBEAE9E9EEACA3FD1FE2939B5337DC6BCB19ABFC46A013233DF03C5E1919D4DA8299892BB835398EF9D9D70DDFF8D80866275F336CC37F65F48B4BE9D49F75FBEA9597F1CA680A23A9C3F9D28303181CE8376C83B8231A415C091A1D4A28800871B62D66F8FABACE63F0855EC3D671A1A70B3D9D5D861D8BC510F2FB110E1C992F12415BACCDB00D627B63039A1ACC4647435D1DEAEB6AE1F7B80DDFD8C81046531731D4FF82E11BB87001A9C124462E8D1ABE803F004BBDCCC7DA2CF8ECB666389A1D866D10579695C154528AF29232A3B3F8E962B85A9D989A18377CB7E6A671E3F56BB83EFE2AAE8E8DE1B52B5731333B67F4DF643B1008A1A2A2D2F0D554D7A2A6460269307C45CF9D3B6B1895E5265494F2202BD8A5674A11F07931373769F89616E7313B338DA91B9398B83E811BD76FE2D6C2B2D13F3BBF8488124355250FA882CF5C5B8FFAFAC3E83B3B7B48FC4C87E1A8E22A2B485E5E566EF8A2A120324BB7B1B3B381CD8D756215CB99252C2E2C607E611E776E2F60797D132BD91CB2770F30BFBC0235DE6EFCDECC281BEA9BD0D464377C5D5D24EE7AF62C6C0D3C230ACECA8A0A925718F6395541762383070FEE2197CB626F6F1BBB5B1B5A3B9B5DC7D676169BB91C76F60FB07BFF0D2CACAC7D89B89EA48D8D56C376B77A74E20ED81B0EB5AF66D49526465E66A2F4E538175719E5320E0E18D1D61A0937B1B5B9869DED0DACAFAD6073731D6B5B5BD8DEDB47EEE001EE2CAF428D75A08EF29AEB2CB03490B8C97684D89D27EE7EB61D760BE5A8E13559E8AC3255C154C6C829797B3CCAC95770FFFE2EB629F7EEDD2DEC6CAD23C7F63A65CF6699829D1DEC3D7880076FBF8DC5D57544E31D2CA67C5E1B2D87D1B6B43859AC3AF17921B6A0EE08B189D196B1BA4D447B4C65A4AB78F3AD7DDCDB23017177378BBB5C4096926FE7B690DBBB87B7DEFE16BEF3BD6F6365230B25DAA655B1D9CC88BF4AEC74A15B8AABFB9B1DB00971750DAA2BAB296F058BAB0CC5C5C528292E4134AA60757D11B97B2422E13C0B6B8198CF6430B7B884D93B8B58CB6E730177B1BB7F9F7D2B088663A8ACAA436D6DA1B01A6DB0711FB7701FBB9C6E5675178A7A9F237143BD965BA9E832EEE53367CEE0F4E9D3C4D3509408322BF3D8D9DDC426F37A67718164773043C2C9B9DBB8317D0B99D50D64B773D8BDB78F3B0B19787C219497B356B8A56A6BCD94DC4272AB7688381D4E1277A2E842E739781C769E32F56830D752F26AD45432BFA672A20289440CD3D313585BCF60758D51CECD62F2E614A66666716D620AD7AE4F721F2F21B3B64E6C607CE2065ADD5E98AA384F9D190D0D5658ADCDB0DB5BD04A993D6E1FBA25C7C9977A9050438847838885FD8806BD08FBDD087A5D08795C68E3767ABEAF13E9A19730F6721A23E9245243490CA753B8343C8C91E111A4D323482653B8787110BD3D3D509528ABB7153EB7073EAF0F219E642ACFEAB81AC3D9F60E2439AE28F962779E34E2871AF4E5897D6EF85D4EF81C0EF8796446E98F8703688B0490E045D216096A763C2CED307D1142418CDF48380C278BA8C5D60267B313ADAD2E78491E0E861125717BE22C2E0E6AC4BD48444324E66421461CF0C045E99B9B2CB0527E8B99672C2BBE9AF29B4C652CBC12949596A0B4A45883B4CBCB4A991613AAAAAA98D33AD4F182B1F0506A6A6AD2721A0804108E2850D538DADBCF32E2643E62216E534208785AB9520B6FA456CAED4524E8E7D5E8879FB23B1D2DB0D96D3C851AB94DEA59F1C55A11D6D6D6F2A030C3CC02B2B07AEDCDCCA5CB05B7DB0D97CB0B87A315569B0D7E1F158DC65833BAD42F75A35DA55CD1309A6D8D8C3CC05C071065BE95900F91801741BF075E2EC645F9CD66334A19E1B1E3C771E2C4099470AFD752119B9D2AB538E0D40A88E329AFDF1F44901287287FA3A58977729452937870084543FDBDE8884598FC009CF6262D727910446947196DC4EFE38F39898F93B92572079A495221673A61B55AB947ED3C1C1C5AA41E4FBEA0E44E0E06831AA9C2FC3B0A922712090C251971AABF0F1D7185D2B420E873B1D048CC7CAB8C5A1162CA1D62D4012EC0EFF5C2E7215C6E7848E212B07ADD1A217D42EAE3B8801F81609E384CE26834CAA2CB93B7B7B763488A2B35D0C7E78D025B53433E5A4A1EE75788456E895A61C4619287038C9E0B08C802BC6ECA292928806D8D944F1E210D69A4214458545155453CDEA6A523D146E2248B2B2DC4BC08AC7CFA24986B5DEA9872481CE522A4C81429362E42DE5321E65D1E097E224042210DC8C238469758A28D70FF4AC4F20E6BE62192684B2095648ED303BD245660B79238962F32915BB697E459935C8F5C1E8022BD9073BF8728699064421890B6C8CB31E19044CA3DCD3D2EF9156295978D9CD5897802C343241E66C442E668B6B1BA154DEA98104BD4057289588B9A0B086B1317A297B696CB3C69888868F2EA8412ADCAFDAB32E2B87625C6F94DA7482C3956B87A9F9BAB51E5894B5212AB1A3109256A8148AE21FFEC0D7121216953D688B6A8109509E6A184B52332C6A35325B1C81CE3D3D6E795BDAC22C5976A5172A087C5C20383E492638D542226B45C172A5C1621DF7CD1E5C9148D906D2E40D123D5A2CD57728C576A4C95FCAA7C53C7B5335BDED7693EF48B92FDDDDC1ACD24E3B95BA8681D22B7109DE783707CFC0A06F89ED61722D2CB57A11DD152C24949A88193AB945A25B12AF915A9E3240E47B44A4F4BC443FD3DF0B436E7F7AFE45723243899108BD4972F8FF211C0FBF6601F7D7D3D946A10972EF1464A258914FA2EF4F1413F824156EB451E0E51919AF915C463F9888538CC0585847888C4A3432FFEC5DD6A2729898868415E436262EC95CB58E6D3766AFA265E1EBBACE1EAB571ED4E9E98E4DD3C3D8399DBB7717B2983547A588B5EC86342CE8215E238F32CA908F3B97C7934F5F7A2B191FE9341AFE353ED86E2DED50A4A8A48AE49D946DC3A9DE7CFA1B7FB39FE1F49AE50D96EFC7F9044236D16648C5FB9B7E392535E8D52038A90D3AF1133CF6D248EB2AF2DAEFC757676F291FF019E3F8CEC9EDF77070000000049454E44AE426082', 'MS16A On 4', 'png', 30, 38, 96),
(110, x'89504E470D0A1A0A0000000D49484452000000110000001808030000002BBCE6000000000467414D410000B18F0BFC610500000300504C5445000000EF7F18F17E18EB8D0FED8613F48317F28018F38218F58318F68719F6861EF18B13F48A16F58E1EF1940EF19313F09018F7901AF6921AF7951CF18E27F58F26F79029F99825F89B2BF99F2FF29F3DF5A128F7A822F9A520F9A522F9A028F8A822F9AA25FBAF2FF7A133F7A237F7A239F5A738F0AB3CF8A034F9A136F9A632FAAF35FAB72BFBB72DFAB82DFABA2FF0B43CF4BA39F5BF3CFBB339F9B739FBB933FBBD32FABD34C99455D5A05BD5A46AEEA954EEB65EF2A041F9AF4AF0AF5AFABA4EF6B757F9BB59E1AC6DEEAD67EDB66CFDC61AFCC71CFEC81BFCC81DFDC91FFDC625FEC820FECA21FDC92BFDCB2BFDCB2EFECD29FECC2CFDCC2EFECF2FFBC037FCC235FAC13AFBC239FBC23AFCC439FCC739FDCB37FDCF30FDCE35FCCB38FCCA3AFCCB3DFDCD3DFDCE3CFED12EFED637FDD03EFCD33FFED43CFED63EF6C44AFBCD42FACF44FCC840FCCA40F9CD4EFCCB4FFCCF4AF2C85EF3CA5EF8C25FFAD140FED642FED747FAD04CFCD04BFDD24AFDD34DFDD74FFEDA48FEDB4AFEDC4DFEDC4EF9D050FDD451F9D258FAD25BFCD658FDD854FEDE51FDDD5AFEDF58F0C468F5C869F5CE6AFACD63F3C472F8D263F8D065FCD760FDDD62F7D075FEE158FEE15EFDE46FFDE370FDE573FDE575FEEB7CFDEC7F9B9B9DACACADBDBDBEE8BC84EFBE8DE2BB95C9C3BECAC4BFEFCA88EEC498F0C781F1CC97F0CE96F7DB95ECCFB0F2D1A0F3D1B0F6DFB5F3DBBFF2DCBEF4DBBCFEEC81FDEE8BFAE194FAE09CFEF090FDF193FFF399FBE4A3FAE5A9F7E2B9FAE7B2FBEAB9FDF4A6FDF6AEFEF7B1FEF8B7C4C4C5C9C9CACACACCCCCCCECECED0D1CBC7D5D1CED0D0D1D1D1D2D1D2D3D4D4D5D6D6D7D7D7D8DAD6D2D8D5D4F5E0C1F7E4C4FAECC6F9EBCDF7E5D4F7E9D6F8EAD0F9ECD3F9EDD8F8ECDCF9EEDFFEF9C6FEFAC9FEFACEE7E7E8E9E9E9EAEAEBECECEDF9EFE6F2F0EFFCF5E0FBF5EDFDF9F2F8F8F900000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000098AC6A950000010074524E53FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0053F70725000000097048597300000EC000000EC0016AD689090000001974455874536F667477617265005061696E742E4E45542076332E352E35498AFCE00000015B49444154285363F80F023B9A4BD34B1B373F01B11980784F8B7B4AFD94F654CF8A9D1091DD151E1E99B50DB599815ED95BC022FDC16E6E21196519B1BE3EFEF99780222B7302BC8332EB1AEA326383FD12263F63F8DF531593D6396BEBF66D1BA796C7C5675D66B85EDC9454337BFFEDBB770E6CEA484E30DAC570ADA835AF7ADAB6BDFBF66E9BD196186DB896E146D7C4DCA8C249D3674E9F54991716617091E1E9920905BA3AFAA191E1E67A66A62636B718FE5FE8EA359693559051505054D2565F0D72CF8A6E07714121212161792D35D79B2091ABCB4A34393958D9792595ADD741FCB5BECF8E8B89894944C37629D4A7FF973B89B1B1318AAABAC0FCFE7F83A3040F370BAFCA02B8C8157B697E3E6601A9357091FFABE63B5B5A582D7C8410797CFCD8A2C5E7CE83C3136417101C3D7CE8E8D187A82247CE9CBC8F2272E2C891930F90454ECC9B33F734C400A839F7CE1E3C75EF395804001294046E748694030000000049454E44AE426082', 'Light Bulb ON', 'png', 17, 24, 96),
(111, x'89504E470D0A1A0A0000000D49484452000000110000001808030000002BBCE6000000000467414D410000B18F0BFC610500000300504C54450000009D9EA09EA0A1A0A5AAA6AAAEAAB6BE82A2C28DAAC991AFCD9AB2CC9DB2C9A3BACBA7BED3B3BCC1B4BAC0BABFC5BABFC7AFC2CDAAC0D6AAC0D8B6C0C9B2C3D3B7CAD6B2CBDFB7CFDDBBCBDCB4CBE2C2C9CCC4C8CCCBCCCDC4CBD4C6CED7C7CFD8C3D0DCC2D2DEC5D3DCC9D0D7CDD3D7CED4D6CAD4DCD3D7D8D7DBDEDADCDED9DCDFC0D3E1C2D4E3C5D5E5C5D7E5C7DBE5CBD8E1CADDE6CCDAE5CDDBE6C8D8E8CDDEEBD0DBE2D3DDE0D0DEE7D5DDE1D4DEE1D4DFE3D6DEE6D4DFE8DADFE1D8DFE7D1E3EFD4E3EBD4E2ECD6E2EED4E4EFDEE1E4DEE2E4DDE4E7D8E5EAD8E5EED9E6EDDCE1E9D8E9EFDEE8EEDFEAEED5E3F0D7E5F1DDEAF2DEEAF5DFEDF8E0E2E4E0E4E6E1E5E8E5EAECE8EBECEAECEEE1E9F0E1EBF5E2ECF3E3EFF3E0ECF5E2EDF6E0EDF7E1EEF4E1EEF5E2EEF6E6EDF2E7EFF2E7EFF4E5EFF5E4EEF6E5EEF7E5EFF8E8EDF0EBEEF0E6F0F7E7F2F7E6F1F8EAF0F2EBF1F3E9F1F5E8F0F6EAF1F7EBF2F5EBF2F6E9F2F7EDF0F1ECF0F2EEF1F3EEF2F3EDF2F4ECF3F6ECF3F7EFF4F6EAF2F8EAF3F9ECF3F8EDF4F8EDF4F9EEF5FBEEF6FAF0F1F2F0F2F3F1F3F5F0F4F6F2F5F7F2F6F6F4F5F6F4F6F7F1F5F8F2F6F8F1F6F9F1F7FAF0F7FBF4F6F8F4F7FAF2F8FBF6F8FAF5F8FBF6FAFBF5F9FCF6FAFCF7FBFDF8F9FAF9FAFBFAFAFCF9FBFDFAFCFDFCFCFCFCFCFD0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000004BFE1CC00000010074524E53FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0053F70725000000097048597300000EBF00000EBF01380553240000001974455874536F667477617265005061696E742E4E45542076332E352E35498AFCE00000015249444154285363F80F024AD61656569A861E20360388AFE49F949C9299EA6118091151B22F6ACF4B0C49C8682834AC04892871572E98DD5C909D97513EABC6B00A28C2ED3E6FC9C2E96D4D8D4D1533674618FE6750E2CE5DB064D1DC9993264F699DB9B80328C2ADD4BB68C9C20533674C9BDE3E7F495FA82103B75AEFA2453327D456D716372E58D2E766C810C69DB660CEB4A95D5D75258D0B16771A1A024DB66C6DA99FDA3FA9B5317FFAFC28A039FFD5C57CE3627B263437A6644E9F6808724F8EA084A373596B695670708B1F580408C4F5522755C404047A030520229CC2762DE92EA6664AE630112E76C98C0C131D03E56E9888289B505086B1BE91152C34FE2B7248C55778B93A85C3456C7974A353FD3C6DC0A10736B98A55C4C14785571A21F25F46554545814F034984DF4D45455E401B59444545408E1F5944855F408099450B49CD7F157E462659B0C07F0055E6E542172465430000000049454E44AE426082', 'Light Bulb OFF', 'png', 17, 24, 96),
(112, x'89504E470D0A1A0A0000000D494844520000004B0000004B0806000000384E7AEA000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000001674455874536F667477617265007061696E742E6E657420342E303BE8F569000013B249444154785EEDDC0B945DD559077008218124E4415E64924848802424992440024D6231096D12D4801828B11502060D1631141A0A3EA8566BD14A6D455B5FA56ABBEA034BB55AAC5651DB8A85D62E54AAB45245EBE211D420D2421272FDFF66CDCE3ABD9C99B9F7CE1D8CD2BDD67FCD9D73CEDD67EF6F7FDFFF7BEC3D73D411DA8EEEC7A8CAE7AFB7A64628C7073383538205C13704B38269C1A4E0B86074F0B21720416D9C3163C603175E78E193EBD7AF7F6AF5EAD58F2E5CB8F0C169D3A6DD73CC31C7BC3BF7BF3FF8A6605E303520BC97A5E0A6046F1E356A5463C284098DF1E3C737264F9EDC88F01AF3E7CF6F2C5AB4A871CA29A73466CE9C7970DCB8715FCAB37706DF1C9C141C13BCACDAC4E0D5C1EF060F05FF1C3C11FC57F042D080A38F3EBA317AF4E8BECFC16381EF9C10D030423B36C077477433587C3226E864C0264A4BCE093607170697069707DF17FC7440907F1BFC47B03FF842F06DC189C1F8E0ECE05B029C4780479CD0080977F404EB83D7041B83B9011E6A87534C6E5C303940EAC85EBF386A71409034697BF0FA8020570604B32266FAE8AA55AB0EE6F35F05AF0D8CE188E135DA706238E6C22D5BB6DCB77BF7EE03175F7CF1A1FC6ED5EF0E7A03026BA599906769C53706E705E7064B025E91E00810B1FB4C10043921A0596FEFE9E9E9E3BB7C0663F860B026C087FFABBCC6DC16CC9B37EF8E9D3B773E170FD69832654A235EAB8F5772EF50F0B301AD68C51C9870EFB1C71EFBF9AD5BB71EDCB469D30BE79C73CEFEC58B17EF9B3D7BF6C321FC0FA7DF1FCD33DF1A082B701C01F8DE8ADC7B9C73C8E7663C1A7C7740B8C6FC92361A801FD69D7DF6D90FECD8B1A3CF63E5F73A20E85705567FA83636383F789AB00BC68C19D39838716283D6C41336221066767FB036D0AFF8EBD65CB738756380E78377054C9999BF246649432665F55F9338E889AC7ED5330D849F0F5AD12E1A727A705DF01BC19F060F045F0CFE3D3810943EBF1A7C57206025802F116C7E0E06C2FC48B02A20E4111598C99E3876ECD86B366CD8F0ECB265CBEA0654877F0D103F6D1CAC158D9D1F9C15AC0B681AB3FB8EE086E03DC1EF07BF146C0A98D61B86D0AA66FC45800B4BB8D1F5A6D3C9D1A2D7AF5DBBF6B9F054DD20068289DC16CC0986D22EF7913CC29E1120F2E21111FFB200619BECC901AD7AA805AD6AC627039EB5EB1AA6339DBEB6B7B7F72B21DBBA970F05A664924379469AF5EA3973E6BC233C7847CC1DA15F15F08E84353DA011C20104BF335A7538606D131F0FCE0C5AF5D62D3503DB70F2C9273F15D4BDB41598D00F043464A095A4550BA2250FAF5BB7AE71FEF9E737FC5CB264C9A1E9D3A73F1FA17C3EF77F24C06B489A667D3AD7EBDED70A68FCAF070B83AE784991F8E278A307172C5850F7C276F060805C79BDBA260C20883F080E933913AB08E4B9E08A80496FCFF52AE97702DFFFA1407FC38AC368C0CCC44DEF93C876C00BCDE0F2AF0D70519D76B926E014FDBF21F8B9E0C3018FF8E5E0E9E01F82CB02C1EEC787A15555F0B4170702D781B47EC8863F7624B63920D6C9E76EE0BE60453090DA5B5DA94E2173244C7826235490C220F76D593C5A56F78E4EF08940DA3490D60FDAC43C8B13267C4199249FBB05C1E18E80060DB48ADE8DC4AD34F08C887D763F44EF777749AB0A68FD9B0289FC501EFB45CDEADE76C20927D4753C5CFC517046800F9B1B8D5B3C6EDCB8374D9B36EDB668B409A83CD04679216D5F1FADFAEFFCACEB7B3860EEE2BBB6C8BE2FD70AF67679F50A4C94002C487373ED76A9CDC68D1B1B2B56AC6844703C29CEFAF6800378EF088DEBF14045A395D4EC70E396259E751D760BBF1370D916A6DA98DCF541496F4A64CE7C69D925D1AA7D5D703675501B6B358F3DDC084B9AF1D960B8AE792028DE29D2092CAB4D4CB73490DEDC14FC4C20167A7BC00BDE39425AA59C734720486DCB0C7924444A2505819F0BAC6CDD4B86033574645D8D6F90BE885A8A234754CB52FD84CDD1A827BBAC5534576544CEA93ACB91B41C3E1838CDB2E20895A910DA9EE02F03D97EDD4B3B41E18852FE45F85CB7F723723F691AE189CDDED24285A355141E7C5FB033904EF184CDB43060232859FC8D81285975D240098E066C08944FFE387826A81B44BBF8E5604572C0DE78DE2D93264DDA19EC39EEB8E36E88B919033A581621BD72C284098F75415842847F0C948DF4AFB2A17C84A7DA0A19ACF09E44EB54735F805314D6746285A9E8698155E00044D7FF19D40DAA254420FBCE3DF7DCFBAFBAEAAA7D575E79E5816DDBB6F5E5846BD7AE6DAC5AB5EAD0E9A79FFEFCD2A54BF75E77DD759FC973B57DB4089CF47701EE53BF17D84A71686FDB71153B95E4DEAB2C9C9FDC3B42658A84A876B43B10E596FAB7E8DA86C1FB03765F3C57CBC03F4AD1F608FBB9481F56BFAF2F64EE5E62AE6A7ED80ED086AAAAEA85D0431CC5DC584CDB422A8D0922D3A2295415E171E5CB67CC98F144CCC30410E1A9017E619E3E5365DECB3D85BE764B269CC7DF04BF16BC35F8C1806351E013283E1BD47D6F3058EC3F0B6E096CA9C92559064EEC5848A5E944FE55B4E3A3C1EA40BA714D6513E2B7022FA6BE72B71F0E982B4173BB56EF1D81FA150DA94EA00E621B82F1EE5704227BA62EF81418CB0B7705F704AD7864F4A16AA1AA7A41A03FA99570A0E324B9B931352B5A5ECAB60D981742C2E5FA4F0526C3143F70EAA9A7BAF6998060699A9AB849BF2ED0870DD1816235DF53D82364E68E1769B895F7936742BC4C1E4FBE25507968EEC702EF0DEE0AEC250A2C79710BDDB2776BA719ACE0CFCB69C4D5010EE3113F15B86E655D478C8B82E29DFE293040C29A1B0DB4FDC525E33C1A631108AD6A9ECC95CBD60FFE186CD509CF620A561510AB1A86366C6E18973A7FD9221B112169064A00F7060620D5B0254E7D4D463CE2FA9301FBA76D5B823279453D1BA2F8EDB2B973E7BE10215A6D757713D4875D6A4EC0F316C33D644B50D54630F8109AB9C5B3CB039BB7FAF18E5F09CA763D2DA49123DA0CCA8A28AC19C423014D91D4F27825C3775FAC4558CE1EB8067F1898B8EBB7F75F0326AB5F9320FCC261F8ECA28036178DF293F62C0A3F6ECD4FF195085E705AD53AA645634B2DCBC68377775487EAA41196D5FFB7C000E4843630A9B308BB4C9239E22342F9F1FE6BF0DEC02E0BED7470C3353CA52A2A6D222CC16C711E1F0884202507238C89090DAEBAE0820B1EDFB973A79ABBEFDB37B4C94188A5D138DAF570A02F14F0CAA059435B6DDE5D78B2DA5C33BE17692AFBE6D99E0A0C80CB552B37481EA5981B0D42C684C5E3B906DC3D2DB0816082AE49942F099832F37402A63CCF5379B60C900056256ADFAB24939FD512B6B30A1C4DE12093C3A3BF17B8CFFBD1C422507D121C8DACF216E7611C34B37846F78513BCAE289E87D76829E511BC7214E57A5FF325ABE5AC9301D00EE181173287A2F2125F2102F3F9B1FE6B34E09A8006D1AC5F0D5C974732597DF09CE53A7216C81278699EF99EA0685E154C161F16CD314941A540D87D3118CF4B1004E59E128F0541010443506BC2A304CC224A9C38258BF2D6891327EAE3172AD76745CB3F9640D878C47A14E1B0867989D5A3514CEE2703E10109D338B195A0D184ACAA5514B0FE79C05DFB8CDF00F1D33A9ECEAA783961154F2BA2B66234AE34E68E03EB84C5DCF027816A45583CA0FB262AC7A32104BAADA7A7677F78EF2BF92C50F69EE911CAFBFB77A768A2314AE3E6E7FA43CE51E4B36D36D4E33DE6BC37F9AAEB161D279A475F33002FA3CE6A48521BDA43885E2642778F09EA8C26D222B10F30299D913EC1F08056C9B3FAA645C2092F67D26AF0DE579A497A67A18102C2A341D5ED33FD89BB6C7C78C6E479DA62626FEB37618B4EBB3CCB532A33B96EB13C6F7EAC8987779D5313005B703C59C213E7BCC490855FFB5AD100AA6B22C5DE09CCA40DC4A40C562318B60C556EF0BCDFABC4E8BBAA9C457398306157FBA2EAF6EF0CDE4469C69F04CACF4C5CBF9A67ADB4F0465F8E13D114DA69FC927BD725CE1C8CF758BC12FEF0ECDB02732494E2E9FF3E60EEC53ACA58599B5ADAD7084B3378832A93E856236CAB59BCAA788EAA1F56ED3442C7879C823D433CC8B9106275A03C2B21E8076818F3B190485AD2EC3A8D63EE4C96072F42C1CB0E9AD07631631993EF119E8515E096FEC57434B0AA1023D6081EC721FB12941AB889309DB2307E3235E6C1AC998F81570749AB4E0D1F3DD85F19B1FA92776642884CDFF905F73E1D302BFD5573DEBF0E1C0DA759AA28AE81A34825ED52A128D7113FEE2D9A3D22CDE479A19E193366BC6ED3A64D9F5BBC7871193018B41527C8664DF67BF335839D1A21DD367BF6EC4395D0C24966853CF529E6F69DC13B832B83921FD2D4F2BC50448C47E3AA39AFFABB70A1EA3C8C173558BCE6F174A59914933A3969CFAE4B2FBDF4819B6EBAE9D0860D1BFA8E4FE67A159F080494346C3035776FFAF8F1E36F58BD7AF5F3EA5BF9BD1938CCE47940E18E6A036DC365CCD6C491F61B03A6CD64699FEF0A7F841B3C7DD519785E58520D73BAD29888C19D9E76D3E5975FFEC55B6EB9E590BDBFB163C75627D50C5903F2A7154C431F341270DDD4B8F095679D75D69DDBB76F3F38C871CC02264E7BF01D72E7CD699D70462AC6345DAB923E1E63AADECF7994CA060F599C47579A55776CB277F9F2E53F71F5D5573F164D6A4403EA346940E4FB4F44A8778D1933E6D6E38F3F7E7702C2EBF577C925977C74F7EEDDCF46F88D3677C769051EE24D11348FE6278F692144ED622B1A670F13B9D37067294A1FEA6272E31779C2761AFBE5C9A624357945DABBAFBDF6DA7DD75F7F7DE38C33CEE8A8FCEB3B4AC7BDBDBD7D35F8AD5BB73692273622B0BEF4A7EE3B2D828931777B06384C306AF2788860DE1C48F239141A57725E4214E208D63B227742F2A269897A5FB57EFDFA0F46405FDDB56B57C3B1C90AE9B60304AD08C8530A18AB4E4000CBACFE25283CD229F4A50F09BDA41F91131861F86C5E84756BE059EF14C630CDB61A2151DD59D3A74FBF78CB962DF7ECD9B367FF15575C31D891EE56215D42A2E230E5E39B0389B915964D48B1DC933E95D06338B018F24D45445E9130842CE6882F652B04F6BD415B479074E0E178EBD9975F74D145F7DD7CF3CD076D574D9A34A96E20ED42B4CEC58B8F0C1A21CB20AC3CB8CE3CDC634248BAAA79C3817E10BC22245EF20E5E9C76D136E340EC64D0521315AF098F7CEAC61B6F7C61F3E6CD4EB4D4BDB85348D00588CC0097F07E168749F0AED56CC23D214137B4AB197252C16D190B2BF2FE969B8717C43B7DF6BCF3CE6BC443D5BD6438503B17FF5C16627FCF942953DE1592175173F92A1ECDAB4A7062202593BAFEBA012103D3AFE6A22D352A794548BB1A2D7713B6DAB8F57B972E5DDAB7039DF84C0821572B955A1C526D04A8DE3EAC5DF02120D7FC9A924C2B4DF68DE8EA3A1C2E78385A257AB7E1A02655E5225E1197357B22311DAD2BE948B7610C0A9C62B0B684651579A25252EE26541D24D6085D7E2806B2C74808AAB4CE6429F8E1B1E66611696427BBD4434124AF8ACB0C5B26760DD1F212A2DCBA8E3B8512B5A21C0D41A404222793CB89B2A5265CB6CDDABA7CD135CF8AAEEBFAEF1494C25172150C8EADAD86E048D8C444C1752FE804EA46FE089CB008A76C164843B86D55029EAF4E50A5494B9492BB79B41B4F8AE48DA32DAD2A8D7659E50F05752F6817846E779AF6DCBA70E1C26792203F336BD6AC47E20D3F128F6B070897D959B61B3E504E864F6CA84863EADED32EEC44F1C462BBB6B8AADA48D84ADB9028FB74C3814D8F7288E42E89B68A840479CE9C397DFF8AA07FF3800BC76303ADB26BC8DFB986E16ABDF2B33FDA9448377BDFB69B788B39F24ECD1B0AED4048208D511A11F8D991FEED401984700C9A3702DE5082ABEE3450BC43EBB978E653F7BE5680A7948D956108BF23F36B6E5413C708D83AF542E55F0A28DB322F66864C699A93348A724EC9BC2D707E55B4EED98126E03AA17BB6138F6D516CFD4B96A554837164DB8DE7A2152664E7A56E0003C1C0947AF19F9004415B49C25085A4B9B44859B7E484C87EA894C398841E9D50048D94C0CB013BE6A9C19AA81EB12AE8B7730057B943E54088B06EF2E4C97727C5F9D0E8D1A305BD765A6C1418346D233C3C4910439945F1D8C66341EADEDD0CCF2909F1A61266E63C22CDE0B97502633665837230189C8323364AC54EB73B432AC5F16779A79D76DA0BC1D30B162C7864EEDCB9F787E0FDFD33AF28DD11830D25300B280176DEABEEFD55E04D673114FD08CA828C683378419B9209D2770278B0555525A0EEB486C6D87DF14F77107ADF33B6C979C1934E3AA93175EAD4F23DC722059F437109ED52C41BAA7C833AEC6E3BC6498B474CA3EA1A92364804FD9B010F5637486518BC42C0BE63A04C0F31CBC3C44A3CA2DD19CEC3A424CABF18B49AA371F9D22745BDE6F713A07F00C4541D9F92628D08470DD5AC3A9216649ABCC36E55CF24D853E12CDE86B0980D7247E8089F09957F45C02BDA0D7616426EC86C5B29957886107CD7296BB117932374A1813FEA4403AD388D116D85C7841626ED1F82D9DEA2690EAD3938827B66250895E6A857D9BA373982E619FD1406206B202409733BEEDC42588072B8459C26B01582A883D1ECA1F8EF256B45CB988E8317D216823179137FE3BC79F30EAC59B3E6C0CC99339F1C376EDC2723BC778E1A35CAAADBCBB34D55CAC822773BD5ED6A01F3227CB9A6C5D39750A51DA1BF64CDCA214E421326303713F0BBB3EE076D79497110B91DA1254B963456AE5CB9FFCC33CF7C7AD9B2655FEEE9E9F14FC63E16304582EEC46C98A5EF1D319A345433D03258042C14B039E07098B4E9704EA712EB0FD66DCBCB13FB2BB3489F09B75D2EF9BFDEACB4B04174AEA2E0B4B138CD810E719520513A64B745018EA09CA3B72DC521BCEC1A2DC31D9C01B3C42B885EB6CFF309419C372048955042EDD40CFFDFB5223C1C47803C26ED2B3923D3FDBAA086684728291F75D4FF00332C54E471D76ABA0000000049454E44AE426082', 'Fan ON 1', 'png', 75, 75, 96),
(113, x'89504E470D0A1A0A0000000D494844520000004B0000004B0806000000384E7AEA0000000467414D410000B18F0BFC6105000000097048597300000EC200000EC20115284A800000001874455874536F667477617265007061696E742E6E657420342E302E338CE697500000169F49444154785EEDDC09B49DE55506E03226B949C81C86849039844C2484240C4988D410424408434A0790A988A1A0D5A2A5ADD6564B11288ADA49D48A55D08A945A5C7509888D9422430BAD690B148184410A04681292907B8FEF93C5E73ADEFEE7DE736F0E83D26FAD779DF9FFBF6F7F7BBFFBDDFBFFEF7DDB1B3C76E9F4F8D3D16930CCAE41DF3DF6D863401E0705C3823D82DD82B7B4E12C9E11F60C860D1D3AF480E1C3872F983E7DFA3B264C9870DEB265CB3EB564C9925FCB67C70793833EC15B6EF09EDD77D96597BD76DF7DF7B1C1DB67CE9C79C9ECD9B36F5CBE7CF95D4B972E7D74D5AA559B8F3EFAE81757AE5CD93E7FFEFC47FAF4E9F389FC667CC0B0CD0EE761E07E41FF60F7E0FF8C779AA8901AB1DB6EBBCD1F3366CC45071D74D0750B162C7870DEBC791B172E5C589B3871626DF0E0C1B5FEFDFBD7060E1C584B38D6FAF5EBD791EF3F92DF9D17084B46686630D29C1CEFC2A953A79E93CD9990D72383BEC19BD66826665707EEBAEBAE33DADADA2E9A3265CA5DD3A64D7B66D2A449EDFBECB34FC78001036AF9AC9605D5F2BD2A74047F188C0B846E77834147041F8A57AE9B3367CE73F1D4470E39E4902B478F1EBD30EF0F0DDE74463399B618616A8CB47AE4C8915F8947FD38136EDF6BAFBDAA8CD2085B83BF0B0E0C9A3196EFEC1FBC2F9BB036DEB9399CB87DFCF8F11BE2BD6BC38D57C66B8FCEE7BCEF0D4F1EC59B64B485D9DD6B478D1AB57EC488115B63B45AC2AACA20DDE16F83B94133BCE5FCCEBD20F8EDE08E9CF39984F7967DF7DDB7960D5B3F64C8901BF3FE2F04C2B3275CD8F2E1E4E3B37BAB1362B76462DBF150DEEB2DB607FF16CC08F05E7743189A83709B191C1BFC56705BBC7CDBA041836AF1EEDA7EFBEDB72E1E7E59BC6F5A3E93049AE5C3960C3B6A31070497C68B1E49086C45D479BDB3F8E7E088A03B0921AC86C50053F20885D8670564C847833B828D7BEEB96747DFBE7D9F8FD77D39AF9706BCF1751B0C7550C0F55F8C2CA85A746F61814B82B6A0D1B0593C6441CE7DEFA2458B6E99356BD687BC0E68359B28942F08BE1A3C19A36ECBE3E6E04F03F2047DBC2E43865914AC09AA16DC5BB407F70747065D799630DA3B608C4DFBEFBF7F473260FBDCB9731F0DB95F11397268DE1F1D48143F13FC5EE0B83F0ABE101C12BCE659D2C1B9BFEC62425F0A5E09AA16DE5BDC15FC7CA0146A34CC634880A378CAF7824DE1CDF6BDF7DEFBC564E3FB1276BF9EF7E6048C6AAEEF093E189C1AF02C9EE91C3CACE51CC64826881B06065CFDA4E0EF830D41D5C27B83C783E5817334DA79EFF38C31C1CF06F8E9D690FABA18E9E518AB3D12E2BBE1D04FE67DE1B8DFABC06D63832109DFC9F9CE6979BE38A0D55A66301C353E44794EB2CA67F2DCEE7073E2D18E7D2D6895877D3F383960AC464388EE97CC6B0E80D4190D87DE197EDAFEAA7411D674DBC181DF30B0EAE0D818EA9A88E5E7264F9EFCCDBC7E67B057B0D361C9E22CFF814CE05B292BB6E4F9B5815D42C23C8D8E71529F5519A0277834382B2007AA266FE39CF3E3E1AAABA2E92E8CBEC371FB06C2EE1DC16783FF0810BAB03E2A107242CF776F4FC86E2098E3899BF2FAA6E0F0C0777A6D303FDC9175824768973C6E0CB83D63D92D1E8048CF096E0F76D6602F05AB82C141E7897BCD886765A10FAB33536FAE8D8EFAF3BCC748D303DEFEF6C01C6DEA4782C30221CB636FB48EA0FE9CF49DEFF2D05E0B57C698177C3E70400BF9D7E09860786027A46A13240A7F35581BEC4C48AE0BDE1F08972A63D14868E0B6F04E07D19942BD76C001073C9CB0FC9DBC6F138526C3A90F791B4F644C829727559D777D7071D06BFE42E8970448D701B9F42F073C694276F7CCE09A70C4AFE4B5C9CD0ECE0FEE0ED4799D27D40C2CE6C24016EB3C69C6B2F3CE7F66F0C500C76D4D387584D049833F0B6C300E6258C63B3DB82BDEA450AF3A27881A91A196EC2AB9FCC4F045E1B72CF897C0C17E1C7C2CE0AA16F2AE10FE3DD9CD6D992821795C80377CFE9B81764BE7093583E70329BE9167C9CA4294B798DFEF0636C7EF2CF8D9E0E702E1BA4F70418C64939BA1071181EFA606CD945B3B861D1D15FC71F074802815A44A052EAE1C21199CC06EF90C41DA1193C4116A34FAA7A721F942209C9CBFDEB3184A42199FCD393CF039C87427069F0E1845214E7AA0053C8A6BABCED3080F0767044D79579994787F2C600C3CA22947CCD1299707625C6ABE2F103613033CA658655406BD2CF8615035A946605CE46C53785119D2FED1F1E4CF87D0BF9CD4FFC984DD0979CF797DD77CE93E5EA55CBA3446FA76608E55E769043440E84E0ABAF52E13B463168AD09F0BFE26981F203F466028842F348581CF88D4F70E1932E4FA2CE8EA3C576648047F103C1374C517F57831A0E5F4A9EA3D4B32796764C2FD13274EDC9E2CB8256AFDD6F0A54D1492A881D1646E3C6B6EE658758EEEF060C0E838AF4BEF622C45328FC1016A2AA4CDA3BCFFA90079DB81AF07BC883A566FAD0DE1BF9C053D94E7C42A72A590198C97564DAC331C5BF8D7EFAC09331635FE47C1433A09394F473C07A9D3658C6B7EEAC087F37EB39B530586FE5CC001BACC8C26A5C541D42963641CBCC0CABC45F63111B1FDE14016947D7E3FD89E49BE9CC73B0385B68C846485CBF5814974B708DE4075CF89021F197940BE309A479EEDB8B2AFBAD41C25925F0C78D4D5AF1AAFA7A15705494B64A8831B0E7CE5C45704C4DEBB03EE2D344D12A7C82C4A1C3CC1AB4C96EB3A098E42D0B8CBEF7816FD85D7FE32B098AE0CB63DA1FCC4E2C58BAF3BE698632E1E3870E071F1D6C5096D7C8974F5AC683BC5B6E39A135D66AECFC55855C7EC0DD0CF2F05E45343EFB28BE21F4932065764408F0AD32702A1A9AA17664243DBC3091852E9E0B78C7B225EE9D7AF9F906640E1AC0127BC3B4FEE7FE0C2467869538CB575D5AA55CF1E71C411FF1452BF2CA1B72AC6106AE643A6F06A9D879BF33EC9B033A1D7192A15446FDDF589E627860F85909D2402BD1602C4DD5F070894F79930CFA3739CE0DB01D16AE719F15A8DC190B045E021824FD1AA1BDA5949FB0EBEB2A39B93E9D6C700CF0D1A34A863CA94291D0B162CA8CD9A35EBFE51A346D9B079F96C52C2F4B83C7E21206DEA8FD50A98CF2D81C8E832148DFA2CE0F98E7E7BC0488427D1481C2A2314BFB80691CB9692C10782C26F0F04C250D6A2BE3D9769CBC4F01C0EBC21700C9BF1F10015FC45704FD0316CD8B05A8AE75752DEAC5DB162C5A5C3870F5F13A36E88B1EA17D92A9837CD876F7BDC91F065242B04585AD3CC41903EAF513FAE08781BD72537EC380F924119990E5312F9AECF4C88ACC07F24806311933221A30A33DE485C129D0FC453B78C183162DBE1871FFEF4CC9933BBBA06D90A989B2486277B64ACAA213C11BC62553752652F6B7A2DDE69A6DB028A9AA17C766EC013194A75704D404C0A5DEA9F07E34DB021BCD771F1A772EBC1186C7BF8EBB53614C8AAA448B3177DBB1D16455DF338CF1DD4C1691F258FD281A758B4B4AF1C3111048AFFB453C80B5E5BB57BDE734CBFE795FAEEB8B1B782B327400FB26CB724DFD35116EA9156C34FB88D5730248EFB8D0037D931A1A76F455E942B2D7E2B4D335C09F592B64D56D83BA6DDFEAFA06A81AD84C810FE36BBCCB1E5A378038FE36D424BCAE7492641A7D1487889518A910626B46644369C3261C28415C9A29205631683F91E0F13D6DA293D2DD27B0234A1EA401BE6D052CFEA6AF02C5581C6A149FC7B401F117C6530ECFC943037CD9933E7A1534F3DF5E5830F3EF88EBC2678E9BEB2B38C6FA7B581BAD26BCED3F9B1842ECFF6DC63493412910CED352D29835F19E062EDAAD765F00627C35D4F0626F627812450F40B03F0BC2BA29DB6454BE98DD7DADADA3AE25DDFC9FB3CA9A46FA0F918B1F4CC2CDC4241BAE7119EFF2050A79230DA37C4348F94743C922AFF10E0256D65F52003690EB8AEA0ED2413361D8226571E0B848547AEE93938A0D7165EEAB97E59B045227C29D8A210FB5581C2B7B836AF42F4EA318BAF87F2481F9D81CAC0897A58F88FF155086A5837C3492AAE4E4B0404B244A3ABFAAEC06F56061A95AE0681A4E32206692381A8836D9C2CEF3C85022A07235868DFEC6ACAB2BDA4FB946B43942F8353A38D8DA29E98D43D76D2A44987845BA6C613E61D7AE8A14B52C7CD9E366DDA098B162D5A367AF4E85352AE9C1B01F99EC30E3BEC8A0848FD6F2EAE38D70F2B2D18E713A68C555537921744AAF397EFDB805302258E0D501168A958241ED4E392C188638BA6FD84B2B02F124675E238CECDF34B362F3C6B239DABE130994121D9D9E3C68D3B7BE1C28517CF9F3FFF63279F7CF2A7972F5F7EE5F1C71F7FC319679CF1C5A54B97DE74DE79E7AD39F1C4136F3EEBACB31E3CF3CC33EF3CE184131E58BD7AF5E32B57AEFCCEE9A79FFE6CDE5F1795FDD405175CD011E3BD3066CC18ED148BC733DCDC428A7B9B281D252CA4EB6230DFD56DC577A5A0B5809259F5D5849BDA542BB82CD4F72CB62CB8FCAE3C37CA6BA33C363D1C84A517A68458134F793C8BDD14436D5DBC78F1A618A623AA79CB51471DD5BE64C992DAF4E9D3DB5D8A1A3B766C476AB6DAF8F1E36BEE89720B64BCD16D3EB578532D045D73EB4F9D802C1D8BFA3E37A3F104E1F28F817E3A43E096D20662088301780B030969C672032F4F6D9A5B7676988CDDBD360BDB5187C5683B88D663DEAFD5ABE684E8FF7AEC211033C2E72D65879D9F8AC729BA132E5AA83B953FE442F1123CA25E5B13C8665A433CAFF4CC7BEC25BD1926A1ABD0D39E796F401FFD558050EB953B8321724216EF50F6458719BCDF674A29A29497EA9195ABC9F49C0DF01BCF5F13C3998449DA51245CB5C05602270933590B7731581916C88B8454E11AA384EA7B83920884ABD7DE67ACE9FDFAF5D3B05378D787794B8DE660B284D6892B39550B6C3912C21B13E257072E9D217906E9BC30AF79C9BEA100AA9F5E127EFA5E5AD5543E6F1A97CF2F193162C42B29B469AB72270E8395ECE71C85E07B3D4C48FAC41776BC73FA6E252C14313F8F1B23371E8B2CB93689E0EC248605E14A9BD616439A4FBFF0E4FEF9CEC23C7E229F3F95F7FDDEFC4811C211B1FBCDD9819B537CF6AD80A69235C986932276DD325E0C583CB61E3D1A2647A4B96253B5C8568014709141E741D6DB3060C080EDEE554866FD6132F0F53366CCB83459F8E265CB96BD3B19F7FD471E79E467F2F88D488FE7B59BF31B70E1C3657A5580F013CA94B8F7F1A10A81D8C46FE7C6C87727333F95E71A8B45B2E0352D21C6ACA781A6065797A215BB9D5BBEAD801D5776B8F9C20E139194B9DB03368F1C39B23677EEDC5A64C9D64894274E3BEDB42D279D74D286E8BC769284042999386010AD6CAA5C77C38D224A199B21D3BA0CC7E3F016C1BA319E251928658856465A166FBD3A1C479A4826D65F463D57568EA25F5C2969E59D7C60A2B8C69566994B152FA1282DDC7D47A4BA51D6DFEF6C890759DC367AADC89606108A0CE34A8F4D562E69DFB834A7D846F69280D09514B4847892CF6E4C29F6A3188CD14914BC86DFC81486F6BA21BFB124A2544688F9AAC9F506CA916F044A96F3C33F1746A41E1B63D85DE763381202C7682DEBBD236E7DF7EEEEC41176AE2631B8ACE8620843D9742D6AA2D6F97DCF31D5803227033FAFE39A47DEAED422C8F789F75E1C235E914794647E0D0DC615B524704A2B7A453A003850517B5A0C747BEACAC7A3F2BF9689F2605780ECA42C55F4953F1830511E2211541DB71E42EFD6C0552621691314EEBCB87C47C92401083705B3B2CAEF789D6E03EED35D5811233D168F6ECFA34E84EF37AC0A58910B5A483313ED0A26F395C08E338ABBEEBE9ECCF7CAE4C9935DA5D910E3B97223F54B2E8527085345B339347BC9DFA2117EB9EA24D49D5BF8FD67A0CBE1F602E4BE3A400B7EC7AB6832DE669394505B9371095E17615CCD424F95C364EDB4CBD6BDBDA91691D340881501DB1DF18F6C751D65C1676328846D9148D764CBA4CC414830AEE3549DA30A36176F3194E8201548096D61F77159386E322FDFD7B1D0D6B191BC1A0D389FF97F37E0893A150DC3D04072DC595F48B3AEF3A4BA0303286514C5C2C9657C0643AC5A28F8C2A530E4AC1B89CB844DBDDA665CA5977E55D5391A41D86BE0B922AE136B0E2E923AB73509BF92BC6461BD2EEF9319BCBC1CC37D1442B33E43560E96B4B376634DC0D2F5136A84A2AA5D2F240D9CECA3E3C68DBBEFC0030FBC266293B70A7186B39B48D9EE215DC6A91F4520BB3FB5EA5C8D60AE5AC22ECE968BBD380CE1F36A5AABCCD32695269F16B570750CD911C79A6BC310AC1F484D7A75C066BD8BF2C71BD2B8DF0A872FF5EDDBB77DF4E8D1EEA57A74F0E0C197E7B5DBAC2D4016DCD1500C8A57193C8BB17846B39CD519B2A8BB7A180307329ADB1F798DF5F022D952F8DB5877FDF81D9E15BE3653D231976E4709050B16D74ED2794205D2AF9D72031BEF61048B25FE3E185E7A205A66D3D0A1436BA3468D7A2C8FD7E57DA1C8A0F51D87FAE1F7AE6E9745F414BC47FF5D9F8BA18439FEE4F58CA6A5C350DA3F657DD62134095CDD8EA60C5506EFF2237DA77B83AA70F49E492151FC34259280FBD22778420632617FFFB7C54D1E8C16E36903118CD27595ABE30AE1E9628585773E6FB390D5702FF12BFC79B5BB14491446340749CC3ACA2DE5E5F25C8F87494BA9FECC43BCD71BCC73F70250DF0855B65911F57D59E4C165FDFBF7C7798CE1F72ECFBB9860E724006D20B720D5B751EA87C922E656D4A9E5CE45214F56F05AA1AFDC2A991185B88D8021D59ABD1A5CD1818523DD512E65896DBB4E3822524631990FC77BB64C9830E1C914AE37C49BB8B47A531894BFCC528BB95F5558F0C22A77B749C423F9B2339E053C872875E9CC1C79B204A663613DDAD21A88A483486A8AD41B0DE14887887337A2F10A1903F9EB43F90CF7E0AA8B82A742E4ED7AF031DABA3163C65C1503963F07E17D5AC7C4A330B0CB55C6E26DC8D945D9565412A084231550040F1392369B8CE1650CD950ADF7643888C5CA50EF0BA85ED24056D31F92714C82F7B82F4BF8BC948ADEBF287869ECD8B1DF4F36BC3CAF85ABEFF98D1D6C44A23C4BBD48AFD587FECE40867461D5A6338CCDC5A985C37ADCA6E96A389870E31D0ECEA36687A34E4979809F709349943B5DBE9A305C1FA9E07EAA8E84E5FA3E7DFAE839A9FC1DA72B65CC906EC1240E5D1AAB5A7C6F508A7A9B461AE027DECD197A94FD9A190E68D72D86579D1F23FC60D6AC594F27DC3E1BC350C2760C71BB094D07C1DFFF6D8E57E139FCC35BA8FA2A622FC3E4490B4DBC56795601A12B1CCDBFCB52A65583B1285F45E953C97C1DE1A98DF1B27B7413E2515235EF29AD174D3E5E85EB2E0D18A22B22652C6588A4D25D9BA659303ACF52608B0489ABE5DE54359C8468E53D9F8B711E4D886DD3ED0CB13F90B063103A89B6E16942D41F1720D2923DBB3296E30B69C692ADAA16DF133094CC7773A0E644252D21F366879349B516EFF6C57B136E1DD15835FF7B66E6CC99F74F9E3CF9236D6D6DA48385ABE0652169BB2B43193EC78B7A523B2B1D18CA6D990CAFDEE4B14D9732AD1A4EC660162F33CA905A242E436D4D586E4F167C21F85E0C46D42A2D88CD6678A2781621EBEEBB2A23740746269AB59A693B7C6A73BBE2CAD77C583C294078BA9587E0BC3B5EF67408FFE5810307BE923055D1E32DDAAA19F7672C5EE83A6157756923D066EE3BF5773EBA1B360AA1BFA186AA1F42879C2038751EB449BE1943B9654818F89B4459B0596339166FF5FB2A8374064FD2AB52DEC8B84A1C5540B9D4F5BA865D3383C10854042E231280FE6C85B7292F7856B3BBCB5B75315C74286DE07A94ECC68B903785AEA0D7C676830B4F4711AF2B91F766301A1275A598B81406EA40866C7687E93964EC6E1A77F6310CA3B952C383D47B5A38FAE61201F27665C77908CDEE92C89B6E308C45831DEE4928E0428B2EC9C3DD820A5EDD01D9577B45B8F3226D15DE4C91F7F43CFF6F86450B253242582BC8190FF7096949E02D6DA0CE83118414D921B49135788FF7FDD4486FBEF1B6B7FD37AA8C6663532E44BA0000000049454E44AE426082', 'Fan ON 2', 'png', 75, 75, 96),
(114, x'89504E470D0A1A0A0000000D494844520000004B0000004B0806000000384E7AEA0000000467414D410000B18F0BFC6105000000097048597300000EC200000EC20115284A800000001874455874536F667477617265007061696E742E6E657420342E302E338CE697500000185249444154785EE5DB09B455E5790660471450F482082833885E4441669141D0A08023B415EBD0AA688C538C3169D324C69AA48D464D35B1318DB6718A63256A3489C668B10EA94671428DC518151CA2CC828C37EF73169B75C573E1DCCBA1A2FDD77AD7B9E79C7DF6FEF7FB7FDFFB0DFFBE9B7DCC63F3608B60CBD5D83AD836D8CAEB165B6CB1CDEABF9B6DBBEDB68E733CFCBF19053988D82E68DFAC59B31E9B6FBE7997BC0EDC61871D46D4D4D48CEED4A9D3D8BC0EDA7EFBEDF7EDD9B3E7D0FCDDB94D9B36DDBA76EDDA7EABADB66A91E3FDDEB93E75833520A845D02116B3F7D65B6F3D26AF47C662CE0B1197B46DDBF69A76EDDADDD6BD7BF7697BECB1C743BD7AF5BAAF7FFFFE8F04BF1C3060C0EDC3860DBB65D4A85117E7EFB36B6B6B8F1A3162C4E8FCB66DCEC7F23E55D6C6BDBA0663822F6FB3CD36D7346FDEFC17AD5AB57A2996F4C64E3BED34AF75EBD60B6239CB42D8CABC5FDABE7DFB551D3A7458D1B973E7A5FDFAF55BB9D75E7B2DEADBB7EF3B13264C983B6EDCB8A70E3AE8A09BF2D90939DF1E01D7FDC4125658123741D4AEC1B1C17F060B63550BE372CB8255415D2CAC6ECB2DB72C21EE557A9FE34AAF71B9D26721B784962D5BD6C515970D1F3E7C45F05C2CF2F47CBF4B8E77BD4FD4400E2D699B1BEF9C5724D1A5DAE0C2E0F560959BCF6B93814088552EC8EB0DF96CF7A059F089182C8905B50EF6CB0D9C1A2BF8468B162D3E97F7BBADC65782D94159029A8290BE2CF865FE3E38A0879BFC4054F3A05B4093A686ACA7A347EF469BACFAE8D5DF1D15FC2A581C94BDF9266079C0B547042D834D56B74C4C24DA21D837F887E0BE6061B0287837B834E81BD4048EF961F04EB02A2877F38DC55BC10F821EC126AB59858073BBF1C18F8219C1FBC19BC16F83CB827141C7808B740F4E0D5E0E5604E56EBE3158193C1D7075D7A834E732F7FF330B7421224EBC8F0E6E0F5E0D96067383EB833383E141F7443484EE1C107CE4DD16B0BE72043406AEF7403034A8D4058BB9F306AF7476A30E16D521F8DBE037811B47D263C1C5C1E181DC67B710353A19F849DB6DB7DD9911E241F9AC7FF0CFC11F0396518E844AF146C0CDF70C2AB969442175EF68E9A4A41C93BA74E9D22BEF375A52EBC4ACE4C8E0C1005173824783B3836181C887945343D68DC9B25F4854BC3FA90477E9179C143C176C882B22FAF94052CAC22BD12BC7B40F4E4A9EF7688F1E3D1E48926BCE723401AAEA84C9940704FF1158D9D7829F0567057B079D0216745140B716C7A256243A3E117C31EF1DB37F40E8DF0E9A2AF4F3823B0241833BAD6F20A2A822CEC8C2FD3ED5C182C18307CF1A3870E0B76261BDB3A855CDD35C905023EBFBC17F073706748B2B30E94303624FE40BAB43DA7703DAD22660797F17BC1834D5BAFE1470E7BD82F5DD6491283BAE5D40266E8A2BCE4A59B53845FAF490F58510B8533EAFAA86B9A830ED8227071302399449B018398F1B914B49406F0988BD5C8BC07361AB3B29B837680A5972AB1782BF0ADC60432E6871C9464DACBB36B098E6D03B383AEFA7C69AE6A4065DD4B163C787520D9C1CEBE7D255EB64980091E4E75C8EA520EAC0E0EA8025491FA4113F09C6064A1D133D2CDA756C5650B60DDF0B10DA58A15F10DC1A58807565EDE6BA633022DA797908B92B1A7572028EF9204C1A332D95C68AB8E4BB29E46F89961D90CFAA9ADC162B56005934EBA980557955071E12107429C42999F0C3C9EA1F0B615764554D8AC539560A508E9486C0BDBF16B8E975E9158B63792CF0C59035BF5FBF7EF7E7552541360607F4EBE11D77DC7149087B33AF97649EEA4CF755F5E1A44C974BDE15107B37222AF60C8E082E0F9ECB24164527DECCEADD9CF7F22DAECC4D1B53FE20F67F82630256D39005200A91C892345F170B9ADDAD5BB70F060C18F052A74E9D4E69D6AC1952CC53309A99EF9764311F6FD9B2E5B1594CBFAB9A3B16C3094D9AD0FE457058206DB072C8E08A4579A33491979D18700356775E20A3AF342A2E0924BDF4B19CBB784FA4350577CFE258481158DA726F34E9BDB8DBC2E8D4ED21E633F94CB06169776631E784405D8CA9218B376C9474C2E4B461E898495A1505F555C11F023728C510394F091049E8DDC8F1C17F051F04E5C8A90FDA26DDF86AD025583B0ABA3137D827377C62DCFDD25D76D9E5B46894286C01CF0D1E8BA82F4C147C2B827E558E334FB52BCF981692E8A1E0614119C1461926CA251187AC8981D400094F04EA44111051FD63F2F4CCFBCF07370595B8E2B240BA8260857939ABB26807C5A26EDB73CF3DE7F7EFDF7F5A34EA9C908210D6F24F72AC58D5B29A9A9A67F339DD2417437C17A8359550925D0B5F7557AC3F9CDC45B820CB9A1A7C3910B998BC499D97C9FE2625D043317D81004C1219E5482A80F81F070DD582AEBD7DC045AF0F612B77DB6DB7B9D1A95FC59A68DC3EC141C18F43D2EC7CBF2CD7A7B18E97CE98E31702049AA7FA71A39265B811ABC5C409393D938B11DA2B826732C99581160E71E59A1A78EBB22EF9D82BC13703EE5E3FB7421A97745DA08767E4FC0FC67AE6A5ACF92091EE67D123D7370F35ED2F82F9C1EF56BF57EB8AEAE6EDF7AEE15CCEBBF6A234382A3EB0DE2822916E0397E4FB0A6B16F4BF81D04FCB44C2290142E55CC82B4714204B2D3A3990DB152BEEB54D8819124B191B0C8ED5B012518E5B3D9FF7CBA35F334316AD438612E9EF03DD0F65DBA88045D23B7356FAF48BF50F8D5CF0867512E60B9370C34ED22CC2D72C211F098D21CFF14047F60BB890A4F5F701720A0D43A48256712D3B2F47D6D2DCB42EEBB0CCA575607EE6A2561D92B97D3351EE8E76EDDA5D1E4197F0AA32E8E2B5C17B2153335263D2C2B02E824F2A1C43CF7CC6A2C6E7DC17C6226FB425178B141424DF65CBA0620204B44FC4706272928911CAFDE3FF9D33E1EDB24ACD7371C755429C6364D9D284CF06ACEB4B81D53509E6EE6F751ECB5A3B9B2FA514B18AA511EAE7870F1F7E7EE6821C5620A070958199E735897CEFF5E9D3E7F5BC5E96F72CCB42FC7540372D90E04097583A3D428E3A17B974F5DF8247F2DBD9D1D3C589980B43FEAFF3199DC3C947069393194F09C35787E1E7870E1DFAE4D8B163A766A2DF3DFCF0C3CF1C366CD8A89CB026265A13E68BFDBA7511C7BAB8A3C9CB9A2583F481BB58696DE1670302CEB2588192497E06F343CEB25D77DD7571E63063D4A85197C47AC6E486BA641EC54D7F35F37DB14D9B36CB93853F9345955729B12CD2DF045C92F50A0E5C6B60205DF84E20FA695EBAA6EB971628E75E11A3D006729CA0F5A17B64DA2C4A92767F30C71E5D12B86503070E5C367EFCF8C5071F7CF0D3799D7AC41147FCF0B0C30E3B31D9F0C0AC7AFBAE5DBB16196F43C4153AC6AD5983BE9255977B4956E5621A884F06C45E24FD97E05F0349E823216056AC6BC101071C303FA44DCDDFC7455B0AEB94607E2F37372337E93CAA09164BB82D92EF051CF913CBD6DE516AB9F6FA028BB487D67DA80462DAC2EB2501112EFD202B569795AD8BFBAD4A64593964C890E5A3478F7E27C43D3969D2A45F4F9E3CF9E2030F3CF0981056DBB973E7AEBD7AF56A95DF210ED61E2C57BFBC684717AD1C853752CE085401DCC8EAD316372A105C1A221EB07B1D779B5B5B5B7B57E6E43CF409691A9257063AB6EE414A400FB999DAD502887E824B254930B036F364916B748B35D0162716561D54EEC725E222A6AB7AF7EE5D97C274C59831635E8B7BBE74FCF1C7DF73C821879C3F68D0A0F17199BDF27D91656F594F905D83065C177031B995568D3D451D0A2BC8BA0B0B2CA22A97652992D8BB13A9E667E1E664616E8D5470656ECDED040DC4C89724AF34D2FD081E2C8EA534A6E1C82D79996BACE970B819AEC405ED1A57D4678A0BD68598BAB8C4AA58DCC2B8C71B53A64C79356E7A43DCF54BBBEFBEFBB87DF6D967508243FB58859BA62117043482EB298A65CB7D02D7470E520B720D7F7301C2CCD2E465F746B7E6E7DA0B62D157E6551B0699E62F17BB3B783C103458516308AA0FBF53BBAA7BE96369D01496C09F458E723F6C105C35825BD7B66DDBBAB8E2CA68DC9C430F3D74EE71C71DF77848FBC9BEFBEE7B7AA2D894DCA08E849BA013CA205148A12BBD2808F22A705849A65F68454198504FD0A7475397F4ECD9F39558F715B1B06BB320F48EC5AAF1586D5349AA0F0BABB34BFF4A0359BA88DF0E585693775E32E11271894A757193A5899E8B4E38E184E5B1BCE9219336B12837A359489BB89DEB23896E76C83946E6E6C727D7199D54C07E23D27C0F9252F9D11D9184F9F9FEFD041A617E5EACCBB9AB41507DE8D1B156B95869307F2158887523E57ED46870D364D5885B959B5911CBF2391D902AD015AB8500A3485B4E4BE4BB239A343D91EFBEB8F1B9B122118D86B13C8412EEB3F2F9CC1CBB32283D2892CFAA0DC4173BEBA26E6998A84868D70593E57ED86464C54BAEBAFA3DF720BA849E5B218B8B8992A2DE43B1AC15B198BA68DDF244BD171250CECA67457F9C151274FAA4BDB3BE027C43A1EA903E0820A5812C05AFF2405258EE47D502579119B39642D06914F1D745B5CD5522974BC73A17856CF918AB2BF48B95E9FBAF33725709F4D58E15CF2B0D622ABBFEF7A0B13DF1C6025977069E7C292A00D717CDE43426575F77C8825443DBA4385E40D09FB271D1503D592D4839E8ABCAA3344C425896B831BB723FAA16040FB90B3774D3850E9101E587A4B1283BBC4A5C09AC32A5B02C79989C50265E519AB3019034DB4790DEAC71036EA1C4A89AC037000448128F0BE42EAE4F8768929D18CF74216C66F04C20B3D78B2A4A2A4096DC4746CE1310DBE408BE0E382F59B049AC4A280D9AC5277D6825CBFD7043E046DC5409D1A3A5D1A11F4593AC166B29AC8BF5D8D5D66FFAC7C096BFCC9EF8D7D7377395DC9A2BB7D6E357E7D1AF6ACB883CCB5C2C6669585979840F6705E57ED4582048A47203BA9CB2F569C1CC90B52245F023C1F1214CDEE4FA05618A6CA40938722CDF5B4C8355A90494329A8182910C9BF571959F06AE63F396F66D48F60E7E2B3B504259B0D22842B7D6866E66B91F560A1A822813D645B0A52FD955D6B0943B43D0C2E45E6FA628BE3679D8C14962EDF620AB7049E4B020F3F21E4912532983EE0152E43F24C3F915D17A531E27B0D34CE3883F37F5000B916E8AAB5A6C89B4825D83A0344C8626689B58FD4AABF20205414A0D27571CEB002813CE4EA64D7314C9D203BBD3F725775A9012E8BD249D5725011DBF4346BEDB3A56672E06E20CEF11A7A724B712842CA81EFA43815A51A926A23A4625A224D2BDD092514C23D4A688E7C258A3B95662718ED1A1B5594C27D70C17D30EF1251D28F7E3FA7022E066FC5AAD27C4AAF7ECA64CCE8D5F140E6ED879E79DBF12EBF128120B127575199E4AF9B33264FD21E5CA4FF7DF7FFFCF8D1E3D7A508AE2D6C9C6B78F8B4A58454B1D8791018B414ED18372F32C95EE39AEB0409087218EB7488994479A783271BDAEE981B28EBE35D489F0191EA453223589583390A56FA3BDDA50442C4E8A20E22AAA317765922D2493A2332287D4E0AE10352BC5EE13B1A42FC6A224965CC926C17921E5D18E1D3BCE1B3A74E8E29123473E3E71E2C42BFBF6ED7BCEA041834EE9D2A5CBD149484F0969178774F9976BB10A56AF5CA2AF88A85F8417A370696EECFBA2CDE3FEB45B744D65E53C80ABF2080B50A42CEE115148E51D6BEF28957482AB88446B3F3BE504FC570E46F054F7BA98840F4112CA63633DDC58F286ACC1B9C90BF3D91F350DDBB76FFF68B4CAF756DCC559CBB9F9ECDEE6CD9BCF8B452D1A3162C49271E3C6BD1BF77C3EA5CE8C943C33534F2E08C9E6E246E8A03630CB744DC1C02345F484357DE886568F8238D114710A77014492CBBDCE0948068BB3088CC075EC4C7BEC5313D16F0A79280D177223B69B1E0E0A139594B1348F428A3AE707DCCC645D945E38E19521E4E9DCE4F7E35EFBE5067C276AFD3C7FBF1F0B793BAF1E822B8A621AC9BCE9CAF521E4A51CB32C6EBBAA4D9B36758AEEBC2F15C9F9BE208A157BCCB2E87F09E72313248E0EE9CA1F6E5E8E30A3B03C378D389EC4CA15E5BAB27F19204E30B29FE96FBB52382992E135C3C9AC903244414D1BE8C2CF0361D9243D2A4473066772FEEDCD89ACEEA1B9D9DB5BB66CB9386E3533649D97F7DCD1EAB1544143A2A9C6E382266AD2C5860352D5790DE548168D4B683B171B1EFA601E40BB2EE4DAEE52128988824141CCFA061210E7373A203C4BCA2210F95BDA52761BCCF0054B11824D8C9B09CBB4411E66E57A676253E25EDF8E154C4EA47352A45C9689BF9E946071C8FA6D2CE3D4BC7761BF752EBBD1CEE73C45DEC40A4C4894535C371485C9801480D6B05896F5AD80E0BF9DF9B05A4FEC98072176DE4A09331C8B38F07BE439C77ACF63D5698A9604F89B4016AE3321667F670899DDA143871B421AC16445ACE3FE88F2D288FA9FB2DA37E56FDFD12FAE475C5905EB2D5CC5AB73FA8F31E50D772B4716481574252C242B67F5441958BFCFCCD75C59BB506F511A439AD1A8E3B94711457AC43A46B468D16244564E18267408783056F57E34EAE556AD5A7D23EF598F55D5779A16AB5A140B7B2B7FDB65110111E47C56ACBE507201AE23A44B1CCB9154808BDA281575B5BF59A2DA9130DB1BA47F16ED33B1F6CFC6DA1C23DF2ACAA48D36DC90153A2464DCDCA953A79B63490A5F2E400CBF9E09CDA8A9A959DABD7BF7D763619F8FB5B11E6D6201E0A94C767E6037B87E77A1FEA4FDCD8AD57E72BB35DB6FEB00426DFDCB996CAD0BEB1E6FA2315CFE64D7CC62BD98792B8764DEC5B537DAE01E04EFC458D4ABB1A00521ECEE58CB84BC67292678732CE76D1696107F4FC85400D32F896DF1E085C822A2949B30EB65AD924D99753972D6863C8BCB09F90832176E2EE1D5A69618CFC91C17C723EEC9DFBA1834D1B5D6AB3F4D1D2C8BDF1F950BDF1BAB599095B2437D55564ECA20829D1E3C9CF0BE301A352784DD11328FCCF774839529713C5BF091846EF5A0275C90F04B0ACB91B336689A2452404012B2A50CBA2592560142C22C18D863646D34B15BE655C84853746C9DC3C958838B5D100B7A2116B422A23D3D826E05E52634EA824C6246CC7E59BE9B95E4F2B2BCA7132665924A96729AE13D0146A62E41A5B5A8A828E7F3448EB9B1286EA9DCF2DD7B01F7F3ACAA452D3D7E9405FF5A82CD0FB2B0C7E46F9656750B63BA744B417A6B48589E92656EF4E9B69042E4599764F422648A8299C823792FA9F53BD66952E526E6DC6E446DF752509418954025216FA34722A366A1AC5B1BC8937C9ED61170A417A4E19AB8E4CB21EAE5E82C592015AE5FD5E126452FBA705A8878206E38AF5BB76E8B6B6B6B6F0C6984B9A801453D9BA7DABCD200C96AFDA8B7F690CFB9191D844A5DB00062A5191E4E330785BB245A54A6519255E746A6F908344B327F7D34791A99705F086B68319B349C48962D47B22ACF66853C2BF54AD7AE5DBF132D736111529DA76CB195C5454DA6A14914919655F8BF9EFA3568A510393D39A8DAB071218820481E67F1FC57AD3E3FF71614E8995A9674B81FD7E7C68298FC715D0BDBA8610564EE5CEFEAACD2ECE8D7A208FA1B71CB8B62E2449A25E92371AD622FB0A1E13BC7B208CF3F35C6050BE84721C0E238171D2AE5588128ECBCBA06F44D03D24608122D2C92A664DE5766E1D57F16BC6A6EC942B88D49297DAECF855E8B6EBD1FE214DC1E3724D684DC4591D19055F99CD5B1025A6333A02964F98DF6B79C0E016A451B18327C1D5459BDEE88168C568E055574CB1175517F17A216C433A42155FF7F1D43CD64F5E882EE83DEB78C9AA09A6C25A6EC1856A0C2E7268D6DF3D68734C27F6C0832EACDD302C53692746BAF09B4B24545D664EB8C9408068211B269262225C75525CB8D3A694198869F6296E97FA4DFD3C030213AF1F5804837C5AAEA43CF494445060DA44B0A6A4FEBB018FA09F4545742C4E49E520D7D7CF99A7CB25C1E5895C164B9A4A8A35DE3E61B6C63D41B88F25B22EB86086F39021A032EA747269B27EECECDD2D489DCD3DFCA22DD8962D342AAE231055580FB585730DAE0C18290C32D595A1182D7371CC305A51626BF212E584071ADC471E38290C5F0CA9AE82B6BE292841E0405DD4FB9230DAB9AB0AF6FB8F9C6AC8889297DE4471A8BD5200B681011A75B828C575192784B4B4066EF69409D08690ED7DBE8DD880D19AC50BEA654A9860B16605DFE594AA4A39DAEC1CD906851D492845C65A1EA28529B4D9628838BD820D50DA864ABAD52B01C8F69CBD8592EBD62692C09490A6A62AF90B6601B4DCCAB35ACA25C4CFF5D8FABA15E7B53A1C523321375A4107AE989EC9E5B6A3E56AAAD1FFB304951474EA39DA28552E9AE704360518A6AD0B9604185F57035B95F435D8F4D7ED00937234A297C2590C2B9FE938C1B790590519F4CAFDE3B16FC4E8EE51F0464ED9A8DBAB42CC8750A7CE2482A86895B75BB471A83A2224DF11C828D531B103639B994478824AD5E6DF802EBB1B9EBA9410D431BADD210359F66A3E2B8927CEF133310A6532974170D443D2799B4DD19D9B88E810C5FCB458F1D6CA1D99450FF116B254CB17FC8D5B8384BFA540E628B34A2AF632191944D6BCC29A710890CEF8BFFCD11E9745F4555A0451BADAFBE290E375AE80B174222200299E0BDEF8BD78F899CCD36FB339140742605ABD3760000000049454E44AE426082', 'Fan ON 3', 'png', 75, 75, 96),
(115, x'89504E470D0A1A0A0000000D494844520000004B0000004B0806000000384E7AEA0000000467414D410000B18F0BFC6105000000097048597300000EC200000EC20115284A800000001874455874536F667477617265007061696E742E6E657420342E302E338CE697500000199A49444154785EDDDC09B8DED39D0770841011B20921B223B16492882C83C8469041ECD322DA31AAB56F558696A6AD99B4D48C8EA1622B8D6DAC2DB5A6D60962EC5B6A27D148842C2D822CF3FDBC4F4E9E2BEEBDB9F7C69DE7EA799EEFF37FDFFFFB5FCEF99EDF7ECEBDAB3581B67A3528CDE7358266CB8E507EAF7ADDDF642B64C09A41F3A065D02ED860F5D5576F1DAC93CFC8F1DBFA41A7A04BB0717EEBD4AC5933D7B6085AAEB1C61A6BE55895C0BF8956A40201AD828D822D82DEC1DF0707878443D75E7BEDBDD65C73CD2DF3DD351B072382EF06A7E7F7339B376F7E6270503E8FCE7144B07908DC30F7B4C9356B0748FE5A136700EB061B06DB0423832383B3828B831B33E04743D4A32D5BB6FCED3AEBACF3AD9CEB1C6C1DFC247838981D82A6879C6772CDD3AD5AB5BA77FDF5D79FD4B66DDBF3DBB56B777CEBD6ADF7CEF70139765D6FBDF55AE57924EE6B451A92D60B0C7C870041BF0CEE081E0BDE08DE0F660773A3527342C63D919263F37DDB60547045E09A25216051085BB4D65A6B2D69D1A2C547216741C89AD9BE7DFB673A75EAF4508F1E3DAEEAD6ADDB491D3B761C96DF36DF60830DDAE6BEB532094D9E341D647BA8D93F0593822782E9C187C18C605680B00783FB82EB82D38341C166C1E8C07D1F044BAB22C42E0D794B43DCE248D2A20D37DC7071487AAF6BD7AECFF4E9D3E79EADB7DE7A7CCF9E3DF7DE74D34DBBB769D366FD1067E29A6C63BCD9A57F08AE0990F45EF076F06C707DF0ABE0CCE07BC101C11E41FFA06340657709AE0D16044B822F1056803488B42D8EA42D8994FD75CB2DB77CB97FFFFE770E1A34E89C7EFDFAED9E739B84B4162199DD6C720D599B0463838B827B0292430D8F0FF60A48D07601DBD42B03EE1F49E996CFED03F7922CA4CE0D6A246B45C4E62DD968A38D166FBBEDB69F85A897860C1972758E476CB1C516FD4362EB5CD3E408D3211EAD6F406A0E0BF60C062E3BC7D05351D2F38FC131B155A745A58E5D77DD75F7C9779E72D7E096605E5067B28AA485B4A5216DE176DB6D377FF4E8D153870D1BF6E38103070E890D63479B9C1D235D1B04D4519C049B06248A1DFB694072EE0F9E8BF19E96014E0DCECE6077CC39E4DE10FC25A89698952113403D9746AAE6EFBCF3CE4F86AC533A74E8609284194DAA993D1286347110A9629F840B8F07EC17E33D3F203D7342D8B31920B276CEF70383DF05F552C31541CAE21D976CB5D5569F0C1E3CF8C184176372BE494A9726E661B4FF39B82B783340CE9F835782A9C1ADC195C159B1C1244AE8C039B0750B836A89A82B1016F5FB34A44D89A41D9C73B2802667BBCC9ED444547E4260F04F068F065707E7068704FB06BB050302EA2AC5E11CFE3B406CB524D4139F85B4FB0293C03C343A59065F5FF195E2902CA9CB31C169019BA5D3C382ED97E1EF82E1094CFBE488B0FD03016CBD0C7C0DF82C20D1FF1948AF4C60A3B5627F0CBC8091AC0B71EED339E100E3CA13226670C07E5472BFCCF80521EAB278C313A28ABC247521898B82EA08A82B104DE56F0CD84101AFFE374A4348910EB6A44F06D6375E6BAB44C62DF3393679CD95918630B68BF88BA5D8A57F09049DD4F2B53CE7FDD8955743D60D89B7841A470424ABC1DE30581CC81890EE7924B6D1F2460F2D526180E70413130F4D4A1E767682BCD1492DBA249EA98B94214C50284F14BD2349CA43CD0CE89D78C3C743FCC47CDE3D4098B0A2D608BE16B887B7FD637062D02B10FB351A5124AA7BF0CDE0AA80042C8857999708F99551A3465D9B08F9D0047E5D23695A6D46D36FA273D1BBF84A5EF8722037BC29F88FE0A480A117668C0BE48C0DF186248A444AADD8486914A26AEBDF2A350F161FB11FBF090CEEF3801B5E9224F5F31D77DC71CE2EBBEC72C79831638EDB7EFBEDB78964D42661925992D52F382AE009A9A26A04A9DD29302881AB093A34B83BF86B501D21B5E1A340E22EF01D1208151A2D99366033C195FF2860207994E51D0A314B65FB03060C787FB7DD76BB7FF7DD773F2DE4F5554F8A5A56D731CF642F48977C1031E01D3CE0C83C739F4C04C91A1A88CB1E0A3E09AA12511BA89E097D27382F503B6BF4984AD4AD52C96EDC1ED0FD2FD90D848598254923164625A746C2C6EFB4D34E8337DB6CB33621ADA699A4DA226852B679C0439D1D4C0C5177E4BE9B63B77E9CEFECA378ECD3E00BEFAD05AE15E85E1EA88749B54C505D9B0945ACF1D7896017A97D9BF19F07CA2B5F90AAAA880DABA41331F4F343D87363C78EBD28AA39AA4B972E1BB66DDBB626094318172EF014B94F0B66E4599F86A8E9091DC443130269515D254B88C15930192680E7AB6B78A31977C5994508BAA52F340B69B5DEEF02031138DE197C1C54D7B9E5C8E0968698A5C9BF160F1D3A74DA9E7BEE7971246C74E7CE9D3B6CB2C9268859F185CE89EAB9F3C98162E00BC1944038F183E0FB415D250B514CC56D01678428C5C7BA10E51A442156E6B06F429793339EB11B6FBC7197366DDAE0A3DAE646EC0A1CFF3D60D46B94AA1591A05286FF5132FCE9071D74D04DB163DFDC669B6D3A77EDDA7545C2A806432E2865132F0814020F0FA43E42879383BA90C5F349C6C5641C077B680CB511E537528F20D7EA1F67362C12755ECB962DFFB7478F1E57F7EEDD9B2AD7289D58147C1263B6AA3EC6B5A292B1394B234D8B235933468E1C79438CFEFE21AB7B60C9ABBCD4D12246D74034CF639920016B8FC080E592CF05B54D163B4AF548A4EBD940EA539BE7F3EEE268141DBD9F44B1D1BBE4FBD509B6DF0D517FEADBB7EFF7431CBBF725E9F2102C7A2197FE4C500915EA0B1296D0E2B3C18307CF19376EDCD4D1A3471F93CFA2FFAAD173B11106A74348521854F4935C5F16B09735A53B244A88F03F01A944784D44792778A781BB4E95F69456AD5A5D9ABE1ED3AE5D3BFD93824D48FFDFCAB9F9B1C3D727F0DE215EBF6554B3F4BBD28A61579CBB3058A53A12C2DAB76FBF24B3F3C1AEBBEE3A79C488112764B6FAC7AEB5CD6FDE6550BC22FB22AAA7828C3A9BF57020F125D935F5817ABE1488D7F45988509D7DF11E522DD5720D8902AA7E47887837845C17B5DB23CE85947D2FC729B1551FC6494D8967B7A6F9A5FA9701740894797F1FAC4A4E5681D02233B628847D7CE081073E171B76CEA04183768898EB3469526D9048FF3A7824783598139018690E208C2A56258DB421F3BF0283B6ECA5FF2B360324BDBDD297BD3349E3A256C3F359D0BB571CD3A4F46556AF5EBD5E8899383CE749170F7D4BAEFB3006FEB58446E393AF5ACCFDC24450115EB0D88A552EBA01C2D221A1C527B161CF4625CFEBD3A7CFC12D5AB4601745D8024F4124825E0FA83FD5E28925C0D221698BF545E47D9E99FF2CEA63C195C766E78A412FAA0606674C88DC35445D1342A604132235233278C6FBFCC476AF74EAD4694ED2B75B33B14C8092F6F89C7F3B447D18AF7845D2B8411907EF5A695EC2E01243ABC05C79B5836F08101672947617451D6745ECEFCF7755D33F05484016234D0D4D9655E98382FD0284C8EF48DF0319E4CCCCF42719F4CB3BECB0C3999984DE1958B18515C39D6B7A8650B112B342000EC9F707E378E68794C91D3B76FC767E933D1C97E313714AF33A74E83039641D90BE56CEC7463D16A2E6454DEF4A5F2DD39990E5CD17F9D92501B21A6CAFAA433A55593C4887166766FF92CE537390C3A942208881A52206C83B910A368D775428144EFC2EE4BCD3BD7BF799F1B8B7F6EBD76F6CC8731DF7DF2EEF199E819E18C93B36D23428AAD633E7F7CBF196BC7B56B76EDD5E8C7AFD30A490A00372FD6DE9D707F9EDE504D7C7E6FB56392F89FF63EE9F97738F86CC43739EE9A834622BCE9097495E0DE22B25AB2AD251CF16EC3E1F90260B14C20846987400C30C3C3415D05959C51119F88D99F17742D6AC24F0BF88546C9F8121AC5B7E3B2A03BC3D84DC1D07736A54C83D88BE28A4CE88447E92DFAE0CE1546E5448F875AE7937A6E2AD107C6EEE273095CA48FA393744BE161C9BC965632B4DA7C4575CB6B248BDE2AB06404822873B3F50D5305004157B434A0AAAAA98EB24DD2764B0F7C75B7D10C97A201E6B5CC8D27F527472487926126439FF3721C0622D891D1F625ECA75F39119E953E910DB9D1D52103237BF5D1CB25C2B91FF65AE7F3DDFDFC8EFBFC8772155A5218BD8B30FBCD24A539C5504E7F170B077A0B0487A0A492448B0D8379DDF2E33DE3D0373BE481AC2107C617E7B332EFFD5903621E4F1AC1667C76580F7B56EDD7A41D4FDFE4804EFC6CB59867B2CCF9D9FC1DF1B58FC552E3A2E7834F7BC97DF6C3510C620919D7C3AE0ECC6079E5D693A430DB87117342818AD23A8E0BB81657D41684927F441246D21E3CC74FEF2A8DAA57106C7858C7E5135C61AA11C915AFE7743E2D418ED0F7BF6EC795B6CCE1EB9C718989289F9FC6688428E528FEBA9D66F031EF781C08208527865BB75EE0D140EA82109D50F12253CF94660922A8D8833AC4707B6034921047DD49114F84CDA44CDBE976011A9BE3B2F16F2DD677150559473EE7154219528136DEFE6E6D98432A06999F9CF1345CF193870E09303060C38356EBCEAB5B4609F107273546A4E5CFF8B21ED884822C7405A6420420EFB2B24D6EE15B81A9F3C54494804EF9DA4CE35FA239CA0CE8841BC1529353149BFC0B4D2CC981BE9B7049A5BF732F18E32099541E28BCBCE5918259E3C99A3120B894482A85A4820C0745E322E80244D857CBFF33822E962C4D5C80D42F5616124A252FA8954D915739B6032E745E2FA4ACAA8E2B551BFF7120ABC99C0F2E7912E861C3CFB94E0D40001C852AAF65938C25EF1B008F11B27C0C9208E74DBC5035B654278638E87E4579A0E60CECD98C4B4177E275000948A98156AEA68C39935403573BAFDC36547B3A370479CFF351012FCDBB2CFD218DE6F41887821383A1DD121EF16B688F15C2BE6AA980184459516C6604F8E01DE37D72B182257E7B9FE8951C5B991C0B9C90CEE8D97BB2CD7531D2990108814E99B5D86A44D1F4D885CD238D82B476351B1305E6A4B350F89A41E1607312CA143EF7CF6CE4A633374A210462545C625E631EBC476700680FD5219D0E1FDD3E16F64200C2C71357B76C59C1B977C467EF39DD1E43C6ECFFDF3230D2F04C7E737B358C82A46D5AA4F51732AABFC6283080FA57F45B248D02579EFEC48E067F17C1F27889C9DEF6C52D930E7F3CCE0B580343B9276475A02B4C13B6988CFB4666A9EF344A4F9A1A8F8BD998C13F30E63AEB4E5D16FB07D06312283E99B1B7CAFB8EB7C3E30E78ECC2C8FCC677A8D9803635B26C6105F12CFC330DA1269C5F7A23CFC91D899AB32807D431CB22BEE3833F446AE7D2DF859DEA30326891D52DF2A95539D2661368F48874830234D15F41569654BD2DC4CC092F443FCC6364221BAAA9D2CC7F23BE9750436D9F7824579E6C24CF6A789D5E6661C57A6DF24BFD274C06C0DCCC0CEC800AF8B489F14620C52723D2603BB29E71F8D5D382B2A515968C8037F94073E154FF56C66F6CC882CE3BA733A7D61C8981EB73E2D2F3B292FF21C91F1C9B95E616D6E6C8C77EC92D9532E212DBC9C09E079A8CAC581F2B2D51F36A56C4443183BC23408731A2D263409E16376C6AB54AD0454693AC17E887BEECC4067C6BB5C97A39C887A7C27643D15B26686E56B3240768D3AFE34E75F9746E4FC25218BBA3194AA8DAFE719EF872C811E15A5CEF2BD3B23899F26997E3C79DDD111F14D4320C9D207C4914EB348121965E6A0840D269591D771AB37D48CA4543BD8AF08264398C30C559ACE92A0B161F30F61727E06F96CC83A3452A2E38785E17B9533249691388316E9FE20BFBF10925C3F39E7C7E47EC4D848FB4C889C1DA9BA2AE70C9CBB1F9ACF13A2A2D37AF7EEFD765295494959F68C71F66E6414C9216508420C4F49550199622052C543571C4123C2445835B700BCDC66E9246BBF5B063329333D2B037A2D8397225001757109E7BCA41633A33E278704641D12B2EE0E597342D694DC7354A4888A0AF844C3C2884B03CFF07C03F5C701B775EEDC79F6902143DE1A356AD405C3870F1F9949D820E4AE1D20A5381CFD724458A96E52CB9B83B296C9E65437D0AF02C8328633822F1878B3498D2E49873F4CBA303D92747E3E33DC8C29373D3DD2362371CDB951332A323C645E1A52DFCBF56F85649137B52D45BD9F05DF0ED82C1E8FDD61EBCE0AD902C90543870E7D3ED2F5ABE4780777EFDE7DBB4C087380588E8521970E51795E55DA21FA36DBD632ED8016403796849908E18E306A7922AD218B813E2BD2F24A4899CD9B450A840724E32751C569393F2F9275E3327B262A16BF3C95DFDE0851E22B469AB7441A29137E900A1252521A6A7C5D9E3D23C67E61089B13B2EE09591342F89121F25B2154EE2680B4A9C373850F0A83C202012E2F69CF84C41F79755E85AA07545F142055634DDCF2662044ED9890F5508CF9BC64EE5332FBE3D27139DCE121E391A8DCC2A8CCD391BA234310723905F9D3A45CA77027FE423C49F282F2874B9A238991D65C91EBDF0D318BF22C2BDB0B22B133F26C1580A9F94D06C12E898FA89C22A1A378C8AE410125F3804C7BAF640AB283EA06DD50588790378AE9D8CFE58D91A702B656FF3E83981FA2A625DD383DB68894EC9E015C115554A99C16A93833DFC53EEC90F53FE51D3196D8CCB3A87681C6FE78A1ECDD405546970F2ECFAD6C38415E2681940029421295535A1657598035D3BC24A7C18E291C1A94B24FC955571C787D41B5DF0A548E8DB14C78A519140FC4465D980EFBBB983FC7A04FCC004AFE745A087A1CF2590A21766287E8B3D84701B11055B5F95EF23F9ECCCAB185094162751D755EE46D91D5E65C922BCF3391EC17EF49726943D933416D959E491E3BB6AA865FE1C0FE3013615C5F6A06C4E69C1455549B9E9318EAE1481655A45EB2702449B66D6334B3C8011DC77E75445145B6CA60A90C69A989288334D83F04D21F513D678268513E352E4BF3505562A9B7FC12C9D2A455210C59F6A4111E66E54BCD6049895ACEF591AE19B1534F0687872C3923CF2464E0357DF79042CE8A2495268D129022CAEE1669CCCA96E4A99F085E65C17BA81CB2B64C3FFAA75F9C8677234AD36FF691635139A13AA2FB861AFEB226493096870CD5359E8BE15641B069DFAE3C59387137A38E5C3B292C9DADAE218FC4B91EB9B63FB2292B238ABD61A3AC48BB4F506C63DB2999B40991F68B63DB8ECC67F6527F4AF33ED22D03904558DCB0FF417851DFD0C2B21C53215E24C935362FAC44DB8192059D15022011390535495269669BB1A7BA4A34BC5A5DBC15B22CDB2BB1886F6C1CB1D346BCF3769CCBAC4896FDF1A50A51B51F3E9B44EF15D6F094F66C30D475CD2149A2EBED0F63924C788D0D115487676407CC1449AACE70D7D43C03B9A44220C921D46723AD6B91A5DAC06E28A590B637A286569F3C93A12FB6AB6AF3BDF49F86088A555F152349586D7D28F692D7A54DB54A55695E588CB6171B7C5D5B214A47CDAC6A6A7DF685922CF114B24835C9F2479ACC0109B5A78BD1E530AADAADAAAD10A61F1C13095539209DB5197E52A50EA638285C22A58DD674924B6780C5526C06235B1FAF842C01A6F544A564C4087CC5563E0B9019DD6D63B7C43F54F10B315095E6BC70883AF1AABCB8F2379BB4A237D64799819C9693606B1BB599090311A0B21544BAA610A126E83495A36AE238DE902734D3BCA38D2BA7C46E8D4F207B4AD4129196CF5654C7D2481E423D476821757A38A81ABE38B2A7521B392DF343AB1AA5E928A24ADE476D18C9FA48544131F0FEEA95CD93948BE91404851E42821743D4AB49BD1E8F67B43061826A33C4248CCA8AD3381C6A66E30995439A051509BA3CD704550D89BED256EC030F4A55B8FCBAC4523501C188A686A4863A9352AA696082450EE0FD48D75D912C764D7C55932A96A69FC5D30B49787A05440EC4C28A4558312467B6B26735B899515E43F554914C30B72A259322595C37F29579E4858C3E12196995069362E5491A261D31C0954903952CFD65EF90A3FCE41902DFFA7AFD7A350F25B254C51298353F33DF10F52B702FB5401695B12A644F04EF4802785841AADF783A79A1BCB443A4ACBA50A2BA861066832D43B4A3EFF5F1FAF56A45ACCBE281D946547504D407C8A2C6C8971316BBA5F8872052E03BA9B0544F0D4F8B0D3B3C6AC930D79530CD750872ACEB3D0D6A1EAE6306C3C3588BFB2A2A963C936749B78420A271472459492655EC187B735BA4E9F110F57C0CFD358162A150A15107DE90A643B27D336CC32E77BF2AEA57E019566DD4BD7926711555A4963C983F895331155C5A339C97786B7AA4EAF266CD9A21935A3559B2E48DB273F68A715E55C2DCCFF329CF204A9D4AA068E11541EA60AA12825D6910972FA4B0DE885C7D6A34DBB32A8DCD12BB0814ED65E0B156B55229E5E0F1C44F5226A51DB576E7542DD4AAD4BAAC5C8BBDE47E226E812BA902843539E932833A665036581854A9543654CADC27E995448BDA4957C911AD47DAC021C92DFFBB46A26F37A085157BB4F68B6A4AB2D9AE468B971ADA4ADCA2BE4465EC62A11A6222C1637DA58C81173A58FC503494A3C9ED10A3DECED053377922C2BC53ED4ADC754FF3E6CD27C7D09F16D2841424BF494998CE98411226ED5078134A58F2B6978B6AB2352B2B8F14208BCAF178D2279503DE1059024852668B9074C82A8F9085BD9C17897A3F9EF1C5102638265D328B26D990A6E2C0668877CCBA7D5C3CA51A14696194956B9056939A3A8F2C8453311222DF24B1EA4C6C16723C8FBA8BEDFC43323B6EE48D56C0D950F792AC26DB8A9489EAADC04821A441066E53999A92014989783C1267B04A26248F54F9AC58285C2059544F04AFD25AC8769FC0D533EC4854EB527E91DFA94CB84F0CD824BDE28AAD906666954FCCB2E0D5FA22EF252CB02B4F8DDF964C2513F91F121DD5EB9553902D5A2F5502E5657156591E63F411C463961D3724BB3E517C93698534B6834D435CD9BB604196CB577EB135918D128832D688527F92D832F28A80E228F68A8AF95DE580534190E77A7E4982BF7644ADD8CA2010474D811715A7812A002FC7982394FD2B2AAD9E4EB59C470C30FCAEF13CCFFD5AA85C435A21CE004943515999BFC13B57065FAEAB7A1ECA33FE9FDA6AABFD1FC70A7FE0A41BDEC40000000049454E44AE426082', 'Fan ON 4', 'png', 75, 75, 96),
(116, x'89504E470D0A1A0A0000000D494844520000004B0000004B0806000000384E7AEA000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000001674455874536F667477617265007061696E742E6E657420342E303BE8F5690000139449444154785EDD9B09745D551586294D870C4DDBA449DAB419DAD899A673D3C1B4E9405B4A518B431DEB1245051C70C4591CC179A0C8721E70E18C2202222A6A9D7142ADA88815906241945A4AA553FCBEEB3B593737F74DC90BB4DD6BFDEBDD77EF7DF79CF39FBDF7D97B9FFB4E7A046548062723C3860E1D3A82E3E1A07CD8B061559548595959F9C89123CB393782E36143860C29ABA8A828F3079C3BE1252207484A25A8064D1032B3BCBCBC1D2CA9A9A959DBD0D0B071C28409EB274E9CB80E74B5B6B62E6B6C6C3CA5A5A56526686D6A6AAA1B33668C249681138E38491A0646816634641ECAB11A6D796A5555D56BAAABAB2F62F09742D0B510714D7373F3D56D6D6DD7CD9A35EB7AF0D5F6F6F62F2C5EBCF8D31D1D1DEF5BB972E56B972F5FFECC850B1776717E7A6D6DED589E2969278C389809E054F01288FAC0F0E1C3AFC3A47E0F59B780DB21EB81BABABA0368D68310E0F143F5F5F50F4E9A3469FFE4C993F7CD993367FFA2458BF640D26D5D5D5D376DD8B0E1EB6BD6AC79C3B469D356F39C713C7728383EA5BBBB3B7314994915E8001F043741D66EB4EA00BEE9A0E0F808E7BAF15BDD1C479FDC177D42EA51CCF4E8881123BAD1C06EC83C3C65CA948368D5FECECECEDB38DE8EF92E455B356B35F8B891A8B312158048560D381D5C07F681A32022A45848AA844260F7B871E30E829BF9FE42AEA9B9C787FF8A131447466AC13AF029702FE8375971A04D87D1BE5BF97C2BDF6700FDE2B12D6924C581B8EA2D046F04BF0307419FC1F70347C01FC0ABC034E02A7B6C4B1A410119D19F3482A7836F81FF80E4C0FB8343E057602B507B8F0D334C0CBE1721B99011034EB5AB0B7C1CDC03D48AF8C08B85BFF7399F078F0615A028079FE863E924FEE0709C0DF17B1456347D4903580C5E036E0103F55B87C1EFC19BC05CE08414226ADF90787F433F4B26F187C61B49027176FB7486A57F148E7801D734C5ABC15E3010C2F47BDF064F019A78BEC0D47E198B0D67F5ECD33F5152496B200E2475D604F1145C0D69E5FA5A60BCF557A0CF8913502834C1FBC0A7C17C3012E43341AFBB00F4DC97EC634925F9F062818C012EF1C645BF00FF0571120AC5017033782D680685A43B4EE4C9997E4492D2BFD24BB2914281981B4E065BC0D7C07E100828060F806F802783F12057AA13CCAFCF3DB17EF53A2E95D8B0CEDA99B4719D6AD489D058362898613511F8740E578177835DA03FABA2FEEEBD60113095CA668281A893596422F71024D9B7F871A9C4C6254A928C9DECE8D078C37128F1EFF8AD9371F473214CB25E04BE078A4D7F34C1DF821700B52A97094624C6FB13A4D073A59030639186854692C8DCD7EBDC28040D5BC9E933C1E5E01FA050ED9254D3A52F83C782D1206A2345FAB41D9742CF9552749A65A1913420BD564712607FB30C18A0BE05DC0A8A21EB2FE0EDC0F42967201A6F5704497E8F4BAE6B03153B1AAD30D9E03D89EF8AE66320F97CF05350285906A23B4170EC692698AA51987E0FA989BEF492C4B5AC13D15FE9D3B924905E8422065D26BE8F079F0477837C84795D93BD02748234ADEAD317AD3EBA9291581F522571BDE46445121AC986E43D9595956320EC499C365DD161E78BB90C60BDEF423007A45618C2F36B6A6A86CC9C393367B810139FA57BE8B99EB947B24A4F58BC91420159CBF9A983376DC9578990AC6F024D3035BD496B43C425792E732C21D6F2932EE391D1AC3440563D3F3D0F5889F83BC8668A9EFF1770F56C07EEECF41A48FCB9B924793DF3DD6759CDB53212BF274ADFFE7F985DBCC11B89E5863B83E147397F183A1B8044F727CF0720562236002B11A62FD972454D5413F4BE1610B4AAD7F38B95CC6F7C866668CCA87655110F9623A3880947B3301884671DB7C418703671F32C6CBF6DCC9831F57CD6B4B6B656B1F40F8DAF2C79C46739B03EA4215E33AF9B059E037E081E0481A0380E949595EDE0F3D9A0D50D59E0337D465689B79769332906D76AB790A829606D5555D5931A1A1ACE62DC1B216E12E752CBD50E4C7574B7E41C7E74515B5BDBBB56AC58F1F6534F3D75DBCA952B3B274C9830BEA2A2A21C2247D4D5D5F5F11B09714076C867DA19AB03CE6238A7965885306EB2E21934CB782A98A4C78769EB4F0B162C78DFE2C58B374C9932A591BEF91C9173E2126479AFD0F16B72338185C3D5C020F9CDE02A9EFDD3A6A6A67B9A9B9BAF64ACEBE1A24FF0EB2CF9009DEEDBD09E1D2CBB774C9B366D6F4747C7AE356BD6DCB861C3864FF079FEAA55ABB6AE5FBF7EE9BC79F35AAAABABF51FFE36CABFF84C8AD754F189A009E89C353F677113783FB8095872D1DCCCFBAC80EE067B3CA62FFBEAEBEBF72E59B264675757D7F6D9B3673F86D5546D307CC8B74961FB92E3BDA16DFB321B585B73A2DC48F931D805317B19C77F21E920A4DD42DBE773DEDFF5D2621BB52260CE65E71FE0C62328D0513A7A08D21EECECECDCBF65CB96BB4F3BEDB41B4F3FFDF44BD7AD5B772E03E89C3163C62C3E274D9E3CB9D2FC8FDF2A71E23CA73649AC13A2E9D9D1CF803F837F03632DFD968541EB5497814BC16719C0F731FF5DCCF6BD8B162DBA95B6AEC04C9E41DF4EE15AB6DABBEDDB9EDAEBA2B0143C063C0D98977E18FC00D8FE3FC143C0A037E4A94778F6ED7C9AF09F027A85219AC812E0439CE5F0A3689F8E8E75339BDD74F230F1CB3EB4EDAE4D9B36DDB675EBD6EF9E71C619EF45EBCEC65C9742D8046664F4A449932AB1791B08A4F9E977F7F724EA0BE04EA026FD1A38BBAF006E3EAC010E4E2DDF0C8CF4DFCF44DCD8D8D878F79C3973F6CC9D3BF76AC83B0B0D30B8556BE2A27BF0BC30807D1C7816F82CB806B858044D96A0689C29082B700882237166F421326F9DC87A51DA8FA35D623AD88D0FE9A6E34731897DCCF42EB4EC871B376EBCF2CC33CFBC18CD7B3A5AB81C6D1C3F7AF4E80AD45A17E764683A1B81A182E519237263ACD783D3803E4495D7BFB9C858FFAA038F02FA95F369FFAAB163C7DED5D2D2222E67DDD98405F85C35D709B11DAB1ADBC08BC145C089B15EAF791BCFE91BF3650CC2D2F5F7C119C0C2652461C6CF063F019641D27EDC0768DB518853DB8EA059875804F46F3740D84770C6E7E1DF1E8F9FE960A198C9A01C843EC2BD4367F606E080DCDAD7976836BA03FBE3040AB54402343793E77321EC1A26E13E26EB4EB4EB928913275AC56803D6F8D5CC770127FD67C044FD7EE0C07BACA54048EA9F805A69FB91D8216DFB65E08F205BBCD307D8750488F0BD04CDF4D0F4E9D3F741DC03DBB66DBB8755F406CCF592A54B975E40EC626AA323D547FD12BC1248809A641F24C7BCB11218EFA895813CAFDB61CDF30D5CFF358BCB5ECC7E270BCD76887B0BEEE2A35C5353F5359A583E33CB070976272A94AF23B143AABAE6A01F29444553A17FF33D044D9585E108C4EDC529FF7DE1C285B76336BBD10A89D2FC3E01F44D121534C955B30DE257F08C4D8408AB79C65408D31C232281666A2DEB739CDF337EFCF8FB69E30E3EEF82DC7B205117321082E2F03996840C29DC3B88447BB764F20EF037D06FB2E250DB24ADB6B6367A0386C1B9C238DB3AF497024DC7B6254242223363D01F67F03FC39C77AC5EBDFA8D68A4FB8DFA30EFD3545DF65FCDB37642EA014CD2376D7C76B16656085C29E5C5367B96583BE4529D2B472B058CD27D9BC6154AB30A26A69A3F0F5C0B59BB59490F4D9D3A753F2BDF2F21FCE59C37ACF13E619CF40CB003487E5A3BA5820B820B520F592E8B3A596B4B3239183314E0E2E1CECE0AA0D9A92D3AF079C0D84A7F7348AD44A38EB080685EFA22C308EF935C497685BA1294EABD8934C883656CC972F188C8B2D33A4E3BD5DFEDA942A14F31F0B4ACEC24055F251906A3C9C9D2BF7D0C9896045334FDF02DC22F0257BAC19C5CDBFF10D045F49065679D59538CC16CDCC93064301733B653536CDF6D2DC30A579FB0CC6B6246D86F0346D12196D2BF19AF191EB83B94D64EA9E0E4B983AE4FEFF1596E837B7240AB610E38789F6B5AE16EB451B9BE47B224419FE47B0B46F2122649D6E735018365E330FD95F71B843E15FC0818E6F8ECC198609FA966B947698A16899D75F3D399BD03946AE98D1364C8E02C1D60D53239DE4E18319FE3B01A9A33BA3CEB8B0C547DF1CDB8CFD72B75FEFA2BEFD374CDF57CDBCF80534D3565D2B76892B6552AE2E4C14CC35DA8A920123B6C1EF56AE00B1B0369CCDF3ADB9A90B995A18801E855C055F05648BA1FE7FD239CF7D91C4F84B8A031FA24776D244D87EA6CFA3D38764B32562E5C09AF076601BEF9F725E04686E6AD464A9E240E9438C9324D7A0930688FC44E18F3988D5B71D067A4FD381BEC50800EDC154D82BE0ACCDA2D1D9BEDFB42C81759DDF690DFDD49D47D3941E7E38891EA3391BA9A23714E9EC44992DF7515F6515354D30C686DC3328E6D988EB85364E8A116B850DC084C55E2B960B1C4F91B2B2167014D3F1267CD1934F9FC0EC89A482760E3CE9E1D7261B073A61B97005399279240B7836968909AEB8A720EC7D743D6DDC4517F2411BF8C487D3349770B795E05F74A8EA4D9A7409EA66786F1446055C494CCA4F8BB4093D58518D9EBF75C084CCA2D35BD077C1DE8FB6E036ABADA522869863926D24E84994624CE9C3E633D3076C9B51CCBB60D3A5B9A819B9E12AC637E1DB0A6AE963691FA8CB04289060D21C8ACC2DC8C8F2C035D8026ED2001BECF3F012C5BB6EC0A22F573D6AE5DDB3163C68C26CCB39640B40E0D6CC4A7F92CD322CB375F01BA09DB35E13767333EB422A06606AD54033565C321355142B7836B81441B7807FF963646E1F8F5B396752C00B862472259AABD03318AB733690FF0E13A523BEC6CF93EA77993B5F12D903319A811BDCAB901880352831DFC8568D10F20F2BEF6F6F67F747676FE78F3E6CD9741DC2B162C58703679E47320F83C087B2B84E99334075726DBB79A60526E60EB24AB81411C8BA469C20ED07F5F187DDBA6DB6866031F016EAB69098E35CDBFF95D7F683BFA4EDD4024217C70F3D2D9B28412ECDC1F69965632F543BE946109443537A135FEF08DBE064CABD7BE5B1219711012B60A12D4B06BC91BEF686E6EFE3709F7BDCB972FDF357FFEFC9D90F58B9A9A9A9B590876739F5AE080D48850FFB22CA3A6DAF7F8F39312160ECDC8761DB8DAEF268913AD7FF36D9EE0DF5C984CC96C4B7FA8091A0F46ED0471D6F5F8CF04AE34926318F19BCC7735CE59790250F52DD4D9D968494F12930D19B12D8B7ABE20722E647C1E0DDA05690F41F85112E3431C1F84A8C35631B8C709539B5DF1ACB9D9BE7F988AFC48CAF393E24045083D6CDB093604711FC005C885C88CC057CF5D554DFD74EC8ED3DFF4121F641AA1139614571CC37CC30983452B9506AEB3E9641B26349AC1A492D4D0D0D033DB6940BC1E08D3214B80A55E1DEA519E1FD5C838169A865AB50338287D9133EDF67FE413539E9F4B6C5B330DA61AFC9B2E488D332C11FE23447FA99947AE25296100C6383A46097215538324721C4EBA019286A575B250206196EDAC2B986419B56BF681A4380C65F453CEB41AE100529F1D50A004E27441C1540D808DE59C10170B898A263F2961C6FDA133578B7984643720B573852226B665279D18733F83C95C2BB069D8C540EDEEA35141D2CE1520F645383E1DB91CA4129494F0C3E8E678E3FD4190F871467CBE09B1E98DBEC2B8291B599E3746320BF0FE3E9A9D91480B52CE17230591D44BE20D0E041971C6929D70609ABC81B02985AB5036B284FEECE7C028BD324B5B152C08C353CE0F9A9444A3E208CF8C89DF8D81ACCEBE13E4D2AA004DD1155A53345A4F6D079FDA4BEB32D2E3420645E20D160B24DF7B4E765CAD327D31633052F6C67CD0D11B855B7B4B8DE93CAFC4BE7B6204246AF2599DF58024DE817C203DE9E327E2C72962A75D71FC9FA0715CA1FB9492E50B24A62FD5F1369350C231B928EBD4B0911026518F1C5948EA0C07A4885AE50AEBAA6619DB724A3E130CD0142DF998A85B592D2818C68FF599CC41917823C5228B18D75821B5D2697DCBE83C4E483E5846D614AD9E8E4A6BB7100C86442A9BD65836642497AA1BC798D89AA0BA0A165B3BB3E261EA63345F97D6874281D84F5D4249A428B232F7E75A75BC6E206AB5C097355C05B345EDD910622E633313FF9C6E201FF0636A7AE956CAB44692C8888DE6D22A2363977DABA63AEA62890AB00A620DCD1D22C9CFD6A7BC4466A4746429690D052012948B24C5EB3A7613F57CE94D3E48B2BFBF0098EC9AB7F6FA0F1112F5277E2E1B0645B23464A70A9919EF33D63141B7266611AFBF64F93B6B4F161FAD865AB9189BD6BF34200FFFEA8848920EBB10B2BCC7A2BF45B742FE51910F2E0CEE3F3E1758972ACAD923834F58E2E112200A31C190DE58657573230CBABF0831971BA0D6A2D4DA9EFE258124C9E931D3634D24D4F4C612ADA5DA52BD9FE0735C55DD687132723AF4F8758F3338E644B27C0D33BC095D687A930F9AA2E992FB86D17B13717292F03A3826098A8BB18C154E4BD49A8E816572E0FD81A67817F01D0D6BE5E56924052021A63A6609B363968FAD73BBF1EA3E63294C50F81C2B161605DD5875DB2BF9DF462598DC31AF558AC1A8FF81311F7460FA1AB54BCD2896B870BF5B756E59B9D3EC76961B0C926590AAB9A9417EDAF671617E41ECA80ED84A83EF53B8B3ECA6A9757535C330423F2624413224D2245B48ACD7AC5098EA98269957EE0012E54686C1A91A2C319A9B38AE488A8B1D7797C808DEAD261369DF5B50D37C67C17F859902F976B024BACD2E219AADC4FAD711AB0DE6846EC5BBD16A89D93F4A499493611B278438C39A84A989853F77757C55D20D087345AB08A62F92E0BB59FEC3D563DF71701535F874FFD230C1A8DD7750FDB39413A0E969762794489883D244C23E9DF197C9B5DAE1DB306EAF5B4DF0B5001705DF9EF19AFB8B6EBF4BB601A879A6D9C3716B6AC54820CEC10A07EEA6A6505384C76AA3D7E2C4043C8C72D249FF03CF673B1A25F51B4E0000000049454E44AE426082', 'Fan OFF', 'png', 75, 75, 96),
(117, x'89504E470D0A1A0A0000000D49484452000000260000002A0806000000DFFF29D5000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000001874455874536F667477617265007061696E742E6E657420342E302E338CE69750000014BF4944415458475558775454D7D77DCCC0CC3033F461402982A0A268248034E9BD4BAF82A0A2B125F197AA3146D32D311AA38925316A2C319A182B1A62038D22184591264D2922A0D158A8B3BF3D93FCF3B1D65BC3BCB9F7DD7DF7D9679F739F80618806FBA027FCF707401802F4FEC1A0E43E9E296FA07DEC865B3F6DCD3FBAEC69F0AEB918BB711A7C7ECE87EDB670C8BFF18678CB1418ED0E85C99E7028B70541F86C2284F74743EF4317483F180BC54A57041DCE4741F90ACD777DBFB795E0C6F266B44F7E862752ED5ABA4B031106A08F212831A451F19E9980C181FF074AFBF71083EA3A4DCFFCAD7FFDF655D68E374E4FF828A6D77AB9FFB0DD973170DA960CDB6F2261BD3514FA1B3CA0DE1F09C5F6001D28D9265F081F8F87F1167F986CF6816CCD4BB0DCEC07F3755E18BB2D167EBBF391B06781E68BF21FBA0FD515AF6B46D7D847E8377C864183FEE121C3E1C1214B82B41D1AD28CD101D1FEF593A52E3CD56BC63F23F677FDF9EBEC92B583413F2DC0A47D39B0FE3E02C6DBA6C2625724AF682837074358E30E83AF7D783F08F27504B1DA13B20DBED05BEF09E35DA130DD1F0EE9F7DEB03C1801F31F8360F56338D4DBC360B12118A336C6C27747FEF0BC924FDB8FFF53BEB9050F539FA0CF6610030683FD032282530A7D78267EAA79267E8467926AB47BADA9DA7B21EEF0DB03765B923072EF3418ED0981E9C110E8EFF28464B73F84AD53183E1F581D4C847C470884CF5D21FAD40D1202936F0E80645B00C43BFCA03C1401B3A391901FF087F12FC150FD1609937DA150EC0C86FA5012D47B1230715F06924FBC3EBCE5DEE1FBE5FD75071EE3F9A47E0CEAE9224750C64FF15CBF5AD3E4B1F0F8F272B76F1287466E8F84FA402C842D1ED0FFC1038A3DDE30F8CE0DC2D713088C4CEDF286B0ED65083B7C20AC9FC4CB1DD26FA742B49DC0BFF385F44008447BFC20FCF032F476B943B49B63B78CE76F93399FE37F64788BE3A1F8D107A30E44C1F5FB64E41E7E07A52F6E5EEAC533BB1783437A646C50C6584FFCB0ECEBB39337C50D3AEE8B86E5E1684EF680F47000847D1E30FAD90F362763202120F98140181D0BE57D2F18FEC6CF1D5ED0279336A7D360519C04BDFD81501E8B82E2683881B94176702A54BF47C1F24C0C644729815F09F8900F4C2EC4F3191361762C08F64792306A6B2C669D583170F959EDFA175AF13FC40BD5C18E33DFE5FCFEEEA0D5F7C1B03F9304ABB224884F0543713E1A46E708F2A017469C4B84C5A9383ECC0766E7E2A07FD45F075CFE5B10CC4E45437A241492DF4260742A467789B59BFA61D2BF60385FF5E734C8CA38FF8F30185C8C87C58D74884E7A417D2506165CCFE4E768B8EEC8C0D2D24D77EE0DF68608ED78E2B3F8FCDAB6C907B231F27422646722A0BC1403EBEA4C88CE86407E211A86A723740B599F4B26939EBA1DCB4E87C2FC5C144C395E5E42760E4F851E2FF16F811C1F09F3D26910930D61BF070C8A83617B2B07D23F136058990C79652A247FC6C2AC220AB2D240185C8E86495932C69DC841F6A9A5837F3DBBF3A670FA7EC5B4FCD20F5E381D4FFD97DE520ABA840FBF44B62E728787FDA038454B3841211F66A61DE2623FFB407A826C9D0F83FE1F81108A6913873D213E3255075A54CC64F983994C964C2E274171261A7AC738EE641044DA8D5C4A80C55F69B0BE9902E1CC54E85F8F8311C18EFD330FF9373EC685E7D7370957D190F246DBE67EE7B20C282BE321B99D08F1D528884AC361762501C2113FC88E87C298E1312B8987E1893048C8802577AE6418A497B85059140C4A82617A2116CA3391303CCFFBDC94A2320586E564A9341A72B2AB8D86DEA920E89F0983F1D504B24699FC19067175224C6A3330FE562116747E890AD4150B5735D5892B1F6FEBF3AE9D0DF33B69106A988D37486D2DFF3FEE45603E509D4F80D9D978189005491917A962C86FF0C15763213E1F01E9D908888E0740C1DFF5C98A703A48172A49252DE5A636740930288D805159245404645A1E07036E485CC18D721D71430ACC9AB3E1723B0FAFF76A81DDAA106EE066D0DA819D0FFD9A8A60D6940EA1691A843BC930BD9B0BE12853FB086DE2A42F41516F5709EE5A2C64B5146B53966E617129B38D0CC9C996793993E60F5684731130A94A81A2260DD2DBBC574589948743743E881B0980AC9C20AB9320F059068D19101A9260DC9285D1D4F5829ECF715673F12681DD9AF4C9F3AD355E753375A8C51D04D4CAC18D8CFFCD300897BC21A90881C9CD38482B23215C0E85F46622A45C58FF6A220C2868C9E50428AFA5C2FC562684F321D0BBC0F068E570831AAD8A8250CFF177D36058C74DF359DACBA09ABF5D61C6D61034C950B667C1B18EC01EAD42715F499D50F2B4D4F9F58ECF4BBD9BE760446721E4DD859074E543D13303C2ED104E4882EA7E06E475A49F8B88AF532B350CCFED54828AD73166402D4AAF4D8369751A0CCAA3A0FC2B41C788E81617AD279856B2733785E05260DC91092382D4AB235B4DA930BC9F0B83DE6C1875E7C0B63E03795D2BB0AFF7D746E132EE38BCF978D3A9A95D8BA06ECB856147160C9AA9AF8604983CE2F7AE5428BB38B925057AD50C2577AF68CB84927A945F8EFD57C83798083712617895623EE307F1854018DF4A60C819EAA624480946F6200792F60C181094A4959B224BFACDD3788F201F24739D74D8B4E620A367250E0E94740B6568B49DDFBBFEF894AE0550B565C1B0330392C6545D0218776641762F1546F773206BCF86BC2D5B07DC883B54F5E4D1C5D9E2EC19435009B0E46E4D2A63209CA2CF9D7267F230D39AD3A1DF488DDD2590AE2C883AA8E1B654C8F869DC950D697B0A0CEED31B3B12A1ECC9C0C8AE3C44772DC1DEE1E241E12C6EDB173D5A5F3CF9C13C187277F2FB9990D6515F97C36052C31094D2A3CEB1EFAA656D2340C35E1A654F3214DCA5C1AD68C8C8A2F5FDE918F7782EC2354BE0FE600E46B66432395220A74EC54C24A12509060FB2217A40C6BAA9C3FA48E8DF6525E8C9847E97962D7A68773AACFF9E8129EDF3B075F000136FF8B24BC6FD8FEA9C3B6743D99909456F2E24F52990DDA220190EA194A1A90CE32249BA5048C996A43B09860F28E83B4CF73B5CA095E9DE9E0E5B3264743D12A62D693A314B5BD2752CEB7764104016FBAA74C81E329BDB92C956920E98B8931B78C0F0B64DD37D8E6F9B854F1E6F6324868AD785B7BDDD6FD53943C786C1A31C8691D957438D318CD2861828A909793B77A75D8C22953EA090EF25405CC3446820408646DEC37914B450CB5AC9308B2909E11E43C939FAF7088CE0843646A09575B69999DC424D728CFE3D264C570644F7F9CCDE4C38B6CDC49B8F3641D8FC627FB37FFBEB30EFCCD52D2AEBA15D54B26BB8150B697322B387206FD2449BD260DA339B21E02E9B18563E5C799DDE55C3CC62D8B40921AAA3CB932D3DFA9201BF5B3C2922CB64AA2159A7511DB87A7A5E5322CC38CE849AD6314AF91832D1140CA54DCB0CCCEE580B61ED93ED8D53DA1690957488EB697477B3756628D0B394D4863644F22A7ACEE57098DF9B0593BB33584212A0AAA50EBF7382D1711661D6564BFA9A5D6D266C78892E8641DD9C0BBBEE221A6736148D5930EFC8A755E4C1B08159CE0D09170260D290AEDBB0D632B4592BEFCEC7E8B6B9C869FE18C2A78F3656B9DF9DA39B20DC8885452B2DE25A1C2457C8D425B63695E130D2D6B413FEB0B89E05CBDA7CBA3F055FC3DD6F7381F141B63707A730433DF8DD11C2CEB12CE8FCBF984DE3095F88D80C98DCA6E0CB09E67C20AB05BDAF44DB1271EC950808574321D69A793533BA3A164EAD4548AA5E3A247CD2FBF575CFC6393AD737B8930A6B9AACC975FA126BA2296BA1F00B4BD24EDAC24E7748F6F1813BD8859E0E865D432184BDEC6ED975383616C1BE69366B66144C58095C3AE6437185C6BA7D0CA47F44C2BE9E634F72EE1E57A8ABB2617B8D9A3A1D086935C7D4D2EF586944D4AAC06A30AEB908D9F52B9E08AB1E6DBBEEDDF00A542D64AA250346AD0CA5B69539E4A1EB20845FD946EF9A0CD531661B7B7FE1DB0930AD48854DE34CA6340B362F877BF3A06E2C80717932CCD8518CEF7E1516DAF2F41FB0D14D73201C23833BC6C1B1762646554D87EC02BB5C4649D69A06FD06560D6AD0B0310D4E750578E5EE9A7B5AF15F0A6A5D0C754B0E4C3A7260CC0142196BE4199E1359FFB43B959009871BD331F222293F3005EADA5C760D7C6805CBCE25367C77F221BA1A479D512F0C9DEDBD99306679128E4C81A2621ACCEBA6D376288D23EC846BF26051C1DF7EE3D9810C993FC8A547C6EA80C95B3375C096FDBDAD49D88BDF4B121FAE8075CB7418D4C641C972A46E9C8E110D0550DCA4457071D3AA74A8B8A85DFD745D191AD144D7BF180AAB8ED9D0ABA2B36B3B0516F7918DB9AC93F130A7E0C52CFAC6F5E9306FCA856933C7573063AFD348FF4A87F262028C2B1261D99C096366B4B67499DECBD2157CE73BB3B1AC77DB036157DFC903F12C9CAA1ADA40256DE1460C4CC9863654A2724E6E2C844D3335723104E6777360712F97F7C8EA69EEFED13C183DA0FF691F5A1902CBFB79CC5A9A2C6BA3B633513D2A84454F21AD85E32BE320FE93022FA6D67EF1810D75665F97A5CB50C39B09D0BBC982CF4A33A66501DE7AB06D48587E7563674CED328CB8C6C9BF735209B570CC0FFABFB3DB64AF6FCE078CAE2F80E462B88E012B323BB246ABC32058B417C0ACAB00323ABE70D99FE1C884BA3B8FF768B6150150F55277DC8C252D467A25118E3505185D998BD165E9B0E7F941CD565E8FD92B3A477B62A8E5551970BA361B1965EF43C8DDBA68C8FFD87C4CB854048B3F18925FD8B7EFF781FE3E1EB3BE63DAAF1FC3C32EB3EF572E7C361AF6951970BD5108BBCB196C530A30B27526540D39D06701B72433DAF0A89AE8E83C68A89B7260453DDA56E7C3B1320FE3CBA6C3F3420154DA57094B6D682F1E901D6642FDEC0F59711CAC2F64C1E3EC2B08D89CAF117EAA3ED6326E79CCB0CB7719B0DB970CE3DD113C2D07426F330FAAEB5FA2038F87DE973CAC6EE04590C23AFAD42A1708ABC7C1F027B2768E665B9E0A057B338B5BE93AABD1F6F38664417A84878F1FBDA1BF9D99BDCA15920F2742B47834841996108AD4B0D8C223E28E20A87E8AC388FD8970D99B8691EFFB63F1AF9F760AEDCFDBDE7D77CFA7DD46692EB07E8327E32F2261BD391C16DF0641BACE1DCACD7CF09A49D05B3311465B79DCFF2600D2E5049C6F0A61AE29C74C86F8072E7CC81F2AB239A28A2EFE3B33902778D1E7DCD4DBF610E6DB403CCF01D2027B286638423FCB16EAB73CE0B09111F8261AA337C5C261254F66F16AC42F49D2543DACD9210CE1A9CB9586AB15994B0A601EE10C598203AC5FF782CDCA20587D1600F355BE30D55EEB08EA4B7F987D19089B35A47F861A42A125ECBEF483D9CE60288FC6C0B9EE158C6D64429CA2696EF783C5276E305D30066645E3609AEB0455A6336C72C7C369A607DCDE8D84CB7B61B07BDD07C6596360186A85809CA938707277FFD381BFE708839A7FC6D6DFAD3EB76EEB5A4465C6C2DED70566BE8E300A77866AFA4B70782B100E1F8561E467AC7FAB4330E2AB08D86FA4C72D9B00214B81D1046A772011EA121EBF5A5EC584D6D760559206D5CE08A83FF2846A912BECE67AC27E8627AC92C6C031CD152E192F636CCA6458053BC0D2D70E56DEA3E095E08B796FCEC1C5CBE73BFBFB5F24086D9D2D765DDD2DBFECDEB315D3F3B21012198CC9FE1EB0F57486F26586CADF14E2480B28B31D61BDC813764BFC30E693408C787B328C67D8C1F9033FB8EC4CC1D8E379702D9F0397B242B81CCE85FD5751B07DDB0BF673DD604530F6292F6174D2445805D9C1DC4D05AB97AD31CE6F2C26FA8D874FA837A6A525E0DD256FE266D5F553181A76161A9B5AE47FFFD373F254C95114CECE43645C38FC43BDF092F768844C73E564238CF295C374B2014C3C8C20F7348659B01AEAA891704C1E873105EE70FE5F209C5686C1755302C6AF67489705C376B63B6CD32760448C132CFCED217DC90C266E1618E1650D273F07F827FA604AA407C67B8F45607400A21322F1ED96AF9F7476B0531C1A36149E3C7E217AF4B827EFDAADF267EF2C7B0BE1B1C1F09E3A11A11113F0FD8E37B0EC8358E4E4B92024CA0AE33D9518EDA6848D9B14B65E86B0F65440E56B04A98F2124C14650C49A4112AA84C4D7184A2F5398BCAC84B9BB112C3DCD312E6A34DC932620616E28167F3C07AF7D5084CC39A9700B9C8C80287F64E5650CEFDDB76BDF3F4FFE1E31F8A24FA27BBD3930D067D6D97BEFF0DA0DAB079333131016E5050F6F353EFB7C1AD67D19832FBE88C0C245E331FF754F142E988CD756F8236D9E3312673BC127C5128E41623884C8302989E18952C335C21E765E2618E126A266F511943F06859F44A3F0A3282C5C9D86859F6622ED95084CCB8F817BA01BE252A3317FD19CBAF2AB977C06FA5FC8746F1431003D6050ACC1C0C4CAAA8AB96FBDBBB82E28C443E3E96D8DCD5B0AF0DE322FB2E68E4DDFC463ED57F1F86C4334DEFDCC0B5FEC88C3D23501C85DE08CB9EF7922798E13DEDF908255DB8B10993901F66E023216B8E155264DFEF229786D4314F296534BAFB991356F4C0A1B8594C2789DBEE292627A0FFD7A20FCF9F3A7E2BE17CFA40426173443C362CD709F7868F885181816D7D7D74EFAEAABD5C7A2623C1FBDB7327D78C5E709D8B0351BF3DF9C8CC8642364CEB261082CB178F944AC58E38FF73FF7C3ABCB2621FF3547BCB92A100B57FA23902C5ABA0A58B2210CDF1E9B8157D77A61D16A1FCCFD702A167D1A8FFFAD9A8185CB6723292FFE61C12B79DBCBAE5CF07CDEF70FD7D7BDC156605823622887E5644CCACB487B833F887ABA3BC714171F2C786759FEFE590B427B8B5E0DD2CC58E885C5CBC2B1E02D1F2C7CCB1D79731D90536887C47413C465C8913E4B8D985C1344E458C22BCE147E296AA42C74C2AA1F3351F4BE07C14460CE7BA1C85E18D457F04666E5C2A5F3F6ED3CF87D4C4BC71D69DFE053DD7B572D161265A57B07AB010C7843C26B24418DE3E500CD90B906FDCACEEE46FB73578E4DDFB27BF5D1773E9CD95AB430B62F7FD6D4E773E7070DCE5F18800F3E4CC51B4BC3B1724D22567D9B899C459330F39D2024CCF643488E077C939C913CD717C9857E9AF459C1BDF3DFC93DB971C7AAED67CACF8CBBD3D124E9479F0E1065241AC680B14633E4C0EF221DB07F9152671A5EC350F25385E1210B0E560CA0DFA00FCFF49FE29171D79346A786962B6EA5977E72FDF9C09A256BD7BED2356F5E041252C7C1375401EF083926048A3121D804E382A89FF94998B3341F1BF7AEC691733F0D5CBD7D71F3DDFBF5F68FFB1FDAF66986FE6308C2205EE81194169848A3D1FC771FC2FF014BEE92A93EE8AD010000000049454E44AE426082', 'Lights - Alarm OFF', 'png', 38, 42, 96),
(118, x'89504E470D0A1A0A0000000D49484452000000260000002A0806000000DFFF29D5000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000001874455874536F667477617265007061696E742E6E657420342E302E338CE697500000143B494441545847755807545467DA1ECD6E369B645D4D547A07E930BD778632F4265840A489020A52AC117B251A6309BA66DD184B36C11493185B0C9660C10A22BDDA0BA0742933CFBE774CF63FF9CFFFCF39DFB97766EEBDDFF33DEFFB3EEFF35D1600D3F8FF3E686A7EA3E587B37EDFAFDCD4FC45C65C944487E158763A0E4787E3DF3C098E7B8B70DA5B805291081B02C5C80A136076300F7121BE888DE522275E8153E9A9C627EBB67D6DF8F1EC0434B78E353DF7B7797F1FAFF06ACC100C63060D236360C43BFF0565340C8D319DFCF641C7CBB1D55F1DB5F9726141EE4A4DE8AD35F2C091EDBA20142B64D8A15363A74C82837C298E0A24289549F1A956825581422C0CE0617190182BC29558142632FDF6B94C89C35C9571BF36A2FB5856FEB1AB7B4B34EDBFFEF257F4768FF9DFE098A311F80396FF7E3AAE555B9D58BF6DD5DEA8D8D64F147E867D7C7F1C100660BF548B7D0A254A4462ECE6F1F14F991C1F2BC5280A106051B8008B830558AD11E09F4209BE11C8F1154F806F38029CE52871CA438E520F25B1EC8FC38AA0C1836171E7AE7CB82B72A8AAF62DF4FC11E0B0D1F0476078D231F6F69E038A438959B776AA4247F7D34A0F70C538C6D7E12BB60A87850A1C16CBF005578A033E021C5268B055294161100F8511626C0956E25F0A394E79C971C955860B53F838EFCA43B99B04BFBACA71CE4D85739E1A9CF550E0478E9FF1535160F789ACC5DFF49C38E383674FDEC070F7D8510C8C31C2F0C66F905E7F4EE67D20FB8748FFE8A0AF0E47B91A94B22977D85C7CCB93E16B81124718506C3E4E794B71C65B81526F09FEC111A258C6C74EA508A514DA931E625C7416E38A8B18E59E229CF310E0828708175C84F8D55E8CAB76125CB715E1B2930CA77CFCE99E20E37E4550F389DC82B986861A19867A27C238F23F8C55947C62F39928A8F60C27C458E6AEC3CF4E2294790971C6878F6F5C7CF1BD9708DF5192FFEC234505ADBEC299614081E33E0ACA3319B12A453985EBBA9314B768D2DB744DB9331F175D78B83C45886B2E12DCA67BEE3AC8516723C61D6B31AE39AB71D6558D1FBDD4C6CF84DADEF36BD7D51A5B9BA2303438968581A131FDE5E56657F396DD3CE9A1355EB696E386AD0C771CE5B8E528A09B0528A7E37577253D588ADBB64254598970D7568E6A0725AED3750C43179D85B8E1204215B1526927325D77C3866FFA5E632F418D1D81B515E3960DDD4BA3C6969E6543C34945732871DA578B4F5581C61B252535E87AEEC5C2B397EFDDD8547CA854E267BC6C2F459B83168DD60AD4D1E4D5D6AF1F7CC755836B9622D413E8262B291AEC94B86B2D337DAFA745DCB513A39AC2C34C586F4B200800735E6BCFFC4FCFB290A2DE92D8A2F33B346A99DF6C88397A0E735E4BCF2A27A6BF9604E0FBF9B9C6E196C68DAC815BD551C792529F7FCF969A686E1ACF431B3DA8912EAEB557E21AB332F74054D8A95063A342AD951C55C4D42DBAB69658A8B514A29E16D144D7355B8BD0422CD4D16F351622D439A809A00A0D961AD45BA8516FA7A3C5D2D15A8B3A6BE6790A3430C34C844A2739BEF5A5EACDCCC1484BC366165ADB232F2CCC6DFB89C49259F5437309EE9B113073293D44893B2E41A8F58D4623672A9A5C83516543E1B353984250472018202D0482190D6602022731B1DA6CA74195A5929E19400C07E3817B2C1EB1A7A15B918291C005E851CCC61D4B629F16FAC08A98B493E204557EC5920F8027F75790903E91B7ECD95EF1A39272882AA9C54C8C47E64ADCA355D659F9E101671A4642F33110928397EA343C13CF402B271A354E7E78EE1E822714FA674E3A3CB453A3D32B04F79CB478EA158A0E760C1E7A4DC50B693ABAA419C0CC7540E64740FA46606E311D37A0C525100F1C680E02564905F1A3B70C57F21711B07BCB08D843079C3B71F44278382ED970718F56C0006B99284723ADF6993011C6E01C74C8670329EB81D40D30441460409B01836C164604D3D1C78EC52001EE12C4122333D0E7371B03BA39C0D455F43D1B885943F76EC5706421FA4273D0139E4B403FC073EF28BC624FC5531B356A1CA9B2BDC5383F3F0BB857BF94859EE7EF5C59B968CDF742AA1E273585528587164AB45A28506D4E39E4E887614D32104A37A4AE06128B68125A75DC0A0CAB33302C4FC1A82E13FD7E19E8F24B456F44265DF701FAE30A301CB1149D92797825CF01F44B81C05CF42A52F15C928836B7503CB40DC490570C3AECFD50ED28C5773E425CCACFC168F595A5AC9E3B37C79FCFCBFAE8385F42F923C7BD49C4D664CA1BCAAF465BA5298F9A1DA4E893C762583F87264F87217639FAF4F9E894A6A04B49A10A5A00A4ADC7D06C025DB00D58BE1398BF19CFF579E8D5E46344B200AFB8749F628E89A55A2B2515032D7E229160AE3185F2969D10DFB245B8B428C7F8E2CAA95C16EEB74CAC5AB97CE7D79E3CD21505DAA97ADA6CD5A83313A2D5528C362B21E58E0AD0C5A35F45C9AB22F6628AD0AB5F84279A6C748617A06FDE1660CD0160DB21A0F83360EB4160D92EF44E5F890ED57CF4B3D301E11CF4F8C4E23EE514331A68F10F2C74687E5F46D191E2A623D3FAC4285B98D1DB57F94B280B0FDBDFAF28CCDB719CFADF5D670D25BF120D5491ADA44FADE6023CB015A09F1A78AF3418437E09E8D3A6A05FBF10A3F1ABF128340FC8DF4D404A815DDF003BE8B8ED087D27804B77C338E74374681610635918E5CC261952A3992AB17EB210CD54F96D93E5440431477A78CB5148AD4E88F369494FD17C9D4D8C354F28CF5EB0ED07EA6BD5A4E26DE6F2D73759CA4802F868B4A282F056A24713837EFF0474A912F022201DC6E455C44A09B099406CFD378D2FF134A10883E99B81C57B8042FA2F7B07104BD7A9166094978C7E9FA9E8F18AC023171DC99110ED0C01049221E196992F09B90227C49ABEDA839F04938ED54EBA9E9DFBC9192A55A65D302B6AB5909BE86D21C1ACB1E4A2D29E8F7BC25074EBA8DAC228B96791D6CCA7F02DFA1886159F10535F025B0EE2414C01BAA62D27607BD139732590B409830154C142920BD93C409202507577B323709FC4BBDD52621A0C4846A0AB49074FBB8A0C2D9FEDCE65759EFFC9F57458F477E73D08182539D3661A2D882D2B198594674AFC369F2074681309104D3673C56B2D4A5E89E1244AF67C4AF615C4CC5A62A888C2CA247FC147E84958894E2D09A93893923E17A374ECE5CEC2A82499CE13D04E05D06046624C7331D169A7426384F99CBBD450B37DD34256DDC1DDBCAF7C846515D4A0ABED15AF813114535532ADA68114BE5349A092498BD2B6A03FA21003E10BD1ADCFC4CB9079189C9E878EE9D930E6ACC64876115ECECA475F32B146053112421211B40CA38A85E8E393B448E7A19B3B138F3CC2D04242CC7407A687365121DCB7529908F9C545D255BF69DD4C567349B1843CD445A6993295728FC4F50115400B69182315751EFEE8209940EA7A3C55CFA31CCB449F922A8C5A0A03B84B370BEDB2280C46A5A33774369E68A7E14528858E61358D943E712B8C01CBD0C949C353F71968760EC34D172D7EA5B4B9634FC24A6C3DB0D5A261A218CDA406175D152DFB75BA1056D38E8DDA324FD94DC651B44E929974E59E9516B5849EB12575FC503C8BCBC5B3F8023CF4CFC223792ABAE54918D22693C227A14B3D1D376D796475A87A5553A97267A0574D0C47E5A13F240F8FE573A968486C03A8D5042DA26B7538374589B3CE12DC247751478C359A51288931E6FC573765D33E994ACFAAFC787D1831D6C0D810E6CF164B3559136AA614D61B2E4A5CF2D6A03A6236EA285C8F1317D1773DEA7DF5E8D14E47A7261E6D822054921FAB7610A25F140928A6A18F5AD303DF483CD3A6E3495036BAA316E329356ECC28C235B7409C268379660AE3C76468B224427E0B239346175DE54D7BC4D26056E5FEADD13F0AE44DB7C9F2369302D79B91A650625EA612BE446DE2843D0767E481B81195804BAA481CB366930DA2B013E0566E80095495BD902C10391327253AA7F8E389A33F5A1D68F8C6A0559A80C198422A9C55784C1A58EE48BE9FBCFF193291B5F62A53F23711538C67637CDE450F65D3B939E9C1ACAE6BC763CAE263DB2EB80AA805BD16BCBB93A4A821D770939CE5197B2E8EB9717156A0C11957117EB6E1E0D61409B9532E6A1DC4A82373C958E55A7311314E1A384962CA53464C1BADA8835082B7930B69F7094735F562C61D5792A5BE692536B53C46CC19EBD340EC31394D69D5D4B07D63306BA8F552F2F582CCA727A925DDA57EC5881EF3D06A2A84DB04EC92AB04A7C8B797B90BC8EB4B51E54C8DDD4561BAF6CE642E6AA87535509933ABBE4726B0954C00D3D65A262BA880E4243B0A62855C2DF9B56A7B0245ACDE76243F47206A18DFC6FC4F697497B48C01F6B3BBA4A976E796605667DDD992B2F9C9032779425A0959E3895C3CA1046D2187594116F9AA9BD4342A68735245B6B996317704BE9184B1992AB9DE9A1162CA1133050D3A9A13286B0D4DA8344DDA622531B5B626EAB9D5B4A82B536428A3FCBA447B883B0EA495C4543DB1DD4C8EB99944F7AC87BCED76F1BAE9AC637BD6741C880D367EC717A2DC968D76478D49366E5AD00683AD36ED8C8EFB8A71CE4766DA1931B69A0949B5838A3A821C95D4E89904667A6C13816AB2A0FB2DC9825398988530AE81C9C3EBEE529C72E5E3075FF2F67C051DA9021D25263BDF4885C6141F530CC73D14AF0ECD4EAA60ED5C9C858D6A39BEA4CD6C052525B361A8B3D7E2BC8B0C2562213E907A61AD8A8B5D2A294A397272994A1CF351D291B66F9EB427A04D04B3E1602ABACD9C2A8C42C98866254D76D94B899F68BB779427C6E76229F6CA24D81BA0C57A21172BADED709AF6A6CC5E93D96955D0B84C9AF9850F6DA283A34659F5E7CE6085BF0EBB245A1CA5BD60998B0A17A768F02D47818D7A2916C5CB901721C132AD0025B42BFF175B821D4209760985F8DC5748FB4CB929244C8B6967728A84B986565F4E9270C447440B9263B55E8EFC3029E6F87391287747B49B25B2DD9D502AD1E0242DF2849B1C277CFD708016B299E63DBBE5A35ED6C0B367F862EB36CCF264A3C84382FD021D8E0875D8435BFDA5217C2C8C932037528235410A1C94697140ACC252A137A6D94FC03C7B33FC8BC3C765DA4F32B9C434E4061BAA5817DAF1B065D8A29161963F078200677884BBC13BCC0D62BD3B54227B646B69072F91E1883210FB046AAC27F79AE6E681F58989233D0D755B58C657C3658D95B7FBD7CCCB4698BD2B921C3D90E346E10BD22237568E8C3831B2A30958B8069F8AD5D82F53A3482F430CDB12D1EE93B0D38F72C59DB4882A8C7109CC4EEB0A85F8B0508E850142044471E03A8303F399BE9814E90C9F380EFCC2B8488DD460995E875C1E9700B921CAC101893A354E1C3DD26D1CE8B564198606048F9A1A7ED9BFFD2324848642E3E90D8D9D3D74EE8EF057B9229E80A54F576149941A3BE42AEC53E9B03240818C001EB43E93B12E58657A49D24015586B213079F75F49563E1748B1205C02BF783EBC6608E03C930FFB480F7884BA43AEF7854EE50599B723C42EB690BA3A209C7278C9826C54945F786C187AE5C47AF8E0DEDFBB1F3FDCF4E581FDA33313E211E0AF819CCB06C7D30E0EAEEFC0C9FB6DF8B2C723546887451221562B94280CF3430AE58C46E188C24835BEE1FB51ABF133BDD1394DBAF7034F8BDD2A2D5222E550C58BE01AEE0B9728369C099495D802B65EEFC3D5CB1C1CC114F0859E50AA44888E09C3D22505B85375FB67E3A8C186D5DCDC3A76E845D79473278F57A7A4CE823E24002A95005C01B116EE0E6FE938B872FF0237B737C1B31E07B19D397CDD2DC0133B42EDEF8368851756B205D8E94939E3E98B628118AB444AA48B44080A10C145E182C9B4A8BF7ABE87777D27C04A6A05579923147A01E47E3C70F8EED0FACB11121E884F4A760C3F7EFCB0687474F4EFA617657D2F3AC6D6DEAE98BA74F9A2419DDE0F7C920895BF27FE71A010854581884FF180C27F22B8DCF170F31C07279F7761CF7E078E3E6F83EDFE2E826CC721DCFC5D443BBC8FB02966E04EFA1BF86ED6B0719F808974BDA56032DC035DE01DE101FD3C1DE6AFCB404E510666A6C5402C639B80CD9C158F43470E56F5F4BC940E0E0EBEC31A181D35BDCDEB7C76FF2FDBB6179746C545421D200647688E4D1B62F0F1F608147F18809C3C6FA4CFE72075011779454AC4A53B626A9223F4A193A0E4BD09A5EF9B08D75AC35F630D1EB1E2CC1F0F731F16EC446F2020C90DE96B8290BC2E1859DBA661EEDA6998363704D1B3822190F9222C468F8CECF497576F56CC7F353C38CE084C620D1987C7FEFE8AF156E54DF3BCC5F9150A3FC9284F60837D251958B55C86B52BC5D8B14B8FAD259158B3DD1F2B368BB0635F30566D5220659E13E6E678217EB6037D8FC0865D490898E58389B4CF89CEF646EE460D5296729053AC41E26A0922F27908491780A3B5C7D4E430483442636854F0A3AFBEFDB2606064D0AE6FA8FF4D02F65796C1F8EACFBF0363464D43A3C5C73BB75F0E089460D58A19D8B2210EBB774EC3825C5F04458D435C8A391252DFC79215EE58B59E87F5DB1498BFCC13A9394E58BB4D8FDC2235A4D1E678CF8B85C55B75F8F4FB24E46FE6D3FE978FD96B7948DDA0C5C2CDD391B73A05310921C6E48CC4FBE5151793FA867ADF1B85E1ED51E02D038C6308CCF05F460DFDFF658D19CF3B3B2C7E3AF1ED99654BD3863333839091A942D21C1E16AF09C6DC3C01F20AD99833D70EB3D2ACA18F791B6133C72136CD02C1F1EF2128CE1C7CFD04C8A22D312DDB151F1D8847FA624F146CD120A94886E81C093296C4226B71B2E170E93F6BEE3D6989EB1FEE235023EF0E1B0D7F1E351AFE647A6B0D83E12D60E4CF468CFC019C11C36F3D7ED6EA77FECA4F07F71EDC72B5706D4A75CA82E0C68434D96046A60299D90AAC583B150B970762C58751D8B0670612B2482C0B35884C9123209E07659823E2D384983A5B80A9698AA139CBE21BB77DBEB1EA978A9F0F353D6AF61EC6C03B060C8EA5B9FF64C0B0E98530CDCDE4FC581646310E30505C0DEFD1184F63ACD1F8BA20863134660083937BF042FAB0AF555573FFAAB2EC4AE9E9AF4A8B478A3FCC445A961E41B15E10F88D07DF7F1C3CE46FC15B39015E723B4CCF0843D69219D87B681D4EFE7268E4466DD9EEE68EFABF3D1FE97AAB1FA37F32BDF8FD3F3EAF89C1D8FF007CD9F3265FA5C02E0000000049454E44AE426082', 'Lights - Alarm ON', 'png', 38, 42, 96),
(120, x'FFD8FFE000104A46494600010201006000600000FFE104504578696600004D4D002A000000080007011200030000000100010000011A00050000000100000062011B0005000000010000006A012800030000000100020000013100020000001C0000007201320002000000140000008E8769000400000001000000A4000000D0000EA60000002710000EA6000000271041646F62652050686F746F73686F70204353322057696E646F777300323030363A30313A32342030303A30353A30320000000003A001000300000001FFFF0000A00200040000000100000011A003000400000001000000190000000000000006010300030000000100060000011A0005000000010000011E011B0005000000010000012601280003000000010002000002010004000000010000012E02020004000000010000031A0000000000000048000000010000004800000001FFD8FFE000104A46494600010200004800480000FFED000C41646F62655F434D0002FFEE000E41646F626500648000000001FFDB0084000C08080809080C09090C110B0A0B11150F0C0C0F1518131315131318110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C010D0B0B0D0E0D100E0E10140E0E0E14140E0E0E0E14110C0C0C0C0C11110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0CFFC00011080019001103012200021101031101FFDD00040002FFC4013F0000010501010101010100000000000000030001020405060708090A0B0100010501010101010100000000000000010002030405060708090A0B1000010401030204020507060805030C33010002110304211231054151611322718132061491A1B14223241552C16233347282D14307259253F0E1F163733516A2B283264493546445C2A3743617D255E265F2B384C3D375E3F3462794A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F637475767778797A7B7C7D7E7F711000202010204040304050607070605350100021103213112044151617122130532819114A1B14223C152D1F0332462E1728292435315637334F1250616A2B283072635C2D2449354A317644555367465E2F2B384C3D375E3F34694A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F62737475767778797A7B7C7FFDA000C03010002110311003F00EBFAB754AAAEBF761E675EFD8B8F5E263DB4B376233D47D9666D77BB7751C7C87BF6331F1FF9AFA0858FD5B1FF006BF4BC7C1FAC9FB5FED391657918BBF0ACFD1B71B2F23D4FD431A8C866CC8A28F7FA9B10FACD9D459F5AB27EC54537CE0627A9EB5CEA63F4DD4B6ECF4F1F2F7FE77FA3595F566CEA2EFF009A6DBE8A6BC61B7D2B19739F63BFC9F99B3D4A1D8F4B2BDCCFA5FAC5BFDB494FA224924929FFD0E8BEB1E2D4EFAC96DD934E79A9D858CCAACC26E6169736DCE75ACB1FD286DDEC6DB4FF003DFE93D8AAF49C0C4ABAD7471D3F1BA8B2BC6B9FB864B33C515D43132E9647ED11F65ABF48FA6AAF6FBFF3177A924A524924929FFFD9FFED08FC50686F746F73686F7020332E30003842494D0425000000000010000000000000000000000000000000003842494D03ED000000000010006000000001000100600000000100013842494D042600000000000E000000000000000000003F8000003842494D040D0000000000040000001E3842494D04190000000000040000001E3842494D03F3000000000009000000000000000001003842494D040A00000000000100003842494D271000000000000A000100000000000000023842494D03F5000000000048002F66660001006C66660006000000000001002F6666000100A1999A0006000000000001003200000001005A00000006000000000001003500000001002D000000060000000000013842494D03F80000000000700000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800003842494D0408000000000010000000010000024000000240000000003842494D041E000000000004000000003842494D041A00000000034900000006000000000000000000000019000000110000000A004B00650079007000610064004C0069006E00630000000100000000000000000000000000000000000000010000000000000000000000110000001900000000000000000000000000000000010000000000000000000000000000000000000010000000010000000000006E756C6C0000000200000006626F756E64734F626A6300000001000000000000526374310000000400000000546F70206C6F6E6700000000000000004C6566746C6F6E67000000000000000042746F6D6C6F6E670000001900000000526768746C6F6E670000001100000006736C69636573566C4C73000000014F626A6300000001000000000005736C6963650000001200000007736C69636549446C6F6E67000000000000000767726F757049446C6F6E6700000000000000066F726967696E656E756D0000000C45536C6963654F726967696E0000000D6175746F47656E6572617465640000000054797065656E756D0000000A45536C6963655479706500000000496D672000000006626F756E64734F626A6300000001000000000000526374310000000400000000546F70206C6F6E6700000000000000004C6566746C6F6E67000000000000000042746F6D6C6F6E670000001900000000526768746C6F6E67000000110000000375726C54455854000000010000000000006E756C6C54455854000000010000000000004D7367655445585400000001000000000006616C74546167544558540000000100000000000E63656C6C54657874497348544D4C626F6F6C010000000863656C6C546578745445585400000001000000000009686F727A416C69676E656E756D0000000F45536C696365486F727A416C69676E0000000764656661756C740000000976657274416C69676E656E756D0000000F45536C69636556657274416C69676E0000000764656661756C740000000B6267436F6C6F7254797065656E756D0000001145536C6963654247436F6C6F7254797065000000004E6F6E6500000009746F704F75747365746C6F6E67000000000000000A6C6566744F75747365746C6F6E67000000000000000C626F74746F6D4F75747365746C6F6E67000000000000000B72696768744F75747365746C6F6E6700000000003842494D042800000000000C000000013FF00000000000003842494D041100000000000101003842494D0414000000000004000000013842494D040C00000000033600000001000000110000001900000034000005140000031A00180001FFD8FFE000104A46494600010200004800480000FFED000C41646F62655F434D0002FFEE000E41646F626500648000000001FFDB0084000C08080809080C09090C110B0A0B11150F0C0C0F1518131315131318110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C010D0B0B0D0E0D100E0E10140E0E0E14140E0E0E0E14110C0C0C0C0C11110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0CFFC00011080019001103012200021101031101FFDD00040002FFC4013F0000010501010101010100000000000000030001020405060708090A0B0100010501010101010100000000000000010002030405060708090A0B1000010401030204020507060805030C33010002110304211231054151611322718132061491A1B14223241552C16233347282D14307259253F0E1F163733516A2B283264493546445C2A3743617D255E265F2B384C3D375E3F3462794A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F637475767778797A7B7C7D7E7F711000202010204040304050607070605350100021103213112044151617122130532819114A1B14223C152D1F0332462E1728292435315637334F1250616A2B283072635C2D2449354A317644555367465E2F2B384C3D375E3F34694A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F62737475767778797A7B7C7FFDA000C03010002110311003F00EBFAB754AAAEBF761E675EFD8B8F5E263DB4B376233D47D9666D77BB7751C7C87BF6331F1FF9AFA0858FD5B1FF006BF4BC7C1FAC9FB5FED391657918BBF0ACFD1B71B2F23D4FD431A8C866CC8A28F7FA9B10FACD9D459F5AB27EC54537CE0627A9EB5CEA63F4DD4B6ECF4F1F2F7FE77FA3595F566CEA2EFF009A6DBE8A6BC61B7D2B19739F63BFC9F99B3D4A1D8F4B2BDCCFA5FAC5BFDB494FA224924929FFD0E8BEB1E2D4EFAC96DD934E79A9D858CCAACC26E6169736DCE75ACB1FD286DDEC6DB4FF003DFE93D8AAF49C0C4ABAD7471D3F1BA8B2BC6B9FB864B33C515D43132E9647ED11F65ABF48FA6AAF6FBFF3177A924A524924929FFFD93842494D042100000000005500000001010000000F00410064006F00620065002000500068006F0074006F00730068006F00700000001300410064006F00620065002000500068006F0074006F00730068006F0070002000430053003200000001003842494D04060000000000070008000000010100FFE13A68687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F003C3F787061636B657420626567696E3D22EFBBBF222069643D2257354D304D7043656869487A7265537A4E54637A6B633964223F3E0A3C783A786D706D65746120786D6C6E733A783D2261646F62653A6E733A6D6574612F2220783A786D70746B3D22332E312E312D313131223E0A2020203C7264663A52444620786D6C6E733A7264663D22687474703A2F2F7777772E77332E6F72672F313939392F30322F32322D7264662D73796E7461782D6E7323223E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A7861704D4D3D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F6D6D2F220A202020202020202020202020786D6C6E733A73745265663D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F73547970652F5265736F7572636552656623223E0A2020202020202020203C7861704D4D3A446F63756D656E7449443E757569643A34303836393435423946384344413131424641373831453036453832384238323C2F7861704D4D3A446F63756D656E7449443E0A2020202020202020203C7861704D4D3A496E7374616E636549443E757569643A34313836393435423946384344413131424641373831453036453832384238323C2F7861704D4D3A496E7374616E636549443E0A2020202020202020203C7861704D4D3A4465726976656446726F6D207264663A7061727365547970653D225265736F75726365223E0A2020202020202020202020203C73745265663A696E7374616E636549443E757569643A33343630443342393938384344413131424641373831453036453832384238323C2F73745265663A696E7374616E636549443E0A2020202020202020202020203C73745265663A646F63756D656E7449443E757569643A33343630443342393938384344413131424641373831453036453832384238323C2F73745265663A646F63756D656E7449443E0A2020202020202020203C2F7861704D4D3A4465726976656446726F6D3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A7861703D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F223E0A2020202020202020203C7861703A437265617465446174653E323030362D30312D32345430303A30353A30322D30363A30303C2F7861703A437265617465446174653E0A2020202020202020203C7861703A4D6F64696679446174653E323030362D30312D32345430303A30353A30322D30363A30303C2F7861703A4D6F64696679446174653E0A2020202020202020203C7861703A4D65746164617461446174653E323030362D30312D32345430303A30353A30322D30363A30303C2F7861703A4D65746164617461446174653E0A2020202020202020203C7861703A43726561746F72546F6F6C3E41646F62652050686F746F73686F70204353322057696E646F77733C2F7861703A43726561746F72546F6F6C3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A64633D22687474703A2F2F7075726C2E6F72672F64632F656C656D656E74732F312E312F223E0A2020202020202020203C64633A666F726D61743E696D6167652F6A7065673C2F64633A666F726D61743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A70686F746F73686F703D22687474703A2F2F6E732E61646F62652E636F6D2F70686F746F73686F702F312E302F223E0A2020202020202020203C70686F746F73686F703A436F6C6F724D6F64653E333C2F70686F746F73686F703A436F6C6F724D6F64653E0A2020202020202020203C70686F746F73686F703A486973746F72792F3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A746966663D22687474703A2F2F6E732E61646F62652E636F6D2F746966662F312E302F223E0A2020202020202020203C746966663A4F7269656E746174696F6E3E313C2F746966663A4F7269656E746174696F6E3E0A2020202020202020203C746966663A585265736F6C7574696F6E3E3936303030302F31303030303C2F746966663A585265736F6C7574696F6E3E0A2020202020202020203C746966663A595265736F6C7574696F6E3E3936303030302F31303030303C2F746966663A595265736F6C7574696F6E3E0A2020202020202020203C746966663A5265736F6C7574696F6E556E69743E323C2F746966663A5265736F6C7574696F6E556E69743E0A2020202020202020203C746966663A4E61746976654469676573743E3235362C3235372C3235382C3235392C3236322C3237342C3237372C3238342C3533302C3533312C3238322C3238332C3239362C3330312C3331382C3331392C3532392C3533322C3330362C3237302C3237312C3237322C3330352C3331352C33333433323B43313833383236343832354531333936463136374242464439383632304234323C2F746966663A4E61746976654469676573743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A657869663D22687474703A2F2F6E732E61646F62652E636F6D2F657869662F312E302F223E0A2020202020202020203C657869663A506978656C5844696D656E73696F6E3E31373C2F657869663A506978656C5844696D656E73696F6E3E0A2020202020202020203C657869663A506978656C5944696D656E73696F6E3E32353C2F657869663A506978656C5944696D656E73696F6E3E0A2020202020202020203C657869663A436F6C6F7253706163653E2D313C2F657869663A436F6C6F7253706163653E0A2020202020202020203C657869663A4E61746976654469676573743E33363836342C34303936302C34303936312C33373132312C33373132322C34303936322C34303936332C33373531302C34303936342C33363836372C33363836382C33333433342C33333433372C33343835302C33343835322C33343835352C33343835362C33373337372C33373337382C33373337392C33373338302C33373338312C33373338322C33373338332C33373338342C33373338352C33373338362C33373339362C34313438332C34313438342C34313438362C34313438372C34313438382C34313439322C34313439332C34313439352C34313732382C34313732392C34313733302C34313938352C34313938362C34313938372C34313938382C34313938392C34313939302C34313939312C34313939322C34313939332C34313939342C34313939352C34313939362C34323031362C302C322C342C352C362C372C382C392C31302C31312C31322C31332C31342C31352C31362C31372C31382C32302C32322C32332C32342C32352C32362C32372C32382C33303B34314543353730393846383537313234304545333944433841454235363836453C2F657869663A4E61746976654469676573743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020203C2F7264663A5244463E0A3C2F783A786D706D6574613E0A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020200A3C3F787061636B657420656E643D2277223F3EFFEE000E41646F626500644000000001FFDB008400010101010101010101010101010101010101010101010101010101010101010101010101010101010101010202020202020202020202030303030303030303030101010101010101010101020201020203030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303FFC00011080019001103011100021101031101FFDD00040003FFC401A20000000602030100000000000000000000070806050409030A0201000B0100000603010101000000000000000000060504030702080109000A0B1000020103040103030203030302060975010203041105120621071322000831144132231509514216612433175271811862912543A1B1F02634720A19C1D13527E1533682F192A24454734546374763285556571AB2C2D2E2F2648374938465A3B3C3D3E3293866F3752A393A48494A58595A6768696A767778797A85868788898A9495969798999AA4A5A6A7A8A9AAB4B5B6B7B8B9BAC4C5C6C7C8C9CAD4D5D6D7D8D9DAE4E5E6E7E8E9EAF4F5F6F7F8F9FA110002010302040403050404040606056D010203110421120531060022134151073261147108428123911552A162163309B124C1D14372F017E18234259253186344F1A2B226351954364564270A7383934674C2D2E2F255657556378485A3B3C3D3E3F3291A94A4B4C4D4E4F495A5B5C5D5E5F52847576638768696A6B6C6D6E6F667778797A7B7C7D7E7F7485868788898A8B8C8D8E8F839495969798999A9B9C9D9E9F92A3A4A5A6A7A8A9AAABACADAEAFAFFDA000C03010002110311003F00D8EBE597C9EDB1B43E7D762F4A774FF363FF0086DFEB2DAFF103E2A7697596DAFEF77F2FBEBFFF004A3BEFB43BA3E6DED3EDACEFF19F99FF001F3B8F706E4FEEDEDFE9CD9B4FF6B83ABA3A3C6FDD6B9E2692B51CFBAF741FEC0F965B03FD9BBF82BD75F1FF00F9DAFF00B3F7FE9A3E407656C0EDEE85FEFF007F2C0EC5FF008C5982F851F2DBB829F7AFDA7C4DF8BFD59DB5B77FBBBDB5D59B513F89265E3C77F957DA54C72FDDA2FBF75EEAFF00BDFBAF75FFD0BCAF9999CF90D85FE69BDD1FE81BABFA63B27EE7E007C14FEF5FFA5DEF8DF1D25FC1BC3F22BF995FF02FEEF7F737E38FC81FEF37F11F2D67DDFDCFF09FB2F043E3FBAF3BFDBD5A9D7BA201FCB4739F21AB9FFE13DF8DEC2EAEE99DB1D5147FDD1FEE3EF5D9BDF1BDF7DF61EE1FB7FE4F5F3229F6D7F7AFAC337F1C7AE76DECDFE2DB6DE7ABAEFB3DDD9EFE1D5D1A5245F7B148D5B16F1534EBDD6E5BEF7D7BAFFFD1B9DFE637D61B6F27FCC937B6FAED2EBCFE60D90D9398F841F12B69EC5DE3F0DF697F32AABDAB94DD5B4BBE7E78E63B0B6D6F0DC3F0171F2622B7706D9C46F7DB7550D16E699AA68A9B2E25A2444AAA969346BE5D7BA01BE267427536D3F9A7FCB6A93E3774BFF326DB5B5BA73B9BB23F8E537C84EB9FE6C786E81EABEA0A2F803F303ADF01F630FCCBC3D37446C7FB4DDFB936DE030FF64297291AD7AD0D15A9E7A889B42BE7D7BADB53DDBAF75FFFD2DFE3DFBAF75EF7EEBDD7BDFBAF75FFD9', 'KeypadLinc ON', 'jpg', 17, 25, 96),
(121, x'FFD8FFE000104A46494600010201006000600000FFE1044B4578696600004D4D002A000000080007011200030000000100010000011A00050000000100000062011B0005000000010000006A012800030000000100020000013100020000001C0000007201320002000000140000008E8769000400000001000000A4000000D0000EA60000002710000EA6000000271041646F62652050686F746F73686F70204353322057696E646F777300323030363A30313A32342030303A30353A33350000000003A001000300000001FFFF0000A00200040000000100000011A003000400000001000000190000000000000006010300030000000100060000011A0005000000010000011E011B0005000000010000012601280003000000010002000002010004000000010000012E0202000400000001000003150000000000000048000000010000004800000001FFD8FFE000104A46494600010200004800480000FFED000C41646F62655F434D0002FFEE000E41646F626500648000000001FFDB0084000C08080809080C09090C110B0A0B11150F0C0C0F1518131315131318110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C010D0B0B0D0E0D100E0E10140E0E0E14140E0E0E0E14110C0C0C0C0C11110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0CFFC00011080019001103012200021101031101FFDD00040002FFC4013F0000010501010101010100000000000000030001020405060708090A0B0100010501010101010100000000000000010002030405060708090A0B1000010401030204020507060805030C33010002110304211231054151611322718132061491A1B14223241552C16233347282D14307259253F0E1F163733516A2B283264493546445C2A3743617D255E265F2B384C3D375E3F3462794A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F637475767778797A7B7C7D7E7F711000202010204040304050607070605350100021103213112044151617122130532819114A1B14223C152D1F0332462E1728292435315637334F1250616A2B283072635C2D2449354A317644555367465E2F2B384C3D375E3F34694A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F62737475767778797A7B7C7FFDA000C03010002110311003F00E9FEB175A7E2F5F7E1DBD6BF6463B7128B6A64E2B77BECB332BB9DBB3E8C87BB6B31E8FE6D03A6F5E75BD6FA66363FD60FDA8DC9BAC65F8D386EFD1B71F2AF167EA58D4DECD97D34FBBD446EB3675167D6AC9FB1514DF38189EA7AD73A98FD3752DBB3D3C7CBDFF9DFE8D657D59B3A8BBFE69B6FA29AF186DF4AC65CE7D8EFF27E66CF528763D2CAF733E97EB16FF6D253E88924924A7FFFD0EAFAC62F4F77D64BEEEA54E79A9D858ACA2CC26E7169736DCF75ECB1FD1C6DDEC6DB47F3FF00E93F46ABE1E0745ABAB7461D1B1BA8B063643F78C96751145748C4CCA5B1FB507D92AFD2BE8AABD9FA4FCC62EC92494A49249253FFD9FFED090450686F746F73686F7020332E30003842494D0425000000000010000000000000000000000000000000003842494D03ED000000000010006000000001000100600000000100013842494D042600000000000E000000000000000000003F8000003842494D040D0000000000040000001E3842494D04190000000000040000001E3842494D03F3000000000009000000000000000001003842494D040A00000000000100003842494D271000000000000A000100000000000000023842494D03F5000000000048002F66660001006C66660006000000000001002F6666000100A1999A0006000000000001003200000001005A00000006000000000001003500000001002D000000060000000000013842494D03F80000000000700000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800003842494D0408000000000010000000010000024000000240000000003842494D041E000000000004000000003842494D041A000000000355000000060000000000000000000000190000001100000010004B00650079007000610064004C0069006E0063002000560032005F004F004E0000000100000000000000000000000000000000000000010000000000000000000000110000001900000000000000000000000000000000010000000000000000000000000000000000000010000000010000000000006E756C6C0000000200000006626F756E64734F626A6300000001000000000000526374310000000400000000546F70206C6F6E6700000000000000004C6566746C6F6E67000000000000000042746F6D6C6F6E670000001900000000526768746C6F6E670000001100000006736C69636573566C4C73000000014F626A6300000001000000000005736C6963650000001200000007736C69636549446C6F6E67000000000000000767726F757049446C6F6E6700000000000000066F726967696E656E756D0000000C45536C6963654F726967696E0000000D6175746F47656E6572617465640000000054797065656E756D0000000A45536C6963655479706500000000496D672000000006626F756E64734F626A6300000001000000000000526374310000000400000000546F70206C6F6E6700000000000000004C6566746C6F6E67000000000000000042746F6D6C6F6E670000001900000000526768746C6F6E67000000110000000375726C54455854000000010000000000006E756C6C54455854000000010000000000004D7367655445585400000001000000000006616C74546167544558540000000100000000000E63656C6C54657874497348544D4C626F6F6C010000000863656C6C546578745445585400000001000000000009686F727A416C69676E656E756D0000000F45536C696365486F727A416C69676E0000000764656661756C740000000976657274416C69676E656E756D0000000F45536C69636556657274416C69676E0000000764656661756C740000000B6267436F6C6F7254797065656E756D0000001145536C6963654247436F6C6F7254797065000000004E6F6E6500000009746F704F75747365746C6F6E67000000000000000A6C6566744F75747365746C6F6E67000000000000000C626F74746F6D4F75747365746C6F6E67000000000000000B72696768744F75747365746C6F6E6700000000003842494D042800000000000C000000013FF00000000000003842494D041100000000000101003842494D0414000000000004000000013842494D040C00000000033100000001000000110000001900000034000005140000031500180001FFD8FFE000104A46494600010200004800480000FFED000C41646F62655F434D0002FFEE000E41646F626500648000000001FFDB0084000C08080809080C09090C110B0A0B11150F0C0C0F1518131315131318110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C010D0B0B0D0E0D100E0E10140E0E0E14140E0E0E0E14110C0C0C0C0C11110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0CFFC00011080019001103012200021101031101FFDD00040002FFC4013F0000010501010101010100000000000000030001020405060708090A0B0100010501010101010100000000000000010002030405060708090A0B1000010401030204020507060805030C33010002110304211231054151611322718132061491A1B14223241552C16233347282D14307259253F0E1F163733516A2B283264493546445C2A3743617D255E265F2B384C3D375E3F3462794A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F637475767778797A7B7C7D7E7F711000202010204040304050607070605350100021103213112044151617122130532819114A1B14223C152D1F0332462E1728292435315637334F1250616A2B283072635C2D2449354A317644555367465E2F2B384C3D375E3F34694A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F62737475767778797A7B7C7FFDA000C03010002110311003F00E9FEB175A7E2F5F7E1DBD6BF6463B7128B6A64E2B77BECB332BB9DBB3E8C87BB6B31E8FE6D03A6F5E75BD6FA66363FD60FDA8DC9BAC65F8D386EFD1B71F2AF167EA58D4DECD97D34FBBD446EB3675167D6AC9FB1514DF38189EA7AD73A98FD3752DBB3D3C7CBDFF9DFE8D657D59B3A8BBFE69B6FA29AF186DF4AC65CE7D8EFF27E66CF528763D2CAF733E97EB16FF6D253E88924924A7FFFD0EAFAC62F4F77D64BEEEA54E79A9D858ACA2CC26E7169736DCF75ECB1FD1C6DDEC6DB47F3FF00E93F46ABE1E0745ABAB7461D1B1BA8B063643F78C96751145748C4CCA5B1FB507D92AFD2BE8AABD9FA4FCC62EC92494A49249253FFD9003842494D042100000000005500000001010000000F00410064006F00620065002000500068006F0074006F00730068006F00700000001300410064006F00620065002000500068006F0074006F00730068006F0070002000430053003200000001003842494D04060000000000070008000000010100FFE13A68687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F003C3F787061636B657420626567696E3D22EFBBBF222069643D2257354D304D7043656869487A7265537A4E54637A6B633964223F3E0A3C783A786D706D65746120786D6C6E733A783D2261646F62653A6E733A6D6574612F2220783A786D70746B3D22332E312E312D313131223E0A2020203C7264663A52444620786D6C6E733A7264663D22687474703A2F2F7777772E77332E6F72672F313939392F30322F32322D7264662D73796E7461782D6E7323223E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A7861704D4D3D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F6D6D2F220A202020202020202020202020786D6C6E733A73745265663D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F73547970652F5265736F7572636552656623223E0A2020202020202020203C7861704D4D3A446F63756D656E7449443E757569643A34333836393435423946384344413131424641373831453036453832384238323C2F7861704D4D3A446F63756D656E7449443E0A2020202020202020203C7861704D4D3A496E7374616E636549443E757569643A34343836393435423946384344413131424641373831453036453832384238323C2F7861704D4D3A496E7374616E636549443E0A2020202020202020203C7861704D4D3A4465726976656446726F6D207264663A7061727365547970653D225265736F75726365223E0A2020202020202020202020203C73745265663A696E7374616E636549443E757569643A34313836393435423946384344413131424641373831453036453832384238323C2F73745265663A696E7374616E636549443E0A2020202020202020202020203C73745265663A646F63756D656E7449443E757569643A34303836393435423946384344413131424641373831453036453832384238323C2F73745265663A646F63756D656E7449443E0A2020202020202020203C2F7861704D4D3A4465726976656446726F6D3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A7861703D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F223E0A2020202020202020203C7861703A437265617465446174653E323030362D30312D32345430303A30353A33352D30363A30303C2F7861703A437265617465446174653E0A2020202020202020203C7861703A4D6F64696679446174653E323030362D30312D32345430303A30353A33352D30363A30303C2F7861703A4D6F64696679446174653E0A2020202020202020203C7861703A4D65746164617461446174653E323030362D30312D32345430303A30353A33352D30363A30303C2F7861703A4D65746164617461446174653E0A2020202020202020203C7861703A43726561746F72546F6F6C3E41646F62652050686F746F73686F70204353322057696E646F77733C2F7861703A43726561746F72546F6F6C3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A64633D22687474703A2F2F7075726C2E6F72672F64632F656C656D656E74732F312E312F223E0A2020202020202020203C64633A666F726D61743E696D6167652F6A7065673C2F64633A666F726D61743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A70686F746F73686F703D22687474703A2F2F6E732E61646F62652E636F6D2F70686F746F73686F702F312E302F223E0A2020202020202020203C70686F746F73686F703A436F6C6F724D6F64653E333C2F70686F746F73686F703A436F6C6F724D6F64653E0A2020202020202020203C70686F746F73686F703A486973746F72792F3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A746966663D22687474703A2F2F6E732E61646F62652E636F6D2F746966662F312E302F223E0A2020202020202020203C746966663A4F7269656E746174696F6E3E313C2F746966663A4F7269656E746174696F6E3E0A2020202020202020203C746966663A585265736F6C7574696F6E3E3936303030302F31303030303C2F746966663A585265736F6C7574696F6E3E0A2020202020202020203C746966663A595265736F6C7574696F6E3E3936303030302F31303030303C2F746966663A595265736F6C7574696F6E3E0A2020202020202020203C746966663A5265736F6C7574696F6E556E69743E323C2F746966663A5265736F6C7574696F6E556E69743E0A2020202020202020203C746966663A4E61746976654469676573743E3235362C3235372C3235382C3235392C3236322C3237342C3237372C3238342C3533302C3533312C3238322C3238332C3239362C3330312C3331382C3331392C3532392C3533322C3330362C3237302C3237312C3237322C3330352C3331352C33333433323B46454545394639454232344434323933433032383531413232454333334230333C2F746966663A4E61746976654469676573743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A657869663D22687474703A2F2F6E732E61646F62652E636F6D2F657869662F312E302F223E0A2020202020202020203C657869663A506978656C5844696D656E73696F6E3E31373C2F657869663A506978656C5844696D656E73696F6E3E0A2020202020202020203C657869663A506978656C5944696D656E73696F6E3E32353C2F657869663A506978656C5944696D656E73696F6E3E0A2020202020202020203C657869663A436F6C6F7253706163653E2D313C2F657869663A436F6C6F7253706163653E0A2020202020202020203C657869663A4E61746976654469676573743E33363836342C34303936302C34303936312C33373132312C33373132322C34303936322C34303936332C33373531302C34303936342C33363836372C33363836382C33333433342C33333433372C33343835302C33343835322C33343835352C33343835362C33373337372C33373337382C33373337392C33373338302C33373338312C33373338322C33373338332C33373338342C33373338352C33373338362C33373339362C34313438332C34313438342C34313438362C34313438372C34313438382C34313439322C34313439332C34313439352C34313732382C34313732392C34313733302C34313938352C34313938362C34313938372C34313938382C34313938392C34313939302C34313939312C34313939322C34313939332C34313939342C34313939352C34313939362C34323031362C302C322C342C352C362C372C382C392C31302C31312C31322C31332C31342C31352C31362C31372C31382C32302C32322C32332C32342C32352C32362C32372C32382C33303B34314543353730393846383537313234304545333944433841454235363836453C2F657869663A4E61746976654469676573743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020203C2F7264663A5244463E0A3C2F783A786D706D6574613E0A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020200A3C3F787061636B657420656E643D2277223F3EFFEE000E41646F626500644000000001FFDB008400010101010101010101010101010101010101010101010101010101010101010101010101010101010101010202020202020202020202030303030303030303030101010101010101010101020201020203030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303FFC00011080019001103011100021101031101FFDD00040003FFC401A20000000602030100000000000000000000070806050409030A0201000B0100000603010101000000000000000000060504030702080109000A0B1000020103040103030203030302060975010203041105120621071322000831144132231509514216612433175271811862912543A1B1F02634720A19C1D13527E1533682F192A24454734546374763285556571AB2C2D2E2F2648374938465A3B3C3D3E3293866F3752A393A48494A58595A6768696A767778797A85868788898A9495969798999AA4A5A6A7A8A9AAB4B5B6B7B8B9BAC4C5C6C7C8C9CAD4D5D6D7D8D9DAE4E5E6E7E8E9EAF4F5F6F7F8F9FA110002010302040403050404040606056D010203110421120531060022134151073261147108428123911552A162163309B124C1D14372F017E18234259253186344F1A2B226351954364564270A7383934674C2D2E2F255657556378485A3B3C3D3E3F3291A94A4B4C4D4E4F495A5B5C5D5E5F52847576638768696A6B6C6D6E6F667778797A7B7C7D7E7F7485868788898A8B8C8D8E8F839495969798999A9B9C9D9E9F92A3A4A5A6A7A8A9AAABACADAEAFAFFDA000C03010002110311003F00BD6FE61BF34337D49F3EB72F4A6EDFE675FEC82F5961FE207C6AED2D8FB6BF897C16DB1FE9137DF62F747CC9DA7D999DFE33F2DBA1BB6F7066FF00826DFEA4DA74FF006B87ABA5A3A0F36B9623255873A35F2EBDD04DF1B3E77E4F74FCDCF843D55D6DFCE07FD9D8C57717737696CDED6E92FE37FCB8B77797AF36F7C35F94DDB543BAF4FC64F8D7D69DA380FE01DA3D69B687DF47968A84F9FED6A124154AA7C09F31D7BAD9EFDEFAF75FFFD0BCAF9999CF90D85FE69BDD1FE81BABFA63B27EE7E007C14FEF5FFA5DEF8DF1D25FC1BC3F22BF995FF02FEEF7F737E38FC81FEF37F11F2D67DDFDCFF09FB2F043E3FBAF3BFDBD5A9D7BA201FCB4739F21AB9FFE13DF8DEC2EAEE99DB1D5147FDD1FEE3EF5D9BDF1BDF7DF61EE1FB7FE4F5F3229F6D7F7AFAC337F1C7AE76DECDFE2DB6DE7ABAEFB3DDD9EFE1D5D1A5245F7B148D5B16F1534EBDD6E5BEF7D7BAFFFD1D807E607587C7EC9FF00324ED6DF5F29BAF3F98364364E63E107C34DA7D51BC7E1BED2FE6A357B5729BAB6977CFF00303CC76AEDADE1B87F97163E4C456EE0DB388DEFB4EAA1A2DD93354D15365C4B8F444AAAB693DD7BA07FA83A13E196D3F967FCB3A93E10F4BFF326DB54BD39F203B03FBCF4DF213AE7F9C9E1BA07AAFA268BF97A7CD1EB7C3FD8C3F3AF0F4DF1EF63FDA6FBDC9B530187FE1E297311AD7AD0D0DA9A7AA89BDD7BAD95FDFBAF75FFD2DFE3DFBAF75EF7EEBDD7BDFBAF75FFD9', 'KeypadLinc OFF', 'jpg', 17, 25, 96),
(122, x'FFD8FFE000104A46494600010201006000600000FFE103C34578696600004D4D002A000000080007011200030000000100010000011A00050000000100000062011B0005000000010000006A012800030000000100020000013100020000001C0000007201320002000000140000008E8769000400000001000000A4000000D0000EA60000002710000EA6000000271041646F62652050686F746F73686F70204353322057696E646F777300323030363A30323A32352032313A31383A30330000000003A001000300000001FFFF0000A00200040000000100000010A003000400000001000000110000000000000006010300030000000100060000011A0005000000010000011E011B0005000000010000012601280003000000010002000002010004000000010000012E02020004000000010000028D0000000000000048000000010000004800000001FFD8FFE000104A46494600010200004800480000FFED000C41646F62655F434D0002FFEE000E41646F626500648000000001FFDB0084000C08080809080C09090C110B0A0B11150F0C0C0F1518131315131318110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C010D0B0B0D0E0D100E0E10140E0E0E14140E0E0E0E14110C0C0C0C0C11110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0CFFC00011080011001003012200021101031101FFDD00040001FFC4013F0000010501010101010100000000000000030001020405060708090A0B0100010501010101010100000000000000010002030405060708090A0B1000010401030204020507060805030C33010002110304211231054151611322718132061491A1B14223241552C16233347282D14307259253F0E1F163733516A2B283264493546445C2A3743617D255E265F2B384C3D375E3F3462794A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F637475767778797A7B7C7D7E7F711000202010204040304050607070605350100021103213112044151617122130532819114A1B14223C152D1F0332462E1728292435315637334F1250616A2B283072635C2D2449354A317644555367465E2F2B384C3D375E3F34694A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F62737475767778797A7B7C7FFDA000C03010002110311003F00ECBA71CCB2B61179F4CCC074BA20A251D16DC47E65CDC9F5BEDD60B1D5DEDDCC60036EDAC4FF00AD55D15FF8243E99914D54319612082674F13A2D2B73B14D701FAA6D0347A85D1C93889C41F4E4004BC444F18FF9CFFFD0D9AFFA355FF16CFC81171BE97CD786249A3753FFD9FFED089650686F746F73686F7020332E30003842494D04040000000000071C020000020002003842494D0425000000000010460CF28926B856DAB09C01A1B0A790773842494D03ED000000000010006000000001000100600000000100013842494D042600000000000E000000000000000000003F8000003842494D040D0000000000040000001E3842494D04190000000000040000001E3842494D03F3000000000009000000000000000001003842494D040A00000000000100003842494D271000000000000A000100000000000000023842494D03F5000000000048002F66660001006C66660006000000000001002F6666000100A1999A0006000000000001003200000001005A00000006000000000001003500000001002D000000060000000000013842494D03F80000000000700000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800003842494D0408000000000010000000010000024000000240000000003842494D041E000000000004000000003842494D041A00000000035B000000060000000000000000000000110000001000000013006D006F00740069006F006E0020006400650074006500630074006F00720020004F004600460000000100000000000000000000000000000000000000010000000000000000000000100000001100000000000000000000000000000000010000000000000000000000000000000000000010000000010000000000006E756C6C0000000200000006626F756E64734F626A6300000001000000000000526374310000000400000000546F70206C6F6E6700000000000000004C6566746C6F6E67000000000000000042746F6D6C6F6E670000001100000000526768746C6F6E670000001000000006736C69636573566C4C73000000014F626A6300000001000000000005736C6963650000001200000007736C69636549446C6F6E67000000000000000767726F757049446C6F6E6700000000000000066F726967696E656E756D0000000C45536C6963654F726967696E0000000D6175746F47656E6572617465640000000054797065656E756D0000000A45536C6963655479706500000000496D672000000006626F756E64734F626A6300000001000000000000526374310000000400000000546F70206C6F6E6700000000000000004C6566746C6F6E67000000000000000042746F6D6C6F6E670000001100000000526768746C6F6E67000000100000000375726C54455854000000010000000000006E756C6C54455854000000010000000000004D7367655445585400000001000000000006616C74546167544558540000000100000000000E63656C6C54657874497348544D4C626F6F6C010000000863656C6C546578745445585400000001000000000009686F727A416C69676E656E756D0000000F45536C696365486F727A416C69676E0000000764656661756C740000000976657274416C69676E656E756D0000000F45536C69636556657274416C69676E0000000764656661756C740000000B6267436F6C6F7254797065656E756D0000001145536C6963654247436F6C6F7254797065000000004E6F6E6500000009746F704F75747365746C6F6E67000000000000000A6C6566744F75747365746C6F6E67000000000000000C626F74746F6D4F75747365746C6F6E67000000000000000B72696768744F75747365746C6F6E6700000000003842494D042800000000000C000000013FF00000000000003842494D041100000000000101003842494D0414000000000004000000023842494D040C0000000002A900000001000000100000001100000030000003300000028D00180001FFD8FFE000104A46494600010200004800480000FFED000C41646F62655F434D0002FFEE000E41646F626500648000000001FFDB0084000C08080809080C09090C110B0A0B11150F0C0C0F1518131315131318110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C010D0B0B0D0E0D100E0E10140E0E0E14140E0E0E0E14110C0C0C0C0C11110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0CFFC00011080011001003012200021101031101FFDD00040001FFC4013F0000010501010101010100000000000000030001020405060708090A0B0100010501010101010100000000000000010002030405060708090A0B1000010401030204020507060805030C33010002110304211231054151611322718132061491A1B14223241552C16233347282D14307259253F0E1F163733516A2B283264493546445C2A3743617D255E265F2B384C3D375E3F3462794A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F637475767778797A7B7C7D7E7F711000202010204040304050607070605350100021103213112044151617122130532819114A1B14223C152D1F0332462E1728292435315637334F1250616A2B283072635C2D2449354A317644555367465E2F2B384C3D375E3F34694A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F62737475767778797A7B7C7FFDA000C03010002110311003F00ECBA71CCB2B61179F4CCC074BA20A251D16DC47E65CDC9F5BEDD60B1D5DEDDCC60036EDAC4FF00AD55D15FF8243E99914D54319612082674F13A2D2B73B14D701FAA6D0347A85D1C93889C41F4E4004BC444F18FF9CFFFD0D9AFFA355FF16CFC81171BE97CD786249A3753FFD9003842494D042100000000005500000001010000000F00410064006F00620065002000500068006F0074006F00730068006F00700000001300410064006F00620065002000500068006F0074006F00730068006F0070002000430053003200000001003842494D04060000000000070008000000010100FFE13A68687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F003C3F787061636B657420626567696E3D22EFBBBF222069643D2257354D304D7043656869487A7265537A4E54637A6B633964223F3E0A3C783A786D706D65746120786D6C6E733A783D2261646F62653A6E733A6D6574612F2220783A786D70746B3D22332E312E312D313131223E0A2020203C7264663A52444620786D6C6E733A7264663D22687474703A2F2F7777772E77332E6F72672F313939392F30322F32322D7264662D73796E7461782D6E7323223E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A7861704D4D3D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F6D6D2F220A202020202020202020202020786D6C6E733A73745265663D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F73547970652F5265736F7572636552656623223E0A2020202020202020203C7861704D4D3A446F63756D656E7449443E757569643A30384545383231303736413644413131413332443930464537463931433643363C2F7861704D4D3A446F63756D656E7449443E0A2020202020202020203C7861704D4D3A496E7374616E636549443E757569643A39384634414537463736413644413131413332443930464537463931433643363C2F7861704D4D3A496E7374616E636549443E0A2020202020202020203C7861704D4D3A4465726976656446726F6D207264663A7061727365547970653D225265736F75726365223E0A2020202020202020202020203C73745265663A696E7374616E636549443E757569643A30364545383231303736413644413131413332443930464537463931433643363C2F73745265663A696E7374616E636549443E0A2020202020202020202020203C73745265663A646F63756D656E7449443E757569643A32443636393131333232393144413131413442423944413339423239384439363C2F73745265663A646F63756D656E7449443E0A2020202020202020203C2F7861704D4D3A4465726976656446726F6D3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A7861703D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F223E0A2020202020202020203C7861703A437265617465446174653E323030362D30322D32355432313A31383A30332D30363A30303C2F7861703A437265617465446174653E0A2020202020202020203C7861703A4D6F64696679446174653E323030362D30322D32355432313A31383A30332D30363A30303C2F7861703A4D6F64696679446174653E0A2020202020202020203C7861703A4D65746164617461446174653E323030362D30322D32355432313A31383A30332D30363A30303C2F7861703A4D65746164617461446174653E0A2020202020202020203C7861703A43726561746F72546F6F6C3E41646F62652050686F746F73686F70204353322057696E646F77733C2F7861703A43726561746F72546F6F6C3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A64633D22687474703A2F2F7075726C2E6F72672F64632F656C656D656E74732F312E312F223E0A2020202020202020203C64633A666F726D61743E696D6167652F6A7065673C2F64633A666F726D61743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A70686F746F73686F703D22687474703A2F2F6E732E61646F62652E636F6D2F70686F746F73686F702F312E302F223E0A2020202020202020203C70686F746F73686F703A436F6C6F724D6F64653E333C2F70686F746F73686F703A436F6C6F724D6F64653E0A2020202020202020203C70686F746F73686F703A486973746F72792F3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A746966663D22687474703A2F2F6E732E61646F62652E636F6D2F746966662F312E302F223E0A2020202020202020203C746966663A4F7269656E746174696F6E3E313C2F746966663A4F7269656E746174696F6E3E0A2020202020202020203C746966663A585265736F6C7574696F6E3E3936303030302F31303030303C2F746966663A585265736F6C7574696F6E3E0A2020202020202020203C746966663A595265736F6C7574696F6E3E3936303030302F31303030303C2F746966663A595265736F6C7574696F6E3E0A2020202020202020203C746966663A5265736F6C7574696F6E556E69743E323C2F746966663A5265736F6C7574696F6E556E69743E0A2020202020202020203C746966663A4E61746976654469676573743E3235362C3235372C3235382C3235392C3236322C3237342C3237372C3238342C3533302C3533312C3238322C3238332C3239362C3330312C3331382C3331392C3532392C3533322C3330362C3237302C3237312C3237322C3330352C3331352C33333433323B31353345394632384541413032324231374544363242453131434445393337343C2F746966663A4E61746976654469676573743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A657869663D22687474703A2F2F6E732E61646F62652E636F6D2F657869662F312E302F223E0A2020202020202020203C657869663A506978656C5844696D656E73696F6E3E31363C2F657869663A506978656C5844696D656E73696F6E3E0A2020202020202020203C657869663A506978656C5944696D656E73696F6E3E31373C2F657869663A506978656C5944696D656E73696F6E3E0A2020202020202020203C657869663A436F6C6F7253706163653E2D313C2F657869663A436F6C6F7253706163653E0A2020202020202020203C657869663A4E61746976654469676573743E33363836342C34303936302C34303936312C33373132312C33373132322C34303936322C34303936332C33373531302C34303936342C33363836372C33363836382C33333433342C33333433372C33343835302C33343835322C33343835352C33343835362C33373337372C33373337382C33373337392C33373338302C33373338312C33373338322C33373338332C33373338342C33373338352C33373338362C33373339362C34313438332C34313438342C34313438362C34313438372C34313438382C34313439322C34313439332C34313439352C34313732382C34313732392C34313733302C34313938352C34313938362C34313938372C34313938382C34313938392C34313939302C34313939312C34313939322C34313939332C34313939342C34313939352C34313939362C34323031362C302C322C342C352C362C372C382C392C31302C31312C31322C31332C31342C31352C31362C31372C31382C32302C32322C32332C32342C32352C32362C32372C32382C33303B45453039343231464241364638314233374442433041433132384530433741323C2F657869663A4E61746976654469676573743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020203C2F7264663A5244463E0A3C2F783A786D706D6574613E0A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020200A3C3F787061636B657420656E643D2277223F3EFFEE000E41646F626500644000000001FFDB008400010101010101010101010101010101010101010101010101010101010101010101010101010101010101010202020202020202020202030303030303030303030101010101010101010101020201020203030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303FFC00011080011001003011100021101031101FFDD00040002FFC401A20000000602030100000000000000000000070806050409030A0201000B0100000603010101000000000000000000060504030702080109000A0B1000020103040103030203030302060975010203041105120621071322000831144132231509514216612433175271811862912543A1B1F02634720A19C1D13527E1533682F192A24454734546374763285556571AB2C2D2E2F2648374938465A3B3C3D3E3293866F3752A393A48494A58595A6768696A767778797A85868788898A9495969798999AA4A5A6A7A8A9AAB4B5B6B7B8B9BAC4C5C6C7C8C9CAD4D5D6D7D8D9DAE4E5E6E7E8E9EAF4F5F6F7F8F9FA110002010302040403050404040606056D010203110421120531060022134151073261147108428123911552A162163309B124C1D14372F017E18234259253186344F1A2B226351954364564270A7383934674C2D2E2F255657556378485A3B3C3D3E3F3291A94A4B4C4D4E4F495A5B5C5D5E5F52847576638768696A6B6C6D6E6F667778797A7B7C7D7E7F7485868788898A8B8C8D8E8F839495969798999A9B9C9D9E9F92A3A4A5A6A7A8A9AAABACADAEAFAFFDA000C03010002110311003F00D94BE3D4FDBFB836F6DE9E0ED8AE8F6E563E663A4A3CEC793CECB8C3495881E96962FBA868E4A2433C6B4DE4BFDB2AE901978F652143AD3D7A4D2B08E452CC074B9D97F0C775F4E66FE47EF9C6F794FD8ABF263B2B15BEF2BB43B8F6C4FBB7666C8A4A3C4B62A6C16D1C7AE4835186A491696927414E69F0789C1E34C65712B535259B3EC1FB9AF77ABC6BF9E5FAD9849A1CD563A2D34AFF0082B8FD358A3A7E9EA6C83F77FDF65F76F91BD88E4D5F6EF61D9BFA93B0C9B6FD558C1E15C6E5AEE0CBE35DB7E5E3B4757D5B95DEEDB8788BFBC7E9EDFF00FFD0D94BE346FEDA3B57646D8C1EE19B23415F8FABCE3D4ABE3DE5A778B275B435343353CE1C878E5A7405C9D3676E350F64D6B388F434A054570715E98BA805C3060DA4FCBFD43A3BDBA3BCBAC67C1C74B16E02B50B1F96188434CCD512C3112B12ABCEADA9DB8B0B9F666D702E72129A4538F1FF00075E860F094A97D4DEBFE4E3D7FFD1B2EC0FFCCBBD8DFF008616CAFF00DD4E37D85EF388FB4F5BE97BD6BFF037FEAA64FF00A14FB38B2FECCFD9D6BAFFD9', 'motion detector ON', 'jpg', 16, 17, 96),
(123, x'FFD8FFE000104A46494600010201006000600000FFE103A44578696600004D4D002A000000080007011200030000000100010000011A00050000000100000062011B0005000000010000006A012800030000000100020000013100020000001C0000007201320002000000140000008E8769000400000001000000A4000000D0000EA60000002710000EA6000000271041646F62652050686F746F73686F70204353322057696E646F777300323030363A30323A32352032313A31373A30390000000003A001000300000001FFFF0000A00200040000000100000010A003000400000001000000110000000000000006010300030000000100060000011A0005000000010000011E011B0005000000010000012601280003000000010002000002010004000000010000012E02020004000000010000026E0000000000000048000000010000004800000001FFD8FFE000104A46494600010200004800480000FFED000C41646F62655F434D0002FFEE000E41646F626500648000000001FFDB0084000C08080809080C09090C110B0A0B11150F0C0C0F1518131315131318110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C010D0B0B0D0E0D100E0E10140E0E0E14140E0E0E0E14110C0C0C0C0C11110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0CFFC00011080011001003012200021101031101FFDD00040001FFC4013F0000010501010101010100000000000000030001020405060708090A0B0100010501010101010100000000000000010002030405060708090A0B1000010401030204020507060805030C33010002110304211231054151611322718132061491A1B14223241552C16233347282D14307259253F0E1F163733516A2B283264493546445C2A3743617D255E265F2B384C3D375E3F3462794A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F637475767778797A7B7C7D7E7F711000202010204040304050607070605350100021103213112044151617122130532819114A1B14223C152D1F0332462E1728292435315637334F1250616A2B283072635C2D2449354A317644555367465E2F2B384C3D375E3F34694A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F62737475767778797A7B7C7FFDA000C03010002110311003F00ECBA71CCB2B61179F4CCC074BA20ABF762DFE9BA2C6383CEADB19200E36B6150E99914D54319612082674F13A2D2B73B14D701FAA0294FFFD0D9AFFA355FF16CFC81171BE97CD786249A3753FFD9FFED087650686F746F73686F7020332E30003842494D04040000000000071C020000020002003842494D0425000000000010460CF28926B856DAB09C01A1B0A790773842494D03ED000000000010006000000001000100600000000100013842494D042600000000000E000000000000000000003F8000003842494D040D0000000000040000001E3842494D04190000000000040000001E3842494D03F3000000000009000000000000000001003842494D040A00000000000100003842494D271000000000000A000100000000000000023842494D03F5000000000048002F66660001006C66660006000000000001002F6666000100A1999A0006000000000001003200000001005A00000006000000000001003500000001002D000000060000000000013842494D03F80000000000700000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF03E800003842494D0408000000000010000000010000024000000240000000003842494D041E000000000004000000003842494D041A00000000035B000000060000000000000000000000110000001000000013006D006F00740069006F006E0020006400650074006500630074006F00720020004F004600460000000100000000000000000000000000000000000000010000000000000000000000100000001100000000000000000000000000000000010000000000000000000000000000000000000010000000010000000000006E756C6C0000000200000006626F756E64734F626A6300000001000000000000526374310000000400000000546F70206C6F6E6700000000000000004C6566746C6F6E67000000000000000042746F6D6C6F6E670000001100000000526768746C6F6E670000001000000006736C69636573566C4C73000000014F626A6300000001000000000005736C6963650000001200000007736C69636549446C6F6E67000000000000000767726F757049446C6F6E6700000000000000066F726967696E656E756D0000000C45536C6963654F726967696E0000000D6175746F47656E6572617465640000000054797065656E756D0000000A45536C6963655479706500000000496D672000000006626F756E64734F626A6300000001000000000000526374310000000400000000546F70206C6F6E6700000000000000004C6566746C6F6E67000000000000000042746F6D6C6F6E670000001100000000526768746C6F6E67000000100000000375726C54455854000000010000000000006E756C6C54455854000000010000000000004D7367655445585400000001000000000006616C74546167544558540000000100000000000E63656C6C54657874497348544D4C626F6F6C010000000863656C6C546578745445585400000001000000000009686F727A416C69676E656E756D0000000F45536C696365486F727A416C69676E0000000764656661756C740000000976657274416C69676E656E756D0000000F45536C69636556657274416C69676E0000000764656661756C740000000B6267436F6C6F7254797065656E756D0000001145536C6963654247436F6C6F7254797065000000004E6F6E6500000009746F704F75747365746C6F6E67000000000000000A6C6566744F75747365746C6F6E67000000000000000C626F74746F6D4F75747365746C6F6E67000000000000000B72696768744F75747365746C6F6E6700000000003842494D042800000000000C000000013FF00000000000003842494D041100000000000101003842494D0414000000000004000000023842494D040C00000000028A00000001000000100000001100000030000003300000026E00180001FFD8FFE000104A46494600010200004800480000FFED000C41646F62655F434D0002FFEE000E41646F626500648000000001FFDB0084000C08080809080C09090C110B0A0B11150F0C0C0F1518131315131318110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C010D0B0B0D0E0D100E0E10140E0E0E14140E0E0E0E14110C0C0C0C0C11110C0C0C0C0C0C110C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0C0CFFC00011080011001003012200021101031101FFDD00040001FFC4013F0000010501010101010100000000000000030001020405060708090A0B0100010501010101010100000000000000010002030405060708090A0B1000010401030204020507060805030C33010002110304211231054151611322718132061491A1B14223241552C16233347282D14307259253F0E1F163733516A2B283264493546445C2A3743617D255E265F2B384C3D375E3F3462794A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F637475767778797A7B7C7D7E7F711000202010204040304050607070605350100021103213112044151617122130532819114A1B14223C152D1F0332462E1728292435315637334F1250616A2B283072635C2D2449354A317644555367465E2F2B384C3D375E3F34694A485B495C4D4E4F4A5B5C5D5E5F55666768696A6B6C6D6E6F62737475767778797A7B7C7FFDA000C03010002110311003F00ECBA71CCB2B61179F4CCC074BA20ABF762DFE9BA2C6383CEADB19200E36B6150E99914D54319612082674F13A2D2B73B14D701FAA0294FFFD0D9AFFA355FF16CFC81171BE97CD786249A3753FFD93842494D042100000000005500000001010000000F00410064006F00620065002000500068006F0074006F00730068006F00700000001300410064006F00620065002000500068006F0074006F00730068006F0070002000430053003200000001003842494D04060000000000070008000000010100FFE13A68687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F003C3F787061636B657420626567696E3D22EFBBBF222069643D2257354D304D7043656869487A7265537A4E54637A6B633964223F3E0A3C783A786D706D65746120786D6C6E733A783D2261646F62653A6E733A6D6574612F2220783A786D70746B3D22332E312E312D313131223E0A2020203C7264663A52444620786D6C6E733A7264663D22687474703A2F2F7777772E77332E6F72672F313939392F30322F32322D7264662D73796E7461782D6E7323223E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A7861704D4D3D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F6D6D2F220A202020202020202020202020786D6C6E733A73745265663D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F73547970652F5265736F7572636552656623223E0A2020202020202020203C7861704D4D3A446F63756D656E7449443E757569643A32443636393131333232393144413131413442423944413339423239384439363C2F7861704D4D3A446F63756D656E7449443E0A2020202020202020203C7861704D4D3A496E7374616E636549443E757569643A30364545383231303736413644413131413332443930464537463931433643363C2F7861704D4D3A496E7374616E636549443E0A2020202020202020203C7861704D4D3A4465726976656446726F6D207264663A7061727365547970653D225265736F75726365223E0A2020202020202020202020203C73745265663A696E7374616E636549443E757569643A32433636393131333232393144413131413442423944413339423239384439363C2F73745265663A696E7374616E636549443E0A2020202020202020202020203C73745265663A646F63756D656E7449443E757569643A32433636393131333232393144413131413442423944413339423239384439363C2F73745265663A646F63756D656E7449443E0A2020202020202020203C2F7861704D4D3A4465726976656446726F6D3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A7861703D22687474703A2F2F6E732E61646F62652E636F6D2F7861702F312E302F223E0A2020202020202020203C7861703A437265617465446174653E323030362D30312D32395431373A35333A35362D30363A30303C2F7861703A437265617465446174653E0A2020202020202020203C7861703A4D6F64696679446174653E323030362D30322D32355432313A31373A30392D30363A30303C2F7861703A4D6F64696679446174653E0A2020202020202020203C7861703A4D65746164617461446174653E323030362D30322D32355432313A31373A30392D30363A30303C2F7861703A4D65746164617461446174653E0A2020202020202020203C7861703A43726561746F72546F6F6C3E41646F62652050686F746F73686F70204353322057696E646F77733C2F7861703A43726561746F72546F6F6C3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A64633D22687474703A2F2F7075726C2E6F72672F64632F656C656D656E74732F312E312F223E0A2020202020202020203C64633A666F726D61743E696D6167652F6A7065673C2F64633A666F726D61743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A70686F746F73686F703D22687474703A2F2F6E732E61646F62652E636F6D2F70686F746F73686F702F312E302F223E0A2020202020202020203C70686F746F73686F703A436F6C6F724D6F64653E333C2F70686F746F73686F703A436F6C6F724D6F64653E0A2020202020202020203C70686F746F73686F703A486973746F72792F3E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A746966663D22687474703A2F2F6E732E61646F62652E636F6D2F746966662F312E302F223E0A2020202020202020203C746966663A4F7269656E746174696F6E3E313C2F746966663A4F7269656E746174696F6E3E0A2020202020202020203C746966663A585265736F6C7574696F6E3E3936303030302F31303030303C2F746966663A585265736F6C7574696F6E3E0A2020202020202020203C746966663A595265736F6C7574696F6E3E3936303030302F31303030303C2F746966663A595265736F6C7574696F6E3E0A2020202020202020203C746966663A5265736F6C7574696F6E556E69743E323C2F746966663A5265736F6C7574696F6E556E69743E0A2020202020202020203C746966663A4E61746976654469676573743E3235362C3235372C3235382C3235392C3236322C3237342C3237372C3238342C3533302C3533312C3238322C3238332C3239362C3330312C3331382C3331392C3532392C3533322C3330362C3237302C3237312C3237322C3330352C3331352C33333433323B37334535313537393633314641313532324636323944363242373743373442423C2F746966663A4E61746976654469676573743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020202020203C7264663A4465736372697074696F6E207264663A61626F75743D22220A202020202020202020202020786D6C6E733A657869663D22687474703A2F2F6E732E61646F62652E636F6D2F657869662F312E302F223E0A2020202020202020203C657869663A506978656C5844696D656E73696F6E3E31363C2F657869663A506978656C5844696D656E73696F6E3E0A2020202020202020203C657869663A506978656C5944696D656E73696F6E3E31373C2F657869663A506978656C5944696D656E73696F6E3E0A2020202020202020203C657869663A436F6C6F7253706163653E2D313C2F657869663A436F6C6F7253706163653E0A2020202020202020203C657869663A4E61746976654469676573743E33363836342C34303936302C34303936312C33373132312C33373132322C34303936322C34303936332C33373531302C34303936342C33363836372C33363836382C33333433342C33333433372C33343835302C33343835322C33343835352C33343835362C33373337372C33373337382C33373337392C33373338302C33373338312C33373338322C33373338332C33373338342C33373338352C33373338362C33373339362C34313438332C34313438342C34313438362C34313438372C34313438382C34313439322C34313439332C34313439352C34313732382C34313732392C34313733302C34313938352C34313938362C34313938372C34313938382C34313938392C34313939302C34313939312C34313939322C34313939332C34313939342C34313939352C34313939362C34323031362C302C322C342C352C362C372C382C392C31302C31312C31322C31332C31342C31352C31362C31372C31382C32302C32322C32332C32342C32352C32362C32372C32382C33303B45453039343231464241364638314233374442433041433132384530433741323C2F657869663A4E61746976654469676573743E0A2020202020203C2F7264663A4465736372697074696F6E3E0A2020203C2F7264663A5244463E0A3C2F783A786D706D6574613E0A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020200A202020202020202020202020202020202020202020202020202020200A3C3F787061636B657420656E643D2277223F3EFFEE000E41646F626500644000000001FFDB008400010101010101010101010101010101010101010101010101010101010101010101010101010101010101010202020202020202020202030303030303030303030101010101010101010101020201020203030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303FFC00011080011001003011100021101031101FFDD00040002FFC401A20000000602030100000000000000000000070806050409030A0201000B0100000603010101000000000000000000060504030702080109000A0B1000020103040103030203030302060975010203041105120621071322000831144132231509514216612433175271811862912543A1B1F02634720A19C1D13527E1533682F192A24454734546374763285556571AB2C2D2E2F2648374938465A3B3C3D3E3293866F3752A393A48494A58595A6768696A767778797A85868788898A9495969798999AA4A5A6A7A8A9AAB4B5B6B7B8B9BAC4C5C6C7C8C9CAD4D5D6D7D8D9DAE4E5E6E7E8E9EAF4F5F6F7F8F9FA110002010302040403050404040606056D010203110421120531060022134151073261147108428123911552A162163309B124C1D14372F017E18234259253186344F1A2B226351954364564270A7383934674C2D2E2F255657556378485A3B3C3D3E3F3291A94A4B4C4D4E4F495A5B5C5D5E5F52847576638768696A6B6C6D6E6F667778797A7B7C7D7E7F7485868788898A8B8C8D8E8F839495969798999A9B9C9D9E9F92A3A4A5A6A7A8A9AAABACADAEAFAFFDA000C03010002110311003F00D94BE3D4FDBFB836F6DE9E0ED8AE8F6E563E663A4A3CEC793CECB8C3495881E96962FBA868E4A2433C6B4DE4BFDB2AE901978F652143AD3D7A4D2B08E452CC0746FB77F596F54DB7954A4DE3B272D0EE3AFA596AF17BE3AEA973183A1C7AD2D3D054623111504B47574D412C34BF770895E4963AD95DF5842A8AA2387C1AD452BD2B9661324441AD01FF000F5FFFD0D94BE346FEDA3B57646D8C1EE19B23415F8FABCE3D4ABE3DE5A778B275B435343353CE1C878E5A7405C9D3676E350F64D6B388F434A054570715E98BA805C3060DA4FCBFD43A3BDBA3BCBAC67C1C74B16E02B50B1F96188434CCD512C3112B12ABCEADA9DB8B0B9F666D702E72129A4538F1FF00075E860F094A97D4DEBFE4E3D7FFD1B2EC0FFCCBBD8DFF008616CAFF00DD4E37D85EF388FB4F5BE97BD6BFF037FEAA64FF00A14FB38B2FECCFD9D6BAFFD9', 'motion detector OFF', 'jpg', 16, 17, 96),
(124, x'89504E470D0A1A0A0000000D494844520000000F0000001708020000005B3182EB000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000000774494D4507D6030517100FBBD965450000001874455874536F667477617265007061696E742E6E657420342E302E338CE697500000011E49444154384F6D9301B2C320084493FB9F2DCD8152CD7FF814ADF3773A648105C1A4C7BBE0F31F6AAD3DFDBEA12EA5102201C1852788DCF79DFCE0810269D4527D1C16033811C8755DB57EF9F549686063144A71CFF3240237DBD5F86BEF266EE7FEF40EF4DE3974AA816AB3B851CF039F6AAC3C842D17C206D5A09F95738B76C2F7791EDDA11E5B66351CA93045709F04DFDE5910C2E1BA25A4ABB1F91640EAB0792C986A40821A2CCDB022B360AAD76B59090570DD393716CEC42DD572833B646CE972A8218436B541B7FC51DB9B6F43C5DA7BF6E2CE559B4805E9ACCCEB8ADCAAF604010714CC5E4637B527C881DF05A447C71E0F3F06408D0D61435E6E9F72EC117F1012D9DB96FB24D851D0DD550A01D1C587F0B56FA0404D29E50F62B693F5FD0E43050000000049454E44AE426082', 'Outlet OFF', 'png', 15, 23, 96),
(125, x'89504E470D0A1A0A0000000D494844520000000F0000001708020000005B3182EB000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000000774494D4507D60305171136FDC7DC0C0000001874455874536F667477617265007061696E742E6E657420342E302E338CE697500000021049444154384F2D936B96DA300C85B3BA96C0AFB64C284B9E12DBCCE956264927B1AC7E57C6E4185B8FABAB8707778B4F2BE5F7523E4ACAAC94F89B4B7A4AD1FA66436371B223E7A7799C5B7593984B6D96BA5C421736174CBDD5EA7E5AA7CBF60BDD79B99DD709C0E67BC9B3E1D81AD62C4B258387068BEFDB843F073C45A07929C58C707510A7E6DD9AE36905F20A0C87D3760D2C2701D182B7BC853DD776201AB7DB79B903234ACB55BAE0C90ED5A889B972C7596CA8CF8E1B44039765B9FC094D1B424DAC0C5E84137580C9B2ED07570CC046D52A1594B7A7F4D0DD21F3B2BE6CB790B0847D90AD1D03C9E25E4A42C3896D5CDEA27C57A8888FC19B1AA8E68392436A4699E805A980D6E939A15E2411B0557A191028EC20D78F94E7FCC082FED394944A78ED828689928D882F8C481E89A4C14DAD118BEA6D176F84734E98C18A628F9F13A67414F672D108E5665F400D5C10313A1C40B82CD3E9DF0D4FDA342E3F623C08CB248A78E7ADDE46FA82D47868AAEEE3F63B6C988B39E6046CE2BB7A19E4344C7C084FEB5D071701AC054A2F5532EA9D3455B57D45724457DE91A9F46885D42B08EAB3D04B19C10F48D8B3EB4232CC77CA72A6F398F2E35D1DE2AD51A1142AC83AF5C621A1266106B6EF6292FF12A3C39FD79FDF60FFF9D64DA3CD4903A2F956AE485526C2A9F9EA8E7AA6F2D1E0E743A67D6245480445193EBC796A3FA7C26BC2BFCFA6DC78B5E6FF01C2939AD98D7D6A7E0000000049454E44AE426082', 'Outlet ON', 'png', 15, 23, 96),
(126, x'89504E470D0A1A0A0000000D494844520000000E00000018080200000045A55B00000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000000774494D4507D60219151606B0746F9E0000001874455874536F667477617265007061696E742E6E657420342E302E338CE69750000001F849444154384F4D93D992DB201045F5682D93CF9B1FCE687126BF318940B2938A176D40E7B4DAE3A4ADA2A0395C2E0DCE229124A514A3CCEB12C31A83F085904484C910C2BAAEDBB6652905D02841367E6193A47D123BB74B287DBFDFB384C6A6745270098B32081AC780B005D941F24A8A528AA6AB9BF6581FBBA669DABA23E8D4758D24341EB297901F7694ACF7FED47F9C4EC3A91FE90FC3D0B6AD1985CE90AC525E4A796CBBD1FD19DCD9FB7E707E545251B6865303829F246B94EEF8E647771A46E249837220339DB1752E872FB178EFBEAA8E3B9F7BE5EC331449552D252FA47891A2EBBE8D831BFC6FDAB37748FADE71B26559E010CEE0AAF43816D038FC3CE3D2FF30943AA04A11541513C42A4276877ACEEE9C6377436FB71BA8AAA257C9214F79DB1EB540FE97FADD0D101830D5075A08C552AF21ED77C54D46C122018AEAA3AE65AA2A2939DC7BF39DF710529CEF13C5611A252A70BD5EF5FC78BD4FDB659A79064DF32600B28669A38C4FF472B93CD069BA85DD77D3B591CB8840E415254CD59E8156C004B045CB8D63CE8E32CF3328490BDEAB5E1A13645163480B4486654F038ADA9109B2F46D68EB6949B2CC32AA4A3041967DA7CFB03E49B40DD0FF962971B14CD0BEEE8175CB1800ADAAD8FA7F535A3B327DF236453CDE00F14C110C29CB2EC102FD887F5E699F62C4E74A24763AC95F784E84583158DEF50000000049454E44AE426082', 'Switch - Decora ON', 'png', 14, 24, 96),
(127, x'89504E470D0A1A0A0000000D494844520000000E00000018080200000045A55B00000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000000774494D4507D6021915163696AD5F320000001874455874536F667477617265007061696E742E6E657420342E302E338CE69750000001E349444154384F4D936976DB300C8475CADCB7AF9128B9F768CD45CE0F5B2BB77C206C3778122D0283C100A4BB9C6B2925D79463398EADA6A3A4CA9352A9B5E69C534AE779C618BB86ACA5C69C78CF5C6AF324C5094B43AFEBDAD5281C70CB134B3C2B7940154702A6095D70B3B7EEE6BC31FD808DA6EF0DCB388EECFABE8712341ABAD9BA1082F7FE77FF1948B2FFE6E09C9317A731468582EEBC17566F6F93B9B012F6EE3A5F6F419002A5343811205A6A4EB94E97DEBB2F28BDB782A29AA83234A4A245AB68707E1A7E057E2C6A6CAB238F42A114D69B152212C6F1F2E505D7A45B9CEE6AE9EC380E70100B2BA4141C864F8240677B6DC902650EB0320461452842D84CD3A4839BED5F507C28745916A2C28A0B041CBD1944DFD35A7B526A50D6A780D6D63C4D7F28908A4C838E91880185F539579115046ACC2463AB655B760E96304C4CE0F178B401E46E5BE3B2EEE98C1C64CE3195188FC4AD7843EFF7FB0BBA1DA08971FAC46891935428A6AC7A0DE4B4940059ACE010A7ADECFB0E54F29B75BCA009E0A50A5B564078487B0B10A8B4D20C2FDFBAD57C569CA4A94758310278A9BBBD4CBF718AF406E0BFF5BC63DC6202AC1FCD90AE1E0580165664FD2CCAAA2DF38D5F4398DE57B1B70B63CB581A859C5C73D4FF5A59DF64D82B138A862EF51B8EDDB1B7D307599E0000000049454E44AE426082', 'Switch - Decora OFF', 'png', 14, 24, 96),
(131, x'89504E470D0A1A0A0000000D494844520000006400000064080600000070E29554000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC200000EC20115284A8000004B9B49444154785ECDBD0798AD5959E7FBEE9CAA76E55C75EAE4CE4D37511B078401C4B1E142833A8ECF15AF82E13A33CAA08233F7B9D82A0698F11A465011101C14504CADDDF2080A8A8D129BCEDD27D53995F3CE39DCDFFFFDF6AE531D806E6C60D639ABD6175778FF6F5CDFFABE1DB2FFCD52B7DB8D7EF4A31F1DFAD4E73E35D96D758F8F4E8C7F53B7D3F9B6B5F5F51BD6D7D763B9DD3DEBB63BD66AB7ADD56C7AD9E9742C168D5A281CB6702864F178DC12C9A42513099B9C986C2ECC1FF9DCCEF6CE3BEE7FF0FE4F1E3B766CF3E69B6FAE7FFCE31FEFDC7AEBAD9D5EB35FD7C418C32A42A15037387239FD6F03C8EDB7DF9EF8E8473FF2AD972E5DBA391A4F9C4AA55257562AF559881D4BA692164B24FDBA642A1110BE9723918897F57ADD8161907E1DA3B68E72B569B56245E035B9EEC16EA7FBA942A974A6502EEE9E3E7522F7F257BE7CEB99CF7CE6E6C4C4C426F756C8EDA0826F4CFA860202A7847EF7777F37FA8FFFF8B12BB656777F2C3B3274733C9E9C0941E591D1310B4763168FC52D0EA7C7E209F6A3D6E8B62C1409591820A291288004C0B4919A6EB703285D6B772435D0B5C3750D406AB4ADDB6A59B3D9B06AB5D2ACD46B8D56BBD9ACD76AADDDBD9DC6E8E848F585FFF6852B2F7EC98BFF617A7AFA4F91B07BBE51C07CDD0111083FF9933F998EC562577EFEB39F7F51BBDDFCB7A36393D78F8C8E8EA53303D10492108EC52C164B5A371CB5282024D369CB64065043096B47C3C8BA390892944838845450729DA4A2D56E59A3D180F84D6BB7DA166E02481DD58604D56A556BD46BD6029C4E07801A75EE35DBDBDBB333671FEEA65289CE4B5EF4A2DD6FBAE99BDE094FFCCF673FFBD9DB5F6F60BE6E80BCF9CD6F0EAFACAC4C2C2D2D3D339BCDBE16DA7DC7F0E804EA286DC94CC600C3526CC79369E3A06506B216453A1229CE0DA42D9D1AB048028000A78BEA05586A55F703351C0E47F82B0931A40509E922156D541640B44A15978E5AB562A542DE2A95B23539DE6ED46C7F7FD75CA1235D3B5B9B96CFED5BA554E85C7FFD75F78F8F8FBFF359CF7AD61DCF7FFEF39700A6E90D7D8DD3D70590F7BDEF7D99DB6EBBEDD5A552E97BD0D5026434914C852299218BA5072178C652E98C0D0C0ED9D0E8B80D0C65918634EA288E7A8A9958B405B737C5FD70B6DB0628DF264B356117DC9604EA0B5586B4F4B743DC1D92D147BD8501B28D54D42AD81424656773DD1A8054EE8154AB92CB80552959616F57C056CAE5F21700E4633FF1133FF111BA215556223FC6183F55E96B0AC8BDF7DE1BFFEDDFFEED7F77F6ECD99F1A1D1DBD168E1B482693689BB06506B3969D3D62A1CCA0A506066C627CCA46C6C62D8ACA6A43F03A7ABF8EBAA9546B5628433408564715158B4500010C0184EA69BBFAE9B8CA122802435E162A916DF691AA4C268584A56C209DB434F6288E0D8AD04618804BF97D2BE673D6A8D528F76D7DF992B58B053FB7BDBD2D503A381AB5C5C5C5D51FFFF11FBFFFF8F1E3BF9C4EA73F0D285F130FED6B02C8DFFFFDDF27FFF44FFFF4F8CAFADA4F41C4574F4C4E0E0C63A431D8A8A0B44D4E4D1B76C3A2C3C396C80ED9C0309282CD28575021B982155031951AC42F572D572C0142D5AA10AC850AAAD5B00D4884EC80A4A6DDD2BE0C7AD7ED4114474060C80D1648B164D80606E29602884180191D861132A8C078D426C6462C81244561F856B3EEC0E4B1277081AD5C5CB20665A954B44261DF72A8B67824DCFDEEEFFACEFD673EE3C65BE7E68EBE1F50F69F6A609E5240204AF20FDEF99E9B6EFBC81D2F6B873AB74492B1F9F199E9702A93B570226D13D30B3631358F4A1AB5A1A1110B2763A8A03AEAA28661CDDBD6CE1E8000008428162046B5114847A98E7488F021AB3423384FA822A82F631EC286A8543C223BA06D4920B69ED1852D116E582ADAC65B93A4246C383B6043D98C6592719B9E1AB3A1C114C08CDAC8E8A0E12FA0CE6AD6AEB62CB7BB6B9BEB2B56CCED5AB59CA3DCB1E2DEB6ED6C6CD8F1230BC557BDE2967FB9E99BBFF9E753835FBC33147A412BA0C0BF3E3D6580ACAEAEA63FFCC77FF6A37FF7771FFBBF338399C5703C1E199E18B3E18971CB00C0C0C8984DCD1EB5D1C939D448C68AA59A6DEC6E59AE54B0BDFDBCADAF6FD9E6D68EE5F36580A8232D0D4C349E13BA3F16250E89E1F662E407872731EAB20D2401C286B663A829A56653DE53034F0B632E909A150C7B89E3C429ED2644EF5A221E415A2336329441550E3B2063003235396E93E3E336020375A9A350D8B1DDAD35DBDD5CB1FDED752B2125C5BD1D6C4CD5EAE572FB05CF7BC103AF78E52DFF697A7EFE134F955D794A004132C2EF79D77B7EE94F3EFCE73F343139319448A7433373B3961E19423212363D7F94BC68E9A131ABD4BBB6BD57B04B2B6B76717DC37601601BC9D8210217089860D44E02433F6C11D4583C19786049795FC4220D882CC72A30ECF2B62419785980A3131CF2520775DCE404B4A8171557AF95E1F6BC5531DA2D3CAC48B8638303495763E3F4757C0249C90EDA1547176C6C08491A4859BB59B5BDED15DB5CB9687B5B1B56478595F791E4ED1D2468A3FBF4A7DD70E9CD3FFF73FF19DBF5574F85FA0A292EF8D7A0FBA637BD69241EB6FFF2CF9FF9FC9B86C6A6A3B3F30B96618023E363363A35659373472C3B3A615D3CA68D9DBC5D5C5EB7F5ED3DD4D3BE2D6F211DA8A6223683480DC20FD9F0D8980D6447080607A03251385E96E21176446E080F64B091AB259D4737695B49764420B8E7E541A2E3E2F786404A12D2AAC3DDC423ED16EDE6F7F0AA8A78572582CC10D2326A339363363F3160D3A3593BB6386F53801407B846256FBB1BAB8042FF2F5DB24A1E5B8727B6B5B965F373B3F9FFF37BBEE7D6D3575FF92E1C978277E6AB4C5FB58408C8B7BEF5ADA73FFBE93BFF6BB150FCAEC5935724B313F3961C18B2B9857954D5A48D9013C413650CF1CAE6AE3D7CE1922DAFA19A006477AF6895768CD822EB12209737911E20F648380021A4241C450D0148081525CEEF10A5A752105F8CC8BE00D0868A30C619ABA23D3FD73FDEED46708BC3D669060E40A755275021A410686D545B4DB1099E5621C7E186E194D9E840C846B3499B9F9DB6C5F9199B9B1EB799B1211468CBF2BB48C6F245DB5A5FB51231CBD6E606206DDB702AB9FFEF5FFDDDEF7ADE8B9FF7FF65329935F5E0AB495F3520CBCBE79FF3C637FED46FB5DBDDEB8786876363B38B161D98B09905D4D3EC9C4D2F2C580DB7F5C2CA865D402A96B1114B6CAFA39EE44DA507876D6CF238200C631F705309062380A1B8A30BE7770045FB11CE818C03D2EA36311B6404DA8DBAA274FD635B5E57009043E1A50A30000CEC86BC2F596D80500CD25134DFA82AC0B1980B58DB7270FCE6C60A2A69C501999E9EB029ECCB0CC6FFE4C21CD2336223A8B146B96017CF9DB5B59525F7BEBA7880AB671EC6950E557EE0FBBFFFB32F7EC98B7F3C9408DDE57D7892E9AB02E481073E37FBE6FFF7E73FD668B5AE583C7E1C9D11B3851357DAD0EC313B72FCB40D61C073E59A3D7CF6A29DBBB80A28819A5ADBDE770F69626AC6661716891B321059F102411B84D7DC1522E280B40DCE566087510F212992804E84C08E7F8AD41F9D148B3C5E8A201D61CC4E582A0B7BD21542D8A110DB8A4B9A0486F55EE4DE414240C8B63797B035FB3EDD32988A63ECA5C6C66D7176C2AEB9F2841D01A8BDED4DDB58BD08632ED9FA85F316C2B6ECE3945C5A3ADFFDCD5FFF8D87AFBAFACA5724B3C907835E3CF1F4A40179CB5B7E66EACE3B3FFB3B783F2F9B5B3C161E85B823631318F0093B7AD5D32D8187B28914ACAC6FDBA5B56D7BF8DCB2ABAB26AA23813A1B9D98B0415CDE90260EE303103A492FDC28B847E560C86610D475388685731075BEC37619AE86C69E848BE6B5942218754D6AF7B1F2F92EB681DA1272857B0625A2921CC619E860D843D82E74994FE94B7A400C3596477A2A103DF0B268D9162627EDC4E28C1D999DC4ED9DB69989116BD68AB6BE860786FA2A9277D656AD4E209BDBD9E9FCD06B5FFB7B2F7DD98B7F1AE9CD073D7A62E94901F29EF7BC67FAB6DB3EFC967AABF31F164E9C4A26513BE3730B36BF78C2A65055961CC458E7EDECD2B23DF0D079BB7009FD5AC0FD84DBC7A6E76D7C66DEA2784A6D88AB3299C270A3A24428115F20A1575C5DD5E1E226C76B70741DA2C9E8B7206488D885BFDE1FFDEDCB8A2444AAEA3020DA8E60DC63D41307E8780CA600F4041209F43ECF458B92459F0D6E13EBB470979B3522758EB7E5FAE251E5893F1AB8E743C4310B33E3761C60AE3C79C48ECC4FBA3DCA6DAFD9FA99876C7B75D56A5CB77C6E89F642851F7EDDEBDEF6AD2FFA96B7028A44EF0925CDC83DA1C480337FFE671FFCD0F94B176F3E71C595C906B71E3975A5CD1E3B658B575C6D75F657B676EDA17317C817EDEC8555DB2DD66D6074D2668F9C709737921AB4703C8D214F5A08FBD004842604325C5A4278EA302BA036F6AA65325E106094E1DA3AC46DA2B2DA10B41B0710498F67C0C32E683B9CC4FE108B8470B343094AAED3B624AD0D759BD4510798AAEA0488129E562C9D7286E8528F1C89966C1139AA638019E2580A47435982552814F0088B56E55E4D56A63249D4F3880D0D0FC21CD8208CBC3F1C23EFEE6C273656579FF5F4EB6ECCBDED57DFF6995B6FBD3520E457484F08908B172F8EFCE3C73FF63BEFFCBDDF7BD915575D13951B3B0AB74F125F8CCE1E812029DBC375BDFBBE07ECCCF9257BE8E10B9627F01B472A66E64F1008120533A8B8666C21BEE6AB2271321E581BE2555A1DDB4797E720541949A863ACDB0CAA1D83F89CEFEAE15492AC792E38BD2B5516096C4C17773570020295D6413D05656F1BC042AA47D9EF0158EAD7768D20B201A55B10BF431D213917B411EF4FBDE0AACBA5969717437AE538EC4B62F27900D3D44D83EB63363935CE3D61E2A4A89534D7C67862D477E1DCB9C4C30F3DF4CDCF7CC633EEFA95B7FDCA12A0F405FA4BA6AF08089231F7BFDEFBDE9F7BF7BBDFF5BD27AFB822323C3E698370FD1C923131B768D9A9593BB7BC61F73D7CD61E3A73C12E60C49BB0E4E8E42CF1C7346EF08847E691385E14419EA4A3CB809B102C07410AB8A365D45343C4435DA1C7504B290BA73300800B0C919C9892084D0AB21F825821F6A5E2FA25D4039CC00E11F1F5B2D45FD8B9BE13E2B8135D0E84EE8BF5D46288B2EBEAB1451F425C13E57AD91F6EE016AEA70ED9B9144CA109CB26B6A75225405480E9816ADBB2D9B44D6267F4CC45EAB34119E7DA07EFBF3F552AE4AE3F32BFF8B95FFDB55F5DFF4AA07C5940A838F5D77FFDD73FF507EFFFC31F5A387E3239303A6E4393733632BD600B27AFB00C867C0DEFE93C36E3EE7B1EB2E5D5029178D4C6A616918EA3164D652D819D89136320F7D661400D0852C688EE68BE0A6B50430D75E0B230DE0CB21E187308EF596EAD882320208A32BBBD1C9CEBEF0781A24A058AFD1C3CC0F21845869DF3BA588E82EFD096406B537F93F3756C491D6F0CD8380DE81E13D11F2EF7F6A9270EB32460946AB5EE939ECD36E701343B9026A01DB22C816D13BB5595F3817ACC6652F6F07DF74D8C0E654E1F9D9BFBDB5F7CEB5BBF6CE0A82E7EC974CF3D9F3BF9A93BFFE9FB889ED3E566D30D736674CA668E9E420216AC506ED8F9F317ED0CD2B1B2B269E56A8460F0A80D8DCE2311598B62E4BB04781DD90BB8BA4247F374B2805D1030DD14034E4A35C17510AE057DDA518D5E540E68168655235867E4075A7624086400F27C795BD70520F4EFEB1D53758C4521883F2311385CE0D224F0E3CA8042D9E4A21A17EFD71B966BB4AD061892D8700A699533826D09A36A359F36383C6DD566C4D6B74AB6BA51B0071EC4FDDDC9C15C691B9D9F27CF59667C84586B4A53FFE17FFEA73BBF65279FFB69622575E74BA62F7952D2F1A94FFEE3CFFED95FDCF66F3243D9D0C92BAEA433293B7EC5553683AAAAC3360F01C6DDF73E6867CE2ED91E067C7064D6E68F1CC756A4E97C04A72B8B614D42E8B055918A42BD89BD68722EEA06B50B217CFD0504559027228A4544301836E0FE83633D6960DB5DDD47673F2E4BFCC8E3FD7ADCE5A2547597EB116041BD7E9E63FCB76E2378CED242A5EA803FFCE2C6841C05549B5C644DF1D701AE542C5BB594276629F923E6540AC006329696FA85898D6B14E3DC7FDFBDE181C1CCA9E3A78EFED92FFFF2DB7669E671D3E30222143FFFF94FFDE8EFBDE73DFF2999492727A6E7B007E3B8B7C76D9EC0AF868DB8EFA17376CF8367EDCCD2AAEDE72B48C69C4DCE9EB0D4C03052813184B36219BC133A5EA835AC825E95B724FB216F288C7A925A121881BA0938578B179CF8BDFD406505A567CEF7AF3F9CFDBAC739AE49C7C3E7A8E172BD7E4CF5EABACBD724E8BFCED701A586BDD073FA18044EC36802A7055309C30C8E8AEAD3B44BA74DF4AF894E8E0F0E6A6272C0124898A6F1351BA0499D4F7EEA53F1D1F189C6073FF8A18F7D295B2286784C6A362B4FFFFDF7BEF7F5855261709408757098800ED19B9A5F70295927E23E73618D388368751B9F3D91B5E985E396C25E28A84B201999E151ABD1E4BE9EC431A0069D0B13194732690BA1AA5A0C4C6A4300689061D487B26F2BF788D3DFD67181E1DCFA38F9E0FA476511FFF03507D752FAA35E65881D3CF6E51C7D689143F4339E1D74BB57462A72C42472C53BD88F01BCCC7062C09299619B9C39822A5FB01C5EE57998534ECDF64EC11D9BA1B1499B5958C4091AF3C70FE393D39177BEF35DAF78E081074EF648FD98F41840908EF01F7EE08FBFEDAE7BEE991A1E1DB51822383C3E6E13D333962196D8D92FDAD2CABAAD11736CE72A18BE013AB468F1CC48A063C545785355065160105505C00CB8932023198601C759473A309A4E28E90471AD38945212A3FD7EE6B4ABADFE71B1A67AFDA87C70FDE364DDDB578D8FBE56757ABBD88FB0327DEA526A758BFA1C51AC42AED1B0DCF2123EB21E3BC770D9651FA3C901E8330D338EDB4EA16A9756771C942DEC490C377F78729A58EDB80D8C8D5B628078251C5DFCAD77FCCE6BA0B362D2C72475F311E9D39FFEF4E4073EF047378D8E4DC46370C920C80E0E8FFBA4619DCE28F8939B7B716307A31622FAC6A32237BAB88718340152227ADD2E16DC9D352D6CA31E43BFCA756DC19D0AD4448090C038A42AA4E05D2DF533C73CF75496AE7DCC354F204BBF043688DC2F19EBE16B9403892230C4D1903AADA1821AF27FD9D718DAD890220161BE56072C825B18AF19264E8A6588C98E593B94C4C0E3755E58B79575D1A76BA9A151246DD892E491F109683912DB58DBF89E0F7EF083CF061475E311E911807041F4139FF8C44B51F7CF1E9D980C678735EF346EB3A8A3182EEC1E6E9E260BCF5D5A433ACAA0CE79E29286A6C7355D9E4EBB4B5BD4124F490360D4C5891A10DC16C29371E23A517A0491DA3A0446EF84AB0FBFB6775E762422B5A6EDC7C97DB5F4E8DCF7AA644B741D7F7C5FD91F7269DCBDBEA85DB5D7D104A6C29914F62E49D42E4F4CF3F2484A833A73F59A95889F3A785E11EC48044988116F0D8DCE5A193DBDBE99B3A5E54DBB00E30A34D1686C76DE1248D61000356AF5A3671F3EF783F7DD771FEEDB23D32300D9DEDEBE6E6B6BEB3FC6E389D1541A3B30388AAA9AB77476C47288E3C5950D5B413276F24500CAA023A78D0DCF1102B92A86ABD2691169E3BDA463D80B714F3FA660E0E450580F99940397D42330B2D3C3CFB3AD6B232A837C709E7BBA6C3F99EC2ACA3DA85E3D2AA95BEECCE1FA8373417F744C1EAF18474E6A9BEB3B727FA47651B90DC0ACB15BA34F2D6C6617606453C6654F0807F2859AADAEEDD8D2EA86CF7AA7D1328A4F62D0288DAA8B2752A1FBEE7FF0552B2B2B27A8E611495DF084748C54ABD5FF7EE79D773E7D303B44209B22D019B7F1A905DCB941DBDACF3B20AB0052C62D1C1C19C7700FE34D11BDE251C97BAA0206AEB9FBF6EED2D2F988C43F2AE2D0180354D6D8E03731AB73AD4B8A24C4CFC3C9922A1D233FE27CEFD893CD07751CDEEEB5F5E8632A1112CC0DA08879748C03B22B84DE2EE92D62A53AE7E56BB538DE8D245C65C793590CF72C834AD8D65EDED63651EF2BABB8FA44F2131381CA42754D60EC37D7D7B3BBBBBB3FF868B5750048ABD57AEE6DB7DDF62DF8D7A12C11671209191E9DB40CA258A8346C65758B2068C797E5E8E99E021E9F94438415E936A46F11F550124388A8EB585BDCC6BE1010C7492A500016A70B080E830E8871387721001B07FBAE6EC83A264AF58F3FE1ECD4BDBC0FC50FB6D596CEF7F795156FC43CF783C9208700214400ABD9684DBF18DBED9E4DD43490264D43D1A449CD0F41B742A96EE751EF7A30B7B59FB3F4D0B04DCDCEC1DC491B201C18C751FACBBFFCCB5701CA5C0F024F34E9D2112A168BC3EF7DEF7BC3333333363C3262A3780593A8AB088DACE969DFA555DBDCD103A630523365493C863692AE29F17CA56C2D79E0DEE12087E2E21CCE6B71B4E8E08353D44DB40D505A0B056607E79419DB413E7C4CBD7CF4F12793FB75783D92BE43C7A1E563EA9554447B5978BAFA2277E8BBAB522445719422FD5697F80A09D06C710CB515C1960E0F8FF98CF1FAD6B66DC0C4CB1B1BBE3843CF82464630F2B1848D0C8FD8850B4B9377DC71C7AB0F4B09CD790ABDEF7DEF3B168FC74359D4909ED029101CC268EBD984C46F0330F68BB8B97042062EE8E2B636E965B5838F5E2E3BB78450512DAAC6B772DF5E1CD7C2F555626CEED91C6C008AEB2E6D43051146734C52196E64B94E59F3482AA5F2FAC703FDDFBF26D0FD8FDCA64EB7135CDBBB2F40BFDF5E905D4D695BBBAA57FF550F077DD74B322754C2B91EF8C14D4804DA01316F44DA6E37F7B0156DE8566D872C4E3C96414ABAA19833F1FADA3676A542DC32889D99C3E3CAFAD3D1C9F1F128DED62DD49855B54AEABAEDECEC643EFC277FF89291B1E1B06C4118172F35366A1DBCA4E5BD5D5B4217AEED15AC8EEB3C3E7B1CAF2A65ED5896A069D0E77C52E35356970784171249EB79049DE59FA6A007B47C070A45452500D274B8D499EB5EC08E84008EAC670F9A69D5A49EF69D525C2FFD2DE28A43E5F9B4D1EB1D082F832D632D8EF709446D3B113947E62A18034F08AED6BDFDEC4CD0CB11F452982C75242908E6C4F46483761C55CEF38F5E590C54E3F4274E7FE21A1F51442B419D43498B4F0CDB5EA3635BF5B635334356D63AB2A10968310218FBB6BB55B05D02E87608469F9BB3D8E404800DDB18D252CAE5AFBBEB739F7B615F4AC2DAF8E8476E7F7E229A3C3D38980D0D0C0D211D63043B133E25BD9B2BD8E6EE9E4B471CBB92204B5F6ADABA54AD41603A877FEE22AC813A8789304129C694AD088E05DCDA85003E6DA23133F03E572A5FE6E0FEB6081F9C732EE51EBF4EF7D05E708EDD7E1DFDF6A5879CA63D89F363FA13945E87D2E17D1DEAEDD36A501FFB6A17F95755BD7618034CA22CE6E80850E2AC62A3693531114C18973745D0D84262F285B215C8F84216C5D34A0CA2DA627ACB2BA139B1A18F7FFC1FA4B646D41D753976E727FFE945915864248D5D48E3578F4A5D61472A0440DB3BBBB6B3BBEB2B45929C8B291227C6D0B384420515062051F6F5524D6080C565947052B002BD47703F1E70B3062A7A5DBE3E2042B0CD353D4ED571E7FE7E16615C8A647C752FB1863256B7DFA6E28DFEBE9F9374EA9CB779B99EC3DBFCBF9C0F9D0F84BADF47E55E1B5E77AFEC8D332A779818ACAE094568A205E4199CA30EBA766FBFE08B014BA876D1696864D8E2A9A4A588DBA05D687D63F35B5757B74E4B38C267CE9C19DADBDFBB3A1E4FC6845A049595C18B124F140A45DBCBEB19077600232530F460A74D876BED0E9E156EA1038284C86678677BC4799C2C00FA5E53FFF9440004DB3DEE0B082497B37F9C9E28C3B3EEF5704C04561658342995EE0F95FC9985EEE9E5FEBEB85BC01C641F5DBFADCB39E857AF24F7DBEF1F0B80BA5CFF418606A29B98B30AAD64C0354D2F3AEA815CA95CF585813B7B7B860FE48B3C52998C3F5B19C4FBDACBED4F6E6C2C5F2B0109DFFD99CF8CB55BEDC534888AA8323C2924A54624BA8F31DFDDDBB73AFA31A3F01F63D5424F94EA2DABB5F0966854DC2003771051F709D9CF1CEF0FB82F1DC100FBA5383300805DDF0E0807F1E9A108CE293F4F4D3DE207C7FBDB024A59AE74FF588CCA04923FFD533F28BD0FFD7D329B7EAC9F957C5B6DF93597B775BC2F1D8FCC5CA38E4203B9C4B576D38AA8F2060C1B4575E9A155132991EA5F23862BA26952D058A0E871B1DE8BC9E5F3D1FDFDFCF5D4120B5FBA78F148A1509A8F73520BD3B2C3E3D88941745F17646B885BD1571EA6FA8084359D8EAE04EA2862D7E9514646143AD2C1CBB9BF1F105CA3D5BE0C71FF9CBCA2C040BB6DF063224260A055B583A26D4A196E3FC6BE66E69421C341F6633A470E8051BBC1032EDD23574365E000C85F0A1C03E5A00FC1763F5A1746EA875FD3DFF73E067DED7B758A67428908117CC7E39292265561582D9F4D0D8E51625F88E5B4BA5FAF5A486A1454EB31849EEFEFE6F2B6B39F3B464C120FA70633CF2A574AA9182EDBC00001A18C516AC00A18F1DD5CCECAD5BA1BF05476C45DDE42B569653DB8D15C15398CEEEC127728E6E8EBEE3EB769C0E8453FD6E7A647709646AB63502938C72EF5E870884053302B6E71EEE79A381B8437C6D82D4199A44C8184624F62512F95752EC84808BD60335053504F394A23AEF2FAB686D2B755F6E7CBB8EE725F837C783C9AAEEF4FD96BBC2D0189844432298C7CC4F2E50A7625EDD34EA810089E47B3D43101456237D42FA0440912538383B419B3E58BCB0BF97C3E15CE24532FA5E250043B200989518916049430E865745F1DD5158DA7B839E3C0D4A8ACC5794D9F6BA586AFF6505721A2B2D8E8322041EAEF7B79E89C3FDBF643FAA347B4BD133DAE1517EB9CD48D548D385F513EAC10948F93F144836DD91C7AA6ED008C0068075B4DF00F060FDAD63E65B0DD2BF53FD8F5D4BFAE9FB4EF207158DE9666B07D7A05903C46434DB5E05205D6525D6DF62BB5B6BFFB82E575FB21402208420C6F6B6373335DCBD7B2E1A5A58B4FEB2FF78FC413B8B610DED72095D06D5A83D4F4D5889A4AA9A1C6F4E8B68B640810C51372F9346DA2C0CDC7E1A314710311771570B07F28BBB807EA81189863BA5812D1533192188E6955BA8F13695099E4B224F7483A0E24A59F7BC7FC3CEA23452989D2E481EEBD0C4AAF4F9C0F40BFDC270F287BFB909863EC723CB886EDDE75C1794D923A3AA82AF668402A48CB61B5CCA802338713296C06311B4C2C7555C40C34A1A31E6FA70910E50C24B9E6ECD9B3E976B79D0D3FF8C0FD03192CBE0C8CA423012075F45F3E5FC27F2E816AD38F6BCD521963DEA01F61801320D297EA84DE1D7740E88C731EC79545E43E271D6C33C2609B6BFC9CEE0BB2C6E2251B512818C560C84BEACF2D29BB7A82C022B616C27BD6BE1F3BB42FF545E9A0B1EDA090BD0D68482F0EFA15707AD0A7837EA9E478D0C7DE757E4DB0DF57BFDAE74F30AD42235A29E3EB93F14A4BB5863B3E9158CAAA04D032E865344F9578451222831FC50D960BBCB2BA3A5CAC55C7C3AB2B2BA184A68F4154135F312AA86323CAD5AABFCFD7C0AF56A30D40AAD69B9A3E74407C62519DA1539ACF91DAD2750E8ACA5EEE77FA20F7CE07833B7C0FDB2A7B5CA7FB148F88886EC829455427305C1C949709EFC467FBF039CF48701F0C197A0ADA60A3DFDF7E56DBFD7EF4F61FEF9AC78C872C8922FE03044DF1B02171449D5731EE75A241C5220DA445EF4DCA2DAED46BCE195AF12F40348D029D53A57CE9EAB09646CA9D531FF55A98569B37D0594D00D0822F4D2036B11B5590AD81ACA63BE477CB4FEFDB0C5DE754941AE87B2E126BA90057478F1473DFD7F6E17D3FDF1FA05C64FAC3A608E82AABB72D2989735E065BEA4AC75D7A28FB863CC176DFA6E85C5FDDF5BDBBE045A7207B1FD56FEFD3E5BE396370ADCEF5FBE8FDEDDFDBBB2E3816CC06E806F7B8A0A1E824FBBB8FA66974DADE5AAD56B32A36A40128A2AB9EDF4771043464B454F8EF3EFEB117853579A7E5957ABEAD97F8154FD49BF8D18D0A00547C5E07F9B23DBCAD12BEB53F82A506DD27B6D5C2E93A0AA04AE39A970A710DB541084C291E89F4B06706123C74A26B9EF5265460337C90BDC189CB7C4A448413312198EC8054519A9CA4CE24D725C869FA30C0B17ED6BEAE8B43142DAE8ED1377783397620216C87E987B29850BD0B24A30704C7C25057DE189B8C457646FDD5273D946152B69BDDA6DB8D16FDADE935074AD143719A56E56892B1DA8AD876A1CA3D29DA880344CBAAA5B22FEAD69C9D584CF376EA670AF19E1A1F7D4138782106F50052D26542AD8E48D51B55AB54B999CE6A59E77E550B16642F0C1BA31732E11A46A7B5B60D2A2FD298BACFD50C3C702DA58735434A150C94411F929E2E83E46E060F28220A99034E1CE736BF566B643B10BF6D29729A9CE0980C7B4A80707D863C4005CADA162092108F49A848B1474CF5707D94BA2230427F719D88DFE772EF1B192BE0FD0D40098CBE5C6F143364823D233E65694D8E6B82548B394A9A2E61BC7A3624E35D8514A53A12D18D13240266548C9EB4365AA789A6D10BA531AD1B263EA149D42A92826394CFEDA450E5EA589F08A21FC4E29FDE0597BD90F4B4E88716246B5FEAABAE6913BD1EC0B5C01274863AF4C43092A2113C0789A4FC747D41416EA7F801F9091A21F9348AFEF9E01D0B06CFBEB639A6E3BACFCF89289C57561DAA2BE665F05C25E0B5400274AF332BFBE26E77797D5B0496EDD27690D8F4B1F7CF2907F40008C0F3F3E47EDF5C6AD41F18309E4A210D612BA3AEB50CB50541F5C1033D170202A7A767C6E92FAA52FA3ED7E8E338AA39890D9173A0A452EFC463A3E8B23AC13F5DACAF24B8EDE8D90F4D8B6BD24C80A8123D909281925E54802363A6C5CC7ACE5CD075489AAF2EE76A3DA48913D93BF13A48A1B88EED408E449C5E66DB83362FD9E7BA83F387AE15A1F5D0C85510D96D477F9F7322BCDB0D4A65DFF6E36AB74754B6F52F0027D877EDC5768F0A8C59FBBA5E804A42E8BF620AA29B105CEFEB7EA30924A18B6668FAF3755F454FF68513DCEBDE1659F4A9E999101D164D5CBA6453A0733C415D6A8C3684835E770090C085A3279EF4DD90001095745080206A0805493C2762D310446B70AC016A0DAEAD724FAEDDB05CAB69550620B7D8EF60B01A4C8C7B7107BC14F10F4010487D30743DFD706EF5D6D8E6805460701EDBC0790F107B6078A9FDDE31979EDE35979D01811A00E6C4F5BA0E3180D74F56BBEA1B40844252510118BA4E728E62A13FB8FC2142801AE3C50B2DC2EC7A4827409A4889165A6BFDE2615002AF4AB657CFE1B916C6157012804022690375A76F7E3920EE7FF34F42E05222CE1720BA01E434AF25A002B661C8643DC051E5525FB556CB2A9A8EA7A1ED6AC52A40D68122BEC41F3055779FA07D22F43B12707E40306AE7BC8FC1CB7EEE4B89AEF347AB7E4F8FD82A0F081F94C1F18098AA8BC2CB403506F77A1BCA07ED70DC2558D7D3672E0E8CBCDAA6018CB53E04A7D71F80CAF64A35CBE1E854A0BC0011D74B4204863255A1AE0085B2DE6CB8BD0D24049A48420420B64734571220A280E870390935FDF3D277DD367402F1206924DC42E572EFA4096547D49116F643ABDBB5664986AE0D6E4DEEC7DC51876C4C00BA9384E30E087B228492FE065B01E1BCEC1D5057FBDBEA7040CC80D89E555FEFB87200006AA077DCCF799B946CF793F743653FF7FA24203CD0D38E13151AC0986DC6A16D8D4BEB95CB047B624CF1AA68C51DFEAF5FA1834296CD50D9A15ECEBAAD11B337F1B6FA496DFBF323F9C6954AC52B083A844EC628EB35E2783C6665DCB452A9E41E84BECA7038B96176F684E0D80EADD7D522B98B5B3BB6B257B69AB0CBC47C69500B4AC90F0B3A2D70BCC960009E0F254E8861E84EA04E0F251F589F096094167A5CA53854D99D110D98EBC4A1AA5912EF4483AF44B87E126FF5B3DAEAA73692AE875B7AF02620EA105D6B93ABF465ABD4B2B5DD225A01094CE0CE6227159987F54C483EB51AA06F4AA28FC010D3EAE1943E96A3F1A6D3FA2A85D6DFA0BE1AAEA6AC5C2ED9E0E0208010914B5CF4D12F6525FF9A8E662EB95062ADCF576844FA3A82DE16125903F662A0DAD671AF9E0EC570EFA82F8F21DBC5C5DBC3012872A6421630BEC01A10F576913A226A7854CC39ED327636BC3997C083522DB02DE742C0CAE150E9120A82F200754C4A40D7B640A3AF3A04AAEA557CA376B4ADB65CCF738FAE0FB2EEC56B8BC6BD4D4D7148DAF558B6C8FE3EEA770FF55CE4A61AD7291E0B9044B5518FFA77907ADB81CC9138E9D3408027EF53FBFAC494DB6BE8DEA2DE9191110B37B5B45EDCDBD43709351CFC76D0D38B8B0980919711B0165DF4CC36A82B3B403E145DA34D9A8763149BE885CD5DECC92E1C508108356C8ADE13F1D961AE5567748B163C8B935D97F488A56D11440476C2D38E3C3A95B24BEAA57283ED0694506C24352260A4049403B0827AF46840757B3BD0A7DF9ED7CFBEB7457D7D405C02217E03349B8CA78683B2DB68D966B566FBD88322CE4B4029AF2E50B1DCE381F0E14C9F1D8E9EC42804D09752F50EA380D0A708650E044603F0676767CB68A1841F101855A44568098854326E4908288DA467137AFDB78B9DB82C216A846DDF57A94EE8BF5C3DA2778E173A4D38AA6E550142AEC3180A291B04965506A7767577EF56DFF65AD911000E9C3888B392C4166D8AF8EEDD518AF0011838176CEB953481A0C7047E4E7591558FEA3D9CFB6DF9393F1F1863B5DDC036B45AD80D44AA898750E2EC1681F22E6329322E9FB7822EEE16C32461881A280CFEF03FC87D1BC6C534A06DD135194F38B30B90BA7FB108DA3B200D9B9D99F9FBF0D8E898D5208E00294BCF2129112129B5051AD226FA445E5F2A2E4B8B1AD7B054EA984A8C3D23D24071B2194CC44ADD96EDD44AB65FAF5A19A951FC127C9A4F5C8FDE1733D061779FA94E59FBAAB94DBD0E865FAB2C6207A0B8C340D6310782ACA59D7D30FA1213B8E66202CEAB0DEA5129DBA37EAA1DDDEB412E39E07B71A1BCA9903F1DDDA9D45053100D7A68E1B52F298D052A575F8808318E90D3805B450721ACFF6CCB699187AA6DA92A7D482D110B001108FA9AAABE3FACD7ACB303437F1E1E1A1E6ACBFFD5097D25479F9E90CB2830A27087471E7446394C25011708757104F87BE30C8BE32E9E0AEB39A62999309CD0E0FE9D7CC17601BB00D855A4A7898EEEC6535421DB4587A4437B36CC5513C75C6D706F5352017DF40EA2966C2A2AD61BBB354921BD53D9A0546EAAE49A061DD182BD404AD0F710ABAE0CE1C410FA6E6330E1275082CC287AE07023CCE4AF1E00DC4EA96A7B5A99891AD6FBEF7A2A0861C00B40B8D49F6C4297804E6455A43A048640E07C144D118331E30774C56ED2872E12124242E444E87B92A1586C297CEDB5D714F244889552C503A63ADB23A8AB542A6E834303FE7E4DB4CB501B0437F8DD614619D25BF8640EFB3CA102323DE3A715B70932A26EB0D0700AA4E29961089AB29D6AC756CA4DDB6813D5270938D1CF5DB643E880702F07008B63211C59AFC21520E62EDB3B902E477B79289787E27BE41DA8BF4D5FB629B7EA1010D1D84784724DBD60CABD809CE7FE3C9D2D429832AAB78CCAAC9015B0C8230B96EFB460063D688AD91AFDBEC070972BA85C44ACD1C6924A7489CAC3D429699054F8F71E19B6CF8D3930DC2846F5495600C6730A97F3966C166C2054B1B16CD466A6872D02C19AA53D6BE7B6AC53CA43DFAE1D3B7E122F6BA4169E9D99BDD4969AA122FF5A27760407D93299B425F50A1AA88618440CAE8D8A13A4A4E59ED0C9BE9BE2530F74324CC507896DC98C84B6D5819BB9566F1FE520D82E04DB46E95700B20D921DC45F0F77FA73227A37DC97D590BB78257A4954C6B5118A2115300DB94AD36E37B8455317F4DAA523B0258174B8CAA24B0A5EB5DD45856AAE2DA59732B369404152A4DC91644B634BE98BFA948398FB5C5F960DA16DF7C1911027767FBC941E44D38E144618E02529AE9EC8AEE6A15BA855B568BB66717A349088E221E369A1F622FAF040294727EB96DBDF93416F34BBCD9DF0D4CCF47D1DA90A4028178BE43C22D4B491D1111B1E1DA6AF7496F334CB7FE97E414F6394C1348BB4943AE157D0D35E4972CD469677247B203B205B554135E62B0D778977E0AC7D885264BC55A4B145732D4AB713488C9AC6A9B14E9D3AEA8837448037BD1F4150CAB608A3E3D4156C730503D6754AF1501CB6806B71B7C4C4B2279A87EA26519909E20BDA9314AD13D4AE144BB8EC7584A1493D48027622F85081D4941453FF5F304ECF0C54DF836CCBDA8B3674BA0BD85D5745F2C7144AC42D9BCD62439218F504E6A1EEF19DECC8E6E6A69D3E7D7A23994CE6C28552E923189BB63E32AC2F14940A39D06D5B9620454BE607B303EE6AEB6B6CFAF857978E62116954198018B3D48E3AA3A8DE3BD803A59F7D7A463A9741F9FB7B1C9371DDAC54ED62A5642B00B489CAD8672C45EEADC2A5E27281A278A2A3B77DF4C64C1749E11A7777A95973682E01BE1F4845E016738DDF2F434EC945AD96FA485DD81CBD0BDF06F806AA26471D1BA8E26588B35A2922B95557670DC6288742B3120242064363717BC306DD08B67D1F5527AF0C92B80D8583BA6D6805CDDAD42506D197B94771A014FC49BAF6F7F63C52EF7FB37E7171F1DCCCCC4C05BB9BFA6C2A9D5AAEA0EF3A4D628662C13F282CE3A3C5C0A323C3482BDCA98F49C239B0293D11287449EA4B4030D0AE4441A9571C2E7D924D035330284E433DE9597C950E170078978EED22A1F2F5F76AB201A8216EF1081F8D118A635B12E424F7506105134A386B1518A0865AA9B2AF9732754C2FFBCB71A885A35EEA8DDAB09EE52A0E02889AEEA7EB6508B89EABD946BE44F45DB1026D37B9B7A3B566622C54B533187D0F4050E6460743D028F3D78F239DD0C0692291C6310AE8042832DE403B349C0510B40E52D2C02CECEFEE00224E0EDBA3A3A3EDA1A1A1BBC7C6C61AE19B6EBA69B5566B7CA6522E77EA7AC4582AD8FECE1695B56C6830E31F87543CD2D1A7229A6574A2408115401DE80F00D1CCF065147AA977489F98E8708FFC1A5DA6A188C3F441999008004855AACB551BB6552C43A48A6D95DBB68D13B08BA1DEA7B90237295A2E43DB12FD290268895C840E251DA3D439A93E3F8F549691CA12042DD0A61C819D1A6AA9D4B095FD8AADEC96085CEB1879C8859DD04A1A943BFD0DA4C1D502F538D925121AB3F4338783431C53F61DAE733AB00D20B2B921EC86B5047FA0BAA4AEB283591BC90C5A0DD3A04F9E073F0E503524A304209F0A8542ADF0F1E3C7CBA74E9FBCABD568D4F676B79084AAD530366D44771C1B3201B213C30318AD1A061DFE6C5560DF128DC1C6E208E904FA1AEA302806A7E473621A900F44A4EF759C52DCA56968570778235A44A6D963398E8A3FA0194402881AE090D7CB0DBB04112F169A769EBC4EB31BCAF08597641D5B81ED2F55DAA89E962D975BEC53922F969A76AED0B025EA58436FEFA21A0BB45352D7340FE55940D017A94624071DA54104001C105E48684CC24B33C648BC0EBB3729C787739451E9EB7AD9228011437976EB251BC0611883B1672627FCABDA65A4438F72653F72B99CA64CD6E7E6E6EE876692336B7FD72DAF5E2A150B75D9881AAAAB84D5D7F7D0F5D5E731C09818CAD800FD0EB70508B14A2390147D8EDB75269E530830DC0F271F44A81A8873D521AE23691C9E39171298BA57D76B6A5F310A590FBCA472645372D4B3CD35DB0C7813C6F30C20FDED0DDF0F79DE2268F1CCB65CE11DEAC797B13C5D2851670D15A6EFAEB4F1DEF432925684E001047D10310586FAA2FEF5FBAFD4EBBF40008A60BA84B1EA331B6D98121F82B11059C1A8E1468512C66D14B9AD69E363599B1C1FB1113CBB064E536977DBBFF9A82F9A22215DECC7A717161696836642A1EE739FFBCD5BB3B35375D98FFCFE8EED6CAD596E7713892BD9ECC4A88D03CA4806371420C2D89210521496EAC2E6106AD3118102EFBB4BC400189CC0700EEA276D1FDAF781716990C56DE28D206BEAC5DFDDEBE52A8C51C3A6694EA9445B45DAF0180382E469AB401D1508A6EBFC5A2F037B5283F875B6EB1C6B28539F1622F8547A405A6FDF33FC89B97C443F0F92F73F60387779354E1F374868ECF23600A64DE810117D50EFCDE2AE8DA46336030DE7A72651FDA858CC41014D24D350C591424A6A2F7CE10BFF99DAFD2B41A28096A0DCF3AA57DE72E7F9B367DABAB04164AA8F05B30320C3B6383D62D323831605F56E350F30A05F0718197990EE1030E9832D9AF7E8D24901A34E070383B3FA03F441059B9E9C1139E0C80559F45062D86E5CE51549C2F43CC525EC326EEEADF9CA0FAEEEC5AAAE719485AF63CCB64FED2A1FD4C5391D5752B3B4AF1908AC40C028EA52703648DAEF6597FA9EAAEE3A1864C6EF6397C784A726C635D109508606E2363F3D6653D051F4CA6DAD5B15DA16C97AEC313D3DFD8553A74E7D18C1F0A1ABCB724B375FF06DDFF65F27C6C72F2A0E6942ECFCDE8E9572BB96C233599C9BB463F3534849DC3A351A432FE22AD00174451D170F30B49222E01632DC13A8000DA0371835A792A23F3879327A5357D9DF4462FF30B1FAD70575E8041BF48790C24BA9FD086558CF6B7B1707900A2AB2C265EDAB8E5EFB9E495E2FC9BD29DD75D07E7061AFBA4766984C73578116201F80419684F88A12D43A80746BFA4663D42647076D7E66C2D20483FA8E7C01096960A3F56DC773E7CE555EF08217FC24AE305E54901C10A52B8F1D7BF87BFFFD777F686D65A553C8E76C775B1F08DEC2A6146C7A62DC8E1E99B5237353788F74426E1D007465D81597E02D786C2211EE7716E289D394FB49743E38C68E7B5CE80811A1FF34AD4F3911A07F6D8C0BA354AD08D9E76B30969A420CEB1BBEBD6D6272CE357C5BE73D773846A0241A87D41859F5F509AC13817BCBADDA467FAA1F4AFD6BFCFADE7EA05EE99B8F910EF5551663F7C81C3A686E4ACC2AAF7462346B735313363B3D4E032D4CC106266197D0226F5B1B6B76EDB5D77E9838E45FBCC15E3A0044B6E4FA1B9EF1E7C562A55EC86378F2B8BFDB5B6EDC07F1126626C6A87CCCD2B2BBD89148B346895BA7392E4AE712176534331D8E20219A4A9196965EF641054D0549E3EE67A57EE9891D0844F8C6FDDCCCBE3FE7D6B688413B5D082D3D1ECC1C909D010E55AAED837D7580D2894D5DFC75947C5BA5B25250BA34F64823C0FCDAC0FB08DA1328E8C550134FA7998006218BA1CE43CDA2C5E4F840932861C3C4D080CD8D0FDB149EAA547C7E0F7AE23415CB655BBAB45C7FD6B39EF5DB8FFEA5B8034094505D0FBEECE52FBB4FAFF176EBC401AB1BB6BEBC62756C89246401404ECC4E595A513B9547403AA4F998C2BE45044C058E2C730E17B3CB76043D1B775034CF45BB6C07D32D410E884BC3706EE09D05440F8804C1450008D109539FFFE2109CE89E5AE0042842F6690BEEE867EDEB5CE024C863E23AD8BCAB456EAACFA580BABD8DA02D5FAC471011A50F9AAF132061EED77A67395D5D24B16D757C951AE148D51AE58AB571AD23B598856B698BE07287F6972D5CDEB076711BC35EB423935376E59145BBF6D8A2650174EBC219DB452AF6F7F76D632F67D75EFFB407883DCED38947A44700F2A217BDA8383838F4B666B3B9BDB3B909A81554D716D2820B1C0FDBF1C5793B79640EE4D316264E0981BA7B5B44A561714EF03003AD2283C7C0295BDA46B4F5644C5CAD271CCA01E7D1684F6C9C777BDB4A812A09B2CE06FA5DDBBA2538F69572701D84EF490394EE95BD63BDC8BB9F68C1DB08FA487F392FD73CF0A82861D2AE22D81A4E4CA569AD4ACD3A02A7540488BD403A884106E2313B0E9D8ECFCFDBB1D959DB595BB68DD565ABA06D76B6372D5FC8354E5D71EA03AF78C52BF67B4D1FA44700224BFF7DDFF77D77D42AF5DBB737375A390CBBF4DDF9730F5B6E67DB4E1E3D628B48C81CEA2BA3F59A785B1E2C52369112D3725275BA86FB07386DB65B18BD36F183227501E24490B1758E3D448D5EF2A98843C028695F57CA0C7C55E9B1CD789D6E5638A94CAF35E9E0A5E6863BD8204D50FA8A1B09A71EB2F883194A78B02BCFB2560D82640582D82F19F408366E6264C08E2DCC6073A7D1E6659F8ED2EC47057BAC9FCE884723679FF34D37FDCDE9D3A7A9E991E91180285D75D555C51FFDD1D7FDDE2E8854F2FBD8910D3C834DDBDB5CA503653BBD386BD79C5CB4C5C95154D31E406C232DB8D0346655549616B6D6188838A88AFA1237B9F70500D2F53E03C720197A503E32F5C13820582F3F1AA4279A5C4A5C127AD9DBEC2104F8BEE7F5F7E44E4C829AF4E94AF555765164D3E45A0315D68C925169789651D474A8510AC65FCB438F7D1B4BC7ED5AE873E5B179ECC7906DAE2ED9EEE69AFF2C46BD52B207EEBBA775F59557DEF6DDB7DC72BFBAF0E82425FB9874FBEDB76F9F3B77F6595FBCFBAEAB868687702AE01B3A3F363A6A839941FFF6A07EC9667B6FD74AD897B0BBA2093CA674303AE9218F198437AA83A0CE935C9640AD130EE81A1D97D9D776708B1F5772C2A80C76839397779F4CEA5773B84A174ED931959E39CA183D39D38881287C3A99126188602623305AA88456C030872170B794B76E79D342F9551B1F88DAD52716ECA61BAEB113D3133070D196CF3C684B671EB01CAAEAFC9987F403000FBCF1BFBDF10D47178E3EEE87307B947A64C2B857DFF8C69FFC334D2CAE5D5AB202AA6B7DF9829D7FE05E8BE34A4E0DA5ECEAA3B376EDF1791B8ED3FFE28E851408795C42EFB524120E52369514219F19A672196F98CE3D300E39919483A603427D0D52D046CFC3EAB52900827D8171B8C43D4055797675C54104268C2A0E0348B84C240E18A1F21E8ECC2EE5AEA5713CAE383269571D9DB30524238A3DB900BD763756FC679214D73DFCC0038D37BEE1A7DFFABCE73C6F49CD3E5E7A5C409416168E7EE8ADBFF44B5F58B97411535124F0CCA3B6D600E8AC65E321BB02406EB8E2281D98B10C9D69EFAD5977670D8F6BCF06D0BDB15AC53ABB3B16934D291330499DB1AD47C05AB2A90546F2E9DD753D14BFF423FD8030FA4372E908B22F2C789CEC6FD1F6B6E589F5B3E290AE7E7F0ADD7D60C7C414AA569E982614C528CA5A1951A71F00D0C51E7ABF31DC060811F5BF50B668159796B1B5F6B72DDEC0CD6DE42CBCB76A4789C64FCC4FDBE2F4A82DCE4CDA3EB4DAC698E7708AF4D8E2FCD987BBFFF54D6FBC7DF1F8F13FA15FBD813D363DAECA52927FFCFDFFD70FFCCDFEFEF68B1F7CE8A1896432A5215A341C35FD3EEDE4D4A40D0CEA0303352BE15BEBD7D49AF8E6B15804835EC398C1459158F07007C2BAA7E2440EB8D0A756A4AE4464EAF57F9CF2456DFCD345FACB95FEDFB7748183F478F9700AF67D72906DAF57BD27F7177607415E50FAC20DCDBF018A3B6415A45CB6501E5589126022658C7105BD55CC59088F2A52DBB71A40841B799B9F1AB2E7DE7885DD70D5296CC7A21577376CF9EC43B671E9024CBCE1AA6A7E76FACC736E7AEE8F7DFBB77FFB8A77EE4BA42F0988D2BBDFFDEE62A35D5BBEF3CE7FFE96BDFDFD21ADAC93F7242ED693AF4C76D09F7D6BEAA206171473250723150980D138A3E1188C0F208A64C58D249FAAD6E0DD865092B556D7D7EBB2A77DEEE07C4058BFC20F06047EBC2C62F701500AB6D506F53AC8813A909A0CC0E865E8AD850B2E1D8C4D3F66DC2DCA39A18315010238D88CB096D32ADEAAEC5B9C00D0CADB0483C41B8071DDC9057BC6D527ED14B15A1E2768F9DC195B7AE801DB5CB96439B4C4DACA72FE55AF78E5CFFEC88F7DEBC76EBDF5ED54FAA5D39705441FFBFDD007FEE4622A9B39F2477FF0FE1B53C954643033600DD9055484DE221DD28F336692BEF8AD52D083977D068ACA8A04CF38B42C538F60B5E447CD85F48E85C8AEC00E0AC9DFF7C85744E58A008C3E4183634A7D6CA0EF574CBAF73220CA6CF35F6DE8998DE6D9BC4D81A0F8428BB750511DD9BC3AC708F4C2FAEA33AACA505121E2B170B56C21BCA80E815F2BBF652124647E7CC06EB8E684DD08182717A601B566AB4B1700E4ACAD637BF58B6E9FFBCCA7BBAF7ED52BFFE87B5FF39AFF313E7E55C53BF565D29705440950DAEF7FDFFFFA627664E8F977DC7EC75C32910CE9179DF576AE1ECDA687876D68949CCDFADC569380719798A55295CA4AE0EDC29D6181224AC6C85261342B9A683A027BE3C19788A88322A4B2139E92E49B22ACFE0442F59824C2F7303800C313318F03D10BF0F45312C14C2D59812C6074B478021BD1AD91918C701D5BA4652C556C08AAD850C921EC6804F5D4DA5BB76E7EDBA6C6D276CDA92376DD5527ECCAE347A8A76C97CE9FB3ADD515DB58BEE4D3EB9FFFCC67BA4FBFF1C6BBDEFE8EB7BF6E66666627E8D0974F5F1110A55FFCC55F2CDEF1D7B7FFF3F6CECED3EEBEEB9E05061C9284E8BD077179329DB29999291BCA644CEF968BE92B0CB084A82B28D417EAA4B19CBE2A01C79F2E2A40EC4948008493DE937FB48652CC2D6207385097A641FA91F7A1AC5B2509FDC83E10294D83704CCE823B0CDCE740A8A42E37DE1CC75E285E72BB81E361158C3E36A3836B6BB8B55656DEB34E61D39256B385E9617BC6F5A7ED5B9E7D832D12FC7561C2BDB5553BFFD0831E916FAFAFD9D9071EE89E3A7EECD33FFBE6FFE775C78E1D3B43679E507A42802821295BBFF15BBFF5C5FBEFB9F7596BEBABD3AD462D94D637A208086348C6D860D6864626FC7BE831C0526852467D55C95A81A148B63F658F55B1484B5130156B2583A25FBD50E20F89506B80250ED6348CB67DB2121A6AED972FEDF187D710DCA37E115FBB0289AC4A35E3DA7B9A291B1181E343A8A0500D10AACADCA2B8A28A9352C3C8D7E903815FA7C4FD85BAA5EB7888484208773ED1C49D2FE229E5D66C22DEB12BE627EC69A716ED39D75F65A7166601B262858D553BF785CFD8DEEA252B6C6FDBF2F9B3363A34F4C00FBEEE075EFFD297BC44BFAE23EE7842E90903A2F43B6F7FFBD60D375C9FFFE007FEE879F56A2523CB9089472DC6E00BF9A2A5B263FEF1B3610C7E428BDE3A0451A8A4FDD58BB8A55D4BC6887221589463299C814E54BF0C0D107A96A00C2D232E2DFC479FBB67A6875D1CF74568ECEB50DF23BA5CF6B65D02C892807E76F543BBFA10BDEC83DE8AAD0130DE6CA7D89B102D21E93DCF2AAC876EBBEB16296E725FCE3AF94D8CFCB64D67A27662660457FFB83DE7BA2B716FC7AD552DDAC587CFD8F9FBEEB6E2FAB2EDAEADA0B6CE6A4549E9BFFDCC9B7EE9BA6BAEF98BA9A92958E28927F1D6934AA8ABE8F2F2F26B6EB9E5969F6D369B73478E1C09C9E39A3F759565E64EDAB12BAEB6D18929CB95CA766679DDEE3BBB645F7CE8826DA0BECA6D0C7A7AC82299AC45281BA98CB5E309D37BF0917432F8511740D377D7BB00AA9586FE82B9D6A9522A8AF76F37A2B2F43451CFB9FD457DB6DD0691FDF12AE0B95B4DEEC85E694D9790F440358887223051B84E9087DD68D76AA82DFDE07D833809F79620B7B1BFEECB3C1388E6F4509AB86B06C9386AA716676C6A6480B86C0FEFE9A2AD5EBA641B17CF5B7573D5E7ABCE9D3BD7F9CDDFFCCDBF78F9CB5FFE03F44B8FF39F547AD2802831F0F0430F3D74D30FFFF00FDFBAB1B1F1FCD3A74F471283233679E24A1B9D9AB6A3C74F52CE583891B1F3ABF8E1EB3B76F799257B503F1486E78222B1F0C0B04587C7AC0318FE7B5200134A024E32E5DF55D72FF2E8839CF8CFC152CF2825840736775DFD1316EA3D81A0FB093AA63F02432AAF6797B48454C2E32A4C0FD3349B00E1630A16F182A20010D6A3D7AA96D1567CC230522B59B45543FAC3363F316C571D5FF02991937313B8F41DCCC98E5DBA70C62E9E3DE3BFF459DAC7B5C5D55D9C9F6BBEFEF5AFFFEC0B5FF8C2D712A03EEE5CD5574A5F15204A0C3E74C71D775CF78E77BCE3D7CF9E3DFBBCD9C563E1C1C959FF4DC3F1C9499B9A9BB323274EFBC7202BD885736B5B76CFD98BF6E0D2AA5D5CDBF08F3387F499F264DAA2FAF620EE74088969C5B4F21C50D21957695A892E09F23553503E11D2DBB048847A2E699194B00D04FAE38008018F7B0004138E70A0D31C104987D602D42C0EC1DBC53CF6032FAA5A7203AE451D5DCE65618299B1619FB1BD1A0FEAF4B139A424639166D9F6B7D6ECC17BEEC6785FF4E0F7D2C5255B5E5AEA2E4C4E6CFFE25B7EE1FDD75D77DD3BD1180F221DEACD934E5F35204A02E5EEBBEF9E7BFBDBDFFEE6F7FFE1075F7DE5F5D7678F9D3C11CE0E0E9BBEFF3B7FFC84CD9F3869C30055228ADFD82FD9D2C6B69D595AB6FB1E3E67AB9BFBA66F6F4513294019B46E2A051802256E2DA4AB83776600E4804842342D229000E6405581892447C4F724C970DB130012CCD802840081D8722C42C40B310089EAD3217850CA51CE6712511BC051B9E2C8821DC16B3C41A0A76990D14CC2725B2B44DF0FD81E067C8518A381346D6DAC23294BB6B0B0B0F43F7EE597DF3031317107DBB5AF160CA57F1520FD2415F6DAD7BEF6DABFF9C8DFBE1E406E2626191B191B0F0D8D8F21290B3636356B538BFA0D8D29DB29966DAF58F15F0E38B782A86FE56D6D63CBF672791C2E781F60F429D566422A0BE2EB455340D147C1F4035D91E4002580B82DA171575D02C73BE292E1EEADDB104D0E4278DC5207466E3A80484AC21CD7828D68BBE1DFDA9A181EB4638B73B6383B6B0B63593BB13067932343562BECDA1ED1F7C6D239BCA7077DB548299FB3CDF555DBDEDA6CDD78E3D33FF38EDF7AC77F1C1E1EFEC2BF06887E7A4A00E9A7919191A1E9E9E957D7EBD59F83536667E6E7FD1344E333B3FE83625347166D7862DAD24323962BD6B127215BDD29D9EADA3AD2B2699B7B7BFE15D472A5EAEB77A5A6F44A4208B5A59FBAD30AC3AE2447AF060808C53C2A957D28920E65011280126D621308EAB4B449F1829675EA538129EECD66533637316E73535336353166B35313FE93AA59EC4404577D97786267533F70BF6A5B2B97AC56CCF9ACEDB9330FE3DE670A37BFEC65EF7EE52B5FF5EB37DE78E3979CBD7DB2E9290544E9A52F7D69E269D75DFD23BFFD8EDFF9EF274F9F8CCE2F1CF18F3E8E61ECC7E7E66D62661E2F6C1280E630DE1378A1492B148BB69BDBB7F5AD4D5BDBDCF6CFA9EEC285793CB51CE7F4690AFC2797128C8D4B8A40D08A7A2F25316A5C718AA443B3C75A050330A176D562C440A944DCB20303FE03F6D9C101530C25102411733333363890B68830C6552F01C0DA45DCD71D2D16AC783CB5BE8C4775E1BC5D387FDE5EFE1DDFBEFB7DAF79CD1B8F1C3BF9C7A74E9D2A3E1592D14F4F39204A9D4B9752FFF0F043FFF8A69F79D3B55BDB5BB16327B02B23233602285A2A1A23783C857B3C3871D412D9091B1A1AB6442AE91F6EDE0398BD5CC1F67339DBDD4735ECEEB83AD3548CDE07D946C5698E4CEF9B684143EFA50180205E4160F47D15FDDE6D0C3739168E42E8844D8C672D8307370C1813A3A3368A7D4BB3BF30330D50315CE9AE554A253CA675DBDBD9B6F2EEA6EDF9EACD1DCBEF6E732E6F85DD9D76B55C6ABCE1BFBCFEDE57BEEA953F3E3A3AF52F0001FA4F6DFA9A00A294CFE7AFD8DA58BEF9A77FFA8DCF78F0C1879E41DC706C64723236393BE75FC88EE35DE9A7923243A3363E3E6163647D78581F86D4C79AFD0B764D7DD9AEEE92522896AC8EA454CB75E23FBDC4A917255B84140D0402471AA9D1D47D12C083375D63A8A63840C76D209BF6972D07D2291B44521231C002840E2EB01E5317A52A77B76C570B10F6F7FDE753F5BECC3600E577776ACD7AF5ECE8F0D09DFFF9C77EE45FBEF365AFF8ABF0A1856D4F75FA9A01A284B18FE672B98177BDEB5D273EFBF94F7FF7DDF7DDF7DA52B936328A5B3C3E3965E363E3841D294B67825F9E19202E19C66D1648D114A06506016FD0BF5BE8BF6B8B84D420A2DEE2F537787BD9172200865ECCD777BAE200AAE91B7D442D70BA8209C6102036F46C1B77B78D41DF5E59B21AAAB1C67EA352F05521052D4828D56D69790575D7DC9E9999FE8D630B73B7BDED6D6F3B8B7DAC3C95EAE9F1D2D71490C3E9939FFCE4209AFD7BFFF6631F7DE3AFFDC66F2E66878742F3B8969A6619C8EAC3F5C422647D37384D249FCA8EFA6F220E0C8FB8E4C839D00B3F03C3599F40F4C94440D0E284C0CB0A86A2D7F33A9A274385E90D5F3DBFA9566A5687E3CB484329B7E7F36B1D6288E2DE1601611949411DEA17AA9192CD4DA2EDE5F5FACB5FF55D5F7CF9777CC75B4BF9DD3B0880BFE2B4F95395BE6E8028296E29954A93A8B367FFFC5BDEF2FC3FFEE30F7D537670F08AC16C7644CF5A243503185E31BC24474F1C5399B4DB9824C1A3081F57000930FA9160BD4FEF1F32F6A792DE82BBBD7AEFA25AABF9FBF7ED46DDDAF50680545D0D35D957D667F65416F6F7BB4871B55C2CEC5DBCB8F4F073FFCD73FFEA57FFE7AF7D74767C76697C7CBCF4B5968847A7AF2B208713E044EEBBEFBE89DFFFFDDF7FCEBDF7DEFB1FA2D1E8BF5B5D5D1DD047BC1289441717BA9DCD663BC96432C2B9885E3ED52F0B54705B63D8088E038ADEEF20124722F43840A0E8454DFFD954B27FBB85EB3B020480F43E9F3EB653D41B4C1871CEE701F46F2E5CB8F07737DF7CF33FBDE10D6F387FD34D37E949C8372C7DC300E92749CDCECECE0C60FCE0DEDEDE0BCE9F3F9FB8EBAEBB3A77DF7DF7D2B973E772CD66F3D4D8D8D8330068B4D96E85F543015162127D20470B1AFA80F8480E00C1B60810E566D3AAA56227B79FDB03902F4E4E4EDE7BEDB5D77688939A67CE9CF9DBABAFBEFA0B30C0BED610043DFAC6A66F3820FD0430C972B93C023806A12ABFF00BBF50FFC4273ED13D79F264E279CF7BDEFF01383FBBBEBE7EECFCF9250C0637A0BEF402A9BED5A2AC8FE8E85563FD10B0E2122DC0004882BDE9DA8D375CFF7100F88DB5B5B57F01F4CAD1A3473BD75C738D7DE7777E67F3EBAD92BE7C32FBFF01F2F77F235B0B6A970000000049454E44AE426082', 'Button - Blue, Round', 'png', 100, 100, 96);
INSERT INTO osae_images VALUES
(132, x'89504E470D0A1A0A0000000D4948445200000064000000650806000000BBBE46F1000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC200000EC20115284A8000004A0849444154785ECDBD09BC65575DE7FB3FF370CF9DE7BAB7E65055092109261A08043F8853E36BBB914678D0C2B3F1092D0D0AB643E3146DD1461F20EDC3E17DBA4504511003326AB710944E4034312143556ABC75E77BCF1DCE3DF37CDEF7F7DFE7DCBA55A9840402F4AAFADFBDF6DE6BAFE13FFFD75E7B9D90FD6F943A9D8EFA13FAC4273E917CE08107C667A6669E93E9CBBCA25EADDD7EE6EC99D17C3E6F5BDB5B562A14ADD168F8338D76CBDAD6B150286CE170C842E130798ED188F5F7F7DBE8E8A8CDEC9FEDCCEEDFBF188D44FEF0C1871EFAD80D274E2C1E3972A47CF3CD373729DBF18AFE3749DF7282408468D56CF6BE2FDF73E0E3777E7CF091471E3A168B25AE4B2613CFEEB43A87C2A1503FF88D66D2198B82E4582C66F1480CA407CF47A3CA77ACD5326B411C41BBDDB14E18E0582A57AC5C2D59ABD9A66CB4128E45E69AD6BE7F736BFBE4D4D4E499E7DFFEDC7F1A4C0E5EF8E11FFE616AF8D6A76F19412044786161E1C85D9FBFEB47FFF66F3FFBE2DC4E6EBCBF2F138BC7427D915024954EA7C27DE93E83AB0D0C5B2A914402C216E13C1689723D4A3E6C51844A8368773A5D405E38B6384A8A9A50AAD16C3A34055CCF554BAD7AB35EE77E796B67672B128EDDF5EC67DFF8FF3DF796E73EF4E217BFB816F4F05B93BED904097DF8C31F0E3FF7B9CF9DFECCA73EF5DA3BEFFCE82B63D1E8C1B1D1D178229108251351CBA492964AA62C118F5B44044015E9188F0644D0F5782C6E0924258674201216964450B90821F5A5D42348ABD9B27A0B82906F3439EFB46CB352B07CA56CF546D3AAF5BAED140ACD4AA5BAB1BD93BFE7865B6EF9E88BBFFF07FE293335B5F4BC13274AA8B4B657F84D4ADF34825CBC7871F8931FFBD8B7AFAEADFDE03F7CF14BDF9B49F71D9A189F8865D269EB4BA52D9D8208B1B00DA4A2964C245C1AE21060A02F63FD997E2744B44B1C49492FDF8120BB5640A3E9EAB21684A9D66A1004A47314D4802604C91673561391909E5AA3653BA5224429591D75B7BC9E6D6EE78B1BC7AF7BE6C9C3478E7CE4D0E1C3FFE3877FF0072F40986F8A4AFB861344AAE93DEF79CFF3EFB9E79E37C3ED2FA8944A99E1CC406C7C742C34D43FE09C9E49F5419494F527C236DA17B7B424C42520EAAA2A49BE56AD39F2DB70751BAE6FC1ED6DCE110AEB401119F5901B750D897C14C3E3F685F32EA17AF95CB9649546DDF2C5A295A8B784946C170B56A1DE1244DBCC172C9BCB59BDD9AA0D0E0D3FD86C35FFF80DAF7FFD87700236BFD14E80BAFA0D4BEF7DEF7B93777EE2AF5E5DCA177E66667ADFD14434169A1E9B70CE4FA2762647466D6C78D452F184136404E9E8EBD49CE3C3A89C3686B851AB5BA35EB39DAD1DCE1B561797573972AD09711A2A87010F87232E5561EC4A887C241AB62475C6A4DE52094B41F414EA3012C3FB1A18B2483C6639BCB68D9D1DAB8A10D4BD9EDBB11D9C8062AD6A65EADE2E707F3B673BF9FCF6C0D0E0DFBDEA55AF7ADFF77DDFF7FD3544C10FF9C6A46F0841EEBAEBAEE8C0E8E875BFF80B6FBD03B6FEEEC9B1F1FEF1B1311B80F393A89B0393FB6C3093B141210955948020194942A761D5AD75DBD9DCB202C8AA16CB56828BABE5A2D52B55EBA05E9A4EA0009A20B129296114114900758720888813C1234BA00A314E102265E9BE3E082482C4AC7F6CC486C6C70CCF81736C1565762A155B5C5FB71AF6A880D46CE477909EAAC356BE6873CB4B2DD45EFEBBBEE7BB4FBDE0452FFA398CD2976EB9E596C0F77E1AD3D34A10B9B0BFF18E775C7376EEFCCB1E3D75F28DFB27C6C7A6C7C74363C3439601E90320641C3535467C3004B286B10F71544C59F14516426C64AD90DBB25AB96C859D3C52B16D25B8B601C782790B09F91C3BD8854EE0E75A3C846488184895ACAFC0BD2C75083516EB1244C8D7B193C43B1B1FB14886F687868953C66D0466C90C0E41D5A809C3957AC3B248CEDAF6A6E58979F2107FB32209DAB2B373176C7AFF81FC2DB77EC73B668F1DFBAF2F79E10B736AEAE94A4F1B414042FCDDBFFBEE1FFFDBBB3EFF9A7024F2ACE9C9F1C4447F06CE4FD81046797C68D0F611A44D0C0C5A0C838ACEB13ABA7A7365D53656576C8780AF0C57E63916B92EC9688398B81BF788235EF9049C2F9B12C73E48F58928A13D5E16D6C489D2222F2F4A1E561DC9AAA93D00336F7108124E229530C6C8C8B88D4F4EDA104419E198E45A62A0DFC2486CAE52B215186515A6582B545C8A6A18FE0B4B0BD4D3290D8E8CBCFBA5FFE207DE8EAB9C77243C0DE969218888F1B3BFF0F36F7EE89FBFF273FDFD99A103FB0F8406D0DBE34880136262C22543EA2A8CEECFAF672DBFB161EBF3F31064C50A5B5B96CB6D5B15A9E834EB4E0019F30C1C9D4926398FF9B34424986B8241BA2DDB8D72724FAB97E4F28A202DEEE92822290EA942581144D020A6D9C23654E51C607F12A8CD61D4D730C4189C9CB0D4D0900D4E4DDAD8EC3E089341623A96AF35EDC26AD656E873BE5A4662AAB6B0BE66CB6BEBA54AB3F6C1DFBAE36DFFF139CF79CED34294AF9B20EF78C73B520F9D7EE4F52BF38BBF383A343472706A9FC97B9A9D4015C412363B356E53A363047066DBABABB6B9B464CB172E58657BDB3697976D75E1A2ABAC241C3F8C9E1F02E40A6738E204B871378C7B4ABA9EF622F418DFC93D26A92A057A6D11C063101122200642E3C4907D81061E34CA15D68566AD61C552D976F0AC8AA847717D03DB333CB3CFFA20CEC0E4380499B1518832443E9E19B4CD52DD36F0BC96B7366C31BB661BC5BCD5A8F3D173E75BEB9B9B77BEFC253FF4936F7EF39B57BF5E2FECEB22C88FFFCA8FA72FDCBFF01FC687467E261D8D8D8D2011FB8646504DA881810123E2839B4186A46273D356E6E66CFDE2BC651716AD0A97D6918828C8184CA76C085536881AE983083E4512261014D21974088CC6901011D5254436C3D10EE2B91048440022844B072224E3DEE2DC8921C249B5918FB402A9AAE11414F0AA360B3B962DA02645C024AA7070D0FA21C4F881199B3A70C086A7A76D6862D69AB4B3592AD85A3E674B5B9BB688C46CF1DC663E5FDBDAC97FECF8896BDEF9CEB7BFF35E88F235C72C62BAAF29C993FA93F7FDD9EB0733FDFF09B5347608CF691A713F3C316527660FDA0174729AC16FAF2ED9C533A76DFEF4699B3B75D22E9C7C0495B566710CF314EAECF0E494DB16494706A42741601A6E4D4B6DC966808404084E428438F938E88E210B513FE2EE82E408C88E4234418436A39C47406E180720ACEB9C13D3F36CC812102AA2E7B8465809E16927198721FA2D89C727D73A8F33B18341AF960816F1F06AF9923388ECE118C63F05E3B4E97F947EA650A910005EE91C3F77E1C2F3FFF1BEFBED4B77DFFD955FFDD55FFD9A88F23511E4C31FFE70E4CD3FFBD33F71CDE12377ECDF37353A3D3262E3A899FD88FB5108339E667020E6ECC9076D1E62CC0117CF3C6AABF3172DC2808FEC9BB6C9A1011B62309918B10731417F226EFD202423A30D21A2203A24E34F79D99504BA2A4C94ED201D24844B72E84F5C00216302902CC44BAD493A9D884E4C880B3112D45D6B51A708435B9A2B8B12B344204C3A9DB4119864A02F8DA757B2ED6CD6CADB3B56C1D3AA57EB2EAD3184ABCF556ACA9F9194D6D000D1782C128F27262E5EBCF0DCFB1EBCBFF8C52FDC732F4491167D4A89EE3EB5F49AD7BC26393777EEDFCFEC9FB963B0BF7F601663384A070F60270E8E62BC13692B66B76C0DE4AF2C5EB0ADF515BB70E68C73DD146EE60CD230C280C5F149EA13C2B014CEB5718810C36B924A72440111A91ECEAB2028505F605D4979C0F0A0DCCBEA8E24C0006822CA8FCBCDD53508D7A680CF6FA1E29BE904FA3F5061324078851E8F9431FA5538BF8664151A8A3FF2B6BEB1693BC43ED3CF386ED3070FDA08067FF6D8336CECD0012B52C77AB164679697EC1CF6B0C2F9FCF28A6D6C6DCF1D3D7ACDEB6EBFFDF6CF3ED559E4A7449037BEF18D89D30F3FFCFA81FEFEB78E8D8D4CCCA25B27652B30E0928C0CAE6925BB6917A59E4E231117CF5A6E2B6B2D0634353C82A19FB00149016E6B42DCC600C4D9523F0AEC449028049115964AD03C541B29E96067149788204E08CA0784E1398E11A7821490E3D7F332E0228022F5A8824E4DC7A0723A44EAF5FEA4D5218CA6E79D28AAB1A3E996183148DD8A04831E8FD0760E177C1DC2EC341AD63F328A919FB0090873F099CFB4A9A347AC82E42E13B33CBAB8E876681B35776161A1536D36EE7FD60DCFBE0397F8334F25807CD22AEBF4E976E2431FFAFD1F8D45A33FBF7F726AEAC0F474682491B2E3B3B376105FBE3F14B5FCEA9A2D3C2A629CC6785FB44DA46404B5746C76BFCDA2D692207A10CECD807CD997348810E8BA24230C225AC41F2D3CA056A96275F27506592F159126D4900826248B083C2F48408118048992176165F8637E2F98050EA1DADA78550DBCA90A8E44915847D32F6D82CD38163F415F121050CF856473D48E5418889673D12787637080C0BC895DD9B6E24E0E69ADF8547E0D224D8C4FE23AA750777D3E6B50A3DE68241AC2D59E3E7BFEFCEDFBF6CDF47FE4231FF932EAEB4911E5491164737373FFBBDE75C77F9C5F58F8A9833333D313C323A1098CDBD1A9693B3A310D7243B6B5B060CBE72F602BCED8DCD9B3C4174B106AC4CB0CC1A1113A3F08070E11B1C741920891419FC7C8378B652BA3D22AC4238D42C9DAE52A08ABA18E1A707FCBED89549B8E1188D73B0A64F06523A2DC0FDC6290CBB9D49CAB3E075DD33D49235244BD1D6C42A75C27406D581844C6BC4C10D784655BB06B3A76206C8B76923096DEC36C6C642DB79DE33E968BF2756C5C465E19F850F9167535212CD4448D8607707E6E3E74E850EDBDEF7DEF3F3C199BF25509323737377DE7473FFA7B9FFEEBCFBCFA19C78F0FCE4E4D418C617BC6BE59908B114CA66D7B71C9E6908C790821621410E1D14CC6AE955F8FE193E7344200368A5B1C85D364F0A3E8FE3684A8EF14F062900A88207525EF2A061265AC055171BF902E371809F06B920421977209EA1732656BA232D4720894C7038AA31A35552F42046A4E517DDBFA083413D41B26BE81CD213E517CB5A6096137F61E75F23CE20252798EBC668EFBF004752C216D65A276D9AD26634BF5F55B14822553691A92A885DC1E95217C281C8E9F3D7B66FFCCCCCCDD7FF8877FB80A5184D6C74D4F48107CF6F0C73EF189977EFAE39FFC89E9C989F4343A749C78611697766A78C8DDD61D2461E9EC195B444D2D9E3D6D95CD0D9B86639E79F080F5636265568730FA03705C046F2446278D01B5A48A205C93A33CAF945C48C5200C32DA456A4C481581BA48954F235B13C556451CC26E031467F83F8E8ABEFD1CE0CF2EC8C284BB12D0A10F9214971ECE433CD3C1F32AA31A5BEAA3668AE170B529173B0933C43A113CAF0A5ED8001E56C6F2A8AE1C52ADD70155BC30217282B86B0046D4E48D6C91DEEBEB35C1CAF2CAF0FADA7A1342DDFDFEF7BFFF09DF483E21410EDD74D3E0DD9FFBDC6F96F33BC7F68F8D8586E182A333B376084F632493B632C1DE45628BB9930FDB226E6D07E41E1C1BB12318EF117CF58118F601A47A0C81BB6AD8889ADE33E4765019155757EE65C189F2B25026D2028E3C2E7BEA1D95149D3BE89FE741A673B2A6DC03D8CDFB7D6756575F0211337803D9BDE73ED9A598C4DD8236CE04C86C377065C9272927298C8761289596DB4D9B7A7DAC72B9F52C2A8F72304D02177E18264D61771AB81555882B47A4D16C861716164F5C77FD8DC5D7BFFEC7BFFCBEF7BDEF7155172378FC54CFE5BEFFD4C9476E9D181B0F8DA02387FB076C6278D8269080165CBE78EE9C9D7FF4949D83283982BDE9D1113BBC6FC667766564150B0CE1DDA4511DCD4AD50AB21370933C24717BA01D02E4F490B68BF42780EE838E6D3FDF0357967D2C303011A45B47AF5D48E853359AC28982F45AB1606518A74DBFE534A49098419850EA5133D463688A699C99086A76736905CFF20C31D7595B5F5CB0349271707A9F0D212DE3B8F96378989D567BE0CE8FFCC5AF5D7FC30D3F86F4AA17574D8F2B211FFCE00727FFE88FFFFB87662626C7F6136BCC8E8DE3DE4ED8146A2B04676C2C2CDAF9871FB2A533E7AC894B3A05B18EEEA31332E070BE226C79586144B6C2C0F29B5B0CAE667D709147D2B411100570E4F25F88EA226717817BF38F03FC79CCB527026FCB8F9C77EB576AD16FE5C252571870DD6B3256BDAD8CC4F0F31408A2C782B793C43994D3FBFF6DBCAF32D2DFA6FF756C543F5ED9E0C498A1A3AC8226A863A3FA20CE830F3F1CABD66AB7DD78E38D0FBDED6D6F3B873D915AB82C5D9520775DB890FC6FBFF75F7F3DD2EABCE89A83FBC37A7FB10FA9B8067595A113D9F979DCDB476DFEE1935620109AE6FEB5B3076C1862C4F030FAF1FDFBB119313C983CD16E2597277A0E634712EE92CA1DD570A5424410474C97184ADD83A7CBF25D50DA9B7F4A490FB99A0BDAD48CB1522025CA0736C757B570DE620CF52A6E73D77B120122E0A029EF8C67D271AC2412A43563A56AD95F82C5538C13BB390A1357884B44E80201645FA6DF1E3E793235BD6FDFF4D1A3473F074176BCF13DE9312A4BE2F481DF79C7BFDCCCAEBFECF0C1FD917E628D71D4D53E5CD8415CBD0A76630DF77619C928ACAC5A862AF6F50FDA009D8C63E0741C400AC288FAF6D29245F162FAA50A204247530C8CD7BD220D5F4810525CEFC37D80BF86E5BE1FDD165CCAEF85C04E5CCA5F79FFF1E04A3BB37B8FBAC4F17AC71211DFD2EF309E6012E7214DFFDBA8E806718C9C90B8A400359CE6F938849A452DC983ACC378DB4BCBB672EEBC652FCCA13976ECF8FE033609338F1140275165FDD897FFF5BFBEF01DA7CE9EBD494E5380F54BE931173EF9D9CFEEC336FCFBD181818926AEDD487F065D39620789271220756B79D936894A73CB4BF8F1659B51637D4415E42388B78816E6B84DB970437E7E9D18A4893DC1A610C475C8CB86881842480FF87355967F422978C29B8F9FD49EB4B8B7DB3D97FAD4CC0064723BA21762212DAEA360425E1652A07155082C3DA6421DA720A2661E04D3789D8AAD440CC1D6C292196EFD081235839A1F44726688F2D389B8DDFB8FFF98BE78EEDCF7D1B4A6E12E4B97A92C2816FDC8473EFC86B98B73AF211A8F88F272611573F4C345A58D753BF39507ECDC830FDAD6FC824DA6331EA96778761035359C8C5B8880AEB2B36D1D225619379FE863DC7267D57A8C7A3CF8C25FEF11C511DBCDBBD474F3BD733FF638790FA81EE7F43DD7F642AF8ECB0089EC1143FF5C1AC98B209A2CE424B8CEF35124C6A590F23190DE2448D57B7C2D2952CA10775470853505A3F2C9BE9455B857AE947D1E4DAF8D075053315479138FAE00D3B66963BB980F154AA5A9C9C9C94FFCFEEFFFFE8657D64D9749C8E2E2E22DE7CF9F7B7326118F8E64FADC439AC1431885BAB59D2D5BBD38675B2B2BB6B3BA6AE348C5330F1FB201F4711F03E843022218AF6631EF465FE2E986DB110F9067DC80F2819725A4E8A21B493F7E15D8836C21F2F1D499E0AACFEF01FE74DBBE74ECD9B1BD653CAFBEE3DACA45D76CB1E6E0DA680F4DC50C838728F734533D06F2475049210CB982E51C8E4F1E7C25B021937A13895A4B2763363E3E6A5FFAD217672E2E2FFC72BBDDEE13EE7B699720D267274F9EFCE9471F7E646C6C68D8F4B269040918CF0CB88866B107F3E7CFD8FAD2227AB563FB27C66D0C75A6A85B1E5508CE69C1192D3A1322C852E025DB09CE4090388E81395C8E2CCEBA47CF5D764FC09F4BE7415783E4580AB2574B7BEBB82A5CD19622F40E474E1C7AE71EB9AB3E629228E38EE365690EADC9382B04872D0893C6664A1B28B83D3039E56EFF260409D4D739B73F1310649CA05253FB7AEF32343C14FEF33FFFE04B8AC5E22DDE4037ED9590890FFDF9875E9881C2FD88620AA335862B3B88516F95AAB64304BE45ACB1B9BA62A383108C00A8512EFA544604EFA35E2E79271564696A83C8CA455D44703520C230183FED11EA0AA404EAE90A82EDC93F06AE78FEC982EAECC1D5EEB7E9A75CD8367AB645BE8395175914D92B880DA37E3A48450B462DE6B661468825678552C30319D34B2CD94ED9DBD58BF356DCDAB20481E3287666081330A8B7A318F9938F9C4A9E3A73EAC8DEB8C409A20B7FF73F3FFBDA930F3E383833396D49F4DC0084982408CC601B54E10EDED5F6D626EE5ECD06FB2118FA55065A117613774FF3418A727B9378F4DD2D14B691561890A8E084B984F05DC4EC22F672627C3570A9BBCAF527038F2186734A5722741421BA7981A4DDC725758BA4685A472B604498B266A5357B1CE31CE6544028AF4A33D51BA8AC95F9452BE18169ADC130923200D18690963436E7DDBFF3AEFD60889A83E404B11D1BFAC09F7DF087068706A3C3106188B842F35603488A16A85D387FDE2E9E3F67592A9F1C19F68943BD5CD25BBE86BFE62C3961342F24A75CB56B66D4D59090EC2EAD88D41DB89F009EBF023140B730FF833257DEDF855D425E05C17BA08754896640F4A07D67855E19CE7789E1E701F4AE85A37A268851543EC1F8D28ABBA853B3C425BCAF061A42F625455945F2A156C7CA04CDD99565183A6B199C1ED91CB9D17A757C6076C61E7AE8E1DBFFF66FFFA2DFE940A215B347CE3F78F3E933A7A76370BEBC89C9C1119BEA43E7A5076C3DBB6EF36B88DECAA2A5E086FD88DC7422616922D324FAB3B4B6EA7355694D1EFA1830E611CDFBF0AF239F5E8B1334781715068BC033081D3B722D9D58100F70E9E11CDD7619E89A97A716214B0871A34E791970CD2B5D02B54F3C0304753FB6BE30ED3AF8B988AEFAC9BB488B8D827F3C0D82D55ED89A9206B48282C20465DD63241E11C40DEFABB0435CB66C83F47294FA46B46A065C96770A965B5D232EB968D5B50D9B4A6197719606A3B8CB68A1FEBECC0D7FF0071FBAB5A7B6C2CA7CFA939F7A763C9944B50D5A0C0325A3948926882BDA56420276B0157AEB97A4B161A42259C7AB80591ADB3922F3061D428CC54042960622448782A9070DCC9D4A1185EB422C251DB1E23C2120782E28EDDA5AF7B8EE65386FFB51D9E01870B4323AE309B8D63957A77B13E58267D55E505F50A750DD258083773E00EE043D1172029060783D62188E3E4989ABAB77357A57AFC515224C0817D84A44EB78557DD8E03E10AF1596B57CC1AADB3BD6E4D847F951883280291848A4A5C6C662A9C48B979797533469FA682679CF17BE70229D4A253575AC4AF4794012C4E711C36DBD53C686685A5AC67E40EFA9B548004294F0325C358953E9E4AEDBA801EB48E33E40F201F235E63DE51CBAD7FC7AB79E2E3829A9C3A758744F08E972B37334E7FCF1361C6032319A43B7DD5DF798F3ABA935FE38ECBDE66A6ACF7910AB74C712F4CAFBAA7AE3680695D12B820E36348F91D7947C1AE918C1662428532462D78A4CBD86D658644B32E05A31CAE8F048B8DD687DE7C73EF6B1833460E12F7DF64B83D144FC1910242C511C44D40600055D5B1B595B5F5DB602063D44237A27AE068C7C1EE9E8C015EA666F4ECA3B8EA8040B9E7B08A484EE75F3C139A03CD7037DCE91F3DD3A7AA0B2428E1F7B206404E0AAC641DCAF3774DDBCCAF5DA0076FBB2A7DE5E5F76EF3904FDDC0BBA4E66B7AFDE6E37AFFB7ACDAB7E6BDAC5909A523E6F0D05865C1B262CE84F26AC86D1DF5A5FB75C368B675A81E9933E732E9B8BCAB26C367B345F2A5DAFD0237CEFFD77639F3B990415F65350863C8394341B35BCAA0DDB5C5BF357AC0A7CE44B6BBA5953E9E55CCEDF13E80D5CF0B6EE8A01EC76FC726E83BD7CE0BB52D2CB77CF03A204103C2364A86EEA830BA51B3B021959A453AF4A0D23E9A03934CEC3E4F5722878B657CF95F55E01DDF6251D57BB1EE483F105FDA57D1F0CF2C2B9A27A29E90EEABC8EDA6AE3F62A36D17A33CD7E97B6B7AD0013E7606E7968228456D5E8AD2684CC2025B77DFEF39F8F8757B3B9A162A9D8DF87C7A0391CD98F44226A15A89C2FE4AC484561FC6DB97109BAA0B84311AA666C7D6D942444824CC782C1060814A8B3020DC675F76E9E8718C45ED5A0444E9775A90BFCD105BF28024B322080133B42BCC01146A1C316A2DFA1988EF1E088D1EC3DE3401D8ED46EE5BBF96EFDCA7B7FBA497DF47E7A22A3B635269DA93EF556CF882139F6DE74EAF573038234B1BD515C60114401639D73BD63C96537FC7D4A1F9233A885DDC279AA2FB4B8B070EB34293C3C317C4BB5D9184FCA8503C729B84E95964B05A0E4E2A749B4B174DADF8387E100498C5E3A497529181221BCD324792F74750FB2776FA1D7495C7049F27B411941801C212128ACB2BDF252410AD0DA0CAC8D64B4F1F75B891847DC0988619A026780C6180CC2C0762E316D794672FDBA7D098CBAEADC03DDA4EBDE27F58D767AFD0BAE015DDB114635AA2E2FC71135E3EA5D8E852FCAA3D761F0D286289A5219C016CBC01B4E510D5BA2E83E8C3B2C1B33D4A7D5923851E0F6F4A38FDE904EA7FF45B858ADFCC8C0D0E080AB2CB9610C522B0273DB0482F8CEED7AD50619A06671133456226257A39A96D60A432DB3D1E4A1C6A6F7DBEAA456A1CBED0CA64B2803EC2285723A77A268E0BA0E3801559E6B7ECEA94F4E4B0A3420018333BDF442AD46E0BC283A3A8CE89BE68FB8A67B9216112544D930F9B013A8ABDAA8BBC7283D507F7B7D539F64037BAAB7E7954922DC2DEF4A4900C1D87A6E76ABD6F0D530C29D24A2552A5A0866EE83218652C18B3ACD74340815CA104604093EAD88A3BED2A8B2EDBEDCF6F69BC3F3CB8BCF8D27E221CDAFA4881FD288BBF45F9960AF8CBBABE5387A29D51F4F228688A20C569BF1417DE94C31A0663B9534B66020C1C9EE6085681F40F79C7C30D04BF777011F535CADE98BC056C0E5B217D8870E40A084544018FA2BC477BA52226873BD230274EF87E401F18C16C0F5ECCF5EA2E8781981C8AB4FBD17DEBBFDE318B8ED01EC3A0EBDA43754B8C09212C52D513129B6B6829D8D73AD8F3615486B4252D1BB702ADC29B854801885D88A592E9CBF7034BCBABE1E4BC059FA06439F95F531207DD3A70F66B02D1E7DF631E8340FAB324DA6692546980EE81D87DE17F8D74CE20E0FF0D4E9406DEDC60F57260DF02AE02A4B4822EF478881E5730E0FA3A242E27E8E2E050E201EEE93644865857046423D29F1731D7B84116103A25CADED9EF10EBAA7F34BE5B8B29BEF4987AEA98F9E831EEEDCA08A348FE7310A44D207489AF94EC10C5A63D01093A3B2F48598EC8E88A04FBB2529FABAEB81071E08856BE8BBB8448AAAFB93C41F3CDCA8D630EA182108A297542910ACB77D324C511AD55A5C71832A9551D357B17EAE8EFB40E0203ABAD7A371EEEB0EBA07423C99DD730D529F1768178610522124867030C24228880D0BB9F4D5912E4228BF0B525B10A0074E8C802022A21305490FC178CEF557F4C5DB07762557D02DD37356E4327B59C61F305E20253C05536ACA880091BF2288D698E9DD50E08D5257B36E755CDE725178058FD493A24FFE961286D7F7300F7EE541C6CA60B44A4F084D702302C7EB0B577DFED568D6ACE1E2D5C118EDA0276BC1AA0CF5575CA02E201D4167B8462703FDAB81E99CCE77AFF506101C55F8D2C00324043663573A345638C79188FD907484DD2E281F9CF724C525A32B15BB44704290771B02115CDA685BEACFDBBBD47EEF5CC7A0FFC1187A6594EFF55FC4F06BCA715D0972B8CA52542F9CF8125624447679676B139C420F8CBA82EB46B56A5524443569ADB108A2ED42F429F8DCC539EC239D55344C89C09B944986E3B5C0591E84BE05973E6C68315803DF3A46302469908A5247B0255044169D6703A3AED57E22884BC09EB47BD61DA46E3B30129F52D92DAF41AB335C73DD2FA32C84C21A3044C78DBCEC09928B3A6B936FEB9A102F03EEF794E719E5FD59D5438F771B0DC09947D06D5BFDF68E0A8470DDEBF59C831845DD02E3FC01FCC8D8C1897F8BC259D86D6CDBFA68B3AA57BE1047DF3C8A71855B4DDB0753F972166052C618A1AF5A2811D67BE00E888EE8EB5401C6B454CCD9E656D6725436807AD05ABCE63A94CE17ACA629F84A914678A6D3C030213DB92DBFA66F381A8D8AD713958AA1519A137A2F01031038E19D70105DA05190228C581FC7C47022A2787DA198002E0F4308628B10762E8457858C9B111B85F0E5C383785B43FDD6E9EFB3561AE70335D716315CAD75A545B1091A202C907B8CEA6A8359AD2675F906398AFA5B5C7320AFD7AD4D1D199736B5D1DBCE36630E4771719394B7869F6B6A491FFA24A309EC45828030E12B33CBD9350C386A8AB8AD4FAE3A84D02A946AB162B502A600773D8E44B718BB3EEA72EFD4BD02FAA33929CD64BAC7E0A05D7582A52EF2ABA354A6C50A7AD18F1EA3F20042EA2820FBE2ABD1D530A2A9E8D513CF07DC274EA45AB197CE75740EEC96E91D851C2F435F24252AA373978A2E28E8730908D4927B59203D38EA3C094138977481504736DCDAF1A05275ABDD3DA076BD0F97AEF9F96E391107A6A4773139128C533319FA92AA819D957DA0372E095AC755E15A5B4C0E5E647335EF472FC0B8340AF5714D75405F8DB6DB06CF23C1615F194E83F2A5B595850A3AF0B05EBEA8222D44D68A0C3953DACC456B95FCE37D6C4A13F0E59295AA1B2D89A1DE20EA7332B9869A6A17A70939BD79A736880EA6E03977D524CDDBCD0B6952519475E4E95CEA065BE07602C909DC5A8E6E1F7A3604C704698E70D4F72011EE77548F8C38476F5BF6C3EBEE225A692F71F8BF4B88EE7557C1488854A170296992AC6B1178589A9A31872A35023FD987B2552146A55604772DB72742BAF0A5A7DC1315CBA1DA3CCFBD3002E1A3A71D2DCE0BFB574A104517451C1594C1D643423E57AC8621F2EFC1C94B786457E4596959BF8C955EC468C94C7173CBAC0CE708D1EA840F863C20A2B441AE13C7395584D0712FA8ACCA74EF8B41FC598E8A257A9281DA09405202613C0F71384A2D69FAC46D89A44220A2A82EA93DEAEB4949E03C0420E48B59DC600BBAC4E85D979D6A91AF332E34BFBFE36964B72D5AE1BC8AC1C650EB43D65AB5049284374203215C8A0DE695BA96472A5BE34E90F2E0B98777B725825ED8EFE2D425462F2F0324823468509466287E4E1B5EAE031134D5DCD1EB5B38A48C2ED5371EEEF6D1A04FA9F406EB819EF24250573A685BB5767904047439594444F539919CA37B48043C0F282F891282455895DB2D2BD039848008F2D494D70B2627ACEAF2BEA83EFA25E2F408D1EDF32E61740E4865A96F4AFADB2E55ADB0BC6AA11212228605E9EDB6BE7D0F0CB63FAB7FE0AAA1558E3CA39D28742EC2EC06D380EEB963A5BC96F67B21444C6AC9755B8F20E840156B62BCF480A24B8614108622EEE601CD5AD52378CDDF9431FE2DAD8505395E4E224A031A97FE3837EAC491013308C4216A89A3138B8107EA2B28D37BA6CD21F8D420A86B37ED66554E2A51ADAA6EC6B6EB59A9AE80095C2ABA7DE91122E853007B8921CEE6AA1B75E5E59E4A77D7D006B5ED1DFFA44212A2B74B284EB7256122453DD303691331A71C184FE056AEB154BDF0EB765A6DEB5C31873E8A9441F74E0809249F4E06AAEE3F070DF452AF2125F79A247E34AAA540755CB71A9CD3DEDAF606F59593DEAAC5C49172004095FEF5767EA397B4DB250ADCEED3E74214F7D08E94A42D755608750890183C4B4DDD38486EB7CE951C8D5E4667C1782E211982E986AAE45CB65347B5D4E35CE75E2F12A813FF8E84FEA6184304039E677CDB2BCBFE5D4B142DA1590B39346A4935B98B0BA85E215B8BEB8A785DBEE11AC991AF36C19DDAF79DEFC09F9B883A27EAA02A1261544E136C5A6C2C50211FB0F751FF74145282BC46AD8EC8C879A7F02E9AB8CB7554978C5D08E9D1B7205ADBA52902054C8CD8DB73A041215088F373FAE01FE1E888C47A3C2404A955EA577B3DA3E844A06F723EB4398D9FFBF520AFA055EEA8F23EBDA371A8ACDA1103B8748A3BD576A0383585EE1F0971D478C4DD1DD491BE838C4A2D6BF1F846D6C2B5867FB08A85F37EF73013FCA54920402692CDB3C2B3AFA8577B623CEAEF68692D6D049FC135FDF33808820430007DC0A88734666D61A4FDA4C4AD1E23702D2059D09037C8A98228CF0BC988A97FDE4CE332701544BABCBA8C8EAD5A18C2E853843048912BA84148A9F89B46D5205196FE23B9B103695A98D6337CAE429D317AC03D21DA09D1258680FEA3B0DDCDD4F7EDE1168E850801174B7F7B59F54188027AC410177B5F20920CB688A32EE97B35392761B444146FAABEB1613B8B8BD6C4786BC11C95799F03AC80DC3DA0BB427620E91CB9A0B6A4897C892ACCA98D716A6260FAA415F29A1D09977157B5D56A951B15D4578B9EC8D78E25621EDC497AFC33312A74AAD3548F284A3A8A88A21AE46340501C1FBD5ACAD3F925ABC24D8A5B448C4E19779036F4C551B813205DD2B2CBD954E49C4A5BB2575AFFA4E149C28568D513F2234807D9BEA900F5691D71A78E618583C5CD1D8E228E68AC471D45B4A3BE29BFB73D3F0AE42972D4D64F6D794D30903E7D73A91107AF2119CB4B162A16FDA35531441B6F4AE8163E2427B2322D392B3D96A3F1266D78D84199308E8AA6A91204AC11BC360980D60237C0830442EA2B5C429D3440903EBF12E841ED3115852871DC4749889AD5750DC78941437E2E36222F85232ED055472439FF4C0CE35E5C59B1A6DCE15680C856A5E4D3FB0A9CDAE8E38EAE730C385D0482EBA472187480381DB92E227421D4930481FADC057D86A6A33C1E71F6EEF3922418C0EBF1400D55E8C0C87494A7C8D1B70FE439DF240D82C8BB6C32860ECE4A7D7DCD5AD8452D854ACBC651574008AA44B29A4858B0A246C400381773D37A80339ED1066BD23C31024A691FEDB55286997C0B2970303E3E0ECE1844938E8B5AD261426C041FDED516A0F39EC809E55EB9F25D9033248AF43A87F9F1754B292E6ADDAF5CC2E27AD6AA2B6BC428157F0F2F0469F02D10AAEFF45A220CFD3021B35651E0A368D4BF4914D7C3460181DC0620153C1FA82521943E03E26621D63F79567D2232CC66956E7DAA0304AB7E056A427C4B7D100178A697574CA5E056364441EFCEE6B6AFAB6AE3ACC4694733167AD924F3ACA1FBB81D3762568F3E3CEF8E87EEF14744711529F50851F449B53EA19620B8CA02FF4D18E2D0A1433808146A80B83A9CE9DB14F16C2C1CB724BEBDE6665A60B810ED5805D0EE079A1EF74E408D60C98DDCCC4072D4B0F4B2E6C334205D4D13CCC91DCE2DAEE02AE69C61230D0C64BDC91184C269B2610DB50FB2C518EAA4CF368B5820542EB5A441D33632A6611025E9D32AF328448BC96990AB4D3E0AF2A37078442A075B663C6BD8B116AA59AF1234EB20155D47827554DB6EF041B2102D94EAB3354978A858B01A92515C5BF5850A849BAE82B4CFA3AF6484222286C61DD85461C1C9D1E558498C24055EA07C2D0983A712CEE852CD0A33601DDF624A1B131C38B0BF1C3E7ADD7596D7B40775141888F27DC98C8DA7476C7260CC22FDFDB61C69D85618445171536D784F223E3157D7585C54043AD7A6C53403A2C52BE25EED9690A1FEC6CA8615FEF9616BADEF60E82148479C882482004DB0A9EE16845400E7113683D75A62CD17B5F5A60D9FBFB3BD6D6D8C6B07A9EBC0B9B6B5692154896D62ABB86E10DD768AD6291483631E6222991D08D9811134CB1006218A77B487AFB6EE1097B8E61792E96F27BB6E95B367AC3A77C1D2F99C8D6A68204D889754D02DAB524F83A35495AE6B83014188C050A07CBBAD9528C184650533D01C1DB4D4E4840D0D8E5AABD6B27CA16415FA5085D8EBD98DE66DB73DEFFEF0338E1DDFDAC14DD5478BDACC4BFB0D8A8299FE01DFBD40D30F753A24C9A1EBBB064AD80FFC91201F88699731FC5A903C28A473BD1D1804F5D5555418B10A6A40AA288C3BA87707491C89443A81ED6218AAC711867A910117E2BA865CB6469F3C08DCA0036DFA2F631E9C8B00382B922EAEB94AE47949AC34BCD6DF26E8650A64CA158F616CF5024B2AB3B9BE6A790851DC5843826BBE5A5393867B938F9941EA2833AABC8F5994725D21354F77C190F026D0B48B5E05447094D2C3C3306ECBCAF2DC90FCEE9EC2A5C9C9C9BF0E8F8F8D7EA4B093C7CED67D778262158E42CF658687AC6F60D0BF0492919054C360AEC65D5D7155E4088902BB29C877FBE8FDF300A9D75110AC2052C8D557569B172F5A6969D10C8608836445FD5A9181CE42BD04F6036D6BE958FC928D104138EA5C476D8ED672BB00F2515D7A5F2DE2C80B8A7591AD5D1DA2B85C8AA0459076B16411315E982014EE0DA1C66C63D30AF447DF086A6F95147644EB95150769131A473ED04B411E69EEE24283D5B107F2BAA4E261116B4078AD158B13E56BD7073D5BC4B9C9E3754A450BEF93D3D3CD89898907C2F7FDE3BD7F4430B4A6F9167959DB7416FFC7D2434336343E6103FAD601235D038F9844B9E66EA8949C203A72EE9A4CF7B8D00325DD1351E4DE864410BC1DFFA80706E8944BD89625CB9E3E6BB5A5156BE7F2EED168CB8BA4A406507026AF4AAF4305C1F4FF1EF0EB785E02DD936300E83D445BB687B6E47929B61133C815D684AA131515D85E5EB6C2F973B6FAE8A920E083B0922095EF4D9BFBA2DB6EF2F176FFA92E8D5B401608FE393100A978D8CAD5700CC61E1C1AB1E18971AB536981BE56905C7D12B7B5B96937DE786308AF76299C49244E93F972B95AE994E9780EAA1519685CDBA8F2F0F0D8388A3366355A64A8DE90BB78DDAEF488E2898CAE07DE86B824E8A8AE8B30525D428A7CB708EA46D31029242386349408B8364F9EB4ED47CF58052275B65167488AF6C392686ABE282282C0CDFAC2D7A7BC01E7740627431EE5189C03205C86D889495EEBA2DC63437A5A10BF3977D12A67CEA09E2E5A7363CB1294D327DDDA224A8B12B4B42998A1A0CF8CF7CAE457189313A27BD47835392479A235C39D00B0313836294CC008B81C9F9CB23692B759295A193CD719631509B9F1A69B562D6D2BE1B7BCE52DC5C24EE1612ED6AB70441177B3486CD0419F2707072DDD3F0841F099A9180712AACBE70E3C2BA5AEF520B74764B9174CF005C93516494FF4823319FA7ED90D6E46D0F744A81607D1110C716D61D972271FB5FC438F5805E969CCCD0708AD4220810855EB22196F2D209A40F94095B9B7215594C7B0AFE1295DB860EB0F3F62AB0F3C60C5B9396CD8B2B572DB786348044CA17EE833E724BA5E4C1344D8C1787C6EAA9B17681C01043850D2A82419B4EA7B33D6A8AB0A94517D4DDCDC582A6DFDE0328D192880E7152DBE968B4F596D6A333D3975EFFED1FDDBE19B6FBEB9B5BCB43C572A97ABDA7ABB0007ADE2C968C74D3D2C3B323533EB155B266315B867A78D21A2791C49FEE13AC24541441A44A56E67642BF6240D445714D368D251D222E3ACA9146D6686FB60495455024427F14012205EC4F108797EC936EE7BC036EF7FD0B6BFF2B0151E3A65E593A7AD7AEAACD50527CF58E3D4A3D644BA1A8F9EB5E6693CA4474ED9D6BDFF6C5B0F3D6C3BE72E5A636DD362783549881443327D9F15DA4DD02E16CA9D0E05926D0D0029913D53ACA1D96D57492037D00A3D160C184E71BA5FEFBEB369C3BC224809CC9444186C58B82F6D4348C7D0E898C5D37D2E1D35694D9E9FBB306733D3FB08851A1F2364A8218DA1F62B5EF18A07CAD5F2A6E657B4EF790E6F2B0FA7C5A1DC08223630326A4D3C840D541ACE27C62AF0BCE8366764BA491DF44E72496AAB774D49C408061080929E74C5C70DDF800C22C5B131310693E018C7754EA24612E8F314C7241291408A62D89968B118BCE3C75BAB2C2DE11CAC00CBE497ADBCB46A8D6C16E2A21251630954434AEA113599825B7C8333EFBDDA0FD0BC1776C7B1E7D8CB5F3A0F6C454F3D2BD668A2971B380E32E4AE5190B63A5E9556590E8F4DD81820DC6CE138650B3BB6B193F3E992782C7EEF918307FF59B450FBF6CA57BEF2C1474F3DFA3F57D6D61B3B18C28D52C1B6F48E3C9DB699FD076D7C6A9F4546466C1522E5D41854C7547AD0E85304D4A18EEA0F6305F672D0A541A89CA097F76748EA8488A3A77447350A82498816088428DC4B70D40742DAE6495B3C75E8631A6EEEE361FD0C8FBE97CF90CF50619A7C1282C6914041CC01D71BA2ABAD5E528B3DE8F5EB6A47D94D077AABDE05EAA9EBDE52A1F672D43C4795FAAB200197C20AB4D922104C8D8DDAD4EC7E9B9C9AB632F66D1555B9592E581E876375355B1F191CFE28461D7733C085DD76DB6D95EFF9BEEFFDF0C6F6F6A6EFDC8C5ED566F41574F1A0BE39C4DBEA9F9EB22AD4C60F82F2D8531AC4E1A4834187957A08560AFC8D00C5BA1F70D3DE636FB0C1E07A6583E72EFDEB3D1DC8A280161578A1363B6D78B18581C7E68538B7B6AEE31D0172B17BB50610105DA9D7FE5E08DE975F1D74EF125C9A9F0AA444D7200EF7145CD761102708203B12CAF4D9E0C4840D83432D69DDCCE56C4B2B1891D632F63A9B5DBBF0AFFEE50F7C1CE990060B08A2F4EBBFFE9B5F59585C9CDBD4CE3D48499608388BE1D1B299F189299BD8BF9F2873CCCA886411CACB8BA8A1877B5C22500A904AEF5C4202DE77FDCB259F62F1CE4BB282B27A2EB8D71D308FB8CA7350DD015904BD9A4598A076D40E17DCD7A3C0E504089E784C521B8066058299816E9BDCD2F54B6D0710F44D6A49AC70A93FC1B87950AC421985039210C70B20C7278A86C98C8EDAC4EC0CB678C037F6175E8B18F302AA77757DBD76EB777CC7BBBEF37BBEE7511EF1A41A3D1D3F7E3CF7FCDB6FFFF2F2EA4A473F74A21F34D116A99A109B98D967E3B3B336CAB18ED12A22FAF2B1511A8F91901E38A21944A063F7409738BEF6499E9888D42DEF03A3478F01B0DD84111C41949377A3F716FA965131911C0BCDBD8A73F782DAB9127A48DEDBD780F3C52CDD7E783FBBE0FD0EC8EBCFEF1246CFF68EC173228AC203C51E5A21931919F66D65A7F71FB0340ED1F64EC16187586F7327D75A5C5BFBC8BFFB0F3FF1C7B21D3CE289E106898BCD9F7CE34F7EA68EF75BCC9708560AB6462CB05E6E587868D446A766D083077C2E660D63B58853B19D4CD826042BD171E9577FD58A856E85F1BE048E4421AA2BDCE4034504A9106935AEA3B85B1FDEEBA8625CBA045C72D8734E49FFAFE435FAF53D840554AEEB30ED1E058FA917F076831CFFE915E0331080EA0D77C2166F45F0C4C444A825C6A8285CE372F2A031E4A4553A11DBE27E3616B332DE696414CF6A72D686A6F75909177F6E67DBD64A45CB6DE56D63797D3995CEFCD6953F42B64B10A5A9A9A9BB6FB8FE86BF59BCB84CFCD4C04F6ED9F9ECA6556309DB07950F1C386423484A399DB005BC95653AB58D975061000D755E23E55A139FB10138D7724DFFA47E82F7270C826B52371E28D2AE3C1E1DB54629049B3E06745DC822096D5E1F75C8DD0ECE7B0A4A650208AE8948978E0E8E688A5C01DEB5EE3FBFD63B027281FD4D22CF8AD80D082290A4E89FFE6B22B1046CE144E7D3196BE2990E1F3A6C078F1DB756226DF378548BC5BCADEE68AB5A6C48BEF2F7D3A3A3E7D5D2DE7419414E9C385164A0EFAAD66B0B39D4D636D4CC52D10E6EE6E8F0A8EDDF376BE3FBF6596662CC8A28EF754DB7202132622EC6744CDC28352650127AFCA87E331ABD47BE32055CED430BD44917F626BF1764BBE5BFB6D4ABC309D03BE9A6DDD7C5972579559286402D06D340B40F34E8A3D8BB0A43960919F2545846538433FDD63F3E869ADF675307F6434DED0054B21221C5261ED6E2D2526B6870F87FBCE10D6F50AC7D59BA8C20A8ADCE8B5EF4A27FAAD5EB7FB65D2A5416D756F1080ABE7140B154B51144F0E0D1A33679E490C5C7076D87CE6DE28616C0B4DC60754E53D20A0C054A1A9E8FA18B614702F9EEB87609D123CAD5D2D5AF0649F79E0A28EDA5F525A2E8EAA53B427CF04CF04F9B5A7A10D82308F7148BE9E775369196F5501375856D4DC42D839B7BF8DA133673F8B0C5FBFB2D9BCFD9DCD2027679DBB6F05E6BCDC6C9782A72EFD5B621BF8C204AAF7BDDEBCA474E1CFBEF0F9D3EF570157E5FD9DEB4F9ECAAAD6C6EEADB05DB7FF41A3B84184E1D3C64CD54CC5608D8B6E9E136CFE619500564379102BC4E570F3D9521D057481A7470AE71052A401CD8339457A6A0DCA574B57CAFCC93815EDA8BFE405A822BDEDF5D62A89F920E6C067774F43AE8A834418DE168DC2B0C76058214C1476272DC260F1FB129D4557A9CD82DBF6D8F5E9CF360BB50ABD9FAD656637474F4936F7BDBDB1EA3AE9414AC3E26BDFCE52FDF5EDDD848942B95EFB148381225104C039ACE1E181EF2F5457AD75C2F16AC816A738BA641415F4C9F4539EADDBA40571FFB37186C30DC2EA27A3710F9DDBCD2DE3C694F7637F9F34F215DCE853D2691550BEA0A0821E2E82C08008970FC4CA9271D55D4F64E346CEBDCC9E97D0ACECFF891C376EC5937D9E19B9E455098B4338B0B36BFBE6EEB85BC9D3C73D6CE2F2C5CFC773FF6DA5F79D9CB5EB610D476797A8C8428694BECF7FDD11F7DF0FEAFDCBFB49EDBB41CC1E2422967A7B22BD80BB3C9F1693B3C73C48E1EBDD6D293537815615B8D876D2311C6B644E1767DE223DDAB381B0FCCF3721F359860F04ABB112F83D33D7755FDCE934F7AEE6B4901E283D4CB07FD0B24557DE91D7BAE7D0F545633BAB550D46AF1B855621C717363B8B94378A363FB66AC1589D91C2AFFC2CA322A2B6F67E6E6883F6AADDB9EFFBC3F7DD39BDEF415AAB86ABAAA8428BDE73DEFA9FCC6FFF35B89BBEFBEFBBBA2F15858BF6EA65FC2EC476DCDF68FD8FE9171D452CB76AA255BABE03D688D2B1C93A093BE07881342628E2470CD978CBA0A93677589CF034D0181D01BCACBF0F730A54320539ED935F4DD832747A62E5C05C4D37EFF0A101F0652A0F38051545288EF11A2A76A25B1FA245B5E95823E812CB1D4B3D4D552836010239ED97FD08E2019CFBCE5161B999EB1954AD1EE3F7FC63631E6F3AB106669B1536BB6EF7EFF07DEFF96818181128F5E353D2E4194DEF2933F75727179F9963317CE1D8E0EF411DBB72DDD467DE18F8F65062D3D31619BF92DDCE2906D96F35622FAD4A635E94EC22AEDAA2B2F132122808EF24076C911A026400AFFBAB2EAFADCAF392EBAA5742D784A876E2E78B6777295D47BF6CA145CEF49EC2502F42456E78154600FA9BFCE13B89F047C212B0132E4DA2811AB6A755CDCF8D43E9BBDF69976ECE69B6DFAC835568051CFACAFD82A6AEAFCE2A2AD6437AC1D89CDBDE6477EF44DDFFDBDDF7D8A471F373D2141DEFFFEF757733BF9F2430F3FF4BDC56623A9DF8E1D4242F489A836A919C69E68D97F9DA1D49B4D2BE50BD62CD61CE52286EFAF28E61731081A450EF76AF68881F33083D699EE892EC19D00F1228AA7DEC56EEA9D5E71F9ABA6A0BC66C6827C201132DC0121E4956B66205055412CA577407518AD1225C6E07E9607B7C2512BA0A622236336F58C6376CD4D10E39A6758035B72766DD91E5E9CB3E5ADC0AB3A3FBF58BEF1C69BFEF3F3BEF3F91FFFD33FFDD35E4470D5F48404D14EFE7FF1E10F372AE5F2ED9FFCCC67660E1E3C1CD217BAFA48B1AA377EF1884D2225DAEB56EBADB4E26F3B97B3ED5AD923D994784C6E2258750E27EFBE3E7507C3EFA2B387745270EFF2E444E95EBCDAFDC75759574FD2FF4E082F1348854F1AC234014150B7308D08A2193F94B1952249CB41842CA5D72853EC637443C336FD8C1376E2DB6EB603D75E8BA688DAB9CD357B6465DEE637B2B60C5C985FE884E3C9CFFED2AFFCCA6FBFF0852F94967BC2F4840451FAB55FFBB5CAB3AE7FD6A148387ACB5F7DF463D184764DC09EF81C534BBB71266D069B3282E4A42271C4BA61D946C5B620CA00C8D78A0DD7C62284001751263FC059705412F2826B974049C7C743ACAE3FDE3DA5ABDD0BD88096A9B897F7C08FF168E5A11384EB5A7B2562282F85BF4EDFA5A6B2106E2793B4E48103367ECD31BBFE3B6E251A3FE13B4A5CD85CB787962E3A51F4F37AF37A151D0A2DDCF1ABBFFC9F5EF8C2173DF0D57EAA42E9AB12844A9AF8CC27AF3BFECC70BBD37ECE67EFFA5CA893087EDF49FB0CA630D8DA906B726804C9895B0B91ADA6B44C1245962B384235C84B286E592CA48908A5CB09A2D42BDF3BF76357C25CCA00476837EF657BA0B2DD74356228E97A4F35A90E9F51A67E0754AB162434B8AEC953BD3555B05B004DEB48471E8FAAD897B0F0F484EDBFF1063B8E64EC474D695B8FC5ED2D3B49F0778E986D6167C3CE5D98B391A1D1EA1BDFF4A69FF9D7FFFAA59FC07E4A28BF6AFAAA0451822885FFF25F7EE39EE3274EEC9BBB38FFEC538F9CF2951B09EC8816696B939768BACF23D4CCE0B0D52B0D8BC612566837EC02B1CAB6DED265064C3BF4E847E6836541F26FBAEB80655F40805E9769454630FB0A50CC91B7077A5E906A70AF88675BDD67E418049CAFFA0090AB959542BC97DD030121910C8EE28E16E3914B5F8359F2F457EF7D1A487F09865BC26B5C26F6AA0D642C353561476FB8C1BDA9C9C3072D343460670807EEBB70DA4E2D2F58993AE71616B45DC6D6AB5EF56F7FFAD65B9FFB81B1B13139674F2A3D2982284194C66FBFFDEDF75C73F4E86D7F77D75DB33BDBDB212D1BD26B5FFDB8A296A12A68ECEF1BB0D181514B26D2D62498D4C209FD46B95E7A69D78814DC54835035549710ABEF3464CDFD57D4409C770924BA3C704D572409816404C815C27B53F9FAE96E2155259CCE4A1C758E9681103EE1E1758B10228C88AFA23DE24A0A2AD4A749D2022045BF4965F846B6C1B8D61963737CDC268E1EB123D75F6F47AF7F968DCCEC23308CD885B5353B8F549CC7B56D21415FF8E2173BF56A7DE1BAEB6EF8F117BEF0BBFEE2A9FEA2F4932688D21D77DC51191F1B7BF8A667DF74FD035FB97FDFD93367C3FA1D747DE0A299777959DA1E706A6ACA06FB33369448590649D12CAE567AE7EB15DBA957ADA575AE94F71591104348927BA97271C444DFAA6B0F15B9C85DD250427A5F04E84A04A0EFC78551FD4A9B3E738736A26DB7BCDCDA40B509E9829E27257529620484302BF2CC0E25B77878ADDDB42C156D12E86E12F0D687339600F9937852D7DE72B31D475569C6BBD46EDBE2E686CDADACD84588B1BAB9650F9D3A65D57A6381B1BFE9AF3EFEC94F1E3E7C58CD3CA5F4940822A3F4CC3BEE587D6EFFC0FD2F78C10B8E66D7B387EFB9E76EC20C1000022B4881162FEB85917EA66E6662D2FA515589BEB44552A82B10B283DDD9D1223110D0267ED10E0C7AD59967809AC2D7B48B907629BAEF490548A5197FBB07C29457D24A4711526502B908F2C1B9DEE6510FF5079E15F6817C995B5A1152A0847E2F4212A1F7183924214B7E138629E345B546876C00B5347B9D628C5B6C3FC63B3AD86F6B85929D23023F0F31CE2E2DDB2AEEEDB9F9793CB3F0FAADCFBDED75AFF83F5FF5E9EBAFBF5E5D7E4AA9D311CB7D0D490F02FB565696DFF5CBBFF4D61F7AE8BEFB220767F6FB9E5AB3E393BE4DE0B731887DFD033608616AC5BCADD061FDEAE7CAF9B336FFE849EB148BFE895812CC6A5142068E1C85ED87EA5AD0A0B9302DCF09FB321D6D6CA0D5E811ECA2DB1F8902499D77C9F0BCFEF6524054794D920A8F27E07EE51B14CBE11DD6A8A30E21AB5CD7CE2325982A47DD0555A3E9F3D9691B3F0431AEB9C6A60F1DB551C6D709C76C05EFE91C3662219BB52C31C646A168672FCE23ADE1E57FF3D297BFFE96E7DCF219DC5B09FDD794BE2682F452BBDD1E9D9FBFF0DB77FCCA2FBDE28B7FF7F7A9673FEB59FEC35EC78856D75757ECD9E8DA6B66F7DB448668B64E80B5B161EBE7CFDBF2D9D356585BB7958539FF193A2DD94C2442361C8AD920E5F4DB84BE17B08076F47D9F7E7F44AB4F6220573223D156E7950B16E55D1A8AF2AE9228AF4941A9C39E5AAC82F80DEE569002971879555A2A2BB58BEB9A901DD4EFF31E3D6A878E1FB7B1E9296BC2541B04BE6BDB39BB8827B5B8BE4E7EDBD681C5B575C2AFC683AF7AF58FFCF45BDFFAD6CF3F596FEAF1D2D745102524656A7171EE2D7FFFF9CFBFFE77DFFDEE7EEDE830BB6FC62627C7B125FB4CBF3F32834B7C7078D42631F4D162D90AE8DC4276C316E72F18CF5A7663CD723B055FF39B26E08CD3AD34C8D20F50EA338614C4D05EB8228C16B7050BD80212B4E15ABD3ED65024377B27072BA84F57574881025511A5064136B92603AC0D05B4015A2495B418D23C7BF41A5FCC313249BFA7A72D8EAA2DD71A96C3857F24BBE6D291453DE947EFB5A66A35BB518FC69377FED8FFFD63BFFE8637BCE111881188EED791BE6E8228212903ED76E35F7DF9CB5F7EDB6FBDFDEDFBCF9C3E6D7D0CE6C0C18336AE9FBBC025D6CF5E1C9F9EB549F21380B6A4C843946D06975D5BB1D5E5655B5B5F21D2CFFACA77FDD267488BE22082F6829C88A0BE505B09F2BE1B11EDCA74F51608B9DDE03C200A04212F5F532E4EB01A84C89B6B225EA76FD0A2A8D52444C8E897206667504B876D58CEC8E404F7FAAC02036CEEECD8F2EA9AADE1BA9FDF5A0FD63D6327E7B11DD54633FFEC6FBBF99D3FFFB33FFB7BC78E1D93E9795AD2D3421025FD9CF76DB77DFBB39657D67EF377FFDFDF7BC1FBFFE48F53C70E1EB669DC45CD796993E6C99111FF1DC46307B03791840D106CA51A2DFFA9A0AD1508B3B566D9DC9A6D6F6EFAFE84053C9746B1E41F7A86F4C1271CAF5FE58446EE4D09F94974BE7F8F04A27D273B2D79C5FBD28F7315B151DA3833148F5A2C9DB2447F9FC591D2C1BE611B181C430A265D1286F7CFD8F0CC8C8560A20D90AEDFBE5DCFEDA092B66C7B27EFAFB2CBD89D1CEEFD3FDCFB8F341ECEFDD22FFDE22F7CDBB7DDF2BE9B6EBAE971676EBF96F4B411A4975061E942A572CBC7EFBCF3156F79DDEB5F34B36FFAC0F8BEA9C4E8D478A8BF1F630961C6E0CC5138F4C020AA8C98652C06B782E45AA362DBC56D2B14767CCB23FD224319E4346B15DBD04797CD9AFF54B66608F46D64BB8EB92ED72C264F0B62F8E6071059B191BEE3EB1B0E168A875309EB1BE8B7BEA1414BA751A1C3D336841A1D20908DA052B5E4338FBDC9428C45DACC165047A8A622125A6D3490DC355B45CD66B7B69AB73EE7D68B7FF227EFFBB94422ADB5B85F97BDB85A7ADA09D24B1026FC371FFFF8913F78CF7BFECD5276E555C57AEDDAFEA181C8284898181DB301A2E0319033991EB049083491D12FD0A42C9D94E30BB7A39EF469B2FF103004D88640DAE1AE2A6254AB56E1D8D467134898549A365A16F86F9C2B1F8B5A3C8DBB9D48583C93F245E36988A21F124EC652BE385C9F92ED4060ADB35DC94304AD2A440A2A18F05CA9846DDBEA64B31BAD9D9D5CE5F9B7DDB6F4432F7DC927BFFFC5FFC77F4B2693BB0BDB9EEEF40D23482F9DFEF4A713BFF3C93BAFC90C0CFEDBCF7DEEB33FB5B6BA963C28950582460607AD1F7B928683FBFBFAFDA79526909C4108358CC733004253186D6D48203D2597B7E3F329986E81EC0284D174BEEC7A2F80744055459012DF4188BCDB11D40ED6C9B6701E3670C5B5ACB3405EB3083B107E0757BC01814BE5AA9D3E7DBAB9B3995B7EFEF3BFF3EF6FBDED395FB8FDF6DBFEE9E6E73CE7D17038FCB4AAA82BD3379C20BDF46908B3B9BA7AC3BDF77DF9B7DFFF813F7B5E341E891E3A72D8C631A2A90CBA3D894DC1651E0C05BFCE300451B499FD0006360552B503B47E394D474DFF6BBF5B7DA4D95BEBE5AB5A2098E64B7CD14413D71662692B28ADDFCA974BD8839CE5AA1C430D5FECACDF16944AAA11A8167122F2F9826D6437F58940E5C435C73EF0FC6F7FDE3B6193F93BFEF00EF907386C5FBF17F5D5D2378D20BD8447963A7FFECCCBDFF9CE77FDD4DFFCCDDF1C8D27E269A2FA701A833A8E6BAC1DB3B51F7A2A91B454326909F2FA0E50BF28A02D29F46B04DA83C5574942046DCAE95986E23BA44A8A7404F4DD7D09CEAF2205427A1DD07B9C320E700DB5B4A90D0DA87B7373C37218F17038D4BEEEBAEB175EF29297FCF22B5FF9CA3F83004F691EEAE948DF7482F4123666ECE1871FFE81BFFCCBBF7CE6030F3C704DB95C3E96CD660F6C6F6E65FAD2E9D0A0D499F646C70688103A6AAF104987CE7BC75635D81CCCF70F51CC01118478ED1DA2A49D76441C490C6DF8C7968A2172F99D4E2412C94F4F4F9FE5B90B784BC557BFFAD5D5D9D9D9BF9C9C9CFC1CD7E43D7FD3D3B78C204A32FC40020F26F3A94F7D6A2C8B4A4BA6FA7E707EE1E2B55FFAD297B6CE9FBF50876BAF191A1C3A94C96462DAEA438411317A9BC6E8A597F22E358C468468C0FDFA88551E997612D286D05B5B5BED66BDBE154B241E3C76DD75FF34313571766969E9D4891327165FFBDAD7E66FBEF9E6067DE94088E2B78A184ADF52825C99C04764797939B1B8B8983A75EA94ADADAD1117B60FED6C6FBFF4BEFBFEF9FF5A5959199B9F9F470D057655BF4B9BC66B928AAA1173087C6722D9128626293B7EFCB85D7BEDB5ED6BAFBBEEDE83070FFCF6A933673E473DC5DB6FBFBDF9B297BDAC0DF2BFE176E1C927B3FF1FC5763139A794F0220000000049454E44AE426082', 'Button - Red, Round', 'png', 100, 101, 96);

-- 
-- Dumping data for table osae_log
--

-- Table osae.osae_log does not contain any data (it is empty)

-- 
-- Dumping data for table osae_object_pattern
--

-- Table osae.osae_object_pattern does not contain any data (it is empty)

-- 
-- Dumping data for table osae_object_property_scraper
--
INSERT INTO osae_object_property_scraper VALUES
(1, 6761, 'http://finance.yahoo.com/q;_ylt=A0LEVyY4nmNVCS8AjGJXNyoA;_ylu=X3oDMTByOHZyb21tBGNvbG8DYmYxBHBvcwMxBHZ0aWQDBHNlYwNzcg--?s=USO', '<span id="yfs_l84_uso">', 0, '</span>', '2016-02-15 13:00:18', '0:30:0'),
(2, 6762, 'http://finance.yahoo.com/q;_ylt=A0LEVyY4nmNVCS8AjGJXNyoA;_ylu=X3oDMTByOHZyb21tBGNvbG8DYmYxBHBvcwMxBHZ0aWQDBHNlYwNzcg--?s=USO', 'Prev Close:</th><td class="yfnc_tabledata1">', 0, '</td>', '2016-02-15 13:00:18', '0:30:0'),
(3, 6764, 'http://finance.yahoo.com/q;_ylt=A0LEVyY4nmNVCS8AjGJXNyoA;_ylu=X3oDMTByOHZyb21tBGNvbG8DYmYxBHBvcwMxBHZ0aWQDBHNlYwNzcg--?s=USO', 'YTD Return <span class="small">(Mkt)</span>&sup2;:</th><td class="yfnc_tabledata1">', 0, '</td>', '2016-02-15 13:00:19', '0:30:0'),
(4, 6766, 'http://finance.yahoo.com/q;_ylt=A0LEVyY4nmNVCS8AjGJXNyoA;_ylu=X3oDMTByOHZyb21tBGNvbG8DYmYxBHBvcwMxBHZ0aWQDBHNlYwNzcg--?s=USO', '52wk Range:</th><td class="yfnc_tabledata1" nowrap><span>', 0, '</span>', '2016-02-15 13:00:19', '0:30:0'),
(5, 6816, 'http://finance.yahoo.com/q;_ylt=A0LEVyY4nmNVCS8AjGJXNyoA;_ylu=X3oDMTByOHZyb21tBGNvbG8DYmYxBHBvcwMxBHZ0aWQDBHNlYwNzcg--?s=USO', '52wk Range:</th><td class="yfnc_tabledata1" nowrap><span>', 21, '</span>', '2016-02-15 13:00:19', '0:30:0'),
(6, 6763, 'http://finance.yahoo.com/q;_ylt=A0LEVyY4nmNVCS8AjGJXNyoA;_ylu=X3oDMTByOHZyb21tBGNvbG8DYmYxBHBvcwMxBHZ0aWQDBHNlYwNzcg--?s=USO', 'Open:</th><td class="yfnc_tabledata1">', 0, '</td>', '2016-02-15 13:00:20', '0:30:0'),
(7, 6817, 'http://finance.yahoo.com/q;_ylt=A0LEVyY4nmNVCS8AjGJXNyoA;_ylu=X3oDMTByOHZyb21tBGNvbG8DYmYxBHBvcwMxBHZ0aWQDBHNlYwNzcg--?s=USO', '<span id="yfs_h53_uso">', 0, '</span>', '2016-02-15 13:00:20', '0:30:0'),
(8, 6765, 'http://finance.yahoo.com/q;_ylt=A0LEVyY4nmNVCS8AjGJXNyoA;_ylu=X3oDMTByOHZyb21tBGNvbG8DYmYxBHBvcwMxBHZ0aWQDBHNlYwNzcg--?s=USO', '<span id="yfs_g53_uso">', 0, '</span>', '2016-02-15 13:00:21', '0:30:0');

-- 
-- Dumping data for table osae_object_state_change_history
--

-- Table osae.osae_object_state_change_history does not contain any data (it is empty)

-- 
-- Dumping data for table osae_object_type
--
INSERT INTO osae_object_type VALUES
(0000000023, 'PLUGIN', 'Generic Plugin Class', NULL, 1, 1, 0000000023, 0, 1),
(0000000024, 'PLACE', 'Core Type: Locations', 0000000043, 1, 0, 0000000024, 1, 1),
(0000000025, 'PERSON', 'Core Type: People', 0000000043, 1, 0, 0000000025, 0, 1),
(0000000026, 'THING', 'TOP Level Type for all Things', 0000000043, 1, 0, 0000000026, 1, 1),
(0000000035, 'BLUETOOTH', 'Bluetooth Plugin', 0000002815, 0, 1, 0000000023, 0, 1),
(0000000036, 'BLUETOOTH DEVICE', 'Bluetooth Device', NULL, 0, 0, 0000000026, 0, 1),
(0000000037, 'NETWORK MONITOR', 'Network Monitor Plugin', 0000002942, 0, 1, 0000000023, 0, 1),
(0000000038, 'NETWORK DEVICE', 'Network Device', NULL, 0, 0, 0000000026, 0, 1),
(0000000039, 'SYSTEM', 'Core System Data', 0000000043, 1, 1, 0000000039, 1, 1),
(0000000045, 'CONTAINER', 'Core Type: Container', NULL, 1, 0, 0000000045, 1, 1),
(0000000049, 'EMAIL', 'Email Plugin', 0000002818, 0, 1, 0000000023, 0, 1),
(0000000050, 'GUI CLIENT', 'Touch Screen App', NULL, 1, 1, 0000000026, 0, 1),
(0000000051, 'SCREEN', 'Core Type: Display Screen', NULL, 1, 0, 0000000051, 1, 1),
(0000000053, 'CONTROL', 'Core Type: Display control', NULL, 1, 0, 0000000053, 0, 1),
(0000000054, 'CONTROL STATE IMAGE', 'Control - Object State', NULL, 1, 0, 0000000053, 0, 1),
(0000000058, 'SPEECH', 'Generic Plugin Class', 0000002826, 0, 1, 0000000023, 0, 1),
(0000000060, 'SCRIPT PROCESSOR', 'Generic Plugin Class', 0000002825, 0, 1, 0000000023, 0, 1),
(0000000063, 'CONTROL PROPERTY LABEL', 'Control - Property Value', NULL, 1, 0, 0000000053, 0, 1),
(0000000064, 'CONTROL STATIC LABEL', 'Control - Static Text Label', NULL, 1, 0, 0000000053, 0, 1),
(0000000065, 'CONTROL CLICK IMAGE', 'Control - Click Control', NULL, 1, 0, 0000000053, 0, 1),
(0000000066, 'CONTROL NAVIGATION IMAGE', 'Control - Navigation Image', NULL, 1, 0, 0000000053, 0, 1),
(0000000068, 'LIST', 'Core Datatype: List', 0000000043, 1, 0, 0000000068, 0, 1),
(0000000069, 'SPEECH LIST', 'Speech List', NULL, 1, 0, 0000000068, 0, 1),
(0000000070, 'FILE LIST', 'File List', NULL, 0, 0, 0000000068, 0, 1),
(0000000075, 'JABBER', 'Jabber Plugin', 0000002941, 1, 1, 0000000023, 0, 1),
(0000000078, 'BINARY SWITCH', 'Binary Switch', NULL, 1, 0, 0000000142, 0, 1),
(0000000079, 'MULTILEVEL SWITCH', 'Multilevel Switch', NULL, 1, 0, 0000000142, 0, 1),
(0000000083, 'WEB SERVER', 'OSA Web Server Plugin', NULL, 0, 1, 0000000023, 0, 1),
(0000000085, 'WEATHER', 'Weather Data', 0000000043, 1, 0, 0000000026, 0, 1),
(0000000086, 'SERVICE', 'OSA Service', NULL, 1, 0, 0000000086, 1, 1),
(0000000087, 'USER CONTROL', 'User Control', 0000000043, 1, 0, 0000000053, 0, 1),
(0000000089, 'CONTROL TIMER LABEL', 'Control - Timer Label', NULL, 1, 0, 0000000053, 0, 0),
(0000000094, 'VR CLIENT', 'Generic Plugin Class', NULL, 1, 1, 0000000026, 0, 1),
(0000000095, 'COMPUTER', 'Core Type: Computer', NULL, 1, 0, 0000000026, 1, 1),
(0000000100, 'CONTROL CAMERA VIEWER', 'Control - IP Camera Viewer', NULL, 1, 0, 0000000053, 0, 1),
(0000000103, 'POWERSHELL', 'PowerShell Script Processor', 0000002824, 0, 0, 0000000023, 0, 1),
(0000000105, 'WUNDERGROUND', 'Weather Underground', 0000002830, 0, 1, 0000000023, 0, 1),
(0000000106, 'REST', 'Rest API', 0000001168, 0, 0, 0000000023, 0, 1),
(0000000113, 'CONTROL BROWSER', 'Control - Browser Frame', NULL, 1, 0, 0000000053, 0, 1),
(0000000131, 'CITY', 'Core Type: Locations', 0000000043, 1, 0, 0000000024, 1, 1),
(0000000135, 'ROOM', 'Core Type: Locations', 0000000043, 1, 0, 0000000024, 1, 1),
(0000000136, 'HOUSE', 'Core Type: Locations', 0000000043, 1, 0, 0000000024, 1, 1),
(0000000137, 'STATE', 'Core Type: Locations', 0000000043, 1, 0, 0000000024, 1, 1),
(0000000138, 'COUNTRY', 'Core Type: Locations', 0000000043, 1, 0, 0000000024, 1, 1),
(0000000139, 'SENSOR', 'TOP Level Sensor Type', NULL, 1, 0, 0000000139, 0, 1),
(0000000140, 'BINARY SENSOR', 'Generic On/Off Sensor', NULL, 1, 0, 0000000139, 0, 1),
(0000000141, 'BINARY SENSOR OC', 'Generic Open/Closed Sensor', NULL, 1, 0, 0000000139, 0, 1),
(0000000142, 'SWITCH', 'Binary Switch', NULL, 1, 0, 0000000142, 0, 1),
(0000000143, 'PROPERTY READER', 'Scrapes data for Properties', NULL, 0, 0, 0000000026, 0, 1),
(0000000144, 'STOCK', 'Profile of a Stock', 0000000043, 0, 0, 0000000026, 0, 1),
(0000000177, 'USER CONTROL WEATHERCONTROL', 'Custom User Control', 0000000043, 1, 0, 0000000087, 0, 1),
(0000000178, 'USER CONTROL MYSTATEBUTTON', 'Custom User Control', 0000000043, 1, 0, 0000000087, 0, 1),
(0000000182, 'INSTEON DIMMER', 'Insteon Dimmer', 0000003013, 0, 0, 0000000142, 0, 0),
(0000000184, 'INSTEON APPLIANCELINC', 'Insteon Appliancelinc', NULL, 1, 0, 0000000142, 0, 0),
(0000000186, 'INSTEON RELAY', 'Insteon Relay', 0000003013, 0, 0, 0000000142, 0, 0),
(0000000190, 'X10 DIMMER', 'X10 Dimmer', 0000003013, 0, 0, 0000000142, 0, 0),
(0000000191, 'X10 RELAY', 'X10 Relay', 0000003013, 0, 0, 0000000142, 0, 0),
(0000000239, 'CONTROL USER SELECTOR', 'Control - User Selector', NULL, 1, 0, 0000000053, 0, 1),
(0000000241, 'CONTROL SCREEN OBJECTS', 'Control - Screen Objects', NULL, 1, 0, 0000000053, 0, 1);

-- 
-- Dumping data for table osae_object_verb
--
INSERT INTO osae_object_verb VALUES
(1, 2850, 1, NULL, 2866, NULL, NULL, NULL, 0);

-- 
-- Dumping data for table osae_pattern
--
INSERT INTO osae_pattern VALUES
(6, 'Away Mode'),
(19, 'Editor'),
(1, 'Greetings'),
(5, 'Home Mode'),
(14, 'How are you'),
(27, 'Is OBJECT in CONTAINER'),
(36, 'Is OBJECT OBJECT TYPE'),
(25, 'Is OBJECT STATE'),
(29, 'OBJECT is in the CONTAINER'),
(23, 'OBJECT is STATE'),
(38, 'OBJECT PROPERTY is VALUE'),
(39, 'OBJECT VERB OBJECT'),
(30, 'Occupant Count'),
(18, 'Security Mode'),
(22, 'test'),
(7, 'Thanks'),
(33, 'This is OBJECT'),
(4, 'Time'),
(28, 'VR Wake'),
(9, 'Weather - Today'),
(10, 'Weather - Tomorrow'),
(35, 'What is OBJECT'),
(32, 'What is OBJECT''s PROPERTY'),
(26, 'Where is OBJECT'),
(13, 'Who is here'),
(31, 'Who is in the OBJECT (DESIGN THIS)'),
(34, 'Who is PERSON');

-- 
-- Dumping data for table osae_script
--
INSERT INTO osae_script VALUES
(5, 'invoke-osa -name "Speech" -Method "Say" -parameter1 "Powershell is working!" -parameter2 "TRUE" ', 2, 'PS Script'),
(7, 'FROMOBJECT.Run Method.Send Message.PARAM1 PARAM2\r\nPARAM1.Set State.PARAM2', 1, 'OBJECT is STATE'),
(8, 'FROMOBJECT.Run Method.Send Message.Hello FROMOBJECT', 1, 'Greetings'),
(11, 'Speech.Run Method.Say.Hello PARAM1\r\nPARAM1.Set Container.Livingroom', 1, 'PERSON arrived'),
(12, 'Speech.Run Method.Say.Goodbye PARAM1\r\nPARAM1.Set Container.Unknown\r\n', 1, 'PERSON left'),
(13, 'Speech.Run Method.Say.System is now in [SYSTEM.State] mode\r\nIF SYSTEM.State = HOME THEN\r\n  SYSTEM.Set Property.Violations = 0\r\nEND IF', 1, 'SYSTEM State Changed'),
(15, 'IF SYSTEM.Occupant Count = 0 THEN\r\n  SYSTEM.Run Method.Set State to Away\r\nELSE\r\n  SYSTEM.Run Method.Set State to Home\r\nEND IF', 1, 'Occupant Logic'),
(16, 'Speech.Run Method.Say From List.Speech List,Weather - Current', 1, 'Current Weather'),
(23, 'IF PARAM1.State = PARAM2 THEN\r\n  FROMOBJECT.Run Method.Send Message.Yes\r\nELSE\r\n  FROMOBJECT.Run Method.Send Message.No\r\nEND IF', 1, 'Is OBJECT STATE'),
(24, 'IF Speech.State = ON THEN\r\n    Speech.Run Method.Say.[SYSTEM.Plugins]\r\nEND IF', 1, 'Plugin Errored'),
(26, 'FROMOBJECT.Run Method.Send Message.I am in [SYSTEM.State] mode\r\nFROMOBJECT.Run Method.Send Message.[SYSTEM.Occupant String]\r\nFROMOBJECT.Run Method.Send Message.[SYSTEM.Occupied Location String]\r\nFROMOBJECT.Run Method.Send Message.[SYSTEM.Plugins]\r\n', 1, 'Status'),
(54, 'IF PARAM1.Container = Unknown THEN\r\n    FROMOBJECT.Run Method.Send Message.I do not know where\r\nELSEIF PARAM1.Container = "" THEN\r\n    FROMOBJECT.Run Method.Send Message.I do not know where\r\nELSE\r\n    FROMOBJECT.Run Method.Send Message.in the [PARAM1.Container]\r\nENDIF', 1, 'Where is OBJECT'),
(59, 'IF PARAM1.Container = PARAM2 THEN\r\n  FROMOBJECT.Run Method.Send Message.Yes\r\nELSE\r\n  FROMOBJECT.Run Method.Send Message.No\r\nEND IF', 1, 'Is OBJECT in the OBJECT'),
(82, 'SYSTEM.Run Method.Set State to Away', 1, 'AWAY Mode'),
(88, 'SYSTEM.Run Method.Set State to Home\r\nSYSTEM.Violation = 0', 1, 'HOME Mode'),
(103, 'Speech.Run Method.Say From List.Speech List,Wake Response', 1, 'VR Wake'),
(140, 'Speech.Run Method.Say From List.Speech List,Thanks', 1, 'Thanks'),
(144, 'FROMOBJECT.Run Method.Send Message.[House.Occupants]', 1, 'Who is here'),
(151, 'PARAM1.Set Container.PARAM2\r\nFROMOBJECT.Run Method.Send Message.OK', 1, 'OBJECT is in CONTAINER'),
(152, 'Speech.Run Method.Say.[SYSTEM.Occupant Count]', 1, 'Occupant Count'),
(166, 'IF PARAM1.PARAM2 = "" THEN\r\n  FROMOBJECT.Run Method.Send Message.I do not know PARAM1''s PARAM2\r\nELSE\r\n  FROMOBJECT.Run Method.Send Message.[PARAM1.PARAM2]\r\nEND IF', 1, 'What is OBJECT''s PROPERTY'),
(195, 'Speech.Run Method.Say.Of course PARAM1', 1, 'This is OBJECT'),
(234, 'FROMOBJECT.Run Method.Send Message.PARAM1', 1, 'Who is PERSON'),
(240, 'FROMOBJECT.Run Method.Send Message.a [PARAM1.Object Type]', 1, 'What is OBJECT'),
(246, 'IF PARAM1.Object Type = "PERSON" THEN\r\n  FROMOBJECT.Run Method.Send Message.Yes\r\nELSE\r\n  FROMOBJECT.Run Method.Send Message.No\r\nEND IF', 1, 'Is OBJECT a OBJECT_TYPE'),
(250, 'PARAM1.Set Property.PARAM2 = PARAM3\r\nFROMOBJECT.Run Method.Send Message.Noted', 1, 'OBJECT PROPERTY is VALUE'),
(286, 'FROMOBJECT.Run Method.Send Message.Creepy', 1, 'Communication Test'),
(292, 'speech.run method.say.I am a [SYSTEM.Object Type] object', 1, 'Test Script');

-- 
-- Dumping data for table osae_script_processors
--
INSERT INTO osae_script_processors VALUES
(1, 'OSA Default Script Processor', 'Script Processor'),
(2, 'PowerShell', 'PowerShell');

-- 
-- Dumping data for table osae_verb
--
INSERT INTO osae_verb VALUES
(1, 'loves', 'present', 1);

-- 
-- Dumping data for table osae_object_type_event
--
INSERT INTO osae_object_type_event VALUES
(0000000001, 'ON', 'Started', 0000000023),
(0000000002, 'OFF', 'Stopped', 0000000023),
(0000000003, 'ON', 'Arrived', 0000000025),
(0000000004, 'OFF', 'Left', 0000000025),
(0000000007, 'ON', 'Occupied', 0000000024),
(0000000008, 'OFF', 'Vacated', 0000000024),
(0000000021, 'ON', 'Started', 0000000035),
(0000000022, 'OFF', 'Stopped', 0000000035),
(0000000023, 'ON', 'Arrived', 0000000036),
(0000000024, 'OFF', 'Left', 0000000036),
(0000000025, 'ON', 'Started', 0000000037),
(0000000026, 'OFF', 'Stopped', 0000000037),
(0000000027, 'ON', 'On-Line', 0000000038),
(0000000028, 'OFF', 'Off-Line', 0000000038),
(0000000037, 'ON', 'On', 0000000026),
(0000000038, 'OFF', 'Off', 0000000026),
(0000000045, 'ON', 'Started', 0000000049),
(0000000046, 'OFF', 'Stopped', 0000000049),
(0000000047, 'EMAIL SENT', 'Email Sent', 0000000049),
(0000000049, 'ON', 'Started', 0000000050),
(0000000050, 'OFF', 'Stopped', 0000000050),
(0000000056, 'ON', 'Started', 0000000058),
(0000000057, 'OFF', 'Stopped', 0000000058),
(0000000062, 'ON', 'Started', 0000000060),
(0000000063, 'OFF', 'Stopped', 0000000060),
(0000000074, 'OCCUPANTS', 'Set Occupants', 0000000039),
(0000000075, 'AWAY', 'State Set to Away', 0000000039),
(0000000076, 'HOME', 'State Set to Home', 0000000039),
(0000000077, 'SLEEP', 'State Set to Sleep', 0000000039),
(0000000080, 'OCCUPIED LOCATIONS', 'Occupied Locations Set', 0000000039),
(0000000081, 'ON', 'Started', 0000000075),
(0000000082, 'OFF', 'Stopped', 0000000075),
(0000000087, 'ON', 'On', 0000000078),
(0000000088, 'OFF', 'Off', 0000000078),
(0000000089, 'ON', 'On', 0000000079),
(0000000090, 'OFF', 'Off', 0000000079),
(0000000109, 'ON', 'Started', 0000000083),
(0000000110, 'OFF', 'Stopped', 0000000083),
(0000000113, 'ON', 'Updated', 0000000085),
(0000000114, 'OFF', 'Hung', 0000000085),
(0000000115, 'ON', 'Started', 0000000086),
(0000000116, 'OFF', 'Stopped', 0000000086),
(0000000117, 'ON', 'Started', 0000000094),
(0000000118, 'OFF', 'Stopped', 0000000094),
(0000000119, 'ON', 'On', 0000000095),
(0000000120, 'OFF', 'Off', 0000000095),
(0000000133, 'ERROR', 'Error', 0000000023),
(0000000136, 'ON', 'Started', 0000000103),
(0000000137, 'OFF', 'Stopped', 0000000103),
(0000000140, 'DAY', 'Day', 0000000085),
(0000000141, 'NIGHT', 'Night', 0000000085),
(0000000142, 'DAWN', 'Dawn', 0000000085),
(0000000143, 'DUSK', 'Dusk', 0000000085),
(0000000144, 'ON', 'Started', 0000000105),
(0000000145, 'OFF', 'Stopped', 0000000105),
(0000000146, 'OFF', 'Stopped', 0000000106),
(0000000147, 'ON', 'Started', 0000000106),
(0000000158, 'PLUGINS ERRORED', 'Plugins Errored', 0000000039),
(0000000188, 'ON', 'Occupied', 0000000135),
(0000000189, 'OFF', 'Vacated', 0000000135),
(0000000191, 'ON', 'Occupied', 0000000136),
(0000000192, 'OFF', 'Vacated', 0000000136),
(0000000199, 'ON', 'On', 0000000139),
(0000000200, 'OFF', 'Off', 0000000139),
(0000000202, 'ON', 'On', 0000000140),
(0000000203, 'OFF', 'Off', 0000000140),
(0000000205, 'ON', 'Closed', 0000000141),
(0000000206, 'OFF', 'Open', 0000000141),
(0000000208, 'ON', 'On', 0000000142),
(0000000209, 'OFF', 'Off', 0000000142),
(0000000211, 'OFF', 'Opened', 0000000045),
(0000000212, 'ON', 'Closed', 0000000045),
(0000000213, 'ON', 'Arrived', 0000000143),
(0000000214, 'OFF', 'Left', 0000000143),
(0000000215, 'ON', 'Opened for Trading', 0000000144),
(0000000216, 'OFF', 'Trading Closed', 0000000144),
(0000000245, 'ON', 'On', 0000000182),
(0000000246, 'OFF', 'Off', 0000000182),
(0000000249, 'ON', 'On', 0000000184),
(0000000250, 'OFF', 'Off', 0000000184),
(0000000253, 'ON', 'On', 0000000186),
(0000000254, 'OFF', 'Off', 0000000186),
(0000000261, 'ON', 'On', 0000000190),
(0000000262, 'OFF', 'Off', 0000000190),
(0000000263, 'ON', 'On', 0000000191),
(0000000264, 'OFF', 'Off', 0000000191),
(0000000305, 'POWERLOST', 'PowerLost', 0000000095),
(0000000306, 'POWERRESTORED', 'PowerRestored', 0000000095);

-- 
-- Dumping data for table osae_object_type_method
--
INSERT INTO osae_object_type_method VALUES
(0000000001, 'ON', 'Start', 0000000023, NULL, NULL, NULL, NULL),
(0000000002, 'OFF', 'Stop', 0000000023, NULL, NULL, NULL, NULL),
(0000000003, 'ON', 'Arriving', 0000000025, NULL, NULL, NULL, NULL),
(0000000004, 'OFF', 'Leaving', 0000000025, NULL, NULL, NULL, NULL),
(0000000007, 'ON', 'Occupy', 0000000024, '', '', '', ''),
(0000000008, 'OFF', 'Vacate', 0000000024, '', '', '', ''),
(0000000021, 'ON', 'Start', 0000000035, '', '', '', ''),
(0000000022, 'OFF', 'Stop', 0000000035, '', '', '', ''),
(0000000023, 'ON', 'Arriving', 0000000036, NULL, NULL, NULL, NULL),
(0000000024, 'OFF', 'Leaving', 0000000036, NULL, NULL, NULL, NULL),
(0000000025, 'ON', 'Start', 0000000037, NULL, NULL, NULL, NULL),
(0000000026, 'OFF', 'Stop', 0000000037, NULL, NULL, NULL, NULL),
(0000000035, 'ON', 'On', 0000000026, NULL, NULL, NULL, NULL),
(0000000036, 'OFF', 'Off', 0000000026, NULL, NULL, NULL, NULL),
(0000000043, 'ON', 'Start', 0000000049, NULL, NULL, NULL, NULL),
(0000000044, 'OFF', 'Stop', 0000000049, '', '', '', ''),
(0000000045, 'SEND EMAIL', 'Send Email', 0000000049, 'TO', 'Message', '', 'Test Message'),
(0000000047, 'SCREEN SET', 'Screen Set', 0000000050, 'Screen Name', '', NULL, NULL),
(0000000055, 'ON', 'Start', 0000000058, '', '', '', ''),
(0000000056, 'OFF', 'Stop', 0000000058, '', '', '', ''),
(0000000058, 'SPEAK', 'Say', 0000000058, 'Message', '', 'Hello', ''),
(0000000062, 'ON', 'Start', 0000000060, '', '', '', ''),
(0000000063, 'OFF', 'Stop', 0000000060, '', '', '', ''),
(0000000075, 'OCCUPANTS', 'Set Occupants', 0000000039, 'Number of Occupants', '', '0', ''),
(0000000076, 'AWAY', 'Set State to Away', 0000000039, '', '', '', ''),
(0000000077, 'HOME', 'Set State to Home', 0000000039, '', '', '', ''),
(0000000078, 'SLEEP', 'Set State to Sleep', 0000000039, '', '', '', ''),
(0000000079, 'SPEAKFROM', 'Say From List', 0000000058, 'Object Name', 'Property Name', 'Speech List', 'Greetings'),
(0000000080, 'PLAY', 'Play', 0000000058, 'File', '', '', ''),
(0000000081, 'PLAYFROM', 'Play From List', 0000000058, 'List', '', '', ''),
(0000000082, 'STOP', 'Stop Playing', 0000000058, '', '', '', ''),
(0000000083, 'PAUSE', 'Pause', 0000000058, '', '', '', ''),
(0000000087, 'SEND MESSAGE', 'Send Message', 0000000075, 'To', 'Message', '', ''),
(0000000088, 'SEND FROM LIST', 'Send From List', 0000000075, 'To', 'List', '', ''),
(0000000095, 'ON', 'On', 0000000078, '', '', '', ''),
(0000000096, 'OFF', 'Off', 0000000078, '', '', '', ''),
(0000000097, 'ON', 'On', 0000000079, 'Level', '100', '', ''),
(0000000098, 'OFF', 'Off', 0000000079, '', '', '', ''),
(0000000101, 'MUTEVR', 'Mute the Microphone', 0000000058, '', '', '', ''),
(0000000104, 'SETVOICE', 'Set Voice', 0000000058, 'Voice', '', 'Anna', ''),
(0000000134, 'ON', 'Start', 0000000083, '', '', '', ''),
(0000000135, 'OFF', 'Stop', 0000000083, '', '', '', ''),
(0000000138, 'ON', 'Updated', 0000000085, '', '', '', ''),
(0000000139, 'OFF', 'Hung', 0000000085, '', '', '', ''),
(0000000140, 'EXECUTE', 'Execute Command', 0000000086, 'Program', 'Arguments', '', ''),
(0000000143, 'SPEAK', 'Say', 0000000094, '', '', '', ''),
(0000000144, 'SPEAKFROM', 'Say From List', 0000000094, '', '', '', ''),
(0000000145, 'PLAY', 'Play', 0000000094, '', '', '', ''),
(0000000146, 'PLAYFROM', 'Play From List', 0000000094, '', '', '', ''),
(0000000147, 'STOP', 'Stop', 0000000094, '', '', '', ''),
(0000000148, 'PAUSE', 'Pause', 0000000094, '', '', '', ''),
(0000000149, 'MUTEVR', 'Mute the Microphone', 0000000094, '', '', '', ''),
(0000000150, 'SETVOICE', 'Set Voice', 0000000094, '', '', '', ''),
(0000000170, 'SETTTSRATE', 'Set TTS Rate', 0000000058, 'Rate', '', '0', ''),
(0000000171, 'SETTTSVOLUME', 'Set TTS Volume', 0000000058, 'Volume', '', '100', ''),
(0000000179, 'ON', 'On', 0000000095, '', '', '', ''),
(0000000180, 'OFF', 'Off', 0000000095, '', '', '', ''),
(0000000193, 'START PLUGIN', 'Start Plugin', 0000000086, 'Plugin name', '', '', ''),
(0000000194, 'STOP PLUGIN', 'Stop Plugin', 0000000086, 'Plugin name', '', '', ''),
(0000000195, 'RUN SCRIPT', 'Run Script', 0000000060, '', '', '', ''),
(0000000196, 'ON', 'Start', 0000000103, '', '', '', ''),
(0000000197, 'OFF', 'Stop', 0000000103, '', '', '', ''),
(0000000200, 'RUN SCRIPT', 'RUN SCRIPT', 0000000103, '', '', '', ''),
(0000000203, 'ON', 'Start', 0000000105, '', '', '', ''),
(0000000204, 'OFF', 'Stop', 0000000105, '', '', '', ''),
(0000000205, 'UPDATE', 'Update', 0000000105, '', '', '', ''),
(0000000206, 'OFF', 'Stop', 0000000106, '', '', '', ''),
(0000000207, 'ON', 'Start', 0000000106, '', '', '', ''),
(0000000208, 'RELOAD PLUGINS', 'Reload plugins', 0000000086, '', '', '', ''),
(0000000269, 'ON', 'Occupy', 0000000135, NULL, NULL, NULL, NULL),
(0000000270, 'OFF', 'Vacate', 0000000135, NULL, NULL, NULL, NULL),
(0000000272, 'ON', 'Occupy', 0000000136, NULL, NULL, NULL, NULL),
(0000000273, 'OFF', 'Vacate', 0000000136, NULL, NULL, NULL, NULL),
(0000000280, 'ON', 'On', 0000000142, NULL, NULL, NULL, NULL),
(0000000281, 'OFF', 'Off', 0000000142, NULL, NULL, NULL, NULL),
(0000000282, 'ON', 'Arriving', 0000000143, NULL, NULL, NULL, NULL),
(0000000283, 'OFF', 'Leaving', 0000000143, NULL, NULL, NULL, NULL),
(0000000288, 'RUN READERS', 'Run Readers', 0000000060, '', '', '', ''),
(0000000289, 'ON', 'Open for Trading', 0000000144, '', '', '', ''),
(0000000290, 'OFF', 'Close Trading', 0000000144, '', '', '', ''),
(0000000291, 'SEND QUESTION', 'Send Question', 0000000075, 'To', '', '', ''),
(0000000292, 'SEND MESSAGE', 'Send Message', 0000000025, 'Message', 'Method', '', 'Automatic'),
(0000000293, 'SET CONTAINER', 'Set Container', 0000000025, 'Object', '', '', ''),
(0000000330, 'ON', 'On', 0000000182, 'Dim Level in %', '', '100', ''),
(0000000331, 'OFF', 'Off', 0000000182, '', '', '', ''),
(0000000332, 'BRIGHT', 'Bright', 0000000182, '', '', '', ''),
(0000000333, 'DIM', 'Dim', 0000000182, '', '', '', ''),
(0000000334, 'ON', 'On', 0000000184, '', '', '', ''),
(0000000335, 'OFF', 'Off', 0000000184, '', '', '', ''),
(0000000340, 'ON', 'On', 0000000186, 'Dim Level in %', '', '100', ''),
(0000000341, 'OFF', 'Off', 0000000186, '', '', '', ''),
(0000000348, 'ON', 'On', 0000000190, 'Dim Level in %', '', '100', ''),
(0000000349, 'OFF', 'Off', 0000000190, '', '', '', ''),
(0000000350, 'BRIGHT', 'Bright', 0000000190, '', '', '', ''),
(0000000351, 'DIM', 'Dim', 0000000190, '', '', '', ''),
(0000000352, 'ON', 'On', 0000000191, 'Dim Level in %', '', '100', ''),
(0000000353, 'OFF', 'Off', 0000000191, '', '', '', ''),
(0000000443, 'OFF', 'Stop', 0000000075, '', '', '', ''),
(0000000444, 'ON', 'Start', 0000000075, '', '', '', ''),
(0000000445, 'ON', 'Start', 0000000086, '', '', '', ''),
(0000000446, 'OFF', 'Stop', 0000000086, '', '', '', ''),
(0000000447, 'BROADCAST', 'Broadcast', 0000000086, 'group', 'message', 'Command', 'Test Message');

-- 
-- Dumping data for table osae_object_type_property
--
INSERT INTO osae_object_type_property VALUES
(0000000025, 'Port', 'Integer', '0', 0000000023, 0, NULL),
(0000000026, 'Email Address', 'String', '', 0000000025, 0, NULL),
(0000000027, 'Home Phone', 'String', '', 0000000025, 0, NULL),
(0000000028, 'Mobile Phone', 'String', '', 0000000025, 0, NULL),
(0000000029, 'Birthdate', 'DateTime', '', 0000000025, 0, NULL),
(0000000060, 'Scan Interval', 'Integer', '60', 0000000035, 0, NULL),
(0000000061, 'Discover Length', 'Integer', '8', 0000000035, 0, NULL),
(0000000062, 'Learning Mode', 'Boolean', 'TRUE', 0000000035, 0, NULL),
(0000000063, 'Discover Type', 'String', '', 0000000036, 0, NULL),
(0000000068, 'ZIP Code', 'String', '', 0000000039, 0, NULL),
(0000000069, 'Latitude', 'Integer', '0', 0000000039, 0, NULL),
(0000000070, 'Longitude', 'Integer', '0', 0000000039, 0, NULL),
(0000000071, 'Date', 'DateTime', '', 0000000039, 0, NULL),
(0000000072, 'Time', 'DateTime', '', 0000000039, 0, NULL),
(0000000073, 'Day Of Week', 'Integer', '0', 0000000039, 0, NULL),
(0000000074, 'Violations', 'Integer', '0', 0000000039, 0, NULL),
(0000000104, 'SMTP Server', 'String', '', 0000000049, 0, NULL),
(0000000105, 'SMTP Port', 'String', '25', 0000000049, 0, NULL),
(0000000106, 'ssl', 'Boolean', 'TRUE', 0000000049, 0, NULL),
(0000000107, 'Username', 'String', '', 0000000049, 0, NULL),
(0000000108, 'Password', 'String', '', 0000000049, 0, NULL),
(0000000109, 'From Address', 'String', '', 0000000049, 0, NULL),
(0000000115, 'Background Image', 'File', '', 0000000051, 0, NULL),
(0000000116, 'Current Screen', 'String', '', 0000000050, 0, NULL),
(0000000121, 'Computer Name', 'Object', '', 0000000023, 0, NULL),
(0000000122, 'Object Name', 'String', '', 0000000054, 0, NULL),
(0000000147, 'State 1 Name', 'String', '', 0000000054, 0, NULL),
(0000000148, 'State 2 Name', 'String', '', 0000000054, 0, NULL),
(0000000149, 'State 3 Name', 'String', '', 0000000054, 0, NULL),
(0000000150, 'State 1 Image', 'File', '', 0000000054, 0, NULL),
(0000000151, 'State 2 Image', 'File', '', 0000000054, 0, NULL),
(0000000152, 'State 3 Image', 'File', '', 0000000054, 0, NULL),
(0000000153, 'State 1 X', 'Integer', '100', 0000000054, 0, NULL),
(0000000154, 'State 1 Y', 'Integer', '100', 0000000054, 0, NULL),
(0000000155, 'State 2 X', 'Integer', '100', 0000000054, 0, NULL),
(0000000156, 'State 2 Y', 'Integer', '100', 0000000054, 0, NULL),
(0000000157, 'State 3 X', 'Integer', '100', 0000000054, 0, NULL),
(0000000158, 'State 3 Y', 'Integer', '100', 0000000054, 0, NULL),
(0000000159, 'ZOrder', 'Integer', '1', 0000000054, 0, NULL),
(0000000160, 'OFF TIMER', 'Integer', '-1', 0000000024, 0, NULL),
(0000000161, 'Object Name', 'String', '', 0000000063, 0, NULL),
(0000000162, 'Property Name', 'String', '', 0000000063, 0, NULL),
(0000000163, 'X', 'Integer', '200', 0000000063, 0, NULL),
(0000000164, 'Y', 'Integer', '200', 0000000063, 0, NULL),
(0000000165, 'Font Name', 'String', '', 0000000063, 0, NULL),
(0000000166, 'Font Size', 'Integer', '0', 0000000063, 0, NULL),
(0000000169, 'X', 'Integer', '0', 0000000064, 0, NULL),
(0000000170, 'Y', 'Integer', '0', 0000000064, 0, NULL),
(0000000171, 'Font Name', 'String', '', 0000000064, 0, NULL),
(0000000172, 'Font Size', 'Integer', '0', 0000000064, 0, NULL),
(0000000174, 'X', 'Integer', '0', 0000000065, 0, NULL),
(0000000175, 'Y', 'Integer', '0', 0000000065, 0, NULL),
(0000000181, 'Normal Image', 'File', '', 0000000065, 0, NULL),
(0000000182, 'Press Object Name', 'String', '', 0000000065, 0, NULL),
(0000000183, 'Press Method Name', 'String', '', 0000000065, 0, NULL),
(0000000184, 'Press Method Param 1', 'String', '', 0000000065, 0, NULL),
(0000000185, 'Press Method Param 2', 'String', '', 0000000065, 0, NULL),
(0000000186, 'Value', 'String', '', 0000000064, 0, NULL),
(0000000187, 'X', 'Integer', '0', 0000000066, 0, NULL),
(0000000188, 'Y', 'Integer', '0', 0000000066, 0, NULL),
(0000000194, 'Image', 'File', '', 0000000066, 0, NULL),
(0000000195, 'Screen', 'String', '', 0000000066, 0, NULL),
(0000000196, 'ZOrder', 'Integer', '0', 0000000066, 0, NULL),
(0000000201, 'ZOrder', 'Integer', '2', 0000000063, 0, NULL),
(0000000204, 'ZOrder', 'Integer', '0', 0000000064, 0, NULL),
(0000000205, 'Default Screen', 'String', '', 0000000050, 0, NULL),
(0000000206, 'Background Color', 'String', '', 0000000064, 0, NULL),
(0000000210, 'Values', 'List', '', 0000000068, 0, NULL),
(0000000216, 'House Entered - HOME', 'List', '', 0000000069, 0, NULL),
(0000000217, 'House Entered - AWAY', 'List', '', 0000000069, 0, NULL),
(0000000218, 'House Entered - Halloween', 'List', '', 0000000069, 0, NULL),
(0000000219, 'Values', 'File', '', 0000000070, 0, NULL),
(0000000220, 'Greetings', 'List', '', 0000000069, 0, NULL),
(0000000223, 'Thanks', 'List', '', 0000000069, 0, NULL),
(0000000224, 'Weather - Current', 'List', '', 0000000069, 0, NULL),
(0000000226, 'Weather - Tomorrow', 'List', '', 0000000069, 0, NULL),
(0000000228, 'Day Of Month', 'Integer', '0', 0000000039, 0, NULL),
(0000000229, 'Time AMPM', 'DateTime', '', 0000000039, 0, NULL),
(0000000235, 'Username', 'String', '', 0000000075, 0, NULL),
(0000000236, 'Password', 'String', '', 0000000075, 0, NULL),
(0000000246, 'Level', 'Integer', '0', 0000000079, 0, NULL),
(0000000247, 'Suffix', 'String', '', 0000000063, 0, NULL),
(0000000248, 'Prefix', 'String', '', 0000000063, 0, NULL),
(0000000249, 'Mailbox - Mailman', 'List', '', 0000000069, 0, NULL),
(0000000250, 'Mailbox - Owners', 'List', '', 0000000069, 0, NULL),
(0000000251, 'Mailbox - Secure', 'List', '', 0000000069, 0, NULL),
(0000000254, 'Voice', 'String', '', 0000000058, 0, NULL),
(0000000255, 'Voices', 'List', '', 0000000058, 0, NULL),
(0000000270, 'JabberID', 'String', '', 0000000025, 0, NULL),
(0000000271, 'JabberStatus', 'String', '', 0000000025, 0, NULL),
(0000000272, 'Security Level', 'String', '', 0000000025, 0, NULL),
(0000000273, 'Password', 'Password', '', 0000000025, 0, NULL),
(0000000276, 'DB Version', 'String', '', 0000000039, 0, NULL),
(0000000277, 'ZOrder', 'Integer', '0', 0000000065, 0, NULL),
(0000000278, 'Temperature', 'Integer', '0', 0000000024, 0, NULL),
(0000000359, 'Debug', 'Boolean', 'TRUE', 0000000039, 0, NULL),
(0000000467, 'Night1 Low', 'Integer', '0', 0000000085, 0, NULL),
(0000000468, 'Night2 Low', 'Integer', '0', 0000000085, 0, NULL),
(0000000469, 'Night3 Low', 'Integer', '0', 0000000085, 0, NULL),
(0000000470, 'Night4 Low', 'Integer', '0', 0000000085, 0, NULL),
(0000000471, 'Day1 High', 'Integer', '0', 0000000085, 0, NULL),
(0000000472, 'Day2 High', 'Integer', '0', 0000000085, 0, NULL),
(0000000473, 'Day3 High', 'Integer', '0', 0000000085, 0, NULL),
(0000000474, 'Day4 High', 'Integer', '0', 0000000085, 0, NULL),
(0000000475, 'Day1 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000476, 'Day2 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000477, 'Day3 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000478, 'Day4 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000479, 'Conditions', 'String', '', 0000000085, 0, NULL),
(0000000480, 'Wind Speed', 'Float', '', 0000000085, 0, NULL),
(0000000481, 'Wind Directions', 'String', '', 0000000085, 0, NULL),
(0000000482, 'Humidity', 'Integer', '0', 0000000085, 0, NULL),
(0000000483, 'Pressure', 'Float', '', 0000000085, 0, NULL),
(0000000484, 'Dewpoint', 'Float', '', 0000000085, 0, NULL),
(0000000485, 'Image', 'String', '', 0000000085, 0, NULL),
(0000000486, 'Visibility', 'Float', '', 0000000085, 0, NULL),
(0000000487, 'Windchill', 'Integer', '0', 0000000085, 0, NULL),
(0000000488, 'Temp', 'Float', '', 0000000085, 0, NULL),
(0000000489, 'Last Updated', 'DateTime', '', 0000000085, 0, NULL),
(0000000490, 'Day5 High', 'Integer', '0', 0000000085, 0, NULL),
(0000000491, 'Day6 High', 'Integer', '0', 0000000085, 0, NULL),
(0000000492, 'Night5 Low', 'Integer', '0', 0000000085, 0, NULL),
(0000000493, 'Night6 Low', 'Integer', '0', 0000000085, 0, NULL),
(0000000494, 'Day7 High', 'Integer', '0', 0000000085, 0, NULL),
(0000000495, 'Day5 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000496, 'Day6 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000497, 'Day7 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000498, 'Night7 Low', 'Integer', '0', 0000000085, 0, NULL),
(0000000499, 'Night1 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000500, 'Night2 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000501, 'Night3 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000502, 'Night4 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000503, 'Night5 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000504, 'Night6 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000505, 'Night7 Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000506, 'Tonight Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000507, 'Today Precip', 'Integer', '0', 0000000085, 0, NULL),
(0000000508, 'Night1 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000509, 'Night2 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000510, 'Night3 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000511, 'Night4 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000512, 'Night5 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000513, 'Night6 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000514, 'Night7 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000515, 'Tonight Forecast', 'String', '', 0000000085, 0, NULL),
(0000000516, 'Today Forecast', 'String', '', 0000000085, 0, NULL),
(0000000517, 'Day1 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000518, 'Day2 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000519, 'Day3 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000520, 'Day4 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000521, 'Day5 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000522, 'Day6 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000523, 'Day7 Forecast', 'String', '', 0000000085, 0, NULL),
(0000000524, 'Night1 Image', 'String', '', 0000000085, 0, NULL),
(0000000525, 'Night2 Image', 'String', '', 0000000085, 0, NULL),
(0000000526, 'Night3 Image', 'String', '', 0000000085, 0, NULL),
(0000000527, 'Night4 Image', 'String', '', 0000000085, 0, NULL),
(0000000528, 'Night5 Image', 'String', '', 0000000085, 0, NULL),
(0000000529, 'Night6 Image', 'String', '', 0000000085, 0, NULL),
(0000000530, 'Night7 Image', 'String', '', 0000000085, 0, NULL),
(0000000531, 'Tonight Image', 'String', '', 0000000085, 0, NULL),
(0000000532, 'Today Image', 'String', '', 0000000085, 0, NULL),
(0000000533, 'Day1 Image', 'String', '', 0000000085, 0, NULL),
(0000000534, 'Day2 Image', 'String', '', 0000000085, 0, NULL),
(0000000535, 'Day3 Image', 'String', '', 0000000085, 0, NULL),
(0000000536, 'Day4 Image', 'String', '', 0000000085, 0, NULL),
(0000000537, 'Day5 Image', 'String', '', 0000000085, 0, NULL),
(0000000538, 'Day6 Image', 'String', '', 0000000085, 0, NULL),
(0000000539, 'Day7 Image', 'String', '', 0000000085, 0, NULL),
(0000000540, 'Night1 Summary', 'String', '', 0000000085, 0, NULL),
(0000000541, 'Night2 Summary', 'String', '', 0000000085, 0, NULL),
(0000000542, 'Night3 Summary', 'String', '', 0000000085, 0, NULL),
(0000000543, 'Night4 Summary', 'String', '', 0000000085, 0, NULL),
(0000000544, 'Night5 Summary', 'String', '', 0000000085, 0, NULL),
(0000000545, 'Night6 Summary', 'String', '', 0000000085, 0, NULL),
(0000000546, 'Night7 Summary', 'String', '', 0000000085, 0, NULL),
(0000000547, 'Tonight Summary', 'String', '', 0000000085, 0, NULL),
(0000000548, 'Today Summary', 'String', '', 0000000085, 0, NULL),
(0000000549, 'Day1 Summary', 'String', '', 0000000085, 0, NULL),
(0000000550, 'Day2 Summary', 'String', '', 0000000085, 0, NULL),
(0000000551, 'Day3 Summary', 'String', '', 0000000085, 0, NULL),
(0000000552, 'Day4 Summary', 'String', '', 0000000085, 0, NULL),
(0000000553, 'Day5 Summary', 'String', '', 0000000085, 0, NULL),
(0000000554, 'Day6 Summary', 'String', '', 0000000085, 0, NULL),
(0000000555, 'Day7 Summary', 'String', '', 0000000085, 0, NULL),
(0000000570, 'Sunrise', 'DateTime', '', 0000000085, 0, NULL),
(0000000571, 'Sunset', 'DateTime', '', 0000000085, 0, NULL),
(0000000572, 'Tonight Low', 'Integer', '0', 0000000085, 0, NULL),
(0000000573, 'Today High', 'Integer', '0', 0000000085, 0, NULL),
(0000000574, 'X', 'Integer', '0', 0000000087, 0, NULL),
(0000000575, 'Y', 'Integer', '0', 0000000087, 0, NULL),
(0000000576, 'Image', 'File', '', 0000000087, 0, NULL),
(0000000577, 'Screen', 'String', '', 0000000087, 0, NULL),
(0000000578, 'ZOrder', 'Integer', '0', 0000000087, 0, NULL),
(0000000579, 'Control Type', 'String', '', 0000000087, 0, NULL),
(0000000580, 'Back Color', 'String', '', 0000000063, 0, NULL),
(0000000581, 'Fore Color', 'String', '', 0000000063, 0, NULL),
(0000000582, 'Object Name', 'String', '', 0000000089, 0, NULL),
(0000000583, 'Property Name', 'String', '', 0000000089, 0, NULL),
(0000000584, 'X', 'Integer', '0', 0000000089, 0, NULL),
(0000000585, 'Y', 'Integer', '0', 0000000089, 0, NULL),
(0000000586, 'Font Name', 'String', '', 0000000089, 0, NULL),
(0000000587, 'Font Size', 'Integer', '0', 0000000089, 0, NULL),
(0000000588, 'ZOrder', 'Integer', '0', 0000000089, 0, NULL),
(0000000589, 'Suffix', 'String', '', 0000000089, 0, NULL),
(0000000590, 'Prefix', 'String', '', 0000000089, 0, NULL),
(0000000591, 'Back Color', 'String', '', 0000000089, 0, NULL),
(0000000592, 'Fore Color', 'String', '', 0000000089, 0, NULL),
(0000000606, 'System Plugin', 'Boolean', 'FALSE', 0000000023, 0, NULL),
(0000000608, 'System Plugin', 'Boolean', 'FALSE', 0000000035, 0, NULL),
(0000000609, 'System Plugin', 'Boolean', 'TRUE', 0000000037, 0, NULL),
(0000000611, 'System Plugin', 'Boolean', 'TRUE', 0000000049, 0, NULL),
(0000000612, 'System Plugin', 'Boolean', 'FALSE', 0000000058, 0, NULL),
(0000000613, 'System Plugin', 'Boolean', 'TRUE', 0000000060, 0, NULL),
(0000000617, 'System Plugin', 'Boolean', 'TRUE', 0000000075, 0, NULL),
(0000000620, 'System Plugin', 'Boolean', 'TRUE', 0000000083, 0, NULL),
(0000000638, 'Type', 'String', '', 0000000089, 0, NULL),
(0000000639, 'Computer Name', 'Object', '', 0000000094, 0, NULL),
(0000000640, 'VR Input Muted', 'Boolean', 'TRUE', 0000000094, 0, NULL),
(0000000641, 'Voice', 'String', '', 0000000094, 0, NULL),
(0000000642, 'Voices', 'List', '', 0000000094, 0, NULL),
(0000000643, 'VR Enabled', 'Boolean', 'FALSE', 0000000094, 0, NULL),
(0000000644, 'VR Sleep Pattern', 'String', 'Thanks', 0000000094, 0, NULL),
(0000000645, 'VR Wake Pattern', 'String', 'VR Wake', 0000000094, 0, NULL),
(0000000675, 'OFF TIMER', 'Integer', '-1', 0000000026, 0, NULL),
(0000000676, 'OFF TIMER', 'Integer', '-1', 0000000025, 0, NULL),
(0000000677, 'Poll Interval', 'Integer', '30', 0000000037, 0, NULL),
(0000000679, 'Prune Logs', 'Boolean', 'TRUE', 0000000039, 0, NULL),
(0000000684, 'TTS Rate', 'Integer', '0', 0000000058, 0, NULL),
(0000000685, 'TTS Volume', 'Integer', '0', 0000000058, 0, NULL),
(0000000686, 'Host Name', 'String', '', 0000000095, 0, NULL),
(0000000687, 'OS', 'String', '', 0000000095, 0, NULL),
(0000000693, 'Can Hear this Plugin', 'Object', 'Speech', 0000000094, 0, NULL),
(0000000694, 'Speaking', 'Boolean', 'FALSE', 0000000058, 0, NULL),
(0000000696, 'X', 'Integer', '0', 0000000100, 0, NULL),
(0000000697, 'Y', 'Integer', '0', 0000000100, 0, NULL),
(0000000698, 'ZOrder', 'Integer', '0', 0000000100, 0, NULL),
(0000000699, 'Object Name', 'String', '', 0000000100, 0, NULL),
(0000000701, 'System Plugin', 'Boolean', 'TRUE', 0000000103, 0, NULL),
(0000000809, 'Latitude', 'Float', '', 0000000085, 0, NULL),
(0000000810, 'Longitude', 'Float', '', 0000000085, 0, NULL),
(0000000811, 'DayNight', 'String', '', 0000000085, 0, NULL),
(0000000813, 'State', 'String', 'KS', 0000000105, 0, NULL),
(0000000814, 'Forecast Interval', 'Integer', '180', 0000000105, 0, NULL),
(0000000815, 'Metric', 'Boolean', 'FALSE', 0000000105, 0, NULL),
(0000000816, 'DuskPre', 'Integer', '0', 0000000105, 0, NULL),
(0000000817, 'DuskPost', 'Integer', '0', 0000000105, 0, NULL),
(0000000818, 'DawnPre', 'Integer', '0', 0000000105, 0, NULL),
(0000000819, 'DawnPost', 'Integer', '0', 0000000105, 0, NULL),
(0000000820, 'System Plugin', 'Boolean', 'TRUE', 0000000106, 0, NULL),
(0000000822, 'Show Help', 'Boolean', 'TRUE', 0000000106, 0, NULL),
(0000000835, 'Refridgerator - HOME', 'List', '', 0000000069, 0, NULL),
(0000000836, 'Refridgerator - AWAY', 'List', '', 0000000069, 0, NULL),
(0000000837, 'Living room - AWAY', 'List', '', 0000000069, 0, NULL),
(0000000838, 'Plugins Found', 'Integer', '0', 0000000039, 1, NULL),
(0000000839, 'Plugins Running', 'Integer', '0', 0000000039, 1, NULL),
(0000000840, 'Plugins Enabled', 'Integer', '0', 0000000039, 1, NULL),
(0000000841, 'Plugins Errored', 'Integer', '0', 0000000039, 1, NULL),
(0000000842, 'Plugins', 'String', '', 0000000039, 0, NULL),
(0000000843, 'Content List', 'List', '', 0000000045, 0, NULL),
(0000000866, 'Timeout', 'Integer', '60', 0000000083, 0, NULL),
(0000000867, 'URI', 'String', '', 0000000113, 0, NULL),
(0000000868, 'X', 'Integer', '0', 0000000113, 0, NULL),
(0000000869, 'Y', 'Integer', '0', 0000000113, 0, NULL),
(0000000870, 'ZOrder', 'Integer', '0', 0000000113, 0, NULL),
(0000000871, 'Width', 'Integer', '0', 0000000113, 0, NULL),
(0000000872, 'Height', 'Integer', '0', 0000000113, 0, NULL),
(0000000900, 'Pressed Image', 'File', '', 0000000065, 0, NULL),
(0000000902, 'Release Method Name', 'String', '', 0000000065, 0, NULL),
(0000000903, 'Release Object Name', 'String', '', 0000000065, 0, NULL),
(0000000904, 'Release Method Param 1', 'String', '', 0000000065, 0, NULL),
(0000000906, 'Press Script Param 2', 'String', '', 0000000065, 0, NULL),
(0000000907, 'Press Script Param 1', 'String', '', 0000000065, 0, NULL),
(0000000908, 'Press Script Name', 'String', '', 0000000065, 0, NULL),
(0000000909, 'Release Method Param 2', 'String', '', 0000000065, 0, NULL),
(0000000910, 'Release Script Name', 'String', '', 0000000065, 0, NULL),
(0000000911, 'Release Script Param 1', 'String', '', 0000000065, 0, NULL),
(0000000912, 'Release Script Param 2', 'String', '', 0000000065, 0, NULL),
(0000000913, 'State 1 Image 2', 'File', '', 0000000054, 0, NULL),
(0000000914, 'State 1 Image 3', 'File', '', 0000000054, 0, NULL),
(0000000915, 'State 1 Image 4', 'File', '', 0000000054, 0, NULL),
(0000000916, 'State 2 Image 2', 'File', '', 0000000054, 0, NULL),
(0000000917, 'State 2 Image 3', 'File', '', 0000000054, 0, NULL),
(0000000918, 'State 2 Image 4', 'File', '', 0000000054, 0, NULL),
(0000000919, 'Repeat Animation', 'Boolean', 'TRUE', 0000000054, 0, NULL),
(0000000920, 'Frame Delay', 'Integer', '100', 0000000054, 0, NULL),
(0000000921, 'Show Slider', 'Boolean', '', 0000000054, 0, NULL),
(0000000922, 'State 3 Image 2', 'File', '', 0000000054, 0, NULL),
(0000000923, 'State 3 Image 3', 'File', '', 0000000054, 0, NULL),
(0000000924, 'State 3 Image 4', 'File', '', 0000000054, 0, NULL),
(0000000930, 'Slider Method', 'String', '', 0000000054, 0, NULL),
(0000000932, 'Debug', 'Boolean', 'FALSE', 0000000058, 0, NULL),
(0000000933, 'Debug', 'Boolean', 'FALSE', 0000000075, 0, NULL),
(0000000934, 'Debug', 'Boolean', 'FALSE', 0000000060, 0, NULL),
(0000000936, 'Debug', 'Boolean', 'FALSE', 0000000035, 0, NULL),
(0000000944, 'Time', 'List', '', 0000000069, 0, NULL),
(0000000946, 'Wake Response', 'List', '', 0000000069, 0, NULL),
(0000000948, 'Occupants', 'String', '', 0000000024, 0, NULL),
(0000000949, 'Light Level', 'Integer', '100', 0000000024, 0, NULL),
(0000000977, 'Population', 'Integer', '0', 0000000131, 0, NULL),
(0000000978, 'Tax Rate', 'Integer', '0', 0000000131, 0, NULL),
(0000001003, 'OFF TIMER', 'Integer', '-1', 0000000135, 0, NULL),
(0000001004, 'Temperature', 'Integer', '0', 0000000135, 1, NULL),
(0000001010, 'OFF TIMER', 'Integer', '-1', 0000000136, 1, NULL),
(0000001011, 'Temperature', 'Integer', '0', 0000000136, 1, NULL),
(0000001012, 'Occupants', 'String', '', 0000000136, 0, NULL),
(0000001013, 'Light Level', 'Integer', '100', 0000000136, 1, NULL),
(0000001017, 'URL', 'String', '', 0000000131, 0, NULL),
(0000001018, 'Mayor', 'String', '', 0000000131, 0, NULL),
(0000001019, 'Population', 'Integer', '0', 0000000137, 0, NULL),
(0000001021, 'URL', 'String', '', 0000000137, 0, NULL),
(0000001022, 'Governor', 'String', '', 0000000137, 0, NULL),
(0000001026, 'Population', 'Integer', '0', 0000000138, 0, NULL),
(0000001027, 'URL', 'String', '', 0000000138, 0, NULL),
(0000001028, 'President', 'String', '', 0000000138, 0, NULL),
(0000001029, 'Full Name', 'String', '', 0000000025, 0, NULL),
(0000001032, 'External Access Points', 'Integer', '0', 0000000136, 0, NULL),
(0000001033, 'External Access Points', 'Integer', '0', 0000000135, 0, NULL),
(0000001034, 'Internal Access Points', 'Integer', '0', 0000000135, 0, NULL),
(0000001035, 'Light Level', 'Integer', '100', 0000000135, 1, NULL),
(0000001036, 'Occupants', 'String', '', 0000000135, 0, NULL),
(0000001038, 'IP Address', 'String', '', 0000000095, 0, NULL),
(0000001039, 'Processor', 'String', '', 0000000095, 0, NULL),
(0000001040, 'RAM', 'String', '', 0000000095, 0, NULL),
(0000001041, 'AI Focused on Object Type', 'String', '', 0000000039, 0, NULL),
(0000001042, 'AI Focused on Property', 'String', '', 0000000039, 0, NULL),
(0000001043, 'IP Address', 'String', '', 0000000038, 0, NULL),
(0000001044, 'Zip Code', 'Integer', '0', 0000000131, 0, NULL),
(0000001045, 'Elevation', 'Integer', '0', 0000000131, 0, NULL),
(0000001046, 'Capital', 'String', '', 0000000137, 0, NULL),
(0000001047, 'Latitude', 'String', '', 0000000137, 0, NULL),
(0000001048, 'Longitude', 'String', '', 0000000137, 0, NULL),
(0000001049, 'Median Household Income', 'Integer', '0', 0000000137, 0, NULL),
(0000001050, 'Vice President ', 'String', '', 0000000138, 0, NULL),
(0000001051, 'Total Area', 'String', '', 0000000138, 0, NULL),
(0000001052, 'GDP', 'String', '', 0000000138, 0, NULL),
(0000001053, 'Trust Level', 'Integer', '90', 0000000025, 0, NULL),
(0000001054, 'Trust Level', 'Integer', '50', 0000000039, 0, NULL),
(0000001056, 'Communication Method', 'String', 'Speech', 0000000025, 0, NULL),
(0000001058, 'URL', 'String', '', 0000000143, 0, NULL),
(0000001059, 'Search Prefix', 'String', '', 0000000143, 0, NULL),
(0000001060, 'Search Suffix', 'String', '', 0000000143, 0, NULL),
(0000001061, 'Search Prefix Offset', 'Integer', '1', 0000000143, 0, NULL),
(0000001062, 'Object', 'String', '', 0000000143, 0, NULL),
(0000001063, 'Property', 'String', '', 0000000143, 0, NULL),
(0000001065, 'Price', 'Integer', '0', 0000000144, 0, NULL),
(0000001066, 'Previous Close', 'Integer', '0', 0000000144, 0, NULL),
(0000001067, 'Open', 'Integer', '0', 0000000144, 0, NULL),
(0000001068, 'Year To Date Return', 'Integer', '0', 0000000144, 0, NULL),
(0000001069, 'Low Today', 'Integer', '0', 0000000144, 0, NULL),
(0000001070, '52 Week Low', 'Integer', '0', 0000000144, 0, NULL),
(0000001071, '52 Week High', 'Integer', '0', 0000000144, 0, NULL),
(0000001072, 'High Today', 'Integer', '0', 0000000144, 0, NULL),
(0000001073, 'Mother', 'Object Type', '', 0000000025, 0, 25),
(0000001074, 'Father', 'Object Type', '', 0000000025, 0, 25),
(0000001076, 'Occupant Count', 'Integer', '0', 0000000136, 1, NULL),
(0000001078, 'Occupant Count', 'Integer', '0', 0000000024, 0, NULL),
(0000001079, 'Occupant Count', 'Integer', '0', 0000000135, 1, NULL),
(0000001080, 'Contained X', 'Integer', '100', 0000000054, 0, NULL),
(0000001081, 'Contained Y', 'Integer', '100', 0000000054, 0, NULL),
(0000001082, 'OFF TIMER', 'Integer', '-1', 0000000140, 0, NULL),
(0000001083, 'OFF TIMER', 'Integer', '-1', 0000000141, 0, NULL),
(0000001084, 'OFF TIMER', 'Integer', '-1', 0000000139, 0, NULL),
(0000001085, 'Rest Port', 'Integer', '8732', 0000000106, 0, NULL),
(0000001092, 'Occupied Rooms', 'String', '', 0000000136, 0, NULL),
(0000001093, 'Occupied Room Count', 'Integer', '0', 0000000136, 0, NULL),
(0000001148, 'test list', 'List', '', 0000000039, 0, NULL),
(0000001149, 'Control Type', 'String', '', 0000000177, 0, NULL),
(0000001150, 'Object Name', 'String', '', 0000000177, 0, NULL),
(0000001151, 'X', 'Integer', '0', 0000000177, 0, NULL),
(0000001152, 'Y', 'Integer', '0', 0000000177, 0, NULL),
(0000001153, 'ZOrder', 'Integer', '0', 0000000177, 0, NULL),
(0000001154, 'Key', 'String', '', 0000000105, 0, NULL),
(0000001155, 'City', 'String', 'Ellis', 0000000105, 0, NULL),
(0000001156, 'Conditions Interval', 'Integer', '15', 0000000105, 0, NULL),
(0000001157, 'Control Type', 'String', '', 0000000178, 0, NULL),
(0000001158, 'Object Name', 'String', '', 0000000178, 0, NULL),
(0000001159, 'X', 'Integer', '0', 0000000178, 0, NULL),
(0000001160, 'Y', 'Integer', '0', 0000000178, 0, NULL),
(0000001161, 'ZOrder', 'Integer', '0', 0000000178, 0, NULL),
(0000001168, 'Off Timer', 'Integer', '-1', 0000000182, 0, NULL),
(0000001169, 'Level', 'Integer', '0', 0000000182, 0, NULL),
(0000001175, 'Off Timer', 'Integer', '-1', 0000000186, 0, NULL),
(0000001182, 'Off Timer', 'Integer', '-1', 0000000190, 0, NULL),
(0000001183, 'Level', 'Integer', '0', 0000000190, 0, NULL),
(0000001184, 'Off Timer', 'Integer', '-1', 0000000191, 0, NULL),
(0000001211, 'PowerLineStatus', 'String', '', 0000000095, 0, NULL),
(0000001212, 'BatteryChargeStatus', 'String', '', 0000000095, 0, NULL),
(0000001213, 'BatteryFullLifeTime', 'Integer', '0', 0000000095, 0, NULL),
(0000001214, 'BatteryLifePercent', 'Integer', '0', 0000000095, 0, NULL),
(0000001215, 'BatteryLifeRemaining', 'Integer', '0', 0000000095, 0, NULL),
(0000001321, 'EventGhost Port', 'String', '', 0000000095, 0, NULL),
(0000001322, 'PIN', 'Password', '', 0000000025, 0, NULL),
(0000001324, 'Current User', 'String', '', 0000000050, 0, NULL),
(0000001325, 'X', 'Integer', '0', 0000000239, 0, NULL),
(0000001326, 'Y', 'Integer', '0', 0000000239, 0, NULL),
(0000001327, 'ZOrder', 'Integer', '0', 0000000239, 0, NULL),
(0000001328, 'Trust Level', 'Integer', '50', 0000000105, 0, NULL),
(0000001329, 'Trust Level', 'Integer', '90', 0000000060, 0, NULL),
(0000001330, 'Trust Level', 'Integer', '90', 0000000035, 0, NULL),
(0000001333, 'Trust Level', 'Integer', '90', 0000000049, 0, NULL),
(0000001334, 'Trust Level', 'Integer', '90', 0000000075, 0, NULL),
(0000001335, 'Trust Level', 'Integer', '90', 0000000023, 0, NULL),
(0000001336, 'Trust Level', 'Integer', '90', 0000000103, 0, NULL),
(0000001338, 'Trust Level', 'Integer', '90', 0000000058, 0, NULL),
(0000001339, 'Debug', 'Boolean', 'FALSE', 0000000050, 0, NULL),
(0000001347, 'X', 'Integer', '0', 0000000241, 1, NULL),
(0000001348, 'Y', 'Integer', '0', 0000000241, 1, NULL),
(0000001349, 'ZOrder', 'Integer', '0', 0000000241, 1, NULL),
(0000001350, 'Debug', 'Boolean', 'FALSE', 0000000105, 0, NULL),
(0000001351, 'Debug', 'Boolean', 'FALSE', 0000000037, 0, NULL),
(0000001352, 'Title', 'String', 'OSA Screens', 0000000050, 0, NULL),
(0000001353, 'Logout on Close', 'Boolean', 'FALSE', 0000000050, 0, NULL),
(0000001355, 'Version', 'String', '', 0000000035, 0, NULL),
(0000001358, 'Version', 'String', '', 0000000049, 0, NULL),
(0000001361, 'Version', 'String', '', 0000000075, 0, NULL),
(0000001362, 'Version', 'String', '', 0000000037, 0, NULL),
(0000001363, 'Version', 'String', '', 0000000023, 0, NULL),
(0000001364, 'Version', 'String', '', 0000000103, 0, NULL),
(0000001366, 'Version', 'String', '', 0000000106, 0, NULL),
(0000001367, 'Version', 'String', '', 0000000060, 0, NULL),
(0000001368, 'Version', 'String', '', 0000000058, 0, NULL),
(0000001370, 'Version', 'String', '', 0000000094, 0, NULL),
(0000001373, 'Version', 'String', '', 0000000083, 0, NULL),
(0000001374, 'Version', 'String', '', 0000000105, 0, NULL),
(0000001375, 'Trust Level', 'Integer', '50', 0000000086, 0, NULL),
(0000001380, 'Distributable', 'Boolean', 'FALSE', 0000000049, 0, NULL),
(0000001383, 'Author', 'String', '', 0000000035, 0, NULL),
(0000001386, 'Author', 'String', '', 0000000049, 0, NULL),
(0000001389, 'Author', 'String', '', 0000000075, 0, NULL),
(0000001390, 'Author', 'String', '', 0000000037, 0, NULL),
(0000001391, 'Author', 'String', '', 0000000023, 0, NULL),
(0000001392, 'Author', 'String', '', 0000000103, 0, NULL),
(0000001394, 'Author', 'String', '', 0000000106, 0, NULL),
(0000001395, 'Author', 'String', '', 0000000060, 0, NULL),
(0000001396, 'Author', 'String', '', 0000000058, 0, NULL),
(0000001399, 'Author', 'String', '', 0000000083, 0, NULL),
(0000001400, 'Author', 'String', '', 0000000105, 0, NULL),
(0000001402, 'Trust Level', 'Integer', '90', 0000000106, 0, NULL),
(0000001403, 'Hide Controls', 'Boolean', 'FALSE', 0000000083, 0, NULL),
(0000001414, 'Trust Level', 'Integer', '50', 0000000050, 0, NULL);

-- 
-- Dumping data for table osae_object_type_state
--
INSERT INTO osae_object_type_state VALUES
(0000000001, 'ON', 'Running', 0000000023),
(0000000002, 'OFF', 'Stopped', 0000000023),
(0000000003, 'ON', 'Here', 0000000025),
(0000000004, 'OFF', 'Gone', 0000000025),
(0000000007, 'ON', 'Occupied', 0000000024),
(0000000008, 'OFF', 'Vacant', 0000000024),
(0000000021, 'ON', 'Running', 0000000035),
(0000000022, 'OFF', 'Stopped', 0000000035),
(0000000023, 'ON', 'Here', 0000000036),
(0000000024, 'OFF', 'Gone', 0000000036),
(0000000025, 'ON', 'Running', 0000000037),
(0000000026, 'OFF', 'Stopped', 0000000037),
(0000000027, 'ON', 'On-Line', 0000000038),
(0000000028, 'OFF', 'Off-Line', 0000000038),
(0000000032, 'HOME', 'Home', 0000000039),
(0000000033, 'AWAY', 'Away', 0000000039),
(0000000034, 'SLEEP', 'Sleep', 0000000039),
(0000000040, 'ON', 'On', 0000000026),
(0000000041, 'OFF', 'Off', 0000000026),
(0000000047, 'ON', 'Running', 0000000049),
(0000000048, 'OFF', 'Stopped', 0000000049),
(0000000050, 'ON', 'Running', 0000000050),
(0000000051, 'OFF', 'Stopped', 0000000050),
(0000000057, 'ON', 'Running', 0000000058),
(0000000058, 'OFF', 'Stopped', 0000000058),
(0000000063, 'ON', 'Running', 0000000060),
(0000000064, 'OFF', 'Stopped', 0000000060),
(0000000077, 'ON', 'Running', 0000000075),
(0000000078, 'OFF', 'Stopped', 0000000075),
(0000000083, 'ON', 'On', 0000000078),
(0000000084, 'OFF', 'Off', 0000000078),
(0000000085, 'ON', 'On', 0000000079),
(0000000086, 'OFF', 'Off', 0000000079),
(0000000105, 'ON', 'Running', 0000000083),
(0000000106, 'OFF', 'Stopped', 0000000083),
(0000000109, 'ON', 'Current', 0000000085),
(0000000110, 'OFF', 'Obsolete', 0000000085),
(0000000111, 'ON', 'Running', 0000000086),
(0000000112, 'OFF', 'Stopped', 0000000086),
(0000000113, 'ON', 'Running', 0000000094),
(0000000114, 'OFF', 'Stopped', 0000000094),
(0000000115, 'ON', 'On', 0000000095),
(0000000116, 'OFF', 'Off', 0000000095),
(0000000124, 'ERROR', 'Error', 0000000023),
(0000000127, 'ON', 'Running', 0000000103),
(0000000128, 'OFF', 'Stopped', 0000000103),
(0000000131, 'ON', 'Running', 0000000105),
(0000000132, 'OFF', 'Stopped', 0000000105),
(0000000133, 'OFF', 'Stopped', 0000000106),
(0000000134, 'ON', 'Running', 0000000106),
(0000000169, 'ON', 'Exists', 0000000131),
(0000000180, 'ON', 'Occupied', 0000000135),
(0000000181, 'OFF', 'Vacant', 0000000135),
(0000000183, 'ON', 'Occupied', 0000000136),
(0000000184, 'OFF', 'Vacant', 0000000136),
(0000000185, 'ON', 'Exists', 0000000137),
(0000000188, 'ON', 'Exists', 0000000138),
(0000000191, 'ON', 'On', 0000000139),
(0000000192, 'OFF', 'Off', 0000000139),
(0000000194, 'ON', 'On', 0000000140),
(0000000195, 'OFF', 'Off', 0000000140),
(0000000197, 'ON', 'Closed', 0000000141),
(0000000198, 'OFF', 'Open', 0000000141),
(0000000200, 'ON', 'On', 0000000142),
(0000000201, 'OFF', 'Off', 0000000142),
(0000000203, 'ON', 'Closed', 0000000045),
(0000000204, 'OFF', 'Open', 0000000045),
(0000000205, 'ON', 'Here', 0000000143),
(0000000206, 'OFF', 'Gone', 0000000143),
(0000000208, 'ON', 'Open', 0000000144),
(0000000209, 'OFF', 'Closed', 0000000144),
(0000000242, 'ON', 'On', 0000000182),
(0000000243, 'OFF', 'Off', 0000000182),
(0000000246, 'ON', 'On', 0000000184),
(0000000247, 'OFF', 'Off', 0000000184),
(0000000250, 'ON', 'On', 0000000186),
(0000000251, 'OFF', 'Off', 0000000186),
(0000000258, 'ON', 'On', 0000000190),
(0000000259, 'OFF', 'Off', 0000000190),
(0000000260, 'ON', 'On', 0000000191),
(0000000261, 'OFF', 'Off', 0000000191);

-- 
-- Dumping data for table osae_pattern_match
--
INSERT INTO osae_pattern_match VALUES
(3, 1, 'Hello'),
(12, 4, 'What time is it'),
(13, 4, 'what is the time'),
(14, 5, 'Disarm the house'),
(15, 5, 'Home mode'),
(16, 5, 'set the house to home'),
(18, 6, 'Set house to away'),
(19, 6, 'Set system to away'),
(20, 6, 'Away mode'),
(21, 6, 'Arm the system'),
(22, 6, 'Arm the house'),
(23, 7, 'Thanks'),
(24, 7, 'Thank You'),
(29, 9, 'Current Weather'),
(30, 9, 'What is weather like'),
(31, 10, 'Tell me about tomorrow''s weather'),
(32, 10, 'what will the weather be like tomorrow'),
(41, 13, 'Who is here'),
(42, 13, 'Who is in the house'),
(43, 14, 'How is System'),
(44, 14, 'How is System doing'),
(50, 18, 'Security Mode'),
(51, 18, 'What mode is the house in'),
(52, 18, 'What mode are you in'),
(55, 23, '[OBJECT] is [STATE]'),
(61, 25, 'Is [OBJECT] [STATE]'),
(62, 14, 'System Status'),
(63, 14, 'Status check'),
(64, 26, 'Where is [OBJECT]'),
(70, 28, 'Computer'),
(71, 29, '[OBJECT] is in [CONTAINER]'),
(72, 30, 'How many people are here'),
(73, 30, 'How many occupants are here'),
(74, 30, 'occupant count'),
(76, 32, 'What is [OBJECT] [PROPERTY]'),
(84, 33, 'This is [OBJECT]'),
(91, 27, 'Is [OBJECT] in [CONTAINER]'),
(93, 5, 'Disarm the system'),
(94, 5, 'set the system to home'),
(95, 1, 'greetings'),
(98, 34, 'Who is [PERSON]'),
(102, 35, 'What is [OBJECT]'),
(105, 36, 'is [OBJECT] an [OBJECT_TYPE]'),
(115, 38, '[OBJECT] [PROPERTY] is [VALUE]'),
(116, 39, '[OBJECT] [VERB] [OBJECT]');

-- 
-- Dumping data for table osae_pattern_script
--
INSERT INTO osae_pattern_script VALUES
(0000000002, 0000000023, 7, 1),
(0000000004, 0000000009, 16, 1),
(0000000005, 0000000025, 23, 1),
(0000000006, 0000000014, 26, 1),
(0000000007, 0000000026, 54, 1),
(0000000008, 0000000027, 59, 1),
(0000000010, 0000000028, 103, 1),
(0000000011, 0000000001, 8, 1),
(0000000012, 0000000007, 140, 1),
(0000000013, 0000000013, 144, 1),
(0000000014, 0000000006, 82, 1),
(0000000015, 0000000005, 88, 1),
(0000000016, 0000000029, 151, 1),
(0000000017, 0000000030, 152, 1),
(0000000019, 0000000032, 166, 1),
(0000000020, 0000000033, 195, 1),
(0000000021, 0000000034, 234, 1),
(0000000022, 0000000035, 240, 1),
(0000000023, 0000000036, 246, 1),
(0000000025, 0000000038, 250, 1);

-- 
-- Dumping data for table osae_object
--
INSERT INTO osae_object VALUES
(0000000043, 'SYSTEM', 'Bob', 'SYSTEM', 0000000032, 0000000039, NULL, '', 0000000043, 1, '2015-09-17 03:13:10', '2015-06-18 06:21:27', 30),
(0000000130, 'Speech List', NULL, 'Text for main speech', NULL, 0000000069, NULL, '', 0000000043, 1, '2014-02-17 11:51:00', NULL, 30),
(0000001115, 'Custom Property List', NULL, 'Custom Property List', NULL, 0000000068, NULL, '', 0000000043, 1, '2014-02-17 11:51:00', NULL, 30),
(0000001196, 'House', NULL, '', 0000000184, 0000000136, NULL, '', NULL, 1, '2016-02-11 21:42:30', '2016-02-11 21:42:30', 30),
(0000001198, 'Unknown', '', 'Unknown Location', 0000000008, 0000000026, NULL, '', 0000001198, 1, '2015-07-28 15:34:39', '2015-07-28 03:05:59', 30),
(0000002777, 'Bedroom', NULL, '', 0000000181, 0000000135, NULL, '', 0000001196, 1, '2016-02-11 21:42:30', '2016-02-11 21:42:30', 30),
(0000002802, 'Bathroom', NULL, '', 0000000181, 0000000135, NULL, '', 0000001196, 1, '2015-12-22 22:46:17', '2015-12-22 22:46:17', 30),
(0000002803, 'Garage', NULL, '', 0000000181, 0000000135, NULL, '', 0000001196, 1, '2016-02-19 03:55:07', '2016-02-19 03:55:07', 30),
(0000002804, 'Kitchen', NULL, '', 0000000181, 0000000135, NULL, '', 0000001196, 1, '2016-01-06 15:27:22', '2016-01-06 15:27:22', 30),
(0000002805, 'Livingroom', NULL, '', 0000000181, 0000000135, NULL, '', 0000001196, 1, '2016-02-10 03:38:23', '2016-02-10 03:38:23', 30),
(0000002872, 'Unidentified Person', '', '', 0000000004, 0000000025, NULL, '', 0000001198, 0, '2016-02-15 16:06:04', '2016-02-15 16:06:04', 30),
(0000003055, 'Guest', '', 'Web UI user', 0000000004, 0000000025, NULL, '', 0000001198, 1, '2015-12-11 01:52:53', '2015-12-11 00:54:54', 30),
(0000003180, 'WUnderground', '', 'WUnderground plugin''s Object', 0000000132, 0000000105, NULL, '', NULL, 0, '2016-02-15 15:40:03', '2016-02-13 21:37:50', 50),
(0000003192, 'Bluetooth', '', 'Bluetooth plugin''s Object', 0000000022, 0000000035, NULL, '', NULL, 0, '2016-02-15 15:40:02', '2016-02-15 15:40:02', 50),
(0000003193, 'Email', '', 'Email plugin''s Object', 0000000048, 0000000049, NULL, '', NULL, 0, '2016-02-15 15:40:02', '2016-02-15 15:40:02', 50),
(0000003195, 'Network Monitor', '', 'Network Monitor plugin''s Object', 0000000026, 0000000037, NULL, '', NULL, 0, '2016-02-15 15:40:02', '2016-02-15 15:40:02', 50),
(0000003196, 'PowerShell', '', 'PowerShell plugin''s Object', 0000000128, 0000000103, NULL, '', NULL, 1, '2016-02-17 06:05:09', '2016-02-15 15:40:03', 50),
(0000003197, 'Rest', '', 'Rest plugin''s Object', 0000000133, 0000000106, NULL, '', NULL, 1, '2016-02-17 06:05:04', '2016-02-15 15:40:03', 50),
(0000003198, 'Script Processor', '', 'Script Processor plugin''s Object', 0000000064, 0000000060, NULL, '', NULL, 1, '2016-02-17 06:04:59', '2016-02-15 15:40:03', 50),
(0000003199, 'Speech', '', 'Speech plugin''s Object', 0000000058, 0000000058, NULL, '', NULL, 1, '2016-02-17 06:04:26', '2016-02-15 15:40:03', 50),
(0000003200, 'Web Server', '', 'Web Server plugin''s Object', 0000000106, 0000000083, NULL, '', NULL, 1, '2016-02-17 06:04:47', '2016-02-15 15:40:03', 50),
(0000003222, 'Weather', '', '', 0000000110, 0000000085, NULL, '', 0000000043, 1, '2016-02-19 05:42:13', '2016-02-19 05:42:13', 30);

-- 
-- Dumping data for table osae_object_type_event_script
--
INSERT INTO osae_object_type_event_script VALUES
(0000000006, 0000000025, 0000000003, 11, 1),
(0000000007, 0000000025, 0000000004, 12, 1);

-- 
-- Dumping data for table osae_object_type_property_option
--

-- Table osae.osae_object_type_property_option does not contain any data (it is empty)

-- 
-- Dumping data for table osae_event_log
--

-- Table osae.osae_event_log does not contain any data (it is empty)

-- 
-- Dumping data for table osae_method_log
--

-- Table osae.osae_method_log does not contain any data (it is empty)

-- 
-- Dumping data for table osae_method_queue
--
INSERT INTO osae_method_queue VALUES
(0000000398, '2016-02-15 16:06:04.029008', 0000003198, 0000000195, '12', 'Unidentified Person', 43, 'process_system_methods -> osae_sp_object_state_set -> event_log_add -> event_log_after_insert -> method_queue_add'),
(0000000399, '2016-02-17 06:04:26.907461', 0000003198, 0000000195, '24', 'SYSTEM', 43, 'system_count_plugins -> object_property_set -> event_log_add -> event_log_after_insert -> method_queue_add'),
(0000000400, '2016-02-17 06:04:47.932318', 0000003198, 0000000195, '24', 'SYSTEM', 43, 'system_count_plugins -> object_property_set -> event_log_add -> event_log_after_insert -> method_queue_add'),
(0000000401, '2016-02-17 06:04:59.945304', 0000003198, 0000000195, '24', 'SYSTEM', 43, 'system_count_plugins -> object_property_set -> event_log_add -> event_log_after_insert -> method_queue_add'),
(0000000402, '2016-02-17 06:05:04.946363', 0000003198, 0000000195, '24', 'SYSTEM', 43, 'system_count_plugins -> object_property_set -> event_log_add -> event_log_after_insert -> method_queue_add'),
(0000000403, '2016-02-17 06:05:09.940537', 0000003198, 0000000195, '24', 'SYSTEM', 43, 'system_count_plugins -> object_property_set -> event_log_add -> event_log_after_insert -> method_queue_add');

-- 
-- Dumping data for table osae_object_event_script
--
INSERT INTO osae_object_event_script VALUES
(0000000016, 0000000043, 0000000074, NULL, NULL),
(0000000017, 0000000043, 0000000075, NULL, NULL),
(0000000022, 0000000043, 0000000076, NULL, NULL),
(0000000023, 0000000043, 0000000077, NULL, NULL),
(0000000027, 0000000043, 0000000075, 13, 1),
(0000000028, 0000000043, 0000000076, 13, 1),
(0000000029, 0000000043, 0000000077, 13, 1),
(0000000033, 0000000043, 0000000074, 15, 1),
(0000000038, 0000000043, 0000000158, 24, 1);

-- 
-- Dumping data for table osae_object_property
--
INSERT INTO osae_object_property VALUES
(0000000083, 0000000043, 0000000068, '66102', NULL, NULL, 50, NULL, 0),
(0000000084, 0000000043, 0000000069, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000000085, 0000000043, 0000000070, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000000086, 0000000043, 0000000071, '2016-02-19', '2016-02-19 00:00:17', NULL, 50, 'SYSTEM', 0),
(0000000087, 0000000043, 0000000072, '05:51:52', '2016-02-19 05:51:52', NULL, 50, 'SYSTEM', 0),
(0000000088, 0000000043, 0000000073, '6', '2016-02-19 00:00:17', NULL, 50, 'SYSTEM', 0),
(0000000089, 0000000043, 0000000074, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000000193, 0000000130, 0000000216, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000194, 0000000130, 0000000217, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000195, 0000000130, 0000000218, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000265, 0000000130, 0000000220, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000329, 0000000130, 0000000223, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000330, 0000000130, 0000000224, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000332, 0000000130, 0000000226, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000334, 0000000043, 0000000228, '19', '2016-02-19 00:00:17', NULL, 50, 'SYSTEM', 0),
(0000000335, 0000000043, 0000000229, '05:51 AM', '2016-02-19 05:51:00', NULL, 50, 'SYSTEM', 0),
(0000000408, 0000000130, 0000000249, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000409, 0000000130, 0000000250, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000410, 0000000130, 0000000251, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000582, 0000000043, 0000000276, '0.4.8', '2016-02-12 19:29:55', NULL, 90, 'Vaughn Rupp', 0),
(0000002519, 0000000043, 0000000359, 'TRUE', '2015-09-16 23:22:41', NULL, 90, 'Vaughn', 0),
(0000002608, 0000000043, 0000000679, 'TRUE', NULL, NULL, 50, NULL, 0),
(0000002615, 0000001115, 0000000210, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000003240, 0000000130, 0000000835, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000003241, 0000000130, 0000000836, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000003248, 0000000130, 0000000837, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000003316, 0000000043, 0000000838, '9', '2016-02-15 16:02:35', NULL, 50, 'SYSTEM', 0),
(0000003317, 0000000043, 0000000839, '0', '2016-02-15 15:21:58', NULL, 50, 'SYSTEM', 0),
(0000003318, 0000000043, 0000000840, '5', '2016-02-17 06:05:09', NULL, 50, 'SYSTEM', 0),
(0000003319, 0000000043, 0000000841, '5', '2016-02-17 06:05:09', NULL, 50, 'SYSTEM', 0),
(0000003320, 0000000043, 0000000842, 'Web Server, PowerShell, Rest, Script Processor and Speech are Stopped!', '2016-02-19 05:51:52', NULL, 50, 'SYSTEM', 0),
(0000006277, 0000000130, 0000000944, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000006279, 0000000130, 0000000946, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000006418, 0000000043, 0000001041, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000006419, 0000000043, 0000001042, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000006420, 0000002777, 0000001003, '-1', '2015-12-31 23:52:13', NULL, 50, NULL, 0),
(0000006421, 0000002777, 0000001004, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006423, 0000002777, 0000001033, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006424, 0000002777, 0000001034, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006425, 0000002777, 0000001035, '100', '2015-05-16 20:10:04', NULL, 50, NULL, 0),
(0000006426, 0000002777, 0000001036, 'Nobody', '2016-02-11 21:42:29', NULL, 50, 'SYSTEM', 0),
(0000006427, 0000002802, 0000001003, '-1', '2015-12-31 23:52:13', NULL, 50, NULL, 0),
(0000006428, 0000002802, 0000001004, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006430, 0000002802, 0000001033, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006431, 0000002802, 0000001034, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006432, 0000002802, 0000001035, '100', '2015-05-16 20:09:02', NULL, 50, NULL, 0),
(0000006433, 0000002802, 0000001036, 'Nobody', '2015-06-18 05:25:18', NULL, 50, 'SYSTEM', 0),
(0000006434, 0000002803, 0000001003, '60', NULL, NULL, 90, 'Vaughn', 0),
(0000006435, 0000002803, 0000001004, '0', '2015-12-28 02:54:42', NULL, 90, 'Vaughn', 0),
(0000006437, 0000002803, 0000001033, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006438, 0000002803, 0000001034, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006439, 0000002803, 0000001035, '80', '2015-12-18 22:24:35', NULL, 90, 'Vaughn', 0),
(0000006440, 0000002803, 0000001036, 'Nobody', '2016-01-06 16:49:45', NULL, 50, 'SYSTEM', 0),
(0000006441, 0000002804, 0000001003, '60', NULL, NULL, 90, 'Vaughn', 0),
(0000006442, 0000002804, 0000001004, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006444, 0000002804, 0000001033, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006445, 0000002804, 0000001034, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006446, 0000002804, 0000001035, '100', '2015-05-16 20:10:30', NULL, 50, NULL, 0),
(0000006447, 0000002804, 0000001036, 'Nobody', '2015-06-17 16:25:22', NULL, 50, 'SYSTEM', 0),
(0000006448, 0000002805, 0000001003, '-1', '2015-12-31 23:52:13', NULL, 50, NULL, 0),
(0000006449, 0000002805, 0000001004, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006451, 0000002805, 0000001033, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006452, 0000002805, 0000001034, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006453, 0000002805, 0000001035, '80', '2015-05-16 20:16:35', NULL, 50, NULL, 0),
(0000006454, 0000002805, 0000001036, 'Nobody', '2016-02-10 03:38:22', NULL, 50, 'SYSTEM', 0),
(0000006563, 0000001196, 0000001010, '-1', '2015-12-31 23:52:13', NULL, 50, NULL, 0),
(0000006564, 0000001196, 0000001011, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006565, 0000001196, 0000001012, 'Nobody is here', '2016-02-11 21:42:29', NULL, 50, 'SYSTEM', 0),
(0000006566, 0000001196, 0000001013, '100', NULL, NULL, 50, NULL, 0),
(0000006568, 0000001196, 0000001032, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000006688, 0000000043, 0000001054, '50', NULL, NULL, 50, NULL, 0),
(0000006837, 0000001196, 0000001076, '0', '2016-02-11 21:42:29', NULL, 50, 'SYSTEM', 0),
(0000006840, 0000002777, 0000001079, '0', '2016-02-11 21:42:29', NULL, 50, '', 0),
(0000006841, 0000002802, 0000001079, '0', '2015-12-28 02:54:42', NULL, 50, '', 0),
(0000006842, 0000002803, 0000001079, '0', '2016-01-06 16:49:45', NULL, 50, '', 0),
(0000006843, 0000002804, 0000001079, '0', '2015-12-28 02:54:42', NULL, 50, '', 0),
(0000006844, 0000002805, 0000001079, '0', '2016-02-10 03:38:22', NULL, 50, '', 0),
(0000007180, 0000001198, 0000000675, '-1', '2015-12-31 23:52:13', NULL, 50, '', 0),
(0000007296, 0000001196, 0000001092, 'All rooms are vacant', '2016-02-11 21:42:29', NULL, 50, 'SYSTEM', 0),
(0000007297, 0000001196, 0000001093, '0', '2016-02-11 21:42:29', NULL, 50, 'SYSTEM', 0),
(0000007554, 0000000043, 0000001148, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000008407, 0000003055, 0000000026, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000008408, 0000003055, 0000000027, '', '2015-12-28 02:51:39', NULL, 50, '', 2),
(0000008409, 0000003055, 0000000028, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000008410, 0000003055, 0000000029, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000008411, 0000003055, 0000000270, '', '2015-12-28 02:51:39', NULL, 50, '', 25),
(0000008412, 0000003055, 0000000271, '', '2015-12-28 02:51:39', NULL, 50, '', 620),
(0000008413, 0000003055, 0000000272, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000008414, 0000003055, 0000000273, '', '2015-12-28 02:51:39', NULL, 50, '', 4),
(0000008415, 0000003055, 0000000676, '-1', '2015-12-31 23:52:13', NULL, 50, '', 0),
(0000008416, 0000003055, 0000001029, '', '2015-12-28 02:51:39', NULL, 50, '', 1),
(0000008417, 0000003055, 0000001053, '30', '2016-01-01 16:58:51', NULL, 90, 'Vaughn', 0),
(0000008418, 0000003055, 0000001056, 'Speech', '2015-12-11 00:54:54', NULL, 50, '', 0),
(0000008419, 0000003055, 0000001073, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000008420, 0000003055, 0000001074, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000008421, 0000003055, 0000001322, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000009794, 0000003180, 0000000813, 'KS', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009795, 0000003180, 0000000814, '180', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009796, 0000003180, 0000000815, 'FALSE', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009797, 0000003180, 0000000816, '0', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009798, 0000003180, 0000000817, '0', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009799, 0000003180, 0000000818, '0', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009800, 0000003180, 0000000819, '0', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009801, 0000003180, 0000001154, '', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009802, 0000003180, 0000001155, 'Ellis', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009803, 0000003180, 0000001156, '15', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009804, 0000003180, 0000001328, '90', '2016-02-15 16:09:58', NULL, 50, 'vaughn rupp', 0),
(0000009805, 0000003180, 0000001350, 'FALSE', '2016-02-13 21:37:50', NULL, 50, '', 0),
(0000009806, 0000003180, 0000001374, '0.4.8', '2016-02-15 15:40:03', NULL, 50, 'SERVICE-PowerSpec', 0),
(0000009807, 0000003180, 0000001400, 'Brian, Automate, Vaughn', '2016-02-15 15:40:03', NULL, 50, 'SERVICE-PowerSpec', 0),
(0000009847, 0000003192, 0000000060, '60', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009848, 0000003192, 0000000061, '8', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009849, 0000003192, 0000000062, 'TRUE', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009850, 0000003192, 0000000608, 'FALSE', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009851, 0000003192, 0000000936, 'FALSE', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009852, 0000003192, 0000001330, '90', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009853, 0000003192, 0000001355, '', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009854, 0000003192, 0000001383, '', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009855, 0000003193, 0000000104, '', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009856, 0000003193, 0000000105, '25', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009857, 0000003193, 0000000106, 'TRUE', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009858, 0000003193, 0000000107, '', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009859, 0000003193, 0000000108, '', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009860, 0000003193, 0000000109, '', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009861, 0000003193, 0000000611, 'TRUE', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009862, 0000003193, 0000001333, '90', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009863, 0000003193, 0000001358, '', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009864, 0000003193, 0000001380, 'FALSE', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009865, 0000003193, 0000001386, '', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009873, 0000003195, 0000000609, 'TRUE', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009874, 0000003195, 0000000677, '30', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009875, 0000003195, 0000001351, 'FALSE', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009876, 0000003195, 0000001362, '', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009877, 0000003195, 0000001390, '', '2016-02-15 15:40:02', NULL, 50, '', 0),
(0000009878, 0000003196, 0000000701, 'TRUE', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009879, 0000003196, 0000001336, '90', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009880, 0000003196, 0000001364, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009881, 0000003196, 0000001392, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009882, 0000003197, 0000000820, 'TRUE', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009883, 0000003197, 0000000822, 'TRUE', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009884, 0000003197, 0000001085, '8732', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009885, 0000003197, 0000001366, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009886, 0000003197, 0000001394, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009887, 0000003197, 0000001402, '90', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009888, 0000003198, 0000000613, 'TRUE', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009889, 0000003198, 0000000934, 'FALSE', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009890, 0000003198, 0000001329, '90', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009891, 0000003198, 0000001367, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009892, 0000003198, 0000001395, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009893, 0000003199, 0000000254, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009894, 0000003199, 0000000255, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009895, 0000003199, 0000000612, 'FALSE', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009896, 0000003199, 0000000684, '0', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009897, 0000003199, 0000000685, '0', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009898, 0000003199, 0000000694, 'FALSE', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009899, 0000003199, 0000000932, 'FALSE', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009900, 0000003199, 0000001338, '90', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009901, 0000003199, 0000001368, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009902, 0000003199, 0000001396, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009903, 0000003200, 0000000620, 'TRUE', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009904, 0000003200, 0000000866, '600', '2016-02-19 02:56:21', NULL, 90, 'vaughn rupp', 0),
(0000009905, 0000003200, 0000001373, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009906, 0000003200, 0000001399, '', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009907, 0000003200, 0000001403, 'FALSE', '2016-02-15 15:40:03', NULL, 50, '', 0),
(0000009908, 0000002872, 0000000026, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009909, 0000002872, 0000000027, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009910, 0000002872, 0000000028, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009911, 0000002872, 0000000029, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009912, 0000002872, 0000000270, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009913, 0000002872, 0000000271, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009914, 0000002872, 0000000272, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009915, 0000002872, 0000000273, '', '2016-02-15 16:05:57', NULL, 50, '', 3),
(0000009916, 0000002872, 0000000676, '-1', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009917, 0000002872, 0000001029, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009918, 0000002872, 0000001053, '90', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009919, 0000002872, 0000001056, 'Speech', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009920, 0000002872, 0000001073, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009921, 0000002872, 0000001074, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009922, 0000002872, 0000001322, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000010120, 0000003222, 0000000467, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010121, 0000003222, 0000000468, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010122, 0000003222, 0000000469, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010123, 0000003222, 0000000470, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010124, 0000003222, 0000000471, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010125, 0000003222, 0000000472, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010126, 0000003222, 0000000473, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010127, 0000003222, 0000000474, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010128, 0000003222, 0000000475, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010129, 0000003222, 0000000476, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010130, 0000003222, 0000000477, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010131, 0000003222, 0000000478, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010132, 0000003222, 0000000479, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010133, 0000003222, 0000000480, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010134, 0000003222, 0000000481, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010135, 0000003222, 0000000482, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010136, 0000003222, 0000000483, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010137, 0000003222, 0000000484, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010138, 0000003222, 0000000485, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010139, 0000003222, 0000000486, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010140, 0000003222, 0000000487, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010141, 0000003222, 0000000488, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010142, 0000003222, 0000000489, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010143, 0000003222, 0000000490, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010144, 0000003222, 0000000491, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010145, 0000003222, 0000000492, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010146, 0000003222, 0000000493, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010147, 0000003222, 0000000494, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010148, 0000003222, 0000000495, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010149, 0000003222, 0000000496, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010150, 0000003222, 0000000497, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010151, 0000003222, 0000000498, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010152, 0000003222, 0000000499, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010153, 0000003222, 0000000500, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010154, 0000003222, 0000000501, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010155, 0000003222, 0000000502, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010156, 0000003222, 0000000503, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010157, 0000003222, 0000000504, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010158, 0000003222, 0000000505, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010159, 0000003222, 0000000506, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010160, 0000003222, 0000000507, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010161, 0000003222, 0000000508, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010162, 0000003222, 0000000509, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010163, 0000003222, 0000000510, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010164, 0000003222, 0000000511, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010165, 0000003222, 0000000512, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010166, 0000003222, 0000000513, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010167, 0000003222, 0000000514, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010168, 0000003222, 0000000515, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010169, 0000003222, 0000000516, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010170, 0000003222, 0000000517, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010171, 0000003222, 0000000518, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010172, 0000003222, 0000000519, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010173, 0000003222, 0000000520, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010174, 0000003222, 0000000521, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010175, 0000003222, 0000000522, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010176, 0000003222, 0000000523, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010177, 0000003222, 0000000524, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010178, 0000003222, 0000000525, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010179, 0000003222, 0000000526, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010180, 0000003222, 0000000527, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010181, 0000003222, 0000000528, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010182, 0000003222, 0000000529, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010183, 0000003222, 0000000530, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010184, 0000003222, 0000000531, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010185, 0000003222, 0000000532, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010186, 0000003222, 0000000533, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010187, 0000003222, 0000000534, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010188, 0000003222, 0000000535, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010189, 0000003222, 0000000536, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010190, 0000003222, 0000000537, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010191, 0000003222, 0000000538, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010192, 0000003222, 0000000539, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010193, 0000003222, 0000000540, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010194, 0000003222, 0000000541, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010195, 0000003222, 0000000542, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010196, 0000003222, 0000000543, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010197, 0000003222, 0000000544, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010198, 0000003222, 0000000545, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010199, 0000003222, 0000000546, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010200, 0000003222, 0000000547, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010201, 0000003222, 0000000548, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010202, 0000003222, 0000000549, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010203, 0000003222, 0000000550, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010204, 0000003222, 0000000551, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010205, 0000003222, 0000000552, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010206, 0000003222, 0000000553, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010207, 0000003222, 0000000554, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010208, 0000003222, 0000000555, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010209, 0000003222, 0000000570, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010210, 0000003222, 0000000571, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010211, 0000003222, 0000000572, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010212, 0000003222, 0000000573, '0', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010213, 0000003222, 0000000809, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010214, 0000003222, 0000000810, '', '2016-02-19 05:42:13', NULL, 50, '', 0),
(0000010215, 0000003222, 0000000811, '', '2016-02-19 05:42:13', NULL, 50, '', 0);

-- 
-- Dumping data for table osae_object_state_history
--

-- Table osae.osae_object_state_history does not contain any data (it is empty)

-- 
-- Dumping data for table osae_schedule_queue
--

-- Table osae.osae_schedule_queue does not contain any data (it is empty)

-- 
-- Dumping data for table osae_schedule_recurring
--

-- Table osae.osae_schedule_recurring does not contain any data (it is empty)

-- 
-- Dumping data for table osae_screen_object
--

-- Table osae.osae_screen_object does not contain any data (it is empty)

-- 
-- Dumping data for table osae_object_property_array
--
INSERT INTO osae_object_property_array VALUES
(0000000010, 0000000194, 'Warning.  Security is active.', ''),
(0000000011, 0000000194, 'You should have disarmed the alarm before entering.', ''),
(0000000012, 0000000194, 'Warning.  You must disarm the alarm.', ''),
(0000000013, 0000000195, 'Boo!', ''),
(0000000014, 0000000195, 'I am a scary talking door.', ''),
(0000000015, 0000000195, 'I am an evil door and I will eat you.', ''),
(0000000016, 0000000193, 'Well Hi there', ''),
(0000000017, 0000000193, 'Greetings.', ''),
(0000000018, 0000000193, 'Hello.', ''),
(0000000021, 0000000265, 'Hey there', ''),
(0000000022, 0000000265, 'Well Hello', ''),
(0000000023, 0000000265, 'Hi There', ''),
(0000000024, 0000000265, 'Hello Yourself', ''),
(0000000026, 0000000329, 'You are welcome', ''),
(0000000027, 0000000329, 'No problem', ''),
(0000000028, 0000000329, 'yup', ''),
(0000000029, 0000000329, 'my pleasure', ''),
(0000000030, 0000000329, 'any time', ''),
(0000000031, 0000000329, 'that is what I am here for', ''),
(0000000032, 0000000329, 'You''re welcome', ''),
(0000000033, 0000000329, 'sure boss', ''),
(0000000034, 0000000329, 'anything for you', ''),
(0000000035, 0000000329, 'I live to serve', ''),
(0000000036, 0000000329, 'not a problem', ''),
(0000000037, 0000000329, 'absolutely', ''),
(0000000038, 0000000329, 'roger', ''),
(0000000070, 0000000408, 'Hello.  Mr. Mail man.', ''),
(0000000071, 0000000408, 'Yay.  I got mail.', ''),
(0000000072, 0000000408, 'I love mail.', ''),
(0000000073, 0000000408, 'There better not be any junk mail in there.', ''),
(0000000154, 0000000193, 'Front Door Opened', ''),
(0000000155, 0000000193, 'Hey there.', ''),
(0000000156, 0000000193, 'I am Not sure if you are coming or going', ''),
(0000000157, 0000000193, 'If I had my way there would be a screen door on there', ''),
(0000000158, 0000000193, 'Don''t let the door hit you in the ass', ''),
(0000000159, 0000000193, 'I need more sensors', ''),
(0000000160, 0000000193, 'Just come and go as you please', ''),
(0000000161, 0000000193, 'It sounds like I could use better speakers', ''),
(0000000162, 0000000193, 'Welcome to Eric''s house', ''),
(0000000163, 0000000193, 'Is it time to smoke', ''),
(0000000164, 0000000193, 'Hello, I am the smartest house in the hood.', ''),
(0000000165, 0000000193, 'Hi, My name is Angel.  Let me know if I can be of assistance.', ''),
(0000000166, 0000000193, 'Damn people are always coming and going.', ''),
(0000000167, 0000000193, 'Do not ignore me.  It is not polite.', ''),
(0000000168, 0000000193, 'I am sick of that fracking door already.', ''),
(0000000169, 0000000193, 'I want to go on the porch.', ''),
(0000000170, 0000000193, 'Leave that door open, I need some fresh air.', ''),
(0000000268, 0000000193, 'I wish I had control over that door lock.', ''),
(0000000269, 0000000193, 'You are killing me with that door already', ''),
(0000000270, 0000000193, 'There is only one sensor on the door, what do you expect from me.', ''),
(0000000271, 0000000193, 'There is nothing good out there, just stay in here.', ''),
(0000000272, 0000000193, 'It sucks outside, just hang tight.', ''),
(0000000273, 0000000193, 'It is [Weather Data.Temp] degrees out there.', ''),
(0000000274, 0000000193, 'close the door, it is [Weather Data.Temp] degrees out', ''),
(0000000275, 0000000193, 'Front Door opened at [SYSTEM.Time AMPM]', ''),
(0000000277, 0000000265, 'Why hello there', 'Why hello there'),
(0000000278, 0000000265, 'Howdy', 'Howdy'),
(0000000280, 0000000265, 'Hola', 'Hola'),
(0000000391, 0000003241, 'So you are going to steal my food?', ''),
(0000000392, 0000003241, 'Refridgerator has been comprimised.', ''),
(0000000393, 0000003241, 'Some of that food is poisoned.   Eat up.', ''),
(0000000396, 0000003240, 'It is [SYSTEM.Time AMPM] and you are in the refrigerator again.', ''),
(0000000400, 0000003248, 'You can''t be in here!', ''),
(0000000401, 0000003248, 'Hey!  Get out of here!', ''),
(0000000402, 0000003248, 'Intruder Alert!', ''),
(0000000403, 0000003248, 'I called for help!', ''),
(0000000688, 0000000330, 'The temperature is [Weather.Temp] degrees.', ''),
(0000000690, 0000000330, '[Weather.Today Summary]', ''),
(0000000701, 0000006277, 'It is [SYSTEM.Time AMPM]', ''),
(0000000710, 0000006279, 'Yes', ''),
(0000000711, 0000006279, 'Yes master', ''),
(0000000712, 0000006279, 'I am listening', ''),
(0000000713, 0000006279, 'Yeah', ''),
(0000000714, 0000006279, 'I''m listening', ''),
(0000000934, 0000007554, 'gggg', 'fffff');

-- 
-- Dumping data for table osae_object_property_history
--

-- Table osae.osae_object_property_history does not contain any data (it is empty)

DELIMITER $$

--
-- Definition for trigger osae_tr_method_queue_before_delete
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER osae_tr_method_queue_before_delete
	BEFORE DELETE
	ON osae_method_queue
	FOR EACH ROW
BEGIN
  INSERT INTO osae_method_log (entry_time,object_id,method_id,parameter_1,parameter_2,from_object_id,debug_trace) VALUES(OLD.entry_time,OLD.object_id,OLD.method_id,OLD.parameter_1,OLD.parameter_2,OLD.from_object_id,CONCAT(OLD.debug_trace,' -> method_queue_before_delete'));
END
$$

--
-- Definition for trigger osae_tr_object_after_insert
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER osae_tr_object_after_insert
	AFTER INSERT
	ON osae_object
	FOR EACH ROW
BEGIN
DECLARE vPropertyID INT;
DECLARE vDefault VARCHAR(255);
DECLARE done INT DEFAULT 0;  
DECLARE cur1 CURSOR FOR SELECT property_id,property_default FROM osae_object_type_property WHERE object_type_id=NEW.`object_type_id`;
DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    OPEN cur1; 
    REPEAT
        FETCH cur1 INTO vPropertyID,vDefault;
        IF NOT done THEN 
            INSERT INTO osae_object_property (object_id,object_type_property_id,property_value) VALUES(NEW.`object_id`,vPropertyID,vDefault);
        END IF;
    UNTIL done END REPEAT;
    CLOSE cur1;
    INSERT INTO osae_object_state_history (object_id,state_id) SELECT NEW.`object_id`, state_id FROM osae_v_object_state WHERE object_id=NEW.object_id;        
END
$$

--
-- Definition for trigger osae_tr_object_after_update
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER osae_tr_object_after_update
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
-- Definition for trigger osae_tr_object_before_insert
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER osae_tr_object_before_insert
	BEFORE INSERT
	ON osae_object
	FOR EACH ROW
BEGIN
DECLARE iState INT;
    IF ISNULL(NEW.state_id) THEN
        SELECT state_id INTO iState FROM osae_object_type_state WHERE object_type_id=NEW.object_type_id AND state_name="OFF";
        IF ISNULL(iState) THEN
            SELECT state_id INTO iState FROM osae_object_type_state WHERE object_type_id=NEW.object_type_id LIMIT 1;
         END IF;
        SET NEW.state_id=iState;
    END IF;
END
$$

--
-- Definition for trigger osae_tr_object_before_update
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER osae_tr_object_before_update
	BEFORE UPDATE
	ON osae_object
	FOR EACH ROW
BEGIN
DECLARE vState VARCHAR(50);
DECLARE vEventCount INT;
DECLARE vHideRedundantRvents INT;
DECLARE vEvent VARCHAR(200);
DECLARE vSystemID INT;
DECLARE vPlaceID INT;
DECLARE vContainer VARCHAR(200);
DECLARE vContainerType VARCHAR(200);
DECLARE vPlaceOnID INT;
DECLARE vPersonID INT;
DECLARE vContainedX INT;
DECLARE vContainedY INT;
    IF OLD.object_type_id <> NEW.object_type_id THEN
        DELETE FROM osae_object_property WHERE object_id=OLD.object_id;
        INSERT INTO osae_object_property (object_id,object_type_property_id,property_value) SELECT OLD.object_id, property_id, property_default FROM osae_object_type_property WHERE object_type_id=NEW.object_type_id;
        
        DELETE FROM osae_object_state_history WHERE object_id=OLD.object_id;
        INSERT INTO osae_object_state_history (object_id,state_id) SELECT OLD.object_id, state_id FROM osae_object_type_state WHERE object_type_id=NEW.object_type_id;        
        
        #DELETE FROM osae_object_property WHERE object_id=OLD.object_id;
        #INSERT INTO osae_object_property (object_id,object_type_property_id,property_value) SELECT OLD.object_id, property_id, property_default FROM osae_object_type_property WHERE object_type_id=NEW.object_type_id;
        DELETE FROM osae_object_event_script WHERE object_id=OLD.object_id;
    ELSE
        #Changing an Object's Type is so severe, we are only going to check containers and states if the object type is the same
        SET vContainerType = (SELECT base_type FROM osae_v_object WHERE object_id=NEW.container_id);
        SET vPersonID = (SELECT object_type_id FROM osae_object_type WHERE object_type='PERSON');
        IF OLD.object_type_id = vPersonID AND OLD.container_id <> NEW.container_id AND vContainerType='PLACE' THEN
            SET vContainer = (SELECT object_name FROM osae_v_object WHERE object_id=NEW.container_id);
            CALL osae_sp_method_queue_add (vContainer,'ON','','','SYSTEM','Auto-Occupancy Logic');
        END IF;
        IF OLD.state_id <> NEW.state_id THEN
            SET NEW.last_state_change=NOW();
            UPDATE osae_object_state_history SET times_this_hour=times_this_hour + 1, times_this_day=times_this_day + 1,times_this_month=times_this_month+1,times_this_year=times_this_year+1,times_ever=times_ever + 1 WHERE object_id=OLD.object_id AND state_id=NEW.state_id;
            INSERT INTO osae_object_state_change_history (object_id, state_id) VALUES (OLD.object_id, NEW.state_id);
        END IF;      
    END IF;

    IF OLD.object_name <> NEW.object_name THEN
      UPDATE osae_object_property SET property_value=NEW.object_name WHERE property_value=OLD.object_name;
    END IF; 

  
END
$$

--
-- Definition for trigger osae_tr_object_property_after_update
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER osae_tr_object_property_after_update
	AFTER UPDATE
	ON osae_object_property
	FOR EACH ROW
BEGIN
  DECLARE vTrack Boolean;
    SELECT track_history INTO vTrack FROM osae_v_object_type_property WHERE property_id=NEW.object_type_property_id;
    IF vTrack THEN
      INSERT INTO osae_object_property_history (object_property_id,property_value) VALUES(NEW.object_property_id,NEW.property_value);
    END IF;
END
$$

--
-- Definition for trigger osae_tr_object_property_before_update
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER osae_tr_object_property_before_update
	BEFORE UPDATE
	ON osae_object_property
	FOR EACH ROW
BEGIN
  IF OLD.property_value != NEW.property_value THEN
    SET NEW.Last_Updated=NOW();
  END IF;
END
$$

--
-- Definition for trigger osae_tr_object_type_properties_after_insert
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER osae_tr_object_type_properties_after_insert
	AFTER INSERT
	ON osae_object_type_property
	FOR EACH ROW
BEGIN
    INSERT INTO osae_object_property (object_id,object_type_property_id,property_value) SELECT object_id,NEW.property_id,NEW.property_default FROM osae_object WHERE object_type_id=NEW.object_type_id;
END
$$

--
-- Definition for trigger tr_osae_event_log_after_insert
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER tr_osae_event_log_after_insert
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

--
-- Definition for trigger tr_recurring_after_insert
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER tr_recurring_after_insert
	AFTER INSERT
	ON osae_schedule_recurring
	FOR EACH ROW
BEGIN
    CALL osae_sp_process_recurring;
END
$$

--
-- Definition for trigger tr_recurring_after_update
--
CREATE 
	DEFINER = 'osae'@'%'
TRIGGER tr_recurring_after_update
	AFTER UPDATE
	ON osae_schedule_recurring
	FOR EACH ROW
BEGIN
    CALL osae_sp_process_recurring;
END
$$

DELIMITER ;

-- 
-- Restore previous SQL mode
-- 
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;

-- 
-- Enable foreign keys
-- 
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;