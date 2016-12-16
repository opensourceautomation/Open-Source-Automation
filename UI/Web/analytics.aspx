<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="analytics.aspx.cs" Inherits="analytics" %>
<%@ MasterType virtualpath="~/MasterPage.master" %>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    
    <script>
        function pageLoad() {
            $("#<%=datepickerFrom.ClientID%>").datepicker();
            $("#<%=datepickerFrom.ClientID%>").datepicker("option", "dateFormat", "yy-mm-dd");

            $("#<%=datepickerTo.ClientID%>").datepicker();
            $("#<%=datepickerTo.ClientID%>").datepicker("option", "dateFormat", "yy-mm-dd");

            $('#<%=timepickerTo.ClientID%>').timepicker();
            $('#<%=timepickerFrom.ClientID%>').timepicker();
        }

        $(function () {
            $("#<%=datepickerFrom.ClientID%>").change(function () {
                if($('#PropertyGrid tr:has(:checkbox:checked)').length + $('#StateGrid tr:has(:checkbox:checked)').length > 0)
                    load();
            });
        });

        $(function () {
            $("#<%=datepickerTo.ClientID%>").change(function () {
                if ($('#PropertyGrid tr:has(:checkbox:checked)').length + $('#StateGrid tr:has(:checkbox:checked)').length > 0)
                    load();
            });
        });

        $(function () {
            $("#<%=timepickerFrom.ClientID%>").change(function () {
                if ($('#PropertyGrid tr:has(:checkbox:checked)').length + $('#StateGrid tr:has(:checkbox:checked)').length > 0)
                    load();
            });
        });

        $(function () {
            $("#<%=timepickerTo.ClientID%>").change(function () {
                        if ($('#PropertyGrid tr:has(:checkbox:checked)').length + $('#StateGrid tr:has(:checkbox:checked)').length > 0)
                            load();
            });
         });


        $(document).ready(function () {
            $('#loading').hide();
            host = window.location.hostname;
        });

        window.onload = function () {
            var strCook = document.cookie;
            if (strCook.indexOf("!~") != 0) {
                var intS = strCook.indexOf("!~");
                var intE = strCook.indexOf("~!");
                var strPos = strCook.substring(intS + 2, intE);
                document.getElementById("PropertyGrid").scrollTop = strPos;
            }
        }

        function SetDivPosition() {
            var intY = document.getElementById("PropertyGrid").scrollTop;
            document.cookie = "yPos=!~" + intY + "~!";
        }

        function load() {
            $('#chart').html('');
            $('#loading').show();
            var datasets = [];
            var markings = [];
            var labels = [];
            var points = [];
            var size = $('#PropertyGrid tr:has(:checkbox:checked)').length + $('#StateGrid tr:has(:checkbox:checked)').length;
            //  for only first check box use '#PropertyGrid tr:has([ID$=chkEnabled]:checkbox:checked)').length + $('#StateGrid tr:has(:checkbox:checked)'
            var count = 0;

            $('#PropertyGrid tr:has(:checkbox:checked)').each(function () {
                var obj = $(this).find('td').eq(1).html();
                var prop = $(this).find('td').eq(2).html();
                var ptype = $(this).find('td').eq(3).html();
                var stepscheck = $(this).find('td').eq(4).find(':checkbox').is(':checked');
                
                var steps = false;
                if (stepscheck){
                    steps = true;
                }
                var yaxis = 1;
                if (ptype == 'B'){
                    steps = true;
                    yaxis = 2;
					}
                else
                {
                    yaxis = 1;
                }

                var from = '1900-01-01';
                var to = '2039-01-01';

                var currentdate = new Date();
                var fromtime = ""; //currentdate.getHours() + ':' + currentdate.getMinutes() + ':' + currentdate.getSeconds();
                var totime ="";

                if ($("#<%=datepickerFrom.ClientID%>").val() != '')
                    from = $("#<%=datepickerFrom.ClientID%>").val()
                if ($("#<%=datepickerTo.ClientID%>").val() != '')
                    to = $("#<%=datepickerTo.ClientID%>").val()
                    
                if ($("#<%=timepickerFrom.ClientID%>").val() != '')
                    fromtime = 'T' + $("#<%=timepickerFrom.ClientID%>").val()
                if ($("#<%=timepickerTo.ClientID%>").val() != '')
                    totime = 'T' + $("#<%=timepickerTo.ClientID%>").val()
      
                $.getJSON('http://' + host + ':<%= hdnRestPort.Value %>/api/analytics/' + obj + '/' + prop + '?f=' + from + fromtime + '&t=' + to + totime + '&callback=?', null, function (data) {
                    $.each(data, function (i, returndata) {
                        datasets.push({"data": returndata.data, "label": returndata.label, "yaxis": yaxis, "lines":{"steps": steps}});
					});
					count++;
				});
            });

            $('#StateGrid tr:has(:checkbox:checked)').each(function () {
                var obj = $(this).find('td').eq(1).html();
                var from = '1900-01-01';
                var to = '2039-01-01';

                if ($("#<%=datepickerFrom.ClientID%>").val() != '')
                    from = $("#<%=datepickerFrom.ClientID%>").val()
                if ($("#<%=datepickerTo.ClientID%>").val() != '')
                    to = $("#<%=datepickerTo.ClientID%>").val()

                $.getJSON('http://' + host + ':<%= hdnRestPort.Value %>/api/analytics/state/' + obj + '?f=' + from + '&t=' + to + '&callback=?', null, function (data) {
                    $.each(data, function (i, obj) {
                        markings.push({ color: "#000", lineWidth: 1, xaxis: { from: + obj.datetime , to:  + obj.datetime } });
                        labels.push(obj.obj + ' - ' + obj.state);
                        points.push(obj.datetime);
                    });

                    count++;
                });
            });


            var videoInterval = setInterval(function () {
                if (count == size) {
                    $('#loading').hide();
                    var options = {
                        xaxis: {
                            mode: "time",
                            tickLength: 5
                        },
                        yaxes: [{
                        },{
                            position: "right",
                            ticks: [[0, "Off"], [1, "On"], [1.2, ""]]
                        }
                        ],
                        selection: {
                            mode: "x"
                        },

                        grid: {
                            markings: markings,
                            hoverable: true,
                            //clickable: true
                        },

                        legend: {
                          //show: boolean,
                          //labelFormatter: null or (fn: string, series object -> string)
                          //labelBoxBorderColor: color
                          //noColumns: number
                          position: "nw"
                          //margin: number of pixels or [x margin, y margin]
                          //backgroundColor: null or color
                          //backgroundOpacity: number between 0 and 1
                          //container: null or jQuery object/DOM element/jQuery expression
                          }
                    };

                    var plot = $.plot("#chart", datasets, options);

                    function showTooltip(x, y, contents) {
                        $('<div id="tooltip">' + contents + '</div>').css({
                            position: 'absolute',
                            display: 'none',
                            top: y + 5,
                            left: x + 5,
                            border: '1px solid #fdd',
                            padding: '2px',
                            'background-color': '#fee',
                            opacity: 0.80
                        }).appendTo("body").fadeIn(200);
                    }

                    var previousPoint = null;
                    $("#chart").bind("plothover", function (event, pos, item) {
                        $("#x").text(pos.x.toFixed(2));
                        $("#y").text(pos.y.toFixed(2));

                        if (item) {
                                if (previousPoint != item.dataIndex) {
                                    previousPoint = item.dataIndex;

                                    $("#tooltip").remove();
                                    var x = item.datapoint[0],
                                        y = item.datapoint[1];

                                    var dUTC = new Date(x);
                                    var d = new Date(+dUTC + dUTC.getTimezoneOffset() * 60000);
                                    var day = d.getDate();
                                    var ordinal = (day > 10 && day < 20 ? 'th' : { 1: 'st', 2: 'nd', 3: 'rd' }[day % 10] || 'th')

                                    // var DATE_FORMAT = "ddd dS H:MM:ss";
                                    // showTooltip(item.pageX, item.pageY, $.plot.formatDate(d, DATE_FORMAT) + ":" + y);

                                    showTooltip(item.pageX, item.pageY,
                                                item.series.label + " = " + y + " @ " + d.format("ddd d") + ordinal + d.format(" H:mm:ss"));
                                }
                        }
                        else {
                                $("#tooltip").remove();
                                previousPoint = null;
                        }
                    });

                    var overview = $.plot("#overview", datasets, {
                        series: {
                            lines: {
                                show: true,
                                lineWidth: 1
                            },
                            shadowSize: 0
                        },
                        xaxis: {
                            ticks: [],
                            mode: "time"
                        },
                        yaxis: {
                            ticks: [],
                            autoscaleMargin: 0.1
                        },
                        selection: {
                            mode: "x"
                        }
                    });

                    for (var i = 0; i < points.length; i++) {
                        var o = plot.pointOffset({ x: points[i]});
                        $("#chart").append("<div style='position:absolute;left:" + (o.left + 4) + "px;top:" + o.top + "px;color:#666;font-size:smaller'>" + labels[i] + "</div>");
                    }

                    $("#chart").bind("plotselected", function (event, ranges) {

                        // do the zooming

                        plot = $.plot("#chart", datasets, $.extend(true, {}, options, {
                            xaxis: {
                                min: ranges.xaxis.from,
                                max: ranges.xaxis.to
                            }
                        }));

                        // don't fire event on the overview to prevent eternal loop

                        overview.setSelection(ranges, true);
                    });

                    $("#overview").bind("plotselected", function (event, ranges) {
                        plot.setSelection(ranges);
                    });

                    clearInterval(videoInterval);
                }

            }, 500);
        }

    </script>
    <style type="text/css">
        .visualization
        {
            float: left;
            width: 100%;
            height: 300px;
            position: relative;
            overflow: hidden;
        }

         /* css for timepicker */
        .ui-timepicker-div .ui-widget-header { margin-bottom: 8px; }
        .ui-timepicker-div dl { text-align: left; }
        .ui-timepicker-div dl dt { height: 25px; margin-bottom: -25px; }
        .ui-timepicker-div dl dd { margin: 0 10px 10px 65px; }
        .ui-timepicker-div td { font-size: 90%; }
        .ui-tpicker-grid-label { background: none; border: none; margin: 0; padding: 0; }

        .ui-timepicker-rtl{ direction: rtl; }
        .ui-timepicker-rtl dl { text-align: right; }
        .ui-timepicker-rtl dl dd { margin: 0 65px 10px 10px; }  

    </style>
    <div class="row-fluid">
        <div class="span3">
            <div ID="ScriptPanel">
                <div class="row-fluid" ID="PropertyGrid" style="overflow: auto; max-height:350px;" onscroll="SetDivPosition()">
                    <h2>Properties</h2>
                    <asp:GridView runat="server" ID="gvProperties"
                        AutoGenerateColumns="False"  
                        GridLines="None"  
                        CssClass="mGrid"
                        ShowHeader="true" 
                        AlternatingRowStyle-CssClass="alt" DataKeyNames="property_name, object_name" ShowHeaderWhenEmpty="true">  
                        <Columns>  
                            <asp:TemplateField HeaderText="View" Visible="True">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkEnabled" runat="server" onclick="load();" title="Select to view this Property in the Analytics chart."/>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="object_name" Visible="True" HeaderText="Object" />   
                            <asp:BoundField DataField="property_name" Visible="True" HeaderText="Property" /> 
                            <asp:BoundField DataField="property_datatype" Visible="True" HeaderText="Type" ItemStyle-HorizontalAlign="Center" />  
                            <asp:TemplateField HeaderText="Step" Visible="True">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkStep" runat="server" onclick="load();" title="Select to view Step."/>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                <div class="row-fluid" ID="StateGrid" style="overflow: auto; max-height:350px;">
                    <h2>States</h2>
                    <asp:GridView runat="server" ID="gvStates"
                        AutoGenerateColumns="False"  
                        GridLines="None"  
                        CssClass="mGrid" ShowHeader="true" 
                        AlternatingRowStyle-CssClass="alt" DataKeyNames="object_name" ShowHeaderWhenEmpty="true">  
                        <Columns>  
                            <asp:TemplateField HeaderText="View" Visible="True">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkEnabled" runat="server" onclick="load();" title="Select to view this State in the Analytics chart."/>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="object_name" Visible="True" HeaderText="Object" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
        <div class="span9">
            <div class="row-fluid">
                From: <asp:TextBox runat="server" ID="datepickerFrom" style="margin-top:10px;" title="Enter the Start Date to display"></asp:TextBox>
                To: <asp:TextBox runat="server" ID="datepickerTo" style="margin-top:10px;" title="Enter the Stop Date to display"></asp:TextBox>
                <br>
                Time: <asp:TextBox runat="server" ID="timepickerFrom" style="margin-top:10px;" title="Enter the Start Time to display"></asp:TextBox>
                &nbsp; &nbsp; &nbsp; <asp:TextBox runat="server" ID="timepickerTo" style="margin-top:10px;" title="Enter the Stop Time to display"></asp:TextBox>

                &nbsp; &nbsp; <img src="Images/refresh.png" onclick="load();" style="margin-top:auto;">
            </div>
            <div class="row-fluid">
                <img ID="loading" src="Images/loading.GIF" style="display:block; max-height:100px; margin-left:auto; margin-right:auto;"/>
                <div ID="chart" class="span12" style="height: 300px;width:95%;margin-left:auto; margin-right:auto;">
                    
                </div>
                <br />
                <br />
                <div ID="overview" class="span12" style="height: 150px;width:95%;margin-left:auto; margin-right:auto;">
                    
                </div>
            </div>
        </div>
    </div>
    <asp:Label runat="server" ID="hdnSelectedRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropID" Visible="false"></asp:Label>
    <asp:HiddenField runat="server" ID="hdnRestPort"/>
</asp:Content>