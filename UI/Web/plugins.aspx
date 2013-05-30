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
                    <asp:TemplateField HeaderText="OSA Object" Visible="True">
                        <ItemTemplate>
                            <asp:Label ID="lblObject" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Version" Visible="True">
                        <ItemTemplate>
                            <asp:Label ID="lblCurVersion" runat="server" Text='<%# Eval("Version") %>'></asp:Label>
                            <asp:HyperLink ID="hypUpgrade" runat="server">
                                <asp:Image runat="server" ImageUrl="Images/upgrade.png" ToolTip="New version available!" />
                            </asp:HyperLink>
                            <asp:Label ID="lblLatestVersion" runat="server" Text='<%# Eval("Upgrade") %>' Visibile="false"></asp:Label>
                            <asp:Label ID="lblID" runat="server" Text='<%# Eval("ID") %>' Visible="false"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>  
            </asp:GridView>            
        </div>
      <%--  <br />
        <br />
        <asp:Button runat="server" ID="btnGetMorePlugins" Text="Get More Plugins" class="btn" OnClick="btnGetMorePlugins_Click" />--%>
    </div>
</asp:Content>

