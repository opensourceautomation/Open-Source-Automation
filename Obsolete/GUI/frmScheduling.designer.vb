<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmScheduling
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmScheduling))
        Me.chkSunday = New System.Windows.Forms.CheckBox()
        Me.chkFriday = New System.Windows.Forms.CheckBox()
        Me.chkThursday = New System.Windows.Forms.CheckBox()
        Me.chkWednesday = New System.Windows.Forms.CheckBox()
        Me.chkTuesday = New System.Windows.Forms.CheckBox()
        Me.chkMonday = New System.Windows.Forms.CheckBox()
        Me.optAnnual = New System.Windows.Forms.RadioButton()
        Me.dtpAnnual = New System.Windows.Forms.DateTimePicker()
        Me.optMonthly = New System.Windows.Forms.RadioButton()
        Me.optDaily = New System.Windows.Forms.RadioButton()
        Me.dtpTime = New System.Windows.Forms.DateTimePicker()
        Me.chkSaturday = New System.Windows.Forms.CheckBox()
        Me.cboDay = New System.Windows.Forms.ComboBox()
        Me.dgvQueue = New System.Windows.Forms.DataGridView()
        Me.queue_datetime = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.schedule_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.schedule_id = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnQueueDelete = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.dgvRecurring = New System.Windows.Forms.DataGridView()
        Me.schedule_name1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cboObject = New System.Windows.Forms.ComboBox()
        Me.cboMethod = New System.Windows.Forms.ComboBox()
        Me.cboPattern = New System.Windows.Forms.ComboBox()
        Me.txtParam1 = New System.Windows.Forms.TextBox()
        Me.txtParam2 = New System.Windows.Forms.TextBox()
        Me.btnRecurringDelete = New System.Windows.Forms.Button()
        Me.optSingle = New System.Windows.Forms.RadioButton()
        Me.dtpSingle = New System.Windows.Forms.DateTimePicker()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.nudMinutes = New System.Windows.Forms.NumericUpDown()
        Me.optMinutes = New System.Windows.Forms.RadioButton()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.radScript = New System.Windows.Forms.RadioButton()
        Me.chkMethod = New System.Windows.Forms.RadioButton()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        CType(Me.dgvQueue, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvRecurring, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.nudMinutes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'chkSunday
        '
        Me.chkSunday.AutoSize = True
        Me.chkSunday.Location = New System.Drawing.Point(30, 162)
        Me.chkSunday.Name = "chkSunday"
        Me.chkSunday.Size = New System.Drawing.Size(62, 17)
        Me.chkSunday.TabIndex = 0
        Me.chkSunday.Text = "Sunday"
        Me.chkSunday.UseVisualStyleBackColor = True
        '
        'chkFriday
        '
        Me.chkFriday.AutoSize = True
        Me.chkFriday.Location = New System.Drawing.Point(30, 252)
        Me.chkFriday.Name = "chkFriday"
        Me.chkFriday.Size = New System.Drawing.Size(54, 17)
        Me.chkFriday.TabIndex = 1
        Me.chkFriday.Text = "Friday"
        Me.chkFriday.UseVisualStyleBackColor = True
        '
        'chkThursday
        '
        Me.chkThursday.AutoSize = True
        Me.chkThursday.Location = New System.Drawing.Point(30, 234)
        Me.chkThursday.Name = "chkThursday"
        Me.chkThursday.Size = New System.Drawing.Size(70, 17)
        Me.chkThursday.TabIndex = 2
        Me.chkThursday.Text = "Thursday"
        Me.chkThursday.UseVisualStyleBackColor = True
        '
        'chkWednesday
        '
        Me.chkWednesday.AutoSize = True
        Me.chkWednesday.Location = New System.Drawing.Point(30, 216)
        Me.chkWednesday.Name = "chkWednesday"
        Me.chkWednesday.Size = New System.Drawing.Size(83, 17)
        Me.chkWednesday.TabIndex = 3
        Me.chkWednesday.Text = "Wednesday"
        Me.chkWednesday.UseVisualStyleBackColor = True
        '
        'chkTuesday
        '
        Me.chkTuesday.AutoSize = True
        Me.chkTuesday.Location = New System.Drawing.Point(30, 198)
        Me.chkTuesday.Name = "chkTuesday"
        Me.chkTuesday.Size = New System.Drawing.Size(67, 17)
        Me.chkTuesday.TabIndex = 4
        Me.chkTuesday.Text = "Tuesday"
        Me.chkTuesday.UseVisualStyleBackColor = True
        '
        'chkMonday
        '
        Me.chkMonday.AutoSize = True
        Me.chkMonday.Location = New System.Drawing.Point(30, 180)
        Me.chkMonday.Name = "chkMonday"
        Me.chkMonday.Size = New System.Drawing.Size(64, 17)
        Me.chkMonday.TabIndex = 5
        Me.chkMonday.Text = "Monday"
        Me.chkMonday.UseVisualStyleBackColor = True
        '
        'optAnnual
        '
        Me.optAnnual.AutoSize = True
        Me.optAnnual.Location = New System.Drawing.Point(12, 330)
        Me.optAnnual.Name = "optAnnual"
        Me.optAnnual.Size = New System.Drawing.Size(59, 17)
        Me.optAnnual.TabIndex = 6
        Me.optAnnual.TabStop = True
        Me.optAnnual.Text = "Anually"
        Me.optAnnual.UseVisualStyleBackColor = True
        '
        'dtpAnnual
        '
        Me.dtpAnnual.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpAnnual.Location = New System.Drawing.Point(84, 330)
        Me.dtpAnnual.Name = "dtpAnnual"
        Me.dtpAnnual.Size = New System.Drawing.Size(108, 20)
        Me.dtpAnnual.TabIndex = 7
        '
        'optMonthly
        '
        Me.optMonthly.AutoSize = True
        Me.optMonthly.Location = New System.Drawing.Point(12, 300)
        Me.optMonthly.Name = "optMonthly"
        Me.optMonthly.Size = New System.Drawing.Size(62, 17)
        Me.optMonthly.TabIndex = 9
        Me.optMonthly.TabStop = True
        Me.optMonthly.Text = "Monthly"
        Me.optMonthly.UseVisualStyleBackColor = True
        '
        'optDaily
        '
        Me.optDaily.AutoSize = True
        Me.optDaily.Location = New System.Drawing.Point(12, 132)
        Me.optDaily.Name = "optDaily"
        Me.optDaily.Size = New System.Drawing.Size(48, 17)
        Me.optDaily.TabIndex = 11
        Me.optDaily.TabStop = True
        Me.optDaily.Text = "Daily"
        Me.optDaily.UseVisualStyleBackColor = True
        '
        'dtpTime
        '
        Me.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpTime.Location = New System.Drawing.Point(48, 48)
        Me.dtpTime.Name = "dtpTime"
        Me.dtpTime.ShowUpDown = True
        Me.dtpTime.Size = New System.Drawing.Size(84, 20)
        Me.dtpTime.TabIndex = 12
        '
        'chkSaturday
        '
        Me.chkSaturday.AutoSize = True
        Me.chkSaturday.Location = New System.Drawing.Point(30, 270)
        Me.chkSaturday.Name = "chkSaturday"
        Me.chkSaturday.Size = New System.Drawing.Size(68, 17)
        Me.chkSaturday.TabIndex = 13
        Me.chkSaturday.Text = "Saturday"
        Me.chkSaturday.UseVisualStyleBackColor = True
        '
        'cboDay
        '
        Me.cboDay.FormattingEnabled = True
        Me.cboDay.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31"})
        Me.cboDay.Location = New System.Drawing.Point(84, 300)
        Me.cboDay.Name = "cboDay"
        Me.cboDay.Size = New System.Drawing.Size(84, 21)
        Me.cboDay.TabIndex = 14
        '
        'dgvQueue
        '
        Me.dgvQueue.AllowUserToAddRows = False
        Me.dgvQueue.AllowUserToDeleteRows = False
        Me.dgvQueue.AllowUserToResizeRows = False
        Me.dgvQueue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvQueue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvQueue.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.queue_datetime, Me.schedule_name, Me.schedule_id})
        Me.dgvQueue.Location = New System.Drawing.Point(0, 18)
        Me.dgvQueue.MultiSelect = False
        Me.dgvQueue.Name = "dgvQueue"
        Me.dgvQueue.ReadOnly = True
        Me.dgvQueue.RowHeadersVisible = False
        Me.dgvQueue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvQueue.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvQueue.Size = New System.Drawing.Size(696, 210)
        Me.dgvQueue.TabIndex = 31
        '
        'queue_datetime
        '
        Me.queue_datetime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.queue_datetime.DataPropertyName = "queue_datetime"
        Me.queue_datetime.HeaderText = "DateTime"
        Me.queue_datetime.Name = "queue_datetime"
        Me.queue_datetime.ReadOnly = True
        '
        'schedule_name
        '
        Me.schedule_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.schedule_name.DataPropertyName = "schedule_name"
        Me.schedule_name.HeaderText = "Name"
        Me.schedule_name.Name = "schedule_name"
        Me.schedule_name.ReadOnly = True
        '
        'schedule_id
        '
        Me.schedule_id.DataPropertyName = "schedule_id"
        Me.schedule_id.HeaderText = "schedule_id"
        Me.schedule_id.Name = "schedule_id"
        Me.schedule_id.ReadOnly = True
        Me.schedule_id.Visible = False
        '
        'btnQueueDelete
        '
        Me.btnQueueDelete.Location = New System.Drawing.Point(6, 234)
        Me.btnQueueDelete.Name = "btnQueueDelete"
        Me.btnQueueDelete.Size = New System.Drawing.Size(60, 24)
        Me.btnQueueDelete.TabIndex = 32
        Me.btnQueueDelete.Text = "Delete"
        Me.btnQueueDelete.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAdd.Enabled = False
        Me.btnAdd.Location = New System.Drawing.Point(723, 596)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(54, 24)
        Me.btnAdd.TabIndex = 33
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'btnUpdate
        '
        Me.btnUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnUpdate.Enabled = False
        Me.btnUpdate.Location = New System.Drawing.Point(810, 596)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(66, 24)
        Me.btnUpdate.TabIndex = 34
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'dgvRecurring
        '
        Me.dgvRecurring.AllowUserToAddRows = False
        Me.dgvRecurring.AllowUserToDeleteRows = False
        Me.dgvRecurring.AllowUserToResizeRows = False
        Me.dgvRecurring.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvRecurring.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvRecurring.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.schedule_name1, Me.DataGridViewTextBoxColumn2})
        Me.dgvRecurring.Location = New System.Drawing.Point(0, 299)
        Me.dgvRecurring.MultiSelect = False
        Me.dgvRecurring.Name = "dgvRecurring"
        Me.dgvRecurring.ReadOnly = True
        Me.dgvRecurring.RowHeadersVisible = False
        Me.dgvRecurring.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvRecurring.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvRecurring.Size = New System.Drawing.Size(696, 291)
        Me.dgvRecurring.TabIndex = 35
        '
        'schedule_name1
        '
        Me.schedule_name1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.schedule_name1.DataPropertyName = "schedule_name"
        Me.schedule_name1.HeaderText = "Name"
        Me.schedule_name1.Name = "schedule_name1"
        Me.schedule_name1.ReadOnly = True
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.DataPropertyName = "interval_unit"
        Me.DataGridViewTextBoxColumn2.HeaderText = "Interval"
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.ReadOnly = True
        '
        'cboObject
        '
        Me.cboObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboObject.Enabled = False
        Me.cboObject.FormattingEnabled = True
        Me.cboObject.Location = New System.Drawing.Point(60, 102)
        Me.cboObject.Name = "cboObject"
        Me.cboObject.Size = New System.Drawing.Size(192, 21)
        Me.cboObject.TabIndex = 36
        '
        'cboMethod
        '
        Me.cboMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboMethod.Enabled = False
        Me.cboMethod.FormattingEnabled = True
        Me.cboMethod.Location = New System.Drawing.Point(60, 126)
        Me.cboMethod.Name = "cboMethod"
        Me.cboMethod.Size = New System.Drawing.Size(192, 21)
        Me.cboMethod.TabIndex = 37
        '
        'cboPattern
        '
        Me.cboPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboPattern.FormattingEnabled = True
        Me.cboPattern.Location = New System.Drawing.Point(30, 42)
        Me.cboPattern.Name = "cboPattern"
        Me.cboPattern.Size = New System.Drawing.Size(216, 21)
        Me.cboPattern.TabIndex = 38
        '
        'txtParam1
        '
        Me.txtParam1.Enabled = False
        Me.txtParam1.Location = New System.Drawing.Point(60, 150)
        Me.txtParam1.Name = "txtParam1"
        Me.txtParam1.Size = New System.Drawing.Size(174, 20)
        Me.txtParam1.TabIndex = 39
        '
        'txtParam2
        '
        Me.txtParam2.Enabled = False
        Me.txtParam2.Location = New System.Drawing.Point(60, 174)
        Me.txtParam2.Name = "txtParam2"
        Me.txtParam2.Size = New System.Drawing.Size(174, 20)
        Me.txtParam2.TabIndex = 40
        '
        'btnRecurringDelete
        '
        Me.btnRecurringDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnRecurringDelete.Location = New System.Drawing.Point(899, 596)
        Me.btnRecurringDelete.Name = "btnRecurringDelete"
        Me.btnRecurringDelete.Size = New System.Drawing.Size(60, 24)
        Me.btnRecurringDelete.TabIndex = 41
        Me.btnRecurringDelete.Text = "Delete"
        Me.btnRecurringDelete.UseVisualStyleBackColor = True
        '
        'optSingle
        '
        Me.optSingle.AutoSize = True
        Me.optSingle.Checked = True
        Me.optSingle.Location = New System.Drawing.Point(12, 84)
        Me.optSingle.Name = "optSingle"
        Me.optSingle.Size = New System.Drawing.Size(81, 17)
        Me.optSingle.TabIndex = 42
        Me.optSingle.TabStop = True
        Me.optSingle.Text = "Single Entry"
        Me.optSingle.UseVisualStyleBackColor = True
        '
        'dtpSingle
        '
        Me.dtpSingle.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpSingle.Location = New System.Drawing.Point(108, 84)
        Me.dtpSingle.Name = "dtpSingle"
        Me.dtpSingle.Size = New System.Drawing.Size(96, 20)
        Me.dtpSingle.TabIndex = 43
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(48, 18)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(186, 20)
        Me.txtName.TabIndex = 44
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 13)
        Me.Label1.TabIndex = 45
        Me.Label1.Text = "Name"
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.nudMinutes)
        Me.GroupBox1.Controls.Add(Me.optMinutes)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.txtName)
        Me.GroupBox1.Controls.Add(Me.dtpSingle)
        Me.GroupBox1.Controls.Add(Me.optSingle)
        Me.GroupBox1.Controls.Add(Me.cboDay)
        Me.GroupBox1.Controls.Add(Me.chkSaturday)
        Me.GroupBox1.Controls.Add(Me.dtpTime)
        Me.GroupBox1.Controls.Add(Me.optDaily)
        Me.GroupBox1.Controls.Add(Me.optMonthly)
        Me.GroupBox1.Controls.Add(Me.dtpAnnual)
        Me.GroupBox1.Controls.Add(Me.optAnnual)
        Me.GroupBox1.Controls.Add(Me.chkMonday)
        Me.GroupBox1.Controls.Add(Me.chkTuesday)
        Me.GroupBox1.Controls.Add(Me.chkWednesday)
        Me.GroupBox1.Controls.Add(Me.chkThursday)
        Me.GroupBox1.Controls.Add(Me.chkFriday)
        Me.GroupBox1.Controls.Add(Me.chkSunday)
        Me.GroupBox1.Location = New System.Drawing.Point(702, 18)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(246, 360)
        Me.GroupBox1.TabIndex = 46
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Schedule Rule:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(132, 111)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(44, 13)
        Me.Label7.TabIndex = 49
        Me.Label7.Text = "Minutes"
        '
        'nudMinutes
        '
        Me.nudMinutes.Location = New System.Drawing.Point(72, 108)
        Me.nudMinutes.Maximum = New Decimal(New Integer() {1439, 0, 0, 0})
        Me.nudMinutes.Name = "nudMinutes"
        Me.nudMinutes.Size = New System.Drawing.Size(54, 20)
        Me.nudMinutes.TabIndex = 48
        Me.nudMinutes.Value = New Decimal(New Integer() {20, 0, 0, 0})
        '
        'optMinutes
        '
        Me.optMinutes.AutoSize = True
        Me.optMinutes.Location = New System.Drawing.Point(12, 108)
        Me.optMinutes.Name = "optMinutes"
        Me.optMinutes.Size = New System.Drawing.Size(52, 17)
        Me.optMinutes.TabIndex = 47
        Me.optMinutes.TabStop = True
        Me.optMinutes.Text = "Every"
        Me.optMinutes.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(30, 13)
        Me.Label2.TabIndex = 46
        Me.Label2.Text = "Time"
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.radScript)
        Me.GroupBox2.Controls.Add(Me.chkMethod)
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.cboPattern)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.txtParam2)
        Me.GroupBox2.Controls.Add(Me.txtParam1)
        Me.GroupBox2.Controls.Add(Me.cboMethod)
        Me.GroupBox2.Controls.Add(Me.cboObject)
        Me.GroupBox2.Location = New System.Drawing.Point(702, 390)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(252, 200)
        Me.GroupBox2.TabIndex = 47
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Action to Perform"
        '
        'radScript
        '
        Me.radScript.AutoSize = True
        Me.radScript.Checked = True
        Me.radScript.Location = New System.Drawing.Point(14, 19)
        Me.radScript.Name = "radScript"
        Me.radScript.Size = New System.Drawing.Size(84, 17)
        Me.radScript.TabIndex = 52
        Me.radScript.TabStop = True
        Me.radScript.Text = "Run a Script"
        Me.radScript.UseVisualStyleBackColor = True
        '
        'chkMethod
        '
        Me.chkMethod.AutoSize = True
        Me.chkMethod.Location = New System.Drawing.Point(16, 75)
        Me.chkMethod.Name = "chkMethod"
        Me.chkMethod.Size = New System.Drawing.Size(93, 17)
        Me.chkMethod.TabIndex = 51
        Me.chkMethod.Text = "Run a Method"
        Me.chkMethod.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 180)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(43, 13)
        Me.Label6.TabIndex = 50
        Me.Label6.Text = "Param2"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 156)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(43, 13)
        Me.Label5.TabIndex = 49
        Me.Label5.Text = "Param1"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 132)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 13)
        Me.Label4.TabIndex = 48
        Me.Label4.Text = "Method"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(18, 108)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 13)
        Me.Label3.TabIndex = 47
        Me.Label3.Text = "Object"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(3, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(382, 16)
        Me.Label8.TabIndex = 48
        Me.Label8.Text = "Method Que entries created by the  Schedules  in the lower Grid"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(3, 280)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(538, 16)
        Me.Label9.TabIndex = 49
        Me.Label9.Text = "These are the Named/Recurring Schedules that will generate entries in the Method " & _
    "Queue"
        '
        'frmScheduling
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(960, 624)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnRecurringDelete)
        Me.Controls.Add(Me.dgvRecurring)
        Me.Controls.Add(Me.btnQueueDelete)
        Me.Controls.Add(Me.dgvQueue)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.btnAdd)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmScheduling"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Scheduling"
        CType(Me.dgvQueue, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvRecurring, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.nudMinutes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents chkSunday As System.Windows.Forms.CheckBox
    Friend WithEvents chkFriday As System.Windows.Forms.CheckBox
    Friend WithEvents chkThursday As System.Windows.Forms.CheckBox
    Friend WithEvents chkWednesday As System.Windows.Forms.CheckBox
    Friend WithEvents chkTuesday As System.Windows.Forms.CheckBox
    Friend WithEvents chkMonday As System.Windows.Forms.CheckBox
    Friend WithEvents optAnnual As System.Windows.Forms.RadioButton
    Friend WithEvents dtpAnnual As System.Windows.Forms.DateTimePicker
    Friend WithEvents optMonthly As System.Windows.Forms.RadioButton
    Friend WithEvents optDaily As System.Windows.Forms.RadioButton
    Friend WithEvents dtpTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents chkSaturday As System.Windows.Forms.CheckBox
    Friend WithEvents cboDay As System.Windows.Forms.ComboBox
    Friend WithEvents dgvQueue As System.Windows.Forms.DataGridView
    Friend WithEvents btnQueueDelete As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents dgvRecurring As System.Windows.Forms.DataGridView
    Friend WithEvents cboObject As System.Windows.Forms.ComboBox
    Friend WithEvents cboMethod As System.Windows.Forms.ComboBox
    Friend WithEvents cboPattern As System.Windows.Forms.ComboBox
    Friend WithEvents txtParam1 As System.Windows.Forms.TextBox
    Friend WithEvents txtParam2 As System.Windows.Forms.TextBox
    Friend WithEvents btnRecurringDelete As System.Windows.Forms.Button
    Friend WithEvents optSingle As System.Windows.Forms.RadioButton
    Friend WithEvents dtpSingle As System.Windows.Forms.DateTimePicker
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents queue_datetime As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents schedule_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents schedule_id As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents schedule_name1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents nudMinutes As System.Windows.Forms.NumericUpDown
    Friend WithEvents optMinutes As System.Windows.Forms.RadioButton
    Friend WithEvents radScript As System.Windows.Forms.RadioButton
    Friend WithEvents chkMethod As System.Windows.Forms.RadioButton
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
End Class
