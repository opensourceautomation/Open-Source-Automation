<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddObject
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAddObject))
        Me.Label21 = New System.Windows.Forms.Label()
        Me.cboContainerAdd = New System.Windows.Forms.ComboBox()
        Me.txtAddressAdd = New System.Windows.Forms.TextBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.txtNewObjectDesc = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.lblType = New System.Windows.Forms.Label()
        Me.cboNewObjectType = New System.Windows.Forms.ComboBox()
        Me.txtNewObjectName = New System.Windows.Forms.TextBox()
        Me.lblName = New System.Windows.Forms.Label()
        Me.btnObjectCancel = New System.Windows.Forms.Button()
        Me.btnObjectAdd = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.Location = New System.Drawing.Point(6, 156)
        Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label21.Size = New System.Drawing.Size(78, 20)
        Me.Label21.TabIndex = 94
        Me.Label21.Text = "Container"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboContainerAdd
        '
        Me.cboContainerAdd.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboContainerAdd.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboContainerAdd.FormattingEnabled = True
        Me.cboContainerAdd.Location = New System.Drawing.Point(90, 150)
        Me.cboContainerAdd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cboContainerAdd.Name = "cboContainerAdd"
        Me.cboContainerAdd.Size = New System.Drawing.Size(491, 28)
        Me.cboContainerAdd.TabIndex = 5
        '
        'txtAddressAdd
        '
        Me.txtAddressAdd.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtAddressAdd.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAddressAdd.Location = New System.Drawing.Point(90, 114)
        Me.txtAddressAdd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtAddressAdd.Name = "txtAddressAdd"
        Me.txtAddressAdd.Size = New System.Drawing.Size(487, 26)
        Me.txtAddressAdd.TabIndex = 4
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(12, 120)
        Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label20.Size = New System.Drawing.Size(68, 20)
        Me.Label20.TabIndex = 91
        Me.Label20.Text = "Address"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtNewObjectDesc
        '
        Me.txtNewObjectDesc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtNewObjectDesc.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtNewObjectDesc.Location = New System.Drawing.Point(90, 42)
        Me.txtNewObjectDesc.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtNewObjectDesc.Name = "txtNewObjectDesc"
        Me.txtNewObjectDesc.Size = New System.Drawing.Size(487, 26)
        Me.txtNewObjectDesc.TabIndex = 2
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(36, 48)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label19.Size = New System.Drawing.Size(46, 20)
        Me.Label19.TabIndex = 89
        Me.Label19.Text = "Desc"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblType.Location = New System.Drawing.Point(36, 84)
        Me.lblType.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblType.Name = "lblType"
        Me.lblType.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblType.Size = New System.Drawing.Size(43, 20)
        Me.lblType.TabIndex = 88
        Me.lblType.Text = "Type"
        Me.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboNewObjectType
        '
        Me.cboNewObjectType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboNewObjectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboNewObjectType.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboNewObjectType.FormattingEnabled = True
        Me.cboNewObjectType.Location = New System.Drawing.Point(90, 78)
        Me.cboNewObjectType.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cboNewObjectType.Name = "cboNewObjectType"
        Me.cboNewObjectType.Size = New System.Drawing.Size(489, 28)
        Me.cboNewObjectType.TabIndex = 3
        '
        'txtNewObjectName
        '
        Me.txtNewObjectName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtNewObjectName.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtNewObjectName.Location = New System.Drawing.Point(90, 6)
        Me.txtNewObjectName.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtNewObjectName.Name = "txtNewObjectName"
        Me.txtNewObjectName.Size = New System.Drawing.Size(487, 26)
        Me.txtNewObjectName.TabIndex = 1
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblName.Location = New System.Drawing.Point(30, 12)
        Me.lblName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblName.Name = "lblName"
        Me.lblName.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblName.Size = New System.Drawing.Size(51, 20)
        Me.lblName.TabIndex = 85
        Me.lblName.Text = "Name"
        Me.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnObjectCancel
        '
        Me.btnObjectCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnObjectCancel.ForeColor = System.Drawing.Color.Black
        Me.btnObjectCancel.Location = New System.Drawing.Point(306, 186)
        Me.btnObjectCancel.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnObjectCancel.Name = "btnObjectCancel"
        Me.btnObjectCancel.Size = New System.Drawing.Size(80, 30)
        Me.btnObjectCancel.TabIndex = 7
        Me.btnObjectCancel.Text = "Cancel"
        Me.btnObjectCancel.UseVisualStyleBackColor = True
        '
        'btnObjectAdd
        '
        Me.btnObjectAdd.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnObjectAdd.ForeColor = System.Drawing.Color.Black
        Me.btnObjectAdd.Location = New System.Drawing.Point(210, 186)
        Me.btnObjectAdd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.btnObjectAdd.Name = "btnObjectAdd"
        Me.btnObjectAdd.Size = New System.Drawing.Size(80, 30)
        Me.btnObjectAdd.TabIndex = 6
        Me.btnObjectAdd.Text = "Add"
        Me.btnObjectAdd.UseVisualStyleBackColor = True
        '
        'frmAddObject
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 222)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.cboContainerAdd)
        Me.Controls.Add(Me.txtAddressAdd)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.txtNewObjectDesc)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.lblType)
        Me.Controls.Add(Me.cboNewObjectType)
        Me.Controls.Add(Me.txtNewObjectName)
        Me.Controls.Add(Me.lblName)
        Me.Controls.Add(Me.btnObjectCancel)
        Me.Controls.Add(Me.btnObjectAdd)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximumSize = New System.Drawing.Size(600, 260)
        Me.MinimumSize = New System.Drawing.Size(325, 175)
        Me.Name = "frmAddObject"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Add a New Object"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents cboContainerAdd As System.Windows.Forms.ComboBox
    Friend WithEvents txtAddressAdd As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents txtNewObjectDesc As System.Windows.Forms.TextBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents lblType As System.Windows.Forms.Label
    Friend WithEvents cboNewObjectType As System.Windows.Forms.ComboBox
    Friend WithEvents txtNewObjectName As System.Windows.Forms.TextBox
    Friend WithEvents lblName As System.Windows.Forms.Label
    Friend WithEvents btnObjectCancel As System.Windows.Forms.Button
    Friend WithEvents btnObjectAdd As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
End Class
