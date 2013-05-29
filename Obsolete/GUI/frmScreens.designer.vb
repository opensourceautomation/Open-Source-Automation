<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmScreens
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmScreens))
        Me.dgvScreens = New System.Windows.Forms.DataGridView()
        Me.screen_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dgvControls = New System.Windows.Forms.DataGridView()
        Me.object_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.control_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.butScreenControlDelete = New System.Windows.Forms.Button()
        Me.butScreenControlUpdate = New System.Windows.Forms.Button()
        Me.butScreenControlAdd = New System.Windows.Forms.Button()
        Me.comObjects = New System.Windows.Forms.ComboBox()
        Me.comControls = New System.Windows.Forms.ComboBox()
        CType(Me.dgvScreens, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvControls, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvScreens
        '
        Me.dgvScreens.AllowUserToAddRows = False
        Me.dgvScreens.AllowUserToDeleteRows = False
        Me.dgvScreens.AllowUserToResizeRows = False
        Me.dgvScreens.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvScreens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvScreens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvScreens.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.screen_name})
        Me.dgvScreens.Location = New System.Drawing.Point(2, 3)
        Me.dgvScreens.MultiSelect = False
        Me.dgvScreens.Name = "dgvScreens"
        Me.dgvScreens.ReadOnly = True
        Me.dgvScreens.RowHeadersVisible = False
        Me.dgvScreens.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvScreens.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvScreens.Size = New System.Drawing.Size(194, 423)
        Me.dgvScreens.TabIndex = 41
        '
        'screen_name
        '
        Me.screen_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.screen_name.DataPropertyName = "object_name"
        Me.screen_name.HeaderText = "Screen"
        Me.screen_name.MinimumWidth = 150
        Me.screen_name.Name = "screen_name"
        Me.screen_name.ReadOnly = True
        '
        'dgvControls
        '
        Me.dgvControls.AllowUserToAddRows = False
        Me.dgvControls.AllowUserToDeleteRows = False
        Me.dgvControls.AllowUserToResizeColumns = False
        Me.dgvControls.AllowUserToResizeRows = False
        Me.dgvControls.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvControls.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvControls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvControls.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.object_name, Me.control_name})
        Me.dgvControls.Location = New System.Drawing.Point(202, 3)
        Me.dgvControls.MultiSelect = False
        Me.dgvControls.Name = "dgvControls"
        Me.dgvControls.ReadOnly = True
        Me.dgvControls.RowHeadersVisible = False
        Me.dgvControls.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvControls.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvControls.Size = New System.Drawing.Size(376, 364)
        Me.dgvControls.TabIndex = 45
        '
        'object_name
        '
        Me.object_name.DataPropertyName = "object_name"
        Me.object_name.HeaderText = "Object"
        Me.object_name.Name = "object_name"
        Me.object_name.ReadOnly = True
        '
        'control_name
        '
        Me.control_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.control_name.DataPropertyName = "control_name"
        Me.control_name.FillWeight = 78.31452!
        Me.control_name.HeaderText = "Control"
        Me.control_name.Name = "control_name"
        Me.control_name.ReadOnly = True
        Me.control_name.Width = 65
        '
        'butScreenControlDelete
        '
        Me.butScreenControlDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butScreenControlDelete.Location = New System.Drawing.Point(313, 400)
        Me.butScreenControlDelete.Name = "butScreenControlDelete"
        Me.butScreenControlDelete.Size = New System.Drawing.Size(54, 26)
        Me.butScreenControlDelete.TabIndex = 48
        Me.butScreenControlDelete.Text = "Delete"
        Me.butScreenControlDelete.UseVisualStyleBackColor = True
        Me.butScreenControlDelete.Visible = False
        '
        'butScreenControlUpdate
        '
        Me.butScreenControlUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butScreenControlUpdate.Location = New System.Drawing.Point(249, 400)
        Me.butScreenControlUpdate.Name = "butScreenControlUpdate"
        Me.butScreenControlUpdate.Size = New System.Drawing.Size(58, 26)
        Me.butScreenControlUpdate.TabIndex = 47
        Me.butScreenControlUpdate.Text = "Update"
        Me.butScreenControlUpdate.UseVisualStyleBackColor = True
        Me.butScreenControlUpdate.Visible = False
        '
        'butScreenControlAdd
        '
        Me.butScreenControlAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butScreenControlAdd.Location = New System.Drawing.Point(202, 400)
        Me.butScreenControlAdd.Name = "butScreenControlAdd"
        Me.butScreenControlAdd.Size = New System.Drawing.Size(41, 26)
        Me.butScreenControlAdd.TabIndex = 46
        Me.butScreenControlAdd.Text = "Add"
        Me.butScreenControlAdd.UseVisualStyleBackColor = True
        Me.butScreenControlAdd.Visible = False
        '
        'comObjects
        '
        Me.comObjects.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.comObjects.FormattingEnabled = True
        Me.comObjects.Location = New System.Drawing.Point(202, 373)
        Me.comObjects.Name = "comObjects"
        Me.comObjects.Size = New System.Drawing.Size(185, 21)
        Me.comObjects.TabIndex = 50
        '
        'comControls
        '
        Me.comControls.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.comControls.FormattingEnabled = True
        Me.comControls.Location = New System.Drawing.Point(393, 373)
        Me.comControls.Name = "comControls"
        Me.comControls.Size = New System.Drawing.Size(185, 21)
        Me.comControls.TabIndex = 51
        '
        'frmScreens
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(578, 438)
        Me.Controls.Add(Me.comControls)
        Me.Controls.Add(Me.comObjects)
        Me.Controls.Add(Me.butScreenControlDelete)
        Me.Controls.Add(Me.butScreenControlUpdate)
        Me.Controls.Add(Me.butScreenControlAdd)
        Me.Controls.Add(Me.dgvControls)
        Me.Controls.Add(Me.dgvScreens)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmScreens"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Screen Design"
        CType(Me.dgvScreens, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvControls, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dgvScreens As System.Windows.Forms.DataGridView
    Friend WithEvents dgvControls As System.Windows.Forms.DataGridView
    Friend WithEvents butScreenControlDelete As System.Windows.Forms.Button
    Friend WithEvents butScreenControlUpdate As System.Windows.Forms.Button
    Friend WithEvents butScreenControlAdd As System.Windows.Forms.Button
    Friend WithEvents comObjects As System.Windows.Forms.ComboBox
    Friend WithEvents comControls As System.Windows.Forms.ComboBox
    Friend WithEvents object_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents control_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents screen_name As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
