CALL osae_sp_object_type_add ('IPCAM','IP Camera Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('IPCAM','ON','Running');
CALL osae_sp_object_type_state_add('IPCAM','OFF','Stopped');
CALL osae_sp_object_type_event_add('IPCAM','ON','Started');
CALL osae_sp_object_type_event_add('IPCAM','OFF','Stopped');
CALL osae_sp_object_type_method_add('IPCAM','SNAPSHOT','Snapshot','','','','');
CALL osae_sp_object_type_method_add('IPCAM','ON','Start','','','','');
CALL osae_sp_object_type_method_add('IPCAM','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add('IPCAM','Height','Integer','','300',0);
CALL osae_sp_object_type_property_add('IPCAM','Width','Integer','','400',0);
CALL osae_sp_object_type_property_add('IPCAM','Save Location','String','','',0);
CALL osae_sp_object_type_property_add('IPCAM','camSnapShot','String','','',0);
CALL osae_sp_object_type_property_add('IPCAM','Author','String','','',0);
CALL osae_sp_object_type_property_add('IPCAM','Version','String','','',0);

CALL osae_sp_object_type_add ('IP CAMERA','Core Type: Container','IPCam','THING',0,1,1,1);
CALL osae_sp_object_type_state_add('IP CAMERA','ON','On');
CALL osae_sp_object_type_state_add('IP CAMERA','OFF','Off');
CALL osae_sp_object_type_event_add('IP CAMERA','OFF','Off');
CALL osae_sp_object_type_event_add('IP CAMERA','ON','On');
CALL osae_sp_object_type_property_add('IP CAMERA','Stream Address','String','','',1);
CALL osae_sp_object_type_property_add('IP CAMERA','Height','Integer','','300',1);
CALL osae_sp_object_type_property_add('IP CAMERA','Width','Integer','','400',0);



