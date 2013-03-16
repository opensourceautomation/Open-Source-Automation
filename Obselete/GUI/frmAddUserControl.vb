Imports MySql.Data.MySqlClient
Public Class frmAddUserControl

    Private Sub frmAddUserControl_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Load_Objects()
    End Sub
    Private Sub Load_Objects()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_type FROM osae_v_object_type WHERE base_type='USER CONTROL' ORDER BY object_type"
        Try
            cboObjects.Items.Clear()
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboObjects.Items.Add(Convert.ToString(myReader.Item("object_type")))
                txtName.Text = gCurrentScreen & " - " & cboObjects.Text
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Objects: " & myerror.Message, "Add User Control")
            CN.Close()
        End Try
    End Sub

    Private Sub cboObjects_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboObjects.SelectedIndexChanged
        If cboObjects.SelectedIndex >= 0 Then
            btnAdd.Enabled = True
            txtName.Text = gCurrentScreen & " - " & cboObjects.Text
        Else
            btnAdd.Enabled = False
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        If IsNothing(gCurrentScreen) Then
            MessageBox.Show("You must add a screen first!  Closing Form", "Add User Control")
            Me.Close()
            Me.Dispose()
        End If
        OSAEApi.ObjectAdd(txtName.Text, txtName.Text, "USER CONTROL", "", gCurrentScreen, True)
        OSAEApi.ObjectPropertySet(txtName.Text, "Control Type", cboObjects.Text)
        OSAEApi.ObjectPropertySet(txtName.Text, "X", 100)
        OSAEApi.ObjectPropertySet(txtName.Text, "Y", 100)
        OSAEApi.ObjectPropertySet(txtName.Text, "ZOrder", 5)

        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_screen_object_add"
        CMD.Parameters.AddWithValue("?screen", gCurrentScreen)
        CMD.Parameters.AddWithValue("?object", txtName.Text)
        CMD.Parameters.AddWithValue("?control", txtName.Text)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            '    sStateName = CMD.ExecuteScalar
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Add_Object: " & myerror.Message)
            CN.Close()
        End Try
        GUI.Load_Screen(gCurrentScreen)
        Me.Close()
    End Sub
End Class