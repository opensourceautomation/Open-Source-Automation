<%@ Page Language="C#" AutoEventWireup="true" CodeFile="config.aspx.cs" Inherits="config"  MasterPageFile="~/MasterPage.master" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder">

    <div class="row-fluid">
        <div class="span2">

        </div>
        <div class="span8">
            <center><img src="Images/OSA.png" /></center>
            <br />
            <br />
            <br />
            <div class="row-fluid">
                <div class="span6" style="text-align:right">
                    Current Version: 
                    <br />
                    Debug Mode: 
                </div>
                <div class="span6" style="text-align:left">
                    <asp:Label ID="lblVersion" runat="server"></asp:Label>
                    <br />
                    <asp:DropDownList ID="ddlDebug" runat="server">
                        <asp:ListItem Text="True" Value="True"></asp:ListItem>
                        <asp:ListItem Text="False" Value="False"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>
        <div class="span2">

        </div>
    </div>
</asp:Content>

