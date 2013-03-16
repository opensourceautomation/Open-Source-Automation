<?php 
session_start();
header("Cache-control: private");

$access = "denied";
if(isset($_SESSION["access"]))
{
  $access = $_SESSION["access"];
}
if ($access != "granted")
  echo '<html><head><meta http-equiv="refresh" content="1;url=/login.php"/></head><body></body></html>';
else
{
  require("includes/functions.php");
  require_once('includes/connect.php');
  // open connection to MySQL server
  $connection = mysql_connect($hostname, $username, $password)
  or die ('Unable to connect!');
  // select database for use
  mysql_select_db($database) or die ('Unable to select database!');
  
  if(isset($_GET["oid"]))
    $oid = $_GET["oid"];
  else
    $oid = 0;

?>

<html lang="en">
  <head>
    <meta charset="utf-8">
    <title>Open Source Automation</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="">
    <meta name="author" content="">

    <!-- Le styles -->
    <link href="bootstrap/css/bootstrap.css" rel="stylesheet">
    <style type="text/css">
      body {
        padding-top: 60px;
        padding-bottom: 40px;
      }
      .sidebar-nav {
        padding: 0 0 0 0;
      }
      
      .hero-unit {
        padding: 10px 10px 50px 10px;
      }
      
      .well {
        padding: 20px 10px 20px 20px;
      }
      
      .accordion-heading {
        background-color: whiteSmoke;
      }  
      
      .twrap {
          overflow: hidden;
          padding: 0 4px 0 12px
      }
      textarea {
          width: 98%;
          height: 300px;
      }?
          
    </style>
    <link href="bootstrap/css/bootstrap-responsive.css" rel="stylesheet" type="text/css">
    <script type="text/javascript" src="bootstrap/js/jquery-1.7.2.min.js"></script>
    <script src="bootstrap/js/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="includes/osae.js"></script>
    <script type="text/javascript" src="http://www.google.com/jsapi"></script>
    <script type="text/javascript">
      google.load('visualization', '1', {packages: ['annotatedtimeline']});
      function drawVisualization() {
        
        <?php
        if($oid != 0)
        {
        ?>
        $('#graph').show();
        var data = new google.visualization.DataTable();
        data.addColumn('date', 'Date');
        data.addColumn('number', 'Temp');
        data.addRows([
          <?php 
          
          $query = 'select YEAR( history_timestamp ) AS YEAR, MONTH( history_timestamp )-1 AS MONTH, DAY( history_timestamp ) AS DAY, HOUR( history_timestamp ) AS HOUR, MINUTE( history_timestamp ) AS MINUTE, property_value from osae_v_object_property_history where object_property_id='.$oid.' ORDER BY history_timestamp desc LIMIT 50000';
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
        
        <?php
        }
        ?>
      }
      
      google.setOnLoadCallback(drawVisualization);
    </script>
    <!-- Le HTML5 shim, for IE6-8 support of HTML5 elements -->
    <!--[if lt IE 9]>
      <script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->

    <!-- Le fav and touch icons -->
  </head>
   
  <body>

    <?php 
      require("includes/navbar.php");
    ?>

    <div class="container-fluid">
      <div class="row-fluid">
        <div class="span3">
          <div class="sidebar-nav" id="navbar">
            <div class="accordion" id="navaccordion">
              <?php
                $i = 0;
                $screenquery = 'SELECT object_name FROM osae_v_object_property where track_history = 1 group by object_name';
                $results = mysql_query($screenquery)
                or die ('Error in query: $query. ' . mysql_error());
                
                while($row = mysql_fetch_row($results))
                {
                  echo '<div class="accordion-group">';
                    echo '<div class="accordion-heading">';
                      echo '<a class="accordion-toggle " data-toggle="collapse" data-target="#navdata' . strval($i) . '" data-parent="#navaccordion" href="#navdata' . strval($i) . '">' . $row[0] . '</a>';
                    echo '</div>';
                    echo '<div id="navdata'.strval($i).'" class="accordion-body collapse">';
                      echo '<div class="accordion-inner">';
                        echo '<ul class="nav nav-list" >';
                        $propquery = 'SELECT object_property_id, property_name FROM osae_v_object_property where object_name = \''.$row[0].'\' and track_history = 1 group by property_name';
                        $results2 = mysql_query($propquery)
                        or die ('Error in query: $query. ' . mysql_error());
                        while($row2 = mysql_fetch_row($results2))
                        {
                          echo '<li><a href="analytics.php?oid='.$row2[0].'">'.$row2[1].'</a></li>';
                        }
                        echo '</ul>';
                      echo '</div>';
                    echo '</div>';
                  echo '</div>';
                  $i++;
                }
                
              ?>
            </div>
          </div>
        </div>
        <div class="span9" id="content">
          <div id="graph" class="well" style="padding: 15px 15px 15px 15px; display: none;">
            <div id="visualization" style="width: 100%; height: 400px; "></div>
          </div> 
        </div>
      </div>


      <hr>

      <footer>
        <p>&copy; Open Source Automation 2012</p>
      </footer>

    </div><!--/.fluid-container-->
    <div id="oid"><?php echo $oid;?></div>
    <!-- Le javascript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="bootstrap/js/bootstrap-transition.js"></script>
    <script src="bootstrap/js/bootstrap-collapse.js"></script>
    <script src="bootstrap/js/bootstrap-dropdown.js"></script> 
    <script src="bootstrap/js/bootstrap-tab.js"></script>
  </body>
</html>
<?php
}
?>