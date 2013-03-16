<?php
  require_once('includes/connect.php');
  if(isset($_GET["sid"]))
  { 
  
    $query = 'select property_value from osae_v_object_property where object_name = "'.$_GET["sid"].'"';
    $results = mysql_query($query)
    or die ('Error in query: $query. ' . mysql_error());
    $row = mysql_fetch_row($results);
	
	// Need to go down one level, as images are not in wwwroot folder, and PHP doesnt resolve HttpServer Virtual Folder
    list($width, $height) = getimagesize('../'.$row[0]);
    $offset = ($_GET["w"]/2)-($width/2)-33;
    
    echo '<center><div style="position:relative; z-index:1;">';
    echo '<img src="'.$row[0].'">';
    
    echo '<div>';

  
    $query = 'select object_id from osae_v_object where container_name="'.$_GET["sid"].'" and object_type = "CONTROL STATE IMAGE"';
    $results = mysql_query($query)
    or die ('Error in query: $query. ' . mysql_error());
    while($row = mysql_fetch_row($results))
    {
      $propQuery = 'select property_name, property_value from osae_v_object_property where object_id ='.$row[0];
      $propResults = mysql_query($propQuery)
      or die ('Error in query: $propQuery. ' . mysql_error());
      while($propRow = mysql_fetch_row($propResults))
      {
         switch ($propRow[0]) {
            case 'Object Name':
                $name = $propRow[1];
                break;
            case 'State 1 Name':
                $s1Name = $propRow[1];
                break;
            case 'State 2 Name':
                $s2Name = $propRow[1];
                break;
            case 'State 3 Name':
                $s3Name = $propRow[1];
                break;
            case 'State 1 Image':
                $s1Img = $propRow[1];
                break;
            case 'State 2 Image':
                $s2Img = $propRow[1];
                break;
            case 'State 3 Image':
                $s3Img = $propRow[1];
                break;
            case 'State 1 X':
                $s1X = $propRow[1]+5;
                break;
            case 'State 2 X':
                $s2X = $propRow[1]+5;
                break;
            case 'State 3 X':
                $s3X = $propRow[1]+5;
                break;
            case 'State 1 Y':
                $s1Y = $propRow[1];
                break;
            case 'State 2 Y':
                $s2Y = $propRow[1];
                break;
            case 'State 3 Y':
                $s3Y = $propRow[1];
                break;
            case 'ZOrder':
                $zOrder = $propRow[1];
                break;
        }
      }
      
      $objQuery = 'select state_name from osae_v_object where object_name = "'.$name.'"';
      $objResults = mysql_query($objQuery)
      or die ('Error in query: $objQuery. ' . mysql_error());
      $objRow = mysql_fetch_row($objResults);

      if($objRow[0] == $s1Name)
      {
        echo '<div id="'.$row[0].'" data-type="state" data-state="'.$objRow[0].'" class="item" style="position:absolute; top:'.$s1Y.'; left:'.($s1X+$offset).'; z-index:'.$zOrder.';">';
        echo '<a href="#" onclick="runMethod(\''.$name.'\',';
        if($objRow[0] == 'ON')
          echo '\'OFF\'';
        else
          echo '\'ON\'';
            
        echo ',\'\',\'\')"><img name="'.$name.'" src="'.$s1Img.'"></a></div>';
      }
      if($objRow[0] == $s2Name)
      {
        echo '<div id="'.$row[0].'" data-type="state" data-state="'.$objRow[0].'" class="item" style="position:absolute; top:'.$s2Y.'; left:'.($s2X+$offset).'; z-index:'.$zOrder.';">';
        echo '<a href="#" onclick="runMethod(\''.$name.'\',';
        if($objRow[0] == 'ON')
          echo '\'OFF\'';
        else
          echo '\'ON\'';
        echo ',\'\',\'\')"><img name="'.$name.'" src="'.$s2Img.'"></a></div>';
      }
      if($objRow[0] == $s3Name)
      {
        echo '<div id="'.$row[0].'" data-type="state" data-state="'.$objRow[0].'" class="item" style="position:absolute; top:'.$s3Y.'; left:'.($s3X+$offset).'; z-index:'.$zOrder.';">';
        echo '<a href="#" onclick="runMethod(\''.$name.'\',';
        if($objRow[0] == 'ON')
          echo '\'OFF\'';
        else
          echo '\'ON\'';
        echo ',\'\',\'\')"><img name="'.$name.'" src="'.$s3Img.'"></a></div>';
      }
      
    }
    
    // PROPERTY LABELS
    $query = 'select object_id from osae_v_object where container_name="'.$_GET["sid"].'" and object_type = "CONTROL PROPERTY LABEL"';
    $results = mysql_query($query)
    or die ('Error in query: $query. ' . mysql_error());
    while($row = mysql_fetch_row($results))
    {
      $propQuery = 'select property_name, property_value from osae_v_object_property where object_id ='.$row[0];
      $propResults = mysql_query($propQuery)
      or die ('Error in query: $propQuery. ' . mysql_error());
      $fontsize=''; $Prefix=''; $Suffix='';
      while($propRow = mysql_fetch_row($propResults))
      {
         switch ($propRow[0]) {
            case 'Object Name':
                $name = $propRow[1];
                break;
            case 'Property Name':
                $propName = $propRow[1];
                break;
            case 'X':
                $x = $propRow[1] + 5;
                break;
            case 'Y':
                $y = $propRow[1];
                break;
            case 'ZOrder':
                $zOrder = $propRow[1];
                break;
            case 'Prefix':
                $Prefix = $propRow[1];
                break;
            case 'Suffix':
                $Suffix = $propRow[1];
                break;
           case 'Font Size':
                $fontsize = 'font-size: '.$propRow[1].'px; ';
                break;
        }
      }
      
      $objQuery = 'select property_value from osae_v_object_property where object_name = "' . $name . '" and property_name = "' . $propName . '"';
      $objResults = mysql_query($objQuery)
      or die ('Error in query: $objQuery. ' . mysql_error());
      $objRow = mysql_fetch_row($objResults);

      echo '<div id="'.$row[0].'" data-type="label" class="item" style="'.$fontsize.'position:absolute; top:'.$y.'; left:'.($x+$offset).'; z-index:'.$zOrder.';">';
      echo '<b>'.$Prefix . $objRow[0] . $Suffix .'</b>'. '</div>';
      
    }
    
    // CONTROL METHOD IMAGE
    $query = 'select object_id from osae_v_object where container_name="'.$_GET["sid"].'" and object_type = "CONTROL METHOD IMAGE"';
    $results = mysql_query($query)
    or die ('Error in query: $query. ' . mysql_error());
    while($row = mysql_fetch_row($results))
    {
      $propQuery = 'select property_name, property_value from osae_v_object_property where object_id ='.$row[0];
      $propResults = mysql_query($propQuery)
      or die ('Error in query: $propQuery. ' . mysql_error());
      while($propRow = mysql_fetch_row($propResults))
      {
         switch ($propRow[0]) {
            case 'Object Name':
                $name = $propRow[1];
                break;
            case 'Method Name':
                $methodName = $propRow[1];
                break;
            case 'X':
                $x = $propRow[1] + 5;
                break;
            case 'Y':
                $y = $propRow[1];
                break;
            case 'ZOrder':
                $zOrder = $propRow[1];
                break;
            case 'Param 1':
                $p1 = $propRow[1];
                break;
            case 'Param 2':
                $p2 = $propRow[1];
                break;
            case 'Image':
                $img = $propRow[1];
                break;
        }
      }
      
      echo '<div id="'.$row[0].'" data-type="method" class="item" style="position:absolute; top:'.$y.'; left:'.($x+$offset).'; z-index:'.$zOrder.';">';
      echo '<a href="#" onclick="runMethod(\''.$name.'\',\''.$methodName.'\',\''.$p1.'\',\''.$p2.'\'';
      echo ')"><img name="'.$name.'" src="'.$img.'"></a></div>';
      
    }

  }
  
  echo '</div>';
  echo '</div></center>';                
?>
