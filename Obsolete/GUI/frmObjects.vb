Imports MySql.Data.MySqlClient
Public Class frmObjects
    Private CN As MySqlConnection
    Private flgLoadingStates As Integer
    Private sRefresh As String = "N"
    Dim dtObject As New DataTable
    Dim daObject As New MySqlDataAdapter
    Dim iRowHolder As Integer
    Dim iScrollPosition As Integer
    Private WithEvents bsObject As BindingSource

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' OSAEApi = New OSAE.OSAE("Dev GUI")
        DB_Connection()
        Load_Object_Types()
        Load_Objects()
        Load_Container_Filter()
        grpParameters.Top = 106
        cboContainers.Width = dgvObjects.Columns("container_name").Width
        cboTypes.Width = dgvObjects.Columns("object_type").Width
        cboTypes.Left = (dgvObjects.Columns("container_name").Width + dgvObjects.Columns("object_name").Width)
    End Sub
    Public Sub DB_Connection()
        CN = New MySqlConnection
        CN.ConnectionString = "server=" & OSAEApi.DBConnection & ";Port=" & OSAEApi.DBPort & ";Database=" & OSAEApi.DBName & ";Password=" & OSAEApi.DBPassword & ";use procedure bodies=false;Persist Security Info=True;User ID=" & OSAEApi.DBUsername
        Try
            CN.Open()
            CN.Close()
            logging.AddToLog("Connected to Database: " & OSAEApi.DBName & " @ " & OSAEApi.DBConnection & ":" & OSAEApi.DBPort, True)
        Catch myerror As Exception
            logging.AddToLog("Error Connecting to Database: " & myerror.Message, True)
        End Try
    End Sub

    Public Sub Load_Objects()
        'If sRefresh = "Y" Then Exit Sub
        Dim sSQL As String = ""
        If dgvObjects.RowCount > 0 Then
            butObjectDelete.Enabled = True
            iRowHolder = dgvObjects.CurrentCell.RowIndex
            iScrollPosition = dgvObjects.FirstDisplayedScrollingRowIndex

        Else
            butObjectDelete.Enabled = False
        End If

        sSQL = "SELECT object_name,object_type,object_description,state_label,address,container_name,enabled,last_updated FROM osae_v_object"
        If cboContainers.Text <> " All Containers" And cboContainers.Text <> "" Then
            sSQL += " WHERE container_name='" & cboContainers.Text & "'"
            'iRowHolder = -1
        ElseIf cboTypes.Text <> " All Types" And cboTypes.Text <> "" Then
            sSQL += " WHERE object_type='" & cboTypes.Text & "'"
            'iRowHolder = -1
        End If
        sSQL += " ORDER BY container_name,object_name"
        bsObject = New BindingSource
        daObject = New MySqlDataAdapter(sSQL, CN)
        daObject.Update(dtObject)
        dtObject.Rows.Clear()
        daObject.Fill(dtObject)

        bsObject.DataSource = dtObject
        dgvObjects.DataSource = bsObject

        If iRowHolder >= dgvObjects.RowCount Then iRowHolder = dgvObjects.RowCount - 1
        If iRowHolder >= 0 Then
            dgvObjects.CurrentCell = dgvObjects.Rows(iRowHolder).Cells("object_name")
            dgvObjects.FirstDisplayedScrollingRowIndex = iScrollPosition
        End If

        dgvObjects.Columns("last_updated").DefaultCellStyle.Format = "MM-dd HH:mm:ss"

    End Sub

    Public Sub Update_Objects()
        'If dgvObjects.RowCount > 0 Then
        '    iRowHolder = dgvObjects.CurrentCell.RowIndex
        'End If
        'daObject.Update(dtObject)
        'dtObject.Rows.Clear()
        'daObject.Fill(dtObject)
        'dgvObjects.Refresh()
        'dgvObjects.CurrentCell = dgvObjects.Rows(iRowHolder).Cells("object_name")
        ' dgvObjects.DataSource = dtObject
        'Update the SQL DataSource and RadGridView
        'MainForm.daBookings.Update(MainForm.DS.Tables("Bookings"))
        'MainForm.DS.Tables("Bookings").Rows.Clear()
        'MainForm.daBookings.Fill(MainForm.DS, "Bookings")
        'Me.rgvBookings.DataSource = MainForm.DS.Tables("Bookings")
        'Me.rgvBookings.EnableFiltering = True

    End Sub

    Public Sub Load_Object_Types()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        sRefresh = "Y"
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_type FROM osae_object_type ORDER BY object_type"
        Try
            cboObjectTypes.Items.Clear()
            cboTypes.Items.Clear()
            cboTypes.Items.Add(" All Types")
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboObjectTypes.Items.Add(Convert.ToString(myReader.Item("object_type")))
                cboTypes.Items.Add(Convert.ToString(myReader.Item("object_type")))
            End While
            CN.Close()
            cboTypes.Text = " All Types"
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_Types: " & myerror.Message)
            CN.Close()
        End Try
        sRefresh = "N"
    End Sub

    Private Sub dgvObjects_CellMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgvObjects.CellMouseDoubleClick
        Load_Objects()
    End Sub

    Private Sub dgvObjects_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvObjects.CurrentCellChanged
        If sRefresh = "Y" Then Exit Sub
        Try
            txtObject.Text = dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value
            cboObjectTypes.Text = "" & dgvObjects("object_type", dgvObjects.CurrentCell.RowIndex).Value
            chkEnabled.Checked = dgvObjects("is_enabled", dgvObjects.CurrentCell.RowIndex).Value
            txtObjectDescription.Text = "" & dgvObjects("object_description", dgvObjects.CurrentCell.RowIndex).Value
            txtAddress.Text = "" & dgvObjects("address", dgvObjects.CurrentCell.RowIndex).Value
            Load_Object_States()
            Load_Object_Methods()
            Load_Object_Events()
            Load_Object_Properties()
            Load_Containers()
            butObjectDelete.Enabled = True
            Validate_Object()
        Catch ex As Exception
            'MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub dgvProperties_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvProperties.CurrentCellChanged
        Try
            lblProperty.Text = "" & dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value
            lblLastUpdated.Text = "" & dgvProperties("property_last_updated", dgvProperties.CurrentCell.RowIndex).Value
            txtProperty.Text = "" & dgvProperties("property_value", dgvProperties.CurrentCell.RowIndex).Value
            If dgvProperties("property_datatype", dgvProperties.CurrentCell.RowIndex).Value = "File" Then
                butFile.Visible = True
            Else
                butFile.Visible = False
            End If
            
            Dim list As DataSet = OSAEApi.GetObjectTypePropertyOptions(dgvObjects("object_type", dgvObjects.CurrentCell.RowIndex).Value, dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value)
            Dim listValues As DataSet = OSAEApi.ObjectPropertyArrayGetAll(dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value, dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value)

            If dgvProperties("property_datatype", dgvProperties.CurrentCell.RowIndex).Value = "List" Then
                btnEditList.Visible = True
                txtProperty.Visible = False
            ElseIf dgvProperties("property_datatype", dgvProperties.CurrentCell.RowIndex).Value = "ListSelection" Then

                btnEditList.Visible = False
                txtProperty.Visible = False

            ElseIf dgvProperties("property_datatype", dgvProperties.CurrentCell.RowIndex).Value = "Boolean" Then
                cboBoolean.Visible = True
                cboBoolean.Text = "" & dgvProperties("property_value", dgvProperties.CurrentCell.RowIndex).Value
                txtProperty.Visible = False
                cboOptions.Visible = False
            ElseIf list.Tables(0).Rows.Count > 0 Then
                cboOptions.Items.Clear()
                For Each dr As DataRow In list.Tables(0).Rows
                    cboOptions.Items.Add(dr("option_name"))
                Next
                txtProperty.Visible = False
                cboOptions.Visible = True
                cboBoolean.Visible = False
                cboOptions.Text = "" & dgvProperties("property_value", dgvProperties.CurrentCell.RowIndex).Value
            Else
                btnEditList.Visible = False
                txtProperty.Visible = True
                cboOptions.Visible = False
                cboBoolean.Visible = False
            End If
            'butObjectDelete.Visible = True
        Catch ex As Exception
            'MsgBox(ex.Message)
        End Try
    End Sub

    Public Sub Load_Object_States()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        flgLoadingStates = 1
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT state_label FROM osae_v_object_state WHERE object_name=?pname ORDER BY state_label"
        CMD.Parameters.AddWithValue("?pname", dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value)
        Try
            comObjectStates.Items.Clear()
            comObjectStates.Text = ""
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                comObjectStates.Items.Add(Convert.ToString(myReader.Item("state_label")))
            End While
            CN.Close()
            comObjectStates.Text = "" & dgvObjects("state_label", dgvObjects.CurrentCell.RowIndex).Value
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_States: " & myerror.Message)
            CN.Close()
        End Try
        flgLoadingStates = 0
    End Sub

    Public Sub Load_Object_Methods()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT method_label FROM osae_v_object_method WHERE object_name=?pname ORDER BY method_label"
        CMD.Parameters.AddWithValue("?pname", dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value)
        Try
            comObjectMethods.Visible = False
            lblMethods.Visible = False
            grpParameters.Visible = False
            comObjectMethods.Items.Clear()
            comObjectMethods.Text = ""
            lblParam1.Text = ""
            lblParam2.Text = ""

            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                comObjectMethods.Items.Add(Convert.ToString(myReader.Item("method_label")))
                comObjectMethods.Visible = True
                lblMethods.Visible = True
            End While

            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_Methods: " & myerror.Message)
            CN.Close()
        End Try
    End Sub

    Public Sub Load_Object_Events()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT event_label FROM osae_v_object_event WHERE object_name=?pname ORDER BY event_label"
        CMD.Parameters.AddWithValue("?pname", dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value)
        Try
            comObjectEvents.Visible = False
            comObjectEvents.Items.Clear()
            lblEvents.Visible = False
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                comObjectEvents.Items.Add(Convert.ToString(myReader.Item("event_label")))
                comObjectEvents.Visible = True
                lblEvents.Visible = True
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_Events: " & myerror.Message)
            CN.Close()
        End Try
    End Sub

    Public Sub Load_Container_Filter()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        sRefresh = "Y"
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE container=1 ORDER BY object_name"
        Try
            cboContainers.Items.Clear()
            cboContainers.Items.Add(" All Containers")
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboContainers.Items.Add(Convert.ToString(myReader.Item("object_name")))
            End While
            CN.Close()
            'comLocation.Text = "" & dgvObjects("container_name", dgvObjects.CurrentCell.RowIndex).Value
            cboContainers.Text = " All Containers"
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Container_Filter: " & myerror.Message)
            CN.Close()
        End Try
        sRefresh = "N"
    End Sub

    Public Sub Load_Containers()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        sRefresh = "Y"
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE container=1 ORDER BY object_name"
        Try
            comLocation.Items.Clear()
            comLocation.Text = ""
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                comLocation.Items.Add(Convert.ToString(myReader.Item("object_name")))
            End While
            CN.Close()
            comLocation.Text = "" & dgvObjects("container_name", dgvObjects.CurrentCell.RowIndex).Value
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Containers: " & myerror.Message)
            CN.Close()
        End Try
        sRefresh = "N"
    End Sub
    Public Sub Load_Object_Properties()
        Dim CMD As New MySqlCommand
        Dim iTemp1 As Integer
        Dim adapter As New MySqlDataAdapter
        Dim dsItems As New DataSet
        Dim dtItems As New DataTable
        Dim dvItems As New DataView
        dgvProperties.Visible = False
        lblProperty.Visible = False
        txtProperty.Visible = False
        butPropertyUpdate.Visible = False
        'dgvProperties.DataSource = Nothing
        CMD.Connection = CN
        CMD.CommandText = "SELECT property_name,property_value,property_datatype,last_updated FROM osae_v_object_property WHERE object_name=?pname ORDER BY property_name"
        CMD.Parameters.AddWithValue("?pname", dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value)
        Console.Write("Name " & dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value & vbCrLf)
        Console.Write("Row " & dgvObjects.CurrentCell.RowIndex & vbCrLf)

        adapter.SelectCommand = CMD
        CN.Open()
        adapter.Fill(dsItems, "AllItems")
        dvItems = dsItems.Tables(0).DefaultView
        CN.Close()
        iTemp1 = dtItems.Rows.Count
        dgvProperties.DataSource = dvItems

        'Dim MyDT As New DataTable
        'Dim MyDA As New MySqlDataAdapter("SELECT property_name,property_datatype,property_value FROM osae_v_object_property WHERE object_name='" & dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value & "' ORDER BY property_name", CN)

        'MyDA.Fill(MyDT)
        ' dgvProperties.DataSource = MyDT
        If dgvProperties.RowCount > 0 Then
            dgvProperties.Visible = True
            lblProperty.Visible = True
            'txtProperty.Visible = True
        End If

    End Sub

    Private Sub txtObject_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObject.TextChanged
        Validate_Object()
    End Sub

    Private Sub txtObjectDescription_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObjectDescription.TextChanged
        Validate_Object()
    End Sub

    Private Sub cboObjectTypes_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboObjectTypes.SelectedIndexChanged
        Validate_Object()
    End Sub

    Private Sub chkEnabled_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkEnabled.CheckedChanged
        Validate_Object()
    End Sub

    Private Sub comLocation_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comLocation.SelectedIndexChanged
        Validate_Object()
    End Sub

    Private Sub txtAddress_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtAddress.TextChanged
        Validate_Object()
    End Sub

    Public Sub Validate_Object()
        Dim CMD As New MySqlCommand
        'Dim myReader As MySqlDataReader
        Dim iCount As Integer
        If txtObject.Text.Length = 0 Or cboObjectTypes.Text.Length = 0 Then
            butObjectAdd.Enabled = False
            Exit Sub
        End If
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT count(object_name) as Results FROM osae_v_object WHERE object_name=?pname OR (address=?paddress AND address IS NOT NULL AND address !='')"
        CMD.Parameters.AddWithValue("?pname", txtObject.Text)
        CMD.Parameters.AddWithValue("?paddress", txtAddress.Text)
        Try
            CN.Open()
            iCount = CMD.ExecuteScalar
            CN.Close()
            CMD.Parameters.Clear()
        Catch myerror As MySqlException
            MessageBox.Show("Error Validate_Object: " & myerror.Message)
            CN.Close()
        End Try
        If iCount = 0 Then
            butObjectAdd.Enabled = True
            butObjectUpdate.Enabled = True
        Else
            butObjectAdd.Enabled = False
            CMD.CommandText = "SELECT count(object_name) as Results FROM osae_v_object WHERE object_name=?pname AND object_description=?pdesc AND object_type=?ptype AND container_name=?pcontainer AND address=?paddress AND enabled=?penabled"
            CMD.Parameters.AddWithValue("?pname", txtObject.Text)
            CMD.Parameters.AddWithValue("?pdesc", txtObjectDescription.Text)
            CMD.Parameters.AddWithValue("?ptype", cboObjectTypes.Text)
            CMD.Parameters.AddWithValue("?pcontainer", comLocation.Text)
            CMD.Parameters.AddWithValue("?paddress", txtAddress.Text)
            CMD.Parameters.AddWithValue("?penabled", chkEnabled.Checked)
            Try
                CN.Open()
                iCount = CMD.ExecuteScalar
                CN.Close()
                CMD.Parameters.Clear()
            Catch myerror As MySqlException
                MessageBox.Show("Error Validate_Object 2: " & myerror.Message)
                CN.Close()
            End Try
            If iCount = 0 Then
                butObjectUpdate.Enabled = True
            Else
                butObjectUpdate.Enabled = False
            End If
        End If
    End Sub

    Private Sub butObjectUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butObjectUpdate.Click
        OSAEApi.ObjectUpdate(dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value, txtObject.Text, txtObjectDescription.Text, cboObjectTypes.Text, txtAddress.Text, comLocation.Text, Math.Abs(CInt(chkEnabled.Checked)))
        Load_Objects()
        Load_Containers()
    End Sub

    Private Sub butObjectDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butObjectDelete.Click
        Dim result As DialogResult = Windows.Forms.DialogResult.Yes
        result = MessageBox.Show("Are you sure you want to Delete: " & dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = Windows.Forms.DialogResult.Yes Then
            OSAEApi.ObjectDelete(dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value)
            Load_Objects()
            Load_Containers()
        End If
    End Sub

    Private Sub txtProperty_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtProperty.TextChanged
        Dim CMD As New MySqlCommand
        Dim iCount As Integer
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT count(property_name) as Results FROM osae_v_object_property WHERE object_name=?pname AND property_name=?pproperty AND property_value=?pvalue"
        CMD.Parameters.AddWithValue("?pname", dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value)
        CMD.Parameters.AddWithValue("?pproperty", dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value)
        CMD.Parameters.AddWithValue("?pvalue", txtProperty.Text)
        Try
            CN.Open()
            iCount = CMD.ExecuteScalar
            CN.Close()
            CMD.Parameters.Clear()
        Catch myerror As MySqlException
            MessageBox.Show("Error txtProperty_TextChanged: " & myerror.Message)
            CN.Close()
        End Try
        If iCount = 0 Then
            butPropertyUpdate.Visible = True
        Else
            butPropertyUpdate.Visible = False
        End If
    End Sub

    Private Sub butPropertyUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butPropertyUpdate.Click
        Debug.Print("------------===: " & dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value & " " & dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value & " " & txtProperty.Text)
        OSAEApi.ObjectPropertySet(dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value, dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value, txtProperty.Text)
        Load_Object_Properties()
    End Sub

    Private Sub comObjectStates_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comObjectStates.SelectedIndexChanged
        If flgLoadingStates = 0 Then

            Try
                OSAEApi.ObjectStateSet(dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value, comObjectStates.Text)
                Load_Objects()
            Catch myerror As MySqlException
                MessageBox.Show("Error comObjectStates: " & myerror.Message)
                CN.Close()
            End Try
        End If
    End Sub

    Private Sub butObjectAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butObjectAdd.Click
        OSAEApi.ObjectAdd(txtObject.Text, txtObjectDescription.Text, cboObjectTypes.Text, txtAddress.Text, comLocation.Text, True)
        OSAEApi.ObjectStateSet(txtObject.Text, "OFF")
        Load_Objects()
        Load_Containers()
    End Sub

    Private Sub comObjectMethods_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comObjectMethods.SelectedIndexChanged
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT param_1_label,param_2_label,param_1_default,param_2_default FROM osae_v_object_method WHERE object_name=?pname AND method_label=?pmethod"
        CMD.Parameters.AddWithValue("?pname", dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value)
        CMD.Parameters.AddWithValue("?pmethod", comObjectMethods.Text)
        Try
            lblParam1.Text = ""
            lblParam2.Text = ""
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                lblParam1.Text = "" & Convert.ToString(myReader.Item("param_1_label"))
                lblParam2.Text = "" & Convert.ToString(myReader.Item("param_2_label"))
                txtParamLabel1.Text = "" & Convert.ToString(myReader.Item("param_1_default"))
                txtParamLabel2.Text = "" & Convert.ToString(myReader.Item("param_2_default"))
            End While
            CN.Close()
            If lblParam1.Text = "" Then
                grpParameters.Visible = False
                lblParam1.Visible = False
                txtParamLabel1.Text = ""
                txtParamLabel1.Visible = False
                Run_Method()
            Else
                grpParameters.Visible = True
                lblParam1.Visible = True

                txtParamLabel1.Visible = True
                If lblParam2.Text = "" Then
                    lblParam2.Visible = False
                    txtParamLabel2.Visible = False
                Else
                    lblParam2.Visible = True
                    'txtParamLabel2.Text = ""
                    txtParamLabel2.Visible = True
                End If
            End If
        Catch myerror As MySqlException
            MessageBox.Show("Error comObjectMethods: " & myerror.Message)
            CN.Close()
        End Try
    End Sub

    Private Sub butExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butExport.Click
        Dim CMD As New MySqlCommand, sName As String, sLabel As String
        Dim myReader As MySql.Data.MySqlClient.MySqlDataReader
        frmSQLBox.Show()
        frmSQLBox.txtScript.Text = "CALL osae_sp_object_add ('" & dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value & "','" & dgvObjects("object_description", dgvObjects.CurrentCell.RowIndex).Value & "','" & dgvObjects("object_type", dgvObjects.CurrentCell.RowIndex).Value & "','" & txtAddress.Text & "','" & comLocation.Text & "',1);" & vbCrLf
        CMD.Connection = CN
        CMD.CommandText = "SELECT property_name,property_value FROM osae_v_object_property WHERE object_name=?ObjectName"
        CMD.Parameters.AddWithValue("?ObjectName", dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value)
        Try
            If CN.State = ConnectionState.Closed Then CN.Open()
            myReader = CMD.ExecuteReader()
            While myReader.Read
                sName = myReader.Item("property_name")
                sLabel = "" & myReader.Item("property_value")
                frmSQLBox.txtScript.Text = frmSQLBox.txtScript.Text & "CALL osae_sp_object_property_set ('" & sName & "','" & sLabel & "','" & dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value & "');" & vbCrLf
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error in Write_Script: " & myerror.Message)
            If CN.State <> ConnectionState.Closed Then CN.Close()
        End Try
    End Sub

    Private Sub butFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butFile.Click
        Dim res As DialogResult = file1.ShowDialog()
        If res = DialogResult.OK Then
            txtProperty.Text = file1.FileName.Replace(OSAEApi.APIpath, "")
        End If
    End Sub

    Private Sub btnRunMethod_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRunMethod.Click
        Run_Method()
    End Sub

    Private Sub Run_Method()
        OSAEApi.MethodQueueAdd(dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value, comObjectMethods.Text, txtParamLabel1.Text, txtParamLabel2.Text)
        comObjectMethods.Text = ""
        txtParamLabel1.Text = ""
        txtParamLabel2.Text = ""
        Load_Objects()
    End Sub

    Private Sub comObjectEvents_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comObjectEvents.SelectedIndexChanged

    End Sub

    Private Sub btEditList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditList.Click
        frmPropertyList.iObject = dgvObjects("object_name", dgvObjects.CurrentCell.RowIndex).Value
        frmPropertyList.iProperty = dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value
        frmPropertyList.Show()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        'Update_Objects()
        ' dgvObjects.Refresh()
        sRefresh = "Y"
        Load_Objects()
        sRefresh = "N"
    End Sub

    Private Sub dgvObjects_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvObjects.GotFocus
        Timer1.Enabled = True
    End Sub

    Private Sub dgvObjects_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvObjects.LostFocus
        Timer1.Enabled = False
    End Sub

    Private Sub cboBoolean_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboBoolean.SelectedIndexChanged
        txtProperty.Text = cboBoolean.Text
    End Sub

    Private Sub cboOptions_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboOptions.SelectedIndexChanged
        txtProperty.Text = cboOptions.Text
    End Sub

    Private Sub cboContainers_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboContainers.SelectedIndexChanged
        If cboContainers.Text <> " All Containers" Then cboTypes.Text = " All Types"
        Load_Objects()
    End Sub

    Private Sub frmObjects_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        cboContainers.Width = dgvObjects.Columns("container_name").Width
        cboTypes.Width = dgvObjects.Columns("object_type").Width
        cboTypes.Left = (dgvObjects.Columns("container_name").Width + dgvObjects.Columns("object_name").Width)
    End Sub

    Private Sub cboTypes_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTypes.SelectedIndexChanged
        If cboTypes.Text <> " All Types" Then cboContainers.Text = " All Containers"
        Load_Objects()
    End Sub

    Private Sub dgvProperties_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvProperties.CellContentClick

    End Sub

    Private Sub dgvObjects_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvObjects.CellContentClick

    End Sub
End Class
