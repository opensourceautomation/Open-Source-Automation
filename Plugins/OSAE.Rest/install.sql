CALL osae_sp_object_type_add ('REST','Rest API','Rest','PLUGIN',0,0,0,1,'This object represents the Rest plugin.
The Rest plugin an be used to execute methods, get object
information, find all objects of a certain type, etc.
For more information: http://www.opensourceautomation.com/wiki/index.php?title=Rest');
CALL osae_sp_object_type_state_add('REST','OFF','Stopped','This state represents that the Rest plugin is NOT running.');
CALL osae_sp_object_type_state_add('REST','ON','Running','This state represents that the Rest plugin IS running.');
CALL osae_sp_object_type_event_add('REST','OFF','Stopped','This event will fire when the Rest plugin
state is changed to Stopped.');
CALL osae_sp_object_type_event_add('REST','ON','Started','This event will fire when the Rest plugin
state is changed to Running.');
CALL osae_sp_object_type_method_add('REST','GENERATECURRENTAUTHKEY','GenerateCurrentAuthKey','User Name','','','','Executing this method will create a valid Authintication key
used for remote access. To create a new key, run this method again.
The length this key is valid depends of the setting of
ApiKeyTimeOut.');
CALL osae_sp_object_type_method_add('REST','ON','Start','','','','','Executing this method will Start the Rest plugin
and set the state to Running.');
CALL osae_sp_object_type_method_add('REST','GENERATESECURITYKEY','GenerateSecurityKey','','','','','Executing this method will create a System Security key
used for remote access. To create a new key,
empty this property and run this method again.
See System Object for more info.');
CALL osae_sp_object_type_method_add('REST','GENERATEAPIKEY','GenerateApiKey','','','','','Executing this method will create a REST Api key
used for remote access. To create a new key,
empty this property and run this method again.');
CALL osae_sp_object_type_method_add('REST','OFF','Stop','','','','','Executing this method will Stop the Rest plugin
and set the state to Stopped.');
CALL osae_sp_object_type_property_add('REST','System Plugin','Boolean','','TRUE',0,0,'This designates if the Rest Plugin is a system plugin.
Select True/False');
CALL osae_sp_object_type_property_add('REST','Show Help','Boolean','','TRUE',0,1,'Select True/False for the Rest plugin to display the Help page.');
CALL osae_sp_object_type_property_add('REST','Rest Port','Integer','','8732',0,1,'Enter the system port number the Rest plugin will listen on.
The default value is 8732.');
CALL osae_sp_object_type_property_add('REST','Version','String','','',0,0,'This is the installed version of the Rest Plugin.
This property is automatically populated by the system.');
CALL osae_sp_object_type_property_add('REST','Author','String','','',0,0,'This is the Author of the Rest Plugin.
This property is automatically populated by the system.');
CALL osae_sp_object_type_property_add('REST','Trust Level','Integer','','90',0,0,'Enter the Trust Level the Rest plugin will have
to be able to control other objects.');
CALL osae_sp_object_type_property_add('REST','APIKEY','String','','',0,1,'This is the Rest Plugin API Key.
This property is used by remote clients and devices to encrypt and decrypt user information for authentication.');
CALL osae_sp_object_type_property_add('REST','ApiKeyTimeOut','Integer','','60',0,1,'Enter the number of seconds the Rest plugin will
allow a Time-Stamp of any received
Authentication Key to be valid.
Default is 60 (1 minute)');
