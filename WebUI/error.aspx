<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="error.aspx.cs" Inherits="error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    <h3>OOPS! there was an error that wasn't expected. The details of the error are:</h3>
    <asp:TextBox ID="errorDetailTextBox" runat="server" ReadOnly="false" TextMode="MultiLine" Width="90em" Height="20em" />
</asp:Content>

