<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddPropertyOption
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
        Me.dgvPropertyOptions = New System.Windows.Forms.DataGridView()
        Me.txbxPropOption = New System.Windows.Forms.TextBox()
        Me.btnUpdatePropOption = New System.Windows.Forms.Button()
        Me.btnAddPropOption = New System.Windows.Forms.Button()
        Me.btnDeletePropOption = New System.Windows.Forms.Button()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.dgvPropertyOptions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvPropertyOptions
        '
        Me.dgvPropertyOptions.AllowUserToAddRows = False
        Me.dgvPropertyOptions.AllowUserToDeleteRows = False
        Me.dgvPropertyOptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPropertyOptions.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1})
        Me.dgvPropertyOptions.Location = New System.Drawing.Point(12, 12)
        Me.dgvPropertyOptions.Name = "dgvPropertyOptions"
        Me.dgvPropertyOptions.ReadOnly = True
        Me.dgvPropertyOptions.RowHeadersVisible = False
        Me.dgvPropertyOptions.Size = New System.Drawing.Size(292, 197)
        Me.dgvPropertyOptions.TabIndex = 0
        '
        'txbxPropOption
        '
        Me.txbxPropOption.Location = New System.Drawing.Point(12, 215)
        Me.txbxPropOption.Name = "txbxPropOption"
        Me.txbxPropOption.Size = New System.Drawing.Size(292, 20)
        Me.txbxPropOption.TabIndex = 1
        '
        'btnUpdatePropOption
        '
        Me.btnUpdatePropOption.Location = New System.Drawing.Point(72, 241)
        Me.btnUpdatePropOption.Name = "btnUpdatePropOption"
        Me.btnUpdatePropOption.Size = New System.Drawing.Size(54, 30)
        Me.btnUpdatePropOption.TabIndex = 76
        Me.btnUpdatePropOption.Text = "Update"
        Me.btnUpdatePropOption.UseVisualStyleBackColor = True
        '
        'btnAddPropOption
        '
        Me.btnAddPropOption.Location = New System.Drawing.Point(12, 241)
        Me.btnAddPropOption.Name = "btnAddPropOption"
        Me.btnAddPropOption.Size = New System.Drawing.Size(54, 30)
        Me.btnAddPropOption.TabIndex = 75
        Me.btnAddPropOption.Text = "Add"
        Me.btnAddPropOption.UseVisualStyleBackColor = True
        '
        'btnDeletePropOption
        '
        Me.btnDeletePropOption.Location = New System.Drawing.Point(132, 241)
        Me.btnDeletePropOption.Name = "btnDeletePropOption"
        Me.btnDeletePropOption.Size = New System.Drawing.Size(60, 30)
        Me.btnDeletePropOption.TabIndex = 77
        Me.btnDeletePropOption.Text = "Delete"
        Me.btnDeletePropOption.UseVisualStyleBackColor = True
        '
        'Column1
        '
        Me.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Column1.DataPropertyName = "option_name"
        Me.Column1.HeaderText = "Option"
        Me.Column1.Name = "Column1"
        Me.Column1.ReadOnly = True
        Me.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        '
        'frmAddPropertyOption
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(316, 283)
        Me.Controls.Add(Me.btnDeletePropOption)
        Me.Controls.Add(Me.btnUpdatePropOption)
        Me.Controls.Add(Me.btnAddPropOption)
        Me.Controls.Add(Me.txbxPropOption)
        Me.Controls.Add(Me.dgvPropertyOptions)
        Me.Name = "frmAddPropertyOption"
        Me.Text = "Add Prperty Options"
        CType(Me.dgvPropertyOptions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dgvPropertyOptions As System.Windows.Forms.DataGridView
    Friend WithEvents txbxPropOption As System.Windows.Forms.TextBox
    Friend WithEvents btnUpdatePropOption As System.Windows.Forms.Button
    Friend WithEvents btnAddPropOption As System.Windows.Forms.Button
    Friend WithEvents btnDeletePropOption As System.Windows.Forms.Button
    Friend WithEvents Column1 As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
