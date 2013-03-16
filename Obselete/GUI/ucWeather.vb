Imports System.IO
Imports System.Net
Public Class ucWeather
    Private sMode As String = "Max"
    Private lastUpdate As String
    Private Const STR_DATANAME = "Weather Data"
    Private Sub UserControl1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Load_All_Weather()
    End Sub
    Private Function GetValue(weatherObj As OSAEObject, keyValue As String) As String
        Return (From j In weatherObj.Properties Where j.Name = keyValue Select j.Value).DefaultIfEmpty().FirstOrDefault()
    End Function

    Private Sub Load_All_Weather()
        Dim weatherObj = OSAEApi.GetObjectByName(STR_DATANAME)
        lblTemperatureCurrent.Text = GetValue(weatherObj, "Temp") & "°"
        lblConditions.Text = GetValue(weatherObj, "Today Summary")
        lastUpdate = GetValue(weatherObj, "Last Updated")
        lblLastUpdated.Text = lastUpdate

        LoadLows(weatherObj)
        LoadHighs(weatherObj)
        LoadDayLabels(weatherObj)
        LoadDaySummaryLabels(weatherObj)
        LoadNightSummaryLabels(weatherObj)
        LoadDates()
        LoadImageControls()

        Label2.Text = ""
    End Sub
    Private Sub LoadNightSummaryLabels(weatherObj As OSAEObject)
        picNight1.Tag = GetValue(weatherObj, "Night1 Summary")
        picNight2.Tag = GetValue(weatherObj, "Night2 Summary")
        picNight3.Tag = GetValue(weatherObj, "Night3 Summary")
        picNight4.Tag = GetValue(weatherObj, "Night4 Summary")
        picNight5.Tag = GetValue(weatherObj, "Night5 Summary")
    End Sub
    Private Sub LoadDaySummaryLabels(weatherObj As OSAEObject)
        picDay1.Tag = GetValue(weatherObj, "Day1 Summary")
        picDay2.Tag = GetValue(weatherObj, "Day2 Summary")
        picDay3.Tag = GetValue(weatherObj, "Day3 Summary")
        picDay4.Tag = GetValue(weatherObj, "Day4 Summary")
        picDay5.Tag = GetValue(weatherObj, "Day5 Summary")
    End Sub
    Private Sub LoadDayLabels(weatherObj As OSAEObject)
        lblDay1.Tag = GetValue(weatherObj, "Day1 Label")
        lblDay2.Tag = GetValue(weatherObj, "Day2 Label")
        lblDay3.Tag = GetValue(weatherObj, "Day3 Label")
        lblDay4.Tag = GetValue(weatherObj, "Day4 Label")
        lblDay5.Tag = GetValue(weatherObj, "Day5 Label")
    End Sub
    Private Sub LoadHighs(weatherObj As OSAEObject)
        lblHighCurrent.Text = String.Format("High: {0}°", GetValue(weatherObj, "Day1 High"))
        lblHigh1.Text = String.Format("High: {0}°", GetValue(weatherObj, "Day1 High"))
        lblHigh2.Text = String.Format("High: {0}°", GetValue(weatherObj, "Day2 High"))
        lblHigh3.Text = String.Format("High: {0}°", GetValue(weatherObj, "Day3 High"))
        lblHigh4.Text = String.Format("High: {0}°", GetValue(weatherObj, "Day4 High"))
        lblHigh5.Text = String.Format("High: {0}°", GetValue(weatherObj, "Day5 High"))
    End Sub
    Private Sub LoadLows(weatherObj As OSAEObject)
        lblLowCurrent.Text = String.Format("Low: {0}°", GetValue(weatherObj, "Night1 Low"))
        lblLow1.Text = String.Format("Low: {0}°", GetValue(weatherObj, "Night1 Low"))
        lblLow2.Text = String.Format("Low: {0}°", GetValue(weatherObj, "Night2 Low"))
        lblLow3.Text = String.Format("Low: {0}°", GetValue(weatherObj, "Night3 Low"))
        lblLow4.Text = String.Format("Low: {0}°", GetValue(weatherObj, "Night4 Low"))
        lblLow5.Text = String.Format("Low: {0}°", GetValue(weatherObj, "Night5 Low"))
    End Sub
    Private Sub LoadDates()
        lblDay1.Text = Format(Now.AddDays(0), "ddd")
        lblDay2.Text = Format(Now.AddDays(1), "ddd")
        lblDay3.Text = Format(Now.AddDays(2), "ddd")
        lblDay4.Text = Format(Now.AddDays(3), "ddd")
        lblDay5.Text = Format(Now.AddDays(4), "ddd")
    End Sub
    Private Sub LoadImageControls()
        LoadImages("Today Image", picToday)
        LoadImages("Tonight Image", picTonight)
        LoadImages("Day1 Image", picDay1)
        LoadImages("Day2 Image", picDay2)
        LoadImages("Day3 Image", picDay3)
        LoadImages("Day4 Image", picDay4)
        LoadImages("Day5 Image", picDay5)
        LoadImages("Night1 Image", picNight1)
        LoadImages("Night2 Image", picNight2)
        LoadImages("Night3 Image", picNight3)
        LoadImages("Night4 Image", picNight4)
        LoadImages("Night5 Image", picNight5)
    End Sub
    Private Sub LoadImages(key As String, imageBox As PictureBox)
        Dim imageName = OSAEApi.GetObjectPropertyValue(STR_DATANAME, key).Value
        If imageName = "" Then Exit Sub

        Dim url As New Uri(imageName)
        Dim path As String = String.Format("{0}\images\Weather{1}", OSAEApi.APIpath, url.AbsolutePath.Replace("/", "\"))
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path))
        If File.Exists(path) Then
            imageBox.Image = New Bitmap(New MemoryStream(New WebClient().DownloadData(url.OriginalString)))
        Else
            Try
                imageBox.Image = New Bitmap(New MemoryStream(New WebClient().DownloadData(url.OriginalString)))
                imageBox.Image.Save(path)

            Catch ex As Exception
                logging.AddToLog("Unable to download weather image " & url.OriginalString, True)
            End Try
        End If
    End Sub
    Private Sub ucWeather_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles picDay1.MouseDoubleClick, picNight1.MouseDoubleClick, picDay2.MouseDoubleClick, picNight2.MouseDoubleClick, picDay3.MouseDoubleClick, picNight3.MouseDoubleClick, picDay4.MouseDoubleClick, picNight4.MouseDoubleClick, picDay5.MouseDoubleClick, picNight5.MouseDoubleClick, picToday.MouseDoubleClick, picTonight.MouseDoubleClick, lblTemperatureCurrent.MouseDoubleClick, lblConditions.MouseDoubleClick, lblHighCurrent.MouseDoubleClick, lblLowCurrent.MouseDoubleClick, MyBase.MouseDoubleClick
        If sMode = "Max" Then
            sMode = "Min"
            Me.Width = picDay1.Left - 6
            Me.Parent.Width = Me.Width
            lblLastUpdated.Visible = False
        Else
            sMode = "Max"
            Me.Width = picDay5.Left + picDay5.Width + 8
            Me.Parent.Width = Me.Width
            lblLastUpdated.Visible = True
        End If
    End Sub

    Private Sub imageHover(sender As Object, e As EventArgs) Handles lblDay1.MouseHover, lblDay5.MouseHover, lblDay4.MouseHover, lblDay3.MouseHover, lblDay2.MouseHover, picDay5.MouseHover, picDay4.MouseHover, picDay3.MouseHover, picDay2.MouseHover, picDay1.MouseHover, picNight5.MouseHover, picNight4.MouseHover, picNight3.MouseHover, picNight2.MouseHover, picNight1.MouseHover
        Label2.Text = sender.Tag
    End Sub

    Private Sub imageLeave(sender As Object, e As EventArgs) Handles lblDay1.MouseLeave, lblDay5.MouseLeave, lblDay4.MouseLeave, lblDay3.MouseLeave, lblDay2.MouseLeave, picDay5.MouseLeave, picDay4.MouseLeave, picDay3.MouseLeave, picDay2.MouseLeave, picDay1.MouseLeave, picNight5.MouseLeave, picNight4.MouseLeave, picNight3.MouseLeave, picNight2.MouseLeave, picNight1.MouseLeave
        Label2.Text = String.Empty
    End Sub

    Private Sub timMain_Tick(sender As Object, e As EventArgs) Handles timMain.Tick
        timMain.Enabled = False
        Dim currUpdate = OSAEApi.GetObjectPropertyValue(STR_DATANAME, "Last Updated").Value
        If currUpdate <> lastUpdate Then
            lastUpdate = currUpdate
            lblLastUpdated.Text = lastUpdate
            Load_All_Weather()
        End If
        timMain.Enabled = True
    End Sub
End Class
