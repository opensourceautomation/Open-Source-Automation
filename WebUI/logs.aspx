<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="logs.aspx.cs" Inherits="logs" %>

<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <script type="text/javascript">
        window.onload = function () {
            var strCook = document.cookie;
            if (strCook.indexOf("!~") != 0) {
                var intS = strCook.indexOf("!~");
                var intE = strCook.indexOf("~!");
                var strPos = strCook.substring(intS + 2, intE);
                document.getElementById("logsGrid").scrollTop = strPos;
            }
        }
        function SetDivPosition() {
            var intY = document.getElementById("logsGrid").scrollTop;
            document.cookie = "yPos=!~" + intY + "~!";
        }
    </script>
    <div class="row-fluid">
        <div class="span2">
            <div id="ObjPanel">
                <div class="row-fluid" id="logsGrid" style="overflow: auto; max-height: 670px;" onscroll="SetDivPosition()">
                    <asp:GridView runat="server" ID="gvLogs"
                        AutoGenerateColumns="false"
                        GridLines="None"
                        CssClass="mGrid"
                        AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvLogs_RowDataBound" DataKeyNames="logName">
                        <Columns>
                            <asp:BoundField HeaderText="Logs" DataField="logName" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            <asp:Button runat="server" ID="btnRefresh" Text="Refresh" class="btn" OnClick="btnRefresh_Click"/>
            <asp:Button runat="server" ID="clearLogs" Text="Clear All" class="btn" OnClick="clearLogs_Click"/>
            <asp:Button runat="server" ID="btnExport" Text="Export" class="btn" OnClick="btnExport_Click"/>
        </div>
        <div class="span10">
            <asp:Panel ID="panelLogContent" runat="server" Visible="false">
                <asp:TextBox ID="logContentTextBox" runat="server" Width="99%" Height="50em" TextMode="MultiLine" ReadOnly="false" />
            </asp:Panel>
            <asp:Button runat="server" ID="btnClearLog" Text="Clear" class="btn" OnClick="clearLog_Click" Visible="false"/>
        </div>
    </div>

    <asp:Label runat="server" ID="hdnSelectedRow" Visible="false" />
</asp:Content>

