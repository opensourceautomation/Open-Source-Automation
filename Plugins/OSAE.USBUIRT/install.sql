CALL osae_sp_object_type_add ('USBUIRT','USBUIRT Plugin','','PLUGIN',1,1,0,1,'This object represents the USBIRT plugin.
This plugin can Currently transmits only. No receiving of IR.
Must use other software to "learn" commands.');
CALL osae_sp_object_type_state_add('USBUIRT','ON','Running','The USBIRT plugin is Running.');
CALL osae_sp_object_type_state_add('USBUIRT','OFF','Stopped','The USBIRT plugin is Stopped.');
CALL osae_sp_object_type_event_add('USBUIRT','ON','Started','The USBIRT plugin Started.');
CALL osae_sp_object_type_event_add('USBUIRT','OFF','Stopped','The USBIRT plugin Stopped.');
CALL osae_sp_object_type_method_add('USBUIRT','ON','Start','','','','','Start the USBIRT plugin.');
CALL osae_sp_object_type_method_add('USBUIRT','OFF','Stop','','','','','Stop the USBIRT plugin.');
CALL osae_sp_object_type_method_add('USBUIRT','TRANSMIT','Transmit','IR Code','','','','Transmit an IR code.');
CALL osae_sp_object_type_property_add('USBUIRT','System Plugin','Boolean','','FALSE',0,0,'Is the USBUIRT Plugin is a system plugin?');
CALL osae_sp_object_type_property_add('USBUIRT','Trust Level','Integer','','90',0,1,'The Trust Level of the Script Processor plugin.');
CALL osae_sp_object_type_property_add('USBUIRT','Version','String','','',0,1,'Version of the USBUIRT Plugin.');
CALL osae_sp_object_type_property_add('USBUIRT','Author','String','','',0,1,'Author of the USBUIRT Plugin.');




