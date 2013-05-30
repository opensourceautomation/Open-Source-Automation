<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPropertyList
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPropertyList))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblObject = New System.Windows.Forms.Label()
        Me.dvgPropertyList = New System.Windows.Forms.DataGridView()
        Me.item_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.item_label = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.txtValue = New System.Windows.Forms.TextBox()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblProperty = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtLabel = New System.Windows.Forms.TextBox()
        CType(Me.dvgPropertyList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Object:"
        '
        'lblObject
        '
        Me.lblObject.AutoSize = True
        Me.lblObject.Location = New System.Drawing.Point(108, 6)
        Me.lblObject.Name = "lblObject"
        Me.lblObject.Size = New System.Drawing.Size(38, 13)
        Me.lblObject.TabIndex = 1
        Me.lblObject.Text = "Object"
        '
        'dvgPropertyList
        '
        Me.dvgPropertyList.AllowUserToAddRows = False
        Me.dvgPropertyList.AllowUserToDeleteRows = False
        Me.dvgPropertyList.AllowUserToResizeRows = False
        Me.dvgPropertyList.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dvgPropertyList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dvgPropertyList.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.item_name, Me.item_label})
        Me.dvgPropertyList.Location = New System.Drawing.Point(9, 22)
        Me.dvgPropertyList.MultiSelect = False
        Me.dvgPropertyList.Name = "dvgPropertyList"
        Me.dvgPropertyList.ReadOnly = True
        Me.dvgPropertyList.RowHeadersVisible = False
        Me.dvgPropertyList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dvgPropertyList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dvgPropertyList.Size = New System.Drawing.Size(810, 294)
        Me.dvgPropertyList.TabIndex = 71
        '
        'item_name
        '
        Me.item_name.DataPropertyName = "item_name"
        Me.item_name.HeaderText = "Value"
        Me.item_name.Name = "item_name"
        Me.item_name.ReadOnly = True
        Me.item_name.Width = 400
        '
        'item_label
        '
        Me.item_label.DataPropertyName = "item_label"
        Me.item_label.HeaderText = "Label"
        Me.item_label.Name = "item_label"
        Me.item_label.ReadOnly = True
        Me.item_label.Width = 400
        '
        'txtValue
        '
        Me.txtValue.Location = New System.Drawing.Point(12, 330)
        Me.txtValue.Name = "txtValue"
        Me.txtValue.Size = New System.Drawing.Size(408, 20)
        Me.txtValue.TabIndex = 72
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(12, 360)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(54, 30)
        Me.btnAdd.TabIndex = 73
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseVisualStyleBackColor = False
        Me.btnAdd.Visible = False
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(72, 360)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(54, 30)
        Me.btnUpdate.TabIndex = 74
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        Me.btnUpdate.Visible = False
        '
        'btnDelete
        '
        Me.btnDelete.Location = New System.Drawing.Point(132, 360)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(60, 30)
        Me.btnDelete.TabIndex = 75
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Location = New System.Drawing.Point(756, 360)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(60, 24)
        Me.btnClose.TabIndex = 76
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'lblProperty
        '
        Me.lblProperty.AutoSize = True
        Me.lblProperty.Location = New System.Drawing.Point(396, 6)
        Me.lblProperty.Name = "lblProperty"
        Me.lblProperty.Size = New System.Drawing.Size(46, 13)
        Me.lblProperty.TabIndex = 78
        Me.lblProperty.Text = "Property"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(300, 6)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(49, 13)
        Me.Label3.TabIndex = 77
        Me.Label3.Text = "Property:"
        '
        'txtLabel
        '
        Me.txtLabel.Location = New System.Drawing.Point(426, 330)
        Me.txtLabel.Name = "txtLabel"
        Me.txtLabel.Size = New System.Drawing.Size(390, 20)
        Me.txtLabel.TabIndex = 79
        '
        'frmPropertyList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(821, 393)
        Me.Controls.Add(Me.txtLabel)
        Me.Controls.Add(Me.lblProperty)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.txtValue)
        Me.Controls.Add(Me.dvgPropertyList)
        Me.Controls.Add(Me.lblObject)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmPropertyList"
        Me.Text = "PropertyList"
        CType(Me.dvgPropertyList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblObject As System.Windows.Forms.Label
    Friend WithEvents dvgPropertyList As System.Windows.Forms.DataGridView
    Friend WithEvents txtValue As System.Windows.Forms.TextBox
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents lblProperty As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents item_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents item_label As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents txtLabel As System.Windows.Forms.TextBox
End Class
