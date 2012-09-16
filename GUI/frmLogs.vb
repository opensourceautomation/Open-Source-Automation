Imports MySql.Data.MySqlClient
Public Class frmLogs
    Private CN As MySqlConnection
    Private Sub logs_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DB_Connection()
        Load_Logs()
        Timer1.Enabled = True

    End Sub
    Public Sub DB_Connection()
        CN = New MySqlConnection
        CN.ConnectionString = "server=" & OSAEApi.DBConnection & ";Port=" & OSAEApi.DBPort & ";Database=" & OSAEApi.DBName & ";Password=" & OSAEApi.DBPassword & ";use procedure bodies=false;Persist Security Info=True;User ID=" & OSAEApi.DBUsername
        Try
            CN.Open()
            CN.Close()
            'Update_Action("Connected to " & vCrib.Database_Name & " database @ " & vCrib.Database_IP & ":" & vCrib.Database_Port)
        Catch myerror As Exception
            'Update_Action("ERROR Connecting to " & vCrib.Database_Name & " database.")
        End Try
    End Sub
    Public Sub Load_Logs()
        Dim MyDT As New DataTable
        Dim MyDA As New MySqlDataAdapter("SELECT log_time,object_name,event_label,parameter_1,parameter_2,from_object_name FROM osae_v_event_log ORDER BY log_time DESC, object_name LIMIT 1000", CN)
        Dim iRowHolder As Integer
        Dim iScrollbarPos As Integer
        MyDA.Fill(MyDT)

        If dgvEvent_Log.RowCount > 0 Then
            iRowHolder = dgvEvent_Log.CurrentCell.RowIndex
            iScrollbarPos = dgvEvent_Log.FirstDisplayedScrollingRowIndex
        End If
        dgvEvent_Log.DataSource = MyDT
        If dgvEvent_Log.RowCount > 0 Then
            dgvEvent_Log.CurrentCell = dgvEvent_Log.Rows(iRowHolder).Cells(1)
            dgvEvent_Log.FirstDisplayedScrollingRowIndex = iScrollbarPos
        End If
    End Sub
    Private Sub ButClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButClose.Click
        Me.Close()
    End Sub
    Private Sub butClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butClear.Click
        OSAEApi.EventLogClear()
        Load_Logs()
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Load_Logs()
    End Sub

    Private Sub dgvEvent_Log_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvEvent_Log.CellContentClick

    End Sub

    Private Sub dgvEvent_Log_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvEvent_Log.MouseEnter
        Timer1.Enabled = False
    End Sub

    Private Sub dgvEvent_Log_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvEvent_Log.MouseLeave
        Timer1.Enabled = True
    End Sub
End Class