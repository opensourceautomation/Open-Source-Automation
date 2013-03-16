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

?>

<html lang="en">
  <head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <title>Open Source Automation</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="">
    <meta name="author" content="">

    <!-- Le styles -->
    <link href="bootstrap/css/bootstrap.css" rel="stylesheet" type="text/css">
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
    <script type="text/javascript" src="includes/osae.js"></script>
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
            
              $places = json_decode(file_get_contents('http://localhost:8732/api/objects/type/place'));
              $count = count($places);  
              for($i=0;$i<$count;$i++)
              {
                echo '<div class="accordion-group">';
                  echo '<div class="accordion-heading">';
                    echo '<a class="accordion-toggle " data-toggle="collapse" data-target="#navdata' . strval($i) . '" data-parent="#navaccordion" href="#navdata' . strval($i) . '">' . $places[$i]->{'Name'} . '</a>';
                  echo '</div>';
                  echo '<div id="navdata'.strval($i).'" class="accordion-body collapse">';
                    echo '<div class="accordion-inner">';
                      echo '<ul class="nav nav-list" >';
                $objs = json_decode(file_get_contents('http://localhost:8732/api/objects/container/' . rawurlencode($places[$i]->{'Name'})));
                $objscount = count($objs);  
                for($j=0;$j<$objscount;$j++)
                {
                        echo '<li><a href="#" onclick="openObject(\''.$objs[$j]->{'Name'}.'\');" >'.$objs[$j]->{'Name'}.'</a></li>';
                }
                      echo '</ul>';
                    echo '</div>';
                  echo '</div>';
                echo '</div>';
              }
              
              $containers = json_decode(file_get_contents('http://localhost:8732/api/objects/type/container'));
              $count = count($containers);
                
              for($l=0;$l<$count;$l++)
              {
                echo '<div class="accordion-group">';
                  echo '<div class="accordion-heading">';
                    echo '<a class="accordion-toggle " data-toggle="collapse" data-target="#navdata' . strval($i) . '" data-parent="#navaccordion" href="#navdata' . strval($i) . '">' . $containers[$l]->{'Name'} . '</a>';
                  echo '</div>';
                  echo '<div id="navdata'.strval($i).'" class="accordion-body collapse">';
                    echo '<div class="accordion-inner">';
                      echo '<ul class="nav nav-list" >';
                $objs = json_decode(file_get_contents('http://localhost:8732/api/objects/container/' . rawurlencode($containers[$l]->{'Name'})));
                $objscount = count($objs);  
                for($k=0;$k<$objscount;$k++)
                {
                        echo '<li><a href="#" onclick="openObject(\''.$objs[$k]->{'Name'}.'\');" >'.$objs[$k]->{'Name'}.'</a></li>';
                }
                      echo '</ul>';
                    echo '</div>';
                  echo '</div>';
                echo '</div>';
                $i++;
              }
              
            ?>
            </div>
          </div><!--/.well -->
        </div><!--/span-->
        <div class="span9" id="content">
        </div><!--/span-->
      </div><!--/row-->

      <hr>

      <footer>
        <p>&copy; Open Source Automation 2012</p>
      </footer>

    </div><!--/.fluid-container-->

    <!-- Le javascript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script type="text/javascript" src="bootstrap/js/bootstrap-transition.js"></script>
    <script type="text/javascript" src="bootstrap/js/bootstrap-collapse.js"></script>
    <script type="text/javascript" src="bootstrap/js/bootstrap-dropdown.js"></script>
  </body>
</html>
<?php
}
?>