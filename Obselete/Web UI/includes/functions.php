<?php
  include('user_agent.php');
  
  function curPageName() {
   return substr($_SERVER["SCRIPT_NAME"],strrpos($_SERVER["SCRIPT_NAME"],"/"));
  }
?>