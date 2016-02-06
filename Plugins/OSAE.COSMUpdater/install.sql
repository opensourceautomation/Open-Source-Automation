CALL osae_sp_object_type_add ('COSMUPDATER','Cosm Updater','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('COSMUPDATER','ON','Running');
CALL osae_sp_object_type_state_add ('COSMUPDATER','OFF','Stopped');
CALL osae_sp_object_type_event_add ('COSMUPDATER','ON','Started');
CALL osae_sp_object_type_event_add ('COSMUPDATER','OFF','Stopped');
CALL osae_sp_object_type_event_add ('COSMUPDATER','DATAWRITE','Data Written');
CALL osae_sp_object_type_method_add ('COSMUPDATER','WRITEDATA','Write Data','','','','');
CALL osae_sp_object_type_method_add ('COSMUPDATER','RELOADITEMS','Reload Item List','','','','');
CALL osae_sp_object_type_method_add ('COSMUPDATER','OFF','Stop','','','','');
CALL osae_sp_object_type_method_add ('COSMUPDATER','ON','Start','','','','');
CALL osae_sp_object_type_property_add ('COSMUPDATER','PollRate','Integer','','60',0);
CALL osae_sp_object_type_property_add('COSMUPDATER','Version','String','','',0);
CALL osae_sp_object_type_property_add('COSMUPDATER','Author','String','','',0);

CALL osae_sp_object_type_add ('COSMITEM','Cosm Item','','THING',0,0,0,1);
CALL osae_sp_object_type_property_add ('COSMITEM','OSAObject','String','','',0);
CALL osae_sp_object_type_property_add ('COSMITEM','OSAObjectProperty','String','','',0);
CALL osae_sp_object_type_property_add ('COSMITEM','COSMFeedID','Integer','','0',0);
CALL osae_sp_object_type_property_add ('COSMITEM','COSMDataStream','String','','',0);
CALL osae_sp_object_type_property_add ('COSMITEM','COSMAPIKey','String','','',0);

