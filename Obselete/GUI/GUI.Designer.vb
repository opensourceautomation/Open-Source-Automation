<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GUI
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GUI))
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.cmsMain = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuChangeScreen = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuViewLogs = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuEditMode = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuAddControl = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuStateImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuPropertyLabel = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuMethodImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuNavigationImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuTimerLabel = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuUserControl = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCreateScreen = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCreateObject = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuTools = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuObjects = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuObjectTypes = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuPatternEditor = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuScriptEditor = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuSchedules = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuScreensTool = New System.Windows.Forms.ToolStripMenuItem()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.CameraViewerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.cmsMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'Timer1
        '
        Me.Timer1.Interval = 500
        '
        'cmsMain
        '
        Me.cmsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuChangeScreen, Me.mnuViewLogs, Me.mnuEditMode, Me.mnuAddControl, Me.mnuCreateScreen, Me.mnuCreateObject, Me.mnuTools})
        Me.cmsMain.Name = "cmsMain"
        Me.cmsMain.Size = New System.Drawing.Size(154, 180)
        '
        'mnuChangeScreen
        '
        Me.mnuChangeScreen.Name = "mnuChangeScreen"
        Me.mnuChangeScreen.Size = New System.Drawing.Size(153, 22)
        Me.mnuChangeScreen.Text = "Change Screen"
        '
        'mnuViewLogs
        '
        Me.mnuViewLogs.Name = "mnuViewLogs"
        Me.mnuViewLogs.Size = New System.Drawing.Size(153, 22)
        Me.mnuViewLogs.Text = "View Logs"
        '
        'mnuEditMode
        '
        Me.mnuEditMode.Name = "mnuEditMode"
        Me.mnuEditMode.Size = New System.Drawing.Size(153, 22)
        Me.mnuEditMode.Text = "Edit Mode"
        '
        'mnuAddControl
        '
        Me.mnuAddControl.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuStateImage, Me.mnuPropertyLabel, Me.mnuMethodImage, Me.mnuNavigationImage, Me.mnuTimerLabel, Me.mnuUserControl, Me.CameraViewerToolStripMenuItem})
        Me.mnuAddControl.Name = "mnuAddControl"
        Me.mnuAddControl.Size = New System.Drawing.Size(153, 22)
        Me.mnuAddControl.Text = "Add Control"
        '
        'mnuStateImage
        '
        Me.mnuStateImage.Name = "mnuStateImage"
        Me.mnuStateImage.Size = New System.Drawing.Size(168, 22)
        Me.mnuStateImage.Text = "State Image"
        '
        'mnuPropertyLabel
        '
        Me.mnuPropertyLabel.Name = "mnuPropertyLabel"
        Me.mnuPropertyLabel.Size = New System.Drawing.Size(168, 22)
        Me.mnuPropertyLabel.Text = "Property Label"
        '
        'mnuMethodImage
        '
        Me.mnuMethodImage.Name = "mnuMethodImage"
        Me.mnuMethodImage.Size = New System.Drawing.Size(168, 22)
        Me.mnuMethodImage.Text = "Method Image"
        '
        'mnuNavigationImage
        '
        Me.mnuNavigationImage.Name = "mnuNavigationImage"
        Me.mnuNavigationImage.Size = New System.Drawing.Size(168, 22)
        Me.mnuNavigationImage.Text = "Navigation Image"
        '
        'mnuTimerLabel
        '
        Me.mnuTimerLabel.Name = "mnuTimerLabel"
        Me.mnuTimerLabel.Size = New System.Drawing.Size(168, 22)
        Me.mnuTimerLabel.Text = "Timer Label"
        '
        'mnuUserControl
        '
        Me.mnuUserControl.Name = "mnuUserControl"
        Me.mnuUserControl.Size = New System.Drawing.Size(168, 22)
        Me.mnuUserControl.Text = "User Control"
        '
        'mnuCreateScreen
        '
        Me.mnuCreateScreen.Name = "mnuCreateScreen"
        Me.mnuCreateScreen.Size = New System.Drawing.Size(153, 22)
        Me.mnuCreateScreen.Text = "Create Screen"
        '
        'mnuCreateObject
        '
        Me.mnuCreateObject.Name = "mnuCreateObject"
        Me.mnuCreateObject.Size = New System.Drawing.Size(153, 22)
        Me.mnuCreateObject.Text = "Create Object"
        '
        'mnuTools
        '
        Me.mnuTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuObjects, Me.mnuObjectTypes, Me.mnuPatternEditor, Me.mnuScriptEditor, Me.mnuSchedules, Me.mnuScreensTool})
        Me.mnuTools.Name = "mnuTools"
        Me.mnuTools.Size = New System.Drawing.Size(153, 22)
        Me.mnuTools.Text = "Tools"
        '
        'mnuObjects
        '
        Me.mnuObjects.Name = "mnuObjects"
        Me.mnuObjects.Size = New System.Drawing.Size(151, 22)
        Me.mnuObjects.Text = "Objects"
        '
        'mnuObjectTypes
        '
        Me.mnuObjectTypes.Name = "mnuObjectTypes"
        Me.mnuObjectTypes.Size = New System.Drawing.Size(151, 22)
        Me.mnuObjectTypes.Text = "Object Types"
        '
        'mnuPatternEditor
        '
        Me.mnuPatternEditor.Name = "mnuPatternEditor"
        Me.mnuPatternEditor.Size = New System.Drawing.Size(151, 22)
        Me.mnuPatternEditor.Text = "Named Scripts"
        '
        'mnuScriptEditor
        '
        Me.mnuScriptEditor.Name = "mnuScriptEditor"
        Me.mnuScriptEditor.Size = New System.Drawing.Size(151, 22)
        Me.mnuScriptEditor.Text = "Script Editor"
        '
        'mnuSchedules
        '
        Me.mnuSchedules.Name = "mnuSchedules"
        Me.mnuSchedules.Size = New System.Drawing.Size(151, 22)
        Me.mnuSchedules.Text = "Schedules"
        '
        'mnuScreensTool
        '
        Me.mnuScreensTool.Name = "mnuScreensTool"
        Me.mnuScreensTool.Size = New System.Drawing.Size(151, 22)
        Me.mnuScreensTool.Text = "Screens"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(780, 462)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(204, 18)
        Me.TextBox1.TabIndex = 1
        Me.TextBox1.Visible = False
        '
        'CameraViewerToolStripMenuItem
        '
        Me.CameraViewerToolStripMenuItem.Name = "CameraViewerToolStripMenuItem"
        Me.CameraViewerToolStripMenuItem.Size = New System.Drawing.Size(168, 22)
        Me.CameraViewerToolStripMenuItem.Text = "Camera Viewer"
        '
        'GUI
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.White
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(1084, 525)
        Me.ContextMenuStrip = Me.cmsMain
        Me.Controls.Add(Me.TextBox1)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Lucida Console", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Name = "GUI"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "OSAE GUI"
        Me.cmsMain.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents cmsMain As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuEditMode As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAddControl As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuStateImage As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPropertyLabel As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuMethodImage As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuNavigationImage As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCreateScreen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCreateObject As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuChangeScreen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewLogs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents mnuTools As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuObjectTypes As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPatternEditor As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSchedules As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuScriptEditor As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuObjects As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuScreensTool As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuTimerLabel As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUserControl As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CameraViewerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
