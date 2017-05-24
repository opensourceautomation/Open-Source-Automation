CALL osae_sp_object_type_add ('REST','Rest API','Rest','PLUGIN',0,0,0,1,'The Rest plugin.
The Rest plugin an be used to execute methods, get object
information, find all objects of a certain type, etc.');
CALL osae_sp_object_type_state_add('REST','OFF','Stopped','The Rest plugin is Stopped.');
CALL osae_sp_object_type_state_add('REST','ON','Running','The Rest plugin is Running.');
CALL osae_sp_object_type_event_add('REST','OFF','Stopped','The Rest plugin Stopped.');
CALL osae_sp_object_type_event_add('REST','ON','Started','The Rest plugin Started.');
CALL osae_sp_object_type_method_add('REST','OFF','Stop','','','','','Stop the Rest plugin.');
CALL osae_sp_object_type_method_add('REST','ON','Start','','','','','Start the Rest plugin.');
CALL osae_sp_object_type_property_add('REST','System Plugin','Boolean','','TRUE',0,0,'Is the Rest Plugin is a system plugin.');
CALL osae_sp_object_type_property_add('REST','Show Help','Boolean','','TRUE',0,1,'Select True/False for the Rest plugin to display the Help page.');
CALL osae_sp_object_type_property_add('REST','Rest Port','Integer','','8732',0,1,'Enter the system port number the Rest plugin will listen on.');
CALL osae_sp_object_type_property_add('REST','Version','String','','',0,0,'Version of the Rest Plugin.');
CALL osae_sp_object_type_property_add('REST','Author','String','','',0,0,'Author of the Rest Plugin.');
CALL osae_sp_object_type_property_add('REST','Trust Level','Integer','','90',0,0,'The Trust Level of the Rest plugin.');