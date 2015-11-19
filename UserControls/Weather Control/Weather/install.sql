CALL osae_sp_object_type_add ('USER CONTROL WEATHERCONTROL','Custom User Control','SYSTEM','USER CONTROL',0,1,0,1);

CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Control Type','String','','',0);
CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Object Name','String','','',0);

CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','X','Integer','100','',0);

CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','Y','Integer','100','',0);

CALL osae_sp_object_type_property_add('USER CONTROL WEATHERCONTROL','ZOrder','Integer','1','',0);

