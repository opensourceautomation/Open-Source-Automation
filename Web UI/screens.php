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
  
  <script type="text/javascript">
    var editMode = false;
    var timeoutId;
    var curscreen;
    
    $(window).load(function() {
      curscreen = $('#sid').html();
      <?php echo 'openScreen ();';
      ?>
    });
    
    function openScreen (screen) {
        if(screen > '') {
          $('#screenTabs a[href="#'+screen+'"]').tab('show');
          curscreen = screen;
        }
        $.post('getScreen.php?sid='+curscreen+'&w='+window.innerWidth, '', ajaxCallback);

    }
    
    var ajaxCallback = function (data) {
        var container = $('#refresh');
        var curHTML = container.html();
        if(container.html() != data) {
          container.html(data);
        }
        //var items = $('.item', container);
        
        if (!editMode) {
            timeoutId = setTimeout(openScreen, 2000);
        }
    };
    
  </script>
  
  <body>

    <?php 
      require("includes/navbar.php");
    ?>

    <div class="container-fluid">
      <div class="row-fluid">
        
          <?php
          
            $screens = json_decode(file_get_contents('http://'.$_SERVER['SERVER_NAME'].':8732/api/objects/type/screen'));
            $count = count($screens);  
            echo '<ul class="nav nav-tabs" id="screenTabs">';
            for($i=0;$i<$count;$i++)
            {
              echo '<li';
              if($i == 0)
              {
                echo ' class="active"';
                $sid = $screens[$i]->{'Name'};
              }
              echo '><a href="#" data-toggle="tab"  id="'.$screens[$i]->{'Name'}.'" onclick="openScreen(\''.$screens[$i]->{'Name'}.'\');" >'.$screens[$i]->{'Name'}.'</a></li>';
            }
            echo '</ul>';
          ?>
          <div class="span12" >

              <div id="refresh">
              
              </div>
    
          </div><!--/.well -->
        </div><!--/span-->
      </div><!--/row-->

      <hr>

      <footer>
        <p>&copy; Open Source Automation 2012</p>
      </footer>

    </div><!--/.fluid-container-->
    <div id="sid"><?php echo $sid;?></div>
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