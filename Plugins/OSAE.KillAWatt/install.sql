CALL osae_sp_object_type_add('KILLAWATT','Kill-A-Watt Plugin','','PLUGIN', 1, 0, 0, 1,'Kill-A-Watt Plugin');
CALL osae_sp_object_type_state_add('KILLAWATT','ON','Running','Kill-A-Watt Plugin is Running');
CALL osae_sp_object_type_state_add('KILLAWATT','OFF','Stopped','Kill-A-Watt Plugin is Stopped');
CALL osae_sp_object_type_event_add('KILLAWATT','ON','Started','Kill-A-Watt Plugin Started');
CALL osae_sp_object_type_event_add('KILLAWATT','OFF','Stopped','Kill-A-Watt Plugin Stopped');
CALL osae_sp_object_type_method_add('KILLAWATT','ON','Start','','','','','Start the Kill-A-Watt Plugin');
CALL osae_sp_object_type_method_add('KILLAWATT','OFF','Stop','','','','','Stop the Kill-A-Watt Plugin');
CALL osae_sp_object_type_property_add('KILLAWATT','Port','Integer','','4',0,1,'Port');
CALL osae_sp_object_type_property_add('KILLAWATT','Interval','Integer','','60',0,1,'Interval');
CALL osae_sp_object_type_property_add('KILLAWATT','VREF','Integer','','0',0,1,'VREF');
CALL osae_sp_object_type_property_add('KILLAWATT','Version','String','','',0,1,'Version of the Kill-A-Watt plugin');
CALL osae_sp_object_type_property_add('KILLAWATT','Author','String','','',0,1,'Author of the Kill-A-Watt plugin');
CALL osae_sp_object_type_property_add('KILLAWATT','Trust Level','Integer','','90',0,1,'Kill-A-Watt plugin Trust Level');

CALL osae_sp_object_type_add('KILLAWATT MODULE','Kill-A-Watt Module','','KILLAWATT MODULE',0,0,0,1,'Kill-A-Watt Module');
CALL osae_sp_object_type_event_add('KILLAWATT MODULE','Current Watts','Current Watts Changed','Current Watts Changed');
CALL osae_sp_object_type_property_add('KILLAWATT MODULE','Error Correction','String','','',0,0,'Error Correction');
CALL osae_sp_object_type_property_add('KILLAWATT MODULE','Current Watts','String','','',0,0,'Current Watts');
CALL osae_sp_object_type_property_add('KILLAWATT MODULE','RSSI','Integer','','0',0,1,'RSSI');

