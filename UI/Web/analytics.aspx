<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="analytics.aspx.cs" Inherits="analytics" %>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    
    <script>
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
            var size = $('#PropertyGrid tr:has(:checkbox:checked)').length;
            var count = 0;

            $('#PropertyGrid tr:has(:checkbox:checked)').each(function () {
                var obj = $(this).find('td').eq(1).html();
                var prop = $(this).find('td').eq(2).html();

                $.getJSON('http://' + host + ':8732/api/analytics/' + obj + '/' + prop + '?callback=?', null, function (data) {
                    datasets.push(data[0]);
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
                        }
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
            </div>
        </div>
        <div class="span9">
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


