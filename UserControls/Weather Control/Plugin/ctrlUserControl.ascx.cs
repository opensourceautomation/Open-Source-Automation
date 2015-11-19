using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class controls_ctrlUserControl : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public DateTime LastUpdated;
    public DateTime LastStateChange;
    public string ObjectName;
    private OSAEImageManager imgMgr = new OSAEImageManager();
    public string StateMatch;
    public string CurState;
    OSAEObject weatherObj;

    protected void Page_Load(object sender, EventArgs e)
    {
        ObjectName = screenObject.Property("Object Name").Value;
        hdnObjName.Value = ObjectName;
        CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;
        OSAEObject curObj = OSAEObjectManager.GetObjectByName(ObjectName);
        hdnCurState.Value = CurState;
        LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;
        Table1.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
        Load_All_Weather();
    }

    public void initialize()
    {
    }

    private void Load_All_Weather()
    {
        weatherObj = OSAEObjectManager.GetObjectByName("Weather");
        lblCurTemp.Text = weatherObj.Property("Temp").Value + "°";
        lblConditions.Text = weatherObj.Property("Conditions").Value;
        lblLastUpd.Text = weatherObj.Property("Last Updated").Value;
        LoadLows();
        LoadHighs();
        LoadDayLabels(); 
        LoadDaySummaryLabels();
        LoadNightSummaryLabels();
        LoadDates();
        LoadImageControls();
    }

    #region Load Night Summary
    private void LoadNightSummaryLabels()
    {
        if (weatherObj.Property("Tonight Summary").Value != "")
        {
            imgTodayNight.ToolTip = weatherObj.Property("Tonight Summary").Value;
        }
        else
        {
            imgTodayNight.ToolTip = "Sorry, No Information Available!";
        }
        if (weatherObj.Property("Night1 Summary").Value != "")
        {
            imgDay1Night.ToolTip = weatherObj.Property("Night1 Summary").Value;
        }
        else
        {
            imgDay1Night.ToolTip = "Sorry, No Information Available!";
        }
        if (weatherObj.Property("Night2 Summary").Value != "")
        {
            imgDay2Night.ToolTip = weatherObj.Property("Night2 Summary").Value;
        }
        else
        {
            imgDay2Night.ToolTip = "Sorry, No Information Available!";
        }
        if (weatherObj.Property("Night3 Summary").Value != "")
        {
            imgDay3Night.ToolTip = weatherObj.Property("Night3 Summary").Value;
        }
        else
        {
            imgDay3Night.ToolTip = "Sorry, No Information Available!";
        }
        if (weatherObj.Property("Night4 Summary").Value != "")
        {
            imgDay4Night.ToolTip = weatherObj.Property("Night4 Summary").Value;
        }
        else
        {
            imgDay4Night.ToolTip = "Sorry, No Information Available!";
        }

        if (weatherObj.Property("Night5 Summary").Value != "")
        {
            imgDay5Night.ToolTip = weatherObj.Property("Night5 Summary").Value;
        }
        else
        {
            imgDay5Night.ToolTip = "Sorry, No Information Available!";
        }
    }
    #endregion

    #region Load Day Summary
    private void LoadDaySummaryLabels()
    {
        if (weatherObj.Property("Today Summary").Value != "")
        {
            imgTodayDay.ToolTip = weatherObj.Property("Today Summary").Value;
        }
        else
        {
            imgTodayDay.ToolTip = "Sorry, No Information Available!";
        }
        if (weatherObj.Property("Day1 Summary").Value != "")
        {
            imgDay1Day.ToolTip = weatherObj.Property("Day1 Summary").Value;
        }
        else
        {
            imgDay1Day.ToolTip = "Sorry, No Information Available!";
        }
        if (weatherObj.Property("Day2 Summary").Value != "")
        {
            imgDay2Day.ToolTip = weatherObj.Property("Day2 Summary").Value;
        }
        else
        {
            imgDay2Day.ToolTip = "Sorry, No Information Available!";
        }
        if (weatherObj.Property("Day3 Summary").Value != "")
        {
            imgDay3Day.ToolTip = weatherObj.Property("Day3 Summary").Value;
        }
        else
        {
            imgDay3Day.ToolTip = "Sorry, No Information Available!";
        }
        if (weatherObj.Property("Day4 Summary").Value != "")
        {
            imgDay4Day.ToolTip = weatherObj.Property("Day4 Summary").Value;
        }
        else
        {
            imgDay4Day.ToolTip = "Sorry, No Information Available!";
        }
        if (weatherObj.Property("Day5 Summary").Value != "")
        {
            imgDay5Day.ToolTip = weatherObj.Property("Day5 Summary").Value;
        }
        else
        {
            imgDay5Day.ToolTip = "Sorry, No Information Available!";
        }
    }
    #endregion

    #region Load Day Labels
    private void LoadDayLabels()
    {
        lblDay1.ToolTip = weatherObj.Property("Day1 Forecast").Value;
        lblDay2.ToolTip = weatherObj.Property("Day2 Forecast").Value;
        lblDay3.ToolTip = weatherObj.Property("Day3 Forecast").Value;
        lblDay4.ToolTip = weatherObj.Property("Day4 Forecast").Value;
        lblDay5.ToolTip = weatherObj.Property("Day5 Forecast").Value;
    }
    #endregion

    #region Load Highs
    private void LoadHighs()
    {
        lblTodayHi.Text = string.Format("High: {0}°", weatherObj.Property("Day1 High").Value);
        lblDay1Hi.Text = string.Format("High: {0}°", weatherObj.Property("Day1 High").Value);
        lblDay2Hi.Text = string.Format("High: {0}°", weatherObj.Property("Day2 High").Value);
        lblDay3Hi.Text = string.Format("High: {0}°", weatherObj.Property("Day3 High").Value);
        lblDay4Hi.Text = string.Format("High: {0}°", weatherObj.Property("Day4 High").Value);
        lblDay5Hi.Text = string.Format("High: {0}°", weatherObj.Property("Day5 High").Value);
    }
    #endregion

    #region Load Lows
    private void LoadLows()
    {
        lblTodayLo.Text = string.Format("Low: {0}°", weatherObj.Property("Night1 Low").Value);
        lblDay1Lo.Text = string.Format("Low: {0}°", weatherObj.Property("Night1 Low").Value);
        lblDay2Lo.Text = string.Format("Low: {0}°", weatherObj.Property("Night2 Low").Value);
        lblDay3Lo.Text = string.Format("Low: {0}°", weatherObj.Property("Night3 Low").Value);
        lblDay4Lo.Text = string.Format("Low: {0}°", weatherObj.Property("Night4 Low").Value);
        lblDay5Lo.Text = string.Format("Low: {0}°", weatherObj.Property("Night5 Low").Value);
    }
    #endregion

    #region Load Dates
    private void LoadDates()
    {
        lblDay1.Text = DateTime.Now.AddDays(0).DayOfWeek.ToString();
        lblDay2.Text = DateTime.Now.AddDays(1).DayOfWeek.ToString();
        lblDay3.Text = DateTime.Now.AddDays(2).DayOfWeek.ToString();
        lblDay4.Text = DateTime.Now.AddDays(3).DayOfWeek.ToString();
        lblDay5.Text = DateTime.Now.AddDays(4).DayOfWeek.ToString();
    }
    #endregion

    #region Load Image Controls
    private void LoadImageControls()
    {
        LoadImages("Today Image", imgTodayDay);
        LoadImages("Tonight Image", imgTodayNight);
        LoadImages("Day1 Image", imgDay1Day);
        LoadImages("Day2 Image", imgDay2Day);
        LoadImages("Day3 Image", imgDay3Day);
        LoadImages("Day4 Image", imgDay4Day);
        LoadImages("Day5 Image", imgDay5Day);
        LoadImages("Night1 Image", imgDay1Night);
        LoadImages("Night2 Image", imgDay2Night);
        LoadImages("Night3 Image", imgDay3Night);
        LoadImages("Night4 Image", imgDay4Night);
        LoadImages("Night5 Image", imgDay5Night);
    }
    #endregion

    #region Load Images
    private void LoadImages(string key, Image imageBox)
    {
        dynamic imageName = weatherObj.Property(key).Value;
        if (string.IsNullOrEmpty(imageName))
        {
            imageBox.Visible = false;
        }
        else
        {
            Uri url = new Uri(imageName);
            imageBox.ImageUrl = url.ToString();
        }
    }
    #endregion
}