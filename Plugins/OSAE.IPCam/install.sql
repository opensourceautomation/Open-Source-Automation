CALL osae_sp_object_type_add ('IPCAM','IP Camera Plugin','IPCAM','PLUGIN',1,1,0,1,'This represents the IP CAM plugin');
CALL osae_sp_object_type_state_add('IPCAM','ON','Running','The IP Cam plgin IS Running');
CALL osae_sp_object_type_state_add('IPCAM','OFF','Stopped','The IP Cam Plugin is NOT Running');
CALL osae_sp_object_type_event_add('IPCAM','ON','Started','Occures when the IP Cam plugin state changes to Running');
CALL osae_sp_object_type_event_add('IPCAM','OFF','Stopped','Occures when the IP Cam plugin state changes to Stopped');
CALL osae_sp_object_type_method_add('IPCAM','ON','Start','','','','','Executes a Start command on the IP Cam plugin');
CALL osae_sp_object_type_method_add('IPCAM','OFF','Stop','','','','','Executes a Stop command on the IP Cam plugin');
CALL osae_sp_object_type_property_add('IPCAM','Height','Integer','','300',0,1,'Sets the Height of the IP Camera Stream');
CALL osae_sp_object_type_property_add('IPCAM','Width','Integer','','400',0,1,'Sets the Width of the IP Camera Stream');
CALL osae_sp_object_type_property_add('IPCAM','Save Location','String','','',0,1,'Sets the path to the location to save video or snapshots');
CALL osae_sp_object_type_property_add('IPCAM','Author','String','','',0,0,'The Author of the IP Cam plugin');
CALL osae_sp_object_type_property_add('IPCAM','Version','String','','',0,0,'The current version of the IP Cam plugin');

CALL osae_sp_object_type_add ('IP CAMERA','IP Camera Device','IPCam','THING',0,0,0,1,'Represents an IP Camera Device');
CALL osae_sp_object_type_state_add('IP CAMERA','ON','On','The IP Camera Device is ON');
CALL osae_sp_object_type_state_add('IP CAMERA','OFF','Off', The IP Camera Dewvice is OFF');
CALL osae_sp_object_type_event_add('IP CAMERA','OFF','Off','Occures when the state of the IP Camera device changes to OFF');
CALL osae_sp_object_type_event_add('IP CAMERA','ON','On','Occures when the state of the IP Camera device changes to ON');
CALL osae_sp_object_type_method_add('IP CAMERA','SNAPSHOT','Snapshot','','','','','Executes a SNAPSHOT command using the URL stored in the SNAP SHOT Property);
CALL osae_sp_object_type_property_add('IP CAMERA','Stream Address','String','','',0,1','The URL to retrieve the video stram from the IP Camera device');
CALL osae_sp_object_type_property_add('IP CAMERA','Height','Integer','','300',0,1,'The Height of the IP Camera Device Stream');
CALL osae_sp_object_type_property_add('IP CAMERA','Width','Integer','','400',0,1,'The Width of the IP Camera Device Stream');



