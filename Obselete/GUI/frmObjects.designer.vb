<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmObjects
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
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmObjects))
        Me.dgvObjects = New System.Windows.Forms.DataGridView()
        Me.container_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.object_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.state_label = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.address = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.object_type = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.last_updated = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.object_description = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.is_enabled = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.butObjectDelete = New System.Windows.Forms.Button()
        Me.butObjectUpdate = New System.Windows.Forms.Button()
        Me.butObjectAdd = New System.Windows.Forms.Button()
        Me.txtObject = New System.Windows.Forms.TextBox()
        Me.cboObjectTypes = New System.Windows.Forms.ComboBox()
        Me.txtObjectDescription = New System.Windows.Forms.TextBox()
        Me.lblMethods = New System.Windows.Forms.Label()
        Me.lblEvents = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.dgvProperties = New System.Windows.Forms.DataGridView()
        Me.property_name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.property_last_updated = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.property_datatype = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.property_value = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.comObjectStates = New System.Windows.Forms.ComboBox()
        Me.comObjectMethods = New System.Windows.Forms.ComboBox()
        Me.comObjectEvents = New System.Windows.Forms.ComboBox()
        Me.txtProperty = New System.Windows.Forms.TextBox()
        Me.lblProperty = New System.Windows.Forms.Label()
        Me.butPropertyUpdate = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtAddress = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.comLocation = New System.Windows.Forms.ComboBox()
        Me.butExport = New System.Windows.Forms.Button()
        Me.chkEnabled = New System.Windows.Forms.CheckBox()
        Me.file1 = New System.Windows.Forms.OpenFileDialog()
        Me.butFile = New System.Windows.Forms.Button()
        Me.grpParameters = New System.Windows.Forms.GroupBox()
        Me.btnRunMethod = New System.Windows.Forms.Button()
        Me.lblParam2 = New System.Windows.Forms.Label()
        Me.lblParam1 = New System.Windows.Forms.Label()
        Me.txtParamLabel2 = New System.Windows.Forms.TextBox()
        Me.txtParamLabel1 = New System.Windows.Forms.TextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnEditList = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.cboBoolean = New System.Windows.Forms.ComboBox()
        Me.cboContainers = New System.Windows.Forms.ComboBox()
        Me.cboTypes = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblLastUpdated = New System.Windows.Forms.Label()
        Me.cboOptions = New System.Windows.Forms.ComboBox()
        CType(Me.dgvObjects, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpParameters.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgvObjects
        '
        Me.dgvObjects.AllowUserToAddRows = False
        Me.dgvObjects.AllowUserToDeleteRows = False
        Me.dgvObjects.AllowUserToResizeColumns = False
        Me.dgvObjects.AllowUserToResizeRows = False
        Me.dgvObjects.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvObjects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvObjects.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.dgvObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvObjects.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.container_name, Me.object_name, Me.state_label, Me.address, Me.object_type, Me.last_updated, Me.object_description, Me.is_enabled})
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvObjects.DefaultCellStyle = DataGridViewCellStyle3
        Me.dgvObjects.Location = New System.Drawing.Point(0, 24)
        Me.dgvObjects.MultiSelect = False
        Me.dgvObjects.Name = "dgvObjects"
        Me.dgvObjects.ReadOnly = True
        Me.dgvObjects.RowHeadersVisible = False
        Me.dgvObjects.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvObjects.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvObjects.Size = New System.Drawing.Size(666, 444)
        Me.dgvObjects.TabIndex = 15
        '
        'container_name
        '
        Me.container_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.container_name.DataPropertyName = "container_name"
        Me.container_name.FillWeight = 78.31452!
        Me.container_name.HeaderText = "Container"
        Me.container_name.Name = "container_name"
        Me.container_name.ReadOnly = True
        Me.container_name.Width = 77
        '
        'object_name
        '
        Me.object_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.object_name.DataPropertyName = "object_name"
        Me.object_name.FillWeight = 74.23858!
        Me.object_name.HeaderText = "Object"
        Me.object_name.Name = "object_name"
        Me.object_name.ReadOnly = True
        '
        'state_label
        '
        Me.state_label.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.state_label.DataPropertyName = "state_label"
        Me.state_label.FillWeight = 78.31452!
        Me.state_label.HeaderText = "State"
        Me.state_label.Name = "state_label"
        Me.state_label.ReadOnly = True
        Me.state_label.Width = 57
        '
        'address
        '
        Me.address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.address.DataPropertyName = "address"
        Me.address.FillWeight = 67.77218!
        Me.address.HeaderText = "Address"
        Me.address.Name = "address"
        Me.address.ReadOnly = True
        Me.address.Width = 70
        '
        'object_type
        '
        Me.object_type.DataPropertyName = "object_type"
        Me.object_type.FillWeight = 78.31452!
        Me.object_type.HeaderText = "Type"
        Me.object_type.MinimumWidth = 80
        Me.object_type.Name = "object_type"
        Me.object_type.ReadOnly = True
        '
        'last_updated
        '
        Me.last_updated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader
        Me.last_updated.DataPropertyName = "last_updated"
        DataGridViewCellStyle2.Format = "MM-dd HH:mm:ss"
        DataGridViewCellStyle2.NullValue = "Never"
        Me.last_updated.DefaultCellStyle = DataGridViewCellStyle2
        Me.last_updated.HeaderText = "Updated"
        Me.last_updated.Name = "last_updated"
        Me.last_updated.ReadOnly = True
        Me.last_updated.Width = 5
        '
        'object_description
        '
        Me.object_description.DataPropertyName = "object_description"
        Me.object_description.HeaderText = "Description"
        Me.object_description.Name = "object_description"
        Me.object_description.ReadOnly = True
        Me.object_description.Visible = False
        '
        'is_enabled
        '
        Me.is_enabled.DataPropertyName = "enabled"
        Me.is_enabled.HeaderText = "enabled"
        Me.is_enabled.Name = "is_enabled"
        Me.is_enabled.ReadOnly = True
        Me.is_enabled.Visible = False
        '
        'butObjectDelete
        '
        Me.butObjectDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butObjectDelete.Enabled = False
        Me.butObjectDelete.Location = New System.Drawing.Point(446, 521)
        Me.butObjectDelete.Name = "butObjectDelete"
        Me.butObjectDelete.Size = New System.Drawing.Size(61, 26)
        Me.butObjectDelete.TabIndex = 23
        Me.butObjectDelete.Text = "Delete"
        Me.butObjectDelete.UseVisualStyleBackColor = True
        '
        'butObjectUpdate
        '
        Me.butObjectUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butObjectUpdate.Enabled = False
        Me.butObjectUpdate.Location = New System.Drawing.Point(382, 521)
        Me.butObjectUpdate.Name = "butObjectUpdate"
        Me.butObjectUpdate.Size = New System.Drawing.Size(58, 26)
        Me.butObjectUpdate.TabIndex = 22
        Me.butObjectUpdate.Text = "Update"
        Me.butObjectUpdate.UseVisualStyleBackColor = True
        '
        'butObjectAdd
        '
        Me.butObjectAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butObjectAdd.Enabled = False
        Me.butObjectAdd.Location = New System.Drawing.Point(324, 521)
        Me.butObjectAdd.Name = "butObjectAdd"
        Me.butObjectAdd.Size = New System.Drawing.Size(52, 26)
        Me.butObjectAdd.TabIndex = 20
        Me.butObjectAdd.Text = "Add"
        Me.butObjectAdd.UseVisualStyleBackColor = True
        '
        'txtObject
        '
        Me.txtObject.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtObject.Location = New System.Drawing.Point(42, 473)
        Me.txtObject.Name = "txtObject"
        Me.txtObject.Size = New System.Drawing.Size(276, 20)
        Me.txtObject.TabIndex = 19
        '
        'cboObjectTypes
        '
        Me.cboObjectTypes.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cboObjectTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboObjectTypes.FormattingEnabled = True
        Me.cboObjectTypes.Location = New System.Drawing.Point(42, 522)
        Me.cboObjectTypes.Name = "cboObjectTypes"
        Me.cboObjectTypes.Size = New System.Drawing.Size(248, 21)
        Me.cboObjectTypes.TabIndex = 24
        '
        'txtObjectDescription
        '
        Me.txtObjectDescription.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtObjectDescription.Location = New System.Drawing.Point(42, 497)
        Me.txtObjectDescription.Name = "txtObjectDescription"
        Me.txtObjectDescription.Size = New System.Drawing.Size(276, 20)
        Me.txtObjectDescription.TabIndex = 25
        '
        'lblMethods
        '
        Me.lblMethods.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMethods.AutoSize = True
        Me.lblMethods.Location = New System.Drawing.Point(702, 54)
        Me.lblMethods.Name = "lblMethods"
        Me.lblMethods.Size = New System.Drawing.Size(43, 13)
        Me.lblMethods.TabIndex = 26
        Me.lblMethods.Text = "Method"
        Me.lblMethods.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblEvents
        '
        Me.lblEvents.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblEvents.AutoSize = True
        Me.lblEvents.Location = New System.Drawing.Point(708, 78)
        Me.lblEvents.Name = "lblEvents"
        Me.lblEvents.Size = New System.Drawing.Size(35, 13)
        Me.lblEvents.TabIndex = 27
        Me.lblEvents.Text = "Event"
        Me.lblEvents.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(714, 30)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(32, 13)
        Me.Label3.TabIndex = 28
        Me.Label3.Text = "State"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'dgvProperties
        '
        Me.dgvProperties.AllowUserToAddRows = False
        Me.dgvProperties.AllowUserToDeleteRows = False
        Me.dgvProperties.AllowUserToResizeRows = False
        Me.dgvProperties.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvProperties.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.dgvProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProperties.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.property_name, Me.property_last_updated, Me.property_datatype, Me.property_value})
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvProperties.DefaultCellStyle = DataGridViewCellStyle5
        Me.dgvProperties.Location = New System.Drawing.Point(672, 102)
        Me.dgvProperties.MultiSelect = False
        Me.dgvProperties.Name = "dgvProperties"
        Me.dgvProperties.ReadOnly = True
        Me.dgvProperties.RowHeadersVisible = False
        Me.dgvProperties.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvProperties.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvProperties.Size = New System.Drawing.Size(307, 306)
        Me.dgvProperties.TabIndex = 29
        '
        'property_name
        '
        Me.property_name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.property_name.DataPropertyName = "property_name"
        Me.property_name.HeaderText = "Property"
        Me.property_name.Name = "property_name"
        Me.property_name.ReadOnly = True
        Me.property_name.Width = 71
        '
        'property_last_updated
        '
        Me.property_last_updated.DataPropertyName = "last_updated"
        Me.property_last_updated.HeaderText = "last_updated"
        Me.property_last_updated.Name = "property_last_updated"
        Me.property_last_updated.ReadOnly = True
        Me.property_last_updated.Visible = False
        '
        'property_datatype
        '
        Me.property_datatype.DataPropertyName = "property_datatype"
        Me.property_datatype.HeaderText = "property_datatype"
        Me.property_datatype.Name = "property_datatype"
        Me.property_datatype.ReadOnly = True
        Me.property_datatype.Visible = False
        '
        'property_value
        '
        Me.property_value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.property_value.DataPropertyName = "property_value"
        Me.property_value.HeaderText = "Value"
        Me.property_value.Name = "property_value"
        Me.property_value.ReadOnly = True
        '
        'comObjectStates
        '
        Me.comObjectStates.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.comObjectStates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comObjectStates.FormattingEnabled = True
        Me.comObjectStates.Location = New System.Drawing.Point(750, 24)
        Me.comObjectStates.Name = "comObjectStates"
        Me.comObjectStates.Size = New System.Drawing.Size(182, 21)
        Me.comObjectStates.TabIndex = 30
        '
        'comObjectMethods
        '
        Me.comObjectMethods.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.comObjectMethods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comObjectMethods.FormattingEnabled = True
        Me.comObjectMethods.Location = New System.Drawing.Point(750, 48)
        Me.comObjectMethods.Name = "comObjectMethods"
        Me.comObjectMethods.Size = New System.Drawing.Size(182, 21)
        Me.comObjectMethods.TabIndex = 31
        '
        'comObjectEvents
        '
        Me.comObjectEvents.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.comObjectEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comObjectEvents.FormattingEnabled = True
        Me.comObjectEvents.Location = New System.Drawing.Point(750, 72)
        Me.comObjectEvents.Name = "comObjectEvents"
        Me.comObjectEvents.Size = New System.Drawing.Size(182, 21)
        Me.comObjectEvents.TabIndex = 32
        '
        'txtProperty
        '
        Me.txtProperty.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtProperty.Location = New System.Drawing.Point(678, 426)
        Me.txtProperty.Name = "txtProperty"
        Me.txtProperty.Size = New System.Drawing.Size(266, 20)
        Me.txtProperty.TabIndex = 33
        Me.txtProperty.Visible = False
        '
        'lblProperty
        '
        Me.lblProperty.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblProperty.AutoSize = True
        Me.lblProperty.Location = New System.Drawing.Point(672, 408)
        Me.lblProperty.Name = "lblProperty"
        Me.lblProperty.Size = New System.Drawing.Size(29, 13)
        Me.lblProperty.TabIndex = 34
        Me.lblProperty.Text = "label"
        '
        'butPropertyUpdate
        '
        Me.butPropertyUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butPropertyUpdate.Location = New System.Drawing.Point(887, 474)
        Me.butPropertyUpdate.Name = "butPropertyUpdate"
        Me.butPropertyUpdate.Size = New System.Drawing.Size(58, 30)
        Me.butPropertyUpdate.TabIndex = 35
        Me.butPropertyUpdate.Text = "Update"
        Me.butPropertyUpdate.UseVisualStyleBackColor = True
        Me.butPropertyUpdate.Visible = False
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(3, 477)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(35, 13)
        Me.Label4.TabIndex = 36
        Me.Label4.Text = "Name"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 501)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(32, 13)
        Me.Label5.TabIndex = 37
        Me.Label5.Text = "Desc"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(5, 527)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(31, 13)
        Me.Label6.TabIndex = 38
        Me.Label6.Text = "Type"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label7
        '
        Me.Label7.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(324, 503)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(45, 13)
        Me.Label7.TabIndex = 39
        Me.Label7.Text = "Address"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtAddress
        '
        Me.txtAddress.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtAddress.Location = New System.Drawing.Point(378, 497)
        Me.txtAddress.Name = "txtAddress"
        Me.txtAddress.Size = New System.Drawing.Size(83, 20)
        Me.txtAddress.TabIndex = 40
        '
        'Label8
        '
        Me.Label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(324, 479)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(52, 13)
        Me.Label8.TabIndex = 41
        Me.Label8.Text = "Container"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'comLocation
        '
        Me.comLocation.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.comLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comLocation.FormattingEnabled = True
        Me.comLocation.Location = New System.Drawing.Point(378, 473)
        Me.comLocation.Name = "comLocation"
        Me.comLocation.Size = New System.Drawing.Size(190, 21)
        Me.comLocation.TabIndex = 42
        '
        'butExport
        '
        Me.butExport.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butExport.Location = New System.Drawing.Point(513, 521)
        Me.butExport.Name = "butExport"
        Me.butExport.Size = New System.Drawing.Size(53, 26)
        Me.butExport.TabIndex = 64
        Me.butExport.Text = "Export"
        Me.butExport.UseVisualStyleBackColor = True
        '
        'chkEnabled
        '
        Me.chkEnabled.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkEnabled.AutoSize = True
        Me.chkEnabled.Location = New System.Drawing.Point(492, 498)
        Me.chkEnabled.Name = "chkEnabled"
        Me.chkEnabled.Size = New System.Drawing.Size(71, 17)
        Me.chkEnabled.TabIndex = 66
        Me.chkEnabled.Text = "Enabled?"
        Me.chkEnabled.UseVisualStyleBackColor = True
        '
        'file1
        '
        Me.file1.FileName = "file1"
        '
        'butFile
        '
        Me.butFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butFile.Location = New System.Drawing.Point(950, 426)
        Me.butFile.Name = "butFile"
        Me.butFile.Size = New System.Drawing.Size(27, 20)
        Me.butFile.TabIndex = 67
        Me.butFile.Text = "..."
        Me.butFile.UseVisualStyleBackColor = True
        Me.butFile.Visible = False
        '
        'grpParameters
        '
        Me.grpParameters.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpParameters.Controls.Add(Me.btnRunMethod)
        Me.grpParameters.Controls.Add(Me.lblParam2)
        Me.grpParameters.Controls.Add(Me.lblParam1)
        Me.grpParameters.Controls.Add(Me.txtParamLabel2)
        Me.grpParameters.Controls.Add(Me.txtParamLabel1)
        Me.grpParameters.Location = New System.Drawing.Point(714, 504)
        Me.grpParameters.Name = "grpParameters"
        Me.grpParameters.Size = New System.Drawing.Size(204, 120)
        Me.grpParameters.TabIndex = 68
        Me.grpParameters.TabStop = False
        Me.grpParameters.Text = "Method Parameters"
        Me.grpParameters.Visible = False
        '
        'btnRunMethod
        '
        Me.btnRunMethod.Location = New System.Drawing.Point(54, 96)
        Me.btnRunMethod.Name = "btnRunMethod"
        Me.btnRunMethod.Size = New System.Drawing.Size(86, 21)
        Me.btnRunMethod.TabIndex = 32
        Me.btnRunMethod.Text = "Execute Method"
        Me.btnRunMethod.UseVisualStyleBackColor = True
        '
        'lblParam2
        '
        Me.lblParam2.AutoSize = True
        Me.lblParam2.Location = New System.Drawing.Point(6, 54)
        Me.lblParam2.Name = "lblParam2"
        Me.lblParam2.Size = New System.Drawing.Size(32, 13)
        Me.lblParam2.TabIndex = 31
        Me.lblParam2.Text = "State"
        Me.lblParam2.Visible = False
        '
        'lblParam1
        '
        Me.lblParam1.AutoSize = True
        Me.lblParam1.Location = New System.Drawing.Point(3, 16)
        Me.lblParam1.Name = "lblParam1"
        Me.lblParam1.Size = New System.Drawing.Size(32, 13)
        Me.lblParam1.TabIndex = 30
        Me.lblParam1.Text = "State"
        Me.lblParam1.Visible = False
        '
        'txtParamLabel2
        '
        Me.txtParamLabel2.Location = New System.Drawing.Point(6, 72)
        Me.txtParamLabel2.Name = "txtParamLabel2"
        Me.txtParamLabel2.Size = New System.Drawing.Size(192, 20)
        Me.txtParamLabel2.TabIndex = 29
        Me.txtParamLabel2.Visible = False
        '
        'txtParamLabel1
        '
        Me.txtParamLabel1.Location = New System.Drawing.Point(6, 30)
        Me.txtParamLabel1.Name = "txtParamLabel1"
        Me.txtParamLabel1.Size = New System.Drawing.Size(192, 20)
        Me.txtParamLabel1.TabIndex = 28
        Me.txtParamLabel1.Visible = False
        '
        'btnEditList
        '
        Me.btnEditList.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnEditList.Location = New System.Drawing.Point(803, 474)
        Me.btnEditList.Name = "btnEditList"
        Me.btnEditList.Size = New System.Drawing.Size(78, 30)
        Me.btnEditList.TabIndex = 70
        Me.btnEditList.Text = "Edit List"
        Me.btnEditList.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Interval = 2000
        '
        'cboBoolean
        '
        Me.cboBoolean.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboBoolean.FormattingEnabled = True
        Me.cboBoolean.Items.AddRange(New Object() {"TRUE", "FALSE"})
        Me.cboBoolean.Location = New System.Drawing.Point(678, 426)
        Me.cboBoolean.Name = "cboBoolean"
        Me.cboBoolean.Size = New System.Drawing.Size(102, 21)
        Me.cboBoolean.TabIndex = 71
        Me.cboBoolean.Visible = False
        '
        'cboContainers
        '
        Me.cboContainers.FormattingEnabled = True
        Me.cboContainers.Location = New System.Drawing.Point(0, 0)
        Me.cboContainers.Name = "cboContainers"
        Me.cboContainers.Size = New System.Drawing.Size(132, 21)
        Me.cboContainers.TabIndex = 72
        '
        'cboTypes
        '
        Me.cboTypes.FormattingEnabled = True
        Me.cboTypes.Location = New System.Drawing.Point(276, 0)
        Me.cboTypes.Name = "cboTypes"
        Me.cboTypes.Size = New System.Drawing.Size(114, 21)
        Me.cboTypes.TabIndex = 73
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(743, 456)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(91, 13)
        Me.Label1.TabIndex = 74
        Me.Label1.Text = "Last Updated On:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblLastUpdated
        '
        Me.lblLastUpdated.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLastUpdated.AutoSize = True
        Me.lblLastUpdated.Location = New System.Drawing.Point(833, 456)
        Me.lblLastUpdated.Name = "lblLastUpdated"
        Me.lblLastUpdated.Size = New System.Drawing.Size(53, 13)
        Me.lblLastUpdated.TabIndex = 75
        Me.lblLastUpdated.Text = "DateTime"
        Me.lblLastUpdated.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboOptions
        '
        Me.cboOptions.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboOptions.FormattingEnabled = True
        Me.cboOptions.Location = New System.Drawing.Point(678, 425)
        Me.cboOptions.Name = "cboOptions"
        Me.cboOptions.Size = New System.Drawing.Size(145, 21)
        Me.cboOptions.TabIndex = 76
        Me.cboOptions.Visible = False
        '
        'frmObjects
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(984, 547)
        Me.Controls.Add(Me.cboOptions)
        Me.Controls.Add(Me.lblLastUpdated)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cboTypes)
        Me.Controls.Add(Me.cboContainers)
        Me.Controls.Add(Me.cboBoolean)
        Me.Controls.Add(Me.btnEditList)
        Me.Controls.Add(Me.grpParameters)
        Me.Controls.Add(Me.butFile)
        Me.Controls.Add(Me.chkEnabled)
        Me.Controls.Add(Me.butExport)
        Me.Controls.Add(Me.comLocation)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.txtAddress)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.butPropertyUpdate)
        Me.Controls.Add(Me.lblProperty)
        Me.Controls.Add(Me.txtProperty)
        Me.Controls.Add(Me.comObjectEvents)
        Me.Controls.Add(Me.comObjectMethods)
        Me.Controls.Add(Me.comObjectStates)
        Me.Controls.Add(Me.dgvProperties)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lblEvents)
        Me.Controls.Add(Me.lblMethods)
        Me.Controls.Add(Me.txtObjectDescription)
        Me.Controls.Add(Me.cboObjectTypes)
        Me.Controls.Add(Me.butObjectDelete)
        Me.Controls.Add(Me.butObjectUpdate)
        Me.Controls.Add(Me.butObjectAdd)
        Me.Controls.Add(Me.txtObject)
        Me.Controls.Add(Me.dgvObjects)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(795, 559)
        Me.Name = "frmObjects"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Objects"
        CType(Me.dgvObjects, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvProperties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpParameters.ResumeLayout(False)
        Me.grpParameters.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dgvObjects As System.Windows.Forms.DataGridView
    Friend WithEvents butObjectDelete As System.Windows.Forms.Button
    Friend WithEvents butObjectUpdate As System.Windows.Forms.Button
    Friend WithEvents butObjectAdd As System.Windows.Forms.Button
    Friend WithEvents txtObject As System.Windows.Forms.TextBox
    Friend WithEvents cboObjectTypes As System.Windows.Forms.ComboBox
    Friend WithEvents txtObjectDescription As System.Windows.Forms.TextBox
    Friend WithEvents lblMethods As System.Windows.Forms.Label
    Friend WithEvents lblEvents As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents dgvProperties As System.Windows.Forms.DataGridView
    Friend WithEvents comObjectStates As System.Windows.Forms.ComboBox
    Friend WithEvents comObjectMethods As System.Windows.Forms.ComboBox
    Friend WithEvents comObjectEvents As System.Windows.Forms.ComboBox
    Friend WithEvents txtProperty As System.Windows.Forms.TextBox
    Friend WithEvents lblProperty As System.Windows.Forms.Label
    Friend WithEvents butPropertyUpdate As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtAddress As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents comLocation As System.Windows.Forms.ComboBox
    Friend WithEvents butExport As System.Windows.Forms.Button
    Friend WithEvents chkEnabled As System.Windows.Forms.CheckBox
    Friend WithEvents file1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents butFile As System.Windows.Forms.Button
    Friend WithEvents grpParameters As System.Windows.Forms.GroupBox
    Friend WithEvents lblParam2 As System.Windows.Forms.Label
    Friend WithEvents lblParam1 As System.Windows.Forms.Label
    Friend WithEvents txtParamLabel2 As System.Windows.Forms.TextBox
    Friend WithEvents txtParamLabel1 As System.Windows.Forms.TextBox
    Friend WithEvents btnRunMethod As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents btnEditList As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents cboBoolean As System.Windows.Forms.ComboBox
    Friend WithEvents cboContainers As System.Windows.Forms.ComboBox
    Friend WithEvents cboTypes As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblLastUpdated As System.Windows.Forms.Label
    Friend WithEvents property_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents property_last_updated As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents property_datatype As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents property_value As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents cboOptions As System.Windows.Forms.ComboBox
    Friend WithEvents container_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents object_name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents state_label As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents address As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents object_type As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents last_updated As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents object_description As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents is_enabled As System.Windows.Forms.DataGridViewTextBoxColumn

End Class
