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


        function load(obj, prop) {
            $('#chart').html('');
            $('#loading').show();
            var datasets;

            $.getJSON('http://' + host + ':8732/api/analytics/' + obj + '/' + prop + '?callback=?', null, function (data) {
                $('#loading').hide();

                var options = {
                    xaxis: {
                        mode: "time",
                        timeformat: "%Y/%m/%d %H:%M%p",
                        tickLength: 5
                    },
                    selection: {
                        mode: "x"
                    }
                };

                var plot = $.plot("#chart", data, options);

                var overview = $.plot("#overview", data, {
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
                        min: 0,
                        autoscaleMargin: 0.1
                    },
                    selection: {
                        mode: "x"
                    }
                });

                $("#chart").bind("plotselected", function (event, ranges) {

                    // do the zooming

                    plot = $.plot("#chart", data, $.extend(true, {}, options, {
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


            });
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
                        AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvProperties_RowDataBound" DataKeyNames="prop_name, property_name, object_name" ShowHeaderWhenEmpty="true">  
                        <Columns>  
                            <asp:BoundField DataField="prop_name" HeaderText="Property" /> 
                            <asp:BoundField DataField="object_name" Visible="false" />   
                            <asp:BoundField DataField="property_name" Visible="false" />  
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


