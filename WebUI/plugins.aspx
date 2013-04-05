<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="plugins.aspx.cs" Inherits="plugins" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    <div class="row-fluid">
        <div class="span12" id="pluginGrid" style="overflow: auto; border:solid; max-height:500px;"  onscroll="SetPluginDivPosition()">
            <asp:GridView runat="server" ID="gvPlugins"
                AutoGenerateColumns="False"  
                GridLines="None"  
                CssClass="mGrid"  
                AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvPlugins_RowDataBound">  
                <Columns>
                    <asp:TemplateField HeaderText="Enabled" Visible="True">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkEnabled" runat="server" AutoPostback="true" OnCheckedChanged="chkEnabled_OnCheckedChanged" />
                            <asp:Label ID="lblEnabled" runat="server" Text='<%# Eval("Enabled") %>' Visible="false"></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Type" HeaderText="Plugin" /> 
                    <asp:BoundField DataField="Status" HeaderText="Status" />  
                    <asp:BoundField DataField="Author" HeaderText="Author" /> 
                    <asp:BoundField DataField="Name" HeaderText="OSA Object" /> 
                    <asp:BoundField DataField="Version" HeaderText="Version" /> 
                </Columns>  
            </asp:GridView>
        </div>
    </div>


</asp:Content>

