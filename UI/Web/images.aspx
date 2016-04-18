<%@ Page Language="C#" AutoEventWireup="true" CodeFile="images.aspx.cs" Inherits="images" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ MasterType virtualpath="~/MasterPage.master" %>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder">
<script type="text/javascript">
function callme()
  {
    document.getElementById('<%=txtName.ClientID%>').value = document.getElementById('<%=fileUpload.ClientID %>').value.replace(/^.*[\\\/]/, '').replace(/\.[^/.]+$/, "");
            
  }
</script>

    <div class="row-fluid">
        <div class="span2">
        </div>
        <div class="span8">
            <center>
                <asp:GridView runat="server" ID="gvImages"
                    AutoGenerateColumns="False"  
                    GridLines="None"  
                    CssClass="mGrid"  
                    AlternatingRowStyle-CssClass="alt" DataKeyNames="image_id" OnRowCommand="gvImages_RowCommand">  
                    <Columns>
                        <asp:TemplateField HeaderText="Image" Visible="True">
                            <ItemTemplate>
                                <asp:image ID="Image1" runat="server" style="max-width:100px;" ImageUrl ='<%# "ImageHandler.ashx?id=" + Eval("image_id") %>'/>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="image_name" HeaderText="Name" /> 
                        <asp:BoundField DataField="image_type" HeaderText="Type" />
                        <asp:BoundField DataField="image_width" HeaderText="Width" />
                        <asp:BoundField DataField="image_height" HeaderText="Height" />
                        <asp:BoundField DataField="image_dpi" HeaderText="DPI" />
                        <asp:TemplateField HeaderText="Delete" Visible="True">
                            <ItemTemplate>
                                <center>
                                    <asp:ImageButton ID="hypDelete" runat="server" CommandName="DeleteImage" ImageUrl="~/images/delete.jpg" tooltip="Delete"></asp:ImageButton>
                                </center>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="False">
                            <ItemTemplate>
                                <asp:Label ID="lblID" runat="server" Text='<%# Eval("image_id") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>  
                </asp:GridView>      
            </center>
        </div>
        <div class="span2">
        </div>
    </div>
    <br />
    <div class="row-fluid">
        <div class="span2">
        </div>
        <div class="span8">
            <h3>add a new image</h3>
            <asp:FileUpload ID="fileUpload" runat="server" onchange="callme(this)" />
            <asp:TextBox ID="txtName" runat="server" CssClass="input"></asp:TextBox>
            <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Add" />
        </div>
        <div class="span2">
        </div>
    </div>

</asp:Content>
