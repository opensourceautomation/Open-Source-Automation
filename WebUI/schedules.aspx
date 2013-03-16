<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"  MaintainScrollPositionOnPostback="true" EnableEventValidation="false" CodeFile="schedules.aspx.cs" Inherits="schedules" %>
<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
   
    <div class="row-fluid">
        <div class="span8">
            <div class="row-fluid">
                <div class="span12">
                    <div class="row-fluid" ID="QueueGrid" style="overflow: auto; max-height:450px;" onscroll="SetQueueDivPosition()">
                        <asp:GridView runat="server" ID="gvQueue"
                            AutoGenerateColumns="False"  
                            GridLines="None"  
                            CssClass="mGrid" ShowHeader="true" 
                            AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvQueue_RowDataBound" DataKeyNames="schedule_id" ShowHeaderWhenEmpty="true">  
                            <Columns>  
                                <asp:BoundField DataField="schedule_name" HeaderText="Name" />
                                <asp:BoundField DataField="queue_datetime" HeaderText="DateTime" />
                                <asp:BoundField DataField="schedule_id" Visible="false" /> 
                            </Columns>  
                        </asp:GridView>
                    </div>
                    <div class="row-fluid">
                        <asp:Button class="btn" runat="server" ID="btnQueueDelete" Text="Delete" />
                    </div>
                </div>
            </div>
            <div class="row-fluid">
                <div class="span12">
                    <div class="row-fluid" ID="RecurringGrid" style="overflow: auto; max-height:450px;" onscroll="SetRecurringDivPosition()">
                        <asp:GridView runat="server" ID="gvRecurring"
                            AutoGenerateColumns="False"  
                            GridLines="None"  
                            CssClass="mGrid" ShowHeader="true" 
                            AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvRecurring_RowDataBound" DataKeyNames="recurring_id" ShowHeaderWhenEmpty="true">  
                            <Columns>  
                                <asp:BoundField DataField="schedule_name" HeaderText="Name" />
                                <asp:BoundField DataField="interval_unit" HeaderText="Interval" />
                                <asp:BoundField DataField="recurring_id" Visible="false" /> 
                            </Columns>  
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
        <div class="span4">
            <div class="row-fluid">

            </div>
        </div>
    </div>
    <asp:Label runat="server" ID="hdnSelectedQueueRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedQueueID" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedRecurringRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedRecurringID" Visible="false"></asp:Label>
</asp:Content>

