CALL osae_sp_object_type_add('KILLAWATT', 'Kill-A-Watt Plugin', '', 'PLUGIN', 1, 0, 0, 1);
CALL osae_sp_object_type_state_add('ON', 'Running', 'KILLAWATT');
CALL osae_sp_object_type_state_add('OFF', 'Stopped', 'KILLAWATT');
CALL osae_sp_object_type_event_add('ON', 'Started', 'KILLAWATT');
CALL osae_sp_object_type_event_add('OFF', 'Stopped', 'KILLAWATT');
CALL osae_sp_object_type_method_add('ON', 'Start', 'KILLAWATT', '', '', '', '');
CALL osae_sp_object_type_method_add('OFF', 'Stop', 'KILLAWATT', '', '', '', '');
CALL osae_sp_object_type_property_add('Port', 'Integer', '', 'KILLAWATT', 0);
CALL osae_sp_object_type_property_add('Interval', 'Integer', '', 'KILLAWATT', 0);
CALL osae_sp_object_type_property_add('Computer Name', 'String', '', 'KILLAWATT', 0);
CALL osae_sp_object_type_property_add('VREF', 'Integer', '', 'KILLAWATT', 0);

CALL osae_sp_object_type_add('KILLAWATT MODULE', 'Kill-A-Watt Module', '', 'KILLAWATT MODULE', 0, 0, 0, 1);
CALL osae_sp_object_type_property_add('Error Correction', 'String', '', 'KILLAWATT MODULE', 0);
CALL osae_sp_object_type_property_add('Current Watts', 'String', '', 'KILLAWATT MODULE', 0);
CALL osae_sp_object_type_property_add('RSSI', 'Integer', '', 'KILLAWATT MODULE', 0);
CALL osae_sp_object_type_event_add('Current Watts', 'Current Watts Changed', 'KILLAWATT MODULE');
