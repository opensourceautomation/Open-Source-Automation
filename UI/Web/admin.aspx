<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin.aspx.cs" Inherits="admin"  MasterPageFile="~/MasterPage.master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ MasterType virtualpath="~/MasterPage.master" %>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>


<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder">
    <div class="row-fluid">
        <div ID="AdminPanel">
            <table ID="Table1" runat="server">
                <tr>
                    <td colspan="13">
                        <asp:Label ID="Label1" runat="server" Text="Web UI Permission settings" Font-Bold="True"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                         
                    </td>
                </tr>
                <tr>
                    <td style="width:175px; text-align:center;">
                         <asp:Label ID="Label9" runat="server" Text="Screen Name" Font-Bold="True"></asp:Label>
                    </td>
                    <td style="width:75px; text-align:center;">
                         <asp:Label ID="Label10" runat="server" Text="Trust Level" Font-Bold="True"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:right;">
                        <asp:Label ID="Label3" runat="server" Text="Screens Level"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="screensLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:label ID="Lable13" runat="server" Text="Default Screen:"></asp:label>
                    </td>
                    <td colspan="5">
                        <asp:DropDownList ID="mainScreen" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:right;">
                        <asp:Label ID="Label2" runat="server" Text="Objects Level"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="objectsLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label21" runat="server" Text="Add"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="objectsAddLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="objUpdLev_lab" runat="server" Text="Update"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="objectsUpdateLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label23" runat="server" Text="Delete"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="objectsDeleteLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:right;">
                        <asp:Label ID="Label4" runat="server" Text="Analytics Level"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="analyticsLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:center;">
                        <asp:Label ID="Label5" runat="server" Text="Management"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="manageLev" runat="server" Width="25px" Visible="False" Font-Bold="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="text-align:right;">
                        <asp:Label ID="Label11" runat="server" Text="Object Type Level"></asp:Label>
                    </td>
                    <td style="text-align:center; width:75px;">
                        <asp:TextBox ID="objecttypeLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label22" runat="server" Text="Add"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="objecttypeAddLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label24" runat="server" Text="Update"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="objecttypeUpdateLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label25" runat="server" Text="Delete"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="objecttypeDeleteLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="text-align:right;">
                        <asp:Label ID="Label12" runat="server" Text="Scripts Level"></asp:Label>
                    </td>
                    <td style="text-align:center; width:75px;">
                        <asp:TextBox ID="scriptLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label26" runat="server" Text="Add"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="scriptAddLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label27" runat="server" Text="Update"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="scriptUpdateLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label28" runat="server" Text="Delete"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="scriptDeleteLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label29" runat="server" Text="Object Event Script Add"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="scriptObjectEventLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label30" runat="server" Text="Object Type Event Script Add"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="scriptObjectTypeEventLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="text-align:right;">
                        <asp:Label ID="Label13" runat="server" Text="Patterns Level"></asp:Label>
                    </td>
                    <td style="text-align:center; width:75px;">
                        <asp:TextBox ID="patternLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label31" runat="server" Text="Add"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="patternAddLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label32" runat="server" Text="Update"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="patternUpdateLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label33" runat="server" Text="Delete"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="patternDeleteLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="text-align:right;">
                        <asp:Label ID="Label14" runat="server" Text="Reader Level"></asp:Label>
                    </td>
                    <td style="text-align:center; width:75px;">
                        <asp:TextBox ID="readerLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label34" runat="server" Text="Add"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="readerAddLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label35" runat="server" Text="Update"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="readerUpdateLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label36" runat="server" Text="Delete"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="readerDeleteLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="text-align:right;">
                        <asp:Label ID="Label15" runat="server" Text="Schedules Level"></asp:Label>
                    </td>
                    <td style="text-align:center; width:75px;">
                        <asp:TextBox ID="scheduleLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label37" runat="server" Text="Add"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="scheduleAddLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label38" runat="server" Text="Update"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="scheduleUpdateLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label39" runat="server" Text="Delete"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="scheduleDeleteLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="text-align:right;">
                        <asp:Label ID="Label16" runat="server" Text="Images Level"></asp:Label>
                    </td>
                    <td style="text-align:center; width:75px;">
                        <asp:TextBox ID="imageLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label40" runat="server" Text="Add"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="imageAddLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label41" runat="server" Text="Delete"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="imageDeleteLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:center;">
                        <asp:Label ID="Label6" runat="server" Text="Logs"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="logsLev" runat="server" Width="25px" Visible="False"></asp:TextBox>
                    </td>
                    <td style="text-align:right;">
                        <asp:Label ID="Label42" runat="server" Text="Clear Logs"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="logsClearLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="text-align:right;">
                        <asp:Label ID="Label17" runat="server" Text="Event Log Level"></asp:Label>
                    </td>
                    <td style="text-align:center; width:75px;">
                        <asp:TextBox ID="eventlogLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="text-align:right;">
                        <asp:Label ID="Label18" runat="server" Text="Method Log Level"></asp:Label>
                    </td>
                    <td style="text-align:center; width:75px;">
                        <asp:TextBox ID="methodlogLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="text-align:right;">
                        <asp:Label ID="Label19" runat="server" Text="Server Log Level"></asp:Label>
                    </td>
                    <td style="text-align:center; width:75px;">
                        <asp:TextBox ID="serverlogLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="1" style="text-align:right;">
                        <asp:Label ID="Label20" runat="server" Text="Debug Log Level"></asp:Label>
                    </td>
                    <td style="text-align:center; width:75px;">
                        <asp:TextBox ID="debuglogLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:right;">
                        <asp:Label ID="Label7" runat="server" Text="Values Level"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="valuesLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:right;">
                        <asp:Label ID="Label8" runat="server" Text="Config Level"></asp:Label>
                    </td>
                    <td style="text-align:center;">
                        <asp:TextBox ID="configLev" runat="server" Width="25px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="13" style="text-align:right; color:red; width:100%;">
                        <asp:Label ID="saveSuc" runat="server" Text="Admin Settings were saved successfully!" Visible="false"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="modal-footer">
        <div class="row-fluid">
            <asp:Button class="btn btn-primary" runat="server" ID="btnSaveAdmin" Text="Save" OnClick="btnAdminSave_Click"/>
            <asp:Button class="btn btn-primary" runat="server" ID="btnCancelAdmin" Text="Cancel" OnClick="btnAdminCancel_Click"/>
        </div>
    </div>
</asp:Content>