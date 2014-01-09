<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="valuedisplay.aspx.cs" Inherits="valuedisplay" %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <div class="row-fluid">
        <div class="span1"></div>
			<div class="span10">
				<asp:GridView ID="valueDisplayGridView" AutoGenerateColumns="false" runat="server" GridLines="None" CssClass="mGrid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Entries Found">
					<Columns>
						<asp:BoundField DataField="Item" HeaderText="Item" />
						<asp:BoundField DataField="Value" HeaderText="Value" />
					</Columns>
				</asp:GridView>
			</div>   
        <div class="span1"></div>           
    </div>
</asp:Content>

