<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="firstrun.aspx.cs" Inherits="firstrun" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder" runat="Server">
   <center> <h2>Welcome to OSA</h2>  <br />
    <h4>This looks to be the first time you have used the OSA admin web interface</h4>
       <h4>In order to get started a user account needs to be set up, this user account</h4>
       <h4>will be an administrator in the system. subsequent users can be added and edited</h4>
       <h4>int the users screen.</h4>
       <br /><br />
       <table>
           <tr>
               <td>
                    <asp:Label runat="server" ID="userLabel" Text="User:" AssociatedControl="userTextBox" />
               </td>
               <td>
                    <asp:TextBox runat="server" ID="userTextBox" />
               </td>
           </tr>
           <tr>
               <td>
                   <asp:Label runat="server" ID="passwordLabel" Text="Password:" AssociatedControl="userTextBox" />
               </td>
               <td>
                    <asp:TextBox runat="server" ID="passwordTextBox" />
               </td>
           </tr>
           <tr>
               <td>
                   <asp:Label runat="server" ID="confirmPasswordLabel" Text="Confirm Password: " AssociatedControl="userTextBox" />
               </td>
               <td>
                   <asp:TextBox runat="server" ID="passwordConfirmTextBox" />
               </td>
           </tr>
       </table>
       <asp:LinkButton runat="server" ID="createUserLinkButton" Text="Create" />
       </center>
</asp:content>

