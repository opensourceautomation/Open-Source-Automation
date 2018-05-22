<%@ Page Language="C#" AutoEventWireup="true" CodeFile="objects.aspx.cs" Inherits="home" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ MasterType virtualpath="~/MasterPage.master" %>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder">
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
        }
        function SetDivPosition() {
            var intY = document.getElementById("ObjGrid").scrollTop;
            document.cookie = "yPos=!~" + intY + "~!";
        }
        function SetPropDivPosition() {
            var intY = document.getElementById("propGrid").scrollTop;
            document.cookie = "yPos=$~" + intY + "~$";
        }
        function selectPropListItem(name, label, element) {
            if (lastSelected != null) {
                lastSelected.style.backgroundColor = 'white';
                $(lastSelected).attr("onmouseout", "this.style.cursor='hand';this.style.background='white';");
                $(lastSelected).attr("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            }
            lastSelected = element;
            $('#<%=hdnPropListItemName.ClientID%>').val(name);
            element.style.backgroundColor = 'lightblue';
            $(element).removeAttr("onmouseout");
            var strName = name;
            strName = strName.replace("^^^", "'");
            $('#<%=txtListItem.ClientID%>').val(strName);

            var strLabel = label;
            strLabel = strLabel.replace("^^^", "'");
            $('#<%=txtListItemLabel.ClientID%>').val(strLabel);
            
        }
        $(function () {
            if ($('#<%=hdnEditingPropList.ClientID%>').val() == "1") {
                $('#<%=hdnEditingPropList.ClientID%>').val("0");
                $('#myPropListModal').modal('show');
            }
        });
    </script> 
    <script>
        $(function () {
            $(document).tooltip();
        });
    </script>
    <style>
        #EditForm
        {
          padding: 15px;
        }
    </style>
    <style type="text/css">
        #gvObjects tr.rowHover:hover {background-color: Yellow;}
        #gvObjects tr.rowHover {background-color: none;}
        #gvObjects tr.rowHoverAlt:hover {background-color: Yellow;}
        #gvObjects tr.rowHoverAlt {background-color: #f4f4f4;}
        #gvProperties tr.rowHover1:hover {background-color: Yellow;}
        #gvProperties tr.rowHover1 {background-color: none;}
        #gvProperties tr.rowHoverAlt1:hover {background-color: Yellow;}
        #gvProperties tr.rowHoverAlt1 {background-color: #f4f4f4;}
    </style>

    <div class="row-fluid">
        <div class="span8">
            <div ID="ObjPanel">
                <div class="row-fluid" ID="ObjGrid" style="overflow: auto; max-height:600px;" onscroll="SetDivPosition()">
                    <asp:GridView runat="server" ID="gvObjects" AllowSorting="True" OnSorting="gvObjects_OnSorting"
                        AutoGenerateColumns="False" SelectedIndex ="0" GridLines="None" CssClass="mGrid" ClientIDMode="Static"
                        OnRowDataBound="gvObjects_RowDataBound" DataKeyNames="object_name, object_type_tooltip" ShowHeaderWhenEmpty="true" >
                        <RowStyle CssClass="rowHover"></RowStyle>
                        <SelectedRowStyle backcolor="lightblue" BorderStyle="Outset" BorderWidth="1px"></SelectedRowStyle>  
                        <AlternatingRowStyle CssClass="rowHoverAlt"></AlternatingRowStyle>
                        <Columns>
                            <asp:BoundField DataField="container_name" HeaderText="Container" SortExpression="container_name"/>
                            <asp:BoundField DataField="object_name" HeaderText="Object" SortExpression="object_name"/>
			    <asp:BoundField DataField="object_type" HeaderText="Type" SortExpression="object_type" />
                            <asp:BoundField DataField="state_label" HeaderText="State" SortExpression="state_label" />
                            <asp:BoundField DataField="last_updated" HeaderText="Updated" ItemStyle-Width="10em" SortExpression="last_updated" />

                            <asp:BoundField DataField="address" HeaderText="Address" SortExpression="address" />
                            <asp:BoundField DataField="object_type_tooltip" Visible="false" />
                        </Columns>
                    </asp:GridView>
                </div>
                <br />
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label title="The proper Name of this object (This is a REQUIRED property!)">Name</label>
                    </div>
                    <div class="span4" style="text-align:left;">
                        <asp:TextBox class="input-xlarge" runat="server" ID="txtName" title="The proper Name of this object (This is a REQUIRED property!)"></asp:TextBox>
                    </div>
                    <div class="span2" style="text-align:right;">
                        <label title="The container in which this object resides (not all objects require a Container)">Container</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:DropDownList runat="server" ID="ddlContainer" datatextfield="Text" datavaluefield="Value" style="width:280px;" title="The container in which this object resides (not all objects require a Container)">
                            <asp:ListItem Selected = "True"  Text = "" Value = ""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label title="The Alias or Nickname of this object. (This is currently optional)">Alias</label>
                    </div>
                    <div class="span4" style="text-align:left;">
                        <asp:TextBox class="input-xlarge" runat="server" ID="txtAlias" title="The Alias or Nickname of this object. (This is currently optional)"></asp:TextBox>
                    </div>
                    <div class="span2" style="text-align:right;">
                        <label title="The protocol address of the object (not all objects require an address)">Address</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:TextBox class="input-xlarge" runat="server" ID="txtAddress" title="The protocol address of the object (not all objects require an address)"></asp:TextBox>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label title="The Object-Type of this object (All objects require a Type)">Type</label>
                    </div>
                    <div class="span4" style="text-align:left;">
                        <asp:DropDownList runat="server" ID="ddlType" datatextfield="Text" datavaluefield="Value" style="width:280px;" title="The Object-Type of this object (All objects require a Type)">
                            <asp:ListItem Selected = "True" Text = "" Value = ""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="span2" style="text-align:right;">
                        <label title="The controling object for this Object (All objects should be owned)">Owned By</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:TextBox class="input-xlarge" runat="server" ID="txtOwned" disabled="true" title="The controling object for this Object (All objects should be owned)"></asp:TextBox>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label title="A description that describes this object (This is currently optional)">Desc</label>
                    </div>
                    <div class="span4" style="text-align:left;">
                        <asp:TextBox class="input-xlarge" runat="server" ID="txtDescr" title="A description that describes this object (This is currently optional)"></asp:TextBox>
                    </div>

                    <div class="span2" style="text-align:right;" >
                        <label title="The minimal trust level an object must have to make changes to this object (All objects should have a Mim Trust Level)">Min Trust Level</label>
                    </div>
                    <div class="span1" style="text-align:left;">
                        <asp:TextBox class="input" runat="server" ID="txtTrustLevel" Width="40px" title="The minimal trust level an object must have to make changes to this object (All objects should have a Mim Trust Level)"></asp:TextBox>
                    </div>
                    <div class="span4" style="text-align:left;" >
                        <asp:CheckBox runat="server" ID="chkEnabled" title="Set if this object is Enabled or Disabled." /> Enabled 
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="span5" style="text-align:right;" >
                    </div>
                    <div class="span5" style="text-align:right;" >
                        <asp:Button runat="server" ID="btnAdd" Text="<%$ Resources:Resources, Add %>" class="btn" OnClick="btnAdd_Click" title="Adds a new object to the Database."/>&nbsp
                        <asp:Button runat="server" ID="btnUpdate" Text="<%$ Resources:Resources, Update %>" class="btn" OnClick="btnUpdate_Click" Visible="false" title="Updates the above selected object."/>&nbsp
                        <asp:Button runat="server" ID="btnDelete" Text="<%$ Resources:Resources, Delete %>" class="btn" OnClick="btnDelete_Click" OnClientClick="return confirm('Are you sure you want to delete the object?');" title="Deletes the above selected object."/>
                        <a href="#linkModal" role="button" class="btn" data-toggle="modal" title="Creates an Export text for the above selected object.">Export</a><br />
                        <asp:Label runat="server" ID="objAddError" Text="Invalid Entry, please try again!" ForeColor="Red" /><br />
                        <asp:Label runat="server" ID="objAddErrorMsg" Text="Some information may still be missing!" ForeColor="Orange" />
                    </div>
                    <div class="span2" style="text-align:right;" >
                    </div>
                </div>
            </div>
        </div>
        <div class="span4">
            <asp:Panel ID="panelEditForm" runat="server" Visible="false">
                <div ID="EditForm" class="hero-unit">
                    <div class="alert alert-success" runat="server" id="alert" visible="false">
                      <asp:Label runat="server" ID="lblAlert"></asp:Label>
                    </div>
                    <div class="row-fluid" runat="server" id="divState">
                        <div class="span3" style="text-align:right;">
                            State:
                        </div>
                        <div class="span9" style="text-align:left;">
                            <asp:DropDownList runat="server" ID="ddlState" datatextfield="Text" datavaluefield="Value" style="width:100%;" AutoPostBack="true" OnSelectedIndexChanged="ddlState_SelectedIndexChanged">                          
                                <asp:ListItem Selected = "True" Text = "" Value = ""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row-fluid" runat="server" id="divMethod">
                        <div class="span3" style="text-align:right;">
                            Method:
                        </div>
                        <div class="span9" style="text-align:left;">
                            <asp:DropDownList runat="server" ID="ddlMethod" datatextfield="Text" datavaluefield="Value" style="width:100%;" AutoPostBack="true" OnSelectedIndexChanged="ddlMethod_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row-fluid" runat="server" id="divParameters" visible ="False">
                        <div class="span6">
                            Parameter 1 <asp:Label  runat="server" ID="lblParam1"></asp:Label>
                            <br />
                            <asp:TextBox class="input-xlarge" runat="server" ID="txtParam1"></asp:TextBox>
                        </div>
                        <div class="span6">
                            Parameter 2 <asp:Label  runat="server" ID="lblParam2"></asp:Label>
                            <br />
                            <asp:TextBox class="input-xlarge" runat="server" ID="txtParam2"></asp:TextBox>
                        </div>
                        <br />
                        <asp:Button class="btn" runat="server" ID="btnExecute" Text="Execute" OnClick="btnExecute_Click"/>
                    </div>
                    <div class="row-fluid" runat="server" id="divEvent">
                        <div class="span3" style="text-align:right;">
                            Event:
                        </div>
                        <div class="span9" style="text-align:left;">
                            <asp:DropDownList runat="server" ID="ddlEvent" datatextfield="Text" datavaluefield="Value" style="width:100%;" AutoPostBack="true" OnSelectedIndexChanged="ddlEvent_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                    </div>
                    <br />
                    <div class="row-fluid">
                        <div class="span1"></div>
                        <div class="span10" ID="propGrid" style="overflow: auto; max-height:500px;" onscroll="SetPropDivPosition()">
                        <asp:Label runat="server" ID="propEmpty" Text="This Object has Empty Properties!" Visible="false" ForeColor="Orange" /><br />
                        <asp:Label runat="server" ID="propError" Text="This Object is missing Required Properties!" Visible="false" ForeColor="Red" />
                            <asp:GridView runat="server" ID="gvProperties" AutoGenerateColumns="False" GridLines="None" CssClass="mGrid"  
                                OnRowDataBound="gvProperties_RowDataBound" ClientIDMode="Static"
                                DataKeyNames="property_name,property_value,property_datatype,object_property_id,property_object_type,last_updated,source_name,trust_level,interest_level,property_tooltip,property_required" ShowHeaderWhenEmpty="True" AllowSorting="True">  
                                <RowStyle CssClass="rowHover1"></RowStyle>
                                <SelectedRowStyle backcolor="lightblue" BorderStyle="Outset" BorderWidth="1px"></SelectedRowStyle>  
                                <AlternatingRowStyle CssClass="rowHoverAlt1"></AlternatingRowStyle>
                                <Columns>
                                    <asp:BoundField DataField="property_name" HeaderText="Property" />

                                    <asp:BoundField DataField="property_value" HeaderText="Value" />
                                    <asp:BoundField DataField="property_datatype" Visible="false" />
                                    <asp:BoundField DataField="object_property_id" Visible="false" />
                                    <asp:BoundField DataField="property_object_type" Visible="false" />
                                    <asp:BoundField DataField="last_updated" Visible="false" />
                                    <asp:BoundField DataField="source_name" Visible="false" />
                                    <asp:BoundField DataField="trust_level" Visible="false" />
                                    <asp:BoundField DataField="interest_level" Visible="false" />
                                    <asp:BoundField DataField="property_tooltip" Visible="false" />
                                    <asp:BoundField DataField="property_required" Visible="false" />
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="span1"></div>
                    </div>
                    <br />
                    <div class="row-fluid">
                        <div class="span1"></div>
                        <div class="span10">
                        <asp:Panel runat="server" ID="panelPropForm" Visible ="false">
                            <form class="form-inline">
                                <asp:Label ID="lblPropType" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lblTrustLevel" runat="server"></asp:Label>
                                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblInterestLevel" runat="server"></asp:Label>
                                <br />
                                <asp:Label ID="lblPropLastUpd" runat="server"></asp:Label>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lblSourceName" runat="server"></asp:Label>
                                <br />
                                <strong><asp:Label ID="lblPropName" runat="server"></asp:Label>&nbsp;&nbsp;
                                <asp:Label ID="lblRequired" runat="server" Font-Bold="True" ForeColor="Red" Visible="False">- Required</asp:Label><br /></strong>
                                <asp:Textbox class="input-xlarge" runat="server" ID="txtPropValue"></asp:Textbox>
                                <asp:DropDownList runat="server" ID="ddlPropValue">
                                </asp:DropDownList>
                                <asp:Button class="btn btn-primary" runat="server" ID="btnPropSave" Text="Save" OnClick="btnPropSave_Click"/>
                                <asp:Button class="btn btn-primary" runat="server" ID="btnEditPropList" Text="Edit List" href="#myPropListModal" data-toggle="modal"/>
                            </form>
                        </asp:Panel>
                            <asp:Label runat="server" ID="propSaveError" Text="Invalid Entry, please try again!" Visible="false" ForeColor="Red" />
                        </div>
                        <div class="span1"></div>
                    </div>
                </div>
            </asp:Panel> 
        </div>
    </div>

    <!-- Property List Modal -->
    <div id="myPropListModal" class="modal hide" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
        <h3 id="myModalLabel">Edit Property List</h3>
      </div>
      <div ID="dvmodalbody" class="modal-body">
        <asp:GridView runat="server" ID="gvPropList"
            AutoGenerateColumns="False"  
            GridLines="None"  
            CssClass="mGrid" 
            AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvPropList_RowDataBound" DataKeyNames="item_name,item_label" ShowHeaderWhenEmpty="true">  
            <Columns>  
                <asp:BoundField DataField="item_name" HeaderText="Item" ItemStyle-Width="50%" />
                <asp:BoundField DataField="item_label" HeaderText="Label" />
            </Columns>  
        </asp:GridView>
      </div>
      <div class="modal-footer">
        <div class="row-fluid">
            <div class="span6" style="text-align:left;">
                <asp:Textbox class="input-large" runat="server" ID="txtListItem"></asp:Textbox>
            </div>
            <div class="span6" style="text-align:left;">
                <asp:Textbox class="input-large" runat="server" ID="txtListItemLabel"></asp:Textbox>
            </div>
        </div>
          
        <br />
        <asp:Button class="btn btn-primary" runat="server" ID="btnAddListItem" Text="Add" OnClick="btnListItemAdd_Click"/>

        <asp:Button class="btn btn" runat="server" ID="btnListItemUpdate" Text="Update" OnClick="btnListItemUpdate_Click"/>

        <asp:Button class="btn btn" runat="server" ID="btnListItemDelete" Text="Delete" OnClick="btnListItemDelete_Click"/>
        <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
      </div>
    </div>

        <!-- Modal -->
    <div id="linkModal" class="modal hide" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
            <h3 id="H1">Object Export</h3>
        </div>
        <div id="exportModalBody" class="modal-body">
            <asp:TextBox ID="txtExportScript" runat="server" TextMode="MultiLine" Font-Size="Smaller"></asp:TextBox>
        </div>
      <div class="modal-footer">
        <asp:Button runat="server" ID="btnExport" Text="Export" class="btn" OnClick="btnExport_Click" ToolTip="Exports selected Object to an SQL or ZIP file." />
          <button type="button" class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
      </div>
    </div>
    
    <asp:Label runat="server" ID="hdnSelectedObjectName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropType" Visible="false"></asp:Label>
    <asp:HiddenField runat="server" ID="hdnEditingPropList"/>
    <asp:HiddenField runat="server" ID="hdnPropListItemRow"/>
    <asp:HiddenField runat="server" ID="hdnPropListItemName"/>

</asp:Content>