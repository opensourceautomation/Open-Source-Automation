<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="scripts.aspx.cs" Inherits="scripts" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server"> 
    <script>
        window.onload = function () {
            var strCook = document.cookie;
            if (strCook.indexOf("!~") != 0) {
                var intS = strCook.indexOf("!~");
                var intE = strCook.indexOf("~!");
                var strPos = strCook.substring(intS + 2, intE);
                document.getElementById("ScriptGrid").scrollTop = strPos;
            }
            if (strCook.indexOf("$~") != 0) {
                var intS = strCook.indexOf("$~");
                var intE = strCook.indexOf("~$");
                var strPos = strCook.substring(intS + 2, intE);
                var grid = document.getElementById("eventScriptGrid");
                if (grid != null) {
                    grid.scrollTop = strPos;
                }                
            }
            if (strCook.indexOf("#~") != 0) {
                var intS = strCook.indexOf("#~");
                var intE = strCook.indexOf("~#");
                var strPos = strCook.substring(intS + 2, intE);
                var grid = document.getElementById("objTypeScriptGrid");
                if (grid != null) {
                    grid.scrollTop = strPos;
                }               
            }
        }

        function saveScriptAdd() {
            document.getElementById('<%=hdnScript.ClientID%>').value = editor.getValue();
            $('#ctl00_ContentPlaceHolder_btnAdd2').click();
        }

        function saveScriptUpdate() {
            document.getElementById('<%=hdnScript.ClientID%>').value = editor.getValue();
            $('#ctl00_ContentPlaceHolder_btnUpdate2').click();
        }
        function SetDivPosition() {
            var intY = document.getElementById("ScriptGrid").scrollTop;
            document.cookie = "yPos=!~" + intY + "~!";
        }
        function SetEventScriptDivPosition() {
            var intY = document.getElementById("eventScriptGrid").scrollTop;
            document.cookie = "yPos=$~" + intY + "~$";
        }
        function SetObjTypeEventScriptDivPosition() {
            var intY = document.getElementById("objTypeScriptGrid").scrollTop;
            document.cookie = "yPos=#~" + intY + "~#";
        }

        function copyScript() {
            $('#dvmodalbody').html(editor.getValue().replace(/\n/g, "<br />"));
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
        <div class="span3">
            <div ID="ScriptPanel">
                <div class="row-fluid" ID="ScriptGrid" style="overflow: auto; max-height:350px;" onscroll="SetDivPosition()">
                    <asp:GridView runat="server" ID="gvScripts"
                        AutoGenerateColumns="False"  
                        GridLines="None"  
                        CssClass="mGrid" ShowHeader="true" 
                        AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvScripts_RowDataBound" DataKeyNames="script_name,script_id, script_processor_id, script_processor_name" ShowHeaderWhenEmpty="true">  
                        <Columns>  
                            <asp:BoundField DataField="script_name" HeaderText="Script" /> 
                            <asp:BoundField DataField="script_id" Visible="false" /> 
                            <asp:BoundField DataField="script_processor_id" Visible="false" /> 
                        </Columns>  
                    </asp:GridView>
                </div>
            </div>
        </div>
        <div class="span9">
            <div class="row-fluid">
                <div class="span12">
                    Name: <asp:TextBox runat="server" ID="txtName" class="input-xlarge" title="The name to be given to the script (used to link events and patterns to scripts)" />
                    <div style="float:right;">
                        Script Processor: <asp:DropDownList runat="server" ID="ddlScriptProcessor" datatextfield="Text" datavaluefield="Value" title="The script processor that will execute the content of the script" style="width:300px"></asp:DropDownList></div>
                </div>
            </div>
            <div class="row-fluid">
                <div class="span12" style="height:300px;width:100%;border:solid">
                    <div id="editor"></div>
                </div>
            </div>
            <div class="row-fluid">
                <div class="span3" style="text-align:left;">
                    <br />
                    <a href="#linkModal" role="button" class="btn" data-toggle="modal" >Linkage</a>
                    <a href="#myModal" role="button" class="btn" data-toggle="modal" onclick="copyScript();">Copy</a>
                </div>
                <div class="span6" style="text-align:left;">
                    <div class="alert alert-error" runat="server" id="alert" visible="false"  >Must enter script name and Script Processor</div> &nbsp;
                    <div class="alert alert-success" runat="server" id="saveAlert" visible="false" >Saved!</div> &nbsp;
                    <div class="alert alert-warning" runat="server" id="deleteAlert" visible="false" >Deleted!</div> &nbsp;
                </div>
                <div class="span3" style="text-align:right;">
                    <br />
                    <asp:Button runat="server" ID="btnAdd" class="btn" OnClientClick="saveScriptAdd();" Text="Add" /> &nbsp;
                    <asp:Button runat="server" ID="btnUpdate" class="btn" OnClientClick="saveScriptUpdate();" Text="Update" /> &nbsp;
                    <asp:Button runat="server" ID="btnDelete" class="btn" OnClick="btnDelete_Click" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete the script?');"/>
                    <asp:Button runat="server" ID="btnAdd2" OnClick="btnAdd_Click" style="display:none;" /> &nbsp;
                    <asp:Button runat="server" ID="btnUpdate2" OnClick="btnUpdate_Click" style="display:none;" /> &nbsp;
                    
                </div>
            </div>
        </div>
    </div>
    <hr />
    <div class="row-fluid">
        <div class="span6">
            <h3>Object Event Scripts</h3>
            <br />
            <div class="row-fluid">
                <div class="span6">
                    <div class="row-fluid">
                        <div class="span2" style="text-align:right;">
                            <label>Object:</label>
                        </div>
                        <div class="span10" style="text-align:left;">
                            <asp:DropDownList runat="server" ID="ddlObject" datatextfield="Text" datavaluefield="Value" style="width:100%" OnSelectedIndexChanged="ddlObject_SelectedIndexChanged" AutoPostBack="true" title="The object you want to associate the script to" />
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div class="span2" style="text-align:right;">
                            <label>Event:</label>
                        </div>
                        <div class="span10" style="text-align:left;">
                            <asp:DropDownList runat="server" ID="ddlEvent" datatextfield="Text" datavaluefield="Value" style="width:100%" OnSelectedIndexChanged="ddlEvent_SelectedIndexChanged" title="the event of the object that you want the script to be associated to" AutoPostBack="true"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row-fluid">       
                        <div class="span2" style="text-align:right;">
                            <label>Script:</label>
                        </div>
                        <div class="span10" style="text-align:left;">
                            <asp:DropDownList runat="server" ID="ddlScript" datatextfield="Text" datavaluefield="Value" title="The script you want to run when the event occurs" style="width:100%"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <asp:Button runat="server" ID="btnAddEventScript" class="btn" OnClick="btnAddEventScript_Click" Text="Add" style="float:right;"/>
                    </div>
                </div>
                <div class="span6">
                    <asp:Panel runat="server" ID="pnlEventScripts" Visible="false">
                        <div class="row-fluid" ID="eventScriptGrid" style="overflow: auto; max-height:350px; " onscroll="SetEventScriptDivPosition()">
                            <asp:GridView runat="server" ID="gvEventScripts"
                                AutoGenerateColumns="False"  
                                GridLines="None"  
                                CssClass="mGrid" ShowHeader="true" 
                                AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvEventScripts_RowDataBound" DataKeyNames="script_name,script_sequence, event_script_id" ShowHeaderWhenEmpty="true">  
                                <Columns>  
                                    <asp:BoundField DataField="script_name" HeaderText="Script" /> 
                                    <asp:BoundField DataField="script_sequence" HeaderText="Sequence" Visible="False"/> 
                                    <asp:BoundField DataField="event_script_id" Visible="False" /> 
                                </Columns>  
                            </asp:GridView>
                        </div>
                    <div class="row-fluid">
                        <br />
                        <asp:Button runat="server" ID="btnDeleteEventScript" class="btn" OnClick="btnDeleteEventScript_Click" Text="Delete" style="float:right;" Visible="false"/>
                    </div>

                    </asp:Panel>
                </div>
            </div>

        </div>
        <div class="span6">
            <h3>Object Type Event Scripts</h3>
            <br />
            <div class="row-fluid">
                <div class="span6">
                    <div class="row-fluid">
                        <div class="span2" style="text-align:right;">
                            <label>Object Type:</label>
                        </div>
                        <div class="span10" style="text-align:left;">
                            <asp:DropDownList runat="server" ID="ddlObjectType" datatextfield="Text" datavaluefield="Value" style="width:100%" OnSelectedIndexChanged="ddlObjectType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div class="span2" style="text-align:right;">
                            <label>Event:</label>
                        </div>
                        <div class="span10" style="text-align:left;">
                            <asp:DropDownList runat="server" ID="ddlObjTypeEvent" datatextfield="Text" datavaluefield="Value" style="width:100%" OnSelectedIndexChanged="ddlObjTypeEvent_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row-fluid">       
                        <div class="span2" style="text-align:right;">
                            <label>Script:</label>
                        </div>
                        <div class="span10" style="text-align:left;">
                            <asp:DropDownList runat="server" ID="ddlObjTypeScript" datatextfield="Text" datavaluefield="Value" style="width:100%"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <asp:Button runat="server" ID="btnAddObjTypeEventScript" class="btn" OnClick="btnAddObjTypeEventScript_Click" Text="Add" style="float:right;"/>
                    </div>
                </div>
                <div class="span6">
                    <asp:Panel runat="server" ID="pnlObjTypeEventScripts" Visible="false">
                        <div class="row-fluid" ID="objTypeScriptGrid" style="overflow: auto; max-height:350px;" onscroll="SetObjtypeEventScriptDivPosition()">
                            <asp:GridView runat="server" ID="gvObjTypeScripts"
                                AutoGenerateColumns="False"  
                                GridLines="None"  
                                CssClass="mGrid" ShowHeader="true" 
                                AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvObjTypeScripts_RowDataBound" DataKeyNames="script_name,script_sequence, object_type_event_script_id" ShowHeaderWhenEmpty="true">  
                                <Columns>  
                                    <asp:BoundField DataField="script_name" HeaderText="Script" /> 
                                    <asp:BoundField DataField="script_sequence" HeaderText="Sequence" Visible="False"/> 
                                    <asp:BoundField DataField="object_type_event_script_id" Visible="False" /> 
                                </Columns>  
                            </asp:GridView>
                        </div>
                    <div class="row-fluid">
                        <br />
                        <asp:Button runat="server" ID="btnDeleteObjTypeEventScript" class="btn" OnClick="btnDeleteObjTypeEventScript_Click" Text="Delete" style="float:right;" Visible="false"/>
                    </div>

                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Modal -->
    <div id="myModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
        <h3 id="myModalLabel">Copy Script</h3>
      </div>
      <div ID="dvmodalbody" class="modal-body">
        
      </div>
      <div class="modal-footer">
        <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
      </div>
    </div>

    <!-- Modal -->
    <div id="linkModal" class="modal hide" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
        <h3 id="H1">Script Linkage</h3>
      </div>
      <div ID="dvlinkmodalbody" class="modal-body">
        <asp:GridView runat="server" ID="gvLinkage"
            AutoGenerateColumns="False"  
            GridLines="None"  
            CssClass="mGrid"  
            AlternatingRowStyle-CssClass="alt" ShowHeaderWhenEmpty="true">  
            <Columns>  
                <asp:BoundField DataField="object_name" HeaderText="Object" />
                <asp:BoundField DataField="event_name" HeaderText="Event" />
            </Columns>  
        </asp:GridView>
      </div>
      <div class="modal-footer">
        <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
      </div>
    </div>
             
    <asp:HiddenField  runat="server" ID="hdnScript"></asp:HiddenField>
    <asp:Label runat="server" ID="hdnSelectedRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedScriptName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedEventScriptRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedEventScriptID" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedObjTypeEventScriptRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedObjTypeEventScriptID" Visible="false"></asp:Label>
    <script src="js/ace.js" type="text/javascript" charset="utf-8"></script>
    <script>
        var editor = ace.edit("editor");
        editor.setTheme("ace/theme/tomorrow");
        //editor.getSession().setMode("ace/mode/powershell");
        editor.setValue(document.getElementById('<%=hdnScript.ClientID%>').value);
        editor.clearSelection();

        $(function () {
            $("#<%=ddlScriptProcessor.ClientID%>").change(function () {
                setSytaxHighlighter();
            });
        });

        function setSytaxHighlighter() {
            if ($("#<%=ddlScriptProcessor.ClientID%>").val() == 'PowerShell') {
                editor.getSession().setMode("ace/mode/powershell");
            }
        }
    </script>
</asp:Content>

