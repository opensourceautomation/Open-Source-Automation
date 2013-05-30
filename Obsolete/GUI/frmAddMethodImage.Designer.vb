<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddMethodImage
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAddMethodImage))
        Me.btnOnImage = New System.Windows.Forms.Button()
        Me.txtMethodImage = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.cboMethods = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtPararm2 = New System.Windows.Forms.TextBox()
        Me.txtPararm1 = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.btnMethodImageCancel = New System.Windows.Forms.Button()
        Me.btnMethodImageAdd = New System.Windows.Forms.Button()
        Me.cboObjects = New System.Windows.Forms.ComboBox()
        Me.picON = New System.Windows.Forms.PictureBox()
        Me.file1 = New System.Windows.Forms.OpenFileDialog()
        CType(Me.picON, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnOnImage
        '
        Me.btnOnImage.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOnImage.Location = New System.Drawing.Point(316, 102)
        Me.btnOnImage.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnOnImage.Name = "btnOnImage"
        Me.btnOnImage.Size = New System.Drawing.Size(32, 17)
        Me.btnOnImage.TabIndex = 89
        Me.btnOnImage.Text = "..."
        Me.btnOnImage.UseVisualStyleBackColor = True
        '
        'txtMethodImage
        '
        Me.txtMethodImage.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtMethodImage.Location = New System.Drawing.Point(66, 102)
        Me.txtMethodImage.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtMethodImage.Name = "txtMethodImage"
        Me.txtMethodImage.Size = New System.Drawing.Size(246, 20)
        Me.txtMethodImage.TabIndex = 88
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(24, 108)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(36, 13)
        Me.Label13.TabIndex = 87
        Me.Label13.Text = "Image"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(12, 36)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(43, 13)
        Me.Label12.TabIndex = 86
        Me.Label12.Text = "Method"
        '
        'cboMethods
        '
        Me.cboMethods.FormattingEnabled = True
        Me.cboMethods.Location = New System.Drawing.Point(66, 30)
        Me.cboMethods.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cboMethods.Name = "cboMethods"
        Me.cboMethods.Size = New System.Drawing.Size(343, 21)
        Me.cboMethods.TabIndex = 85
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(18, 12)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(38, 13)
        Me.Label8.TabIndex = 84
        Me.Label8.Text = "Object"
        '
        'txtPararm2
        '
        Me.txtPararm2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPararm2.Location = New System.Drawing.Point(66, 78)
        Me.txtPararm2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtPararm2.Name = "txtPararm2"
        Me.txtPararm2.Size = New System.Drawing.Size(246, 20)
        Me.txtPararm2.TabIndex = 83
        '
        'txtPararm1
        '
        Me.txtPararm1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPararm1.Location = New System.Drawing.Point(66, 54)
        Me.txtPararm1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtPararm1.Name = "txtPararm1"
        Me.txtPararm1.Size = New System.Drawing.Size(246, 20)
        Me.txtPararm1.TabIndex = 82
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 84)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(46, 13)
        Me.Label4.TabIndex = 81
        Me.Label4.Text = "Param 2"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(12, 60)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(46, 13)
        Me.Label7.TabIndex = 80
        Me.Label7.Text = "Param 1"
        '
        'btnMethodImageCancel
        '
        Me.btnMethodImageCancel.Location = New System.Drawing.Point(288, 234)
        Me.btnMethodImageCancel.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnMethodImageCancel.Name = "btnMethodImageCancel"
        Me.btnMethodImageCancel.Size = New System.Drawing.Size(74, 20)
        Me.btnMethodImageCancel.TabIndex = 79
        Me.btnMethodImageCancel.Text = "Cancel"
        Me.btnMethodImageCancel.UseVisualStyleBackColor = True
        '
        'btnMethodImageAdd
        '
        Me.btnMethodImageAdd.Location = New System.Drawing.Point(12, 234)
        Me.btnMethodImageAdd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnMethodImageAdd.Name = "btnMethodImageAdd"
        Me.btnMethodImageAdd.Size = New System.Drawing.Size(74, 20)
        Me.btnMethodImageAdd.TabIndex = 78
        Me.btnMethodImageAdd.Text = "Add"
        Me.btnMethodImageAdd.UseVisualStyleBackColor = True
        '
        'cboObjects
        '
        Me.cboObjects.FormattingEnabled = True
        Me.cboObjects.Location = New System.Drawing.Point(66, 6)
        Me.cboObjects.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cboObjects.Name = "cboObjects"
        Me.cboObjects.Size = New System.Drawing.Size(343, 21)
        Me.cboObjects.TabIndex = 77
        '
        'picON
        '
        Me.picON.Location = New System.Drawing.Point(102, 126)
        Me.picON.Name = "picON"
        Me.picON.Size = New System.Drawing.Size(168, 102)
        Me.picON.TabIndex = 90
        Me.picON.TabStop = False
        '
        'file1
        '
        Me.file1.FileName = "file1"
        '
        'frmAddMethodImage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(414, 262)
        Me.Controls.Add(Me.picON)
        Me.Controls.Add(Me.btnOnImage)
        Me.Controls.Add(Me.txtMethodImage)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.cboMethods)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.txtPararm2)
        Me.Controls.Add(Me.txtPararm1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.btnMethodImageCancel)
        Me.Controls.Add(Me.btnMethodImageAdd)
        Me.Controls.Add(Me.cboObjects)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmAddMethodImage"
        Me.Opacity = 0.9R
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Add an Image representing an Object's Method"
        Me.TopMost = True
        CType(Me.picON, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnOnImage As System.Windows.Forms.Button
    Friend WithEvents txtMethodImage As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents cboMethods As System.Windows.Forms.ComboBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtPararm2 As System.Windows.Forms.TextBox
    Friend WithEvents txtPararm1 As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents btnMethodImageCancel As System.Windows.Forms.Button
    Friend WithEvents btnMethodImageAdd As System.Windows.Forms.Button
    Friend WithEvents cboObjects As System.Windows.Forms.ComboBox
    Friend WithEvents picON As System.Windows.Forms.PictureBox
    Friend WithEvents file1 As System.Windows.Forms.OpenFileDialog
End Class
