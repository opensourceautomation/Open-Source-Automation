<?php

$iphone = strpos($_SERVER['HTTP_USER_AGENT'],"iPhone");
$android = strpos($_SERVER['HTTP_USER_AGENT'],"Android");
$palmpre = strpos($_SERVER['HTTP_USER_AGENT'],"webOS");
$berry = strpos($_SERVER['HTTP_USER_AGENT'],"BlackBerry");
$ipod = strpos($_SERVER['HTTP_USER_AGENT'],"iPod");

if ($iphone || $android || $palmpre || $ipod || $berry == true) 
{ 
  $host=(isset($_SERVER['HTTP_HOST'])) ? $_SERVER['HTTP_HOST'] : exec("hostname");
  $port = '8081';
  $obj = json_decode(file_get_contents('http://localhost:8732/api/object/Web%20Server'));
  $props = $obj->{'Properties'};
  $count = count($props);
  for($i = 0;$i < $count; $i++)
  {
    if($obj->{'Properties'}[$i]->{'Name'} == 'Port')
      $port = $obj->{'Properties'}[$i]->{'Value'};
  }   



  //header($host . '/mobile/index.html');
  echo "<script>window.location='http://".$host . ":" . $port . "/mobile/index.php'</script>";
}
?>