<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="debuglog.aspx.cs" Inherits="debuglog" %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <div class="row-fluid">
        <div class="span1"></div>
        <div class="span10">
            <asp:Button class="btn" runat="server" ID="clearLogButton" Text="Clear Debug Log" OnClick="clearLogButton_Click" ToolTip="Clears the Debug Log." />
            <asp:Button runat="server" ID="btnRefresh" Text="Refresh" class="btn" OnClick="btnRefresh_Click" ToolTip="Refreshes the Debug Log."/>
            <asp:Button runat="server" ID="btnExport" Text="Export" class="btn" OnClick="btnExport_Click" ToolTip="Exports the Debug log to a CVS file." />
            <asp:GridView ID="debugLogGridView" AutoGenerateColumns="false" runat="server" GridLines="None" CssClass="mGrid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Entries Found">
                <Columns>
                    <asp:BoundField DataField="log_time" HeaderText="Time"  ItemStyle-Width="10em" DataFormatString="{0:MM-dd HH:mm:ss.ff}" />
                    <asp:BoundField DataField="entry" HeaderText="Entry" />
                    <asp:BoundField DataField="debug_trace" HeaderText="Debug Trace" />
                </Columns>
            </asp:GridView>
            <asp:Button class="btn" runat="server" ID="clearLogButton2" Text="Clear Debug Log" OnClick="clearLogButton_Click" ToolTip="Clears the Debug Log." />
            <asp:Button runat="server" ID="btnRefresh2" Text="Refresh" class="btn" OnClick="btnRefresh_Click" ToolTip="Refreshes the Debug Log."/>
            <asp:Button runat="server" ID="btnExport2" Text="Export" class="btn" OnClick="btnExport_Click" ToolTip="Exports the Debug log to a CVS file." />
        </div>   
        <div class="span1"></div>           
    </div>
</asp:Content>

