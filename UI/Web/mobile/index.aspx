<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="mobile_index"  MasterPageFile="MobileMasterPage.master"%>


<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder">
  <style type=text/css>
    input.ui-slider-input {
        display : none !important;
    }
    
    .ui-grid-a .ui-block-a { width: 80% }
    .ui-grid-a .ui-block-b { width: 20%; }
    .ui-content {
        overflow-x: visible;
    }

    .fliper { position:absolute; right:8px; top:-4px; }


    div.ui-slider-switch {
        width: 7.1em;
    }

    </style>
  
  <script type="text/javascript">
      var host = '';

      $(document).ready(function () {
          host = window.location.hostname;
          load();
      });



      function load() {
          $('#homeBtn').addClass("ui-btn-active");
          $('#statusBtn').removeClass("ui-btn-active");
          $('#propsBtn').removeClass("ui-btn-active");
          $('#callback').html('');

          $.getJSON('http://' + host + ':8732/api/objects/type/place?callback=?', null, function (data) {
              //$('#callback').html(data);
              $("#callback").append('<ul id="places" data-role="listview" data-theme="g">');
              $.each(data, function (i, obj) {
                  if (obj.State.Value.toUpperCase() == 'ON') {
                      $("#places").append('<li><a href="#" onclick="openPlace(\'' + obj.Name + '\');" >' + obj.Name + '</a><span class="fliper"><select name="' + obj.Name + '" id="flip-a" data-role="slider"><option value="off">Vacant</option><option value="on" selected>Occupied</option></select></span></li>');
                  }
                  else {
                      $("#places").append('<li><a href="#" onclick="openPlace(\'' + obj.Name + '\');" >' + obj.Name + '</a><span class="fliper"><select name="' + obj.Name + '" id="flip-a" data-role="slider"><option value="off" selected>Vacant</option><option value="on">Occupied</option></select></span></li>');
                  }
              });
              $('#places').listview();
              $('select').slider();

              $('select').bind("change", function (event, ui) {
                  if (event.target.value == 'on') {
                      runMethod(event.target.name, 'ON', '', '');
                  }
                  else {
                      runMethod(event.target.name, 'OFF', '', '');
                  }
              });
          });

          $('#header').html('<center><img src="images/osa_logo.png" /></center><br />');
      }

      function openPlace(place) {
          $('#callback').html('');
          $.getJSON('http://' + host + ':8732/api/objects/container/' + place + '?callback=?', null, function (data) {

              $("#callback").append('<ul id="places" data-role="listview" data-theme="g">');
              $.each(data, function (i, obj) {
                  if (obj.Name != place) {
                      if (obj.State.Value.toUpperCase() == 'ON') {
                          $("#places").append('<li><a href="#" onclick="openObject(\'' + place + '\',\'' + obj.Name + '\');" >' + obj.Name + '</a><span class="fliper"><select name="' + obj.Name + '" id="flip-a" data-role="slider"><option value="off">Off</option><option value="on" selected>On</option></select></span></li>');
                      }
                      else if(obj.State.Value.toUpperCase() == 'OFF'){
                          $("#places").append('<li><a href="#" onclick="openObject(\'' + place + '\',\'' + obj.Name + '\');" >' + obj.Name + '</a><span class="fliper"><select name="' + obj.Name + '" id="flip-a" data-role="slider"><option value="off" selected>Off</option><option value="on">On</option></select></span></li>');
                      }
                      else{
                          $("#places").append('<li><a href="#" onclick="openObject(\'' + place + '\',\'' + obj.Name + '\');" >' + obj.Name + '</a></li>');
                      }

                  }
              });
              $('#places').listview();
              $('select').slider();

              $('select').bind("change", function (event, ui) {
                  if (event.target.value == 'on') {
                      runMethod(event.target.name, 'ON', '', '');
                  }
                  else {
                      runMethod(event.target.name, 'OFF', '', '');
                  }
              });

          });
          $('#header').html('<h1 style="white-space: normal" class="ui-title" role="heading" aria-level="1" >' + place + '</h1><a href="index.php" onClick="window.location.reload()" id="backBtn" class="ui-btn-left" data-icon="arrow-l">Back</a>');
          $('#header').attr('data-position', 'inline');
          $('#backBtn').button();
          
      }

      function openObject(place, object) {
          $('#callback').html('');
          $.getJSON('http://' + host + ':8732/api/object/' + object + '?callback=?', null, function (data) {


              if (data.BaseType == 'BINARY SWITCH' || data.BaseType == 'MULTILEVEL SWITCH') {

                  if (data.State.Value.toUpperCase() == 'ON') {
                      $("#callback").append('<center>Power State<br /><select name="slider" id="flip-a" data-role="slider"><option value="off">Off</option><option value="on" selected>On</option></select></center>');
                  }
                  else {
                      $("#callback").append('<center>Power State<br /><select name="slider" id="flip-a" data-role="slider"><option value="off">Off</option><option value="on">On</option></select></center>');
                  }
                  var lvl = 0;
                  if (data.BaseType == 'MULTILEVEL SWITCH') {
                      $.each(data.Properties, function (i, prop) {
                          if (prop.Name == 'Level') {
                              lvl = prop.Value;
                          }
                      });
                      $("#callback").append('<br /><center><label for="slider-0">Brightness</label><input type="range" name="slider" id="slider" value="' + lvl + '" min="0" max="100" /></center>');

                  }

                  $('select').bind("change", function (event, ui) {
                      if (event.target.value == 'on') {
                          runMethod(object, 'ON', '', '');
                      }
                      else {
                          runMethod(object, 'OFF', '', '');
                      }
                  });

                  $('#slider').bind("change", function (event, ui) {
                      if (event.target.value == 0) {
                          runMethod(object, 'OFF', '', '');
                          $('#flip-a').val('off').slider('refresh');
                      }
                      else {
                          runMethod(object, 'ON', event.target.value, '');
                          $('#flip-a').val('on').slider('refresh');
                      }
                  });

                  $('select').slider();
                  $('#slider').slider();

                  //$("#callback").append('<center><br /><br /><h2>Methods</h2></center>');
                  //$.each(data.Methods,function(i,meth){
                  //   if(meth != 'ON' && meth != 'OFF')
                  //   $("#callback").append('<a id="btn'+i+'"href="#" data-role="button" onclick="runMethod(\''+object+'\',\''+meth+'\');" >'+meth+'</a>');
                  //   $('#btn'+i).button();
                  //});
              }
              else if (data.BaseType == 'THERMOSTAT') {
                  var coolSet, heatSet, curTemp, fanState, mode;
                  $.each(data.Properties, function (i, prop) {
                      if (prop.Name == 'Cool Setpoint') {
                          coolSet = prop.Value;
                      }
                      else if (prop.Name == 'Heat Setpoint') {
                          heatSet = prop.Value;
                      }
                      else if (prop.Name == 'Fan Mode') {
                          fanState = prop.Value;
                      }
                      else if (prop.Name == 'Temperature') {
                          curTemp = prop.Value;
                      }
                      else if (prop.Name == 'Operating Mode') {
                          mode = prop.Value;
                      }
                  });
                  $('#callback').html('');
                  $("#callback").append('<div class="ui-grid-b"><div id="coolSet" class="ui-block-a"></div><div id="temp" class="ui-block-b"></div><div id="heatSet" class="ui-block-c"></div></div><br /><div class="ui-grid-a"><div id="opState" class="ui-block-a"></div><div id="fanState" class="ui-block-b"></div></div><hr><br /><div data-role="fieldcontain"><label for="select-choice-1" class="select">Cool:</label><select name="select-choice-1" id="selectCool" onchange="setHVAC(\'' + object + '\',\'COOLSP\', this.value);"><option value="64">64</option><option value="65">65</option><option value="66">66</option><option value="67">67</option><option value="68">68</option><option value="69">69</option><option value="70">70</option><option value="71">71</option><option value="72">72</option><option value="73">73</option><option value="74">74</option><option value="75">75</option><option value="76">76</option><option value="77">77</option><option value="78">78</option><option value="79">79</option></select><label for="select-choice-1" class="select">Heat:</label><select name="select-choice-1" id="selectHeat" onchange="setHVAC(\'' + object + '\',\'HEATSP\', this.value);"><option value="64">64</option><option value="65">65</option><option value="66">66</option><option value="67">67</option><option value="68">68</option><option value="69">69</option><option value="70">70</option><option value="71">71</option><option value="72">72</option><option value="73">73</option><option value="74">74</option><option value="75">75</option><option value="76">76</option><option value="77">77</option><option value="78">78</option><option value="79">79</option></select></div><br /><center><a href="#" id="heatBtn" data-role="button" data-inline="true" data-mini="true" onclick="runMethod(\'' + object + '\',\'HEAT\', \'\',\'\');">Heat</a><a href="#" id="coolBtn" data-role="button" data-inline="true" data-mini="true" onclick="runMethod(\'' + object + '\',\'COOL\', \'\',\'\');">Cool</a><a href="#" id="autoBtn" data-role="button" data-inline="true" data-mini="true" onclick="runMethod(\'' + object + '\',\'AUTO\', \'\',\'\');">Auto</a><a href="#" id="offBtn" data-role="button" data-inline="true" data-mini="true" onclick="runMethod(\'' + object + '\',\'OFF\', \'\',\'\');">Off</a></center>');

                  $("#coolSet").append('<center>Cool Set<br /><font size="6">' + coolSet + '</font></center>');
                  $("#temp").append('<center>Temp<br /><font size="10">' + curTemp + '</font></center>');
                  $("#heatSet").append('<center>Heat Set<br /><font size="6">' + heatSet + '</font></center>');

                  if (mode == 'Off' || mode == 'Idle') {
                      $("#opState").append('<strong><center><label style="color: grey">Cool</label> <label style="color: grey">Heat</label> <label style="color: lightblue">Off</label></center></strong>');
                  }
                  else if (mode == 'Heat') {
                      $("#opState").append('<strong><center><label style="color: grey">Cool</label> <label style="color: ltblue">Heat</label> <label style="color: grey">Off</label></center></strong>');
                  }
                  else if (mode == 'Cool') {
                      $("#opState").append('<strong><center><label style="color: blue">Cool</label> <label style="color: grey">Heat</label> <label style="color: grey">Off</label></center></strong>');
                  }


                  if (fanState == 'Off') {
                      $("#fanState").append('<strong><center><label style="color: grey">Fan</label></center></strong>');
                  }
                  else {
                      $("#fanState").append('<strong><center><label style="color: lightblue">Fan</label></center></strong>');
                  }

                  $('#selectCool').selectmenu();
                  $('#selectHeat').selectmenu();

                  $('#selectCool').val(coolSet);
                  $('#selectHeat').val(heatSet);
                  $('#selectCool').selectmenu("refresh");
                  $('#selectHeat').selectmenu("refresh");

                  $('#heatBtn').buttonMarkup();
                  $('#coolBtn').buttonMarkup();
                  $('#autoBtn').buttonMarkup();
                  $('#offBtn').buttonMarkup();

                  jQuery('.ui-select, .ui-select a').css({ 'width': '50%' });
              }
              else {
                  $("#callback").append('<center><br /><br /><h2>Methods</h2></center>');
                  $.each(data.Methods, function (i, meth) {
                      $("#callback").append('<a id="btn' + i + '"href="#" data-role="button" onclick="runMethod(\'' + object + '\',\'' + meth.MethodName + '\', \'\',\'\');" >' + meth.MethodLabel + '</a>');
                      $('#btn' + i).button();
                  });
              }

              $('#header').html('<h1 style="white-space: normal" class="ui-title" role="heading" aria-level="1" >' + object + '</h1><a href="#" onclick="openPlace(\'' + place + '\');" id="backBtn" class="ui-btn-left" data-icon="arrow-l">Back</a>');
              $('#header').attr('data-position', 'inline');
              $('#backBtn').button();

          });
      }

      function setHVAC(obj, method, value) {
          if (method == 'HEATSP') {
              $("#heatSet").html('<center>Heat Set<br /><font size="6">' + value + '</font></center>');
          }
          else if (method == 'COOLSP') {
              $("#coolSet").html('<center>Cool Set<br /><font size="6">' + value + '</font></center>');
          }
          runMethod(obj, method, value, '');
      }

      function loadStates() {
          $('#homeBtn').removeClass("ui-btn-active");
          $('#statusBtn').addClass("ui-btn-active");
          $('#propsBtn').removeClass("ui-btn-active");
          $('#callback').html('');

          $.getJSON('http://' + host + ':8732/api/system/states?callback=?', null, function (data) {
              $("#callback").append('<div id="states" data-role="controlgroup">');
              $.each(data, function (i, obj) {
                  $("#states").append('<a href="#" id="btn_' + obj + '" onclick="changeState(\'' + obj + '\');" data-role="button">' + obj + '</a></li>');
                  $('#btn_' + obj.toUpperCase()).button();
              });
          });

          $.getJSON('http://' + host + ':8732/api/object/system?callback=?', null, function (data) {
              $('#btn_' + data.State.Value.toUpperCase()).attr("data-theme", "b").removeClass("ui-btn-up-a").addClass("ui-btn-up-b");
          });

          $('#header').html('<center><img src="images/osa_logo.png" /></center><br />');
      }

      function changeState(state) {
          runMethod('SYSTEM', state, '', '');
          setTimeout(function () {
              loadStates();
          }, 200);

      }

      function runMethod(object, method, p1, p2) {
          if (p1 != '') {
              $.post('http://' + host + ':8732/api/object/' + object + '/' + method + '?param1=' + p1 + '&callback=?', null, function (data) {
                  return data;
              });
          }
          else if (p2 != '') {
              $.post('http://' + host + ':8732/api/object/' + object + '/' + method + '?param1=' + p1 + '&param2=' + p2 + '&callback=?', null, function (data) {
                  return data;
              });
          }
          else {
              $.post('http://' + host + ':8732/api/object/' + object + '/' + method + '?callback=?', null, function (data) {
                  return data;
              });
          }

      }

      function loadProperties() {
          $('#homeBtn').removeClass("ui-btn-active");
          $('#statusBtn').removeClass("ui-btn-active");
          $('#propsBtn').addClass("ui-btn-active");
          $('#callback').html('');

          $.getJSON('http://' + host + ':8732/api/object/propertylist/Custom Property List/Values?callback=?', null, function (data) {
              $("#callback").append('<div id="props" class="ui-grid-a"></div>');
              $.each(data, function (i, obj) {
                  var array = obj.split(':=');
                  $.getJSON('http://' + host + ':8732/api/object/' + array[0] + '?callback=?', null, function (data2) {
                      var val = '';
                      var objName = array[1];
                      if (objName.toLowerCase() == 'state') {
                          val = data2.State.Value;
                      }
                      else {

                          $.each(data2.Properties, function (i, prop) {
                              if (prop.Name.toLowerCase() == objName.toLowerCase()) {
                                  val = prop.Value;
                              }
                          });
                      }
                      $("#props").append('<div class="ui-block-a" ><strong>' + array[0] + ' - ' + array[1] + '</strong></div><div class="ui-block-b" ><strong>' + val + '</strong></div><br /><br />');
                  });

              });
          });

          $.getJSON('http://' + host + ':8732/api/object/system?callback=?', null, function (data) {
              $('#btn_' + data.State.Value.toUpperCase()).attr("data-theme", "b").removeClass("ui-btn-up-a").addClass("ui-btn-up-b");
          });

          $('#header').html('<center><img src="images/osa_logo.png" /></center><br />');
      }

  </script>    
           
    <div data-role="page" data-theme="a" id="jqm-home">        
      <div id="header" data-role="header"> 
      	<center><img src="images/osa_logo.png" /></center><br />
      </div>
      
      <div id="callback" data-role="content"> 
      </div>
                           
      <div data-role="footer" data-position="fixed">                             
        <div data-role="navbar">                                   
          <ul>                                         
            <li>                
            <a href="#" id="homeBtn" onClick="load();" class="ui-btn-active" data-iconpos="top" data-icon="home">Home</a>                
            </li>                                         
            <li>                
            <a href="#" id="statusBtn" onClick="loadStates();" data-iconpos="top" data-icon="gear">System</a>                
            </li>                                         
            <li>                
            <a href="#" id="propsBtn" onClick="loadProperties();" data-iconpos="top" data-icon="info">Status</a>                
            </li>                                
          </ul>                             
        </div>                           
      </div> 
    </div>
</asp:Content>