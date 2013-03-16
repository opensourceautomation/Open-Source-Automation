Imports MySql.Data.MySqlClient

Public Class frmScheduling
    Private CN As MySqlConnection

    Private Sub Scheduling_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DB_Connection()
        Load_Queue()
        Load_Recurring()
        Load_Objects()
        Load_Patterns()
    End Sub

    Public Sub DB_Connection()
        CN = New MySqlConnection
        CN.ConnectionString = "server=" & OSAEApi.DBConnection & ";Port=" & OSAEApi.DBPort & ";Database=" & OSAEApi.DBName & ";Password=" & OSAEApi.DBPassword & ";use procedure bodies=false;Persist Security Info=True;User ID=" & OSAEApi.DBUsername
        Try
            CN.Open()
            CN.Close()
        Catch myerror As MySqlException
            logging.AddToLog("Error Connecting to Database: " & myerror.Message, True)
        End Try
    End Sub

    Public Sub Load_Queue()
        Dim CMD As New MySqlCommand
        Dim iTemp1 As Integer
        Dim adapter As New MySqlDataAdapter
        Dim dsItems As New DataSet
        Dim dtItems As New DataTable
        Dim dvItems As New DataView
        CMD.Connection = CN
        CMD.CommandText = "SELECT schedule_id,queue_datetime,schedule_name FROM osae_v_schedule_queue ORDER BY queue_datetime"
        adapter.SelectCommand = CMD
        CN.Open()
        adapter.Fill(dsItems, "ScheduleQueue")
        dvItems = dsItems.Tables(0).DefaultView
        CN.Close()
        iTemp1 = dtItems.Rows.Count
        dgvQueue.DataSource = dvItems
    End Sub

    Public Sub Load_Recurring()
        Dim CMD As New MySqlCommand
        Dim iTemp1 As Integer
        Dim adapter As New MySqlDataAdapter
        Dim dsItems As New DataSet
        Dim dtItems As New DataTable
        Dim dvItems As New DataView
        CMD.Connection = CN
        CMD.CommandText = "SELECT schedule_name,interval_unit FROM osae_v_schedule_recurring ORDER BY schedule_name"
        adapter.SelectCommand = CMD
        CN.Open()
        adapter.Fill(dsItems, "RecurringSchedule")
        dvItems = dsItems.Tables(0).DefaultView
        CN.Close()
        iTemp1 = dtItems.Rows.Count
        dgvRecurring.DataSource = dvItems
    End Sub

    Public Sub Validate_Save()
        btnAdd.Enabled = True
        btnUpdate.Enabled = True
        If (txtName.TextLength = 0 And optSingle.Checked = False) Or (cboObject.SelectedIndex < 0 And cboPattern.SelectedIndex < 0) Or (cboPattern.SelectedIndex < 0 And cboMethod.SelectedIndex < 0) Then
            btnAdd.Enabled = False
            btnUpdate.Enabled = False
            Exit Sub
        End If
        Dim CMD As New MySqlCommand
        Dim iCount As Integer
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT count(recurring_id) as Results FROM osae_schedule_recurring WHERE schedule_name=?pname"
        CMD.Parameters.AddWithValue("?pname", txtName.Text)
        Try
            CN.Open()
            iCount = CMD.ExecuteScalar
            If iCount > 0 Then
                btnAdd.Enabled = False
                'btnUpdate.Enabled = True
            Else
                btnAdd.Enabled = True
                'btnUpdate.Enabled = False
            End If
            CMD.Parameters.Clear()
        Catch ex As Exception
            MessageBox.Show("Error Validate_Object_Type: " & ex.Message, "Scheduling")
        End Try
        CN.Close()
    End Sub

    Private Sub btnQueueDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQueueDelete.Click
        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure

        CMD.CommandText = "osae_sp_schedule_queue_delete"
        CMD.Parameters.AddWithValue("?pqueueid", dgvQueue("schedule_id", dgvQueue.CurrentCell.RowIndex).Value)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error btnQueueDelete: " & myerror.Message, "Scheduling")
            CN.Close()
        End Try
        Load_Queue()
        Load_Recurring()
    End Sub

    Private Sub btnRecurringDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRecurringDelete.Click
        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_schedule_recurring_delete"
        CMD.Parameters.AddWithValue("?pScheduleName", dgvRecurring("schedule_name1", dgvRecurring.CurrentCell.RowIndex).Value)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error btnRecurringDelete: " & myerror.Message, "Scheduling")
            CN.Close()
        End Try
        Load_Queue()
        Load_Recurring()
    End Sub

    Public Sub Load_Objects()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT DISTINCT object_name FROM osae_v_object_method WHERE method_name<>'' ORDER BY object_name"
        Try
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboObject.Items.Add(Convert.ToString(myReader.Item("object_name")))
            End While
            CN.Close()
            'cboObject.Text = frmObjects.dgvObjects("object_name", frmObjects.dgvObjects.CurrentCell.RowIndex).Value
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Objects: " & myerror.Message, "Scheduling")
            CN.Close()
        End Try
    End Sub

    Public Sub Load_Patterns()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT pattern FROM osae_pattern WHERE script IS NOT NULL AND script <>'' ORDER BY pattern"
        Try
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboPattern.Items.Add(Convert.ToString(myReader.Item("pattern")))
            End While
            CN.Close()
            'cboPattern.Text = frmObjects.dgvObjects("pattern", frmObjects.dgvObjects.CurrentCell.RowIndex).Value
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Patterns: " & myerror.Message, "Scheduling")
            CN.Close()
        End Try
    End Sub

    Private Sub cboObject_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboObject.SelectedIndexChanged
        Load_Methods()
        Validate_Save()
    End Sub

    Public Sub Load_Methods()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        cboMethod.Items.Clear()
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT method_label FROM osae_v_object_method WHERE UPPER(object_name)=UPPER(?ObjectName) ORDER BY method_label"
        CMD.Parameters.AddWithValue("?ObjectName", cboObject.Text)
        Try
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboMethod.Items.Add(Convert.ToString(myReader.Item("method_label")))
            End While
            CN.Close()
            If cboMethod.Items.Count > 0 Then cboMethod.SelectedIndex = 0
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Methods: " & myerror.Message, "Scheduling")
            CN.Close()
        End Try
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        If optSingle.Checked = True Then
            OSAEApi.ScheduleQueueAdd(dtpSingle.Value.ToString("yyyy-MM-dd") & " " & dtpTime.Value.ToString("HH:mm:ss"), cboObject.Text, cboMethod.Text, txtParam1.Text, txtParam2.Text, cboPattern.Text, 0)
        ElseIf optMinutes.Checked = True Then
            OSAEApi.ScheduleRecurringAdd(txtName.Text, cboObject.Text, cboMethod.Text, txtParam1.Text, txtParam2.Text, cboPattern.Text, dtpTime.Value.ToString("HH:mm:ss"), Math.Abs(CInt(chkSunday.Checked)), Math.Abs(CInt(chkMonday.Checked)), Math.Abs(CInt(chkTuesday.Checked)), Math.Abs(CInt(chkWednesday.Checked)), Math.Abs(CInt(chkThursday.Checked)), Math.Abs(CInt(chkFriday.Checked)), Math.Abs(CInt(chkSaturday.Checked)), "T", nudMinutes.Value, 0, dtpSingle.Value.ToString("yyyy-MM-dd"))
        ElseIf optDaily.Checked = True Then
            OSAEApi.ScheduleRecurringAdd(txtName.Text, cboObject.Text, cboMethod.Text, txtParam1.Text, txtParam2.Text, cboPattern.Text, dtpTime.Value.ToString("HH:mm:ss"), Math.Abs(CInt(chkSunday.Checked)), Math.Abs(CInt(chkMonday.Checked)), Math.Abs(CInt(chkTuesday.Checked)), Math.Abs(CInt(chkWednesday.Checked)), Math.Abs(CInt(chkThursday.Checked)), Math.Abs(CInt(chkFriday.Checked)), Math.Abs(CInt(chkSaturday.Checked)), "D", 0, 0, dtpSingle.Value.ToString("yyyy-MM-dd"))
        ElseIf optMonthly.Checked = True Then
            OSAEApi.ScheduleRecurringAdd(txtName.Text, cboObject.Text, cboMethod.Text, txtParam1.Text, txtParam2.Text, cboPattern.Text, dtpTime.Value.ToString("HH:mm:ss"), Math.Abs(CInt(chkSunday.Checked)), Math.Abs(CInt(chkMonday.Checked)), Math.Abs(CInt(chkTuesday.Checked)), Math.Abs(CInt(chkWednesday.Checked)), Math.Abs(CInt(chkThursday.Checked)), Math.Abs(CInt(chkFriday.Checked)), Math.Abs(CInt(chkSaturday.Checked)), "M", 0, cboDay.Text, dtpSingle.Value.ToString("yyyy-MM-dd"))
        ElseIf optAnnual.Checked = True Then
            ' logging.AddToLog("ScheduleRecurringAdd(" & txtName.Text & "," & cboObject.Text & "," & cboMethod.Text & "," & txtParam1.Text & "," & txtParam2.Text & "," & cboPattern.Text & "," & dtpTime.Value.ToString("HH:mm:ss") & "," & Math.Abs(CInt(chkSunday.Checked)) & "," & Math.Abs(CInt(chkMonday.Checked)) & "," & Math.Abs(CInt(chkTuesday.Checked)) & "," & Math.Abs(CInt(chkWednesday.Checked)) & "," & Math.Abs(CInt(chkThursday.Checked)) & "," & Math.Abs(CInt(chkFriday.Checked)) & "," & Math.Abs(CInt(chkSaturday.Checked)) & ",Y,0,0," & dtpAnnual.Value.ToString("yyyy-MM-dd") & "))", False)
            OSAEApi.ScheduleRecurringAdd(txtName.Text, cboObject.Text, cboMethod.Text, txtParam1.Text, txtParam2.Text, cboPattern.Text, dtpTime.Value.ToString("HH:mm:ss"), Math.Abs(CInt(chkSunday.Checked)), Math.Abs(CInt(chkMonday.Checked)), Math.Abs(CInt(chkTuesday.Checked)), Math.Abs(CInt(chkWednesday.Checked)), Math.Abs(CInt(chkThursday.Checked)), Math.Abs(CInt(chkFriday.Checked)), Math.Abs(CInt(chkSaturday.Checked)), "Y", 0, 0, dtpAnnual.Value.ToString("yyyy-MM-dd"))
        End If
        Load_Queue()
        Load_Recurring()
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        'CALL osae_sp_schedule_recurring_update(@OldScheduleName, @NewScheduleName, @Object, @Method, @Parameter1, @Parameter2, @Pattern, @RecurringTime, @Sunday, @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, @Saturday, @Interval, @RecurringMinutes, @RecurringDay, @RecurringDate)";
        If optMinutes.Checked = True Then
            OSAEApi.ScheduleRecurringUpdate(dgvRecurring("schedule_name1", dgvRecurring.CurrentCell.RowIndex).Value, txtName.Text, cboObject.Text, cboMethod.Text, txtParam1.Text, txtParam2.Text, cboPattern.Text, dtpTime.Value.ToString("HH:mm:ss"), Math.Abs(CInt(chkSunday.Checked)), Math.Abs(CInt(chkMonday.Checked)), Math.Abs(CInt(chkTuesday.Checked)), Math.Abs(CInt(chkWednesday.Checked)), Math.Abs(CInt(chkThursday.Checked)), Math.Abs(CInt(chkFriday.Checked)), Math.Abs(CInt(chkSaturday.Checked)), "T", nudMinutes.Value, cboDay.Text, dtpSingle.Value.ToString("yyyy-MM-dd"))
        ElseIf optDaily.Checked = True Then
            OSAEApi.ScheduleRecurringUpdate(dgvRecurring("schedule_name1", dgvRecurring.CurrentCell.RowIndex).Value, txtName.Text, cboObject.Text, cboMethod.Text, txtParam1.Text, txtParam2.Text, cboPattern.Text, dtpTime.Value.ToString("HH:mm:ss"), Math.Abs(CInt(chkSunday.Checked)), Math.Abs(CInt(chkMonday.Checked)), Math.Abs(CInt(chkTuesday.Checked)), Math.Abs(CInt(chkWednesday.Checked)), Math.Abs(CInt(chkThursday.Checked)), Math.Abs(CInt(chkFriday.Checked)), Math.Abs(CInt(chkSaturday.Checked)), "D", nudMinutes.Value, 0, dtpSingle.Value.ToString("yyyy-MM-dd"))
        ElseIf optMonthly.Checked = True Then
            OSAEApi.ScheduleRecurringUpdate(dgvRecurring("schedule_name1", dgvRecurring.CurrentCell.RowIndex).Value, txtName.Text, cboObject.Text, cboMethod.Text, txtParam1.Text, txtParam2.Text, cboPattern.Text, dtpTime.Value.ToString("HH:mm:ss"), Math.Abs(CInt(chkSunday.Checked)), Math.Abs(CInt(chkMonday.Checked)), Math.Abs(CInt(chkTuesday.Checked)), Math.Abs(CInt(chkWednesday.Checked)), Math.Abs(CInt(chkThursday.Checked)), Math.Abs(CInt(chkFriday.Checked)), Math.Abs(CInt(chkSaturday.Checked)), "M", nudMinutes.Value, cboDay.Text, dtpSingle.Value.ToString("yyyy-MM-dd"))
        ElseIf optAnnual.Checked = True Then
            OSAEApi.ScheduleRecurringUpdate(dgvRecurring("schedule_name1", dgvRecurring.CurrentCell.RowIndex).Value, txtName.Text, cboObject.Text, cboMethod.Text, txtParam1.Text, txtParam2.Text, cboPattern.Text, dtpTime.Value.ToString("HH:mm:ss"), Math.Abs(CInt(chkSunday.Checked)), Math.Abs(CInt(chkMonday.Checked)), Math.Abs(CInt(chkTuesday.Checked)), Math.Abs(CInt(chkWednesday.Checked)), Math.Abs(CInt(chkThursday.Checked)), Math.Abs(CInt(chkFriday.Checked)), Math.Abs(CInt(chkSaturday.Checked)), "Y", nudMinutes.Value, cboDay.Text, dtpSingle.Value.ToString("yyyy-MM-dd"))
        End If
        Load_Queue()
        Load_Recurring()
    End Sub

    Private Sub txtName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtName.TextChanged
        Validate_Save()
    End Sub

    Private Sub cboMethod_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboMethod.SelectedIndexChanged
        Validate_Save()
    End Sub

    Private Sub cboPattern_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboPattern.SelectedIndexChanged
        Validate_Save()
    End Sub

    Private Sub dgvQueue_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvQueue.CurrentCellChanged
        btnQueueDelete.Enabled = True
    End Sub

    Private Sub dgvQueue_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvQueue.DoubleClick
        Load_Queue()
        Load_Recurring()
    End Sub

    Private Sub dgvRecurring_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvRecurring.CurrentCellChanged
        Try
            btnRecurringDelete.Enabled = True
            txtName.Text = dgvRecurring("schedule_name1", dgvRecurring.CurrentCell.RowIndex).Value
            Dim CMD As New MySqlCommand
            Dim myReader As MySqlDataReader
            Dim sInterval As String = ""
            Dim sPattern As String = "", tTime As DateTime, sObject As String, sMethod As String, sParam1 As String, sParam2 As String
            CMD.Connection = CN
            CMD.CommandType = CommandType.Text
            CMD.CommandText = "SELECT * FROM osae_v_schedule_recurring WHERE schedule_name=?ScheduleName"
            CMD.Parameters.AddWithValue("?ScheduleName", dgvRecurring("schedule_name1", dgvRecurring.CurrentCell.RowIndex).Value)
            Try
                CN.Open()
                myReader = CMD.ExecuteReader
                While myReader.Read
                    sInterval = Convert.ToString(myReader.Item("interval_unit"))
                    Select Case sInterval
                        Case "D"
                            optDaily.Checked = True
                            chkSunday.Checked = Convert.ToInt32(myReader.Item("sunday"))
                            chkMonday.Checked = Convert.ToInt32(myReader.Item("monday"))
                            chkTuesday.Checked = Convert.ToInt32(myReader.Item("tuesday"))
                            chkWednesday.Checked = Convert.ToInt32(myReader.Item("wednesday"))
                            chkThursday.Checked = Convert.ToInt32(myReader.Item("thursday"))
                            chkFriday.Checked = Convert.ToInt32(myReader.Item("friday"))
                            chkSaturday.Checked = Convert.ToInt32(myReader.Item("saturday"))
                        Case "M"
                            optMonthly.Checked = True
                            cboDay.Text = Convert.ToString(myReader.Item("recurring_day"))
                        Case "Y"
                            optAnnual.Checked = True
                            dtpAnnual.Value = Convert.ToDateTime(myReader.Item("recurring_date"))
                    End Select
                    sPattern = Convert.ToString(myReader.Item("pattern"))
                    sObject = Convert.ToString(myReader.Item("object_name"))
                    sMethod = Convert.ToString(myReader.Item("method_name"))
                    sParam1 = Convert.ToString(myReader.Item("parameter_1"))
                    sParam2 = Convert.ToString(myReader.Item("parameter_2"))
                    tTime = Now.Date & " " & Convert.ToString(myReader.Item("recurring_time"))
                    dtpTime.Value = tTime

                End While
                CN.Close()
                If sPattern.Length > 0 Then
                    cboPattern.Text = sPattern
                    radScript.Checked = True
                Else
                    chkMethod.Checked = True
                    cboObject.Text = sObject
                    cboMethod.Text = sMethod
                    txtParam1.Text = sParam1
                    txtParam2.Text = sParam2
                End If
            Catch ex As Exception
                MessageBox.Show("Error Load_Object_Types: " & ex.Message, "Scheduling")
                CN.Close()
            End Try
        Catch
        End Try
    End Sub

    Private Sub chkMethod_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkMethod.CheckedChanged
        If chkMethod.Checked Then
            cboObject.Enabled = True
            cboMethod.Enabled = True
            txtParam1.Enabled = True
            txtParam2.Enabled = True
            cboPattern.Text = ""
            cboPattern.SelectedIndex = -1
            cboPattern.Enabled = False
        End If
    End Sub

    Private Sub radScript_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radScript.CheckedChanged
        If radScript.Checked Then
            cboObject.SelectedIndex = -1
            cboObject.Enabled = False

            cboMethod.Text = ""
            cboMethod.SelectedIndex = -1
            cboMethod.Enabled = False

            txtParam1.Text = ""
            txtParam1.Enabled = False
            txtParam2.Text = ""
            txtParam2.Enabled = False
            cboPattern.Enabled = True
        End If
    End Sub

    Private Sub dgvRecurring_CellContentClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvRecurring.CellContentClick
        Rad_Changed()
    End Sub

    Private Sub dtpTime_ValueChanged(sender As System.Object, e As System.EventArgs) Handles dtpTime.ValueChanged
        Rad_Changed()
    End Sub

    Private Sub optMonthly_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles optMonthly.CheckedChanged
        Rad_Changed()
    End Sub

    Private Sub optDaily_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles optDaily.CheckedChanged
        Rad_Changed()
    End Sub

    Private Sub optSingle_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles optSingle.CheckedChanged
        Rad_Changed()
    End Sub

    Private Sub optMinutes_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles optMinutes.CheckedChanged
        Rad_Changed()
    End Sub

    Private Sub optAnnual_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles optAnnual.CheckedChanged
        Rad_Changed()
    End Sub

    Private Sub Rad_Changed()
        If optSingle.Checked Then
            dtpSingle.Enabled = True
        Else
            dtpSingle.Enabled = False
        End If

        If optMinutes.Checked Then
            nudMinutes.Enabled = True
        Else
            nudMinutes.Enabled = False
        End If

        If optDaily.Checked Then
            chkSunday.Enabled = True
            chkMonday.Enabled = True
            chkTuesday.Enabled = True
            chkWednesday.Enabled = True
            chkThursday.Enabled = True
            chkFriday.Enabled = True
            chkSaturday.Enabled = True
        Else
            chkSunday.Enabled = False
            chkMonday.Enabled = False
            chkTuesday.Enabled = False
            chkWednesday.Enabled = False
            chkThursday.Enabled = False
            chkFriday.Enabled = False
            chkSaturday.Enabled = False
        End If


        If optMonthly.Checked Then
            cboDay.Enabled = True
        Else
            cboDay.Enabled = False
        End If

        If optAnnual.Checked Then
            dtpAnnual.Enabled = True
        Else
            dtpAnnual.Enabled = False
        End If

    End Sub
End Class