CALL osae_sp_object_type_add('KILLAWATT','Kill-A-Watt Plugin','','PLUGIN', 1, 0, 0, 1);
CALL osae_sp_object_type_state_add('KILLAWATT','ON','Running');
CALL osae_sp_object_type_state_add('KILLAWATT','OFF','Stopped');
CALL osae_sp_object_type_event_add('KILLAWATT','ON','Started');
CALL osae_sp_object_type_event_add('KILLAWATT','OFF','Stopped');
CALL osae_sp_object_type_method_add('KILLAWATT','ON','Start','','','','');
CALL osae_sp_object_type_method_add('KILLAWATT','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add('KILLAWATT','Port','Integer','','4',0);
CALL osae_sp_object_type_property_add('KILLAWATT','Interval','Integer','','60',0);
CALL osae_sp_object_type_property_add('KILLAWATT','Computer Name','String','','',0);
CALL osae_sp_object_type_property_add('KILLAWATT','VREF','Integer','','0',0);
CALL osae_sp_object_type_property_add('KILLAWATT','Trust Level','Integer','','90',0);
CALL osae_sp_object_type_property_add('KILLAWATT','Version','String','','',0);
CALL osae_sp_object_type_property_add('KILLAWATT','Author','String','','',0);

CALL osae_sp_object_type_add('KILLAWATT MODULE','Kill-A-Watt Module','','KILLAWATT MODULE',0,0,0,1);
CALL osae_sp_object_type_event_add('KILLAWATT MODULE','Current Watts','Current Watts Changed');
CALL osae_sp_object_type_property_add('KILLAWATT MODULE','Error Correction','String','','',0);
CALL osae_sp_object_type_property_add('KILLAWATT MODULE','Current Watts','String','','',0);
CALL osae_sp_object_type_property_add('KILLAWATT MODULE','RSSI','Integer','','0',0);

