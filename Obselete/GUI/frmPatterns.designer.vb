<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPatterns
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPatterns))
        Me.dgvPatterns = New System.Windows.Forms.DataGridView()
        Me.pattern = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dgvItems = New System.Windows.Forms.DataGridView()
        Me.match = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnMatchDelete = New System.Windows.Forms.Button()
        Me.btnMatchAdd = New System.Windows.Forms.Button()
        Me.btnMatchUpdate = New System.Windows.Forms.Button()
        Me.txtMatch = New System.Windows.Forms.TextBox()
        Me.txtPattern = New System.Windows.Forms.TextBox()
        CType(Me.dgvPatterns, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvItems, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvPatterns
        '
        Me.dgvPatterns.AllowUserToAddRows = False
        Me.dgvPatterns.AllowUserToDeleteRows = False
        Me.dgvPatterns.AllowUserToResizeRows = False
        Me.dgvPatterns.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvPatterns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPatterns.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.pattern})
        Me.dgvPatterns.Location = New System.Drawing.Point(0, 0)
        Me.dgvPatterns.MultiSelect = False
        Me.dgvPatterns.Name = "dgvPatterns"
        Me.dgvPatterns.ReadOnly = True
        Me.dgvPatterns.RowHeadersVisible = False
        Me.dgvPatterns.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvPatterns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvPatterns.Size = New System.Drawing.Size(312, 426)
        Me.dgvPatterns.TabIndex = 30
        '
        'pattern
        '
        Me.pattern.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.pattern.DataPropertyName = "pattern"
        Me.pattern.HeaderText = "Pattern"
        Me.pattern.Name = "pattern"
        Me.pattern.ReadOnly = True
        '
        'dgvItems
        '
        Me.dgvItems.AllowUserToAddRows = False
        Me.dgvItems.AllowUserToDeleteRows = False
        Me.dgvItems.AllowUserToResizeRows = False
        Me.dgvItems.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvItems.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.match})
        Me.dgvItems.Location = New System.Drawing.Point(330, 24)
        Me.dgvItems.MultiSelect = False
        Me.dgvItems.Name = "dgvItems"
        Me.dgvItems.ReadOnly = True
        Me.dgvItems.RowHeadersVisible = False
        Me.dgvItems.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvItems.Size = New System.Drawing.Size(384, 402)
        Me.dgvItems.TabIndex = 31
        '
        'match
        '
        Me.match.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.match.DataPropertyName = "match"
        Me.match.HeaderText = "Match"
        Me.match.Name = "match"
        Me.match.ReadOnly = True
        '
        'btnDelete
        '
        Me.btnDelete.Location = New System.Drawing.Point(147, 456)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(69, 22)
        Me.btnDelete.TabIndex = 55
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        Me.btnDelete.Visible = False
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(21, 456)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(60, 22)
        Me.btnAdd.TabIndex = 54
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseVisualStyleBackColor = True
        Me.btnAdd.Visible = False
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(84, 456)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(60, 22)
        Me.btnUpdate.TabIndex = 53
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        Me.btnUpdate.Visible = False
        '
        'btnMatchDelete
        '
        Me.btnMatchDelete.Location = New System.Drawing.Point(456, 456)
        Me.btnMatchDelete.Name = "btnMatchDelete"
        Me.btnMatchDelete.Size = New System.Drawing.Size(69, 22)
        Me.btnMatchDelete.TabIndex = 58
        Me.btnMatchDelete.Text = "Delete"
        Me.btnMatchDelete.UseVisualStyleBackColor = True
        Me.btnMatchDelete.Visible = False
        '
        'btnMatchAdd
        '
        Me.btnMatchAdd.Location = New System.Drawing.Point(330, 456)
        Me.btnMatchAdd.Name = "btnMatchAdd"
        Me.btnMatchAdd.Size = New System.Drawing.Size(60, 22)
        Me.btnMatchAdd.TabIndex = 57
        Me.btnMatchAdd.Text = "Add"
        Me.btnMatchAdd.UseVisualStyleBackColor = True
        Me.btnMatchAdd.Visible = False
        '
        'btnMatchUpdate
        '
        Me.btnMatchUpdate.Location = New System.Drawing.Point(393, 456)
        Me.btnMatchUpdate.Name = "btnMatchUpdate"
        Me.btnMatchUpdate.Size = New System.Drawing.Size(60, 22)
        Me.btnMatchUpdate.TabIndex = 56
        Me.btnMatchUpdate.Text = "Update"
        Me.btnMatchUpdate.UseVisualStyleBackColor = True
        Me.btnMatchUpdate.Visible = False
        '
        'txtMatch
        '
        Me.txtMatch.Location = New System.Drawing.Point(330, 432)
        Me.txtMatch.Name = "txtMatch"
        Me.txtMatch.Size = New System.Drawing.Size(384, 20)
        Me.txtMatch.TabIndex = 59
        '
        'txtPattern
        '
        Me.txtPattern.Location = New System.Drawing.Point(0, 432)
        Me.txtPattern.Name = "txtPattern"
        Me.txtPattern.Size = New System.Drawing.Size(312, 20)
        Me.txtPattern.TabIndex = 60
        '
        'frmPatterns
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(724, 504)
        Me.Controls.Add(Me.txtPattern)
        Me.Controls.Add(Me.txtMatch)
        Me.Controls.Add(Me.btnMatchDelete)
        Me.Controls.Add(Me.btnMatchAdd)
        Me.Controls.Add(Me.btnMatchUpdate)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.dgvItems)
        Me.Controls.Add(Me.dgvPatterns)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmPatterns"
        Me.Text = "Patterns"
        CType(Me.dgvPatterns, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvItems, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dgvPatterns As System.Windows.Forms.DataGridView
    Friend WithEvents dgvItems As System.Windows.Forms.DataGridView
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnMatchDelete As System.Windows.Forms.Button
    Friend WithEvents btnMatchAdd As System.Windows.Forms.Button
    Friend WithEvents btnMatchUpdate As System.Windows.Forms.Button
    Friend WithEvents txtMatch As System.Windows.Forms.TextBox
    Friend WithEvents txtPattern As System.Windows.Forms.TextBox
    Friend WithEvents pattern As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents match As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
