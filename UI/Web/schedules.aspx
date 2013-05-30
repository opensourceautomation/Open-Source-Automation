<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"  MaintainScrollPositionOnPostback="true" EnableEventValidation="false" CodeFile="schedules.aspx.cs" Inherits="schedules" %>

<%@ Implements Interface="System.Web.UI.IPostBackEventHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
    <script>
        function pageLoad() {
            $("#<%=datepicker.ClientID%>").datepicker();
            $("#<%=datepicker.ClientID%>").datepicker("option", "dateFormat", "yy-mm-dd");
            
            $("#<%=datepicker.ClientID%>").val($("#<%=txtPickedDate.ClientID%>").val());

            $('#<%=tsTime.ClientID%>').timepicker();
        }

        

        $(function () {
            $("#<%=datepicker.ClientID%>").change(function () {
                $("#<%=txtPickedDate.ClientID%>").val($("#<%=datepicker.ClientID%>").val());
            });
        });
  </script>
    <style>
        .radioButtonList input
         {    
              float: left;
         }

         .radioButtonList label
         {    
              margin-left: 20px;
              display: block;
              text-align: left;
         }

         /* css for timepicker */
        .ui-timepicker-div .ui-widget-header { margin-bottom: 8px; }
        .ui-timepicker-div dl { text-align: left; }
        .ui-timepicker-div dl dt { height: 25px; margin-bottom: -25px; }
        .ui-timepicker-div dl dd { margin: 0 10px 10px 65px; }
        .ui-timepicker-div td { font-size: 90%; }
        .ui-tpicker-grid-label { background: none; border: none; margin: 0; padding: 0; }

        .ui-timepicker-rtl{ direction: rtl; }
        .ui-timepicker-rtl dl { text-align: right; }
        .ui-timepicker-rtl dl dd { margin: 0 65px 10px 10px; }  
    </style>
    <div class="row-fluid">
        <div class="span8">
            <div class="row-fluid">
                <div class="span12">
                    <div class="row-fluid" ID="QueueGrid" style="overflow: auto; height:350px;" onscroll="SetQueueDivPosition()">
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
                        <asp:Button class="btn" runat="server" ID="btnQueueDelete" Text="Delete" Visible="false" OnClick="btnQueueDelete_Click" OnClientClick="return confirm('Are you sure you want to delete the queued schedule?');"/>
                    </div>
                </div>
            </div>
            <div class="row-fluid">
                <div class="span12">
                    <div class="row-fluid" ID="RecurringGrid" style="overflow: auto; height:350px;" onscroll="SetRecurringDivPosition()">
                        <asp:GridView runat="server" ID="gvRecurring"
                            AutoGenerateColumns="False"  
                            GridLines="None"  
                            CssClass="mGrid" ShowHeader="true" 
                            AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvRecurring_RowDataBound" DataKeyNames="recurring_id, schedule_name" ShowHeaderWhenEmpty="true">  
                            <Columns>  
                                <asp:TemplateField HeaderText="Name" Visible="True">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRecurringName" runat="server" Text='<%# Eval("schedule_name") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="interval_unit" HeaderText="Interval" />
                                <asp:BoundField DataField="recurring_id" Visible="false" /> 
                                <asp:TemplateField HeaderText="Active" Visible="True">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkActive" runat="server" AutoPostback="true" Enabled="false" />
                                        <asp:Label ID="lblActive" runat="server" Text='<%# Eval("active") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>  
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
        <div class="span4">
            <div class="row-fluid hero-unit">
                <div class="span12">
                    <div class="row-fluid">
                        <div class="span2" style="text-align:right;">
                            Name: 
                        </div>
                        <div class="span10">
                            <asp:TextBox class="input-xlarge" runat="server" ID="txtName"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div class="span2" style="text-align:right;">
                            Time: 
                        </div>
                        <div class="span10">
                            <asp:TextBox runat="server" name="timepicker" ID="tsTime"></asp:TextBox>
                         
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div class="span2" style="text-align:right;">
                            Active: 
                        </div>
                        <div class="span10">
                            <asp:CheckBox runat="server" ID="chkActive" />
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div class="span12">
                            <asp:RadioButtonList runat="server" ID="rbScheduleType" OnSelectedIndexChanged="rbScheduleType_SelectedIndexChanged"  CssClass="radioButtonList"
                                AutoPostBack="true"
                                RepeatDirection="Vertical">
                                <asp:ListItem Text="Single Entry" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Every # of Minutes" Value="T"></asp:ListItem>
                                <asp:ListItem Text="Daily" Value="D"></asp:ListItem>
                                <asp:ListItem Text="Monthly" Value="M"></asp:ListItem>
                                <asp:ListItem Text="Anually" Value="Y"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="row-fluid">
                        <div class="span12">    
                            <br />
                            <asp:TextBox runat="server" ID="datepicker" Visible="false"></asp:TextBox>
                            <asp:TextBox runat="server" ID="txtMinutes" class="input-small" Visible="false">20</asp:TextBox>
                            <asp:Panel runat="server" ID="pnlDaily" Visible="false">
                                <asp:CheckBox runat="server" ID="chkSunday" /> Sunday<br />
                                <asp:CheckBox runat="server" ID="chkMonday" /> Monday<br />
                                <asp:CheckBox runat="server" ID="chkTuesday" /> Tuesady<br />
                                <asp:CheckBox runat="server" ID="chkWednesday" /> Wednesday<br />
                                <asp:CheckBox runat="server" ID="chkThursday" /> Thursday<br />
                                <asp:CheckBox runat="server" ID="chkFriday" /> Friday<br />
                                <asp:CheckBox runat="server" ID="chkSaturday" /> Saturday
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlMonthly" Visible="false">
                                Day of the month: 
                                <asp:DropDownList runat="server" ID="ddlMonthDay">
                                    <asp:ListItem Text="" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                    <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                    <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                    <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                    <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                    <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                    <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                    <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                    <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                    <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                    <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                    <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                    <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                    <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                    <asp:ListItem Text="21" Value="21"></asp:ListItem>
                                    <asp:ListItem Text="22" Value="22"></asp:ListItem>
                                    <asp:ListItem Text="23" Value="23"></asp:ListItem>
                                    <asp:ListItem Text="24" Value="24"></asp:ListItem>
                                    <asp:ListItem Text="25" Value="25"></asp:ListItem>
                                    <asp:ListItem Text="26" Value="26"></asp:ListItem>
                                    <asp:ListItem Text="27" Value="27"></asp:ListItem>
                                    <asp:ListItem Text="28" Value="28"></asp:ListItem>
                                    <asp:ListItem Text="29" Value="29"></asp:ListItem>
                                    <asp:ListItem Text="30" Value="30"></asp:ListItem>
                                    <asp:ListItem Text="31" Value="31"></asp:ListItem>      
                                </asp:DropDownList>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row-fluid hero-unit">
                <div class="span12">
                    <h3>Action to Perform</h3>
                    <br />
                    <asp:RadioButtonList runat="server" ID="rblAction" OnSelectedIndexChanged="rblAction_SelectedIndexChanged"  CssClass="radioButtonList"
                        AutoPostBack="true"
                        RepeatDirection="Vertical">
                        <asp:ListItem Text="Run a Script" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Run an Object Method" Value="2"></asp:ListItem>
                    </asp:RadioButtonList>
                    <br />
                    <asp:DropDownList runat="server" ID="ddlScript" datatextfield="Text" datavaluefield="Value" title="The script you want to run when the schedule occurs" style="width:100%" Visible="false"></asp:DropDownList>
                    <asp:Panel runat="server" ID="pnlMethod" Visible="false">
                        Object: <asp:DropDownList runat="server" ID="ddlObject" datatextfield="Text" datavaluefield="Value" style="width:100%" OnSelectedIndexChanged="ddlObject_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                        Method: <asp:DropDownList runat="server" ID="ddlMethod" datatextfield="Text" datavaluefield="Value" style="width:100%"></asp:DropDownList>
                        Parameter 1: <asp:TextBox class="input-xlarge" runat="server" ID="txtParam1"></asp:TextBox>
                        <br />Parameter 2: <asp:TextBox class="input-xlarge" runat="server" ID="txtParam2"></asp:TextBox>
                    </asp:Panel>
                    <br />
                    <asp:Button runat="server" ID="btnAdd" Text="Add" class="btn" OnClick="btnAdd_Click" Visible="false"/>&nbsp
                    <asp:Button runat="server" ID="btnUpdate" Text="Update" class="btn" OnClick="btnUpdate_Click" Visible="false"/>&nbsp
                    <asp:Button runat="server" ID="btnDelete" Text="Delete" class="btn" OnClick="btnDelete_Click" Visible="false" OnClientClick="return confirm('Are you sure you want to delete the schedule?');" />
                    <br />
                    <br />
                    <div class="alert alert-success" runat="server" id="alert" visible="false">
                      <asp:Label runat="server" ID="lblAlert"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:TextBox runat="server" ID="txtPickedDate" style="display:none;"></asp:TextBox>
    <asp:Label runat="server" ID="hdnSelectedQueueRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedQueueID" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedRecurringRow" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedRecurringID" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="hdnSelectedRecurringName" Visible="false"></asp:Label>
</asp:Content>

