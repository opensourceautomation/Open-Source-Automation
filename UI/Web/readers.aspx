<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="readers.aspx.cs" Inherits="readers" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" ValidateRequest="false"%>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    <script>
        window.onload = function () {
            var strCook = document.cookie;
            if (strCook.indexOf("!~") != 0) {
                var intS = strCook.indexOf("!~");
                var intE = strCook.indexOf("~!");
                var strPos = strCook.substring(intS + 2, intE);
                var grid = document.getElementById("PatternsGrid");
                if (grid != null) {
                    grid.scrollTop = strPos;
                }               
            }
            if (strCook.indexOf("$~") != 0) {
                var intS = strCook.indexOf("$~");
                var intE = strCook.indexOf("~$");
                var strPos = strCook.substring(intS + 2, intE);
                var grid = document.getElementById("MatchesGrid");
                if (grid != null) {
                    grid.scrollTop = strPos;
                }
            }
            if (strCook.indexOf("#~") != 0) {
                var intS = strCook.indexOf("#~");
                var intE = strCook.indexOf("~#");
                var strPos = strCook.substring(intS + 2, intE);
                var grid = document.getElementById("ScriptGrid");
                if (grid != null) {
                    grid.scrollTop = strPos;
                }               
            }
        }

        function SetPatternDivPosition() {
            var intY = document.getElementById("PatternsGrid").scrollTop;
            document.cookie = "yPos=!~" + intY + "~!";
        }

    </script>
    <style type="text/css" media="screen">
    #editor { 
        position:relative;
        width: 100%;
        height: 100%;
    }
    </style>
    <asp:Panel ID="pnlGrid" runat="server" Height="365px" Width="775px">
        <asp:GridView runat="server" ID="gvReaders" AutoGenerateColumns="False" GridLines="None" CssClass="mGrid" 
            AlternatingRowStyle-CssClass ="alt" OnRowDataBound="gvReaders_RowDataBound" 
            DataKeyNames="object_name,property_name,object_property_scraper_id,object_id,object_property_id,URL,search_prefix,search_prefix_offset,search_suffix,update_interval" 
            ShowHeaderWhenEmpty="True" Width="779px">  
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
            <Columns>  
                <asp:BoundField DataField="object_name" HeaderText="Object" /> 
                <asp:BoundField DataField="property_name" HeaderText="Property" /> 
                <asp:BoundField DataField="object_property_scraper_id" Visible="False" />
                <asp:BoundField DataField="object_id" Visible="False" />
                <asp:BoundField DataField="object_property_id" Visible="False" />
                <asp:BoundField DataField="URL" Visible="False" />
                <asp:BoundField DataField="search_prefix" Visible="False" />
                <asp:BoundField DataField="search_prefix_offset" Visible="False" />
                <asp:BoundField DataField="search_suffix" Visible="False" />
                <asp:BoundField DataField="update_interval" Visible="False" />
            </Columns>  
        </asp:GridView>
    </asp:Panel>
    <asp:Panel ID="pnlDropDowns" runat="server" Height="55px" Width="775px">
        <asp:Label ID="Label5" runat="server" Text="Object  "></asp:Label>
        <asp:DropDownList runat="server" ID="ddlObjects" datatextfield="object_name" datavaluefield="object_id" style="width:280px;" OnSelectedIndexChanged="ddlObjects_SelectedIndexChanged" AutoPostBack="true">
            <asp:ListItem Selected = "True"  Text = "" Value = ""></asp:ListItem>
        </asp:DropDownList>
        &nbsp;&nbsp;&nbsp;
        <asp:Label ID="Label6" runat="server" Text="  Property  "></asp:Label>
        <asp:DropDownList ID="ddlProperties" runat="server" datatextfield="property_name" datavaluefield="property_id" style="width:280px;">
            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
         </asp:DropDownList>
     </asp:Panel>
     <asp:Panel ID="pnlURL" runat="server" Height="55px" Width="775px">
         <asp:Label ID="Label7" runat="server" Text="URL  "></asp:Label>
         <asp:TextBox ID="txtURL" runat="server" Width="714px"></asp:TextBox>
     </asp:Panel>
     <asp:Panel ID="pnlSettings" runat="server" Height="55px" Width="775px">
         <asp:Label ID="Label1" runat="server" Text="Prefix  "></asp:Label>
         <asp:TextBox ID="txtPrefix" runat="server"></asp:TextBox>
         &nbsp;
         <asp:Label ID="Label2" runat="server" Text="Prefix Offset  "></asp:Label>
         <asp:TextBox ID="txtPrefixOffset" runat="server" Width="36px"></asp:TextBox>
         &nbsp;
         <asp:Label ID="Label3" runat="server" Text="Suffix  "></asp:Label>
         <asp:TextBox ID="txtSuffix" runat="server" Width="145px"></asp:TextBox>
         &nbsp;
         <asp:Label ID="Label4" runat="server" Text="Interval  "></asp:Label>
         <asp:TextBox ID="txtInterval" runat="server" Width="64px"></asp:TextBox>
     </asp:Panel>
     <asp:Panel ID="pnlButtons" runat="server" Height="55px" Width="775px">
         <asp:Button runat="server" ID="btnReaderSave" class="btn" OnClick="btnReaderAdd_Click" Text="Add" /> &nbsp;
         <asp:Button runat="server" ID="btnReaderUpdate" class="btn" OnClick="btnReaderUpdate_Click" Text="Update" /> &nbsp;
         <asp:Button runat="server" ID="btnReaderDelete" class="btn" OnClick="btnReaderDelete_Click" Text="Delete"/>
         <a href="#linkModal" role="button" class="btn" data-toggle="modal" >Export</a>
     </asp:Panel>
    <br />

    <!-- Modal -->
    <div id="linkModal" class="modal hide" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
            <h3 id="H1">Readers Script</h3>
        </div>
        <div id="exportModalBody" class="modal-body">
            <asp:TextBox ID="lblExportReaders" runat="server" TextMode="MultiLine" Font-Size="Smaller"></asp:TextBox>
        </div>
    </div>
        
    <asp:Label runat="server" ID="hdnSelectedReadersRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedReadersName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedReadersLastRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedReadersLastName" Visible="false"></asp:Label>


    
</asp:Content>

