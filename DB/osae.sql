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
AUTO_INCREMENT = 3786
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
AUTO_INCREMENT = 129
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
AUTO_INCREMENT = 1288
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
AUTO_INCREMENT = 73019
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
AUTO_INCREMENT = 1414
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
AUTO_INCREMENT = 3206
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
AUTO_INCREMENT = 73
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
AUTO_INCREMENT = 30
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
AUTO_INCREMENT = 399
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
AUTO_INCREMENT = 9951
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
AUTO_INCREMENT = 46
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
AUTO_INCREMENT = 4815
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
                UPDATE osae_object SET container_id = vContainerID WHERE object_name=vObjectName;
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
-- Definition for view osae_v_object
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object
AS
	select `osae_object`.`object_id` AS `object_id`,`osae_object`.`object_name` AS `object_name`,`osae_object`.`object_alias` AS `object_alias`,`osae_object`.`object_description` AS `object_description`,`osae_object`.`object_value` AS `object_value`,`osae_object`.`address` AS `address`,`osae_object`.`last_updated` AS `last_updated`,`osae_object`.`last_state_change` AS `last_state_change`,`osae_object`.`min_trust_level` AS `min_trust_level`,`osae_object`.`enabled` AS `enabled`,`osae_object_type`.`object_type_id` AS `object_type_id`,`osae_object_type`.`object_type` AS `object_type`,`osae_object_type`.`object_type_description` AS `object_type_description`,`osae_object_type`.`plugin_object_id` AS `plugin_object_id`,`osae_object_type`.`system_hidden` AS `system_hidden`,`osae_object_type`.`object_type_owner` AS `object_type_owner`,`osae_object_type`.`base_type_id` AS `base_type_id`,`osae_object_type`.`container` AS `container`,`osae_object_type_state`.`state_id` AS `state_id`,coalesce(`osae_object_type_state`.`state_name`,'') AS `state_name`,coalesce(`osae_object_type_state`.`state_label`,'') AS `state_label`,`objects_2`.`object_name` AS `owned_by`,`object_types_2`.`object_type` AS `base_type`,`objects_1`.`object_name` AS `container_name`,`osae_object`.`container_id` AS `container_id`,timestampdiff(SECOND,`osae_object`.`last_state_change`,now()) AS `time_in_state` from (((((`osae_object` left join `osae_object_type` on((`osae_object`.`object_type_id` = `osae_object_type`.`object_type_id`))) left join `osae_object_type` `object_types_2` on((`osae_object_type`.`base_type_id` = `object_types_2`.`object_type_id`))) left join `osae_object` `objects_2` on((`osae_object_type`.`plugin_object_id` = `objects_2`.`object_id`))) left join `osae_object_type_state` on(((`osae_object_type`.`object_type_id` = `osae_object_type_state`.`object_type_id`) and (`osae_object_type_state`.`state_id` = `osae_object`.`state_id`)))) left join `osae_object` `objects_1` on((`objects_1`.`object_id` = `osae_object`.`container_id`)));

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
-- Definition for view osae_v_object_list_full
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_list_full
AS
	select `osae_v_object`.`base_type` AS `base_type`,`osae_v_object`.`object_type` AS `object_type`,`osae_v_object`.`object_name` AS `object_name`,`osae_v_object`.`container` AS `container` from `osae_v_object` where (`osae_v_object`.`base_type` not in ('CONTROL','SCREEN','LIST')) union select `osae_v_object`.`base_type` AS `base_type`,`osae_v_object`.`object_type` AS `object_type`,`osae_v_object`.`object_alias` AS `object_name`,`osae_v_object`.`container` AS `container` from `osae_v_object` where ((`osae_v_object`.`object_alias` is not null) and (`osae_v_object`.`object_alias` <> '') and (`osae_v_object`.`base_type` not in ('CONTROL','SCREEN','LIST'))) order by `base_type`,`object_type`,`object_name`;

--
-- Definition for view osae_v_object_property_scraper
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_property_scraper
AS
	select `osae_object_property_scraper`.`object_property_scraper_id` AS `object_property_scraper_id`,`osae_object_property_scraper`.`object_property_id` AS `object_property_id`,`osae_object_property_scraper`.`URL` AS `URL`,`osae_object_property_scraper`.`search_prefix` AS `search_prefix`,`osae_object_property_scraper`.`search_prefix_offset` AS `search_prefix_offset`,`osae_object_property_scraper`.`search_suffix` AS `search_suffix`,`osae_object_property_scraper`.`last_updated` AS `last_updated`,subtime(now(),`osae_object_property_scraper`.`update_interval`) AS `update_due`,`osae_object_property_scraper`.`update_interval` AS `update_interval`,`osae_v_object_property`.`object_id` AS `object_id`,`osae_v_object_property`.`object_name` AS `object_name`,`osae_v_object_property`.`object_alias` AS `object_alias`,`osae_v_object_property`.`object_description` AS `object_description`,`osae_v_object_property`.`state_id` AS `state_id`,`osae_v_object_property`.`address` AS `address`,`osae_v_object_property`.`container_id` AS `container_id`,`osae_v_object_property`.`property_name` AS `property_name`,`osae_v_object_property`.`property_default` AS `property_default`,`osae_v_object_property`.`property_datatype` AS `property_datatype`,`osae_v_object_property`.`object_type_id` AS `object_type_id`,`osae_v_object_property`.`object_type_description` AS `object_type_description`,`osae_v_object_property`.`object_type` AS `object_type`,`osae_v_object_property`.`property_value` AS `property_value`,`osae_v_object_property`.`source_name` AS `source_name`,`osae_v_object_property`.`interest_level` AS `interest_level`,`osae_v_object_property`.`trust_level` AS `trust_level` from (`osae_object_property_scraper` left join `osae_v_object_property` on((`osae_object_property_scraper`.`object_property_id` = `osae_v_object_property`.`object_property_id`)));

--
-- Definition for view osae_v_object_type_state_list_full
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_type_state_list_full
AS
	select `osae_v_object_type_state`.`base_type` AS `base_type`,`osae_v_object_type_state`.`object_type` AS `object_type`,`osae_v_object_type_state`.`state_label` AS `state_label` from `osae_v_object_type_state` where ((`osae_v_object_type_state`.`base_type` not in ('CONTROL','SCREEN','LIST')) and `osae_v_object_type_state`.`object_type` in (select distinct `osae_v_object`.`object_type` from `osae_v_object`));

--
-- Definition for view osae_v_screen_object
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_screen_object
AS
	select `so`.`screen_object_id` AS `screen_object_id`,`so`.`screen_id` AS `screen_id`,`so`.`object_id` AS `object_id`,`so`.`control_id` AS `control_id`,`screen`.`object_name` AS `screen_name`,`co`.`enabled` AS `control_enabled`,`co`.`object_name` AS `control_name`,`oo`.`object_name` AS `object_name`,`ot`.`object_type` AS `control_type`,`oo`.`last_updated` AS `last_updated`,`oo`.`last_state_change` AS `last_state_change`,timestampdiff(MINUTE,`oo`.`last_state_change`,now()) AS `time_in_state`,`ots`.`state_name` AS `state_name`,(select max(`osae_v_object_property`.`last_updated`) from `osae_v_object_property` where ((`osae_v_object_property`.`object_id` = `oo`.`object_id`) and (`osae_v_object_property`.`property_name` <> 'Time'))) AS `property_last_updated` from (((((`osae_screen_object` `so` join `osae_object` `screen` on((`screen`.`object_id` = `so`.`screen_id`))) join `osae_object` `oo` on((`so`.`object_id` = `oo`.`object_id`))) join `osae_object` `co` on((`so`.`control_id` = `co`.`object_id`))) join `osae_object_type` `ot` on((`ot`.`object_type_id` = `co`.`object_type_id`))) left join `osae_object_type_state` `ots` on((`ots`.`state_id` = `oo`.`state_id`)));

--
-- Definition for view osae_v_system_occupied_rooms
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_system_occupied_rooms
AS
	select `o1`.`object_name` AS `room`,count(`o2`.`object_name`) AS `occupant_count` from (`osae_v_object` `o1` left join `osae_v_object` `o2` on(((`o1`.`object_id` = `o2`.`container_id`) and (`o2`.`base_type` = 'PERSON')))) where (`o1`.`object_type` = 'ROOM') group by `o1`.`object_name`;

--
-- Definition for view osae_v_object_property_scraper_ready
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_object_property_scraper_ready
AS
	select `osae_v_object_property_scraper`.`object_property_scraper_id` AS `object_property_scraper_id`,`osae_v_object_property_scraper`.`object_property_id` AS `object_property_id`,`osae_v_object_property_scraper`.`URL` AS `URL`,`osae_v_object_property_scraper`.`search_prefix` AS `search_prefix`,`osae_v_object_property_scraper`.`search_prefix_offset` AS `search_prefix_offset`,`osae_v_object_property_scraper`.`search_suffix` AS `search_suffix`,`osae_v_object_property_scraper`.`last_updated` AS `last_updated`,`osae_v_object_property_scraper`.`update_due` AS `update_due`,`osae_v_object_property_scraper`.`update_interval` AS `update_interval`,`osae_v_object_property_scraper`.`object_id` AS `object_id`,`osae_v_object_property_scraper`.`object_name` AS `object_name`,`osae_v_object_property_scraper`.`object_alias` AS `object_alias`,`osae_v_object_property_scraper`.`object_description` AS `object_description`,`osae_v_object_property_scraper`.`state_id` AS `state_id`,`osae_v_object_property_scraper`.`address` AS `address`,`osae_v_object_property_scraper`.`container_id` AS `container_id`,`osae_v_object_property_scraper`.`property_name` AS `property_name`,`osae_v_object_property_scraper`.`property_default` AS `property_default`,`osae_v_object_property_scraper`.`property_datatype` AS `property_datatype`,`osae_v_object_property_scraper`.`object_type_id` AS `object_type_id`,`osae_v_object_property_scraper`.`object_type_description` AS `object_type_description`,`osae_v_object_property_scraper`.`object_type` AS `object_type`,`osae_v_object_property_scraper`.`property_value` AS `property_value`,`osae_v_object_property_scraper`.`source_name` AS `source_name`,`osae_v_object_property_scraper`.`interest_level` AS `interest_level`,`osae_v_object_property_scraper`.`trust_level` AS `trust_level` from `osae_v_object_property_scraper` where (`osae_v_object_property_scraper`.`last_updated` < `osae_v_object_property_scraper`.`update_due`);

--
-- Definition for view osae_v_screen_updates
--
CREATE OR REPLACE 
	DEFINER = 'osae'@'%'
