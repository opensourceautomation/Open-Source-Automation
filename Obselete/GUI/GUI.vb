Option Explicit On
Imports VB = Microsoft.VisualBasic
Imports MySql.Data.MySqlClient
Imports System.IO
Public Class GUI
    'Protected Sub SetStyle(ByVal flag As ControlStyles, ByVal value As Boolean)
    Private WithEvents aControlStateImage As ControlStateImage
    Private WithEvents aControlPropertyLabel As ControlPropertyLabel
    Private WithEvents aControlStaticLabel As ControlStaticLabel
    Private WithEvents aControlTimerLabel As ControlTimerLabel
    Private WithEvents aControlMethodImage As ControlMethodImage
    Private WithEvents aControlNavImage As ControlNavImage
    Private WithEvents aControlUserControl As ControlUserControl
    Private g_toolTip As ToolTip = Nothing
    Private dragging As Boolean
    Private mousex As Integer
    Private mousey As Integer
    Private ucSlider As New ucSlider1
    Private gLastRefresh As DateTime
    Public CNTimer As MySqlConnection
    Private gXOriginal As Integer
    Private gYOriginal As Integer
    Private gXLast As Integer
    Private gYLast As Integer
    Private gXRatio As Decimal
    Private gYRatio As Decimal
    Private gXFullRatio As Decimal
    Private gYFullRatio As Decimal


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        EnableDoubleBuffering()
        OSAEApi = New OSAE("GUI")
        DB_Connection()
        Load_App_Name()
        OSAEApi.ObjectStateSet(gAppName, "ON")
        g_toolTip = New ToolTip()
        g_toolTip.AutoPopDelay = 0
        g_toolTip.InitialDelay = 0
        g_toolTip.ReshowDelay = 0
        g_toolTip.ShowAlways = True
        Controls_Load()
        'Load_Dropdowns()
        gCurrentScreen = OSAEApi.GetObjectPropertyValue(gAppName, "Default Screen").Value
        If gCurrentScreen = "" Then
            Set_Default_Screen()
        End If
        Load_Screen(gCurrentScreen)
        Timer1.Enabled = True
        ' Me.Controls.Add(ucSlider)
        'TEST_Load_Weather()
    End Sub

    Public Sub EnableDoubleBuffering()
        ' Set the value of the double-buffering style bits to true.
        Me.SetStyle(ControlStyles.DoubleBuffer _
          Or ControlStyles.UserPaint _
          Or ControlStyles.AllPaintingInWmPaint, _
          True)
        Me.UpdateStyles()
    End Sub

    Public Sub DB_Connection()
        CN = New MySqlConnection
        CN.ConnectionString = "server=" & OSAEApi.DBConnection & ";Port=" & OSAEApi.DBPort & ";Database=" & OSAEApi.DBName & ";Password=" & OSAEApi.DBPassword & ";use procedure bodies=false;Persist Security Info=True;User ID=" & OSAEApi.DBUsername
        CN2 = New MySqlConnection
        CN2.ConnectionString = "server=" & OSAEApi.DBConnection & ";Port=" & OSAEApi.DBPort & ";Database=" & OSAEApi.DBName & ";Password=" & OSAEApi.DBPassword & ";use procedure bodies=false;Persist Security Info=True;User ID=" & OSAEApi.DBUsername
        CNTimer = New MySqlConnection
        CNTimer.ConnectionString = "server=" & OSAEApi.DBConnection & ";Port=" & OSAEApi.DBPort & ";Database=" & OSAEApi.DBName & ";Password=" & OSAEApi.DBPassword & ";use procedure bodies=false;Persist Security Info=True;User ID=" & OSAEApi.DBUsername

        Try
            CN.Open()
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Connecting to Database: " & myerror.Message)
        End Try
    End Sub

    Private Sub Set_Default_Screen()
        Dim CMD As New MySqlCommand
        Dim sScreen As String = ""
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE base_type='SCREEN' LIMIT 1"
        Try
            CN.Open()
            gCurrentScreen = CMD.ExecuteScalar
            OSAEApi.ObjectPropertySet(gAppName, "Default Screen", gCurrentScreen)
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Set_Default_Screen(): " & myerror.Message)
            CN.Close()
        End Try
    End Sub

    Private Sub Controls_Load()
        Dim iLoop As Integer
        aControlStateImage = New ControlStateImage(Me)
        aControlPropertyLabel = New ControlPropertyLabel(Me)
        aControlMethodImage = New ControlMethodImage(Me)
        aControlNavImage = New ControlNavImage(Me)
        aControlStaticLabel = New ControlStaticLabel(Me)
        aControlTimerLabel = New ControlTimerLabel(Me)
        aControlUserControl = New ControlUserControl(Me)
        For iLoop = 1 To 100
            aControlStateImage.AddNewControlStateImage()
            AddHandler aControlStateImage(iLoop).Click, AddressOf ClickControlStateImage
            AddHandler aControlStateImage(iLoop).MouseDown, AddressOf ControlStateImageClick
            AddHandler aControlStateImage(iLoop).MouseMove, AddressOf ControlStateImageMove
            AddHandler aControlStateImage(iLoop).MouseUp, AddressOf ControlStateImageUp
        Next iLoop
        For iLoop = 1 To 30
            aControlPropertyLabel.AddNewPropertyLabel()
            AddHandler aControlPropertyLabel(iLoop).MouseDown, AddressOf ControlPropertyLabelClick
            AddHandler aControlPropertyLabel(iLoop).MouseMove, AddressOf ControlPropertyLabelMove
            AddHandler aControlPropertyLabel(iLoop).MouseUp, AddressOf ControlPropertyLabelUp
            AddHandler aControlPropertyLabel(iLoop).GotFocus, AddressOf  ControlPropertyLabelSIC
        Next iLoop
        For iLoop = 1 To 10
            aControlMethodImage.AddNewMethodImage()
            AddHandler aControlMethodImage(iLoop).Click, AddressOf ClickControlMethodImage
            AddHandler aControlMethodImage(iLoop).MouseDown, AddressOf ControlMethodImageClick
            AddHandler aControlMethodImage(iLoop).MouseMove, AddressOf ControlMethodImageMove
            AddHandler aControlMethodImage(iLoop).MouseUp, AddressOf ControlMethodImageUp
        Next iLoop
        For iLoop = 1 To 10
            aControlNavImage.AddNewNavImage()
            AddHandler aControlNavImage(iLoop).Click, AddressOf ClickControlNavImage
            AddHandler aControlNavImage(iLoop).MouseDown, AddressOf ControlNavImageClick
            AddHandler aControlNavImage(iLoop).MouseMove, AddressOf ControlNavImageMove
            AddHandler aControlNavImage(iLoop).MouseUp, AddressOf ControlNavImageUp
        Next iLoop
        For iLoop = 1 To 10
            aControlStaticLabel.AddNewStaticLabel()
            AddHandler aControlStaticLabel(iLoop).MouseDown, AddressOf ControlStaticLabelClick
            AddHandler aControlStaticLabel(iLoop).MouseMove, AddressOf ControlStaticLabelMove
            AddHandler aControlStaticLabel(iLoop).MouseUp, AddressOf ControlStaticLabelUp
        Next iLoop
        For iLoop = 1 To 50
            aControlTimerLabel.AddNewStaticLabel()
            AddHandler aControlTimerLabel(iLoop).MouseDown, AddressOf ControlTimerLabelClick
            AddHandler aControlTimerLabel(iLoop).MouseMove, AddressOf ControlTimerLabelMove
            AddHandler aControlTimerLabel(iLoop).MouseUp, AddressOf ControlTimerLabelUp
        Next iLoop
        For iLoop = 1 To 10
            aControlUserControl.AddNewPanel()
            'AddHandler aControlUserControl(iLoop).MouseDown, AddressOf ControlUserControlClick
            'AddHandler aControlUserControl(iLoop).MouseMove, AddressOf ControlUserControlMove
            'AddHandler aControlUserControl(iLoop).MouseUp, AddressOf ControlUserControlUp
        Next iLoop
    End Sub

    Private Sub Controls_Clear()
        Dim iLoop As Integer
        For iLoop = 1 To iStateImageCount
            aControlStateImage(iLoop).Visible = False
            Application.DoEvents()
        Next iLoop
        iStateImageCount = 0
        iStateImageList = ""
        Application.DoEvents()

        For iLoop = 1 To iPropertyLabelCount
            aControlPropertyLabel(iLoop).Visible = False
        Next iLoop
        iPropertyLabelCount = 0
        Application.DoEvents()

        For iLoop = 1 To iMethodImageCount
            aControlMethodImage(iLoop).Visible = False
        Next iLoop
        iMethodImageCount = 0
        Application.DoEvents()

        For iLoop = 1 To iNavImageCount
            aControlNavImage(iLoop).Visible = False
        Next iLoop
        iNavImageCount = 0
        Application.DoEvents()

        For iLoop = 1 To iStaticLabelCount
            aControlStaticLabel(iLoop).Visible = False
        Next iLoop
        iStaticLabelCount = 0
        Application.DoEvents()

        For iLoop = 1 To iTimerLabelCount
            aControlTimerLabel(iLoop).Visible = False
        Next iLoop
        iTimerLabelCount = 0
        Application.DoEvents()

        For iLoop = 1 To iUserControlCount
            aControlUserControl(iLoop).Visible = False
        Next iLoop
        iUserControlCount = 0
        Application.DoEvents()


    End Sub

    Private Sub Load_App_Name()
        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_name FROM osae_v_object_property WHERE base_type='GUI CLIENT' AND property_name='Computer Name' AND property_value='" & OSAEApi.ComputerName & "'"
        Try
            CN.Open()
            gAppName = CMD.ExecuteScalar
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_App_Name: " & myerror.Message)
            CN.Close()
        End Try
        If gAppName = "" Then
            OSAEApi.ObjectAdd("GUI CLIENT " & OSAEApi.ComputerName, "GUI CLIENT " & OSAEApi.ComputerName, "GUI CLIENT", "", "SYSTEM", True)
            OSAEApi.ObjectPropertySet("GUI CLIENT " & OSAEApi.ComputerName, "Computer Name", OSAEApi.ComputerName)
            ' Load_App_Name()
        End If
        gAppPath = OSAEApi.APIpath
    End Sub

    Private Sub cboScreens_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Load_Screen(gCurrentScreen)
    End Sub

    Public Sub Load_Screen(ByVal sScreen As String)
        Dim sPath As String, iOldHeight As Integer
        'Me.BackgroundImage = Nothing
        iOldHeight = Me.Height
        gCurrentScreen = sScreen
        OSAEApi.ObjectPropertySet(gAppName, "Current Screen", sScreen)
        sPath = OSAEApi.GetObjectPropertyValue(sScreen, "Background Image").Value

        'sPath = sPath.Replace(".\", OSAEApi.APIpath & "\")
        sPath = OSAEApi.APIpath + sPath

        If gCurrentScreen <> sScreen Then gCurrentScreen = sScreen
        Controls_Clear()
        gXRatio = 1
        gYRatio = 1
        gXFullRatio = 1
        gYFullRatio = 1

        Application.DoEvents()
        Load_Objects(gCurrentScreen)
        Load_User_Controls(gCurrentScreen)
        Application.DoEvents()
        If File.Exists(sPath) Then
            Dim msToday As MemoryStream = New MemoryStream(File.ReadAllBytes(sPath))
            Me.BackgroundImage = Image.FromStream(msToday)
            Me.Height = Me.BackgroundImage.Height + 34
            Me.Width = Me.BackgroundImage.Width + 12
            gXOriginal = Me.Width
            gYOriginal = Me.Height
            gXLast = Me.Width
            gYLast = Me.Height

        End If
    End Sub

    Private Sub Process_Queue()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        Dim sMethod As String, sParam1 As String, iQueueID As Integer
        CMD.Connection = CNTimer
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT * FROM osae_v_method_queue WHERE object_name='" & gAppName & "'"
        Try
            CNTimer.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                sMethod = Convert.ToString(myReader.Item("method_name"))
                sParam1 = Convert.ToString(myReader.Item("parameter_1"))
                iQueueID = Convert.ToInt16(myReader.Item("method_queue_id"))
                If sMethod = "SCREEN SET" Then
                    gCurrentScreen = sParam1
                End If
                OSAEApi.MethodQueueDelete(iQueueID)
            End While
            CNTimer.Close()
        Catch myerror As MySqlException
            MessageBox.Show("GUI Error Process_Queue: " & myerror.Message, "GUI")
            CNTimer.Close()
        End Try
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
        Process_Queue()
        Update_Objects()
        Timer1.Enabled = True
    End Sub

