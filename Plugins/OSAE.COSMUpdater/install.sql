CALL osae_sp_object_type_add ('COSMUPDATER','Cosm Updater','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','COSMUPDATER');
CALL osae_sp_object_type_state_add ('OFF','Stopped','COSMUPDATER');
CALL osae_sp_object_type_event_add ('ON','Started','COSMUPDATER');
CALL osae_sp_object_type_event_add ('OFF','Stopped','COSMUPDATER');
CALL osae_sp_object_type_event_add ('DATAWRITE','Data Written','COSMUPDATER');
CALL osae_sp_object_type_method_add ('WRITEDATA','Write Data','COSMUPDATER','','','','');
CALL osae_sp_object_type_method_add ('RELOADITEMS','Reload Item List','COSMUPDATER','','','','');
CALL osae_sp_object_type_method_add ('OFF','Stop','COSMUPDATER','','','','');
CALL osae_sp_object_type_method_add ('ON','Start','COSMUPDATER','','','','');
CALL osae_sp_object_type_property_add ('PollRate','Integer','','COSMUPDATER',0);

CALL osae_sp_object_type_add ('COSMITEM','Cosm Item','','COSMITEM',0,0,0,1);
CALL osae_sp_object_type_property_add ('OSAObject','String','','COSMITEM',0);
CALL osae_sp_object_type_property_add ('OSAObjectProperty','String','','COSMITEM',0);
CALL osae_sp_object_type_property_add ('COSMFeedID','Integer','','COSMITEM',0);
CALL osae_sp_object_type_property_add ('COSMDataStream','String','','COSMITEM',0);
CALL osae_sp_object_type_property_add ('COSMAPIKey','String','','COSMITEM',0);

