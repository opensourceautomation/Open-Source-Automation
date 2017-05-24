CALL osae_sp_object_type_add ('COSMUPDATER','Cosm Updater','','PLUGIN',1,0,0,1,'Cosm Updater');
CALL osae_sp_object_type_state_add ('COSMUPDATER','ON','Running','Cosm Updater is Running');
CALL osae_sp_object_type_state_add ('COSMUPDATER','OFF','Stopped','Cosm Updater is Stopped');
CALL osae_sp_object_type_event_add ('COSMUPDATER','ON','Started','Cosm Updater Started');
CALL osae_sp_object_type_event_add ('COSMUPDATER','OFF','Stopped','Cosm Updater Stopped');
CALL osae_sp_object_type_event_add ('COSMUPDATER','DATAWRITE','Data Written','Data Written');
CALL osae_sp_object_type_method_add ('COSMUPDATER','WRITEDATA','Write Data','','','','','Write Data');
CALL osae_sp_object_type_method_add ('COSMUPDATER','RELOADITEMS','Reload Item List','','','','','Reload Item List');
CALL osae_sp_object_type_method_add ('COSMUPDATER','OFF','Stop','','','','','Stop the Cosm Updater');
CALL osae_sp_object_type_method_add ('COSMUPDATER','ON','Start','','','','','Start the Cosm Updater');
CALL osae_sp_object_type_property_add ('COSMUPDATER','PollRate','Integer','','60',0,1,'PollRate');
CALL osae_sp_object_type_property_add('COSMUPDATER','Version','String','','',0,1,'Version of the Cosm Updater plugin');
CALL osae_sp_object_type_property_add('COSMUPDATER','Author','String','','',0,1,'Author of the Cosm Updater plugin');
CALL osae_sp_object_type_property_add('COSMUPDATER','Trust Level','Integer','','90',0,1,'Cosm Updater plugin Trust Level');

CALL osae_sp_object_type_add ('COSMITEM','Cosm Item','','THING',0,0,0,1,'Cosm Item');
CALL osae_sp_object_type_property_add ('COSMITEM','OSAObject','String','','',0,0,'OSAObject');
CALL osae_sp_object_type_property_add ('COSMITEM','OSAObjectProperty','String','','',0,0,'OSAObjectProperty');
CALL osae_sp_object_type_property_add ('COSMITEM','COSMFeedID','Integer','','0',0,0,'COSMFeedID');
CALL osae_sp_object_type_property_add ('COSMITEM','COSMDataStream','String','','',0,0,'COSMDataStream');
CALL osae_sp_object_type_property_add ('COSMITEM','COSMAPIKey','String','','',0,0,'COSMAPIKey');