#Region "State Image"

    Private Sub ClickControlStateImage(ByVal sender As Object, ByVal e As System.EventArgs)
        If mnuEditMode.Checked = False Then
            Dim outputStatus As String
            Dim CMD As New MySqlCommand
            Dim iResults As Integer
            CMD.Connection = CN
            CMD.CommandType = CommandType.Text
            outputStatus = DirectCast(sender, PictureBox).Tag
            If aScreenObject(outputStatus).Object_State = "ON" Then
                CMD.CommandText = "SELECT COUNT(*) FROM osae_v_object_method WHERE object_name='" & aScreenObject(outputStatus).Object_Name & "' AND method_name='OFF'"
                Try
                    CN.Open()
                    iResults = CMD.ExecuteScalar
                    CN.Close()
                Catch myerror As MySqlException
                    MessageBox.Show("Error ClickControlStateImage: " & myerror.Message)
                    CN.Close()
                End Try
                If iResults > 0 Then
                    OSAEApi.MethodQueueAdd(aScreenObject(outputStatus).Object_Name, "OFF", "", "")
                Else
                    OSAEApi.ObjectStateSet(aScreenObject(outputStatus).Object_Name, "OFF")
                End If
            Else
                CMD.CommandText = "SELECT COUNT(*) FROM osae_v_object_method WHERE object_name='" & aScreenObject(outputStatus).Object_Name & "' AND method_name='ON'"
                Try
                    CN.Open()
                    iResults = CMD.ExecuteScalar
                    CN.Close()
                Catch myerror As MySqlException
                    '    MessageBox.Show("Error Process_Event_Queue: " & myerror.Message)
                    CN.Close()
                End Try
                If iResults > 0 Then
                    OSAEApi.MethodQueueAdd(aScreenObject(outputStatus).Object_Name, "ON", "", "")
                Else
                    OSAEApi.ObjectStateSet(aScreenObject(outputStatus).Object_Name, "ON")
                End If
            End If
        End If
        Update_Objects()
    End Sub

    Public Sub ControlStateImageClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then     ' Find out if it is in Drag and Drop mode
            If e.Button = Windows.Forms.MouseButtons.Right Then  'Prosedure to move an image from the workspace. Using Mousebutton right
                Dim Response As MsgBoxResult = MsgBox("Do you want to remove this object", MsgBoxStyle.YesNo, "Id = " & sender.tag)
                If Response = MsgBoxResult.Yes Then   ' User chose Yes.
                    Me.Controls.Remove(sender)         'Remove from workspace
                    OSAEApi.ObjectDelete(aScreenObject(sender.tag).Control_Name)    'Remove from database
                End If
            End If

            'Procedure to select the image
            If e.Button = Windows.Forms.MouseButtons.Left Then
                Me.Cursor = Cursors.Hand
                dragging = True
                mousex = -e.X
                mousey = -e.Y
                Dim clipleft As Integer = Me.PointToClient(MousePosition).X - sender.Location.X()
                Dim cliptop As Integer = Me.PointToClient(MousePosition).Y - sender.Location.Y()
                Dim clipwidth As Integer = Me.ClientSize.Width - (sender.Width - clipleft)
                Dim clipheight As Integer = Me.ClientSize.Height - (sender.Height - cliptop)
                Windows.Forms.Cursor.Clip = Me.RectangleToScreen(New Rectangle(clipleft, cliptop, clipwidth, clipheight))
                sender.Invalidate()
            End If
        End If
    End Sub

    Public Sub ControlStateImageMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then    ' Find out if it is in Drag and Drop mode
            If dragging Then
                'move control to new position
                Dim MPosition As New Point()
                MPosition = Me.PointToClient(MousePosition)
                MPosition.Offset(mousex, mousey)
                'ensure control cannot leave container
                sender.Location = MPosition
            End If
        End If
    End Sub

    Private Sub ControlStateImageUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim sControlName As String = "", sState As String, sStateMatch As String ', sTemp As String
        Dim CMD As New MySqlCommand, sState1 As Integer, sState2 As Integer, sState3 As Integer
        ' Find out if it is in Drag and Drop mode
        If mnuEditMode.Checked = True Then
            If dragging Then
                ' sControlName = aScreenObject(aControlStateImage(sender.tag).Text).Control_Name
                sControlName = aScreenObject(sender.tag).Control_Name
                'sTemp = OSAEApi.GetObjectState(aScreenObject(sender.tag).Control_Name)
                sState = aScreenObject(sender.tag).Object_State
                CMD.Connection = CN
                CMD.CommandType = CommandType.Text
                CMD.CommandText = "SELECT property_name FROM osae_v_object_property WHERE object_name=?ObjectName AND property_value=?pstate"
                CMD.Parameters.AddWithValue("?ObjectName", sControlName)
                CMD.Parameters.AddWithValue("?pstate", sState)
                Try
                    CN.Open()
                    sStateMatch = CMD.ExecuteScalar
                    CN.Close()
                    If sStateMatch <> "" Then
                        sStateMatch = sStateMatch.Substring(0, 7)
                    End If
                    OSAEApi.ObjectPropertySet(sControlName, sStateMatch & " X", sender.Location.X)
                    OSAEApi.ObjectPropertySet(sControlName, sStateMatch & " Y", sender.Location.Y)
                    If sStateMatch <> "State 1" Then
                        sState1 = Val(OSAEApi.GetObjectPropertyValue(sControlName, "State 1 X").Value)
                        If (sState1 + 200) < sender.Location.X Or (sState1 - 200) > sender.Location.X Then
                            OSAEApi.ObjectPropertySet(sControlName, "State 1 X", sender.Location.X)
                        End If
                        sState1 = Val(OSAEApi.GetObjectPropertyValue(sControlName, "State 1 Y").Value)
                        If (sState1 + 200) < sender.Location.Y Or (sState1 - 200) > sender.Location.Y Then
                            OSAEApi.ObjectPropertySet(sControlName, "State 1 Y", sender.Location.Y)
                        End If
                    End If
                    If sStateMatch <> "State 2" Then
                        sState2 = Val(OSAEApi.GetObjectPropertyValue(sControlName, "State 2 X").Value)
                        If (sState2 + 200) < sender.Location.X Or (sState2 - 200) > sender.Location.X Then
                            OSAEApi.ObjectPropertySet(sControlName, "State 2 X", sender.Location.X)
                        End If
                        sState2 = Val(OSAEApi.GetObjectPropertyValue(sControlName, "State 2 Y").Value)
                        If (sState2 + 200) < sender.Location.Y Or (sState2 - 200) > sender.Location.Y Then
                            OSAEApi.ObjectPropertySet(sControlName, "State 2 Y", sender.Location.Y)
                        End If
                    End If
                    If sStateMatch <> "State 3" Then
                        sState3 = Val(OSAEApi.GetObjectPropertyValue(sControlName, "State 3 X").Value)
                        If (sState3 + 200) < sender.Location.X Or (sState3 - 200) > sender.Location.X Then
                            OSAEApi.ObjectPropertySet(sControlName, "State 3 X", sender.Location.X)
                        End If
                        sState3 = Val(OSAEApi.GetObjectPropertyValue(sControlName, "State 3 Y").Value)
                        If (sState3 + 200) < sender.Location.Y Or (sState3 - 200) > sender.Location.Y Then
                            OSAEApi.ObjectPropertySet(sControlName, "State 3 Y", sender.Location.Y)
                        End If
                    End If
                Catch myerror As MySqlException
                    MessageBox.Show("Error ControlStateImageUp: " & myerror.Message)
                    CN.Close()
                End Try
                dragging = False
                Windows.Forms.Cursor.Clip = Nothing
                sender.Invalidate()
            End If
            Me.Cursor = Cursors.Arrow
        End If
    End Sub
