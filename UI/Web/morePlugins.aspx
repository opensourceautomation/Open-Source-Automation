<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="morePlugins.aspx.cs" Inherits="morePlugins" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <div class="row-fluid">
        <div class="span12" id="pluginGrid" style="overflow: auto; border: solid; max-height: 500px;">

            <asp:GridView runat="server" ID="AvailablePluginsGridView"
                AutoGenerateColumns="False"
                GridLines="None"
                CssClass="mGrid"
                DataKeyNames="guid"
                AlternatingRowStyle-CssClass="alt" OnRowCommand="AvailablePluginsGridView_RowCommand" OnRowCreated="AvailablePluginsGridView_RowCreated">
                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                <Columns>
                    <asp:TemplateField HeaderText="Plugin" ItemStyle-Width="350px">
                        <ItemTemplate>
                            <asp:Image ID="image" runat="server" Width="140px" Height="140px" ImageUrl='<%# Bind("imageURL") %>' />
                            <asp:Label ID="Label1" runat="server" Font-Bold="true" Text='<%# Bind("name") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="version" HeaderText="Version" />
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="LinkButton1" EventName="Click" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:LinkButton ID="LinkButton1" CommandArgument='<%# Bind("link") %>' runat="server" OnClientClick="return confirm('This may take some time to download and install depending on the size of the plugin');"
                                        CausesValidation="false" CommandName="install" Text="Install" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdateProgress ID="UpdateProgress1" runat="Server" AssociatedUpdatePanelID="UpdatePanel1">
                                <ProgressTemplate>
                                    <asp:Label ID="lblwait" runat="server" Text="Please wait.."></asp:Label>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>


        </div>
    </div>
</asp:Content>

