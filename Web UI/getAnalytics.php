<?php
  require_once('includes/connect.php');
  if(isset($_GET["oid"]))
  { 
  
   ?>
     <script type="text/javascript">
      google.load('visualization', '1', {packages: ['annotatedtimeline']});
      function drawVisualization() {
        var data = new google.visualization.DataTable();
        data.addColumn('date', 'Date');
        data.addColumn('number', 'Temp');
        data.addRows([
          <?php 
          $query = 'select YEAR( history_timestamp ) AS YEAR, MONTH( history_timestamp ) AS MONTH, DAY( history_timestamp ) AS DAY, HOUR( history_timestamp ) AS HOUR, MINUTE( history_timestamp ) AS MINUTE, property_value from osae_v_object_property_history where object_property_id='.$oid.' ORDER BY history_timestamp desc LIMIT 1000';
          $results = mysql_query($query)
          or die ('Error in query: $query. ' . mysql_error());
          
          while($row = mysql_fetch_row($results))
          {
          echo '[new Date('.$row[0].', '.$row[1].', '.$row[2].', '.$row[3].', '.$row[4].'), '.$row[5].'], ';
          }
          ?>
        ]);
      
        var annotatedtimeline = new google.visualization.AnnotatedTimeLine(
            document.getElementById('visualization'));
        annotatedtimeline.draw(data, {'displayAnnotations': true});
      }
      
      google.setOnLoadCallback(drawVisualization);
      
    </script>
   
   <?php
      
   
  }
  
  echo '<div id="visualization" style="width: 100%; height: 400px;"></div>';
                
?>
