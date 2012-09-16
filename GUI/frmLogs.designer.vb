<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLogs
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmLogs))
        Me.dgvEvent_Log = New System.Windows.Forms.DataGridView()
        Me.butClear = New System.Windows.Forms.Button()
        Me.ButClose = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.log_time = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.object_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.event_label = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.parameter_1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.parameter_2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.From_Object_Name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.dgvEvent_Log, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvEvent_Log
        '
        Me.dgvEvent_Log.AllowUserToAddRows = False
        Me.dgvEvent_Log.AllowUserToDeleteRows = False
        Me.dgvEvent_Log.AllowUserToResizeRows = False
        Me.dgvEvent_Log.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEvent_Log.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvEvent_Log.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEvent_Log.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.log_time, Me.object_name, Me.event_label, Me.parameter_1, Me.parameter_2, Me.From_Object_Name})
        Me.dgvEvent_Log.Location = New System.Drawing.Point(2, 1)
        Me.dgvEvent_Log.MultiSelect = False
        Me.dgvEvent_Log.Name = "dgvEvent_Log"
        Me.dgvEvent_Log.ReadOnly = True
        Me.dgvEvent_Log.RowHeadersVisible = False
        Me.dgvEvent_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvEvent_Log.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvEvent_Log.Size = New System.Drawing.Size(815, 547)
        Me.dgvEvent_Log.TabIndex = 41
        '
        'butClear
        '
        Me.butClear.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butClear.Location = New System.Drawing.Point(4, 552)
        Me.butClear.Name = "butClear"
        Me.butClear.Size = New System.Drawing.Size(52, 24)
        Me.butClear.TabIndex = 42
        Me.butClear.Text = "Clear"
        Me.butClear.UseVisualStyleBackColor = True
        '
        'ButClose
        '
        Me.ButClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButClose.Location = New System.Drawing.Point(765, 552)
        Me.ButClose.Name = "ButClose"
        Me.ButClose.Size = New System.Drawing.Size(52, 24)
        Me.ButClose.TabIndex = 43
        Me.ButClose.Text = "Close"
        Me.ButClose.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Interval = 1000
        '
        'log_time
        '
        Me.log_time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.log_time.DataPropertyName = "log_time"
        DataGridViewCellStyle1.Format = "T"
        DataGridViewCellStyle1.NullValue = Nothing
        Me.log_time.DefaultCellStyle = DataGridViewCellStyle1
        Me.log_time.HeaderText = "Time"
        Me.log_time.Name = "log_time"
        Me.log_time.ReadOnly = True
        Me.log_time.Width = 55
        '
        'object_name
        '
        Me.object_name.DataPropertyName = "object_name"
        Me.object_name.HeaderText = "Object Name"
        Me.object_name.Name = "object_name"
        Me.object_name.ReadOnly = True
        '
        'event_label
        '
        Me.event_label.DataPropertyName = "event_label"
        Me.event_label.HeaderText = "Event"
        Me.event_label.Name = "event_label"
        Me.event_label.ReadOnly = True
        '
        'parameter_1
        '
        Me.parameter_1.DataPropertyName = "parameter_1"
        Me.parameter_1.HeaderText = "Parameter 1"
        Me.parameter_1.Name = "parameter_1"
        Me.parameter_1.ReadOnly = True
        '
        'parameter_2
        '
        Me.parameter_2.DataPropertyName = "parameter_2"
        Me.parameter_2.HeaderText = "Parameter 2"
        Me.parameter_2.Name = "parameter_2"
        Me.parameter_2.ReadOnly = True
        '
        'From_Object_Name
        '
        Me.From_Object_Name.DataPropertyName = "from_object_name"
        Me.From_Object_Name.HeaderText = "Sent From"
        Me.From_Object_Name.Name = "From_Object_Name"
        Me.From_Object_Name.ReadOnly = True
        '
        'frmLogs
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(819, 578)
        Me.Controls.Add(Me.ButClose)
        Me.Controls.Add(Me.butClear)
        Me.Controls.Add(Me.dgvEvent_Log)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmLogs"
        Me.Opacity = 0.95R
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Object Event Logs"
        CType(Me.dgvEvent_Log, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dgvEvent_Log As System.Windows.Forms.DataGridView
    Friend WithEvents butClear As System.Windows.Forms.Button
    Friend WithEvents ButClose As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents log_time As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents object_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents event_label As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents parameter_1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents parameter_2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents From_Object_Name As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
