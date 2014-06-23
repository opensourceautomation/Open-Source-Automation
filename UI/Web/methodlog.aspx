<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="methodlog.aspx.cs" Inherits="methodlog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <div class="row-fluid">
        <div class="span1"></div>
        <div class="span10">
            <asp:GridView ID="methodLogGridView" AutoGenerateColumns="false" runat="server" GridLines="None" CssClass="mGrid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Entries Found">
                <Columns>
                    <asp:BoundField DataField="entry_time" HeaderText="Time" ItemStyle-Width="10em" />
                    <asp:BoundField DataField="object_name" HeaderText="Object" />
                    <asp:BoundField DataField="method_name" HeaderText="Method" />
                    <asp:BoundField DataField="parameter_1" HeaderText="Parameter 1" />
                    <asp:BoundField DataField="parameter_2" HeaderText="Parameter 2" />
                    <asp:BoundField DataField="from_object" HeaderText="From" />
                </Columns>
            </asp:GridView>
            <asp:Button class="btn" runat="server" ID="clearLogButton" Text="Clear Method Log" OnClick="clearLogButton_Click" />
        </div>   
        <div class="span1"></div>           
    </div>
</asp:Content>

