<%@ Page Language="C#" AutoEventWireup="true" CodeFile="config.aspx.cs" Inherits="config" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder">
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
    <script>
        $(function () {
            $("#accordion").accordion({
                active: false,
                collapsible: true
            });
        });
    </script>
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
                            <div id="accordion">
                            <h3><asp:Label ID="dbSize" runat="server" Text="" /></h3>
                                <div>
                                    &emsp;<b>Images:</b><asp:Label ID="imagesSize" runat="server" Text="" /> <br />
                                    &emsp;<b>Debug Log:</b><asp:Label ID="debugLog" runat="server" Text="" /><br /> 
                                    &emsp;<b>Event Log:</b><asp:Label ID="eventLogSize" runat="server" Text="" /> <br />
                                    &emsp;<b>Method Log:</b><asp:Label ID="methodLogSize" runat="server" Text="" /> <br />
                                    &emsp;<b>Method Queue:</b><asp:Label ID="methodQueueSize" runat="server" Text="" /> <br />
                                    &emsp;<b>Objects:</b><asp:Label ID="objectsSize" runat="server" Text="" /> <br />
                                    &emsp;<b>Object State History:</b><asp:Label ID="objectStateHistory" runat="server" Text="" /> <br />
                                    &emsp;<b>Object Property History:</b><asp:Label ID="objectPropertyHistory" runat="server" Text="" /> <br />
                                    &emsp;<b>Scripts:</b><asp:Label ID="scriptSize" runat="server" Text="" /> <br />
                                 </div>
                            </div>
                           
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
                            <asp:Button ID="scriptsExportButton"  class="btn" runat="server" Text="Export" OnClick="scriptsExportButton_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Objects:
                        </td>
                        <td>
                            <asp:Button ID="objectsExportButton"  class="btn" runat="server" Text="Export" OnClick="objectsExportButton_Click" />
                        </td>
                    </tr>
                </table>
                </center>
        </div>
    </div>
    <div class="span2">
    </div>
</asp:Content>

