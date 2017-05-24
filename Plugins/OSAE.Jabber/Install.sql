CALL osae_sp_object_type_add ('JABBER','Jabber Plugin','Jabber','PLUGIN',1,1,0,1,'Jabber plugin.
The Jabber plugin is used to enable communication
with OSA via a jabber instant messager like Google Talk.');
CALL osae_sp_object_type_state_add('JABBER','ON','Running','The Jabber plugin is Running.');
CALL osae_sp_object_type_state_add('JABBER','OFF','Stopped','The Jabber plugin is NOT running.');
CALL osae_sp_object_type_event_add('JABBER','ON','Started','The Jabber plugin Started.');
CALL osae_sp_object_type_event_add('JABBER','OFF','Stopped','The Jabber plugin Stopped.');
CALL osae_sp_object_type_method_add('JABBER','SEND MESSAGE','Send Message','To','Message','','','Send Message');
CALL osae_sp_object_type_method_add('JABBER','SEND FROM LIST','Send From List','To','List','','','Send From List');
CALL osae_sp_object_type_method_add('JABBER','SEND QUESTION','Send Question','To','','','','Send Question');
CALL osae_sp_object_type_method_add('JABBER','OFF','Stop','','','','','Stop the Jabber plugin.');
CALL osae_sp_object_type_method_add('JABBER','ON','Start','','','','','Start the Jabber plugin.');
CALL osae_sp_object_type_property_add('JABBER','Username','String','','',0,1,'Enter the Username to login to the Jabber service.');
CALL osae_sp_object_type_property_add('JABBER','Password','String','','',0,1,'Enter the Password used to Login to the Jabber service.');
CALL osae_sp_object_type_property_add('JABBER','System Plugin','Boolean','','TRUE',0,0,'Is the Jabber Plugin is a system plugin?');
CALL osae_sp_object_type_property_add('JABBER','Debug','Boolean','','FALSE',0,1,'Add extra logging to help with Debugging.');
CALL osae_sp_object_type_property_add('JABBER','Trust Level','Integer','','90',0,1,'The Trust Level of the Jabber plugin.');
CALL osae_sp_object_type_property_add('JABBER','Version','String','','',0,0,'Version of the Jabber Plugin.');
CALL osae_sp_object_type_property_add('JABBER','Author','String','','',0,0,'Author of the Jabber Plugin.');