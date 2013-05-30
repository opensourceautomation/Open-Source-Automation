<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="firstrun.aspx.cs" Inherits="firstrun" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder" runat="Server">
    <div class="row-fluid">

    <div class="span3"></div>
    <div class="span6">
        <center><h2>Welcome to Open Source Automation</h2>  <br />
        <h4>You do not have any users set up in your system yet.
        In order to get started a user account needs to be set up, this user account
        will be an administrator in the system. subsequent users can be added and edited
        in the users screen.</h4>
        <br /><br /></center>
        
        <div class="form-horizontal">
            <div class="control-group">
            <label class="control-label" for="txtUser">User</label>
                <div class="controls">
                    <asp:TextBox class="input-xlarge" runat="server" ID="txtUser" />
                </div>
            </div>
            <div class="control-group">
                <label class="control-label" for="txtPass">Password</label>
                <div class="controls">
                    <asp:TextBox  class="input-xlarge" TextMode="password" runat="server" ID="txtPass" />
                </div>
            </div>
            <div class="control-group">
                <label class="control-label" for="txtPass2">Confirm Password</label>
                <div class="controls">
                    <asp:TextBox class="input-xlarge" TextMode="password" runat="server" ID="txtPass2" />
                </div>
            </div>
            <div class="control-group">
                <div class="controls">
                    <asp:LinkButton runat="server" class="btn" ID="createUserLinkButton" Text="Create" OnClick="createUserLinkButton_Click" />
                </div>
            </div>
        </div>
        <br />
        <div class="alert" runat="server" id="divError" visible ="false">
            The passwords do not match.  Please correect and try again.
        </div>
    </div>

    <div class="span3"></div>

    </div>
</asp:content>

