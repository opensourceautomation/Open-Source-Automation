Imports MySql.Data.MySqlClient
Public Class frmObjectTypes
    Private CN As MySqlConnection
    Private Sub ObjectTypes_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DB_Connection()
        Load_Object_Types()
        Load_Object_Owners()
    End Sub
    Public Sub DB_Connection()
        CN = New MySqlConnection
        CN.ConnectionString = "server=" & OSAEApi.DBConnection & ";Port=" & OSAEApi.DBPort & ";Database=" & OSAEApi.DBName & ";Password=" & OSAEApi.DBPassword & ";use procedure bodies=false;Persist Security Info=True;User ID=" & OSAEApi.DBUsername
        Try
            CN.Open()
            CN.Close()
            ' logging.AddToLog("Connected to Database: " & OSAEApi.DBName & " @ " & OSAEApi.DBConnection & ":" & OSAEApi.DBPort, "CM15A")
        Catch myerror As MySqlException
            logging.AddToLog("Error Connecting to Database: " & myerror.Message, True)
        End Try
    End Sub
    Public Sub Load_Object_Types()
        Dim MyDT As New DataTable
        Dim MyDA As New MySqlDataAdapter("SELECT object_type,object_type_description,object_name,object_type_owner,system_hidden,base_type,container,hide_redundant_events FROM osae_v_object_type ORDER BY base_type, object_type", CN)
        Dim iRowHolder As Integer
        txtObjectType.Text = ""
        txtTypeDescription.Text = ""
        comTypeOwner.Text = ""
        comBaseType.Text = ""
        butObjectDelete.Enabled = False
        If dgvObjectTypes.RowCount > 0 Then
            iRowHolder = dgvObjectTypes.CurrentCell.RowIndex
        End If
        MyDA.Fill(MyDT)
        dgvObjectTypes.DataSource = MyDT
        Load_Base_Types()
        dgvObjectTypes.CurrentCell = dgvObjectTypes.Rows(iRowHolder).Cells("object_type")
        ' frmObjects.Load_Object_Types()
    End Sub
    Public Sub Load_Object_Owners()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE object_type_owner=1 ORDER BY object_name"
        Try
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                comTypeOwner.Items.Add(Convert.ToString(myReader.Item("object_name")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_Owners: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
    Public Sub Load_Base_Types()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_type FROM osae_v_object_type ORDER BY object_type"
        Try
            comBaseType.Items.Clear()
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                comBaseType.Items.Add(Convert.ToString(myReader.Item("object_type")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_Owners: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
    Public Sub Write_Script()
        Dim CMD As New MySqlCommand, sName As String, sLabel As String, sParam1Label As String, sParam2Label As String
        Dim sParam1Default As String, sParam2Default As String, sHistory As Integer, sPropertyDefault As String
        Dim myReader As MySql.Data.MySqlClient.MySqlDataReader
        frmSQLBox.Show()
        frmSQLBox.txtScript.Text = "CALL osae_sp_object_type_add ('" & txtObjectType.Text & "','" & txtTypeDescription.Text & "','" & comTypeOwner.Text & "','" & comBaseType.Text & "'," & Math.Abs(CInt(chkOwner.Checked)) & "," & Math.Abs(CInt(chkSystem.Checked)) & "," & Math.Abs(CInt(chkContainer.Checked)) & "," & Math.Abs(CInt(chkHideRedundant.Checked)) & ");" & vbCrLf
        CMD.Connection = CN
        CMD.CommandText = "SELECT state_name,state_label FROM osae_v_object_type_state WHERE object_type=?ObjectType"
        CMD.Parameters.AddWithValue("?ObjectType", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Try
            If CN.State = ConnectionState.Closed Then CN.Open()
            myReader = CMD.ExecuteReader()
            While myReader.Read
                sName = myReader.Item("state_name")
                sLabel = myReader.Item("state_label")
                frmSQLBox.txtScript.Text = frmSQLBox.txtScript.Text & "CALL osae_sp_object_type_state_add ('" & sName & "','" & sLabel & "','" & dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value & "');" & vbCrLf
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error in Write_Script: " & myerror.Message)
            If CN.State <> ConnectionState.Closed Then CN.Close()
        End Try
        CMD.Parameters.Clear()
        CMD.CommandText = "SELECT event_name,event_label FROM osae_v_object_type_event WHERE object_type=?ObjectType"
        CMD.Parameters.AddWithValue("?ObjectType", dgvObjectTypes(0, dgvObjectTypes.CurrentCell.RowIndex).Value)
        Try
            If CN.State = ConnectionState.Closed Then CN.Open()
            myReader = CMD.ExecuteReader()
            While myReader.Read
                sName = myReader.Item("event_name")
                sLabel = myReader.Item("event_label")
                frmSQLBox.txtScript.Text = frmSQLBox.txtScript.Text & "CALL osae_sp_object_type_event_add ('" & sName & "','" & sLabel & "','" & dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value & "');" & vbCrLf
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error in Write_Script: " & myerror.Message)
            If CN.State <> ConnectionState.Closed Then CN.Close()
        End Try
        CMD.Parameters.Clear()
        CMD.CommandText = "SELECT method_name,method_label,param_1_label,param_2_label,param_1_default,param_2_default FROM osae_v_object_type_method WHERE object_type=?ObjectType"
        CMD.Parameters.AddWithValue("?ObjectType", dgvObjectTypes(0, dgvObjectTypes.CurrentCell.RowIndex).Value)
        Try
            If CN.State = ConnectionState.Closed Then CN.Open()
            myReader = CMD.ExecuteReader()
            While myReader.Read
                sName = myReader.Item("method_name")
                sLabel = myReader.Item("method_label")
                sParam1Label = "" & myReader.Item("param_1_label")
                sParam2Label = "" & myReader.Item("param_2_label")
                sParam1Default = "" & myReader.Item("param_1_default")
                sParam2Default = "" & myReader.Item("param_2_default")
                frmSQLBox.txtScript.Text = frmSQLBox.txtScript.Text & "CALL osae_sp_object_type_method_add ('" & sName & "','" & sLabel & "','" & dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value & "','" & sParam1Label & "','" & sParam2Label & "','" & sParam1Default & "','" & sParam2Default & "');" & vbCrLf
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error in Write_Script: " & myerror.Message)
            If CN.State <> ConnectionState.Closed Then CN.Close()
        End Try
        CMD.Parameters.Clear()
        CMD.CommandText = "SELECT property_name,property_datatype,property_default,track_history FROM osae_v_object_type_property WHERE object_type=?ObjectType"
        CMD.Parameters.AddWithValue("?ObjectType", dgvObjectTypes(0, dgvObjectTypes.CurrentCell.RowIndex).Value)
        Try
            If CN.State = ConnectionState.Closed Then CN.Open()
            myReader = CMD.ExecuteReader()
            While myReader.Read
                sName = myReader.Item("property_name")
                sLabel = myReader.Item("property_datatype")
                sPropertyDefault = "" & myReader.Item("property_default")
                sHistory = myReader.Item("track_history")
                frmSQLBox.txtScript.Text = frmSQLBox.txtScript.Text & "CALL osae_sp_object_type_property_add ('" & sName & "','" & sLabel & "','" & sPropertyDefault & "','" & dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value & "'," & Math.Abs(sHistory) & ");" & vbCrLf
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error in Write_Script: " & myerror.Message)
            If CN.State <> ConnectionState.Closed Then CN.Close()
        End Try
    End Sub
    Public Sub Load_Object_States()
        Dim MyDT As New DataTable
        Dim MyDA As New MySqlDataAdapter("SELECT state_name, state_label FROM osae_v_object_type_state WHERE object_type='" & dgvObjectTypes(0, dgvObjectTypes.CurrentCell.RowIndex).Value & "' ORDER BY state_name", CN)
        txtState.Text = ""
        txtStateLabel.Text = ""
        butStateDelete.Enabled = False
        MyDA.Fill(MyDT)
        dgvStates.DataSource = MyDT
    End Sub
    Public Sub Load_Object_Events()
        Dim MyDT As New DataTable
        Dim MyDA As New MySqlDataAdapter("SELECT event_name,event_label FROM osae_v_object_type_event WHERE object_type='" & dgvObjectTypes(0, dgvObjectTypes.CurrentCell.RowIndex).Value & "' ORDER BY event_name", CN)
        txtEvent.Text = ""
        txtEventLabel.Text = ""
        butEventDelete.Enabled = False
        MyDA.Fill(MyDT)
        dgvEvents.DataSource = MyDT
    End Sub
    Public Sub Load_Object_Methods()
        Dim MyDT As New DataTable
        Dim MyDA As New MySqlDataAdapter("SELECT method_name,method_label,param_1_label,param_2_label,param_1_default,param_2_default FROM osae_v_object_type_method WHERE object_type='" & dgvObjectTypes(0, dgvObjectTypes.CurrentCell.RowIndex).Value & "' ORDER BY method_name", CN)
        txtMethod.Text = ""
        txtMethodLabel.Text = ""
        butMethodDelete.Enabled = False
        MyDA.Fill(MyDT)
        dgvMethods.DataSource = MyDT
    End Sub

    Public Sub Load_Object_Properties()
        Dim MyDT As New DataTable
        Dim MyDA As New MySqlDataAdapter("SELECT property_name,property_datatype,COALESCE(property_default,'') AS property_default,COALESCE(track_history,0) AS track_history FROM osae_v_object_type_property WHERE object_type='" & dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value & "' ORDER BY property_name", CN)
        txtProperty.Text = ""
        comPropertyType.Text = ""
        txtPropertyDefault.Text = ""
        butPropertyDelete.Enabled = False
        MyDA.Fill(MyDT)
        dgvProperties.DataSource = MyDT
    End Sub

    Private Sub butObjectAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butObjectAdd.Click
        OSAEApi.ObjectTypeAdd(txtObjectType.Text, txtTypeDescription.Text, comTypeOwner.Text, comBaseType.Text, Math.Abs(CInt(chkOwner.Checked)), Math.Abs(CInt(chkSystem.Checked)), Math.Abs(CInt(chkContainer.Checked)), Math.Abs(CInt(chkHideRedundant.Checked)))
        Load_Object_Types()
    End Sub

    Private Sub butNewSubtype_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butNewSubtype.Click
        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_object_type_clone"
        CMD.Parameters.AddWithValue("?Name", txtObjectType.Text)
        CMD.Parameters.AddWithValue("?Type", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error butNewSubtype: " & myerror.Message)
            CN.Close()
        End Try
        Load_Object_Types()
        ' api needs this added
    End Sub
    Private Sub butObjectUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butObjectUpdate.Click
        Object_Type_Update()
    End Sub
    Public Sub Object_Type_Update()
        OSAEApi.ObjectTypeUpdate(dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value, txtObjectType.Text, txtTypeDescription.Text, comTypeOwner.Text, comBaseType.Text, Math.Abs(CInt(chkOwner.Checked)), Math.Abs(CInt(chkSystem.Checked)), Math.Abs(CInt(chkContainer.Checked)), Math.Abs(CInt(chkHideRedundant.Checked)))
        Load_Object_Types()
    End Sub
    Private Sub butObjectDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butObjectDelete.Click
        Dim result As DialogResult = Windows.Forms.DialogResult.Yes
        result = MessageBox.Show("Are you sure you want to Delete: " & dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = Windows.Forms.DialogResult.Yes Then
            OSAEApi.ObjectTypeDelete(dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
            Load_Object_Types()
        End If
    End Sub
    Private Sub butStateAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butStateAdd.Click
        OSAEApi.ObjectTypeStateAdd(txtState.Text, txtStateLabel.Text, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Load_Object_States()
    End Sub
    Private Sub butStateUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butStateUpdate.Click
        OSAEApi.ObjectTypeStateUpdate(dgvStates("state_name", dgvStates.CurrentCell.RowIndex).Value, txtState.Text, txtStateLabel.Text, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Load_Object_States()
    End Sub
    Private Sub butStateDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butStateDelete.Click
        OSAEApi.ObjectTypeStateDelete(dgvStates("state_name", dgvStates.CurrentCell.RowIndex).Value, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Load_Object_States()
    End Sub
    Private Sub butEventAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butEventAdd.Click
        OSAEApi.ObjectTypeEventAdd(txtEvent.Text, txtEventLabel.Text, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Load_Object_Events()
    End Sub
    Private Sub butEventUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butEventUpdate.Click
        OSAEApi.ObjectTypeEventUpdate(dgvEvents("event_name", dgvEvents.CurrentCell.RowIndex).Value, txtEvent.Text, txtEventLabel.Text, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Load_Object_Events()
    End Sub
    Private Sub butEventDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butEventDelete.Click
        OSAEApi.ObjectTypeEventDelete(dgvEvents("event_name", dgvEvents.CurrentCell.RowIndex).Value, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Load_Object_Events()
    End Sub
    Private Sub butMethodAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butMethodAdd.Click
        OSAEApi.ObjectTypeMethodAdd(txtMethod.Text, txtMethodLabel.Text, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value, txtParamLabel1.Text, txtParamLabel2.Text, txtParamDefault1.Text, txtParamDefault2.Text)
        Load_Object_Methods()
    End Sub
    Private Sub butMethodUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butMethodUpdate.Click
        OSAEApi.ObjectTypeMethodUpdate(dgvMethods("method_name", dgvMethods.CurrentCell.RowIndex).Value, txtMethod.Text, txtMethodLabel.Text, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value, txtParamLabel1.Text, txtParamLabel2.Text, txtParamDefault1.Text, txtParamDefault2.Text)
        Load_Object_Methods()
    End Sub
    Private Sub butMethodDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butMethodDelete.Click
        OSAEApi.ObjectTypeMethodDelete(dgvMethods("method_name", dgvMethods.CurrentCell.RowIndex).Value, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Load_Object_Methods()
    End Sub
    Private Sub butPropertyAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butPropertyAdd.Click
        OSAEApi.ObjectTypePropertyAdd(txtProperty.Text, comPropertyType.Text, txtPropertyDefault.Text, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value, Math.Abs(CInt(chkTrackHistory.Checked)))
        Load_Object_Properties()
    End Sub
    Private Sub butPropertyUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butPropertyUpdate.Click
        OSAEApi.ObjectTypePropertyUpdate(dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value, txtProperty.Text, comPropertyType.Text, txtPropertyDefault.Text, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value, Math.Abs(CInt(chkTrackHistory.Checked)))
        Load_Object_Properties()
    End Sub
    Private Sub butPropertyDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butPropertyDelete.Click
        OSAEApi.ObjectTypePropertyDelete(dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value, dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Load_Object_Properties()
    End Sub
    Private Sub dgvObjectTypes_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvObjectTypes.CurrentCellChanged
        Try
            comTypeOwner.SelectedIndex = -1
            comBaseType.SelectedIndex = -1
            txtObjectType.Text = dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value
            txtTypeDescription.Text = "" & dgvObjectTypes("object_type_description", dgvObjectTypes.CurrentCell.RowIndex).Value
            comTypeOwner.Text = "" & dgvObjectTypes("object_name", dgvObjectTypes.CurrentCell.RowIndex).Value
            comBaseType.Text = "" & dgvObjectTypes("base_type", dgvObjectTypes.CurrentCell.RowIndex).Value
            chkOwner.Checked = dgvObjectTypes("object_type_owner", dgvObjectTypes.CurrentCell.RowIndex).Value
            chkSystem.Checked = dgvObjectTypes("system_hidden", dgvObjectTypes.CurrentCell.RowIndex).Value
            chkContainer.Checked = dgvObjectTypes("is_container", dgvObjectTypes.CurrentCell.RowIndex).Value
            chkHideRedundant.Checked = dgvObjectTypes("hide_redundant_events", dgvObjectTypes.CurrentCell.RowIndex).Value
            butObjectUpdate.Enabled = True
            If chkSystem.Checked Then butObjectDelete.Enabled = False Else butObjectDelete.Enabled = True
            Load_Object_States()
            Load_Object_Events()
            Load_Object_Methods()
            Load_Object_Properties()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub dgvStates_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvStates.CurrentCellChanged
        Try
            txtState.Text = dgvStates(0, dgvStates.CurrentCell.RowIndex).Value
            txtStateLabel.Text = "" & dgvStates(1, dgvStates.CurrentCell.RowIndex).Value
            butStateDelete.Enabled = True
        Catch ex As Exception

        End Try
    End Sub
    Private Sub dgvEvents_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvEvents.CurrentCellChanged
        Try
            txtEvent.Text = dgvEvents(0, dgvEvents.CurrentCell.RowIndex).Value
            txtEventLabel.Text = "" & dgvEvents(1, dgvEvents.CurrentCell.RowIndex).Value
            butEventDelete.Enabled = True
        Catch ex As Exception

        End Try
    End Sub
    Private Sub dgvMethods_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvMethods.CurrentCellChanged
        Try
            txtMethod.Text = dgvMethods("method_name", dgvMethods.CurrentCell.RowIndex).Value
            txtMethodLabel.Text = "" & dgvMethods("method_label", dgvMethods.CurrentCell.RowIndex).Value
            txtParamLabel1.Text = "" & dgvMethods("param_1_label", dgvMethods.CurrentCell.RowIndex).Value
            txtParamLabel2.Text = "" & dgvMethods("param_2_label", dgvMethods.CurrentCell.RowIndex).Value
            txtParamDefault1.Text = "" & dgvMethods("param_1_default", dgvMethods.CurrentCell.RowIndex).Value
            txtParamDefault2.Text = "" & dgvMethods("param_2_default", dgvMethods.CurrentCell.RowIndex).Value
            butMethodDelete.Enabled = True
        Catch ex As Exception

        End Try
    End Sub

    Private Sub dgvProperties_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvProperties.Click

    End Sub
    Private Sub dgvProperties_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvProperties.CurrentCellChanged
        Try
            txtProperty.Text = dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value
            comPropertyType.Text = "" & dgvProperties("property_datatype", dgvProperties.CurrentCell.RowIndex).Value
            txtPropertyDefault.Text = dgvProperties("property_default", dgvProperties.CurrentCell.RowIndex).Value
            chkTrackHistory.Checked = dgvProperties("track_history", dgvProperties.CurrentCell.RowIndex).Value
            butPropertyDelete.Enabled = True
        Catch ex As Exception

        End Try
    End Sub
    Private Sub txtObjectType_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObjectType.TextChanged
        Validate_Object_Type()
    End Sub
    Private Sub txtTypeDescription_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtTypeDescription.TextChanged
        Validate_Object_Type()
    End Sub
    Public Sub Validate_Object_Type()
        Dim CMD As New MySqlCommand
        'Dim myReader As MySqlDataReader
        Dim iCount As Integer
        If txtObjectType.Text.Length = 0 Or txtTypeDescription.Text.Length = 0 Then
            butObjectAdd.Enabled = False
            butNewSubtype.Enabled = False
            butObjectUpdate.Enabled = False
            Exit Sub
        End If
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT count(object_type) as Results FROM osae_v_object_type WHERE object_type=?pname"
        CMD.Parameters.AddWithValue("?pname", txtObjectType.Text)
        Try
            CN.Open()
            iCount = CMD.ExecuteScalar
            CN.Close()
            CMD.Parameters.Clear()
        Catch myerror As MySqlException
            MessageBox.Show("Error Validate_Object_Type: " & myerror.Message)
            CN.Close()
        End Try
        If iCount = 0 Then
            butObjectAdd.Enabled = True
            butNewSubtype.Enabled = True
            butObjectUpdate.Enabled = True
        Else
            butObjectAdd.Enabled = False
        End If
    End Sub
    Private Sub txtState_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtState.TextChanged
        Validate_State()
    End Sub
    Private Sub txtStateLabel_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtStateLabel.TextChanged
        Validate_State()
    End Sub
    Public Sub Validate_State()
        Dim CMD As New MySqlCommand
        'Dim myReader As MySqlDataReader
        Dim iCount As Integer
        If txtState.Text.Length = 0 Or txtStateLabel.Text.Length = 0 Then
            butStateAdd.Enabled = False
            butStateUpdate.Enabled = False
            Exit Sub
        End If
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT count(state_name) as Results FROM osae_v_object_type_state WHERE state_name=?pname AND object_type=?ptype"
        CMD.Parameters.AddWithValue("?pname", txtState.Text)
        CMD.Parameters.AddWithValue("?ptype", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Try
            CN.Open()
            iCount = CMD.ExecuteScalar
            CN.Close()
            CMD.Parameters.Clear()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_Status: " & myerror.Message)
            CN.Close()
        End Try
        If iCount = 0 Then
            butStateAdd.Enabled = True
            butStateUpdate.Enabled = True
        Else
            butStateAdd.Enabled = False
            CMD.CommandText = "SELECT count(state_name) as Results FROM osae_v_object_type_state WHERE state_name=?pname AND state_label=?pdescription AND object_type=?ptype"
            CMD.Parameters.AddWithValue("?pname", txtState.Text)
            CMD.Parameters.AddWithValue("?pdescription", txtStateLabel.Text)
            CMD.Parameters.AddWithValue("?ptype", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
            Try
                CN.Open()
                iCount = CMD.ExecuteScalar
                CN.Close()
                CMD.Parameters.Clear()
            Catch myerror As MySqlException
                MessageBox.Show("Error Load_Object_Status 2: " & myerror.Message)
                CN.Close()
            End Try
            If iCount = 0 Then
                butStateUpdate.Enabled = True
            Else
                butStateUpdate.Enabled = False
            End If
        End If
    End Sub
    Private Sub txtEvent_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtEvent.TextChanged
        Validate_Event()
    End Sub
    Private Sub txtEventLabel_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtEventLabel.TextChanged
        Validate_Event()
    End Sub
    Public Sub Validate_Event()
        Dim CMD As New MySqlCommand
        Dim iCount As Integer
        If txtEvent.Text.Length = 0 Or txtEventLabel.Text.Length = 0 Then
            butEventAdd.Enabled = False
            butEventUpdate.Enabled = False
            Exit Sub
        End If
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT count(event_name) as Results FROM osae_v_object_type_event WHERE event_name=?pname AND object_type=?ptype"
        CMD.Parameters.AddWithValue("?pname", txtEvent.Text)
        CMD.Parameters.AddWithValue("?ptype", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Try
            CN.Open()
            iCount = CMD.ExecuteScalar
            CN.Close()
            CMD.Parameters.Clear()
        Catch myerror As MySqlException
            MessageBox.Show("Error txtEvent_TextChanged 1: " & myerror.Message)
            CN.Close()
        End Try
        If iCount = 0 Then
            butEventAdd.Enabled = True
            butEventUpdate.Enabled = True
        Else
            butEventAdd.Enabled = False
            CMD.CommandText = "SELECT count(event_name) as Results FROM osae_v_object_type_event WHERE event_name=?pname AND event_label=?pdescription AND object_type=?ptype"
            CMD.Parameters.AddWithValue("?pname", txtEvent.Text)
            CMD.Parameters.AddWithValue("?pdescription", txtEventLabel.Text)
            CMD.Parameters.AddWithValue("?ptype", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
            Try
                CN.Open()
                iCount = CMD.ExecuteScalar
                CN.Close()
                CMD.Parameters.Clear()
            Catch myerror As MySqlException
                MessageBox.Show("Error txtEvent_TextChanged 2: " & myerror.Message)
                CN.Close()
            End Try
            If iCount = 0 Then
                butEventUpdate.Enabled = True
            Else
                butEventUpdate.Enabled = False
            End If
        End If
    End Sub
    Private Sub txtMethod_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtMethod.TextChanged
        Validate_Method()
    End Sub
    Private Sub txtMethodLabel_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtMethodLabel.TextChanged
        Validate_Method()
    End Sub
    Public Sub Validate_Method()
        Dim CMD As New MySqlCommand
        Dim iCount As Integer
        If txtMethod.Text.Length = 0 Or txtMethodLabel.Text.Length = 0 Then
            butMethodAdd.Enabled = False
            butMethodUpdate.Enabled = False
            Exit Sub
        End If
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT count(method_name) as Results FROM osae_v_object_type_method WHERE method_name=?pname AND object_type=?ptype"
        CMD.Parameters.AddWithValue("?pname", txtMethod.Text)
        CMD.Parameters.AddWithValue("?ptype", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Try
            CN.Open()
            iCount = CMD.ExecuteScalar
            CN.Close()
            CMD.Parameters.Clear()
        Catch myerror As MySqlException
            MessageBox.Show("Error Validate_Method 1: " & myerror.Message)
            CN.Close()
        End Try
        If iCount = 0 Then
            butMethodAdd.Enabled = True
            butMethodUpdate.Enabled = True
        Else
            butMethodAdd.Enabled = False
            CMD.CommandText = "SELECT count(method_name) as Results FROM osae_v_object_type_method WHERE LOWER(method_name)=?pname AND LOWER(method_label)=?plabel AND LOWER(param_1_label)=?pparam1 AND LOWER(param_2_label)=?pparam2 AND object_type=?ptype"
            CMD.Parameters.AddWithValue("?pname", txtMethod.Text.ToLower)
            CMD.Parameters.AddWithValue("?plabel", txtMethodLabel.Text.ToLower)
            CMD.Parameters.AddWithValue("?ptype", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
            CMD.Parameters.AddWithValue("?pparam1", txtParamLabel1.Text.ToLower)
            CMD.Parameters.AddWithValue("?pparam2", txtParamLabel2.Text.ToLower)
            Try
                CN.Open()
                iCount = CMD.ExecuteScalar
                CN.Close()
                CMD.Parameters.Clear()
            Catch myerror As MySqlException
                MessageBox.Show("Error Validate_Method 2: " & myerror.Message)
                CN.Close()
            End Try
            If iCount = 0 Then
                butMethodUpdate.Enabled = True
            Else
                butMethodUpdate.Enabled = False
            End If
        End If
    End Sub
    Private Sub txtProperty_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtProperty.TextChanged
        Validate_Property()
    End Sub
    Private Sub comPropertyType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comPropertyType.SelectedIndexChanged
        Validate_Property()
    End Sub
    Public Sub Validate_Property()
        Dim CMD As New MySqlCommand
        Dim iCount As Integer
        If txtProperty.Text.Length = 0 Or comPropertyType.Text.Length = 0 Then
            butPropertyAdd.Enabled = False
            butPropertyUpdate.Enabled = False
            Exit Sub
        End If
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT count(property_name) as Results FROM osae_v_object_type_property WHERE property_name=?pname AND object_type=?ptype"
        CMD.Parameters.AddWithValue("?pname", txtProperty.Text)
        CMD.Parameters.AddWithValue("?ptype", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
        Try
            CN.Open()
            iCount = CMD.ExecuteScalar
            CN.Close()
            CMD.Parameters.Clear()
        Catch myerror As MySqlException
            MessageBox.Show("Error Validate_Property 1: " & myerror.Message)
            CN.Close()
        End Try
        If iCount = 0 Then
            butPropertyAdd.Enabled = True
            butPropertyUpdate.Enabled = False
            btnPropOption.Enabled = False
            CMD.CommandText = "SELECT count(property_name) as Results FROM osae_v_object_type_property WHERE UPPER(property_name)=UPPER(?pname) AND property_datatype=?pdatatype AND object_type=?ptype AND track_history=?ptrackhistory AND property_default=?ppropertydefault"
            CMD.Parameters.AddWithValue("?pname", txtProperty.Text)
            CMD.Parameters.AddWithValue("?pdatatype", comPropertyType.Text)
            CMD.Parameters.AddWithValue("?ptype", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
            CMD.Parameters.AddWithValue("?ptrackhistory", Math.Abs(CInt(chkTrackHistory.Checked)))
            CMD.Parameters.AddWithValue("?ppropertydefault", txtPropertyDefault.Text)

            Try
                CN.Open()
                iCount = CMD.ExecuteScalar
                CN.Close()
                CMD.Parameters.Clear()
            Catch myerror As MySqlException
                MessageBox.Show("Error Validate_Property 1: " & myerror.Message)
                CN.Close()
            End Try
            If iCount = 0 Then butPropertyUpdate.Enabled = True Else butPropertyUpdate.Enabled = False
        Else
            butPropertyAdd.Enabled = False
            If comPropertyType.SelectedItem <> "List" And comPropertyType.SelectedItem <> "Boolean" Then
                btnPropOption.Enabled = True
            Else
                btnPropOption.Enabled = False
            End If
            CMD.CommandText = "SELECT count(property_name) as Results FROM osae_v_object_type_property WHERE UPPER(property_name)=UPPER(?pname) AND property_datatype=?pdatatype AND object_type=?ptype AND track_history=?ptrackhistory AND property_default=?ppropertydefault"
            CMD.Parameters.AddWithValue("?pname", txtProperty.Text)
            CMD.Parameters.AddWithValue("?pdatatype", comPropertyType.Text)
            CMD.Parameters.AddWithValue("?ptype", dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value)
            CMD.Parameters.AddWithValue("?ptrackhistory", Math.Abs(CInt(chkTrackHistory.Checked)))
            CMD.Parameters.AddWithValue("?ppropertydefault", txtPropertyDefault.Text)
            Try
                CN.Open()
                iCount = CMD.ExecuteScalar
                CN.Close()
                CMD.Parameters.Clear()
            Catch myerror As MySqlException
                MessageBox.Show("Error Validate_Property 2: " & myerror.Message)
                CN.Close()
            End Try
            If iCount = 0 Then
                butPropertyUpdate.Enabled = True
            Else
                butPropertyUpdate.Enabled = False
            End If
        End If
    End Sub
    Private Sub butClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butClose.Click
        Me.Close()
    End Sub
    Private Sub butExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butExport.Click
        Write_Script()
    End Sub

    Private Sub dgvObjectTypes_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvObjectTypes.CellContentClick

    End Sub

    Private Sub dgvProperties_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvProperties.CellContentClick

    End Sub

    Private Sub chkTrackHistory_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkTrackHistory.CheckedChanged
        Validate_Property()
    End Sub

    Private Sub txtPropertyDefault_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPropertyDefault.TextChanged
        Validate_Property()
    End Sub

    Private Sub btnPropOption_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPropOption.Click
        frmAddPropertyOption.iObjectType = dgvObjectTypes("object_type", dgvObjectTypes.CurrentCell.RowIndex).Value
        frmAddPropertyOption.iProperty = dgvProperties("property_name", dgvProperties.CurrentCell.RowIndex).Value
        frmAddPropertyOption.Show()
    End Sub
End Class