#End Region

#Region "Property Label"
    Public Sub ControlPropertyLabelClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then   ' Find out if it is in Drag and Drop mode
            'Prosedure to move an image from the workspace. Using Mousebutton right
            If e.Button = Windows.Forms.MouseButtons.Right Then
                Dim Response As MsgBoxResult = MsgBox("Do you want to remove this object", MsgBoxStyle.YesNo, "Id = " & sender.tag)
                If Response = MsgBoxResult.Yes Then   ' User chose Yes.
                    'Remove from workspace
                    Me.Controls.Remove(sender)
                    'Remove from database
                    OSAEApi.ObjectDelete(aScreenObject(aControlPropertyLabel(sender.tag).Tag).Control_Name)
                End If
            End If

            'Procedure to select the image
            If e.Button = Windows.Forms.MouseButtons.Left Then
                Me.Cursor = Cursors.Hand
                dragging = True
                mousex = -e.X
                mousey = -e.Y
                Dim clipleft As Integer = Me.PointToClient(MousePosition).X - sender.Location.X()
                Dim cliptop As Integer = Me.PointToClient(MousePosition).Y - sender.Location.Y()
                Dim clipwidth As Integer = Me.ClientSize.Width - (sender.Width - clipleft)
                Dim clipheight As Integer = Me.ClientSize.Height - (sender.Height - cliptop)
                Windows.Forms.Cursor.Clip = Me.RectangleToScreen(New Rectangle(clipleft, cliptop, clipwidth, clipheight))
                sender.Invalidate()

            End If
        End If
    End Sub
    Public Sub ControlPropertyLabelMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then        ' Find out if it is in Drag and Drop mode
            If dragging Then
                'move control to new position
                Dim MPosition As New Point()
                MPosition = Me.PointToClient(MousePosition)
                MPosition.Offset(mousex, mousey)
                'ensure control cannot leave container
                sender.Location = MPosition
            End If
        End If
    End Sub
    Private Sub ControlPropertyLabelUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim sControlName As String = "" ', iLoop As Integer, sState As String, sStateMatch As String, sTemp As String
        Dim CMD As New MySqlCommand
        ' Find out if it is in Drag and Drop mode
        If mnuEditMode.Checked = True Then
            If dragging Then
                sControlName = aScreenObject(sender.tag).Control_Name
                OSAEApi.ObjectPropertySet(sControlName, "X", sender.Location.X)
                OSAEApi.ObjectPropertySet(sControlName, "Y", sender.Location.Y)
                dragging = False
                Windows.Forms.Cursor.Clip = Nothing
                sender.Invalidate()
            End If
            Me.Cursor = Cursors.Arrow
        End If
    End Sub
    Private Sub ControlPropertyLabelSIC(ByVal sender As Object, ByVal e As System.EventArgs)
        aControlPropertyLabel(sender.tag).SelectionLength = 0
    End Sub
#End Region

