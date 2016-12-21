<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="objtypes.aspx.cs" Inherits="objtypes" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ MasterType virtualpath="~/MasterPage.master" %>
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
    <style type="text/css">
        #gvObjectTypes tr.rowHover:hover {background-color: Yellow;}
        #gvObjectTypes tr.rowHover {background-color: none;}
        #gvObjectTypes tr.rowHoverAlt:hover {background-color: Yellow;}
        #gvObjectTypes tr.rowHoverAlt {background-color: #f4f4f4;}

        #gvProperties tr.rowHover1:hover {background-color: Yellow;}
        #gvProperties tr.rowHover1 {background-color: none;}
        #gvProperties tr.rowHoverAlt1:hover {background-color: Yellow;}
        #gvProperties tr.rowHoverAlt1 {background-color: #f4f4f4;}

        #gvStates tr.rowHover1:hover {background-color: Yellow;}
        #gvStates tr.rowHover1 {background-color: none;}
        #gvStates tr.rowHoverAlt1:hover {background-color: Yellow;}
        #gvStates tr.rowHoverAlt1 {background-color: #f4f4f4;}

        #gvMethods tr.rowHover1:hover {background-color: Yellow;}
        #gvMethods tr.rowHover1 {background-color: none;}
        #gvMethods tr.rowHoverAlt1:hover {background-color: Yellow;}
        #gvMethods tr.rowHoverAlt1 {background-color: #f4f4f4;}

        #gvEvents tr.rowHover1:hover {background-color: Yellow;}
        #gvEvents tr.rowHover1 {background-color: none;}
        #gvEvents tr.rowHoverAlt1:hover {background-color: Yellow;}
        #gvEvents tr.rowHoverAlt1 {background-color: #f4f4f4;}
    </style>

    <div class="row-fluid">
        <div class="span6">
            <div ID="ObjPanel">
                <div class="row-fluid" ID="ObjGrid" style="overflow: auto; max-height:670px; " onscroll="SetDivPosition()">
                    <asp:GridView runat="server" ID="gvObjectTypes" AllowSorting="True" OnSorting="gvObjectTypes_OnSorting"
                        AutoGenerateColumns="False" SelectedIndex ="0" GridLines="None" CssClass="mGrid" ClientIDMode="Static" OnRowDataBound="gvObjectTypes_RowDataBound" 
                        DataKeyNames="object_type">
                        <RowStyle CssClass="rowHover"></RowStyle>
                        <SelectedRowStyle backcolor="lightblue" BorderStyle="Outset" BorderWidth="1px"></SelectedRowStyle>  
                        <AlternatingRowStyle CssClass="rowHoverAlt"></AlternatingRowStyle>
                        <Columns>  
                            <asp:BoundField DataField="base_type" HeaderText="Base Type" SortExpression="base_type"/>  
                            <asp:BoundField DataField="object_type" HeaderText="Object Type" SortExpression="object_type"/>  
                            <asp:BoundField DataField="object_type_description" HeaderText="Description" SortExpression="object_type_description"/> 
                            <asp:BoundField DataField="object_type_tooltip" HeaderText="Tooltip" Visible="false" />
                        </Columns>  
                    </asp:GridView>
                </div>
                <br />
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label title="The proper Name of this object (This is a REQUIRED property!)">Name</label>
                    </div>
                    <div class="span4" style="text-align:left;">
                        <asp:TextBox class="input" runat="server" ID="txtName" style="width:100%;" ToolTip="The proper Name of this object (This is a REQUIRED property!)"></asp:TextBox>
                    </div>
                    <div class="span2" style="text-align:right;">
                        <label title="The controling object for this Object (All object should be Owned!)">Owned By</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:DropDownList runat="server" ID="ddlOwnedBy" datatextfield="Text" datavaluefield="Value" style="width:100%;" ToolTip="The controling object for this Object (All object should be Owned!)">
                            <asp:ListItem Selected = "True" Text = "" Value = ""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label title="A description that describes this object (This property is Optional)">Desc</label>
                    </div>
                    <div class="span4" style="text-align:left;">
                        <asp:TextBox class="input" runat="server" ID="txtDescr" style="width:100%;" ToolTip="A description that describes this object (This property is Optional)"></asp:TextBox>
                    </div>
                    <div class="span2" style="text-align:right;">
                        <label title="This is the Base-Type for this object. (All Objects require a Base-Type)">Base Type</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:DropDownList runat="server" ID="ddlBaseType" datatextfield="Text" datavaluefield="Value" style="width:100%;" ToolTip="This is the Base-Type for this object. (All Objects require a Base-Type)">
                            <asp:ListItem Selected = "True" Text = "" Value = ""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row-fluid">
                   Tooltip:  <asp:TextBox class="input" runat="server" ID="txtObjectTypeTooltip" TextMode="MultiLine" Rows="2" Height="39px" Width="415px" ToolTip="Enter Help Text to be displayed on the Objects page for this object."></asp:TextBox>
                </div>
                <div class="row-fluid">
                    <div class="span7" style="text-align:left;">
                        <asp:CheckBox runat="server" ID="chkOwner" ToolTip="Select if this object will be an Object Owner Type." /> Object Type Owner &nbsp;
                        <asp:CheckBox runat="server" ID="chkContainer" ToolTip="Select if this object will be a Container object." /> Container &nbsp;
                        <br />
                        <asp:CheckBox runat="server" ID="chkSysType" ToolTip="Select if this object will be a Rquired Type." /> Required Type &nbsp;
                        <asp:CheckBox runat="server" ID="chkHideEvents" ToolTip="Select to hide all Redundent Events." /> Hide Redundant Events &nbsp;
                    </div>
                    <div class="span5" style="text-align:center;" >
                        <asp:Button runat="server" ID="btnAdd" Text="Add" class="btn" OnClick="btnAdd_Click" ToolTip="This will ADD the newly enter Object information above." />&nbsp
                        <asp:Button runat="server" ID="btnUpdate" Text="Update" class="btn" OnClick="btnUpdate_Click" ToolTip="This will UPDATE the above Object information." />&nbsp
                        <asp:Button runat="server" ID="btnDelete" Text="Delete" class="btn" OnClick="btnDelete_Click" OnClientClick="return confirm('Are you sure you want to delete the object type?');" ToolTip="This will Delete the above selected Object." />
                        <a href="#linkModal" role="button" class="btn" data-toggle="modal" title="This display the Export SQL for the above selected object."  >Export</a>
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
                           <asp:GridView runat="server" ID="gvStates" AutoGenerateColumns="False" SelectedIndex ="0" GridLines="None" 
                               CssClass="mGrid"  ClientIDMode="Static" OnRowDataBound="gvStates_RowDataBound" 
                               DataKeyNames="state_name, state_label, state_tooltip" ShowHeaderWhenEmpty="true">  
                                <RowStyle CssClass="rowHover1"></RowStyle>
                                <SelectedRowStyle backcolor="lightblue" BorderStyle="Outset" BorderWidth="1px"></SelectedRowStyle>  
                                <AlternatingRowStyle CssClass="rowHoverAlt1"></AlternatingRowStyle> 
                                <Columns>  
                                    <asp:BoundField DataField="state_name" HeaderText="Name" /> 
                                    <asp:BoundField DataField="state_label" HeaderText="Label" /> 
                                    <asp:BoundField DataField="state_tooltip" HeaderText="Tooltip" Visible="false" /> 
                                </Columns>  
                            </asp:GridView>
                        </div>
                        <div class="span6">
                            <asp:Panel runat="server" ID="pnlStateForm">
                                    Name: <asp:TextBox  runat="server" ID="txtStateName" style="width:225px;" ToolTip="Enter the Name of the State. (On/Off)"></asp:TextBox><br />
                                    Label: <asp:TextBox  runat="server" ID="txtStateLabel" style="width:225px;" ToolTip="Enter the Stae Lable. (Running/Stopped, Online/Offline)"></asp:TextBox><br />
                                    Tooltip: <asp:TextBox  runat="server" ID="txtStateTooltip" TextMode="MultiLine" Rows="2" Height="39px" Width="215px" ToolTip="Enter Help Text to be displayed on the Objects page for this object's state."></asp:TextBox><br />
                                    <asp:Button class="btn btn" runat="server" ID="btnStateSave" Text="Save" OnClick="btnStateSave_Click" ToolTip="This will UPDATE the State information above." />
                                    <asp:Button class="btn btn" runat="server" ID="btnStateAdd" Text="Add" OnClick="btnStateAdd_Click" ToolTip="This will ADD the newly entered State information above." />
                                    <asp:Button class="btn btn" runat="server" ID="btnStateDelete" Text="Delete" OnClick="btnStateDelete_Click"  ToolTip="This will DELETE the selected State above." />
                           </asp:Panel>
                        </div>
                    </div>
                </div>
                <div  runat="server" ID="divMethods" class="hero-unit" style="padding: 10px; padding-right: 0px; margin-bottom: 10px;">
                    <div class="row-fluid">
                        <h3 style="float:right; margin-right:10px;">Methods</h3>
                        <br />
                        <div class="span5" id="methodGrid" style="overflow: auto;max-height:300px;"  onscroll="SetMethodDivPosition()">
                            <asp:GridView runat="server" ID="gvMethods" AutoGenerateColumns="False" SelectedIndex ="0" GridLines="None"  
                                CssClass="mGrid" ClientIDMode="Static" OnRowDataBound="gvMethods_RowDataBound" 
                                DataKeyNames="method_name, method_label, param_1_label, param_2_label, param_1_default, param_2_default, method_tooltip" ShowHeaderWhenEmpty="true">  
                                <RowStyle CssClass="rowHover1"></RowStyle>
                                <SelectedRowStyle backcolor="lightblue" BorderStyle="Outset" BorderWidth="1px"></SelectedRowStyle>  
                                <AlternatingRowStyle CssClass="rowHoverAlt1"></AlternatingRowStyle> 
                                <Columns>  
                                    <asp:BoundField DataField="method_name" HeaderText="Name" /> 
                                    <asp:BoundField DataField="method_label" HeaderText="Label" />  
                                    <asp:BoundField DataField="param_1_label" Visible="false" />  
                                    <asp:BoundField DataField="param_2_label" Visible="false" /> 
                                    <asp:BoundField DataField="param_1_default" Visible="false" />  
                                    <asp:BoundField DataField="param_2_default" Visible="false" /> 
                                    <asp:BoundField DataField="method_tooltip" Visible="false" /> 
                                </Columns>  
                            </asp:GridView>
                        </div>
                        <div class="span6">
                            <asp:Panel runat="server" ID="pnlMethodForm">
                                    Name: <asp:TextBox  runat="server" ID="txtMethodName" style="width:225px;" ToolTip="Enter the name of this Method"></asp:TextBox><br />
                                    Label: <asp:TextBox  runat="server" ID="txtMethodLabel" style="width:225px;" ToolTip="Enter the Lable for this method"></asp:TextBox><br />
                                    Parameter Labels / Default Values<br />
                                    <asp:TextBox  runat="server" ID="txtParam1Label" style="width:100px;" ToolTip="Enter Parmeter-1 Lable"></asp:TextBox><asp:TextBox  runat="server" ID="txtParam1Default" style="width:100px;" ToolTip="Enter Parmeter-1 Default Value"></asp:TextBox><br />
                                    <asp:TextBox  runat="server" ID="txtParam2Label" style="width:100px;" ToolTip="Enter Parameter-2 Lable"></asp:TextBox><asp:TextBox  runat="server" ID="txtParam2Default" style="width:100px;" ToolTip="Enter Parameter-2 Default Value"></asp:TextBox><br />
                                    Tooltip: <asp:TextBox  runat="server" ID="txtMethodTooltip" TextMode="MultiLine" Rows="2" Height="39px" Width="215px" ToolTip="Enter Help Text to be displayed on the Objects page for this object's method."></asp:TextBox><br />
                                    <asp:Button class="btn btn" runat="server" ID="btnMethodSave" Text="Save" OnClick="btnMethodSave_Click" ToolTip="This will UPDATE Method information above." />
                                    <asp:Button class="btn btn" runat="server" ID="btnMethodAdd" Text="Add" OnClick="btnMethodAdd_Click" ToolTip="This will ADD the newly entered Method information above." />
                                    <asp:Button class="btn btn" runat="server" ID="btnMethodDelete" Text="Delete" OnClick="btnMethodDelete_Click" ToolTip="This will DELETE the selected Method above." />
                            </asp:Panel>
                        </div>
                    </div>
                </div>
                <div  runat="server" ID="divEvents" class="hero-unit" style="padding: 10px; padding-right: 0px; margin-bottom: 10px;">
                    <div class="row-fluid">
                        <h3 style="float:right; margin-right:10px;">Events</h3>
                        <br />
                        <div class="span5" ID="eventGrid" style="overflow: auto; max-height:300px;"  onscroll="SetEventDivPosition()">
                            <asp:GridView runat="server" ID="gvEvents" AutoGenerateColumns="False" SelectedIndex ="0" GridLines="None"  
                                CssClass="mGrid" ClientIDMode="Static" OnRowDataBound="gvEvents_RowDataBound" 
                                DataKeyNames="event_name, event_label, event_tooltip" ShowHeaderWhenEmpty="true"> 
                                <RowStyle CssClass="rowHover1"></RowStyle>
                                <SelectedRowStyle backcolor="lightblue" BorderStyle="Outset" BorderWidth="1px"></SelectedRowStyle>  
                                <AlternatingRowStyle CssClass="rowHoverAlt1"></AlternatingRowStyle> 
                                <Columns>  
                                    <asp:BoundField DataField="event_name" HeaderText="Name" /> 
                                    <asp:BoundField DataField="event_label" HeaderText="Label" /> 
                                    <asp:BoundField DataField="event_tooltip" HeaderText="Label" Visible="false"/> 
                                </Columns>  
                            </asp:GridView>
                        </div>
                        <div class="span6">
                            <asp:Panel runat="server" ID="pnlEventForm">
                                    Name: <asp:TextBox  runat="server" ID="txtEventName" style="width:225px;" ToolTip="Enter the Name of this Event"></asp:TextBox><br />
                                    Label: <asp:TextBox  runat="server" ID="txtEventLabel" style="width:225px;" ToolTip="Enter the Label of this Event"></asp:TextBox><br />
                                    Tooltip: <asp:TextBox  runat="server" ID="txtEventTooltip" TextMode="MultiLine" Rows="2" Height="39px" Width="215px" ToolTip="Enter Help Text to be displayed on the Objects page for this object's event."></asp:TextBox><br />
                                    <asp:Button class="btn btn" runat="server" ID="btnEventSave" Text="Save" OnClick="btnEventSave_Click" ToolTip="This will UPDATE the Event information above."/>
                                    <asp:Button class="btn btn" runat="server" ID="btnEventAdd" Text="Add" OnClick="btnEventAdd_Click" ToolTip="This will ADD the newly entered Event information above."/>
                                    <asp:Button class="btn btn" runat="server" ID="btnEventDelete" Text="Delete" OnClick="btnEventDelete_Click" ToolTip="This will DELETE the selected Event above."/>
                           </asp:Panel>
                        </div>
                    </div>
                </div>
                <div  runat="server" ID="divProps" class="hero-unit" style="padding: 10px; padding-right: 0px; margin-bottom: 10px;">
                    <div class="row-fluid">
                        <h3 style="float:right; margin-right:10px;">Properties</h3>
                        <br />
                        <div class="span5" id="propGrid" style="overflow: auto; max-height:300px;" onscroll="SetPropDivPosition()">
                            <asp:GridView runat="server" ID="gvProperties" AutoGenerateColumns="False" SelectedIndex ="0" GridLines="None" 
                                CssClass="mGrid" ClientIDMode="Static" OnRowDataBound="gvProperties_RowDataBound" 
                                DataKeyNames="property_name, property_datatype, property_object_type, property_default, property_tooltip, track_history, property_required, property_id" ShowHeaderWhenEmpty="true"> 
                                <RowStyle CssClass="rowHover1"></RowStyle>
                                <SelectedRowStyle backcolor="lightblue" BorderStyle="Outset" BorderWidth="1px"></SelectedRowStyle>  
                                <AlternatingRowStyle CssClass="rowHoverAlt1"></AlternatingRowStyle> 
                                <Columns>  
                                    <asp:BoundField DataField="property_name" HeaderText="Property" /> 
                                    <asp:BoundField DataField="property_datatype" HeaderText="Type" /> 
                                    <asp:BoundField DataField="property_object_type" visible="false" />
                                    <asp:BoundField DataField="property_default" visible="false" />
                                    <asp:BoundField DataField="property_tooltip" visible="false" />
                                    <asp:BoundField DataField="track_history" visible="false" />
                                    <asp:BoundField DataField="property_required" visible="false" />
                                </Columns>  

                            </asp:GridView>
                        </div>
                        <div class="span6">
                            <asp:Panel runat="server" ID="panelPropForm">
                                Name: <asp:TextBox  runat="server" ID="txtPropName" style="width:225px;" ToolTip="Enter the Name of this Property"></asp:TextBox>
                                <br />
                                Type: <asp:DropDownList runat="server" ID="ddlPropType" datatextfield="Text" datavaluefield="Value" style="width:200px;"  AutoPostBack="true" OnSelectedIndexChanged="ddlPropType_SelectedIndexChanged" ToolTip="Choose the Data Type of this property.">
                                        <asp:ListItem Selected = "True" Text = "String" Value = "String"></asp:ListItem>
                                        <asp:ListItem Text = "Boolean" Value = "Boolean"></asp:ListItem>
                                        <asp:ListItem Text = "DateTime" Value = "DateTime"></asp:ListItem>
                                        <asp:ListItem Text = "File" Value = "File"></asp:ListItem>
                                        <asp:ListItem Text = "Float" Value = "Float"></asp:ListItem>
                                        <asp:ListItem Text = "Integer" Value = "Integer"></asp:ListItem>
                                        <asp:ListItem Text = "List" Value = "List"></asp:ListItem>
                                        <asp:ListItem Text = "Object" Value = "Object"></asp:ListItem>
                                        <asp:ListItem Text = "Object Type" Value = "Object Type"></asp:ListItem>
                                        <asp:ListItem Text = "Password" Value = "Password"></asp:ListItem>
                                </asp:DropDownList>
                                <br />
                                <asp:Label runat="server" ID="lblPropObjectType" Text="Object Type: "></asp:Label>
                                <asp:DropDownList runat="server" ID="ddlBaseType2" datatextfield="Text" datavaluefield="Value" style="width:200px;" ToolTip="Select the Object-Type for this Property.">
                                    <asp:ListItem Selected = "True" Text = "" Value = ""></asp:ListItem>
                                </asp:DropDownList>
                                <br />
                                Default: <asp:TextBox  runat="server" ID="txtPropDefault" style="width:215px;" ToolTip="Enter the Default Value for this Property."></asp:TextBox>
                                <br />
                                ToolTip: <asp:TextBox runat="server" ID="txtPropertyTooltip" TextMode="MultiLine" Rows="2" Height="39px" Width="215px" ToolTip="Enter Help Text to be displayed on the Objects page for this object's property."></asp:TextBox>
                                <br />
                                <asp:CheckBox runat="server" ID="chkTrackChanges" ToolTip="Enable/Disable the Tracking of changes for this property." /> Track Changes &nbsp;
                                <br />
                                <asp:CheckBox runat="server" ID="chkRequired" tooltip="Select this option to make this Property REQUIRED. (Not Empty)"/> Required &nbsp;
                                <br />
                                <asp:Button class="btn btn" runat="server" ID="btnPropSave" Text="Save" OnClick="btnPropSave_Click" ToolTip="This will UPDATE the Property information above."/>
                                <asp:Button class="btn btn" runat="server" ID="btnPropAdd" Text="Add" OnClick="btnPropAdd_Click" ToolTip="This will ADD the newly entered Property information above."/>
                                <asp:Button class="btn btn" runat="server" ID="btnPropDelete" Text="Delete" OnClick="btnPropDelete_Click" ToolTip="This will DELETE the selected Property above."/>
                                <asp:Button class="btn btn" runat="server" ID="btnEditPropOptions" Text="Edit Options" href="#myPropOptionsModal" data-toggle="modal" ToolTip="This will allow editing of this Properties Options."/>
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
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×
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
    
    <!-- Modal -->
    <div id="linkModal" class="modal hide" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
            <h3 id="H1">Object Type Export Script</h3>
        </div>
        <div id="exportModalBody" class="modal-body">
            <asp:TextBox ID="lblExportScript" runat="server" TextMode="MultiLine" Font-Size="Smaller"></asp:TextBox>
        </div>
    </div>

    <asp:Label runat="server" ID="hdnSelectedObjectName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropDataType" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedStateRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedStateName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedMethodName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedMethodRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedEventName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedEventRow" Visible="false"></asp:Label>
    <asp:HiddenField runat="server" ID="hdnEditingPropOptions"/>
    <asp:HiddenField runat="server" ID="hdnPropOptionsItemName"/>
</asp:Content>
