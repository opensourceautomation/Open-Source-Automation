<%@ Page Language="C#" AutoEventWireup="true" CodeFile="config.aspx.cs" Inherits="config" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder">

    <div class="row-fluid">
        <div class="span2">
        </div>
        <div class="span8">
            <center><img src="Images/OSA.png" /></center>
            <br />
            <br />
            <br />
            <center>
                <table  style="text-align:left; padding-bottom:4px">

                    <tr>
                        <td style="padding-right: 20px">
                            <b>Current Version:</b>
                        </td>
                        <td>
                            <asp:Label ID="lblVersion" runat="server"></asp:Label>
                            <asp:HyperLink ID="hypUpgrade" runat="server" NavigateUrl="http://www.opensourceautomation.com/downloads.php">
                                <asp:Image ID="Image1" runat="server" ImageUrl="Images/upgrade.png" ToolTip="New version available!" />
                            </asp:HyperLink>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Debug Mode: </b>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlDebug" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlDebug_SelectedIndexChanged">
                        <asp:ListItem Text="True" Value="True"></asp:ListItem>
                        <asp:ListItem Text="False" Value="False"></asp:ListItem>
                    </asp:DropDownList>
                        </td>
                    </tr>
                     <tr>
                        <td>
                           <b> Service:</b>
                        </td>
                        <td>
                            <asp:Label ID="serviceLabel" runat="server" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                           <b> DB Size:</b>
                        </td>
                        <td>
                            <asp:Label ID="dbSize" runat="server" Text="" />
                        </td>
                    </tr>
                     <tr>
                        <td>
                            <b>Export</b>
                        </td>
                        <td>
                            
                        </td>
                    </tr>
                    <tr>
                        <td>
                           Scripts:
                        </td>
                        <td>
                            <asp:Button ID="scriptsExportButton"  class="btn" runat="server" Text="Export" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Objects:
                        </td>
                        <td>
                            <asp:Button ID="objectsExportButton"  class="btn" runat="server" Text="Export" />
                        </td>
                    </tr>
                </table>
                </center>
        </div>
    </div>
    <div class="span2">
    </div>    
</asp:Content>