#Region "Method Image"
    Private Sub ClickControlMethodImage(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim sObject, sMethod, sParam1, sParam2 As String
        Dim outputStatus As String = DirectCast(sender, PictureBox).Tag
        If mnuEditMode.Checked = False Then
            sObject = aScreenObject(outputStatus).Object_Name
            sMethod = OSAEApi.GetObjectPropertyValue(aScreenObject(outputStatus).Control_Name, "Method Name").Value
            sParam1 = OSAEApi.GetObjectPropertyValue(aScreenObject(outputStatus).Control_Name, "Param 1").Value
            sParam2 = OSAEApi.GetObjectPropertyValue(aScreenObject(outputStatus).Control_Name, "Param 2").Value
            OSAEApi.MethodQueueAdd(sObject, sMethod, sParam1, sParam2)
        End If
    End Sub

    Public Sub ControlMethodImageClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then     ' Find out if it is in Drag and Drop mode
            'Prosedure to move an image from the workspace. Using Mousebutton right
            If e.Button = Windows.Forms.MouseButtons.Right Then
                Dim Response As MsgBoxResult = MsgBox("Do you want to remove this object", MsgBoxStyle.YesNo, "Id = " & sender.tag)
                If Response = MsgBoxResult.Yes Then   ' User chose Yes.
                    Me.Controls.Remove(sender)      'Remove from workspace
                    OSAEApi.ObjectDelete(aScreenObject(aControlStateImage(sender.tag).Text).Control_Name)   'Remove from database
                End If
            End If

            'Procedure to select the image
            If e.Button = Windows.Forms.MouseButtons.Left Then
                Me.Cursor = Cursors.Hand
                dragging = True
                mousex = -e.X
                mousey = -e.Y
                Dim clipleft As Integer = Me.PointToClient(MousePosition).X - sender.Location.X()
                Dim cliptop As Integer = Me.PointToClient(MousePosition).Y - sender.Location.Y()
                Dim clipwidth As Integer = Me.ClientSize.Width - (sender.Width - clipleft)
                Dim clipheight As Integer = Me.ClientSize.Height - (sender.Height - cliptop)
                Windows.Forms.Cursor.Clip = Me.RectangleToScreen(New Rectangle(clipleft, cliptop, clipwidth, clipheight))
                sender.Invalidate()
            End If
        End If
    End Sub
    Public Sub ControlMethodImageMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then    ' Find out if it is in Drag and Drop mode
            If dragging Then
                'move control to new position
                Dim MPosition As New Point()
                MPosition = Me.PointToClient(MousePosition)
                MPosition.Offset(mousex, mousey)
                'ensure control cannot leave container
                sender.Location = MPosition
            End If
        End If
    End Sub
    Private Sub ControlMethodImageUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim sControlName As String = ""
        Dim CMD As New MySqlCommand
        ' Find out if it is in Drag and Drop mode
        If mnuEditMode.Checked = True Then
            If dragging Then
                sControlName = aScreenObject(sender.tag).Control_Name
                'sTemp = OSAEApi.GetObjectState(aScreenObject(sender.tag).Control_Name)
                'sState = aScreenObject(sender.tag).Object_State
                'CMD.Connection = CN
                'CMD.CommandType = CommandType.Text
                'CMD.CommandText = "SELECT property_name FROM osae_v_object_property WHERE object_name=?ObjectName AND property_value=?pstate"
                'CMD.Parameters.AddWithValue("?ObjectName", sControlName)
                'CMD.Parameters.AddWithValue("?pstate", sState)
                Try
                    '    CN.Open()
                    '    sStateMatch = CMD.ExecuteScalar
                    '    CN.Close()
                    '    If sStateMatch <> "" Then
                    '        sStateMatch = sStateMatch.Substring(0, 7)
                    '    End If
                    OSAEApi.ObjectPropertySet(sControlName, "X", sender.Location.X)
                    OSAEApi.ObjectPropertySet(sControlName, "Y", sender.Location.Y)
                Catch myerror As MySqlException
                    MessageBox.Show("Error ControlStateImageUp: " & myerror.Message)
                    CN.Close()
                End Try
                dragging = False
                Windows.Forms.Cursor.Clip = Nothing
                sender.Invalidate()
            End If
            Me.Cursor = Cursors.Arrow
        End If
    End Sub
#End Region

#Region "StaticLabel"
    Public Sub ControlStaticLabelClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then
            'Prosedure to move an image from the workspace. Using Mousebutton right
            If e.Button = Windows.Forms.MouseButtons.Right Then
                Dim Response As MsgBoxResult = MsgBox("Do you want to remove this object", MsgBoxStyle.YesNo, "Id = " & sender.tag)
                If Response = MsgBoxResult.Yes Then   ' User chose Yes.
                    'Remove from workspace
                    Me.Controls.Remove(sender)
                    'Remove from database
                    OSAEApi.ObjectDelete(aScreenObject(sender.tag).Control_Name)
                End If
            End If

            'Procedure to select the image
            If e.Button = Windows.Forms.MouseButtons.Left Then
                Me.Cursor = Cursors.Hand
                dragging = True
                mousex = -e.X
                mousey = -e.Y
                Dim clipleft As Integer = Me.PointToClient(MousePosition).X - sender.Location.X()
                Dim cliptop As Integer = Me.PointToClient(MousePosition).Y - sender.Location.Y()
                Dim clipwidth As Integer = Me.ClientSize.Width - (sender.Width - clipleft)
                Dim clipheight As Integer = Me.ClientSize.Height - (sender.Height - cliptop)
                Windows.Forms.Cursor.Clip = Me.RectangleToScreen(New Rectangle(clipleft, cliptop, clipwidth, clipheight))
                sender.Invalidate()
            End If
        End If
    End Sub
    Public Sub ControlStaticLabelMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then
            If dragging Then
                'move control to new position
                Dim MPosition As New Point()
                MPosition = Me.PointToClient(MousePosition)
                MPosition.Offset(mousex, mousey)
                'ensure control cannot leave container
                sender.Location = MPosition
            End If
        End If
    End Sub
    Private Sub ControlStaticLabelUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim sControlName As String = ""
        Dim CMD As New MySqlCommand
        If mnuEditMode.Checked = True Then
            If dragging Then
                sControlName = aScreenObject(sender.tag).Control_Name
                OSAEApi.ObjectPropertySet(sControlName, "X", sender.Location.X)
                OSAEApi.ObjectPropertySet(sControlName, "Y", sender.Location.Y)
                dragging = False
                Windows.Forms.Cursor.Clip = Nothing
                sender.Invalidate()
            End If
            Me.Cursor = Cursors.Arrow
        End If
    End Sub
#End Region

#Region "TimerLabel"
    Public Sub ControlTimerLabelClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then
            'Prosedure to move an image from the workspace. Using Mousebutton right
            If e.Button = Windows.Forms.MouseButtons.Right Then
                Dim Response As MsgBoxResult = MsgBox("Do you want to remove this object", MsgBoxStyle.YesNo, "Id = " & sender.tag)
                If Response = MsgBoxResult.Yes Then   ' User chose Yes.
                    'Remove from workspace
                    Me.Controls.Remove(sender)
                    'Remove from database
                    OSAEApi.ObjectDelete(aScreenObject(sender.tag).Control_Name)
                End If
            End If

            'Procedure to select the image
            If e.Button = Windows.Forms.MouseButtons.Left Then
                Me.Cursor = Cursors.Hand
                dragging = True
                mousex = -e.X
                mousey = -e.Y
                Dim clipleft As Integer = Me.PointToClient(MousePosition).X - sender.Location.X()
                Dim cliptop As Integer = Me.PointToClient(MousePosition).Y - sender.Location.Y()
                Dim clipwidth As Integer = Me.ClientSize.Width - (sender.Width - clipleft)
                Dim clipheight As Integer = Me.ClientSize.Height - (sender.Height - cliptop)
                Windows.Forms.Cursor.Clip = Me.RectangleToScreen(New Rectangle(clipleft, cliptop, clipwidth, clipheight))
                sender.Invalidate()
            End If
        End If
    End Sub
    Public Sub ControlTimerLabelMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then
            If dragging Then
                'move control to new position
                Dim MPosition As New Point()
                MPosition = Me.PointToClient(MousePosition)
                MPosition.Offset(mousex, mousey)
                'ensure control cannot leave container
                sender.Location = MPosition
            End If
        End If
    End Sub
    Private Sub ControlTimerLabelUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim sControlName As String = ""
        Dim CMD As New MySqlCommand
        If mnuEditMode.Checked = True Then
            If dragging Then
                sControlName = aScreenObject(sender.tag).Control_Name
                OSAEApi.ObjectPropertySet(sControlName, "X", sender.Location.X)
                OSAEApi.ObjectPropertySet(sControlName, "Y", sender.Location.Y)
                dragging = False
                Windows.Forms.Cursor.Clip = Nothing
                sender.Invalidate()
            End If
            Me.Cursor = Cursors.Arrow
        End If
    End Sub
#End Region

#Region "Nav Image"
    Private Sub ClickControlNavImage(ByVal sender As Object, ByVal e As System.EventArgs)
        If mnuEditMode.Checked = False Then
            Dim outputStatus As String
            outputStatus = DirectCast(sender, PictureBox).Tag
            Load_Screen(aScreenObject(outputStatus).Object_Name)
        End If
    End Sub
    Public Sub ControlNavImageClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then
            'Prosedure to move an image from the workspace. Using Mousebutton right
            If e.Button = Windows.Forms.MouseButtons.Right Then
                Dim Response As MsgBoxResult = MsgBox("Do you want to remove this object", MsgBoxStyle.YesNo, "Id = " & sender.tag)
                If Response = MsgBoxResult.Yes Then   ' User chose Yes.
                    'Remove from workspace
                    Me.Controls.Remove(sender)
                    'Remove from database
                    OSAEApi.ObjectDelete(aScreenObject(sender.tag).Control_Name)
                End If
            End If

            'Procedure to select the image
            If e.Button = Windows.Forms.MouseButtons.Left Then
                Me.Cursor = Cursors.Hand
                dragging = True
                mousex = -e.X
                mousey = -e.Y
                Dim clipleft As Integer = Me.PointToClient(MousePosition).X - sender.Location.X()
                Dim cliptop As Integer = Me.PointToClient(MousePosition).Y - sender.Location.Y()
                Dim clipwidth As Integer = Me.ClientSize.Width - (sender.Width - clipleft)
                Dim clipheight As Integer = Me.ClientSize.Height - (sender.Height - cliptop)
                Windows.Forms.Cursor.Clip = Me.RectangleToScreen(New Rectangle(clipleft, cliptop, clipwidth, clipheight))
                sender.Invalidate()
            End If
        End If
    End Sub
    Public Sub ControlNavImageMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mnuEditMode.Checked = True Then
            If dragging Then
                'move control to new position
                Dim MPosition As New Point()
                MPosition = Me.PointToClient(MousePosition)
                MPosition.Offset(mousex, mousey)
                'ensure control cannot leave container
                sender.Location = MPosition
            End If
        End If
    End Sub
    Private Sub ControlNavImageUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim sControlName As String = ""
        Dim CMD As New MySqlCommand
        If mnuEditMode.Checked = True Then
            If dragging Then
                ' sControlName = aScreenObject(aControlStateImage(sender.tag).Text).Control_Name
                sControlName = aScreenObject(sender.tag).Control_Name
                OSAEApi.ObjectPropertySet(sControlName, "X", sender.Location.X)
                OSAEApi.ObjectPropertySet(sControlName, "Y", sender.Location.Y)
                dragging = False
                Windows.Forms.Cursor.Clip = Nothing
                sender.Invalidate()
            End If
            Me.Cursor = Cursors.Arrow
        End If
    End Sub
#End Region

    Private Sub Load_Objects(ByVal screen As String)
        Timer1.Enabled = False
        Controls_Clear()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        Dim iLoop As Integer, sState As String, sStateMatch As String, sImage As String
        Dim iX As Integer, iY As Integer, iZOrder As Integer, sForeColor As String
        Dim sPropertyValue As String, sPropertyName As String, sBackColor As String
        Dim sFontName As String, iFontSize As String
        Dim sPrefix As String = "", sSuffix As String = ""
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text

        'COUNT **ALL** control objects for this screen
        CMD.CommandText = "SELECT COUNT(*) as Results FROM osae_v_screen_object WHERE screen_name=?pscreen"
        CMD.Parameters.AddWithValue("?pscreen", screen)
        Try
            CN.Open()
            iObjectCount = CMD.ExecuteScalar
            CN.Close()
            ReDim aScreenObject(iObjectCount)
        Catch myerror As MySqlException
            MessageBox.Show("GUI Error Load_Objects 1: " & myerror.Message)
            CN.Close()
        End Try
        CMD.Parameters.Clear()
        'Select **ALL** control objects for this screen
        CMD.CommandText = "SELECT * FROM osae_v_object_property WHERE object_id IN(SELECT control_id FROM osae_v_screen_object WHERE screen_name=?pscreen) AND property_name='ZOrder' ORDER BY property_value"
        CMD.Parameters.AddWithValue("?pscreen", screen)
        Try
            iObjectCount = 0
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                iObjectCount = iObjectCount + 1
                aScreenObject(iObjectCount).Control_Name = Convert.ToString(myReader.Item("object_name"))
                aScreenObject(iObjectCount).Control_Type = Convert.ToString(myReader.Item("object_type"))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("GUI Error Load_Objects 2: " & myerror.Message)
            CN.Close()
        End Try
        CMD.Parameters.Clear()
        For iLoop = 1 To iObjectCount
            If aScreenObject(iLoop).Control_Type = "CONTROL STATE IMAGE" Then
                aScreenObject(iLoop).Object_Name = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Object Name").Value
                iStateImageCount = iStateImageCount + 1
                iStateImageList = iStateImageList & "'" & aScreenObject(iLoop).Object_Name & "',"
                aScreenObject(iLoop).Control_Index = iStateImageCount
                aControlStateImage(iStateImageCount).Tag = iLoop
                sState = OSAEApi.GetObjectStateValue(aScreenObject(iLoop).Object_Name).Value
                CMD.Parameters.Clear()
                CMD.CommandText = "SELECT COALESCE(last_state_change,NOW()) FROM osae_v_object WHERE object_name=?ObjectName"
                CMD.Parameters.AddWithValue("?ObjectName", aScreenObject(iLoop).Object_Name)
                Try
                    CN.Open()
                    aScreenObject(iLoop).Object_State_Time = CMD.ExecuteScalar
                    CN.Close()
                Catch myerror As MySqlException
                    MessageBox.Show("GUI Error Load_Objects 2.5: " & myerror.Message)
                    CN.Close()
                End Try
                CMD.Parameters.Clear()
                CMD.CommandText = "SELECT property_name FROM osae_v_object_property WHERE object_name=?ObjectName AND property_value=?pstate"
                CMD.Parameters.AddWithValue("?ObjectName", aScreenObject(iLoop).Control_Name)
                CMD.Parameters.AddWithValue("?pstate", sState)
                Try
                    CN.Open()
                    sStateMatch = CMD.ExecuteScalar
                    CN.Close()
                    If sStateMatch <> "" Then
                        sStateMatch = sStateMatch.Substring(0, 7)
                    End If
                    sImage = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, sStateMatch & " Image").Value
                    'sImage = sImage.Replace(".\", "\")
                    If File.Exists(gAppPath & sImage) Then sImage = gAppPath & sImage
                    iZOrder = Val(OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "ZOrder").Value)
                    iX = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, sStateMatch & " X").Value)
                    iY = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, sStateMatch & " Y").Value)
                    aScreenObject(iLoop).Object_State = sState
                    If File.Exists(sImage) Then
                        Dim tImage As Image = Image.FromFile(sImage)
                        aControlStateImage(aScreenObject(iLoop).Control_Index).Image = tImage
                        aControlStateImage(aScreenObject(iLoop).Control_Index).Left = iX * gXFullRatio
                        aControlStateImage(aScreenObject(iLoop).Control_Index).Top = iY * gYFullRatio
                        aControlStateImage(aScreenObject(iLoop).Control_Index).Height = tImage.Height * gYFullRatio
                        aControlStateImage(aScreenObject(iLoop).Control_Index).Width = tImage.Width * gXFullRatio
                        If iZOrder = 0 Then
                            aControlStateImage(aScreenObject(iLoop).Control_Index).SendToBack()
                        Else
                            aControlStateImage(aScreenObject(iLoop).Control_Index).BringToFront()
                        End If
                        aControlStateImage(aScreenObject(iLoop).Control_Index).Visible = True
                    Else
                        aControlStateImage(aScreenObject(iLoop).Control_Index).Image = Nothing
                        aControlStateImage(aScreenObject(iLoop).Control_Index).Visible = False
                    End If
                Catch myerror As MySqlException
                    MessageBox.Show("GUI Error Load_Objects 3: " & myerror.Message)
                    CN.Close()
                End Try
            ElseIf aScreenObject(iLoop).Control_Type = "CONTROL PROPERTY LABEL" Then
                iPropertyLabelCount = iPropertyLabelCount + 1
                aScreenObject(iLoop).Object_Name = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Object Name").Value
                aScreenObject(iLoop).Control_Index = iPropertyLabelCount
                sPropertyName = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Property Name").Value
                aScreenObject(iLoop).Property_Name = sPropertyName
                sPropertyValue = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Object_Name, sPropertyName).Value
                sBackColor = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Back Color").Value
                sForeColor = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Fore Color").Value
                sPrefix = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Prefix").Value
                sSuffix = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Suffix").Value
                iFontSize = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Font Size").Value
                sFontName = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Font Name").Value
                aControlPropertyLabel(iPropertyLabelCount).Tag = iLoop
                aControlPropertyLabel(iPropertyLabelCount).ReadOnly = True
                aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Font = New Font(sFontName, iFontSize, FontStyle.Regular, GraphicsUnit.Point)
                g_toolTip.SetToolTip(aControlPropertyLabel(iPropertyLabelCount), aScreenObject(iLoop).Object_Name & " " & sPropertyName & " = " & sPropertyValue)
                iX = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "X").Value)
                iY = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Y").Value)
                If sPropertyValue <> "" Then
                    If sBackColor <> "" Then
                        Try
                            aControlPropertyLabel(aScreenObject(iLoop).Control_Index).BackColor = Color.FromName(sBackColor)
                        Catch ex As Exception
                        End Try
                    End If
                    If sForeColor <> "" Then
                        Try
                            aControlPropertyLabel(aScreenObject(iLoop).Control_Index).ForeColor = Color.FromName(sForeColor)
                        Catch ex As Exception
                        End Try
                    End If
                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Text = sPrefix & sPropertyValue & sSuffix
                    TextBox1.Text = aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Text
                    Dim g As Graphics = TextBox1.CreateGraphics
                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Width = g.MeasureString(aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Text, aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Font).Width + 10

                    aScreenObject(iLoop).Object_State = ""
                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Left = iX * gXFullRatio
                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Top = iY * gYFullRatio
                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).BringToFront()
                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Visible = True
                Else
                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Text = ""
                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Visible = False
                End If
            ElseIf aScreenObject(iLoop).Control_Type = "CONTROL STATIC LABEL" Then
                iStaticLabelCount = iStaticLabelCount + 1
                aScreenObject(iLoop).Object_Name = ""
                aScreenObject(iLoop).Control_Index = iStaticLabelCount
                sPropertyValue = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Object_Name, "Value").Value
                sBackColor = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Back Color").Value
                sForeColor = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Fore Color").Value
                aControlStaticLabel(iStaticLabelCount).Tag = iLoop
                g_toolTip.SetToolTip(aControlStaticLabel(iStaticLabelCount), "")
                iX = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "X").Value)
                iY = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Y").Value)
                If sPropertyValue <> "" Then
                    If sBackColor <> "" Then
                        Try
                            aControlPropertyLabel(aScreenObject(iLoop).Control_Index).BackColor = Color.FromName(sBackColor)
                        Catch ex As Exception
                        End Try
                    End If
                    If sForeColor <> "" Then
                        Try
                            aControlPropertyLabel(aScreenObject(iLoop).Control_Index).ForeColor = Color.FromName(sForeColor)
                        Catch ex As Exception
                        End Try
                    End If
                    aControlStaticLabel(aScreenObject(iLoop).Control_Index).Text = sPropertyValue
                    aControlStaticLabel(aScreenObject(iLoop).Control_Index).Width = sPropertyValue.Length * 7
                    aControlStaticLabel(aScreenObject(iLoop).Control_Index).Left = iX * gXFullRatio
                    aControlStaticLabel(aScreenObject(iLoop).Control_Index).Top = iY * gYFullRatio
                    aControlStaticLabel(aScreenObject(iLoop).Control_Index).BringToFront()
                    aControlStaticLabel(aScreenObject(iLoop).Control_Index).Visible = True
                Else
                    aControlStaticLabel(aScreenObject(iLoop).Control_Index).Text = ""
                    aControlStaticLabel(aScreenObject(iLoop).Control_Index).Visible = False
                End If
            ElseIf aScreenObject(iLoop).Control_Type = "CONTROL TIMER LABEL" Then
                iTimerLabelCount = iTimerLabelCount + 1
                aScreenObject(iLoop).Control_Index = iTimerLabelCount
                aScreenObject(iLoop).Object_Name = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Object Name").Value
                sPropertyName = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Type").Value
                aScreenObject(iLoop).Property_Name = sPropertyName
                sPropertyValue = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Object_Name, "OFF Timer").Value
                aScreenObject(iLoop).Property_Value = sPropertyValue
                sBackColor = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Back Color").Value
                sForeColor = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Fore Color").Value
                aControlTimerLabel(iTimerLabelCount).Tag = iLoop
                sState = OSAEApi.GetObjectStateValue(aScreenObject(iLoop).Object_Name).Value
                aScreenObject(iLoop).Object_State = sState
                CMD.Parameters.Clear()

                CMD.CommandText = "SELECT COALESCE(last_updated,NOW()) FROM osae_v_object WHERE object_name=?ObjectName"
                CMD.Parameters.AddWithValue("?ObjectName", aScreenObject(iLoop).Object_Name)
                Try
                    CN.Open()
                    aScreenObject(iLoop).Object_Last_Updated = CMD.ExecuteScalar
                    CN.Close()
                Catch myerror As MySqlException
                    MessageBox.Show("GUI Error Load_Objects 2.7: " & myerror.Message)
                    CN.Close()
                End Try
                CMD.Parameters.Clear()
                CMD.CommandText = "SELECT COALESCE(last_state_change,NOW()) FROM osae_v_object WHERE object_name=?ObjectName"
                CMD.Parameters.AddWithValue("?ObjectName", aScreenObject(iLoop).Object_Name)
                Try
                    CN.Open()
                    aScreenObject(iLoop).Object_State_Time = CMD.ExecuteScalar
                    CN.Close()
                Catch myerror As MySqlException
                    MessageBox.Show("GUI Error Load_Objects 666: " & myerror.Message)
                    CN.Close()
                End Try
                iX = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "X").Value)
                iY = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Y").Value)
                If sBackColor <> "" Then
                    Try
                        aControlTimerLabel(aScreenObject(iLoop).Control_Index).BackColor = Color.FromName(sBackColor)
                    Catch ex As Exception
                    End Try
                End If
                If sForeColor <> "" Then
                    Try
                        aControlTimerLabel(aScreenObject(iLoop).Control_Index).ForeColor = Color.FromName(sForeColor)
                    Catch ex As Exception
                    End Try
                End If
                aControlTimerLabel(aScreenObject(iLoop).Control_Index).Text = sPropertyValue
                aControlTimerLabel(aScreenObject(iLoop).Control_Index).Width = sPropertyValue.Length * 7
                aControlTimerLabel(aScreenObject(iLoop).Control_Index).Left = iX * gXFullRatio
                aControlTimerLabel(aScreenObject(iLoop).Control_Index).Top = iY * gYFullRatio
                aControlTimerLabel(aScreenObject(iLoop).Control_Index).BringToFront()
                aControlTimerLabel(aScreenObject(iLoop).Control_Index).Visible = True
            ElseIf aScreenObject(iLoop).Control_Type = "CONTROL METHOD IMAGE" Then
                iMethodImageCount = iMethodImageCount + 1
                aScreenObject(iLoop).Object_Name = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Object Name").Value

                aScreenObject(iLoop).Control_Index = iMethodImageCount
                aControlMethodImage(aScreenObject(iLoop).Control_Index).Tag = iLoop
                g_toolTip.SetToolTip(aControlMethodImage(iMethodImageCount), aScreenObject(iLoop).Object_Name)
                CMD.Parameters.Clear()
                Try
                    sImage = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Image").Value
                    sImage = sImage.Replace(".\", "\")
                    If File.Exists(gAppPath & sImage) Then sImage = gAppPath & sImage
                    iZOrder = Val(OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "ZOrder").Value)
                    iX = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "X").Value)
                    iY = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Y").Value)
                    If File.Exists(sImage) Then
                        aControlMethodImage(aScreenObject(iLoop).Control_Index).Image = Image.FromFile(sImage)
                        aScreenObject(iLoop).Object_State = ""
                        aControlMethodImage(aScreenObject(iLoop).Control_Index).Left = iX * gXFullRatio
                        aControlMethodImage(aScreenObject(iLoop).Control_Index).Top = iY * gYFullRatio
                        aControlMethodImage(aScreenObject(iLoop).Control_Index).Visible = True
                    Else
                        aControlMethodImage(aScreenObject(iLoop).Control_Index).Image = Nothing
                        aControlMethodImage(aScreenObject(iLoop).Control_Index).Visible = False
                    End If
                Catch myerror As MySqlException
                    MessageBox.Show("GUI Error Load_Objects 4: " & myerror.Message)
                    CN.Close()
                End Try
            ElseIf aScreenObject(iLoop).Control_Type = "CONTROL NAVIGATION IMAGE" Then
                iNavImageCount = iNavImageCount + 1
                aScreenObject(iLoop).Object_Name = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Screen").Value
                aScreenObject(iLoop).Control_Index = iNavImageCount
                aScreenObject(iLoop).Object_State = ""
                aControlNavImage(iNavImageCount).Tag = iLoop
                g_toolTip.SetToolTip(aControlNavImage(iNavImageCount), aScreenObject(iLoop).Object_Name)
                CMD.Parameters.Clear()
                Try
                    sImage = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Image").Value
                    sImage = sImage.Replace(".\", "\")
                    If File.Exists(gAppPath & sImage) Then sImage = gAppPath & sImage
                    iX = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "X").Value)
                    iY = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Y").Value)
                    If File.Exists(sImage) Then
                        Dim tImage As Image = Image.FromFile(sImage)
                        aControlNavImage(aScreenObject(iLoop).Control_Index).Image = Image.FromFile(sImage)
                        aControlNavImage(aScreenObject(iLoop).Control_Index).Top = iY * gYFullRatio
                        aControlNavImage(aScreenObject(iLoop).Control_Index).Left = iX * gXFullRatio
                        aControlNavImage(aScreenObject(iLoop).Control_Index).Height = tImage.Height * gYFullRatio
                        aControlNavImage(aScreenObject(iLoop).Control_Index).Width = tImage.Width * gXFullRatio
                        aControlNavImage(aScreenObject(iLoop).Control_Index).Visible = True
                    Else
                        aControlNavImage(aScreenObject(iLoop).Control_Index).Visible = False
                        aControlNavImage(aScreenObject(iLoop).Control_Index).Image = Nothing
                        aControlNavImage(aScreenObject(iLoop).Control_Index).Left = 0
                        aControlNavImage(aScreenObject(iLoop).Control_Index).Top = 0

                    End If
                Catch myerror As MySqlException
                    MessageBox.Show("GUI Error Load_Objects 5: " & myerror.Message)
                    CN.Close()
                End Try
            ElseIf aScreenObject(iLoop).Control_Type = "USER CONTROL" Then
                iUserControlCount += 1
                Dim sUCType As String = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Control Type").Value
                If sUCType = "USER CONTROL WEATHER" Then
                    Dim tempControl As New ucWeather
                    aScreenObject(iLoop).Control_Index = iUserControlCount
                    If aControlUserControl(aScreenObject(iLoop).Control_Index).Controls.Count = 0 Then
                        aControlUserControl(aScreenObject(iLoop).Control_Index).Controls.Add(tempControl)
                    End If
                    aControlUserControl(aScreenObject(iLoop).Control_Index).Top = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Y").Value
                    aControlUserControl(aScreenObject(iLoop).Control_Index).Left = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "X").Value
                    aControlUserControl(aScreenObject(iLoop).Control_Index).Width = tempControl.Width
                    aControlUserControl(aScreenObject(iLoop).Control_Index).Height = tempControl.Height
                    Application.DoEvents()
                    aControlUserControl(aScreenObject(iLoop).Control_Index).Visible = True

                End If

            End If
        Next iLoop
        If iStateImageList.EndsWith(",") Then iStateImageList = iStateImageList.Substring(0, iStateImageList.Length - 1)
        Timer1.Enabled = True
    End Sub

    Private Sub Update_Objects()
        Update_Tool_Tips()
        Dim CMD As New MySqlCommand
        Dim CMD2 As New MySqlCommand
        Dim myReader As MySqlDataReader
        Dim sStateMatch As String, sImage As String, iZOrder As Integer
        Dim sPropertyValue As String, sPropertyName As String
        Dim sPrefix As String = "", sSuffix As String = ""
        Dim sFontName As String, iFontSize As String
        'Dim iControlID As Integer
        Dim iLoop As Integer
        Dim iX As Integer, iY As Integer ', sForeColor As String
        Dim sState As String = ""
        'Dim iSecondsSinceRefresh As Double
        If gCurrentScreen = "" Then Exit Sub
        'Try
        'iSecondsSinceRefresh = DateDiff(DateInterval.Second, gLastRefresh, Now()).ToString("HH:mm:ss")
        ' Catch
        'condsSinceRefresh = 0
        ' End Try
        'Dim dtStateTime As New DateTime(1, 1, 1, 0, 0, iSecondsSinceRefresh)
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD2.Connection = CN2
        CMD2.CommandType = CommandType.Text
        CMD2.CommandText = "SELECT * FROM osae_v_screen_object WHERE screen_name=?ScreenName AND last_updated > subtime(now(),'00:00:05')"
        CMD2.Parameters.AddWithValue("?ScreenName", gCurrentScreen)
        Try
            CN2.Open()
            myReader = CMD2.ExecuteReader
            While myReader.Read
                'iControlID = Convert.ToString(myReader.Item("control_id"))
                For iLoop = 0 To aScreenObject.Length - 1
                    If aScreenObject(iLoop).Object_Name = Convert.ToString(myReader.Item("object_name")) Then
                        If aScreenObject(iLoop).Control_Type = "CONTROL STATE IMAGE" Then
                            '            ' Check to see if the State has change for these controls
                            sState = Convert.ToString(myReader.Item("state_name"))
                            If Not IsDBNull(myReader.Item("last_state_change")) Then
                                aScreenObject(iLoop).Object_State_Time = Convert.ToDateTime(myReader.Item("last_state_change"))
                            Else
                                aScreenObject(iLoop).Object_State_Time = Nothing
                            End If
                            '            sPropertyBlock = Read_Properties(aScreenObject(iLoop).Object_Name)
                            ' g_toolTip.SetToolTip(aControlStateImage(aScreenObject(iLoop).Control_Index), aScreenObject(iLoop).Object_Name & " " & sState)

                            'If sState <> aScreenObject(iLoop).Object_State Then
                            aScreenObject(iLoop).Object_State = sState
                            CMD.Parameters.Clear()
                            CMD.CommandText = "SELECT property_name FROM osae_v_object_property WHERE object_name=?ObjectName AND property_value=?pstate"
                            CMD.Parameters.AddWithValue("?ObjectName", aScreenObject(iLoop).Control_Name)
                            CMD.Parameters.AddWithValue("?pstate", sState)
                            Try
                                CN.Open()
                                sStateMatch = CMD.ExecuteScalar
                                CN.Close()
                                If sStateMatch <> "" Then
                                    sStateMatch = sStateMatch.Substring(0, 7)
                                End If
                                sImage = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, sStateMatch & " Image").Value
                                'sImage = sImage.Replace(".\", "\")
                                If File.Exists(gAppPath & sImage) Then sImage = gAppPath & sImage
                                iZOrder = Val(OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "ZOrder").Value)
                                iX = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, sStateMatch & " X").Value)
                                iY = Val("" & OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, sStateMatch & " Y").Value)

                                If File.Exists(sImage) Then
                                    Dim tImage As Image = Image.FromFile(sImage)
                                    aControlStateImage(aScreenObject(iLoop).Control_Index).Image = Image.FromFile(sImage)
                                    'aControlStateImage(aScreenObject(iLoop).Control_Index).Text = sState
                                    aControlStateImage(aScreenObject(iLoop).Control_Index).Left = iX * gXFullRatio
                                    aControlStateImage(aScreenObject(iLoop).Control_Index).Top = iY * gYFullRatio
                                    aControlStateImage(aScreenObject(iLoop).Control_Index).Height = tImage.Height * gYFullRatio
                                    aControlStateImage(aScreenObject(iLoop).Control_Index).Width = tImage.Width * gXFullRatio
                                    If iZOrder = 0 Then
                                        aControlStateImage(aScreenObject(iLoop).Control_Index).SendToBack()
                                    Else
                                        aControlStateImage(aScreenObject(iLoop).Control_Index).BringToFront()
                                    End If
                                    aControlStateImage(aScreenObject(iLoop).Control_Index).Visible = True
                                Else
                                    aControlStateImage(aScreenObject(iLoop).Control_Index).Visible = False
                                    aControlStateImage(aScreenObject(iLoop).Control_Index).Image = Nothing
                                    aControlStateImage(aScreenObject(iLoop).Control_Index).Left = 0
                                    aControlStateImage(aScreenObject(iLoop).Control_Index).Top = 0
                                End If
                            Catch myerror As MySqlException
                                MessageBox.Show("GUI Error Update_Objects 1: " & myerror.Message)
                                CN.Close()
                            End Try
                            'End If
                        ElseIf aScreenObject(iLoop).Control_Type = "CONTROL PROPERTY LABEL" Then
                            If aScreenObject(iLoop).Property_Name = Convert.ToString(myReader.Item("property_name")) Then
                                sPropertyName = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Property Name").Value
                                sPropertyValue = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Object_Name, sPropertyName).Value
                                sPrefix = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Prefix").Value
                                sSuffix = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Suffix").Value
                                'g_toolTip.SetToolTip(aControlPropertyLabel(iPropertyLabelCount), aScreenObject(iLoop).Object_Name & " " & sPropertyName & " = " & sPropertyValue)
                                If sPropertyValue <> "" Then
                                    iFontSize = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Font Size").Value
                                    sFontName = OSAEApi.GetObjectPropertyValue(aScreenObject(iLoop).Control_Name, "Font Name").Value
                                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Text = sPrefix & sPropertyValue & sSuffix
                                    TextBox1.Text = aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Text
                                    Dim g As Graphics = TextBox1.CreateGraphics
                                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Width = g.MeasureString(aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Text, aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Font).Width + 10

                                    aScreenObject(iLoop).Object_State = ""

                                Else
                                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Text = ""
                                    aControlPropertyLabel(aScreenObject(iLoop).Control_Index).Visible = False
                                End If

                            End If
                        ElseIf aScreenObject(iLoop).Control_Type = "CONTROL TIMER LABEL" Then
                            sState = OSAEApi.GetObjectStateValue(aScreenObject(iLoop).Object_Name).Value
                            aScreenObject(iLoop).Object_State = sState
                            CMD.Parameters.Clear()

                            CMD.CommandText = "SELECT COALESCE(last_updated,NOW()) FROM osae_v_object WHERE object_name=?ObjectName"
                            CMD.Parameters.AddWithValue("?ObjectName", aScreenObject(iLoop).Object_Name)
                            Try
                                CN.Open()
                                aScreenObject(iLoop).Object_Last_Updated = CMD.ExecuteScalar
                                CN.Close()
                            Catch myerror As MySqlException
                                MessageBox.Show("GUI Error Load_Objects 666: " & myerror.Message)
                                CN.Close()
                            End Try
                            CMD.Parameters.Clear()

                            CMD.CommandText = "SELECT COALESCE(last_state_change,NOW()) FROM osae_v_object WHERE object_name=?ObjectName"
                            CMD.Parameters.AddWithValue("?ObjectName", aScreenObject(iLoop).Object_Name)
                            Try
                                CN.Open()
                                aScreenObject(iLoop).Object_State_Time = CMD.ExecuteScalar
                                CN.Close()
                            Catch myerror As MySqlException
                                MessageBox.Show("GUI Error Load_Objects 666: " & myerror.Message)
                                CN.Close()
                            End Try
                        End If
                    End If

                Next
            End While
            CN2.Close()
        Catch myerror As MySqlException
            MessageBox.Show("GUI Error Update_Objects 2: " & myerror.Message)
            CN2.Close()
        End Try
        gLastRefresh = Now()
    End Sub

    Private Sub Load_User_Controls(ByVal screen As String)
        'Dim CMD As New MySqlCommand
        'Dim myReader As MySqlDataReader
        'CMD.Connection = CN
        'CMD.CommandType = CommandType.Text
        'CMD.CommandText = "SELECT * FROM osae_v_screen_object WHERE screen_name=?pscreen AND control_base_type='USER CONTROL'"
        'CMD.Parameters.AddWithValue("?pscreen", screen)
        'Try
        '    iObjectCount = 0
        '    CN.Open()
        '    myReader = CMD.ExecuteReader
        '    While myReader.Read
        '        iObjectCount = iObjectCount + 1
        '        aScreenObject(iObjectCount).Control_Name = Convert.ToString(myReader.Item("object_name"))
        '        aScreenObject(iObjectCount).Control_Type = Convert.ToString(myReader.Item("control_base_type"))
        '    End While
        '    CN.Close()
        'Catch myerror As MySqlException
        '    MessageBox.Show("GUI Error Load_Objects 2: " & myerror.Message)
        '    CN.Close()
        'End Try
    End Sub

