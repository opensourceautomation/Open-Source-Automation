<%@ Page Language="C#" AutoEventWireup="true" CodeFile="objects.aspx.cs" Inherits="home" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder">
    <script type="text/javascript">
       
    </script> 
    <style>
        #EditForm {
          position: fixed;
          top: 65px;
          right: 20px;
          bottom: 35px; 
          overflow: auto;
          padding: 15px;
          height:90%; 
        }

        #ObjGrid
        {
            height:90%; 
            overflow: auto; 
            position: fixed; 
            bottom: 35px; 
            top: 65px;
        }
    </style>
    <div class="row-fluid">
        <div class="span8" ID="ObjGrid" >
            <asp:GridView runat="server" ID="gvObjects"
                AutoGenerateColumns="False"  
                GridLines="None"  
                CssClass="mGrid"  
                AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvObjects_RowDataBound" DataKeyNames="object_name">  
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
        <div class="span4">
            <asp:Panel ID="panelEditForm" runat="server" Visible="false">
                <div ID="EditForm" class="hero-unit" style="width:30%; ">
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
                        <div class="span10">
                        <asp:GridView runat="server" ID="gvProperties"
                            AutoGenerateColumns="False"  
                            GridLines="None"  
                            CssClass="mGrid"  
                            AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvProperties_RowDataBound" DataKeyNames="property_name">  
                            <Columns>  
                                <asp:BoundField DataField="property_name" HeaderText="Property" /> 
                                <asp:BoundField DataField="property_value" HeaderText="Value" /> 
                            </Columns>  

                        </asp:GridView>

                        </div>
                        <div class="span1"></div>
                    </div>
                </div>
            </asp:Panel> 
        </div>
    </div>


    
    <asp:Label runat="server" ID="hdnSelectedRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedObjectName" Visible="false"></asp:Label>
</asp:Content>