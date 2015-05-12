CALL osae_sp_object_property_set('SYSTEM','Debug','FALSE','','');
DELETE FROM osae_debug_log;
DELETE FROM osae_event_log;
DELETE FROM osae_method_log;
DELETE FROM osae_object_property_history;
DELETE FROM osae_object_state_history;
DELETE FROM osae_object_state_change_history;
#DELETE FROM osae_images;
DELETE FROM osae_object
  WHERE object_type_id IN (25, 50, 51, 54, 65, 95, 86, 35, 58, 87,34,24, 61, 81, 74, 77);


SELECT * FROM osae_v_object;
