CALL osae_sp_object_type_add ('IPCam','IP Camera Plugin','','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add ('ON','Running','IPCam');
CALL osae_sp_object_type_state_add ('OFF','Stopped','IPCam');
CALL osae_sp_object_type_method_add ('SNAPSHOT','Snapshot','IP CAMERA','','','','');
CALL osae_sp_object_type_property_add  ('Height','Integer','300','IP CAMERA',0);
CALL osae_sp_object_type_property_add  ('Width','Integer','400','IP CAMERA',0);
CALL osae_sp_object_type_property_add ('Save Location','String','','IP CAMERA',0);
CALL osae_sp_object_type_property_add ('camSnapShot','String','','IP CAMERA',0);
