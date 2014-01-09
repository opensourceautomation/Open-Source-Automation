<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="logs.aspx.cs" Inherits="logs" %>
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
            <asp:Button runat="server" ID="btnRefresh" Text="Refresh" class="btn" OnClick="btnRefresh_Click"/>
            <asp:GridView ID="gvLog" runat="server"
                AutoGenerateColumns="false"
                GridLines="None"
                CssClass="mGrid"
                AlternatingRowStyle-CssClass="alt" >
                <Columns>
                    <asp:BoundField HeaderText="Time" DataField="Date" />
                    <asp:BoundField HeaderText="Level" DataField="Level" />
                    <asp:BoundField HeaderText="Source" DataField="Logger" />
                    <asp:BoundField HeaderText="Message" DataField="Message" ItemStyle-Width="40%"/>
                    <asp:BoundField HeaderText="Exception" DataField="Exception" ItemStyle-Width="40%" />
                </Columns>
            </asp:GridView>
            
        </div>
        <div class="span1">
        </div>
    </div>

    <asp:Label runat="server" ID="hdnSelectedRow" Visible="false" />
</asp:Content>

