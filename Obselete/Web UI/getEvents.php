<?php
  require_once('includes/connect.php');
  
  $query = 'select event_label, event_name from osae_v_object_event where object_name = "' . $_GET["obj"] . '"';
  $results = mysql_query($query)
    or die ('Error in query: $query. ' . mysql_error());

  while($row = mysql_fetch_row($results))
  {
    echo '<li><a href="#" onclick="eventSelected(\''.$_GET["obj"].'\',\''.$row[1].'\',\''.$row[0].'\');">'.$row[0].'</a></li>';
  }
    
?>