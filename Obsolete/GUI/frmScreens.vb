Imports MySql.Data.MySqlClient
Public Class frmScreens
    Private CN As MySqlConnection
    Private Sub Screens_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DB_Connection()
        Load_Objects()
        Load_Controls()
        Load_Screens()
    End Sub
    Public Sub DB_Connection()
        CN = New MySqlConnection
        CN.ConnectionString = "server=" & OSAEApi.DBConnection & ";Port=" & OSAEApi.DBPort & ";Database=" & OSAEApi.DBName & ";Password=" & OSAEApi.DBPassword & ";use procedure bodies=false;Persist Security Info=True;User ID=" & OSAEApi.DBUsername
        Try
            CN.Open()
            CN.Close()
            logging.AddToLog("Connected to Database: " & OSAEApi.DBName & " @ " & OSAEApi.DBConnection & ":" & OSAEApi.DBPort, True)
        Catch myerror As MySqlException
            logging.AddToLog("Error Connecting to Database: " & myerror.Message, True)
        End Try
    End Sub
    Public Sub Load_Screens()
        Dim MyDT As New DataTable
        Dim MyDA As New MySqlDataAdapter("SELECT object_name FROM osae_v_object WHERE base_type='SCREEN' ORDER BY object_name", CN)
        Dim iRowHolder As Integer
        If dgvScreens.RowCount > 0 Then
            iRowHolder = dgvScreens.CurrentCell.RowIndex
        End If
        MyDA.Fill(MyDT)
        dgvScreens.DataSource = MyDT
        If dgvScreens.RowCount > 0 Then
            dgvScreens.CurrentCell = dgvScreens.Rows(iRowHolder).Cells("screen_name")
        End If

    End Sub
    Public Sub Load_Controls()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_type FROM osae_v_object_type WHERE base_type='CONTROL' ORDER BY object_type"
        Try
            comControls.Items.Clear()
            comControls.Text = ""
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                comControls.Items.Add(Convert.ToString(myReader.Item("object_type")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Controls: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
    Public Sub Load_Objects()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE base_type<>'SCREEN' ORDER BY object_name"
        Try
            comObjects.Items.Clear()
            comObjects.Text = ""
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                comObjects.Items.Add(Convert.ToString(myReader.Item("object_name")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Objects: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
    Public Sub Load_Screen_Controls()
        Try
            Dim MyDT As New DataTable
            Dim MyDA As New MySqlDataAdapter("SELECT object_name,control_name FROM osae_v_screen_object WHERE screen_name='" & dgvScreens("screen_name", dgvScreens.CurrentCell.RowIndex).Value & "' ORDER BY object_name,control_name", CN)
            'Dim iRowHolder As Integer
            ' If dgvScreens.RowCount > 0 Then
            'iRowHolder = dgvScreens.CurrentCell.RowIndex
            ' End If
            MyDA.Fill(MyDT)
            dgvControls.DataSource = MyDT
            'dgvControls.CurrentCell = dgvScreens.Rows(iRowHolder).Cells("control_name")
        Catch ex As Exception

        End Try
    End Sub
    Private Sub dgvScreens_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvScreens.CurrentCellChanged
        Load_Screen_Controls()
    End Sub

    Private Sub comObjects_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comObjects.SelectedIndexChanged
        Validate_Screen_Controls()
    End Sub

    Private Sub comControls_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comControls.SelectedIndexChanged
        Validate_Screen_Controls()
    End Sub
    Public Sub Validate_Screen_Controls()
        Try

        Catch ex As Exception

        End Try
    End Sub

    'Public Overloads Sub Show()
    '    If dgvScreens.RowCount > 0 Then
    '        MyBase.Show()
    '    Else
    '        MessageBox.Show("You have no Screens setup.  Please Add a Screen.")
    '    End If
    'End Sub
End Class