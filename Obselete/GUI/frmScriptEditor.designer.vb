<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmScriptEditor
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmScriptEditor))
        Me.lstResults = New System.Windows.Forms.ListBox()
        Me.btnRunScript = New System.Windows.Forms.Button()
        Me.txtX = New System.Windows.Forms.TextBox()
        Me.txtY = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtWord = New System.Windows.Forms.TextBox()
        Me.txtObject = New System.Windows.Forms.TextBox()
        Me.lstObjects = New System.Windows.Forms.ListBox()
        Me.lstSet = New System.Windows.Forms.ListBox()
        Me.lstOptions = New System.Windows.Forms.ListBox()
        Me.comObject = New System.Windows.Forms.ComboBox()
        Me.comObjectEvents = New System.Windows.Forms.ComboBox()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.txtSet = New System.Windows.Forms.TextBox()
        Me.txtOption = New System.Windows.Forms.TextBox()
        Me.txtParam2 = New System.Windows.Forms.TextBox()
        Me.txtParam1 = New System.Windows.Forms.TextBox()
        Me.txtType = New System.Windows.Forms.TextBox()
        Me.txtParam1Pos = New System.Windows.Forms.TextBox()
        Me.txtParam2Pos = New System.Windows.Forms.TextBox()
        Me.txtOptionPos = New System.Windows.Forms.TextBox()
        Me.txtSetPos = New System.Windows.Forms.TextBox()
        Me.txtObjectPos1 = New System.Windows.Forms.TextBox()
        Me.lstParam1 = New System.Windows.Forms.ListBox()
        Me.lstParam2 = New System.Windows.Forms.ListBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.radEvent = New System.Windows.Forms.RadioButton()
        Me.radPattern = New System.Windows.Forms.RadioButton()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.grpEvent = New System.Windows.Forms.GroupBox()
        Me.cboPatterns = New System.Windows.Forms.ComboBox()
        Me.lblPattern = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtTestParameter = New System.Windows.Forms.TextBox()
        Me.txtScript = New System.Windows.Forms.TextBox()
        Me.txtCurrentLine = New System.Windows.Forms.TextBox()
        Me.txtPosition = New System.Windows.Forms.TextBox()
        Me.txtEnd = New System.Windows.Forms.TextBox()
        Me.txtSize = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtObjectPos2 = New System.Windows.Forms.TextBox()
        Me.lstLine = New System.Windows.Forms.ListBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtStart = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.txtEcho = New System.Windows.Forms.TextBox()
        Me.grpEvent.SuspendLayout()
        Me.SuspendLayout()
        '
        'lstResults
        '
        Me.lstResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lstResults.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstResults.FormattingEnabled = True
        Me.lstResults.ItemHeight = 16
        Me.lstResults.Location = New System.Drawing.Point(552, 406)
        Me.lstResults.Name = "lstResults"
        Me.lstResults.Size = New System.Drawing.Size(496, 130)
        Me.lstResults.TabIndex = 1
        '
        'btnRunScript
        '
        Me.btnRunScript.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRunScript.Location = New System.Drawing.Point(966, 371)
        Me.btnRunScript.Name = "btnRunScript"
        Me.btnRunScript.Size = New System.Drawing.Size(79, 30)
        Me.btnRunScript.TabIndex = 2
        Me.btnRunScript.Text = "Run Script"
        Me.btnRunScript.UseVisualStyleBackColor = True
        '
        'txtX
        '
        Me.txtX.Location = New System.Drawing.Point(797, 681)
        Me.txtX.Name = "txtX"
        Me.txtX.Size = New System.Drawing.Size(49, 20)
        Me.txtX.TabIndex = 4
        '
        'txtY
        '
        Me.txtY.Location = New System.Drawing.Point(291, 610)
        Me.txtY.Name = "txtY"
        Me.txtY.Size = New System.Drawing.Size(49, 20)
        Me.txtY.TabIndex = 5
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(852, 684)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(14, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Y"
        '
        'txtWord
        '
        Me.txtWord.Location = New System.Drawing.Point(117, 633)
        Me.txtWord.Name = "txtWord"
        Me.txtWord.Size = New System.Drawing.Size(234, 20)
        Me.txtWord.TabIndex = 8
        '
        'txtObject
        '
        Me.txtObject.Location = New System.Drawing.Point(3, 677)
        Me.txtObject.Name = "txtObject"
        Me.txtObject.Size = New System.Drawing.Size(189, 20)
        Me.txtObject.TabIndex = 9
        '
        'lstObjects
        '
        Me.lstObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lstObjects.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstObjects.FormattingEnabled = True
        Me.lstObjects.ItemHeight = 17
        Me.lstObjects.Location = New System.Drawing.Point(4, 726)
        Me.lstObjects.Name = "lstObjects"
        Me.lstObjects.Size = New System.Drawing.Size(188, 36)
        Me.lstObjects.TabIndex = 10
        Me.lstObjects.Visible = False
        '
        'lstSet
        '
        Me.lstSet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lstSet.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstSet.FormattingEnabled = True
        Me.lstSet.ItemHeight = 17
        Me.lstSet.Location = New System.Drawing.Point(199, 725)
        Me.lstSet.Name = "lstSet"
        Me.lstSet.Size = New System.Drawing.Size(93, 36)
        Me.lstSet.TabIndex = 11
        Me.lstSet.Visible = False
        '
        'lstOptions
        '
        Me.lstOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lstOptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstOptions.FormattingEnabled = True
        Me.lstOptions.ItemHeight = 17
        Me.lstOptions.Location = New System.Drawing.Point(298, 726)
        Me.lstOptions.Name = "lstOptions"
        Me.lstOptions.Size = New System.Drawing.Size(176, 36)
        Me.lstOptions.TabIndex = 17
        Me.lstOptions.Visible = False
        '
        'comObject
        '
        Me.comObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comObject.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.comObject.FormattingEnabled = True
        Me.comObject.Location = New System.Drawing.Point(59, 23)
        Me.comObject.Name = "comObject"
        Me.comObject.Size = New System.Drawing.Size(206, 24)
        Me.comObject.TabIndex = 43
        '
        'comObjectEvents
        '
        Me.comObjectEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comObjectEvents.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.comObjectEvents.FormattingEnabled = True
        Me.comObjectEvents.Location = New System.Drawing.Point(321, 21)
        Me.comObjectEvents.Name = "comObjectEvents"
        Me.comObjectEvents.Size = New System.Drawing.Size(202, 24)
        Me.comObjectEvents.TabIndex = 44
        '
        'btnUpdate
        '
        Me.btnUpdate.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(74, 370)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(73, 30)
        Me.btnUpdate.TabIndex = 45
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        Me.btnUpdate.Visible = False
        '
        'btnAdd
        '
        Me.btnAdd.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAdd.Location = New System.Drawing.Point(3, 370)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(61, 30)
        Me.btnAdd.TabIndex = 51
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseVisualStyleBackColor = True
        Me.btnAdd.Visible = False
        '
        'btnDelete
        '
        Me.btnDelete.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(156, 370)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(69, 30)
        Me.btnDelete.TabIndex = 52
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        Me.btnDelete.Visible = False
        '
        'txtSet
        '
        Me.txtSet.Location = New System.Drawing.Point(199, 677)
        Me.txtSet.Name = "txtSet"
        Me.txtSet.Size = New System.Drawing.Size(93, 20)
        Me.txtSet.TabIndex = 53
        '
        'txtOption
        '
        Me.txtOption.Location = New System.Drawing.Point(298, 677)
        Me.txtOption.Name = "txtOption"
        Me.txtOption.Size = New System.Drawing.Size(176, 20)
        Me.txtOption.TabIndex = 54
        '
        'txtParam2
        '
        Me.txtParam2.Location = New System.Drawing.Point(595, 677)
        Me.txtParam2.Name = "txtParam2"
        Me.txtParam2.Size = New System.Drawing.Size(93, 20)
        Me.txtParam2.TabIndex = 55
        '
        'txtParam1
        '
        Me.txtParam1.Location = New System.Drawing.Point(480, 677)
        Me.txtParam1.Name = "txtParam1"
        Me.txtParam1.Size = New System.Drawing.Size(109, 20)
        Me.txtParam1.TabIndex = 56
        '
        'txtType
        '
        Me.txtType.Location = New System.Drawing.Point(434, 633)
        Me.txtType.Name = "txtType"
        Me.txtType.Size = New System.Drawing.Size(106, 20)
        Me.txtType.TabIndex = 57
        '
        'txtParam1Pos
        '
        Me.txtParam1Pos.Location = New System.Drawing.Point(544, 700)
        Me.txtParam1Pos.Name = "txtParam1Pos"
        Me.txtParam1Pos.Size = New System.Drawing.Size(45, 20)
        Me.txtParam1Pos.TabIndex = 62
        '
        'txtParam2Pos
        '
        Me.txtParam2Pos.Location = New System.Drawing.Point(648, 700)
        Me.txtParam2Pos.Name = "txtParam2Pos"
        Me.txtParam2Pos.Size = New System.Drawing.Size(40, 20)
        Me.txtParam2Pos.TabIndex = 61
        '
        'txtOptionPos
        '
        Me.txtOptionPos.Location = New System.Drawing.Point(422, 700)
        Me.txtOptionPos.Name = "txtOptionPos"
        Me.txtOptionPos.Size = New System.Drawing.Size(52, 20)
        Me.txtOptionPos.TabIndex = 60
        '
        'txtSetPos
        '
        Me.txtSetPos.Location = New System.Drawing.Point(253, 699)
        Me.txtSetPos.Name = "txtSetPos"
        Me.txtSetPos.Size = New System.Drawing.Size(39, 20)
        Me.txtSetPos.TabIndex = 59
        '
        'txtObjectPos1
        '
        Me.txtObjectPos1.Location = New System.Drawing.Point(53, 699)
        Me.txtObjectPos1.Name = "txtObjectPos1"
        Me.txtObjectPos1.Size = New System.Drawing.Size(58, 20)
        Me.txtObjectPos1.TabIndex = 58
        '
        'lstParam1
        '
        Me.lstParam1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lstParam1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstParam1.FormattingEnabled = True
        Me.lstParam1.ItemHeight = 17
        Me.lstParam1.Location = New System.Drawing.Point(480, 726)
        Me.lstParam1.Name = "lstParam1"
        Me.lstParam1.Size = New System.Drawing.Size(109, 36)
        Me.lstParam1.TabIndex = 63
        Me.lstParam1.Visible = False
        '
        'lstParam2
        '
        Me.lstParam2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lstParam2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstParam2.FormattingEnabled = True
        Me.lstParam2.ItemHeight = 17
        Me.lstParam2.Location = New System.Drawing.Point(595, 726)
        Me.lstParam2.Name = "lstParam2"
        Me.lstParam2.Size = New System.Drawing.Size(93, 36)
        Me.lstParam2.TabIndex = 64
        Me.lstParam2.Visible = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(6, 24)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(47, 16)
        Me.Label3.TabIndex = 65
        Me.Label3.Text = "Object"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(275, 24)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(42, 16)
        Me.Label4.TabIndex = 66
        Me.Label4.Text = "Event"
        '
        'radEvent
        '
        Me.radEvent.AutoSize = True
        Me.radEvent.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.radEvent.Location = New System.Drawing.Point(165, 347)
        Me.radEvent.Name = "radEvent"
        Me.radEvent.Size = New System.Drawing.Size(60, 20)
        Me.radEvent.TabIndex = 67
        Me.radEvent.Text = "Event"
        Me.radEvent.UseVisualStyleBackColor = True
        '
        'radPattern
        '
        Me.radPattern.AutoSize = True
        Me.radPattern.Checked = True
        Me.radPattern.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.radPattern.Location = New System.Drawing.Point(88, 347)
        Me.radPattern.Name = "radPattern"
        Me.radPattern.Size = New System.Drawing.Size(71, 20)
        Me.radPattern.TabIndex = 68
        Me.radPattern.TabStop = True
        Me.radPattern.Text = "Named"
        Me.radPattern.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(2, 347)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(80, 16)
        Me.Label5.TabIndex = 69
        Me.Label5.Text = "Script Type:"
        '
        'grpEvent
        '
        Me.grpEvent.Controls.Add(Me.Label4)
        Me.grpEvent.Controls.Add(Me.Label3)
        Me.grpEvent.Controls.Add(Me.comObjectEvents)
        Me.grpEvent.Controls.Add(Me.comObject)
        Me.grpEvent.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpEvent.Location = New System.Drawing.Point(231, 348)
        Me.grpEvent.Name = "grpEvent"
        Me.grpEvent.Size = New System.Drawing.Size(529, 53)
        Me.grpEvent.TabIndex = 70
        Me.grpEvent.TabStop = False
        Me.grpEvent.Text = "Attach Script to This Event"
        '
        'cboPatterns
        '
        Me.cboPatterns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboPatterns.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboPatterns.FormattingEnabled = True
        Me.cboPatterns.Location = New System.Drawing.Point(329, 365)
        Me.cboPatterns.Name = "cboPatterns"
        Me.cboPatterns.Size = New System.Drawing.Size(218, 24)
        Me.cboPatterns.TabIndex = 71
        '
        'lblPattern
        '
        Me.lblPattern.AutoSize = True
        Me.lblPattern.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPattern.Location = New System.Drawing.Point(240, 368)
        Me.lblPattern.Name = "lblPattern"
        Me.lblPattern.Size = New System.Drawing.Size(83, 16)
        Me.lblPattern.TabIndex = 73
        Me.lblPattern.Text = "Select Script"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(794, 355)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(130, 16)
        Me.Label6.TabIndex = 74
        Me.Label6.Text = "Run With Parameter:"
        '
        'txtTestParameter
        '
        Me.txtTestParameter.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtTestParameter.Location = New System.Drawing.Point(766, 374)
        Me.txtTestParameter.Name = "txtTestParameter"
        Me.txtTestParameter.Size = New System.Drawing.Size(194, 22)
        Me.txtTestParameter.TabIndex = 75
        '
        'txtScript
        '
        Me.txtScript.AcceptsReturn = True
        Me.txtScript.AcceptsTab = True
        Me.txtScript.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtScript.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtScript.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtScript.Location = New System.Drawing.Point(39, 3)
        Me.txtScript.Multiline = True
        Me.txtScript.Name = "txtScript"
        Me.txtScript.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtScript.Size = New System.Drawing.Size(1006, 334)
        Me.txtScript.TabIndex = 76
        '
        'txtCurrentLine
        '
        Me.txtCurrentLine.Location = New System.Drawing.Point(117, 558)
        Me.txtCurrentLine.Name = "txtCurrentLine"
        Me.txtCurrentLine.Size = New System.Drawing.Size(795, 20)
        Me.txtCurrentLine.TabIndex = 77
        '
        'txtPosition
        '
        Me.txtPosition.Location = New System.Drawing.Point(117, 608)
        Me.txtPosition.Name = "txtPosition"
        Me.txtPosition.Size = New System.Drawing.Size(45, 20)
        Me.txtPosition.TabIndex = 78
        '
        'txtEnd
        '
        Me.txtEnd.Location = New System.Drawing.Point(291, 584)
        Me.txtEnd.Name = "txtEnd"
        Me.txtEnd.Size = New System.Drawing.Size(58, 20)
        Me.txtEnd.TabIndex = 81
        '
        'txtSize
        '
        Me.txtSize.Location = New System.Drawing.Point(918, 558)
        Me.txtSize.Name = "txtSize"
        Me.txtSize.Size = New System.Drawing.Size(52, 20)
        Me.txtSize.TabIndex = 83
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(13, 611)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(98, 13)
        Me.Label8.TabIndex = 84
        Me.Label8.Text = "Cursor is at position"
        '
        'txtObjectPos2
        '
        Me.txtObjectPos2.Location = New System.Drawing.Point(135, 699)
        Me.txtObjectPos2.Name = "txtObjectPos2"
        Me.txtObjectPos2.Size = New System.Drawing.Size(58, 20)
        Me.txtObjectPos2.TabIndex = 85
        '
        'lstLine
        '
        Me.lstLine.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstLine.FormattingEnabled = True
        Me.lstLine.ItemHeight = 15
        Me.lstLine.Location = New System.Drawing.Point(-25, 3)
        Me.lstLine.Name = "lstLine"
        Me.lstLine.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lstLine.Size = New System.Drawing.Size(65, 319)
        Me.lstLine.TabIndex = 86
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(2, 587)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(109, 13)
        Me.Label9.TabIndex = 87
        Me.Label9.Text = "Line Starts at Position"
        '
        'txtStart
        '
        Me.txtStart.Location = New System.Drawing.Point(117, 584)
        Me.txtStart.Name = "txtStart"
        Me.txtStart.Size = New System.Drawing.Size(45, 20)
        Me.txtStart.TabIndex = 80
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(182, 587)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(106, 13)
        Me.Label10.TabIndex = 88
        Me.Label10.Text = "Line Ends at Position"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(47, 561)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(64, 13)
        Me.Label11.TabIndex = 89
        Me.Label11.Text = "Current Line"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(976, 561)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(57, 13)
        Me.Label12.TabIndex = 90
        Me.Label12.Text = "characters"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(240, 611)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(42, 13)
        Me.Label13.TabIndex = 91
        Me.Label13.Text = "on Line"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(45, 636)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(60, 13)
        Me.Label14.TabIndex = 92
        Me.Label14.Text = "Word Type"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(374, 636)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(54, 13)
        Me.Label15.TabIndex = 93
        Me.Label15.Text = "Line Type"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(71, 661)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(38, 13)
        Me.Label16.TabIndex = 94
        Me.Label16.Text = "Object"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(9, 703)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(30, 13)
        Me.Label17.TabIndex = 95
        Me.Label17.Text = "From"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(114, 702)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(20, 13)
        Me.Label18.TabIndex = 96
        Me.Label18.Text = "To"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(227, 703)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(20, 13)
        Me.Label19.TabIndex = 97
        Me.Label19.Text = "To"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(396, 702)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(20, 13)
        Me.Label20.TabIndex = 98
        Me.Label20.Text = "To"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(217, 661)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(65, 13)
        Me.Label1.TabIndex = 99
        Me.Label1.Text = "Option Type"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(356, 661)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(38, 13)
        Me.Label21.TabIndex = 100
        Me.Label21.Text = "Option"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(502, 661)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(64, 13)
        Me.Label22.TabIndex = 101
        Me.Label22.Text = "Parameter 1"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(604, 661)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(64, 13)
        Me.Label23.TabIndex = 102
        Me.Label23.Text = "Parameter 2"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(518, 703)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(20, 13)
        Me.Label24.TabIndex = 103
        Me.Label24.Text = "To"
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(622, 703)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(20, 13)
        Me.Label25.TabIndex = 104
        Me.Label25.Text = "To"
        '
        'txtEcho
        '
        Me.txtEcho.Location = New System.Drawing.Point(5, 406)
        Me.txtEcho.Multiline = True
        Me.txtEcho.Name = "txtEcho"
        Me.txtEcho.Size = New System.Drawing.Size(541, 130)
        Me.txtEcho.TabIndex = 105
        '
        'frmScriptEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.LightGray
        Me.ClientSize = New System.Drawing.Size(1052, 557)
        Me.Controls.Add(Me.txtEcho)
        Me.Controls.Add(Me.Label25)
        Me.Controls.Add(Me.Label24)
        Me.Controls.Add(Me.Label23)
        Me.Controls.Add(Me.cboPatterns)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.lblPattern)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.txtObjectPos2)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.txtSize)
        Me.Controls.Add(Me.txtEnd)
        Me.Controls.Add(Me.txtStart)
        Me.Controls.Add(Me.lstObjects)
        Me.Controls.Add(Me.txtPosition)
        Me.Controls.Add(Me.txtCurrentLine)
        Me.Controls.Add(Me.txtTestParameter)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.grpEvent)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.radPattern)
        Me.Controls.Add(Me.radEvent)
        Me.Controls.Add(Me.lstParam2)
        Me.Controls.Add(Me.lstParam1)
        Me.Controls.Add(Me.txtParam1Pos)
        Me.Controls.Add(Me.txtParam2Pos)
        Me.Controls.Add(Me.txtOptionPos)
        Me.Controls.Add(Me.txtSetPos)
        Me.Controls.Add(Me.txtObjectPos1)
        Me.Controls.Add(Me.txtType)
        Me.Controls.Add(Me.txtParam1)
        Me.Controls.Add(Me.txtParam2)
        Me.Controls.Add(Me.txtOption)
        Me.Controls.Add(Me.txtSet)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.lstOptions)
        Me.Controls.Add(Me.lstSet)
        Me.Controls.Add(Me.txtObject)
        Me.Controls.Add(Me.txtWord)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtY)
        Me.Controls.Add(Me.txtX)
        Me.Controls.Add(Me.btnRunScript)
        Me.Controls.Add(Me.lstResults)
        Me.Controls.Add(Me.txtScript)
        Me.Controls.Add(Me.lstLine)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmScriptEditor"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Script Editor"
        Me.grpEvent.ResumeLayout(False)
        Me.grpEvent.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lstResults As System.Windows.Forms.ListBox
    Friend WithEvents btnRunScript As System.Windows.Forms.Button
    Friend WithEvents txtX As System.Windows.Forms.TextBox
    Friend WithEvents txtY As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtWord As System.Windows.Forms.TextBox
    Friend WithEvents txtObject As System.Windows.Forms.TextBox
    Friend WithEvents lstObjects As System.Windows.Forms.ListBox
    Friend WithEvents lstSet As System.Windows.Forms.ListBox
    Friend WithEvents lstOptions As System.Windows.Forms.ListBox
    Friend WithEvents comObject As System.Windows.Forms.ComboBox
    Friend WithEvents comObjectEvents As System.Windows.Forms.ComboBox
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents txtSet As System.Windows.Forms.TextBox
    Friend WithEvents txtOption As System.Windows.Forms.TextBox
    Friend WithEvents txtParam2 As System.Windows.Forms.TextBox
    Friend WithEvents txtParam1 As System.Windows.Forms.TextBox
    Friend WithEvents txtType As System.Windows.Forms.TextBox
    Friend WithEvents txtParam1Pos As System.Windows.Forms.TextBox
    Friend WithEvents txtParam2Pos As System.Windows.Forms.TextBox
    Friend WithEvents txtOptionPos As System.Windows.Forms.TextBox
    Friend WithEvents txtSetPos As System.Windows.Forms.TextBox
    Friend WithEvents txtObjectPos1 As System.Windows.Forms.TextBox
    Friend WithEvents lstParam1 As System.Windows.Forms.ListBox
    Friend WithEvents lstParam2 As System.Windows.Forms.ListBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents radEvent As System.Windows.Forms.RadioButton
    Friend WithEvents radPattern As System.Windows.Forms.RadioButton
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents grpEvent As System.Windows.Forms.GroupBox
    Friend WithEvents cboPatterns As System.Windows.Forms.ComboBox
    Friend WithEvents lblPattern As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtTestParameter As System.Windows.Forms.TextBox
    Friend WithEvents txtScript As System.Windows.Forms.TextBox
    Friend WithEvents txtCurrentLine As System.Windows.Forms.TextBox
    Friend WithEvents txtPosition As System.Windows.Forms.TextBox
    Friend WithEvents txtEnd As System.Windows.Forms.TextBox
    Friend WithEvents txtSize As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtObjectPos2 As System.Windows.Forms.TextBox
    Friend WithEvents lstLine As System.Windows.Forms.ListBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtStart As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents txtEcho As System.Windows.Forms.TextBox
End Class
