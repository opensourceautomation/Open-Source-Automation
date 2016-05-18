<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctrlStateImage.ascx.cs" Inherits="controls_ctrlStateImage" className="ctrlStateImage"%>
<script type="text/javascript">
    function methFunc(object, method, p1, p2, objTrust, img, n)
    {
        var sI = document.getElementById(n);
        sI.setAttribute('src', img);
        runMethod(object, method, p1, p2, objTrust);
    }
</script>
<asp:Image ID="imgStateImage" runat="server"/>
<asp:HiddenField ID="hdnCurState" runat="server" />
<asp:HiddenField ID="hdnObjName" runat="server" />

