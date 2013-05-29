Imports MySql.Data.MySqlClient
Public Class frmScripts
    Private CN As MySqlConnection
    Private Sub Patterns_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DB_Connection()
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
    Public Sub Load_Patterns()
        Dim MyDT As New DataTable
        Dim MyDA As New MySqlDataAdapter("SELECT pattern FROM osae_pattern ORDER BY pattern", CN)
        Dim iRowHolder As Integer
        txtPattern.Text = ""
        btnDelete.Enabled = False
        If dgvScripts.RowCount > 0 Then
            iRowHolder = dgvScripts.CurrentCell.RowIndex
        End If
        MyDA.Fill(MyDT)
        dgvScripts.DataSource = MyDT
    End Sub
    Public Sub Load_Matches()
        Try
            dgvItems.Refresh()
            Dim MyDT As New DataTable
            Dim MyDA As New MySqlDataAdapter("SELECT `match` FROM osae_v_pattern WHERE pattern='" & dgvScripts("pattern", dgvScripts.CurrentCell.RowIndex).Value & "' ORDER BY `match`", CN)
            txtMatch.Text = ""
            btnMatchDelete.Enabled = False
            MyDA.Fill(MyDT)
            dgvItems.DataSource = MyDT
        Catch ex As Exception
        End Try
    End Sub
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_pattern_add"
        CMD.Parameters.AddWithValue("?pname", txtPattern.Text)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error btnAdd_Click: " & myerror.Message)
            CN.Close()
        End Try
        Load_Patterns()
    End Sub
    Private Sub btnMatchAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMatchAdd.Click
        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_pattern_match_add"
        CMD.Parameters.AddWithValue("?ppattern", dgvScripts("pattern", dgvScripts.CurrentCell.RowIndex).Value)
        CMD.Parameters.AddWithValue("?pname", txtMatch.Text)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error btnMatchAdd_Click: " & myerror.Message)
            CN.Close()
        End Try
        Load_Matches()
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_pattern_match_update"
        CMD.Parameters.AddWithValue("?ppattern", dgvScripts("pattern", dgvScripts.CurrentCell.RowIndex).Value)
        CMD.Parameters.AddWithValue("?pname", txtMatch.Text)

        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error btnMatchAdd_Click: " & myerror.Message)
            CN.Close()
        End Try
        Load_Matches()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_pattern_delete"
        CMD.Parameters.AddWithValue("?ppattern", dgvScripts("pattern", dgvScripts.CurrentCell.RowIndex).Value)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error btnDelete_Click: " & myerror.Message)
            CN.Close()
        End Try
        Load_Patterns()
    End Sub

    Public Sub Validate_Pattern()
        Dim CMD As New MySqlCommand
        'Dim myReader As MySqlDataReader
        Dim iCount As Integer
        If txtPattern.Text.Length = 0 Then
            btnAdd.Enabled = False
            Exit Sub
        End If
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT count(pattern) as Results FROM osae_pattern WHERE pattern=?pname"
        CMD.Parameters.AddWithValue("?pname", txtPattern.Text)
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
            btnAdd.Enabled = True
            btnUpdate.Enabled = True
        Else
            btnAdd.Enabled = False
            btnUpdate.Enabled = False
        End If
    End Sub
    Public Sub Validate_Item()
        Dim CMD As New MySqlCommand
        'Dim myReader As MySqlDataReader
        Dim iCount As Integer
        If txtMatch.Text.Length = 0 Then
            btnMatchAdd.Enabled = False
            Exit Sub
        End If
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT count(`match`) as Results FROM osae_v_pattern WHERE `match`=?pname"
        CMD.Parameters.AddWithValue("?pname", txtMatch.Text)
        Try
            CN.Open()
            iCount = CMD.ExecuteScalar
            CN.Close()
            CMD.Parameters.Clear()
        Catch myerror As MySqlException
            MessageBox.Show("Error Validate_Item: " & myerror.Message)
            CN.Close()
        End Try
        If iCount = 0 Then
            btnMatchAdd.Enabled = True
            btnMatchUpdate.Enabled = True
        Else
            btnMatchAdd.Enabled = False
            btnMatchUpdate.Enabled = False
        End If
    End Sub

    Private Sub txtPattern_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPattern.TextChanged
        Validate_Pattern()
    End Sub

    Private Sub txtMatch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtMatch.TextChanged
        Validate_Item()
    End Sub



    Private Sub dgvPatterns_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvScripts.CurrentCellChanged
        Try
            btnDelete.Enabled = True
            txtPattern.Text = "" & dgvScripts("pattern", dgvScripts.CurrentCell.RowIndex).Value
        Catch ex As Exception
        End Try
        Load_Matches()
    End Sub


    Private Sub dgvItems_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvItems.CurrentCellChanged
        Try
            txtMatch.Text = "" & dgvItems("match", dgvItems.CurrentCell.RowIndex).Value
            btnMatchDelete.Enabled = True
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnMatchDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMatchDelete.Click
        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_pattern_match_delete"
        CMD.Parameters.AddWithValue("?pname", dgvItems("match", dgvItems.CurrentCell.RowIndex).Value)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error btnMatchDelete_Click: " & myerror.Message)
            CN.Close()
        End Try
        Load_Matches()
    End Sub

    Private Sub btnEditScript_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditScript.Click
        frmScriptEditor.Show()
        frmScriptEditor.radPattern.Checked = True
        frmScriptEditor.cboPatterns.Text = dgvScripts("pattern", dgvScripts.CurrentCell.RowIndex).Value
    End Sub
End Class