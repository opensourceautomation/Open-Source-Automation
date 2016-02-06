CALL osae_sp_object_type_add ('EVENTGHOST','EventGhost','EventGhost','PLUGIN',1,0,0,1);
CALL osae_sp_object_type_state_add('EVENTGHOST','ON','Running');
CALL osae_sp_object_type_state_add('EVENTGHOST','OFF','Stopped');
CALL osae_sp_object_type_event_add('EVENTGHOST','ON','Started');
CALL osae_sp_object_type_event_add('EVENTGHOST','OFF','Stopped');
CALL osae_sp_object_type_method_add('EVENTGHOST','ON','Start','','','','');
CALL osae_sp_object_type_method_add('EVENTGHOST','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add('EVENTGHOST','Version','String','','',0);
CALL osae_sp_object_type_property_add('EVENTGHOST','Author','String','','',0);