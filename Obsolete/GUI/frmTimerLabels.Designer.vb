<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTimerLabels
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
        Me.txtFontSize = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtFont = New System.Windows.Forms.TextBox()
        Me.picForeColor = New System.Windows.Forms.PictureBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.picBackColor = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.cboObjectList1 = New System.Windows.Forms.ComboBox()
        Me.radProperty = New System.Windows.Forms.RadioButton()
        Me.radState = New System.Windows.Forms.RadioButton()
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
        CType(Me.picForeColor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picBackColor, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtFontSize
        '
        Me.txtFontSize.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFontSize.Location = New System.Drawing.Point(448, 133)
        Me.txtFontSize.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtFontSize.Name = "txtFontSize"
        Me.txtFontSize.Size = New System.Drawing.Size(63, 30)
        Me.txtFontSize.TabIndex = 44
        Me.txtFontSize.Text = "8.25"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(376, 140)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(51, 25)
        Me.Label8.TabIndex = 43
        Me.Label8.Text = "Size"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(80, 140)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(51, 25)
        Me.Label7.TabIndex = 42
        Me.Label7.Text = "Font"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtFont
        '
        Me.txtFont.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFont.Location = New System.Drawing.Point(144, 133)
        Me.txtFont.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtFont.Name = "txtFont"
        Me.txtFont.Size = New System.Drawing.Size(215, 30)
        Me.txtFont.TabIndex = 41
        Me.txtFont.Text = "Microsoft Sans Serif"
        '
        'picForeColor
        '
        Me.picForeColor.BackColor = System.Drawing.Color.Black
        Me.picForeColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picForeColor.Location = New System.Drawing.Point(384, 185)
        Me.picForeColor.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.picForeColor.Name = "picForeColor"
        Me.picForeColor.Size = New System.Drawing.Size(47, 29)
        Me.picForeColor.TabIndex = 36
        Me.picForeColor.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(264, 185)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(104, 25)
        Me.Label2.TabIndex = 35
        Me.Label2.Text = "Fore Color"
        '
        'picBackColor
        '
        Me.picBackColor.BackColor = System.Drawing.Color.White
        Me.picBackColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picBackColor.Location = New System.Drawing.Point(384, 222)
        Me.picBackColor.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.picBackColor.Name = "picBackColor"
        Me.picBackColor.Size = New System.Drawing.Size(47, 29)
        Me.picBackColor.TabIndex = 34
        Me.picBackColor.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(264, 222)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(108, 25)
        Me.Label1.TabIndex = 33
        Me.Label1.Text = "Back Color"
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(16, 22)
        Me.Label6.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 30)
        Me.Label6.TabIndex = 32
        Me.Label6.Text = "Object"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnCancel
        '
        Me.btnCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(560, 214)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(99, 37)
        Me.btnCancel.TabIndex = 29
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAdd.Location = New System.Drawing.Point(40, 214)
        Me.btnAdd.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(99, 37)
        Me.btnAdd.TabIndex = 28
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'cboObjectList1
        '
        Me.cboObjectList1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboObjectList1.FormattingEnabled = True
        Me.cboObjectList1.Location = New System.Drawing.Point(112, 15)
        Me.cboObjectList1.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.cboObjectList1.Name = "cboObjectList1"
        Me.cboObjectList1.Size = New System.Drawing.Size(511, 33)
        Me.cboObjectList1.TabIndex = 27
        '
        'radProperty
        '
        Me.radProperty.AutoSize = True
        Me.radProperty.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.radProperty.Location = New System.Drawing.Point(328, 59)
        Me.radProperty.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.radProperty.Name = "radProperty"
        Me.radProperty.Size = New System.Drawing.Size(114, 29)
        Me.radProperty.TabIndex = 45
        Me.radProperty.Text = "Off Timer"
        Me.radProperty.UseVisualStyleBackColor = True
        '
        'radState
        '
        Me.radState.AutoSize = True
        Me.radState.Checked = True
        Me.radState.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.radState.Location = New System.Drawing.Point(144, 59)
        Me.radState.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.radState.Name = "radState"
        Me.radState.Size = New System.Drawing.Size(149, 29)
        Me.radState.TabIndex = 46
        Me.radState.TabStop = True
        Me.radState.Text = "Time In State"
        Me.radState.UseVisualStyleBackColor = True
        '
        'frmTimerLabels
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(675, 272)
        Me.Controls.Add(Me.radState)
        Me.Controls.Add(Me.radProperty)
        Me.Controls.Add(Me.txtFontSize)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtFont)
        Me.Controls.Add(Me.picForeColor)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.picBackColor)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.cboObjectList1)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "frmTimerLabels"
        Me.Text = "Add Timer Label"
        CType(Me.picForeColor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picBackColor, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtFontSize As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtFont As System.Windows.Forms.TextBox
    Friend WithEvents picForeColor As System.Windows.Forms.PictureBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents picBackColor As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents cboObjectList1 As System.Windows.Forms.ComboBox
    Friend WithEvents radProperty As System.Windows.Forms.RadioButton
    Friend WithEvents radState As System.Windows.Forms.RadioButton
    Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
End Class
