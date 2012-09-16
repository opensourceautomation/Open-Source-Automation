Imports System.IO
Public Class ucWeather
    Private sMode As String = "Max"
    Private sSettings As String = "Off"
    Private pProperty As New ObjectProperty
    Private Sub UserControl1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Load_All_Weather()
    End Sub
    Private Sub Load_All_Weather()
        Dim sTemp As String, sPath As String
        pProperty = OSAEApi.GetObjectPropertyValue("Weather Data", "Temp")
        sTemp = pProperty.Value
        sTemp = sTemp & "°"
        lblTemperatureCurrent.Text = sTemp
        lblConditions.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Today Summary").Value
        lblLowCurrent.Text = "Low: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Night1 Low").Value & "°"
        lblHighCurrent.Text = "High: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Day1 High").Value & "°"
        lblLow1.Text = "Low: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Night1 Low").Value & "°"
        lblLow2.Text = "Low: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Night2 Low").Value & "°"
        lblLow3.Text = "Low: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Night3 Low").Value & "°"
        lblLow4.Text = "Low: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Night4 Low").Value & "°"
        lblLow5.Text = "Low: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Night5 Low").Value & "°"
        lblLow6.Text = "Low: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Night6 Low").Value & "°"
        lblHigh1.Text = "High: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Day1 High").Value & "°"
        lblHigh2.Text = "High: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Day2 High").Value & "°"
        lblHigh3.Text = "High: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Day3 High").Value & "°"
        lblHigh4.Text = "High: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Day4 High").Value & "°"
        lblHigh5.Text = "High: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Day5 High").Value & "°"
        lblHigh6.Text = "High: " & OSAEApi.GetObjectPropertyValue("Weather Data", "Day6 High").Value & "°"
        lblDay1.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day1 Label").Value
        lblDay2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day2 Label").Value
        lblDay3.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day3 Label").Value
        lblDay4.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day4 Label").Value
        lblDay5.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day5 Label").Value
        lblDay6.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day6 Label").Value
        Try
            'sPath = gAppPath & OSAEApi.GetObjectProperty("Weather Data", "Today Image").Split(".")(0) & ".jpg"
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Today Image").Value
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picToday.Image = Image.FromStream(msToday)
            End If
            ' sPath = gAppPath & OSAEApi.GetObjectProperty("Weather Data", "Tonight Image").Split(".")(0) & ".jpg"
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Tonight Image").Value
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picTonight.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Day1 Image").Value '.Split(".")(0) & ".jpg"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picDay1.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Day2 Image").Value '.Split(".")(0) & ".jpg"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picDay2.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Day3 Image").Value '.Split(".")(0) & ".jpg"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picDay3.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Day4 Image").Value '.Split(".")(0) & ".jpg"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picDay4.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Day5 Image").Value '.Split(".")(0) & ".png"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picDay5.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Day6 Image").Value '.Split(".")(0) & ".jpg"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picDay6.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Night1 Image").Value '.Split(".")(0) & ".jpg"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picNight1.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Night2 Image").Value '.Split(".")(0) & ".jpg"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picNight2.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Night3 Image").Value '.Split(".")(0) & ".png"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picNight3.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Night4 Image").Value '.Split(".")(0) & ".jpg"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picNight4.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Night5 Image").Value '.Split(".")(0) & ".png"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picNight5.Image = Image.FromStream(msToday)
            End If
            sPath = gAppPath & OSAEApi.GetObjectPropertyValue("Weather Data", "Night6 Image").Value '.Split(".")(0) & ".jpg"
            If File.Exists(sPath) Then
                Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
                picNight6.Image = Image.FromStream(msToday)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try



    End Sub

    'Private Sub ucWeather_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseEnter
    '    Me.Width = 632
    'End Sub

    'Private Sub ucWeather_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseLeave
    '    Me.Width = 94
    'End Sub

    Private Sub ucWeather_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseClick, picDay1.MouseClick, picNight1.MouseClick, picDay2.MouseClick, picNight2.MouseClick, picDay3.MouseClick, picNight3.MouseClick, picDay4.MouseClick, picNight4.MouseClick, picDay5.MouseClick, picNight5.MouseClick, picToday.MouseClick, picTonight.MouseClick, lblTemperatureCurrent.MouseClick, lblConditions.MouseClick, lblHighCurrent.MouseClick, lblLowCurrent.MouseClick
        If sMode = "Max" Then
            sMode = "Min"
            Me.Width = 580
        Else
            sMode = "Max"
            Me.Width = 112
        End If
    End Sub

    Private Sub lblSettings_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
        If sSettings = "Off" Then
            sSettings = "On"
            Me.Height = 350
        Else
            sSettings = "Off"
            Me.Height = 282
        End If
    End Sub

    Private Sub picDay1_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay1.MouseHover
        Label2.Text = OSAEApi.GetObjectProperty("Weather Data", "Day1 Summary")
    End Sub

    Private Sub picDay1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay1.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub picDay2_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay2.MouseHover
        Label2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day2 Summary").Value
    End Sub

    Private Sub picDay2_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay2.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub picDay3_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay3.MouseHover
        Label2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day3 Summary").Value
    End Sub

    Private Sub picDay3_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay3.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub picDay4_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay4.MouseHover
        Label2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day4 Summary").Value

    End Sub

    Private Sub picDay4_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay4.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub picDay5_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay5.MouseHover
        Label2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day5 Summary").Value
    End Sub

    Private Sub picDay5_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay5.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub picDay6_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay6.MouseHover
        Label2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Day6 Summary").Value
    End Sub

    Private Sub picDay6_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picDay6.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub PicNight1_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight1.MouseHover
        Label2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Night1 Summary").Value
    End Sub

    Private Sub PicNight1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight1.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub PicNight2_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight2.MouseHover
        Label2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Night2 Summary").Value
    End Sub

    Private Sub PicNight2_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight2.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub PicNight3_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight3.MouseHover
        Label2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Night3 Summary").Value
    End Sub

    Private Sub PicNight3_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight3.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub PicNight4_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight4.MouseHover
        Label2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Night4 Summary").Value
    End Sub

    Private Sub PicNight4_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight4.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub PicNight5_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight5.MouseHover
        Label2.Text = OSAEApi.GetObjectPropertyValue("Weather Data", "Night5 Summary").Value
    End Sub

    Private Sub PicNight5_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight5.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub PicNight6_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight6.MouseHover
        ' 
        pProperty = OSAEApi.GetObjectPropertyValue("Weather Data", "Night6 Summary")
        ' Label2.Text = pProperty.
    End Sub

    Private Sub PicNight6_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picNight6.MouseLeave
        Label2.Text = ""
    End Sub

    Private Sub Label2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label2.Click

    End Sub


End Class
