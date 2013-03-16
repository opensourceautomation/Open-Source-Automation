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
      .well {
        padding: 20px 10px 50px 20px;
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
    <link href="bootstrap/css/bootstrap-responsive.css" rel="stylesheet">
    <script src="bootstrap/js/jquery-1.7.2.min.js"></script>
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
        <div class="span3" >
          <div class="well" style="padding: 8px 0;">
            <ul class="nav nav-list">
              <?php
                $plugins = json_decode(file_get_contents('http://localhost:8732/api/plugins'));
                $count = count($plugins);  
                
                for($i=0;$i<$count;$i++)
                {
                  if($plugins[$i]->{'State'} == 'Stopped')
                    $state = 'important';
                  else
                    $state = 'success';
                  echo '<li>';
                  echo '<a href="#" onclick="openObject(\''.$plugins[$i]->{'Name'}.'\');"><span class="label label-'.$state.'">'.$plugins[$i]->{'State'}.'</span> &nbsp;'.$plugins[$i]->{'Name'}.'</a>';
                  echo '</li>';
                }
                
              ?>
            </ul>
          </div>
        </div>
        <div class="span9" id="content">
        </div>
      </div>


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
    <script src="bootstrap/js/bootstrap-dropdown.js"></script> 
    <script src="bootstrap/js/bootstrap-tab.js"></script>
  </body>
</html>
<?php
}
?>