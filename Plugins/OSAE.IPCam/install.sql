CALL osae_sp_object_type_add ('IPCam','IP Camera Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('IPCam','ON','Running');
CALL osae_sp_object_type_state_add ('IPCam','OFF','Stopped');
CALL osae_sp_object_type_method_add ('IPCam','SNAPSHOT','Snapshot','','','','');
CALL osae_sp_object_type_property_add ('IPCam','Height','Integer','','300',0);
CALL osae_sp_object_type_property_add ('IPCam','Width','Integer','','400',0);
CALL osae_sp_object_type_property_add ('IPCam','Save Location','String','','',0);
CALL osae_sp_object_type_property_add ('IPCam','camSnapShot','String','','',0);
