<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctrlUserControl.ascx.cs" Inherits="controls_ctrlUserControl" className="ctrlUserControl"%>
<asp:Table ID="Table1" runat="server" Height="219px" Width="400px" BorderStyle="Solid" BorderWidth="2px">
    <asp:TableRow runat="server">
        <asp:TableCell runat="server">
            <asp:Label ID="lblCurTemp" runat="server" Text="lblCur°" Font-Bold="True" Font-Size="Large"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" ColumnSpan="5">
            <asp:Label ID="lblConditions" runat="server" Text="Conditions"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow runat="server">
        <asp:TableCell runat="server">
            
        </asp:TableCell>
        <asp:TableCell runat="server"  Height="64" Width="64">
            <asp:Label ID="lblDay1" runat="server" Text="Day 1" Font-Bold="True" Font-Size="Small"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server"  Height="64" Width="64">
            <asp:Label ID="lblDay2" runat="server" Text="Day 2" Font-Bold="True" Font-Size="Small"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server"  Height="64" Width="64">
            <asp:Label ID="lblDay3" runat="server" Text="Day 3" Font-Bold="True" Font-Size="Small"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server"  Height="64" Width="64">
            <asp:Label ID="lblDay4" runat="server" Text="Day 4" Font-Bold="True" Font-Size="Small"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server"  Height="64" Width="64">
             <asp:Label ID="lblDay5" runat="server" Text="Day 5" Font-Bold="True" Font-Size="Small"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow runat="server">
        <asp:TableCell runat="server" Height="83" Width="83" >
            <asp:Image ID="imgTodayDay" runat="server" Height="83" Width="83" />
        </asp:TableCell>
        <asp:TableCell runat="server"  Height="64" Width="64">
            <asp:Image ID="imgDay1Day" runat="server" Height="64" Width="64" />
        </asp:TableCell>
        <asp:TableCell runat="server"  Height="64" Width="64">
            <asp:Image ID="imgDay2Day" runat="server" Height="64" Width="64" />
        </asp:TableCell>
        <asp:TableCell runat="server"  Height="64" Width="64">
            <asp:Image ID="imgDay3Day" runat="server" Height="64" Width="64" />
        </asp:TableCell>
        <asp:TableCell runat="server"  Height="64" Width="64">
            <asp:Image ID="imgDay4Day" runat="server" Height="64" Width="64" />
        </asp:TableCell>
        <asp:TableCell runat="server"  Height="64" Width="64">
            <asp:Image ID="imgDay5Day" runat="server" Height="64" Width="64" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow runat="server">
        <asp:TableCell runat="server" Height="20" Width="64">
             <asp:Label ID="lblTodayHi" runat="server" Text="Hi: xx°" Font-Bold="False" ForeColor="#FF3300"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblDay1Hi" runat="server" Text="Hi: xx°" Font-Bold="False" ForeColor="#FF3300"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblDay2Hi" runat="server" Text="Hi: xx°" Font-Bold="False" ForeColor="#FF3300"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblDay3Hi" runat="server" Text="Hi: xx°" Font-Bold="False" ForeColor="#FF3300"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblDay4Hi" runat="server" Text="Hi: xx°" Font-Bold="False" ForeColor="#FF3300"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblDay5Hi" runat="server" Text="Hi: xx°" Font-Bold="False" ForeColor="#FF3300"></asp:Label>                        
        </asp:TableCell>
    </asp:TableRow>        
    <asp:TableRow runat="server">
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblTodayLo" runat="server" Text="Low: xx°" Font-Bold="False" ForeColor="Black"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblDay1Lo" runat="server" Text="Low: xx°" Font-Bold="False" ForeColor="Black"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblDay2Lo" runat="server" Text="Low: xx°" Font-Bold="False" ForeColor="Black"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblDay3Lo" runat="server" Text="Low: xx°" Font-Bold="False" ForeColor="Black"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblDay4Lo" runat="server" Text="Low: xx°" Font-Bold="False" ForeColor="Black"></asp:Label>
        </asp:TableCell>
        <asp:TableCell runat="server" Height="20" Width="64">
            <asp:Label ID="lblDay5Lo" runat="server" Text="Low: xx°" Font-Bold="False" ForeColor="Black"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow runat="server">
        <asp:TableCell runat="server" Height="64" Width="64">
            <asp:Image ID="imgTodayNight" runat="server" Height="83" Width="83" />
        </asp:TableCell>
        <asp:TableCell runat="server" Height="64" Width="64">
            <asp:Image ID="imgDay1Night" runat="server" Height="64" Width="64" />
        </asp:TableCell>
        <asp:TableCell runat="server" Height="64" Width="64">
            <asp:Image ID="imgDay2Night" runat="server" Height="64" Width="64" />
        </asp:TableCell>
        <asp:TableCell runat="server" Height="64" Width="64">
            <asp:Image ID="imgDay3Night" runat="server" Height="64" Width="64" />
        </asp:TableCell>
        <asp:TableCell runat="server" Height="64" Width="64">
            <asp:Image ID="imgDay4Night" runat="server" Height="64" Width="64" />
        </asp:TableCell>
        <asp:TableCell runat="server" Height="64" Width="64">
            <asp:Image ID="imgDay5Night" runat="server" Height="64" Width="64" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow runat="server">
        <asp:TableCell runat="server" ColumnSpan="6" Height="20">
            <asp:Label ID="lblLastUpd" runat="server" Text="Label"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:HiddenField ID="hdnCurState" runat="server" />
<asp:HiddenField ID="hdnObjName" runat="server" />

