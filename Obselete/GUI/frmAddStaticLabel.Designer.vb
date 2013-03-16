<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddStaticLabel
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAddStaticLabel))
        Me.txtStaticName = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtStaticText = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.btnStaticLabelCancel = New System.Windows.Forms.Button()
        Me.btnStaticLabelAdd = New System.Windows.Forms.Button()
        Me.picForeColor = New System.Windows.Forms.PictureBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.picBackColor = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.picForeColor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picBackColor, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtStaticName
        '
        Me.txtStaticName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtStaticName.Location = New System.Drawing.Point(47, 4)
        Me.txtStaticName.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtStaticName.Name = "txtStaticName"
        Me.txtStaticName.Size = New System.Drawing.Size(269, 20)
        Me.txtStaticName.TabIndex = 76
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(9, 7)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(35, 13)
        Me.Label9.TabIndex = 75
        Me.Label9.Text = "Name"
        '
        'txtStaticText
        '
        Me.txtStaticText.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtStaticText.Location = New System.Drawing.Point(47, 30)
        Me.txtStaticText.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtStaticText.Name = "txtStaticText"
        Me.txtStaticText.Size = New System.Drawing.Size(269, 20)
        Me.txtStaticText.TabIndex = 74
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(15, 34)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(28, 13)
        Me.Label10.TabIndex = 73
        Me.Label10.Text = "Text"
        '
        'btnStaticLabelCancel
        '
        Me.btnStaticLabelCancel.Location = New System.Drawing.Point(234, 114)
        Me.btnStaticLabelCancel.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnStaticLabelCancel.Name = "btnStaticLabelCancel"
        Me.btnStaticLabelCancel.Size = New System.Drawing.Size(74, 20)
        Me.btnStaticLabelCancel.TabIndex = 72
        Me.btnStaticLabelCancel.Text = "Cancel"
        Me.btnStaticLabelCancel.UseVisualStyleBackColor = True
        '
        'btnStaticLabelAdd
        '
        Me.btnStaticLabelAdd.Location = New System.Drawing.Point(6, 114)
        Me.btnStaticLabelAdd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnStaticLabelAdd.Name = "btnStaticLabelAdd"
        Me.btnStaticLabelAdd.Size = New System.Drawing.Size(74, 20)
        Me.btnStaticLabelAdd.TabIndex = 71
        Me.btnStaticLabelAdd.Text = "Add"
        Me.btnStaticLabelAdd.UseVisualStyleBackColor = True
        '
        'picForeColor
        '
        Me.picForeColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picForeColor.Location = New System.Drawing.Point(156, 96)
        Me.picForeColor.Name = "picForeColor"
        Me.picForeColor.Size = New System.Drawing.Size(36, 24)
        Me.picForeColor.TabIndex = 80
        Me.picForeColor.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(96, 102)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(55, 13)
        Me.Label2.TabIndex = 79
        Me.Label2.Text = "Fore Color"
        '
        'picBackColor
        '
        Me.picBackColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picBackColor.Location = New System.Drawing.Point(156, 66)
        Me.picBackColor.Name = "picBackColor"
        Me.picBackColor.Size = New System.Drawing.Size(36, 24)
        Me.picBackColor.TabIndex = 78
        Me.picBackColor.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(96, 72)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(59, 13)
        Me.Label1.TabIndex = 77
        Me.Label1.Text = "Back Color"
        '
        'frmAddStaticLabel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(321, 143)
        Me.Controls.Add(Me.picForeColor)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.picBackColor)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtStaticName)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.txtStaticText)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.btnStaticLabelCancel)
        Me.Controls.Add(Me.btnStaticLabelAdd)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmAddStaticLabel"
        Me.Text = "frmAddStaticLabel"
        CType(Me.picForeColor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picBackColor, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtStaticName As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtStaticText As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents btnStaticLabelCancel As System.Windows.Forms.Button
    Friend WithEvents btnStaticLabelAdd As System.Windows.Forms.Button
    Friend WithEvents picForeColor As System.Windows.Forms.PictureBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents picBackColor As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
