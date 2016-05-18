<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="logs.aspx.cs" Inherits="logs"  %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <script type="text/javascript">
    </script>
    <div class="row-fluid">
        <div class="span1">
            <div id="ObjPanel">
            </div>
        </div>
        <div class="span10">
            <br />
            <asp:Button runat="server" ID="btnRefresh" Text="Refresh" class="btn" OnClick="btnRefresh_Click"/>
            <asp:Button runat="server" ID="btnClear" Text="Clear" class="btn" OnClick="btnClear_Click"/>
            INFO: <asp:CheckBox ID="chkInfo" runat="server" Checked="true" OnCheckedChanged="CheckedChanged" AutoPostBack="true" />
            DEBUG: <asp:CheckBox ID="chkDebug" runat="server" Checked="true" OnCheckedChanged="CheckedChanged" AutoPostBack="true" />
            ERROR: <asp:CheckBox ID="chkError" runat="server" Checked="true" OnCheckedChanged="CheckedChanged" AutoPostBack="true" />
            <br />
            <asp:GridView ID="gvLog" runat="server" AutoGenerateColumns="False" GridLines="None" CssClass="mGrid" AlternatingRowStyle-CssClass="alt" >
                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                <Columns>
                    <asp:BoundField HeaderText="Time" DataField="log_time" ItemStyle-Width="10em" DataFormatString="{0:MM-dd HH:mm:ss.ff}" />
                    <asp:BoundField HeaderText="Level" DataField="Level"/>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            Source:
                            <asp:DropDownList ID="ddlSource" runat="server"
                            OnSelectedIndexChanged = "CheckedChanged" AutoPostBack = "true"
                            AppendDataBoundItems = "true">
                                <asp:ListItem Text = "ALL" Value = "ALL"></asp:ListItem>
                            </asp:DropDownList>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%# Eval("Logger") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Message" DataField="Message"/>
                    <asp:BoundField HeaderText="Exception" DataField="Exception"/>
                </Columns>
            </asp:GridView>
            <asp:Button runat="server" ID="btnRefresh2" Text="Refresh" class="btn" OnClick="btnRefresh_Click"/>
            <asp:Button runat="server" ID="btnClear2" Text="Clear" class="btn" OnClick="btnClear_Click"/>
        </div>
        <div class="span1">
        </div>
    </div>
    <asp:Label runat="server" ID="hdnSelectedRow" Visible="false" />
</asp:Content>

