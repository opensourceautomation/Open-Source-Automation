<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" CodeFile="schedules.aspx.cs" Inherits="schedules" %>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <script type="text/javascript">
        window.onload = function () {
            var strCook = document.cookie;
            if (strCook.indexOf("!~") != 0) {
                var intS = strCook.indexOf("!~");
                var intE = strCook.indexOf("~!");
                var strPos = strCook.substring(intS + 2, intE);
                document.getElementById("ObjGrid").scrollTop = strPos;
            }
        }
        function SetDivPosition() {
            var intY = document.getElementById("ObjGrid").scrollTop;
            document.cookie = "yPos=!~" + intY + "~!";
        }
    </script>
    <style>
        #EditForm
        {
            position: fixed;
            top: 65px;
            right: 20px;
            bottom: 35px;
            overflow: auto;
            padding: 15px;
            height: 90%;
        }

        #ObjGrid
        {
            height: 90%;
            overflow: auto;
            position: fixed;
            bottom: 35px;
            top: 65px;
        }
    </style>
    <div class="row-fluid">
        <div class="span8" id="ObjGrid" onscroll="SetDivPosition()">
            <asp:GridView runat="server" ID="schedulesGridView"
                AutoGenerateColumns="False"
                GridLines="None"
                CssClass="mGrid"
                AlternatingRowStyle-CssClass="alt" DataKeyNames="schedule_name" Style="overflow: auto;">
                <Columns>
                    <asp:BoundField DataField="schedule_name" HeaderText="Name" />
                    <asp:BoundField DataField="interval_unit" HeaderText="Interval" />
                </Columns>

            </asp:GridView>
        </div>
        <div class="span4">
        </div>
    </div>

    <asp:Label runat="server" ID="hdnSelectedRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedObjectName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropName" Visible="false"></asp:Label>
</asp:Content>

