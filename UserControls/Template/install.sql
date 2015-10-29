--
-- Custom User Control install SQL
--

--
-- Change the following lines to your UserControl Name.
-- Only change "CHANGE THIS" in all UPPERCASE
--
-- Example: CALL osae_sp_object_type_add ('USER CONTROL MY NEW CONTROL','Custom User Control','SYSTEM','USER CONTROL',0,1,0,1);
--

CALL osae_sp_object_type_add ('USER CONTROL CHANGE THIS','Custom User Control','SYSTEM','USER CONTROL',0,1,0,1);
CALL osae_sp_object_type_property_add('USER CONTROL CHANGE THIS','Control Type','String','','',0);
CALL osae_sp_object_type_property_add('USER CONTROL CHANGE THIS','Object Name','String','','',0);
CALL osae_sp_object_type_property_add('USER CONTROL CHANGE THIS','X','Integer','','',0);
CALL osae_sp_object_type_property_add('USER CONTROL CHANGE THIS','Y','Integer','','',0);
CALL osae_sp_object_type_property_add('USER CONTROL CHANGE THIS','ZOrder','Integer','','',0);



--
-- Add any additional properties your Custom UserControl may require below
--