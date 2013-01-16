<?php
include('connect.php');
/* get the incoming ID and password hash */
$user = $_POST["userid"];
$pass = $_POST["password"];

/* SQL statement to query the database */
$query = "SELECT property_value FROM osae_v_object_property WHERE object_name='Web Server' AND property_name='Username'";
$result = mysql_query($query);
$row = mysql_fetch_row($result);
$u = $row[0];

$query = "SELECT property_value FROM osae_v_object_property WHERE object_name='Web Server' AND property_name='Password'";
$result = mysql_query($query);
$row = mysql_fetch_row($result);
$p = $row[0];

/* Allow access if a matching record was found, else deny access. */
if ($u==$user && $p==$pass)
{
  session_start();
  header("Cache-control: private");
  $_SESSION["access"] = "granted";
  echo '<html><head><meta http-equiv="refresh" content="1;url=/home.php"/></head><body></body></html>';
}
else
{
  echo '<html><head><meta http-equiv="refresh" content="1;url=/login.php?attempt=f"/></head><body></body></html>';
}


?>