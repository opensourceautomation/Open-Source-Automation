<?php
$fail = false;
if(isset($_GET["attempt"]))
  $fail = true;
?>
<html lang="en">                 
  <head>                             
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />                             
    <title>Open Source Automation                         
    </title>                             
    <meta name="viewport" content="width=device-width, initial-scale=1.0">                             
    <meta name="description" content="">                             
    <meta name="author" content="">                             
    <link href="bootstrap/css/bootstrap.css" rel="stylesheet" type="text/css">                             
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
    <div class="container-fluid">                           
      <div class="row-fluid">                                       
        <div class="span4" >                               
        </div>                 
        <div class="span4" style="margin-left: 0;">                       
          <img src="images/OSA.png">                   
        </div>                   
        <div class="span4">                               
        </div>      
      </div>      
      <div class="row-fluid">                    
        <div class="span4">                               
        </div>                                     
        <div class="span4 well" style="margin: 20px 0 0 0;padding-top: 50px;">                                             
          <div class="span3">          
          </div>                     
          <div class="span6">          
            <form action="includes/validate.php" method="post">                                                                                                                                              
              <input type="hidden" name="next" value="/">                                                                                                                                              
              <fieldset>                                                                                                                                                                              
                <input name="userid" id="userid" type="text" placeholder="Username" class="input-medium span12" >                                                                       
                <input name="password" id="password" type="password" placeholder="Password" class="input-medium span12">                                                                                     
                <div class="actions">                                                                                                                                                                                      
                  <input tabindex="3" class="btn primary large" type="submit" value="Sign In">                                                                           
                </div>                                                                                                                                              
              </fieldset>                                                                                                                          
            </form>          
          </div>          
          <div class="span3">          
          </div>          
<?php
          if($fail)
          {
            echo '<div class="alert alert-error span12" id="loginError" style="margin-left: 0;">Login Failed!</div>';          
          }
                    ?>                                
        </div>                          
        <div class="span4" style="margin-left: 0;">                               
        </div>                              
      </div>                   
    </div>              
  </body>