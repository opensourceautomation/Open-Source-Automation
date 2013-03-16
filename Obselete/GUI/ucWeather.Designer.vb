<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucWeather
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.lblConditions = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.picToday = New System.Windows.Forms.PictureBox()
        Me.lblTemperatureCurrent = New System.Windows.Forms.Label()
        Me.lblDay2 = New System.Windows.Forms.Label()
        Me.lblLow2 = New System.Windows.Forms.Label()
        Me.lblDay3 = New System.Windows.Forms.Label()
        Me.lblDay4 = New System.Windows.Forms.Label()
        Me.lblHigh2 = New System.Windows.Forms.Label()
        Me.lblLow3 = New System.Windows.Forms.Label()
        Me.lblHigh3 = New System.Windows.Forms.Label()
        Me.lblLow4 = New System.Windows.Forms.Label()
        Me.lblHigh4 = New System.Windows.Forms.Label()
        Me.lblLowCurrent = New System.Windows.Forms.Label()
        Me.lblHighCurrent = New System.Windows.Forms.Label()
        Me.picDay2 = New System.Windows.Forms.PictureBox()
        Me.picDay3 = New System.Windows.Forms.PictureBox()
        Me.picDay4 = New System.Windows.Forms.PictureBox()
        Me.picDay5 = New System.Windows.Forms.PictureBox()
        Me.lblLow5 = New System.Windows.Forms.Label()
        Me.lblHigh5 = New System.Windows.Forms.Label()
        Me.lblDay5 = New System.Windows.Forms.Label()
        Me.picTonight = New System.Windows.Forms.PictureBox()
        Me.picNight2 = New System.Windows.Forms.PictureBox()
        Me.picNight3 = New System.Windows.Forms.PictureBox()
        Me.picNight4 = New System.Windows.Forms.PictureBox()
        Me.picNight5 = New System.Windows.Forms.PictureBox()
        Me.lblDay1 = New System.Windows.Forms.Label()
        Me.lblLow1 = New System.Windows.Forms.Label()
        Me.lblHigh1 = New System.Windows.Forms.Label()
        Me.picDay1 = New System.Windows.Forms.PictureBox()
        Me.picNight1 = New System.Windows.Forms.PictureBox()
        Me.lblLastUpdated = New System.Windows.Forms.Label()
        Me.timMain = New System.Windows.Forms.Timer(Me.components)
        CType(Me.picToday, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picDay2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picDay3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picDay4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picDay5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picTonight, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picNight2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picNight3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picNight4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picNight5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picDay1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picNight1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblConditions
        '
        Me.lblConditions.Location = New System.Drawing.Point(115, 12)
        Me.lblConditions.Name = "lblConditions"
        Me.lblConditions.Size = New System.Drawing.Size(357, 30)
        Me.lblConditions.TabIndex = 2
        Me.lblConditions.Text = "Conditions"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(118, 238)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(354, 34)
        Me.Label2.TabIndex = 61
        Me.Label2.Text = "Label2"
        '
        'picToday
        '
        Me.picToday.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picToday.Location = New System.Drawing.Point(10, 43)
        Me.picToday.Name = "picToday"
        Me.picToday.Size = New System.Drawing.Size(90, 90)
        Me.picToday.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picToday.TabIndex = 1
        Me.picToday.TabStop = False
        '
        'lblTemperatureCurrent
        '
        Me.lblTemperatureCurrent.AutoSize = True
        Me.lblTemperatureCurrent.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTemperatureCurrent.Location = New System.Drawing.Point(22, 7)
        Me.lblTemperatureCurrent.Name = "lblTemperatureCurrent"
        Me.lblTemperatureCurrent.Size = New System.Drawing.Size(63, 20)
        Me.lblTemperatureCurrent.TabIndex = 3
        Me.lblTemperatureCurrent.Text = "Label1"
        '
        'lblDay2
        '
        Me.lblDay2.Location = New System.Drawing.Point(187, 49)
        Me.lblDay2.Name = "lblDay2"
        Me.lblDay2.Size = New System.Drawing.Size(66, 13)
        Me.lblDay2.TabIndex = 6
        Me.lblDay2.Text = "Day 2"
        '
        'lblLow2
        '
        Me.lblLow2.AutoSize = True
        Me.lblLow2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLow2.ForeColor = System.Drawing.Color.SteelBlue
        Me.lblLow2.Location = New System.Drawing.Point(190, 151)
        Me.lblLow2.Name = "lblLow2"
        Me.lblLow2.Size = New System.Drawing.Size(55, 13)
        Me.lblLow2.TabIndex = 9
        Me.lblLow2.Text = "Low: xx°"
        '
        'lblDay3
        '
        Me.lblDay3.Location = New System.Drawing.Point(259, 49)
        Me.lblDay3.Name = "lblDay3"
        Me.lblDay3.Size = New System.Drawing.Size(66, 13)
        Me.lblDay3.TabIndex = 11
        Me.lblDay3.Text = "Day 3"
        '
        'lblDay4
        '
        Me.lblDay4.Location = New System.Drawing.Point(331, 49)
        Me.lblDay4.Name = "lblDay4"
        Me.lblDay4.Size = New System.Drawing.Size(66, 13)
        Me.lblDay4.TabIndex = 15
        Me.lblDay4.Text = "Day 4"
        '
        'lblHigh2
        '
        Me.lblHigh2.AutoSize = True
        Me.lblHigh2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHigh2.ForeColor = System.Drawing.Color.Crimson
        Me.lblHigh2.Location = New System.Drawing.Point(190, 133)
        Me.lblHigh2.Name = "lblHigh2"
        Me.lblHigh2.Size = New System.Drawing.Size(48, 13)
        Me.lblHigh2.TabIndex = 21
        Me.lblHigh2.Text = "Hi : xx°"
        '
        'lblLow3
        '
        Me.lblLow3.AutoSize = True
        Me.lblLow3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLow3.ForeColor = System.Drawing.Color.SteelBlue
        Me.lblLow3.Location = New System.Drawing.Point(262, 151)
        Me.lblLow3.Name = "lblLow3"
        Me.lblLow3.Size = New System.Drawing.Size(55, 13)
        Me.lblLow3.TabIndex = 22
        Me.lblLow3.Text = "Low: xx°"
        '
        'lblHigh3
        '
        Me.lblHigh3.AutoSize = True
        Me.lblHigh3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHigh3.ForeColor = System.Drawing.Color.Crimson
        Me.lblHigh3.Location = New System.Drawing.Point(262, 133)
        Me.lblHigh3.Name = "lblHigh3"
        Me.lblHigh3.Size = New System.Drawing.Size(48, 13)
        Me.lblHigh3.TabIndex = 23
        Me.lblHigh3.Text = "Hi : xx°"
        '
        'lblLow4
        '
        Me.lblLow4.AutoSize = True
        Me.lblLow4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLow4.ForeColor = System.Drawing.Color.SteelBlue
        Me.lblLow4.Location = New System.Drawing.Point(334, 151)
        Me.lblLow4.Name = "lblLow4"
        Me.lblLow4.Size = New System.Drawing.Size(55, 13)
        Me.lblLow4.TabIndex = 24
        Me.lblLow4.Text = "Low: xx°"
        '
        'lblHigh4
        '
        Me.lblHigh4.AutoSize = True
        Me.lblHigh4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHigh4.ForeColor = System.Drawing.Color.Crimson
        Me.lblHigh4.Location = New System.Drawing.Point(334, 133)
        Me.lblHigh4.Name = "lblHigh4"
        Me.lblHigh4.Size = New System.Drawing.Size(48, 13)
        Me.lblHigh4.TabIndex = 25
        Me.lblHigh4.Text = "Hi : xx°"
        '
        'lblLowCurrent
        '
        Me.lblLowCurrent.AutoSize = True
        Me.lblLowCurrent.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLowCurrent.ForeColor = System.Drawing.Color.SteelBlue
        Me.lblLowCurrent.Location = New System.Drawing.Point(10, 151)
        Me.lblLowCurrent.Name = "lblLowCurrent"
        Me.lblLowCurrent.Size = New System.Drawing.Size(55, 13)
        Me.lblLowCurrent.TabIndex = 28
        Me.lblLowCurrent.Text = "Low: xx°"
        '
        'lblHighCurrent
        '
        Me.lblHighCurrent.AutoSize = True
        Me.lblHighCurrent.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHighCurrent.ForeColor = System.Drawing.Color.Crimson
        Me.lblHighCurrent.Location = New System.Drawing.Point(10, 133)
        Me.lblHighCurrent.Name = "lblHighCurrent"
        Me.lblHighCurrent.Size = New System.Drawing.Size(44, 13)
        Me.lblHighCurrent.TabIndex = 29
        Me.lblHighCurrent.Text = "Hi: xx°"
        '
        'picDay2
        '
        Me.picDay2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picDay2.Location = New System.Drawing.Point(190, 67)
        Me.picDay2.Name = "picDay2"
        Me.picDay2.Size = New System.Drawing.Size(66, 66)
        Me.picDay2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picDay2.TabIndex = 31
        Me.picDay2.TabStop = False
        '
        'picDay3
        '
        Me.picDay3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picDay3.Location = New System.Drawing.Point(262, 67)
        Me.picDay3.Name = "picDay3"
        Me.picDay3.Size = New System.Drawing.Size(66, 66)
        Me.picDay3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picDay3.TabIndex = 33
        Me.picDay3.TabStop = False
        '
        'picDay4
        '
        Me.picDay4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picDay4.Location = New System.Drawing.Point(334, 67)
        Me.picDay4.Name = "picDay4"
        Me.picDay4.Size = New System.Drawing.Size(66, 66)
        Me.picDay4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picDay4.TabIndex = 35
        Me.picDay4.TabStop = False
        '
        'picDay5
        '
        Me.picDay5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picDay5.Location = New System.Drawing.Point(406, 67)
        Me.picDay5.Name = "picDay5"
        Me.picDay5.Size = New System.Drawing.Size(66, 66)
        Me.picDay5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picDay5.TabIndex = 37
        Me.picDay5.TabStop = False
        '
        'lblLow5
        '
        Me.lblLow5.AutoSize = True
        Me.lblLow5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLow5.ForeColor = System.Drawing.Color.SteelBlue
        Me.lblLow5.Location = New System.Drawing.Point(406, 151)
        Me.lblLow5.Name = "lblLow5"
        Me.lblLow5.Size = New System.Drawing.Size(55, 13)
        Me.lblLow5.TabIndex = 39
        Me.lblLow5.Text = "Low: xx°"
        '
        'lblHigh5
        '
        Me.lblHigh5.AutoSize = True
        Me.lblHigh5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHigh5.ForeColor = System.Drawing.Color.Crimson
        Me.lblHigh5.Location = New System.Drawing.Point(406, 133)
        Me.lblHigh5.Name = "lblHigh5"
        Me.lblHigh5.Size = New System.Drawing.Size(48, 13)
        Me.lblHigh5.TabIndex = 40
        Me.lblHigh5.Text = "Hi : xx°"
        '
        'lblDay5
        '
        Me.lblDay5.Location = New System.Drawing.Point(403, 49)
        Me.lblDay5.Name = "lblDay5"
        Me.lblDay5.Size = New System.Drawing.Size(66, 13)
        Me.lblDay5.TabIndex = 41
        Me.lblDay5.Text = "Day 5"
        '
        'picTonight
        '
        Me.picTonight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picTonight.Location = New System.Drawing.Point(10, 169)
        Me.picTonight.Name = "picTonight"
        Me.picTonight.Size = New System.Drawing.Size(90, 90)
        Me.picTonight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picTonight.TabIndex = 30
        Me.picTonight.TabStop = False
        '
        'picNight2
        '
        Me.picNight2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picNight2.Location = New System.Drawing.Point(190, 169)
        Me.picNight2.Name = "picNight2"
        Me.picNight2.Size = New System.Drawing.Size(66, 66)
        Me.picNight2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picNight2.TabIndex = 32
        Me.picNight2.TabStop = False
        '
        'picNight3
        '
        Me.picNight3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picNight3.Location = New System.Drawing.Point(262, 169)
        Me.picNight3.Name = "picNight3"
        Me.picNight3.Size = New System.Drawing.Size(66, 66)
        Me.picNight3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picNight3.TabIndex = 34
        Me.picNight3.TabStop = False
        '
        'picNight4
        '
        Me.picNight4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picNight4.Location = New System.Drawing.Point(334, 169)
        Me.picNight4.Name = "picNight4"
        Me.picNight4.Size = New System.Drawing.Size(66, 66)
        Me.picNight4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picNight4.TabIndex = 36
        Me.picNight4.TabStop = False
        '
        'picNight5
        '
        Me.picNight5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picNight5.Location = New System.Drawing.Point(406, 169)
        Me.picNight5.Name = "picNight5"
        Me.picNight5.Size = New System.Drawing.Size(66, 66)
        Me.picNight5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picNight5.TabIndex = 38
        Me.picNight5.TabStop = False
        '
        'lblDay1
        '
        Me.lblDay1.Location = New System.Drawing.Point(115, 49)
        Me.lblDay1.Name = "lblDay1"
        Me.lblDay1.Size = New System.Drawing.Size(66, 13)
        Me.lblDay1.TabIndex = 52
        Me.lblDay1.Text = "Day 1"
        '
        'lblLow1
        '
        Me.lblLow1.AutoSize = True
        Me.lblLow1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLow1.ForeColor = System.Drawing.Color.SteelBlue
        Me.lblLow1.Location = New System.Drawing.Point(118, 151)
        Me.lblLow1.Name = "lblLow1"
        Me.lblLow1.Size = New System.Drawing.Size(55, 13)
        Me.lblLow1.TabIndex = 53
        Me.lblLow1.Text = "Low: xx°"
        '
        'lblHigh1
        '
        Me.lblHigh1.AutoSize = True
        Me.lblHigh1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHigh1.ForeColor = System.Drawing.Color.Crimson
        Me.lblHigh1.Location = New System.Drawing.Point(118, 133)
        Me.lblHigh1.Name = "lblHigh1"
        Me.lblHigh1.Size = New System.Drawing.Size(48, 13)
        Me.lblHigh1.TabIndex = 54
        Me.lblHigh1.Text = "Hi : xx°"
        '
        'picDay1
        '
        Me.picDay1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picDay1.Location = New System.Drawing.Point(118, 67)
        Me.picDay1.Name = "picDay1"
        Me.picDay1.Size = New System.Drawing.Size(66, 66)
        Me.picDay1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picDay1.TabIndex = 55
        Me.picDay1.TabStop = False
        '
        'picNight1
        '
        Me.picNight1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.picNight1.Location = New System.Drawing.Point(118, 169)
        Me.picNight1.Name = "picNight1"
        Me.picNight1.Size = New System.Drawing.Size(66, 66)
        Me.picNight1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picNight1.TabIndex = 56
        Me.picNight1.TabStop = False
        '
        'lblLastUpdated
        '
        Me.lblLastUpdated.Location = New System.Drawing.Point(10, 272)
        Me.lblLastUpdated.Name = "lblLastUpdated"
        Me.lblLastUpdated.Size = New System.Drawing.Size(420, 24)
        Me.lblLastUpdated.TabIndex = 62
        Me.lblLastUpdated.Text = "Label1"
        '
        'timMain
        '
        Me.timMain.Interval = 60000
        '
        'ucWeather
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.Gainsboro
        Me.Controls.Add(Me.lblLastUpdated)
        Me.Controls.Add(Me.picDay1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblHigh1)
        Me.Controls.Add(Me.lblLow1)
        Me.Controls.Add(Me.lblDay1)
        Me.Controls.Add(Me.lblConditions)
        Me.Controls.Add(Me.picNight5)
        Me.Controls.Add(Me.lblTemperatureCurrent)
        Me.Controls.Add(Me.picNight4)
        Me.Controls.Add(Me.picToday)
        Me.Controls.Add(Me.picNight3)
        Me.Controls.Add(Me.lblDay2)
        Me.Controls.Add(Me.picNight2)
        Me.Controls.Add(Me.lblLow2)
        Me.Controls.Add(Me.picTonight)
        Me.Controls.Add(Me.lblDay3)
        Me.Controls.Add(Me.lblDay4)
        Me.Controls.Add(Me.lblHigh2)
        Me.Controls.Add(Me.lblLow3)
        Me.Controls.Add(Me.lblDay5)
        Me.Controls.Add(Me.lblHigh3)
        Me.Controls.Add(Me.lblHigh5)
        Me.Controls.Add(Me.lblLow4)
        Me.Controls.Add(Me.lblLow5)
        Me.Controls.Add(Me.lblHigh4)
        Me.Controls.Add(Me.picDay5)
        Me.Controls.Add(Me.lblLowCurrent)
        Me.Controls.Add(Me.picDay4)
        Me.Controls.Add(Me.lblHighCurrent)
        Me.Controls.Add(Me.picDay3)
        Me.Controls.Add(Me.picDay2)
        Me.Controls.Add(Me.picNight1)
        Me.DoubleBuffered = True
        Me.Name = "ucWeather"
        Me.Size = New System.Drawing.Size(480, 300)
        CType(Me.picToday, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picDay2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picDay3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picDay4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picDay5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picTonight, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picNight2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picNight3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picNight4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picNight5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picDay1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picNight1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblConditions As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents picToday As System.Windows.Forms.PictureBox
    Friend WithEvents lblTemperatureCurrent As System.Windows.Forms.Label
    Friend WithEvents lblDay2 As System.Windows.Forms.Label
    Friend WithEvents lblLow2 As System.Windows.Forms.Label
    Friend WithEvents lblDay3 As System.Windows.Forms.Label
    Friend WithEvents lblDay4 As System.Windows.Forms.Label
    Friend WithEvents lblHigh2 As System.Windows.Forms.Label
    Friend WithEvents lblLow3 As System.Windows.Forms.Label
    Friend WithEvents lblHigh3 As System.Windows.Forms.Label
    Friend WithEvents lblLow4 As System.Windows.Forms.Label
    Friend WithEvents lblHigh4 As System.Windows.Forms.Label
    Friend WithEvents lblLowCurrent As System.Windows.Forms.Label
    Friend WithEvents lblHighCurrent As System.Windows.Forms.Label
    Friend WithEvents picDay2 As System.Windows.Forms.PictureBox
    Friend WithEvents picDay3 As System.Windows.Forms.PictureBox
    Friend WithEvents picDay4 As System.Windows.Forms.PictureBox
    Friend WithEvents picDay5 As System.Windows.Forms.PictureBox
    Friend WithEvents lblLow5 As System.Windows.Forms.Label
    Friend WithEvents lblHigh5 As System.Windows.Forms.Label
    Friend WithEvents lblDay5 As System.Windows.Forms.Label
    Friend WithEvents picTonight As System.Windows.Forms.PictureBox
    Friend WithEvents picNight2 As System.Windows.Forms.PictureBox
    Friend WithEvents picNight3 As System.Windows.Forms.PictureBox
    Friend WithEvents picNight4 As System.Windows.Forms.PictureBox
    Friend WithEvents picNight5 As System.Windows.Forms.PictureBox
    Friend WithEvents lblDay1 As System.Windows.Forms.Label
    Friend WithEvents lblLow1 As System.Windows.Forms.Label
    Friend WithEvents lblHigh1 As System.Windows.Forms.Label
    Friend WithEvents picDay1 As System.Windows.Forms.PictureBox
    Friend WithEvents picNight1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblLastUpdated As System.Windows.Forms.Label
    Friend WithEvents timMain As System.Windows.Forms.Timer

End Class
