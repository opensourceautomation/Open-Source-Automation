<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="importexport.aspx.cs" Inherits="importexport"  EnableEventValidation="false"  %>
<%@ MasterType virtualpath="~/MasterPage.master" %>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentPlaceHolder">
    <script type="text/javascript">
        function ShowButton()
        {
            var iBtn = document.getElementById('<%=btnImport.ClientID %>');
            var cBtn = document.getElementById('<%=btnClear1.ClientID %>');
            var fLoad = document.getElementById('<%=FileLoader.ClientID %>');
            if (fLoad.value == "No file selected")
            {
                iBtn.disabled = true;
                cBtn.disabled = true;
            }
            else
            {
                iBtn.disabled = false;
                cBtn.disabled = false;
            }
        }
    </script>

    <asp:Table ID="Table1" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="10%">
                <asp:Label ID="Label1" runat="server" Text="OSA Import" Font-Bold="True" Font-Size="X-Large"></asp:Label>
                <br />&nbsp;
            </asp:TableCell>
            <asp:TableCell Width="60%">
                <asp:Label ID="Label2" runat="server" Text="Import objects from files created by the EXPORT function below."></asp:Label>
                <br />&nbsp;
            </asp:TableCell>
            <asp:TableCell Width="20%">
                &nbsp;
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                &nbsp;
            </asp:TableCell>
            <asp:TableCell>
                &nbsp;
            </asp:TableCell>
            <asp:TableCell>
                &nbsp;
            </asp:TableCell>
            <asp:TableCell>
                    Choose Import Type:
            <asp:DropDownList ID="ddlImportType" runat="server" OnSelectedIndexChanged="ddlImportType_SelectedIndexChanged" OnTextChanged="ddlImportType_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Value="0">Select an Import Type</asp:ListItem>
                <asp:ListItem Value="Image">Image</asp:ListItem>
                <asp:ListItem Value="Sql">Sql</asp:ListItem>
                <asp:ListItem Value="Package">Package</asp:ListItem>
            </asp:DropDownList>
            </asp:TableCell>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
                <asp:TableCell>
                &nbsp;
            </asp:TableCell>
            <asp:TableCell>
                &nbsp;
            </asp:TableCell>
            <asp:TableCell>
                &nbsp;
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="lblSelectImport" runat="server" Text="Select an Import Type above."></asp:Label><asp:FileUpload ID="FileLoader" runat="server" Accept="" onChange="ShowButton();" ToolTip="Select the file to upload" Enabled="False" Visible="false"/>
                <asp:Label ID="lblImportWarn" runat="server" Text="Incorrect File Type" ForeColor="Red" Visible="false"></asp:Label>
            </asp:TableCell>
            <asp:TableCell>
                &nbsp;
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
                <asp:TableCell>
                &nbsp;
            </asp:TableCell>
            <asp:TableCell>
                &nbsp;
            </asp:TableCell>
            <asp:TableCell>
                &nbsp;
            </asp:TableCell>
            <asp:TableCell CssClass="btn-large">
                <asp:Button ID="btnImport" runat="server" Text="IMPORT" ToolTip="Click to Import selected file." OnClick="btnImport_Click" Class="btn" Enabled="false"/>&nbsp;
                <asp:Button ID="btnClear1" runat="server" Text="CLEAR" ToolTip="Clear current page." OnClick="btnClear_Click" Class="btn" Enabled="false"/>
            </asp:TableCell>
            <asp:TableCell>
                &nbsp;
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
        
    <hr />

    <asp:Table ID="Table2" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="10%">
                <asp:Label ID="Label3" runat="server" Text="OSA Export" Font-Bold="True" Font-Size="X-Large"></asp:Label>
                <br />&nbsp;
            </asp:TableCell>
            <asp:TableCell Width="60%">
                <asp:Label ID="Label4" runat="server" Text="Export objects to files used by the IMPORT function above."></asp:Label>
                <br />&nbsp;
            </asp:TableCell>
            <asp:TableCell Width="20%">
                &nbsp;
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="10%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="60%">
                <asp:CheckBox ID="ckbExportMulti" runat="server" Text="" AutoPostBack="True" BorderStyle="None" OnCheckedChanged="ckbExportMulti_CheckedChanged" Width="25px"></asp:CheckBox>Create a Package with multiple objects, Files or Images.
            </asp:TableCell>
            <asp:TableCell Width="20%">
                &nbsp;
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="10%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="60%">
                Choose Export Type:
            <asp:DropDownList ID="ddlExportType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlExportType_SelectedIndexChanged">
                <asp:ListItem>Select an Export Type</asp:ListItem>
                <asp:ListItem>Image</asp:ListItem>
                <asp:ListItem>Log</asp:ListItem>
                <asp:ListItem>Object</asp:ListItem>
                <asp:ListItem>ObjectType</asp:ListItem>
                <asp:ListItem>Pattern</asp:ListItem>
                <asp:ListItem>Reader</asp:ListItem>
                <asp:ListItem>Schedule</asp:ListItem>
                <asp:ListItem>Screen</asp:ListItem>
                <asp:ListItem>Script</asp:ListItem>
            </asp:DropDownList>
            </asp:TableCell>
            <asp:TableCell Width="20%">
                &nbsp;
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="10%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="60%">
                <asp:Label ID="lblExportObj" runat="server" Text="No Objects to Export:"></asp:Label><asp:Label ID="lblFileList" runat="server" Text="Files to be included in this package:" Visible="False"></asp:Label><br /><br />
                <asp:ListBox ID="ddlObjToExport" runat="server" Enabled="False" OnSelectedIndexChanged="ddlObjToExport_SelectedIndexChanged" AutoPostBack="True" ></asp:ListBox>&nbsp;
                <asp:Button ID="btnAddFile" runat="server" Text="Add File" onClick="btnAddFile_Click" Visible="False" />&nbsp;
                <asp:ListBox ID="lstFileList" runat="server" Enabled="False" Visible="False" Width="150"></asp:ListBox>
            </asp:TableCell>
            <asp:TableCell Width="20%">
                &nbsp;
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="5%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="10%">
                &nbsp;
            </asp:TableCell>
            <asp:TableCell Width="60%">
                <asp:Label ID="lblZipName" runat="server" Text="Enter File Name for Package" Visible="False"></asp:Label><asp:TextBox ID="txtZipName" runat="server" OnTextChanged="txtZipName_Changed"  Visible="False"></asp:TextBox>
                <asp:Label ID="lblFNError" runat="server" Text="Please Enter A File Name" Font-Bold="True" ForeColor="Red" Visible="False"></asp:Label>
                <asp:Button ID="btnExport" runat="server" Text="EXPORT" ToolTip="Click to Export selected Object." OnClick="btnExport_Click" Enabled="False" Visible="False" />&nbsp;
                <asp:Button ID="btnClear2" runat="server" Text="CLEAR" ToolTip="Clear current page." OnClick="btnClear_Click" Enabled="False" Visible="False"/> 
            </asp:TableCell>
            <asp:TableCell Width="20%">
                &nbsp;
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
        
    <asp:HiddenField ID="hdnImportType" runat="server" />
</asp:Content>