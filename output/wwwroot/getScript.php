<?php
  require_once('includes/connect.php');
  if(isset($_GET["obj"]))
  { 
    $query = 'select event_script from osae_v_object_event_script where object_name = \'' . $_GET["obj"] . '\' and event_name=\'' . $_GET["event"].'\'';

    $results = mysql_query($query)
    or die ('Error in query: $query. ' . mysql_error());
    $row = mysql_fetch_row($results);
    
    echo $row[0];
  }
      
?>
