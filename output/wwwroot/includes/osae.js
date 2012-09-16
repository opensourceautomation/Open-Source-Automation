    var host = '';
    var selectedMethod = '';
    var selectedEvent = '';
    var objName;
    
    $(document).ready(function() {
      host = window.location.hostname;
      
    });
    
    function openObject(object) {
      objName = object;
      selectedMethod = '';
      $('#content').html('');
      $.getJSON('http://' + host + ':8732/api/object/' + object + '?callback=?', null, function (data) {
          $('#content').append('<h2>'+data.Name+' (' + data.State + ')</h2><br /><div class="row-fluid"><div class="span7" id="objmain"><div id="methodDiv"><h3>Methods</h3><br /><form class="well" id="methods"></form></div><div id="scriptsDiv"><h3>Scripts</h3><br /><form class="well" id="scripts"></form></div></div><div class="span5"><div id="propDiv"><h3>Properties</h3><br /><form class="well form-horizontal" id="properties" style="padding: 20px 10px 40px 20px"></form></div></div></div>')
          
          $('#propDiv').hide();
          $.each(data.Properties,function(i,prop){
            $('#properties').append('<div class="control-group"><label class="control-label span3" for="input'+i+'" id="proplbl'+i+'">'+prop.Name+'</label><div class="controls span8" id="propcontrols'+i+'"></div><i id="updateicon'+i+'"class="icon-exclamation-sign" style="display:none;"></i></div>');
            if(prop.DataType == 'Boolean')
            {
              $('#propcontrols'+i).append('<select id="ddlBool'+i+'" onchange="propChanged('+i+');"><option>True</option><option>False</option></select>');
              $('#ddlBool'+i).val( prop.Value.charAt(0).toUpperCase() + prop.Value.substr(1).toLowerCase() ).attr('selected',true); 
            }
            else
            {
              $('#propcontrols'+i).append('<input class="span12" type="text" id="input'+i+'" value="'+prop.Value+'" onkeydown="propChanged('+i+');">');  
            }
            $('#propDiv').show();
          });
          $('#properties').append('<div class="alert alert-error" id="propSaveError" style="display:none;">Save Failed!</div><div class="alert alert-success" id="propSaveSuccess" style="display:none;">Success!</div><button id="btnPropSave" class="btn pull-right" onclick="saveProperties();" style="display:none;">Save changes</button>');
          
          
          $('#methods').append('<div class="btn-toolbar"><div class="btn-group"><button class="btn dropdown-toggle btn-large"  data-toggle="dropdown" id="methbtn">Methods <span class="caret"></span></button><ul class="dropdown-menu" id="ddlmethods"></ul></div></div>');
          $('#methodDiv').hide();
          $.each(data.Methods,function(i,meth){
            $('#ddlmethods').append('<li><a href="#" onclick="methodSelected(\''+object+'\',\''+meth+'\');">'+meth+'</a></li>');
            $('#methodDiv').show();
          });
          $('#methods').append('<div id="parameters" style="display:none;"><label class="control-label" for="param1">Param 1</label><input type="text" class="span6" id="param1" /><label class="control-label" for="param2">Param 2</label><input type="text" class="span6" id="param2" /><br /><button class="btn btn-primary" id="runmethbtn">Run Method</button></div>');
          
          $('#scripts').append('<div class="btn-toolbar"><div class="btn-group"><button class="btn dropdown-toggle btn-large"  data-toggle="dropdown" id="eventbtn">Events <span class="caret"></span></button><ul class="dropdown-menu" id="ddlevents"></ul></div><div id="scriptchng" style="display:none;" class="pull-right"><i id="updateicon" class="icon-exclamation-sign"></i></div><div id="scriptloader" style="display:none;" class="pull-right"><img src="images/loader.gif" /></div></div><div class="twrap"><textarea id="script" style="display:none;" onkeydown="scriptChanged();"></textarea></div><button id="btnScriptTest" class="btn pull-left" onclick="testScript();" style="display:none;">Test Script</button><button id="btnScriptSave" class="btn pull-right" onclick="saveScript();" style="display:none;">Save changes</button><br /><br /><div class="alert alert-error" id="scriptSaveError" style="display:none;">Save Failed!</div><div class="alert alert-success" id="scriptSaveSuccess" style="display:none;">Success!</div>');
          $.post('getEvents.php?obj='+object, '', eventDDLCallback);
      });
    }    
    
    var eventDDLCallback = function (data) {
        var container = $('#ddlevents');
        var curHTML = container.html();
        if(container.html() != data) {
          container.append(data);
        }

    };
    
    function propChanged(i){
      $('#btnPropSave').show();
      $('#updateicon'+i).show();
      $('#propSaveSuccess').hide();
      $('#propSaveError').hide();
    }
    
    function scriptChanged(){
      $('#btnScriptSave').show();
      $('#scriptchng').show();
      $('#scriptSaveSuccess').hide();
      $('#scriptSaveError').hide();
    }
    
    function methodSelected(obj,meth){

        $('#methbtn').html(meth + ' <span class="caret"></span>');
        var js = 'runMethodForm(\''+obj+'\',\''+meth+'\');';
        var newclick = new Function(js);
        $("#runmethbtn").attr('onclick', '').click(newclick);
        $('#parameters').show();
    
    }
    
    function runMethodForm(object, method) {
      if($('#param1').val() != '' && $('#param2').val() != '') {
        $.getJSON('http://' + host + ':8732/api/object/' + object + '/' + method  + '?param1='+$('#param1').val()+'&param2='+$('#param2').val()+'&callback=?', null, function (data) {
          return data;
        });
      }
      else if($('#param1').val() != '') {
        $.getJSON('http://' + host + ':8732/api/object/' + object + '/' + method  + '?param1='+$('#param1').val()+'&callback=?', null, function (data) {
          return data;
        });
      }
      else {
        $.getJSON('http://' + host + ':8732/api/object/' + object + '/' + method  +'?callback=?', null, function (data) {
          return data;
        });
      }
    
    }
    
    function runMethod(object, method, p1, p2) {
      if(p1 != '' && p2 != '') {
        $.getJSON('http://' + host + ':8732/api/object/' + object + '/' + method  + '?param1='+p1+'&param2='+p2+'&callback=?', null, function (data) {
          return data;
        });
      }
      else if(p1 != '') {
        $.getJSON('http://' + host + ':8732/api/object/' + object + '/' + method  + '?param1='+p1+'&&callback=?', null, function (data) {
          return data;
        });
      }
      else {
        $.getJSON('http://' + host + ':8732/api/object/' + object + '/' + method  +'?callback=?', null, function (data) {
          return data;
        });
      }
    
    }
    
    function saveProperties() {
      var success = '';
      $('.icon-exclamation-sign').each(function(index) {
          index = index - 1;
          if($(this).is(":visible")) {
            var uri = 'http://' + host + ':8732/api/property/update?objName='+objName+'&propName='+$('#proplbl' + index).html()+'&propVal=';
            if ($('#input' + index).length > 0)
            {
              uri = uri + $('#input' + index).val();
            }
            else if($('#ddlBool' + index).length > 0)
            {
              uri = uri + $('#ddlBool' + index + ' option:selected').val();
            }
            $.getJSON(uri+'&callback=?', null, function (data) {
              success = data;    
              if(success){
                $('#propSaveSuccess').show();
              }
              else {
                $('#propSaveError').show();
              }
            });
            $(this).hide(); 
          }
      });  
      $('#btnPropSave').hide();
    }
    
    function eventSelected(obj, event, label) {
      $('#scriptloader').show();
      $('#script').show();
      $('#btnScriptTest').show();
      $('#script').val('');
      $.post('getScript.php?obj=' + encodeURIComponent(obj) + '&event=' + encodeURIComponent(event), '', eventScriptCallback); 
      $('#eventbtn').html(label + ' <span class="caret"></span>');
      selectedEvent = event 
    }
    
    var eventScriptCallback = function (data) {
        $('#script').val(data);
        $('#scriptloader').hide();
        $('#scriptchng').hide();
        $('#btnScriptSave').hide();
    };
    
    function saveScript() {
      var success = '';
      $.getJSON('http://' + host + ':8732/api/script/update?obj='+objName+'&event='+selectedEvent+'&script='+encodeURI($('#script').val())+'&callback=?', null, function (data) {
        success = data;    
        if(success){
          $('#scriptSaveSuccess').show();
        }
        else {
          $('#scriptSaveError').show();
        }
      });
      $('#scriptchng').hide();     
      $('#btnScriptSave').hide();
    }
    
    function testScript() {
      var success = '';
      $.getJSON('http://' + host + ':8732/api/namedscript/update?name=Editor&oldName=Editor&script='+encodeURI($('#script').val())+'&callback=?', null, function (data) {
        success = data;    
        if(success){
           $.getJSON('http://' + host + ':8732/api/namedscript/Editor?callback=?', null, function (data) {
              success = data;    
              if(success){
                
              }
              else {
                
              }
            });
        }
      });
    }