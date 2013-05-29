<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmObjectTypes
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmObjectTypes))
        Me.txtState = New System.Windows.Forms.TextBox()
        Me.comTypeOwner = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.dgvStates = New System.Windows.Forms.DataGridView()
        Me.state_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.state_label = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.butStateAdd = New System.Windows.Forms.Button()
        Me.txtStateLabel = New System.Windows.Forms.TextBox()
        Me.butStateUpdate = New System.Windows.Forms.Button()
        Me.butStateDelete = New System.Windows.Forms.Button()
        Me.txtEvent = New System.Windows.Forms.TextBox()
        Me.dgvProperties = New System.Windows.Forms.DataGridView()
        Me.property_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Property_Default = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.property_datatype = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.track_history = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dgvEvents = New System.Windows.Forms.DataGridView()
        Me.event_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.event_label = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dgvMethods = New System.Windows.Forms.DataGridView()
        Me.method_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.param_1_default = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.param_2_default = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.param_1_label = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.param_2_label = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.method_label = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.txtEventLabel = New System.Windows.Forms.TextBox()
        Me.txtMethodLabel = New System.Windows.Forms.TextBox()
        Me.txtMethod = New System.Windows.Forms.TextBox()
        Me.butEventDelete = New System.Windows.Forms.Button()
        Me.butEventUpdate = New System.Windows.Forms.Button()
        Me.butEventAdd = New System.Windows.Forms.Button()
        Me.butMethodDelete = New System.Windows.Forms.Button()
        Me.butMethodUpdate = New System.Windows.Forms.Button()
        Me.butMethodAdd = New System.Windows.Forms.Button()
        Me.butPropertyDelete = New System.Windows.Forms.Button()
        Me.butPropertyUpdate = New System.Windows.Forms.Button()
        Me.butPropertyAdd = New System.Windows.Forms.Button()
        Me.txtProperty = New System.Windows.Forms.TextBox()
        Me.comPropertyType = New System.Windows.Forms.ComboBox()
        Me.dgvObjectTypes = New System.Windows.Forms.DataGridView()
        Me.base_type = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.object_type = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.is_container = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.object_type_description = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.object_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.object_type_owner = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.system_hidden = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.hide_redundant_events = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.butObjectDelete = New System.Windows.Forms.Button()
        Me.butObjectUpdate = New System.Windows.Forms.Button()
        Me.txtTypeDescription = New System.Windows.Forms.TextBox()
        Me.butObjectAdd = New System.Windows.Forms.Button()
        Me.txtObjectType = New System.Windows.Forms.TextBox()
        Me.chkOwner = New System.Windows.Forms.CheckBox()
        Me.chkSystem = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.comBaseType = New System.Windows.Forms.ComboBox()
        Me.butNewSubtype = New System.Windows.Forms.Button()
        Me.butClose = New System.Windows.Forms.Button()
        Me.butExport = New System.Windows.Forms.Button()
        Me.chkContainer = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtParamDefault2 = New System.Windows.Forms.TextBox()
        Me.txtParamDefault1 = New System.Windows.Forms.TextBox()
        Me.txtParamLabel2 = New System.Windows.Forms.TextBox()
        Me.txtParamLabel1 = New System.Windows.Forms.TextBox()
        Me.chkHideRedundant = New System.Windows.Forms.CheckBox()
        Me.chkTrackHistory = New System.Windows.Forms.CheckBox()
        Me.txtPropertyDefault = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.btnPropOption = New System.Windows.Forms.Button()
        CType(Me.dgvStates, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvEvents, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvMethods, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvObjectTypes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtState
        '
        Me.txtState.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtState.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtState.Location = New System.Drawing.Point(646, 4)
        Me.txtState.Name = "txtState"
        Me.txtState.Size = New System.Drawing.Size(176, 20)
        Me.txtState.TabIndex = 8
        '
        'comTypeOwner
        '
        Me.comTypeOwner.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.comTypeOwner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comTypeOwner.FormattingEnabled = True
        Me.comTypeOwner.Location = New System.Drawing.Point(70, 385)
        Me.comTypeOwner.Name = "comTypeOwner"
        Me.comTypeOwner.Size = New System.Drawing.Size(226, 21)
        Me.comTypeOwner.TabIndex = 12
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(8, 388)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(56, 13)
        Me.Label6.TabIndex = 13
        Me.Label6.Text = "Owned By"
        '
        'dgvStates
        '
        Me.dgvStates.AllowUserToAddRows = False
        Me.dgvStates.AllowUserToDeleteRows = False
        Me.dgvStates.AllowUserToResizeRows = False
        Me.dgvStates.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvStates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvStates.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.state_name, Me.state_label})
        Me.dgvStates.Location = New System.Drawing.Point(388, 2)
        Me.dgvStates.MultiSelect = False
        Me.dgvStates.Name = "dgvStates"
        Me.dgvStates.ReadOnly = True
        Me.dgvStates.RowHeadersVisible = False
        Me.dgvStates.RowTemplate.Height = 24
        Me.dgvStates.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvStates.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvStates.Size = New System.Drawing.Size(204, 88)
        Me.dgvStates.TabIndex = 14
        '
        'state_name
        '
        Me.state_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.state_name.DataPropertyName = "state_name"
        Me.state_name.HeaderText = "State"
        Me.state_name.Name = "state_name"
        Me.state_name.ReadOnly = True
        Me.state_name.Width = 57
        '
        'state_label
        '
        Me.state_label.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.state_label.DataPropertyName = "state_label"
        Me.state_label.HeaderText = "Label"
        Me.state_label.Name = "state_label"
        Me.state_label.ReadOnly = True
        '
        'butStateAdd
        '
        Me.butStateAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butStateAdd.Enabled = False
        Me.butStateAdd.Location = New System.Drawing.Point(656, 56)
        Me.butStateAdd.Name = "butStateAdd"
        Me.butStateAdd.Size = New System.Drawing.Size(52, 26)
        Me.butStateAdd.TabIndex = 15
        Me.butStateAdd.Text = "Add"
        Me.butStateAdd.UseVisualStyleBackColor = True
        '
        'txtStateLabel
        '
        Me.txtStateLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtStateLabel.Location = New System.Drawing.Point(646, 30)
        Me.txtStateLabel.Name = "txtStateLabel"
        Me.txtStateLabel.Size = New System.Drawing.Size(177, 20)
        Me.txtStateLabel.TabIndex = 16
        '
        'butStateUpdate
        '
        Me.butStateUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butStateUpdate.Enabled = False
        Me.butStateUpdate.Location = New System.Drawing.Point(712, 56)
        Me.butStateUpdate.Name = "butStateUpdate"
        Me.butStateUpdate.Size = New System.Drawing.Size(52, 26)
        Me.butStateUpdate.TabIndex = 17
        Me.butStateUpdate.Text = "Update"
        Me.butStateUpdate.UseVisualStyleBackColor = True
        '
        'butStateDelete
        '
        Me.butStateDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butStateDelete.Enabled = False
        Me.butStateDelete.Location = New System.Drawing.Point(770, 56)
        Me.butStateDelete.Name = "butStateDelete"
        Me.butStateDelete.Size = New System.Drawing.Size(52, 26)
        Me.butStateDelete.TabIndex = 18
        Me.butStateDelete.Text = "Delete"
        Me.butStateDelete.UseVisualStyleBackColor = True
        '
        'txtEvent
        '
        Me.txtEvent.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEvent.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtEvent.Location = New System.Drawing.Point(646, 97)
        Me.txtEvent.Name = "txtEvent"
        Me.txtEvent.Size = New System.Drawing.Size(176, 20)
        Me.txtEvent.TabIndex = 20
        '
        'dgvProperties
        '
        Me.dgvProperties.AllowUserToAddRows = False
        Me.dgvProperties.AllowUserToDeleteRows = False
        Me.dgvProperties.AllowUserToResizeRows = False
        Me.dgvProperties.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProperties.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.property_name, Me.Property_Default, Me.property_datatype, Me.track_history})
        Me.dgvProperties.Location = New System.Drawing.Point(389, 346)
        Me.dgvProperties.MultiSelect = False
        Me.dgvProperties.Name = "dgvProperties"
        Me.dgvProperties.ReadOnly = True
        Me.dgvProperties.RowHeadersVisible = False
        Me.dgvProperties.RowTemplate.Height = 24
        Me.dgvProperties.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvProperties.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvProperties.Size = New System.Drawing.Size(204, 174)
        Me.dgvProperties.TabIndex = 22
        '
        'property_name
        '
        Me.property_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.property_name.DataPropertyName = "property_name"
        Me.property_name.HeaderText = "Property"
        Me.property_name.Name = "property_name"
        Me.property_name.ReadOnly = True
        '
        'Property_Default
        '
        Me.Property_Default.DataPropertyName = "Property_Default"
        Me.Property_Default.HeaderText = "Property_Default"
        Me.Property_Default.Name = "Property_Default"
        Me.Property_Default.ReadOnly = True
        Me.Property_Default.Visible = False
        '
        'property_datatype
        '
        Me.property_datatype.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.property_datatype.DataPropertyName = "property_datatype"
        Me.property_datatype.HeaderText = "DataType"
        Me.property_datatype.Name = "property_datatype"
        Me.property_datatype.ReadOnly = True
        Me.property_datatype.Width = 79
        '
        'track_history
        '
        Me.track_history.DataPropertyName = "track_history"
        Me.track_history.HeaderText = "track_history"
        Me.track_history.Name = "track_history"
        Me.track_history.ReadOnly = True
        Me.track_history.Visible = False
        '
        'dgvEvents
        '
        Me.dgvEvents.AllowUserToAddRows = False
        Me.dgvEvents.AllowUserToDeleteRows = False
        Me.dgvEvents.AllowUserToResizeRows = False
        Me.dgvEvents.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEvents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEvents.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.event_name, Me.event_label})
        Me.dgvEvents.Location = New System.Drawing.Point(388, 97)
        Me.dgvEvents.MultiSelect = False
        Me.dgvEvents.Name = "dgvEvents"
        Me.dgvEvents.ReadOnly = True
        Me.dgvEvents.RowHeadersVisible = False
        Me.dgvEvents.RowTemplate.Height = 24
        Me.dgvEvents.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvEvents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvEvents.Size = New System.Drawing.Size(204, 83)
        Me.dgvEvents.TabIndex = 23
        '
        'event_name
        '
        Me.event_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.event_name.DataPropertyName = "event_name"
        Me.event_name.HeaderText = "Event"
        Me.event_name.Name = "event_name"
        Me.event_name.ReadOnly = True
        '
        'event_label
        '
        Me.event_label.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.event_label.DataPropertyName = "event_label"
        Me.event_label.HeaderText = "Label"
        Me.event_label.Name = "event_label"
        Me.event_label.ReadOnly = True
        Me.event_label.Width = 58
        '
        'dgvMethods
        '
        Me.dgvMethods.AllowUserToAddRows = False
        Me.dgvMethods.AllowUserToDeleteRows = False
        Me.dgvMethods.AllowUserToResizeRows = False
        Me.dgvMethods.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvMethods.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMethods.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.method_name, Me.param_1_default, Me.param_2_default, Me.param_1_label, Me.param_2_label, Me.method_label})
        Me.dgvMethods.Location = New System.Drawing.Point(388, 189)
        Me.dgvMethods.MultiSelect = False
        Me.dgvMethods.Name = "dgvMethods"
        Me.dgvMethods.ReadOnly = True
        Me.dgvMethods.RowHeadersVisible = False
        Me.dgvMethods.RowTemplate.Height = 24
        Me.dgvMethods.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvMethods.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvMethods.Size = New System.Drawing.Size(204, 151)
        Me.dgvMethods.TabIndex = 24
        '
        'method_name
        '
        Me.method_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.method_name.DataPropertyName = "method_name"
        Me.method_name.HeaderText = "Method"
        Me.method_name.Name = "method_name"
        Me.method_name.ReadOnly = True
        '
        'param_1_default
        '
        Me.param_1_default.DataPropertyName = "param_1_default"
        Me.param_1_default.HeaderText = "param_1_default"
        Me.param_1_default.Name = "param_1_default"
        Me.param_1_default.ReadOnly = True
        Me.param_1_default.Visible = False
        '
        'param_2_default
        '
        Me.param_2_default.DataPropertyName = "param_2_default"
        Me.param_2_default.HeaderText = "param_2_default"
        Me.param_2_default.Name = "param_2_default"
        Me.param_2_default.ReadOnly = True
        Me.param_2_default.Visible = False
        '
        'param_1_label
        '
        Me.param_1_label.DataPropertyName = "param_1_label"
        Me.param_1_label.HeaderText = "param_1_label"
        Me.param_1_label.Name = "param_1_label"
        Me.param_1_label.ReadOnly = True
        Me.param_1_label.Visible = False
        '
        'param_2_label
        '
        Me.param_2_label.DataPropertyName = "param_2_label"
        Me.param_2_label.HeaderText = "param_2_label"
        Me.param_2_label.Name = "param_2_label"
        Me.param_2_label.ReadOnly = True
        Me.param_2_label.Visible = False
        '
        'method_label
        '
        Me.method_label.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.method_label.DataPropertyName = "method_label"
        Me.method_label.HeaderText = "Label"
        Me.method_label.Name = "method_label"
        Me.method_label.ReadOnly = True
        Me.method_label.Width = 58
        '
        'txtEventLabel
        '
        Me.txtEventLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEventLabel.Location = New System.Drawing.Point(646, 122)
        Me.txtEventLabel.Name = "txtEventLabel"
        Me.txtEventLabel.Size = New System.Drawing.Size(176, 20)
        Me.txtEventLabel.TabIndex = 25
        '
        'txtMethodLabel
        '
        Me.txtMethodLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtMethodLabel.Location = New System.Drawing.Point(646, 214)
        Me.txtMethodLabel.Name = "txtMethodLabel"
        Me.txtMethodLabel.Size = New System.Drawing.Size(176, 20)
        Me.txtMethodLabel.TabIndex = 27
        '
        'txtMethod
        '
        Me.txtMethod.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtMethod.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtMethod.Location = New System.Drawing.Point(646, 188)
        Me.txtMethod.Name = "txtMethod"
        Me.txtMethod.Size = New System.Drawing.Size(176, 20)
        Me.txtMethod.TabIndex = 26
        '
        'butEventDelete
        '
        Me.butEventDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butEventDelete.Enabled = False
        Me.butEventDelete.Location = New System.Drawing.Point(772, 148)
        Me.butEventDelete.Name = "butEventDelete"
        Me.butEventDelete.Size = New System.Drawing.Size(51, 26)
        Me.butEventDelete.TabIndex = 30
        Me.butEventDelete.Text = "Delete"
        Me.butEventDelete.UseVisualStyleBackColor = True
        '
        'butEventUpdate
        '
        Me.butEventUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butEventUpdate.Enabled = False
        Me.butEventUpdate.Location = New System.Drawing.Point(712, 148)
        Me.butEventUpdate.Name = "butEventUpdate"
        Me.butEventUpdate.Size = New System.Drawing.Size(52, 26)
        Me.butEventUpdate.TabIndex = 29
        Me.butEventUpdate.Text = "Update"
        Me.butEventUpdate.UseVisualStyleBackColor = True
        '
        'butEventAdd
        '
        Me.butEventAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butEventAdd.Enabled = False
        Me.butEventAdd.Location = New System.Drawing.Point(656, 148)
        Me.butEventAdd.Name = "butEventAdd"
        Me.butEventAdd.Size = New System.Drawing.Size(52, 26)
        Me.butEventAdd.TabIndex = 28
        Me.butEventAdd.Text = "Add"
        Me.butEventAdd.UseVisualStyleBackColor = True
        '
        'butMethodDelete
        '
        Me.butMethodDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butMethodDelete.Enabled = False
        Me.butMethodDelete.Location = New System.Drawing.Point(770, 314)
        Me.butMethodDelete.Name = "butMethodDelete"
        Me.butMethodDelete.Size = New System.Drawing.Size(52, 26)
        Me.butMethodDelete.TabIndex = 33
        Me.butMethodDelete.Text = "Delete"
        Me.butMethodDelete.UseVisualStyleBackColor = True
        '
        'butMethodUpdate
        '
        Me.butMethodUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butMethodUpdate.Enabled = False
        Me.butMethodUpdate.Location = New System.Drawing.Point(712, 314)
        Me.butMethodUpdate.Name = "butMethodUpdate"
        Me.butMethodUpdate.Size = New System.Drawing.Size(52, 26)
        Me.butMethodUpdate.TabIndex = 32
        Me.butMethodUpdate.Text = "Update"
        Me.butMethodUpdate.UseVisualStyleBackColor = True
        '
        'butMethodAdd
        '
        Me.butMethodAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butMethodAdd.Enabled = False
        Me.butMethodAdd.Location = New System.Drawing.Point(656, 314)
        Me.butMethodAdd.Name = "butMethodAdd"
        Me.butMethodAdd.Size = New System.Drawing.Size(52, 26)
        Me.butMethodAdd.TabIndex = 31
        Me.butMethodAdd.Text = "Add"
        Me.butMethodAdd.UseVisualStyleBackColor = True
        '
        'butPropertyDelete
        '
        Me.butPropertyDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butPropertyDelete.Enabled = False
        Me.butPropertyDelete.Location = New System.Drawing.Point(776, 462)
        Me.butPropertyDelete.Name = "butPropertyDelete"
        Me.butPropertyDelete.Size = New System.Drawing.Size(52, 26)
        Me.butPropertyDelete.TabIndex = 38
        Me.butPropertyDelete.Text = "Delete"
        Me.butPropertyDelete.UseVisualStyleBackColor = True
        '
        'butPropertyUpdate
        '
        Me.butPropertyUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butPropertyUpdate.Enabled = False
        Me.butPropertyUpdate.Location = New System.Drawing.Point(718, 462)
        Me.butPropertyUpdate.Name = "butPropertyUpdate"
        Me.butPropertyUpdate.Size = New System.Drawing.Size(51, 26)
        Me.butPropertyUpdate.TabIndex = 37
        Me.butPropertyUpdate.Text = "Update"
        Me.butPropertyUpdate.UseVisualStyleBackColor = True
        '
        'butPropertyAdd
        '
        Me.butPropertyAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butPropertyAdd.Enabled = False
        Me.butPropertyAdd.Location = New System.Drawing.Point(661, 462)
        Me.butPropertyAdd.Name = "butPropertyAdd"
        Me.butPropertyAdd.Size = New System.Drawing.Size(52, 26)
        Me.butPropertyAdd.TabIndex = 36
        Me.butPropertyAdd.Text = "Add"
        Me.butPropertyAdd.UseVisualStyleBackColor = True
        '
        'txtProperty
        '
        Me.txtProperty.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtProperty.Location = New System.Drawing.Point(652, 348)
        Me.txtProperty.Name = "txtProperty"
        Me.txtProperty.Size = New System.Drawing.Size(177, 20)
        Me.txtProperty.TabIndex = 34
        '
        'comPropertyType
        '
        Me.comPropertyType.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.comPropertyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comPropertyType.FormattingEnabled = True
        Me.comPropertyType.Items.AddRange(New Object() {"Boolean", "Color", "DateTime", "File", "Float", "Integer", "List", "ListSelection", "String"})
        Me.comPropertyType.Location = New System.Drawing.Point(652, 378)
        Me.comPropertyType.Name = "comPropertyType"
        Me.comPropertyType.Size = New System.Drawing.Size(177, 21)
        Me.comPropertyType.TabIndex = 39
        '
        'dgvObjectTypes
        '
        Me.dgvObjectTypes.AllowUserToAddRows = False
        Me.dgvObjectTypes.AllowUserToDeleteRows = False
        Me.dgvObjectTypes.AllowUserToResizeRows = False
        Me.dgvObjectTypes.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvObjectTypes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvObjectTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvObjectTypes.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.base_type, Me.object_type, Me.is_container, Me.object_type_description, Me.object_name, Me.object_type_owner, Me.system_hidden, Me.hide_redundant_events})
        Me.dgvObjectTypes.Location = New System.Drawing.Point(2, 2)
        Me.dgvObjectTypes.MultiSelect = False
        Me.dgvObjectTypes.Name = "dgvObjectTypes"
        Me.dgvObjectTypes.ReadOnly = True
        Me.dgvObjectTypes.RowHeadersVisible = False
        Me.dgvObjectTypes.RowTemplate.Height = 24
        Me.dgvObjectTypes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvObjectTypes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvObjectTypes.Size = New System.Drawing.Size(379, 324)
        Me.dgvObjectTypes.TabIndex = 40
        '
        'base_type
        '
        Me.base_type.DataPropertyName = "base_type"
        Me.base_type.HeaderText = "Base Type"
        Me.base_type.Name = "base_type"
        Me.base_type.ReadOnly = True
        '
        'object_type
        '
        Me.object_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.object_type.DataPropertyName = "object_type"
        Me.object_type.HeaderText = "Object Type"
        Me.object_type.MinimumWidth = 150
        Me.object_type.Name = "object_type"
        Me.object_type.ReadOnly = True
        Me.object_type.Width = 150
        '
        'is_container
        '
        Me.is_container.DataPropertyName = "container"
        Me.is_container.HeaderText = "Container"
        Me.is_container.Name = "is_container"
        Me.is_container.ReadOnly = True
        Me.is_container.Visible = False
        '
        'object_type_description
        '
        Me.object_type_description.DataPropertyName = "object_type_description"
        Me.object_type_description.HeaderText = "Description"
        Me.object_type_description.Name = "object_type_description"
        Me.object_type_description.ReadOnly = True
        '
        'object_name
        '
        Me.object_name.DataPropertyName = "object_name"
        Me.object_name.HeaderText = "Owner"
        Me.object_name.Name = "object_name"
        Me.object_name.ReadOnly = True
        Me.object_name.Visible = False
        '
        'object_type_owner
        '
        Me.object_type_owner.DataPropertyName = "object_type_owner"
        Me.object_type_owner.HeaderText = "Type Owner"
        Me.object_type_owner.Name = "object_type_owner"
        Me.object_type_owner.ReadOnly = True
        Me.object_type_owner.Visible = False
        '
        'system_hidden
        '
        Me.system_hidden.DataPropertyName = "system_hidden"
        Me.system_hidden.HeaderText = "System"
        Me.system_hidden.Name = "system_hidden"
        Me.system_hidden.ReadOnly = True
        Me.system_hidden.Visible = False
        '
        'hide_redundant_events
        '
        Me.hide_redundant_events.DataPropertyName = "hide_redundant_events"
        Me.hide_redundant_events.HeaderText = "Redundant Events"
        Me.hide_redundant_events.Name = "hide_redundant_events"
        Me.hide_redundant_events.ReadOnly = True
        Me.hide_redundant_events.Visible = False
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(608, 33)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(33, 13)
        Me.Label3.TabIndex = 42
        Me.Label3.Text = "Label"
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(608, 100)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(35, 13)
        Me.Label4.TabIndex = 43
        Me.Label4.Text = "Event"
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(608, 122)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(33, 13)
        Me.Label5.TabIndex = 44
        Me.Label5.Text = "Label"
        '
        'Label7
        '
        Me.Label7.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(601, 191)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(43, 13)
        Me.Label7.TabIndex = 45
        Me.Label7.Text = "Method"
        '
        'Label8
        '
        Me.Label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(607, 217)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(33, 13)
        Me.Label8.TabIndex = 46
        Me.Label8.Text = "Label"
        '
        'Label9
        '
        Me.Label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(598, 354)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(46, 13)
        Me.Label9.TabIndex = 47
        Me.Label9.Text = "Property"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label10
        '
        Me.Label10.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(592, 384)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(54, 13)
        Me.Label10.TabIndex = 48
        Me.Label10.Text = "DataType"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(32, 362)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 13)
        Me.Label1.TabIndex = 55
        Me.Label1.Text = "Desc."
        '
        'Label11
        '
        Me.Label11.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(33, 336)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(35, 13)
        Me.Label11.TabIndex = 54
        Me.Label11.Text = "Name"
        '
        'butObjectDelete
        '
        Me.butObjectDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butObjectDelete.Location = New System.Drawing.Point(182, 493)
        Me.butObjectDelete.Name = "butObjectDelete"
        Me.butObjectDelete.Size = New System.Drawing.Size(52, 26)
        Me.butObjectDelete.TabIndex = 53
        Me.butObjectDelete.Text = "Delete"
        Me.butObjectDelete.UseVisualStyleBackColor = True
        '
        'butObjectUpdate
        '
        Me.butObjectUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butObjectUpdate.Enabled = False
        Me.butObjectUpdate.Location = New System.Drawing.Point(124, 492)
        Me.butObjectUpdate.Name = "butObjectUpdate"
        Me.butObjectUpdate.Size = New System.Drawing.Size(52, 26)
        Me.butObjectUpdate.TabIndex = 52
        Me.butObjectUpdate.Text = "Update"
        Me.butObjectUpdate.UseVisualStyleBackColor = True
        '
        'txtTypeDescription
        '
        Me.txtTypeDescription.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtTypeDescription.Location = New System.Drawing.Point(70, 359)
        Me.txtTypeDescription.Name = "txtTypeDescription"
        Me.txtTypeDescription.Size = New System.Drawing.Size(226, 20)
        Me.txtTypeDescription.TabIndex = 51
        '
        'butObjectAdd
        '
        Me.butObjectAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butObjectAdd.Location = New System.Drawing.Point(12, 492)
        Me.butObjectAdd.Name = "butObjectAdd"
        Me.butObjectAdd.Size = New System.Drawing.Size(52, 26)
        Me.butObjectAdd.TabIndex = 50
        Me.butObjectAdd.Text = "Add"
        Me.butObjectAdd.UseVisualStyleBackColor = True
        '
        'txtObjectType
        '
        Me.txtObjectType.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtObjectType.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtObjectType.Location = New System.Drawing.Point(70, 333)
        Me.txtObjectType.Name = "txtObjectType"
        Me.txtObjectType.Size = New System.Drawing.Size(226, 20)
        Me.txtObjectType.TabIndex = 49
        '
        'chkOwner
        '
        Me.chkOwner.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkOwner.AutoSize = True
        Me.chkOwner.Location = New System.Drawing.Point(37, 442)
        Me.chkOwner.Name = "chkOwner"
        Me.chkOwner.Size = New System.Drawing.Size(124, 17)
        Me.chkOwner.TabIndex = 56
        Me.chkOwner.Text = "Object Type Owner?"
        Me.chkOwner.UseVisualStyleBackColor = True
        '
        'chkSystem
        '
        Me.chkSystem.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkSystem.AutoSize = True
        Me.chkSystem.Location = New System.Drawing.Point(36, 470)
        Me.chkSystem.Name = "chkSystem"
        Me.chkSystem.Size = New System.Drawing.Size(142, 17)
        Me.chkSystem.TabIndex = 57
        Me.chkSystem.Text = "Reserved System Type?"
        Me.chkSystem.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(607, 7)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(32, 13)
        Me.Label2.TabIndex = 58
        Me.Label2.Text = "State"
        '
        'Label12
        '
        Me.Label12.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(8, 415)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(58, 13)
        Me.Label12.TabIndex = 60
        Me.Label12.Text = "Base Type"
        '
        'comBaseType
        '
        Me.comBaseType.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.comBaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comBaseType.FormattingEnabled = True
        Me.comBaseType.Location = New System.Drawing.Point(70, 412)
        Me.comBaseType.Name = "comBaseType"
        Me.comBaseType.Size = New System.Drawing.Size(226, 21)
        Me.comBaseType.TabIndex = 59
        '
        'butNewSubtype
        '
        Me.butNewSubtype.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butNewSubtype.Location = New System.Drawing.Point(70, 493)
        Me.butNewSubtype.Name = "butNewSubtype"
        Me.butNewSubtype.Size = New System.Drawing.Size(48, 25)
        Me.butNewSubtype.TabIndex = 61
        Me.butNewSubtype.Text = "Clone"
        Me.butNewSubtype.UseVisualStyleBackColor = True
        '
        'butClose
        '
        Me.butClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butClose.Location = New System.Drawing.Point(776, 497)
        Me.butClose.Name = "butClose"
        Me.butClose.Size = New System.Drawing.Size(53, 23)
        Me.butClose.TabIndex = 62
        Me.butClose.Text = "Close"
        Me.butClose.UseVisualStyleBackColor = True
        '
        'butExport
        '
        Me.butExport.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butExport.Location = New System.Drawing.Point(243, 492)
        Me.butExport.Name = "butExport"
        Me.butExport.Size = New System.Drawing.Size(53, 26)
        Me.butExport.TabIndex = 63
        Me.butExport.Text = "Export"
        Me.butExport.UseVisualStyleBackColor = True
        '
        'chkContainer
        '
        Me.chkContainer.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkContainer.AutoSize = True
        Me.chkContainer.Location = New System.Drawing.Point(203, 442)
        Me.chkContainer.Name = "chkContainer"
        Me.chkContainer.Size = New System.Drawing.Size(77, 17)
        Me.chkContainer.TabIndex = 64
        Me.chkContainer.Text = "Container?"
        Me.chkContainer.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.txtParamDefault2)
        Me.GroupBox1.Controls.Add(Me.txtParamDefault1)
        Me.GroupBox1.Controls.Add(Me.txtParamLabel2)
        Me.GroupBox1.Controls.Add(Me.txtParamLabel1)
        Me.GroupBox1.Location = New System.Drawing.Point(593, 240)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(238, 68)
        Me.GroupBox1.TabIndex = 65
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Parameter Labels        /      Default Values"
        '
        'txtParamDefault2
        '
        Me.txtParamDefault2.Location = New System.Drawing.Point(140, 44)
        Me.txtParamDefault2.Name = "txtParamDefault2"
        Me.txtParamDefault2.Size = New System.Drawing.Size(90, 20)
        Me.txtParamDefault2.TabIndex = 31
        '
        'txtParamDefault1
        '
        Me.txtParamDefault1.Location = New System.Drawing.Point(140, 20)
        Me.txtParamDefault1.Name = "txtParamDefault1"
        Me.txtParamDefault1.Size = New System.Drawing.Size(90, 20)
        Me.txtParamDefault1.TabIndex = 30
        '
        'txtParamLabel2
        '
        Me.txtParamLabel2.Location = New System.Drawing.Point(6, 45)
        Me.txtParamLabel2.Name = "txtParamLabel2"
        Me.txtParamLabel2.Size = New System.Drawing.Size(130, 20)
        Me.txtParamLabel2.TabIndex = 29
        '
        'txtParamLabel1
        '
        Me.txtParamLabel1.Location = New System.Drawing.Point(6, 19)
        Me.txtParamLabel1.Name = "txtParamLabel1"
        Me.txtParamLabel1.Size = New System.Drawing.Size(130, 20)
        Me.txtParamLabel1.TabIndex = 28
        '
        'chkHideRedundant
        '
        Me.chkHideRedundant.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkHideRedundant.AutoSize = True
        Me.chkHideRedundant.Location = New System.Drawing.Point(203, 466)
        Me.chkHideRedundant.Name = "chkHideRedundant"
        Me.chkHideRedundant.Size = New System.Drawing.Size(146, 17)
        Me.chkHideRedundant.TabIndex = 66
        Me.chkHideRedundant.Text = "Hide Reduandant Events"
        Me.chkHideRedundant.UseVisualStyleBackColor = True
        '
        'chkTrackHistory
        '
        Me.chkTrackHistory.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkTrackHistory.AutoSize = True
        Me.chkTrackHistory.Location = New System.Drawing.Point(652, 439)
        Me.chkTrackHistory.Name = "chkTrackHistory"
        Me.chkTrackHistory.Size = New System.Drawing.Size(99, 17)
        Me.chkTrackHistory.TabIndex = 67
        Me.chkTrackHistory.Text = "Track Changes"
        Me.chkTrackHistory.UseVisualStyleBackColor = True
        '
        'txtPropertyDefault
        '
        Me.txtPropertyDefault.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPropertyDefault.Location = New System.Drawing.Point(652, 408)
        Me.txtPropertyDefault.Name = "txtPropertyDefault"
        Me.txtPropertyDefault.Size = New System.Drawing.Size(175, 20)
        Me.txtPropertyDefault.TabIndex = 72
        '
        'Label13
        '
        Me.Label13.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(604, 414)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(41, 13)
        Me.Label13.TabIndex = 74
        Me.Label13.Text = "Default"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnPropOption
        '
        Me.btnPropOption.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPropOption.Enabled = False
        Me.btnPropOption.Location = New System.Drawing.Point(604, 462)
        Me.btnPropOption.Name = "btnPropOption"
        Me.btnPropOption.Size = New System.Drawing.Size(52, 26)
        Me.btnPropOption.TabIndex = 75
        Me.btnPropOption.Text = "Options"
        Me.btnPropOption.UseVisualStyleBackColor = True
        '
        'frmObjectTypes
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(835, 526)
        Me.Controls.Add(Me.btnPropOption)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.txtPropertyDefault)
        Me.Controls.Add(Me.chkTrackHistory)
        Me.Controls.Add(Me.chkHideRedundant)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.chkContainer)
        Me.Controls.Add(Me.butExport)
        Me.Controls.Add(Me.butClose)
        Me.Controls.Add(Me.butNewSubtype)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.comBaseType)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.chkSystem)
        Me.Controls.Add(Me.chkOwner)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.butObjectDelete)
        Me.Controls.Add(Me.butObjectUpdate)
        Me.Controls.Add(Me.txtTypeDescription)
        Me.Controls.Add(Me.butObjectAdd)
        Me.Controls.Add(Me.txtObjectType)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.dgvObjectTypes)
        Me.Controls.Add(Me.comPropertyType)
        Me.Controls.Add(Me.butPropertyDelete)
        Me.Controls.Add(Me.butPropertyUpdate)
        Me.Controls.Add(Me.butPropertyAdd)
        Me.Controls.Add(Me.txtProperty)
        Me.Controls.Add(Me.butMethodDelete)
        Me.Controls.Add(Me.butMethodUpdate)
        Me.Controls.Add(Me.butMethodAdd)
        Me.Controls.Add(Me.butEventDelete)
        Me.Controls.Add(Me.butEventUpdate)
        Me.Controls.Add(Me.butEventAdd)
        Me.Controls.Add(Me.txtMethodLabel)
        Me.Controls.Add(Me.txtMethod)
        Me.Controls.Add(Me.txtEventLabel)
        Me.Controls.Add(Me.dgvMethods)
        Me.Controls.Add(Me.dgvEvents)
        Me.Controls.Add(Me.dgvProperties)
        Me.Controls.Add(Me.txtEvent)
        Me.Controls.Add(Me.butStateDelete)
        Me.Controls.Add(Me.butStateUpdate)
        Me.Controls.Add(Me.txtStateLabel)
        Me.Controls.Add(Me.butStateAdd)
        Me.Controls.Add(Me.dgvStates)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.comTypeOwner)
        Me.Controls.Add(Me.txtState)
        Me.Controls.Add(Me.Label10)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(780, 558)
        Me.Name = "frmObjectTypes"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Object Types"
        CType(Me.dgvStates, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvEvents, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvMethods, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvObjectTypes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtState As System.Windows.Forms.TextBox
    Friend WithEvents comTypeOwner As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents dgvStates As System.Windows.Forms.DataGridView
    Friend WithEvents butStateAdd As System.Windows.Forms.Button
    Friend WithEvents txtStateLabel As System.Windows.Forms.TextBox
    Friend WithEvents butStateUpdate As System.Windows.Forms.Button
    Friend WithEvents butStateDelete As System.Windows.Forms.Button
    Friend WithEvents txtEvent As System.Windows.Forms.TextBox
    Friend WithEvents dgvProperties As System.Windows.Forms.DataGridView
    Friend WithEvents dgvEvents As System.Windows.Forms.DataGridView
    Friend WithEvents dgvMethods As System.Windows.Forms.DataGridView
    Friend WithEvents txtEventLabel As System.Windows.Forms.TextBox
    Friend WithEvents txtMethodLabel As System.Windows.Forms.TextBox
    Friend WithEvents txtMethod As System.Windows.Forms.TextBox
    Friend WithEvents butEventDelete As System.Windows.Forms.Button
    Friend WithEvents butEventUpdate As System.Windows.Forms.Button
    Friend WithEvents butEventAdd As System.Windows.Forms.Button
    Friend WithEvents butMethodDelete As System.Windows.Forms.Button
    Friend WithEvents butMethodUpdate As System.Windows.Forms.Button
    Friend WithEvents butMethodAdd As System.Windows.Forms.Button
    Friend WithEvents butPropertyDelete As System.Windows.Forms.Button
    Friend WithEvents butPropertyUpdate As System.Windows.Forms.Button
    Friend WithEvents butPropertyAdd As System.Windows.Forms.Button
    Friend WithEvents txtProperty As System.Windows.Forms.TextBox
    Friend WithEvents comPropertyType As System.Windows.Forms.ComboBox
    Friend WithEvents dgvObjectTypes As System.Windows.Forms.DataGridView
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents butObjectDelete As System.Windows.Forms.Button
    Friend WithEvents butObjectUpdate As System.Windows.Forms.Button
    Friend WithEvents txtTypeDescription As System.Windows.Forms.TextBox
    Friend WithEvents butObjectAdd As System.Windows.Forms.Button
    Friend WithEvents txtObjectType As System.Windows.Forms.TextBox
    Friend WithEvents chkOwner As System.Windows.Forms.CheckBox
    Friend WithEvents chkSystem As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents comBaseType As System.Windows.Forms.ComboBox
    Friend WithEvents butNewSubtype As System.Windows.Forms.Button
    Friend WithEvents butClose As System.Windows.Forms.Button
    Friend WithEvents butExport As System.Windows.Forms.Button
    Friend WithEvents chkContainer As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents txtParamLabel2 As System.Windows.Forms.TextBox
    Friend WithEvents txtParamLabel1 As System.Windows.Forms.TextBox
    Friend WithEvents txtParamDefault2 As System.Windows.Forms.TextBox
    Friend WithEvents txtParamDefault1 As System.Windows.Forms.TextBox
    Friend WithEvents chkHideRedundant As System.Windows.Forms.CheckBox
    Friend WithEvents base_type As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents object_type As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents is_container As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents object_type_description As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents object_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents object_type_owner As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents system_hidden As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents hide_redundant_events As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents chkTrackHistory As System.Windows.Forms.CheckBox
    Friend WithEvents txtPropertyDefault As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents btnPropOption As System.Windows.Forms.Button
    Friend WithEvents state_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents state_label As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents property_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Property_Default As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents property_datatype As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents track_history As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents event_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents event_label As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents method_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents param_1_default As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents param_2_default As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents param_1_label As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents param_2_label As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents method_label As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
