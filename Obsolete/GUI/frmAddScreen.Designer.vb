<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddScreen
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAddScreen))
        Me.btnFile4 = New System.Windows.Forms.Button()
        Me.txtScreenImage = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtNewScreenName = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.btnNewScreenCancel = New System.Windows.Forms.Button()
        Me.btnNewScreenAdd = New System.Windows.Forms.Button()
        Me.picScreen = New System.Windows.Forms.PictureBox()
        Me.file1 = New System.Windows.Forms.OpenFileDialog()
        CType(Me.picScreen, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnFile4
        '
        Me.btnFile4.Location = New System.Drawing.Point(238, 24)
        Me.btnFile4.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnFile4.Name = "btnFile4"
        Me.btnFile4.Size = New System.Drawing.Size(32, 20)
        Me.btnFile4.TabIndex = 86
        Me.btnFile4.Text = "..."
        Me.btnFile4.UseVisualStyleBackColor = True
        '
        'txtScreenImage
        '
        Me.txtScreenImage.Location = New System.Drawing.Point(43, 24)
        Me.txtScreenImage.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtScreenImage.Name = "txtScreenImage"
        Me.txtScreenImage.Size = New System.Drawing.Size(194, 20)
        Me.txtScreenImage.TabIndex = 85
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(3, 28)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(36, 13)
        Me.Label14.TabIndex = 84
        Me.Label14.Text = "Image"
        '
        'txtNewScreenName
        '
        Me.txtNewScreenName.Location = New System.Drawing.Point(43, 6)
        Me.txtNewScreenName.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtNewScreenName.Name = "txtNewScreenName"
        Me.txtNewScreenName.Size = New System.Drawing.Size(224, 20)
        Me.txtNewScreenName.TabIndex = 83
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(6, 6)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(35, 13)
        Me.Label11.TabIndex = 82
        Me.Label11.Text = "Name"
        '
        'btnNewScreenCancel
        '
        Me.btnNewScreenCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNewScreenCancel.Location = New System.Drawing.Point(579, 482)
        Me.btnNewScreenCancel.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnNewScreenCancel.Name = "btnNewScreenCancel"
        Me.btnNewScreenCancel.Size = New System.Drawing.Size(74, 20)
        Me.btnNewScreenCancel.TabIndex = 81
        Me.btnNewScreenCancel.Text = "Cancel"
        Me.btnNewScreenCancel.UseVisualStyleBackColor = True
        '
        'btnNewScreenAdd
        '
        Me.btnNewScreenAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnNewScreenAdd.Location = New System.Drawing.Point(6, 482)
        Me.btnNewScreenAdd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnNewScreenAdd.Name = "btnNewScreenAdd"
        Me.btnNewScreenAdd.Size = New System.Drawing.Size(74, 20)
        Me.btnNewScreenAdd.TabIndex = 80
        Me.btnNewScreenAdd.Text = "Add"
        Me.btnNewScreenAdd.UseVisualStyleBackColor = True
        '
        'picScreen
        '
        Me.picScreen.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.picScreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picScreen.Location = New System.Drawing.Point(6, 48)
        Me.picScreen.Name = "picScreen"
        Me.picScreen.Size = New System.Drawing.Size(649, 431)
        Me.picScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picScreen.TabIndex = 87
        Me.picScreen.TabStop = False
        '
        'file1
        '
        Me.file1.FileName = "file1"
        '
        'frmAddScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(657, 507)
        Me.Controls.Add(Me.picScreen)
        Me.Controls.Add(Me.btnFile4)
        Me.Controls.Add(Me.txtScreenImage)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.txtNewScreenName)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.btnNewScreenCancel)
        Me.Controls.Add(Me.btnNewScreenAdd)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmAddScreen"
        Me.Opacity = 0.9R
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Add New Screen"
        Me.TopMost = True
        CType(Me.picScreen, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnFile4 As System.Windows.Forms.Button
    Friend WithEvents txtScreenImage As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents txtNewScreenName As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents btnNewScreenCancel As System.Windows.Forms.Button
    Friend WithEvents btnNewScreenAdd As System.Windows.Forms.Button
    Friend WithEvents picScreen As System.Windows.Forms.PictureBox
    Friend WithEvents file1 As System.Windows.Forms.OpenFileDialog
End Class
