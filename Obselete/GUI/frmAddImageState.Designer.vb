<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddImageState
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAddImageState))
        Me.btnOffImage = New System.Windows.Forms.Button()
        Me.txtOFF = New System.Windows.Forms.TextBox()
        Me.btnOnImage = New System.Windows.Forms.Button()
        Me.txtON = New System.Windows.Forms.TextBox()
        Me.lblImage1 = New System.Windows.Forms.Label()
        Me.btnCancelAdd = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.lblObject = New System.Windows.Forms.Label()
        Me.cboAddObject = New System.Windows.Forms.ComboBox()
        Me.picON = New System.Windows.Forms.PictureBox()
        Me.picOFF = New System.Windows.Forms.PictureBox()
        Me.file1 = New System.Windows.Forms.OpenFileDialog()
        Me.btnALARMImage = New System.Windows.Forms.Button()
        Me.txtALARM = New System.Windows.Forms.TextBox()
        Me.picAlarm = New System.Windows.Forms.PictureBox()
        Me.lblImage3 = New System.Windows.Forms.Label()
        Me.lblImage2 = New System.Windows.Forms.Label()
        Me.lblON = New System.Windows.Forms.Label()
        Me.lblOFF = New System.Windows.Forms.Label()
        Me.lblALARM = New System.Windows.Forms.Label()
        CType(Me.picON, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picOFF, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picAlarm, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnOffImage
        '
        Me.btnOffImage.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOffImage.Location = New System.Drawing.Point(409, 120)
        Me.btnOffImage.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnOffImage.Name = "btnOffImage"
        Me.btnOffImage.Size = New System.Drawing.Size(24, 24)
        Me.btnOffImage.TabIndex = 81
        Me.btnOffImage.Text = "..."
        Me.btnOffImage.UseVisualStyleBackColor = True
        Me.btnOffImage.Visible = False
        '
        'txtOFF
        '
        Me.txtOFF.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOFF.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtOFF.Location = New System.Drawing.Point(162, 120)
        Me.txtOFF.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtOFF.Name = "txtOFF"
        Me.txtOFF.Size = New System.Drawing.Size(237, 26)
        Me.txtOFF.TabIndex = 80
        Me.txtOFF.Visible = False
        '
        'btnOnImage
        '
        Me.btnOnImage.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOnImage.Location = New System.Drawing.Point(409, 60)
        Me.btnOnImage.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnOnImage.Name = "btnOnImage"
        Me.btnOnImage.Size = New System.Drawing.Size(24, 24)
        Me.btnOnImage.TabIndex = 79
        Me.btnOnImage.Text = "..."
        Me.btnOnImage.UseVisualStyleBackColor = True
        Me.btnOnImage.Visible = False
        '
        'txtON
        '
        Me.txtON.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtON.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtON.Location = New System.Drawing.Point(162, 60)
        Me.txtON.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtON.Name = "txtON"
        Me.txtON.Size = New System.Drawing.Size(237, 26)
        Me.txtON.TabIndex = 78
        Me.txtON.Visible = False
        '
        'lblImage1
        '
        Me.lblImage1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblImage1.Location = New System.Drawing.Point(102, 66)
        Me.lblImage1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblImage1.Name = "lblImage1"
        Me.lblImage1.Size = New System.Drawing.Size(54, 24)
        Me.lblImage1.TabIndex = 76
        Me.lblImage1.Text = "Image"
        Me.lblImage1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblImage1.Visible = False
        '
        'btnCancelAdd
        '
        Me.btnCancelAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancelAdd.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancelAdd.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancelAdd.Location = New System.Drawing.Point(407, 240)
        Me.btnCancelAdd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnCancelAdd.Name = "btnCancelAdd"
        Me.btnCancelAdd.Size = New System.Drawing.Size(74, 30)
        Me.btnCancelAdd.TabIndex = 75
        Me.btnCancelAdd.Text = "Cancel"
        Me.btnCancelAdd.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAdd.Location = New System.Drawing.Point(24, 240)
        Me.btnAdd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(74, 30)
        Me.btnAdd.TabIndex = 74
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseVisualStyleBackColor = True
        Me.btnAdd.Visible = False
        '
        'lblObject
        '
        Me.lblObject.AutoSize = True
        Me.lblObject.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblObject.Location = New System.Drawing.Point(54, 12)
        Me.lblObject.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblObject.Name = "lblObject"
        Me.lblObject.Size = New System.Drawing.Size(55, 20)
        Me.lblObject.TabIndex = 73
        Me.lblObject.Text = "Object"
        Me.lblObject.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboAddObject
        '
        Me.cboAddObject.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboAddObject.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboAddObject.FormattingEnabled = True
        Me.cboAddObject.Location = New System.Drawing.Point(120, 6)
        Me.cboAddObject.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cboAddObject.Name = "cboAddObject"
        Me.cboAddObject.Size = New System.Drawing.Size(384, 28)
        Me.cboAddObject.TabIndex = 72
        '
        'picON
        '
        Me.picON.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.picON.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picON.Location = New System.Drawing.Point(443, 48)
        Me.picON.Name = "picON"
        Me.picON.Size = New System.Drawing.Size(66, 54)
        Me.picON.TabIndex = 82
        Me.picON.TabStop = False
        Me.picON.Visible = False
        '
        'picOFF
        '
        Me.picOFF.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.picOFF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picOFF.Location = New System.Drawing.Point(443, 108)
        Me.picOFF.Name = "picOFF"
        Me.picOFF.Size = New System.Drawing.Size(66, 54)
        Me.picOFF.TabIndex = 83
        Me.picOFF.TabStop = False
        Me.picOFF.Visible = False
        '
        'file1
        '
        Me.file1.FileName = "file1"
        '
        'btnALARMImage
        '
        Me.btnALARMImage.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnALARMImage.Location = New System.Drawing.Point(409, 180)
        Me.btnALARMImage.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnALARMImage.Name = "btnALARMImage"
        Me.btnALARMImage.Size = New System.Drawing.Size(24, 24)
        Me.btnALARMImage.TabIndex = 86
        Me.btnALARMImage.Text = "..."
        Me.btnALARMImage.UseVisualStyleBackColor = True
        Me.btnALARMImage.Visible = False
        '
        'txtALARM
        '
        Me.txtALARM.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtALARM.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtALARM.Location = New System.Drawing.Point(162, 180)
        Me.txtALARM.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtALARM.Name = "txtALARM"
        Me.txtALARM.Size = New System.Drawing.Size(237, 26)
        Me.txtALARM.TabIndex = 85
        Me.txtALARM.Visible = False
        '
        'picAlarm
        '
        Me.picAlarm.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.picAlarm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picAlarm.Location = New System.Drawing.Point(443, 168)
        Me.picAlarm.Name = "picAlarm"
        Me.picAlarm.Size = New System.Drawing.Size(66, 54)
        Me.picAlarm.TabIndex = 87
        Me.picAlarm.TabStop = False
        Me.picAlarm.Visible = False
        '
        'lblImage3
        '
        Me.lblImage3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblImage3.Location = New System.Drawing.Point(102, 186)
        Me.lblImage3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblImage3.Name = "lblImage3"
        Me.lblImage3.Size = New System.Drawing.Size(54, 24)
        Me.lblImage3.TabIndex = 88
        Me.lblImage3.Text = "Image"
        Me.lblImage3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblImage3.Visible = False
        '
        'lblImage2
        '
        Me.lblImage2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblImage2.Location = New System.Drawing.Point(102, 126)
        Me.lblImage2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblImage2.Name = "lblImage2"
        Me.lblImage2.Size = New System.Drawing.Size(54, 24)
        Me.lblImage2.TabIndex = 89
        Me.lblImage2.Text = "Image"
        Me.lblImage2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblImage2.Visible = False
        '
        'lblON
        '
        Me.lblON.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblON.Location = New System.Drawing.Point(6, 66)
        Me.lblON.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblON.Name = "lblON"
        Me.lblON.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblON.Size = New System.Drawing.Size(90, 24)
        Me.lblON.TabIndex = 90
        Me.lblON.Text = "Image"
        Me.lblON.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblON.Visible = False
        '
        'lblOFF
        '
        Me.lblOFF.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblOFF.Location = New System.Drawing.Point(6, 126)
        Me.lblOFF.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblOFF.Name = "lblOFF"
        Me.lblOFF.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblOFF.Size = New System.Drawing.Size(90, 24)
        Me.lblOFF.TabIndex = 91
        Me.lblOFF.Text = "Image"
        Me.lblOFF.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblOFF.Visible = False
        '
        'lblALARM
        '
        Me.lblALARM.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblALARM.Location = New System.Drawing.Point(6, 186)
        Me.lblALARM.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblALARM.Name = "lblALARM"
        Me.lblALARM.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblALARM.Size = New System.Drawing.Size(90, 24)
        Me.lblALARM.TabIndex = 92
        Me.lblALARM.Text = "Image"
        Me.lblALARM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblALARM.Visible = False
        '
        'frmAddImageState
        '
        Me.AcceptButton = Me.btnAdd
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.Gainsboro
        Me.CancelButton = Me.btnCancelAdd
        Me.ClientSize = New System.Drawing.Size(516, 277)
        Me.Controls.Add(Me.lblALARM)
        Me.Controls.Add(Me.lblOFF)
        Me.Controls.Add(Me.lblON)
        Me.Controls.Add(Me.lblImage2)
        Me.Controls.Add(Me.lblImage3)
        Me.Controls.Add(Me.picAlarm)
        Me.Controls.Add(Me.btnALARMImage)
        Me.Controls.Add(Me.txtALARM)
        Me.Controls.Add(Me.picOFF)
        Me.Controls.Add(Me.picON)
        Me.Controls.Add(Me.btnOffImage)
        Me.Controls.Add(Me.txtOFF)
        Me.Controls.Add(Me.btnOnImage)
        Me.Controls.Add(Me.txtON)
        Me.Controls.Add(Me.lblImage1)
        Me.Controls.Add(Me.btnCancelAdd)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.lblObject)
        Me.Controls.Add(Me.cboAddObject)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(900, 315)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(375, 315)
        Me.Name = "frmAddImageState"
        Me.Opacity = 0.95R
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Add an Image representing an Object's State"
        Me.TopMost = True
        CType(Me.picON, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picOFF, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picAlarm, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnOffImage As System.Windows.Forms.Button
    Friend WithEvents txtOFF As System.Windows.Forms.TextBox
    Friend WithEvents btnOnImage As System.Windows.Forms.Button
    Friend WithEvents txtON As System.Windows.Forms.TextBox
    Friend WithEvents lblImage1 As System.Windows.Forms.Label
    Friend WithEvents btnCancelAdd As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents lblObject As System.Windows.Forms.Label
    Friend WithEvents cboAddObject As System.Windows.Forms.ComboBox
    Friend WithEvents picON As System.Windows.Forms.PictureBox
    Friend WithEvents picOFF As System.Windows.Forms.PictureBox
    Friend WithEvents file1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnALARMImage As System.Windows.Forms.Button
    Friend WithEvents txtALARM As System.Windows.Forms.TextBox
    Friend WithEvents picAlarm As System.Windows.Forms.PictureBox
    Friend WithEvents lblImage3 As System.Windows.Forms.Label
    Friend WithEvents lblImage2 As System.Windows.Forms.Label
    Friend WithEvents lblON As System.Windows.Forms.Label
    Friend WithEvents lblOFF As System.Windows.Forms.Label
    Friend WithEvents lblALARM As System.Windows.Forms.Label
End Class
