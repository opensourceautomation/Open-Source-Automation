using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class admin : System.Web.UI.Page
{
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Session["Username"] == null) Response.Redirect("~/Default.aspx");
            if (Session["SecurityLevel"].ToString() != "Admin") Response.Redirect("~/permissionError.aspx");
            OSAEAdmin adSet = OSAEAdminManager.GetAdminSettings();
            screensLev.Text = adSet.ScreenTrust.ToString();
            string dScreen = adSet.defaultScreen;
            objectsLev.Text = adSet.ObjectsTrust.ToString();
            objectsAddLev.Text = adSet.ObjectsAddTrust.ToString();
            objectsUpdateLev.Text = adSet.ObjectsUpdateTrust.ToString();
            objectsDeleteLev.Text = adSet.ObjectsDeleteTrust.ToString();
            analyticsLev.Text = adSet.AnalyticsTrust.ToString();
            manageLev.Text = adSet.ManagementTrust.ToString();
            objecttypeLev.Text = adSet.ObjectTypeTrust.ToString();
            objecttypeAddLev.Text = adSet.ObjectTypeAddTrust.ToString();
            objecttypeUpdateLev.Text = adSet.ObjectTypeUpdateTrust.ToString();
            objecttypeDeleteLev.Text = adSet.ObjectTypeDeleteTrust.ToString();
            scriptLev.Text = adSet.ScriptTrust.ToString();
            scriptAddLev.Text = adSet.ScriptAddTrust.ToString();
            scriptUpdateLev.Text = adSet.ScriptUpdateTrust.ToString();
            scriptDeleteLev.Text = adSet.ScriptDeleteTrust.ToString();
            scriptObjectEventLev.Text = adSet.ScriptObjectAddTrust.ToString();
            scriptObjectTypeEventLev.Text = adSet.ScriptObjectTypeAddTrust.ToString();
            patternLev.Text = adSet.PatternTrust.ToString();
            patternAddLev.Text = adSet.PatternAddTrust.ToString();
            patternUpdateLev.Text = adSet.PatternUpdateTrust.ToString();
            patternDeleteLev.Text = adSet.PatternDeleteTrust.ToString();
            readerLev.Text = adSet.ReaderTrust.ToString();
            readerAddLev.Text = adSet.ReaderAddTrust.ToString();
            readerUpdateLev.Text = adSet.ReaderUpdateTrust.ToString();
            readerDeleteLev.Text = adSet.ReaderDeleteTrust.ToString();
            scheduleLev.Text = adSet.ScheduleTrust.ToString();
            scheduleAddLev.Text = adSet.ScheduleAddTrust.ToString();
            scheduleUpdateLev.Text = adSet.ScheduleUpdateTrust.ToString();
            scheduleDeleteLev.Text = adSet.ScheduleDeleteTrust.ToString();
            imageLev.Text = adSet.ImagesTrust.ToString();
            imageAddLev.Text = adSet.ImagesAddTrust.ToString();
            imageDeleteLev.Text = adSet.ImagesDeleteTrust.ToString();
            logsLev.Text = adSet.LogsTrust.ToString();
            logsClearLev.Text = adSet.LogsClearTrust.ToString();
            eventlogLev.Text = adSet.EventLogTrust.ToString();
            methodlogLev.Text = adSet.MethodLogTrust.ToString();
            serverlogLev.Text = adSet.ServerLogTrust.ToString();
            debuglogLev.Text = adSet.DebugLogTrust.ToString();
            valuesLev.Text = adSet.ValuesTrust.ToString();
            configLev.Text = adSet.ConfigTrust.ToString();
            OSAEObjectCollection screens = OSAEObjectManager.GetObjectsByType("SCREEN");
            foreach (OSAEObject s in screens)
            {
                ListItem li = new ListItem(s.Name);
                if(s.Name == dScreen) li.Selected = true;
                mainScreen.Items.Add(li);
            }
            saveSuc.Visible = false;
        }
    }

    protected void btnAdminSave_Click(object sender, EventArgs e)
    {
        try
        {
            OSAEAdmin newadSet = new OSAEAdmin();
            newadSet.ScreenTrust = Convert.ToInt32(screensLev.Text);
            newadSet.defaultScreen = mainScreen.Text;
            newadSet.ObjectsTrust = Convert.ToInt32(objectsLev.Text);
            newadSet.ObjectsAddTrust = Convert.ToInt32(objectsAddLev.Text);
            newadSet.ObjectsUpdateTrust = Convert.ToInt32(objectsUpdateLev.Text);
            newadSet.ObjectsDeleteTrust = Convert.ToInt32(objectsDeleteLev.Text);
            newadSet.AnalyticsTrust = Convert.ToInt32(analyticsLev.Text);
            newadSet.ManagementTrust = Convert.ToInt32(manageLev.Text);
            newadSet.ObjectTypeTrust = Convert.ToInt32(objecttypeLev.Text);
            newadSet.ObjectTypeAddTrust = Convert.ToInt32(objecttypeAddLev.Text);
            newadSet.ObjectTypeUpdateTrust = Convert.ToInt32(objecttypeUpdateLev.Text);
            newadSet.ObjectTypeDeleteTrust = Convert.ToInt32(objecttypeDeleteLev.Text);
            newadSet.ScriptTrust = Convert.ToInt32(scriptLev.Text);
            newadSet.ScriptAddTrust = Convert.ToInt32(scriptAddLev.Text);
            newadSet.ScriptUpdateTrust = Convert.ToInt32(scriptUpdateLev.Text);
            newadSet.ScriptDeleteTrust = Convert.ToInt32(scriptDeleteLev.Text);
            newadSet.ScriptObjectAddTrust = Convert.ToInt32(scriptObjectEventLev.Text);
            newadSet.ScriptObjectTypeAddTrust = Convert.ToInt32(scriptObjectTypeEventLev.Text);
            newadSet.PatternTrust = Convert.ToInt32(patternLev.Text);
            newadSet.PatternAddTrust = Convert.ToInt32(patternAddLev.Text);
            newadSet.PatternUpdateTrust = Convert.ToInt32(patternUpdateLev.Text);
            newadSet.PatternDeleteTrust = Convert.ToInt32(patternDeleteLev.Text);
            newadSet.ReaderTrust = Convert.ToInt32(readerLev.Text);
            newadSet.ReaderAddTrust = Convert.ToInt32(readerAddLev.Text);
            newadSet.ReaderUpdateTrust = Convert.ToInt32(readerUpdateLev.Text);
            newadSet.ReaderDeleteTrust = Convert.ToInt32(readerDeleteLev.Text);
            newadSet.ScheduleTrust = Convert.ToInt32(scheduleLev.Text);
            newadSet.ScheduleAddTrust = Convert.ToInt32(scheduleAddLev.Text);
            newadSet.ScheduleUpdateTrust = Convert.ToInt32(scheduleUpdateLev.Text);
            newadSet.ScheduleDeleteTrust = Convert.ToInt32(scheduleDeleteLev.Text);
            newadSet.ImagesTrust = Convert.ToInt32(imageLev.Text);
            newadSet.ImagesAddTrust = Convert.ToInt32(imageAddLev.Text);
            newadSet.ImagesDeleteTrust = Convert.ToInt32(imageDeleteLev.Text);
            newadSet.LogsTrust = Convert.ToInt32(logsLev.Text);
            newadSet.LogsClearTrust = Convert.ToInt32(logsClearLev.Text);
            newadSet.EventLogTrust = Convert.ToInt32(eventlogLev.Text);
            newadSet.MethodLogTrust = Convert.ToInt32(methodlogLev.Text);
            newadSet.ServerLogTrust = Convert.ToInt32(serverlogLev.Text);
            newadSet.DebugLogTrust = Convert.ToInt32(debuglogLev.Text);
            newadSet.ValuesTrust = Convert.ToInt32(valuesLev.Text);
            newadSet.ConfigTrust = Convert.ToInt32(configLev.Text);
            OSAEAdminManager.UpdateAdminSettings(newadSet);
            saveSuc.Visible = true;
        }
        catch { }
    }

    protected void btnAdminCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/admin.aspx");
    }
}