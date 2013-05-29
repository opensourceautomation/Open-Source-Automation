<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="patterns.aspx.cs" Inherits="patterns" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

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
        function SetMatchScriptDivPosition() {
            var intY = document.getElementById("MatchesGrid").scrollTop;
            document.cookie = "yPos=$~" + intY + "~$";
        }
        function SetScriptDivPosition() {
            var intY = document.getElementById("ScriptGrid").scrollTop;
            document.cookie = "yPos=#~" + intY + "~#";
        }
    </script>
    <style type="text/css" media="screen">
    #editor { 
        position:relative;
         width: 100%;
        height: 100%;
    }
    </style>
    
    <div class="row-fluid">
        <div class="span4">
            <div class="row-fluid">
                <div class="span12">
                    <div class="row-fluid" ID="PatternsGrid" style="overflow: auto; max-height:650px;" onscroll="SetPatternDivPosition()">
                        <asp:GridView runat="server" ID="gvPatterns"
                            AutoGenerateColumns="False"  
                            GridLines="None"  
                            CssClass="mGrid" ShowHeader="true" 
                            AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvPatterns_RowDataBound" DataKeyNames="pattern,pattern_id" ShowHeaderWhenEmpty="true">  
                            <Columns>  
                                <asp:BoundField DataField="pattern" HeaderText="Pattern" /> 
                                <asp:BoundField DataField="pattern_id" Visible="false" /> 
                            </Columns>  
                        </asp:GridView>
                    </div>
                </div>
            </div>
            <div class="row-fluid">
                <div class="span12">
                    <br />
                    <asp:TextBox runat="server" ID="txtPattern" class="input-large"></asp:TextBox>
                    <asp:Button runat="server" ID="btnPatternSave" class="btn" OnClick="btnPatternAdd_Click" Text="Add" /> &nbsp;
                    <asp:Button runat="server" ID="btnPatternDelete" class="btn" OnClick="btnPatternDelete_Click" Text="Delete"/>
                </div>
            </div>
        </div>
        <div class="span4">
            <div class="row-fluid">
                <div class="span12">
                    <div class="row-fluid" ID="MatchesGrid" style="overflow: auto; max-height:650px;" onscroll="SetMatchDivPosition()">
                        <asp:GridView runat="server" ID="gvMatches"
                            AutoGenerateColumns="False"  
                            GridLines="None"  
                            CssClass="mGrid" ShowHeader="true" 
                            AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvMatches_RowDataBound" DataKeyNames="match,match_id" ShowHeaderWhenEmpty="true">  
                            <Columns>  
                                <asp:BoundField DataField="match" HeaderText="Match" /> 
                                <asp:BoundField DataField="match_id" Visible="false" /> 
                            </Columns>  
                        </asp:GridView>
                    </div>
                </div>
            </div>
            <div class="row-fluid">
                <div class="span12" style="text-align:right;">
                    <asp:Panel runat="server" ID="pnlMatchForm" Visible ="false">
                        <br />
                        <asp:TextBox runat="server" ID="txtMatch" class="input-large"></asp:TextBox>
                        <asp:Button runat="server" ID="btnMatchAdd" class="btn" OnClick="btnMatchAdd_Click" Text="Add" /> &nbsp;
                        <asp:Button runat="server" ID="btnMatchDelete" class="btn" OnClick="btnMatchDelete_Click" Text="Delete"/>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <div class="span4">
            <div class="row-fluid">
                <div class="span12">
                    <div class="row-fluid" ID="scriptGrid" style="overflow: auto; max-height:650px; " onscroll="SetScriptDivPosition()">
                        <asp:GridView runat="server" ID="gvScripts"
                                AutoGenerateColumns="False"  
                                GridLines="None"  
                                CssClass="mGrid" ShowHeader="true" 
                                AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvScripts_RowDataBound" DataKeyNames="script_name,script_sequence, pattern_script_id" ShowHeaderWhenEmpty="true">  
                                <Columns>  
                                    <asp:BoundField DataField="script_name" HeaderText="Script" /> 
                                    <asp:BoundField DataField="script_sequence" HeaderText="Sequence" Visible="False" /> 
                                    <asp:BoundField DataField="pattern_script_id" Visible="False" /> 
                                </Columns>  
                            </asp:GridView>
                    </div>
                </div>
            </div>
            <div class="row-fluid">
                <div class="span12" style="text-align:right;">
                    <asp:Panel runat="server" ID="pnlScriptForm" Visible ="false">
                        <br />
                        <asp:DropDownList runat="server" ID="ddlScript" datatextfield="Text" datavaluefield="Value" ></asp:DropDownList>
                        <asp:Button runat="server" ID="btnScriptAdd" class="btn" OnClick="btnScriptAdd_Click" Text="Add" /> &nbsp;
                        <asp:Button runat="server" ID="btnScriptDelete" class="btn" OnClick="btnScriptDelete_Click" Text="Delete"/>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
        
    <asp:HiddenField  runat="server" ID="hdnScript"></asp:HiddenField>
    <asp:Label runat="server" ID="hdnSelectedPatternRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPatternName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedMatchRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedMatchID" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedMatchName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedScriptRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedScriptID" Visible="false"></asp:Label>
    
</asp:Content>

