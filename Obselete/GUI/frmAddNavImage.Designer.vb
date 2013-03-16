<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddNavImage
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAddNavImage))
        Me.btnImage = New System.Windows.Forms.Button()
        Me.txtNavImage = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtNavName = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.btnNavCancel = New System.Windows.Forms.Button()
        Me.btnNavAdd = New System.Windows.Forms.Button()
        Me.picImage = New System.Windows.Forms.PictureBox()
        Me.file1 = New System.Windows.Forms.OpenFileDialog()
        Me.cboTarget = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.picImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnImage
        '
        Me.btnImage.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnImage.Location = New System.Drawing.Point(241, 30)
        Me.btnImage.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnImage.Name = "btnImage"
        Me.btnImage.Size = New System.Drawing.Size(29, 24)
        Me.btnImage.TabIndex = 86
        Me.btnImage.Text = "..."
        Me.btnImage.UseVisualStyleBackColor = True
        '
        'txtNavImage
        '
        Me.txtNavImage.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtNavImage.Location = New System.Drawing.Point(43, 30)
        Me.txtNavImage.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtNavImage.Name = "txtNavImage"
        Me.txtNavImage.Size = New System.Drawing.Size(194, 20)
        Me.txtNavImage.TabIndex = 85
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(6, 36)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(36, 13)
        Me.Label15.TabIndex = 84
        Me.Label15.Text = "Image"
        '
        'txtNavName
        '
        Me.txtNavName.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtNavName.Location = New System.Drawing.Point(43, 6)
        Me.txtNavName.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtNavName.Name = "txtNavName"
        Me.txtNavName.Size = New System.Drawing.Size(224, 20)
        Me.txtNavName.TabIndex = 83
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(6, 6)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(35, 13)
        Me.Label16.TabIndex = 82
        Me.Label16.Text = "Name"
        '
        'btnNavCancel
        '
        Me.btnNavCancel.Location = New System.Drawing.Point(204, 186)
        Me.btnNavCancel.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnNavCancel.Name = "btnNavCancel"
        Me.btnNavCancel.Size = New System.Drawing.Size(74, 20)
        Me.btnNavCancel.TabIndex = 81
        Me.btnNavCancel.Text = "Cancel"
        Me.btnNavCancel.UseVisualStyleBackColor = True
        '
        'btnNavAdd
        '
        Me.btnNavAdd.Location = New System.Drawing.Point(18, 186)
        Me.btnNavAdd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnNavAdd.Name = "btnNavAdd"
        Me.btnNavAdd.Size = New System.Drawing.Size(74, 20)
        Me.btnNavAdd.TabIndex = 80
        Me.btnNavAdd.Text = "Add"
        Me.btnNavAdd.UseVisualStyleBackColor = True
        '
        'picImage
        '
        Me.picImage.Location = New System.Drawing.Point(78, 84)
        Me.picImage.Name = "picImage"
        Me.picImage.Size = New System.Drawing.Size(132, 96)
        Me.picImage.TabIndex = 87
        Me.picImage.TabStop = False
        '
        'file1
        '
        Me.file1.FileName = "file1"
        '
        'cboTarget
        '
        Me.cboTarget.FormattingEnabled = True
        Me.cboTarget.Location = New System.Drawing.Point(42, 54)
        Me.cboTarget.Name = "cboTarget"
        Me.cboTarget.Size = New System.Drawing.Size(192, 21)
        Me.cboTarget.TabIndex = 88
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 60)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(38, 13)
        Me.Label1.TabIndex = 89
        Me.Label1.Text = "Target"
        '
        'frmAddNavImage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(297, 214)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cboTarget)
        Me.Controls.Add(Me.picImage)
        Me.Controls.Add(Me.btnImage)
        Me.Controls.Add(Me.txtNavImage)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.txtNavName)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.btnNavCancel)
        Me.Controls.Add(Me.btnNavAdd)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmAddNavImage"
        Me.Opacity = 0.9R
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Add A Navigation Image"
        Me.TopMost = True
        CType(Me.picImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnImage As System.Windows.Forms.Button
    Friend WithEvents txtNavImage As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents txtNavName As System.Windows.Forms.TextBox
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents btnNavCancel As System.Windows.Forms.Button
    Friend WithEvents btnNavAdd As System.Windows.Forms.Button
    Friend WithEvents picImage As System.Windows.Forms.PictureBox
    Friend WithEvents file1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents cboTarget As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