#Region "Menu Items"

    Private Sub mnuStateImage_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuStateImage.Click
        frmAddImageState.Show()
    End Sub

    Private Sub mnuPropertyLabel_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPropertyLabel.Click
        frmAddPropertyLabel.Show()
    End Sub

    Private Sub mnuMethodImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuMethodImage.Click
        frmAddMethodImage.Show()
    End Sub

    Private Sub mnuNavigationImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuNavigationImage.Click
        frmAddNavImage.Show()
    End Sub

    Private Sub mnuCreateScreen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCreateScreen.Click
        frmAddScreen.Show()
    End Sub

    Private Sub mnuCreateObject_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCreateObject.Click
        frmAddObject.Show()
    End Sub

    Private Sub mnuEditMode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEditMode.Click
        If mnuEditMode.Checked Then
            mnuEditMode.Checked = False
        Else
            mnuEditMode.Checked = True
        End If
    End Sub

    Private Sub mnuChangeScreen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuChangeScreen.Click
        Dim CMD As New MySqlCommand
        Dim iCounter As Integer
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT COUNT(object_ID) FROM osae_v_object_property WHERE base_type='SCREEN' AND property_name='Background Image'"
        Try
            'cboScreens.Items.Clear()
            CN.Open()
            iCounter = CMD.ExecuteScalar
            CN.Close()
            If iCounter = 0 Then
                MessageBox.Show("You have no Screens setup.  Please Add a Screen.")
            Else
                frmChangeScreen.Show()
            End If
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Dropdowns: " & myerror.Message)
            CN.Close()
        End Try
    End Sub

    Private Sub mnuViewLogs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewLogs.Click
        frmLogs.Show()
    End Sub

    Private Sub UserControlToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuUserControl.Click
        frmAddUserControl.Show()
    End Sub

    Private Sub mnuScriptEditor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuScriptEditor.Click
        frmScriptEditor.Show()
    End Sub

    Private Sub mnuSchedules_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSchedules.Click
        frmScheduling.Show()
    End Sub

    Private Sub mnuPatternEditor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPatternEditor.Click
        frmScripts.Show()
    End Sub

    Private Sub mnuObjectTypes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuObjectTypes.Click
        frmObjectTypes.Show()
    End Sub

    Private Sub mnuObjects_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuObjects.Click
        frmObjects.Show()
    End Sub
    Private Sub mnuScreensTool_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuScreensTool.Click
        frmScreens.Show()
    End Sub

    Private Sub mnuTimerLabel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuTimerLabel.Click
        frmTimerLabels.Show()
    End Sub

