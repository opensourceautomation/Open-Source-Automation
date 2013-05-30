<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="error.aspx.cs" Inherits="error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    <div class="row-fluid">
        <div class="span12">

            <h3>OOPS! there was an error that wasn't expected. The details of the error are:</h3>
            <br />
            <asp:TextBox ID="errorDetailTextBox" runat="server" ReadOnly="true" TextMode="MultiLine" Height="20em" />
            <br />
            <br />
            <h4>What to do now</h4>
            <ul>
                <li>
                    Try the  to see if anyone else is having the same issue or if anyone else is able to help in the <asp:HyperLink ID="HyperLink1" Text="Forum" NavigateUrl="http://www.opensourceautomation.com/phpBB3/" runat="server" />
                </li>
                <li>
                    If no one in the <asp:HyperLink ID="forumHyperLink" Text="Forum" NavigateUrl="http://www.opensourceautomation.com/phpBB3/" runat="server" /> can solve the problem you could sumit a bug report to
                    <asp:HyperLink ID="gitHubHyperLink" Text="GIT HUB" NavigateUrl="https://github.com/opensourceautomation/Open-Source-Automation/issues" runat="server" />            
                </li>
                <li>
                    Remember to make a copy of the error details as it will help to solve the problem
                </li>
            </ul>   
        </div>
    </div>
</asp:Content>

