Imports MySql.Data.MySqlClient
Public Class frmPropertyList
    Public iObject As String
    Public iProperty As String
    Private CN As MySqlConnection

    Private Sub PropertyList_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lblObject.Text = iObject
        lblProperty.Text = iProperty
        ' OSAEApi = New OSAE.OSAE("Dev GUI")
        DB_Connection()
        Load_Grid()
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

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub Load_Grid()
        Dim MyDT As New DataTable
        Dim MyDA As New MySqlDataAdapter("SELECT item_name,item_label FROM osae_v_object_property_array WHERE object_name='" & lblObject.Text & "' AND property_name='" & lblProperty.Text & "'", CN)
        Dim iRowHolder As Integer
        txtValue.Text = ""
        btnDelete.Visible = False
        If dvgPropertyList.RowCount > 0 Then
            iRowHolder = dvgPropertyList.CurrentCell.RowIndex
        End If
        MyDA.Fill(MyDT)
        dvgPropertyList.DataSource = MyDT
        ' dvgPropertyList.CurrentCell = dvgPropertyList.Rows(iRowHolder).Cells("item_name")
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        OSAEApi.ObjectPropertyArrayAdd(lblObject.Text, lblProperty.Text, txtValue.Text, txtLabel.Text)
        Load_Grid()
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_object_property_array_update"
        CMD.Parameters.AddWithValue("?pobject", lblObject.Text)
        CMD.Parameters.AddWithValue("?pproperty", lblProperty.Text)
        CMD.Parameters.AddWithValue("?polditem", dvgPropertyList("item_name", dvgPropertyList.CurrentCell.RowIndex).Value)
        CMD.Parameters.AddWithValue("?pnewitem", txtValue.Text)
        CMD.Parameters.AddWithValue("?pnewlabel", txtLabel.Text)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error btnAdd_Click: " & myerror.Message, "Property List")
            CN.Close()
        End Try
        Load_Grid()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        OSAEApi.ObjectPropertyArrayDelete(lblObject.Text, lblProperty.Text, txtValue.Text)
        Load_Grid()
    End Sub

    Private Sub dvgPropertyList_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvgPropertyList.CurrentCellChanged
        Try
            txtValue.Text = "" & dvgPropertyList("item_name", dvgPropertyList.CurrentCell.RowIndex).Value
            txtLabel.Text = "" & dvgPropertyList("item_label", dvgPropertyList.CurrentCell.RowIndex).Value
            btnDelete.Visible = True
        Catch ex As Exception
            btnDelete.Visible = False
        End Try
    End Sub

    Private Sub dvgPropertyList_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dvgPropertyList.CellContentClick
        Validate_Add()
    End Sub

    Private Sub txtValue_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtValue.TextChanged
        Validate_Add()
    End Sub

    Private Sub txtLabel_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtLabel.TextChanged
        Validate_Add()
    End Sub

    Private Sub Validate_Add()
        If txtValue.TextLength = 0 Then
            btnAdd.Visible = False
            btnUpdate.Visible = False
        Else
            btnAdd.Visible = True
            btnUpdate.Visible = True
        End If
    End Sub
End Class