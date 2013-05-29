<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="objtypes.aspx.cs" Inherits="objtypes" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder">
    <script type="text/javascript">
        var lastSelected = null;

        window.onload = function () {
            var strCook = document.cookie;
            if (strCook.indexOf("!~") != 0) {
                var intS = strCook.indexOf("!~");
                var intE = strCook.indexOf("~!");
                var strPos = strCook.substring(intS + 2, intE);
                document.getElementById("ObjGrid").scrollTop = strPos;
            }
            if (strCook.indexOf("$~") != 0) {
                var intS = strCook.indexOf("$~");
                var intE = strCook.indexOf("~$");
                var strPos = strCook.substring(intS + 2, intE);
                var grid = document.getElementById("propGrid");
                if (grid != null) {
                    grid.scrollTop = strPos;
                }
            }
            if (strCook.indexOf("#~") != 0) {
                var intS = strCook.indexOf("#~");
                var intE = strCook.indexOf("~#");
                var strPos = strCook.substring(intS + 2, intE);
                var grid = document.getElementById("stateGrid");
                if (grid != null) {
                    grid.scrollTop = strPos;
                }
            }
            if (strCook.indexOf("@~") != 0) {
                var intS = strCook.indexOf("@~");
                var intE = strCook.indexOf("~@");
                var strPos = strCook.substring(intS + 2, intE);
                var grid = document.getElementById("methodGrid");
                if (grid != null) {
                    grid.scrollTop = strPos;
                }
            }
        }
        function SetDivPosition() {
            var intY = document.getElementById("ObjGrid").scrollTop;
            document.cookie = "yPos=!~" + intY + "~!";
        }
        function SetPropDivPosition() {
            var intY = document.getElementById("propGrid").scrollTop;
            document.cookie = "yPos=$~" + intY + "~$";
        }
        function SetPropDivPosition() {
            var intY = document.getElementById("stateGrid").scrollTop;
            document.cookie = "yPos=#~" + intY + "~#";
        }
        function SetPropDivPosition() {
            var intY = document.getElementById("methodGrid").scrollTop;
            document.cookie = "yPos=@~" + intY + "~@";
        }

        function selectPropOptionsItem(name, element) {
            if (lastSelected != null) {
                lastSelected.style.backgroundColor = 'white';
                $(lastSelected).attr("onmouseout", "this.style.cursor='hand';this.style.background='white';");
                $(lastSelected).attr("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            }
            $('#<%=hdnPropOptionsItemName.ClientID%>').val(name);
            element.style.backgroundColor = 'lightblue';
            $(element).removeAttr("onmouseout");

            lastSelected = element;
        }

        $(function () {
            if ($('#<%=hdnEditingPropOptions.ClientID%>').val() == "1") {
                $('#<%=hdnEditingPropOptions.ClientID%>').val("0");
                $('#myPropOptionsModal').modal('show');
            }
        });
    </script> 
    <style>

    </style>
    <div class="row-fluid">
        <div class="span6">
            <div ID="ObjPanel">
                <div class="row-fluid" ID="ObjGrid" style="overflow: auto; max-height:670px; " onscroll="SetDivPosition()">
                    <asp:GridView runat="server" ID="gvObjectTypes"
                        AutoGenerateColumns="False"  
                        GridLines="None"  
                        CssClass="mGrid"  
                        AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvObjectTypes_RowDataBound" DataKeyNames="object_type">  
                        <Columns>  
                            <asp:BoundField DataField="base_type" HeaderText="Base Type" /> 
                            <asp:BoundField DataField="object_type" HeaderText="Object Type" />   
                            <asp:BoundField DataField="object_type_description" HeaderText="Description" />  
                        </Columns>  
                    </asp:GridView>
                </div>
                <br />
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label>Name</label>
                    </div>
                    <div class="span4" style="text-align:left;">
                        <asp:TextBox class="input" runat="server" ID="txtName" style="width:100%;"></asp:TextBox>
                    </div>
                    <div class="span2" style="text-align:right;">
                        <label>Owned By</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:DropDownList runat="server" ID="ddlOwnedBy" datatextfield="Text" datavaluefield="Value" style="width:100%;">
                            <asp:ListItem Selected = "True" Text = "" Value = ""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label>Desc</label>
                    </div>
                    <div class="span4" style="text-align:left;">
                        <asp:TextBox class="input" runat="server" ID="txtDescr" style="width:100%;"></asp:TextBox>
                    </div>
                    <div class="span2" style="text-align:right;">
                        <label>Base Type</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:DropDownList runat="server" ID="ddlBaseType" datatextfield="Text" datavaluefield="Value" style="width:100%;">
                            <asp:ListItem Selected = "True" Text = "" Value = ""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="span8" style="text-align:left;">
                        <asp:CheckBox runat="server" ID="chkOwner" /> Object Type Owner &nbsp;
                        <asp:CheckBox runat="server" ID="chkContainer" /> Container &nbsp;
                        <br />
                        <asp:CheckBox runat="server" ID="chkSysType" /> Reserved System Type &nbsp;
                        <asp:CheckBox runat="server" ID="chkHideEvents" /> Hide Redundant Events &nbsp;
                    </div>
                    <div class="span4" style="text-align:right;" >
                        <asp:Button runat="server" ID="btnAdd" Text="Add" class="btn" OnClick="btnAdd_Click"/>&nbsp
                        <asp:Button runat="server" ID="btnUpdate" Text="Update" class="btn" OnClick="btnUpdate_Click"/>&nbsp
                        <asp:Button runat="server" ID="btnDelete" Text="Delete" class="btn" OnClick="btnDelete_Click" OnClientClick="return confirm('Are you sure you want to delete the object type?');"/>
                    </div>
                </div>
            </div>
        </div>
        <div class="span6">
            <asp:Panel ID="panelEditForm" runat="server" Visible="false">
                <div runat="server" ID="divStates" class="hero-unit" style="padding: 10px; padding-right: 0px; margin-bottom: 10px;">
                     <div class="row-fluid">   
                        <h3 style="float:right; margin-right:10px;">States</h3>
                        <br />
                        <div class="span5"  id="stateGrid" style="overflow: auto; max-height:300px;"  onscroll="SetStateDivPosition()">
                           <asp:GridView runat="server" ID="gvStates"
                                AutoGenerateColumns="False"  
                                GridLines="None"  
                                CssClass="mGrid"  
                                AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvStates_RowDataBound" DataKeyNames="state_name, state_label" ShowHeaderWhenEmpty="true">  
                                <Columns>  
                                    <asp:BoundField DataField="state_name" HeaderText="Name" /> 
                                    <asp:BoundField DataField="state_label" HeaderText="Label" /> 
                                </Columns>  

                            </asp:GridView>
                        </div>
                        <div class="span6">
                            <asp:Panel runat="server" ID="pnlStateForm">
                                
                                    Name: <asp:TextBox  runat="server" ID="txtStateName" style="width:225px;"></asp:TextBox>
                                    <br />
                                    Label: <asp:TextBox  runat="server" ID="txtStateLabel" style="width:225px;"></asp:TextBox>
                                    <br />
                                    <asp:Button class="btn btn" runat="server" ID="btnStateSave" Text="Save" OnClick="btnStateSave_Click"/>
                                    <asp:Button class="btn btn" runat="server" ID="btnStateAdd" Text="Add" OnClick="btnStateAdd_Click"/>
                                    <asp:Button class="btn btn" runat="server" ID="btnStateDelete" Text="Delete" OnClick="btnStateDelete_Click"/>
                                
                           </asp:Panel>
                        </div>
                    </div>
                </div>
                <div  runat="server" ID="divMethods" class="hero-unit" style="padding: 10px; padding-right: 0px; margin-bottom: 10px;">
                    <div class="row-fluid">
                        <h3 style="float:right; margin-right:10px;">Methods</h3>
                        <br />
                        <div class="span5" id="methodGrid" style="overflow: auto;max-height:300px;"  onscroll="SetMethodDivPosition()">
                            <asp:GridView runat="server" ID="gvMethods"
                                AutoGenerateColumns="False"  
                                GridLines="None"  
                                CssClass="mGrid"  
                                AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvMethods_RowDataBound" DataKeyNames="method_name, method_label, param_1_label, param_2_label, param_1_default, param_2_default" ShowHeaderWhenEmpty="true">  
                                <Columns>  
                                    <asp:BoundField DataField="method_name" HeaderText="Name" /> 
                                    <asp:BoundField DataField="method_label" HeaderText="Label" />  
                                    <asp:BoundField DataField="param_1_label" Visible="false" />  
                                    <asp:BoundField DataField="param_2_label" Visible="false" /> 
                                    <asp:BoundField DataField="param_1_default" Visible="false" />  
                                    <asp:BoundField DataField="param_2_default" Visible="false" /> 
                                </Columns>  

                            </asp:GridView>
                        </div>
                        <div class="span6">
                            <asp:Panel runat="server" ID="pnlMethodForm">
                                
                                    Name: <asp:TextBox  runat="server" ID="txtMethodName" style="width:225px;"></asp:TextBox>
                                    <br />
                                    Label: <asp:TextBox  runat="server" ID="txtMethodLabel" style="width:225px;"></asp:TextBox>
                                    <br />
                                    Parameter Labels / Default Values
                                    <br />
                                    <asp:TextBox  runat="server" ID="txtParam1Label" style="width:100px;"></asp:TextBox><asp:TextBox  runat="server" ID="txtParam1Default" style="width:100px;"></asp:TextBox>
                                    <br />
                                    <asp:TextBox  runat="server" ID="txtParam2Label" style="width:100px;"></asp:TextBox><asp:TextBox  runat="server" ID="txtParam2Default" style="width:100px;"></asp:TextBox>
                                    <br />
                                    <asp:Button class="btn btn" runat="server" ID="btnMethodSave" Text="Save" OnClick="btnMethodSave_Click"/>
                                    <asp:Button class="btn btn" runat="server" ID="btnMethodAdd" Text="Add" OnClick="btnMethodAdd_Click"/>
                                    <asp:Button class="btn btn" runat="server" ID="btnMethodDelete" Text="Delete" OnClick="btnMethodDelete_Click"/>
                                
                           </asp:Panel>
                        </div>

                    </div>
                </div>
                <div  runat="server" ID="divEvents" class="hero-unit" style="padding: 10px; padding-right: 0px; margin-bottom: 10px;">
                    <div class="row-fluid">
                        <h3 style="float:right; margin-right:10px;">Events</h3>
                        <br />
                        <div class="span5" ID="eventGrid" style="overflow: auto; max-height:300px;"  onscroll="SetEventDivPosition()">
                            <asp:GridView runat="server" ID="gvEvents"
                                AutoGenerateColumns="False"  
                                GridLines="None"  
                                CssClass="mGrid"  
                                AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvEvents_RowDataBound" DataKeyNames="event_name, event_label" ShowHeaderWhenEmpty="true">  
                                <Columns>  
                                    <asp:BoundField DataField="event_name" HeaderText="Name" /> 
                                    <asp:BoundField DataField="event_label" HeaderText="Label" /> 
                                </Columns>  

                            </asp:GridView>
                        </div>
                        <div class="span6">
                            <asp:Panel runat="server" ID="pnlEventForm">
                                
                                    Name: <asp:TextBox  runat="server" ID="txtEventName" style="width:225px;"></asp:TextBox>
                                    <br />
                                    Label: <asp:TextBox  runat="server" ID="txtEventLabel" style="width:225px;"></asp:TextBox>
                                    <br />
                                    <asp:Button class="btn btn" runat="server" ID="btnEventSave" Text="Save" OnClick="btnEventSave_Click"/>
                                    <asp:Button class="btn btn" runat="server" ID="btnEventAdd" Text="Add" OnClick="btnEventAdd_Click"/>
                                    <asp:Button class="btn btn" runat="server" ID="btnEventDelete" Text="Delete" OnClick="btnEventDelete_Click"/>
                                
                           </asp:Panel>
                        </div>

                    </div>
                </div>
                <div  runat="server" ID="divProps" class="hero-unit" style="padding: 10px; padding-right: 0px; margin-bottom: 10px;">
                    <div class="row-fluid">
                        <h3 style="float:right; margin-right:10px;">Properties</h3>
                        <br />
                        <div class="span5" id="propGrid" style="overflow: auto; max-height:300px;"  onscroll="SetPropDivPosition()">
                            <asp:GridView runat="server" ID="gvProperties"
                                AutoGenerateColumns="False"  
                                GridLines="None"  
                                CssClass="mGrid"  
                                AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvProperties_RowDataBound" DataKeyNames="property_name, property_datatype, property_default, track_history, property_id" ShowHeaderWhenEmpty="true">  
                                <Columns>  
                                    <asp:BoundField DataField="property_name" HeaderText="Property" /> 
                                    <asp:BoundField DataField="property_datatype" HeaderText="Type" /> 
                                    <asp:BoundField DataField="property_default" visible="false" />
                                    <asp:BoundField DataField="track_history" visible="false" />
                                </Columns>  

                            </asp:GridView>
                        </div>
                        <div class="span6">
                            <asp:Panel runat="server" ID="panelPropForm">
                                
                                Name: <asp:TextBox  runat="server" ID="txtPropName" style="width:225px;"></asp:TextBox>
                                <br />
                                Type: <asp:DropDownList runat="server" ID="ddlPropType" datatextfield="Text" datavaluefield="Value" style="width:200px;">
                                        <asp:ListItem Selected = "True" Text = "String" Value = "String"></asp:ListItem>
                                        <asp:ListItem Text = "Boolean" Value = "Boolean"></asp:ListItem>
                                        <asp:ListItem Text = "Integer" Value = "Integer"></asp:ListItem>
                                        <asp:ListItem Text = "Float" Value = "Float"></asp:ListItem>
                                        <asp:ListItem Text = "List" Value = "List"></asp:ListItem>
                                        <asp:ListItem Text = "Password" Value = "Password"></asp:ListItem>
                                        <asp:ListItem Text = "File" Value = "File"></asp:ListItem>
                                        <asp:ListItem Text = "DateTime" Value = "DateTime"></asp:ListItem>
                                </asp:DropDownList>
                                <br />
                                Default: <asp:TextBox  runat="server" ID="txtPropDefault" style="width:215px;"></asp:TextBox>
                                <br />
                                <asp:CheckBox runat="server" ID="chkTrackChanges" /> Track Changes &nbsp;
                                <br />
                                <asp:Button class="btn btn" runat="server" ID="btnPropSave" Text="Save" OnClick="btnPropSave_Click"/>
                                <asp:Button class="btn btn" runat="server" ID="btnPropAdd" Text="Add" OnClick="btnPropAdd_Click"/>
                                <asp:Button class="btn btn" runat="server" ID="btnPropDelete" Text="Delete" OnClick="btnPropDelete_Click"/>
                                <asp:Button class="btn btn" runat="server" ID="btnEditPropOptions" Text="Edit Options" href="#myPropOptionsModal" data-toggle="modal"/>
                           </asp:Panel>
                        </div>
                    </div>
                </div>
            </asp:Panel> 
        </div>
    </div>
    <!-- Property Options Modal -->
    <div id="myPropOptionsModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
        <h3 id="myModalLabel">Edit Property Options</h3>
      </div>
      <div ID="dvmodalbody" class="modal-body">
        <asp:GridView runat="server" ID="gvPropOptions"
            AutoGenerateColumns="False"  
            GridLines="None"  
            CssClass="mGrid"  
            AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvPropOptions_RowDataBound" DataKeyNames="option_name" ShowHeaderWhenEmpty="true">  
            <Columns>  
                <asp:BoundField DataField="option_name" HeaderText="Item" />
            </Columns>  
        </asp:GridView>
      </div>
      <div class="modal-footer">
        <asp:Textbox class="input-xlarge" runat="server" ID="txtOptionsItem"></asp:Textbox>
        <asp:Button class="btn btn-primary" runat="server" ID="btnAddOptionsItem" Text="Add" OnClick="btnOptionsItemSave_Click"/>
          <asp:Button class="btn btn" runat="server" ID="btnOptionsItemDelete" Text="Delete" OnClick="btnOptionsItemDelete_Click"/>
        <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
      </div>
    </div>
    
    <asp:Label runat="server" ID="hdnSelectedRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedObjectName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedStateRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedStateName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedMethodName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedMethodRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedEventName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedEventRow" Visible="false"></asp:Label>
    <asp:HiddenField runat="server" ID="hdnEditingPropOptions"/>
    <asp:HiddenField runat="server" ID="hdnPropOptionsItemName"/>
</asp:Content>
