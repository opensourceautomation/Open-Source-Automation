

--Update Images to have consistant Path, Removeing "." from ".\"
update osae_object_property set property_value = replace(property_value, '.\\', '\\') where property_value like '.\\\\%'


-- Set DB version 
CALL osae_sp_object_property_set('SYSTEM', 'DB Version', '0.4.0', '', '');