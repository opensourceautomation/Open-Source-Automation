<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="eventlogs.aspx.cs" Inherits="eventlogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <div class="row-fluid">
        <div class="span1"></div>
        <div class="span10">
            <asp:GridView ID="eventLogGridView" AutoGenerateColumns="false" runat="server" GridLines="None" CssClass="mGrid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Entries Found">
                <Columns>
                    <asp:BoundField DataField="log_time" HeaderText="Time" ItemStyle-Width="10em" />
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