VIEW osae_v_screen_updates
AS
	select `osae_v_screen_object`.`screen_object_id` AS `screen_object_id`,`osae_v_screen_object`.`screen_id` AS `screen_id`,`osae_v_screen_object`.`object_id` AS `object_id`,`osae_v_screen_object`.`control_id` AS `control_id`,`osae_v_screen_object`.`screen_name` AS `screen_name`,`osae_v_screen_object`.`control_name` AS `control_name`,`osae_v_screen_object`.`control_enabled` AS `control_enabled`,`osae_v_screen_object`.`object_name` AS `object_name`,`osae_v_screen_object`.`last_updated` AS `last_updated`,`osae_v_screen_object`.`last_state_change` AS `last_state_change`,`osae_v_screen_object`.`time_in_state` AS `time_in_state`,`osae_object_type_state`.`state_name` AS `state_name`,`osae_object_type_state`.`state_label` AS `state_label`,`osae_object_type_property`.`property_name` AS `property_name`,`osae_object_property`.`property_value` AS `property_value`,`osae_object_property`.`last_updated` AS `property_last_updated`,`osae_v_screen_object`.`control_type` AS `control_type` from ((((`osae_object` left join `osae_object_type_state` on((`osae_object`.`state_id` = `osae_object_type_state`.`state_id`))) join `osae_v_screen_object` on((`osae_object`.`object_id` = `osae_v_screen_object`.`object_id`))) left join `osae_object_type_property` on((`osae_object_type_property`.`object_type_id` = `osae_object`.`object_type_id`))) join `osae_object_property` on((`osae_object_type_property`.`property_id` = `osae_object_property`.`object_type_property_id`))) where ((`osae_v_screen_object`.`last_updated` > subtime(now(),'00:00:30')) or (`osae_object_property`.`last_updated` > subtime(now(),'00:00:30')));

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
(128, x'89504E470D0A1A0A0000000D49484452000003E80000028A0802000000218AE656000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000FFA549444154785EECFD677C1D55BAFF0BAEAADA41395BC9926539CA0103066330199A4C9FCE81D481D0A0B9FFCF9D17F7FF7E66DECD99F99C7BE79EF9CC91C9D04DD3409F86263674D30DB4A1494E38E028CB922C5956CEDAA9AAE6799E557B5B960396D992F6DEFA7D55AABD6A555AB56A55ADDF7AEAA955466363A302000000000000A436A6F70B000000000000486120DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC010000000000480320DC01000000000048038CC6C6462F08320147C6A672E9DCEA11002039741C6B1B191C89D185651AA663BA96AB1CD7300CD7A54B6D8ED1C9700D65BA8EAB7C346D99AAA6A6262F2F07069AF483EFDD741E4D434A966D288BEEEDAEE91A34360C65D089E67BBB4B5334919237FA49A9A2EB83122DD5138A62323999C712E211FD1329581E40F280704F63B89A96DBA14CD8AE61D1AFADA8D2266254791BB851029014A45E6C6A6AF226D3848D1B37AE5B7F29A9786F1AA40D74EB7662CAF0296A1DCAF9F37E186E28F2AD9F6FEFACD35C9B9A697A564A314943A2269A1974CB4D02F423E502599DF940B8A73DA7CA77FA775CC3A40BD884C51D8024A125C8E6A6A6ACA0FF473FFAD1D0F0285BDB4D524B0E41B34CD3A4008D5954CD16A75CFB824418812CABB7B7FF9F1F7E74F1C5176FDA7485EB5A5316036900DFBE1D2A642CC3D8DACEE297553C91B8DF9F56005204BA380C654AAA29C5FA093004E54CC0254472D9A6861C6B78C7876CCE7820DCD392C4FDFAE48D5B5ADE6C7A51627AD13773DC2A01480E2C429A363755562CF8C10F7F2CF229AA945F2B659AD07A7DD654D4D9151BA7D331ECD1C1D1DFFFE1454FB8F3C33790969002D6BE312C7C5DD3361CCB315C93CFBD2E033466FFA85433D1D0D520E65FF1EDA1D471A2A1DD6702B91751D6EADC4EB57200660408F7746572CDDDDBDBBB7FFFFEA03FCB613141B3D8D1956F94008064609A3E47D9DBB76EA3F075D75C3B3C3EEC46DD15AB1BCA4A4AF50229C2AEDD7BC706078279F98343FD07F6ED2FC82BBCE8D255A1D118EB27903EB015867FA80DE698F21E85E1AA717BBCA0B468FDCA4B459DB1206643B67EBA9A7A708343191391F081AFBE9A883A96E95746CC9B07920467B16BC6941B30DD58C49E0887FC0163F5AA75A5A5A9755F02C905C23DBDD1F2FDAF7FFD6B7373B31705009815AEDA70C525975FE64DCC3A939BEE8970DA79E183E9F29B1FFD86C63127EA9966B4493BD59054E56467B7751E7DFDA3B7BC48302B5C7AE9A5575E79A53701321108F73443D7D093EBEC575E79A5AFAF4F870100B3C6868D975FBE7E8337219C7E79CE02937707E19ED9DCAA6E09A98878901396A362712799D4D2EE31E5049469295F8FEADDA5BEF262C10CF3F0C30F3FF5D4530D0D0D37DE78A31705321108F77425515BBFF0C20B2323233A52918AD8A8D490370500480EAE4823F6DC552A4BA95EA5FEECCD292C2CBCF7DE7BBD895365F48C3279473B76ECF8ECB3CF7458552BF563A5A82DCFAFCEC6930DD20E3AB7B69C447688512A5FA9DFAAD291D23BD7DFE9185133E68F19363BD0680D9F7298AE11CB0BE4B4F574BED7F2AE5AAFD4954AC5AB2990640AB86C24B277DDBA75575F7DB53701321108F7B4E714E1FE80687700C08C32A0D4FFCD0B128F3EFAA869B2C973966DED09DE79E79DD6D6D69C9C9CF1F171759952BFF4E24146F1FF55EA908A57D9E9F1A2675F57FF2BAFBDAC7EA61494E48CF2A452BBBDE0FAF5EB376EDCE84D804C04C23DED8170076036D0864F4DBF52FF7775E73577758C1CFBEAABAFAEBBEE3A9FCF178BC548B59376F796992DF2F3F349B8D37EE966CEAE321729F548DCCA9E9AD6587061A4A570EF7DE5B53F42B8CF3810EEF30908F7B407C21D801947ABF68476178B3B09F7FE58DF491F95B9C613EE6B45B8EB744E6E6C807407C21D9C0D08F7F904847BDA03E10EC06C305904F729F5FF20E1FEDDF6A1A3BB77EFFEEE77BF3BFB16F7C4BE0A0A0A5E7DF5D58989094FB85FA4D44371D776A8F64C02C21D9C0D08F7F9441A5CF9000030F79C26824937EBCFA6565757575454D4D4D42C5CB890C6B343625FF9F9F94545453A494C42D141B5030040C601E10E0000178298BC3DFBBA7E2775D6CCED04EF5B7637D5CCAF6DEDB39710000000B307843B00004C07D2C4F11BA7D6EB7AAC03A768E819E6ACBBA3E4D000ED0E00001907843B00007C132482133A9834313BC848F03443FBAC6977BD6B3D3E33893950F000009029E0E5D4B4E79B5F4EED53EAFF506AD89B0260F63059E316FE505DB352755BCAA75489ADDEFE44A97F7AF3014826D456A156CAD5EAD6EBD5A8A1F25DF5DEDF94FAD22B8719C07AB5BE52950754B0F0B6929CFA2C395E475A6829668373956B38866B8E9D087FFEDA9603EA90170FBE25EB947A38DE149FDC66C7CBA9F30908F7B4E79B857B8752FFAEF2555E4D7E7528125186AD5CCB9D7CCD039024A8AA76A87819A615B4DA87BA42D25EDC788DBAF64AEF9D49AA719E7B4EF5F5AA869AD58EAD626ED8F43E5B446BBAFC1548C3A22DC43FE49E2A50926844A9B20DD3740CDB8C39456A3C3460287F5656D6C90B30D968F5631BD4E6218126D942B9655A9189983B1EF5A9002DE0DA2A2B2FCBE7F38D0F8F070B8279E13C77C4302C4AB36DD102BC7C8436E26D31B330944365868ECEF205462686BAED131459B950DDFF733E6154869E795EF5F7F29295FEAA027F4E84CA5C7A9078C598A0B0CFCC362A07AA9695D4FBAF0B1A39AE95EB377DAEAB4CFE762A2D9A62F773CE7C3901D1AEE891D79A5B556BB828E60BD19CCC2C87330D95F36030B87F48DA3FFF2951C4E43202E13E9F80704F7BBE41B8D3DDB39385FB4D97DFB8724383AECC009859484B1AEE9E5DFBB67CF2314D5DBD495D7995943C297F4D4FAAF1A1C0FFFAD3DF98A3966DB31EF52B33C6F53C57F6D4A8E4D5534C68FA5C4B198E2D4A497F649E9ABE86EBF038EE1B33434E32942724C31D236AD93E9B1A0DAE69FA0CBBC88E15502ED9EC2AE398D4E831BC3A3CD1EA915A9D92C3C29D92999982490A14417ADC1CE91FFAC3CB2FD2C4C26AF5F37BBD1BDD33CF79C2FDFEFBEFCFCFCBF762D300478B722A75FA3C520B2DDA150994F95D9F615013921BC8A9D6BC9D0C27CF881A236D23C37F1B2CBEB9247B454EE26C81E922A541FDE5E9BF1E8D347BC27D4A6642B8CF2720DCD39E6FB6B89370FF7FAA1BAEB871D56509E13EB9A90E40B2619DA1F6EED9B365CB169ABA7693DA7895285D99F9EC4BAAEF98BA41DD6CABA8AB62A60A1A2A4CDA98B4978F9F07912A21359F6A7895A4A96C12D2D2D83062A2E275BC1CF14CA812DA7EC010733BED8BAA6F47C5C6D4C8F29AE5A577979382235DAE0C4BEBA4F8B52D214FF36932FA62A723A5C1540303432FBDC4C2BDBA4ADD7B9F378B857B1F0749B8E7E5E7CFC4199A21E404BAA4D7E9FC6AEB359D7F12C352E4E4847AA7D8F4645D6AC1C5301675EDFE68B03CC8D70A975270A1480178FBE937DB22C758B8EBF2301908F7F904847BDAF3CDC2BD5DA9FFB7BAE98A9B56AC5FA6F855B694BBC7838C812A1419B38ADDB377D7C7FFFC8426AFBA5A6DBA92A34CA96F9EF86F357C54DDA46EAEBBAD36B028CB198FC94AA6E13AAC434DC364737B4274A6042485296D3A2CCF06F870085359092BBBB6B827D7EE6E9B8EE5B2A74CCC702960FB03BEA8DAFFFB83C505F955F755D1DE78AFB49C64EC49FD160FE9F44854A6A21B33741AD4C0605CB857AB7BEFF54AE1F3CFAADEB870CFCFCFE5339626D0C5606A7DCE8F5C78ACB51A8D38C8FFB4085D2C866BC4D8972A9570F563293A355C12633CE2E73E2997CE74419FFC779E79A735DCAAFE4FAF249C0284FB7C02C23DED394F1FF71B2FBBA9E18A9552CD518596CACF58415A73D21CB867AF6771DF24C29DE3A4D43D2716F7ABD4A65255EA0FFA63844D2592062A9AB43AA914162ABC68CA4009A394F9943FC6E9E4B4918ED7866EBD00A599AE2B1D4E22D260E04CA17D59B4931C9F331E69536D750B9654FDB8423BC070A5CEFF3AE71D11EB16BFB248992819EED96B33112D6AE53C988303037F78F9259A48B8CAD0713FFFB4EA1DE025EF7BE0DEFCBCFC74731962933B370515953B4BDA607C96F932F1E2390BF8514FAA9D5F4E198D6CFDB205254F8A62C2A10B4C0FB9A8DD779E7997853B2CEEF31E5C45F305D7123320DBE1A0DAC1CC42254DEA6CA9BD453872B52DDA968668842317AA1A9A7D3C7CACD7EE354897A86854451C653B2A16E54562293544D98BDA0EAB90ADC2944E1AE438A23115D303A539114EE220BB88526652CEF4A9818EF1D6B08AD5A845F9CBF3294F257F490CE97E4538CC5738A976AAE6E5D6AE33DCD10B662262D7A5A3E602E61DB384792C61275EC5990E1B7DBD89F441C43A9D5EBA691BFA410E8D0C3A16BEBA1CC3613DA78F37A5A02B9E52655063539A931296A3001786BE99BAF157ABF519CFD8CB1A7C03B0B8A73DDF6C713FA6D4FF4BDDB8E1E686CB574825CEAFAAA5DEAD1E64042C2BF448EDDEF3F5C75B3EA2387E397593378BD8BC598D8DFA1A1B1FA1D2D8D2D4E25781DAC65A9929953CDBB64961A5B4476CFC506603BD2F5B39FDBFEF1B18EE5DDEB88A2239AFE84296904E095FDA9E2FD27C62D2010FF69FB4B8DF73AF8E9BFC72EA2FF2F273D32D73E452A044F3D31C9B9DDDBD231627190EF2F522E3D4432A1BB919B0B948273B45939A06C45CE57BF7E9778F465A4EF62A331958DCE713B88AE6015E65C57629B1DCC42300483A52B674013327BD624A31866775136FDD44698C93A8D45D659AEC8432B370127432E289A100290D99A291044E2E76322063EDBFCB01EF7706D1FB722CC51DC7F0A30B97F390F38AF273D2B5AC333331395F9874C009C37322C0E8B3C624CE5A1A117F3ACAE79A55BB04095366E8CB27552B71A96C683439D9120417804F3270166E38200DC085040098114ED14F6741EA75C351B6772F12592CEEDA330F278FA4851835BDD4F2FB1F5A69D0BFCB0FA6393194281D45CA5EA78D66C81BA8337EFF743D8F7A6F47AE38BA73320000F30CB9E70000E10E00983B4826DBAC995918B34A667332A9E719BF2F69AF6F1E7117E85E7568708FE83A2526579186659CB4D24ADAD83B8566B133B5A1B857F72492A8952757CFDCE1A39784987E3AA0B3486280C7144193DCF302000029052A0000C09C612A9B14B0A78F491673CF7EB3666EA73D911896773EE4254E799FCE7392F1E4B2BC1A28F0E74B95C1DF8932D87397C29E0F40B2D0AF2192064D04249A9A0AACD85DE5E39E62662773D20ACA269D63094E9D0200808C02C21D003067905226256C9B861D729D88EB8E5B76C831C6DDD8843DA3833D11B5C71D6342C5C28E1D8E39B4F790C3DDB7509278C42D094339D4A45061D39988BAE1A81A77EC908A856C1A27C25336FB2D073A761A74C009732644C38E4B61CA1352A8515B298BDB0FF1C605000080F9067A95497BCEB71FF70D37365CDEE0C50030F3EC897F39F5EA4DEAAA4D3A8EF9AF26353E6636363E4AE1234D2D416555FCAAAAEDB9765B45226C803744CDCFAC3675C42C2B760B4BBEDE6A5ACAE753E6E247978A9B8CEE7AC9A1DFCE273B86ED515AD6513156CF9230E92B83109FF86470B643F6293FED53BE266B8655245B0597362E8BBFB1084E616060E0A597B85799EA85EADE7B749C7AE6D9935F4ECDCFCF973800D292779E7ABB35DA865E65002CEE008039C4614D1C53FDAAA7539DC85179554B2BCBAA1794CFF05059B5A0A2BAACACAAB4BCA6BC767D1509E1A3AA39A2A206776147B74512D2DA73464DD8131DAACD51E3951565E5B5650B16965654959755579457572EA82E9BB2D90B1ECA68530BCB1750A0CA0BD0B8A6BE26A8B2C6D4686979B152F680EA3395C5699BD9464DBA62EA0F8D02004046038B7BDA038B3B484DCED3E21E5056D57D8BF6FD7E578FEA6F286FA8FC51F92CBD7CE9F08BA64ECC98D8176EFFB8F5B8EAA855754BC4E24E829D7DDD4D1E1DDFDCFEB5DA5FAD6A1B1E5B211DF271DAD8142FB27EE6D0CEEE7D1FF4B5EE3FBAF6A18BBB9F3E7E44352F54354B1B97916E87C9FD74C2E1F033CF3C430158DC4146028B3BD0C04401009811C459FC1B900EDB4DDB88D0B84C955BB6AB227C539AB22A4FBAA465E31F0EFCD6D0065D7EB9D40CB78C757C7C8C62CA5599AD6C7915948DEE1CE2F7509DA88A15A9925C9563B7C7BCAEC079650E24B15B46EE10930F923F3E25C7CE69E0AFEC8C1885AAC8E21683E9706E71EF9050ED0974C12028DF023E7FA15140E1E111353CA6BADB793C31C67309F817817467CA4BD860DE02E10E00981144DC7E03A4C40DE5FAA2EC9FB2ECD6C5D111D5FCF411D6A62CD3B522A3B1F4A6C29E2B49FB301325CD70D4C8C7C3CDEFB72EFAB7DA958D2BCBF22AF8235086E113856789D2335CD3564EB5AAAAFEB7F2836F1F1EDB3FCE096283BCDE48D2EE9FA66BD171526E982E6F9BBD754CA3EDF1B6BE8E81FAC6C596454D169A49CD1AEE485EF205307C1EB8677D873FAB6F9ADFA9BC85224687D5134DEAB72FF3782224CB0190FE4CE9F614CC5B20DC0100738688545F8C95B0ED46D4C25F541614E71CDDDC121D8E186CB18F917E659F1112B4A6C11DAA240DC73555C4893A2A660645FFF9FCD438E00603772D1FD3765C9961C65428B0306BF1C6BAAE8FBA07FE3E48CB18B40091BC6A541A03FC795499A2BDDB9D4DC702A67FD1F76A4C6A443896CCA231D5DDE7D3209A3FB0D7123B2E49AE94DC527AF72DDFBDEEBA1BAEBEEAD6ABAFBE79D3A6EFDC78C3AD375E7FC36DB7DC9E951384EA0100640010EE008039C365EF93980C861389299FE12CB28EAA8E58440B54BE41F1036297457BD2ECED0C6F99F6105631FE9E92632B3344FF1443D2DD3224C0F62DC755B64F052890B73EB7471DEFEF1C1489E8E36D244F41B3B19FF6EEB5057C864199D01ECE0B05AB596E52A3C5516197BBABA45978397532A658DCA9187109B272CCDA65356B56AF5A77C9D28BD6AD5877F1D2950D4B1B56AFAC5F566759FEE49D2E0000983320DC01007309DBAF0D3F3B87B83ED25EF945398B5575647F488D1B9E4B3BEB542DDA93E653AE5CC7198839DDB10A55E1F3B15F8AE1042C12F12EDB6FE9C648224F3C4A4D4B5931798F95F65EAB166787B3A327A2E2B84391494B8F6C9FA15DBA1167FCAB912A555E5C5524B9C39EF5A6B20CE5B3754AA0404FC20D1ECA10F634E2B3C88F6958CAF307676306C979CA3E3AA5D428E24E3E650D0000486720DC01007386A14C935FC4246DEAB08B8A52B9AB726BEEAB6DDBD3D6BFA5DF72E5655196AE49F6EE740DF3D84BED03DD838B1A6BAC427EE9D326F52E9D2D9232E67DB12739BF2A2ADA9CEDFD14BFB0B13A120D1D79B599C4BDCDBE2D49BB7F8ADE6441EE1A6AFC48E8C0BF0E2FB87E61E14D85A44159A9BB2AAA7C06CFA4F682AC00049D398C41CD2BFA93C721EC581553CA4799E5289B1A6114C9121E790700487F20DC01007306AB63FAE32E534454B96C3DD57DB7B0F15424172D240E2C5AA02507DA7654C5223422852E5BB6DC18C93E5B5B6CD94A4B2A8F52638BAB8C8F16E39438FC2D24367BBBF20E2BA53A49B0FD5EA78330229430C7A03D922AA5D60C3B0D05B8094149E051122DFD690F659AC8717E5382E43ABFA4C04D2E83543B679B65BA96143236B7B3CA0700803407373200C09C612A4B3CCB1D4B590EE9631161262B2C9F19175B3422094B1A9E96D46B7D7B8EBFD8916B142CB97131FB8F6B73378B3F2B1E64D9CE6F855232F8E55412CF6C86B74DA3EA8EEAB2FCC28EC78FD92394D424DE3FD9CD831A0CE35F8CF5FD6364F9CA1579B5412DD37D74F8AE13E34CA02470C386053D10E8DCE9DCA0368D452316EBBADDC726763E993461C4B8EB4F0A24F9B10D0000CC01A800000033425C049F0B47D996F219EC854C37A3188F48A79B2AAC266CD26022B74481B18435936769EE191E886545721A727807B253DA2B3BEDB07A16E2B746DD6C90B4B02375F6E2A0551BEC717BA2E188B74432D03BA556CAC0E1C121355874438199C70E3C124D629D7DEE092F6DE02427AB30CE1CCE27796F58889F4B9F977BF16900D2111460A081700700CC082476CF03D2E2DAD5415C1B2444F22B5F153B91A86B3B3C836A2CEEEF8FC7BC4032F029CBE25A30BE410A9ED3019A5326ED1012F13EC3478D0DCBF0E998A4C11E41A4314D9FEE3A060000269170A603F31C087700C0DC63B039C9E09AC954FE2CDFF2C7EA478F8FB53FD121BA9DFE6D7E4D33796AD6A2C601FB4F78725CA33F5E7A463861B4B424C0600777935D6766A01ED5A94A6213050000402681EA010030E7B03F3B9BDCB57B8DA1EDF014C9FDBBB362372C43DC2092C9A9E2F8DC1A5C9BC059ABCB24A596557EB2531407B765000000670635040060CED0CA579BD2451CCB1D89D5BABC6BC8AFABF2EBA3BC8CAB8DEFC981F720BD3EF2F6C557C770B9CB768E3933DC574902F1784F627264539209000000C03980700700CC21276F416C7217282CAFA3D23C9E4B6136BDB3353E6952D955B6333D471793D2A77B2CD14F0566C855268EF6FB070000004E01C21D00309768EBB5A31C936DEE8CD6EEA60A384ABE9DCA3D9AFB284CB3783219B0413F8EEE4F90604FFAB393D83BE977DD84486A7A26E3BD269BB4AD030000C81420DC010073892782B54C15871416C42C8E49466BA33377D7C81D98270FDAA9B739BD0782BFB279BE38CAF47A0B4F229212DD68C19D190000C01941F500009811B44BC979408A59ACD804BBB37317900469F5F8ED49FB979FBFAEFE667CDC27BBECC7D3DFB40BFE3CAA9E380BF229555ECBD13E3CDA393E49E8EFA2BAD47E908D27F36001001940121FF181B406C21D003023C46DDAE7427C537CA6EB37F8CBA99EB5D93E754596B4A4EBF5B7EB93444445696C9052F6F43B8BE673BC9CEAB9DCC713E6CAB7A25863270B6A125032F85183C50F1AF87893E8D20F00487BE28FE3C07C07C21D003067582CDD6309E77236D2BB267F6C898DCFB68E25496BB824E60DF79CDF489A26A6E958BA4347BD77839372D67A91FDE059483B5E42797176DF491A940EF10E721CCA133A4C6AAD9C47BB070000C03C03C21D003087907A36BCEEDB09D6AF8E3C1126054B7727CFC4E4924EE69949BB5F39D45AF045588B53638037CE5BE6BE27CF01BF23ABA85141ABC4D854EF7A3E3DC980DA032E354E0CCB30EC28A54DF7689FCC860A0000804C00C21D003067B8DCAD22EB619B3B8D611D4C9AD51EB75B9B3A0A8AB36AEEA9A1189BEDE2EC2F933499ACD4F21F2EB3C7D4F1278ECB974AB53ED69EF4E7C6EC7EB57B74EFF8CADB97FB8A69C5A4A588ADEC72FC0BBE5751B9ACAAF5F196487B24A9267D00000099002A0600C09C61A8984563D711112C3E32245F6376BB6A8B95B8BE2293A62C973435DFA9E20A3B09042A821115EE77FA28CCFAD8E506C3399DCA69D76C6D1FEB9E185443D9F5D9CA6789093E69481342F972ADC0A29C3ED51F190B2773EB0000003202087700C01C42B7207D1732F9EB4686131B77C7BF8E56A99ABC05B9DE2CC3310D764DD19DBA240552E87E9595AD72C41D8507DA395BF5CF8AF7F2A84F59D92A2BC6BE2D8E78C82707DD62A063949DD8D92A6818964C010000002781700700CC19AEF405299D41B2A7BBE1BAA3DB879B771C5AF4A3858597E6F1023C8BC5BD63884F4D922095CE5DD5B0FAA63D9028374DC7E763057F3658D5D30A963228353ED74AB2EF0EA5C190AFC3BACAEFF8E8504DC7A62CF1660200000002843B0060466097ED6F463CC50D9BEE44FC9EA8D8D44D1520C52A7A9D9D51C443C6B1585EEB559282195313136AC2D45D40D2AE8C488CCDEFB20FF69C6173B7A36CDAAFEEB55D70222A3CAEC29426935A19C9D3D592573EC5EFA7AA98B227549412239BE714D14C4A813E7E4E15FF0200E617E8C71D6812151200002493F3D3B524CD2D837B66B40CD727CE27A64FD97169CA3728F141373926A9D556D59DB5E5F90B3A9ADAED91A8C861C3C7D674AFB9611836297792F5B68A46494B4B3FEB1DBFEBCD327296DEB4589699D2DDFCB7821A2DD252B048BB076BB357ACAA1FF86860F88B014A82CE46FE562B6591687713BDCD0030FF80EF1CD040B80300E60C12A5A48CD9F7847470762CD21FE9DDDD175211C77BF1533E90A4974B36C1BAA0AA758FA89668889D600CEED8857766D04E4920BB16EB636A4F28BF5E9E22DB469B27F226B257E6B2599C57485E3DEA1A9404DA9C6118BE1CABE0FAE263AAA3EFD060C23F48243B7F585516C67D1B0000E629A80000007386A97C3E12AC468C6469B4D7197C7B605C0D15AA52D3E72954FAE19056AC49D4C932CECECBAE5095B1CE317ED5D4F0C997515D71CFA11049658A70222AE6573E8A09B74D94ABCAA2A22292CF2EBBB5B0E95B36930C58B78B738CEC94020B55753014B427D84D489245F76A6AC5B0AB7F125D74000000A41710EE008039C3564E4CF94CD30C2863CF577BCD80B5AEF1D2FA47EAFD0541B6304F32389F1C2703F69B7755FEFA82C53FA96FFB577B78DB98AF889DCB95CB7DC5B0270AE976FA15FF194BF9227DD1C36F1FAED85856727709DD343921464CB6941CD8D8EE52EB858DFD8CABEA1A17452276F3F32D940DF2B9565BBE3FE5A305D8DE0F0000605E02E10E0098334840B38B8AE392645E5EB4ACF4AE2212AE861563676FED574E7379D024D1B79B6F7D24CDFDC5FEFA9B970D6C1DE869EEF6A920ED9B7669B172A65683A94CD7528136D57EEC8FC7EAAEA8CB5993474965471AD6FD6C864F26063F65A0C1E52E74D8CBBFF6E70B0B72738F34B559514A0637624CC55F69627B3F0000807909843B00602E213D1AE55E10DDDCD5B92ACFC79A58BA3017C7159365F2C9DB54D2EE57248BD9A0AF4CD2C2D92B022167EC983AA67B98315C16CD6CDE361C4A0A29F42255547A7149FEA50546507A9391B9AC9F9387ADEDF7D254A1C60CB517682F56B159FA9DE2BC92FC9E57BBC655D8CF3B0DC353060000E633466363A31704E9C90B2FBC303232E24D3CA0D4062F78920EA5FE5DDDB8E1C686CB1BBC1800669E3D7BF66CD9B28502576F52576DD271CC7F35A9F131B3B1F1510AB734B5045456F9BDA57B5FDC5B595891B7BA283611E54FA5726F8B621427E56EC85BA274B732F8C551DE4432D0FAD7305C2B60757CDEDDA98E56A9AA158FAEE6FE29E54551D760A3FBA1CD875C155BFCDD6513C7C65CFE4C13897BD3548E63526BC3D6894C02A661D96ECC34293D31D76187180ADA8E55ECB723B17D9FEE8FAA5896324B54F592C63A79F2009B0B00F38B779E7ABB35DAA6FED39B3C852795DAED05D7AF5FBF71E3466F02642210EE690F843B484DF6EEDEF3CF8FBF51B83707544EE52FCBF73F7F58A930C598CAB4D9A43DB3884D9DC624C4D9C6EF57565885F25559DDA30BD9086F38B6322D4A87A5DA9BDABB5577960A7A6BCE2ED44A882ACBCF893526D444812A58FA687D325F8A0500A4097F79FA9DA39156087700E19EF640B883D4E47C2CEEAD4D2D96B26A1A17857A62469464BB239F4F750CFE3EE9CC62B26C774DB6AB73772EB4BF48969D5D98C5DE311EB448CC1932A2E3B6F8EDCC01A67C93C9359D6091BFF3D98E3127B2BCB19E3BDB817407609E018B3BD0E0792B0060CE208D6C2B76170F2E308355BE6085E5AF328255C140957F46077F95DF574D019F55E5F35759FE4ABF55E1CF2E0A3886F4E0C2929D3B99311CCB2AB4B22A0353569FB5C15715F0579A590B82464005F2B2B4833B443B0000CC5B20DC0100738B58BDF5CBA8FAFBA933AF4C757790D2693A7BB35390CDECAEC9EFA7BA263BD6F3075CD9D19CE7CDA952E6245102F8DBB1B6A3C21C8072070080F90A843B0060CE20556C4D12C6DA9EECCE423FE5DE175979D7DC858B36657324EFDAD07677D6CA3A6D49EC86F20270A821611B8E633896DCB113D905000060BE01E10E00983348833A8A54325B915D91C7D255CBCCF7536E18DA6EADFBAB210C4A864902DE27D3F26154F67EE70E21E5ABA57304BB11398661B064E76CE1E7125E8A010000CC3F20DC01007306C965533E2764905E371D8374B26189F17B8607DE35FF2664B0F6D421E52EEF7D3A0E8528CEA13BA47C066A8E06CA0AB6B38BD19D5A37DA019FF24A7E010000CC3B20DC01007386AB2C5BBBAD18E25C2E225914F40C0FB22709525B4192A2F1C226E9787650A1A558CACB327331D0483A7FA47F4AA9EE4BC611F722000000F311087700C08CC036EBF3C0E4CED405D1A95E78A6D17BE29D895B0E07A6DE0CC541451BE053E03EC97D56EA0CD5CF060000F30BFE021D0010EE008019423B91030000F8F6B0BF1C0010EE000000000000A40510EE000000000000A40110EE000000000000A40110EE000000000000A40110EE000000000000A40110EE000000000000A40110EE008019E13CFB71072019C4F843B3AE1B6A9B18DD3DECC5E92FCE52397469363E370B9200DFD5E4CEA66F6F89C959C03520D800837200009811D08F3B987158963B22C97D229F9CC1AF877A760E70500B2AFDCD1A43F994356B020B6430FCAD652A5AEEA4DBDB2CDDE81CAFFCF2FE75E106F314087700000069896B388632A51A730C0A1B96D96D19A36EC4888C8E8E8F8D8E8E0D8F44C251EFCB3568498224209F2EA606A14B0DC698A162B320A16517A665481355AB3614E6790C843B00008034C5745CD130AE49EA666CF768604C7DA2BE7AFABF9EFEDDEF9EFFDDEF7EFFBBDFBFF0CAAB2FC26D0B24072947327294B25CE553AE2F6E039F41A8FCBAD44EE07DC9EEF500E62B466363A31704E9C90B2FBC303232E24D3CA0D4062F78920EA5FE5DDDB8E1C686CB1BBC1800669E3D7BF66CD9B28502576F52576DD271CC7F35A9F131B3B1F1510AB734B5F89459DB58A767817370E2C5130343830D8D2BBD69C038CA3695A91CC3B587EC375E7CB35B75952AB5F21A151A53D979EA132E80AAB8B8F47B3FB83B3B90033B25F876904637C7F7EC1ADCF2C96CBE33412ABD50A977941AA489FFD471A7F2A452BBBDE0FAF5EB376EDCE84D804C04167700000069894B5598A8765319612B44AA9D22B3ABD4A62BD58D37ABAB36AAB2125E6C60A02F1A8D7208806F87E3AAF1E3BD1DE2ABE293C1122135A383DE0B8CEC40038B7BDA038B3B484D60714F2EB0B89F0E4919B1A13B340C0C8DBEF4E21F686261B5FAD9BD9E6DFDB967545F3F07EEBFFFFEFCFC7C8903E05BD1FFC65B031DEDF50FFDCA0C64531174956D18A4AB6710971AA74ABDF3F4DBAD91636C7167BBBF37CB0316F7F9C494930F000000A407D2C58756313E43F72E2318AE32C50F38E11A236F1402F0AD90B79CB9C8F1BFEE9CD15033ADDA0593DFE2D05D24C914030BFC7C05C21D003023E085C0CCC3756DAF87965481AA30AF1673E21DF491A8E23E3F585189A1D28B447104DF1683A53395372E6ADC329C2DB814537926ED3E9978D906F30D087700C08C807EDC330FC3B044BBA41CD49CD0BD421229994000BE2D29D666067306843B00008069908202829A130EFB2F30903700800C06C21D0000C079A125FB24A3BB33E7223EB1FB84AB310CEE00800C06C21D0000C079719A9F8C990A9E33531A0FB0B8030032180877000000E7057FBE915FFE9CAA95E7106A374C693CE8DE3E00002023C11D0E0000C079411239051DDCA7008B3B0020838170070000705EF4F40CB4B4B41F696E6B3DDA79B4A583869623ED475B8EE9F05C0D2D478E751DEF6D6F3BAE13895E650000190CBE9C9AF6E0CBA92035D9BB7BCF3F3FC6975393462A7C39B5A9A9C90BA530D5D5EADE7BBDF033CFAAFE3E0EE0CBA92059F4BFF17A7F47E792871E3403412F6A56F8CBD3EF1C8DB4F297534F075F4E9D4F40B8A73D10EE2035D9B367CF962D10EE49233584FB534A457F76AF1A1F52DE77D86970E7ACCF7EDEB97C1E555BD92D9F8A39AAB258152F90D910EE6006982BE1FECE536FB746DB20DC015C650000009C27665640D554AB95ABD48AD5AA61955AD5C0E3D50D733334AC54AB65EF0D125EBE4C35AC50C5655E5A010020F38070070000709E58BA9B74FDAD233675CFED9BA006F7E3EED0987E0C4E15A747520800001909843B000080F3C461A714D7AB394824CF95938C26D172D0A9E05441B50300321A0877000000E789E3B827BB6DE15FD1CD738921C910A33B3724742400006428783935EDC1CBA92035C1CBA9C925355E4E7D362B2BF4BFFC0F11C8A2980F1D56AFFF59F97DDE025398E99E19F9CD54DA8573D2F01F8DA9CBAF52375CED4DE2E5549074F0722A985B6071070000709E58EC9A2272594BE5817E1E935C3EE31089CEECE0EDC23EB947A2A395C70000909140B803006684397E6D11CC08B64B927D86EDE8DF92144F1E00178681920D04B8CAA43D709501A9095C65924BEAB8CAFC8FFFE14D125F7CA1FEF94F0E5C79E595975E7AA9E338242F5CEEE46556314DB3BFBFFFE5975FA67055B5BA0F1F600233065C65C0DC028B3B000080E440029A843B8D6719DA75C21E09BB240020838170070000903466D9DC0E872C00C0BC02C21D00004012D0927DCE3D7167DD4F070000660F087700000049604EBCDB4F6F25C055060090C140B80300004802A4DAE7DCDC0E0000990D843B00008024A055BB36BACFBEE93DB147B8CA00003218087700C08C807EDCE7275ABECFBEE93DB14718FD414682C7594003E10E009811125FA1070000F02D99FDA75820358170070000000000200D807007000090AEC0080900985740B8030000000000900640B80300004857F0260500605E01E10E0000000000401A00E10E0000000000401A00E10E009811D08F7B26E238DA37854EAE0CA84200981DD08F3BD0E0AE0B009811D08F7B46E293F6189D5C3DC424120030D3A01F77A08170070000709EF81D8B4DEDE387D4E86E153CA6A29DDE0C000000B300843B000080F3C4F6E7A940AFDAF5FAD5BBDFBB79C74B77B61C5EE8CD992360840400CC2B20DC0100009C27A66B283B4A8140C3C6BF97A977C22A4FCF000000300B40B8030000384F6CC3E1D78E4D65E72C57C10A159D6B9337DEA40000CC2B20DC0100009C27DCB385EE2F882A0F12F13E65493C000080D900C21D0000C0F9A2BB837495E31A1C7660F20600805904C21D003023A01FF78CC474B9174853F43A850100B3C399FB713FCB3538B9EF48F423996140B803006604927700000092C299F5F7596EB393553EBEDC946140B80300000000A421E76171071906843B0000807405F204CC6BB4317D92495D4B76C3302800F99E9140B80300000000A42DA7E97392ECA4DDE124939140B80300004857204CC0FCE54CF6742DD613921D46F7CC03C21D0000000020DD2071EE78C104DBB76F7FF2C9279B9A9A5E78E1859E9E1E52F0D0EE1906843B00000000401AA245DCA90F9E62B1188D474646060606280087990C03C21D003023A01FF74CC474E4CBA9DACC271F63628900E69AD3ECAE17045DB272D53AAE6B53C80BF3189C44BE634068F9C4399F3C93B66CCD3B0BFCC301FEE7B370B2835D6F769C5327D7AFBFA4B1B1F1EEBBEFA670341AD591209380700700CC08E8C73D1361994EC2DD927699C5DF4DF5E919604E31954B9A8F07FEA8ED85EA78EF63B8AEA90C8BCE2F5DC1FC8AE3856E2D036189EC18AE413942629D728AF24ABF06AAE77F4B5CF9A6196DCBDB1C359239D2A6780AE8AF9EE9784FACEB314DDA12F0C02599E140B8030000384F2C97B50AE9779BE48263917280AA9B7B483BBA06294883CE8D4183D4EC5AD44D0F3AB5A41D6933D4EE1695C81ED2D00971C41821F94C39E33AACA429985447146E10CB430EB6E24BD8302C79C6E5460A459EFFAF32FC5F278D69D8CB7334D2AC00990C2E48000000E789614AA561299F49528205C61C572217224F330E52D7A4D64464BB2C25A53575217252349F48755E5BF236862C4EA01F3EF0DB9E94C5A625534E1275B29C3FD7A69617ED802668D326FDF33935953BE10CD332AB4BD63694AC5C59B8B2A160150DAB8A565C5A76895EBDB2B2BCB0B0B0BA72A19E04990A843B000080F3C4613B207BC818314B9EEB9FFA901ECC09ACB4A921C5CA52772142328FA62E4051B2E307FDD86CE165F1EE2A5F32EDC9698F49C5DD2F9E31A1C307C65A9AC776EF99D8F7F5E8DEDD348C7DBD67CA30B1777AC3E8BEFD137BBF8EECDD33B66FCFF8DEFDE37BF6D16478EFDE89030778D7369F9A75B7AE5D7BD39A3537AF5A7D53C3DAEFACBEE8D6F5EBBEB75E27EE073FF8D1BDF7FEBCB616C23DC3B1366CD8E005417AB26BD7AE4824E24D5CACD4E9D7EC88529FA8FA85F565D5655E0C00334F7777775B5B1B0516D5AADA5A1DC77CB95545A3C6860D97537870EBA0A98CC20D457A16380763BBC742E150D986B9BC8AB76EDD1AC873AEAA5347F62E5AB8AED539AC768F5744540FCDAAA9A9A9AAAAD28BCD260959190A85F6ECD94381FC0275D1453A4EEDD8A9262638B06EDDBA603028711988E12A9BD5A44323F1DE50AE113394A5E79E3FA4D64594DA26BFC5E0501BCDE03FDA146C7C1A87DA47E1CE13DD7D3DA1D6B6D1E6E6C1B6F6A1D6D6B1D6B6111946653C1C0F53605AC368EBD1A1B6565A7788B7D03AD2D63ADCDADADFDAD6DFD2525B537B283C31D2DB4785FCEBAFF7EDDBB77FDFFEFDF4BB77EF9EAF76ECD0895BBF6183C92E4EFC373C3C7CF0E0C1BABABAF2F2723D17640C10EE690F843B484D20DC934B6A08F73D81BCE8E54B55E7EE9AAA75EDE123EAEBB105732BDC13CC67E14E229B159BC16E15A2DB1CAADCC5AF637AE6725A9357304CDA12BF10C91B732EA00190A9B0C3B969845A0EF6F5F5D364E58AE515D75D5FB07449E9FAF5C56BD72663585374D19AA2B56B16AC5A9D57BBA870C9E2928B5765FB7346BB4F542E5BF6E5B1CED0D0D0F7FEED474B962E5DBE6CF9DA75ABB3B273BA8E77AD5CB9F2A61B6E5CB576757E4E2E9F7A398710EE198CD1D8D8E805417AF2C20B2F8C8C9036171E50EAF486588752FFAE6EDC7063C3E50D5E0C00330FA9A82D5BB650E0EA4DEAAA4D3A8EF9AF26353E6636363E4AE196A6169F326B1BEBF42C700E4EBC78626068B0A171A5373D171CDCDFFEE5D76F854FA82BD5356BEEDB3EF6EED23FF5F58EAAE334EBCA2BAFBCF4D24BF56273C2C0C0C04B2FBD4481EA85EADE7B749C7AE659D5DFC781FBEFBF3F3F3F5FE232105B39164977D3315C12F0B1D0DE48D73F7B7D2CE2A6E72D63B2659D56F129157155C05076C5CF2A03C58169EAFF0C26469933FCC13F0EED3F90A3D4A2EBAFCD5DB536DCD1DEFBE65B9471EC9ACECE2C939866BE19AECF5131DA4EA07C61EEED370672F2697BA1B6D6636FBFB3E6AEBB9EDDB163B8B3B3F1B146DA6C4CD93ED76A6B6F7BFBEDB7AFBFF6BAD56B5749DF36B43F7DC6CDF6F6F6B7DE7AEBBAEBAE5BB3668DC480CC01CFBF0000338231A50E03E9CF8A86DABABAABC32A97F441C4BE3F32B1762185033CABA8088F4DE60C8BF4BA493ADD1ADE313CFC596874FF38696E5745E5FDD2690CF2BEB169ABB0AB2C5B455DE58C6D1D1DFE6268E0A3C1D1AD43EC63CD7FECFE4E783FF30B964CA68A9240A683B7FCB96E7462ECB32F284C8A9EDA3A344E0C94FB948334C4644C039F92730FBCAC1A70D5E1131DEE883C2A124E697E4963C0C7BE4CFC3A3285755F373A2C2984AECB7060714F7B607107A9092CEEC925152CEE24216CA526BE8EB47E7478E98F560EFEA96F428D2C6A5C3ED997827B26F434C4AC329F2DEEA4F6A89D1CEA0A1FFCF3A163AA7D855ABEB471991372F49BA6E70FEB7FC755069D4FC30A98D191E8BE17F70FA8019A2E5485AB7FB9D6CCE60E6C5C33C6CEF0A21DE71B94A143FFF87BCB8183416AC7FEE87BDDFFF87464A07B85E82883FB71E40ED7E932918BE094CCB795EE90E9DC79C65B18DDB9BDFBD3CF2B6FBA3177E50A3A25E1B6D6A3EFBC7351C2E23E49B39DC3AC0E8B7B06839619000080F3C4B4D8B63741A2CD47024EB936ABBC530D82DC5B1EE34D839987B4E0C4C1B1A37F3E1C53E315AA62544DEC6BDA7BE8D97D079EDB3FADE1E03307F63F7770DFB37B0E3DBB7FF7E3BB0EBDB8CF514E9ECAF5AB1C9F0AF239A593CB1FCFF5B123F5FC53ED041DB4E1FAB8A5442ADB513E2392C561875F0A204125A53EEEB52253945FDCAA322DE9A5C7A02BE6D4EBE554F863C4F9975CB2E8C73FE9FAC70783EF7FC87D38F196003809843B000080F386C4881BB095ED1A8EC336C533BFB9386B4677514AF31DCA043BEC8CA9F0A245CB565FD7B064FDE2856BAACAD7D6D44C7FA8BEA8A6E6E29AAAD535B56B17565D54B3647DFDAA9B56D5AA4A57C54C929E7CF6F9CC4AC7E27AE7F308396227263D6F52D8722D6AC1D2042975DBE4F62A654DBC671E823F864551FCAFE7B164A7EBE5ACBACB55DC2420F56EE5E444E89C462728C7692F7A2E001A0877000000E78974E12EBAC431D8AB5A04CCD47A841680C57D363194E3382AA8AC9CD5F9D96BB2F2AFC82BBCAEA0E4DAC2C2E90FC55717165C555A787D6ED1B525455717156CCCCD59991D5894AD949F4DBF6C6727F57E0E9B7126237ADCF4C92F65418CAE0029E754E0455CD3D5C1DDFBB0F15D177F598122E882F0B24CBEADC4FF671A38832960BAA4D6FDF42B4F356C039F4A00A700E10E0000E03CE12AC355B678373B2E0912B625CAFFA9CC9AC57D967693F2586CD1F51B2A26B65D0AFBBC19D3842DC4ECCFC1275A4E22051C937DAFC33E9A47BFDC41A414837999F55CD05DFE12121D3DE5936352FB55F1D765D98DC8A2BC970CA47909A14E4156DEA2EC39DFE86A91D8330CACDE49F6B3019FA669C27295299F3903E024528C00000080F3C4E58A83548B4972911D03F8BB3F30B1CF2124EF585CB309989A547C52942DE78395E034061AB16074692B9EEE9468DA3449781293263B84B041986789B49C67505395FB7267FBBAE481C9EFF2522389DB4A5E8ED10FE97476658FD9324495E9974CF30CE7E768D3B2A4774DD9BC7789E978002603E10E0000E0BC20F1C1A2CD346CB6B792408CF258EBF8497204227E9621356D28C7CF3D3952CE733B8ABFBFC433A637D088705983D239E5FE1E59A7B2C99DE7D0F92641CF2F62D214ED67FE9D64D730495753F1A643E73CE08C38D9C221280B2597A29D4F3C7EF8C9279A9F7CE2C8934F857BBA2947A96DEB2D76F67CE3271B3A6F8D184DBAE280331F5F2600E704C21D003023483D35158A9B7CD3217D4111D2FDF1C92A0DD55402C90A4F197875B90E0B9C6F891C13CBDF4C63189668153630CA9964793749B17B9CC3A608660231F8FA5CEECE51FBB15051D19D91E8CED62775B92E66F573C16F31C8352A3DC3D3B9A420BF964A23F6E2D0E79D2F5D1EE619FAA0F911840E4B2B566678EE4314CB39A4FC25EB2F9F506A50A9E2DABA406E3EC7CB027C319F2BDFF8BD576938795033C9367CB269003C501E0000338254601E54BFD140D5112900D10DDA6F93C4A6ED90C2E0EF0572E78252BDC5CEA8F8E7279485ACA338E72887B8FEA68A5CE68814A37CE3CC6289E6B24DF4647D3FD324E4DF37EA40300B4801884549B05BEC6E210EEE5458E8FA326DC3E12224FED3FA5C9DF3FA72E4F18914242E6A24D4B9B948D0892674930C4F54A6C09E305E50AE48A5B2365E5CBB7CD5A2ECFC82BB6F5539D9D2EA66B71AF6413A47E6198E256D24BAA46929D9A8BC7100C02420DC0100330ED5F65CE173B5A63F43E2F8864D67C288AA684C39B1906D77C5C227229113D158978A7487A35D510C3484BB226E572C72DC75C2863D4ACA2C163B1EB37B63F638D5EBFCFE9AA9F3554B01D15C605EE25071A0D3EFB3D9444EC5212E0E1D8BDF6ED48D628A6455E85D8A67C4D52D6BAF2099AE458D6ABDADB81F0D3803894C65052F21C30D14DE7C7DD9AFEEE5EF1E7006FA686C728BC7BB64CF8C741FC967491EA0B10D43321F80C9E0CBA9690FBE9C0A529353BE9C7A953CCDA76AC850CFFC410D74A8DBD4ADA4DA27F833E14E50F9496198CA17E1CF84F3D7BCCF69959A47885AA2ACA0C64E2CA20C5BD99457611559BCB4A6E4960552C7BBEC3BEB781D467BABCD30635F8F1DF9A8A5E1470DDD7F3A31A12696352EF366CC05943B5A08CDE72FA7DA4A4DEC1C6FF9B479F9ED4BB2EAB3A57D2C571007387B68014B728A94234FEB2C3B0D5E8797D065899F90D195682AB7FFADC1AEF6AE15BF58E9CB95CE51E63783FFF8A0E5C0FEA052CB7EF083E10F3F1A1BE8AF231DC58F37BC6C8D29575E573D593829C411D2DAD627E26CF04C5A7862E2EBE79FABAA5B547CE75DA1B696636FBFBB065F4E057120DCD31E0877909A4C16EE576EE2AA4A8B86A7FE5B0DB6AA1BD4F50B2FA9C95D9C130A45AC90E31ABE98E1585C6D59A6C31D37E88DCC73583A51BE19E2B3E037CD80E98CB9073F3850515455714F05EB327644F6169E2413661608F75483AEACF1AF865B3E3DB6E48EBA9CBA5CD2886CDA957C49B4EAB834B1783CF7A5254D6683DF8774B84B424BFC65CCDE77FABBDBBA56FEB2C1CA99A5C6612A7366E14EB099DCE0BE966482FE699AF29C25BCBC1E40D92F6F9A9E350F1385D9B1A3D1236D7DEFFF3577497DDEE5571C79E5650877900017210060C6A1DA88553BFD88A420822A183B618F1C190DB744421DE150C778B42312699F081F1B9FE8A498090C34C4BC40344479723432D1CC79E5537EBF482BBE818B554FFEE375FE3C635E1EF45428134471BB3E878A043581F53BA54ECF7BFD1D4DC7BA9FEEEA79EAF8F167BA4F3C7DBCEBE9AECE67BA3A9F3A7EC6A1E3C9EECECD5DD18E1809CD7867269EEF874B17AE5CBA70703F33A4CC4D768461833AE7104FB0503748B1AB18850DBA686D5EE2ECF967F02CDA90322C7F7079FD3835128EB4281F9E72805380C53DED81C51DA426532CEE84D6584FBDAC06DBD58DEAFA7E35CC351AFBC9C46CAEE41C798CCC755BFC154C60CAB76F385B4CEEEC8FB2855D1796D4ACA8FC2E5BDC3DE30BD5F7B3286053CAE29E607E5BDCDDD15D6347FFD5BAFCEE7A7F4170AC79CCF45986CFE8FBA87F580D05C404ECC865A54BD1D92C76B6722D6596D52EC859991B1B89B8012377799E19543D6FF574B777AF6457191FED6B9EF71A74568BBB7EBAA12F457D49B28F7A22BBE2CE342EB5ABCE69748FAFD5DED444676EC1BDF71D7DF1F7B0B88304B0B80300661E719211739277D3B19455AC8A0A5471AECA2E52C5252AAF4415E6ABDC32555CA8F28A5411061A8A557EA12AC8573925AAB4401594AA02CAB1A0CA0D18D4DAF1E05CE507F12EC9061D03E61B2C0D63FC1EAA9513E87EB577DFE75F1FF9A4F9E047FB4BAECC5FD3B86A4563030D0D8FAD58D9D8B0BC71D9CAC756EA98D387558FD1C22B87DA87BE7A7FF7E1CF0FB76F391AA1A6193F2B337C6233F6F625C0F43E098764B92D06762F825F2F956E53BD6EDF29BB744FEE24C9CFAEDAD93A4F2BB26AA76575DF40E732D18379092CEE690F2CEE2035D9BB7BCF3F3F8EBF9C2A1677D29554AD3DB1598D8C5A8FFEF811276CDBAED7D91C384F5805B8AE6F81CF9F1DA0DCE49A7E96CD9FAE1ADD3FD6F2A167710FA9D092C67A7997718EC9288BBB7E8B51022201F533282F93394E4A023B4FF3A4DBFF467F6B676B54D959CA57B3A0B6E0C6A2D8709414A0AF26E0F793E4D6EB7D33AC105D15ED0EDBC3512B3B189E889C78BF63480D95ABEADA4717BAFCAA2ABB7128D6A816173F4E89244F6BCBD3929A910CFDFD83E683FBB3E216F79181FEFA64E928B6B5CB4995BC3DD6D41450AAF49E7B5AFEF0873577DDF5DCCE9D431D1DB0B8834CBEBA00007388768D3D0955E9541FE90A9E6E3D0B0C5F8D2F589B85615A43D6A26C1A58B5538ECE81D382E31A8EC1364387F4D984191D5711F60E0049C6E60EBF69E06FF168FF0A6D782565CC5A99C274EAA383D1D6CD6D4737B74E748ED5D5D4ADA85F5659589D77499EBFC497559F9DB5348755FB740A085D9E86E1F8CB7DD9CB7302D5FE82FABCE2FAB2BAC2FA40BE79ECF1B6D6A6F6D0119B9751161502BD3C276CD2C59EF1AA7D66A16CD40367325BDC2983919B600A2812008019472A791953987FF43FCCED17028B75CE4C4FB5937CD7F1B3802BDF79322C37A462BDEFF6E71A599515A5222E4152A12C3EB53944A79BC5B156C9527147BB2343EF0E38CA89A968EED2BCD2BB4BF36F2FAAB8774170993C8AD15A9A16A4F0344B886158FCCD2653C57CD192DB0ACBEE2DCB5A933BA1C2AE72063FEA9B381A666DAEA53A970619D11ED8DC4E64BEA898629248E601935EE733AD77416796B14FDD1D00997F8D0100E61CAE7AB478108B1CDF79B88AF24914B81012AA7D56E53BEFCA34B2AC5C95D739D619A8F197FDB0DC3DEBBB8EB3C1AC1CF66CC332987FE932F13C9CE92C538CD67306BFDDA8BA3F18E81DECABFD65F5B2C6A545B796906EF6C9879668157658A1D51CEF8B9BBA849C0FAE27BE69F3E2F5E25A7ADDC24B0B96342EAF6FAC9F084FF4BEDB65D1862525FC5A057782C293EC2443A5902E6FBD898C66E60E911F65C8E9A25DD07545DA7D7A0F4DC03C00C21D0030E3E8AADD4357F954D3A33EFA7624543B71FEE2ECDBC0FB7055A0366BD92F965E72DFBAD25B8B28118E098B7B92A1E6AD185E39E42A5BA668C2A1FCD7DD96B437B547FAC71B1A57F972021423E785E359F4C90B076CB097E86942EBE846359528D996D8F82946774CB3A47109CD39F064B356AEDC43BCFED62A87E95FF63D0F9EC04CCED9641E2DE71F5FC99CFFE28D849E20C1E940B80300661C5DCF513D446244AA75A99C925AE5CD4312627D96CCED02C934D3747CB99659E053966518EEDC6A8BC9122A73202DCE9FEBA12B84DA66F2D57C71760F1F0E1DDC7C687FD3015FC05FF56F55DE79A7E558345331B04DD788D104893E8E9C76FDCE99C99BE27635EFDC33C07B0136E5BB6EC54FAB0A4AF30E3535EF6F3A34F4F9A8E15052E542E655B878684F1E7001706EBBFC9C841F7DC8B76F296771970453C00506009879B8DA17639C219ADD935BB8FF7C5BB4749B1D73BB408AD060DF18794181E41DE90B9CC7E4C3E7532CDFCAEB4DD050E6E8AE899EBFF5E5AA60AECA2BB8222FB8308B4F050BE518CDA6A5A418D8EC59A125FB85170AB6A2B3359D4E305BF1F9DA35548C4DFB86112CF6155F5394C3C9C81DDAD137F4C9102D2F8F04282D26B51C66AD2CCE156C098F43873B69EADBC259A7EF8F317BE4C32D21A57C3E1F9E4C8229E0860B009871B8EED10AC0ABE5D88AC4353DF876CCA264D770956192A434A23E3EA5E2DE9E44E5024EA2AF0E473775A327C2039F74C7546C61E3A2DAC76A73D7E691607494214299DF15919260725F8D7C426C392717707D89AD9D5DE84F9E5471C5A14D79BED63415AC0AD634D6D434569BCA3FB0A76FFCE884C5267E290BBC0CAEEB0B861B3FF4138D46DBF77DDDA1943F3B871DA5009804843B006046986C89921A9FC754BD276A755DD38374C4E0EFDD12F2C1197D7641F2E04B87153B8B66ED5BD6F2EA319A5ADC58C7B3759E8B3F34074EC3302C89BE80EB4BFCE279BBB26DDD2ED45171E231BC0C69F7A29A82D6BF34C74663A42F59B5B3A747E65ED7E2CFCF9F9F9529CE1EBFE5B98AC5A2762CA2221127428110296F23147522E1330E6E38EA4623763446615A8502B22285634E34CC673F12C9562A8B76C89E4792DD3A0C8052D6860DA77FB007A413BB76ED8A4422DEC4C54A2DF482271951EA1355BFB0BEACBACC8B0160E6E9EEE96E6B6BA3C0A25A555BABE3982FB72AAAD4366CB8DC9B0620198442A13D7BF65020BF405D74918E533B76AA89090EAC5BB72E180C4A5C7AA0353349E1686FE4E8F36DA58B0ACB7E52616A419E3290900C2ECD0E58C163EF7566E50402654183DF543E29DCE30E3C2703E90D37A5ECC8D1B6FEDE5ED2EB85550B0BAFDD640D8FB4BFF7DEE0F6ED03DB770CECD831B863FBE0F69D43DBB7F7EFDC3EC4936718FA766C1FA285B7F302FD3B76F0C23B760C7BB3768C6CDD3ABC7B4F4CA9B052C5C1AC9CE5CB06F6EC295FB162675757786464B2661B1E1E3E78F0605D5D5D7979B91715E71CB340BA93B92D63000000200D11852BEF83AA68CC713BD431A7D8B57C29575F533A4D9FE15F98D5AD3AC72642342D56777E87D55B202ED63341B51374587420E2BB42C763D8115B99596BD716979717373454ADBDA874CD9AB2D5ABCB575F54BE76ED8235ABCA68F24C43E56A1E57AC5DBD60F59AF235EB16D01656AD2AA8AECEAFA8ACBEE8125F5161A77404C91677653B26741A38051408000000E98AA710331152884EA71AFDFBE862B534B72A4FDC34520DFE866B20CBAA57F5EE57EEC49171833B30E47758695EE6B9768807BFA79A284807682AC757B1A0E407DF0B94163B3E9F95E533B2B3555640052D2B986304B3CE3204696C06B2DDEC2C272B406DB29C92E292EFDE55F6C31F04D6AE2CA85A58AF5450A910EFC66F3A7867009C02843B0000009052688713B3E7E39EFEC1EE9A072AB2EBB3D9D69B7A3846D457EC5FD8583B1A1DE9F96B0F7BE4C775E6642B7B668878FDDE0EC977FAA563734C8BDFDC75CDD8D1637D9F7CDAB173C7896D5F756FDB7662FB360A1CA7F15986CE1DDB69DCB17D5BEFB66DDDDBB61EFF6A67F727FF8A741EA78D77FEF18F27F67D5DFDE86374FAA3BC47877BEF07601210EE000000D2950C5535DC17647BD3B1485FB8E1B1D5669E8FA5622A6A5FD3547E797BD659DA58EF3702879A5AB85B14EECEDD4BAD0E6488AB0C1F84F76DD3181DBC63BB8633BA6D6BF37B7FA9B8F59686C71A97FCE297CB7EF90B1A2FFDE5AF78A0C09986E5BFFCD5925F3EB0E2970FD4FFE281A51478ACB1FCE61B8FBEF956B4BD35985BC0F9C7B67C65D3C019079D064E0105020000004825B81FF5E8B01A1E54A33A42D462CA695FD1E60EB732A413C371737C480DEB54F26B9C9924D927414744074607EC2F2C18FF6CDBD0975FD66DBC3ABB66111DB3999BA37282343672B28D9CA04181330E3941959DA772F2286C66E5520605162DAEB96263CF5BEFF8B3B3CB5637B43CF1F809A596646795DF7B4F74945D66004800E10E000000A410AEE3F4FD75B458152C59B3C86683B663A6A06C6775CE2E3D22CF29D166F9A68A2A55D6F96C4768342C715E9233C34F86E0C3704D47D9746016057D59036D474694CA5EBFCE09066C71128A0D8DF4BFFA5AA4ABF35CFA8A3F65C50BF357E928FF48F16765E55EB67E50A9C8C468D1C64DF979F94BCB2BCA6FBD954FBD2DFD22011007C21D0030234CEEC71D640667546093233346A2251D7BC81EDD3536BE7774F4ABE1A16D23C3DB876918D9363ABC9D87D1ED63A33B862830B463706CD7C8D0B6B1E6E6C3AAD2977F5DBEA59531C3522F0521F9AE157ADEDA9CA2EB178C906CDF1E1A6B098DEFA2631CE563DC39C607BB6384C2635BC787770C0EED1896E31D1EDB353CB47D7CFC20A95F96B1F2B97F6DC84F45F830F99F7B56A7A4BACAC97254B6CCA20939534E6C64FCE889AEFE7F7CE48E8C5134BFBD2B278EE43E8D62CA0D1D3E34FEC5D6B19DBB8677EE98D8B1737CC7AED19D3BC777ED9AD8BDDB4FC27D682472E244D10DD7E76CB8CC57506018568CDB08009C04C21D003023480F0C20C32009E2C947ADD169AC3FFAAE27D98B00DAFD4C8C7E35B4FB939D07FE79B8ED5FADED5FB4757E7EECD8E7EDAD5FB4B67EDED2F179EBD1CF5B5B3E6B6BFBBCB5EDB3E32D9FB4756E3BE25356568E9FBF61CA9A508F531116A62E0B56BADE0D65E6AC0E2C7F7059E7D7C777BFF755F3BF8EB67DDE42C7D8F1596BFBE76DED9F1D6DFDA2A5E5CB96B6CF3A8E7DD6D6FA5917E5C0E14F9AF77FBEEBC4DF7BA8D8D0162C5DBA58BBA7682BE56C3806EB723A53A632F2946A1F1AECFDDBBBCAB569D22541EF521E5934E97354CFDFDE3FBCFDCB639F7E7AFCD3CFDA3FFB94868E4F3F6DFBE493B68F3FA6D31C52EAE83B6F1F7EEBADE6B7DF19F9E233DAB24F9A3300248070070000705E18FC457DAFD6D0BECBA2D4392621D9753C01059F8032226AD81115A240F9E2EA953F6FA8BB73F192BB962CBF9387C5772E5D7AE79255B7372CBB63E9D23B16ADFCD1F2653F5B43193D36366172F3D7E1314942C9E7D48225BBEEBC3D5E2AA42F484A7C444DE4AB82869FACA003ACBF73E9923B972DBB830F902657DCB16CE91D2B96DF55B7E4FB4B022AAB46D5965CB1A079734BE844889B01273D6CF44F7A408D0ECA0696E976745CA9FABADA05DFFB813228924F21376E0D15E9E839FCF8E6A24B2E5DF7D8C32BEEBB6FC5BDF72EBDE71E1A96DD7BEF3209D43FF08BA50FDCB7ECE73F5F7EDFFD6B1E7D64E4C0E1EEA79E320B0BBD7D002040B80300009836095D4E427D8A64D74C999C21D242DD0DFEAD377220B6BAFA5253F98225A6AF4865D5E5642DCA0ED6F190BD282BBBCEE75BECCFAACBCA599CEB3A4EEFEB276A2AEACAAFE46F5EB2F0339C18AB3FBDB15442BA6D9766854E1D7F9AC856C6840A4794E3577EAB2C18ACF3C90166F916FBB216E7F8EAB2038B8381BA80BF362BAB2A68AA60B0C0E72F3507D4A01932C42CCDDBE2568A7881A7072E1BC5A9C0D385E02B2D5FBC6279D1951BB5870B374364AEAB1C5F517ED9A2C5633B77746D7EAAFBF7BF3FF1E28BFD7FF843EF1FFED0FDE28B7D34A6F09FFED4F7FB974EBCF452FF1B6F763DFEA41508167DE79618E52500938070070000709E24B494232A8591F029A25222C1497A0EF70F46874ABE531254668C7BF82379EA6551DC56EDD3CA97E4DDF889505BA823F7D2AC60A59F759F61D8CAF4E9CF75A61A2EF7DACE69A4E4D111B9ECFC6D9A6655C182725541F27D6CD788E3C8C1F26B987C84961C87695099E182C35F0625E5EF9A392A609BB658A759BA2B83BB5BE45DA40796F6CCA77365E56617DDF41DAB6881977E6A7EF01308C234F2F28BEEBCCD0C0643D29B64989F4B7067ED14A681D66F1F1F1B70F85DD49691215AA6F8DA4D81C575C6047A9501A700E10E0000E0FC704D6D3E94BA839D22C4A038B51E99A2E36794D9DBD3B720A002B92A481A8EE49DD6B8064B549A947717B9731616F212543ED27E2A400196C1E2ED2DBACF4CC1B7BD5D65D8A6F4B1C269A53F39229FB3E0DEAA05AAB455B51CFDE4A84BB2538A89871C94A73D4CC354267B02B1CB8DE9A3B1E4022D6B534B268D30B8DB1F4A3C8974697B387460726AE99FBBE4D74D1A3E83865AF0E0838B1B1B6B1E7DACAEB171616363ED638DB58D8DD58F3DB6F0B1C672A5CA6BAB2ABF7373A552B5DFBD2BB8A281B7C2AB0270120877000000E705CB48361FB2D5945509C17A44873C606E3F1D92A7DAA9846598E367216B521E72E387759E58A339178D18CD8F598ED85FA576366262CA95071D29E83A621896232E216C57A61FFE8C281D121153619F0AF8A905C29DCEE84370748789930A0C4D464D8764AFE3281A28336CDE14F7D0E21D747AC07D44D261CB79E48B830F811A2BA75C23AE8AB1B6E7069876ACA1CB484EBA2CC25EF0EEA2C6C6C8B1E3FDEFFFBDEE91DF1C7DF3EDC1BFFD5D3C86704181538070070000705EBCF9F69F9B982737CBD0D4F4B84C32870F1FA605D8962C12149C8A49B24C5EE43413CF2748A9F140224F6718877D7DEFF70FFF6B78C5352B02354131C4FB4C16B1B46A2AC229F40E877F38912433A58161297F5445C7D5381509FE4093430B98D69487333CC94D1AD2B42135267EEDFC8A276F84574B1FE56E38FC4C818F5FDA68921BD260D11D65B28EA76CF171CB8417E6361C9F6FBD4ABCAD2B178E915FD8A7D4D1279F08B387D5C1D14F3EB57273792E00714EBD8A00002049C4CD4DCCA4A0AE9E44884C8E05A984A7244EA3A3BD8BC60B2A9C92A2587161ACA2FCA4B41A1991AEB8671B4E80AB5FFE4B014BBF2ED5344E24C5611F6649A58CD8AF88053AA596442A4F732DAC976615E78CB78D0DA9A1DC8B722C3F8B5A5940CF2452B0BE3E25493A91DC3851CE840A2D5045F5CB96F5FEB93FD41DA559DC91B93E9C78EE5056D1608FAB9CDADCBA9CFAFE7F0C865AC6E411045BAEBD85520B6D4AF76CE0AEC14F0BE247ED1DBFA04F1C8D75FEF0A4BCCC109FA465E90069195945B775E99FCAB119090795CA310D1A772B35D876D4F2FB681665CBC9D2407031932719BC396AED88031B4FA64F6B075C28A75C750000902CC4B8E4C141113484292A8B9F874F5A00A414FCE0FE341D2C3159D941F58B07D4830FAB871E510FFC425D73BD375733D9DC3E6B4A5AA7F6E4AEE756BAB01E8B2915255D451960BA7E29F7314B59B64933DC982B0A9E131B4FE8C93C332DC3E7537E6F2A6DE1974DF3F3AB8295BE3A7574E8E878DBB8B8B2B3FF8BCC8EFF8A7F881D8B189659F2F3B2CED0B1C15DFDDA744D8BB0013EE510D7A6B872D24D2E91CCC98132C80E87B2952A7BE43714A621C05B177DEE18869FFDFEF7EDDBB787D8BDFBE0A183070E37534C73CB91E6E6D6B663ED92B11075990FCE31006056D1AFDAD1BDE7A45C0129C6293A388EC4F0EB83F493D0E473EDC3C1559824F6646A4F4BF8ECC266642AE27E43999412D27922F57C3A9F28B906B55C49B1D3340BD3531A199CBF09DF93B4C65055F72DF0D7677FF5F75D7E650582EC027FC6863A1D2C67180762B92AC76F6473CEF00B00A9099F2FD7B4E8878FC6717DDC77FB542EB8C9CA87AD1B2DF182611AFCB155093AD9D9FC91D60F3FFC700BF1C9C7FF78FF838307F6514C7B7BEBDFFEF6DEDB6FBE35A538814C25136E11008014876B1EA9B6E3159A7EC68B6A2645395DB5C7E15EFC126793CE6262B9B308FDD98076346BD6FD73C3074CFF9C47FC80897B19D111D2E9A12C2298FCC8497CC1CF58056740BDCCEF5DB2873E1FBD69B816B5D3CF581A0CF126E12CB283E2844221C916D7966F91A616D4FCE03B173F4AE1D3E998468C7B8F998A2EF91750264D43454D639C33C4D2D9653AB67CCF4A59CA181B626FB45F3DF2EB5FFEF2D7F7DFFBCB071EF8E52FEEFF25851B1B1BFD56402FCE693C438A404601E10E009871A812D2B5184B3D1DE011EE3F69C014FDC12664FEF1C2DAC397C3D39729C940BA0FE7E6443C1D738C672A667DC76F589212632115521361EEB39B5269FA62E209A3FB2039037C385E309DA1F361B18DDD67B20AD782D24317159D51DCBA116C2BCACD1D12F2E221436A35059FC97192388DFCA62DA5DBB25D6A939C2D95D32D93942D3D7FFC63C0B4EA6FB9CD8E46A9D5C27B31FCCA8D5198761CC86575FE8F27FFF6B7E7FFFAC18B7F7BFF77EFFDF585F7FFFAFC5FFEF2C4BB519B3FD2F4DBDFFEF689CD4F3637B7CAF640C672C61B0700002413AAE4A816D3F518D7E6543BC7ED9120C539557F70677622ACF4A4D70C9B0249102DCE661E6DC9E514CDD61ECF0D89542EE034D2659E34DEC8B6F13C9557B6BCC40C53A413F545F9FD8EB3A83A6A08A5C2617C6BD8FDDBF5BE836ABACAE652133F307DEC538EDF62314CF9C67DECF09D415E10D0B352073A0227DED70D1F8D8F4B1D49660AC70FEE02E1D50DE3586FEFA8DF1758B658393689F571A522EC8BC6AD412A3341378B966A338E751A1D1D66E771D5D565741C37BA8E5A2DB20D35363666BB4E4F6FAF9E04998AD1D8D8E805417AF2C20B2F9CECCFE101A53678C1937428F5EFEAC60D37365CDEE0C50030F3ECD9B367CB962D14B87A93BA72132B3C36A729F5F24BAAFD98FAD94FEF8B46A3A921B6C03743622B180CFEE1A51772B2DDFFCBFF22424394D7E75FAA2D1F71A0A161F5C62BAE1A1EA67BD1AC9E5396C0AE9B9595D5D57DE2837FFC95621656AB7BEED533D533CFAAFE3E0EDC7FFFFDF9F9F91237C370CE78DA2EA60C7FD8DAFECC9715E515D53FAA0E75479BFF7460E1C53545579550EE91143BDDA8DCF974E748647465E30A6F3A3D21E56D38C6F007C3FB0E1EF42BDFCA6B96E75E949728339339D8B43F47E52D6CAC7142B1FDCF36575695967EBF443286F230F584BBBC4E31F4F70F9A0FEE0F2AB5E27B778FEDDA3FD472B8E6873F701CC7A54116F0969E0EA6C907DBF5E7D7B32A2B4ABFFF032714E97CEE9909A5CA6A6B0A375D77E4E517D7DDF1DDD7B61E6DE9D9ADFE535698923D4F29B5CB0BAE5FBF7EE3C68DEDEDED6FBDF5D675D75DB766CD1A6F06C81420DCD31E0877909A4C16EE576DD2711E09B507D28EAA2A75DF7D5E98F8E20BF5CF7F7AE114614E85BBA7A758A38A82B76DF7C013074BCB4A2B7E5C1E3E1139FCDAC1DA8BEA72AFC93559E1E9D7B44FA1EBA9E343D1E1958D2BBDE9F4444BF4C17F0CEC3F702047652DBD7A69EEBAFC33E9764FB8D73C5663879D43CF1E58505556F4FD059483E2629472C25D8CDFE6D0077F3BBAFF3009F74537DC94DBB0726CEBB6D6AD5FC45FB93F05AF01771A89C5262F40F953B3E9CADC8B2FE563774C2712325CDBC8CE8EB41D6B7DFBED3577DDF5FA97ED47BB7779C23D919B142048B8EF968052975E76C995575C05E19EC140B8A73D10EE2035D9BB7BCF3F3F3E29DCB9A291CA8646A343EAC001659E41B7CC12F281722FAC1F0550B22839FCF1F6B39358851F1DC4C349475C89BCEDEBF0F9436BD11AEC774EAB4B22CF9DD429DB3FE34E13AB53E658AEAAAE5515E5B27DC25083C3AAAD45593E65479576C5A5BDEB4E3778EFF1AD4D49C3748FEB6C9C4CB0C36FF3594165C7546595AAAC90D9B32FDC293D7C685A74CA38EAEE7F6A7F6969E9829F9645BB9C03AF1DACBBA826EF9A3CC980330853B1B88FA4BB70D70C7C30D0BCFFB0A57C2BAF5A917D492E7F0154BA3B9FCCC1A6832CDC1BAB9D0967FF73FBCBABCACBBE5FE6CD4B3DF4E91DFAC7DF9B0F1CCC26E17EFDB539ABD71AE363A1AE6E6F89499CC3F47EB6278D81EA2A332B40058377C4D945BF4EA4F544CB3B6F5C7CD79DAF7FDE7EA4376E719FC2248BFB646EBEF9E6152BD2FBE90D381D08F7B407C21DA426535C651252926A33FD4623ABAEB3566D330BD59BBC67FA8F57A024FEA69516DE42F212AF53C1C9896F3611332DBCA399BC916F4AA7DEE3E404D04F620D6F6E7C19BD406283A7CC8DC79F71F294C0F48FEB6C784992B00EF09627ED6096853BA7844DE974AC2CCAB984C7DCBD4FED658BFB4F2A625DCEC1D7F6D7AEAB29B8BA8816E55695CE9149648A70E746CBE03F860E1E38E853BE15572F635719EE10726A5B25BD843B1F976B0E7DF88F23FB0FC494AA542A3B188C9A2A905BE0CDFF76D823C3B66D531E71C9A0424445C86FC6262227A862FFEEBFBDF5590BBBCAFC7FA478EBE2CE4BC8F8C9931677821A8AD168342F2FEFBAEBAE2B2A2A928D5DA00F0F4841CED0E2070080E4C2158DC841AABA49B5F3A484E70A4A8CAEF5A46EE431A747479D099AE30DF165925B09EA8DF1B6E39BA5DF0BD8036D8157943559284B389EE43330798F04AF1E57EDB43A6F41AFAEC7B2218A215146412F5EF6A267F1AF9E24E2FBE5C4C43745502089F0B6F561C6271329991324259C0A69A0523ED98E21D6537E9C43059E7B0A2101E5F53DA3732413D1B66292B9A230F838395BA4BB9834870FC18EC57C4A55C9F458381C9E088FF4F62465E0ADC562E3B158281A9D88C568189D8844952AA10C2CCAF7ECF49404CA5029553C9CC6FAF5EB7FFAD39FDE77DF7DDFFBDEF78A8B8B7524547B26018B7BDA038B3B484DCEE8E3AE7D0874A523B5D0DCA02B315257D478D0C920A191089F0EA756A4E7940566A832F464AE24695AF0E27AAD8450D689E7E09999BC07BD70FCE7247A759D5DA7CE11217E6A2445E8CCD461FA4DE433CF8A2F9314F47E13A7C69B8C0788397839558CB2FADD4A4A8913750E3F75B0A4A478C1CF2A2227A2875E3D507BD1A2826BD840CB6F709E663BCB205719276E71CF5A71F592BC75F9A79C9838E9E82A33F0C6EBFD1D9D8B1FFCB519CCA2C2C75D7F9E761E2F9C29B9A4FB1495B8779E7EB33572ECCCAE32932CEEFAE5546F026422C92B6D00007016A832A2617288748D5825E766D036E384473BFD6AA1A9E79E3EF02CFD23E9D729D7079214BC2DC90F8DB40CD52A795A03AFAE252CADCB9BF022272F3365D090F665092E7BA7589D3F12118726243D7A319AD401DE51225243B324261197C8DB447D43E1A40C1A9D5D1A0A24E2E702D1EBA4DA7567E4AEB27CE6D25F2D1FEB9F38F6C4095F3625CD8A5A3497137A967EDC33045799B6B2436A82AE33298BCEA41291AED07148C1E622C65749D2553B61507EC9A625CBF47D87FF79BFB2A329B9A827E7B4D083590616F7B4071677909A4CF57197CA856A998F3F579F7FA654CE5CD635A6CBB523D58905B6671E8D9100D562F74C68E9A9492CC3C2FA2CCB7F1BF4662949544BB3BBEBB420A568A8900C9CDBB49D49A6E833A28F402F9008EB55BCD5E5B55DE7D4EDE8599349ACCB796BA82C57051D594B163B9969F1AD250D9133BC4149D2D888BAF25A7555DCE038FBBDCA507212072739C622BE65737354B9CB7FB164EF6FBFAE5957537875912CC32A5F163C498658DC5DD5FB976E7BD00D2C0C1EFCFA50C3E5CB0B2E2F3CA3B746FA59DC5DD5FF265BDC973CF4B019E06F694DBE2E92029718BA6C5CB6B5D3EEA4A5C081779E7EBB35DAC61677BDCBC9C50716F7F9C474AB0500009836AC44B9FEE1EA66F0885251A586943B77833DACD430078646D5F0308F49F08D0E2B6A029F71A05989617264229CC4416F76784C0D8E9D127F5EC3A81A1C51213A3A394C7D8CE7CE6A9A9B582011F6C67AF561CEAE44A41EF4ACC943625DCEDB214E03E52A25696AA68D7AF1491B121BA45339C2EDB123CD5EA99B53C4AC4E70BB90DA2AFE2CE573F8ED4C6B52A59BC9F56F7FEBC089A1EE6061B0425505CBFD7CE8E90FDDC4889852213AB1AE2D537456F92729E842C3258690860EDD35E9D7E0AF3C51285E60745EEA2992EF6741FBC44F1E83CC0016F7B4071677909A4CF171A77A836A38C750FD4FA9E3832A9FEBA4B92457A99D67EE422DEDB194FAB95261097F6326D302B43CE9026A4C9112E0D314B7E551980234490B50983FDB18DFA01E6BFD40E8808E240A957A45A9516F6AB6A9A9523F8FF7343FFBBDCA24F244C26C3C2565D7D6D46E287BE1AFEA0E3EB7AF665D55C1D525B248C65ADC4926766E3EB65F1D2C55A5EB7EB5CECCA6DC30C5036BEAF1A69FC5DD51431F7D38BA7F5FD5430F9A81200BE2333E4AB860A65C5A822E576F3FFB4E5BA8756A3FEE9AD33EC0E44D804C04C23DED817007A9C9E47EDCB5AB0C41754DEBFFAE461C55DFD868476386ED98AEE398E260A06BAC04067F86D0129F11933D861DDBE09EC4BDB9E787294D8544405BFD2CDA9FE1985981817F6C693BF0F5B29FFE28B7B43C140A19A68FF648E9E1C7D486322969866348476ABC9A528160F0F833CF1F0D8F93E8A7C46A699B14F40EB44AA6B15FA9DA9B6E2A5CB9321CD6F2FB7C0906822D9B9B28618BE8C61E0DB936C571D629CE3A1FE568CC304C121A6EC43502B22B3E522B1838F1E4D3EDB1181D974E0341635A9BDD5A2450A054F54F7E12282B8985A2A6722DD78999268D6DD7B2944B79CB12D5A0DCA37D06AD2CB3E3E9E70E46C20B945A7EEFCFCDC26237142679631BAE9C654952520906835D5D5DAFBDF61A85AB17AA7BEFD1D173F272EA1938DA7494CA54CD2FEBF63FFF75CDC5B5855716B0178476ABE18FFCF36BAC9CEDAE6A7FA67D2432BE9A843B47C51D6FBCD929075F55F248C10B708CAD0C8BCE72C7E6F683EAC84255B9EC57CBCD6C9EC547412371E28A879DC34DCDB92ABBBAB1C60EC5F63DBBAFACBAB2F2DF4A5DE3A4BB51EAC1AF8B2ABA924C9F17A1917324232ADB34D0A540E7D89BA9674C81EF2CF125BC302F36F9D29082115FF79DA7E2AE32A7035799F904847BDA03E10E5293D32DEE04554047FF438DBA2CDC2542D7582C98E35598AEB7A8DA93A5E3F5996D38962883E92155A1462AD44465C99BB227C2F6EEAF7AB66D2BBEECF29C0D1BA4A9C07A8A66B19892DA9297A7AA93D5064BD3639B9BC6955A72EB77CC8908C9505A2039C84E48F4FA72F346F71F3E7AF4C8F29B6EC85AB9CACB92F38632AD6373D304C9E5C71A39E99CA9AC21645E5C3311893897DB423143B5353545955A7ADFBDEEC4382B137695A73CE71CF71714F6FFF5DDA1E35DB5F7FCCC5758C8B28C37E66D2A1160781722E00D153BDEA9FA06D444B87FEBE785375C1F6C584DCBC5178EEF3EA9F4F7F7BFFCF2CB144841E1DECA167767E12F6BF73DFFF5A275D5DAE24E6257CFA556A4D8E5B9C4D923D1E18F4606DA87EABE5F6B55712EC594E1E316A42D0532F9F9F66D91934A2D0C3AE95E6193CBE848D3E1667564895AB2EC974B54B6BCF86050138F8F9696917634ABD2434DFBF3738A2BEE2E3BF2CAD1B25515F95764AB1C6A0A7249368C9852A78AE394C019FAECB3911D5F2D7CF821C31790E3E2F342571B67011D9F6448BCA80B923B89168B5CEB9366539888DFA968165D93749AF532D278F6F2C113EEFFA75E4EC60920DCE713A9771700006428BAAE910A7E1254413909C313413725932A315E4C57785C2352ED45BA85E4B46CE5FC075E5506DA94DCEE684FBC1571B8B7B22DEBE27559454583DBB69FD8DCD4FBDFAF92587579EF268B245A85EB4E4E875E7DE0EF7F33FDBEAA75EB7C4B979B6BD7F8D6ACF5276B58BD86C681B517998B97665F79455565C5D027FF0A1F3CA873E4FC19F8DBFB94C28AF5EB78C26B0AB13CE2E3950981DF7BE548164F16CDF1B94EE5CD3795D7D48C7DFAF9F8CEDDA3BB7687BEDA3EBC7B67F8AB9DA33BB70F6DDF1A3EDE45B2DE30FDB4BC6C86B346B6C0611EB108752944EA93F64A61ABAADA58BB462D597242A9B1FE3EBD30AF1B3F11C985DB5793CA50AA21F94DA58E0E9C14181FBE36519342639146AA9D4E13CD775D2BDF6FD558BDAA37140AD1F214E3E36CA35CB3BC5ED153093EAD92EB741889DC775CB3FFCDBE82D282656AB9ABA2361D16CD23D54ECBF331F2E1F072746D5241513ED36F8E8F8686D440D612CBCA32A944D2D1D242F60C94936460C64EF4F5D301C5F84642A58EEF183449C58F9BC1727791F413943F7488749EF5F5A7B34B2F93987B72519D97D48CE30BD3BB6053330BC0DC825201009815A466D24C0AB2D4D3559454E75E8D4E7F63479B473EFF7C64EB97E35BB78D6CFB6264DBD6D1AD3B46B7D2781AC3C8B66DC35BBFA4D5C7B66D1BFB9286EDA35BB78FEFD91F0B8DB2795DF9AC6056C93DF7F8B272C6949AE8E919DBBAD59908B3BCA064C49D77488868F5DB71E8B0939B9BBFC9B366897D3139C49F2570E3C45F5C5CFA831FF48623BDDBB7EBD8F3A7A3F990919B57B0F16A99727402450DF0D8503152D76ED421693EF2F9A7635FEEA0E31DDBB17DE8CB1DC1ECDCA8E5DF79A4F9C091E6C3478EEC6B397AA8F9C8BE232D2D2DADFB77EF6997AA6262C7B6D0AE5DA3DBBFA46C1CDFBADD18D30FFAD8CF49D406FB37786A8335BC63B113881354CA4F5314CF59486D062F5793056D4DAB766EE0A5305CA8CDB05231478CB3DC7812DD2AB3E4F493ECE3562567A14F65E97688C841651B26AF947AD53525D1667B30C1454D1F881B8DB61C6B31AA8CF28B16C83C3910BEDCB891CE678A753E47711EA81889FB806D06550E9732690198DCB8A612957A072CE7CB709D6C3A7093CE15BF18C2C74287CF87478D1352DD3C6D7314E9783A1A16F234E6524ACBD0D98C1F97C32E667CC6BDE564E3B241BEDDC8ED8515BC436D370026917A1706002013398B3DD461E70AD22B5CDB996E34E68C8FAA50886AF781BFFCB579E78EEE2FB71EDBFA65E717DBBABEF8B2E3CB2F3ABEA4F13486135F7E415BE8FA621B6F64EB175D5BBF6CFBF2CBA35B3E0C1F6A6373A76B533D4AF565F9AF1EA86F6C2CBDEDB6AEADDB43FBF6B3BCE02AD6965A9765042B12C7CD552A401572FC403CBD951C44D730BC4DDA2DED2B4B14C0B4C863CD275BE20A9F8DB9223528FDA430941D25416044FBBA4F7CF271FBF69D9421ED5BBF3CFEF9173DDBBE38F2D69B03AD2DD54A15C9671A69A850AA4C5CDB295C2CDB3BFEF5BEF67F7DD2FAF9565A8BCEC5787B076DD38D8439C5621BE67DE9C3D00A86943C2B54D62F9C85BA397432FF92066B41D1EEDE74EA41A5866DAE362B30D3E6A71616E78579F25CB1C7057B9270AE5956AEB2E22F179B8E61536ED22126B1A198345C65393E4ABC1C8B9C591A1B94FE6CBF139014BB067B0151E3974F8F6344E980292025810B0E5DFC96EBB7E926C041DA4E6216ABF754836E56FABC116271B0E804D22DCC0E4DD0E04C4CB813634E68D4094DB8E321158ABAE3134E68CC098FBB342B1CA665E8E6668E4FC44213B1F190391155E3617722A26445373446637B22EC8E8DD35AEE78D8A1468DD7CA01E024F0714F7BE0E30E5293337E399548F8B827C41607A4D2EF79E58F27FA7B8352792F585C5F74DD75D1A121AED1A9AA2411A84D56D381B66BBA31E5FA62A641A2C997933D74787FCB17DB698BCB162F2ABCFD2EDAADDE3B2FE9D8919EDEF6575FCD2F295DF0B31FB3199C134595A78FF64B0BB43435E517142CB8EF1E4E0FA5249915AA1C236785ED1896A59C96A6C7F38A0A17DC73AF9E7D9E700A0B0BCBEEBD5704F329353E49A8C1575FEB3871229F44F9CDB758E5A5CEF884C5872E1673CE1ED25C74C8745228823577D4702D125D568CC4598C96B15957F10CCB6747EDE36FFCB95FA94AA5AA1B1F95CCD0CECD9453B451F68B6035D637D0F2CA2B156BD6E55F177F0840D91CCFD9E4323030F0D24B2F5120057DDC0F353553F62EF955DDFEE70E575E54517A4D49A2FC706EB0E155C246D4547E2A03E1CE58FB1B6D25F50B4A6FE76FAC728611C9CFB364200742238213E8AA58D4DEF5F4F6FA354B5CCB7774D7E175BFBAD4CC3648E4725993C548E7CB5BE7A449ED434D470B0BF2F237151F7EF7E0D2BB5764D70415BBB6D3F5454BA75C538C0FD375875F7FBBE7787BFD83BFB6825974A18576EF6DFBF863BF146E5A8006395B0CA59F22697CC680CE34CD94306D21A254CDC56BF3375DEBF2E71CF882C2CBA9409328600000301B246A634FB5CBB78FA81A3FF1C41391FEDE85CB97952DAA29ADAACCB9E4622337C75F5D15A8AEE2716565B0AA82C2D31A8295B4EE425A3DABAAD25F55A60A0B731BD62D5DB57AC9F265A1B6E347373775363539A323A21548685ABE8A8AD2F5EBEDFEBEFE3FBF49CA412A5956ED943C1EF334D7AA34E6070549842DD6B451B6567215ED9A6CD99FBE6F042B6BB9AB4B6A2985622024D81A6AF477F7901A285DBB367BF1A24041415675B9B5B0527278A1AFAA82F2C75F59EEAF58E02B2FB768A828CF5A50C1310BAA291C285F10A8E6FCF75555D202C1EAB2E275172FAAAB0DE4141E6B7ABCABE9F1706B333592B8F963902863DDC5AF2588D38C9C689D1816F709FD965CA801E685528F45B72F2CCC2F687DAE35A642C1207FB5C711EB329D173A337C9E254B0CE5A73CA23210A8084CA8D1F018B570691E35731C5E20258F8FDD3CA4B0E9DFF196D0B1A75BEBEA96859AA3E14363AB6E5F6B05A80149E551AE27819B83541CB85DE93394CD25C475A8B4F0F3205E945F03A032A3AFB894829264704F58DC83BB24D0197EEFFDFE8F3F2E59B2B478516DF9EA1595975F5EB5EE92B2458B4B6BEBCA162D29A9AD2BAEAB2BA95B5CCC03C52C2E59545BB9B6A162FD650BD65F52B56E6DE99225B4220DA5758B17D05A758B68A8AC5D44AB542EAB1FF96A4FFF9FFE64BA562A976D30FB40B80300668333D63C5CFFB9BE586874E8A30F628E9DB3A6A1F8E65B8AEFFC6EC9F77F10ACAAD26A8F6B716FE569DFAF582CB02C9297C61CEEA5CE9F9B5D70C3B5C5DFB925B8BC5E2B89818FB6B87D43A4FBB4BE28DCB86142A913C73B65A7A425D8DD36B16B16A2E299CAA67CE96D3129B033090FB42382C724DFD989769A88CAE374C7D8E19C82DAE8CEDB19DDB93DDF7097D6D6E55F7BB51D08B826EDC127966F3E34594C5C695DCA907F4D7CF1A5D8ECF5091299C279C3AEB76C4FE759FEC2AB3795DD7967FE55EBA931D0AED4C4F15E5199DA0984651CAD6CBBA4CC284019E56520EB1DD9F2BC22581FC8BE3CDF56B1B07206B60EF6BFDBDBF76A5FCFABDDBD7FEE19FE5337875FEBE979ADBBF7D5EEFE3F77F7BDDED7FF5A2F352543DDA1484B88CF013FC810ED9E6290A0E4B79BB990D0F975A2C7C2C35B06A8C956766BF16068A863A233D63DDEF7667FEF9F7AFBFEDCD3FB6A5FF76B2768DCFB6A6FEFEBDDDDAF77D1AC513536363CD6FB7EFF849AE0A3E4D61D1720DE780A1E2F5FA13E6E5473813622875AC68E34FBB2820B6EBDB5E8AE3B0355B576DFA09B1B2CBEFDE6A2BBEF2CBCEB96E2BBEF2CBBE3CEE23BEF28B9F38EB2DBEFCCBFEBB6E2BB6E370BAB9D896173221AB1B28AAFBDA6F0AEBB4AEFBCBBE48E3B0AEFBAADE4CEBB6828B8FB8E425AE5E6DB820B4A43DDDD63F2AE0BB43B4810BF990200405289AB6D8F442DCC76652FC801D2ACA1AEEE235F1FCCBFF6DAE2EBAEE72843641F23EA9348AC304D783DDE208954EFBD3781B75974D3CD8B1A1F5D70EF7DC7DADB06766C67C327C5F20266B67C9E492F2C096051A265843881788A423B849C9B29D52D4D9CB1FA8D6F48764463790220BDD74F0FDAB8CDE66DADC113B025BCF3D3CFDDFCC2C2BBEFA449DA9D777678AC05BA8869C78D0C0D8E7CBD7B74FB0E3E4F7C8CEC84CC5B9493C2036F3FE1EA620657342CFAD9CF4A283E1CF2E2F84752CE79C76B39BAF92026D69373938D242C45A1739ABF227BF14F9794A8C21135D6D5D2D37DA2BBEF445F7757CFF1EEDEEE13277ABA7A7A4EF4D2D0DDD57BA2F344776F6F91CAE956BD6DEF766873BB34C3BC6DC54789D337670D219DE7D26F236176BE73FCF8F8F1BAC63AD7A78A0B0A03CAD7B9BDFBF8F1CEDEEE9EBEAEBEEE135D7D5D72B07298BD9D035DC73AF35551544507ED81225562F9E53AA56DEAC6806C34A5483C04A31FC3B28EBEFF57372BA7E2D7BF0A777646070606FFFEF79696C3039F7E3ED67A2C76BCD31E1DE50391BB0E9F283A858E13E9EAEDFFE41FC7F71D3ABE6F6FDF8EAD13870E39BD7DB1E13199AFA1B3C9572835BA4B7FFC53ABAAB2ED8BCFDD6824958B37986566E4060A0000530C669ED4104E86B9973B1570ED204FD0ED48AB9399BF2F490A0C6E4358B46B4A29558B8996C6E4A47A90F6F42C81162DEAAA28BF90E719C8CF85AE6E13F29DD6E4BD4CD9016F995FDAA3DD9046A3156CE5A349AAE665F605C21B8C9F02FAA57CB62657FE129297EDD8CEA9ADE0B1FEFEA37F78B960F9CA058F3EC4EF4AF246F8FDDD787A2F243DF175E7359C85D4CE2B35963CB86CC543CB1B1E6C58FDEBB5CB1E5CD570D6A1A1BE7179B9AA943E4AD8358BB7C282569EFF78AD4816F473DE672225C5A576A25C0B7E97AE266EF312153F2F5FF3EBB5CB1F5CB1E6C1350D0F2EA3C08A8756AF7C70F5AA5FAFA2A1E1570DAB7EDDD0F0EB86250F2D5EF6D0F2350FADAE7F7049A0828A211DA96EB77FABC23FA3586278774C15502A2B3F18E9EC6E7BE3F5032FBD92B562F9A58F3556DE7E47E77BEFEDF9F3EB437FFB075F6574EDF3C9E79315EAE86A7BEDB5E20D1B9637362E7FECE1BA9FFFFCC4BF3EFDEA8F7FECFBEF97E48C9E722A298206BA66F93A04601210EE00803983AAABD17F7ED4F5DEFB75D7DD98B77C3155DA5A29CE345497B29826D59E135C7AE7BF4D1C3A3CF8C757A6B4344E816B5147CCD8DCD1BBEBFA4D96FDE74A28EF20A1D713F25D6AE3C93BE285D82627DFA651D27FBC8A99D2419C2DEEEEC9411A488996C924B8CB1C694BB057BD3F3F67E1D557E5ADBF58A787F48941628C92C24927F93829DD603A706951B6E9FA8DA0F2052C3368A82C87442E05CE36D04A8BEE5B989395DFD2D46C8CC7B89CF019A45696296F00937C67C92CADABB9ABC7E5490D1710651E6D3AEA38CEB29F2C214DCB49B54C2BE833027C8C2AE037839615308CA0E16691DA35CC6CD3C832AC2C9FF21B56C032FCA611A4E6894F3F95912DF2D1A51A722DF0987EE95CD0D852562812E2B7B497D4E76DBA929609D4542FBCEEDAA8526323C37C714B7F4074058D7EF679F75B6F565FB63E7BED2A3E6B86DF57585275E7AD4574C14722B480EC81987436E9FCBA06541A98028A0400602E19EBE919522A77E55233984515151B98CEC392FD2DA1DA94207DE1F8CC40DDC231A5067AFBCE61E4E3AA57B1B4E53089149E94A49E1DBD0B6F42A04956C8DE9484F878F532DC690E2520C6B13E523DAC8792E7434F9CDCEF24E4B0B81543214A9C1BCCCE5D778955524CA7C035A2DA3D8396A199ECA52EB9409C715373454A25E6ACC88996FCF3FAFD1417ACB397373EFB8E5560155C9367A960D72B3DB1135218B829456BD1A920F94ED296169CD3F614B5F0E88046EC137F3CEE53C1A2F545FE525FFCCD0A393C7983535EDEE09453BCB8C230D212E16CE1492E7E5CBEE4C0F9ED115D1E65C1148292EAD069948B82A0EB7FB0A737BA73DBB2AACAD2ABAFB5B273F8407C81ACD5AB56AD5E150C87873FFC888F858FCAEAEBED3AA15470DD655656AE9737A61BA85B4A81084FE8A70D5361E9EF05F55D0800087700C09C62B9DC67B9CB9FF131B5969E350B62A2A7E86CAA50B9563EEB7E758D19650F72522522AF2F2891BC9D4935B18626D99ECAB66FD347AA203211DABD9BEB72DFB91A06D34324D1E915BFA714B84B1BCE7C8E619D4187460D093FCF6571C8FDD98B8DD0CB2E305D38E7A5C070F1D1D918177F67443C2B48FE9A39CB72CBBFBFE0C844CB60EBA02C2F7E4DB4AE3E73FCC33173886D1AA191E8E1DE43D9EB03F997177AED134E13BFDA40C7AA8B130F1490783D48333601373F683DAD5E79D63972670EA1EB44524689A4EB842E8A834A751FEF2EFDFEF78CBC5C7D03E1E75486957FDDB5E3CA38BA7F1F5D3A942176777FEED0702D5DD33E9B5B2CFCD044BAB4775549515196521347DBD96F4DB7EA1248269C1A05C0D92B2A00009869B8AAD69595CBCE017447E2F16C08442D9EE40628963DD9FB3960376316F754D33AA67F6CDC9918E5EFA49C6D181F3DE340ABA8F1091D8E8578A0801A1F3326C65D8A0C8DD29EC69B8F1CF9F86312EEBE8222BDEF1941E4807CB79215152B25FD457D7EE2C1BF22BF386B488570FED0BFCEAE6FCEAB5925A51273562827393FB909441949852921E5CF02CD72E4AB4C4CB92AF687F80C5024ABC3F805C21B3993997636E15E65424EA95AE0F305C4C14ACE08B7F7E4E8D8C16A526A791EC5278638BCB07749F2228E744DA47BCC4C2938717C207C28E22654A8147F1D202649950F06F313030A1B56B6EB14531C2B7CD5F6A7974787472B1B1B1DCBC70F13780B7E8AA7B9A53FBFC7BFB0BAED2F6FA908297ED981A0CF2B675A1C5D02009874E50000C0ECE375F6E8D5EE044BC9198676413B657F7AA91AB91AE6C9B323CFF1173FF21B351ADAF378D381DFFEB6F9F9DF1DFAEDF3CD67198E3CFFBB161928A0073D49AB1CFEEDF33ABEE539199EFFDDE1E77F4B032FF6DCEF9A37371DFBF09F134AD55FBABEF8B6DBBCBDCF042208F4B7A5384093FC322CFDB2F860FD4163C38879DE0B3C839701170635F8B809C43948C28E0A1365A9D795FB59A0226AB21F8A0A540416DFBBA2FBEB81EECD1D7492442ED2064820D2EADC53FE9CEA3967E0BD81B6F75AEBEFA82BBC2C9FDAE0BAFCC8C1CAB31A4EB008595DA2CE02BFD24107A48B226B7ECE29E9D03DB59096AC5C0E9CF57CFE96FA7DC59B36353FF954B4BB47AE2993BD7C943ADED4641ABEDA471F9196702C46C7275B1003BCF7D6397BDDC87307D731A234E97918799C3201C02420DC01007388F77458DE49D5C6434FC8CF28220BD896AC2769871423D5F139709465E55E7F6D754545C5E2BA8ABA450B16D72FA0F19986D27517575EBEA17CFD6515F181C234D45CB6BE6CC5F2A245750B16F162658BEB4A16D795F276EACB68B28EBFC0B2A0BE6EC5AAD5396B57C5EBFA642006DFD33356D439FF88A86241494D1489E2AA8132C4C72B7AC99885F392A9701EB31B98E42A378128ECEACF619E0D9DEDDC9FA6A17C85BEEA8D1561D73DD6D4A99DE3C5EECEA28FCF096D7D8EE87EB96FB065B06A6D75D6C25C6E89B0DEA6E489EEE64316072B1A28956C87F6F4C64987108EE7C1A283A595E89845CBCEDD017D0394B2F879E19348C2DD5F58102B2BEF52AAF54FFF1DDAB59B8FA6B7B7A3A9E99052A3457986E9E353AD7C8B6EBD253727B7F7D9678D189D337986C5D9416D1573F09DB7ECE39DCBAEBADCA496DA14E8EAA325013895D30A0A000024832955CEE4CA785258F7A322463976EA1566A7DEF6F6E2ED94756B3CEA0CFBE70A9BE7E7AE6C28F9E10F0BEFB8B3E0CEBB8AEFB8BDE8CE3B8AEEBCABF0CEBBF4B89063EE2ABAE3AE6071B13338E886479D48D40E8754246C87236E38141D19CFBFF892D2BBEE2CBC8B572CBEE34EDA080D8577F28A25B2B592DBEF2CB8E17A5F5EC1B7BC3FF3517C73AD9FD88504681D7D9C12C3010EC67F75F4393875777AEA1B579A3778258DC694C7DFA44EA53C2AC736B8434843C5F2D6E7E737E447D478EF6BDDF688E42D297FF642E1C5780D2FF329CC6E5DC4A967E3FC88AF482A5C4CFBF1AD700B21B15927D26BF7BCD2DBF35A4FB87F34AF3437F7DA42DB6FB3ABD5D97629C79C38E093C72EF132C32B87F1B2274C0AA6129C54CE1AEDEE148E669595D4CB971F86B77DD9F7EE5F87DEFF80662E55AA62D9325A50FBFE04972C8B95141D0D85A276880F9FE274E7F7863ADED63EA0DCEC8B3718FCE958156E3E38F0F65B83EFFFA3EFF5D787FFF9F144D7715E0C804978570B00002497299E1593EBF4B3D5EF29C2749267C6CD87ECBA604C84426DC722BD5D831F7DD871F850F7DE033D7B76F5EFDDDBB5674FDFDEDD27F6EEED3EB87FF48BCFA27D7D91AE6E598BF03AAB493A9CACC429108F972967248988C49BAAB4F4548A9FEB94C631C4144D99C8FDFA17DF5858714D754F57FFC89EA1C8F189485FD4ABC1B583388B78D2D662D86651C85DB84C179736C2565E79075B6268A35290E8979D4078AE638DEE1CECEE3BD1D9752C7F55E9829F96FB1CFDEEB28AB1B78B5716321EDDFC088542C160B0BAB1910637E6B6B5348F0EF4563DF4E0C2471B732EBD84BDCDF8C2E3B3939B5F50A654ECD831158B4A9E7293CC1918A0067A4920C08F1C59CF3B035F6EDDDDD6DE7CE8C0506767C7D7BB8F29EE2D1E80C940B80300C00542F52FD5B6E2B1406AC91ADDBFFFD0DB6F1EF8EFD772D75DD4D0D8B8F4C1DF2C7DF8D78B1F7A78C9C38F2C79E857CB1FF9CDB2C6C6706BFB97AFBC72FCB5D7C4D195FF2D7EE43EEBB07400290A17273A412C9E4988F3F796E4F9552CFBA2EC15BF68E8DED9F3C59F3F3FF1C76E9AC5E2514B6CAECDD9D7497C39B4BFD38568686EDD1916FBDFEB9E9E78F3BC4192E5D48AA0E6E9B1A7DBFB0FF5AE695CB5EE37EB0BAE2F64A5CF89E51DFAE85A38D994CD642893E83013FA490ED859F0E0AF2E7DF0C1450F3F62FAFD3C8F1B3F3C87B2D471CDA21B6EA8B8E5F6637F7B7F64EF3E69429B6E68A2F5A597726B1655FCFA4143597417A0ACF429CBAF549E528BEE7FA0C8CF7BC8FCDC04D32451F00000004C0F76606597651637BD7F7E6DF8F32F96DE70DD922B37E6AF6EE0B9414BF9B38C805FF97D4620DBF559B458F11D77AEBBF686C2952B8E3FDE146D6F1767E773F93A2711837BBD88E3A9BDB42723658D3E3922FAE8AC5135AD75B88F14B295ED2EBCB1FAD29BAEF4E75ACD4D47ED716EF5B176A6E69F768996B714C4E764DAF53BFB49C97E4D973FA7C07EF92231E93F46B2DC358F3575383167D1F79750A461D9A6A15FC5A4FDFAB4278DB42574F2331C438CE9DAEE4E21F689B1FC463068F8A9FD42A7801B31DE29700DEE79D655598BAAABAFBF61F05F9F1C696A6A6D6A6A79FEB9E2350D45375FC3FD39E9ACA76587072B95AABFE376333F9B3FD03C093917004CFFC2060000A0619B1A6B1B67E0CDB75B8F77F52B95B36A55EEA5EBCDE2325650D2C9060DECF0C05533AFE05F5C9BB776A55953DBAAD4F85884453F0B9E1986DB179EC89809BC8A64160E64BE102F302CF8A88C50063BB6CE65D3C85D9997B73250B4293F4B65773DDFD1DAD43AF4E910DBC4F56BAF86C38677297EB2C27490C24CDB900DB0C15F52E28CED9F38DED4D6B1B9D3A754F1C6027F25F7F12F5BE70B807E3889BC8E7E6B9667642A3A57F9F025E44D529E27DA2BAE4D71EC0743B9E14ACB873D884C6EA10782747FC85B549BAF1435E773F38B8A2FB94C6517489671B6D38F6DF35394C0E27A6A3B5180B6CEF90BC02420DC0100E002A10AD98E4E8C7FB675FCD8B112A5169695D24D95BB916005233639AE8BE936CB775A5DC78B0833737272EA48E5171546FB7A4287F6F3B6661E2DC76602D216DC2A3875F37CB433CF4C1DD29CC2C28F14317B53C9175209F177A72CA542447A904460706956C54F17445474548D8DEF1C1F3F12915267B2855ED6108BF03431A5AB42E915462CE8BCEF70576CE4D381513531AE460A6F2AC85B5FC4DB974701EC2123659B12CB85CBD3B273E1FA355B7826761D4E4CCA8BB9FAD85DC332B8B34EBECC2993E424D049E12EF96519B7F8CEBB173CF69BAA871ACBEFBF471514E82DD0B549D9291E476CADD76A5DEF25C16CB4F0413AC0971C0000800B80744DE8E091F61DDB8AAFBF61516363F14F7ECCE2892B68BAB5C65C15A55A993FB3CA1D5B731D2E2FFF719D6CD5D454373EEA2F5FD0F3CA7F1FFF3B774331B3885D74C6743B6F5C948687E8376EBA800B83251A952193E5A00E8B8416212886757997D1F295588B1B97AE695CAD0276FB7BAD2A6C46A3B63D613B613E011750BFD3893378254785951A759C516E211C7FEDF84428B2EAB195CB1B97E7AE2C10F9C827977EF5A3000A5162F9749366E5D85972FD9A7D3CFBBA8CE94CD08F1743D920C53DD1C3AC86CF15E7149D378E2739CF6B519CC19F53E5D68E6EE570A6D17D81F2901B65BC866C90A36526009399F6850D000040D3FFCEDB031F6F59FCFDEFE734ACA09A591E88735D4BB52F7FDB48F969191F45508DCC153CFD98ACEB791912BAA6A776784B330B692BD207F2B175867F932A07E8606CAF1B7E461B11C105A39B589C9FD4D2A3B2C393DC382221AFCFA17CCED6650D28543F5453B1A662EF33FB9A9F3A78F0F9FD5D2F1C971238B931755E909814DD68F6BCD3B3E3773B0FFEEEEB1D9B77E4D7E72E6E5C2CEF7398366D53B4A84E18C1655EFC7924B91499C90D3696DDD4B6894B763A525DD4E590E9C0B90DC3F3384F68016A5CC945C1CBC8E993F3C57A5DAFE29AB42556F494698E296FF5D24D8333504E5F8C66D1657B4A530000087700C00C2115D849A46EF3981C4E41CE963C4F09E9E39271746C6244297F4595322DFEFE2155DB5AB7687133095DC1535DAE650EC7F03457CC133C5F10F1E3659BF7333DC4FEA9D78DFF32AC186A6FB8D61D1A3CF1DCF31C4989A1545C00DE96F51130A1AF761D7FF9E5854B96165F7C091D91448B47473C2DF46FB8767C71513389097016B86C784588EBE87880072A4862E7A67FFE95594CDE86FCDA7555C36AF8886AE98E74756E3ED6DE74A2BDA9ADA5A9E55893D717B856D7F1EC174DC9FF7C9A7A5FEB3AD8D47C64734B6BD391234D8763C3CECAAB1BAA2FAB5EBE6969A4337AB4E968E7332754840529094A2F25B2214A00374CB944D1944E64C642474CC769B3CF3A85F880753B4AFEE5D8752B4BAE2DB9E4B9692ECB519B87E6CA0CEDFC44D9CF3F9C5D7CB1D0767979F138A218BE8FF8E8E62053009C42265F6300803944D74009A496F7981C4E41CE983CAE44B5D4D1C725E6354FCAB3ADCC760D9395BB54CA6745BE992886375D61BBC5EB2E2E576AF4E38F9D48982B6F6DB7E35927C5EEF9436B26EEE9B40F4A8E6C84654160D5DAACC5F5CEC478EF9B6FF4BFFE7AFFEB6FF4BDF646EF9FFF3CBDE1F5D7FA5E7BABF78D377B5FE7C9C1B7DEEADDFA39F75E77F5D56E5111ED4B273E910586E2372A0DE5E718ED2FC49F7CD733411271A8DCB8FE406DA0EE527343AECA3DA0F61F5107F6AB83C36AC89F65F5BEDF3FF05EDFC85F7BFBFE3A30F85ECFC0BB03037F1DECFF5B7FDFFB83BD7F1DE8FFC76064D0CE51590115F4AB6080CE57C074861D67C2898D723B61440D7787BBEC58449A0D3EDA1FBB7FCDBFF3C86D1529C172F59ECC001DCF172D8749A2F39D802F743D5B6E1562389718B975904E97B974BDD06D85EE1E0E451AA4E983FE8852C3EFBD4FF186C9B70B00A600E10E0000DF08F7D8C6A23C519152CD1B0D5B91683E8749CD70578FFA51B8D8E0CE0CEB72AEE0BD254805155EBDC958D37068F7EEC8E8A8C4512340F400BB0B930C9836242A4804500342C4816877D91DF70772C7AD85575C317CAC63A0B373A0F3787F57C7D0F1E3D31A868F770D74750C77740E73F8F8F1F6765FC4AE6C6CB47273F9E3FCDC4E30C5F428CD0FB632B2F288D10C4E114B153A382D584012A1CC0DED1B39B46D6FC1CDB9D58F56545D5E5DA96A17F2B0B076496DDE5539070EEDFBFAC881AF8F1C3DD4BCFFD09123075A0E1D683E7CE8F0BE0387BF6E696EDE7160872AF1D5342EACF87969E53DE5358F2E324DE7E8EEB6AEAF7B5A771DC9BE22A7EEE2BAA0CA51868F5BAAD256A5F3EB85E615EC00C31728CBED846CD717B4946DE93F8A24BABE138844E7F6AAF786B1A3F81B567C33A1D97255F2F542FA9CEE1E34E66B43B95985C34AF51C69762251D3A73703C02940B80300C03742152F55C971D704AA730DA7E3A967A3C3C3358D8F9A862315323F0AE76AFD5C95AD98DCA40120F2554B1F5FB6527E09B3D58D6B6F6DC39B76AD4D2B383A2DAE4B3A80253B4DD0766467A4A4B3D75F56FF9B87973FFC9B250FFF7AE96F1EA6F0B486258FD0F09BC5BF79B8EE915FD73FFCD0AA871E2A7FF4614E35279A1DB0F91878F7DA31802B188A3154CCEB1A454617E8A57316F491CD73285F4DC397A502A61BA02294775941C36F962F7D64C9EAC655B14167F7077B7355619ECAA50572544E960AE6283F8573556E8ECA0B28FF0255D4D7D9BDBB69EFC1970E1DF8C3C12F1EDFEE84DD758FAD59FE9B95EB1EBAB8704DBE3D1065D149794D8D423AB714702C8B4FEFBC834BBA64B825D7282B709A36635ED9666CEDE2C273A9FD2C9DC41034A92DF02CD6E912106DCFEA5D9BDEE922E52D386AA8B74CA9A58F35F2F71F22622F8803150F3410EE0000F04D88930CF70FC3BF518932E927A6AB61F670271C51AB2297CF060B69AAC8F91D3E99E4AABC74C3650B375CD9F5F27F8F7DF515AB6D51F5FAF3F2D365E18F7F6C8C8CED6F7ABC7DF3E6E8E85042FA8B5F3DFB36501BC3B0FCFC29287F9661FA4D6B7A83F2F90D9F61983E9354A23FA002010AEBA3604CD7E46FC6F0F10FBCF2C7834D4D9D2FBF5C75C515F9575C294F0F127A1DF54ED2D18E2B3E0A70F1315DD7A233C3C1FC1B0AD75CB16AD9758B97DFB07CE9F54B96DEB064E9F52B96DCB072E9F5CB965CBF62E9F54B69BCE286958BAFADAFBB6A51DDA6458BAF5CB2F2F2E525B794D2062DD330A82120C839A346299D624B5CC2CE55CC331ECE0D29F696B4B10D57E7BC1AFD64DBE1CD4FB43535850E1FA22B9D62F88EE059E7E986C1F70909D3BABC0D6ECFB23B3B377D696B3A52EB7ECA690AD01D2671D900A0311A1B1BBD20484F5E78E1859191116FE201A53678C1937428F5EFEAC60D37365CCE5F73046076D8B367CF962D5B2870F52675D526AE96A4C65247FF438DBAAA3E7EE7E9FFEF3FF5F7742F7BE46112853A66F6696F6AA21AB53A9EA4E34D4D54C72E9A746F14470F12A4DCA1044DBA5413BB76FBE6A7A8BAAE8A2FC632867B6B160D7F76B4DAD159219942CD015F747870DF8B7F58B87A4DE9F5D79020D0B313BB9B16C35F7C39BE6D9BCACE3627F8A39A2404F42648019084D072CB60314181696FDCA6B44AAA69A0E394B714D9DF97E2699BB265F6E5A5FD5A3EF604F6E515147CFFBBBEAC6C9A25EBEA3D92146181925C0606065E7AE9250A542F54F7DEA3E3D433CFAAFE3E0EDC7FFFFDF9F9ECD6949150F68EED1C3DFA69CBD2DB9765D7677B513462396893D4D693DC5CE427313C97E5A41435396B829E66DF0E291ED2A1B8251D93F7BFD9DF73AC77C52F169BB901397B747DCCD9D53AE7F4BFF1C6604747DD638F1EDBFC7896CF57FEC8436EC4EE7DE5657B64C4342D373B8B3FE5109E88386E764D75C95DDFA32B44B296AE382EF662608F677ADC798C4E133BDA18AAABA92942379FC71A29BEB3A9A9955AE34AD53EF4A01908BEF3D4DBADD136F59FB2E2149E546AB7175CBF7EFDC68D1BBD09908924FFEE0900001946DCD1437E44F0BA067F3C928D616E4457C4A28445218979FE6C180E77CCA1915F165254D3E7B0BD94224C6DD517D57EAEED9C1957156CD850D9F870F98F7F48D57F48867119486A455C9726C31CE94E70C09DEEC01B916E70683C269BA2C809CA02D9A684399EC6590DABCA1F7EB8F8E73FF605B329557428DAB540E719483AF2C2A44FA421973F2A5494D194E32CC5B95F426E6BEAC266A898168DDC4BBC58EAA9B0F1A911A70E0E727391BF19246D50F97E27CF8A1ACA470294DA012EA9D0798CC91FA9E257543967B228939CD17F7D1C1A19A1626F16E755FEE217E5BFFAA5EDF0B51639D639FAD50E371C96B6B8E9F988D18FA1EC8181D18F3F1ED8F241ACA797CF146D8C16513131BFCBBD862F1900CE002CEE690F2CEE2035C9248BBB569CFC6D43A956F98D4B83D7A2D8DAC71AF9C05C7E05D3C79A9C55AA78779F115A9B5F1B1501C5767C12415467478706DA5F7CA978F5DAE2EBAF1543B6CF5B7C9A483AB9CAE704B2B5952486D7338518F41C7EC956922647307DB48190437C203C4911D2C2907F59C683E52345904811EDA857E4439794249FF96C7127C6B70F1DFABC6DE56DCBB296644BABEFE4C990F32FA5429F3B3E3172FA182A21FC5AE4E4D3CA2A9E266531C735A9D5DAFF76EF89B69EE5BF58EECBA1324EDA92BD444E39DBF3039DB1036FBCDED7D1B9ECD1C75A1FDF9C5B90937BF1A6635BDEAFFBFE0F039515DA704E99493949B78BD16D5B8F7FF9E5A25B6EF32F5D22CA9CB3D79E085BD9C1912F3F6FDFBA9D2EF5C54B5714DC7AB33B3EA1B2B368E3271EDF3CEEB875BF79D8B4FC5D4D4DCD4AD5C0E20E4E655EB79B0100330757FC93985CCDA778957FC6E4B11617FDA955BB281ED1D76246A3AA9AFD40F4DFD9EFABDA202A0FCD69100757FA13133E55E196EBC8871445B59F9A7BE7896C5036CA3FF42BCEF7122B5AD953ED84CC9F3E7278826C8877A47778FA16E9E86819536C8D8915790A76C4E44382D192B745596AF349A11652A20851CEEB13E49D22FE4914513941934EAB68539994C5F4B3266AFEE926A694555E57DB85E71B866173ABC6657F386AC0D0B87D787C78CBFB4B7EFE737F6519653E976ECE3D718D3154EE6597D6DD796BE7DFDE1BFDE823C948CEBA13CF3DB3BBA96968EBF6E53FFFF99A5FFF62A2F9E0CEA6A6CEE79F33A2513E69F985C7956A79E2A9234D4DD4E40CEAF300C02412572F00002413313B9D64B210BD20513A7B9C9E3CEF50B4E3819E705913EB255924F10F8FC5AC7976B8DA6739CB7A97D53B2FCD8332D8775C2A7E6F9B53DA3D009C032A3971F71592DAFC9DAF44419D0EB422954CD2EE5C4E27E1C85705689BAE410D4B0D3F6F997F70F3DCE5D7D0292FB85133ACD42835B98B8B1D16F32635A07836CDE2B3402D1DCB2AAD8C28150EE9CE5E09A75BDE0F28BDE1467F51B195955B72DB1DB9A4E6D915894F58E1B557AF5ABEAC74E9D2C2654BEB365EBE30FE2E3C000920DC0100E09BA0CA5874CA4945232647AA692986048DB8BD70BD7B3E8EE97A230961253FDC91226B2FEE509277C722E982FA7107F3126EE8B162143DEDBD21FDCD25712AB4A2C84D9190B4418D48764717F244F19EFEC63303CA0E435FEA9C278C5651D4E0A6CB953FD360383C5B378194639A26BF17CE1DA4AAD8C4E8E83F3EAC5646CD860DD9AB1A684394BBFE258BAAAEBF9104FAE8DF3F8C0D0D66D72E2AFECE2D45B77EA7F4E65BF2D66F085456C9E6013809843B00007C03A46148BB48A5AD6536EB16AAB6B9E6D6224757E5123A87A4D18A488F450B896A971568449289FE794B34D0F4377550038873E4F63CC2F1E92F84E967429C27FC30470BCB6941CA93953AB703A870724965BD2E5F02A55934A9DFBEE0775ACFF9682963916B575FE3DC8152B152594A858FB52BDB26ED4E27219EF97CF19A517BA2F5480E2D935F60BA76B4B7FFE08103FE4D57E55D76392FC3173A3FCAC85BDD507CD71D875B0E8F1F3D221BE691DE936B259A4AF334C3C1E940B80300C037E0496DADCED9EEC8A29DEE9E342DB52B47D3982DE40943FA99E12F3479417DFB75B83EF6C907556DAAA4B9BEA71D5C80E402F318368947B957222346E5874B2097C3E9D7EFEC6FC3A59CE5BF8E90A6A4C33DF4B33B0E17751E12EDD4F906E7ADAB623A67A34A552C2829BEF996A36FBE15EEE9E43CE73EA3E4CD73C9AAE19D3BDB3FFC67CDDD77E75FB589DAE196EB642B15706C96E0FA4661789F41B0959B27667BEFC99E9C01BA57F8E86600C0A94CFFC2060080F98716DC52D7B20D4C3F2E975A9C917A9A55CF245D7E66A80DE0F9C46BB8827662A63B4AA2C01169C48677468FC1B9412E11EC726DF017014CC7EB8FE81BCBE1D9A015694DBD32174569B39AB6CF8BD1255EC6F3F0FD54D2D372F49C1BDE553A16CE5ABE6CE1860D27FEFCD6BEA6A69EDFFF81E21CE51CDFDC74A0A96978EBD645DFB9D1AC2AA3BB055DDABEEADAFADB6E1FFEECF3C1BFBC237E4DDE8796873FFEB4EFED77175D7F4DF69A35ACF945F7B37D4019EC321F87EF3B0040B80300C0F9E019DD3944FFEC49AC3B3227A8A27595CD9E047AB1B3D7AF544DD34C31677AA2C775EDD0B18E8157DFAA2E2EC95BB59C743BFB19933E88EF0D806FC4E2C73534327ADFEBEE7AA5EFF8135DC737B71F6BEA98EED0BEB9BD63736747D3B1E33C79AC6D735BE7B31DBDFF7DE2444747484D18F23D5F2ABED44EA0F2C9CF97E61D0E3557E8D8B9892E22DE8C84A8B59D7FF1A5399515A57EBF1D8E746D6EEA6D7ADCCAF217FB54F6F265D9CB965B3EFE00993262A6650597D49F50AAED68EBF05FDED5B78B918F3E6CDFBDB34BA9ACE5AB4C7F9073983F7A45B368B6831B01381D087700009806ECDB22D2BCAC2837A0D4C4BE16FD221A770723F1E7A86A3DF52F0FC4B5699374C07873EBB1D068C92DB7659557CB875459D7A3BA06D382747B58855BD4D1AFFB7677D81D11FE1E5638AC42D31AA2CAD60192E9B68ADACA3E126AFDAAE7EB7115AA503586DFC776606927188E6BF113A6F985EEBAC75131CA05D3656739DBF0513BDCF55BA53FF861F9430FE75EB47644BE7A5670FD8D150F3796DC7C8B7E53452E6712E1EC69B3B0A0204FA9B1A3AD235FEC1CDBB17DE4EB7DB94AD5F92C831A028623AE73ECD82EAFA99BB1933DB802E0812201009811A63CD89DAC44535C959E3B796C2FA7250C5576CF2F82650B3A3FFCAB333AA6EFA55AB27B2E0489C317AD9398E2B9EC0C63B18A771DD7F05BCA2EA0A54CFEBCA5769095D6815E1C9C1B2F73699C286FDA1549BB8AE8B708331E3A4AD352B92AAB549595A9E24A55B5B8B1AEFE37B54B7EB3747AC323B58B7FB364C96F962D79A47E51637DFD2FEAAA5575A1CAAB54950B1FAB30A89DCA0E62BA9794F991B353E0C769261D7AA2704936F0F54EF942C1FC8D572C6F6C5CD4D89855BF945F0E961B421CCA3BBE4B2CB8EFBEA58D8DC5D75CD3BEEDF3E6CF3ECF5E7771FDA38F953DC25F5CE285F82E202B4A2B5FBFBDCAD100C441690000CC08530CCF52FD784C0EA720E79FBCD21FFFB872C386F6177E3F7EB8852B5CAEC269902EAE4539F2BFBCC847FFB259129AA6F4F22CE2DEA079FC7153FE58BA4B828816E12FCC53BD2D0B836F44B797E4DD00DDED1E4DB20954327C52265269CCE0EE35E960B397662DFD49C3D29F2E5EFBC8FA9CFAC0AEA6AFF63FD1BCEF89AFA735EC7DF2C0011A3FB1E7EB270F7FDDF4F5BEDF1EACBCAEE4D25FAE2F7FA04267B1E468DC8D9ECBF53CC3116721B68DD3B51A8D2815E16E1E3D1DC5458E2F5ECA18BA8229C3CEA1AF9C40C3AA953FFCD1AA1F7EBF60FD65EC01EFC5EB1C9615252AE6FA429CD5B2DBC452607E03E10E0000178AA1CCA2E251A5ECE89854D97447E56F4FB2A689EB75429C62244C9A5D3425CF2621E4AAC1F73F0C7DBD67C986CB8CFC42F9E8227F3C95843FEAE8F3C331548CB28AF494C9DFB0623863298AD4938CE391248ED8692163B1AC4099152CCD367D6EDE2579D53555954B1654D4574E6BA8E47145F59285E54BCBAA1695572F2FCF5E9C6BE558BE5C8B5B43ECE9117F16C4993BFF908BDA74A4EF4D5BD5DD717BC036BB5F7A99B283F385EDF1517E6426F78144D93B13A665F9ACF272FF824A2B2BC8EBF22ADCECD74D7ACE63D7E97FE375F3F8F1C5D75C6D9ABC437D1BF1F21FCC63B834000000B800A80ED5EFA88928D495AB3DD532E670D71C3AA00525D7D02C2E59FB9C3874A047A99CCBAFB0FC3EBD162F7C2E5B1D980C651477A8E78C38F63EC712637047A77AF73DF5FAABEABD77556F9F5E8CB23592D18A879D2B1CFDBEA8ABFC1559A5DF2D2BB9ADB8F4F692690E4525B79715DD5654F69DD2923BCB0ABF53AA72B58B36954D2AB45C32C5E6AC8BE9BCC3A6F2E63A74DD52518AB9317FDDE2B16C7FCB407FFF5B6F3BE361BE17288BF247770C7BEEE73B54663953A5FDAEBDEF786D1AA4456446A3831F7CD0D2D139A0DCECB5EB0C4B5BE20160503D0000C005C2356FC02C50CA9F9D130B8D454E74C9F371B14AB2415D4C6462A5D301AD75A88676A2D1685B5BE44467BE5215B41D5EDE7366653F195A8E9DB7C59D069C13CE644305B2AD40B6FFC7EA2739FC3D1CB577B73A74447DBD472FC2F88C6C3E5B194A8C0525AB462A70DC20E423F524E074F1D6E1873E7A7DAF91C9D9CC5D1A4A21E526A814F279060B26C3D4F9EBA362671825E595A54A0DB71F0B7DBD27D2792C74A425D27C3474A43572F8B07DF860F8F091330FCD8723878F860F35479A5B68159A1C3F7238D47C64A2F9C8F8D1E65047EBD8A1C3C3070ED2CDA1ACBC0C2A0D4CC1686C6CF482203D79E18517464646BC890794DAE0054FD2A1D4BFAB1B37DCD87079831703C0CCB367CF9E2D5BB650E0EA4DEAAA4D5CCB6B4D70F43FD4A8ABEAE3779EFEFFFE537F4FF7B2471E563E7E376B4E686F6AB294AA8E27E978535354A945E7756F646D6DDBAE6559279A9A424AD5D15AAE8A198E4FB4146B9EC9A649D63A5165F8C32D475ADF7D6F50A9650BAB4BBEFB3DD6000983BC2CCE529EB4914805703612858A4F84434AD334ED1877C4E198FCE8439A4C14CF8E0616B588B83B94CCC455B6E158FA3B49F118269E3BE78BB796BCF2CB658F1DB559C1CB23A098E1F2379E88F83786A6BDFD74470EB9F78DD7073B3AEB1E7CC81FF4490E19EEE8D8D1DFFD7654BEA2AAB3C496C2A6B3F38CD0623AFF68497D91EB152932A614DD0A177FFF07FECA4A6AC69B062F42F17F79EAEDD6689BFA3FE38B4EE649A5767BC1F5EBD76FDCB8D19B0099086A050000B86048209A96C982704CA971A9771DC3F6B190945E6212752C692156F154E95A1488DAB121AA9B49B5DF721BBFC7460BB013368B7C835F80D34BE3FEFC0D18F2BE29E5390D24D3B9A9E4337CA665F80CCBCFBF069D1B9F49639A6F65F0130CEEAF84DB2BECA421D9417A928DE27CC8D318A87C721165070E6EF4F056B9048B7B8CA17CF4CF65524772CCBC8332802E78C56D711FEB69CA35CA30C7CA0E56DD7CCBB2ABAFADBDEEFA9AEBAEA5810255D7DE50CB81330ED72CBCF69A85D75FBBF0DAEB165E47C3F514AEBEEEFAEAEB6EA4B94BAEDEB4F086EBFCA525BC4351EDB26F003C50200000E08239E98E507DD30DC539599D4D4D5D4D4F8C1F6AA3DA3DC662886159C9929C8D94FDEFFCE5D8E6A6F1BFFD7D59F5C292EBAF73B303E2DB1A6319C46A9D55132FCECBF21438078E6875CA2F7D1658777266929AA2ACA353636A0334F79B4FC82B85190A1DACE33A74BC96340EE950F9DBFB52C54F63906CE4CE7974D9D3DE30AE6B4B9BC7F399E145BC595E783EC19F59A326111DBCF8C199F2DAA8E958BEECE5CB722E5A95B56675F69AB5D96B56E7AC599DB776A584CF3064ADB92867CD45D9ABD7E4AC5D93BF6655EEEA869CD56B73D7ACA640D69AB5818B2ECE5EB5C60D04F82EC0E745EF2B8E2EEB601EC365020000920E576E93985CDDA478D5339DE4392C5EE4BDC0E0CA55451B36700F714A8DEDFC726CFB97639F7C36F2AF4F07FFF5E9E8279F0C7FFAE93005B66D8DB4B6855918A9827FBBD357504CABCAEEA447146DC8D42A8AC2937D6CC09988DB7E2528274E67A0645DBC7613F5C3F1995CDD7199A1560CA1738007094D1736ACD358CA9EDE44FCC56B9DD95E26CA2C2F9C89482B857E27B79E29A4D5BADCDB7C6EFC936BBA9F28FA37B82F57413781CE9A3F7AF9F819A25FBD3C4752802668ECC568E474C8C39079D95C02A792C1171E00602EF1AA99385CC3C5991C4E41A6913CC7E22A953F05CF2FEE65AD5E5BDFF8E8E2C647A3BDBD873EDFDEBDEBABAE5D3BFBBEDAD9B16B57D7573B8F7FB5B3E5CB2F732E5DBFA4B1B1FCB1C7B80B1443C5147BCE0000520792ED861D768686A2C383EEF0B03D3C688F0CC54646A223217E4D37CC2F95854687EDD0843DD24FF1B1E1A1191D8CA1516EE147A29CB8846AC37D63BE829753D31EBC9C0A5293F9F0722A579DDCFF237FA25CBF4B2A87E9C48647DCF130A97AAA640DD78D5986E57085CB0EB24585A63F48AD1ACE0DC755A6E1F20B68B23900406A30FCAF4F7ABEDA65C7DF31F5C95BA4FA261623D5AE54AECC1229CD811985F658ACD43B4A4D503AFEF7783A26839753E713A82E0000E0C2D10ADCAB47E31F57F215E4FA2ACB820B1658650B7CE5E559250B7C14A6A1BCDCF4C5553B214FC0A1DA014829863FF847D757BBC24A95D72FAEBC7C03E9720A9735AC2A5DB1B268C5B2AAD52B96AD5B57BA6279E9F2E595CB9796536086878A154BCB2FBA389F52A6BF0EECDD3EC03C0516F7B4071677909ACC8FEE200947FA1D744DEED0C374B5BBAACBC67452E43A4C3F3CF2B280EA5E92EA5AAD278CF4008054E16853D311A5962A55FBD0C366C07FE2C927C763B1FAC64794CBEE6D84EEEA858274F1BA7CEDCFEC15EC724FB1C65F9EFBCBD1D051F59FDE2DC51B6B60719F4FC0D403000017087716AEB8334812ECAEEB63D54EB52957A9B66972471FEC0DA3553BC54AC0553EEE504EAA5D5ED633D203005205BF5239D2AA760CF685311C5B9C61E40D72D2E9AECBDDECF0C7AEF8E2E53EED6705E9FB54A01D7AF714301F81700700800B843BDC60F54D153CD7E1549372FFD9122FFD123214A9EB59BD20C7F072122FA63A890700A40A746D127201F3054ED7724C5FBF7CD152839C7EB8E722EE355F5FFB330D7741E90519DAE1E44930CF80700700800B44EA7236CC992EA97111E5D2BD4CBC36A71BAC29A639F193E1B1EEF7CD212540116C8CE71A99570400A40874919224E7CB529AD6DA13262E95D9C22E5733C5B0AC9F693F19C6A1FB06B5F5B587FBC9A480F909843B0060469862879A5CD7A478BD73FEC99325599D4B886FA7FCABC3F1AD504847C66711B4A48F823CF04CDC87014821C23258FAF2E4663737B313E82BF7D4D0CC22EFAF937617871D30EF418501009811A6787E4EAEF94E95F429478A270F0030A3142F5BB248BF81EA46E93E66BBFE397B717E12F2F5270020DC0100000000E214DE725BC9C285EC2DA3B5B2114BBC170AC09C03E10E000000007012C3651F7753BBB9C9AB293077831401C21D00000000C0839D528C28C923D7310CD736BD8FA4E12572901240B803000000007870578F4E405ED2218D64454D3B4A91D04B20354041040000000048E028D3B095B22D56EF3ED7926F2FC1E20E52020877000000008004A6A16C92473EC75186E3BAAE6876E8259012A02002006684F9D08F3B00203391EE6CA507469649A9704F906F3E0000E10E009819D08F3B00203348857B02FA71071A0877000000000000D2000877000000000000D2000877000000000000D2000877000000000000D2000877000000000000D2000877000000000000D200087700C08C807EDC010099412ADC13D08F3BD040B803006604F4E30E00C80C52E19E807EDC8106C21D0000000000803400C21D0000000000803400C21D0000000000803400C21D0000000000803400C21D0000000000803400C21D0000000000803400C21D0030235C583FEEDFD0E119CD7615F78AA6031C13E57847823CC3F1E229F00DD0027A355E9202B21683DE9201009349857B02FA71071A087700C08C709EFDB8C73B27967B91FB0D15246F9307C7351C1A7394E1E7F54D83B7C3159BA937E14ED9FD19305DD9A96B98DE56E35A7F72F200002015EE09E8C71D6820DC010073893623B9E7674C9245E99F743ADDBB684C538E3736C472CE700C8B78D7F622CE04EB7C11EB34A03E040000901640B80300E60C52CCAED8C65D65B37AFE46F54E1ADDA0A562ACD5F98F620C8E211DEF9A963699BBA66B881C372C9E3C138EB2A5A540CB4B0340C7E27E08000020B541450500986348379B62F4661D2F316743543B2D4352DB8DB16257235FFC2BD47CF414C52FCB4C92E367C05424F249F8EBDD7EC34E010000801401C21D00306790B6D6AE32A6F8A7EBE11C3ADA706334979D5C0CD352863334D8BB7DD7D007EFF31AECA2CE4EA089B5F935D5B3C03344D8F3BF8CCFBA28000000903240B80300E69298525116D986D6CEE716D0AEE123B96E3A86EBDA13478E34FFE10F8BEEFE9EE537DB9B9A681BA66B196ED470656BD218D06B9D8EE18A678EB73B270CE10E0000201D80700700CC2524B4234AD99335F659F5B6C654A4DC959555595E71D1455D6FBD6E4F84CB6EF90E7711C3BDCDF85D8342710FF8B362F14E0C658F8F77353D9E5F5C52F1C31FEA1900000040CA02E10E009811CEAF1F77A778CDDA0ACB1C7AFBEDE8E0106B6DEEF32CDE3DCC9971944B1A3D66E4E4155C734DC017CC5DB52667D972D9A6D7B1A3B6B64FDEE354A42B19DA9B118D76D37841997F4179A20F9A73AD085209F186D29DF1F3148F74E1A1901EE27C53A102E05CA4C23D01FDB8030D843B006046E05747273149444D0E9B390D0D79B7DE71A4A3A3FFC3F79D70882AA7298A7F32DE26750FEE84A3CA1FF945EEF5D77A93D3C0E1F75B2313A3FB0FF895F2D18DD030CC78F79167DF3F482D4C2E645C8BD1AF4DCDB6B897148FE8FC7AEE57BAE7225A0CDA1D5C20A9704F403FEE4003E10E00985B1CC3B28A946AEBEA1E78F72F1273F6FB129BC94DB1B3FA58DFB34F8C4FD4DB34912A707CE79ED6EDDB624A59C11CD279C6D9BB8F04A9099F79DD1FBFAB2C3AA546944EA3E390409781F43A45F287B97409417D0700487B70230300CC1936CB2A33ABA2B8FACE3B2EFEDEF7A3C7BB4E34359D4387D32C433926893416DEACDF795983B4F73431AC13CF3ED7B76DEB8ABBEF5A79DBCD39975C463A4F3609D209E9AD9F8A414CBEA4ABA29DCE91A6E681B707956B52D1E2C2E2BDECC02504DE3200800CC0686C6CF482203D79E18517464646BC890794DAE0054FD2A1D4BFAB1B37DCD87079831703C0CCB367CF9E2D5BB650E0EA4DEAAA4DAC9EB4203FFA1F6AD455F5F13B8FEBDA86C142DC309CC3CFBDD03B31B648CF380B933D1E68837AB3D3D5DCB4BCA554CEC56B0B36B19B8D6C84B49D4FCF3DDED414556A11EE8D690095057EEC12336C9FB2C29D9143AF1FCC51B94B1AEBBD593296F3AB2701382F065E7FA3AFB363C9AF7F6D66659D78FC8971C74EDCB2E68A779E7ABB35DAA6FED39B3C852795DAED05D7AF5FBF71E3466F026422B8910100E612766388F700B360D9F26AB18E9E6388C8D89681E435AD48E3C90B9CCF401BC95BB3B6E0AA6B1356589754BB4E044827B80AA3E243AADD8D3801C32A53A5036AFCA3AFBEDAF2CF3D1F7DBC7BCBC75F1E6D694E2C090000E90E2CEE690F2CEE2035395F8B3BC593F2329CF8EB83E71658DE0289AD318E7C4AF5C2E01752F9534ED2538D17078BFBF9A05F959BDB9E2EA8DD65D089A3C6D898DBF34A5773B8395BE5EE515BC7B9757692871F7ACCE79FE3A482F402167790B2C0080100987358B5938266F5FC0D666F5A80171409E6907624F5685F886A6743BB6C88543BFDC80BAEDFB06BC068BD2E39CFE8C8B9828A0DBFF6E03856B6617D27B05BEDFE427D965F1AFB9FFF533DF488FADFFEA7CACFE1C59E7A7AF3D804B516010020ED81700700CC08DCA1471C0AEAFE3D0892D99E4876593DB36C36F846C45DBAD0044F9F93937DBF9806232B4E9BF81EE34832E89F2DB5348F353D21099611F0D0766B1A9B828E9C3B62941AD7320DCBF1E57BAF2858011E1717F0990B66491405B979E7B945652472414940B7AC32FA60CF8A77AD3A7CF83ACCE3E96785CE491AE8D644DBBAE0077A49436E95865F4F48CABC209887C05526ED81AB0C484D26BBCA5CB9C913E454DD7437A9BE31955D5CA262B621DA3D75B00DC39715ECE9E921EDB7E4B14649B42329878D2345D10286CE517F7FEFCB2FFF91C29535EABE9F730CCD7AEE39D5D7CB0BDCFF8BFBF2B3F32FDCA52AB51199CEDDD8F39F6E5AB1723DE9FD4590A0F7E6CC03B854248E9726A67FDCFD6FBC31D0D151FFE02FCD6056CF138F0FDB7C43386BFEE9523873D07935D5DB4FBDD31E6D85AB0CB0366C385DE8817462D7AE5D9148C49BB858A9855EF024A4EA3F51F50BEBCBAACBBC1800669EEEEEEEB6B6360A2CAA558B6A58FF52AD4743DB1ED53CA18A431376241C8B44526A702949E3E341A5F28A0B72D7AEA5241BEC8A31FD6A1FCC0EFC7644D4541605C2A1E89EBD2C5E0AF2D5BA8B48B67169DBB94D4D8478C175175D1C0C062F40C0A50554465D6E75EAA2EA72A1E53852F34642AFD338E3B53B9F73FE8F51F3C534E8F8C51B4EA2F402E74FF8F0FEF0D0484E41B1E50B0CEFDE4331050B16D8BD7D765FBFD33FEBC3606F5E38B4F7EBAF875544DD2EE9A3039A7C58DBE986EB05ABAAAA6A6A6ABC099089C0E29EF6C0E20E52932916770DD5354F34A9D131B3F1B147A5076E8A4821D866C9AFC95AF4EB3A86E93A34929E2261714F4DB81F77C3354D65F40DF4BFF2CA2B14555DA5EEB9CF1336CF3FAB7AFB78B9FBEFBF372FBF90CE274F641C94036C60371DD7F119ACE00D9B0EDE6493BB56EAF3CBDCEEDAAEAB3DEF62543E2EE0C0A93C0DFCFDC3D68307E29E56CC1CDEAA68D7F94ABDAF54880EEBFF88474D3E2C58DCE71310EE690F843B484DCEE82A43F5CD0B2FAAAEE37A0280E45353A57E1617EECF3EABFA45B8CF4F6EB9E59665CB9648701EB53C3FFCF0C37DFBF6791319864FA9FF3855B26B20DCE71330230100661C7E58AD07435D7783CACBF3E201483A4EDCB59BC7A74B9CF944381C4DD4F2FAA5D54C87BF7F3C3C3CEC4D651874267F29455ADF4BC17C0516F7B4071677909A4CE9C79D10DDCEE3F6A3EA95575596A19C0BEA116606E11E24F84B9BE2323CC9B0816A323531A444C959A380E92852AA1BAF52D76E52ECE6ADD4871FA86DDB5420C0AF58F868B14C3D8F72B0FC2B59E133D5C4044F5E73F50D17AD5B2573E609FC92EE6B7F7AB7ABBB253BA81C879DFEE99C4FB7CDC22FE4B00B961766CF39876F56FC0DDE39223FA2FABEA7D4CDDEA477478A9F7758DCE71510EE690F843B484DA67E8089BBBB90FA46AA52F1263F59EFA40AA47BB45E97744EFA05290DCB18696E916C65994553F1D295BA852D89C8011234D225F67897FAFD0B6AE315D7AD5FBF461FF9BC7173775EFCEDDF87C60EFF6FFF5394B7BEDB78B3A6832E33F195E99798B3EC93C4FC47A15234C8E4D4A440B8CF27E02A03009811B8CE9B049BBEA4BED1958E1E528E84953D9EB8A427920E5F0FF2CFC47FE7113A07122F8AEAC92905665A7071A27F934F1F4BD849329D0399ADDAE5901307C899496D186FC237E9C829BF33F3DDDCC950FBC436F930756B8DCB959E315D7496C657A6DF0BDC4E327068DFFA22994CE69F4C706620DC01003302EBA738BAD2F12CEE32A2AA68BACFAF3300AA6A751ED0982BE3F90A1DBAE99E743CE09C90671DE082F1C4BAE42A5D68B664A6F400C9B9CC73E787B9DD302C43F1878AF455C63FE90F35424E9E3DEFC020DFE62F7095497BE02A035293A9AE32F1EAE6EBFD6AFF1E15C8E6F07CAB7AA802D68A8A72831A367E5B45C5483CDF600B715C623ADA322A830F46C40B43E4297B79C965E60FA89E5ED5D9A1365D75FDC597AC962518ED2D93D13E33DC2BE64B7F7C6FA0EFC89A8BC4315D4A54BA5F627410798EDA4A874127F37A39DD534E205C65E61310EE690F843B484D4E11EE57714DC3B63F43BDF2B26A6FD78B00006690EBAEBD69CDDAE55AB8CE03D5EEF1C69B6F771CE34FBF65207EE90E523359BE43B8CF27E6A1AD070030EBC49D4D696CC3AA0AC0EC60C4B4AB0C0745AF67BC6A57DCFB8B77C81948E2E63959B58379062CEE690F2CEE203599F20126AA68C4EEA79E7A520D0EAB1FFDE827763466BB315976BEE028DB5F94DDFF4677CB509B3889982432C439797E55C2243FA829175056ED9225855716D8236131065B9384099826AECF351CD3B05D9BAE34CB75A38E322A16946765795FFF9C0FB676820E932AC4FEFE7ECBF2FBD8594622D3FFB8F3F3F3FFF2E2DF06548FFA4F2FE61460719F4F40B8A73D10EE20359922DC19B1126DFEFFA9B109ABB1F1E1F9F8C44F72A0FF8FFD43BD0315B515A665D94ED435FCA62730E60D4123D66B9F18385EBDB63AEBDA2C8B4B021EFF5E382EA975C3921728489BEB9CD425CA9CE22193F1F29D8DED921B7C900E1D2D5D71E97FC072DF78EB9937DBC3C73CE1CEC7C9911E10EEF30908F7B407C21DA42667ECC79DAA9BA626353E66343EFA981BFFC2E5FC81EB5F579DF843CFC050FFCAC756B0A2E2CA9734D6BC93AD91DEE8A13F1EAC595D53787DA164C07CCC8424C28A5CAEA7F80F17AC29AA5D93E9DA3DAA1CBF4BC58933C4CA985B0C9DB5BF3CFB97D6702B2CEE00374A00C08CC09A74125A2AC4AB51360B66B076381B7CC8FCCFD650AA89BD29DC87390350197D2B588B5361D2AA9DA7652457DD14999ED1AA9DF0CBBDC5348CCC51ED049DB5297754306FC1BD1200302364805F290000A408BAA90F00843B0000000000006900843B0000000000006900843B0000000000006900843B0000000000006900843B0000000000006900843B0000000000006900843B00604640AFC367C17194E153F2651C2F663E62BACA50966DDA7A92BFFD890203C0D9C9F40EF8C1F902E10E009811D08FFBE98834350DD78971507F949E4804E60DAEB24D3A6CC7722CFE30BD72B815830203C0D9413FEE4003E10E0000B304A953362D9B96A57C8663F1248BD879771F76E9E01D12EAFCF57D9A345CD3553E3D0B0000C0398070070080D982642ABBC838517E2061907E95B8796771775DAA7AB8F6712C36227A8644D8130100E09B807007008059C331D8BBDBB694E9988E724D52F20EABD8F9051D361BD9291F6C6AB498D476315C1BAE320000F08D188D8D8D5E10A4272FBCF0C2C8C88837F180521BBCE0493A94FA7775E3861B1B2E6FF062009879F6ECD9B365CB160A5CBD495DB549C731FFD5A4C6C7CCC6C647BDE979058955431D7FA5A3AFAFAFEECAA5965FC5623153F9E79B9BBB9565853B43470F1CADBF6871FE3545A4DB0DF673A7160CC43B0067E69DA7DE6E8DB6A9FFF4264FE149A5767BC1F5EBD76FDCB8D19B009908847BDA03E10E521308F7D3611BBB7207DE1E3AD4D692A7FC6C6866A9EACC375F194B1931C56FE8AEB86C59CE15B9F2AA2AE5039E0003705620DC8106C23DED817007A90984FBE9B0599974FA8815E919675719D2AA8E390FBBDF11077FD3B1DCEC0501339BDA325417C55CE583BD1D80B301E10E3410EE690F843B484DF6EEDEF3CF8F21DC01002009FCE5E9778E465A21DC011E4D020066847968480600801902FDB8030D843B0000000000006900843B0000000000F8FFB7F7265E761C75BE674466DE7B6B536D2A9556CBB2764B061BD942D860D3061B786DB65E81A669A01B1AEACD99F7CE9979F3E62F9899F3E69C39675ECF3B2D43B375639BEEA6CD6EA0D9B1C1605BDE24D9DA6C6B2D2DB548B52FF7E632BFEF2FF2DE2A4955A5D25677D1F7A3ABACC8C82D2233F217DFF8656424A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC0921D705CBB1CB0821E41A612D47D82580C29D10725DE038EE841072ADE038EEC441E14E0821841042481540E14E0821841042481540E14E0821841042481540E14E0821841042481540E14E0821841042481540E14E0821841042481540E14E08B92E701C77426E588A4317C626917F1A2A1A041785694A9CFEBD9E5C3C96A2C62051BA204D83A4EAC2F52A068EE34E1C14EE8490EB02C77127E40625119599068D957F8811B5A19A38F4E42F66253A8E20956589E8E6EBF813317E81EA2DC678560269C220E5BD243A6FBD4A82E3B81307853B21841042AE1D56F4B253995E2A36A13A2351C9091AF41EB4BAAC1223ACBA5EE6AFE34F24FA74D52BE1928C4FAC2443D284D6045A1B560384543052A60921841042AE35F0AD47F8EB8926F68D8DAD4C21EC0591ECD68794C7CCF5C679DC9D7C4FC3FAC393416BD163469F0C24D2A8286A7A422A130A77420821845C4BBCA28FDB1A3F95CB93363A158F7717264E4DE6BBC7F2DD13E3A726274FE5F3A7C70BDDF985F985270B329543E3777272B27B22E99386450825944422DB45B4F3E51C52E1D8AEAEAE3448AA93471E79647878389DF98431DBD3E014DDC6FC3773FFF6FB37DFB5398D21E4FAB377EFDEA79E7A4A026FBFC7DC7D8F8B037FBFD38C8D7A5D5D9F4BE709213547924470AE2726B68973620F1D3C77EC17C7457578C68B8C674D949824307E6822D75DE67AE2C1F55FC49AC4F5E4894DD86416ADFDF43A5B2709882555BE5BAB229DEE3FFCD213470BC7CCDFA5B3E7F10FC6EC4983DBB66DDBB163473A436A110AF7AA87C29D542614EE84DCC0C43134BA93C0780375F8C581D79E397CF3EA9B72AB1BC289D813F12C7A5AF5FDF40EE8D7036B93D8A6DDEDADB561127B9EC9D5D7F53C7966C80C6CFA9BAD7EC6473F782876499550899D1128DC89835D650821841072CD28BAAD550BEB6C022F772663B28DB73536BC29DB7257D3A2B7E2D7BCA3A509D345D7F5B7E8ADCD2DDB9B652ABFA6ED8B5ADFDAD27C574B766BA66E45BD67B2261235AF09865BDED3516E08A95C5840C965E3BC23A98FC44DCEF797E862B7C4792F74402E8D770F288B8B4A4B67055BE9DAC5B04C741F88C486C585F343D79EDA64AE8D3555186C181D1F5D784ED2BC602DBC6E257F2526DD89C6E81B5A6E37F83F37D84F7ACAB0B2DBF9CCA43BD323BBF5D39899985A34152A0EFF00D207C8A1FCD180465E21EC2A3A378909F52FC6AAAB614A05B3589CA697375225C825939F9AA534A688D83A3577E7A13258FB8D4B483B8E0BD6AA0D849C0F342A951F6EE9C283916470F048DF55D57C49DA242B686ECC60FA4AD94CFFA6730B579EDD3BB58450B89379018B0B85011305F31127328518554BE20C4AC9AE6146E375E85EC1C3E85BBA104F4E131D14CC78B1AEA41E0E2C9A1159C35AB1A1F8695866D2B58B7FE46FBA828B9805A97330E897A023086065373B33091EAB8636F18C8F24CBFAA9F36836B0BEE405D9446EE5E4C80692FD389628241D632920EFC564CC8A9E1CD909CE9DA4D5A2E7A55B7231EE9CA375E18E8C03B92533E3AEC205E70A5BA5F1298126E22A3D4F97C8E60D8AEA583DDB3609302F170C25BC36918C7A92652DF6284EB87BA56CAB392155040C91893D5404C5022C85565A9EB1D83A01716A8B4AB580B3756E81531A3A106465E1ECA1A459932EFF70274A21554D7F219ACD586E60CDAEDCB8B897A53C2F58B6A6CE2DB9B1B9AA8A99DC3888EA842A55CB25B318DB4B7062B448FA60547E10C9250189F56543B18AA8BC61F62085B13B319BCE19535C7526A46E104D8C9FCC88E512252D3B921FBA2CA27F248C195680F99C5500E90A2ED1315E8BD2A83929C84A4182BD6345DDB96E361BA93EC78ADA9512E70A824C0575318BFAB6561A3323BAA5D69250FE6E73CF57B137237ACE714EB065B1629C2B9DB24F154FB26AF11100D2A3C7425837D687C55249D7AEA02C23B80B1439EB72FAE54ACCA8126A039426F47346B1171D27C50A0FB0BCF3EC06A9066025702163355FD0AC62250267729C9E84C583A815ABA1115A3B20A0F656B652BBAFAB550E561A9192BE40122E59B1B0F10567042FA0A8ED3D77AFC284AA370A8D13CD3E210B065F4EAD7A16EAE5547598A9322F5A35E80F98EFA91858376797237867C4163A0D3BB5896E8E48C817318530EE90A7E9F28B9055F05291AC2F6BC95EB0A3580F2221DD508DA6D85C99994B0E884A2D8E70E0E613EB5BB40AE0F5BC18D9AB285CAD798A665C133D2B588A13E2F282F390AE1D49DB4254B024301689ACB1A8F466D9959C371F47C4CAB223B41A90D5994F11B25E3CE17AE631540244FC9CE8997089405D241B9F9F3339D0B5195D812FA7CE407AAEA71781786CDF44F7AF4E0626D0184CB4957B15A7BE8290A22B79C6C8210D7E76C5DFAEBAA0B491AA402F9A33BCCE68C0F0AA684FCBABBBA6CEFE176D36EC61B19C233CF4FCE0B1674FAE7F686DDDEA7AC45500BDDFEDEB3FD9B7F1531BBD7A584349F705D6B354A3A561C9A9B45CD26C0259367D9DEB0A5F4E258E19D40021338011BD62D185B04F62B1F093C203A30CE78BB361122E3A1FA03EA574A9074397A6A236568B5FB479456357DCFC62AC857A56FF4E880F79C04BA99A4603328D45ACDA5076A82AD41D6506121FFD7CB4CD20921DFBD563CFACDA0559AA59485DD1EED13E8E380BF026E26E4A1B1230EBB2A11EC82BBAD8757792E6C8B3AE73F30CC0D1ADE743D2A73E7C77461179312E1F2E559A21D1FA9750ED728A640DB701E6DC45D4A93415108FFFA50B34EBF924D786C4E48F4443CF0DF9469A917249E4C46BB1AE1DE4961750EAC228EAFF4E6F34C2425585A0487A3036B0F9DAFB496CBB96D3D42AA61603562595F56267C5F4C9D5D6022DD63099DDDE960B317D91C96B30B41895324DAD646A9A5DD58A05CE75595D9F2AC0F4C630F0C866843F842C20F4B8573D0BE67157612A164BEB5D883CE70C4E81994BB5A0733FC0F91B5B1DA017A21F4BB06DE48986C64CBA375D0F7F67064E90583617E328CA46F6ED0E926E2407857285A344E2F019EDD97624965537D7B4A1D69159686BF50CCD0C8C310E27C7928DBD28097D0B81EB965E08D6D17CC5A2BD71AEA491E1DEC072C7C6DED0671E71A5533503725AA7A72ADDED85074D3382E5BAB23B4431722E9282B1195D3DDDAD264C0F227BC049D11116F4FAA61F39BC52E871BF189C793DE37A47E0F9CFC1870FE54DE1B6AE2D88AD39D27B13252D1E3B307EF417DD2B6E5FDE724F9D31195D4EAA07354D7229F5823A0B9C560AE8CE0ECBE31EA5EA9AF24FA344D74A518F8D17247664D7E8915D472ACDE3DE7BB267D3A737FB5915E5E7194609C8EC94ED4D8DADC68B68476D8313801A628E2AEC1A428F3B714C154A42E6C0D9273818D43A8B22D79948C290EC62E0A047C48CA976448498B250443AD6777E77DD89F10B622061D4E53F0C2016EBC299114309FDEFEA001C05325AF789798946A4984DAC16CEB1235D24FB41CAD08EC03F597FD6F2AFBB95FDEBDAB235F21FA4226446741518707818659348FBC723AD5ABD614B51ED72DE3417E9461793D842A96ED05999CC90483D1F38DF11D6741549DA2641FCEC24A2DAA59675E7502F1F12865CE22D5889971AD8A5DFD828ED2D43AE1D90057AE91096D292240DA6A9C188903D0F774FD50028CCB88304AF6153E3864FAD3BF372F799C7077421A92A601FD28787304A4EC463568AB4983FD894E25218281473314708F8620C65ADC89F744B2B081DE80615191E27B8F4AB1F046115F21AC60F661696169653F32F61B462503D146F694216047ADCAB9E85F1B88BA912A3D6F7BD33FD2707449CCAAC0FA77241FE8AD1825E2F1A2F593A6EC696AD5CDEF1C10ED5935E64631F7E68AC77FCE1E32366243059D9366332054C7DB19D735ABE2434D1B2954B5B3FD80AAD5374F3A02A80068F077E3070FAF89900BD44E407DB7A31D353A8F54CBE75F5D2CE8716BB988B19DF3F7EEC974765871E3EEF97B1A690989C3E4E9D9582891699C6555D2BF188409219A17F8E2438B2A16F82D1FD63DDBFECF691972448FB0FCC80A633232727ABDD8245F1CBB9BAF94F6F0A96A41E4A5C08178A9342A170FA2BBDC3E66CCED4C995903A24D0DC4DAD7311B2343076CD9FAEF69764867E33D4BDE7948FB321BB95531A89825CFAB19B33ADBAA25C32593047862F053DEEB3E02EBE94013C8FEADE7972C24CAEEB5A576C005FC519AF5070C3A28C41DB45AF7EE1505B5BFB8A8F2E4D1792AA004D7ABC7EE39E07C21B9D3727BF7C7AD40C5993CD9ADCB81916C3123BBF85DB20352C62D0A48E808B417E12B3EE03B76457E5A4304C15F5390CD675E6DC4FCE1E7EFD7093592461EDDF2E265792E2F22848E2516FA94FDD936CE6CD4464A225CB3B167FA823920D90BDF439EA02408F3B71E82D46C8A5805D12897C72486477737651936914B1B8C820D0601A1A4DA3FC5C40A6129F5B9A81ADD75EE910FA62CFD584372C6F6D32F58B4C4383DF9CB3D966D35C6765DB3AD96AF69F1CA529582E6515BE1067FDE5E7FA9D4B646699D5E3B688EEBC68DBF4D7689B9A4C33C25E7D8B690C24D0D9E01E86CE48B0C86F344D22C4EB6DE322D394C3343B7D8717FCEA4D53B369C9DD24121F7A17DAD9578F3EFE637045AF256E3275F5A659F25277D1E6A59FA6D36F364DF5326BEB25CDF5D93A939BBA4F25E3A93BD6B3D6F7EA1A720DA639671A9A4DA35E94466C38FBAFD164EB738D89B44112E3B7FBB249A327F18B16655B65CFA74C6F38994F95A51CD33D13B952D47D452E464B32FE8B5080D0913B4366E1A4AC41D52EB8CE159A419391063FFCAFA4BA80AA75960D4557AE9F58E173F2CFF8BE89CF986E31950D7E93DACCA6065BD768EA1BBD7A31C812586472399369AA839D97A23E4303B57CC521C617543DA93E9ABC45A83E6C931A49C9C8A23A5B27B6BAC1E4EA6D5D6BB659DA1E67CCE97A533F6E26864F8D44B6800792F01EA9836341A851FB402E1B7ADCAB9E85EAE30E71716CE751B1753777DDE27AFB396126B6A4A880F1E81FC6451768D70B5879A9A6530D87615C0A41024DAFD2451D19B045B239E66704A3A0C8FAFA6AAC689D92AB46C782F49DD71DABE135D65977A28700BAF5A5CD9FF3EA6BCF1174F3D6E3CF91464117BBF53445DA6DC5B3E8E229C7932DE1C38EF4C1C3DCA3D9D838D11DE060B12978711079E7F5359FEEAC721702217D2A8DC6D1DCB9C301E48F3AD4D53B86D32895B20D879F1E3DFAF2F10D1FDA985B9E8D2D22E772DDCF037ADCE7C3F19DC727CDE4FAAEF5E97C6D13DBFD5FD8DFD6D6B6F4A39D690CA94EA28970DF57F7DFB4E526EBDB137BBA377D72A35FEFC67B144A5619944CC8E08B03C77FDFBDFEA1B5B99BEAE663841780FE6F0FF49CEEBEF593B79A7AD444EE1DACA20D97A9A412FFC4100E3F337CFC85C3B77E626BF7D74F45A6B0A6EBE6D4E00B97AA1BAE15F4B813C742943652034020BA87886A954B7E17677DA11D61E3E0128043C58AE0757DB5B119D611DBA7AB06CEA0CB92D4E461A2227936741414B5A430A9B2BFA2C557498D1757657B5941F72BA99B1939843B0ACC70F1E0B3AF2E3B977D86FAF0142D076C03993B2BBA73745FC6D82CAADA25C19A505FCE838863990DD183C61D5E97CC049CAF183452B6470774386545B58B289FC6F43A2F55ED02BA9B5E4AB50B385B5ACDA091A527046D273940E0B287961076EA1A011A450821D381BDF3C54AE0451ED8B950BBE0A50B615ED406C20CCABAEE0961C9C49C6FC1CA4B14E4C51416C4EA39936DF09110F91B49FA51D5245AF5200F12F64CA0A3E468EED56F8595757DB7374216061638322F6068F1C91ED1B262A5D00106BF69A891439433CA7E0CB7BABADCA3345E0C2304287AB4BB79D83E67D3E7408C3F1A02AA9AB1C94588E8443521B587266D16508BE0502AEFB1912662F6E20F5F3B7A8C231310EF974A26D0A38B654FAB001C483F920A57389A2F018CBF5401182147379801D485F227422B25467F784D62DAD7E03CE4D45D7CF6647EEE539A5E1320F512E6B5DE95FF7164114ADC60E229C59A96906B034B54D5030303236145E6C276C03E8BD5729542B10323809D911560FD6069E4DA57DE70903186E6F5630C260CFF39FE1524DE4753049943AA257B1286E18EE30863A2A14E725973E51967839085C355E1845C8A040E868289C7CD2486E385D112797A5E355CD4EE1209312D521532144F1ED5B041312220C6DBCD230A06714EBB970E042CFBC0FAB245B166881190C3E0434F5821757ECC812E75874EBCD80DF0321B5EA4721DAD01ACAE7FD2AE41333275649C28F939F90C418C652A928DC550F4D6CE3D588B665385BD0FA9AFCEF699369053573CE129DA3A987A223123EA5F97BFDA24402D05E08347CF9989613366628C6A8C5A0CB531ED0321E43C8A06464C8F1837382A8CC9E02D2609143B16A6A89149517F461AAE18C4EAC1CEC1BBAEA9454C26428E60002587561B1B694EA55A40C520EBBA4A476C35A8B85C915A871533991F90B071FBCAB68EA58BA14AB5C7798271CD41493DAA6A9448F9390F3762240A265BFB72405EA778E98C6C3CA7E5937DC9961091A2893D3DA4ACEF3E2C2A3B90848951C50A6EF5598051869D85D295BDC4D64BB40FF72C44BE0E8F88F5D5B69FDF59E562DCE2928CC6D9D21915F138A02E08D0F74663E7406A05598ED387BD20D9D1EC1F6C3A5FA6CBCA7A7A66DF7F825DC5DA9A925468174E1C073D67EA1637DD64569AC69C6C9EEE148D2542AE21289FA4AA510356B40CB127D62A3451A0036EA5B66DBADF5D4D091C0A323DDFD153094846D0A0C0135D546A89765094C6073C177082A8E1D7DA477E30AF3613627D593D4AAB24584BDA49B2A0D08C927901E59D788B3FD8B1E48F3AD5992E45674A33BA5049B64AC0396B751E3D64E0BE48BDF01A03ED28064FE7E0F2C0DF39803D9575E4E7C43EA4BA6EA456136390BB43CD8E6C06098E63CAFF18AD86745F33E3E3E900B2907E80098E7FC9945B3803FABEA97EBAC8254CFE4353A356C001914C9C16F903A7BBDB6646E0B00F35ADB211EA033993BEEBB4533CBDD3B920526690CED90F80FE3FD8B7A651A610EFBA99891BD6D7AFF8FCCA4CB3F69851E63A4184901B12310AF8A90D89BC42620ABE085F4FAC96C6635AF4BB43D24E19A8F3BD0C95811A7531A2EA6097F4F9525D690D25A0BE80E1B7783C1BFBB2965443F9F4731E16A3868981D43C5147910585058ECC0BB16145718CF72CD5217161E129D9650940F23909AB3D6460BE8BDD08F591AA2E2C9AF429D33E13B2D4EA675305D94A7DDF38B4C6634614B6CA71AC301BB2994870F5A9B894CB267356239A7455C5AED1A1499F8BA0F86DD77435ADB370203D152AC175A778A7768E0363257D7A0BAF158E299B3BFD3C637A2F889419247776B0374C5CCD842EF8581BFFE1E6D7746A371D41CEE89C678810728330E5624F71C6DFF3A42ED02F318BF54B2B08B119301BAA6E61369D9DC7FACEF25714A2DBD5DEE2452C983B9981B59518CDAFAB2FE06BF745BAC32CC7622725A75299C19BA2AA5F7744C802E26E3F422E09C4B1EA64E8CE4B1AE0A915748C145593EAC9851F3B8D28C9C2E2DF99394F3DAA952D06855201BE5449C666F2BFB4F57CD0A4A7214C2E9150659AD84DD7D789266F1E9B2BEEE4685B454FF57CB79B07D89BBB2060EA6CC885856B09CE78994B2BE9ABAC92B4B4904B720D2F6F75C0825175C06C97D0CB174F3D3974CF15E10A4923C0348B578CBE94EFA30C689D26E9F74B892F06A659488D88B53690CA2F31E80894AE2613FD7BDDC19913B33CED2A0812E9CE6829A0BCF0C20B3B8B747777A7B1A48638BF1C10321BEA293FF7E4B9BE5FF6EB7CAC9E6F522348ED337E72E2E4BF9CCA8F8625B3709555125C57E4D2DC7037120B0621978745C7C57434B292C190FBC8DD4A323D5FCAAD5CB9D205DC375E66EC6649AA170A77323FE0FAB53DAFF6F61EE8751197E5BB26954E62464F8C759F3B1E0E175F2CE3F5258490CAC05A9BBEF47B8165164D1EEBAFC8B66DDB3EA048388EF13A997B3E40F95E3350B893792177BCDCFA75F8EC7F4E23B4E4D00ED40C36CEC49945A621D0176163BCD240C835679ABE2084CC0FB5C6F185DE3267A3254E6AE3694B5C573427D34BAA5D280548B543E14EE685DCF16A08A28492AE16C17B635E12A1FBA67A68C4C8174D3F2184903202C58D5709D4E52E56D9FD66D1E13A2C7DBA8C62BD26A17027F343C718D15794A619021D2C85D4009E71E3D8F8186681469F5C2F58E310724578366AD50AF77F31E63FEBEF3FE94F02FFAB317B749D94A9BB8CCE979A8466945C1601EC07CC019AFE340AB58334CC30EA59FAA5280A764208A914D42C8FC62332BD395973B3B9F926B37A95597EB359B9D2AC5A6F3660B131CD4D8B327E76C992256EF602585FD70C14EE647E40AD4721BE8987097CB1F0C117C7FC26554F24A23DAFDF5251FBCE41830821A432D087DB85497C42FBDD9FFA83FB3EF18E777CE2BE777EFC817B3E71FFBBFEEA5D0FFECD03818E1EFCF14FFCC5673EFB37B7AC5D8B4D2E824F506B060A77326FACEF9B44849D04507268046A09EB8B5E0F4C2632AE7FE435183488C375CF8F1BEE4662C120E43281546B6E6E96E957BEF2B5AF7FFDD16F7CFD91471FFDBA4CBFFE8FFFB4F3E19D61E46E2AFD068886480D43E14E2E0FF66AAF69AEE5E5E570DDF3E38693B12C18845C016E50F677BDEB5DF7DD77DFBDF7DEFBE0830F6EDEBC596256AF5EFD9EF7BCE7831FFCA0AE456A1F0A7742082184908A268AF06AD9A64D9BB66EDD7ADB6DB7AD572466CD9A35EBD6AD731F5D6247F61B010A774208210B039FD81172B538753EA3466747F61B010A7742082184908AA6A4D4459D4BD84D5D8C6346294F6A0F0A774208210B036B1C42AE10E74D77EADC852FF0AF5F2CE5494D42334A0821841052A138393E5DB24F677ACCC54B49ED41E14E0821841052A13857FA6CA29C5EF61B0D0A77322F2213BB61EBAC7BBD2C116381BFA466F063B1067271FDF4EB4B577D7D395CF7C5C829C12F49475D16E4A427A59197119BDE5F35F21E67299FC88F66DD144484A451A44A482F98987DB98CC5D21A4BB5A033B2D44FD24FB661DC93D2E7DBE40F3628B8B00E035A59A5BAF4B98AA9046B0A91F0F4EBE09A215933F12432761F0E975CBA538115D24DAF3774A5931214EE645EE0BB4B305856CCB6DAEE38C6B7536B425B105C5953F0A5BAF5139B565452235FE5F5E570DD1723ED5E392B5207AB7695132EB3BE35A2E3216BF1CFDD6838751EE6AA1D9422142FC99E97D8D84B7C93F3227E71B9CA805A15098B8FFBA0798F6B1A051E1AF98917E3C36DA195328B78BDB453BA02D7DC64B044B6C475F72A4A801690A34012E9D2E4EC9DE4426E4F974E7C1D3CB5829287443326F1D84C4F85AEC66A902C2C5337182173E024446826F26642E5BB54C074A9D61449529834A35AF9AA5990CBECEA29720DD1539AE0B1778C47DF893766C6C74D01F122059CA6C199C7DF5A00925DDAF9A1644F949D1725C36664D29F4C9792AA2186844D25AA342923E3C7DAC497A2EA47462E6816A2BCA8615180E587E2EC210EDEEA30F6F2325F51FD3A3C135951EFB82B43342E24C189E642AA383C8144F94D2C522C77AB94E1D84A1B1B05D96D2EE0B103CD24595858E2C8BCC0C3416B16DFBE6CC5969562BFE03883F060F9A911A4AE6A5EDD7CD3D2357EB354C0A8A630511949AE2536543DE3C36B2782C09AE56FEFECF017F7FF6B9F198B2102A00BE0DB53C5906E54C54856A1DA025890DEE8F4632757D42D6FBFAB2D5D4AAA0631F562F153CDED5A9810F236D2DE2622D4C32945AE455C4AAF94615D08D56B9200C5A0D2483292B518B79BE7895817906229B16848633974BC243E92BB3544134472EEA335EA7E584F660959506C5757571A24D5C9238F3CE2BE840C3E61CCF6343845B731FFCDDCBFFDFECD77E1F3C85781E8090FAE1438DC3DA72A7442AA1FF523C9C5755300F928D7F7CADB667BF7EE7DEAA9A724F0F67BCCDDF7B838F0F73BCDD8A8D7D5F5B974FE0643CF6A3A55C29EDF9CDBBFE7C096759BEA3A1AA209D1EBA16732DA81B80688FDC48B3D3FC8790307CF1D3E7BF88E07B735AE6FA0DDA83A52A36F512E45B22713C92B5FDDDBECB7FA99A07FE2F4AA5BD7E49A7261BE50F0A28C08F43889BC38487C69858A7A9742E0377813CF4FF68DF7AC7BFFBADC4D75E94ECBCDD9EFF79E3C7166D5D65541BD1F869239B4AB03E347782026410F4F8A4C2168CC0EBE3CDA33727AF5E69B7AF7F7654DE696AEB5685A1BB8E1A7DFCCD7957FFCC77F1C1B1B9BAED98E1F3FFE831FFCE0BEFBEEDBBA756B1A55648E45A4DAA170AF7A164EB843D58562CBD446393D8787A01A20558FBA91227848B5DB868BBA9A0A89C27D06B46924370EBAFE7ADA5BC6D8D1E747DE78EE88CCCBF9C622BDA7E472A86858104570DD809F166248FE4AEEC28CC9AEFD0FB764D7C0CD99AE41AA05B506E8E425C5520293C9E1AF1C193243F526D0F75545E78AA4F70283B7642446BBCD481996A528EB0513C7A65067EAD67D705D666576A1B4EE2518F859FFA14387EB4DBD34465C82D3328B6A4E7204F12EF7A3DE9B7263C2273F69263A339D2BFF6605DE53C1CDAA77ED8294670A77E2A070AF7A164CB8C38489DD72FE0535BA15627CC9B5412B20A9ABD207E1527DE1098B86AE080AF7D940859FB68EE24804CDA449062291F1113AD806A28DDC730F37029CDBA44A412B45459CC3FA2668AFB3A2E548F5918A5AB99E5E22C5332A9C2B24139EE7D948CAB2C4AACA4D9218423746C995D542B450A54887D996E0ECD3C3A75F3BB9FEFD6BB3AB72451B53667ABFDBD77FF2CCCDEF5B9B690992893896BCA04F8FF5A595A9ED6655F1FAD4405A2B122D3191F51719DBECE18D56C9C502E683C29D38E8F620F305064A0CB22D88B1528DA7FD17498D20D593F69381370DA08AC2031672ED5149A3A1C413299BC906D9A5B960795D6E597D665926BB3C9B5B96CD2C0F322B7212AEEA5F6E6936B7229B5D2661C96336D319C0030B0948AA8DC4739DB73CB10D70E1F8D9F63AB9B85262EB3AFD9C96DBECF24C764526B7CCCFAD0864D65F1ED42D9732EC4BA469F0EADA02F8B0D11AAD94AAC39A283292D45CD09E0956E472CB8206C9C8B2ACBF2270F760A099AA5B2E99CA663B91B5CCCA4054BB3E96846A97AA506B4342160E0A77323FE08985D1C5AB3998929AC2B5C4F073CD311D60E12A5F26B3ACD06627BD83DC9FD2ED34EDBE721D66AA9EA90C7A9AA1E91DED48D580D730F5AD4C803158F479116438C4BC5408E9BD8EB53CF501C83AF052C30B8F6518053286DCAF2C121D8C15962F1DAF5DB3960690170D0B1A2FD9C268EE3091C8BD16657D32A6AB10B25094CA252197468CAF8D6182618FF58314A43640CF8DC4932926530ED1ABAA68D9BC23A43610D10A618BC6BCBEA18A118FD085C4F9CE512FE86AD0B34EE1222015848A5DBC3903E55F81B8245A0F633E4A9251ADA1974FBA743ADA04714E7627E221DF252A961333D3FA845C3F28DCC9BC80B14E4C680A8907BF2CA47B6AA5498D2097585F3ED6CB8AFFFA0225218424FA652531FAEA6056A78D76177112020256FE4F234E4258147D7E8701E0651A7B89276BEBA0E95338415C2E44AFC33981311F034987D668FA5048B2A3E992E4398A6D13547FB2485DEFC8B897703848B2D0B06226F342ED6F1C980CBAE7C29C796A92AFCA234B2A07E764C215069E7A9EA49EE5F52584C0E24790A9C6E22D54310BA2DA238C99982EC602994CCD7AA2D6216F65A23D4920F1A3D49E9C67554ACEFAB2204993948912D7F7C56101353A867A7726B188C60BF0D1ABB58C648268756911B29050B89379816E80B11DF8C5B9733F392BDA5D0C16BC2811CB4FCD10E78F16CE3C76CA0C4AD5854EAA521BE9D36042C80D8FC5C7F6C41C884685F759A6D6F7E1A856B1AE762251DF7949C4BA7574A9AA61A9400CDCDBCE475031B85608DA15529F496A8BCD0C50CA8BCB9A038651A6F2479B2DA1AC95CA7D4216080A2F322F7C292AD69E3AD073E2F553CE86C194F91C75A44690C6D8E8A99137868F8C8F173F478F17CCA6AA2B42C80D8BA86D15E2301498383FBA085655B14ED74A3C142DD6D66D644D598A79B12361EC45D92807FD5E49E0D922D0D684F54BDD788A397233C89A664A7BEABBA0097026F047A6D45164416181239741A3695864EAD30F55A84576F1A40690DAB5CD340789EBCB2A75938E354E08B9E15109EB7A47263A16244C44389AC4835161380EC7E370B8104B6028C26FB82091D19089870A932385683089864419FBF98931312BEAA6AF18F499413C9224934879614432928F24F1FA73D9891019E5470ABAB4100E15C2C9A9E6876A797ADCC982E26FDF7EF1077B4835B17BF7EE7C3E9FCEDC6ECCCA3438C5B031BF35B7ACBCA56345471A73F9A00BA0B567770D88BD6EDBDE6ED277752AC90493AB237FA270EEF4B9C59B97F88B02B9E0789941DD6C574C4F4FCFB163C724B0FA2673D34D2E0E3CB7CB140A76FBF6BBD279424885034F7384D63D465FC1982A49DE1EFBDAD1937B8F0FEE1E1C7E79F8CC9E3367F7F4F5EDE9EBDFD3DF2FD3DDBD7D7B7ACFEE392781DE3DBD837B46CFEC3A35D433E41B7FF1AD8B8345FABA6A9169AF7E2E34F9639367CEF60DBE3AD4FB62EFB93D7D6777F79DDB73B677B7E4A2AF4FA72E207919D83D30B27BB8674F6FCF9E3EFF806DBCBD31B2A197C8A9D013B220BCFCF2CB854261BA661B1A1A3A78F0E0CD37DFDCD9D9994615996311A976E85123F3C2D926F92F86CACD931A238658876497B0BBDC577991B5B72821A4FA81971D6FAE3BCB8081DBBD68D48C3799B67AD33062863A6F5AB66CE3F2E5B72C5DBE567E1258B67CEDB2A56B3B25B074EDB2C5EB5A566D59D5613A6012A20BFDD3E552ED425828D49BBACED5ED2BD7AF5A76CB0A24757DE78A5B562CBB65F9B25B5622B07649E7DAE5376F59D3E4370C9991CE959D720646C646121BFB8976931168E8C8C242E14E2E0F3E14AC69AEE5E595760021A426384F2A88D4C618EEC6B46E6D69BD5DB47B5DCB03AD6DEF6E6F7F6F47FB7B17E3F73E04DADED32E818EF7762C7EB0A3F99D2D753B1A30D0CCB4173DCB8E2DF89EF13AFE6049CB838BDADEDBD6FE9EC54B1E5CD2F6BEB6C5EF6B6D7F5FABC42C7EAFA4BFB5F19D8D6DB72FF68DD7FEAEB666D388F7BDF48164EAEEA0A1230B0B853B21841042E60D5E4EF5F0E10713D9248A8CE763C0188C14395D9597FA53BAF756FDCA6BCA6B2B228A7D4DBC8425812E0708E24B4C09BE48A7EFE37AD63381ACEFE9806ABA7EA41DF631BE8CC610B24050B813420821645E40ECAA02B7C68FACA87191BD13F8801174794951A8142EFDF47D56FD6A5F65A1031AFB50E73A4A0CBE0E8BB17044B7CB02CD8BE4C9E9F938F4A4A1627D8CFF2811B20906BBC432BEC44F1618163842082184CC078C52900615D1E4199315355FF2AFABD31AC2D7C9DF9208D6F1DF2B0BD1EC89893D3C3C884484BB11B5249548A804D0D0888D27F991753C697F84F20F4D11AC24B16E801D4216180A7742082184CC0B7548A3978C08578C836EA3501470BA50B1A96FBDE274FA2CC4B168721FC99564A3B74F88DCE94FFF88AEF742748889B351E0A7F21E7DFCD16D463771FB216461A07027841042C87CF08CF689C1C795A0D045BEFAD684E86402B487B8EBD22EEA56F53B82AAE3DD6CA581DEEB6869B8B4E203E13609904111EA49842FA426898444D78B608FBD48738A4F3569165DCEA8A3C882C20247082184905999AEB84B5D65206D6D12C3F51E200E7D475239EBB8A0534D25A2DD7722F8D55D5A3DF4714F53ED7AB1230659B4A1C877E4495647C6751589C6324216140A77323FC4389DF7F27CAC1E0A3E22AC1DA4CEC5C00A52214D554457757DB5872821A4EA49A5AC43EF6BEDDEED6950A5BB20A2FDBCF5A6A1F195A9E3631307D0EDE9AC303D952E6C8D171B2FFD8689CEBA808B98BE3E210B00853B991730D33057899796193C5E64F9A925BC48473A93EA18F5B093EC57757D8B8E2B420821845C1B28BCC87CD10785FAB7045DAAB5429224BE7AC87D51EDF80BCB50915D52092184901B170A77322FAC3E284C4C8C41B3148ABA5AC25AABE3139BC8442547B9E56809841042482541E14E2E0391EF6E302C0DCB7F0ABB5A42DA6622DE6113D8C985104208A94028DCC97CD1B7517D27E910867F96E5A77648BF616EBD695F15E1F5258410422A0856CC649EE0837963666CDC8C8B6AD737538B43F7929A20F2C2013314E9A06EEE4D06F686228410422A0A0A77324F3CD171ED6D6DED4D6DE97BAA1539B617B962EA5BEB56999BB2D90063CBC0328876675728420821A482A07027F3C69AA51FEDECFCC452EBBE0ECDC2535BD46FAE5FD5B52A682F7DBC1C1FFA4E835704C7712784906BC5C503EC166DEC94A1D61EADA4C6A1F622845C17388E3B21845C03448DC7897F91498D55C1D9A27E4F7BB1C6D4EE350E853B2184104248A5228ADDB3E1B4BE8BCEB16E139570EEBD24287855F6EE83B6A476A1702784104208A954A6E9F2248944AABB8F6C68D87DD77C9AA6A7AEAB75788109218410422A15D5E5819791E9C993A78F1C3972F8F0D133A77A4E9F3C2331A7CEF4F4F4F4C97FA7EF49CD43E14E0821841052C12466516383FCFDDEF7BEF7A31FFDFB8F7FFCE36F7DE7F1E75FDC2531870E1C7C5CD1F5F8F9BCDA87C29D10420821A462F1126B06478725F4B18F7DFCCFFEEC237FFAA77FFA477FF26199FEC5473FF699BFFEAC5B29FD76DE54AF19529B50B81342082184542A09FCE8B94C56822FFEFBAE577FBCFB951FEF7DE5A77BF7FCFBEEE79F78F199EFFFDEADF5AD6F7FEBEB8FFED3B113C7DC2CA9556C5757571A24D5C9238F3C323C8C8638F88431DBD3E014DDC6FC3773FFF6FB37DFB5398DB92292C48C1F1AB671A67EB3980F2F31F1558EF34D2A8AF06C61F4F5F1E6372D32B9E2E804C9653F76C516F81F4B09D9BB77EF534F3D2591F7BCDDDC7D3796EAAB55E6EF1F3663A3C1E7BB3E8BDAA8F21EEB8EEE1FCB9FC963B8061BC55EE247D6582FC6E096895FEF9B30890B2636D63771841C95A957A92427E3F919938CC7FA3934F9EF7BB847CBE36D938B1E5BEB65ACF5BD64228E6DE825593981E51A555A4C935C9AB8DEB7A131059C22B99CB858898DF1265F606CA1715373A6532C188DD83C2916ADD8335E9C4C26FBBF7268C596E571C69E7EF9C4A64F6DF6EA3D0C6FE2D92489F0AD0F01A64036C3C997F0D08B43C79E39BEFEA1B575ABEB7547E5E7EC77FB4E9FECDDFCA94DB65E0A8C03B64BA69A760F032CCA5F6B869F1D39FAFC912D9FD87AE2EBC724476BBAD6E83AF82EE10218314D8CF9C137BF73BCEF541A353BDBB66DDBB163C7F1E3C77FF0831FDC77DF7D5BB76E4D17905A81C2BDEA5920E1AE16F9F0CE43796336756D7076243527A42618F8DDB9C32FBDB1E5C35B72CBEAD3CB7A65D7174393A1E6DEBBF7D5A79EFAB544BCE31EF3B67B54DBC98C353B778A70CF7EAEEBAFF1F4B7E2CA4FFCEACE7D4366A4C5B44426147D17420DFB818924104ABE8CCD998C0444BC8818514758599066433269A20603279C4B890EE05C1E192A57D637899CB18209732627AD0A29034E019505B94C0511EC260A8CB46EB2111223A5CD879A97069809FA4DDF865B362E7B5F27D6A61D9B176162029C276DFE9809EFC057F72FDFBA4CE2BA5F3EB1F953B742B8CB423794782A7FDDB9750D277FE4B9D123BB8E549470EFFD6E5FDFC9539B3EB5D5AB136B24B7788014A389E772912299187A6EE8F8AE139BFF6AD3A97F12E1EEDDDC75B32B36EAC012AE7B4997A3FDF0CB3F3A963F62FEAE383FBDD07EC998DD6970DBB63B76ECB8FBD8B1634F3CF104857B4D42E15EF52C98C73D32E6F8CEE35237AFFDFC9AF34C33A97EA4FA19FADDE0D1974E6DFAD0FAECF2ACABB3E48A973EA33A6FA62AECBDAFEC7EEAA9DF4AF0DE7BCC5B45B86B5D23BF9D5F30E3C34157D7A7531D50611CDCF95AA3A95FF1F96585FED8F34475D8C4830351AAF243DF38D0E2B52EFBECF27020EFA375521C4779C1F15B33FD3FEAE939DEB3F63DEBE47AC5E3612272D48BCA951E21D39639F1A51323D1C8BA8FAEB75E1847BE1FA7A266E1B1196B26CDC17F7BADADAE65F9279716CE15B458C6BE68F7C08693F16B8F1F6AEF58D2F9A78BE5F26A21A729BB0450B5DAD28659886D1886AF7DF9C0F2AD2B54B8776FFAE426AF21B516EE4E77573E6D4A42E09AC117864F3C736CC3FBD7E56EAAD385E5E7CCF7FAFABB7B377E6AA35F2F8D3AB15D029CE87830931689F481C1C83323475F38B2091EF7E3621AD774DD9CC49EC52A2AF7AF773947FB20F9F72FFFE848FE2884BB3BBFCED6BAE93F18B3275DF7CEB7DCB57DC75DDDDDDDDFFFFEF729DC6B12573409B9343E2C84D47F89B5EEF9370B4FED807A27093C134A93CC8D102C75835EF1CBC5D524A8594A2524EF63FFA86BB416F730F4B04404C5EFFD5512782C10DA067867331D7ED09EF117077EBBEFB7D8A015D9F1EA8CE7C5D9B6C05F9CCDB467338B83B2FC3C3FC9368828B208D72799C5D96C875FC6F464167B894D32B94C60E24C9B0D9A73D976CFEFF02E5A6D817E41B395AB9698306890B64C9469939323F159BBD8DA16936D97A2EE7B71C1488B2CF192F2B576AA889238857497D64E8CFE6398456DE05B6934EA5D6DA554A6CFA1C40EE870E398410BCE7DF8B3A2BEC9EFC5F8F6A876A3C29C6874CD121AE4124058BF4DEA1E16E2A14D1249EB448C0356F662CD0A9A33D71D3423E454EAA993B4B8234AD2242C538D2EF1FC8BBB1E7EF86151ED12CE64321575C2C93581068BCC1F4F7B8E8ACD1003178931A03DA819D093D3B33E2A215C55D87A38C9AED03E245259E369735A409EFBBD79E2FBE65BFF66BEF9B8F9F6136674CC349A8EC1EF0FE5CFA23AAC2822F5B479B1A814E72C86EC50571C4E053C6F40FE8A6A815229177AF765AD0963D4E2BECC484CD16558163C91689117E72521B1881E699C4106A50BCB80A4075D6290063C8B40DB115E4BF45A16A9692323164C4E20CE5A19AF63F5A0454BCF94B479649A58B9D685BE57FACFEE1E1C37A367FEA5BFF79F7BCF3C76E6F43F9F3AFDCF67CE3CD67BFAD1BED38FF69C7EAC47C23D8FF6F47EABA7FFD97E74309BD605A5EC04599B3793A7BE79EAF4BF9EE9FD7A3F52FEE8E9D38F9DEC7D4CF27252C267FEB9F7D4377ACE3D7EAEEF85FE7153E8FF66CF59732E3221FAF1E3491C4ECB02DC77AEE5901A649994AA5E77875D744645AFCB796E6969696B6BABA8134EAE096534ACA49A28190A6703B4B7685C891E537245C0B8279322FF7CD136B1CCE91B66977F7D55ACBB27C8714343FA403C2E987D07CCE123E6E811F3FA3EB3CAACFFB0B9F7CC89DEC2A951B742E520120F75A4641FB6113FC98F9E0657712242A6A1F6D99DBA2BCA81C863919E890D25ADD2ECC28D59D6F448826C2C0239839E0536092539EED14D3988D353216D0994EDD4378CAB59C0A9B2A15C5A69F6A81E8A17C2635AF5680712D716472B314A02AFC32C13D53B69265A4DD3D0D868DFB9BE738303438383E706060607070686CE0D0D9D1B94D0E0E0D0D050DF991ED1F772EA3DAF825487CD0405138F8C8E0EF40FF48F9C1E1A387B6E68706870B87FE8AC4C25FDE706FB24437D3DBD0533D16E9ACF4E8EE44CAEB1A3199E09A08F0FAFBF8E2A56BB7A43C935985E622FBAEBB76DBBE3339FF9CCE73FFFF98F7DEC634B962C4963490DC13EEE55CF42F57147557878E709CFC43777AD712A21D24786A41648CCD0EF478EBF746CED1F6DAA5B26AD32218EE0A8BC3244B605A8E6F18A9754F652EB4BF19106017CF1BE0ED2E289F0B46822A45B540AF1819DAFB7D6372DFDD472ED9FAB9D73054966620E3DBCBFB9B175E95F2EC353F2B4F62C930A49CCE0AF06BAF71FDFF0D18D99B69C49447D2225E53B9BB8A4A7BF7E7A687470E3E737691761A19C122D89C3835F78BDADBDB5F3CF3BF050422D98768180E77DDF175FE968EFE8FCF3A522E22BF35D8B8A227D58810216CA2DEC253E5EDB8E33B128786B82D88FF04283B666D110927B1ECD213FB1AEF186BB3EF1469E1B39F2C2D18A7A39B5EFBBBD7D27FB367C7A8397D3C708B88B70C34B16642A4D62497EECA70D168B5B2CB2788135D69321B3AE697ADDCBB91C5F12F4C3AFFEE0E8E471F3DF35CA1559BD2460DACBA9776DDBBE7DC75DCEF8A651A4B628A761255505FAC90478025EF4A2C12E17C3A4CA915A363105A99EB371C1AAC4117CF7E7F270A313A26E936A436A3A4C6CE4E119B99416DF579B830ED152F15762BD82E445D6F9D12479D202493DC7728A3456EA6C9995D5CADA154447EA884DC62558A4949CF5329E4D69E2C859123183AB9F763E29DFC9513D23723C11952949B1222A2502164C16C552F4D05339135BD73BB922DFB5A83070EAD2021648935BFEE0F47A89673D8CD4E3E3E99CE7C93C0A02D646BC1409B9C57D1BFB58C71305AC37D04538595C16A40CE08E8ED18147C00D2D4649122E2D4F381CE224C0F337C99AE408E64A5A80C82A9621D128F20B917E39324CB43B904CF44A9CC7B42444B62053C98E9B25B547396D2BA93AC45AA9612BA24E3E5203A062506513E19A42B1C2385C89E5C7402C2E20FF8B3BD0726203ADF934C2D5E81AAC3CA68FCD22F5BA2436CD0B120C7F5B316BE5CC804BC35477A6F256D37A72D2B71595D2092C0F921E2D87A225313B5594F19050749948C8C88FDD63015DBB6CA4BE0F69F2484A5598A567118D2069389E8D92098C433A756ACB45E92C8979D0B61922F05FAE75DA735D5751EBA1A1543BC2978D48BC090CCF3D9E79083A89CBEB1816658E86B83E2E9059A41469418E34C99A35AC58CC51FA5FB383BF325D905EFB7AC2DDDB05485489998E3CCD7C91DA8417985C1E693D436A135EDE4B304D9B92EA46DDEDE507252A3656DDD8A200E1CA9584C5C9E4B1C96838F4DB8CCD4AF311A38F46557F7BC63E7A256906A1E0253BA2806506CFB5AE2B25A7B8044A6121F10A72DEF58D742BF1E923B54A657ACAC98D0C853B218410520644297AC662901BF4BF8A21E12792643C49425BB7B4DE6F1299ABE3559AD026BE57E59E5495CC05C909743BF2E21E8708D73D5F25A7B80484920296932E214F1320D1EAD026A4D2A17027841042CA80AAF208B2328290B589377976DC14229B35492EC4CB15509822EA9DB2D46DAA19EDEC81BCE880A10003D42CB80E2969772FC940BB5BD77B277DF851C94E7742040A77420821A41CD838861EC7302D508D1371DD8A06DB1CE02B01097A95A073337ACFE8E77EAA5C508A5C8EB453B66866FD16B10EDB82EE41A9FFFB7A5372B44B00D21DDD7522BC6DAA0BD07E487BEF1352D150B81342082165C1F38D170DE4E3C1020637A9D71A199F72C567464B7AD6F9DAAB5D518A3CF6258F46D4BBE4158D1617BFF05A19AD05881F2FC1E84C31BEA484447810F44867B59F6952E350B81342082165A2606D53C66FCEE848A9A265D36152E18D1611895153454AC21FBCF05D4AAE16D7F2D02934B1EF154C34FE463E7FB4307E707CF8C0C4C8A1F1B10393E307C647F78F5DD7DFD881D1D1FD23138770A0B10372D0D19183A3A63B8EBAD14BC6BAF16F71C2E18757594F48E5C20F30553D0BF501267074E7D1C8C46BBB6E49E7490D31F0BBC1EE978E6FF8F0C6ECF26C1A75437260E7FEE6FAE6E59F5A91CE4B9D5EACC7B1A8A16DF92797A6F365E5DC2FCF9DDC7F72D3473606ED9934AADC9CFEC7530363839BBBAED6CE5C1B62BBFF0BAFB6B7B7777EE4C2EB954466DF17F7E1034C1F29F37725131327B1E7A55F06380FEDC8518CD44238AD2456092EC5C5748FED1E3BF4DB43920F6B02C9982864CF44317ABD8B70BEBE3993C38928C7330DB47F2401084B60D24C369B964D9F5E17E57C1DF71117E2BC335F49FCF04B4F1C2D1C337F97CE9EC73F18B3270D6EDBB66DC78E1DE90CA94528DCAB9E0512EE6A7C8FED3C1C1A734BD72DCECA56AC8123974D62067E3F4DB8BBCAADFA94C235606EE1DEDAD0BEF4939DE97C59A170BF049712EE4BDA3A967CB43CC25DFB54839A77EE4EDD3D71522814A263491C63EC768990BC97CE43B9B089B58B6CC3F2FAD88F90CE646AC4FF0AE4475FFEE191FC510A7742D545E607CC593C6EF263660CE3EF8AED451CCB4FAD60E3D016E4E2C6782C2F68F7D31A17155702C771AF19CAD893F9C6E98C31954FCF663299BA0DB9864DF58D9B1BE4570A94F1D7B0B9BE6E65463FE92A75593A3C65C5DEE1656FE7900A81C28BCC1FAF7DF1E28ED60EF5B327A9C023B541E235B4352E35CB83FA0C1C92D6D3CFA31042AE0B375447EA5471EA478E34A262B0B15C084C345D7A4DA47AA3E923150D853B99378959F2671D4B3FB6045FBB939223468ED2BD5648ACA9DF945BDDB5326896EBEABB6A96D79790EBC70DE4772FE6B4F2B22C564EEB32D46678471543CAE0E35084542E2CA0E43240BF672933FA9D0AED004DCF448DA00E277D588C6F908752BBA6DFFF2684906B4105FA0120D631943E9286AACD7D0CEB46694F916A85C29DCC0B317062CE8A6E13B571023D13B582D45EAE15A6D30001EB279558D51242AA920AD4C336C1D76AF146AA98BA7420C8AAFFD015A97928BCC8BC10EBA62A0E460D1FA94EA2C8C257E196926A076F2D7805092436920BAD3D51D1FB53171242480DE23CED1A7023527A099E395217918A860594CC0FFD4685FCA0E0AD6FADEFC3C091DAC19A4C8CCE503EBCEF8810E3C0861921E4DA53216FA9165D13534E0AFD4BBB472A1A0A77322F9C674255BBFC8388D74E152C3F35035E5C48AF2EFEBAAA8BD7F742708A484DE006122165A1C29EE65D60E82AD4EEF1112871B06226F342FB0242AC6BFFBF52B1A167A24610D11E39B19E443E44BC5C620E593E033C29354319C77127E40AE038EEC441E14EE6877EA85A0A0C3A4327897356C1454B6A8324F1E56A8A74B73ECC025ED6A27399104208A92C28BCC83C114D178FEE1F19DB3B8A6E8022E9D0C79D1EF71AC1DAA4D01F0E3E3B148DE31555C460126A90104208211501853B991709BE7AE79DFAE5A9EEA74E5ABC811FAB6867F9A915126FE4C0D81B2F1E2A0C24690F285C658C0B49082184900A81C28BCC0BEDE06E32A6CE3701FACC60F411131A7EA3A7564892D84BB2A61EDF228975E84FB8DCF94485104208A92028DCC9FC80589749E4994882DA57C604B1EF1692AAC7B3D93814A59E952619DE67C04B9856143C218410422A0656CC649EC4A2E82278DE3DBC989AE0434C91478F6C8DA0A3050511FAC6E013B96219E47227BCBE841042482541E14EE6093EB724C5C5BA2F546074198B7148484D20D7537B3FC518C45D9FAFE88FD7F74238D44ECDE086C622A45AE038EEC4C18A995C1EF4C1D634BCBC9780E3B8D70CD25225A48AE038EEC441E14E0821841042481540E14E0821841042481540E14E0821841042481540E14E08B97149947486104208A96C28DC092184104208A90228DC092184104208A90228DCC9E591969818DD0BD8C3A0768813CF580F1F62F2E53FFED6F4F54DB3868F01CBBFC44C5E22B7F83E558229C671C79A720360BEBC58377CA724445352F62E3F4112A8854807152D6F72E482E1CB04458B85C49C9F208EE34EAA0B8EE34E1C14EE647E40B9442214627C3955541E4C883561BA9454397241439BC8C5B576D2B3F82BAA26D585B588AB0021BDA1DCBDC4E2C3C073806F51D9D8E2CBB2D08311A26C590702973B51847A2069905C484254A7960D292DF28BBC8294992491C2839253CED3234DAC248E4C24D74C5226B60B7152B0A751D6CB47C865C3B7718883C29DCC0FA8163F36D617C122415751AB8E21358008D76CE4C5C6F7E24C2266018256A865FB804A503D58521DDA40FDC473548BF06A7BA2D8632BDA54EE04D7702D63C3C6F3F12D634D80A4451257BC666541A4BA1C5C4A0E0C030C8457CE7393A644AE692236CBD92E15EDACEF0821550F0D199927F0C15A632313192F825410AD972E22554FE225793FEF1B2F968B0BDD03595BDBD7B7A472F100DA13E939A73194B575032F0E42B905AC55E757F9ECA73E2C9074EB5D8927609298725E2F1B2649E4C330F8389749944833B07CE21DD7CAD3C60C4E15CE8E5EABB236260821E45A60BBBABAD220A94E1E79E491E1E1E174E613C66C4F8353741BF3DFCCFDDBEFDF7CD7E634E6F2494C68E3E0E8178E4AF8E6AE9BA53E5419436A87A1DF8D1C7FE9C8860F6FCC2CC7831488D9DABECC317CE870C442627AE1403E68CD4AF4819DFB9BEB9B977F6A855B6BFA4938B4F3B5D8C44DA649B4BB67C2B87CC25DAE4E6826C6CD78A359E499209296177E6553A61E9A0DC184192E987C9369B326D27685342BB48FCA822387962B75D69CBBE9A6D54B1E5A2C31E818A369423832FBBEB8AFA3BDA3F3234B304F4835F0C32F3D71B470CCFC5D3A7B1EFF60CC9E34B86DDBB61D3B76A433A416299FC7885415E8DD6BC3BC89F2262FF2205532E574F1916B8BC8D042DE1444DFC08FEB2CC3F97D826B09F416F5E43FFCD56206318B2E307320325F34A89C913032795D55A4AAFC957BA02C3F5C1BD1CAFAFEA595FF4862F9D2232D07630AA2952505E85084B75FE474C9F4BCD516EC67D18C4131F6B4406B6F99A2D52284906A861EF7AA67613CEE4ADCBDF374C1846B3EBFDAD586A8225919D6047225479E1F3AF5ECE9357F7C4B7669A61459C397175DDBADBEDA293ACF26E15012344300CFEC71C74AF1C19D875AB3CD4BFF7A793881B71E7D899BBB83CD75C3ABF3FA7EDED773E8E4FA0FDF1A2CF7CC84A4CF4F6C54AEF42426B2F541EF974F0EE407377E7A4B9C896C2189138B4178CA81CD9A682279ED9F0EB575B477FE59A735A1347210AF4BE97127D5083DEEC441E15EF52C907057E1124FA0EF7350E7AA63F4714F5DB3A4DA494C1C25D144E837F82267D12EAB61CD2EA03CA32787FB6B6C9C84810D4291C4B3759591337478E791B3E65CBDA98B4D9C31410C39589EF22FF75D020F77208D0749A03E3B854759C365C01D5A3DEEF0735B1DF4069DDDD3E56540AEECA4195FBB7A7DEB436D788D18C9292EA270275508853B7150B8573D0BE87127E44A814B16BD3AA0EF6C689200B308BBC515C4AC7DDC13F3FAC3AF8B40EE787B871DF0A2B2F623F21BBDA15D43E7A2C165B72CCBB6E64C3E0CB5A77ED95E504D92A025D3FB74CF84C92F7DD332CFF3E238F4631F234396031B0471A170F295EE254B972CF9E34E5CC36985CD09F7256D1D4B3E4AE14EAA861F7DF98747F24729DC09857BD543E14E2A1B7CAB4854BA53EA890D2D7CD549DAF9B8ACB8DE32D3A71239BBC71D8BDA1ADA97FE950AC17227FEECAFCE76EFEBDEFCD1CD415B06CE6E938EBD5F1622694B1873FA9F4E0F8C9EDDDCB5A514532E74E8766FDF175FE968EF845B1D672696C874247E7ADC4915428F3B71B09F0321E49A21F2370D2932AB7AD81741E98B5C12F9940412855E0B187C1C316564BA5E77D339B026861285FE4BE20A180BD58B7D2FFD90426C13A876690A950B3933228BF5755D49129A6A789C52BE73246722B131DE4FC5B841110A1B3AF0B8D3450821550C853B21E49A5194BFB1AAB748668B31B2CC4F95A5C66080D172FA644149B5CF93C83D3D4043445420B2E8E2CB823EB14002F09E89A8531D38C52D2A07AE87BD5428F257E4B21BAEA77C249EAF1F5F92429876B53FBF49490821550A853B21E41A03973A6C8BD3E52AE2112B5A0ECA328E52D77BD9B594487011E208C9646E158E852E53821B1A3276EABD2C687264920E2313B9B469B84C78D2A291B6043ED0564C5C199F00C4368A6CF1798E9C16498DAA774208A976CA68E80921B58945379844A6712C4A1D920EEE58C838189CB1D746A192AD09CB277C4B943CEEA9829F0D1D97048F08AC2FD9D18706EEBB9CE501E7338E74E076B485D02FC5C5960F3993B8A4921E7D6D17CF00DC8272E0195F9A34BEF12490A48F2374B47E4208A97228DC0921D7129547B1B5C9C0C0C033CF3CF3ECB3BB7EFBDBDFFDEA972F3FF9AB177EF9EB177EFDFC0B277E713ABFBF206B956D0894999898C8A7A119416A458846E80382EEE4C863BAA83CC49EF57D1348A2F42C469AA0729FD024B09224542BF8AA6C19DF619013A1A33F26DADC4AFB3595FF6D684208B96A38AA4CD5C3516548A5A1EE76BB77EFDEA79E7A2A8D9A46D6F8EF34EF59B2B5B5F96DCD365B11BE0351985612A2BA6EC651657482458DA679E5A757E4070A8917FBB1572EAD1CB406E77E7CB6FB54F7C6FB3706CB33661CC980882F9353591A33D9D6DC89AF1E1F4E4636FCF106EB9B246FE504952B3D7ED63763D181EF1FEC58D2B1F44F96E1ED5D5C64CF75DE921047952155074795210E0AF7AAE712C25DEACD934EB8BF7BF39D1BD5092522E58A5E0A945DA56E52F91FDA3890CA50F6A73E2D488652AF034157F013AF1885C1D9D271BB5D4D8EAE07B127EA078FB05515CD02EAFED8FAAE33008E84CE17D80906F6904D5D1F5FEC05AF42CEE72112468573A311CE7E60080EEC0E078BB42B42A24FDBD3C533138AA0C22E75B7BA0397473D880EFA21699E3A273351DC1C5F0292ECC1B73B473ADD525C81F307579C6DFDD212FCC1E993B0A653A2F4B4A6BBC225951DBA845F1EA5D21527BBF7BFFADB5F3F29C13BEF347F70BF191BC5C2C666B3F3BF9BB142E37FFCEC27E330C12736ADF53C9C5827F2A697A2EB8A1CAE74ACE9873EB8735F73FDE2A59FEAC4C9979285AB28AA1E2BBCB1F3D859D35B6FEAA544A01F90C66B2994D29D910280014CE65102AF0952B62213F9C677DFF6971237FF56044A225E27BD66A75AEF62A447769BC1BB0DF33A0972F8528A25ACA9C209D48143E594CA429CDB2B7889D99D8A49337EF39A351DEF5BE24ABB2BE43868141FF8E281D6C51DCBFE7C898E117925974CCD42015DA7DC3EAF743FD71735111A5083266705B3C59B5D4EAFBBFD534B422A145CB7D8FCE06BDF3F3E79C2FC7777115DAC06040AF71B090AF7AA67DE1EF7776EBE6BCBF43BFDB2809433B6F7D1D37118757E7285A804B5F6D3868E96804317B800168A9C408C28749184A29550C90950F289AF9E3017310BAEE281C6728FDDA18CA5E295FDC8865AEB1435AEAE3CDBCEA45A7575B6A60D752E0C21226719210EA2ADB873D1DA1283F5E7AC9835A9B292AAFC52A0549DEBE6A8DE91F13933AD927A7A5E66CF97E212564A1EC4F7ECEB6329AE65222A33C006C8971CC19B7875B4F7D73DCB3FB2266843A2FDA91D5E062EA5DA4CB3FBF6ECFDD56F7E2B91F7BCDDDCF3365D24CB12B3F361333AEA77757D26BD6A95843438F7ECDC57308536D3E6C45F51104B52C32133BEDCEB58F2C12585A182D3FD9191262556900B860C6A3372EE0B704D9043D8C00641502814A438CBE110E394D9EC941236BDD172ADC0CE33D204F3A2BC9C1C4D8B1BF77326B4044E854B6B26B86942BD713C69FACB8D2DCD3A39C1977B3EBD4C12E5EDB15F1D8D4C5C67B2B83290AEB8F124C633FE097362E3EA4DCB1FEAD4A4E2FAA65BCE135C6CFC2D367F6516ED765D5641C889D5BB2EC4BBE09A4E9C79592096D786181E09E04E2F668854267060FDE8ABFF7E74B2F801A60B2E1885FB8D04857BD5334F8FFBBBEE7AF7A6BB9CC7BD68BB2F0BDDEAE0CE03A2C936776D51119BCA0039822C2CD5AC6A4FA43A93905489A8184426ABD04F572879886555A95350B368ECECA882C41ED5B35EF46C4578132EC076BAE812C25A9355DA15AAAD390F3B25F4D39A1951739D37B77F5959B274DE4A71E9013D90BD4AFEE76AAEC81AB23315EE0AFCDFD86896ACA5F94A49ABE9399996523D939A55113643BF1B38F8D2C1377FF8CD75CBEAB112BEC4295CA6A0C1FE65B732F55ED9FBEA534FFD5A62DEF176F3B6BB11E54EE0FF78D84C8C40B84F2B35154362FA7EDA3BF1FA9888302960D27E9124FBAAF92470CAF42E328B3A6E698DC6622F0E421B05788A20A752D4BB6606FA199F2F9D1AD2E47CAEAD5C4E2FBE1E514EBA7B3E2029291D05BAAD7850173F7D7A716469E5CBC593F3E1415B94762BD33976555A1365C0A1E51C225DCAA52C940CC9A4B854B2E602F3C40F82300C4FF574E74C63AB6D2EE8A0907A13C5FAA801CD83C50F2E6D5897C3194CCFE3E510CB1EA519E0A91340F629C543EEF2D43455146E545604904B384FE2FEC82EF2BD9CDA6E977117B882F34016045C99C4FCF0AB3F9C12EEC2F4EB45E17E2341E15EF55CDAE37EC298FFDBBC73FB7DB7DEB515550B2A97CB36CF5A33D9633B8F48F8E6CFAF715FAD773AACC4B4CA383528D084B292CCB9784844D49DA8D7AD48A24B53DC13E4B3FC493791DDB890563D32AF7373398DB048579DDAC5FC54231411D63C3F0173E0D28606899E1E5572B217F43D39EF24CC0A060B375EE0CEDEB44B35757AA783753567A89ED1409879B522A8C5F1B4C1CDE80F69C15E869E1E3EFAF2D14D1FDA905B91434F2713E02ACDBAA759C03E9124D9E72B7B5E7DF237E8E37EF7DBCDDD774F5D9E87779A91D1A0ABEBB3C5880A439A5AE8FFA079C789C139C2A930E6CC977B260B63B2583B7278111A92023AC9E8798C646D3DB9184CE7BAE20A906B4E4C6F51968E8B4E5AD38A99844A8BA46C86DA26712B5CBC932B60EA48C51DBA809EB61998717D411229925D1AE45AF80534821224F5F2D0673E70DC776E5FDA78579DC4686143B737B996D24871A7C445BA4D2E0BDD1084F924C8A81B2091E6C6E51BD6EB8C4B6729B5725693497BFA2BA7967E72A9572F251C16C39DABF91A44520EDC35FAE15754B8BBAE327AAB4C5D310AF71B090AF7AAE712C25D6EEF9310EEF76DBB6FCB8E5BC382EBB37BD9365A4CBCEFD9230F1F956DD776AD9118789AB07738DEA51E7462516A08A9AC518BE13F3684E4C58C2CD69A1832186BCAFFB452517938476A7435D723577699CA53472A8E05B8EBE04C9A7D4FA8A4651DB7D8C982391CDFE9AEF4D898E8C6B3AD2C4C5F8E1322693D7F27121D2372D6F4394A59D08D90C85498CFBA99663C5DD93D044985F38CA46B6AC6252C14FBBAC783BF1F39F1E2B1F50FADCDADAE0B435C35D5379787A45F54AFB40EB37EB0FBE5979F7EFA69897CFB3DE66DF7E0D8725E44253DFCB01919F144B88B224B22E471FE4CBFBE38D6BC910D1D12761B4E9F969014821827087DBC50DA92085E601B892CF3A28C9C6D9C2D89D0114B1274D92A785180CFFDB85DC9C6DA43EA7A92A069913AA1259572225D18454133E579B81D34EE3C906E7769673F7B732F9D91E9D955DFB32408C973E9B914A97496E37AB157F00A81BE66A10B60492E373102AC0C0C9DB482252FB293307D0921367A6624B9A19F78724D432FCE4A294CA6ACCA7C90FDC75E98B1D9815DC39936DBB8A101A99C9F33626171F7973EB1B47161242E74178EFCE2C886BF589B69C98561885B1CDDBC5024E2589F72908A2408821F7CE97BC70BDDF0B8E36AA6F12914EE371214EE55CFA5BBCA9C36E6FF4AE7AE92C566F1BBCDBBDABADAA4F58F79BC8928F6A3E4B22A7AB0A02155D96B40AA5D1819A7ADA7F5B64CA73251E93123D8D6EDDEAD9F8ACE626D549C970096CFB21301A231156D4889D4BBD056E9A6B3806DF078024E77993A5FF52CA4FB47168BD944CEDDFEF52C684A91D3E206B3A119D5573C35A95AA5CE924AEC126F09EB492EBAD275FB39F65F4C81A049754FD2136F72D7E44BBB5E7AC1BC902EBC76DC7B8F79EB3DE961255D8F3C66CE486392948FCF7EF66F7EF0C3274E758B692057C512B3F801F31F5ABB1661C6DD7A95456A20459A0F3F356C1BBD5C90F9DDD3CFBE6AF6A6CB49752105ECFF4D83A054DE28DC6F2428DCAB9E4B77951937E6B7C67C2F9DBB1A3A4DE7FDE69DA3187C4EDD692613A3B7800866B11F7089EB5A76D28C2FD9D2DA72EF9244FB494BAC760086FCEDFB7ADFC0C860160A58B5AC2E8DD1937866A52907F24D66C28C2FDEB2B4F59DCD4E1AA7CBB05BB41C069F1CEE7FA5276772A1F67A4F975D087A0EC83124DD129035176F6D6DBEB765364D3C7174F4D40FCF0426ABAFB249CA25A71298D5F527798F4D58BFA8B1F3E39DAE5920914EF7BBEA7CFCF0E8C91F9FCE985CB15BC5CCA8EB3490568FFCD12AD7932CAFF8E84ABFEDA27CC9F1C2089D37CC448026923BFFB2BE6B07CD9C52D9A7349B967F7485D766465F98EC7FA6C7335939C989F132C63F67869F343F4F57BD86BC4FC4BB2B07FAFB47630EA54B6A8FBBEE7AEBD9BEFE302EA4F3D71929C02587B40BD7D7D71F3E7C389F9F1A967ED3A64DE3E3E35A24759D281E1D1D3D7BF6ACCC6EDFBEFDCC993358A04CDFDBE572714A5C78464A89A9ABAB3BB8FF80C6552BCB4DE787BBFE4483E759A70A019741AE459CECFEE29E8CC92E319D27CCB1DF9BDFBBA5A49A904AE0AF8DB94D2FAADE415350B8DF4850B8573D97F6B8BB3BFC51635E32461D4333DCF6974436C91A73CADC69EE5C6A56242694AAE0826ACAF97644AA4E9AFC920D4BDB1E6881CE8660762BC622DF4F7EEDD4D8C4886844F8E3D1095574BF7B9C0E117F31B1BED9993761E7FAC52D0F887077FDB3B1431C0ED2381AFCF970EFC1BEACC9A85A9D793F826CEBDA1B226D453BB76F6A6B7B57FB6CA761F2B589133F3D25EB079A3C7C4E46474A49175F841C55DA300DB9FA957FBD4A52817CA55D7A343D8937FAC678EF4F4E262610BD2D59D7A7127320C246B253903D89285FF191D541FB94B35F2491281E1C6332EEFE6A77C14CFAE8BC9BF8789F5212391772D5A421B4F2232B82B6CCE0AE919E5DA7A561213159E39D33A32F9A1786EB864C8B362E2EB78494909CC9B6F2D3EB6A468DF9A4315B35EC90A6C18FB528CA0A977B14B7CF2B4E9B30C741DDCE05B7C28CB33295532C31EE02A25CEBB417732B57DEF4E63BB6AE5EB1DA0B667D38736D490BC3F93CF7DC73BB76EDF23C2F8EE3458B16FDE55FFEA58B772B0F0E0E3EF6D86332BB76EDDADB6FBF7DD9B2656EE982A227538AB728FC279EF8D1B163C7EAFCDC443489D89518C7116755648A347F643AF7BD321B72083931EE4AC9059A7EF95C5828AD73B9B80DA519DE8DB9AEAECFC97E5D5C6591A6293605FFCC6367C2B1C97E33F0BC7966A2B580C4CB99919FAC50894927D390F7A7855E00002353494441540B3464CC878D79C7B42B35FDAA51B8DF4850B8573DE70977A9A045B85F3F43FC5FCCA27CF35F767D5CF60FBF6E5A2592EA2731FB9F3FF8CBE77E6EFEAB31ABD2387219FC2773F72D3BEAD735FDE2673FFF8B8F7EACA5AD358D2F074F3FFDF4CB2FBFFC99CF7CE64B5FFA92C8F7CF7D4E64E514030303DFF8C637DEF5AE777577771F3870A02C5580EB7EE6C2DFFAD6B7CE9C3923C9D8B973A7D966CCA72E325F15282B4B49FA1FC61C14E18E73383D5395845AE962829FFBC9AE5DAF3F87CE938DBA90543BEECA9E27DCEFDAB1E3E227EFA476A0EAAA2D4AD7F3827A4EEEEDE9BF2B46FDBC08246ECAF25323C436B270B317B99A42E2B8FA3D5405D3B3E959ED1A567EE6EEA672B1877EE17102778674BA13383D81B24AF9D37B11E727299654A2BF60C5D9433DC133A5AA74E2E72A29A41A90A2281771DA75F4F14499D432145EB5C56CBA41EEEDE9BF2B0695938FFE92DA9FDA755425358087CE4BD36CFF155FD9D23E6EBCB291608C99A92AF302553AB798BE56B8A34C97E6A570290173276C61D2E998A1097171B19198854BD1BC29254913EC61E09A4A4CE7D4092E06EC1CAFF0936A44AEA75CDCD2853626D40F51931A8617B8B6F0F506963BB9649C67B4D25766BA8B5B590C1AE6C722E2AF6C3FA42209F01243B102B8E22B5BAA3F6E90B221F99DD65AF6635854F85F65C9F9AA7406917A1DB8F82825212E8B6614E56549E7A5919496125B19293A0F4992BBEE9A48E7C8C00BEA154BF16462282D873BC315786EC9FC9112E7AEE0B4A2E7A91522350CFBB8573DB3BE9C3ADD283BAB7D95365A76F25FCDA2C9E68F777D4C8C030D7E8D7160D7ABBF78EED7E67F33E6A634865C9AD29DF59FCCDBD6DEBD685DD34F7FFAD36DDBEE58BC78C9E4245EB574727936D17C3D686A6A7AE69967FAFBFB4B7DDCFFF66FFF76BA1C2FF5713F73E6CC2BAFBC72DF7DF705FA9DD174F1022267A5A9A9F989279E9070DAC7FD2DDAC7DD2556B4885320D34D59A5F177C6BC663EDFD555B1094CDC792CBEC4BCEBA7CF3FF7DAB3E6FF9482E29693EA47AEF0578CD99DCEB18F7BCD43E15EF5CC2CDCAF475527FBFCDF4DEB44D347BB3E015FBB8ECB9E2E22D54E62F6BDF0EAAF9EFDB55C62B3A28275526522B7C37F3677AF7B5BFBE6254F3CF1FD34B2DC38E1EEFBBE0877992DB51F0607079D7017BBF1DC73CFB995CBCE9470FF741A5305C875FF7B69F2BA97532BD61E3AE19ECE3CFFD3E79F7DFD59F37F5CC50863A4A2705770DACBA9776EBBE3AD3BEE4E67482D42E15EF55C38AACC5DD3AA8F0B8CF2D5DB688C2AD3F0F1AE4F623C191D9A9DDABD6678F5B903BFDEF58B7454996235CF4A7D5EC8E912E17ECB8E45EB5A7FF2B37FBFF7DE777676764C4E4E96B4B24C8585F932E5A2458B9E7CF2C9EEEEEEE9A3CAB834B8154A1EF7D3A74FBFFAEAABEF7FFFFB832088A248D691A5B29A0B2C08717373EBBFFDDBBFE5F3F9CF757DFE0B3B1FC6A8329FD452E79250F9C54F84FBFE7454996B6060AF397869562F7D316DCFFC74D70BAF7154999AE38BA6F44DADB7DCB9ED6D6FE57090B50C857BD5739E70CF1AB3DA9889746EAA229140A49E17C1558A5750C1E48C39069DFE49F33723185456F631F5DD7552D5648C3F64469E32BF1E6C1C36CD5A54A49008975B4E5CD172EF5A385C79AB552477810E7B7F228D103EFA171F696B694F67CAC1AE5DBB4AAEF4E9E3B83B8686861E7DF4D174465DDD696821494D103E0CFCE31FFFF8C891232E1A6C30664C8B9098163137CE6A551AAE78D7A19F8CF07EF39018C368FAB84C95024E743A782F4EA7376C865F342F0D750E22F1A51A4102327501525DC83D22F5FEEBE99C70E75BB6BDF56D14EEB50C857B75932489D4C153C27D4158696ECA9B49AB8316D7B024BBA190AA3D67B28DA6FE805322E44A696969F9F0873FDCD0D090CE978363C78EB98EE38EDB6FBF7D6262C2F9D133998C980B59C12D5AB972E5073FF841175E78DC7380BD7BF73EF5D453695475F201F381505D1871C5295FB9E8BE367F2479B6DED49D31A77799170BF8C615A94DF801A69A87C2BDEAF9F6B7BF7DFAF4E974664E72B9FA5CCE4765995CF6D708630F7E1B9B78918D26ADFBB0A1177BA117CBAE48D523A2CE6A3B2C63327EE2C7EA7C135175590307C9EA619C8C8D8EEAFB70371CB7DD76DBBDF7DE9BCE9415278865FAD8638F0D0DE1E1D8C5BCF39DEFDCB2658B5B338D2A07A5A40A5FF8C217D2D8EAC29AAECF755568BFC12432D6377122F7B6DCD462C577BDF0CC73CFBC902E25B5C8DD77DF7DC71D77A433A416A170AF325C3D97CE280565B6DA5734BAE7C5BEEF3FF2CF8F2D5BB6ECBD0F3E3039598029775F509A37DA9F3DB1A2EB6C1EAE7613C45E7E6A643152E5C8A5953698B4C482D88468A4890AB9BCD69D94C06C367BE2E4F1EF7FF7070F3EF8E0FAF5EB27C6C68D67A3E4B21B0015087230ADE7B7CCD6D5D5E1654AED6A32313111C7B1643F08828BEFD03292CFE7671B2E2697CB799EE7925A4AF3C2277EFA11272727A3286A68683879F2E477BFFB5D6908BDE31DEF9073EB9656145A1CB448D8306B737E2E6BE5A6B1A1C55D534994FAB82328F7739CC449617C92A382D50672655108CFBF859D21D2E5A436A170AF62DCBD5ABA6367C4690D59FCE8D71F11E1FEEE071E9099B9379915ED29291B6B1505F15E891E267245E0A35A89E8382D31EA3BC45FFC4F57B8980B4A919BEDE9E97BFCF16F7EE0A10FAE5ABD12B170F5C93A15E98CBC6A4AC25DF22E814BDE8C954C85A4DC25E3ECD9B3FFF22FFF52E14FFCDDCD119BC81A5F022801157CF5DDED9C486AE1B599EBBE2655C4C5B76DF59A20327F28DCAB1B7797CE79AF8631FA38DA471E81707FE08107743818B1DC5722A4CE3B10747C013E785203E07B5A5AB3EBF5C5FF79D7EED34B454F4FCFE38F3FFED0430FAD5CBD4A4A58FA11F89A100A17DF654EB82F5DBA349FCFBB980A610E83307DD1C5ABCDB1E135A7742C09C8B4745CDFF7474747C7C7C745B58B767791958696686D8EA6655BC26EA0ADCA6AA022759AC2348018DCE30B748DC982E06EA5E9D37401A95128DCAB9E4BDFA852A758F3C8A38F88C278F0C107D3C8CB24D157513588EA4A65DE253CB2A48A409DEE443B3AC5BA8B2A977BBE9EF2522174C2FD030FBD7FE5EA95D8191ECF20DEEDB1DAB9A06AFCDEF7BED7DDDDED16916BCE7BDEF39E75EBD649A042B588761CC7A32AA8755F222AD41C42AA4F89754DE47CEF6B522D54E83D42AE0F14EED547E916BD4046CC883A59509DA8C7BDF381071E10937D05158C33F7AEDF433A2F0ACF4375456A80C4BDC466D04917D71A8F53E62A24B395BA9EDEFEC7FFED5FD3AE32A5C257435C70F74D8F9CFB4E5C30664CCCFCD35621B91066CC4885907A2E9C00C60D53A952D8DD83829C42A4D3CD50B8D7329579CB906B08EFDEEAA3744FCEA75683535C9F44EBB50EDC15BF827B1A9B249E7BD88AA09539AAF6DAC15AF4D375AFD6E15AA7AA5D2AF89971652F9D99861BD73F361196E91E510067DF4FD551CAF8F4FBCE85E7BE13170C97C20B1233FFB4B9CDD399F251CAC2FC53BE9068C559702FE7E3CD1FF5865420489BFE704D8B273261BD5FBB5C7CEF93DA8337707573E95B145D15DC3A31BCAA578356001705490D33977D98B1EC959A76A5655A546ACACE547EBD789529AC840C56FA4946CB36E392387D5A69945235FD7C566652C935A1F2AD13B97A28DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC092184104208A90228DC0921E44A490A61983F391E8F26B18B884D84683331969F3C3E3ED1333E703C6F86A65BDAC4D8B83032397C78D41B4DC607F285DEC9308C464F8F8DF7E7BD096BB10F138E8F8D9CCB8763C626D8C21426867B46860F0C0FED1F19DD3F327E2A6F0B5822C4EE4F9224B25EA4B3713C961F3F7B34F4C7622BD16E1D9B4C8C8D8D1C1E1E3A343C7A7064F8F5A1E15726C2A371204B6D3C3A382EBB1D3D343278706474DFF0D8D1C90869D64CC9B6499C1F1F1B3A1EFAA389240F319A482C456E354008216401F0B76FDF9E06494DB37BF7EEA6A6A675EBD6A5F3845C07464747F7EDDBB771E3C696969634AAB6B1BD3D3D87BF73B0356ECFADCE2022369E27D2DC9C78F644EF538786F69DDA7BD0BB75A8BE7E839F17656DA1B66D76F4D5E70FEF7EAA777D7DD31BCF9C8E5FE8F3D6341EFAF673675EC92D6B5EEC77C6A2B3475FDDB5FBE9B8717849C79AB8604DE1CC8167BFBD7FDFE1BEA123A77A8E1C39BBBFB1ADBEBD71695C30C613299D2491B19E687E4F8F100E9ED87BE8E95F79DB4D7D76B5978F35DE8B8FFEFAD0C1670E74BFD1DF73F854EFEBDD878F0EE60EE56EBAAB613237BAFB6B870EBE7EE8EC1B7DBD874F9F3A72B4E7906D35CB5A5663FFD6C64961F0F0EE377EF36BFBD64C2EBBC2CFDB1839B1924BF87E64C6D320218490EB0E3DEE841072857846D4B84CBC30C926D31CCF9E09C70B59635BFF6069C10C8D1C1D896211D4F0A68B84368303E3A70742D3B962A59FF127F3268CE33834519F3937F1E4702BA4BD4DC2686C340A9320B2D1C8A9DD3FFB6EB4D1DCFAE1FBEFD8BAFDCD5B3EB87ED09C38FCDB83C91B4136D4C359EBA3C9802DA5E13078C84E3CE3DD6E5E7B3D3F367CCE97868468ECD84B26CE444B4CDB1D9BB6BDE5AD6F7AF3833B5637D49D31070FFC2269F226270B752BCD2D3BEEBF6DCB8EDBDE74D7B65BDFB9AA616B3821A98D4C6CBD91D74DFC7C728739B47F6C7C70C0F3247BA2E691751327894DE4A084104216040A774208B9421213C4225B6D686D04A7739CF850CF5814CB34D7BC62F51633FE9A19ED3BE307567B9E7866705FA1BEDBBE79CDB291E5B93CD6F712E3654DC37A337ACC0C1E3C670351DF36B0B9A0B12E8C938143DF195869162FDFBE3AB36951FB5DADADABD6DEBE36DB6FFA77BFE4E70A680C4040439923E8E5FAF79D191C321DDBDFE5EFEA8D875FCE6654588B04B791DF641A96DDDFD078676BC3BAFA8D1F6ACA2CF7761F9CA88FC2C0347498B6559B5B5BB7B5B66C6F6BDD521FB4A934F78D57D7BFFBE4C8B059B2E3DD99BD3DD1F8EE8C2FF19240399CEC57DA049A5D4208210B01853B21845C29CED99C646DE285226245CE62D62489A8EFD80475E71AD6FDB97FDC1BEFFF4EDC9C582F4992ECE881432294DBEFB8371816993F9164D03F5DD46F76EDCD753D2BC333FF32BED84BC220C94853203B3E583877D474BC75794BE35DD118F60D3AFEC3EAA57FB43C5936AE5DE0A19F3DEDAE12CAA10FF6C5C7CFFADE727FF32D49DF9079E3748B244B257882B6842914D0E5DD43579720CE666213059258337EDAF4BFFE4C7FFF933D3D3F3A3B79605CDA22C89DEC7EFF99E84C5FE2AF34EBD714FA86BDD77ADA642FB24359AEBE7C09B21E218490058206971042AE101B7B5E826E2A9EE85F51D109F4ACA85951CA91686A51D3C6BB69551C47E3E6842C89452E771FEF3F3B9137AB9B9AE2D88671E489EC86FA3671BE7DD5CA4CAED59CEC9BB4BEEC3A97C46351DC1F19D39EEDCCC83A8108E9388C860AF9B36DEB5B96DEF6363B96C3B6E82483A32581F15F7F6DB475B4B0E5ED75E7BC8E7541DFD064FFC97316DE7191D7413269A2896EEB0D1692C43BBE6FB47074687367C39897F5EBC64E9A632FBFF0EAC1575EDB7764FFD02F06B2922BE87BEFF503134B070BB7BD3D3B92ED586FFA7BF37DA7CE79DABF1D7D7FE44FA88D074208210B01853B21845C21217A8A4C1AEB4730A5B12A59C86875426380193F3113CB37DC32199AE3A7F70575893DB3AF7FA59FACDEDADEEB3AC73B718F695C18C9DDFA263BB669F4A99F99C63E1BF8363049601351EE718C2E35A2BA075F7FF5DF1E7DEEB7DF78F299AF1D3CF1B5C9B609EC2180275DBBD0379DDCF77ACB22B361E36D8538CADCFAA1A67DADC9FEEF446D4895975B929C30A77FF7E3A79FFBC673CF3FFCDBC32F85379BDBDEF27E336CCCD844D306B3E9FEBF7ADB1D7FB26DC787EE6CFBD8D2BCB403247FB2C3C3B2C3F5EBB716C6C3E0D63F6B78A3CDECFF76DC26492A766D97068BFE25841072FDA1702784902B24484C64C58AAA7B3DED28E359F47A375624B51759E3C5D186DB72232DD9BD4F99C6DCD997F636B444CD2BEE9BC88BF0D5FE35227F0B4E044F24C1EA567F5D4B7CACBBAE3061EBBD7C36485AFCC09C8BCE8DFBD220F0B3F54B56DEB979C3EDAD8B474D747CD2640B3641D776D9DC1325DDFBF2D1303A7EC41CFDEDCE97F6EEDCBBEFF1DEB181FE60E24CFD8817C4363FE2B59BE62D2D1B57ADDD68CD70DE4CDA256B2673B2ADB40D827A936B680CEA3BEBEA56D405AD1E868DF14DFF8B47A3F8C44173F4373B5FDAB7F39503DFEC1B3AD767274FD58D595F9F2BC4783F554F052184900580C29D1042AE18D1EA8131FE221D2F1DDD55100927BA73BDEB30E766FD32B3B2251F1D1A7EF68D51D3DE685636E91034B25AE0C906D6C734B185B8107B2B9BDBD6989EDFBF1146DD5E2E976B09DA6F32BDBF3B313CFE529031416651C786FB97B4BF27F05B47264DE2C931D4DD1EC5663C3B79F827DE06D3B8CCD48F185F07A75CBADEF89E39B9F7A017484227CC62B368F9C7162F7F6FC7D60FDFD2D66AC6068F9A010C3B23BB915CE8A88EC8402CD948CCB03FF9DACFEC26D3B0123B94A3DBC82CDB68BC8C39F5F2219BC923C9D256C1789484104216060A774208B942621B66E3D08483AF4C8CBC3A31726874ECC0C8E8DE89C00BC78D15E90C977B64E289C51B5734349823BB7E37BC36DBD4F4870DE79C935E476681CF1A5F53B21E9CEA36DBE137FE69661F7ADAE47351906417ADFBC3E693E6DC89674E25DDA3638787478F16F6FEAC9019F06F69F42772189246A47F2698189F383260566DCEBCE9AEAE8D5BBBDEB4E1F35B6FEDDAB0F5934BF39BA3579E196ACC17BC6C5230261CB05E68322B36AE5AD3702EBF77CFE385FA24F625DA8C9E39363E74483FC0F446C10C8F9BF1231366D5A6DC9BDFD2B5796BD76D9BBBB66EF9FCFA5BFFAAB3B0A5F0EAD3A38D7DDA45479F15B83E33841042AE3B14EE841072852436F0CCB899D833BCF7852777EDFEF9F3CFFFF2E57D4F9D302F4ED675E0ADD3C817418E015D5A9666FCCEF898A9DF6433ED378505746AC76BACA2A44576E39B4DF09A2779990BEBB3AB566F315E9D892727AC1F659A6FBEFD0F3FDEF88A39F42FDFDBB5E7672FBDFCC3274F9FADDFB6E4CE37FD55705644B768FE24498627F33FEF5EB9AAD57F4FE38873F4E3CB4949D2B2B87E65A7317D933DF9A80EBD62BCD09A20F19364E95BDABD25757BF27B87F74E3665F2C7CCA1679E787EEFCF5E7EF957CFFEEE27C7CF3C3AD2FE64F7F29B3AA2F7348CCB56681EC83E8D69EDC8AE5A624C6FBE6F52E2E4E8AC44082164E1B05D5D5D6990D4348F3CF2C8D2A54B1F7CF0C1749E90EB404F4FCFE38F3FFED0430FAD5EBD3A8DAA71C24261FCC4583C62E250A47292783618CFB66DC98C67C2E85454B7AA5EA47DC1264152181B1C1F3BE6B7ADAC0B16AB9C57C6FAC6FDC924B32437716AAC7E71BD6982173B4C92C2E991300A1A5A1B334DB2636172F8E4E4F8F1A4BE4E547A14E71A5A96D567DB9208BD7344FF5B5B882646866CA6255B6F231F2F8CA22B8EE8774FD47F983F102FBA35138E16E2D1A46E793D0680940478E1C8D9C9FCEBA66D4B303E1C4E9E2AF8912DF8C68F129BCB36B405D9FAD1896C73361BC4B2FE34819E1F0B0B07A3960DB9A8D1A28580CEFD6E09218490EB0C85FB8D02853B59006E38E19EA8173AFD1BC3F56D4595473ABC39F434BEC3E489888E3CE3BBD5E0A5464875B06E531C0DFD8208286F1D5F122FBBE2E1A8D3E8069F7AC2BEDC5BA1388A1F99D88FF1415311F5FA1555979EF4781A72689F16F4C3F743694C60010E87A98E17290795901E3E16498E7DCA2AC88AFAEFB13134BA1CC5E0E5548991BCC58947E14E08210B049F721242C8152212D7096895B3624E7D8B912065CE4B0534C4B0EA70C5C971686509D92831FABD53D5C48890F5557D6BC06DA01D5E240E47D1F680AAF65443CB16D24E48121F961CBA5E66F157F700922882EAC712B401801CD097285F768408F4C217592E32DD1D1D295311EFCB5165AFBA39866B8F559DEB4E2451924D55FCB203E41001420821D71F0A774208B9522064BDC814F0595211B6F8A91F1A8BB01C02D7430F9AD4DBED626CAC8E6E11C82AF29DBAC7427458D7F5454EEBCAB211F62BEBA85A86F31B2BA6EBBB1753B10BEC238190C7361A831512CF174D2F016D15A8D8564D0F41AE5E76DD47247B4663401649B21087FD480AB48F0EE21189C6836CA1CB6546D6D03DEA3CEB114208592068700921E40A11C92BD3004E707DBF1471709943A6CB12747A818076AF76628298108E6DF5893B4D0E790EDD2DD65804B14875D9D6C79A585D5730210E047D8F8EF41A83B6017AD44B10FFA1F831B03A8E926EA3EBA9BF1C5344628574F354800B22DA9D5C9743C4E815EF8E1AFB260E5C180B102F9B2718B41E3831AFA915DC941042C87527B5C2841042AE1478A045113B51AC6A5E4D2B22555B63CE896CD1B841D1DB2DB1A1BEDD993AB0F121248035F14D56D5C6E821936810F259B4B4CA6CD72F069BC83FE7BE9FA6A1D39DC74ECAA720520FA247D7870002D6D7C303AB5D7DDCAE7425B7953624807AF415D762493335FD28841042AE2734B88410B2604C295D687113F8A2BC4521C7F9A193077F74E4E0CEE393DF19688C13F8F0A381937B5E7FE51F4EF47F61A8EE5892F50B03FD67F6FED391535F3D7AEC2B278E3CD633F893F1A63C76E8CCB873EB7BE8132F7F13BC668A08CCC80A692718C1AD8F376621FA232FECDB73ECD037CF4CFC2EDF880D9264D4FA1378F75487BE493707255F3E2184907241434C08210B867AC4A1869D831CD8B8104F9EFACDD1E3474E9C32278E9D3A39F092CD7966F0B5DE13BF7DE37874AA3B3E39F46ABEDE4423BDFD07C77A4E4EF4F44C1E7C6DE858CFEBF9A64975C6BBCEE97094CB8C0E45836F9A1A8B50EA895712ED198F0DE487DE3A5E9CCF0EBEFA9BE3DD7DBDDEBE38679289F133C3031376C216C77F448FF938D2D68076B9218410524E28DC09216461D18EEFA90EB6C62FF479BD878FAEF9D09BEEF9E07FDE32F9CECCAF9E19E9084FEF7FBE7EB5D9F137FFD39D9BDF3BF98BA1C1C3BF69DAB075CB1F7FEECEB776BDF5AE3F5A73F39B9BCD9B970CE4BC0892DA75B6F162C8F4C4B3E8A213E2206EF4171CC6C35799A0E671740871BC7D1AC793E3E7FAFBCCE60736DFDAFED77547C70A47BF76243A3C12B54405EC50FBE1688F7CC8764208216587C29D1042168A041A58F14C8C815A4419C7A6102563C66F9C10291D4CB4274326F4CEB4BEE503CBDB3ED072268C8268A02308827A7D7354447466FCA55F6616ED6EBDF39EFC58600AE86D0E551D5A7BEAB9177EBBF3573F7FF8E9033B5FC8FF34A8B749A1FFC49E877FF6EB9DCF1C7AF88D17769E1DFF59BE19FB3022EE6D6207CF0EBEF6B5938BCC9E5DFBCF46470AFEC8E1DD267C65CFA9D12F8E34ABE28F6C12485BC0A2E74CA95B3C218490B241434C08210B87E86C741617FD0E07B9FCF392EC72AFE3CDDB277E71E8E033FF5FF7B29F36FFE17D8B7A3A72B9A6C02E1ADAF79DBDBB7FD678479BDF7A473E848FDEC6DEB1735ECE06CB30948DC8788876D9596164F2D42BBB1A3798D577AD58DDD4B93C5A92B776F2D5D727C64DE7167F493673A6DB9C1D39E6630C19DDC4B351637DB66D69C384695B6EB2419B1F679BDB8CB7D834E6D6D5633C4A0C38635DCF763970DA679E10424819A17027849085A2D8E35C476589DDA08A36F10BA66E49DC67C6FB4D188CF82D0D51A85F570A27F3E3C3A3C62C99ECF7CC688C6F22D968E4FB93372F31F57F6606B5338CE870F49049A224CC8F995CE3A296B6CD2D765D73BEA960CE1EEB3B1B3404776EF8DBB52B3F94B326CA2718D01187D6F123B38B1ADB3FB278D2AC5DBAA53969F1BC4CC7BAAC597B5F53E6DDC1840E5623358476941730F424FBB813424899A1702784900503265714B06AE140FBA0C7DEE81BBDC77EF9C3BA0FDF76C7FBFEE79B4F3FD8FFED1FF7B7BD36296B651675BCF953F7BCE9BF34FEBE6F70FC9FC63B8C19CA0DED7FBD71D5D94C476B141B0F5DD88DF5F0E9A4BAA6A69BEFB8F5D4F3C32FFCE2172F1CFBDDCB3D3FF573AF792DF5B1B73C29982458B9386E6E12918F048816F7127CCE298CED409498C95C1461578119CFDBB17EEBC5890E25A92E764923243CDCFA841042CA0C853B21842C342293453DA3E70B5EFFB4F8D451836DF063936DB24DCD6662327CE3273FE9EFF966DC6A4445FB62A8E1ED1629ED4D8C9A6C5D926B427F17BC328A51202D3AE064A264CD1FBCF5BECF3CF807FF71FBAA4FAE3A6D06FC423CEA07E1F142CED8F8EC783C962F580C3F09318E6D4CECC35B8F791FCF00C2D0F826A9CBD9828F0F2D39B98EC37A128FF509218494190A77420859205CB713FC87D64E3C741CF74CD3E2A6E59DE6955F3DFBFBDFFD7F2F8EFDA477AD5915AD5CBB71557FF7E8333FF9FB3DAFFD3FC7B69A86A6F7B6F47BF989E1A193266B739938C6B6EAB9977D8A7E2F0C4E1EFDE5CE032F7F69EFAB7FFFEAB97FEC59695A0A6B37BDE9E62434BFFFE1CE975FF9FA505B68972C426301ABCB86894C2511F9511399BC84938620AE5B3DF99B174E8FEC9C6C4E7BC8E0E140AAE0DD3C2184903242E14E08210B04C67281204EC796D17EEE26F19A324D5B763487A6CE330D4B4DFB8A3FCB8DB5E6D66C68E9B8B56ED264EA4DCB8A7B1BFCB56118D986B075EBA6E6646B30E663273AA28C7697372648B239E3C3356EEA9B4DFBBA77E7469745ED372D5A796763623281C9148C6743B418E4C8189D1D5EFF4C5DA6F3F60D4DF15A6F52D47CD6EFBC6B759B69328BC350879D91353D1D6BD2F56E677D41082165C6767575A54152D33CF2C8234B972E7DF0C107D37942AE033D3D3D8F3FFEF8430F3DB47AF5EA348A4CC779ADA1833116A40B8876C7F8EA49841E2B268404574DAF2BA8B4D78FA246E8B25230C647EF7305CBB1440291456F16D95616B95126238BBDC9C6B228236BB51EF9D72F066F3BBBF48ECF07679D7AD74DB52FBBAC8F374F63D9007B4B3CD9264063A078206922A08181260221849032420F0A21842C14D395AF1541AC63432200D5AE9D52D0DDDDADA50E6FC8650CEE623C1FE331623C75A7B865E5A99D615B41B64D553B62649F89487E8984737D34A98B12CF774E7AB7512C474855BBC460CC1AAD11240CD52EBBC74E31F40DBCEF50EDCEEF4E0821A46C50B81342C802A2AA59D43054B18861FC8DD53B2E4B6090F1992555CCEEAFC8E5542F63A187AF2069D776EC067FD0D35D82EA0C0FB11554B8E878F5C16303ED9C634DB8E2DEF7752EFDE3CC201A0B7A5038F293489B07DA8B5D1386C348004771DE7C11ECBAB25B440821A4ACD0101342C8C291AA611D570641286EE72617D4A92EEB68A71759439DDC9E0F452EE0034C10D5F09A6B071A486D1D5546FE637B78EB6527217AA76B0F7578CBA55520CBE328D7D69ECD74D810BE749B44CEF8E393A8A2DAD5E56E4297064C6535D981F3B9A3B5809DA145410821A4AC50B81342C842A142D8216A1D6AD9C5A9D71CA3AB631AA5AE77D5E5F2BFA8C8BD04DB40884BB828EB355EF6A1221B9D6A124FC787970D31D03BF681DE361EDE879599441A06388A7667C7CED15507C25F7615E0AF3B621278680EA08F8DE0BACBEB26841042CA89AB150821845C7F9C6B5B03A9F94D63F07AAAA86C9D8A42C6A28B84B2AE5332DAC53DE82698D1A5EE3FA6C535B5EB8BCCEA3A6EE9D44ED20899D598D22AA5D534A04C6D420821A45CD016134208218410520550B8134208218410520550B8134208218410520550B8134208218410520550B8134208218410520550B8134208218410520550B8134208218410520550B8134208218410520550B813420821841052F118F3FF030DD0E862D88AAD1A0000000049454E44AE4260820000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000', 'Main Floor', 'png', 1000, 650, 96);

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
(8, 'FROMOBJECT.Run Method.Send Message.Hello FROMOBJECT\r\n', 1, 'Greetings'),
(11, 'Speech.Run Method.Say.Hello PARAM1\r\nPARAM1.Set Container.Livingroom', 1, 'PERSON arrived'),
(12, 'Speech.Run Method.Say.Goodbye PARAM1\r\nPARAM1.Set Container.Unknown\r\n', 1, 'PERSON left'),
(13, 'Speech.Run Method.Say.System is now in [SYSTEM.State] mode\r\nIF SYSTEM.State = HOME THEN\r\n  SYSTEM.Set Property.Violations = 0\r\nEND IF', 1, 'SYSTEM State Changed'),
(15, 'IF SYSTEM.Occupant Count = 0 THEN\r\n  SYSTEM.Run Method.Set State to Away\r\nELSE\r\n  SYSTEM.Run Method.Set State to Home\r\nEND IF', 1, 'Occupant Logic'),
(16, 'Speech.Run Method.Say From List.Speech List,Weather - Current\r\n', 1, 'Current Weather'),
(23, 'IF PARAM1.State = PARAM2 THEN\r\n  FROMOBJECT.Run Method.Send Message.Yes\r\nELSE\r\n  FROMOBJECT.Run Method.Send Message.No\r\nEND IF', 1, 'Is OBJECT STATE'),
(24, 'IF Speech.State = ON THEN\r\n    Speech.Run Method.Say.[SYSTEM.Plugins]\r\nEND IF', 1, 'Plugin Errored'),
(26, 'FROMOBJECT.Run Method.Send Message.I am in [SYSTEM.State] mode\r\nFROMOBJECT.Run Method.Send Message.[SYSTEM.Occupant String]\r\nFROMOBJECT.Run Method.Send Message.[SYSTEM.Occupied Location String]\r\nFROMOBJECT.Run Method.Send Message.[SYSTEM.Plugins]\r\n', 1, 'Status'),
(54, 'IF PARAM1.Container = Unknown THEN\r\n    FROMOBJECT.Run Method.Send Message.I do not know where\r\nELSEIF PARAM1.Container = "" THEN\r\n    FROMOBJECT.Run Method.Send Message.I do not know where\r\nELSE\r\n    FROMOBJECT.Run Method.Send Message.in the [PARAM1.Container]\r\nENDIF', 1, 'Where is OBJECT'),
(59, 'IF PARAM1.Container = PARAM2 THEN\r\n  FROMOBJECT.Run Method.Send Message.Yes\r\nELSE\r\n  FROMOBJECT.Run Method.Send Message.No\r\nEND IF', 1, 'Is OBJECT in the OBJECT'),
(82, 'SYSTEM.Run Method.Set State to Away', 1, 'AWAY Mode'),
(88, 'SYSTEM.Run Method.Set State to Home\r\nSYSTEM.Violation = 0', 1, 'HOME Mode'),
(103, 'Speech.Run Method.Say From List.Speech List,Wake Response\r\n\r\n', 1, 'VR Wake'),
(140, 'Speech.Run Method.Say From List.Speech List,Thanks', 1, 'Thanks'),
(144, 'FROMOBJECT.Run Method.Send Message.[House.Occupants]', 1, 'Who is here'),
(151, 'PARAM1.Set Container.PARAM2\r\nFROMOBJECT.Run Method.Send Message.OK', 1, 'OBJECT is in CONTAINER'),
(152, 'Speech.Run Method.Say.[SYSTEM.Occupant Count]', 1, 'Occupant Count'),
(166, 'IF PARAM1.PARAM2 = "" THEN\r\n  FROMOBJECT.Run Method.Send Message.I do not know PARAM1''s PARAM2\r\nELSE\r\n  FROMOBJECT.Run Method.Send Message.[PARAM1.PARAM2]\r\nEND IF', 1, 'What is OBJECT''s PROPERTY'),
(185, 'Jabber.Run Method.Send From List.Vaughn,Speech List,Time', 1, 'Jabber example'),
(195, 'Speech.Run Method.Say.Of course PARAM1', 1, 'This is OBJECT'),
(234, 'FROMOBJECT.Run Method.Send Message.PARAM1', 1, 'Who is PERSON'),
(240, 'FROMOBJECT.Run Method.Send Message.a [PARAM1.Object Type]', 1, 'What is OBJECT'),
(246, 'IF PARAM1.Object Type = "PERSON" THEN\r\n  FROMOBJECT.Run Method.Send Message.Yes\r\nELSE\r\n  FROMOBJECT.Run Method.Send Message.No\r\nEND IF', 1, 'Is OBJECT a OBJECT_TYPE'),
(250, 'PARAM1.Set Property.PARAM2 = PARAM3\r\nFROMOBJECT.Run Method.Send Message.Noted', 1, 'OBJECT PROPERTY is VALUE'),
(286, 'FROMOBJECT.Run Method.Send Message.Creepy', 1, 'Communication Test'),
(292, 'speech.run method.say.I am a [Vaughn.Object Type] object', 1, 'Test Script');

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
(0000001403, 'Hide Controls', 'Boolean', 'FALSE', 0000000083, 0, NULL);

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
(0000002803, 'Garage', NULL, '', 0000000181, 0000000135, NULL, '', 0000001196, 1, '2016-01-06 16:49:46', '2016-01-02 18:25:05', 30),
(0000002804, 'Kitchen', NULL, '', 0000000181, 0000000135, NULL, '', 0000001196, 1, '2016-01-06 15:27:22', '2016-01-06 15:27:22', 30),
(0000002805, 'Livingroom', NULL, '', 0000000181, 0000000135, NULL, '', 0000001196, 1, '2016-02-10 03:38:23', '2016-02-10 03:38:23', 30),
(0000002872, 'Unidentified Person', '', '', 0000000004, 0000000025, NULL, '', 0000001198, 0, '2016-02-15 16:06:04', '2016-02-15 16:06:04', 30),
(0000003055, 'Guest', '', 'Web UI user', 0000000004, 0000000025, NULL, '', 0000001198, 1, '2015-12-11 01:52:53', '2015-12-11 00:54:54', 30),
(0000003173, 'Weather', '', '', 0000000109, 0000000085, NULL, '', 0000000043, 0, '2016-02-12 10:01:04', '2016-02-12 10:01:04', 30),
(0000003180, 'WUnderground', '', 'WUnderground plugin''s Object', 0000000132, 0000000105, NULL, '', NULL, 0, '2016-02-15 15:40:03', '2016-02-13 21:37:50', 50),
(0000003192, 'Bluetooth', '', 'Bluetooth plugin''s Object', 0000000022, 0000000035, NULL, '', NULL, 0, '2016-02-15 15:40:02', '2016-02-15 15:40:02', 50),
(0000003193, 'Email', '', 'Email plugin''s Object', 0000000048, 0000000049, NULL, '', NULL, 0, '2016-02-15 15:40:02', '2016-02-15 15:40:02', 50),
(0000003195, 'Network Monitor', '', 'Network Monitor plugin''s Object', 0000000026, 0000000037, NULL, '', NULL, 0, '2016-02-15 15:40:02', '2016-02-15 15:40:02', 50),
(0000003196, 'PowerShell', '', 'PowerShell plugin''s Object', 0000000128, 0000000103, NULL, '', NULL, 0, '2016-02-15 15:40:03', '2016-02-15 15:40:03', 50),
(0000003197, 'Rest', '', 'Rest plugin''s Object', 0000000133, 0000000106, NULL, '', NULL, 0, '2016-02-15 15:40:03', '2016-02-15 15:40:03', 50),
(0000003198, 'Script Processor', '', 'Script Processor plugin''s Object', 0000000064, 0000000060, NULL, '', NULL, 0, '2016-02-15 15:40:03', '2016-02-15 15:40:03', 50),
(0000003199, 'Speech', '', 'Speech plugin''s Object', 0000000058, 0000000058, NULL, '', NULL, 0, '2016-02-15 15:40:03', '2016-02-15 15:40:03', 50),
(0000003200, 'Web Server', '', 'Web Server plugin''s Object', 0000000106, 0000000083, NULL, '', NULL, 0, '2016-02-15 15:40:03', '2016-02-15 15:40:03', 50);

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
(0000000398, '2016-02-15 16:06:04.029008', 0000003198, 0000000195, '12', 'Unidentified Person', 43, 'process_system_methods -> osae_sp_object_state_set -> event_log_add -> event_log_after_insert -> method_queue_add');

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
(0000000086, 0000000043, 0000000071, '2016-02-16', '2016-02-16 00:00:17', NULL, 50, 'SYSTEM', 0),
(0000000087, 0000000043, 0000000072, '01:54:06', '2016-02-16 01:54:06', NULL, 50, 'SYSTEM', 0),
(0000000088, 0000000043, 0000000073, '3', '2016-02-16 00:00:17', NULL, 50, 'SYSTEM', 0),
(0000000089, 0000000043, 0000000074, '0', '2015-12-28 02:54:42', NULL, 50, NULL, 0),
(0000000193, 0000000130, 0000000216, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000194, 0000000130, 0000000217, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000195, 0000000130, 0000000218, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000265, 0000000130, 0000000220, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000329, 0000000130, 0000000223, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000330, 0000000130, 0000000224, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000332, 0000000130, 0000000226, '', '2015-12-28 02:51:39', NULL, 50, NULL, 0),
(0000000334, 0000000043, 0000000228, '16', '2016-02-16 00:00:17', NULL, 50, 'SYSTEM', 0),
(0000000335, 0000000043, 0000000229, '01:54 AM', '2016-02-16 01:54:00', NULL, 50, 'SYSTEM', 0),
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
(0000003318, 0000000043, 0000000840, '0', '2016-02-15 15:21:58', NULL, 50, 'SYSTEM', 0),
(0000003319, 0000000043, 0000000841, '0', '2016-02-13 21:37:51', NULL, 50, 'SYSTEM', 0),
(0000003320, 0000000043, 0000000842, 'All Plugins are Running', '2016-02-13 21:37:51', NULL, 50, 'SYSTEM', 0),
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
(0000008414, 0000003055, 0000000273, '', '2015-12-28 02:51:39', NULL, 50, '', 2),
(0000008415, 0000003055, 0000000676, '-1', '2015-12-31 23:52:13', NULL, 50, '', 0),
(0000008416, 0000003055, 0000001029, '', '2015-12-28 02:51:39', NULL, 50, '', 1),
(0000008417, 0000003055, 0000001053, '30', '2016-01-01 16:58:51', NULL, 90, 'Vaughn', 0),
(0000008418, 0000003055, 0000001056, 'Speech', '2015-12-11 00:54:54', NULL, 50, '', 0),
(0000008419, 0000003055, 0000001073, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000008420, 0000003055, 0000001074, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000008421, 0000003055, 0000001322, '', '2015-12-28 02:51:39', NULL, 50, '', 0),
(0000009652, 0000003173, 0000000467, '30', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009653, 0000003173, 0000000468, '31', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009654, 0000003173, 0000000469, '41', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009655, 0000003173, 0000000470, '37', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009656, 0000003173, 0000000471, '40', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009657, 0000003173, 0000000472, '55', '2016-02-12 13:00:58', NULL, 50, 'WUnderground', 0),
(0000009658, 0000003173, 0000000473, '60', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009659, 0000003173, 0000000474, '63', '2016-02-12 13:00:58', NULL, 50, 'WUnderground', 0),
(0000009660, 0000003173, 0000000475, '10', '2016-02-12 13:00:58', NULL, 50, 'WUnderground', 0),
(0000009661, 0000003173, 0000000476, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009662, 0000003173, 0000000477, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009663, 0000003173, 0000000478, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009664, 0000003173, 0000000479, 'Overcast', '2016-02-12 10:00:53', NULL, 50, 'WUnderground', 0),
(0000009665, 0000003173, 0000000480, '9.0', '2016-02-12 18:30:53', NULL, 50, 'WUnderground', 0),
(0000009666, 0000003173, 0000000481, 'ENE', '2016-02-12 18:30:53', NULL, 50, 'WUnderground', 0),
(0000009667, 0000003173, 0000000482, '76%', '2016-02-12 18:30:53', NULL, 50, 'WUnderground', 0),
(0000009668, 0000003173, 0000000483, '30.45', '2016-02-12 18:00:53', NULL, 50, 'WUnderground', 0),
(0000009669, 0000003173, 0000000484, '28', '2016-02-12 18:00:53', NULL, 50, 'WUnderground', 0),
(0000009670, 0000003173, 0000000485, 'http://icons.wxug.com/graphics/wu2/logo_130x80.png', '2016-02-12 10:00:53', NULL, 50, 'WUnderground', 0),
(0000009671, 0000003173, 0000000486, '10.0', '2016-02-12 11:00:53', NULL, 50, 'WUnderground', 0),
(0000009672, 0000003173, 0000000487, '28', '2016-02-12 18:30:53', NULL, 50, 'WUnderground', 0),
(0000009673, 0000003173, 0000000488, '35.1', '2016-02-12 18:30:53', NULL, 50, 'WUnderground', 0),
(0000009674, 0000003173, 0000000489, 'Last Updated on February 12, 6:30 PM CST', '2016-02-12 18:30:53', NULL, 50, 'WUnderground', 0),
(0000009675, 0000003173, 0000000490, '68', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009676, 0000003173, 0000000491, '75', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009677, 0000003173, 0000000492, '41', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009678, 0000003173, 0000000493, '46', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009679, 0000003173, 0000000494, '75', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009680, 0000003173, 0000000495, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009681, 0000003173, 0000000496, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009682, 0000003173, 0000000497, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009683, 0000003173, 0000000498, '46', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009684, 0000003173, 0000000499, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009685, 0000003173, 0000000500, '20', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009686, 0000003173, 0000000501, '0', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009687, 0000003173, 0000000502, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009688, 0000003173, 0000000503, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009689, 0000003173, 0000000504, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009690, 0000003173, 0000000505, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009691, 0000003173, 0000000506, '20', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009692, 0000003173, 0000000507, '20', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009693, 0000003173, 0000000508, '', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009694, 0000003173, 0000000509, '', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009695, 0000003173, 0000000510, '', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009696, 0000003173, 0000000511, '', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009697, 0000003173, 0000000512, '', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009698, 0000003173, 0000000513, '', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009699, 0000003173, 0000000514, '', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009700, 0000003173, 0000000515, '', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009701, 0000003173, 0000000516, 'Partly Cloudy', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009702, 0000003173, 0000000517, 'Partly Cloudy', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009703, 0000003173, 0000000518, 'Mostly Cloudy', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009704, 0000003173, 0000000519, 'Clear', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009705, 0000003173, 0000000520, 'Clear', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009706, 0000003173, 0000000521, 'Clear', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009707, 0000003173, 0000000522, 'Clear', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009708, 0000003173, 0000000523, 'Clear', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009709, 0000003173, 0000000524, 'http://icons.wxug.com/i/c/k/nt_partlycloudy.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009710, 0000003173, 0000000525, 'http://icons.wxug.com/i/c/k/nt_partlycloudy.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009711, 0000003173, 0000000526, 'http://icons.wxug.com/i/c/k/nt_partlycloudy.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009712, 0000003173, 0000000527, 'http://icons.wxug.com/i/c/k/nt_clear.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009713, 0000003173, 0000000528, 'http://icons.wxug.com/i/c/k/nt_clear.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009714, 0000003173, 0000000529, 'http://icons.wxug.com/i/c/k/nt_clear.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009715, 0000003173, 0000000530, 'http://icons.wxug.com/i/c/k/nt_clear.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009716, 0000003173, 0000000531, 'http://icons.wxug.com/i/c/k/nt_partlycloudy.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009717, 0000003173, 0000000532, 'http://icons.wxug.com/i/c/k/partlycloudy.gif', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009718, 0000003173, 0000000533, 'http://icons.wxug.com/i/c/k/partlycloudy.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009719, 0000003173, 0000000534, 'http://icons.wxug.com/i/c/k/clear.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009720, 0000003173, 0000000535, 'http://icons.wxug.com/i/c/k/clear.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009721, 0000003173, 0000000536, 'http://icons.wxug.com/i/c/k/clear.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009722, 0000003173, 0000000537, 'http://icons.wxug.com/i/c/k/clear.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009723, 0000003173, 0000000538, 'http://icons.wxug.com/i/c/k/clear.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009724, 0000003173, 0000000539, 'http://icons.wxug.com/i/c/k/clear.gif', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009725, 0000003173, 0000000540, 'Partly cloudy. Low near 30F. SSE winds shifting to WNW at 10 to 15 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009726, 0000003173, 0000000541, 'Cloudy skies early, followed by partial clearing. Low 31F. Winds W at 10 to 15 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009727, 0000003173, 0000000542, 'A few clouds. Low 41F. Winds WNW at 10 to 20 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009728, 0000003173, 0000000543, 'A mostly clear sky. Low 37F. Winds S at 5 to 10 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009729, 0000003173, 0000000544, 'A few clouds overnight. Low 41F. Winds SSE at 10 to 15 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009730, 0000003173, 0000000545, 'A mostly clear sky. Low 46F. Winds WSW at 10 to 20 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009731, 0000003173, 0000000546, 'A mostly clear sky. Low 46F. Winds WSW at 10 to 20 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009732, 0000003173, 0000000547, 'Cloudy skies this evening will become partly cloudy after midnight. Patchy freezing drizzle possible. Low 21F. Winds ESE at 10 to 15 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009733, 0000003173, 0000000548, 'Clearing skies late. Lows overnight in the low 20s.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009734, 0000003173, 0000000549, 'Partly cloudy. High near 40F. Winds SSE at 10 to 20 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009735, 0000003173, 0000000550, 'Partly cloudy skies in the morning will give way to cloudy skies during the afternoon. High near 55F. Winds NW at 10 to 15 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009736, 0000003173, 0000000551, 'Sunny, along with a few afternoon clouds. High around 60F. Winds WNW at 10 to 20 mph.', '2016-02-12 13:00:58', NULL, 50, 'WUnderground', 0),
(0000009737, 0000003173, 0000000552, 'Mainly sunny. High 63F. Winds NW at 10 to 20 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009738, 0000003173, 0000000553, 'Sunny. High 68F. Winds SW at 10 to 15 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009739, 0000003173, 0000000554, 'A few passing clouds, otherwise generally sunny. High around 75F. Winds SSW at 10 to 15 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009740, 0000003173, 0000000555, 'A few passing clouds, otherwise generally sunny. High around 75F. Winds SSW at 10 to 15 mph.', '2016-02-12 16:00:58', NULL, 50, 'WUnderground', 0),
(0000009741, 0000003173, 0000000570, '7:33', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009742, 0000003173, 0000000571, '18:11', '2016-02-12 10:00:58', NULL, 50, 'WUnderground', 0),
(0000009743, 0000003173, 0000000572, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009744, 0000003173, 0000000573, '0', '2016-02-12 00:37:14', NULL, 50, '', 0),
(0000009745, 0000003173, 0000000809, '38.910210', '2016-02-12 10:00:53', NULL, 50, 'WUnderground', 0),
(0000009746, 0000003173, 0000000810, '-99.373505', '2016-02-12 10:00:53', NULL, 50, 'WUnderground', 0),
(0000009747, 0000003173, 0000000811, 'Night', '2016-02-12 18:11:04', NULL, 50, 'WUnderground', 0),
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
(0000009904, 0000003200, 0000000866, '60', '2016-02-15 15:40:03', NULL, 50, '', 0),
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
(0000009915, 0000002872, 0000000273, '', '2016-02-15 16:05:57', NULL, 50, '', 1),
(0000009916, 0000002872, 0000000676, '-1', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009917, 0000002872, 0000001029, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009918, 0000002872, 0000001053, '90', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009919, 0000002872, 0000001056, 'Speech', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009920, 0000002872, 0000001073, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009921, 0000002872, 0000001074, '', '2016-02-15 16:05:57', NULL, 50, '', 0),
(0000009922, 0000002872, 0000001322, '', '2016-02-15 16:05:57', NULL, 50, '', 0);

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
  SELECT base_type, object_name INTO vContainerType, vContainerName FROM osae_v_object WHERE object_id = NEW.container_id;
  SELECT base_type INTO vBaseType FROM osae_v_object WHERE object_id = OLD.object_id;
  SET vPersonID = (SELECT object_type_id FROM osae_object_type WHERE object_type = 'PERSON');
  IF NEW.object_type_id = vPersonID AND OLD.container_id <> NEW.container_id AND vContainerType = 'PLACE' THEN
    CALL osae_sp_debug_log_add('Object After Update - Update Screen Position: ', 'SYSTEM');
    #CALL osae_sp_system_count_room_occupants;
    #Update the Screen Objects to reflect the container
    CALL osae_sp_screen_object_position(OLD.object_name, vContainerName);
  END IF;

  IF OLD.state_id <> NEW.state_id AND vBaseType = "SENSOR" THEN
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