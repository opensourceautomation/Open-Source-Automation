<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctrlUserControl.ascx.cs" Inherits="controls_ctrlUserControl" className="ctrlUserControl"%>

<asp:Button ID="btnState" runat="server" Text="Button" Width="85px" OnClick="btnState_Click" />
<asp:HiddenField ID="hdnCurState" runat="server" />
<asp:HiddenField ID="hdnObjName" runat="server" />