<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddPropertyLabel
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAddPropertyLabel))
        Me.Label6 = New System.Windows.Forms.Label()
        Me.cboProperty = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.btnPropertyLabelCancel = New System.Windows.Forms.Button()
        Me.btnPropertyLabelAdd = New System.Windows.Forms.Button()
        Me.cboObjectList1 = New System.Windows.Forms.ComboBox()
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
        Me.lblBackColor = New System.Windows.Forms.Label()
        Me.lblForeColor = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtPrefix = New System.Windows.Forms.TextBox()
        Me.txtSuffix = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtFont = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtFontSize = New System.Windows.Forms.TextBox()
        Me.cboForeColors = New System.Windows.Forms.ComboBox()
        Me.cboBackColors = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(6, 12)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(72, 24)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Object"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboProperty
        '
        Me.cboProperty.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboProperty.FormattingEnabled = True
        Me.cboProperty.Location = New System.Drawing.Point(84, 42)
        Me.cboProperty.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cboProperty.Name = "cboProperty"
        Me.cboProperty.Size = New System.Drawing.Size(384, 28)
        Me.cboProperty.TabIndex = 13
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(6, 48)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(72, 24)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "Property"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnPropertyLabelCancel
        '
        Me.btnPropertyLabelCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPropertyLabelCancel.Location = New System.Drawing.Point(396, 186)
        Me.btnPropertyLabelCancel.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnPropertyLabelCancel.Name = "btnPropertyLabelCancel"
        Me.btnPropertyLabelCancel.Size = New System.Drawing.Size(74, 30)
        Me.btnPropertyLabelCancel.TabIndex = 11
        Me.btnPropertyLabelCancel.Text = "Cancel"
        Me.btnPropertyLabelCancel.UseVisualStyleBackColor = True
        '
        'btnPropertyLabelAdd
        '
        Me.btnPropertyLabelAdd.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPropertyLabelAdd.Location = New System.Drawing.Point(6, 186)
        Me.btnPropertyLabelAdd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnPropertyLabelAdd.Name = "btnPropertyLabelAdd"
        Me.btnPropertyLabelAdd.Size = New System.Drawing.Size(74, 30)
        Me.btnPropertyLabelAdd.TabIndex = 10
        Me.btnPropertyLabelAdd.Text = "Add"
        Me.btnPropertyLabelAdd.UseVisualStyleBackColor = True
        '
        'cboObjectList1
        '
        Me.cboObjectList1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboObjectList1.FormattingEnabled = True
        Me.cboObjectList1.Location = New System.Drawing.Point(84, 6)
        Me.cboObjectList1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cboObjectList1.Name = "cboObjectList1"
        Me.cboObjectList1.Size = New System.Drawing.Size(384, 28)
        Me.cboObjectList1.TabIndex = 9
        '
        'lblBackColor
        '
        Me.lblBackColor.AutoSize = True
        Me.lblBackColor.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBackColor.Location = New System.Drawing.Point(100, 202)
        Me.lblBackColor.Name = "lblBackColor"
        Me.lblBackColor.Size = New System.Drawing.Size(86, 20)
        Me.lblBackColor.TabIndex = 15
        Me.lblBackColor.Text = "Back Color"
        '
        'lblForeColor
        '
        Me.lblForeColor.AutoSize = True
        Me.lblForeColor.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblForeColor.Location = New System.Drawing.Point(102, 162)
        Me.lblForeColor.Name = "lblForeColor"
        Me.lblForeColor.Size = New System.Drawing.Size(83, 20)
        Me.lblForeColor.TabIndex = 17
        Me.lblForeColor.Text = "Fore Color"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(30, 84)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(48, 20)
        Me.Label3.TabIndex = 19
        Me.Label3.Text = "Prefix"
        '
        'txtPrefix
        '
        Me.txtPrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPrefix.Location = New System.Drawing.Point(84, 78)
        Me.txtPrefix.Name = "txtPrefix"
        Me.txtPrefix.Size = New System.Drawing.Size(162, 26)
        Me.txtPrefix.TabIndex = 20
        '
        'txtSuffix
        '
        Me.txtSuffix.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSuffix.Location = New System.Drawing.Point(312, 78)
        Me.txtSuffix.Name = "txtSuffix"
        Me.txtSuffix.Size = New System.Drawing.Size(156, 26)
        Me.txtSuffix.TabIndex = 22
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(252, 84)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(49, 20)
        Me.Label4.TabIndex = 21
        Me.Label4.Text = "Suffix"
        '
        'txtFont
        '
        Me.txtFont.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFont.Location = New System.Drawing.Point(84, 120)
        Me.txtFont.Name = "txtFont"
        Me.txtFont.Size = New System.Drawing.Size(162, 26)
        Me.txtFont.TabIndex = 23
        Me.txtFont.Text = "Microsoft Sans Serif"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(36, 126)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(42, 20)
        Me.Label7.TabIndex = 24
        Me.Label7.Text = "Font"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(258, 126)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(40, 20)
        Me.Label8.TabIndex = 25
        Me.Label8.Text = "Size"
        '
        'txtFontSize
        '
        Me.txtFontSize.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFontSize.Location = New System.Drawing.Point(312, 120)
        Me.txtFontSize.Name = "txtFontSize"
        Me.txtFontSize.Size = New System.Drawing.Size(48, 26)
        Me.txtFontSize.TabIndex = 26
        Me.txtFontSize.Text = "8.25"
        '
        'cboForeColors
        '
        Me.cboForeColors.FormattingEnabled = True
        Me.cboForeColors.Location = New System.Drawing.Point(209, 161)
        Me.cboForeColors.Name = "cboForeColors"
        Me.cboForeColors.Size = New System.Drawing.Size(164, 21)
        Me.cboForeColors.TabIndex = 27
        '
        'cboBackColors
        '
        Me.cboBackColors.FormattingEnabled = True
        Me.cboBackColors.Location = New System.Drawing.Point(209, 201)
        Me.cboBackColors.Name = "cboBackColors"
        Me.cboBackColors.Size = New System.Drawing.Size(164, 21)
        Me.cboBackColors.TabIndex = 28
        '
        'frmAddPropertyLabel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(478, 237)
        Me.Controls.Add(Me.cboBackColors)
        Me.Controls.Add(Me.cboForeColors)
        Me.Controls.Add(Me.txtFontSize)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtFont)
        Me.Controls.Add(Me.txtSuffix)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtPrefix)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lblForeColor)
        Me.Controls.Add(Me.lblBackColor)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.cboProperty)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.btnPropertyLabelCancel)
        Me.Controls.Add(Me.btnPropertyLabelAdd)
        Me.Controls.Add(Me.cboObjectList1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAddPropertyLabel"
        Me.Opacity = 0.9R
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Add Property Label Control"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents cboProperty As System.Windows.Forms.ComboBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents btnPropertyLabelCancel As System.Windows.Forms.Button
    Friend WithEvents btnPropertyLabelAdd As System.Windows.Forms.Button
    Friend WithEvents cboObjectList1 As System.Windows.Forms.ComboBox
    Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
    Friend WithEvents lblBackColor As System.Windows.Forms.Label
    Friend WithEvents lblForeColor As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtPrefix As System.Windows.Forms.TextBox
    Friend WithEvents txtSuffix As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtFont As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtFontSize As System.Windows.Forms.TextBox
    Friend WithEvents cboForeColors As System.Windows.Forms.ComboBox
    Friend WithEvents cboBackColors As System.Windows.Forms.ComboBox
End Class
