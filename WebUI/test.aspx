<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test.aspx.cs" Inherits="test" %>

<%@ Register Assembly="GoogleVisualizationAPI" Namespace="GoogleVisualizationAPI"
    TagPrefix="gvapi" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Google Visualization API Test</title>
    <gvapi:GoogleVisualization ID="GVAPI" runat="server" />
    <style type="text/css">
        .visualization
        {
            float: left;
            width: 220px;
            height: 200px;
            position: relative;
            overflow: hidden;
        }
    </style>
</head>
<body>
    <gvapi:GoogleContainer ID="visLineChart" ClassName="visualization" runat="server" />
    <gvapi:GoogleContainer ID="visPieChart" ClassName="visualization" runat="server" />
    <gvapi:GoogleContainer ID="visOrgChart" ClassName="visualization" runat="server" />
    <gvapi:GoogleContainer ID="visAnnotatedTimeLine" ClassName="visualization" runat="server" />
</body>
</html>
