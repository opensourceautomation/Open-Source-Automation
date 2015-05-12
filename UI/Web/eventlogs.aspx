<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="eventlogs.aspx.cs" Inherits="eventlogs" %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <div class="row-fluid">
        <div class="span1"></div>
        <div class="span10">
            <asp:GridView ID="eventLogGridView" AutoGenerateColumns="False" runat="server" GridLines="None" CssClass="mGrid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Entries Found">
<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                <Columns>
                    <asp:BoundField DataField="log_time" HeaderText="Time" ItemStyle-Width="10em" DataFormatString="{0:MM-dd HH:mm:ss.ff}" />
                    <asp:BoundField DataField="object_name" HeaderText="Object Name" />
                    <asp:BoundField DataField="event_label" HeaderText="Event" />
                    <asp:BoundField DataField="parameter_1" HeaderText="Parameter 1" />
                    <asp:BoundField DataField="parameter_2" HeaderText="Parameter 2" />
                    <asp:BoundField DataField="from_object_name" HeaderText="From" />
                </Columns>
            </asp:GridView>
            <asp:Button class="btn" runat="server" ID="clearLogButton" Text="Clear Event Log" OnClick="clearLogButton_Click" />
        </div>   
        <div class="span1"></div>           
    </div>
</asp:Content>

