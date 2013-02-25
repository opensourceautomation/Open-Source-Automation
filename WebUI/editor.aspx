<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="editor.aspx.cs" Inherits="editor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <table>
        <tr>
            <td>
                <asp:Label ID="scriptProcessorTypeLabel" AssociatedControlID="scriptProcessorTypeDropDownList" runat="server" Text="Script Processor Type:" />
            </td>
            <td>
                <asp:DropDownList ID="scriptProcessorTypeDropDownList" AppendDataBoundItems="true" runat="server">
                    <asp:ListItem>All</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:Label ID="scriptTypeLabel" AssociatedControlID="" runat="server" Text="Script Type:" />
            </td>
            <td>
                <asp:DropDownList ID="scriptsTypeDropDownList" runat="server">
                    <asp:ListItem>Named Script</asp:ListItem>
                    <asp:ListItem>Event Script</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>

