
<!DOCTYPE html>
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
        padding: 9px 0;
      }
    </style>
    <link href="bootstrap/css/bootstrap-responsive.css" rel="stylesheet">
    <script src="bootstrap/js/jquery-1.7.2.min.js"></script>
    <!-- Le HTML5 shim, for IE6-8 support of HTML5 elements -->
    <!--[if lt IE 9]>
      <script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->

    <!-- Le fav and touch icons -->
    <link rel="shortcut icon" href="../assets/ico/favicon.ico">
    <link rel="apple-touch-icon-precomposed" sizes="144x144" href="../assets/ico/apple-touch-icon-144-precomposed.png">
    <link rel="apple-touch-icon-precomposed" sizes="114x114" href="../assets/ico/apple-touch-icon-114-precomposed.png">
    <link rel="apple-touch-icon-precomposed" sizes="72x72" href="../assets/ico/apple-touch-icon-72-precomposed.png">
    <link rel="apple-touch-icon-precomposed" href="../assets/ico/apple-touch-icon-57-precomposed.png">
  </head>
  
  <script type="text/javascript">
    var host = '';
    
    $(document).ready(function() {
      $host = window.location.hostname;
      
    });
  </script>
  
  <body>

    <div class="navbar navbar-fixed-top">
      <div class="navbar-inner">
        <div class="container-fluid">
          <a class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse">
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
          </a>
          <a class="brand" href="#">Open Source Automation</a>
          <div class="nav-collapse">
            <ul class="nav">
              <li class="active"><a href="#">Screens</a></li>
              <li><a href="places.php">Places</a></li>
              <li><a href="#contact">Objects</a></li>
            </ul>
          </div><!--/.nav-collapse -->
        </div>
      </div>
    </div>

    <div class="container-fluid">
      <div class="row-fluid">
        <div class="span3">
          <div class="well sidebar-nav" id="navbar">
            <div class="accordion" id="navaccordion">
            <?php
            
              $places = json_decode(file_get_contents('http://'.$_SERVER['SERVER_NAME'].':8732/api/objects/type/plugin'));
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
                $objs = json_decode(file_get_contents('http://'.$_SERVER['SERVER_NAME'].':8732/api/objects/container/' . rawurlencode($places[$i]->{'Name'})));
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
              
            ?>
            </div>
          </div><!--/.well -->
        </div><!--/span-->
        <div class="span9">
          <div class="hero-unit">
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          <br />
          </div>
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
    <script src="bootstrap/js/bootstrap-transition.js"></script>
    <script src="bootstrap/js/bootstrap-collapse.js"></script>

  </body>
</html>