#End Region

    Public Function Read_Properties(ByVal sObject As String) As String
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader, sBlock As String = ""
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT property_name, property_value FROM osae_v_object_property WHERE object_name=?ObjectName"
        CMD.Parameters.AddWithValue("?ObjectName", sObject)
        Try
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                If Convert.ToString(myReader.Item("property_value")) <> "" Then
                    sBlock = sBlock + Convert.ToString(myReader.Item("property_name")) & " = " & Convert.ToString(myReader.Item("property_value")) & vbCrLf
                End If
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Read_Properties: " & myerror.Message, "GUI")
            CN.Close()
        End Try
        Read_Properties = sBlock
    End Function

    Private Sub Update_Tool_Tips()
        Dim dtTime As String, dSeconds As Double, sType As String = "", dtTarget As DateTime
        Try
            For iLoop = 0 To aScreenObject.Length - 1
                'If aScreenObject(iLoop).Object_State_Time <> Nothing Then
                If aScreenObject(iLoop).Control_Type = "CONTROL STATE IMAGE" Then
                    dSeconds = DateDiff(DateInterval.Second, aScreenObject(iLoop).Object_State_Time, Now())
                    '            ' Check to see if the State has change for these controls
                    dtTime = Format(DateAdd("s", dSeconds, "00:00:00"), "HH:mm:ss")

                    '            sPropertyBlock = Read_Properties(aScreenObject(iLoop).Object_Name)
                    g_toolTip.SetToolTip(aControlStateImage(aScreenObject(iLoop).Control_Index), aScreenObject(iLoop).Object_Name & " " & aScreenObject(iLoop).Object_State & " (For: " & dtTime & ")")
                ElseIf aScreenObject(iLoop).Control_Type = "CONTROL TIMER LABEL" Then
                    ' sType = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Type")
                    sType = aScreenObject(iLoop).Property_Name
                    If sType = "State" Then
                        dSeconds = DateDiff(DateInterval.Second, aScreenObject(iLoop).Object_State_Time, Now())
                        '            ' Check to see if the State has change for these controls
                        dtTime = Format(DateAdd("s", dSeconds, "00:00:00"), "HH:mm:ss")

                        '            sPropertyBlock = Read_Properties(aScreenObject(iLoop).Object_Name)
                        aControlTimerLabel(aScreenObject(iLoop).Control_Index).Text = dtTime
                        Dim g As Graphics = aControlTimerLabel(aScreenObject(iLoop).Control_Index).CreateGraphics
                        aControlTimerLabel(aScreenObject(iLoop).Control_Index).Width = g.MeasureString(aControlTimerLabel(aScreenObject(iLoop).Control_Index).Text, aControlTimerLabel(aScreenObject(iLoop).Control_Index).Font).Width + 10
                        g_toolTip.SetToolTip(aControlTimerLabel(aScreenObject(iLoop).Control_Index), aScreenObject(iLoop).Object_Name & " " & aScreenObject(iLoop).Object_State & " (For: " & dtTime & ")")

                        ', aScreenObject(iLoop).Object_Name & " " & aScreenObject(iLoop).Object_State & " (For: " & dtTime & ")")
                    Else
                        If aScreenObject(iLoop).Object_State = "OFF" Then
                            aControlTimerLabel(aScreenObject(iLoop).Control_Index).Text = "OFF"
                            Dim g As Graphics = aControlTimerLabel(aScreenObject(iLoop).Control_Index).CreateGraphics
                            aControlTimerLabel(aScreenObject(iLoop).Control_Index).Width = g.MeasureString(aControlTimerLabel(aScreenObject(iLoop).Control_Index).Text, aControlTimerLabel(aScreenObject(iLoop).Control_Index).Font).Width + 10

                        Else
                            dtTarget = DateAdd("s", Val(aScreenObject(iLoop).Property_Value), aScreenObject(iLoop).Object_Last_Updated)
                            dSeconds = DateDiff(DateInterval.Second, Now(), dtTarget)
                            If dSeconds < 0 Then dSeconds = 0
                            '            ' Check to see if the State has change for these controls
                            dtTime = Format(DateAdd("s", dSeconds, "00:00:00"), "HH:mm:ss")

                            '            sPropertyBlock = Read_Properties(aScreenObject(iLoop).Object_Name)
                            aControlTimerLabel(aScreenObject(iLoop).Control_Index).Text = dtTime
                            Dim g As Graphics = aControlTimerLabel(aScreenObject(iLoop).Control_Index).CreateGraphics
                            aControlTimerLabel(aScreenObject(iLoop).Control_Index).Width = g.MeasureString(aControlTimerLabel(aScreenObject(iLoop).Control_Index).Text, aControlTimerLabel(aScreenObject(iLoop).Control_Index).Font).Width + 10

                            g_toolTip.SetToolTip(aControlTimerLabel(aScreenObject(iLoop).Control_Index), aScreenObject(iLoop).Object_Name & " " & aScreenObject(iLoop).Object_State & " (OFF in: " & dtTime & ")")
                        End If
                    End If
                End If
                'End If
            Next iLoop
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TEST_Load_Weather()
        Dim oWeather As New ucWeather
        Me.Controls.Add(oWeather)
        oWeather.BringToFront()
    End Sub

    Private Sub GUI_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Dim rect As Rectangle = Screen.PrimaryScreen.WorkingArea
        'Divide the screen in half, and find the center of the form to center it
        Me.Top = (rect.Height / 2) - (Me.Height / 2)
        Me.Left = (rect.Width / 2) - (Me.Width / 2)
        '  Me.Height = Me.BackgroundImage.Height + 34
        '  Me.Width = Me.BackgroundImage.Width + 12
        Try
            If gXOriginal = 0 Or gYOriginal = 0 Then Exit Sub
            gXRatio = (Me.Width - 0) / gXLast
            gYRatio = (Me.Height + 0) / gYLast
            'Me.Height = Me.BackgroundImage.Height + 34
            'Me.Width = Me.BackgroundImage.Width + 12
            gXFullRatio = Me.Width / gXOriginal
            gYFullRatio = Me.Height / gYOriginal
            'lblX.Text = gXRatio
            'lblY.Text = gYRatio
            Dim tControl As Control
            ' If gYRatio Then
            For Each tControl In Me.Controls
                Debug.Print(tControl.Name)
                tControl.Height = tControl.Height * gYRatio
                tControl.Width = tControl.Width * gXRatio
                tControl.Top = tControl.Top * gYRatio
                tControl.Left = tControl.Left * gXRatio
            Next
        Catch myerror As Exception
            '    MessageBox.Show("GUI Error GUI_Resize: " & myerror.Message)

        End Try
        gXLast = Me.Width
        gYLast = Me.Height
    End Sub

    Private Sub GUI_Layout(sender As Object, e As System.Windows.Forms.LayoutEventArgs) Handles Me.Layout




    End Sub

    Private Sub CameraViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CameraViewerToolStripMenuItem.Click
        frmAddCameraViewer.Show()
    End Sub
End Class
