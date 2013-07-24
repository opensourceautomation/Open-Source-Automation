<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="analytics.aspx.cs" Inherits="analytics" %>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    
    <script>
        function pageLoad() {
            $("#<%=datepickerFrom.ClientID%>").datepicker();
            $("#<%=datepickerFrom.ClientID%>").datepicker("option", "dateFormat", "yy-mm-dd");

            $("#<%=datepickerTo.ClientID%>").datepicker();
            $("#<%=datepickerTo.ClientID%>").datepicker("option", "dateFormat", "yy-mm-dd");
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
            var count = 0;

            $('#PropertyGrid tr:has(:checkbox:checked)').each(function () {
                var obj = $(this).find('td').eq(1).html();
                var prop = $(this).find('td').eq(2).html();
                var from = '1900-01-01';
                var to = '2039-01-01';

                if ($("#<%=datepickerFrom.ClientID%>").val() != '')
                    from = $("#<%=datepickerFrom.ClientID%>").val()
                if ($("#<%=datepickerTo.ClientID%>").val() != '')
                    to = $("#<%=datepickerTo.ClientID%>").val()

                $.getJSON('http://' + host + ':8732/api/analytics/' + obj + '/' + prop + '?f=' + from + '&t=' + to + '&callback=?', null, function (data) {
                    datasets.push(data[0]);
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

                $.getJSON('http://' + host + ':8732/api/analytics/state/' + obj + '?f=' + from + '&t=' + to + '&callback=?', null, function (data) {
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
                        selection: {
                            mode: "x"
                        },
                        grid: { markings: markings }
                    };

                    var plot = $.plot("#chart", datasets, options);

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
    </style>
    <div class="row-fluid">
        <div class="span3">
            <div ID="ScriptPanel">
                <div class="row-fluid" ID="PropertyGrid" style="overflow: auto; max-height:350px;" onscroll="SetDivPosition()">
                    <h2>Properties</h2>
                    <asp:GridView runat="server" ID="gvProperties"
                        AutoGenerateColumns="False"  
                        GridLines="None"  
                        CssClass="mGrid" ShowHeader="true" 
                        AlternatingRowStyle-CssClass="alt" DataKeyNames="property_name, object_name" ShowHeaderWhenEmpty="true">  
                        <Columns>  
                            <asp:TemplateField HeaderText="View" Visible="True">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkEnabled" runat="server" onclick="load();"/>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="object_name" Visible="True" HeaderText="Object" />   
                            <asp:BoundField DataField="property_name" Visible="True" HeaderText="Property" /> 
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
                                    <asp:CheckBox ID="chkEnabled" runat="server" onclick="load();"/>
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
                From: <asp:TextBox runat="server" ID="datepickerFrom" style="margin-top:10px;"></asp:TextBox> To: <asp:TextBox runat="server" ID="datepickerTo" style="margin-top:10px;"></asp:TextBox> &nbsp; &nbsp; <img src="Images/refresh.png" onclick="load();" style="margin-top:auto;">
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
</asp:Content>


