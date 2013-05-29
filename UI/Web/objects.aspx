<%@ Page Language="C#" AutoEventWireup="true" CodeFile="objects.aspx.cs" Inherits="home" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
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
        function selectPropListItem(name, element) {
            if (lastSelected != null) {
                lastSelected.style.backgroundColor = 'white';
                $(lastSelected).attr("onmouseout", "this.style.cursor='hand';this.style.background='white';");
                $(lastSelected).attr("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            }
            $('#<%=hdnPropListItemName.ClientID%>').val(name);
            element.style.backgroundColor = 'lightblue';
            $(element).removeAttr("onmouseout");
            
            lastSelected = element;
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
        #EditForm {
          padding: 15px;
        }

    </style>
    <div class="row-fluid">
        <div class="span8">
            <div ID="ObjPanel">
                <div class="row-fluid" ID="ObjGrid" style="overflow: auto; max-height:670px;" onscroll="SetDivPosition()">
                    <asp:GridView runat="server" ID="gvObjects"
                        AutoGenerateColumns="False"  
                        GridLines="None"  
                        CssClass="mGrid"  
                        AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvObjects_RowDataBound" DataKeyNames="object_name" ShowHeaderWhenEmpty="true">  
                        <Columns>  
                            <asp:BoundField DataField="container_name" HeaderText="Container" />  
                            <asp:BoundField DataField="object_name" HeaderText="Object" />  
                            <asp:BoundField DataField="object_type" HeaderText="Type" />  
                            <asp:BoundField DataField="state_name" HeaderText="State" />  
                            <asp:BoundField DataField="last_updated" HeaderText="Updated" />  
                            <asp:BoundField DataField="address" HeaderText="Address" />  
                        </Columns>  
                    </asp:GridView>
                </div>
                <br />
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label>Name</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:TextBox class="input-xlarge" runat="server" ID="txtName"></asp:TextBox>
                    </div>
                    <div class="span1" style="text-align:right;">
                        <label>Container</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:DropDownList runat="server" ID="ddlContainer" datatextfield="Text" datavaluefield="Value" style="width:280px;">
                            <asp:ListItem Selected = "True"  Text = "" Value = ""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label>Desc</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:TextBox class="input-xlarge" runat="server" ID="txtDescr"></asp:TextBox>
                    </div>
                    <div class="span1" style="text-align:right;">
                        <label>Address</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:TextBox class="input-xlarge" runat="server" ID="txtAddress" title="The protocal address of the object (not all objects require an address)"></asp:TextBox>
                    </div>
                </div>
                <div class="row-fluid">
                    <div class="span1" style="text-align:right;">
                        <label>Type</label>
                    </div>
                    <div class="span5" style="text-align:left;">
                        <asp:DropDownList runat="server" ID="ddlType" datatextfield="Text" datavaluefield="Value" style="width:280px;">
                            <asp:ListItem Selected = "True" Text = "" Value = ""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="span2" style="text-align:left;">
                        <asp:CheckBox runat="server" ID="chkEnabled" /> Enabled
                    </div>
                    <div class="span4" style="text-align:right;" >
                        <asp:Button runat="server" ID="btnAdd" Text="Add" class="btn" OnClick="btnAdd_Click"/>&nbsp
                        <asp:Button runat="server" ID="btnUpdate" Text="Update" class="btn" OnClick="btnUpdate_Click"/>&nbsp
                        <asp:Button runat="server" ID="btnDelete" Text="Delete" class="btn" OnClick="btnDelete_Click" OnClientClick="return confirm('Are you sure you want to delete the object?');" />
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
                        <div class="span10" ID="propGrid" style="overflow: auto; max-height:500px;"  onscroll="SetPropDivPosition()">
                            <asp:GridView runat="server" ID="gvProperties"
                                AutoGenerateColumns="False"  
                                GridLines="None"  
                                CssClass="mGrid"  
                                AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvProperties_RowDataBound" DataKeyNames="property_name, property_value, property_datatype, object_property_id, last_updated" ShowHeaderWhenEmpty="true">  
                                <Columns>  
                                    <asp:BoundField DataField="property_name" HeaderText="Property" /> 
                                    <asp:BoundField DataField="property_value" HeaderText="Value" /> 
                                    <asp:BoundField DataField="property_datatype" Visible="false" />
                                    <asp:BoundField DataField="object_property_id" Visible="false" /> 
                                    <asp:BoundField DataField="last_updated" Visible="false" /> 
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
                                <strong><asp:Label  runat="server" ID="lblPropName"></asp:Label></strong><br />
                                <asp:Label  runat="server" ID="lblPropLastUpd"></asp:Label><br />
                                <asp:Textbox class="input-xlarge" runat="server" ID="txtPropValue"></asp:Textbox>
                                <asp:DropDownList runat="server" ID="ddlPropValue">
                                </asp:DropDownList>
                                <asp:Button class="btn btn-primary" runat="server" ID="btnPropSave" Text="Save" OnClick="btnPropSave_Click"/>
                                <asp:Button class="btn btn-primary" runat="server" ID="btnEditPropList" Text="Edit List" href="#myPropListModal" data-toggle="modal"/>
                            </form>
                        </asp:Panel>
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
        <asp:Button class="btn btn-primary" runat="server" ID="btnAddListItem" Text="Add" OnClick="btnListItemSave_Click"/>
        <asp:Button class="btn btn" runat="server" ID="btnListItemDelete" Text="Delete" OnClick="btnListItemDelete_Click"/>
        <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
      </div>
    </div>
    
    <asp:Label runat="server" ID="hdnSelectedRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedObjectName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropName" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedPropType" Visible="false"></asp:Label>
    <asp:HiddenField runat="server" ID="hdnEditingPropList"/>
    <asp:HiddenField runat="server" ID="hdnPropListItemName"/>
</asp:Content>