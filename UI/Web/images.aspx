<%@ Page Language="C#" AutoEventWireup="true" CodeFile="images.aspx.cs" Inherits="images" MasterPageFile="~/MasterPage.master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder">

    <div class="row-fluid">
        <div class="span2">
        </div>
        <div class="span8">
            <center>
                <asp:GridView runat="server" ID="gvImages"
                    AutoGenerateColumns="False"  
                    GridLines="None"  
                    CssClass="mGrid"  
                    AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvImages_RowDataBound">  
                    <Columns>
                        <asp:TemplateField HeaderText="Image" Visible="True">
                            <ItemTemplate>
                                <asp:image ID="Image1" runat="server" style="max-width:100px;" ImageUrl ='<%# "imgHandler.aspx?ImageID=" + Eval("image_id") %>'/>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="image_name" HeaderText="Name" /> 
                        <asp:BoundField DataField="image_type" HeaderText="Type" />
                        <asp:TemplateField HeaderText="Delete" Visible="True">
                            <ItemTemplate>
                                <center>
                                    <asp:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click" Text="Delete" CssClass='<%# "class" + Eval("image_id") %>' />
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

</asp:Content>
